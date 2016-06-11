<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ParentPage.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.ParentPage.ParentPage" %>
 

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        
        <asp:Panel ID="pnlSearch" CssClass="panel panel-block" runat="server" >
            
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> Code Lookup</h1>
            </div>
           
            <div class="panel-body">
                
                <div class="col-md-4">
        <Rock:RockTextBox runat="server" Label="Check-in Code" MaxLength="3" ID="rtbCode" CssClass="input-width-sm"/>
            
            <Rock:BootstrapButton ID="lbSave" runat="server" Text="Search Code" CssClass="btn btn-primary" OnClick="FindPerson"
    DataLoadingText="&lt;i class='fa fa-refresh fa-spin fa-2x'&gt;&lt;/i&gt; Searching" />
        
                    </div>
                
                <div class="col-md-4">
                    <strong><asp:Label runat="server" ID="lbTitle"></asp:Label></strong><br/>
                    <asp:Label runat="server" ID="lbName"></asp:Label><br/>
                    <asp:Label runat="server" ID="lbFamily"></asp:Label><br/>
                    <asp:Label runat="server" ID="lbCampus"></asp:Label><br/>
                </div>
                
                
                <div class="col-md-4">
                    <strong><asp:Label runat="server" ID="lbFamilyTitle"></asp:Label></strong><br/>
                    <asp:Label runat="server" ID="lbNameToText"></asp:Label><br/>
                    <asp:Label runat="server" ID="lbFamilyToText"></asp:Label><br/>
                    <asp:Label runat="server" ID="lbNumberToText"></asp:Label><br/>
                </div>
                
                </div>
            </asp:Panel>
        
        <asp:Panel ID="pnlResults" CssClass="panel panel-block" runat="server" Visible="False" >
        
        <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> Choose Person</h1>
            </div>

            <div class="panel-body">
        
        <Rock:Grid ID="gPeople" runat="server" AllowSorting="true" OnRowSelected="gPeople_RowSelected" DataKeyNames="Id">
        <Columns>
            <asp:BoundField DataField="FullName" HeaderText="Name" />
            <asp:BoundField DataField="Group" HeaderText="Group" />
            <asp:BoundField DataField="Age" HeaderText="Age" />
            <asp:BoundField DataField="Gender" HeaderText="Gender" />
            <asp:BoundField DataField="Date" HeaderText="Date" />
            <asp:BoundField DataField="Time" HeaderText="Time" />
            <asp:BoundField DataField="Campus" HeaderText="Campus" />

        </Columns>
    </Rock:Grid>
            
            </div>
            </asp:Panel>
        
        
        
        <asp:Panel ID="pnlNumbers" CssClass="panel panel-block" runat="server" Visible="false">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> Relevant Adults</h1>
            </div>

            <div class="panel-body">
            
                <Rock:Grid ID="gFamily" runat="server" AllowSorting="true" DataKeyNames="Id" OnRowSelected="gFamily_RowSelected">
        <Columns>
            <asp:BoundField DataField="FullName" HeaderText="Name" />
            <asp:BoundField DataField="FamilyName" HeaderText="Family" />
            <asp:BoundField DataField="PhoneNumber" HeaderText="Phone" />
   

        </Columns>
                    </Rock:Grid>

                </div>
            </asp:Panel>
        
        
        
        <asp:Panel ID="pnlMessage" CssClass="panel panel-block" runat="server" Visible="false">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> Send Text Message</h1>
            </div>

            <div class="panel-body">
        
                <Rock:RockTextBox runat="server" Rows="4" ID="rtMessage" Label="Message"/>
                
                <Rock:BootstrapButton ID="lbSend" runat="server" Text="Send Message" CssClass="btn btn-danger" OnClick="FindPerson"
    DataLoadingText="&lt;i class='fa fa-refresh fa-spin fa-2x'&gt;&lt;/i&gt; Sending" />
                

                </div>

            </asp:Panel>
        
    </ContentTemplate>
</asp:UpdatePanel>


