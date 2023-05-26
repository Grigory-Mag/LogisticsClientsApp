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
using static LogisticsClientsApp.Pages.Tables.DriverLicenceTablePage;
using static LogisticsClientsApp.Pages.Tables.RequestsTablePage;

namespace LogisticsClientsApp.Pages.Tables
{
    /// <summary>
    /// Логика взаимодействия для DriversTablePage.xaml
    /// </summary>
    public partial class DriversTablePage : Page
    {
        public List<DriversObject> Drivers { get; set; }
        public List<DriversObject> DriversOriginal { get; set; }
        private Locale locale;
        string tableName = "водители";

        public int takePages = 10;
        public int skipPages = 0;

        StartWindow startWindow;

        public DriversTablePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            var tablePage = startWindow.MainFrameK.Content as TablePage;
            tablePage.TextBlockTableName.Text = tableName;
            Drivers = new List<DriversObject>();

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
                    case "ФИО":
                        text = text.Trim();
                        var data = text.Split(' ');
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (i > 2)
                                data[2] = ($"{data[2]} {data[i]}");
                        }
                        switch (data.Length)
                        {
                            case 1:
                                Drivers = DriversOriginal
                                    .Where(x => x.Surname.Contains(data[0]) || x.Name.Contains(data[0]) || x.Patronymic.Contains(data[0]))
                                    .ToList();
                                break;
                            case 2:
                                Drivers = DriversOriginal
                                    .Where(x => x.Surname.Contains(data[0])
                                    && x.Name.Contains(data[1]) || x.Patronymic.Contains(data[1]) || x.Patronymic.Contains(data[0]))
                                    .ToList();
                                break;
                            case 3:
                                Drivers = DriversOriginal
                                    .Where(x => x.Surname.Contains(data[0])
                                    && x.Name.Contains(data[1])
                                    && x.Patronymic.Contains(data[2]))
                                    .ToList();
                                break;
                        }
                        if (Drivers.Count == 0)
                            Drivers = DriversOriginal;
                        break;
                }
            else
                Drivers = DriversOriginal;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Drivers.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {Drivers.Count}";
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
                var skippedCargo = Drivers.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {Drivers.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < Drivers.Count)
            {
                skipPages += 10;
                var skippedCargo = Drivers.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {Drivers.Count}";
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                var item = dataGrid.SelectedItem as DriversObject;
                startWindow.client.DeleteDriverAsync(new GetOrDeleteDriversRequest { Id = (int)item.Id }, startWindow.headers);
                Drivers.Remove(item);

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = Drivers;
            }
        }

        private async void SetData()
        {
            try
            {
                var item = await startWindow.client.GetListDriversAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                Drivers = new List<DriversObject>();
                Drivers.AddRange(item.Drivers.ToList());

                DriversOriginal = Drivers;

                var selectedDrivers = Drivers.Skip(skipPages).Take(takePages).ToList();
                PaginationTextBlock.Text = $"{skipPages + 10} из {Drivers.Count}";

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = selectedDrivers;
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
            Drivers.Clear();
            DriversOriginal.Clear();
            dataGrid.ItemsSource = null;
            BindingOperations.ClearAllBindings(dataGrid);
        }
    }
}
