$("#dbl-void-order").click(function (e) {
    //clearOrder();
    confirmVoidOrder();
});
function confirmVoidOrder() {
    let user_privillege = db.table("tb_user_privillege").get('P005');
    if (user_privillege.Used === false) {
        let dlg = new DialogBox({      
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
                        let access = accessSecurity(this.meta.content, 'P005');
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
                                    initFormVoidOrder(dlg);
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {
        initFormVoidOrder();
    }
};

function initFormVoidOrder(dlg) {
    let msg = new DialogBox(
        {
            caption: "Void Order",
            content:"Do you want to void this order ?",
            position: "top-center",
            type: "ok-cancel"
        }
    );
    msg.setting.animation.shutdown.animation_type = "slide-up";
    msg.setting.button.ok.text = "Done";
    msg.confirm(function (e) {
        if (dlg != undefined) {
            dlg.shutdown();
        }
        processVoidOrder();
    });
    msg.reject(function (e) {
        this.meta.shutdown();
    });
};

function processVoidOrder() {
    $.ajax({
        url: '/POS/VoidOrder',
        type: 'POST',
        data: { orderid: order_master.OrderID },
        success: function (status) {       
            if (status == 'N') {
               
                let msg1 = new DialogBox(
                    {
                        caption: "Cancel Receipt",
                        content: "Please get authorization from administrator to cancel...!",
                        position: "top-center",
                        type: "ok"
                    }
                );
            }
            else {
                //clearNewBarcode();
                clearOrder();
                this.meta.shutdown();
            }

        }
    });  
};