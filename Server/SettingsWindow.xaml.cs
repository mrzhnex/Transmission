using Core.Localization;
using Core.Main;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Server
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            SetLanguageBinding();
            ServerPortField.Text = Manage.ApplicationManager.Current.ServerSettings.Port.ToString();
            ServerPasswordField.Text = Manage.ApplicationManager.Current.ServerSettings.Password;
            for (int i = 1; i < 4; i++)
            {
                OutputType.Items.Add($"вид {i}");
                InputType.Items.Add($"вид {i}");
            }
            Sort.Items.Add($"имя");
            Sort.Items.Add($"IP");
            Sort.Items.Add($"время");
            OutputType.SelectedItem = "вид 1";
            InputType.SelectedItem = "вид 1";
            Sort.SelectedValue = "имя";
        }
        private void Languages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Manage.LocalizationManager.Current.CompareToNew(Manage.LocalizationManager.Languages.FirstOrDefault(x => x.Name == ((ComboBox)sender).SelectedValue.ToString()));
        }

        private void SetLanguageBinding()
        {
            foreach (Language language in Manage.LocalizationManager.Languages)
            {
                Languages.Items.Add(language.Name);
            }
            Languages.SelectedValue = Manage.LocalizationManager.Current.Name;
        }

        private void ServerPortField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse((sender as TextBox).Text, out int port) || port < 1025 || port > 65535)
            {
                Manage.Logger.Add($"Некорректный порт {(sender as TextBox).Text}", LogType.Application, LogLevel.Warn);
                return;
            }
            Manage.ApplicationManager.Current.ServerSettings.Port = port;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.MainWindowInstance.IsSettingsWindowOpened = false;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Deploy_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Deploy.Content = FindResource("Deploy");
                WindowState = WindowState.Normal;
            }
            else
            {
                Deploy.Content = FindResource("DeployTwo");
                WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ServerPasswordField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text == null)
            {
                Manage.Logger.Add($"Некорректный порт {(sender as TextBox).Text}", LogType.Application, LogLevel.Warn);
                return;
            }
            Manage.ApplicationManager.Current.ServerSettings.Password = (sender as TextBox).Text;
        }

        private void RecordSaveFolder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                InitialDirectory = Manage.Logger.LogsFolder,

                AddToMostRecentlyUsedList = false,
                AllowNonFileSystemItems = false,
                DefaultDirectory = Manage.Logger.LogsFolder,
                EnsureFileExists = true,
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                Multiselect = false,
                ShowPlacesList = true
            };

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string folder = dlg.FileName;
                throw new System.Exception(folder);
                // Do something with selected folder string
            }
        }
    }
}