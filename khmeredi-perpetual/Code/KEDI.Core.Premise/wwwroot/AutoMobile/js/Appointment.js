"use strict";
$(document).ready(function () {
    let _master = JSON.parse($("#masterApp").text());

    const AppServiceList = ViewTable({
        selector: "#list-service-appointment",
        keyField: "KeyID",
        indexed: true,
        visibleFields: [
            "ServiceName", "ServiceUom", "ServiceDate", "Action"
        ],
        columns: [
            {
                name: "ServiceDate",
                template: "<input type='date'>",
                on: {
                    "change": function (e) {
                        var _value = this.value;
                        $.grep(_master.AppointmentServices, function (item) {
                            if (item.KeyID == e.key) {
                                item.ServiceDate = _value;
                            }
                        });
                    }
                }
            },
        ],
        actions: [
            {
                template: "<i class='fas fa-trash-alt fn-red' style='margin: 0 50%;'></i>",
                on: {
                    "click": function (e) {
                        _master.AppointmentServices = $.grep(_master.AppointmentServices, function (item) {
                            return e.key !== item.KeyID;
                        });
                        AppServiceList.removeRow(e.key);
                        if (AppServiceList.yield().length == 0) {
                            $("#sh-save").prop("hidden", true);
                        }
                        AppServiceList.reload();
                    }
                }
            }
        ]
    });

    //Check Save Button
    function CheckSaveBtn() {
        if (AppServiceList.yield() == null) {
            $("#sh-save").prop("hidden", true);
        } else {
            $("#sh-save").prop("hidden", false);
        }
    }

    //Search Bar
    //Search Model
    $("#search-app-cus").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-customer tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    $("#search-app-vehicle").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-vehicle tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    //Search List
    $("#search-app-list-quote").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-of-quote tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    $("#search-app-list-invoice").keyup(function () {
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
    $("#search-resource-itemMaster").keyup(function () {
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

    $("#show-list-cus").click(function () {
        $.get("/Appointment/GetCustomer", function (respones) {

            var paging = new Kernel.DataPaging({
                pageSize: 5,
                style: {
                    mode: "circle",
                    border: 0
                }
            });

            paging.start(respones, function (filtered) {
                let index = filtered.current;
                if (index > 0) {
                    index = paging.setting.pageSize * (filtered.current - 1);
                }
                BindTableCus("#list-customer", filtered.data, index);
            });
            $("#paging-cus").html(paging.selfElement);

        });

    });

    function BindTableCus(selector, data, _index) {
        $("#Modal-Customer").modal("show");
        $(selector).find("tr:not(:first-child)").remove();
        if (data.length == 0) {
            $("#Modal-Customer").append("<tr><td colspan='3'class='text-center'> No Customer </td></tr>")
        } else {
            $.each(data, function (i, item) {
                _index++;
                var tr = $("<tr data-name='" + item.Name + "' data-id=" + item.ID + "></tr>").on("dblclick click", choosecustomer)
                tr.append("<td style='min-width:3px;'>" + _index + "</td>")
                    .append("<td hidden>" + item.ID + "</td>")
                    .append("<td>" + item.Code + "</td>")
                    .append("<td>" + item.Name + "</td>")
                    .append("<td>" + item.Phone + "</td>")
                $(selector).append(tr);
            });
        }
    }

    function choosecustomer(event) {
        if (event.type === "dblclick") {
            dblCus(this);
        } else {
            clickCus(this);
        }
    }

    function dblCus(c) {
        var id = $(c).data("id");
        var name = $(c).data("name");
        $("#txtcus").val(name);
        $("#Modal-Customer").modal("hide");

        $("#select-resource option").parent().val(0);

        _master.CustomerID = id;
        _master.VehicleID = 0;
        $("#txtvehicle").val("");

        $.post("/Appointment/GetVehicle", { CusID: id }, function (res) {
            if (res.length == 0) {
                $("#list-vehicle-sh").prop("hidden", true);
            } else {
                $("#list-vehicle-sh").prop("hidden", false);
            }
        });
        AppServiceList.clearRows();
        $("#sh-save").prop("hidden", true);
    }
    //Start Choose Button
    var _id_cus = 0;
    var _cus_name = "";
    function clickCus(c) {
        _id_cus = $(c).data("id");
        _cus_name = $(c).data("name");
        $(c).addClass("Active").siblings().removeClass("Active");
    }
    $("#cus-choose").click(function () {
        $("#txtcus").val(_cus_name);
        _master.CustomerID = _id_cus;
    });
    //End

    $("#show-list-vehicles").click(function () {
        $.post("/Appointment/GetVehicle", { CusID: _master.CustomerID }, function (respones) {

            var paging = new Kernel.DataPaging({
                pageSize: 5,
                style: {
                    mode: "circle",
                    border: 0
                }
            });

            paging.start(respones, function (filtered) {
                let index = filtered.current;
                if (index > 0) {
                    index = paging.setting.pageSize * (filtered.current - 1);
                }
                BindTableVehicle("#list-vehicle", filtered.data);
            });
            $("#paging-vehicle").html(paging.selfElement);

        });
    });

    function BindTableVehicle(selector, data) {
        $("#Modal-Vehicle").modal("show");
        $(selector).find("tr:not(:first-child)").remove();
        $.each(data, function (i, item) {
            i++;
            var tr = $("<tr data-name=" + item.Plate + " data-id=" + item.AutoMID + "></tr>").on("dblclick click", chooseVehicle)
            tr.append("<td>" + i + "</td>")
                .append("<td>" + item.Plate + "</td>")
                .append("<td>" + item.Engine + "</td>")
                .append("<td>" + item.Frame + "</td>")
                .append("<td>" + item.VehiTypes + "</td>")
                .append("<td>" + item.VehiBrands + "</td>")
                .append("<td>" + item.VehiModels + "</td>")
                .append("<td>" + item.VehiColors + "</td>")
                .append("<td>" + item.Year + "</td>")
            $(selector).append(tr);
        });
    }

    function chooseVehicle(event) {
        if (event.type === "dblclick") {
            var id = $(this).data("id");
            var name = $(this).data("name");
            $("#txtvehicle").val(name);
            $("#Modal-Vehicle").modal("hide");

            $("#select-resource option").parent().val(0);
            _master.VehicleID = id;
        }
    }

    $("#select-resource").change(function () {
        if (_master.CustomerID == 0) {
            $(this).val(0);
            $("#txt-warning").text("Please choose customer first!");
            $("#modal-warning").modal("show");
        } else {
            if (_master.VehicleID == 0) {
                $.post("/Appointment/ChkIfCusHasV", { CusID: _master.CustomerID }, function (_res) {
                    if (_res.CusV == "Yes") {
                        $("#select-resource option").parent().val(0);
                        $("#txt-warning").text("Please choose a vehicle of this customer!");
                        $("#modal-warning").modal("show");
                    } else {
                        var resource = $("#select-resource option").parent().val(); //value: ( 1 = from quote), (2 = from invoice), (3 = from item masterData)
                        $.post("/Appointment/GetResource", { Resource: resource, CusID: _master.CustomerID }, function (res) {
                            BindResource(resource, res);
                        });
                    }
                });
            } else {
                var resource = $("#select-resource option").parent().val(); //value: ( 1 = from quote), (2 = from invoice), (3 = from item masterData)
                $.post("/Appointment/GetResource", { Resource: resource, CusID: _master.CustomerID, AutoMID: _master.VehicleID }, function (res) {
                    BindResource(resource, res);
                });
            }
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
                btnDropServices.click(btnDropService);

                tr.append("<td class='text-center'>" + i + "</td>")
                    .append("<td>" + item.ItemName + "</td>")
                    .append("<td class='text-center'>" + item.ItemUom + "</td>")
                    .append($("<td class='text-center'></td>").append(btnDropServices))
                $(selector).append(tr);
            });
        }
        $("#modal-resource-itemMaster").modal("show");
    }

    //Drop
    //Button Drop Quote
    function btnDropQuote() {
        var row = $(this).parent().parent();
        var _qid = row.data("id");
        row.remove();

        $.post("/Appointment/DropQuote", { Id: _qid }, function (_res) {
            //Push to orignal Data
            _master.AppointmentServices.push(_res[0]);
            //Add to viewTable
            AppServiceList.addRow(_res[0]);
        });
        CheckSaveBtn();
    }

    //Button Drop Invoice
    function btnDropInvoice() {
        var row = $(this).parent().parent();
        var _invId = row.data("id");
        row.remove();

        $.post("/Appointment/DropInvoice", { Id: _invId }, function (_res) {
            //Push to orignal Data
            _master.AppointmentServices.push(_res[0]);
            //Add to viewTable
            AppServiceList.addRow(_res[0]);
        });
        CheckSaveBtn();
    }
    //Button Drop Service
    function btnDropService() {
        var row = $(this).parent().parent();
        var _itemId = row.data("id");
        row.remove();

        $.post("/Appointment/DropItemMaster", { Id: _itemId }, function (_res) {
            //Push to orignal Data
            _master.AppointmentServices.push(_res[0]);
            //Add to viewTable
            AppServiceList.addRow(_res[0]);
        });
        CheckSaveBtn();
    }

    $("#save-app").click(function () { 
        $.post("/Appointment/SaveAppointment", { appoint: JSON.stringify(_master) }, function (_res) {
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

}); //End

