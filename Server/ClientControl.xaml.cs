using Core.Server;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using Core.Handlers;
using Core.Events;
using System.Windows.Media;
using Core.Main;

namespace Server
{
    public partial class ClientControl : UserControl, IEventHandlerFontFamilyChanged, IEventHandlerSpectrumUpdate
    {
        public SpectrumControl InputSpectrum { get; set; } = new SpectrumControl();
        public int Id { get; set; } = -1;
        public ClientControl()
        {
            InitializeComponent();
            InputSpectrum = InputSpectrumControl;
            Manage.Application.AddEventHandlers(this);
        }

        public void OnFontFamilyChanged(FontFamilyChangedEvent fontFamilyChangedEvent)
        {
            FontFamily = new FontFamily(fontFamilyChangedEvent.FontFamilyName);
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
        public void OnSpectrumUpdate(SpectrumUpdateEvent spectrumUpdateEvent)
        {
            if (Id == spectrumUpdateEvent.Id)
            {
                InputSpectrum.ProcessData(spectrumUpdateEvent.Data, spectrumUpdateEvent.Silent);
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Id = ((ConnectionInfo)DataContext).Id;
        }
    }
}