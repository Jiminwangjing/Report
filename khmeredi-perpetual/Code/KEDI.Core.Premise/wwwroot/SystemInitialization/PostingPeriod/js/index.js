$(document).ready(function () {
    $("#txtSearch").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#allw-gl tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#hidetr").dblclick(function () {
        $(this).hide();
    });
    $("#showtr").dblclick(function () {
        $(hidetr).show();
    });

    function onGetWhAccTemplates(success) {
        $.get("/postingperiod/getallpostingperiods", success);
    }
    let $listJE = ViewTable({
        keyField: "ID",
        selector: "#allw-gl",
        paging: {
            enabled: true,
            pageSize: 20
        },
        visibleFields: [
            "PeriodCode",
            "PeriodName",
            "PeroidStatus",
            "PostingDateFrom",
            "PostingDateTo",
            "DueDateFrom",
            "DueDateTo",
        ],
        columns: [
            {
                name: "PeriodCode",
                on: {
                    "dblclick": function (e) {
                        location.href = `/postingperiod/edit?id=${e.key}`;
                    }
                }
            }
        ]
    });
    onGetWhAccTemplates(function (postingperiod) {
        $listJE.bindRows(postingperiod);
    });
    $("#hideform").hide();
    $("#flexCheckIndeterminate").click(function () {
        if ($("#flexCheckIndeterminate").prop("checked") == true) {
            $("#hideform").show();
            $("#hideform").val(1);
        }
        if ($("#flexCheckIndeterminate").prop("checked") == false) {
            $("#hideform").hide();
        }
    })
    
})