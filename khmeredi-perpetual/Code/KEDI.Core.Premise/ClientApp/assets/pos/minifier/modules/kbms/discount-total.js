$('#btn-dis-total').click(function () {
    confirmDiscountTotal();
});

function confirmDiscountTotal() {
    let user_privillege = db.table("tb_user_privillege").get('P022');
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
                        let access = accessSecurity(this.meta.content, 'P006');
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
                                    initFormDiscountTotal();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormDiscountTotal();
    }
};

function initFormDiscountTotal() {
    $('.discount-total-erorr').text('');
    let dlg = new DialogBox({
        position: "top-center",
        content: {
            selector: "#content-dis-total",
            class: 'login'
        },
        icon: "fas fa-percent fa-fw",
        button: {
            ok: {
                text: "OK",
                callback: function (e) {
                    var discount = parseFloat(dlg.content.find('.discount-total').val());
                    var type = dlg.content.find('.dis-total-type').val();
                    if (isNaN(discount)) {
                        dlg.content.find('.discount-total').val('');
                        dlg.content.find('.discount-total').focus();
                        $('.discount-total-erorr').text("Please input discount...!");
                    }
                    else {
                        if (type == 'Percent') {
                            if (discount <= 100) {
                                order_master.DiscountRate = discount;
                                order_master.TypeDis = type;
                                summaryTotal(0, 'Y');
                                this.meta.shutdown();
                            }
                            else {
                                dlg.content.find('.discount-total').val('');
                                dlg.content.find('.discount-total').focus();
                                $('.discount-total-erorr').text("Discount can not bigger than 100%");
                            }
                        }
                        else {
                         
                            if (discount <= order_master.GrandTotal) {
                                order_master.DiscountRate = discount;
                                order_master.TypeDis = type;
                                summaryTotal(0, 'Y');
                                this.meta.shutdown();
                            }
                            else {
                                dlg.content.find('.discount-total').val('');
                                dlg.content.find('.discount-total').focus();
                                $('.discount-total-erorr').text("Discount can not bigger than pay");
                            }
                        }
                        
                    }
                   
                }
            }
        }
    });
    dlg.startup("after", function (dialog) {
        dialog.content.find('.discount-total').val('');
        dialog.content.find('.discount-total').focus();

    });
};