using System.Windows.Controls;
using Core.Application;

namespace Client
{
    public partial class SpectrumControl : UserControl
    {
        public SpectrumControl()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            ((Spectrum)DataContext).Values.Dispose();
        }
    }
}