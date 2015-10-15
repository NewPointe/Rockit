using Rock.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock.Attribute;
using Rock.Data;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Model;
using System.ComponentModel;


using Rock;

[DisplayName("Page Redirect")]
[Category("NewPointe.org Web Blocks")]
[Description("Create a page for friendly URL and set this block to redirect to the correct message or event page")]
[TextField("ID", "Enter Series ID", required: true)]
public partial class Plugins_org_newpointe_PageRedirect_PageRedirect : Rock.Web.UI.RockBlock
{
    protected void Page_Load(object sender, EventArgs e)
    {

        var isAdmin = Request.QueryString["isAdmin"];
        var type = GetAttributeValue("ChooseType");
        var id = GetAttributeValue("ID");
        if (isAdmin != "true")
        {


            if (!string.IsNullOrEmpty(id))
            {
                Response.Redirect("/message-archive/message?SeriesID=" + id);
            }
        }

    }


}