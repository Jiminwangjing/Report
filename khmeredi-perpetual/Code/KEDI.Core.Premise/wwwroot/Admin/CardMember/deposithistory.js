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
            visibleFields: ["Number", "PostingDateFormat", "CustomerName", "PriceListName", "TotalDepositF", "CardName", "DocCode"]
        });
        _customer.clearRows();
        _customer.bindRows(data);
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
        $.get("/CardMember/GetDepositCardMemberHistory", { id: params.id, dateFrom: params.dateFrom, dateTo: params.dateTo }, success);
    }
})