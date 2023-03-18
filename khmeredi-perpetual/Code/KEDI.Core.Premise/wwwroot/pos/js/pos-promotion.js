"use strict";

function PosPromotion(PosCore) {
    if (!(this instanceof PosPromotion)) {
        return new PosPromotion(PosCore);
    }
    const __this = this, __core = PosCore;
    let __orderInfo = {
        batches: [],
        serials: [],
    };
    let redemptPointMaster = {
        ID: 0,
        PostingDate: new Date(),
        Number: 0,
        UserID: 0,
        CustomerID: 0,
        PriceListID: 0,
        LocalCurrencyID: 0,
        SysCurrencyID: 0,
        WarehouseID: 0,
        BranchID: 0,
        CompanyID: 0,
        Delete: false,
        PLCurrencyID: 0,
        PLRate: 0,
        LocalSetRate: 0,
        SeriesID: 0,
        SeriesDID: 0,
        DocTypeID: 0,
        PointRedemptions: [],
    }
    var __urls = {
        getBuyXGetXDetails: "/POS/GetBuyXGetXDetails",
        fetchLoyaltyProgram: "/POS/FetchLoyaltyProgram",
        getPointRedemptions: "/POS/GetPointRedemptions",
        postPointRedemptions: "/POS/PostPointRedemptions",
        getPointRedemptionwarehouse: "/POS/GetPointRedemptionWarehouse",
        getInvoice: "/POS/GetInvoice",
        getComboSale: "/POS/GetComboSale",
        getPromotionInfo:"/POS/GetPromotionInfo",
    };
   
    var __listViewItemOrder = __core.getListViewItemOrder();
    __core.load(function (orderInfo) {
        __orderInfo = orderInfo;
        var memberCount = __orderInfo.LoyaltyProgram.PointMembers.length;
        displayPointBadge();
        $(".point-badge").text(memberCount);
        $(".point-badge").removeClass("hidden");
        if (memberCount <= 0) {
            $(".point-badge").addClass("hidden");
        }
        // __this.bindBuyXgetXs(__orderInfo.LoyaltyProgram.BuyXGetXDetails);
        __this.bindPointMembers(__orderInfo.LoyaltyProgram.PointMembers);
        __this.bindComboSale(__orderInfo.LoyaltyProgram.ComboSales);
        
    });

    function displayPointBadge(enabled = true) {
        if (enabled) {
            $(".point-badge").removeClass("hidden");
        } else {
            $(".point-badge").addClass("hidden");
        }
    }

    $("#loyalty-program").click(function () {
        let priceListId=__orderInfo.Order.PriceListID;
        let warehouseId=__orderInfo.Order.WarehouseID;
        $.ajax({
            url:__urls.getPromotionInfo,
            type:"POST",
            data:{priceListId,warehouseId},
            success:function(orderinfo){
             __this.bindBuyXgetXs(orderinfo.LoyaltyProgram.BuyXGetXDetails);
            }
        })
        __this.bindPointMembers(__orderInfo.LoyaltyProgram.PointMembers);
        __this.bindComboSale(__orderInfo.LoyaltyProgram.ComboSales);
        $("#content-point-redempt-details").hide();
        $("#content-point-members").show();

    });
    $("#close-point-redemption").on("click", function () {
        $("#content-point-redempt-details").hide();
        $("#content-point-members").show();
        displayPointBadge();
    });

    // combo sale

    this.bindComboSale = function (res) {
        let $listComboSale = ViewTable({
            keyField: "LineID",
            selector: "#list-combo-sale",
            visibleFields: ["Name", "ItemCode", "ItemName", "QtyF", "Type", "UnitPriceF"],
            actions: [
                {
                    template: "<i class='fas fa-arrow-circle-down fa-lg fa-fw fn-sky csr-pointer'></i>",
                    on: {
                        "click": function (e) {
                            setComboSale(e.data);
                        }
                    }
                }
            ]
        });

        $listComboSale.clearRows();
        $listComboSale.bindRows(res);
        //var allitems = items;
        $("#search-promo-combo-sale").on("keyup", function () {
            var items = searchComboSale(res, this.value);
            $listComboSale.clearRows();
            $listComboSale.bindRows(items);
        });
        function setComboSale(data) {
            let isReadOnly = false,
                lineItem = createLineItem(data.ItemID, data.UomID, data.TaxGroupID);

            lineItem.LineID = data.LineID
            lineItem.PromoType = 2 // combo sale
            lineItem.PromoTypeDisplay = __orderInfo.PromoTypes[lineItem.PromoType]
            ///data.TypeEnum = {None = 0, SaleParent = 1, SaleChild = 2}
            if (data.TypeEnum === 1) {
                lineItem.ComboSaleType = 1
                isReadOnly = true
                lineItem.IsReadonly = false
                lineItem.UnitPrice = data.UnitPrice
            } else if (data.TypeEnum === 2) {
                lineItem.ComboSaleType = 2
                lineItem.IsReadonly = true
                isReadOnly = false
                lineItem.UnitPrice = 0
            }
            lineItem.Qty = lineItem.PrintQty = lineItem.BaseQty = data.Qty
            __core.sumLineItem(lineItem)
            __listViewItemOrder.updateRow(lineItem)

            if (!isReadOnly) {
                const masterRow = __listViewItemOrder.getRow(lineItem.LineID);
                $(masterRow).addClass("readonly");
            }
            if (isValidArray(data.ComboSaleDetials)) {
                data.ComboSaleDetials.forEach(i => {
                    var subLineItem = createLineItem(i.ItemID, i.UoMID, data.TaxGroupID)
                    if (data.TypeEnum === 2) {
                        subLineItem.UnitPrice = i.UnitPrice
                        subLineItem.IsReadonly = false
                    } else if (data.TypeEnum === 1) {
                        subLineItem.UnitPrice = 0
                        subLineItem.IsReadonly = true
                    }
                    subLineItem.LineID = i.LineID
                    lineItem.PromoType = 2 // combo sale
                    subLineItem.PromoTypeDisplay = __orderInfo.PromoTypes[lineItem.PromoType]
                    subLineItem.LinePosition = 1 // LinePosition -> Children
                    subLineItem.Qty = subLineItem.PrintQty = subLineItem.BaseQty = i.Qty
                    __core.sumLineItem(subLineItem)
                    subLineItem.IsReadonly = isReadOnly
                    subLineItem.ParentLineID = lineItem.LineID
                    __core.addSubLineItem(subLineItem, `<i class='fas fa-gift' style="color: #FF7C08"></i>`, isReadOnly)
                })
            }
        }
        function searchComboSale(items, keyword) {
            let input = keyword.replace(/\s/g, '');
            let regex = new RegExp(input, "i");
            return $.grep(items, function (item) {
                return regex.test(item.Name) || regex.test(item.ItemName) || regex.test(item.ItemCode) || regex.test(item.Type)
                    || regex.test(item.UoM);
            });
        }
    }
    this.bindPointMembers = function (members) {
        var _pointRedempts = new Array(), _member = {};
        let $listPointRedempts = ViewTable({
            keyField: "LineID",
            selector: "#list-point-redemptions",
            dataSynced: true,
            paging: {
                pageSize: 3
            },
            visibleFields: ["Code", "Title", "PointQty", "DateTo", "Factor", "Redeemed"],
            columns: [
                {
                    name: "DateTo",
                    dataType: "date",
                    dataFormat: "MM/DD/YYYY"
                },
                {
                    name: "Factor",
                    template: '<input type="number" min="1">',
                    on: {
                        "keyup blur": function (e) {
                            var redeemedPoints = 0;
                            for (let j = 0; j < e.data.PointItems.length; j++) {
                                if (e.data.PointItems[j].PointRedemptID == e.key) {
                                    let item = e.data.PointItems[j];
                                    let factor = e.data.Factor;
                                    item.Qty = item.BaseItemQty * factor;
                                }
                            }
                            _pointRedempts = $.grep($listPointRedempts.yield(), function (pr) {
                                if (pr.Redeemed) {

                                    redeemedPoints += (pr.PointQty) * pr.Factor;
                                }

                                return pr.Redeemed;
                            });
                            setPointItems(_pointRedempts);
                            setMemberPoint(_member.OutstandPoint, redeemedPoints);
                        }
                    }
                },
                {
                    name: "Redeemed",
                    template: "<input type='checkbox'>",
                    on: {
                        "change": function (e) {

                            var redeemedPoints = 0;
                            var warehouse = parseInt($('#warehouse-id').val());
                            if (isNaN(warehouse)) {
                                __core.dialog("Selection", "Please select Warehouse.")
                                    .confirm(function () {
                                        this.meta.shutdown();
                                    });
                            }
                            else {
                                _pointRedempts = $.grep($listPointRedempts.yield(), function (pr) {

                                    if (pr.Redeemed) {

                                        redeemedPoints += (pr.PointQty) * pr.Factor;
                                    }
                                    return pr.Redeemed;
                                });
                                setPointItems(_pointRedempts);
                                setMemberPoint(_member.OutstandPoint, redeemedPoints);
                            }

                        }
                    }
                },

            ]
        });

        let $listPointMembers = ViewTable({
            keyField: "ID",
            selector: "#list-point-members",
            visibleFields: ["Code", "Name", "OutstandPoint", "Phone", "Email"],
            actions: [
                {
                    template: "<i class='fas fa-arrow-circle-right fn-dark-pink fa-lg csr-pointer'></i>",
                    on: {
                        "click": function (e) {
                            _member = e.data;
                            var redeemedPoints = 0;
                            setInvoice();
                            var warehouse = parseInt($('#warehouse-id').val());
                            setPointMember($listPointRedempts, e.data, warehouse);
                            $("#warehouse-id").on("change", function () {
                                var warehouse = parseInt($('#warehouse-id').val());
                                setPointMember($listPointRedempts, e.data, warehouse);
                            });
                            $("#btn-reset-point").off("click").on("click", function () {
                                var warehouse = parseInt($('#warehouse-id').val());
                                setPointMember($listPointRedempts, e.data, warehouse);
                            });
                            $("#btn-redeem-point").off("click").on("click", function () {
                                var prs = $.grep($listPointRedempts.yield(), function (pr) {
                                    if (pr.Redeemed) {
                                        redeemedPoints += (pr.PointQty) * pr.Factor;
                                    }
                                    return pr.Redeemed;
                                });
                                var total = 0;
                                var pointItems = new Array(), Qty = 0, Instock = 0;
                                var warehouse = parseInt($('#warehouse-id').val());
                                for (var i = 0; i < prs.length; i++) {
                                    let item = prs[i];
                                    pointItems = pointItems.concat(prs[i].PointItems);
                                    redeemedPoints = item.PointQty * item.Factor;
                                    total += redeemedPoints;
                                }
                                for (let i = 0; i < pointItems.length; i++) {
                                    Qty = pointItems[i].Qty;
                                    Instock = pointItems[i].Instock;
                                }
                                if (isNaN(warehouse)) {
                                    __core.dialog("Selection", "Please select Warehouse.")
                                        .confirm(function () {
                                            this.meta.shutdown();
                                        });
                                }
                                else if (!isValidArray(prs)) {
                                    __core.dialog("Selection", "Please select at least one redemption.")
                                        .confirm(function () {
                                            this.meta.shutdown();
                                        });
                                }
                                else if (_member.OutstandPoint < total) {
                                    __core.dialog("Warning", "Outstanding Points can not be smaller than Redeemed points.")
                                        .confirm(function () {
                                            this.meta.shutdown();
                                        });
                                }
                                let isOutOfStock = false;
                                pointItems.forEach(i => {
                                    if (i.Process != "Standard" && i.Instock == 0) {
                                        isOutOfStock = true;
                                    }
                                })
                                if (isOutOfStock) {
                                    if (i.Process != "Standard") {
                                        __core.dialog("Warning", "This Item Out of Stock.")
                                            .confirm(function () {
                                                this.meta.shutdown();
                                            });
                                    }
                                }
                                else {
                                    submitPointMemberInfo(e.data, prs, warehouse);
                                }
                            });
                        }
                    }
                }
            ]
        });
        $listPointMembers.clearRows();
        $listPointMembers.bindRows(members);
        var allMembers = members;
        $("#search-point-members").on("keyup", function () {
            var _members = searchPointMembers(allMembers, this.value);
            $listPointMembers.clearRows();
            $listPointMembers.bindRows(_members);
        });
    }

    function setPointItems(pointRedempts) {
        var pointItems = new Array(), sumItemQty = 0;
        if (isValidArray(pointRedempts)) {
            for (let i = 0; i < pointRedempts.length; i++) {
                pointItems = pointItems.concat(pointRedempts[i].PointItems);
            }
            for (let i = 0; i < pointItems.length; i++) {
                sumItemQty += pointItems[i].Qty;
            }
        }
        var $pointItems = ViewTable({
            keyField: "LineID",
            selector: "#list-point-items",
            visibleFields: ["ItemCode", "ItemName", "Qty", "UomName", "Instock"],
            paging: {
                enabled: false
            }
        });
        $pointItems.clearRows();
        $pointItems.bindRows(pointItems);
        $("#total-items").text(pointItems.length);
        $("#total-item-qty").text(sumItemQty);
    }

    function setMemberPoint(totalPoints, redeemedPoints) {
        var currentPoints = 0, pointRatio = 0, className = "";
        var $points = $("#pm-current-points"),
            $pointPercent = $("#pm-points-percent"),
            $redeemedPoints = $("#pm-redeemed-points");
        currentPoints = parseFloat(totalPoints) - parseFloat(redeemedPoints);
        pointRatio = (currentPoints / totalPoints) * 100;
        className = "pc-circle p" + parseInt(pointRatio);
        if (pointRatio >= 50) {
            className += " green";
        } else if (pointRatio >= 0) {
            className += " orange";
        }
        $redeemedPoints.addClass("fn-red").text(-redeemedPoints);
        $points.text(currentPoints.toFixed(2));
        $points.parent()[0].className = className;
        $pointPercent.text((pointRatio <= -100) ? "-99..%" : parseInt(pointRatio) + "%");
        if (pointRatio < 0) {
            $points.addClass("fn-red");
            $pointPercent.addClass("fn-red");
        } else {
            $points.removeClass("fn-red");
            $pointPercent.removeClass("fn-red");
        }
    }

    //add new 
    function setPointMember($listPointRedempts, member, warehouse) {
        displayPointBadge(false);
        $("#content-point-redempt-details").show();
        $("#content-point-members").hide();
        $("#pm-code").text(member.Code);
        $("#pm-name").text(member.Name);
        $("#pm-phone").text(member.Phone);
        $("#pm-email").text(member.Email);
        $("#pm-outstand-points").text(member.OutstandPoint);
        $("#pm-redeemed-points").text(member.OutstandPoint);
        $("#pm-current-points").text(member.OutstandPoint);
        var _pointRedempts = [];
        $.post(__urls.getPointRedemptionwarehouse, {
            customerId: member.ID,
            warehouseId: warehouse,
            priceListId: __orderInfo.Order.PriceListID
        }, function (pointRedempts) {
            if (pointRedempts == 0) {
                $listPointRedempts.clearRows();
            }
            else {

                $listPointRedempts.bindRows(pointRedempts);
                setPointItems([]);
                $("#search-point-redemptions").on("keyup", function () {
                    _pointRedempts = searchPointRedempts(pointRedempts, this.value);
                    $listPointRedempts.clearRows();
                    $listPointRedempts.bindRows(_pointRedempts);
                });
            }

        });
        setMemberPoint(member.OutstandPoint, 0);
    }

    //add new
    function submitPointMemberInfo(member, prs, whId) {
        __core.loadScreen();
        redemptPointMaster.PointRedemptions = prs;
        redemptPointMaster.PriceListID = __orderInfo.Order.PriceListID;
        redemptPointMaster.PLCurrencyID = __orderInfo.Order.PLCurrencyID;
        redemptPointMaster.PLRate = __orderInfo.Order.PLRate;
        redemptPointMaster.WarehouseID = whId;
        displayPointBadge(false);
        $("#pm-code").text(member.Code);
        $("#pm-name").text(member.Name);
        $("#pm-phone").text(member.Phone);
        $("#pm-email").text(member.Email);
        $("#pm-outstand-points").text(member.OutstandPoint);
        $("#pm-redeemed-points").text(member.OutstandPoint);
        $("#pm-current-points").text(member.OutstandPoint);
        $.post(__urls.postPointRedemptions, {
            customerId: member.ID,
            point: JSON.stringify(redemptPointMaster),
            batch: JSON.stringify(__orderInfo.batches),
            serial: JSON.stringify(__orderInfo.serials),
        }, function (res) {
            ViewMessage({
                summary: {
                    selector: ".display-message"
                }
            }, res);
            if (res.IsSerail) {
                __core.loadScreen(false);
                const serial = SerialTemplate({
                    data: {
                        serials: res.Data,
                    }
                });
                serial.serialTemplate();
                const seba = serial.callbackInfo();
                __orderInfo.serials = seba.serials;
            } else if (res.IsBatch) {
                __core.loadScreen(false);
                const batch = BatchNoTemplate({
                    data: {
                        batches: res.Data,
                    }
                });
                batch.batchTemplate();
                const seba = batch.callbackInfo();
                __orderInfo.batches = seba.batches;
            } else if (res.IsRejected) {
                __core.loadScreen(false);
            } else {
                __core.loadScreen();
                $("#content-point-redempt-details").hide();
                $("#content-point-members").show();
                location.reload();
            }
        });
        setMemberPoint(member.OutstandPoint, 0);
    }

    function searchPointRedempts(items, keyword) {
        let input = keyword.replace(/\s/g, '');
        let regex = new RegExp(input, "i");
        return $.grep(items, function (mb) {
            return regex.test(mb.Code) || regex.test(mb.Title) || regex.test(mb.PointQty);
        });
    }

    function searchPointMembers(items, keyword) {
        let input = keyword.replace(/\s/g, '');
        let regex = new RegExp(input, "i");
        return $.grep(items, function (mb) {
            return regex.test(mb.Code) || regex.test(mb.Name) || regex.test(mb.Points) || regex.test(mb.Phone) || regex.test(mb.Email);
        });
    }

    // addd new 
    function setInvoice() {
        $.post(__urls.getInvoice, {
        }, function (res) {
            if (isValidArray(res)) {
                $("#txtInvoice").val(res[0].Name);
                $("#next_number").val(res[0].NextNo);
            }
        });
    }

    this.bindBuyXgetXs = function (items) {
        let $listBuyXgetX = ViewTable({
            keyField: "LineID",
            selector: "#list-buyxgetx-items",
            visibleFields: ["ProCode", "ItemCode", "BuyItemName", "BuyQty", "UoM", "Item", "GetItemCode", "GetItemName", "GetQty", "GetUomName"],
            actions: [
                {
                    template: "<i class='fas fa-arrow-circle-down fa-lg fa-fw fn-sky csr-pointer'></i>",
                    on: {
                        "click": function (e) {
                            setBuyXgetXs(e.data);
                        }
                    }
                }
            ]
        });

        $listBuyXgetX.clearRows();
        $listBuyXgetX.bindRows(items);
        var allitems = items;
        $("#search-promo-buyxgetxs").on("keyup", function () {
            var items = searchPromoItems(allitems, this.value);
            $listBuyXgetX.clearRows();
            $listBuyXgetX.bindRows(items);
        });
    }

    function searchPromoItems(items, keyword) {
        let input = keyword.replace(/\s/g, '');
        let regex = new RegExp(input, "i");
        return $.grep(items, function (item) {
            return regex.test(item.ProCode) || regex.test(item.ItemCode) || regex.test(item.BuyItemName) || regex.test(item.UoM)
                || regex.test(item.GetItemCode) || regex.test(item.GetItemName) || regex.test(item.GetUomName);
        });
    }

    function setBuyXgetXs(buyxGetxItem) {
        var lineItem = createLineItem(buyxGetxItem.BuyItemID, buyxGetxItem.ItemUomID);
        var subLineItem = createLineItem(buyxGetxItem.GetItemID, buyxGetxItem.GetUomID);
        lineItem.LineID = Date.now().toString();
        lineItem.PromoType = 1;
        lineItem.PromoTypeDisplay = __orderInfo.PromoTypes[lineItem.PromoType];
        lineItem.Qty = lineItem.PrintQty = lineItem.BaseQty = buyxGetxItem.BuyQty;
        __core.sumLineItem(lineItem);
        subLineItem.LineID = (Date.now() + 1).toString();
        subLineItem.PromoType = 1; // PromotionType -> BuyXgetX
        subLineItem.LinePosition = 1; // LinePosition -> Children
        subLineItem.PromoTypeDisplay = __orderInfo.PromoTypes[lineItem.PromoType];
        subLineItem.Qty = subLineItem.PrintQty = subLineItem.BaseQty = buyxGetxItem.GetQty;
        subLineItem.UnitPrice = 0;
        __core.sumLineItem(subLineItem);
        subLineItem.IsReadonly = true;
        subLineItem.ParentLineID = lineItem.LineID;
        if (__listViewItemOrder.find(lineItem) == undefined) {
            __listViewItemOrder.updateRow(lineItem);
            __core.addSubLineItem(subLineItem, "<i class='fas fa-gift fn-red'></i>", true);
        }
    }

    //Create object order detail by sale item from pos-core.
    function createLineItem(itemId, uomId, disabledUoM = true) {
        var saleItem = __core.findSaleItemByItemID(itemId);
        return __core.createLineItem(saleItem.ID, uomId);
    }

    /// End QRCode Order ///    
    function isValidJSON(value) {
        return !isEmpty(value) && value.constructor === Object && Object.keys(value).length > 0;
    }

    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }

    function clone(prop, deep) {
        if (deep) {
            return $(__template[prop]).clone(deep);
        }
        return $(__template[prop]).clone();
    }

    function findInArray(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
}