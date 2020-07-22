using System;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Functions
{
    public class Play
    {
        public static void Video(string path)
        {
            try
            {
                UIElements.MediaElement.Stop();
                UIElements.MediaElement.Close();
                UIElements.MediaElement.Source = new Uri(path);
                UIElements.MediaElement.Play();
            }
            catch
            {
                Write.ToEventLog("Unable to load video preview! Exporting and importing should still work fine.", "warning");
            }
        }
    }
}
