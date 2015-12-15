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


namespace RockWeb.Plugins.org_newpointe.Alerts
{
    [DisplayName("Alerts")]
    [Category("NewPointe Web Blocks")]
    [Description("Main menu")]
    [DateTimeField("Start")]
    [CodeEditorField("Text", "The text (HTML) to display at the top of the confirmation section.  <span class='tip tip-lava'></span> <span class='tip tip-html'></span>",
        CodeEditorMode.Html, CodeEditorTheme.Rock, 200, true, @"
<h3>Snow Closings</h3>
<h4>Dover Campus</h4>
<p>
There will be no 9:00am Service at Dover due to snow.
</p>
", "Text Options")]


    public partial class Alerts : RockBlock
{
    protected void Page_Load(object sender, EventArgs e)
    {

            maWarning.Show(GetAttributeValue("Text"), ModalAlertType.Alert);

        }
}
}