using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static LogisticsClientsApp.Pages.Tables.RequestsTablePage;

namespace LogisticsClientsApp.Pages.Tables
{
    /// <summary>
    /// Логика взаимодействия для RequisitesTablePage.xaml
    /// </summary>
    public partial class RequisitesTablePage : Page
    {
        public List<RequisitesObject> Requisites { get; set; }
        public List<RequisitesObject> RequisitesOriginal { get; set; }

        public List<RolesObject> Roles { get; set; }
        public List<RequisiteTypeObject> RequisiteTypes { get; set; }

        public List<string> SearchFields = new List<string>() { "Тип", "Роль", "Владелец", "Название", "Юр. адрес", "ИНН" };
        public List<object> SearchItemsList = new List<object>();
        public TablePage tablePage;
        private Locale locale;

        public static ResourceDictionary textBoxResource = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.TextBox.xaml", UriKind.RelativeOrAbsolute)
        };
        public static ResourceDictionary comboBoxResource = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.ComboBox.xaml", UriKind.RelativeOrAbsolute)
        };

        public int takePages = 10;
        public int skipPages = 0;

        public static RequisitesTablePage PageInstance;
        static StartWindow startWindow;
        public RequisitesTablePage()
        {
            InitializeComponent();
        }

        public static RequisitesTablePage CreateInstance()
        {
            if (PageInstance == null)
                PageInstance = new RequisitesTablePage();
            return PageInstance;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            string tableName = "организации";
            tablePage = startWindow.MainFrameK.Content as TablePage;

            Roles = new List<RolesObject>();
            RequisiteTypes = new List<RequisiteTypeObject>();

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
                        Requisites = RequisitesOriginal
                            .Where(x => x.Name.Contains(text))
                            .ToList();
                        break;
                    case "ИНН":
                        text = text.Trim();
                        Requisites = RequisitesOriginal
                            .Where(x => x.Inn.ToString().Contains(text))
                            .ToList();
                        break;
                }
            else
                Requisites = RequisitesOriginal;

            if (Requisites.Count == 0)
                Requisites = RequisitesOriginal;

            skipPages = 0;
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = Requisites.Skip(skipPages).Take(takePages);
            PaginationTextBlock.Text = $"{skipPages + 10} из {Requisites.Count}";
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
                var skippedCargo = Requisites.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {Requisites.Count}";
            }
        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {
            if (skipPages + 10 < Requisites.Count)
            {
                skipPages += 10;
                var skippedCargo = Requisites.Skip(skipPages).Take(takePages).ToList();
                dataGrid.ItemsSource = skippedCargo;
                PaginationTextBlock.Text = $"{skipPages + 10} из {Requisites.Count}";
            }
        }

        private async void CreateAdvancedSearchFields()
        {
            tablePage.AdvancedSearch.Children.Clear();
            SearchItemsList.Clear();
            // typeItemsSource.Add(new CargoTypesObject { Id = -1, Name = "Все типы" });
            foreach (var item in SearchFields)
            {
                var textBox = new TextBox();
                var comboBox = new ComboBox();

                HintAssist.SetHint(textBox, item);
                textBox.Margin = new Thickness(0, 0, 20, 0);
                textBox.Style = textBoxResource["MaterialDesignOutlinedTextBox"] as Style;
                textBox.MaxWidth = 180;
                textBox.MinWidth = 150;
                textBox.FontSize = 14;
                textBox.Height = 55;
                textBox.TextChanged += SearchTextBoxChanged;

                HintAssist.SetHint(comboBox, item);
                comboBox.Margin = new Thickness(0, 0, 20, 0);
                comboBox.Style = comboBoxResource["MaterialDesignOutlinedComboBox"] as Style;
                comboBox.MaxWidth = 180;
                comboBox.MinWidth = 150;
                comboBox.FontSize = 14;
                comboBox.Height = 55;
                comboBox.IsEditable = true;
                comboBox.VerticalAlignment = VerticalAlignment.Bottom;
                comboBox.SelectionChanged += SearchComboBox_SelectionChanged;


                switch (item)
                {
                    case "Тип":
                        comboBox.DisplayMemberPath = @"Name";
                        comboBox.ItemsSource = RequisiteTypes;
                        SearchItemsList.Add(comboBox);
                        tablePage.AdvancedSearch.Children.Add(comboBox);
                        break;
                    case "Роль":
                        comboBox.DisplayMemberPath = @"Name";
                        comboBox.ItemsSource = Roles;
                        SearchItemsList.Add(comboBox);
                        tablePage.AdvancedSearch.Children.Add(comboBox);
                        break;
                    default:
                        tablePage.AdvancedSearch.Children.Add(textBox);
                        SearchItemsList.Add(textBox);
                        break;
                }
            }
            await SyncSearch();
        }

        /// <summary>
        /// Async Search needed due to probably high amount of data to filter
        /// </summary>
        /// <returns>filters data in List of custom object and synchronize search parameters.
        /// Requiring list of widgets</returns>
        private async Task SyncSearch()
        {
            try
            {
                await Task.Run(() =>
                {
                    var emptyFields = 0;
                    List<string> values = new List<string>();

                    if (SearchItemsList.Count > 0)
                    {
                        var itemRole = new RolesObject();
                        var itemType = new RequisiteTypeObject();
                        Dispatcher.Invoke(() =>
                        {
                            itemType = (SearchItemsList[0] as ComboBox)!.SelectedItem as RequisiteTypeObject;
                            itemRole = (SearchItemsList[1] as ComboBox)!.SelectedItem as RolesObject;                            
                        });

                        Dispatcher.Invoke(() =>
                        {

                            foreach (var value in SearchItemsList)
                            {
                                switch (value)
                                {
                                    case TextBox:
                                        values.Add((value as TextBox).Text);
                                        break;
                                }
                            }
                        });

                        Requisites = RequisitesOriginal
                        .Where((x) =>
                        {
                            return itemType == null ? true : x.Type.Id == itemType.Id;
                        })
                        .Where((x) =>
                        {
                            return itemRole == null ? true : x.Role.Id == itemRole.Id;
                        })
                        .Where((x) =>
                        {
                            return values[0] == "" ? true : x.Ceo.Contains(values[0]);
                        })
                        .Where((x) =>
                        {
                            return values[1] == "" ? true : x.Name.Contains(values[1]);
                        })
                        .Where((x) =>
                        {
                            return values[2] == "" ? true : x.LegalAddress.Contains(values[2]);
                        })
                        .Where((x) =>
                        {
                            return values[3] == "" ? true : x.Inn.ToString().Contains(values[3]);
                        })
                        .ToList();


                        if (Requisites.Count == 0)
                            Requisites = RequisitesOriginal;
                    }
                });

                dataGrid.ItemsSource = null;
                if (Requisites.Count == 0)
                    Requisites = RequisitesOriginal;
                dataGrid.ItemsSource = Requisites.Skip(skipPages).Take(takePages);
                PaginationTextBlock.Text = $"{skipPages + 10} из {Requisites.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private async void SearchTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            await SyncSearch();
        }

        private async void SearchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await SyncSearch();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show($"Вы действительно хотите удалить запись?", "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.OK)
            {
                try
                {
                    var item = dataGrid.SelectedItem as RequisitesObject;
                    var resultLocal = await startWindow.client.DeleteRequisiteAsync(new GetOrDeleteRequisitesRequest { Id = item.Id }, startWindow.headers);
                    RequisitesOriginal.Remove(item);
                    Requisites = RequisitesOriginal;

                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = RequisitesOriginal.Skip(skipPages).Take(takePages);
                    PaginationTextBlock.Text = $"{skipPages + 10} из {RequisitesOriginal.Count}";
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
                var item = await startWindow.client.GetListRequisitesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                Requisites = new List<RequisitesObject>();
                Requisites.AddRange(item.Requisites.ToList());
                Requisites = Requisites.OrderBy(x => x.Id).ToList();
                RequisitesOriginal = Requisites;
                Requisites.ForEach(x =>
                {
                    Roles.Add(x.Role);
                    RequisiteTypes.Add(x.Type);
                }
                );

                Roles = Roles.Distinct().ToList();
                RequisiteTypes = RequisiteTypes.Distinct().ToList();

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = Requisites.Skip(skipPages).Take(takePages);
                PaginationTextBlock.Text = $"{skipPages + 10} из {Requisites.Count}";
                locale.SetLocale(this);
                CreateAdvancedSearchFields();
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
            Requisites.Clear();
            RequisitesOriginal.Clear();
            RequisiteTypes.Clear();
            dataGrid.ItemsSource = null;
            BindingOperations.ClearAllBindings(dataGrid);
        }
    }
}
