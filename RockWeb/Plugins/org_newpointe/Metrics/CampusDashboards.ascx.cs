using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Financial;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using System.Data.SqlClient;


namespace RockWeb.Plugins.org_newpointe.Metrics
{

    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName("Campus Dashboards")]
    [Category("NewPointe Metrics")]
    [Description("Campus Dashboards")]
    [DateField("Fiscal Year Start Date","Select the date the Fiscal Year starts", true)]


    public partial class CampusDashboards : Rock.Web.UI.RockBlock
    {


        public string FiscalYearStartDate;
        public string SelectedCampus = string.Empty;
        public int SelectedCampusId;

        public string AttendanceAverage;
        public string AttendanceLastWeekAll;
        public string AttendanceLastWeekAud;
        public string AttendanceLastWeekChild;
        public string AttendanceLastWeekStudent;

        public string AttendanceGoalCurrent;
        public string AttendanceGoal2020;
        public string AttendanceGoalProgress;

        public string AttendanceAudGoalCurrent;
        public string AttendanceAudGoal2020;
        public string AttendanceAudGoalProgress;

        public string AttendanceChildGoalCurrent;
        public string AttendanceChildGoal2020;
        public string AttendanceChildGoalProgress;

        public string AttendanceStudentGoalCurrent;
        public string AttendanceStudentGoal2020;
        public string AttendanceStudentGoalProgress;

        public string AttendanceAllGoalCurrent;
        public string AttendanceAllGoal2020;
        public string AttendanceAllGoalProgress;
        
        public string Baptisms;
        public string BaptismsGoalCurrent;
        public string BaptismsGoal2020;
        public string BaptismsGoalProgress;

        public string Commitments;
        public string CommitmentsGoalCurrent;
        public string CommitmentsGoal2020;
        public string CommitmentsGoalProgress;

        public string Recommitments;
        public string RecommitmentsGoalCurrent;
        public string RecommitmentsGoal2020;
        public string RecommitmentsGoalProgress;

        public string AllCommitments;
        public string AllCommitmentsGoalCurrent;
        public string AllCommitmentsGoal2020;
        public string AllCommitmentsGoalProgress;

        public string NewHere;
        public string NewHereGoalCurrent;
        public string NewHereGoal2020;
        public string NewHereGoalProgress;

        public string Partners;
        public string PartnersGoalCurrent;
        public string PartnersGoal2020;
        public string PartnersGoalProgress;

        public string SmallGroupLeaders;
        public string SmallGroupLeadersGoalCurrent;
        public string SmallGroupLeadersGoal2020;
        public string SmallGroupLeadersGoalProgress;

        public string Volunteers;
        public string VolunteersGoalCurrent;
        public string VolunteersGoal2020;
        public string VolunteersGoalProgress;

        public string Involvement;
        public string InvolvementGoalCurrent;
        public string InvolvementGoal2020;
        public string InvolvementGoalProgress;

        public string NewtoNewPointe;
        public string NewtoNewPointeGoalCurrent;
        public string NewtoNewPointeGoal2020;
        public string NewtoNewPointeGoalProgress;

        public string DiscoverGroups;
        public string DiscoverGroupsGoalCurrent;
        public string DiscoverGroupsGoal2020;
        public string DiscoverGroupsGoalProgress;

        public string SmallGroupParticipants;
        public string SmallGroupParticipantsGoalCurrent;
        public string SmallGroupParticipantsGoal2020;
        public string SmallGroupParticipantsGoalProgress;

        public string Assimilation;
        public string AssimilationGoalCurrent;
        public string AssimilationGoal2020;
        public string AssimilationGoalProgress;


        public string SundayDate;

        public string InactiveFollowup;
        public string InactiveFollowupComplete;
        public string InactiveFollowupIncomplete;
        public string InactiveFollowupPercentage;
        public string InactiveFollowupColor;


        public string SundayDateSQLFormatted;
        public string AttendanceLastWeekCampus;
        public string AttendanceLastWeekLastYearAll;
        public string AttendanceLastWeekLastYearCampus;

        public string GivingLastWeek;
        public string GivingYtd; 
        public string GivingLastWeekCampus;
        public string GivingYtdCampus;

        public string GivingTwoWeeksAgo;
        public string GivingTwoWeeksAgoCampus;


        public string FinancialStartDate;
        public string FinancialEndDate;
        public string FinancialStartDateLastWeek;
        public string FinancialEndDateLastWeek;

        public int CurrentMonthInFiscalYear = 1;
        public float GoalOffset = 1;
        


        RockContext rockContext = new RockContext();


        protected void Page_Load(object sender, EventArgs e)
        {

            FiscalYearStartDate = GetAttributeValue("FiscalYearStartDate");

            //Set Last Sunday Date
            DateTime now = DateTime.Now;
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
            SundayDate = dt.ToShortDateString();
            SundayDateSQLFormatted = dt.Date.ToString("yyyy-MM-dd");

            DateTime lastTuesday = DateTime.Now.AddDays(-1);
            while (lastTuesday.DayOfWeek != DayOfWeek.Wednesday)
                lastTuesday = lastTuesday.AddDays(-1);

            DateTime lastWednesday = DateTime.Now.AddDays(-1);
            while (lastWednesday.DayOfWeek != DayOfWeek.Wednesday)
                lastWednesday = lastWednesday.AddDays(-1);

            FinancialStartDate = lastTuesday.AddDays(-7).ToString("yyyy-MM-dd");
            FinancialEndDate = lastWednesday.ToString("yyyy-MM-dd");

            FinancialStartDateLastWeek = lastTuesday.AddDays(-14).ToString("yyyy-MM-dd");
            FinancialEndDateLastWeek = lastWednesday.AddDays(-7).ToString("yyyy-MM-dd");

            string sMonth = DateTime.Now.ToString("M");

            switch (sMonth)
            {
                case "9":
                    CurrentMonthInFiscalYear = 1;
                    GoalOffset = CurrentMonthInFiscalYear/12;
                    break;
                case "10":
                    CurrentMonthInFiscalYear = 2;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
                case "11":
                    CurrentMonthInFiscalYear = 3;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
                case "12":
                    CurrentMonthInFiscalYear = 4;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
                case "1":
                    CurrentMonthInFiscalYear = 5;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
                case "2":
                    CurrentMonthInFiscalYear = 6;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
                case "3":
                    CurrentMonthInFiscalYear = 7;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
                case "4":
                    CurrentMonthInFiscalYear = 8;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
                case "5":
                    CurrentMonthInFiscalYear = 9;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
                case "6":
                    CurrentMonthInFiscalYear = 10;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
                case "7":
                    CurrentMonthInFiscalYear = 11;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
                case "8":
                    CurrentMonthInFiscalYear = 12;
                    GoalOffset = CurrentMonthInFiscalYear / 12;
                    break;
            }

            if (!Page.IsPostBack)
            {
                //Get Attributes
                FiscalYearStartDate = GetAttributeValue("FiscalYearStartDate").ToString();

                //Generate Campus List
                string[] campusList = {"All Org", "Canton Campus", "Coshocton Campus", "Dover Campus", "Millersburg Campus", "Wooster Campus"};
                cpCampus.DataSource = campusList;
                cpCampus.DataBind();

                //Get the campus of the currently logged in person
                //PersonService personService = new PersonService(rockContext);
                //var personObject = personService.Get(CurrentPerson.Guid);
                //var campus = personObject.GetFamilies().FirstOrDefault().Campus ?? new Campus();
                //SelectedCampusId = campus.Id;
                //cpCampus.SelectedValue = campus.Name;
                //SelectedCampus = campus.Name;


                PersonService personService = new PersonService(rockContext);
                GroupService groupService = new GroupService(rockContext);
                GroupMemberService groupMemberService = new GroupMemberService(rockContext);
                var personObject = personService.Get(CurrentPerson.Guid);

                //Is Person in Akron Campus?
                if (groupMemberService.GetByGroupIdAndPersonId(74786, (int)CurrentPersonId).Any() == true)
                {
                    SelectedCampusId = 5;
                    cpCampus.SelectedValue = "Akron Campus";
                    SelectedCampus = "Akron Campus";
                }
                //Is Person in Canton Campus?
                if (groupMemberService.GetByGroupIdAndPersonId(74787, (int)CurrentPersonId).Any() == true)
                {
                    SelectedCampusId = 2;
                    cpCampus.SelectedValue = "Canton Campus";
                    SelectedCampus = "Canton Campus";
                }
                //Is Person in Coshocton Campus?
                if (groupMemberService.GetByGroupIdAndPersonId(74788, (int)CurrentPersonId).Any() == true)
                {
                    SelectedCampusId = 3;
                    cpCampus.SelectedValue = "Coshocton Campus";
                    SelectedCampus = "Coshocton Campus";
                }
                //Is Person in Dover Campus?
                if (groupMemberService.GetByGroupIdAndPersonId(74789, (int)CurrentPersonId).Any() == true)
                {
                    SelectedCampusId = 1;
                    cpCampus.SelectedValue = "Dover Campus";
                    SelectedCampus = "Dover Campus";
                }
                //Is Person in Millersburg Campus?
                if (groupMemberService.GetByGroupIdAndPersonId(74790, (int)CurrentPersonId).Any() == true)
                {
                    SelectedCampusId = 4;
                    cpCampus.SelectedValue = "Millersburg Campus";
                    SelectedCampus = "Millersburg Campus";
                }
                //Is Person in Wooster Campus?
                if (groupMemberService.GetByGroupIdAndPersonId(74791, (int)CurrentPersonId).Any() == true)
                {
                    SelectedCampusId = 6;
                    cpCampus.SelectedValue = "Wooster Campus";
                    SelectedCampus = "Wooster Campus";
                }
                //Is Person in Central?
                if (groupMemberService.GetByGroupIdAndPersonId(74785, (int)CurrentPersonId).Any() == true)
                {
                    cpCampus.SelectedValue = "All Org";
                    SelectedCampus = "All Org";
                    SelectedCampusId = 0;
                }


            }

            DoSQL();

        }


        protected void cpCampus_OnSelectionChanged(object sender, EventArgs e)
        {

            SelectedCampus = cpCampus.SelectedValue.ToString();
            switch (SelectedCampus)
            {
                case "Akron Campus":
                    SelectedCampusId = 6;
                    break;
                case "Canton Campus":
                    SelectedCampusId = 2;
                    break;
                case "Coshocton Campus":
                    SelectedCampusId = 3;
                    break;
                case "Dover Campus":
                    SelectedCampusId = 1;
                    break;
                case "Millersburg Campus":
                    SelectedCampusId = 4;
                    break;
                case "Wooster Campus":
                    SelectedCampusId = 5;
                    break;
                case "All Org":
                    SelectedCampusId = 0;
                    break;
            }

            DoSQL();

        }



        protected void DoSQL()
        {
            //Find the average attendacne over the past year
            if (SelectedCampusId == 0)
            {
                AttendanceAverage = rockContext.Database.SqlQuery<int>(@"SELECT CAST(AVG(att) as int)
                FROM
                (
                SELECT TOP 50 SUM(mv.YValue) as att, DATEPART(isowk, mv.MetricValueDateTime) as weeknumber
                FROM MetricValue mv
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 2 OR mv.MetricId = 3 OR mv.MetricId = 4 OR mv.MetricId = 5) AND mv.EntityId != 8 AND mv.MetricValueDateTime > DATEADD(week, -50, GETDATE())
                GROUP by DATEPART(isowk, mv.MetricValueDateTime)
                ) inner_query
                ").ToList<int>()[0].ToString();
            }
            else
            {
                AttendanceAverage = rockContext.Database.SqlQuery<int>(@"SELECT CAST(AVG(att) as int)
                FROM
                (
                SELECT TOP 50 SUM(mv.YValue) as att, DATEPART(isowk, mv.MetricValueDateTime) as weeknumber
                FROM MetricValue mv
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 2 OR mv.MetricId = 3 OR mv.MetricId = 4 OR mv.MetricId = 5) AND mv.EntityId = @CampusId AND mv.MetricValueDateTime > DATEADD(week, -50, GETDATE())
                GROUP by DATEPART(isowk, mv.MetricValueDateTime)
                ) inner_query
                ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int>()[0].ToString();
            }

            //Attendance Last Week - All Environments
            if (SelectedCampusId == 0)
            {
                AttendanceLastWeekAll = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 2 OR mv.MetricId = 3 OR mv.MetricId = 4 OR mv.MetricId = 5) AND(DATEPART(isowk, mv.MetricValueDateTime) = DATEPART(isowk, GETDATE()) - 1)
                AND(DATEPART(yy, mv.MetricValueDateTime) = DATEPART(yy, GETDATE())) AND mv.EntityId != 8; ")
                    .ToList<int?>()[0].ToString();

                AttendanceGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv
                WHERE mv.MetricValueType = 1 AND (mv.MetricId = 2 OR mv.MetricId = 3 OR mv.MetricId = 4 OR mv.MetricId = 5 OR mv.MetricId = 23) 
				AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())")
                    .ToList<int?>()[0].ToString();

                AttendanceGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv
                WHERE mv.MetricValueType = 1 AND (mv.MetricId = 2 OR mv.MetricId = 3 OR mv.MetricId = 4 OR mv.MetricId = 5 OR mv.MetricId = 23) 
				AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'")
                    .ToList<int?>()[0].ToString();



            }
            else
            {
                AttendanceLastWeekAll = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 2 OR mv.MetricId = 3 OR mv.MetricId = 4 OR mv.MetricId = 5) AND(DATEPART(isowk, mv.MetricValueDateTime) = DATEPART(isowk, GETDATE()) - 1)
                AND(DATEPART(yy, mv.MetricValueDateTime) = DATEPART(yy, GETDATE())) AND mv.EntityId = @CampusId; ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                AttendanceGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv
                WHERE mv.MetricValueType = 1 AND (mv.MetricId = 2 OR mv.MetricId = 3 OR mv.MetricId = 4 OR mv.MetricId = 5 OR mv.MetricId = 23) 
				AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                AttendanceGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv
                WHERE mv.MetricValueType = 1 AND (mv.MetricId = 2 OR mv.MetricId = 3 OR mv.MetricId = 4 OR mv.MetricId = 5 OR mv.MetricId = 23) 
				AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();
            }

            //Attendance Last Week - Auditorium
            if (SelectedCampusId == 0)
            {
                AttendanceLastWeekAud = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 2) AND DATEPART(isowk, mv.MetricValueDateTime) = DATEPART(isowk, GETDATE())-1 AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) AND mv.EntityId != 8").ToList<int?>()[0].ToString();

                AttendanceAudGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 2 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                AttendanceAudGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 2 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();
            }

            else
            {
                AttendanceLastWeekAud = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 2) AND DATEPART(isowk, mv.MetricValueDateTime) = DATEPART(isowk, GETDATE())-1 
                AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) AND mv.EntityId = @CampusId; ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                AttendanceChildGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE (mv.MetricId = 3 OR mv.MetricId = 4) AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                AttendanceChildGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE (mv.MetricId = 3 OR mv.MetricId = 4) AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();
            }

            //Attendance Last Week - Rainforest + Velocity
            if (SelectedCampusId == 0)
            {
                AttendanceLastWeekChild = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 3 OR mv.MetricId = 4) AND DATEPART(isowk, mv.MetricValueDateTime) = DATEPART(isowk, GETDATE())-1 AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) AND mv.EntityId != 8")
                    .ToList<int?>()[0].ToString();

                AttendanceChildGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE (mv.MetricId = 3 OR mv.MetricId = 4) AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                AttendanceChildGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE (mv.MetricId = 3 OR mv.MetricId = 4) AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();

            }
            else
            {
                AttendanceLastWeekChild = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 3 OR mv.MetricId = 4) AND DATEPART(isowk, mv.MetricValueDateTime) = DATEPART(isowk, GETDATE())-1
                AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) AND mv.EntityId = @CampusId; ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                AttendanceChildGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE (mv.MetricId = 3 OR mv.MetricId = 4) AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                AttendanceChildGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE (mv.MetricId = 3 OR mv.MetricId = 4) AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

            }




            //Attendance Last Week - The Collective
            if (SelectedCampusId == 0)
            {
                AttendanceLastWeekStudent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 5 OR mv.MetricId = 23) AND DATEPART(isowk, mv.MetricValueDateTime) = DATEPART(isowk, GETDATE())-1 AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) AND mv.EntityId != 8")
                    .ToList<int?>()[0].ToString();

                AttendanceStudentGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE (mv.MetricId = 5 OR mv.MetricId = 23) AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                AttendanceStudentGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE (mv.MetricId = 5 OR mv.MetricId = 23) AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();

            }
            else
            {
                AttendanceLastWeekStudent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue),0) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 5 OR mv.MetricId = 23) AND DATEPART(isowk, mv.MetricValueDateTime) = 
                DATEPART(isowk, GETDATE())-1 AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) AND mv.EntityId = @CampusId; ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                AttendanceStudentGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE (mv.MetricId = 5 OR mv.MetricId = 23) AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                AttendanceStudentGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE (mv.MetricId = 5 OR mv.MetricId = 23) AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();
            }


            //AttendanceGoalCurrent = (Int32.Parse(AttendanceStudentGoalCurrent) + Int32.Parse(AttendanceChildGoalCurrent) + Int32.Parse(AttendanceGoalCurrent)).ToString();
            //AttendanceGoal2020 = (Int32.Parse(AttendanceStudentGoal2020) + Int32.Parse(AttendanceChildGoal2020) + Int32.Parse(AttendanceGoal2020)).ToString();


            //Baptisms
            if (SelectedCampusId == 0)
            {
                Baptisms = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 11) AND mv.MetricValueDateTime >= '2015-09-01'").ToList<int?>()[0].ToString();

                BaptismsGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 11 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                BaptismsGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 11 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();


            }
            else
            {
                Baptisms = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 11) AND mv.MetricValueDateTime >= '2015-09-01' AND mv.EntityId = @CampusId; ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                BaptismsGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 11 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                BaptismsGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 11 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

             }



            //First Time Commitments
            if (SelectedCampusId == 0)
            {
                Commitments = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 12) AND mv.MetricValueDateTime >= '2015-09-01'").ToList<int?>()[0].ToString();

                CommitmentsGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 12 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                CommitmentsGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 12 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();


            }
            else
            {
                Commitments = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 12) AND mv.MetricValueDateTime >= '2015-09-01' AND mv.EntityId = @CampusId; ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                CommitmentsGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 12 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                CommitmentsGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 12 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();
            }


            //Re-commitments
            if (SelectedCampusId == 0)
            {
                Recommitments = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND mv.MetricValueType = 0 AND (mv.MetricId = 13) AND mv.MetricValueDateTime >= '2015-09-01'").ToList<int?>()[0].ToString();

                RecommitmentsGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 13 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                RecommitmentsGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 13 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();


            }
            else
            {
                Recommitments = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 13) AND mv.MetricValueDateTime >= '2015-09-01' AND mv.EntityId = @CampusId; ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                RecommitmentsGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 13 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                RecommitmentsGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 13 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

            }

            //Total Commitments
            if (SelectedCampusId == 0)
            {
                AllCommitments = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 12 OR mv.MetricId = 13) AND mv.MetricValueDateTime >= '2015-09-01'").ToList<int?>()[0].ToString();
            }
            else
            {
                AllCommitments = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 12 OR mv.MetricId = 13) AND mv.MetricValueDateTime >= '2015-09-01' AND mv.EntityId = @CampusId; ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();
            }

            //New Here Guests
            if (SelectedCampusId == 0)
            {
                NewHere = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 14) AND mv.MetricValueDateTime >= '2015-09-01'").ToList<int?>()[0].ToString();

                NewHereGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 14 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                NewHereGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 14 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();

            }

            else
            {
                NewHere = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(SUM(YValue) as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricValueType = 0 AND (mv.MetricId = 14) AND mv.MetricValueDateTime >= '2015-09-01' AND mv.EntityId = @CampusId; ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                NewHereGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 14 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                NewHereGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 14 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

            }

            //Partners
            if (SelectedCampusId == 0)
            {
                Partners = rockContext.Database.SqlQuery<int?>(@"SELECT TOP 1 CAST(YValue as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricId = 20
				ORDER BY MetricValueDateTime DESC;").ToList<int?>()[0].ToString();

                PartnersGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 20 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                PartnersGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 20 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();

            }
            else
            {
                Partners = rockContext.Database.SqlQuery<int?>(@"SELECT TOP 1 CAST(YValue as int) as att
                FROM MetricValue mv 
                WHERE mv.MetricId = 20 AND mv.EntityId = @CampusId
				ORDER BY MetricValueDateTime DESC; ", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                PartnersGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 20 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                PartnersGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 20 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

            }

            //Small Group Leaders
            if (SelectedCampusId == 0)
            {
                SmallGroupLeaders =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT TOP 1 CAST(ISNULL(SUM(YValue), 0) as int) as att
	                    FROM MetricValue mv 
	                    WHERE mv.MetricValueType = 0 AND (mv.MetricId = 18) AND DATEPART(month, mv.MetricValueDateTime) = DATEPART(month, GETDATE()) AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE())").ToList<int?>()[0].ToString();

                SmallGroupLeadersGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 18 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                SmallGroupLeadersGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 18 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();

            }
            else
            {
                SmallGroupLeaders =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT TOP 1 CAST(ISNULL(SUM(YValue), 0) as int) as att
	                    FROM MetricValue mv 
	                    WHERE mv.MetricValueType = 0 AND (mv.MetricId = 18) AND DATEPART(month, mv.MetricValueDateTime) = DATEPART(month, GETDATE()) AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) 
                        AND mv.EntityId = @CampusId", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                SmallGroupLeadersGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 18 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                SmallGroupLeadersGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 18 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();
            }

            //Volunteers
            if (SelectedCampusId == 0)
            {
                Volunteers =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT TOP 1 CAST(ISNULL(SUM(YValue), 0) as int) as att
	                    FROM MetricValue mv 
	                    WHERE mv.MetricValueType = 0 AND (mv.MetricId = 16) AND DATEPART(month, mv.MetricValueDateTime) = DATEPART(month, GETDATE()) 
                        AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) ").ToList<int?>()[0].ToString();

                VolunteersGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 16 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                VolunteersGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 16 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();

            }
            else
            {
                Volunteers =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT TOP 1 CAST(ISNULL(SUM(YValue), 0) as int) as att
	                    FROM MetricValue mv 
	                    WHERE mv.MetricValueType = 0 AND (mv.MetricId = 16) AND DATEPART(month, mv.MetricValueDateTime) = DATEPART(month, GETDATE()) AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) 
                        AND mv.EntityId = @CampusId", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                VolunteersGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 16 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                VolunteersGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 16 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

            }

            //Total Involvement
            if (SelectedCampusId == 0)
            {
                Involvement =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT COUNT(DISTINCT PersonId) as [count] FROM [GroupMember] gm
                JOIN [Group] g on gm.GroupId = g.Id
                WHERE ((g.GroupTypeId = 25 and GroupRoleId = 24) OR g.GroupTypeId = 42) and g.IsActive = 'true';")
                        .ToList<int?>()[0].ToString();
            }
            else
            {
                Involvement =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT COUNT(DISTINCT PersonId) as [count] FROM [GroupMember] gm
                        JOIN [Group] g on gm.GroupId = g.Id
                        WHERE ((g.GroupTypeId = 25 and GroupRoleId = 24) OR g.GroupTypeId = 42) and g.IsActive = 'true' 
                        AND g.CampusId = @CampusId", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();
            }

            //New to NewPointe
            if (SelectedCampusId == 0)
            {
                NewtoNewPointe =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT COUNT(DISTINCT PersonAliasId) as [count] FROM Attendance a
                        JOIN [Group] g on a.GroupId = g.Id
                        WHERE (g.GroupTypeId = 64 OR g.GroupTypeId = 71 OR g.GroupTypeId = 121 OR g.GroupTypeId = 122 OR g.GroupTypeId = 123 OR
                        g.GroupTypeId = 124 OR g.GroupTypeId = 125) AND a.StartDateTime > '2015-09-01'").ToList<int?>()[0].ToString();

                NewtoNewPointeGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 21 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                NewtoNewPointeGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 21 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();
            }
            else
            {
                NewtoNewPointe =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT COUNT(DISTINCT PersonAliasId) as [count] FROM Attendance a
                        JOIN [Group] g on a.GroupId = g.Id
                        WHERE (g.GroupTypeId = 64 OR g.GroupTypeId = 71 OR g.GroupTypeId = 121 OR g.GroupTypeId = 122 OR g.GroupTypeId = 123 OR
                        g.GroupTypeId = 124 OR g.GroupTypeId = 125) AND a.StartDateTime > '2015-09-01' AND g.CampusId = @CampusId", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                NewtoNewPointeGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 21 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                NewtoNewPointeGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 21 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

            }

            //Discover Groups
            if (SelectedCampusId == 0)
            {
                DiscoverGroups =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT COUNT(DISTINCT PersonAliasId) as [count] FROM Attendance a
                JOIN [Group] g on a.GroupId = g.Id
                WHERE (g.GroupTypeId = 62 OR g.GroupTypeId = 63 OR g.GroupTypeId = 65 OR g.GroupTypeId = 66 OR g.GroupTypeId = 67  OR g.GroupTypeId = 72 OR g.GroupTypeId = 86
                OR g.GroupTypeId = 96 OR g.GroupTypeId = 97 OR g.GroupTypeId = 98 OR g.GroupTypeId = 108 OR g.GroupTypeId = 113 OR g.GroupTypeId = 120
                OR g.GroupTypeId = 142 OR g.GroupTypeId = 143  OR g.GroupTypeId = 144) AND a.StartDateTime > '2015-09-01';").ToList<int?>()[0].ToString();

                DiscoverGroupsGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 22 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                DiscoverGroupsGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 22 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();
            }
            else
            {
                DiscoverGroups =
                   rockContext.Database.SqlQuery<int?>(
                       @"SELECT COUNT(DISTINCT PersonAliasId) as [count] FROM Attendance a
                JOIN [Group] g on a.GroupId = g.Id
                WHERE (g.GroupTypeId = 62 OR g.GroupTypeId = 63 OR g.GroupTypeId = 65 OR g.GroupTypeId = 66 OR g.GroupTypeId = 67  OR g.GroupTypeId = 72 OR g.GroupTypeId = 86
                OR g.GroupTypeId = 96 OR g.GroupTypeId = 97 OR g.GroupTypeId = 98 OR g.GroupTypeId = 108 OR g.GroupTypeId = 113 OR g.GroupTypeId = 120
                OR g.GroupTypeId = 142 OR g.GroupTypeId = 143  OR g.GroupTypeId = 144) AND a.StartDateTime > '2015-09-01'AND g.CampusId = @CampusId", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                DiscoverGroupsGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 22 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

               DiscoverGroupsGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 22 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();
            }

            //Small Group Participants
            if (SelectedCampusId == 0)
            {
                SmallGroupParticipants =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT TOP 1 CAST(ISNULL(SUM(YValue), 0) as int) as att
	                    FROM MetricValue mv 
	                    WHERE mv.MetricValueType = 0 AND (mv.MetricId = 17) AND DATEPART(month, mv.MetricValueDateTime) = DATEPART(month, GETDATE()) 
                        AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) ").ToList<int?>()[0].ToString();

                SmallGroupParticipantsGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 17 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE())").ToList<int?>()[0].ToString();

                SmallGroupParticipantsGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 17 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020'").ToList<int?>()[0].ToString();
            }
            else
            {
                SmallGroupParticipants =
                    rockContext.Database.SqlQuery<int?>(
                        @"SELECT TOP 1 CAST(ISNULL(SUM(YValue), 0) as int) as att
	                    FROM MetricValue mv 
	                    WHERE mv.MetricValueType = 0 AND (mv.MetricId = 17) AND DATEPART(month, mv.MetricValueDateTime) = DATEPART(month, GETDATE()) AND DATEPART(year, mv.MetricValueDateTime) = DATEPART(year, GETDATE()) 
                        AND mv.EntityId = @CampusId", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                SmallGroupParticipantsGoalCurrent = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 17 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = DATEPART(YEAR,GETDATE()) AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();

                SmallGroupParticipantsGoal2020 = rockContext.Database.SqlQuery<int?>(@"SELECT CAST(ISNULL(SUM(YValue), 0) as int) as att
	            FROM MetricValue mv 
                WHERE mv.MetricId = 17 AND MetricValueType = 1
	            AND DATEPART(YEAR,mv.MetricValueDateTime) = '2020' AND mv.EntityId = @CampusId;", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();
            }



            //All Assimilation
            if (SelectedCampusId == 0)
            {
                Assimilation = rockContext.Database.SqlQuery<int?>(@"SELECT SUM(thetotal) FROM
                (
                SELECT COUNT(DISTINCT PersonAliasId) as thetotal FROM Attendance a
                JOIN [Group] g on a.GroupId = g.Id
                WHERE (g.GroupTypeId = 62 OR g.GroupTypeId = 63 OR g.GroupTypeId = 65 OR g.GroupTypeId = 66 OR g.GroupTypeId = 67  OR g.GroupTypeId = 72 OR g.GroupTypeId = 86
                OR g.GroupTypeId = 96 OR g.GroupTypeId = 97 OR g.GroupTypeId = 98 OR g.GroupTypeId = 108 OR g.GroupTypeId = 113 OR g.GroupTypeId = 120
                OR g.GroupTypeId = 142 OR g.GroupTypeId = 143  OR g.GroupTypeId = 144 OR g.GroupTypeId = 64 OR g.GroupTypeId = 71 OR g.GroupTypeId = 121 OR g.GroupTypeId = 122 OR g.GroupTypeId = 123 OR g.GroupTypeId = 124 OR g.GroupTypeId = 125)
                AND a.StartDateTime > '2015-09-01'
                UNION
                SELECT COUNT(DISTINCT PersonId) as thetotal FROM [GroupMember] gm
                JOIN [Group] g on gm.GroupId = g.Id
                WHERE g.GroupTypeId = 25 and g.IsActive = 'true'
                ) s").ToList<int?>()[0].ToString();
            }
            else
            {
                Assimilation = rockContext.Database.SqlQuery<int?>(@"SELECT SUM(thetotal) FROM
                (
                SELECT COUNT(DISTINCT PersonAliasId) as thetotal FROM Attendance a
                JOIN [Group] g on a.GroupId = g.Id
                WHERE (g.GroupTypeId = 62 OR g.GroupTypeId = 63 OR g.GroupTypeId = 65 OR g.GroupTypeId = 66 OR g.GroupTypeId = 67  OR g.GroupTypeId = 72 OR g.GroupTypeId = 86
                OR g.GroupTypeId = 96 OR g.GroupTypeId = 97 OR g.GroupTypeId = 98 OR g.GroupTypeId = 108 OR g.GroupTypeId = 113 OR g.GroupTypeId = 120
                OR g.GroupTypeId = 142 OR g.GroupTypeId = 143  OR g.GroupTypeId = 144 OR g.GroupTypeId = 64 OR g.GroupTypeId = 71 OR g.GroupTypeId = 121 OR g.GroupTypeId = 122 OR g.GroupTypeId = 123 OR g.GroupTypeId = 124 OR g.GroupTypeId = 125)
                AND a.StartDateTime > '2015-09-01' AND g.CampusId = @CampusId
                UNION
                SELECT COUNT(DISTINCT PersonId) as thetotal FROM [GroupMember] gm
                JOIN [Group] g on gm.GroupId = g.Id
                WHERE g.GroupTypeId = 25 and g.IsActive = 'true' AND g.CampusId = @CampusId
                ) s", new SqlParameter("CampusId", SelectedCampusId)).ToList<int?>()[0].ToString();
            }

            //Inactive Follow-up
            if (SelectedCampusId == 0)
            {
                InactiveFollowupComplete = rockContext.Database.SqlQuery<int?>(@"SELECT COUNT(wf.Id) as Workflows
                  FROM [rock-production].[dbo].[Workflow] wf 
                  JOIN AttributeValue av ON wf.Id = av.EntityID
                  WHERE wf.WorkflowTypeId = 120 AND av.AttributeId = 10213 AND wf.[Status] = 'Completed'
                  AND Month(ActivatedDateTime) = Month(GETDATE()) AND Year(ActivatedDateTime) = Year(GETDATE())")
                    .ToList<int?>()[0].ToString();
            }
            else
            {
                InactiveFollowupComplete = rockContext.Database.SqlQuery<int?>(@"SELECT COUNT(wf.Id) as Workflows
                  FROM [rock-production].[dbo].[Workflow] wf 
                  JOIN AttributeValue av ON wf.Id = av.EntityID
                  WHERE wf.WorkflowTypeId = 120 AND av.AttributeId = 10213 AND wf.[Status] = 'Completed' AND av.Value = @CampusName
                  AND Month(ActivatedDateTime) = Month(GETDATE()) AND Year(ActivatedDateTime) = Year(GETDATE())", new SqlParameter("CampusName", SelectedCampus)).ToList<int?>()[0].ToString();
            }

            if (SelectedCampusId == 0)
            {
                InactiveFollowupIncomplete = rockContext.Database.SqlQuery<int?>(@"SELECT COUNT(wf.Id) as Workflows
                  FROM [rock-production].[dbo].[Workflow] wf 
                  JOIN AttributeValue av ON wf.Id = av.EntityID
                  WHERE wf.WorkflowTypeId = 120 AND av.AttributeId = 10213 AND wf.[Status] = 'Active'
                  AND Month(ActivatedDateTime) = Month(GETDATE()) AND Year(ActivatedDateTime) = Year(GETDATE())")
                    .ToList<int?>()[0].ToString();
            }
            else
            {
                InactiveFollowupIncomplete = rockContext.Database.SqlQuery<int?>(@"SELECT COUNT(wf.Id) as Workflows
                  FROM [rock-production].[dbo].[Workflow] wf 
                  JOIN AttributeValue av ON wf.Id = av.EntityID
                  WHERE wf.WorkflowTypeId = 120 AND av.AttributeId = 10213 AND wf.[Status] = 'Active' AND av.Value = @CampusName
                  AND Month(ActivatedDateTime) = Month(GETDATE()) AND Year(ActivatedDateTime) = Year(GETDATE())", new SqlParameter("CampusName", SelectedCampus)).ToList<int?>()[0].ToString();
            }

            var totalFollowups = Int32.Parse(InactiveFollowupComplete) + Int32.Parse(InactiveFollowupIncomplete);
            decimal followupPercent = 0;
            InactiveFollowup = totalFollowups.ToString();
            if (InactiveFollowup != "0")
            {
                followupPercent = (decimal.Parse(InactiveFollowupComplete) / decimal.Parse(InactiveFollowup)) * 100;
                InactiveFollowupPercentage = followupPercent.ToString();
            }
            else
            {
                InactiveFollowupPercentage = "100";
            }
            if (followupPercent <= 30)
            {
                InactiveFollowupColor = "danger";
            }
            else if (followupPercent <= 60)
            {
                InactiveFollowupColor = "warning";
            }
            else if (followupPercent <= 90)
            {
                InactiveFollowupColor = "info";
            }
            else if (followupPercent <= 100)
            {
                InactiveFollowupColor = "success";
            }

        }
    }
}