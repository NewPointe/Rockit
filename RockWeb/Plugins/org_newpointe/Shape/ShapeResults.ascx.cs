using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using System.Data;
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
                    SelectedPerson = GetPersonFromForm(Base64Decode(personInUrl)); }

                else
                {
                    //Load the person based on the current person
                    if (CurrentPerson != null)
                    {
                        SelectedPerson = CurrentPerson;
                    }
                    else
                    {
                        nbNoPerson.Visible = true;
                        return;
                    }
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



            // TODO: Look up the gift name, description, associated volunteer roles from defined types


            DefinedTypeService definedTypeService = new DefinedTypeService(rockContext);

            ShapeObject shapeGift1 = new ShapeObject();
            ShapeObject shapeGift2 = new ShapeObject();
            ShapeObject shapeHeart1 = new ShapeObject();
            ShapeObject shapeHeart2 = new ShapeObject();

            // First SHAPE Gift
            shapeGift1.Name =
                definedTypeService.Queryable().ToList()
                    .Where(a => a.Id == SpiritualGift1)
                    .Select(a => a.Name)
                    .FirstOrDefault();



            // Build the UI

            lbPersonName.Text = SelectedPerson.NickName;
            lbGift1Title.Text = shapeGift1.Name;
            lbGift1Body.Text = shapeGift1.Description;




        }




        protected Person GetPersonFromForm(string formId)
        {
            AttributeValueService attributeValueService = new AttributeValueService(rockContext);
            PersonService personService = new PersonService(rockContext);
            PersonAliasService personAliasService = new PersonAliasService(rockContext);
            
            var formAttribute = attributeValueService.Queryable().Where(a => a.Value == formId).FirstOrDefault();
            var person = personService.Queryable().Where(p => p.Id == formAttribute.EntityId).FirstOrDefault();

            return person;
        }



        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }


    }
}

class ShapeObject
{
    public int Type { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
}