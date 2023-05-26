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
        public List<DriversLicenceReady> DriversLicenceReadies { get; set; }
        public List<DriversLicenceReady> DriversLicenceOriginal { get; set; }

        public int takePages = 10;
        public int skipPages = 0;
        private Locale locale;
        string tableName = "водительские лицензии";

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
            var tablePage = startWindow.MainFrameK.Content as TablePage;
            DriversLicenceOriginal = DriversLicenceReadies = new List<DriversLicenceReady>();

            tablePage.TextBlockTableName.Text = tableName;
            SetData();
            startWindow.SizeChanged += (o, e) =>
            {
                ResizeDataGrid();
            };
        }

        public void FastSearch(string text, string? param)
        {
            if (text != "")
                switch (param)
                {
                    case "Серия/Номер":
                        text = text.Trim();
                        var data = text.Split('/');
                        if (data.Length == 2)
                        {
                            DriversLicenceReadies = DriversLicenceOriginal
                                        .Where(x => x.Series.ToString()!.Contains(data[0]) 
                                        && x.Number.ToString()!.Contains(data[1]))
                                        .ToList();
                        }

                        if (DriversLicenceReadies.Count == 0)
                            DriversLicenceReadies = DriversLicenceOriginal;
                        break;
                }
            else
                DriversLicenceReadies = DriversLicenceOriginal;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = DriversLicenceReadies.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {DriversLicence.Count}";
        }

        public void ResizeDataGrid()
        {
            dataGrid.MaxHeight = startWindow.Height / 2 - 40; ;
        }

        private void PrevTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages - 10 >= 0)
            {
                skipPages -= 10;
                var skippedCargo = DriversLicenceReadies.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {DriversLicenceReadies.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < DriversLicence.Count)
            {
                skipPages += 10;
                var skippedCargo = DriversLicenceReadies.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {DriversLicenceReadies.Count}";
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                var item = dataGrid.SelectedItem as DriversLicenceReady;
                startWindow.client.DeleteDriverLicenceAsync(new GetOrDeleteDriverLicenceRequest { Id = (int)item.Id}, startWindow.headers);
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
                var item = await startWindow.client.GetListDriverLicencesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                

                DriversLicence = new List<DriverLicenceObject>();
                DriversLicence.AddRange(item.DriverLicence.ToList());
                DriversLicence.ForEach(license => DriversLicenceReadies.Add(new DriversLicenceReady(license.Id, license.Series, license.Number, license.Date)));
                
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = DriversLicenceReadies.Skip(skipPages).Take(takePages);
                PaginationTextBlock.Text = $"{skipPages + 10} из {DriversLicence.Count}";
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

        public void Dispose()
        {
            startWindow.SizeChanged -= (o, e) =>
            {
                ResizeDataGrid();
            };
            DriversLicence.Clear();
            DriversLicenceOriginal.Clear();
            DriversLicenceReadies.Clear();
            dataGrid.ItemsSource = null;
            BindingOperations.ClearAllBindings(dataGrid);
        }
    }
}
