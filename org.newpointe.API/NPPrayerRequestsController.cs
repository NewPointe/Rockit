using System;
using System.Web.Http;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Rest;

namespace org.newpointe.API.Controllers
{
    public class NPPrayerRequestsController : ApiControllerBase
    {
        public class PrayerRequestData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Text { get; set; }
            public DateTime? EnteredDateTime { get; set; }
        }

        public IHttpActionResult Post(PrayerRequestData data)
        {
            if (
                string.IsNullOrWhiteSpace(data.FirstName)
                || string.IsNullOrWhiteSpace(data.LastName)
                || string.IsNullOrWhiteSpace(data.Text)
            ) return BadRequest();

            try
            {
                var rockContext = new RockContext();

                var personAlias = GetPersonAlias();

                new PrayerRequestService(rockContext).Add(new PrayerRequest
                {
                    Email = data.Email,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Text = data.Text,
                    CreatedDateTime = data.EnteredDateTime ?? RockDateTime.Now,
                    ModifiedDateTime = data.EnteredDateTime ?? RockDateTime.Now,
                    EnteredDateTime = data.EnteredDateTime ?? RockDateTime.Now,
                    ExpirationDate = RockDateTime.Now.AddDays(14),
                    AllowComments = true,
                    IsUrgent = false,
                    IsPublic = false,
                    IsActive = true,
                    IsApproved = true,
                    ApprovedOnDateTime = RockDateTime.Now,
                    CreatedByPersonAlias = personAlias,
                    RequestedByPersonAlias = personAlias
                });

                rockContext.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}
