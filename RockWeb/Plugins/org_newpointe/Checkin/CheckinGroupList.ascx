<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckinGroupList.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Checkin.CheckinGroupList" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">

            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-check-square-o"></i>Check-in Areas</h1>
            </div>
            <div class="panel-body">
                <asp:Literal ID="lWarnings" runat="server" />
                <asp:Literal ID="lContent" runat="server" />

                <asp:HiddenField ID="hfInitialCategoryParentIds" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfSelectedItemId" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hfPageRouteTemplate" runat="server" ClientIDMode="Static" />

                <div class="treeview">

                    <div class="treeview-actions" id="divTreeviewActions" runat="server">
                    </div>
                    <Rock:NotificationBox ID="nbWarning" runat="server" NotificationBoxType="Warning" />
                    <div class="treeview-scroll scroll-container scroll-container-horizontal">

                        <div class="viewport">
                            <div class="overview">
                                <div class="panel-body treeview-frame">
                                    <div id="treeview-content">
                                        <ul class="rocktree" id="TreeviewContentList" runat="server"></ul>
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
                    var deparam = function (querystring) {
                        // remove any preceding url and split
                        querystring = querystring.substring(querystring.indexOf('?') + 1).split('&');
                        var params = {}, pair, d = decodeURIComponent;
                        // march and parse
                        for (var i = querystring.length - 1; i >= 0; i--) {
                            pair = querystring[i].split('=');
                            params[d(pair[0])] = d(pair[1]);
                        }

                        return params;
                    };//--  fn  deparam

                    var scrollbCategory = $('.treeview-scroll');
                    scrollbCategory.tinyscrollbar({ axis: 'x', sizethumb: 60, size: 200 });

                    // resize scrollbar when the window resizes
                    $(document).ready(function () {
                        $(window).on('resize', function () {
                            resizeScrollbar(scrollbCategory);
                        });
                    });

                    // scrollbar hide/show
                    var timerScrollHide;
                    $("[id$='upCategoryTree']").on({
                        mouseenter: function () {
                            clearTimeout(timerScrollHide);
                            $("[id$='upCategoryTree'] div[class~='scrollbar'] div[class='track'").fadeIn('fast');
                        },
                        mouseleave: function () {
                            timerScrollHide = setTimeout(function () {
                                $("[id$='upCategoryTree'] div[class~='scrollbar'] div[class='track'").fadeOut('slow');
                            }, 1000);
                        }
                    });

                    $(function () {
                        var $selectedId = $('#hfSelectedItemId'),
                            $expandedIds = $('#hfInitialCategoryParentIds');
                        $('#treeview-content')
                            .on('rockTree:selected', function (e, id) {

                                var query = deparam(location.search);


                                var urlParameter = 'CategoryId';

                                query["CategoryId"] = id;

                                var currentItemId = $selectedId.val();

                                if (currentItemId !== id) {
                                    // get the data-id values of rock-tree items that are showing visible children (in other words, Expanded Nodes)
                                    var expandedDataIds = $(e.currentTarget).find('.rocktree-children').filter(":visible").closest('.rocktree-item').map(function () {
                                        return $(this).attr('data-id')
                                    }).get().join(',');

                                    var pageRouteTemplate = $('#hfPageRouteTemplate').val();
                                    var locationUrl = "";
                                    var regex = new RegExp("{" + urlParameter + "}", "i");

                                    if (pageRouteTemplate.match(regex)) {
                                        locationUrl = Rock.settings.get('baseUrl') + pageRouteTemplate.replace(regex, id);
                                        query["ExpandedIds"] = encodeURIComponent(expandedDataIds);
                                    }
                                    else {
                                        locationUrl = window.location.href.split('?')[0];
                                        query["ExpandedIds"] = encodeURIComponent(expandedDataIds);
                                    }


                                    window.location = locationUrl + "?" + $.param(query);
                                }

                            })
                                .on('rockTree:rendered', function () {

                                    // update viewport height
                                    resizeScrollbar(scrollbCategory);

                                })
                                .rockTree({
                                    selectedIds: $selectedId.val() ? $selectedId.val().split(',') : null,
                                    expandedIds: $expandedIds.val() ? $expandedIds.val().split(',') : null
                                });
                    });

                    function resizeScrollbar(scrollControl) {
                        var overviewHeight = $(scrollControl).find('.overview').height();

                        $(scrollControl).find('.viewport').height(overviewHeight);

                        scrollControl.tinyscrollbar_update('relative');
                    }


                </script>
            </div>
            --%>
        
        </asp:Panel>


    </ContentTemplate>
</asp:UpdatePanel>
