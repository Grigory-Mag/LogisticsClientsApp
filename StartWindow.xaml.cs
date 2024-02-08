using ApiService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages;
using LogisticsClientsApp.Pages.Tables;
using MaterialDesignThemes.Wpf;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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
        public double windowSize = 0;
        public int role = 0;
        public bool IsConnected = true;

        public string userName { get; set; }
        public string userSurname { get; set; }
        public string userPatronymic { get; set; }
        public string userRole { get; set; }

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

            userName = "Name";
            userSurname = "Surname";
            userPatronymic = "Patronymic";
            userRole = "Role";


            LoginPage = new LoginPage(this);
            ChangePage(LoginPage);
            //DEBUG ONLY:
            //Properties.Default.Address = "http://185.248.101.68:8008";
            //Properties.Default.Save();

            try
            {
                Uri path = new Uri(Directory.GetCurrentDirectory() + @"\Resources\Images\loginBackground.jpg");
                MainGrid.Background = new ImageBrush(new BitmapImage(path));
            }
            catch (Exception ex)
            {

            }

            MenuOpenBtn.Click += Button_Click_1;
            MenuCloseBtn.Click += btnclose_Click;

            Locale locale = new Locale("ru");
            locale.SetLocale(this);

            NameTextBlock.Text = userName;
            SurnameTextBlock.Text = userSurname;
            RoleTextBlock.Text = userRole;

            ITheme theme = new PaletteHelper().GetTheme();
            PaletteHelper palette = new PaletteHelper(); ;

            var SelectedTheme = Properties.Default.SelectedTheme;
            switch (SelectedTheme)
            {
                case "Light":
                    theme.SetBaseTheme(Theme.Light);
                    palette.SetTheme(theme);
                    break;
                case "Dark":
                    theme.SetBaseTheme(Theme.Dark);
                    palette.SetTheme(theme);
                    break;
                default:
                    theme.SetBaseTheme(Theme.Light);
                    palette.SetTheme(theme);
                    break;
            }
            IconImage.Source = LoadLogo();

            InitElements();
            LoadImages();
            SelectBtn("References");
        }

        public BitmapImage LoadLogo()
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(@"pack://application:,,,/Resources/Images/truck.ico");
            logo.EndInit();
            return logo;
        }

        private void LoadImages()
        {
            var fileBackgroundPath = Properties.Default.BackgroundImage;
            var fileForegroundPath = Properties.Default.ForegroundImage;
            Properties.Default.Save();
            Uri? uri;
            var created = Uri.TryCreate(fileBackgroundPath, UriKind.RelativeOrAbsolute, out uri);
            if (created)
                try
                {
                    BitmapImage background = new BitmapImage();
                    background.BeginInit();
                    background.UriSource = new Uri(fileBackgroundPath);
                    background.EndInit();
                    ProfileBackgroundImage.ImageSource = background;
                }
                catch (Exception ex)
                {
                    try
                    {
                        var path = Directory.GetCurrentDirectory() + @"\Resources\Images\background_image.png";
                        BitmapImage background = new BitmapImage();
                        background.BeginInit();
                        background.UriSource = new Uri(path);
                        background.EndInit();
                        ProfileBackgroundImage.ImageSource = background;
                    }
                    catch (Exception ex2)
                    {

                    }

                    //MessageBox.Show("Изображение не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            created = Uri.TryCreate(fileForegroundPath, UriKind.RelativeOrAbsolute, out uri);
            if (created)
                try
                {
                    BitmapImage background = new BitmapImage();
                    background.BeginInit();
                    background.UriSource = new Uri(fileForegroundPath);
                    background.EndInit();
                    UserProfileImage.ImageSource = background;
                }
                catch (Exception ex)
                {
                    try
                    {
                        var path = Directory.GetCurrentDirectory() + @"\Resources\Images\employee.png";
                        BitmapImage background = new BitmapImage();
                        background.BeginInit();
                        background.UriSource = new Uri(path);
                        background.EndInit();
                        UserProfileImage.ImageSource = background;
                    }
                    catch (Exception ex2)
                    {

                    }

                    //MessageBox.Show("Изображение не найдено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }

        public void ChangePage(Page page)
        {
            if (MainFrameK.Content != null)
            {
                switch (MainFrameK.Content)
                {
                    case var cls when cls == typeof(CargoTablePage):
                        (MainFrameK.Content as CargoTablePage)!.Dispose();
                        break;
                    case var cls when cls == typeof(CargoTypesPage):
                        (MainFrameK.Content as CargoTypesPage)!.Dispose();
                        break;
                    case var cls when cls == typeof(DriverLicenceTablePage):
                        (MainFrameK.Content as DriverLicenceTablePage)!.Dispose();
                        break;
                    case var cls when cls == typeof(DriversTablePage):
                        (MainFrameK.Content as DriversTablePage)!.Dispose();
                        break;
                    case var cls when cls == typeof(RequestsTablePage):
                        (MainFrameK.Content as RequestsTablePage)!.Dispose();
                        break;
                    case var cls when cls == typeof(RolesTablePage):
                        (MainFrameK.Content as RolesTablePage)!.Dispose();
                        break;
                    case var cls when cls == typeof(VehiclesTypesTablePage):
                        (MainFrameK.Content as VehiclesTypesTablePage)!.Dispose();
                        break;
                    case var cls when cls == typeof(VehiclesTablePage):
                        (MainFrameK.Content as VehiclesTablePage)!.Dispose();
                        break;

                    case var cls when cls == typeof(RouteActionsTablePage):
                        (MainFrameK.Content as RouteActionsTablePage)!.Dispose();
                        break;
                }
            }
            //MainFrameK.RemoveBackEntry();
            MainFrameK.Content = null;



            //if (this.MainFrameK.CanGoBack)
            //{
            //    var entry = this.MainFrameK.RemoveBackEntry();
            //    while (entry != null)
            //    {
            //        entry = this.MainFrameK.RemoveBackEntry();
            //    }

            //    this.MainFrameK.Navigate(new PageFunction<string>() { RemoveFromJournal = true });
            //    //MessageBox.Show("Cleared");
            //}


            MainFrameK.Navigate(page);
            page = null;

            if (MainFrameK.CanGoBack && MainFrameK.CanGoForward)
            {
                var entry = MainFrameK.RemoveBackEntry();
                while (entry != null)
                {
                    entry = MainFrameK.RemoveBackEntry();
                }
            }

        }

        private void InstantiateNewControl()
        {
            Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(InstantiateNewControl));
        }

        public void ClearFrameHistory()
        {
            MainFrameK.NavigationService.RemoveBackEntry();
            var data = MainFrameK.NavigationService;
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

            var page = MainFrameK.Content as TablePage;
            if (page == null)
            {
                page = TablePage.CreateInstance(RequestsTablePage.CreateInstance());
                ChangePage(page);
            }
            else
                ChangePage(TablePage.CreateInstance(RequestsTablePage.CreateInstance()));


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
            MenuCloseBtn.Visibility = Visibility.Collapsed;
            MenuOpenBtn.Visibility = Visibility.Collapsed;

            LeftMenu.Visibility = Visibility.Collapsed;
            LoginPage.ErrorStackPanel.Visibility = Visibility.Hidden;

            foreach (KeyValuePair<string, List<object>> entry in buttonsReferences)
                (buttonsReferences[entry.Key][4] as Button)!.Visibility = Visibility.Collapsed;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Uri path = new Uri(Directory.GetCurrentDirectory() + @"\Resources\Images\loginBackground.jpg");
            MainGrid.Background = new ImageBrush(new BitmapImage(path));
            HideSideMenu();
            ChangePage(LoginPage);
        }

        public async Task<LoginReply> Login(string login, string password)
        {
            SHA512 crypt = SHA512.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(password);
            byte[] hash = crypt.ComputeHash(bytes);
            var output = Convert.ToHexString(hash);

            var loginObject = new LoginObject { Login = login, Password = output };
            var item = await loginClient.LoginUserAsync(new LoginRequest { Data = loginObject });
            if (item.Token != "Invalid data")
            {
                headers.Clear();
                headers.Add("Authorization", $"Bearer {item.Token}");
                role = item.User.UserRole.Id;
                var options = new CallOptions().WithHeaders(headers);
                client = new UserService.UserServiceClient(GrpcChannel.ForAddress(Properties.Default.Address.ToString()));
                Locale locale = new Locale("ru");
                locale.SetLocale(this);
                ReferencesMenu.Items.Clear();
                foreach (var value in tables)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Click += ReferencesMenu_SubMenu_Click;
                    if (value.Value != null)
                        menuItem.ItemsSource = value.Value;
                    menuItem.Header = value.Key;
                    ReferencesMenu.Items.Add(menuItem);
                }
            }

            return await Task.FromResult(item);
        }

        private void ReferencesMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (sender as MenuItem);
            var a = menuItem.Items;
        }

        private async Task LoadAsyncPage()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChangePage(TablePage.CreateInstance());
                });
            });
        }

        private async void ReferencesMenu_SubMenu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (sender as MenuItem);
            var mainMenu = menuItem.Header;

            MenuItem obMenuItem = e.OriginalSource as MenuItem;
            var subMenu = obMenuItem.Header;

            var page = MainFrameK.Content as TablePage;
            if (page == null)
            {
                page = TablePage.CreateInstance();
                await LoadAsyncPage();
                page = MainFrameK.Content as TablePage;
            }

            var keys = tables.Keys.ToList();

            for (int i = 0; i < tablesList.Count(); i++)
            {
                if (subMenu.ToString() == tablesList[i])
                {
                    switch (i)
                    {
                        case 0:
                            page!.ChangeSelectedTable(CargoTablePage.CreateInstance());
                            break;
                        case 1:
                            page!.ChangeSelectedTable(CargoTypesPage.CreateInstance());
                            break;
                        case 2:
                            page!.ChangeSelectedTable(DriversTablePage.CreateInstance());
                            break;
                        case 3:
                            page!.ChangeSelectedTable(DriverLicenceTablePage.CreateInstance());
                            break;
                        case 4:
                            page!.ChangeSelectedTable(RequisitesTablePage.CreateInstance());
                            break;
                        case 5:
                            page!.ChangeSelectedTable(RolesTablePage.CreateInstance());
                            break;
                        case 6:
                            page!.ChangeSelectedTable(RequisiteTypesTablePage.CreateInstance());
                            break;
                        case 7:
                            page!.ChangeSelectedTable(VehiclesTablePage.CreateInstance());
                            break;
                        case 8:
                            page!.ChangeSelectedTable(VehiclesTypesTablePage.CreateInstance());
                            break;
                        case 9:
                            page!.ChangeSelectedTable(RequestsTablePage.CreateInstance());
                            break;
                        case 10:
                            page!.ChangeSelectedTable(RouteActionsTablePage.CreateInstance());
                            break;
                        case 11:
                            page!.ChangeSelectedTable(UsersTablePage.CreateInstance());
                            break;
                    }
                }
            }

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            windowSize = Height / 2 - 40;
        }

        private void CloseAppButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ResizeAppWindowButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                // Получаем высоту экрана и рабочей области
                double screenHeight = SystemParameters.PrimaryScreenHeight;
                double workAreaHeight = SystemParameters.WorkArea.Height;

                // Вычисляем высоту taskbar'а
                double taskbarHeight = screenHeight - workAreaHeight;

                // Устанавливаем максимальную высоту окна с учетом taskbar'а
                MaxHeight = screenHeight - taskbarHeight + 15;

                // Устанавливаем состояние окна на "Максимизировано"                
                WindowState = WindowState.Maximized;

                ResizeIcon.Kind = PackIconKind.Resize;
            }
            else
            {
                WindowState = WindowState.Normal;
                ResizeIcon.Kind = PackIconKind.Maximize;
            }

        }

        private void MinimizeAppWindowButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OperationsWithWindowPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
                WindowState = WindowState.Normal;
                ResizeIcon.Kind = PackIconKind.Maximize;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;

            // Получаем высоту экрана и рабочей области
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double workAreaHeight = SystemParameters.WorkArea.Height;

            // Вычисляем высоту taskbar'а
            double taskbarHeight = screenHeight - workAreaHeight;

            // Устанавливаем максимальную высоту окна с учетом taskbar'а
            MaxHeight = screenHeight - taskbarHeight + 15;

            // Устанавливаем состояние окна на "Максимизировано"                
            WindowState = WindowState.Maximized;
        }
    }

}
