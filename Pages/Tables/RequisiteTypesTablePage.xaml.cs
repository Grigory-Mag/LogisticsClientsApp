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
    /// Логика взаимодействия для RequisiteTypesTablePage.xaml
    /// </summary>
    public partial class RequisiteTypesTablePage : Page
    {
        public List<RequisiteTypeObject> RequisitesTypes { get; set; }
        public List<RequisiteTypeObject> RequisitesTypesOriginal { get; set; }
        private Locale locale;
        public byte mode = 0;

        string tableName = "типы организаций";

        public int takePages = 10;
        public int skipPages = 0;

        public static RequisiteTypesTablePage PageInstance;
        static StartWindow startWindow;

        public RequisiteTypesTablePage()
        {
            InitializeComponent();
        }

        public static RequisiteTypesTablePage CreateInstance()
        {
            if (PageInstance == null)
                PageInstance = new RequisiteTypesTablePage();
            return PageInstance;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            string tableName = "типы организаций";
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
                        RequisitesTypes = RequisitesTypesOriginal
                            .Where(x => x.Name.Contains(text))
                            .ToList();
                        if (RequisitesTypes.Count == 0)
                            RequisitesTypes = RequisitesTypesOriginal;
                        break;
                }
            else
                RequisitesTypes = RequisitesTypesOriginal;
            skipPages = 0;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = RequisitesTypes.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {RequisitesTypes.Count}";
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
                var skippedCargo = RequisitesTypes.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {RequisitesTypes.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < RequisitesTypes.Count)
            {
                skipPages += 10;
                var skippedCargo = RequisitesTypes.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {RequisitesTypes.Count}";
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    var item = dataGrid.SelectedItem as RequisiteTypeObject;
                    var resultLocal = await startWindow.client.DeleteRequisiteTypeAsync(new GetOrDeleteRequisiteTypeRequest { Id = item.Id }, startWindow.headers);
                    RequisitesTypesOriginal.Remove(item);
                    RequisitesTypes = RequisitesTypesOriginal;

                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = RequisitesTypesOriginal.Skip(skipPages).Take(takePages);
                    PaginationTextBlock.Text = $"{skipPages + 10} из {RequisitesTypesOriginal.Count}";
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
                var item = await startWindow.client.GetListRequisiteTypesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                RequisitesTypes = new List<RequisiteTypeObject>();
                RequisitesTypes.AddRange(item.RequisiteType.ToList());
                RequisitesTypes = RequisitesTypes.OrderBy(x => x.Id).ToList();

                RequisitesTypesOriginal = RequisitesTypes;

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = RequisitesTypes.Skip(skipPages).Take(takePages);
                locale.SetLocale(this);
                PaginationTextBlock.Text = $"{skipPages + 10} из {RequisitesTypes.Count}";
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
            RequisitesTypes.Clear();
            RequisitesTypesOriginal.Clear();
            dataGrid.ItemsSource = null;
            BindingOperations.ClearAllBindings(dataGrid);
        }
    }
}