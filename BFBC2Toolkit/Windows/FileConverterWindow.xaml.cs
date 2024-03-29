﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using MahApps.Metro.Controls;
using BFBC2Toolkit.Data;
using BFBC2Toolkit.Tools;
using BFBC2Shared.Functions;
using BFBC2Shared.Data;

namespace BFBC2Toolkit.Windows
{
    public partial class FileConverterWindow : MetroWindow
    {
        private int skippedFiles = 0;

        private bool copyToOutputEnabled = false;

        public FileConverterWindow()
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
            ofd.Filter = "All|*.dds;*.itexture;*.ps3texture;*.xenontexture;*.terrainheightfield;*.dbx;*.xml;*.binkmemory;*.bik|Texture|*.itexture;*.ps3texture;*.xenontexture;*.dds|Heightmap|*.terrainheightfield|Text|*.dbx;*.xml|Video|*.binkmemory;*.bik";
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
                filesCountB = 0;

            filesCountB = await Task.Run(() => CountFiles(files));

            foreach (string file in files)
            {
                lblMain.Content = "Converting file " + filesCountA + " of " + filesCountB + "...";

                await ConvertFile(file);

                filesCountA++;
            }

            lblMain.Content = "Done!";

            MessageBox.Show("Done! Converted: " + (filesCountB - skippedFiles) + " Skipped: " + skippedFiles + " (Not supported yet)", "Result");

            filesCountA = 1;
            skippedFiles = 0;
        }

        private async Task ConvertFile(string filePath)
        {          
            try
            {
                if (filePath.EndsWith(".itexture") || filePath.EndsWith(".ps3texture") || filePath.EndsWith(".xenontexture") || filePath.EndsWith(".dds") || filePath.EndsWith(".terrainheightfield"))
                {
                    string[] file = { filePath };

                    await Task.Run(() => TextureConverter.ConvertFile(file, copyToOutputEnabled, true, "")); //TODO: add option to convert to .ps3texture and .xenontexture
                }
                else if (filePath.EndsWith(".dbx") || filePath.EndsWith(".xml"))
                {
                    await Python.ExecuteScript(Dirs.ScriptDBX, filePath);

                    if (copyToOutputEnabled)
                    {
                        string targetFileName = String.Empty;

                        if (filePath.EndsWith(".dbx"))
                        {
                            filePath = filePath.Replace(".dbx", ".xml");
                            targetFileName = Path.GetFileName(filePath.Replace(".dbx", ".xml"));
                        }
                        else
                        {
                            filePath = filePath.Replace(".xml", ".dbx");
                            targetFileName = Path.GetFileName(filePath.Replace(".xml", ".dbx"));
                        }

                        string targetFilePath = Dirs.OutputXML + @"\" + targetFileName;

                        if (File.Exists(targetFilePath))
                            await Task.Run(() => File.Delete(targetFilePath));

                        await Task.Run(() => File.Copy(filePath, targetFilePath));
                    }
                }                
                else if (filePath.EndsWith(".binkmemory") || filePath.EndsWith(".bik"))
                {
                    string targetFilePath = filePath.Replace(".binkmemory", ".bik");

                    if (filePath.EndsWith(".bik"))
                        targetFilePath = filePath.Replace(".bik", ".binkmemory");

                    string fileName = Path.GetFileName(targetFilePath);

                    if (copyToOutputEnabled)
                        targetFilePath = Dirs.OutputVideo + @"\" + fileName;

                    if (File.Exists(targetFilePath))
                        await Task.Run(() => File.Delete(targetFilePath));

                    await Task.Run(() => File.Copy(filePath, targetFilePath));
                }
                else if (filePath.EndsWith(".swfmovie"))
                {
                    string destFilePath = Path.GetDirectoryName(filePath);

                    if (copyToOutputEnabled)
                        destFilePath = Dirs.OutputSwfMovie;

                    await Python.ExecuteScript(Dirs.ScriptSwfMovie, filePath, destFilePath);
                }
                else
                {
                    skippedFiles++;
                }
            }
            catch (Exception ex)
            {
                skippedFiles++;

                lblMain.Content = "Unable to convert file! See error.log";
                Log.Error(ex.ToString());               
            }
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
            MessageBox.Show("BFBC2 File Converter lets you convert several Frostbite 1 files.\n\nSupported File Formats:\ndbx, xml, itexture, ps3texture, xenontexture, dds, terrainheightfield,\nbinkmemory & bik", "Info (Placeholder)");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOpenOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                Process.Start("explorer.exe", Dirs.Output);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                lblMain.Content = "Unable to open folder! See error.log";
            }
        }

        private void ChkBoxCopyToOutput_Checked(object sender, RoutedEventArgs e)
        {
            copyToOutputEnabled = true;
        }

        private void ChkBoxCopyToOutput_Unchecked(object sender, RoutedEventArgs e)
        {
            copyToOutputEnabled = false;
        }
    }
}
