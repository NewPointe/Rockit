using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Jobs;

using Quartz;
using Rock;

namespace org.newpointe.SqlToWorkflow
{
    /// <summary>
    /// Job to run quick SQL queries on a schedule
    /// </summary>
    [CodeEditorField("SQL Query", "SQL query to run", CodeEditorMode.Sql, CodeEditorTheme.Rock, 200, true, "SELECT TOP 3 FirstName AS Person FROM [Person] ORDER BY Id", "General", 0, "SQLQuery")]
    //[TextField("Workflow Guid", "The GUID of the workflow to launch", true, "dd8531c0-e6a0-4206-9aa4-ea855f21e36a", "General", 1, "WGuid")]
    [WorkflowTypeField("Workflow", "The Workflow to launch", false, true, "", "General", 2, "WGuid")]
    [DisallowConcurrentExecution]
    public class SQLToWorkflows : IJob
    {

        public SQLToWorkflows()
        {
        }

        public virtual void Execute(IJobExecutionContext context)
        {

            JobDataMap dataMap = context.JobDetail.JobDataMap;
            string query = dataMap.GetString("SQLQuery");
            string wGuidStr = dataMap.GetString("WGuid");
            Guid  wGuid = new Guid(wGuidStr);


            DataSet data = DbService.GetDataSet(query, CommandType.Text, new Dictionary<string, object>());

            foreach (DataTable tbl in data.Tables)
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Dictionary<string, string> attr = new Dictionary<string, string>();
                    foreach (DataColumn col in tbl.Columns)
                    {
                        attr.Add(col.ColumnName, row[col].ToString());
                    }
                    launchWorkflow(wGuid, attr);
                }

            }

        }


        protected void launchWorkflow(Guid workflowGuid, Dictionary<string, string> attributes)
        {

            RockContext _rockContext = new RockContext();
            WorkflowService _workflowService = new WorkflowService(_rockContext);
            WorkflowTypeService _workflowTypeService = new WorkflowTypeService(_rockContext);
            WorkflowType _workflowType = _workflowTypeService.Get(workflowGuid);

            Workflow _workflow = Rock.Model.Workflow.Activate(_workflowType, "New Test" + _workflowType.WorkTerm);


            foreach (KeyValuePair<string, string> attribute in attributes)
            {
                _workflow.SetAttributeValue(attribute.Key, attribute.Value);
            }


            List<string> errorMessages;
            if (_workflowService.Process(_workflow, out errorMessages))
            {
                // If the workflow type is persisted, save the workflow
                if (_workflow.IsPersisted || _workflowType.IsPersisted)
                {
                    if (_workflow.Id == 0)
                    {
                        _workflowService.Add(_workflow);
                    }

                    _rockContext.WrapTransaction(() =>
                    {
                        _rockContext.SaveChanges();
                        _workflow.SaveAttributeValues(_rockContext);
                        foreach (var activity in _workflow.Activities)
                        {
                            activity.SaveAttributeValues(_rockContext);
                        }
                    });

                }
            }
        }

    }
}
