"use strict"

const __filterType = {
    Customer: 0,
    Warehouse: 0,
    DateFrom: $("#sale-date-from").val(),
    DateTo: $("#sale-date-to").val(),
    DateType: parseInt($("#sale-date-type").val()),
    SaleType: 4 //{ Quotation = 1, Order = 2, Delivery = 3, AR = 4, CreditMemo = 5 }
}

Object.defineProperty(__filterType, "SaleType", {
    writable: false,
    configurable: false
});

const __filterUrl = "/Sale/FilterSaleHistory",
    __searchUrl = "/Sale/SearchSaleHistory",
    _printUrlAR = JSON.parse($("#print-template-url").text()).Templateurl;

window.onload = function () {
    bindSaleHistories("#sale-histories", JSON.parse($("#datasource").text()));
}
const date = new Date();
$("#sale-date-from")[0].valueAsDate = date;
$("#sale-date-to")[0].valueAsDate = date;

$("#btn-fillter").click(() => {
    let customer = $("#sale-customer").val();
    let warehouse = $("#sale-warehouse").val();
    let datefrom = $("#sale-date-from").val();
    let dateto = $("#sale-date-to").val();
    let delivery = $("#delivery-by").val();
    let check = $("#checkAllItem").prop('checked') ? true : false;

    __filterType.Customer = customer;
    __filterType.Warehouse = warehouse;
    __filterType.DateFrom = datefrom;
    __filterType.DateTo = dateto;
    __filterType.ShippedBy = delivery;
    __filterType.Check = check;

    searchSaleHistory("#sale-histories", __filterUrl, __filterType, datefrom, dateto);
})
function searchSaleHistory(selector, url, data) {
    const __option = {
        url: url,
        data: data,
        success: function (data) {
            bindSaleHistories("#sale-histories", data)
        }
    };
    $.ajax(__option);
}
function bindSaleHistories(selector, data) {
    // $("#data-loading").show();

    let $listhistory = ViewTable({
        keyField: "ID",
        indexed: true,
        selector: selector,
        paging: {
            pageSize: 20,
            enabled: true
        },
        visibleFields: ["InvoiceNo", "CustomerName", "BalanceDueLC", "PostingDate", "DocumentDate", "DueDate", "Status", "VatType"],
        columns: [
            {
                name: "VatType",
                template: `<select>`,
                on: {
                    "change": function (e) {
                        $listhistory.updateColumn(e.data.ID, "TypeVatNumber", this.value);
                    }
                }
            },
        ],
        actions: [
            {
                template: "<i class='fas fa-print fa-lg csr-pointer fn-green'></i>",
                on: {
                    "click": function (e) {
                        let ID = e.data.ID;
                        let vatnumber = parseInt(e.data.TypeVatNumber);
                        let method = "SaleARHistory";
                        if (vatnumber == 1)
                            method = "SaleARHistoryNoneVat";
                        if (vatnumber == 2)
                            method = "SaleARHistoryVat";
                        if (vatnumber == 3)
                            method = "SaleARHistoryVatB";
                        if (vatnumber == 4)
                            method = "SaleARHistoryVatC";
                        if (vatnumber == 5)
                            method = "SaleARNoneVat01History";
                        if (vatnumber >= 1) {
                            //let __printUrl = "/Print/" + method + "?id=" + e.data.ID;
                            let __printUrl = `${_printUrlAR}Home/${method}/?id=${ID}`;
                            window.open(__printUrl, "_blank");
                        }
                        else {
                            method = "SaleARHistory";
                            window.open("/Print/" + method + "?id=" + ID + "", "_blank");
                        }
                    }
                }
            }
        ]
    });
    $listhistory.clearRows();
    $listhistory.bindRows(data);
    $("#data-loading").hide();
    $.each($("[data-date]"), function (i, t) {
        setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
    });
    function setDate(selector, date_value) {
        var _date = $(selector);
        _date[0].valueAsDate = new Date(date_value);
        _date[0].setAttribute(
            "data-date",
            moment(_date[0].value)
                .format(_date[0].getAttribute("data-date-format"))
        );
    }
    if (data.length > 0) {

        $("#btn-print").click(function () {
            let method = "SummarySheet";
            let customer = $("#sale-customer").val();
            let warehouse = $("#sale-warehouse").val();
            let datefrom = $("#sale-date-from").val();
            let dateto = $("#sale-date-to").val();
            let delivery = $("#delivery-by").val();
            let check = $("#checkAllItem").prop('checked') ? true : false;
            window.open("/Print/" + method + "?DateFrom=" + datefrom + "&DateTo=" + dateto + "&Customer=" + customer + "&WarehouseUser=" + warehouse + "&Delivery=" + delivery + "&Check=" + check + "", "_blank");
        })
    }
}
