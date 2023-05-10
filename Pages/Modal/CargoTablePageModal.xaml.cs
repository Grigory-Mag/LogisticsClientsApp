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
    /// Логика взаимодействия для CargoTablePageModal.xaml
    /// </summary>
    public partial class CargoTablePageModal : UserControl
    {
        private static CargoTablePageModal instance;
        public StartWindow startWindow;
        public CargoObject data = new CargoObject();
        public ListCargoType cargoTypes;
        public byte mode = 0;

        public CargoTablePageModal()
        {
            InitializeComponent();
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

        public void UpdateDisplayedData(CargoObject cargoObject)
        {
            this.data = cargoObject;
            WeightTextBox.Text = data.Weight.ToString();
            VolumeTextBox.Text = data.Volume.ToString();
            NameTextBox.Text = data.Name.ToString();
            PriceTextBox.Text = data.Price.ToString();
            ConstraintsTextBox.Text = data.Constraints.ToString();
            if (startWindow != null)
                SetLinkedData();

        }

        public async void SetLinkedData()
        {
            cargoTypes = await startWindow.client.GetListCargoTypesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            TypeComboBox.ItemsSource = cargoTypes.CargoType;
            TypeComboBox.SelectedItem = data.CargoType == null ? null : cargoTypes.CargoType.First(x => x.Id == data.CargoType.Id);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }

        private async void UpdateData()
        {
            try
            {
                CargoObject reqResult = new CargoObject();
                if (mode == 0)
                    reqResult = await startWindow.client.UpdateCargoAsync(new CreateOrUpdateCargoRequest { Cargo = data }, startWindow.headers);
                if (mode == 1)
                    reqResult = await startWindow.client.CreateCargoAsync(new CreateOrUpdateCargoRequest { Cargo = data }, startWindow.headers);

                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as CargoTablePage;
                if (mode == 0)
                {
                    var index = page.CargoObjects.FindIndex(t => t.Id == reqResult.Id);
                    page.CargoObjects[index] = reqResult;
                }
                if (mode == 1)                
                    page.CargoObjects.Add(reqResult);
                
                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.CargoObjects;
                page.dataGrid.Items.Refresh();
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
                if (WeightTextBox.Text != data.Weight.ToString())
                    changedDataNotify.Append($"Масса: {data.Weight} -> {WeightTextBox.Text}\n");
                if (VolumeTextBox.Text != data.Volume.ToString())
                    changedDataNotify.Append($"Объём: {data.Volume} -> {VolumeTextBox.Text}\n");
                if (NameTextBox.Text != data.Name.ToString())
                    changedDataNotify.Append($"Название: {data.Name} -> {NameTextBox.Text}\n");
                if (PriceTextBox.Text != data.Price.ToString())
                    changedDataNotify.Append($"Цена: {data.Price} -> {PriceTextBox.Text}\n");
                if (ConstraintsTextBox.Text != data.Constraints.ToString())
                    changedDataNotify.Append($"Ограничения: {data.Constraints} -> {ConstraintsTextBox.Text}\n");
                if ((TypeComboBox.SelectedItem as CargoTypesObject)!.Name != data.CargoType.Name.ToString())
                    changedDataNotify.Append($"Тип груза: {data.CargoType.Name} -> {(TypeComboBox.SelectedItem as CargoTypesObject).Name}\n");
            }

            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                data.Weight = double.Parse(WeightTextBox.Text);
                data.Volume = double.Parse(VolumeTextBox.Text);
                data.Name = NameTextBox.Text;
                data.Price = double.Parse(PriceTextBox.Text);
                data.Constraints = ConstraintsTextBox.Text;
                data.CargoType = TypeComboBox.SelectedItem as CargoTypesObject;
                UpdateData();
            }
        }
    }
}
