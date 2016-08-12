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
using Quartz.Util;

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
        public int SpiritualGift3;
        public int SpiritualGift4;
        public int Ability1;
        public int Ability2;

        public string PersonEncodedKey = "";

        protected void Page_Load(object sender, EventArgs e)
        {


            if (!string.IsNullOrWhiteSpace(PageParameter("FormId")))
            {
                //Load the person based on the FormId
                var personInUrl = PageParameter("FormId");
                SelectedPerson = GetPersonFromForm(personInUrl);
                PersonEncodedKey = SelectedPerson.UrlEncodedKey;
            }

            else if (!string.IsNullOrWhiteSpace(PageParameter("PersonId")))
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
            DefinedValueService definedValueService = new DefinedValueService(rockContext);

            string spiritualGift1 = "";
            string spiritualGift2 = "";
            string spiritualGift3 = "";
            string spiritualGift4 = "";
            string heartCategories = "";
            string heartCauses = "";
            string heartPassion = "";
            string ability1 = "";
            string ability2 = "";
            string people = "";
            string places = "";
            string events = "";

            var spiritualGift1AttributeValue =
                attributeValueService
                    .Queryable()
                    .FirstOrDefault(a => a.Attribute.Key == "SpiritualGift1" && a.EntityId == SelectedPerson.Id);

                var spiritualGift2AttributeValue =
              attributeValueService
                  .Queryable()
                  .FirstOrDefault(a => a.Attribute.Key == "SpiritualGift2" && a.EntityId == SelectedPerson.Id);

                var spiritualGift3AttributeValue =
              attributeValueService
                  .Queryable()
                  .FirstOrDefault(a => a.Attribute.Key == "SpiritualGift3" && a.EntityId == SelectedPerson.Id);

                var spiritualGift4AttributeValue =
              attributeValueService
                  .Queryable()
                  .FirstOrDefault(a => a.Attribute.Key == "SpiritualGift4" && a.EntityId == SelectedPerson.Id);

                var ability1AttributeValue =
                    attributeValueService
                        .Queryable().FirstOrDefault(a => a.Attribute.Key == "Ability1" && a.EntityId == SelectedPerson.Id);

                var ability2AttributeValue =
                    attributeValueService
                        .Queryable().FirstOrDefault(a => a.Attribute.Key == "Ability2" && a.EntityId == SelectedPerson.Id);

                var peopleAttributeValue = attributeValueService
                    .Queryable()
                    .FirstOrDefault(a => a.Attribute.Key == "SHAPEPeople" && a.EntityId == SelectedPerson.Id);

                var placesAttributeValue = attributeValueService
                    .Queryable()
                    .FirstOrDefault(a => a.Attribute.Key == "SHAPEPlaces" && a.EntityId == SelectedPerson.Id);

                var eventsAttributeValue = attributeValueService
                    .Queryable()
                    .FirstOrDefault(a => a.Attribute.Key == "SHAPEEvents" && a.EntityId == SelectedPerson.Id);

                var heartCategoriesAttributeValue = attributeValueService
                    .Queryable()
                    .FirstOrDefault(a => a.Attribute.Key == "HeartCategories" && a.EntityId == SelectedPerson.Id);

                var heartCausesAttributeValue = attributeValueService
                    .Queryable()
                    .FirstOrDefault(a => a.Attribute.Key == "HeartCauses" && a.EntityId == SelectedPerson.Id);

                var heartPassionAttributeValue = attributeValueService
                    .Queryable()
                    .FirstOrDefault(a => a.Attribute.Key == "HeartPassion" && a.EntityId == SelectedPerson.Id);


                if (spiritualGift1AttributeValue.Value != null) spiritualGift1 = spiritualGift1AttributeValue.Value;
                if (spiritualGift2AttributeValue.Value != null) spiritualGift2 = spiritualGift2AttributeValue.Value;
                if (spiritualGift3AttributeValue.Value != null) spiritualGift3 = spiritualGift3AttributeValue.Value;
                if (spiritualGift4AttributeValue.Value != null) spiritualGift4 = spiritualGift4AttributeValue.Value;
                if (heartCategoriesAttributeValue.Value != null) heartCategories = heartCategoriesAttributeValue.Value;
                if (heartCausesAttributeValue.Value != null) heartCauses = heartCausesAttributeValue.Value;
                if (heartPassionAttributeValue.Value != null) heartPassion = heartPassionAttributeValue.Value;
                if (ability1AttributeValue.Value != null) ability1 = ability1AttributeValue.Value;
                if (ability2AttributeValue.Value != null) ability2 = ability2AttributeValue.Value;
                if (peopleAttributeValue.Value != null) people = peopleAttributeValue.Value;
                if (placesAttributeValue.Value != null) places = placesAttributeValue.Value;
                if (eventsAttributeValue.Value != null) events = eventsAttributeValue.Value;


                string spiritualGift1Guid;
                string spiritualGift2Guid;
                string spiritualGift3Guid;
                string spiritualGift4Guid;
                string ability1Guid;
                string ability2Guid;


            // Check to see if there are already values saved as an ID.  If so, convert them to GUID
            if (spiritualGift1.ToString().Length < 5)
            {
                if (spiritualGift1 != null) SpiritualGift1 = Int32.Parse(spiritualGift1);
                if (spiritualGift2 != null) SpiritualGift2 = Int32.Parse(spiritualGift2);
                if (spiritualGift3 != null) SpiritualGift3 = Int32.Parse(spiritualGift3);
                if (spiritualGift4 != null) SpiritualGift4 = Int32.Parse(spiritualGift4);
                if (ability1 != null) Ability1 = Int32.Parse(ability1);
                if (ability2 != null) Ability2 = Int32.Parse(ability2);

                var intsOfGifts =
                    definedValueService.GetByIds(new List<int>
                    {
                            SpiritualGift1,
                            SpiritualGift2,
                            SpiritualGift3,
                            SpiritualGift4,
                            Ability1,
                            Ability2
                    });

                spiritualGift1Guid = intsOfGifts.ToList()[SpiritualGift1].Guid.ToString();
                spiritualGift2Guid = intsOfGifts.ToList()[SpiritualGift2].Guid.ToString();
                spiritualGift3Guid = intsOfGifts.ToList()[SpiritualGift3].Guid.ToString();
                spiritualGift4Guid = intsOfGifts.ToList()[SpiritualGift4].Guid.ToString();
                ability1Guid = intsOfGifts.ToList()[Ability1].Guid.ToString();
                ability2Guid = intsOfGifts.ToList()[Ability2].Guid.ToString();
            }
            else
            {
                spiritualGift1Guid = spiritualGift1;
                spiritualGift2Guid = spiritualGift2;
                spiritualGift3Guid = spiritualGift3;
                spiritualGift4Guid = spiritualGift4;
                ability1Guid = ability1;
                ability2Guid = ability2;
            }




            // Get all of the data about the assiciated gifts and ability categories
            var shapeGift1Object = definedValueService.GetListByGuids(new List<Guid> { new Guid(spiritualGift1Guid) }).FirstOrDefault();
            var shapeGift2Object = definedValueService.GetListByGuids(new List<Guid> { new Guid(spiritualGift2Guid) }).FirstOrDefault();
            var shapeGift3Object = definedValueService.GetListByGuids(new List<Guid> { new Guid(spiritualGift3Guid) }).FirstOrDefault();
            var shapeGift4Object = definedValueService.GetListByGuids(new List<Guid> { new Guid(spiritualGift4Guid) }).FirstOrDefault();
            var ability1Object = definedValueService.GetListByGuids(new List<Guid> { new Guid(ability1Guid) }).FirstOrDefault();
            var ability2Object = definedValueService.GetListByGuids(new List<Guid> { new Guid(ability2Guid) }).FirstOrDefault();

            shapeGift1Object.LoadAttributes();
            shapeGift2Object.LoadAttributes();
            shapeGift3Object.LoadAttributes();
            shapeGift4Object.LoadAttributes();
            ability1Object.LoadAttributes();
            ability2Object.LoadAttributes();


            // Get heart choices Values from Guids
            string heartCategoriesString = "";
            if (!heartCategories.IsNullOrWhiteSpace())
            {
                string[] heartCategoryArray = heartCategories.Split(',');
                foreach (string category in heartCategoryArray)
                {
                    var definedValueObject =
                        definedValueService.Queryable().FirstOrDefault(a => a.Guid == new Guid(category));

                    if (category.Equals(heartCategoryArray.Last()))
                    {
                        heartCategoriesString += definedValueObject.Value;
                    }
                    else
                    {
                        heartCategoriesString += definedValueObject.Value + ", ";
                    }
                }

            }



            // Get Volunteer Opportunities

            string gift1AssociatedVolunteerOpportunities =
                    shapeGift1Object.GetAttributeValue("AssociatedVolunteerOpportunities");
            string gift2AssociatedVolunteerOpportunities =
                shapeGift2Object.GetAttributeValue("AssociatedVolunteerOpportunities");
            string gift3AssociatedVolunteerOpportunities =
                shapeGift3Object.GetAttributeValue("AssociatedVolunteerOpportunities");
            string gift4AssociatedVolunteerOpportunities =
                shapeGift4Object.GetAttributeValue("AssociatedVolunteerOpportunities");

            string allAssociatedVolunteerOpportunities = gift1AssociatedVolunteerOpportunities + "," +
                                                         gift2AssociatedVolunteerOpportunities + "," +
                                                         gift3AssociatedVolunteerOpportunities + "," +
                                                         gift4AssociatedVolunteerOpportunities;

            if (allAssociatedVolunteerOpportunities != ",,,")
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

                    // Only display connection if it is marked Active
                    if (connection.IsActive == true)
                    {
                        connectionOpportunityList.Add(connection);
                    }
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

            lbGift3Title.Text = shapeGift3Object.Value;
            lbGift3BodyHTML.Text = shapeGift3Object.GetAttributeValue("HTMLDescription");

            lbGift4Title.Text = shapeGift4Object.Value;
            lbGift4BodyHTML.Text = shapeGift4Object.GetAttributeValue("HTMLDescription");

            lbAbility1Title.Text = ability1Object.Value;
            lbAbility1BodyHTML.Text = ability1Object.GetAttributeValue("HTMLDescription");

            lbAbility2Title.Text = ability2Object.Value;
            lbAbility2BodyHTML.Text = ability2Object.GetAttributeValue("HTMLDescription");

            lbPeople.Text = people;
            lbPlaces.Text = places;
            lbEvents.Text = events;

            lbHeartCategories.Text = heartCategoriesString;
            lbHeartCauses.Text = heartCauses;
            lbHeartPassion.Text = heartPassion;

            if (spiritualGift1AttributeValue.ModifiedDateTime != null)
            {
                lbAssessmentDate.Text = spiritualGift1AttributeValue.ModifiedDateTime.Value.ToShortDateString();
            }
            else
            {
                lbAssessmentDate.Text = spiritualGift1AttributeValue.CreatedDateTime.Value.ToShortDateString();
            }


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