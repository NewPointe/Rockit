using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Rock;
using Rock.Communication;
using Rock.Data;
using Rock.Model;
using Rock.Rest;
using Rock.Web.Cache;

namespace org.newpointe.API.Controllers
{
    public class NPRegisterController : ApiControllerBase
    {
        public IHttpActionResult Post(RegisterPost register)
        {
            try
            {

                var rockContext = new RockContext();

                if (new UserLoginService(rockContext).GetByUserName(register.Username.Trim()) != null)
                {
                    return BadRequest("The selected username is already taken.");
                }


                if (!UserLoginService.IsPasswordValid(register.Password.Trim()))
                {
                    return BadRequest("Password is invalid.");
                }

                Person person = null;

                // Try to find person by name/email
                var matches = new PersonService(rockContext).GetByMatch(register.FirstName.Trim(), register.LastName.Trim(), register.Email.Trim()).ToList();
                if (matches.Count == 1)
                {
                    // Found them!
                    person = matches.First();
                }
                else
                {
                    // Otherwise create the person and family record for the new person
                    person = new Person
                    {
                        FirstName = register.FirstName.Trim(),
                        LastName = register.LastName.Trim(),
                        Email = register.Email.Trim(),
                        IsEmailActive = true,
                        EmailPreference = EmailPreference.EmailAllowed,
                        RecordTypeValueId = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid()).Id,
                        ConnectionStatusValueId = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_WEB_PROSPECT.AsGuid()).Id,
                        RecordStatusValueId = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_PENDING.AsGuid()).Id,
                        Gender = Gender.Unknown
                    };

                    PersonService.SaveNewPerson(person, rockContext);
                }

                var user = UserLoginService.Create(
                    rockContext,
                    person,
                    AuthenticationServiceType.Internal,
                    EntityTypeCache.Read(Rock.SystemGuid.EntityType.AUTHENTICATION_DATABASE.AsGuid()).Id,
                    register.Username.Trim(),
                    register.Password.Trim(),
                    false
                );

                var mergeObjects = Rock.Lava.LavaHelper.GetCommonMergeFields(null, person);
                mergeObjects.Add("ConfirmAccountUrl", "https://newpointe.org/ConfirmAccount");
                mergeObjects.Add("Person", person);
                mergeObjects.Add("User", user);

                Email.Send(
                    "17aaceef-15ca-4c30-9a3a-11e6cf7e6411".AsGuid(),
                    new List<RecipientData> { new RecipientData(person.Email, mergeObjects) },
                    "",
                    "",
                    false
                );

                return Ok("Success");

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public class RegisterPost
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

    }
}
