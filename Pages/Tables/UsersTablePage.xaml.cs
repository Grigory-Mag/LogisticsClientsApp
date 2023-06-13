using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticsClientsApp.Pages.Tables
{
    /// <summary>
    /// Логика взаимодействия для UsersTablePage.xaml
    /// </summary>
    public partial class UsersTablePage : Page
    {
        public List<LoginObject> Users { get; set; }
        public List<LoginObject> UsersOriginal { get; set; }
        private Locale locale;
        public int takePages = 10;
        public int skipPages = 0;

        public static UsersTablePage PageInstance;
        static StartWindow startWindow;

        public UsersTablePage()
        {
            InitializeComponent();
        }

        public static UsersTablePage CreateInstance()
        {
            if (PageInstance == null)            
                PageInstance = new UsersTablePage();
            
            return PageInstance;
        }

        public void FastSearch(string text, string? param)
        {
            if (text != "")
                switch (param)
                {
                    case "Логин":
                        text = text.Trim();
                        Users = UsersOriginal
                            .Where(x => x.Login.Contains(text))
                            .ToList();
                        if (Users.Count == 0)
                            Users = UsersOriginal;
                        break;
                }
            else
                Users = UsersOriginal;
            skipPages = 0;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Users.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {Users.Count}";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            string tableName = "пользователи";
            var tablePage = startWindow.MainFrameK.Content as TablePage;
            tablePage.TextBlockTableName.Text = tableName;
            ResizeDataGrid();
            startWindow.SizeChanged += (o, e) =>
            {
                ResizeDataGrid();
            };
            SetData();
        }

        public void ResizeDataGrid()
        {
            dataGrid.MaxHeight = startWindow.Height / 2 - 40; ;
        }

        private void PrevTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages - 10 >= 0)
            {
                skipPages -= 10;
                var skippedUsers = Users.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedUsers;
                PaginationTextBlock.Text = $"{skipPages + 10} из {Users.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < Users.Count)
            {
                skipPages += 10;
                var skippedUsers = Users.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedUsers;
                PaginationTextBlock.Text = $"{skipPages + 10} из {Users.Count}";
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    var item = dataGrid.SelectedItem as LoginObject;
                    var resultLocal = await startWindow.client.DeleteUserAsync(new LoginRequest { Data = item }, startWindow.headers);
                    UsersOriginal.Remove(item);
                    Users = UsersOriginal;

                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = UsersOriginal.Skip(skipPages).Take(takePages);
                    PaginationTextBlock.Text = $"{skipPages + 10} из {UsersOriginal.Count}";
                }
                catch (RpcException ex)
                {
                    if (ex.StatusCode == StatusCode.Unauthenticated)
                        MessageBox.Show("Ваше время сессии истекло. Перезайдите в аккаунт", "Сессия", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show($"Возникла ошибка: {ex.StatusCode}. Проверьте, что данная запись нигде более не используется", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SetData()
        {
            try
            {
                var item = await startWindow.client.GetListUsersAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                Users = new List<LoginObject>();
                Users.AddRange(item.Logins.ToList());
                Users = Users.OrderBy(x => x.Id).ToList();
                UsersOriginal = Users;
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = Users.Skip(skipPages).Take(takePages);
                locale.SetLocale(this);
                PaginationTextBlock.Text = $"{skipPages + 10} из {Users.Count}";
                startWindow.IsConnected = true;
            }
            catch (RpcException ex)
            {
                switch (ex.StatusCode)
                {
                    case StatusCode.Unavailable:
                        startWindow.IsConnected = false;
                        MessageBox.Show($"Возникли проблемы с интернет-соединением, обратитесь к администратору: {ex.StatusCode}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case StatusCode.Unauthenticated:
                        break;
                }
#warning TODO
            }
        }

        private void OpenRowButton_Click(object sender, RoutedEventArgs e)
        {
            TablePage tablePage = (TablePage)startWindow.MainFrameK.Content;
            tablePage.ShowModalPage(0);
        }
    }
}
