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


using Rock;
 


namespace RockWeb.Plugins.org_newpointe.Staff 
{
    [DisplayName("Staff")]
    [Category("Newpointe")]
    [Description("This will display all member in the central services staff group")]
    public partial class Staff : RockBlock
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            GroupService gs = new GroupService(new RockContext());

            var g = gs.Get(74655); //59654 is the group id for dev site
            var s = g.Members.Select(m => m.Person).OrderBy(m => m.LastName).ThenBy(m => m.FirstName).ToList();

            var people = new List<PersonData>();

            foreach (var person in s)
            {
                person.LoadAttributes();
                people.Add(new PersonData { Name = person.FullName, PhotoUrl = person.PhotoUrl, Position = person.GetAttributeValue("Position") });
            }

            this.rptStaff.DataSource = people;
            this.rptStaff.DataBind();


        }
        protected void lblName_DataBinding(object sender, EventArgs e)
        {
            (sender as Label).Text = Eval("Name").ToString();
        }
        protected void lblJob_DataBinding(object sender, EventArgs e)
        {
            (sender as Label).Text = Eval("Position").ToString();
        }
        protected void img_DataBinding(object sender, EventArgs e)
        {
            (sender as Image).ImageUrl = Eval("PhotoUrl").ToString();
        }
        protected void rptStaff_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemIndex + 1) % 6 == 0)
            {
                var p = new Panel();
                p.CssClass = "visible-lg-block";
                e.Item.Controls.Add(p);
            }
            if ((e.Item.ItemIndex + 1) % 4 == 0)
            {
                var p = new Panel();
                p.CssClass = "clearfix visible-md-block";
                e.Item.Controls.Add(p);
            }
            if ((e.Item.ItemIndex + 1) % 3 == 0)
            {
                var p = new Panel();
                p.CssClass = "clearfix visible-sm-block";
                e.Item.Controls.Add(p);
            }
            if ((e.Item.ItemIndex + 1) % 2 == 0)
            {
                var p = new Panel();
                p.CssClass = "clearfix visible-xs-block";
                e.Item.Controls.Add(p);
            }
        }
    }

    public class PersonData
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public string PhotoUrl { get; set; }
    }
}