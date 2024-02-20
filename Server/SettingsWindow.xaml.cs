using Core.Application;
using Core.Events;
using Core.Handlers;
using Core.Localization;
using Core.Main;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
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

            foreach (KeyValuePair<ThemeDesignation, Theme> keyValuePair in Core.Application.Manager.Themes)
            {
                Themes.Items.Add(new ComboBoxItem() { Name = keyValuePair.Key.ThemeType.ToString(), Content = keyValuePair.Key.ThemeName });
            }


            ServerPortField.Text = Manage.ApplicationManager.ServerSettings.Port.ToString();
            ServerPasswordField.Text = Manage.ApplicationManager.ServerSettings.Password;

            Sort.Items.Add($"имя");
            Sort.Items.Add($"IP");
            Sort.Items.Add($"время");
            Sort.SelectedValue = "имя";

            FontStyles.ItemsSource = Core.Application.Info.FontFamilies;

            foreach (FontFamily fontFamily in Core.Application.Info.FontFamilies)
            {
                if (fontFamily.Source == Manage.ApplicationManager.ServerSettings.FontFamily)
                {
                    FontStyles.SelectedItem = fontFamily;
                }
            }

            if (FontStyles.SelectedItem == null)
            {
                FontStyles.SelectedItem = new FontFamily(Manage.DefaultInformation.DefaultFontFamily);
            }

            ShouldMirroAudio.SelectedItem = Manage.ApplicationManager.ServerSettings.ShouldMirrorAudio ? ShouldMirroAudio.Items[0] : ShouldMirroAudio.Items[1];
            foreach (ComboBoxItem comboBoxItem in Themes.Items)
            {
                if ((ThemeType)Enum.Parse(typeof(ThemeType), comboBoxItem.Name) == Manage.ApplicationManager.ServerSettings.ThemeType)
                    Themes.SelectedItem = comboBoxItem;
            }
            RecordSaveFolder.Text = Manage.ApplicationManager.ServerSettings.RecordSaveFolder == string.Empty ? Manage.Logger.LogsFolder : Manage.ApplicationManager.ServerSettings.RecordSaveFolder;
            PlayAudioFile.Text = Manage.ApplicationManager.ServerSettings.PlayAudioFile == string.Empty ? Manage.DefaultInformation.DefaultFileName : Manage.ApplicationManager.ServerSettings.PlayAudioFile;
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
            Manage.ApplicationManager.ServerSettings.Port = port;
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
                Manage.Logger.Add($"Некорректный пароль {(sender as TextBox).Text}", LogType.Application, LogLevel.Warn);
                return;
            }
            Manage.ApplicationManager.ServerSettings.Password = (sender as TextBox).Text;
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
                Manage.ApplicationManager.ServerSettings.RecordSaveFolder = commonOpenFileDialog.FileName;
                RecordSaveFolder.Text = Manage.ApplicationManager.ServerSettings.RecordSaveFolder;
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
                Manage.ApplicationManager.ServerSettings.PlayAudioFile = commonOpenFileDialog.FileName;
                PlayAudioFile.Text = Manage.ApplicationManager.ServerSettings.PlayAudioFile;
                Manage.Application.LoadAudioData(Manage.ApplicationManager.ServerSettings.PlayAudioFile);
            }
        }

        private void Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Manage.ApplicationManager.ServerSettings.ThemeType = (ThemeType)Enum.Parse(typeof(ThemeType), (((ComboBox)sender).SelectedItem as ComboBoxItem).Name);
        }

        private void FontStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily fontFamily = (FontFamily)((ComboBox)sender).SelectedItem;
            Manage.ApplicationManager.ServerSettings.FontFamily = fontFamily.Source;
            Manage.EventManager.ExecuteEvent<IEventHandlerFontFamilyChanged>(new FontFamilyChangedEvent(fontFamily.Source));
        }

        public void OnFontFamilyChanged(FontFamilyChangedEvent fontFamilyChangedEvent)
        {
            FontFamily = new FontFamily(fontFamilyChangedEvent.FontFamilyName);
        }

        private void ShouldMirroAudio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Manage.ApplicationManager.ServerSettings.ShouldMirrorAudio = bool.Parse((((ComboBox)sender).SelectedItem as ComboBoxItem).Name);
        }
    }
}