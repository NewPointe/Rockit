<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Podcasts.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.Podcasts.Podcasts" %>

<div class="container-fluid">


    <asp:Repeater runat="server" ID="rpt">
        <ItemTemplate>
            <div class="row" style="padding-bottom:15px;">


                <div class="hidden-xs col-sm-3 col-md-2">
                    <img src='<%#Eval("Image") %>' />
                </div>
                <div class="col-sm-5 col-md-3">
                    <p><strong><%# Eval("Title") %></strong></p>
                    <p><em><%# Eval("SubTitle") %></em></p>
                </div>
                <div class="col-sm-4 col-md-3">
                    <p class="small"><strong><%# Eval("Date") %></strong></p>
                    <p class="small"><%# Eval("Duration") %></p>
                </div>
                <div class="col-md-4">
                   <audio controls>
          
                      <source src='<%# Eval("AudioFile") %>' type="audio/mpeg">
                    Your browser does not support the audio element.
                    </audio>
                </div>


            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>




