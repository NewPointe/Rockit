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

            var newItem = new MessageArchiveItem();

            newItem.Id = item.Id;
            newItem.Content = item.Content;
            newItem.Date = DateTime.Parse(item.GetAttributeValue("Date")).ToShortDateString();
            newItem.Speaker = item.GetAttributeValue("Speaker");
            newItem.SpeakerTitle = item.GetAttributeValue("SpeakerTitle");
            newItem.Title = item.Title;
            newItem.VimeoId = item.GetAttributeValue("VideoId");


            dailyItems.Add(newItem);
        }


        return Ok(dailyItems.AsQueryable());
    }
  
}
 

public class MessageArchiveItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public String Date { get; set; }
    public string Speaker { get; set; }
    public string SpeakerTitle { get; set; }
    public string VimeoId { get; set; }
    public string Content { get; set; }
}