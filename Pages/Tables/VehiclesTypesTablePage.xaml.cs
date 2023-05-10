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
    /// Логика взаимодействия для VehiclesTypesTablePage.xaml
    /// </summary>
    public partial class VehiclesTypesTablePage : Page
    {
        public List<VehiclesTypesObject> Types { get; set; }
        private Locale locale;

        StartWindow startWindow;
        public VehiclesTypesTablePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            string tableName = "типы транспорта";
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
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                var item = dataGrid.SelectedItem as VehiclesTypesObject;
                startWindow.client.DeleteVehiclesTypeAsync(new GetOrDeleteVehiclesTypesRequest { Id = item.Id }, startWindow.headers);
                Types.Remove(item);

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = Types;
            }
        }

        private async void SetData()
        {
            try
            {
                var item = await startWindow.client.GetListVehiclesTypesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                Types = new List<VehiclesTypesObject>();
                Types.AddRange(item.VehiclesTypes.ToList());
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = Types;
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
