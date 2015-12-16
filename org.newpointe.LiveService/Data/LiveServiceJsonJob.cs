using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using Rock.Data;

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
            //
            // TODO: Add constructor logic here
            //

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

            rockContext.Database.ExecuteSqlCommand("UPDATE [rock-production].[dbo].[AttributeValue] SET [Value] = @value WHERE AttributeId = 16493;", new SqlParameter("@value", isLive));
            Rock.Web.Cache.GlobalAttributesCache.Flush();


        }




    }


}