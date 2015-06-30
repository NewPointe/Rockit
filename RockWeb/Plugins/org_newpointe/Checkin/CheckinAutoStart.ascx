<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckinAutoStart.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Checkin.CheckinAutoStart" %>

<asp:UpdatePanel ID="upContent" runat="server">
<ContentTemplate>

    <asp:PlaceHolder ID="phScript" runat="server"></asp:PlaceHolder>
    <asp:HiddenField ID="hfLatitude" runat="server" />
    <asp:HiddenField ID="hfLongitude" runat="server" />
    <asp:HiddenField ID="hfTheme" runat="server" />
    <asp:HiddenField ID="hfKiosk" runat="server" />
    <asp:HiddenField ID="hfGroupTypes" runat="server" />

    <asp:LinkButton ID="lbRefresh" runat="server" OnClick="lbRefresh_Click"></asp:LinkButton>

    <Rock:ModalAlert ID="maWarning" runat="server" />

    <div class="checkin-header">
        <h1>Check-in Configuration</h1>
    </div>
    
    <asp:Panel runat="server" CssClass="checkin-body" ID="pnlManualConfig" Visible="true">

        <div class="checkin-scroll-panel">
 
            <div class="scroller">

               
                <Rock:RockDropDownList ID="ddlKiosk" runat="server" CssClass="input-xlarge" Label="Choose Campus/Kiosk" AutoPostBack="true" OnSelectedIndexChanged="ddlKiosk_SelectedIndexChanged" DataTextField="Name" DataValueField="Id"  ></Rock:RockDropDownList>
                <!--  <Rock:RockTextBox ID="grouplist" runat="server" Label="Groups"></Rock:RockTextBox> -->

            </div>
        </div>

    </asp:Panel>
    

    <div class="checkin-footer">   
        <div class="checkin-actions">
            <asp:LinkButton CssClass="btn btn-primary" ID="lbOk" runat="server" OnClick="lbOk_Click" Text="Start Check-in" Visible="true" />
            <asp:LinkButton CssClass="btn btn-default" runat="server" ID="lbManual" visible="true" OnClick="lbManual_Click" Text="Manual Config" />
        </div>
    </div>

</ContentTemplate>
</asp:UpdatePanel>
