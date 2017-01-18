using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Search;

namespace org.newpointe.ExtraSearch
{
    /// <summary>
    /// Searches for pages with matching titles
    /// </summary>
    [Description( "Page Title Search" )]
    [Export( typeof( SearchComponent ) )]
    [ExportMetadata( "ComponentName", "Page Title" )]
    public class PageTitle : SearchComponent
    {

        /// <summary>
        /// Gets the attribute value defaults.
        /// </summary>
        /// <value>
        /// The attribute defaults.
        /// </value>
        public override Dictionary<string, string> AttributeValueDefaults
        {
            get
            {
                var defaults = new Dictionary<string, string>();
                defaults.Add( "SearchLabel", "Page Title" );
                return defaults;
            }
        }

        /// <summary>
        /// Returns a list of matching people
        /// </summary>
        /// <param name="searchterm"></param>
        /// <returns></returns>
        public override IQueryable<string> Search( string searchterm )
        {
            var regExp = String.Join( "\\w* ", searchterm.Split( ' ' ).Select( t => Regex.Escape( t ) ) );
            return new PageService( new RockContext() ).Queryable().ToList().Where( p => Regex.IsMatch( p.PageTitle, regExp, RegexOptions.IgnoreCase ) ).Select( p => p.PageTitle ).AsQueryable();
        }
    }
}
