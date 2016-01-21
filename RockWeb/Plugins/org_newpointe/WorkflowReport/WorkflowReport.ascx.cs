using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Data;
using Rock.Security;
using Rock.Model;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.org_newpointe.WorkflowReport
{
    /// <summary>
    /// Template block for a TreeView.
    /// </summary>
    [DisplayName("Workflow Report")]
    [Category("Newpointe Reporting")]
    [Description("Workflow report/stats.")]

    public partial class WorkflowReport : Rock.Web.UI.RockBlock
    {

        RockContext _rockContext = null;
        WorkflowTypeService workTypeServ = null;
        WorkflowService workServ = null;
        WorkflowActivityService workActServ = null;
        CampusService campServ = null;
        CategoryService catServ = null;

        protected string workflowChartData1;
        protected string workflowChartData2;
        protected string workflowChartData3;
        protected string workflowChartData4;
        protected string workflowChartData5;


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _rockContext = new RockContext();
            workServ = new WorkflowService(_rockContext);
            workTypeServ = new WorkflowTypeService(_rockContext);
            workActServ = new WorkflowActivityService(_rockContext);
            campServ = new CampusService(_rockContext);
            catServ = new CategoryService(_rockContext);

            dateRange.UpperValue = DateTime.Now;
            dateRange.LowerValue = DateTime.Parse("4/15/2015");// DateTime.Now.AddYears(-1);

            lReadOnlyTitle.Text = "Workflows".FormatAsHtmlTitle();
            workflowFilters.Show();

        }

        protected override void OnLoad(EventArgs e)
        {

            ScriptManager.RegisterStartupScript(Page, this.GetType(), "AKey", "initCharts();", true);



            if (!Page.IsPostBack)
            {
                doStuff();
            }

        }

        public class adjWorkflowType : WorkflowType
        {
            public override ISecured ParentAuthority {
                get
                {
                    return Category != null ? Category : base.ParentAuthority;
                }
 	        }
        }

        protected bool checkPerms(WorkflowType type)
        {
            adjWorkflowType awt = new adjWorkflowType();
            awt.CopyPropertiesFrom(type);
            awt.Category = catServ.Get(awt.CategoryId ?? -1);
            return awt.IsAuthorized("View", CurrentPerson);
        }

        protected void doStuff()
        {

            int workflowTypeFilter = int.TryParse(wftListBox.SelectedValue, out workflowTypeFilter) ? workflowTypeFilter : -1;
            int campusFilter = int.TryParse(campusPicker.SelectedValue, out campusFilter) ? campusFilter : -1;
            int workerFilter = int.TryParse(assignWork.SelectedValue, out workerFilter) ? workerFilter : -1;
            SortProperty workflowSort = workflowReportTable.SortProperty;
            string statusFilter = workStatus.SelectedValue;


            
            workflowChartData1 = campusFilter.ToString();
            //SortProperty workflowSort = workflowReportTable.SortProperty;

            // Campus
            //bindNameAndId(campusPicker, campServ.Queryable().OrderBy(c => c.Name).ToList(), campusFilter.ToString());
            var oldVal = campusPicker.SelectedCampusId;
            campusPicker.Campuses = Rock.Web.Cache.CampusCache.All();
            campusPicker.SelectedCampusId = oldVal;
            campusFilter = campusPicker.SelectedCampusId ?? -1; //int.TryParse(campusPicker.SelectedValue, out campusFilter) ? campusFilter : -1;


            // Workflow Type
            var allWorkflowTypes = workTypeServ.Queryable();

            var viewableWorkflowTypes = allWorkflowTypes.ToList().Where(awt => checkPerms(awt));

            bindNameAndId(wftListBox, viewableWorkflowTypes.OrderBy(x => x.Name).ToList(), workflowTypeFilter.ToString());
            workflowTypeFilter = int.TryParse(wftListBox.SelectedValue, out workflowTypeFilter) ? workflowTypeFilter : -1;


            // Workflow
            var allWorkflowData = from ws in workServ.Queryable()
                                  join was in workActServ.Queryable() on ws.Id equals was.WorkflowId into wj
                                  select new WorkflowData
                                  {
                                      Workflow = ws,
                                      Activity = wj.Where(o => o.AssignedGroupId != null || o.AssignedPersonAliasId != null).OrderByDescending(o => o.ActivatedDateTime).FirstOrDefault()
                                  };

            var assignedWorkflowData = allWorkflowData.Where(awd => awd.Activity.AssignedPersonAliasId != null || awd.Activity.AssignedGroupId != null);


            IQueryable<WorkflowData> viewableWorkflowData;
            var ids = viewableWorkflowTypes.Select(vwt => vwt.Id).ToList();

            if (ids.Contains(workflowTypeFilter))
            {
                viewableWorkflowData = (from awd in assignedWorkflowData
                                        where awd.Workflow.WorkflowTypeId == workflowTypeFilter
                                        select awd);
            }
            else
            {
                viewableWorkflowData = (from awd in assignedWorkflowData
                                        where ids.Contains(awd.Workflow.WorkflowTypeId)
                                        select awd);
            }

            var viewableWorkflowDataList = viewableWorkflowData.ToList().AsQueryable();

            // Assigned Worker
            IQueryable<ShallowPersonData> assignedWorkerData;

            if (campusFilter == -1)
            {
                assignedWorkerData = from vwd in viewableWorkflowDataList
                                     where vwd.Activity.AssignedPersonAliasId != null
                                     group vwd by vwd.Activity.AssignedPersonAliasId into vwdg
                                     select new ShallowPersonData
                                     {
                                         Id = vwdg.Key,
                                         FirstName = vwdg.Select(x => x.Activity.AssignedPersonAlias.Person.NickName).FirstOrDefault(),
                                         LastName = vwdg.Select(x => x.Activity.AssignedPersonAlias.Person.LastName).FirstOrDefault()
                                     };
            }
            else
            {
                assignedWorkerData = from vwd in viewableWorkflowDataList
                                     where vwd.Activity.AssignedPersonAliasId != null && vwd.Activity.AssignedPersonAlias.Person.GetFamilies(_rockContext).FirstOrDefault().CampusId == campusFilter
                                     group vwd by vwd.Activity.AssignedPersonAliasId into vwdg
                                     select new ShallowPersonData
                                     {
                                         Id = vwdg.Key,
                                         FirstName = vwdg.Select(x => x.Activity.AssignedPersonAlias.Person.NickName).FirstOrDefault(),
                                         LastName = vwdg.Select(x => x.Activity.AssignedPersonAlias.Person.LastName).FirstOrDefault()
                                     };
            }





            var assignedGroupPeopleData = (from vwd in viewableWorkflowDataList
                                           where vwd.Activity.AssignedGroupId != null && (campusFilter == -1 || vwd.Activity.AssignedGroup.CampusId == campusFilter)
                                           select vwd.Activity.AssignedGroup.Members.Select(x => new ShallowPersonData
                                           {
                                               Id = x.Person.Aliases.FirstOrDefault().Id,
                                               FirstName = x.Person.FirstName,
                                               LastName = x.Person.LastName
                                           })).SelectMany(x => x);

            var allAssignedPeople = assignedGroupPeopleData.Count() > 0 ? assignedWorkerData.Union(assignedGroupPeopleData, new ShallowPersonDataComparer()) : assignedWorkerData;


            bindNameAndId(assignWork, allAssignedPeople.OrderBy("LastName").ToList(), workerFilter.ToString());
            workerFilter = int.TryParse(assignWork.SelectedValue, out workerFilter) ? workerFilter : -1;

            
            // Status
            bindObject(workStatus, viewableWorkflowData.ToList().GroupBy(wd => wd.Status).Select(x => x.FirstOrDefault().Status).OrderBy(x => x).ToList(), statusFilter);
            statusFilter = workStatus.SelectedValue;


            // Filtered Workflows
            IQueryable<WorkflowData> filteredWorkflowData;

            if (workerFilter == -1)
            {
                if (campusFilter != -1)
                {
                    filteredWorkflowData = from vwd in viewableWorkflowDataList
                                           where (vwd.Activity.AssignedPersonAlias != null && vwd.Activity.AssignedPersonAlias.Person.GetFamilies(_rockContext).FirstOrDefault().CampusId == campusFilter) ||
                                                 (vwd.Activity.AssignedGroup != null && vwd.Activity.AssignedGroup.CampusId == campusFilter)
                                           select vwd;
                }
                else
                {
                    filteredWorkflowData = viewableWorkflowData;
                }
            }
            else
            {
                filteredWorkflowData = from vwd in viewableWorkflowData
                                       where vwd.Activity.AssignedPersonAliasId == workerFilter ||
                                            (vwd.Activity.AssignedGroup != null && vwd.Activity.AssignedGroup.Members.Select(x => x.Person.Aliases.Select(y => y.Id).Contains(workerFilter)).Contains(true))
                                       select vwd;
            }

            if (statusFilter != "-1")
            {
                filteredWorkflowData = filteredWorkflowData.ToList().AsQueryable().Where(x => x.Status == statusFilter);
            }

            filteredWorkflowData = filteredWorkflowData.ToList().AsQueryable().Where(ws => ws.CreatedDateTime > dateRange.LowerValue && ws.CreatedDateTime < (dateRange.UpperValue ?? DateTime.Now).Date.AddHours(23).AddMinutes(59).AddSeconds(59));
            
            List<WorkflowData> wrtData;
            if (workflowSort != null)
            {
                wrtData = filteredWorkflowData.Sort(workflowSort).ToList();
            }
            else
            {
                wrtData = filteredWorkflowData.OrderBy("Completed").ThenBy("WorkflowTypeName").ThenBy("AssignedEntityName").ToList();
            }
            workflowReportTable.DataSource = wrtData;
            workflowReportTable.DataKeyNames = new string[] { "Id" };
            workflowReportTable.DataBind();

            doStats(wrtData);

        }
        protected void bindObject<T>(ListControl control, List<T> entityList, string selectedValue)
        {
            control.DataSource = entityList;
            //control.DataTextField = "Name";
            //control.DataValueField = "Id";
            control.DataBind();
            control.Items.Insert(0, new ListItem("", "-1"));

            if (control.Items.FindByValue(selectedValue) != null)
            {
                control.SelectedValue = selectedValue;
            }
            else
            {
                control.SelectedValue = "-1";
            }

        }

        protected void bindNameAndId<T>(ListControl control, List<T> entityList, string selectedValue)
        {
            control.DataSource = entityList;
            control.DataTextField = "Name";
            control.DataValueField = "Id";
            control.DataBind();
            control.Items.Insert(0, new ListItem("", "-1"));

            if (control.Items.FindByValue(selectedValue) != null)
            {
                control.SelectedValue = selectedValue;
            }
            else
            {
                control.SelectedValue = "-1";
            }

        }

        protected void workflowReportTable_RowSelected(object sender, RowEventArgs e)
        {
            Response.Redirect("~/Workflow/" + ((GridView)sender).DataKeys[e.RowIndex]["Id"].ToString(), false);
        }


        protected int countWorkflows(IQueryable<WorkflowData> workflowData, int completed, int dayDiffStart, int dayDiffEnd)
        {
            return workflowData.Where(
                    x => x.Completed == completed &&
                    (dayDiffStart == 0 || x.CreatedDateTime > DateTime.Now.AddDays(dayDiffStart)) &&
                    (dayDiffEnd == 0 || x.CreatedDateTime < DateTime.Now.AddDays(dayDiffEnd))
                ).Count();
        }
        protected void workflowReportTable_GridRebind(object sender, EventArgs e)
        {
            doStuff();
        }


        protected void doStats(List<WorkflowData> workflowData)
        {
            //IQueryable<int> tmp = workflowData.AsQueryable().Where(x => x.Completed == 1).Select(x => ((x.CompletedDateTime - x.CreatedDateTime) ?? TimeSpan.Zero).Days);
            //int _0to90_AverageTime = tmp.Count() == 0 ? -1 : (int)tmp.Average();
            //rlWorkflowStats.Text = "Average time to completion is " + _0to90_AverageTime + " days.";

            System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

            List<object[]> campusCount = workflowData.GroupBy(wd => wd.CampusId).Select(wd => new object[] { campServ.Get(wd.FirstOrDefault().CampusId).Name, wd.Count() }).ToList();
            campusCount.Insert(0, new string[] { "Campus", "Count" });
            workflowChartData2 = jsSerializer.Serialize(campusCount).EncodeHtml();

            List<object[]> wfTypeCount = workflowData.GroupBy(wd => wd.WorkflowTypeId).Select(wd => new object[] { workTypeServ.Get(wd.FirstOrDefault().WorkflowTypeId).Name, wd.Count() }).ToList();
            wfTypeCount.Insert(0, new string[] { "Workflow Type", "Count" });
            workflowChartData3 = jsSerializer.Serialize(wfTypeCount).EncodeHtml();

            List<object[]> statusCount = workflowData.GroupBy(wd => wd.Status).Select(wd => new object[] { wd.FirstOrDefault().Status, wd.Count() }).ToList();
            statusCount.Insert(0, new string[] { "Status", "Count" });
            workflowChartData4 = jsSerializer.Serialize(statusCount).EncodeHtml();

            List<object[]> workerCount = workflowData.GroupBy(wd => wd.AssignedEntityName).Select(wd => new object[] { wd.FirstOrDefault().AssignedEntityName, wd.Count() }).ToList();
            workerCount.Insert(0, new string[] { "Worker", "Count" });
            workflowChartData5 = jsSerializer.Serialize(workerCount).EncodeHtml();

            List<String> statuses = workflowData.GroupBy(wd => wd.Status).Select(x => x.FirstOrDefault().Status).ToList();

            List<object[]> ageList = workflowData.Select(x => makeWFHistRow(statuses, x)).ToList();
            ageList.Insert(0, statuses.ToArray());
            workflowChartData1 = jsSerializer.Serialize(ageList).EncodeHtml();
        }

        protected object[] makeWFHistRow(List<string> statuses, WorkflowData wd)
        {
            return statuses.Select(x => x == wd.Status ? (object)wd.AgeInt : null).ToArray();
        }

        protected void workflowFilters_ApplyFilterClick(object sender, EventArgs e)
        {
            doStuff();
            workflowFilters.Show();
        }


        public class WorkflowData
        {
            public int Id { get { return Workflow.Id; } }
            public int WorkflowTypeId { get { return Workflow.WorkflowTypeId; } }
            public String WorkflowTypeName { get { return Workflow.WorkflowType.Name; } }
            public String Name { get { return Workflow.Name; } }
            public String Status { get { return Workflow.CompletedDateTime != null ? "Completed" : Workflow.Status; } }
            public DateTime? CreatedDateTime { get { return Workflow.CreatedDateTime; } }
            public DateTime? CompletedDateTime { get { return Workflow.CompletedDateTime; } }
            public int Completed { get { return (Workflow.Status == "Completed" || Workflow.CompletedDateTime != null).Bit(); } }
            public int AgeInt { get { return ((Workflow.CompletedDateTime ?? DateTime.Now) - (Workflow.CreatedDateTime ?? DateTime.Now)).Days; } }
            public int CampusId { get { return (Activity.AssignedPersonAlias != null ? Activity.AssignedPersonAlias.Person.GetFamilies().FirstOrDefault().CampusId : (Activity.AssignedGroup != null ? Activity.AssignedGroup.CampusId : -1)) ?? -1; } }


            public Workflow Workflow { get; set; }
            public WorkflowActivity Activity { get; set; }
            public String AssignedEntityName
            {
                get
                {
                    return Activity.AssignedPersonAlias != null ? Activity.AssignedPersonAlias.Person.NickName + " " + Activity.AssignedPersonAlias.Person.LastName : (
                        Activity.AssignedGroup != null ? Activity.AssignedGroup.Name : "Nobody"
                    );
                }
            }
        }

        public class ShallowPersonData
        {
            public int? Id { get; set; }
            public String Name { get { return FirstName + " " + LastName + (Note != null ? " [" + Note + "]" : ""); } }
            public String FirstName { get; set; }
            public String LastName { get; set; }
            public String Note { get; set; }
        }

        public class ShallowPersonDataComparer : IEqualityComparer<ShallowPersonData>
        {
            public bool Equals(ShallowPersonData a, ShallowPersonData b)
            {
                return a.Id == b.Id;
            }

            public int GetHashCode(ShallowPersonData a)
            {
                return a.Id ?? -1;
            }
        }
    }
}