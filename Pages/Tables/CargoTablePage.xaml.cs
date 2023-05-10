using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages.Modal;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

namespace LogisticsClientsApp.Pages.Tables
{
    /// <summary>
    /// Логика взаимодействия для CargoTablePage.xaml
    /// </summary>
    public partial class CargoTablePage : Page
    {
        public List<CargoObject> CargoObjects { get; set; }
        public List<string> SearchFields = new List<string>() { "Тип", "Ограничения", "Масса", "Объём", "Название", "Цена"};
        public TablePage tablePage;
        private Locale locale;

        StartWindow startWindow;
        public CargoTablePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            tablePage = (TablePage)startWindow.MainFrameK.Content;
            locale = new Locale(startWindow.selectedLocale);
            startWindow.SizeChanged += (o, e) =>
            {
                ResizeDataGrid();
            };
            CreateAdvancedSearchFields();
            SetData();
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                var item = dataGrid.SelectedItem as CargoObject;
                startWindow.client.DeleteCargoAsync(new GetOrDeleteCargoRequest { Id = item.Id });
                CargoObjects.Remove(item);
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = CargoObjects;
            }

        }

        private void CreateAdvancedSearchFields()
        {
            foreach (var item in SearchFields)
            {
                var textBox = new TextBox();
                var myResourceDictionary = new ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.TextBox.xaml", UriKind.RelativeOrAbsolute)
                };
                HintAssist.SetHint(textBox, item);
                textBox.Margin = new Thickness(0, 0, 20, 0);
                textBox.Style = myResourceDictionary["MaterialDesignOutlinedTextBox"] as Style;
                textBox.MaxWidth = 130;
                textBox.MinWidth = 130;
                textBox.FontSize = 16;
                textBox.MaxLength = 30;
                tablePage.AdvancedSearch.Children.Add(textBox);
            }
        }

        private async void SetData()
        {
            try
            {
                var item = await startWindow.client.GetCargoAsync(new GetOrDeleteCargoRequest { Id = 1 }, startWindow.headers);
                var item2 = await startWindow.client.GetListCargoAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                CargoObjects = new List<CargoObject>();
                CargoObjects.AddRange(item2.Cargo.ToList());
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = CargoObjects;
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
            /*
            tablePage.MainPanel.Opacity = .5;
            tablePage.MainPanel.IsEnabled = false;
            CargoTablePageModal cargoTablePage = new CargoTablePageModal();
            tablePage.ModalPageFrame.Content = cargoTablePage;
            
            var data = dataGrid.SelectedItem as CargoObject;

            cargoTablePage.UpdateDisplayedData(dataGrid.SelectedItem as CargoObject);
            */
        }

    }
}
