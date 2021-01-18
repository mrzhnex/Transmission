using Core.Server;
using System.Windows;
using System.Windows.Controls;

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
            if (MainWindow.MainWindowInstance.OpenedClients.Contains(connectionInfo.Id))
                return;
            MainWindow.MainWindowInstance.OpenedClients.Add(connectionInfo.Id);
            ClientWindow clientWindow = new ClientWindow(connectionInfo.Id);
            clientWindow.Show();
        }

        private void UserControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow.MainWindowInstance.DragConnectionInfo = DataContext as ConnectionInfo;
            DragDrop.DoDragDrop(this, DataContext, DragDropEffects.Move);
        }
    }
}