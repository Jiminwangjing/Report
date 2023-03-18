"use strict";

function CanRingExchange(posCore) {
    if (!(this instanceof CanRingExchange)) {
        return new CanRingExchange(posCore);
    }

    const __this = this, __core = posCore;
    this.isReadyPayment = false;
    let __orderInfo = {},
        batches = [],
        serials = [],
        batchPur = [],
        serialPur = [],
        canRingMasterObj = {
            ID: 0,
            Number: "",
            DocTypeID: 0,
            SeriesID: 0,
            SeriesDID: 0,
            CusId: 0,
            PriceListID: 0,
            UserID: 0,
            CanRingDetials: [],
            WarehouseID: 0,
            BranchID: 0,
            LocalSetRate: 0,
            LocalCurrencyID: 0,
            CompanyID: 0,
            SysCurrencyID: 0,
            Total: 0,
            TotalAlt: 0,
            TotalSystem: 0,
            Change: 0,
            ChangeAlt: 0,
            Received: 0,
            ReceivedAlt: 0,
            OtherPaymentGrandTotal: 0,
            GrandTotalAndChangeCurrencies: [],
            DisplayPayOtherCurrency: []
        },
        __urls = {
            getCanRingLists: "/CanRing/GetCanRingsSetup",
            getCanRing: "/CanRing/GetCanRingSetup",
            submitBuyCanRing: "/POS/SubmitBuyCanRing",
            getCanRingReport: "/CanRing/GetCanRingReport"
        };
    const $listviewks = ViewTable({
        selector: ".view-can-ring",
        keyField: "LineID",
        visibleFields: ["Name", "Qty", "ItemName", "ChangeQty", "UomLists",
            "ItemChangeName", "UomChangeLists", "ChargePrice"
        ],
        paging: {
            enabled: false
        },
        columns: [
            {
                name: "UomChangeLists",
                template: "<select></select>",
                on: {
                    change: function (e) {
                        setupCanRingTable.updateColumn(e.key, "UomChangeID", this.value);
                    }
                }
            },
            {
                name: "ChargePrice",
                template: "<input autocomplete='off'/>",
                on: {
                    keyup: function (e) {
                        $(this).asNumber()
                        if (isNaN(this.value) || this.value == '') this.value = 0;
                        e.data.ChargePrice = this.value;
                        //$listviewks.updateColumn(e.key, "ChargePrice", this.value);
                        total()
                    }
                }
            },
            {
                name: "ItemChangeName",
                //template: "<input autocomplete='off'/>",
                on: {
                    click: function (e) {
                        utils.fxChooseItems(true, $listviewks, e.data);
                        //__this.fxCanringListDialog(e.key, true)
                    }
                }
            },
            {
                name: "UomLists",
                template: "<select></select>",
                on: {
                    change: function (e) {
                        $listviewks.updateColumn(e.key, "UomID", this.value);
                    }
                }
            }
        ],
        scaleColumns: [
            {
                name: "Qty",
                onScaleDown: {
                    "click": function (e) {
                        if (e.data.Qty <= 0) {
                            e.data.Qty = 0
                            $listviewks.updateColumn(e.key, "Qty", e.data.Qty)
                            removeItem(e);
                        }
                    }
                },
                onScaleUp: {
                    "click": function () { }
                }
            },
            {
                name: "ChangeQty",
                onScaleDown: {
                    "click": function () { }
                },
                onScaleUp: {
                    "click": function () { }
                }
            }
        ],
        actions: [
            {
                template: "<i class='fas fa-trash-alt csr-pointer fn-red'></i>",
                on: {
                    "click": function (e) {
                        __this.deleteItem(e);
                    }
                }
            }
        ]
    });
    __core.loadScreen();
    __core.on("load", function (orderInfo) {
        __orderInfo = orderInfo;
        setSeries(__orderInfo);
        __core.loadScreen(false);
        total();
    });
    var utils = __core.instance["PosUtils"];
    this.fxChooseItems = function () {
        __core.fallbackInfo(function (orderInfo) {
            var $dialog = new DialogBox({
                caption: "List of items for sale",
                icon: "fas fa-tag",
                content: {
                    selector: "#group-item-listview"
                },
                button: {
                    ok: {
                        text: "Close",
                        callback: function () {
                            $dialog.shutdown();
                        }
                    }
                }
            });

            $dialog.invoke(function () {
                $dialog.content.find(".group-search-boxes").removeClass("hidden");
                var $itemListView = ViewTable({
                    selector: "#list-sale-items",
                    keyField: "ID",
                    dynamicCol: {
                        afterAction: true,
                        headerContainer: "#item-col-append",
                    },
                    visibleFields: ["Code", "KhmerName", "UnitPrice", "UoM"],
                    columns: [
                        {
                            name: "AddictionProps",
                            valueDynamicProp: "ValueName",
                            dynamicCol: true,
                        }
                    ],
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down fn-sky fa-lg csr-pointer'></i>",
                            on: {
                                "click": function (e) {
                                    __core.changeItemQty(e.key, e.data.Qty);
                                }
                            }
                        },
                    ],
                    paging: {
                        enabled: true
                    }
                });
                if (isValidArray(orderInfo.SaleItems)) {
                    $itemListView.clearHeaderDynamic(orderInfo.SaleItems[0]["AddictionProps"])
                    $itemListView.createHeaderDynamic(orderInfo.SaleItems[0]["AddictionProps"])
                }

                $itemListView.bindRows(orderInfo.SaleItems);
                let $inputSearch = $dialog.content.find("#search-sale-items");
                $inputSearch.on("keyup", function () {
                    $itemListView.clearRows();
                    var searcheds = searchSaleItems(orderInfo.SaleItems, this.value);
                    $itemListView.bindRows(searcheds);
                });
            });
        });

        function searchSaleItems(saleItems, keyword = "") {
            let input = keyword.replace(/\s/g, '');
            let regex = new RegExp(input, "i");
            var filtereds = $.grep(saleItems, function (item, i) {
                return regex.test(item.KhmerName.replace(/\s/g, ''))
                    || regex.test(item.Code.replace(/\s/g, ''))
                    || regex.test(item.UnitPrice.toString().replace(/\s/g, ''))
                    || regex.test(item.UoM.replace(/\s/g, ''));
            });
            return filtereds;
        }
    }
    $("#goto-panel-can-ring").click(function () {
        __this.gotoPanelCanRing()
    })
    //Process pay order
    $("#pay-can-ring").off("click").on("click", payOrder);
    $(document).on("keyup", function (e) {
        if (e.which == 13) {
            if (__this.isReadyPayment)
                payOrder(e);
        }
    });
    $("#pay-cash-can-ring").on("keyup focus", function (e) {
        let check = __core.toNumber($("#pay-cash-can-ring").val());
        if (isNaN(check)) {
            $('#pay-cash-can-ring').val(0);
        }
        const altcurrencyId = $("#other-currency-list-can-ring").val();
        const currency = findInArray("AltCurrencyID", altcurrencyId, __orderInfo.DisplayPayOtherCurrency);
        displayPayment(canRingMasterObj, currency);
    });
    $("#pay-other-currency-can-ring").on("keyup focus", function () {
        let value = __core.toNumber($("#pay-other-currency-can-ring").val());
        let cash = __core.toNumber($("#pay-cash-can-ring").val());
        const altcurrencyId = $("#other-currency-list-can-ring").val();
        if (isNaN(cash)) {
            $('#pay-cash-can-ring').val(0);
        }

        if (isNaN(value)) {
            $('#pay-other-currency-can-ring').val(0);
        }
        const currency = findInArray("AltCurrencyID", altcurrencyId, __orderInfo.DisplayPayOtherCurrency);
        displayPayment(canRingMasterObj, currency);
    });

    this.gotoPanelPaymentCanRing = function () {
        $("#panel-view-mode").addClass("hidden");
        __this.isReadyPayment = true;
        __this.checkCart(function () {
            defineOtherCurrencies(canRingMasterObj, __orderInfo.DisplayPayOtherCurrency);
            __this.bindAdditionalCurrencies();
            $("#panel-payment-can-ring").addClass("show");
            $("#panel-group-items").removeClass("show");
            $("#panel-group-tables").removeClass("show");
            $(".container-buy-can-ring").css("display", "none");
        });
    }
    this.gotoPanelCanRing = function () {
        $("#panel-view-mode").addClass("hidden");
        $("#panel-group-items").addClass("show");
        $("#panel-payment-can-ring").removeClass("show");
        $(".container-buy-can-ring").css("display", "block");
        __this.isReadyPayment = false;
    }
    this.fxsubmitBuyCanRing = function () {
        __this.checkCart(function () {
            if (__core.toNumber(canRingMasterObj.Total) <= __core.toNumber(canRingMasterObj.Received)) {
                let confirmDialog = __core.dialog("Confirmation",
                    `Are you sure you want to submit the buying can ring(s) ?`,
                    "warning", "ok-cancel"
                )
                confirmDialog.confirm(function () {
                    const detials = $listviewks.yield();
                    __orderInfo.Order.DisplayPayOtherCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.AltCurrencyID != i.BaseCurrencyID && i.IsShowCurrency);
                    __orderInfo.Order.DisplayPayOtherCurrency = __orderInfo.DisplayPayOtherCurrency;
                    canRingMasterObj.UserID = __orderInfo.Setting.UserID
                    canRingMasterObj.PriceListID = __orderInfo.Setting.PriceListID
                    canRingMasterObj.CusId = __orderInfo.Setting.CustomerID
                    canRingMasterObj.CanRingDetials = detials
                    canRingMasterObj.WarehouseID = __orderInfo.Setting.WarehouseID
                    canRingMasterObj.ExchangeRate = __orderInfo.Order.PLRate
                    canRingMasterObj.LocalCurrencyID = __orderInfo.Order.LocalCurrencyID
                    canRingMasterObj.LocalSetRate = __orderInfo.Order.LocalSetRate
                    canRingMasterObj.SysCurrencyID = __orderInfo.Order.SysCurrencyID
                    canRingMasterObj.BranchID = __orderInfo.Order.BranchID
                    canRingMasterObj.CompanyID = __orderInfo.Order.CompanyID
                    __core.loadScreen()
                    confirmDialog.shutdown()
                    $.post(__urls.submitBuyCanRing, {
                        canring: JSON.stringify(canRingMasterObj),
                        batch: JSON.stringify(batches),
                        serial: JSON.stringify(serials),
                        serailPur: JSON.stringify(serialPur),
                        batchPur: JSON.stringify(batchPur)
                    }, function (res) {
                        if (res.IsSerail) {
                            __core.loadScreen(false);
                            const serial = SerialTemplate({
                                data: {
                                    serials: res.Data,
                                }
                            });
                            serial.serialTemplate();
                            const seba = serial.callbackInfo();
                            serials = seba.serials;
                        }
                        else if (res.IsBatch) {
                            __core.loadScreen(false);
                            const batch = BatchNoTemplate({
                                data: {
                                    batches: res.Data,
                                }
                            });
                            batch.batchTemplate();
                            const seba = batch.callbackInfo();
                            batches = seba.batches;
                        }
                        else if (res.IsSerailPur) {
                            __core.loadScreen(false);
                            const serial = SerialTemplatePur({
                                data: {
                                    serials: res.Data,
                                }
                            });
                            serial.serialTemplate();
                            const seba = serial.callbackInfo();
                            serialPur = seba.serials;
                        }
                        else if (res.IsBatchPur) {
                            __core.loadScreen(false);
                            const batch = BatchTemplatePur({
                                data: {
                                    batches: res.Data,
                                }
                            });
                            batch.batchTemplate();
                            const seba = batch.callbackInfo();
                            batchPur = seba.batches;
                        }
                        else if (res.IsApproved) {
                            const { Items: { seriesCR } } = res
                            batches = []
                            serials = []
                            $("#can-ring-number").val(seriesCR.NextNo)
                            $listviewks.clearRows()
                            clearPayment();
                            __this.gotoPanelCanRing()
                            total()
                        }
                        else if (res.ItemsReturns.length > 0) {
                            alertOutStock(res.ItemsReturns)
                        }
                        __core.loadScreen(false)
                    })
                })
                confirmDialog.reject(function () {
                    confirmDialog.shutdown()
                })
            }
            else __core.dialog("Payment Not Enought", `Total is greater than Receive!`, "warning")
            __this.isReadyPayment = false;
        })

    }
    this.deleteItem = function (e) {
        __core.checkPrivilege("P011", function () {
            let dialogDelete = __core.dialog("Delete item",
                `Are you sure you want to remove "${e.data.Name}" ?`,
                "warning", "ok-cancel"
            )

            dialogDelete.confirm(function () {
                removeItem(e)
                dialogDelete.shutdown();
            })

            dialogDelete.reject(function () {
                dialogDelete.shutdown();
            })
        })
    }
    function removeItem(e) {
        $listviewks.removeRow(e.key)
        total();
    }
    function alertOutStock(itemReturns) {
        if (itemReturns.length > 0) {
            ViewTable({
                keyField: "LineID",
                selector: "#item-stock-info-listview",
                visibleFields: ["Code", "KhmerName", "Instock", "OrderQty", "Committed"],
                paging: {
                    enabled: false
                }
            }).bindRows(itemReturns);
            let dialog = new DialogBox({
                caption: "Not enough stock",
                icon: "warning",
                content: {
                    selector: "#item-stock-info"
                }
            });
            dialog.confirm(function () {
                dialog.shutdown();
            });
        }
    }
    //let hideShowCanRingExchangeContainer = false
    this.fxShowHideCanRing = function (service, panel) {
        if ($("#buy-can-ring").prop("title") == "Back to Sell") {
            $("#buy-can-ring").prop("title", "Purchase Can Ring")
            $(service).css("display", "none")
            $(panel).css("display", "block")
        } else {
            $("#buy-can-ring").prop("title", "Back to Sell")
            $(service).css("display", "block")
            $(panel).css("display", "none")
            $("#panel-payment-can-ring").removeClass("show");
            $("#panel-group-items").addClass("show");
            total();
        }
        $("#can-ring-cur").text("USD")
    }
    //report 
    this.fxCanringReportListDialog = function () {
        __core.checkPrivilege("P028", function (posInfo) {
            let dialog = new DialogBox({
                caption: "Can Rings Report",
                icon: "fas fa-user-friends",
                content: {
                    selector: "#content-can-ring-report"
                },
                button: {
                    ok: {
                        class: "bg-red",
                        text: "Close"
                    }
                }
            })

            dialog.invoke(function () {
                const date = new Date();
                dialog.content.find(".date-from")[0].valueAsDate = date;
                dialog.content.find(".date-to")[0].valueAsDate = date;
                dialog.content.find(".choose-customer").click(function () {
                    const vendorDialog = new DialogBox({
                        content: {
                            selector: ".customer-container-list"
                        },
                        caption: "Vendor Lists",
                        type: "ok/cancel",
                        button: {
                            ok: {
                                text: "Reset"
                            },
                            cancel: {
                                text: "Close"
                            }
                        }
                    });
                    vendorDialog.invoke(function () {
                        const venTable = ViewTable({
                            keyField: "ID",
                            selector: "#list-customers",
                            indexed: true,
                            paging: {
                                pageSize: 20,
                                enabled: true
                            },
                            visibleFields: ["Code", "Name", "Type", "Phone"],
                            actions: [
                                {
                                    template: `<i class="fa fa-arrow-alt-circle-down fa-lg csr-pointer"></i>`,
                                    on: {
                                        "click": function (e) {
                                            $(".customer-name").val(e.data.Name);
                                            $(".customer-id").val(e.data.ID);
                                            vendorDialog.shutdown();
                                        }
                                    }
                                }
                            ]
                        });
                        //GetVendors
                        $.get("/CanRing/GetCustomers", function (res) {
                            venTable.bindRows(res);
                            $("#find-customer").on("keyup", function (e) {
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
                        $(".customer-name").val("");
                        $(".customer-id").val(0);
                        vendorDialog.shutdown();
                    })
                    vendorDialog.reject(function () {
                        vendorDialog.shutdown();
                    })

                })
                dialog.content.find("#filtering").click(function () {
                    search(dialog);
                })
                search(dialog);
            })
            dialog.confirm(function () {
                dialog.shutdown()
            })
        })
        function search(dialog) {
            const customerId = dialog.content.find(".customer-id").val();
            const paymentMeansId = dialog.content.find(".pm-id").val();
            const dateFrom = dialog.content.find(".date-from").val();
            const dateTo = dialog.content.find(".date-to").val();
            dialog.content.find("#can-ring-report-show-hide").css("display", "none")
            $.get(__urls.getCanRingReport, {
                dateFrom, dateTo, customerId, paymentMeansId
            }, function (res) {
                viewTable(res, dialog.content.find("#can-ring-report"));
                dialog.content.find("#loading-view").css("display", "none")
                dialog.content.find("#can-ring-report-show-hide").css("display", "block")
            })
        }
        var collapsed = false;
        function viewTable(data, container) {
            const dataGrid = $(container).dxDataGrid({
                dataSource: data,
                allowColumnResizing: true, columnResizingMode: "nextColumn",
                columnMinWidth: 50,
                columnAutoWidth: true,
                scrolling: { columnRenderingMode: "virtual" },
                paging: {
                    pageSize: 10
                },
                pager: {
                    showPageSizeSelector: true,
                    allowedPageSizes: [10, 25, 50, 100]
                },
                remoteOperations: false,
                searchPanel: {
                    visible: true,
                    highlightCaseSensitive: true
                },
                groupPanel: { visible: true },
                grouping: {
                    autoExpandAll: true
                },
                loadPanel: {
                    enabled: true
                },
                allowColumnReordering: true,
                rowAlternationEnabled: true,
                showBorders: true,
                export: {
                    enabled: true
                },
                columns: [
                    {
                        dataField: "Number",
                        caption: "Invoice",
                        groupIndex: 0
                    },
                    {
                        dataField: "Name",
                    },
                    {
                        dataField: "ItemName",
                        caption: "Item",
                    },
                    {
                        dataField: "Qty",
                    },
                    {
                        dataField: "ItemChangeName",
                        caption: "Item Change",
                    },
                    {
                        dataField: "ChangeQty",
                        caption: "Qty Change"
                    },
                    {
                        dataField: "Total",
                    },
                ],
                summary: {
                    groupItems: [
                        {
                            column: 'DocCode',
                            summaryType: 'count',
                            displayFormat: '{0}',
                        },
                        {
                            showInColumn: "ItemName",
                            column: 'CreatedAt',
                            summaryType: 'max',
                            displayFormat: "Posting Date : {0}",
                            alignByColumn: true,
                        },
                        {
                            showInColumn: "Qty",
                            column: 'CustomerName',
                            summaryType: 'max',
                            displayFormat: "Customer : {0}",
                            alignByColumn: true,
                        },
                        {
                            showInColumn: "ItemChangeName",
                            column: 'WarehouseName',
                            summaryType: 'max',
                            displayFormat: "Warehouse : {0}",
                            alignByColumn: true,
                        },
                        {
                            showInColumn: "ChangeQty",
                            column: 'PriceList',
                            summaryType: 'max',
                            displayFormat: "Price List : {0}",
                            alignByColumn: true,
                        },
                        {
                            showInColumn: "Total",
                            column: 'PaymentMeans',
                            summaryType: 'max',
                            displayFormat: "Payment Means : {0}",
                            alignByColumn: true,
                        },
                    ],
                    totalItems: [
                        {
                            showInColumn: "ChangeQty",
                            column: "TotalDis",
                            summaryType: "max",
                            displayFormat: "Total: {0}",
                            alignment: "center"
                        },
                        {
                            showInColumn: "ChangeQty",
                            column: "TotalSystemDis",
                            summaryType: "max",
                            displayFormat: "Total SCC: {0}",
                            alignment: "center"
                        },
                        {
                            showInColumn: "ChangeQty",
                            column: "TotalLocal",
                            summaryType: "max",
                            displayFormat: "Total LCC: {0}",
                            alignment: "center"
                        },

                    ],
                },
                onExporting: function (e) {
                    var workbook = new ExcelJS.Workbook();
                    const title = 'Can Ring Report';
                    var worksheet = workbook.addWorksheet(title);

                    DevExpress.excelExporter.exportDataGrid({
                        component: e.component,
                        worksheet: worksheet,
                        topLeftCell: { row: 4, column: 1 }
                    }).then(function (dataGridRange) {
                        // header
                        var headerRow = worksheet.getRow(2);
                        headerRow.height = 30;
                        worksheet.mergeCells(2, 1, 2, 8);
                        headerRow.getCell(1).value = title;
                        headerRow.getCell(1).font = { name: 'Segoe UI Light', size: 22 };
                        headerRow.getCell(1).alignment = { horizontal: 'center' };

                    }).then(function () {
                        // https://github.com/exceljs/exceljs#writing-xlsx
                        workbook.xlsx.writeBuffer().then(function (buffer) {
                            saveAs(new Blob([buffer], { type: 'application/octet-stream' }), `${title}.xlsx`);
                        });
                    });
                    e.cancel = true;
                },
                onToolbarPreparing: function (e) {
                    var dataGrid = e.component;
                    e.toolbarOptions.items.unshift({
                        location: "before",
                        template: function () {
                            return
                        }
                    },
                        {
                            location: "after",
                            widget: "dxButton",
                            options: {
                                icon: "fas fa-file-pdf",
                                onClick: function () {
                                    printPdf();
                                }
                            }
                        }
                    );
                },
                onContentReady: function (e) {
                    if (!collapsed) {
                        collapsed = true;
                        e.component.expandRow(["EnviroCare"]);
                    }
                }
            }).dxDataGrid("instance");
            $("#autoExpand").dxCheckBox({
                value: true,
                text: "Expand All Groups",
                onValueChanged: function (data) {
                    dataGrid.option("grouping.autoExpandAll", data.value);
                }
            });
        }
    }
    //choose can ring
    this.fxCanringListDialog = function (itemChangeKey, isChangeItem = false) {
        __core.checkPrivilege("P028", function (posInfo) {
            const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0] ?? { DecimalPlaces: 0 };
            let dialog = new DialogBox({
                caption: "List of can rings",
                icon: "fas fa-user-friends",
                content: {
                    selector: "#choose-content-can-ring"
                },
                button: {
                    ok: {
                        class: "bg-red",
                        text: "Close"
                    }
                }
            })

            dialog.invoke(function () {
                let $listView = ViewTable({
                    keyField: "LineID",
                    indexed: true,
                    visibleFields: [
                        "Name", "Qty", "ItemName", "ChangeQty", "UomName",
                        "ItemChangeName", "UomChangeName", "ChargePrice"
                    ],
                    selector: "#can-ring-list",
                    paging: {
                        pageSize: 10
                    },
                    columns: [
                        {
                            name: "ChargePrice",
                            dataType: "number",
                            dataFormat: { fixed: baseCurrency.DecimalPlaces }
                        },
                    ],
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>",
                            on: {
                                "click": function (e) {
                                    $.get(__urls.getCanRing, { id: e.data.ID }, function (res) {
                                        res[0].ID = 0;
                                        if (res.length > 0) {
                                            if (isChangeItem) {
                                                $listviewks.updateColumn(itemChangeKey, "ItemChangeName", res[0].ItemChangeName);
                                                $listviewks.updateColumn(itemChangeKey, "ChangeQty", res[0].ChangeQty);
                                                $listviewks.updateColumn(itemChangeKey, "UomChangeLists", res[0].UomChangeLists);
                                                $listviewks.updateColumn(itemChangeKey, "ChargePrice", res[0].ChargePrice);
                                                $listviewks.updateColumn(itemChangeKey, "ItemChangeID", res[0].ItemChangeID);
                                                $listviewks.updateColumn(itemChangeKey, "UomChangeID", res[0].UomChangeID);
                                                //$listviewks.updateColumn(itemChangeKey, "UomChangeLists", res[0].UomChangeLists);
                                                dialog.shutdown()
                                            } else {
                                                $listviewks.addRow(res[0]);
                                            }
                                            total();
                                        }
                                    })
                                }
                            }
                        }
                    ]
                })
                search(dialog, $listView)
                $("#search-customers-ks").on("keyup", function () {
                    search(dialog, $listView, this.value)
                })
            })
            dialog.confirm(function () {
                dialog.shutdown()
            })
        })
        function search(dialog, $listView, keyword = "") {
            $.get(__urls.getCanRingLists, {
                plId: __orderInfo.Setting.PriceListID,
                keyword: keyword
            }, function (res) {
                if (isValidArray(res)) {
                    $listView.bindRows(res)
                    dialog.content.find("#loading").css("display", "none")
                    dialog.content.find("#no-data").css("display", "none")
                } else {
                    dialog.content.find("#loading").css("display", "none")
                    $("#no-data").css("display", "flex")
                }
            })
        }
    }
    this.checkCart = function (succeeded) {
        if (isValidArray(__this.fallBack())) {
            if (typeof succeeded === "function") {
                succeeded.call(this, __this.fallBack());
            }
        } else {
            __core.dialog("Empty Cart", "Please choose any items first!", "warning");
        }

    }
    this.fallBack = function () {
        return $listviewks.yield();
    }
    //Display additional total currencies and total change currencies
    this.bindAdditionalCurrencies = function () {
        const additionalCurrenciesEl = $(".addition-currencies-can-ring");
        const additionalCurrenciesElChange = $(".addition-currencies-change-can-ring");
        const subParent = $(`<div id="remove-sub"></div>`);
        const subParentChange = $(`<div id="remove-sub-change"></div>`);
        $("#remove-sub").remove();
        $("#remove-sub-change").remove();

        if (isValidArray(__orderInfo.DisplayTotalAndChangeCurrency)) {
            for (var i of __orderInfo.DisplayTotalAndChangeCurrency) {
                const total = canRingMasterObj.Total * i.AltRate;
                const totalChange = canRingMasterObj.Change * i.AltRate;
                const stackEl = $(`
                        <div class="stack-inline">
                            <div class="widget-stackcrumb">
                                <div class="step"><i class="far fa-money-bill-alt"> Total</i></div>
                            </div>
                            <input class="dy-grand-total" readonly value="${__core.toCurrency(total, i.DecimalPlaces)}">
                            <div class="symbol bg-grey">${i.AltCurrency}</div>
                        </div>`
                );
                const stackElChange = $(`
                    <div class="stack-inline">
                        <div class="widget-stackcrumb">
                            <div class="step"><i class="far fa-money-bill-alt"> Changed</i></div>
                        </div>
                        <input readonly class="error-change dy-change-total" value="${__core.toCurrency(totalChange, i.DecimalPlaces)}">
                        <div class="symbol bg-grey">${i.AltCurrency}</div>
                    </div>
                `);
                subParent.append(stackEl)
                subParentChange.append(stackElChange)
            }
            additionalCurrenciesEl.append(subParent);
            additionalCurrenciesElChange.append(subParentChange);
        }
    }
    this.calculateOrder = function (order) {
        if (isValidJSON(order)) {
            const altActiveCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.IsActive)[0] ?? { AltCurrency: "", Rate: 0 };
            order.ReceivedAlt = __core.toNumber(order.Received) * __core.toNumber(altActiveCurrency.AltRate);
            order.TotalAlt = __core.toNumber(order.Total) * __core.toNumber(altActiveCurrency.AltRate);
            order.Change = __core.toNumber(order.Received) - __core.toNumber(order.Total);
            order.ChangeAlt = __core.toNumber(order.Change) * __core.toNumber(altActiveCurrency.AltRate);
            return order;
        }
    }
    function payOrder() {
        canRingMasterObj.PaymentMeanID = $("#payment-mean-id-can-ring").val();
        //Check credit or not
        __this.fxsubmitBuyCanRing();
    }
    function defineOtherCurrencies(order, currency) {
        $("#other-currency-list-can-ring").children().remove();
        if (isValidArray(currency)) {
            currency.forEach(i => {
                let option = $("<option value='" + i.AltCurrencyID + "'>" + i.AltCurrency + "</option>");
                if (i.IsLocalCurrency) {
                    option = $("<option selected value='" + i.AltCurrencyID + "'>" + i.AltCurrency + "</option>");
                }
                $("#other-currency-list-can-ring").append(option);
            })
        }
        $("#other-currency-list-can-ring").on("change", function () {
            $(".other-curr-symbol").text($(this).find("option:selected").text());
            const _currency = findInArray("AltCurrencyID", this.value, currency);
            displayPayment(order, _currency);
        });
        $(".other-curr-symbol").text($("#other-currency-list-can-ring").find("option:selected").text());
        const firstCurrency = currency.filter(i => i.IsLocalCurrency)[0];
        displayPayment(order, firstCurrency);
        setTimeout(function () {
            $(activeInput).focus();
            activeInput.selectionStart = activeInput.value.length;
        }, 200);
    }
    function displayPayment(order, currency) {
        const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0] ?? { DecimalPlaces: 0 };
        const grandTotalDisplay = __core.toCurrency(order.Total, baseCurrency.DecimalPlaces);
        if ($("#other-currency-list-can-ring").val() == currency.AltCurrencyID) {
            order.OtherPaymentGrandTotal = __core.toNumber($("#pay-other-currency-can-ring").val()) / currency.AltRate;
        } else {
            order.OtherPaymentGrandTotal = __core.toNumber($("#pay-other-currency-can-ring").val());
        }
        order.Total = __core.toNumber(grandTotalDisplay);
        order.Received = (__core.toNumber($("#pay-cash-can-ring").val()) + order.OtherPaymentGrandTotal);
        order.PaymentMeanID = $("#payment-mean-id-can-ring").val();
        __this.calculateOrder(order);
        __this.bindAdditionalCurrencies();
        if (__core.toNumber(order.Received) < __core.toNumber(order.Total)) {
            $(".error-change").addClass("fn-red").removeClass("fn-green");
            $('#pay-received-can-ring').addClass("fn-red");
        }
        else {
            $(".error-change").removeClass("fn-red").addClass("fn-green");
            $('#pay-received-can-ring').removeClass("fn-red");
        }
        $("#pay-total-can-ring").val(grandTotalDisplay);
        $("#pay-total-alt-can-ring").val(__core.toCurrency(order.Total * currency.AltRate, currency.DecimalPlaces));
        $(".base-currency").text(currency.BaseCurrency);
        $(".alt-currency").text(currency.AltCurrency);
        $('#pay-received-can-ring').val(__core.toCurrency(order.Received, baseCurrency.DecimalPlaces));
        $('#pay-change-can-ring').val(__core.toCurrency(order.Change, baseCurrency.DecimalPlaces));
        $('#pay-change-alt-can-ring').val(__core.toCurrency(order.Change * currency.AltRate, currency.DecimalPlaces));
        //__actionKeypressed = false;
    }
    //........................Grid button clicked......................................//
    const numberPad = $("#panel-payment-can-ring .number-pad .grid");
    const numberMultiply = $("#panel-payment-can-ring .number-multiply .grid");
    const numberPlus = $("#panel-payment-can-ring .number-plus .grid");
    let value = "";
    var activeInput = document.querySelector("#pay-cash-can-ring");
    $("input[class='number']").asNumber();
    $("input[class='number']").focus(function (e) {
        activeInput = this;
    });

    let inputValue = 0;
    numberPlus.click(function (e) {
        onQuickNumber(this, true);
    });

    numberMultiply.click(function (e) {
        onQuickNumber(this, false);
    });

    numberPad.on("click", function (e) {
        let input_value = activeInput.value;
        if (input_value !== undefined) {
            setTimeout(() => {
                activeInput.selectionStart = activeInput.value.length;
                $(activeInput).focus();
            }, 0);

            value = input_value + $(this).data("value").toString();
            value = $.validNumber(value);
            activeInput.value = value;
            if ($(this).data("value") === -1) {
                activeInput.value = input_value.substring(0, input_value.length - 1);
                if (activeInput.value === "") {
                    activeInput.value = 0;
                }
            }
        }

    });

    function onQuickNumber(input, isPlus) {
        let value = __core.toNumber($("#pay-other-currency-can-ring").val());
        let cash = __core.toNumber($("#pay-cash-can-ring").val());
        if (isNaN(cash)) {
            $('#pay-cash-can-ring').val(0);
        }
        if (isNaN(value)) {
            $('#pay-other-currency-can-ring').val(0);
        }

        inputValue = __core.toNumber($(activeInput).val());
        if (isPlus) {
            inputValue += __core.toNumber($(input).data("value"));
        } else {
            inputValue *= __core.toNumber($(input).data("value"));
        }

        $(activeInput).val(inputValue);
        activeInput.selectionStart = activeInput.value.length;
        $(activeInput).focus();
    }
    function clearPayment() {
        $('#pay-cash-can-ring').val(0.000);
        $('#other-pay-can-ring').val(0.000);
        $("#pay-total-can-ring").val(0.000);
        $("#pay-change-can-ring").val(0.000);
        $('#pay-change-sys-can-ring').val(0.000);
        $("#pay-total-alt-can-ring").val(0.000);
        $("#pay-change-alt-can-ring").val(0.000);
        $("#pay-other-currency-can-ring").val(0.000);
        $("#pay-received-can-ring").val(0.000);
        $("#payment-mean-id-can-ring").val($("#payment-mean-id-can-ring > option:first-child").val());
        $("#error-message").text("");
        $(".dy-grand-total").val(0.000);
        $(".dy-change-total").val(0.000);
        $("#can-ring-summary-total").val(0.000);
    }
    //invioce
    function setSeries(orderInfo) {
        let selected = $("#can-ring-no");
        selectSeries(selected, orderInfo);

        $('#can-ring-no').change(function () {
            var id = ($(this).val());
            var seriesCR = findInArray("ID", id, orderInfo.SeriesCR);
            $("#can-ring-number").val(seriesCR.NextNo);
            canRingMasterObj.Number = seriesCR.NextNo
            canRingMasterObj.DocTypeID = seriesCR.DocumentTypeID
            canRingMasterObj.SeriesID = seriesCR.ID
        });
        if (orderInfo.SeriesCR.length == 0) {
            $('#can-ring-no').append(`
                    <option selected> No Invoice Numbers Created!!</option>
                `).prop("disabled", true);
            $("#fx-buy-can-ring").css("pointer-events", "none");
        }
    }
    function selectSeries(selected, orderInfo) {
        $(selected).children().remove();
        $.each(orderInfo.SeriesCR, function (i, item) {
            if (item.Default == true) {
                $("<option selected value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
                $("#can-ring-number").val(item.NextNo);
                canRingMasterObj.Number = item.NextNo
                canRingMasterObj.SeriesID = item.ID
                canRingMasterObj.DocTypeID = item.DocumentTypeID
            }
            else {
                $("<option value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
            }
        });
        return selected.on('change')
    }
    const total = () => {
        const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0] ?? { DecimalPlaces: 0 };
        const data = $listviewks.yield();
        let total = 0;
        if (data.length > 0) {
            data.forEach(i => {
                total += __core.toNumber(i.ChargePrice);
            })
            canRingMasterObj.Total = total;
            canRingMasterObj.Change = canRingMasterObj.Change - total;
            canRingMasterObj.TotalSystem * data[0].ExchangRate;
            $("#can-ring-summary-total").text(`${__core.toCurrency(total, baseCurrency.DecimalPlaces)}`);
        }
        $("#can-ring-summary-total").text(`${__core.toCurrency(total, baseCurrency.DecimalPlaces)}`);
    }
    function isValidJSON(value) {
        return !isEmpty(value) && value.constructor === Object && Object.keys(value).length > 0
    }
    function findInArray(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
    function isEmpty(value) {
        return value == undefined || value == null || value == "";
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0
    }
}