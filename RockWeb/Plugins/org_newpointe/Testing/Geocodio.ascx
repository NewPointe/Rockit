<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Geocodio.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Geocodio.Geocodio" %>
 
<Rock:NotificationBox ID="result" runat="server" Title="School District Found!" Text="This is a success message." NotificationBoxType="Success" visible="false" />

<!--<Rock:AddressControl runat="server" name="theAddress" ID="theAddress" />-->

<Rock:PersonPicker runat="server" name="thePerson" ID="thePerson"/>

<br/>
<Rock:BootstrapButton ID="btnSend" runat="server" Text="Get School District" OnClick="btnSend_Click" CssClass="btn btn-primary" DataLoadingText="&lt;i class='fa fa-refresh fa-spin'&gt;&lt;/i&gt; Doing Stuff..." />