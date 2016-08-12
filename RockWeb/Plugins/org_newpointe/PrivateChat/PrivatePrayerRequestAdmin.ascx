<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PrivatePrayerRequestAdmin.ascx.cs" Inherits="RockWeb.Plugins.org_newpointe.PrivateChat.PrivatePrayerRequestAdmin" %>

<!-- Latest compiled and minified CSS -->
<link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/bootstrap-table/1.11.0/bootstrap-table.min.css">
<link rel="stylesheet" href="//rawgit.com/vitalets/x-editable/master/dist/bootstrap3-editable/css/bootstrap-editable.css">


<!-- Latest compiled and minified JavaScript -->
<script src="//cdnjs.cloudflare.com/ajax/libs/bootstrap-table/1.11.0/bootstrap-table.min.js"></script>

<table id="prayer-table" data-search="true">
    <thead>
        <tr>
            <th data-events="answerPrayer" data-formatter="NameFormatter" data-field="Name">Name</th>
        </tr>
    </thead>
    <tbody>
    </tbody>
</table>

<script>
    $(document).ready(function () {
        GetPrayers();

        setInterval(function () {
            $('#prayer-table').bootstrapTable('destroy');
            GetPrayers();
        }, 30000)
    });

    function GetPrayers() {
        $.ajax({
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            url: 'https://' + window.location.host + '/api/PrivatePrayer',
            error: function () {
                console.log('error occurred');
            },
            success: function (result) {
                var data = result;
                BindTable(data)
            }
        });
    }

    function NameFormatter(value, row, index) {
        return '<a class="aAnswerPrayer" href="javascript:void(0)"> ' + row.Name + '</a>';
    }

    function BindTable(data) {
        $('#prayer-table').bootstrapTable({
            data: data
        });
    }

    function HandlePrayer(row)
    {
        $.ajax({
            type: 'PATCH',
            crossDomain: true,
            url: 'https://' + window.location.host + '/api/PrivatePrayerRequests/' + row.Id,
            contentType: "application/json; charset=UTF-8",
            data: "{\r\nAnswered: true,\r\n}",
            success: function (data) {
                window.location.replace('https://' + window.location.host + '/page/1123?RoomId=' + row.RoomId);
                $('#prayer-table').bootstrapTable('destroy');
                GetPrayers();
            }, error: function () {
                console.log('error occurred');
            }
        })
    }

    window.answerPrayer = {
        'click .aAnswerPrayer': function (e, value, row, index) {
            HandlePrayer(row);
        }
    }
</script>
