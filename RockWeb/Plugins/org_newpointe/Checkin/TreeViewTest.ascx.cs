using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Entity;
using System.Text;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;
using Rock.Attribute;

namespace RockWeb.Plugins.org_newpointe.Checkin
{
    /// <summary>
    /// Template block for a TreeView.
    /// </summary>
    [DisplayName("Tree View Test")]
    [Category("Newpointe Check-in")]
    [Description("A test Tree View.")]
    public partial class TreeViewTest : Rock.Web.UI.RockBlock
    {

        protected string SelectedIdQueryName = "SelectedId";
        protected string ExpandedIdsQueryName = "ExpandedIds";
        protected string RestUrl;
        protected string RestParms;
        protected StringBuilder LocalData = new StringBuilder();
        RockContext _rockContext = null;


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _rockContext = new RockContext();
        }

        protected override void OnLoad(EventArgs e)
        {

            if (!Page.IsPostBack)
            {


                Guid configurationTemplateGuid = Guid.Empty;
                if (Request["Area"] != null)
                {
                    var groupType = Rock.Web.Cache.GroupTypeCache.Read(Request["Area"]);
                    configurationTemplateGuid = groupType.Guid;
                    //RestParms = string.Format("?GroupTypeId={0}", Request["Area"]);
                }
                else if (Request["GroupTypeId"] != null)
                {
                    int gtype = Int32.Parse(Request["GroupTypeId"]);
                    var groupType = Rock.Web.Cache.GroupTypeCache.Read(gtype);
                    configurationTemplateGuid = groupType.Guid;
                    //RestParms = string.Format("?GroupTypeId={0}", gtype);
                }
                else if (Request["GroupId"] != null)
                {
                    // get the root group type of this group
                    int groupId = Int32.Parse(Request["GroupId"]);

                    var rootGroupType = GetRootGroupType(groupId);
                    if (rootGroupType != null)
                    {
                        configurationTemplateGuid = rootGroupType.Guid;
                    }
                    //RestParms = string.Format("?GroupTypeId={0}", groupId);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(GetAttributeValue("Check-inType")))
                    {
                        configurationTemplateGuid = new Guid(GetAttributeValue("Check-inType"));
                    }
                }

                if (configurationTemplateGuid == Guid.Empty)
                {
                    lWarnings.Text = "<div class='alert alert-warning'>No check-in configuration template configured.</div>";
                }
                else
                {
                    LocalData.Append("[");
                    BuildHeirarchy2(configurationTemplateGuid);
                    LocalData.Remove(LocalData.Length - 1, 1);
                    LocalData.Append("]");
                    //TreeviewContentList.InnerHtml = LocalData.ToString();
                }

                hfTreeViewData.Value = LocalData.Length > 0 ? LocalData.ToString() : null ;
                hfTreeViewRestUrl.Value = RestUrl;
                hfTreeViewRestParams.Value = RestParms;
                hfTreeViewSelectedId.Value = Request[SelectedIdQueryName];
                hfTreeViewExpandedIds.Value = Server.UrlDecode(Request[ExpandedIdsQueryName]);
                hfPageRouteTemplate.Value = null;
                hfSelectedIdQueryName.Value = SelectedIdQueryName;
                hfExpandedIdsQueryName.Value = ExpandedIdsQueryName;

            }

        }

