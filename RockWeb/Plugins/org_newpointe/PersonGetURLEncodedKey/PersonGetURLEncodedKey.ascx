<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonGetURLEncodedKey.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.PersonGetURLEncodedKey.PersonGetURLEncodedKey" %>
 

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:PersonPicker ID="ppPerson" runat="server" Label="Person" />
            
            <Rock:BootstrapButton ID="lbSave" runat="server" Text="Click Me" CssClass="btn btn-primary" OnClick="ppPerson_SelectPerson"
    DataLoadingText="&lt;i class='fa fa-refresh fa-spin fa-2x'&gt;&lt;/i&gt; Saving" />
        
        
<Rock:NotificationBox ID="nbSuccess" runat="server" Title="URL Encoded Key" Text="This is a success message." NotificationBoxType="Success" visible="false" />


    </ContentTemplate>
</asp:UpdatePanel>
