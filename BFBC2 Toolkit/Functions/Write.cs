using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Functions
{
    public class Write
    {
        public static void ToErrorLog(Exception ex)
        {
            //Create detailed error log in error.log
            using (StreamWriter sw = new StreamWriter(Dirs.errorLog, true))
            {
                sw.WriteLine("#Error Log**************************Error Log**************************Error Log#");
                sw.WriteLine("Name: BFBC2 Mod Loader");
                sw.WriteLine("Ver: " + Vars.VersionClient);
                sw.WriteLine("Date: " + DateTime.Now);
                sw.WriteLine("Error: " + ex);
            }
        }

        public static void ToEventLog(string log, string result)
        {
            var bc = new BrushConverter();

            if (result == "done")
            {
                var tr = new TextRange(Elements.TxtBoxEventLog.Document.ContentEnd, Elements.TxtBoxEventLog.Document.ContentEnd);

                tr.Text = "[" + DateTime.Now + "] ";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
                tr = new TextRange(Elements.TxtBoxEventLog.Document.ContentEnd, Elements.TxtBoxEventLog.Document.ContentEnd);
                tr.Text = "Done! ";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString("#FF41BB41"));
                tr = new TextRange(Elements.TxtBoxEventLog.Document.ContentEnd, Elements.TxtBoxEventLog.Document.ContentEnd);
                tr.Text = log + "\r";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
            }
            else if (result == "error")
            {
                var tr = new TextRange(Elements.TxtBoxEventLog.Document.ContentEnd, Elements.TxtBoxEventLog.Document.ContentEnd);

                tr.Text = "[" + DateTime.Now + "] ";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
                tr = new TextRange(Elements.TxtBoxEventLog.Document.ContentEnd, Elements.TxtBoxEventLog.Document.ContentEnd);
                tr.Text = "Error: ";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Tomato);
                tr = new TextRange(Elements.TxtBoxEventLog.Document.ContentEnd, Elements.TxtBoxEventLog.Document.ContentEnd);
                tr.Text = log + "\r";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
            }
            else if (result == "warning")
            {
                var tr = new TextRange(Elements.TxtBoxEventLog.Document.ContentEnd, Elements.TxtBoxEventLog.Document.ContentEnd);

                tr.Text = "[" + DateTime.Now + "] ";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
                tr = new TextRange(Elements.TxtBoxEventLog.Document.ContentEnd, Elements.TxtBoxEventLog.Document.ContentEnd);
                tr.Text = "Warning: ";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Gold);
                tr = new TextRange(Elements.TxtBoxEventLog.Document.ContentEnd, Elements.TxtBoxEventLog.Document.ContentEnd);
                tr.Text = log + "\r";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
            }
            else
            {
                var tr = new TextRange(Elements.TxtBoxEventLog.Document.ContentEnd, Elements.TxtBoxEventLog.Document.ContentEnd);

                tr.Text = "[" + DateTime.Now + "] " + log + "\r";
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
            }

            Elements.TxtBoxEventLog.ScrollToEnd();
            Elements.TxtBoxEventLog.LineUp();
        }

        public static void ToInfoBox(CustomTreeViewItem ctvi)
        {
            if (ctvi.Type == "Texture")
            {
                Elements.TxtBoxInformation.Document.Blocks.Clear();
                Elements.TxtBoxInformation.Document.PageWidth = ctvi.Name.Length * 15;
                Elements.TxtBoxInformation.AppendText("Name:\r" + ctvi.Name + "\r\rType:\r" + ctvi.Type + "\r\rFormat:\r" + ctvi.Format + "\r" + Vars.TextureFormat + "\r\rResolution:\r" + Vars.TextureWidth + "x" + Vars.TextureHeight + "\r\rMipmaps:\r" + Vars.MipmapCount + "\r\rArchive:\r" + ctvi.Archive + "\r\rSupported:\r" + ctvi.Supported);
            }
            else
            {
                Elements.TxtBoxInformation.Document.Blocks.Clear();
                Elements.TxtBoxInformation.Document.PageWidth = ctvi.Name.Length * 15;
                Elements.TxtBoxInformation.AppendText("Name:\r" + ctvi.Name + "\r\rType:\r" + ctvi.Type + "\r\rFormat:\r" + ctvi.Format + "\r\rArchive:\r" + ctvi.Archive + "\r\rSupported:\r" + ctvi.Supported);
            }         
        }
    }
}
