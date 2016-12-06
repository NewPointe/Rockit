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
using System.Web;
using System.Web.UI.HtmlControls;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using org.newpointe.Stars.Data;
using org.newpointe.Stars.Model;
using Quartz.Util;
using Rock.Security;
using Rock.Web.UI;
using WebGrease.Css.Extensions;
using HttpResponse = RestSharp.HttpResponse;

namespace RockWeb.Plugins.org_newpointe.Stars
{
    /// <summary>
    /// Template block for a TreeView.
    /// </summary>
    [DisplayName("Stars Report")]
    [Category("NewPointe Stars")]
    [Description(
        "Report to show star totals.")]


    public partial class StarsReport : Rock.Web.UI.RockBlock
    {
        private readonly RockContext _rockContext = new RockContext();
        private readonly StarsProjectContext _starsProjectContext = new StarsProjectContext();
        
        public Person SelectedPerson;

        public int FilterMonth = 0;
        public int FilterYear = 2017;



        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cpCampus.DataSource = CampusCache.All();
                cpCampus.DataBind();  
            }

            starsFilters.Show();

            mypMonth.MinimumYear = 2016;
            mypMonth.MaximumYear = 2017;

            DateTime selectedDate = mypMonth.SelectedDate ?? DateTime.Now;

            mypMonth.SelectedDate = selectedDate;

            FilterMonth = selectedDate.Month;
            FilterYear = selectedDate.Year;

            BindGrid();

        }


        protected void BindGrid()
        {
            StarsService starsService = new StarsService(_starsProjectContext);

            //Get Sum of stars
            var startsList = from x in starsService.Queryable().ToList()
                where x.TransactionDateTime.Month == FilterMonth && x.TransactionDateTime.Year == FilterYear
                group x by x.PersonAlias.Person into g
                select new {
                    Person = g.Key,
                    PersonId = g.Key.Id,
                    Sum = g.Sum(x => x.Value),
                    Month = g.Select(x => x.TransactionDateTime.Month),
                    PersonZip = g.Key.GetFamilies(_rockContext).FirstOrDefault().GroupLocations.FirstOrDefault().Location.PostalCode,
                    PersonCampus = g.Key.GetFamilies(_rockContext).FirstOrDefault().Campus.Name
                };


            //Filter Star Levels
            int starsValueFilter = 0;

            if (!ddlStars.SelectedValue.IsNullOrWhiteSpace())
            {
                starsValueFilter = Convert.ToInt32(ddlStars.SelectedValue);
                startsList = startsList.AsQueryable().Where(a => a.Sum >= starsValueFilter && a.Sum < starsValueFilter + 10);
            }



            //Filter Campuses
            var selectedCampuses = cpCampus.SelectedValues;

            if (selectedCampuses.Count > 0)
            {
                var starsListWithCampus = (from x in startsList.ToList()
                    let firstOrDefault = x.Person.GetFamilies(_rockContext).FirstOrDefault()
                    where firstOrDefault != null && selectedCampuses.Contains(firstOrDefault.Campus.Name) select x);


                startsList = starsListWithCampus;

            }

            //Order the list
            startsList = startsList.OrderBy(g => g.PersonCampus).ThenBy(g => g.PersonZip).ThenBy(g => g.Person.LastName).ThenBy(g => g.Person.FirstName);

            //Bind list to grid
            gStars.DataSource = startsList.ToList();
            gStars.DataBind();
        }


        protected void gStars_OnRowSelected(object sender, RowEventArgs e)
        {
            Response.Redirect("~/Person/" + e.RowKeyValue + "/Stars");
        }

        protected void filters_ApplyFilterClick(object sender, EventArgs e)
        {
            BindGrid();
            starsFilters.Show();
        }

    }
}