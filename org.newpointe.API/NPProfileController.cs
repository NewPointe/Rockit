using System.Web.Http;
using Rock.Data;
using Rock.Model;
using Rock.Rest;

namespace org.newpointe.API.Controllers
{
    [Authorize]
    public class NPProfileController : ApiControllerBase
    {

        public class PersonProfileData
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        public IHttpActionResult Get()
        {
            var person = GetAuthenticatedPerson();
            if (person == null) return Unauthorized();

            return Ok(new PersonProfileData
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Email = person.Email
            });
        }

        public IHttpActionResult Post(PersonProfileData data)
        {
            var rockContext = new RockContext();

            var person = GetAuthenticatedPerson(rockContext);
            if (person == null) return Unauthorized();

            var editPerson = new PersonService(rockContext).Get(person.Id);

            if (data.FirstName != null) editPerson.FirstName = data.FirstName;
            if (data.LastName != null) editPerson.LastName = data.LastName;
            if (data.Email != null) editPerson.Email = data.Email;

            rockContext.SaveChanges();

            return Ok();

        }

        private Person GetAuthenticatedPerson(RockContext rockContext = null)
        {
            var person = GetPerson();

            if (person == null && User != null)
            {
                person = new UserLoginService(rockContext ?? new RockContext()).GetByUserName(User.Identity.Name)?.Person;
            }

            return person;
        }

    }
}
