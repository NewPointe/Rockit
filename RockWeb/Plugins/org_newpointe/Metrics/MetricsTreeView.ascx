<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/Plugins/org_newpointe/Metrics/MetricsTreeView.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Metrics.MetricsTreeView" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-check-square-o"></i>Check-in Areas</h1>
            </div>
            <div class="panel-body">
                <asp:Literal ID="lWarnings" runalt="server" />
                <asp:Literal ID="lContent" runat="server" />


                <asp:HiddenField ID="hfTreeViewData" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfTreeViewRestUrl" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfTreeViewRestParams" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfTreeViewSelectedId" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfTreeViewExpandedIds" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfPageRouteTemplate" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfSelectedIdQueryName" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfExpandedIdsQueryName" runat="server" ClientIDMode="Static" />

                <div class="treeview">
                    <div class="treeview-actions" id="divTreeviewActions" runat="server">

                        <%--<div class="btn-group">
                    <button type="button" class="btn btn-action btn-xs dropdown-toggle" data-toggle="dropdown">
                        <i class="fa fa-plus-circle"></i> Add Location <span class="fa fa-caret-down"></span>
                    </button>
                    <ul class="dropdown-menu" role="menu">
                        <li><asp:LinkButton ID="lbAddLocationRoot" OnClick="lbAddLocationRoot_Click" Text="Add Top-Level" runat="server"></asp:LinkButton></li>
                        <li><asp:LinkButton ID="lbAddLocationChild" OnClick="lbAddLocationChild_Click" Enabled="false" Text="Add Child To Selected" runat="server"></asp:LinkButton></li>
                    </ul>
                </div>--%>
                    </div>

                    <div class="treeview-scroll scroll-container scroll-container-horizontal">

                        <div class="viewport">
                            <div class="overview">
                                <div class="panel-body treeview-frame">
                                    <div id="treeview-content">
                                        <%--<ul class="rocktree" id="TreeviewContentList" runat="server"></ul>--%>
                                    </div>
                                </div>

                            </div>
                        </div>
                        <div class="scrollbar">
                            <div class="track">
                                <div class="thumb">
                                    <div class="end"></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


                <script type="text/javascript">
                    function deparam(querystring) {
                        // remove any preceding url and split
                        querystring = querystring.substring(querystring.indexOf('?') + 1).split('&');
                        var params = {}, pair, d = decodeURIComponent;
                        // march and parse
                        for (var i = querystring.length - 1; i >= 0; i--) {
                            pair = querystring[i].split('=');
                            params[d(pair[0])] = d(pair[1]);
                        }

                        return params;
                    };

                    $(function () {

                        var $data = $('#hfTreeViewData'),
                            $restUrl = $('#hfTreeViewRestUrl'),
                            $restParams = $('#hfTreeViewRestParams'),
                            $selectedId = $('#hfTreeViewSelectedId'),
                            $expandedIds = $('#hfTreeViewExpandedIds');


                        var scrollbCategory = $('.treeview-scroll');
                        scrollbCategory.tinyscrollbar({ axis: 'x', sizethumb: 60, size: 200 });

                        // resize scrollbar when the window resizes
                        $(document).ready(function () {
                            $(window).on('resize', function () {
                                resizeScrollbar(scrollbCategory);
                            });
                        });

                        $('#treeview-content')
                            .on('rockTree:selected', function (e, id) {

                                var query = deparam(location.search);
                                var selParameter = $('#hfSelectedIdQueryName').val();
                                var expParameter = $('#hfExpandedIdsQueryName').val();


                                query[selParameter] = id;


                                var oldItemId = $selectedId.val();
                                if (oldItemId !== id) {

                                    // get the data-id values of rock-tree items that are showing visible children (in other words, Expanded Nodes)
                                    var expandedDataIds = $(e.currentTarget).find('.rocktree-children').filter(":visible").closest('.rocktree-item').map(function () {
                                        return $(this).attr('data-id')
                                    }).get().join(',');


                                    query[expParameter] = encodeURIComponent(expandedDataIds);


                                    var pageRouteTemplate = $('#hfPageRouteTemplate').val();
                                    var locationUrl = "";
                                    var regex = new RegExp("{" + selParameter + "}", "i");

                                    if (pageRouteTemplate.match(regex)) {
                                        locationUrl = Rock.settings.get('baseUrl') + pageRouteTemplate.replace(regex, id);
                                    }
                                    else {
                                        locationUrl = window.location.href.split('?')[0]
                                    }

                                    window.location = locationUrl + "?" + $.param(query);
                                }
                            })
                            .on('rockTree:rendered', function () {

                                // update viewport height
                                resizeScrollbar(scrollbCategory);

                            })
                            .rockTree({
                                restUrl: $restUrl.val() ? $restUrl.val() : null,
                                restParams: $restParams.val() ? $restParams.val() : null,
                                local: $data.val() ? JSON.parse($data.val()) : null,
                                multiSelect: false,
                                selectedIds: $selectedId.val() ? $selectedId.val().split(',') : null,
                                expandedIds: $expandedIds.val() ? $expandedIds.val().split(',') : null
                            });

                        // Generic recursive utility function to find a node in the tree by its id
                        _findNodeById = function (id, array) {
                            var currentNode,
                                node;

                            if (!array || typeof array.length !== 'number') {
                                return null;
                            }

                            for (var i = 0; i < array.length; i++) {
                                currentNode = array[i];

                                // remove surrounding single quotes from id if they exist
                                var idCompare = id.toString().replace(/(^')|('$)/g, '');

                                if (currentNode.id.toString() === idCompare) {
                                    return currentNode;
                                } else if (currentNode.hasChildren) {
                                    node = _findNodeById(id, currentNode.children || []);

                                    if (node) {
                                        return node;
                                    }
                                }
                            }

                            return null;
                        };


                        var rt = $('#treeview-content').data("rockTree");
                        var ev = $expandedIds.val().split(',');
                        for (i = 0; i < ev.length; ++i) {
                            node = _findNodeById(ev[i], rt.nodes);
                            if (node !== null) {
                                node.isOpen = true;
                            }
                        }
                        rt.render();
                    });

                    function resizeScrollbar(scrollControl) {
                        var overviewHeight = $(scrollControl).find('.overview').height();

                        $(scrollControl).find('.viewport').height(overviewHeight);

                        scrollControl.tinyscrollbar_update('relative');
                    }
                </script>
            </div>

        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
