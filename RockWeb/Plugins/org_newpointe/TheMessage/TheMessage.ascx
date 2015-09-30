<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TheMessage.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.TheMessage.TheMessage" %>


<div class="the-message container-fluid">



    <div class="col-sm-6 col-md-12">
        <div class="row title">

            <div class="col-xs-12">
                <h2>The message</h2>
            </div>
        </div>
        <div class="row">
            <div class="col-xs-12">
                <asp:Image runat="server" ImageUrl="~/Content/Central/message.png" />
            </div>
        </div>
           <div class="row" style="height:6px;">
               </div>
    </div>


    <div class="col-sm-6 col-md-12">

        <div class="row title">
            <div class="col-xs-12">
                <h2>New Here?</h2>
            </div>
        </div>

        <div class="row">
            <div class="col-xs-12">
                <asp:Image runat="server" ImageUrl="~/Content/Central/visitor.png" />
            </div>
        </div>

    </div>
</div>


