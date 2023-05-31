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
    /// Логика взаимодействия для VehiclesTypesTablePage.xaml
    /// </summary>
    public partial class VehiclesTypesTablePage : Page
    {
        public List<VehiclesTypesObject> Types { get; set; }
        public List<VehiclesTypesObject> TypesOriginal { get; set; }
        private Locale locale;

        public int takePages = 10;
        public int skipPages = 0;

        StartWindow startWindow;
        public VehiclesTypesTablePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            string tableName = "типы транспорта";
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
                        Types = TypesOriginal
                            .Where(x => x.Name.Contains(text))
                            .ToList();
                        if (Types.Count == 0)
                            Types = TypesOriginal;
                        break;
                }
            else
                Types = TypesOriginal;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Types.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {Types.Count}";
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
                var skippedCargo = Types.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {Types.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < Types.Count)
            {
                skipPages += 10;
                var skippedCargo = Types.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {Types.Count}";
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    var item = dataGrid.SelectedItem as VehiclesTypesObject;
                    startWindow.client.DeleteVehiclesTypeAsync(new GetOrDeleteVehiclesTypesRequest { Id = item.Id }, startWindow.headers);
                    TypesOriginal.Remove(item);

                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = TypesOriginal.Skip(skipPages).Take(takePages);
                }
                catch (RpcException ex)
                {
                    if (ex.StatusCode == StatusCode.Unauthenticated)
                        MessageBox.Show("Ваше время сессии истекло. Перезайдите в аккаунт", "Сессия", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show($"Возникла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void SetData()
        {
            try
            {
                var item = await startWindow.client.GetListVehiclesTypesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                Types = new List<VehiclesTypesObject>();
                Types.AddRange(item.VehiclesTypes.ToList());
                TypesOriginal = Types;

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = Types.Skip(skipPages).Take(takePages);
                locale.SetLocale(this);
                PaginationTextBlock.Text = $"{skipPages + 10} из {Types.Count}";
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

        public void Dispose()
        {
            startWindow.SizeChanged -= (o, e) =>
            {
                ResizeDataGrid();
            };
            Types.Clear();
            TypesOriginal.Clear();
            dataGrid.ItemsSource = null;
            BindingOperations.ClearAllBindings(dataGrid);
        }
    }
}
