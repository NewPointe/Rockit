using Rock.Rest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;

using org.newpointe.PrivateChat.Data;
using org.newpointe.PrivateChat.Model;

[EnableQuery]
public class PrivatePrayerController : ApiControllerBase
{
    public IHttpActionResult Get()
    {
        var data = Initialize();
        return Ok(data.AsQueryable());
    }


    public IEnumerable<PrivatePrayerRequest> Initialize()
    {

        return new PrivatePrayerRequestService(new PrivateChatContext()).Queryable().Where(a => a.Answered != true);
    }
}