const ___branchID = parseInt($("#BranchID").text());
$(document).ready(function () {
    $(".from-sh-posting-period").click(function () {
        postingPeriod("from");
    });
    $(".to-sh-posting-period").click(function () {
        postingPeriod("to");
    });
    $("#btn-ok").click(function () {
        const fromDate = $("#from-date").val();
        const toDate = $("#to-date").val();
        const timeTo = $("#to-time").val();
        const timeFrom = $('#from-time').val();
        const userId = $('#userid').val();
        $("#loading-view").prop("hidden", false);
        $("#option").prop("hidden", true);
        $("#list_sale_gross_profit").prop("hidden", true);
        GetSaleGrossProfitReport(fromDate, toDate, timeTo, timeFrom, userId, function (data) {
            $("#list_sale_gross_profit").prop("hidden", false);
            viewTable(data);
            $("#loading-view").prop("hidden", true);
            $("#option").prop("hidden", false);
        });
    })
    $("#to-date").change(function () {
        $("#from-time").prop('disabled', false);
        $("#to-time").prop('disabled', false);
    });

    function viewTable(data) {
        const dataGrid = $("#list_sale_gross_profit").dxDataGrid({
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
                    dataField: "Code",
                    caption: "Item Code",
                    groupIndex: 0
                },
                {
                    dataField: "ItemName",
                    caption: "Item Name",
                },
                {
                    dataField: "InvoiceNo",
                },
                {
                    dataField: "PostingDate",
                    caption: "Date In",
                },
                {
                    dataField: "TimeIn",
                },
                {
                    dataField: "DateOut",
                },
                {
                    dataField: "TimeOut",
                },
                {
                    dataField: "QtyF",
                    caption: "Qty",
                },
                {
                    dataField: "UoMName",
                    caption: "UoM Name",
                },
                {
                    dataField: "PriceF",
                    caption: "Unit Price",
                },
                {
                    dataField: "TotalPriceF",
                    caption: "Total Price",
                },

                {
                    dataField: "DiscountF",
                    caption: "Discount",
                },
                {
                    dataField: "TotalAfterDisF",
                    caption: "Total After Discount",
                },
                {
                    dataField: "CostF",
                    caption: "Cost",
                },
                {
                    dataField: "TotalCostF",
                    caption: "Total Cost",
                },
                //{
                //    dataField: "TotalF",
                //    caption: "Total",
                //    hidden: true
                //},
                {
                    dataField: "GrossProfitF",
                    caption: "Gross Profit",
                },
                {
                    dataField: "GrossProfitPF",
                    caption: "Gross Profit %",
                }
            ],
            summary: {
                groupItems: [
                    {
                        column: 'ItemName',
                        summaryType: 'count',
                        displayFormat: '{0} items',
                    },
                    {
                        showInColumn: "GrossProfitF",
                        column: 'GrossProfitItemF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "GrossProfitF",
                        column: 'GrossProfitItemF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "TotalCostF",
                        column: 'TotalItemCostF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "TotalCostF",
                        column: 'TotalItemCostF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "TotalPriceF",
                        column: 'TotalItemPriceF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "TotalPriceF",
                        column: 'TotalItemPriceF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "TotalAfterDisF",
                        column: 'TotalAfterDisItemF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "TotalAfterDisF",
                        column: 'TotalAfterDisItemF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "Discount",
                        column: 'TotalDiscountItemF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "Discount",
                        column: 'TotalDiscountItemF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "GrossProfitPF",
                        column: 'GrossProfitItemPF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        showInGroupFooter: true,
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "GrossProfitPF",
                        column: 'GrossProfitItemPF',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        alignByColumn: true,
                    },
                ],
                totalItems: [

                    {
                        showInColumn: "DiscountF",
                        column: "TotalDiscountF",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                    {
                        showInColumn: "TotalPriceF",
                        column: "TotalAllPriceF",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                    {
                        showInColumn: "TotalCostF",
                        column: "TotalAllCostF",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },

                    {
                        showInColumn: "TotalAfterDisF",
                        column: "TotalAfterDisAllF",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },

                    {
                        showInColumn: "GrossProfitF",
                        column: "TotalGrossProfitF",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                    {
                        showInColumn: "GrossProfitPF",
                        column: "TotalGrossProfitPF",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                ],
            },
            onExporting: function (e) {
                var workbook = new ExcelJS.Workbook();
                const title = 'Sale Gross Profit';
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
    var collapsed = false;
    function GetSaleGrossProfitReport(fromDate, toDate, timeTo, timeFrom, userId, success) {
        $.get("/FinancialReports/GetSaleGrossProfitReport", { fromDate, toDate, userId, timeFrom, timeTo }, success);
    }
    function postingPeriod(type) {
        let dialog = new DialogBox({
            content: {
                selector: "#postingPeriod-content"
            }
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
        dialog.invoke(function () {
            let $postingperiod = ViewTable({
                keyField: "ID",
                selector: ".posting-period",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 20
                },
                visibleFields: [
                    "PeriodCode",
                    "PeriodName",
                ],
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                if (type.toLowerCase() == "to") {
                                    $("#to-date").val(formatDate(e.data.PostingDateTo, "YYYY-MM-DD"));
                                    //$("#btn-ok").prop("disabled", false);
                                }
                                if (type.toLowerCase() == "from") {
                                    $("#from-date").val(formatDate(e.data.PostingDateFrom, "YYYY-MM-DD"));
                                    $(".to-sh-posting-period").css("pointer-events", "all");
                                }
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            onGetPostingPeriodTemplates(function (postingPeriod) {
                $postingperiod.clearRows();
                $postingperiod.bindRows(postingPeriod);
            });
        })
    }
    function onGetPostingPeriodTemplates(success) {
        $.get("/FinancialReports/GetPostingPeriods", success);
    }
    function formatDate(value, format) {
        let dt = new Date(value),
            objFormat = {
                MM: ("0" + (dt.getMonth() + 1)).slice(-2),
                DD: ("0" + dt.getDate()).slice(-2),
                YYYY: dt.getFullYear()
            },
            dateString = "";

        let dateFormats = format.split("-");
        for (let i = 0; i < dateFormats.length; i++) {
            dateString += objFormat[dateFormats[i]];
            if (i < dateFormats.length - 1) {
                dateString += "-";
            }
        }

        return dateString;
    }
    function printPdf() {
        var datefrom = $("#from-date").val();
        var dateto = $("#to-date").val();
        var timeTo = $("#to-time").val();
        var timeFrom = $('#from-time').val();
        var userId = $('#userid').val();
        window.open(`/DevPrint/SaleGrossProfit?fromDate=${datefrom}&toDate=${dateto}&timeFrom=${timeFrom}&timeTo=${timeTo}&userId=${userId}`, "_blank");
    }
});

