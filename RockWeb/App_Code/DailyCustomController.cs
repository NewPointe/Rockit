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

[EnableQuery]
public class DailyCustomController : ApiControllerBase
{
    public IHttpActionResult Get() 
    {
        var dailyItems = Test();
        //Initialize();
        return Ok(dailyItems.AsQueryable().OrderByDescending(i=>i));
    }

    public IHttpActionResult Get(int id)
    {
        var dailyItem = GetDailyItem(id);
        //Initialize();
        return Ok(dailyItem);
    }
 



    public TheDailyItem GetDailyItem(int id)
    {
        var rockContext = new RockContext();

        var contentItemService = new ContentChannelItemService(rockContext);
 
        ContentChannelItem i = null;

        var attrService = new AttributeService(rockContext);


        var dailyItem = new  TheDailyItem();

        i = contentItemService.Get(id); 

        if (i != null)
        {
            i.LoadAttributes();

            var attributes = i.Attributes;

            var pdfAttr = i.Attributes["PDF"];

            var binaryFile = new BinaryFileService(new RockContext()).Get(pdfAttr.Id);
            var pdfUrl = binaryFile.Url;

            var scriptureAttr = i.Attributes["ScriptureCards"];

            binaryFile = new BinaryFileService(new RockContext()).Get(scriptureAttr.Id);
            var scriptureUrl = binaryFile.Url;

            dailyItem  = (new TheDailyItem { Id = i.Id, Title = i.Title, Content = i.Content, DailyPDF = pdfUrl, ScriptiureCards = scriptureUrl });

        }

     

        return dailyItem;

    }




    public List<int> Test()
    {
        var rockContext = new RockContext();

        var contentItemService = new ContentChannelService(rockContext);
        var d = new DotLiquid.Context();
        ContentChannel contentItem = null;

        var attrService = new AttributeService(rockContext);


        var dailyItems = new List<int>();


        contentItem = contentItemService.Get(20);
        var items = contentItem.Items.Where(a => a.StartDateTime < DateTime.Now).OrderByDescending(i=>i.Id);

        foreach (var i in contentItem.Items)
        {
            dailyItems.Add(i.Id);
        }

        return dailyItems;

    }
     
}

public class TheDailyItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string DailyPDF { get; set; }
    public string ScriptiureCards { get; set; }
}