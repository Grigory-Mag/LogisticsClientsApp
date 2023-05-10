using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogisticsClientsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public static List<string> Languages = new List<string>() { "Русский", "English (United States)" };

        ITheme theme = new PaletteHelper().GetTheme();
        public PaletteHelper palette;

        public SettingsPage()
        {
            InitializeComponent();
            LanguagesComboBox.ItemsSource = Languages;
            LanguagesComboBox.SelectedIndex = 0;
        }

        private void ToggleDarkMode_Checked(object sender, RoutedEventArgs e)
        {
            theme.SetBaseTheme(Theme.Dark);
            palette.SetTheme(theme);
        }

        private void ToggleDarkMode_Unchecked(object sender, RoutedEventArgs e)
        {
            theme.SetBaseTheme(Theme.Light);
            palette.SetTheme(theme);
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            palette = new PaletteHelper();
        }

        private void ChooseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".png";
            openFileDialog.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg)";

            var result = openFileDialog.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = openFileDialog.FileName;
                FilePathTextBox.Text = filename;
                var uri = new Uri(filename);
            }
        }

        private void FilePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            Uri? uri;
            var created = Uri.TryCreate(textBox.Text, UriKind.Absolute, out uri);
            if (created)
                try
                {
                    BitmapImage background = new BitmapImage();
                    background.BeginInit();
                    background.UriSource = new Uri(textBox.Text);
                    background.EndInit();
                    SelectedBackgroundImage.Source = background;
                    var startWindow = Window.GetWindow(this) as StartWindow;
                    startWindow.ProfileBackgroundImage.ImageSource = background;
                    SelectedBackgroundImage.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }

        private void SelectedBackgroundImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var imageBtn = (Image)sender;
            if (imageBtn.Source != null)
            {
                var imagePage = new ImagePage((BitmapImage)imageBtn.Source);
                imagePage.ShowDialog();
            }
        }

        private void ChooseMainFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".png";
            openFileDialog.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg)";

            var result = openFileDialog.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = openFileDialog.FileName;
                MainFilePathTextBox.Text = filename;
                var uri = new Uri(filename);
            }
        }


        private void MainFilePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            Uri? uri;
            var created = Uri.TryCreate(textBox.Text, UriKind.Absolute, out uri);
            if (created)
                try
                {
                    BitmapImage background = new BitmapImage();
                    background.BeginInit();
                    background.UriSource = new Uri(textBox.Text);
                    background.EndInit();
                    SelectedForegroundImage.Source = background;
                    var startWindow = Window.GetWindow(this) as StartWindow;
                    startWindow!.UserProfileImage.ImageSource = background;
                    SelectedForegroundImage.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }

        private void SelectedForegroundImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var imageBtn = (Image)sender;
            if (imageBtn.Source != null)
            {
                var imagePage = new ImagePage((BitmapImage)imageBtn.Source);
                imagePage.ShowDialog();
            }
        }
    }
}
