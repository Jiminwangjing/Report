$("#dbl-return-item").click(function (e) {
    confirmReturnReceipt();
});
function confirmReturnReceipt() {
    let user_privillege = db.table("tb_user_privillege").get('P021');//not asign
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
                        let access = accessSecurity(this.meta.content, 'P021');
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

                                    initFromReturnReceipt();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFromReturnReceipt();
    }
};
let dlg_return= null;
function initFromReturnReceipt() {
    dlg_return = new DialogBox({
        position: "top-center",
        caption: "Return Item",
        type:'ok-cancel',
        content: {
            selector: "#item-return-order"
        },
        button: {
            ok: {
                text: "Apply",
                callback: function (e) {
                    if (db.from('tb_item_return') != 0) {
                        processReutunItem();
                    }
                    else {
                        $('.error-return-item').text('Error : Can not return, because data empty...!');
                      
                    }
                }
            }
        },
        icon: "fas fa-receipt"
    });
    dlg_return.reject(function (e) {
        this.meta.shutdown();
    })
    dlg_return.startup("before", function (dialog) {
        $.ajax({
            url: '/POS/GetReceiptReturn',
            type: 'GET',
            dataType:'JSON',
            data: { branchid: order_info.branchid, date_from: null, date_to: null },
            success: function (receipts) {
                initDataReturnReceipt(receipts);
            }
        });
        
    }); 
};

function initDataReturnReceipt(receipts) {
    let list = dlg_return.content.find('.return-invoice-listview');
    $.bindRows(list, receipts, "ReceiptID", {
        show_key: false,
        hidden_columns: ["ReceiptID"],
        postbuild: function () {           
            $(this).on("click", function () {
                rowClickedReturn(this);
            });
            //$(this).find("td:first-child").before("<td style:'width: 10px;'>"+ index++ +"</td>");
        }
    });
};

function rowClickedReturn(row) {
    let receipt_id = $(row).data('receiptid');
    $.ajax({
        url: '/POS/GetReceiptReturnDetail',
        type: 'GET',
        dataType: 'JSON',
        data: { ReceiptID: receipt_id},
        success: function (detail) {
            initDataReturnReceiptDetail(detail, receipt_id);
        }
    });
};
function initDataReturnReceiptDetail(details, receipt_id) {
   
    let list = dlg_return.content.find('.item-return-choosed-listview');
    db.table('tb_item_return').clear();
    $.each(details, function (i, detail) {      
        let ReturnItem = {};
        ReturnItem.id = detail.ID;
        ReturnItem.receipt_id = receipt_id;
        ReturnItem.item_id = detail.ItemID;
        ReturnItem.code = detail.Code;
        if (detail.Name != 'manual') {
            ReturnItem.kh = detail.KhmerName + '(' + detail.Name + ')';
        }
        else {
            ReturnItem.kh = detail.KhmerName;
        }
       
        ReturnItem.openQty = detail.OpenQty;
        ReturnItem.returnQty = 0;
        db.insert('tb_item_return', ReturnItem, "id");
    })
    $.bindRows(list, db.from('tb_item_return'), "id", {
        show_key: false,
        hidden_columns: ["id", "receipt_id", "returnQty","item_id"],
        postbuild: function () {
            //$(this).on("click", rowClickedReturnRemove);
            //$(this).find("td:first-child").before("<td style:'width: 10px;'>" + index++ + "</td>");
            let input = $("<td></td>").append($("<input>").on("keyup", editReturnQty))
            let icon = $("<td></td>").append($("<i class='fas fa-trash trash csr-pointer'></i>").on("click", removeReturnQty));
            $(this).append(input).append(icon);
        }
    });
    
};

function editReturnQty() {
    let line_id = $(this).parent().parent().data('id');
    let item_return = db.table('tb_item_return').get(line_id);
    let qty = parseFloat($(this).val());
    item_return.returnQty = qty;
    db.insert("tb_item_return", item_return, "id");
};

function removeReturnQty() {
    let line_id = $(this).parent().parent().data('id');
    $(this).parent().parent().remove();
    db.table("tb_item_return").delete(line_id);
};

function processReutunItem() {
    let loader = null;
    loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
    $.ajax({
        url: '/POS/SendReturnItem',
        type: 'POST',
        dataType: "JSON",
        data: {
            returnItems: db.from('tb_item_return')
        },
        success: function (response) {
            if (response.status == false) {
                $('.error-return-item').text('Error : Can not return, because one or more item qty bigger than original qty...!');               
            }
            else {
                
                db.table('tb_item_return').clear();
                let list = dlg_return.content.find('.item-return-choosed-listview');
                $.bindRows(list, db.from('tb_item_return'), "id", {
                });
                $('.error-return-item').text('Successfully...').removeClass('trash').css("color", "green");
                setTimeout(() => {
                    $('.error-return-item').text('').css("color","red");
                }, 1000);

            }
            loader.close();
        }
        
    });
};

$('#txt-item-return-search').keyup(function () {
    var value = $(this).val().toLowerCase();
    $(".return-invoice-listview").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
    });
});

//filter date
let date_to_return = null;
let date_from_return = null;
$('.return-date-from').change(function () {
    date_from_return = $(this).val();
});
$('.return-date-to').change(function () {
    date_to_return = $(this).val();
    $.ajax({
        url: '/POS/GetReceiptReturn',
        type: 'GET',
        dataType: 'JSON',
        data: { branchid: order_info.branchid, date_from: date_from_return, date_to: date_to_return },
        success: function (receipts) {
            initDataReturnReceipt(receipts);
        }
    });
});