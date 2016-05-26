<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PersonPageHistoryBlock.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.PersonGetURLEncodedKey.PersonGetURLEncodedKey" %>
 

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
       
        
        
        
        <div class="panel panel-block">
                <div class="panel-heading">
                    <h1 class="panel-title margin-t-sm"><i class="fa fa-desktop"></i> Page View History</h1>
                </div>
                <div class="panel-body">
                    <a href="<%= PageViewHistoryUrl %>">Page View History</a>
                </div>
            </div>      


    </ContentTemplate>
</asp:UpdatePanel>
