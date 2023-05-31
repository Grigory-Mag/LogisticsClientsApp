using ApiService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticsClientsApp.Pages.Tables
{
    /// <summary>
    /// Логика взаимодействия для RequestsTablePage.xaml
    /// </summary>
    public partial class RequestsTablePage : Page
    {
        public List<RequestsReady> RequestsReadyObjects { get; set; }
        public List<RequestsReady> RequestsReadyObjectsOriginal { get; set; }
        public List<RequestsObject> Requests { get; set; }

        public List<DriversReady> Drivers { get; set; }
        public List<RequisitesObject> Customers { get; set; }
        public List<RequisitesObject> Transporters { get; set; }
        public List<CargoObject> Cargo { get; set; }
        public List<VehiclesTypesObject> VehiclesTypes { get; set; }

        public List<string> SearchFields = new List<string>() { "Номер", "Тип машины", "Водитель", "Перевозчик", "Заказчик", "Груз", "Дата создания", "Цена перевозки",
             "Номер машины"};
        public List<object> SearchItemsList = new List<object>();
        private Locale locale;
        public TablePage tablePage;
        public int takePages = 10;
        public int skipPages = 0;

        StartWindow startWindow;

        public class RequestsReady
        {
            public int Id { get; set; }
            public double? Price { get; set; }
            public VehiclesObject Vehicle { get; set; }
            public bool IsFinished { get; set; }
            public DateTime CreationDate { get; set; }
            public string DriverFIO { get; set; }
            public bool Documents { get; set; }
            public DriversObject Driver { get; set; }
            public CargoObject Cargo { get; set; }
            public RequisitesObject CustomerReq { get; set; }
            public RequisitesObject TransporterReq { get; set; }
            public ListRouteObjects Routes { get; set; }

            public RequestsReady()
            {
            }

            public RequestsReady(int id, double? price, VehiclesObject vehicle, bool isFinished, DateTime creationDate, bool documents, DriversObject driver, CargoObject cargo, RequisitesObject customerReq, RequisitesObject transporterReq, ListRouteObjects routes)
            {
                Id = id;
                Price = price;
                Vehicle = vehicle;
                IsFinished = isFinished;
                CreationDate = creationDate;
                Documents = documents;
                Driver = driver;
                Cargo = cargo;
                CustomerReq = customerReq;
                TransporterReq = transporterReq;
                Routes = routes;
                DriverFIO = $"{Driver.Surname} {Driver.Name} {Driver.Patronymic}";
            }

            public static explicit operator RequestsReady(RequestsObject item)
            {
                return new RequestsReady()
                {
                    Id = item.Id,
                    Price = item.Price,
                    Vehicle = item.Vehicle,
                    IsFinished = item.IsFinished,
                    Driver = item.Driver,
                    Cargo = item.Cargo,
                    CustomerReq = item.CustomerReq,
                    CreationDate = item.CreationDate.ToDateTime(),
                    Documents = item.Documents,
                    Routes = item.Routes,
                    TransporterReq = item.TransporterReq,
                    DriverFIO = $"{item.Driver.Name} {item.Driver.Surname} {item.Driver.Patronymic}",
                };
            }
        }
  
        public class DriversReady
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Patronymic { get; set; }
            public string FIO { get; set; }

            public DriversReady(DriversObject item)
            {
                Id = item.Id;
                Name = item.Name;
                Surname = item.Surname;
                Patronymic = item.Patronymic;
                FIO = $"{item.Surname} {item.Name} {item.Patronymic}";
            }

            public DriversReady(int id, string name, string surname, string patronymic)
            {
                Id = id;
                Name = name;
                Surname = surname;
                Patronymic = patronymic;
                FIO = $"{Surname} {Name} {Patronymic}";
            }
            public DriversReady()
            {

            }
        }

        public RequestsTablePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
            startWindow = (StartWindow)Window.GetWindow(this);
            string tableName = "заявки";
            tablePage = startWindow.MainFrameK.Content as TablePage;
            tablePage.TextBlockTableName.Text = tableName;
            locale = new Locale(startWindow.selectedLocale);

            Cargo = new List<CargoObject>();
            Drivers = new List<DriversReady>();
            Customers = new List<RequisitesObject>();
            Transporters = new List<RequisitesObject>();           

            SetData();
            startWindow.SizeChanged += (o, e) =>
            {
                ResizeDataGrid();
            };
            GC.Collect();
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
                var skippedCargo = RequestsReadyObjects.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {RequestsReadyObjects.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < RequestsReadyObjects.Count)
            {
                skipPages += 10;
                var skippedCargo = RequestsReadyObjects.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {RequestsReadyObjects.Count}";
            }
        }

        public RequestsObject GetSelectedDataGridItem()
        {
            return Requests.First(x => x.Id == (dataGrid.SelectedItem as RequestsReady).Id);
        }

        public void SetDataGridItems()
        {
            var requestsReady = new List<RequestsReady>();
            Requests.ForEach(x => requestsReady.Add((RequestsReady)x));
            RequestsReadyObjectsOriginal = requestsReady;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = RequestsReadyObjectsOriginal.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {RequestsReadyObjectsOriginal.Count}";
        }

        private async void SetData()
        {
            try
            {
                var item = await startWindow.client.GetListRequestsAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                var vehiclesTypes = await startWindow.client.GetListVehiclesTypesAsync(new Empty(), startWindow.headers);
                Requests = new List<RequestsObject>();
                Requests.AddRange(item.Requests.ToList());
                var requestsReady = new List<RequestsReady>();
                Requests.ForEach(val => requestsReady.Add((RequestsReady)val));

                RequestsReadyObjects = requestsReady;
                RequestsReadyObjectsOriginal = requestsReady;
                requestsReady.ForEach(x =>
                {
                    Drivers.Add(new DriversReady(x.Driver));
                    Customers.Add(x.CustomerReq);
                    Transporters.Add(x.TransporterReq);
                    Cargo.Add(x.Cargo);
                });

                Cargo = Cargo.Distinct().ToList();
                Drivers = Drivers.Distinct().ToList();
                Customers = Customers.Distinct().ToList();
                Transporters = Transporters.Distinct().ToList();
                VehiclesTypes = vehiclesTypes.VehiclesTypes.ToList();

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = requestsReady;
                locale.SetLocale(this);
                CreateAdvancedSearchFields();
            }
            catch (RpcException ex)
            {
#warning TODO
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            codeNumber = new Random().Next(100000, 999999);
            CodeNumberText.Text = "";
            var digits = codeNumber.ToString().Select(t => int.Parse(t.ToString())).ToArray();
            controlSum = digits[0] + digits[1] + digits[2] + digits[3] + digits[4] + digits[5];
            CodeNumberText.Text = $"{digits[0]} - {digits[1]} - {digits[2]} - {digits[3]} - {digits[4]} - {digits[5]}";
            OpenThisDialog.IsOpen = true;
            System.Media.SystemSounds.Beep.Play();
        }

        private async void ConfirmDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = dataGrid.SelectedItem as RequestsReady;
            try
            {
                await startWindow.client.DeleteRequestAsync(new GetOrDeleteRequestObjRequest { Id = item.Id }, startWindow.headers);
                RequestsReadyObjectsOriginal.Remove(item);

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = RequestsReadyObjectsOriginal.Skip(skipPages).Take(takePages);
            }
            catch (RpcException ex)
            {
                if (ex.StatusCode == StatusCode.Unauthenticated)
                    MessageBox.Show("Ваше время сессии истекло. Перезайдите в аккаунт", "Сессия", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show($"Возникла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Requests.Remove(Requests.First(x => x.Id == item.Id));
            SetDataGridItems();

            OpenThisDialog.IsOpen = false;
        }

        public void FastSearch(string text, string? param)
        {
            int number;
            double price;

            bool isPriceEmpty;
            bool isNumberEmpty;
            if (text != "")
                switch (param)
                {
                    case "Номер":
                        isNumberEmpty = int.TryParse(text, out number);
                        if (isNumberEmpty)
                            RequestsReadyObjects = RequestsReadyObjectsOriginal.Where(x => x.Id == number).ToList();
                        if (RequestsReadyObjects.Count == 0)
                            RequestsReadyObjects = RequestsReadyObjectsOriginal;
                        break;
                    case "Цена перевозки":
                        isPriceEmpty = double.TryParse(text, out price);
                        if (isPriceEmpty)
                            RequestsReadyObjects = RequestsReadyObjectsOriginal.Where(x => x.Price == price).ToList();
                        if (RequestsReadyObjects.Count == 0)
                            RequestsReadyObjects = RequestsReadyObjectsOriginal;
                        break;
                    case "Груз":
                        RequestsReadyObjects = RequestsReadyObjectsOriginal.Where(x => x.Cargo.Name.Contains(text)).ToList();
                        if (RequestsReadyObjects.Count == 0)
                            RequestsReadyObjects = RequestsReadyObjectsOriginal;
                        break;
                }
            else
                RequestsReadyObjects = RequestsReadyObjectsOriginal;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = RequestsReadyObjects;
        }

        private async void CreateAdvancedSearchFields()
        {
            tablePage.AdvancedSearch.Children.Clear();

            // typeItemsSource.Add(new CargoTypesObject { Id = -1, Name = "Все типы" });
            foreach (var item in SearchFields)
            {
                var textBox = new TextBox();
                var comboBox = new ComboBox();
                var datePicker = new DatePicker();
                var textBoxResource = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.TextBox.xaml", UriKind.RelativeOrAbsolute)
                };
                var comboBoxResource = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.ComboBox.xaml", UriKind.RelativeOrAbsolute)
                };
                var datePickerResource = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.DatePicker.xaml", UriKind.RelativeOrAbsolute)
                };

                HintAssist.SetHint(textBox, item);
                textBox.Margin = new Thickness(0, 0, 20, 0);
                textBox.Style = textBoxResource["MaterialDesignOutlinedTextBox"] as Style;
                textBox.MaxWidth = 180;
                textBox.MinWidth = 150;
                textBox.FontSize = 16;
                textBox.MaxLength = 30;
                textBox.TextChanged += SearchTextBoxChanged;
                //textBox.Name = item;


                HintAssist.SetHint(comboBox, item);
                comboBox.Margin = new Thickness(0, 0, 20, 0);
                comboBox.Style = comboBoxResource["MaterialDesignOutlinedComboBox"] as Style;
                comboBox.MaxWidth = 180;
                comboBox.MinWidth = 130;
                comboBox.FontSize = 16;
                comboBox.IsEditable = true;
                comboBox.VerticalAlignment = VerticalAlignment.Bottom;
                comboBox.SelectionChanged += SearchComboBox_SelectionChanged;


                switch (item)
                {
                    case "Груз":
                        comboBox.DisplayMemberPath = @"Name";
                        comboBox.ItemsSource = Cargo;
                        SearchItemsList.Add(comboBox);
                        tablePage.AdvancedSearch.Children.Add(comboBox);
                        break;
                    case "Дата создания":
                        HintAssist.SetHint(datePicker, "Дата создания");
                        datePicker.Margin = new Thickness(0, 0, 20, 0);
                        datePicker.Style = datePickerResource["MaterialDesignOutlinedDatePicker"] as Style;
                        datePicker.MaxWidth = 200;
                        datePicker.MinWidth = 180;
                        datePicker.FontSize = 16;
                        datePicker.VerticalAlignment = VerticalAlignment.Bottom;
                        datePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
                        datePicker.Language = Language;
                        SearchItemsList.Add(datePicker);
                        tablePage.AdvancedSearch.Children.Add(datePicker);
                        break;
                    case "Водитель":
                        comboBox.DisplayMemberPath = @"FIO";
                        comboBox.ItemsSource = Drivers;
                        SearchItemsList.Add(comboBox);
                        tablePage.AdvancedSearch.Children.Add(comboBox);
                        break;
                    case "Тип машины":
                        comboBox.DisplayMemberPath = @"Name";
                        comboBox.ItemsSource = VehiclesTypes;
                        SearchItemsList.Add(comboBox);
                        tablePage.AdvancedSearch.Children.Add(comboBox);
                        break;
                    case "Перевозчик":
                        comboBox.DisplayMemberPath = @"Name";
                        comboBox.ItemsSource = Transporters;
                        SearchItemsList.Add(comboBox);
                        tablePage.AdvancedSearch.Children.Add(comboBox);
                        break;
                    case "Заказчик":
                        comboBox.DisplayMemberPath = @"Name";
                        comboBox.ItemsSource = Customers;
                        //comboBox.SelectedItem = typeItemsSource.First(x => x.Id == -1);
                        SearchItemsList.Add(comboBox);
                        tablePage.AdvancedSearch.Children.Add(comboBox);
                        break;
                    default:
                        tablePage.AdvancedSearch.Children.Add(textBox);
                        SearchItemsList.Add(textBox);
                        break;
                }
            }
            await SyncSearch();
        }

        private async void DatePicker_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            await SyncSearch();
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
                    var emptyFields = 0;
                    var searchRequestsObjects = RequestsReadyObjectsOriginal;
                    List<string> values = new List<string>();

                    if (SearchItemsList.Count > 0)
                    {
                        var itemVehicleType = new VehiclesTypesObject();
                        var itemDriver = new DriversReady();
                        var itemTransporter = new RequisitesObject();
                        var itemCustomer = new RequisitesObject();
                        var itemCargo = new CargoObject();
                        DateTime? itemDate = new DateTime();
                        Dispatcher.Invoke(() =>
                        {
                            itemVehicleType = (SearchItemsList[1] as ComboBox)!.SelectedItem as VehiclesTypesObject;
                            itemDriver = (SearchItemsList[2] as ComboBox)!.SelectedItem as DriversReady;
                            itemTransporter = (SearchItemsList[3] as ComboBox)!.SelectedItem as RequisitesObject;
                            itemCustomer = (SearchItemsList[4] as ComboBox)!.SelectedItem as RequisitesObject;
                            itemCargo = (SearchItemsList[5] as ComboBox)!.SelectedItem as CargoObject;
                            itemDate = (SearchItemsList[6] as DatePicker)!.SelectedDate;
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

                        int number;
                        double price;

                        var isNumberEmpty = int.TryParse(values[0], out number);
                        var isPriceEmpty = double.TryParse(values[1], out price);

                        RequestsReadyObjects = searchRequestsObjects
                        .Where((x) =>
                        {
                            return isNumberEmpty == false ? true : x.Id == number;
                        })
                        .Where((x) =>
                        {
                            return itemVehicleType == null ? true : x.Vehicle.Type.Name == itemVehicleType.Name;
                        })
                        .Where((x) =>
                        {
                            return itemTransporter == null ? true : x.TransporterReq.Id == itemTransporter.Id;
                        })
                        .Where((x) =>
                        {
                            return itemCustomer == null ? true : x.CustomerReq.Id == itemCustomer.Id;
                        })
                        .Where((x) =>
                        {
                            return itemDriver == null ? true : x.Driver.Id == itemDriver.Id;
                        })
                        .Where((x) =>
                        {
                            return itemDate == null ? true : x.CreationDate.Date == itemDate.Value.Date;
                        })
                        .Where((x) =>
                        {
                            return isPriceEmpty == false ? true : x.Price == price;
                        })
                        .Where((x) =>
                        {
                            return values[2] == "" ? true : x.Vehicle.Number.Contains(values[2]);
                        })
                        .Where((x) =>
                        {
                            return itemCargo == null ? true : x.Cargo.Id == itemCargo.Id;
                        })
                        .ToList();


                        if (RequestsReadyObjects.Count == 0)
                            RequestsReadyObjects = searchRequestsObjects;
                    }
                });

                dataGrid.ItemsSource = null;
                if (RequestsReadyObjects.Count == 0)
                    RequestsReadyObjects = RequestsReadyObjectsOriginal;
                dataGrid.ItemsSource = RequestsReadyObjects.Skip(skipPages).Take(takePages);
                PaginationTextBlock.Text = $"{skipPages + 10} из {RequestsReadyObjects.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }


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

        int codeNumber;
        int controlSum;
        int[] nums = new int[6];

        private void CancelDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            OpenThisDialog.IsOpen = false;
        }


        private void TextBoxNumberCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            int num;
            int completedNum;
            int.TryParse(textBox.Text, out num);
            switch (textBox.Tag)
            {
                case "Num1":
                    nums[0] = num;
                    break;
                case "Num2":
                    nums[1] = num;
                    break;
                case "Num3":
                    nums[2] = num;
                    break;
                case "Num4":
                    nums[3] = num;
                    break;
                case "Num5":
                    nums[4] = num;
                    break;
                case "Num6":
                    nums[5] = num;
                    break;
            }
            completedNum = nums[0] + nums[1] + nums[2] + nums[3] + nums[4] + nums[5];

            ConfirmDeleteButton.IsEnabled = int.Parse($"{nums[0]}{nums[1]}{nums[2]}{nums[3]}{nums[4]}{nums[5]}") == codeNumber;
        }

        public void Dispose()
        {
            startWindow.SizeChanged -= (o, e) =>
            {
                ResizeDataGrid();
            };
            Requests.Clear();
            dataGrid.ItemsSource = null;
            BindingOperations.ClearAllBindings(dataGrid);
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var datagrid = sender as DataGrid;
            if (dataGrid.SelectedItem as RequestsReady != null)
            {
                TablePage tablePage = (TablePage)startWindow.MainFrameK.Content;
                tablePage.ShowModalPage(0);
            }
        }
    }
}

