using ApiService;
using Grpc.Core;
using LogisticsClientsApp.Localizations;
using LogisticsClientsApp.Pages.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для RequisitesTablePageModal.xaml
    /// </summary>
    public partial class RequisitesTablePageModal : UserControl
    {
        public RequisitesObject data;
        public ListRoles roles;
        private Locale locale;

        StartWindow startWindow;

        public RequisitesTablePageModal()
        {
            InitializeComponent();
        }

        private void ModalPageControl_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            Locale locale = new Locale(startWindow.selectedLocale);
            locale.SetLocale(this);
            SetRoles();
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

        public async void SetRoles()
        {
            roles = await startWindow.client.GetListRolesAsync(new Google.Protobuf.WellKnownTypes.Empty());
            RoleComboBox.ItemsSource = roles.RolesObject;
            RoleComboBox.SelectedItem = roles.RolesObject.First(x=> x.Name == data.Role.Name);
        }

        public void UpdateDisplayedData(RequisitesObject data)
        {
            this.data = data;
            NameTextBox.Text = data.Name.ToString();
            AddressTextBox.Text = data.LegalAddress.ToString();
            InnTextBox.Text = data.Inn.ToString();
            CeoTextBox.Text = data.Ceo.ToString();
            if (startWindow != null)
                SetRoles();
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
            try
            {
                var reqResult = await startWindow.client.UpdateRequisiteAsync(new CreateOrUpdateRequisitesRequest { Requisite = data });
                var tablePage = (TablePage)startWindow.MainFrameK.Content;
                var page = tablePage.DataGridFrame.Content as RequisitesTablePage;
                var index = page.requisites.FindIndex(t => t.Id == reqResult.Id);
                page.requisites[index] = reqResult;
                page.dataGrid.ItemsSource = null;
                page.dataGrid.ItemsSource = page.requisites;
            }
            catch (RpcException ex)
            {

            }

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder changedDataNotify = new StringBuilder();

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
            
            var result = MessageBox.Show($"Применить изменения?\n {changedDataNotify}", "Обновление", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                data.Name = NameTextBox.Text;
                data.Ceo = CeoTextBox.Text;
                data.Inn = InnTextBox.Text;
                data.LegalAddress = AddressTextBox.Text;
                data.Role = RoleComboBox.SelectedItem as RolesObject;
                UpdateData();
            }
        }
    }
}
