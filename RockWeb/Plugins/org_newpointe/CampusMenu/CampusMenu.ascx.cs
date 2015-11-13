using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Model;
using Rock.Web.UI;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Data;
using Rock.Security;
using System.ComponentModel;
using System.Data;


[DisplayName("Campus Menu")]
[Category("NewPointe.org Web Blocks")]
[Description("Main menu")]

public partial class Plugins_org_newpointe_CampusMenu_CampusMenu : RockBlock
{
    public string LiveServiceText = "WATCH LIVE EVERY WEEK! SUNDAYS AT 9 & 11 A.M.";

    protected void Page_Load(object sender, EventArgs e)
    {
        //current time
        DateTime currentTime = DateTime.Now;
        string dayOfWeek = currentTime.DayOfWeek.ToString();

        if (dayOfWeek == "Sunday")
        {
            //service timea
            TimeSpan start9 = new TimeSpan(8, 55, 0);
            TimeSpan end9 = new TimeSpan(10, 05, 0);
            TimeSpan start11 = new TimeSpan(10, 55, 0);
            TimeSpan end11 = new TimeSpan(12, 05, 0);

            bool service9 = TimeBetween(currentTime, start9, end9);
            bool service11 = TimeBetween(currentTime, start11, end11);

            if (service9 || service11 == true)
            {
                LiveServiceText = "<a href='http://live.newpointe.org'>WATCH LIVE NOW! <i class='fa fa-laptop'></i></a>";
            }

        }
    }


    bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
    {
        // convert datetime to a TimeSpan
        TimeSpan now = datetime.TimeOfDay;
        // see if start comes before end
        if (start < end)
            return start <= now && now <= end;
        // start is after end, so do the inverse comparison
        return !(end < now && now < start);
    }
}