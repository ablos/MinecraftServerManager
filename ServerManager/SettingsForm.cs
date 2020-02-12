using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace ServerManager
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            this.BackColor = Color.FromArgb(28, 28, 28);
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            memorySelection.Maximum = Functions.GetComputerRAM() - 1;
            textBox1.Select();

            memorySelection.Value = Settings.memSize;
            useNGROKBox.SelectedIndex = Settings.useNGROK ? 0 : 1;
            customIPTextBox.Text = Settings.customIP;
            localPortBox.Text = Settings.localPort;
            serverDirectoryBox.Text = Settings.serverDirectory;
            serverFilenameBox.Text = Settings.serverFileName;
            ngrokDirectoryBox.Text = Settings.ngrokDirectory;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Directory.Exists(serverDirectoryBox.Text))
            {
                MessageBox.Show("Invalid directory given for 'Server directory'", "Settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            if (!Directory.Exists(ngrokDirectoryBox.Text))
            {
                MessageBox.Show("Invalid directory given for 'NGROK directory'", "Settings error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }

            Settings.memSize = (int)memorySelection.Value;
            Settings.useNGROK = useNGROKBox.Text == "Enabled" ? true : false;
            Settings.customIP = customIPTextBox.Text;
            Settings.localPort = localPortBox.Text;
            Settings.serverDirectory = serverDirectoryBox.Text;
            Settings.serverFileName = serverFilenameBox.Text;
            Settings.ngrokDirectory = ngrokDirectoryBox.Text;

            Functions.SaveSettings();
        }

        private void CustomIPTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Back) && e.Control)
            {
                e.SuppressKeyPress = true;
                int selStart = customIPTextBox.SelectionStart;
                while (selStart > 0 && customIPTextBox.Text.Substring(selStart - 1, 1) == " ")
                {
                    selStart--;
                }
                int prevSpacePos = -1;
                if (selStart != 0)
                {
                    prevSpacePos = customIPTextBox.Text.LastIndexOf(' ', selStart - 1);
                }
                customIPTextBox.Select(prevSpacePos + 1, customIPTextBox.SelectionStart - prevSpacePos - 1);
                customIPTextBox.SelectedText = "";
            }
        }

        private void UseNGROKBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Select();
        }

        private void LocalPortBox_TextChanged(object sender, EventArgs e)
        {
            if (!Functions.IsDigitsOnly(localPortBox.Text))
            {
                int selectionIndex = localPortBox.SelectionStart;
                localPortBox.Text = new string(localPortBox.Text.Where(Char.IsDigit).ToArray());
                localPortBox.SelectionStart = selectionIndex - 1;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SelectServerDirectoryButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    serverDirectoryBox.Text = fbd.SelectedPath;
                }
            }
        }

        private void SelectServerFilenameButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                ofd.Filter = "Java Files (*.jar)|*.jar";
                ofd.RestoreDirectory = true;

                DialogResult result = ofd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrEmpty(ofd.FileName))
                {
                    serverFilenameBox.Text = Path.GetFileName(ofd.FileName);
                }
            }
        }

        private void SelectNGROKDirectoryButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    ngrokDirectoryBox.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
