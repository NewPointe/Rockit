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
    public class ReferralAgencyService : SampleProjectService<ReferralAgency>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferralAgencyService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ReferralAgencyService( SampleProjectContext context ) : base( context ) { }

        [HttpGet]
        public HttpResponseMessage ping()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Accepted);


            return response;
        }
    }
}
