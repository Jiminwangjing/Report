
$("#send").on('click', function () {
    if (db.from("tb_check_open_shift") === 0) {
        let msg = new DialogBox(
            {
                caption: "Information",
                content: "Please open shift before send...!",
                position: "top-center",
                type: "ok",//ok, ok-cancel, yes-no, yes-no-cancel
                icon: "info" //info, warning, danger
            }
        );
        msg.setting.button.ok.callback = function (e) {
            this.meta.shutdown();
        };
        return;
    }

    if (db.from("tb_order_detail") !== 0) {
        if (order_master.OrderID === 0) {
            sendData(0, 'Send');
        }
        else {

            $("#send").text("Updating ...");
            sendData(order_master.OrderID, 'Send');
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

});

function sendData(orderid, print_type) {
   
    let loader = null;
    loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
    $('#pay').text('Paying...');
    Order = {
        OrderID: orderid,
        OrderNo: order_master.OrderNo,
        TableID: table_info.id,
        ReceiptNo: order_master.ReceiptNo,
        QueueNo: order_master.QueueNo,
        DateIn: new Date(order_master.DateIn),
        DateOut: new Date(),
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
        CurrencyDisplay: dis_curr[0].AltCurr,
        DisplayRate: dis_curr[0].Rate,
        GrandTotal_Display: fx.convert(order_master.GrandTotal, { from: fx.base, to: dis_curr[0].AltCurr }),
        Change_Display: fx.convert(order_master.Change, { from: fx.base, to: dis_curr[0].AltCurr }),
        PaymentMeansID: parseInt($(".payment-means-id-choosed").val()),
        CheckBill: order_master.CheckBill,
        OrderDetail: db.from("tb_order_detail"),
        OrderDetail_Addon: db.from("tb_order_addon")
    }
    $.ajax({
        url: "/POS/Send",
        type: "POST",
        dataType: "JSON",
        data: { data: JSON.stringify(Order), print_type: print_type },
        success: function (order_return) {
            
            if (order_return.length > 0) {
                returnItemsNotStock(order_return);
                $("#panel-group-items").addClass("show");
                $("#panel-group-items").removeClass("hide");
                $(".nav-header .min-max").addClass("show");
                $("#send").text("Send");
                $('.Table').text("");
                $('#pay').text('Pay');
            }
            else {
                $("#send").text("Send");
                $('.Table').text("");
                $('#pay').text('Pay');
                //$("#panel-group-items").removeClass("show");
                //$("#panel-group-tables").addClass("show");
                clearOrder();
            }
            loader.close();
        }

    });
};