using System;
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
    [DisplayName("SHAPE Results")]
    [Category("NewPointe Core")]
    [Description("Shows a person the results of their SHAPE Assessment and recommends volunteer positions based on them.")]

    public partial class ShapeResults : Rock.Web.UI.RockBlock
    {
        private RockContext rockContext = new RockContext();
        private Person SelectedPerson;

        public int SpiritualGift1;
        public int SpiritualGift2;
        public int Heart1;
        public int Heart2;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!string.IsNullOrWhiteSpace(PageParameter("FormId")))
                {
                    //Load the person based on the FormId
                    var personInUrl = PageParameter("FormId");
                    SelectedPerson = GetPersonFromForm(personInUrl); }

                else if (!string.IsNullOrWhiteSpace(PageParameter("PersonId")))
                {
                    //Load the person based on the PersonId
                    GetPersonFromId(PageParameter("PersonId"));
                }

                else if (CurrentPerson != null)
                {
                    //Load the person based on the currently logged in person
                    SelectedPerson = CurrentPerson;
                }

                else
                {
                    //Show Error Message
                    nbNoPerson.Visible = true;
                    return;
                }
            }

            // Load the attributes

            AttributeValueService attributeValueService = new AttributeValueService(rockContext);

            var spiritualGift1 =
                attributeValueService
                    .Queryable().FirstOrDefault(a => a.Attribute.Key == "SpiritualGift1" && a.EntityId == SelectedPerson.Id);

            var spiritualGift2 =
                attributeValueService
                    .Queryable().FirstOrDefault(a => a.Attribute.Key == "SpiritualGift2" && a.EntityId == SelectedPerson.Id);

            var heart1 =
                attributeValueService
                    .Queryable().FirstOrDefault(a => a.Attribute.Key == "Heart1" && a.EntityId == SelectedPerson.Id);

            var heart2 =
                attributeValueService
                    .Queryable().FirstOrDefault(a => a.Attribute.Key == "Heart2" && a.EntityId == SelectedPerson.Id);


            if (spiritualGift1 != null) SpiritualGift1 = Int32.Parse(spiritualGift1.Value);
            if (spiritualGift2 != null) SpiritualGift2 = Int32.Parse(spiritualGift2.Value);
            if (heart1 != null) Heart1 = Int32.Parse(heart1.Value);
            if (heart2 != null) Heart2 = Int32.Parse(heart2.Value);



            // Get all of the data about the assiciated gifts and heart categories
            
            DefinedValueService definedValueService = new DefinedValueService(rockContext);

            var shapeGift1Object = definedValueService.GetListByIds(new List<int> { SpiritualGift1 }).FirstOrDefault();
            var shapeGift2Object = definedValueService.GetListByIds(new List<int> { SpiritualGift2 }).FirstOrDefault();
            var heart1Object = definedValueService.GetListByIds(new List<int> { Heart1 }).FirstOrDefault();
            var heart2Object = definedValueService.GetListByIds(new List<int> { Heart2 }).FirstOrDefault();

            shapeGift1Object.LoadAttributes();
            shapeGift2Object.LoadAttributes();
            heart1Object.LoadAttributes();
            heart2Object.LoadAttributes();



            // Get Volunteer Opportunities

            string vol1 = shapeGift1Object.GetAttributeValue("AssociatedVolunteerOpportunities");
            string vol2 = shapeGift2Object.GetAttributeValue("AssociatedVolunteerOpportunities");
            string allVol = vol1 + "," + vol2;

            List<int> TagIds = allVol.Split(',').Select(t => int.Parse(t)).ToList();
            Dictionary<int, int> VolunteerOpportunities = new Dictionary<int, int>();

            var i = 0;
            var q = from x in TagIds
                    group x by x into g
                    let count = g.Count()
                    orderby count descending
                    select new { Value = g.Key, Count = count };
            foreach (var x in q)
            {
                VolunteerOpportunities.Add(i,x.Value);
                i++;
            }

            int volunteerOpportunity1 = VolunteerOpportunities[0];
            int volunteerOpportunity2 = VolunteerOpportunities[1];
            int volunteerOpportunity3 = VolunteerOpportunities[2];
            int volunteerOpportunity4 = 8;

            ConnectionOpportunityService connectionOpportunityService = new ConnectionOpportunityService(rockContext);

            ConnectionOpportunity connectionOpportunityObject1 = new ConnectionOpportunity();
            ConnectionOpportunity connectionOpportunityObject2 = new ConnectionOpportunity();
            ConnectionOpportunity connectionOpportunityObject3 = new ConnectionOpportunity();
            ConnectionOpportunity connectionOpportunityObject4 = new ConnectionOpportunity();

            connectionOpportunityService.TryGet(volunteerOpportunity1, out connectionOpportunityObject1);
            connectionOpportunityService.TryGet(volunteerOpportunity2, out connectionOpportunityObject2);
            connectionOpportunityService.TryGet(volunteerOpportunity3, out connectionOpportunityObject3);
            connectionOpportunityService.TryGet(volunteerOpportunity4, out connectionOpportunityObject4);

            List<ConnectionOpportunity> connectionOpportunityList = new List<ConnectionOpportunity>();

            connectionOpportunityList.Add(connectionOpportunityObject1);
            connectionOpportunityList.Add(connectionOpportunityObject2);
            connectionOpportunityList.Add(connectionOpportunityObject3);
            connectionOpportunityList.Add(connectionOpportunityObject4);

            rpVolunteerOpportunities.DataSource = connectionOpportunityList;
            rpVolunteerOpportunities.DataBind();




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
            PersonService personService = new PersonService(rockContext);

            var person = personService.Queryable().FirstOrDefault(p => p.Id == Int32.Parse(PersonId));

            return person;
        }



        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }


    }
}