using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using Microsoft.Ajax.Utilities;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;



namespace RockWeb.Plugins.org_newpointe.MachFormEmbed
{

    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName("Embed MachForm")]
    [Category("NewPointe.org Web Blocks")]
    [Description("Embed a MachForm.")]
    [TextField("Machform Server Root","Address of the MachForm Server",true,"https://forms.newpointe.org","",1)]
    [IntegerField("Form ID","The ID of the form.",true,18093,"",2)]
    [IntegerField("Height", "The height of the form.", true, 595, "", 3)]
    [IntegerField("Name Element Id","Element that contains the person's name.", false,1,"",4)]
    [IntegerField("Email Element Id", "Element that contains the person's email address.", false, 1, "", 5)]
    [KeyValueListField("Passed Paramaters","The Element ID is the ID of the form element on this MachForm.  The URL Paramater is the Paramater to look for the the Query String.",false,"","ElementId","URL Paramater","","","",6)]


    public partial class MachFormEmbed : Rock.Web.UI.RockBlock

    {
        public string ServerRoot;
        public string FormID;
        public string Height;
        public string Paramaters;
        public string MFParamaters;
        public string FormScript;
        public string NameElement;
        public string EmailElement;

    protected void Page_Load(object sender, EventArgs e)
    {
        //Get Attribute Values
        ServerRoot = GetAttributeValue("MachformServerRoot");
        FormID = GetAttributeValue("FormID");
        Height = GetAttributeValue("Height");
        NameElement = GetAttributeValue("NameElementId");
        EmailElement = GetAttributeValue("EmailElementId");

        //Set URL Paramaters
        if (!GetAttributeValue("PassedParamaters").IsNullOrWhiteSpace())
        {
            Paramaters = GetAttributeValue("PassedParamaters").TrimEnd('|');
            string[] mfParams = Paramaters.Split('|');
            var mfParamsLength = mfParams.Length ;
            for (int i = 0; i < mfParamsLength; i++)
            {
                string[] moreMfParams = mfParams[i].Split('^');
                var element = moreMfParams[0];
                var theParam = moreMfParams[1] ?? "";
                var URLparam = PageParameter(theParam);
                MFParamaters += "&element_" + element + "=" + URLparam;
            }
        }

        //Insert the logged in person's name if they are logged in and a name element is defined.
            
                var currentPerson = CurrentPerson;
                if (currentPerson != null)
                {
                    if (!NameElement.IsNullOrWhiteSpace())
                    {
                        MFParamaters += "&element_" + NameElement + "_1=" + currentPerson.NickName;
                        MFParamaters += "&element_" + NameElement + "_2=" + currentPerson.LastName;
                    }
                if (!EmailElement.IsNullOrWhiteSpace())
                {
                    MFParamaters += "&element_" + EmailElement + "=" + currentPerson.Email;
                }

            }


        //Generate Form Javascript
                FormScript = String.Format(@"<script type='text/javascript'>
            var __machform_url = '{0}/embed.php?id={1}{2}';
            var __machform_height = {3};
            </script>
            <div id='mf_placeholder'></div>
            <script type='text/javascript' src='https://forms.newpointe.org/js/jquery.ba-postmessage.min.js'></script>
            <script type='text/javascript' src='https://forms.newpointe.org/js/machform_loader.js'></script>", ServerRoot, FormID, MFParamaters, Height);
    }



 

}
}