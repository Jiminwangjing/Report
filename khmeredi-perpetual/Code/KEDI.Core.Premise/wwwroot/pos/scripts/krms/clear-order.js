$("#dbl-clear-order").click(function (e) {
    clearOrder();
});
function clearOrder() {
    parentLevel = "";
    newdefaultOrder();
    defaultGroup1();
    order_master.OrderID = 0;
    db.map("tb_order_detail").clear();
    db.map("tb_show_item_barcode").clear();
    //Customer
    let customer = db.get('tb_customer').get(order_master.CustomerID);
    $('.customer-name').text(customer.Name);
    if (db.from('tb_show_item') != 0) {
        db.from("tb_show_item").where(function (json) {
            json.PrintQty = 0;
        });
    }
    $.bindRows("#item-listview", db.from("tb_order_detail"), 'Line_ID', {});
    summaryTotal(0, 'Y');

}
