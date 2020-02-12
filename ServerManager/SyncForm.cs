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
        private bool syncCompleted = false;
        private bool isPaused = false;
        private bool isClosing = false;
        private static string[] Scopes = { DriveService.Scope.Drive };
        private static string ApplicationName = "MinecraftServer";

        // Any path to a file that contains one of these strings, won't be uploaded nor downloaded.
        private static string[] excludedFiles =
        {
            "\\backups\\",
            "\\logs\\",
            "\\.mixin.out\\",
            "\\crash-reports\\",
            "server.settings",
            "credentials.json",
            "\\token.json\\",
            "server-ip="
        };

        public SyncForm(bool download = true)
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

            if (download)
                this.Text = "Downloading server...";
            else
                this.Text = "Uploading server...";
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

            UpdateStatusLabel("Authenticating Google Drive...");

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

            UpdateProgressBar(progressBar.Maximum);
            UpdateStatusLabel("Authenticated Google Drive.");

            // Check here if server is already running

            List<Google.Apis.Drive.v3.Data.File> driveFiles = await Task.Run(async () => await getDriveFiles(service));
            FileInfo[] serverFiles = Functions.GetServerFiles(new DirectoryInfo(serverDirectory));

            if (download)
                await Task.Run(async () => await DownloadSync(service, driveFiles, serverDirectory));
            else
                await Task.Run(async () => await UploadSync(service, driveFiles, serverFiles, serverDirectory));

            UpdateSubStatusLabel("");
            UpdateSubProgressBar(subProgressBar.Maximum);
            UpdateProgressBar(progressBar.Maximum);
            UpdateStatusLabel("Sync completed.");
            syncCompleted = true;

            this.Close();
        }

        private async Task DownloadSync(DriveService service, List<Google.Apis.Drive.v3.Data.File> driveFiles, string serverDirectory)
        {
            UpdateProgressBar(0);
            SetProgressBarMaximum(driveFiles.Count);
            UpdateStatusLabel("Scanning files...");

            bool changesMade = false;

            int i = 0;
            foreach (Google.Apis.Drive.v3.Data.File driveFile in driveFiles)
            {
                while (isPaused) { }

                if (!isExcluded(driveFile.Name))
                {
                    UpdateSubProgressBar(0);
                    UpdateSubStatusLabel("Scanning for: " + driveFile.Name);
                    if (System.IO.File.Exists(serverDirectory + driveFile.Name))
                    {
                        UpdateSubProgressBar(subProgressBar.Maximum);
                        UpdateSubStatusLabel("Found: " + driveFile.Name);
                        if (System.IO.File.GetLastWriteTime(serverDirectory + driveFile.Name) < driveFile.ModifiedTime)
                        {
                            UpdateSubProgressBar(0);
                            UpdateSubStatusLabel("Updating: " + driveFile.Name);

                            System.IO.File.Delete(serverDirectory + driveFile.Name);

                            UpdateSubProgressBar(subProgressBar.Maximum);
                            UpdateSubStatusLabel("Deleted: " + driveFile.Name);

                            await Task.Run(() => downloadFile(service, driveFile, serverDirectory));
                            changesMade = true;
                        }
                    }
                    else
                    {
                        await Task.Run(() => downloadFile(service, driveFile, serverDirectory));
                        changesMade = true;
                    }

                    UpdateProgressBar(++i);
                }
            }

            UpdateProgressBar(progressBar.Maximum);
            UpdateStatusLabel("Files scanned.");

            if (changesMade)
                await UpdateLocalFiles(service, serverDirectory);
        }

        private void downloadFile(DriveService service, Google.Apis.Drive.v3.Data.File driveFile, string serverDirectory)
        {
            // Make sure all directories exist before downloading
            Directory.CreateDirectory(Path.GetDirectoryName(serverDirectory + driveFile.Name));

            FilesResource.GetRequest getRequest = service.Files.Get(driveFile.Id);
            //getRequest.AcknowledgeAbuse = true;
            MemoryStream stream = new MemoryStream();
            UpdateSubStatusLabel("Downloading: " + driveFile.Name);
            SetSubProgressBarMaximum((int)driveFile.Size);
            UpdateSubProgressBar(0);
            getRequest.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case Google.Apis.Download.DownloadStatus.Downloading:
                        UpdateSubProgressBar((int)progress.BytesDownloaded);
                        break;
                    case Google.Apis.Download.DownloadStatus.Completed:
                        UpdateSubProgressBar(subProgressBar.Maximum);
                        UpdateSubStatusLabel("Downloaded: " + driveFile.Name);
                        SaveStream(stream, serverDirectory + driveFile.Name);
                        Thread.Sleep(25);
                        break;
                    case Google.Apis.Download.DownloadStatus.Failed:
                        Console.WriteLine("Download failed.");
                        Console.WriteLine(progress.Exception.Message);
                        break;
                }
            };

            getRequest.Download(stream);
        }

        private void SaveStream(MemoryStream stream, string saveTo)
        {
            using (FileStream file = new FileStream(saveTo, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(file);
            }
        }

        private async Task UploadSync(DriveService service, List<Google.Apis.Drive.v3.Data.File> driveFiles, FileInfo[] serverFiles, string serverDirectory)
        {
            // Replace all modified files
            UpdateProgressBar(0);
            SetProgressBarMaximum(serverFiles.Length);
            UpdateStatusLabel("Scanning files...");
            UpdateSubProgressBar(0);

            bool changesMade = false;
            int i = 0;
            foreach (FileInfo serverFile in serverFiles)
            {
                while (isPaused) { }

                if (!isExcluded(serverFile.FullName))
                {
                    UpdateSubProgressBar(0);
                    UpdateSubStatusLabel("Scanning for: " + serverFile.FullName.Replace(serverDirectory, ""));

                    bool foundFile = false;
                    for (int x = 0; x < driveFiles.Count; x++)
                    {
                        if (driveFiles[x].Name.Trim() == serverFile.FullName.Replace(serverDirectory, "").Trim())
                        {
                            UpdateSubProgressBar(subProgressBar.Maximum);
                            UpdateSubStatusLabel("Found: " + driveFiles[x].Name);

                            if (driveFiles[x].ModifiedTime < serverFile.LastWriteTime)
                            {
                                UpdateSubProgressBar(0);
                                UpdateSubStatusLabel("Updating: " + driveFiles[x].Name);

                                await Task.Run(() => deleteFile(service, driveFiles[x]));
                                await Task.Run(() => uploadFile(service, serverFile.FullName, serverDirectory));

                                foundFile = true;
                                changesMade = true;
                                driveFiles.RemoveAt(x);
                                break;
                            }

                            driveFiles.RemoveAt(x);
                            foundFile = true;
                        }
                    }

                    if (!foundFile)
                    {
                        UpdateSubProgressBar(progressBar.Maximum);
                        UpdateSubStatusLabel("Couldn't find file: " + serverFile.FullName.Replace(serverDirectory, ""));
                        await Task.Run(() => uploadFile(service, serverFile.FullName, serverDirectory));
                        changesMade = true;
                    }
                }

                UpdateProgressBar(++i);
            }

            UpdateStatusLabel("Files scanned.");
            UpdateProgressBar(progressBar.Maximum);

            // Delete all files that don't exist in the local directory anymore
            i = 0;
            UpdateProgressBar(0);
            SetProgressBarMaximum(driveFiles.Count);
            UpdateStatusLabel("Deleting files...");

            foreach (Google.Apis.Drive.v3.Data.File driveFile in driveFiles)
            {
                while (isPaused) { }

                await Task.Run(() => deleteFile(service, driveFile));
                UpdateProgressBar(++i);
            }

            UpdateProgressBar(progressBar.Maximum);
            UpdateStatusLabel("Deleted files.");

            if (changesMade)
                await UpdateLocalFiles(service, serverDirectory);
        }

        private async Task UpdateLocalFiles(DriveService service, string serverDirectory)
        {
            UpdateSubProgressBar(0);
            UpdateSubStatusLabel("");

            List<Google.Apis.Drive.v3.Data.File> driveFiles = await getDriveFiles(service);

            UpdateProgressBar(0);
            SetProgressBarMaximum(driveFiles.Count);
            UpdateStatusLabel("Updating local files...");

            int i = 0;
            foreach (Google.Apis.Drive.v3.Data.File driveFile in driveFiles)
            {
                while (isPaused) { }

                UpdateSubProgressBar(0);
                UpdateSubStatusLabel("Updating: " + driveFile.Name);

                System.IO.File.SetLastWriteTime(serverDirectory + driveFile.Name, driveFile.ModifiedTime.Value);

                UpdateSubProgressBar(subProgressBar.Maximum);
                UpdateSubStatusLabel("Updated: " + driveFile.Name);
                UpdateProgressBar(++i);
            }

            UpdateProgressBar(progressBar.Maximum);
            UpdateStatusLabel("Local files updated.");
        }

        private void deleteFile(DriveService service, Google.Apis.Drive.v3.Data.File file)
        {
            UpdateSubProgressBar(0);
            UpdateSubStatusLabel("Deleting: " + file.Name);
            FilesResource.DeleteRequest delReq = service.Files.Delete(file.Id);
            delReq.Execute();

            UpdateSubProgressBar(subProgressBar.Maximum);
            UpdateSubStatusLabel("Deleted: " + file.Name);
            Thread.Sleep(25);
        }

        private static bool isExcluded(string pathToFile)
        {
            foreach (string s in excludedFiles)
            {
                if (pathToFile.Contains(s))
                    return true;
            }

            return false;
        }

        private void uploadFile(DriveService _service, string _uploadFile, string serverDirectory, string _descrp = "Uploaded with .NET!")
        {
            if (System.IO.File.Exists(_uploadFile))
            {
                UpdateSubStatusLabel("Uploading: " + _uploadFile.Replace(serverDirectory, ""));
                UpdateSubProgressBar(0);

                Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                body.Name = _uploadFile.Replace(serverDirectory, "");
                body.Description = _descrp;
                body.MimeType = Functions.GetMimeType(_uploadFile);
                body.Parents = null;

                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                MemoryStream stream = new MemoryStream(byteArray);

                SetSubProgressBarMaximum(byteArray.Length);

                try
                {
                    FilesResource.CreateMediaUpload request = _service.Files.Create(body, stream, Functions.GetMimeType(_uploadFile));

                    request.ProgressChanged += (Google.Apis.Upload.IUploadProgress progress) =>
                    {
                        switch (progress.Status)
                        {
                            case Google.Apis.Upload.UploadStatus.Uploading:
                                UpdateSubProgressBar((int)progress.BytesSent);
                                break;
                            case Google.Apis.Upload.UploadStatus.Completed:
                                UpdateSubProgressBar(subProgressBar.Maximum);
                                UpdateSubStatusLabel("Uploaded: " + _uploadFile.Replace(serverDirectory, ""));
                                Thread.Sleep(25);
                                break;
                            case Google.Apis.Upload.UploadStatus.Failed:
                                Console.WriteLine("Upload Failed");
                                Console.WriteLine(progress.Exception.Message);
                                break;
                        }
                    };

                    request.Upload();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error Occured");
                }
            }
            else
            {
                UpdateSubProgressBar(0);
                UpdateSubStatusLabel("File doesn't exist: " + _uploadFile.Replace(serverDirectory, ""));
            }
        }

        private async Task<List<Google.Apis.Drive.v3.Data.File>> getDriveFiles(DriveService service)
        {
            List<Google.Apis.Drive.v3.Data.File> driveFiles = new List<Google.Apis.Drive.v3.Data.File>();

            UpdateProgressBar(0);
            UpdateStatusLabel("Getting file list from Google Drive...");

            FileList result = null;
            while (true)
            {
                while (isPaused) { }

                if (result != null && string.IsNullOrWhiteSpace(result.NextPageToken))
                    break;

                FilesResource.ListRequest listRequest = service.Files.List();
                listRequest.PageSize = 1000;
                listRequest.Fields = "nextPageToken, files(id, name, modifiedTime, size)";
                if (result != null)
                    listRequest.PageToken = result.NextPageToken;

                result = await listRequest.ExecuteAsync();
                driveFiles.AddRange(result.Files);
            }

            UpdateProgressBar(progressBar.Maximum);
            UpdateStatusLabel("Retrieved file list from Google Drive.");

            return driveFiles;
        }

        private void UpdateStatusLabel(string text)
        {
            statusLabel.Invoke((MethodInvoker)(() => statusLabel.Text = text));
            statusLabel.Invoke((MethodInvoker)(() => statusLabel.Update()));
        }

        private void UpdateSubStatusLabel(string text)
        {
            subStatusLabel.Invoke((MethodInvoker)(() => subStatusLabel.Text = text));
            subStatusLabel.Invoke((MethodInvoker)(() => subStatusLabel.Update()));
        }

        private void UpdateProgressBar(int value)
        {
            progressBar.Invoke((MethodInvoker)(() => progressBar.SetProgressNoAnimation(value)));
            progressBar.Invoke((MethodInvoker)(() => progressBar.Update()));
        }

        private void SetProgressBarMaximum(int maximum)
        {
            progressBar.Invoke((MethodInvoker)(() => progressBar.Maximum = maximum));
        }

        private void UpdateSubProgressBar(int value)
        {
            subProgressBar.Invoke((MethodInvoker)(() => subProgressBar.SetProgressNoAnimation(value)));
            subProgressBar.Invoke((MethodInvoker)(() => subProgressBar.Update()));
        }

        private void SetSubProgressBarMaximum(int maximum)
        {
            subProgressBar.Invoke((MethodInvoker)(() => subProgressBar.Maximum = maximum));
        }

        private void SyncForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!syncCompleted && !isClosing)
            {
                isPaused = true;

                DialogResult result = MessageBox.Show("Are you sure you want to cancel synchronization?", "Closing warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    isPaused = false;
                    e.Cancel = true;
                }else
                {
                    isClosing = true;
                    Application.Exit();
                }
            }
        }
    }
}