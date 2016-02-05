using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;
using Rock.Web.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace org.newpointe.Giving.Workflow
{
    [Description( "Gets the UrlEncodedKey for a Person." )]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Get UrlEncodedKey")]
    [WorkflowAttribute("Person", "Workflow attribute that contains the person to get the key for.", true, "", "", 0, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Attribute", "The workflow attribute to store the key in.", true, "", "", 1, null, new string[] { "Rock.Field.Types.TextFieldType" })]
    class GetUrlEncodedKey : ActionComponent
    {

        public override bool Execute(RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            string attributeValue = GetAttributeValue(action, "Attribute");
            Guid guid = attributeValue.AsGuid();
            if (!guid.IsEmpty())
            {
                var attribute = AttributeCache.Read(guid, rockContext);
                if (attribute != null)
                {
                    string value = GetAttributeValue(action, "Person");
                    action.AddLogEntry(string.Format("value: '{0}'", value));
                    guid = value.AsGuid();
                    if (!guid.IsEmpty())
                    {
                        var person = GetPersonAliasFromActionAttribute("Person", rockContext, action, errorMessages);
                        if (person != null)
                        {
                            action.Activity.Workflow.SetAttributeValue(attribute.Key, person.UrlEncodedKey);
                            action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, value));
                            return true;
                        }
                        else
                        {
                            errorMessages.Add(string.Format("Person could not be found for selected value ('{0}')!", guid.ToString()));
                        }
                    }
                    else
                    {
                        action.Activity.Workflow.SetAttributeValue(attribute.Key, string.Empty);
                        action.AddLogEntry(string.Format("Set '{0}' attribute to nobody.", attribute.Name));
                        return true;
                    }
                }
                else
                {
                    errorMessages.Add(string.Format("Attribute could not be found for selected attribute value ('{0}')!", guid.ToString()));
                }
            }
            else
            {
                errorMessages.Add(string.Format("Selected attribute value ('{0}') was not a valid Guid!", attributeValue));
            }

            errorMessages.ForEach(m => action.AddLogEntry(m, true));

            return false;
        }




        private Person GetPersonAliasFromActionAttribute(string key, RockContext rockContext, WorkflowAction action, List<string> errorMessages)
        {
            string value = GetAttributeValue(action, key);
            Guid guidPersonAttribute = value.AsGuid();
            if (!guidPersonAttribute.IsEmpty())
            {
                var attributePerson = AttributeCache.Read(guidPersonAttribute, rockContext);
                if (attributePerson != null)
                {
                    string attributePersonValue = action.GetWorklowAttributeValue(guidPersonAttribute);
                    if (!string.IsNullOrWhiteSpace(attributePersonValue))
                    {
                        if (attributePerson.FieldType.Class == "Rock.Field.Types.PersonFieldType")
                        {
                            Guid personAliasGuid = attributePersonValue.AsGuid();
                            if (!personAliasGuid.IsEmpty())
                            {
                                PersonAliasService personAliasService = new PersonAliasService(rockContext);
                                return personAliasService.Queryable().AsNoTracking()
                                    .Where(a => a.Guid.Equals(personAliasGuid))
                                    .Select(a => a.Person)
                                    .FirstOrDefault();
                            }
                            else
                            {
                                errorMessages.Add(string.Format("Person could not be found for selected value ('{0}')!", guidPersonAttribute.ToString()));
                                return null;
                            }
                        }
                        else
                        {
                            errorMessages.Add(string.Format("The attribute used for {0} to provide the person was not of type 'Person'.", key));
                            return null;
                        }
                    }
                }
            }

            return null;
        }




    }
}
