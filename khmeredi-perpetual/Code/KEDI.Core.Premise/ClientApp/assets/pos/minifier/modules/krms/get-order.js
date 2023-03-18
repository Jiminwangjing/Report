function GetUoMs() {
    $.ajax({
        url: "/POS/GetUoMs", 
        async: false,
        success: function (uoms) {
            db.insert("tb_uom", uoms, "ID");
        }
    });
};

GetUoMs();
function clickOnOrderNo(tableid, orderid) {
    clearOrder();
    newdefaultOrder();
    let loader = null;
    loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
    $.ajax({
        url: "/POS/GetOrder",
        type: "POST",
        dataType: "JSON",
        data: { tableid: tableid, orderid: orderid, userid: order_master.UserOrderID },
        success: function (response) {
            let orders = response[0];
            //Assign data to order master
            if (orders !== undefined) {
                order_master = {
                    OrderID: orders.OrderID,
                    OrderNo: orders.OrderNo,
                    TableID: orders.TableID,
                    ReceiptNo: orders.ReceiptNo,
                    QueueNo: orders.QueueNo,
                    DateIn: orders.DateIn,
                    DateOut: orders.DateOut,
                    TimeIn: orders.TimeIn,
                    TimeOut: orders.TimeOut,
                    WaiterID: orders.WaiterID,
                    UserOrderID: orders.UserOrderID,
                    UserDiscountID: orders.UserDiscountID,
                    CustomerID: orders.CustomerID,
                    CustomerCount: orders.CustomerCount,
                    PriceListID: orders.PriceListID,
                    LocalCurrencyID: orders.LocalCurrencyID,
                    SysCurrencyID: orders.SysCurrencyID,
                    ExchangeRate: orders.ExchangeRate,
                    WarehouseID: orders.WarehouseID,
                    BranchID: orders.BranchID,
                    CompanyID: orders.CompanyID,
                    Sub_Total: orders.Sub_Total,
                    DiscountRate: orders.DiscountRate,
                    DiscountValue: orders.DiscountValue,
                    TypeDis: orders.TypeDis,
                    TaxRate: orders.TaxRate,
                    TaxValue: orders.TaxValue,
                    GrandTotal: orders.GrandTotal,
                    GrandTotal_Sys: orders.GrandTotal_Sys,
                    Tip: orders.Tip,
                    Received: orders.Received,
                    Change: orders.Change,
                    PaymentMeansID: orders.PaymentMeansID,
                    CheckBill: orders.CheckBill
                };
                if (orders.OrderDetail.length == 0) {
                    $(".Table").text(table_info.name);
                    $("#send").text("Send");
                    clearOrder();
                    newdefaultOrder();
                    return;
                }
                let item_masters = [];
                //Assign data to order detail
                $.each(orders.OrderDetail, function (i, detail) {
                    item = {};
                    item.Line_ID = detail.Line_ID;
                    item.OrderDetailID = detail.OrderDetailID;
                    item.Code = detail.Code;
                    item.ItemID = detail.ItemID;
                    item.KhmerName = detail.KhmerName;
                    item.EnglishName = detail.EnglishName;
                    item.UnitPrice = parseFloat(detail.UnitPrice);
                    item.Cost = detail.Cost;
                    item.Qty = detail.Qty;
                    item.UoM = db.table("tb_uom").get(detail.UomID).Name;
                    item.PrintQty = 0;
                    item.DiscountRate = parseFloat(detail.DiscountRate);
                    item.TypeDis = detail.TypeDis;
                    item.ItemStatus = 'old';
                    item.Currency = detail.Currency;
                    item.Total = detail.Total;
                    item.DiscountValue = detail.DiscountValue;
                    item.Total_Sys = detail.Total_Sys;                    
                    item.UomID = detail.UomID;
                    item.ItemPrintTo = detail.ItemPrintTo;
                    item.Comment = detail.Comment;
                    item.ItemType = detail.ItemType;
                    item.Description = detail.Description;
                    item.ParentLevel = detail.ParentLevel;
                    let item_master = db.from("tb_item_master").first(function (json) {
                        return json.ID == parseInt(detail.Line_ID);
                    });
                    if (item_master != undefined) {
                        item_masters.push(item_master);
                        item_master.Qty = detail.Qty;
                        item_master.KhmerName = detail.KhmerName;
                        item_master.EnglishName = detail.KhmerName;
                        item_master.PrintQty = 0;

                        db.insert('tb_show_item', item_master, 'ID');
                    }
                    db.insert("tb_order_detail", item, "Line_ID");
                });
                singleItem($("#group-item-gridview .wrap-grid"), item_masters);
                //defaultGroup1();
                summaryTotal(0, 'Y');
                $('#send').text('Save');
                $(".Table").text(table_info.name + "  #" + order_master.OrderNo);
               
            }
            else {
                $(".Table").text(table_info.name);
                $("#send").text("Send");
                newdefaultOrder();
            }
            //Customer
            let customer = db.get('tb_customer').get(order_master.CustomerID);
            $('.customer-name').text(customer.Name);
            loader.close();
        }
    });
   
};

