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
    [Description("Sets multiple properties of an entity")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Set multiple properties of an entity")]

    [EntityTypeField("Entity Type", "The type of the entity. If left blank, attempts to infer the Type from the Entity Attribute.", false, "", 0)]
    [WorkflowAttribute("Entity", "The entity to set the property of (Or a text field with it's Guid).", true, "", "", 1)]
    [CustomDropdownListField("Empty Value Handling", "How to handle empty property values", "IGNORE^Ignore empty values,EMPTY^Leave empty values empty,NULL^Set empty values to NULL (where possible)", true, "", "", 2)]
    [KeyValueListField("Entity Properties", "The properties to create the entity with. <span class='tip tip-lava'></span>", true, "", "Property", "Value", "", "", "", 3)]
    class SetEntityProperties : ActionComponent
    {
        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            String propertyName = GetAttributeValue(action, "PropertyName").ResolveMergeFields(GetMergeFields(action));
            String propertyValue = GetAttributeValue(action, "PropertyValue").ResolveMergeFields(GetMergeFields(action));
            Guid workflowAttributeGuid = GetAttributeValue(action, "Entity").AsGuid();
            Guid entityGuid = action.GetWorklowAttributeValue(workflowAttributeGuid).AsGuid();

            if (!entityGuid.IsEmpty())
            {
                IEntity entityObject = null;
                EntityTypeCache cachedEntityType = EntityTypeCache.Read(GetAttributeValue(action, "EntityType").AsGuid());

                if (cachedEntityType != null)
                {
                    Type entityType = cachedEntityType.GetEntityType();
                    entityObject = rockContext.Set<IEntity>().AsQueryable().Where(e => e.Guid == entityGuid).FirstOrDefault();
                }
                else {
                    var field = AttributeCache.Read(workflowAttributeGuid).FieldType.Field;
                    entityObject = ((Rock.Field.IEntityFieldType)field).GetEntity(entityGuid.ToString(), rockContext);
                }
                
                var propertyValues = GetAttributeValue(action, "EntityProperties").Replace(" ! ", " | ").ResolveMergeFields(GetMergeFields(action)).TrimEnd('|').Split('|').Select(p => p.Split('^')).Select(p => new { Name = p[0], Value = p[1] });

                foreach (var prop in propertyValues)
                {
                    PropertyInfo propInf = entityObject.GetType().GetProperty(prop.Name, BindingFlags.Public | BindingFlags.Instance);
                    if (null != propInf && propInf.CanWrite)
                    {
                        if (!(GetAttributeValue(action, "EmptyValueHandling") == "IGNORE" && String.IsNullOrWhiteSpace(prop.Value)))
                        {
                            try
                            {
                                propInf.SetValue(entityObject, ConvertObject(prop.Value, propInf.PropertyType, GetAttributeValue(action, "EmptyValueHandling") == "NULL"), null);
                            }
                            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
                            {
                                errorMessages.Add("Invalid Property Value: " + prop.Name + ": " + ex.Message);
                            }
                        }
                    }
                    else
                    {
                        errorMessages.Add("Invalid Property: " + prop.Name);
                    }
                }

                rockContext.SaveChanges();
                return true;
            }
            else {
                errorMessages.Add("Invalid Entity Attribute");
            }
            return false;
        }

        private object ConvertObject(String theObject, Type objectType, bool tryToNull = true)
        {
            if (objectType.IsEnum)
            {
                return String.IsNullOrWhiteSpace(theObject) ? null : Enum.Parse(objectType, theObject, true);
            }
            else
            {
                Type underType = Nullable.GetUnderlyingType(objectType);
                if (underType == null) // not nullable
                {
                    return Convert.ChangeType(theObject, objectType);
                }
                else //nullable
                {
                    if (tryToNull && String.IsNullOrWhiteSpace(theObject))
                    {
                        return null;
                    }
                    else {
                        return Convert.ChangeType(theObject, objectType);
                    }
                }
            }
        }
    }
}
