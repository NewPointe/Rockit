using System;
using Rock.Model;
using Rock.Web.UI;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Rock;
using System.Data.SqlClient;
using Rock.Web.Cache;

[DisplayName("ServiceU Events")]
[Category("NewPointe.org Web Blocks")]
[Description("Upcoming events for each campus")]
public partial class Plugins_org_newpointe_ServiceUEvents_ServiceUEvents : RockBlock
{
   
    protected void Page_Load(object sender, EventArgs e)
    {
        this.RockPage.AddScriptLink("~/Scripts/jquery.autocomplete.min.js");
        
        //this.RockPage.AddScriptLink("~/Scripts/compenents/underscore.min.js");
        //this.RockPage.AddScriptLink("~/Scripts/compenents/jstz.min.js");
        //this.RockPage.AddScriptLink("~/Scripts/compenents/calendar.js");
        //this.RockPage.AddScriptLink("~/Plugins/org_newpointe/ServiceUevents/app.js");
        this.RockPage.AddCSSLink("~/Styles/autocomplete-styles.css");
        this.RockPage.AddCSSLink("~/Plugins/org_newpointe/ServiceUevents/ServiceUEvents.css");
        var eventtitleroute = PageParameter("eventcalendarroute");
        GetCampus();

        if (!Page.IsPostBack)
        {
            //i'm filtering out those axd calls becuase they are shwoing up for some reson as a valid valud of eventcalendarroute. 
            if (!string.IsNullOrEmpty(eventtitleroute) && eventtitleroute != "WebResource.axd" && eventtitleroute != "ScriptResource.axd")
            {
                var rc = new Rock.Data.RockContext();
                // int eventid = 0;
                string eventId = string.Empty;
                using (rc)
                {
                    eventId = rc.Database.SqlQuery<string>("exec newpointe_getEventIDbyUrl @url", new SqlParameter("url", eventtitleroute)).ToList<string>()[0];
                }
                if (string.IsNullOrEmpty(eventId))
                {
                    SiteCache site = SiteCache.GetSiteByDomain(Request.Url.Host);

                    site.RedirectToPageNotFoundPage();

                }
                else
                {
                    hdnEventId.Value = eventId;
                }
            }
            else if (!string.IsNullOrEmpty(eventtitleroute))
            {
                Response.Redirect(eventtitleroute);
            }
        }

    }

    protected void GetCampus()
    {
        var param = Request.QueryString["campusID"];

        var campusParam = PageParameter("campusName");
        var campusId = 51773;

        if (campusParam == "Akron")
        {
            campusId = 67713;
        }
        else if (campusParam == "Canton")
        {
            campusId = 53103;
        }
        else if (campusParam == "Coshocton")
        {
            campusId = 62004;
        }
        else if (campusParam == "Dover")
        {
            campusId = 51773;
        }
        else if (campusParam == "Millersburg")
        {
            campusId = 51774;
        }
        else if (campusParam == "Wooster")
        {
            campusId = 67714;
        }


        if (!String.IsNullOrEmpty(param))
        {
            hdnCampus.Value = param;
        }

        if (!String.IsNullOrEmpty(campusParam))
        {
            hdnCampus.Value = campusId.ToString();
        }
    }
}