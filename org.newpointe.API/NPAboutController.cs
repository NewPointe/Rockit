using System.Web.Http;
using Rock.Rest;
using Rock.Web.Cache;

namespace org.newpointe.API.Controllers
{
    public class NPAboutController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            return Ok(GlobalAttributesCache.Value("AppAboutUs"));
        }
    }
}
