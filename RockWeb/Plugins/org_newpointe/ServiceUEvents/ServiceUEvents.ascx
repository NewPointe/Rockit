<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ServiceUEvents.ascx.cs" Inherits="Plugins_org_newpointe_ServiceUEvents_ServiceUEvents" %>
<%--<link rel="stylesheet" href="/Styles/plus.css" type="text/css">--%>
<link rel="stylesheet" href="/Themes/NewpointeMain/Styles/calendar.min.css" />

    <h1>Event Calendar</h1>

    <%--Search box--%>


<asp:HiddenField ID="hdnEventId" runat="server" />
<asp:HiddenField ID="hdnCampus" runat="server" />
<div class="container nopadding" style="height: 125px;">
    <div class="col-xs-12 col-md-8">
        <input type="text" id="calendar-search" name="calendar-search" placeholder="Search for Events Here" class="element text large" />
    </div>
    <div class="col-xs-12 col-md-4"></div>
    <div id="collapseFilter" class="collapsed-filter">

        <div id="CampusButtons" class="hidden-xs col-md-7">

            <h4>Choose a campus</h4>
            <a href="#" class="campusButton btn btn-block-xs btn-akron" data-fullname="Akron" data-shortname="AKR" data-campusid="67713">AKR</a>
            <a href="#" class="campusButton btn btn-block-xs btn-canton" data-fullname="Canton" data-shortname="CAN" data-campusid="53103">CAN</a>
            <a href="#" class="campusButton btn btn-block-xs btn-coshocton" data-fullname="Coshocton" data-shortname="COS" data-campusid="62004">COS</a>
            <a href="#" class="campusButton btn btn-block-xs btn-dover" data-fullname="Dover" data-shortname="DOV" data-campusid="51773">DOV</a>
            <a href="#" class="campusButton btn btn-block-xs btn-millersburg" data-fullname="Millersburg" data-shortname="MIL" data-campusid="51774">MIL</a>
            <a href="#" class="campusButton btn btn-block-xs btn-wooster" data-fullname="Wooster" data-shortname="WST" data-campusid="67714">WST</a>
            <a href="#" class="campusButton btn btn-block-xs btn-campus-all" data-fullname="ALL" data-shortname="ALL" data-campusid="ALL">ALL</a>
        </div>


        <div id="CampusDropdown" class="col-xs-12 hidden-sm hidden-md hidden-lg">
            <h4>Choose a campus</h4>
            <asp:DropDownList runat="server" ID="ddlCampusDropdown" CssClass="form-control">
                <asp:ListItem text="ALL" Value="ALL" />
                <asp:ListItem Text="Akron" Value="67713" />
                <asp:ListItem Text="Canton" Value="53103" />
                <asp:ListItem Text="Coshocton" Value="62004" />
                <asp:ListItem Text="Dover" Value="51773" />
                <asp:ListItem Text="Millersburg" Value="51774" />
                <asp:ListItem Text="Wooster" Value="67714" />
            </asp:DropDownList>
        </div>

        <div id="CategoryButtons" class="hidden-xs col-md-5">
            <h4>Choose a category</h4>
            <a href="#" class="categoryButton btn btn-cm" data-categoryid="13399" data-hovername="CM| BIRTH-5TH GRADE" data-shortname="CM">CM</a>
            <a href="#" class="categoryButton btn btn-sm" data-categoryid="13405" data-hovername="SM| 6TH-12TH GRADE" data-shortname="SM">SM</a>
            <a href="#" class="categoryButton btn btn-ya" data-categoryid="21205" data-hovername="YA| 19-29 YEARS" data-shortname="YA">YA</a>
            <a href="#" class="categoryButton btn btn-g" data-categoryid="11111" data-hovername="G| ADULT GROUPS" data-shortname="G">G</a>
            <a href="#" class="categoryButton btn btn-ae" data-categoryid="00000" data-hovername="AE| ANYTHING ELSE" data-shortname="AE">AE</a>
            <a href="#" class="categoryButton btn btn-all active" data-categoryid="all" data-hovername="ALL" data-shortname="ALL">ALL</a>
        </div>

        <div id="CategoryDropdown" class="col-xs-12 hidden-sm hidden-md hidden-lg">
            <h4>Choose a category</h4>
            <asp:DropDownList ID="ddlCategoryDropdown" CssClass="form-control" runat="server">
                <asp:ListItem Text="ALL" Value="all" />
                <asp:ListItem Text="CM| BIRTH-5TH GRADE" Value="13399" />
                <asp:ListItem Text="SM| 6TH-12TH GRADE" Value="13405" />
                <asp:ListItem Text="YA| 19-29 YEARS" Value="21205" />
                <asp:ListItem Text="G| Adult Groups" Value="11111" />
                <asp:ListItem Text="ANYTHING ELSE" Value="00000" />
            </asp:DropDownList>
        </div>
    </div>

</div>


<div class="row">
    <div class="col-md-4 col-md-push-8 top-cal">
        <%--<div class="col-md-12 hidden-xs hidden-sm col-spacer"></div>--%>
        <h2 class="text-center" id="event-date">Event Details</h2>
        <div id="events-list" data-key="na"><i class="fa fa-spinner fa-spin fa-3x"></i></div>
        <div id="event-description"></div>
    </div>
    <div class="col-md-8 col-md-pull-4">

        <div class="page-header">


            <div class="pull-right form-inline">
                <div class="btn-group">
                    <button class="btn btn-primary" data-calendar-nav="prev" style="margin-right: 10px;"><< Prev</button>

                    <button class="btn btn-primary" data-calendar-nav="next">Next >></button>
                </div>
                <%--                <div class="btn-group">
                    <button class="btn btn-warning" data-calendar-view="year">Year</button>
                    <button class="btn btn-warning active" data-calendar-view="month">Month</button>
                    <button class="btn btn-warning" data-calendar-view="week">Week</button>
                    <button class="btn btn-warning" data-calendar-view="day">Day</button>
                </div>--%>
            </div>
            <h3></h3>
        </div>

        <div id="calendar"></div>
    </div>

</div>
<script src="/Scripts/compenents/underscore.min.js"></script>
<script src="/Scripts/compenents/jstz.min.js"></script>
<script src="/Scripts/compenents/calendar.js"></script>
<script src="/Plugins/org_newpointe/ServiceUevents/app.js"></script>

