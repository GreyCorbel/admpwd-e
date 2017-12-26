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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            this.axRdpClient.Width = this.ClientSize.Width;
            this.axRdpClient.Height = this.ClientSize.Width;
            
        }

        public void SetCredentials(string userName, string domain, string password)
        {
            axRdpClient.UserName = userName;
            axRdpClient.Domain = domain;
            var secure = (IMsTscNonScriptable)axRdpClient.GetOcx();
            secure.ClearTextPassword = password;
        }

        public void SetServerName(string server)
        {
            axRdpClient.Server = server;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                axRdpClient.Connect();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect to server {axRdpClient.Server}\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (axRdpClient.Connected != 0)
                axRdpClient.Disconnect();

        }
    }
}
