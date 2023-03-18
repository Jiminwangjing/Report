
$(document).ready(function () {
    $(".from-sh-posting-period").click(function () {
        postingPeriod("from");
    });
    $(".to-sh-posting-period").click(function () {
        postingPeriod("to");
    });
    $.get("/FinancialReports/GetDocumentTYpe", function (res) {
        let data = "<option>---Select Item---</option>";
        $.each(res, function (i, item) {
            data +=
                '<option value="' + item.Code + '">' + item.Name + '</option>';
        });
        $("#date-select").html(data);

    });
    $("#btn-ok").click(function () {
        const fromDate = $("#date_from").val();
        const toDate = $("#date_to").val();
        const transactio = $("#date-select").val();

        $("#loading-view").prop("hidden", false);
        $("#option").prop("hidden", true);
        $("#list_transaction_journal").prop("hidden", true);
        GetTransactionJournalReport(transactio, fromDate, toDate, function (data) {

            $("#list_transaction_journal").prop("hidden", false);

            viewTable(data);
            $("#loading-view").prop("hidden", true);
            $("#option").prop("hidden", false);
        });
    })

    function viewTable(data) {

        const dataGrid = $("#list_transaction_journal").dxDataGrid({
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
                    dataField: "ID",
                    caption: "Number",
                    groupIndex: 0
                },
                {
                    dataField: "Date",
                    caption: "Date",
                },
                {
                    dataField: "Series",
                    caption: "Series",
                    alignment: "center",
                },

                {
                    dataField: "Number",
                    caption: "Series Number",
                    alignment: "center",

                },
                {
                    dataField: "Type",
                    caption: "Type",
                    alignment: "center",
                },
                {
                    dataField: "Trans",
                    caption: "Trans",
                    alignment: "center",
                },
                {
                    dataField: "AccountBPCode",
                    caption: "G/L Acct/BP Code",
                    alignment: "center",
                },
                {
                    dataField: "AccountBPName",
                    caption: "G/L Acct/BP Name",
                    alignment: "center",
                },
                {
                    dataField: "DebitF",
                    caption: "Debit",
                    alignment: "center",
                },
                {
                    dataField: "CreditF",
                    caption: "Credit",
                    alignment: "center",
                },
                {
                    dataField: "Remarks",
                    caption: "Remarks",
                    alignment: "center",
                },
            ],
            summary: {
                groupItems: [
                    {
                        column: 'Trans',
                        column: 'Creator',
                        summaryType: 'max',
                        displayFormat: "Creator = {0}",
                    },
                    {
                        showInColumn: "DebitF",
                        column: 'TotalGroupDebit',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "CreditF",
                        column: 'TotalGroupCredit',
                        summaryType: 'max',
                        displayFormat: "{0}",
                        alignByColumn: true,
                    },
                    {
                        showInColumn: "Remarks",
                        column: 'MasterRemarks',
                        summaryType: "max",
                        //valueFormat: "currency",
                        displayFormat: "Remarks : {0}",
                        showInGroupFooter: true,
                        alignByColumn: true
                    },
                ],
                totalItems: [

                    {
                        showInColumn: "DebitF",
                        column: "TotalDebit",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                    {
                        showInColumn: "CreditF",
                        column: "TotalCrebit",
                        summaryType: "max",
                        displayFormat: "{0}",
                        alignment: "center"
                    },
                    {
                        showInColumn: "Remarks",
                        column: 'MasterRemarks',
                        // summaryType: 'max',
                        // displayFormat: "{0}",
                        alignment: "center"
                    },
                ],
            },
            onExporting: function (e) {
                var workbook = new ExcelJS.Workbook();
                const title = 'Transaction Journal Report';
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
                    //{
                    //    location: "after",
                    //    widget: "dxButton",
                    //    options: {
                    //        icon: "fas fa-file-pdf",
                    //        onClick: function () {
                    //            printPdf();
                    //        }
                    //    }
                    //}
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
    function GetTransactionJournalReport(journalvalue, fromDate, toDate, success) {
        $.get("/FinancialReports/GetTransactionReport", { journalvalue, fromDate, toDate }, success);
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
                    pageSize: 10
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
    //function printPdf() {
    //    var datefrom = $("#from-date").val();
    //    var dateto = $("#to-date").val();
    //    window.open("/DevPrint/SaleGrossProfit?fromDate=" + datefrom + "&toDate=" + dateto + "", "_blank");
    //}
});

