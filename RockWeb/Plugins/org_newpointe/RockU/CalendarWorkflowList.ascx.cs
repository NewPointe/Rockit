using System;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.org_newpointe.RockU
{
    [DisplayName( "Calendar Item Occurence Workflow List" )]
    [Category( "RockU" )]
    [Description( "Lists the workflows associated to a particular calendar item occurrence." )]

    [WorkflowTypeField( "Workflows", "The workflows to make available.", true, true, "", "", 0 )]
    [TextField( "EventItemOccurrence Attribute Key", "The key for the Workflow Attribute that stores the Event Item Occurrence.", true, "EventItemOccurrence", "", 1 )]
    [LinkedPage("Workflow Entry Page", "The page that has the workflow entry block.", false, Rock.SystemGuid.Page.WORKFLOW_ENTRY, "", 2)]
    public partial class CalendarWorkflowList : RockBlock
    {

        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            if(!IsPostBack)
            {
                BindGrid();
            }
        }

        protected void BindGrid()
        {
            var rockContext = new RockContext();
            var eventItemOccurrence = new EventItemOccurrenceService( rockContext ).Get( PageParameter( "EventItemOccurrenceId" ).AsInteger() );

            if ( eventItemOccurrence != null )
            {
                IQueryable<WorkflowType> workflowTypes = new WorkflowTypeService( rockContext ).GetByGuids( GetAttributeValue( "Workflows" ).Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries ).Select( g => g.AsGuid() ).ToList() );
                if ( workflowTypes.Count() > 0 )
                {
                    IQueryable<Workflow> workflows = new WorkflowService( rockContext )
                        .Queryable()
                        .Where( w => workflowTypes.Select( wt => wt.Id ).Contains( w.WorkflowTypeId ) )
                        .WhereAttributeValue( rockContext, GetAttributeValue( "EventItemOccurrenceAttributeKey" ), eventItemOccurrence.Guid.ToString() );

                    if ( gWorkflows.SortProperty != null )
                        workflows = workflows.Sort( gWorkflows.SortProperty );
                    else
                        workflows = workflows.OrderBy( w => w.ActivatedDateTime );

                    gWorkflows.DataKeyNames = new string[] { "Id", "WorkflowTypeId" };
                    gWorkflows.SetLinqDataSource( workflows );
                    gWorkflows.DataBind();

                    rddlNewWorkflowType.DataValueField = "Id";
                    rddlNewWorkflowType.DataTextField = "Name";
                    rddlNewWorkflowType.DataSource = workflowTypes.ToList();
                    rddlNewWorkflowType.DataBind();
                }
            }
        }

        protected void gWorkflows_RowSelected( object sender, Rock.Web.UI.Controls.RowEventArgs e )
        {
            if(!string.IsNullOrWhiteSpace(GetAttributeValue( "WorkflowEntryPage" ) ))
            {
                NavigateToLinkedPage( "WorkflowEntryPage", new Dictionary<string, string>
                {
                    { "WorkflowId", e.RowKeyValues["Id"].ToString() },
                    { "WorkflowTypeId", e.RowKeyValues["WorkflowTypeId"].ToString() }
                } );
            }
        }

        protected void lbGo_Click( object sender, EventArgs e )
        {
            using ( var rockContext = new RockContext() )
            {
                var workflowType = new WorkflowTypeService( rockContext ).Get( rddlNewWorkflowType.SelectedValue.AsInteger() );

                if ( workflowType != null )
                {
                    var eventItemOccurrence = new EventItemOccurrenceService( rockContext ).Get( PageParameter( "EventItemOccurrenceId" ).AsInteger() );
                    if ( eventItemOccurrence != null )
                    {
                        LaunchWorkflow( rockContext, eventItemOccurrence, workflowType );
                    }
                }
            }
        }
        

        /// <summary>
        /// Launches the workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="connectionWorkflow">The connection workflow.</param>
        /// <param name="name">The name.</param>
        private void LaunchWorkflow( RockContext rockContext, EventItemOccurrence eventOccurrence, WorkflowType workflowType )
        {
            if ( eventOccurrence != null && workflowType != null )
            {
                var workflow = Workflow.Activate( workflowType, workflowType.WorkTerm, rockContext );
                if ( workflow != null )
                {
                    var workflowService = new WorkflowService( rockContext );

                    List<string> workflowErrors;
                    if ( workflowService.Process( workflow, eventOccurrence, out workflowErrors ) )
                    {
                        if ( workflow.Id != 0 )
                        {

                            if ( workflow.HasActiveEntryForm( CurrentPerson ) )
                            {
                                var qryParam = new Dictionary<string, string>();
                                qryParam.Add( "WorkflowTypeId", workflowType.Id.ToString() );
                                qryParam.Add( "WorkflowId", workflow.Id.ToString() );
                                NavigateToLinkedPage( "WorkflowEntryPage", qryParam );
                            }
                            else
                            {
                                mdWorkflowLaunched.Show( string.Format( "A '{0}' workflow has been started.",
                                    workflowType.Name ), ModalAlertType.Information );
                            }
                            BindGrid();
                        }
                        else
                        {
                            mdWorkflowLaunched.Show( string.Format( "A '{0}' workflow was processed (but not persisted).",
                                workflowType.Name ), ModalAlertType.Information );
                        }
                    }
                    else
                    {
                        mdWorkflowLaunched.Show( "Workflow Processing Error(s):<ul><li>" + workflowErrors.AsDelimited( "</li><li>" ) + "</li></ul>", ModalAlertType.Information );
                    }
                }
            }
        }


    }
}