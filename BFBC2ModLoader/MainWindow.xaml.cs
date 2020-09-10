using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using Microsoft.Win32;
using MahApps.Metro.Controls;
using BFBC2ModLoader.Functions;
using BFBC2ModLoader.Data;
using BFBC2ModLoader.Data.Bindings;
using BFBC2ModLoader.Windows;
using BFBC2Shared.Functions;
using BFBC2Shared.Data;

/// <summary>
/// BFBC2 Mod Loader
/// By Nico Hellmund 
/// Aka Heico
/// Copyright 2020
/// </summary>

namespace BFBC2ModLoader
{
    public partial class MainWindow : MetroWindow
    {
        private Stream mediaStream;

        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += (sender, arguments) =>
            {
                #if DEBUG
                MessageBox.Show("Unhandled exception: " + arguments.ExceptionObject);
                #endif

                Log.Write("An unhandled exception has occurred! See error.log", "error");
                Log.Error("Unhandled exception: " + arguments.ExceptionObject);
            };
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeStartup();            
        }

        private async void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (btnApply.IsEnabled == true)
            {
                var result = MessageBox.Show("There are unsaved changes! Do you want to save them?", "Save changes?", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    await Install.Mods();
                    Misc.OrderNumber();
                    Save.ModsXML();
                }
            }
        }

        private async Task InitializeStartup()
        {
            Opacity = 0;

            var windowStartup = new WindowStartup();
            windowStartup.Owner = this;
            windowStartup.Show();            

            UIElements.SetElements(txtBoxEventLog, txtBoxModInfo, txtBoxServerInfo, txtBoxMapInfo, dataGridModManager, dataGridServerBrowser, dataGridMapBrowser);            
            Globals.SetSharedVars();
            Dirs.SetSharedVars();
            Dirs.SetFbrbDirsAndFiles();

            Log.Write("Click 'Install Mod' to select the mod you want to install.");

            if (File.Exists(Environment.CurrentDirectory + @"\BFBC2Game.exe"))
            {
                Title += " (Client)";
                Globals.IsClient = true;
            }
            else if (File.Exists(Environment.CurrentDirectory + @"\Frost.Game.Main_Win32_Final.exe"))
            {
                Title += " (Server)";
                Globals.IsClient = false;
                Dirs.SwitchToServer();
            }
            else
            {
                EnableAllButtons(false);
               
                btnStartGame.IsEnabled = false;

                EnableAllTabs("ModManager", false);

                frmModsDisabled.Visibility = Visibility.Hidden;
                lblModsDisabled.Visibility = Visibility.Hidden;

                Log.Write("BFBC2 Mod Loader is not installed correctly!", "warning");

                windowStartup.Close();
                Opacity = 100;
                Activate();

                return;
            }
            
            Delete.OldVersion();
            Delete.LogFiles();
            Create.PrecreateDirs();            
            Create.ConfigFiles();
            Load.ConfigXML();
            Load.UpdateXML();
            Load.NewsXML();
            Load.ModsXML();
            Load.MapsXML();            

            if (!File.Exists(SharedSettings.PathToPython))
            {
                MessageBox.Show("Unable to locate Python 2.7 installation!\nPlease select pythonw.exe...", "Error");

                string path = Python.ChangePath();

                if (path == String.Empty)
                {
                    MessageBox.Show("Unable to locate pythonw.exe!\nPress 'OK' to close the app.", "Error");

                    Environment.Exit(0);
                }
                else
                {
                    bool isCorrectPythonVersion = Python.CheckVersion();

                    if (!isCorrectPythonVersion)
                    {
                        MessageBox.Show("Incorrect version of Python detected!\nIt must be version 2.7!\nPress 'OK' to close the app.", "Error");

                        Environment.Exit(0);
                    }

                    bool hasErrorOccurredOnSave = Save.ConfigXML();

                    if (hasErrorOccurredOnSave)
                    {
                        MessageBox.Show("Unable to save settings.config! See error.log\nPress 'OK' to close the app.", "Error");

                        Environment.Exit(0);
                    }
                }
            }

            bool hasErrorOccurredOnLoad = Load.ConfigXML();

            if (hasErrorOccurredOnLoad)
            {
                MessageBox.Show("Unable to load settings.config! See error.log\nPress 'OK' to close the app.", "Error");

                Environment.Exit(0);
            }         

            txtBoxPathToPython.Text = SharedSettings.PathToPython;
            chkBoxAutoCheckUpdates.IsChecked = Settings.IsAutoUpdateCheckEnabled;

            await Load.ServersXML();

            if (Settings.IsAutoUpdateCheckEnabled)
            {
                Check.Update();
                Check.MapUpdate();
            }

            RenderOptions.SetBitmapScalingMode(imgBoxModManager, BitmapScalingMode.HighQuality);
            RenderOptions.SetBitmapScalingMode(imgBoxServerBrowser, BitmapScalingMode.HighQuality);
            RenderOptions.SetBitmapScalingMode(imgBoxMapBrowser, BitmapScalingMode.HighQuality);

            ShowLogo("");

            SetupButtons();

            if (Settings.ModsEnabled == false)
            {
                dataGridModManager.IsEnabled = false;

                EnableAllButtons(false);

                btnEnableMods.IsEnabled = true;
                btnStartGame.IsEnabled = true;

                EnableAllTabs("ModManager", false);
            }
            else
            {
                btnEnableMods.IsEnabled = false;

                frmModsDisabled.Visibility = Visibility.Hidden;
                lblModsDisabled.Visibility = Visibility.Hidden;
            }

            windowStartup.Close();
            Opacity = 100;
            Activate();
        }

        private async void BtnInstallMod_Click(object sender, RoutedEventArgs e)
        {
            //Choose mod (zip)
            var ofd = new OpenFileDialog();
            ofd.Filter = "zip file (.zip)|*.zip";
            ofd.Title = "Select mod...";

            Log.Write("Installing mod, please wait...");

            EnableAllButtons(false);
            EnableAllTabs("ModManager", false);

            dataGridModManager.IsEnabled = false;
            progressRingMM.IsActive = true;

            if (ofd.ShowDialog() == true)
                await Install.Mod(ofd.FileName, true);
            else
                Log.Write("Click 'Install Mod' to select the mod you want to install.");                                   

            EnableAllButtons(true);
            EnableAllTabs("", true);

            dataGridModManager.IsEnabled = true;
            progressRingMM.IsActive = false;

            Log.Write("", "done");
        }       

        private void BtnEnableMods_Click(object sender, RoutedEventArgs e)
        {
            //Enable mods
            try
            {
                if (File.Exists(Dirs.MftRoot))
                    File.Delete(Dirs.MftRoot);
                File.Copy(Dirs.MftModded, Dirs.MftRoot);
                if (File.Exists(Dirs.BundleManiRoot))
                    File.Delete(Dirs.BundleManiRoot);
                File.Copy(Dirs.BundleManiModded, Dirs.BundleManiRoot);

                Settings.ModsEnabled = true;

                frmModsDisabled.Visibility = Visibility.Hidden;
                lblModsDisabled.Visibility = Visibility.Hidden;

                dataGridModManager.IsEnabled = true;

                Save.ConfigXML();

                EnableAllButtons(true);
                EnableAllTabs("", true);

                Log.Write("Enabled all mods.");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not enable mods! See error.log", "error");
            }
        }

        private void BtnDisableMods_Click(object sender, RoutedEventArgs e)
        {
            //Disable mods
            try
            {
                if (File.Exists(Dirs.MftRoot))
                    File.Delete(Dirs.MftRoot);
                File.Copy(Dirs.MftOriginal, Dirs.MftRoot);
                if (File.Exists(Dirs.BundleManiRoot))
                    File.Delete(Dirs.BundleManiRoot);
                File.Copy(Dirs.BundleManiOriginal, Dirs.BundleManiRoot);

                Settings.ModsEnabled = false;

                dataGridModManager.IsEnabled = false;

                frmModsDisabled.Visibility = Visibility.Visible;
                lblModsDisabled.Visibility = Visibility.Visible;

                Save.ConfigXML();

                EnableAllButtons(false);

                btnEnableMods.IsEnabled = true;
                btnStartGame.IsEnabled = true;

                EnableAllTabs("ModManager", false);

                Log.Write("Disabled all mods.");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not disable mods! See error.log", "error");
            }
        }

        private void BtnDelAllMods_Click(object sender, RoutedEventArgs e)
        {
            //Delete mods
            try
            {
                Log.Write("Deleting mods...");

                var result = MessageBox.Show("Do you really want to delete ALL mods?", "Delete all mods?", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    EnableAllButtons(false);

                    Delete.Mods();

                    EnableAllButtons(true);

                    Log.Write("All mods deleted.", "done");
                }
                else
                {
                    Log.Write("Aborted!", "done");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to open message box! See error.log", "error");
            }
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            //Up
            try
            {
                EnableAllButtons(false);

                btnUp.IsEnabled = true;
                btnDown.IsEnabled = true;
                btnReset.IsEnabled = true;
                btnApply.IsEnabled = true;
                btnDelMod.IsEnabled = false;

                EnableAllTabs("ModManager", false);

                int totalRows = dataGridModManager.Items.Count;
                int rowIndex = dataGridModManager.SelectedIndex;
                if (rowIndex == 0)
                    return;
                var selectedRow = dataGridModManager.Items[rowIndex];
                dataGridModManager.Items.Remove(selectedRow);
                dataGridModManager.Items.Insert(rowIndex - 1, selectedRow);
                dataGridModManager.SelectedItem = null;
                dataGridModManager.SelectedItem = dataGridModManager.Items[rowIndex - 1];

                if (Globals.HasModMoved == false)
                    Log.Write("Moved mod up or down. Don't forget to apply changes!", "warning");

                Globals.HasModMoved = true;
            }
            catch
            {
                Log.Write("Could not move mod up! No mod is selected or installed?", "warning");
            }
        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            //Down
            try
            {
                EnableAllButtons(false);

                btnUp.IsEnabled = true;
                btnDown.IsEnabled = true;
                btnReset.IsEnabled = true;
                btnApply.IsEnabled = true;
                btnDelMod.IsEnabled = false;

                EnableAllTabs("ModManager", false);

                int totalRows = dataGridModManager.Items.Count;
                int rowIndex = dataGridModManager.SelectedIndex;
                if (rowIndex == totalRows - 1)
                    return;
                var selectedRow = dataGridModManager.Items[rowIndex];
                dataGridModManager.Items.Remove(selectedRow);
                dataGridModManager.Items.Insert(rowIndex + 1, selectedRow);
                dataGridModManager.SelectedItem = null;
                dataGridModManager.SelectedItem = dataGridModManager.Items[rowIndex + 1];

                if (Globals.HasModMoved == false)
                    Log.Write("Moved mod up or down. Don't forget to apply changes!", "warning");

                Globals.HasModMoved = true;
            }
            catch
            {
                Log.Write("Could not move mod down! No mod is selected or installed?", "warning");
            }
        }

        private async void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            //Apply
            try
            {
                Log.Write("This may take a while. Do NOT close the application!");
                Log.Write("Applying changes, please wait...");

                EnableAllButtons(false);

                dataGridModManager.IsEnabled = false;
                progressRingMM.IsActive = true;

                await Install.Mods();
                Misc.OrderNumber();
                Save.ModsXML();

                EnableAllButtons(true);
                EnableAllTabs("", true);

                dataGridModManager.IsEnabled = true;
                progressRingMM.IsActive = false;

                Globals.HasModMoved = false;
                Globals.HasModBeenUnChecked = false;

                Log.Write("Changes have been applied.", "done");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not apply changes! See error.log", "error");
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            //Reset
            try
            {
                Log.Write("Resetting changes, please wait...");

                EnableAllButtons(false);

                Load.ModsXML();

                EnableAllButtons(true);
                EnableAllTabs("", true);

                Globals.HasModMoved = false;
                Globals.HasModBeenUnChecked = false;

                Log.Write("Changes have been reset.", "done");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not reset changes! See error.log", "error");
            }
        }

        private async void BtnDelMod_Click(object sender, RoutedEventArgs e)
        {
            //Delete selected mod
            try
            {
                EnableAllButtons(false);
                EnableAllTabs("ModManager", false);

                dataGridModManager.IsEnabled = false;
                progressRingMM.IsActive = true;

                if (dataGridModManager.SelectedItems.Count > 0)
                {
                    if (dataGridModManager.Items.Count < 2)
                    {
                        Delete.Mods();
                    }
                    else
                    {
                        Log.Write("This may take a while. Do NOT close the application!");
                        Log.Write("Deleting mod, please wait...");

                        var item = dataGridModManager.SelectedItem as ModManagerItem;
                        var item1 = dataGridMapBrowser.SelectedItem as MapBrowserItem;

                        string modName = item.ModName;
                        string modType = item.ModType;
                        var mapInfo = new IniFile(Dirs.ModsFolder + "\\" + modName + @"\ModInfo.ini");
                        string mapName = mapInfo.Read("MapID", "ModInfo");

                        dataGridModManager.Items.RemoveAt(dataGridModManager.Items.IndexOf(item));
                        if (Directory.Exists(Dirs.ModsFolder + "\\" + modName))
                            Directory.Delete(Dirs.ModsFolder + "\\" + modName, true);

                        if (modType.Contains("map"))
                        {
                            if (Directory.Exists(Dirs.LevelsPathPackage + "\\" + mapName))
                                Directory.Delete(Dirs.LevelsPathPackage + "\\" + mapName, true);
                            if (Directory.Exists(Dirs.LevelsPathDist + "\\" + mapName))
                                Directory.Delete(Dirs.LevelsPathDist + "\\" + mapName, true);
                        }
                        else
                            await Install.Mods();

                        Misc.OrderNumber();
                        Save.ModsXML();

                        if (item1 != null)
                        {
                            if (modName == item1.MapName)
                            {
                                btnInstallMap.IsEnabled = true;
                            }
                        }

                        Log.Write("Mod deleted.", "done");
                    }
                }
                else
                {
                    Log.Write("No mod is selected.", "warning");
                }

                EnableAllButtons(true);
                EnableAllTabs("", true);

                dataGridModManager.IsEnabled = true;
                progressRingMM.IsActive = false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to delete selected mod! See error.log", "error");
            }
        }

        private async void BtnInstallMods_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Globals.IsClient == true)
                {
                    var itemSB = dataGridServerBrowser.SelectedItem as ServerBrowserItem;

                    if (!itemSB.ServerReq.Contains("None"))
                    {
                        EnableAllButtons(false);
                        EnableAllTabs("ServerBrowser", false);

                        dataGridServerBrowser.IsEnabled = false;
                        progressRingSB.IsActive = true;

                        int aCount = 0;
                        int bCount = 0;

                        foreach (var item in dataGridMapBrowser.Items)
                        {
                            var itemMB = item as MapBrowserItem;

                            bool a = true;

                            if (itemSB.ServerReq.Contains(itemMB.MapName))
                            {
                                foreach (var item1 in dataGridModManager.Items)
                                {
                                    var itemMM = item1 as ModManagerItem;

                                    if (itemMB.MapName.Contains(itemMM.ModName))
                                    {
                                        a = true;
                                        break;
                                    }
                                    else
                                    {
                                        a = false;
                                    }
                                }
                                if (dataGridModManager.Items.Count < 1)
                                {
                                    a = false;
                                }
                            }
                            if (a == false)
                            {
                                aCount++;
                            }
                        }

                        foreach (var item in dataGridMapBrowser.Items)
                        {
                            var itemMB = item as MapBrowserItem;

                            bool b = true;

                            if (itemSB.ServerReq.Contains(itemMB.MapName))
                            {
                                foreach (var item1 in dataGridModManager.Items)
                                {
                                    var itemMM = item1 as ModManagerItem;

                                    if (itemMB.MapName.Contains(itemMM.ModName))
                                    {
                                        b = true;
                                        break;
                                    }
                                    else
                                    {
                                        b = false;
                                    }
                                }
                                if (dataGridModManager.Items.Count < 1)
                                {
                                    b = false;
                                }
                            }
                            if (b == false)
                            {
                                bCount++;

                                Log.Write("This may take a while. Do NOT close the application!");
                                Log.Write("Downloading mod " + bCount + " of " + aCount + ", please wait...");

                                string reqLink = itemMB.MapLink;

                                await Task.Run(() => FileDownloader.DownloadFileFromURLToPath(reqLink, Dirs.MapZIP));

                                Log.Write("This may take a while. Do NOT close the application!");
                                Log.Write("Installing mod " + bCount + " of " + aCount + ", please wait...");

                                await Install.Mod(Dirs.MapZIP, false);
                            }
                        }

                        EnableAllButtons(true);
                        EnableInstallMapButton();
                        EnableAllTabs("", true);

                        dataGridServerBrowser.IsEnabled = true;
                        progressRingSB.IsActive = false;

                        MessageBox.Show("All files were successfully downloaded and installed! If you suddenly can not join the server in the future anymore, make sure to come back, select the server and press 'Install Maps' again, maybe new mods have been installed for the server. Also do not forget to update the mods if updates are available. Then you should be able to join the server again.", "Installation done!", MessageBoxButton.OK);

                        Log.Write("", "done");
                    }
                    else
                    {
                        MessageBox.Show("No mods are required for this server.", "No mods required!", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show("This function is not available for servers!", "Server detected!", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not download or install mod! See error.log", "error");
            }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Log.Write("Refreshing server list...");

            EnableAllButtons(false);
            EnableAllTabs("ServerBrowser", false);

            dataGridServerBrowser.IsEnabled = false;
            progressRingSB.IsActive = true;

            await Load.ServersXML();

            EnableAllButtons(true);
            EnableAllTabs("", true);

            dataGridServerBrowser.IsEnabled = true;
            progressRingSB.IsActive = false;

            Log.Write("", "done");
        }

        private async void BtnInstallMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnableAllButtons(false);
                EnableAllTabs("MapBrowser", false);

                dataGridMapBrowser.IsEnabled = false;
                progressRingMB.IsActive = true;

                int aCount = 0;
                int bCount = 0;

                foreach (var item in dataGridMapBrowser.Items)
                {
                    var itemMB = item as MapBrowserItem;
                    var itemMBSel = dataGridMapBrowser.SelectedItem as MapBrowserItem;

                    bool a = true;

                    if (itemMBSel.MapReq.Contains(itemMB.MapName))
                    {
                        foreach (var item1 in dataGridModManager.Items)
                        {
                            var itemMM = item1 as ModManagerItem;

                            if (itemMB.MapName.Contains(itemMM.ModName))
                            {
                                a = true;
                                break;
                            }
                            else
                            {
                                a = false;
                            }
                        }

                        if (dataGridModManager.Items.Count < 1)
                        {
                            a = false;
                        }
                    }
                    if (a == false)
                    {
                        aCount++;
                    }
                }

                foreach (var item in dataGridMapBrowser.Items)
                {
                    var itemMB = item as MapBrowserItem;
                    var itemMBSel = dataGridMapBrowser.SelectedItem as MapBrowserItem;

                    bool b = true;

                    if (itemMBSel.MapReq.Contains(itemMB.MapName))
                    {
                        foreach (var item1 in dataGridModManager.Items)
                        {
                            var itemMM = item1 as ModManagerItem;

                            if (itemMB.MapName.Contains(itemMM.ModName))
                            {
                                b = true;
                                break;
                            }
                            else
                            {
                                b = false;
                            }
                        }
                        if (dataGridModManager.Items.Count < 1)
                        {
                            b = false;
                        }
                    }
                    if (b == false)
                    {
                        bCount++;

                        Log.Write("This may take a while. Do NOT close the application!");
                        Log.Write("Downloading resource " + bCount + " of " + aCount + ", please wait...");

                        string reqLink = itemMB.MapLink;

                        await Task.Run(() => FileDownloader.DownloadFileFromURLToPath(reqLink, Dirs.MapZIP));

                        Log.Write("This may take a while. Do NOT close the application!");
                        Log.Write("Installing resource " + bCount + " of " + aCount + ", please wait...");

                        await Install.Mod(Dirs.MapZIP, false);
                    }
                }
                Log.Write("This may take a while. Do NOT close the application!");
                Log.Write("Downloading map, please wait...");

                var itemMBSelected = dataGridMapBrowser.SelectedItem as MapBrowserItem;

                string link = itemMBSelected.MapLink;

                await Task.Run(() => FileDownloader.DownloadFileFromURLToPath(link, Dirs.MapZIP));

                Log.Write("This may take a while. Do NOT close the application!");
                Log.Write("Installing map, please wait...");

                await Install.Mod(Dirs.MapZIP, false);

                EnableAllButtons(true);
                EnableAllTabs("", true);

                dataGridMapBrowser.IsEnabled = true;
                progressRingMB.IsActive = false;

                Log.Write("", "done");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not download or install map! See error.log", "error");
            }
        }

        private async void BtnUpdateMaps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EnableAllButtons(false);
                EnableAllTabs("MapBrowser", false);

                dataGridMapBrowser.IsEnabled = false;
                progressRingMB.IsActive = true;

                int aCount = 0;
                int bCount = 0;

                foreach (var item in dataGridModManager.Items)
                {
                    foreach (var item1 in dataGridMapBrowser.Items)
                    {
                        var itemMM = item as ModManagerItem;
                        var itemMB = item1 as MapBrowserItem;

                        if (itemMM.ModName == itemMB.MapName)
                        {
                            if (itemMM.ModVersion != itemMB.MapVersion)
                            {
                                aCount++;
                            }
                        }
                    }
                }

                foreach (var item in dataGridModManager.Items)
                {
                    foreach (var item1 in dataGridMapBrowser.Items)
                    {
                        var itemMM = item as ModManagerItem;
                        var itemMB = item1 as MapBrowserItem;

                        if (itemMM.ModName == itemMB.MapName)
                        {
                            if (itemMM.ModVersion != itemMB.MapVersion)
                            {
                                bCount++;

                                Log.Write("This may take a while. Do NOT close the application.");
                                Log.Write("Downloading map " + bCount + " of " + aCount + ", please wait...");

                                string link = itemMB.MapLink;

                                await Task.Run(() => FileDownloader.DownloadFileFromURLToPath(link, Dirs.MapZIP));

                                Log.Write("This may take a while. Do NOT close the application.");
                                Log.Write("Installing map " + bCount + " of " + aCount + ", please wait...");

                                await Install.Mod(Dirs.MapZIP, false);
                            }
                        }
                    }
                }

                EnableAllButtons(true);
                EnableAllTabs("", true);

                dataGridMapBrowser.IsEnabled = true;
                progressRingMB.IsActive = false;

                Globals.MapUpdatesAvailable = false;

                Log.Write("", "done");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not download or install map! See error.log", "error");
            }
        }       

        private void BtnSelectPathToPython_Click(object sender, RoutedEventArgs e)
        {
            string path = Python.ChangePath();

            if (path == String.Empty)
            {
                MessageBox.Show("Unable to locate pythonw.exe!", "Error");
            }
            else
            {
                bool isCorrectPythonVersion = Python.CheckVersion();

                if (isCorrectPythonVersion)
                {
                    txtBoxPathToPython.Text = path;

                    Save.ConfigXML();
                }
                else
                {
                    MessageBox.Show("Incorrect version of Python detected!\nIt must be version 2.7!", "Error");
                }
            }
        }

        private void BtnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            Log.Write("Checking for updates...");

            Check.Update();
            Check.MapUpdate();

            Log.Write("", "done");
        }

        private void ChkBoxAutoCheckUpdates_Checked(object sender, RoutedEventArgs e)
        {
            Settings.IsAutoUpdateCheckEnabled = true;

            Save.ConfigXML();
        }

        private void ChkBoxAutoCheckUpdates_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.IsAutoUpdateCheckEnabled = false;

            Save.ConfigXML();
        }        

        private void BtnStartGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Globals.IsClient == true)
                {
                    Process.Start(Dirs.StartupPath + @"\BFBC2Game.exe");
                    Close();
                }
                else
                {
                    MessageBox.Show("This function is not available for servers!", "Server detected!");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to start the game! See error.log", "error");
            }
        }

        private void BtnShowInfo_Click(object sender, RoutedEventArgs e)
        {
            var windowMapInfo = new WindowMapInfo();
            windowMapInfo.Owner = this;
            windowMapInfo.ShowDialog();
        }

        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            var windowAppInfo = new WindowAppInfo();
            windowAppInfo.Owner = this;
            windowAppInfo.ShowDialog();
        }

        private void BtnVisitHeico_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://www.nexusmods.com/battlefieldbadcompany2/users/45260312");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to open link! See error.log", "error");
            }
        }

        private void BtnJoinDiscord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://discord.me/battlefieldmodding");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to open link! See error.log", "error");
            }
        }

        private void BtnReportBug_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://www.nexusmods.com/battlefieldbadcompany2/mods/4?tab=bugs");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to open link! See error.log", "error");
            }
        }        

        private void ShowLogo(string imgBox)
        {
            try
            {
                if (mediaStream != null)
                {
                    mediaStream.Close();
                    mediaStream.Dispose();
                }

                var bitmap = new BitmapImage();
                mediaStream = new FileStream(Dirs.LogoPng, FileMode.Open);

                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.None;
                bitmap.StreamSource = mediaStream;
                bitmap.EndInit();
                bitmap.Freeze();

                if (imgBox == "ModManager")
                    imgBoxModManager.Source = bitmap;
                else if (imgBox == "ServerBrowser")
                    imgBoxServerBrowser.Source = bitmap;
                else if (imgBox == "MapBrowser")
                    imgBoxMapBrowser.Source = bitmap;
                else
                {
                    imgBoxModManager.Source = bitmap;
                    imgBoxServerBrowser.Source = bitmap;
                    imgBoxMapBrowser.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Unable to load preview image! See error.log", "error");
            }
        }

        private void DataGridModManager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridModManager.SelectedItem == null)
            {
                //ShowLogo("ModManager");
                return;
            }

            if (dataGridModManager.Items.Count > 1 && btnUp.IsEnabled == false)
            {
                btnUp.IsEnabled = true;
                btnDown.IsEnabled = true;
            }

            /*
            if (dataGridModManager.Items.IndexOf(dataGridModManager.SelectedItem) == 0)
                btnUp.IsEnabled = false;

            if (dataGridModManager.Items.IndexOf(dataGridModManager.SelectedItem) == dataGridModManager.Items.Count - 1)
                btnDown.IsEnabled = false;
            */

            if (btnApply.IsEnabled == false)
                btnDelMod.IsEnabled = true;

            var item = dataGridModManager.SelectedItem as ModManagerItem;

            if (item.ModImage != "")
                imgBoxModManager.Source = new BitmapImage(new Uri(item.ModImage));

            Write.ToModInfo();
        }

        private void DataGridMapBrowser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridMapBrowser.SelectedItem == null)
            {
                //ShowLogo("MapBrowser");
                return;
            }

            var item = dataGridMapBrowser.SelectedItem as MapBrowserItem;

            if (item.MapImage != "")
                imgBoxMapBrowser.Source = new BitmapImage(new Uri(item.MapImage));

            EnableInstallMapButton();
        }

        private void DataGridServerBrowser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridServerBrowser.SelectedItem == null)
            {
                //ShowLogo("ServerBrowser");
                return;
            }
            
            var item = dataGridServerBrowser.SelectedItem as ServerBrowserItem;

            if (item.ServerMap != "")
            {
                using (var xr = new XmlTextReader(Dirs.ListMapsXML) as XmlReader)
                {
                    bool b = false;

                    while (xr.Read())
                    {
                        while (xr.MoveToNextAttribute())
                        {
                            if (xr.Name.StartsWith("AIR") || xr.Name.StartsWith("BC1") || xr.Name.StartsWith("MP") || xr.Name.StartsWith("NAM"))
                                if (item.ServerMap == xr.Value)
                                    b = true;

                            if (xr.Name == "Image" && xr.Value != String.Empty && b == true)
                            {
                                imgBoxServerBrowser.Source = new BitmapImage(new Uri(xr.Value));
                                b = false;
                            }
                        }
                    }
                }
            }
            
            Write.ToServerInfo();

            btnInstallMods.IsEnabled = true;
        }        

        void OnModChecked(object sender, RoutedEventArgs e)
        {
            var cell = sender as DataGridCell;

            if (cell.IsLoaded)
                OnModUnChecked();
        }

        void OnModUnchecked(object sender, RoutedEventArgs e)
        {
            var cell = sender as DataGridCell;

            if (cell.IsLoaded)
                OnModUnChecked();
        }

        private void OnModUnChecked()
        {
            EnableAllButtons(false);

            if (dataGridModManager.Items.Count > 1 && btnUp.IsEnabled == false)
            {
                btnUp.IsEnabled = true;
                btnDown.IsEnabled = true;
            }

            btnReset.IsEnabled = true;
            btnApply.IsEnabled = true;
            btnDelMod.IsEnabled = false;

            EnableAllTabs("ModManager", false);

            if (Globals.HasModBeenUnChecked == false)
                Log.Write("Mod enabled/disabled. Don't forget to apply changes!", "warning");

            Globals.HasModBeenUnChecked = true;
        }

        private void EnableAllTabs(string tab, bool enable)
        {
            if (enable)
            {
                tabModManager.IsEnabled = true;
                tabServerBrowser.IsEnabled = true;
                tabMapBrowser.IsEnabled = true;
                tabSettings.IsEnabled = true;
            }
            else
            {
                switch (tab)
                {
                    case "ModManager":
                        tabServerBrowser.IsEnabled = false;
                        tabMapBrowser.IsEnabled = false;
                        tabSettings.IsEnabled = false;
                        break;
                    case "ServerBrowser":
                        tabModManager.IsEnabled = false;
                        tabMapBrowser.IsEnabled = false;
                        tabSettings.IsEnabled = false;
                        break;
                    case "MapBrowser":
                        tabModManager.IsEnabled = false;
                        tabServerBrowser.IsEnabled = false;
                        tabSettings.IsEnabled = false;
                        break;
                    case "Settings":
                        tabModManager.IsEnabled = false;
                        tabServerBrowser.IsEnabled = false;
                        tabMapBrowser.IsEnabled = false;
                        break;
                }
            }
        }

        private void SetupButtons()
        {
            btnUp.IsEnabled = false;
            btnDown.IsEnabled = false;
            btnReset.IsEnabled = false;
            btnApply.IsEnabled = false;
            btnDelMod.IsEnabled = false;
            btnUpdateMaps.IsEnabled = false;
            btnInstallMap.IsEnabled = false;
            btnInstallMods.IsEnabled = false;

            if (dataGridModManager.Items.Count < 1)
            {
                btnDelAllMods.IsEnabled = false;
                btnEnableMods.IsEnabled = false;
                btnDisableMods.IsEnabled = false;
            }
        }

        private void EnableAllButtons(bool enabled)
        {
            if (enabled == true)
            {
                btnStartGame.IsEnabled = true;
                btnInstallMod.IsEnabled = true;

                if (dataGridModManager.Items.Count > 0)
                {
                    if (Settings.ModsEnabled == true)
                    {
                        btnEnableMods.IsEnabled = false;
                        btnDisableMods.IsEnabled = true;
                    }

                    btnDelAllMods.IsEnabled = true;
                }

                if (dataGridModManager.SelectedItem != null)
                {
                    btnDelMod.IsEnabled = true;

                    if (dataGridModManager.Items.Count > 1)
                    {
                        btnUp.IsEnabled = true;
                        btnDown.IsEnabled = true;
                    }
                }

                if (dataGridServerBrowser.SelectedItem != null)
                    btnInstallMods.IsEnabled = true;

                if (dataGridMapBrowser.SelectedItem != null)
                    EnableInstallMapButton();

                if (Globals.MapUpdatesAvailable == true)
                    btnUpdateMaps.IsEnabled = true;

                btnShowInfo.IsEnabled = true;
                btnRefresh.IsEnabled = true;

                if (Settings.ModsEnabled == false)
                {
                    btnInstallMod.IsEnabled = false;
                    btnEnableMods.IsEnabled = true;
                    btnDisableMods.IsEnabled = false;
                    btnDelAllMods.IsEnabled = false;
                    btnUp.IsEnabled = false;
                    btnDown.IsEnabled = false;
                    btnApply.IsEnabled = false;
                    btnReset.IsEnabled = false;
                    btnDelMod.IsEnabled = false;
                    btnInstallMods.IsEnabled = false;
                    btnRefresh.IsEnabled = false;
                    btnInstallMap.IsEnabled = false;
                    btnUpdateMaps.IsEnabled = false;
                    btnShowInfo.IsEnabled = false;
                }
            }
            else
            {
                btnStartGame.IsEnabled = false;
                btnInstallMod.IsEnabled = false;
                btnEnableMods.IsEnabled = false;
                btnDisableMods.IsEnabled = false;
                btnDelAllMods.IsEnabled = false;
                btnUp.IsEnabled = false;
                btnDown.IsEnabled = false;
                btnApply.IsEnabled = false;
                btnReset.IsEnabled = false;
                btnDelMod.IsEnabled = false;
                btnInstallMods.IsEnabled = false;
                btnRefresh.IsEnabled = false;
                btnInstallMap.IsEnabled = false;
                btnUpdateMaps.IsEnabled = false;
                btnShowInfo.IsEnabled = false;
            }
        }

        private void EnableInstallMapButton()
        {
            if (dataGridMapBrowser.SelectedItem == null)
                return;

            bool b = false;

            foreach (var item in dataGridModManager.Items)
            {
                var itemMM = item as ModManagerItem;
                var itemMB = dataGridMapBrowser.SelectedItem as MapBrowserItem;

                if (itemMB != null)
                {
                    if (itemMB.MapName.Contains(itemMM.ModName))
                    {
                        b = true;
                        break;
                    }
                    else
                    {
                        b = false;
                    }
                }
            }

            if (b == true)
                btnInstallMap.IsEnabled = false;
            else
                btnInstallMap.IsEnabled = true;

            if (btnInstallMap.IsEnabled)
                Write.ToMapInfo("No");
            else
                Write.ToMapInfo("Yes");
        }        
    }
}
