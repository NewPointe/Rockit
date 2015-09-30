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
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Model;
using System.ComponentModel;



namespace RockWeb.Plugins.org_newpointe.MoviesStickyHeader
{
    [DisplayName("MoviesStickyHeader")]
    [Category("Newpointe")]
    [Description("This the sticky block at the top of movies page")]
    public partial class MoviesStickyHeader : RockBlock
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }

}