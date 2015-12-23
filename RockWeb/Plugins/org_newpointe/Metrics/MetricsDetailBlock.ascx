<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MetricsDetailBlock.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Reporting.MetricsDetailBlock" %>

<div class="panel panel-block"> <div class="panel-heading"><h4 class="panel-title"><i class="fa fa-line-chart"></i> Goals Dashboard - <%= SelectedCampus %> <small style="color: slategray;">(for Fiscal Year 2016)</small></h4></div>

<div class="row">
<div class='col-md-4'>
    <div class='col-md-12' style=" border-radius: 4px; border: 1px solid #e7e7e7; border-top: 4px solid #8BC540; box-shadow: 2px 2px 0px 1px rgba(0,0,0,0.05)">
        <div class='col-md-12 text-center'>
            <h4>Weekend Attendance<small style="color: slategray;"> (<%= SundayDate %>)</small></h4>
        </div>

        <div class='row'>    
            <div class='col-md-6 text-right'>
                <span style='font-size:50px'> <%= AttendanceLastWeekAll %></span>
            </div>
            <div class='col-md-6' style="font-size: 80%; padding-top: 13px">
                <%= AttendanceLastWeekAud %> Adults<br/>
                <%= AttendanceLastWeekChild %> Kids<br/>
                <%= AttendanceLastWeekStudent %> Students<br/>
            </div>

            <div class='col-md-12'>
                <!--<div class="progress">
                    <div class="progress-bar progress-bar-success progress-bar-striped" role="progressbar"
                     aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width:40%"> 41 of 100
                    </div>
                </div>-->
                <div class='col-md-12 text-right'>
                    <a href="https://rock.newpointe.org/metrics/attendanceDashboard" class="text-xs-right" style="font-size: 70%">See Trends</a>
                </div>
            </div>
        </div>
    </div>
</div>

<div class='col-md-4'>
    <div class='col-md-12' style=" border-radius: 4px; border: 1px solid #e7e7e7; border-top: 4px solid #800000; box-shadow: 2px 2px 0px 1px rgba(0,0,0,0.05)">
        <div class='col-md-12 text-center'>
            <h4>Commitments & Baptisms</h4>
        </div>

        <div class='row'>    
            <div class='col-md-4 text-right'>
                <span style='font-size:50px'><%= AllCommitments %></span>
            </div>
            <div class='col-md-8' style="font-size: 80%; padding-top: 13px">
                 <%= Commitments %> Commitments<br/>
                 <%= Recommitments %> Re-Commitments<br/>

            </div>

            <div class='col-md-12'>
                 <!--<div class="progress">
                    <div class="progress-bar progress-bar-success progress-bar-striped" role="progressbar"
                     aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width:40%"> 41 of 100
                    </div>
                </div>-->
                <div class='col-md-12 text-right'>
                    <a href="https://rock.newpointe.org/page/525" class="text-xs-right" style="font-size: 70%">See Trends</a>
                </div>
            </div>
        </div>
    </div>
</div>

<div class='col-md-4'>
    <div class='col-md-12' style=" border-radius: 4px; border: 1px solid #e7e7e7; border-top: 4px solid #d2691e; box-shadow: 2px 2px 0px 1px rgba(0,0,0,0.05)">
        <div class='col-md-12 text-center'>
            <h4>Involvement</h4>
        </div>

        <div class='row'>    
            <div class='col-md-6 text-right'>
                <span style='font-size:50px'><%= Involvement %></span>
            </div>
            <div class='col-md-6' style="font-size: 80%; padding-top: 13px">
                <%= Volunteers %> Volunteers<br/>
                <%= SmallGroupLeaders %> Small Group Leaders<br/>
            </div>

            <div class='col-md-12'>
                 <!--<div class="progress">
                    <div class="progress-bar progress-bar-success progress-bar-striped" role="progressbar"
                     aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width:40%"> 41 of 100
                    </div>
                </div>-->
                <div class='col-md-12 text-right'>
                    <a href="https://rock.newpointe.org/page/551" class="text-xs-right" style="font-size: 70%">See Trends</a>
                </div>
            </div>
        </div>
    </div>
