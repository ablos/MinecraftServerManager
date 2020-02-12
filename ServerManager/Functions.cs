using Microsoft.VisualBasic.Devices;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace ServerManager
{
    public static class Functions
    {
        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static ulong GetComputerRAM()
        {
            return new ComputerInfo().TotalPhysicalMemory / 1024 / 1024 / 1024;
        }

        [Serializable]
        private class SettingsClass
        {
            public int memSize;
            public bool useNGROK;
            public string customIP;
            public string localPort;
            public string serverDirectory;
            public string serverFileName;
            public string ngrokDirectory;
        }

        public static void SaveSettings()
        {
            SettingsClass settings = new SettingsClass();
            settings.memSize = Settings.memSize;
            settings.useNGROK = Settings.useNGROK;
            settings.customIP = Settings.customIP;
            settings.localPort = Settings.localPort;
            settings.serverDirectory = Settings.serverDirectory;
            settings.serverFileName = Settings.serverFileName;
            settings.ngrokDirectory = Settings.ngrokDirectory;

            using (Stream stream = File.Create("server.settings"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, settings);
            }
        }

        public static void LoadSettings()
        {
            if (!File.Exists("server.settings"))
                return;

            using (Stream stream = File.Open("server.settings", FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                SettingsClass settings = (SettingsClass)formatter.Deserialize(stream);

                Settings.memSize = settings.memSize;
                Settings.useNGROK = settings.useNGROK;
                Settings.customIP = settings.customIP;
                Settings.localPort = settings.localPort;
                Settings.serverDirectory = settings.serverDirectory;
                Settings.serverFileName = settings.serverFileName;
                Settings.ngrokDirectory = settings.ngrokDirectory;
            }
        }

        public static FileInfo[] GetServerFiles(DirectoryInfo dir)
        {
            return dir.GetFiles("*", SearchOption.AllDirectories);
        }

        public static string RemoveLast(this string text, string character)
        {
            if (text.Length < 1) return text;
            return text.Remove(text.ToString().LastIndexOf(character), character.Length);
        }

        public static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// Sets the progress bar value, without using 'Windows Aero' animation.
        /// This is to work around a known WinForms issue where the progress bar 
        /// is slow to update. 
        /// </summary>
        public static void SetProgressNoAnimation(this ProgressBar pb, int value)
        {
            try
            {
                // To get around the progressive animation, we need to move the 
                // progress bar backwards.
                if (value == pb.Maximum)
                {
                    // Special case as value can't be set greater than Maximum.
                    pb.Maximum = value + 1;     // Temporarily Increase Maximum
                    pb.Value = value + 1;       // Move past
                    pb.Maximum = value;         // Reset maximum
                }
                else
                {
                    pb.Value = value + 1;       // Move past
                }
                pb.Value = value;               // Move to correct value
            }
            catch { }
        }
    }
}
