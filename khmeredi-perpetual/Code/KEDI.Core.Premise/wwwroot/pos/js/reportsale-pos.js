function ReportSale(corePos) {
    if (!(this instanceof ReportSale)) {
        return new ReportSale(corePos);
    }

    var __pos = corePos;
    var __orderInfo = {};
    var __urls = {
        getSaleHistory: "/BusinessPartner/GetingSaleHistory"
    };

    __pos.load(function (orderInfo) {
        __orderInfo = orderInfo;
    });

    $("#icon-receipt").click(() => {
        DialogReportSale();
    })

    function DialogReportSale() {
        let dialog = new DialogBox({
            caption: "List of Sale",
            icon: "fas fa-user-friends",
            content: {
                selector: "#dialog-reportsale"
            },
            type: 'ok',
            button: {
                ok: {
                    text: "Cancel"
                }
            }
        });
        dialog.invoke(function () {
            const __tablesalehistory = ViewTable({
                keyField: "LineID",
                selector: "#list-sale",
                paging: {
                    enabled: true,
                    pageSize: 15
                },
                indexed: true,
                visibleFields: [
                    "ReceiptNmber",
                    "DouType",
                    "DateOut",
                    "ItemName",
                    "Uom",
                    "Price",
                    "Qty",
                    "Total",
                ],

            });

            __pos.loadScreen(true);

            $.get(__urls.getSaleHistory, { id: __orderInfo.Order.Customer.ID }, function (data) {
                if (data.length == 0) {
                    __pos.loadScreen(false);
                }
                if (data.length > 0) {
                    __pos.loadScreen(false);
                    const i = data[0];
                    $("#lcc").val(i.SGrandTotalLCC);
                    $("#scc").val(i.SGrandTotalSys);
                    $("#vat").val(i.SVat);
                    $("#distotal").val(i.SDiscountTotal);
                    $("#disitem").val(i.SDiscountItem);
                    __tablesalehistory.clearRows();
                    __tablesalehistory.bindRows(data);
                    $("#search-sale").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(data, function (item) {
                            return item.ReceiptNmber.toLowerCase().replace(/\s+/, "").includes(__value) || item.DouType.toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.DateOut.toLowerCase().replace(/\s+/, "").includes(__value) || item.GrandTotal.toLowerCase().replace(/\s+/, "").includes(__value)
                        });
                        __tablesalehistory.bindRows(items)
                    });
                }
            })
            // block filter

            $("#filter-reportsale").click(function () {

                let datef = $("#datefrom").val();
                let datet = $("#dateto").val();
                if (datef != null || datef != "" && datet != null || datet != "") {
                    $.get(__urls.getSaleHistory, { id: __orderInfo.Order.Customer.ID, datefrom: datef, dateto: datet }, function (data) {
                        __tablesalehistory.clearRows();
                        __tablesalehistory.bindRows(data)
                    });
                }

            });

        });
        dialog.confirm(function () {


            dialog.shutdown();
        });
        dialog.reject(function () {
            dialog.shutdown();
        });
    }


}