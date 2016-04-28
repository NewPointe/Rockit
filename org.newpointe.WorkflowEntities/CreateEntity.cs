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
    [Description("Creates an entity")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Create an entity")]

    [EntityTypeField("Entity Type", "The type of entity to create.", true, "", 0)]
    [KeyValueListField("Entity Properties", "The properties to create the entity with. <span class='tip tip-lava'></span>", true, "", "Property", "Value", "", "", "", 1)]
    [WorkflowAttribute("Entity Attribute", "The attribute to save the entity to.", false, "", "", 2)]
    class CreateEntity : ActionComponent
    {
        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            EntityTypeCache cachedEntityType = EntityTypeCache.Read(GetAttributeValue(action, "EntityType").AsGuid());
            var propertyValues = GetAttributeValue(action, "EntityProperties").ResolveMergeFields(GetMergeFields(action)).TrimEnd('|').Split('|').Select(p => p.Split('^')).Select(p => new { Name = p[0], Value = p[1] });

            if(cachedEntityType != null)
            {
                Type entityType = cachedEntityType.GetEntityType();
                IEntity newEntity = (IEntity)Activator.CreateInstance(entityType);

                foreach (var prop in propertyValues)
                {
                    PropertyInfo propInf = entityType.GetProperty(prop.Name, BindingFlags.Public | BindingFlags.Instance);
                    if (null != propInf && propInf.CanWrite)
                    {
                        try
                        {
                            Type propType = Nullable.GetUnderlyingType(propInf.PropertyType) ?? propInf.PropertyType;
                            object convertedValue = prop.Value == null ? null : Convert.ChangeType(prop.Value, propType);
                            propInf.SetValue(newEntity, convertedValue, null);
                        }
                        catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
                        {
                            errorMessages.Add("Invalid Property Value: " + prop.Name + ": " + ex.Message);
                        }
                    }
                    else
                    {
                        errorMessages.Add("Invalid Property: " + prop.Name);
                    }
                }

                rockContext.Set(entityType).Add(newEntity);
                rockContext.SaveChanges();

                // If request attribute was specified, requery the request and set the attribute's value
                Guid? entityAttributeGuid = GetAttributeValue(action, "EntityAttribute").AsGuidOrNull();
                if (entityAttributeGuid.HasValue)
                {
                    newEntity = (IEntity) rockContext.Set(entityType).Find(new object[] { newEntity.Id });
                    if (newEntity != null)
                    {
                        SetWorkflowAttributeValue(action, entityAttributeGuid.Value, newEntity.Guid.ToString());
                    }
                }


                return true;

            }
            else
            {
                errorMessages.Add("Invalid Entity Type");
            }
            return false;
        }
    }
}
