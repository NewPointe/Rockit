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


namespace RockWeb.Plugins.org_newpointe.CampusAvailability
{
    [DisplayName("Campus Menu")]
    [Category("Newpointe")]
    [Description("Main menu")]
    [CampusesField("AvailableCampuses")]


    public partial class CampusAvailability : RockBlock
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.hlblExample1.Text = "test";

        var selectedCampuses = GetAttributeValue("AvailableCampuses").ToList();


        //foreach (Campus c in selectedCampuses)
        //{
        //   var test = c.ShortCode.ToString();
        //}

        }
}
}