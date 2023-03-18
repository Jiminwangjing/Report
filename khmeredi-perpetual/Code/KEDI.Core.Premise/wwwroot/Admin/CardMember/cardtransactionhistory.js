$(document).ready(function () {
    function bindData(data) {
        const _customer = ViewTable({
            keyField: "ID",
            selector: "#historical",
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: true
            },
            visibleFields: ["Number", "PostingDateFormat", "CustomerName", "PriceListName", "InAmount", "CardName", "DocCode", "OutAmount", "CumulativeBalanceDisplay"]
        });
        if (data.length == 0) {
            $("#historical-no-data").prop("hidden", false);
            $("#historical").prop("hidden", true);
            $(".box-footer").prop("hidden", true);
        } else {
            $("#historical-no-data").prop("hidden", true);
            $("#historical").prop("hidden", false);
            $(".box-footer").prop("hidden", false);
            _customer.clearRows();
            _customer.bindRows(data);
        }
        
    }
    getHistory({
        id: 0,
        dateFrom: null,
        dateTo: null
    }, function (res) {
        bindData(res);
    })
    $("#filter").click(function () {
        getHistory({
            id: $("#cus-id").val(),
            dateFrom: $("#dateFrom").val(),
            dateTo: $("#dateTo").val()
        }, function (res) {
            bindData(res);
        })
    })
    function getHistory(params, success) {
        $.get("/CardMember/GetCardTransactions", { id: params.id, dateFrom: params.dateFrom, dateTo: params.dateTo }, success);
    }
})