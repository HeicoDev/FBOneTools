using System;
using System.Windows;
using System.Diagnostics;
using MahApps.Metro.Controls;
using BFBC2ModLoader.Data;

namespace BFBC2ModLoader.Windows
{
    public partial class WindowAppInfo : MetroWindow
    {
        public WindowAppInfo()
        {
            InitializeComponent();
        }

        private void AppInfo_Loaded(object sender, RoutedEventArgs e)
        {
            lblName.Content = "BFBC2 Mod Loader " + Globals.VersionClient;
        }

        private void BtnCredits_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.CurrentDirectory + @"\BFBC2ModLoader\Docs");
        }
    }
}
