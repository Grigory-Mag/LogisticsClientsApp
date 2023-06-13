using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages.Tables;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
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
    /// Логика взаимодействия для DriverLicenceTablePageModal.xaml
    /// </summary>
    public partial class DriverLicenceTablePageModal : UserControl
    {
        public StartWindow startWindow;
        public DriversLicenceReady data = new DriversLicenceReady();
        public byte mode = 0;
        public string text = "Обновить";
        public DriverLicenceTablePageModal()
        {
            InitializeComponent();
        }

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

        private void ModalPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
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

        public void UpdateDisplayedData(DriversLicenceReady data)
        {
            this.data = data;
            SeriesTextBox.Text = data.Series.ToString();
            NumberTextBox.Text = data.Number.ToString();
            DatePicker.SelectedDate = data.Date;

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }

        private async void UpdateData()
        {
            try
            {
                var reqResult = new DriverLicenceObject();
                if (mode == 0)
                    reqResult = await startWindow.client.UpdateDriverLicenceAsync(new CreateOrUpdateDriverLicenceRequest { DriverLicence = new DriverLicenceObject { Id = (int)data.Id, Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(data.Date.ToUniversalTime()), Number = data.Number, Series = data.Series } }, startWindow.headers);
                if (mode == 1)
                    reqResult = await startWindow.client.CreateDriverLicenceAsync(new CreateOrUpdateDriverLicenceRequest { DriverLicence = new DriverLicenceObject { Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(data.Date.ToUniversalTime()), Number = data.Number, Series = data.Series } }, startWindow.headers);
                
                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as DriverLicenceTablePage;
                if (mode == 0)
                {                    
                    var index = page.DriversLicence.FindIndex(t => t.Id == reqResult.Id);
                    page.DriversLicence[index] = reqResult;
                }
                if (mode == 1)
                    page.DriversLicence.Add(reqResult);

                List<DriversLicenceReady> driversLicenceReadies = new List<DriversLicenceReady>();
                page.DriversLicence.ForEach(license => driversLicenceReadies.Add(new DriversLicenceReady(license.Id, license.Series, license.Number, license.Date)));
                page.DriversLicenceOriginal = driversLicenceReadies;
                page.DriversLicenceReadies = page.DriversLicenceOriginal;

                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.DriversLicenceOriginal.Skip(page.skipPages).Take(page.takePages);
                page.dataGrid.Items.Refresh();
                page.PaginationTextBlock.Text = $"{page.skipPages + 10} из {page.DriversLicenceOriginal.Count}";

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
                if (SeriesTextBox.Text != data.Series.ToString())
                    changedDataNotify.Append($"Серия: {data.Series} -> {SeriesTextBox.Text}\n");
                if (NumberTextBox.Text != data.Number.ToString())
                    changedDataNotify.Append($"Номер: {data.Number} -> {NumberTextBox.Text}\n");
                if (DatePicker.SelectedDate.Value.Date != data.Date.Date)
                    changedDataNotify.Append($"Дата выдачи: {data.Date.Date.ToShortDateString()} -> {DatePicker.SelectedDate.Value.Date.ToShortDateString()}\n");
            }

            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", $"{text}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    data.Number = int.Parse(NumberTextBox.Text);
                    data.Series = int.Parse(SeriesTextBox.Text);
                    data.Date = DatePicker.SelectedDate!.Value.AddDays(1);
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
