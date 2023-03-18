﻿"use strict"
const __filterType = {
    Customer: 0,
    Warehouse: 0,
    DateFrom: $("#sale-date-from").val(),
    DateTo: $("#sale-date-to").val(),
    DateType: parseInt($("#sale-date-type").val()),
    SaleType: 8 //{ Quotation = 1, Order = 2, Delivery = 3, AR = 4, CreditMemo = 5 }
}

Object.defineProperty(__filterType, "SaleType", {
    writable: false,
    configurable: false
});

const __filterUrl = "/Sale/FilterARReserveHistory",
    __searchUrl = "/Sale/SearchARReserveHistory";
const _printUrlAR = JSON.parse($("#print-template-url").text()).Templateurl;

window.onload = function () {
    bindSaleHistories("#sale-histories", JSON.parse($("#datasource").text()));
}

$("#btn-fillter").click(() => {
    let customer = $("#sale-customer").val();
    let warehouse = $("#sale-warehouse").val();
    let datefrom = $("#sale-date-from").val();
    let dateto = $("#sale-date-to").val();
    let delivery = $("#delivery-by").val();
    console.log("dd", delivery);
    let check = $("#checkAllItem").prop('checked') ? true : false;

    __filterType.Customer = customer;
    __filterType.Warehouse = warehouse;
    __filterType.DateFrom = datefrom;
    __filterType.DateTo = dateto;
    __filterType.ShippedBy = delivery;
    console.log("ddd", __filterType.ShippedBy);
    __filterType.Check = check;

    searchARReserveHistory("#sale-histories", __filterUrl, __filterType, datefrom, dateto);
})

function searchARReserveHistory(selector, url, data) {
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
                        let vatnumber = parseInt(e.data.TypeVatNumber);
                        let method = "ARReserveHistory";
                        if (vatnumber == 1)
                            method = "SaleARReserveHistoryNoneVat";
                        if (vatnumber == 2)
                            method = "ARReserveHistoryVat";
                        if (vatnumber == 3)
                            method = "ARReserveHistoryVatB";
                        if (vatnumber == 4)
                            method = "ARReserveHistoryVatC";
                        if (vatnumber == 5)
                            method = "ARReserveNoneVat01History";
                        if (vatnumber > 0) {
                            // let _printUrlAR = "/Home/" + method + "?id=" + e.data.ID;
                            // window.open(_printUrlAR, "_blank");
                            let __printUrl = `${_printUrlAR}Home/${method}/?id=${e.data.ID}`;
                            window.open(__printUrl, "_blank");
                        }
                        else {

                            window.open("/Print/" + method + "?id=" + e.data.ID + "", "_blank");
                        }

                    }
                }
            }
        ]
    });
    $listhistory.clearRows();
    $listhistory.bindRows(data);
    $("#data-loading").hide();
}

