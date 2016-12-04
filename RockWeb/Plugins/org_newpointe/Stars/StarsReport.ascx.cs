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
        private RockContext rockContext = new RockContext();
        private StarsProjectContext starsProjectContext = new StarsProjectContext();
        
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

            mypMonth.MinimumYear = 2016;
            mypMonth.MaximumYear = 2017;

            DateTime selectedDate = mypMonth.SelectedDate ?? DateTime.Now;

            FilterMonth = selectedDate.Month;
            FilterYear = selectedDate.Year;

            BindGrid();

        }


        protected void BindGrid()
        {
            StarsService starsService = new StarsService(starsProjectContext);

            var starsList =
                starsService.Queryable()
                    .Where(a => a.TransactionDateTime.Month == FilterMonth && a.TransactionDateTime.Year == FilterYear)
                    .GroupBy(a => a.PersonAlias.Person).Select(
                        g =>
                            new
                            {
                                Person = g.Key,
                                PersonId = g.Key.Id,
                                Sum = g.Sum(a => a.Value),
                                Month = g.Select(a => a.TransactionDateTime.Month)
                            });


            //Filter Star Levels

            int starsValueFilter = 0;

            if (!ddlStars.SelectedValue.IsNullOrWhiteSpace())
            {
                starsValueFilter = Convert.ToInt32(ddlStars.SelectedValue);
                starsList = starsList.Where(a => a.Sum >= starsValueFilter && a.Sum < starsValueFilter + 10);
            }



            //Filter Campuses

            var selectedCampuses = cpCampus.SelectedValues;

            if (selectedCampuses.Count > 0)
            {
                starsList = starsList.Where(a => selectedCampuses.Contains(a.Person.GetCampus().Name));
            }



            var starsListForGrid = starsList.ToList();


            gStars.DataSource = starsListForGrid;
            gStars.DataBind();
        }


        protected void gStars_OnRowSelected(object sender, RowEventArgs e)
        {
            Response.Redirect("~/Person/" + e.RowKeyValue);
        }

        protected void filters_ApplyFilterClick(object sender, EventArgs e)
        {
            BindGrid();
        }

    }
}