"use strict";
$(document).ready(function () {
    let __carousel = new Carousel({
        container: "#carousel-container"
    });
    var __setting = {
        idleTimeout: 0,
        timer: undefined,
        carousel: __carousel
    };

    var __secondScreen = new SecondScreen();
    __secondScreen.createHubConnection(function () {
        this.on("LoadOrderInfo", function (orderInfo, isOrderOnly) {
            let _orderInfo = JSON.parse(orderInfo);
            __secondScreen.mergeConfig({ orderOnly: isOrderOnly });
            __secondScreen.mergeOrderInfo(_orderInfo);
            $("#company-logo").attr("src", "/images/company/" + _orderInfo.Company.Logo);
            $("#branch-name").text(_orderInfo.Branch.Name);
            __secondScreen.switchPanelViewMode();
            if (_orderInfo.Setting.SlideShow) {
                __setting.idleTimeout = _orderInfo.Setting.TimeOut;
                __setting.carousel.build(_orderInfo.SlideImageNames);
                __setting.carousel.setDisplayTimeout(__setting.idleTimeout);
            }
        });

        this.on("ReceiveOrder", function (order, table) {
            __secondScreen.setOrder(order, table);
            // __setting.idleTimeout = 30000;
            __setting.carousel.hide();
            __setting.carousel.setDisplayTimeout(__setting.idleTimeout);
        });
        this.on("ChangeViewMode", function (setting) {
            let settingModel = {};
            settingModel.Setting = setting;
            __secondScreen.mergeOrderInfo(settingModel);
            __secondScreen.switchPanelViewMode();
        });
        this.on("ClearOrder", function () {
            __secondScreen.clearCurrentOrder();
        });
    });
});

