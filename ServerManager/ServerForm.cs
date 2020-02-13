using System;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.ComponentModel;

namespace ServerManager
{
    public partial class ServerForm : Form
    {
        bool hasIp = false;
        bool serverRunning = false;
        StreamWriter serverInput = null;
        List<string> onlinePlayers = new List<string>();
        Process server;
        int ramUpdateTimeInterval = 500;
        int copyRespondTime = 1000;
        string lastCommand = "";
        string ip;

        ComponentResourceManager resources;

        private static string[] Scopes = { DriveService.Scope.Drive };
        private static string ApplicationName = "MinecraftServer";
        private DriveService driveService;
        private Google.Apis.Drive.v3.Data.File serverIpFile;

        Thread updateRamThread;
        ToolTip playerNames = new ToolTip();
        ToolTip ipCopyTip = new ToolTip();

        public ServerForm()
        {
            resources = new ComponentResourceManager(typeof(ServerForm));

            InitializeComponent();

            this.BackColor = Color.FromArgb(28, 28, 28);
            playerNames.ShowAlways = true;
            playerNames.Active = false;

            ipCopyTip.ShowAlways = true;
            ipCopyTip.Active = false;
            ipCopyTip.SetToolTip(ipLabel, "Click to copy");

            ramLabel.Text = "0/" + (Settings.memSize * 1024) + " MB RAM";

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
            driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        private void IpLabel_Click(object sender, EventArgs e)
        {
            if (hasIp)
            {
                Clipboard.SetText(ipLabel.Text);

                new Thread(() =>
                {
                    ipLabel.Invoke((MethodInvoker)(() => ipLabel.Text = "Copied!"));
                    Thread.Sleep(copyRespondTime);
                    ipLabel.Invoke((MethodInvoker)(() => ipLabel.Text = ip));
                }).Start();
            }
        }

        private async void StartButton_Click(object sender, EventArgs e)
        {
            if (statusLabel.Text != "Offline")
                return;

            serverConsole.Clear();

            if (!System.IO.File.Exists(Settings.ngrokDirectory + "\\ngrok.exe"))
            {
                serverConsole.AppendText("[ERROR] ngrok.exe was not found in the given path! Please check the settings to make sure the path is correct.", Color.Red);
                return;
            }

            if (!System.IO.File.Exists(Settings.serverDirectory + "\\" + Settings.serverFileName))
            {
                serverConsole.AppendText("[ERROR] Server file with name '" + Settings.serverFileName + "' was not found in the given path! Please check the settings to make sure the path and filename are correct!", Color.Red);
                return;
            }

            statusLabel.Text = "Starting...";
            statusLabel.ForeColor = Color.Orange;
            this.Text = "Server Console - Starting...";
            this.Icon = Properties.Resources.server_loading;
            
            FilesResource.ListRequest listRequest = driveService.Files.List();
            listRequest.Q = "name contains 'server-ip='";
            listRequest.Fields = "files(id, name)";
            FileList result = await listRequest.ExecuteAsync();

            if (result != null && result.Files.Count > 0)
            {
                AlreadyRunningForm arf = new AlreadyRunningForm(result.Files[0].Name.Replace("server-ip=", ""));
                if (arf.ShowDialog() == DialogResult.No)
                {
                    statusLabel.Text = "Offline";
                    statusLabel.ForeColor = Color.Red;
                    this.Text = "Server Console - Offline";
                    this.Icon = Properties.Resources.server_offline;
                    return;
                }

                foreach (Google.Apis.Drive.v3.Data.File file in result.Files)
                {
                    await Task.Run(() => driveService.Files.Delete(file.Id).Execute() );
                }
            }

            Settings.hasCompletedUpload = false;
            Functions.SaveSettings();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                if (!System.IO.File.Exists(Settings.ngrokDirectory + "\\OpenTunnel.bat"))
                {
                    StreamWriter sw = new StreamWriter(Settings.ngrokDirectory + "\\OpenTunnel.bat");

                    sw.WriteLine("ngrok tcp -region eu %1");
                    sw.Close();
                }

                Process ipRetriever = new Process();
                ipRetriever.StartInfo.FileName = Settings.ngrokDirectory + "\\OpenTunnel.bat";
                if (string.IsNullOrEmpty(Settings.ngrokDirectory))
                    ipRetriever.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                else
                    ipRetriever.StartInfo.WorkingDirectory = Settings.ngrokDirectory;
                ipRetriever.StartInfo.Arguments = Settings.localPort;
                ipRetriever.StartInfo.RedirectStandardOutput = true;
                ipRetriever.StartInfo.UseShellExecute = false;
                ipRetriever.StartInfo.CreateNoWindow = true;
                ipRetriever.OutputDataReceived += new DataReceivedEventHandler((_sender, args) =>
                {
                    try
                    {
                        WebClient client = new WebClient();
                        ip = client.DownloadString("http://127.0.0.1:4040/api/tunnels").Split(new string[] { "tcp://" }, StringSplitOptions.None)[1].Split('"')[0];
                        ipLabel.Invoke((MethodInvoker)(() => ipLabel.Text = ip));
                        hasIp = true;
                        ipCopyTip.Active = true;

                        if (serverIpFile == null)
                        {
                            Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
                            body.Name = "server-ip=" + ip;
                            body.Description = "File to store the currently active server IP";

                            FilesResource.CreateRequest request = driveService.Files.Create(body);
                            request.Fields = "id";
                            serverIpFile = request.Execute();
                        }
                    }catch { }
                });

                ipRetriever.Start();
                ipRetriever.BeginOutputReadLine();
                ipRetriever.WaitForExit();
                ipRetriever.Close();
            }).Start();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                string memory = Settings.memSize.ToString() + "G";

