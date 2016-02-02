<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddFamily.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Checkin.AddFamily" %>

<asp:UpdatePanel ID="upAddFamily" runat="server">
    <ContentTemplate>

        <Rock:RockLiteral ID="dbgTxt" runat="server" Visible="false"></Rock:RockLiteral>

        <style>
            .hide-role table tbody td:nth-child(1), .hide-role table thead th:nth-child(1),
            .hide-title table tbody td:nth-child(2), .hide-title table thead th:nth-child(2),
            .hide-suffix table tbody td:nth-child(4), .hide-suffix table thead th:nth-child(4),
            .hide-conStatus table tbody td:nth-child(5), .hide-conStatus table thead th:nth-child(5),
            .hide-email table tbody td:nth-child(5), .hide-email table thead th:nth-child(5) {
                display: none;
            }

            .hide-role table tbody td:nth-child(6),
            .hide-title table tbody td:nth-child(6),
            .hide-suffix table tbody td:nth-child(6),
            .hide-conStatus table tbody td:nth-child(6),
            .hide-role table tbody td:nth-child(9),
            .hide-title table tbody td:nth-child(9),
            .hide-suffix table tbody td:nth-child(9),
            .hide-conStatus table tbody td:nth-child(9) {
                width: 90px;
            }

            .hide-role table tbody td:nth-child(7),
            .hide-title table tbody td:nth-child(7),
            .hide-suffix table tbody td:nth-child(7),
            .hide-conStatus table tbody td:nth-child(7) {
                width: 25%;
            }

                .hide-role table tbody td:nth-child(7) > .js-date-picker,
                .hide-title table tbody td:nth-child(7) > .js-date-picker,
                .hide-suffix table tbody td:nth-child(7) > .js-date-picker,
                .hide-conStatus table tbody td:nth-child(7) > .js-date-picker {
                    width: 100%;
                }

            .hide-role table tfoot td:nth-child(1),
            .hide-title table tfoot td:nth-child(1),
            .hide-suffix table tfoot td:nth-child(1),
            .hide-conStatus table tfoot td:nth-child(1) {
                /*display: none;*/
            }

            .hide-role table tfoot td:nth-child(2),
            .hide-title table tfoot td:nth-child(2),
            .hide-suffix table tfoot td:nth-child(2),
            .hide-conStatus table tfoot td:nth-child(2) {
                width: 50px;
            }

            .panel-body > div:nth-child(1) {
                margin: -15px -15px 15px;
            }

            .actions .btn-primary {
                float: right;
            }


            .hide-role table thead tr .hide-title table thead tr,
            .hide-suffix table thead tr,
            .hide-conStatus table thead tr {
                background-color: #edeae6;
                font-size: 14px;
                color: #6a6a6a;
                font-weight: 600;
                border-bottom: 2px solid #ddd;
                border-color: #d8d1c8;
                vertical-align: bottom;
            }
        </style>

        <div class="panel panel-block">
            <div class="panel-heading">
                <h1 class="panel-title">
                    <asp:Literal ID="lTitle" runat="server"></asp:Literal></h1>
            </div>
            <div class="panel-body">


                <asp:Panel ID="pnlFamilyData" runat="server">

                    <%--<div class="row">
                        <div class="col-md-12">
                            <h4>Family Members</h4>
                        </div>
                    </div>--%>

                    <%--<div class="row">--%>
                    <div class="<%--col-md-12--%>" id="nfmMemberDiv" runat="server">
                        <Rock:NewFamilyMembers ID="nfmMembers" runat="server" OnAddFamilyMemberClick="nfmMembers_AddFamilyMemberClick" />
                    </div>
                    <%--</div>--%>

                    <div class="clearfix">
                        <div class="col-md-4">
                            <Rock:CampusPicker ID="cpCampus" runat="server" Required="true" />
                            <Rock:RockDropDownList ID="ddlMaritalStatus" runat="server" Label="Marital Status of Adults"
                                Help="The marital status to use for the adults in this family." />
                        </div>

                        <div class="col-md-8">
                            <Rock:AddressControl ID="acAddress" runat="server" UseStateAbbreviation="false" UseCountryAbbreviation="false" />
                        </div>
                    </div>

                </asp:Panel>

                <asp:Panel ID="pnlContactInfo" runat="server" Visible="false">
                    <Rock:NewFamilyContactInfo ID="nfciContactInfo" runat="server" />
                </asp:Panel>

                <asp:Panel ID="pnlAttributes" runat="server" Visible="false">
                </asp:Panel>

                <asp:Panel ID="pnlDuplicateWarning" runat="server" Visible="false">
                    <Rock:NotificationBox ID="nbDuplicateWarning" runat="server" NotificationBoxType="Warning" Title="Possible Duplicates!"
                        Text="<p>One ore more of the people you are adding may already exist! Please confirm that none of the existing people below are the same person as someone that you are adding." />
                    <asp:PlaceHolder ID="phDuplicates" runat="server" />
                </asp:Panel>

                <asp:ValidationSummary ID="valSummaryTop" runat="server" HeaderText="Please Correct the Following" CssClass="alert alert-danger" />

                <div class="actions">
                    <asp:LinkButton ID="btnPrevious" runat="server" Text="Previous" CssClass="btn btn-link" OnClick="btnPrevious_Click" Visible="false" CausesValidation="false" />
                    <asp:LinkButton ID="btnNext" runat="server" Text="Next" CssClass="btn btn-primary" OnClick="btnNext_Click" />
                </div>
            </div>
        </div>

    </ContentTemplate>
</asp:UpdatePanel>
