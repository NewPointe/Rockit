using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using DDay.iCal;
using DotLiquid;
using DotLiquid.Util;
using Humanizer;
using Humanizer.Localisation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;

namespace org.newpointe.Lava
{
    /// <summary>
    /// 
    /// </summary>
    public static class RockFilters
    {
        /// <summary>
        /// Get the Attribute objects for an object
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object NP_GetAttributes( Object input )
        {
            IHasAttributes item = null;

            if ( input == null )
            {
                return string.Empty;
            }

            if ( input is IHasAttributes )
            {
                item = ( IHasAttributes ) input;
            }
            else if ( input is IHasAttributesWrapper )
            {
                item = ( ( IHasAttributesWrapper ) input ).HasAttributesEntity;
            }

            if ( item == null )
            {
                return string.Empty;
            }

            return item.Attributes.Select( x => x.Value );
        }

        /// <summary>
        /// Get the Attribute objects for an object
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object NP_AddSortValue( Object input, Object sortValue, Object objectValue )
        {
            IList items = null;

            if ( input is IList )
            {
                items = ( IList ) input;
            }
            else if ( input is String && String.IsNullOrWhiteSpace( ( String ) input ) )
            {
                items = new List<Object>();
            }

            if (items == null)
            {
                return string.Empty;
            }

            var dict = new Dictionary<String, Object>();
            dict.Add( "SortValue", sortValue );
            dict.Add( "Value", objectValue );
            items.Add( dict );

            return items;
        }

        /// <summary>
        /// Sort elements of the list
        /// </summary>
        /// <param name="input"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IEnumerable NP_SortValues( object input )
        {
            List<object> ary;
            if ( input is IEnumerable )
                ary = ( ( IEnumerable ) input ).Cast<object>().ToList();
            else
                ary = new List<object>( new[] { input } );
            if ( !ary.Any() )
                return ary;

            ary.Sort( ( a, b ) => Comparer.Default.Compare( ( ( IDictionary ) a )["SortValue"], ( ( IDictionary ) b )["SortValue"] ) );

            return ary;
        }

        /// <summary>
        /// Gets the rock context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private static RockContext GetRockContext( DotLiquid.Context context )
        {
            if ( context.Registers.ContainsKey( "rock_context" ) )
            {
                return context.Registers["rock_context"] as RockContext;
            }
            else
            {
                var rockContext = new RockContext();
                context.Registers["rock_context"] = rockContext;
                return rockContext;
            }
        }
    }
}