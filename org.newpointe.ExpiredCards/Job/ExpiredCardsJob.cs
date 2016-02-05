using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Quartz;
using RestSharp;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;

namespace org.newpointe.LiveService.Data
{

    /// <summary>
    /// Summary description for CustomJob
    /// </summary>
    /// 
    [UrlLinkField("ChurchOnline API Address","The URL to check for the ChurchOnline API",true, "http://live.newpointe.org/api/v1/events/current","General",1,"Address")]
    public class LiveServiceJsonJob : IJob
    {
        public string LivePlatformUrlJson;
        RockContext rockContext = new RockContext();
        
        public LiveServiceJsonJob()
        {

        }

        public void Execute(IJobExecutionContext context)
        {

            var rockContext = new RockContext();
            var financialPaymentDetailService = new FinancialPaymentDetailService(rockContext);


            var fff = financialPaymentDetailService.Queryable();

            var q = financialPaymentDetailService.ExecuteQuery("SELECT * FROM [FinancialPaymentDetail] WHERE ExpirationMonthEncrypted IS NOT NULL ORDER BY ID DESC").ToList();

            var count = q.Count;



        }

    }

}