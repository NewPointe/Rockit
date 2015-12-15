using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using DotLiquid;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Model;



namespace RockWeb.Plugins.org_newpointe.ChooseCampus
{
    [DisplayName("Choose Campus")]
    [Category("NewPointe.org Web Blocks")]
    [Description("Choose Campuse widget for the homepage")]
    public partial class ChooseCampus : RockBlock
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            CampusService campusService = new CampusService(new RockContext());

            var qry = campusService.Queryable().Where(c => !c.Name.Contains("Central Services") && !c.Name.Contains("Online") && !c.Name.Contains("Future")).Select(p => new { Name = p.Name.Replace("Campus", "").Trim(), URL = p.Url }).OrderBy(c => c.Name).ToList();

            rptCampuses.DataSource = qry;
            rptCampuses.DataBind();

        }

        protected void lnk_DataBinding(object sender, EventArgs e)
        {
            var lnk = (System.Web.UI.WebControls.HyperLink)sender;
            var name = Eval("Name").ToString();
            var url = Eval("URL");
            if (url != null)
            {
                lnk.NavigateUrl =  url.ToString().Trim();
            }
            lnk.Text = name;
            lnk.CssClass = "btn btn-default btn-block " + name;
        }
    }

    public class Test
    {

    }

}

 