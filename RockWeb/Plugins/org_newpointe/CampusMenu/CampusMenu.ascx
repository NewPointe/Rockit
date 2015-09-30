<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CampusMenu.ascx.cs" Inherits="Plugins_org_newpointe_CampusMenu_CampusMenu" %>

<div class="hidden-xs hidden-sm hidden-md">
    <asp:Repeater ID="rptCampuses" runat="server">
        <HeaderTemplate>
            <ul id="npCampusMenu" class="nav navbar-nav">
                <li role="presentation" class="dropdown npCampusMenu">
                    <a class="dropdown-toggle" aria-expanded="false" aria-haspopup="true" role="button" data-toggle="dropdown" href="#">CHOOSE A LOCATION &nbsp;&nbsp;<i class="caret"></i></a>
                    <ul class="dropdown-menu" style="z-index: 2500;">
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <asp:HyperLink CssClass="dropdown2" runat="server" ID="lnk" OnDataBinding="lnk_DataBinding"></asp:HyperLink></li>
        </ItemTemplate>
        <FooterTemplate>
            </ul></li></ul>
        </FooterTemplate>
    </asp:Repeater>
</div>

<div id="mobileCampusMenu" class="hidden-lg">
    <asp:Repeater ID="rptMobileCampuses" runat="server">
        <HeaderTemplate>
            <ul><li>CHOOSE A LOCATION</li></ul>
            <ul class="ulCampusList">
        </HeaderTemplate>
        <ItemTemplate>
            <li><asp:HyperLink runat="server" ID="lnk2" OnDataBinding="lnk_DataBinding"></asp:HyperLink></li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>
</div>

<div id="divCampusTimes" class="hidden-xs hidden-sm hidden-md col-lg-12">
    <h5 class="pull-right">WATCH LIVE EVERY WEEK! SUNDAYS AT 9 & 11 A.M.</h5>
</div>