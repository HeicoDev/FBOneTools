using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace BFBC2_Toolkit
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        /*public void AppStartup(object sender, StartupEventArgs e)
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                //MessageBox.Show(AppDomain.CurrentDomain.BaseDirectory);

                string appName = "app";
                var currentAssembly = Assembly.GetExecutingAssembly();

                // Setup path to application Config file in ./Config dir:
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationBase = Environment.CurrentDirectory;
                setup.ConfigurationFile = setup.ApplicationBase +
                                    string.Format(@"\Config\app.Config");

                // Create a new app domain using setup with new Config file path:
                AppDomain newDomain = AppDomain.CreateDomain("NewAppDomain", null, setup);
                int ret = newDomain.ExecuteAssemblyByName(currentAssembly.FullName, e.Args);
                // Above causes recusive call to this method.

                //--------------------------------------------------------------------------//

                AppDomain.Unload(newDomain);
                Environment.ExitCode = ret;
                // We get here when the new app domain we created is shutdown.  Shutdown the 
                // original default app domain (to avoid running app again there):
                // We could use Shutdown(0) but we have to remove the main window uri from xaml
                // and then set it for new app domain (above execute command) using:
                // StartupUri = new Uri("Window1.xaml", UriKind.Relative);
                Environment.Exit(0);
                return;
            }
        }*/
    }
}
