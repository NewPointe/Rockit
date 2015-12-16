<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CampusMenu.ascx.cs" Inherits="Plugins_org_newpointe_CampusMenu_CampusMenu" %>

<div id="divCampusTimes" class="hidden-xs hidden-sm hidden-md col-lg-12">
    <h5 class="pull-right"><%= LiveServiceText %></h5>
</div>

<!-- Live Service Modal -->
<div id="liveModal" class="modal fade" role="dialog">
    
    <!-- Modal content-->
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal">&times;</button>
        <h3 class="modal-title">Service are Live Now!</h3>
      </div>
      <div class="modal-body text-center">
          <div class="text-center"><i class="fa fa-5x fa-video-camera"></i></div>
        <a class="btn btn-newpointe" href="http://live.newpointe.org">WATCH LIVE NOW</a>
        <button type="button" class="btn btn-newpointe" data-dismiss="modal">CONTINUE TO OUR SITE</button>
      </div>
      <!--<div class="modal-footer">
        <button type="button" class="btn btn-default" data-dismiss="modal">GOT IT</button>
      </div> -->
    </div>
</div>

 <script type="text/javascript">
    function openModal() {
        $('#liveModal').modal('show');
    }
</script>