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
    /// Логика взаимодействия для CargoTypesTablePageModal.xaml
    /// </summary>
    public partial class CargoTypesTablePageModal : UserControl
    {
        public StartWindow startWindow;
        public CargoTypesObject data = new CargoTypesObject();
        public byte mode = 0;
        public CargoTypesTablePageModal()
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

        public void UpdateDisplayedData(CargoTypesObject data)
        {
            this.data = data;
            NameTextBox.Text = data.Name.ToString();

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }

        private async void UpdateData()
        {
            try
            {
                var reqResult = new CargoTypesObject();
                if (mode == 0)
                    reqResult = await startWindow.client.UpdateCargoTypeAsync(new CreateOrUpdateCargoTypesRequest { CargoType = data });
                if (mode == 1)
                    reqResult = await startWindow.client.CreateCargoTypeAsync(new CreateOrUpdateCargoTypesRequest { CargoType = data });

                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as CargoTypesPage;
                if (mode == 0)
                {
                    var index = page.CargoTypes.FindIndex(t => t.Id == reqResult.Id);
                    page.CargoTypes[index] = reqResult;
                }
                if (mode == 1)
                    page.CargoTypes.Add(reqResult);
                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.CargoTypes;
                page.dataGrid.Items.Refresh();
            }
            catch (RpcException ex)
            {

            }

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder changedDataNotify = new StringBuilder();
            if (mode == 0)
            {
                if (NameTextBox.Text != data.Name)
                    changedDataNotify.Append($"Серия: {data.Name} -> {NameTextBox.Text}\n");
            }

            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                data.Name = NameTextBox.Text;
                UpdateData();
            }
        }
    }
}
