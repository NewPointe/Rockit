(function ($) {
    var jsonData = [];
    var searchData;
    var id = $("input[name *= hdnCampus]").val();
    var dropdownlist = $("[id*=ddlCampusDropdown]");
    "use strict";

    var campusID = $("input[name *= hdnCampus]").val();
    var campus;
    var catID = "";
    var calclass = "";

    if (campusID == "") {
        campusID = '51773'
    }

    function getCampusId(cmp) {
        switch (cmp) {
            case "Akron Campus":
                return 67713;
            case "Canton Campus":
                return 53103;
            case "Coshocton Campus":
                return 62004;
            case "Millersburg Campus":
                return 51774;
            case "Wooster Campus":
                return 67714;
            case "Dover Campus":
                return 51773;
        }
    }


    switch (campusID) {
        case "67713":
            campus = "Akron Campus";
            $(".btn-akron").addClass("active");
            $(dropdownlist).val("67713");
            break;

        case "53103":
            campus = "Canton Campus";
            $(".btn-canton").addClass("active");
            $(dropdownlist).val("53103");
            break;

        case "62004":
            campus = "Coshocton Campus";
            $(".btn-coshocton").addClass("active");
            $(dropdownlist).val("62004");
            break;

        case "51774":
            campus = "Millersburg Campus";
            $(".btn-millersburg").addClass("active");
            $(dropdownlist).val(51774);
            break;

        case "67714":
            campus = "Wooster Campus";
            $(dropdownlist).val("67714");
            $(".btn-wooster").addClass("active");
            break;

        case "51773":
            campus = "Dover Campus"
            $(".btn-dover").addClass("active");
            $(dropdownlist).val("51773");
            break;

        default:
            campus = "Dover Campus"
            $(".btn-dover").addClass("active");
            $(dropdownlist).val("51773");
            break;
    }


    function getArray() {
        return $.getJSON('/assets/calendar-full.json');
    }

    getArray().done(bindCalendar);


    function bindCalendar(json) {
        jsonData = json;

        searchData = $.grep(json, function (d) {
            if (calclass == "") {
                return d.departmentname == campus;
            } else {
                return d.departmentname == campus && d.class == calclass;
            }
        });



        var options = {
            events_source: searchData, //'/assets/calendar.json',
            view: 'month',
            tmpl_path: '/Scripts/tmpls/',
            tmpl_cache: false,
            format12: true,
            day: 'now',
            display_week_number: false,
            weekbox: false,
            onAfterEventsLoad: function (events) {
                if (!events) {
                    return;
                }

                var list = $('#eventlist');
                list.html('');

                $.each(events, function (key, val) {
                    $(document.createElement('li'))
                        .html('<a href="' + val.url + '">' + val.title + '</a>')
                        .appendTo(list);
                });
            },
            onAfterViewLoad: function (view) {
                $('.page-header h3').text(this.getTitle());
                $('.btn-group button').removeClass('active');
                $('button[data-calendar-view="' + view + '"]').addClass('active');
                //ads on click event to calendar date. 
                $(".cal-cell").click(function () {
                    getEventsBy($(this).find("div.cal-month-day span").data("cal-date"));
                });
                //void event links 
                $("div.events-list a").attr("href", "javscript:void(0);");
                console.log(this);
                getEventsBy(new Date());
                $.each($(".cal-row-head div.cal-cell1"), function () {
                    $(this).html($(this).html().slice(0, 3));
                });
            },
            classes: {
                months: {
                    general: 'label'
                }
            }
        };


        var calendar = $('#calendar').calendar(options);


        $('.btn-group button[data-calendar-nav]').each(function () {
            var $this = $(this);
            $this.click(function (e) {
                e.preventDefault();
                calendar.navigate($this.data('calendar-nav'));
            });
        });

        $('.btn-group button[data-calendar-view]').each(function () {
            var $this = $(this);
            $this.click(function () {
                calendar.view($this.data('calendar-view'));
            });

        });

        $('#first_day').change(function () {
            var value = $(this).val();
            value = value.length ? parseInt(value) : null;
            calendar.setOptions({ first_day: value });
            calendar.view();
        });
    }

    function getEventsBy(date, id) {
        var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
        date = new Date(date);
        $("#event-date").html(monthNames[date.getMonth()] + ' ' + date.getUTCDate());
        var eventList = $("#events-list");
        var search = "/Plugins/org_newpointe/CalendarSearch.aspx?date=" + (date.getUTCMonth() + 1) + "/" + date.getUTCDate() + "/" + date.getUTCFullYear();
        eventList.empty();
        $(".event-description").hide();
        $("#event-description").empty();
        if (id != undefined) {
            search = "/Plugins/org_newpointe/CalendarSearch.aspx?id=" + id;
        }
        $.getJSON(search, function (data) {
            //filter out based on campus if this isn't a drop down list saerch
            if (id == undefined) {
                var searchData = $.grep(data, function (d) {
                    if (calclass == "") {
                        return d.departmentname == campus;
                    } else {
                        return d.departmentname == campus && d.class == calclass;
                    }
                });
            }

            var start, end;
            $.each(searchData, function (key, val) {
                start = new Date(data[key].start);
                end = new Date(data[key].end);
                eventList.append('<div class="col-sm-12 events nopadding"  data-id="' + data[key].id + '" data-description="' + data[key].description + '"><i class="fa fa-plus-square"></i><i class="fa fa-info-circle" style="display:none;"></i><strong>' + data[key].startTime + '-' + data[key].endtime + '</strong> ' + data[key].title + '</div><div class="col-sm-12 event-description  parent-' + data[key].id + '" style="display:none;">' + data[key].description + '</div>');
            });
            if (searchData.length == 0) {
                $("#event-description").html("<div class='text-center'><strong>NO EVENTS TO DISPLAY</strong></div>");
            }
            eventList.find("div.events").click(function () {
                $(".event-description").hide();
                $("i.fa-info-circle").hide();
                $("i.fa-plus-square").show();
                $(this).find("i.fa-info-circle").show();
                $(this).find("i.fa-plus-square").hide();
                $(".parent-" + $(this).data("id")).slideDown();
            });
        });
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
        catID = cID;
        if (getEvents) {
            setActiveButton(id, cID);
        }
    }

    function setActiveButton(eventID, catID) {
        var campusName = $("input[name *= CampusName]").val();
        var dropdownlist = $("[id*=ddlCampusDropdown]");
        $("#CampusButtons a").removeClass("active");
        switch (eventID) {
            // set Akron active
            case '67713':
                $(".btn-akron").addClass("active");
                campus = "Akron Campus"
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Akron campus");
                break;
            case '53103':
                $(".btn-canton").addClass("active");
                campus = "Canton Campus"
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Canton campus");
                break;
            case '62004':
                $(".btn-coshocton").addClass("active");
                campus = "Coshocton Campus"
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Coshocton campus");
                break;
            case '51774':
                $(".btn-millersburg").addClass("active");
                campus = "Millersburg Campus"
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Millersburg campus");
                break;
            case '67714':
                $(".btn-wooster").addClass("active");
                campus = "Wooster Campus"
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Wooster campus");
                break;
            default:
                $(".btn-dover").addClass("active");
                campus = "Dover Campus"
                $(dropdownlist).val(id);
                $(campusName).text("Upcoming events for Dover campus");
                break;
        }

        id = eventID;
        GetEvents(campus, catID);
    }

    function GetEvents(campus, catID) {
        campusID = getCampusId(campus);

        switch (catID) {
            case "13399":
                calclass = "event-important";
                break;
            case "13405":
                calclass = "event-info";
                break;
            case "21205":
                calclass = "event-warning";
                break;
            case "11111":
                calclass = "event-success";
                break;
            case "00000":
                calclass = "event-inverse";
                break;
            default:
                calclass = "";
                break;
        }
        bindCalendar(jsonData);
    }

    $(document).ready(function () {

        $(".campusButton").hover(function (e) {
            $(this).text($(this).attr("data-fullname"));
        }, function (t) {
            $(this).text($(this).attr("data-shortname"));
        });

        $(".categoryButton").hover(function (e) {
            $(this).text($(this).attr("data-hovername"));
        }, function (t) {
            $(this).html($(this).attr("data-shortname"));
        });

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

        $("[id*='ddlCampusDropdown']").change(function () {
            id = $(this).val();
            setActiveButton(id, catID);
        });

        $("[id*='ddlCategoryDropdown']").change(function () {
            catID = $(this).val();
            setActiveCategory(catID, false);
            setActiveButton(id, catID)
        });
        $("#collapse-Button").click(function () {
            console.log('dafsd');
            var cfilter = $("#collapseFilter");
            if (!cfilter.is(':hidden')) {
                cfilter.slideUp();
            } else {
                cfilter.slideDown();

            }
        });


        $("#calendar-search").autocomplete({
            serviceUrl: "../../Plugins/org_newpointe/CalendarSearch.aspx",
            onSelect: function (suggestion) {
                //Logs the EventID from the selected event.
                getEventsBy('', suggestion.data);
            }
        });
        var event = $("input[name *= hdnEventId ]");
        if (event.val() != '') {
            getEventsBy('', event.val());
            event.val('');
        }
    });

}(jQuery));