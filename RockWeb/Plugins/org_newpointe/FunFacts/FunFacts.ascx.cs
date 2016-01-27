using Rock.Web.UI;
using System;
using System.ComponentModel;


using Rock;
using Rock.SystemGuid;


namespace RockWeb.Plugins.org_newpointe.FunFacts
{
    [DisplayName("FunFacts")]
    [Category("Newpointe.org Web Blocks")]
    [Description("This block will display a fun Rock Fact")]

    public partial class FunFacts : RockBlock
    {



        public string RockFunFacts = Rock.Web.Cache.GlobalAttributesCache.Value("RockFunFacts");
        public string TheFactTitle = "";
        public string TheFact = "";


        protected void Page_Load(object sender, EventArgs e)
        {
            string v1 = "";
            string v2 = "";

            string[] mfParams = RockFunFacts.TrimEnd('|').Split('|');
            var mfParamsLength = mfParams.Length;


            var test = mfParams[new Random().Next(0, mfParams.Length)];

            string[] theFact = test.Split('^');

            TheFactTitle = theFact[0];
            TheFact = theFact[1];


        }

   

        }
        
}