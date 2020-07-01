using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
//using Memory;

namespace BFBC2_Toolkit.Windows
{
    public partial class CustomizerWindow : MetroWindow
    {
        
        //private static Mem memory = new Mem();
        //private static Timer timer;

        public CustomizerWindow()
        {
            //InitializeComponent();           
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            bool isHooked = InitHook();

            if (isHooked && timer == null)
                StartTimer();
                */
        }

        private void MetroWindow_KeyUp(object sender, KeyEventArgs e)
        {
            /*
            if (e.Key == Key.Enter)
                WriteMemory();
                */
        }

        private void BtnHookProcess_Click(object sender, RoutedEventArgs e)
        {
            /*
            bool isHooked = InitHook();

            if (isHooked && timer == null)
                StartTimer();
                */
        }

        /*
        private bool InitHook()
        {

            bool isProcessAvailable = false;

            int processID = memory.GetProcIdFromName("BFBC2Game");

            if (processID > 0)
                isProcessAvailable = memory.OpenProcess(processID);

            if (isProcessAvailable)
            {
                ReadMemory();

                Title = "BFBC2 Customizer (Hooked)";

                return true;
            }
            else
            {
                Title = "BFBC2 Customizer (Not Hooked)";

                return false;
            }
            
        }
        */

        private void ReadMemory()
        {
            /*
            string mapID = memory.ReadString("base+01170F64,40,10,38,B8,0");
            string mapName = memory.ReadString("base+011705F8,4C0");

            lblMapIDValue.Content = "_" + mapID;
            lblMapNameValue.Content = "_" + ToTitleCase(mapName);

            float satR = memory.ReadFloat("base+0117C2E0,3C,B8,74,30", "", false);
            float satG = memory.ReadFloat("base+0117C2E0,3C,A8,1C,24", "", false);
            float satB = memory.ReadFloat("base+0117C2E0,3C,98,1C0,10,58", "", false);
            float conR = memory.ReadFloat("base+0117C2E0,3C,B8,70,20", "", false);
            float conG = memory.ReadFloat("base+0117C2E0,3C,B8,74,54", "", false);
            float conB = memory.ReadFloat("base+0117C2E0,3C,B8,74,58", "", false);
            float brightR = memory.ReadFloat("base+0117C2E0,3C,98,1BC,14,60", "", false);
            float brightG = memory.ReadFloat("base+0117C2E0,3C,98,1C4,C,64", "", false);
            float brightB = memory.ReadFloat("base+0117C2E0,3C,98,1BC,14,68", "", false);

            float sunTheta = memory.ReadFloat("base+0117C2E0,3C,98,1BC,1C,40", "", false);
            float sunPhi = memory.ReadFloat("base+0117C2E0,3C,98,5C,8", "", false);
            float sunSize = memory.ReadFloat("base+0117C2E0,3C,98,1C4,14,4C", "", false);
            float sunColorR = memory.ReadFloat("base+0117C2E0,3C,98,1C0,18,20", "", false);
            float sunColorG = 0f;
            float sunColorB = 0f;

            float minimapPosX = memory.ReadFloat("base+011729E0,5C8", "", false);
            float minimapPosY = memory.ReadFloat("base+011729E0,5CC", "", false);
            float minimapSizeX = memory.ReadFloat("base+011729E0,5C0", "", false);
            float minimapSizeY = memory.ReadFloat("base+011729E0,5C4", "", false);
            float minimapOverlayAlpha = memory.ReadFloat("base+011729E0,5DC", "", false);
            float minimapOverlayColorR = memory.ReadFloat("base+011729E0,5D0", "", false);
            float minimapOverlayColorG = memory.ReadFloat("base+011729E0,5D4", "", false);
            float minimapOverlayColorB = memory.ReadFloat("base+011729E0,5D8", "", false);
            float minimapCameraFOV = memory.ReadFloat("base+011729E0,5B4", "", false);
            float minimapCameraDistance = memory.ReadFloat("base+011729E0,5B8", "", false);
            float minimapCameraLookDistance = memory.ReadFloat("base+011729E0,5BC", "", false);

            txtBoxSatR.Text = satR.ToString();
            txtBoxSatG.Text = satG.ToString();
            txtBoxSatB.Text = satB.ToString();
            txtBoxConR.Text = conR.ToString();
            txtBoxConG.Text = conG.ToString();
            txtBoxConB.Text = conB.ToString();
            txtBoxBrightR.Text = brightR.ToString();
            txtBoxBrightG.Text = brightG.ToString();
            txtBoxBrightB.Text = brightB.ToString();

            txtBoxSunTheta.Text = sunTheta.ToString();
            txtBoxSunPhi.Text = sunPhi.ToString();
            txtBoxSunSize.Text = sunSize.ToString();
            txtBoxSunColorR.Text = sunColorR.ToString();
            txtBoxSunColorG.Text = sunColorG.ToString();
            txtBoxSunColorB.Text = sunColorB.ToString();

            txtBoxMinimapPosX.Text = minimapPosX.ToString();
            txtBoxMinimapPosY.Text = minimapPosY.ToString();
            txtBoxMinimapSizeX.Text = minimapSizeX.ToString();
            txtBoxMinimapSizeY.Text = minimapSizeY.ToString();
            txtBoxMinimapOverlayAlpha.Text = minimapOverlayAlpha.ToString();
            txtBoxMinimapOverlayColorR.Text = minimapOverlayColorR.ToString();
            txtBoxMinimapOverlayColorG.Text = minimapOverlayColorG.ToString();
            txtBoxMinimapOverlayColorB.Text = minimapOverlayColorB.ToString();
            txtBoxMinimapCameraFov.Text = minimapCameraFOV.ToString();
            txtBoxMinimapCameraDistance.Text = minimapCameraDistance.ToString();
            txtBoxMinimapCameraLookDistance.Text = minimapCameraLookDistance.ToString();
            */
        }

