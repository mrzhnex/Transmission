using Core.Localization;
using Core.Main;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Server
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            SetLanguageBinding();
            ServerPortField.Text = Manage.DefaultInformation.ServerPort.ToString();
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
            TextBlock[] textBlocks = new TextBlock[]
            {
                ServerPort
            };
            foreach (TextBlock textBlock in textBlocks)
            {
                SetLanguageBindingTextBlock(textBlock);
            }
        }
        private void SetLanguageBindingTextBlock(TextBlock textBlock)
        {
            textBlock.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { textBlock.SetBinding(TextBlock.TextProperty, new Binding(textBlock.Name) { Source = Manage.LocalizationManager.Current }); }));
        }

        private void ServerPortField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse((sender as TextBox).Text, out int port) || port < 1025 || port > 65535)
            {
                Manage.Logger.Add($"Некорректный порт {(sender as TextBox).Text}", LogType.Application, LogLevel.Warn);
                return;
            }
            Manage.DefaultInformation.ServerPort = port;
        }
    }
}