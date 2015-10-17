using AdmPwd.Portal.Utilities;
//using AdmPwd.Utils.LDAPExceptions;
using Resources;
using System;
using System.Configuration;
using System.Threading;

namespace AdmPwd.Portal.Controls
{
    public partial class UserIdentityControl : System.Web.UI.UserControl
    {

        #region Properties

        public LDAPUserInfo LDAPUserInfo
        {
            get
            {
                if (this.ViewState["LDAPUserInfo"] != null)
                    return (LDAPUserInfo)this.ViewState["LDAPUserInfo"];

                return null;
            }
            set
            {
                this.ViewState["LDAPUserInfo"] = value;
            }
        }

        public bool UserIdentificationLoaded
        {
            get { return this.ViewState["UserIdentificationLoaded"] != null ? (bool)this.ViewState["UserIdentificationLoaded"] : false; }
            set { this.ViewState["UserIdentificationLoaded"] = value; }
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            this.UserIdentificationLoaded = false;
            this.LDAPUserInfo = null;
            this.lblUserIdentification.Text = string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            SetControlState();
        }

        #endregion

        #region Event Handlers

        public void InitCurrentUserIdenfication()
        {
            this.LabelLDAPResult.Text = string.Empty;
            this.PanelLDAPResult.Visible = false;
            this.UserIdentificationLoaded = false;

            string[] currentUserIdentity = Thread.CurrentPrincipal.Identity.Name.Split('\\');
            try
            {
                lblUserIdentification.Text = currentUserIdentity[0] + "\\" + currentUserIdentity[1];
            }
            catch
            {
                this.LabelLDAPResult.Text = Messages.Errors_IISAuthentication;
                this.PanelLDAPResult.Visible = true;
                return;
            }

            this.UserIdentificationLoaded = true;
            SetControlState();
        }

        #endregion

        #region Helpers

        private void SetControlState()
        {
            this.PanelUserInfo.Visible = false;

            if (this.LDAPUserInfo != null)
            {
                this.PanelUserInfo.Visible = true;

                if (this.UserIdentificationLoaded)
                {
                    this.labelFullName.Text = this.LDAPUserInfo.FullName;
                    this.labelDomain.Text = this.LDAPUserInfo.Domain;
                    this.labelEmail.Text = this.LDAPUserInfo.Email;
                }
            }
        }

        #endregion

    }
}