using System;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Model;

namespace RockWeb.Blocks.CheckIn
{
    /// <summary>
    /// Block to set check-in device id and group types from the query string
    /// </summary>
    [DisplayName("Auto Start")]
    [Category("rocksolidchurch > CheckIn")]
    [Description("Check-in Auto Start block")]
    public partial class AutoStart : CheckInBlock
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Set the check-in state from values passed on query string
            CurrentKioskId = Request.QueryString["KioskId"].AsIntegerOrNull();

            CurrentGroupTypeIds = (Request.QueryString["GroupTypeIds"] ?? "")
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList()
                .Select(s => s.AsInteger())
                .ToList();

            // If valid parameters were used, set state and navigate to welcome page
            if (CurrentKioskId.HasValue && CurrentGroupTypeIds.Any())
            {
                // Save the check-in state
                SaveState();

                // Navigate to the check-in home (welcome) page
                NavigateToHomePage();
            }
        }

        /// <summary>
        /// Handles the Click event of the lbContinue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbContinue_Click(object sender, EventArgs e)
        {
            NavigateToHomePage();
        }
    }
}