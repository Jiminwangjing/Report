$("#goto-main-menu").click(function (e) {
    let main_menu = new DialogBox(
        {
            icon: "far fa-list-alt",
            close_button: true,
            content: {
                selector: "#main-menu-content",
            },
            caption: "Menu",
            position: "top-center",
            type: "ok",
            button: {
                ok: {
                    text:"Close",
                    callback: function (e) {
                        this.meta.shutdown();
                    }
                }
            }
        }
    );
    let main_menu_setting = main_menu.setting;
    main_menu.setting.button.ok.text = "Cancel";
    main_menu.setting.animation.shutdown.animation_type = "slide-up";
});
$('#goto-set-print-receipt').click(function (e) {
    confirmSetPrinter();
});
function confirmSetPrinter() {
    let user_privillege = db.table("tb_user_privillege").get('P016');
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
                        let access = accessSecurity(this.meta.content, 'P016');
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
                                    initFromSetprintReceipt();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFromSetprintReceipt();
    }
}
function initFromSetprintReceipt() {
    let set_printer = new DialogBox({
        position: "top-center",
        caption: "Set Printer Name",
        content: {
            selector: "#set-print-receipt-content"
        },
        animation: {
            startup: {
                delay: 0,
                duration: 0
            }
        },
        button: {
            ok: {
                text: "Done",
                callback: function (e) {
                    let content=$(this.meta.content);
                    let PrinterName = {
                        Name: content.find(".set-printer-name").val(),
                        MachineName: ""
                    };
                    $.ajax({
                        url: '/POS/SetPrinterName',
                        async: false,
                        type: 'POST',
                        data: { printer: PrinterName },
                        success: function () {

                        }
                    });
                    this.meta.shutdown();
                }
            }
        },
        icon: "fas fa-print"
    });
}
$("#goto-reprint").click(function (e) {
    confirmReprint();
});
function confirmReprint() {
    let user_privillege = db.table("tb_user_privillege").get('P014');
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
                        let access = accessSecurity(this.meta.content, 'P014');
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
                                    initFormReprint();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormReprint();
    }
}
function initFormReprint() {
    let reprint = new DialogBox({
        position: "top-center",
        caption: "Reprint",
        content: {
            selector: "#reprint-content"
        },
        animation: {
            startup: {
                delay: 0,
                duration: 0
            }
        },
        button: {
            ok: {
                text: "Done",
                callback: function (e) {
                    this.meta.shutdown();
                }
            }
        },
        icon: "fas fa-print"
    });
    reprint.startup("before", function (dialog) {
        let receipts = $.ajax({
            url: '/POS/GetReceiptReprint',
            async: false,
            type: 'GET',
            data: { branchid: order_info.branchid, date_from: null, date_to: null },
            success: function () {

            }

        }).responseJSON;
        initDataReceipt(receipts);
    });
}

$("#goto-setting").click(function (e) {
    confirmSetting();
});
function confirmSetting() {
    let user_privillege = db.table("tb_user_privillege").get('P015');
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
                        let access = accessSecurity(this.meta.content, 'P015');
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
                                    initFormSetting();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormSetting();
    }
}
function initFormSetting() {
    let pos_setting = new DialogBox({
        position: "top-center",
        caption: "Setting",
        content: {
            selector: "#setting-content"
        },
        animation: {
            startup: {
                delay: 0,
                duration: 0
            }
        },
        button: {
            ok: {
                text: "Done",
                callback: function (e) {
                    processSetSetting(this.meta.content);
                    this.meta.shutdown();
                    window.location.reload();
                }
            }
        },
        icon: "fas fa-cogs"

    });
    pos_setting.startup("after", function (dialog) {

        initDataSetting(dialog.content);
    });
}

