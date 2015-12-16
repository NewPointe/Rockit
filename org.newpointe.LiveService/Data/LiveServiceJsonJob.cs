using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using Rock;
using Rock.Data;
using Rock.Model;

namespace org.newpointe.LiveService.Data
{

    /// <summary>
    /// Summary description for CustomJob
    /// </summary>
    /// 
    public class LiveServiceJsonJob : IJob
    {
        private const string _baseUrl = "http://live.newpointe.org/api/v1/events/current";
        private DateTime today = DateTime.Today;
        public string LivePlatformUrlJson;
        RockContext rockContext = new RockContext();


        public LiveServiceJsonJob()
        {

        }

        public void Execute(IJobExecutionContext context)
        {
            //Check ChurchOnline Platform API to see if there is a live event
            string livePlatformUrl = "http://live.newpointe.org/api/v1/events/current";

            using (WebClient wc = new WebClient())
            {
                LivePlatformUrlJson = wc.DownloadString(livePlatformUrl);
            }

            dynamic isServiceLive = JsonConvert.DeserializeObject(LivePlatformUrlJson);

            string isLive = isServiceLive.response.item.isLive.ToString();

            // specify which attributeId we want to work with
            //var attributeId = 15762;  //testing
            var attributeId = 16493;  //production

            var attributeValueService = new AttributeValueService(rockContext);

            // specify NULL as the EntityId since this is a GlobalAttribute
            var globalAttributeValue = attributeValueService.GetByAttributeIdAndEntityId(attributeId, null);

            if (globalAttributeValue != null)
            {
                // save updated value to database
                globalAttributeValue.Value = isLive;
                rockContext.SaveChanges();

                // flush the attributeCache for this attribute so it gets reloaded from the database
                Rock.Web.Cache.AttributeCache.Flush(attributeId);

                // flush the GlobalAttributeCache since this attribute is a GlobalAttribute
                Rock.Web.Cache.GlobalAttributesCache.Flush();
            }

        }

    }

}