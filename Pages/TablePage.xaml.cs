using MaterialDesignThemes.Wpf;
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
using LogisticsClientsApp.Providers;
using ApiService;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Xaml.Behaviors.Core;
using System.Resources;
using LogisticsClientsApp.Localizations.Data;
using LogisticsClientsApp.Localizations;
using System.DirectoryServices.ActiveDirectory;
using System.Runtime.InteropServices;
using System.Reflection;
using LogisticsClientsApp.Pages.Tables;
using System.Globalization;
using LogisticsClientsApp.Validations;
using LogisticsClientsApp.Pages.Modal;

namespace LogisticsClientsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {

        public PaletteHelper palette;
        public Page selectedPage { get; set; }
        Locale locale;
        public CargoTablePage cargoTablePageInstance;
        ITheme theme = new PaletteHelper().GetTheme();
        StartWindow startWindow;

        public TablePage()
        {
            InitializeComponent();

            //SetData();
            //test();
        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            cargoTablePageInstance = new CargoTablePage();

            startWindow.ClearFrameHistory();
            startWindow.MainGrid.Background = new SolidColorBrush(Colors.Transparent);
            //DataGridFrame.Navigate(cargoTablePageInstance);

            startWindow.LeftMenu.Visibility = Visibility.Visible;
            startWindow.MenuOpenBtn.Visibility = Visibility.Visible;


            palette = new PaletteHelper();
            PageInitialize();

        }

        public async Task<bool> PageInitialize()
        {
            test();
            //await SetData();
            var a = DataGridFrame.Content;
            locale = new Locale(startWindow.selectedLocale);
            ChangeSelectedTable(new CargoTablePage());
            return true;
        }

        public string IdHeader;
        public Binding binding = new Binding();
        private void test()
        {
            IdHeader = RU.SearchField;
            binding.ElementName = "Hint";
            binding.Path = new PropertyPath("Text");
            HintAssist.SetHint(testBix, IdHeader);
        }
        public async Task<int> SetData()
        {
            CargoObject cargo = await GetTestData();
            List<CargoObject> objects = new List<CargoObject>();
            objects.Add(cargo);
            cargoTablePageInstance.dataGrid.ItemsSource = objects;

            return 1;
        }

        public async Task<CargoObject> GetTestData()
        {          
            try
            {
                var item = await startWindow.client.GetCargoAsync(new GetOrDeleteCargoRequest { Id = 1 }, startWindow.headers);
                return await Task.FromResult(item);
            }
            catch (RpcException ex)
            {
                return await Task.FromResult(new CargoObject());
            }

        }

        private Dictionary<string, List<object>> buttonsReferences = new Dictionary<string, List<object>>();

        private List<bool> selectedBtns = new List<bool>();
        private List<TextBlock> textBlocks = new List<TextBlock>();
        private List<PackIcon> packIcons = new List<PackIcon>();
        private List<Button> buttons = new List<Button>();

        private static Color PRIMARY_COLOR = Color.FromArgb(255, 33, 150, 243);

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleDarkMode_Checked(object sender, RoutedEventArgs e)
        {
            theme.SetBaseTheme(Theme.Dark);
            palette.SetTheme(theme);
        }

        private void ToggleDarkMode_Unchecked(object sender, RoutedEventArgs e)
        {
            theme.SetBaseTheme(Theme.Light);
            palette.SetTheme(theme);
        }

        private void PrevTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public class NumericValidationRule : ValidationRule
        {
            public Type ValidationType { get; set; }
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                string strValue = Convert.ToString(value);

                if (string.IsNullOrEmpty(strValue))
                    return new ValidationResult(false, $"Value cannot be coverted to string.");
                bool canConvert = false;
                switch (ValidationType.Name)
                {
                    
                    case "Boolean":
                        bool boolVal = false;
                        canConvert = bool.TryParse(strValue, out boolVal);
                        return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input should be type of boolean");
                    case "Int32":
                        int intVal = 0;
                        canConvert = int.TryParse(strValue, out intVal);
                        return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input should be type of Int32");
                    case "Double":
                        double doubleVal = 0;
                        canConvert = double.TryParse(strValue, out doubleVal);
                        return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input should be type of Double");
                    case "Int64":
                        long longVal = 0;
                        canConvert = long.TryParse(strValue, out longVal);
                        return canConvert ? new ValidationResult(true, null) : new ValidationResult(false, $"Input should be type of Int64");
                    default:
                        throw new InvalidCastException($"{ValidationType.Name} is not supported");
                }
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        public void ChangeSelectedTable(Page page)
        {
            DataGridFrame.Navigate(page);
            selectedPage = page;
            locale.SetLocale(this);
        }

        public void ShowModalPage()
        {
            switch (DataGridFrame.Content)
            {
                case CargoTablePage:
                    CargoTablePageModal cargoTablePage = new CargoTablePageModal();
                    ModalPageFrame.Content = cargoTablePage;
                    var modalPage = DataGridFrame.Content as CargoTablePage;
                    cargoTablePage.UpdateDisplayedData(modalPage.dataGrid.SelectedItem as CargoObject);

                    Storyboard? sb = cargoTablePage.Resources["OpenModal"] as Storyboard;
                    sb!.Begin(ModalPageFrame);
                    break;
                case CargoTypesPage:
                    locale.SetLocale(this);
                    break;
            }
            MainPanel.Opacity = .5;
            MainPanel.IsEnabled = false;            
        }
    }

}
