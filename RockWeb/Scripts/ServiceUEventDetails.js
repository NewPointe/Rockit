var width = $(window).width();
var id = $("[id*='hdnEvent']").val();
var catID = "";


$(document).ready(function () {
    GetEvent(id);
});


function GetEvent(id) {
    var url = "http://api.serviceu.com/rest/events/7808086?orgkey=195d2f5d-2cc0-48c8-a533-0c990a59a46f&format=json&callback";

    $("#event").empty();
    $.ajax({
        type: 'GET',
        dataType: 'jsonp',
        url: url,
        crossDomain:true,
        error: function (e) {
            console.log(e);
            alert('error occurred');
        },
        success: function (result) {
            console.log(result)
            if (result.length == 0) {
                $("#event").append("<h6>Event not found</h6>");
            }

            else {
                $.each(result, function (index, element) {
                    
                    $("#event").append(
                            '<h1>' + result.Name + '</h1> <p>' + result.Description + "</p>"
                            
                        )
                });
            }
        }
    });
}

