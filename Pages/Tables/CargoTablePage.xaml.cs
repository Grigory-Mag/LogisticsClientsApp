using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages.Modal;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace LogisticsClientsApp.Pages.Tables
{
    /// <summary>
    /// Логика взаимодействия для CargoTablePage.xaml
    /// </summary>
    public partial class CargoTablePage : Page
    {
        public List<CargoObject> CargoObjects { get; set; }
        public List<CargoObject> CargoObjectsOriginal { get; set; }
        public List<string> SearchFields = new List<string>() { "Тип", "Ограничения", "Масса", "Объём", "Название", "Цена" };
        public ListCargoType CargoTypes { get; set; }
        public TablePage tablePage;
        public List<object> SearchItemsList = new List<object>();
        public int takePages = 10;
        public int skipPages = 0;
        private Locale locale;

        StartWindow startWindow;
        public CargoTablePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            tablePage = (TablePage)startWindow.MainFrameK.Content;
            CargoTypes = new ListCargoType();
            CargoTypes.CargoType.Add(new CargoTypesObject { Id = 0, Name = "Base" });
            locale = new Locale(startWindow.selectedLocale);
            startWindow.SizeChanged += (o, e) =>
            {
                ResizeDataGrid();
            };
            SetData();
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
                var skippedCargo = CargoObjects.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {CargoObjects.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < CargoObjects.Count)
            {
                skipPages += 10;
                var skippedCargo = CargoObjects.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {CargoObjects.Count}";
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                var item = dataGrid.SelectedItem as CargoObject;
                startWindow.client.DeleteCargoAsync(new GetOrDeleteCargoRequest { Id = item.Id });
                CargoObjects.Remove(item);
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = CargoObjects;
            }

        }

        private void CreateAdvancedSearchFields()
        {
            tablePage.AdvancedSearch.Children.Clear();
            var typeItemsSource = CargoTypes.CargoType;
            typeItemsSource.Add(new CargoTypesObject { Id = -1, Name = "Все типы" });
            foreach (var item in SearchFields)
            {
                var textBox = new TextBox();
                var textBoxResource = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.TextBox.xaml", UriKind.RelativeOrAbsolute)
                };
                var comboBoxResource = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.ComboBox.xaml", UriKind.RelativeOrAbsolute)
                };
                HintAssist.SetHint(textBox, item);
                textBox.Margin = new Thickness(0, 0, 20, 0);
                textBox.Style = textBoxResource["MaterialDesignOutlinedTextBox"] as Style;
                textBox.MaxWidth = 130;
                textBox.MinWidth = 130;
                textBox.FontSize = 16;
                textBox.MaxLength = 30;
                textBox.TextChanged += SearchTextBoxChanged;
                textBox.Name = item;
                if (item == "Тип")
                {
                    var comboBox = new ComboBox();
                    HintAssist.SetHint(comboBox, item);
                    comboBox.Margin = new Thickness(0, 0, 20, 0);
                    comboBox.Style = comboBoxResource["MaterialDesignOutlinedComboBox"] as Style;
                    comboBox.MaxWidth = 130;
                    comboBox.MinWidth = 130;
                    comboBox.FontSize = 16;
                    comboBox.DisplayMemberPath = @"Name";
                    comboBox.SelectionChanged += SearchComboBox_SelectionChanged;
                    comboBox.ItemsSource = typeItemsSource;
                    comboBox.SelectedItem = typeItemsSource.First(x => x.Id == -1);
                    SearchItemsList.Add(comboBox);
                    tablePage.AdvancedSearch.Children.Add(comboBox);
                }
                else
                {
                    tablePage.AdvancedSearch.Children.Add(textBox);
                    SearchItemsList.Add(textBox);
                }
            }
            var box = SearchItemsList.First((x) =>
            {
                var textBox = x as TextBox;
                if (textBox != null)
                    return textBox.Name == "Масса";
                else
                    return false;
            }) as TextBox;

            //MessageBox.Show(box.Name);
        }

        private async void SetData()
        {
            try
            {
                var item2 = await startWindow.client.GetListCargoAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                CargoTypes = await startWindow.client.GetListCargoTypesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                CargoObjects = new List<CargoObject>();
                CargoObjects.AddRange(item2.Cargo.ToList());
                CargoObjectsOriginal = CargoObjects;

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = CargoObjects;
                locale.SetLocale(this);
                CreateAdvancedSearchFields();
            }
            catch (RpcException ex)
            {
#warning TODO
            }
        }

        /// <summary>
        /// Async Search needed due to probably high amount of data to filter
        /// </summary>
        /// <returns>filters data in List of custom object and synchronize search parameters.
        /// Requiring list of widgets</returns>
        private async Task SyncSearch()
        {
            await Task.Run(() =>
            {
                var emptyFields = 0;
                var searchCargoObjects = CargoObjectsOriginal;

                if (SearchItemsList.Count > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        searchCargoObjects = CargoObjectsOriginal.Where(x => x.CargoType.Id ==
                            ((SearchItemsList[0] as ComboBox)!.SelectedItem as CargoTypesObject)!.Id).ToList();
                    });

                    CargoObjects = searchCargoObjects.Where((x) =>
                    {
                        string text;
                        List<string> values = new List<string>();
                        //List<object> objectValues = new List<object>();
                        CargoTypesObject selectedItem;
                        Dispatcher.Invoke(() =>
                        {
                            foreach (var item in SearchItemsList)
                            {
                                switch (item)
                                {
                                    case TextBox:
                                        values.Add((item as TextBox).Text);
                                        break;
                                        /*                                    case ComboBox:
                                                                                objectValues.Add((item as ComboBox).SelectedItem);
                                                                                break;*/
                                }
                            }
                        });

                        double weight;
                        double volume;
                        double price;

                        var isWeightEmpty = double.TryParse(values[1], out weight);
                        var isVolumeEmpty = double.TryParse(values[2], out volume);
                        var isPriceEmpty = double.TryParse(values[4], out price);

                        /*                        var cargoTypeSearch = (x.CargoType.Id == (objectValues[0] as CargoTypesObject)!.Id
                                                        && (objectValues[0] as CargoTypesObject)!.Id != -1);*/
                        var paramsSearch = (x.Constraints.Contains(values[0]) && values[0] != "") ||
                            (x.Weight == weight && !isWeightEmpty) ||
                            (x.Volume == volume && !isVolumeEmpty) ||
                            (x.Name.Contains(values[3]) && values[3] != "") ||
                            (x.Price == price && !isPriceEmpty);

                        //Debug.WriteLine($"============ \n c- {cargoTypeSearch} \n p- {paramsSearch} \n {!(cargoTypeSearch && paramsSearch)}");

                        return paramsSearch;

                        /*                        if (cargoTypeSearch && paramsSearch)
                                                    return true;
                                                else
                                                    return cargoTypeSearch;*/

                        //return (cargoTypeSearch && paramsSearch);
                    }).ToList();
                    if (CargoObjects.Count == 0)
                        CargoObjects = searchCargoObjects;
                }

            });

            dataGrid.ItemsSource = null;
            if (CargoObjects.Count == 0)
                CargoObjects = CargoObjectsOriginal;
            dataGrid.ItemsSource = CargoObjects.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {CargoObjects.Count}";

        }
        private async void SearchTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            var hint = HintAssist.GetHint(textBox).ToString();
            var text = textBox.Text;

            await SyncSearch();
        }

        private async void SearchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await SyncSearch();
        }

        private void OpenRowButton_Click(object sender, RoutedEventArgs e)
        {

            TablePage tablePage = (TablePage)startWindow.MainFrameK.Content;
            tablePage.ShowModalPage(0);
        }

    }
}
