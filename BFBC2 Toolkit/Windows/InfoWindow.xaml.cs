using MahApps.Metro.Controls;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Windows
{
    public partial class InfoWindow : MetroWindow
    {
        public InfoWindow()
        {
            InitializeComponent();
        }

        private void InfoWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            lblName.Content = "BFBC2 Toolkit " + Vars.VersionClient;
        }
    }
}
