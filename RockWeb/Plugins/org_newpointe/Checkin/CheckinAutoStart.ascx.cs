using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock.Attribute;
using Rock.CheckIn;
using Rock.Constants;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock;
using Rock.Data;
using System.Diagnostics;


namespace RockWeb.Plugins.org_newpointe.Checkin
{

    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName( "Checkin AutoStart" )]
    [Category( "NewPointe Check-in" )]
    [Description( "Auto-start check-in with all Group Types selected based on Location." )]
    
    public partial class CheckinAutoStart : Rock.Web.UI.RockBlock
    {

        protected override void OnLoad( EventArgs e )
        {
            RockPage.AddScriptLink( "~/Blocks/CheckIn/Scripts/geo-min.js" );
            RockPage.AddScriptLink( "~/Scripts/iscroll.js" );
            RockPage.AddScriptLink( "~/Scripts/CheckinClient/checkin-core.js" );

            if ( !Page.IsPostBack )
            {

                var rockContext = new RockContext();
                Guid kioskDeviceType = Rock.SystemGuid.DefinedValue.DEVICE_TYPE_CHECKIN_KIOSK.AsGuid();
                var devices = new DeviceService( rockContext ).Queryable()
                        .Where( d => d.DeviceType.Guid.Equals( kioskDeviceType ) )
                        .ToList();
                ddlKiosk.Items.Clear();
                ddlKiosk.DataSource = devices;
                ddlKiosk.DataBind();
                ddlKiosk.Items.Insert( 0, new ListItem( None.Text, None.IdValue ) );

                //Get the campus from the URL
                var campusParam = PageParameter( "Campus" );

                var campus = CampusCache.All().FirstOrDefault( c => c.ShortCode == campusParam || c.Name == campusParam );
                if ( campus != null )
                {
                    var device = devices.FirstOrDefault( d => d.Locations.Select( l => (int?)l.Id ).Contains( campus.LocationId ) );

                    if ( device != null )
                    {
                        ddlKiosk.SelectedValue = device.Id.ToString();
                    }
                }

                string script = string.Format( @"
                    <script>
                        $(document).ready(function (e) {{
                            if (localStorage) {{
                                if (localStorage.checkInKiosk) {{
                                    $('[id$=""hfKiosk""]').val(localStorage.checkInKiosk);
                                    if (localStorage.theme) {{
                                        $('[id$=""hfTheme""]').val(localStorage.theme);
                                    }}
                                    if (localStorage.checkInGroupTypes) {{
                                        $('[id$=""hfGroupTypes""]').val(localStorage.checkInGroupTypes);
                                    }}
                                    {0};
                                }}
                            }}
                        }});
                    </script>
                ", this.Page.ClientScript.GetPostBackEventReference( lbRefresh, "" ) );
                phScript.Controls.Add( new LiteralControl( script ) );

            }
            else
            {
                phScript.Controls.Clear();
            }

            BindGroupTypes();
        }

        /// <summary>
        /// Used by the local storage script to rebind the group types if they were previously
        /// saved via local storage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbRefresh_Click( object sender, EventArgs e )
        {
            ListItem item = ddlKiosk.Items.FindByValue( hfKiosk.Value );
            if ( item != null )
            {
                ddlKiosk.SelectedValue = item.Value;
            }

            BindGroupTypes( hfGroupTypes.Value );
        }

        /// <summary>
        /// Gets the device group types.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns></returns>
        private List<GroupType> GetDeviceGroupTypes( int deviceId, RockContext rockContext )
        {
            var groupTypes = new Dictionary<int, GroupType>();

            var locationService = new LocationService( rockContext );

            // Get all locations (and their children) associated with device
            var locationIds = locationService
                .GetByDevice( deviceId, true )
                .Select( l => l.Id )
                .ToList();

            // Requery using EF
            foreach ( var groupType in locationService
                .Queryable().AsNoTracking()
                .Where( l => locationIds.Contains( l.Id ) )
                .SelectMany( l => l.GroupLocations )
                .Where( gl => gl.Group.GroupType.TakesAttendance )
                .Select( gl => gl.Group.GroupType )
                .ToList() )
            {
                groupTypes.AddOrIgnore( groupType.Id, groupType );
            }

            return groupTypes
                .Select( g => g.Value )
                .OrderBy( g => g.Order )
                .ToList();
        }

        private void BindGroupTypes()
        {
            BindGroupTypes( string.Empty );
        }

        protected void BindGroupTypes( string selectedValues )
        {
            if ( ddlKiosk.SelectedValue != None.IdValue && ddlKiosk.SelectedValue != null )
            {
                var rockContext = new RockContext();
                var kiosk = new DeviceService( rockContext ).Get( ddlKiosk.SelectedValue.AsInteger() );
                if ( kiosk != null )
                {
                    Session["redirectURL"] = "~/checkin?KioskId=" + kiosk.Id + "&GroupTypeIds=" + string.Join( ",", GetDeviceGroupTypes( kiosk.Id, rockContext ) );
                    Response.Redirect( Session["redirectURL"].ToString() );
                }

            }
        }

        public void lbOk_Click( object sender, EventArgs e )
        {
            if ( ddlKiosk.SelectedValue == None.IdValue || ddlKiosk.SelectedValue == null )
            {
                maWarning.Show( "A Kiosk Device needs to be selected!", ModalAlertType.Warning );
            }
            else
            {
                BindGroupTypes();
            }
        }

        protected void lbManual_Click( object sender, EventArgs e )
        {
            Response.Redirect( "~/checkin" );
        }

    }
}