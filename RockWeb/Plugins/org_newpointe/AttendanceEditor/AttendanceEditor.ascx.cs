using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using System.Data;
using System.Text;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Workflow;


using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using Rock.Security;
using Rock.Web.UI;

namespace RockWeb.Plugins.org_newpointe.AttendanceEditor
{
    /// <summary>
    /// Template block for a TreeView.
    /// </summary>
    [DisplayName("Attendance Editor")]
    [Category("Newpointe Attendance")]
    [Description("Attendance Editor.")]

    public partial class AttendanceEditor : Rock.Web.UI.RockBlock
    {

        RockContext _rockContext = null;
        AttendanceService attendServ = null;
        GroupService groupServ = null;
        LocationService locServ = null;


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            attendanceFilters.Show();
            _rockContext = new RockContext();
            attendServ = new AttendanceService(_rockContext);

            lReadOnlyTitle.Text = "Attendance".FormatAsHtmlTitle();

            if (IsUserAuthorized(Authorization.EDIT))
            {
                rgAttendanceList.IsDeleteEnabled = true;
                //rgAttendanceList.Actions.ShowAdd = true;
            }
            else
            {
                rgAttendanceList.IsDeleteEnabled = false;
                //rgAttendanceList.Actions.ShowAdd = false;
            }

        }




        protected List<GroupType> getCGT(List<GroupType> gtypes, string prefix)
        {
            List<GroupType> cgtypes = new List<GroupType>();
            foreach (var gtype in gtypes)
            {
                gtype.Name = prefix + gtype.Name;
                cgtypes.Add(gtype);
                if (gtype.ChildGroupTypes.Count > 0)
                {
                    cgtypes.AddRange(getCGT(gtype.ChildGroupTypes.ToList(), Server.HtmlDecode("&nbsp;&nbsp;&nbsp;&nbsp;") + prefix));
                }
            }
            return cgtypes;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!IsPostBack)
            {
                doCheckinGroupStuff();
                doStuff();
            }
        }

        protected void doCheckinGroupStuff()
        {
            int checkinConfigFilter = int.TryParse(gtpCheckinConfig.SelectedValue, out checkinConfigFilter) ? checkinConfigFilter : -1;
            int checkinAreaFilter = int.TryParse(gtpCheckinArea.SelectedValue, out checkinAreaFilter) ? checkinAreaFilter : -1;
            int checkinGroupFilter = int.TryParse(rddlCheckinGroup.SelectedValue, out checkinGroupFilter) ? checkinGroupFilter : -1;

            int groupTypePurposeCheckInTemplateId = DefinedValueCache.Read(new Guid(Rock.SystemGuid.DefinedValue.GROUPTYPE_PURPOSE_CHECKIN_TEMPLATE)).Id;
            List<GroupType> checkinConfigs = new GroupTypeService(_rockContext).Queryable().Where(a => a.GroupTypePurposeValueId == groupTypePurposeCheckInTemplateId).OrderBy(a => a.ParentGroupTypes.FirstOrDefault().Id).ThenBy(a => a.Order).ThenBy(a => a.Name).ToList();

            gtpCheckinConfig.GroupTypes = checkinConfigs;
            if (gtpCheckinConfig.Items.FindByValue(checkinConfigFilter.ToString()) != null)
            {
                gtpCheckinConfig.SelectedValue = checkinConfigFilter.ToString();
            }

            gtpCheckinArea.GroupTypes = getCGT(checkinConfigs.Where(a => a.Id == gtpCheckinConfig.SelectedGroupTypeId).ToList(), "");
            if (gtpCheckinArea.Items.FindByValue(checkinAreaFilter.ToString()) != null)
            {
                gtpCheckinArea.SelectedValue = checkinAreaFilter.ToString();
            }

            List<Rock.Model.Group> checkinGroups = new GroupService(_rockContext).Queryable().Where(a => a.GroupTypeId == gtpCheckinArea.SelectedGroupTypeId).OrderBy(a => a.Order).ThenBy(a => a.Name).ToList();
            checkinGroups.Insert(0, new Rock.Model.Group() { Name = "", Id = -1 });
            rddlCheckinGroup.DataSource = checkinGroups;
            rddlCheckinGroup.DataTextField = "Name";
            rddlCheckinGroup.DataValueField = "Id";
            rddlCheckinGroup.DataBind();

            if (rddlCheckinGroup.Items.FindByValue(checkinGroupFilter.ToString()) != null)
            {
                rddlCheckinGroup.SelectedValue = checkinGroupFilter.ToString();
            }
        }

        protected void doStuff()
        {

            int personFilter = ppGroupMember.PersonId ?? -1;
            int checkinGroupFilter = int.TryParse(rddlCheckinGroup.SelectedValue, out checkinGroupFilter) ? checkinGroupFilter : -1;
            SortProperty attendanceSort = rgAttendanceList.SortProperty;

            var attendance = attendServ.Queryable();
            if (checkinGroupFilter != -1)
            {
                attendance = attendance.Where(x => x.GroupId == checkinGroupFilter);
            }
            if (personFilter != -1)
            {
                attendance = attendance.Where(x => x.PersonAlias.PersonId == personFilter);
            }
            if (dateRange.LowerValue != null && dateRange.UpperValue != null)
            {
                attendance = attendance.Where(x => x.StartDateTime > dateRange.LowerValue && x.StartDateTime < dateRange.UpperValue);
            }

            var attendanceData = attendance.Select(x => new AttendanceData { Id = x.Id, CheckinDate = x.StartDateTime, CheckinGroupName = x.Group.Name, PersonName = x.PersonAlias.Person.NickName + " " + x.PersonAlias.Person.LastName, DidAttend = x.DidAttend, DidNotOccur = x.DidNotOccur });

            if (attendanceSort != null)
            {
                attendanceData = attendanceData.Sort(attendanceSort);
            }
            else
            {
                SortProperty prop = new SortProperty();
                prop.Direction = SortDirection.Descending;
                prop.Property = "CheckinDate";
                attendanceData = attendanceData.Sort(prop);
            }

            rgAttendanceList.DataSource = attendanceData.ToList();
            rgAttendanceList.DataKeyNames = new string[] { "Id" };
            rgAttendanceList.DataBind();

        }

        protected void testDel()
        {

        }

        protected void attendanceFilters_ApplyFilterClick(object sender, EventArgs e)
        {
            attendanceFilters.Show();
            doCheckinGroupStuff();
            doStuff();
        }

        protected void gtpGroupType_SelectedIndexChanged(object sender, EventArgs e)
        {
            doCheckinGroupStuff();
        }

        public class AttendanceData
        {
            public int? Id { get; set; }
            public String CheckinGroupName { get; set; }
            public String PersonName { get; set; }
            public DateTime CheckinDate { get; set; }
            public bool? DidAttend { get; set; }
            public bool? DidNotOccur { get; set; }
        }

        protected void rgAttendanceList_GridRebind(object sender, EventArgs e)
        {
            doStuff();
        }

        protected void attendanceDelete_Click(object sender, RowEventArgs e)
        {
            RockContext editContext = new RockContext();
            AttendanceService editAttServe = new AttendanceService(editContext);

            var attendItem = editAttServe.Queryable().Where(x => x.Id == e.RowKeyId).FirstOrDefault();
            if (attendItem.IsAuthorized("Edit", CurrentPerson))
            {
                attendItem.DidAttend = !attendItem.DidAttend;
                attendItem.DidNotOccur = !attendItem.DidAttend;
                editContext.SaveChanges();
            }

            _rockContext = new RockContext();
            attendServ = new AttendanceService(_rockContext);
            doStuff();
        }
    }
}