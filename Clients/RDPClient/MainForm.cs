using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MSTSCLib;

namespace RDPClient
{
    [StructLayout(LayoutKind.Explicit)]
    struct LONG
    {
        [FieldOffset(0)]
        public long Value;

        [FieldOffset(0)]
        public int Low;

        [FieldOffset(4)]
        public int High;
    }

    public partial class MainForm : Form
    {
        FormWindowState LastWindowState = FormWindowState.Normal;
        Size LastWindowsSize = new Size(0, 0);

        #region P-Invoke
        // P/Invoke constants
        private const int WM_SYSCOMMAND = 0x112;
        private const int MF_STRING = 0x0;
        private const int MF_SEPARATOR = 0x800;

        // P/Invoke declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string lpNewItem);
        #endregion

        #region Menus
        // ID for the About item on the system menu
        private int SYSMENU_ABOUT_ID = 0x1;
        private int SYSMENU_RECONNECT_ID = 0x2;

        #endregion
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != LastWindowState)
            {
                //we're resizing from other than minimized state -> we require reconnect to fit to window
                if ((WindowState == FormWindowState.Maximized || WindowState == FormWindowState.Normal ) && LastWindowState != FormWindowState.Minimized)
                {
                    ResizeRdpClient();
                }
                LastWindowState = WindowState;
            }
        }

        public void SetCredentials(string userName, string domain, string password)
        {
            axRdpClient.UserName = userName;
            if(domain!=null)
                axRdpClient.Domain = domain;
            var secure = (IMsTscNonScriptable)axRdpClient.GetOcx();
            secure.ClearTextPassword = password;
        }
        public long GetSize()
        {
            LONG sizer = new LONG()
            {
                Low = Width,
                High = Height
            };
            return sizer.Value;
        }
        public void SetSize(long size)
        {
            LONG sizer = new LONG()
            {
                Low = 0,
                High = 0
            };
            
            sizer.Value = size;
            this.Size = new Size(sizer.Low, sizer.High);
            LastWindowsSize = this.ClientSize;
            axRdpClient.ClientSize = LastWindowsSize;
        }
        public void SetServerName(string server, ushort port)
        {
            axRdpClient.Server = server;
            axRdpClient.AdvancedSettings2.RDPPort = port;
            axRdpClient.AdvancedSettings7.EnableCredSspSupport = true;

            axRdpClient.ConnectingText = "Connecting...";
            axRdpClient.ConnectedStatusText = $"Connected: {server}";
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                axRdpClient.Connect();
                this.Text = $"RDP: {axRdpClient.Server}";
                LastWindowsSize = this.ClientSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to server {axRdpClient.Server}\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }
        }

        private void axRdpClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            //this is not closed when form explicitly closed by user
            var r = e.discReason;
            if (r != 2)
                MessageBox.Show($"RDPClient disconnected with reason code {r}.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Application.Exit();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (axRdpClient.Connected != 0)
                axRdpClient.Disconnect();

        }


        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            if (LastWindowsSize != this.ClientSize)
            {
                ResizeRdpClient();
            }
        }

        private void ResizeRdpClient()
        {
            this.axRdpClient.Size = new Size(ClientSize.Width, ClientSize.Height);
            axRdpClient.Invalidate();
            axRdpClient.Reconnect((uint)ClientSize.Width, (uint)ClientSize.Height);
            LastWindowsSize = ClientSize;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Get a handle to a copy of this form's system (window) menu
            IntPtr hSysMenu = GetSystemMenu(this.Handle, false);

            // Add a separator
            AppendMenu(hSysMenu, MF_SEPARATOR, 0, string.Empty);

            // Add the About menu item
            AppendMenu(hSysMenu, MF_STRING, SYSMENU_RECONNECT_ID, "&Reconnect");

            // Add a separator
            AppendMenu(hSysMenu, MF_SEPARATOR, 0, string.Empty);

            // Add the About menu item
            AppendMenu(hSysMenu, MF_STRING, SYSMENU_ABOUT_ID, "&About…");
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            // Test if the Reconnect item was selected from the system menu
            if ((m.Msg == WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_RECONNECT_ID))
            {
                axRdpClient.Reconnect((uint)LastWindowsSize.Width, (uint)LastWindowsSize.Height);
            }

            // Test if the About item was selected from the system menu
            if ((m.Msg == WM_SYSCOMMAND) && ((int)m.WParam == SYSMENU_ABOUT_ID))
            {
                Form aboutForm = new About();
                aboutForm.ShowDialog(this);
            }

        }
    }
}
