﻿using ApiService;
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
    /// Логика взаимодействия для RouteActionsTablePage.xaml
    /// </summary>
    public partial class RouteActionsTablePage : Page
    {
        public List<RouteActionsObject> RouteActions { get; set; }
        public List<RouteActionsObject> RouteActionsOriginal { get; set; }

        public int takePages = 10;
        public int skipPages = 0;
        private Locale locale;

        public static RouteActionsTablePage PageInstance;
        static StartWindow startWindow;
        public RouteActionsTablePage()
        {
            InitializeComponent();
        }

        public static RouteActionsTablePage CreateInstance()
        {
            if (PageInstance == null)
                PageInstance = new RouteActionsTablePage();
            return PageInstance;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            string tableName = "категории";
            var tablePage = startWindow.MainFrameK.Content as TablePage;
            tablePage.TextBlockTableName.Text = tableName;
            SetData();
            ResizeDataGrid();
            startWindow.SizeChanged += (o, e) =>
            {
                ResizeDataGrid();
            };
        }

        public void FastSearch(string text, string? param)
        {
            if (text != "")
                switch (param)
                {
                    case "Название":
                        text = text.Trim();
                        RouteActions = RouteActionsOriginal
                            .Where(x => x.Action.Contains(text))
                            .ToList();
                        if (RouteActions.Count == 0)
                            RouteActions = RouteActionsOriginal;
                        break;
                }
            else
                RouteActions = RouteActionsOriginal;
            skipPages = 0;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = RouteActions.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {RouteActions.Count}";
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
                var skippedCargo = RouteActions.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {RouteActions.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < RouteActions.Count)
            {
                skipPages += 10;
                var skippedCargo = RouteActions.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {RouteActions.Count}";
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    var item = dataGrid.SelectedItem as RouteActionsObject;
                    var resultLocal = await startWindow.client.DeleteRouteActionAsync(new GetOrDeleteRouteActionsRequest { Id = item.Id }, startWindow.headers);
                    RouteActionsOriginal.Remove(item);
                    RouteActions = RouteActionsOriginal;

                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = RouteActionsOriginal.Skip(skipPages).Take(takePages);
                    PaginationTextBlock.Text = $"{skipPages + 10} из {RouteActionsOriginal.Count}";
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
                var item = await startWindow.client.GetListRouteActionsAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                RouteActions = new List<RouteActionsObject>();
                RouteActions.AddRange(item.RouteActionsObject.ToList());
                RouteActions = RouteActions.OrderBy(x => x.Id).ToList();

                RouteActionsOriginal = RouteActions;

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = RouteActions.Skip(skipPages).Take(takePages);
                locale.SetLocale(this);
                PaginationTextBlock.Text = $"{skipPages + 10} из {RouteActions.Count}";
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

        public void Dispose()
        {
            startWindow.SizeChanged -= (o, e) =>
            {
                ResizeDataGrid();
            };
            RouteActions.Clear();
            RouteActionsOriginal.Clear();
            dataGrid.ItemsSource = null;
            BindingOperations.ClearAllBindings(this);
        }
    }
}