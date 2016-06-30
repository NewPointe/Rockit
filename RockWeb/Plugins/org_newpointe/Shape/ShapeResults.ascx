<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShapeResults.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Shape.ShapeResults" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:RockTextBox runat="server" ID="tbText"/>
    </ContentTemplate>
</asp:UpdatePanel>
