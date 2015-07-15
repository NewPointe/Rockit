<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PostAttendance.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.PostAttendance.PostAttendance" %>
 

<asp:UpdatePanel ID="upExceptionList" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlEvent" CssClass="panel panel-block" runat="server" Visible="true">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-ticket"></i> Event Details</h1>
            </div>

            <div class="panel-body">


<Rock:ModalAlert ID="Warning" runat="server" />

 <fieldset> 
    
     <Rock:CampusPicker ID="cpCampus" runat="server" Required="true" />
     <Rock:DataDropDownList ID="ddlEvent" runat="server" Label="Event" PropertyName="Name" Required="true" SourceTypeName="Rock.Model.GroupType, Rock"  />
     <Rock:DateTimePicker ID="dtpDateTime" runat="server" Required="true" Label="Event Date / Time" />




        <div class="actions">
            <asp:LinkButton ID="btnSaveEvent" runat="server" Text="Save Event and Add People" OnClick="btnSaveEvent_Click" CssClass="btn btn-primary"/>
        </div>
    </fieldset>   
                <br /><p><small>Need to add attendance for an event not listed here?  Please let us know: <a href="mailto:help@newpointe.org">help@newpointe.org</a></small></p>
                </div>
            </asp:Panel>





        <asp:Panel ID="pnlEventDetails" CssClass="panel panel-block" runat="server" Visible="false">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-ticket"></i> Event Details</h1>
            </div>

            <div class="panel-body">
                
                <h3><strong><%= Session["eventName"] %> at <%= Session["campusName"] %></strong> <br /> <%= Session["startDateTime"] %></h3>
                <%= Session["location"] %> <%= Session["schedule"] %>


                </div>
            </asp:Panel>




        <asp:Panel ID="pnlPeople" CssClass="panel panel-block" runat="server" Visible="false">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user-plus"></i> Event Participants</h1>
            </div>

            <div class="panel-body">
                
            <Rock:PersonPicker ID="ppPerson" runat="server" Label="Person" />

            <fieldset>
                <div class="actions">
            <asp:LinkButton ID="btnSave" runat="server" Text="Add Attendance" OnClick="btnSave_Click" CssClass="btn btn-primary"/>
        </div>
    </fieldset>   

                </div>
            </asp:Panel>




        </ContentTemplate>
    </asp:UpdatePanel>