using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.newpointe.WorkflowEntities
{
    class ObjectConverter
    {
        public static object ConvertObject(String theObject, Type objectType, bool tryToNull = true)
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
                        return Convert.ChangeType(theObject, underType);
                    }
                }
            }
        }
    }
}
