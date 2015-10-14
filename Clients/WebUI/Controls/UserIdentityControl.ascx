<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserIdentityControl.ascx.cs" Inherits="AdmPwd.Portal.Controls.UserIdentityControl" EnableViewState="true" %>

<asp:Panel ID="PanelUserIdentification" runat="server">
    <div class="controlHeader">
        <asp:Literal ID="lit1" runat="server" Text="<%$ Resources:Messages,UIC_CurrentUserIdentification%>" /></div>
    <table class="controlTable">
        <tr>
            <td class="controlLabel bold">
                <asp:Label ID="labelUserIdentification" Text="<%$ Resources:Messages,UIC_UserID%>" runat="server"></asp:Label></td>
            <td class="controlValue">
                <asp:Label ID="lblUserIdentification" runat="server"></asp:Label>
            </td>
        </tr>
    </table>

    <asp:Panel ID="PanelLDAPResult" Visible="false" CssClass="errorMessage" runat="server">
        <asp:Label ID="LabelLDAPResult" runat="server"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="PanelUserInfo" Visible="false" runat="server">
        <table class="controlTable">
            <tr>
                <td class="controlLabel bold"><asp:Literal ID="lit2" runat="server" Text="<%$ Resources:Messages,UIC_FullName%>" /></td>
                <td class="controlValue">
                    <asp:Label ID="labelFullName" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="controlLabel"><asp:Literal ID="lit3" runat="server" Text="<%$ Resources:Messages,UIC_Domain%>" /></td>
                <td class="controlValue">
                    <asp:Label ID="labelDomain" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="controlLabel"><asp:Literal ID="lit4" runat="server" Text="<%$ Resources:Messages,UIC_Email%>" /></td>
                <td class="controlValue">
                    <asp:Label ID="labelEmail" runat="server"></asp:Label></td>
            </tr>
        </table>
    </asp:Panel>
</asp:Panel>
