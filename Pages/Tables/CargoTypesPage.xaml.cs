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
    /// Логика взаимодействия для CargoTypesPage.xaml
    /// </summary>
    public partial class CargoTypesPage : Page
    {
        public List<CargoTypesObject> CargoTypes { get; set; }
        public List<CargoTypesObject> CargoTypesOriginal { get; set; }
        private Locale locale;
        string tableName = "типы грузов";

        public int takePages = 10;
        public int skipPages = 0;

        StartWindow startWindow;
        public CargoTypesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            var tablePage = startWindow.MainFrameK.Content as TablePage;
            tablePage.TextBlockTableName.Text = tableName;
            SetData();
            ResizeDataGrid();
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
                    case "Наименование":
                        text = text.Trim();
                        CargoTypes = CargoTypesOriginal
                            .Where(x => x.Name.Contains(text))
                            .ToList();
                        if (CargoTypes.Count == 0)
                            CargoTypes = CargoTypesOriginal;
                        break;
                }
            else
                CargoTypes = CargoTypesOriginal;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = CargoTypes.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {CargoTypes.Count}";
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
                var skippedCargo = CargoTypes.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {CargoTypes.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < CargoTypes.Count)
            {
                skipPages += 10;
                var skippedCargo = CargoTypes.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {CargoTypes.Count}";
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                var item = dataGrid.SelectedItem as CargoTypesObject;
                startWindow.client.DeleteCargoTypeAsync(new GetOrDeleteCargoTypesRequest { Id = item.Id }, startWindow.headers);
                CargoTypes.Remove(item);
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = CargoTypes;
            }
        }

        private async void SetData()
        {
            try
            {
                var item = await startWindow.client.GetListCargoTypesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                CargoTypes = new List<CargoTypesObject>();
                CargoTypes.AddRange(item.CargoType.ToList());

                CargoTypesOriginal = CargoTypes;

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = CargoTypes.Skip(skipPages).Take(takePages);
                PaginationTextBlock.Text = $"{skipPages + 10} из {CargoTypes.Count}";
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
            CargoTypes.Clear();
            CargoTypesOriginal.Clear();
            dataGrid.ItemsSource = null;
            BindingOperations.ClearAllBindings(dataGrid);
        }
    }
}
