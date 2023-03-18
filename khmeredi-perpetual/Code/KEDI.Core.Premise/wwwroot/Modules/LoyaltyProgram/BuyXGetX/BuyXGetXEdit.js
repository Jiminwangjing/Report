
$(document).ready(function () {
    var __BuyXGetXDetailsView = new Array(),
        __config = {
            master: {
                keyField: "BuyItemID",
            },
            detail: {
                keyField: "BuyItemID"
            }
        };
    var __Promo = JSON.parse($("#buyxgetx").text());
    $("#choose-items").on("click", function () {
        // let pricelist_id = parseInt($('#Pricelist').val());
        let pricelist_id = $(".selectbox.pricelist").val();
        chooseItem(pricelist_id, {}, false);
    });
    $.ajax({
        url: '/LoyaltyProgram/GetPriceList',
        type: 'GET',
        dataType: 'JSON',
        success: function (price_lists) {
            $.each(price_lists, function (i, price_list) {
                if (__Promo.BuyXGetX.PriListID === price_list.ID)
                    $("#Pricelist").append(" <option value=" + price_list.ID + " selected>" + price_list.Name + "</option>");
                else
                    $("#Pricelist").append(" <option value=" + price_list.ID + ">" + price_list.Name + "</option>");
            });
        }
    });
    var $listDetails = ViewTable({
        keyField: "LineID",
        selector: "#list-BuyXGetXDetail",
        visibleFields: ["ProCode", "ItemCode", "BuyItemName", "BuyQty", "ItemUoMs", "Item", "GetItemCode", "GetItemName", "GetQty", "PromoUoMs"],
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: false
        },
        columns: [{
            name: "ProCode",
            template: "<input>",
            on: {
                "keyup": function (e) {
                    // updateItem(e.key, "ProCode", this.value);
                    updateItem(__Promo.BuyXGetXDetailModelView, "LineID", e.key, "ProCode", this.value);

                }
            }
        },
        {
            name: "ItemUoMs",
            template: "<select></select>",
            on: {
                "change": function (e) {
                    // updateItem(e.key, "ItemUomID", this.value);
                    updateItem(__Promo.BuyXGetXDetailModelView, "LineID", e.key, "ItemUomID", this.value);


                }
            }
        },
        {
            name: "BuyQty",
            template: "<input type='number'>",
            on: {
                "keyup": function (e) {
                    $(this).asNumber();
                    // updateItem(e.key, "BuyQty", this.value);
                    updateItem(__Promo.BuyXGetXDetailModelView, "LineID", e.key, "BuyQty", this.value);

                }
            }
        },
        {
            name: "GetQty",
            template: "<input type='number'>",
            on: {
                "keyup": function (e) {
                    $(this).asNumber();
                    // updateItem(e.key, "GetQty", this.value);
                    updateItem(__Promo.BuyXGetXDetailModelView, "LineID", e.key, "GetQty", this.value);

                }
            }
        },
        {
            name: "PromoUoMs",
            template: "<select></select>",
            on: {
                "change": function (e) {
                    // updateItem(e.key, "GetUomID", this.value);
                    updateItem(__Promo.BuyXGetXDetailModelView, "LineID", e.key, "GetUomID", this.value);
                }
            }
        }
        ],
        actions: [
            {
                template: '<i class="fa fa-trash" style="text-align:center;color:red"></i>',
                on: {
                    "click": function (e) {
                        let $dialog = new DialogBox({
                            type: "ok-cancel",
                            caption: "Delete Item",
                            content: "Are you sure you want to delete this?"
                        });
                        $dialog.confirm(function () {
                            $.post("/LoyaltyProgram/DeleteItem", { itemId: e.data.ID },
                                function () {
                                    $listDetails.removeRow(e.data.LineID, true);
                                    __Promo.BuyXGetXDetailModelView = __Promo.BuyXGetXDetailModelView.filter(i => i.BuyItemID !== e.data.BuyItemID);
                                    // $listDetails.removeRow(e.data.LineID, true);
                                    // __Promo.BuyXGetXDetailModelView = __Promo.BuyXGetXDetailModelView.filter(i => i.BuyItemID !== e.data.BuyItemID);
                                    location.reload();
                                });
                            $dialog.shutdown();
                        });
                    }
                }
            },
            {
                template: '<i class="fas fa-plus-circle" id="choose-itemPro"></i>',
                on: {
                    "click": function (e) {
                        // let pricelist_id = parseInt($('#Pricelist').val());
                        let pricelist_id = $(".selectbox.pricelist").val();
                        chooseItem(pricelist_id, e, true);
                    }
                }
            },
        ],
    });
    $listDetails.bindRows(__Promo.BuyXGetXDetailModelView);
    function chooseItem(plId, _e, isPromotion = false) {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    }
                }
            },
            content: {
                selector: "#choose_Item"
            }
        });
        dialog.invoke(function () {
            let $listItemDetail = ViewTable({
                keyField: "LineID",
                selector: dialog.content.find("#list-items"),
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["ItemCode", "ItemName", "UoM"],
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down fa-lg csr-pointer'></i>",
                        on: {
                            "click": function (e) {
                                if (isPromotion) {
                                    $listDetails.updateColumn(_e.key, "GetItemName", e.data.ItemName);
                                    $listDetails.updateColumn(_e.key, "GetItemCode", e.data.ItemCode);
                                    $listDetails.updateColumn(_e.key, "PromoUoMs", e.data.PromoUoMs);
                                    $listDetails.updateColumn(_e.key, "PromoUoMs", e.data.PromoUoMs);
                                    $listDetails.updateColumn(_e.key, "GetItemID", e.data.BuyItemID);
                                    $listDetails.updateColumn(_e.key, "GetUomID", e.data.GetUomID);
                                    // updateItem(_e.key, "GetItemCode", e.data.ItemCode);
                                    // updateItem(_e.key, "GetItemName", e.data.ItemName);
                                    // updateItem(_e.key, "GetItemID", e.data.BuyItemID);
                                    // updateItem(_e.key, "GetUomID", e.data.GetUomID);
                                    updateItem(__Promo.BuyXGetXDetailModelView, "LineID", e.key, "GetItemCode", e.data.ItemCode);
                                    updateItem(__Promo.BuyXGetXDetailModelView, "LineID", e.key, "GetItemName", e.data.ItemName);
                                    updateItem(__Promo.BuyXGetXDetailModelView, "LineID", e.key, "GetItemID", e.data.BuyItemID);
                                    updateItem(__Promo.BuyXGetXDetailModelView, "LineID", e.key, "GetUomID", e.data.GetUomID);
                                    dialog.shutdown();
                                }
                                else {
                                    addItem(e.data, isPromotion, $listDetails);
                                    dialog.shutdown();
                                }
                            }
                        }
                    }
                ]
            });
            getBuyXGetXDetails(plId, function (res) {
                $listItemDetail.bindRows(res);
                let $inputSearch = dialog.content.find("#search-item");
                $inputSearch.on("keyup", function () {
                    $listItemDetail.clearRows();
                    var searcheds = searchItems(res, this.value);
                    $listItemDetail.bindRows(searcheds);
                });
            })
        });
    }

    function searchItems(res, keyword = "") {
        let input = keyword.replace(/\s+/g, '');
        let regex = new RegExp(input, "ig");
        var filtereds = $.grep(res, function (item, i) {
            return regex.test(item.ItemName.replace(/\s+/g, ''))
                || regex.test(item.ItemCode.replace(/\s+/g, ''))
                || regex.test(item.UoM.replace(/\s+/g, ''));

        });
        return filtereds;
    }

    function getBuyXGetXDetails(plId, success) {
        $.get("/LoyaltyProgram/GetBuyXGetXDetails", { plId }, success);
    }
    function addItem(item, isPromotion, $listDetails) {
        if (isValidJSON(item)) {
            $.get("/LoyaltyProgram/CreateBuyXGetXModel", { itemId: item.BuyItemID }, function (resp) {
                __Promo.BuyXGetXDetailModelView.push(resp);
                $listDetails.addRow(resp);
                // var isExisted = __Promo.BuyXGetXDetailModelView.some((_item) => _item[__config.detail.keyField] == item[__config.detail.keyField]);
                // if (!isExisted) {
                //     __Promo.BuyXGetXDetailModelView.push(resp);
                //     $listDetails.addRow(resp);
                // }
                // else {
                //     if (isPromotion) {
                //         $listDetail.updateRow(resp);

                //     }
                // }
            });
        };
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function updateItem(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue)
                    i[prop] = propValue
            })
        }
    }
    // function updateItem(itemId, keyField, keyValue) {
    //     $.grep(__Promo.BuyXGetXDetailModelView, function (item) {
    //         if (item.LineID == itemId) {
    //             item[keyField] = keyValue;
    //         }
    //     });
    // }
    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }
    $("#add-item").on("click", function (e) {
        __Promo.BuyXGetX.Code = $("#promocode-input").val();
        __Promo.BuyXGetX.Name = $("#promoname-input").val();
        __Promo.BuyXGetX.Active = $("#active").prop("checked") ? true : false;
        __Promo.BuyXGetX.DateF = $("#datef").val();
        __Promo.BuyXGetX.DateT = $("#datet").val();
        __Promo.BuyXGetX.PriListID = $(".selectbox.pricelist").val();
        __Promo.BuyXGetXDetails = __Promo.BuyXGetXDetailModelView;
        $.ajax({
            url: "/LoyaltyProgram/SubmitPromotion",
            type: "POST",
            data: $.antiForgeryToken({ data: JSON.stringify(__Promo) }),
            success: function (data) {
                if (data.IsApproved) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        }
                    }, data.Model);
                    location.href = "/LoyaltyProgram/BuyXGetX";
                }
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, data);
                $(window).scrollTop(0);
            }
        });

    });
});





