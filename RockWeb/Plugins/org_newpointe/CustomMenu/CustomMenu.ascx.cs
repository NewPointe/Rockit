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
using System.Web.Script.Serialization;

[DisplayName("Custom Menu")]
[Category("NewPointe.org Web Blocks")]
[Description("Main menu")]
public partial class Plugins_org_newpointe_CustomMenu_CustomMenu : RockBlock
{

    public string jsonSearch { get; set; }
    public string replaceId = "{{seriesid}}";
    public string replaceImage= "{{seriesimage}}";
    public string replacedId = Rock.Web.Cache.GlobalAttributesCache.Value("LatestMessageSeriesId");
    public string replacedImage = Rock.Web.Cache.GlobalAttributesCache.Value("LatestMessageSeriesImage");


    protected void Page_Load(object sender, EventArgs e)
    {
        var rc = new RockContext();
        var customer = rc.Database.SqlQuery<Aaron>("EXEC GetAaronData").ToList();
        rptMenuLinks.DataSource = customer;
        rptMenuLinks.DataBind();
        rptMenuDivs.DataSource = customer;
        rptMenuDivs.DataBind();

        replacedId = Rock.Web.Cache.GlobalAttributesCache.Value("LatestMessageSeriesId");
        replacedImage = Rock.Web.Cache.GlobalAttributesCache.Value("LatestMessageSeriesImage");
    }

    public class Aaron
    {
        public int ID { get; set; }
        public string PageTitle { get; set; }
        public string HtmlContent { get; set; }
    }

    public class NPSearch
    {
        public int ID  { get; set; }
        public string PageTitle { get; set; }
    }


    protected void rptMenuLinks_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //if(e.Item.ItemType == ListItemType.Footer)
        //{
        //    Repeater repeater = e.Item.FindControl("rptMenuDivs") as Repeater;

        //    var rc = new RockContext();
        //    var data = rc.Database.SqlQuery<Aaron>("EXEC GetAaronData").ToList();
        //    repeater.DataSource = data;
        //    repeater.DataBind();
        //}
    }
}