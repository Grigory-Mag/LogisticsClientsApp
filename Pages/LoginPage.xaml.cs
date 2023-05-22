using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Localizations.Data;
using LogisticsClientsApp.Pages.Tables;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LogisticsClientsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        StartWindow startWindow;

        public LoginPage(StartWindow startWindow)
        {
            InitializeComponent();
            startWindow.MainFrameK.NavigationService.RemoveBackEntry();
            if (startWindow != null) this.startWindow = startWindow;
            startWindow.LeftMenu.Visibility = Visibility.Collapsed;
            Locale locale = new Locale("ru");
            locale.SetLocale(this);

            ErrorStackPanel.Visibility = Visibility.Collapsed;
        }

        private async void LoginHandler()
        {
            try
            {

                var data = await startWindow.Login(LoginTextBox.Text.ToString(), PasswordTextBox.Password.ToString());
                if (data.Token == "Invalid data")
                {
                    ErrorStackPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    startWindow.ChangePage(new TablePage());
                    startWindow.ShowSideMenu();
                    LoginInProcess.Visibility = Visibility.Hidden;
                    startWindow.NameTextBlock.Text = data.User.Name;
                    startWindow.SurnameTextBlock.Text = data.User.Surname;
                    startWindow.RoleTextBlock.Text = data.User.UserRole.Name;
                }
                LoginInProcess.Visibility = Visibility.Hidden;


            }
            catch (RpcException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                LoginInProcess.Visibility = Visibility.Hidden;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginInProcess.Visibility  = Visibility.Visible;
            //Thread.Sleep(5000);
            LoginHandler();
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            RotateTransform transform = new RotateTransform();

            var a = CanvasItem.RenderTransform;
            transform.Angle += 20;
        }

        private void UnSetErrorWhileLogin()
        {
            ErrorStackPanel.Visibility = Visibility.Collapsed;
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UnSetErrorWhileLogin();
        }

        private void PasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            UnSetErrorWhileLogin();
        }
    }
}
