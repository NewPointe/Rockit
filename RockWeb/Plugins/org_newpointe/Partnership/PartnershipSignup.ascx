<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PartnershipSignup.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Partnership.PartnershipSignup" %>


<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>
        
        <Rock:ModalAlert ID="mdNotLoggedIn" runat="server" />

 
        <asp:Panel ID="pnlSignup" runat="server" CssClass="panel panel-block" Visible="True">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> NewPointe Partnership Agreement</h1>
            </div>
            <div class="panel-body">

                <h2>Partnership Covenant for <asp:Literal runat="server" ID="lPersonInfo"></asp:Literal></h2>
                
                <p>Our Mission: To lead people realize and reach their full potential in Jesus Christ.</p>
                
                <p>As a partner with NewPointe Community Church, I am committing to uphold these values as we work together to fulfill our mission. And as we uphold these values, we will create a welcoming and beautiful community of Christ followers.</p>
                
                <ol>
                   <li>I will protect the unity of my church
                        <ul>
                            <li>by acting in love toward others <small>(1 Peter 1:22)</small></li>
                            <li>by refusing to gossip <small>(Ephesians 4:29)</small></li>
                            <li>by submitting to church leadership <small>(Hebrews 13:17)</small></li>
                        </ul>
                    </li>
                    
                    <li>I will share the responsibilities of my church
                        <ul>
                            <li>by praying for our church <small>(1 Thessalonians 1:2)</small></li>
                            <li>by inviting unconnected people to church <small>(Luke 14:23)</small></li>
                            <li>by welcoming those who visit our church <small>(Romans 15:7)</small></li>
                        </ul>
                    </li>
                    
                    <li>I will serve my church
                        <ul>
                            <li>by cultivating a servant’s heart</li>
                            <li>by using my spiritual gift in my church <small>(1 Peter 4:10)</small></li>
                            <li>by participating in my church’s training gatherings <small>(Ephesians 4:11-12)</small></li>
                        </ul>
                    </li>
                    
                    <li>I will support my church
                        <ul>
                            <li>by attending faithfully <small>(Hebrews 10:25)</small></li>
                            <li>by pursuing spiritual growth and holy living <small>(Philippians 1:27)</small></li>
                            <li>by giving financially <small>(1 Corinthians 16:2 & Leviticus 27:30)</small></li>
                        </ul>
                    </li>
                    
                </ol>
                
                <br /><br />
                <ul>
                    <li>To develop these values within my personal life, I am committing to
                        <ul>
                            <li>faithfully pray for the ministry of NewPointe</li>
                            <li>use my spiritual gifts in ministry at NewPointe</li>
                            <li>give financially at NewPointe</li>
                            <li>attend regularly at NewPointe</li>
                        </ul>
                    </li>
                </ul>
                
                <br /><br />
                
            </div>
        </asp:Panel>
        
        
        
        <asp:Panel ID="pnlSignature" runat="server" CssClass="panel panel-block" Visible="True">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> NewPointe Partnership Agreement</h1>
            </div>
            <div class="panel-body">

                <p>By signing <small>(typing my name in the box)</small> below, I agree to...</p>

               <Rock:RockTextBox runat="server" ID="tbSignature" Label="Signature"/>
                
                <Rock:BootstrapButton runat="server" ID="btnSubmit" DataLoadingText="Submitting Partnership Covenant" CssClass="btn btn-newpointe" OnClick="btnSubmit_OnClick">Submit</Rock:BootstrapButton>
                
                
                <br /><br />

            </div>
        </asp:Panel>

        
        
        <asp:Panel ID="pnlOpportunities" runat="server" CssClass="panel panel-block" Visible="False">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> NewPointe Partnership Covenant</h1>
            </div>
            <div class="panel-body">
                
                <h4>Thank you! Your Partnership Covenant has been submitted.</h4>

                <div class="well">
                <h3>My Partnership Opportunities</h3>
                
                <p><strong>Serving:</strong> <asp:Literal runat="server" ID="lServing"></asp:Literal> </p>
                <p><strong>Giving:</strong> <asp:Literal runat="server" ID="lGiving"></asp:Literal> </p>
                <p><strong>DISCOVER My Church:</strong> <asp:Literal runat="server" ID="lDiscover"></asp:Literal> </p> 
                
                </div>

            </div>
        </asp:Panel>
        
        
        
        
        
        
        
        <asp:Panel ID="pnlNotLoggedIn" runat="server" CssClass="panel panel-block" Visible="False">
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-user"></i> NewPointe Partnership Agreement</h1>
            </div>
            <div class="panel-body">

                <Rock:NotificationBox runat="server" NotificationBoxType="Danger" Text="You must be logged to sign the Partnership Covenant. " Visible="True" ID="nbNoPerson" Title="No Valid Person"></Rock:NotificationBox>

            </div>
        </asp:Panel>
        
        


    </ContentTemplate>
</asp:UpdatePanel>


