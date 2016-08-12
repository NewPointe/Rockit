﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CompositeScores.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Metrics.CompositeScores" %>

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
                <th>YTD</th>
                <th>LYTD</th>
                <th>Growth</th>
                <th>Goal</th>
                <th>Progress</th>
                <th>8Wk TY</th>
                <th>8Wk LY</th>
                <th>Growth</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Attendance - Adults <small>(Avg past 6wks)</small></td>
                <td><%= iAttendanceAud %></td>
                <td><%= iAttendanceAudLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iAttendanceAud / (double)iAttendanceAudLastYear) - 1) * 100) %>%</td>
                <td><%= iAttendanceAudGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iAttendanceAudGoalProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <tr>
                <td>Attendance - Kids <small>(Avg past 6wks)</small></td>
                <td><%= iAttendanceKids %></td>
                <td><%= iAttendanceKidsLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iAttendanceKids / (double)iAttendanceKidsLastYear) - 1) * 100) %>%</td>
                <td><%= iAttendanceChildGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iAttendanceChildGoalProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <tr>
                <td>Attendance - Students <small>(Middle School, Avg past 6wks)</small></td>
                <td><%= iAttendanceStudents %></td>
                <td><%= iAttendanceStudentsLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iAttendanceStudents / (double)iAttendanceStudentsLastYear) - 1) * 100) %>%</td>
                <td><%= iAttendanceStudentGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iAttendanceStudentGoalProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <tr>
                <td><span style="text-indent: 20px; display: block;"><i>All Attendance <small>(Avg past 6wks)</small></i></span></td>
                <td><%= iAttendanceAll %></td>
                <td><%= iAttendanceAllLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iAttendanceAll / (double)iAttendanceAllLastYear) - 1) * 100) %>%</td>
                <td><%= iAttendanceAllGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iAttendanceAllGoalProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <tr>
                <td>Attendance - High School <small>(Avg past 6wks)</small></td>
                <td><%= iAttendanceHighSchool %></td>
                <td><%= iAttendanceHighSchoolLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iAttendanceHighSchool / (double)iAttendanceHighSchoolLastYear) - 1) * 100) %>%</td>
                <td><%= iAttendanceHighSchoolGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iAttendanceHighSchoolGoalProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <tr>
                <td>Volunteers</td>
                <td><%= iVolunteers %></td>
                <td><%= iVolunteersLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iVolunteers / (double)iVolunteersLastYear) - 1) * 100) %>%</td>
                <td><%= iVolunteersGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iVolunteersGoalProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <tr>
                <td>Commitments</td>
                <td><%= iAllCommitments %></td>
                <td><%= iAllCommitmentsLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iAllCommitments / (double)iAllCommitmentsLastYear) - 1) * 100) %>%</td>
                <td><%= iAllCommitmentsGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iAllCommitmentsGoalProgress) %>%</td>
                <td><%= iAllCommitments8Wk %></td>
                <td><%= iAllCommitments8WkLy %></td>
                <td><%= String.Format("{0:0}", dAllCommitments8WkProgress) %>%</td>
            </tr>
            <tr>
                <td>Baptisms</td>
                <td><%= iBaptisms %></td>
                <td><%= iBaptismsLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iBaptisms / (double)iBaptismsLastYear) - 1) * 100) %>%</td>
                <td><%= iBaptismsGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iBaptismsGoalProgress) %>%</td>
                <td><%= iBaptisms8Wk %></td>
                <td><%= iBaptisms8WkLy %></td>
                <td><%= String.Format("{0:0}", iBaptisms8WkProgress) %>%</td>
            </tr>
            <tr>
                <td>Small Group Participation <small>(People in Groups)</small></td>
                <td><%= iSmallGroupParticipants %></td>
                <td><%= iSmallGroupParticipantsLastYear %></td>
                <td><%= String.Format("{0:00}", (((double)iSmallGroupParticipants / (double)iSmallGroupParticipantsLastYear) - 1) * 100) %>%</td>
                <td><%= iSmallGroupParticipantsGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iSmallGroupParticipantsGoalProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <tr>
                <td>Small Groups <small>(Number of Groups)</small></td>
                <td><%= iSmallGroups %></td>
                <td><%= iSmallGroupsLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iSmallGroups / (double)iSmallGroupsLastYear) - 1) * 100) %>%</td>
                <td><%= iSmallGroupsGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iSmallGroupsGoalProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <tr>
                <td>Small Groups Leaders <small></small></td>
                <td><%= iSmallGroupLeaders %></td>
                <td><%= iSmallGroupLeadersLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iSmallGroupLeaders / (double)iSmallGroupLeadersLastYear) - 1) * 100) %>%</td>
                <td><%= iSmallGroupLeadersGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iSmallGroupLeadersGoalProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <tr>
                <td>Partners</td>
                <td><%= iPartners %></td>
                <td><%= iPartnersLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iPartners / (double)iPartnersLastYear) - 1) * 100) %>%</td>
                <td><%= iPartnersGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iPartnersGoalProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <tr>
                <td>New to NewPointe</td>
                <td><%= iNewtoNewPointe %></td>
                <td><%= iNewtoNewPointeLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iNewtoNewPointe / (double)iNewtoNewPointeLastYear) - 1) * 100) %>%</td>
                <td><%= iNewtoNewPointeGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iNewtoNewPointeGoalProgress) %>%</td>
                <td><%= iNewtoNewPointe8Wk %></td>
                <td><%= iNewtoNewPointe8WkLy %></td>
                <td><%= String.Format("{0:0}", dNewtoNewPointe8WkProgress) %>%</td>
            </tr>
            <tr>
                <td>Discover Groups</td>
                <td><%= iDiscoverGroups %></td>
                <td><%= iDiscoverGroupsLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iDiscoverGroups / (double)iDiscoverGroupsLastYear) - 1) * 100) %>%</td>
                <td><%= iDiscoverGroupsGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iDiscoverGroupsGoalProgress) %>%</td>
                <td><%= iDiscoverGroups8Wk %></td>
                <td><%= iDiscoverGroups8WkLy %></td>
                <td><%= String.Format("{0:0}", dDiscoverGroups8WkProgress) %>%</td>
            </tr>
            <tr>
                <td>New Here Guests</td>
                <td><%= iNewHere %></td>
                <td><%= iNewHereLastYear %></td>
                <td><%= String.Format("{0:0}", (((double)iNewHere / (double)iNewHereLastYear) - 1) * 100) %>%</td>
                <td><%= iNewHereGoalCurrent %></td>
                <td><%= String.Format("{0:0}", iNewHereGoalProgress) %>%</td>
                <td><%= iNewHere8Wk %></td>
                <td><%= iNewHere8WkLy %></td>
                <td><%= String.Format("{0:0}", dNewHere8WkProgress) %>%</td>
            </tr>
            <tr>
                <td>Inactive Follow-up</td>
                <td><%= iInactiveFollowupComplete %></td>
                <td> </td>
                <td> </td>
                <td><%= iInactiveFollowup %></td>
                <td><%= String.Format("{0:0}",iInactiveFollowupProgress) %>%</td>
                <td> </td>
                <td> </td>
                <td> </td>
            </tr>
            <!-- <tr>
                <td>Giving</td>
                <td><%= String.Format("{0:0}", giving * 100) %>%</td>
                <td> </td>
                <td> </td>
                <td>100%</td>
                <td>105% </td>
            </tr>
            <tr>
                <td>Spending</td>
                <td><%= String.Format("{0:0}", expenses * 100)  %>%</td>
                <td> </td>
                <td> </td>
                <td>100%</td>
                <td>98% </td>
            </tr> -->
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
                                Last 8 Week Start Date: <%= Last8WkStartDate %> <br/>
                                Last 8 Week End Date: <%= Last8WkEndDate %> <br/>
                                Last 8 Week Start Date Last Year: <%= Last8WkStartDateLy %> <br/>
                                Last 8 Week End Date Last Year: <%= Last8WkEndDateLy %> <br/>
                                Last 6 Week Start Date: <%= Last6WkStartDate %> <br/>
                                Last 6 Week End Date: <%= Last6WkEndDate %> <br/>
                                Last 6 Week Start Date Last Year: <%= Last6WkStartDateLy %> <br/>
                                Last 6 Week End Date Last Year: <%= Last6WkEndDateLy %> <br/>

                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>