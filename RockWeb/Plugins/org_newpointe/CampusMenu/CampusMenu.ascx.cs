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
[Category("Newpointe")]
[Description("Main menu")]

public partial class Plugins_org_newpointe_CampusMenu_CampusMenu : RockBlock
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
        {
            BindGrid();
        }
    }

    private void BindGrid()
    {
        CampusService cs = new CampusService(new RockContext());

        var qry = cs.Queryable().Where(c => !c.Name.Contains("Central Services") && !c.Name.Contains("Future Campus")).Select(a => new { Name = a.Name.Replace("Services","").Trim(), URL = a.Url }).OrderBy(n => n.Name).ToList();

        rptCampuses.DataSource = qry;
        rptCampuses.DataBind();

        rptMobileCampuses.DataSource = qry;
        rptMobileCampuses.DataBind();
    }

    protected void lnk_DataBinding(object sender, EventArgs e)
    {
        var lnk = (System.Web.UI.WebControls.HyperLink)sender;
        var name = Eval("Name").ToString();
        var url = "/event-calendar";

        name = name.Replace(" ", "").Replace("Campus", "");

        switch (name.ToLower())
        {
            case "akron":
                url += "?campusID=67713";
                break;
            case "canton":
                url += "?campusID=53103";
                break;
            case "coshocton":
                url += "?campusID=62004";
                break;
            case "dover":
                url += "?campusID=51773";
                break;
            case "millersburg":
                url += "?campusID=51774";
                break;
            case "wooster":
                url += "?campusID=67714";
                break;
            default:
                break;
        }

        lnk.NavigateUrl = url;


        lnk.Text = name;
    }
    protected void lnk2_DataBinding(object sender, EventArgs e)
    {
        var lnk = (System.Web.UI.WebControls.HyperLink)sender;
        var name = Eval("Name").ToString();
        var url = "/EventCalendar";

        switch (name.ToLower())
        {
            case "akron":
                url += "?campusID=67713";
                break;
            case "canton":
                url += "?campusID=53103";
                break;
            case "coshocton":
                url += "?campusID=62004";
                break;
            case "dover":
                url += "?campusID=51773";
                break;
            case "millersburg":
                url += "?campusID=51774";
                break;
            case "wooster":
                url += "?campusID=67714";
                break;
            default:
                break;
        }

        lnk.NavigateUrl = url;
        lnk.Text = name;
    }
}