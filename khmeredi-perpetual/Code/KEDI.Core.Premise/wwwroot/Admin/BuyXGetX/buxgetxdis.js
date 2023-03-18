$(document).ready(function () {
    let $table = ViewTable({
        keyField: "LineID",
        selector: ".item-detail",
        indexed: true,
        paging: {
            enabled: true,
            pageSize: 10
        },
        visibleFields: [
            "Name",
            "Code",
            "PriceListSelect",
            "DateF",
            "DateT",
            "Amount",
            "DisTypeSelect",
            "DisRateValue",
            "Active",
        ],
        columns: [

            {
                name: "Name",
                template: "<input class='input-box-kernel'>",
                on: {
                    "keyup": function (e) {
                        updateDetails($table.yield(), "LineID", e.key, "Name", this.value);
                    }
                }
            },
            {
                name: "Code",
                template: "<input class='input-box-kernel'>",
                on: {
                    "keyup": function (e) {
                        updateDetails($table.yield(), "LineID", e.key, "Code", this.value);
                    }
                }
            },
            {
                name: "PriceListSelect",
                template: "<select></select>",
                on: {
                    "change": function (e) {
                        updateDetails($table.yield(), "LineID", e.key, "PriListID", this.value);
                    }
                }
            },
            {
                name: "DisTypeSelect",
                template: "<select></select>",
                on: {
                    "change": function (e) {
                        updateDetails($table.yield(), "LineID", e.key, "DisType", this.value);
                    }
                }
            },
            {
                name: "DateF",
                template: "<input type='date' class='input-box-kernel'>",
                on: {
                    "change": function (e) {
                        updateDetails($table.yield(), "LineID", e.key, "DateF", this.value);
                    }
                }
            },
            {
                name: "DateT",
                template: "<input type='date' class='input-box-kernel'>",
                on: {
                    "change": function (e) {
                        updateDetails($table.yield(), "LineID", e.key, "DateT", this.value);
                    }
                }
            },
            {
                name: "Amount",
                template: "<input class='input-box-kernel'>",
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updateDetails($table.yield(), "LineID", e.key, "Amount", this.value);
                    }
                }
            },
            {
                name: "DisRateValue",
                template: "<input class='input-box-kernel'>",
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updateDetails($table.yield(), "LineID", e.key, "DisRateValue", this.value);
                    }
                }
            },
            {
                name: "Active",
                template: "<input type='checkbox' class='input-box-kernel'>",
                on: {
                    "click": function (e) {
                        const active = $(this).prop("checked") ? true : false;
                        updateDetails($table.yield(), "LineID", e.key, "Active", active);

                    }
                }
            }

        ]
    });
    GetingItemDis(function (glex) {
        glex.forEach(i => {
            i.DateF = i.DateF.toString().split("T")[0]
            i.DateT = i.DateT.toString().split("T")[0]
        });
        $table.bindRows(glex);
    });
    $("#add-new-item").click(function () {
        $.get("/LoyaltyProgram/GetTable", function (res) {
            $table.addRow(res);
        })
    });
    function GetingItemDis(succuss) {
        $.get("/LoyaltyProgram/GetItemDis", succuss);
    }
    $("#Update").click(function () {
        $("#item-id").val();
        $.ajax({
            url: "/LoyaltyProgram/UpdateItemdetail",
            type: "POST",
            dataType: "JSON",
            data: { data: JSON.stringify($table.yield()) },
            success: function (respones) {
                const errorMessage = new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    },
                }, respones);
                if (respones.IsApproved) {
                    errorMessage.refresh(1000)
                }
            }
        });
    });
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

