using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Attribute;


namespace RockWeb.Plugins.org_newpointe.PersonGetURLEncodedKey
{

    /// <summary>
    /// Block to pick a person and get their URL encoded key.
    /// </summary>
    [DisplayName("APerson Get URL Encoded Key")]
    [Category("NewPointe Person")]
    [Description("Gets the URL Enceded Key for the given person.")]
    [BooleanField("Set to Logged In Person", "Yes", "No", "Choose whether or not you want to set the picker to the logged in person on page load.")]


    public partial class PersonGetURLEncodedKey : Rock.Web.UI.RockBlock
    {
        //public Guid typeGuid;
        RockContext rockContext = new RockContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            //Set the person picker to the currently logged in person.
            PersonService personService = new PersonService(rockContext);
            var personObject = personService.Get(CurrentPerson.Guid);
            ppPerson.SetValue(personObject);
        }

        protected void ppPerson_SelectPerson(object sender, EventArgs e)
        {
            //Get the person from the PersonPicker
            int? ppPersonAliasId = ppPerson.PersonAliasId;
            int personAliasIdInt = ppPersonAliasId.Value;

            PersonAliasService personAliasService = new PersonAliasService(rockContext);
            var thePerson = personAliasService.Queryable().AsNoTracking()
                .Where(a => a.AliasPersonId == personAliasIdInt)
                .Select(a => a.Person)
                .FirstOrDefault();

            //Get the URL encoded key from the person object
            var urlEncodedKey = thePerson.UrlEncodedKey;

            //Show the key to the user
            nbSuccess.Visible = true;
            nbSuccess.Text = urlEncodedKey;
            
        }

    }
}