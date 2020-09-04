using System;
using BFBC2Toolkit.Data;
using BFBC2Shared.Functions;

namespace BFBC2Toolkit.Functions
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
                Log.Write("Unable to load video preview! Exporting and importing should still work fine.", "warning");
            }
        }
    }
}
