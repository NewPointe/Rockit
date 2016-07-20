<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShapeResults.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Shape.ShapeResults" %>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        <Rock:NotificationBox runat="server" NotificationBoxType="Danger" Text="You must be logged in or access this page from a vaild link." Visible="False" ID="nbNoPerson" Title="No Valid Person"></Rock:NotificationBox>
        
        <Rock:NotificationBox runat="server" NotificationBoxType="Success" Text="It looks like you don't have a MyNewPointe account yet.  Create one at the bottom of this page after you finish reading your SHAPE Profile!  " Visible="False" ID="nbTip" Title="Tip!"></Rock:NotificationBox>
        
 
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
            </div>
            
            
            <asp:Panel runat="server" ID="DISCResults" Visible="False">

            <div class="col-md-12" >
                
                                    <ul class="discchart">
                        <li class="discchart-midpoint"></li>
                        <li style="height: 100%; width:0px;"></li>
                        <li id="discNaturalScore_D" runat="server" class="discbar discbar-d">
                            <div class="discbar-label">D</div>
                        </li>
                        <li id="discNaturalScore_I" runat="server" class="discbar discbar-i">
                            <div class="discbar-label">I</div>
                        </li>
                        <li id="discNaturalScore_S" runat="server" class="discbar discbar-s">
                            <div class="discbar-label">S</div>
                        </li>
                        <li id="discNaturalScore_C" runat="server" class="discbar discbar-c">
                            <div class="discbar-label">C</div>
                        </li>
                    </ul>
                

                
                <h3>Description</h3>
                    <asp:Literal ID="lDescription" runat="server"></asp:Literal>

                    <h3>Strengths</h3>
                    <asp:Literal ID="lStrengths" runat="server"></asp:Literal>

                    <h3>Challenges</h3>
                    <asp:Literal ID="lChallenges" runat="server"></asp:Literal>

                    <h3>Team Contribution</h3>
                    <asp:Literal ID="lTeamContribution" runat="server"></asp:Literal>

                    <h3>Leadership Style</h3>
                    <asp:Literal ID="lLeadershipStyle" runat="server"></asp:Literal>
                
                
                <br />
            </div>
                
                </asp:Panel>
            
            <asp:Panel runat="server" ID="NoDISCResults" Visible="True">

            <div class="col-md-12 text-center" >
                
                <p>The DISC Assessment is used for the Personality portion of your SHAPE Profile.
                    It looks like you haven't taken the DISC Assessment yet.</p>
                <a href="https://newpointe.org/DISC?rckipid=<%= SelectedPerson.UrlEncodedKey %>" target="_blank" class="btn btn-primary np-button">TAKE THE DISC ASSESSMENT NOW</a>
                
                
                <br />
            </div>
                
                </asp:Panel>


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
                <h4 class="text-center">People </h4>
                <p><asp:Label runat="server" ID="lbPeople"></asp:Label><br /></p>
                
                <h4 class="text-center">Places </h4>
                <p><asp:Label runat="server" ID="lbPlaces"></asp:Label><br /></p>
                
                <h4 class="text-center">Events </h4>
                <p><asp:Label runat="server" ID="lbEvents"></asp:Label><br /></p>
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
                                    <i class='<%# Eval("IconCssClass") %>'></i> <%# Eval("Name") %>
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
        
        <div class="col-md-12">
                <br />
            </div>
        
        <asp:Panel runat="server" Visible="False" Id="pnlAccount">
        <div class="col-xs-12 resourceMenu">
            <h4>
                <a data-toggle="collapse" data-target="#collapseAccount" aria-expanded="true" aria-controls="collapseExample">
                    <i class="fa fa-share-square-o"></i> MyNewPointe Account
                </a>
            </h4>
        </div>
        <div class="col-xs-12 collapse in gray-bg" id="collapseAccount">
            <div class="col-md-12">
            </div>
        
        <asp:Panel ID="pnlSaveAccount" runat="server" Visible="true">
                <div class="well">
                    <legend>Create a MyNewPointe Account</legend>
                    <fieldset>

                        <asp:PlaceHolder ID="phCreateLogin" runat="server" Visible="true">

                            <Rock:RockTextBox ID="txtUserName" runat="server" Label="Username" CssClass="input-medium" />
                            <Rock:RockTextBox ID="txtPassword" runat="server" Label="Password" CssClass="input-medium" TextMode="Password" />
                            <Rock:RockTextBox ID="txtPasswordConfirm" runat="server" Label="Confirm Password" CssClass="input-medium" TextMode="Password" />
                            
                        </asp:PlaceHolder>

                        <Rock:NotificationBox ID="nbSaveAccount" runat="server" Visible="false" NotificationBoxType="Danger"></Rock:NotificationBox>

                        <div id="divSaveActions" runat="server" class="actions">
                            <asp:LinkButton ID="lbSaveAccount" runat="server" Text="Save Account" CssClass="btn btn-primary" OnClick="lbSaveAccount_Click"  />
                        </div>
                    </fieldset>
                </div>
            </asp:Panel>
            
            </div>
            
            <div class="col-md-12">
                <br />
            </div>
            
            </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
