using System.Diagnostics;
using MahApps.Metro.Controls;
using BFBC2Toolkit.Data;
using BFBC2Shared.Data;

namespace BFBC2Toolkit.Windows
{
    public partial class InfoWindow : MetroWindow
    {
        public InfoWindow()
        {
            InitializeComponent();
        }

        private void InfoWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            lblName.Content = "BFBC2 Toolkit " + SharedGlobals.ClientVersion;
        }

        private void BtnCredits_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(Dirs.Docs);
        }
    }
}
