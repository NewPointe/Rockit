using System.Linq;
using System.Net;
using System.Web.Http;

using org.newpointe.PrivateChat.Model;

using Rock.Rest;
using Rock.Rest.Filters;
using System.Collections;
using Rock.Data;
using System.Collections.Generic;

namespace org.newpointe.PrivateChat.Rest
{
    public class PrivatePrayerRequestsController : ApiController<PrivatePrayerRequest>
    {
        public PrivatePrayerRequestsController() : base(new PrivatePrayerRequestService(new Data.PrivateChatContext())) { }

        public IEnumerable<PrivatePrayerRequest> GetUnansweredPrayers()
        {
            return new PrivatePrayerRequestService(new Data.PrivateChatContext()).Queryable().Where(a => a.Answered != true);
        }
    }
}
