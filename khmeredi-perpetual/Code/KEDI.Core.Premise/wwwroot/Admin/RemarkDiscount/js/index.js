$(document).ready(function () {
    let data = new Array();
    const remarkdiscount = ViewTable({
        keyField: "ID",
        selector: "#remarkdiscount",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: true
        },
        visibleFields: ["Remark", "Active"],
        columns: [
            {
                name: "Remark",
                template: `<input />`,
                on: {
                    "keyup": function (e) {
                        updateDetails(data, "ID", e.key, "Remark", this.value);
                    }
                }
            },
            {
                name: "Active",
                template: `<input type="checkbox" />`,
                on: {
                    "click": function (e) {
                        const isChecked = $(this).prop("checked") ? true : false;
                        updateDetails(data, "ID", e.key, "Active", isChecked);
                    }
                }
            }
        ],
        actions: [
            {
                template: `<i class="fas fa-edit hover"></i>`,
                on: {
                    "click": function (e) {
                        location.href = "/RemarkDiscount/Edit?id=" + e.key;
                    }
                }
            }
        ]
    })
    $.get("/RemarkDiscount/GetRemarkDiscounts", function (res) {
        data = res;
        if (data.length > 0) {
            remarkdiscount.bindRows(data)
            $("#txtSearch").on("keyup", function () {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let items = $.grep(data, function (item) {
                    return item.Remark.toLowerCase().replace(/\s+/, "");
                });
                remarkdiscount.bindRows(items)
            });
        }
    })


    $("#save").click(function () {
        $.post("/RemarkDiscount/SaveAll", { data: JSON.stringify(data) }, function (res) {
            new ViewMessage({
                summary: {
                    selector: ".message"
                }
            }, res)
            if (res.IsApproved) {
                location.reload()
            }
        })
    })

    function updateDetails(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue)
                    i[prop] = propValue
            })
        }
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
})