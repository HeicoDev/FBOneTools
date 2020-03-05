using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Functions
{
    public class Tree
    {
        private static string fileType = "Unknown",
                              fileFormat = "Unknown",
                              fileSupported = "No",
                              fileArchive = "Unknown";

        public static void Populate(TreeView treeView, string path)
        {
            treeView.Items.Clear();

            var directoryInfo = new DirectoryInfo(path);

            treeView.Items.Add(CreateDirectoryNode(directoryInfo));
        }

        private static CustomTreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var treeViewfolder = new CustomTreeViewItem { Name = directoryInfo.Name, Path = directoryInfo.FullName, Type = "Folder", Format = "None", Supported = "No", Archive = "None" };

            foreach (var directory in directoryInfo.GetDirectories())
            {
                treeViewfolder.Items.Add(CreateDirectoryNode(directory));
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                foreach (string format in Vars.fileFormats)
                {
                    if (file.Name.EndsWith("." + format))
                    {
                        GetFileInfo(file.Name, file.FullName);

                        treeViewfolder.Items.Add(new CustomTreeViewItem { Name = file.Name, Path = file.FullName, Type = fileType, Format = fileFormat, Supported = fileSupported, Archive = fileArchive, ParentItem = treeViewfolder });

                        fileType = "Unknown";
                        fileFormat = "Unknown";
                        fileSupported = "No";
                        fileArchive = "";

                        break;
                    }
                }
            }

            return treeViewfolder;
        }

        private static void GetFileInfo(string fileName, string filePath)
        {
            if (!fileName.Contains("."))
            {
                fileType = "Folder";
                fileFormat = "None";
                fileSupported = "No";
                fileArchive = "None";
            }
            else if (fileName.EndsWith(".dbx"))
            {
                fileType = "Text";
                fileFormat = "XML (.xml)";
                fileSupported = "Yes";

                foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                {
                    if (filePath.Contains(kvp.Value))
                    {
                        fileArchive = kvp.Key;
                        break;
                    }
                }
            }
            else if (fileName.EndsWith(".ini"))
            {
                fileType = "Text";
                fileFormat = "INI (.ini)";
                fileSupported = "Yes";

                foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                {
                    if (filePath.Contains(kvp.Value))
                    {
                        fileArchive = kvp.Key;
                        break;
                    }
                }
            }
            else if (fileName.EndsWith(".txt"))
            {
                fileType = "Text";
                fileFormat = "TXT (.txt)";
                fileSupported = "Yes";

                foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                {
                    if (filePath.Contains(kvp.Value))
                    {
                        fileArchive = kvp.Key;
                        break;
                    }
                }
            }
            else if (fileName.EndsWith(".itexture") || fileName.EndsWith(".ps3texture") || fileName.EndsWith(".xenontexture"))
            {
                fileType = "Texture";
                fileFormat = "DDS (.dds)";
                fileSupported = "Yes";

                foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                {
                    if (filePath.Contains(kvp.Value))
                    {
                        fileArchive = kvp.Key;
                        break;
                    }
                }
            }
            else if (fileName.EndsWith(".terrainheightfield"))
            {
                fileType = "Heightmap";
                fileFormat = "RAW (.raw)";
                fileSupported = "Partially";

                foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                {
                    if (filePath.Contains(kvp.Value))
                    {
                        fileArchive = kvp.Key;
                        break;
                    }
                }
            }
            else if (fileName.EndsWith(".binkmemory"))
            {
                fileType = "Video";
                fileFormat = "Bink (.bik)";
                fileSupported = "Yes";

                foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                {
                    if (filePath.Contains(kvp.Value))
                    {
                        fileArchive = kvp.Key;
                        break;
                    }
                }
            }
            else
            {
                foreach (string format in Vars.fileFormats)
                {
                    if (fileName.EndsWith("." + format))
                    {
                        fileType = "Unknown";
                        fileFormat = "Unknown";
                        fileSupported = "No";

                        foreach (KeyValuePair<string, string> kvp in Vars.fbrbFiles)
                        {
                            if (filePath.Contains(kvp.Value))
                            {
                                fileArchive = kvp.Key;
                                break;
                            }
                        }

                        break;
                    }
                }
            }
        }
    }
}
