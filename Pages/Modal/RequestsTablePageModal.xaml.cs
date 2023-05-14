using ApiService;
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

        private Locale locale;
        public byte mode = 0;
        public List<CustomTrash> dataMobile { get; set; }
        StartWindow startWindow;

        public class CustomTrash
        {
            public int Number { get; set; }
            public string Address { get; set; }
            public string Action { get; set; }
            public DateTime SelectedDate { get; set; }

            public CustomTrash() 
            {
                Number = 0;
                Action = "Разгрузить";
                SelectedDate = DateTime.Now;
            }

            public CustomTrash(int number, string address, DateTime selectedDate, string action)
            {
                Number = number;
                Address = address;
                SelectedDate = selectedDate;
                Action = action;
            }
        }

        public RequestsTablePageModal()
        {
            InitializeComponent();

            dataMobile = new List<CustomTrash>()
            {
                new CustomTrash(1, "0_адрес", DateTime.Now, "Разгрузить"),
                new CustomTrash(2, "1_адрес", DateTime.Now.AddDays(-1), "Загрузить"),
                new CustomTrash(3, "2_адрес", DateTime.Now.AddDays(-2), "Разгрузить"),
                new CustomTrash(4, "3_адрес", DateTime.Now.AddDays(-3), "Загрузить"),
                new CustomTrash(5, "4_адрес", DateTime.Now.AddDays(-4), "Разгрузить"),
                new CustomTrash(6, "5_адрес", DateTime.Now.AddDays(-5), "Загрузить"),
            };
            //Aboba.ItemsSource = new List<string>() { "Разгрузить", "Загрузить" };
            //RoutesDataGrid.ItemsSource = dataMobile;
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }
        private void ModalPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            SetLinkedData();
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

        public async void SetLinkedData()
        {
            transporters = await startWindow.client.GetListRequisitesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            drivers = await startWindow.client.GetListDriversAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            cargos = await startWindow.client.GetListCargoAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            vehicles = await startWindow.client.GetListVehiclesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            routeActions = await startWindow.client.GetListRouteActionsAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            routes = await startWindow.client.GetListRouteAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);

            TransporterComboBox.ItemsSource = transporters.Requisites;
            CustomerComboBox.ItemsSource = transporters.Requisites;
            DriverComboBox.ItemsSource = drivers.Drivers;
            CargoComboBox.ItemsSource = cargos.Cargo;
            VehicleComboBox.ItemsSource = vehicles.Vehicle;

            ActionsComboBox.ItemsSource = routeActions.RouteActionsObject;
            RoutesDataGrid.ItemsSource = routes.RouteObjects;

            if (data != null)
            {
                if (data.TransporterReq != null)
                    TransporterComboBox.SelectedItem = transporters.Requisites.First(x => x.Id == data.TransporterReq.Id);
                if (data.CustomerReq != null)
                    CustomerComboBox.SelectedItem = transporters.Requisites.First(x => x.Id == data.CustomerReq.Id);
                if (data.Driver != null)
                    DriverComboBox.SelectedItem = drivers.Drivers.First(x => x.Id == data.Driver.Id);
                if (data.Vehicle != null)
                    VehicleComboBox.SelectedItem = vehicles.Vehicle.First(x => x.Id == data.Vehicle.Id);
                if (data.Cargo != null)
                    CargoComboBox.SelectedItem = cargos.Cargo.First(x => x.Id == data.Cargo.Id);
            }
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
            /*            try
                        {
                            var reqResult = new RequisitesObject();
                            if (mode == 0)
                                reqResult = await startWindow.client.UpdateRequisiteAsync(new CreateOrUpdateRequisitesRequest { Requisite = data });
                            if (mode == 1)
                                reqResult = await startWindow.client.CreateRequisiteAsync(new CreateOrUpdateRequisitesRequest { Requisite = data });
                            var tablePage = (TablePage)startWindow.MainFrameK.Content;
                            var page = tablePage.DataGridFrame.Content as RequisitesTablePage;
                            if (mode == 0)
                            {
                                var index = page.Requisites.FindIndex(t => t.Id == reqResult.Id);
                                page.Requisites[index] = reqResult;
                            }
                            if (mode == 1)
                                page.Requisites.Add(reqResult);
                            page.dataGrid.ItemsSource = null;
                            page.dataGrid.ItemsSource = page.Requisites;
                        }
                        catch (RpcException ex)
                        {

                        }*/

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            /*            StringBuilder changedDataNotify = new StringBuilder();

                        if (mode == 0)
                        {
                            if (NameTextBox.Text != data.Name.ToString())
                                changedDataNotify.Append($"Название: {data.Name} -> {NameTextBox.Text}\n");
                            if (CeoTextBox.Text != data.Ceo.ToString())
                                changedDataNotify.Append($"Владелец: {data.Ceo} -> {CeoTextBox.Text}\n");
                            if (InnTextBox.Text != data.Inn)
                                changedDataNotify.Append($"ИНН: {data.Inn} -> {InnTextBox.Text}\n");
                            if (AddressTextBox.Text != data.LegalAddress.ToString())
                                changedDataNotify.Append($"Юр. адрес: {data.LegalAddress} -> {AddressTextBox.Text}\n");
                            if ((RoleComboBox.SelectedItem as RolesObject)!.Name != data.Role.Name)
                                changedDataNotify.Append($"Роль: {data.Role.Name} -> {(RoleComboBox.SelectedItem as RolesObject)!.Name}\n");
                            if ((TypeComboBox.SelectedItem as RequisiteTypeObject)!.Name != data.Type.Name)
                                changedDataNotify.Append($"Роль: {data.Type.Name} -> {(TypeComboBox.SelectedItem as RequisiteTypeObject)!.Name}\n");
                        }

                        var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                        if (result == MessageBoxResult.Yes)
                        {
                            data.Name = NameTextBox.Text;
                            data.Ceo = CeoTextBox.Text;
                            data.Inn = InnTextBox.Text;
                            data.LegalAddress = AddressTextBox.Text;
                            data.Role = RoleComboBox.SelectedItem as RolesObject;
                            data.Type = TypeComboBox.SelectedItem as RequisiteTypeObject;
                            UpdateData();
                        }*/
        }
    }
}