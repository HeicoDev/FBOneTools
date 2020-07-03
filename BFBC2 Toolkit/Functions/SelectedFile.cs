using System;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using BFBC2_Toolkit.Data;
using BFBC2_Toolkit.Tools;

namespace BFBC2_Toolkit.Functions
{
    public class SelectedFile
    {        
        public static async Task Export()
        {
            try
            {
                string selectedFilePath = "",
                       selectedFileName = "";

                if (Vars.isDataTreeView == true)
                {
                    selectedFilePath = Dirs.selectedFilePathData;
                    selectedFileName = Dirs.selectedFileNameData;
                }
                else
                {
                    selectedFilePath = Dirs.selectedFilePathMod;
                    selectedFileName = Dirs.selectedFileNameMod;
                }

                if (selectedFilePath.EndsWith(".dbx"))
                {
                    string path = selectedFilePath.Replace(".dbx", ".xml"),
                           name = selectedFileName.Replace(".dbx", ".xml");

                    if (File.Exists(Dirs.outputXML + @"\" + name))
                        await Task.Run(() => File.Delete(Dirs.outputXML + @"\" + name));

                    await Task.Run(() => File.Copy(path, Dirs.outputXML + @"\" + name));
                }
                else if (selectedFilePath.EndsWith(".itexture"))
                {
                    string path = selectedFilePath.Replace(".itexture", ".dds"),
                           name = selectedFileName.Replace(".itexture", ".dds");

                    if (File.Exists(Dirs.outputDDS + @"\" + name))
                        await Task.Run(() => File.Delete(Dirs.outputDDS + @"\" + name));

                    await Task.Run(() => File.Copy(path, Dirs.outputDDS + @"\" + name));
                }
                else if (selectedFilePath.EndsWith(".ps3texture"))
                {
                    string path = selectedFilePath.Replace(".ps3texture", ".dds"),
                           name = selectedFileName.Replace(".ps3texture", ".dds");

                    if (File.Exists(Dirs.outputDDS + @"\" + name))
                        await Task.Run(() => File.Delete(Dirs.outputDDS + @"\" + name));

                    await Task.Run(() => File.Copy(path, Dirs.outputDDS + @"\" + name));
                }
                else if (selectedFilePath.EndsWith(".terrainheightfield"))
                {
                    string path = selectedFilePath.Replace(".terrainheightfield", ".raw"),
                           name = selectedFileName.Replace(".terrainheightfield", ".raw");

                    if (File.Exists(Dirs.outputHeightmap + @"\" + name))
                        await Task.Run(() => File.Delete(Dirs.outputHeightmap + @"\" + name));

                    await Task.Run(() => File.Copy(path, Dirs.outputHeightmap + @"\" + name));
                }
                else if (selectedFilePath.EndsWith(".binkmemory"))
                {
                    string name = selectedFileName.Replace(".binkmemory", ".bik");

                    if (File.Exists(Dirs.outputVideo + @"\" + name))
                        await Task.Run(() => File.Delete(Dirs.outputVideo + @"\" + name));

                    await Task.Run(() => File.Copy(selectedFilePath, Dirs.outputVideo + @"\" + name));
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to export file! See error.log", "error");
            }
        }

        public static async Task Import()
        {
            try
            {
                await MediaStream.Dispose();

                string selectedFilePath = "";

                if (Vars.isDataTreeView == true)
                    selectedFilePath = Dirs.selectedFilePathData;
                else
                    selectedFilePath = Dirs.selectedFilePathMod;

                if (selectedFilePath.EndsWith(".dbx"))
                {
                    var ofd = new OpenFileDialog();
                    ofd.Filter = "xml file (.xml)|*.xml";
                    ofd.Title = "Select xml file...";

                    if (ofd.ShowDialog() == true)
                    {
                        Write.ToEventLog("Importing file...", "");

                        string path = selectedFilePath.Replace(".dbx", ".xml");

                        if (File.Exists(path))
                            await Task.Run(() => File.Delete(path));

                        await Task.Run(() => File.Copy(ofd.FileName, path));

                        var process = Process.Start(Dirs.scriptDBX, "\"" + path.Replace(@"\", @"\\"));
                        await Task.Run(() => process.WaitForExit());

                        Elements.TextEditor.Text = await Task.Run(() => File.ReadAllText(path));
                        Elements.TextEditor.ScrollToHome();

                        Write.ToEventLog("", "done");
                    }
                }
                else if (selectedFilePath.EndsWith(".itexture"))
                {
                    var ofd = new OpenFileDialog();
                    ofd.Filter = "dds file (.dds)|*.dds";
                    ofd.Title = "Select dds file...";

                    if (ofd.ShowDialog() == true)
                    {
                        Write.ToEventLog("Importing file...", "");

                        string path = selectedFilePath.Replace(".itexture", ".dds");

                        if (File.Exists(path))
                            await Task.Run(() => File.Delete(path));

                        await Task.Run(() => File.Copy(ofd.FileName, path));

                        string[] file = { path };

                        await Task.Run(() => TextureConverter.ConvertFile(file, false));

                        try
                        {
                            var bitmap = new BitmapImage();
                            MediaStream.stream = new FileStream(path, FileMode.Open);

                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.None;
                            bitmap.StreamSource = MediaStream.stream;
                            bitmap.EndInit();

                            bitmap.Freeze();
                            Elements.ImageElement.Source = bitmap;
                        }
                        catch
                        {
                            Elements.ImageElement.Visibility = Visibility.Hidden;

                            Write.ToEventLog("Unable to load texture preview! Exporting and importing should still work fine.", "warning");
                        }

                        Write.ToEventLog("", "done");
                    }
                }
                else if (selectedFilePath.EndsWith(".bik"))
                {
                    var ofd = new OpenFileDialog();
                    ofd.Filter = "bik file (.bik)|*.bik";
                    ofd.Title = "Select bik file...";

                    if (ofd.ShowDialog() == true)
                    {
                        Write.ToEventLog("Importing file...", "");

                        await RenameToBik();

                        string path = selectedFilePath.Replace(".binkmemory", ".mp4");

                        if (File.Exists(path))
                            await Task.Run(() => File.Delete(path));

                        if (File.Exists(selectedFilePath.Replace(".binkmemory", ".bik")))
                            await Task.Run(() => File.Delete(selectedFilePath.Replace(".binkmemory", ".bik")));

                        await Task.Run(() => File.Copy(ofd.FileName, path));

                        if (File.Exists(selectedFilePath))
                            await Task.Run(() => File.Delete(selectedFilePath));

                        await Task.Run(() => File.Copy(ofd.FileName, selectedFilePath));

                        Play.Video(path);

                        Write.ToEventLog("", "done");
                    }
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to import file! See error.log", "error");
            }
        }

        public static async Task DeleteFile(string explorer)
        {
            try
            {
                if (explorer == "data")
                {
                    if (Vars.isGameProfile == false)
                    {
                        if (Elements.TreeViewDataExplorer.SelectedItem != null)
                        {
                            await Task.Run(() => File.Delete(Dirs.selectedFilePathData));

                            var item = Elements.TreeViewDataExplorer.SelectedItem as CustomTreeViewItem;
                            var parent = item.ParentItem;

                            parent.Items.Remove(item);
                        }
                    }
                    else
                    {
                        Write.ToEventLog("You can't delete a file from a game profile!", "warning");
                    }
                }
                else if (explorer == "mod")
                {
                    if (Elements.TreeViewModExplorer.SelectedItem != null)
                    {
                        await Task.Run(() => File.Delete(Dirs.selectedFilePathMod));

                        var item = Elements.TreeViewModExplorer.SelectedItem as CustomTreeViewItem;
                        var parent = item.ParentItem;

                        parent.Items.Remove(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to delete file! See error.log", "error");
            }
        }

        public static async Task RestoreFile()
        {
            try
            {
                await Task.Run(() => CleanUp.FilesAndDirs(Dirs.filesPathMod));

                string filePathData = "";

                if (Vars.isGameProfile == false)
                {
                    foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                    {
                        if (Dirs.filePath.Contains(kvp.Value))
                        {
                            filePathData = Dirs.filePath.Replace(kvp.Value + @"\", "");
                            break;
                        }
                    }
                }
                else
                {
                    filePathData = Dirs.filePath;
                }

                filePathData = Dirs.filesPathData + filePathData;

                if (File.Exists(filePathData))
                {
                    if (File.Exists(Dirs.selectedFilePathMod))
                        await Task.Run(() => File.Delete(Dirs.selectedFilePathMod));

                    await Task.Run(() => File.Copy(filePathData, Dirs.selectedFilePathMod));
                }
                else
                {
                    MessageBox.Show("Unable to locate the original file!\nOpen the correct game profile or fbrb archive.", "Error!");
                }

                var item = Elements.TreeViewModExplorer.SelectedItem as CustomTreeViewItem;

                item.IsSelected = false;
                item.IsSelected = true;
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to restore file! See error.log", "error");
            }
        }

        public static async Task CopyToMod()
        {
            try
            {
                await MediaStream.Dispose();

                await Task.Run(() => CleanUp.FilesAndDirs(Dirs.filesPathMod));

                string filePathMod = Dirs.filesPathMod + @"\" + Dirs.filePath;

                if (Vars.isGameProfile == false)
                {
                    foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                    {
                        if (Dirs.filesPathData.EndsWith(kvp.Value))
                        {
                            filePathMod = Dirs.filesPathMod + @"\" + kvp.Value + @"\" + Dirs.filePath;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (string gameId in Vars.gameIds)
                    {
                        if (filePathMod.Contains(gameId))
                        {
                            filePathMod = filePathMod.Replace(gameId + @"\", "");
                            break;
                        }
                    }
                }

                if (File.Exists(filePathMod))
                    await Task.Run(() => File.Delete(filePathMod));

                var fileInfo = new FileInfo(filePathMod);
                await Task.Run(() => fileInfo.Directory.Create());

                await Task.Run(() => File.Copy(Dirs.selectedFilePathData, filePathMod));

                Tree.Populate(Elements.TreeViewModExplorer, Dirs.filesPathMod);
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to copy file to mod project! See error.log", "error");
            }
        }

        public static async Task RenameToBik()
        {
            try
            {
                string selectedFilePath = "",
                       selectedFileName = "";

                Elements.MediaElement.Stop();
                Elements.MediaElement.Close();
                Elements.MediaElement.Source = null;

                if (Vars.isDataTreeView == true)
                {
                    selectedFilePath = Dirs.selectedFilePathData;
                    selectedFileName = Dirs.selectedFileNameData;
                }
                else
                {
                    selectedFilePath = Dirs.selectedFilePathMod;
                    selectedFileName = Dirs.selectedFileNameMod;
                }

                await Task.Run(() => GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true));

                await Task.Delay(500);

                if (File.Exists(selectedFilePath.Replace(".binkmemory", ".mp4")))
                    await Task.Run(() => FileSystem.RenameFile(selectedFilePath.Replace(".binkmemory", ".mp4"), selectedFileName.Replace(".binkmemory", ".bik")));
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to rename file! See error.log", "error");
            }
        }

        public static void OpenFileLocation()
        {
            try
            {
                string selectedFilePath = "";

                if (Vars.isDataTreeView == true)
                    selectedFilePath = Dirs.selectedFilePathData;
                else
                    selectedFilePath = Dirs.selectedFilePathMod;

                Process.Start("explorer.exe", "/select, " + selectedFilePath);
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to open file location! See error.log", "error");
            }
        }
    }
}
