using System;
using System.Linq;
using System.Web.Http;
using Rock.Data;
using Rock.Model;
using Rock.Rest;
using Rock.Security;

namespace org.newpointe.API.Controllers
{
    public class NPMyNotesController : ApiControllerBase
    {
        public IHttpActionResult GetById(int id)
        {
            var rockContext = new RockContext();

            var person = GetAuthenticatedPerson(rockContext);
            if (person == null) return Unauthorized();

            try
            {
                return Ok(Encryption.DecryptString(rockContext.Database.SqlQuery<string>($"SELECT Notes FROM [dbo].[_org_newpointe_CustomNotes] cn INNER JOIN PersonAlias pa ON pa.Id = cn.UserId WHERE pa.PersonId = {person.Id} AND MessageId = {id}").FirstOrDefault()));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        public IHttpActionResult Post(CustomNotePost note)
        {
            var rockContext = new RockContext();

            var person = GetAuthenticatedPerson(rockContext);
            if (person == null) return Unauthorized();

            try
            {

                rockContext.Database.ExecuteSqlCommand($"DELETE cn FROM [dbo].[_org_newpointe_CustomNotes] cn INNER JOIN PersonAlias pa ON pa.Id = cn.UserId WHERE pa.PersonId = {person.Id} AND MessageId = {note.MessageId}");
                rockContext.Database.ExecuteSqlCommand($"INSERT INTO [dbo].[_org_newpointe_CustomNotes] VALUES ({person.PrimaryAliasId},{note.MessageId},'{Encryption.EncryptString(note.Notes)}')");

                return Ok("Success");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }


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

        public class CustomNotePost
        {
            public int MessageId { get; set; }
            public string Notes { get; set; }
        }
    }
}
