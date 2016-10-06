<%@ WebHandler Language="C#" Class="Twilio" %>
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

public class Webhooks : IHttpHandler
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

        RockContext rockContext = new RockContext();

        var body = request.Form["Body"];
        var from = request.Form["From"];
        var city = request.Form["FromCity"];
        var state = request.Form["FromState"];
        var media = request.Form["NumMedia"];
        var mediaAttachment = "";
        int mediaNumber = 0;

        if (Int32.TryParse(media, out mediaNumber))
        {
            if (mediaNumber >= 1)
            {
                mediaAttachment = request.Form["MediaUrl0"];
            }
        }



        var slackClient = new SlackClient("https://hooks.slack.com/services/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");

        var slackMessage = new SlackMessage
        {
            Channel = "#_helpdesk",
            Text = "Helpdesk SMS Received",
            Username = "Chip",
            IconUrl = new Uri("https://newpointe.blob.core.windows.net/newpointe-webassets/upload/ddd8612b1f9a4aef9a7faf6525136191_Rock-Lobster_edited.jpg")
        };

        var slackAttachment = new SlackAttachment
        {
            Fallback = "Helpdesk SMS Received from " + from,
            Color = "#8bc54d",
            ImageUrl = mediaAttachment,
            Fields =
        new List<SlackField>
            {
                    new SlackField
                        {
                            Title = "Message",
                            Value = body,
                            Short = false
                        },
                    new SlackField
                        {
                            Title = "Phone",
                            Value = from + "(" + city + ", " + state + ")",
                            Short = true
                        },
                    new SlackField
                        {
                            Title = "Person",
                            Value = PhoneString(from),
                            Short = true
                        }

            }
        };
        slackMessage.Attachments = new List<SlackAttachment> {slackAttachment};
        slackMessage.Mrkdwn = true;
        slackClient.Post(slackMessage);

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