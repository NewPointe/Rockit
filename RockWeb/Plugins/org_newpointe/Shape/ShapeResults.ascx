<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShapeResults.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Shape.ShapeResults" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:NotificationBox runat="server" NotificationBoxType="Danger" Text="You must be logged in or access this page from a vaild link." Visible="False" ID="nbNoPerson" Title="No Valid Person" ></Rock:NotificationBox>
        
        
        <asp:Label runat="server" ID="lbPersonName"></asp:Label>

        <asp:Label runat="server" ID="lbGift1Title"></asp:Label>
        <asp:Label runat="server" ID="lbGift1Body"></asp:Label>
        
        <asp:Label runat="server" ID="lbGift2Title"></asp:Label>
        <asp:Label runat="server" ID="lbGift2Body"></asp:Label>
        
        

    </ContentTemplate>
</asp:UpdatePanel>
