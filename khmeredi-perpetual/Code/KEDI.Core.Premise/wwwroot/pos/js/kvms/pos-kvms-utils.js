"use strict";
function UtilsKVMS(KPOS) {
    if (!(this instanceof UtilsKVMS)) {
        return new UtilsKVMS(KPOS);
    }

    const __this = this;
    var __dataset = new Warehouse();
    let tbcusquote = {};
    let __BPCustomer = JSON.parse($("#master-bus").text());

    this.fxCreateCustomer = function () {
        initFromCustomer();
    }

    this.fxCreateNewVehicle = function () {
        ClearVehicle();
        $("#model_create_v").modal("show");
    }

    this.fxListCustomer = function () {
        $("#modal-list-cus-form").modal("show");
        GetListCusV();
    }

    this.fxChangeCustomerPOS = function () {
        $("#modal-list-cus-form").modal("show");
        GetListCusV();
    }

    this.fxListQuotation = function () {
        $("#modal-list-quote").modal("show");

        $.ajax({
            url: "/KVMS/GetListQuote",
            type: "GET",
            dataType: "JSON",
            success: function (qlist) {
                if (qlist.length == 0) {
                    $("#list-quote-form tr:not(:first-child)").remove();
                    var emptydata = $("<tr></tr>").append("<td class='text-center' colspan='10'><b> Empty </b></td>");
                    $("#list-quote-form").append(emptydata);
                } else {
                    $("#list-quote-form tr:not(:first-child)").remove();
                    $.each(qlist, function (i, item) {
                        let dropQ = $("<button class='btn btn-sm btn-info' style='background-color:#4a76a7; width:100%; padding-right:6px; height:2rem; line-height:2rem; border-radius:7px; padding-left:6px; font-family: Trebuchet MS, Arial, Helvetica, sans-serif;'><i class='fa fa-chevron-circle-down'></i></button>");
                        dropQ.on("click", dropDaQ);

                        let Preview = $("<button class='btn btn-sm btn-info' style='background-color:#4a76a7; width:100%; padding-right:6px; height:2rem; line-height:2rem; border-radius:7px; padding-left:6px; font-family: Trebuchet MS, Arial, Helvetica, sans-serif;'><i class='fa fa-print'></i></button>");
                        Preview.on("click", dropDaPreview);

                        if (item.Plate == null) {
                            var tr = $("<tr data-id='" + item.ID + "'></tr>");
                            tr.append("<td class='text-left'>" + item.QNo + "</td>"),
                                tr.append("<td class='text-left'>" + item.Name + "</td>"),
                                tr.append("<td class='text-left'>" + item.Phone + "</td>"),
                                tr.append("<td colspan='5' class='text-danger text-center font-weight-bold'>" + "No Vehicles" + "</td>"),
                                tr.append($("<td></td>").append(dropQ)),
                                tr.append($("<td></td>").append(Preview))

                            $("#list-quote-form").append(tr);

                        } else {
                            var tr = $("<tr data-id='" + item.ID + "'></tr>");
                            tr.append("<td class='text-left'>" + item.QNo + "</td>"),
                                tr.append("<td class='text-left'>" + item.Name + "</td>"),
                                tr.append("<td class='text-left'>" + item.Phone + "</td>"),
                                tr.append("<td class='text-left'>" + item.Plate + "</td>"),
                                tr.append("<td class='text-left'>" + item.BrandName + "</td>"),
                                tr.append("<td class='text-left'>" + item.ModelName + "</td>"),
                                tr.append("<td class='text-left'>" + item.ColorName + "</td>"),
                                tr.append("<td class='text-left'>" + item.Year + "</td>"),
                                tr.append($("<td></td>").append(dropQ)),
                                tr.append($("<td></td>").append(Preview))
                        }

                        $("#list-quote-form").append(tr);

                    });
                }
            }
        });
    }

    this.fxListInvoice = function () {
        $("#modal-list-invoice").modal("show");

        $.get("/KVMS/GetListInvoice", function (invoice) {
            if (invoice.length == 0) {
                $("#list-invoice-form tr:not(:first-child)").remove();
                var emptydata = $("<tr></tr>").append("<td class='text-center' colspan='9'><b> Empty </b></td>");
                $("#list-invoice-form").append(emptydata);
            } else {
                $("#list-invoice-form tr:not(:first-child)").remove();
                $.each(invoice, function (i, item) {
                    let Preview = $("<button class='btn btn-sm btn-info' style='background-color:#4a76a7; width:100%; padding-right:6px; height:2rem; line-height:2rem; border-radius:7px; padding-left:6px; font-family: Trebuchet MS, Arial, Helvetica, sans-serif;'><i class='fa fa-print'></i></button>");
                    Preview.on("click", dropDaPreviewInvoice);

                    if (item.KvmsInfo.Plate == null) {
                        var tr = $("<tr data-id='" + item.ReceiptKvmsID + "'></tr>");
                        tr.append("<td>" + item.ReceiptNo + "</td>"),
                            tr.append("<td class='text-left'>" + item.KvmsInfo.Name + "</td>"),
                            tr.append("<td class='text-left'>" + item.KvmsInfo.Phone + "</td>"),
                            tr.append("<td colspan='5' class='text-danger text-center font-weight-bold'>" + "No Vehicles" + "</td>"),
                            tr.append($("<td></td>").append(Preview))

                        $("#list-quote-form").append(tr);

                    } else {
                        var tr = $("<tr data-id='" + item.ReceiptKvmsID + "'></tr>");
                        tr.append("<td class='text-left'>" + item.ReceiptNo + "</td>"),
                            tr.append("<td class='text-left'>" + item.KvmsInfo.Name + "</td>"),
                            tr.append("<td class='text-left'>" + item.KvmsInfo.Phone + "</td>"),
                            tr.append("<td class='text-left'>" + item.KvmsInfo.Plate + "</td>"),
                            tr.append("<td class='text-left'>" + item.KvmsInfo.BrandName + "</td>"),
                            tr.append("<td class='text-left'>" + item.KvmsInfo.ModelName + "</td>"),
                            tr.append("<td class='text-left'>" + item.KvmsInfo.ColorName + "</td>"),
                            tr.append("<td class='text-left'>" + item.KvmsInfo.Year + "</td>"),
                            tr.append($("<td></td>").append(Preview))
                    }

                    $("#list-invoice-form").append(tr);

                });
            }
        });
    }

    this.fxResetCusVInfo = function () {
        tbcusquote = {};
    }

    this.fxSaveQuotation = function () {
        KPOS.updateOrder(function (posInfo) {
            KPOS.validateOrder();
            posInfo.Order.OrderDetail = posInfo.Order.OrderDetailQAutoMs;

            if (posInfo.Order.OrderDetailQAutoMs == 0) {
                $("#modal-warning-save-quote").modal("show");
            } else {
                let quote;
                if (posInfo.Order.OrderID == 0) {
                    //Save Quote
                    if (Object.keys(tbcusquote).length === 0 && tbcusquote.constructor === Object) {
                        //General Cus
                        quote = {
                            ID: 0,
                            CusID: posInfo.Order.CustomerID,
                            OrderQAutoM: posInfo.Order
                        }
                    } else {
                        if (tbcusquote.AutoMID == 0) {
                            //Cus by select without vehicles
                            quote = {
                                ID: 0,
                                CusID: tbcusquote.ID,
                                OrderQAutoM: posInfo.Order
                            }
                        } else {
                            //Cus by select with vehicles
                            quote = {
                                ID: 0,
                                CusID: tbcusquote.ID,
                                AutoMID: tbcusquote.AutoMID,
                                OrderQAutoM: posInfo.Order
                            }
                        }
                    }

                    if (quote.AutoMID == undefined) {
                        $.post("/KVMS/CheckIfCuzHasV", { CusID: quote.CusID }, function (_res) {
                            if (_res.CusV == "Yes") {
                                $("#modal-warning-save-quote-hascusv").modal("show");
                            } else {
                                $.post("/KVMS/SaveKVMSQuote", { quote: JSON.stringify(quote) }, function (qid) {
                                    window.open("/KVMS/PrintQuoteCus?qid=" + qid + "", "_blank");
                                    KPOS.resetOrder();
                                });
                            }
                        });
                    } else {
                        $.post("/KVMS/SaveKVMSQuote", { quote: JSON.stringify(quote) }, function (qid) {
                            window.open("/KVMS/PrintQuoteCus?qid=" + qid + "", "_blank");
                            KPOS.resetOrder();
                        });
                    }

                } else {
                    //Update Quote

                    const datacusv = __dataset.from("tb_CusVInfo");
                    if (!(datacusv[0].AutoMId)) {
                        //Customer without vehicles
                        quote = {
                            ID: datacusv[0].QId,
                            CusID: datacusv[0].CusId,
                            QNo: datacusv[0].QNo,
                            OrderQAutoM: posInfo.Order
                        }
                    } else {
                        //Customer with vehicles
                        quote = {
                            ID: datacusv[0].QId,
                            CusID: datacusv[0].CusId,
                            AutoMID: datacusv[0].AutoMId,
                            QNo: datacusv[0].QNo,
                            OrderQAutoM: posInfo.Order
                        }
                    }

                    $.post("/KVMS/SaveKVMSQuote", { quote: JSON.stringify(quote) }, function (qid) {
                        window.open("/KVMS/PrintQuoteCus?qid=" + qid + "", "_blank");
                        KPOS.resetOrder();
                    });
                }
            }
        });
    }

    //VMC Edition
    $("#chkbox").click(function () {
        var chkbox = document.getElementById("chkbox");
        var list = document.getElementById("list_vehicle");
        if (chkbox.checked == true) {
            list.style.display = "block";
            list.scrollIntoView({ behavior: "smooth", block: "center" });
        } else {
            list.style.display = "none";
        }
    });
    function error(message) {
        $("#error-val").text(message);
        $("#show-hide").show();
        setTimeout(function () {
            $("#show-hide").hide();
        }, 3000);
    }
    function ClearVehicle() {
        $("#plateid").val("");
        $("#frameid").val("");
        $("#engineid").val("");
        $("#yearid").val("");
        $("#typeid").val(0);
        $("#brandid").val(0);
        $("#modelid").val(0);
        $("#colorid").val(0);
    }

    //Search Bar
    $("#search-listCus").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-cus-form-pos tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    $("#search-listV").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-v-cus-form-pos tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    $("#search-listQuote").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-quote-form tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    $("#search-listInvoice").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-invoice-form tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });

    function initFromCustomer() {
        let create_customer = new DialogBox(
            {
                icon: "far fa-list-alt",
                close_button: true,
                content: {
                    selector: "#create-customer-content",
                },
                caption: "Create Customer",
                position: "top-center",
                type: "ok-cancel",
                button: {
                    ok: {
                        text: "Add",
                        callback: function (e) {
                            let content = this.meta.content;
                            if (content.find('.customer-code').val() == null || content.find('.customer-code').val() == '') {

                                $('.customer-code-validate').text('Code required !');
                                $("#form-customer").show();
                                setTimeout(function () {
                                    $("#form-customer").hide();
                                }, 3000);
                                content.find('.customer-code').focus();
                            }
                            else if (content.find('.customer-name').val() == null || content.find('.customer-name').val() == '') {

                                $('.customer-code-validate').text('Name required !');
                                $("#form-customer").show();
                                setTimeout(function () {
                                    $("#form-customer").hide();
                                }, 3000);
                                content.find('.customer-name').focus();
                            }
                            else if (content.find('.price-list-id').val() == null || content.find('.price-list-id').val() == '' || content.find('.price-list-id').val() == 0) {

                                $('.customer-code-validate').text('Price list required !');
                                $("#form-customer").show();
                                setTimeout(function () {
                                    $("#form-customer").hide();
                                }, 3000);
                                content.find('.price-list-id').focus();
                            }
                            else if (content.find('.customer-type').val() == null || content.find('.customer-type').val() == '' || content.find('.customer-type').val() == 0) {

                                $('.customer-code-validate').text('Type required !');
                                $("#form-customer").show();
                                setTimeout(function () {
                                    $("#form-customer").hide();
                                }, 3000);
                                content.find('.customer-type').focus();
                            }
                            else if (content.find('.customer-phone').val() == null || content.find('.customer-phone').val() == '') {

                                $('.customer-code-validate').text('Phone required !');
                                $("#form-customer").show();
                                setTimeout(function () {
                                    $("#form-customer").hide();
                                }, 3000);
                                content.find('.customer-phone').focus();
                            }
                            else if (content.find('.customer-address').val() == null || content.find('.customer-address').val() == '') {

                                $('.customer-code-validate').text('Address required !');
                                $("#form-customer").show();
                                setTimeout(function () {
                                    $("#form-customer").hide();
                                }, 3000);
                                content.find('.customer-address').focus();
                            }
                            else {
                                createCustomer(content);
                            }
                        }
                    },
                    cancel: {
                        callback: function (e) {
                            this.meta.shutdown();
                        }
                    }
                }
            }
        );

        create_customer.startup("after", function (dialog) {
            let content = dialog.content;
            content.find(".customer-code").focus();
        });

        const Vlist = ViewTable({
            selector: create_customer.content.find("#list-body-vehicle"),
            keyField: "KeyID",
            visibleFields: [
                "Plate", "Frame", "Engine", "VehiTypes", "VehiBrands", "VehiModels", "Year", "VehiColors"
            ],
            columns: [
                {
                    name: "Plate",
                    template: "<input class='vmcinput'>",
                    on: {
                        "keyup": function (e) {
                            var _value = this.value;
                            $.grep(__BPCustomer.AutoMobile, function (item) {
                                if (item.KeyID == e.key) {
                                    item.Plate = _value;
                                }
                            })
                        }
                    }
                },
                {
                    name: "Frame",
                    template: "<input class='vmcinput'>",
                    on: {
                        "keyup": function (e) {
                            var _value = this.value;
                            $.grep(__BPCustomer.AutoMobile, function (item) {
                                if (item.KeyID == e.key) {
                                    item.Frame = _value;
                                }
                            })
                        }
                    }
                },
                {
                    name: "Engine",
                    template: "<input class='vmcinput'>",
                    on: {
                        "keyup": function (e) {
                            var _value = this.value;
                            $.grep(__BPCustomer.AutoMobile, function (item) {
                                if (item.KeyID == e.key) {
                                    item.Engine = _value;
                                }
                            })
                        }
                    }
                },
                {
                    name: "VehiTypes",
                    template: "<select class='vmcselect'>",
                    on: {
                        "change": function (e) {
                            var _value = this.value;
                            $.grep(__BPCustomer.AutoMobile, function (item) {
                                if (item.KeyID == e.key) {
                                    item.TypeID = parseInt(_value);
                                }
                            });
                        }
                    }
                },
                {
                    name: "VehiBrands",
                    template: "<select class='vmcselect'>",
                    on: {
                        "change": function (e) {
                            var _value = this.value;
                            $.grep(__BPCustomer.AutoMobile, function (item) {
                                if (item.KeyID == e.key) {
                                    item.BrandID = parseInt(_value);
                                }
                            })
                        }
                    }
                },
                {
                    name: "VehiModels",
                    template: "<select class='vmcselect'>",
                    on: {
                        "change": function (e) {
                            var _value = this.value;
                            $.grep(__BPCustomer.AutoMobile, function (item) {
                                if (item.KeyID == e.key) {
                                    item.ModelID = parseInt(_value);
                                }
                            })
                        }
                    }
                },
                {
                    name: "Year",
                    template: "<input class='vmcinput'>",
                    on: {
                        "keyup": function (e) {
                            var _value = this.value;
                            $.grep(__BPCustomer.AutoMobile, function (item) {
                                if (item.KeyID == e.key) {
                                    item.Year = _value;
                                }
                            })
                        }
                    }
                },
                {
                    name: "VehiColors",
                    template: "<select class='vmcselect'>",
                    on: {
                        "change": function (e) {
                            var _value = this.value;
                            $.grep(__BPCustomer.AutoMobile, function (item) {
                                if (item.KeyID == e.key) {
                                    item.ColorID = parseInt(_value);
                                }
                            })
                        }
                    }
                }
            ]
        });

        $("#drop_down_modal").on("click", function () {
            var keyID = Date.now();
            var plate = $("#plateid").val();
            var frame = $("#frameid").val();
            var engine = $("#engineid").val();
            var year = $("#yearid").val();
            var typeID = $("#typeid option:selected").val();
            var brandID = $("#brandid option:selected").val();
            var modelID = $("#modelid option:selected").val();
            var colorID = $("#colorid option:selected").val();

            if (plate.trim() == "") {
                error("Please input plate");
                return;
            }
            if (frame.trim() == "") {
                error("Please input frame");
                return;
            }
            if (engine.trim() == "") {
                error("Please input engine");
                return;
            }
            if (typeID == 0) {
                error("Please select type");
                return;
            }
            if (brandID == 0) {
                error("Please select brand");
                return;
            }
            if (modelID == 0) {
                error("Please select model");
                return;
            }
            if (year.trim() == "") {
                error("Please input year");
                return;
            }
            if (colorID == 0) {
                error("Please select color");
                return;
            }


            var item = {
                KeyID: keyID,
                AutoMID: 0,
                Plate: plate,
                Frame: frame,
                Engine: engine,
                TypeID: typeID,
                BrandID: brandID,
                ModelID: modelID,
                Year: year,
                ColorID: colorID
            }

            __BPCustomer.AutoMobile.push(item);

            $.ajax({
                url: "/BusinessPartner/GetBusDetail",
                type: "POST",
                dataType: "JSON",
                data: { data: item },
                success: function (_detail) {
                    Vlist.addRow(_detail);
                }
            });
            ClearVehicle();
        });
    };

    function GetListCusV() {
        $.ajax({
            url: "/KVMS/GetListCusForm",
            type: "GET",
            dataType: "JSON",
            success: function (_res) {
                $("#list-cus-form-pos tr:not(:first-child)").remove();
                $.each(_res, function (i, item) {
                    let dropBtn = $("<button class='btn btn-sm btn-info' style='background-color:#4a76a7; width:100%; padding-right:6px; height:2rem; line-height:2rem; border-radius:7px; padding-left:6px; font-family: Trebuchet MS, Arial, Helvetica, sans-serif;'><i class='fa fa-chevron-circle-down'></i></button>");
                    dropBtn.on("click", dropDaCus);
                    var tr = $("<tr data-id='" + item.ID + "'></tr>");
                    tr.append("<td>" + item.Code + "</td>"),
                        tr.append("<td>" + item.Name + "</td>"),
                        tr.append("<td>" + item.Phone + "</td>"),
                        tr.append("<td>" + item.Email + "</td>"),
                        tr.append("<td>" + item.Address + "</td>"),
                        tr.append($("<td style='text-align:center!important'></td>").append(dropBtn))

                    $("#list-cus-form-pos").append(tr);
                });
            }
        });
    }

    function dropDaCus() {
        var id = $(this).parent().parent().data("id");
        $.ajax({
            url: "/KVMS/CheckVCus",
            type: "POST",
            dataType: "JSON",
            data: { id: id },
            success: function (res) {
                if (res.status == "Y") {
                    $("#modal-list-v-cus-form").on("show.bs.modal", function () {
                        $("#modal-list-cus-form").modal("hide");
                    });

                    $("#modal-list-v-cus-form").modal("show");

                    $.ajax({
                        url: "/KVMS/GetVCus",
                        type: "POST",
                        dataType: "JSON",
                        data: { id: id },
                        success: function (resV) {
                            $("#list-v-cus-form-pos tr:not(:first-child)").remove();
                            $.each(resV, function (i, item) {
                                let dropV = $("<button class='btn btn-sm btn-info' style='background-color:#4a76a7; width:100%; padding-right:6px; height:2rem; line-height:2rem; border-radius:7px; padding-left:6px; font-family: Trebuchet MS, Arial, Helvetica, sans-serif;'><i class='fa fa-chevron-circle-down'></i></button>");
                                dropV.on("click", dropDaV);

                                var tr = $("<tr data-id='" + item.AutoMID + "'></tr>");
                                tr.append("<td>" + item.Plate + "</td>"),
                                    tr.append("<td>" + item.Frame + "</td>"),
                                    tr.append("<td>" + item.Engine + "</td>"),
                                    tr.append("<td>" + item.TypeName + "</td>"),
                                    tr.append("<td>" + item.BrandName + "</td>"),
                                    tr.append("<td>" + item.ModelName + "</td>"),
                                    tr.append("<td>" + item.Year + "</td>"),
                                    tr.append("<td>" + item.ColorName + "</td>"),
                                    tr.append($("<td></td>").append(dropV))

                                $("#list-v-cus-form-pos").append(tr);
                            });
                        }
                    });

                } else {
                    $.ajax({
                        url: "/KVMS/GoChooseCus",
                        type: "POST",
                        dataType: "JSON",
                        data: { id: id },
                        success: function (_partner) {
                            $('.change-customer').text(_partner.Name);
                            KPOS.updateOrder(function (orderInfo) {
                                orderInfo.Order.CustomerID = _partner.ID;
                                orderInfo.Setting.CustomerID = _partner.ID;
                                orderInfo.Setting.PriceListID = _partner.PriceListID;
                                
                                $.post("/KVMS/UpdateSetting", { setting: orderInfo.Setting }, function () {
                                    KPOS.resetOrder();
                                });
                            });

                            //Get only customerId to get pay
                            //Action of POS: Click list of customer then pay
                            KPOS.getCID(_partner.ID);

                            tbcusquote = {};
                            tbcusquote = {
                                ID: _partner.ID,
                                AutoMID: 0
                            };
                        }
                    });
                    $("#modal-list-cus-form").modal("toggle");
                }
            }

        });
    }
    function dropDaV() {
        var id = $(this).parent().parent().data("id");
        $.ajax({
            url: "/KVMS/ChooseCusV",
            type: "POST",
            dataType: "JSON",
            data: { id: id },
            success: function (_partner) {
                $('.change-customer').text(_partner.Name);
                KPOS.fallbackInfo(function (orderInfo) {
                    orderInfo.Order.CustomerID = _partner.ID;
                    orderInfo.Setting.CustomerID = _partner.ID;
                    orderInfo.Setting.PriceListID = _partner.PriceListID;

                    //AutoMobile
                    orderInfo.AutoMobile.AutoID = _partner.AutoMobile[0].AutoMID;

                    $.post("/KVMS/UpdateSetting", { setting: orderInfo.Setting }, function () {
                        KPOS.resetOrder();
                    });                  
                });

                //Get customerId and vehicleId to get pay
                //Action of POS: Click list of customer then choose vehicle then pay
                KPOS.getCID(_partner.ID);
                KPOS.getVID(id);

                tbcusquote = {};
                tbcusquote = {
                    ID: _partner.ID,
                    AutoMID: _partner.AutoMobile[0].AutoMID
                };
            }
        });
       
        $("#modal-list-v-cus-form").modal("hide");
    }

    //Save
    function createCustomer(content) {

        KPOS.loadScreen();
        __BPCustomer.ID = 0,
            __BPCustomer.Code = content.find('.customer-code').val(),
            __BPCustomer.Name = content.find('.customer-name').val(),
            __BPCustomer.PriceListID = content.find('.price-list-id').val(),
            __BPCustomer.Type = "Customer",
            __BPCustomer.Phone = content.find('.customer-phone').val(),
            __BPCustomer.Email = content.find('.customer-email').val(),
            __BPCustomer.Address = content.find('.customer-address').val(),
            __BPCustomer.Delete = false

        $.ajax({
            url: '/KVMS/CreateCustomer',
            type: 'POST',
            data: $.antiForgeryToken({ business: JSON.stringify(__BPCustomer) }),
            dataType: 'JSON',
            success: function (response) {
                if (response.ID <= 0) {

                    $('.customer-code-validate').text('Code already being used...!');
                    $("#form-customer").show();
                    setTimeout(function () {
                        $("#form-customer").hide();
                    }, 5555);
                    content.find('.customer-code').focus();
                    KPOS.loadScreen(false);
                }
                else {

                    //KPOS.updateOrder(function (orderInfo) {
                    //    orderInfo.Order.CustomerID = response.ID;
                    //});
                    KPOS.loadScreen(false);
                    clearCreateCustomer(content);
                    location.reload();
                }

            }
        });
    };
    function clearCreateCustomer(content) {
        content.find('.customer-code').focus();
        content.find('.customer-code').val('');
        content.find('.customer-name').val('');
        //PriceListID: setting.PriceListID;
        PriceListID: content.find('.price-list-id').val(0);
        Type: "";
        Phone: content.find('.customer-phone').val('');
        Email: content.find('.customer-email').val('');
        Address: content.find('.customer-address').val('');
        //setTimeout(function () {
        //    $('.customer-code-validate').text('').css("color", "red");
        //},1000);
    };

    function dropDaQ() {

        $("#fx-save-quotation").html("<i class='fa fa-save'></i> Update & Preview Quote ");
        var quoteid = $(this).parent().parent().data("id");
        $.post("/KVMS/GetCusVInfo", { QID: quoteid }, function (res) {
            __dataset.table("tb_CusVInfo").clear();
            __dataset.insert("tb_CusVInfo", res, "QId");
            $('.change-customer').text(res.CusName);

            KPOS.getCID(res.CusId);
            KPOS.getSQNo(res.QNo);
        });
        KPOS.setOrderofQuote(1, quoteid);
    }

    function dropDaPreview() {
        var QID = $(this).parent().parent().data("id");
        window.open("/KVMS/PrintQuoteCus?qid=" + QID + "", "_blank");
    }

    function dropDaPreviewInvoice() {
        var RID = $(this).parent().parent().data("id");
        window.open("/KVMS/PrintInvoice?Invid=" + RID + "", "_blank");
    }

}
