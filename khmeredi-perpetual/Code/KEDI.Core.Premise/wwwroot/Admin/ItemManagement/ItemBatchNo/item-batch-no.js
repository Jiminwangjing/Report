$(document).ready(function () {

    const filterObj = {
        Operations: 1,
        ItemNo: "",
        DateFrom: "",
        DateTo: "",
        GoodsReceiptPO: false,
        APInvoices: false,
        APCreditMemos: false,
        Deliveries: false,
        ReturnDeliveries: false,
        ARInvoices: false,
        ARCreditMemos: false,
        GoodsReciept: false,
        GoodsIssue: false,
    }
    let batchCreated = ViewTable({});
    const batchNoDoc = ViewTable({
        keyField: "LineID",
        selector: ".batch-no-doc",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: false
        },
        visibleFields: ["DocNo", "ItemCode", "ItemName1", "ItemName2", "WhsCode", "WhsName"],
        columns: [
            {
                name: "DocNo",
                on: {
                    "dblclick": function (e) {
                        //13=> GoodsIssue, 4=> AR, 3=> Delivery, 11=> PurCreditMemo
                        if (e.data.TransType == 13 || e.data.TransType == 4 || e.data.TransType == 3 || e.data.TransType == 11) {
                            if (!isValidArray(e.data.Batches)) {
                                $.get(
                                    "/ItemBatchNo/GetBatchCreated",
                                    {
                                        itemId: e.data.ItemID,
                                        saleId: e.data.TransId,
                                        transType: e.data.TransType
                                    },
                                    function (res) {
                                        e.data.Batches = res;
                                        batchCreated.clearRows();
                                        batchCreated.bindRows(e.data.Batches)
                                    }
                                )
                            }
                        }
                        //9=> GRPO(PurID), 10=> PurAP(PurID), 6=> ReturnDelivery(SaleID), 5=> CreditMemo(SaleID), 12=> GoodsReceipt(GRGIID)
                        if (e.data.TransType == 9 || e.data.TransType == 10 || e.data.TransType == 6 || e.data.TransType == 5 || e.data.TransType == 12) {
                            if (!isValidArray(e.data.Batches)) {
                                $.get(
                                    "/ItemBatchNo/GetBatchCreatedWPId",
                                    {
                                        propId: "InStockFrom",
                                        itemId: e.data.ItemID,
                                        saleId: e.data.TransId,
                                        transType: e.data.TransType
                                    },
                                    function (res) {
                                        e.data.Batches = res;
                                        batchCreated.clearRows();
                                        batchCreated.bindRows(e.data.Batches)
                                    }
                                )
                            }
                        }
                        bindSerialSetial(e.data, e.data.Batches);
                    }
                }
            }
        ]
    })
    $("#cancel-data").click(function () {
        location.reload();
    })
    $("#save-data").click(function () {
        $.post(
            '/ItemBatchNo/UpdateBatchNo',
            {
                batches: batchNoDoc.yield()
            },
            function (res) {
                if (res.IsRejected) {
                    new ViewMessage({
                        summary: {
                            selector: ".batch-message"
                        }
                    }, res);
                } else {
                    //$(".serial-message").text("");
                    new ViewMessage({
                        summary: {
                            selector: ".batch-message"
                        }
                    }, res);
                    location.reload();
                }
            }
        )
    })
    $("#optionbtn").click(function () {
        const optionModel = new DialogBox({
            content: {
                selector: "#optionfilter"
            },
            caption: "Option filter",
            type: "ok/cancel",
        });
        $("#operation").val(filterObj.Operations);
        $("#itemCode").val(filterObj.ItemNo);
        $("#date-from").val(filterObj.DateFrom);
        $("#date-to").val(filterObj.DateTo);
        $("#grpo").prop("checked", filterObj.GoodsReceiptPO);
        $("#apinvoice").prop("checked", filterObj.APInvoices);
        $("#apcreditmemo").prop("checked", filterObj.APCreditMemos);
        $("#delivery").prop("checked", filterObj.Deliveries);
        $("#returndeliveries").prop("checked", filterObj.ReturnDeliveries);
        $("#arinvoice").prop("checked", filterObj.ARInvoices);
        $("#arcreditmemo").prop("checked", filterObj.ARCreditMemos);
        $("#goodsReceipt").prop("checked", filterObj.GoodsReciept);
        $("#goodsIssue").prop("checked", filterObj.GoodsIssue);
        optionModel.confirm(function () {
            filterObj.Operations = $("#operation").val();
            filterObj.ItemNo = $("#itemCode").val().trim();
            filterObj.DateFrom = $("#date-from").val();
            filterObj.DateTo = $("#date-to").val();
            filterObj.GoodsReceiptPO = $("#grpo").prop("checked") ? true : false;
            filterObj.APInvoices = $("#apinvoice").prop("checked") ? true : false;
            filterObj.APCreditMemos = $("#apcreditmemo").prop("checked") ? true : false;
            filterObj.Deliveries = $("#delivery").prop("checked") ? true : false;
            filterObj.ReturnDeliveries = $("#returndeliveries").prop("checked") ? true : false;
            filterObj.ARInvoices = $("#arinvoice").prop("checked") ? true : false;
            filterObj.ARCreditMemos = $("#arcreditmemo").prop("checked") ? true : false;
            filterObj.GoodsReciept = $("#goodsReceipt").prop("checked") ? true : false;
            filterObj.GoodsIssue = $("#goodsIssue").prop("checked") ? true : false;
            $.get("/ItemBatchNo/GetBatchNoDoc", { filter: JSON.stringify(filterObj) }, function (res) {
                if (res.IsRejected) {
                    new ViewMessage({
                        summary: {
                            selector: ".err-succ-message"
                        }
                    }, res);
                } else {
                    $(".err-succ-message").text("");
                    batchNoDoc.clearRows();
                    batchCreated.clearRows();
                    batchNoDoc.bindRows(res);
                    optionModel.shutdown();
                }
            })

        })
        optionModel.reject(function () {
            optionModel.shutdown();
        })
    });
    $(".choose-item").click(function () {
        const itemDialog = new DialogBox({
            content: {
                selector: ".item-lists"
            },
            caption: "Item Master Data",
            type: "ok",
            button: {
                ok: {
                    text: "Close"
                }
            }
        });
        itemDialog.invoke(function () {
            const itemTable = ViewTable({
                keyField: "ID",
                selector: ".item-table",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: false
                },
                visibleFields: ["Code", "Barcode", "KhmerName"],
                actions: [
                    {
                        template: `<i class='fas fa-arrow-circle-down' style="cursor:pointer"></i>`,
                        on: {
                            "click": function (e) {
                                $("#itemCode").val(e.data.Code);
                                itemDialog.shutdown();
                            }
                        }
                    }
                ]
            });
            $.get("/ItemBatchNo/GetItemMasterData", function (res) {
                itemTable.bindRows(res);
            });
        })
        itemDialog.confirm(function () {
            itemDialog.shutdown();
        })
    })
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function bindSerialSetial(master, detials) {
        const masterData = batchNoDoc.yield();
        batchCreated = ViewTable({
            keyField: "LineID",
            selector: ".created-batch-no",
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: [
                "Batch", "", "BatchAttribute1", "BatchAttribute2", "ExpirationDate",
                "MfrDate", "AdmissionDate", "Location", "Details", "UnitCost"
            ],
            columns: [
                {
                    name: "Batch",
                    template: "<input />",
                    on: {
                        "keyup": function (e) {
                            updateSerialDetails(
                                masterData,
                                "LineID",
                                master.LineID,
                                "LineID",
                                e.key,
                                "Batch",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "BatchAttribute1",
                    template: "<input />",
                    on: {
                        "keyup": function (e) {
                            updateSerialDetails(
                                masterData,
                                "LineID",
                                master.LineID,
                                "LineID",
                                e.key,
                                "BatchAttribute2",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "BatchAttribute2",
                    template: "<input />",
                    on: {
                        "keyup": function (e) {
                            updateSerialDetails(
                                masterData,
                                "LineID",
                                master.LineID,
                                "LineID",
                                e.key,
                                "BatchAttribute2",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "ExpirationDate",
                    template: "<input type='date'/>",
                    on: {
                        "change": function (e) {
                            updateSerialDetails(
                                masterData,
                                "LineID",
                                master.LineID,
                                "LineID",
                                e.key,
                                "ExpirationDate",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "MfrDate",
                    template: "<input type='date'/>",
                    on: {
                        "change": function (e) {
                            updateSerialDetails(
                                masterData,
                                "LineID",
                                master.LineID,
                                "LineID",
                                e.key,
                                "MfrDate",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "AdmissionDate",
                    template: "<input type='date'/>",
                    on: {
                        "change": function (e) {
                            updateSerialDetails(
                                masterData,
                                "LineID",
                                master.LineID,
                                "LineID",
                                e.key,
                                "AdmissionDate",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "Location",
                    template: "<input type='text'/>",
                    on: {
                        "keyup": function (e) {
                            updateSerialDetails(
                                masterData,
                                "LineID",
                                master.LineID,
                                "LineID",
                                e.key,
                                "Location",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "Details",
                    template: "<input type='text'/>",
                    on: {
                        "keyup": function (e) {
                            updateSerialDetails(
                                masterData,
                                "LineID",
                                master.LineID,
                                "LineID",
                                e.key,
                                "Details",
                                this.value
                            );
                        }
                    }
                },
            ]
        });
        batchCreated.bindRows(detials)
    }
    function updateSerialDetails(data, masterKey, masterValue, detailKey, detialValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[masterKey] === masterValue) {
                    if (isValidArray(i["Batches"])) {
                        i["Batches"].forEach(j => {
                            if (j[detailKey] === detialValue) {
                                j[prop] = propValue;
                            }
                        });
                    }
                }
            });
        }
    }
});