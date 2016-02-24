using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Workflow;
using Rock.Workflow.Action.CheckIn;
using Rock.CheckIn;

namespace org.newpointe.Checkin.Workflow
{
    [Description("Balances locations.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Filter Balance Locations")]

    [BooleanField("Remove", "Select 'Yes' if locations should be be removed.  Select 'No' if they should just be marked as excluded.", true)]
    public class FilterBalanceLocation : CheckInActionComponent
    {
        public override bool Execute(RockContext rockContext, Rock.Model.WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            var rand = new Random();
            var checkInState = GetCheckInState(entity, out errorMessages);
            if (checkInState != null)
            {
                var family = checkInState.CheckIn.Families.Where(f => f.Selected).FirstOrDefault();
                if (family != null)
                {
                    var remove = GetAttributeValue(action, "Remove").AsBoolean();

                    foreach (var person in family.People.Where(p => p.Selected))
                    {
                        foreach (var groupType in person.GroupTypes.Where(gt => gt.Selected))
                        {
                            foreach (var group in groupType.Groups.Where(g => g.Selected))
                            {
                                
                                int numValidLocations = group.Locations.Take(2).Count();
                                if (numValidLocations > 0)
                                {
                                    CheckInLocation bestLocation = null;
                                    if (numValidLocations == 1)
                                    {
                                        bestLocation = group.Locations.FirstOrDefault();
                                    }
                                    else
                                    {
                                        bestLocation = group.Locations.Where(l => !l.ExcludedByFilter && l.Schedules.Any(s => s.Schedule.IsCheckInActive)).OrderBy(l => KioskLocationAttendance.Read(l.Location.Id).CurrentCount).FirstOrDefault();
                                    }

                                    if (bestLocation != null)
                                    {
                                        foreach (var location in group.Locations.ToList())
                                        {
                                            if (location != bestLocation)
                                            {
                                                if (remove)
                                                {
                                                    group.Locations.Remove(location);
                                                }
                                                else
                                                {
                                                    location.ExcludedByFilter = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }
    }
}