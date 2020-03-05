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
                Elements.MediaElement.Stop();
                Elements.MediaElement.Close();
                Elements.MediaElement.Source = new Uri(path);
                Elements.MediaElement.Play();
            }
            catch
            {
                Write.ToEventLog("Unable to load video preview! Exporting and importing should still work fine.", "warning");
            }
        }
    }
}
