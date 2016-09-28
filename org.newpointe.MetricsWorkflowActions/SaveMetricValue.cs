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

namespace org.newpointe.MetricsWorkflowActions
{
    [ActionCategory("Metrics")]
    [Description( "Save a Metric Value to the MetricValues table." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Save MetricValue" )]

    [WorkflowTextOrAttribute("Metric Guid","Metric Attribute", "<span class='tip tip-lava'></span> The metric entity attribute or Guid to use.", true,"","",1,"MetricId")]
    [WorkflowTextOrAttribute("Metric Value", "Metric Value Attribute", "<span class='tip tip-lava'></span> The value to save.", true,"","",2, "MetricValue")]
    [WorkflowAttribute("Metric DateTime Attribute", "DateTime to save with the Metric Value (attribute can be a DateTime or Date Entity).", true,"","",3,"MetricDate")]
    [WorkflowTextOrAttribute("Entity Id", "Entity Id Attribute", "<span class='tip tip-lava'></span> Entity (eg. campus) Id to save with the metric value.", false, "", "", 4, "EntityId")]
    //[CampusField("Campus","Campus for the Metric Partition",false,"","",4)]  //TODO: V6 multi-partition?
    [WorkflowTextOrAttribute("Notes","Notes Attribute","Note to save with the metric",false,"","",5, "Notes")]


    class SaveMetricValue : ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            //Get the Attribute Values
            var metricId = GetAttributeValue(action, "MetricId");
            var metricValue = GetAttributeValue(action, "MetricValue");
            var metricDate = GetAttributeValue(action, "MetricDate");
            var metricEntityId = GetAttributeValue(action, "EntityId");
            var metricNotes = GetAttributeValue(action, "Notes");

            //Get Metric Id
            string metricIdContent = metricId;
            Guid metricIdGuid = metricId.AsGuid();
            if (metricIdGuid.IsEmpty())
            {
                metricIdContent = metricIdContent.ResolveMergeFields(GetMergeFields(action));
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue(metricIdGuid);

                if (attributeValue != null)
                {
                    metricIdContent = attributeValue;
                }
            }


            //Get Metric Value
            string metricValueContent = metricValue;
            Guid metricValueGuid = metricValue.AsGuid();
            if (metricValueGuid.IsEmpty())
            {
                metricValueContent = metricValueContent.ResolveMergeFields(GetMergeFields(action));
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue(metricValueGuid);

                if (attributeValue != null)
                {
                    metricValueContent = attributeValue;
                }
            }

            //Get Metric Date
            string metricDateContent = metricDate;
            Guid metricDateGuid = metricDate.AsGuid();
            if (metricDateGuid.IsEmpty())
            {
                metricDateContent = metricDateContent.ResolveMergeFields(GetMergeFields(action));
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue(metricDateGuid);

                if (attributeValue != null)
                {
                    metricDateContent = attributeValue;
                }
            }

            //Get Metric Entity
            string metricNotesContent = metricNotes;
            Guid metricNotesGuid = metricNotes.AsGuid();
            if (metricDateGuid.IsEmpty())
            {
                metricNotesContent = metricNotesContent.ResolveMergeFields(GetMergeFields(action));
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue(metricNotesGuid);

                if (attributeValue != null)
                {
                    metricNotesContent = attributeValue;
                }
            }

            //Get Metric Notes
            string metricEntityIdContent = metricEntityId;
            Guid metricEntityIdGuid = metricEntityId.AsGuid();
            if (metricDateGuid.IsEmpty())
            {
                metricEntityIdContent = metricEntityIdContent.ResolveMergeFields(GetMergeFields(action));
            }
            else
            {
                var attributeValue = action.GetWorklowAttributeValue(metricEntityIdGuid);

                if (attributeValue != null)
                {
                    metricEntityIdContent = attributeValue;
                }
            }


            //Convert the Attribute values to approprate types
            DateTime metricDateAsDate = Convert.ToDateTime(metricDateContent);
            Decimal metricValueAsDecimal = Decimal.Parse(metricValueContent);
            int metricEntityIdAsInt = Int32.Parse(metricEntityIdContent);

            //Convert Metric Attribute to Id
            var metricIdAsGuidAsValues = metricIdContent.Split('|');
            var metricIdAsGuid = Guid.Parse(metricIdAsGuidAsValues[0]);  
            MetricService metricService = new MetricService(rockContext);
            var selectedMetric = metricService.Queryable().FirstOrDefault(m => m.Guid == metricIdAsGuid);
            if (selectedMetric != null)
            {
                int metricIdAsInt = selectedMetric.Id;

                //Save the Metric
                SaveMetric(metricDateAsDate, metricIdAsInt, metricValueAsDecimal, metricEntityIdAsInt, metricNotesContent);
            }

            return true;
        }

        public void SaveMetric(DateTime dt, int metric, Decimal value, int campus, string notes)
        {
            int metricValueId = 0;
            RockContext rockContext = new RockContext();
            MetricValue metricValue;
            MetricValueService metricValueService = new MetricValueService(rockContext);

            //Does this metric already exist?
            var existingMetric = metricValueService
                .Queryable(
                    ).FirstOrDefault(a => a.MetricId == metric && a.MetricValueDateTime == dt && a.EntityId == campus);

            if (existingMetric != null)
            {
                metricValueId = existingMetric.Id;
            }


            if (metricValueId == 0)
            {
                metricValue = new MetricValue();
                metricValueService.Add(metricValue);
                metricValue.MetricId = metric;
                metricValue.Metric = metricValue.Metric ?? new MetricService(rockContext).Get(metricValue.MetricId);
            }
            else
            {
                metricValue = metricValueService.Get(metricValueId);
            }

            metricValue.MetricValueType = (MetricValueType)metric;
            metricValue.XValue = null;
            metricValue.YValue = value;
            metricValue.Note = notes;
            metricValue.MetricValueDateTime = dt;
            metricValue.EntityId = campus;

            rockContext.SaveChanges();

        }
    }
}
