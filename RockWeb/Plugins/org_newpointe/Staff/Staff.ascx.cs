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
using Rock.SystemGuid;


namespace RockWeb.Plugins.org_newpointe.Staff 
{
    [DisplayName("Staff")]
    [Category("Newpointe")]
    [Description("This block will display all members of the selected group")]
    [GroupField("Root Group", "Select the root group to use as a starting point for the tree view.", false, order: 1)]
    public partial class Staff : RockBlock
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Guid? rootGroupGuid = GetAttributeValue("RootGroup").AsGuidOrNull();
            GroupService gs = new GroupService(new RockContext());

            if (rootGroupGuid != null)
            {
                var staffGroup = gs.Get(rootGroupGuid.Value);
                var groupMembers = staffGroup.Members.OrderByDescending(g => g.GroupMemberStatus).Select(m => m.Person).OrderBy(m => m.LastName).ThenBy(m => m.FirstName).ToList();
                var groupName = staffGroup.Name.ToString();
                this.lblGroupName.Text = groupName;

                var people = new List<PersonData>();

                foreach (var person in groupMembers)
                {
                    person.LoadAttributes();
                    people.Add(new PersonData { Name = person.FullName, PhotoUrl = person.PhotoUrl, Position = person.GetAttributeValue("Position") });
                }

                this.rptStaff.DataSource = people;
                this.rptStaff.DataBind();
            }
            


        }
        protected void lblName_DataBinding(object sender, EventArgs e)
        {
            var label = sender as Label;
            if (label != null) label.Text = Eval("Name").ToString();
        }

        protected void lblJob_DataBinding(object sender, EventArgs e)
        {
            var label = sender as Label;
            if (label != null) label.Text = Eval("Position").ToString();
        }

        protected void img_DataBinding(object sender, EventArgs e)
        {
            var image = sender as Image;
            if (image != null) image.ImageUrl = Eval("PhotoUrl").ToString();
        }

        protected void rptStaff_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if ((e.Item.ItemIndex + 1) % 6 == 0)
            {
                var p = new Panel();
                p.CssClass = "clearfix visible-lg-block";
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