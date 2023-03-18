const ___branchID = parseInt($("#BranchID").text());
$(document).ready(function () {
    let startDate = moment().startOf('month');
    let nextDate = moment().endOf('month');
    $("#txtdatefrom").val(formatDate(startDate, "YYYY-MM-DD"));
    $("#txtdateto").val(formatDate(nextDate, "YYYY-MM-DD"));
    //get warehouse by branch
    $.ajax({
        url: "/Outgoingpayment/GetWarehousesOutgoingPayment",
        type: "GET",
        dataType: "JSON",
        data: {
            ID: ___branchID,
        },
        success: function (e) {
            var data = "";
            $.each(e, function (i, item) {
                data +=
                    '<option value="' + item.ID + '">' + item.Name + '</option>';
            })
            $("#txtwarehouse").append(data);
        }
    });

    $(".btnFilter").click(function () {
        onGetOutgoingPayment(function (outgoingPayment) {
            viewTable(outgoingPayment);
        });
    })

    onGetOutgoingPayment(function (outgoingPayment) {
        viewTable(outgoingPayment);
    });

    function viewTable(data) {
        $("#list_outgoingPayment").dxDataGrid({
            //dataSource: data,
            //paging: {
            //    pageSize: 10
            //},
            //pager: {
            //    showPageSizeSelector: true,
            //    allowedPageSizes: [10, 25, 50, 100]
            //},
            //remoteOperations: false,
            //searchPanel: {
            //    visible: true,
            //    highlightCaseSensitive: true
            //},
            //groupPanel: { visible: true },
            //grouping: {
            //    autoExpandAll: false
            //},
            //allowColumnReordering: true,
            //rowAlternationEnabled: true,
            //showBorders: true,

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
                    dataField: "Invioce",
                    groupIndex: 0
                },
                {
                    dataField: "ItemInvoice",
                    caption: "Item Invoice",
                    width: 150,
                },
                {
                    dataField: "Date",
                    caption: "Date",
                    dataType: "date",
                    //format: "percent",
                    //alignment: "right",
                    //allowGrouping: false,
                    //cellTemplate: discountCellTemplate,
                    //cssClass: "bullet"
                },
                {
                    dataField: "Vendor",
                    dataType: "string",
                    width: 150
                },
                {
                    dataField: "User",
                    dataType: "string"
                },
                {
                    dataField: "TotalAmountDue",
                    caption: "Total Amount Due",
                    dataType: "string",
                },
                {
                    dataField: "Totalpayment",
                    caption: "Total Payment",
                    //dataType: "string",
                },
                {
                    dataField: "Status",
                    dataType: "string",
                    width: 150
                },
                //{
                //    caption: "Cancel",
                //    width: 90,
                //    alignment: "center",
                //    cellTemplate: function (container, options) {
                //        $('<i class="fas fa-ban text-danger cursor"></i>')
                //            .on('click', function () {
                //                //previewDetail($(this).parent().prev().text())
                //            })
                //            .appendTo(container);
                //    }
                //},
            ],
            //summary: {
            //    groupItems: [
            //        {
            //            showInColumn: "TotalAmountDue",
            //            column: "TotalAmountDueSum",
            //            displayFormat: "{0}",
            //            showInGroupFooter: false,
            //        },
            //        {
            //            showInColumn: "Totalpayment",
            //            column: "TotalpaymentSum",
            //            displayFormat: "{0}",
            //            showInGroupFooter: false,
            //        },
            //    ],
            //},

            onContentReady: function (e) {
                if (!collapsed) {
                    collapsed = true;
                    e.component.expandRow(["EnviroCare"]);
                }
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

        });




    }
    var collapsed = false;
});

function printPdf() {
    var datefrom = $("#txtdatefrom").val();
    var dateto = $("#txtdateto").val();
    const status = $("#status").val();
    window.open("/DevPrint/PreviewOutgoingPaymentReport?status=" + status + "&fromDate=" + datefrom + "&toDate=" + dateto + "", "_blank");
}
function onGetOutgoingPayment(success) {
    const dateFrom = $("#txtdatefrom").val();
    const dateTo = $("#txtdateto").val();
    const status = $("#status").val();
    $.get("/Outgoingpayment/OutgoingpaymentHistoryFilter", { status, DateFrom: dateFrom, DateTo: dateTo }, success);
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