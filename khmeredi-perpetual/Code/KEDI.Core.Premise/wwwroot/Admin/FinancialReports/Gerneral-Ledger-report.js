$(document).ready(function () {

    $(".from-sh-posting-period").click(function () {
        postingPeriodToFrom(true);
    });
    $(".to-sh-posting-period").click(function () {
        postingPeriodToFrom();
    });

    $("#btn-ok").click(function () {
        const dateFrom = $("#from-date").val();
        const dateTo = $("#to-date").val();
        $.get("/FinancialReports/GetFinancialReports", { dateFrom: dateFrom, dateTo: dateTo }, function (data) {
            viewTable(data);
        });

    })
    let collapsed = false;
    function viewTable(data) {
        const dataGrid = $("#GeneralLedger").dxDataGrid({
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
                    dataField: "PostingDate",
                    caption: "Posting Date",
                },
                {
                    dataField: "DocumentDate",
                    caption: "Document Date",
                },
                {
                    dataField: "Creator",
                },
                {
                    dataField: "SeriesName",
                    caption: "Series Name",
                },
                {
                    dataField: "Receiptno",
                    caption: "JE Number",
                },
                {
                    dataField: "TransNo"
                },
                //{
                //    dataField: "Ofsetaccount",
                //    caption: "Ofset Acount",
                //},
                {
                    dataField: "AccountName",
                    caption: "Account Name",
                },
                {
                    dataField: "DebitCredit",
                    caption: "Deb/Cred",
                },
                {
                    dataField: "Cumolativebalance",
                    caption: "Cumolativebalance(L/C)",
                }


            ],

            onExporting: function (e) {
                var workbook = new ExcelJS.Workbook();
                var worksheet = workbook.addWorksheet('general-ledger');

                DevExpress.excelExporter.exportDataGrid({
                    component: e.component,
                    worksheet: worksheet,
                    topLeftCell: { row: 4, column: 1 }
                }).then(function (dataGridRange) {
                    // header
                    var headerRow = worksheet.getRow(2);
                    headerRow.height = 30;
                    worksheet.mergeCells(2, 1, 2, 8);
                    headerRow.getCell(1).value = 'General Ledger';
                    headerRow.getCell(1).font = { name: 'Segoe UI Light', size: 22 };
                    headerRow.getCell(1).alignment = { horizontal: 'center' };

                }).then(function () {
                    // https://github.com/exceljs/exceljs#writing-xlsx
                    workbook.xlsx.writeBuffer().then(function (buffer) {
                        saveAs(new Blob([buffer], { type: 'application/octet-stream' }), 'General-Ledger.xlsx');
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

    }
    function printPdf() {
        var datefrom = $("#from-date").val();
        var dateto = $("#to-date").val();
        window.open("/DevPrint/PrintGeneralLedger?datefrom=" + datefrom + "&dateto=" + dateto + "", "_blank");
    }
    function postingPeriodToFrom(isFrom = false) {
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
                                if (isFrom) {
                                    $("#from-date").val(formatDate(e.data.PostingDateFrom, "YYYY-MM-DD"));
                                    $(".to-sh-posting-period").css("pointer-events", "all");
                                }
                                else $("#to-date").val(formatDate(e.data.PostingDateTo, "YYYY-MM-DD"));
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
});