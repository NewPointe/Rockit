using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Rest;

namespace org.newpointe.API.Controllers
{
    [EnableQuery]
    public class NPTheDailyController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var dailyChannel = new ContentChannelService(new RockContext()).Get(20);
            if (dailyChannel == null) return NotFound();

            return Ok(dailyChannel
                    .Items
                    .Where(a => a.StartDateTime < DateTime.Now)
                    .OrderByDescending(i => i.StartDateTime )
                    .Take(365)
                    .Select(i => i.Id)
            );
        }

        public IHttpActionResult GetById(int id)
        {
            var rockContext = new RockContext();

            var item = new ContentChannelItemService(rockContext).Get(id);
            if (item == null) return NotFound();

            item.LoadAttributes();

            var binaryFileService = new BinaryFileService(rockContext);

            return Ok(new TheDailyItem
            {
                Id = item.Id,
                Title = item.Title,
                Date = item.StartDateTime,
                Content = item.Content, //DotLiquid.StandardFilters.StripHtml(item.Content.Replace("<br", "\n<br").Replace("<p>", "\n<p>")),
                DailyPDF = GetFileUrlOrNull(binaryFileService, item, "PDF"),
                ScriptiureCards = GetFileUrlOrNull(binaryFileService, item, "ScriptureCards")
            });
        }

        private static string GetFileUrlOrNull(BinaryFileService binaryFileService, IHasAttributes item,
            string attributeKey)
        {
            return binaryFileService.Get(item.GetAttributeValue(attributeKey).AsGuid())?.Path;
        }

        public class TheDailyItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public DateTime? Date { get; set; }
            public string Content { get; set; }
            public string DailyPDF { get; set; }
            public string ScriptiureCards { get; set; }
        }
    }
}
