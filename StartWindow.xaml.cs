using ApiService;
using Grpc.Core;
using Grpc.Net.Client;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages;
using LogisticsClientsApp.Pages.Tables;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
using System.Windows.Shapes;

namespace LogisticsClientsApp
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public LoginPage LoginPage;
        public string selectedLocale = "ru";
        public Dictionary<string, List<string>> tables = new Dictionary<string, List<string>>();
        public UserService.UserServiceClient client;
        public Metadata headers = new Metadata();
        public List<string> tablesList = new List<string>();

        private UserService.UserServiceClient loginClient = new UserService.UserServiceClient(GrpcChannel.ForAddress(Properties.Default.Address.ToString()));
        private Dictionary<string, List<object>> buttonsReferences = new Dictionary<string, List<object>>();
        private List<bool> selectedBtns = new List<bool>();
        private List<TextBlock> textBlocks = new List<TextBlock>();
        private List<PackIcon> packIcons = new List<PackIcon>();
        private List<Button> buttons = new List<Button>();

        private static Color PRIMARY_COLOR = Color.FromArgb(255, 33, 150, 243);

        public StartWindow()
        {
            InitializeComponent();

            LoginPage = new LoginPage(this);
            ChangePage(LoginPage);


            Uri path = new Uri(Directory.GetCurrentDirectory() + @"\Resources\Images\loginBackground.jpg");
            MainGrid.Background = new ImageBrush(new BitmapImage(path));
            MenuOpenBtn.Click += Button_Click_1;
            MenuCloseBtn.Click += btnclose_Click;

            Locale locale = new Locale("ru");
            locale.SetLocale(this);

            InitElements();

            SelectBtn("References");

        }

        public void ChangePage(Page page)
        {
            MainFrameK.Navigate(page);
        }

        public void ClearFrameHistory()
        {
            MainFrameK.NavigationService.RemoveBackEntry();
            var data = MainFrameK.NavigationService;

            //while (MainFrameK.NavigationService.RemoveBackEntry() != null) ;
        }

        public void ToggleSideMenu(bool isToggle)
        {

        }

        private void DarkModeToggle_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void MainFrameK_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private void InitElements()
        {
            foreach (var item in tables)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Click += ReferencesMenu_SubMenu_Click;
                if (item.Value != null)
                    menuItem.ItemsSource = item.Value;
                menuItem.Header = item.Key;
                ReferencesMenu.Items.Add(menuItem);
            }

            buttonsReferences.Add("References", new List<object>()
            {   TextBlockReferences,
                ReferencesIcon,
                ReferencesBtn,
                true,
                MenuReferencesBtn,
                MenuReferencesIcon
            });

            buttonsReferences.Add("Messages", new List<object>()
            {  TextBlockMessages,
                MessagesIcon,
                MessagesBtn,
                false,
                MenuMessagesBtn,
                MenuMessagesIcon
            });

            buttonsReferences.Add("Email", new List<object>()
            {  TextBlockEmail,
                EmailIcon,
                EmailBtn,
                false,
                MenuEmailBtn,
                MenuEmailIcon
            });

            buttonsReferences.Add("Account", new List<object>()
            {  TextBlockAccount,
                AccountIcon,
                AccountBtn,
                false,
                MenuAccountBtn,
                MenuAccountIcon
            });
        }

        private void SetSelectedColor(string key)
        {
            //(sender as Button).Foreground = new SolidColorBrush(Colors.LightGray);
            (buttonsReferences[key][2] as Button)!.Background = new SolidColorBrush(PRIMARY_COLOR);
            (buttonsReferences[key][2] as Button)!.Background.Opacity = .7;

            (buttonsReferences[key][4] as Button)!.Background = new SolidColorBrush(PRIMARY_COLOR);
            (buttonsReferences[key][4] as Button)!.Background.Opacity = .7;
        }

        private void SetUnselectedColor(string key)
        {
            (buttonsReferences[key][2] as Button)!.Foreground = new SolidColorBrush(PRIMARY_COLOR);
            (buttonsReferences[key][2] as Button)!.Background = null;

            (buttonsReferences[key][4] as Button)!.Foreground = new SolidColorBrush(PRIMARY_COLOR);
            (buttonsReferences[key][4] as Button)!.Background = null;
        }

        private void UnselectBtn(string key)
        {
            (buttonsReferences[key][0] as TextBlock)!.Foreground = new SolidColorBrush(PRIMARY_COLOR);
            (buttonsReferences[key][1] as PackIcon)!.Foreground = new SolidColorBrush(PRIMARY_COLOR);
            (buttonsReferences[key][5] as PackIcon)!.Foreground = new SolidColorBrush(PRIMARY_COLOR);
            buttonsReferences[key][3] = false;
            SetUnselectedColor(key);
        }

        private void SelectBtn(string key)
        {
            (buttonsReferences[key][0] as TextBlock)!.Foreground = new SolidColorBrush(Colors.White);
            (buttonsReferences[key][1] as PackIcon)!.Foreground = new SolidColorBrush(Colors.White);
            (buttonsReferences[key][5] as PackIcon)!.Foreground = new SolidColorBrush(Colors.White);
            buttonsReferences[key][3] = true;

            SetSelectedColor(key);
        }

        private void CheckSelectionBtn(string key)
        {
            if ((bool)buttonsReferences[key][3])
                UnselectBtn(key);
            else
                SelectBtn(key);

        }

        private void UncheckAllBtns()
        {
            //selectedBtns.ForEach(item => item = true);
            foreach (KeyValuePair<string, List<object>> entry in buttonsReferences)
            {
                buttonsReferences[entry.Key][3] = true;
                UnselectBtn(entry.Key);
            }

            // buttons.ForEach(item => UnselectBtn(buttons.IndexOf(item), item));
        }



        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {

        }

        /* --------------
         * MENU BUTTONS
         * --------------
         */

        private void ReferencesBtn_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllBtns();
            CheckSelectionBtn("References");
            ReferencesMenu.IsSubmenuOpen = true;
        }

        private void MessagesBtn_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllBtns();
            CheckSelectionBtn("Messages");
        }

        private void EmailBtn_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllBtns();
            CheckSelectionBtn("Email");
        }

        private void AccountBtn_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllBtns();
            CheckSelectionBtn("Account");
        }

        private void MenuCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["CloseMenu"] as Storyboard;
            sb.Begin(LeftMenu);
            MenuCloseBtn.Visibility = Visibility.Hidden;
            MenuOpenBtn.Visibility = Visibility.Visible;

            foreach (KeyValuePair<string, List<object>> entry in buttonsReferences)
                (buttonsReferences[entry.Key][4] as Button)!.Visibility = Visibility.Visible;
        }

        private void MenuOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["OpenMenu"] as Storyboard;
            sb.Begin(LeftMenu);
            MenuOpenBtn.Visibility = Visibility.Hidden;
            MenuCloseBtn.Visibility = Visibility.Visible;

            foreach (KeyValuePair<string, List<object>> entry in buttonsReferences)
                (buttonsReferences[entry.Key][4] as Button)!.Visibility = Visibility.Hidden;

        }

        private void MenuReferencesBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        public void ShowSideMenu()
        {
            MenuOpenBtn.Visibility = Visibility.Visible;
            MenuCloseBtn.Visibility = Visibility.Hidden;
            LoginPage.ErrorStackPanel.Visibility = Visibility.Hidden;

            foreach (KeyValuePair<string, List<object>> entry in buttonsReferences)
                (buttonsReferences[entry.Key][4] as Button)!.Visibility = Visibility.Visible;
        }

        public void HideSideMenu()
        {
            Storyboard sb = Resources["CloseMenu"] as Storyboard;
            sb.Begin(LeftMenu);
            MenuCloseBtn.Visibility = Visibility.Hidden;
            MenuOpenBtn.Visibility = Visibility.Hidden;

            LeftMenu.Visibility = Visibility.Hidden;
            LoginPage.ErrorStackPanel.Visibility = Visibility.Hidden;

            foreach (KeyValuePair<string, List<object>> entry in buttonsReferences)
                (buttonsReferences[entry.Key][4] as Button)!.Visibility = Visibility.Hidden;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Uri path = new Uri(Directory.GetCurrentDirectory() + @"\Resources\Images\loginBackground.jpg");
            MainGrid.Background = new ImageBrush(new BitmapImage(path));
            HideSideMenu();
            ChangePage(LoginPage);
        }

        public async Task<string> Login(string login, string password)
        {
            var loginObject = new LoginObject { Login = login, Password = password };
            var item = await loginClient.LoginUserAsync(new LoginRequest { Data = loginObject });
            if (item.Token != "Invalid data")
            {
                client = new UserService.UserServiceClient(GrpcChannel.ForAddress(Properties.Default.Address.ToString()));
                headers.Add("Authorization", $"Bearer {item.Token}");
            }

            return await Task.FromResult(item.Token);
        }

        private void ReferencesMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (sender as MenuItem);
            var a = menuItem.Items;
        }

        private void ReferencesMenu_SubMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (sender as MenuItem);
            var mainMenu = menuItem.Header;

            MenuItem obMenuItem = e.OriginalSource as MenuItem;
            var subMenu = obMenuItem.Header;

            var page = MainFrameK.Content as TablePage;
            var keys = tables.Keys.ToList();

            for (int i = 0; i < tablesList.Count(); i++)
            {
                if (subMenu.ToString() == tablesList[i])
                {
                    switch (i)
                    {
                        case 0:
                            page.ChangeSelectedTable(new CargoTablePage());
                            MessageBox.Show("Changed");
                            break;
                        case 1:
                            page.ChangeSelectedTable(new CargoTypesPage());
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        case 5:
                            break;
                        case 6:
                            break;
                        case 7:
                            break;
                        case 8:
                            break;
                        case 9:
                            break;
                        case 10:
                            break;
                        case 11:
                            break;
                    }
                }
            }

        }

    }

}
