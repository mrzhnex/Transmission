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

namespace Client
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

            FontStyles.ItemsSource = Core.Application.Info.FontFamilies;

            foreach (FontFamily fontFamily in Core.Application.Info.FontFamilies)
            {
                if (fontFamily.Source == Manage.ApplicationManager.ClientSettings.FontFamily)
                {
                    FontStyles.SelectedItem = fontFamily;
                }
            }

            if (FontStyles.SelectedItem == null)
            {
                FontStyles.SelectedItem = new FontFamily(Manage.DefaultInformation.DefaultFontFamily);
            }

            ShouldLog.SelectedItem = Manage.ApplicationManager.ClientSettings.ShouldLog ? ShouldLog.Items[0] : ShouldLog.Items[1];
            foreach (ComboBoxItem comboBoxItem in Themes.Items)
            {
                if ((ThemeType)Enum.Parse(typeof(ThemeType), comboBoxItem.Name) == Manage.ApplicationManager.ClientSettings.ThemeType)
                    Themes.SelectedItem = comboBoxItem;
            }
            RecordSaveFolder.Text = Manage.ApplicationManager.ClientSettings.RecordSaveFolder == string.Empty ? Manage.Logger.LogsFolder : Manage.ApplicationManager.ClientSettings.RecordSaveFolder;
            PlayAudioFile.Text = Manage.ApplicationManager.ClientSettings.PlayAudioFile == string.Empty ? Manage.DefaultInformation.DefaultFileName : Manage.ApplicationManager.ClientSettings.PlayAudioFile;
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
                Manage.ApplicationManager.ClientSettings.PlayAudioFile = commonOpenFileDialog.FileName;
                PlayAudioFile.Text = Manage.ApplicationManager.ClientSettings.PlayAudioFile;
                Manage.Application.LoadAudioData(Manage.ApplicationManager.ClientSettings.PlayAudioFile);
            }
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
                Manage.ApplicationManager.ClientSettings.RecordSaveFolder = commonOpenFileDialog.FileName;
                RecordSaveFolder.Text = Manage.ApplicationManager.ClientSettings.RecordSaveFolder;
            }
        }

        private void FontStyles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily fontFamily = (FontFamily)((ComboBox)sender).SelectedItem;
            Manage.ApplicationManager.ClientSettings.FontFamily = fontFamily.Source;
            Manage.EventManager.ExecuteEvent<IEventHandlerFontFamilyChanged>(new FontFamilyChangedEvent(fontFamily.Source));
        }


        public void OnFontFamilyChanged(FontFamilyChangedEvent fontFamilyChangedEvent)
        {
            FontFamily = new FontFamily(fontFamilyChangedEvent.FontFamilyName);
        }

        private void Themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Manage.ApplicationManager.ClientSettings.ThemeType = (ThemeType)Enum.Parse(typeof(ThemeType), (((ComboBox)sender).SelectedItem as ComboBoxItem).Name);
        }

        private void ShouldLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Manage.ApplicationManager.ClientSettings.ShouldLog = bool.Parse((((ComboBox)sender).SelectedItem as ComboBoxItem).Name);
        }
    }
}