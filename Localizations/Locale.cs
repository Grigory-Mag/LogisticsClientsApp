using LogisticsClientsApp.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LogisticsClientsApp.Localizations.Data;
using System.Windows;
using MaterialDesignThemes.Wpf;
using System.Diagnostics.Contracts;
using LogisticsClientsApp.Pages.Tables;
using LogisticsClientsApp.Pages.Modal;

namespace LogisticsClientsApp.Localizations
{
    class Locale
    {
        int test = 0;
        private class TablesLocale
        {
            static List<string> cargoTableHeaders_RU = new List<string>() { "Номер", "Тип", "Ограничения", "Масса", "Объём", "Наименование", "Цена", "Открыть", "Удалить" };

            public static List<string> GetCargoTableHeaders_RU() { return cargoTableHeaders_RU; }
        }

        private Page page;
        private Window window;
        private UserControl control;
        private string locale;
        public Locale(string locale)
        {
            this.locale = locale;
        }

        public void SetLocale(Page page)
        {
            this.page = page;
            switch (page)
            {
                case TablePage:
                    SetTablePageLocale_RU();
                    break;
                case LoginPage:
                    SetLoginPageLocale_RU();
                    break;
                case CargoTablePage:
                    SetCargoTableLocale_RU();
                    break;
                case CargoTypesPage:
                    SetCargoTypesTableLocale_RU();
                    break;

            }
        }

        public void SetLocale(Window window)
        {
            this.window = window;
            switch (window)
            {
                case StartWindow:
                    SetStartWindowLocale();
                    break;
            }
        }

        public void SetLocale(UserControl control)
        {
            this.control = control;
            switch (control)
            {
                case CargoTablePageModal:
                    SetCargoTablePageModalLocale_RU();
                    break;
            }
        }

        private void SetTablePageLocale_RU()
        {
            switch (locale)
            {
                case "ru":
                    List<string> table = TablesLocale.GetCargoTableHeaders_RU();
                    TablePage tablePage = page as TablePage;

                    tablePage.AddRecordBtn.Text = RU.AddRecordBtn;
                    HintAssist.SetHint(tablePage.SearchFilter, RU.FilterSearch);
                    switch (tablePage.selectedPage)
                    {
                        case CargoTablePage:
                            List<string> source = new List<string>();
                            for (int i = 0; i < table.Count - 2; i++)
                                source.Add(table[i]);
                            tablePage.SearchFilter.ItemsSource = source;
                            tablePage.TextBlockTableName.Text = RU.Cargo.ToLower();
                            break;
                        case CargoTypesPage:
                            tablePage.SearchFilter.ItemsSource = new List<string>() { table[5] };
                            tablePage.TextBlockTableName.Text = RU.Cargo_Types.ToLower();
                            break;
                        default:
                            tablePage.SearchFilter.ItemsSource = table;
                            tablePage.TextBlockTableName.Text = RU.Cargo.ToLower();
                            break;
                    }

                    break;
            }

        }

        private void SetCargoTableLocale_RU()
        {
            var page = this.page as CargoTablePage;
            List<string> table = TablesLocale.GetCargoTableHeaders_RU();

            for (int i = 0; i < (page.dataGrid.Columns.Count); i++)
                page.dataGrid.Columns[i].Header = table[i];

            page.dataGrid.Columns[0].Header = RU.SearchField;
        }

        private void SetCargoTypesTableLocale_RU()
        {
            var page = this.page as CargoTypesPage;
            var table = TablesLocale.GetCargoTableHeaders_RU();
            page.dataGrid.Columns[0].Header = table[5];
            page.dataGrid.Columns[1].Header = table[7];
            page.dataGrid.Columns[2].Header = table[8];
        }

        private void SetStartWindowLocale()
        {
            switch (locale)
            {
                case "ru":
                    StartWindow startWindow = window as StartWindow;
                    startWindow.TextBlockReferences.Text = RU.ReferencesBtn;
                    startWindow.TextBlockMessages.Text = RU.MessagesBtn;
                    startWindow.TextBlockEmail.Text = RU.EamilBtn;
                    startWindow.TextBlockAccount.Text = RU.AccountBtn;
                    startWindow.TextBlockLogout.Text = RU.LogoutBtn;
                    startWindow.tables = new Dictionary<string, List<string>>()
                    {
                        { RU.Cargo, new List<string>() {RU.Cargo_Types, RU.Constraints, RU.Cargo } },
                        { RU.Drivers, new List<string>() {RU.Driver_Licence, RU.Drivers} },
                        { RU.Customers, new List<string>() {RU.Requisites, RU.Ownerships, RU.Customers} },
                        { RU.Vehicles, new List<string>() {RU.Vehicle_Types, RU.Vehicles} },
                        { RU.Orders, new List<string>() },
                        { RU.Requests, new List<string>()},
                        { RU.Users, new List<string>()},
                    };
                    startWindow.tablesList = new List<string>()
                    {
                        RU.Cargo,
                        RU.Cargo_Types,
                        RU.Constraints,
                        RU.Drivers,
                        RU.Driver_Licence,
                        RU.Customers,
                        RU.Requisites,
                        RU.Ownerships,
                        RU.Orders,
                        RU.Vehicles,
                        RU.Vehicle_Types,
                        RU.Requests,
                        RU.Users,
                    };

                    /*{
                        
                        RU.Cargo,
                            RU.Cargo_Types,
                            RU.Constraints,
                        RU.Drivers,
                            RU.Driver_Licence,
                        RU.Customers,
                            RU.Requisites,
                                RU.Ownerships,
                        RU.Orders,
                        RU.Vehicles,
                            RU.Vehicle_Types,
                        RU.Requests,
                        RU.Users,
                    };*/
                    break;
            }
        }

        private void SetCargoTablePageModalLocale_RU()
        {
            CargoTablePageModal cargoModalPage = control as CargoTablePageModal;
            cargoModalPage.MainDataTextBlock.Text = RU.MainDataTextBlock;
            cargoModalPage.SecondaryDataTextBlock.Text = RU.SecondaryDataTextBlock;
            HintAssist.SetHint(cargoModalPage.WeightTextBox, RU.CargoWeight);
            HintAssist.SetHint(cargoModalPage.VolumeTextBox, RU.CargoVolume);
            HintAssist.SetHint(cargoModalPage.NameTextBox, RU.CargoName);
            HintAssist.SetHint(cargoModalPage.PriceTextBox, RU.CargoPrice);
            HintAssist.SetHint(cargoModalPage.TypeComboBox, RU.Cargo_Types);
        }

        private void SetLoginPageLocale_RU()
        {
            switch (locale)
            {
                case "ru":
                    LoginPage loginPage = page as LoginPage;
                    loginPage.LoginButton.Content = RU.LoginButtonText;
                    loginPage.WelcomeBackTextBlock.Text = RU.WelcomeBack;
                    loginPage.LoginTextBlock.Text = RU.LoginAccountPlease;
                    loginPage.RoleComboBox.ItemsSource = new List<string>() { RU.RoleLogistic, RU.RoleAdmin };
                    loginPage.ErrorTextBlock.Text = RU.ErrorTextBlock;
                    HintAssist.SetHint(loginPage.RoleComboBox, RU.RoleComboBoxHint);
                    HintAssist.SetHint(loginPage.LoginTextBox, RU.LoginHint);
                    HintAssist.SetHint(loginPage.PasswordTextBox, RU.PasswordHint);
                    break;
            }
        }
    }
}
