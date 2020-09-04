using System;
using System.IO;
using System.Text;
using System.Xml;
using BFBC2Toolkit.Data;

namespace BFBC2Toolkit.Functions
{
    public class Create
    {
        public static void PrecreateDirs()
        {
            Directory.CreateDirectory(Dirs.Games);
            Directory.CreateDirectory(Dirs.Logs);
            Directory.CreateDirectory(Dirs.Projects);
            Directory.CreateDirectory(Dirs.Output);
            Directory.CreateDirectory(Dirs.OutputDDS);
            Directory.CreateDirectory(Dirs.OutputHeightmap);
            Directory.CreateDirectory(Dirs.OutputiTexture);
            Directory.CreateDirectory(Dirs.OutputMods);
            Directory.CreateDirectory(Dirs.OutputVideo);
            Directory.CreateDirectory(Dirs.OutputXML);
            Directory.CreateDirectory(Dirs.OutputSwfMovie);
        }

        public static void ConfigFiles()
        {
            if (!File.Exists(Dirs.ConfigGames))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<Games>" +
                               "</Games>");

                File.WriteAllText(Dirs.ConfigGames, IndentXml(xmlDoc));
            }

            if (!File.Exists(Dirs.ConfigSettings))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<Settings>" +
                               "</Settings>");

                File.WriteAllText(Dirs.ConfigSettings, IndentXml(xmlDoc));
            }

            CreateSettingsTemplate();
        }

        private static void CreateSettingsTemplate()
        {
            var settings = new Settings();

            var xmlDocSettings = new XmlDocument();
            xmlDocSettings.Load(Dirs.ConfigSettings);
            var nodeList = xmlDocSettings.SelectNodes("/Settings/Setting");

            foreach (var prop in settings.GetType().GetProperties())
            {
                bool entryExists = false;

                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i].Attributes["Name"].Value == prop.Name)
                    {
                        entryExists = true;
                        break;
                    }
                }

                if (!entryExists)
                {
                    var rootNode = xmlDocSettings.DocumentElement;
                    var newElement = xmlDocSettings.CreateElement("Setting");

                    var attrName = xmlDocSettings.CreateAttribute("Name");
                    attrName.Value = prop.Name;

                    newElement.Attributes.Append(attrName);

                    var attrPlat = xmlDocSettings.CreateAttribute("Value");
                    attrPlat.Value = prop.GetValue(settings).ToString();

                    newElement.Attributes.Append(attrPlat);

                    rootNode.AppendChild(newElement);
                    xmlDocSettings.AppendChild(rootNode);
                }
            }

            xmlDocSettings.Save(Dirs.ConfigSettings);
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

            //Very cheap 'hack' to avoid a null exception when loading "Select Game" window because I'm somehow unable to Output a file in real utf-8 encoding. 
            //Working with xml is really a pain sometimes. I will try to look into a proper fix for this issue. 
            return sb.ToString().Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "<?xml version=\"1.0\" encoding=\"utf-8\"?>"); 
        }
    }
}
