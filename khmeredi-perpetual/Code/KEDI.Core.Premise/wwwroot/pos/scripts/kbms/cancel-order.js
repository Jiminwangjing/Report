$("#dbl-cancel-receipt").click(function (e) {
    confirmCancelReceipt();
});
function confirmCancelReceipt() {
    let user_privillege = db.table("tb_user_privillege").get('P020');//not asign
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
                        let access = accessSecurity(this.meta.content, 'P020');
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
                                  
                                    initFormSaleReceipt();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormSaleReceipt();
    }
}
function initFormSaleReceipt() {
    let reprint = new DialogBox({
        position: "top-center",
        caption: "Cancel Receipt",
        content: {
            selector: "#cancel-receipt-content"
        },
        animation: {
            startup: {
                delay: 0,
                duration: 0
            }
        },
        button: {
            ok: {
                text: "Close",
                callback: function (e) {
                    this.meta.shutdown();
                }
            }
        },
        icon: "fas fa-print"
    });
    reprint.startup("before", function (dialog) {
        let receipts = $.ajax({
            url: '/POS/GetReceiptCancel',
            async: false,
            type: 'GET',
            data: { branchid: order_info.branchid, date_from: null, date_to: null },
            success: function () {
            }

        }).responseJSON;
        initDataCancelReceipt(receipts);
    });
}
function initDataCancelReceipt(receipts) {

    db.map("tb_cancel_receipt").clear();
    $.each(receipts, function (i, receipt) {
        let rec = {};
        rec.orderid = receipt.ReceiptID;
        rec.receipt_no = receipt.ReceiptNo;
        rec.cashier = receipt.UserAccount.Employee.Name;
        rec.dateout = receipt.DateOut.slice(0, 10);
        rec.timeout = receipt.TimeOut;
        //rec.table = receipt.Table.Name;

        if (local_currency.symbol === 'KHR' || local_currency.symbol === '៛') {
            rec.amount = numeral(receipt.GrandTotal).format('0,0');
        }
        else {
            rec.amount = receipt.GrandTotal.toFixed(2);
        }
        rec.currency = receipt.Currency.Description;
        if (receipt.GrandTotal < 0) {
            rec.action = '<div class="btn action disabled" style="margin-right:10px; color:red;"><i class="fas fa-trash-restore"></i></div>'
        }
        else {
            rec.action = '<div class="btn action" style="margin-right:10px;"><i class="fas fa-trash-restore"></i></div>'
        }
        db.insert("tb_cancel_receipt", rec, "orderid");
    });

    $.bindRows(".cancel-receipt-listview", db.from("tb_cancel_receipt"), "orderid", {

        text_align: [{ "cashier": "left" }],
        
        show_key: false,
        hidden_columns: ["orderid"]

    });
    $(".action", ".cancel-receipt-listview").click(function () {
        
        rowClickedCancel(this);
    });
}

function rowClickedCancel(row) {
    let orderid = parseInt($(row).parent().parent().data("orderid"));
    $(row).parent().parent().remove();
    $.ajax({
        url: "/POS/CancelReceipt",
        async: false,
        type: "POST",
        data: { orderid: orderid},
        success: function () {

        }
    });
}
//filter date
let date_to_cancel = null;
let date_from_cancel = null;
$('.cancel-date-from').change(function () {
    date_from_cancel = $(this).val();
});
$('.cancel-date-to').change(function () {
    date_to_cancel = $(this).val();
    let receipts = $.ajax({
        url: '/POS/GetReceiptCancel',
        async: false,
        type: 'GET',
        data: { branchid: order_info.branchid, date_from: date_from_cancel, date_to: date_to_cancel },
        success: function () {

        }

    }).responseJSON;
    initDataCancelReceipt(receipts);
});

$('#txt-item-cancel-search').keyup(function () {
    var value = $(this).val().toLowerCase();
    $(".cancel-receipt-listview tr:not(first-child)").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
    });
})