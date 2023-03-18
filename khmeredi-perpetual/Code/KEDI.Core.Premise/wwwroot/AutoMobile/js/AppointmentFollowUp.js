"use strict";

$(document).ready(function () {
    let _master = JSON.parse($("#master-app").text());

    const ListFollowUp = ViewTable({
        selector: "#list-service-appointment-followup",
        keyField: "ID",
        paging: {
            enabled: true,
            pageSize: 5
        },
        visibleFields: [
            "ServiceName", "ServiceUom", "ServiceDate", "Status", "Action"
        ],
        columns: [
            {
                name: "ServiceDate",
                template: "<input type='date'>",
                on: {
                    "change": function (e) {
                        var _value = $(this).val().split("-");
                        var _date = _value[1] + "/" + _value[2] + "/" + _value[0];

                        $.grep(_master.AppointmentServices, function (item) {
                            if (item.ID != 0) {
                                if (item.ID == e.key) {
                                    item.ServiceDate = _date;
                                }
                            } else {
                                if (item.KeyID == e.data.KeyID) {
                                    item.ServiceDate = _date;
                                }
                            }
                        });
                    }
                }
            },
            {
                name: "Status",
                template: '<p class="text-center mb-0 vmcstatus"></p>'
            }
        ],
        actions: [
            {
                template: "<button class='btn btn-sm Close' style='height:15px; min-height:27px; background-color:#bf414d'> Close </button>",
                on: {
                    "click": function (e) {
                        $("#modal-warning-status-followup-close").modal("show");
                        CheckCloseConfirm(e.key, this, e.data.KeyID);
                    }
                }
            },
            {
                template: "<button class='btn btn-sm Cancel' style='height:15px; min-height:27px; background-color:#d59928'> Cancel </button>",
                on: {
                    "click": function (e) {
                        $("#modal-warning-status-followup-cancel").modal("show");
                        CheckCancelConfirm(e.key, this, e.data.KeyID);
                    }
                }
            }
            
        ]
    });

    BindDataToListFollowUp();

    //Bind first datas
    function BindDataToListFollowUp() {
        $.each(_master.AppointmentServices, function (i, item) {
            ListFollowUp.addRow(item);
            if (item.Status == "Open") {
                $(`.vmcstatus:nth(${i})`).css("color", "green");
            }
            if (item.Status == "Close") {
                $(`.vmcstatus:nth(${i})`).css("color", "red");
            }
            if (item.Status == "Cancel") {
                $(`.vmcstatus:nth(${i})`).css("color", "#e49709");
            }
            $("input[type='date']").prop("disabled", true);
        });
    }

    function CheckCloseConfirm(Id, _this, KeyId) {
        //Close Button
        $("#close-app-cf-followup").off().click(function () {
            $.each(_master.AppointmentServices, function (i, item) {
                if (item.ID != 0) {
                    if (item.ID == Id) {
                        item.Status = "Close";
                        $(`.vmcstatus:nth(${i})`).text("Close");
                        $(`.vmcstatus:nth(${i})`).css("color", "red");
                    }
                } else {
                    if (item.KeyID == KeyId) {
                        item.Status == "Close";
                        $(`.vmcstatus:nth(${i})`).text("Close");
                        $(`.vmcstatus:nth(${i})`).css("color", "red");
                    }
                }
            });
            $("#modal-warning-status-followup-close").modal("hide");
        });
    }

    function CheckCancelConfirm(Id, _this, KeyId) {
        //Cancel Button
        $("#cancel-app-cf-followup").off().click(function () {
            $.each(_master.AppointmentServices, function (i, item) {
                if (item.ID != 0) {
                    if (item.ID == Id) {
                        item.Status = "Cancel";
                        $(`.vmcstatus:nth(${i})`).text("Cancel");
                        $(`.vmcstatus:nth(${i})`).css("color", "#e49709");
                    }
                } else {
                    if (item.KeyID == KeyId) {
                        item.Status == "Cancel";
                        $(`.vmcstatus:nth(${i})`).text("Cancel");
                        $(`.vmcstatus:nth(${i})`).css("color", "#e49709");
                    }
                }
            });
            $("#modal-warning-status-followup-cancel").modal("hide");
        });
    }

    //Search Bar
    //Search List
    $("#search-app-list-quote-followUp").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-of-quote tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    $("#search-app-list-invoice-followUp").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-of-invoice tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });

    //Search Resource
    $("#search-resource-quote").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-resource-quote tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    $("#search-resource-invoice").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-resource-invoice tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    $("#search-resource-itemMaster-followUp").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-resource-itemMaster tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    //End of Search Bar

    //Clear Select
    $(".clearSelect").click(function () {
        $("#select-resource option").parent().val(0);
    });
    //End of Clear Select

    //Resource
    $("#select-resource").change(function () {
        if (_master.VehicleID == 0) {

            var resource = $("#select-resource option").parent().val(); //value: ( 1 = from quote), (2 = from invoice), (3 = from item masterData)
            $.post("/Appointment/GetResource", { Resource: resource, CusID: _master.CustomerID }, function (res) {
                BindResource(resource, res);
            });
        } else {

            var resource = $("#select-resource option").parent().val(); //value: ( 1 = from quote), (2 = from invoice), (3 = from item masterData)
            $.post("/Appointment/GetResource", { Resource: resource, CusID: _master.CustomerID, AutoMID: _master.VehicleID }, function (res) {
                BindResource(resource, res);
            });
        }
    });
    function BindResource(resource, data) {
        if (resource == 1) {
            //Resource Quote
            $("#list-of-quote tr:not(:first-child)").remove();
            if (data.length == 0) {
                $("#list-of-quote").append($('<tr></tr>').append('<td colspan="3" class="text-center text-danger"><b> Empty </b></td>'));
            } else {
                $.each(data, function (i, item) {
                    i++;

                    var tr = $('<tr data-qno="' + item.QNo + '" data-id="' + item.ID + '"></tr>');
                    var btnListQs = $("<button class='btn btn-sm btn-info jsBtnDrop'><i class='fa fa-chevron-circle-down'></i></button>");
                    btnListQs.click(btnShowListQ);

                    tr.append('<td class="text-center">' + i + '</td>')
                    tr.append('<td class="text-center">' + item.QNo + '</td>')
                    tr.append($('<td class="text-center"></td>').append(btnListQs))
                    $("#list-of-quote").append(tr);
                });
            }
            $("#modal-of-list-quote").modal("show");
        } else if (resource == 2) {
            //Resource Invoice
            $("#list-of-invoice tr:not(:first-child)").remove();
            if (data.length == 0) {
                $("#list-of-invoice").append($('<tr></tr>').append('<td colspan="3" class="text-center text-danger"><b> Empty </b></td>'));
            } else {
                $.each(data, function (i, item) {
                    i++;
                    var tr = $('<tr data-invno="' + item.ReceiptNo + '" data-id="' + item.ReceiptKvmsID + '"></tr>');
                    var btnListInvs = $("<button class='btn btn-sm btn-info jsBtnDrop'><i class='fa fa-chevron-circle-down'></i></button>");
                    btnListInvs.click(btnShowListInv);

                    tr.append('<td class="text-center">' + i + '</td>')
                    tr.append('<td class="text-center">' + item.ReceiptNo + '</td>')
                    tr.append($('<td class="text-center"></td>').append(btnListInvs))
                    $("#list-of-invoice").append(tr);
                });
            }
            $("#modal-of-list-invoice").modal("show");
        } else {
            //Resource Item Master Data
            var paging = new Kernel.DataPaging({
                pageSize: 10,
                style: {
                    mode: "circle",
                    border: 0
                }
            });

            paging.start(data, function (filtered) {
                let index = filtered.current;
                if (index > 0) {
                    index = paging.setting.pageSize * (filtered.current - 1);
                }
                BindItemMaster("#list-resource-itemMaster", filtered.data);
            });
            $("#paging-itemMaster").html(paging.selfElement);
        }
    }

    //Button Show Quote
    function btnShowListQ() {
        var quoteId = $(this).parent().parent().data("id");
        $.post("/Appointment/GetItemQuotes", { QuoteID: quoteId }, function (_res) {
            $("#list-resource-quote tr:not(:first-child)").remove();
            $.each(_res, function (i, item) {
                i++;
                let tr = $('<tr data-id="' + item.ID + '"></tr>');
                let btnDropQuotes = $("<button class='btn btn-sm btn-info jsBtnDrop'><i class='fa fa-chevron-circle-down'></i></button>");
                btnDropQuotes.click(btnDropQuote);

                tr.append('<td class="text-center">' + i + '</td>')
                tr.append('<td>' + item.KhmerName + '</td>')
                tr.append('<td class="text-center">' + item.Uom + '</td>')
                tr.append($('<td class="text-center"></td>').append(btnDropQuotes))

                $("#list-resource-quote").append(tr);
            });
        });
        var quoteNo = $(this).parent().parent().data("qno");
        $("#title-service-quote").text("Services in ( " + quoteNo + " )");
        $("#modal-resource-quote").modal("show");
    }
    //Button Show Invoice
    function btnShowListInv() {
        var invoiceId = $(this).parent().parent().data("id");
        $.post("/Appointment/GetItemInvoices", { InvoiceID: invoiceId }, function (_res) {
            $("#list-resource-invoice tr:not(:first-child)").remove();
            $.each(_res, function (i, item) {
                i++;
                let tr = $('<tr data-id="' + item.ID + '"></tr>');
                let btnDropInvoices = $("<button class='btn btn-sm btn-info jsBtnDrop'><i class='fa fa-chevron-circle-down'></i></button>");
                btnDropInvoices.click(btnDropInvoice);

                tr.append('<td class="text-center">' + i + '</td>')
                tr.append('<td>' + item.KhmerName + '</td>')
                tr.append('<td class="text-center">' + item.UnitofMeansure.Name + '</td>')
                tr.append($('<td class="text-center"></td>').append(btnDropInvoices))

                $("#list-resource-invoice").append(tr);
            });
        });
        var InvNo = $(this).parent().parent().data("invno");
        $("#title-service-invoice").text("Services in ( " + InvNo + " )");
        $("#modal-resource-invoice").modal("show");
    }
    //Button Show Item Master Data
    function BindItemMaster(selector, data) {
        $(selector).find("tr:not(:first-child)").remove();
        if (data.length == 0) {
            $("#list-resource-itemMaster").append($('<tr></tr>').append('<td colspan="4" class="text-center text-danger"><b> Empty </b></td>'));
        } else {
            $.each(data, function (i, item) {
                i++;
                let tr = $("<tr data-id=" + item.ItemID + "></tr>");
                let btnDropServices = $("<button class='btn btn-sm btn-info jsBtnDrop'><i class='fa fa-chevron-circle-down'></i></button>");
                btnDropServices.click(btnDropService)

                tr.append("<td class='text-center'>" + i + "</td>")
                    .append("<td>" + item.ItemName + "</td>")
                    .append("<td class='text-center'>" + item.ItemUom + "</td>")
                    .append($("<td class='text-center'></td>").append(btnDropServices))
                $(selector).append(tr);
            });
        }
        $("#modal-resource-itemMaster").modal("show");
    }

    //Button Drop Quote
    function btnDropQuote() {
        var row = $(this).parent().parent();
        var _qid = row.data("id");
        row.remove();

        $.post("/Appointment/DropQuote", { Id: _qid }, function (_res) {
            //Push to orignal Data
            _res[0].Status = "Open";
            _master.AppointmentServices.push(_res[0]);
            //Add to viewTable
            ListFollowUp.addRow(_res[0]);

            var index = $(`.Cancel`).length - 1;
            var indexColor = _master.AppointmentServices.length - 1;
            $(`.vmcstatus:nth(${indexColor})`).css("color", "green");
            $(`.Cancel:nth(${index})`).parent().text("");

        });
    }
    //Button Drop Invoice
    function btnDropInvoice() {
        var row = $(this).parent().parent();
        var _invId = row.data("id");
        row.remove();

        $.post("/Appointment/DropInvoice", { Id: _invId }, function (_res) {
            //Push to orignal Data
            _res[0].Status = "Open";
            _master.AppointmentServices.push(_res[0]);
            //Add to viewTable
            ListFollowUp.addRow(_res[0]);

            var index = $(`.Cancel`).length - 1;
            var indexColor = _master.AppointmentServices.length - 1;
            $(`.vmcstatus:nth(${indexColor})`).css("color", "green");
            $(`.Cancel:nth(${index})`).parent().text("");
        });
    }
    //Button Drop Service
    function btnDropService() {
        var row = $(this).parent().parent();
        
        var _itemId = row.data("id");
        row.remove();

        $.post("/Appointment/DropItemMaster", { Id: _itemId }, function (_res) {
            //Push to orignal Data
            _res[0].Status = "Open";
            _master.AppointmentServices.push(_res[0]);
            //Add to viewTable
            ListFollowUp.addRow(_res[0]);

            var index = $(`.Cancel`).length - 1;
            var indexColor = _master.AppointmentServices.length - 1;
            $(`.vmcstatus:nth(${indexColor})`).css("color", "green");
            $(`.Cancel:nth(${index})`).parent().text("");
        });
    }

    $("#save-app").click(function () {

        $.post("/Appointment/UpdateAppointment", { appoint: JSON.stringify(_master) }, function (_res) {
            if (_res.Action == -1) {
                ViewMessage({
                    summary: {
                        selector: "#validation-summary"
                    }
                }, _res);

                setTimeout(function () {
                    $("#validation-summary").text("");
                }, 5000);
            }

            if (_res.Action == 1) {
                ViewMessage({
                    summary: {
                        selector: "#validation-summary"
                    }
                }, _res);

                setTimeout(function () {
                    location.href = "/Appointment/AppointmentList";
                }, 1700);
            }
        });
    });

});