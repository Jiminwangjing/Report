

"use strict";
function PosPayment(PosCore) {
    let cusId = 0;
    if (!(this instanceof PosPayment)) {
        return new PosPayment(PosCore);
    }
    const _printUrlPOS = JSON.parse($("#print-template-url").text()).Templateurl;

    const __this = this, __posCore = PosCore, __config = PosCore.config();
    var __orderInfo = {},
        __paymentType = {
            Normal: 0,
            CardMember: 1
        },
        __urls = {
            postPromoCode: "/POS/PostPromoCode",
            getCardMemberDetial: "/POS/GetCardMemberDetial",
            updateFrieght: "/POS/UpdateFreight"
        };
    const __listPaymentMean = ViewTable({
        selector: "#list-payment-means",
        keyField: "ID",
        visibleFields: ["Type", "Currency", "IsChecked"],
        paging: {
            enabled: false
        },
        columns: [
            {
                name: "Currency",
                template: "<select class='currid'></select",
                on: {
                    "change": function (e) {
                        updateMultiPaymentMean(e.data.ID, "AltCurrencyID", parseInt(this.value));
                        __listPaymentMean.updateColumn(e.key, "AltCurrencyID", parseInt(this.value));
                        __orderInfo.PaymentMeans = __listPaymentMean.yield();
                        
                        onPaymentMeanChanged(e.data); 
                    }
                }
            },
            {
                name: "IsChecked",
                template: "<input type='checkbox' class='checked'>",
                on: {
                    "change": function (e) {
                        __listPaymentMean.updateColumn(e.key, "IsChecked", this.checked);
                        __orderInfo.PaymentMeans = __listPaymentMean.yield();
                        updateMultiPaymentMean(e.data.ID, "AltCurrencyID", e.data.AltCurrencyID);
                        updateMultiPaymentMean(e.data.ID, "Amount", 0)
                        updateMultiPaymentMean(e.data.ID, "Total", 0)
                        onPaymentMeanChanged(e.data, e.key);
                        if (this.checked == false) {
                            removeMultiPaymentMean(e.data.ID)
                        } else {
                            let colCurr = __listPaymentMean.getColumn(e.key, "Currency");
                            let select = $(colCurr).children("select");
                            let currencyId = $(select).find("option:selected").val();
                        }
                        displayPayment(__orderInfo.Order);
                    }
                }
            }
            // {
            //     name: "IsReceivedChange",
            //     template: "<input name='received-change' type='radio'>",
            //     on: {
            //         "change": function (e) {
            //             __listPaymentMean.updateColumn(e.key, "IsReceivedChange", this.checked);
            //             __orderInfo.PaymentMeans = __listPaymentMean.yield();
            //             onPaymentMeanChanged(e.data);
            //         }
            //     }
            // }
        ]
    });

    __posCore.onPanelPayment(function (info) {

        const _customertip = {
            ReceiptID: 0,
            Amount: 0,
            AltCurrencyID: 0,
            AltCurrency: "",
            AltRate: 0,
            SCRate: 0,
            LCRate: 0,
            BaseCurrency: "",
            BaseCurrencyID: 0,

        };
        __posCore.readyPayment = true;
        __orderInfo = info;
        cusId = __orderInfo.Order.CustomerID;
        __orderInfo.Order.MultiPaymentMeans = [];
        __orderInfo.Order.CustomerTips = {};
        defineOtherCurrencies(__orderInfo.Order, info.DisplayPayOtherCurrency);
        __orderInfo.Order.CustomerTips = _customertip;
        setPaymentMeans(function (paymentMeans) {
            renderInputsPaymentMeans(".container_payment_means", paymentMeans, onInput, onSelect);
        });
        $.ajax({
            url: "/POS/GetOrderInfoCurr",
            type: "POST",
            success: function (res) {
                __listPaymentMean.bindRows(res.PaymentMeans)
            }
        });
        setTimeout(() => {
            let _lineInput = $(".container_payment_means").children()[0];
            activeInput = $(_lineInput).find("input")[0];
            $(activeInput).focus();
        }, 200);
    });

    this.setupFreights = function () {
        let amount = 0;
        let dialog = new DialogBox({
            content: {
                selector: "#freight-content"
            },
            type: "yes/no/cancel",
            button: {
                no: {
                    text: "Confirm",
                    callback: confirmFreight
                },
                yes: { text: "Update", callback: updateFrieght },
                cancel: {
                    callback: function () { dialog.shutdown() }
                },
            }
        });

        dialog.invoke(function () {
            const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0] ?? { DecimalPlaces: 0 };

            let freights = __orderInfo.Freights;
            amount = __orderInfo.Order.FreightAmount = __posCore.sumFreights();
            dialog.content.find("#freight-currency").text(baseCurrency.BaseCurrency);
            dialog.content.find("#total-freights").text(__posCore.toCurrency(amount));
            var $listFreights = ViewTable({
                keyField: "FreightID",
                selector: "#list-freights",
                visibleFields: ["Name", "AmountReven", "FreightReceiptTypes", "IsActive"],
                columns: [
                    {
                        template: "<input type='number'/>",
                        name: "AmountReven",
                        on: {
                            "keyup focus": function (e) {
                                let _value = this.value;
                                if (_value == "") { _value = "0"; }
                                e.data.AmountReven = _value;
                                amount = __posCore.sumFreights();
                                dialog.content.find("#freight-currency").text(baseCurrency.BaseCurrency);
                                dialog.content.find("#total-freights").text(__posCore.toCurrency(amount, baseCurrency.DecimalPlaces));
                                __orderInfo.Order.FreightAmount = amount;
                                displayFreightTotal();
                            }
                        }
                    },
                    {
                        template: "<select></select>",
                        name: "FreightReceiptTypes",
                        on: {
                            "change": function (e) {
                                e.data.FreightReceiptType = this.value;
                                amount = __posCore.sumFreights();
                                dialog.content.find("#freight-currency").text(baseCurrency.BaseCurrency);
                                dialog.content.find("#total-freights").text(__posCore.toCurrency(amount, baseCurrency.DecimalPlaces));
                                __orderInfo.Order.FreightAmount = amount;
                                displayFreightTotal();

                                updateSelectTypeFrieght(e.data.FreightReceiptTypes, this.value);
                            }
                        }
                    },
                    {
                        template: "<input type='checkbox'/>",
                        name: "IsActive",
                        on: {
                            "click": function (e) {
                                const isChecked = $(this).prop("checked");
                                e.data.IsActive = isChecked;
                            }
                        }
                    }
                ],
                paging: {
                    enabled: false
                }
            }).bindRows(freights);
        });

        function confirmFreight() {
            displayFreightTotal();
            dialog.shutdown();
        }

        function updateFrieght() {
            let data = __orderInfo.Order.Freights ?? "";
            data = JSON.stringify(data)
            $.post(__urls.updateFrieght, { data }, function (res) {
                new ViewMessage({
                    summary: {
                        selector: "#frieght-error-summary"
                    }
                }, res);
            })
        }
        function updateSelectTypeFrieght(data, key) {
            if (isValidArray(data)) {
                data.forEach(i => {
                    if (i.Key == key) i.Selected = true
                    else i.Selected = false
                })
            }
        }

        function displayFreightTotal() {
            __posCore.calculateOrder(__orderInfo.Order);
            $("#amount-freight").val(__posCore.toCurrency(__orderInfo.Order.FreightAmount));
            displayPayment(__orderInfo.Order);
        }
    }

    this.chooseOtherCurrencies = function () {
        __posCore.checkPrivilege("P023", function (info) {
            let dialog = new DialogBox({
                caption: "Other Currency",
                icon: "fas fa-exchange-alt",
                content: {
                    selector: "#pay-other-currencies-content"
                },
                type: "ok-cancel",
                button: {
                    ok: {
                        text: "Save"
                    }
                }
            });
            dialog.invoke(function () {
                var viewTableOtherCurrencies = ViewTable({
                    keyField: "LineID",
                    visibleFields: ["BaseCurrency", "AltCurrency", "Amount", "AltAmount"],
                    selector: "#list-pay-other-currencies",
                    paging: {
                        enabled: false,
                    },
                    columns: [
                        {
                            name: "Amount",
                            dataType: "number"
                        },
                        {
                            name: "AltAmount",
                            template: "<input class='number'>",
                            on: {
                                keyup: function (e) {
                                    e.data.AltAmount = !isNaN(__posCore.toNumber(this.value)) ? __posCore.toNumber(this.value) : 0.000;
                                    calculateOtherCurrencyToPlCurrency(viewTableOtherCurrencies, e.data);
                                }
                            }
                        }
                    ],
                });
                viewTableOtherCurrencies.bindRows(info.DisplayGrandTotalOtherCurrency);
                dialog.confirm(function () {
                    calculateOtherCurrenciesToPlCurrency(viewTableOtherCurrencies.yield());
                    dialog.shutdown();
                });

                dialog.reject(function () {
                    dialog.shutdown();
                });

                dialog.content.find("table input.number").asPositiveNumber();
            });

        });
    }

    function calculateOtherCurrencyToPlCurrency(table, data) {
        const amount = data.AltAmount / data.AltRate;
        table.updateColumn(data.LineID, "Amount", amount);

    }
    function calculateOtherCurrenciesToPlCurrency(data) {
        if (isValidArray(data)) {
            let plAmount = 0;
            data.forEach(i => {
                plAmount += i.Amount;
            });
            if (plAmount.toString().includes(".")) {
                plAmount = plAmount.toString().split(".")[0] + "." + plAmount.toString().split(".")[1].substring(0, 3);
            }

            __orderInfo.Order.GrandTotalOtherCurrencies = data.filter(i => i.AltAmount)
            displayPayment(__orderInfo.Order);
        }
    }

    function clearCalculateOtherCurrenciesToPlCurrency(data) {
        if (isValidArray(data)) {
            data.forEach(i => {
                i.Amount = 0;
                i.AltAmount = 0;
            });
        }
    }

    $("#promo-code").on("keypress", function () {
        __posCore.readyPayment = false;
        var promo = $.trim($("#promo-code").val());
        __posCore.loadScreen();
        __posCore.checkPrivilege("P015", function () {
            $.post(__urls.postPromoCode, { priceListID: __orderInfo.Order.PriceListID, code: promo },
                function (message) {
                    __posCore.loadScreen(false);
                    var pcds = message.Items["PromoCodeDiscount"];
                    var pcd = message.Items["PromoCodeDetail"];
                    ViewMessage({}, message);
                    if (isEmpty(pcds)) {
                        __orderInfo.Order.PromoCodeID = 0;
                    }
                    if (isEmpty(pcd)) {
                        __orderInfo.Order.PromoCodeID = 0;
                    }
                    if (!isEmpty(pcd)) {
                        if (pcd.PromoCode != promo) {
                            __orderInfo.Order.PromoCodeID = 0;
                        } else {
                            __orderInfo.Order.PromoCodeID = pcd.PromoCodeID;
                        }
                    }
                    if (!isEmpty(pcds)) {
                        if (pcds.PromoType == "1") {
                            __orderInfo.Order.PromoCodeDiscRate = pcds.PromoValue;
                        }
                        else {
                            __orderInfo.Order.PromoCodeDiscRate = pcds.PromoValue / __orderInfo.Order.Sub_Total * 100;
                        }
                    }
                    if (message.IsRejected) {
                        new DialogBox({
                            content: message.Data["PromoCode"],
                            icon: "warning",
                            caption: "Invalid Code"
                        });
                        __orderInfo.Order.PromoCodeDiscRate = 0;
                    }
                    __posCore.readyPayment = true;
                    if (isValidArray(__orderInfo.Order.OrderDetail)) {
                        __orderInfo.Order.OrderDetail.forEach(i => {
                            __posCore.sumLineItem(i);
                        })
                    }
                    __posCore.summarizeOrder(__orderInfo.Order);
                    displayPayment(__orderInfo.Order);
                });
        });
    });

    $("#pay-other-currency").on("keyup focus", function () {
        displayPayment(__orderInfo.Order);
    });


    function clearCardMemberPayment(cardElement) {
        $(".other-pay-way").css("display", "none");
        $("#pay-other").val(0);
        if (cardElement) cardElement.val("");
        $(".manual-card-message").text("");
        __orderInfo.CardMemberNumber = undefined;
        __orderInfo.Order.CustomerID = cusId;
        __orderInfo.Order.PaymentType = 0;
        __orderInfo.Order.Received -= __orderInfo.Order.OtherPaymentGrandTotal;
        __orderInfo.Order.OtherPaymentGrandTotal = 0;

        displayPayment(__orderInfo.Order);
    }

    function initScanCardMember(orderInfo, otherPaymentDialog) {
        $("#form-scan-card").prop("hidden", false);
        $("#form-card-manual").prop("hidden", true);
        var $accessToken = otherPaymentDialog.content.find("#scan-card-number");
        $(otherPaymentDialog.content.parent()).on("click", function () {
            $accessToken.focus();
            otherPaymentDialog.content.find("#form-scan-card").addClass("frame-active");
        });

        setTimeout(function () {
            $accessToken.focus();
            otherPaymentDialog.content.find("#form-scan-card").addClass("frame-active");
        }, 250);
        $accessToken.on("focusout", function () {
            otherPaymentDialog.content.find("#form-scan-card").removeClass("frame-active");
        });

        $accessToken.on("keyup", function (e) {
            __posCore.readyPayment = false;
            if (e.which === 13) {
                $.post(
                    __urls.getCardMemberDetial,
                    {
                        cardNumber: this.value.trim(),
                        grandTotal: orderInfo.Order.GrandTotal,
                        pricelistId: orderInfo.Setting.PriceListID
                    },
                    function (res) {
                        $accessToken.focus().val("");
                        responseCardMemberPay(res, orderInfo, otherPaymentDialog);
                    })
            }
        });
    }

    function initManualCardMember(cardElement, orderInfo, otherPaymentDialog) {
        $("#form-scan-card").prop("hidden", true);
        $("#form-card-manual").prop("hidden", false);
        cardElement.on("keyup", function (e) {
            if (e.which === 13) {
                $.post(
                    __urls.getCardMemberDetial,
                    {
                        cardNumber: this.value.trim(),
                        grandTotal: orderInfo.Order.GrandTotal,
                        pricelistId: orderInfo.Setting.PriceListID
                    },
                    function (res) {
                        responseCardMemberPay(res, orderInfo, otherPaymentDialog);
                    })
            }
        })
    }

    function responseCardMemberPay(res, orderInfo, otherPaymentDialog) {
        ViewMessage({
            summary: {
                selector: ".manual-card-message",
                bordered: false
            }
        }, res);
        if (!res.IsAlerted) {
            setTimeout(function () {
                $(".manual-card-message").text("")
            }, 5000);
        }
        const cardData = res.Items["data"];
        //const altcurrencyId = $("#other-currency-list").val();
        const altcurrencyId = __orderInfo.Order.Currency.ID;
        const currency = findInArray("AltCurrencyID", altcurrencyId, __orderInfo.DisplayPayOtherCurrency);
        if (res.IsAlerted) {
            otherPaymentDialog.confirm(function () {
                cusId = orderInfo.Order.CustomerID;
                $(".other-pay-way").css("display", "flex");
                $("#pay-other").val(cardData.Customer.Balance);
                displayPayment(orderInfo.Order, currency);
                orderInfo.Order.CustomerID = cardData.Customer.ID;
                $(".manual-card-message").text("")
                orderInfo.Order.PaymentType = 1;
                otherPaymentDialog.shutdown();
            })
            otherPaymentDialog.reject(function () {
                $(".other-pay-way").css("display", "none");
                $("#pay-other").val(0);
                orderInfo.Order.PaymentType = 0;
                $(".manual-card-message").text("")
                otherPaymentDialog.shutdown();
            })
            __posCore.readyPayment = true;
        }
        if (cardData) {
            if (cardData.Customer.Balance >= __posCore.toNumber(__orderInfo.Order.GrandTotal)) {
                cusId = orderInfo.Order.CustomerID;
                $("#pay-other").val(__orderInfo.Order.GrandTotal);

                orderInfo.Order.CustomerID = cardData.Customer.ID;
                orderInfo.Order.PaymentType = 1;
                otherPaymentDialog.shutdown();
                __posCore.readyPayment = true;
            }

            let _muliPaymentMean = createMultiPaymentMean(cardData.ID, currency, __orderInfo.Order.GrandTotal, __paymentType.CardMember);
            if (isValidJSON(_muliPaymentMean)) {
                mergeMultiPaymentMean(_muliPaymentMean, __paymentType.CardMember);
            }
        }
        displayPayment(__orderInfo.Order);
    }

    // pay with other currencies
    $("#other-currencies").click(function () {
        __this.chooseOtherCurrencies();
    });

    //Process pay order
    $("#form-payment").on("submit", function (e) {
        e.preventDefault();
        if (__orderInfo.Order.MultiPaymentMeans.length == 0) {
            var $dialog = __posCore.dialog("Invalid PaymentMeans", "Please Check At Least One PaymentMeans", "warning", "yes-no")
                .confirm(function () {
                    $dialog.shutdown();

                }).reject(function () {
                    $dialog.shutdown();
                    if (typeof reject === "function") {
                        reject.call(__this, __this.fallbackInfo());
                    }
                });
        }
        else {
            if (__posCore.readyPayment) {
                __posCore.readyPayment = false;
                __orderInfo.Order.PaymentMeansID = $("#payment-mean-id").val();
                __posCore.payOrder(onPaymentSuccess, function () {
                    __posCore.readyPayment = true;
                }, function () {
                    __posCore.readyPayment = false;
                });

                if (__orderInfo.Setting.PreviewReceipt == true) {
                    let order = {
                        userid: __orderInfo.Order.UserOrderID,
                        tableid: __orderInfo.Order.TableID,
                        cusid: __orderInfo.Order.CustomerID,
                        syscurrid: __orderInfo.Order.SysCurrencyID,
                        branchid: __orderInfo.Order.BranchID,
                        paymentid: __orderInfo.Order.PaymentMeansID,
                        cusid: __orderInfo.Order.PaymentMeansID,
                        OrderDetail: __orderInfo.Order.OrderDetail
                    }

                    let method = "PreviewReceipt";
                    $.post(`/${_printUrlPOS}Home/${method}`, { order: order }, function (__printUrl) {
                        window.open(__printUrl, "_blank");
                    });
                }
            }
        }

    });

    function onPaymentSuccess() {
        $("#customer-name-ks").text(__orderInfo.Order.Customer.Name);
        clearCardMemberPayment();
        clearPayment();
        if (__config.orderOnly) {
            __posCore.loadCurrentOrderInfo(__orderInfo.OrderTable.ID, 0, 0, true);
            __posCore.gotoPanelItemOrder();
        } else {
            __posCore.loadCurrentOrderInfo(__orderInfo.OrderTable.ID, 0);
            __posCore.gotoPanelGroupTable();
        }

        if (__orderInfo.Setting.DaulScreen) {
            $.post("/api/pos/OnPaymentSuccess", { order: JSON.stringify(__orderInfo.Order) });
        }
    }

    $("#amount-freight").on("click", function () {
        __this.setupFreights();
    });
    //=================add customer tips================================================
    $("#cusdeposit-currency").on("keyup focus", function () {
        $(this).asNumber();
        UpdateCustomerTip("Amount", __posCore.toNumber(this.value));
        displayPayment(__orderInfo.Order);
    })
    function UpdateCustomerTip(key, value) {
        // __orderInfo.Order.CustomerTips[key] = __posCore.toNumber(value);
        __orderInfo.Order.CustomerTips[key] = value;

    }
    function defineOtherCurrencies(order, currency) {
        $("#cusdeposit-currency-list").children().remove();
        if (isValidArray(currency)) {
            currency.forEach(i => {
                let option = $("<option value='" + i.AltCurrencyID + "'>" + i.AltCurrency + "</option>");
                if (i.IsLocalCurrency) {
                    option = $("<option selected value='" + i.AltCurrencyID + "'>" + i.AltCurrency + "</option>");
                }
                $("#cusdeposit-currency-list").append(option);
            })
        }

        $("#other-currency-list").on("change", function () {
            $(".other-curr-symbol").text($(this).find("option:selected").text());
            displayPayment(order);
        });
        $(".other-curr-symbol").text($("#other-currency-list").find("option:selected").text());
        //$("#pay-cash").val(order.GrandTotal);
        $("#cusdeposit-currency-list").on("change", function () {
            $(".cusdeposit-curr-symbol").text($(this).find("option:selected").text());
            const _currency = findCurrencyById(this.value);
            UpdateCustomerTip("AltCurrency", _currency.AltCurrency);
            UpdateCustomerTip("AltCurrencyID", _currency.AltCurrencyID);
            UpdateCustomerTip("AltRate", _currency.AltRate);
            UpdateCustomerTip("SCRate", _currency.Rate);
            UpdateCustomerTip("LCRate", _currency.LCRate);
            UpdateCustomerTip("BaseCurrencyID", _currency.BaseCurrencyID);
            UpdateCustomerTip("BaseCurrency", _currency.BaseCurrency);
            displayPayment(order);
        });

        displayPayment(order);
    }


    function clearPayment() {
        $("#amount-freight").val(0.000);
        //$('#pay-cash').val(0.000);
        $('#other-pay').val(0.000);
        $("#pay-total").val(0.000);
        $("#promo-code").val("");
        $("#apply-promo").text("");
        $("#pay-change").val(0.000);
        $('#pay-change-sys').val(0.000);
        $("#pay-total-alt").val(0.000);
        $("#pay-change-alt").val(0.000);
        $("#pay-other-currency").val(0.000);
        $("#pay-received").val(0.000);
        $("#payment-mean-id").val($("#payment-mean-id > option:first-child").val());
        $("#error-message").text("");
        $(".other-pay-way").css("display", "none");
        __orderInfo.CardMemberNumber = undefined;
    }
    this.displayPayment = displayPayment;
    function displayPayment(order) {
        if (__orderInfo.Setting.CustomerTips) {
            $("#wrapper-customer-tips").show();
        }
        else {
            $("#wrapper-customer-tips").hide();

        }
        $("#cusdeposit-currency-list").find("option:selected").text();
        var curr = __posCore.toNumber($("#cusdeposit-currency-list").val());
        const _currency = findCurrencyById(curr);
        UpdateCustomerTip("AltCurrency", _currency.AltCurrency);
        UpdateCustomerTip("AltCurrencyID", _currency.AltCurrencyID);
        UpdateCustomerTip("AltRate", _currency.AltRate);
        UpdateCustomerTip("SCRate", _currency.Rate);
        UpdateCustomerTip("LCRate", _currency.LCRate);
        UpdateCustomerTip("BaseCurrencyID", _currency.BaseCurrencyID);
        UpdateCustomerTip("BaseCurrency", _currency.BaseCurrency);

        const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.IsLocalCurrency)[0];
        if (isNaN(order.CustomerTips.Amount)) {
            order.CustomerTips.Amount = 0;
        }
        let cusTips = $("#cusdeposit-currency");
        let currTips = cusTips.siblings("#cusdeposit-currency-list");
        let currencyTips = findCurrencyById(currTips.val());

        var totalcus = __posCore.roundNumber(__posCore.toNumber(__orderInfo.Order.CustomerTips.Amount) * currencyTips.Rate);
        if (__posCore.toNumber($("#pay-other").val()) == 0) {
            order.Received = totalMultiPaymentMeans() == 0 ? 0 : totalMultiPaymentMeans();
            order.Received = totalMultiPaymentMeans();
        }
        else {

            order.Received = totalMultiPaymentMeans() == 0 ? 0 : totalMultiPaymentMeans();
            order.Received = totalMultiPaymentMeans();

        }
        order.PaymentMeansID = $("#payment-mean-id").val();
        order.PostingDate = $("#post-date").val();
        order.DelayHours = $("#delayhours").val();
        __posCore.calculateOrder(order);
        __posCore.bindAdditionalCurrencies(totalcus);
        __posCore.bindAdditionalCurrenciesReceived();
        if (__posCore.toNumber(order.Received) < __posCore.toNumber(order.GrandTotal)) {
            $(".error-change").addClass("fn-red").removeClass("fn-green");
            $('#pay-received').addClass("fn-red");
            $('.pay-received').addClass("fn-red");
        }
        else {
            $(".error-change").removeClass("fn-red").addClass("fn-green");
            $('#pay-received').removeClass("fn-red");
            $('.pay-received').removeClass("fn-red");
        }
        let changes = order.Change.toString().split(".");
        let xtChange = __posCore.toNumber("." + changes[1]);
        $("#pay-total").val(__posCore.toCurrency(order.GrandTotal));
        $("#pay-total-alt").val(__posCore.toCurrency(order.GrandTotal * baseCurrency.AltRate, baseCurrency.DecimalPlaces));
        $(".base-currency").text(baseCurrency.BaseCurrency);
        $(".alt-currency").text(baseCurrency.AltCurrency);
        $('#pay-received').val(__posCore.toCurrency(order.Received, baseCurrency.DecimalPlaces));
        $('#pay-change').val(__posCore.toCurrency(order.Change, baseCurrency.DecimalPlaces));
        $("#post-date").val(order.PostingDate);
        $('#pay-change-alt').val(__posCore.toCurrency(order.Change * baseCurrency.AltRate, baseCurrency.DecimalPlaces));
        $("#apply-promo").text(__orderInfo.Order.PromoCodeDiscRate + "%");

    }

    //........................Grid button clicked......................................//
    const numberPad = $("#panel-payment .number-pad .grid");
    const numberMultiply = $("#panel-payment .number-multiply .grid");
    const numberPlus = $("#panel-payment .number-plus .grid");
    let value = "";
    let containerPaymentMean = $(".container_payment_means");
    var activeInput = $(containerPaymentMean).find(".input-number-pad")[0];
    $("input[class='number']").asNumber();
    let inputValue = 0;
    numberPlus.click(function (e) {
        onQuickNumber(this, true);
    });

    numberMultiply.click(function (e) {
        onQuickNumber(this, false);
    });

    numberPad.on("click", function (e) {
        let input_value = activeInput.value;
        if (input_value !== undefined) {
            value = input_value + $(this).data("value").toString();
            value = $.validNumber(value);
            activeInput.value = value;
            if ($(this).data("value") === -1) {
                activeInput.value = input_value.substring(0, input_value.length - 1);
                if (activeInput.value === "") {
                    activeInput.value = 0;
                }
            }
            setOpenAmountMultipayment();
        }

        $(activeInput).focus();
    });

    function setOpenAmountMultipayment() {
        __orderInfo.Order.MultiPaymentMeans.forEach((s, index) => {
            if (s.PaymentMeanID == obj.PaymentMeanID) {
                s.OpenAmount = 1;
            }
            else {
                s.OpenAmount = 0;
            }
        })
    }

    function onQuickNumber(input, isPlus) {
        let value = __posCore.toNumber($("#pay-other-currency").val());
        if (isNaN(value)) {
            $('#pay-other-currency').val(0);
        }

        inputValue = __posCore.toNumber($(activeInput).val());
        if (isPlus) {
            inputValue += __posCore.toNumber($(input).data("value"));
        } else {
            inputValue *= __posCore.toNumber($(input).data("value"));
        }

        setOpenAmountMultipayment();
        $(activeInput).val(inputValue);
        activeInput.selectionStart = activeInput.value.length;
        $(activeInput).focus();
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
    function findInArray(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }

    $("#other-payment").click(function () {
        __posCore.checkPrivilege(
            "P008",
            function (orderInfo) {
                const otherPaymentDialog = new DialogBox({
                    caption: "Information of Other Payment",
                    icon: "fas fa-user-friends",
                    content: {
                        selector: "#other-payment-dialog-contianer"
                    },
                    type: "yes-no",
                    button: {
                        no: {
                            class: "bg-red",
                            text: "Close"
                        },
                        yes: {
                            class: "bg-red",
                            text: "Ok"
                        }
                    }
                });
                otherPaymentDialog.invoke(function () {
                    __posCore.readyPayment = false;
                    const cardElement = otherPaymentDialog.content.find("#card-number");
                    if (orderInfo.CardMemberNumber) {
                        cardElement.val(orderInfo.CardMemberNumber);
                    }
                    if (orderInfo.CardMemberOption == 1) { /// Scan Card Member                      
                        initScanCardMember(orderInfo, otherPaymentDialog);
                    }
                    if (orderInfo.CardMemberOption == 0) { /// Manual input card member number
                        initManualCardMember(cardElement, orderInfo, otherPaymentDialog);
                    }

                    $("#clear-card-payment").click(function () {
                        otherPaymentDialog.shutdown();
                        clearCardMemberPayment(cardElement);
                    })
                })
                otherPaymentDialog.confirm(function () {
                    otherPaymentDialog.shutdown()
                })
                otherPaymentDialog.reject(function () {
                    otherPaymentDialog.shutdown()
                })
            }
        )
    })

    function setPaymentMeans(onAfterCheck) {
        //$.each(__orderInfo.PaymentMeans, function (i, p) {
        //    let _payment = $(".checkboxpayment")[i];
        //    $(_payment).prop("checked", p.IsChecked);
        //});
        if (typeof onAfterCheck === "function") {
            onAfterCheck.call(this, __orderInfo.PaymentMeans.filter(p => p.IsChecked));
        }
    }

    let num;
    let MultipayMeansSetting = {
        ID: 0,
        SettingID: 0,
        PaymentID: 0,
        Check: false,
        Changed: false,
    }

    $("#goto-panel-payment").click(function () {
        $.ajax({
            url: "/POS/GetPaymentMeansSelect",
            type: "get",
            success: function (res) {
                res.forEach(i => {
                    renderInputsPaymentMeans(".container_payment_means", res, onInput, onSelect, i.AltCurrencyID);
                })

            }
        })
        //====================delay hours==========================
       
        let hours;
        $.ajax({
            url: "/POS/GetDelayhours",
            type: "get",
            success: function (res) {
                    hours = res.DelayHours;
                    $("#delayhours").val(hours);
                    if (hours > 0) {
                        // const result = addHours(hours, this.value);
                        // let myDate = (result.getFullYear()) + "-" + (result.getMonth() + 1) + "-" + (result.getDate());
                        // $("#post-date-delay").val(myDate);
                        // function addHours(hours, date = new Date()) {
                        //     const dateCopy = date;
                        //     dateCopy.setTime(dateCopy.getTime() - hours * 60 * 60 * 1000);
                        //     dateCopy.getUTCFullYear() + "/" + (dateCopy.getMonth() + 1) + "/" + dateCopy.getDate() + " " + dateCopy.getHours() + ":" + dateCopy.getMinutes() + ":" + dateCopy.getSeconds();
                        //     return dateCopy;
                        // }
                            var result  = new Date();
                            var end     = result.setTime(result.getTime() - hours * 60 * 60 * 1000);
                            $("#post-date").val(dateToYMD(end));
                    
                        function dateToYMD(end_date) {
                            var ed = new Date(end_date);
                            var d = ed.getDate();
                            var m = ed.getMonth() + 1;
                            var y = ed.getFullYear();
                            return '' + y + '-' + (m<=9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d);
                        }
                    }
                    else if(hours==0){
                        $.each($("[data-date]"), function (i, t) {
                            setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
                        });
                        function setDate(selector, date_value) {
                            var _date = $(selector);
                            _date[0].valueAsDate = new Date(date_value);
                            _date[0].setAttribute(
                                "data-date",
                                moment(_date[0].value)
                                    .format(_date[0].getAttribute("data-date-format"))
                            );
                        }
                    }

            }
        })
        //=====================end delay hours====================


        function getCardMemberPaymentSetting(onSuccess) {
            $.ajax({
                url: "/POS/GetMultipaymentCardMemberSetting",
                type: "get",
                dataType: "Json",
                success: onSuccess
            });
        }
        getCardMemberPaymentSetting(function (paymentSetting) {
            paymentSetting.forEach(i => {
                if (i.Check) {

                    $(".other-pay-way").css("display", "flex");
                    $(".card").prop("checked", true);

                }

            })
        });
    })


    $(".card").change(function () {
        if (this.checked == true) {
            $(".other-pay-way").css("display", "flex");
        }

        if (this.checked == false) {
            $(".other-pay-way").css("display", "none");
        }
        MultipayMeansSetting.SettingID = __orderInfo.Setting.ID;
        MultipayMeansSetting.PaymentID = 0;
        MultipayMeansSetting.Check = this.checked;

        $.ajax({
            url: "/POS/InsertSettingMultipayMeans",
            type: "post",
            dataType: "Json",
            data: { data: MultipayMeansSetting },
            success: function (response) {
            }
        });
    })
    function onPaymentMeanChanged(paymentMean = {}, paymentMeanId = 0) {
        let paymentMeans = $.grep(__orderInfo.PaymentMeans, function (pm) {
            return pm.IsChecked;
        });
        renderInputsPaymentMeans(".container_payment_means", paymentMeans, onInput, onSelect);
        
        MultipayMeansSetting.SettingID = __orderInfo.Setting.ID;
        MultipayMeansSetting.PaymentID = paymentMean.ID;
        MultipayMeansSetting.Check = paymentMean.IsChecked;
        MultipayMeansSetting.Changed = paymentMean.IsReceivedChange;
        MultipayMeansSetting.AltCurrencyID = paymentMean.AltCurrencyID
        $.ajax({
            url: "/POS/InsertSettingMultipayMeans",
            type: "post",
            dataType: "Json",
            data: { data: MultipayMeansSetting },
            success: function (response) {
            }
        });
    }
    function onInput(multiPaymentMean, currency) {
        $(this).asNumber();
        //$("#pay-cash").val(0);
        updateMultiPaymentMean(multiPaymentMean.PaymentMeanID, "Amount", __posCore.toNumber(this.value));
        updateMultiPaymentMean(multiPaymentMean.PaymentMeanID, "Total", __posCore.toNumber(this.value));
        obj = multiPaymentMean;

        sumMultiPaymentMeans(currency);
    }

    function onSelect(multiPaymentMean, currency) {
        multiPaymentMean.AltCurrencyID = this.value;
        sumMultiPaymentMeans(currency);
    }
    function renderInputsPaymentMeans(container_payment_means, paymentMeans, onInput, onSelect) {
        let _container = $(container_payment_means);
        _container.children().remove();
        for (let i = 0; i < paymentMeans.length; i++) {
            // let _wrapper = createInputWrapper(paymentMeans[i].ID, paymentMeans[i].Type, __orderInfo.DisplayPayOtherCurrency, onInput, onSelect); old
            let _wrapper = createInputWrapper(paymentMeans[i].ID, paymentMeans[i].Type, __orderInfo.DisplayPayOtherCurrency, onInput, onSelect, paymentMeans[i].AltCurrencyID);
            _container.append(_wrapper);
        }
    }
    var obj = {};
    function totalMultiPaymentMeans() {
        var sumAmount = 0;
        if (isValidArray(__orderInfo.Order.MultiPaymentMeans)) {
            for (var i = 0; i < __orderInfo.Order.MultiPaymentMeans.length; i++) {
                sumAmount += __orderInfo.Order.MultiPaymentMeans[i].Amount * __orderInfo.Order.MultiPaymentMeans[i].PLRate;

            }
        }
       
        return sumAmount;
    }
    function sumMultiPaymentMeans(currency = {}) {
        displayPayment(__orderInfo.Order);
    }
    function createInputWrapper(paymentMeanId, displayName, currencies = [], onInput, onSelect, altcurrencyId) {
        if (isValidArray(currencies)) {
            let _multiPaymentMean = findMultiPaymentMean(paymentMeanId);
            let _wrapper = $("<div class='stack-inline'></div>");
            // let _select = createSelectCurrency(paymentMeanId, currencies, onSelect);
            let _select = createSelectCurrency(paymentMeanId, currencies, onSelect, altcurrencyId);
            let _input = $(`<input autocomplete="off" style='color:black !important;'  class="number input-number-pad">`);
            let _label = $('<div class="other-curr-symbol symbol bg-grey">' + displayName + '</div>');
            let currency = findInArray("AltCurrencyID", _select[0].value, __orderInfo.DisplayPayOtherCurrency);
            //let currency = __orderInfo.DisplayPayOtherCurrency.filter(c => c.IsLocalCurrency)[0];
            let multiPaymentMean = {
                ReceiptID: 0,
                Type: 0,
                PaymentMeanID: paymentMeanId,
                AltCurrencyID: currency.AltCurrencyID,
                AltCurrency: currency.AltCurrency,
                AltRate: currency.AltRate,
                PLCurrencyID: currency.BaseCurrencyID,
                PLCurrency: currency.BaseCurrency,
                PLRate: currency.Rate,
                LCRate: currency.LCRate,
                SCRate: currency.Rate,
                //SCRate: currency.SCRate,
                Total: 0,
                Amount: 0,
                Change: 0,
                Exceed: false,
                OpenAmount: 0,
            };
            // console.log("defaultamount",__orderInfo)
            // if(__orderInfo.Setting.DefualtAmount){
            //     if (isValidJSON(_multiPaymentMean)) {
            //         multiPaymentMean.Amount = _multiPaymentMean.Amount;
            //         _input[0].value = multiPaymentMean.Amount;
            //     }
            // }
            // if(__orderInfo.Setting.DefualtAmount){
            //     _input[0].value = multiPaymentMean.Amount;
            // }
            mergeMultiPaymentMean(multiPaymentMean);
            _input[0].value = multiPaymentMean.Amount;
            _wrapper.append(_select);
            _wrapper.append(_input[0]);
            _wrapper.append(_label);
            _input.on("keyup focus", function (event) {
                if (event.type == "focus") {
                    activeInput = this;
                }
                if (event.type != "focus") {
                    __orderInfo.Order.MultiPaymentMeans.forEach(s => {
                        if (s.PaymentMeanID == obj.PaymentMeanID) {
                            s.OpenAmount = 1;
                        }
                        else {
                            s.OpenAmount = 0;
                        }
                    })

                }

                if (this.value == "") { this.value = "0"; }
                if (typeof onInput === "function") {
                    onInput.apply(event.target, [multiPaymentMean, currency]);
                }
            });
            activeInput = _input[0];
            if(__orderInfo.Order.MultiPaymentMeans[0] !=undefined){
                if(__orderInfo.Order.MultiPaymentMeans[0].PaymentMeanID==paymentMeanId){
                    if(_multiPaymentMean !=undefined){
                        _input[0].value = __orderInfo.Order.GrandTotal*__orderInfo.Order.MultiPaymentMeans[0].AltRate;
                    }
                }
           }
            return _wrapper;
        }
    }

    function createSelectCurrency(paymentMeanId, currencies, onSelect, altcurrencyId) {
        let multiPaymentMean = findMultiPaymentMean(paymentMeanId);
        let _select = $("<select class='symbol bg-grey'></select>");
        let currency = findCurrencyById(altcurrencyId);
        
        for (let i = 0; i < currencies.length; i++)
         {
            let _option = $("<option value='" + currencies[i].AltCurrencyID + "'>" + currencies[i].AltCurrency + "</option>");
            if(currencies[i].AltCurrencyID == altcurrencyId){
                $(_option).attr("selected", "selected");
            }         
             _select.append(_option);
        }
        toMultiPayentMean(multiPaymentMean, currency);
        mergeMultiPaymentMean(multiPaymentMean);
        if (typeof onSelect === "function") {
            sumcalselect(multiPaymentMean, currency, altcurrencyId);
        }
        _select.on("change", function (event) {
            multiPaymentMean = findMultiPaymentMean(paymentMeanId);
            let currency = findCurrencyById(this.value);
            toMultiPayentMean(multiPaymentMean, currency);
            mergeMultiPaymentMean(multiPaymentMean);
            if (typeof onSelect === "function") {
                onSelect.apply(event.target, [multiPaymentMean, currency, event]);
            }
        });
        return _select;
    } 
    function sumcalselect(multiPaymentMean, currency, altcurrencyId) {
        //  multiPaymentMean.AltCurrencyID = altcurrencyId;
        sumMultiPaymentMeans(currency);
    }
    function updateMultiPaymentMean(paymentMeanId, keyName, keyValue) {
        let _multiPMs = __orderInfo.Order.MultiPaymentMeans;
        for (let i = 0; i < _multiPMs.length; i++) {
            if (paymentMeanId == _multiPMs[i].PaymentMeanID) {
                _multiPMs[i][keyName] = keyValue;
            }
        }

    }

    // type[ 0: Normal, 1: CardMember ]
    function mergeMultiPaymentMean(multiPaymentMean, type = 0) {
        if (isValidJSON(multiPaymentMean)) {
            let _multiPaymentMean = findMultiPaymentMean(multiPaymentMean.PaymentMeanID, type);
            if (isValidJSON(_multiPaymentMean)) {
                $.extend(_multiPaymentMean, multiPaymentMean);
            } else {
                __orderInfo.Order.MultiPaymentMeans.push(multiPaymentMean);
            }
        }
    }

    function removeMultiPaymentMean(paymentMeanId) {
        let _multiPaymentMeans = __orderInfo.Order.MultiPaymentMeans;
        if (isValidArray(_multiPaymentMeans)) {
            __orderInfo.Order.MultiPaymentMeans = $.grep(_multiPaymentMeans, function (p) {
                return p.PaymentMeanID != paymentMeanId;
            });
        }
    }

    // type[ 0: Normal, 1: CardMember ]
    function findMultiPaymentMean(paymentMeanId, type = 0) {
        return $.grep(__orderInfo.Order.MultiPaymentMeans, function (c) {
            return c.PaymentMeanID == paymentMeanId && c.Type == type;
        })[0];
    }
    function findCurrencyById(currencyId) {     
            return __orderInfo.DisplayPayOtherCurrency.filter(c => c.AltCurrencyID == currencyId)[0];  
    }
    function toMultiPayentMean(multiPaymentMean = {}, currency = {}) {
        if (isValidJSON(currency) && isValidJSON(multiPaymentMean)) {
            multiPaymentMean.AltCurrencyID = currency.AltCurrencyID;
            multiPaymentMean.AltCurrency = currency.AltCurrency;
            multiPaymentMean.AltRate = currency.AltRate;
            multiPaymentMean.PLCurrencyID = currency.BaseCurrencyID;
            multiPaymentMean.PLCurrency = currency.BaseCurrency;
            multiPaymentMean.PLRate = currency.Rate;
            multiPaymentMean.LCRate = currency.LCRate;
            multiPaymentMean.SCRate = currency.Rate;
            multiPaymentMean.OpenAmount = 0;
        }
        return multiPaymentMean;
    }

    function createMultiPaymentMean(paymentMeanId, currency = {}, amount = 0, type = 0) {
        if (typeof parseInt(paymentMeanId) === "number") {
            let _multiPaymentMean = {
                ReceiptID: 0,
                Type: type,
                PaymentMeanID: paymentMeanId,
                AltCurrencyID: currency.AltCurrencyID,
                AltCurrency: currency.AltCurrency,
                AltRate: currency.AltRate,
                PLCurrencyID: currency.BaseCurrencyID,
                PLCurrency: currency.BaseCurrency,
                LCRate: currency.LCRate,
                SCRate: currency.SCRate,
                PLRate: currency.Rate,
                Amount: amount,
                OpenAmount: 0,

            };
            return _multiPaymentMean;
        }
    }

}