using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для RequestsTablePage.xaml
    /// </summary>
    public partial class RequestsTablePage : Page
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
            public bool Documents { get; set; }
            public CargoObject Cargo { get; set; }
            public RequisitesObject CustomerReq { get; set; }
            public RequisitesObject TransporterReq { get; set; }
            public ListRouteObjects Routes { get; set; }

            public RequestsReady()
            {
            }

            public RequestsReady(int id, double? price, VehiclesObject vehicle, bool isFinished, DateTime creationDate, bool documents, CargoObject cargo, RequisitesObject customerReq, RequisitesObject transporterReq, ListRouteObjects routes)
            {
                Id = id;
                Price = price;
                Vehicle = vehicle;
                IsFinished = isFinished;
                CreationDate = creationDate;
                Documents = documents;
                Cargo = cargo;
                CustomerReq = customerReq;
                TransporterReq = transporterReq;
                Routes = routes;
            }

            public static explicit operator RequestsReady(RequestsObject item)
            {
                return new RequestsReady()
                {
                    Id = item.Id,
                    Price = item.Price,
                    Vehicle = item.Vehicle,
                    IsFinished = item.IsFinished,
                    Cargo = item.Cargo,
                    CustomerReq = item.CustomerReq,
                    CreationDate = item.CreationDate.ToDateTime(),
                    Documents = item.Documents,
                    Routes = item.Routes,
                    TransporterReq = item.TransporterReq
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
        }

        private void PrevTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = dataGrid.SelectedItem;
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
    }
}

