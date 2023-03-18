$(document).ready(function () {
    let __dataItems = [];
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
            "DateF",
            "DateT",
            "BuyItem",
            "Qty",
            "UomSelect",
            "DisItem",
            "DisRate",
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
                name: "Qty",
                template: "<input class='input-box-kernel'>",
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updateDetails($table.yield(), "LineID", e.key, "Qty", this.value);
                    }
                }
            },
            {
                name: "UomSelect",
                template: "<select></select>",
                on: {
                    "change": function (e) {
                        updateDetails($table.yield(), "LineID", e.key, "UomSelect", this.value);
                    }
                }
            },
            {
                name: "BuyItem",
                template: "<input class='input-box-kernel cursor'  >",
                on: {
                    "click": function (e) {
                        console.log(e.data.ID)
                        updateDetails($table.yield(), "LineID", e.key, "ID", e.data.ID);
                        bindItemDetail(e);
                    }
                }
            },
            {
                name: "DisItem",
                template: "<input class='input-box-kernel cursor' readonly>",
                on: {
                    "click": function (e) {
                        updateDetails($table.yield(), "LineID", e.key, "ID", e.data.ID);
                        BindDisitemDetail(e);
                    }
                }
            },
            {
                name: "DisRate",
                template: "<input class='input-box-kernel'>",
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updateDetails($table.yield(), "LineID", e.key, "DisRate", this.value);
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
    function BindDisitemDetail(_e) {
        console.log("_e", _e)
        let dialog = new DialogBox({
            content: {
                selector: ".itemdetailDIS_container"
            },
            caption: "Item Master Data Detail"
        });
        dialog.invoke(function () {
            const __listItems = ViewTable({
                selector: "#list-itemdetailDIS",
                keyField: "LineID",
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Code", "Barcode", "KhmerName", "EnglishName", "Uom", "Action"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down"  ></i>`,
                        on: {
                            "click": function (e) {
                                $table.updateColumn(_e.key, "DisItem", e.data.KhmerName)
                                $table.updateColumn(_e.key, "DisItemID", e.data.ItemID)
                                // updateDetails($table.yield(), "LineID", _e.key, "ID", e.data.ID);
                                updateDetails($table.yield(), "LineID", _e.key, "DisItemID", e.data.ItemID);
                                updateDetails($table.yield(), "LineID", _e.key, "DisItem", e.data.KhmerName);
                                updateDetails($table.yield(), "LineID", _e.key, "ID", _e.data.ID);
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            bindTable(__listItems);
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
    }
    function bindTable(table) {
        if (isValidArray(__dataItems)) {
            table.bindRows(__dataItems)
            searchItemMasterData(__dataItems, table, "#search-item-masters");
            $("#loading").prop("hidden", true);
        } else {
            $.ajax({
                url: "/LoyaltyProgram/Buyitemgetdis",
                type: "Get",
                dataType: "Json",
                success: function (response) {
                    __dataItems = response;
                    table.bindRows(__dataItems);
                    searchItemMasterData(__dataItems, table, "#search-item-masters");
                    $("#loading").prop("hidden", true);
                }
            });
        }
    }
    function bindItemDetail(_e) {
        console.log(_e)
        let dialog = new DialogBox({
            content: {
                selector: ".itemdetail_container"
            },
            caption: "Item Master Data Detail"
        });
        dialog.invoke(function () {
            const __listItems = ViewTable({
                selector: "#list-itemdetail",
                keyField: "LineID",
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Code", "Barcode", "KhmerName", "EnglishName", "Uom", "Action"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down"  ></i>`,
                        on: {
                            "click": function (e) {
                                $table.updateColumn(_e.key, "BuyItem", e.data.KhmerName)
                                $table.updateColumn(_e.key, "BuyItemID", e.data.ItemID)
                                $table.updateColumn(_e.key, "UomID", e.data.GuomID)
                                $table.updateColumn(_e.key, "UomSelect", e.data.UomSelect)
                                $table.updateColumn(_e.key, "UomSelect", e.data.UomSelect)
                                $table.updateColumn(_e.key, "ID", _e.data.ID)

                                updateDetails($table.yield(), "LineID", _e.key, "UomSelect", e.data.UomSelect);
                                updateDetails($table.yield(), "LineID", _e.key, "UomID", e.data.UomID);
                                // updateDetails($table.yield(), "LineID", _e.key, "ID", e.data.ID);
                                updateDetails($table.yield(), "LineID", _e.key, "BuyItemID", e.data.ItemID);
                                updateDetails($table.yield(), "LineID", _e.key, "BuyItem", e.data.KhmerName);
                                updateDetails($table.yield(), "LineID", _e.key, "ID", _e.data.ID);
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            bindTable(__listItems)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
    }
    GetingItemDis(function (glex) {
        glex.forEach(i => {
            i.DateF = i.DateF.toString().split("T")[0]
            i.DateT = i.DateT.toString().split("T")[0]

        });
        $table.bindRows(glex);
    });
    $("#add-new-item").click(function () {
        $.get("/LoyaltyProgram/GetEmptyTable", function (res) {
            $table.addRow(res);
        })
    });
    function GetingItemDis(succuss) {
        $.get("/LoyaltyProgram/GetItemBuyXqtyGetXDis", succuss);
    }
    $("#Update").click(function () {
        $("#item-id").val();
        $.ajax({
            url: "/LoyaltyProgram/UpdateBuyXqtyGetDis",
            type: "POST",
            dataType: "JSON",
            data: { data: JSON.stringify($table.yield()) },
            success: function (respones) {
                const msg = new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    },
                }, respones);
                if (respones.IsApproved) {
                    msg.refresh(1000);
                }
            }
        });
    });

    function searchItemMasterData(data, table, inputsearch) {
        $(inputsearch).on("keyup", function (e) {
            let __value = this.value.toLowerCase().replace(/\s+/, "");
            let rex = new RegExp(__value, "gi");
            let items = $.grep(data, function (item) {
                const barcode = item.Barcode ?? "";
                const name2 = item.EnglishName ?? "";
                return item.Code.toLowerCase().replace(/\s+/, "").match(rex) || item.KhmerName.toLowerCase().replace(/\s+/, "").match(rex) ||
                    name2.toLowerCase().replace(/\s+/, "").match(rex) ||
                    item.Uom.toLowerCase().replace(/\s+/, "").match(rex) ||
                    barcode.toLowerCase().replace(/\s+/, "").match(rex) ||
                    item.Cost === __value
            });
            table.bindRows(items);
        });
    }

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