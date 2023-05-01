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
using static LogisticsClientsApp.Pages.Tables.DriverLicenceTablePage;

namespace LogisticsClientsApp.Pages.Modal
{
    /// <summary>
    /// Логика взаимодействия для DriversTablePageModal.xaml
    /// </summary>
    public partial class DriversTablePageModal : UserControl
    {
        public DriversObject data;
        public ListDriverLicence licenses;
        private Locale locale;

        StartWindow startWindow;

        public DriversTablePageModal()
        {
            InitializeComponent();
        }

        private void ModalPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            Locale locale = new Locale(startWindow.selectedLocale);
            locale.SetLocale(this);
            SetDriverLicences();
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

        public async void SetDriverLicences()
        {
            licenses = await startWindow.client.GetListDriverLicencesAsync(new Google.Protobuf.WellKnownTypes.Empty());
            List<string> stringLicenses = new List<string>();
            licenses.DriverLicence.ToList().ForEach(item => stringLicenses.Add($"{item.Series}/{item.Number}"));
            LicenceComboBox.ItemsSource = stringLicenses;
            LicenceComboBox.SelectedItem = ($"{data.Licence.Series}/{data.Licence.Number}");
        }

        public void UpdateDisplayedData(DriversObject data)
        {
            this.data = data;
            NameTextBox.Text = data.Name.ToString();
            SurnameTextBox.Text = data.Surname.ToString();
            PatrTextBox.Text = data.Patronymic.ToString();
            SanCheckBox.IsChecked = data.Sanitation;
            if (startWindow != null)
                SetDriverLicences();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }

        DriverLicenceObject foundedData;

        private void LicenceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (sender as ComboBox);
            var number = comboBox.SelectedItem.ToString();
            var numberSeries = number.Split("/");
            foundedData = licenses.DriverLicence.First(item => item.Series == int.Parse(numberSeries[0].ToString()) && item.Number == int.Parse(numberSeries[1].ToString()));
        }

        private async void UpdateData()
        {
            try
            {
                var reqResult = await startWindow.client.UpdateDriverAsync(new CreateOrUpdateDriversRequest { Driver = data });
                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as DriversTablePage;
                var index = page.Drivers.FindIndex(t => t.Id == reqResult.Id);
                page.Drivers[index] = reqResult;
                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.Drivers;
            }
            catch (RpcException ex)
            {

            }

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder changedDataNotify = new StringBuilder();

            if (NameTextBox.Text != data.Name.ToString())
                changedDataNotify.Append($"Название: {data.Name} -> {NameTextBox.Text}\n");
            if (SurnameTextBox.Text != data.Surname.ToString())
                changedDataNotify.Append($"Владелец: {data.Surname} -> {SurnameTextBox.Text}\n");
            if (PatrTextBox.Text != data.Patronymic)
                changedDataNotify.Append($"ИНН: {data.Patronymic} -> {SurnameTextBox.Text}\n");
            if (SanCheckBox.IsChecked != data.Sanitation)
                changedDataNotify.Append($"Юр. адрес: {data.Sanitation} -> {SanCheckBox.IsChecked}\n");
            if (foundedData.Id != data.Licence.Id)
                changedDataNotify.Append($"Роль: {data.Licence.Series}/{data.Licence.Number} -> {foundedData.Series}/{foundedData.Number}\n");

            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                data.Name = NameTextBox.Text;
                data.Surname = SurnameTextBox.Text;
                data.Patronymic = PatrTextBox.Text;
                data.Sanitation = (bool)SanCheckBox.IsChecked;
                data.Licence = foundedData;
                UpdateData();
            }
        }
    }
}
