using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using MahApps.Metro.Controls;
using BFBC2Toolkit.Tools;
using BFBC2Shared.Functions;

namespace BFBC2Toolkit.Windows
{
    public partial class FilePorterWindow : MetroWindow
    {
        public FilePorterWindow()
        {
            InitializeComponent();
        }

        private void TxtBoxDragAndDrop_PreviewDragOver(object sender, DragEventArgs e)
        {
            //Allow drag and drop handler of the textbox to handle all file formats
            e.Handled = true;
        }

        private async void TxtBoxDragAndDrop_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                await ConvertFiles(files);
            }
        }

        private async void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "All|*.terrainheightfield;*.watermesh;*.visualwater;*.terrainmaterialmap;*.visualterrain;*.ps3texture;*.xenontexture|Texture|*.ps3texture;*.xenontexture|Heightmap|*.terrainheightfield|Misc Terrain|*.terrainmaterialmap;*.visualterrain|Water|*.watermesh;*.visualwater";
            ofd.Title = "Open a file...";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == true)
            {
                await ConvertFiles(ofd.FileNames);
            }
        }

        private async Task ConvertFiles(string[] files)
        {
            int filesCountA = 1,
                filesCountB = await Task.Run(() => CountFiles(files));

            foreach (string file in files)
            {
                lblMain.Content = "Converting file " + filesCountA + " of " + filesCountB + "...";

                try
                {
                    await Task.Run(() => FilePorter.ConvertFile(file));
                }
                catch (Exception ex)
                {
                    lblMain.Content = "Unable to convert file! See error.log";
                    Log.Error(ex.ToString());
                }

                filesCountA++;
            }

            lblMain.Content = "Done!";
        }

        private int CountFiles(string[] files)
        {
            int filesCount = 0;

            foreach (string file in files)
                filesCount++;

            return filesCount;
        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("BFBC2 File Porter is able to port several Frostbite 1 files from console (PS3 & Xbox 360) to PC.\n\nNote: This tool is still WIP!\n\nSupported File Formats:\nterrainheightfield, watermesh, visualwater, ps3texture & xenontexture\n\nSupport for the remaining terrain related files will follow soon.", "Info (Placeholder)");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
