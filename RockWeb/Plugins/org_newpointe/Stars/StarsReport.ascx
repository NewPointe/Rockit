<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StarsReport.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Stars.StarsReport" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        
        <asp:Panel ID="pnlSearch" CssClass="panel panel-block" runat="server" >
            
                        <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> Stars Report</h1>
            </div>
           
            <div class="panel-body">
                
                <div class="col-md-12">
                             
                    <Rock:Grid ID="gStars" runat="server" AllowSorting="true" AllowPaging="True" EmptyDataText="No Data Found" RowClickEnabled="True" OnRowSelected="gStars_OnRowSelected" >
                        <Columns>
                            <asp:BoundField DataField="Person" HeaderText="Person" />
                            <asp:BoundField DataField="Sum" HeaderText="Stars" />
                        </Columns>
                    </Rock:Grid>
                    
                      

                  </div>  </div>
            </asp:Panel>
        
        
        


    </ContentTemplate>
</asp:UpdatePanel>
