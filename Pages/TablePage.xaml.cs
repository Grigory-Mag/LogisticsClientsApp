﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations.Data;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages.Tables;
using System.Globalization;
using LogisticsClientsApp.Pages.Modal;
using static LogisticsClientsApp.Pages.Tables.DriverLicenceTablePage;
using System.Windows.Automation;
using Microsoft.Win32;
using System.IO;
using System.Data;
using System.Reflection;
using ExcelExportLib;
using System.Threading;
using static LogisticsClientsApp.Pages.Tables.RequestsTablePage;
using System.Configuration;
using System.Windows.Threading;

namespace LogisticsClientsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {

        //public PaletteHelper palette;
        public Page selectedPage { get; set; }
        Locale locale;
        public CargoTablePage cargoTablePageInstance = CargoTablePage.CreateInstance();

        public static TablePage PageInstance;
        static StartWindow startWindow;
        static Excel ExcelProvider = new Excel();

        public enum Messages
        {
            Success,
            Error,
        }

        public TablePage()
        {
            InitializeComponent();
        }

        public TablePage(Page page)
        {
            InitializeComponent();
            ChangeSelectedTable(page);
        }

        public static TablePage CreateInstance()
        {
            if (PageInstance == null)
            {
                PageInstance = new TablePage();
                PageInstance.SetData();
            }
            return PageInstance;
        }

        public static TablePage CreateInstance(Page page)
        {
            if (PageInstance == null)
            {
                PageInstance = new TablePage();
                PageInstance.ChangeSelectedTable(page);
                page = null;
                //PageInstance.SetData();
            }
            else
                if (PageInstance.selectedPage != page)
            {
                PageInstance = new TablePage();
                PageInstance.ChangeSelectedTable(page);
                page = null;
            }
            return PageInstance;
        }


        private void InnerInit()
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            //cargoTablePageInstance = new CargoTablePage();

            startWindow.ClearFrameHistory();
            startWindow.MainGrid.Background = new SolidColorBrush(Colors.Transparent);
            //DataGridFrame.Navigate(cargoTablePageInstance);

            startWindow.LeftMenu.Visibility = Visibility.Visible;
            startWindow.MenuOpenBtn.Visibility = Visibility.Visible;

