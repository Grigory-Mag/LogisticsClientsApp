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
    /// Логика взаимодействия для RequisitesTypeTablePageModal.xaml
    /// </summary>
    public partial class RequisitesTypeTablePageModal : UserControl
    {
        public RequisiteTypeObject data = new RequisiteTypeObject();
        public ListRoles roles;
        private Locale locale;
        public byte mode = 0;
        StartWindow startWindow;

        public RequisitesTypeTablePageModal()
        {
            InitializeComponent();
        }


        private void ModalPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            Locale locale = new Locale(startWindow.selectedLocale);
            locale.SetLocale(this);
            //SetRoles();
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

        public void UpdateDisplayedData(RequisiteTypeObject data)
        {
            this.data = data;
            NameTextBox.Text = data.Name.ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }

        private void LicenceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (sender as ComboBox);
            //var number = comboBox.SelectedItem.ToString();
            //var numberSeries = number.Split("/");
            //var foundedData = licenses.DriverLicence.Where(item => item.Series == int.Parse(numberSeries[0].ToString()) && item.Number == int.Parse(numberSeries[1].ToString())).ToList();
        }

        private async void UpdateData()
        {
            try
            {
                RequisiteTypeObject reqResult = new RequisiteTypeObject();
                if (mode == 0)
                    reqResult = await startWindow.client.UpdateRequisiteTypeAsync(new CreateOrUpdateRequisiteTypeRequest { RequisiteType = data }, startWindow.headers);
                if (mode == 1)
                    reqResult = await startWindow.client.CreateRequisiteTypeAsync(new CreateOrUpdateRequisiteTypeRequest { RequisiteType = data }, startWindow.headers);

                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as RequisiteTypesTablePage;
                if (mode == 0)
                {
                    var index = page.RequisitesTypes.FindIndex(t => t.Id == reqResult.Id);
                    page.RequisitesTypes[index] = reqResult;
                }
                if (mode == 1)
                    page.RequisitesTypes.Add(reqResult);

                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.RequisitesTypes;
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
                if (NameTextBox.Text != data.Name.ToString())
                    changedDataNotify.Append($"Название: {data.Name} -> {NameTextBox.Text}\n");
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
