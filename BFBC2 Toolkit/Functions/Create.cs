using System;
using System.IO;
using System.Text;
using System.Xml;
using BFBC2_Toolkit.Data;

namespace BFBC2_Toolkit.Functions
{
    public class Create
    {
        public static void PrecreateDirs()
        {
            try
            {
                Directory.CreateDirectory(Dirs.games);
                Directory.CreateDirectory(Dirs.logs);
                Directory.CreateDirectory(Dirs.projects);
                Directory.CreateDirectory(Dirs.output);
                Directory.CreateDirectory(Dirs.outputDDS);
                Directory.CreateDirectory(Dirs.outputHeightmap);
                Directory.CreateDirectory(Dirs.outputiTexture);
                Directory.CreateDirectory(Dirs.outputMods);
                Directory.CreateDirectory(Dirs.outputVideo);
                Directory.CreateDirectory(Dirs.outputXML);
                Directory.CreateDirectory(Dirs.outputSwfMovie);
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to precreate dirs! See error.log", "error");
            }
        }

        public static void ConfigFiles()
        {
            try
            {
                if (!File.Exists(Dirs.configGames))
                {
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml("<Games>" +
                                   "</Games>");

                    File.WriteAllText(Dirs.configGames, IndentXml(xmlDoc));
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Unable to create config files! See error.log", "error");
            }
        }

        private static string IndentXml(XmlDocument xmlDoc)
        {
            var sb = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = Environment.NewLine,
                NewLineHandling = NewLineHandling.Replace
            };
            using (var writer = XmlWriter.Create(sb, settings))
            {
                xmlDoc.Save(writer);
            }

            //Very cheap 'hack' to avoid a null exception when loading "Select Game" window because I'm somehow unable to output a file in real utf-8 encoding. 
            //Working with xml is really a pain sometimes. I will try to look into a proper fix for this issue. 
            return sb.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "<?xml version=\"1.0\" encoding=\"utf-8\"?>"); 
        }
    }
}
