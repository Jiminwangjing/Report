"use strict"
let ardown_details = [],
    ExChange = [],
    master = [],
    _currency = "",
    _curencyID = 0,
    _priceList = 0,
    isCopied = false,
    dised = false,
    __singleElementJE,
    ___CD = JSON.parse($("#data-invoice").text()),
    disSetting = ___CD.genSetting.Display;
const itemArr = [".remark", ".uom", ".unitprice", ".disvalue", ".disrate", ".taxgroup"]
$(document).ready(function () {
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });
    $.each($("[data-date]"), function (i, t) {
        setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
    });
    var itemmaster = {
        ARDID: 0,
        CusID: 0,
        BranchID: 0,
        WarehouseID: 0,
        UserID: 0,
        SaleCurrencyID: 0,
        CompanyID: 0,
        DocTypeID: 0,
        SeriesID: 0,
        SeriesDID: 0,
        RequestedBy: "",
        ShippedBy: "",
        ReceivedBy: "",
        InvoiceNumber: "",
        RefNo: "",
        InvoiceNo: "",
        ExchangeRate: 0,
        PostingDate: "",
        DueDate: "",
        ValidUntilDate: "",
        DocumentDate: "",
        IncludeVat: false,
        Status: "",
        Remarks: "",
        TotalAfterDis: 0,
        TotalAfterDisSys: 0,
        Total: 0,
        SubTotal: 0,
        SubTotalSys: 0,
        DisRate: 0,
        DisValue: 0,
        DPMValue: 0,
        DPMRate: 0,
        TypeDis: "",
        TotalAmount: 0,
        TotalAmountSys: 0,
        CopyType: 0,
        CopyKey: "",
        BasedCopyKeys: "",
        ChangeLog: "",
        PriceListID: 0,
        LocalCurID: 0,
        LocalSetRate: 0,
        ARDownPaymentDetail: new Array
    }
    master.push(itemmaster);
    // select inoice with defualt == true
    ___CD.seriesCD.forEach(i => {
        if (i.Default == true) {
            $("#DocumentTypeID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    ___CD.seriesJE.forEach(i => {
        if (i.Default == true) {
            $("#JEID").val(i.ID);
            $("#JENumber").val(i.NextNo);
            __singleElementJE = findArray("ID", i.ID, ___CD.seriesJE);
        }
    });
    let selected = $("#invoice-no");
    selectSeries(selected);
    $('#invoice-no').change(function () {
        var id = ($(this).val());
        var seriesSQ = findArray("ID", id, ___CD.seriesCD);
        ___CD.seriesCD.Number = seriesSQ.NextNo;
        ___CD.seriesCD.ID = id;
        $("#DocumentID").val(seriesSQ.DocumentTypeID);
        $("#number").val(seriesSQ.NextNo);
        $("#next_number").val(seriesSQ.NextNo);
    });
    if (___CD.seriesCD.length == 0) {
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
                            return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s/g, "").match(rex)
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
        return value == null || value == undefined || value == "";
    }
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
    $("#cus-choosed").click(function () {
        $("#cus-id").val(_nameCus);
        $("#sale-emid").val(semid);
        $("#sale-em").val(semnale);
        master.forEach(i => {
            i.CusID = _idCus;
        })
        $("#barcode-reading").prop("disabled", false).focus();
        $("#copy-from").prop("disabled", false);
        $("#item-id").prop("disabled", false);
        $("#ModalCus").modal("hide");
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
        selector: $("#list-detailDonw"),
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
            "UoMs", "ItemNameKH", "DisValue", "DisRate", "Remarks", "FinDisRate", "FinDisValue", "TaxOfFinDisValue", "FinTotalValue",
            "TaxDownPaymentValue"
        ],
        columns: [
            {
                name: "UnitPrice",
                template: `<input class='form-control font-size disabledrowinput' type='text' />`,
                on: {
                    "keyup": function (e) {
                        dised = false;
                        $(this).asNumber();
                        updatedetail(ardown_details, e.data.UomID, e.key, "UnitPrice", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this.value);
                        setSummary();
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
                        dised = false;
                        $(this).asNumber();
                        calDisValue(e, this);
                        setSummary();
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "DisRate",
                template: `<input class='form-control font-size disabledrowinput' type='text' />`,
                on: {
                    "keyup": function (e) {
                        dised = false;
                        $(this).asNumber();
                        calDisRate(e, this);
                        setSummary();
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "Qty",
                template: `<input class='form-control font-size' type='text' />`,
                on: {
                    "keyup": function (e) {
                        dised = false;
                        $(this).asNumber();
                        if (isCopied && this.value > e.data.OpenQty) this.value = e.data.OpenQty;
                        updatedetail(ardown_details, e.data.UomID, e.key, "Qty", this.value);
                        updatedetail(ardown_details, e.data.UomID, e.key, "PrintQty", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this.value);
                        setSummary();
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "TaxGroupList",
                template: `<select class='form-control font-size disabledrowinput'></select>`,
                on: {
                    "change": function (e) {
                        dised = false;
                        const taxg = findArray("ID", this.value, e.data.TaxGroups);
                        if (!!taxg) {
                            updatedetail(ardown_details, e.data.UomID, e.key, "TaxRate", taxg.Rate);

                        } else {
                            updatedetail(ardown_details, e.data.UomID, e.key, "TaxRate", 0);
                        }
                        updatedetail(ardown_details, e.data.UomID, e.key, "TaxGroupID", parseInt(this.value))
                        totalRow($listItemChoosed, e, this.value);
                        setSummary();
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "UoMs",
                template: `<select class='form-control font-size disabledrowinput'></select>`,
                on: {
                    "change": function (e) {
                        dised = false;
                        updatedetail(ardown_details, e.data.UomID, e.key, "UomID", parseInt(this.value));
                        const uomList = findArray("UoMID", this.value, e.data.UomPriceLists);
                        const uom = findArray("ID", this.value, e.data.UoMsList);
                        if (!!uom && !!uomList) {
                            updatedetail(ardown_details, e.data.UomID, e.key, "UnitPrice", uomList.UnitPrice);
                            updatedetail(ardown_details, e.data.UomID, e.key, "UomName", uom.Name);
                            updatedetail(ardown_details, e.data.UomID, e.key, "Factor", uom.Factor);
                            $listItemChoosed.updateColumn(e.key, "UnitPrice", num.formatSpecial(uomList.UnitPrice, disSetting.Prices));
                            calDisvalue(e);
                            totalRow($listItemChoosed, e, this.value);
                            setSummary();
                            disInputUpdate(master[0]);
                        }
                    }
                }
            },
            {
                name: "Remarks",
                template: `<input>`,
                on: {
                    "keyup": function (e) {
                        updatedetail(ardown_details, e.data.UomID, e.key, "Remarks", this.value);
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
                template: `<i class="fas fa-trash"></i>`,
                on: {
                    "click": function (e) {
                        dised = false;
                        $listItemChoosed.removeRow(e.key);
                        ardown_details = ardown_details.filter(i => i.LineID !== e.key);
                        setSummary();
                        disInputUpdate(master[0]);
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
                    ardown_details = [];
                    $listItemChoosed.clearRows();
                    $listItemChoosed.bindRows(ardown_details);
                    setSummary(ardown_details);
                    disInputUpdate(master[0]);
                })
            }
        });
    })

    /// dis rate on invoice // cash
    $("#dis-rate-id").keyup(function () {
        $(this).asNumber();
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > 100) this.value = 100;
        const subtotal = num.toNumberSpecial($("#sub-id").val());
        const disvalue = (num.toNumberSpecial(this.value) * subtotal) === 0 ? 0 : (num.toNumberSpecial(this.value) * subtotal) / 100;
        $("#dis-value-id").val(num.toNumberSpecial(disvalue, disSetting.Prices));
        invoiceDisAllRowRate(this);
        const totalAfDis = subtotal - parseFloat(disvalue) + master[0].VatValue;
        $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
        master[0].TotalAmount = totalAfDis;
        master[0].DisValue = disvalue;
        master[0].DisRate = this.value;
        master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
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
        if (parseFloat(disrate) > 0) {
            const totalAfDis = subtotal - num.toNumberSpecial(this.value) + master[0].VatValue;
            $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
            master[0].TotalAmount = totalAfDis;
            master[0].DisValue = this.value;
            master[0].DisRate = disrate;
            master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
        } else {
            setSummary();
        }
    });

    // DPM //
    $("#dpm-rate-id").keyup(function () {
        dised = false;
        $(this).asNumber();
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > 100) this.value = 100;
        const subtotalafdis = num.toNumberSpecial($("#sub-after-dis").val());
        const dpmvalue = (num.toNumberSpecial(this.value) * subtotalafdis) === 0 ? 0 : num.toNumberSpecial(this.value) * subtotalafdis / 100;
        $("#dpm-value-id").val(num.toNumberSpecial(dpmvalue, disSetting.Prices));
        const totalAfDis = dpmvalue + master[0].VatValue;
        $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        let balanceDue = subtotalafdis - totalAfDis;

        $("#balanceDue").val(balanceDue);
        master[0].TotalAmount = totalAfDis;
        master[0].BalanceDue = balanceDue;
        master[0].DPMValue = dpmvalue;
        master[0].DPMRate = this.value;
        master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
        master[0].BalanceDueSys = balanceDue * master[0].ExchangeRate;
        setSummary();

    });
    $("#dpm-value-id").keyup(function () {
        dised = false;
        $(this).asNumber();
        const subtotalafdis = num.toNumberSpecial($("#sub-after-dis").val());
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > subtotalafdis) this.value = subtotalafdis;

        const dpmrate = (num.toNumberSpecial(this.value) * 100 === 0) || (subtotalafdis === 0) ? 0 :
            (num.toNumberSpecial(this.value) * 100) / subtotalafdis;
        $("#dpm-rate-id").val(dpmrate);
        const totalAfDis = num.toNumberSpecial(this.value) + master[0].VatValue;
        $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        let balanceDue = subtotalafdis - totalAfDis;
        $("#balanceDue").val(balanceDue);
        master[0].TotalAmount = totalAfDis;
        master[0].BalanceDue = balanceDue;
        master[0].DPMValue = this.value;
        master[0].DPMRate = dpmrate;
        master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
        master[0].BalanceDueSys = balanceDue * master[0].ExchangeRate;
        setSummary();

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
                            if (isValidArray(ardown_details)) {
                                res.forEach(i => {
                                    const isExisted = ardown_details.some(qd => qd.ItemID === i.ItemID);
                                    if (isExisted) {
                                        const item = ardown_details.filter(_i => _i.BarCode === i.BarCode)[0];
                                        if (!!item) {
                                            const qty = parseFloat(item.Qty) + 1;
                                            const openqty = parseFloat(item.Qty) + 1;
                                            updatedetail(ardown_details, i.UomID, item.LineID, "Qty", qty);
                                            updatedetail(ardown_details, i.UomID, item.LineID, "OpenQty", openqty);
                                            $listItemChoosed.updateColumn(item.LineID, "Qty", qty);
                                            const _e = { key: item.LineID, data: item };
                                            calDisvalue(_e);
                                            totalRow($listItemChoosed, _e, qty);
                                            //setSummary(ardown_details);
                                        }
                                    } else {
                                        ardown_details.push(i);
                                        $listItemChoosed.addRow(i);
                                    }
                                })
                            } else {
                                $listItemChoosed.clearHeaderDynamic(res[0].AddictionProps)
                                $listItemChoosed.createHeaderDynamic(res[0].AddictionProps)
                                res.forEach(i => {
                                    ardown_details.push(i);
                                    $listItemChoosed.addRow(i);
                                })

                            }
                            setItemToAbled(itemArr);
                            setSummary();
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
            $("#submit-item").prop("disabled", true);
            $("#copy-from").prop("disabled", true);
            if ($("#next_number").val() == "*") {
                initModalDialog("/Sale/GetSaleARDownDisplay", "/Sale/FindSaleARDown", "Fine Sale ARDownPayment", "ARDID", "ARDownPayment", "SaleARDownDetails", 369);
            }
            else {
                $.ajax({
                    url: "/Sale/FindSaleARDown",
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
                                    ardown_details.push(res[0]);
                                    setItemToAbled(itemArr);
                                    $listItemChoosed.addRow(res[0])
                                    setSummary();
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
                break;
            case "2":
                initModalDialog("/Sale/GetSaleOrders", "/Sale/GetSaleOrderDetailCopy", "Sale Orders (Copy)", "SOID", "SaleOrder", "SaleOrderDetails", 2);
                break;
            case "3":
                initModalDialog("/Sale/GetSaleDeliveries", "/Sale/GetSaleDeliveryDetailCopy", "Sale Delivery (Copy)", "SDID", "SaleDelivery", "SaleDeliveryDetails", 3);
                break;

        }
        $(this).val(0);
    });

    /// submit data ///
    $("#submit-item").on("click", function (e) {
        const item_master = master[0];
        item_master.RequestedBy = $("#request_by").val();
        item_master.ShippedBy = $("#shipped_by").val();
        item_master.ReceivedBy = $("#received_by").val();
        item_master.WarehouseID = parseInt($("#ware-id").val());
        item_master.RefNo = $("#ref-id").val();
        item_master.PostingDate = $("#post-date").val();
        item_master.DueDate = $("#due-date").val();
        item_master.SaleEmID = $("#sale-emid").val();
        item_master.DeliveryDate = $("#due-date").val();
        item_master.DocumentDate = $("#document-date").val();
        item_master.Remarks = $("#remark-id").val();
        item_master.ARDownPaymentDetails = ardown_details.length === 0 ? new Array() : ardown_details;
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.SeriesDID = parseInt($("#SeriesDetailID").val());
        item_master.SeriesID = parseInt($("#invoice-no").val());
        item_master.DocTypeID = parseInt($("#DocumentTypeID").val());
        item_master.InvoiceNumber = parseInt($("#next_number").val());
        item_master.BranchID = parseInt($("#branch").val());
        $("#loading").prop("hidden", false);
        $.ajax({
            url: "/Sale/UpdateARDonw",
            type: "post",
            data: $.antiForgeryToken({ data: JSON.stringify(item_master), seriesJE: JSON.stringify(__singleElementJE) }),
            success: function (model) {
                if (model.Action == 1) {
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
                if (res.length > 0) {
                    $list_sem.bindRows(res);
                    $("#txtsearchsaleem").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s/g, "");
                        let items = $.grep(res, function (item) {
                            return item.Barcode === __value || item.KhmerName.toLowerCase().replace(/\s/g, "").includes(__value)
                                || item.UnitPrice == __value || item.Code.toLowerCase().replace(/\s/g, "").includes(__value)
                                || item.UoM.toLowerCase().replace(/\s/g, "").includes(__value);
                        });
                        itemMaster.bindRows(items);
                    });
                }
            });
        });
        dialogprojmana.reject(function () {
            dialogprojmana.shutdown();
        });
    })

    function getSaleItemMasters(_master) {
        if (!!_master) {
            $.get("/Sale/GetCustomer", { id: _master.ARDownPayment.CusID }, function (cus) {
                $("#cus-id").val(cus.Name);
            });
            master[0] = _master.ARDownPayment;
            ardown_details = _master.SaleARDownDetails;
            $("#request_by").prop("disabled", true);
            $("#shipped_by").prop("disabled", true);
            $("#received_by").prop("disabled", true);

            $("#request_name").val(_master.ARDownPayment.RequestedByName);
            $("#shipped_name").val(_master.ARDownPayment.ShippedByName);
            $("#received_name").val(_master.ARDownPayment.ReceivedByName);
            $("#branch").val(_master.ARDownPayment.BranchID);
            $("#next_number").val(_master.ARDownPayment.InvoiceNumber);
            $("#request_by").val(_master.ARDownPayment.RequestedByID);
            $("#shipped_by").val(_master.ARDownPayment.ShippedByID);
            $("#received_by").val(_master.ARDownPayment.ReceivedByID);
            $("#ware-id").val(_master.ARDownPayment.WarehouseID);
            $("#ref-id").val(_master.ARDownPayment.RefNo);
            $("#cur-id").val(_master.ARDownPayment.SaleCurrencyID);
            $("#sta-id").val(_master.ARDownPayment.Status);
            $("#sub-id").val(num.formatSpecial(_master.ARDownPayment.SubTotal, disSetting.Prices));
            $("#sub-dis-id").val(_master.ARDownPayment.TypeDis);
            $("#sale-emid").val(_master.ARDownPayment.SaleEmID);
            $("#sale-em").val(_master.ARDownPayment.SaleEmName);
            $("#sub-after-dis").val(num.formatSpecial(_master.ARDownPayment.SubTotalAfterDis, disSetting.Prices));
            $("#freight-value").val(num.formatSpecial(_master.ARDownPayment.FreightAmount, disSetting.Prices));
            $("#dpm-value-id").val(num.formatSpecial(_master.ARDownPayment.DPMValue, disSetting.Prices));
            $("#dpm-rate-id").val(num.formatSpecial(_master.ARDownPayment.DPMRate, disSetting.Rates));
            $("#applied-amount").val(num.formatSpecial(_master.ARDownPayment.AppliedAmount, disSetting.Prices));
            $("#balanceDue").val(num.formatSpecial(_master.ARDownPayment.BalanceDue, disSetting.Prices));
            //$("#sub-dis-id").val(_master.SaleQuote.TypeDis);
            $("#dis-rate-id").val(num.formatSpecial(_master.ARDownPayment.DisRate, disSetting.Rates));
            $("#dis-value-id").val(num.formatSpecial(_master.ARDownPayment.DisValue, disSetting.Prices));
            setDate("#post-date", _master.ARDownPayment.PostingDate.toString().split("T")[0]);
            setDate("#due-date", _master.ARDownPayment.DeliveryDate.toString().split("T")[0]);
            setDate("#document-date", _master.ARDownPayment.DocumentDate.toString().split("T")[0]);
            $("#remark-id").val(_master.ARDownPayment.Remarks);
            $("#total-id").val(num.formatSpecial(_master.ARDownPayment.TotalAmount, disSetting.Prices));
            $("#vat-value").val(num.formatSpecial(_master.ARDownPayment.VatValue, disSetting.Prices));
            $("#item-id").prop("disabled", false);
            var ex = findArray("CurID", _master.ARDownPayment.SaleCurrencyID, ExChange[0]);
            if (!!ex) {
                $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
                $(".cur-class").text(ex.CurName);
            }
            if (isValidArray(_master.SaleARDownDetails)) {
                $listItemChoosed.clearHeaderDynamic(_master.SaleARDownDetails[0].AddictionProps);
                $listItemChoosed.createHeaderDynamic(_master.SaleARDownDetails[0].AddictionProps);
                $listItemChoosed.bindRows(_master.SaleARDownDetails);
            }
            //setRequested(_master, key, detailKey, copyType);
        } else {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Sale Quotation Not found!",
                close_button: "none"
            });
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
                                    $("#sale-emid").val(res[0].SaleEmID);
                                    $("#sale-em").val(res[0].SaleEmName);
                                    $.get(urlDetail, { number: e.data.InvoiceNo, seriesId: e.data.SeriesID }, function (res) {
                                        if (type == 369) {
                                            getSaleItemMasters(res);
                                        }
                                        else {
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
        ardown_details = _master[keyDetail];
        $("#branch").val(_master[keyMaster].BranchID); 
        $("#request_name").val(_master[keyMaster].RequestedByName);
        $("#shipped_name").val(_master[keyMaster].ShippedByName);
        $("#received_name").val(_master[keyMaster].ReceivedByName);

        $("#request_by").val(_master[keyMaster].RequestedByID);
        $("#shipped_by").val(_master[keyMaster].ShippedByID);
        $("#received_by").val(_master[keyMaster].ReceivedByID);
        $("#ware-id").val(_master[keyMaster].WarehouseID);
        $("#ref-id").val(_master[keyMaster].RefNo);
        $("#cur-id").val(_master[keyMaster].SaleCurrencyID);
        $("#sta-id").val(_master[keyMaster].Status);
        $("#sub-id").val(num.formatSpecial(_master[keyMaster].SubTotal, disSetting.Prices));
        $("#sub-dis-id").val(_master[keyMaster].TypeDis);
        $("#sub-after-dis").val(num.formatSpecial(_master[keyMaster].SubTotalAfterDis, disSetting.Prices));
        $("#seriesID").val(_master[keyMaster].SeriesID);
        //$("#sub-dis-id").val(_master.SaleQuote.TypeDis);
        $("#dis-rate-id").val(num.formatSpecial(_master[keyMaster].DisRate, disSetting.Rates));
        $("#dis-value-id").val(num.formatSpecial(_master[keyMaster].DisValue, disSetting.Prices));
        $("#remark-id").val(_master[keyMaster].Remarks);
        //$("#total-id").val(num.formatSpecial(_master[keyMaster].TotalAmount, disSetting.Prices));
        //$("#vat-value").val(num.formatSpecial(_master[keyMaster].VatValue, disSetting.Prices));
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
        master[0].BasedCopyKeys = _master[keyMaster].InvoiceNo;
        setSourceCopy(master[0].BasedCopyKeys)

        if (isValidArray(_master[keyDetail])) {
            $listItemChoosed.clearHeaderDynamic(_master[keyDetail][0].AddictionProps);
            $listItemChoosed.createHeaderDynamic(_master[keyDetail][0].AddictionProps);
            $listItemChoosed.bindRows(_master[keyDetail]);
        }
        //if (copyType == 3) {
        //    setDisabled();
        //    isCopied = !isCopied;
        //} else {
        //    isCopied = false;
        //    setItemToAbled(itemArr);
        //}
    }
    function setSourceCopy(basedCopyKeys) {
        // let copyKeys = new Array();
        // if (basedCopyKeys) {
        //     copyKeys = basedCopyKeys.split("/");
        // }
        // var copyInfo = "";
        // $.each(copyKeys, function (i, key) {
        //     if (key.startsWith("SQ")) {
        //         copyInfo += "Sale Quotation: " + key;
        //     }

        //     if (key.startsWith("SO")) {
        //         copyInfo += "/Sale Order: " + key;
        //     }

        //     if (key.startsWith("DN")) {
        //         copyInfo += "/Sale Delivery: " + key;
        //     }
        // });
        master[0].BasedCopyKeys = basedCopyKeys;
        $("#source-copy").val(basedCopyKeys);
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
        $.each(___CD.seriesCD, function (i, item) {
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
        var __date = $(selector);
        __date[0].valueAsDate = new Date(date_value);
        __date[0].setAttribute(
            "data-date",
            moment(__date[0].value)
                .format(__date[0].getAttribute("data-date-format"))
        );
    }
    function totalRow(table, e, _this) {
        if (_this === '' || _this === '-') _this = 0;
        const totalWDis = num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice) - num.toNumberSpecial(e.data.DisValue);
        let vatValue = num.toNumberSpecial(e.data.TaxRate) * totalWDis === 0 ? 0 :
            num.toNumberSpecial(e.data.TaxRate) * totalWDis / 100;

        let totalrow = num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice) - num.toNumberSpecial(e.data.DisValue) + vatValue;

        // Update Object
        updatedetail(ardown_details, e.data.UomID, e.key, "TotalWTax", isNaN(totalrow) ? 0 : totalrow);
        updatedetail(ardown_details, e.data.UomID, e.key, "Total", isNaN(totalrow - vatValue) ? 0 : totalrow - vatValue);

        let subtotal = 0;
        let disRate = num.toNumberSpecial($("#dis-rate-id").val());
        ardown_details.forEach(i => {
            subtotal += num.toNumberSpecial(i.Total) - num.toNumberSpecial(i.DisValue);
        });
        const disValue = (disRate * subtotal) === 0 ? 0 : (disRate * subtotal) / 100;
        const taxFinDisValue = disRate === 0 ? vatValue : disRate * (totalWDis - disValue) === 0 ? 0 :
            disRate * (totalWDis - disValue) / 100;
        const finalTotal = num.toNumberSpecial(e.data.Total) - disValue;
        updatedetail(ardown_details, e.data.UomID, e.key, "FinTotalValue", finalTotal);
        updatedetail(ardown_details, e.data.UomID, e.key, "TaxOfFinDisValue", taxFinDisValue);

        //Update View
        table.updateColumn(e.key, "TaxOfFinDisValue", num.formatSpecial(isNaN(taxFinDisValue) ? 0 : taxFinDisValue, disSetting.Prices));
        table.updateColumn(e.key, "Total", num.formatSpecial(isNaN(totalrow - vatValue) ? 0 : totalrow - vatValue, disSetting.Prices));
        table.updateColumn(e.key, "TotalWTax", num.formatSpecial(isNaN(totalrow) ? 0 : totalrow, disSetting.Prices));
        table.updateColumn(e.key, "TaxValue", num.formatSpecial(isNaN(vatValue) ? 0 : vatValue, disSetting.Prices));
        table.updateColumn(e.key, "TaxRate", num.formatSpecial(isNaN(e.data.TaxRate) ? 0 : e.data.TaxRate, disSetting.Rate));
        table.updateColumn(e.key, "FinTotalValue", num.formatSpecial(isNaN(e.data.FinTotalValue) ? 0 : e.data.FinTotalValue, disSetting.Prices));
    }
    function calDisRate(e, _this) {
        if (_this.value > 100) _this.value = 100;
        if (_this.value < 0) _this.value = 0;
        if (num.toNumberSpecial(_this.value) > 0) {
            if (_this.value.toString().includes(".")) {
                _this.value = num.toNumberSpecial(_this.value);
            }
        }
        updatedetail(ardown_details, e.data.UomID, e.key, "DisRate", num.toNumberSpecial(_this.value));
        const disvalue = num.toNumberSpecial(_this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0 ? 0 :
            num.toNumberSpecial(_this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) / 100;

        updatedetail(ardown_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(disvalue, disSetting.Prices));
        totalRow($listItemChoosed, e, _this.value);
    }
    function calDisValue(e, _this) {
        if (_this.value > e.data.Qty * e.data.UnitPrice) _this.value = e.data.Qty * e.data.UnitPrice;
        if (_this.value == '' || _this.value < 0) _this.value = 0;
        if (num.toNumberSpecial(_this.value) > 0) {
            if (_this.value.toString().includes(".")) {
                _this.value = num.toNumberSpecial(_this.value);
            }
        }

        const value = num.toNumberSpecial(_this.value);
        updatedetail(ardown_details, e.data.UomID, e.key, "DisValue", value);
        const ratedis = (value * 100 === 0) || (num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice) === 0) ? 0 :
            (value * 100) / (num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice));
        updatedetail(ardown_details, e.data.UomID, e.key, "DisRate", ratedis);
        $listItemChoosed.updateColumn(e.key, "DisRate", num.formatSpecial(ratedis, disSetting.Rates));
        totalRow($listItemChoosed, e, _this.value);
    }
    function calDisvalue(e) {
        let disvalue = num.toNumberSpecial(e.data.DisRate) * num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice) === 0 ? 0 :
            (num.toNumberSpecial(e.data.DisRate) * num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice)) / 100;
        if (disvalue > num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice)) disvalue = num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice);
        updatedetail(ardown_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
    }
    function setSummary() {
        let subtotal = 0;
        let total = 0;
        let vat = 0;
        const _master = master[0];
        let disRate = $("#dis-rate-id").val();
        ardown_details.forEach(i => {
            if (!dised) {
                const lastDisval = num.toNumberSpecial(i.Total) - (num.toNumberSpecial(_master.DisRate) * num.toNumberSpecial(i.Total) / 100);
                const _value = num.toNumberSpecial(_master.DPMRate) * lastDisval === 0 ? 0 :
                    num.toNumberSpecial(_master.DPMRate) * lastDisval / 100;
                const txValue = num.toNumberSpecial(i.TaxRate) * _value === 0 ? 0 : num.toNumberSpecial(i.TaxRate) * _value / 100;
                updatedetail(ardown_details, i.UomID, i.LineID, "TaxDownPaymentValue", txValue);
                $listItemChoosed.updateColumn(i.LineID, "TaxDownPaymentValue", num.formatSpecial(txValue, disSetting.Prices));
            }
            subtotal += num.toNumberSpecial(i.Total);
            total += num.toNumberSpecial(i.FinTotalValue);
            vat += num.toNumberSpecial(i.TaxDownPaymentValue);
        });
        const disValue = (num.toNumberSpecial(disRate) * subtotal) === 0 ? 0 : (num.toNumberSpecial(disRate) * subtotal) / 100;
        _master.SubTotalSys = subtotal * _master.ExchangeRate;
        _master.SubTotal = subtotal;
        _master.VatRate = (vat * 100) === 0 ? 0 : vat * 100 / total;
        _master.VatValue = vat;
        _master.DisValue = disValue;
        _master.SubTotalAfterDis = subtotal - disValue;
        _master.SubTotalBefDis = subtotal;
        _master.DisRate = disRate;
        _master.DPMValue = _master.SubTotalAfterDis * _master.DPMRate === 0 ? 0 : _master.SubTotalAfterDis * _master.DPMRate / 100;
        _master.TotalAmount = total * _master.DPMRate === 0 ? _master.VatValue : (total * _master.DPMRate / 100) + _master.VatValue;
        _master.TotalAmountSys = _master.TotalAmount * _master.ExchangeRate;
        _master.BalanceDue = _master.SubTotalAfterDis - _master.TotalAmount;//_master.TotalAmount;
        _master.BalanceDueSys = _master.BalanceDue * _master.ExchangeRate;
        ardown_details.forEach(i => {
            const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_master.DisRate) === 0 ? 0 :
                num.toNumberSpecial(i.Total) * num.toNumberSpecial(_master.DisRate) / 100;

            $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
            $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_master.DisRate, disSetting.Prices));

            updatedetail(ardown_details, i.UomID, i.LineID, "FinDisValue", value);
            updatedetail(ardown_details, i.UomID, i.LineID, "FinDisRate", _master.DisRate);

            const lastDisval = num.toNumberSpecial(i.Total) - (num.toNumberSpecial(_master.DisRate) * num.toNumberSpecial(i.Total) / 100);
            const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                num.toNumberSpecial(i.TaxRate) * lastDisval / 100;

            updatedetail(ardown_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
            updatedetail(ardown_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);

            $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
            $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
        });
        $("#vat-value").val(num.formatSpecial(vat, disSetting.Prices));
        $("#dpm-value-id").val(num.formatSpecial(_master.DPMValue, disSetting.Prices));
        $("#sub-id").val(num.formatSpecial(subtotal, disSetting.Prices));
        $("#total-id").val(num.formatSpecial(_master.TotalAmount, disSetting.Prices));
        $("#sub-after-dis").val(num.formatSpecial(_master.SubTotalAfterDis, disSetting.Prices));
        $("#balanceDue").val(num.formatSpecial(_master.BalanceDue, disSetting.Prices));
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
    function invoiceDisAllRowRate(_this) {
        dised = true;
        if (isValidArray(ardown_details)) {
            ardown_details.forEach(i => {
                const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) / 100;
                if (_this.value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(value);
                    const taxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;

                    updatedetail(ardown_details, i.UomID, i.LineID, "TaxOfFinDisValue", taxOfFinDisValue);
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(taxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));

                    const _value = num.toNumberSpecial(master[0].DPMRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(master[0].DPMRate) * lastDisval / 100;
                    const txValue = num.toNumberSpecial(i.TaxRate) * _value === 0 ? 0 : num.toNumberSpecial(i.TaxRate) * _value / 100;
                    updatedetail(ardown_details, i.UomID, i.LineID, "TaxDownPaymentValue", txValue);
                    $listItemChoosed.updateColumn(i.LineID, "TaxDownPaymentValue", num.formatSpecial(txValue, disSetting.Prices));
                } else if (_this.value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    updatedetail(ardown_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));

                    updatedetail(ardown_details, i.UomID, i.LineID, "TaxDownPaymentValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxDownPaymentValue", num.formatSpecial(0, disSetting.Prices));
                }
            });
            setSummary();
        }
    }
    function invoiceDisAllRowValue(value) {
        dised = true;
        if (isValidArray(ardown_details)) {
            ardown_details.forEach(i => {
                const __value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) / 100;
                if (value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(__value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinDisValue", __value);
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinDisRate", value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(__value);
                    const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
                    updatedetail(ardown_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                    const _value = num.toNumberSpecial(master[0].DPMRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(master[0].DPMRate) * lastDisval / 100;

                    const txValue = num.toNumberSpecial(i.TaxRate) * _value === 0 ? 0 : num.toNumberSpecial(i.TaxRate) * _value / 100;
                    updatedetail(ardown_details, i.UomID, i.LineID, "TaxDownPaymentValue", txValue);
                    $listItemChoosed.updateColumn(i.LineID, "TaxDownPaymentValue", num.formatSpecial(txValue, disSetting.Prices));
                }
                else if (value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinDisValue", _thisvalue);
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinDisRate", value);

                    updatedetail(ardown_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(ardown_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));

                    updatedetail(ardown_details, i.UomID, i.LineID, "TaxDownPaymentValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxDownPaymentValue", num.formatSpecial(0, disSetting.Prices));
                }
            });
            setSummary();
        }

    }
});