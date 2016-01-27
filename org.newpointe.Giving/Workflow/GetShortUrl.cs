
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;
using Rock.Web.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace org.newpointe.Giving.Workflow
{
    [Description("Gets the Short URL.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Get Short URL")]

    [WorkflowAttribute("Attribute", "The workflow attribute to store the short URL in.", true, "", "", 0, null, new string[] { "Rock.Field.Types.TextFieldType" })]
    [WorkflowTextOrAttribute("URL", "URL", "Workflow attribute that contains the url to shorten.", true, "", "", 1, "URL", new string[] { "Rock.Field.Types.TextFieldType" })]
    public class GetShortUrl : ActionComponent
    {

        public override bool Execute(RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages)
        {

            errorMessages = new List<string>();

            Guid guid = GetAttributeValue(action, "Attribute").AsGuid();
            if (!guid.IsEmpty())
            {
                var attribute = AttributeCache.Read(guid, rockContext);
                if (attribute != null)
                {
                    string value = GetAttributeValue(action, "URL");
                    guid = value.AsGuid();
                    if (guid.IsEmpty())
                    {
                        value = value.ResolveMergeFields(GetMergeFields(action));
                    }
                    else
                    {
                        var attributeValue = action.GetWorklowAttributeValue(guid);

                        if (attributeValue != null)
                        {
                            value = attributeValue;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(value) && IsUrl(value))
                    {
                        var url = GetShortUrlFromString(value);

                        if (attribute.EntityTypeId == new Rock.Model.Workflow().TypeId)
                        {
                            action.Activity.Workflow.SetAttributeValue(attribute.Key, url);
                            action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, url));
                        }
                        else if (attribute.EntityTypeId == new Rock.Model.WorkflowActivity().TypeId)
                        {
                            action.Activity.SetAttributeValue(attribute.Key, url);
                            action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, url));
                        }

                    }
                    else
                    {
                        action.AddLogEntry(string.Format("Error: '{0}' is not a valid url.", value));
                    }

                }
            }

            return true;
        }

        public static bool IsUrl(string str)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(str, UriKind.Absolute, out uriResult);
            return result;
        }

        //Shorten the give URL
        public static string GetShortUrlFromString(string url, int rec = 0)
        {
            if (string.IsNullOrWhiteSpace(url) && !IsUrl(url))
            {
                //throw new ArgumentException(String.Format("'{0}' is not a valid url.", url));
                return url;
            }

            //Generate a random Short URL from a GUID
            string randomShortUrl = Guid.NewGuid().ToString("N").Substring(0, 8).ToLower();

            //Construct the URL for the request
            string theUrl = String.Format("https://npgive.org/yourls-api.php?signature=05e2685fc7&action=shorturl&url={0}&keyword={1}&format=simple", Uri.EscapeDataString(url), randomShortUrl);

            //GET request to API
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(theUrl);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    String responseStr = reader.ReadToEnd();
                    if (string.IsNullOrWhiteSpace(responseStr))
                    {
                        if (rec < 4)
                        {
                            return GetShortUrlFromString(url, rec++);
                        }
                        else
                        {
                            //throw new ArgumentException(String.Format("Could not get short url for '{0}'", url));
                            return url;
                        }
                    }
                    else
                    {
                        return responseStr;
                    }
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();

                }
                throw;
            }
        }


    }
}