<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PageMenu.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.MobilePageMenu.PageMenu" %>

<style>
            .mobile-page-menu table.gsc-search-box td {
            /*padding-top: 13px;
            vertical-align: top;*/
        }

        .mobile-page-menu .gsc-search-box {
            height: 100px !important;
        }

        .mobile-page-menu table.gsc-search-box tbody tr td * {
            font-size:20px !important;
        }
        .mobile-page-menu table.gsc-search-box tbody tr td table {
            height: 100% !important;
        }
        .mobile-page-menu table.gsc-search-box tbody tr td input {
            border: solid 1px rgb(238, 238, 238) !important;
        }

        .gsc-input-box table {
        }
</style>
<asp:UpdatePanel ID="upContent" runat="server">
    <ContentTemplate>
        <div class="hidden-lg">
            <asp:PlaceHolder ID="phContent" runat="server"></asp:PlaceHolder>
            <script>
                if (!window.gcseInjected) {
                    window.gcseInjected = true;
                    (function () {
                        var cx = '001784362343258229631:fdnsp4tdlfa';
                        var gcse = document.createElement('script');
                        gcse.type = 'text/javascript';
                        gcse.async = true;
                        gcse.src = (document.location.protocol == 'https:' ? 'https:' : 'http:') +
                            '//cse.google.com/cse.js?mob=1&cx=' + cx;
                        var s = document.getElementsByTagName('script')[0];
                        s.parentNode.insertBefore(gcse, s);
                    })();
                }
            </script>
             <gcse:searchbox-only resultsurl="https://newpointe.org/SearchResults"></gcse:searchbox-only>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>



<script>
    $(document).ready(function () {
        setTimeout(showMobileSearchText, 3000);

        function showMobileSearchText() {
            $("#gsc-i-id1").attr("placeholder", "Enter to search");

        }
    })

</script>