using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages.Tables;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticsClientsApp.Pages.Modal
{
    /// <summary>
    /// Логика взаимодействия для VehiclesTablePageModal.xaml
    /// </summary>
    public partial class VehiclesTablePageModal : UserControl
    {
        public VehiclesObject data = new VehiclesObject();
        public ListRequisites requisites;
        public ListVehiclesTypes types;
        private Locale locale;
        public byte mode = 0;

        StartWindow startWindow;

        public VehiclesTablePageModal()
        {
            InitializeComponent();
        }

        private void ModalPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            Locale locale = new Locale(startWindow.selectedLocale);
            locale.SetLocale(this);
            SetLinkedData();
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
            requisites = await startWindow.client.GetListRequisitesAsync(new Google.Protobuf.WellKnownTypes.Empty());
            CeoComboBox.ItemsSource = requisites.Requisites;
            if (data.Id != 0)
                CeoComboBox.SelectedItem = requisites.Requisites.First(x => x.Id == data.Owner.Id);
            types = await startWindow.client.GetListVehiclesTypesAsync(new Google.Protobuf.WellKnownTypes.Empty());
            TypeComboBox.ItemsSource = types.VehiclesTypes;
            if (data.Type != null)
                TypeComboBox.SelectedItem = types.VehiclesTypes.First(x => x.Id == data.Type.Id);
        }

        public void UpdateDisplayedData(VehiclesObject data)
        {
            this.data = data;
            NumberTextBox.Text = data.Number.ToString();
            TrailerNumberTextBox.Text = data.TrailerNumber.ToString();
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

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (sender as ComboBox);
            var selectedItem = comboBox.SelectedItem as VehiclesTypesObject;
        }

        private void CeoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void UpdateData()
        {
            try
            {
                var reqResult = new VehiclesObject();
                if (mode == 0)
                    reqResult = await startWindow.client.UpdateVehicleAsync(new CreateOrUpdateVehiclesRequest { Vehicle = data });
                if (mode == 1)
                    reqResult = await startWindow.client.CreateVehicleAsync(new CreateOrUpdateVehiclesRequest { Vehicle = data });
                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as VehiclesTablePage;
                if (mode == 0)
                {
                    var index = page.Vehicles.FindIndex(t => t.Id == reqResult.Id);
                    page.Vehicles[index] = reqResult;
                }
                if (mode == 1)
                    page.Vehicles.Add(reqResult);

                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.Vehicles;
            }
            catch (RpcException ex)
            {

            }

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder changedDataNotify = new StringBuilder();

            if (mode == 0)
            {
                if ((TypeComboBox.SelectedItem as VehiclesTypesObject)!.Name != data.Type.Name)
                    changedDataNotify.Append($"Тип: {data.Type.Name} -> {(TypeComboBox.SelectedItem as VehiclesTypesObject).Name}\n");
                if ((CeoComboBox.SelectedItem as RequisitesObject)!.Name != data.Owner.Name)
                    changedDataNotify.Append($"Владелец: {data.Owner.Name} -> {(CeoComboBox.SelectedItem as RequisitesObject).Name}\n");
                if (NumberTextBox.Text != data.Number)
                    changedDataNotify.Append($"Номер машины: {data.Number} -> {NumberTextBox.Text}");
                if (TrailerNumberTextBox.Text != data.TrailerNumber)
                    changedDataNotify.Append($"Номер прицепа: {data.TrailerNumber} -> {TrailerNumberTextBox.Text}");
            }


            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                data.Type = TypeComboBox.SelectedItem as VehiclesTypesObject;
                data.Owner = CeoComboBox.SelectedItem as RequisitesObject;
                data.Number = NumberTextBox.Text;
                data.TrailerNumber = TrailerNumberTextBox.Text;
                UpdateData();
            }
        }
    }
}
