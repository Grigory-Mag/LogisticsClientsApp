using ApiService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages.Tables;
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
using static LogisticsClientsApp.Pages.Tables.RequestsTablePage;
using static System.Net.Mime.MediaTypeNames;

namespace LogisticsClientsApp.Pages.Modal
{
    /// <summary>
    /// Логика взаимодействия для RequestsTablePageModal.xaml
    /// </summary>
    public partial class RequestsTablePageModal : UserControl
    {
        public RequestsObject data = new RequestsObject();
        public ListVehicles vehicles;
        public ListRequisites transporters;
        public ListRequisites customers;
        public ListDrivers drivers;
        public ListCargo cargos;
        public ListRouteActions routeActions;
        public ListRouteObjects routes;

        public List<RouteObjectReady> routeObjectsReady = new List<RouteObjectReady>();
        public List<DriversObjectReady> driversReady = new List<DriversObjectReady>();
        public List<VehiclesObjectReady> vehiclesReady = new List<VehiclesObjectReady>();

        private Locale locale;
        public byte mode = 0;
        public string text = "Обновить";
        private StartWindow startWindow;
        public static RequestsTablePageModal PageInstance;

        public class DriversObjectReady
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string Patronymic { get; set; }
            public string FIO { get; set; }
            public DriverLicenceObject Licence { get; set; }

            public DriversObjectReady(DriversObject item)
            {
                Id = item.Id;
                Name = item.Name;
                Surname = item.Surname;
                Patronymic = item.Patronymic;
                FIO = $"{Surname} {Name} {Patronymic}";
                Licence = item.Licence;
            }
        }

        public class VehiclesObjectReady
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string NumberAndTrailer { get; set; }

