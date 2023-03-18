"use strict"
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/realalert", { transport: signalR.HttpTransportType.LongPolling })
    .configureLogging(signalR.LogLevel.Information)
    .build();
$(document).ready(function () {
    connection.on("AlertStock", function (res, countNoti, count) {
        $("#sound-alert")[0].play();
        bindStock(res, countNoti, count);
    });
    connection.on("ExpireItem", function (res, countNoti, count) {
        $("#sound-alert")[0].play();
        bindExpireItem(res, countNoti, count);
    });
    connection.on("AlertDueDate", function (res, countNoti, count) {
        $("#sound-alert")[0].play();
        bindDueDate(res, countNoti, count);
    });
    connection.on("AlertCashOut", function (res, countNoti, count) {
        $("#sound-alert")[0].play();
        bindCashOut(res, countNoti, count);
    });
    try {
        connection.start();
        console.info("SignalR Connected.");
    } catch (err) {
        console.error(err);
    }
    $("#duedatefilter").click(function () {
        filter("/ControlAlert/GetDueDateAlert", "#notificationDueDate", "duedate");        
    });
    $("#cashoutfilter").click(function () {
        filter("/ControlAlert/GetCashOutAlert", "#notificationcashout", "cashout");        
    });
    $("#stockfilter").click(function () {
        filter("/ControlAlert/GetStockAlert", "#notification", "stock");
    });
    $("#expireItemfilter").click(function () {
        filter("/ControlAlert/GetExpirationStockItemAlert", "#notificationExpireItem", "expire");
    });
    $.get("/ControlAlert/GetNotification", function (res) {
        bindStock(JSON.stringify(res.Stock), res.CountNoti, res.CountStock);
        bindDueDate(JSON.stringify(res.DueDate), res.CountNoti, res.CountDueDate);
        bindCashOut(JSON.stringify(res.CashOuts), res.CountNoti, res.CountCashOut);
        bindExpireItem(JSON.stringify(res.ExpirationItems), res.CountNoti, res.CountExpirationItem);
    });
    function filter(url, container, type) {
        const dialog = new DialogBox({
            content: {
                selector: "#container-notification-filter"
            },
            button: {
                cancel: {
                    text: "Cancel",
                    callback: function () {
                        dialog.shutdown();
                    }
                },
                yes: {
                    text: "Filter"
                },
                no: {
                    text: "Clear",
                    callback: function () {
                        $.get(url, { typeRead: "", typeOrder: "", isClear: true }, function (res) {
                            $(container).children("div").remove();
                            if (type === "stock") {
                                bindStock(JSON.stringify(res.Stock), res.CountNoti, res.CountStock);
                            }
                            if (type === "duedate") {
                                bindDueDate(JSON.stringify(res.DueDate), res.CountNoti, res.CountDueDate);
                            }
                            if (type === "cashout") {
                                bindCashOut(JSON.stringify(res.CashOuts), res.CountNoti, res.CountCashOut);
                            }
                            if (type === "expire") {
                                bindExpireItem(JSON.stringify(res.ExpirationItems), res.CountNoti, res.CountExpirationItem);
                            }
                            dialog.shutdown();
                        });
                    }
                }
            },
            type: "yes-no-cancel"
        });
        dialog.confirm(function () {
            var typeRead = $("#readOption").val();
            var typeOrder = $("#orderOption").val();
            $.get(url, { typeRead, typeOrder }, function (res) {
                $(container).children("div").remove();
                if (type === "stock") {
                    bindStock(JSON.stringify(res.Stock), res.CountNoti, res.CountStock);
                }
                if (type === "duedate") {
                    bindDueDate(JSON.stringify(res.DueDate), res.CountNoti, res.CountDueDate);
                }
                if (type === "cashout") {
                    bindCashOut(JSON.stringify(res.CashOuts), res.CountNoti, res.CountCashOut);
                }
                if (type === "expire") {
                    bindExpireItem(JSON.stringify(res.ExpirationItems), res.CountNoti, res.CountExpirationItem);
                }
                dialog.shutdown();
            });
        });
    }    
    function bindStock(data, countNoti, count) {
        if (countNoti == 0) {
            $(".ck-notification .n-value").css("width", "0px");
        } else {
            $(".ck-notification .n-value").css("width", "15px");
        }
        $("#numberOfNotification").text(countNoti == 0 ? "" : countNoti > 99 ? "99+" : countNoti);
        $("#stockAlertcount").text(count > 99 ? "99+" : count);
        const _data = JSON.parse(data);
        if (data.length > 0) {
            _data.forEach(i => {
                var date = i.CreatedAt.split("T")[0];
                var time = i.CreatedAt.split("T")[1].split(".")[0];
                let _container = $(`
                <div style="margin-bottom: 10px;" data-key="${i.ItemID}" data-id="${i.ID}" class="cursor ${i.IsRead ? "isRead" : "isNotRead"}" >
                    <div class="row">
                        <div class="col-md-3 img-area">
                            <img src="/Images/items/${i.Image === "" ? "no-image.jpg" : i.Image}" class="img" />
                        </div>
                        <div class="col-md-7 textDiv">
                            <p style="font-size: 10px;" class="text-nowrap mb-0 text-left">Type: ${i.Type}</p>
                            <p class="text-nowrap mb-0 font-size text-left">${i.ItemName}</p>
                            <p class="text-nowrap mb-0 font-size text-left">Instock: ${i.InStock}. Warehouse: ${i.WarehouseName}</p>
                            <small class="smallText">${date} ${time}</small>
                        </div>
                    </div>
               </div>
            `);
                $("#notification").prepend(_container);
                _container.click(function () {
                    const itemId = $(this).data("key");
                    const id = $(this).data("id");
                    $.post("/ControlAlert/UpdateStockAlert", { id }, function (res) {
                        if (res) location.href = `/ItemMasterData/Edit?id=${itemId}&currPage=1/10/${itemId}`;
                        else {
                            const errDialog = new DialogBox({
                                content: "Not Found",
                            });
                            errDialog.confirm(function () {
                                errDialog.shutdown();
                            });
                        }
                    });
                });
            });
        }
    }
    function bindExpireItem(data, countNoti, count) {
        if (countNoti == 0) {
            $(".ck-notification .n-value").css("width", "0px");
        } else {
            $(".ck-notification .n-value").css("width", "15px");
        }
        $("#numberOfNotification").text(countNoti == 0 ? "" : countNoti > 99 ? "99+" : countNoti);
        $("#expireItemAlertcount").text(count > 99 ? "99+" : count);
        const _data = JSON.parse(data);
        if (data.length > 0) {
            _data.forEach(i => {
                var date = i.CreatedDate.split("T")[0];
                var time = i.CreatedDate.split("T")[1].split(".")[0];
                let _container = $(`
                <div style="margin-bottom: 10px; height: 90px;" data-key="${i.ItemId}" data-id="${i.ID}" class="cursor ${i.IsRead ? "isRead" : "isNotRead"}" >
                    <div class="row">
                        <div class="col-md-1"></div>
                        <div class="col-md-3 img-areaExpire">
                            <img src="/Images/items/${i.ImageItem === "" ? "no-image.jpg" : i.ImageItem}" class="imgExpire" />
                        </div>
                        <div class="col-md-7 textDiv">
                            <p style="font-size: 10px;" class="text-nowrap mb-0 text-left">Type: ${i.Type}</p>
                            <p class="text-nowrap mb-0 font-size text-left">Item Name: ${i.ItemName}</p>
                            <p class="text-nowrap mb-0 font-size text-left">Batch No: ${i.BatchNo}</p>
                            <p class="text-nowrap mb-0 font-size text-left">Expiration date: ${i.ExpirationDate.split("T")[0]}</p>
                            <p class="text-nowrap mb-0 font-size text-left">Instock: ${i.Instock} ${i.UomName}. Warehouse: ${i.WarehouseName}</p>
                            <small class="smallText">${date} ${time}</small>
                        </div>
                    </div>
               </div>
            `);
                $("#notificationExpireItem").prepend(_container);
                _container.click(function () {
                    const id = $(this).data("id");
                    $.post("/ControlAlert/UpdateExpirationItemAlert", { id }, function (res) {
                        if (res) location.href = `/ControlAlert/ExprationItemAlert?id=${id}`;
                        else {
                            const errDialog = new DialogBox({
                                content: "Not Found",
                            });
                            errDialog.confirm(function () {
                                errDialog.shutdown();
                            });
                        }
                    });
                });
            });
        }
    }
    function bindDueDate(data, countNoti, count) {
        if (countNoti == 0) {
            $(".ck-notification .n-value").css("width", "0px");
        } else {
            $(".ck-notification .n-value").css("width", "15px");
        }
        $("#numberOfNotification").text(countNoti == 0 ? "" : countNoti > 99 ? "99+" : countNoti);
        $("#dueDateAlertcount").text(count > 99 ? "99+" : count);
        const _data = JSON.parse(data);
        if (data.length > 0) {
            _data.forEach(i => {
                var date = i.CreatedAt.split("T")[0];
                var time = i.CreatedAt.split("T")[1].split(".")[0];
                let _container = $(`
                <div style="margin-bottom: 10px;" data-invoiceid="${i.InvoiceID}" data-id="${i.ID}" class="cursor ${i.IsRead ? "isRead" : "isNotRead"}" >
                    <div class="row">
                        <div class="col-md-1 img-area"></div>
                        <div class="col-md-7 textDiv">
                            <p style="font-size: 10px;" class="text-nowrap mb-0 text-left">Type: ${i.Type}</p>
                            <p class="text-nowrap mb-0 font-size text-left">${i.Type} Name: ${i.Name}</p>
                            <p class="text-nowrap mb-0 font-size text-left">Invoice Number: ${i.InvoiceNumber} </p>
                            <div class='row'>
                                <div class="col-md-6"><p class="text-nowrap mb-0 font-size text-left">Due Date: ${i.DueDate.split("T")[0]}</p></div>
                                <div class="col-md-6"><p class="text-nowrap mb-0 font-size" style="margin: 3px 27px 2px 40px;"><small class="smallText">${date} ${time}</small></div>
                            </div>                            
                        </div>
                    </div>
               </div>
            `);
                $("#notificationDueDate").prepend(_container);
                _container.click(function () {
                    const id = $(this).data("id");
                    const invoiceId = $(this).data("invoiceid");
                    $.post("/ControlAlert/UpdateDueDateAlert", { id }, function (res) {
                        if (res) location.reload();// location.href = `/ControlAlert/DueDateAlert?id=${id}&invoiceId=${invoiceId}`;
                        else {
                            const errDialog = new DialogBox({
                                content: "Not Found",
                            });
                            errDialog.confirm(function () {
                                errDialog.shutdown();
                            });
                        }
                    });
                });
            });
        }
    }
    function bindCashOut(data, countNoti, count) {
        if (countNoti == 0) {
            $(".ck-notification .n-value").css("width", "0px");
        } else {
            $(".ck-notification .n-value").css("width", "15px");
        }
        $("#numberOfNotification").text(countNoti == 0 ? "" : countNoti > 99 ? "99+" : countNoti);
        $("#cashoutAlertcount").text(count > 99 ? "99+" : count);
        const _data = JSON.parse(data);
        if (data.length > 0) {
            _data.forEach(i => {
                var date = i.CreatedAt.split("T")[0];
                var time = i.CreatedAt.split("T")[1].split(".")[0];
                let _container = $(`
                <div style="margin-bottom: 10px; height: 83px" data-id="${i.ID}" class="cursor ${i.IsRead ? "isRead" : "isNotRead"}" >
                    <div class="row">
                        <div class="col-md-1 img-area"></div>
                        <div class="col-md-7 textDiv">
                            <p class="text-nowrap mb-0 font-size text-left">Cashier : ${i.EmpName}, Branch : ${i.BrandName}</p>
                            <p class="text-nowrap mb-0 font-size text-left">Date: In ${i.DateIn.split("T")[0]}  <small>${i.TimeIn}</small>, Out ${i.DateOut.split("T")[0]}  <small>${i.TimeOut}</small></p>
                            <p class="text-nowrap mb-0 font-size text-left">Cash In: ${i.Currency} ${curFormat(i.CashInAmountSys)}, Sale: ${i.Currency} ${curFormat(i.SaleAmountSys)}</p>
                            <p class="text-nowrap mb-0 font-size text-left">Grand Total: ${i.Currency} ${curFormat(i.GrandTotal)}</p>
                            <small class="smallText text-nowrap text-left">${date} ${time}</small>
                        </div>
                    </div>
               </div>
            `);
                $("#notificationcashout").prepend(_container);
                _container.click(function () {
                    const id = $(this).data("id");
                    $.post("/ControlAlert/UpdateCashOutAlert", { id }, function (res) {
                        if (res) location.href = `/ControlAlert/CashOutAlert?id=${id}`;
                        else {
                            const errDialog = new DialogBox({
                                content: "Not Found",
                            });
                            errDialog.confirm(function () {
                                errDialog.shutdown();
                            });
                        }
                    });
                });
            });
        }
    }
    //format currency
    function curFormat(value) {
        return value.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
});