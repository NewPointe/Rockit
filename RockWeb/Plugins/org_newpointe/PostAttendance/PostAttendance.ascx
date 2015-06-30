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
    
    <Rock:LocationPicker ID="locpLocation" runat="server" Label="Location" />
     <Rock:CampusPicker ID="cpCampus" runat="server" Required="true" Label="Campus" />
     <Rock:DateTimePicker ID="dtpDateTime" runat="server" Required="true" Label="Rock:DateTimePicker" />
     <Rock:GroupPicker ID="gpGroup" runat="server" Label="Rock:GroupPicker" ItemId="129" InitialItemParentIds="129"   />
     <Rock:SchedulePicker ID="spSchedule" runat="server" Label="Schedule Picker" />




        <div class="actions">
            <asp:LinkButton ID="btnSaveEvent" runat="server" Text="Save Event and Add People" OnClick="btnSaveEvent_Click" CssClass="btn btn-primary"/>
        </div>
    </fieldset>   

                </div>
            </asp:Panel>





        <asp:Panel ID="pnlEventDetails" CssClass="panel panel-block" runat="server" Visible="false">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-ticket"></i> Event Details</h1>
            </div>

            <div class="panel-body">
                
           Campus:  <%= selectedCampus %> 
                Time; <%= startDateTime %>
                Group  <%= selectedGroup %>
                Location <%= selectedLocation %>



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