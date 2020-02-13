using System;
using System.Drawing;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ServerManager
{
    public partial class AlreadyRunningForm : Form
    {
        int copyRespondTime = 1000;
        ToolTip ipCopyTip = new ToolTip();
        string ip = "IP Missing!";
        bool forceRun = false;

        #region ICON STUFF
        public enum SHSTOCKICONID : uint
        {
            //...
            SIID_WARNING = 78,
            SIID_INFO = 79,
            //...
        }

        [Flags]
        public enum SHGSI : uint
        {
            SHGSI_ICONLOCATION = 0,
            SHGSI_ICON = 0x000000100,
            SHGSI_SYSICONINDEX = 0x000004000,
            SHGSI_LINKOVERLAY = 0x000008000,
            SHGSI_SELECTED = 0x000010000,
            SHGSI_LARGEICON = 0x000000000,
            SHGSI_SMALLICON = 0x000000001,
            SHGSI_SHELLICONSIZE = 0x000000004
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct SHSTOCKICONINFO
        {
            public UInt32 cbSize;
            public IntPtr hIcon;
            public Int32 iSysIconIndex;
            public Int32 iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260/*MAX_PATH*/)]
            public string szPath;
        }

        [DllImport("Shell32.dll", SetLastError = false)]
        public static extern Int32 SHGetStockIconInfo(SHSTOCKICONID siid, SHGSI uFlags, ref SHSTOCKICONINFO psii);
        #endregion

        public AlreadyRunningForm(string ip)
        {
            this.ip = ip;

            InitializeComponent();

            ipCopyTip.ShowAlways = true;
            ipCopyTip.Active = true;
            ipCopyTip.SetToolTip(ipLabel, "Click to copy");

            SHSTOCKICONINFO sii = new SHSTOCKICONINFO();
            sii.cbSize = (UInt32)Marshal.SizeOf(typeof(SHSTOCKICONINFO));
            Marshal.ThrowExceptionForHR(SHGetStockIconInfo(SHSTOCKICONID.SIID_WARNING, SHGSI.SHGSI_ICON, ref sii));

            warningIconBox.Image = Icon.FromHandle(sii.hIcon).ToBitmap();
            ipLabel.Text = ip;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ForceButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to force run the server over here?", "Force server run", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            if (result == DialogResult.Yes)
            {
                forceRun = true;
                this.Close();
            }
        }

        private void IpLabel_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ipLabel.Text);

            new Thread(() =>
            {
                ipLabel.Invoke((MethodInvoker)(() => ipLabel.Text = "Copied!"));
                Thread.Sleep(copyRespondTime);
                ipLabel.Invoke((MethodInvoker)(() => ipLabel.Text = ip));
            }).Start();
        }

        private void AlreadyRunningForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (forceRun)
            {
                this.DialogResult = DialogResult.Yes;
            }else
            {
                this.DialogResult = DialogResult.No;
            }
        }

        private void AlreadyRunningForm_Shown(object sender, EventArgs e)
        {
            SoundPlayer simpleSound = new SoundPlayer(@"C:\Windows\Media\Windows Background.wav");
            simpleSound.Play();
        }
    }
}
