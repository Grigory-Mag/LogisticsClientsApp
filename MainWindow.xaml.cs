using ApiService;
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
using System.Windows.Media.Animation;
using System.Reflection;
using MaterialDesignThemes.Wpf.Converters;
using MaterialDesignThemes.Wpf;

namespace ClientsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MenuOpenBtn.Click += Button_Click_1;
            MenuCloseBtn.Click += btnclose_Click;
            textBlocks.Add(TextBlockReferences);
            textBlocks.Add(TextBlockMessages);
            textBlocks.Add(TextBlockEmail);
            textBlocks.Add(TextBlockAccount);

            selectedBtns.Add(true);
            selectedBtns.Add(false);
            selectedBtns.Add(false);
            selectedBtns.Add(false);

            packIcons.Add(ReferencesIcon);
            packIcons.Add(MessagesIcon);
            packIcons.Add(EmailIcon);
            packIcons.Add(AccountIcon);

            buttons.Add(ReferencesBtn);
            buttons.Add(MessagesBtn);

            SelectBtn(0, ReferencesBtn);
        }

        private List<bool> selectedBtns = new List<bool>();
        private List<TextBlock> textBlocks = new List<TextBlock>();
        private List<PackIcon> packIcons = new List<PackIcon>();
        private List<Button> buttons = new List<Button>();

        private static Color PRIMARY_COLOR = Color.FromArgb(255, 33, 150, 243);


        private void SetSelectedColor(object sender)
        {
            //(sender as Button).Foreground = new SolidColorBrush(Colors.LightGray);
            (sender as Button).Background = new SolidColorBrush(PRIMARY_COLOR);
            (sender as Button).Background.Opacity = .7;
        }

        private void SetUnselectedColor(object sender)
        {
            (sender as Button).Foreground = new SolidColorBrush(PRIMARY_COLOR);
            (sender as Button).Background = null;
        }

        private void UnselectBtn(int index, object sender)
        {
            SetUnselectedColor(sender);
            textBlocks[index].Foreground = new SolidColorBrush(PRIMARY_COLOR);
            packIcons[index].Foreground = new SolidColorBrush(PRIMARY_COLOR);
            selectedBtns[index] = false;
        }

        private void SelectBtn(int index, object sender)
        {
            SetSelectedColor(sender);
            textBlocks[index].Foreground = new SolidColorBrush(Colors.White);
            packIcons[index].Foreground = new SolidColorBrush(Colors.White);
            selectedBtns[index] = true;
        }

        private void CheckSelectionBtn(int index, object sender)
        {
            if (selectedBtns[index])
                UnselectBtn(index, sender);
            else
                SelectBtn(index, sender);

        }

        private void UncheckAllBtns()
        {
            selectedBtns.ForEach(item => item = true);
            buttons.ForEach(item => UnselectBtn(buttons.IndexOf(item), item));
        }

        

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReferencesBtn_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllBtns();
            CheckSelectionBtn(0, sender);
        }

        private void MessagesBtn_Click(object sender, RoutedEventArgs e)
        {
            UncheckAllBtns();
            CheckSelectionBtn(1, sender);
        }

        private void MenuCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["CloseMenu"] as Storyboard;
            sb.Begin(LeftMenu);
            MenuCloseBtn.Visibility = Visibility.Hidden;
            MenuOpenBtn.Visibility = Visibility.Visible;
        }

        private void MenuOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = Resources["OpenMenu"] as Storyboard;
            sb.Begin(LeftMenu);
            MenuOpenBtn.Visibility = Visibility.Hidden;
            MenuCloseBtn.Visibility = Visibility.Visible;
        }
    }
}
