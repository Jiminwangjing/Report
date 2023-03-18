"use strict"
let quote_details = [];
let ExChange = [];
let master = [];
let _currency = "";
let _curencyID = 0;
let _priceList = 0;
let freights = [];
let freightMaster = {};
const ___PA = JSON.parse($("#data-invoice").text());
const disSetting = ___PA.genSetting.Display;
let storeBaseonID = 0;
let storeKey = "";
let storeCopy = 0;
$(document).ready(function () {
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    })
    $.each($("[data-date]"), function (i, t) {
        setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
    });
    // tb master
    var itemmaster = {
        ID: 0,
        Name:"",
        CusID: 0,
        ConTactID:0,
        BranchID: 0,
        WarehouseID: 0,
        UserID: 0,
        SaleCurrencyID: 0,
        RefNo: "",
        InvoiceNo: "",
        ExchangeRate: 0,
        PostingDate: "",
        ValidUntilDate: "",
        DocumentDate: "",
        IncludeVat: false,
        Status: "open",
        Remarks: "",
        SaleEMID: 0,
        OwnerID:0,
        SubTotal: 0,
        SubTotalSys: 0,
        DisRate: 0,
        DisValue: 0,
        TypeDis: "Prices",
        VatRate: 0,
        VatValue: 0,
        TotalAmount: 0,
        TotalAmountSys: 0,
        PriceListID: 0,
        LocalSetRate: 0,
        TotalMargin: 0,
        TotalCommission: 0,
        OtherCost: 0,
        ExpectedTotalProfit: 0,
        BaseOnID: 0,
        CopyType: "0",
        KeyCopy:"",
        ProjCostAnalysisDetails: new Array()
    }
    master.push(itemmaster);
    // select inoice with defualt == true
    ___PA.seriesPA.forEach(i => {
        if (i.Default == true) {
            $("#DocumentTypeID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    let selected = $("#invoice-no");
    selectSeries(selected);
    $('#invoice-no').change(function () {
        var id = ($(this).val());
        var seriesPA = findArray("ID", id, ___PA.seriesPA);
        ___PA.seriesPA.Number = seriesPA.NextNo;
        ___PA.seriesPA.ID = id;
        $("#DocumentID").val(seriesPA.DocumentTypeID);
        $("#number").val(seriesPA.NextNo);
        $("#next_number").val(seriesPA.NextNo);
    });
    if (___PA.seriesPA.length == 0) {
        $('#invoice-no').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
        $("#submit-item").prop("disabled", true);
    }
    // get warehouse
    //setWarehouse(0);
    // get Default currency
    $.ajax({
        url: "/ProjectCostAnalysis/GetDefaultCurrency",
        type: "Get",
        dataType: "Json",
        async: false,
        success: function (response) {
            _currency = response.Description;
            _curencyID = response.ID;
            $(".cur-class").text(_currency);
        }
    });
    // date format
    $("input[type='date']").on("change", function () {
        this.setAttribute(
            "data-date",
            moment(this.value, "YYYY-MM-DD")
                .format(this.getAttribute("data-date-format"))
        )
    });
    // get  currency option
    $.ajax({
        url: "/ProjectCostAnalysis/GetPriceList",
        type: "Get",
        dataType: "Json",
        success: function (response) {
            $("#cur-id option:not(:first-child)").remove();
            $.each(response, function (i, item) {
                $("#cur-id").append("<option  value=" + item.CurrencyID + ">" + item.Name + "</option>");
            });
        }
    });
    //$(".modal-header").on("mousedown", function (mousedownEvt) {
    //    var $draggable = $(this);
    //    var x = mousedownEvt.pageX - $draggable.offset().left,
    //        y = mousedownEvt.pageY - $draggable.offset().top;
    //    $("body").on("mousemove.draggable", function (mousemoveEvt) {
    //        $draggable.closest(".modal-dialog").offset({
    //            "left": mousemoveEvt.pageX - x,
    //            "top": mousemoveEvt.pageY - y
    //        });
    //    });
    //    $("body").one("mouseup", function () {
    //        $("body").off("mousemove.draggable");
    //    });
    //    $draggable.closest(".modal").one("bs.modal.hide", function () {
    //        $("body").off("mousemove.draggable");
    //    });
    //});
    //// get customer
    //$("#show-list-cus").click(function () {
    //    $("#show-list-cus").prop("disabled", false);
    //    $("#ModalCus").modal("show");
    //    $.ajax({
    //        url: "/ProjectCostAnalysis/GetCustomer",
    //        type: "Get",
    //        dataType: "Json",
    //        success: function (response) {
    //            bindCustomer(response);
    //            $(".modal-dialog #find-cus").on("keyup", function (e) {
    //                let __value = this.value.toLowerCase().replace(/\s+/, "");
    //                let __customers = $.grep(response, function (person) {
    //                    return person.Code.toLowerCase().replace(/\s+/, "").includes(__value) || person.Name.toLowerCase().replace(/\s+/, "").includes(__value)
    //                        || person.Phone.toLowerCase().replace(/\s+/, "").includes(__value) || person.Type.toLowerCase().replace(/\s+/, "").includes(__value);
    //                });
    //                bindCustomer(__customers);
    //            });
    //        }
    //    });
    //});
    $("#cus-choosed").click(function () {
        $(".cus-id").val(_nameCus);
        master.forEach(i => {
            i.CusID = _idCus;
        })
        $("#barcode-reading").prop("disabled", false).focus();
        $("#item-id").prop("disabled", false);
        
    });
    $.ajax({
        url: "/ProjectCostAnalysis/GetExchange",
        type: "Get",
        dataType: "Json",
        success: function (res) {
            $.each(res, function (i, item) {
                if (item.CurID == _curencyID) {
                    $("#ex-id").val(1 + " " + _currency + " = " + item.SetRate + " " + item.CurName);
                    master.forEach(j => {
                        j.ExchangeRate = item.Rate;
                    })
                }
            });
            ExChange.push(res);
        }
    });
    //// Bind Item Choosed ////
    const $listItemChoosed = ViewTable({
        keyField: "LineID",
        selector: "#list-detail",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: true
        },
       
        visibleFields: [
            "ItemCode", "BarCode", "Description", "Qty", "UoMs", "Currency", "Cost", "UnitPrice", "DisRate", "DisValue", "LineTotalBeforeDis", "LineTotalCost", "TaxGroupList", "TaxRate", "TaxValue",
            "TaxOfFinDisValue", "FinDisRate", "FinDisValue", "Total", "UnitMargin", "TotalWTax", "LineTotalMargin", "InStock", "FinTotalValue", "Remarks",
        ],
        //"ItemCode", "BarCode", "Description", "Qty", "UnitPrice", "TaxGroupList", "TaxRate", "TaxValue", "TotalWTax", "Total", "Currency",
        //"UoMs", "ItemNameKH", "DisValue", "DisRate", "Remarks", "FinDisRate", "FinDisValue", "TaxOfFinDisValue", "FinTotalValue"
        columns: [
            {
                name: "UnitPrice",
                template: `<input class='font-size' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updatedetail(quote_details, e.data.UomID, e.key, "UnitPrice", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this.value);
                        setSummary(quote_details);
                       // disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "Currency",
                template: `<span class='font-size cur'></span`,
            },
            {
                name: "DisValue",
                template: `<input class='font-size' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        if(this.value > num.toNumberSpecial(e.data.LineTotalBeforeDis))
                            this.value = num.toNumberSpecial(e.data.LineTotalBeforeDis);
                        calDisValue(e, this.value);
                        setSummary(quote_details);
                       // disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "DisRate",
                template: `<input class='font-size' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        if(this.value > 100) {
                            this.value = 100;
                        }
                        calDisRate(e, this.value);
                        setSummary(quote_details);
                       // disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "Qty",
                template: `<input class='font-size' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                       
                        updatedetail(quote_details, e.data.UomID, e.key, "Qty", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this.value);
                        setSummary(quote_details);
                       // disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "Cost",
                template: `<input class='font-size' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updatedetail(quote_details, e.data.UomID, e.key, "Cost", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this.value);
                        setSummary(quote_details);
                        //disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "TaxGroupList", 
                template: `<select class='font-size'></select>`,
                on: {
                    "change": function (e) {
                        const taxg = findArray("ID", this.value, e.data.TaxGroups);
                        if (!!taxg) {
                            updatedetail(quote_details, e.data.UomID, e.key, "TaxRate", taxg.Rate);
                        } else {
                            updatedetail(quote_details, e.data.UomID, e.key, "TaxRate", 0);
                        }
                        updatedetail(quote_details, e.data.UomID, e.key, "TaxGroupID", parseInt(this.value))
                        totalRow($listItemChoosed, e, this.value);
                        setSummary(quote_details);
                       // disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "UoMs",
                template: `<select class='font-size'></select>`,
                on: {
                    "change": function (e) {
                        updatedetail(quote_details, e.data.UomID, e.key, "UomID", parseInt(this.value));
                        const uomList = findArray("UoMID", this.value, e.data.UomPriceLists);
                        const uom = findArray("ID", this.value, e.data.UoMsList);
                        if (!!uom && !!uomList) {
                            updatedetail(quote_details, e.data.UomID, e.key, "UnitPrice", uomList.UnitPrice);
                            updatedetail(quote_details, e.data.UomID, e.key, "UomName", uom.Name);
                            updatedetail(quote_details, e.data.UomID, e.key, "Factor", uom.Factor);
                            $listItemChoosed.updateColumn(e.key, "UnitPrice", num.formatSpecial(uomList.UnitPrice, disSetting.Prices));
                            calDisvalue(e);
                            totalRow($listItemChoosed, e, this.value);
                            setSummary(quote_details);
                           // disInputUpdate(master[0]);
                        }
                    }
                }
            },
            {
                name: "InStock",
                template: `<input class='font-size'>`,
                on: {
                    "keyup": function (e) {
                        updatedetail(quote_details, e.data.UomID, e.key, "InStock", this.value);
                    }
                }
            },
            {
                name: "Remarks",
                template: `<input>`,
                on: {
                    "keyup": function (e) {
                        updatedetail(quote_details, e.data.UomID, e.key, "Remarks", this.value);
                    }
                }
            }
        ],
        actions: [
            {
                template: `<i class="fas fa-trash"></i>`,
                on: {
                    "click": function (e) {
                        if (e.data.ID == 0) {
                            $listItemChoosed.removeRow(e.key);
                            quote_details = quote_details.filter(i => i.LineID !== e.key);
                        }
                        setSummary(quote_details);
                       // disInputUpdate(master[0]);
                    }
                }
            }
        ]
    });
    //// change currency 
    $("#cur-id").change(function () {
        var $this = this;
        _priceList = $this.value;
        $.ajax({
            url: "/ProjectCostAnalysis/GetSaleCur",
            type: "Get",
            dataType: "Json",
            data: { PriLi: _priceList },
            success: function (res) {
          
                const ex = ExChange[0].filter(i => {
                    return res.CurrencyID == i.CurID
                })
                if (master.length > 0) {
                    master.forEach(i => {
                        i.ExchangeRate = ex[0].Rate;
                        i.SaleCurrencyID = res.CurrencyID;
                        i.PriceListID = _priceList;
                    });
                   
                }
                $("#ex-id").val(1 + " " + _currency + " = " + ex[0].SetRate + " " + ex[0].CurName);
                $(".cur-class").text(ex[0].CurName);
                // change currency name in the rows of items after choosed ///
                $(".cur").text(ex[0].CurName);
                quote_details = [];
                $listItemChoosed.clearRows();
                $listItemChoosed.bindRows(quote_details);
                setSummary(quote_details);
                disInputUpdate(master[0]);
            }
        });
    })

    /// dis rate on invoice //
    $("#dis-rate-id").keyup(function () {
        $(this).asNumber();
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > 100) this.value = 100;
        const subtotal = num.toNumberSpecial($("#sub-id").val());
        let value = num.toNumberSpecial(this.value) / 100;
       // value = num.toNumberSpecial(value, disSetting.Prices);
        const disvalue = num.toNumberSpecial(value) * subtotal;
       
       $("#dis-value-id").val(num.formatSpecial(disvalue, disSetting.Prices));
        invoiceDisAllRowRate(this);
        if (parseFloat(disvalue) > 0) {
            const totalAfDis = subtotal - parseFloat(disvalue) + master[0].VatValue;
            $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
            master[0].TotalAmount = totalAfDis;
            master[0].DisValue = disvalue;
            master[0].DisRate = this.value;
            master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
        } else {
            setSummary(quote_details);
        }
    });
    /// dis value on invoice //
    $("#dis-value-id").keyup(function ()
    {
        $(this).asNumber();
        const subtotal = num.toNumberSpecial($("#sub-id").val());
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > subtotal) {
            this.value = subtotal;
        }
        let rate = num.toNumberSpecial(this.value) / subtotal;
        rate = num.toNumberSpecial(rate, disSetting.Prices);

        let disrate = (num.toNumberSpecial(this.value) * 100 === 0) || (subtotal === 0) ? 0 : ((num.toNumberSpecial(rate) * 100));
     
        $("#dis-rate-id").val(num.formatSpecial(disrate, disSetting.Prices));
        invoiceDisAllRowValue(disrate);
        if (parseFloat(disrate) > 0)
        {
            const totalAfDis = subtotal - num.toNumberSpecial(this.value) + master[0].VatValue;
            $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
            master[0].TotalAmount = totalAfDis;
            master[0].DisValue = this.value;
            master[0].DisRate = disrate;
            master[0].SubTotalAfterDis = totalAfDis - master[0].VatValue;
            master[0].SubTotalBefDis = subtotal;
            master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
        } else {
            setSummary2(quote_details);
        }
    }); 
    $("#totalcommission").keyup(function () {
        $(this).asNumber();
        const margin = num.toNumberSpecial($("#totalmargin").val());
        const othercost = num.toNumberSpecial($("#txt_othercost").val());
        const subafterdis = num.toNumberSpecial($("#sub-after-dis").val());
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > margin) {
            this.value = margin;

        }
        master[0].TotalCommission = this.value;
        master[0].ExpectedTotalProfit = margin - num.toNumberSpecial(this.value) - num.toNumberSpecial(othercost);
        let percenttage = (num.toNumberSpecial(this.value) / subafterdis) * 100;
        let profpercent = (num.toNumberSpecial(master[0].ExpectedTotalProfit) / subafterdis) * 100;
        $("#txt_totalcommitpersentage").val(num.formatSpecial(percenttage, disSetting.Prices));
        $("#expectedtotalprofit").val(num.formatSpecial(master[0].ExpectedTotalProfit, disSetting.Prices));
        $("#txt_totalprofit").val(num.formatSpecial(profpercent, disSetting.Prices));

    });
    $("#txt_othercost").keyup(function () {
        $(this).asNumber();
        const margin = num.toNumberSpecial($("#totalmargin").val());
        const permission = num.toNumberSpecial($("#totalcommission").val());
        const subafterdis = num.toNumberSpecial($("#sub-after-dis").val());
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > margin) {
            this.value = margin;
        }
        master[0].OtherCost = this.value;
        master[0].ExpectedTotalProfit = margin - num.toNumberSpecial(this.value) - num.toNumberSpecial(permission);
        let percenttage = (num.toNumberSpecial(this.value) / subafterdis) * 100;
        let profpercent = (num.toNumberSpecial(master[0].ExpectedTotalProfit) / subafterdis) * 100;
        $("#txt_otherpersentage").val(num.formatSpecial(percenttage, disSetting.Prices));
        $("#expectedtotalprofit").val(num.formatSpecial(master[0].ExpectedTotalProfit, disSetting.Prices));
        $("#txt_totalprofit").val(num.formatSpecial(profpercent, disSetting.Prices));
    })
    // block Prices Tage
    $("#txt_totalcommitpersentage").keyup(function () {
        $(this).asNumber();
        const margin = num.toNumberSpecial($("#totalmargin").val());
        const othercost = num.toNumberSpecial($("#txt_othercost").val());
        const subafterdis = num.toNumberSpecial($("#sub-after-dis").val());
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > 100) {
            this.value = 100;
        }
        let permission = (num.toNumberSpecial(this.value) / 100) * subafterdis;
        master[0].TotalCommission = permission;
        master[0].ExpectedTotalProfit = margin - num.toNumberSpecial(permission) - num.toNumberSpecial(othercost);
        let profpercent = (num.toNumberSpecial(master[0].ExpectedTotalProfit) / subafterdis) * 100;
        $("#totalcommission").val(num.formatSpecial(permission, disSetting.Prices));
        $("#expectedtotalprofit").val(num.formatSpecial(master[0].ExpectedTotalProfit, disSetting.Prices));
        $("#txt_totalprofit").val(num.formatSpecial(profpercent, disSetting.Prices));
    })
    $("#txt_otherpersentage").keyup(function () {
        $(this).asNumber();
        const margin = num.toNumberSpecial($("#totalmargin").val());
        const permission = num.toNumberSpecial($("#totalcommission").val());
        const subafterdis = num.toNumberSpecial($("#sub-after-dis").val());
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > 100) {
            this.value = 100;
        }
        let othercost = (num.toNumberSpecial(this.value) / 100) * subafterdis;
        master[0].TotalCommission = permission;
        master[0].ExpectedTotalProfit = margin - num.toNumberSpecial(permission) - num.toNumberSpecial(othercost);
        let profpercent = (num.toNumberSpecial(master[0].ExpectedTotalProfit) / subafterdis) * 100;
        $("#txt_othercost").val(num.formatSpecial(othercost, disSetting.Prices));
        $("#expectedtotalprofit").val(num.formatSpecial(master[0].ExpectedTotalProfit, disSetting.Prices));
        $("#txt_totalprofit").val(num.formatSpecial(profpercent, disSetting.Prices));
    })
    $("#txt_copyprojcostans").click(() => {
        let status = num.toNumberSpecial($("#sta-id").val());
        if (status == 2) {

        }
    })

    //// Choose Item by BarCode ////
    $(window).on("keypress", function (e) {
        if (e.which === 13) {
            if (document.activeElement === this.document.getElementById("barcode-reading")) {
                let activeElem = this.document.activeElement;
                if (master[0].CurID != 0) {
                    $.get("/ProjectCostAnalysis/GetItemDetails", { PriLi: _priceList, itemId: 0, barCode: activeElem.value.trim(), uomId: 0 }, function (res) {
                        if (res.IsError) {
                            new DialogBox({
                                content: res.Error,
                            });
                        } else {
                            if (isValidArray(quote_details)) {
                                res.forEach(i => {
                                    const isExisted = quote_details.some(qd => qd.ItemID === i.ItemID);
                                    if (isExisted) {
                                        const item = quote_details.filter(_i => _i.BarCode === i.BarCode)[0];
                                        if (!!item) {
                                            const qty = parseFloat(item.Qty) + 1;
                                            const openqty = parseFloat(item.Qty) + 1;
                                            updatedetail(quote_details, i.UomID, item.LineID, "Qty", qty);
                                            updatedetail(quote_details, i.UomID, item.LineID, "OpenQty", openqty);
                                            $listItemChoosed.updateColumn(item.LineID, "Qty", qty);
                                            const _e = { key: item.LineID, data: item };
                                            calDisvalue(_e);
                                            totalRow($listItemChoosed, _e, qty);
                                        }
                                    } else {
                                        quote_details.push(i);
                                        $listItemChoosed.addRow(i);
                                    }
                                })
                            } else {
                                res.forEach(i => {
                                    quote_details.push(i);
                                    $listItemChoosed.addRow(i);
                                })

                            }
                            setSummary(quote_details);
                            disInputUpdate(master[0]);
                            activeElem.value = "";
                        }
                    });
                }

            }
        }
    });
    //// Find Sale Qoute ////
    $("#btn-find").on("click", function () {
        $("#btn-addnew").prop("hidden", false);
        $("#btn-find").prop("hidden", true);
        $("#next_number").val("").prop("readonly", false).focus();
    });
    $("#next_number").on("keypress", function (e) {
        if (e.which === 13 && !!$("#invoice-no").val().trim()) {
            $.ajax({
                url: "/ProjectCostAnalysis/FindProjectCost",
                data: { number: $("#next_number").val(), seriesID: parseInt($("#invoice-no").val()) },
                success: function (result) {
                    
                    getSaleItemMasters(result);
                }
            });
        }
    });
    //// Bind Item Master ////
    function BindItemMastor(result) {
        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: "List Item Data",
            content: {
                selector: "#ModalItem"
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown();
        })
        dialogprojmana.invoke(function () {
            const itemMaster = ViewTable({
                keyField: "ID",
                selector: $("#list-item"),
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Code", "KhmerName", "Currency", "UnitPrice", "BarCode", "UoM", "InStock"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                $.get("/ProjectCostAnalysis/GetItemDetails", { PriLi: e.data.PricListID, itemId: e.data.ItemID, barCode: "", uomId: e.data.UomID }, function (res) {
                                   
                                    res[0].TaxRate = num.formatSpecial(res[0].TaxRate, disSetting.Prices);
                                    res[0].TaxValue = num.formatSpecial(res[0].TaxValue, disSetting.Prices);
                                    res[0].TaxOfFinDisValue = num.formatSpecial(res[0].TaxOfFinDisValue, disSetting.Prices);
                                    res[0].FinDisRate = num.formatSpecial(res[0].FinDisRate, disSetting.Prices);
                                    res[0].FinDisValue = num.formatSpecial(res[0].FinDisValue, disSetting.Prices);
                                    res[0].Total = num.formatSpecial(res[0].Total, disSetting.Prices);
                                    res[0].Cost = num.formatSpecial(res[0].Cost, disSetting.Prices);
                                    res[0].LineTotalCost = num.formatSpecial(res[0].LineTotalCost, disSetting.Prices);
                                    res[0].UnitMargin = num.formatSpecial(res[0].UnitMargin, disSetting.Prices);
                                    res[0].TotalWTax = num.formatSpecial(res[0].TotalWTax, disSetting.Prices);
                                    res[0].LineTotalMargin = num.formatSpecial(res[0].LineTotalMargin, disSetting.Prices);
                                    res[0].FinTotalValue = num.formatSpecial(res[0].FinTotalValue, disSetting.Prices);
                                    res[0].UnitPriceAfterDis = num.formatSpecial(res[0].UnitPriceAfterDis, disSetting.Prices);
                                    res[0].LineTotalBeforeDis = num.formatSpecial(res[0].LineTotalBeforeDis, disSetting.Prices);
                                    quote_details.push(res[0]);
                                    // $listItemChoosed.addRow(quote_details);
                                    $listItemChoosed.bindRows(quote_details);
                                    setSummary(quote_details);
                                    disInputUpdate(master[0]);
                                   
                                });
                            }
                        }
                    }
                ]
            });
            itemMaster.bindRows(result);
        });

    }
   
    //// choose item
    $("#item-id").click(function () {
        //$("#ModalItem").modal("show");
        $("#loadingitem").prop("hidden", false);
        if (master.CurID != 0) {
            $.ajax({
                url: "/ProjectCostAnalysis/GetItem",
                type: "Get",
                dataType: "Json",
                data: { PriLi: _priceList },
                success: function (res) {
 
                    if (res.status == "T") {
                        if (res.ls_item.length > 0) {
                            BindItemMastor(res.ls_item)
                          //  itemMaster.bindRows(res.ls_item);
                            $("#loadingitem").prop("hidden", true);
                            $(".modal-dialog #find-item").on("keyup", function (e) {
                                let __value = this.value.toLowerCase().replace(/\s+/, "");
                                let items = $.grep(res.ls_item, function (item) {
                                    return item.Barcode === __value || item.KhmerName.toLowerCase().replace(/\s+/, "").includes(__value)
                                        || item.UnitPrice == __value || item.Code.toLowerCase().replace(/\s+/, "").includes(__value)
                                        || item.UoM.toLowerCase().replace(/\s+/, "").includes(__value);
                                });
                                BindItemMastor(items)
                                //itemMaster.bindRows(items);
                            });
                        }
                        else {
                            $("#loadingitem").prop("hidden", true);
                        }
                        setTimeout(() => {
                            $(".modal-dialog #find-item").focus();
                        }, 300);
                    }
                }
            });
        }
    });

    // Freight Dialog //
    $("#freight-dailog").click(function () {
        const freightDialog = new DialogBox({
            content: {
                selector: "#container-list-freight"
            },
            caption: "Freight",
        });
        freightDialog.invoke(function () {
            const freightView = ViewTable({
                keyField: "LineID",
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
                        template: `<input class="disabled" >`,
                        on: {
                            "keyup": function (e) {
                                $(this).asNumber();
                                if (this.value === "") this.value = 0;
                                e.data.Amount = this.value;
                                updatedetailFreight(freights, e.key, "Amount", this.value);
                                totalRowFreight(freightView, e);
                                sumSummaryFreight(freights);
                               
                            }
                        }
                    },
                    {
                        name: "TaxGroupSelect",
                        template: `<select class="disabled"></select>`,
                        on: {
                            "change": function (e) {
                                const taxg = findArray("ID", this.value, e.data.TaxGroups);
                               
                                updateSelect(e.data.TaxGroupSelect, this.value, "Selected");
                                if (taxg) {
                                    updatedetailFreight(freights, e.key, "TaxRate", taxg.Rate);
                                    e.data.TaxRate = taxg.Rate;
                                } else {
                                    updatedetailFreight(freights, e.key, "TaxRate", 0);
                                    e.data.TaxRate = 0;
                                }
                                updatedetailFreight(freights, e.key, "TaxGroupID", parseInt(this.value));
                                totalRowFreight(freightView, e);
                                sumSummaryFreight(freights);
                               
                            }
                        }
                    }
                ],
            });
            freightDialog.content.find(".freightSumAmount").val(num.formatSpecial(freightMaster.AmountReven, disSetting.Prices));
            freightView.bindRows(freights);
         

        });
        freightDialog.confirm(function () {
            $("#freight-value").val(num.formatSpecial(freightMaster.AmountReven, disSetting.Prices));
            setSummary(quote_details);
            freightDialog.shutdown();
        });
    });
    getFreight(function (res) {
        freightMaster = res;
        if (isValidArray(res.FreightSaleDetailViewModels)) {
            if (isValidArray(freights)) {
                freights = [];
                res.FreightSaleDetailViewModels.forEach(i => {
                    freights.push(i);
                });
            }
            else {
                res.FreightSaleDetailViewModels.forEach(i => {
                    freights.push(i);
                });
            }
        }
    });
    function getFreight(success) {
        $.get("/ProjectCostAnalysis/GetFreights", success);
    }
    function getSaleItemMasters(_master) {
       
        if (!!_master)
        {
            
           
            freights = [];
            quote_details = [];
            master[0] = _master.ProjectCostAnalysis;
            freights = _master.ProjectCostAnalysis.FreightProjectCost.FreightProjCostDetails;
            freightMaster = _master.ProjectCostAnalysis.FreightProjectCost;
            freights.forEach(i => {
                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
                i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
                i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
            });
            _master.DetailItemMasterDatas.forEach(i => {
               i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Prices);
                i.TaxValue = num.formatSpecial(i.TaxValue, disSetting.Prices);
                i.DisRate = num.formatSpecial(i.DisRate, disSetting.Prices);
                i.TaxOfFinDisValue = num.formatSpecial(i.TaxOfFinDisValue, disSetting.Prices);
                i.FinDisRate = num.formatSpecial(i.FinDisRate, disSetting.Prices);
                i.FinDisValue = num.formatSpecial(i.FinDisValue, disSetting.Prices);
                i.Total = num.formatSpecial(i.Total, disSetting.Prices);
                i.Cost = num.formatSpecial(i.Cost, disSetting.Prices);
                i.LineTotalCost = num.formatSpecial(i.LineTotalCost, disSetting.Prices);
                i.UnitMargin = num.formatSpecial(i.UnitMargin, disSetting.Prices);
                i.TotalWTax = num.formatSpecial(i.TotalWTax, disSetting.Prices);
               i.LineTotalMargin = num.formatSpecial(i.LineTotalMargin, disSetting.Prices);
                i.FinTotalValue = num.formatSpecial(i.FinTotalValue, disSetting.Prices);
                i.UnitPriceAfterDis = num.formatSpecial(i.UnitPriceAfterDis, disSetting.Prices);
                i.LineTotalBeforeDis = num.formatSpecial(i.LineTotalBeforeDis, disSetting.Prices);
                quote_details.push(i);
            });
            
            
            _priceList = _master.Currency.ID;
            $("#txt_proid").val(_master.ProjectCostAnalysis.ID);
            $("#txt_proname").val(_master.ProjectCostAnalysis.Name);
            $("#txt_cusname").val(_master.ProjectCostAnalysis.CusName);
            $("#txt_idcus").val(_master.ProjectCostAnalysis.CusID);
            $("#txt_cuscode").val(_master.ProjectCostAnalysis.CusCode);
            $("#txt_idcontect").val(_master.ProjectCostAnalysis.ConTactID);
            $("#txt_contactperson").val(_master.ProjectCostAnalysis.ContName);
            $("#txt_phone").val(_master.ProjectCostAnalysis.Phone);
            $("#invoice-no").val(_master.ProjectCostAnalysis.SeriesID);
            $("#invoice-no").val(_master.ProjectCostAnalysis.SeriesID);
            $("#next_number").val(_master.ProjectCostAnalysis.InvoiceNumber);

            $("#ware-id").val(_master.ProjectCostAnalysis.WarehouseID);
            $("#ref-id").val(_master.ProjectCostAnalysis.RefNo);
            $("#cur-id").val(_master.ProjectCostAnalysis.SaleCurrencyID);
            $("#sta-id").val(_master.ProjectCostAnalysis.Status);
            $("#sub-id").val(num.formatSpecial(_master.ProjectCostAnalysis.SubTotal, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(_master.ProjectCostAnalysis.SubTotalAfterDis, disSetting.Prices));
            $("#freight-value").val(num.formatSpecial(_master.ProjectCostAnalysis.FreightAmount, disSetting.Prices));
            $("#source-copy").val(_master.ProjectCostAnalysis.BasedCopyKeys);
            $("#dis-rate-id").val(num.formatSpecial(_master.ProjectCostAnalysis.DisRate, disSetting.Prices));
            $("#dis-value-id").val(num.formatSpecial(_master.ProjectCostAnalysis.DisValue, disSetting.Prices));
            setDate("#post-date", _master.ProjectCostAnalysis.PostingDate.toString().split("T")[0]);
            setDate("#valid-date", _master.ProjectCostAnalysis.ValidUntilDate.toString().split("T")[0]);
            setDate("#document-date", _master.ProjectCostAnalysis.DocumentDate.toString().split("T")[0]);
            $("#txt_idem").val(_master.ProjectCostAnalysis.SaleEMID);
            $("#saleem").val(_master.ProjectCostAnalysis.EmName);
            $("#txt_idowner").val(_master.ProjectCostAnalysis.OwnerID);
            $("#owner").val(_master.ProjectCostAnalysis.OwnerName);
            $("#remark-id").val(_master.ProjectCostAnalysis.Remarks);
            $("#total-id").val(num.formatSpecial(_master.ProjectCostAnalysis.TotalAmount, disSetting.Prices));
            $("#vat-value").val(num.formatSpecial(_master.ProjectCostAnalysis.VatValue, disSetting.Prices));
            $("#totalcommission").val(num.formatSpecial(_master.ProjectCostAnalysis.TotalCommission, disSetting.Prices));
      
            $("#txt_othercost").val(num.formatSpecial(_master.ProjectCostAnalysis.OtherCost, disSetting.Prices));
          
            $("#item-id").prop("disabled", false);
            var ex = findArray("CurID", _master.ProjectCostAnalysis.SaleCurrencyID, ExChange[0]);
            if (!!ex) {
                $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
                $(".cur-class").text(ex.CurName);
            }
      
            $listItemChoosed.bindRows(quote_details);
            setSummary(quote_details);
        } else {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Project CostAnalysis Not found!",
                close_button: "none"
            });
        }
    }


    function GetSelectItemMasterCopy(_master) {
        if (!!_master) {


            
            quote_details = [];
            master[0] = _master.ProjectCostAnalysis;
          
           
            _master.DetailItemMasterDatas.forEach(i => {
                i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Prices);
                i.TaxValue = num.formatSpecial(i.TaxValue, disSetting.Prices);
                i.DisRate = num.formatSpecial(i.DisRate, disSetting.Prices);
                i.TaxOfFinDisValue = num.formatSpecial(i.TaxOfFinDisValue, disSetting.Prices);
                i.FinDisRate = num.formatSpecial(i.FinDisRate, disSetting.Prices);
                i.FinDisValue = num.formatSpecial(i.FinDisValue, disSetting.Prices);
                i.Total = num.formatSpecial(i.Total, disSetting.Prices);
                i.Cost = num.formatSpecial(i.Cost, disSetting.Prices);
                i.LineTotalCost = num.formatSpecial(i.LineTotalCost, disSetting.Prices);
                i.UnitMargin = num.formatSpecial(i.UnitMargin, disSetting.Prices);
                i.TotalWTax = num.formatSpecial(i.TotalWTax, disSetting.Prices);
                i.LineTotalMargin = num.formatSpecial(i.LineTotalMargin, disSetting.Prices);
                i.FinTotalValue = num.formatSpecial(i.FinTotalValue, disSetting.Prices);
                i.UnitPriceAfterDis = num.formatSpecial(i.UnitPriceAfterDis, disSetting.Prices);
                i.LineTotalBeforeDis = num.formatSpecial(i.LineTotalBeforeDis, disSetting.Prices);
                quote_details.push(i);
            });


            _priceList = _master.Currency.ID;
            $("#txt_proid").val(_master.ProjectCostAnalysis.ID);
            $("#txt_proname").val(_master.ProjectCostAnalysis.Name);
            $("#txt_cusname").val(_master.ProjectCostAnalysis.CusName);
            $("#txt_idcus").val(_master.ProjectCostAnalysis.CusID);
            $("#txt_cuscode").val(_master.ProjectCostAnalysis.CusCode);
            $("#txt_idcontect").val(_master.ProjectCostAnalysis.ConTactID);
            $("#txt_contactperson").val(_master.ProjectCostAnalysis.ContName);
            $("#txt_phone").val(_master.ProjectCostAnalysis.Phone);
            
           
            $("#ware-id").val(_master.ProjectCostAnalysis.WarehouseID);
            $("#ref-id").val(_master.ProjectCostAnalysis.RefNo);
            $("#cur-id").val(_master.ProjectCostAnalysis.SaleCurrencyID);
            $("#sta-id").val(_master.ProjectCostAnalysis.Status);
            $("#sub-id").val(num.formatSpecial(_master.ProjectCostAnalysis.SubTotal, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(_master.ProjectCostAnalysis.SubTotalAfterDis, disSetting.Prices));
            $("#freight-value").val(num.formatSpecial(_master.ProjectCostAnalysis.FreightAmount, disSetting.Prices));
            $("#source-copy").val(_master.ProjectCostAnalysis.BasedCopyKeys);
            $("#dis-rate-id").val(num.formatSpecial(_master.ProjectCostAnalysis.DisRate, disSetting.Prices));
            $("#dis-value-id").val(num.formatSpecial(_master.ProjectCostAnalysis.DisValue, disSetting.Prices));
            setDate("#post-date", _master.ProjectCostAnalysis.PostingDate.toString().split("T")[0]);
            setDate("#valid-date", _master.ProjectCostAnalysis.ValidUntilDate.toString().split("T")[0]);
            setDate("#document-date", _master.ProjectCostAnalysis.DocumentDate.toString().split("T")[0]);
            $("#txt_idem").val(_master.ProjectCostAnalysis.SaleEMID);
            $("#saleem").val(_master.ProjectCostAnalysis.EmName);
            $("#txt_idowner").val(_master.ProjectCostAnalysis.OwnerID);
            $("#owner").val(_master.ProjectCostAnalysis.OwnerName);
            $("#remark-id").val(_master.ProjectCostAnalysis.Remarks);
            $("#total-id").val(num.formatSpecial(_master.ProjectCostAnalysis.TotalAmount, disSetting.Prices));
            $("#vat-value").val(num.formatSpecial(_master.ProjectCostAnalysis.VatValue, disSetting.Prices));
            $("#totalcommission").val(num.formatSpecial(_master.ProjectCostAnalysis.TotalCommission, disSetting.Prices));

            $("#txt_othercost").val(num.formatSpecial(_master.ProjectCostAnalysis.OtherCost, disSetting.Prices));

            $("#item-id").prop("disabled", false);
            var ex = findArray("CurID", _master.ProjectCostAnalysis.SaleCurrencyID, ExChange[0]);
            if (!!ex) {
                $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
                $(".cur-class").text(ex.CurName);
            }

            $listItemChoosed.bindRows(quote_details);
            setSummary(quote_details);
        } else {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Project CostAnalysis Not found!",
                close_button: "none"
            });
        }
    }
    /// submit data ///
    $("#submit-item").on("click", function (e) {
        const item_master = master[0];
        freightMaster.FreightProjCostDetails = freights.length === 0 ? new Array() : freights;
        item_master.Name = $("#txt_proname").val();
        item_master.CusID = parseInt($("#txt_idcus").val());
        item_master.ConTactID = parseInt($("#txt_idcontect").val());
        item_master.WarehouseID = parseInt($("#ware-id").val()); 
        item_master.RefNo = $("#ref-id").val();
        item_master.PostingDate = $("#post-date").val();
        item_master.ValidUntilDate = $("#valid-date").val();
        item_master.DocumentDate = $("#document-date").val();
        item_master.Remarks = $("#remark-id").val();
        item_master.ProjCostAnalysisDetails = quote_details.length === 0 ? new Array() : quote_details;
        item_master.FreightProjectCost = freightMaster;
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.SeriesDID = parseInt($("#SeriesDetailID").val());
        item_master.SeriesID = parseInt($("#invoice-no").val());
        item_master.DocTypeID = parseInt($("#DocumentTypeID").val());
        item_master.InvoiceNumber = parseInt($("#next_number").val());
        item_master.FreightAmount = $("#freight-value").val();
        item_master.SaleEMID = $("#txt_idem").val();
        item_master.OwnerID = $("#txt_idowner").val();

        item_master.DisValue = $("#dis-value-id").val();
        item_master.DisRate = $("#dis-rate-id").val();
        item_master.VatRate = $("#txt_taxpersentage").val();
        item_master.VatValue = $("#vat-value").val();
      //  $("#loading").prop("hidden", false);
       //console.log(item_master);
     
        $.ajax({
            url: "/ProjectCostAnalysis/UpdateProjCost",
            type: "post",
            data: $.antiForgeryToken({ data: JSON.stringify(item_master) }),
            success: function (model) {
                $("#submit-item").html("Save");
                if (model.Action == 1) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        }
                    }, model).refresh(800);
                   // $("#loading").prop("hidden", true);
                }
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, model);
                $(window).scrollTop(0);
              //  $("#loading").prop("hidden", true);
            }
        });
    });
    setWarehouse(0);
    function setWarehouse(id) {
        $.ajax({
            url: "/ProjectCostAnalysis/GetWarehouse",
            type: "Get",
            dataType: "Json",
            success: function (response) {
                $("#ware-id option:not(:first-child)").remove();
                $.each(response, function (i, item) {
                    $("#ware-id").append("<option value=" + item.ID + ">" + item.Name + "</option>");
                    if (!!id && item.ID === id) {
                        $("#ware-id").val(id);
                    }
                });
            }
        });
    }
    //$.get("/ProjectCostAnalysis/GetWarehouse", function (response) {
    //    $("#ware-id option:not(:first-child)").remove();
    //    $.each(response, function (i, item) {
    //        $("#ware-id").append("<option value=" + item.ID + ">" + item.Name + "</option>");
    //        if (!!id && item.ID === id) {
    //            $("#ware-id").val(id);
    //        }
    //    });
    //});
    // Modal Story ProjectCostAnalysis
    $("#History").click(() => {
        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: "List ProjectCostAnalysis",
            content: {
                selector: "#projectcost-content"
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown();
        });
        dialogprojmana.invoke(function () {
            /// Bind Customers /// 
            const $listprojectcost = ViewTable({
                keyField: "ID",
                selector: "#list-projcost",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Name", "InvoiceNumber", "PostingDate", "ValidUntilDate", "DocumentDate",],

                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {

                                $.ajax({
                                    url: "/ProjectCostAnalysis/FindProjectCost",
                                    // data: { number: $("#next_number").val(), seriesID: ) },
                                    data: { number: e.data.InvoiceNumber, seriesID: e.data.SeriesID },
                                    success: function (result) {
                                        console.log(result);
                                        getSaleItemMasters(result);
                                        $("#btn-addnew").prop("hidden", false);
                                        $("#btn-find").prop("hidden", true);
                                        $("#submit-item").html("Update");

                                    }
                                });
                                dialogprojmana.shutdown();
                            }
                        }
                    }
                ]
            });
            $.get("/ProjectCostAnalysis/GetstoryProjectCost", function (res) {
                $listprojectcost.bindRows(res);
            })
        });
    })
    //// bind customer
    //function bindCustomer(response) {
    //    const paging = new Kernel.DataPaging({
    //        pageSize: 10
    //    }).render(response, function (summary) {
    //        $(".modal-dialog #list-cus tr:not(:first-child)").remove();
    //        var _index = 1;
    //        $.each(summary.data, function (i, item) {
    //            var tr = $("<tr></tr>").on("dblclick", function () {
    //                dblCus(this);
    //            }).on("click", function () {
    //                clickCus(this);
    //            });
    //            tr.append("<td>" + _index + "</td>")
    //                .append("<td hidden>" + item.ID + "</td>")
    //                .append("<td>" + item.Code + "</td>")
    //                .append("<td>" + item.Name + "</td>")
    //                .append("<td>" + item.Type + "</td>")
    //                .append("<td>" + item.Phone + "</td>");
    //            $(".modal-dialog #list-cus").append(tr);
    //            _index++;
    //        });
    //        $(".modal-dialog .ck-data-loading").hide();
    //    });
    //    $("#data-paging-customer").html(paging.selfElement);
    //}
    //  dbl cus
    //function dblCus(c) {
    //    $("#show-list-cus").prop("disabled", false);
    //    var id = parseInt($(c).find("td:eq(1)").text());
    //    var name = $(c).find("td:eq(3)").text();
    //    $(".cus-id").val(name);
    //    master[0].CusID = id;
    //    $("#item-id").prop("disabled", false);
    //    $("#barcode-reading").prop("disabled", false).focus();
    //    $("#ModalCus").modal("hide");
    //}
    //// click Cus
    //var _nameCus = "";
    //var _idCus = 0;
    //function clickCus(c) {
    //    $("#show-list-cus").prop("disabled", false);
    //    _idCus = parseInt($(c).find("td:eq(1)").text());
    //    _nameCus = $(c).find("td:eq(3)").text();
    //    $(c).addClass("active").siblings().removeClass("active");
    //}
    function selectSeries(selected) {
        $.each(___PA.seriesPA, function (i, item) {
            if (item.Default == true) {
                $("<option selected value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
                $("#next_number").val(item.NextNo);
            }
            else {
                $("<option value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
            }
        });
        return selected.on('change')
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
    function totalRow(table, e, _this)
    {
      
        if (_this === '' || _this === '-') _this = 0;
        let qty = num.toNumberSpecial(e.data.Qty);
        let price = num.toNumberSpecial(e.data.UnitPrice);

        if (qty <= 0 || price <= 0) {
            table.updateColumn(e.key, "Total", num.formatSpecial(0.00, disSetting.Prices));
            table.updateColumn(e.key, "UnitMargin", num.formatSpecial(0.00, disSetting.Prices));
            table.updateColumn(e.key, "TotalWTax", num.formatSpecial(0.00, disSetting.Prices));
            table.updateColumn(e.key, "LineTotalMargin", num.formatSpecial(0.00, disSetting.Prices));
            table.updateColumn(e.key, "FinTotalValue", num.formatSpecial(0.00, disSetting.Prices));
            table.updateColumn(e.key, "TaxValue", num.formatSpecial(0.00, disSetting.Prices));
            table.updateColumn(e.key, "TaxOfFinDisValue", num.formatSpecial(0.00, disSetting.Prices));
            table.updateColumn(e.key, "LineTotalBeforeDis", num.formatSpecial(0.00, disSetting.Prices));
            table.updateColumn(e.key, "UnitPriceAfterDis", num.formatSpecial(0.00, disSetting.Prices));
            $("#txt_taxpersentage").val(0);
            $("#txt_totalamountpersentage").val(0);
            $("#txt_totalmgpersentage").val(0);
            $("#txt_totalcommitpersentage").val(0);
            $("#txt_otherpersentage").val(0);
            $("#txt_totalprofit").val(0);
           // $("#dis-rate-id").val(0);
        }
        else {
            let unitpriceafterdis = num.toNumberSpecial(price) - num.toNumberSpecial(e.data.DisValue);
            let linetotalbeforedis = num.toNumberSpecial(qty) * num.toNumberSpecial(price);
            let linetotalafterdis = linetotalbeforedis - num.toNumberSpecial(e.data.DisValue); //num.toNumberSpecial(qty) * num.toNumberSpecial(e.data.DisValue);
            let linetotalcost = num.toNumberSpecial(qty) * num.toNumberSpecial(e.data.Cost);
            let vatValue = num.toNumberSpecial(e.data.TaxRate) * linetotalafterdis === 0 ? 0 : num.toNumberSpecial(e.data.TaxRate) * linetotalafterdis / 100;
            let totalrow = linetotalafterdis + vatValue;//final total value

            // Update Object
            updatedetail(quote_details, e.data.UomID, e.key, "TotalWTax", isNaN(totalrow) ? 0 : totalrow);
            updatedetail(quote_details, e.data.UomID, e.key, "Total", linetotalafterdis);
            updatedetail(quote_details, e.data.UomID, e.key, "UnitPriceAfterDis", unitpriceafterdis);
            updatedetail(quote_details, e.data.UomID, e.key, "LineTotalBeforeDis", linetotalbeforedis);
            updatedetail(quote_details, e.data.UomID, e.key, "LineTotalCost", linetotalcost);

            let unitmargin = num.toNumberSpecial(e.data.UnitPrice) - num.toNumberSpecial(e.data.Cost);
            updatedetail(quote_details, e.data.UomID, e.key, "UnitMargin", isNaN(unitmargin) ? 0 : unitmargin);

            let linetotalmargin = num.toNumberSpecial(unitmargin) * num.toNumberSpecial(e.data.Qty);
            updatedetail(quote_details, e.data.UomID, e.key, "LineTotalMargin", isNaN(linetotalmargin) ? 0 : linetotalmargin);

            //const taxFinDisValue = disRate === 0 ? vatValue : disRate * (linetotalafterdis - disValue) === 0 ? 0 : disRate * (linetotalafterdis - disValue) / 100;
            const taxFinDisValue = num.toNumberSpecial(e.data.FinTotalValue) * (num.toNumberSpecial(e.data.TaxRate) / 100);
            const finalTotal = (num.toNumberSpecial(e.data.Total)) - num.toNumberSpecial(e.data.FinDisValue);

            updatedetail(quote_details, e.data.UomID, e.key, "FinTotalValue", finalTotal);
            updatedetail(quote_details, e.data.UomID, e.key, "TaxOfFinDisValue", taxFinDisValue);


            //Update View
            table.updateColumn(e.key, "UnitPriceAfterDis", num.formatSpecial(unitpriceafterdis, disSetting.Prices));
            table.updateColumn(e.key, "LineTotalBeforeDis", num.formatSpecial(linetotalbeforedis, disSetting.Prices));
            table.updateColumn(e.key, "TaxOfFinDisValue", num.formatSpecial(taxFinDisValue, disSetting.Prices));
            table.updateColumn(e.key, "Total", num.formatSpecial(linetotalafterdis, disSetting.Prices));
            table.updateColumn(e.key, "LineTotalCost", num.formatSpecial(linetotalcost, disSetting.Prices));
            table.updateColumn(e.key, "TotalWTax", num.formatSpecial(isNaN(totalrow) ? 0 : totalrow, disSetting.Prices));
            table.updateColumn(e.key, "TaxValue", num.formatSpecial(isNaN(vatValue) ? 0 : vatValue, disSetting.Prices));
            table.updateColumn(e.key, "TaxRate", num.formatSpecial(isNaN(e.data.TaxRate) ? 0 : e.data.TaxRate, disSetting.Prices));
            table.updateColumn(e.key, "FinTotalValue", num.formatSpecial(isNaN(e.data.FinTotalValue) ? 0 : e.data.FinTotalValue, disSetting.Prices));
            table.updateColumn(e.key, "UnitMargin", num.formatSpecial(isNaN(unitmargin) ? 0 : unitmargin, disSetting.Prices));
            table.updateColumn(e.key, "LineTotalMargin", num.formatSpecial(isNaN(linetotalmargin) ? 0 : linetotalmargin, disSetting.Prices));

        }


        //const totalWDis = num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice) - num.toNumberSpecial(e.data.DisValue);
        //let vatValue = num.toNumberSpecial(e.data.TaxRate) * totalWDis === 0 ? 0 :
        //    num.toNumberSpecial(e.data.TaxRate) * totalWDis / 100;
        //let totalrow = num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice) - num.toNumberSpecial(e.data.DisValue) + vatValue;

        //// Update Object
        //updatedetail(quote_details, e.data.UomID, e.key, "TotalWTax", isNaN(totalrow) ? 0 : totalrow);
        //updatedetail(quote_details, e.data.UomID, e.key, "Total", isNaN(totalrow - vatValue) ? 0 : totalrow - vatValue);


        //let subtotal = 0;
        //let disRate = num.toNumberSpecial($("#dis-rate-id").val());
        //quote_details.forEach(i => {
        //    subtotal += num.toNumberSpecial(i.Total) - num.toNumberSpecial(i.DisValue);
        //});
        //const disValue = (disRate * subtotal) === 0 ? 0 : (disRate * subtotal) / 100;
        //const taxFinDisValue = disRate === 0 ? vatValue : disRate * (totalWDis - disValue) === 0 ? 0 :
        //    disRate * (totalWDis - disValue) / 100;
        //const finalTotal = num.toNumberSpecial(e.data.Total) - disValue;
        //updatedetail(quote_details, e.data.UomID, e.key, "FinTotalValue", finalTotal);
        //updatedetail(quote_details, e.data.UomID, e.key, "TaxOfFinDisValue", taxFinDisValue);

        ////Update View
        //table.updateColumn(e.key, "TaxOfFinDisValue", num.formatSpecial(isNaN(taxFinDisValue) ? 0 : taxFinDisValue, disSetting.Prices));
        //table.updateColumn(e.key, "Total", num.formatSpecial(isNaN(totalrow - vatValue) ? 0 : totalrow - vatValue, disSetting.Prices));
        //table.updateColumn(e.key, "TotalWTax", num.formatSpecial(isNaN(totalrow) ? 0 : totalrow, disSetting.Prices));
        //table.updateColumn(e.key, "TaxValue", num.formatSpecial(isNaN(vatValue) ? 0 : vatValue, disSetting.Prices));
        //table.updateColumn(e.key, "TaxRate", num.formatSpecial(isNaN(e.data.TaxRate) ? 0 : e.data.TaxRate, disSetting.Rate));
        //table.updateColumn(e.key, "FinTotalValue", num.formatSpecial(isNaN(e.data.FinTotalValue) ? 0 : e.data.FinTotalValue, disSetting.Prices));
    }
    function calDisRate(e, _this)
    {
        let value = num.toNumberSpecial(_this)
        if (value > 100) value = 100;
       // if (value < 0) value = 0;
       
        updatedetail(quote_details, e.data.UomID, e.key, "DisRate", num.toNumberSpecial(value));
        let disvalue = ((value / 100) * (num.toNumberSpecial(e.data.UnitPrice) * num.toNumberSpecial(e.data.Qty)));

        updatedetail(quote_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(num.toNumberSpecial(disvalue), disSetting.Prices));
        totalRow($listItemChoosed, e, value);
    }
    function calDisValue(e, _this)
    {
        console.log(_this);
        let value = num.toNumberSpecial(_this)
        if (value > num.toNumberSpecial(e.data.LineTotalBeforeDis))
            value = num.toNumberSpecial(e.data.LineTotalBeforeDis);
        if (value == '' || value < 0)
            value = 0;
        console.log(value);
        updatedetail(quote_details, e.data.UomID, e.key, "DisValue", value);
        const ratedis = (value / (num.toNumberSpecial(e.data.UnitPrice) * num.toNumberSpecial(e.data.Qty))) * 100;
        updatedetail(quote_details, e.data.UomID, e.key, "DisRate", ratedis);

        $listItemChoosed.updateColumn(e.key, "DisRate", num.formatSpecial(num.toNumberSpecial(ratedis), disSetting.Rates));
        totalRow($listItemChoosed, e, value);

    }
    function calDisvalue(e)
    {
        let disvalue = num.toNumberSpecial(e.data.DisRate) * num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice) === 0 ? 0 :
            (num.toNumberSpecial(e.data.DisRate) * num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice)) / 100;
        if (disvalue > num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice)) disvalue = num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice);
        updatedetail(quote_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
    }
    function setSummary(data)
    {
        const _master = master[0];
        let subtotal = 0;
        var total = 0;
        let vat = 0;
        let linetotalcost = 0;
        let disRate = $("#dis-rate-id").val();
        if (disRate == "" || isNaN(disRate))
            disRate = 0;
        _master.TotalCommission = num.toNumberSpecial($("#totalcommission").val());
        _master.OtherCost = num.toNumberSpecial($("#txt_othercost").val());

        if (num.toNumberSpecial(_master.TotalCommission) <= 0 || _master.TotalCommission == "" || isNaN(_master.TotalCommission))
            _master.TotalCommission = 0;
        if (num.toNumberSpecial(_master.OtherCost) <= 0 || _master.OtherCost == "" || isNaN(_master.OtherCost))
            _master.OtherCost = 0;

        data.forEach(i => {
            subtotal += num.toNumberSpecial(i.Total);
            total += num.toNumberSpecial(i.Total);
            vat += num.toNumberSpecial(i.TaxOfFinDisValue);
            linetotalcost += num.toNumberSpecial(i.LineTotalCost);
        });
        const disValue = (num.toNumberSpecial(disRate) / 100) * subtotal;
        
        _master.SubTotalSys = subtotal * _master.ExchangeRate;
        _master.SubTotal = subtotal;
        _master.VatRate = (vat * 100) === 0 ? 0 : vat * 100 / subtotal;
        _master.VatValue = vat + num.toNumberSpecial(freightMaster.TaxSumValue);
        _master.DisValue = disValue;
        _master.SubTotalAfterDis = subtotal - disValue;
        _master.SubTotalBefDis = subtotal;
        _master.DisRate = disRate;
        _master.TotalAmount = num.toNumberSpecial(_master.SubTotalAfterDis) + _master.VatValue + num.toNumberSpecial(freightMaster.AmountReven);
        _master.TotalAmountSys = _master.TotalAmount * _master.ExchangeRate;
        _master.TotalMargin = num.toNumberSpecial(_master.SubTotalAfterDis) - linetotalcost;

        _master.ExpectedTotalProfit = num.toNumberSpecial(_master.TotalMargin) - num.toNumberSpecial(_master.TotalCommission) - num.toNumberSpecial(_master.OtherCost)
        // calculate percentage
        let taxpercent =( num.toNumberSpecial(_master.VatValue) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let totalamountpercent = (num.toNumberSpecial(_master.TotalAmount) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let marginpercent = (num.toNumberSpecial(_master.TotalMargin) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let profitpercent = (num.toNumberSpecial(_master.ExpectedTotalProfit) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let permissionpercent = (num.toNumberSpecial(_master.TotalCommission) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let otherpercent = (num.toNumberSpecial(_master.OtherCost) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let disvalue = (num.toNumberSpecial(disRate) / 100) * subtotal;


        quote_details.forEach(i => {
            const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_master.DisRate) === 0 ? 0 : num.toNumberSpecial(i.Total) * (num.toNumberSpecial(_master.DisRate) / 100);
           
            $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
            $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(disRate, disSetting.Prices));
            updatedetail(quote_details, i.UomID, i.LineID, "FinDisValue", value);
            const lastDisval = num.toNumberSpecial(i.Total) - (num.toNumberSpecial(_master.DisRate) * num.toNumberSpecial(i.Total) / 100);
            const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 : num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
            updatedetail(quote_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
            updatedetail(quote_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
            $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
            $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
        });
        if (subtotal <= 0) {
            $("#txt_taxpersentage").val(num.formatSpecial(0, disSetting.Prices));
            $("#txt_totalamountpersentage").val(num.formatSpecial(0, disSetting.Prices));
            $("#txt_totalmgpersentage").val(num.formatSpecial(0, disSetting.Prices));

            $("#txt_totalcommitpersentage").val(num.formatSpecial(0, disSetting.Prices));
            $("#txt_otherpersentage").val(num.formatSpecial(0, disSetting.Prices));
            $("#txt_totalprofit").val(num.formatSpecial(0, disSetting.Prices));
        }
        else {
            $("#txt_taxpersentage").val(num.formatSpecial(taxpercent, disSetting.Prices));
            $("#txt_totalamountpersentage").val(num.formatSpecial(totalamountpercent, disSetting.Prices));
            $("#txt_totalmgpersentage").val(num.formatSpecial(marginpercent, disSetting.Prices));

            $("#txt_totalcommitpersentage").val(num.formatSpecial(permissionpercent, disSetting.Prices));
            $("#txt_otherpersentage").val(num.formatSpecial(otherpercent, disSetting.Prices));
            $("#txt_totalprofit").val(num.formatSpecial(profitpercent, disSetting.Prices));
        }

        $("#vat-value").val(num.formatSpecial(_master.VatValue, disSetting.Prices));
        $("#sub-id").val(num.formatSpecial(subtotal, disSetting.Prices));
        $("#dis-value-id").val(num.formatSpecial(disvalue, disSetting.Prices));
        $("#total-id").val(num.formatSpecial(_master.TotalAmount, disSetting.Prices));
        $("#sub-after-dis").val(num.formatSpecial(_master.SubTotalAfterDis, disSetting.Prices));
        $("#totalmargin").val(num.formatSpecial(_master.TotalMargin, disSetting.Prices));
        $("#expectedtotalprofit").val(num.formatSpecial(_master.ExpectedTotalProfit, disSetting.Prices));
        // block permission
       
    }




    function setSummary2(data)
    {
        const _master = master[0];
        let subtotal = 0;
        var total = 0;
        let vat = 0;
        let linetotalcost = 0;
        let disRate = $("#dis-rate-id").val();
        if (disRate == "" || isNaN(disRate))
            disRate = 0;
        _master.TotalCommission = num.toNumberSpecial($("#totalcommission").val());
        _master.OtherCost = num.toNumberSpecial($("#txt_othercost").val());

        if (num.toNumberSpecial(_master.TotalCommission) <= 0 || _master.TotalCommission == "" || isNaN(_master.TotalCommission))
            _master.TotalCommission = 0;
        if (num.toNumberSpecial(_master.OtherCost) <= 0 || _master.OtherCost == "" || isNaN(_master.OtherCost))
            _master.OtherCost = 0;

        data.forEach(i => {
            subtotal += num.toNumberSpecial(i.Total);
            total += num.toNumberSpecial(i.Total);
            vat += num.toNumberSpecial(i.TaxOfFinDisValue);
            linetotalcost += num.toNumberSpecial(i.LineTotalCost);
        });
        const disValue = (num.toNumberSpecial(disRate) / 100) * subtotal;

        _master.SubTotalSys = subtotal * _master.ExchangeRate;
        _master.SubTotal = subtotal;
        _master.VatRate = (vat * 100) === 0 ? 0 : vat * 100 / subtotal;
        _master.VatValue = vat + num.toNumberSpecial(freightMaster.TaxSumValue);
        _master.DisValue = disValue;
        _master.SubTotalAfterDis = subtotal - disValue;
        _master.SubTotalBefDis = subtotal;
        _master.DisRate = disRate;
        _master.TotalAmount = num.toNumberSpecial(_master.SubTotalAfterDis) + _master.VatValue + num.toNumberSpecial(freightMaster.AmountReven);
        _master.TotalAmountSys = _master.TotalAmount * _master.ExchangeRate;
        _master.TotalMargin = subtotal - linetotalcost;

        _master.ExpectedTotalProfit = (subtotal - linetotalcost) - num.toNumberSpecial(_master.TotalCommission) - num.toNumberSpecial(_master.OtherCost)
        // calculate percentage
        let taxpercent = (num.toNumberSpecial(_master.VatValue) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let totalamountpercent = (num.toNumberSpecial(_master.TotalAmount) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let marginpercent = (num.toNumberSpecial(_master.TotalMargin) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let profitpercent = (num.toNumberSpecial(_master.ExpectedTotalProfit) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let permissionpercent = (num.toNumberSpecial(_master.TotalCommission) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
        let otherpercent = (num.toNumberSpecial(_master.OtherCost) / num.toNumberSpecial(_master.SubTotalAfterDis)) * 100;
     


        quote_details.forEach(i => {
            const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_master.DisRate) === 0 ? 0 : num.toNumberSpecial(i.Total) * (num.toNumberSpecial(_master.DisRate) / 100);

            $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
            $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(disRate, disSetting.Prices));
            updatedetail(quote_details, i.UomID, i.LineID, "FinDisValue", value);
            const lastDisval = num.toNumberSpecial(i.Total) - (num.toNumberSpecial(_master.DisRate) * num.toNumberSpecial(i.Total) / 100);
            const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 : num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
            updatedetail(quote_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
            updatedetail(quote_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
            $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
            $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
        });
        if (subtotal <= 0) {
            $("#txt_taxpersentage").val(num.formatSpecial(0, disSetting.Prices));
            $("#txt_totalamountpersentage").val(num.formatSpecial(0, disSetting.Prices));
            $("#txt_totalmgpersentage").val(num.formatSpecial(0, disSetting.Prices));

            $("#txt_totalcommitpersentage").val(num.formatSpecial(0, disSetting.Prices));
            $("#txt_otherpersentage").val(num.formatSpecial(0, disSetting.Prices));
            $("#txt_totalprofit").val(num.formatSpecial(0, disSetting.Prices));
        }
        else {
            $("#txt_taxpersentage").val(num.formatSpecial(taxpercent, disSetting.Prices));
            $("#txt_totalamountpersentage").val(num.formatSpecial(totalamountpercent, disSetting.Prices));
            $("#txt_totalmgpersentage").val(num.formatSpecial(marginpercent, disSetting.Prices));

            $("#txt_totalcommitpersentage").val(num.formatSpecial(permissionpercent, disSetting.Prices));
            $("#txt_otherpersentage").val(num.formatSpecial(otherpercent, disSetting.Prices));
            $("#txt_totalprofit").val(num.formatSpecial(profitpercent, disSetting.Prices));
        }

        $("#vat-value").val(num.formatSpecial(_master.VatValue, disSetting.Prices));
        $("#sub-id").val(num.formatSpecial(subtotal, disSetting.Prices));
        $("#total-id").val(num.formatSpecial(_master.TotalAmount, disSetting.Prices));
        $("#sub-after-dis").val(num.formatSpecial(_master.SubTotalAfterDis, disSetting.Prices));
        $("#totalmargin").val(num.formatSpecial(_master.TotalMargin, disSetting.Prices));
        $("#expectedtotalprofit").val(num.formatSpecial(_master.TotalMargin, disSetting.Prices));
        // block permission

    }
    function disInputUpdate(_master) {
        $("#dis-rate-id").val(num.formatSpecial(_master.DisRate, disSetting.Prices));
        $("#dis-value-id").val(num.formatSpecial(_master.DisValue, disSetting.Prices));
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function findArray(keyName, keyValue, values) {
      
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
    function updatedetail(data, uomid, key, prop, value) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i.LineID === key && i.UomID === uomid) {
                    i[prop] = value;
                }
            });
        }
    }
    function updatedetailFreight(data, key, prop, value) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i.LineID === key) {
                    i[prop] = value;
                }
            });
        }
    }
    function invoiceDisAllRowRate(_this) {
        if (isValidArray(quote_details)) {
            quote_details.forEach(i => {
                const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) / 100;
                if (_this.value < 100)
                {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Prices));
                    updatedetail(quote_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(quote_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(value);
                    const taxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;

                    updatedetail(quote_details, i.UomID, i.LineID, "TaxOfFinDisValue", taxOfFinDisValue);
                    updatedetail(quote_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(taxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                } else if (_this.value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Prices));
                    updatedetail(quote_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(quote_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    updatedetail(quote_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(quote_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(quote_details);
        }
    }
    function invoiceDisAllRowValue(value) {
        if (isValidArray(quote_details)) {
            quote_details.forEach(i => {
                const _value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) / 100;
                if (value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(_value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Prices));
                    updatedetail(quote_details, i.UomID, i.LineID, "FinDisValue", _value);
                    updatedetail(quote_details, i.UomID, i.LineID, "FinDisRate", value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(_value);
                    const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
                    updatedetail(quote_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
                    updatedetail(quote_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                }
                else if (value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Prices));
                    updatedetail(quote_details, i.UomID, i.LineID, "FinDisValue",_value);
                    updatedetail(quote_details, i.UomID, i.LineID, "FinDisRate", value);

                    updatedetail(quote_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(quote_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary2(quote_details);
        }

    } 
    function totalRowFreight(table, e) {
        table.updateColumn(e.key, "TaxRate", num.formatSpecial(e.data.TaxRate, disSetting.Rates));
        const taxValue = num.toNumberSpecial(e.data.TaxRate) * num.toNumberSpecial(e.data.Amount) === 0 ? 0 :
            num.toNumberSpecial(e.data.TaxRate) * num.toNumberSpecial(e.data.Amount) / 100;
        const amountWTax = num.toNumberSpecial(e.data.Amount) + taxValue;
        updatedetailFreight(freights, e.key, "TotalTaxAmount", taxValue);
        updatedetailFreight(freights, e.key, "AmountWithTax", amountWTax);

        table.updateColumn(e.key, "TotalTaxAmount", num.formatSpecial(taxValue, disSetting.Prices));
        table.updateColumn(e.key, "AmountWithTax", num.formatSpecial(amountWTax, disSetting.Prices));
    }
    function sumSummaryFreight(data) {
        if (isValidArray(data)) {
            let sumFreight = 0;
            let sumTax = 0;
            data.forEach(i => {
                sumFreight += num.toNumberSpecial(i.Amount);
                sumTax += num.toNumberSpecial(i.TotalTaxAmount);
            });
            freightMaster.AmountReven = sumFreight;
            freightMaster.TaxSumValue = sumTax;
            $(".freightSumAmount").val(num.formatSpecial(sumFreight, disSetting.Prices));
        }
    }
    function updateSelect(data, key, prop)
    {
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
    // Copy Project Cost Analysis to SaleQuote
    $("#txtcopy").change(function () {
        
 
        let value = parseInt($(this).val());
        let cusid = parseInt($("#txt_idcus").val());
        if (value == 1) {
      
            $.get("/ProjectCostAnalysis/GetProjAnalysisinQuote/", { id: cusid }, function (res) {
               
                CopyDataSaleQuote(res, "List Sale Quote");

            })
        }
        else if (value == 2) {
            $.get("/ProjectCostAnalysis/GetstorySolutionData/", { id: cusid }, function (res) {

                CopyDataSolutonData(res,"List Solution Data Management");
            })
        }
        $(this).val(0);
        
    })
    function CopyDataSaleQuote(res,titile) {
        

        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: titile,
            content: {
                selector: "#projectcost-content"
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown();
        });
        dialogprojmana.invoke(function () {
            /// Bind Customers /// 
            $("#txtSearchprojcost").on("keyup", function (e) {
                var value = $(this).val().toLowerCase();
                $("#list-projcost tr:not(:first-child)").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
            const $listprojectcost = ViewTable({
                keyField: "ID",
                selector: "#list-projcost",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Name", "InvoiceNumber", "PostingDate", "ValidUntilDate", "DocumentDate",],

                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                storeBaseonID = e.data.BaseOnID;
                                storeCopy = e.data.CopyTypeSQ;
                                storeKey = e.data.KeyCopy;
                                $.get("/ProjectCostAnalysis/FindProjectCost/", { number: e.data.InvoiceNumber, seriesID: e.data.SeriesID }, function (res) {
                                    res.ProjectCostAnalysis.ID = 0;
                                    res.ProjectCostAnalysis.BaseOnID = storeBaseonID;
                                    res.ProjectCostAnalysis.CopyType = storeCopy;
                                    res.ProjectCostAnalysis.KeyCopy = storeKey;
                                    res.ProjectCostAnalysis.FreightProjectCost.ID = 0;
                                    res.ProjectCostAnalysis.FreightProjectCost.ID = 0;
                                    res.ProjectCostAnalysis.FreightProjectCost.ProjCAID = 0;
                                    res.ProjectCostAnalysis.FreightProjectCost.FreightProjCostDetails.forEach(i => {
                                        i.ID = 0;
                                        i.FreightProjectCostID = 0;

                                    })
                                    res.DetailItemMasterDatas.forEach(i => {
                                        i.ID = 0;
                                        i.ProjCostID = 0;
                                    });
                                    getSaleItemMasters(res);
                                    storeBaseonID = 0;
                                    storeCopy = 0;
                                    storeKey = "";
                                   
                                })

                                //location.href = "/Sale/SaleQuote?obj=" + JSON.stringify(obj);

                                dialogprojmana.shutdown();
                            }
                        }
                    }
                ]
            });
           
                $listprojectcost.bindRows(res);
            
        });
    }
    function CopyDataSolutonData(res, titile) {


        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: titile,
            content: {
                selector: "#projectcost-content"
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown();
        });
        dialogprojmana.invoke(function () {
            /// Bind Customers /// 
            $("#txtSearchprojcost").on("keyup", function (e) {
                var value = $(this).val().toLowerCase();
                $("#list-projcost tr:not(:first-child)").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
            const $listprojectcost = ViewTable({
                keyField: "ID",
                selector: "#list-projcost",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Name", "InvoiceNumber", "PostingDate", "ValidUntilDate", "DocumentDate",],

                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                $.get("/ProjectCostAnalysis/CopySolutionData/", { number: e.data.InvoiceNumber, seriesID: e.data.SeriesID }, function (res) {
                                    res.ProjectCostAnalysis.ID = 0;
                                    res.ProjectCostAnalysis.TypeDis="Prices";
                                 
                                    res.DetailItemMasterDatas.forEach(i => {
                                        i.ID = 0;
                                        i.ProjCostID = 0;
                                    })
                                    GetSelectItemMasterCopy(res);
                                    console.log(res);
                                })

                                //location.href = "/Sale/SaleQuote?obj=" + JSON.stringify(obj);

                                dialogprojmana.shutdown();
                            }
                        }
                    }
                ]
            });

            $listprojectcost.bindRows(res);

        });
    }

})
