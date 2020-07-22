using System;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace BFBC2_Toolkit.Data
{
    public class CompletionData : ICompletionData
    {
        private string Symbol { get; set; }

        public CompletionData(string text, string symbol)
        {
            this.Text = text;
            this.Symbol = symbol;
        }

        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        public object Content
        {
            get { return this.Text; }
        }

        public object Description
        {
            get { return "Description for " + this.Text; }
        }

        public double Priority { get; }

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            if (this.Symbol == "/")
                this.Text += ">";
            else if (Symbol == " ")
                this.Text += "=\"\"";

            textArea.Document.Replace(completionSegment, this.Text);
        }
    }

    public class CompletionHandler
    {
        public static void HandleInput(string text)
        {
            //if (UIElements.CodeComWindow != null && !UIElements.CodeComWindow.CompletionList.ListBox.HasItems)
            //    UIElements.CodeComWindow.Close();

            if (text == "<" || text == "/")
            {
                UIElements.CodeComWindow = new CompletionWindow(UIElements.TextEditor.TextArea);
                IList<ICompletionData> data = UIElements.CodeComWindow.CompletionList.CompletionData;
                data.Add(new CompletionData("array", text));
                data.Add(new CompletionData("complex", text));
                data.Add(new CompletionData("field", text));
                data.Add(new CompletionData("instance", text));
                data.Add(new CompletionData("item", text));
                data.Add(new CompletionData("partition", text));
                UIElements.CodeComWindow.Show();
                UIElements.CodeComWindow.Closed += delegate {
                    UIElements.CodeComWindow = null;
                };
            }

            if (text == " ")
            {
                UIElements.CodeComWindow = new CompletionWindow(UIElements.TextEditor.TextArea);
                IList<ICompletionData> data = UIElements.CodeComWindow.CompletionList.CompletionData;
                data.Add(new CompletionData("exportMode", text));
                data.Add(new CompletionData("guid", text));
                data.Add(new CompletionData("id", text));
                data.Add(new CompletionData("name", text));
                data.Add(new CompletionData("primaryInstance", text));
                data.Add(new CompletionData("ref", text));
                data.Add(new CompletionData("type", text));
                UIElements.CodeComWindow.Show();
                UIElements.CodeComWindow.Closed += delegate {
                    UIElements.CodeComWindow = null;
                };
            }
        }
    }
}
