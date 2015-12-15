using org.newpointe.SampleProject.Data;
using Rock.Model;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace org.newpointe.SampleProject.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class CalendarService : Rock.Rest.ApiController<Calendar>
    {

       

        

        [HttpGet]
        public HttpResponseMessage ping()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Accepted);


            return response;
        }

    }
}
