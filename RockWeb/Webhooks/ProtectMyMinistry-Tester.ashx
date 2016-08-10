<%@ WebHandler Language="C#" Class="RockWeb.Webhooks.ProtectMyMinistry" %>
// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//

using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Rock;
using Rock.Data;
using Rock.Model;

namespace RockWeb.Webhooks
{
    /// <summary>
    /// Handles the background check results sent from Protect My Ministry
    /// </summary>
    public class ProtectMyMinistry : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            response.ContentType = "text/plain";

            if (request.HttpMethod != "POST")
            {
                response.Write("Invalid request type.");
                return;
            }

            if (request.Form["REQUEST"] != null)
            {
                //try
                //{
                var rockContext = new Rock.Data.RockContext();

                string logFile = HttpContext.Current.Server.MapPath("~/App_Data/Logs/PMMLog.txt");

                using (System.IO.FileStream fs = new System.IO.FileStream(logFile, System.IO.FileMode.Append, System.IO.FileAccess.Write))
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fs))
                {
                    sw.WriteLine(string.Format("{0} - Recieved Request: {1}", RockDateTime.Now.ToString(), HttpUtility.UrlDecode(request.Form["REQUEST"])));
                }

                //try
                //{
                XDocument xRequest = XDocument.Parse(HttpUtility.UrlDecode(request.Form["REQUEST"]));
                // Return the success XML to PMM
                XDocument xdocResult = null;

                IEnumerable<XElement> orderDets = xRequest.Root.Element("Order").Elements("OrderDetail");

                XElement ssntraceOrder = orderDets.Where(x => x.Attribute("ServiceCode").Value == "SSNTrace").FirstOrDefault();
                if (ssntraceOrder != null)
                {
                    xdocResult = XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
			<OrderXML>
			  <Method>PUSH RESULTS</Method>
			   <Authentication>
			    <Username>PRS000023382</Username>
			    <Password>CH18978</Password>
			  </Authentication>
			  <ReportID>2883348</ReportID>		  
			  <Order>
			  	<BillingReferenceCode>Dover Campus</BillingReferenceCode>
			    <Subject>
			      <FirstName>Tyler</FirstName>
			      <MiddleName>Lane</MiddleName>
			      <LastName>Schrock</LastName>
			      <Suffix></Suffix>
			      <DOB>2/6/1996</DOB>
			      <SSN>295-98-9615</SSN>
			    </Subject>
									    
				<ReportLink><![CDATA[https://services.priorityresearch.com/webservice/getreport.cfm?ReportID=2883348&ReportKey=01EC3B33-1AD1-40A2-8270-3ED8230D2C5C]]></ReportLink>
				
	<OrderDetail ServiceCode=""SSNTrace"" OrderId=""" + ssntraceOrder.Attribute("OrderId").Value + @""" CRAorderId=""6315435"">
		<Status>COMPLETE</Status>
		
        <Result>
    		<AddressCount>3</AddressCount>
			
			<Summary>
				<DeathIndex>No</DeathIndex> <isIssued>Yes</isIssued> <isValid>Yes</isValid> <StateName>Ohio</StateName> <YearIssued>1996</YearIssued> 
			</Summary> 
			<Individual>
	    		<FirstName>TYLER</FirstName> <MiddleName>L</MiddleName> <LastName>SCHROCK</LastName> 				
				<DOB>
					<Month>2</Month> <Day>6</Day> <Year>1996</Year> 
				</DOB>
				<Address>208 E MAIN ST</Address> <City>BALTIC</City> <State>OH</State> <ZipCode>43804</ZipCode> <County>TUSCARAWAS</County> 
				<StartDate>
					<Month>11</Month> 
					<Year>2012</Year> 
				</StartDate>
				
				<EndDate>
					<Month>4</Month> 
					<Year>2016</Year>
				</EndDate>	
									
			</Individual>
	    
			<Individual>
	    		<FirstName>TYLER</FirstName> <MiddleName>L</MiddleName> <LastName>SCHROCK</LastName> 				
				<DOB>
					<Month>2</Month> <Day>6</Day> <Year>1996</Year> 
				</DOB>
				<Address>PO BOX 373</Address> <City>BALTIC</City> <State>OH</State> <ZipCode>43804</ZipCode> <County>HOLMES</County> 
				<StartDate>
					<Month>9</Month> 
					<Year>2014</Year> 
				</StartDate>
				
				<EndDate>
					<Month>12</Month> 
					<Year>2015</Year>
				</EndDate>	
									
			</Individual>
	    
			<Individual>
	    		<FirstName>TYLER</FirstName> <MiddleName>L</MiddleName> <LastName>SCHROCK</LastName> 				
				<DOB>
					<Month>2</Month> <Day>6</Day> <Year>1996</Year> 
				</DOB>
				<Address>208 W MAIN ST</Address> <City>BALTIC</City> <State>OH</State> <ZipCode>43804</ZipCode> <County>COSHOCTON</County> 
				<StartDate>
					<Month>11</Month> 
					<Year>2012</Year> 
				</StartDate>
				
				<EndDate>
					<Month>11</Month> 
					<Year>2012</Year>
				</EndDate>	
									
			</Individual>
	    
    	</Result>
          
	</OrderDetail>
			
			  </Order>
			 </OrderXML>
                            ");
                }

                XElement bgCheckOrder = orderDets.Where(x => x.Attribute("ServiceCode").Value == "CountyCrim" || x.Attribute("ServiceCode").Value == "StateCriminal").FirstOrDefault();
                if (ssntraceOrder == null && bgCheckOrder != null)
                {

                    string doc = @"<?xml version=""1.0"" encoding=""utf-8""?> <OrderXML> <Method>PUSH RESULTS</Method> <Authentication> <Username>PRS000023382</Username> <Password>CH18978</Password> </Authentication> <ReportID>2855190</ReportID>	<Order> <BillingReferenceCode>Dover Campus</BillingReferenceCode>
<Subject> <FirstName>Tyler</FirstName> <MiddleName>Lane</MiddleName> <LastName>Schrock</LastName> <Suffix></Suffix> <DOB>2/6/1996</DOB> <SSN>295-98-9615</SSN> </Subject>
<ReportLink><![CDATA[https://services.priorityresearch.com/webservice/getreport.cfm?ReportID=2855190&ReportKey=B0C8D3A8-1854-49B7-A3FD-04E716A53EE7]]></ReportLink>";

                    IEnumerable<string[]> counties = orderDets.Where(x => x.Attribute("ServiceCode").Value == "CountyCrim").Select(x => new string[] { x.Element("County").Value, x.Element("State").Value });

                    foreach (var county in counties)
                    {
                        doc += @"<OrderDetail ServiceCode=""CountyCrim"" OrderId=""" + bgCheckOrder.Attribute("OrderId").Value + @""" CRAorderId=""6254214""> <Status>NO RECORD</Status> <County>" + county[0] + @"</County> <State>" + county[1] + @"</State> <YearsToSearch>10</YearsToSearch> <RecordsRequested><![CDATA[Felony & Misdemeanor]]></RecordsRequested> </OrderDetail>";
                    }

                    doc += @"<OrderDetail ServiceCode=""combo"" OrderId=""" + bgCheckOrder.Attribute("OrderId").Value + @""" CRAorderId=""6254213""> <Status>NO RECORD</Status> <Result> <OffenderCount>0</OffenderCount> </Result> </OrderDetail> </Order> </OrderXML>";

                    xdocResult = XDocument.Parse(doc);


                }
                response.StatusCode = 200;
                response.ContentType = "text/xml";
                response.AddHeader("Content-Type", "text/xml");
                xdocResult.Save(response.OutputStream);
                //}
                //catch { }
                //}
                //catch (SystemException ex)
                //{
                //    ExceptionLogService.LogException(ex, context);
                //}
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}