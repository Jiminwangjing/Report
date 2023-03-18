const ___branchID = parseInt($("#BranchID").text()),
    ___userID = parseInt($("#UserID").text()),
    __AllData = JSON.parse($("#data-invoice").text()),
    ___PU = __AllData.seriesPU,
    ___JE = __AllData.seriesJE;
let __singleElementJE,
    _currency = "",
    _currencyID = 0,
    itemMaster = {},
    itemMasters = [],
    apDetail = [],
    ExChange = [],
    freights = [],
    serials = [],
    batches = [],
    serialsCancel = [],
    batchesCancel = [],
    disSetting = __AllData.genSetting.Display,
    freightMaster = {},
    params = {
        purchaseId: 0
    };
$(document).ready(function () {
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });
    itemMaster = {
        PurchaseAPID: 0,
        VendorID: 0,
        DraftID: 0,
        BranchID: 0,
        PurCurrencyID: 0,
        SysCurrencyID: 0,
        WarehouseID: 0,
        UserID: 0,
        DocumentTypeID: 0,
        SeriesID: 0,
        SeriesDetailID: 0,
        CompanyID: 0,
        Number: "",
        ReffNo: "",
        InvoiceNo: "",
        PurRate: 0,
        BalanceDueSys: 0,
        SubTotal: "",
        SubTotalsys: "",
        SubTotalAfterDis: "",
        SubTotalAfterDisSys: "",
        DiscountRate: "",
        DiscountValue: "",
        TypeDis: "",
        TaxRate: 0,
        TaxValue: 0,
        DownPayment: 0,
        DownPaymentSys: 0,
        AppliedAmount: 0,
        AppliedAmountSys: 0,
        ReturnAmount: 0,
        BalanceDue: 0,
        AdditionalExpense: 0,
        AdditionalNote: "",
        Remark: "",
        Status: "",
        LocalSetRate: 0,
        LocalCurID: 0,
        BaseOnID: 0,
        DraftDetails: {}
    };
    var d = new Date();
    document.getElementById("txtPostingdate").valueAsDate = d;
    document.getElementById("txtDuedate").valueAsDate = d;
    document.getElementById("txtDocumentDate").valueAsDate = d;
    $("#addfontawesome").addClass("fa-percent");
    $(".findbtn").html($(".btnfind").text());
    $(".btnfind_new").text("find");
    $("#txtstatus").val("open");

    ___PU.forEach(i => {
        if (i.Default == true) {
            $("#DocumentTypeID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    ___JE.forEach(i => {
        if (i.Default == true) {
            $("#JEID").val(i.ID);
            $("#JENumber").val(i.NextNo);
            __singleElementJE = find("ID", i.ID, ___JE);
        }
    });

    //invioce
    let selected = $("#txtInvoice");
    selectSeries(selected);

    $('#txtInvoice').change(function () {
        var id = ($(this).val());
        var seriesPU = find("ID", id, ___PU);
        ___PU.Number = seriesPU.NextNo;
        ___PU.ID = id;
        $("#DocumentID").val(seriesPU.DocumentTypeID);
        $("#number").val(seriesPU.NextNo);
        $("#next_number").val(seriesPU.NextNo);
    });
    if (___PU.length == 0) {
        $('#txtInvoice').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
        $(".btn_ADD").prop("disabled", true);
    }
    //Get Business Partner
    $("#vendor-id").click(function () {
        $(this).prop("disabled", false);
        $("#ModalCus").modal("show");
        $.ajax({
            url: "/PurchaseAP/GetBusinessPartners",
            type: "Get",
            dataType: "Json",
            success: function (response) {
                bindVendor(response);
            }
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
    //GetWarehouse
    $.ajax({
        url: "/PurchaseAP/GetWarehouses",
        type: "Get",
        dataType: "Json",
        data: {
            ID: ___branchID
        },
        success: function (response) {

            var data = "";
            $.each(response, function (i, item) {
                data +=
                    '<option value="' + item.ID + '">' + item.Name + '</option>';
            });
            $("#txtwarehouse").append(data).on("change", changeWarehouse);
        }
    });
    //get currency
    $.ajax({
        url: "/PurchaseAP/GetCurrencyDefualt",
        type: "GET",
        dataType: "JSON",
        success: function (res) {
            const e = res[0];
            $(".cur-class").text(e.Description);
            _currency = e.Description
            _currencyID = e.ID;
            itemMaster.ExchangeRate = e.ExchangeRate;
            $("#txtExchange").val(1 + " " + _currency + " = " + 1 + " " + e.Description);
        }
    });

    $("#txtcurrency").change(function () {
        $.ajax({
            url: "/PurchaseAP/GetFilterLocaCurrency",
            type: "Get",
            dataType: "Json",
            data: { CurrencyID: this.value },
            success: function (res) {
                $.get("/PurchaseOrder/GetDisplayFormatCurrency", { curId: res[0].CurrencyID }, function (resp) {
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
            "Code", "PurchasPrice", "Barcode", "Qty", "TaxGroupSelect", "TaxRate", "TaxValue", "TotalWTax", "Total", "CurrencyName",
            "UoMSelect", "ItemName", "DiscountValue", "DiscountRate", "FinDisRate", "FinDisValue", "TaxOfFinDisValue", "FinTotalValue", "Remark"
        ],
        columns: [
            {
                name: "PurchasPrice",
                template: `<input class='form-control font-size unitprice' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "PurchasPrice", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this.value);
                        setSummary(apDetail);
                        disInputUpdate(itemMaster);
                    }
                }
            },
            {
                name: "CurrencyName",
                template: `<span class='font-size cur'></span`,
            },
            {
                name: "DiscountValue",
                template: `<input class='form-control font-size disvalue' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        calDisValue(e, this);
                        totalRow($listItemChoosed, e, this.value);
                        setSummary(apDetail);
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
                        totalRow($listItemChoosed, e, this.value);
                        setSummary(apDetail);
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

                        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "Qty", this.value);
                        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "OpenQty", this.value);
                        //calDisrate(e);
                        calDisvalue(e);

                        totalRow($listItemChoosed, e, this.value);
                        setSummary(apDetail);
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
                            updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxRate", taxg.Rate);

                        } else {
                            updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxRate", 0);
                        }
                        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxGroupID", parseInt(this.value))

                        totalRow($listItemChoosed, e, this.value);
                        setSummary(apDetail);
                        disInputUpdate(itemMaster);

                    }
                }
            },
            {
                name: "UoMSelect",
                template: `<select class='form-control font-size uom'></select>`,
                on: {
                    "change": function (e) {
                        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "UomID", this.value);
                    }
                }
            },
            {
                name: "Remark",
                template: `<input class='form-control font-size remark' />`,
                on: {
                    "change": function (e) {
                        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "Remark", this.value);
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
                        apDetail = apDetail.filter(i => i.LineIDUN !== e.key);
                        setSummary(apDetail);
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
            setSummary(apDetail);
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
        if (this.value == '' || this.value < 0)
            this.value = 0;
        if (this.value > 100) this.value = 100;


        if (!isNaN(this.value)) {
            const subtotal = num.toNumberSpecial($("#sub-id").val());
            const disvalue = (num.toNumberSpecial(this.value) * subtotal) === 0 ? 0 : (num.toNumberSpecial(this.value) * subtotal) / 100;
            $("#dis-value-id").val(num.toNumberSpecial(disvalue, disSetting.Prices));
            invoiceDisAllRowRate(this);
        }

        // if (parseFloat(disvalue) > 0) {
        //     const totalAfDis = subtotal - num.toNumberSpecial(disvalue) + num.toNumberSpecial(itemMaster.TaxValue)+  num.toNumberSpecial(freightMaster.ExpenceAmount);
        //     $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        //     $("#sub-after-dis").val(num.formatSpecial(totalAfDis - num.toNumberSpecial(itemMaster.TaxValue), disSetting.Prices));
        //     itemMaster.BalanceDue = totalAfDis;
        //     itemMaster.DiscountValue = disvalue;
        //     itemMaster.DiscountRate = this.value;
        //     itemMaster.BalanceDueSys = totalAfDis * itemMaster.ExchangeRate;
        // } else {
        //     setSummary(apDetail);
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
        //     const totalAfDis = subtotal - num.toNumberSpecial(this.value) + num.toNumberSpecial(itemMaster.TaxValue)+ num.toNumberSpecial(freightMaster.ExpenceAmount);
        //     $("#sub-after-dis").val(num.formatSpecial(totalAfDis - num.toNumberSpecial(itemMaster.TaxValue), disSetting.Prices));
        //     $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
        //     itemMaster.BalanceDue = totalAfDis;
        //     itemMaster.DiscountValue = this.value;
        //     itemMaster.DiscountRate = disrate;
        //     itemMaster.BalanceDueSys = totalAfDis * itemMaster.ExchangeRate;
        // } else {
        //     setSummary(apDetail);
        // }
    });
    $("#save-draft").click(function () {
        freightMaster.FreightPurchaseDetials = freights.length === 0 ? new Array() : freights;
        itemMaster.WarehouseID = parseInt($("#txtwarehouse").val());
        itemMaster.ReffNo = $("#txtreff_no").val();
        itemMaster.PostingDate = $("#txtPostingdate").val();
        itemMaster.DueDate = $("#txtDuedate").val();
        itemMaster.DocumentDate = $("#txtDocumentDate").val();
        itemMaster.Remark = $("#txtRemark").val();

        //itemMaster.draft_Details = apDetail.length === 0 ? new Array() : apDetail;
        itemMaster.DraftDetails = apDetail.length === 0 ? new Array() : apDetail;
        itemMaster.FreightPurchaseView = freightMaster;
        itemMaster.SeriesDetailID = parseInt($("#SeriesDetailID").val());
        itemMaster.SeriesID = parseInt($("#txtInvoice").val());
        itemMaster.DocumentTypeID = parseInt($("#DocumentTypeID").val());
        itemMaster.Number = $("#next_number").val();
        itemMaster.FrieghtAmount = $("#freight-value").val();
        itemMaster.FrieghtAmountSys = num.toNumberSpecial(itemMaster.FrieghtAmount) * num.toNumberSpecial(itemMaster.ExchangeRate);
        itemMaster.PurRate = itemMaster.ExchangeRate;
        itemMaster.BranchID = parseInt($("#branch").val());
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
        dialogsave.confirm(function (e) {
            itemMaster.DraftName = $("#draftname").val();
            $("#loading").prop("hidden", false);
            $.ajax({
                url: "/PurchaseAP/SaveDraft",
                type: "POST",
                dataType: "JSON",
                data: {
                    draft: JSON.stringify(itemMaster),
                },
                success: function (response) {

                    successRes(response);
                }
            });
            this.meta.shutdown();
        });
    });
    $("#submit-data").click(function () {
        freightMaster.FreightPurchaseDetials = freights.length === 0 ? new Array() : freights;
        itemMaster.WarehouseID = parseInt($("#txtwarehouse").val());
        itemMaster.ReffNo = $("#txtreff_no").val();
        itemMaster.PostingDate = $("#txtPostingdate").val();
        itemMaster.DueDate = $("#txtDuedate").val();
        itemMaster.DocumentDate = $("#txtDocumentDate").val();
        itemMaster.Remark = $("#txtRemark").val();
        itemMaster.PurchaseAPDetails = apDetail.length === 0 ? new Array() : apDetail;
        itemMaster.FreightPurchaseView = freightMaster;
        itemMaster.SeriesDetailID = parseInt($("#SeriesDetailID").val());
        itemMaster.SeriesID = parseInt($("#txtInvoice").val());
        itemMaster.DocumentTypeID = parseInt($("#DocumentTypeID").val());
        itemMaster.Number = $("#next_number").val();
        itemMaster.FrieghtAmount = $("#freight-value").val();
        itemMaster.FrieghtAmountSys = num.toNumberSpecial(itemMaster.FrieghtAmount) * num.toNumberSpecial(itemMaster.ExchangeRate);
        itemMaster.PurRate = itemMaster.ExchangeRate;
        itemMaster.BranchID = parseInt($("#branch").val());

        const type = $("#txttype").text();
        var dialogSubmit = new DialogBox({
            content: "Are you sure you want to save the item?",
            type: "yes-no",
            icon: "warning"
        });
        dialogSubmit.confirm(function () {

            $("#loading").prop("hidden", false);
            $.ajax({
                url: "/PurchaseAP/SavePurchaseAP",
                type: "POST",
                dataType: "JSON",
                data: {
                    purchase: JSON.stringify(itemMaster),
                    Type: type,
                    je: JSON.stringify(__singleElementJE),
                    serials: JSON.stringify(serials),
                    batches: JSON.stringify(batches),
                },
                success: function (response) {

                    $listItemChoosed.disableColumns(undefined, ["Qty"]);
                    if (response.Action != -1) {
                        if (itemMaster.DraftID != 0) {
                            $.post("/PurchaseAP/RemoveDraft", { id: itemMaster.DraftID })
                        };
                    }

                    successRes(response);


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
                url: "/PurchaseAP/FindPurchaseAP",
                data: { number: $("#next_number").val().trim(), seriesID: parseInt($("#txtInvoice").val()) },
                success: function (res) {
                    $("#txttype").text("Update");
                    $("#submit-data").prop("disabled", true);
                    $("#cancel-invoice-data").prop("hidden", false);
                    getPurchaseItemMaster(res);
                }
            });
        }
    });
    $("#find-draft").click(function () {
        DraftDataDialog("/PurchaseAP/DisplayDraftAP", "/PurchaseAP/FindDraft", "Draft Detail", "LineID", "Draft", "DraftDetails");
    });
    $("#cancel-invoice-data").click(function () {
        params.purchaseId = itemMaster.PurchaseAPID == 0 ? params.purchaseId : itemMaster.PurchaseAPID;
        const _this = this;
        $(_this).prop("disabled", true);
        if (itemMaster.PurchaseAPID > 0) {
            $.get("/PurchaseAP/GetPurchaseAPByInvoiceNo", { invoiceNo: itemMaster.Number, seriesID: itemMaster.SeriesID }, function (data) {
                $(_this).prop("disabled", false);
                if (data) {
                    if (data.AppliedAmount > 0) {
                        var dialogSubmit = new DialogBox({
                            content: "The remaining applied amount is " + data.AppliedAmount
                                + ". Do you want clear and cancel this A/P Invoice?",
                            type: "ok-cancel",
                            icon: "warning"
                        });

                        dialogSubmit.confirm(function () {
                            cancelAP(itemMaster, params.purchaseId)
                            dialogSubmit.shutdown();
                        });
                    }
                    else {
                        var dialogSubmit = new DialogBox({
                            content: "Are you sure you want to cancel this A/P Invoice?",
                            type: "ok-cancel",
                            icon: "warning"
                        });
                        dialogSubmit.confirm(function () {
                            cancelAP(itemMaster, params.purchaseId)
                            dialogSubmit.shutdown();
                        });
                    }
                }
                else {
                    var dialogSubmit = new DialogBox({
                        content: "Are you sure you want to cancel this A/P Invoice?",
                        type: "ok-cancel",
                        icon: "warning"
                    });
                    dialogSubmit.confirm(function () {
                        cancelAP(itemMaster, params.purchaseId)
                        dialogSubmit.shutdown();
                    });
                }
            });
        } else {
            var dialogSubmit = new DialogBox({
                content: "Are you sure you want to cancel this A/P Invoice?",
                type: "ok-cancel",
                icon: "warning"
            });
            dialogSubmit.confirm(function () {
                cancelAP(itemMaster, params.purchaseId)
                dialogSubmit.shutdown();
            });
        }
    })
    $("#txtcopy").change(function () {
        switch (this.value) {
            case "1":
                initModalDialog("/PurchaseAP/GetPurchaseorder", "/PurchaseAP/GetPurOrderDetailCopy", "Purchase Order (Copy)", "LineID", "PurchaseOrder", "PurchaseOrderDetials", 1);
                $("#txttype").text("PO");
                break;
            case "2":
                initModalDialog("/PurchaseAP/GetGoodsRecieptPO", "/PurchaseAP/GetGoodsRecieptPODetailCopy", "Goods Reciept PO (Copy)", "LineID", "PurchasePO", "PurchasePODetials", 2);
                $("#txttype").text("GRPO");
                break;
        }
        $(this).val(0);
    });
    // reload //
    $("#btn-addnew").click(function () {
        location.reload();
    });
    $("#cancel-data").click(function () {
        location.reload();
    });
    $(window).on("keypress", function (e) {
        if (e.which === 13) {
            if (document.activeElement === this.document.getElementById("txtbarcode")) {
                let activeElem = this.document.activeElement;
                $.get("/PurchaseAP/GetItemDetails", { itemId: 0, curId: itemMaster.PurCurrencyID, barcode: activeElem.value.trim() }, function (res) {
                    if (res.IsError) {
                        new DialogBox({
                            content: res.Error,
                        });
                    } else {
                        if (isValidArray(apDetail)) {
                            const isExisted = findArray("LineIDUN", res.LineIDUN, "UomID", res.UomID, apDetail);
                            if (!!isExisted) {
                                const qty = parseFloat(isExisted.Qty) + 1;
                                const openqty = parseFloat(isExisted.Qty) + 1;
                                updateData(apDetail, "LineIDUN", res.LineIDUN, "UomID", res.UomID, "Qty", qty);
                                updateData(apDetail, "LineIDUN", res.LineIDUN, "UomID", res.UomID, "OpenQty", openqty);
                                $listItemChoosed.updateColumn(isExisted.LineIDUN, "Qty", qty);
                                const _e = { key: isExisted.LineIDUN, data: isExisted };
                                calDisvalue(_e);
                                totalRow($listItemChoosed, _e, qty);
                            } else {
                                apDetail.push(res);
                                $listItemChoosed.addRow(res);
                            }
                        } else {
                            apDetail.push(res);
                            $listItemChoosed.addRow(res);
                        }
                        if (res) {
                            $listItemChoosed.clearHeaderDynamic(res.AddictionProps)
                            $listItemChoosed.createHeaderDynamic(res.AddictionProps)
                        }
                        setSummary(apDetail);
                        disInputUpdate(itemMaster);
                        activeElem.value = "";
                    }
                });
            }
        }
    });
    function cancelAP(item_master, purchaseApId) {
        $("#loading").prop("hidden", false);
        item_master.PurchaseAPID = 0;
        item_master.PurchaseAPDetails = apDetail;
        item_master.FreightPurchaseView.FreightPurchaseDetials = item_master.FreightPurchaseView.FreightPurchaseDetials;
        item_master.PurchaseAPDetails = item_master.PurchaseAPDetails.map(i => {
            return { ...i, PurchaseDetailAPID: 0 }
        });

        $.post('/PurchaseAP/CancelPurchaseAP',
            {
                purchaseAp: JSON.stringify(item_master),
                purchaseApId,
                serials: JSON.stringify(serialsCancel),
                batches: JSON.stringify(batchesCancel)
            }, function (res) {
                successResCancel(res);
            });
    }
    function successResCancel(response) {
        if (response.Action == 1) {
            new ViewMessage({
                summary: {
                    selector: "#error-summary"
                }
            }, response).refresh(1500);
        }
        else if (response.IsSerail) {
            $("#loading").prop("hidden", true);
            const apcs = APCSerialTemplate({
                data: {
                    serials: response.Data,
                }
            });
            apcs.serialTemplate();
            const seba = apcs.callbackInfo();
            serialsCancel = seba.serials;
        }
        else if (response.IsBatch) {
            $("#loading").prop("hidden", true);
            const batch = APCBatchNoTemplate({
                data: {
                    batches: response.Data,
                }
            });
            batch.batchTemplate();
            const seba = batch.callbackInfo();
            batchesCancel = seba.batches;
        }
        else {
            new ViewMessage({
                summary: {
                    selector: "#error-summary"
                }
            }, response);
        }
        $("#loading").prop("hidden", true);
    }
    function DraftDataDialog(urlMaster, urlDetail, caption, key, keyMaster, keyDetail) {
        $.get(urlMaster, { vendorId: itemMaster.VendorID }, function (res) {
            res.forEach(i => {
                i.PostingDate = i.PostingDate.toString().split("T")[0];
                i.SubTotal = num.formatSpecial(i.SubTotal, disSetting.Prices);
                i.BalanceDue = num.formatSpecial(i.BalanceDue, disSetting.Prices);
            })
            let dialog = new DialogBox({
                content: {
                    selector: "#Draft-list"
                },
                button: {
                    ok: {
                        Text: "Close"
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
                    type: "POST",
                    dataType: "JSON",
                    visibleFields: ["DocType", "DraftName", "PostingDate", "CurrencyName", "SubTotal", "BalanceDue", "Remarks"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
                            on: {
                                "click": function (e) {
                                    $.get(urlDetail, { draftname: e.data.DraftName, draftId: e.data.DraftID }, function (res) {
                                        bindDraft(res, keyMaster, keyDetail);
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
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(res, function (item) {
                            return item.DraftName.toLowerCase().replace(/\s+/, "").includes(__value) || item.SubTotal.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.PostingDate.toLowerCase().replace(/\s+/, "").includes(__value) || item.BalanceDue.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.CurrencyName.toLowerCase().replace(/\s+/, "").includes(__value);
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
    function initModalDialog(urlMaster, urlDetail, caption, key, keyMaster, keyDetail, type) {
        $.get(urlMaster, { vendorId: itemMaster.VendorID }, function (res) {
            res.forEach(i => {
                i.PostingDate = i.PostingDate.toString().split("T")[0];
                i.SubTotal = num.formatSpecial(i.SubTotal, disSetting.Prices);
                i.BalanceDue = num.formatSpecial(i.BalanceDue, disSetting.Prices);
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
                    visibleFields: ["DocType", "Invoice", "PostingDate", "CurrencyName", "SubTotal", "BalanceDue", "Remarks"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
                            on: {
                                "click": function (e) {
                                    $.get(urlDetail, { seriesId: e.data.SeriesID, number: e.data.Number }, function (res) {
                                        bindItemCopy(res, keyMaster, keyDetail, type);
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
                            return item.Invoice.toLowerCase().replace(/\s+/, "").includes(__value) || item.SubTotal.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.PostingDate.toLowerCase().replace(/\s+/, "").includes(__value) || item.BalanceDue.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.CurrencyName.toLowerCase().replace(/\s+/, "").includes(__value);
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
    function bindDraft(_master, keyMaster, keyDetail) {
        currencyOption(_master[keyMaster].PurCurrencyID);
        $("#freight-dailog").removeClass("disabled");
        $.get("/PurchaseAP/Getbp", { id: _master[keyMaster].VendorID }, function (cus) {
            $("#vendor-id").val(cus.Name);
        });
        itemMaster = _master[keyMaster];
        freights = _master[keyMaster].FreightPurchaseView.FreightPurchaseDetailViewModels;
        freightMaster = _master[keyMaster].FreightPurchaseView;
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
        apDetail = _master[keyDetail];
        itemMaster.ExchangeRate = _master[keyMaster].PurRate;
        $("#branch").val(_master[keyMaster].BranchID);
        $("#txtwarehouse").val(_master[keyMaster].WarehouseID);
        $("#txtreff_no").val(_master[keyMaster].ReffNo);
        $("#cur-id").val(_master[keyMaster].PurCurrencyID);
        $("#sta-id").val(_master[keyMaster].Status);
        $("#sub-id").val(num.formatSpecial(_master[keyMaster].SubTotal, disSetting.Prices));
        $("#sub-after-dis").val(num.formatSpecial(_master[keyMaster].SubTotalAfterDis, disSetting.Prices));
        $("#freight-value").val(num.formatSpecial(_master[keyMaster].FrieghtAmount, disSetting.Prices));
        //$("#sub-dis-id").val(_master[keyMaster].TypeDis);
        $("#dis-rate-id").val(num.formatSpecial(_master[keyMaster].DiscountRate, disSetting.Rates));
        $("#dis-value-id").val(num.formatSpecial(_master[keyMaster].DiscountValue, disSetting.Prices));
        setDate("#txtPostingdate", _master[keyMaster].PostingDate.toString().split("T")[0]);
        setDate("#txtDuedate", _master[keyMaster].DeliveryDate.toString().split("T")[0]);
        setDate("#txtDocumentDate", _master[keyMaster].DocumentDate.toString().split("T")[0]);
        $("#txtRemark").val(_master[keyMaster].Remark);
        $("#total-id").val(num.formatSpecial(_master[keyMaster].BalanceDue, disSetting.Prices));
        $("#vat-value").val(num.formatSpecial(_master[keyMaster].TaxValue, disSetting.Prices));
        $("#item-id").prop("disabled", false);
        $.ajax({
            url: "/PurchaseAP/GetFilterLocaCurrency",
            type: "Get",
            dataType: "Json",
            data: { CurrencyID: _master[keyMaster].PurCurrencyID },
            success: function (res) {
                $("#txtExchange").val(1 + " " + _currency + " = " + res[0].SetRate + " " + res[0].Currency.Description);
                $(".cur-class").text(res[0].Currency.Description);
                // change currency name in the rows of items after choosed ///
                $(".cur").text(res[0].Currency.Description);
            }
        });
        $.ajax({
            url: "/PurchaseAP/GetItemMasterData",
            type: "GET",
            dataType: "JSON",
            data: { ID: _master[keyMaster].WarehouseID },
            success: function (res) {
                itemMasters = res;
            }
        });

        if (_master[keyDetail].length > 0) {
            $listItemChoosed.clearHeaderDynamic(_master[keyDetail][0].AddictionProps)
            $listItemChoosed.createHeaderDynamic(_master[keyDetail][0].AddictionProps)
        }
        $listItemChoosed.bindRows(_master[keyDetail]);
    }
    function bindItemCopy(_master, keyMaster, keyDetail, copyType = 0) {
        currencyOption(_master[keyMaster].PurCurrencyID);
        $("#freight-dailog").removeClass("disabled");
        $.get("/PurchaseAP/Getbp", { id: _master[keyMaster].VendorID }, function (cus) {
            $("#vendor-id").val(cus.Name);
        });
        itemMaster = _master[keyMaster];
        freights = _master[keyMaster].FreightPurchaseView.FreightPurchaseDetailViewModels;
        freightMaster = _master[keyMaster].FreightPurchaseView;
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
        apDetail = _master[keyDetail];
        itemMaster.ExchangeRate = _master[keyMaster].PurRate;
        $("#txtwarehouse").val(_master[keyMaster].WarehouseID);
        $("#txtreff_no").val(_master[keyMaster].ReffNo);
        $("#branch").val(_master[keyMaster].BranchID);
        $("#cur-id").val(_master[keyMaster].PurCurrencyID);
        $("#sta-id").val(_master[keyMaster].Status);
        $("#sub-id").val(num.formatSpecial(_master[keyMaster].SubTotal, disSetting.Prices));
        $("#sub-after-dis").val(num.formatSpecial(_master[keyMaster].SubTotalAfterDis, disSetting.Prices));
        $("#freight-value").val(num.formatSpecial(_master[keyMaster].FrieghtAmount, disSetting.Prices));
        //$("#sub-dis-id").val(_master[keyMaster].TypeDis);
        $("#dis-rate-id").val(num.formatSpecial(_master[keyMaster].DiscountRate, disSetting.Rates));
        $("#dis-value-id").val(num.formatSpecial(_master[keyMaster].DiscountValue, disSetting.Prices));
        setDate("#txtPostingdate", _master[keyMaster].PostingDate.toString().split("T")[0]);
        setDate("#txtDuedate", _master[keyMaster].DeliveryDate.toString().split("T")[0]);
        setDate("#txtDocumentDate", _master[keyMaster].DocumentDate.toString().split("T")[0]);
        $("#remark-id").val(_master[keyMaster].Remark);
        $("#total-id").val(num.formatSpecial(_master[keyMaster].BalanceDue, disSetting.Prices));
        $("#vat-value").val(num.formatSpecial(_master[keyMaster].TaxValue, disSetting.Prices));
        $("#item-id").prop("disabled", false);
        $.ajax({
            url: "/PurchaseAP/GetFilterLocaCurrency",
            type: "Get",
            dataType: "Json",
            data: { CurrencyID: _master[keyMaster].PurCurrencyID },
            success: function (res) {
                $("#txtExchange").val(1 + " " + _currency + " = " + res[0].SetRate + " " + res[0].Currency.Description);
                $(".cur-class").text(res[0].Currency.Description);
                // change currency name in the rows of items after choosed ///
                $(".cur").text(res[0].Currency.Description);
            }
        });
        $.ajax({
            url: "/PurchaseAP/GetItemMasterData",
            type: "GET",
            dataType: "JSON",
            data: { ID: _master[keyMaster].WarehouseID },
            success: function (res) {
                itemMasters = res;
            }
        });
        itemMaster.CopyType = copyType; // 1: Order, 2: GRPO, 3: AP, 4: Credit  
        itemMaster.CopyKey = _master[keyMaster].InvoiceNo;
        itemMaster.BasedCopyKeys = "/" + _master[keyMaster].InvoiceNo;

        setSourceCopy(itemMaster.BasedCopyKeys)
        if (_master[keyDetail].length > 0) {

            $listItemChoosed.clearHeaderDynamic(_master[keyDetail][0].AddictionProps)
            $listItemChoosed.createHeaderDynamic(_master[keyDetail][0].AddictionProps)
        }
        $listItemChoosed.bindRows(_master[keyDetail]);
    }
    function selectSeries(selected) {
        $.each(___PU, function (i, item) {
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
    function successRes(response) {

        if (response.Action == 1) {
            new ViewMessage({
                summary: {
                    selector: "#error-summary"
                }
            }, response).refresh(1500);
        }
        else if (response.IsSerail) {
            $("#loading").prop("hidden", true);
            const serial = SerialTemplatePur({
                data: {
                    purData: itemMaster,
                    serials: response.Data,
                }
            });
            serial.serialTemplate();
            const seba = serial.callbackInfo();
            serials = seba.serials;

        }
        else if (response.IsBatch) {
            $("#loading").prop("hidden", true);
            const batch = BatchTemplatePur({
                data: {
                    batches: response.Data,
                }
            });
            batch.batchTemplate();
            const seba = batch.callbackInfo();
            batches = seba.batches;
        }
        else {
            new ViewMessage({
                summary: {
                    selector: "#error-summary"
                }
            }, response);
        }
        $("#loading").prop("hidden", true);
    }
    function setSourceCopy(basedCopyKeys) {
        let copyKeys = new Array();
        if (basedCopyKeys) {
            copyKeys = basedCopyKeys.split("/");
        }
        var copyInfo = "";
        $.each(copyKeys, function (i, key) {
            if (key.startsWith("PO")) {
                copyInfo += "/Purchase Order: " + key;
            }

            if (key.startsWith("PD")) {
                copyInfo += "/Goods Receipt PO: " + key;
            }

            if (key.startsWith("PU")) {
                copyInfo += "/Purchase AP: " + key;
            }
        });
        itemMaster.BasedCopyKeys = copyInfo;
        $("#txtRemark").val(copyInfo);
        $("#txtRemark").prop("readonly", true);
    }
    function getPurchaseItemMaster(_master) {
        if (_master.Error) {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: _master.Message,
                close_button: "none"
            });
        } else {
            currencyOption(_master.PurchaseAP.PurCurrencyID);
            $("#freight-dailog").removeClass("disabled");
            $.get("/PurchaseAP/Getbp", { id: _master.PurchaseAP.VendorID }, function (cus) {
                $("#vendor-id").val(cus.Name);
            });
            itemMaster = _master.PurchaseAP;
            freights = _master.PurchaseAP.FreightPurchaseView.FreightPurchaseDetailViewModels;
            freightMaster = _master.PurchaseAP.FreightPurchaseView;
            freightMaster.IsEditabled = true;
            freights.forEach(i => {
                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
                i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
                i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
            });
            apDetail = _master.PurchaseAPDetials;
            itemMaster.ExchangeRate = _master.PurchaseAP.PurRate;
            $("#branch").val(_master.PurchaseAP.BranchID);
            $("#txtwarehouse").val(_master.PurchaseAP.WarehouseID);
            $("#txtreff_no").val(_master.PurchaseAP.ReffNo);
            $("#cur-id").val(_master.PurchaseAP.PurCurrencyID);
            $("#txtstatus").val(_master.PurchaseAP.Status);
            $("#sub-id").val(num.formatSpecial(_master.PurchaseAP.SubTotal, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(_master.PurchaseAP.SubTotalAfterDis, disSetting.Prices));
            $("#freight-value").val(num.formatSpecial(_master.PurchaseAP.FrieghtAmount, disSetting.Prices));
            //$("#sub-dis-id").val(_master.PurchaseOrder.TypeDis);
            $("#dis-rate-id").val(num.formatSpecial(_master.PurchaseAP.DiscountRate, disSetting.Rates));
            $("#dis-value-id").val(num.formatSpecial(_master.PurchaseAP.DiscountValue, disSetting.Prices));
            setDate("#txtPostingdate", _master.PurchaseAP.PostingDate.toString().split("T")[0]);
            setDate("#txtDuedate", _master.PurchaseAP.DeliveryDate.toString().split("T")[0]);
            setDate("#txtDocumentDate", _master.PurchaseAP.DocumentDate.toString().split("T")[0]);
            $("#remark-id").val(_master.PurchaseAP.Remark);
            $("#total-id").val(num.formatSpecial(_master.PurchaseAP.BalanceDue, disSetting.Prices));
            $("#vat-value").val(num.formatSpecial(_master.PurchaseAP.TaxValue, disSetting.Prices));
            $("#item-id").prop("disabled", false);
            $.ajax({
                url: "/PurchaseAP/GetFilterLocaCurrency",
                type: "Get",
                dataType: "Json",
                data: { CurrencyID: _master.PurchaseAP.PurCurrencyID },
                success: function (res) {
                    $("#txtExchange").val(1 + " " + _currency + " = " + res[0].SetRate + " " + res[0].Currency.Description);
                    $(".cur-class").text(res[0].Currency.Description);
                    // change currency name in the rows of items after choosed ///
                    $(".cur").text(res[0].Currency.Description);
                }
            });
            if (_master.PurchaseAPDetials.length > 0) {
                $listItemChoosed.clearHeaderDynamic(_master.PurchaseAPDetials[0].AddictionProps)
                $listItemChoosed.createHeaderDynamic(_master.PurchaseAPDetials[0].AddictionProps)
            }
            $listItemChoosed.bindRows(_master.PurchaseAPDetials);
            $listItemChoosed.disableColumns(undefined, ["Qty", "UoMSelect", "PurchasPrice", "TaxGroupSelect", "DiscountRate", "DiscountValue", "Remark"])
            $.ajax({
                url: "/PurchaseAP/GetItemMasterData",
                type: "GET",
                dataType: "JSON",
                data: { ID: _master.PurchaseAP.WarehouseID },
                success: function (res) {
                    itemMasters = res;
                }
            });
            if (_master.PurchaseAP.Status == "close") {
                $("#submit-data").prop("disabled", true);
                $("#cancel-invoice-data").prop("disabled", true);
                $("#save-draft").prop("disabled", true);
            }
            if (_master.PurchaseAP.AppliedAmount != 0)
                $("#cancel-invoice-data").prop("disabled", true);
        }
    }
    function getFreight(success) {
        $.get("/PurchasePO/GetFreights", success);
    }
    // bind Vendor
    function bindVendor(response) {
        const vendorDialog = new DialogBox({
            content: {
                selector: ".vendor-container-list"
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
                        template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
                        on: {
                            "click": function (e) {
                                $("#vendor-id").prop("disabled", false);
                                $("#vendor-id").val(e.data.Name);
                                $("#txtcopy").prop("disabled", false);
                                itemMaster.VendorID = e.data.ID;
                                vendorDialog.shutdown();
                            }
                        }
                    }
                ]
            });
            venTable.bindRows(response);
            $("#find-vendor").on("keyup", function (e) {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let rex = new RegExp(__value, "gi");
                let __vendors = $.grep(response, function (person) {
                    return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.Phone.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.Type.match(rex);
                });
                venTable.bindRows(__vendors);
            });
        });
        vendorDialog.confirm(function () {
            vendorDialog.shutdown();
            $("#vendor-id").prop("disabled", false);
        })
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
        freightDialog.content.find(".freightSumAmount").val(num.formatSpecial(freightMaster.ExpenceAmount, disSetting.Prices));
        freightView.bindRows(freights);
    }
    function changeWarehouse(e) {
        let id = $(this).find("option:selected").val();
        $.ajax({
            url: "/PurchaseAP/GetItemMasterData",
            type: "GET",
            dataType: "JSON",
            data: { ID: id },
            success: function (res) {
                itemMasters = res;
                $("#freight-dailog").removeClass("disabled");
                $("#txtbarcode").prop("readonly", false).focus();
                currencyOption();
            }
        });
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
    //// get currency option
    function currencyOption(id = null) {
        $("#txtcurrency option").remove();
        $.ajax({
            url: "/PurchaseAP/Getcurrency",
            type: "Get",
            dataType: "Json",
            success: function (response) {
                var data = "";
                $.each(response, function (i, item) {
                    if (id !== null) {
                        if (item.ID === id) {
                            data += '<option selected value="' + item.ID + '">' + item.Description + '&nbsp&nbsp' + '(' + item.Symbol + ')' + '</option>';
                            itemMaster.PurCurrencyID = item.ID;
                        } else {
                            data += '<option  value="' + item.ID + '">' + item.Description + '&nbsp&nbsp' + '(' + item.Symbol + ')' + '</option>';
                        }
                    } else {
                        if (item.SysCur) {
                            data += '<option selected value="' + item.ID + '">' + item.Description + '&nbsp&nbsp' + '(' + item.Symbol + ')' + '</option>';
                            itemMaster.PurCurrencyID = item.ID;
                        } else {
                            data += '<option  value="' + item.ID + '">' + item.Description + '&nbsp&nbsp' + '(' + item.Symbol + ')' + '</option>';
                        }
                    }
                });
                $("#txtcurrency").append(data).prop("disabled", false);
                $.get("/PurchaseOrder/GetDisplayFormatCurrency", { curId: $("#txtcurrency").val() }, function (resp) {
                    disSetting = resp.Display;
                })
            }
        });
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
                visibleFields: ["Code", "Barcode", "ItemName1", "ItemName2", "UomName", "Cost", "Currency"],
                columns: [
                    {
                        name: "AddictionProps",
                        valueDynamicProp: "ValueName",
                        dynamicCol: true,
                    }
                ],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
                        on: {
                            "click": function (e) {

                                chooseItemDetail(e);
                            }
                        }
                    }
                ]
            });
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
                        return item.Code.toLowerCase().replace(/\s+/, "").match(rex) || item.ItemName1.toLowerCase().replace(/\s+/, "").match(rex) ||
                            name2.toLowerCase().replace(/\s+/, "").match(rex) ||
                            item.UomName.toLowerCase().replace(/\s+/, "").match(rex) ||
                            barcode.toLowerCase().replace(/\s+/, "").match(rex) ||
                            item.Cost.toString().toLowerCase().replace(/\s+/, "").match(rex)
                    });
                    itemView.bindRows(items);
                });
            }
        });
        itemDialog.confirm(function () {
            itemDialog.shutdown();
        });
    }

    function chooseItemDetail(e) {
        getItemMasterDetials(e.data.ID, function (res) {

            apDetail.push(res);
            $listItemChoosed.addRow(res);

            const e = {
                key: res.LineIDUN,
                data: res
            }
            totalRow($listItemChoosed, e, res.Qty);
            // if (isValidArray(apDetail)) 
            // {
            //     var isExisted = findArray("ItemID", res.ItemID, "UomID", res.UomID, apDetail);
            //     if (isExisted) {
            //         var qty = isExisted.Qty + 1;
            //         updateData(apDetail, "ItemID", res.ItemID, "UomID", res.UomID, "Qty", qty);
            //         updateData(apDetail, "ItemID", res.ItemID, "UomID", res.UomID, "OpenQty", qty);
            //         $listItemChoosed.updateColumn(isExisted.LineIDUN, "Qty", qty);
            //         e.data = isExisted;
            //         totalRow($listItemChoosed, e, qty);
            //     } else {
            //         $listItemChoosed.addRow(res);
            //         apDetail.push(res);
            //         totalRow($listItemChoosed, e, res.Qty);
            //     }
            // } else {
            //     $listItemChoosed.addRow(res);
            //     apDetail.push(res);
            //     totalRow($listItemChoosed, e, res.Qty);
            // }
            if (res) {
                $listItemChoosed.clearHeaderDynamic(res.AddictionProps);
                $listItemChoosed.createHeaderDynamic(res.AddictionProps);
            }
            setSummary(apDetail);
        });
    }
    function getItemMasterDetials(itemid, success) {
        $.get("/PurchaseAP/GetItemDetails", { itemid, curId: itemMaster.PurCurrencyID, barcode: "" }, success);
    }

    ////////////////
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
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "Total", totalWDis);
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxRate", e.data.TaxRate);
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxValue", vatValue);
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "FinDisRate", e.data.FinDisRate);
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "FinDisValue", fidis);
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "FinTotalValue", fitotal);
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TaxOfFinDisValue", taxoffinal);
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "TotalWTax", totalwtax);




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
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "DiscountRate", _this.value);
        const disvalue = parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.PurchasPrice) === 0 ? 0 :
            parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.PurchasPrice) / 100;

        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "DiscountValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DiscountValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisValue(e, _this) {
        if (_this.value > e.data.Qty * e.data.PurchasPrice) _this.value = e.data.Qty * e.data.PurchasPrice;
        if (_this.value == '' || _this.value < 0) _this.value = 0;
        const value = num.toNumberSpecial(_this.value);
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "DiscountValue", value);
        const ratedis = (value * 100 === 0) || (num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice) === 0) ? 0 :
            (value * 100) / (num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice));
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "DiscountRate", ratedis);
        $listItemChoosed.updateColumn(e.key, "DiscountRate", num.formatSpecial(isNaN(ratedis) ? 0 : ratedis, disSetting.Rates));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisvalue(e) {
        let disvalue = num.toNumberSpecial(e.data.DiscountRate) * num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice) === 0 ? 0 :
            (num.toNumberSpecial(e.data.DiscountRate) * num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice)) / 100;
        if (disvalue > num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice))
            disvalue = num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.PurchasPrice);
        updateData(apDetail, "LineIDUN", e.data.LineIDUN, "UomID", e.data.UomID, "DiscountValue", disvalue);
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

        const disValue = (num.toNumberSpecial(disRate) * subtotal) === 0 ? 0 : (num.toNumberSpecial(disRate / 100) * subtotal);
        itemMaster.SubTotalSys = subtotal * itemMaster.ExchangeRate;
        itemMaster.SubTotal = subtotal;
        itemMaster.TaxRate = (vat * 100) === 0 ? 0 : vat * 100 / subtotal;
        itemMaster.TaxValue = vat + num.toNumberSpecial(freightMaster.TaxSumValue);
        itemMaster.DiscountValue = disValue;
        itemMaster.SubTotalAfterDis = subtotal - disValue;
        itemMaster.SubTotalAfterDisSys = subtotal * itemMaster.ExchangeRate;
        itemMaster.DiscountRate = disRate;
        itemMaster.BalanceDue = num.toNumberSpecial(itemMaster.SubTotalAfterDis) + num.toNumberSpecial(itemMaster.TaxValue) + num.toNumberSpecial(freightMaster.ExpenceAmount);
        itemMaster.BalanceDueSys = itemMaster.BalanceDue * itemMaster.ExchangeRate;


        // if (isValidArray(apDetail)) {
        //     apDetail.forEach(i => {
        //         $listItemChoosed.updateColumn(i.ItemID, "FinDisValue", itemMaster.DiscountValue);
        //         updateData(apDetail, "ItemID", i.ItemID, "UomID", i.UomID, "FinDisValue", itemMaster.DiscountValue);
        //         const lastDisval = num.toNumberSpecial(i.Total) - (num.toNumberSpecial(itemMaster.DiscountRate) * num.toNumberSpecial(i.Total) / 100);
        //         const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
        //             num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
        //         updateData(apDetail, "ItemID", i.ItemID, "UomID", i.UomID, "TaxOfFinDisValue", TaxOfFinDisValue);
        //         updateData(apDetail, "ItemID", i.ItemID, "UomID", i.UomID, "FinTotalValue", lastDisval);
        //         $listItemChoosed.updateColumn(i.ItemID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
        //         $listItemChoosed.updateColumn(i.ItemID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
        //     });
        // }
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
        if (isValidArray(apDetail)) {
            apDetail.forEach(i => {
                const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * (num.toNumberSpecial(_this.value) / 100);
                if (num.toNumberSpecial(_this.value) < 100) {
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisValue", value);
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisRate", _this.value);

                    let lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(value);
                    let taxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        (num.toNumberSpecial(i.TaxRate) / 100) * lastDisval;

                    let totalwtax = num.toNumberSpecial(i.Total) + num.toNumberSpecial(taxOfFinDisValue)
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "TaxOfFinDisValue", taxOfFinDisValue);
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinTotalValue", lastDisval);
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "TotalWTax", totalwtax);
                    $listItemChoosed.updateColumn(i.LineIDUN, "TaxOfFinDisValue", num.formatSpecial(taxOfFinDisValue, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineIDUN, "TotalWTax", num.formatSpecial(totalwtax, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Prices));

                } else if (num.toNumberSpecial(_this.value) >= 100) {
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisValue", value);
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisRate", _this.value);

                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "TaxOfFinDisValue", 0);
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinTotalValue", 0);
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "TotalWTax", i.TotalWTax);
                    $listItemChoosed.updateColumn(i.LineIDUN, "TotalWTax", num.formatSpecial(i.TotalWTax, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineIDUN, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(apDetail);
        }
    }
    function invoiceDisAllRowValue(value) {
        if (isValidArray(apDetail)) {
            apDetail.forEach(i => {
                let _value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * (num.toNumberSpecial(value) / 100);
                if (value < 100) {
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisValue", num.formatSpecial(_value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisValue", _value);
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisRate", value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(_value);
                    const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "TaxOfFinDisValue", TaxOfFinDisValue);
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineIDUN, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                }
                else if (value >= 100) {
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisValue", _value);
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinDisRate", value);

                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "TaxOfFinDisValue", 0);
                    updateData(apDetail, "LineIDUN", i.LineIDUN, "UomID", i.UomID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineIDUN, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineIDUN, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(apDetail);
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