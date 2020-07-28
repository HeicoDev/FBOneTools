using System;
using System.Windows;
using System.Xml;
using MahApps.Metro.Controls;
using BFBC2_Toolkit.Data;
using BFBC2_Toolkit.Functions;

namespace BFBC2_Toolkit.Windows
{
    public partial class SelectGameWindow : MetroWindow
    {
        public SelectGameWindow()
        {
            InitializeComponent();
        }

        private void SelectGameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var xr = new XmlTextReader(Dirs.ConfigGames))
                {
                    string name = "",
                           platform = "";

                    while (xr.Read())
                    {
                        while (xr.MoveToNextAttribute())
                        {
                            if (xr.Name == "Name")
                            {
                                name = xr.Value;
                            }

                            if (xr.Name == "Platform")
                            {
                                platform = xr.Value;
                            }

                            if (platform != "" && name != "")
                            {
                                dataGrid.Items.Add(new GameProfile() { Name = name, Platform = platform });

                                name = "";
                                platform = "";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to load game profiles! See error.log", "error");

                Close();
            }
        }

        private async void BtnSelectGame_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                progressRing.IsActive = true;                

                var selectedProfile = dataGrid.SelectedItem as GameProfile;

                await Profile.Load(selectedProfile);

                Vars.IsDataAvailable = true;
                Vars.IsGameProfile = true;

                progressRing.IsActive = false;

                Close();
            }
        }

        private async void BtnDeleteGame_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                progressRing.IsActive = true;

                var selectedProfile = dataGrid.SelectedItem as GameProfile;

                await Profile.Delete(selectedProfile);

                dataGrid.Items.RemoveAt(dataGrid.Items.IndexOf(dataGrid.SelectedItem));

                progressRing.IsActive = false;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }        
    }   
}
