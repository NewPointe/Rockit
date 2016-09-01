using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Rest;
using Rock.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using Rock.Web.Cache;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

[EnableQuery]
public class MessageArchiveCustomController : ApiControllerBase
{

    public IHttpActionResult Get(int seriesId)
    {

        var rockContext = new RockContext();

        var contentItemService = new ContentChannelService(rockContext);
        var d = new DotLiquid.Context();
        ContentChannel contentItem = null;

        var attrService = new AttributeService(rockContext);


        var dailyItems = new List<MessageArchiveItem>();


        var ids = rockContext.Database.SqlQuery<int>("exec GetMessageArchivesBySeriesId @seriesId", new SqlParameter("seriesId", seriesId)).ToList();


        contentItem = contentItemService.Get(11);


        var items = contentItem.Items.Where(a => ids.Contains(a.Id)).ToList();


        foreach (var item in items)
        {



            item.LoadAttributes(rockContext);

            var link = GetVimeoLink(item.GetAttributeValue("VideoId"));

            var newItem = new MessageArchiveItem();

            newItem.Id = item.Id;
            newItem.Content = item.Content;
            newItem.Date = DateTime.Parse(item.GetAttributeValue("Date")).ToShortDateString();
            newItem.Speaker = item.GetAttributeValue("Speaker");
            newItem.SpeakerTitle = item.GetAttributeValue("SpeakerTitle");
            newItem.Title = item.Title;
            newItem.VimeoLink = link;


            dailyItems.Add(newItem);
        }


        return Ok(dailyItems.AsQueryable());
    }

    private string GetVimeoLink(string id)
    {
        if (id != "")
        {
            var client = new WebClient();

            try
            {
                var reply = client.DownloadString(string.Format("https://player.vimeo.com/video/{0}/config", id));

                var response = JsonConvert.DeserializeObject<RootObject>(reply);

                var count = response.request.files.progressive.Count;

                return response.request.files.progressive[count - 1].url;
            }
            catch
            {
                return "";
            }

        }
        else
        {
            return "";
        }

    }