                server = new Process();
                server.StartInfo.FileName = "java.exe";
                if (string.IsNullOrEmpty(Settings.serverDirectory))
                    server.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                else
                    server.StartInfo.WorkingDirectory = Settings.serverDirectory;
                server.StartInfo.Arguments = "-server -Xmx" + memory + " -XX:+UseG1GC -Xms" + memory + " -Dsun.rmi.dgc.server.gcInterval=2147483646 -XX:+UnlockExperimentalVMOptions -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M -Dfml.queryResult=confirm -jar \"" + Settings.serverFileName + "\" nogui";//Settings.memSize + " \"" + Settings.serverFileName + "\"";
                server.StartInfo.RedirectStandardOutput = true;
                server.StartInfo.RedirectStandardInput = true;
                server.StartInfo.UseShellExecute = false;
                server.StartInfo.CreateNoWindow = true;
                server.OutputDataReceived += new DataReceivedEventHandler((_sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        if (args.Data.Contains("[Server thread/WARN]") || args.Data.Contains("[main/WARN]"))
                        {
                            serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(args.Data, Color.Orange)));
                            serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(Environment.NewLine)));
                        }
                        else if (args.Data.Contains("[Server thread/ERROR]") || args.Data.Contains("[main/ERROR]"))
                        {
                            serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(args.Data, Color.Red)));
                            serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(Environment.NewLine)));

                            if (args.Data.Contains("crash"))
                            {
                                statusLabel.Invoke((MethodInvoker)(() => statusLabel.Text = "Offline"));
                                statusLabel.Invoke((MethodInvoker)(() => statusLabel.ForeColor = Color.Red));
                                this.Invoke((MethodInvoker)(() => this.Text = "Server Console - Offline"));

                                serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText("[CRASH] The server crashed. Check the crash report.", Color.Red)));
                                serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(Environment.NewLine)));

                                server.Kill();
                            }
                        }
                        else if (args.Data.Contains("[INDICATOR FOR SERVER MANAGER THAT SERVER HAS STARTED]"))
                        {
                            statusLabel.Invoke((MethodInvoker)(() => statusLabel.Text = "Online"));
                            statusLabel.Invoke((MethodInvoker)(() => statusLabel.ForeColor = Color.Green));
                            this.Invoke((MethodInvoker)(() => this.Text = "Server Console - Online"));
                            this.Invoke((MethodInvoker)(() => this.Icon = Properties.Resources.server_online));

                            serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText("Server done loading!", Color.Green)));
                            serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(Environment.NewLine)));
                        }
                        else
                        {
                            serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(args.Data)));
                            serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(Environment.NewLine)));
                        }

                        if (args.Data.Contains("[Server thread/INFO] [icbmclassic]: Stopping threads"))
                        {
                            statusLabel.Invoke((MethodInvoker)(() => statusLabel.Text = "Stopping..."));
                            statusLabel.Invoke((MethodInvoker)(() => statusLabel.ForeColor = Color.Orange));
                            this.Invoke((MethodInvoker)(() => this.Text = "Server Console - Stopping..."));
                            this.Invoke((MethodInvoker)(() => this.Icon = Properties.Resources.server_loading));
                        }

                        if (args.Data.Contains("joined the game") && args.Data.Contains("[Server thread/INFO] [minecraft/DedicatedServer]") && args.Data.Split(' ').Length == 8 && !args.Data.Split(' ')[4].Contains("<"))
                        {
                            onlinePlayers.Add(args.Data.Split(' ')[4]);
                            UpdateOnlinePlayers();
                        }

                        if (args.Data.Contains("left the game") && args.Data.Contains("[Server thread/INFO] [minecraft/DedicatedServer]") && args.Data.Split(' ').Length == 8 && !args.Data.Split(' ')[4].Contains("<"))
                        {
                            onlinePlayers.Remove(args.Data.Split(' ')[4]);
                            UpdateOnlinePlayers();
                        }
                    }
                });

                server.Start();
                server.BeginOutputReadLine();
                serverInput = server.StandardInput;
                serverInput.AutoFlush = true;
                serverRunning = true;
                CreateRAMCheckThread();
                updateRamThread.Start();
                SendCommand("say [INDICATOR FOR SERVER MANAGER THAT SERVER HAS STARTED]", false);
                server.WaitForExit();
                serverRunning = false;
                ramLabel.Invoke((MethodInvoker)(() => ramLabel.Text = "0/" + (Settings.memSize * 1024) + " MB RAM"));
                serverInput.Close();
                server.Close();
                server.Dispose();
                statusLabel.Invoke((MethodInvoker)(() => statusLabel.Text = "Offline"));
                statusLabel.Invoke((MethodInvoker)(() => statusLabel.ForeColor = Color.Red));
                this.Invoke((MethodInvoker)(() => this.Text = "Server Console - Offline"));
                this.Invoke((MethodInvoker)(() => this.Icon = Properties.Resources.server_offline));

                serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText("Server stopped.", Color.Red)));
                serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(Environment.NewLine)));

                onlinePlayers.Clear();
                UpdateOnlinePlayers();
            }).Start();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (statusLabel.Text != "Online")
                return;

            SendCommand("stop");
        }

        private void SendCommand(string command, bool showInConsole = true)
        {
            command = command.Trim();

            if (string.IsNullOrEmpty(command) || !serverRunning)
                return;

            if (showInConsole)
            {
                serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(command)));
                serverConsole.Invoke((MethodInvoker)(() => serverConsole.AppendText(Environment.NewLine)));
                lastCommand = command;
            }

            serverInput.WriteLine(command);
            commandBox.Invoke((MethodInvoker)(() => commandBox.Clear()));
        }

        private void UpdateOnlinePlayers()
        {
            playersLabel.Invoke((MethodInvoker)(() => playersLabel.Text = onlinePlayers.Count.ToString() + " online players"));

            if (onlinePlayers.Count > 0)
            {
                string players = "";

                for (int i = 0; i < onlinePlayers.Count; i++)
                {
                    if (i > 20)
                    {
                        players += "and " + (onlinePlayers.Count - (i + 1)).ToString() + " more...";
                    }

                    players += onlinePlayers[i] + "\n";
                }

                playersLabel.Invoke((MethodInvoker)(() => playerNames.SetToolTip(playersLabel, players)));
                playerNames.Active = true;
            }else
            {
                playerNames.Active = false;
            }
        }

        private void CreateRAMCheckThread()
        {
            updateRamThread = new Thread(() =>
            {
                PerformanceCounter pc = ProcessCounter.GetPerfCounterForProcessId(server.Id, "Working Set - Private");
                pc.CategoryName = "Process";
                pc.CounterName = "Working Set - Private";

                while (serverRunning)
                {
                    try
                    {
                        ramLabel.Invoke((MethodInvoker)(() => ramLabel.Text = ((int)Math.Round(pc.NextValue() / 1024 / 1024)).ToString() + "/" + (Settings.memSize * 1024) + " MB RAM"));
                        Thread.Sleep(ramUpdateTimeInterval);
                    }catch { }
                }

                pc.Close();
                pc.Dispose();
            });
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendCommand(commandBox.Text);
        }

        private void CommandBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for enter button
            if (e.KeyChar == (char)13)
            {
                SendCommand(commandBox.Text);
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (statusLabel.Text != "Offline")
            {
                MessageBox.Show("Please shut down the server before closing this program to prevent data corruption.", "Closing error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }

            try
            {
                Process.GetProcessesByName("ngrok.exe")[0].Kill();
            }
            catch { }
        }

        private void CommandBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Back) && e.Control)
            {
                e.SuppressKeyPress = true;
                int selStart = commandBox.SelectionStart;
                while (selStart > 0 && commandBox.Text.Substring(selStart - 1, 1) == " ")
                {
                    selStart--;
                }
                int prevSpacePos = -1;
                if (selStart != 0)
                {
                    prevSpacePos = commandBox.Text.LastIndexOf(' ', selStart - 1);
                }
                commandBox.Select(prevSpacePos + 1, commandBox.SelectionStart - prevSpacePos - 1);
                commandBox.SelectedText = "";
            }
            else if (e.KeyCode == Keys.Up && !string.IsNullOrEmpty(lastCommand))
            {
                commandBox.Invoke((MethodInvoker)(() => commandBox.Text = lastCommand));
                commandBox.Invoke((MethodInvoker)(() => commandBox.SelectionStart = commandBox.Text.Length));
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            if (statusLabel.Text != "Offline")
            {
                MessageBox.Show("You can only edit the settings when the server is not running.", "Settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open settingsform as dialog here
            SettingsForm settings = new SettingsForm();
            settings.ShowInTaskbar = false;
            settings.ShowDialog();

            ramLabel.Text = "0/" + (Settings.memSize * 1024) + " MB RAM";
        }
    }
}