        private int depth = 0;
        private void BuildHeirarchy2(Guid parentGroupTypeGuid, int id = -1)
        {
            GroupTypeService groupTypeService = new GroupTypeService(_rockContext);

            var groupTypes = groupTypeService.Queryable("Groups, ChildGroupTypes").AsNoTracking()
                            .Where(t => t.ParentGroupTypes.Select(p => p.Guid).Contains(parentGroupTypeGuid) && t.Guid != parentGroupTypeGuid).ToList();

            foreach (var groupType in groupTypes)
            {
                ++depth;
                if (groupType.GroupTypePurposeValueId == null || groupType.Groups.Count > 0)
                {
                    //LocalData.Append(string.Format("<li class=\"rocktree-item rocktree-folder\" data-id=\"{0}\"><i class=\"rocktree-icon fa fa-fw fa-caret-down\"></i><span class=\"rocktree-name\"> {1}</span><ul class=\"rocktree-children\">", groupType.Id, groupType.Name));
                    if (groupType.Groups.Count > 0 || groupType.ChildGroupTypes.Count > 0)
                    {
                        LocalData.Append("{" + string.Format("\"Guid\": \"{0}\", \"Name\": \"{1}\", \"ParentId\": \"{2}\", \"HasChildren\": true, \"Children\": [", groupType.Id, groupType.Name, id));
                    }
                    else
                    {
                        LocalData.Append("{" + string.Format("\"Guid\": \"{0}\", \"Name\": \"{1}\", \"ParentId\": \"{2}\", \"HasChildren\": false", groupType.Id, groupType.Name, id) + "},");
                    }
                    if (groupType.ChildGroupTypes.Count > 0)
                    {
                        BuildHeirarchy2(groupType.Guid, groupType.Id);
                    }
                    foreach (var group in groupType.Groups)
                    {
                        ++depth;
                        LocalData.Append("{" + string.Format("\"Id\": \"_{0}\", \"Name\": \"{1}\", \"ParentId\": \"{2}\", \"HasChildren\": false", group.Id, group.Name, groupType.Id) + "},");
                        //LocalData.Append(string.Format("<li class=\"rocktree-item rocktree-leaf\" data-id=\"{0}\"><span class=\"rocktree-name\"> {1}</span></li>", group.Id, group.Name));
                        --depth;
                    }
                    if (groupType.Groups.Count > 0 || groupType.ChildGroupTypes.Count > 0)
                    {
                        LocalData.Remove(LocalData.Length - 1, 1);
                        LocalData.Append("]},");
                    }
                }
                else
                {
                    LocalData.Append("{" + string.Format("\"Guid\": \"{0}\", \"Name\": \"{1}\", \"ParentId\": \"{2}\", \"HasChildren\": true, \"Children\": [", groupType.Id, groupType.Name, id));
                    BuildHeirarchy2(groupType.Guid, groupType.Id);
                    LocalData.Remove(LocalData.Length - 1, 1);
                    LocalData.Append("]},");
                }
                --depth;
            }

        }



        private GroupType GetRootGroupType(int groupId)
        {
            List<int> parentRecursionHistory = new List<int>();
            GroupTypeService groupTypeService = new GroupTypeService(_rockContext);
            var groupType = groupTypeService.Queryable().AsNoTracking().Include(t => t.ParentGroupTypes).Where(t => t.Groups.Select(g => g.Id).Contains(groupId)).FirstOrDefault();

            while (groupType != null && groupType.ParentGroupTypes.Count != 0)
            {
                if (parentRecursionHistory.Contains(groupType.Id))
                {
                    var exception = new Exception("Infinite Recursion detected in GetRootGroupType for groupId: " + groupId.ToString());
                    LogException(exception);
                    return null;
                }
                else
                {
                    var parentGroupType = GetParentGroupType(groupType);
                    if (parentGroupType != null && parentGroupType.Id == groupType.Id)
                    {
                        // the group type's parent is itself
                        return groupType;
                    }

                    groupType = parentGroupType;
                }

                parentRecursionHistory.Add(groupType.Id);
            }

            return groupType;
        }

        private GroupType GetParentGroupType(GroupType groupType)
        {
            GroupTypeService groupTypeService = new GroupTypeService(_rockContext);
            return groupTypeService.Queryable()
                                .Include(t => t.ParentGroupTypes)
                                .AsNoTracking()
                                .Where(t => t.ChildGroupTypes.Select(p => p.Id).Contains(groupType.Id)).FirstOrDefault();
        }
    }
}