            public VehiclesObjectReady(VehiclesObject item)
            {
                Id = item.Id;
                NumberAndTrailer = $"{item.Number} - {item.TrailerNumber}";
            }
        }

        public class RouteObjectReady
        {
            public int Id { get; set; }
            public string Address { get; set; }
            public RouteActionsObject Action { get; set; }
            public DateTime ActionDate { get; set; }

            public RouteObjectReady()
            {
                Address = "";
                ActionDate = DateTime.Now;
            }

            public RouteObjectReady(int id, string address, RouteActionsObject action, Timestamp actionDate)
            {
                Id = id;
                Address = address;
                Action = action;
                ActionDate = actionDate.ToDateTime();
            }

            public RouteObjectReady(RouteObject x)
            {
                Id = x.Id;
                Address = x.Address;
                Action = x.Action;
                ActionDate = x.ActionDate.ToDateTime();
            }

            public static explicit operator RouteObject(RouteObjectReady item)
            {
                return new RouteObject
                {
                    Id = item.Id,
                    Action = item.Action,
                    ActionDate = Timestamp.FromDateTime(item.ActionDate.ToUniversalTime()),
                    Address = item.Address,
                };
            }
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

        public RequestsTablePageModal()
        {
            InitializeComponent();
        }

        public static RequestsTablePageModal CreateInstance()
        {
            if (PageInstance == null)
                PageInstance = new RequestsTablePageModal();

            return PageInstance;
        }

        private void ModalPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadModalPage();
        }

        public void LoadModalPage()
        {
            if (IsLoaded)
            {
                startWindow = (StartWindow)Window.GetWindow(this);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
                Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
                SetLinkedData();
                Locale locale = new Locale(startWindow.selectedLocale);
                locale.SetLocale(this);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = RoutesDataGrid.SelectedItem as RouteObjectReady;
#warning TODO

            RoutesDataGrid.UnselectAllCells();
            RoutesDataGrid.ItemsSource = null;
            if (routeObjectsReady.Count > 0)
                routeObjectsReady.Remove(item);          

            RoutesDataGrid.ItemsSource = routeObjectsReady;
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

        public async void SetLinkedData()
        {
            transporters = await startWindow.client.GetListRequisitesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            drivers = await startWindow.client.GetListDriversAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            cargos = await startWindow.client.GetListCargoAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            vehicles = await startWindow.client.GetListVehiclesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            routeActions = await startWindow.client.GetListRouteActionsAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            routes = await startWindow.client.GetListRouteAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);

            driversReady.Clear();
            vehiclesReady.Clear();
            drivers.Drivers.ToList().ForEach(x => driversReady.Add(new DriversObjectReady(x)));
            vehicles.Vehicle.ToList().ForEach(x => vehiclesReady.Add(new VehiclesObjectReady(x)));

            TransporterComboBox.ItemsSource = transporters.Requisites;
            CustomerComboBox.ItemsSource = transporters.Requisites;
            DriverComboBox.ItemsSource = driversReady;
            CargoComboBox.ItemsSource = cargos.Cargo;
            VehicleComboBox.ItemsSource = vehiclesReady;

            ActionsComboBox.ItemsSource = routeActions.RouteActionsObject;


            if (data != null)
            {
                IsFinishedTextBox.IsChecked = data.IsFinished;
                DocumentsTextBox.IsChecked = data.Documents;
                if (data.TransporterReq != null)
                    TransporterComboBox.SelectedItem = transporters.Requisites.First(x => x.Id == data.TransporterReq.Id);
                if (data.CustomerReq != null)
                    CustomerComboBox.SelectedItem = transporters.Requisites.First(x => x.Id == data.CustomerReq.Id);
                if (data.Driver != null)
                    DriverComboBox.SelectedItem = driversReady.First(x => x.Id == data.Driver.Id);
                if (data.Vehicle != null)
                    VehicleComboBox.SelectedItem = vehiclesReady.First(x => x.Id == data.Vehicle.Id);
                if (data.Cargo != null)
                    CargoComboBox.SelectedItem = cargos.Cargo.First(x => x.Id == data.Cargo.Id);
                if (data.Routes != null)
                {
                    data.Routes.RouteObjects.ToList().ForEach(x => routeObjectsReady.Add(new RouteObjectReady(x)));
                    RoutesDataGrid.ItemsSource = routeObjectsReady;
                }

            }
            if (data.Routes == null)
                RoutesDataGrid.ItemsSource = routeObjectsReady;
        }

        public void UpdateDisplayedData(RequestsObject data)
        {
            this.data = data;
            NumberTextBox.Text = data.Id.ToString();
            PriceTextBox.Text = data.Price.ToString();
            DatePicker.SelectedDate = data.CreationDate.ToDateTime();
            if (startWindow != null)
                SetLinkedData();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }

        private void LicenceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (sender as ComboBox);
            //var number = comboBox.SelectedItem.ToString();
            //var numberSeries = number.Split("/");
            //var foundedData = licenses.DriverLicence.Where(item => item.Series == int.Parse(numberSeries[0].ToString()) && item.Number == int.Parse(numberSeries[1].ToString())).ToList();
        }

        private async void UpdateData()
        {
            try
            {
                var reqResult = new RequestsObject();
                if (mode == 0)
                    reqResult = await startWindow.client.UpdateRequestAsync(new CreateOrUpdateRequestObjRequest { Requests = data }, startWindow.headers);
                if (mode == 1)
                    reqResult = await startWindow.client.CreateRequestAsync(new CreateOrUpdateRequestObjRequest { Requests = data }, startWindow.headers);
                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as RequestsTablePage;
                if (mode == 0)
                {
                    var index = page.Requests.FindIndex(t => t.Id == reqResult.Id);
                    page.Requests[index] = reqResult;
                }
                if (mode == 1)
                    page.Requests.Add(reqResult);
                page.SetDataGridItems();

                ShowToast(TablePage.Messages.Success);
            }
            catch (RpcException ex)
            {
                ShowToast(TablePage.Messages.Error);
                MessageBox.Show($"Возникла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder changedDataNotify = new StringBuilder();

            if (mode == 0)
            {
                if (PriceTextBox.Text != data.Price.ToString())
                    changedDataNotify.Append($"Цена: {data.Price} -> {PriceTextBox.Text}\n");
                if (DatePicker.SelectedDate.Value.Date != data.CreationDate.ToDateTime().Date)
                    changedDataNotify.Append($"Дата создания: {data.CreationDate.ToDateTime().Date} -> {DatePicker.SelectedDate.Value.Date}\n");
                if (IsFinishedTextBox.IsChecked != data.IsFinished)
                    changedDataNotify.Append($"Заявка завершена: {(IsFinishedTextBox.IsChecked == true ? "Да" : "Нет")}\n");
                if (DocumentsTextBox.IsChecked != data.Documents)
                    changedDataNotify.Append($"Оригинал документов: {(DocumentsTextBox.IsChecked == true ? "Да" : "Нет")}\n");
                if ((VehicleComboBox.SelectedItem as VehiclesObjectReady)!.Id != data.Vehicle.Id)
                    changedDataNotify.Append($"Номер тягача/номер прицепа: {data.Vehicle.Number}/{data.Vehicle.TrailerNumber} -> " +
                        $"{(VehicleComboBox.SelectedItem as VehiclesObjectReady).NumberAndTrailer}\n");
                if ((TransporterComboBox.SelectedItem as RequisitesObject)!.Id != data.TransporterReq.Id)
                    changedDataNotify.Append($"Перевозчик: {data.TransporterReq.Name} -> {(TransporterComboBox.SelectedItem as RequisitesObject).Name}\n");
                if ((DriverComboBox.SelectedItem as DriversObjectReady).Id != data.Driver.Id)
                    changedDataNotify.Append($"Водитель: {data.Driver.Name}" +
                        $" {data.Driver.Surname} {data.Driver.Patronymic}" +
                        $" ({data.Driver.Licence.Series}/{data.Driver.Licence.Number})" +
                        $" -> {(DriverComboBox.SelectedItem as DriversObjectReady)!.Name}" +
                        $" {(DriverComboBox.SelectedItem as DriversObjectReady)!.Surname}" +
                        $" {(DriverComboBox.SelectedItem as DriversObjectReady)!.Patronymic} " +
                        $"({(DriverComboBox.SelectedItem as DriversObjectReady)!.Licence.Series}/" +
                        $"{(DriverComboBox.SelectedItem as DriversObjectReady)!.Licence.Number})\n");
                if ((CargoComboBox.SelectedItem as CargoObject)!.Id != data.Cargo.Id)
                    changedDataNotify.Append($"Груз: {data.Cargo.Name} -> {(CargoComboBox.SelectedItem as CargoObject)!.Name}\n");
                if ((CustomerComboBox.SelectedItem as RequisitesObject)!.Id != data.CustomerReq.Id)
                    changedDataNotify.Append($"Заказчик: {data.CustomerReq.Name} -> {(CustomerComboBox.SelectedItem as RequisitesObject)!.Name}\n");
            }

            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", $"{text}", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                var listRoutes = new ListRouteObjects();
                var routesItems = RoutesDataGrid.ItemsSource as List<RouteObjectReady>;
                routesItems.ForEach(x => listRoutes.RouteObjects.Add((RouteObject)x));

                try
                {
                    if ((CustomerComboBox.SelectedItem as RequisitesObject)!.Id == (TransporterComboBox.SelectedItem as RequisitesObject)!.Id)
                        throw new InvalidOperationException();
                    data.Price = double.Parse(PriceTextBox.Text);
                    data.CreationDate = Timestamp.FromDateTime(DatePicker.SelectedDate!.Value.AddDays(1).ToUniversalTime());
                    data.Vehicle = vehicles.Vehicle.First(x => x.Id == (VehicleComboBox.SelectedItem as VehiclesObjectReady)!.Id);
                    data.CustomerReq = CustomerComboBox.SelectedItem as RequisitesObject;
                    data.TransporterReq = TransporterComboBox.SelectedItem as RequisitesObject;
                    data.Driver = drivers.Drivers.First(x => x.Id == (DriverComboBox.SelectedItem as DriversObjectReady)!.Id);
                    data.Cargo = CargoComboBox.SelectedItem as CargoObject;
                    data.IsFinished = IsFinishedTextBox.IsChecked == null ? false : (bool)IsFinishedTextBox.IsChecked;
                    data.Documents = DocumentsTextBox.IsChecked == null ? false : (bool)DocumentsTextBox.IsChecked;
                    data.Routes = listRoutes;
                    UpdateData();
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        case RpcException:
                            MessageBox.Show($"Возникла ошибка. Обратитесь к администратору\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                        case InvalidOperationException:
                            MessageBox.Show($"Проверьте, что заказчик и перевозчик не являеются одной и той же организацией", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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