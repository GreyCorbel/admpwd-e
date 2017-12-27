using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MSTSCLib;

namespace RDPClient
{
    public partial class Form1 : Form
    {
        FormWindowState LastWindowState = FormWindowState.Normal;
        Size LastWindowsSize = new Size(0, 0);
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState != LastWindowState)
            {
                LastWindowState = WindowState;


                if (WindowState == FormWindowState.Maximized)
                {

                    ResizeRdpClient();
                }
                if (WindowState == FormWindowState.Normal)
                {

                    ResizeRdpClient();
                }
            }
        }

        public void SetCredentials(string userName, string domain, string password)
        {
            axRdpClient.UserName = userName;
            //axRdpClient.Domain = domain;
            var secure = (IMsTscNonScriptable)axRdpClient.GetOcx();
            secure.ClearTextPassword = password;
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
            var r = e.discReason;
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
                LastWindowsSize = ClientSize;
            }
        }

        private void ResizeRdpClient()
        {
            this.axRdpClient.Size = new Size(this.ClientSize.Width, ClientSize.Height);
            axRdpClient.Invalidate();
            axRdpClient.Reconnect((uint)ClientSize.Width, (uint)ClientSize.Height);

        }
    }
}
