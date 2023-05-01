using ApiService;
using Google.Protobuf.WellKnownTypes;
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
    /// Логика взаимодействия для VehiclesTypesTablePageModal.xaml
    /// </summary>
    public partial class VehiclesTypesTablePageModal : UserControl
    {
        public VehiclesTypesObject data;
        private Locale locale;

        StartWindow startWindow;

        public VehiclesTypesTablePageModal()
        {
            InitializeComponent();
        }

        private void ModalPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
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

        public void UpdateDisplayedData(VehiclesTypesObject data)
        {
            this.data = data;
            NameTextBox.Text = data.Name.ToString();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }

        private async void UpdateData()
        {
            try
            {
                var reqResult = await startWindow.client.UpdateVehiclesTypeAsync(new CreateOrUpdateVehiclesTypesRequest { VehiclesTypes = data });
                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as VehiclesTypesTablePage;
                var index = page.types.FindIndex(t => t.Id == reqResult.Id);
                page.types[index] = reqResult;
                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.types;
            }
            catch (RpcException ex)
            {

            }
            
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (data.Name != NameTextBox.Text && NameTextBox.Text != "")
            {
                var result = MessageBox.Show($"Применить изменения?\n {data.Name} -> {NameTextBox.Text}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                {
                    data.Name = NameTextBox.Text;
                    UpdateData();
                }

            }            
        }
    }
}
