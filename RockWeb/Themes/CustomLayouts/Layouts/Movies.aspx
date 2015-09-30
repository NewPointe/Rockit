<%@ Page Language="C#" MasterPageFile="Movies.Master" AutoEventWireup="true" Inherits="Rock.Web.UI.RockPage" %>

<asp:Content ID="ctMain" ContentPlaceHolderID="main" runat="server">

    <main class="container at-the-movies">
                
        <!-- Start Content Area -->
        
        <!-- Page Title -->
     <%--    <Rock:PageIcon ID="PageIcon" runat="server" /> <h1><Rock:PageTitle ID="PageTitle" runat="server" /></h1>
        <Rock:PageBreadCrumbs ID="PageBreadCrumbs" runat="server" />--%>

        <div class="row">
            <div class="col-md-12">
                <Rock:Zone Name="Logo" runat="server" />
            </div>
        </div>

                    
        <!-- Ajax Error -->
        <div class="alert alert-danger ajax-error" style="display:none">
            <p><strong>Error</strong></p>
            <span class="ajax-error-message"></span>
        </div>

        <div class="row">
            <div class="col-md-12">
                <Rock:Zone Name="Feature" runat="server" />
            </div>
        </div>

        <div class="row headlines">
            <div class="col-sm-4">
                <Rock:Zone Name="Sidebar 1" runat="server" />
            </div>
            <div class="col-sm-4">
                <Rock:Zone Name="Main" runat="server" />
            </div>
            <div class="col-sm-4">
                <Rock:Zone Name="Sidebar 2" runat="server" />
            </div>
        </div>

        <div class="row description border-bottom">
            <div class="col-md-12">
                <Rock:Zone Name="Description A" runat="server" />
            </div>
        </div>

        <div class="row description">
            <div class="col-md-12">
                <Rock:Zone Name="Description B" runat="server" />
            </div>
        </div>

        <div class="row movie">
            <div class="col-md-6">
                <Rock:Zone Name="Movie Trailer 1" runat="server" />
            </div>
            <div class="col-md-6">
                <Rock:Zone Name="Moview Text 1" runat="server" />
            </div>
        </div>

        <div class="row movie">
            <div class="col-md-6">
                <Rock:Zone Name="Movie Trailer 2" runat="server" />
            </div>
            <div class="col-md-6">
                <Rock:Zone Name="Moview Text 2" runat="server" />
            </div>
        </div>

                <div class="row movie">
            <div class="col-md-6">
                <Rock:Zone Name="Movie Trailer 3" runat="server" />
            </div>
            <div class="col-md-6">
                <Rock:Zone Name="Moview Text 3" runat="server" />
            </div>
        </div>

                <div class="row movie">
            <div class="col-md-6">
                <Rock:Zone Name="Movie Trailer 4" runat="server" />
            </div>
            <div class="col-md-6">
                <Rock:Zone Name="Moview Text 4" runat="server" />
            </div>
        </div>

        <div class="row movie">
            <div class="col-md-6">
                <Rock:Zone Name="Movie Trailer 5" runat="server" />
            </div>
            <div class="col-md-6">
                <Rock:Zone Name="Moview Text 5" runat="server" />
            </div>
        </div>


        
        <div class="row">
            <div class="col-md-4 social">
                <Rock:Zone Name="Social 1" runat="server" />
            </div>
            <div class="col-md-4 social">
                <Rock:Zone Name="Social 2" runat="server" />
            </div>
            <div class="col-md-4 social">
                <Rock:Zone Name="Social 3" runat="server" />
            </div>
        </div>

        <!-- End Content Area -->

    </main>

</asp:Content>
