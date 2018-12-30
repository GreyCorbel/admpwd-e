using AdmPwd.Portal.Controls;
using AdmPwd.Portal.Utilities;
using AdmPwd.Types;
using Resources;
using System;
using System.Security.Principal;
using System.Web.UI;

namespace AdmPwd.Portal
{
    public partial class _Default : System.Web.UI.Page
    {
        #region Fields

        private UserIdentityControl userIdentity;
        private CryptoKeyManagementControl cryptoKeyManagement;
        private ADAdminPwdRecoveryControl adminPasswordRecovery;

        #endregion

        #region Properties

        public PageStatus CurrentPageStatus
        {
            get
            {
                if (this.ViewState["PageStatus"] != null)
                    return (PageStatus)this.ViewState["PageStatus"];

                return PageStatus.AdminPasswordRecovery;
            }
            set
            {
                this.ViewState["PageStatus"] = (int)value;
            }
        }

        public UserIdentityControl UserIdentityControl
        {
            get { return this.userIdentity; }
        }

        public ADAdminPwdRecoveryControl ADAdminPwdRecoveryControl
        {
            get { return this.adminPasswordRecovery; }
        }

        public CryptoKeyManagementControl CryptoKeyManagementControl
        {
            get { return this.cryptoKeyManagement; }
        }

        #endregion

        #region Methods

        public void ShowBackToNewRequestButton()
        {
            pnlManageCryptoKeysNavigation.Visible = true;
            btnNewRequest.Visible = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.btnNewRequest.Click += new EventHandler(btnNewRequest_Click);
            this.btnManageCryptoKeys.Click += new EventHandler(btnManageCryptoKeys_Click);
            this.PanelMainContent.Visible = true;

            LoadControls();
            SetControlStatus();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        #endregion

        #region Event Handlers

        private void btnNewRequest_Click(object sender, EventArgs e)
        {
            this.CurrentPageStatus = PageStatus.AdminPasswordRecovery;
            this.ADAdminPwdRecoveryControl.Initialize();
            btnNewRequest.Visible = false;
            btnManageCryptoKeys.Visible = true;
            SetControlStatus();
            this.ShowHideButtonManageKeys();
        }

        private void btnManageCryptoKeys_Click(object sender, EventArgs e)
        {
            this.CurrentPageStatus = PageStatus.CryptoKeyManagement;
            this.CryptoKeyManagementControl.Initialize();
            btnNewRequest.Visible = true;
            btnManageCryptoKeys.Visible = false;
            SetControlStatus();
        }

        #endregion

        #region Helpers

        private void LoadControls()
        {
            this.userIdentity = (UserIdentityControl)LoadControl("~/Controls/UserIdentityControl.ascx");
            this.PHControls.Controls.Add(this.userIdentity);
            this.userIdentity.InitCurrentUserIdenfication();

            if (!this.userIdentity.UserIdentificationLoaded)
            {
                // problems with user account, do not continue
            }
            else
            {
                this.cryptoKeyManagement = (CryptoKeyManagementControl)LoadControl("~/Controls/CryptoKeyManagementControl.ascx");
                this.cryptoKeyManagement.Visible = false;
                this.PHControls.Controls.Add(this.cryptoKeyManagement);

                this.adminPasswordRecovery = (ADAdminPwdRecoveryControl)LoadControl("~/Controls/ADAdminPwdRecoveryControl.ascx");
                this.adminPasswordRecovery.Visible = false;
                this.PHControls.Controls.Add(this.adminPasswordRecovery);

                this.ShowHideButtonManageKeys();
            }
        }

        private void ShowHideButtonManageKeys()
        {
            using (WindowsImpersonationContext wic = ((WindowsIdentity)Page.User.Identity).Impersonate())
            {
                try
                {
                    if (PDSUtils.PdsWrapper.IsPDSAdmin())
                    {
                        pnlManageCryptoKeysNavigation.Visible = true;
                        btnManageCryptoKeys.Visible = true;
                    }
                    else
                    {
                        pnlManageCryptoKeysNavigation.Visible = false;
                        btnManageCryptoKeys.Visible = false;
                    }
                }
                catch (AutodiscoverException ex)
                {
                    labelError.Text = Messages.Errors_ServiceNotAvailable + " - " + ex.Message;
                    PanelError.Visible = true;
                    PHControls.Visible = false;
                    pnlManageCryptoKeysNavigation.Visible = false;
                    return;
                }
            }
        }

        private void SetControlStatus()
        {
            if (!this.userIdentity.UserIdentificationLoaded)
            {
                // problems with user account, do not continue
            }
            else
            {
                this.cryptoKeyManagement.Visible = this.CurrentPageStatus == PageStatus.CryptoKeyManagement;
                this.adminPasswordRecovery.Visible = this.CurrentPageStatus == PageStatus.AdminPasswordRecovery;
            }
        }

        #endregion
    }
}
