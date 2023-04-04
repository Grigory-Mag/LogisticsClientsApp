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

namespace LogisticsClientsApp.Localizations
{
    class Locale
    {
        private class TablesLocale
        {
            static List<string> cargoTableHeaders = new List<string>() { "Номер", "Тип", "Ограничения", "Масса", "Объём", "Наименование", "Цена" };
            
            public static List<string> GetCargoTableHeaders() { return cargoTableHeaders; }
        }

        private Page page;
        private Window window;
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
                    SetTablePageLocale();
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

        private void SetTablePageLocale()
        {
            switch (locale)
            {
                case "ru":
                    List<string> table = TablesLocale.GetCargoTableHeaders();
                    TablePage tablePage = page as TablePage;
                    for (int i = 0; i < (tablePage.phonesGrid.Columns.Count - 2); i++)
                        tablePage.phonesGrid.Columns[i].Header = table[i];
                    
                    tablePage.phonesGrid.Columns[0].Header = RU.SearchField;
                    tablePage.AddRecordBtn.Text = RU.AddRecordBtn;
                    HintAssist.SetHint(tablePage.SearchFilter, RU.FilterSearch);
                    tablePage.SearchFilter.ItemsSource = table;
                    break;
            }

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
                    break;
            }
        }
    }
}
