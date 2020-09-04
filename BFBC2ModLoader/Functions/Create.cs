using System;
using System.IO;
using System.Xml;
using BFBC2ModLoader.Data;
using BFBC2Shared.Functions;

namespace BFBC2ModLoader.Functions
{
    public class Create
    {
        public static void ConfigFiles()
        {
            try
            {
                if (!File.Exists(Dirs.ConfigXML))
                {
                    var xw = XmlWriter.Create(Dirs.ConfigXML);
                    xw.Close();
                    Save.ConfigXML();
                }

                if (!File.Exists(Dirs.ModsXML))
                {
                    var xw = XmlWriter.Create(Dirs.ModsXML);
                    xw.Close();
                    Save.ModsXML();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not create config files! See error.log", "error");
            }
        }        

        public static void PrecreateDirs()
        {
            try
            {
                if (!Directory.Exists(Dirs.ModsFolder))
                    Directory.CreateDirectory(Dirs.ModsFolder);
                if (!Directory.Exists(Dirs.Logs))
                    Directory.CreateDirectory(Dirs.Logs);
                if (!Directory.Exists(Dirs.Downloads))
                    Directory.CreateDirectory(Dirs.Downloads);
                if (!Directory.Exists(Dirs.ModsCommon))
                    Directory.CreateDirectory(Dirs.ModsCommon);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not precreate dirs! See error.log", "error");
            }
        }
    }
}
