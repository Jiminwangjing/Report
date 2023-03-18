$("#dbl-split-receipt").click(function (e) {
    confirmSplit();
});
function confirmSplit() {
    let user_privillege = db.table("tb_user_privillege").get('P003');
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
                        let access = accessSecurity(this.meta.content, 'P003');
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
                                   
                                    initFormSplit();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormSplit();
    }
}

function initFormSplit() {
    if ($("#item-listview").find('tr:not(:first-child)').length !== 0) {
        if (order_master.OrderID !== 0) {
            let msg = new DialogBox(
                {
                    caption: "Split Receipt ( " + order_master.OrderNo + " )",
                    content: {
                        selector: "#split-receipt"
                    },
                    position: "top-center",
                    type: "ok-cancel"
                }
            );
            msg.setting.animation.shutdown.animation_type = "slide-up";
            msg.setting.button.ok.text = "Split";
            msg.confirm(spitItemTo);
            msg.reject(function (e) {
                this.meta.shutdown();
            })
            db.map("tb_order_split").clear();
            $.each(db.from("tb_order_detail"), function (i, item) {
                item_split = {};
                item_split.Line_ID = item.Line_ID;
                item_split.code = item.Code;
                item_split.name = item.KhmerName;
                item_split.qty = parseFloat(item.Qty) - 1;
                item_split.uom = item.UoM;
                item_split.dis_qty = item.Qty;
                item_split.symbol = "->";
                item_split.split_qty = 1;

                db.insert("tb_order_split", item_split, "Line_ID");
            });
            bindRowSpilt();
        }
        else {
            let msg = new DialogBox(
                {
                    caption: "Information",
                    content: "Please send data before split...!",
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
function processSplit() {
    $.each(db.from("tb_order_split"), function (i, item_split) {
        let item_update = db.get("tb_order_detail").get(item_split.Line_ID);
        item_update.Qty = item_split.qty;
        item_update.PrintQty = item_split.split_qty;
        if (item_update.TypeDis === "Percent") {
            item_update.Total = ((item_update.Qty * item_update.UnitPrice) * (1 - (item_update.DiscountRate / 100)));
            item_update.DiscountValue = (item_update.Qty * item_update.UnitPrice) * item_update.DiscountRate / 100;
        }
        else {
            item_update.Total = ((item_update.Qty * item_update.UnitPrice) - item_update.DiscountRate);
            item_update.DiscountValue = istem_update.DiscountRate;
        }
        item_update.Total_Sys = item_update.Total * local_currency.ratein;
        db.insert("tb_order_detail", item_update, "Line_ID");
        
    });
    summaryTotal(0,'Y');
    sendDataSplit(order_master.OrderID);
}
function spitItemTo() {
    processSplit();
    this.meta.shutdown();
}
function sendDataSplit(orderid) {
    let loader = null;
    loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
    Order = {
        OrderID: orderid,
        OrderNo: order_master.OrderNo,
        TableID: table_info.id,
        ReceiptNo: order_master.ReceiptNo,
        QueueNo: order_master.QueueNo,
        DateIn: order_master.DateIn,
        DateOut: date.getDate(),
        TimeIn: order_master.TimeIn,
        TimeOut: date.toLocaleTimeString(),
        WaiterID: order_master.WaiterID,
        UserOrderID: order_master.UserOrderID,
        UserDiscountID: order_master.UserDiscountID,
        CustomerID: order_master.CustomerID,
        CustomerCount: order_master.CustomerCount,
        PriceListID: order_master.PriceListID,
        LocalCurrencyID: order_master.LocalCurrencyID,
        SysCurrencyID: order_master.SysCurrencyID,
        ExchangeRate: order_master.ExchangeRate,
        WarehouseID: order_master.WarehouseID,
        BranchID: order_master.BranchID,
        CompanyID: order_master.CompanyID,
        Sub_Total: order_master.Sub_Total,
        DiscountRate: order_master.DiscountRate,
        DiscountValue: order_master.DiscountValue,
        TypeDis: order_master.TypeDis,
        TaxRate: order_master.TaxRate,
        TaxValue: order_master.TaxValue,
        GrandTotal: order_master.GrandTotal,
        GrandTotal_Sys: order_master.GrandTotal_Sys,
        Received: order_master.Received,
        Change: order_master.Change,
        PaymentMeansID: order_master.PaymentMeansID,
        CheckBill: 'N',
        OrderDetail: db.from("tb_order_detail"),
        OrderDetail_Addon: db.from("tb_order_addon")
    };
    
    $.ajax({
        url: "/POS/SendSplit",
        type: "POST",
        dataType: "JSON",
        data: { data: Order,addnew:Order },
        success: function (response) {           
            clickOnOrderNo(response.TableID, response.OrderID);
            $.each(db.from("tb_order_detail"), function (i, item) {
                item_split = {};
                item_split.Line_ID = item.Line_ID;
                item_split.code = item.Code;
                item_split.name = item.KhmerName;
                item_split.qty = parseFloat(item.Qty) - 1;
                item_split.dis_qty = item.Qty;
                item_split.symbol = "->";
                item_split.split_qty = 1;
                db.insert("tb_order_split", item_split, "Line_ID");
            });
            bindRowSpilt();
            loader.close();
        }
    });
}

function bindRowSpilt() {
    let items_split = db.from("tb_order_split");  
    $.bindRows("#split-receipt-listview", items_split, 'Line_ID', {
        scalable: {
            column: "split_qty",
            event: "click",
            callback: rowSpiltClicked
        },
        text_align: [{ "name": "left" }],
        html: [
            {
                column: -1,
                insertion: "replace",
                element: '<i class="fa fa-trash trash"></i>',
                listener: ["click", function (e) {
                    rowSpiltDelete(this);
                }]
            }
        ],
        show_key: false,
        mouseover: row_hovered,
        hidden_columns: ["Line_ID","qty"]

    });
}

function rowSpiltDelete(row) {
    let $row = $(row).parent().parent();
    let line_id = $row.data("line_id");
    let item_old = db.table("tb_order_detail").get(line_id);
    let item_split = db.table("tb_order_split").get(line_id);
    item_split.qty = item_old.Qty;
    item_split.split_qty = 0;
    db.insert("tb_order_split", item_split, "Line_ID");
    $row.remove();
}

function rowSpiltClicked(e) {
    let $row = $(this).parent().parent();
    let line_id = $row.data("line_id");
    let scale = $(this).children(".scale");
    if ($(e.target).is($(".scale-up"))) {

        let item_split = db.get("tb_order_split").get(line_id);
        if (item_split.qty <= 0) {
            return;
        }
        item_split.split_qty = item_split.split_qty + 1;
        item_split.qty = item_split.qty - 1;
        
        db.insert("tb_order_split", item_split, "Line_ID");
        scale.text(item_split.split_qty);
        //$row[0].cells[6].innerHTML = item_split.amount.toFixed(2);
      
        bindRowSpilt();
    }
    if ($(e.target).is($(".scale-down"))) {
      
        let item_old = db.get("tb_order_detail").get(line_id);
        let item_split = db.get("tb_order_split").get(line_id);
        item_split.split_qty = item_split.split_qty - 1;
        item_split.qty = item_split.qty + 1;

        if (item_split.split_qty <= 0) {
           
            $(this).parent().parent().remove();
            item_split.Qty = item_old.Qty;
            item_split.split_qty = 0;
            db.update("tb_order_split", item_split, "Line_ID");
            return;
        }
      
        db.insert("tb_order_split", item_split, "Line_ID");
        scale.text(item_split.split_qty);
      
    }

}
