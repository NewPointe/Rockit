﻿using System;
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
using System.Web.UI.HtmlControls;
using Rock.Security;
using Rock.Web.UI;

namespace RockWeb.Plugins.org_newpointe.Shape
{
    /// <summary>
    /// Template block for a TreeView.
    /// </summary>
    [DisplayName("SHAPE Results Internal")]
    [Category("NewPointe Core")]
    [Description("Shows a person the results of their SHAPE Assessment and recommends volunteer positions based on them.  Formatted for use on the internal Rock site.")]

    public partial class ShapeResultsInternal : Rock.Web.UI.RockBlock
    {
        private RockContext rockContext = new RockContext();
        public Person SelectedPerson;

        public int SpiritualGift1;
        public int SpiritualGift2;
        public int Heart1;
        public int Heart2;

        protected void Page_Load(object sender, EventArgs e)
        {

                if (!string.IsNullOrWhiteSpace(PageParameter("PersonId")))
                {
                    //Load the person based on the PersonId
                    SelectedPerson = GetPersonFromId(PageParameter("PersonId"));
                }

                else
                {
                    //Show Error Message
                    nbNoPerson.Visible = true;
                    return;
                }
            

            // Load the attributes

            AttributeValueService attributeValueService = new AttributeValueService(rockContext);

            var spiritualGift1 =
                attributeValueService
                    .Queryable()
                    .FirstOrDefault(a => a.Attribute.Key == "SpiritualGift1" && a.EntityId == SelectedPerson.Id);

            var spiritualGift2 =
                attributeValueService
                    .Queryable()
                    .FirstOrDefault(a => a.Attribute.Key == "SpiritualGift2" && a.EntityId == SelectedPerson.Id);

            var heart1 =
                attributeValueService
                    .Queryable().FirstOrDefault(a => a.Attribute.Key == "Heart1" && a.EntityId == SelectedPerson.Id);

            var heart2 =
                attributeValueService
                    .Queryable().FirstOrDefault(a => a.Attribute.Key == "Heart2" && a.EntityId == SelectedPerson.Id);

            var people = attributeValueService
                .Queryable()
                .FirstOrDefault(a => a.Attribute.Key == "SHAPEPeople" && a.EntityId == SelectedPerson.Id)
                .Value;

            var places = attributeValueService
                .Queryable()
                .FirstOrDefault(a => a.Attribute.Key == "SHAPEPlaces" && a.EntityId == SelectedPerson.Id)
                .Value;

            var events = attributeValueService
                .Queryable()
                .FirstOrDefault(a => a.Attribute.Key == "SHAPEEvents" && a.EntityId == SelectedPerson.Id)
                .Value;


            if (spiritualGift1 != null) SpiritualGift1 = Int32.Parse(spiritualGift1.Value);
            if (spiritualGift2 != null) SpiritualGift2 = Int32.Parse(spiritualGift2.Value);
            if (heart1 != null) Heart1 = Int32.Parse(heart1.Value);
            if (heart2 != null) Heart2 = Int32.Parse(heart2.Value);



            // Get all of the data about the assiciated gifts and heart categories

            DefinedValueService definedValueService = new DefinedValueService(rockContext);

            var shapeGift1Object = definedValueService.GetListByIds(new List<int> {SpiritualGift1}).FirstOrDefault();
            var shapeGift2Object = definedValueService.GetListByIds(new List<int> {SpiritualGift2}).FirstOrDefault();
            var heart1Object = definedValueService.GetListByIds(new List<int> {Heart1}).FirstOrDefault();
            var heart2Object = definedValueService.GetListByIds(new List<int> {Heart2}).FirstOrDefault();

            shapeGift1Object.LoadAttributes();
            shapeGift2Object.LoadAttributes();
            heart1Object.LoadAttributes();
            heart2Object.LoadAttributes();



            // Get Volunteer Opportunities

            string gift1AssociatedVolunteerOpportunities =
                shapeGift1Object.GetAttributeValue("AssociatedVolunteerOpportunities");
            string gift2AssociatedVolunteerOpportunities =
                shapeGift2Object.GetAttributeValue("AssociatedVolunteerOpportunities");
            string allAssociatedVolunteerOpportunities = gift1AssociatedVolunteerOpportunities + "," +
                                                         gift2AssociatedVolunteerOpportunities;

            if (allAssociatedVolunteerOpportunities != ",")
            {
                List<int> associatedVolunteerOpportunitiesList =
                    allAssociatedVolunteerOpportunities.Split(',').Select(t => int.Parse(t)).ToList();
                Dictionary<int, int> VolunteerOpportunities = new Dictionary<int, int>();


                var i = 0;
                var q = from x in associatedVolunteerOpportunitiesList
                    group x by x
                    into g
                    let count = g.Count()
                    orderby count descending
                    select new {Value = g.Key, Count = count};
                foreach (var x in q)
                {
                    VolunteerOpportunities.Add(i, x.Value);
                    i++;
                }

                ConnectionOpportunityService connectionOpportunityService = new ConnectionOpportunityService(rockContext);
                List<ConnectionOpportunity> connectionOpportunityList = new List<ConnectionOpportunity>();

                int z = 0;
                foreach (KeyValuePair<int, int> entry in VolunteerOpportunities.Take(4))
                {
                    var connection = connectionOpportunityService.GetByIds(new List<int> {entry.Value}).FirstOrDefault();
                    connectionOpportunityList.Add(connection);
                }

                rpVolunteerOpportunities.DataSource = connectionOpportunityList;
                rpVolunteerOpportunities.DataBind();
            }


            //Get DISC Info

            DiscService.AssessmentResults savedScores = DiscService.LoadSavedAssessmentResults(SelectedPerson);

            if (!savedScores.Equals(null))
            {
                ShowResults(savedScores);
                DISCResults.Visible = true;
                NoDISCResults.Visible = false;
            }






            // Build the UI

            lbPersonName.Text = SelectedPerson.FullName;

            lbGift1Title.Text = shapeGift1Object.Value;
            lbGift1BodyHTML.Text = shapeGift1Object.GetAttributeValue("HTMLDescription");

            lbGift2Title.Text = shapeGift2Object.Value;
            lbGift2BodyHTML.Text = shapeGift2Object.GetAttributeValue("HTMLDescription");

            lbHeart1Title.Text = heart1Object.Value;
            lbHeart1BodyHTML.Text = heart1Object.GetAttributeValue("HTMLDescription");

            lbHeart2Title.Text = heart2Object.Value;
            lbHeart2BodyHTML.Text = heart2Object.GetAttributeValue("HTMLDescription");

            lbPeople.Text = people;
            lbPlaces.Text = places;
            lbEvents.Text = events;


        }




