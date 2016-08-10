using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Rock.Web.UI
{
    /// <summary>
    /// Summary description for RockPageMod
    /// </summary>
    public class RockPageMod : Rock.Web.UI.RockPage
    {
        protected override void SavePageStateToPersistenceMedium(object state)
        {

            string file = GenerateFileName(true);

            FileStream filestream = new FileStream(file, FileMode.Create);

            LosFormatter formator = new LosFormatter();

            formator.Serialize(filestream, state);

            filestream.Flush();

            filestream.Close();

            filestream = null;

        }

        protected override object LoadPageStateFromPersistenceMedium()
        {

            object state = null;

            StreamReader reader = new StreamReader(GenerateFileName());

            LosFormatter formator = new LosFormatter();

            state = formator.Deserialize(reader);

            reader.Close();

            return state;

        }

        Guid ViewStateGuid = Guid.Empty;

        private string GenerateFileName(bool save = false)
        {
            if (IsPostBack)
            {
                String VSGuid = Request.Form["__VIEWSTATEGUID"];
                Guid.TryParse(VSGuid, out ViewStateGuid);
            }
            if (ViewStateGuid.IsEmpty())
            {
                ViewStateGuid = Guid.NewGuid();
            }

            if (save)
            {
                ScriptManager.RegisterHiddenField(this, "__VIEWSTATEGUID", ViewStateGuid.ToString());
            }

            string file = Session.SessionID.ToString() + "." + ViewStateGuid.ToString() + ".viewstate";

            file = Path.Combine(Server.MapPath("~/ViewStateFiles") + "/" + file);

            return file;

        }

        private Control FindControlRecursive(Control root, string id, int maxDepth = -2)
        {
            if (maxDepth != -1)
            {
                if (root.ID == id)
                {
                    return root;
                }
                foreach (Control c in root.Controls)
                {
                    Control t = FindControlRecursive(c, id, maxDepth - 1);
                    if (t != null)
                    {
                        return t;
                    }
                }
            }
            return null;
        }
    }
}