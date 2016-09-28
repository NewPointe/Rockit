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
    [Description( "Save MetricValue" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Save Metric Value" )]

    [WorkflowTextOrAttribute("Metric Guid","Metric Attribute","The metric id to use.",true,"","",1,"MetricId")]
    [WorkflowTextOrAttribute("Metric Value", "Metric Value Attribute", "The value to save.", true,"","",2, "MetricValue")]
    [WorkflowAttribute("Metric DateTime Attribute", "Date to save with the Metric Value", true,"","",3,"MetricDate")]
    [CampusField("Campus","Campus for the Metric Partition",false,"","",4)]
    [WorkflowTextOrAttribute("Notes","Notes Attribute","Note to save with the metric",false,"","",5)]


    class SaveMetricValue : ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            //Get the Attribute Values
            var metricId = GetAttributeValue(action, "MetricId");
            var metricValue = GetAttributeValue(action, "MetricValue");
            var metricDate = GetAttributeValue(action, "MetricDate");
            var metricCampus = GetAttributeValue(action, "Campus");
            var notes = GetAttributeValue(action, "Notes");

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


            //Get Metric Id
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

            //Get Metric Notes
            string metricNotesContent = notes;
            Guid metricNotesGuid = notes.AsGuid();
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


            //Convert the Attribute values to approprate types
            DateTime metricDateAsDate = Convert.ToDateTime(metricDateContent);
            //int metricIdAsInt = Convert.ToInt32(metricId);
            Decimal metricValueAsDecimal = Decimal.Parse(metricValueContent);

            //Convert Campus Attribute to Id
            var metricCampusAsGuid = Guid.Parse(metricCampus);
            CampusService campusService = new CampusService(rockContext);
            var selectedCampus = campusService.Queryable().FirstOrDefault(c => c.Guid == metricCampusAsGuid);
            int metricCampusAsInt = selectedCampus.Id;

            //Convert Metric Attribute to Id
            var metricIdAsGuidAsValues = metricIdContent.Split('|');
            var metricIdAsGuid = Guid.Parse(metricIdAsGuidAsValues[0]);  
            MetricService metricService = new MetricService(rockContext);
            var selectedMetric = metricService.Queryable().FirstOrDefault(m => m.Guid == metricIdAsGuid);
            int metricIdAsInt = selectedMetric.Id;

            //Save the Metric
            SaveMetric(metricDateAsDate, metricIdAsInt, metricValueAsDecimal, metricCampusAsInt, metricNotesContent);

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