        protected Person GetPersonFromForm(string formId)
        {
            AttributeValueService attributeValueService = new AttributeValueService(rockContext);
            PersonService personService = new PersonService(rockContext);
            PersonAliasService personAliasService = new PersonAliasService(rockContext);

            var formAttribute = attributeValueService.Queryable().FirstOrDefault(a => a.Value == formId);
            var person = personService.Queryable().FirstOrDefault(p => p.Id == formAttribute.EntityId);

            return person;
        }


        protected Person GetPersonFromId(string PersonId)
        {

            int intPersonId = Int32.Parse(PersonId);
            PersonService personService = new PersonService(rockContext);

            var person = personService.Queryable().FirstOrDefault(p => p.Id == intPersonId);

            return person;
        }



        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }




        /// <summary>
        /// Shows the results of the assessment test.
        /// </summary>
        /// <param name="savedScores">The saved scores.</param>
        private void ShowResults(DiscService.AssessmentResults savedScores)
        {
            // Plot the Natural graph
            DiscService.PlotOneGraph(discNaturalScore_D, discNaturalScore_I, discNaturalScore_S, discNaturalScore_C,
                savedScores.NaturalBehaviorD, savedScores.NaturalBehaviorI, savedScores.NaturalBehaviorS,
                savedScores.NaturalBehaviorC, 35);
            ShowExplaination(savedScores.PersonalityType);

        }

        /// <summary>
        /// Shows the explaination for the given personality type as defined in one of the
        /// DefinedValues of the DISC Results DefinedType.
        /// </summary>
        /// <param name="personalityType">The one or two letter personality type.</param>
        private void ShowExplaination(string personalityType)
        {
            var personalityValue =
                DefinedTypeCache.Read(Rock.SystemGuid.DefinedType.DISC_RESULTS_TYPE.AsGuid())
                    .DefinedValues.Where(v => v.Value == personalityType)
                    .FirstOrDefault();
            if (personalityValue != null)
            {
                lDescription.Text = personalityValue.Description;
                lStrengths.Text = personalityValue.GetAttributeValue("Strengths");
                lChallenges.Text = personalityValue.GetAttributeValue("Challenges");
                lUnderPressure.Text = personalityValue.GetAttributeValue("UnderPressure");
                lMotivation.Text = personalityValue.GetAttributeValue("Motivation");
                lTeamContribution.Text = personalityValue.GetAttributeValue("TeamContribution");
                lLeadershipStyle.Text = personalityValue.GetAttributeValue("LeadershipStyle");
                lFollowerStyle.Text = personalityValue.GetAttributeValue("FollowerStyle");
            }
        }


    }
}