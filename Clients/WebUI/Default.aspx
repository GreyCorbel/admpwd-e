<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AdmPwd.Portal._Default" EnableViewState="true" ViewStateEncryptionMode="Always" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %></h1>
            </hgroup>
        </div>
    </section>
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Panel ID="PanelMainContent" runat="server">
        <asp:PlaceHolder ID="PHControls" runat="server"></asp:PlaceHolder>

        <asp:Panel ID="pnlManageCryptoKeysNavigation" Visible="false" runat="server">
            <div class="controlHeader">
                <asp:Literal ID="lit1" runat="server" Text="<%$ Resources:Messages,UIC_Navigation%>" />
            </div>
            <table class="controlTable">
                <tr>
                    <td class="controlLabel"></td>
                    <td class="controlValue">
                        <asp:Button ID="btnNewRequest" Text="<%$ Resources:Messages,Home_BackToNewRequest%>" Width="250px" CausesValidation="false" Visible="false" runat="server"></asp:Button>
                        <asp:Button ID="btnManageCryptoKeys" Text="<%$ Resources:Messages,Home_ManageEncryptionKeys%>" Width="200px" CausesValidation="false" Visible="false" runat="server"></asp:Button>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel ID="PanelError" Visible="false" CssClass="errorMessage" runat="server">
            <asp:Label ID="labelError" runat="server"></asp:Label>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
