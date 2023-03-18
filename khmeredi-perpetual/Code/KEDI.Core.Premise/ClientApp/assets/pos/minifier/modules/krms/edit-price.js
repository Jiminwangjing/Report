$("#dbl-edit-price").click(function (e) {
    confirmEditPrice();
});

function confirmEditPrice() {
    let user_privillege = db.table("tb_user_privillege").get('P004');
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
                        let access = accessSecurity(this.meta.content, 'P004');
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
                                    initFormEditPrice();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormEditPrice();
    }
};

function initFormEditPrice() {
    if ($("#item-listview").find('tr:not(:first-child)').length !== 0) {
        db.map("tb_edit_item_price").clear();
        let msg = new DialogBox(
            {
                caption: "Edit Item Price",
                content: {
                    selector: "#edit-item-price"
                },
                position: "top-center",
                type: "ok-cancel"
            }
        );
        msg.setting.animation.shutdown.animation_type = "slide-up";
        msg.setting.button.ok.text = "Edit";
        msg.confirm(editItemPrice);
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
            item_add.price = item.UnitPrice;
            item_add.symbol = '->';
            db.insert("tb_edit_item_price", item_add, "line_id");
        });
        bindRowEditPrice();
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

function editItemPrice() {
    let items_edit = $("input[name='edit-price']");
    let items_name = $("input[name='name']", this.meta.content.find("table"));
    //Edit price
    $.each(items_edit, function (i, item) {
        let new_price = parseFloat($(item).val());        
        if (isNaN(new_price)) {
            //console.log(item);
        }
        else {
            let line_id = $(item).parent().parent().data("line_id");
            
            let items_ordered = db.get("tb_order_detail").get(line_id);
            items_ordered.UnitPrice = new_price;
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
    //Edit item name
    $.each(items_name, function (i, item) {        
        let new_name = $(item).val();    
        let line_id = $(item).parent().parent().data("line_id");
        let items_ordered = db.get("tb_order_detail").get(line_id);
        items_ordered.KhmerName = new_name;
        db.insert("tb_order_detail", items_ordered, "Line_ID");     
    });
    //console.log(db.get("tb_order_detail"));
    summaryTotal(0,'Y');
    this.meta.shutdown();
};

function bindRowEditPrice() {
    let edit_item_price = db.from("tb_edit_item_price");
    //console.log(edit_item_price);
    $.bindRows("#edit-item-price-listview", edit_item_price, "line_id", {
        text_align: [{ "name": "left" }],
        prebuild: function (data) {
            data.name = '<input name=name value="'+ data.name +'" type="text" />';
        },
        html: [
            
            {
                column: -1,
                insertion: "replace",
                element: ' <input name=edit-price type="text" />',
                listener: ["keyup", function (e) {
                    rowEditItemPrice(this);
                }]
            }
        ],
        hidden_columns:["line_id","new_price"]
    });
};

function rowEditItemPrice() {
   
};