using ClientsApp;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages;
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
using System.Windows.Shapes;

namespace LogisticsClientsApp
{
    /// <summary>
    /// Логика взаимодействия для StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
            MainFrameK.Navigate(new TablePage());

            MenuOpenBtn.Click += Button_Click_1;
            MenuCloseBtn.Click += btnclose_Click;

            //LeftMenu.Visibility = Visibility.Hidden;

            InitElements();

            Locale locale = new Locale("ru");
            locale.SetLocale(this);

            SelectBtn("References");

        }

        private void DarkModeToggle_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void MainFrameK_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            
        }

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

        private Dictionary<string, List<object>> buttonsReferences = new Dictionary<string, List<object>>();

        private List<bool> selectedBtns = new List<bool>();
        private List<TextBlock> textBlocks = new List<TextBlock>();
        private List<PackIcon> packIcons = new List<PackIcon>();
        private List<Button> buttons = new List<Button>();

        private static Color PRIMARY_COLOR = Color.FromArgb(255, 33, 150, 243);

        private void InitElements()
        {

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
            Sanyok.IsSubmenuOpen = true;
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
    }

}