            //locale = new Locale(startWindow.selectedLocale);
            //palette = new PaletteHelper();
        }

        public void OuterInit()
        {
            InnerInit();
        }

        private void SetSelectedPageParams(Page page)
        {
            SearchFilter.ItemsSource = null;
            switch (page)
            {
                case CargoTablePage:
                    SearchFilter.ItemsSource = new List<string>() { "Номер", "Масса", "Цена" };
                    AdvancedSearchButton.Visibility = Visibility.Visible;
                    break;
                case CargoTypesPage:
                    SearchFilter.ItemsSource = new List<string>() { "Наименование" };
                    AdvancedSearchButton.Visibility = Visibility.Collapsed;
                    break;
                case DriverLicenceTablePage:
                    SearchFilter.ItemsSource = new List<string>() { "Серия/Номер" };
                    AdvancedSearchButton.Visibility = Visibility.Collapsed;
                    break;
                case DriversTablePage:
                    SearchFilter.ItemsSource = new List<string>() { "ФИО" };
                    AdvancedSearchButton.Visibility = Visibility.Collapsed;
                    break;
                case RequisitesTablePage:
                    SearchFilter.ItemsSource = new List<string>() { "Название", "ИНН" };
                    AdvancedSearchButton.Visibility = Visibility.Visible;
                    break;
                case RequisiteTypesTablePage:
                    SearchFilter.ItemsSource = new List<string>() { "Название" };
                    AdvancedSearchButton.Visibility = Visibility.Collapsed;
                    break;
                case RolesTablePage:
                    SearchFilter.ItemsSource = new List<string>() { "Название" };
                    AdvancedSearchButton.Visibility = Visibility.Collapsed;
                    break;
                case VehiclesTablePage:
                    SearchFilter.ItemsSource = new List<string>() { "Номер тягача", "Владелец" };
                    AdvancedSearchButton.Visibility = Visibility.Visible;
                    break;
                case VehiclesTypesTablePage:
                    SearchFilter.ItemsSource = new List<string>() { "Название" };
                    AdvancedSearchButton.Visibility = Visibility.Collapsed;
                    break;
                case RequestsTablePage:
                    SearchFilter.ItemsSource = new List<string> { "Номер", "Цена перевозки", "Груз" };
                    AdvancedSearchButton.Visibility = Visibility.Visible;
                    break;
                case RouteActionsTablePage:
                    SearchFilter.ItemsSource = new List<string> { "Название" };
                    AdvancedSearchButton.Visibility = Visibility.Collapsed;
                    break;
                case UsersTablePage:
                    SearchFilter.ItemsSource = new List<string> { "Логин" };
                    AdvancedSearchButton.Visibility = Visibility.Collapsed;
                    break;
            }
            SearchFilter.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InnerInit();
            if (selectedPage == null)
                ChangeSelectedTable(cargoTablePageInstance);
            else
                ChangeSelectedTable(selectedPage);
        }

        public void SetFilter()
        {
            SetSelectedPageParams(selectedPage);
        }

        public string IdHeader;
        public async Task<int> SetData()
        {
            CargoObject cargo = await GetTestData();
            List<CargoObject> objects = new List<CargoObject>();
            objects.Add(cargo);
            cargoTablePageInstance.dataGrid.ItemsSource = objects;

            return 1;
        }

        public async Task<CargoObject> GetTestData()
        {
            try
            {
                var item = await startWindow.client.GetCargoAsync(new GetOrDeleteCargoRequest { Id = 1 }, startWindow.headers);
                return await Task.FromResult(item);
            }
            catch (RpcException ex)
            {
                return await Task.FromResult(new CargoObject());
            }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PrevTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public class NumericValidationRule : ValidationRule
        {
            public Type ValidationType { get; set; }
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                string strValue = Convert.ToString(value);

                if (string.IsNullOrEmpty(strValue))
                    return new ValidationResult(false, $"Value cannot be coverted to string.");
                bool canConvert = false;
                switch (ValidationType.Name)
                {

                    case "Boolean":
                        bool boolVal = false;
                        canConvert = bool.TryParse(strValue, out boolVal);
                        return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input should be type of boolean");
                    case "Int32":
                        int intVal = 0;
                        canConvert = int.TryParse(strValue, out intVal);
                        return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input should be type of Int32");
                    case "Double":
                        double doubleVal = 0;
                        canConvert = double.TryParse(strValue, out doubleVal);
                        return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input should be type of Double");
                    case "Int64":
                        long longVal = 0;
                        canConvert = long.TryParse(strValue, out longVal);
                        return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input should be type of Int64");
                    default:
                        throw new InvalidCastException($"{ValidationType.Name} is not supported");
                }
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void ChangeSelectedTable(Page page)
        {
            if (page is UsersTablePage)
                ExportButton.Visibility = Visibility.Hidden;
            else
                ExportButton.Visibility = Visibility.Visible;
            DataGridFrame.Content = null;
            DataGridFrame.Navigate(page);
            selectedPage = page;
            SetSelectedPageParams(selectedPage);
            locale = new Locale(startWindow.selectedLocale);
            ModalPageFrame.Content = null;
            locale.SetLocale(this);
        }

        public void ShowModalPage(byte mode)
        {
            switch (DataGridFrame.Content)
            {
                case CargoTablePage:
                    CargoTablePageModal cargoTablePage = new CargoTablePageModal();
                    cargoTablePage.SetMode(mode);
                    ModalPageFrame.Content = cargoTablePage;
                    if (mode == 0)
                    {
                        var modalPage = DataGridFrame.Content as CargoTablePage;
                        cargoTablePage.UpdateDisplayedData(modalPage.dataGrid.SelectedItem as CargoObject);
                        modalPage.dataGrid.SelectedItem = null;
                    }
                    Storyboard? sb = cargoTablePage.Resources["OpenModal"] as Storyboard;
                    sb!.Begin(ModalPageFrame);
                    break;
                case CargoTypesPage:
                    var cargoType = new CargoTypesTablePageModal();
                    cargoType.SetMode(mode);
                    ModalPageFrame.Content = cargoType;
                    if (mode == 0)
                    {
                        var modalPage = DataGridFrame.Content as CargoTypesPage;
                        cargoType.UpdateDisplayedData(modalPage.dataGrid.SelectedItem as CargoTypesObject);
                        modalPage.dataGrid.SelectedItem = null;
                    }
                    (cargoType.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    locale.SetLocale(this);
                    break;
                case DriverLicenceTablePage:
                    var driverLicence = new DriverLicenceTablePageModal();
                    driverLicence.SetMode(mode);
                    ModalPageFrame.Content = driverLicence;
                    if (mode == 0)
                    {
                        var driverLicenceModalPage = DataGridFrame.Content as DriverLicenceTablePage;
                        driverLicence.UpdateDisplayedData(driverLicenceModalPage.dataGrid.SelectedItem as DriversLicenceReady);
                        driverLicenceModalPage.dataGrid.SelectedItem = null;
                    }
                    (driverLicence.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    break;
                case DriversTablePage:
                    var driver = new DriversTablePageModal();
                    ModalPageFrame.Content = driver;
                    driver.SetMode(mode);
                    if (mode == 0)
                    {
                        var driverTableModalPage = DataGridFrame.Content as DriversTablePage;
                        driver.UpdateDisplayedData(driverTableModalPage.dataGrid.SelectedItem as DriversObject);
                        driverTableModalPage.dataGrid.SelectedItem = null;
                    }
                    (driver.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    break;
                case RequisitesTablePage:
                    var requisite = new RequisitesTablePageModal();
                    ModalPageFrame.Content = requisite;
                    requisite.SetMode(mode);
                    if (mode == 0)
                    {
                        var requisitesTableModalPage = DataGridFrame.Content as RequisitesTablePage;
                        requisite.UpdateDisplayedData(requisitesTableModalPage.dataGrid.SelectedItem as RequisitesObject);
                        requisitesTableModalPage.dataGrid.SelectedItem = null;
                    }

                    (requisite.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    break;
                case RolesTablePage:
                    var role = new RolesTablePageModal();
                    ModalPageFrame.Content = role;
                    role.SetMode(mode);
                    if (mode == 0)
                    {
                        var rolesTablePageModal = DataGridFrame.Content as RolesTablePage;
                        role.UpdateDisplayedData(rolesTablePageModal.dataGrid.SelectedItem as RolesObject);
                        rolesTablePageModal.dataGrid.SelectedItem = null;
                    }

                    (role.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    break;
                case VehiclesTablePage:
                    var vehicle = new VehiclesTablePageModal();
                    ModalPageFrame.Content = vehicle;
                    vehicle.SetMode(mode);
                    if (mode == 0)
                    {
                        var vehiclesTableModalPage = DataGridFrame.Content as VehiclesTablePage;
                        vehicle.UpdateDisplayedData(vehiclesTableModalPage.dataGrid.SelectedItem as VehiclesObject);
                        vehiclesTableModalPage.dataGrid.SelectedItem = null;
                    }

                    (vehicle.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    break;
                case VehiclesTypesTablePage:
                    var vehicleType = new VehiclesTypesTablePageModal();
                    ModalPageFrame.Content = vehicleType;
                    vehicleType.SetMode(mode);
                    if (mode == 0)
                    {
                        var vehiclesTypeTableModalPage = DataGridFrame.Content as VehiclesTypesTablePage;
                        vehicleType.UpdateDisplayedData(vehiclesTypeTableModalPage.dataGrid.SelectedItem as VehiclesTypesObject);
                        vehiclesTypeTableModalPage.dataGrid.SelectedItem = null;
                    }

                    (vehicleType.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    break;
                case RequisiteTypesTablePage:
                    var requisiteType = new RequisitesTypeTablePageModal();
                    ModalPageFrame.Content = requisiteType;
                    requisiteType.SetMode(mode);
                    if (mode == 0)
                    {
                        var requisiteTypeModalPage = DataGridFrame.Content as RequisiteTypesTablePage;
                        requisiteType.UpdateDisplayedData(requisiteTypeModalPage.dataGrid.SelectedItem as RequisiteTypeObject);
                        requisiteTypeModalPage.dataGrid.SelectedItem = null;
                    }

                    (requisiteType.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    break;
                case RequestsTablePage:
                    var request = new RequestsTablePageModal();
                    request.LoadModalPage();
                    ModalPageFrame.Content = request;
                    request.SetMode(mode);
                    if (mode == 0)
                    {
                        var requestModalPage = DataGridFrame.Content as RequestsTablePage;
                        request.UpdateDisplayedData(requestModalPage.GetSelectedDataGridItem());
                        requestModalPage.dataGrid.SelectedItem = null;
                    }

                    (request.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    break;
                case RouteActionsTablePage:
                    var routeAction = new RouteActionsTablePageModal();
                    ModalPageFrame.Content = routeAction;
                    routeAction.SetMode(mode);
                    if (mode == 0)
                    {
                        var routeActionModalPage = DataGridFrame.Content as RouteActionsTablePage;
                        routeAction.UpdateDisplayedData(routeActionModalPage.dataGrid.SelectedItem as RouteActionsObject);
                        routeActionModalPage.dataGrid.SelectedItem = null;
                    }

                    (routeAction.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    break;
                case UsersTablePage:
                    var login = new UsersTablePageModal();
                    ModalPageFrame.Content = login;
                    login.SetMode(mode);
                    if (mode == 0)
                    {
                        var loginModalPage = DataGridFrame.Content as UsersTablePage;
                        login.UpdateDisplayedData(loginModalPage.dataGrid.SelectedItem as LoginObject);
                        loginModalPage.dataGrid.SelectedItem = null;
                    }

                    (login.Resources["OpenModal"] as Storyboard)!.Begin(ModalPageFrame);
                    break;
            }
            MainPanel.Opacity = .5;
            MainPanel.IsEnabled = false;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ShowModalPage(1);
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static DataTable ToDataTableReady<T>(List<T> items)
        {
            var dataReady = new DataTable();
            switch (typeof(T))
            {
                case var cls when cls == typeof(CargoObject):
                    dataReady.Columns.Add("Тип");
                    dataReady.Columns.Add("Масса");
                    dataReady.Columns.Add("Объём");
                    dataReady.Columns.Add("Название");
                    dataReady.Columns.Add("Цена");
                    dataReady.Columns.Add("Ограничения");
                    foreach (var item in items as List<CargoObject>)
                        dataReady.Rows.Add(new object[6] { item.CargoType.Name, item.Weight, item.Volume, item.Name, item.Price, item.Constraints });
                    break;
                case var cls when cls == typeof(CargoTypesObject):
                    dataReady.Columns.Add("Название");
                    foreach (var item in items as List<CargoTypesObject>)
                        dataReady.Rows.Add(new object[1] { item.Name });
                    break;
                case var cls when cls == typeof(DriversLicenceReady):
                    dataReady.Columns.Add("Серия");
                    dataReady.Columns.Add("Номер");
                    dataReady.Columns.Add("Дата выдачи");
                    foreach (var item in items as List<DriversLicenceReady>)
                        dataReady.Rows.Add(new object[3] { item.Series, item.Number, item.Date.Date });
                    break;
                case var cls when cls == typeof(DriversObject):
                    dataReady.Columns.Add("Фамилия");
                    dataReady.Columns.Add("Имя");
                    dataReady.Columns.Add("Отчество");
                    dataReady.Columns.Add("Сан. обработка");
                    dataReady.Columns.Add("Лицензия");
                    foreach (var item in items as List<DriversObject>)
                        dataReady.Rows.Add(new object[5] { item.Surname, item.Name, item.Patronymic, item.Sanitation == true ? "Есть" : "Нет", $"{item.Licence.Series}/{item.Licence.Number}" });
                    break;
                case var cls when cls == typeof(RequestsReady):
                    //dataReady.Columns.Add("Номер");
                    //dataReady.Columns.Add("Транспорт");
                    //dataReady.Columns.Add("Водитель");
                    //dataReady.Columns.Add("Цена");
                    //dataReady.Columns.Add("Дата создания");
                    //dataReady.Columns.Add("Оригинал документов");
                    //dataReady.Columns.Add("Заказчик");
                    //dataReady.Columns.Add("Перевозчик");
                    //dataReady.Columns.Add("Груз");
                    //dataReady.Columns.Add("Масса груза");
                    //dataReady.Columns.Add("Тип груза");
                    //dataReady.Columns.Add("Статус");
                    //foreach (var item in items as List<RequestsReady>)
                    //    dataReady.Rows.Add(new object[12] { item.Id, $"{item.Vehicle.Type.Name}, Тягач: {item.Vehicle.Number}, Прицеп: {item.Vehicle.TrailerNumber}",
                    //        $"{item.Driver.Surname} {item.Driver.Name} {item.Driver.Patronymic}",
                    //        item.Price,
                    //        item.CreationDate.Date,
                    //        item.Documents == true ? "Да" : "Нет",
                    //        item.CustomerReq.Name,
                    //        item.TransporterReq.Name,
                    //        item.Cargo.Name,
                    //        item.Cargo.Weight,
                    //        item.Cargo.CargoType.Name,
                    //        item.IsFinished == true ? "Завершен" : "Не завершен"});
                    break;
                case var cls when cls == typeof(RequisitesObject):
                    dataReady.Columns.Add("Тип");
                    dataReady.Columns.Add("Название");
                    dataReady.Columns.Add("Юр. адрес");
                    dataReady.Columns.Add("ИНН");
                    dataReady.Columns.Add("Ген. директор");
                    dataReady.Columns.Add("Роль");
                    foreach (var item in items as List<RequisitesObject>)
                        dataReady.Rows.Add(new object[6] { item.Type.Name, item.Name, item.LegalAddress, item.Inn, item.Ceo, item.Role.Name });
                    break;
                case var cls when cls == typeof(RolesObject):
                    dataReady.Columns.Add("Название");
                    foreach (var item in items as List<RolesObject>)
                        dataReady.Rows.Add(new object[1] { item.Name });
                    break;
                case var cls when cls == typeof(VehiclesTypesObject):
                    dataReady.Columns.Add("Название");
                    foreach (var item in items as List<VehiclesTypesObject>)
                        dataReady.Rows.Add(new object[1] { item.Name });
                    break;
                case var cls when cls == typeof(VehiclesObject):
                    dataReady.Columns.Add("Тип");
                    dataReady.Columns.Add("Номер тягача");
                    dataReady.Columns.Add("Номер прицепа");
                    dataReady.Columns.Add("Владелец");
                    foreach (var item in items as List<VehiclesObject>)
                        dataReady.Rows.Add(new object[4] { item.Type.Name, item.Number, item.TrailerNumber, item.Owner.Name });
                    break;
                case var cls when cls == typeof(VehiclesObject):
                    dataReady.Columns.Add("Название");
                    foreach (var item in items as List<RouteActionsObject>)
                        dataReady.Rows.Add(new object[1] { item.Action });
                    break;
            }
            return dataReady;
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Экспорт"; // Default file name
            dlg.DefaultExt = ".xlsx"; // Default file extension
            dlg.Filter = "Документ Excel (.xlsx)|*.xlsx"; // Filter files by extension
            var result = dlg.ShowDialog();
            string filename = "";
            var content = DataGridFrame.Content;
            if (result == true)
            {
                filename = dlg.FileName;
                await Task.Run(() =>
                {
                    try
                    {
                        switch (content)
                        {
                            case CargoTablePage:
                                ExcelProvider.GenerateExcel(ToDataTableReady((content as CargoTablePage).CargoObjects), filename);
                                break;
                            case CargoTypesPage:
                                ExcelProvider.GenerateExcel(ToDataTableReady((content as CargoTypesPage).CargoTypes), filename);
                                break;
                            case DriverLicenceTablePage:
                                ExcelProvider.GenerateExcel(ToDataTableReady((content as DriverLicenceTablePage).DriversLicenceReadies), filename);
                                break;
                            case DriversTablePage:
                                ExcelProvider.GenerateExcel(ToDataTableReady((content as DriversTablePage).Drivers), filename);
                                break;
                            case RequestsTablePage:
                                //ExcelProvider.GenerateExcel(ToDataTableReady((content as RequestsTablePage).RequestsReadyObjects), filename);
                                break;
                            case RequisitesTablePage:
                                ExcelProvider.GenerateExcel(ToDataTableReady((content as RequisitesTablePage).Requisites), filename);
                                break;
                            case RolesTablePage:
                                ExcelProvider.GenerateExcel(ToDataTableReady((content as RolesTablePage).Roles), filename);
                                break;
                            case VehiclesTypesTablePage:
                                ExcelProvider.GenerateExcel(ToDataTableReady((content as VehiclesTypesTablePage).Types), filename);
                                break;
                            case RouteActionsTablePage:
                                ExcelProvider.GenerateExcel(ToDataTableReady((content as RouteActionsTablePage).RouteActions), filename);
                                break;
                            case VehiclesTablePage:
                                ExcelProvider.GenerateExcel(ToDataTableReady((content as VehiclesTablePage).Vehicles), filename);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Возникла ошибка {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });

            }
        }

        private async Task AdvancesSearchCollapsed()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(500);
            });

        }

        private async void AdvancedSearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (AdvancedSearch.Visibility == Visibility.Visible)
            {
                Storyboard? sb = Resources["CloseAdvancedSearch"] as Storyboard;
                ChevronSearch.Kind = PackIconKind.ChevronDown;
                sb!.Begin(AdvancedSearch);
                await AdvancesSearchCollapsed();
                AdvancedSearch.Visibility = Visibility.Collapsed;
                SimpleSearchPanel.IsEnabled = true;
            }
            else
            {
                SimpleSearchPanel.IsEnabled = false;
                AdvancedSearch.Visibility = Visibility.Visible;
                ChevronSearch.Kind = PackIconKind.ChevronUp;
                Storyboard? sb = Resources["OpenAdvancedSearch"] as Storyboard;
                sb!.Begin(AdvancedSearch);
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            startWindow.ChangePage(new SettingsPage());
        }

        private void FastSearchCall()
        {
            var text = SearchTextBox.Text;
            if (SearchFilter.Items.Count > 0 && startWindow.IsConnected == true)
                switch (DataGridFrame.Content)
                {
                    case CargoTablePage:
                        (DataGridFrame.Content as CargoTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case CargoTypesPage:
                        (DataGridFrame.Content as CargoTypesPage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case DriverLicenceTablePage:
                        (DataGridFrame.Content as DriverLicenceTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case DriversTablePage:
                        (DataGridFrame.Content as DriversTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case RequisitesTablePage:
                        (DataGridFrame.Content as RequisitesTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case RequisiteTypesTablePage:
                        (DataGridFrame.Content as RequisiteTypesTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case RolesTablePage:
                        (DataGridFrame.Content as RolesTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case VehiclesTablePage:
                        (DataGridFrame.Content as VehiclesTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case VehiclesTypesTablePage:
                        (DataGridFrame.Content as VehiclesTypesTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case RequestsTablePage:
                        (DataGridFrame.Content as RequestsTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case UsersTablePage:
                        (DataGridFrame.Content as UsersTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                    case RouteActionsTablePage:
                        (DataGridFrame.Content as RouteActionsTablePage)!.FastSearch(text, SearchFilter.SelectedItem.ToString());
                        break;
                }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FastSearchCall();
        }

        private void SearchFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FastSearchCall();
        }

    }

}
