
$("#dbl-combine-receipt").click(function (e) {
    comfirmCombine();
});
function comfirmCombine() {
    let user_privillege = db.table("tb_user_privillege").get('P002');
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
                        let access = accessSecurity(this.meta.content, 'P002');
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
                                    initFormCombine();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormCombine();
    }
}
function initFormCombine() {
    if ($("#item-listview").find('tr:not(:first-child)').length !== 0) {
        if (order_master.OrderID !== 0) {
            let out = "";
            let orders = $.ajax({
                url: '/POS/GetReceiptCombine',
                type: 'GET',
                async: false,
                data: { orderid: order_master.OrderID }
            }).responseJSON;

            $.each(orders, function (i, order) {
          
                let table_name = db.table("tb_table").get(order.TableID);
                out += "<label class='rc-container mx1'>"
                    + "<input data-id=" + order.OrderID + " name='receipt' type='checkbox'>"
                    + "<span class='checkmark'></span>"
                    + table_name.Name + " #" + order.OrderNo
                    + "</label>"
                    + "<hr/>";
            });
            $('.combine-receipt-list').html(out);

            let msg = new DialogBox(
                {
                    caption: "Combine Receipt ( " + order_master.OrderNo + " )",
                    content: {
                        selector: "#combine-receipt"
                    },
                    position: "top-center",
                    type: "ok-cancel"
                }
            );
            msg.setting.animation.shutdown.animation_type = "slide-up";
            msg.setting.button.ok.text = "Combine";
            msg.confirm(combineReceipt);
            msg.reject(function (e) {
                this.meta.shutdown();
            })
        }
        else {
            let msg = new DialogBox(
                {
                    caption: "Information",
                    content: "Please send data before combine...!",
                    position: "top-center",
                    type: "ok",
                    icon: "info"
                }
            );
            msg.setting.button.ok.callback = function (e) {
                this.meta.shutdown();
            };
        }
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
        };
    }
}
$('input[name=search-combine-receipt]').on('keyup', function () {
    //getTables(-1);
    var query = $(this).val().toLowerCase();
    $('.combine-receipt-list .rc-container').each(function () {
        var $this = $(this);
        if ($this.text().toLowerCase().indexOf(query) === -1)
            $this.closest('.rc-container').hide();
        else $this.closest('.rc-container').show();
    });

});

function combineReceipt() {
    let loader = null;
    loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
    db.map("tb_combine_receipt").clear();
    let receipts = $("input[name='receipt']");
    $.each(receipts, function (i, receipt) {
        if ($(receipt).prop("checked")) {
            rec = {};
            rec.OrderID = $(receipt).data("id");
            db.insert("tb_combine_receipt", rec, 'OrderID');

        }
    });
    let CombineReceipt = {
        TableId: table_info.id,
        OrderID: order_master.OrderID,
        Receipts: db.from("tb_combine_receipt")
    };
    this.meta.shutdown();
    $.ajax({
        url: '/POS/CombineReceipt',
        type: 'POST',
        data: { combineReceipt: CombineReceipt },
        success: function (response) {
           
            loader.close();
            clickOnOrderNo(table_info.id, order_master.OrderID);
           
        }

    });
    
}