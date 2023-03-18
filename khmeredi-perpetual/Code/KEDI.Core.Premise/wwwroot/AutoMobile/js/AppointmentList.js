"use strict";
$(document).ready(function () {
    var appMaster = JSON.parse($("#_listApp").text());

    DataPagingAP();

    function DataPagingAP() {
        var paging = new DataPaging({
            pageSize: 10,
            style: {
                border: 1
            }
        });

        paging.render(appMaster, function (filtered) {
            let index = filtered.current;
            if (index > 0) {
                index = paging.setting.pageSize * (filtered.current - 1);
            }
            BindTableApp("#list-app", filtered.dataset);
        });
        paging.appendTo($("#paging-app"));
    }

    function BindTableApp(selector, data) {
        $(selector).find("tr:not(:first-child)").remove();
        if (data.length == 0) {
            $(selector).append("<tr><td colspan='9'class='text-center'> Empty </td></tr>")
        } else {
            if (selector == "#list-app") {
                $.each(data, function (i, item) {
                    i++;
                    //Btn
                    let btnFollowup = $("<a class='btn btn-sm btn-info text-light' style='cursor:pointer'>Follow up</a>").click(ButtonFollowUp);
                    let btnModelWarning = $("<a class='btn btn-sm btn-danger text-light' style='cursor:pointer'>Cancel</a>").click(ButtonModelWarning);

                    if (item.Status == "Open") {
                        var statusOpen = "<a class='text-success font-weight-bold' style='cursor: default;'>Open</a>"
                        BindDataAppointment(selector, i, item, btnFollowup, btnModelWarning, statusOpen);
                    } else {
                        if (item.Status == "Cancel") {
                            var statusCancel = "<a class='font-weight-bold' style='color:#e49709;cursor: default;'>Cancel</a>"
                            BindDataAppointment(selector, i, item, btnFollowup, btnModelWarning, statusCancel);
                        } else {
                            var statusClose = "<a class='text-danger font-weight-bold' style='cursor: default;'>Close</a>"
                            BindDataAppointment(selector, i, item, btnFollowup, btnModelWarning, statusClose);
                        }
                    }
                });
            }
        }
    }

    function BindDataAppointment(selector, index, item, btnFollowup, btnModelWarning, Status) {
        if (item.Plate == null) {
            var tr = $("<tr data-id=" + item.ID + "></tr>")
            tr.append("<td class='text-center'>" + index + "</td>")
                .append("<td>" + item.CusName + "</td>")
                .append("<td>" + item.PhoneCus + "</td>")
                .append("<td colspan='3' class='text-center text-danger font-weight-bold'> No Vehicle </td>")
                .append($("<td class='text-center'></td>").append("<i class='fa fa-bell text-danger fa-lg'><span class='count'>" + item.Notification + "</span></i>"))
                .append($("<td class='text-center'></td>").append(Status))
                .append($('<td class="text-center"></td>').append(btnFollowup).append(" ✦ ").append(btnModelWarning))
            $(selector).append(tr);
        } else {
            var tr = $("<tr data-id=" + item.ID + "></tr>")
            tr.append("<td class='text-center'>" + index + "</td>")
                .append("<td>" + item.CusName + "</td>")
                .append("<td>" + item.PhoneCus + "</td>")
                .append("<td>" + item.Plate + "</td>")
                .append("<td>" + item.Brand + "</td>")
                .append("<td>" + item.Model + "</td>")
                .append($("<td class='text-center'></td>").append("<i class='fa fa-bell text-danger fa-lg'><span class='count'>" + item.Notification + "</span></i>"))
                .append($("<td class='text-center'></td>").append(Status))
                .append($('<td class="text-center"></td>').append(btnFollowup).append(" ✦ ").append(btnModelWarning))
            $(selector).append(tr);
        }
    }

    function ButtonFollowUp() {
        var id = $(this).parent().parent().data("id");
        location.href = `/Appointment/AppointmentFollowUp?AppID=${id}`;
    }

    let appID = 0;
    function ButtonModelWarning() {
        $("#modal-warning").modal("show");
        var id = $(this).parent().parent().data("id");
        appID = id;
    }
    $("#cancel-app-cf").click(function () {
        $.post("/Appointment/CancelAppointment", { AppID: appID }, function () {
            location.reload();
        });
    });

    //Search Box
    $("#search-app").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-app tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });

    //Create Appiontment
    $("#go-to-create-app").click(function () {
        $.get("/Appointment/CheckAlertBeforeApp", function (_res) {
            if (_res.errormess == 1) {
                $("#modal-config-alert-warning").modal("show");
            } else {
                location.href = "/Appointment/Appointment";
            }
        });
    });

    //Go to config Alert Management
    $("#go-to-config-amSetting").click(function () {
        location.href = "/AlertManagement/AlertManagement";
    });
});