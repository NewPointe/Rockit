using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using System.Diagnostics;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.CheckIn;
using System.Data;



namespace RockWeb.Plugins.org_newpointe.PostAttendance
{

    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName("Post Attendance")]
    [Category("NewPointe Attendance")]
    [Description("Add attendance to a person after the event.")]


        //LocationId --
        //ScheduleId
        //GroupId
        //DeviceId
        //SearchTypeValueId
        //AttendanceC0deId
        //StartDateTime
        //DidAttend
        //Note
        //Guid
        //CampusId --
        //PersonAliasId --


public partial class PostAttendance : Rock.Web.UI.RockBlock
{


    private ScheduleOccurrence _occurrence = null;

    public RockContext rockContext = new RockContext();
    public PersonPicker person;
    public LocationPicker location;
    public CampusPicker campus;
    public string campusString;
    public DateTime startDateTime;
    public SchedulePicker schedule;
    public GroupPicker group;

    public String selectedCampus;
    public String selectedGroup;
    public String selectedLocation;
    public String selectedStartDateTimeString;
    public String eventName;
    public String campusName;

    
               

    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!Page.IsPostBack)
        {
            //Generate Campus List
            cpCampus.Campuses = CampusCache.All();

            //Set Event List (static for now)
            string[] eventList = { "New to NewPointe" };

            ddlEvent.DataSource = eventList;
            ddlEvent.DataBind();

        }


        if (Page.IsPostBack) {

        }

 
    }



    protected void btnSaveEvent_Click(object sender, EventArgs e)
    {

        //Set Variables from form
        person = ppPerson;
        //campusString = cpCampus.SelectedCampusId.ToString();
        startDateTime = Convert.ToDateTime(dtpDateTime.SelectedDateTime);
        eventName = ddlEvent.SelectedValue.ToString();


        int? newCampusId = cpCampus.SelectedCampusId;
        if (newCampusId.HasValue)
        {
            var campus = CampusCache.Read(newCampusId.Value);
            if (campus != null)
            {
                campusString = newCampusId.ToString();
                campusName = cpCampus.SelectedItem.ToString();
            }
        }


        selectedCampus = campusString;
        //selectedGroup = group.SelectedValue.ToString();
        //selectedLocation = location.Location.ToStringSafe();
        selectedStartDateTimeString = startDateTime.ToString();

        Session["person"] = person;
        //Session["location"] = location;
        Session["campus"] = campusString;
        Session["campusName"] = campusName;
        Session["startDateTime"] = startDateTime;
        Session["eventName"] = eventName;
        //Session["schedule"] = schedule;
        //Session["group"] = group;


        //Set Panel Visability
        pnlEventDetails.Visible = true;
        pnlEvent.Visible = false;
        pnlPeople.Visible = true;
        

        //Set Variables based on Campus and Event selected

        if (Session["eventName"].ToString() == "New to NewPointe")
            {
                schedule = null;

                if (Session["campusName"].ToString() == "Akron Campus")
                {
                    Session["group"] = 1;
                    Session["location"] = 1;
                }
                else if (Session["campusName"].ToString() == "Canton Campus")
                {
                    Session["group"] = 1;
                    Session["location"] = 1;
                }
                else if (Session["campusName"].ToString() == "Coshocton Campus")
                {
                    Session["group"] = 1;
                    Session["location"] = 1;
                }
                else if (Session["campusName"].ToString() == "Dover Campus")
                {
                    Session["group"] = 1;
                    Session["location"] = 1;
                }
                else if (Session["campusName"].ToString() == "Millersburg Campus")
                {
                    Session["group"] = 1;
                    Session["location"] = 1;
                }
                else if (Session["campusName"].ToString() == "Wooster Campus")
                {
                    Session["group"] = 1;
                    Session["location"] = 1;
                }


                Session["schedule"] = 1;
                Session["person"] = person.SelectedValue.ToString();

            }


            return;
        }



    protected void btnSave_Click(object sender, EventArgs e)
    {

        AttendanceCodeService attendanceCodeService = new AttendanceCodeService(rockContext);
        AttendanceService attendanceService = new AttendanceService(rockContext);
        GroupMemberService groupMemberService = new GroupMemberService(rockContext);
        PersonAliasService personAliasService = new PersonAliasService(rockContext);


            // Only create one attendance record per day for each person/schedule/group/location
            Debug.WriteLine(Convert.ToDateTime(Session["startDateTime"]));
            Debug.WriteLine(Session["location"].ToString());
            Debug.WriteLine(Session["schedule"].ToString());
            Debug.WriteLine(Session["group"].ToString());
            Debug.WriteLine(Session["person"].ToString());

        var attendance = attendanceService.Get(Convert.ToDateTime(Session["startDateTime"]), int.Parse(Session["location"].ToString()), int.Parse(Session["schedule"].ToString()), int.Parse(Session["group"].ToString()), int.Parse(Session["person"].ToString()));
 
            if (attendance == null)
        {
            var primaryAlias = personAliasService.GetPrimaryAlias(Int32.Parse(person.SelectedValue.ToString()));
            if (primaryAlias != null)
            {
                attendance = rockContext.Attendances.Create();
                attendance.LocationId = location.Location.Id;
                attendance.CampusId = campus.SelectedCampusId;
                attendance.ScheduleId = Int32.Parse(schedule.SelectedValue);
                //attendance.GroupId = Int32.Parse(gpGroup.SelectedValue);
                attendance.PersonAlias = primaryAlias;
                attendance.PersonAliasId = primaryAlias.Id;
                attendance.DeviceId = null;
                attendance.SearchTypeValueId = 1;
                attendanceService.Add(attendance);
            }
        }

        attendance.AttendanceCodeId = null;
        attendance.StartDateTime = startDateTime;
        attendance.EndDateTime = null;
        attendance.DidAttend = true;

        //KioskLocationAttendance.AddAttendance(attendance);
        rockContext.SaveChanges();

        //Clear Person field
        ppPerson.PersonId = null;


        //Update Current Participants List


    }

 

}
}


