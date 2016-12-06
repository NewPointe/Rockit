<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StarsReport.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Stars.StarsReport" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        
        <asp:Panel ID="pnlSearch" CssClass="panel panel-block" runat="server" >
            
                        <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> Stars Report</h1>
            </div>
            
            <Rock:GridFilter ID="starsFilters" runat="server" OnApplyFilterClick="filters_ApplyFilterClick">
                    <Rock:MonthYearPicker runat="server" ID="mypMonth" Label="Month / Year"/>
                    <Rock:CampusesPicker runat="server" ID="cpCampus" Label="Campus"/>
                    <Rock:RockDropDownList runat="server" ID="ddlStars" Label="Stars Level">
                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                        <asp:ListItem Text="10" Value="10"></asp:ListItem>
                        <asp:ListItem Text="20" Value="20"></asp:ListItem>
                        <asp:ListItem Text="30" Value="30"></asp:ListItem>
                    </Rock:RockDropDownList>
                </Rock:GridFilter>
           
            <div class="panel-body">

                             
                    <Rock:Grid ID="gStars" runat="server" AllowSorting="true" AllowPaging="True" EmptyDataText="No Data Found" RowClickEnabled="True" OnRowSelected="gStars_OnRowSelected" EntityTypeId="15" DataKeyNames="PersonId" >
                        <Columns>
                            <asp:BoundField DataField="Person" HeaderText="Person" />
                            <asp:BoundField DataField="Sum" HeaderText="Stars" />
                        </Columns>
                    </Rock:Grid>
                    
                 </div>
            </asp:Panel>
        
        
        


    </ContentTemplate>
</asp:UpdatePanel>
