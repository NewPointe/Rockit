using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;

using Rock;
using Rock.Data;
using Rock.Attribute;


namespace RockWeb.Plugins.org_newpointe.AzureUpload
{

    /// <summary>
    /// Template block for developers to use to start a new block.
    /// </summary>
    [DisplayName("Azure Upload")]
    [Category("NewPointe.org Web Blocks")]
    [Description("Azure Upload.")]
    [TextField("File Type GUID","GUID of the file type to use.",true, "2CF8A379-33BB-49C1-8CBB-DF8B822C3E75", "",1)] 


    public partial class AzureUpload : Rock.Web.UI.RockBlock
    {
        public Guid typeGuid;
        RockContext rockContext = new RockContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            typeGuid = Guid.Parse(GetAttributeValue("FileTypeGUID"));
        }

        protected void fuTemplateBinaryFile_FileUploaded(object sender, EventArgs e)
        {

            var fileId = fuTemplateBinaryFile.BinaryFileId;

            var thePath = rockContext.Database.SqlQuery<string>(@"SELECT TOP 1 Path FROM BinaryFile WHERE ID = @fileId
                ", new SqlParameter("fileId", fileId)).ToList<string>()[0].ToString();

            var theGuid = rockContext.Database.SqlQuery<string>(@"SELECT TOP 1 convert(nvarchar(50),Guid) as Guid FROM BinaryFile WHERE ID = 31977
                ", new SqlParameter("fileId", fileId)).ToList<string>()[0].ToString();


            nbSuccess.Visible = true;
            nbSuccess.Text = "<br />" + thePath + "<br />http://newpointe.org/GetFile.ashx?guid=" + theGuid.ToLower();

        }

    }
}