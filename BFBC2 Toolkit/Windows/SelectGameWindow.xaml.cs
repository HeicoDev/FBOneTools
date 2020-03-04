using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using MahApps.Metro.Controls;
using BFBC2_Toolkit.Data;
using BFBC2_Toolkit.Functions;

namespace BFBC2_Toolkit.Windows
{
    public partial class SelectGameWindow : MetroWindow
    {
        private TreeView treeViewDataExplorer;

        public SelectGameWindow(TreeView treeView)
        {
            InitializeComponent();

            treeViewDataExplorer = treeView;
        }

        private void SelectGameWindow_Loaded(object sender, RoutedEventArgs e)
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

            /*dataGrid.Items.Add(new Item() { Thumbnail = new Uri(Environment.CurrentDirectory +  @"\BFBC2Toolkit\Resources\Logos\bfbc2.png"), Name = "Battlefield: Bad Company 2", Platform = new Uri(Environment.CurrentDirectory + @"\BFBC2Toolkit\Resources\Logos\pc.png") });
            dataGrid.Items.Add(new Item() { Thumbnail = new Uri(Environment.CurrentDirectory +  @"\BFBC2Toolkit\Resources\Logos\bfbc2.png"), Name = "Battlefield: Bad Company 2", Platform = new Uri(Environment.CurrentDirectory + @"\BFBC2Toolkit\Resources\Logos\ps3.png") });
            dataGrid.Items.Add(new Item() { Thumbnail = new Uri(Environment.CurrentDirectory +  @"\BFBC2Toolkit\Resources\Logos\bfbc2.png"), Name = "Battlefield: Bad Company 2", Platform = new Uri(Environment.CurrentDirectory + @"\BFBC2Toolkit\Resources\Logos\xbox.png") });
            dataGrid.Items.Add(new Item() { Thumbnail = new Uri(Environment.CurrentDirectory +  @"\BFBC2Toolkit\Resources\Logos\bfbc2.png"), Name = "Battlefield: Bad Company 2 Server", Platform = new Uri(Environment.CurrentDirectory + @"\BFBC2Toolkit\Resources\Logos\pc.png") });
            dataGrid.Items.Add(new Item() { Thumbnail = new Uri(Environment.CurrentDirectory +  @"\BFBC2Toolkit\Resources\Logos\bfbc.png"), Name = "Battlefield: Bad Company", Platform = new Uri(Environment.CurrentDirectory + @"\BFBC2Toolkit\Resources\Logos\ps3.png") });
            dataGrid.Items.Add(new Item() { Thumbnail = new Uri(Environment.CurrentDirectory +  @"\BFBC2Toolkit\Resources\Logos\bfbc.png"), Name = "Battlefield: Bad Company", Platform = new Uri(Environment.CurrentDirectory + @"\BFBC2Toolkit\Resources\Logos\xbox.png") });
            dataGrid.Items.Add(new Item() { Thumbnail = new Uri(Environment.CurrentDirectory +  @"\BFBC2Toolkit\Resources\Logos\bf1943.png"), Name = "Battlefield 1943", Platform = new Uri(Environment.CurrentDirectory + @"\BFBC2Toolkit\Resources\Logos\ps3.png") });
            dataGrid.Items.Add(new Item() { Thumbnail = new Uri(Environment.CurrentDirectory +  @"\BFBC2Toolkit\Resources\Logos\bf1943.png"), Name = "Battlefield 1943", Platform = new Uri(Environment.CurrentDirectory + @"\BFBC2Toolkit\Resources\Logos\xbox.png") });*/
        }

        private async void BtnSelectGame_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(Dirs.filesPathData) && Vars.isGameProfile == false)
                await Task.Run(() => Directory.Delete(Dirs.filesPathData, true));

            if (dataGrid.SelectedItems != null)
            {
                var row = dataGrid.SelectedItem as GameProfile;

                if (row.Name == "Battlefield Bad Company 2" && row.Platform == "PC")
                {
                    Tree.Populate(treeViewDataExplorer, Dirs.games + @"\BFBC2-PC");
                    Dirs.filesPathData = Dirs.games + @"\BFBC2-PC";
                }
                else if (row.Name == "Battlefield Bad Company 2 Server" && row.Platform == "PC")
                {
                    Tree.Populate(treeViewDataExplorer, Dirs.games + @"\BFBC2-Server-PC");
                    Dirs.filesPathData = Dirs.games + @"\BFBC2-Server-PC";
                }
                else if (row.Name == "Battlefield Bad Company 2" && row.Platform == "PS3")
                {
                    Tree.Populate(treeViewDataExplorer, Dirs.games + @"\BFBC2-PS3");
                    Dirs.filesPathData = Dirs.games + @"\BFBC2-PS3";
                }
                else if (row.Name == "Battlefield Bad Company 2" && row.Platform == "Xbox")
                {
                    Tree.Populate(treeViewDataExplorer, Dirs.games + @"\BFBC2-Xbox");
                    Dirs.filesPathData = Dirs.games + @"\BFBC2-Xbox";
                }
                else if (row.Name == "Battlefield Bad Company" && row.Platform == "PS3")
                {
                    Tree.Populate(treeViewDataExplorer, Dirs.games + @"\BFBC-PS3");
                    Dirs.filesPathData = Dirs.games + @"\BFBC-PS3";
                }
                else if (row.Name == "Battlefield Bad Company" && row.Platform == "Xbox")
                {
                    Tree.Populate(treeViewDataExplorer, Dirs.games + @"\BFBC-Xbox");
                    Dirs.filesPathData = Dirs.games + @"\BFBC-Xbox";
                }
                else if (row.Name == "Battlefield 1943" && row.Platform == "PS3")
                {
                    Tree.Populate(treeViewDataExplorer, Dirs.games + @"\BF1943-PS3");
                    Dirs.filesPathData = Dirs.games + @"\BF1943-PS3";
                }
                else if (row.Name == "Battlefield 1943" && row.Platform == "Xbox")
                {
                    Tree.Populate(treeViewDataExplorer, Dirs.games + @"\BF1943-Xbox");
                    Dirs.filesPathData = Dirs.games + @"\BF1943-Xbox";
                }

                Vars.isGameProfile = true;

                Close();
            }
            
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class GameProfile
    {       
        public string Name { get; set; }
        public string Platform { get; set; }
    }
}
