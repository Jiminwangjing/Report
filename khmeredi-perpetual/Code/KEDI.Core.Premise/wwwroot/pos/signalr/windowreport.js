//"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/windowreport").build();
//Initail connection
connection.start().then(function () {
    console.log("window connected!");
}).catch(function (err) {
    return console.error(err.toString());
});