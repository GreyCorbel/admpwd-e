<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ADAdminPwdRecoveryControl.ascx.cs" Inherits="AdmPwd.Portal.Controls.ADAdminPwdRecoveryControl" EnableViewState="true" %>

<asp:Panel ID="PanelAdminPasswordRecovery" runat="server">
    <div class="controlHeader">
        <asp:Literal ID="lit1" runat="server" Text="<%$ Resources:Messages,RC_AdminPasswordRecoveryWorkflow%>" />
    </div>

    <asp:Panel ID="PanelComputerName" runat="server">
        <table class="controlTable">
            <tr>
                <td class="controlLabel bold">
                    <asp:Literal ID="lit2" runat="server" Text="<%$ Resources:Messages,RC_ComputerName%>" /></td>
                <td class="controlValue">
                    <asp:TextBox ID="textComputerName" CssClass="controlStyle" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="textComputerNameRequiredFieldValidator" Display="Dynamic" ControlToValidate="textComputerName" Text="*" ErrorMessage="<%$ Resources:Messages,RC_Val_FieldComputerNameRequired%>" runat="server"></asp:RequiredFieldValidator>
                </td>
                </tr>
            <tr>
                <td class="controlLabel bold">
                    <asp:Literal ID="lit3" runat="server" Text="<%$ Resources:Messages,RC_ForestName%>" /></td>
                <td class="controlValue">
<%--                    <asp:TextBox ID="textForestDnsName" CssClass="controlStyle" runat="server"></asp:TextBox>--%>
                    <asp:DropDownList ID="cboForestNames" CssClass="controlStyle" runat="server"></asp:DropDownList>
                </td>
                <td style="vertical-align: bottom;">
                    <asp:Button ID="btnRecoverySubmit" Text="<%$ Resources:Messages,RC_Submit%>" Width="120px" runat="server" /></td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="PanelAdminPasswordData" Visible="false" runat="server">
        <div class="controlHeader">
            <asp:Literal ID="lit5" runat="server" Text="<%$ Resources:Messages,RC_AdminPassword%>" />
        </div>
        <table class="controlTable">
            <tr>
                <td class="controlLabel">
                    <asp:Literal ID="lit10" runat="server" Text="<%$ Resources:Messages,RC_ComputerDN%>" /></td>
                <td class="controlValue">
                    <asp:TextBox ID="textAdminPasswordComputerDN" Width="398" CssClass="controlStyle" ReadOnly="true" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="controlLabel">
                    <asp:Literal ID="lit6" runat="server" Text="<%$ Resources:Messages,RC_AdminPassword%>" /></td>
                <td class="controlValue">
                    <asp:TextBox ID="textAdminPassword" Width="398" CssClass="controlStyle" ReadOnly="true" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="controlLabel">
                    <asp:Literal ID="lit7" runat="server" Text="<%$ Resources:Messages,RC_ExpirationDate%>" /></td>
                <td class="controlValue">
                    <asp:TextBox ID="textAdminPasswordExpiration" Width="398" CssClass="controlStyle" ReadOnly="true" runat="server"></asp:TextBox></td>
            </tr>
        </table>
        <asp:Panel ID="PanelExpirationDateChange" runat="server">
            <div class="controlHeader">
                <asp:Literal ID="lit8" runat="server" Text="<%$ Resources:Messages,RC_ExpirationDateChange%>" />
            </div>
            <table class="controlTable">
                <tr>
                    <td class="controlLabel">
                        <asp:Literal ID="lit9" runat="server" Text="<%$ Resources:Messages,RC_RegeneratePasswordAfterDate%>" /></td>
                    <td class="controlValue">
                        <asp:TextBox ID="textUpdateExpirationDateNewValue" Width="398" CssClass="controlStyle" runat="server"></asp:TextBox></td>
                    <td style="vertical-align: bottom;">
                        <asp:Button ID="btnUpdateExpirationDate" Text="<%$ Resources:Messages,RC_Update%>" runat="server" OnClick="btnUpdateExpirationDate_Click" /></td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="PanelPasswordHistory" runat="server">
            <div class="controlHeader">
                <asp:Literal ID="lit11" runat="server" Text="<%$ Resources:Messages,RC_PasswordHistory%>" />
            </div>
            <table class="controlTable">
                <tr>
                    <td class="controlLabel">
                        <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Messages,RC_PasswordHistory%>" /></td>
                    <td class="controlValueHistory">
                        <asp:Table ID="tblPasswordHistoryList" runat="server">
                            <asp:TableRow runat="server" CssClass="header">
                                <asp:TableCell runat="server">Password</asp:TableCell>
                                <asp:TableCell runat="server">Valid from</asp:TableCell>
                                <asp:TableCell runat="server">Valid until</asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </asp:Panel>
    <asp:Panel ID="PanelResult" Visible="false" CssClass="errorMessage" runat="server">
        <asp:Label ID="labelResult" runat="server"></asp:Label>
    </asp:Panel>
</asp:Panel>
