var width = $(window).width();
var id = $("input[name *= hdnCampus]").val();
var catID = "";


$(document).ready(function () {
    if (id == "") {
        id = '51773'
    }

    setActiveButton(id, catID);
    //setActiveCategory(catID);

    $(".categoryButton").click(function (e) {
        e.preventDefault();
        setActiveCategory($(this).attr("data-categoryid"), true);
    })

    $(".campusButton").click(function (e) {
        e.preventDefault();
        $("#CampusName").text("Upcoming events for " + $(this).text()) + "campus";
        setActiveButton($(this).attr("data-campusid"), catID);
        //GetEvents($(this).attr("data-campusid"));
    });

    $(".categoryButton").hover(function (e) {
        $(this).text($(this).attr("data-hovername"));
    }, function (t) {
        $(this).html($(this).attr("data-shortname"));
    });

    $("[id*='ddlCampusDropdown']").change(function () {
        id = $(this).val();
        GetEvents(id, catID);
    })

    $("[id*='rckStartDate']").change(function () {
        if ($(this).val() != "") {
            GetEvents(id, catID);
        }
    })

    $("[id*='rckEndDate']").change(function () {
        if ($(this).val() != "") {
            GetEvents(id, catID);
        }
    })

    $("[id*='ddlCategoryDropdown']").change(function () {
        catID = $(this).val();
        setActiveCategory(catID, false);
        GetEvents(id, catID);
    })

    $(window).on('resize', function (e) {
        SetButtonText();
    });
});

function SetButtonText() {
    width = $(window).width();
    if (width < 992) {
        $("#CampusButtons a").each(function () {
            $(this).text($(this).attr("data-shortname"));
        });
    }
    else {
        $("#CampusButtons a").each(function () {
            $(this).text($(this).attr("data-fullname"));
        });
    }
}

function setActiveCategory(cID, getEvents) {
    var ddl = $("[id*=ddlCategoryDropdown");
    $(ddl).val(cID);
    $("#CategoryButtons a").removeClass("active");
    switch (cID) {
        case '13399':
            $(".btn-cm").addClass("active");
            break;

        case '13405':
            $(".btn-sm").addClass("active");

            break;

        case '21205':
            $(".btn-ya").addClass("active");
            break;

        case '11111':
            $(".btn-g").addClass("active");
            break;

        case '00000':
            $(".btn-ae").addClass("active");
            break;

        default:
            $(".btn-all").addClass("active");
            break;
    }

    if (cID == "") {
        catID = "all"
    }

    else {
        catID = cID
    }

    if (getEvents) {
        setActiveButton(id, catID);
    }
}

function setActiveButton(eventID, catID) {
    var campusName = $("#CampusName");
    var dropdownlist = $("[id*=ddlCampusDropdown]");
        $("#CampusButtons a").removeClass("active");
        switch (eventID) {
            // set Akron active
            case '67713':
                $(".btn-akron").addClass("active");
                
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Akron campus");
                break;
            case '53103':
                $(".btn-canton").addClass("active");
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Canton campus");
                break;
            case '62004':
                $(".btn-coshocton").addClass("active");
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Coshocton campus");
                break;
            case '51774':
                $(".btn-millersburg").addClass("active");
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Millersburg campus");
                break;
            case '67714':
                $(".btn-wooster").addClass("active");
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Wooster campus");
                break;
            default:
                $(".btn-dover").addClass("active");
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Dover campus");
                break;
        }
        
        id = eventID;
        GetEvents(id, catID);
}

function GetEvents(id, catID) {
    var url = "http://api.serviceu.com/rest/events/occurrences?";
    var deptID = id;
    var start = $("[id*='rckStartDate']").val();
    var end = $("[id*='rckEndDate']").val();
    var startDate = "";
    var endDate = "";

    if (start == "")
    {
        startDate = Date.today();
    }
    else {
        startDate = Date.parse(start);
    }
    
    if (end == "") {
        endDate = new Date(startDate);
        endDate = endDate.add({ days: 30 });
    }
    else {
        endDate = Date.parse(end);
    }

    if (deptID == "") {
        deptID = "51773";
    }

    url += 'departmentids=' + deptID;


    if (start != "" || end != "") {
        url += "&startDate=" + startDate.toString("MM/dd/yyyy");
        url += "&endDate=" + endDate.toString("MM/dd/yyyy");
    }
    if (catID != "" && catID != "all") {
        url += "&categoryIDs=" + catID;
        //url = 'http://api.serviceu.com/rest/events/occurrences?departmentids=' + deptID + '&nextDays=7&orgkey=195d2f5d-2cc0-48c8-a533-0c990a59a46f&format=json&callback=?';
    }


    url += "&nextDays=7&orgkey=195d2f5d-2cc0-48c8-a533-0c990a59a46f&format=json&callback=?";


    $(".plus-loader").show();
    $("#CampusEvents").empty();
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: url,
        error: function () {
            alert('error occurred');
        },
        success: function (result) {
            $(".plus-loader").hide();
            if (result.length == 0) {
                $("#CampusEvents").append("<h6>No upcoming events</h6>");
            }

            else {
                $.each(result, function (index, element) {
                    
                    $("#CampusEvents").append(
                            "<p><strong>" + element.Name + "</strong> <br /> Date:" + element.OccurrenceStartTime + '<br/> <a href="/EventDetails?EventID=' + element.EventId + '" target="_blank">More Details</a></p><hr style="border-color:#404040;" />'
                        )
                });
            }
        }
    });
}

