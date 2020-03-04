using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BFBC2_Toolkit.Data
{
    public class Elements
    {
        public static RichTextBox txtBoxEventLog;
        public static RichTextBox txtBoxInformation;

        public static void SetElements(RichTextBox rtbEventLog, RichTextBox rtbInformation)
        {
            txtBoxEventLog = rtbEventLog;
            txtBoxInformation = rtbInformation;
        }
    }
}
