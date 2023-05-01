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
    /// Логика взаимодействия для VehiclesTypesTablePage.xaml
    /// </summary>
    public partial class VehiclesTypesTablePage : Page
    {
        public List<VehiclesTypesObject> types { get; set; }
        private Locale locale;

        StartWindow startWindow;
        public VehiclesTypesTablePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            startWindow = (StartWindow)Window.GetWindow(this);
            locale = new Locale(startWindow.selectedLocale);
            SetData();
        }

        private void PrevTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextTablePageButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var item = dataGrid.SelectedItem;
        }

        private async void SetData()
        {
            try
            {
                var item = await startWindow.client.GetListVehiclesTypesAsync(new Google.Protobuf.WellKnownTypes.Empty());
                types = new List<VehiclesTypesObject>();
                types.AddRange(item.VehiclesTypes.ToList());
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = types;
                locale.SetLocale(this);
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