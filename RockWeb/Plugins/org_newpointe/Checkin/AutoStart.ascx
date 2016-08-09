<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AutoStart.ascx.cs" Inherits="RockWeb.Blocks.CheckIn.AutoStart" %>
<asp:UpdatePanel ID="upContent" runat="server">
<ContentTemplate>

    <div class="checkin-header">
        <h1>Check-in Auto Start</h1>
    </div>
    
    <div class="checkin-body">
        <Rock:NotificationBox ID="nbError" runat="server" NotificationBoxType="Warning" 
            Text="Check-in could not be started automatically. Select 'Continue' to configure check-in options." />
    </div>

    <div class="checkin-footer">   
        <div class="checkin-actions">
            <asp:LinkButton CssClass="btn btn-primary" ID="lbContinue" runat="server"
                 OnClick="lbContinue_Click" Text="Continue" Visible="true" />
        </div>
    </div>

</ContentTemplate>
</asp:UpdatePanel>