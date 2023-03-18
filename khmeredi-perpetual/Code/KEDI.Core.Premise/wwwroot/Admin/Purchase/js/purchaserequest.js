const ___userid = parseInt($("#userID").text());
const ___branchid = parseInt($("#branchID").text());
let __PO = JSON.parse($("#data-invoice").text()),
    _currency = "",
    _currencyID = 0,
    itemMaster = {},
    orderDetail = [],
    ExChange = [],
    freights = [],
    freightMaster = {},
    disSetting = __PO.genSetting.Display;


$(document).ready(function () {
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });
    var now = new Date();
    document.getElementById("txtPostingdate").valueAsDate = now;
    document.getElementById("txtDelivery").valueAsDate = now;
    document.getElementById("txtDocumentDate").valueAsDate = now;

    $("#addfontawesome").addClass("fa-percent");
    $(".btnfind_new").text("find");
    $("#txtstatus").val("open");
    __PO.seriesPO.forEach(i => {
        if (i.Default == true) {
            $("#DocumentTypeID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    })
    itemMaster = {
        ID: 0,
        SubTotal: 0,
        SubTotalSys: 0,
        BalanceDue: 0,
        BalanceDueSys: 0,
        PurCurrencyID: 0,
        LocalCurID: 0,
        SysCurrencyID: 0,
        DiscountRate: 0,
        DiscountValue: 0,
        TaxRate: 0,
        TaxValue: 0,
        TypeDis: "Percent",
        TaxRate: 0,
        TaxValue: 0,
        DownPayment: 0,
        AppliedAmount: 0,
        RequesterID: 0,
    }
     // Get Mutibranch
     $.get("/Branch/GetMultiBranch",function(res){
        let data='<option selected value="0">--- No Branch ---</option>';
        if(res.length>0)
        {
            data="";
           $.each(res,function(i,item){
                   if(item.Active)
                        data+=`<option selected value="${item.ID}">${item.Name}</option>`;
                   else
                        data+=`<option value="${item.ID}">${item.Name}</option>`;
           });
           
        }
        $("#branch").append(data);       
   });

    //Get Business Partner
    $(".show-list-requester").click(function () {
        $(this).prop("disabled", false);
        $.ajax({
            url: "/PurchaseRequest/GetRequesters",
            type: "Get",
            dataType: "Json",
            success: function (response) {
                bindVendor(response);
            }
        });
    });

    // bind Vendor
    function bindVendor(response) {
        const vendorDialog = new DialogBox({
            content: {
                selector: ".requester-container-list"
            },
            caption: "Requester Lists",
        });

        vendorDialog.invoke(function () {
            const venTable = ViewTable({
                keyField: "ID",
                selector: ".list-requester",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Username", "Code", "Name"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down font-size hover"></i>`,
                        on: {
                            "click": function (e) {
                                $(".show-list-requester").prop("disabled", false);
                                $("#requester-id").val(e.data.Username);
                                $("#requester-name").val(e.data.Name);
                                $("#requester-code").val(e.data.Code);
                                itemMaster.RequesterID = e.data.ID;
                                vendorDialog.shutdown();
                            }
                        }
                    }
                ]
            });
            venTable.bindRows(response);
            $("#find-cus").on("keyup", function (e) {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let rex = new RegExp(__value, "gi");
                let __vendors = $.grep(response, function (person) {
                    return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.Username.toLowerCase().replace(/\s+/, "").match(rex);
                });
                venTable.bindRows(__vendors);
            });
        });
        vendorDialog.confirm(function () {
            vendorDialog.shutdown();
            $(".show-list-requester").prop("disabled", false);
        })
    }
    //get currency
    $.ajax({
        url: "/PurchaseRequest/GetCurrencyDefualt",
        type: "GET",
        dataType: "JSON",
        success: function (res) {
            const e = res[0];
            $(".downpayment_cur").text(e.Description);
            $(".applied_amount_cur").text(e.Description);
            $(".Additional_cur_expense").text(e.Description);
            $(".Return_cur").text(e.Description);
            $(".sub_tota_cur").text(e.Description);
            $(".balance_due_cur").text(e.Description);
            $(".cur-class").text(e.Description);
            _currency = e.Description
            _currencyID = e.ID;
            itemMaster.ExchangeRate = e.ExchangeRate;
            $("#txtExchange").val(1 + " " + _currency + " = " + 1 + " " + e.Description);
        }
    });
    //Invoice
    let selected = $("#txtInvoice");
    selectSeries(selected);

    $('#txtInvoice').change(function () {
        var id = ($(this).val());
        var series = find("ID", id, __PO.seriesPO);
        __PO.seriesPO.Number = series.NextNo;
        __PO.seriesPO.ID = id;
        $("#DocumentTypeID").val(series.DocumentTypeID);
        $("#number").val(series.NextNo);
        $("#next_number").val(series.NextNo);
    });
    if (__PO.seriesPO.length == 0) {
        $('#txtInvoice').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
        $(".btn_ADD").prop("disabled", true);
    }
    $("#txtcurrency").change(function () {
        const __this = this;
        $.ajax({
            url: "/PurchaseRequest/GetFilterLocaCurrency",
            type: "Get",
            dataType: "Json",
            data: { CurrencyID: this.value },
            success: function (res) {
                if (__this.value != 0) $("#chooseItem").prop("disabled", false)
                else $("#chooseItem").prop("disabled", true)
                $.get("/PurchaseRequest/GetDisplayFormatCurrency", { curId: res[0].CurrencyID }, function (resp) {
                    disSetting = resp.Display;
                    itemMaster.ExchangeRate = res[0].Rate;
                    itemMaster.SaleCurrencyID = res[0].CurrencyID;
                    itemMaster.PurCurrencyID = res[0].CurrencyID;
                    $("#txtExchange").val(1 + " " + _currency + " = " + res[0].SetRate + " " + res[0].Currency.Description);
                    $(".cur-class").text(res[0].Currency.Description);
                    // change currency name in the rows of items after choosed ///
                    $(".cur").text(res[0].Currency.Description);
                })

            }
        });
    });
    //// Bind Item Choosed ////
    const $listItemChoosed = ViewTable({
        keyField: "LineIDUN",
        selector: $("#list-items"),
        indexed: true,
        paging: {
            pageSize: 20,
            enabled: false
        },
        dynamicCol: {
            afterAction: true,
            headerContainer: "#col-to-append-after-detail",
        },
        visibleFields: [
            "Code", "PurchasPrice", "Barcode", "Qty", "TaxGroupSelect", "VendorName", "VendorCode", "RequiredDate", "TaxRate", "TaxValue", "TotalWTax", "Total",
            "UoMSelect", "ItemName", "DiscountValue", "DiscountRate", "FinDisRate", "FinDisValue", "TaxOfFinDisValue", "FinTotalValue", "Remark"
        ],
        columns: [
            {
                name: "VendorCode",
                template: `<input class='form-control font-size hover' type='text' readonly/>`,
                on: {
                    "click": function (e) {
                        chooseVendor($listItemChoosed, e);
                    }
                }
            },
            {
                name: "RequiredDate",
                template: `<input class='form-control font-size' type='date'/>`,
                on: {
                    "change": function (e) {

                    }
                }
            },
            {
                name: "PurchasPrice",
                template: `<input class='form-control font-size unitprice' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "PurchasPrice", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this.value);
                        setSummary(orderDetail);
                        disInputUpdate(itemMaster);
                    }
                }
            },
            {
                name: "DiscountValue",
                template: `<input class='form-control font-size disvalue' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        calDisValue(e, this);
                        setSummary(orderDetail);
                        disInputUpdate(itemMaster);
                    }
                }
            },
            {
                name: "DiscountRate",
                template: `<input class='form-control font-size disrate' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        calDisRate(e, this);
                        setSummary(orderDetail);
                        disInputUpdate(itemMaster);
                    }
                }
            },
            {
                name: "Qty",
                template: `<input class='form-control font-size qty' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        if (e.data.IsLimitOrder) {
                            if (this.value < e.data.MinOrderQty) {
                                //new DialogBox({
                                //    content: `Qty is less than minimun planning data!`,
                                //    caption: "Qty checking"
                                //});
                                this.value = e.data.MinOrderQty;
                            }
                            if (this.value > e.data.MaxOrderQty) {
                                //new DialogBox({
                                //    content: `Qty is more than maximun planning data!`,
                                //    caption: "Qty checking"
                                //});
                                this.value = e.data.MaxOrderQty;
                            }
                        }
                        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "Qty", this.value);
                        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "OpenQty", this.value);
                        //calDisrate(e);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this.value);
                        setSummary(orderDetail);
                        disInputUpdate(itemMaster);
                    }
                }
            },
            {
                name: "TaxGroupSelect",
                template: `<select class='form-control font-size taxgroup'></select>`,
                on: {
                    "change": function (e) {
                        const taxg = find("ID", this.value, e.data.TaxGroups);
                        if (!!taxg) {
                            updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxRate", taxg.Rate);

                        } else {
                            updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxRate", 0);
                        }
                        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxGroupID", parseInt(this.value))
                        totalRow($listItemChoosed, e, this.value);
                        setSummary(orderDetail);
                        disInputUpdate(itemMaster);
                    }
                }
            },
            {
                name: "UoMSelect",
                template: `<select class='form-control font-size uom'></select>`,
                on: {
                    "change": function (e) {
                        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "UomID", this.value);
                    }
                }
            },
            {
                name: "Remark",
                template: `<input class='form-control font-size remark' />`,
                on: {
                    "change": function (e) {
                        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "Remark", this.value);
                    }
                }
            },
            {
                name: "AddictionProps",
                valueDynamicProp: "ValueName",
                dynamicCol: true,
            }
        ],
        actions: [
            {
                template: `<i class="fas fa-trash font-size hover"></i>`,
                on: {
                    "click": function (e) {
                        $listItemChoosed.removeRow(e.key);
                        orderDetail = orderDetail.filter(i => i.LineIDUN !== e.key);
                        setSummary(orderDetail);
                        disInputUpdate(itemMaster);
                        //$("#source-copy").val("");
                    }
                }
            }
        ]
    });
    $("#chooseItem").click(function () {
        itemMasterDataDialog();
    });
    // set date reqiure
    $("#txtPostingdate").change(function () {
        $listItemChoosed.updateColumn(undefined, "RequiredDate", this.value)
    })
    // Freight Dialog //
    $("#freight-dailog").click(function () {
        const freightDialog = new DialogBox({
            content: {
                selector: "#container-list-freight"
            },
            caption: "Freight",
        });
        freightDialog.invoke(function () {
            if (freightMaster.IsEditabled) {
                freightsEditable(freightDialog);
            } else {
                freightsNoneEditable(freightDialog);
            }
        });
        freightDialog.confirm(function () {
            $("#freight-value").val(num.formatSpecial(freightMaster.ExpenceAmount, disSetting.Prices));
            setSummary(orderDetail);
            freightDialog.shutdown();
        });
    });
    getFreight(function (res) {
        freightMaster = res;
        freightMaster.IsEditabled = true;
        if (isValidArray(res.FreightPurchaseDetailViewModels)) {
            if (isValidArray(freights)) {
                freights = [];
                res.FreightPurchaseDetailViewModels.forEach(i => {
                    freights.push(i);
                });
            }
            else {
                res.FreightPurchaseDetailViewModels.forEach(i => {
                    freights.push(i);
                });
            }
        }
    });

    /// dis rate on invoice //
    $("#dis-rate-id").keyup(function () {
        $(this).asNumber();
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > 100) this.value = 100;
        const subtotal = num.toNumberSpecial($("#sub-id").val());
        const disvalue = (num.toNumberSpecial(this.value) * subtotal) === 0 ? 0 : (num.toNumberSpecial(this.value) * subtotal) / 100;
        $("#dis-value-id").val(num.toNumberSpecial(disvalue, disSetting.Prices));
        invoiceDisAllRowRate(this);
        // if (parseFloat(disvalue) > 0) {
        //     const totalAfDis = subtotal - num.toNumberSpecial(disvalue) + num.toNumberSpecial(itemMaster.TaxValue);
        //     $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        //     $("#sub-after-dis").val(num.formatSpecial(totalAfDis - num.toNumberSpecial(itemMaster.TaxValue), disSetting.Prices));
        //     itemMaster.BalanceDue = totalAfDis;
        //     itemMaster.DiscountValue = disvalue;
        //     itemMaster.DiscountRate = this.value;
        //     itemMaster.BalanceDueSys = totalAfDis * itemMaster.ExchangeRate;
        // } else {
        //     setSummary(orderDetail);
        // }
    });
    /// dis value on invoice //
    $("#dis-value-id").keyup(function () {
        $(this).asNumber();
        const subtotal = num.toNumberSpecial($("#sub-id").val());
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > subtotal) this.value = subtotal;

        const disrate = (num.toNumberSpecial(this.value) * 100 === 0) || (subtotal === 0) ? 0 :
            (num.toNumberSpecial(this.value) * 100) / subtotal;
        $("#dis-rate-id").val(num.formatSpecial(disrate, disSetting.Rates));
        invoiceDisAllRowValue(disrate);
        // if (parseFloat(disrate) > 0) {
        //     const totalAfDis = subtotal - num.toNumberSpecial(this.value) + num.toNumberSpecial(itemMaster.TaxValue);
        //     $("#sub-after-dis").val(num.formatSpecial(totalAfDis - num.toNumberSpecial(itemMaster.TaxValue), disSetting.Prices));
        //     $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        //     itemMaster.BalanceDue = totalAfDis;
        //     itemMaster.DiscountValue = this.value;
        //     itemMaster.DiscountRate = disrate;
        //     itemMaster.BalanceDueSys = totalAfDis * itemMaster.ExchangeRate;
        // } else {
        //     setSummary(orderDetail);
        // }
    });


    let masterid = 0;
    $("#submit-data").click(function () {
        freightMaster.FreightPurchaseDetials = freights.length === 0 ? new Array() : freights;
        itemMaster.ID = masterid;
        itemMaster.ReffNo = $("#txtreff_no").val();
        itemMaster.PostingDate = $("#txtPostingdate").val();
        itemMaster.DeliveryDate = $("#txtDelivery").val();
        itemMaster.DocumentDate = $("#txtDocumentDate").val();
        itemMaster.Remark = $("#txtRemark").val();
        itemMaster.PurchaseRequestDetails = orderDetail.length === 0 ? new Array() : orderDetail;
        itemMaster.FreightPurchaseView = freightMaster;
        itemMaster.SeriesDetailID = parseInt($("#SeriesDetailID").val());
        itemMaster.SeriesID = parseInt($("#txtInvoice").val());
        itemMaster.DocumentTypeID = parseInt($("#DocumentTypeID").val());
        itemMaster.Number = parseInt($("#next_number").val());
        itemMaster.FrieghtAmount = $("#freight-value").val();
        itemMaster.FrieghtAmountSys = num.toNumberSpecial(itemMaster.FrieghtAmount) * num.toNumberSpecial(itemMaster.ExchangeRate);
        itemMaster.PurRate = itemMaster.ExchangeRate;
        itemMaster.BranchID = parseInt($("#branch").val());

        //return;
        const type = $("#txttype").text();
        var dialogSubmit = new DialogBox({
            content: "Are you sure you want to save the item?",
            type: "yes-no",
            icon: "warning"
        });
        dialogSubmit.confirm(function () {
            $("#loading").prop("hidden", false);
            $.ajax({
                url: "/PurchaseRequest/SavePurchaseRequest",
                type: "POST",
                dataType: "JSON",
                data: { purchase: JSON.stringify(itemMaster), Type: type },
                success: function (response) {
                    if (response.Action == -1) {
                        new ViewMessage({
                            summary: {
                                selector: "#error-summary"
                            }
                        }, response);
                    } else {
                        new ViewMessage({
                            summary: {
                                selector: "#error-summary"
                            }
                        }, response).refresh(1500);
                    }
                    $("#loading").prop("hidden", true);
                }
            });
            this.meta.shutdown();
        });
        dialogSubmit.reject(function () {
            $("#loading").prop("hidden", true);
        });
    });
    // Find Invoice //
    $("#find-invoice").click(function () {
        $("#btn-addnew").prop("hidden", false);
        $("#find-invoice").prop("hidden", true);
        $("#next_number").val("").prop("readonly", false).focus();
    });
    $("#next_number").on("keypress", function (e) {
        if (e.which === 13 && !!$("#txtInvoice").val().trim()) {
            $.ajax({
                url: "/PurchaseRequest/FindPurchaseRequest",
                data: { seriesID: parseInt($("#txtInvoice").val()), number: $("#next_number").val().trim() },
                success: function (res) {
                    $("#txttype").text("Update");
                    getPurchaseItemMaster(res);
                }
            });
        }
    });

    // reload //
    $("#cancel-data").click(function () {
        location.reload();
    });

    $(window).on("keypress", function (e) {
        if (e.which === 13) {
            if (document.activeElement === this.document.getElementById("txtbarcode")) {
                let activeElem = this.document.activeElement;
                $.get("/PurchaseRequest/GetItemDetails", { itemId: 0, curId: itemMaster.PurCurrencyID, barcode: activeElem.value.trim() }, function (res) {
                    if (res.IsError) {
                        new DialogBox({
                            content: res.Error,
                        });
                    } else {
                        if (isValidArray(orderDetail)) {
                            const isExisted = findArray("LineIDUN", res.LineIDUN, "UomID", res.UomID, orderDetail);
                            if (!!isExisted) {
                                const qty = parseFloat(isExisted.Qty) + 1;
                                const openqty = parseFloat(isExisted.Qty) + 1;
                                updateData(orderDetail, "LineIDUN", res.LineIDUN, "UomID", res.UomID, "Qty", qty);
                                updateData(orderDetail, "LineIDUN", res.LineIDUN, "UomID", res.UomID, "OpenQty", openqty);
                                $listItemChoosed.updateColumn(isExisted.LineIDUN, "Qty", qty);
                                const _e = { key: isExisted.LineIDUN, data: isExisted };
                                calDisvalue(_e);
                                totalRow($listItemChoosed, _e, qty);
                            } else {
                                orderDetail.push(res);
                                $listItemChoosed.addRow(res);
                            }
                        } else {
                            orderDetail.push(res);
                            $listItemChoosed.addRow(res);
                        }
                        if (res) {
                            $listItemChoosed.clearHeaderDynamic(res.AddictionProps)
                            $listItemChoosed.createHeaderDynamic(res.AddictionProps)
                        }
                        setSummary(orderDetail);
                        disInputUpdate(itemMaster);
                        activeElem.value = "";
                    }
                });
            }
        }
    });

    const chooseVendor = (table, data) => {
        const vendorDialog = new DialogBox({
            content: {
                selector: ".vendor-container-list"
            },
            type: "yes-no",
            button: {
                yes: {
                    text: "Close"
                },
                no: {
                    text: "Reset"
                }
            },
            caption: "Vendor Lists",
        });
        vendorDialog.invoke(function () {
            const venTable = ViewTable({
                keyField: "ID",
                selector: $("#list-vendor"),
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "Type", "Phone"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down font-size hover"></i>`,
                        on: {
                            "click": function (e) {
                                table.updateColumn(data.key, "VendorCode", e.data.Code)
                                table.updateColumn(data.key, "VendorName", e.data.Name)
                                table.updateColumn(data.key, "VendorID", e.data.ID)
                                vendorDialog.shutdown();
                            }
                        }
                    }
                ]
            });
            $.get("/PurchaseRequest/GetVendors", function (res) {
                venTable.bindRows(res);
                $("#find-vendor").on("keyup", function (e) {
                    let __value = this.value.toLowerCase().replace(/\s+/, "");
                    let rex = new RegExp(__value, "gi");
                    let __vendors = $.grep(res, function (person) {
                        return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s+/, "").match(rex)
                            || person.Phone.toLowerCase().replace(/\s+/, "").match(rex)
                            || person.Type.match(rex);
                    });
                    venTable.bindRows(__vendors);
                });
            })

        });
        vendorDialog.confirm(function () {
            vendorDialog.shutdown();
        })
        vendorDialog.reject(function () {
            table.updateColumn(data.key, "VendorCode", "")
            table.updateColumn(data.key, "VendorName", "")
            table.updateColumn(data.key, "VendorID", 0)
            vendorDialog.shutdown();
        })
    }
    function getPurchaseItemMaster(_master) {
        if (_master.Error) {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: _master.Message,
                close_button: "none"
            });
        }
        else {
            $("#freight-dailog").removeClass("disabled");
            $.get("/PurchaseRequest/Getbp", { id: _master.PurchaseOrder.VendorID }, function (cus) {
                $("#vendor-id").val(cus.Name);
            });
            itemMaster = _master.PurchaseOrder;
            freights = _master.PurchaseOrder.FreightPurchaseView.FreightPurchaseDetailViewModels;
            freightMaster = _master.PurchaseOrder.FreightPurchaseView;
            freightMaster.IsEditabled = true;
            freights.forEach(i => {
                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
                i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
                i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
            });

            masterid = _master.PurchaseOrder.PurchaseOrderID;
            orderDetail = _master.PurchaseOrderDetials;
            itemMaster.ExchangeRate = _master.PurchaseOrder.PurRate;
            $("#txtwarehouse").val(_master.PurchaseOrder.WarehouseID);
            $("#txtreff_no").val(_master.PurchaseOrder.ReffNo);

            $("#requester-id").val(_master.PurchaseOrder.RequesterUsername);
            $("#requester-name").val(_master.PurchaseOrder.RequesterName);
            $("#requester-code").val(_master.PurchaseOrder.RequesterCode);
            $("#txtcurrency").val(_master.PurchaseOrder.PurCurrencyID);
            $("#branch").val(_master.PurchaseOrder.BranchID);
            $("#sta-id").val(_master.PurchaseOrder.Status);
            $("#sub-id").val(num.formatSpecial(_master.PurchaseOrder.SubTotal, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(_master.PurchaseOrder.SubTotalAfterDis, disSetting.Prices));
            $("#freight-value").val(num.formatSpecial(_master.PurchaseOrder.FrieghtAmount, disSetting.Prices));
            //$("#sub-dis-id").val(_master.SaleQuote.TypeDis);
            $("#dis-rate-id").val(num.formatSpecial(_master.PurchaseOrder.DiscountRate, disSetting.Rates));
            $("#dis-value-id").val(num.formatSpecial(_master.PurchaseOrder.DiscountValue, disSetting.Prices));
            setDate("#txtPostingdate", _master.PurchaseOrder.PostingDate.toString().split("T")[0]);
            setDate("#txtDelivery", _master.PurchaseOrder.DeliveryDate.toString().split("T")[0]);
            setDate("#txtDocumentDate", _master.PurchaseOrder.DocumentDate.toString().split("T")[0]);
            $("#remark-id").val(_master.PurchaseOrder.Remark);
            $("#txtRemark").val(_master.PurchaseOrder.Remark);
            $("#total-id").val(num.formatSpecial(_master.PurchaseOrder.BalanceDue, disSetting.Prices));
            $("#vat-value").val(num.formatSpecial(_master.PurchaseOrder.TaxValue, disSetting.Prices));
            $("#item-id").prop("disabled", false);
            $.ajax({
                url: "/PurchaseRequest/GetFilterLocaCurrency",
                type: "Get",
                dataType: "Json",
                data: { CurrencyID: _master.PurchaseOrder.PurCurrencyID },
                success: function (res) {
                    $("#txtExchange").val(1 + " " + _currency + " = " + res[0].SetRate + " " + res[0].Currency.Description);
                    $(".cur-class").text(res[0].Currency.Description);
                    // change currency name in the rows of items after choosed ///
                    $(".cur").text(res[0].Currency.Description);
                }
            });
            if (_master.PurchaseOrderDetials.length > 0) {
                $listItemChoosed.clearHeaderDynamic(_master.PurchaseOrderDetials[0].AddictionProps)
                $listItemChoosed.createHeaderDynamic(_master.PurchaseOrderDetials[0].AddictionProps)
            }
            $listItemChoosed.clearRows();
            $listItemChoosed.bindRows(_master.PurchaseOrderDetials);
            $.ajax({
                url: "/PurchaseRequest/GetItemMasterData",
                type: "GET",
                dataType: "JSON",
                data: { ID: _master.PurchaseOrder.WarehouseID },
                success: function (res) {
                    itemMasters = res;
                }
            });
            if (_master.PurchaseOrder.Status == "close") {
                $("#submit-data").prop("disabled", true);
            }
        }
    }
    function getFreight(success) {
        $.get("/PurchaseRequest/GetFreights", success);
    }

    function freightsEditable(freightDialog) {
        const freightView = ViewTable({
            keyField: "FreightID",
            selector: freightDialog.content.find(".freight-lists"),
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["Amount", "Name", "TaxGroupSelect", "TaxRate", "TotalTaxAmount", "AmountWithTax"],
            columns: [
                {
                    name: "Amount",
                    template: `<input>`,
                    on: {
                        "keyup": function (e) {
                            $(this).asNumber();
                            if (this.value === "") this.value = 0;
                            updatedetailFreight(freights, e.key, "Amount", this.value);
                            totalRowFreight(freightView, e);
                            sumSummaryFreight(freights);
                        }
                    }
                },
                {
                    name: "TaxGroupSelect",
                    template: `<select></select>`,
                    on: {
                        "change": function (e) {
                            const taxg = find("ID", this.value, e.data.TaxGroups);
                            //const taxgselect = findArray("Value", this.value, e.data.TaxGroupSelect);
                            //if (!!taxgselect) {
                            //    updateSelect(e.data.TaxGroupSelect, ta)
                            //}
                            updateSelect(e.data.TaxGroupSelect, this.value, "Selected");
                            if (!!taxg) {
                                updatedetailFreight(freights, e.key, "TaxRate", taxg.Rate);
                            } else {
                                updatedetailFreight(freights, e.key, "TaxRate", 0);
                            }
                            updatedetailFreight(freights, e.key, "TaxGroupID", parseInt(this.value));
                            totalRowFreight(freightView, e);
                            sumSummaryFreight(freights);
                        }
                    }
                }
            ],
        });
        freightDialog.content.find(".freightSumAmount").val(num.formatSpecial(freightMaster.ExpenceAmount, disSetting.Prices));
        freightView.bindRows(freights);
    }
    function freightsNoneEditable(freightDialog) {
        const freightView = ViewTable({
            keyField: "FreightID",
            selector: freightDialog.content.find(".freight-lists"),
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["Amount", "Name", "TaxGroupSelect", "TaxRate", "TotalTaxAmount", "AmountWithTax"],
            columns: [
                {
                    name: "TaxGroupSelect",
                    template: "<select disabled></select>"
                }
            ]
        });
        freightDialog.content.find(".freightSumAmount").val(num.formatSpecial(freightMaster.ExpenceAmount, disSetting.Prices));
        freightView.bindRows(freights);
    }

    function setDate(selector, date_value) {
        var _date = $(selector);
        _date[0].valueAsDate = new Date(date_value);
        _date[0].setAttribute(
            "data-date",
            moment(_date[0].value)
                .format(_date[0].getAttribute("data-date-format"))
        );
    }
    function selectSeries(selected) {
        $.each(__PO.seriesPO, function (i, item) {
            if (item.Default == true) {
                $("<option selected value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
                $("#next_number").val(item.NextNo);
            }
            else {
                $("<option value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
                $("#next_number").val(item.NextNo);
            }
        });
        return selected.on('change')
    }
    function itemMasterDataDialog() {
        const itemDialog = new DialogBox({
            content: {
                selector: ".itemMaster-container-list"
            },
            caption: "Item Master Data"
        });
        itemDialog.invoke(function () {
            const itemView = ViewTable({
                keyField: "ID",
                selector: itemDialog.content.find("#item-master"),
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                dynamicCol: {
                    afterAction: true,
                    headerContainer: "#col-to-append-after",
                },
                visibleFields: ["Code", "Barcode", "ItemName1", "ItemName2", "UomName", "Process"],
                columns: [
                    {
                        name: "AddictionProps",
                        valueDynamicProp: "ValueName",
                        dynamicCol: true,
                    }
                ],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down font-size hover"></i>`,
                        on: {
                            "click": function (e) {
                                chooseItemDetail(e);
                            }
                        }
                    }
                ]
            });
            $.get("/PurchaseRequest/GetItemMasterData", function (itemMasters) {
                if (itemMasters.length > 0) {
                    itemView.clearHeaderDynamic(itemMasters[0].AddictionProps)
                    itemView.createHeaderDynamic(itemMasters[0].AddictionProps)
                    itemView.bindRows(itemMasters);
                    $("#txtSearchitemMaster").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let rex = new RegExp(__value, "gi");
                        let items = $.grep(itemMasters, function (item) {
                            const barcode = item.Barcode ?? "";
                            const name2 = item.ItemName2 ?? "";
                            return item.Code === __value || item.ItemName1.toLowerCase().replace(/\s+/, "").match(rex) ||
                                name2.toLowerCase().replace(/\s+/, "").match(rex) ||
                                item.UomName.toLowerCase().replace(/\s+/, "").match(rex) ||
                                barcode.toLowerCase().replace(/\s+/, "").match(rex) ||
                                item.Cost === __value
                        });
                        itemView.bindRows(items);
                    });
                }
            })
        });
        itemDialog.confirm(function () {
            itemDialog.shutdown();
        });
    }

    function chooseItemDetail(e) {
        getItemMasterDetials(e.data.ID, function (res) {


            if (!res.IsError) {

                const ee =
                {
                    key: res.LineIDUN,
                    data: res
                }
                $listItemChoosed.addRow(res);
                orderDetail.push(res);

                totalRow($listItemChoosed, ee, res.Qty);
            }



            if (res) {
                $listItemChoosed.clearHeaderDynamic(res.AddictionProps)
                $listItemChoosed.createHeaderDynamic(res.AddictionProps)
            }
            setSummary(orderDetail);
        });
    }
    function getItemMasterDetials(itemid, success) {
        $.get("/PurchaseRequest/GetItemDetails", { itemid, curId: itemMaster.PurCurrencyID, barcode: "" }, success);
    }
    function totalRow(table, e, _this) {

        if (_this === '' || _this === '-') _this = 0;
        e.data.FinDisRate = num.toNumberSpecial($("#dis-rate-id").val());
        let totalWDis = (num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice)) - num.toNumberSpecial(e.data.DiscountValue);
        let vatValue = num.toNumberSpecial(e.data.TaxRate) * totalWDis === 0 ? 0 : num.toNumberSpecial(e.data.TaxRate) * totalWDis / 100;
        let fidis = num.toNumberSpecial(e.data.FinDisRate) == 0 ? 0 : (num.toNumberSpecial(e.data.FinDisRate) / 100) * totalWDis;
        let fitotal = totalWDis == 0 ? 0 : totalWDis - fidis;
        let taxoffinal = num.toNumberSpecial(e.data.TaxRate) == 0 ? 0 : num.toNumberSpecial(e.data.TaxRate) / 100 * fitotal;
        let totalwtax = totalWDis + taxoffinal;
        // let totalrow = num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice) - num.toNumberSpecial(e.data.DiscountValue) + vatValue;

        // Update Object
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "Total", totalWDis);
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxRate", e.data.TaxRate);
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxValue", vatValue);
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "FinDisRate", e.data.FinDisRate);
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "FinDisValue", fidis);
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "FinTotalValue", fitotal);
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxOfFinDisValue", taxoffinal);
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TotalWTax", totalwtax);

        //Update View
        table.updateColumn(e.key, "Total", num.formatSpecial(isNaN(totalWDis) ? 0 : totalWDis, disSetting.Prices));
        table.updateColumn(e.key, "TaxValue", num.formatSpecial(isNaN(vatValue) ? 0 : vatValue, disSetting.Prices));
        table.updateColumn(e.key, "FinDisRate", num.formatSpecial(isNaN(e.data.FinDisRate) ? 0 : e.data.FinDisRate, disSetting.Prices));
        table.updateColumn(e.key, "FinDisValue", num.formatSpecial(isNaN(fidis) ? 0 : fidis, disSetting.Prices));
        table.updateColumn(e.key, "FinTotalValue", num.formatSpecial(isNaN(fitotal) ? 0 : fitotal, disSetting.Prices));
        table.updateColumn(e.key, "TaxOfFinDisValue", num.formatSpecial(isNaN(taxoffinal) ? 0 : taxoffinal, disSetting.Prices));
        table.updateColumn(e.key, "TotalWTax", num.formatSpecial(totalwtax, disSetting.Prices));
        table.updateColumn(e.key, "TaxRate", num.formatSpecial(e.data.TaxRate, disSetting.Prices));

    }

    function calDisRate(e, _this) {
        if (_this.value > 100) _this.value = 100;
        if (_this.value < 0) _this.value = 0;
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "DiscountRate", _this.value);
        const disvalue = parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.PurchasPrice) === 0 ? 0 :
            parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.PurchasPrice) / 100;

        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "DiscountValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DiscountValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisValue(e, _this) {
        if (_this.value > e.data.Qty * e.data.PurchasPrice) _this.value = e.data.Qty * e.data.PurchasPrice;
        if (_this.value == '' || _this.value < 0) _this.value = 0;
        const value = num.toNumberSpecial(_this.value);
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "DiscountValue", value);
        const ratedis = (value * 100 === 0) || (num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice) === 0) ? 0 :
            (value * 100) / (num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice));
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "DiscountRate", ratedis);
        $listItemChoosed.updateColumn(e.key, "DiscountRate", num.formatSpecial(isNaN(ratedis) ? 0 : ratedis, disSetting.Rates));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisvalue(e) {
        let disvalue = num.toNumberSpecial(e.data.DiscountRate) * num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice) === 0 ? 0 :
            (num.toNumberSpecial(e.data.DiscountRate) * num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice)) / 100;
        if (disvalue > num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice)) disvalue = num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice);
        updateData(orderDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "DiscountValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DiscountValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
    }

    function setSummary(data) {
        let subtotal = 0;
        let vat = 0;
        let disRate = $("#dis-rate-id").val();
        if (isValidArray(data)) {
            data.forEach(i => {
                subtotal += num.toNumberSpecial(i.Total);
                vat += num.toNumberSpecial(i.TaxOfFinDisValue);
            });
        }
        const disValue = (num.toNumberSpecial(disRate) * subtotal) === 0 ? 0 : (num.toNumberSpecial(disRate) * subtotal) / 100;
        itemMaster.SubTotalSys = subtotal * itemMaster.ExchangeRate;
        itemMaster.SubTotal = subtotal;
        itemMaster.TaxRate = (vat * 100) === 0 ? 0 : vat * 100 / subtotal;
        itemMaster.TaxValue = vat + num.toNumberSpecial(freightMaster.TaxSumValue);
        itemMaster.DiscountValue = disValue;
        itemMaster.SubTotalAfterDis = subtotal - disValue;
        itemMaster.SubTotalAfterDisSys = subtotal * itemMaster.ExchangeRate;
        itemMaster.DiscountRate = disRate;
        itemMaster.BalanceDue = num.toNumberSpecial(itemMaster.SubTotalAfterDis) + itemMaster.TaxValue + num.toNumberSpecial(freightMaster.ExpenceAmount);
        itemMaster.BalanceDueSys = itemMaster.BalanceDue * itemMaster.ExchangeRate;

        $("#vat-value").val(num.formatSpecial(itemMaster.TaxValue, disSetting.Prices));
        $("#sub-id").val(num.formatSpecial(subtotal, disSetting.Prices));
        $("#total-id").val(num.formatSpecial(itemMaster.BalanceDue, disSetting.Prices));
        $("#sub-after-dis").val(num.formatSpecial(itemMaster.SubTotalAfterDis, disSetting.Prices));
    }
    function disInputUpdate(_master) {
        $("#dis-rate-id").val(_master.DiscountRate);
        $("#dis-value-id").val(_master.DiscountValue);
    }

    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function find(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
    function findArray(keyName, keyValue, keyName1, keyVaue1, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue && item[keyName1] == keyVaue1;
            })[0];
        }
    }
    function updateData(data, keyField, keyValue, keyField1, keyValue1, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] == keyValue && i[keyField1] == keyValue1) {
                    i[prop] = propValue;
                }
            });
        }
    }
    function updatedetailFreight(data, key, prop, value) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i.FreightID === key) {
                    i[prop] = value;
                }
            });
        }
    }
    function invoiceDisAllRowRate(_this) {
        if (isValidArray(orderDetail)) {
            orderDetail.forEach(i => {
                const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) / 100;
                if (_this.value < 100) {
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisValue", value);
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisRate", _this.value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(value);
                    const taxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;

                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "TaxOfFinDisValue", taxOfFinDisValue);
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineIDUN, "TaxOfFinDisValue", num.formatSpecial(taxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                } else if (_this.value >= 100) {
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisValue", value);
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisRate", _this.value);

                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "TaxOfFinDisValue", 0);
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineIDUN, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(orderDetail);
        }
    }
    function invoiceDisAllRowValue(value) {
        if (isValidArray(orderDetail)) {
            orderDetail.forEach(i => {
                const _value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) / 100;
                if (value < 100) {
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisValue", num.formatSpecial(_value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisValue", _value);
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisRate", value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(_value);
                    const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "TaxOfFinDisValue", TaxOfFinDisValue);
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineIDUN, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                }
                else if (value >= 100) {
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisValue", _value);
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisRate", value);

                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "TaxOfFinDisValue", 0);
                    updateData(orderDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineIDUN, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(orderDetail);
        }
    }
    function totalRowFreight(table, e,) {
        table.updateColumn(e.key, "TaxRate", num.formatSpecial(e.data.TaxRate, disSetting.Rates));
        const taxValue = num.toNumberSpecial(e.data.TaxRate) * num.toNumberSpecial(e.data.Amount) === 0 ? 0 :
            num.toNumberSpecial(e.data.TaxRate) * num.toNumberSpecial(e.data.Amount) / 100;
        const amountWTax = num.toNumberSpecial(e.data.Amount) + taxValue;
        updatedetailFreight(freights, e.key, "TotalTaxAmount", taxValue);
        updatedetailFreight(freights, e.key, "AmountWithTax", amountWTax);

        table.updateColumn(e.key, "TotalTaxAmount", num.formatSpecial(e.data.TotalTaxAmount, disSetting.Prices));
        table.updateColumn(e.key, "AmountWithTax", num.formatSpecial(e.data.AmountWithTax, disSetting.Prices));
    }
    function sumSummaryFreight(data) {
        if (isValidArray(data)) {
            let sumFreight = 0;
            let sumTax = 0;
            data.forEach(i => {
                sumFreight += num.toNumberSpecial(i.Amount);
                sumTax += num.toNumberSpecial(i.TotalTaxAmount);
            });
            freightMaster.ExpenceAmount = sumFreight;
            freightMaster.TaxSumValue = sumTax;
            $(".freightSumAmount").val(num.formatSpecial(sumFreight, disSetting.Prices));
        }
    }
    function updateSelect(data, key, prop) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i.Value == key) {
                    i[prop] = true;
                } else {
                    i[prop] = false;
                }
            });
        }
    }
});