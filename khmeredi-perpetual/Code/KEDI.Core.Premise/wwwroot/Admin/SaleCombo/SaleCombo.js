"use strict"
let salecombo_details = [];
let master = [];
let _priceList = 0;
let getitem = [];
let plId = 0;
$(document).ready(function () {
    var itemmaster = {
        BID: 0,
        ItemID: 0,
        UserID: 0,
        UomID: 0,
        PostingDate: "",
        Active: 0,
        Type: 0,
    }
    master.push(itemmaster);
    var getitemdis = {
        ID: 0,
        DisItemID: 0,
        Code: "",
        Name: "",
        DateF: "",
        DateT: "",
        Active: 0
    }
    getitem.push(getitemdis);
    $("input[type='date']").on("change", function () {
        this.setAttribute(
            "data-date",
            moment(this.value, "YYYY-MM-DD")
                .format(this.getAttribute("data-date-format"))
        )
    });
    $("#show-list-itemster").click(function () {
        $.ajax({
            url: "/LoyaltyProgram/GetItemMasters",
            type: "Get",
            data: { ID: plId },
            dataType: "Json",
            success: function (response) {
                bindItemMaster(response);
            }
        });
    });
    function bindItemMaster(res) {
        let dialog = new DialogBox({
            content: {
                selector: ".itemmaster_containers"
            },
            caption: "Item Master Data"
        });
        dialog.invoke(function () {
            const __listComboItemsMster = ViewTable({
                keyField: "ItemID",
                selector: "#list-itemmater",
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Code", "Barcode", "UomSelect", "KhmerName", "EnglishName", "Active"],
                columns: [

                    {
                        name: "UomSelect",
                        template: "<select></select>",
                        on: {
                            "change": function (e) {
                            }
                        }
                    }
                ],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down hover"  ></i>`,
                        on: {
                            "click": function (e) {
                                $("#code").val(e.data.Code);
                                $("#item-id").val(e.data.KhmerName);
                                $("#uom-name").val(e.data.Uom);
                                $("#uom-id").val(e.data.UomID);
                                $("#itemID").val(e.data.ItemID);
                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listComboItemsMster.bindRows(res)
            searchItemMasterData(res, __listComboItemsMster, "#search-item-masters");
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    //DETAIL
    function bindItemDetail(res) {
        let dialog = new DialogBox({
            content: {
                selector: ".itemdetail_container"
            },
            caption: "Item Master Data Detail"
        });
        dialog.invoke(function () {
            const __listComboItems = ViewTable({
                selector: "#list-itemdetail",
                keyField: "LineID",
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Code", "Barcode", "KhmerName", "EnglishName", "UomSelect", "Action"],
                columns: [

                    {
                        name: "UomSelect",
                        template: "<select></select>",
                        on: {
                            "change": function (e) {
                            }
                        }
                    }
                ],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down hover"></i>`,
                        on: {
                            "click": function (e) {
                                __chooseItemDetail.addRow(e.data);
                            }
                        }
                    }
                ]
            });
            __listComboItems.bindRows(res)
            searchItemMasterData(res, __listComboItems, "#search-item-masters")
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    const __chooseItemDetail = ViewTable({
        selector: "#list-detail",
        keyField: "LineID",
        paging: {
            pageSize: 10,
            enabled: true
        },
        visibleFields: ["Code", "KhmerName", "UomSelect", "Qty", "Active"],
        columns: [
            {
                name: "UomSelect",
                template: "<select></select>",
                on: {
                    "change": function (e) {
                        updateDetails(__chooseItemDetail.yield(), "LineID", e.key, "UomSelect", this.value);
                    }
                }
            },
            {
                name: "Qty",
                template: `<input id='qty' autocomplete="off">`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updateDetails(__chooseItemDetail.yield(), "LineID", e.key, "Qty", this.value);
                    }
                }
            }
        ],
        actions: [
            {
                template: "<a asp-action='RemoveItemDetail' ><i title='Remove' class='fas fa-trash-alt fa-lg text-danger csr-pointer'></i></a>",
                on: {
                    "click": function (e) {
                        let $dialog = new DialogBox({
                            type: "ok-cancel",
                            caption: "Delete Item",
                            content: "Are you sure you want to delete this?"
                        });
                        if (e.data.ID > 0) {
                            $dialog.confirm(function () {
                                $.post("/LoyaltyProgram/DeleteComboSaleDetail", { id: e.data.ID }, function (res) {
                                    if (!res.Error) {
                                        __chooseItemDetail.removeRow(e.key)
                                        $dialog.shutdown();
                                    } else {
                                        $dialog.shutdown();
                                        new DialogBox({
                                            caption: "Error deleting Item",
                                            content: res.Message
                                        });
                                    }
                                })
                            });
                        } else {
                            $dialog.confirm(function () {
                                __chooseItemDetail.removeRow(e.key)
                                $dialog.shutdown();
                            });
                        }
                    }
                }
            }
        ]
    });
    $("#choseitem-detail").click(function () {
        $.ajax({
            url: "/LoyaltyProgram/GetItemDetailspl",
            type: "Get",
            data: { ID: plId },
            dataType: "Json",
            success: function (response) {
                bindItemDetail(response);
            }
        });
    });
    $("#price-list").change(function () {
        plId = this.value;
    });
    function __getDis() {
        $("#codedis").keyup(function () {
            var code = $("#codedis").val();
        })
    }
    $("#codedis").keyup(function (e) {
        updateDetails(__getDis(), "ID", e.key, "Code", this.value);
    })
    /// submit data ///
    $("#submit-item").on("click", function () {
        const item_master = master[0];
        item_master.BID = item_master.BID;
        item_master.Barcode = $("#item-barcode").val();
        item_master.PostingDate = $("#post-date").val();
        item_master.Active = $("#active").prop("checked");
        item_master.Code = $("#code").val();
        item_master.Type = $("#set-type").val();
        item_master.PriListID = $("#price-list").val();
        item_master.UomID = $("#uom-id").val();
        item_master.Uom = $("#uom-id").val();
        item_master.ItemID = $("#itemID").val();
        item_master.ComboDetails = __chooseItemDetail.yield().length == 0 ? new Array() : __chooseItemDetail.yield();
        $("#loading").prop("hidden", false);
        let dialogSubmit = new DialogBox({
            content: "Are you sure you want to save the item?",
            type: "yes-no",
            icon: "warning"
        });
        dialogSubmit.confirm(function () {
            $.ajax({
                url: "/LoyaltyProgram/CreateSaleCombo",
                type: "POST",
                data: $.antiForgeryToken({ data: JSON.stringify(item_master) }),
                success: function (data) {
                    $("#item-barcode").focus();
                    $("#qty").focus();
                    if (data.Model.IsApproved) {
                        new ViewMessage({
                            summary: {
                                selector: "#error-summary"
                            }
                        }, data.Model).refresh(1500);
                    }
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        }
                    }, data.Model);
                    $(window).scrollTop(0);
                }
            });
            this.meta.shutdown();
        });
    });

    const listComboSales = ViewTable({
        selector: "#list-combo-sale",
        keyField: "ID",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: true
        },
        visibleFields: ["DateFormat", "Barcode", "ItemName1", "ItemName2", "UoMName", "TypeDisplay", "PriceListName", "Creator", "Active"],
        columns: [

            {
                name: "Active",
                template: `<input disabled type="checkbox" />`,
            }
        ]
    });
    $.get("/LoyaltyProgram/GetSaleCombos", function (res) {
        if (res.length > 0) {
            listComboSales.bindRows(res);
            $("#txtSearch-item-copy").on("keyup", function () {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let items = $.grep(res, function (item) {
                    return item.PostingDate.toLowerCase().replace(/\s+/, "").includes(__value) || item.Barcode.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                        || item.ItemName2.toLowerCase().replace(/\s+/, "").includes(__value) || item.ItemName1.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                        || item.UoMName.toLowerCase().replace(/\s+/, "").includes(__value) || item.TypeDisplay.toLowerCase().replace(/\s+/, "").includes(__value)
                        || item.PriceListName.toLowerCase().replace(/\s+/, "").includes(__value) || item.Creator.toLowerCase().replace(/\s+/, "").includes(__value);
                });
                listComboSales.bindRows(items);
            });
        }
    })

    $("#searchtoupdate").on("keypress", function (e) {
        if (e.which === 13) {
            $.ajax({
                url: "/LoyaltyProgram/SearchSaleCombo",
                data: { barCode: $("#searchtoupdate").val().toString().trim() },
                success: function (res) {
                    bindSearchData(res);
                }
            });
        }
    });
    function bindSearchData(data) {
        master[0] = data;
        plId = data.PriListID;
        $("#item-barcode").val(data.Barcode);
        $("#item-id").val(data.ItemName1);
        setDate("#post-date", data.PostingDate.toString().split("T")[0]);
        $("#active").prop("checked", data.Active);
        $("#code").val(data.Code);
        $("#set-type").val(data.Type);
        $("#price-list").val(data.PriListID);
        $("#uom-id").val(data.UomID);
        $("#uom-name").val(data.UoMName);
        $("#itemID").val(data.ItemID);
        __chooseItemDetail.bindRows(data.ComboDetails);
        //$("#choseitem-detail").prop("hidden", true);
    }
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
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function updateDetails(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue) {
                    i[prop] = propValue;
                }
            });
        }
    }
    function setDate(selector, date_value) {
        var _date = $(selector);
        _date[0].valueAsDate = new Date(date_value);
        _date[0].setAttribute(
            "data-date",
            moment(_date[0].value)
                .format(_date[0].getAttribute("data-date-format"))
        );
    }
});