$("#open-shift").click(function (e) {
    confirmOpenShift();
});
function confirmOpenShift() {

    let user_privillege = db.table("tb_user_privillege").get('P012');
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
                        let access = accessSecurity(this.meta.content, 'P012');
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
                                    initFormOpenShift();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormOpenShift();
    }
}
function initFormOpenShift() {
    let open_shift = new DialogBox(
        {
            caption: "Cash In",
            content: {
                selector: "#open-shift-content"
            },
            position: "top-center",
            animation: {
                startup: {
                    delay: 0,
                    duration: 0
                }
            },
            type: "ok-cancel",
            icon: "far fa-money-bill-alt",
            button: {
                ok: {
                    text: "Done",
                    callback: function (e) {
                        this.meta.shutdown();
                    }
                }
            }
        }
    );
    let currencies = [];
    open_shift.startup("before", function (dialog) {

        $.each(db.from("tb_exchange_rate"), function (i, exchange) {
            let curr = {};
            curr.id = exchange.CurrencyID;
            curr.index = i + 1 + ". ";
            curr.cash = 'Cash';
            curr.input_cash = "";
            curr.currency = exchange.Currency.Description;
            curr.ratein = exchange.Rate;
            curr.total = 0;
            currencies.push(curr);
        });
        $.bindRows(".open-cash-in", currencies, "id", {
            html: [
                {
                    column: "input_cash",
                    insertion: "replace",
                    element: "<input autofocus class='input-cash'>",
                    listener: ["keyup", function (e) {
                        this.value = _$_.validNumber(this.value);
                        if (this.value.length === 0) {
                            this.value = 0;
                        }
                        let id = parseFloat($(this).parent().parent().data("id"));
                        let cash = this.value;
                        let curr = currencies.find(w => w.id === id);
                        let sys_currency = currencies.find(w => w.id === setting.SysCurrencyID);
                        curr.total = curr.ratein * cash;
                        total_cash = sumTotal(currencies);
                        $(".cash-in-total").text(sys_currency.currency + " " + total_cash);
                    }],
                }
            ],
            show_key: false,
            hidden_columns: ["id", "ratein", "total"]
        });
        $("input.input-cash").val("0");

    });
    open_shift.confirm(function (e) {
        processOpenShift(sumTotal(currencies));
        OpenShift();
        this.meta.shutdown();
        $(".cash-in-total").text("0.000");
    });
    open_shift.reject(function (e) {
        this.meta.shutdown();
        $(".cash-in-total").text("0.000");
    });
}