    public class MessageArchiveItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public String Date { get; set; }
        public string Speaker { get; set; }
        public string SpeakerTitle { get; set; }
        public string VimeoLink { get; set; }
        public string Content { get; set; }
    }

    public class AkfireInterconnect
    {
        public string url { get; set; }
        public string origin { get; set; }
    }

    public class Level3SkyfireGce
    {
        public string url { get; set; }
        public string origin { get; set; }
    }

    public class FastlySkyfire
    {
        public string url { get; set; }
        public string origin { get; set; }
    }

    public class Cdns
    {
        public AkfireInterconnect akfire_interconnect { get; set; }
        public Level3SkyfireGce level3_skyfire_gce { get; set; }
        public FastlySkyfire fastly_skyfire { get; set; }
    }

    public class Stream2
    {
        public string profile { get; set; }
        public string quality { get; set; }
        public string id { get; set; }
        public string fps { get; set; }
    }

    public class Dash
    {
        public string origin { get; set; }
        public string url { get; set; }
        public string cdn { get; set; }
        public Cdns cdns { get; set; }
        public List<Stream2> streams { get; set; }
        public bool separate_av { get; set; }
        public string default_cdn { get; set; }
    }

    public class AkfireInterconnect2
    {
        public string url { get; set; }
        public string origin { get; set; }
    }

    public class Level3SkyfireGce2
    {
        public string url { get; set; }
        public string origin { get; set; }
    }

    public class FastlySkyfire2
    {
        public string url { get; set; }
        public string origin { get; set; }
    }

    public class Cdns2
    {
        public AkfireInterconnect2 akfire_interconnect { get; set; }
        public Level3SkyfireGce2 level3_skyfire_gce { get; set; }
        public FastlySkyfire2 fastly_skyfire { get; set; }
    }

    public class Hls
    {
        public string origin { get; set; }
        public string url { get; set; }
        public string cdn { get; set; }
        public Cdns2 cdns { get; set; }
        public string default_cdn { get; set; }
        public bool separate_av { get; set; }
    }

    public class Progressive
    {
        public string profile { get; set; }
        public string width { get; set; }
        public string mime { get; set; }
        public string fps { get; set; }
        public string url { get; set; }
        public string cdn { get; set; }
        public string quality { get; set; }
        public string id { get; set; }
        public string origin { get; set; }
        public string height { get; set; }
    }

    public class Files
    {
        public Dash dash { get; set; }
        public Hls hls { get; set; }
        public List<Progressive> progressive { get; set; }
    }

    public class ThumbPreview
    {
        public string url { get; set; }
        public string frame_width { get; set; }
        public string height { get; set; }
        public string width { get; set; }
        public string frame_height { get; set; }
        public string frames { get; set; }
        public string columns { get; set; }
    }

    public class Flags
    {
        public string flash_hls { get; set; }
        public string preload_video { get; set; }
        public string webp { get; set; }
        public string autohide_controls { get; set; }
        public string plays { get; set; }
        public string blurr { get; set; }
        public string cedexis { get; set; }
        public string dnt { get; set; }
        public string partials { get; set; }
        public string login { get; set; }
        public string increase_tap_area { get; set; }
    }

    public class Cookie
    {
        public string scaling { get; set; }
        public string volume { get; set; }
        public object quality { get; set; }
        public string hd { get; set; }
        public object captions { get; set; }
    }

    public class Build
    {
        public string player { get; set; }
        public string js { get; set; }
    }

    public class Urls
    {
        public string zeroclip_swf { get; set; }
        public string js { get; set; }
        public string proxy { get; set; }
        public string flideo { get; set; }
        public string moog { get; set; }
        public string comscore_js { get; set; }
        public string blurr { get; set; }
        public string chromeless_css { get; set; }
        public string cedexis { get; set; }
        public string vuid_js { get; set; }
        public string chromeless_js { get; set; }
        public string moog_js { get; set; }
        public string zeroclip_js { get; set; }
        public string css { get; set; }
    }

    public class Request
    {
        public Files files { get; set; }
        public string ga_account { get; set; }
        public ThumbPreview thumb_preview { get; set; }
        public object referrer { get; set; }
        public string cookie_domain { get; set; }
        public string timestamp { get; set; }
        public string fingerprint { get; set; }
        public string expires { get; set; }
        public Flags flags { get; set; }
        public string currency { get; set; }
        public string comscore_id { get; set; }
        public string session { get; set; }
        public Cookie cookie { get; set; }
        public Build build { get; set; }
        public Urls urls { get; set; }
        public string signature { get; set; }
        public string cedexis_cache_ttl { get; set; }
        public string country { get; set; }
    }

    public class Owner
    {
        public string account_type { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string url { get; set; }
        public string img_2x { get; set; }
        public string id { get; set; }
    }

    public class Thumbs
    {
        public string __invalid_name__640 { get; set; }
        public string __invalid_name__960 { get; set; }
        public string __invalid_name__1280 { get; set; }
        public string @base { get; set; }
    }

    public class Video
    {
        public string allow_hd { get; set; }
        public string height { get; set; }
        public Owner owner { get; set; }
        public Thumbs thumbs { get; set; }
        public string duration { get; set; }
        public string id { get; set; }
        public string hd { get; set; }
        public string embed_code { get; set; }
        public string default_to_hd { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string privacy { get; set; }
        public string share_url { get; set; }
        public string width { get; set; }
        public string embed_permission { get; set; }
        public string fps { get; set; }
    }

    public class Build2
    {
        public string player { get; set; }
        public string rpc { get; set; }
    }

    public class Settings
    {
        public string fullscreen { get; set; }
        public string byline { get; set; }
        public string like { get; set; }
        public string playbar { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public string branding { get; set; }
        public string share { get; set; }
        public string scaling { get; set; }
        public string logo { get; set; }
        public string collections { get; set; }
        public string info_on_pause { get; set; }
        public string watch_later { get; set; }
        public string portrait { get; set; }
        public string embed { get; set; }
        public string badge { get; set; }
        public string volume { get; set; }
    }

    public class Email2
    {
        public string text { get; set; }
        public string confirmation { get; set; }
        public string time { get; set; }
    }

    public class Embed
    {
        public string autopause { get; set; }
        public string color { get; set; }
        public string on_site { get; set; }
        public string outro { get; set; }
        public string api { get; set; }
        public string player_id { get; set; }
        public object quality { get; set; }
        public Settings settings { get; set; }
        public string context { get; set; }
        public string time { get; set; }
        public Email2 email { get; set; }
        public string loop { get; set; }
        public string autoplay { get; set; }
    }

    public class User
    {
        public string liked { get; set; }
        public string account_type { get; set; }
        public string progress { get; set; }
        public string owner { get; set; }
        public string watch_later { get; set; }
        public string logged_in { get; set; }
        public string id { get; set; }
        public string mod { get; set; }
    }

    public class RootObject
    {
        public string cdn_url { get; set; }
        public string view { get; set; }
        public Request request { get; set; }
        public string player_url { get; set; }
        public Video video { get; set; }
        public Build2 build { get; set; }
        public Embed embed { get; set; }
        public string vimeo_url { get; set; }
        public User user { get; set; }
    }
}
