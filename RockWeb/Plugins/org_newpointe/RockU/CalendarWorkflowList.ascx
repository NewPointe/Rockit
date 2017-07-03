<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CalendarWorkflowList.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.RockU.CalendarWorkflowList" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:ModalAlert ID="mdWorkflowLaunched" runat="server" />

        <asp:Panel ID="pnlContent" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title">
                    <i class="fa fa-cogs"></i> Support Services
                </h1>
            </div>

            <div class="panel-body">
                <Rock:ModalAlert ID="mdGridWarning" runat="server" />

                <div class="grid grid-panel">
                    <Rock:Grid ID="gWorkflows" runat="server" OnRowSelected="gWorkflows_RowSelected" ShowActionRow="false" PagerSettings-Visible="false">
                        <Columns>
                            <%--<Rock:RockBoundField DataField="Id" HeaderText="Id" SortExpression="Id" />--%>
                            <Rock:RockBoundField DataField="WorkflowType.Name" HeaderText="Type" SortExpression="WorkflowType.Name" />
                            <Rock:RockBoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                            <Rock:RockBoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        </Columns>
                    </Rock:Grid>
                </div>
            </div>
            <div class="panel-footer clearfix">

                <div class="form-inline pull-right">
                    <div class="form-group">
                        <label >Add Support Service: &nbsp;</label>
                        <Rock:RockDropDownList ID="rddlNewWorkflowType" runat="server" />
                    </div>
                    <asp:LinkButton ID="lbGo" runat="server" CssClass="btn btn-default" Text="Go" OnClick="lbGo_Click" />
                </div>

            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
