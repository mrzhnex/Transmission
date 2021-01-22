using Core.Server;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

namespace Server
{
    public partial class ClientControl : UserControl
    {
        public ClientControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ConnectionInfo connectionInfo = (ConnectionInfo)DataContext;
            if (MainWindow.MainWindowInstance.ClientWindows.FirstOrDefault(x => x.Id == connectionInfo.Id) != default)
                return;
            ClientWindow clientWindow = new ClientWindow(connectionInfo.Id);
            MainWindow.MainWindowInstance.ClientWindows.Add(clientWindow);
            clientWindow.Show();
        }

        private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.MainWindowInstance.DragConnectionInfo = DataContext as ConnectionInfo;
            DragDrop.DoDragDrop(this, DataContext, DragDropEffects.Move);
        }
    }
}