        private void WriteMemory()
        {
            /*
            memory.WriteMemory("base+0117C2E0,3C,B8,74,30", "float", txtBoxSatR.Text);
            memory.WriteMemory("base+0117C2E0,3C,A8,1C,24", "float", txtBoxSatG.Text);
            memory.WriteMemory("base+0117C2E0,3C,98,1C0,10,58", "float", txtBoxSatB.Text);
            memory.WriteMemory("base+0117C2E0,3C,B8,70,20", "float", txtBoxConR.Text);
            memory.WriteMemory("base+0117C2E0,3C,B8,74,54", "float", txtBoxConG.Text);
            memory.WriteMemory("base+0117C2E0,3C,B8,74,58", "float", txtBoxConB.Text);
            memory.WriteMemory("base+0117C2E0,3C,98,1BC,14,60", "float", txtBoxBrightR.Text);
            memory.WriteMemory("base+0117C2E0,3C,98,1C4,C,64", "float", txtBoxBrightG.Text);
            memory.WriteMemory("base+0117C2E0,3C,98,1BC,14,68", "float", txtBoxBrightB.Text);

            memory.WriteMemory("base+0117C2E0,3C,98,1BC,1C,40", "float", txtBoxSunTheta.Text);
            memory.WriteMemory("base+0117C2E0,3C,98,5C,8", "float", txtBoxSunPhi.Text);
            memory.WriteMemory("base+0117C2E0,3C,98,1C4,14,4C", "float", txtBoxSunSize.Text);
            memory.WriteMemory("base+0117C2E0,3C,98,1C0,18,20", "float", txtBoxSunColorR.Text);
            //memory.WriteMemory("address", "float", txtBoxSunColorG); //SunColorG
            //memory.WriteMemory("address", "float", txtBoxSunColorB); //SunColorB

            memory.WriteMemory("base+011729E0,5C8", "float", txtBoxMinimapPosX.Text);
            memory.WriteMemory("base+011729E0,5CC", "float", txtBoxMinimapPosY.Text);
            memory.WriteMemory("base+011729E0,5C0", "float", txtBoxMinimapSizeX.Text);
            memory.WriteMemory("base+011729E0,5C4", "float", txtBoxMinimapSizeY.Text);
            memory.WriteMemory("base+011729E0,5DC", "float", txtBoxMinimapOverlayAlpha.Text);
            memory.WriteMemory("base+011729E0,5D0", "float", txtBoxMinimapOverlayColorR.Text);
            memory.WriteMemory("base+011729E0,5D4", "float", txtBoxMinimapOverlayColorG.Text);
            memory.WriteMemory("base+011729E0,5D8", "float", txtBoxMinimapOverlayColorB.Text);
            memory.WriteMemory("base+011729E0,5B4", "float", txtBoxMinimapCameraFov.Text);
            memory.WriteMemory("base+011729E0,5B8", "float", txtBoxMinimapCameraDistance.Text);
            memory.WriteMemory("base+011729E0,5BC", "float", txtBoxMinimapCameraLookDistance.Text);
            */
        }

        /*
        public string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        private void StartTimer()
        {
            timer = new Timer();
            timer.Interval = 500;
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Enabled = true;            
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReadMemory();

            MessageBox.Show("Timer started...");
        }
        */
        
    }
}
