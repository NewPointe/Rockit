using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using org.newpointe.Stars.Data;
using org.newpointe.Stars.Model;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Workflow;
using Rock;

namespace org.newpointe.Stars
{
    [ActionCategory("Stars")]
    [Description( "Save a Stars to a person's record" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Save Stars" )]

    [WorkflowAttribute("Person", "Workflow attribute that contains the person to update.", true, "", "", 0, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [DefinedValueField(SystemGuid.DefinedType.STARS_TYPE, "Location Type", "The type of location to update (if attribute is not specified or is an invalid value).", true, false, SystemGuid.DefinedType.STARS_TYPE_VERSE, "", 2)]


    [WorkflowTextOrAttribute("Metric Guid","Metric Attribute", "<span class='tip tip-lava'></span> The metric entity attribute or Guid to use.", true,"","",1,"MetricId")]
    [WorkflowTextOrAttribute("MetricValue Value", "Metric Value Attribute", "<span class='tip tip-lava'></span> The value to save.", true,"","",2, "MetricValue")]
    [WorkflowAttribute("MetricValue DateTime Attribute", "DateTime to save with the Metric Value (attribute can be a DateTime or Date Entity).", true,"","",3,"MetricDate")]
    [WorkflowTextOrAttribute("MetricValue Entity Id", "MetricValue Entity Id Attribute", "<span class='tip tip-lava'></span> Entity (eg. campus) Id to save with the metric value.", false, "", "", 4, "EntityId")]
    //[CampusField("Campus","Campus for the Metric Partition",false,"","",4)]  //TODO: V6 multi-partition?
    [WorkflowTextOrAttribute("MetricValue Notes", "Notes Attribute","Note to save with the metric",false,"","",5, "Notes")]
    [WorkflowTextOrAttribute("MetricValue Type", "MetricValue Type Attribute", "Int of the Type of MetricValue to Save",true,"0","",6,"MetricType")]




    class SaveStarsWorkflowAction : ActionComponent
    {
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            // Get person
            Person person = null;
            int personAliasId = 1;

            string personAttributeValue = GetAttributeValue(action, "Person");
            Guid? guidPersonAttribute = personAttributeValue.AsGuidOrNull();
            if (guidPersonAttribute.HasValue)
            {
                var attributePerson = AttributeCache.Read(guidPersonAttribute.Value, rockContext);
                if (attributePerson != null || attributePerson.FieldType.Class != "Rock.Field.Types.PersonFieldType")
                {
                    string attributePersonValue = action.GetWorklowAttributeValue(guidPersonAttribute.Value);
                    if (!string.IsNullOrWhiteSpace(attributePersonValue))
                    {
                        Guid personAliasGuid = attributePersonValue.AsGuid();
                        if (!personAliasGuid.IsEmpty())
                        {
                            person = new PersonAliasService(rockContext).Queryable()
                                .Where(a => a.Guid.Equals(personAliasGuid))
                                .Select(a => a.Person)
                                .FirstOrDefault();
                            if (person == null)
                            {
                                errorMessages.Add(string.Format("Person could not be found for selected value ('{0}')!", guidPersonAttribute.ToString()));
                                return false;
                            }
                        }
                    }
                }
            }

            
            if (person == null)
            {
                errorMessages.Add("The attribute used to provide the person was invalid, or not of type 'Person'.");
                return false;
            }

            if (person != null)
            {
                PersonAliasService personAliasService = new PersonAliasService(rockContext);
                personAliasId = person.PrimaryAliasId ?? default(int);
            }

            //Get DateTime
            DateTime currentDateTime =  DateTime.Now;


            //Get Stars Value
            AttributeValueService attributeValueService = new AttributeValueService(rockContext);


            // TODO:
            // 1. First, get the Defined Value that was picked in the WF.
            // 2. Get the defined value that the person selected.
            // 3. Then get the StarsValue attribute attached to the defined value
            // 4. Finally, set the AttributeValue of that attribute. That's the value to save.



            //Save Stars
            SaveStars(currentDateTime, personAliasId, 1m);


                return true;
        }




        public void SaveStars(DateTime dt, int paId, decimal starsValue)
        {
            StarsProjectContext starsProjectContext = new StarsProjectContext();
            StarsService starsService = new StarsService(starsProjectContext);
            org.newpointe.Stars.Model.Stars stars = new org.newpointe.Stars.Model.Stars();

            stars.PersonAliasId = paId;
            stars.CampusId = 1;
            stars.TransactionDateTime = DateTime.Now;
            stars.Value = value;

            starsService.Add(stars);

            starsProjectContext.SaveChanges();

        }

        public void SaveMetric(DateTime dt, int metric, Decimal value, int campus, string notes, int type)
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

            metricValue.MetricValueType = (MetricValueType)type;
            metricValue.XValue = null;
            metricValue.YValue = value;
            metricValue.Note = notes;
            metricValue.MetricValueDateTime = dt;
            metricValue.EntityId = campus;

            rockContext.SaveChanges();

        }
    }
}
