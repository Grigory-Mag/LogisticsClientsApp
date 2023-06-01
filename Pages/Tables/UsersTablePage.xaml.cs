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

        StartWindow startWindow;

        public UsersTablePage()
        {
            InitializeComponent();
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
            SetData();
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

                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = UsersOriginal.Skip(skipPages).Take(takePages);
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
                UsersOriginal = Users;
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = Users.Skip(skipPages).Take(takePages);
                locale.SetLocale(this);
                PaginationTextBlock.Text = $"{skipPages + 10} из {Users.Count}";
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
        }
    }
}
