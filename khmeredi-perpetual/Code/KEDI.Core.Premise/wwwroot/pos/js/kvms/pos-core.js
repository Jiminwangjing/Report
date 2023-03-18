"use strict";
function KPOS(config) {
    if (!(this instanceof KPOS)) {
        return new KPOS();
    }

    var __config = {
        orderOnly: false,
        master: {
            keyField: ""
        },
        detail: {
            keyField: "",
            fieldName: "OrderDetail"
        }
    }

    if (isValidJSON(config)) {
        __config = $.extend(true, __config, config);
    }

    let __this = this,
        __setting = {},
        __baseItemGroups = new Array(),
        __listOrders = new Array(),
        __serviceItems = new Array(),
        //AutoMobile
        __autoMobile = {},
        __activeOrder = {},
        __activeOrderDetail = {},
        __activeOrderTable = {},
        __activeCurrency = {},
        __btnPanelPaymentFallback = function () { }
    let __loadscreen = "#loadscreen",
        __notFound = "#image-not-found",
        __toggleGridViewMobile = "#toggle-gridview-mobile.screen-min-max",
        __btnPanelTable = ".goto-panel-group-tables",
        __btnPanelPayment = "#goto-panel-payment",
        __btnPanelOrder = "#goto-panel-group-items",
        __wrapGridTimeTable = "#table-item-gridview .wrap-grid",
        __tableGroup = ".table-group",
        __itemListview = "#item-listview",
        __wrapGridItem = "#group-item-gridview .wrap-grid",
        __textSearch = "#item-search",
        __barcodeSearch = "#item-search-barcode",
        __listOrderReceipt = "#list-order-receipt",
        __groupItemStep = ".group-item-steps";
    const __template = {
        div: document.createElement("div"),
        i: document.createElement("i")
    },
        $groupGridItemPaging = DataPaging({
            container: "#pagination-list",
            pageSize: 12,
            startIndexSize: 4
        }),
        $gridItemPaging = DataPaging({
            container: "#pagination-list",
            keyField: "ID",
            pageSize: 12,
            startIndexSize: 4
        }),
        $listview = ViewTable({
            selector: __itemListview,
            keyField: "Line_ID",
            visibleFields: ["Code", "KhmerName", "UnitPrice", "Qty", "Uom", "DiscountRate", "Total"],
            columns: [
                {
                    name: "UnitPrice",
                    dataType: "number",
                    title: {
                        enabled: true,
                        text: "Double click to copy the item."
                    },
                    template: "<div class='csr-pointer'></div>",
                    on: {
                        "dblclick": function (e) {
                            copyItem(e.data);
                        }

                    }
                },
                {
                    name: "Code",
                    title: {
                        enabled: true,
                        text: "Click to comment on the item."
                    },
                    template: "<div class='csr-pointer'></div>",
                    on: {
                        "click": function (e) {
                            __this.commentItem(e.data);
                        }
                    }
                }
            ],
            scaleColumns: [
                {
                    name: "Qty",
                    onScaleDown: {
                        "click": function (e) {
                            updateOrderDetail(e.key, -1, false);
                        }
                    },
                    onScaleUp: {
                        "click": function (e) {
                            updateOrderDetail(e.key, 1, false);
                        }
                    }

                }
            ],
            actions: [
                {
                    template: "<i class='fas fa-trash-alt csr-pointer fn-red'></i>",
                    on: {
                        "click": function (e) {
                            __this.onDeleteItem(e.data);
                        }
                    }
                }
            ]
        }), $itemListView = ViewTable({
            container: "#group-item-listview",
            keyField: "ID",
            visibleFields: ["Code", "KhmerName", "UnitPrice", "UoM"],
            actions: [
                {
                    template: "<i class='fas fa-arrow-circle-left fa-lg fn-gainsboro csr-pointer'></i>",
                    on: {
                        "click": function (e) {
                            updateOrderDetail(e.key, 1, true);
                        }
                    }
                }
            ],
            paging: {
                enabled: false
            }
        });

    //SignalR Implementation
    this.startSignalR = function (onConnected) {
        var connection = new signalR.HubConnectionBuilder()
            .withUrl("/pos")
            .build();
        connection.onclose((e) => {
            console.info('Connection closed!', e);
        });

        connection.start().then(function () {
            if (typeof onConnected === "function") {
                onConnected.call(connection, __this.fallbackInfo());
            }
        }).catch(function (err) {
            return console.error('Error connection->:' + err.toString());
        });
    }

    //Search table grids
    //maksokmanh
    $("#panel-view-mode").on("click", function () {
        var viewMode = $(this).data("view-mode");
        switchPanelViewMode(viewMode);
    });

    $("#panel-resizer").on("click", function () {
        $(this).toggleClass("fa-compress fa-expand");
        $("#panel-group-items .right-panel").toggleClass("hide");
        if ($("#panel-group-items .right-panel").hasClass("hide")) {
            $("#group-buttons").addClass("fixed-width-half");
        } else {
            $("#group-buttons").removeClass("fixed-width-half");
        }
    });

    $(__toggleGridViewMobile).on("click", function () {
        $(this).toggleClass("fa-toggle-on fa-toggle-off");
        $("#panel-group-items .right-panel").toggleClass("hide");
    });

    $(__btnPanelPayment).on("click", function () {
        __this.fallbackInfo(function (posInfo) {

            if (posInfo.AutoMobile.AutoID == undefined) {
                $.post("/KVMS/CheckIfCuzHasV", { CusID: posInfo.Order.CustomerID }, function (_res) {
                    if (_res.CusV == "Yes") {
                        $("#modal-warning-save-quote-hascusv").modal("show");
                    } else {
                        __this.checkCart(function () {
                            __this.gotoPanelPayment(__btnPanelPaymentFallback);
                        });
                    }
                });
            } else {
                __this.checkCart(function () {
                    __this.gotoPanelPayment(__btnPanelPaymentFallback);
                });
            }
        });
    });

    $(__btnPanelOrder).on("click", function () {
        __this.gotoPanelItemOrder();
    });

    $('#search-tables').on('keyup', function () {
        __this.bindServiceTables(this.value, __wrapGridTimeTable, "#all-table");
    });

    $(__btnPanelTable).on("click", function () {
        __this.gotoPanelGroupTable();
    });

    $(__wrapGridTimeTable).children(".grid").on("click", onClickTimeTableGrid);

    $(__textSearch).on("keyup", function () {
        let keyword = this.value;
        searchSaleItems("/KVMS/SearchSingleItems", keyword, function (itemGrids) {
            if (itemGrids.length > 0) {
                $(__wrapGridItem).children().remove();
                bindSingleItemGrids(itemGrids);
            }
        });
    });

    $(__barcodeSearch).on("change", function () {
        let barcode = this.value;
        let items = $.grep(__serviceItems, function (item) {
            return item.Barcode.toLowerCase() === barcode.toLowerCase();
        });
        if (items.length > 0) {
            items[0].PrintQty++;
            items[0].Qty++;
            $(__wrapGridItem).children().remove();
            bindSingleItemGrids(items);
            onFirstClickItemGrid(items[0].ID, 1);
        }
    });

    this.bindTimeTableGrids = function (tableTimers) {
        for (let i = 0; i < tableTimers.length; i++) {
            $('#' + tableTimers[i].tableTime.id).html(tableTimers[i].tableTime.time);
            $("#user" + tableTimers[i].tableTime.id).text("");
            switch (tableTimers[i].tableTime.status) {
                case 'A':
                    updateTableGrids(tableTimers[i].tableTime, "#CC9");
                    break;
                case 'B':
                    updateTableGrids(tableTimers[i].tableTime, "#E03454");
                    break;
                case 'P':
                    updateTableGrids(tableTimers[i].tableTime, "#50A775");
                    break;
                default:
                    updateTableGrids(tableTimers[i].tableTime, "#CC9");
                    break;
            }
        }

    }

    function updateTableGrids(tableTime, statusColor) {
        let grids = $(__wrapGridTimeTable).children();
        $('#' + tableTime.id).css("color", "white");
        $.each(grids, function (i, grid) {
            let grid_id = $(grid).data('id');
            if (grid_id === tableTime.id) {
                $(grid).css("background-color", statusColor);
                $(grid).find(".grid-image").find(".time").remove();
                $(grid).find(".grid-image")
                    .append(tableTime.status == 'A' ? "" : "<div class='time'>" + tableTime.time + "</div>");
            }
        });
    }

    this.onDeleteItem = function (item) {
        //Previlege for deleting item
        this.checkPrivilege("P011", function () {
            let dialogDelete = __this.dialog("Delete item",
                "Are you sure you want to remove " + item.KhmerName + "(" + item.Code + ")?",
                "warning", "ok-cancel"
            );

            dialogDelete.confirm(function () {
                item.PrintQty = parseFloat(item.Qty) * (-1);
                item.Qty = 0;
                updateOrderDetail(item.Line_ID, item.PrintQty);
                dialogDelete.shutdown();
            });

            dialogDelete.reject(function () {
                dialogDelete.shutdown();
            });
        });
    }

    this.load = function (onSucceed) {
        //Use TableID =  1 by default
        this.fetchOrders(1, onSucceed);
    }

    //VMC Edition
    function onFirstClickItemGrid(itemKey, scaleValue) {
        let OrderDetailQAutoMs = $listview.find(itemKey);
        if (OrderDetailQAutoMs) {
            OrderDetailQAutoMs.PrintQty += scaleValue;
            OrderDetailQAutoMs.Qty += scaleValue;
            updateOrderDetail(itemKey, OrderDetailQAutoMs.PrintQty);
        } else {
            updateOrderDetail(itemKey, scaleValue, true);
        }
    }

    this.fetchOrders = function (tableId, orderId, setDefaultOrder, onSucceed) {
        __activeOrderDetail = {};
        __this.loadScreen();
        $listview.clearRows();
        $("#panel-resizer").removeClass("hidden");
        $.get("/KVMS/FetchOrders",
            {
                tableId: tableId,
                orderId: orderId,
                setDefaultOrder: setDefaultOrder
            },
            function (resp) {
                __baseItemGroups = resp.BaseItemGroups;
                __listOrders = resp.Orders;
                __setting = resp.Setting;
                __activeOrder = resp.Order;
                __activeOrderTable = resp.OrderTable;
                __activeCurrency = resp.DisplayCurrency;
                __serviceItems = resp.ServiceItems;
                if (setDefaultOrder) {
                    bindOrderNumbers(__listOrders, __activeOrder.OrderID);
                }
                __this.setOrder(__activeOrder);
                if (typeof onSucceed === "function") {
                    onSucceed.call(__this, __this.fallbackInfo());
                }
                __this.loadScreen(false);
            }
        );
    };

    function checkOrderDetails(orderDetails) {
        let isValid = false;
        if (isValidArray(orderDetails)) {
            $.grep(orderDetails, function (item, i) {
                let newProps = Object.getOwnPropertyNames(item),
                    prevProps = Object.getOwnPropertyNames(__activeOrder[__config.detail.fieldName][0]);
                if (newProps.length == prevProps.length) {
                    for (let j = 0; j < newProps.length; j++) {
                        if (newProps[i] === prevProps[i]) {
                            isValid = true;
                        }
                    }
                }
            });
        }
        return isValid;
    }

    function checkOrder(order) {
        let isValid = false;
        if (isValidJSON(order)) {
            let newProps = Object.getOwnPropertyNames(order),
                prevProps = Object.getOwnPropertyNames(__activeOrder);
            if (newProps.length == prevProps.length) {
                for (let j = 0; j < newProps.length; j++) {
                    if (newProps[j] === prevProps[j]) {
                        isValid = true;
                    }
                }
            }
        }
        return isValid;
    }

    this.updateOrder = function (order, orderDetails) {
        if (checkOrderDetails(orderDetails)) {
            order[__config.detail.fieldName] = orderDetails;
        }

        if (checkOrder(order)) {
            __activeOrder = $.extend(true, __activeOrder, order);
        }

        if (isValidJSON(__activeOrder)) {
            let items = __activeOrder[__config.detail.fieldName];
            if (isValidArray(items)) {
                for (let i = 0; i < items.length; i++) {
                    $listview.updateRow(items[i]);
                    updateItemGridView(items[i].Line_ID, items[i].Qty, items[i].PrintQty);
                }
            }
            this.onSummaryOrderQuote(__activeOrder);
        }

        if (typeof order === "function") {
            if (isValidJSON(__activeOrder)) {
                order.call(this, this.fallbackInfo());
            }
        }
    }

    this.setOrderofQuote = function (tableId, orderId) {

        $.get("/KVMS/FindOrderofQuote", { tableId: tableId, orderId: orderId }, function (resp) {
            $listview.clearRows();
            if (isValidJSON(resp.Order)) {
                __activeOrder = resp.Order;
                __activeCurrency = resp.DisplayCurrency;
                __serviceItems = resp.ServiceItems;
                $listview.clearRows();
                if (__activeOrder.OrderID > 0) {
                    $listview.bindRows(__activeOrder[__config.detail.fieldName]);
                    bindItemGrids(__serviceItems, __serviceItems[0]);
                } else {
                    resetGroupItems();
                }
                __this.onSummaryOrderQuote(__activeOrder);
            }
        });
    }

    this.onSummaryOrderQuote = function (order) {
        if (isValidJSON(order)) {
            let result = 0;
            if (isValidArray(order.OrderDetailQAutoMs)) {
                for (let i = 0; i < order.OrderDetailQAutoMs.length; i++) {
                    result += order.OrderDetailQAutoMs[i].Total;
                }
            }
            order.Sub_Total = result;
            order.Total_Sys = result * order.PLRate;
            order.GrandTotal = result * (1 - order.DiscountRate / 100);
            order.GrandTotal_Sys = order.GrandTotal * order.PLRate;
            $("#summary-sub-total").text(__activeCurrency.BaseCurrency + " " + parseFloat(order.Sub_Total).toFixed(3));
            $("#summary-discount").text(order.DiscountRate + " " + "%");
            $("#summary-vat").text(__activeCurrency.BaseCurrency + " " + parseFloat(order.TaxValue).toFixed(3));
            $("#summary-total").text(__activeCurrency.BaseCurrency + " " + parseFloat(order.GrandTotal).toFixed(3));
            $("#send .tag-count").text(order.OrderDetailQAutoMs.length);
        }
    }

    //Update item order details
    function updateOrderDetail(itemKey, scaleValue, isFromGrid = true) {
        let listItem = $listview.find(itemKey);
        if (listItem) {
            listItem.Total = parseFloat(totalOrderDetail(listItem).toFixed(3));
            if (isFromGrid) {
                $listview.updateRow(listItem);
            } else {
                listItem.PrintQty += scaleValue;
                listItem.Total = parseFloat(totalOrderDetail(listItem).toFixed(3));
                $listview.updateColumn(itemKey, "Total", listItem.Total);
                let gridItem = $gridItemPaging.find(listItem.Line_ID);
                if (gridItem) {
                    gridItem.PrintQty = listItem.PrintQty;
                    $gridItemPaging.updateJson(gridItem);
                }
            }

            updateItemGridView(listItem.Line_ID, listItem.Qty, listItem.PrintQty);
            if (listItem.PrintQty >= 0) {
                $listview.yield(function (OrderDetailQAutoMs) {
                    __activeOrder[__config.detail.fieldName] = OrderDetailQAutoMs;
                    __this.onSummaryOrderQuote(__activeOrder);
                });
            }
        }
        else {

            $.ajax({
                url: "/KVMS/GetNewOrderDetail",
                data: { id: itemKey, orderId: __activeOrder.OrderID },
                success: function (OrderDetailQAutoMs) {
                    OrderDetailQAutoMs.Qty = 1;
                    OrderDetailQAutoMs.PrintQty = 1;
                    OrderDetailQAutoMs.Total = totalOrderDetail(OrderDetailQAutoMs);
                    $listview.updateRow(OrderDetailQAutoMs);
                    updateItemGridView(OrderDetailQAutoMs.Line_ID, OrderDetailQAutoMs.Qty, OrderDetailQAutoMs.PrintQty);
                    $listview.yield(function (OrderDetailQAutoMs) {
                        __activeOrder[__config.detail.fieldName] = OrderDetailQAutoMs;
                        __this.onSummaryOrderQuote(__activeOrder);
                    });
                }
            });
        }
    }

    //Prccess add comment to item order detail.
    this.commentItem = function (orderDetail) {
        const dialog = new DialogBox({
            caption: "Comment",
            icon: "fas fa-comments",
            content: {
                selector: "#comment-item-content"
            },
            type: "ok-cancel",
            button: {
                ok: {
                    "text": "Save"
                }
            }
        });

        dialog.invoke(function () {
            let $item = dialog.content.find("#detail-item-comment");
            $item.append(orderDetail.KhmerName + " (" + orderDetail.Code + ")");
            const $chosenComments = ViewTable({
                selector: "#listview-choosed-item-comments",
                visibleFields: ["Index", "Description"],
                keyField: "ID",
                actions: [
                    {
                        template: "<i class='fas fa-trash-alt fa-lg fn-red csr-pointer'></i>",
                        on: {
                            "click": function (e) {
                                removeComment(e.data);
                            }
                        }
                    }
                ]
            });
            const $comments = ViewTable({
                selector: "#listview-item-comments",
                visibleFields: ["Description"],
                keyField: "ID",
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-right fa-lg fn-green csr-pointer'></i>",
                        on: {
                            "click": function (e) {
                                chooseComment(e.data);
                            }
                        }
                    }
                ]
            });

            function chooseComment(comment) {
                let _commented = $chosenComments.find(comment.ID);
                if (!_commented) {
                    let _comment = {};
                    _comment["Index"] = $chosenComments.yield().length + 1;
                    _comment = $.extend(_comment, comment);
                    $chosenComments.updateRow(_comment);
                }

            }

            function removeComment(comment) {
                $chosenComments.removeRow(comment.ID);
            }

            $.get("/POS/GetItemComments", function (comments) {
                let itemComments = new Array();
                if (orderDetail.Comment) {
                    itemComments = orderDetail.Comment.split(",");
                    $.grep(comments, function (cmt) {
                        for (let i = 0; i < itemComments.length; i++) {
                            if (cmt.Description.toLowerCase() == itemComments[i].toLowerCase()) {
                                chooseComment(cmt);
                            }
                        }
                    });
                }

                $comments.bindRows(comments);
            });

            const $saveNewComment = dialog.content.find("#add-new-comment"),
                $searchComments = dialog.content.find("#search-item-comment"),
                $message = dialog.content.find(".error-message").addClass("fn-green");

            $searchComments.on("keyup", function () {
                $message.text("");
                let $descComment = dialog.content.find("#listview-item-comments tr:not(:first-child)");
                var value = $(this).val().toLowerCase();
                $descComment.filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
                });

                $saveNewComment.hide();
                if ($(this).text().toLowerCase().indexOf(value) <= -1) {
                    $saveNewComment.show();
                }

            });

            $(window).on("keypress", function (e) {
                if (e.which == 13 && $searchComments.is(":focus")) {
                    saveComment();
                }
            });

            $saveNewComment.on("click", function () {
                saveComment();
            });

            function saveComment() {
                if ($searchComments.val() !== "") {
                    $.post("/KVMS/CreateItemComment", {
                        description: $searchComments.val()
                    }, function (resp) {
                        if (resp.ErrorMsg === "") {
                            orderDetail.Comment = "";
                            chooseComment(resp.Comment);
                            $comments.updateRow(resp.Comment);
                            $message.addClass("fn-green").removeClass("fn-red").text("New comment added.");
                            setTimeout(function () {
                                $message.addClass("fn-green").removeClass("fn-red").text("");
                            }, 1500);
                            $searchComments.val("");
                        } else {
                            $message.removeClass("fn-green").addClass("fn-red").text(resp.ErrorMsg);
                        }
                    });
                }
            }

            //Save comment description to item.
            dialog.confirm(function () {
                $.each($chosenComments.yield(), function (i, comment) {
                    if (i <= 0) {
                        orderDetail.Comment = comment.Description;
                    } else {
                        orderDetail.Comment += "," + comment.Description;
                    }
                });
                dialog.shutdown();
            });

            dialog.reject(function () {
                dialog.shutdown();
            });
        });
    }

    function copyItem(rowData) {
        let newRow = {}
        newRow = $.extend(newRow, rowData);
        newRow.Line_ID += rowData.Line_ID + 1;
        newRow.OrderDetailID = 0;
        $listview.addRow(newRow);
        updateOrderDetail(newRow.Line_ID, newRow.Qty, false);
    }

    function resetGroupItems(showOnlyAddon = false) {
        $(__groupItemStep).find(":not(:first-child)").remove();
        let groups = __baseItemGroups;
        if (showOnlyAddon) {
            groups = $.grep(__baseItemGroups, function (group) {
                return group.IsAddon || group.KhmerName.includes("*");
            });
        }

        if (isValidArray(groups)) {
            $gridItemPaging.load(groups, groups[0], function (info) {
                $(__wrapGridItem).children().remove();
                $.each(info.dataset, function (i, item) {
                    if (item.ItemID <= 0) {
                        createGroupItemGrid(item, 0);
                    }
                });
            });
        }
    }

    this.checkCart = function (succeeded) {
        if (isValidJSON(__activeOrder)) {
            if (__activeOrder[__config.detail.fieldName].length <= 0) {
                __this.dialog("Empty Cart", "Please order something first!", "warning");
            } else {
                if (typeof succeeded === "function") {
                    succeeded.call(this, __this.fallbackInfo());
                }
            }
        }

    }

    var CusID = 0;
    this.getCID = function (CId) {
        CusID = CId;
    }
    var VID = 0;
    this.getVID = function (VId) {
        VID = VId;
    }
    var SQNoo = "";
    this.getSQNo = function (SQNo) {
        SQNoo = SQNo;
    }

    this.validateOrder = function (order, orderDetails) {

        let __orderDetails = $listview.yield();
        if (checkOrderDetails(orderDetails)) {
            __orderDetails = orderDetails;
        }

        if (checkOrder(order)) {
            __activeOrder = order;
        }

        __activeOrder.Sub_Total = __this.toNumber(__activeOrder.Sub_Total);
        __activeOrder[__config.detail.fieldName] = $.grep(__orderDetails, function (item) {
            item.UnitPrice = __this.toNumber(item.UnitPrice);
            item.Total = __this.toNumber(item.Total);
            return !(item.Qty === 0 && item.PrintQty === 0);
        });
        return __activeOrder;
    }

    this.submitOrder = function (order, printType, succeeded) {

        __activeOrder[__config.detail.fieldName] = $listview.yield();
        __this.validateOrder();
        __this.updateOrder(function (dataInfo) {
            dataInfo.Order.OrderDetail = dataInfo.Order.OrderDetailQAutoMs;
        });

        __this.checkCart(function () {
            __this.loadScreen();
            $.get("/KVMS/GetGeneralSetting", function (setting) {
                if (setting.AutoQueue) {

                    $.post("/KVMS/SendQuote",
                        { data: JSON.stringify(order), printType: printType, CID: CusID, VID: VID, SQNo: SQNoo }, function (itemReturns) {
                            if (itemReturns[0].ReceiptID == 0) {
                                alertOutStock(itemReturns);
                            } else {
                                if (typeof succeeded === "function") {
                                    succeeded(__this.fallbackInfo());
                                }
                                window.open("/KVMS/PrintInvoice?Invid=" + itemReturns[0].ReceiptID + "", "_blank");
                            }
                            __this.loadScreen(false);
                        });
                } else {
                    let dialog = new DialogBox({
                        caption: "Set Order Number",
                        content: {
                            selector: "#order-editorderno"
                        },
                        type: "ok-cancel"
                    });
                    dialog.confirm(function () {
                        let orderNo = dialog.content.find('.order-number').val();
                        if (orderNo === "") {
                            $('.order-number-erorr').text('Please input order number!');
                            dialog.content.find('.order-number').focus();
                        } else {
                            order.OrderNo = orderNo;
                            $.post("/KVMS/Send",
                                { data: JSON.stringify(order), printType: printType },
                                function (itemReturns) {
                                    if (itemReturns.length > 0) {
                                        alertOutStock(itemReturns);
                                    } else {
                                        if (typeof succeeded === "function") {
                                            succeeded(__this.fallbackInfo());
                                        }
                                    }

                                    __this.loadScreen(false);
                                });
                        }
                    });
                    dialog.reject(function () {
                        dialog.shutdown();
                    });
                }
            });
        });
    }

    function switchPanelViewMode(mode) {
        switch (mode) {
            case 0:
                $("#panel-view-mode").attr("src", "/pos/icons/dual-screens.png");
                $("#panel-group-items .right-panel").addClass("width-full").removeClass("hide");
                $("#group-item-gridview").addClass("full-desktop");
                $("#panel-view-mode").data("view-mode", 1);
                break;
            case 1:
                $("#panel-view-mode").attr("src", "/pos/icons/single-screen.png");
                $("#panel-group-items .right-panel").removeClass("width-full hide");
                $("#group-item-gridview").removeClass("full-desktop");
                $("#panel-view-mode").data("view-mode", 0);
                break;
        }
        $("#group-buttons").removeClass("fixed-width-half");
        __setting.PanelViewMode = $("#panel-view-mode").data("view-mode");
        $.post("/POS/UpdateSetting", {
            setting: __setting
        });
    }

    this.resetOrder = function (tableId, setDefaultOrder = false, onSuccess) {
        return __this.fetchOrders(tableId, 0, setDefaultOrder, onSuccess);
    }

    //this.resetOrder = function () {
    //    resetGroupItems();
    //    __this.setOrderofQuote(__activeOrder.TableID, 0);
    //}

    //End of VMC Edition

    //this.setOrder = function (order) {
    //    $listview.clearRows();
    //    if (order.OrderID > 0) {
    //        $listview.bindRows(order[__config.detail.fieldName]);
    //        $gridItemPaging.load(__serviceItems, __serviceItems[0], function (info) {
    //            $(__wrapGridItem).children().remove();
    //            bindSingleItemGrids(info.dataset);
    //        });
    //    } else {
    //        resetGroupItems();
    //    }
    //    __this.onSummaryOrderQuote(order);
    //}

    this.setOrder = function (order) {
        let pageSetting = {};
        pageSetting.pageSize = __setting.ItemPageSize;
        $gridItemPaging.configure(pageSetting);
        bindOrderDetails(order[__config.detail.fieldName]);
        switch (__setting.ItemViewType) {
            case 0:
                $("#group-item-gridview").show();
                $("#group-item-listview").hide();
                showInGridview(order);
                break;
            case 1:
                $("#group-item-gridview").hide();
                $("#group-item-listview").show();
                showInListview(__serviceItems);
                break;
        }
        __this.onSummaryOrderQuote(order);
    }

    function showInListview(items) {
        $gridItemPaging.load(items, items[0], function (info) {
            $itemListView.bindRows(info.dataset);
        });
    }

    function showInGridview(order) {
        if (order.OrderID > 0) {
            let _items = $.grep(__serviceItems, function (item) {
                return isValidJSON($listview.find(item.ID));
            });
            $gridItemPaging.load(_items, _items[0], function (info) {
                $(__wrapGridItem).children().remove();
                bindSingleItemGrids(info.dataset);
            });
        } else {
            resetGroupItems();
        }
    }

    function bindOrderDetails(items) {
        let _addons = $.grep(items, function (item) {
            return item.ItemType.toLowerCase() === "addon";
        });
        if (isValidArray(items)) {
            for (let i = 0; i < items.length; i++) {
                if (isValidJSON(items[i])) {
                    if (items[i].ItemType.toLowerCase() !== "addon") {
                        $listview.addRow(items[i]);
                        for (let j = 0; j < _addons.length; j++) {
                            if (parseInt(_addons[j].ParentLevel) === items[i].Line_ID) {
                                $listview.addRow(_addons[j], function (info) {
                                    $(info.row).find("[data-field='Code']")
                                        .html("<i class='fas fa-plus-circle fn-green fa-xs'></i>")
                                        .css("text-align", "right");
                                });
                            }
                        }
                    }
                }
            }
        }
    }

    //Changed from bindItemOrder to bindOrderNumbers
    function bindOrderNumbers(orders, activeOrderId) {
        let $listBox = $(__listOrderReceipt);
        $listBox.find("#dropbox-order").children().remove();
        $listBox.find("#badge-order").text("");
        if (isValidArray(orders)) {
            __listOrders = new Array();
            for (let i = 0; i < orders.length; i++) {
                __this.addNewOrder(orders[i], activeOrderId);
            }
        }
        $listBox.find("#add-new-order").on("click", function (e) {
            __this.resetOrder(__activeOrder.TableID);
            $listBox.find("#dropbox-order .option").removeClass("active");
        });
    }

    this.addNewOrder = function (order, activeOrderId) {
        if (isValidJSON(order) && order.OrderID > 0) {
            __listOrders.push(order);
            let $listBox = $(__listOrderReceipt);
            let $option = $("<div data-id='" + order.OrderID + "' class='option csr-pointer'><i class='fas fa-receipt'></i> "
                + order.OrderNo + " (" + order.TimeIn + ")" + "</div > ");
            if (order.OrderID === activeOrderId) {
                $option.addClass("active");
            }

            if (order.CheckBill === 'Y') {
                $option.addClass("bg-light-green");
            }

            $option.on("click", function () {
                __this.fetchOrders(order.TableID, order.OrderID);
                $(this).addClass("active").siblings().removeClass("active");
            });

            $listBox.find("#dropbox-order").prepend($option);
            $listBox.find("#badge-order").html("<span class='badge'>" + __listOrders.length + "</span>");
        }
    }

    function searchSaleItems(url, keyword, success) {
        if (keyword !== "") {
            $.get(url,
                { keyword: keyword },
                success);
        } else {
            bindGroupItemGrids(0, 0, 0, 0);
        }
    }

    this.bindServiceTables = function (keyword, selector, groupSelector) {
        if (groupSelector !== undefined) {
            $(groupSelector).siblings().remove();
        }

        if (keyword !== "") {
            let data = { keyword: keyword };
            $.get("/KVMS/SearchTables", data, function (tables) {
                bindTimeTableGrids(tables, selector);
            });
        } else {
            $.get("/KVMS/GetServiceTables", function (resp) {
                bindGroupTimeTables(resp.GroupTables);
                bindTimeTableGrids(resp.Tables, selector);
            });
        }
    }

    function updateItemGridView(itemKey, qty, printQty) {
        let $grid = $(__wrapGridItem).find(".grid[data-id='" + itemKey + "']");
        let $scaleBox = $grid.find(".scale-box");
        if (qty <= 0) {
            $listview.removeRow(itemKey);
            $scaleBox.removeClass("active");
        }

        if (printQty <= 0) {
            $scaleBox.removeClass("active");
            $grid.removeClass("active");
            $grid.find(".icon-calculator").removeClass("show");
        } else {
            let $scaleLabel = $scaleBox.find(".scale-label").text(printQty);
            $scaleBox.find(".scale-label").removeClass("small-font2x small-font3x");
            if ($scaleLabel.text().length > 4) {
                $scaleLabel.addClass("small-font2x");
                if ($scaleLabel.text().length > 5) {
                    $scaleLabel.removeClass("small-font2x").addClass("small-font3x");
                }
            }
            $scaleBox.addClass("active");
            $grid.addClass("active").siblings().removeClass("active");
            $grid.find(".icon-calculator").addClass("show");
        }
    }

    function totalOrderDetail(listItem) {
        switch (listItem.TypeDis) {
            case "Percent":
                listItem.Total = ((listItem.Qty * listItem.UnitPrice) * (1 - (listItem.DiscountRate / 100)));
                listItem.DiscountValue = (listItem.Qty * listItem.UnitPrice) * listItem.DiscountRate / 100;
                break;
            default:
                listItem.Total = listItem.Qty * listItem.UnitPrice - listItem.DiscountRate;
                listItem.DiscountValue = listItem.DiscountRate;
                break;
        }

        listItem.Total_Sys = parseFloat(listItem.Total) * parseFloat(__activeOrder.ExchangeRate);
        return parseFloat(listItem.Total);
    }

    this.toNumber = function (value) {
        if (typeof value === "number") {
            value = value.toString();
        }

        return parseFloat(value.split(",").join(""));
    }

    this.toCurrency = function (value) {
        return parseFloat(value).toFixed(3).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }

    this.loadScreen = function (enabled = true) {
        $(__loadscreen).show();
        if (!enabled) {
            $(__loadscreen).hide();
        }
    }

    this.saveOrder = function (printType, succeeded) {
        this.submitOrder(__activeOrder, printType, succeeded);
    }

    this.dialog = function (caption, content, icon, type, closeButton = true) {
        return new DialogBox({
            caption: caption,
            content: content,
            icon: icon,
            type: type ? type : undefined,
            closeButton: closeButton
        });
    }

    this.checkPrivilege = function (code, success) {
        $.get("/KVMS/CheckPrivilege", { code: code }, function (cp) {
            if (typeof success === "function") {
                if (cp.Privilege.Used) {
                    success(__this.fallbackInfo(), cp.Privilege);
                } else {
                    let dialog = new DialogBox({
                        caption: "Authentication",
                        content: {
                            selector: "#admin-authorization"
                        },
                        button: {
                            ok: { text: "<i class='fas fa-sign-in-alt'> Access" }
                        }
                    });

                    dialog.confirm(function () {
                        let username = dialog.content.find('.security-username').val();
                        let password = dialog.content.find('.security-password').val();
                        $.ajax({
                            url: "/KVMS/GetUserAccessAdmin",
                            data: { username: username, password: password, code: code },
                            success: function (resp) {
                                dialog.content.find(".error-security-login").text("");
                                ViewMessage({
                                    summary: {
                                        bordered: false
                                    }
                                }).bind(resp.Message)
                                    .appendTo(dialog.content.find(".error-security-login"));
                                if (!resp.Message.IsRejected) {
                                    setTimeout(function () {
                                        success(__this.fallbackInfo(), resp.Privilege);
                                        dialog.shutdown();
                                    }, 500);
                                }

                            }
                        });
                    });
                }

            }
        });
    }

    function clearOrder() {
        __this.fetchOrders(__activeOrder.TableID);
    }

    function alertOutStock(itemReturns) {
        if (itemReturns[0].ReceiptID == 0) {
            let dialog = new DialogBox({
                caption: "Not enough stock",
                icon: "warning",
                content: {
                    selector: "#items-return"
                }
            });
            dialog.invoke(function () {
                ViewTable({
                    keyField: "Line_ID",
                    selector: dialog.content.find("#items-return-listview"),
                    visibleFields: ["Code", "KhmerName", "UomName", "OrderQty", "Instock", "Committed"],
                    paging: {
                        enabled: false
                    }
                }).bindRows(itemReturns);
            });


            dialog.confirm(function () {
                dialog.shutdown();
            });
        }
    }

    function bindGroupTimeTables(tableGroups) {
        $.each(tableGroups, function (i, group) {
            $('<div data-group-id= "' + group.ID + '" class="step">' + group.Name + '</div>');
        });
    }

    function bindTimeTableGrids(tables, selector) {
        $(selector).children().remove();
        $.each(tables, function (i, table) {
            if (!table.Image) {
                table.Image = 'no-image.jpg';
            }

            let userid = "user" + table.ID;
            let $grid = $("<div data-id='" + table.ID + "' data-name='" + table.Name + "' class='grid'></div>")
                .append('<div class="grid-caption">' + table.Name + '</div>');
            let $grid_image = $("<div class='grid-image'></div>");
            switch (table.Status) {
                case 'A':
                    $grid.addClass("status-a");
                    break;
                case 'B':
                    $grid.addClass("status-b");
                    break;
                case 'P':
                    $grid.addClass("status-p");
                    break;
            }

            $grid_image.append('<img src="/Images/table/' + table.Image + '"/><div class="time">' + table.Time + '</div>');
            $grid.append($grid_image);
            $grid.append("<div id='" + userid + "' class='grid-subtitle'></div>");
            $(selector).append($grid.on("click", onClickTimeTableGrid));
        });
    }

    this.showNotFound = function (enabled = true) {
        if (enabled) {
            $(__notFound).removeClass("hide");
        } else {
            $(__notFound).addClass("hide");
        }
    }

    function onClickTimeTableGrid() {
        let tableId = $(this).data("id");
        __this.fetchOrders(tableId);
        __this.gotoPanelItemOrder();
    }

    function bindGroupItemGrids(level, group1, group2, group3, itemId, orderId = 0) {
        $(".not-found").removeClass("show");
        $.get("/KVMS/GetGroupItems", {
            level: level,
            group1: group1,
            group2: group2,
            itemId, itemId,
            orderId, orderId
        }, function (groupItems) {
            if (groupItems.length > 0) {
                level++;
                $groupGridItemPaging.render(groupItems, function (page) {
                    $(__wrapGridItem).children(".grid").remove();
                    $.each(page.dataset, function (i, item) {
                        var data = "", _imagePath = item.Images ? "/Images/itemgroup/" + item.Images : "/Images/no-image.jpg";
                        if (level === 1) {
                            group1 = item.ItemG1ID;
                            data = '<div data-group="' + level + '" data-group1="' + group1 + '" data-group2="' + group2 + '" data-group3="' + group3 + '" data-step="' + item.Name + '" data-id="' + item.ItemG1ID + '" hidden></div>'
                        }
                        else if (level === 2) {
                            group2 = item.ItemG2ID;
                            data = '<div data-group="' + level + '" data-group1="' + group1 + '" data-group2="' + group2 + '" data-group3="' + group3 + '" data-step="' + item.Name + '" data-id="' + item.ItemG2ID + '" hidden></div>'
                        } else {
                            group3 = item.ID;
                            data = '<div data-group="' + level + '" data-group1="' + group1 + '" data-group2="' + group2 + '" data-group3="' + group3 + '" data-step="' + item.Name + '" data-id="' + item.ID + '" hidden></div>'
                        }

                        let $grid = $("<div class='grid'></div>")
                            .append(data)
                            .append('<div class="grid-caption">' + item.Name + '</div>');
                        let $grid_image = $("<div class='grid-image'></div>")
                            .append('<img src="' + _imagePath + '">');

                        $grid.append($grid_image);
                        $grid.on("click", function () {
                            onClickGroupItem(level, group1, group2, group3, item.Name);
                        });
                        $(__wrapGridItem).append($grid);
                    });
                });

            } else {
                level = level - 1;
                $.get("/KVMS/GetItemsByGroup", {
                    level: level,
                    group1: group1,
                    group2: group2,
                    group3: group3,
                    orderId: orderId
                }, function (items) {
                    if (items.length > 0) {
                        bindItemGrids(items, items[0]);
                    }
                });
            }
        });
    }

    $(".all-groups", __groupItemStep).on('click', function (e) {
        resetGroupItems();
        $(this).siblings().removeClass("active");
        $(this).addClass("active");
    });

    function filterGroupItems(group1, group2, group3, orderId, level) {
        __this.loadScreen();
        $(".not-found").removeClass("show");
        $.get("/KVMS/GetGroupItems", {
            group1: group1,
            group2: group2,
            group3, group3,
            orderId, orderId,
            level: level
        }, function (resp) {
            bindItemGrids(resp, resp[0], level);
            __this.loadScreen(false);
        });
    }

    function createGroupItemGrid(item, level) {
        var data = "", _imagePath = item.Image ? "/Images/itemgroup/" + item.Image : "/Images/no-image.jpg";
        let $grid = $("<div class='grid'></div>")
            .append(data)
            .append('<div class="grid-caption">' + item.KhmerName + '</div>');
        let $grid_image = $("<div class='grid-image'></div>")
            .append('<img src="' + _imagePath + '">');
        $grid.append($grid_image);
        $grid.on("click", function () {
            level++;
            let multiUomItems = findMultiUomItems(item.ItemID);
            if (multiUomItems.length > 1) {
                $(__wrapGridItem).children().remove();
                bindSingleItemGrids(multiUomItems);
            } else {
                onClickGroupItem(item.Group1, item.Group2, item.Group3, item.KhmerName, level);
            }
        });
        $(__wrapGridItem).append($grid);
    }

    function findMultiUomItems(itemId) {
        let multiUomItems = $.grep(__serviceItems, function (item) {
            if (item.ItemID > 0) {
                return item.ItemID === itemId;
            }
        });
        return multiUomItems;
    }

    function groupBy(values, keys, process) {
        let grouped = {};
        $.each(values, function (k, a) {
            keys.reduce(function (o, k, i) {
                o[a[k]] = o[a[k]] || (i + 1 === keys.length ? [] : {});
                return o[a[k]];
            }, grouped).push(a);
        });
        if (!!process && typeof process === "function") {
            process(grouped);
        }
        return grouped;
    }

    function onClickGroupItem(group1, group2, group3, name, level) {
        //Find group from controler        
        filterGroupItems(group1, group2, group3, __activeOrder.OrderID, level);
        createGroupItemStep(group1, group2, group3, name, level);
    };

    function createGroupItemStep(group1, group2, group3, name, level) {
        $(__groupItemStep).append(
            $('<li class="active"'
                + '" data-group1="' + group1
                + '" data-group2="' + group2
                + '" data-group3="' + group3
                + '" data-group="' + level + '" >' + name + '</li>')
                .on("click", function () {
                    __activeOrderDetail = {};
                    filterGroupItems(group1, group2, group3, __activeOrder.OrderID, level);
                    $(this).addClass("active").next().remove();
                }));
        $(__groupItemStep).children(":not(:last-child)").removeClass("active");
    }

    function bindItemGrids(items, activeItem, level) {
        $gridItemPaging.load(items, activeItem, function (info) {
            bindGroupItemGrids(info.dataset, level);
        });
    }

    function bindSingleItemGrids(items) {
        $.each(items, function (i, item) {
            let discType = (item.TypeDis.toLowerCase() === "percent") ? "%" : "",
                imagePath = item.Image ? "/Images/" + item.Image : "/Images/no-image.jpg";
            let khName = item.KhmerName + ' (' + item.UoM + ')';

            if (!item.Image) {
                item.Image = 'no-image.jpg';
            }
            let $calc = $("<i class='icon-calculator csr-pointer fas fa-calculator fa-lg fn-gray'></i>").on("click", openCalculator);
            let $grid = clone("div").addClass("grid").attr("data-id", item.ID),
                $title = clone("div").addClass("grid-caption").prop("title", khName).text(khName).prop("title", khName),
                $image = clone("div").addClass("grid-image").append("<img src=" + imagePath + ">"),
                $price = clone("div").addClass("price").append(__activeCurrency.BaseCurrency + " "),
                $priceSlash = clone("div").addClass("price-slash").text(__activeCurrency.BaseCurrency + " " + item.UnitPrice.toFixed(3)),
                $discount = clone("div").addClass("discount hover-resize"),
                $barcode = clone("div").addClass("grid-subtitle").text(item.Barcode),
                $qty = createScale(item, $calc);

            if (item.KhmerName.toLowerCase() !== item.EnglishName.toLowerCase()) {
                $title.text(item.Code + ' ' + item.EnglishName);
            }

            $image.append($qty).append($price);
            if (item.DiscountRate > 0) {
                $discount.text(-item.DiscountRate + "%");
                $image.append($discount);
                $price.append(" " + __this.toCurrency(__this.toNumber(item.UnitPrice) * (1 - item.DiscountRate / 100)));
                $image.append($priceSlash);
            } else {
                $price.append(__this.toCurrency(__this.toNumber(item.UnitPrice) * (1 - item.DiscountRate / 100)));
            }
            $image.append($price);
            $grid.append($title).append($image).append($barcode).append($calc);
            $grid.on("click", function (e) {
                if (!$grid.find(".scale-box").hasClass("active")) {
                    item.PrintQty = 1;
                    item.Qty = 1;
                    onFirstClickItemGrid(item.ID, item.PrintQty);
                }
            });
            $(__wrapGridItem).append($grid);
        });
    }

    function bindGroupItemGrids(items, level) {
        $(__wrapGridItem).children().remove();
        if (items.length > 0) {
            let multiUomItem = groupBy(items, ["ItemID"]);
            let props = Object.getOwnPropertyNames(multiUomItem);
            for (let i = 0; i < props.length; i++) {
                if (parseInt(props[i]) > 1) {
                    if (multiUomItem[props[i]].length > 1) {
                        createGroupItemGrid(multiUomItem[props[i]][0], level);
                    }
                }
            }

            $.each(items, function (i, item) {
                if (item.ItemID <= 0) {
                    createGroupItemGrid(item, level);
                } else {
                    if (multiUomItem[item.ItemID].length === 1) {
                        bindSingleItemGrids(multiUomItem[item.ItemID]);
                    }
                }
            });
        }
    }

    //Mini calculator
    function openCalculator(e) {
        e.preventDefault();
        const calc = new DialogCalculator();
        calc.accept(e => {
            let qty = parseFloat($(calc.template).find(".navigator.output").text());
            let line_id = parseInt($(this).parent(".grid").data('id'));
            calc.shutdown("before", function () {
                if (qty < 0) { return; }
                setQty(line_id, qty);

            });
        });

        $(this).parent().addClass("active");
        $(this).parent().parent().parent().addClass("active")
            .siblings().removeClass("active")
            .find(".wrap-scale").removeClass("active");
    }

    function setQty(itemKey, value) {
        let OrderDetailQAutoMs = $listview.find(itemKey);
        if (OrderDetailQAutoMs) {
            value -= OrderDetailQAutoMs.PrintQty;
        } else {
            value = OrderDetailQAutoMs.PrintQty;
        }

        onFirstClickItemGrid(itemKey, value);
    }

    function createScale(item, $calc) {
        let itemOrder = $listview.find(item.ID);
        if (itemOrder) {
            item.PrintQty = itemOrder.PrintQty;
        }

        let $scaleBox = clone("div").addClass("scale-box"),
            $scaleDown = clone("i").addClass("scale-down").text("-"),
            $scaleUp = clone("i").addClass("scale-up").text("+"),
            $value = clone("div").addClass("scale-label").text(item.PrintQty);
        if (item.PrintQty > 0) {
            $scaleBox.addClass("active");
            $calc.addClass("show");
        }

        $scaleDown.on("click", function (e) {
            e.stopPropagation();
            if (item.PrintQty >= 0) {
                onGridScale(item.ID, -1);
            }
        });

        $scaleUp.on("click", function (e) {
            e.stopPropagation();
            onGridScale(item.ID, 1);
        });

        return $scaleBox.append($scaleDown).append($value).append($scaleUp);
    }

    function onGridScale(itemKey, scaleValue) {
        let listItem = $listview.find(itemKey);
        if (listItem) {
            listItem.PrintQty += scaleValue
            listItem.Qty += scaleValue;
            updateOrderDetail(itemKey, scaleValue);
        }
    }

    //Panel accessibility
    this.gotoPanelGroupTable = function (callback) {
        //resetGroupItems();
        $("#panel-group-tables").addClass("show");
        $("#panel-payment").removeClass("show");
        $("#panel-group-items").removeClass("show");
        $(".nav-header .min-max").removeClass("show");
        if (typeof callback === "function") {
            callback.call(__this, __this.fallbackInfo());
        }
    }

    this.gotoPanelItemOrder = function (callback) {
        $("#panel-group-items").addClass("show")
        $("#panel-group-tables").removeClass("show");
        $("#panel-payment").removeClass("show");
        $(".nav-header .min-max").addClass("show");
        if (typeof callback === "function") {
            callback.call(__this, __this.fallbackInfo());
        }
    }

    this.gotoPanelPayment = function (callback) {
        $("#panel-payment").addClass("show");
        $("#panel-group-tables").removeClass("show");
        $("#panel-group-items").removeClass("show");
        $(".nav-header .min-max").removeClass("show");
        if (typeof callback === "function") {
            callback.call(__this, __this.fallbackInfo());
        }
    }

    this.onPanelPayment = function (callback) {
        if (typeof callback === "function") {
            __btnPanelPaymentFallback = callback;
        }
    }

    //Utility functions
    this.openShift = function () {
        this.checkPrivilege("P012", function () {
            MainMenu.openShift();
        });
    }

    this.sendOrder = function () {
        __this.submitOrder(__activeOrder, "Send", function () {
            if (__config.orderOnly) {
                __this.gotoPanelItemOrder();
            } else {
                __this.gotoPanelGroupTable();
            }
        });
    }

    this.billOrder = function () {
        __this.submitOrder(__activeOrder, "Bill", function () {
            if (__config.orderOnly) {
                __this.gotoPanelItemOrder();
            } else {
                __this.gotoPanelGroupTable();
            }
        });
    }

    this.payOrder = function (onPaid) {
        if (parseFloat(__activeOrder.Received) < parseFloat(__activeOrder.GrandTotal)) {
            if (parseFloat(__activeOrder.Received) > 0) {
                let noten = __this.dialog("Payment", "Payment is not enough!<br>Do you want to pay the remaining balance later?", "warning", "yes-no");
                noten.confirm(function () {
                    noten.shutdown();
                    // Aging Payment with some balance
                    __this.submitOrder(__activeOrder, "Pay", function () {
                        __this.resetOrder();
                        if (__config.orderOnly) {
                            __this.gotoPanelItemOrder();
                        } else {
                            __this.gotoPanelGroupTable();
                        }
                        if (typeof onPaid === "function") {
                            onPaid.call(__this, __this.fallbackInfo());
                        }
                    });
                });
            } else {
                let dpay = __this.dialog("Payment", "You didn't input any balance!<br>Do you want to pay the remaining balance later?", "warning", "yes-no");
                dpay.confirm(function () {
                    dpay.shutdown();
                    // Aging Payment without any balance
                    __this.submitOrder(__activeOrder, "Pay", function () {
                        __this.resetOrder();
                        if (__config.orderOnly) {
                            __this.gotoPanelItemOrder();
                        } else {
                            __this.gotoPanelGroupTable();
                        }
                        if (typeof onPaid === "function") {
                            onPaid.call(__this, __this.fallbackInfo());
                        }
                    });
                });
            }
        } else {
            __this.submitOrder(__activeOrder, "Pay", function () {
                __this.resetOrder();
                if (__config.orderOnly) {
                    __this.gotoPanelItemOrder();
                } else {
                    __this.gotoPanelGroupTable();
                }

                if (typeof onPaid === "function") {
                    onPaid.call(__this, __this.fallbackInfo());
                }

            });
        }
    }

    this.moveTable = function (previousId, currentId, onAfterMove) {
        if (currentId > 0) {
            $.ajax({
                url: '/KVMS/MoveTable',
                type: 'POST',
                data: { previousId: previousId, currentId: currentId },
                success: function (currentTable) {
                    __activeOrderTable = currentTable;
                    let $prevTable = $("#table-item-gridview .wrap-grid")
                        .find(".grid[data-id='" + previousId + "']");
                    switch (currentTable.Status) {
                        case 'B':
                            $prevTable.removeClass("status-b")
                                .siblings(".grid[data-id='" + currentId + "']").addClass("status-b");
                        case 'P':
                            $prevTable.removeClass("status-p")
                                .siblings(".grid[data-id='" + currentId + "']").addClass("status-p");
                    }

                    __this.gotoPanelGroupTable(onAfterMove);
                }
            });
        } else {
            __this.dialog("Move Table Failed", "Please select any table to move!", "warning");
        }
    }

    this.voidOrder = function (orderId) {
        let dialog = __this.dialog("Void Order", "Do you want to void this order ?", "warning", "ok-cancel");
        dialog.confirm(function () {
            $.post("/KVMS/VoidOrder", { orderId: orderId }, function (status) {
                if (status == 'N') {
                    __this.dialog("Void Order", "Please get authorization from administrator to cancel...!", "warning");
                }
                else {
                    clearOrder();
                }
            });
            dialog.shutdown();
        });

    }

    this.fallbackInfo = function (callback) {
        var info = {
            Config: __config,
            Order: __activeOrder,
            OrderTable: __activeOrderTable,
            DisplayCurrency: __activeCurrency,
            Setting: __setting,
            AutoMobile: __autoMobile
        }
        if (typeof callback === "function") {
            callback.call(this, info);
        }
        return info;
    }

    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.keys(value).length > 0;
    }

    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }

    function clone(prop, deep) {
        if (deep) {
            return $(__template[prop]).clone(deep);
        }
        return $(__template[prop]).clone();
    }
}

$(document).ready(function () {
    $(".number").asNumber();
    setInterval(function () {
        const now = new Date().toLocaleString().split(",");
        $("#datetime").text(now[0] + " " + now[1]);
    }, 1000);
    $("#loadscreen").hide();
});