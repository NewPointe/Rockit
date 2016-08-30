<%@ WebHandler Language="C#" Class="RockWeb.Webhooks.Attendance" %>
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
using System.Linq;
using Newtonsoft.Json;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;
using Rock.Communication;
using System.Diagnostics;
using System.Globalization;

namespace RockWeb.Webhooks
{

    public class Attendance : IHttpHandler
    {
        private RockContext rockContext = new RockContext();
        private int metricValueId = 0;
        private int metricCategoryId = 2;
        private int metricValueType = 0;

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

            if (request.Form["campus"] != null)
            {
                
                response.Write(request.Form["name"]);

                var formDate = request.Form["date"];
                var formCampus = request.Form["campus"];
                var form6pm = request.Form["6pm"];
                var form7pm = request.Form["7pm"];
                var form9am = request.Form["9am"];
                var form10am = request.Form["10am"];
                var form11am = request.Form["11am"];
                var formFirstName = request.Form["firstname"];
                var formLastName = request.Form["lastname"];
                var formEmail = request.Form["email"];
                var formNotes = "Submitted by Webhook via Machform: " + request.Form["formid"] + " / " + request.Form["entrynumber"] + ". " + request.Form["notes"];
                

                DateTime dt = Convert.ToDateTime(formDate);

                int campusEntityId = 0;

                switch (formCampus)
                {
                    case "Akron":
                        campusEntityId = 6;
                        break;
                    case "Canton":
                        campusEntityId = 2;
                        break;
                    case "Coshocton":
                        campusEntityId = 3;
                        break;
                    case "Dover":
                        campusEntityId = 1;
                        break;
                    case "Millersburg":
                        campusEntityId = 4;
                        break;
                    case "Wooster":
                        campusEntityId = 5;
                        break;
                    case "_Online":
                        campusEntityId = 8;
                        break;
                }



                if (!String.IsNullOrWhiteSpace(form6pm))
                {
                    SaveMetric(dt.AddHours(18), campusEntityId, form6pm, formNotes); 
                }

                if (!String.IsNullOrWhiteSpace(form7pm))
                {
                    SaveMetric(dt.AddHours(19), campusEntityId, form7pm, formNotes); 
                }


                if (!String.IsNullOrWhiteSpace(form9am))
                {
                    SaveMetric(dt.AddHours(9), campusEntityId, form9am, formNotes);
                }

                if (!String.IsNullOrWhiteSpace(form10am))
                {
                    SaveMetric(dt.AddHours(10), campusEntityId, form10am, formNotes);
                }


                if (!String.IsNullOrWhiteSpace(form11am))
                {
                    SaveMetric(dt.AddHours(11), campusEntityId, form11am, formNotes);
                }


                //SendEmail("bwitting@newpointe.org", "bwitting@newpointe.org", "Attendance Submitted for " + formCampus, formNotes, rockContext);
                
                
            }



            response.ContentType = "text/xml";
            response.AddHeader("Content-Type", "text/xml");
            response.StatusCode = 200;

        }


        public void SaveMetric(DateTime dt, int campusEntityId, string attendance, string notes)
        {
            MetricValue metricValue;
            MetricValueService metricValueService = new MetricValueService(rockContext);

            if (metricValueId == 0)
            {
                metricValue = new MetricValue();
                metricValueService.Add(metricValue);
                metricValue.MetricId = metricCategoryId;
                metricValue.Metric = metricValue.Metric ?? new MetricService(rockContext).Get(metricValue.MetricId);
            }
            else
            {
                metricValue = metricValueService.Get(metricValueId);
            }

            metricValue.MetricValueType = (MetricValueType)metricValueType;
            metricValue.XValue = null;
            metricValue.YValue = attendance.AsDecimalOrNull();
            metricValue.Note = notes;
            metricValue.MetricValueDateTime = dt;
            metricValue.EntityId = campusEntityId;

            rockContext.SaveChanges();
            
        }
        

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }






        private void SendEmail(string recipient, string from, string subject, string body, RockContext rockContext)
        {
            var recipients = new List<string>();
            recipients.Add(recipient);

            var mediumData = new Dictionary<string, string>();
            mediumData.Add("From", from);
            mediumData.Add("Subject", subject);
            mediumData.Add("Body", body);

            var mediumEntity = EntityTypeCache.Read(Rock.SystemGuid.EntityType.COMMUNICATION_MEDIUM_EMAIL.AsGuid(), rockContext);
            if (mediumEntity != null)
            {
                var medium = MediumContainer.GetComponent(mediumEntity.Name);
                if (medium != null && medium.IsActive)
                {
                    var transport = medium.Transport;
                    if (transport != null && transport.IsActive)
                    {
                        var appRoot = GlobalAttributesCache.Read(rockContext).GetValue("InternalApplicationRoot");
                        transport.Send(mediumData, recipients, appRoot, string.Empty);
                    }
                }
            }
        }





    }


}