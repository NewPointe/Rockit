<%@ WebHandler Language="C#" Class="RockWeb.Webhooks.LiveService" %>
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

    public class LiveService : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            RockContext rockContext = new RockContext();
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            response.ContentType = "text/plain";

            //if (request.HttpMethod != "POST" || request.HttpMethod != "GET")
            //{
            //    response.Write("Invalid request type.");
            //    return;
            //}


            // specify which attribute key we want to work with
            var attributeKey = "LiveService";  //production

            var attributeValueService = new AttributeValueService(rockContext);

            // specify NULL as the EntityId since this is a GlobalAttribute
            var globalAttributeValue = attributeValueService.GetGlobalAttributeValue(attributeKey);

            response.Write(globalAttributeValue);
            response.StatusCode = 200;
            

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