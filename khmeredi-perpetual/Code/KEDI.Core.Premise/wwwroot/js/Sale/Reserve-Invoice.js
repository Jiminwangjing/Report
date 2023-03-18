"use strict"
let att_chement = [];
let re_details = [],
    _currency = "",
    _curencyID = 0,
    master = [],
    ExChange = [],
    ARDownPayments = [],
    freights = [],
    freightMaster = {},
    __singleElementJE,
    _nameCus = "",
    _idCus = 0,
    serials = [],
    batches = [],
    type = "AR",
    isCopied = false,
    __ardpSum = {
        sum: 0,
        taxSum: 0,
    },
    _priceList = 0,
    ___IN = JSON.parse($("#data-invoice").text()),
    /// class of colunms items ////
    itemArr = [".remark", ".uom", ".unitprice", ".disvalue", ".disrate", ".taxgroup"],
    disSetting = ___IN.genSetting.Display;
freightMaster.IsEditabled = true;

$(document).ready(function () {
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });
    $.each($("[data-date]"), function (i, t) {
        setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
    });
    // tb master
    var itemmaster = {
        ID: 0,
        CusID: 0,
        BranchID: 0,
        WarehouseID: 0,
        RequestedBy: 0,
        ShippedBy: 0,
        Types: "",
        ReceivedBy: 0,
        UserID: 0,
        SaleCurrencyID: 0,
        RefNo: "",
        InvoiceNo: "",
        InvoiceNumber: "",
        ExchangeRate: 0,
        PostingDate: "",
        DueDate: "",
        DocumentDate: "",
        IncludeVat: false,
        Status: "open",
        Remarks: "",
        SubTotal: 0,
        SubTotalSys: 0,
        DisRate: 0,
        DisValue: 0,
        TypeDis: "Percent",
        VatRate: 0,
        VatValue: 0,
        AppliedAmount: 0,
        FeeNote: 0,
        FeeAmount: 0,
        TotalAmount: 0,
        TotalAmountSys: 0,
        CopyType: 0,
        CopyKey: "",
        BasedCopyKeys: "",
        LocalSetRate: 0,
        PriceListID: 0,
        DownPayment: 0,
        BaseOnID: 0,
        ARReserveInvoiceDetails: new Array()
    }
    master.push(itemmaster);
    // Invoice
    ___IN.seriesIN.forEach(i => {
        if (i.Default == true) {
            $("#DocumentTypeID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    ___IN.seriesJE.forEach(i => {
        if (i.Default == true) {
            $("#JEID").val(i.ID);
            $("#JENumber").val(i.NextNo);
            __singleElementJE = findArray("ID", i.ID, ___IN.seriesJE);
        }
    });
    let selected = $("#invoice-no");
    selectSeries(selected);
    $('#invoice-no').change(function () {
        var id = $(this).val();
        var seriesIN = findArray("ID", id, ___IN.seriesIN);
        ___IN.seriesIN.Number = seriesIN.NextNo;
        ___IN.seriesIN.ID = id;
        $("#DocumentID").val(seriesIN.DocumentTypeID);
        $("#number").val(seriesIN.NextNo);
        $("#next_number").val(seriesIN.NextNo);
    });
    if (___IN.seriesIN.length == 0) {
        $('#invoice-no').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
        $("#submit-item").prop("disabled", true);
    }
    // get warehouse
    setWarehouse(0);
    // get Default currency
    $.ajax({
        url: "/Sale/GetDefaultCurrency",
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
        url: "/Sale/GetPriceList",
        type: "Get",
        dataType: "Json",
        success: function (response) {
            $("#cur-id option:not(:first-child)").remove();
            $.each(response, function (i, item) {
                $("#cur-id").append("<option  value=" + item.ID + ">" + item.Name + "</option>");
            });
        }
    });
    $(".modal-header").on("mousedown", function (mousedownEvt) {
        var $draggable = $(this);
        var x = mousedownEvt.pageX - $draggable.offset().left,
            y = mousedownEvt.pageY - $draggable.offset().top;
        $("body").on("mousemove.draggable", function (mousemoveEvt) {
            $draggable.closest(".modal-dialog").offset({
                "left": mousemoveEvt.pageX - x,
                "top": mousemoveEvt.pageY - y
            });
        });
        $("body").one("mouseup", function () {
            $("body").off("mousemove.draggable");
        });
        $draggable.closest(".modal").one("bs.modal.hide", function () {
            $("body").off("mousemove.draggable");
        });
    });
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
    // get customer
    $("#show-list-cus").click(function () {
        const cusDialog = new DialogBox({
            content: {
                selector: "#cus-content"
            },
            caption: "Customers",
        });
        cusDialog.invoke(function () {
            const customers = ViewTable({
                keyField: "ID",
                selector: "#list-cus",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "Type", "Phone"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                const id = e.key;
                                const name = e.data.Name;
                                $("#cus-id").val(name);
                                master[0].CusID = id;
                                $("#item-id").prop("disabled", false);
                                $("#barcode-reading").prop("disabled", false).focus();
                                $("#copy-from").prop("disabled", false);
                                $("#tdp-dailog").removeClass("disabled");


                                cusDialog.shutdown();
                                getARDownPayments(id, "close", 0, function (res) {
                                    if (isValidArray(res)) {
                                        if (isValidArray(ARDownPayments)) {
                                            ARDownPayments = [];
                                            res.forEach(i => {
                                                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                                                ARDownPayments.push(i);
                                            });
                                        }
                                        else {
                                            res.forEach(i => {
                                                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                                                ARDownPayments.push(i);
                                            });
                                        }
                                    }
                                });
                            }
                        }
                    }
                ]
            });
            $.ajax({
                url: "/Sale/GetCustomer",
                type: "Get",
                dataType: "Json",
                success: function (response) {
                    customers.bindRows(response);
                    $("#find-cus").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let rex = new RegExp(__value, "gi");
                        let __customers = $.grep(response, function (person) {
                            let phone = isEmpty(person.Phone) ? "" : person.Phone;
                            return person.Code.match(rex) || person.Name.replace(/\s+/, "").match(rex)
                                || phone.replace(/\s+/, "").match(rex)
                                || person.Type.match(rex)
                        });
                        customers.bindRows(__customers);
                    });
                }
            });

        })
        cusDialog.confirm(function () {
            cusDialog.shutdown()
        })
    });

    function isEmpty(value) {
        return value == undefined || value == null || value == "";
    }

    $("#post-date").change(function () {
        var days = $("#days").val();
        var sdays = new Date(this.value);
        sdays.setDate(sdays.getDate() + parseInt(days) - 1);
        document.getElementById("due-date").valueAsDate = sdays;
    })
    var semid = 0;
    var semnale = "";
    $("#cus-cancel").click(function () {
        $("#sale-emid").val(0);
        $("#sale-em").val("");
    });
    $("#cus-choosed").click(function () {
        $("#sale-emid").val(semid);
        $("#sale-em").val(semnale);
        $("#cus-id").val(_nameCus);
        master.forEach(i => {
            i.CusID = _idCus;
        })
        $("#barcode-reading").prop("disabled", false).focus();
        $("#item-id").prop("disabled", false);
        $("#ModalCus").modal("hide");
        $("#copy-from").prop("disabled", false);
        $("#tdp-dailog").removeClass("disabled");
        getARDownPayments(_idCus, "close", 0, function (res) {
            if (isValidArray(res)) {
                if (isValidArray(ARDownPayments)) {
                    ARDownPayments = [];
                    res.forEach(i => {
                        i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                        ARDownPayments.push(i);
                    });
                }
                else {
                    res.forEach(i => {
                        i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                        ARDownPayments.push(i);
                    });
                }
            }
        })
    });
    $.ajax({
        url: "/Sale/GetExchange",
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
        selector: $("#list-detail"),
        paging: {
            pageSize: 10,
            enabled: false
        },
        dynamicCol: {
            afterAction: true,
            headerContainer: "#col-to-append-after-detail",
        },
        visibleFields: [
            "ItemCode", "UnitPrice", "BarCode", "Qty", "TaxGroupList", "TaxRate", "TaxValue", "TotalWTax", "Total", "Currency",
            "UoMs", "ItemNameKH", "DisValue", "DisRate", "Remarks", "FinDisRate", "FinDisValue", "TaxOfFinDisValue", "FinTotalValue"
        ],
        columns: [
            {
                name: "UnitPrice",
                template: `<input class='form-control font-size unitprice' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updatedetail(re_details, e.data.UomID, e.key, "UnitPrice", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this);
                        setSummary(re_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "Currency",
                template: `<span class='font-size cur'></span`,
            },
            {
                name: "DisValue",
                template: `<input class='form-control font-size disvalue' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        calDisValue(e, this);
                        totalRow($listItemChoosed, e, this);
                        setSummary(re_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "DisRate",
                template: `<input class='form-control font-size disrate' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        calDisRate(e, this);
                        totalRow($listItemChoosed, e, this);
                        setSummary(re_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "Qty",
                template: `<input class='form-control font-size qty' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        if (isCopied && this.value > e.data.OpenQty) this.value = e.data.OpenQty;
                        updatedetail(re_details, e.data.UomID, e.key, "Qty", this.value);
                        //calDisrate(e);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this);
                        setSummary(re_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "TaxGroupList",
                template: `<select class='form-control font-size taxgroup'></select>`,
                on: {
                    "change": function (e) {
                        const taxg = findArray("ID", this.value, e.data.TaxGroups);
                        if (!!taxg) {
                            updatedetail(re_details, e.data.UomID, e.key, "TaxRate", taxg.Rate);

                        } else {
                            updatedetail(re_details, e.data.UomID, e.key, "TaxRate", 0);
                        }
                        updatedetail(re_details, e.data.UomID, e.key, "TaxGroupID", parseInt(this.value))
                        totalRow($listItemChoosed, e, this);
                        setSummary(re_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "UoMs",
                template: `<select class='form-control font-size uom'></select>`,
                on: {
                    "change": function (e) {
                        updatedetail(re_details, e.data.UomID, e.key, "UomID", parseInt(this.value));
                        const uomList = findArray("UoMID", this.value, e.data.UomPriceLists);
                        const uom = findArray("ID", this.value, e.data.UoMsList);
                        if (!!uom && !!uomList) {
                            updatedetail(re_details, e.data.UomID, e.key, "UnitPrice", uomList.UnitPrice);
                            updatedetail(re_details, e.data.UomID, e.key, "UomName", uom.Name);
                            updatedetail(re_details, e.data.UomID, e.key, "Factor", uom.Factor);
                            $listItemChoosed.updateColumn(e.key, "UnitPrice", num.formatSpecial(uomList.UnitPrice, disSetting.Prices));
                            calDisvalue(e);
                            totalRow($listItemChoosed, e, this);
                            setSummary(re_details);
                            disInputUpdate(master[0]);
                        }
                    }
                }
            },
            {
                name: "Remarks",
                template: `<input class="form-control font-size remark">`,
                on: {
                    "keyup": function (e) {
                        updatedetail(re_details, e.data.UomID, e.key, "Remarks", this.value);
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
                template: `<i class="fas fa-trash font-size"></i>`,
                on: {
                    "click": function (e) {
                        $listItemChoosed.removeRow(e.key);
                        re_details = re_details.filter(i => i.LineID !== e.key);
                        setSummary(re_details);
                        disInputUpdate(master[0]);
                        //$("#source-copy").val("");
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
            url: "/Sale/GetSaleCur",
            type: "Get",
            dataType: "Json",
            data: { PriLi: _priceList },
            success: function (res) {
                $.get("/Sale/GetDisplayFormatCurrency", { curId: res.CurrencyID }, function (resp) {
                    disSetting = resp.Display;
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
                    re_details = [];
                    $listItemChoosed.clearRows();
                    $listItemChoosed.bindRows(re_details);
                    setSummary(re_details);
                    disInputUpdate(master[0]);
                })
            }
        });
    })

    /// applied-amount ///
    $("#applied-amount").keyup(function () {
        $(this).asNumber();
        if (this.value < 0) this.value = 0;
        if (this.value > num.toNumberSpecial(master[0].TotalAmount)) {
            this.value = num.toNumberSpecial(master[0].TotalAmount);
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
        //     const totalAfDis = subtotal - parseFloat(disvalue) + master[0].VatValue;
        //     $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        //     $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
        //     master[0].TotalAmount = totalAfDis;
        //     master[0].DisValue = disvalue;
        //     master[0].DisRate = this.value;
        //     master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
        // } else {
        //     setSummary(re_details);
        // }
    });
    /// dis value on invoice //
    $("#dis-value-id").keyup(function () {
        $(this).asNumber();
        const subtotal = num.toNumberSpecial($("#sub-id").val());
        const vat = num.toNumberSpecial($("#vat-value").val());
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > subtotal) this.value = subtotal;

        const disrate = (num.toNumberSpecial(this.value) * 100 === 0) || (subtotal === 0) ? 0 :
            (num.toNumberSpecial(this.value) * 100) / subtotal;
        $("#dis-rate-id").val(disrate);
        invoiceDisAllRowValue(disrate);
        // if (parseFloat(disrate) > 0) {
        //     const totalAfDis = subtotal - num.toNumberSpecial(this.value) + master[0].VatValue;
        //     $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
        //     $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        //     master[0].TotalAmount = totalAfDis;
        //     master[0].DisValue = this.value;
        //     master[0].DisRate = disrate;
        //     master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
        // } else {
        //     setSummary(re_details);
        // }
    });
    //// Choose Item by BarCode ////
    $(window).on("keypress", function (e) {
        if (e.which === 13) {
            if (document.activeElement === this.document.getElementById("barcode-reading")) {
                let activeElem = this.document.activeElement;
                if (master[0].CurID != 0) {
                    $.get("/Sale/GetItemDetails", { PriLi: _priceList, itemId: 0, barCode: activeElem.value.trim(), uomId: 0 }, function (res) {
                        if (res.IsError) {
                            new DialogBox({
                                content: res.Error,
                            });
                        } else {
                            if (isValidArray(re_details)) {
                                res.forEach(i => {
                                    const isExisted = re_details.some(qd => qd.ItemID === i.ItemID);
                                    if (isExisted) {
                                        const item = re_details.filter(_i => _i.BarCode === i.BarCode)[0];
                                        if (!!item) {
                                            const qty = parseFloat(item.Qty) + 1;
                                            const openqty = parseFloat(item.Qty) + 1;
                                            updatedetail(re_details, i.UomID, item.LineID, "Qty", qty);
                                            updatedetail(re_details, i.UomID, item.LineID, "OpenQty", openqty);
                                            $listItemChoosed.updateColumn(item.LineID, "Qty", qty);
                                            const _e = { key: item.LineID, data: item };
                                            calDisvalue(_e);
                                            totalRow($listItemChoosed, _e, qty);
                                            //setSummary(re_details);
                                        }
                                    } else {
                                        re_details.push(i);
                                        $listItemChoosed.addRow(i);
                                    }
                                })
                            } else {
                                $listItemChoosed.clearHeaderDynamic(res[0].AddictionProps)
                                $listItemChoosed.createHeaderDynamic(res[0].AddictionProps)
                                res.forEach(i => {
                                    re_details.push(i);
                                    $listItemChoosed.addRow(i);
                                })

                            }
                            setItemToAbled([...itemArr, ".qty"]);
                            setSummary(re_details);
                            disInputUpdate(master[0]);
                            activeElem.value = "";
                        }
                    });
                }

            }
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
            if (freightMaster.IsEditabled) {
                freightsEditable(freightDialog);
            } else {
                freightsNoneEditable(freightDialog);
            }
        });
        freightDialog.confirm(function () {
            $("#freight-value").val(num.formatSpecial(freightMaster.AmountReven, disSetting.Prices));
            setSummary(re_details);
            freightDialog.shutdown();
        });
    });
    getFreight(function (res) {
        freightMaster = res;
        freightMaster.IsEditabled = true;
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
        $.get("/Sale/GetFreights", success);
    }
    // TDP Dialog //
    $("#tdp-dailog").click(function () {
        const ARDownPaymentsDialog = new DialogBox({
            content: {
                selector: "#container-list-tdp"
            },
            caption: "A/R Down Payments",
        });
        ARDownPaymentsDialog.invoke(function () {
            const ARDownPaymentsView = ViewTable({
                keyField: "ARDID",
                selector: ARDownPaymentsDialog.content.find(".tdp-lists"),
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: false
                },
                visibleFields: ["Docnumber", "DocType", "Remarks", "Amount", "DocDate", "Selected", "Currency"],
                columns: [
                    {
                        name: "Selected",
                        template: "<input type='checkbox'>",
                        on: {
                            "click": function (e) {
                                const _this = this;
                                if (e.data.CurrencyID !== master[0].SaleCurrencyID) {
                                    const done = new DialogBox({
                                        content: `Currency name "${_currency}" does not match with currency name "${e.data.Currency}"`,
                                        animation: {
                                            startup: {
                                                name: "slide-left",
                                                duration: 500,
                                            }
                                        }
                                    });
                                    done.confirm(function () {
                                        $(_this).prop("checked", false);
                                    })
                                } else {
                                    const isSelected = $(this).prop("checked") ? true : false;
                                    updateARDP(ARDownPayments, "ARDID", e.key, "Selected", isSelected);
                                }
                            }
                        }
                    },
                    {
                        name: "Docnumber",
                        on: {
                            "dblclick": function (e) {
                                ARDPDdialog(e.data.SaleARDPINCNDetails);
                            }
                        }
                    }
                ]
            });
            ARDownPaymentsView.bindRows(ARDownPayments);
        });
        ARDownPaymentsDialog.confirm(function () {
            //setSummary(re_details);
            sumARDP(ARDownPayments);
            $("#total-dp-value").val(num.formatSpecial(__ardpSum.sum, disSetting.Prices));
            master[0].DownPayment = num.formatSpecial(__ardpSum.sum, disSetting.Prices);
            ARDownPaymentsDialog.shutdown();
        });
    });
    //// Find Sale AR ////
    $("#btn-find").on("click", function () {
        $("#btn-addnew").prop("hidden", false);
        $("#btn-find").prop("hidden", true);
        $("#next_number").val("").prop("readonly", false).focus();
        $("#copy-from").prop("disabled", true);
    });
    $("#next_number").on("keypress", function (e) {
        if (e.which === 13 && !!$("#invoice-no").val().trim()) {
            setDisabled();
            $("#submit-item").prop("disabled", true);
            $("#copy-from").prop("disabled", true);
            if ($("#next_number").val() == "*") {
                initModalDialog("/Sale/GetARReserveInvoiceDisplay", "/Sale/FindARReserveInvoice", "Fine AR Reserve Invoice", "ID", "ARReserveInvoice", "ARReserveInvoiceDetails", 369);
            }
            else {
                $.ajax({
                    url: "/Sale/FindARReserveInvoice",
                    data: { number: $("#next_number").val(), seriesID: parseInt($("#invoice-no").val()) },
                    success: function (result) {
                        getSaleItemMasters(result);
                    }
                });
            }
        }
    });

    //// choose item
    $("#item-id").click(function () {
        $("#loadingitem").prop("hidden", false);
        const itemMasterDataDialog = new DialogBox({
            content: {
                selector: "#item-master-content"
            },
            caption: "Item master data",
        });
        itemMasterDataDialog.invoke(function () {
            //// Bind Item Master ////
            const itemMaster = ViewTable({
                keyField: "ID",
                selector: $("#list-item"),
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                dynamicCol: {
                    afterAction: true,
                    headerContainer: "#col-to-append-after",
                },
                visibleFields: ["Code", "KhmerName", "Currency", "UnitPrice", "BarCode", "UoM", "InStock"],
                columns: [
                    {
                        name: "UnitPrice",
                        dataType: "number",
                    },
                    {
                        name: "InStock",
                        dataType: "number",
                    },
                    {
                        name: "AddictionProps",
                        valueDynamicProp: "ValueName",
                        dynamicCol: true,
                    }
                ],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                $.get("/Sale/GetItemDetails", { PriLi: e.data.PricListID, itemId: e.data.ItemID, barCode: "", uomId: e.data.UomID }, function (res) {
                                    re_details.push(res[0]);
                                    $listItemChoosed.addRow(res[0]);
                                    setItemToAbled(itemArr);
                                    const ee = {
                                        key: res[0].LineID,
                                        data: res[0]
                                    }

                                    totalRow($listItemChoosed, ee, res[0].Qty);
                                    setSummary(re_details);
                                    disInputUpdate(master[0]);
                                });
                            }
                        }
                    }
                ]
            });
            if (master.CurID != 0) {
                $.ajax({
                    url: "/Sale/GetItem",
                    type: "Get",
                    dataType: "Json",
                    data: { PriLi: _priceList, wareId: $("#ware-id").val() },
                    success: function (res) {
                        if (res.length > 0) {
                            itemMaster.clearHeaderDynamic(res[0].AddictionProps);
                            itemMaster.createHeaderDynamic(res[0].AddictionProps);
                            $listItemChoosed.clearHeaderDynamic(res[0].AddictionProps);
                            $listItemChoosed.createHeaderDynamic(res[0].AddictionProps);
                            itemMaster.bindRows(res);
                            $("#loadingitem").prop("hidden", true);
                            $("#find-item").on("keyup", function (e) {
                                let __value = this.value.toLowerCase().replace(/\s+/, "");
                                let items = $.grep(res, function (item) {
                                    return item.Barcode === __value || item.KhmerName.toLowerCase().replace(/\s+/, "").includes(__value)
                                        || item.UnitPrice == __value || item.Code.toLowerCase().replace(/\s+/, "").includes(__value)
                                        || item.UoM.toLowerCase().replace(/\s+/, "").includes(__value);
                                });
                                itemMaster.bindRows(items);
                            });
                        }
                        setTimeout(() => {
                            $("#find-item").focus();
                        }, 300);
                    }
                });
            }
        });
        itemMasterDataDialog.confirm(function () {
            itemMasterDataDialog.shutdown();
        })
    });
    //Copy from quotation
    $("#copy-from").change(function () {
        switch (this.value) {
            case "1":
                initModalDialog("/Sale/GetSaleQuotes", "/Sale/GetSaleQuoteDetailCopy", "Sale Quotes (Copy)", "SQID", "SaleQuote", "SaleQuoteDetails", 1);
                type = "SQ";
                break;
            case "2":
                initModalDialog("/Sale/GetSaleOrders", "/Sale/GetSaleOrderDetailCopy", "Sale Orders (Copy)", "SOID", "SaleOrder", "SaleOrderDetails", 2);
                type = "SO";
                break;


        }
        $(this).val(0);
    });
    // find draft
    $("#btn-finddraft").click(function () {
        DraftDataDialog("/Sale/DisplayDraftReserveInvoice", "/Sale/FindDraftReserveInvoice", "Draft Detail", "LineID", "SaleDraft", "DraftARDetail");
    });
    //====show employee===
    $("#show-list-empre").click(function () {
        chooseEmpReqShipRec(true)
    });

    $("#show-list-empship").click(function () {
        chooseEmpReqShipRec(false, true)
    });
    $("#show-list-emprece").click(function () {
        chooseEmpReqShipRec(false, false, true)
    });
    function chooseEmpReqShipRec(isReq, isShip, isRec) {
        const cusDialog = new DialogBox({
            content: {
                selector: "#empre-content"
            },
            caption: "Employee",
        });
        cusDialog.invoke(function () {
            const employes = ViewTable({
                keyField: "ID",
                selector: "#list-empre",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "Phone", "Email", "Address", "Positon"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                if (isReq) {
                                    $("#request_by").val(e.data.ID);
                                    $("#request_name").val(e.data.Name);
                                }
                                if (isShip) {
                                    $("#shipped_by").val(e.data.ID);
                                    $("#shipped_name").val(e.data.Name);
                                }
                                if (isRec) {
                                    $("#received_by").val(e.data.ID);
                                    $("#received_name").val(e.data.Name);
                                }
                                cusDialog.shutdown();
                            }
                        }
                    }
                ]
            });
            $.ajax({
                url: "/Sale/GetEmp",
                type: "Get",
                dataType: "Json",
                success: function (response) {
                    employes.bindRows(response);
                    $("#find-cus").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let rex = new RegExp(__value, "gi");
                        let __customers = $.grep(response, function (person) {
                            return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s+/, "").match(rex)
                                || person.Phone.toLowerCase().replace(/\s+/, "").match(rex)
                                || person.Type.match(rex)
                        });
                        employes.bindRows(__customers);
                    });
                }
            });

        })
        cusDialog.confirm(function () {
            cusDialog.shutdown()
        })
    }
    var draftid = 0;
    /// submit data ///
    $("#submit-item").on("click", function (e) {
        SubmitData(1)
    });
    $("#submit-draft").on("click", function (e) {
        SubmitData(2)
    });
    function SubmitData(savetype) {
        const item_master = master[0];
        freightMaster.FreightSaleDetails = freights.length === 0 ? new Array() : freights;
        item_master.WarehouseID = parseInt($("#ware-id").val());
        item_master.RefNo = $("#ref-id").val();

        item_master.Types = "AR";
        item_master.RequestedBy = $("#request_by").val();
        item_master.ShippedBy = $("#shipped_by").val();
        item_master.ReceivedBy = $("#received_by").val();
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.PostingDate = $("#post-date").val();
        item_master.SaleEmID = $("#sale-emid").val();
        item_master.DueDate = $("#due-date").val();
        item_master.DeliveryDate = $("#due-date").val();
        item_master.DocumentDate = $("#document-date").val();
        item_master.Remarks = $("#remark-id").val();
        item_master.ARReserveInvoiceDetails = re_details.length === 0 ? new Array() : re_details;
        item_master.FreightSalesView = freightMaster;
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.SeriesDID = parseInt($("#SeriesDetailID").val());
        item_master.SeriesID = parseInt($("#invoice-no").val());
        item_master.DocTypeID = parseInt($("#DocumentTypeID").val());
        item_master.InvoiceNumber = parseInt($("#next_number").val());
        item_master.AppliedAmount = parseInt($("#applied-amount").val());
        item_master.FreightAmount = $("#freight-value").val();
        item_master.DownPayment = num.toNumberSpecial(__ardpSum.sum);
        item_master.DownPaymentSys = num.toNumberSpecial(__ardpSum.sum) * num.toNumberSpecial(item_master.ExchangeRate);
        item_master.BranchID = parseInt($("#branch").val());
        const _this = this;
        $(_this).prop("disabled", true);
        //----------------------Noted----------------
        //savetype == 1 => data must save to ARReserveInvoice
        //savetype != 1 => data must save to DraftReserveInvoice
        //------------------------------------------------

        if (savetype == 1) {
            var dialogSubmit = new DialogBox({
                content: "Are you sure you want to save the item?",
                type: "yes-no",
                icon: "warning"
            });
            dialogSubmit.confirm(function () {
                $("#loading").prop("hidden", false);
                if (draftid != 0)
                    item_master.ID = 0;
                $.ajax({
                    url: "/Sale/CreateARReserveInvoice",
                    type: "post",
                    data: $.antiForgeryToken({
                        data: JSON.stringify(item_master),
                        seriesJE: JSON.stringify(__singleElementJE),
                        ardownpayment: JSON.stringify(ARDownPayments),
                        _type: type,
                        serial: JSON.stringify(serials),
                        batch: JSON.stringify(batches),
                    }),
                    success: function (model) {
                        if (model.IsSerail) {
                            $("#loading").prop("hidden", true);
                            $(_this).prop("disabled", false);
                            const serial = SerialTemplate({
                                data: {
                                    serials: model.Data,
                                }
                            });
                            serial.serialTemplate();
                            const seba = serial.callbackInfo();
                            serials = seba.serials;
                        } if (model.IsBatch) {
                            $("#loading").prop("hidden", true);
                            $(_this).prop("disabled", false);
                            const batch = BatchNoTemplate({
                                data: {
                                    batches: model.Data,
                                }
                            });
                            batch.batchTemplate();
                            const seba = batch.callbackInfo();
                            batches = seba.batches;
                        } if (model.Model.IsApproved) {
                            $.get("/Sale/DeleteDraftReserveInvoice", { id: draftid }, function () { })
                            new ViewMessage({
                                summary: {
                                    selector: "#error-summary"
                                }
                            }, model.Model).refresh(1500);
                            $(_this).prop("disabled", false);
                            $("#loading").prop("hidden", true);
                        }
                        new ViewMessage({
                            summary: {
                                selector: "#error-summary"
                            }
                        }, model.Model);
                        $(window).scrollTop(0);
                        $(_this).prop("disabled", false);
                        $("#loading").prop("hidden", true);
                    }
                });
                this.meta.shutdown();
            });
            dialogSubmit.reject(function () {
                $(_this).prop("disabled", false);
            });
        }
        else {
            var dialogsave = new DialogBox({
                caption: "Save Draft",
                content: {
                    selector: "#form-Draft-name",
                },
                button: {
                    ok: {
                        text: "Save"
                    }
                },
                type: "ok-cancel"

            });
            dialogsave.reject(function () {
                dialogsave.shutdown();
            })
            dialogsave.confirm(function () {
                item_master.Name = $("#draftname").val();
                item_master.DraftReserveInvoiceDetails = re_details.length === 0 ? new Array() : re_details;
                $("#loading").prop("hidden", false);

                $.ajax({
                    url: "/Sale/SaveDraftReserveInvoice",
                    type: "POST",
                    dataType: "JSON",
                    data: {
                        draftAR: JSON.stringify(item_master),
                    },
                    success: function (model) {
                        if (model.Action != -1) {

                            new ViewMessage({
                                summary: {
                                    selector: "#error-summary"
                                }
                            }, model).refresh(1500);
                            $("#loading").prop("hidden", true);
                        }
                        new ViewMessage({
                            summary: {
                                selector: "#error-summary"
                            }
                        }, model);
                        $(window).scrollTop(0);
                        $("#loading").prop("hidden", true);
                    }
                });
                this.meta.shutdown();
            });
        }

    }

    $("#cancel-item").click(function () {
        const item_master = master[0];
        const saleArId = item_master.ID;
        const _this = this;
        $(_this).prop("disabled", true);
        $.get("/Sale/GetSaleARByInvoiceNo", { invoiceNo: item_master.InvoiceNumber, seriesID: item_master.SeriesID }, function (data) {
            $(_this).prop("disabled", false);
            if (data.AppliedAmount > 0) {
                var dialogSubmit = new DialogBox({
                    content: "The remaining applied amount is " + data.AppliedAmount
                        + ". Do you want clear and cancel this A/R Invoice?",
                    type: "ok-cancel",
                    icon: "warning"
                });
                dialogSubmit.confirm(function () {
                    cancelAr(item_master, saleArId)
                    dialogSubmit.shutdown();
                });
            } else {
                var dialogSubmit = new DialogBox({
                    content: "Are you sure you want to cancel this A/R Invoice?",
                    type: "ok-cancel",
                    icon: "warning"
                });
                dialogSubmit.confirm(function () {
                    cancelAr(item_master, saleArId)
                    dialogSubmit.shutdown();
                });
            }
        });


    })
    $("#sale-em").click(function () {
        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Cancel",
                },
            },
            // type: "ok-cancel",
            caption: "List Sale Employee",
            content: {
                selector: "#em-content",
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown()
        });
        //  dialogprojmana.shutdown();

        dialogprojmana.invoke(function () {
            const $list_sem = ViewTable({
                keyField: "ID",//id unit not in dababase
                selector: "#list_saleem",// id of table
                indexed: true,
                paging: {
                    pageSize: 15,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "GenderDisplay", "Position", "Address", "Phone", "Email", "EMType"],

                actions: [
                    {
                        name: "",
                        template: `<i class="fas fa-arrow-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                $("#sale-emid").val(e.data.ID);
                                $("#sale-em").val(e.data.Name);
                                dialogprojmana.shutdown();

                            }
                        }
                    },
                ],
            })
            $.get("/Sale/GetSaleEmployee", function (res) {
                $list_sem.bindRows(res);
            });
        });
        dialogprojmana.reject(function () {
            dialogprojmana.shutdown();
        });
    })
    // dialog diplay draft
    function DraftDataDialog(urlMaster, urlDetail, caption, key, keyMaster, keyDetail) {
        $.get(urlMaster, { cusId: master[0].CusID }, function (res) {
            res.forEach(i => {
                i.PostingDate = i.PostingDate.toString().split("T")[0];
                i.SubTotal = num.formatSpecial(i.SubTotal, disSetting.Prices);
                i.TotalAmount = num.formatSpecial(i.TotalAmount, disSetting.Prices);
            })
            let dialog = new DialogBox({
                content: {
                    selector: "#Draft-list"
                },
                button: {
                    ok: {
                        text: "Close"
                    }
                },
                caption: caption,
            });
            dialog.invoke(function () {
                const itemMaster = ViewTable({
                    keyField: key,
                    selector: $("#Draft-Data"),
                    indexed: true,
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    visibleFields: ["DocType", "DraftName", "PostingDate", "CurrencyName", "SubTotal", "BalanceDue", "Remarks"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
                            on: {
                                "click": function (e) {

                                    $.get(urlDetail, { draftname: e.data.DraftName, draftId: e.data.DraftID }, function (res) {

                                        draftid = res.SaleDraft.DraftID;
                                        res.ARReserveInvoice = res.SaleDraft;
                                        res.ARReserveInvoice.ID = res.ARReserveInvoice.DraftID;
                                        res.ARReserveInvoice.RequestedByID = res.ARReserveInvoice.RequestedBy;
                                        res.ARReserveInvoice.ReceivedByID = res.ARReserveInvoice.ReceivedBy;
                                        res.ARReserveInvoice.ShippedByID = res.ARReserveInvoice.ShippedBy;

                                        res.ARReserveInvoiceDetails = res.DraftARDetail;
                                        $("#draftname").val(res.ARReserveInvoice.Name);
                                        if (res.ARReserveInvoiceDetails.length > 0) {
                                            res.ARReserveInvoiceDetails.forEach(i => {
                                                i.ID = i.DraftDetailID;
                                            })
                                            getSaleItemMasters(res, false);
                                        }

                                    });
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                if (res.length > 0) {
                    itemMaster.bindRows(res);
                    $("#txtSearchdraft").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s/g, "");
                        let items = $.grep(res, function (item) {
                            return item.DraftName.toLowerCase().replace(/\s/g, "").includes(__value) || item.SubTotal.toString().toLowerCase().replace(/\s/g, "").includes(__value)
                                || item.PostingDate.toLowerCase().replace(/\s/g, "").includes(__value) || item.BalanceDue.toString().toLowerCase().replace(/\s/g, "").includes(__value)
                                || item.CurrencyName.toLowerCase().replace(/\s/g, "").includes(__value);
                        });
                        itemMaster.bindRows(items);
                    });
                }
            });
            dialog.confirm(function () {
                dialog.shutdown();
            });
        });
    }
    function cancelAr(item_master, saleArId) {
        item_master.SeriesID = $("#invoice-no").val();
        item_master.ID = 0;
        item_master.ARReserveInvoiceDetails = re_details;
        item_master.FreightSalesView.FreightSaleDetails = item_master.FreightSalesView.FreightSaleDetailViewModels;
        getARDownPayments(item_master.CusID, "used", item_master.SARID, function (res) {
            if (isValidArray(res)) {
                item_master.SaleARDPINCNs = res ?? new Array();
            } else {
                item_master.SaleARDPINCNs = new Array();
            }
            $.post('/Sale/CancelSaleAR', { saleAr: JSON.stringify(item_master), saleArId }, function (res) {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, res)
                if (res.IsApproved) {
                    setTimeout(function () { location.reload() }, 1500);
                }
            });
        });
    }
    function getARDownPayments(curId, status, arid, success) {
        $.get("/Sale/ARDownPaymentINCN", { curId, status, arid }, success);
    }
    function freightsEditable(freightDialog) {
        const freightView = ViewTable({
            keyField: "FreightID",
            selector: freightDialog.content.find(".freight-lists"),
            indexed: true,
            paging: {
                pageSize: 20,
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
                            const taxg = findArray("ID", this.value, e.data.TaxGroups);
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
        freightDialog.content.find(".freightSumAmount").val(num.formatSpecial(freightMaster.AmountReven, disSetting.Prices));
        freightView.bindRows(freights);
    }
    function freightsNoneEditable(freightDialog) {
        const freightView = ViewTable({
            keyField: "FreightID",
            selector: freightDialog.content.find(".freight-lists"),
            indexed: true,
            paging: {
                pageSize: 20,
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
        freightDialog.content.find(".freightSumAmount").val(num.formatSpecial(freightMaster.AmountReven, disSetting.Prices));
        freightView.bindRows(freights);
    }
    function initModalDialog(urlMaster, urlDetail, caption, key, keyMaster, keyDetail, type) {
        $.get(urlMaster, { cusId: master[0].CusID }, function (res) {
            res.forEach(i => {
                i.PostingDate = i.PostingDate.toString().split("T")[0];
                i.SubTotal = num.formatSpecial(i.SubTotal, disSetting.Prices);
                i.TotalAmount = num.formatSpecial(i.TotalAmount, disSetting.Prices);
            })
            let dialog = new DialogBox({
                content: {
                    selector: "#container-list-item-copy"
                },
                button: {
                    ok: {
                        text: "Close"
                    }
                },
                caption: caption,
            });
            dialog.invoke(function () {
                const itemMasterCopy = ViewTable({
                    keyField: key,
                    selector: $(".item-copy"),
                    indexed: true,
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    visibleFields: ["Code", "InvoiceNumber", "PostingDate", "Currency", "SubTotal", "TypeDis", "TotalAmount", "Remarks"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                            on: {
                                "click": function (e) {
                                    $("#sale-emid").val(res[0].SaleEmID);
                                    $("#sale-em").val(res[0].SaleEmName);
                                    $.get(urlDetail, { number: e.data.InvoiceNo, seriesId: e.data.SeriesID }, function (res) {
                                        if (type == 369) {
                                            getSaleItemMasters(res);
                                        }
                                        else {
                                            bindItemCopy(res, keyMaster, keyDetail, key, type);
                                        }
                                        dialog.shutdown();
                                    });
                                }
                            }
                        }
                    ]
                });
                if (res.length > 0) {
                    itemMasterCopy.bindRows(res);
                    $("#txtSearch-item-copy").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(res, function (item) {
                            return item.InvoiceNumber.toLowerCase().replace(/\s+/, "").includes(__value) || item.SubTotal.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.PostingDate.toLowerCase().replace(/\s+/, "").includes(__value) || item.TotalAmount.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.Currency.toLowerCase().replace(/\s+/, "").includes(__value) || item.TypeDis.toLowerCase().replace(/\s+/, "").includes(__value);
                        });
                        itemMasterCopy.bindRows(items);
                    });
                }
            });
            dialog.confirm(function () {
                dialog.shutdown();
            });
            //itemMasterCopy.bindRows(res)
        });
    }
    function initModalDialogitem(urlMaster, urlDetail, caption, key) {
        $.get(urlMaster, { cusId: master[0].CusID }, function (res) {
            if (res.length > 0) {
                res.forEach(i => {
                    //$("#customer").html(i.CustomerName);
                    $("#customer").append(`<option>${i.CustomerName}</option>`)
                })
            }
            res.forEach(i => {
                i.PostingDate = i.PostingDate.toString().split("T")[0];
                i.SubTotal = num.formatSpecial(i.SubTotal, disSetting.Prices);
                i.TotalAmount = num.formatSpecial(i.TotalAmount, disSetting.Prices);
            })
            let dialog = new DialogBox({
                content: {
                    selector: "#container-list-item-copy-itemmasterdata"
                },
                button: {
                    ok: {
                        text: "Close"
                    }
                },
                caption: caption,
            });
            dialog.invoke(function () {
                const itemMasterCopy = ViewTable({
                    keyField: key,
                    selector: ".item-copy-masterdata",
                    indexed: true,
                    paging: {
                        pageSize: 10,
                        enabled: true
                    },
                    visibleFields: ["Code", "KhmerName", "EnglishName", "Dateitem", "MonthlyFee"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                            on: {
                                "click": function (e) {
                                    //$("#ware-id").append(`<option selected>${e.data.Wherehouse}</option>`)
                                    ////$("#ware-id").val(e.data.PriceListID);
                                    //$("#ware-id").css("pointer-events", "none").prop("disabled", true);
                                    $("#source-copy").val("/Franchise :");
                                    $("#cur-id").append(`<option selected>${e.data.Wherehouse}</option>`)
                                    $("#cur-id").val(e.data.PriceListID);
                                    $("#cur-id").css("pointer-events", "none").prop("disabled", true);

                                    $.get(urlDetail, { Dateitem: e.data.Dateitem, plid: e.data.PriceListID, itemId: e.data.ID, frid: e.data.FranchiseID }, function (res) {
                                        re_details.push(res[0]);
                                        $listItemChoosed.addRow(res[0]);
                                        setItemToAbled(itemArr);
                                        setSummary(re_details);
                                        disInputUpdate(master[0]);
                                    });
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                if (res.length > 0) {
                    itemMasterCopy.bindRows(res);
                    $("#txtSearch-item-copy").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(res, function (item) {
                            return item.InvoiceNumber.toLowerCase().replace(/\s+/, "").includes(__value) || item.SubTotal.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.PostingDate.toLowerCase().replace(/\s+/, "").includes(__value) || item.TotalAmount.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.Currency.toLowerCase().replace(/\s+/, "").includes(__value) || item.TypeDis.toLowerCase().replace(/\s+/, "").includes(__value);
                        });
                        itemMasterCopy.bindRows(items);
                    });
                }
            });
            dialog.confirm(function () {
                dialog.shutdown();
            });
        });
    }
    //========================
    function bindItemCopy(_master, keyMaster, keyDetail, key, copyType = 0) {
        $.get("/Sale/GetCustomer", { id: _master[keyMaster].CusID }, function (cus) {
            $("#cus-id").val(cus.Name);
        });
        master[0] = _master[keyMaster];
        master[0].BaseOnID = _master[keyMaster][key];
        freights = _master[keyMaster].FreightSalesView.FreightSaleDetailViewModels;
        freightMaster = _master[keyMaster].FreightSalesView;
        freightMaster.ID = 0;
        freightMaster.IsEditabled = true;
        freights.forEach(i => {
            i.ID = 0;
            i.FreightSaleID = 0;
            i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
            i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
            i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
            i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
        });
        re_details = _master[keyDetail];
        $("#request_by").val(_master[keyMaster].RequestedByID);
        $("#shipped_by").val(_master[keyMaster].ShippedByID);
        $("#received_by").val(_master[keyMaster].ReceivedByID);
        $("#request_name").val(_master[keyMaster].RequestedByName);
        $("#shipped_name").val(_master[keyMaster].ShippedByName);
        $("#received_name").val(_master[keyMaster].ReceivedByName);

        $("#branch").val(_master[keyMaster].BranchID); 
        $("#ware-id").val(_master[keyMaster].WarehouseID);
        $("#ref-id").val(_master[keyMaster].RefNo);
        $("#cur-id").val(_master[keyMaster].SaleCurrencyID);
        $("#sta-id").val(_master[keyMaster].Status);
        $("#sub-id").val(num.formatSpecial(_master[keyMaster].SubTotal, disSetting.Prices));
        $("#sub-dis-id").val(_master[keyMaster].TypeDis);
        $("#sub-after-dis").val(num.formatSpecial(_master[keyMaster].SubTotalAfterDis, disSetting.Prices));
        $("#freight-value").val(num.formatSpecial(_master[keyMaster].FreightAmount, disSetting.Prices));
        $("#seriesID").val(_master[keyMaster].SeriesID);
        //$("#sub-dis-id").val(_master.SaleQuote.TypeDis);
        $("#dis-rate-id").val(num.formatSpecial(_master[keyMaster].DisRate, disSetting.Rates));
        $("#dis-value-id").val(num.formatSpecial(_master[keyMaster].DisValue, disSetting.Prices));
        $("#remark-id").val(_master[keyMaster].Remarks);
        $("#total-id").val(num.formatSpecial(_master[keyMaster].TotalAmount, disSetting.Prices));
        $("#vat-value").val(num.formatSpecial(_master[keyMaster].VatValue, disSetting.Prices));
        setDate("#post-date", _master[keyMaster].PostingDate.toString().split("T")[0]);
        setDate("#due-date", _master[keyMaster].DeliveryDate.toString().split("T")[0]);
        setDate("#document-date", _master[keyMaster].DocumentDate.toString().split("T")[0]);
        $("#show-list-cus").addClass("noneEvent");
        var ex = findArray("CurID", _master[keyMaster].SaleCurrencyID, ExChange[0]);
        if (!!ex) {
            $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
            $(".cur-class").text(ex.CurName);
        }
        master[0].CopyType = copyType; // 1: Quotation, 2: Order, 3: Delivery, 4: AR  
        master[0].CopyKey = _master[keyMaster].InvoiceNo;
        master[0].BasedCopyKeys = "/" + _master[keyMaster].InvoiceNo;
        setSourceCopy(master[0].BasedCopyKeys)
        if (isValidArray(_master[keyDetail])) {
            $listItemChoosed.clearHeaderDynamic(_master[keyDetail][0].AddictionProps);
            $listItemChoosed.createHeaderDynamic(_master[keyDetail][0].AddictionProps);
            $listItemChoosed.bindRows(_master[keyDetail]);
        }
        if (copyType == 3) {
            setDisabled();
            setItemToDisabled(itemArr);
            isCopied = !isCopied;
        } else {
            isCopied = false;
            setItemToAbled(itemArr);
        }
        $("#tdp-dailog").removeClass("disabled");
        getARDownPayments(_master[keyMaster].CusID, "close", 0, function (res) {
            if (isValidArray(res)) {
                if (isValidArray(ARDownPayments)) {
                    ARDownPayments = [];
                    res.forEach(i => {
                        i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                        ARDownPayments.push(i);
                    });
                }
                else {
                    res.forEach(i => {
                        i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                        ARDownPayments.push(i);
                    });
                }
            }
        });
    }
    function setSourceCopy(basedCopyKeys) {
        let copyKeys = new Array();
        if (basedCopyKeys) {
            copyKeys = basedCopyKeys.split("/");
        }
        var copyInfo = "";
        $.each(copyKeys, function (i, key) {
            if (key.startsWith("SQ")) {
                copyInfo += "Sale Quotation: " + key;
            }

            if (key.startsWith("SO")) {
                copyInfo += "/Sale Order: " + key;
            }

            if (key.startsWith("DN")) {
                copyInfo += "/Sale Delivery: " + key;
            }
        });
        $("#source-copy").val(basedCopyKeys);
    }
    function getSaleItemMasters(_master, disabletable = true) {
        if (!!_master) {
            $.get("/Sale/GetCustomer", { id: _master.ARReserveInvoice.CusID }, function (cus) {
                $("#cus-id").val(cus.Name);
            });
            master[0] = _master.ARReserveInvoice;
            freights = _master.ARReserveInvoice.FreightSalesView.FreightSaleDetailViewModels;
            freightMaster = _master.ARReserveInvoice.FreightSalesView;
            if (disabletable)
                freightMaster.IsEditabled = false;
            else
                freightMaster.IsEditabled = true;

            freightMaster.ID = 0;
            freights.forEach(i => {
                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
                i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
                i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
                updatedetailFreight(freights, i.FreightID, "ID", 0);
            });
            re_details = _master.ARReserveInvoiceDetails;
            //$("#request_by").prop("disabled", true);
            //$("#shipped_by").prop("disabled", true);
            //$("#received_by").prop("disabled", true);

            $("#request_name").val(_master.ARReserveInvoice.RequestedByName);
            $("#shipped_name").val(_master.ARReserveInvoice.ShippedByName);
            $("#received_name").val(_master.ARReserveInvoice.ReceivedByName);

            $("#request_by").val(_master.ARReserveInvoice.RequestedByID);
            $("#shipped_by").val(_master.ARReserveInvoice.ShippedByID);
            $("#received_by").val(_master.ARReserveInvoice.ReceivedByID);
            $("#branch").val(_master.ARReserveInvoice.BranchID); 
            $("#next_number").val(_master.ARReserveInvoice.InvoiceNumber);
            $("#sale-emid").val(_master.ARReserveInvoice.SaleEmID);
            $("#sale-em").val(_master.ARReserveInvoice.SaleEmName);
            $("#ware-id").val(_master.ARReserveInvoice.WarehouseID);
            $("#ref-id").val(_master.ARReserveInvoice.RefNo);
            $("#cur-id").val(_master.ARReserveInvoice.SaleCurrencyID);
            $("#sta-id").val(_master.ARReserveInvoice.Status);
            $("#sub-id").val(num.formatSpecial(_master.ARReserveInvoice.SubTotal, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(_master.ARReserveInvoice.SubTotalAfterDis, disSetting.Prices));
            $("#freight-value").val(num.formatSpecial(_master.ARReserveInvoice.FreightAmount, disSetting.Prices));
            $("#total-dp-value").val(num.formatSpecial(_master.ARReserveInvoice.DownPayment, disSetting.Prices));
            //$("#sub-dis-id").val(_master.SaleQuote.TypeDis);
            $("#dis-rate-id").val(num.formatSpecial(_master.ARReserveInvoice.DisRate, disSetting.Rates));
            $("#dis-value-id").val(num.formatSpecial(_master.ARReserveInvoice.DisValue, disSetting.Prices));
            setDate("#post-date", _master.ARReserveInvoice.PostingDate.toString().split("T")[0]);
            setDate("#due-date", _master.ARReserveInvoice.DeliveryDate.toString().split("T")[0]);
            setDate("#document-date", _master.ARReserveInvoice.DocumentDate.toString().split("T")[0]);
            $("#remark-id").val(_master.ARReserveInvoice.Remarks);
            $("#total-id").val(num.formatSpecial(_master.ARReserveInvoice.TotalAmount, disSetting.Prices));
            $("#vat-value").val(num.formatSpecial(_master.ARReserveInvoice.VatValue, disSetting.Prices));
            $("#item-id").prop("disabled", false);
            $("#applied-amount").val(num.formatSpecial(_master.ARReserveInvoice.AppliedAmount, disSetting.Prices))
            var ex = findArray("CurID", _master.ARReserveInvoice.SaleCurrencyID, ExChange[0]);
            if (!!ex) {
                $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
                $(".cur-class").text(ex.CurName);
            }
            if (isValidArray(_master.ARReserveInvoiceDetails)) {
                $listItemChoosed.clearHeaderDynamic(_master.ARReserveInvoiceDetails[0].AddictionProps);
                $listItemChoosed.createHeaderDynamic(_master.ARReserveInvoiceDetails[0].AddictionProps);
                $listItemChoosed.bindRows(_master.ARReserveInvoiceDetails);
            }

            if (disabletable)
                setItemToDisabled([...itemArr, ".qty"]);
            if (master[0].Status != 'cancel') $("#cancel-item").prop("hidden", false);
            //setRequested(_master, key, detailKey, copyType);
        } else {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Sale A/R Not found!",
                close_button: "none"
            });
        }
    }
    function setWarehouse(id) {
        $.ajax({
            url: "/Sale/GetWarehouse",
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
    function setDisabled() {
        $("#item-id").prop("disabled", true);
        $("#sub-dis-id").prop("disabled", true);
        $("#dis-rate-id").prop("disabled", true);
        $("#dis-value-id").prop("disabled", true);
        $("#sub-dis-id").prop("disabled", true);
        $("#sub-id").prop("disabled", true);
        $("#sub-after-dis").prop("disabled", true);
        $("#remark-id").prop("disabled", true);
        $("#include-vat-id").prop("disabled", true);
        $("#applied-amount").prop("disabled", true);
        $("#cur-id").prop("disabled", true);
        $("#ref-id").prop("disabled", true);
        $("#ware-id").prop("disabled", true);
        $("#branch").prop("disabled", true);
        $("#show-list-cus").addClass("csr-default").off();
        $("#fee-note").prop("disabled", true);
        $("#fee-amount").prop("disabled", true);
        $.each($("[data-date]"), function (i, t) {
            $(t).prop("disabled", true);
        });
    }
    function selectSeries(selected) {
        $.each(___IN.seriesIN, function (i, item) {
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
    function totalRow(table, e, _this) {
        if (_this === '' || _this === '-') _this = 0;
        e.data.FinDisRate = num.toNumberSpecial($("#dis-rate-id").val());
        const totalWDis = (num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice)) == 0 ? 0 : (num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice)) - num.toNumberSpecial(e.data.DisValue);
        let vatValue = num.toNumberSpecial(e.data.TaxRate) * totalWDis === 0 ? 0 : num.toNumberSpecial(e.data.TaxRate) * totalWDis / 100;
        let fidis = num.toNumberSpecial(e.data.FinDisRate) == 0 ? 0 : (num.toNumberSpecial(e.data.FinDisRate) / 100) * totalWDis;
        let fitotal = totalWDis == 0 ? 0 : totalWDis - fidis;
        let taxoffinal = num.toNumberSpecial(e.data.TaxRate) == 0 ? 0 : num.toNumberSpecial(e.data.TaxRate) / 100 * fitotal;
        let totalwtax = totalWDis + taxoffinal;
        // Update Object

        updatedetail(re_details, e.data.UomID, e.key, "Total", totalWDis);
        updatedetail(re_details, e.data.UomID, e.key, "TaxRate", e.data.TaxRate);
        updatedetail(re_details, e.data.UomID, e.key, "TaxValue", vatValue);
        updatedetail(re_details, e.data.UomID, e.key, "FinDisRate", e.data.FinDisRate);
        updatedetail(re_details, e.data.UomID, e.key, "FinDisValue", fidis);
        updatedetail(re_details, e.data.UomID, e.key, "TotalWTax", totalwtax);
        updatedetail(re_details, e.data.UomID, e.key, "FinTotalValue", fitotal);
        updatedetail(re_details, e.data.UomID, e.key, "TaxOfFinDisValue", taxoffinal);

        //Update View
        table.updateColumn(e.key, "Total", num.formatSpecial(totalWDis, disSetting.Prices));
        table.updateColumn(e.key, "TaxRate", num.formatSpecial(e.data.TaxRate, disSetting.Prices));
        table.updateColumn(e.key, "TaxValue", num.formatSpecial(vatValue, disSetting.Prices));
        table.updateColumn(e.key, "FinDisRate", num.formatSpecial(e.data.FinDisRate, disSetting.Prices));
        table.updateColumn(e.key, "FinDisValue", num.formatSpecial(fidis, disSetting.Prices));
        table.updateColumn(e.key, "TotalWTax", num.formatSpecial(totalwtax, disSetting.Prices));
        table.updateColumn(e.key, "FinTotalValue", num.formatSpecial(fitotal, disSetting.Prices));
        table.updateColumn(e.key, "TaxOfFinDisValue", num.formatSpecial(taxoffinal, disSetting.Prices));

    }
    function calDisRate(e, _this) {
        if (_this.value > 100) _this.value = 100;
        if (_this.value < 0) _this.value = 0;
        updatedetail(re_details, e.data.UomID, e.key, "DisRate", _this.value);
        const disvalue = parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0 ? 0 :
            parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) / 100;

        updatedetail(re_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisValue(e, _this) {
        if (_this.value > e.data.Qty * e.data.UnitPrice) _this.value = e.data.Qty * e.data.UnitPrice;
        if (_this.value == '' || _this.value < 0) _this.value = 0;
        const value = parseFloat(_this.value);
        updatedetail(re_details, e.data.UomID, e.key, "DisValue", value);
        const ratedis = (value * 100 === 0) || (parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0) ? 0 :
            (value * 100) / (parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice));
        updatedetail(re_details, e.data.UomID, e.key, "DisRate", ratedis);
        $listItemChoosed.updateColumn(e.key, "DisRate", num.formatSpecial(isNaN(ratedis) ? 0 : ratedis, disSetting.Rates));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisvalue(e) {
        let disvalue = num.toNumberSpecial(e.data.DisRate) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0 ? 0 :
            (num.toNumberSpecial(e.data.DisRate) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice)) / 100;
        if (disvalue > parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice)) disvalue = parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice);
        updatedetail(re_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
    }
    function setSummary(data) {
        let subtotal = 0;
        let vat = 0;
        let disRate = $("#dis-rate-id").val();
        data.forEach(i => {
            subtotal += num.toNumberSpecial(i.Total);
            vat += num.toNumberSpecial(i.TaxOfFinDisValue);
        });
        const disValue = (num.toNumberSpecial(disRate) * subtotal) === 0 ? 0 : (num.toNumberSpecial(disRate) * subtotal) / 100;
        const _master = master[0];
        _master.SubTotalSys = subtotal * _master.ExchangeRate;
        _master.SubTotal = subtotal;
        _master.VatRate = (vat * 100) === 0 ? 0 : vat * 100 / subtotal;
        _master.VatValue = vat + num.toNumberSpecial(freightMaster.TaxSumValue) - num.toNumberSpecial(__ardpSum.taxSum);
        _master.DisValue = disValue;
        _master.SubTotalAfterDis = subtotal - disValue;
        _master.SubTotalBefDis = subtotal;
        _master.DisRate = disRate;
        _master.TotalAmount = num.toNumberSpecial(_master.SubTotalAfterDis) + _master.VatValue - num.toNumberSpecial(__ardpSum.sum) + num.toNumberSpecial(freightMaster.AmountReven);
        _master.TotalAmountSys = _master.TotalAmount * _master.ExchangeRate;
        // re_details.forEach(i => {
        //     $listItemChoosed.updateColumn(i.LineID, "FinDisValue", _master.DisValue);
        //     updatedetail(re_details, i.UomID, i.LineID, "FinDisValue", _master.DisValue);
        //     const lastDisval = num.toNumberSpecial(i.Total) - (num.toNumberSpecial(_master.DisRate) * num.toNumberSpecial(i.Total) / 100);
        //     const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
        //         num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
        //     updatedetail(re_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
        //     updatedetail(re_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
        //     $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
        //     $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
        // });
        $("#vat-value").val(num.formatSpecial(_master.VatValue, disSetting.Prices));
        $("#sub-id").val(num.formatSpecial(subtotal, disSetting.Prices));
        $("#total-id").val(num.formatSpecial(_master.TotalAmount, disSetting.Prices));
        $("#sub-after-dis").val(num.formatSpecial(_master.SubTotalAfterDis, disSetting.Prices));
    }
    function disInputUpdate(_master) {
        $("#dis-rate-id").val(_master.DisRate);
        $("#dis-value-id").val(_master.DisValue);
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
    function setItemToAbled(item) {
        if (isValidArray(item)) {
            item.forEach(i => {
                $(i).prop("disabled", false);
            });
        }
    }
    function setItemToDisabled(item) {
        if (isValidArray(item)) {
            item.forEach(i => {
                $(i).prop("disabled", true);
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
        if (isValidArray(re_details)) {
            re_details.forEach(i => {
                const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) / 100;
                if (_this.value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updatedetail(re_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(re_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(value);
                    const taxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;

                    updatedetail(re_details, i.UomID, i.LineID, "TaxOfFinDisValue", taxOfFinDisValue);
                    updatedetail(re_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(taxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                } else if (_this.value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updatedetail(re_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(re_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    updatedetail(re_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(re_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(re_details);
        }
    }
    function invoiceDisAllRowValue(value) {
        if (isValidArray(re_details)) {
            re_details.forEach(i => {
                const _value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) / 100;
                if (value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(_value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updatedetail(re_details, i.UomID, i.LineID, "FinDisValue", _value);
                    updatedetail(re_details, i.UomID, i.LineID, "FinDisRate", value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(_value);
                    const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
                    updatedetail(re_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
                    updatedetail(re_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                }
                else if (value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updatedetail(re_details, i.UomID, i.LineID, "FinDisValue", _value);
                    updatedetail(re_details, i.UomID, i.LineID, "FinDisRate", value);

                    updatedetail(re_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(re_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(re_details);
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
            freightMaster.AmountReven = sumFreight;
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
    function updateARDP(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue) {
                    i[prop] = propValue;
                }
            });
        }
    }
    function sumARDP(data) {
        let sum = 0,
            taxSum = 0;
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i.Selected) {
                    sum += num.toNumberSpecial(i.Amount);
                    if (isValidArray(i.SaleARDPINCNDetails)) {
                        i.SaleARDPINCNDetails.forEach(j => {
                            taxSum += num.toNumberSpecial(j.TaxDownPaymentValue);
                        });
                    }
                }
            });
        }
        __ardpSum.sum = sum;
        __ardpSum.taxSum = taxSum;
        setSummary(re_details);
    }
    function ARDPDdialog(data) {
        const ARDownPaymentsdDialog = new DialogBox({
            content: {
                selector: "#container-list-tdpd"
            },
            caption: "A/R Down Payment Details",
        });
        ARDownPaymentsdDialog.invoke(function () {
            const ARDownPaymentsdView = ViewTable({
                keyField: "ID",
                selector: ARDownPaymentsdDialog.content.find(".tdpd-lists"),
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: [
                    "Barcode", "ItemCode", "Remarks", "ItemName", "Quantity", "UnitPrice", "UoMName", "TaxCode",
                    "TaxRate", "TaxValue", "TaxOfFinDisValue", "TaxDownPaymentValue", "DisRate", "DisValue",
                    "FinDisRate", "FinDisValue", "Total", "TotalWTax", "FinTotalValue", "Currency"
                ],
            });
            if (isValidArray(data)) {
                data.forEach(i => {
                    i.Quantity = num.formatSpecial(i.Quantity, disSetting.Amounts);
                    i.UnitPrice = num.formatSpecial(i.UnitPrice, disSetting.Prices);
                    i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
                    i.TaxValue = num.formatSpecial(i.TaxValue, disSetting.Prices);
                    i.TaxOfFinDisValue = num.formatSpecial(i.TaxOfFinDisValue, disSetting.Prices);
                    i.TaxDownPaymentValue = num.formatSpecial(i.TaxDownPaymentValue, disSetting.Prices);
                    i.DisRate = num.formatSpecial(i.DisRate, disSetting.Rates);
                    i.DisValue = num.formatSpecial(i.DisValue, disSetting.Prices);
                    i.FinDisRate = num.formatSpecial(i.FinDisRate, disSetting.Rates);
                    i.FinDisValue = num.formatSpecial(i.FinDisValue, disSetting.Prices);
                    i.Total = num.formatSpecial(i.Total, disSetting.Prices);
                    i.TotalWTax = num.formatSpecial(i.TotalWTax, disSetting.Prices);
                    i.FinTotalValue = num.formatSpecial(i.FinTotalValue, disSetting.Prices);
                });
                ARDownPaymentsdView.bindRows(data);
            }
        });
        ARDownPaymentsdDialog.confirm(function () {
            ARDownPaymentsdDialog.shutdown();
        })
    }
});