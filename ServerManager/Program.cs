using System;
using System.Reflection;
using System.Windows.Forms;

namespace ServerManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Load DLLs
            EmbeddedAssembly.Load("ServerManager.lib.Google.Apis.dll", "Google.Apis.dll");
            EmbeddedAssembly.Load("ServerManager.lib.Google.Apis.Auth.dll", "Google.Apis.Auth.dll");
            EmbeddedAssembly.Load("ServerManager.lib.Google.Apis.Auth.PlatformServices.dll", "Google.Apis.Auth.PlatformServices.dll");
            EmbeddedAssembly.Load("ServerManager.lib.Google.Apis.Core.dll", "Google.Apis.Core.dll");
            EmbeddedAssembly.Load("ServerManager.lib.Google.Apis.Drive.v3.dll", "Google.Apis.Drive.v3.dll");
            EmbeddedAssembly.Load("ServerManager.lib.Google.Apis.PlatformServices.dll", "Google.Apis.PlatformServices.dll");
            EmbeddedAssembly.Load("ServerManager.lib.Newtonsoft.Json.dll", "Newtonsoft.Json.dll");
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            // Load settings
            Functions.LoadSettings();

            // Run application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SyncForm(true));
            Application.Run(new ServerForm());
            if (!Settings.hasCompletedUpload)
                Application.Run(new SyncForm(false));
        }

        // When app can't find DLL it will be found here
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
