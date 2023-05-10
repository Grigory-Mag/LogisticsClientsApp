﻿using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages.Tables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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

namespace LogisticsClientsApp.Pages.Modal
{
    /// <summary>
    /// Логика взаимодействия для RequestsTablePageModal.xaml
    /// </summary>
    public partial class RequestsTablePageModal : UserControl
    {
        public RequestsObject data = new RequestsObject();
        public ListRoles roles;
        public ListRequisiteTypes types;
        private Locale locale;
        public byte mode = 0;
        public List<CustomTrash> dataMobile { get; set; }
        StartWindow startWindow;

        public class CustomTrash
        {
            public int Number { get; set; }
            public string Address { get; set; }
            public string Action { get; set; }
            public DateTime SelectedDate { get; set; }

            public CustomTrash() 
            {
                Number = 0;
                Action = "Сдохнуть";
                SelectedDate = DateTime.Now;
            }

            public CustomTrash(int number, string address, DateTime selectedDate, string action)
            {
                Number = number;
                Address = address;
                SelectedDate = selectedDate;
                Action = action;
            }
        }

        public RequestsTablePageModal()
        {
            InitializeComponent();
            dataMobile = new List<CustomTrash>()
            {
                new CustomTrash(1, "0_abobus", DateTime.Now, "Сдохнуть"),
                new CustomTrash(2, "1_abobus", DateTime.Now.AddDays(-1), "Умереть"),
                new CustomTrash(3, "2_abobus", DateTime.Now.AddDays(-2), "Сдохнуть"),
                new CustomTrash(4, "3_abobus", DateTime.Now.AddDays(-3), "Умереть"),
                new CustomTrash(5, "4_abobus", DateTime.Now.AddDays(-4), "Сдохнуть"),
                new CustomTrash(6, "5_abobus", DateTime.Now.AddDays(-5), "Умереть"),
            };
            Aboba.ItemsSource = new List<string>() { "Сдохнуть", "Умереть" };
            RoutesDataGrid.ItemsSource = dataMobile;
            Language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        }
        private void ModalPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            Locale locale = new Locale(startWindow.selectedLocale);
            locale.SetLocale(this);
            SetLinkedData();
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

        public async void SetLinkedData()
        {
            /*            roles = await startWindow.client.GetListRolesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                        types = await startWindow.client.GetListRequisiteTypesAsync(new Google.Protobuf.WellKnownTypes.Empty(), startWindow.headers);
                        RoleComboBox.ItemsSource = roles.RolesObject;
                        TypeComboBox.ItemsSource = types.RequisiteType;
                        if (data.Type != null)
                            TypeComboBox.SelectedItem = types.RequisiteType.First(x => x.Name == data.Type.Name);
                        if (data.Role != null)
                            RoleComboBox.SelectedItem = roles.RolesObject.First(x => x.Name == data.Role.Name);*/
        }

        public void UpdateDisplayedData(RequestsObject data)
        {
            /*            this.data = data;
                        NameTextBox.Text = data.Name.ToString();
                        AddressTextBox.Text = data.LegalAddress.ToString();
                        InnTextBox.Text = data.Inn.ToString();
                        CeoTextBox.Text = data.Ceo.ToString();
                        if (startWindow != null)
                            SetLinkedData();*/
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseAnimation();
        }

        private void LicenceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (sender as ComboBox);
            //var number = comboBox.SelectedItem.ToString();
            //var numberSeries = number.Split("/");
            //var foundedData = licenses.DriverLicence.Where(item => item.Series == int.Parse(numberSeries[0].ToString()) && item.Number == int.Parse(numberSeries[1].ToString())).ToList();
        }

        private async void UpdateData()
        {
            /*            try
                        {
                            var reqResult = new RequisitesObject();
                            if (mode == 0)
                                reqResult = await startWindow.client.UpdateRequisiteAsync(new CreateOrUpdateRequisitesRequest { Requisite = data });
                            if (mode == 1)
                                reqResult = await startWindow.client.CreateRequisiteAsync(new CreateOrUpdateRequisitesRequest { Requisite = data });
                            var tablePage = (TablePage)startWindow.MainFrameK.Content;
                            var page = tablePage.DataGridFrame.Content as RequisitesTablePage;
                            if (mode == 0)
                            {
                                var index = page.Requisites.FindIndex(t => t.Id == reqResult.Id);
                                page.Requisites[index] = reqResult;
                            }
                            if (mode == 1)
                                page.Requisites.Add(reqResult);
                            page.dataGrid.ItemsSource = null;
                            page.dataGrid.ItemsSource = page.Requisites;
                        }
                        catch (RpcException ex)
                        {

                        }*/

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            /*            StringBuilder changedDataNotify = new StringBuilder();

                        if (mode == 0)
                        {
                            if (NameTextBox.Text != data.Name.ToString())
                                changedDataNotify.Append($"Название: {data.Name} -> {NameTextBox.Text}\n");
                            if (CeoTextBox.Text != data.Ceo.ToString())
                                changedDataNotify.Append($"Владелец: {data.Ceo} -> {CeoTextBox.Text}\n");
                            if (InnTextBox.Text != data.Inn)
                                changedDataNotify.Append($"ИНН: {data.Inn} -> {InnTextBox.Text}\n");
                            if (AddressTextBox.Text != data.LegalAddress.ToString())
                                changedDataNotify.Append($"Юр. адрес: {data.LegalAddress} -> {AddressTextBox.Text}\n");
                            if ((RoleComboBox.SelectedItem as RolesObject)!.Name != data.Role.Name)
                                changedDataNotify.Append($"Роль: {data.Role.Name} -> {(RoleComboBox.SelectedItem as RolesObject)!.Name}\n");
                            if ((TypeComboBox.SelectedItem as RequisiteTypeObject)!.Name != data.Type.Name)
                                changedDataNotify.Append($"Роль: {data.Type.Name} -> {(TypeComboBox.SelectedItem as RequisiteTypeObject)!.Name}\n");
                        }

                        var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                        if (result == MessageBoxResult.Yes)
                        {
                            data.Name = NameTextBox.Text;
                            data.Ceo = CeoTextBox.Text;
                            data.Inn = InnTextBox.Text;
                            data.LegalAddress = AddressTextBox.Text;
                            data.Role = RoleComboBox.SelectedItem as RolesObject;
                            data.Type = TypeComboBox.SelectedItem as RequisiteTypeObject;
                            UpdateData();
                        }*/
        }
    }
}