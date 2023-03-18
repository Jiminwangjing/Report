"use strict";

function PosKSMS(posCore) {
    if (!(this instanceof PosKSMS)) {
        return new PosKSMS(posCore);
    }

    const __this = this, __core = posCore,
        ksServiceMasterObj = {
            ID: 0,
            Number: "",
            DocTypeID: 0,
            SeriesID: 0,
            SeriesDID: 0,
            CusId: 0,
            PriceListID: 0,
            UserID: 0,
            KsServiceDetials: [],
            WarehouseID: 0,
            BranchID: 0,
            LocalSetRate: 0,
            LocalCurrencyID: 0,
            CompanyID: 0,
            SysCurrencyID: 0,
        };
    let __orderInfo = {},
        batches = [],
        serials = [];

    var __urls = {
        getKSServices: "/POS/GetKSServices",
        getKSServiceDetail: "/POS/GetServiceDetail",
        getCustomers: "/POS/GetCustomers",
        getSoldService: "/POS/GetSoldService",
        userService: "/POS/UpdateUseServices",
    };
    var __listViewItemOrder = __core.getListViewItemOrder();
    var collapsed = false;
    __core.loadScreen();
    __core.on("load", function (orderInfo) {
        __orderInfo = orderInfo;
        setSeries(__orderInfo);
        __core.loadScreen(false);
    });

    this.getSaleReport = function (fromdate, todate) {
        $("#loading-view").css("display", "block");
        $("#option").prop("hidden", true);
        $("#ks-report").css("display", "none");
        getSaleReport(fromdate, todate, function (data) {
            $("#ks-report").css("display", "block");
            viewTable(data, "#ks-report", "#autoExpand");
            $("#loading-view").css("display", "none");
            $("#option").prop("hidden", false);
        });
    }


    this.getUsedServiceReport = function (fromdate, todate) {
        $("#used-loading-view").css("display", "block");
        $("#used-option").prop("hidden", true);
        $("#used-ks-report").css("display", "none");
        getSaleReport(fromdate, todate, function (data) {
            $("#used-ks-report").css("display", "block");
            //viewTable(data, "#used-ks-report", "#used-autoExpand");
            viewTable(data, "#used-ks-report", "#used-autoExpand");
            $("#used-loading-view").css("display", "none");
            $("#used-option").prop("hidden", false);
        });
    }

    function viewTable(data, container, autoExpand) {
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
            loadPanel: {
                enabled: true
            },
            searchPanel: {
                visible: true,
                highlightCaseSensitive: true
            },
            groupPanel: { visible: true },
            grouping: {
                autoExpandAll: true
            },

            allowColumnReordering: true,
            rowAlternationEnabled: true,
            showBorders: true,
            export: {
                enabled: true
            },

            columns: [
                {
                    dataField: "CusName",
                    caption: "Customer Name",
                    groupIndex: 0
                },
                {
                    dataField: "BranchName",
                },
                {
                    dataField: "Plate",
                },
                {
                    dataField: "ModelName",
                },
                {
                    dataField: "ItemCode",
                },
                {
                    dataField: "ItemName",
                },
                {
                    dataField: "PostingDate",
                    caption: "Date",
                },
                {
                    dataField: "Qty",
                    caption: "Qty",
                },
                {
                    dataField: "UoM",
                    caption: "UoM Name",
                },
                {
                    dataField: "UnitPrice",
                },
                {
                    dataField: "Discount",
                },
                {
                    dataField: "TotalF",
                    caption: "Total",
                },
            ],
            summary: {
                groupItems: [
                    {
                        column: 'ItemName',
                        summaryType: 'count',
                        displayFormat: '{0}',
                    },
                    {
                        showInColumn: "TotalF",
                        column: 'TotalInvoice',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        alignByColumn: true,
                    },
                ],
                totalItems: [

                    {
                        showInColumn: "TotalF",
                        column: "TotalAll",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                ],
            },
            onExporting: function (e) {
                var workbook = new ExcelJS.Workbook();
                const title = 'Sale Report';
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
        $(autoExpand).dxCheckBox({
            value: true,
            text: "Expand All Groups",
            onValueChanged: function (data) {
                dataGrid.option("grouping.autoExpandAll", data.value);
            }
        });
    }



    function getSaleReport(fromDate, toDate, success) {
        $.get("/POS/GetSaleReport", { fromDate, toDate }, success);
    }

    function printPdf() {
        var datefrom = $("#from-date").val();
        var dateto = $("#to-date").val();
        window.open("/DevPrint/SaleKSMSReport?fromDate=" + datefrom + "&toDate=" + dateto + "", "_blank");
    }

    //invioce
    function setSeries(orderInfo) {
        let selected = $("#ks-no");
        selectSeries(selected, orderInfo);

        $('#ks-no').change(function () {
            var id = ($(this).val());
            var seriesPS = findInArray("ID", id, orderInfo.SeriesPS);
            $("#ks-number").val(seriesPS.NextNo);
            ksServiceMasterObj.Number = seriesPS.NextNo
            ksServiceMasterObj.DocTypeID = seriesPS.DocumentTypeID
            ksServiceMasterObj.SeriesID = seriesPS.ID
        });
        if (orderInfo.SeriesPS.length == 0) {
            $('#ks-no').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
            $("#fx-use-service").css("pointer-events", "none");
        }
    }

    const $listviewks = ViewTable({
        selector: ".viewksservice",
        keyField: "LineID",
        visibleFields: ["SetupCode", "ItemName", "ItemCode", "Qty", "MaxCount", "UsedCount", "UoM", "UnitPrice", "CurName"],
        paging: {
            //pageSize: 11,
            enabled: false
        },
        dataSynced: false,
        columns: [
            {
                name: "UnitPrice",
                dataType: "number"
            },
            {
                name: "MaxCount",
                dataType: "number"
            },
            {
                name: "Qty",
                dataType: "number"
            }
        ],
        scaleColumns: [
            {
                name: "UsedCount",
                dataType: "number",
                onScaleDown: {
                    "click": function (e) {
                        if (e.data.UsedCount <= 0) {
                            e.data.UsedCount = 0
                            $listviewks.updateColumn(e.key, "UsedCount", e.data.UsedCount)
                            removeItem(e)
                        }
                    }
                },
                onScaleUp: {
                    "click": function (e) {
                        if (e.data.UsedCount >= e.data.Qty) {
                            e.data.UsedCount = e.data.Qty
                            $listviewks.updateColumn(e.key, "UsedCount", e.data.UsedCount)
                        }
                    }
                }
            }
        ],
        actions: [
            {
                template: "<i class='fas fa-trash-alt csr-pointer fn-red'></i>",
                on: {
                    "click": function (e) {
                        __this.deleteItem(e)
                    }
                }
            }
        ]
    });

    //choose customer
    this.fxChooseCustomer = function () {
        __core.checkPrivilege("P018", function (posInfo) {
            let dialog = new DialogBox({
                caption: "Customer Information",
                icon: "fas fa-user-friends",
                content: {
                    selector: "#choose-customer-content-ks"
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
                    keyField: "ID",
                    indexed: true,
                    visibleFields: ["Code", "Name", "Type", "Phone", "Email"],
                    selector: "#listview-customers-ks",
                    paging: {
                        pageSize: 7
                    },
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>",
                            on: {
                                "click": function (e) {
                                    dialog.content.find(".error-message").text("")

                                    if (posInfo.Order.CustomerID != e.data.ID) {
                                        var dlgCustomer = DialogBox.Create({
                                            content: "Do you want to change current customer to [" + e.data.Name + "]?",
                                            type: "ok-cancel"
                                        })

                                        //If applying the customer for current order.
                                        dlgCustomer.confirm(function () {
                                            updateOrderCustomer(e.data)
                                            dlgCustomer.shutdown()
                                            dialog.shutdown()
                                        })

                                        //If nothing to change.
                                        dlgCustomer.reject(function () {
                                            dlgCustomer.shutdown()
                                        })
                                    }

                                }
                            }
                        }
                    ]
                })
                searchCustomers($listView)
                $("#search-customers-ks").on("keyup", function () {
                    searchCustomers($listView, this.value)
                })

            })

            dialog.confirm(function () {
                dialog.shutdown()
            })
        })
        function updateOrderCustomer(customer) {
            if (isValidJSON(customer)) {
                __orderInfo.Order.CustomerID = customer.ID
                __orderInfo.Setting.CustomerID = customer.ID
                var _customer = $.extend(__orderInfo.Order.Customer, customer)
                $("#customer-name-ks").text(_customer.Name)
                $(".customer-name").text(_customer.Name)
                $(".customer-phone").text(isEmpty(_customer.Phone) ? "N/A" : _customer.Phone)
                $("#customer-phone-ks").text(isEmpty(_customer.Phone) ? "N/A" : _customer.Phone)
            }
        }

        function searchCustomers($listView, keyword = "") {
            $.post(__urls.getCustomers, {
                keyword: keyword
            }, function (customers) {
                $listView.bindRows(customers)
            })
        }
    }
    //choose Service
    this.fxServiceDialog = function () {
        __core.checkPrivilege("P018", function (posInfo) {
            let dialog = new DialogBox({
                caption: "Information of Services",
                icon: "fas fa-user-friends",
                content: {
                    selector: "#choose-content-ks"
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
                    visibleFields: ["SetupCode", "ItemCode", "ItemName", "Qty", "UoM", "UnitPrice", "CurName", "MaxCount", "UsedCountM"],
                    selector: "#ksservice-list",
                    paging: {
                        pageSize: 10
                    },
                    columns: [
                        {
                            name: "MaxCount",
                            dataType: "number"
                        },
                        {
                            name: "Qty",
                            dataType: "number"
                        },
                        {
                            name: "UsedCountM",
                            dataType: "number"
                        },
                        {
                            name: "UnitPrice",
                            dataType: "number"
                        }
                    ],
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>",
                            on: {
                                "click": function (e) {
                                    $listviewks.addRow(e.data)
                                    if (isValidArray(e.data.ServiceSetupDetials)) {
                                        e.data.ServiceSetupDetials.forEach(i => {
                                            addSubLineItem(i, `<i class="fas fa-cogs" style="font-size: 14px"></i>`, true)
                                        })
                                    }

                                }
                            }
                        }
                    ]
                })
                searchSoldService(dialog, $listView)
                $("#search-ks").on("keyup", function () {
                    searchSoldService(dialog, $listView, this.value)
                })
            })
            dialog.confirm(function () {
                dialog.shutdown()
            })
        })
        function searchSoldService(dialog, $listView, keyword = "") {
            $.post(__urls.getSoldService, {
                cusid: __orderInfo.Setting.CustomerID,
                plId: __orderInfo.Setting.PriceListID,
                keyword: keyword
            }, function (res) {
                dialog.content.find("#loading").prop("hidden", true)
                dialog.content.find("#no-data").prop("hidden", true)
                if (isValidArray(res)) {
                    $listView.bindRows(res)
                } else {
                    $("#no-data").prop("hidden", false)
                }

            })
        }
    }
    // retrieve KSMS Service
    this.fxGetKSService = function () {
        let ksService = ViewTable({
            keyField: "LineID",
            selector: "#list-ksservice-items",
            visibleFields: ["SetupCode", "ItemCode", "UoM", "ItemName", "Cost"],
            actions: [
                {
                    template: "<i class='fas fa-arrow-circle-down fa-lg fa-fw fn-sky csr-pointer'></i>",
                    on: {
                        "click": function (e) {
                            const ksId = e.data.KSServiceSetupId
                            const detials = $listviewks.yield().filter(i => i.IsKsmsMaster)
                            $.get(__urls.getKSServiceDetail, { id: ksId }, function (res) {
                                const exsitedData = findInArray("KSServiceSetupId", ksId, detials)
                                if (!exsitedData) setKSServiceToItem(res)

                            })
                        }
                    }
                }
            ]
        });
        $.get(__urls.getKSServices, { plId: __orderInfo.Setting.PriceListID }, function (items) {
            $("#loading").prop("hidden", true)
            $("#no-data").prop("hidden", true)
            ksService.clearRows()
            if (isValidArray(items)) {
                ksService.bindRows(items)
                $("#search-ksservice").on("keyup", function () {
                    let input = $(this).val().replace(/\s/g, '')
                    let regex = new RegExp(input, "i")
                    const _items = $.grep(items, function (item) {
                        return regex.test(item.SetupCode) || regex.test(item.ItemCode) || regex.test(item.UoM) || regex.test(item.ItemName)
                    })
                    ksService.bindRows(_items)
                })
            } else {
                $("#no-data").prop("hidden", false)
            }

        })

        function setKSServiceToItem(data) {
            var lineItem = createLineItem(data.ItemID, data.UomID, data.TaxGroupID)
            lineItem.LineID = data.KSServiceSetupId
            lineItem.IsKsmsMaster = true
            lineItem.VehicleId = __orderInfo.Order.VehicleID
            lineItem.KSServiceSetupId = data.KSServiceSetupId
            lineItem.UnitPrice = data.Cost
            lineItem.Qty = lineItem.PrintQty = lineItem.BaseQty = data.Qty
            __core.sumLineItem(lineItem)
            __listViewItemOrder.updateRow(lineItem)
            if (isValidArray(data.ServiceSetupDetials)) {
                data.ServiceSetupDetials.forEach(i => {
                    var subLineItem = createLineItem(i.ItemID, i.UoMID, data.TaxGroupID)
                    subLineItem.VehicleId = __orderInfo.Order.VehicleID
                    subLineItem.KSServiceSetupId = data.KSServiceSetupId
                    subLineItem.IsKsms = true
                    subLineItem.LineID = i.LineID
                    subLineItem.LinePosition = 1 // LinePosition -> Children
                    subLineItem.Qty = subLineItem.PrintQty = subLineItem.BaseQty = i.Qty
                    subLineItem.UnitPrice = 0
                    __core.sumLineItem(subLineItem)
                    subLineItem.IsReadonly = true
                    subLineItem.ParentLineID = lineItem.LineID
                    __core.addSubLineItem(subLineItem, `<i class="fas fa-cogs" data-field="KsIcon" style="font-size: 14px"></i>`, true, true)
                })
            }
        }
    }

    function addSubLineItem(item, prefixSymbol, isReadonly = false) {
        if (isValidJSON(item)) {
            $listviewks.addRowAfter(item, item.ParentLineID, function (info) {
                if (info.data.ParentLineID == item.ParentLineID) {
                    info.data.Prefix = prefixSymbol
                    var $codeColumn = $(info.row).find("[data-field='SetupCode']")
                        .html(info.data.Prefix)
                        .css("text-align", "left")
                        .css("padding-left", "25px")
                    $(info.row).find("[data-field='UnitPrice']").html("")
                    $(info.row).find("[data-field='MaxCount']").html("")
                    $(info.row).find("[data-field='UsedCount']").html("")
                    $(info.row).find("[data-field='CurName']").html("")
                    $(info.row).find(".fa-trash-alt").parent().html("")
                    if (prefixSymbol != undefined) {
                        $codeColumn.html(prefixSymbol)
                    }
                    if (isReadonly) {
                        $(info.row).addClass("readonly")
                    }
                }
            })
        }
    }

    let hideShowServiceContainer = false
    this.fxShowHideUseService = function (service, panel) {
        if (hideShowServiceContainer) {
            $("#use-sell-service").prop("title", "Use Services")
            $(service).css("display", "none")
            $(panel).css("display", "block")
            hideShowServiceContainer = false
        } else {
            $("#use-sell-service").prop("title", "Sell Services")
            $(service).css("display", "block")
            $(panel).css("display", "none")
            hideShowServiceContainer = true
        }
    }

    this.fxUseService = function () {
        __this.checkCart(function () {
            let confirmDialog = __core.dialog("Confirmation",
                `Are you sure you want to submit the service(s)?`,
                "warning", "ok-cancel"
            )
            confirmDialog.confirm(function () {
                const detials = $listviewks.yield().filter(i => i.IsKsmsMaster)
                ksServiceMasterObj.UserID = __orderInfo.Setting.UserID
                ksServiceMasterObj.PriceListID = __orderInfo.Setting.PriceListID
                ksServiceMasterObj.CusId = __orderInfo.Setting.CustomerID
                ksServiceMasterObj.KsServiceDetials = detials
                ksServiceMasterObj.WarehouseID = __orderInfo.Setting.WarehouseID
                __core.loadScreen(true)
                confirmDialog.shutdown()
                $.post(__urls.userService, {
                    data: JSON.stringify(ksServiceMasterObj),
                    batch: JSON.stringify(batches),
                    serial: JSON.stringify(serials)
                }, function (res) {
                    if (res.IsSerail) {
                        __core.loadScreen(false);
                        const serial = SerialTemplate({
                            data: {
                                isKsms: true,
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
                                isKsms: true,
                                batches: res.Data,
                            }
                        });
                        batch.batchTemplate();
                        const seba = batch.callbackInfo();
                        batches = seba.batches;
                    }
                    else if (res.length > 0) {
                        alertOutStock(res)
                    }
                    else {
                        const { Items: { seriesUS } } = res
                        batches = []
                        serials = []
                        $("#ks-number").val(seriesUS.NextNo)
                        $listviewks.clearRows()
                    }

                    __core.loadScreen(false)
                })
            })
            confirmDialog.reject(function () {
                confirmDialog.shutdown()
            })
        })

    }
    this.deleteItem = function (e) {
        __core.checkPrivilege("P011", function () {
            let dialogDelete = __core.dialog("Delete item",
                `Are you sure you want to remove "${e.data.SetupCode}" ?`,
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

    this.checkCart = function (succeeded) {
        if (isValidArray(__this.fallBack())) {
            if (typeof succeeded === "function") {
                succeeded.call(this, __this.fallBack());
            }
        } else {
            __core.dialog("Empty Cart", "Please choose any service first!", "warning");
        }

    }
    this.fallBack = function () {
        return $listviewks.yield();
    }

    function selectSeries(selected, orderInfo) {
        $(selected).children().remove();
        $.each(orderInfo.SeriesPS, function (i, item) {
            if (item.Default == true) {
                $("<option selected value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
                $("#ks-number").val(item.NextNo);
                ksServiceMasterObj.Number = item.NextNo
                ksServiceMasterObj.SeriesID = item.ID
                ksServiceMasterObj.DocTypeID = item.DocumentTypeID
            }
            else {
                $("<option value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
            }
        });
        return selected.on('change')
    }

    function removeItem(e) {
        $listviewks.removeRow(e.key)
        if (isValidArray(e.data.ServiceSetupDetials)) {
            e.data.ServiceSetupDetials.forEach(i => $listviewks.removeRow(i.LineID))
        }
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
    //Create object order detail by sale item from pos-core.
    function createLineItem(itemId, uomId, taxgroupId) {
        var saleItem = __core.findSaleItemByItemID(itemId)
        return __core.createLineItem(saleItem.ID, uomId, taxgroupId)
    }
    function isValidJSON(value) {
        return !isEmpty(value) && value.constructor === Object && Object.keys(value).length > 0
    }

    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0
    }

    function clone(prop, deep) {
        if (deep) {
            return $(__template[prop]).clone(deep)
        }
        return $(__template[prop]).clone()
    }

    function findInArray(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue
            })[0]
        }
    }
    function isEmpty(value) {
        return value == undefined || value == null || value == ""
    }

}
