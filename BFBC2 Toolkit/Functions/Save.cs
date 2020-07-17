using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Functions
{
    public class Save
    {
        public static async Task TextEditorChanges()
        {
            try
            {
                string selectedFilePath = "";

                if (Vars.isDataTreeView)
                    selectedFilePath = Dirs.SelectedFilePathData;
                else
                    selectedFilePath = Dirs.SelectedFilePathMod;

                string textEditorText = Elements.TextEditor.Text;

                if (selectedFilePath.EndsWith(".dbx"))
                {
                    string path = selectedFilePath.Replace(".dbx", ".xml");

                    await Task.Run(() => File.WriteAllText(path, textEditorText));

                    var process = Process.Start(Dirs.scriptDBX, "\"" + path.Replace(@"\", @"\\"));
                    await Task.Run(() => process.WaitForExit());
                }
                else if (selectedFilePath.EndsWith(".ini") || selectedFilePath.EndsWith(".txt"))
                {
                    await Task.Run(() => File.WriteAllText(selectedFilePath, textEditorText));
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to save text editor changes! See error.log", "error");
            }
        }
    }
}
