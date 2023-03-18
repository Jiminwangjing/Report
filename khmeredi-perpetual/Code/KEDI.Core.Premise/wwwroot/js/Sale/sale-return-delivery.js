"use strict"
let __singleElementJE,
    re_details = [],
    master = [],
    ExChange = [],
    _currency = "",
    _curencyID = 0,
    _priceList = 0,
    freights = [],
    serials = [],
    batches = [],
    freightMaster = {},
    type = "add",
    ___RE = JSON.parse($("#data-invoice").text()),
    disSetting = ___RE.genSetting.Display;
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
        SOID: 0,
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
        DeliveryDate: "",
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
        LocalSetRate: 0,
        PriceListID: 0,
        SaleOrderDetails: new Array()
    }
    master.push(itemmaster);
    // Invoice
    ___RE.seriesRE.forEach(i => {
        if (i.Default == true) {
            $("#DocumentTypeID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    ___RE.seriesJE.forEach(i => {
        if (i.Default == true) {
            $("#JEID").val(i.ID);
            $("#JENumber").val(i.NextNo);
            __singleElementJE = findArray("ID", i.ID, ___RE.seriesJE);
        }
    });
    let selected = $("#invoice-no");
    selectSeries(selected);

    $('#invoice-no').change(function () {
        var id = ($(this).val());
        var seriesRE = findArray("ID", id, ___RE.seriesRE);
        ___RE.seriesRE.Number = seriesRE.NextNo;
        ___RE.seriesRE.ID = id;
        $("#DocumentID").val(seriesRE.DocumentTypeID);
        $("#number").val(seriesRE.NextNo);
        $("#next_number").val(seriesRE.NextNo);
    });
    if (___RE.seriesRE.length == 0) {
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
                                $("#sale-emid").val(semid);
                                $("#sale-em").val(semnale);
                                $("#cus-id").val(name);
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
                template: `<input class='form-control font-size unitprice disabledrowinput' type='text' />`,
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
                template: `<span class='font-size cur disabledrowinput'></span`,
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
                template: `<select class='form-control font-size taxgroup disabledrowinput'></select>`,
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
                template: `<select class='form-control font-size uom disabledrowinput'></select>`,
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
                            setSummary(re_details);
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
        setDisabled();
        if (e.which === 13 && !!$("#invoice-no").val().trim()) {
            $.ajax({
                url: "/Sale/FindReturnDelivery",
                data: { number: $("#next_number").val(), seriesID: parseInt($("#invoice-no").val()) },
                success: function (result) {
                    getSaleItemMasters(result);
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
                                    const ee = {
                                        key: res[0].LineID,
                                        data: res[0]
                                    }

                                    totalRow($listItemChoosed, ee, res[0].Qty);
                                    setSummary(re_details);
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
            case "3":
                //initModalDialog("/Sale/GetSaleDeliverys", "SQID", 1);
                $.get("/Sale/GetSaleDeliveries", { cusId: master[0].CusID }, function (res) {
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
                        }
                    });
                    dialog.invoke(function () {
                        const itemMasterCopy = ViewTable({
                            keyField: "SDID",
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
                                            $.get("/Sale/GetSaleDeliveryDetailCopy", { number: e.data.InvoiceNo, seriesId: e.data.SeriesID }, function (res) {
                                                bindItemCopy(res, 3);
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
                break;
        }
        $(this).val(0);
    });
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
    function bindItemCopy(_master, copyType = 0) {
        $.get("/Sale/GetCustomer", { id: _master.SaleDelivery.CusID }, function (cus) {
            $("#cus-id").val(cus.Name);
        });
        master[0].BaseOnID = _master.SaleDelivery.BaseOnID;
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
        re_details = _master.SaleDeliveryDetails;
        $("#request_by").val(_master.SaleDelivery.RequestedByID);
        $("#shipped_by").val(_master.SaleDelivery.ShippedByID);
        $("#received_by").val(_master.SaleDelivery.ReceivedByID);
        $("#request_name").val(_master.SaleDelivery.RequestedByName);
        $("#shipped_name").val(_master.SaleDelivery.ShippedByName);
        $("#received_name").val(_master.SaleDelivery.ReceivedByName);
        $("#branch").val(_master.SaleDelivery.BranchID);
        $("#sale-emid").val(_master.SaleDelivery.SaleEmID);
        $("#sale-em").val(_master.SaleDelivery.SaleEmName);
        $("#ware-id").val(_master.SaleDelivery.WarehouseID);
        $("#ref-id").val(_master.SaleDelivery.RefNo);
        $("#cur-id").val(_master.SaleDelivery.SaleCurrencyID);
        $("#sta-id").val(_master.SaleDelivery.Status);
        $("#sub-id").val(num.formatSpecial(_master.SaleDelivery.SubTotal, disSetting.Prices));
        $("#sub-after-dis").val(num.formatSpecial(_master.SaleDelivery.SubTotalAfterDis, disSetting.Prices));
        $("#dis-rate-id").val(num.formatSpecial(_master.SaleDelivery.DisRate, disSetting.Rates));
        $("#dis-value-id").val(num.formatSpecial(_master.SaleDelivery.DisValue, disSetting.Prices));
        $("#freight-value").val(num.formatSpecial(_master.SaleDelivery.FreightAmount, disSetting.Prices));
        $("#remark-id").val(_master.SaleDelivery.Remarks);
        $("#SeriesID").val(_master.SaleDelivery.SeriesID);
        $("#total-id").val(num.formatSpecial(_master.SaleDelivery.TotalAmount, disSetting.Prices));
        $("#vat-value").val(num.formatSpecial(_master.SaleDelivery.VatValue, disSetting.Prices));
        setDate("#post-date", _master.SaleDelivery.PostingDate.toString().split("T")[0]);
        setDate("#delivery-date", _master.SaleDelivery.DeliveryDate.toString().split("T")[0]);
        setDate("#document-date", _master.SaleDelivery.DocumentDate.toString().split("T")[0]);
        $("#show-list-cus").addClass("noneEvent");
        type = "dn";
        var ex = findArray("CurID", _master.SaleDelivery.SaleCurrencyID, ExChange[0]);
        if (!!ex) {
            $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
            $(".cur-class").text(ex.CurName);
        }
        master[0].CopyType = copyType; // 1: Quotation, 2: Order, 3: Delivery, 4: AR  
        master[0].CopyKey = _master.SaleDelivery.InvoiceNo;
        master[0].BasedCopyKeys = "/" + _master.SaleDelivery.InvoiceNo;
        setSourceCopy(master[0].BasedCopyKeys)
        if (isValidArray(_master.SaleDeliveryDetails)) {
            $listItemChoosed.clearHeaderDynamic(_master.SaleDeliveryDetails[0].AddictionProps);
            $listItemChoosed.createHeaderDynamic(_master.SaleDeliveryDetails[0].AddictionProps);
            $listItemChoosed.bindRows(_master.SaleDeliveryDetails);
        }
        //setDisabledrowInput();
    }
    function setSourceCopy(basedCopyKeys) {
        let copyKeys = new Array();
        if (basedCopyKeys) {
            copyKeys = basedCopyKeys.split("/");
        }
        var copyInfo = "";
        $.each(copyKeys, function (i, key) {
            if (key.startsWith("DN")) {
                copyInfo += "/Sale Delivery: " + key;
            }
        });
        master[0].BasedCopyKeys = copyInfo;
        $("#source-copy").val(basedCopyKeys);
    }
    function getSaleItemMasters(_master) {
        if (!!_master) {
            $.get("/Sale/GetCustomer", { id: _master.ReturnDelivery.CusID }, function (cus) {
                $("#cus-id").val(cus.Name);
            });
            master[0] = _master.ReturnDelivery;
            freights = _master.ReturnDelivery.FreightSalesView.FreightSaleDetailViewModels;
            freightMaster = _master.ReturnDelivery.FreightSalesView;
            freights.forEach(i => {
                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
                i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
                i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
            });
            freightMaster.IsEditabled = false;
            re_details = _master.ReturnDeliveryDetails;
            $("#request_by").prop("disabled", true);
            $("#shipped_by").prop("disabled", true);
            $("#received_by").prop("disabled", true);
            $("#branch").val(_master.ReturnDelivery.BranchID);
            $("#request_name").val(_master.ReturnDelivery.RequestedByName);
            $("#shipped_name").val(_master.ReturnDelivery.ShippedByName);
            $("#received_name").val(_master.ReturnDelivery.ReceivedByName);
            $("#request_by").val(_master.ReturnDelivery.RequestedByID);
            $("#shipped_by").val(_master.ReturnDelivery.ShippedByID);
            $("#received_by").val(_master.ReturnDelivery.ReceivedByID);
            $("#sale-emid").val(_master.ReturnDelivery.SaleEmID);
            $("#sale-em").val(_master.ReturnDelivery.SaleEmName);
            $("#ware-id").val(_master.ReturnDelivery.WarehouseID);
            $("#ref-id").val(_master.ReturnDelivery.RefNo);
            $("#cur-id").val(_master.ReturnDelivery.SaleCurrencyID);
            $("#sta-id").val(_master.ReturnDelivery.Status);
            $("#sub-id").val(num.formatSpecial(_master.ReturnDelivery.SubTotal, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(_master.ReturnDelivery.SubTotalAfterDis, disSetting.Prices));
            $("#freight-value").val(num.formatSpecial(_master.ReturnDelivery.FreightAmount, disSetting.Prices));
            //$("#sub-dis-id").val(_master.SaleDelivery.TypeDis);
            $("#dis-rate-id").val(num.formatSpecial(_master.ReturnDelivery.DisRate, disSetting.Rates));
            $("#dis-value-id").val(num.formatSpecial(_master.ReturnDelivery.DisValue, disSetting.Prices));
            setDate("#post-date", _master.ReturnDelivery.PostingDate.toString().split("T")[0]);
            setDate("#delivery-date", _master.ReturnDelivery.DeliveryDate.toString().split("T")[0]);
            setDate("#document-date", _master.ReturnDelivery.DocumentDate.toString().split("T")[0]);
            $("#remark-id").val(_master.ReturnDelivery.Remarks);
            $("#total-id").val(num.formatSpecial(_master.ReturnDelivery.TotalAmount, disSetting.Prices));
            $("#vat-value").val(num.formatSpecial(_master.ReturnDelivery.VatValue, disSetting.Prices));
            $("#source-copy").val(_master.ReturnDelivery.BasedCopyKeys);
            $("#item-id").prop("disabled", false);
            var ex = findArray("CurID", _master.ReturnDelivery.SaleCurrencyID, ExChange[0]);
            if (!!ex) {
                $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
                $(".cur-class").text(ex.CurName);
            }
            if (isValidArray(_master.ReturnDeliveryDetails)) {
                $listItemChoosed.clearHeaderDynamic(_master.ReturnDeliveryDetails[0].AddictionProps);
                $listItemChoosed.createHeaderDynamic(_master.ReturnDeliveryDetails[0].AddictionProps);
                $listItemChoosed.bindRows(_master.ReturnDeliveryDetails);
            }
            //setRequested(_master, key, detailKey, copyType);
        } else {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Return Delivery Not found!",
                close_button: "none"
            });
        }
    }
    function setDisabled() {
        $("#submit-item").prop("disabled", true);
        $("#item-id").prop("disabled", true);
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
    /// submit data ///
    $("#submit-item").on("click", function (e) {
        const item_master = master[0];
        item_master.SaleEmID = $("#sale-emid").val();
        freightMaster.FreightSaleDetails = freights.length === 0 ? new Array() : freights;
        item_master.WarehouseID = parseInt($("#ware-id").val());
        item_master.RequestedBy = $("#request_by").val();
        item_master.ShippedBy = $("#shipped_by").val();
        item_master.ReceivedBy = $("#received_by").val();
        item_master.RefNo = $("#ref-id").val();
        item_master.PostingDate = $("#post-date").val();
        item_master.DeliveryDate = $("#delivery-date").val();
        item_master.DocumentDate = $("#document-date").val();
        item_master.Remarks = $("#remark-id").val();
        item_master.ReturnDeliveryDetails = re_details.length === 0 ? new Array() : re_details;
        item_master.FreightSalesView = freightMaster;
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.SeriesDID = parseInt($("#SeriesDetailID").val());
        item_master.SeriesID = parseInt($("#invoice-no").val());
        item_master.DocTypeID = parseInt($("#DocumentTypeID").val());
        item_master.InvoiceNumber = parseInt($("#next_number").val());
        item_master.FreightAmount = $("#freight-value").val();
        const seriesID = parseInt($("#SeriesID").val());
        item_master.BranchID = parseInt($("#branch").val());
        const checkingSerialString = $("#checkingSerialString").val();
        const checkingBatchString = $("#checkingBatchString").val();
        //return;
        $("#loading").prop("hidden", false);
        $.ajax({
            url: "/Sale/CreateReturnDelivery",
            type: "post",
            data: $.antiForgeryToken(
                {
                    data: JSON.stringify(item_master),
                    type,
                    seriesJE: JSON.stringify(__singleElementJE),
                    seriesID,
                    serial: JSON.stringify(serials),
                    batch: JSON.stringify(batches),
                    checkingSerialString,
                    checkingBatchString,
                }),
            success: function (model) {
                if (model.IsSerail) {
                    $("#loading").prop("hidden", true);
                    const serial = SerialTemplate({
                        data: {
                            isReturnDelivery: true,
                            serials: model.Data,
                        }
                    });
                    serial.serialTemplate();
                    const seba = serial.callbackInfo();
                    serials = seba.serials;
                } else if (model.IsBatch) {
                    $("#loading").prop("hidden", true);
                    const batch = BatchNoTemplate({
                        data: {
                            isReturnDelivery: true,
                            batches: model.Data,
                        }
                    });
                    batch.batchTemplate();
                    const seba = batch.callbackInfo();
                    batches = seba.batches;
                } else if (model.IsSerialCopy) {
                    $("#loading").prop("hidden", true);
                    const serial = SerialTemplate({
                        data: {
                            isCopy: true,
                            isReturnDelivery: true,
                            serials: model.Data,
                        }
                    });
                    serial.serialTemplate();
                    const seba = serial.callbackInfo();
                    serials = seba.serials;
                } else if (model.IsBatchCopy) {
                    $("#loading").prop("hidden", true);
                    const batch = BatchNoTemplate({
                        data: {
                            isCopy: true,
                            isReturnDelivery: true,
                            batches: model.Data,
                        }
                    });
                    batch.batchTemplate();
                    const seba = batch.callbackInfo();
                    batches = seba.batches;
                } else if (model.Model.Action == 1) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        }
                    }, model.Model).refresh(1500);
                    $("#loading").prop("hidden", true);
                }
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, model.Model);
                $(window).scrollTop(0);
                $("#loading").prop("hidden", true);
            }
        });
    });
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
    function selectSeries(selected) {
        $.each(___RE.seriesRE, function (i, item) {
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
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue));
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
        _master.TotalAmountSys = total * _master.ExchangeRate;
        _master.TotalAmount = num.toNumberSpecial(_master.SubTotalAfterDis) + _master.VatValue + num.toNumberSpecial(freightMaster.AmountReven);
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
                    updatedetail(re_details, i.UomID, i.LineID, "FinDisValue", _thisvalue);
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
});