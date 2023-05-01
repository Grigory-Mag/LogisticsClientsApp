using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticsClientsApp.Pages.Tables
{
    /// <summary>
    /// Логика взаимодействия для DriverLicenceTablePage.xaml
    /// </summary>
    public partial class DriverLicenceTablePage : Page
    {
        public List<DriverLicenceObject> DriversLicence { get; set; }
        private Locale locale;

        StartWindow startWindow;

        public class DriversLicenceReady
        {
            public int? Id { get; set; }
            public int? Series { get; set; }
            public int? Number { get; set; }
            public DateTime Date { get; set; }

            public DriversLicenceReady(int? id, int? series, int? number, Google.Protobuf.WellKnownTypes.Timestamp date)
            {
                this.Id = id;
                this.Series = series;
                this.Number = number;
                this.Date = date.ToDateTime();
            }

            public DriversLicenceReady() { }
        }

        public DriverLicenceTablePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            SetData();
        }

        private void PrevTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                var item = dataGrid.SelectedItem as DriversLicenceReady;
                startWindow.client.DeleteDriverLicenceAsync(new GetOrDeleteDriverLicenceRequest { Id = (int)item.Id });
                DriversLicence.Remove(DriversLicence.First(x=> x.Id == item.Id));

                List<DriversLicenceReady> driversLicenceReadies = new List<DriversLicenceReady>();
                DriversLicence.ForEach(license => driversLicenceReadies.Add(new DriversLicenceReady(license.Id, license.Series, license.Number, license.Date)));

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = driversLicenceReadies;
            }
        }

        private async void SetData()
        {
            try
            {
                var item = await startWindow.client.GetListDriverLicencesAsync(new Google.Protobuf.WellKnownTypes.Empty());
                List<DriversLicenceReady> driversLicenceReadies = new List<DriversLicenceReady>();

                DriversLicence = new List<DriverLicenceObject>();
                DriversLicence.AddRange(item.DriverLicence.ToList());
                DriversLicence.ForEach(license => driversLicenceReadies.Add(new DriversLicenceReady(license.Id, license.Series, license.Number, license.Date)));
                
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = driversLicenceReadies;
                locale.SetLocale(this);
            }
            catch (RpcException ex)
            {
#warning TODO
            }
        }

        private void OpenRowButton_Click(object sender, RoutedEventArgs e)
        {
            TablePage tablePage = (TablePage)startWindow.MainFrameK.Content;
            tablePage.ShowModalPage(0);
        }
    }
}
