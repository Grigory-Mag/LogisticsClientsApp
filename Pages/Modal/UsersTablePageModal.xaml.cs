using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для UsersTablePageModal.xaml
    /// </summary>
    public partial class UsersTablePageModal : UserControl
    {
        public LoginObject data = new LoginObject();
        public ListUserRoles roles;
        private Locale locale;
        public byte mode = 0;

        StartWindow startWindow;

        public UsersTablePageModal()
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
        public void UpdateDisplayedData(LoginObject data)
        {
            this.data = data;
            LoginTextBox.Text = data.Login.ToString();
            NameTextBox.Text = data.Name.ToString();
            SurnameTextBox.Text = data.Surname.ToString();
            PatrTextBox.Text = data.Patronymic.ToString();
            PasswordBox.Password = data.Password.ToString();
            if (startWindow != null)
                SetLinkedData();
        }

        public async void SetLinkedData()
        {
            roles = await startWindow.client.GetListUserRolesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
            RoleComboBox.ItemsSource = roles.UserRole;
            RoleComboBox.SelectedItem = data.UserRole == null ? null : roles.UserRole.First(x => x.Id == data.UserRole.Id);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }

        private async void UpdateData()
        {
            try
            {
                var reqResult = new LoginObject();
                if (mode == 0)
                    reqResult = await startWindow.client.UpdateUserAsync(new LoginRequest { Data = data });
                if (mode == 1)
                    reqResult = await startWindow.client.CreateUserAsync(new LoginRequest { Data = data });
                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as UsersTablePage;
                if (mode == 0)
                {
                    var index = page.Users.FindIndex(t => t.Id == reqResult.Id);
                    page.Users[index] = reqResult;
                }
                if (mode == 1)
                    page.Users.Add(reqResult);

                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.Users;
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
                if (LoginTextBox.Text != data.Login.ToString())
                    changedDataNotify.Append($"Логин: {data.Login} -> {LoginTextBox.Text}");
                if (NameTextBox.Text != data.Name.ToString())
                    changedDataNotify.Append($"Название: {data.Name} -> {NameTextBox.Text}");
                if (SurnameTextBox.Text != data.Surname.ToString())
                    changedDataNotify.Append($"Название: {data.Surname} -> {SurnameTextBox.Text}");
                if (PatrTextBox.Text != data.Patronymic.ToString())
                    changedDataNotify.Append($"Название: {data.Patronymic} -> {PatrTextBox.Text}");
                if (PasswordBox.Password != data.Password.ToString())
                    changedDataNotify.Append($"Название: {data.Password} -> {PasswordBox.Password}");
                if ((RoleComboBox.SelectedItem as UserRoleObject).Id != data.UserRole.Id)
                    changedDataNotify.Append($"Название: {data.UserRole.Name} -> {(RoleComboBox.SelectedItem as UserRoleObject).Name}");
            }

            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                data.Name = NameTextBox.Text;
                data.Surname = SurnameTextBox.Text;
                data.Patronymic = PatrTextBox.Text;
                data.Login = LoginTextBox.Text;
                data.UserRole = RoleComboBox.SelectedItem as UserRoleObject;

                if (PasswordBox.Password != data.Password.ToString())
                {
                    SHA512 crypt = SHA512.Create();
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] bytes = encoding.GetBytes(PasswordBox.Password);
                    byte[] hash = crypt.ComputeHash(bytes);
                    var output = Convert.ToHexString(hash);
                    data.Password = output;
                }
                else
                    data.Password = "Don't set";
                    
                UpdateData();
            }
        }
    }
}
