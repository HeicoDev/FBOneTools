using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media;
using BFBC2Shared.Data;

namespace BFBC2Shared.Functions
{
    public class Log
    {
        public static void Error(string ex)
        {
            //Create detailed error log in error.log
            using (StreamWriter sw = new StreamWriter(SharedDirs.ErrorLog, true))
            {
                sw.WriteLine("#Error Log**************************Error Log**************************Error Log#");
                sw.WriteLine($"Name: { SharedGlobals.ClientName }");
                sw.WriteLine($"Ver: { SharedGlobals.ClientVersion }");
                sw.WriteLine($"Date: { DateTime.Now }");
                sw.WriteLine($"Error: { ex }");
            }
        }

        public static void Write(string log)
        {
            Write(log, "");
        }

        public static void Write(string log, string result)
        {
            var bc = new BrushConverter();
            var tr = new TextRange(SharedUIElements.TxtBoxEventLog.Document.ContentEnd, SharedUIElements.TxtBoxEventLog.Document.ContentEnd);

            switch (result.ToLower())
            {
                case "done":
                    tr.Text = $"[{ DateTime.Now }] ";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
                    tr = new TextRange(SharedUIElements.TxtBoxEventLog.Document.ContentEnd, SharedUIElements.TxtBoxEventLog.Document.ContentEnd);
                    tr.Text = "Done! ";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertFromString("#FF41BB41"));
                    tr = new TextRange(SharedUIElements.TxtBoxEventLog.Document.ContentEnd, SharedUIElements.TxtBoxEventLog.Document.ContentEnd);
                    tr.Text = $"{log}\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
                    break;
                case "error":
                    tr.Text = $"[{ DateTime.Now }] ";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
                    tr = new TextRange(SharedUIElements.TxtBoxEventLog.Document.ContentEnd, SharedUIElements.TxtBoxEventLog.Document.ContentEnd);
                    tr.Text = "Error: ";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Tomato);
                    tr = new TextRange(SharedUIElements.TxtBoxEventLog.Document.ContentEnd, SharedUIElements.TxtBoxEventLog.Document.ContentEnd);
                    tr.Text = $"{log}\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
                    break;
                case "warning":
                    tr.Text = $"[{ DateTime.Now }] ";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
                    tr = new TextRange(SharedUIElements.TxtBoxEventLog.Document.ContentEnd, SharedUIElements.TxtBoxEventLog.Document.ContentEnd);
                    tr.Text = "Warning: ";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Gold);
                    tr = new TextRange(SharedUIElements.TxtBoxEventLog.Document.ContentEnd, SharedUIElements.TxtBoxEventLog.Document.ContentEnd);
                    tr.Text = $"{log}\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
                    break;
                default:
                    tr.Text = $"[{ DateTime.Now }] { log }\r";
                    tr.ApplyPropertyValue(TextElement.ForegroundProperty, bc.ConvertToString("#FFE2E2E2"));
                    break;
            }
          
            SharedUIElements.TxtBoxEventLog.ScrollToEnd();
            SharedUIElements.TxtBoxEventLog.LineUp();
        }        
    }
}
