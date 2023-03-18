//Click button bill

$("#bill").click(function () {
    //Bill
    
    let user_privillege = db.table("tb_user_privillege").get('P007');
    if (user_privillege.Used === false) {
        let dlg= new DialogBox({
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
                        let access = accessSecurity(this.meta.content, 'P007');
                        if (access === false) {
                            this.meta.content.find('.error-security-login').text('You can not access ...!');
                            return;
                        }
                        else {
                            this.meta.content.find('.security-username').focus();
                            this.meta.setting.icon = "fas fa-lock fa-spin";
                            this.meta.content.find('.error-security-login').text('');
                            this.text = "Logging...";
                            this.meta.build(this.setting);
                            setTimeout(() => {
                                this.meta.build(this.setting);
                                this.meta.setting.icon = "fas fa-unlock-alt";
                                setTimeout(() => {
                                    confirmBill(this.meta);
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }
           
        });
    }
    else {
        confirmBill();
    }
});

function confirmBill(dlg_main) {

    if (db.from("tb_check_open_shift") === 0) {
        let msg = new DialogBox(
            {
                caption: "Information",
                content: "Please open shift before bill...!",
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
    if (db.from("tb_order_detail") !== 0) {
        
        if (order_master.OrderID === 0) {
            if (setting.AutoQueue == false) {
                setOrderNo('Bill');
            }
            else {
                $("#send").text("Sending ...");
                sendData(0, 'Bill');
            }
        }
        else {
            if (dlg_main != undefined) {
                dlg_main.shutdown();
            }
            $("#send").text("Updating ...");         
            sendData(order_master.OrderID, 'Bill');
        }
    }
    else {
        let msg = new DialogBox(
            {
                caption: "Information",
                content: "Data was empty...!",
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