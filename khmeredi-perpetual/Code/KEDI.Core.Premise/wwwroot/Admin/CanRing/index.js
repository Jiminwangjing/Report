$(document).ready(function () {
    let $listView = ViewTable({
        keyField: "ID",
        selector: "#list",
        indexed: true,
        paging: {
            enabled: true,
            pageSize: 20
        },
        visibleFields: ["Name", "PriceList", "ItemName", "Qty", "UomName", "ItemChangeName", "ChangeQty", "UomChangeName", "ChargePrice", "Currency", "IsActive"],
        columns: [
            {
                name: "Name",
                on: {
                    "dblclick": function (e) {
                        location.href = "/CanRing/CanRingSetup?id=" + e.key;
                    }
                }
            },
            {
                name: "IsActive",
                template: `<input type="checkbox" />`,
                on: {
                    "click": function (e) {
                        const active = $(this).prop("checked");
                        $.post("/canring/updateactive", { id: e.key, active })
                    }
                }
            },
            {
                name: "ChargePrice",
                dataType: "number",
                dataFormat: { fixed: 2 }
            }
        ]
    });
    $("#plid").change(function () {
        getCanRings(this.value, $("#active").val());
    })
    $("#active").change(function () {
        getCanRings($("#plid").val(), this.value);
    })
    getCanRings($("#plid").val(), $("#active").val());
    function getCanRings(plId, active) {
        $.get("/CanRing/GetCanRingsSetup", { plId, active }, function (res) {
            $listView.clearRows();
            $listView.bindRows(res);
            $("#search-can-ring-setup").on("keyup", function () {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let items = $.grep(res, function (item) {
                    return item.Name.toLowerCase().replace(/\s+/, "").includes(__value) || item.PriceList.toLowerCase().replace(/\s+/, "").includes(__value)
                        || item.ItemName.toLowerCase().replace(/\s+/, "").includes(__value) || item.UomName.toLowerCase().replace(/\s+/, "").includes(__value)
                        || item.ItemChangeName.toLowerCase().replace(/\s+/, "").includes(__value) || item.UomChangeName.toLowerCase().replace(/\s+/, "").includes(__value)
                        || item.Currency.toLowerCase().replace(/\s+/, "").includes(__value);
                });
                $listView.bindRows(items);
            });
        })
    }
});