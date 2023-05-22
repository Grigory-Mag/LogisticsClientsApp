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
    public partial class RequestsTablePage : Page, IMyPage
    {
        public List<RequestsObject> requests { get; set; }
        private Locale locale;

        StartWindow startWindow;
        public RequestsTablePage()
        {
            InitializeComponent();
        }

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
                DriverFIO = $"{Driver.Name} {Driver.Surname} {Driver.Patronymic}";
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            string tableName = "заявки";
            var tablePage = startWindow.MainFrameK.Content as TablePage;
            tablePage.TextBlockTableName.Text = tableName;
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

        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public RequestsObject GetSelectedDataGridItem()
        {
            return requests.First(x => x.Id == (dataGrid.SelectedItem as RequestsReady).Id);
        }

        public void SetDataGridItems()
        {
            var requestsReady = new List<RequestsReady>();
            requests.ForEach(x => requestsReady.Add((RequestsReady)x));
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = requestsReady;
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
            }
            catch (RpcException ex)
            {
                MessageBox.Show(ex.Message);
            }

            requests.Remove(requests.First(x => x.Id == item.Id));
            SetDataGridItems();

            OpenThisDialog.IsOpen = false;
        }

        private async void SetData()
        {
            try
            {
                var item = await startWindow.client.GetListRequestsAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                requests = new List<RequestsObject>();
                requests.AddRange(item.Requests.ToList());
                var requestsReady = new List<RequestsReady>();
                requests.ForEach(val => requestsReady.Add((RequestsReady)val));

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = requestsReady;
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
            requests.Clear();
            dataGrid.ItemsSource = null;
            BindingOperations.ClearAllBindings(dataGrid);
        }
    }
}

