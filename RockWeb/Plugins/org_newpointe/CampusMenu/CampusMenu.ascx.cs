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
    public string LiveServiceText = "WATCH LIVE EVERY WEEK! SUNDAYS AT 9 & 11 A.M.";
    public string LivePlatformUrlJson;

    protected void Page_Load(object sender, EventArgs e)
    {

        //Check ChurchOnline Platform API to see if there is a live event
        string livePlatformUrl = "http://live.newpointe.org/api/v1/events/current";

        using (WebClient wc = new WebClient())
        {
            LivePlatformUrlJson = wc.DownloadString(livePlatformUrl);
        }

        dynamic isServiceLive = JsonConvert.DeserializeObject(LivePlatformUrlJson);

        string isLive = isServiceLive.response.item.isLive.ToString();

        if (isLive.ToLower() == "true")
        {
            LiveServiceText = "<a href='http://live.newpointe.org'>WATCH LIVE NOW! <i class='fa fa-video-camera'></i></a>";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
            //popup.Style["display"] = "block";
        }


    }
}