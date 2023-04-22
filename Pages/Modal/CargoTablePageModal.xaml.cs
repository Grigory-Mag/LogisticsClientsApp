using ApiService;
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
        public CargoObject cargoObjects;

        public CargoTablePageModal()
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

        List<string> cargoTypeList;

        public void UpdateDisplayedData(CargoObject cargoObject)
        {           
            this.cargoObjects = cargoObject;
            cargoTypeList = new List<string>
            {
                cargoObject.CargoType.Name,
            };
            WeightTextBox.Text = cargoObjects.Weight.ToString();
            VolumeTextBox.Text = cargoObjects.Volume.ToString();
            NameTextBox.Text = cargoObjects.Name.ToString();
            PriceTextBox.Text = cargoObjects.Price.ToString();
            TypeComboBox.ItemsSource = cargoTypeList;
            TypeComboBox.SelectedItem = cargoTypeList.First();
            
        }
        /*
        public static CargoTablePageModal getInstance()
        {
            if (instance == null)
                instance = new CargoTablePageModal();

            return instance;
        }*/

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }
    }
}
