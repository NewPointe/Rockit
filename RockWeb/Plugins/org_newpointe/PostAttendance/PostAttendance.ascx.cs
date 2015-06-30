using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.CheckIn;



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
    public DateTime startDateTime;
    public SchedulePicker schedule;
    public GroupPicker group;

    public String selectedCampus;
    public String selectedGroup;
    public String selectedLocation;

    
               

    protected void Page_Load(object sender, EventArgs e)
    {
        //Generate Campus List
        var campusi = CampusCache.All();
        cpCampus.Campuses = campusi;
        cpCampus.Visible = campusi.Any();
        
        //Generate Group List?


        if (!Page.IsPostBack)
        {

        }

        if (Page.IsPostBack) {

        }

 
    }


    protected void btnSaveEvent_Click(object sender, EventArgs e)
    {

        //Set Variables from form
        //person = ppPerson;
        location = locpLocation;
        campus = cpCampus;
        startDateTime = Convert.ToDateTime(dtpDateTime.SelectedDateTime);
        schedule = spSchedule;
        group = gpGroup;

        //Session["person"] = person;
        Session["location"] = location;
        Session["campus"] = campus;
        Session["startDateTime"] = startDateTime;
        Session["schedule"] = schedule;
        Session["group"] = group;



        selectedCampus = campus.SelectedCampusId.ToString();
        selectedGroup = group.SelectedValue.ToString();
        selectedLocation = location.Location.ToStringSafe();


        //Set Panel Visability
        pnlEventDetails.Visible = true;
        pnlEvent.Visible = false;
        pnlPeople.Visible = true;
        return;

    }



    protected void btnSave_Click(object sender, EventArgs e)
    {

        AttendanceCodeService attendanceCodeService = new AttendanceCodeService(rockContext);
        AttendanceService attendanceService = new AttendanceService(rockContext);
        GroupMemberService groupMemberService = new GroupMemberService(rockContext);
        PersonAliasService personAliasService = new PersonAliasService(rockContext);

        
        // Only create one attendance record per day for each person/schedule/group/location
        var attendance = attendanceService.Get(startDateTime, location.Location.Id, Int32.Parse(schedule.SelectedValue), Int32.Parse(group.SelectedValue), Int32.Parse(person.SelectedValue.ToString()));
        if (attendance == null)
        {
            var primaryAlias = personAliasService.GetPrimaryAlias(Int32.Parse(person.SelectedValue.ToString()));
            if (primaryAlias != null)
            {
                attendance = rockContext.Attendances.Create();
                attendance.LocationId = location.Location.Id;
                attendance.CampusId = campus.SelectedCampusId;
                attendance.ScheduleId = Int32.Parse(schedule.SelectedValue);
                attendance.GroupId = Int32.Parse(gpGroup.SelectedValue);
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


