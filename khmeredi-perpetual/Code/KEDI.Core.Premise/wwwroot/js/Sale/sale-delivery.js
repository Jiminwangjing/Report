"use strict"
let __singleElementJE,
    delivery_details = [],
    ExChange = [],
    master = [],
    _currency = "",
    _curencyID = 0,
    _priceList = 0,
    freights = [],
    freightMaster = {},
    type = "AR",
    serials = [],
    batches = [],
    ___DN = JSON.parse($("#data-invoice").text()),
    ___branchID = parseInt($("#BranchID").text()),
    disSetting = ___DN.genSetting.Display;
$(document).ready(function () {
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });
    const serial = SerialTemplate({});
    $("#click").click(function () {
        serial.serialTemplate();
    });
    $.each($("[data-date]"), function (i, t) {
        setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
    });
    // tb master
    var draftid = 0;
    const itemmaster = {
        SDID: 0,
        CusID: 0,
        BranchID: 0,
        WarehouseID: 0,
        UserID: 0,
        SaleCurrencyID: 0,
        RequestedBy: 0,
        ShippedBy: 0,
        ReceivedBy: 0,
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
        FeeNote: 0,
        FeeAmount: 0,
        TotalAmount: 0,
        TotalAmountSys: 0,
        CopyType: 0,
        CopyKey: "",
        BasedCopyKeys: "",
        PriceListID: 0,
        LocalSetRate: 0,
        BaseOnID: 0,
        SaleDeliveryDetails: new Array(),
        DraftDeliveryDetails: new Array()
    }
    master.push(itemmaster);
    // get warehouse
    setWarehouse(0);
    // Invoice
    ___DN.seriesDN.forEach(i => {
        if (i.Default == true) {
            $("#DocumentTypeID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    ___DN.seriesJE.forEach(i => {
        if (i.Default == true) {
            $("#JEID").val(i.ID);
            $("#JENumber").val(i.NextNo);
            __singleElementJE = findArray("ID", i.ID, ___DN.seriesJE);
        }
    });
    let selected = $("#invoice-no");
    selectSeries(selected);
    $('#invoice-no').change(function () {
        var id = $(this).val();
        var seriesDN = findArray("ID", id, ___DN.seriesDN);
        ___DN.seriesDN.Number = seriesDN.NextNo;
        ___DN.seriesDN.ID = id;
        $("#DocumentID").val(seriesDN.DocumentTypeID);
        $("#number").val(seriesDN.NextNo);
        $("#next_number").val(seriesDN.NextNo);
    });
    if (___DN.seriesDN.length == 0) {
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
        //$("#show-list-cus").prop("disabled", true);

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
    //====show employee===
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
                        updatedetail(delivery_details, e.data.UomID, e.key, "UnitPrice", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this);
                        setSummary(delivery_details);
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
                        setSummary(delivery_details);
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
                        setSummary(delivery_details);
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
                        updatedetail(delivery_details, e.data.UomID, e.key, "Qty", this.value);
                        updatedetail(delivery_details, e.data.UomID, e.key, "OpenQty", this.value);
                        //calDisrate(e);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this);
                        setSummary(delivery_details);
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
                            updatedetail(delivery_details, e.data.UomID, e.key, "TaxRate", taxg.Rate);
                        } else {
                            updatedetail(delivery_details, e.data.UomID, e.key, "TaxRate", 0);
                        }
                        updatedetail(delivery_details, e.data.UomID, e.key, "TaxGroupID", parseInt(this.value))
                        totalRow($listItemChoosed, e, this);
                        setSummary(delivery_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "UoMs",
                template: `<select class='form-control font-size uom'></select>`,
                on: {
                    "change": function (e) {
                        updatedetail(delivery_details, e.data.UomID, e.key, "UomID", parseInt(this.value));
                        const uomList = findArray("UoMID", this.value, e.data.UomPriceLists);
                        const uom = findArray("ID", this.value, e.data.UoMsList);
                        if (!!uom && !!uomList) {
                            updatedetail(delivery_details, e.data.UomID, e.key, "UnitPrice", uomList.UnitPrice);
                            updatedetail(delivery_details, e.data.UomID, e.key, "UomName", uom.Name);
                            updatedetail(delivery_details, e.data.UomID, e.key, "Factor", uom.Factor);
                            $listItemChoosed.updateColumn(e.key, "UnitPrice", num.formatSpecial(uomList.UnitPrice, disSetting.Prices));
                            calDisvalue(e);
                            totalRow($listItemChoosed, e, this);
                            setSummary(delivery_details);
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
                        updatedetail(delivery_details, e.data.UomID, e.key, "Remarks", this.value);
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
                        delivery_details = delivery_details.filter(i => i.LineID !== e.key);
                        setSummary(delivery_details);
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
                    delivery_details = [];
                    $listItemChoosed.clearRows();
                    $listItemChoosed.bindRows(delivery_details);
                    setSummary(delivery_details);
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
        //     $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
        //     master[0].TotalAmount = totalAfDis;
        //     master[0].DisValue = disvalue;
        //     master[0].DisRate = this.value;
        //     master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
        // } else {
        //     setSummary(delivery_details);
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
        //     setSummary(delivery_details);
        // }
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
            setSummary(delivery_details);
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
                            if (isValidArray(delivery_details)) {
                                res.forEach(i => {
                                    const isExisted = delivery_details.some(qd => qd.ItemID === i.ItemID);
                                    if (isExisted) {
                                        const item = delivery_details.filter(_i => _i.BarCode === i.BarCode)[0];
                                        if (!!item) {
                                            const qty = parseFloat(item.Qty) + 1;
                                            const openqty = parseFloat(item.Qty) + 1;
                                            updatedetail(delivery_details, i.UomID, item.LineID, "Qty", qty);
                                            updatedetail(delivery_details, i.UomID, item.LineID, "OpenQty", openqty);
                                            $listItemChoosed.updateColumn(item.LineID, "Qty", qty);
                                            const _e = { key: item.LineID, data: item };
                                            calDisvalue(_e);
                                            totalRow($listItemChoosed, _e, qty);
                                            //setSummary(delivery_details);
                                        }
                                    } else {
                                        delivery_details.push(i);
                                        $listItemChoosed.addRow(i);
                                    }
                                })
                            } else {
                                $listItemChoosed.clearHeaderDynamic(res[0].AddictionProps)
                                $listItemChoosed.createHeaderDynamic(res[0].AddictionProps)
                                res.forEach(i => {
                                    delivery_details.push(i);
                                    $listItemChoosed.addRow(i);
                                })

                            }
                            setSummary(delivery_details);
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
        $("#copy-from").prop("disabled", true);
    });
    $("#next_number").on("keypress", function (e) {
        if (e.which === 13 && !!$("#invoice-no").val().trim()) {
            setDisabled();
            if ($("#next_number").val() == "*") {
                initModalDialog("/Sale/GetSaleDeliveriesDisplay", "/Sale/FindSaleDelivery", "Fine Sale Delivery", "SDID", "SaleDelivery", "SaleDeliveryDetails", 369);
            }
            else {
                $.ajax({
                    url: "/Sale/FindSaleDelivery",
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
                                    delivery_details.push(res[0]);
                                    $listItemChoosed.addRow(res[0]);
                                    const ee = {
                                        key: res[0].LineID,
                                        data: res[0]
                                    }

                                    totalRow($listItemChoosed, ee, res[0].Qty);
                                    setSummary(delivery_details);
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
    //Copy from quotation
    $("#copy-from").change(function () {
        switch (this.value) {
            case "1":
                initModalDialog("/Sale/GetSaleQuotes", "/Sale/GetSaleQuoteDetailCopy", "Sale Quotes (Copy)", "SQID", "SaleQuote", "SaleQuoteDetails", 1);
                type = "QO";
                break;
            case "2":
                initModalDialog("/Sale/GetSaleOrders", "/Sale/GetSaleOrderDetailCopy", "Sale Orders (Copy)", "SOID", "SaleOrder", "SaleOrderDetails", 2);
                type = "SO";
                break;
            case "9":
                initModalDialog("/Sale/GetARReserveInvoice", "/Sale/GetARReserveInvoiceDetailDetailCopy", "AR Reserve Invoice (Copy)", "ID", "ARReserveInvoice", "ARReserveInvoiceDetails", 9);
                type = "AR";
                break;
            case "10":
                initModalDialog("/Sale/GetARReserveInvoiceEDT", "/Sale/GetARReserveInvoiceEDTDetailCopy", "AR Reserve Invoice Editable (Copy)", "ID", "ARReserveInvoice", "ARReserveInvoiceDetails", 15);
                type = "AR";
                break;
        }
        $(this).val(0);
    });
    // find draft
    $("#btn-finddraft").click(function () {
        DraftDataDialog("/Sale/DisplayDraftDelivery", "/Sale/FindDraftDelivery", "Draft Detail", "LineID", "SaleDraft", "DraftARDetail");
    });
    /// submit data ///
    $("#submit-item").on("click", function (e) {
        SubmitData("saledelivery");
    });
    // save draft
    $("#submit-draft").on("click", function (e) {
        SubmitData("draft");
    });
    function SubmitData(savetype) {
        const item_master = master[0];
        item_master.SaleEmID = $("#sale-emid").val();
        freightMaster.FreightSaleDetails = freights.length === 0 ? new Array() : freights;
        item_master.WarehouseID = parseInt($("#ware-id").val());
        item_master.RequestedBy = $("#request_by").val();
        item_master.ShippedBy = $("#shipped_by").val();
        item_master.ReceivedBy = $("#received_by").val();
        item_master.RefNo = $("#ref-id").val();
        item_master.PostingDate = $("#post-date").val();
        item_master.DueDate = $("#due-date").val();
        item_master.DeliveryDate = $("#due-date").val();
        item_master.DocumentDate = $("#document-date").val();
        item_master.Remarks = $("#remark-id").val();
        item_master.SaleDeliveryDetails = delivery_details.length === 0 ? new Array() : delivery_details;
        item_master.DraftDeliveryDetails = delivery_details.length === 0 ? new Array() : delivery_details;
        item_master.FreightSalesView = freightMaster;
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.SeriesDID = parseInt($("#SeriesDetailID").val());
        item_master.SeriesID = parseInt($("#invoice-no").val());
        item_master.DocTypeID = parseInt($("#DocumentTypeID").val());
        item_master.InvoiceNumber = parseInt($("#next_number").val());
        item_master.FreightAmount = $("#freight-value").val();
        item_master.BranchID = parseInt($("#branch").val());
        if (savetype == "saledelivery") {
            var dialogSubmit = new DialogBox({
                content: "Are you sure you want to save the item?",
                type: "yes-no",
                icon: "warning"
            });
            dialogSubmit.confirm(function () {
                $("#loading").prop("hidden", false);
                $.ajax({
                    url: "/Sale/CreateSaleDelivery",
                    type: "post",
                    data: $.antiForgeryToken(
                        {
                            data: JSON.stringify(item_master),
                            seriesJE: JSON.stringify(__singleElementJE),
                            serial: JSON.stringify(serials),
                            batch: JSON.stringify(batches),
                            type: type
                        }),
                    success: function (model) {

                        if (model.IsSerail) {
                            $("#loading").prop("hidden", true);
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
                            const batch = BatchNoTemplate({
                                data: {
                                    batches: model.Data,
                                }
                            });
                            batch.batchTemplate();
                            const seba = batch.callbackInfo();
                            batches = seba.batches;

                        } if (model.Model.Action == 1) {
                            $.get("/Sale/DeleteDraftDelivert", { id: draftid }, function () { })
                            new ViewMessage({
                                summary: {
                                    selector: "#error-summary"
                                }
                            }, model.Model).refresh(1500);
                            $("#loading").prop("hidden", true);
                        } else {
                            $("#loading").prop("hidden", true);
                            new ViewMessage({
                                summary: {
                                    selector: "#error-summary"
                                }
                            }, model.Model);
                        }
                        $(window).scrollTop(0);

                    }
                });
                this.meta.shutdown();
            });
            dialogSubmit.reject(function () {
                $("#loading").prop("hidden", true);
            })
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
                $("#loading").prop("hidden", false);

                $.ajax({
                    url: "/Sale/SaveDraftDelivery",
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
                                        res.SaleDelivery = res.SaleDraft;
                                        res.SaleDelivery.ID = res.SaleDraft.DraftID;
                                        res.SaleDelivery.RequestedByID = res.SaleDelivery.RequestedBy;
                                        res.SaleDelivery.ReceivedByID = res.SaleDelivery.ReceivedBy;
                                        res.SaleDelivery.ShippedByID = res.SaleDelivery.ShippedBy;

                                        res.SaleDeliveryDetails = res.DraftARDetail;
                                        $("#draftname").val(res.SaleDelivery.Name);
                                        res.SaleDeliveryDetails.forEach(i => {
                                            i.ID = i.DraftDetailID;
                                        })

                                        getSaleItemMasters(res);
                                        //bindDraft(res, keyMaster, keyDetail, key);
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
                    template: `<input class="disabled" >`,
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
                    template: `<select class="disabled"></select>`,
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
                        pageSize: 10,
                        enabled: true
                    },
                    visibleFields: ["Code", "InvoiceNumber", "PostingDate", "Currency", "SubTotal", "TypeDis", "TotalAmount", "Remarks"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                            on: {
                                "click": function (e) {
                                    master[0].BaseOnID = e.key;
                                    $("#sale-emid").val(res[0].SaleEmID);
                                    $("#sale-em").val(res[0].SaleEmName);
                                    $.get(urlDetail, { number: e.data.InvoiceNo, seriesId: e.data.SeriesID }, function (res) {
                                        if (type == 369) {
                                            getSaleItemMasters(res, type);
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





    function bindItemCopy(_master, keyMaster, keyDetail, key, copyType = 0) {
        $.get("/Sale/GetCustomer", { id: _master[keyMaster].CusID }, function (cus) {
            $("#cus-id").val(cus.Name);
        });
        master[0] = _master[keyMaster];
        master[0].BaseOnID = _master[keyMaster][key];
        freights = _master[keyMaster].FreightSalesView.FreightSaleDetailViewModels;
        freightMaster = _master[keyMaster].FreightSalesView;
        freightMaster.IsEditabled = true;
        freights.forEach(i => {
            i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
            i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
            i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
            i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
        });
        delivery_details = _master[keyDetail];
        _priceList = _master[keyMaster].PriceListID;

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
        // setSummary(delivery_details);
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

            if (key.startsWith("SD")) {
                copyInfo += "/Sale Delivery: " + key;
            }
            if (key.startsWith("RI")) {
                copyInfo += "AR Reserve Invoice: " + key;
            }
        });
        master[0].BasedCopyKeys = copyInfo;
        $("#source-copy").val(basedCopyKeys);
    }
    function getSaleItemMasters(_master) {
        if (!!_master) {
            $.get("/Sale/GetCustomer", { id: _master.SaleDelivery.CusID }, function (cus) {
                $("#cus-id").val(cus.Name);
            });
            master[0] = _master.SaleDelivery;
            freights = _master.SaleDelivery.FreightSalesView.FreightSaleDetailViewModels;
            freightMaster = _master.SaleDelivery.FreightSalesView;
            freightMaster.IsEditabled = false;
            freightMaster.ID = 0;
            freights.forEach(i => {
                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
                i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
                i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
                updatedetailFreight(freights, i.FreightID, "ID", 0);
            });
            delivery_details = _master.SaleDeliveryDetails;
            //$("#request_by").prop("disabled", true);
            //$("#shipped_by").prop("disabled", true);
            //$("#received_by").prop("disabled", true);

            $("#request_name").val(_master.SaleDelivery.RequestedByName);
            $("#shipped_name").val(_master.SaleDelivery.ShippedByName);
            $("#received_name").val(_master.SaleDelivery.ReceivedByName);
            $("#branch").val(_master.SaleDelivery.BranchID);
            $("#request_by").val(_master.SaleDelivery.RequestedByID);
            $("#shipped_by").val(_master.SaleDelivery.ShippedByID);
            $("#received_by").val(_master.SaleDelivery.ReceivedByID);
            $("#next_number").val(_master.SaleDelivery.InvoiceNumber);
            $("#sale-emid").val(_master.SaleDelivery.SaleEmID);
            $("#sale-em").val(_master.SaleDelivery.SaleEmName);
            $("#ware-id").val(_master.SaleDelivery.WarehouseID);
            $("#ref-id").val(_master.SaleDelivery.RefNo);
            $("#cur-id").val(_master.SaleDelivery.SaleCurrencyID);
            $("#sta-id").val(_master.SaleDelivery.Status);
            $("#sub-id").val(num.formatSpecial(_master.SaleDelivery.SubTotal, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(_master.SaleDelivery.SubTotalAfterDis, disSetting.Prices));
            $("#freight-value").val(num.formatSpecial(_master.SaleDelivery.FreightAmount, disSetting.Prices));
            //$("#sub-dis-id").val(_master.SaleQuote.TypeDis);
            $("#dis-rate-id").val(num.formatSpecial(_master.SaleDelivery.DisRate, disSetting.Rates));
            $("#dis-value-id").val(num.formatSpecial(_master.SaleDelivery.DisValue, disSetting.Prices));
            setDate("#post-date", _master.SaleDelivery.PostingDate.toString().split("T")[0]);
            setDate("#due-date", _master.SaleDelivery.DeliveryDate.toString().split("T")[0]);
            setDate("#document-date", _master.SaleDelivery.DocumentDate.toString().split("T")[0]);
            $("#remark-id").val(_master.SaleDelivery.Remarks);
            $("#total-id").val(num.formatSpecial(_master.SaleDelivery.TotalAmount, disSetting.Prices));
            $("#vat-value").val(num.formatSpecial(_master.SaleDelivery.VatValue, disSetting.Prices));
            $("#source-copy").val(_master.SaleDelivery.BasedCopyKeys);
            $("#item-id").prop("disabled", false);
            if (type == 369) {
                $("#source-copy").val(_master.SaleDelivery.InvoiceNo);
            }
            var ex = findArray("CurID", _master.SaleDelivery.SaleCurrencyID, ExChange[0]);
            if (!!ex) {
                $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
                $(".cur-class").text(ex.CurName);
            }
            if (isValidArray(_master.SaleDeliveryDetails)) {
                $listItemChoosed.clearHeaderDynamic(_master.SaleDeliveryDetails[0].AddictionProps);
                $listItemChoosed.createHeaderDynamic(_master.SaleDeliveryDetails[0].AddictionProps);
                $listItemChoosed.bindRows(_master.SaleDeliveryDetails);
            }
            //setRequested(_master, key, detailKey, copyType);
        } else {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Sale Delivery Not found!",
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
        $("#dis-id").prop("disabled", true);
        $("#sub-dis-id").prop("disabled", true);
        $("#sub-id").prop("disabled", true);
        $("#sub-after-dis").prop("disabled", true);
        $("#dis-value-id").prop("disabled", true);
        $("#dis-rate-id").prop("disabled", true);
        $("#remark-id").prop("disabled", true);
        $("#include-vat-id").prop("disabled", true);
        $("#cur-id").prop("disabled", true);
        $("#ref-id").prop("disabled", true);
        $("#ware-id").prop("disabled", true);
        $("#branch").prop("disabled", true);
        $("#copy-from").prop("disabled", true);
        $("#show-list-cus").addClass("csr-default").off();
        $("#fee-note").prop("disabled", true);
        $("#fee-amount").prop("disabled", true);
        $.each($("[data-date]"), function (i, t) {
            $(t).prop("disabled", true);
        });
    }
    function selectSeries(selected) {
        $.each(___DN.seriesDN, function (i, item) {
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

        updatedetail(delivery_details, e.data.UomID, e.key, "Total", totalWDis);
        updatedetail(delivery_details, e.data.UomID, e.key, "TaxRate", e.data.TaxRate);
        updatedetail(delivery_details, e.data.UomID, e.key, "TaxValue", vatValue);
        updatedetail(delivery_details, e.data.UomID, e.key, "FinDisRate", e.data.FinDisRate);
        updatedetail(delivery_details, e.data.UomID, e.key, "FinDisValue", fidis);
        updatedetail(delivery_details, e.data.UomID, e.key, "TotalWTax", totalwtax);
        updatedetail(delivery_details, e.data.UomID, e.key, "FinTotalValue", fitotal);
        updatedetail(delivery_details, e.data.UomID, e.key, "TaxOfFinDisValue", taxoffinal);

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
        updatedetail(delivery_details, e.data.UomID, e.key, "DisRate", _this.value);
        const disvalue = parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0 ? 0 :
            parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) / 100;

        updatedetail(delivery_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisValue(e, _this) {
        if (_this.value > e.data.Qty * e.data.UnitPrice) _this.value = e.data.Qty * e.data.UnitPrice;
        if (_this.value == '' || _this.value < 0) _this.value = 0;
        const value = parseFloat(_this.value);
        updatedetail(delivery_details, e.data.UomID, e.key, "DisValue", value);
        const ratedis = (value * 100 === 0) || (parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0) ? 0 :
            (value * 100) / (parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice));
        updatedetail(delivery_details, e.data.UomID, e.key, "DisRate", ratedis);
        $listItemChoosed.updateColumn(e.key, "DisRate", num.formatSpecial(isNaN(ratedis) ? 0 : ratedis, disSetting.Rates));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisvalue(e) {
        let disvalue = num.toNumberSpecial(e.data.DisRate) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0 ? 0 :
            (num.toNumberSpecial(e.data.DisRate) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice)) / 100;
        if (disvalue > parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice)) disvalue = parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice);
        updatedetail(delivery_details, e.data.UomID, e.key, "DisValue", disvalue);
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
        _master.VatValue = vat + num.toNumberSpecial(freightMaster.TaxSumValue);
        _master.DisValue = disValue;
        _master.SubTotalAfterDis = subtotal - disValue;
        _master.SubTotalBefDis = subtotal;
        _master.DisRate = disRate;
        _master.TotalAmount = num.toNumberSpecial(_master.SubTotalAfterDis) + _master.VatValue + num.toNumberSpecial(freightMaster.AmountReven);
        _master.TotalAmountSys = _master.TotalAmount * _master.ExchangeRate;
        // delivery_details.forEach(i => {
        //     const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_master.DisRate) === 0 ? 0 :
        //         num.toNumberSpecial(i.Total) * num.toNumberSpecial(_master.DisRate) / 100;
        //     $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
        //     updatedetail(delivery_details, i.UomID, i.LineID, "FinDisValue", value);
        //     const lastDisval = num.toNumberSpecial(i.Total) - (num.toNumberSpecial(_master.DisRate) * num.toNumberSpecial(i.Total) / 100);
        //     const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
        //         num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
        //     updatedetail(delivery_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
        //     updatedetail(delivery_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
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
        if (isValidArray(delivery_details)) {
            delivery_details.forEach(i => {
                const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) / 100;
                if (_this.value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(value);
                    const taxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;

                    updatedetail(delivery_details, i.UomID, i.LineID, "TaxOfFinDisValue", taxOfFinDisValue);
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(taxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                } else if (_this.value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    updatedetail(delivery_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(delivery_details);
        }
    }
    function invoiceDisAllRowValue(value) {
        if (isValidArray(delivery_details)) {
            delivery_details.forEach(i => {
                const _value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) / 100;
                if (value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(_value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinDisValue", _value);
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinDisRate", value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(_value);
                    const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
                    updatedetail(delivery_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                }
                else if (value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinDisValue", _thisvalue);
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinDisRate", value);

                    updatedetail(delivery_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(delivery_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(delivery_details);
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
});