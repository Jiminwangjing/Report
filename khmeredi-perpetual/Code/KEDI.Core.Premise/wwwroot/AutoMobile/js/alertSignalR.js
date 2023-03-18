//"use strict";
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/realbox", { transport: signalR.HttpTransportType.LongPolling })
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

connection.on("SumAlert", function (alertSum) {
    $(".n-value").text(alertSum);
});

connection.on("SendAppCount", function (app) {
    $(".app-alert-value").text(app);
});

connection.on("SendStockCount", function (stock) {
    $(".stock-alert-value").text(stock);
});



