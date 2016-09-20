using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Model;
using Rock.Web.UI.Controls;
using System.Text.RegularExpressions;
using System.Data.Entity.SqlServer;
using Rock.Data;
using Rock.Web.Cache;
using System.Diagnostics;
using System.Data.Entity.Core.Objects;
using System.Text;
using RestSharp.Extensions;

namespace RockWeb.Plugins.org_newpointe.PageSearch
{

    /// <summary>
    /// Block to pick a person and get their URL encoded key.
    /// </summary>
    [DisplayName( "Page Search" )]
    [Category( "CMS" )]
    [Description( "Displays list of pages that match a given search type and term." )]

    public partial class PageSearch : Rock.Web.UI.RockBlock
    {
        #region Fields

        private DefinedValueCache _inactiveStatus = null;
        #endregion

        #region Base Control Methods

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            BindGrid();
        }

        #endregion

        #region Events

        protected void gPeople_RowSelected( object sender, RowEventArgs e )
        {
            NavigateToPage( ( Guid ) e.RowKeyValue, null );
        }

        #endregion

        #region Internal Methods

        private void BindGrid()
        {
            string type = PageParameter( "SearchType" );
            string term = PageParameter( "SearchTerm" );

            if ( !string.IsNullOrWhiteSpace( type ) && !string.IsNullOrWhiteSpace( term ) )
            {
                term = term.Trim();
                type = type.Trim().ToLower();

                var terms = term.Split( ' ' );

                var pageService = new PageService( new RockContext() );
                var pages = new List<Page>();

                switch ( type )
                {
                    case ( "name" ):
                        {
                            pages = pageService.Queryable().ToList().Where( p => Regex.IsMatch( p.PageTitle, String.Join("\\w* ", terms.Select(t => Regex.Escape(t))), RegexOptions.IgnoreCase ) ).ToList();
                            break;
                        }
                }

                if ( pages.Count == 1 )
                {
                    NavigateToPage( pages[0].Guid, null );
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    gPages.DataKeyNames = new string[] { "Guid" };
                    gPages.EntityTypeId = EntityTypeCache.GetId<Page>();
                    gPages.DataSource = pages.Select( p => new
                    {
                        p.Guid,
                        p.PageTitle,
                        Structure = ParentStructure( p ),
                        Site = p.Layout.Site.Name
                    } ).ToList();
                    gPages.DataBind();
                }
            }
        }
        private string ParentStructure( Page page, List<int> parentIds = null )
        {
            if ( page == null )
            {
                return string.Empty;
            }

            // Create or add this node to the history stack for this tree walk.
            if ( parentIds == null )
            {
                parentIds = new List<int>();
            }
            else
            {
                // If we have encountered this node before during this tree walk, we have found an infinite recursion in the tree.
                // Truncate the path with an error message and exit.
                if ( parentIds.Contains( page.Id ) )
                {
                    return "#Invalid-Parent-Reference#";
                }
            }

            parentIds.Add( page.Id );

            string prefix = ParentStructure( page.ParentPage, parentIds );

            if ( !string.IsNullOrWhiteSpace( prefix ) )
            {
                prefix += " <i class='fa fa-angle-right'></i> ";
            }

            string pageUrl = RockPage.ResolveUrl( "~/Page/" );

            return string.Format( "{0}<a href='{1}{2}'>{3}</a>", prefix, pageUrl, page.Id, page.PageTitle );
        }
        #endregion
    }
}