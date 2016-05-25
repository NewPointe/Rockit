<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CompositeScores.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Metrics.CompositeScores" %>

<div class="panel panel-block" style="background: #f9f9f9">
     <div class="panel-heading" style="display: block !important;"><h4 class="panel-title"><i class="fa fa-line-chart"></i> Campus Composite Scores - <%= SelectedCampus %> </h4></div>
    
    <div class="row center-block">
    <div class="col-md-12 center-block">
        <h4 style="text-align: center;"><strong><%= SelectedCampus %> Index: <td><%= String.Format("{0:0.00}", CompositeScore) %>%</td></strong></h4>
    </div>
   </div>

<div class="container">    
    
    <table class="table table-striped table-responsive table-condensed">
        <thead>
            <tr>
                <th>Metric</th>
                <th>Current</th>
                <th>Last Period</th>
                <th>Growth</th>
                <th>Goal</th>
                <th>Progress</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Attendance - Adults</td>
                <td><%= iAttendanceAud %></td>
                <td><%= iAttendanceAudLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iAttendanceAud / (double)iAttendanceAudLastYear) - 1) * 100) %>%</td>
                <td><%= iAttendanceAudGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iAttendanceAudGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>Attendance - Kids</td>
                <td><%= iAttendanceKids %></td>
                <td><%= iAttendanceKidsLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iAttendanceKids / (double)iAttendanceKidsLastYear) - 1) * 100) %>%</td>
                <td><%= iAttendanceChildGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iAttendanceChildGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>Attendance - Students</td>
                <td><%= iAttendanceStudents %></td>
                <td><%= iAttendanceStudentsLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iAttendanceStudents / (double)iAttendanceStudentsLastYear) - 1) * 100) %>%</td>
                <td><%= iAttendanceStudentGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iAttendanceStudentGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>Volunteers</td>
                <td><%= iVolunteers %></td>
                <td><%= iVolunteersLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iVolunteers / (double)iVolunteersLastYear) - 1) * 100) %>%</td>
                <td><%= iVolunteersGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iVolunteersGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>Commitments</td>
                <td><%= iAllCommitments %></td>
                <td><%= iAllCommitmentsLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iAllCommitments / (double)iAllCommitmentsLastYear) - 1) * 100) %>%</td>
                <td><%= iAllCommitmentsGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iAllCommitmentsGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>Baptisms</td>
                <td><%= iBaptisms %></td>
                <td><%= iBaptismsLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iBaptisms / (double)iBaptismsLastYear) - 1) * 100) %>%</td>
                <td><%= iBaptismsGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iBaptismsGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>Small Group Participation</td>
                <td><%= iSmallGroupParticipants %></td>
                <td><%= iSmallGroupParticipantsLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iSmallGroupParticipants / (double)iSmallGroupParticipantsLastYear) - 1) * 100) %>%</td>
                <td><%= iSmallGroupParticipantsGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iSmallGroupParticipantsGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>Partners</td>
                <td><%= iPartners %></td>
                <td><%= iPartnersLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iPartners / (double)iPartnersLastYear) - 1) * 100) %>%</td>
                <td><%= iPartnersGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iPartnersGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>New to NewPointe</td>
                <td><%= iNewtoNewPointe %></td>
                <td><%= iNewtoNewPointeLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iNewtoNewPointe / (double)iNewtoNewPointeLastYear) - 1) * 100) %>%</td>
                <td><%= iNewtoNewPointeGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iNewtoNewPointeGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>Discover Groups</td>
                <td><%= iDiscoverGroups %></td>
                <td><%= iDiscoverGroupsLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iDiscoverGroups / (double)iDiscoverGroupsLastYear) - 1) * 100) %>%</td>
                <td><%= iDiscoverGroupsGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iDiscoverGroupsGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>New Here Guests</td>
                <td><%= iNewHere %></td>
                <td><%= iNewHereLastYear %></td>
                <td><%= String.Format("{0:0.00}", (((double)iNewHere / (double)iNewHereLastYear) - 1) * 100) %>%</td>
                <td><%= iNewHereGoalCurrent %></td>
                <td><%= String.Format("{0:0.00}", iNewHereGoalProgress) %>%</td>
            </tr>
            <tr>
                <td>Inactive Follow-up</td>
                <td><%= iInactiveFollowupComplete %></td>
                <td> </td>
                <td> </td>
                <td><%= iInactiveFollowup %></td>
                <td><%= String.Format("{0:0.00}",iInactiveFollowupProgress) %></td>
            </tr>
            <tr>
                <td>Giving</td>
                <td><%= String.Format("{0:0.00}", giving * 100) %>%</td>
                <td> </td>
                <td> </td>
                <td>100%</td>
                <td>105% </td>
            </tr>
            <tr>
                <td>Spending</td>
                <td><%= String.Format("{0:0.00}", expenses * 100)  %>%</td>
                <td> </td>
                <td> </td>
                <td>100%</td>
                <td>98% </td>
            </tr>
        </tbody>
    </table>
    
    

    </div>
    


   

    <div class="panel-footer" style="text-align: right;">Choose a different campus: <Rock:ButtonDropDownList ID="cpCampus" runat="server" OnSelectionChanged="cpCampus_OnSelectionChanged" ToolTip="Choose a Campus"/></div>
    </div>

    
        <div class="col-md-8">
        <br/><br/>
        <div class="panel-group" id="accordion">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4 class="panel-title">
                        <a data-toggle="collapse" data-parent="#accordion" href="#collapseOne"><i class="fa fa-power-off"></i> Diagnostic Info</a>
                    </h4>
                </div>
                <div id="collapseOne" class="panel-collapse collapse">
                    <div class="panel-body">
                        <div class="col-md-6">
                            <p>
                                Campus: <%= SelectedCampus %> <br/>
                                Fiscal Year Start Date: <%= FiscalYearStartDate %> <br/>
                                Fiscal Year End Date: <%= FiscalYearEndDate %> <br/>
                                Reporting Period Year Start Date: <%= PeriodStartDate %> <br/>
                                Reporting Period Year End Date: <%= PeriodEndDate %> <br/>
                                Last Period Year Start Date: <%= LastPeriodStartDate %> <br/>
                                Last Period Year End Date: <%= LastPeriodEndDate %> <br/>
                            </p>
                        </div>
                        <div class="col-md-6">
                            <p>
                                test    
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>