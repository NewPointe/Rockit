using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Attribute;
using Rock.Communication;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using System.Web.UI.WebControls;
using System.Data;
using System.Web;
using DotLiquid.Tags;

namespace RockWeb.Plugins.org_newpointe.Partnership
{

    /// <summary>
    /// Block to pick a person and get their URL encoded key.
    /// </summary>
    [DisplayName( "Partnership Signup" )]
    [Category( "NewPointe Partnership" )]
    [Description("Partnership Signup")]



    public partial class PartnershipSignup : Rock.Web.UI.RockBlock
    {

        RockContext rockContext = new RockContext();
        Person _activePerson = new Person();


        protected override void OnLoad( EventArgs e )
        {
            if ( !IsPostBack )
            {
                
            }

            if (CurrentPerson != null)
            {
                _activePerson = CurrentPerson;
                var currentPersonCampus = CurrentPerson.GetCampus();

                string lastAttended = GetDiscoverAttendanceInfo(_activePerson);
                string volunteerGroups = GetServingInfo(_activePerson);

                lDiscover.Text = String.Format( "You attended DISCOVER My Church on {0}.", lastAttended);

                if (lastAttended == "never")
                {
                    lDiscover.Text = "Upcoming DISCOVER My Church Opportunities at the " + currentPersonCampus.Name + ":<br />" + UpcomingDiscover(currentPersonCampus, "DISCOVER My Church");
                }

                lServing.Text = volunteerGroups;

                lGiving.Text = "Thanks for Giving! Your donations are making an eternal difference.  <a href= \"https://newpointe.org/GiveNow \">Click here</a> to start or manage your online giving at NewPointe - it's quick and easy!";

                lPersonInfo.Text = _activePerson.FullName;

            }
            else
            {
                string path = HttpContext.Current.Request.Url.AbsolutePath;
                path = path.UrlEncode();

                mdNotLoggedIn.Show( string.Format("Before you can sign the Partnership Covenant, you must log in with your MyNewPointe account.<br /><br /> <p class=\"text-center\"><a href = \"https://newpointe.org/Login?returnurl={0} \" class=\"btn btn-newpointe\">LOG IN</a> <a href = \"https://newpointe.org/NewAccount?returnurl={0} \" class=\"btn btn-newpointe\">REGISTER</a></p>", path), ModalAlertType.Alert);

                pnlSignup.Visible = false;
                pnlNotLoggedIn.Visible = true;
                pnlSignature.Visible = false;
            }


        }

        protected string GetDiscoverAttendanceInfo(Person thePerson)
        {
            string lastAttendedDate = "never";

            AttendanceService attendanceService = new AttendanceService(rockContext);
            GroupService groupService = new GroupService(rockContext);

            var discoverGroups = groupService.Queryable().Where(g => g.Name.Contains("DISCOVER My Church"));

            var discoverGroupAttendance = attendanceService.Queryable().Where(a => discoverGroups.Contains(a.Group) && a.PersonAliasId == thePerson.PrimaryAliasId);

            if (discoverGroupAttendance.Any())
            {
                discoverGroupAttendance = discoverGroupAttendance.OrderByDescending(a => a.StartDateTime);
                lastAttendedDate =
                    discoverGroupAttendance.Select(a => a.StartDateTime).FirstOrDefault().ToShortDateString();
            }

            return lastAttendedDate;
        }


        protected string UpcomingDiscover(Campus theCampus, string theTitle)
        {

            string upcomingDiscovers = "";

            ContentChannelItemService contentChannelItemService = new ContentChannelItemService(rockContext);

            List<ContentChannelItem> contentChannelItemsList = new List<ContentChannelItem>();

            var upcoming =
                contentChannelItemService.Queryable().Where(a => a.ContentChannelId == 14 && a.Title.Contains(theTitle) && a.StartDateTime >= DateTime.Now);


            foreach (var x in upcoming)
            {
                x.LoadAttributes();

                var campus = x.AttributeValues["Campus"];

                if (campus.ValueFormatted == theCampus.Name)
                    {
                    contentChannelItemsList.Add(x);
                    }

            }

            foreach (var x in contentChannelItemsList)
            {
                x.LoadAttributes();

                string registrationLink = "";

                if (x.AttributeValues["RegistrationLink"].ValueFormatted != "")
                {
                    registrationLink = String.Format("<a href= \"{0}\">Register Now!",
                        x.AttributeValues["RegistrationLink"].Value);
                }

                upcomingDiscovers += String.Format("Date: {0} at {1}. Location: {2}. {3} <br>", x.StartDateTime.ToShortDateString(),
                    x.StartDateTime.ToShortTimeString(), x.AttributeValues["Location"], registrationLink);
            }

            if (!contentChannelItemsList.Any())
            {
                upcomingDiscovers = String.Format("There are not upcoming {0} Opportinuties at the {1}.", theTitle,
                    theCampus.Name);
            }
     

            return upcomingDiscovers;
        }



        protected string GetServingInfo(Person thePerson)
        {

            string volunteerGroupsString = "<a href= \"https://newpointe.org/VolunteerOpportunities \">Click here</a> to check out some incredible serving opportunities at NewPointe. NewPointe Volunteers are making a difference all across Northeast Ohio!";
            string teamTerm = "team";

            GroupMemberService groupMemberService = new GroupMemberService(rockContext);

            var volunteerGroups = groupMemberService.Queryable().Where(m => m.PersonId == thePerson.Id && m.Group.GroupTypeId == 42).Select(m => m.Group.Name).ToList();

            string joined = string.Join(", ", volunteerGroups);


            if (volunteerGroups.Count > 0)
            {
                if (volunteerGroups.Count > 1)
                {
                    teamTerm = "teams";
                }

                volunteerGroupsString = String.Format("Thanks for volunteering on the {0} {1}! Your service is making a difference all across Northeast Ohio.", joined, teamTerm);
            }

            
            return volunteerGroupsString;
        }


        protected void btnSubmit_OnClick(object sender, EventArgs e)
        {

            //TODO: Also set partnership year attribute

            pnlOpportunities.Visible = true;
            pnlSignature.Visible = false;

            AttributeValueService attributeValueService = new AttributeValueService(rockContext);
            PersonService personService = new PersonService(rockContext);

            List<Guid> personGuidList = new List<Guid>();
            personGuidList.Add(_activePerson.Guid);

            var p = attributeValueService.GetByAttributeIdAndEntityId(906, _activePerson.Id);

            var personFromService = personService.GetByGuids(personGuidList).FirstOrDefault();

            p.Value = DateTime.Now.ToString();


            personFromService.ConnectionStatusValue.Value = "Partner";
            personFromService.ConnectionStatusValueId = 65;


            rockContext.SaveChanges();


        }
    }
}