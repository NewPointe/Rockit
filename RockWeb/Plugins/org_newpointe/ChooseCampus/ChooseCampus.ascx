<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ChooseCampus.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.ChooseCampus.ChooseCampus" %>

<div class="choose-campus container-fluid">

    <div class="row title">
        <div class="col-xs-12">
            <h2>Choose a Campus</h2>
        </div>
    </div>
    <div class="row campuses">
        <asp:Repeater runat="server" ID="rptCampuses">
            <ItemTemplate>
                <div class="col-xs-6">
                    <asp:HyperLink runat="server" ID="lnk" OnDataBinding="lnk_DataBinding"></asp:HyperLink>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <div class="col-xs-12">
            <a href="/10by20" class="btn btn-default btn-block tenbytwenty">Our Multisite Strategy</a>
        </div>

    </div>
    <div class="row map">
        <div class="col-xs-12 text-right">
                <a href="/locations">VIEW MAP & DIRECTIONS</a> 
        </div>
    </div>
</div>

