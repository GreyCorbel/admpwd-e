using AdmPwd.Portal.Utilities;
using AdmPwd.PDSUtils.PdsProxy;
using AdmPwd.Types;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace AdmPwd.Portal.Controls
{
    public partial class CryptoKeyManagementControl : System.Web.UI.UserControl
    {

        #region Methods

        public void Initialize()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.btnGenerateKeyPair.Click += new EventHandler(btnGenerateKeyPair_Click);

            if (!IsPostBack)
            {
                try
                {
                    // get supported key sizes
                    ddlKeySize.DataSource = PDSUtils.PdsWrapper.GetSupportedKeySizes();
                    ddlKeySize.DataBind();
                }
                catch (AutodiscoverException ex)
                {
                    labelError.Text = Messages.Errors_ServiceNotAvailable + " - " + ex.Message;
                    PanelError.Visible = true;
                    PanelKeyList.Visible = false;
                    return;
                }
            }

            this.InitializeKeyList();
        }

        private void InitializeKeyList()
        {
            List<PublicKey> publicKeys = null;
            using (WindowsImpersonationContext wic = ((WindowsIdentity)Page.User.Identity).Impersonate())
            {
                try
                {
                    // get public keys list
                    publicKeys = PDSUtils.PdsWrapper.GetPublicKeys();
                }
                catch (AutodiscoverException ex)
                {
                    labelError.Text = Messages.Errors_ServiceNotAvailable + " - " + ex.Message;
                    PanelError.Visible = true;
                    PanelKeyList.Visible = false;
                    return;
                }
            }

            gvKeys.DataSource = publicKeys;
            gvKeys.DataBind();

            if (publicKeys.Count() == 0)
            {
                labelResult.Text = Messages.EKM_NoKeysInStorage + "!";
                PanelResult.Visible = true;
            }
            else
                PanelResult.Visible = false;
        }

        #endregion

        #region Event Handlers

        private void btnGenerateKeyPair_Click(object sender, EventArgs e)
        {
            _Default defaultPage = (_Default)this.Page;

            using (WindowsImpersonationContext wic = ((WindowsIdentity)Page.User.Identity).Impersonate())
            {
                try
                {
                    // save crypto keypair
                    PDSUtils.PdsWrapper.GenerateKeyPair(int.Parse(ddlKeySize.SelectedValue));
                }
                catch (AutodiscoverException ex)
                {
                    labelResult.Text = Messages.Errors_ServiceNotAvailable + " - " + ex.Message;
                    PanelResult.Visible = true;
                    return;
                }
                catch (PDSException ex)
                {
                    labelResult.Text = Messages.Errors_CannotResetPassword + " - " + ex.Message;
                    PanelResult.Visible = true;
                    return;
                }
                catch (Exception ex)
                {
                    labelResult.Text = Messages.Errors_CannotResetPassword + " - " + ex.Message;
                    PanelResult.Visible = true;
                    return;
                }
            }

            // refresh list
            InitializeKeyList();
        }

        #endregion

    }
}