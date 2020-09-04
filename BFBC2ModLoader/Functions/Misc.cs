using System;
using BFBC2ModLoader.Data;

namespace BFBC2ModLoader.Functions
{
    public class Misc
    {
        public static void OrderNumber()
        {
            try
            {
                int a = 1;

                foreach (var item in UIElements.DataGridModManager.Items)
                {
                    var itemMM = item as ModManagerItem;
                    itemMM.ModOrder = a.ToString();
                    UIElements.DataGridModManager.Items.Refresh();
                    a++;
                }
            }
            catch (Exception ex)
            {
                Write.ToErrorLog(ex);
                Write.ToEventLog("Could not add order numbers! See error.log", "error");
            }
        }
    }
}
