using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.OData;
using Newtonsoft.Json;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Rest;

namespace org.newpointe.API.Controllers
{
    [EnableQuery]
    public class NPMessagesController : ApiControllerBase
    {
        public const int SeriesContentChannelId = 10;
        public const int MessageContentChannelId = 11;

        private readonly RockContext _rockContext;
        private readonly BinaryFileService _binaryFileService;

        public NPMessagesController()
        {
            _rockContext = new RockContext();
            _binaryFileService = new BinaryFileService(_rockContext);
        }

        public IHttpActionResult GetById(int id)
        {

            var item = new ContentChannelItemService(_rockContext).Get(id);

            if (item != null && item.ContentChannelId == MessageContentChannelId)
            {

                var archiveItem = GetArchiveObjectForMessage(item);

                var series = item.ParentItems.Select(i => i.ContentChannelItem).FirstOrDefault(i => i.ContentChannelId == SeriesContentChannelId);
                if (series != null)
                {
                    series.LoadAttributes();
                    var seriesImageFile = _binaryFileService.Get(series.GetAttributeValue("PodcastImage").AsGuid());
                    if (seriesImageFile != null)
                    {
                        archiveItem.AudioImage = seriesImageFile.Path;
                    }
                }

                return Ok(archiveItem);

            }
            else
            {
                return NotFound();
            }

        }

        public IHttpActionResult Get(int seriesId)
        {

            var series = new ContentChannelItemService(_rockContext).Get(seriesId);

            if (series != null && series.ContentChannelId == 10)
            {

                series.LoadAttributes();
                var seriesImageFile = _binaryFileService.Get(series.GetAttributeValue("PodcastImage").AsGuid());

                return Ok(series.ChildItems
                    .Select(i => i.ChildContentChannelItem)
                    .Where(i => i.ContentChannelId == 11 && i.StartDateTime < DateTime.Now)
                    .OrderBy(i => i.StartDateTime)
                    .Select(item =>
                    {
                        var archiveItem = GetArchiveObjectForMessage(item);

                        if (seriesImageFile != null)
                        {
                            archiveItem.AudioImage = seriesImageFile.Path;
                        }

                        return archiveItem;

                    })
                );
            }
            else
            {
                return NotFound();
            }

        }

        private MessageArchiveItem GetArchiveObjectForMessage(ContentChannelItem message)
        {

            message.LoadAttributes(_rockContext);

            var vimeoLinks = GetVimeoLink(message.GetAttributeValue("VideoId"));
            var messageDate = message.GetAttributeValue("Date").AsDateTime();

            return new MessageArchiveItem()
            {
                Id = message.Id,
                Title = message.Title,
                Date = messageDate?.ToShortDateString(),
                Content = DotLiquid.StandardFilters.StripHtml(message.Content).Replace("\n\n", "\r\n\r\n"),
                Speaker = message.GetAttributeValue("Speaker"),
                SpeakerTitle = message.GetAttributeValue("SpeakerTitle"),
                VimeoLink = vimeoLinks?.Url,
                VimeoImage = vimeoLinks?.Image,
                AudioLink = GetFileUrlOrNull(message, "PodcastAudio"),
                AudioImage = GetFileUrlOrNull(message, "PodcastImage"),
                Notes = GetFileUrlOrNull(message, "MessageNotes"),
                TalkItOver = GetFileUrlOrNull(message, "TalkItOver")
            };

        }

        private string GetFileUrlOrNull(IHasAttributes item, string attributeKey)
        {
            return _binaryFileService.Get(item.GetAttributeValue(attributeKey).AsGuid())?.Path;
        }

        private static VimeoReturnObject GetVimeoLink(string id)
        {
            if (int.TryParse(id, out int intId) && intId > 0)
            {
                try
                {
                    var response = JsonConvert.DeserializeObject<VimeoVideoConfig>(
                        new WebClient().DownloadString(
                            $"https://player.vimeo.com/video/{intId}/config"
                        )
                    );

                    return new VimeoReturnObject()
                    {
                        Url = response.Request.Files.Progressive.Last().Url,
                        Image = response.Video.Thumbs.Image640
                    };
                }
                catch
                {
                    // Ignored
                }
            }
            return null;
        }

        public class VimeoReturnObject
        {
            public string Url { get; set; }
            public string Image { get; set; }
        }

        public class MessageArchiveItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public String Date { get; set; }
            public string Speaker { get; set; }
            public string SpeakerTitle { get; set; }
            public string VimeoLink { get; set; }
            public string VimeoImage { get; set; }
            public string Content { get; set; }
            public string Notes { get; set; }
            public string TalkItOver { get; set; }
            public string AudioLink { get; set; }
            public string AudioImage { get; set; }
        }

        public class VimeoProgressive
        {
            [JsonProperty(PropertyName = "url")]
            public string Url { get; set; }
        }

        public class VimeoFiles
        {
            [JsonProperty(PropertyName = "progressive")]
            public List<VimeoProgressive> Progressive { get; set; }
        }

        public class VimeoRequest
        {
            [JsonProperty(PropertyName = "files")]
            public VimeoFiles Files { get; set; }
        }

        public class VimeoThumbs
        {
            [JsonProperty(PropertyName = "640")]
            public string Image640 { get; set; }
        }

        public class VimeoVideo
        {
            [JsonProperty(PropertyName = "thumbs")]
            public VimeoThumbs Thumbs { get; set; }
        }

        public class VimeoVideoConfig
        {
            [JsonProperty(PropertyName = "request")]
            public VimeoRequest Request { get; set; }
            [JsonProperty(PropertyName = "video")]
            public VimeoVideo Video { get; set; }
        }
    }
}