$("#dbl-discount-item").click(function (e) {
    confirmDiscountItem();
});

function confirmDiscountItem() {
    let user_privillege = db.table("tb_user_privillege").get('P019');
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
                        let access = accessSecurity(this.meta.content, 'P019');
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
                                    initFormDiscountItem();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormDiscountItem();
    }
};

function initFormDiscountItem() {
    if ($("#item-listview").find('tr:not(:first-child)').length !== 0) {
        db.map("tb_discount-item").clear();
        let msg = new DialogBox(
            {
                caption: "Discount Item",
                content: {
                    selector: "#discount-item"
                },
                position: "top-center",
                type: "ok-cancel"
            }
        );
        msg.setting.animation.shutdown.animation_type = "slide-up";
        msg.setting.button.ok.text = "Apply";
        msg.confirm(function () {
            discountItem(this.meta.content);
            this.meta.shutdown();
        });
        msg.reject(function (e) {
            this.meta.shutdown();
        })
        //Intail data to edit price

        $.each(db.from("tb_order_detail").where(w => { return w.Qty > 0; }), function (i, item) {
            let item_add = {};
            item_add.line_id = item.Line_ID;
            item_add.code = item.Code;
            item_add.name = item.KhmerName;
            item_add.uom = item.UoM;
            if (item.DiscountRate == 0) {
                item_add.discountRate = "<input width='100' name=discount-item value='" + '' + "' type='text'>";
            }
            else {
                item_add.discountRate = "<input name=discount-item value='" + item.DiscountRate + "' type='text'>";
            }
            db.insert("tb_discount-item", item_add, "line_id");
        });
        bindRowDiscountItem();
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
};

let typedis='Percent';
$('.discount-item-type').change(function () {
    typedis = $(this).val();
});
function discountItem(content) {
    let items_discount = content.find("input[name='discount-item']");
  
    $.each(items_discount, function (i, item) {
        let discount = parseFloat($(item).val());
       
        if (isNaN(discount)) {
            //console.log(new_price);
        }
        else {
            let line_id = $(item).parent().parent().data("line_id");
            
            let items_ordered = db.get("tb_order_detail").get(line_id);
            items_ordered.DiscountRate = discount;
            items_ordered.TypeDis = typedis;
            if (items_ordered.TypeDis === 'Percent') {
                items_ordered.Total = ((items_ordered.Qty * items_ordered.UnitPrice) * (1 - (items_ordered.DiscountRate / 100)));
                items_ordered.DiscountValue = ((items_ordered.Qty * items_ordered.UnitPrice) * items_ordered.DiscountRate) / 100;
             
            }
            else {
                items_ordered.Total = ((items_ordered.Qty * items_ordered.UnitPrice) - items_ordered.DiscountRate);
                items_ordered.DiscountValue = items_ordered.DiscountRate;
            }
            items_ordered.Total_Sys = items_ordered.Total * local_currency.ratein;
            db.insert("tb_order_detail", items_ordered, "Line_ID");
        }
    });
    summaryTotal(0,'Y');
    
};

function bindRowDiscountItem() {
    let discount_item = db.from("tb_discount-item");
    $.bindRows("#discount-item-listview", discount_item, "line_id", {
        text_align: [{ "name": "left" }],
        hidden_columns:["line_id"]
    });
};
