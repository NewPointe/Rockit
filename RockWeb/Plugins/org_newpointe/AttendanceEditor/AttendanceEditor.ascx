<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AttendanceEditor.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.AttendanceEditor.AttendanceEditor" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlDetails" CssClass="js-group-panel" runat="server">

            <div class="panel panel-block">
                <div class="panel-heading panel-follow clearfix">
                    <h1 class="panel-title pull-left">
                        <asp:Literal ID="lReadOnlyTitle" runat="server" />
                    </h1>
                </div>
                <Rock:GridFilter ID="attendanceFilters" runat="server" OnApplyFilterClick="attendanceFilters_ApplyFilterClick">
                    
                    <Rock:GroupTypePicker ID="gtpCheckinConfig" runat="server" Required="false" Label="Checkin Configuration" AutoPostBack="true" OnSelectedIndexChanged="gtpGroupType_SelectedIndexChanged" />
                    <Rock:GroupTypePicker ID="gtpCheckinArea" runat="server" Required="false" Label="Checkin Area" AutoPostBack="true" OnSelectedIndexChanged="gtpGroupType_SelectedIndexChanged" />
                    <Rock:RockDropDownList ID="rddlCheckinGroup" runat="server" Required="false" Label="Checkin Group" />

                    <Rock:PersonPicker ID="ppGroupMember" runat="server" Label="Person" Required="true" />

                    <Rock:DateRangePicker ID="dateRange" ClientIDMode="Static" runat="server" CssClass="np_customDate" Label="Date" />
                </Rock:GridFilter>
                <Rock:Grid ID="rgAttendanceList" runat="server" OnGridRebind="rgAttendanceList_GridRebind" AllowSorting="true" AllowPaging="true">
                    <Columns>
                        <asp:BoundField HeaderText="Attendance Id" DataField="Id" SortExpression="Id" />
                        <asp:BoundField HeaderText="Person" DataField="PersonName" SortExpression="PersonName" />
                        <asp:BoundField HeaderText="Checkin Group" DataField="CheckinGroupName" SortExpression="CheckinGroupName" />
                        <asp:BoundField HeaderText="Date" DataField="CheckinDate" SortExpression="CheckinDate" />
                        <Rock:BoolField HeaderText="DidAttend" DataField="DidAttend" SortExpression="DidAttend" />
                        <%--<Rock:ToggleField ControlStyle-CssClass="switch-large" HeaderText="DidAttend" DataField="DidAttend" SortExpression="DidAttend" OnCheckedChanged="rgDidAttend_CheckedChanged" Enabled="True" OnText="yes" OffText="no" OnCssClass="btn-success" ButtonSizeCssClass="btn-xs" />--%>
                        <Rock:BoolField HeaderText="DidNotOccur" DataField="DidNotOccur" SortExpression="DidNotOccur" />
                        
                        <%--<Rock:EditField OnClick="attendanceEdit_Click" />--%>
                        <Rock:LinkButtonField OnClick="attendanceDelete_Click" CssClass="btn-danger btn-sm" Text="<i class='fa fa-arrows-h'></i>" />
                    </Columns>
                </Rock:Grid>
            </div>

        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
