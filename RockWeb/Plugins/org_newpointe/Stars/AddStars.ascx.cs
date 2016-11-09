using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using System.Data;
using System.Diagnostics;
using System.Text;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Workflow;


using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using org.newpointe.Stars.Data;
using org.newpointe.Stars.Model;
using Quartz.Util;
using Rock.Security;
using Rock.Web.UI;

namespace RockWeb.Plugins.org_newpointe.Stars
{
    /// <summary>
    /// Template block for a TreeView.
    /// </summary>
    [DisplayName("Add Stars")]
    [Category("NewPointe Core")]
    [Description(
        "Shows a person the results of their SHAPE Assessment and recommends volunteer positions based on them.")]
    [LinkedPage("SHAPE Assessment Page", "Choose the Assessment page to redirect to if the person hasn't taken the SHAPE assessment.", true)]


    public partial class AddStars : Rock.Web.UI.RockBlock
    {
        private RockContext rockContext = new RockContext();
        private StarsProjectContext starsProjectContext = new StarsProjectContext();
        public Person SelectedPerson;

        protected void Page_Load(object sender, EventArgs e)
        {
          
            StarsService starsService = new StarsService(starsProjectContext);

            org.newpointe.Stars.Model.Stars stars = new org.newpointe.Stars.Model.Stars();

            stars.PersonAliasId = 1;
            stars.CampusId = 1;
            stars.TransactionDateTime = DateTime.Now;
            stars.Value = 2;

            starsProjectContext.SaveChanges();

        }




    }
}