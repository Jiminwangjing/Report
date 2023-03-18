//Go to payment detail

$("#goto-panel-group-items").click(function (e) {
    $("#panel-payment").removeClass("show");
    $("#panel-group-tables").removeClass("show");
    $(".nav-header .min-max").addClass("show");
    clearPayment();
});

$(".goto-panel-group-tables").click(function (e) {
    $("#panel-group-tables").addClass("show");
    $("#panel-payment").removeClass("show");
    $("#panel-group-items").removeClass("show");
    $(".nav-header .min-max").removeClass("show");
   
    clearPayment();
});

//Intail data
let curr_sys_symbol = db.from("tb_exchange_rate").where(w => { return w.CurrencyID === setting.SysCurrencyID });
//$('.other-currency-list').val(setting.SysCurrencyID);
//$('.other-curr-symbol').text($('.other-currency-list').find("option:selected").text());
$('.print-receipt-pay').prop('checked', setting.PrintReceiptTender);
$('.payment-means-id').val(setting.PaymentMeansID);
$('.sys-symbol').text(curr_sys_symbol[0].Currency.Description);
$('.other-curr-symbol').text(curr_sys_symbol[0].Currency.Description);

$('.payment-means-id').change(function () {
    $('#pay-cash').focus();
});
//End
$("#goto-panel-payment").on("click", function () {
    let user_privillege = db.table("tb_user_privillege").get('P008');
    if (user_privillege.Used === false) {
        let dlg = new DialogBox({
            // close_button: false,
            position: "top-center",
            content: {
                selector: "#admin-authorization",
                class: "login"
            },
            icon: "fas fa-lock",
            button: {
                ok: {
                    text: "Login",
                    callback: function (e) {
                        let access = accessSecurity(this.meta.content, 'P008');
                        if (access === false) {
                            this.meta.content.find('.error-security-login').text('You can not access ...!');
                            return;
                        }
                        else {
                            this.meta.content.find('.security-username').focus();
                            this.meta.setting.icon = "fas fa-lock fa-spin";
                            this.text = "Logging...";
                            this.meta.content.find('.error-security-login').text('');
                            this.meta.build(this.setting);
                            setTimeout(() => {
                                this.meta.build(this.setting);
                                this.meta.setting.icon = "fas fa-unlock-alt";
                                setTimeout(() => {
                                    this.meta.shutdown();
                                    confirmPay();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {
        confirmPay();
    }

});
function confirmPay() {
    if (db.from("tb_check_open_shift") === 0) {
        let msg = new DialogBox(
            {
                caption: "Information",
                content: "Please open shift before pay...!",
                position: "top-center",
                type: "ok",//ok, ok-cancel, yes-no, yes-no-cancel
                icon: "info" //info, warning, danger
            }
        );
        msg.setting.button.ok.callback = function (e) {
            this.meta.shutdown();
        }
        return;
    }
    if ($("#item-listview").find('tr:not(:first-child)').length !== 0) {

        //let fac_exhange = db.table("tb_exchange_rate").get(1);//order_master.GrandTotal * fac_exhange.Rate;
        let total_pay = fx.convert(order_master.GrandTotal, { from: fx.base, to: dis_curr[0].AltCurr });
        if (dis_curr[0].AltCurr === 'KHR' || dis_curr[0].AltCurr === '៛') {
            $('#pay-total-sys').val(numeral(total_pay).format('0.000'));
        }
        else {
            $('#pay-total-sys').val(total_pay.toFixed(2));
        }
        $('.display-symbol').text(dis_curr[0].AltCurr);
        calulatePayment();
    }
    else {
        let msg = new DialogBox(
            {
                caption: "Information",
                content: "Data was empty...!",
                position: "top-center",
                type: "ok",
                icon: "info"
            }
        );
        msg.setting.button.ok.callback = function (e) {
            this.meta.shutdown();
        }
        return;
    }
    gotoPaymentPanel(); 
}
function gotoPaymentPanel() {
    $("#panel-payment").addClass("show");
    $("#panel-group-tables").removeClass("show");
    $("#panel-group-items").addClass("hide");
    $(".nav-header .min-max").removeClass("show");
    setTimeout(() => {
        $("#panel-payment input#pay-cash").val("").focus();
        //$("#pay-other-currency").val("");
    }, 100);
}
//Key up in text box cash
$("#pay-cash").keyup(function (e) {
    let check = parseFloat($("#pay-cash").val());
    if (isNaN(check)) {
        $('#pay-cash').val(0);
        $('#pay-change-display-curr-fx').val('');
    }

    calulatePayment();
});


$("#pay-other-currency").keyup(function () {
    let value = parseFloat($("#pay-other-currency").val());
    let cash = parseFloat($("#pay-cash").val());
    if (isNaN(cash)) {
        $('#pay-cash').val(0);
        $('#pay-change-display-curr-fx').val('');
    }
    if (isNaN(value)) {
        $('#pay-other-currency').val(0);
        $('#pay-change-display-curr-fx').val('');
    }
    calulatePayment();
});
$("#pay-payment-means").keyup(function () {

    let check = parseFloat($("#pay-payment-means").val());
    if (isNaN(check)) {
        $('#pay-payment-means').val(0);
    }
    calulatePayment();
});
$('#other-currency-list').change(function () {
    $('.other-curr-symbol').text($(this).find("option:selected").text());
    let value = parseFloat($("#pay-other-currency").val());
    let cash = parseFloat($("#pay-cash").val());
    if (isNaN(cash)) {
        $('#pay-cash').val(0);
        $('#pay-change-display-curr-fx').val('');
    }
    if (isNaN(value)) {
        $('#pay-other-currency').val(0);
        $('#pay-change-display-curr-fx').val('');
    }
    calulatePayment();
});

//Click button pay
$("#pay").click(function () {
    pay();
});

function pay() {
    //Pay
    if (order_master.Change >= 0) {
        if (order_master.OrderID === 0) {
            if (setting.AutoQueue == false) {
                setOrderNo('Pay');
            }
            else {
                $("#send").text("Sending ...");
                sendData(0, 'Pay');
            }
        }
        else {
            $("#send").text("Updating ...");
            sendData(order_master.OrderID, 'Pay');
        }
        clearPayment();
    }
    else {
        let msg = new DialogBox(
            {
                caption: "Information",
                content: "Cash not engouh ...!",
                position: "top-center",
                type: "ok",//ok, ok-cancel, yes-no, yes-no-cancel
                icon: "info" //info, warning, danger
            }
        );
        msg.setting.button.ok.callback = function (e) {
            this.meta.shutdown();
        };

    }
}
function calulatePayment() {
   
    let cash = parseFloat($('#pay-cash').val());
    let pay_means = parseFloat($('#pay-payment-means').val());
    if (isNaN(pay_means)) {
        pay_means = 0;
    }
    let pay_other_curr = parseFloat($('#pay-other-currency').val());
    //let pay_other_curr_id = parseInt($("#other-currency-list").val());

    let dis_alt_curr = $('.other-curr-symbol').text();

    let conv_other_pay =fx.convert(pay_other_curr, { from: dis_alt_curr, to: fx.base });
    order_master.Received = cash + pay_means + conv_other_pay;
   
    order_master.Change = order_master.Received - (order_master.GrandTotal);
    let conv_curr_change = fx.convert(order_master.Change, { from: fx.base, to: dis_curr[0].AltCurr });
    order_master.PaymentMeansID = $("pay-payment-means-id").val();
    if (order_master.Change < 0) {
        $(".error_enghoug").css("color", "red");
        $('#pay-received').css("color", "black");
    }
    else {
        $(".error_enghoug").css("color", "green");
        $('#pay-received').css("color", "red");
        //toFxied change
        let to_curr = db.from('tb_display_curr');
        var change = order_master.Change.toString().split('.');
        let fix_change = parseFloat('.' + change[1]);
        
        let fixed_change = fx.convert(fix_change, { from: fx.base, to: to_curr[0].AltCurr });
        if (!isNaN(fix_change)) {
            if (dis_curr[0].AltCurr === 'KHR' || dis_curr[0].AltCurr === '៛') {
                $('#pay-change-display-curr-fx').val(fx.base + ' ' + fix_change.toFixed(3) + '= ' + to_curr[0].AltCurr + ' ' + numeral(fixed_change).format('0.000'));
            }
            else {
                $('#pay-change-display-curr-fx').val(fx.base + ' ' + fix_change.toFixed(3) + '= ' + to_curr[0].AltCurr + ' ' + fixed_change);
            }
        }
        else {
            $('#pay-change-display-curr-fx').val('');
        }

    }
    if (local_currency.symbol == 'KHR' || local_currency.symbol == '៛') {
        $("#pay-total").val(numeral(order_master.GrandTotal).format('0.000'));
        $('#pay-received').val(numeral(order_master.Received).format('0.000'));
        $('#pay-change').val(numeral(order_master.Change).format('0.000'));
        $('#pay-change-display-curr').val(numeral(conv_curr_change).format('0.000'));
    }
    else {
        $("#pay-total").val(order_master.GrandTotal.toFixed(3));
        $('#pay-received').val(order_master.Received.toFixed(3));
        $('#pay-change').val(order_master.Change.toFixed(3));
        $('#pay-change-display-curr').val(numeral(conv_curr_change).format('0.000'));

    }
    if (dis_curr[0].AltCurr === 'KHR' || dis_curr[0].AltCurr === '៛') {
        $('#pay-change-display-curr').val(numeral(conv_curr_change).format('0.000'));
    }
    else {
        $('#pay-change-display-curr').val(numeral(conv_curr_change).format('0.000'));

    }
    
}
function clearPayment() {
    $("#pay-total").val(0);
    $("#pay-change").val(0);
    $('#pay-change-sys').val(0);
    $('#pay-cash').val(0);
    $('#pay-other-currency').val(0);
    $('#pay-payment-means').val(0);
    $('#pay-cash').val(0);
    $('#pay-change-display-curr-fx').val('');
}
//........................Grid button clicked......................................//
const number_pad = $("#panel-payment .number-pad .grid");
const quick_number = $("#panel-payment .quick-number .grid");
let value = "";
var input_flag = $();
$(".number").val(0);
$(".number").prop("maxlength", 12);
number_pad.click(function (e) {
    let input_value = input_flag.value;
    if (input_value !== undefined && input_value.length <= 12) {
        setTimeout(() => {
            input_flag.selectionStart = input_flag.value.length;
            $(input_flag).focus();
        }, 0);
     
        value = input_value + $(this).data("value").toString();
        value = _$_.validNumber(value);
        input_flag.value = value;
        if ($(this).data("value") === -1) {
            input_flag.value = input_value.substring(0, input_value.length - 1);
            if (input_flag.value === "") {
                input_flag.value = 0;
            }
            
        }
        calulatePayment();
    }
   
});


let q_val = 0;
quick_number.click(function (e) {
    let value = parseFloat($("#pay-other-currency").val());
    let cash = parseFloat($("#pay-cash").val());
    if (isNaN(cash)) {
        $('#pay-cash').val(0);
        $('#pay-change-display-curr-fx').val('');
    }
    if (isNaN(value)) {
        $('#pay-other-currency').val(0);
        $('#pay-change-display-curr-fx').val('');
    }
    q_val = parseFloat($(input_flag).val());
    q_val += parseFloat($(this).data("value"));
    
    $(input_flag).val(q_val);
    calulatePayment();
});

$("input[class=number]").focus(function (e) {
    input_flag = this;
});

let timer;
$(".number").on("keyup", function (e) {
    clearTimeout(timer);
    timer = setTimeout(() => {
        if (parseFloat(this.value) === 0) {
            this.value = 0;
        }
    }, 1500);

    if (this.value === "" && e.keyCode === 8) {
        this.value = 0;
    }

    this.value = _$_.validNumber(this.value);

});



