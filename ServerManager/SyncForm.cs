using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;

namespace ServerManager
{
    public partial class SyncForm : Form
    {
        private bool download = true;
        private static string[] Scopes = { DriveService.Scope.Drive };
        private static string ApplicationName = "MinecraftServer";

        private static string[] excludedFolders = { "\\backups\\", "\\logs\\", "\\.mixin.out\\", "\\crash-reports\\", "server.settings", "credentials.json", "\\token.json\\" };

        public SyncForm(bool download = false)
        {
            this.download = download;

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            InitializeComponent();
        }

        private void SyncForm_Load(object sender, EventArgs e)
        {
            // No server directory set yet...
            if (!System.IO.File.Exists("server.settings"))
            {
                DialogResult dr = MessageBox.Show("No path to server directory has been set. Would you like to set the current directory as the server directory?", "No path set", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr != DialogResult.Yes)
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                    {
                        DialogResult result = fbd.ShowDialog();

                        if (result == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                        {
                            Settings.serverDirectory = fbd.SelectedPath;
                        }
                    }
                }

                Functions.SaveSettings();

                this.WindowState = FormWindowState.Minimized;
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }

        private async void SyncForm_Shown(object sender, EventArgs e)
        {
            // Get serverdirectory
            string serverDirectory;
            if (string.IsNullOrEmpty(Settings.serverDirectory))
                serverDirectory = Environment.CurrentDirectory;
            else
                serverDirectory = Settings.serverDirectory;

            // Authorize
            UserCredential credential;
            using (FileStream stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Drive API service.
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });


            List<Google.Apis.Drive.v3.Data.File> driveFiles = await getDriveFiles(service);
            FileInfo[] serverFiles = Functions.GetServerFiles(new DirectoryInfo(serverDirectory));

            if (download)
                DownloadSync(service, driveFiles, serverDirectory);
            else
                UploadSync(service, driveFiles, serverFiles, serverDirectory);

            Console.WriteLine("Sync completed.");
        }

        private async void DownloadSync(DriveService service, List<Google.Apis.Drive.v3.Data.File> driveFiles, string serverDirectory)
        {
            foreach (Google.Apis.Drive.v3.Data.File driveFile in driveFiles)
            {
                if (System.IO.File.Exists(serverDirectory + driveFile.Name))
                {
                    if (System.IO.File.GetLastWriteTime(serverDirectory + driveFile.Name) < driveFile.ModifiedTime)
                    {
                        System.IO.File.Delete(serverDirectory + driveFile.Name);
                        await downloadFile(service, driveFile, serverDirectory);
                    }
                }else
                {
                    await downloadFile(service, driveFile, serverDirectory);
                }
            }
        }

        private async Task downloadFile(DriveService service, Google.Apis.Drive.v3.Data.File driveFile, string serverDirectory)
        {
            // Make sure all directories exist before downloading
            Directory.CreateDirectory(Path.GetDirectoryName(serverDirectory + driveFile.Name));

            FilesResource.GetRequest getRequest = service.Files.Get(driveFile.Id);
            MemoryStream stream = new MemoryStream();
            getRequest.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case Google.Apis.Download.DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case Google.Apis.Download.DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            string saveto = Path.GetTempPath() + Guid.NewGuid().ToString() + Path.GetExtension(driveFile.Name);
                            SaveStream(stream, serverDirectory + driveFile.Name);
                            break;
                        }
                    case Google.Apis.Download.DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };

            await getRequest.DownloadAsync(stream);
        }

        private static void SaveStream(MemoryStream stream, string saveTo)
        {
            using (FileStream file = new FileStream(saveTo, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(file);
            }
        }

        private async void UploadSync(DriveService service, List<Google.Apis.Drive.v3.Data.File> driveFiles, FileInfo[] serverFiles, string serverDirectory)
        {
            // Replace all modified files
            bool changesMade = false;
            foreach (FileInfo serverFile in serverFiles)
            {
                if (!isExcluded(serverFile.FullName))
                {
                    bool foundFile = false;
                    for (int i = 0; i < driveFiles.Count; i++)
                    {
                        if (driveFiles[i].Name.Trim() == serverFile.FullName.Replace(serverDirectory, "").Trim())
                        {
                            if (driveFiles[i].ModifiedTime < serverFile.LastAccessTime)
                            {
                                await deleteFile(service, driveFiles[i].Id);
                                await uploadFile(service, serverFile.FullName, serverDirectory);

                                foundFile = true;
                                changesMade = true;
                                driveFiles.RemoveAt(i);
                                break;
                            }

                            driveFiles.RemoveAt(i);
                            foundFile = true;
                        }
                    }

                    if (!foundFile)
                    {
                        await uploadFile(service, serverFile.FullName, serverDirectory);
                        changesMade = true;
                    }
                }
            }

            // Delete all files that don't exist in the local directory anymore
            foreach (Google.Apis.Drive.v3.Data.File driveFile in driveFiles)
            {
                await deleteFile(service, driveFile.Id);
            }

            if (!changesMade)
                return;

            driveFiles = await getDriveFiles(service);

            foreach (Google.Apis.Drive.v3.Data.File driveFile in driveFiles)
            {
                System.IO.File.SetLastWriteTime(serverDirectory + driveFile.Name, driveFile.ModifiedTime.Value);
            }
        }

        private static async Task deleteFile(DriveService service, string fileID)
        {
            FilesResource.DeleteRequest delReq = service.Files.Delete(fileID);
            await delReq.ExecuteAsync();
        }

        private static bool isExcluded(string pathToFile)
        {
            foreach (string s in excludedFolders)
            {
                if (pathToFile.Contains(s))
                    return true;
            }

            return false;
        }

        private static async Task uploadFile(DriveService _service, string _uploadFile, string serverDirectory, string _descrp = "Uploaded with .NET!")
        {
            if (System.IO.File.Exists(_uploadFile))
            {
                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                body.Name = _uploadFile.Replace(serverDirectory, "");
                body.Description = _descrp;
                body.MimeType = Functions.GetMimeType(_uploadFile);
                body.Parents = null;

                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                MemoryStream stream = new MemoryStream(byteArray);
                try
                {
                    FilesResource.CreateMediaUpload request = _service.Files.Create(body, stream, Functions.GetMimeType(_uploadFile));
                    await request.UploadAsync();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error Occured");
                }
            }
            else
            {
                MessageBox.Show("The file does not exist.", "404");
            }
        }

        private static async Task<List<Google.Apis.Drive.v3.Data.File>> getDriveFiles(DriveService service)
        {
            List<Google.Apis.Drive.v3.Data.File> driveFiles = new List<Google.Apis.Drive.v3.Data.File>();

            FileList result = null;
            while (true)
            {
                if (result != null && string.IsNullOrWhiteSpace(result.NextPageToken))
                    break;

                FilesResource.ListRequest listRequest = service.Files.List();
                listRequest.PageSize = 1000;
                listRequest.Fields = "nextPageToken, files(id, name, modifiedTime)";
                if (result != null)
                    listRequest.PageToken = result.NextPageToken;

                result = await listRequest.ExecuteAsync();
                driveFiles.AddRange(result.Files);
            }

            return driveFiles;
        }
    }
}