</div>
</div>
<div class="row" style="padding-top: 10px;">
<div class='col-md-3'>
    <div class='col-md-12' style=" border-radius: 4px; border: 1px solid #e7e7e7; border-top: 4px solid #8AC097; box-shadow: 2px 2px 0px 1px rgba(0,0,0,0.05)">
        <div class='col-md-12 text-center'>
            <h4>New Here Guests</h4>
        </div>

        <div class='row'>    
            <div class='col-md-6 text-right'>
                <span style='font-size:50px'><%= NewHere %></span>
            </div>
            <div class='col-md-6' style="font-size: 80%; padding-top: 13px">

            </div>

            <div class='col-md-12'>
                 <!--<div class="progress">
                    <div class="progress-bar progress-bar-success progress-bar-striped" role="progressbar"
                     aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width:40%"> 41 of 100
                    </div>
                </div>-->
                <div class='col-md-12 text-right'>
                    <a href="https://rock.newpointe.org/page/525" class="text-xs-right" style="font-size: 70%">See Trends</a>
                </div>
            </div>
        </div>
    </div>
</div>

<div class='col-md-2'>
    <div class='col-md-12' style=" border-radius: 4px; border: 1px solid #e7e7e7; border-top: 4px solid #0000cd; box-shadow: 2px 2px 0px 1px rgba(0,0,0,0.05)">
        <div class='col-md-12 text-center'>
            <h4>Baptisms</h4>
        </div>

        <div class='row'>    
            <div class='col-md-6 text-right'>
                <span style='font-size:50px'><%= Baptisms %></span>
            </div>
            <div class='col-md-6' style="font-size: 80%; padding-top: 13px">

            </div>

            <div class='col-md-12'>
                 <!--<div class="progress">
                    <div class="progress-bar progress-bar-success progress-bar-striped" role="progressbar"
                     aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width:40%"> 41 of 100
                    </div>
                </div>-->
                <div class='col-md-12 text-right'>
                    <a href="https://rock.newpointe.org/page/525" class="text-xs-right" style="font-size: 70%">See Trends</a>
                </div>
            </div>
        </div>
    </div>
</div>

<div class='col-md-4'>
    <div class='col-md-12' style=" border-radius: 4px; border: 1px solid #e7e7e7; border-top: 4px solid #ffd700; box-shadow: 2px 2px 0px 1px rgba(0,0,0,0.05)">
        <div class='col-md-12 text-center'>
            <h4>Assimilation</h4>
        </div>

        <div class='row'>    
            <div class='col-md-6 text-right'>
                <span style='font-size:50px'><%= Assimilation %></span>
            </div>
            <div class='col-md-6' style="font-size: 80%; padding-top: 13px">
                <%= NewtoNewPointe %> New to NewPointe<br/>
                <%= DiscoverGroups %> Discover Group<br/>
                <%= SmallGroupParticipants %> Small Group<br/>
            </div>

            <div class='col-md-12'>
                <!--<div class="progress">
                    <div class="progress-bar progress-bar-success progress-bar-striped" role="progressbar"
                     aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width:40%"> 41 of 100
                    </div>
                </div>-->
                <div class='col-md-12 text-right'>
                    <a href="https://rock.newpointe.org/page/526" class="text-xs-right" style="font-size: 70%">See Trends</a>
                </div>
            </div>
        </div>
    </div>
</div>
    
 <div class='col-md-3'>
    <div class='col-md-12' style=" border-radius: 4px; border: 1px solid #e7e7e7; border-top: 4px solid #cd853f; box-shadow: 2px 2px 0px 1px rgba(0,0,0,0.05)">
        <div class='col-md-12 text-center'>
            <h4>Inactive Follow-up</h4>
        </div>

        <div class='row'>    
            <div class='col-md-6 text-right'>
                <span style='font-size:50px'><%= InactiveFollowup %></span>
            </div>
            <div class='col-md-6' style="font-size: 80%; padding-top: 13px">
                <%= InactiveFollowupComplete %> Completed<br/>
                <%= InactiveFollowupIncomplete %> Open<br/>
            </div>

            <div class='col-md-12'>
                <div class="col-md-6"></div>
                <div class="col-md-6 right"><small>Goal: <%= InactiveFollowup %></small></div>
                <div class='col-md-12'>
                 <div class="progress">
                    <div class="progress-bar progress-bar-<%= InactiveFollowupColor %> progress-bar-striped" role="progressbar"
                     aria-valuenow="40" aria-valuemin="0" aria-valuemax="100" style="width:<%= InactiveFollowupPercentage %>%"> 
                    </div>
                </div>

                </div>
                <div class='col-md-12 text-right'>
                    <a href="https://rock.newpointe.org/page/735" class="text-xs-right" style="font-size: 70%">See Report</a>
                </div>
            </div>
        </div>
    </div>
</div>

</div>
    <div class="panel-footer" style="text-align: right;"><Rock:ButtonDropDownList ID="cpCampus" runat="server" OnSelectionChanged="cpCampus_OnSelectionChanged" ToolTip="Choose a Campus" /></div>
    </div>