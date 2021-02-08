using Core.Application;
using Core.Events;
using Core.Handlers;
using Core.Localization;
using Core.Main;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Server
{
    public partial class SettingsWindow : Window, IEventHandlerFontFamilyChanged
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Manage.Application.AddEventHandlers(this);
            SetLanguageBinding();

            foreach (KeyValuePair<ThemeType, Theme> keyValuePair in Core.Application.Manager.Themes)
            {
                Themes.Items.Add(keyValuePair.Key);
            }

            ServerPortField.Text = Manage.ApplicationManager.Current.ServerSettings.Port.ToString();
            ServerPasswordField.Text = Manage.ApplicationManager.Current.ServerSettings.Password;

            for (int i = 1; i < 4; i++)
            {
                InputOutputType.Items.Add($"вид {i}");
            }
            InputOutputType.SelectedItem = "вид 1";
            Sort.Items.Add($"имя");
            Sort.Items.Add($"IP");
            Sort.Items.Add($"время");
            Sort.SelectedValue = "имя";

            var installedFontCollection = new System.Drawing.Text.InstalledFontCollection();
            foreach (System.Drawing.FontFamily fontFamily in installedFontCollection.Families)
            {
                FontFamily font = new FontFamily(fontFamily.Name);
                if (font.Source == Manage.ApplicationManager.Current.ServerSettings.FontFamily)
                {
                    FontStyles.SelectedItem = font;
                }
            }

            if (FontStyles.SelectedItem == null)
            {
                FontStyles.SelectedItem = new FontFamily(Manage.DefaultInformation.DefaultFontFamily);
            }

            Themes.SelectedItem = Manage.ApplicationManager.Current.ServerSettings.ThemeType;

            RecordSaveFolder.Text = Manage.ApplicationManager.Current.ServerSettings.RecordSaveFolder == string.Empty ? Manage.Logger.LogsFolder : Manage.ApplicationManager.Current.ServerSettings.RecordSaveFolder;
            PlayAudioFile.Text = Manage.ApplicationManager.Current.ServerSettings.PlayAudioFile == string.Empty ? Manage.DefaultInformation.DefaultFileName : Manage.ApplicationManager.Current.ServerSettings.PlayAudioFile;
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
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
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
            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Manage.ApplicationManager.Current.ServerSettings.RecordSaveFolder = commonOpenFileDialog.FileName;
                RecordSaveFolder.Text = Manage.ApplicationManager.Current.ServerSettings.RecordSaveFolder;
            }
        }

        private void PlayAudioFile_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
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
            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Manage.ApplicationManager.Current.ServerSettings.PlayAudioFile = commonOpenFileDialog.FileName;
                PlayAudioFile.Text = Manage.ApplicationManager.Current.ServerSettings.PlayAudioFile;
                Manage.Application.LoadAudioData(Manage.ApplicationManager.Current.ServerSettings.PlayAudioFile);
            }
        }

        private void Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ThemeType themeType = (ThemeType)comboBox.SelectedItem;
            Manage.ApplicationManager.Current.ServerSettings.ThemeType = themeType;
        }

        private void FontStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            FontFamily fontFamily = (FontFamily)comboBox.SelectedItem;
            Manage.ApplicationManager.Current.ServerSettings.FontFamily = fontFamily.Source;
            Manage.EventManager.ExecuteEvent<IEventHandlerFontFamilyChanged>(new FontFamilyChangedEvent(fontFamily.Source));
        }

        public void OnFontFamilyChanged(FontFamilyChangedEvent fontFamilyChangedEvent)
        {
            FontFamily = new FontFamily(fontFamilyChangedEvent.FontFamilyName);
        }
    }
}