function SecondScreen(config = {}) {
    if (!(this instanceof SecondScreen)) {
        return new SecondScreen(config);
    }

    const __this = this;
    const __config = {
        orderOnly: false,
        master: {
            keyField: ""
        },
        detail: {
            keyField: "",
            fieldName: "OrderDetail"
        },
        services: [],
        events: {
            "load": []
        }
    }
    const __template = {
        div: document.createElement("div"),
        i: document.createElement("i")
    }

    var __orderInfo = {
        Setting: {
            ItemPageSize: 12
        }
    },
        __itemListview = "#item-listview",
        __wrapGridItem = "#group-item-gridview .wrap-grid",
        __loadscreen = "#loadscreen";

    this.gridViewPaging = DataPaging({
        container: "#pagination-list",
        keyField: "ID",
        pageSize: __orderInfo.Setting.ItemPageSize,
        startIndexSize: 4
    });

    this.listViewOrder = ViewTable({
        selector: __itemListview,
        keyField: "LineID",
        visibleFields: ["Code", "KhmerName", "UnitPrice", "Qty", "ItemUoMs", "DiscountRate", "TaxGroups", "Total"],
        paging: {
            //pageSize: 11,
            enabled: false
        },
        dataSynced: false,
        dynamicCol: {
            afterAction: true,
            headerContainer: "#col-to-append-after",
        },
        columns: [
            {
                name: "KhmerName",
                title: {
                    enabled: true
                },
                template: "<div title='Double click to adddon' class='csr-pointer'></div>",
                on: {
                    "dblclick": function (e) {
                        //PromoType = 0 //Not promo item
                        if (e.data.PromoType == 0 && e.data.ItemType.toLowerCase() !== "addon") {
                            __orderInfo.OrderDetail = e.data;
                            resetGroupItems(true);
                            if (__orderInfo.Setting.PanelViewMode == 1) {
                                __this.listItemAddons();
                            }
                        } else {
                            __this.dialog("Unavailable Addon", "The item is not available for addon.", "warning");
                        }
                    }
                }
            },
            {
                name: "Qty",
                dataType: "number"
            },
            {
                name: "UnitPrice",
                dataType: "number",
                title: {
                    enabled: true,
                    text: "Double click to copy the item."
                },
                template: "<div class='csr-pointer'></div>",
                on: {
                    "dblclick": function (e) {
                        if (e.data.ItemType.toLowerCase() !== "addon") {
                            copyItem(e.data);
                        }
                    }
                }
            },
            {
                name: "ItemUoMs",
                template: "<select>",
                on: {
                    "change": function (e) {
                        var uomVal = parseInt(this.value);
                        changeItemByUoM(e.key, uomVal, e.data.Qty);
                    }
                }
            },
            {
                name: "TaxGroups",
                template: "<select>",
                on: {
                    "change": function (e) {
                        e.data.TaxGroupID = this.value;
                        changeItemByTaxGroup(e.data);
                    }
                }
            },
            {
                name: "Total",
                dataType: "number",
                //dataFormat: { fixed: baseCurrency?.DecimalPlaces }
            },
            {
                name: "Code",
                title: {
                    enabled: true,
                    text: "Click to comment on the item."
                },
                template: "<span class='csr-pointer'></span>",
                on: {
                    "click": function (e) {
                        __this.commentItem(e.data);
                    }
                }
            },
            {
                name: "AddictionProps",
                valueDynamicProp: "ValueName",
                dynamicCol: true,
            }
        ]
    });

    this.mergeOrderInfo = function (orderInfo = {}) {
        $.extend(__orderInfo, orderInfo);
    }

    this.mergeConfig = function (config = {}) {
        $.extend(__config, config);
    }

    this.switchPanelViewMode = function () {
        switch (__orderInfo.Setting.PanelViewMode) {
            case 1:
                $("#panel-group-items .right-panel").addClass("width-zero");
                $("#left-search-box").show(200).find(".search-box input").focus();
                $("#right-toolbar").hide(200);
                $("#group-buttons").addClass("fixed-width");
                break;
            case 0:
                $("#panel-group-items .right-panel").removeClass("width-zero");
                $("#right-toolbar").show(200);
                $("#left-search-box").hide(200);
                $("#group-buttons").removeClass("fixed-width");
                break;
        }
    }

    this.loadScreen = function (enabled = true) {
        $(__loadscreen).show();
        if (!enabled) {
            $(__loadscreen).hide();
        }
    }

    this.setOrder = function (order = {}, table = {}) {
        if (isValidJSON(table)) {
            __orderInfo.OrderTable = table;
        }

        __this.bindLineItems(order[__config.detail.fieldName], __this.listViewOrder);
        __this.summarizeOrder(order);

        __this.clearItemGrids();
        let saleItems = __orderInfo.SaleItems.filter(s => order[__config.detail.fieldName].some(od => od.LineID == s.ID));
        __this.bindSingleItemGrids(saleItems);

    }

    this.createHubConnection = function (onConnected) {
        var connection = new signalR.HubConnectionBuilder()
            .withUrl("/realbox")
            .configureLogging(signalR.LogLevel.Information)
            .build();

        async function start() {
            await connection.start().then(function () {
                console.log("Second screen connected.");
                if (typeof onConnected === "function") {
                    onConnected.call(connection, __orderInfo);
                }
            }).catch(function (err) {
                console.error('Error connection ->: ' + err.toString());
                setTimeout(start, 3000);
            });
        }

        connection.onclose(async e => {
            console.info('Second screen disconnected ->: ', e);
            await start();
        });

        start();
        return connection;
    }

    //Changed on 16-08-2021
    this.bindLineItems = function (items, table) {
        var orderDetails = [];
        __this.listViewOrder.clearRows();
        if (table !== undefined) {
            table.clearRows();
        }

        for (let i = 0; i < items.length; i++) {
            items[i].IsReadonly = true;
            items[i].ItemUoMs = __this.filterUoMs(items[i].GroupUomID, items[i].UomID, true);
            items[i].TaxGroups = selectTaxGroups(items[i].TaxGroupID, false);
            items[i].RemarkDiscounts = selectRemarkDiscount(items[i].RemarkDiscountID, false);
            items[i].TaxOption = __orderInfo.Setting.TaxOption;
            if (isEmpty(items[i].ParentLineID)) {
                orderDetails.push(items[i]);
            }

            var _addons = $.grep(items, function (item) {
                return item.ParentLineID == items[i].LineID;
            });

            for (let j = 0; j < _addons.length; j++) {
                if (items[i].LineID == _addons[j].ParentLineID) {
                    orderDetails.push(_addons[j]);
                }
            }
            __this.sumLineItem(items[i]);
        }

        orderDetails = $.grep(orderDetails, function (_item) {
            if (_item.ItemType.toLowerCase() === "service") {
                return _item.PrintQty >= 0;
            } else {
                return !(_item.Qty === 0 && _item.PrintQty === 0);
            }
        });
        displayLineItems(orderDetails, table);
        const baseCurrency = __orderInfo.DisplayPayOtherCurrency
            .filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0] ?? { DecimalPlaces: 0 };
        table.updateFieldFormat(["Total", "Qty", "UnitPrice"], "number", { fixed: baseCurrency.DecimalPlaces });
    }

    function displayLineItems(orderDetails, table) {
        if (table !== undefined) {
            for (let i = 0; i < orderDetails.length; i++) {
                table.addRow(orderDetails[i], function (info) {
                    if (orderDetails[i].IsReadonly) {
                        $(info.row).addClass("readonly");
                    }

                    if (!isEmpty(info.data.ParentLineID)) {
                        $(info.row).find("[data-field='Code']")
                            .prepend("<span>" + info.data.Prefix + "</span>&nbsp;")
                            .css("text-align", "left")
                            .css("padding-left", "15px");
                    }
                });

                $(table.getRow(orderDetails[i])).find(".scale-box .scale-down").removeClass("readonly");
            }

            if (isValidArray(orderDetails)) {
                const prop = orderDetails[0]["AddictionProps"];
                table.clearHeaderDynamic(prop);
                table.createHeaderDynamic(prop);
                __this.listViewOrder.clearHeaderDynamic(prop);
                __this.listViewOrder.createHeaderDynamic(prop);
            }

        }
    }

    function findInArray(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }

    this.bindSingleItemGrids = function (items = [], currencies = []) {
        var __wrapGridItem = "#group-item-gridview .wrap-grid";
        if (!isValidArray(currencies)) {
            currencies = __orderInfo.DisplayPayOtherCurrency;
        }

        const baseCurrency = currencies.filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0] ?? { DecimalPlaces: 0 };
        $.each(items, function (i, item) {
            let discType = (item.TypeDis.toLowerCase() === "percent") ? "%" : "",
                imagePath = getImage(item.Image);
            let khName = item.KhmerName + ' (' + item.UoM + ')';
            let $grid = clone("div").addClass("grid").attr("data-id", item.ID),
                $title = clone("div").addClass("grid-caption").text(khName).prop("title", khName),
                $image = clone("div").addClass("grid-image").append("<img src=" + imagePath + ">"),
                $price = clone("div").addClass("price").append(baseCurrency.BaseCurrency + " "),
                $priceSlash = clone("div").addClass("price-slash").text(baseCurrency.BaseCurrency + " "
                    + __this.toCurrency(item.UnitPrice, baseCurrency.DecimalPlaces, currencies)),
                $discount = clone("div").addClass("discount hover-resize"),
                $barcode = clone("div").addClass("grid-subtitle").text(item.Code),
                $qty = $("<div></div>");
            if (__orderInfo.Setting.Portraite) {
                $grid.addClass("portrait");
            }

            $image.append($qty).append($price);
            if (item.DiscountRate > 0) {
                $discount.text(-item.DiscountRate + "%");
                $image.append($discount);
                $price.append(" " + __this.toCurrency(__this.toNumber(item.UnitPrice) * (1 - item.DiscountRate / 100), baseCurrency.DecimalPlaces, currencies));
                $image.append($priceSlash);
            } else {
                $price.append(__this.toCurrency(__this.toNumber(item.UnitPrice) * (1 - item.DiscountRate / 100), baseCurrency.DecimalPlaces, currencies));
            }
            $image.append($price);
            $grid.append($title).append($image).append($barcode);
            $(__wrapGridItem).append($grid);
            $grid[0].scrollIntoView(false);
            $grid[0].scrollIntoView({ behavior: "smooth", block: "end", inline: "nearest" });
        });
    }

    function applyTax(lineItem) {
        if (isValidJSON(lineItem)) {
            let taxRate = 0;
            if (lineItem.TaxGroupID > 0) {
                taxRate = getTaxRate(lineItem.TaxGroupID);
            }
            const disCountTotal = lineItem.TotalNoTax * (1 - (__this.toNumber(__orderInfo.Order.DiscountRate) + __this.toNumber(__orderInfo.Order.PromoCodeDiscRate) + __this.toNumber(__orderInfo.Order.BuyXAmGetXDisRate) + __this.toNumber(__orderInfo.Order.CardMemberDiscountRate)) / 100);
            lineItem.TaxRate = taxRate;
            lineItem.TaxValue = disCountTotal * taxRate / (100 + taxRate);
            __this.listViewOrder.updateColumn(lineItem.LineID, "TaxValue", lineItem.TaxValue);

            //TaxOption.None == 0 || TaxOption.InvoiceVAT == 3
            if (__orderInfo.Setting.TaxOption == 0 || __orderInfo.Setting.TaxOption == 3) {
                lineItem.TaxGroupID = 0;
                lineItem.TaxValue = 0;
            }
            //TaxOption.VAT
            if (__orderInfo.Setting.TaxOption == 1) {
                const taxDisplay = __this.toNumber(lineItem.Total) * taxRate / 100;
                lineItem.TaxValue = disCountTotal * taxRate / 100;
                lineItem.Total = __this.toNumber(lineItem.Total) + taxDisplay;
                lineItem.TotalNet = lineItem.TotalNoTax;
            }
            lineItem.Total = __this.toNumber(lineItem.Total);
            lineItem.Total_Sys = __this.toNumber(lineItem.Total) * parseFloat(__orderInfo.Order.PLRate);
        }
    }

    function getTaxRate(taxGroupId) {
        let taxGroup = findInArray("ID", taxGroupId, __orderInfo.TaxGroups);
        if (isValidJSON(taxGroup)) {
            let taxRate = taxGroup["Rate"];
            return __this.toNumber(taxRate);
        }
    }

    function selectTaxGroups(taxGroupId, disabled = false) {
        var taxes = __orderInfo.TaxGroups;
        var selectList = [];
        if (isValidArray(taxes)) {
            selectList.push({
                Value: 0,
                Text: "---"
            });
            for (let i = 0; i < taxes.length; i++) {
                selectList.push({
                    Value: taxes[i].ID,
                    Text: taxes[i].Name,
                    Selected: taxes[i].ID == taxGroupId,
                    Disabled: disabled
                });
            }
        }
        return selectList;
    }

    function selectRemarkDiscount(remarkDisId, disabled = false) {
        let remarks = __orderInfo.RemarkDiscountItem;
        if (isValidArray(remarks)) {
            let selectList = [];
            for (let i = 0; i < remarks.length; i++) {
                selectList.push({
                    Value: remarks[i].Value,
                    Text: remarks[i].Text,
                    Selected: remarks[i].Value == remarkDisId,
                    Disabled: disabled
                });
            }
            return selectList;
        }
    }

    this.filterUoMs = function (groupUoMId, uomId, disabled = false) {
        var selectList = [];
        if (isValidArray(__orderInfo.ItemUoMs)) {
            $.grep(__orderInfo.ItemUoMs, function (uom, i) {
                if (uom.GroupUomID == groupUoMId) {
                    selectList.push({
                        Value: uom.UomID,
                        Text: uom.Name,
                        Selected: uom.UomID == uomId,
                        Disabled: disabled
                    });
                }
            });
            return selectList;
        }
    }

    this.toNumber = function (value) {
        if (value !== undefined) {
            if (typeof value === "number") {
                value = value.toString();
            }
            return parseFloat(value.split(",").join(""));
        }
    }

    this.toCurrency = function (value, decimalPlaces = -1, currencies = []) {
        if (!isValidArray(currencies)) {
            currencies = __orderInfo.DisplayPayOtherCurrency;
        }
        const baseCurrency = currencies.filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0] ?? { DecimalPlaces: 0 };
        decimalPlaces = decimalPlaces == -1 ? baseCurrency.DecimalPlaces : decimalPlaces;
        let data = formatNumber(value, decimalPlaces);
        return data;
    }

    function createZeroes(length) {
        let result = "";
        for (let i = 0; i < length; i++, result += "0");
        return result;
    }

    function formatNumber(value, decimalPlaces = 3) {
        if (!isNaN(value)) {
            let format = { decimal: ".", separator: ",", fixed: decimalPlaces };
            //format = $.extend(format, dataFormat);
            let _value = value.toString();
            if (_value.indexOf(format.decimal) > 0) {
                let values = value.toString().split(format.decimal);
                values[values.length - 1] += createZeroes(format.fixed - values[values.length - 1].length);
                values[values.length - 1] = values[values.length - 1].substring(0, format.fixed);
                _value = values.join(format.decimal);
            } else {
                _value = _value + format.decimal + createZeroes(format.fixed);
            }

            let pattern = new RegExp("\\d(?=(\\d{3})+\\" + format.decimal + ")", "g");
            let data = _value.replace(pattern, "$&" + format.separator);
            if (format.fixed == 0) {
                data = data.split(".")[0];
            }
            return data;
        }
    }

    this.clearItemGrids = function () {
        $(__wrapGridItem).children().remove();
    }

    this.sumLineItem = function (lineItem) {
        if (isValidJSON(lineItem) && !isEmpty(lineItem.LineID)) {
            lineItem.DiscountRate = __this.toNumber(lineItem.DiscountRate);
            lineItem.DiscountValue = __this.toNumber(lineItem.DiscountValue);
            switch (lineItem.TypeDis.toLowerCase()) {
                case "percent":
                    lineItem.TotalNoTax = lineItem.Qty * __this.toNumber(lineItem.UnitPrice) * (1 - (lineItem.DiscountRate / 100));
                    lineItem.Total = lineItem.TotalNoTax;
                    lineItem.DiscountValue = (lineItem.Qty * __this.toNumber(lineItem.UnitPrice)) * lineItem.DiscountRate / 100;
                    break;
                default:
                    lineItem.DiscountRate = (lineItem.Qty * __this.toNumber(lineItem.UnitPrice)) == 0 ? 0 : 100 * lineItem.DiscountValue / lineItem.Total;
                    lineItem.TotalNoTax = lineItem.Qty * __this.toNumber(lineItem.UnitPrice) - lineItem.DiscountValue;
                    lineItem.Total = lineItem.TotalNoTax;
                    break;
            }

            applyTax(lineItem);
            return __this.toNumber(lineItem.Total);
        }
    }

    this.sum = function (items, fieldName, callback) {
        let total = 0;
        for (let i = 0; i < items.length; i++) {
            total += __this.toNumber(items[i][fieldName]);
            if (typeof callback === "function") {
                callback.call(__this, i, items[i], total);
            }
        }
        return total;
    }

    this.calculateInvoiceVAT = function (order) {
        if (__orderInfo.Setting.TaxOption == 3) {
            const altActiveCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.IsActive)[0] ?? { AltCurrency: "", Rate: 0 };
            const taxRate = getTaxRate(__orderInfo.Setting.Tax);
            const disvalueTotal = order.Sub_Total * (1 - (__this.toNumber(order.DiscountRate) + __this.toNumber(order.PromoCodeDiscRate) + __this.toNumber(order.BuyXAmGetXDisRate) + __this.toNumber(order.CardMemberDiscountRate)) / 100);
            order.TaxRate = taxRate;
            order.TaxGroupID = __orderInfo.Setting.Tax;
            order.TaxValue = taxRate * disvalueTotal / 100;
            order.GrandTotal = disvalueTotal + order.TaxValue;

            order.Change = __this.toNumber(order.Received) - __this.toNumber(order.GrandTotal);
            order.Change_Display = __this.toNumber(order.Change) * __this.toNumber(altActiveCurrency?.Rate);
            order.GrandTotal_Display = __this.toNumber(order.GrandTotal) * __this.toNumber(altActiveCurrency?.Rate);
        }
    }

    this.calculateBuyXAmGetXDis = function () {
        if (isValidArray(__orderInfo.LoyaltyProgram.BuyXAmGetXDis)) {
            const buyx = __orderInfo.LoyaltyProgram.BuyXAmGetXDis.filter(i => i.Amount <= __orderInfo.Order.GrandTotal)
            var buyxMax = 0;
            var _buyx = {};
            buyx.forEach(i => {
                if (i.Amount > buyxMax) {
                    buyxMax = i.Amount
                    _buyx = i
                }
            });
            if (isValidJSON(_buyx)) {
                /**
                 * _buyx.DisType : Rate = 1, Value = 2
                 * */
                __orderInfo.Order.BuyXAmountGetXDisID = _buyx.ID;
                if (_buyx.DisType == 1) {
                    const grandxdisRateValue = _buyx.DisRateValue * __orderInfo.Order.GrandTotal;
                    const value = grandxdisRateValue == 0 ? 0 : grandxdisRateValue / 100;
                    __orderInfo.Order.GrandTotal -= value;
                    __orderInfo.Order.BuyXAmGetXDisRate = _buyx.DisRateValue;
                    __orderInfo.Order.BuyXAmGetXDisValue = value;
                    __orderInfo.Order.BuyXAmGetXDisType = _buyx.DisType;
                }
                if (_buyx.DisType == 2) {
                    const rate = _buyx.DisRateValue * 100 == 0 || __orderInfo.Order.GrandTotal == 0 ? 0 : _buyx.DisRateValue * 100 / __orderInfo.Order.GrandTotal
                    __orderInfo.Order.GrandTotal -= _buyx.DisRateValue;
                    __orderInfo.Order.BuyXAmGetXDisRate = rate;
                    __orderInfo.Order.BuyXAmGetXDisValue = _buyx.DisRateValue;
                    __orderInfo.Order.BuyXAmGetXDisType = _buyx.DisType;
                }
            } else {
                __orderInfo.Order.BuyXAmGetXDisRate = 0;
                __orderInfo.Order.BuyXAmGetXDisValue = 0;
            }
        }
        return __orderInfo.Order;
    }

    this.calculateOrder = function (order) {
        if (isValidJSON(order)) {
            __orderInfo.Order = $.extend(__orderInfo.Order, order);
            if (__orderInfo.Setting.TaxOption == 3) {
                if (__orderInfo.Order.Sub_Total <= 0) {
                    __orderInfo.Order.TaxRate = 0;
                }
            }

            let subtotal = 0, subNet = 0, subTotalNoTax = 0;
            if (isValidArray(order[__config.detail.fieldName])) {
                subNet = __this.sum(order[__config.detail.fieldName], "TotalNet");
                subtotal = __this.sum(order[__config.detail.fieldName], "Total");
                subTotalNoTax = __this.sum(order[__config.detail.fieldName], "TotalNoTax");
            }

            order.Sub_Total = subtotal;
            //__orderInfo.Setting.TaxOption == 1 => Exclude
            const _subtotal = __orderInfo.Setting.TaxOption == 1 ? subNet : subtotal;
            order.CardMemberDiscountValue = order.CardMemberDiscountRate * subtotal / 100;
            const grandTotalAfterDiscount = _subtotal * (1 - (__this.toNumber(__orderInfo.Order.DiscountRate) + __this.toNumber(__orderInfo.Order.PromoCodeDiscRate) + __this.toNumber(__orderInfo.Order.BuyXAmGetXDisRate) + __this.toNumber(__orderInfo.Order.CardMemberDiscountRate)) / 100);
            order.DiscountValue = __this.toNumber(order.Sub_Total) * __this.toNumber(order.DiscountRate) / 100;
            order.PromoCodeDiscValue = __this.toNumber(order.Sub_Total) * __this.toNumber(order.PromoCodeDiscRate) / 100;
            order.GrandTotal = grandTotalAfterDiscount;
            //TaxOption.VAT
            if (__orderInfo.Setting.TaxOption == 1) {
                order.GrandTotal = grandTotalAfterDiscount + __this.sum(order[__config.detail.fieldName], "TaxValue");
            }
            order.SubTotalNoTax = subTotalNoTax;
            order.GrandTotal += order.FreightAmount;
            order.GrandTotal_Sys = order.GrandTotal * order.PLRate;

            __this.calculateBuyXAmGetXDis();
            const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0] ?? { DecimalPlaces: 0 };
            const altActiveCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.IsActive)[0] ?? { AltCurrency: "", Rate: 0 };
            order.CurrencyDisplay = altActiveCurrency.AltCurrency;
            order.TaxOption = __orderInfo.Setting.TaxOption;
            order.TaxValue = __this.sum(order[__config.detail.fieldName], "TaxValue");
            __this.calculateInvoiceVAT(order);
            const gtotalStr = __this.toCurrency(order.GrandTotal);
            order.GrandTotal = __this.toNumber(gtotalStr);
            order.Change = __this.toNumber(order.Received) - __this.toNumber(order.GrandTotal);
            order.Change_Display = __this.toNumber(order.Change) * __this.toNumber(altActiveCurrency.Rate);
            order.GrandTotal_Display = __this.toNumber(order.GrandTotal) * __this.toNumber(altActiveCurrency.Rate);
            return __orderInfo.Order;
        }
    }

    this.clearCurrentOrder = function () {
        __this.listViewOrder.clearRows();
        __this.clearItemGrids();
        const currencyBase = __orderInfo.DisplayPayOtherCurrency.filter(i => i.BaseCurrencyID === i.AltCurrencyID)[0] ?? { BaseCurrency: 0 };
        $("#summary-discount-rate").text("(" + __this.toCurrency(0, currencyBase?.DecimalPlaces) + "%)");
        $("#summary-discount").text(currencyBase.BaseCurrency + " " + __this.toCurrency(0, currencyBase?.DecimalPlaces));
        $("#summary-vat").text(currencyBase.BaseCurrency + " " + __this.toCurrency(0, currencyBase?.DecimalPlaces));
        $("#summary-vat-rate").text(__this.toCurrency(0) + " %");
        $("#summary-sub-total").text(currencyBase.BaseCurrency + " " + __this.toCurrency(0, currencyBase?.DecimalPlaces));
        $("#total-qty").text(currencyBase.BaseCurrency + " " + __this.toCurrency(0, currencyBase?.DecimalPlaces));
        $("#summary-total").text(currencyBase.BaseCurrency + " " + __this.toCurrency(0, currencyBase?.DecimalPlaces));
        $("#count-items").text(0);
        $("#order-number").text("");
        $("#count-row-number-text").text(0);
        __this.displayTax();
    }

    this.summarizeOrder = function (order = {}) {
        if (isValidJSON(order)) {
            __this.calculateOrder(order);
            const currencyBase = __orderInfo.DisplayPayOtherCurrency.filter(i => i.BaseCurrencyID === i.AltCurrencyID)[0] ?? { BaseCurrency: 0 };
            $("#summary-discount-rate").text("(" + __this.toCurrency(__orderInfo.Order.DiscountRate + __orderInfo.Order.BuyXAmGetXDisRate + __orderInfo.Order.CardMemberDiscountRate) + "%)");
            $("#summary-discount").text(currencyBase.BaseCurrency + " " + __this.toCurrency(__orderInfo.Order.DiscountValue + __orderInfo.Order.BuyXAmGetXDisValue + __orderInfo.Order.CardMemberDiscountValue, currencyBase?.DecimalPlaces));
            $("#summary-vat").text(currencyBase.BaseCurrency + " " + __this.toCurrency(__orderInfo.Order.TaxValue, currencyBase?.DecimalPlaces));
            $("#summary-vat-rate").text(__this.toCurrency(__orderInfo.Order.TaxRate) + " %");
            $("#summary-sub-total").text(currencyBase.BaseCurrency + " " + __this.toCurrency(__orderInfo.Order.Sub_Total, currencyBase?.DecimalPlaces));
            $("#total-qty").text(currencyBase.BaseCurrency + " " + __this.toCurrency(__orderInfo.Order.Sub_Total, currencyBase?.DecimalPlaces));
            $("#summary-total").text(currencyBase.BaseCurrency + " " + __this.toCurrency(__orderInfo.Order.GrandTotal, currencyBase?.DecimalPlaces));
            $("#count-items").text(__orderInfo.Order[__config.detail.fieldName].length);
            __this.displayOrderTitle("#order-number", __config.orderOnly);
            __this.displayTax();
        }
    }

    this.displayTax = function () {
        switch (__orderInfo.Setting.TaxOption) {
            case 0:
                $(".tax-display").hide();
                $("[data-field='TaxGroups']").hide();
                $(".tax-rate-display-invoice").hide();
                break;
            case 3:
                $(".tax-display").hide();
                $(".tax-display-invoice").show();
                $("[data-field='TaxGroups']").hide();
                $(".tax-rate-display-invoice").show();
                break;
            default:
                $(".tax-display").show();
                $("[data-field='TaxGroups']").show();
                $(".tax-rate-display-invoice").hide();
                break;
        }
    }

    this.displayOrderTitle = function (selector, orderOnly = false, itemCount = 0) {
        let count = __orderInfo.Order[__config.detail.fieldName].length;
        if (itemCount > 0) {
            count = itemCount;
        }
        let title = isValidJSON(__orderInfo.OrderTable) ?
            __orderInfo.OrderTable.Name + " > #" + __orderInfo.Order.OrderNo
            : "#" + __orderInfo.Order.OrderNo;
        if (orderOnly) { title = __orderInfo.Order.OrderNo; }
        $("#count-row-number-text").text(count);
        $(selector).text(title);
    }

    function isEmpty(value) {
        return value == undefined || value == null || value == "";
    }

    function isValidJSON(value) {
        return !isEmpty(value) && value.constructor === Object && Object.keys(value).length > 0;
    }

    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }

    function getImage(imagePath) {
        return imagePath ? "/Images/items/" + imagePath : "/Images/default/no-image.jpg";
    }

    function clone(prop, deep) {
        if (deep) {
            return $(__template[prop]).clone(deep);
        }
        return $(__template[prop]).clone();
    }
}

