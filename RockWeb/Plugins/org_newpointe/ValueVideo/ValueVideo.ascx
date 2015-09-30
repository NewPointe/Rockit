<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ValueVideo.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.ValueVideo.ValueVideo" %>

<div class="value-video container-fluid">

    <div class="row title">
        <div class="col-xs-12">
            <h2>This is who we are</h2>
        </div>
    </div>
    <div class="row">
 
            <video controls="" poster="http://newpointe.org/wp-content/uploads/2013/08/Video_image_placeholder_for_the_website.png">
                <source onclick="this.paused?this.play():this.pause();" 
                    src="http://newpointe.org/wp-content/uploads/assets/video/newpointe_..._this_is_who_we_are_..._this_is_what_we_value_1280x720.mp4?_=1" type="video/mp4" />
                Your browser does not support the video tag.
            </video>

    </div>
</div>




