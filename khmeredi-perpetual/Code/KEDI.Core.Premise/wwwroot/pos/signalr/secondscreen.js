//"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/secondscreen").build();
//Get order by table
const $wrap_grid = $("#group-item-gridview .wrap-grid");

connection.on("PushDataToSecondScreen", function (orders) {
    
    let summary = {};
    summary.sub_total = 0;
    summary.discount = 0;
    summary.discount_value = 0;
    summary.discount_symbol = '%';
    summary.vat = 0;
    summary.total = 0;
    db.table("tb_second_screen").clear();
    $("#Table").text(orders.orderNo);
    $.each(orders.orderDetail, function (i, detail) {
        let item = {};

        let item_master = db.get("tb_item_master").get(detail.line_ID);
        item.Line_ID = item_master.ID;
        item.Code = item_master.Code;
        item.KhmerName = detail.khmerName;
        if (local_currency.symbol === 'KHR' || local_currency.symbol === '៛') {
            item.UnitPrice = parseFloat(detail.unitPrice);
        }
        else {
            item.UnitPrice = parseFloat(detail.unitPrice).toFixed(2);
        }

        item.Qty = parseFloat(detail.qty);
        if (local_currency.symbol === 'KHR' || local_currency.symbol === '៛') {
            item.Total = detail.total;
        }
        else {
            item.Total = detail.total.toFixed(2);
        }
        
        item.Price = item_master.UnitPrice;
        item.Image = item_master.Image;
        item.EnglishName = detail.englishName;
        item.DiscountRate = detail.discountRate;
        if (item.Qty > 0) {
            db.insert("tb_second_screen", item, "Line_ID");
        }
        
    });
  
    $.bindRows("#item-listview", db.from("tb_second_screen"), 'Line_ID', {
        highlight_row: orders.tableID,
        text_align: [{ "KhmerName": "left" }],
        hidden_columns: ["Line_ID", "Price", "EnglishName", "Image","DiscountRate"]
    });
    
    $.each(db.from("tb_second_screen"), function (i, item) {
 
        summary.sub_total += parseFloat(item.Total);
    });
    
    summary.discount = orders.discountRate;
    if (orders.typeDis === "Percent") {
        summary.discount_value = summary.sub_total * summary.discount / 100;
        summary.discount_symbol = '%';
    }
    else {
        summary.discount_value = summary.discount;
        summary.discount_symbol = local_currency.symbol;
    }
    let vat = (orders.taxRate + 100) / 100;
    let rate = orders.taxRate / 100;

    summary.vat = (summary.sub_total) / vat * rate;
    summary.total = summary.sub_total - summary.discount_value;

    if (isNaN(summary.total)) {
        summary.sub_total = 0;
        summary.discount = 0;
        summary.discount_value = 0;
        summary.discount_symbol = '%';
        summary.vat = 0;
        summary.total = 0;
      
    }
    if (local_currency.symbol === 'KHR' || local_currency.symbol=== '៛'){
        $("#summary-sub-total").text(local_currency.symbol + " " + numeral(summary.sub_total).format('0,0'));
        $("#summary-bonus").text(summary.discount + " " + summary.discount_symbol);
        $("#summary-vat").text(local_currency.symbol + " " + numeral(summary.vat).format('0,0'));
        $("#summary-total").text(local_currency.symbol + " " + numeral(summary.total).format('0,0'));
    }
    else
    {
        $("#summary-sub-total").text(local_currency.symbol + " " + summary.sub_total.toFixed(2));
        $("#summary-bonus").text(summary.discount + " " + summary.discount_symbol);
        $("#summary-vat").text(local_currency.symbol + " " + summary.vat.toFixed(2));
        $("#summary-total").text(local_currency.symbol + " " + summary.total.toFixed(2));
    }
  
    singleItem($wrap_grid, db.from("tb_second_screen"));
    
});

//Initail connection
connection.start().then(function () {
    console.log("second screen connected!");
}).catch(function (err) {
    return console.error(err.toString());
});