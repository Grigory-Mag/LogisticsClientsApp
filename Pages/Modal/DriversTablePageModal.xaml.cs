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
using static System.Net.Mime.MediaTypeNames;

namespace LogisticsClientsApp.Pages.Modal
{
    /// <summary>
    /// Логика взаимодействия для DriversTablePageModal.xaml
    /// </summary>
    public partial class DriversTablePageModal : UserControl
    {
        public DriversObject data = new DriversObject();
        public ListDriverLicence licenses;
        private Locale locale;
        public string text = "Обновить";
        public byte mode = 0;
        StartWindow startWindow;

        public void SetMode(byte mode)
        {
            this.mode = mode;
            if (mode == 0)
            {
                UpdateButton.Content = "обновить";
                text = "Обновить";
            }
            else
            {
                UpdateButton.Content = "добавить";
                text = "Добавить";
            }
        }

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
            licenses = await startWindow.client.GetListDriverLicencesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            List<string> stringLicenses = new List<string>();
            licenses.DriverLicence.ToList().ForEach(item => stringLicenses.Add($"{item.Series}/{item.Number}"));
            LicenceComboBox.ItemsSource = stringLicenses;
            if (data.Licence != null)
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
                var reqResult = new DriversObject();
                if (mode == 0)
                    reqResult = await startWindow.client.UpdateDriverAsync(new CreateOrUpdateDriversRequest { Driver = data }, startWindow.headers);
                if (mode == 1)
                    reqResult = await startWindow.client.CreateDriverAsync(new CreateOrUpdateDriversRequest { Driver = data }, startWindow.headers);
                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as DriversTablePage;
                if (mode == 0)
                {
                    var index = page.DriversOriginal.FindIndex(t => t.Id == reqResult.Id);
                    page.DriversOriginal[index] = reqResult;
                }
                if (mode == 1)
                    page.DriversOriginal.Add(reqResult);

                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.DriversOriginal.Skip(page.skipPages).Take(page.takePages);
                page.PaginationTextBlock.Text = $"{page.skipPages + 10} из {page.DriversOriginal.Count}";

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
            }

            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", $"{text}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    data.Name = NameTextBox.Text;
                    data.Surname = SurnameTextBox.Text;
                    data.Patronymic = PatrTextBox.Text;
                    data.Sanitation = (bool)SanCheckBox.IsChecked;
                    data.Licence = foundedData;
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
