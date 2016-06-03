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
    [DisplayName("Composite Index Scores")]
    [Category("NewPointe Metrics")]
    [Description("Composite Index Scores")]
    [DateField("Fiscal Year Start Date","Select the date the Fiscal Year starts", true)]


    public partial class CompositeScores : Rock.Web.UI.RockBlock
    {


        public string SelectedCampus = string.Empty;
        public int SelectedCampusId;

        public string SundayDate;
        public string SundayDateSQLFormatted;
        public string AttendanceLastWeekCampus;
        public string AttendanceLastWeekLastYearAll;
        public string AttendanceLastWeekLastYearCampus;
        
        public string FinancialStartDate;
        public string FinancialEndDate;
        public string FinancialStartDateLastWeek;
        public string FinancialEndDateLastWeek;
        

        public int? iAttendanceAud;
        public int? iAttendanceAudLastYear;
        public int? iAttendanceKids;
        public int? iAttendanceKidsLastYear;
        public int? iAttendanceStudents;
        public int? iAttendanceStudentsLastYear;
        public int? iAttendanceAll;
        public int? iAttendanceAllLastYear;

        public int? iAttendanceAverage;
        public int? iAttendanceAveragePast6Weeks;

        public int? iAttendanceLastWeekAll;
        public int? iAttendanceLastWeekAud;
        public int? iAttendanceLastWeekChild;
        public int? iAttendanceLastWeekStudent;

        public int? iAttendanceGoalCurrent;
        public int? iAttendanceGoal2020;
        public int? iAttendanceGoalProgress;

        public int? iAttendanceAudGoalCurrent;
        public int? iAttendanceAudGoal2020;
        public double? iAttendanceAudGoalProgress;

        public int? iAttendanceChildGoalCurrent;
        public int? iAttendanceChildGoal2020;
        public double? iAttendanceChildGoalProgress;

        public int? iAttendanceStudentGoalCurrent;
        public int? iAttendanceStudentGoal2020;
        public double? iAttendanceStudentGoalProgress;

        public int? iAttendanceAllGoalCurrent;
        public int? iAttendanceAllGoal2020;
        public double? iAttendanceAllGoalProgress;

        public int? iBaptisms;
        public int? iBaptismsLastYear;
        public int? iBaptismsGoalCurrent;
        public int? iBaptismsGoal2020;
        public double iBaptismsGoalProgress;

        public int? iCommitments;
        public int? iCommitmentsLastYear;
        public int? iCommitmentsGoalCurrent;
        public int? iCommitmentsGoal2020;
        public double iCommitmentsGoalProgress;

        public int? iRecommitments;
        public int? iRecommitmentsLastYear;
        public int? iRecommitmentsGoalCurrent;
        public int? iRecommitmentsGoal2020;
        public double iRecommitmentsGoalProgress;

        public int? iAllCommitments;
        public int? iAllCommitmentsLastYear;
        public int? iAllCommitmentsGoalCurrent;
        public int? iAllCommitmentsGoal2020;
        public double iAllCommitmentsGoalProgress;

        public int? iNewHere;
        public int? iNewHereLastYear;
        public int? iNewHereGoalCurrent;
        public int? iNewHereGoal2020;
        public double iNewHereGoalProgress;

        public int? iPartners;
        public int? iPartnersLastYear;
        public int? iPartnersGoalCurrent;
        public int? iPartnersGoal2020;
        public double iPartnersGoalProgress;

        public int? iSmallGroupLeaders;
        public int? iSmallGroupLeadersLastYear;
        public int? iSmallGroupLeadersGoalCurrent;
        public int? iSmallGroupLeadersGoal2020;
        public double iSmallGroupLeadersGoalProgress;

        public int? iVolunteers;
        public int? iVolunteersLastYear;
        public int? iVolunteersGoalCurrent;
        public int? iVolunteersGoal2020;
        public double iVolunteersGoalProgress;

        public int? iInvolvement;
        public int? iInvolvementLastYear;
        public int? iInvolvementGoalCurrent;
        public int? iInvolvementGoal2020;
        public double iInvolvementGoalProgress;

        public int? iNewtoNewPointe;
        public int? iNewtoNewPointeLastYear;
        public int? iNewtoNewPointeGoalCurrent;
        public int? iNewtoNewPointeGoal2020;
        public double iNewtoNewPointeGoalProgress;

        public int? iDiscoverGroups;
        public int? iDiscoverGroupsLastYear;
        public int? iDiscoverGroupsGoalCurrent;
        public int? iDiscoverGroupsGoal2020;
        public double iDiscoverGroupsGoalProgress;

        public int? iCampusGroups;
        public int? iCampusGroupsLastYear;
        public int? iCampusGroupsGoalCurrent;
        public int? iCampusGroupsGoal2020;
        public double iCampusGroupsGoalProgress;

        public int? iSmallGroupParticipants;
        public int? iSmallGroupParticipantsLastYear;
        public int? iSmallGroupParticipantsGoalCurrent;
        public int? iSmallGroupParticipantsGoal2020;
        public double iSmallGroupParticipantsGoalProgress;

        public int? iInactiveFollowup;
        public int? iInactiveFollowupComplete;
        public int? iInactiveFollowupIncomplete;
        public double iInactiveFollowupProgress;


        public double expenses;
        public double giving;


        public int CurrentMonthInFiscalYear =1;
        public double GoalOffsetMultiplier = 1;
        public double SecondaryGoalOffsetMultiplier = 1;
        public double GoalTarget = .9;
        
        public string sMonth;
        public string sYear;


        public string FiscalYearStartDate;
        public string FiscalYearEndDate;
        public string PeriodStartDate;
        public string PeriodEndDate;
        public string LastPeriodStartDate;
        public string LastPeriodEndDate;

        public bool UseGlobalAttributeGoal;
        public double? GoalMultiplier;

        public double? CompositeScore;



        RockContext rockContext = new RockContext();


        protected void Page_Load(object sender, EventArgs e)
        {



            if (!Page.IsPostBack)
            {
                //Get Attributes
                FiscalYearStartDate = GetAttributeValue("FiscalYearStartDate");

                //Generate Campus List
                string[] campusList = {"Canton Campus", "Coshocton Campus", "Dover Campus", "Millersburg Campus", "Wooster Campus"};
                cpCampus.DataSource = campusList;
                cpCampus.DataBind();

                //Get the campus of the currently logged in person
                PersonService personService = new PersonService(rockContext);
                var personObject = personService.Get(CurrentPerson.Guid);
                var campus = personObject.GetFamilies().FirstOrDefault().Campus ?? new Campus();
                SelectedCampusId = campus.Id;
                cpCampus.SelectedValue = campus.Name;
                SelectedCampus = campus.Name;

            }

            CalulateMetrics();

        }


        protected void cpCampus_OnSelectionChanged(object sender, EventArgs e)
        {

            SelectedCampus = cpCampus.SelectedValue;
            switch (SelectedCampus)
            {
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
            }

            CalulateMetrics();

        }



        protected void CalulateMetrics()
        {


            // specify which attribute key we want to work with
            var attributeKeyCompositeUseGlobalValue = "CompositeUseGlobalValue";
            var attributeKeyCompositeGoalMultiplier = "CompositeGoalMultiplier";


            var attributeValueService = new AttributeValueService(rockContext);

            // specify NULL as the EntityId since this is a GlobalAttribute
            var compositeUseGlobalValue = attributeValueService.GetGlobalAttributeValue(attributeKeyCompositeUseGlobalValue);
            var compositeGoalMultiplier = attributeValueService.GetGlobalAttributeValue(attributeKeyCompositeGoalMultiplier);

            if (bool.Parse(compositeUseGlobalValue.ToString()) == true)
            {
                UseGlobalAttributeGoal = true;
                GoalMultiplier = Convert.ToDouble(compositeGoalMultiplier.ToString());
            }



            //Get Needed dates

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

            sMonth = DateTime.Now.Month.ToString();
            sYear = DateTime.Now.Year.ToString();
            int iMonth = Int32.Parse(sMonth);
            int iYear = Int32.Parse(sYear);

            int lastDayOfCurrentMonth = DateTime.DaysInMonth(iYear, iMonth);
            int lastDayOfLastMonth = DateTime.DaysInMonth(iYear, iMonth - 1);

            DateTime fiscalYearStartDate = new DateTime(iYear, 9, 1);
            DateTime fiscalYearEndDate = new DateTime(iYear + 1, 8, 31);
            DateTime lastFiscalYearStartDate = new DateTime(iYear - 1, 9, 1);
            DateTime lastFiscalYearEndDate = new DateTime(iYear, 8, 31);

            DateTime start2020 = new DateTime(2020, 1, 1);
            DateTime end2020 = new DateTime(2020, 12, 31);

            if (iMonth < 9)
            {
                fiscalYearStartDate = fiscalYearStartDate.AddYears(-1);
                fiscalYearEndDate = fiscalYearEndDate.AddYears(-1);
                lastFiscalYearStartDate = lastFiscalYearStartDate.AddYears(-1);
                lastFiscalYearEndDate = lastFiscalYearEndDate.AddYears(-1);
            }

            DateTime periodEndDate = new DateTime(iYear, iMonth - 1, lastDayOfLastMonth);
            DateTime lastPeriodEndDate = new DateTime(iYear - 1, iMonth - 1, lastDayOfLastMonth);


            FiscalYearStartDate = fiscalYearStartDate.ToShortDateString();
            FiscalYearEndDate = fiscalYearEndDate.ToShortDateString();
            PeriodStartDate = fiscalYearStartDate.ToShortDateString();
            PeriodEndDate = periodEndDate.ToShortDateString();
            LastPeriodStartDate = lastFiscalYearStartDate.ToShortDateString();
            LastPeriodEndDate = lastPeriodEndDate.ToShortDateString();


            switch (iMonth)
            {
                case 09:
                    CurrentMonthInFiscalYear = 1;
                    GoalOffsetMultiplier = .083;
                    SecondaryGoalOffsetMultiplier = .89;
                    break;
                case 10:
                    CurrentMonthInFiscalYear = 2;
                    GoalOffsetMultiplier = .167;
                    SecondaryGoalOffsetMultiplier = .90;
                    break;
                case 11:
                    CurrentMonthInFiscalYear = 3;
                    GoalOffsetMultiplier = .25;
                    SecondaryGoalOffsetMultiplier = .91;
                    break;
                case 12:
                    CurrentMonthInFiscalYear = 4;
                    GoalOffsetMultiplier = .333;
                    SecondaryGoalOffsetMultiplier = .92;
                    break;
                case 01:
                    CurrentMonthInFiscalYear = 5;
                    GoalOffsetMultiplier = .417;
                    SecondaryGoalOffsetMultiplier = .93;
                    break;
                case 02:
                    CurrentMonthInFiscalYear = 6;
                    GoalOffsetMultiplier = .5;
                    SecondaryGoalOffsetMultiplier = .94;
                    break;
                case 03:
                    CurrentMonthInFiscalYear = 7;
                    GoalOffsetMultiplier = .583;
                    SecondaryGoalOffsetMultiplier = .95;
                    break;
                case 04:
                    CurrentMonthInFiscalYear = 8;
                    GoalOffsetMultiplier = .667;
                    SecondaryGoalOffsetMultiplier = .96;
                    break;
                case 05:
                    CurrentMonthInFiscalYear = 9;
                    GoalOffsetMultiplier = .75;
                    SecondaryGoalOffsetMultiplier = .97;
                    break;
                case 06:
                    CurrentMonthInFiscalYear = 10;
                    GoalOffsetMultiplier = .883;
                    SecondaryGoalOffsetMultiplier = .98;
                    break;
                case 07:
                    CurrentMonthInFiscalYear = 11;
                    GoalOffsetMultiplier = .917;
                    SecondaryGoalOffsetMultiplier = .99;
                    break;
                case 08:
                    CurrentMonthInFiscalYear = 12;
                    GoalOffsetMultiplier = 1;
                    SecondaryGoalOffsetMultiplier = 1;
                    break;
            }






            //-------Attendance-------

            //Auditorium
            iAttendanceAud = Get6WkAttendanceAuditorium(SelectedCampusId, periodEndDate);
            iAttendanceAudLastYear = Get6WkAttendanceAuditorium(SelectedCampusId, lastPeriodEndDate);
            iAttendanceAudGoal2020 = GetMetrics(2, SelectedCampusId, start2020, end2020, 1);
            
            //Kids
            iAttendanceKids = Get6WkAttendanceKids(SelectedCampusId, periodEndDate);
            iAttendanceKidsLastYear = Get6WkAttendanceKids(SelectedCampusId, lastPeriodEndDate);
            iAttendanceChildGoal2020 = GetMetrics(3, SelectedCampusId, start2020, end2020, 1);
            
            //Students
            iAttendanceStudents = Get6WkAttendanceStudents(SelectedCampusId, periodEndDate);
            iAttendanceStudentsLastYear = Get6WkAttendanceStudents(SelectedCampusId, lastPeriodEndDate);
            iAttendanceStudentGoal2020 = GetMetrics(5, SelectedCampusId, start2020, end2020, 1);
            


            if (UseGlobalAttributeGoal == true)
            {
                iAttendanceAudGoalCurrent = Convert.ToInt32((double?)iAttendanceAudLastYear * GoalMultiplier);
                iAttendanceChildGoalCurrent = Convert.ToInt32((double?)iAttendanceKidsLastYear * GoalMultiplier);
                iAttendanceStudentGoalCurrent = Convert.ToInt32((double?)iAttendanceStudentsLastYear * GoalMultiplier);
            }
            else
            {
                iAttendanceAudGoalCurrent = GetMetrics(2, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iAttendanceChildGoalCurrent = GetMetrics(3, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iAttendanceStudentGoalCurrent = GetMetrics(5, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
            }


            //All
            iAttendanceAll = Get6WkAttendanceAll(SelectedCampusId, periodEndDate);
            iAttendanceAllLastYear = Get6WkAttendanceAll(SelectedCampusId, lastPeriodEndDate);
            iAttendanceAllGoalCurrent = iAttendanceAudGoalCurrent + iAttendanceAllGoalCurrent + iAttendanceStudentGoalCurrent;
            iAttendanceAllGoal2020 = iAttendanceAudGoal2020 + iAttendanceChildGoal2020 + iAttendanceStudentGoal2020;
            


            //Calculate attendance goal progress
            iAttendanceAudGoalProgress = (double?)iAttendanceAud / ((double?)iAttendanceAudGoalCurrent * SecondaryGoalOffsetMultiplier) * 100;
            iAttendanceChildGoalProgress = (double?)iAttendanceKids / ((double?)iAttendanceChildGoalCurrent * SecondaryGoalOffsetMultiplier) * 100;
            iAttendanceStudentGoalProgress = (double?)iAttendanceStudents / ((double?)iAttendanceStudentGoalCurrent * SecondaryGoalOffsetMultiplier) * 100;
            iAttendanceAllGoalProgress = (double?)iAttendanceAll / ((double?)iAttendanceAllGoalCurrent * SecondaryGoalOffsetMultiplier) * 100;



            //-------Baptisms-------

            //YTD 
            iBaptisms = GetMetrics(11, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //LastYTD 
             iBaptismsLastYear = GetMetrics(11, SelectedCampusId, lastFiscalYearStartDate, lastPeriodEndDate, 0);

            //Current Goal 
             iBaptismsGoalCurrent = GetMetrics(11, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);

            //2020 Goal 
             iBaptismsGoal2020 = GetMetrics(11, SelectedCampusId, start2020, end2020, 1);




            //-------Partners-------

            //YTD 
            iPartners = GetMetricsLatest(20, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //LastYTD 
             iPartnersLastYear = GetMetricsLatest(20, SelectedCampusId, lastFiscalYearStartDate, lastPeriodEndDate, 0);

            //Current Goal 
             iPartnersGoalCurrent = GetMetricsLatest(20, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);

            //2020 Goal 
             iPartnersGoal2020 = GetMetricsLatest(20, SelectedCampusId, start2020, end2020, 1);






            //-------Commitments-------

            //YTD 
            iCommitments = GetMetrics(12, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //LastYTD 
             iCommitmentsLastYear = GetMetrics(12, SelectedCampusId, lastFiscalYearStartDate, lastPeriodEndDate, 0);

            //Current Goal 
             iCommitmentsGoalCurrent = GetMetrics(12, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);

            //2020 Goal 
             iCommitmentsGoal2020 = GetMetrics(12, SelectedCampusId, start2020, end2020, 1);
       





            //-------Recommitments-------

            //YTD 
            iRecommitments = GetMetrics(13, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //LastYTD 
             iRecommitmentsLastYear = GetMetrics(13, SelectedCampusId, lastFiscalYearStartDate, lastPeriodEndDate, 0);

            //Current Goal 
             iRecommitmentsGoalCurrent = GetMetrics(13, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);

            //2020 Goal 
             iRecommitmentsGoal2020 = GetMetrics(13, SelectedCampusId, start2020, end2020, 1);




            //-------Volunteers-------

            //YTD 
            iVolunteers = GetMetricsLatest(16, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //LastYTD 
             iVolunteersLastYear = GetMetricsLatest(16, SelectedCampusId, lastFiscalYearStartDate, lastPeriodEndDate, 0);

            //Current Goal 
             iVolunteersGoalCurrent = GetMetricsLatest(16, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);

            //2020 Goal 
             iVolunteersGoal2020 = GetMetricsLatest(16, SelectedCampusId, start2020, end2020, 1);






            //-------SmallGroupParticipants-------

            //YTD 
            iSmallGroupParticipants = GetMetricsLatest(17, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //LastYTD 
             iSmallGroupParticipantsLastYear = GetMetricsLatest(17, SelectedCampusId, lastFiscalYearStartDate, lastPeriodEndDate, 0);

            //Current Goal 
             iSmallGroupParticipantsGoalCurrent = GetMetricsLatest(17, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);

            //2020 Goal 
             iSmallGroupParticipantsGoal2020 = GetMetricsLatest(17, SelectedCampusId, start2020, end2020, 1);

  





            //-------NewtoNewPointe-------

            //YTD 
            iNewtoNewPointe = GetMetrics(21, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //LastYTD 
             iNewtoNewPointeLastYear = GetMetrics(21, SelectedCampusId, lastFiscalYearStartDate, lastPeriodEndDate, 0);

            //Current Goal 
             iNewtoNewPointeGoalCurrent = GetMetrics(21, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);

            //2020 Goal 
             iNewtoNewPointeGoal2020 = GetMetrics(21, SelectedCampusId, start2020, end2020, 1);







            //-------DiscoverGroups-------

            //YTD 
            iDiscoverGroups = GetMetrics(22, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //LastYTD 
             iDiscoverGroupsLastYear = GetMetrics(22, SelectedCampusId, lastFiscalYearStartDate, lastPeriodEndDate, 0);

            //Current Goal 
             iDiscoverGroupsGoalCurrent = GetMetrics(22, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);

            //2020 Goal 
             iDiscoverGroupsGoal2020 = GetMetrics(2, SelectedCampusId, start2020, end2020, 1);

  





            //-------CampusGroups-------

            //YTD 
            iCampusGroups = GetMetrics(24, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //LastYTD 
             iCampusGroupsLastYear = GetMetrics(24, SelectedCampusId, lastFiscalYearStartDate, lastPeriodEndDate, 0);

            //Current Goal 
             iCampusGroupsGoalCurrent = GetMetrics(24, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);

            //2020 Goal 
             iCampusGroupsGoal2020 = GetMetrics(24, SelectedCampusId, start2020, end2020, 1);





            //-------NewHere-------

            //YTD 
            iNewHere = GetMetrics(14, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //LastYTD 
             iNewHereLastYear = GetMetrics(14, SelectedCampusId, lastFiscalYearStartDate, lastPeriodEndDate, 0);

            //Current Goal 
             iNewHereGoalCurrent = GetMetrics(14, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);

            //2020 Goal 
             iNewHereGoal2020 = GetMetrics(14, SelectedCampusId, start2020, end2020, 1);





            //-------Inactive Followups-------

            //Total 
            iInactiveFollowup = GetMetrics(35, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //Completed 
            iInactiveFollowupComplete = GetMetrics(38, SelectedCampusId, fiscalYearStartDate, periodEndDate, 0);

            //Inconplete
            iInactiveFollowupIncomplete = iInactiveFollowup - iInactiveFollowupComplete;

            //Progress
            iInactiveFollowupProgress = (double)iInactiveFollowupComplete / (double)iInactiveFollowup  * 100;


            //-------Finances-------

            expenses = .99;
            giving = .95;

            //TODO: need to make a method to get these as a double (not an int)




            //Get Goals based on Global Attribute Multiplier or values stored in metrics table

            if (UseGlobalAttributeGoal == true)
            {
                iBaptismsGoalCurrent = Convert.ToInt32((double?)iBaptismsLastYear * GoalMultiplier);
                iPartnersGoalCurrent = Convert.ToInt32((double?)iPartnersLastYear * GoalMultiplier);
                iCommitmentsGoalCurrent = Convert.ToInt32((double?)iCommitmentsLastYear * GoalMultiplier);
                iRecommitmentsGoalCurrent = Convert.ToInt32((double?)iRecommitmentsLastYear * GoalMultiplier);
                iVolunteersGoalCurrent = Convert.ToInt32((double?)iVolunteersLastYear * GoalMultiplier);
                iSmallGroupParticipantsGoalCurrent = Convert.ToInt32((double?)iSmallGroupParticipantsGoalProgress * GoalMultiplier);
                iNewtoNewPointeGoalCurrent = Convert.ToInt32((double?)iNewtoNewPointeLastYear * GoalMultiplier);
                iDiscoverGroupsGoalCurrent = Convert.ToInt32((double?)iDiscoverGroupsLastYear * GoalMultiplier);
                iCampusGroupsGoalCurrent = Convert.ToInt32((double?)iCampusGroupsLastYear * GoalMultiplier);
                iNewHereGoalCurrent = Convert.ToInt32((double?)iNewHereLastYear * GoalMultiplier);

            }
            else
            {
                iBaptismsGoalCurrent = GetMetrics(11, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iPartnersGoalCurrent = GetMetricsLatest(20, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iCommitmentsGoalCurrent = GetMetrics(12, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iRecommitmentsGoalCurrent = GetMetrics(13, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iVolunteersGoalCurrent = GetMetricsLatest(16, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iSmallGroupParticipantsGoalCurrent = GetMetricsLatest(17, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iNewtoNewPointeGoalCurrent = GetMetrics(21, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iDiscoverGroupsGoalCurrent = GetMetrics(22, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iCampusGroupsGoalCurrent = GetMetrics(24, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
                iNewHereGoalCurrent = GetMetrics(14, SelectedCampusId, lastFiscalYearStartDate, fiscalYearEndDate, 1);
            }


            //-------AllCommitments-------

            //YTD 
            iAllCommitments = iCommitments + iRecommitments;

            //LastYTD 
            iAllCommitmentsLastYear = iCommitmentsLastYear + iRecommitmentsLastYear;

            //Current Goal 
            iAllCommitmentsGoalCurrent = iCommitmentsGoalCurrent + iRecommitmentsGoalCurrent;

            //2020 Goal 
            iAllCommitmentsGoal2020 = iCommitmentsGoal2020 + iRecommitmentsGoal2020;

            //Current Goal Progress
            iAllCommitmentsGoalProgress = ((double)iCommitments + (double)iRecommitments) / (((double)iCommitmentsGoalCurrent + (double)iRecommitmentsGoalCurrent) * GoalOffsetMultiplier) * 100;



            // Caculate the progress toeards the goals
            iBaptismsGoalProgress = (double)iBaptisms / ((double)iBaptismsGoalCurrent * GoalOffsetMultiplier) * 100;
            iPartnersGoalProgress = (double)iPartners / ((double)iPartnersGoalCurrent * GoalOffsetMultiplier) * 100;
            iCommitmentsGoalProgress = (double)iCommitments / ((double)iCommitmentsGoalCurrent * GoalOffsetMultiplier) * 100;
            iRecommitmentsGoalProgress = (double)iRecommitments / ((double)iRecommitmentsGoalCurrent * GoalOffsetMultiplier) * 100;
            iVolunteersGoalProgress = (double)iVolunteers / ((double)iVolunteersGoalCurrent * GoalOffsetMultiplier) * 100;
            iSmallGroupParticipantsGoalProgress = (double)iSmallGroupParticipants / ((double)iSmallGroupParticipantsGoalCurrent * GoalOffsetMultiplier) * 100;
            iNewtoNewPointeGoalProgress = (double)iNewtoNewPointe / ((double)iNewtoNewPointeGoalCurrent * GoalOffsetMultiplier) * 100;
            iDiscoverGroupsGoalProgress = (double)iDiscoverGroups / ((double)iDiscoverGroupsGoalCurrent * GoalOffsetMultiplier) * 100;
            iCampusGroupsGoalProgress = (double)iCampusGroups / ((double)iCampusGroupsGoalCurrent * GoalOffsetMultiplier) * 100;
            iNewHereGoalProgress = (double)iNewHere / ((double)iNewHereGoalCurrent * GoalOffsetMultiplier) * 100;




            //Calculate the Composite Score
            CompositeScore = CalculateCompositeScore();




        }



        //Methods for getting metrics from stored procedures

        protected int? GetMetrics(int metricId, int campusId, DateTime startDate, DateTime endDate, int type)
        {
            int? result =
                rockContext.Database.SqlQuery<int?>(
                    "EXEC dbo.spNPMetrics_GetMetricCumulative @metricId, @campusId, @startDate, @endDate, @type",
                    new SqlParameter("metricId", metricId), new SqlParameter("campusId", campusId), new SqlParameter("startDate", startDate), new SqlParameter("endDate", endDate), new SqlParameter("type", type)).ToList<int?>()[0]; ;

            return result;
        }

        protected int? GetMetricsLatest(int metricId, int campusId, DateTime startDate, DateTime endDate, int type)
        {
            int? result =
                rockContext.Database.SqlQuery<int?>(
                    "EXEC dbo.spNPMetrics_GetMetric @metricId, @campusId, @startDate, @endDate, @type",
                    new SqlParameter("metricId", metricId), new SqlParameter("campusId", campusId), new SqlParameter("startDate", startDate), new SqlParameter("endDate", endDate), new SqlParameter("type", type)).ToList<int?>()[0]; ;

            return result;
        }


        protected int? Get6WkAttendanceAll(int campusId, DateTime endDate)
        {
            int? result =
                rockContext.Database.SqlQuery<int?>(
                    "EXEC dbo.spNPMetrics_Attendace6WkAll @campusId, @endDate",
                    new SqlParameter("campusId", campusId), new SqlParameter("endDate", endDate)).ToList<int?>()[0]; ;

            return result;
        }


        protected int? Get6WkAttendanceKids(int campusId, DateTime endDate)
        {
            int? result =
                rockContext.Database.SqlQuery<int?>(
                    "EXEC dbo.spNPMetrics_Attendace6WkKids @campusId, @endDate",
                    new SqlParameter("campusId", campusId), new SqlParameter("endDate", endDate)).ToList<int?>()[0]; ;

            return result;
        }


        protected int? Get6WkAttendanceStudents(int campusId, DateTime endDate)
        {
            int? result =
                rockContext.Database.SqlQuery<int?>(
                    "EXEC dbo.spNPMetrics_Attendace6WkStudents @campusId, @endDate",
                    new SqlParameter("campusId", campusId), new SqlParameter("endDate", endDate)).ToList<int?>()[0]; ;

            return result;
        }

        
        protected int? Get6WkAttendanceAuditorium(int campusId, DateTime endDate)
        {
            int? result =
                rockContext.Database.SqlQuery<int?>(
                    "EXEC dbo.spNPMetrics_Attendace6WkAuditorium @campusId, @endDate",
                    new SqlParameter("campusId", campusId), new SqlParameter("endDate", endDate)).ToList<int?>()[0]; ;

            return result;
        }


        protected double? CalculateCompositeScore()
        {
            double? result = 1;

            // Total Weighted Factors
            double? denominator = 51;

            //Weight for each metric
            double? indexTotal =
                (iVolunteersGoalProgress*7) +
                (iAttendanceAudGoalProgress*6) +
                (iAttendanceChildGoalProgress*4) +
                (iAttendanceStudentGoalProgress*5) +
                (iAllCommitmentsGoalProgress*4) +
                (iBaptismsGoalProgress*5) +
                (iSmallGroupParticipantsGoalProgress*4) +
                (iPartnersGoalProgress*3) +
                (iNewtoNewPointeGoalProgress*3) +
                (iDiscoverGroupsGoalProgress*3) +
                (iNewHereGoalProgress*4) +
                (iInactiveFollowupProgress * 3);

            //Divide to get the score
            result = indexTotal/denominator;


            return result;
        }

    }
}