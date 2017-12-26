<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CryptoKeyManagementControl.ascx.cs" Inherits="AdmPwd.Portal.Controls.CryptoKeyManagementControl" EnableViewState="true" %>

<asp:Panel ID="PanelKeyList" runat="server">
    <div class="controlHeader">
        <asp:Literal ID="lit1" runat="server" Text="<%$ Resources:Messages,EKM_EncryptionKeysManagement%>" />
    </div>

    <asp:GridView ID="gvKeys" Width="100%" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:BoundField DataField="Id" ItemStyle-Width="30px" HeaderText="Id" />
            <asp:TemplateField HeaderText="Key value">
                <ItemTemplate>
                    <asp:TextBox ID="txtKeyValue" CssClass="keyValueMultiline" Width="700px" Height="70px" TextMode="MultiLine" runat="server" Text='<%# Bind("Key") %>'></asp:TextBox>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:Panel ID="PanelResult" Visible="false" CssClass="errorMessage" runat="server">
        <asp:Label ID="labelResult" runat="server"></asp:Label>
    </asp:Panel>

    <div class="controlHeader">
        <asp:Literal ID="lit3" runat="server" Text="<%$ Resources:Messages,EKM_GenerateNewKey%>" />
    </div>

    <table class="controlTable">
        <tr>
            <td class="controlLabel bold">
                <asp:Literal ID="lit4" runat="server" Text="<%$ Resources:Messages,EKM_KeySize%>" /></td>
            <td class="controlValue">
                <asp:DropDownList ID="ddlKeySize" DataTextField="Size" DataValueField="Size" runat="server">
                </asp:DropDownList>
            </td>
            <td style="vertical-align: bottom;">
                <asp:Button ID="btnGenerateKeyPair" runat="server" CausesValidation="false" Width="180px" Text="<%$ Resources:Messages,EKM_GenerateNewKey%>" />
            </td>
        </tr>
    </table>
</asp:Panel>

<asp:Panel ID="PanelError" Visible="false" CssClass="errorMessage" runat="server">
    <asp:Label ID="labelError" runat="server"></asp:Label>
</asp:Panel>