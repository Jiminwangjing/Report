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
        $("#loading-view").prop("hidden", false);
        $("#list_cash_flow_for_treasury").prop("hidden", true);
        $("#option").prop("hidden", true);
        GetCashFlowForTreasuryReport(fromDate, toDate, function (data) {
            viewTable(data);
            $("#loading-view").prop("hidden", true);
            $("#list_cash_flow_for_treasury").prop("hidden", false);
            $("#option").prop("hidden", false);
        });
    })

    function viewTable(data) {
        const dataGrid = $("#list_cash_flow_for_treasury").dxDataGrid({
            dataSource: data,
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
                    dataField: "DueDate",
                    caption: "Posting Date",
                    groupIndex: 0
                },
                {
                    dataField: "Origin",
                    caption: "Origin",
                    width: 150,
                },
                {
                    dataField: "Referrence",
                    caption: "Referrence",
                },
                {
                    dataField: "ControlAccount",
                    caption: "Controll Account",
                    width: 150
                },
                {
                    dataField: "GLAccBPCode",
                    caption: "GL/Account/BPCode",
                    dataType: "string"
                },
                {
                    dataField: "Remarks",
                    dataType: "string",
                },
                {
                    dataField: "Debit",
                },
                {
                    dataField: "Credit",
                },
                {
                    dataField: "Total",
                },
                {
                    dataField: "Balance",
                },
            ],
            summary: {
                totalItems: [

                    {
                        showInColumn: "Debit",
                        column: "DebitTotal",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                    {
                        showInColumn: "Credit",
                        column: "CreditTotal",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                    {
                        showInColumn: "Total",
                        column: "TotalSummary",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                    {
                        showInColumn: "Balance",
                        column: "BalanceTotal",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                ],
            },
            onExporting: function (e) {
                var workbook = new ExcelJS.Workbook();
                var worksheet = workbook.addWorksheet('cash-flow-for-treasury');

                DevExpress.excelExporter.exportDataGrid({
                    component: e.component,
                    worksheet: worksheet,
                    topLeftCell: { row: 4, column: 1 }
                }).then(function (dataGridRange) {
                    // header
                    var headerRow = worksheet.getRow(2);
                    headerRow.height = 30;
                    worksheet.mergeCells(2, 1, 2, 8);
                    headerRow.getCell(1).value = 'Cash Flow For Treasury';
                    headerRow.getCell(1).font = { name: 'Segoe UI Light', size: 22 };
                    headerRow.getCell(1).alignment = { horizontal: 'center' };

                }).then(function () {
                    // https://github.com/exceljs/exceljs#writing-xlsx
                    workbook.xlsx.writeBuffer().then(function (buffer) {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'Cash-Flow-For-Treasury.xlsx');
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
    function GetCashFlowForTreasuryReport(fromDate, toDate, success) {
        $.get("/FinancialReports/GetCashFlowForTreasuryReport", { fromDate, toDate }, success);
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
        if (datefrom == "" || dateto == "") {
            $("#from-date").css("border-color", "red");
            $("#to-date").css("border-color", "red");
            return;
        } else {
            $("#from-date").css("border-color", "#DDDDDD"); 
            $("#to-date").css("border-color", "#DDDDDD");
            window.open("/DevPrint/CashFlowTreasury?fromDate=" + datefrom + "&toDate=" + dateto + "", "_blank");
        }
    }
});

