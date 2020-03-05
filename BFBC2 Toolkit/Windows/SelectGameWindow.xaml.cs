using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                using (var xr = new XmlTextReader(Dirs.configGames))
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
            try
            {
                if (Directory.Exists(Dirs.filesPathData) && Vars.isGameProfile == false)
                    await Task.Run(() => Directory.Delete(Dirs.filesPathData, true));

                if (dataGrid.SelectedItems != null)
                {
                    var row = dataGrid.SelectedItem as GameProfile;

                    if (row.Name == "Battlefield Bad Company 2" && row.Platform == "PC")
                    {
                        Tree.Populate(Elements.TreeViewDataExplorer, Dirs.games + @"\BFBC2-PC");
                        Dirs.filesPathData = Dirs.games + @"\BFBC2-PC";
                    }
                    else if (row.Name == "Battlefield Bad Company 2 Server" && row.Platform == "PC")
                    {
                        Tree.Populate(Elements.TreeViewDataExplorer, Dirs.games + @"\BFBC2-Server-PC");
                        Dirs.filesPathData = Dirs.games + @"\BFBC2-Server-PC";
                    }
                    else if (row.Name == "Battlefield Bad Company 2" && row.Platform == "PS3")
                    {
                        Tree.Populate(Elements.TreeViewDataExplorer, Dirs.games + @"\BFBC2-PS3");
                        Dirs.filesPathData = Dirs.games + @"\BFBC2-PS3";
                    }
                    else if (row.Name == "Battlefield Bad Company 2" && row.Platform == "Xbox")
                    {
                        Tree.Populate(Elements.TreeViewDataExplorer, Dirs.games + @"\BFBC2-Xbox");
                        Dirs.filesPathData = Dirs.games + @"\BFBC2-Xbox";
                    }
                    else if (row.Name == "Battlefield Bad Company" && row.Platform == "PS3")
                    {
                        Tree.Populate(Elements.TreeViewDataExplorer, Dirs.games + @"\BFBC-PS3");
                        Dirs.filesPathData = Dirs.games + @"\BFBC-PS3";
                    }
                    else if (row.Name == "Battlefield Bad Company" && row.Platform == "Xbox")
                    {
                        Tree.Populate(Elements.TreeViewDataExplorer, Dirs.games + @"\BFBC-Xbox");
                        Dirs.filesPathData = Dirs.games + @"\BFBC-Xbox";
                    }
                    else if (row.Name == "Battlefield 1943" && row.Platform == "PS3")
                    {
                        Tree.Populate(Elements.TreeViewDataExplorer, Dirs.games + @"\BF1943-PS3");
                        Dirs.filesPathData = Dirs.games + @"\BF1943-PS3";
                    }
                    else if (row.Name == "Battlefield 1943" && row.Platform == "Xbox")
                    {
                        Tree.Populate(Elements.TreeViewDataExplorer, Dirs.games + @"\BF1943-Xbox");
                        Dirs.filesPathData = Dirs.games + @"\BF1943-Xbox";
                    }

                    Vars.isGameProfile = true;

                    Close();
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to load game profile! See error.log", "error");

                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }   
}
