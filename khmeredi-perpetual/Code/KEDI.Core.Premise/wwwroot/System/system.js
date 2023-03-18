"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubSystem", { transport: signalR.HttpTransportType.Websocket })
    .configureLogging(signalR.LogLevel.Information)
    .build();

function encode(text) {
    return text.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
}

connection.on("SignOut", function () {
    $.get("/Account/Logout", function () {
        location.reload();
    });
});

connection.onclose = (e) => {
    console.log('Connecting ...');
    setTimeout(function () {
        startConnection();
    }, 3000);
};

startConnection();
function startConnection() {
    try {
        connection.start();
        console.log('Connected ...');
    } catch (err) {
        console.error(err);
    }
}
