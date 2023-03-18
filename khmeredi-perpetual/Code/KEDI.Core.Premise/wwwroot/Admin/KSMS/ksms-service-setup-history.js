$(document).ready(function () {
    
    const detialTable = ViewTable({
        keyField: "ID",
        selector: "#list-service",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: true
        },
        visibleFields: ["Active", "CreationDate", "ItemCode", "ItemName", "SetupCode", "Price", "PriceList", "Uom", "UserName"],
        columns: [
            {
                name: "Active",
                template: `<input type="checkbox" disabled/>`
            }
        ]
    })
    $.get("/KSMSServiceSetUp/GetServiceHistory", function (res) {
        detialTable.bindRows(res);
    })
})