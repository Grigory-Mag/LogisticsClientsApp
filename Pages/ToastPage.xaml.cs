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
using System.Windows.Threading;

namespace LogisticsClientsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для Test.xaml
    /// </summary>
    public partial class ToastPage : Page
    {

        public ToastPage()
        {
            InitializeComponent();
        }

        public ToastPage(TablePage.Messages answer)
        {

            InitializeComponent();

            switch (answer)
            {
                case TablePage.Messages.Success:
                    ToastIcon.Kind = PackIconKind.CheckCircleOutline;
                    ToastIcon.Foreground = new SolidColorBrush(Colors.LimeGreen);
                    MessageBox.Text = "операция завершена успешно";
                    break;
                case TablePage.Messages.Error:
                    ToastIcon.Kind = PackIconKind.CloseCircle;
                    ToastIcon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DB4437"));
                    MessageBox.Text = "произошла ошибка";
                    break;
            }
            StartTimer();
        }

        public void StartTimer()
        {
            DrawerHost.IsTopDrawerOpen = true;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var timer = sender as DispatcherTimer;
            ClosingProgress.Value += .5;
            if (ClosingProgress.Value == 100)
            {
                ClosingProgress.Value = 0;
                timer.Stop();
                DrawerHost.IsTopDrawerOpen = false;
            }
        }
    }
}
