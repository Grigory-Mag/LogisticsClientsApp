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

namespace LogisticsClientsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {
        class TestObject
        {
            public string phone { get; set; }
            public int number { get; set; }

            public TestObject(string phone, int number)
            {
                this.phone = phone;
                this.number = number;
            }
        }
        public PaletteHelper palette;
        ITheme theme = new PaletteHelper().GetTheme();

        public TablePage()
        {
            InitializeComponent();

            palette = new PaletteHelper();
            List<TestObject> phones = new List<TestObject>()
            {
                new TestObject("123211", 1),
                new TestObject("Clowns", 2),
                new TestObject("abb1234", 3)
            };
            phonesGrid.ItemsSource = phones;

            //var DataProvider = new DataProvider();
            //CargoObject result = DataProvider.getTestData().Result;
            SetData();
            //List<CargoObject> objects = new List<CargoObject>();
            //objects.Add(cargo);
            //phonesGrid.ItemsSource = objects;

        }
        public async Task<int> SetData()
        {
            CargoObject cargo = await GetTestData();
            List<CargoObject> objects = new List<CargoObject>();
            objects.Add(cargo);
            phonesGrid.ItemsSource = objects;

            return 1;
        }

        public async Task<CargoObject> GetTestData()
        {
            UserService.UserServiceClient client = new UserService.UserServiceClient(GrpcChannel.ForAddress("http://localhost:5088"));
            try
            {
                var item = await client.GetCargoAsync(new GetOrDeleteCargoRequest { Id = 1 });
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
    }

}
