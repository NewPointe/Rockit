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
    /// Searches for people with matching names, emails, or phone numbers
    /// </summary>
    [Description( "Person Search" )]
    [Export( typeof( SearchComponent ) )]
    [ExportMetadata( "ComponentName", "Person Info" )]
    public class Person : SearchComponent
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
                defaults.Add( "SearchLabel", "Person Info" );
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

            // if has @ then only email
            // if has number then not name
            // if space then not email
            // if letter then not phone
            // ignore everything after :

            var terms = searchterm.Split( ':' );
            var search = terms[0];
            var person = terms.Count() > 1 ? terms[1].Trim().ToLower() : null;
            var rockContext = new RockContext();
            var personServ = new PersonService( rockContext );
            List<string> searchQuery = new List<string>();

            if ( search.Contains( '@' ) )
            {
                if ( person != null )
                    searchQuery.AddRange( personServ.Queryable().Where( p => p.Email.StartsWith( search ) ).Where( p => ( p.NickName + " " + p.LastName ).ToLower().StartsWith( person ) ).OrderBy( p => p.Email ).Select( p => p.Email + ": " + p.NickName + " " + p.LastName ).Distinct() );
                else
                    searchQuery.AddRange( personServ.Queryable().Where( p => p.Email.StartsWith( search ) ).OrderBy( p => p.Email ).Select( p => p.Email + ": " + p.NickName + " " + p.LastName ).Distinct() );
            }
            else
            {
                if ( Regex.Matches( search, @"[0-9]" ).Count == 0 )
                {
                    if ( person != null )
                        searchQuery.AddRange( personServ.GetByFullName( searchterm, true ).Where( p => ( p.NickName + " " + p.LastName ).ToLower().StartsWith( person ) ).Select( p => p.NickName + " " + p.LastName ).Distinct() );
                    else
                        searchQuery.AddRange( personServ.GetByFullName( searchterm, true ).Select( p => p.NickName + " " + p.LastName ).Distinct() );
                }
                if ( Regex.Matches( search, @" " ).Count == 0 )
                {
                    if ( person != null )
                        searchQuery.AddRange( personServ.Queryable().Where( p => p.Email.StartsWith( search ) ).Where( p => ( p.NickName + " " + p.LastName ).ToLower().StartsWith( person ) ).OrderBy( p => p.Email ).Select( p => p.Email + ": " + p.NickName + " " + p.LastName ).Distinct() );
                    else
                        searchQuery.AddRange( personServ.Queryable().Where( p => p.Email.StartsWith( search ) ).OrderBy( p => p.Email ).Select( p => p.Email + ": " + p.NickName + " " + p.LastName ).Distinct() );
                }
                if ( Regex.Matches( search, @"[a-zA-Z]" ).Count == 0 )
                {
                    if ( person != null )
                        searchQuery.AddRange( new PhoneNumberService( rockContext ).GetBySearchterm( searchterm ).DistinctBy( pn => pn.Person ).Where( pn => ( pn.Person.NickName + " " + pn.Person.LastName ).ToLower().StartsWith( person ) ).Select( pn => pn.NumberFormatted + ": " + pn.Person.NickName + " " + pn.Person.LastName ) );
                    else
                        searchQuery.AddRange( new PhoneNumberService( rockContext ).GetBySearchterm( searchterm ).DistinctBy( pn => pn.Person ).Select( pn => pn.NumberFormatted + ": " + pn.Person.NickName + " " + pn.Person.LastName ) );
                }
            }

            return searchQuery.AsQueryable();

        }
    }
}
