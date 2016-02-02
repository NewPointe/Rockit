using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;

using Rock.Workflow;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace RockWeb.Plugins.org_newpointe.Geocodio
{

    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName("Geocod.io Test")]
    [Category("NewPointe Testing")]
    [Description("Geocod.io Test")]
    [TextField("API Key", "API", true, "05d88b8e068e656d704558d85d8fd74370e45d7", "05d88b8e068e656d704558d85d8fd74370e45d7", 1)]


    public partial class Geocodio : Rock.Web.UI.RockBlock

    {

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }


        protected void btnSend_Click(object sender, EventArgs e)
        {
            //Send the email
            result.Visible = true;
            string address = theAddress.Street1 + " " + theAddress.City + " " + theAddress.State + " " + theAddress.PostalCode;


            int personAddress = thePerson.PersonId ?? default(int);

       
            List<int> list = new List<int>();
            list.Add(personAddress);
            var rockContext = new RockContext();
            var personService = new PersonService(rockContext);






            var locationService = new LocationService(rockContext);

            locationService.






            var person = personService.Get(personAddress);

            var homeAddress = personService.GetFirstLocation(personAddress,19);
            string address2 = homeAddress.ToString();

            //string source = GetShortUrlFromString(address);
            string source = GetShortUrlFromString(address2);

            dynamic data = JObject.Parse(source);
            string schoolDistrict = data.results[0].fields.school_districts.unified.name;


            result.Text = source + " <br/><br/>" + schoolDistrict;

            var rc = new RockContext();
            var ats = new AttributeService(rc);
            var argsd = ats.Queryable().Where(x => x.Key == "SchoolDistrict").FirstOrDefault();
            if (argsd == null)
            {
                argsd = new Rock.Model.Attribute();
                argsd.FieldTypeId = 85;
                argsd.EntityTypeId = 15;
                argsd.Key = "SchoolDistrict";
                argsd.Name = "School District";
                argsd.Guid = Guid.NewGuid();
                argsd.CreatedDateTime = argsd.ModifiedDateTime = DateTime.Now;
                ats.Add(argsd);
                rc.SaveChanges();
                rc = new RockContext();
                ats = new AttributeService(rc);
                argsd = ats.Queryable().Where(x => x.Key == "SchoolDistrict").FirstOrDefault();
            }
            if (argsd != null)
            {
                var atvs = new AttributeValueService(rc);
                var argsdVal = atvs.Queryable().Where(x => x.AttributeId == argsd.Id && x.EntityId == person.Id).FirstOrDefault();
                if (argsdVal == null)
                {
                    argsdVal = new Rock.Model.AttributeValue();
                    argsdVal.AttributeId = argsd.Id;
                    argsdVal.EntityId = person.Id;
                    argsdVal.Value = schoolDistrict;
                    argsdVal.Guid = Guid.NewGuid();
                    argsdVal.CreatedDateTime = argsdVal.ModifiedDateTime = DateTime.Now;

                    atvs.Add(argsdVal);
                    rc.SaveChanges();
                }
                else
                {
                    //argsdVal.Value = DateTime.Now.ToString("o");
                    //rc.SaveChanges();
                }
            }


        }

        //Shorten the give URL
        public string GetShortUrlFromString(string addressG)
        {

           string theUrl = String.Format("https://api.geocod.io/v1/geocode?q={0}&fields=school&api_key={1}", Uri.EscapeDataString(addressG), GetAttributeValue("APIKey"));

            //GET request to API
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(theUrl);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    String responseStr = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(responseStr))
                    {
                        throw new ArgumentException(String.Format("Could not get short url for '{0}'", "URL"));
                        
                    }
                    else
                    {
                        return responseStr;
                    }
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();

                }
                throw;
            }
        }


    }
}