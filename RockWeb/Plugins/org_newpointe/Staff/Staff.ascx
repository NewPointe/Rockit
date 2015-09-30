<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Staff.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Staff.Staff" %>

<div class="container-fluid staff">

 
            <h2 class="text-center"><span>CENTRAL SERVICES TEAM</span></h2>

    <hr />

    <div class="row ">

    <asp:Repeater runat="server" ID="rptStaff" OnItemDataBound="rptStaff_ItemDataBound">
        <ItemTemplate>

            <div class="col-xs-6 col-sm-4 col-md-3 col-lg-2 text-center">
                <asp:Image runat="server" ID="img" OnDataBinding="img_DataBinding" CssClass="staff"/>
                <p>
                    <asp:Label runat="server" ID="lblName" OnDataBinding="lblName_DataBinding"></asp:Label>
                </p>
                <p>
                    <asp:Label runat="server" ID="lblJob" OnDataBinding="lblJob_DataBinding"></asp:Label>
                </p>
            </div>

    

        </ItemTemplate>
    </asp:Repeater>
    </div>
</div>




