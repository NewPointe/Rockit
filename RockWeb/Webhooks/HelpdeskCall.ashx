<%@ WebHandler Language="C#" Class="HelpdeskCall" %>
// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
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
using RestSharp;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Slack.Webhooks;
using HttpResponse = System.Web.HttpResponse;

public class HelpdeskCall : IHttpHandler
{
    private HttpRequest request;
    private HttpResponse response;

    public string personImageUrl = "";

    public void ProcessRequest( HttpContext context )
    {
        request = context.Request;
        response = context.Response;

        response.ContentType = "text/plain";

        if ( request.HttpMethod != "POST" )
        {
            response.Write( "Invalid request type." );
            return;
        }


        var from = request.Form["From"];
        var city = request.Form["FromCity"];
        var state = request.Form["FromState"];
        var callerName = request.Form["CallerName"];


        string formattedPhone = from.Substring(2);




        var slackClient = new SlackClient("https://hooks.slack.com/services/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");

        var slackMessage = new SlackMessage
        {
            Channel = "#testing",
            Text = "Incoming Helpdesk Call",
            Username = "Chip",
            IconUrl = new Uri("https://newpointe.blob.core.windows.net/newpointe-webassets/upload/ddd8612b1f9a4aef9a7faf6525136191_Rock-Lobster_edited.jpg")
        };

        var slackAttachment = new SlackAttachment
        {
            Fallback = "Incoming Helpdesk Call from " + from,
            Color = "#af1630",
            Fields =
        new List<SlackField>
            {
                    new SlackField
                        {
                            Title = "Caller ID",
                            Value = callerName,
                            Short = false
                        },
                    new SlackField
                        {
                            Title = "Phone",
                            Value = formattedPhone + " (" + city + ", " + state + ")",
                            Short = true
                        },
                    new SlackField
                        {
                            Title = "Rock Lookup",
                            Value = PhoneString(formattedPhone),
                            Short = true
                        }

            }
        };
        slackMessage.Attachments = new List<SlackAttachment> {slackAttachment};
        slackMessage.Mrkdwn = true;
        slackClient.Post(slackMessage);


        response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n");
        //response.Write(
        //    "<Response>\r\n    <Say>Thank you for calling the NewPointe Community Church Helpline!  Please hold while we connect you.</Say>\r\n    <Dial action=\"helplinequeueISstatus.php\" method=\"GET\" timeout=\"15\">\r\n         <Number>13303652226</Number> \r\n         <Number>13303652224</Number>\r\n         <Number>13303652220</Number>\r\n         <Number>13303652280</Number>\r\n    </Dial>\r\n</Response>");
        response.Write(
            "<Response>\r\n    <Say>Thank you for calling the NewPointe Community Church Helpline!  Please hold while we connect you.</Say>\r\n    <Dial action=\"http://twimlets.com/forward?PhoneNumber=3303652269\" method=\"GET\" timeout=\"15\">\r\n         <Number>13303652226</Number> \r\n       </Dial>\r\n</Response>");
        response.StatusCode = 200;

    }

    private string PhoneString(string fromNumber)
    {
        RockContext rockContext = new RockContext();
        PersonService personService = new PersonService(rockContext);
        List<Person> personList = null;
        string personResults = "";

        try
        {
            var t = personService.GetByPhonePartial(fromNumber);

            if (t != null)
            {
                personList = t.ToList();
            }
            if (personList.Count != null)
            {
                foreach (var p in t)
                {
                    personResults += " <https://rock.newpointe.org/Person/" + p.Id + "|" + p.FullName + ">  ";
                }

            }

        }
        catch (NullReferenceException e)
        {

        }

        return personResults;
    }




    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}