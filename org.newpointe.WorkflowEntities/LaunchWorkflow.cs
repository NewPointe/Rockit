using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Workflow;
using Rock;

namespace org.newpointe.WorkflowEntities
{
    [Description( "Launches a Workflow" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Launch Workflow" )]

    [WorkflowTypeField( "Workflow Type", "The type of workflow to launch", false, true, "", "", 0 )]
    [ValueListField( "Attributes", "The attributes to copy to the launched workflow.", false, "", "AttributeKey", "", "", "", 1 )]
    class LaunchWorkflow : ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            var workflowTypeGuid = GetAttributeValue( action, "WorkflowType" ).AsGuid();
            var workflowType = new WorkflowTypeService( rockContext ).Get( workflowTypeGuid );

            if ( workflowType != null )
            {
                var coppiedAttributes = new Dictionary<string, string>();
                var attributeKeyList = GetAttributeValue( action, "Attributes" ).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach ( var attributeKey in attributeKeyList )
                {
                    var value = action.Activity.GetAttributeValue( attributeKey );
                    if(string.IsNullOrWhiteSpace(value))
                    {
                        value = action.Activity.Workflow.GetAttributeValue( attributeKey );
                    }
                    coppiedAttributes.Add( attributeKey, value );
                }

                launchWorkflow( workflowType, coppiedAttributes, rockContext );
            }
            return true;
        }

        protected void launchWorkflow( WorkflowType _workflowType, Dictionary<string, string> attributes, RockContext rockContext = null )
        {

            RockContext _rockContext = rockContext == null ? new RockContext() : rockContext;
            WorkflowService _workflowService = new WorkflowService( _rockContext );
            WorkflowTypeService _workflowTypeService = new WorkflowTypeService( _rockContext );

            Workflow _workflow = Rock.Model.Workflow.Activate( _workflowType, "New " + _workflowType.WorkTerm );


            foreach ( KeyValuePair<string, string> attribute in attributes )
            {
                _workflow.SetAttributeValue( attribute.Key, attribute.Value );
            }


            List<string> errorMessages;
            if ( _workflowService.Process( _workflow, out errorMessages ) )
            {
                // If the workflow type is persisted, save the workflow
                if ( _workflow.IsPersisted || _workflowType.IsPersisted )
                {
                    if ( _workflow.Id == 0 )
                    {
                        _workflowService.Add( _workflow );
                    }

                    _rockContext.WrapTransaction( () =>
                    {
                        _rockContext.SaveChanges();
                        _workflow.SaveAttributeValues( _rockContext );
                        foreach ( var activity in _workflow.Activities )
                        {
                            activity.SaveAttributeValues( _rockContext );
                        }
                    } );

                }
            }
        }
    }
}
