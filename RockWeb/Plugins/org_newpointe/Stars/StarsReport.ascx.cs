using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Entity.SqlServer;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

using org.newpointe.Stars.Data;
using org.newpointe.Stars.Model;

namespace RockWeb.Plugins.org_newpointe.Stars
{
    /// <summary>
    /// Template block for a TreeView.
    /// </summary>
    [DisplayName( "Stars Report" )]
    [Category( "NewPointe Stars" )]
    [Description( "Report to show star totals." )]
    public partial class StarsReport : Rock.Web.UI.RockBlock
    {
        private readonly RockContext _rockContext = new RockContext();
        private readonly StarsProjectContext _starsProjectContext = new StarsProjectContext();

        public Person SelectedPerson;


        protected void Page_Load( object sender, EventArgs e )
        {
            if ( !IsPostBack )
            {
                cpCampus.DataSource = CampusCache.All();
                cpCampus.DataBind();
                BindGrid();
            }

            starsFilters.Show();

            mypMonth.MinimumYear = DateTime.Now.Year - 10;
            mypMonth.MaximumYear = DateTime.Now.Year;

            mypMonth.SelectedDate = mypMonth.SelectedDate ?? DateTime.Now;

        }


        protected void BindGrid()
        {
            StarsService starsService = new StarsService( _starsProjectContext );

            DateTime selectedDate = mypMonth.SelectedDate ?? DateTime.Now;
            var selectedDateBefore = selectedDate.AddMonths( -2 );
            var selectedDateAfter = selectedDate.AddMonths( 2 );

            var starsList = starsService.Queryable()
                .Where( x => x.TransactionDateTime > selectedDateBefore && x.TransactionDateTime < selectedDateAfter )
                .Select( x => new
                {
                    Star = x,
                    Person = x.PersonAlias.Person,
                    SundayDate = SqlFunctions.DateAdd( "day", 7 - ( ( SqlFunctions.DateDiff( "day", x.TransactionDateTime, "19000101" ) % 7 ) + 1 ), x.TransactionDateTime )
                } )
                .Where( x => x.SundayDate != null && ( x.SundayDate.Value.Month == selectedDate.Month && x.SundayDate.Value.Year == selectedDate.Year ) )
                .GroupBy( x => x.Person)
                .Select(x => new { Person = x.Key, Sum = x.Sum( y => y.Star.Value ) } )
                .ToList()
                .Select( x => new { x.Person, x.Sum, Campus = x.Person.GetCampus() } );

            //Filter Campuses
            var selectedCampuses = cpCampus.SelectedValues;
            if ( selectedCampuses.Count > 0 )
            {
                starsList = starsList.Where( x => x.Campus == null || selectedCampuses.Contains( x.Campus.Name ));
            }

            //Get Sum of stars
            var startsList = starsList.Select( g =>
            {
                var personLoc = g.Person.GetHomeLocation();
                return new
                {
                    g.Person,
                    g.Sum,
                    PersonId = g.Person.Id,
                    Month = selectedDate.Month,
                    PersonZip = personLoc != null ? personLoc.PostalCode : "",
                    PersonCampus = g.Campus != null ? g.Campus.Name : ""
                };
            } );

            //Filter Star Levels
            int starsValueFilter = 0;

            if ( !string.IsNullOrWhiteSpace(ddlStars.SelectedValue) )
            {
                starsValueFilter = ddlStars.SelectedValue.AsInteger();
                startsList = startsList.Where( a => a.Sum >= starsValueFilter && a.Sum < starsValueFilter + 10 );
            }

            //Order the list
            startsList = startsList.OrderBy( g => g.PersonCampus ).ThenBy( g => g.PersonZip ).ThenBy( g => g.Person.LastName ).ThenBy( g => g.Person.FirstName );

            //Bind list to grid
            gStars.DataSource = startsList.ToList();
            gStars.DataBind();
        }


        protected void gStars_OnRowSelected( object sender, RowEventArgs e )
        {
            Response.Redirect( "~/Person/" + e.RowKeyValue + "/Stars" );
        }

        protected void filters_ApplyFilterClick( object sender, EventArgs e )
        {
            BindGrid();
            starsFilters.Show();
        }

    }
}