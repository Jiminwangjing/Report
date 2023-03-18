"use strict"
let creditMemo_details = [],
    ExChange = [],
    master = [],
    _currency = "",
    _curencyID = 0,
    _priceList = 0,
    ARDownPayments = [],
    freights = [],
    freightMaster = {},
    serials = [],
    batches = [],
    _baseOn = 0,
    _nameCus = "",
    _idCus = 0,
    __ardpSum = {
        sum: 0,
        taxSum: 0,
    },
    type = "CN",
    __singleElementJE,
    iscopied = false,
    ___CN = JSON.parse($("#data-invoice").text()),
    ___branchID = parseInt($("#BranchID").text()),
    disSetting = ___CN.genSetting.Display;
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
    const itemmaster = {
        SCMOID: 0,
        CusID: 0,
        DraftID: 0,
        BranchID: 0,
        WarehouseID: 0,
        UserID: 0,
        SaleCurrencyID: 0,
        RequestedBy: "",
        ShippedBy: "",
        ReceivedBy: "",
        RefNo: "",
        InvoiceNo: "",
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
        BasedCopyKey: "",
        LocalSetRate: 0,
        PriceListID: 0,
        BasedOn: 0,
        SaleCreditMemoDetails: new Array()
    }
    master.push(itemmaster);
    // get warehouse
    setWarehouse(0);
    // Invoice
    ___CN.seriesCN.forEach(i => {
        if (i.Default == true) {
            $("#DocumentTypeID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    ___CN.seriesJE.forEach(i => {
        if (i.Default == true) {
            $("#JEID").val(i.ID);
            $("#JENumber").val(i.NextNo);
            __singleElementJE = findArray("ID", i.ID, ___CN.seriesJE);
        }
    });
    let selected = $("#invoice-no");
    selectSeries(selected);
    $('#invoice-no').change(function () {
        var id = $(this).val();
        var seriesCN = findArray("ID", id, ___CN.seriesCN);
        ___CN.seriesCN.Number = seriesCN.NextNo;
        ___CN.seriesCN.ID = id;
        $("#DocumentID").val(seriesCN.DocumentTypeID);
        $("#number").val(seriesCN.NextNo);
        $("#next_number").val(seriesCN.NextNo);
    });
    if (___CN.seriesCN.length == 0) {
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
    var semid = 0;
    var semnale = "";
    $("#cus-cancel").click(function () {
        $("#sale-emid").val(0);
        $("#sale-em").val("");
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
                                $("#sale-emid").val(semid);
                                $("#sale-em").val(semnale);
                                master[0].CusID = id;
                                $("#item-id").prop("disabled", false);
                                $("#show-list-cus").prop("disabled", false);
                                $("#barcode-reading").prop("disabled", false).focus();
                                $("#copy-from").prop("disabled", false);
                                cusDialog.shutdown();
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
                        let __value = this.value.toLowerCase().replace(/\s/g, "");
                        let rex = new RegExp(__value, "i");
                        let __customers = $.grep(response, function (person) {
                            let phone = isEmpty(person.Phone) ? "" : person.Phone;
                            return person.Code.match(rex) || person.Name.replace(/\s/g, "").match(rex)
                                || phone.replace(/\s/g, "").match(rex)
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
            pageSize: 20,
            enabled: false
        },
        dynamicCol: {
            afterAction: true,
            headerContainer: "#col-to-append-after-detail",
        },
        visibleFields: [
            "ItemCode", "Cost", "UnitPrice", "BarCode", "Qty", "TaxGroupList", "Cost", "TaxRate", "TaxValue", "TotalWTax", "Total", "Currency",
            "UoMs", "ItemNameKH", "DisValue", "DisRate", "Remarks", "FinDisRate", "FinDisValue", "TaxOfFinDisValue", "FinTotalValue"
        ],
        columns: [
            {
                name: "Cost",
                template: `<input class='form-control font-size disabledrowinput' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updatedetail(creditMemo_details, e.data.UomID, e.key, "Cost", this.value);

                    }
                }
            },
            {
                name: "UnitPrice",
                template: `<input class='form-control font-size disabledrowinput' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updatedetail(creditMemo_details, e.data.UomID, e.key, "UnitPrice", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this);
                        setSummary(creditMemo_details);
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
                template: `<input class='form-control font-size disabledrowinput' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        calDisValue(e, this);
                        totalRow($listItemChoosed, e, this);
                        setSummary(creditMemo_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "DisRate",
                template: `<input class='form-control font-size disabledrowinput' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        calDisRate(e, this);
                        totalRow($listItemChoosed, e, this);
                        setSummary(creditMemo_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            //{
            //    name: "Qty",
            //    template: `<input class='form-control font-size disabledrowinput' type='text' />`,
            //    on: {
            //        "keyup": function (e) {
            //            $(this).asNumber();
            //            if (iscopied && this.value > e.data.OpenQty) this.value = e.data.OpenQty;
            //            updatedetail(creditMemo_details, e.data.UomID, e.key, "Qty", this.value);
            //            updatedetail(creditMemo_details, e.data.UomID, e.key, "OpenQty", this.value);
            //            //calDisrate(e);
            //            calDisvalue(e);
            //            totalRow($listItemChoosed, e, this);
            //            setSummary(creditMemo_details);
            //            disInputUpdate(master[0]);
            //        }
            //    }
            //},
            {
                name: "Qty",
                template: `<input class='form-control font-size disabledrowinput' type='text' />`,
                on: {
                    "keyup": function (e) {


                        $(this).asNumber();
                        if (this.value == "") { this.value = "0"; }

                        if (parseFloat(e.data.SaleCopyType) != 0) {
                            if (parseFloat(this.value) >= e.data.OpenQty) {
                                this.value = e.data.OpenQty;
                            }
                            else {
                                updatedetail(creditMemo_details, e.data.UomID, e.key, "Qty", parseFloat(this.value));
                                //updateData(creditmemoDetials, "ItemID", e.data.ItemID, "UomID", e.data.UomID, "Qty", parseFloat(this.value));
                                calDisvalue(e);
                                totalRow($listItemChoosed, e, this);
                                setSummary(creditMemo_details);
                                disInputUpdate(master[0]);

                            }


                        }
                        else {

                            updatedetail(creditMemo_details, e.data.UomID, e.key, "Qty", parseFloat(this.value));
                            //updateData(creditmemoDetials, "ItemID", e.data.ItemID, "UomID", e.data.UomID, "Qty", parseFloat(this.value));
                            calDisvalue(e);
                            totalRow($listItemChoosed, e, this);
                            setSummary(creditMemo_details);
                            disInputUpdate(master[0]);
                        }


                        // if (iscopied && this.value > e.data.OpenQty) this.value = e.data.OpenQty;

                        //updatedetail(creditMemo_details, e.data.UomID, e.key, "OpenQty", this.value);
                        //calDisrate(e);
                        // calDisvalue(e);

                    }
                }
            },
            {
                name: "TaxGroupList",
                template: `<select class='form-control font-size disabledrowinput'></select>`,
                on: {
                    "change": function (e) {
                        //setItemToAbled(itemArr);
                        const taxg = findArray("ID", this.value, e.data.TaxGroups);
                        if (!!taxg) {
                            updatedetail(creditMemo_details, e.data.UomID, e.key, "TaxRate", taxg.Rate);
                        } else {
                            updatedetail(creditMemo_details, e.data.UomID, e.key, "TaxRate", 0);
                        }
                        updatedetail(creditMemo_details, e.data.UomID, e.key, "TaxGroupID", parseInt(this.value))
                        totalRow($listItemChoosed, e, this);
                        setSummary(creditMemo_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "UoMs",
                template: `<select class='form-control font-size disabledrowinput'></select>`,
                on: {
                    "change": function (e) {
                        updatedetail(creditMemo_details, e.data.UomID, e.key, "UomID", parseInt(this.value));
                        const uomList = findArray("UoMID", this.value, e.data.UomPriceLists);
                        const uom = findArray("ID", this.value, e.data.UoMsList);
                        if (!!uom && !!uomList) {
                            updatedetail(creditMemo_details, e.data.UomID, e.key, "UnitPrice", uomList.UnitPrice);
                            updatedetail(creditMemo_details, e.data.UomID, e.key, "UomName", uom.Name);
                            updatedetail(creditMemo_details, e.data.UomID, e.key, "Factor", uom.Factor);
                            $listItemChoosed.updateColumn(e.key, "UnitPrice", num.formatSpecial(uomList.UnitPrice, disSetting.Prices));
                            calDisvalue(e);
                            totalRow($listItemChoosed, e, this);
                            setSummary(creditMemo_details);
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
                        updatedetail(creditMemo_details, e.data.UomID, e.key, "Remarks", this.value);
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
                        creditMemo_details = creditMemo_details.filter(i => i.LineID !== e.key);
                        setSummary(creditMemo_details);
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
                    creditMemo_details = [];
                    $listItemChoosed.clearRows();
                    $listItemChoosed.bindRows(creditMemo_details);
                    setSummary(creditMemo_details);
                    disInputUpdate(master[0]);
                })
            }
        });
    })

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
        //     $("#applied-amount").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        //     $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
        //     master[0].TotalAmount = totalAfDis;
        //     master[0].AppliedAmount = totalAfDis;
        //     master[0].DisValue = disvalue;
        //     master[0].DisRate = this.value;
        //     master[0].TotalAmount_Sys = totalAfDis * master[0].ExchangeRate;
        // } else {
        //     setSummary(creditMemo_details);
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
        $("#dis-rate-id").val(disrate);
        invoiceDisAllRowValue(disrate);
        // if (parseFloat(disrate) > 0) {
        //     const totalAfDis = subtotal - num.toNumberSpecial(this.value) + master[0].VatValue;
        //     $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
        //     $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        //     $("#applied-amount").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        //     master[0].TotalAmount = totalAfDis;
        //     master[0].AppliedAmount = totalAfDis;
        //     master[0].DisValue = this.value;
        //     master[0].DisRate = disrate;
        //     master[0].TotalAmount_Sys = totalAfDis * master[0].ExchangeRate;
        // } else {
        //     setSummary(creditMemo_details);
        // }
    });
    //// Choose Item by BarCode ////
    $(window).on("keypress", function (e) {
        if (e.which === 13) {
            if (document.activeElement === this.document.getElementById("barcode-reading")) {
                let activeElem = this.document.activeElement;
                if (master[0].CurID != 0) {
                    const wareId = $("#ware-id").val();
                    $.get("/Sale/GetItemDetailsForSaleCM",
                        {
                            PriLi: _priceList,
                            itemId: 0,
                            barCode: activeElem.value.trim(),
                            uomId: 0,
                            wareId,
                            process: ""
                        },
                        function (res) {
                            if (res.IsError) {
                                new DialogBox({
                                    content: res.Error,
                                });
                            } else {
                                if (isValidArray(creditMemo_details)) {
                                    //res.forEach(i => {
                                    const isExisted = creditMemo_details.some(qd => qd.ItemID === res.ItemID);
                                    const item = creditMemo_details.filter(_i => _i.BarCode === res.BarCode)[0];
                                    if (isExisted && !!item) {
                                        const qty = parseFloat(item.Qty) + 1;
                                        const openqty = parseFloat(item.Qty) + 1;
                                        updatedetail(creditMemo_details, res.UomID, item.LineID, "Qty", qty);
                                        updatedetail(creditMemo_details, res.UomID, item.LineID, "OpenQty", openqty);
                                        $listItemChoosed.updateColumn(item.LineID, "Qty", qty);
                                        const _e = { key: item.LineID, data: item };
                                        calDisvalue(_e);
                                        totalRow($listItemChoosed, _e, qty);
                                    } else {
                                        creditMemo_details.push(res);
                                        $listItemChoosed.addRow(res);
                                    }
                                    // })
                                } else {
                                    //res.forEach(i => {
                                    //    creditMemo_details.push(i);
                                    //    $listItemChoosed.addRow(i);
                                    //})
                                    creditMemo_details.push(res);
                                    $listItemChoosed.addRow(res);

                                }
                                $listItemChoosed.clearHeaderDynamic(res.AddictionProps)
                                $listItemChoosed.createHeaderDynamic(res.AddictionProps)
                                //setItemToAbled(itemArr);
                                setSummary(creditMemo_details);
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
            setSummary(creditMemo_details);
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
                    pageSize: 20,
                    enabled: false
                },
                visibleFields: ["Docnumber", "DocType", "Remarks", "Amount", "DocDate", "Selected", "Currency"],
                columns: [
                    {
                        name: "UnitPrice",
                        dataType: "number",
                    },
                    {
                        name: "InStock",
                        dataType: "number",
                    }
                ],
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
            //setSummary(creditMemo_details);
            sumARDP(ARDownPayments);
            $("#total-dp-value").val(num.formatSpecial(__ardpSum.sum, disSetting.Prices));
            ARDownPaymentsDialog.shutdown();
        });
    });
    function getARDownPayments(curId, arid, success) {
        $.get("/Sale/ARDownPaymentINCN", { curId, status: "used", arid }, success);
    }
    //// Find Sale Credit ////
    $("#btn-find").on("click", function () {
        $("#btn-addnew").prop("hidden", false);
        $("#btn-find").prop("hidden", true);
        $("#next_number").val("").prop("readonly", false).focus();
        $("#copy-from").prop("disabled", true);
    });
    $("#next_number").on("keypress", function (e) {
        if (e.which === 13 && !!$("#invoice-no").val().trim()) {
            setDisabled();
            if ($("#next_number").val() == "*") {
                initModalDialog("/Sale/GetSaleCrediMemoDisplay", "/Sale/FindSaleCreditMemo", "Fine Sale CreditMemo", "SCMOID", "SaleCreditMemo", "SaleCreditMemoDetail", 369);
            }
            else {
                $.ajax({
                    url: "/Sale/FindSaleCreditMemo",
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

                                const wareId = $("#ware-id").val();
                                $.get("/Sale/GetItemDetailsForSaleCM",
                                    {
                                        PriLi: e.data.PricListID,
                                        itemId: e.data.ItemID,
                                        barCode: "",
                                        uomId: e.data.UomID,
                                        wareId,
                                        process: e.data.Process
                                    },
                                    function (res) {
                                        //if (isValidArray(res)) {
                                        //    res.forEach(i => {
                                        //        creditMemo_details.push(i);
                                        //        $listItemChoosed.addRow(i);
                                        //    })
                                        //}
                                        if (res.IsError) {
                                            new DialogBox({
                                                content: res.Error,
                                            });
                                        } else {
                                            creditMemo_details.push(res);
                                            $listItemChoosed.addRow(res);
                                            //setItemToAbled(itemArr);

                                            const ee = {
                                                key: res.LineID,
                                                data: res
                                            }

                                            totalRow($listItemChoosed, ee, res.Qty);
                                            setSummary(creditMemo_details);
                                            disInputUpdate(master[0]);
                                        }
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
                    data: { PriLi: _priceList, wareId: parseInt($("#ware-id").val()) },
                    success: function (res) {

                        if (res.length > 0) {
                            itemMaster.clearHeaderDynamic(res[0].AddictionProps);
                            itemMaster.createHeaderDynamic(res[0].AddictionProps);
                            $listItemChoosed.clearHeaderDynamic(res[0].AddictionProps);
                            $listItemChoosed.createHeaderDynamic(res[0].AddictionProps);
                            itemMaster.bindRows(res);
                            $("#loadingitem").prop("hidden", true);
                            $("#find-item").on("keyup", function (e) {
                                let __value = this.value.toLowerCase().replace(/\s/g, "");
                                let items = $.grep(res, function (item) {
                                    return item.Barcode === __value || item.KhmerName.toLowerCase().replace(/\s/g, "").includes(__value)
                                        || item.UnitPrice == __value || item.Code.toLowerCase().replace(/\s/g, "").includes(__value)
                                        || item.UoM.toLowerCase().replace(/\s/g, "").includes(__value);
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
    $("#find-draft").click(function () {
        DraftDataDialog("/Sale/DisplayDraftCM", "/Sale/FindDraft", "Draft Detail", "LineID", "SaleDraft", "draftARDetail");
    });
    //Copy from quotation
    $("#copy-from").change(function () {
        switch (this.value) {
            case "4":
                initModalDialog("/Sale/GetSaleARs", "/Sale/GetSaleARsDetailCopy", "A/R (Copy)", "SARID", "SaleAR", "SaleARDetails", 4);
                type = "IN";
                break;
            case "10":
                initModalDialog("/Sale/GetSaleAREdit", "/Sale/GetSaleAREditDetailCopy", "A/R Edit (Copy)", "SARID", "SaleAR", "SaleARDetails", 10);
                type = "IN";
                break;
            case "7":
                initModalDialog("/Sale/GetSaleARDownPM", "/Sale/GetSaleARDownPMDetailCopy", "A/R Down Payment Invoice (Copy)", "ARDID", "ARDownPayment", "SaleARDownDetails", 7);
                type = "CD";
                break;
            case "9":
                initModalDialog("/Sale/GetARReserveInvoice", "/Sale/GetARReserveInvoiceDetailDetailCopy", "AR Reserve Invoice (Copy)", "ID", "ARReserveInvoice", "ARReserveInvoiceDetails", 9);
                type = "AR";
                break;
            case "15":
                initModalDialog("/Sale/GetARReserveInvoiceEDTDisplay", "/Sale/GetARReserveInvoiceEDTDetailCopyToMemo", "AR Reserve Invoice (Copy)", "ID", "ARReserveInvoice", "ARReserveInvoiceDetails", 15);
                type = "ARReserveInvoiceEDT";
                break;
        }
        $(this).val(0);
    });
    $("#save-draft").click(function () {
        const item_master = master[0];
        item_master.SaleEmID = $("#sale-emid").val();
        item_master.RequestedBy = $("#request_by").val();
        item_master.ReceivedBy = $("#received_by").val();
        item_master.ShippedBy = $("#shipped_by").val();
        item_master.SaleEmID = $("#sale-emid").val();
        item_master.WarehouseID = parseInt($("#ware-id").val());
        item_master.RefNo = $("#ref-id").val();
        item_master.PostingDate = $("#post-date").val();
        item_master.DueDate = $("#due-date").val();
        item_master.DeliveryDate = $("#due-date").val();
        item_master.DocumentDate = $("#document-date").val();
        item_master.Remarks = $("#remark-id").val();
        item_master.DraftAPDetails = creditMemo_details.length === 0 ? new Array() : creditMemo_details;
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.SeriesDID = parseInt($("#SeriesDetailID").val());
        item_master.SeriesID = parseInt($("#invoice-no").val());
        item_master.DocTypeID = parseInt($("#DocumentTypeID").val());
        item_master.InvoiceNumber = parseInt($("#next_number").val());
        item_master.AppliedAmount = num.toNumberSpecial($("#applied-amount").val());
        item_master.DownPayment = num.toNumberSpecial($("#total-dp-value").val());
        freightMaster.FreightSaleDetails = freights.length === 0 ? new Array() : freights;
        item_master.FreightSalesView = freightMaster;
        item_master.FreightAmount = $("#freight-value").val();
        item_master.BranchID = parseInt($("#branch").val());

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
        });
        dialogsave.confirm(function () {
            item_master.DraftName = $("#draftname").val();
            $("#loading").prop("hidden", false);

            $.ajax({
                url: "/Sale/SaveDraft",
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
    });
    /// submit data ///
    $("#submit-item").on("click", function (e) {
        const item_master = master[0];
        item_master.SaleEmID = $("#sale-emid").val();
        item_master.WarehouseID = parseInt($("#ware-id").val());
        item_master.RefNo = $("#ref-id").val();
        item_master.PostingDate = $("#post-date").val();
        item_master.DueDate = $("#due-date").val();
        item_master.DeliveryDate = $("#due-date").val();
        item_master.DocumentDate = $("#document-date").val();
        item_master.Remarks = $("#remark-id").val();
        item_master.RequestedBy = $("#request_by").val();
        item_master.ShippedBy = $("#shipped_by").val();
        item_master.ReceivedBy = $("#received_by").val();
        item_master.SaleCreditMemoDetails = creditMemo_details.length === 0 ? new Array() : creditMemo_details;
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.SeriesDID = parseInt($("#SeriesDetailID").val());
        item_master.SeriesID = parseInt($("#invoice-no").val());
        item_master.DocTypeID = parseInt($("#DocumentTypeID").val());
        item_master.InvoiceNumber = parseInt($("#next_number").val());
        item_master.AppliedAmount = num.toNumberSpecial($("#applied-amount").val()) == 0 ? num.toNumberSpecial($("#total-id").val()) : num.toNumberSpecial($("#applied-amount").val());
        item_master.DownPayment = num.toNumberSpecial($("#total-dp-value").val());
        freightMaster.FreightSaleDetails = freights.length === 0 ? new Array() : freights;
        item_master.FreightSalesView = freightMaster;
        item_master.FreightAmount = $("#freight-value").val();
        item_master.BranchID = parseInt($("#branch").val());
        const seriesID = parseInt($("#seriesID").val());
        if (master[0].SARID > 0 && master[0].CopyType > 0) {

            $.get("/Sale/GetSaleARByInvoiceNo", { invoiceNo: master[0].CopyKey, seriesID: seriesID }, function (data) {
                if (data.AppliedAmount > 0) {
                    createSaleCreditMemo(master[0], __singleElementJE, seriesID, ARDownPayments);
                    // var dialogSubmit = new DialogBox({
                    //     content: "The remaining applied amount is " + data.AppliedAmount
                    //         + ". Do you want clear and create sale credit memo?",
                    //     type: "ok-cancel",
                    //     icon: "warning"
                    // });

                    // dialogSubmit.confirm(function () {
                    //     createSaleCreditMemo(master[0], __singleElementJE, seriesID, ARDownPayments);
                    //     this.meta.shutdown();
                    // });
                }
                else {
                    var dialogSubmit = new DialogBox({
                        content: "Are you sure you want to create sale credit memo?",
                        type: "ok-cancel",
                        icon: "warning"
                    });
                    dialogSubmit.confirm(function () {
                        createSaleCreditMemo(master[0], __singleElementJE, seriesID, ARDownPayments);
                        this.meta.shutdown();
                    });
                }
            });
        } else {
            var dialogSubmit = new DialogBox({
                content: "Are you sure you want to create sale credit memo?",
                type: "ok-cancel",
                icon: "warning"
            });

            dialogSubmit.confirm(function () {
                $("#loading").prop("hidden", false);
                createSaleCreditMemo(master[0], __singleElementJE, seriesID, ARDownPayments);
                this.meta.shutdown();
            });
            dialogSubmit.reject(function () {
                $("#loading").prop("hidden", true);
                this.meta.shutdown();
            });
        }
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
                        let __value = this.value.toLowerCase().replace(/\s/g, "");
                        let rex = new RegExp(__value, "i");
                        let __customers = $.grep(response, function (person) {
                            return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s/g, "").match(rex)
                                || person.Phone.toLowerCase().replace(/\s/g, "").match(rex)
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
    //sale employee
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
                    pageSize: 20,
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
                    template: `<input />`,
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
    function createSaleCreditMemo(_data, seriesJE, seriesID, ard) {
        let checkingSerialString = $("#checkingSerialString").val();
        let checkingBatchString = $("#checkingBatchString").val();
        let basedOnId = $("#basedonid").val();

        $.ajax({
            url: "/Sale/CreateSaleCreditMemo",
            type: "POST",
            data: $.antiForgeryToken({
                data: JSON.stringify(_data),
                seriesJE: JSON.stringify(seriesJE),
                ardownpayment: JSON.stringify(ard),
                seriesID,
                type,
                copykey: $("#copykey").val(),
                BapsedOnID: basedOnId == "" ? "0" : basedOnId,
                serial: JSON.stringify(serials),
                batch: JSON.stringify(batches),
                checkingSerialString,
                checkingBatchString,
            }),
            success: function (data) {

                if (data.IsSerail) {
                    $("#loading").prop("hidden", true);
                    const serial = SerialTemplate({
                        data: {
                            isCreditMemo: true,
                            serials: data.Data,
                        }
                    });
                    serial.serialTemplate();
                    const seba = serial.callbackInfo();
                    serials = seba.serials;
                } else if (data.IsBatch) {
                    $("#loading").prop("hidden", true);
                    const batch = BatchNoTemplate({
                        data: {
                            isCreditMemo: true,
                            batches: data.Data,
                        }
                    });
                    batch.batchTemplate();
                    const seba = batch.callbackInfo();
                    batches = seba.batches;
                } else if (data.IsSerialCopy) {
                    $("#loading").prop("hidden", true);
                    const serial = SerialTemplate({
                        data: {
                            isCopy: true,
                            isCreditMemo: true,
                            serials: data.Data,
                        }
                    });
                    serial.serialTemplate();
                    const seba = serial.callbackInfo();
                    serials = seba.serials;
                } else if (data.IsBatchCopy) {
                    $("#loading").prop("hidden", true);
                    const batch = BatchNoTemplate({
                        data: {
                            isCopy: true,
                            isCreditMemo: true,
                            batches: data.Data,
                        }
                    });
                    batch.batchTemplate();
                    const seba = batch.callbackInfo();
                    batches = seba.batches;
                } else if (data.Model.Action == 1) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        }
                    }, data.Model).refresh(1500);
                    $("#loading").prop("hidden", true);
                }
                else if (data.Model.Action != -1) {
                    if (_data.DraftID != 0) {
                        $.post("/Sale/RemoveDraft", { id: _data.DraftID })
                    };
                }
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, data.Model);
                $(window).scrollTop(0);
                $("#loading").prop("hidden", true);
            }
        });
    }
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
                                    $.get(urlDetail, { draftname: e.data.DraftName, draftARId: e.data.DraftID }, function (res) {

                                        bindDraft(res, keyMaster, keyDetail, key);
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
    function bindDraft(_master, keyMaster, keyDetail, key) {

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
        _master.DraftARDetail.forEach(i => {
            i.DisValue = num.formatSpecial(i.DisValue, disSetting.Prices);
            i.Total = num.formatSpecial(i.Total, disSetting.Prices);
            i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Prices);
            i.TaxValue = num.formatSpecial(i.TaxValue, disSetting.Prices);
            i.FinDisRate = num.formatSpecial(i.FinDisRate, disSetting.Prices);
            i.FinDisValue = num.formatSpecial(i.FinDisValue, disSetting.Prices);
            i.FinTotalValue = num.formatSpecial(i.FinTotalValue, disSetting.Prices);
            i.TaxOfFinDisValue = num.formatSpecial(i.TaxOfFinDisValue, disSetting.Prices);
            i.TotalWTax = num.formatSpecial(i.TotalWTax, disSetting.Prices);

        });

        _priceList = _master[keyMaster].PriceListID;
        // Add more property
        $("#ware-id").val(_master[keyMaster].WarehouseID);
        $("#request_name").val(_master[keyMaster].RequestedByName);
        $("#received_name").val(_master[keyMaster].ReceivedByName);
        $("#shipped_name").val(_master[keyMaster].ShippedByName);
        $("#branch").val(_master[keyMaster].BranchID); 
        $("#request_by").val(_master[keyMaster].RequestedBy);
        $("#received_by").val(_master[keyMaster].ReceivedBy);
        $("#shipped_by").val(_master[keyMaster].ShippedBy);

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
        $("#item-id").prop("disabled", false);
        var ex = findArray("CurID", _master[keyMaster].SaleCurrencyID, ExChange[0]);
        if (!!ex) {
            $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
            $(".cur-class").text(ex.CurName);
        }

        if (_master.DraftARDetail.length > 0) {
            $listItemChoosed.clearRows();
            $listItemChoosed.bindRows(_master.DraftARDetail);
        }
        if (isValidArray(_master[keyDetail])) {
            $listItemChoosed.clearHeaderDynamic(_master[keyDetail][0].AddictionProps)
            $listItemChoosed.createHeaderDynamic(_master[keyDetail][0].AddictionProps)

        }
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

                                    const params = type == 7 ? { number: e.data.InvoiceNo, seriesId: e.data.SeriesID, fromCN: true } :
                                        { number: e.data.InvoiceNo, seriesId: e.data.SeriesID };
                                    $("#sale-emid").val(res[0].SaleEmID);
                                    $("#sale-em").val(res[0].SaleEmName);
                                    $("#copykey").val(e.data.InvoiceNumber);
                                    $("#basedonid").val(e.data.ID);
                                    $.get(urlDetail, params, function (res) {
                                        if (type == 369) {
                                            getSaleItemMasters(res);
                                        } else {
                                            bindItemCopy(res, keyMaster, keyDetail, type);
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
                        let __value = this.value.toLowerCase().replace(/\s/g, "");
                        let items = $.grep(res, function (item) {
                            return item.InvoiceNumber.toLowerCase().replace(/\s/g, "").includes(__value) || item.SubTotal.toString().toLowerCase().replace(/\s/g, "").includes(__value)
                                || item.PostingDate.toLowerCase().replace(/\s/g, "").includes(__value) || item.TotalAmount.toString().toLowerCase().replace(/\s/g, "").includes(__value)
                                || item.Currency.toLowerCase().replace(/\s/g, "").includes(__value) || item.TypeDis.toLowerCase().replace(/\s/g, "").includes(__value);
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
    function bindItemCopy(_master, keyMaster, keyDetail, copyType = 0) {


        $.get("/Sale/GetCustomer", { id: _master[keyMaster].CusID }, function (cus) {
            $("#cus-id").val(cus.Name);
        });
        master[0] = _master[keyMaster];
        freightMaster.IsEditabled = true;
        if (copyType !== 7) {
            freights = _master[keyMaster].FreightSalesView.FreightSaleDetailViewModels;
            freightMaster = _master[keyMaster].FreightSalesView;
            freightMaster.ID = 0;
            freights.forEach(i => {
                i.ID = 0;
                i.FreightSaleID = 0;
                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
                i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
                i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);

            });
            $("#tdp-dailog").removeClass("disabled");
            $("#total-dp-value").val(num.formatSpecial(_master[keyMaster].DownPayment, disSetting.Prices));
            getARDownPayments(_master[keyMaster].CusID, _master[keyMaster].SARID, function (res) {
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
        // creditMemo_details=[];
        _master[keyDetail].forEach(i => {
            i.DisValue = num.formatSpecial(i.DisValue, disSetting.Prices);
            i.Total = num.formatSpecial(i.Total, disSetting.Prices);
            i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Prices);
            i.TaxValue = num.formatSpecial(i.TaxValue, disSetting.Prices);
            i.FinDisRate = num.formatSpecial(i.FinDisRate, disSetting.Prices);
            i.FinDisValue = num.formatSpecial(i.FinDisValue, disSetting.Prices);
            i.FinTotalValue = num.formatSpecial(i.FinTotalValue, disSetting.Prices);
            i.TaxOfFinDisValue = num.formatSpecial(i.TaxOfFinDisValue, disSetting.Prices);
            i.TotalWTax = num.formatSpecial(i.TotalWTax, disSetting.Prices);

        });
        creditMemo_details = _master[keyDetail];


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

        $("#freight-value").val(num.formatSpecial(isNaN(_master[keyMaster].FreightAmount) ? 0 : _master[keyMaster].FreightAmount, disSetting.Prices));
        $("#seriesID").val(_master[keyMaster].SeriesID);
        $("#dis-rate-id").val(num.formatSpecial(_master[keyMaster].DisRate, disSetting.Rates));
        $("#dis-value-id").val(num.formatSpecial(_master[keyMaster].DisValue, disSetting.Prices));
        $("#remark-id").val(_master[keyMaster].Remarks);
        $("#total-id").val(num.formatSpecial(_master[keyMaster].TotalAmount, disSetting.Prices));
        $("#vat-value").val(num.formatSpecial(_master[keyMaster].VatValue, disSetting.Prices));
        setDate("#post-date", _master[keyMaster].PostingDate.toString().split("T")[0]);
        setDate("#due-date", _master[keyMaster].DeliveryDate.toString().split("T")[0]);
        setDate("#document-date", _master[keyMaster].DocumentDate.toString().split("T")[0]);
        $("#seriesID").val(_master[keyMaster].SeriesID);
        $("#show-list-cus").addClass("noneEvent");
        // const appliedAmount = (copyType > 0) ? _master[keyMaster].TotalAmount : _master[keyMaster].AppliedAmount;

        $("#applied-amount").val(num.formatSpecial(_master[keyMaster].AppliedAmount, disSetting.Prices));
        var ex = findArray("CurID", _master[keyMaster].SaleCurrencyID, ExChange[0]);
        if (!!ex) {
            $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
            $(".cur-class").text(ex.CurName);
        }
        iscopied = true;
        master[0].BaseOn = _master[keyMaster].BaseOnID;
        master[0].CopyType = copyType; // 1: Quotation, 2: Order, 3: Delivery, 4: AR  
        master[0].CopyKey = _master[keyMaster].InvoiceNo;
        master[0].BasedCopyKeys = "/" + _master[keyMaster].InvoiceNo;
        setSourceCopy(master[0].BasedCopyKeys);
        if (isValidArray(_master[keyDetail])) {
            $listItemChoosed.clearHeaderDynamic(_master[keyDetail][0].AddictionProps);
            $listItemChoosed.createHeaderDynamic(_master[keyDetail][0].AddictionProps);
            $listItemChoosed.clearRows();
            $listItemChoosed.bindRows(_master[keyDetail]);
        }

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
            if (key.startsWith("IN")) {
                copyInfo += "/Sale AR: " + key;
            }
            if (key.startsWith("AREDT")) {
                copyInfo += "/Sale ARReserve Invoice Editable: " + key;
            }
        });
        $("#source-copy").val(basedCopyKeys);
    }
    function getSaleItemMasters(_master) {
        if (!!_master) {
            $.get("/Sale/GetCustomer", { id: _master.SaleCreditMemo.CusID }, function (cus) {
                $("#cus-id").val(cus.Name);
            });

            master[0] = _master.SaleCreditMemo;
            freights = _master.SaleCreditMemo.FreightSalesView.FreightSaleDetailViewModels;
            freightMaster = _master.SaleCreditMemo.FreightSalesView;
            freightMaster.IsEditabled = false;
            freightMaster.ID = 0;
            freights.forEach(i => {
                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
                i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
                i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
                updatedetailFreight(freights, i.FreightID, "ID", 0);
            });
            creditMemo_details = _master.SaleCreditMemoDetail;
            $("#request_by").prop("disabled", true);
            $("#shipped_by").prop("disabled", true);
            $("#received_by").prop("disabled", true);

            $("#request_name").val(_master.SaleCreditMemo.RequestedByName);
            $("#shipped_name").val(_master.SaleCreditMemo.ShippedByName);
            $("#received_name").val(_master.SaleCreditMemo.ReceivedByName);
            $("#branch").val(_master.SaleCreditMemo.BranchID); 
            $("#request_by").val(_master.SaleCreditMemo.RequestedBy);
            $("#shipped_by").val(_master.SaleCreditMemo.ShippedBy);
            $("#received_by").val(_master.SaleCreditMemo.ReceivedBy);

            $("#next_number").val(_master.SaleCreditMemo.InvoiceNumber);
            $("#sale-emid").val(_master.SaleCreditMemo.SaleEmID);
            $("#sale-em").val(_master.SaleCreditMemo.SaleEmName);
            $("#ware-id").val(_master.SaleCreditMemo.WarehouseID);
            $("#ref-id").val(_master.SaleCreditMemo.RefNo);
            $("#cur-id").val(_master.SaleCreditMemo.SaleCurrencyID);
            $("#sta-id").val(_master.SaleCreditMemo.Status);
            $("#applied-amount").val(_master.SaleCreditMemo.AppliedAmount);
            $("#sub-id").val(num.formatSpecial(_master.SaleCreditMemo.SubTotal, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(_master.SaleCreditMemo.SubTotalAfterDis, disSetting.Prices));
            $("#freight-value").val(num.formatSpecial(_master.SaleCreditMemo.FreightAmount, disSetting.Prices));
            //$("#sub-dis-id").val(_master.SaleQuote.TypeDis);
            $("#dis-rate-id").val(num.formatSpecial(_master.SaleCreditMemo.DisRate, disSetting.Rates));
            $("#dis-value-id").val(num.formatSpecial(_master.SaleCreditMemo.DisValue, disSetting.Prices));
            setDate("#post-date", _master.SaleCreditMemo.PostingDate.toString().split("T")[0]);
            setDate("#due-date", _master.SaleCreditMemo.DeliveryDate.toString().split("T")[0]);
            setDate("#document-date", _master.SaleCreditMemo.DocumentDate.toString().split("T")[0]);
            $("#remark-id").val(_master.SaleCreditMemo.Remarks);
            $("#total-id").val(num.formatSpecial(_master.SaleCreditMemo.TotalAmount, disSetting.Prices));
            $("#vat-value").val(num.formatSpecial(_master.SaleCreditMemo.VatValue, disSetting.Prices));
            $("#item-id").prop("disabled", false);
            $("#applied-amount").val(num.formatSpecial(_master.SaleCreditMemo.AppliedAmount, disSetting.Prices));
            var ex = findArray("CurID", _master.SaleCreditMemo.SaleCurrencyID, ExChange[0]);
            if (!!ex) {
                $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
                $(".cur-class").text(ex.CurName);
            }
            if (isValidArray(_master.SaleCreditMemoDetail)) {
                $listItemChoosed.clearHeaderDynamic(_master.SaleCreditMemoDetail[0].AddictionProps);
                $listItemChoosed.createHeaderDynamic(_master.SaleCreditMemoDetail[0].AddictionProps);
                $listItemChoosed.clearRows();
                $listItemChoosed.bindRows(_master.SaleCreditMemoDetail);
            }
        } else {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Sale Credit Memo Not found!",
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
        $("#submit-item").prop("disabled", true);
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
        $.each(___CN.seriesCN, function (i, item) {
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
    function calDisRate(e, _this) {
        if (_this.value > 100) _this.value = 100;
        if (_this.value < 0) _this.value = 0;
        updatedetail(creditMemo_details, e.data.UomID, e.key, "DisRate", _this.value);
        const disvalue = parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0 ? 0 :
            parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) / 100;

        updatedetail(creditMemo_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
        totalRow($listItemChoosed, e, _this);
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

        updatedetail(creditMemo_details, e.data.UomID, e.key, "Total", totalWDis);
        updatedetail(creditMemo_details, e.data.UomID, e.key, "TaxRate", e.data.TaxRate);
        updatedetail(creditMemo_details, e.data.UomID, e.key, "TaxValue", vatValue);
        updatedetail(creditMemo_details, e.data.UomID, e.key, "FinDisRate", e.data.FinDisRate);
        updatedetail(creditMemo_details, e.data.UomID, e.key, "FinDisValue", fidis);
        updatedetail(creditMemo_details, e.data.UomID, e.key, "TotalWTax", totalwtax);
        updatedetail(creditMemo_details, e.data.UomID, e.key, "FinTotalValue", fitotal);
        updatedetail(creditMemo_details, e.data.UomID, e.key, "TaxOfFinDisValue", taxoffinal);

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
    function calDisValue(e, _this) {
        if (_this.value > e.data.Qty * e.data.UnitPrice) _this.value = e.data.Qty * e.data.UnitPrice;
        if (_this.value == '' || _this.value < 0) _this.value = 0;
        const value = parseFloat(_this.value);
        updatedetail(creditMemo_details, e.data.UomID, e.key, "DisValue", value);
        const ratedis = (value * 100 === 0) || (parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0) ? 0 :
            (value * 100) / (parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice));
        updatedetail(creditMemo_details, e.data.UomID, e.key, "DisRate", ratedis);
        $listItemChoosed.updateColumn(e.key, "DisRate", num.formatSpecial(isNaN(ratedis) ? 0 : ratedis, disSetting.Rates));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisvalue(e) {
        let disvalue = num.toNumberSpecial(e.data.DisRate) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0 ? 0 : (num.toNumberSpecial(e.data.DisRate) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice)) / 100;
        if (disvalue > parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice))
            disvalue = parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice);
        updatedetail(creditMemo_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
    }
    function setSummary(data) {

        let subtotal = 0;
        let total = 0;
        let vat = 0;
        let disRate = $("#dis-rate-id").val();
        data.forEach(i => {
            subtotal += num.toNumberSpecial(i.Total);
            total += num.toNumberSpecial(i.Total);
            vat += num.toNumberSpecial(i.TaxOfFinDisValue);
        });

        const disValue = (num.toNumberSpecial(disRate) * subtotal) === 0 ? 0 : (num.toNumberSpecial(disRate) * subtotal) / 100;
        const _master = master[0];
        _master.SubTotalSys = subtotal * _master.ExchangeRate;
        _master.SubTotal = subtotal;
        _master.VatRate = (vat * 100) === 0 ? 0 : vat * 100 / subtotal;
        _master.VatValue = vat + num.toNumberSpecial(isNaN(freightMaster.TaxSumValue) ? 0 : freightMaster.TaxSumValue) - num.toNumberSpecial(isNaN(__ardpSum.taxSum) ? 0 : __ardpSum.taxSum);
        _master.DisValue = disValue;
        _master.SubTotalAfterDis = subtotal - disValue;
        _master.SubTotalBefDis = subtotal;
        _master.DisRate = disRate;
        _master.TotalAmountSys = total * _master.ExchangeRate;
        _master.TotalAmount = num.toNumberSpecial(_master.SubTotalAfterDis) + _master.VatValue - num.toNumberSpecial(isNaN(__ardpSum.sum) ? 0 : __ardpSum.sum) + num.toNumberSpecial(isNaN(freightMaster.AmountReven) ? 0 : freightMaster.AmountReven);
        _master.AppliedAmount = _master.TotalAmount;
        // creditMemo_details.forEach(i => {
        //     $listItemChoosed.updateColumn(i.LineID, "FinDisValue", _master.DisValue);
        //     updatedetail(creditMemo_details, i.UomID, i.LineID, "FinDisValue", _master.DisValue);
        //     const lastDisval = num.toNumberSpecial(i.Total) - (num.toNumberSpecial(_master.DisRate) * num.toNumberSpecial(i.Total) / 100);
        //     const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
        //         num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
        //     updatedetail(creditMemo_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
        //     updatedetail(creditMemo_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
        //     $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
        //     $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
        // });

        $("#vat-value").val(num.formatSpecial(_master.VatValue, disSetting.Prices));
        $("#sub-id").val(num.formatSpecial(subtotal, disSetting.Prices));
        $("#total-id").val(num.formatSpecial(_master.TotalAmount, disSetting.Prices));
        $("#applied-amount").val(num.formatSpecial(_master.AppliedAmount, disSetting.Prices));
        $("#sub-after-dis").val(num.formatSpecial(_master.SubTotalAfterDis, disSetting.Prices));
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
                if (i.FreightID === key) {
                    i[prop] = value;
                }
            });
        }
    }
    function invoiceDisAllRowRate(_this) {
        if (isValidArray(creditMemo_details)) {
            creditMemo_details.forEach(i => {
                const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) / 100;
                if (_this.value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(value);
                    const taxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;

                    updatedetail(creditMemo_details, i.UomID, i.LineID, "TaxOfFinDisValue", taxOfFinDisValue);
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(taxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                } else if (_this.value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    updatedetail(creditMemo_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(creditMemo_details);
        }
    }
    function invoiceDisAllRowValue(value) {
        if (isValidArray(creditMemo_details)) {
            creditMemo_details.forEach(i => {
                const _value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) / 100;
                if (value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(_value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinDisValue", _value);
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinDisRate", value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(_value);
                    const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                }
                else if (value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinDisValue", _thisvalue);
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinDisRate", value);

                    updatedetail(creditMemo_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(creditMemo_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(creditMemo_details);
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
            freightMaster.AmountReven = isNaN(sumFreight) ? 0 : sumFreight;
            freightMaster.TaxSumValue = isNaN(sumTax) ? 0 : sumTax;
            $(".freightSumAmount").val(num.formatSpecial(freightMaster.AmountReven, disSetting.Prices));
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
        setSummary(creditMemo_details);
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
                    pageSize: 20,
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