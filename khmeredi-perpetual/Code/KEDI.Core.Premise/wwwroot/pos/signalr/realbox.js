//"use strict";
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/realbox")
    .build();

//Initail connection
connection.onclose((e) => {
    console.info('Connection closed!', e);
});
connection.start().then(function () {
    console.info("Timer connected...");
    //connection.send('TimeWalker', 'csad');
}).catch(function (err) {
    return console.error('error start time->:'+err.toString());
});

//Table timer
connection.on("TimeWalker", function (tableTimerGlobals) {
    let grids = $("#table-item-gridview .wrap-grid .grid"); 
    $.each(tableTimerGlobals, function (key, val) {  
        $('#' + val.tableTime.id).html(val.tableTime.time);
        switch (val.tableTime.status) {
            case 'A':
                $("#user" + val.tableTime.id).text("");
                bindGrids(grids, val.tableTime, "#CC9");               
                break;
            case 'B':
                bindGrids(grids, val.tableTime, "#E03454");
                break;
            case 'P':
                $("#user" + val.tableTime.id).text("");
                bindGrids(grids, val.tableTime, "#50A775");
                break;
        }      
    });

});

//Bind table's time in grids
function bindGrids(grids, tableTime, bgColor) {
    $('#' + tableTime.id).css("color", "white");
    $.each(grids, function (i, grid) {
        let grid_id = $(grid).children().data('id');
        if (grid_id === tableTime.id) {
            $(grid).css("background-color", bgColor);
            $(grid).find(".grid-image").find(".time").remove();
            $(grid).find(".grid-image")
            .append(tableTime.status == 'A' ? "" : "<div class='time'>" + tableTime.time + "</div>");
        }
    });
}

//real time push notification count order on receipt and user stay in table
connection.on("PushOrder", function (orders, tableid, user) {
    $("#user" + tableid).text(user + '...');
    let dropbox_order = $('#dropbox-order');
    if (orders.length !== 0) {
        $("#badge-order").text(orders.length);
        dropbox_order.children().remove();
        $.each(orders, function (i, order) {
            if (order.checkBill === 'Y') {
                dropbox_order.append('<a href="#" class="option " style="color:white;background:#50A775;" data-id=' + order.orderID + ' onclick=clickOnOrderNo(' + order.tableID + ',' + order.orderID + ')><i class="fas fa-receipt"></i> ' + order.orderNo + '/' + order.timeIn + ' </a>');
            }
            else {
                dropbox_order.append('<a href="#" class="option" data-id=' + order.orderID + ' onclick=clickOnOrderNo(' + order.tableID + ',' + order.orderID + ')><i class="fas fa-receipt"></i> ' + order.orderNo + '/' + order.timeIn + ' </a>');
            }
        });
    }
    else {
        $('#badge-order').text(orders.length);
        dropbox_order.children().remove();
    }

});

connection.on("ClearUserOrder", function (tableid) {
    $("#user" + tableid).text("");
});

//real time change status bill in client
connection.on("PushStatusBill", function (order_id) {
    let dropbox_order = $('#dropbox-order .option');
    $.each(dropbox_order, function (i, order) {
        let id = $(order).data("id");
        if (id === order_id) {
            $(order).css("background-color", "#50A775");
            $(order).css("color", "white");
        }
    });
});

//real time move table not work error
connection.on("GetTableAvailable", function (response) {
    let out = "";
    $.ajax({
        url: '/POS/GetTableAvailable',
        type: 'GET',
        data: { group_id: 0, tableid: table_info.id },
        success: function (tables) {
            $.each(tables, function (i, table) {
                out += "<label class='grid rc-container'>"
                    + "<input data-id=" + table.ID + " name='table' type='radio'>"
                    + "<span class='radiomark'></span>"
                    + table.Name
                    + "</label>";
            });
            $('#list-table-free').html(out);
        }
    });
});