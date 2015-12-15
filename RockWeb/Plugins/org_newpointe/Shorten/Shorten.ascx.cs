using Rock.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock.Attribute;
using Rock.Data;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Model;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using Rock;

[DisplayName("Page Redirect")]
[Category("NewPointe.org Web Blocks")]
[Description("Create a page for friendly URL and set this block to redirect to the correct message or event page")]
[TextField("ID", "Enter Series ID", required: true)]
public partial class Plugins_org_newpointe_PageRedirect_Shorten : Rock.Web.UI.RockBlock
{
    protected void Page_Load(object sender, EventArgs e)
    {

        var isAdmin = Request.QueryString["isAdmin"];
        var type = GetAttributeValue("ChooseType");
        var id = GetAttributeValue("ID");
        if (isAdmin != "true")
        {


            if (!string.IsNullOrEmpty(id))
            {
                Response.Redirect("/message-archive/message?SeriesID=" + id);
            }
        }

    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        string shortenedUrl = GetShortUrl("https://newpointe.org/GiveNow?q=2");
        tbTest.Text = shortenedUrl;
    }


    //Shorten the give URL
    string GetShortUrl(string url)
    {
        //Generate a random Short URL from a GUID
        string randomShortUrl = Guid.NewGuid().ToString("N").Substring(0, 8);

        //Construct the URL for the request
        string theUrl = String.Format("http://npgive.org/yourls-api.php?signature=05e2685fc7&action=shorturl&url={0}&keyword={1}2&format=simple",url, randomShortUrl);

        //GET request to API
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(theUrl);
        try
        {
            WebResponse response = request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                return reader.ReadToEnd();
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