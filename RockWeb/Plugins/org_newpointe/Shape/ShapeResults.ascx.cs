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

        public string SpiritualGift1;
        public string SpiritualGift2;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (!string.IsNullOrWhiteSpace(PageParameter("FormId")))
                {
                    //Load the person based on the FormId
                    SelectedPerson = GetPersonFromForm(PageParameter("FormId"));
                }
                else
                {
                    //Load the person based on the current person
                    if (CurrentPerson != null)
                    {
                        SelectedPerson = CurrentPerson;
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


            if (spiritualGift1 != null) SpiritualGift1 = spiritualGift1.Value;
            if (spiritualGift2 != null) SpiritualGift2 = spiritualGift2.Value;


            // TODO: Look up the gift name, description, associated volunteer roles from defined types

            DefinedTypeService definedTypeService = new DefinedTypeService(rockContext);

            

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
    }
}