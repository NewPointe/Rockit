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
        public static object GetAttributes( Object input )
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
        public static object AddKeyValue( Object input, Object key, Object value )
        {
            IDictionary item = null;

            if ( input is IDictionary )
            {
                item = ( IDictionary ) input;
            }
            else if ( input is String && String.IsNullOrWhiteSpace( ( String ) input ) )
            {
                item = new Dictionary<Object, Object>();
            }
            else
            {
                return string.Empty;
            }

            item.Add( key, value );

            return item;
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