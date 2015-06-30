using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [DisplayName("Checkin AutoStart")]
    [Category("NewPointe Check-in")]
    [Description("Auto-start check-in with all Group Types selected based on Location.")]
   


    public partial class CheckinAutoStart : Rock.Web.UI.RockBlock
    {

       // public string redirectURL = "";
        public string listofareas = "";
        public string selectedkiosk = "";
        public int kioskID = 0;
        


        protected override void OnLoad(EventArgs e)
        {
            RockPage.AddScriptLink("~/Blocks/CheckIn/Scripts/geo-min.js");
            RockPage.AddScriptLink("~/Scripts/iscroll.js");
            RockPage.AddScriptLink("~/Scripts/CheckinClient/checkin-core.js");

            //Get the campus from the URL
            string campus = PageParameter("Campus");

            switch (campus)
            {
                case "AKR":
                case "Akron":
                    kioskID = 17;
                    ddlKiosk.SelectedValue = kioskID.ToString();
                    break;

                case "CAN":
                case "Canton":
                    kioskID = 13;
                    ddlKiosk.SelectedValue = kioskID.ToString();
                    break;

                case "COS":
                case "Coshocton":
                    kioskID = 16;
                    ddlKiosk.SelectedValue = kioskID.ToString();
                    break;

                case "DOV":
                case "Dover":
                    kioskID = 14;
                    ddlKiosk.SelectedValue = kioskID.ToString();
                    break;

                case "MIL":
                case "Millersburg":
                    kioskID = 15;
                    ddlKiosk.SelectedValue = kioskID.ToString();
                    break;

                case "WST":
                case "Wooster":
                    kioskID = 18;
                    ddlKiosk.SelectedValue = kioskID.ToString();
                    break;


            }

            if (!Page.IsPostBack)
            {


                Guid kioskDeviceType = Rock.SystemGuid.DefinedValue.DEVICE_TYPE_CHECKIN_KIOSK.AsGuid();
                ddlKiosk.Items.Clear();
                using (var rockContext = new RockContext())
                {
                    ddlKiosk.DataSource = new DeviceService(rockContext).Queryable()
                        .Where(d => d.DeviceType.Guid.Equals(kioskDeviceType))
                        .ToList();
                }
                ddlKiosk.DataBind();
                ddlKiosk.Items.Insert(0, new ListItem(None.Text, None.IdValue));

                string script = string.Format(@"
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
                ", this.Page.ClientScript.GetPostBackEventReference(lbRefresh, ""));
                phScript.Controls.Add(new LiteralControl(script));


            }
            else
            {
                phScript.Controls.Clear();
            }


            //if (ddlKiosk.SelectedValue != None.IdValue)
            //{
                BindGroupTypes();
                //Response.Redirect(Session["redirectURL"].ToString());
            //}
        }


        protected void ddlKiosk_SelectedIndexChanged(object sender, EventArgs e)
        {
            //BindGroupTypes();
        }


        /// <summary>
        /// Used by the local storage script to rebind the group types if they were previously
        /// saved via local storage.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbRefresh_Click(object sender, EventArgs e)
        {

                ListItem item = ddlKiosk.Items.FindByValue(hfKiosk.Value);
                if (item != null)
                {
                    ddlKiosk.SelectedValue = item.Value;
                }

                BindGroupTypes(hfGroupTypes.Value);
         }

        
        /// <summary>
        /// Sets the "DeviceId" cookie to expire after TimeToCacheKioskGeoLocation minutes
        /// if IsMobile is set.
        /// </summary>
        /// <param name="kiosk"></param>
        private void SetDeviceIdCookie(Device kiosk)
        {
            // set an expiration cookie for these coordinates.
            double timeCacheMinutes = double.Parse(GetAttributeValue("TimetoCacheKioskGeoLocation") ?? "0");

            HttpCookie deviceCookie = Request.Cookies[Rock.CheckIn.CheckInBlock.CheckInCookie.DEVICEID];
            if (deviceCookie == null)
            {
                deviceCookie = new HttpCookie(Rock.CheckIn.CheckInBlock.CheckInCookie.DEVICEID, kiosk.Id.ToString());
            }

            deviceCookie.Expires = (timeCacheMinutes == 0) ? DateTime.MaxValue : RockDateTime.Now.AddMinutes(timeCacheMinutes);
            Response.Cookies.Set(deviceCookie);

            HttpCookie isMobileCookie = new HttpCookie(Rock.CheckIn.CheckInBlock.CheckInCookie.ISMOBILE, "true");
            Response.Cookies.Set(isMobileCookie);
        }

        /// <summary>
        /// Clears the flag cookie that indicates this is a "mobile" device kiosk.
        /// </summary>
        private void ClearMobileCookie()
        {
            HttpCookie isMobileCookie = new HttpCookie(Rock.CheckIn.CheckInBlock.CheckInCookie.ISMOBILE);
            isMobileCookie.Expires = RockDateTime.Now.AddDays(-1d);
            Response.Cookies.Set(isMobileCookie);
        }


        /// <summary>
        /// Gets the device group types.
        /// </summary>
        /// <param name="deviceId">The device identifier.</param>
        /// <returns></returns>
        private List<GroupType> GetDeviceGroupTypes(int deviceId, RockContext rockContext)
        {
            var groupTypes = new Dictionary<int, GroupType>();

            var locationService = new LocationService(rockContext);

            // Get all locations (and their children) associated with device
            var locationIds = locationService
                .GetByDevice(deviceId, true)
                .Select(l => l.Id)
                .ToList();

            // Requery using EF
            foreach (var groupType in locationService.Queryable()
                .Where(l => locationIds.Contains(l.Id))
                .SelectMany(l => l.GroupLocations)
                .Where(gl => gl.Group.GroupType.TakesAttendance)
                .Select(gl => gl.Group.GroupType)
                .Distinct()
                .ToList())
            {
                if (!groupTypes.ContainsKey(groupType.Id))
                {
                    groupTypes.Add(groupType.Id, groupType);
                }
            }

            return groupTypes.Select(g => g.Value).ToList();
        }


        private void BindGroupTypes()
        {
            BindGroupTypes(string.Empty);
        }

        protected void BindGroupTypes(string selectedValues)
        {

            if (ddlKiosk.SelectedValue != None.IdValue)
            {
                using (var rockContext = new RockContext())
                {
                    var kiosk = new DeviceService(rockContext).Get(Int32.Parse(ddlKiosk.SelectedValue));
                    if (kiosk != null)
                    {
                        foreach (var id in GetDeviceGroupTypes(kiosk.Id, rockContext))
                        {
                            listofareas += id.Id + ",";
                            selectedkiosk = kiosk.Id.ToString();

                        }
                    }
                }
            }

            //grouplist.Text = "?KioskId=" + selectedkiosk + "&GroupTypeIds=" + listofareas.TrimEnd(',');
            Session["redirectURL"] = "~/checkin?KioskId=" + selectedkiosk + "&GroupTypeIds=" + listofareas.TrimEnd(',');

            if (kioskID != 0)
            {
                Response.Redirect(Session["redirectURL"].ToString());
            }

        }

        public void lbOk_Click(object sender, EventArgs e)
        {
            if (ddlKiosk.SelectedValue == None.IdValue)
            {
                maWarning.Show("A Kiosk Device needs to be selected!", ModalAlertType.Warning);
                return;
            }

            else if (ddlKiosk.SelectedValue == null)
            {
                maWarning.Show("A Kiosk Device needs to be selected!", ModalAlertType.Warning);
                return;
            }

            else
            {
                BindGroupTypes();
                Response.Redirect(Session["redirectURL"].ToString());
            }

        }


        protected void lbManual_Click(object sender, EventArgs e)
        {
            if (ddlKiosk.SelectedValue == None.IdValue)
            {
                maWarning.Show("A Kiosk Device needs to be selected!", ModalAlertType.Warning);
                return;
            }

            Response.Redirect("~/checkin");

        }


    }
}