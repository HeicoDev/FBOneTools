using System;
using System.Text;
using System.Xml;
using BFBC2ModLoader.Data;
using BFBC2ModLoader.Data.Bindings;
using BFBC2Shared.Functions;

namespace BFBC2ModLoader.Functions
{
    public class Save
    {
        public static void ModsXML()
        {
            try
            {
                using (var xw = new XmlTextWriter(Dirs.ModsXML, new UnicodeEncoding()))
                {
                    xw.Formatting = Formatting.Indented;
                    xw.WriteStartDocument();
                    xw.WriteStartElement("mods");

                    foreach (var item in UIElements.DataGridModManager.Items)
                    {
                        var itemMM = item as ModManagerItem;

                        string cell0 = itemMM.ModOrder,
                               cell1 = itemMM.ModEnabled.ToString(),
                               cell2 = itemMM.ModName,
                               cell3 = itemMM.ModVersion,
                               cell4 = itemMM.ModType;

                        xw.WriteStartElement("mod");
                        xw.WriteAttributeString("Order", cell0);
                        xw.WriteAttributeString("Enabled", cell1);
                        xw.WriteAttributeString("Name", cell2);
                        xw.WriteAttributeString("Version", cell3);
                        xw.WriteAttributeString("Type", cell4);
                        xw.WriteEndElement();
                    }

                    xw.WriteEndElement();
                }

                Misc.OrderNumber();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not save mods.config! See error.log", "error");
            }
        }

        public static bool ConfigXML()
        {
            try
            {
                using (var xw = new XmlTextWriter(Dirs.ConfigXML, new UnicodeEncoding()))
                {
                    xw.Formatting = Formatting.Indented;
                    xw.WriteStartDocument();
                    xw.WriteStartElement("configs");
                    xw.WriteStartElement("config");
                    xw.WriteAttributeString("modsEnabled", Settings.ModsEnabled.ToString());
                    xw.WriteEndElement();
                    xw.WriteStartElement("config");
                    xw.WriteAttributeString("autoUpdateCheckEnabled", Settings.IsAutoUpdateCheckEnabled.ToString());
                    xw.WriteEndElement();
                    xw.WriteStartElement("config");
                    xw.WriteAttributeString("pathToPython", Settings.PathToPython);
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Write("Could not save settings.config! See error.log", "error");

                return true;
            }
        }
    }
}
