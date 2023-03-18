function returnItemsNotStock(items) {
   
    let msg = new DialogBox(
        {
            caption: "Item not enough in stock",
            content: {
                selector: "#items-return"
            },
            position: "top-center",
            type: "ok"
        }
    );
    msg.setting.animation.shutdown.animation_type = "slide-up";
    msg.setting.button.ok.callback = function (e) {
        this.meta.shutdown();
    };
    $.bindRows("#items-return-listview", items, 'Line_ID', {
        //scalable: {
        //    column: "OrderQty",
        //    event: "click",
        //    callback: rowReturnScaleClicked
        //},
        text_align: [{ "KhmerName": "left" }, { "In Stock": "center" },{"Ordered Qty":"center"}],
        show_key: false,
        hidden_columns:["Line_ID","ItemID"]
    });
   
};

function rowReturnScaleClicked(e) {

    let item = {};
    let $row = $(this).parent().parent();
    let line_id = $row.data("line_id");
    item_master = db.get("tb_item_master").get(line_id);
    let scale = $(this).children(".scale");

    if ($(e.target).is($(".scale-up"))) {
        item = db.get("tb_order_detail").get(line_id);
        item.Qty = item.Qty + 1;
        item.PrintQty = item.PrintQty + 1;
        item_master.PrintQty = item.PrintQty;

        if (item.TypeDis === "Percent") {
            item.Total = ((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)));
            item.DiscountValue = (item.Qty * item.UnitPrice) * item.DiscountRate / 100;

        }
        else {
            item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate);
            item.DiscountValue = item.DiscountRate;
        }
        item.Total_Sys = item.Total * local_currency.ratein;
        //$(this).parent().parent()[0].cells[13].innerHTML = item.Total;
    }
    if ($(e.target).is($(".scale-down"))) {
        item = db.get("tb_order_detail").get(line_id);
        item.Qty = item.Qty - 1;
        item.PrintQty = item.PrintQty - 1;
        item_master.PrintQty = item.PrintQty;

        if (item.TypeDis === "Percent") {
            item.Total = ((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100))).toFixed(2);
            item.DiscountValue = (item.Qty * item.UnitPrice) * item.DiscountRate / 100;

        }
        else {
            item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate).toFixed(2);
            item.DiscountValue = item.DiscountRate;
        }
        item.Total_Sys = item.Total * local_currency.ratein;

        if (item.PrintQty <= 0) {
            $row.remove();
            db.insert("tb_order_detail", item, "Line_ID");
            db.table("tb_order_detail").delete(line_id);
            db.insert("tb_item_master", item_master, "ID");
            scale.data("scale", item.PrintQty);
            scale.text(item.PrintQty);
            updateSclaeGrids($row, item);
            summaryTotal(0, 'Y');
            return;
        }

    }

    db.insert("tb_order_detail", item, "Line_ID");
    db.insert("tb_item_master", item_master, "ID");
    scale.data("scale", item.PrintQty);
    scale.text(item.PrintQty);
    updateSclaeGrids($row, item);
    summaryTotal(0,'Y');
};

function rowItemReturnDelete() {

};
