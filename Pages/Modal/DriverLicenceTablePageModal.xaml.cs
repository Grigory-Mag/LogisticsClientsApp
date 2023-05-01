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
    /// Логика взаимодействия для DriverLicenceTablePageModal.xaml
    /// </summary>
    public partial class DriverLicenceTablePageModal : UserControl
    {
        public StartWindow startWindow;
        public DriversLicenceReady data;

        public DriverLicenceTablePageModal()
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
                var reqResult = await startWindow.client.UpdateDriverLicenceAsync(new CreateOrUpdateDriverLicenceRequest { DriverLicence = new DriverLicenceObject { Id = (int)data.Id, Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(data.Date.ToUniversalTime()), Number = data.Number, Series = data.Series } });
                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as DriverLicenceTablePage;
                var index = page.DriversLicence.FindIndex(t => t.Id == reqResult.Id);
                page.DriversLicence[index] = reqResult;
                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.DriversLicence;
                page.dataGrid.Items.Refresh();
            }
            catch (RpcException ex)
            {

            }

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder changedDataNotify = new StringBuilder();
            
            if (SeriesTextBox.Text != data.Series.ToString())
                changedDataNotify.Append($"Серия: {data.Series} -> {SeriesTextBox.Text}\n");
            if (NumberTextBox.Text != data.Number.ToString())
                changedDataNotify.Append($"Номер: {data.Number} -> {NumberTextBox.Text}\n");
            if (DatePicker.SelectedDate != data.Date)
                changedDataNotify.Append($"Дата выдачи: {data.Date} -> {DatePicker.SelectedDate}\n");
            

            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                data.Number = int.Parse(NumberTextBox.Text);
                data.Series = int.Parse(SeriesTextBox.Text);
                data.Date = DatePicker.SelectedDate!.Value.AddDays(1);
                UpdateData();
            }
        }
    }
}
