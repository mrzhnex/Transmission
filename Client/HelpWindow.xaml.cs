using System.Windows;

namespace Client
{
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            Closing += HelpWindow_Closing;
        }

        private void HelpWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.MainWindowInstance.IsHelpWindowOpened = false;
        }

    }
}