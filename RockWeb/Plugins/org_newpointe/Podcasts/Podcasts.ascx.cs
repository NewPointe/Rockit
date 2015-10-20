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
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using Rock.Model;
using System.ComponentModel;

using System.Xml;
using System.Linq;
using System.Xml.Linq;

namespace RockWeb.Plugins.org_newpointe.Podcasts
{
    [DisplayName("Podcasts")]
    [Category("NewPointe.org Web Blocks")]
    [Description("This wil display the top 4 podcast items in the XML file.")]
    public partial class Podcasts : RockBlock
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadXML();
        }


        private void LoadXML()
        {



            var xml = XDocument.Load((Server.MapPath("~/content/assets/podcast.xml")));


            var list = new List<Data>();



            XNamespace ns = "http://www.itunes.com/dtds/podcast-1.0.dtd";
            foreach (var item in xml.Descendants("item"))
            {
                var title = item.Element("title").Value;
                var subtitle = item.Element(ns + "subtitle").Value;
                var author = item.Element(ns + "author").Value;
                var img = item.Element(ns + "image").Attribute("href").Value;
                var file = item.Element("enclosure").Attribute("url").Value;
                var duration = item.Element(ns + "duration").Value;
                var date = item.Element("pubDate").Value;
         
                date = date.Remove(date.LastIndexOf("EST"));
                
                date = DateTime.ParseExact(date, "ddd, dd MMM yyyy hh:mm:ss K", System.Globalization.CultureInfo.InvariantCulture).ToShortDateString();

 
                list.Add(new Data { Title = title, SubTitle = subtitle, Image = img, AudioFile = file, Duration = duration, Date = date });
                if (list.Count == 4)
                {
                    break;
                }
            }

            this.rpt.DataSource = list;
            this.rpt.DataBind();
            

        }
    }

    public class Data
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string AudioFile { get; set; }
        public string Date { get; set; }
        public string Duration { get; set; }
    }
}