<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShapeResults.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Shape.ShapeResults" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:NotificationBox runat="server" NotificationBoxType="Danger" Text="You must be logged in or access this page from a vaild link." Visible="False" ID="nbNoPerson" Title="No Valid Person"></Rock:NotificationBox>


        <h1 class="text-center">My SHAPE Profile</h1>
        <h2 class="text-center" style="margin-top: -10px;">
            <asp:Label runat="server" ID="lbPersonName"></asp:Label></h2>

        <div class="col-xs-12 resourceMenu">
            <h4>
                <a data-toggle="collapse" data-target="#collapseSpiritualGifts" aria-expanded="true" aria-controls="collapseExample">
                    <i class="fa fa-share-square-o"></i> Spiritual Gifts
                </a>
            </h4>
        </div>

        <div class="col-xs-12 collapse in gray-bg" id="collapseSpiritualGifts">
            <div class="col-md-6">
                <h5 class="text-center">Spiritual Gift 1: </h5>
                <h3 class="text-center" style="margin-top: -10px;">
                    <asp:Label runat="server" ID="lbGift1Title"></asp:Label></h3>
                <asp:Label runat="server" ID="lbGift1BodyHTML"></asp:Label>
            </div>

            <div class="col-md-6">
                <h5 class="text-center">Spiritual Gift 2: </h5>
                <h3 class="text-center" style="margin-top: -10px;">
                    <asp:Label runat="server" ID="lbGift2Title"></asp:Label></h3>
                <asp:Label runat="server" ID="lbGift2BodyHTML"></asp:Label>
            </div>

            <div class="col-md-12">
                <br />
            </div>

        </div>


        <div class="col-xs-12 resourceMenu">
            <h4>
                <a data-toggle="collapse" data-target="#collapseHeart" aria-expanded="true" aria-controls="collapseExample">
                    <i class="fa fa-share-square-o"></i> Heart and Abilities
                </a>
            </h4>
        </div>
        <div class="col-xs-12 collapse in gray-bg" id="collapseHeart">
            <div class="col-md-6">
                <h5 class="text-center">Heart/Ability 1: </h5>
                <h3 class="text-center" style="margin-top: -10px;">
                    <asp:Label runat="server" ID="lbHeart1Title"></asp:Label></h3>
                <asp:Label runat="server" ID="lbHeart1BodyHTML"></asp:Label>
            </div>

            <div class="col-md-6">
                <h5 class="text-center">Heart/Ability 2: </h5>
                <h3 class="text-center" style="margin-top: -10px;">
                    <asp:Label runat="server" ID="lbHeart2Title"></asp:Label></h3>
                <asp:Label runat="server" ID="lbHeart2BodyHTML"></asp:Label>
            </div>


            <div class="col-md-12">
                <br />
            </div>

        </div>



        <div class="col-xs-12 resourceMenu">
            <h4>
                <a data-toggle="collapse" data-target="#collapsePersonality" aria-expanded="true" aria-controls="collapseExample">
                    <i class="fa fa-share-square-o"></i> Personality
                </a>
            </h4>
        </div>
        <div class="col-xs-12 collapse in gray-bg" id="collapsePersonality">
            <div class="col-md-12">
                <h4 class="text-center">DISC Goes Here </h4>
            </div>

            <div class="col-md-12">
                <br />
            </div>

        </div>



        <div class="col-xs-12 resourceMenu">
            <h4>
                <a data-toggle="collapse" data-target="#collapsePersonality" aria-expanded="true" aria-controls="collapseExample">
                    <i class="fa fa-share-square-o"></i> Experiences 
                </a>
            </h4>
        </div>
        <div class="col-xs-12 collapse in gray-bg" id="collapsePersonality">
            <div class="col-md-12">
                <h4 class="text-center">Experiences Go Here </h4>
            </div>

            <div class="col-md-12">
                <br />
            </div>

        </div>





        <div class="col-xs-12 resourceMenu">
            <h4>
                <a data-toggle="collapse" data-target="#collapseVolunteer" aria-expanded="true" aria-controls="collapseExample">
                    <i class="fa fa-share-square-o"></i>Volunteer Opportunities
                </a>
            </h4>
        </div>
        <div class="col-xs-12 collapse in gray-bg" id="collapseVolunteer">

            <div class="col-md-12">
                <h2 class="text-center">Volunteer Opportunities</h2>
                <p>Based on your profile, here is a list of strategic serving opportunities that match your SHAPE:</p>

                <asp:Repeater runat="server" ID="rpVolunteerOpportunities">
                    <ItemTemplate>

                        <div class="panel panel-default margin-t-md">
                            <div class="panel-heading clearfix">
                                <h1 class="panel-title pull-left">
                                    <i class='<%# Eval("IconCssClass") %>'></i><%# Eval("Name") %>
                                </h1>
                            </div>
                            <div class="panel-body">
                                <div class="col-md-12">
                                    <p><%# Eval("Summary") %></p>
                                    <a class="btn btn-default" href="https://newpointe.org/VolunteerOpportunities/<%# Eval("Id") %>" role="button">More Info</a>
                                </div>
                            </div>
                        </div>

                    </ItemTemplate>
                </asp:Repeater>

                <a class="btn btn-newpointe" href="https://newpointe.org/VolunteerOpportunities/" role="button">See More Volunteer Opportunities</a>


            </div>


            <div class="col-md-12">
                <br />
            </div>

        </div>

    </ContentTemplate>
</asp:UpdatePanel>
