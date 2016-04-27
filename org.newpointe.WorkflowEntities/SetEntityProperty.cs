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
    [Description("Sets the property of an entity")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Set the property of an entity")]

    //[EntityTypeField("Entity Type", "The type of the entity. If left blank, attempts to infer the Type from the Entity Attribute.", false, "", 0)]
    [WorkflowAttribute("Entity", "The entity to set the property of (Or a text field with it's Guid).", true, "", "", 1)]
    [TextField("Property Name", "The name of the property to set. <span class='tip tip-lava'></span>", true, "", "", 2)]
    [TextField("Property Value", "The value to set the property to. <span class='tip tip-lava'></span>", true, "", "", 3)]
    class SetEntityProperty : ActionComponent
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
                //EntityTypeCache cachedEntityType = EntityTypeCache.Read(GetAttributeValue(action, "EntityType").AsGuid());

                //if (cachedEntityType != null)
                //{
                //    Type entityType = cachedEntityType.GetEntityType();
                //    entityObject = select e from (IEntity)rockContext.Set(entityType) where e.Guid == "";
                //}
                //else {
                    var field = AttributeCache.Read(workflowAttributeGuid).FieldType.Field;
                    entityObject = ((Rock.Field.IEntityFieldType)field).GetEntity(entityGuid.ToString(), rockContext);
                //}



                PropertyInfo prop = entityObject.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    try {
                        Type propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        object convertedValue = propertyValue == null ? null : Convert.ChangeType(propertyValue, propType);
                        prop.SetValue(entityObject, convertedValue, null);
                        rockContext.SaveChanges();
                        return true;
                    }
                    catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
                    {
                        errorMessages.Add("Invalid Property Value: " + ex.Message);
                    }
                }
                else
                {
                    errorMessages.Add("Invalid Property");
                }
            }
            else {
                errorMessages.Add("Invalid Entity Attribute");
            }
            return false;
        }
    }
}
