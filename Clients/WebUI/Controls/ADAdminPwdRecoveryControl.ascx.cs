using AdmPwd.Portal.Utilities;
using AdmPwd.ServiceUtils.PdsProxy;
using AdmPwd.Types;
using Resources;
using System;
using System.Configuration;
using System.Security.Principal;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Win32;

namespace AdmPwd.Portal.Controls
{
    public partial class ADAdminPwdRecoveryControl : System.Web.UI.UserControl
    {

        #region Methods

        public void Initialize()
        {
            this.InitializeControls();

            this.textComputerName.Enabled = true;
            this.btnRecoverySubmit.Visible = true;
            this.btnRecoverySubmit.Attributes.Add("style", "color: black; background-color: #d3dce0;");
            this.PanelAdminPasswordData.Visible = false;
            this.textAdminPassword.Text = string.Empty;


        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.btnRecoverySubmit.Click += new EventHandler(btnRecoverySubmit_Click);

            try
            {
                RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                RegistryKey pol = root.OpenSubKey("Software\\Policies\\Microsoft Services\\AdmPwd");
                if (pol != null)
                {
                    string[] forestList = pol.GetValue("SupportedForests") as string[];
                    if (forestList != null)
                    {
                        this.cboForestNames.DataSource = forestList;
                        this.cboForestNames.DataBind();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            _Default defaultPage = (_Default)this.Page;
            if (defaultPage.CurrentPageStatus == PageStatus.AdminPasswordRecovery)
            {
                defaultPage.Form.DefaultFocus = this.textComputerName.ClientID;
                defaultPage.Form.DefaultButton = this.btnRecoverySubmit.UniqueID;
            }
        }

        #endregion

        #region Event Handlers

        private void btnRecoverySubmit_Click(object sender, EventArgs e)
        {
            this.PanelResult.Visible = false;
            this.labelResult.Text = string.Empty;

            this.InitializeControls();

            _Default defaultPage = (_Default)this.Page;
            PasswordInfo data = null;

            // read configuration if PasswordHistory is visible
            bool isPasswordHistoryVisible = false;
            if (ConfigurationManager.AppSettings["AdmPwd.IsPasswordHistoryVisible"] != null && ConfigurationManager.AppSettings["AdmPwd.IsPasswordHistoryVisible"] != string.Empty)
                if (!bool.TryParse(ConfigurationManager.AppSettings["AdmPwd.IsPasswordHistoryVisible"].ToString(), out isPasswordHistoryVisible))
                    isPasswordHistoryVisible = false;

            var current = System.Security.Principal.WindowsIdentity.GetCurrent();
            using (WindowsImpersonationContext wic = ((WindowsIdentity)Page.User.Identity).Impersonate())
            {
                try
                {
                    data = AdmPwd.ServiceUtils.PdsWrapper.GetPassword(this.cboForestNames.Text, this.textComputerName.Text, isPasswordHistoryVisible); //or false if we don't need password history
                }
                catch (AutodiscoverException ex)
                {
                    this.labelResult.Text = Messages.Errors_ServiceNotAvailable + " - " + ex.Message;
                    this.PanelResult.Visible = true;
                    return;
                }
                catch (System.ServiceModel.FaultException<ServiceFault> faex)
                {
                    switch (faex.Detail.IssueCode)
                    {
                        case IssueType.ComputerNotFound:
                            this.labelResult.Text = Messages.Errors_ADNoComputerFoundException + " " + faex.Message;
                            break;
                        case IssueType.ComputerAmbiguous:
                            this.labelResult.Text = Messages.Errors_MultipleComputerObjectsFound + ": " + faex.Message;
                            break;
                        case IssueType.AccessDenied:
                            this.labelResult.Text = Messages.Errors_NoReadPasswordForUserOnComputerObject + ": " + faex.Message;
                            break;
                        case IssueType.CannotRetrievePassword:
                            this.labelResult.Text = Messages.Errors_CannotRetrievePassword + ": " + faex.Message;
                            break;
                        default:
                            this.labelResult.Text = Messages.Errors_CannotRetrievePassword + ": " + faex.Message;
                            break;
                    }
                    this.PanelResult.Visible = true;
                    return;
                }
                catch (Exception ex)
                {
                    labelResult.Text = Messages.Errors_CannotRetrievePassword + " - " + ex.Message;
                    PanelResult.Visible = true;
                    return;
                }

            }
            current = System.Security.Principal.WindowsIdentity.GetCurrent();

            if (!String.IsNullOrEmpty(data.Password))
            {
                this.btnRecoverySubmit.Visible = false;
                textComputerName.Enabled = false;

                this.PanelAdminPasswordData.Visible = true;
                this.textAdminPassword.Text = data.Password;
                this.textAdminPasswordExpiration.Text = data.ExpirationTimestamp > DateTime.MinValue ? data.ExpirationTimestamp.ToString() : string.Empty;
                this.textAdminPasswordComputerDN.Text = data.DistinguishedName;

                if (isPasswordHistoryVisible)
                {
                    if (data.PasswordHistory.Count > 0)
                    {
                        PanelPasswordHistory.Visible = true;
                        tblPasswordHistoryList.Visible = true;
                        foreach (PasswordHistory pi in data.PasswordHistory)
                        {
                            TableRow rw = new TableRow();
                            rw.Cells.Add(new TableCell() { Text = pi.Password });
                            rw.Cells.Add(new TableCell() { Text = pi.ValidSince.ToString() });
                            rw.Cells.Add(new TableCell() { Text = pi.ValidUntil.ToString() });
                            tblPasswordHistoryList.Rows.Add(rw);
                        }
                    }
                    else
                        PanelPasswordHistory.Visible = false;
                }

                defaultPage.ShowBackToNewRequestButton();

                this.textUpdateExpirationDateNewValue.Text = DateTime.Now.ToString();
                PanelExpirationDateChange.Visible = true;
            }
        }

        protected void btnUpdateExpirationDate_Click(object sender, EventArgs e)
        {
            this.PanelPasswordHistory.Visible = false;

            _Default defaultPage = (_Default)this.Page;
            DateTime newExpirationDate;
            if (!DateTime.TryParse(textUpdateExpirationDateNewValue.Text, out newExpirationDate))
            {
                this.PanelResult.Visible = true;
                this.labelResult.Text = Messages.RC_NotValidExpirationDate;
                this.PanelResult.CssClass = "errorMessage";
            }
            else
            {
                PasswordResetStatus rslt = null;
                var current = System.Security.Principal.WindowsIdentity.GetCurrent();
                using (WindowsImpersonationContext wic = ((WindowsIdentity)Page.User.Identity).Impersonate())
                {
                    try
                    {
                        rslt = AdmPwd.ServiceUtils.PdsWrapper.ResetPassword(this.cboForestNames.Text, this.textComputerName.Text, newExpirationDate);
                    }
                    catch (System.ServiceModel.FaultException<ServiceFault> faex)
                    {
                        switch(faex.Detail.IssueCode)
                        {
                            case IssueType.ComputerNotFound:
                                this.labelResult.Text = Messages.Errors_ADNoComputerFoundException + " " + faex.Message;
                                break;
                            case IssueType.ComputerAmbiguous:
                                this.labelResult.Text = Messages.Errors_MultipleComputerObjectsFound + ": " + faex.Message;
                                break;
                            case IssueType.AccessDenied:
                                this.labelResult.Text = Messages.Errors_NoResetPasswordForUserOnComputerObject + ": " + faex.Message;
                                break;
                            case IssueType.CannotResetPassword:
                                this.labelResult.Text = Messages.Errors_CannotResetPassword + ": " + faex.Message;
                                break;
                            default:
                                this.labelResult.Text = Messages.Errors_CannotResetPassword + ": " + faex.Message;
                                break;
                        }
                        this.PanelResult.Visible = true;
                        return;
                    }
                    catch (Exception ex)
                    {
                        labelResult.Text = Messages.Errors_ServiceNotAvailable + " - " + ex.Message;
                        PanelResult.Visible = true;
                        return;
                    }

                }
                current = System.Security.Principal.WindowsIdentity.GetCurrent();

                PanelExpirationDateChange.Visible = false;
                textAdminPasswordExpiration.Text = newExpirationDate.ToString();

                this.PanelPasswordHistory.Visible = false;
                this.PanelResult.Visible = true;
                this.labelResult.Text = Messages.RC_ExpirationDateUpdated;
                this.PanelResult.CssClass = "okMessage";

                defaultPage.ShowBackToNewRequestButton();
            }
        }

        #endregion

        #region Helpers

        private void InitializeControls()
        {
            this.PanelResult.Visible = false;
            this.labelResult.Text = string.Empty;
        }

        #endregion

    }
}