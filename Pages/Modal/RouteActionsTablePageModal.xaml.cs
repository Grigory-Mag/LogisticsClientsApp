using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticsClientsApp.Pages.Modal
{
    /// <summary>
    /// Логика взаимодействия для RouteActionsTablePageModal.xaml
    /// </summary>
    public partial class RouteActionsTablePageModal : UserControl
    {
        public RouteActionsObject data = new RouteActionsObject();

        private Locale locale;
        public byte mode = 0;

        StartWindow startWindow;
        public RouteActionsTablePageModal()
        {
            InitializeComponent();
        }

        private void ModalPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            Locale locale = new Locale(startWindow.selectedLocale);
            locale.SetLocale(this);
        }

        public void CloseAnimation()
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            var tablePage = (TablePage)startWindow.MainFrameK.Content;
            tablePage.MainPanel.Opacity = 1;
            tablePage.MainPanel.IsEnabled = true;

            Storyboard sb = Resources["CloseModal"] as Storyboard;
            sb.Begin(ModalPageControl);
        }
        public void UpdateDisplayedData(RouteActionsObject data)
        {
            this.data = data;
            NameTextBox.Text = data.Action.ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }

        private async void UpdateData()
        {
            try
            {
                var reqResult = new RouteActionsObject();
                if (mode == 0)
                    reqResult = await startWindow.client.UpdateRouteActionAsync(new CreateOrUpdateRouteActionsRequest { RouteAction = data });
                if (mode == 1)
                    reqResult = await startWindow.client.CreateRouteActionAsync(new CreateOrUpdateRouteActionsRequest { RouteAction = data });
                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as RouteActionsTablePage;
                if (mode == 0)
                {
                    var index = page.RouteActionsOriginal.FindIndex(t => t.Id == reqResult.Id);
                    page.RouteActionsOriginal[index] = reqResult;
                }
                if (mode == 1)
                    page.RouteActionsOriginal.Add(reqResult);

                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.RouteActionsOriginal;

                ShowToast(TablePage.Messages.Success);
            }
            catch (RpcException ex)
            {
                ShowToast(TablePage.Messages.Error);
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder changedDataNotify = new StringBuilder();

            if (mode == 0)
            {
                if (NameTextBox.Text != data.Action.ToString())
                    changedDataNotify.Append($"Название: {data.Action} -> {NameTextBox.Text}");
            }

            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    data.Action = NameTextBox.Text;
                    UpdateData();
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        case RpcException:
                            MessageBox.Show($"Возникла ошибка. Обратитесь к администратору\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                        default:
                            MessageBox.Show("Проверьте заполненность всех полей. Удостоверьтесь, что численные значения введены верно", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }
                }
            }
        }

        public void ShowToast(TablePage.Messages result)
        {
            ModalPageFrameNotification.Content = new ToastPage(result);
        }
    }
}
