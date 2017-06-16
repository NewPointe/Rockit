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
    public class NPMessageSeriesController : ApiControllerBase
    {
        public const int SeriesContentChannelId = 10;
        public const int MessageContentChannelId = 11;

        private readonly RockContext _rockContext;
        private readonly BinaryFileService _binaryFileService;
        private readonly DefinedValueService _definedValueService;

        public NPMessageSeriesController()
        {
            _rockContext = new RockContext();
            _binaryFileService = new BinaryFileService(_rockContext);
            _definedValueService = new DefinedValueService(_rockContext);
        }

        public class MessageSeriesDetailsShort
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string SmallTileImageUrl { get; set; }
        }

        public class MessageSeriesDetailsFull : MessageSeriesDetailsShort
        {
            public string LargeTileImageUrl { get; set; }
            public string BannerImageUrl { get; set; }
            public string[] Topics { get; set; }
        }

        public IHttpActionResult Get()
        {
            return Ok(new ContentChannelItemService(_rockContext).Queryable()
                .Where(i => i.ContentChannelId == SeriesContentChannelId && i.StartDateTime < DateTime.Now)
                .OrderByDescending(i => i.StartDateTime)
                .Take(100)
                .ToList()
                .Select(i => new MessageSeriesDetailsShort()
                {
                    Id = i.Id,
                    Title = i.Title,
                    SmallTileImageUrl = GetFileUrlOrNull(i, "OptimizedSeriesArchiveImage")
                }));
        }

        public IHttpActionResult GetById(int id)
        {

            var series = new ContentChannelItemService(_rockContext).Get(id);

            if (series != null
                && series.ContentChannelId == SeriesContentChannelId
                && series.StartDateTime < DateTime.Now)
            {
                return Ok(new MessageSeriesDetailsFull()
                {
                    Id = series.Id,
                    Title = series.Title,
                    SmallTileImageUrl = GetFileUrlOrNull(series, "OptimizedSeriesArchiveImage"),
                    LargeTileImageUrl = GetFileUrlOrNull(series, "SermonGraphic"),
                    BannerImageUrl = GetFileUrlOrNull(series, "HeaderGraphic"),
                    Topics = _definedValueService.GetByGuids(series.GetAttributeValues("Topic").Select(t => t.AsGuid()).ToList()).Select(dv => dv.Value).ToArray()
                });
            }
            else
            {
                return NotFound();
            }
        }
        
        private string GetFileUrlOrNull(IHasAttributes item, string attributeKey)
        {
            item.LoadAttributes();

            return _binaryFileService.Get(item.GetAttributeValue(attributeKey).AsGuid())?.Path;
        }
    }
}