$("#close-shift").click(function (e) {
    confirmCloseShift();
});
function confirmCloseShift() {
    let user_privillege = db.table("tb_user_privillege").get('P013');
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
                        let access = accessSecurity(this.meta.content, 'P013');
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
                                    initFormCloseShift();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormCloseShift();
    }
}
function initFormCloseShift() {
    let close_shift = new DialogBox(
        {
            caption: "Cash Out",
            content: {
                selector: "#close-shift-content"
            },
            position: "top-center",
            animation: {
                startup: {
                    delay: 0,
                    duration: 0
                }
            },
            type: "ok-cancel",
            icon: "far fa-money-bill-alt",
            button: {
                ok: {
                    text: "Done",
                    callback: function (e) {
                        this.meta.shutdown();
                    }
                }
            }
        }
    );
    let currencies = [];
    close_shift.startup("before", function (dialog) {
        $(".close-cash-out").empty();
        $.each(db.from("tb_exchange_rate"), function (i, exchange) {
            let curr = {};
            curr.id = exchange.CurrencyID;
            curr.index = i + 1 + ". ";
            curr.cash = 'Cash';
            curr.input_cash = "";
            curr.currency = exchange.Currency.Description;
            curr.ratein = exchange.Rate;
            curr.total = 0;
            currencies.push(curr);
        });
        $.bindRows(".close-cash-out", currencies, "id", {
            html: [
                {
                    column: "input_cash",
                    insertion: "replace",
                    element: "<input autofocus class='input-cash'>",
                    listener: ["keyup", function (e) {
                        this.value = _$_.validNumber(this.value);
                        if (this.value.length === 0) {
                            this.value = 0;
                        }
                        let id = parseFloat($(this).parent().parent().data("id"));
                        let cash = this.value;
                        let curr = currencies.find(w => w.id === id);
                        let sys_currency = currencies.find(w => w.id === setting.SysCurrencyID);
                        curr.total = curr.ratein * cash;
                        total_cash = sumTotal(currencies);
                        $(".cash-out-total").text(sys_currency.currency + " " + total_cash);
                    }]

                }
            ],
            show_key: false,
            hidden_columns: ["id", "ratein", "total"]
        });
        $("input.input-cash").val("0");

    });
    close_shift.confirm(function (e) {
        processCloseShift(sumTotal(currencies));
        db.map("tb_check_open_shift").clear();
        window.location.href = "/Account/Login";
        this.meta.shutdown();
        $(".cash-out-total").text("0.000");

    });
    close_shift.reject(function (e) {
        this.meta.shutdown();
        $(".cash-out-total").text("0.000");

    });
}
function sumTotal(array) {
    for (
        var
        index = 0,              // The iterator
        length = array.length,  // Cache the array length
        sum = 0;                // The total amount
        index < length;         // The "for"-loop condition
        sum += array[index++].total  // Add number on each iteration
    );
    return sum;
}
function initDataSetting(content) {
   
    content.find('.print-receipt-order').prop('checked', setting.PrintReceiptOrder);
    content.find('.print-receipt-pay').prop('checked', setting.PrintReceiptTender);
    content.find('.print-receipt-count').val(setting.PrintCountReceipt);
    content.find('.max-queue-count').val(setting.QueueCount);
    content.find(".receipt-size").val(setting.Receiptsize);

    content.find(".receipt-template").val(setting.ReceiptTemplate);
    content.find(".printer-id").val(setting.Printer);
    content.find('.dual-screen').prop('checked', setting.DaulScreen);
    content.find('.vat-able').prop('checked', setting.VatAble);
    content.find('.vat-invoice-no').val(setting.VatNum);
    content.find('.password-wifi').val(setting.Wifi);
    content.find('.customer-id').val(setting.CustomerID);
    content.find('.price-list-id').val(setting.PriceListID);
    content.find('.receipt-other-curr').val(setting.CurrencyDisplay);
    content.find('.other-curr-rate').val(setting.DisplayRate);
    content.find('.warehouse-id').val(setting.WarehouseID);
    content.find('.payment-means-id').val(setting.PaymentMeansID);
}
function processSetSetting(content) {
    setting.PrintReceiptOrder = content.find(".print-receipt-order").is(":checked");
    setting.PrintReceiptTender = content.find(".print-receipt-pay").is(":checked");
    setting.PrintCountReceipt = parseInt(content.find('.print-receipt-count').val());
    setting.QueueCount = parseInt(content.find('.max-queue-count').val());
    setting.Receiptsize = content.find(".receipt-size option:selected").text();
    setting.ReceiptTemplate = content.find('.receipt-template option:selected').text();
    setting.Printer = parseInt(content.find(".printer-id").val());
    setting.DaulScreen = content.find('.dual-screen').is(':checked');
    setting.VatAble = content.find('.vat-able').is(':checked');
    setting.VatNum = content.find('.vat-invoice-no').val();
    setting.Wifi = content.find('.password-wifi').val();
    setting.CustomerID = parseInt(content.find('.customer-id').val());
    setting.PriceListID = parseInt(content.find('.price-list-id').val());
    setting.CurrencyDisplay = content.find('.receipt-other-curr').text();
    setting.DisplayRate = parseFloat(content.find('.other-curr-rate').val());
    setting.WarehouseID = parseInt(content.find('.warehouse-id').val());
    setting.PaymentMeansID = parseInt(content.find('.payment-means-id').val());
 
    $.ajax({
        url: '/POS/UpdateSetting',
        type: 'POST',
        data: { setting: setting },
        dataType: 'JSON',
        success: function (response) {
            
        }

    });

}
function initDataReceipt(receipts) {
  
    db.map("tb_reprint").clear();
    $.each(receipts, function (i, receipt) {
        let rec = {};
        rec.orderid = receipt.ReceiptID;
        rec.receipt_no = receipt.ReceiptNo;
        rec.cashier = receipt.UserAccount.Employee.Name;
        rec.dateout = receipt.DateOut.slice(0, 10);
        rec.timeout = receipt.TimeOut;
        //rec.table = receipt.Table.Name;
        if (local_currency.symbol === 'KHR' || local_currency.symbol === '៛') {
            rec.amount = numeral(receipt.GrandTotal).format('0.000');
        }
        else {
            rec.amount = receipt.GrandTotal.toFixed(2);
        }

        rec.currency = receipt.Currency.Description;
        db.insert("tb_reprint", rec, "orderid");
    });

    $.bindRows(".reprint-receipt-listview", db.from("tb_reprint"), "orderid", {

        text_align: [{ "cashier": "left" }],
        html: [
            {
                column: -1,
                insertion: "replace",
                element: '<div class=btn><i class="fa fa-print"></i></div>',
                listener: ["click", function (e) {
                    rowClickedPrint(this);
                }]
            }
        ],
        show_key: false,
        hidden_columns: ["orderid"]

    });
}
//Reprint
//print
function rowClickedPrint(row) {
    let orderid = parseInt($(row).parent().parent().data("orderid"));
    $.ajax({
        url: "/POS/PrintReceiptReprint",
        async: false,
        type: "POST",
        data: { orderid: orderid, print_type: "Reprint" },
        success: function () {

        }
    });
}

