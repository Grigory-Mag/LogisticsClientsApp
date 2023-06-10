using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using MaterialDesignThemes.Wpf;
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
    /// Логика взаимодействия для VehiclesTabplePage.xaml
    /// </summary>
    public partial class VehiclesTablePage : Page
    {
        public List<VehiclesObject> Vehicles { get; set; }
        public List<VehiclesObject> VehiclesOriginal { get; set; }
        public List<VehiclesTypesObject> VehiclesTypes { get; set; }
        private Locale locale;

        public List<string> SearchFields = new List<string>() { "Тип", "Номер тягача", "Владелец", "Номер прицепа" };
        public List<object> SearchItemsList = new List<object>();
        bool isFirst = true;

        public TablePage tablePage;

        public static ResourceDictionary textBoxResource = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.TextBox.xaml", UriKind.RelativeOrAbsolute)
        };
        public static ResourceDictionary comboBoxResource = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.ComboBox.xaml", UriKind.RelativeOrAbsolute)
        };

        public int takePages = 10;
        public int skipPages = 0;

        public static VehiclesTablePage PageInstance;
        static StartWindow startWindow;

        public VehiclesTablePage()
        {
            InitializeComponent();
        }

        public static VehiclesTablePage CreateInstance()
        {
            if (PageInstance == null)
                PageInstance = new VehiclesTablePage();
            return PageInstance;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            string tableName = "транспорт";
            tablePage = startWindow.MainFrameK.Content as TablePage;
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
                    case "Номер тягача":
                        text = text.Trim();
                        Vehicles = VehiclesOriginal
                            .Where(x => x.Number.Contains(text))
                            .ToList();
                        break;
                    case "Владелец":
                        text = text.Trim();
                        Vehicles = VehiclesOriginal
                            .Where(x => x.Owner.Name.ToString().Contains(text))
                            .ToList();
                        break;
                }
            else
                Vehicles = VehiclesOriginal;

            if (Vehicles.Count == 0)
                Vehicles = VehiclesOriginal;

            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Vehicles.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {Vehicles.Count}";
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
                var skippedCargo = Vehicles.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {Vehicles.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < Vehicles.Count)
            {
                skipPages += 10;
                var skippedCargo = Vehicles.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {Vehicles.Count}";
            }
        }

        private void CreateAdvancedSearchFields()
        {
            SearchItemsList.ForEach(x =>
            {
                switch (x)
                {
                    case TextBox:
                        (x as TextBox).TextChanged -= SearchTextBoxChanged;
                        break;
                    case ComboBox:
                        (x as ComboBox).SelectionChanged -= SearchComboBox_SelectionChanged;
                        break;
                }
            });
            tablePage.AdvancedSearch.Children.Clear();

            BindingOperations.ClearAllBindings(tablePage.AdvancedSearch);
            SearchItemsList.Clear();

            // typeItemsSource.Add(new CargoTypesObject { Id = -1, Name = "Все типы" });
            foreach (var item in SearchFields)
            {
                var textBox = new TextBox();
                var comboBox = new ComboBox();

                HintAssist.SetHint(textBox, item);
                textBox.Margin = new Thickness(0, 0, 20, 0);
                textBox.Style = textBoxResource["MaterialDesignOutlinedTextBox"] as Style;
                textBox.MaxWidth = 180;
                textBox.MinWidth = 150;
                textBox.FontSize = 14;
                textBox.Height = 55;
                textBox.TextChanged += SearchTextBoxChanged;

                HintAssist.SetHint(comboBox, item);
                comboBox.Margin = new Thickness(0, 0, 20, 0);
                comboBox.Style = comboBoxResource["MaterialDesignOutlinedComboBox"] as Style;
                comboBox.MaxWidth = 180;
                comboBox.MinWidth = 130;
                comboBox.FontSize = 14;
                comboBox.Height = 55;
                comboBox.IsEditable = true;
                comboBox.VerticalAlignment = VerticalAlignment.Bottom;
                comboBox.SelectionChanged += SearchComboBox_SelectionChanged;

                switch (item)
                {
                    case "Тип":
                        comboBox.DisplayMemberPath = @"Name";
                        comboBox.ItemsSource = VehiclesTypes;
                        SearchItemsList.Add(comboBox);
                        tablePage.AdvancedSearch.Children.Add(comboBox);
                        break;
                    default:
                        tablePage.AdvancedSearch.Children.Add(textBox);
                        SearchItemsList.Add(textBox);
                        break;
                }
            }
            SyncSearch();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    var item = dataGrid.SelectedItem as VehiclesObject;
                    var resultLocal = await startWindow.client.DeleteVehicleAsync(new GetOrDeleteVehiclesRequest { Id = item.Id }, startWindow.headers);
                    VehiclesOriginal.Remove(item);

                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = VehiclesOriginal.Skip(skipPages).Take(takePages);
                }
                catch (RpcException ex)
                {
                    if (ex.StatusCode == StatusCode.Unauthenticated)
                        MessageBox.Show("Ваше время сессии истекло. Перезайдите в аккаунт", "Сессия", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show($"Возникла ошибка: {ex.StatusCode}. Проверьте, что данная запись нигде более не используется", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        /// <summary>
        /// Async Search needed due to probably high amount of data to filter
        /// </summary>
        /// <returns>filters data in List of custom object and synchronize search parameters.
        /// Requiring list of widgets</returns>
        private async Task SyncSearch()
        {
            try
            {
                await Task.Run(() =>
                {
                    List<string> values = new List<string>();

                    if (SearchItemsList.Count > 0)
                    {
                        var itemType = new VehiclesTypesObject();
                        Dispatcher.Invoke(() =>
                        {
                            itemType = (SearchItemsList[0] as ComboBox)!.SelectedItem as VehiclesTypesObject;
                        });

                        Dispatcher.Invoke(() =>
                        {

                            foreach (var value in SearchItemsList)
                            {
                                switch (value)
                                {
                                    case TextBox:
                                        values.Add((value as TextBox).Text);
                                        break;
                                }
                            }
                        });

                        Vehicles = VehiclesOriginal
                        .Where((x) =>
                        {
                            return itemType == null ? true : x.Type.Id == itemType.Id;
                        })
                        .Where((x) =>
                        {
                            return values[0] == "" ? true : x.Number.Contains(values[0]);
                        })
                        .Where((x) =>
                        {
                            return values[1] == "" ? true : x.Owner.Name.Contains(values[1]);
                        })
                        .Where((x) =>
                        {
                            return values[2] == "" ? true : x.TrailerNumber.Contains(values[2]);
                        })
                        .ToList();


                        if (Vehicles.Count == 0)
                            Vehicles = VehiclesOriginal;
                    }
                });

                dataGrid.ItemsSource = null;
                if (Vehicles.Count == 0)
                    Vehicles = VehiclesOriginal;
                dataGrid.ItemsSource = Vehicles.Skip(skipPages).Take(takePages);
                PaginationTextBlock.Text = $"{skipPages + 10} из {Vehicles.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private async void SearchTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            await SyncSearch();
        }

        private async void SearchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await SyncSearch();
        }

        private async void SetData()
        {
            try
            {
                BindingOperations.ClearAllBindings(dataGrid);
                var item = await startWindow.client.GetListVehiclesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                Vehicles = new List<VehiclesObject>();
                Vehicles.AddRange(item.Vehicle.ToList());
                Vehicles = Vehicles.OrderBy(x => x.Id).ToList();
                VehiclesOriginal = Vehicles;

                VehiclesTypes = new List<VehiclesTypesObject>();
                Vehicles.ForEach(x => VehiclesTypes.Add(x.Type));
                VehiclesTypes = VehiclesTypes.Distinct().ToList();


                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = Vehicles.Skip(skipPages).Take(takePages);
                locale.SetLocale(this);
                PaginationTextBlock.Text = $"{skipPages + 10} из {Vehicles.Count}";
                CreateAdvancedSearchFields();
            }
            catch (RpcException ex)
            {
                MessageBox.Show($"Возникла ошибка: {ex.StatusCode}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            Vehicles.Clear();
            VehiclesOriginal.Clear();
            VehiclesTypes.Clear();
            dataGrid.ItemsSource = null;
            BindingOperations.ClearAllBindings(dataGrid);
        }
    }
}
