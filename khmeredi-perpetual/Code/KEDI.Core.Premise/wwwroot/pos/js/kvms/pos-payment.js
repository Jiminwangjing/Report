"use strict";
function POSPayment(KPOS) {
    if (!(this instanceof POSPayment)) {
        return new POSPayment(KPOS);
    }

    var utilskvms = new UtilsKVMS(KPOS);

    KPOS.onPanelPayment(function (info) {
        defineOtherCurrencies(info.Order, info.DisplayCurrency);
        $("#pay-cash").on("keyup focus", function (e) {
            let check = parseFloat($("#pay-cash").val());
            if (isNaN(check)) {
                $('#pay-cash').val(0);
            }

            calulatePayment(info.Order, info.DisplayCurrency);
        });

        $("#pay-other-currency").on("keyup focus", function () {
            let value = parseFloat($("#pay-other-currency").val());
            let cash = parseFloat($("#pay-cash").val());
            if (isNaN(cash)) {
                $('#pay-cash').val(0);
            }

            if (isNaN(value)) {
                $('#pay-other-currency').val(0);
            }

            calulatePayment(info.Order, info.DisplayCurrency);
        });
        //VMC Edtion
        //Process pay order
        $("#pay").off().on("click", function () {
            info.Order.PaymentMeansID = $("#payment-mean-id").val();
            info.Order.DisplayRate = info.DisplayCurrency.Rate;

            KPOS.payOrder(function (e) {
                //KPOS.resetOrder();
                clearPayment();
                $("#fx-save-quotation").html("<i class='fa fa-save'></i> Save & Preview Quote ");

                $.post("/KVMS/DefaultCusN", { CusId: e.Setting.CustomerID }, function (e) {
                    $('.change-customer').text(e.CusName);
                });

            });
        });
    });
    //End of VMC Edition
    function defineOtherCurrencies(order, currency) {
        $("#other-currency-list").children().remove();
        $("#other-currency-list")
            .append("<option value='" + currency.BaseCurrencyID + "'>" + currency.BaseCurrency + "</option>")
            .append("<option value='" + currency.AltCurrencyID + "'>" + currency.AltCurrency + "</option>")
            .on("change", function () {
                $(".other-curr-symbol").text($(this).find("option:selected").text());
                calulatePayment(order, currency);
            });
        $(".other-curr-symbol").text(currency.BaseCurrency);
        calulatePayment(order, currency);
        setTimeout(function () {
            activeInput.selectionStart = activeInput.value.length;
            $(activeInput).focus();
        }, 0);
    }

    function clearPayment(currentOrder) {
        $('#pay-cash').val(0);
        $("#pay-total").val(0);
        $("#pay-change").val(0);
        $('#pay-change-sys').val(0);
        $("#pay-total-alt").val(0);
        $("#pay-change-alt").val(0);
    }

    function calulatePayment(order, currency) {
        if ($("#other-currency-list").val() == currency.AltCurrencyID) {
            order.Received = parseFloat($("#pay-other-currency").val()) / currency.Rate;
        } else {
            order.Received = parseFloat($("#pay-other-currency").val());
        }
        order.Received += parseFloat($("#pay-cash").val());
        order.PaymentMeansID = $("pay-payment-means-id").val();
        order.Change = parseFloat(order.Received) - parseFloat(order.GrandTotal);
        if (parseFloat(order.Received) < parseFloat(order.GrandTotal)) {
            $(".error-change").addClass("fn-red").removeClass("fn-green");
            $('#pay-received').addClass("fn-red");
        }
        else {
            $(".error-change").removeClass("fn-red").addClass("fn-green");
            $('#pay-received').removeClass("fn-red");
        }

        $("#pay-total").val(order.GrandTotal.toFixed(3));
        $("#pay-total-alt").val(order.GrandTotal * currency.Rate);
        $(".base-currency").text(currency.BaseCurrency);
        $(".alt-currency").text(currency.AltCurrency);
        $('#pay-received').val(order.Received.toFixed(3));
        $('#pay-change').val(order.Change.toFixed(3));
        $('#pay-change-alt').val((order.Change * currency.Rate).toFixed(3));
    }

    //........................Grid button clicked......................................//
    const numberPad = $("#panel-payment .number-pad .grid");
    const numberMultiply = $("#panel-payment .number-multiply .grid");
    const numberPlus = $("#panel-payment .number-plus .grid");
    let value = "";
    var activeInput = document.querySelector("#pay-cash");
    $("input[class='number']").asNumber();
    $("input[class='number']").focus(function (e) {
        activeInput = this;
    });

    let q_val = 0;
    numberPlus.click(function (e) {
        onQuickNumber(this, true);
    });

    numberMultiply.click(function (e) {
        onQuickNumber(this, false);
    });

    numberPad.on("click", function (e) {
        let input_value = activeInput.value;
        if (input_value !== undefined) {
            setTimeout(() => {
                activeInput.selectionStart = activeInput.value.length;
                $(activeInput).focus();
            }, 0);

            value = input_value + $(this).data("value").toString();
            value = $.validNumber(value);
            activeInput.value = value;
            if ($(this).data("value") === -1) {
                activeInput.value = input_value.substring(0, input_value.length - 1);
                if (activeInput.value === "") {
                    activeInput.value = 0;
                }
            }
        }

    });

    function onQuickNumber(input, isPlus) {
        let value = parseFloat($("#pay-other-currency").val());
        let cash = parseFloat($("#pay-cash").val());
        if (isNaN(cash)) {
            $('#pay-cash').val(0);
        }
        if (isNaN(value)) {
            $('#pay-other-currency').val(0);
        }

        q_val = parseFloat($(activeInput).val());
        if (isPlus) {
            q_val += parseFloat($(input).data("value"));
        } else {
            q_val *= parseFloat($(input).data("value"));
        }

        $(activeInput).val(q_val);
        activeInput.selectionStart = activeInput.value.length;
        $(activeInput).focus();
    }
}