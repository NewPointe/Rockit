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
using System.Net;
using Newtonsoft.Json;


[DisplayName("Campus Menu")]
[Category("NewPointe.org Web Blocks")]
[Description("Main menu")]

public partial class Plugins_org_newpointe_CampusMenu_CampusMenu : RockBlock
{
    public string LiveServiceText;
    public string LiveAttribute = Rock.Web.Cache.GlobalAttributesCache.Value("LiveService");
    public string LiveTextLive = Rock.Web.Cache.GlobalAttributesCache.Value("LiveServiceTextLive");
    public string LiveTextNotLive = Rock.Web.Cache.GlobalAttributesCache.Value("LiveServiceTextNotLive");

    protected void Page_Load(object sender, EventArgs e)
    {
        string sessionLive = (string)Session["livePopup"] ?? "false";

        if (LiveAttribute.ToLower() == "true")
        {
            LiveServiceText = LiveTextLive;
            if (sessionLive != "true")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
                Session["livePopup"] = "true";
            }
            
        }
        else
        {
            LiveServiceText = LiveTextNotLive;
        }


    }
}