//filter date
let date_to = null;
let date_from = null;
$('.reprint-date-to').change(function () {
    date_to = $(this).val();
    let receipts = $.ajax({
        url: '/POS/GetReceiptReprint',
        async: false,
        type:'GET',
        data: { branchid: order_info.branchid, date_from: date_from, date_to: date_to },
        success: function () {

        }

    }).responseJSON;
    initDataReceipt(receipts);
});

$('.reprint-date-from').change(function () {
    date_from = $(this).val();
});
//search reprint
$('input[name="search-receipt-reprint"]').keyup(function () {
    let query = $(this).val().toLowerCase();
    let receipts = db.from("tb_reprint").where(w => {
        return w.receipt_no.toLowerCase().includes(query) || w.cashier.toLowerCase().includes(query);
    });
    $.bindRows(".reprint-receipt-listview", receipts, "orderid", {

        text_align: [{ "cashier": "left" }],
        html: [
            {
                column: -1,
                insertion: "replace",
                element: '<div class=btn><i class="fa fa-print"></i></div>',
                listener: ["click", function (e) {
                    rowClickedPrint(this);
                }]
            }
        ],
        show_key: false,
        hidden_columns: ["orderid"]

    });
});

//End
function processOpenShift(total_cash_in) {
 
    $.ajax({
        url: '/POS/OpenShift',
        type: 'POST',
        async: false,
        data: { userid: order_info.user, cash: total_cash_in },
        success: function (response) {
            
        }
    });
}
function processCloseShift(total_cash_out) {
    let loader = null;
    loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
    $.ajax({
        url: '/POS/CloseShift',
        type: 'POST',
        async: false,
        data: { userid: order_info.user, cashout: total_cash_out },
        success: function (response) {
            //window.open("/DevPrint/PrintCloseShiftDetail?TranF=" + response.Trans_From + "&TranT=" + response.Trans_To + "&UserID=" + response.UserID + "&Type=" + "Admin" + "", "_blank");
            loader.close();
        }
    });
}