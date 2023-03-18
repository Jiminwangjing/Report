"use strict";
function POSUtils(KPOS) {
    if (!(this instanceof POSUtils)) {
        return new POSUtils(KPOS);
    }
    const __this = this;
    this.load = function (callback) {
        if (typeof callback === "function") {
            callback.call(this, KPOS);
        }
    }

    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.keys(value).length > 0;
    }

    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }

    this.fxDiscountMember = function () {
        KPOS.checkPrivilege("P006", function (posInfo) {
            let dialog = new DialogBox({
                caption: "Discount Member",
                icon: "fas fa-user-friends",
                content: {
                    selector: "#discount-membercard-content"
                },
                button: {
                    ok: {
                        text: "Apply"
                    }
                },
                type: "ok-cancel"
            });

            dialog.invoke(function () {
                let discount = 0;
                let $listVeiw = ViewTable({
                    keyField: "ID",
                    visibleFields: ["Ref_No", "Name", "CardType"],
                    selector: "#listview-discount-membercards",
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-right fn-green csr-pointer'></i>",
                            on: {
                                "click": function (e) {
                                    discount = e.data.Discount;
                                    setMembercardDetail(dialog, e.data);
                                    if (discount > 0) {
                                        dialog.content.find(".error-message").text("");
                                    }
                                }
                            }
                        }
                    ]

                });

                bindMembercards($listVeiw);
                dialog.content.find("#search-membercards").on("keyup", function () {
                    bindMembercards($listVeiw, this.value);
                });

                dialog.confirm(function () {
                    if (discount <= 0) {
                        dialog.content.find(".error-message").text("Please choose any member card!");
                    } else {
                        posInfo.Order.DiscountRate = discount;
                        KPOS.updateOrder(posInfo.Order);
                        dialog.shutdown();
                    }

                });
            });

            dialog.reject(function () {
                dialog.shutdown();
            });

            function bindMembercards($listVeiw, keyword) {
                $.get("/POS/GetMemberCards", {
                    keyword: keyword
                }, function (cards) {
                    $listVeiw.clearRows();
                    $listVeiw.bindRows(cards);
                });
            }

            function setMembercardDetail(dialog, membercard) {
                dialog.content.find(".member-id").text(membercard.Ref_No);
                dialog.content.find(".member-name").text(membercard.Name);
                dialog.content.find(".member-card-type").text(membercard.CardType);
                dialog.content.find(".member-discount").text(membercard.Discount + "%");
                dialog.content.find(".member-expire").text(membercard.ExpireDate);
                dialog.content.find(".member-description").text(membercard.Description);
            }
        });
    }

    this.fxAddNewItem = function () {
        KPOS.fallbackInfo(function (posInfo) {
            $.get("/POS/GetOrderDetailUnknown", function (resp) {
                let printers = resp.Printers;
                let unknownItem = resp.UnknownItem;
                unknownItem.Qty = 1;
                unknownItem.PrintQty = 1;
                if (!resp.UnknownItem || resp.UnknownItem.ItemID <= 0) {
                    new DialogBox(
                        {
                            caption: "Information",
                            content: "Please add default item follow by Khmer Name name is 'unknown' and process type Standard",
                            position: "top-center",
                            type: "ok"
                        });
                } else {

                    let dialog = new DialogBox({
                        caption: "Add New Item",
                        type: "ok-cancel",
                        content: {
                            selector: "#add-new-item-content"
                        }
                    });
                    dialog.invoke(function () {
                        let listNewItems = new Array();
                        listNewItems.push(unknownItem);
                        var $listView = ViewTable({
                            selector: "#add-new-item-listview",
                            keyField: "Line_ID",
                            visibleFields: ["Code", "KhmerName", "Qty", "UnitPrice", "Total", "ItemPrintTo"],
                            columns: [
                                {
                                    name: "KhmerName",
                                    template: "<input>",
                                    on: {
                                        "keyup": function (e) {
                                            let khname = this.value;
                                            $.grep(listNewItems, function (item) {
                                                if (item.Line_ID === e.key) {
                                                    item.KhmerName = khname;
                                                }
                                            });
                                        }
                                    }
                                },
                                {
                                    name: "Code",
                                    template: "<input>",
                                    on: {
                                        "keyup": function (e) {
                                            let code = this.value;
                                            $.grep(listNewItems, function (item) {
                                                if (item.Line_ID === e.key) {
                                                    item.Code = code;
                                                }
                                            });
                                        }
                                    }
                                },
                                {
                                    name: "UnitPrice",
                                    template: "<input class='number'>",
                                    on: {
                                        "keyup": function (e) {
                                            let unitPrice = parseFloat(this.value);
                                            $.grep(listNewItems, function (item) {
                                                if (item.Line_ID === e.key) {
                                                    updateUnitPrice(item, unitPrice);
                                                    item.UnitPrice = unitPrice;
                                                    $listView.updateColumn(e.key, "Total", item.Total.toFixed(3));
                                                }
                                            });
                                        }

                                    }
                                }
                            ]
                        });
                        let __item = {};
                        __item = $.extend(__item, unknownItem);
                        __item.ItemPrintTo = createPrinters(listNewItems, __item.Line_ID, printers);
                        $listView.updateRow(__item);
                        dialog.content.find("#add-new-row").on("click", function () {
                            $.get("/POS/GetOrderDetailUnknown", function (resp) {
                                let __newItem = {};
                                resp.UnknownItem.Qty = 1;
                                resp.UnknownItem.PrintQty = 1;
                                __newItem = $.extend(__newItem, resp.UnknownItem);
                                __newItem.ItemPrintTo = createPrinters(listNewItems, __newItem.Line_ID, printers);
                                $listView.updateRow(__newItem);
                                listNewItems.push(resp.UnknownItem);
                                dialog.content.find("table input.number").asPositiveNumber();
                            });
                        });
                        dialog.content.find("table input.number").asPositiveNumber();
                        dialog.confirm(function () {
                            posInfo.Order.OrderDetailQAutoMs = Array.from(listNewItems);
                            KPOS.updateOrder(posInfo.Order);
                            dialog.shutdown();

                        });
                    });

                    dialog.reject(function () {
                        dialog.shutdown();
                    });

                }

            });

            function createPrinters(listNewItems, itemKey, printers) {
                let _select = $("<select></select>");
                if (printers.length > 0) {
                    for (let i = 0; i < printers.length; i++) {
                        let pname = $("<option value='" + printers[i].ID + "'>" + printers[i].Name + "</option>");
                        _select.append(pname);
                    }
                }
                $(_select).on("change", function () {
                    let _value = $(this).find("option:selected").text();
                    $.grep(listNewItems, function (item) {
                        if (item.Line_ID === itemKey) {
                            item.ItemPrintTo = _value;
                        }
                    });
                });
                return _select;
            }
        });
    }

    this.highlight = function (target, enabled = true) {
        if (target) {
            if (enabled) {
                $(target).addClass("active").siblings().removeClass("active");
            } else {
                $(target).removeClass("active");
            }
        }
    }

    this.fxCancelReceipts = function (target = undefined) {
        __this.highlight(target);
        KPOS.checkPrivilege("P020", function () {
            let dialog = new DialogBox({
                position: "top-center",
                caption: "Cancel Receipt",
                icon: "fas fa-trash-restore",
                content: {
                    selector: "#cancel-receipt-content"
                },
                button: {
                    ok: {
                        text: "Close",
                        callback: function (e) {
                            this.meta.shutdown();
                        }
                    }
                },
                icon: "fas fa-print"
            });

            dialog.invoke(function () {
                const $listView = ViewTable({
                    selector: ".cancel-receipt-listview",
                    visibleFields: ["ReceiptNo", "Cashier", "DateOut", "TimeOut", "GrandTotal"],
                    keyField: "ReceiptID",
                    paging: {
                        pageSize: 6
                    },
                    actions: [
                        {
                            template: "<i class='fas fa-trash-alt csr-pointer fn-red'></i>",
                            on: {
                                "click": function (e) {
                                    cancelReceipt(e.data, $listView);
                                }
                            }
                        }
                    ]
                });

                let $dateFrom = dialog.content.find(".cancel-date-from");
                let $dateTo = dialog.content.find(".cancel-date-to");
                $dateFrom[0].valueAsDate = new Date();
                $dateTo[0].valueAsDate = new Date();
                let __dateFrom = $dateFrom.val(),
                    __dateTo = $dateTo.val();
                searchReceipts($listView, __dateFrom, __dateTo);
                $dateFrom.on("change", function () {
                    __dateFrom = this.value;
                    searchReceipts($listView, __dateFrom, __dateTo);
                });

                $dateTo.on("change", function () {
                    __dateTo = this.value;
                    searchReceipts($listView, __dateFrom, __dateTo);
                });

                //Search receipts to cancel
                dialog.content.find("#search-receipt-cancel").keyup(function () {
                    searchReceipts($listView, __dateFrom, __dateTo, this.value);
                });

                dialog.confirm(function () {
                    __this.highlight(target, false);
                    dialog.shutdown();
                });

            });
        });

        function searchReceipts($listView, dateFrom, dateTo, keyword = "") {
            $.get("/POS/GetReceiptCancels",
                {
                    dateFrom: dateFrom,
                    dateTo: dateTo,
                    keyword: keyword
                },
                function (receipts) {
                    $listView.clearRows();
                    $.grep(receipts, function (rec) {
                        if (rec.DateOut) {
                            rec.DateOut = rec.DateOut.toString().slice(0, 10);
                        }
                    });
                    $listView.bindRows(receipts);
                });
        }

        function cancelReceipt(receipt, $listView) {
            KPOS.dialog("Cancel Receipt", "Are you sure you want to cancel receipt '" + receipt.ReceiptNo + "'?", "warning", "yes-no")
                .confirm(function () {
                    $.post("/POS/CancelReceipt", {
                        orderId: receipt.ReceiptID
                    });
                    $listView.removeRow(receipt.ReceiptID);
                    this.meta.shutdown();
                });
        }

    }

    this.fxReturnReceipts = function (target = undefined) {
        KPOS.checkPrivilege("P021", function (posInfo) {
            const dialog = new DialogBox({
                caption: "Return Receipts",
                type: "ok-cancel",
                icon: "fas fa-file-invoice-dollar",
                content: {
                    selector: "#return-receipt-content"
                },
                button: {
                    ok: {
                        text: "Apply",
                        callback: function (e) {
                            if (db.from('tb_item_return') != 0) {
                                processReutunItem();
                            }
                            else {
                                $('.error-return-item').text('Error : Can not return, because data empty...!');

                            }
                        }
                    }
                }
            });

            dialog.invoke(function () {
                let $dateFrom = dialog.content.find(".return-date-from"),
                    $dateTo = dialog.content.find(".return-date-to");
                $dateFrom[0].valueAsDate = new Date();
                $dateTo[0].valueAsDate = new Date();
                let $receipts = ViewTable({
                    selector: dialog.content.find(".listview-return-receipts"),
                    keyField: "ReceiptID",
                    visibleFields: ["ReceiptNo"],
                    paging: {
                        pageSize: 5
                    },
                    columns: [
                        {
                            name: "ReceiptNo",
                            on: {
                                "click": function (e) {
                                    onGetReceiptDetails(e.key);
                                }
                            }
                        }
                    ]
                });

                let $receiptDetail = ViewTable({
                    selector: dialog.content.find(".item-return-choosed-listview"),
                    keyField: "ID",
                    visibleFields: ["Code", "KhName", "UoM", "OpenQty", "ReturnQty"],
                    columns: [
                        {
                            name: "ReturnQty",
                            template: "<input class='number'>",
                            on: {
                                "keyup": function (e) {
                                    e.data.ReturnQty = parseFloat(this.value);
                                    checkOverQty(dialog, $receiptDetail.yield());
                                }
                            }
                        }
                    ]
                });

                searchReceiptReturns($receipts, $dateFrom.val(), $dateTo.val());
                $dateFrom.on("change", function () {
                    searchReceiptReturns($receipts, this.value, $dateTo.val());
                });

                $dateTo.on("change", function () {
                    searchReceiptReturns($receipts, $dateFrom.val(), this.value);
                });

                dialog.content.find('#search-return-receipts').keyup(function () {
                    var value = $(this).val().toLowerCase();
                    searchReceiptReturns($receipts, $dateFrom.val(), $dateTo.val(), value);
                });

                dialog.confirm(function () {

                    function __hiddenDia() {
                        dialog.content.find(".error-message").text("");
                    }

                    if ($receiptDetail.yield().length <= 0) {
                        dialog.content.find(".error-message").removeClass("fn-green").addClass("fn-red").text("No item to return.");
                        setTimeout(function () {
                            __hiddenDia();
                        }, 3500);
                    } else {
                        $.post("/KVMS/SendReturnItem", {
                            returnItems: $receiptDetail.yield()
                        }, function (isReturned) {
                            if (isReturned == "CancelF") {
                                var _cancelFirst = KPOS.dialog("Warning", "In order to return items, You have to cancel all the payment within this receipt first. Do you want to cancel it?", "warning", "yes-no")
                                _cancelFirst.confirm(function () {
                                    $.post("/KVMS/CancelReFirst", { ReceiptID: $receiptDetail.yield()[0].ReceiptID }, function (e) {
                                        dialog.content.find(".error-message").removeClass("fn-red").addClass("fn-green").text("Cancel receipt successfully.");
                                        setTimeout(function () {
                                            __hiddenDia();
                                        }, 3500);
                                    });
                                    _cancelFirst.shutdown();
                                });
                            } else {
                                if (isReturned == "true") {
                                    $.grep($receiptDetail.yield(), function (item) {
                                        item.OpenQty -= item.ReturnQty;
                                        item.ReturnQty = 0;
                                        if (item.OpenQty <= 0) {
                                            $receiptDetail.removeRow(item.ID);
                                        } else {
                                            $receiptDetail.updateRow(item);
                                        }
                                    });
                                    dialog.content.find(".error-message").removeClass("fn-red").addClass("fn-green").text("Return item successfully.");
                                    setTimeout(function () {
                                        __hiddenDia();
                                    }, 3500);
                                    dialog.content.find("table input.number").asNumber();
                                } else {
                                    dialog.content.find(".error-message").removeClass("fn-red").addClass("fn-red").text("Please return at least 1 item");
                                    setTimeout(function () {
                                        __hiddenDia();
                                    }, 3500);
                                }
                            }
                        });
                    }

                });

                function onGetReceiptDetails(receiptId) {
                    $.get("/KVMS/GetReceiptReturnDetail", {
                        receiptId: receiptId
                    }, function (receiptDetails) {
                        $receiptDetail.clearRows();
                        $receiptDetail.bindRows(receiptDetails);
                        dialog.content.find("table input.number").asNumber();
                    });
                }

            });

            dialog.reject(function () {
                dialog.shutdown();
                __this.highlight(target, false);
            });
        });

        function searchReceiptReturns($listView, dateFrom, dateTo, keyword = "") {
            $.get("/KVMS/GetReceiptReturns", {
                dateFrom: dateFrom,
                dateTo: dateTo,
                keyword: keyword
            }, function (receipts) {
                $listView.clearRows();
                $listView.bindRows(receipts);
            });
        }

        function checkOverQty(dialog, items) {
            let isOverQty = false;
            $.grep(items, function (_item) {
                if (parseFloat(_item.ReturnQty) > _item.OpenQty) {
                    isOverQty = true;
                }
            });

            if (isOverQty) {
                dialog.content.find(".error-message").text("Returned quantity cannot exceed original quantity.");
                dialog.preventConfirm(true);
            } else {
                dialog.content.find(".error-message").text("");
                dialog.preventConfirm(false);
            }
        }
    }

    this.fxSendOrder = function () {
        __this.checkOpenShift(function () {
            KPOS.sendOrder();
        });
    }

    this.fxBillOrder = function () {
        __this.checkOpenShift(function () {
            KPOS.billOrder();
        });
    }

    this.fxPayOrder = function (onPaid) {
        KPOS.payOrder(onPaid);
    }

    this.fxVoidOrder = function () {
        KPOS.voidOrder();
    }

    this.fxDiscountTotal = function () {
        KPOS.checkCart(function () {
            KPOS.checkPrivilege("P022", onCheckPrivilege);
            function onCheckPrivilege(posInfo) {
                $('.discount-total-erorr').text('');
                let dialog = new DialogBox({
                    position: "top-center",
                    caption: "Discount Order",
                    content: {
                        selector: "#content-discount-total",
                        class: 'login'
                    },
                    icon: "fas fa-percent fa-fw",
                    type: "ok-cancel"
                });

                dialog.confirm(function () {
                    var discount = dialog.content.find(".discount-total").val();
                    if (isNaN(discount)) {
                        dialog.content.find('.discount-total').val('');
                        dialog.content.find('.discount-total').focus();
                        $('.discount-total-erorr').text("Please input discount value ...!");
                    }
                    else {
                        if (parseFloat(discount) <= 100) {
                            posInfo.Order.DiscountRate = parseFloat(discount);
                            posInfo.Order.DiscountValue = posInfo.Order.Sub_Total * (parseFloat(discount) / 100);
                            KPOS.onSummaryOrderQuote(posInfo.Order);
                            dialog.shutdown();
                        }
                        else {
                            dialog.content.find('.discount-total').val('');
                            dialog.content.find('.discount-total').focus();
                            $('.discount-total-erorr').text("Discount can not be greater than 100%.");
                        }
                    }
                });

                dialog.reject(function () {
                    dialog.shutdown();
                });

                dialog.startup("after", function () {
                    dialog.content.find('.discount-total').focus();
                });
            }
        });

    }

    this.fxMoveTable = function () {
        KPOS.checkPrivilege("P001", function (posInfo) {
            if (posInfo.Order.OrderID <= 0) {
                KPOS.dialog('Move', '"' + posInfo.OrderTable.Name + '" cannot be moved.', 'warning');
                return;
            }

            KPOS.loadScreen();
            $.get("/POS/GetTableAvailable",
                { group_id: posInfo.OrderTable.GroupTableID, tableid: posInfo.OrderTable.ID },
                function (freeTables) {
                    let tableDialog = new DialogBox(
                        {
                            caption: "Move Table ( " + posInfo.OrderTable.Name + " )",
                            content: {
                                selector: "#move-table"
                            },
                            icon: "fas fa-arrow-alt-circle-right",
                            position: "top-center",
                            type: "ok-cancel",
                            button: {
                                ok: {
                                    text: "Move"
                                }
                            }
                        }
                    );

                    let $container = tableDialog.content.find("#move-table-list");
                    $container.html(createList(freeTables));

                    tableDialog.content.find("#search-move-table").on("keyup", function () {
                        var query = this.value.toLowerCase();
                        $.each($container.find("table tr td"), function (i, td) {
                            if (td.textContent.toLowerCase().indexOf(query) === - 1) {
                                $(td).parent().hide();
                            } else {
                                $(td).parent().show();
                            }
                        });

                    });

                    tableDialog.confirm(function () {
                        let $tables = tableDialog.content.find("input[name='table']");
                        let currentTableId = 0;
                        $.each($tables, function (i, table) {
                            if ($(table).prop("checked")) {
                                currentTableId = parseInt($(table).data("id"));
                            }
                        });

                        if (currentTableId > 0) {
                            KPOS.moveTable(posInfo.Order.TableID, currentTableId);
                            tableDialog.shutdown();
                        } else {
                            KPOS.dialog("Move Table Order",
                                "Please select any table to move.", "warning");
                        }
                    });

                    tableDialog.reject(function (e) {
                        tableDialog.shutdown();
                    });
                    KPOS.loadScreen(false);
                });
        });

        function createList(freeTables) {
            let template = "";
            let $table = $("<table></table>");
            for (let i = 0; i < freeTables.length; i++) {
                template = '<input name="table" data-id="' + freeTables[i].ID + '" type="radio"> '
                    + freeTables[i].Name;
                let row = $("<tr><td>" + template + "</td></tr>");
                $table.append(row);
            }
            return $table;
        }
    }

    this.fxDiscountItem = function (target = undefined) {
        __this.highlight(target);
        KPOS.checkPrivilege("P019", function (posInfo) {
            KPOS.checkCart(function () {
                let dialog = new DialogBox({
                    caption: "Discount Item",
                    type: "yes-no",
                    icon: "fas fa-percent",
                    content: {
                        selector: "#discount-item"
                    }
                });

                dialog.invoke(function () {
                    checkOverDiscount(dialog, posInfo.Order.OrderDetailQAutoMs);
                    var $listView = ViewTable({
                        selector: ".discount-item-listview",
                        keyField: "Line_ID",
                        visibleFields: ["Code", "KhmerName", "Qty", "UnitPrice", "Uom", "DiscountRate", "DiscountValue", "Total"],
                        paging: {
                            enabled: false
                        },
                        columns: [
                            {
                                name: "DiscountRate",
                                template: "<input class='number'>",
                                on: {
                                    "keyup": function (e) {
                                        discountOrderDetail(e.data, this.value);
                                        $listView.updateColumn(e.key, "DiscountValue", e.data.DiscountValue);
                                        $listView.updateColumn(e.key, "Total", e.data.Total.toFixed(3));
                                        checkOverDiscount(dialog, $listView.yield());
                                    }

                                }
                            }
                        ]
                    }).bindRows(posInfo.Order.OrderDetailQAutoMs)
                        .appendTo(dialog.content.find(".wrap-table"));
                    dialog.content.find("table input.number").asPositiveNumber();

                    dialog.confirm(function () {
                        posInfo.Order.OrderDetailQAutoMs = $listView.yield();
                        KPOS.updateOrder(posInfo.Order);
                        dialog.shutdown();
                        __this.highlight(target, false);
                    });
                    dialog.reject(function () {
                        dialog.shutdown();
                        __this.highlight(target, false);
                    });
                });
            });

        });

        function checkOverDiscount(dialog, items) {
            let isOverDiscount = false;
            $.grep(items, function (_item) {
                if (parseFloat(_item.DiscountRate) > 100) {
                    isOverDiscount = true;
                }
            });

            if (isOverDiscount) {
                dialog.content.find(".error-message").text("Discount rate cannot exceed 100%.");
                dialog.preventConfirm(true);
            } else {
                dialog.content.find(".error-message").text("");
                dialog.preventConfirm(false);
            }
        }

        function discountOrderDetail(item, inputValue) {
            if (inputValue == "") {
                inputValue = "0";
            }

            if (!Number.isNaN(inputValue)) {
                if (item.TypeDis === "Percent") {
                    item.DiscountRate = parseFloat(inputValue);
                }

                item.DiscountValue = parseFloat(item.Qty) * parseFloat(item.UnitPrice) * (parseFloat(inputValue) / 100);
                item.DiscountValue = parseFloat(item.DiscountValue.toFixed(3));
                item.Total = parseFloat(item.Qty) * parseFloat(item.UnitPrice) * (1 - (parseFloat(inputValue) / 100));
                item.Total = parseFloat(item.Total.toFixed(3));
            }
        }
    }


    this.fxCombineReceipts = function () {
        KPOS.checkPrivilege("P002", function (posInfo) {
            KPOS.checkCart(function () {
                KPOS.loadScreen();
                $.get("/POS/GetReceiptsToCombine", { orderId: posInfo.Order.OrderID }, function (receipts) {
                    if (receipts.length > 0) {
                        let dialog = new DialogBox({
                            caption: "Combine Receipt ( " + posInfo.Order.OrderNo + " )",
                            content: {
                                selector: "#combine-receipt"
                            },
                            icon: "fas fa-object-group",
                            type: "ok-cancel"
                        });

                        let $container = dialog.content.find("#combine-receipt-list");
                        $container.html(createList(receipts));
                        dialog.content.find("#search-combine-receipts").on("keyup", function () {
                            var query = this.value.toLowerCase();
                            $.each($container.find("table tr td"), function (i, td) {
                                if (td.textContent.toLowerCase().indexOf(query) === - 1) {
                                    $(td).parent().hide();
                                } else {
                                    $(td).parent().show();
                                }
                            });

                        });

                        dialog.confirm(function () {
                            KPOS.loadScreen();
                            var $receipts = dialog.content.find("input[name='receipt']");
                            let activeReceipts = [];
                            $.each($receipts, function (i, rec) {
                                if ($(rec).prop("checked")) {
                                    activeReceipts.push({
                                        OrderID: $(rec).data("id")
                                    });
                                }
                            });

                            let combinedReceipt = {
                                TableId: posInfo.Order.TableID,
                                OrderID: posInfo.Order.OrderID,
                                Receipts: activeReceipts
                            };

                            if (activeReceipts.length > 0) {
                                $.post("/POS/CombineReceipt",
                                    { combineReceipt: combinedReceipt },
                                    function () {
                                        KPOS.fetchOrders(posInfo.Order.TableID);
                                        KPOS.loadScreen(false);
                                    }
                                );
                                dialog.shutdown();
                            } else {
                                KPOS.dialog("Combine Receipts", "Please select any receipt to combine!", "warning");
                            }

                        });
                        dialog.reject(function () { dialog.shutdown(); });
                        KPOS.loadScreen(false);
                    } else {
                        KPOS.dialog("Combine Receipts", "No receipt to combine!", "warning");
                        KPOS.loadScreen(false);
                    }

                });
            });
        });

        function createList(receipts) {
            let template = "";
            let $table = $("<table></table>");
            for (let i = 0; i < receipts.length; i++) {
                template = '<input name="receipt" data-id="' + receipts[i].OrderID + '" type="radio"> '
                    + receipts[i].ReceiptNote;
                let row = $("<tr><td style='text-align: left;'>" + template + "</td></tr>");
                $table.append(row);
            }
            return $table;
        }
    }

    this.fxSplitReceipts = function () {
        KPOS.checkPrivilege("P003", function (posInfo) {
            if (isValidJSON(posInfo.Order) && posInfo.Order.OrderID > 0) {
                onCheckedOrder(posInfo.Order);
            } else {
                KPOS.dialog("Split Receipt Denied", "No receipts to split.", "warning");
            }
        });

        function onCheckedOrder(order) {
            var __orderSplit = {};
            __orderSplit = $.extend(true, __orderSplit, order);
            let dialog = new DialogBox(
                {
                    caption: "Split Receipt ( " + __orderSplit.OrderNo + " )",
                    content: {
                        selector: "#split-receipt-content"
                    },
                    icon: "fas fa-clone",
                    position: "top-center",
                    type: "ok-cancel"
                }
            );
            dialog.invoke(function () {
                var $listView = ViewTable({
                    selector: dialog.content.find("#split-receipt-listview"),
                    keyField: "Line_ID",
                    visibleFields: ["Code", "KhmerName", "Qty", "PrintQty", "Uom"],
                    paging: {
                        enabled: false
                    },
                    columns: [
                        {
                            name: "PrintQty",
                            template: "<input class='number'>",
                            on: {
                                "keyup": function (e) {
                                    e.data.PrintQty = parseFloat(this.value);
                                    checkOverQtySplit(dialog, __orderSplit.OrderDetail);
                                }
                            }
                        }
                    ],

                    actions: [
                        {
                            template: "<i class='fas fa-trash-alt csr-pointer fn-red'></i>",
                            on: {
                                "click": function (e) {
                                    $listView.removeRow(e.key);
                                    __orderSplit.OrderDetail = $.grep(__orderSplit.OrderDetail, function (item) {
                                        return item.Line_ID !== e.key;
                                    });
                                }
                            }
                        }
                    ]

                }).bindRows(__orderSplit.OrderDetail);
                dialog.confirm(function () {
                    $.grep(order.OrderDetail, function (item, i) {
                        $.grep(__orderSplit.OrderDetail, function (_item) {
                            if (item.Line_ID == _item.Line_ID) {
                                item.Qty -= _item["PrintQty"];
                                item.Total = KPOS.totalOrderDetail(item);
                            }
                        });
                    });

                    $.ajax({
                        url: "/POS/SendSplit",
                        type: "POST",
                        data: { data: order, addnew: __orderSplit },
                        success: function (orderSplit) {
                            if (orderSplit.OrderID != order.OrderID) {
                                KPOS.updateOrder(order);
                                KPOS.addNewOrder(orderSplit);
                            }
                        }
                    });
                    dialog.shutdown();
                });
                dialog.reject(function () { dialog.shutdown(); });
                dialog.content.find("table input.number").asPositiveNumber();
            });
        }

        function checkOverQtySplit(dialog, items) {
            let isOverQty = false;
            let isNegative = false;
            $.grep(items, function (_item) {
                if (parseFloat(_item.PrintQty < 0)) {
                    isNegative = true;
                }

                if (parseFloat(_item.PrintQty) > _item.Qty || parseFloat(_item.PrintQty < 0) < 0) {
                    isOverQty = true;
                }
            });

            if (isNegative) {
                dialog.content.find(".error-message").text("Split quantity cannot be negative.");
                dialog.preventConfirm(true);
            }

            if (isOverQty) {
                dialog.content.find(".error-message").text("Split quantity cannot exceed original quantity.");
                dialog.preventConfirm(true);
            } else {
                dialog.content.find(".error-message").text("");
                dialog.preventConfirm(false);
            }
        }
    }

    //Main menu sections
    this.fxReprint = function () {
        KPOS.checkPrivilege("P014", function (posInfo) {
            let $reprint = new DialogBox({
                position: "top-center",
                caption: "Reprint",
                content: {
                    selector: "#reprint-content"
                },
                button: {
                    ok: {
                        text: "Close",
                        callback: function (e) {
                            this.meta.shutdown();
                        }
                    }
                },
                icon: "fas fa-print"
            });

            $reprint.invoke(function () {
                let $listView = ViewTable({
                    selector: ".reprint-receipt-listview",
                    visibleFields: ["ReceiptNo", "Cashier", "DateOut", "TimeOut", "TableName", "GrandTotal"],
                    keyField: "ReceiptID",
                    paging: {
                        enabled: true,
                        pageSize: 6
                    },
                    actions: [
                        {
                            template: '<div class="btn"><i class="fas fa-print"></i></div>',
                            on: {
                                "click": function (e) {
                                    onReprint(e.key);
                                }
                            }
                        }
                    ]
                });

                let $dateFrom = $reprint.content.find(".reprint-date-from"),
                    $dateTo = $reprint.content.find(".reprint-date-to");
                $dateFrom[0].valueAsDate = new Date();
                $dateTo[0].valueAsDate = new Date();
                bindReprintReceipts($listView, $dateFrom.val(), $dateTo.val());
                $dateFrom.on("change", function () {
                    bindReprintReceipts($listView, this.value, $dateTo.val());
                });

                $dateTo.on("change", function () {
                    bindReprintReceipts($listView, $dateFrom.val(), this.value);
                });

                $reprint.content.find("#search-reprint-receipts").on("keyup", function () {
                    bindReprintReceipts($listView, $dateFrom.val(), $dateTo.val(), this.value);
                });
            });

            function bindReprintReceipts($listView, dateFrom, dateTo, keyword = "") {
                $.get("/POS/GetReceiptReprints", {
                    dateFrom: dateFrom,
                    dateTo: dateTo,
                    keyword: keyword
                }, function (receipts) {
                    $listView.clearRows();
                    $listView.bindRows(receipts);
                });
            };

            function onReprint(receiptId) {
                $.post("/POS/PrintReceiptReprint",
                    {
                        orderid: receiptId,
                        print_type: "Reprint"
                    });
            };
        });
    }

    this.checkOpenShift = function (onSucceed) {
        KPOS.checkCart(function (posInfo) {
            $.get("/POS/CheckOpenShift", function (hasOpenShift) {
                if (hasOpenShift) {
                    if (typeof onSucceed === "function") {
                        onSucceed(posInfo);
                    }
                } else {
                    let alert = KPOS.dialog("Check Open Shift", "You need to open shift before sending order, "
                        + "open shift now ? ", "warning", "yes-no");
                    alert.confirm(function () {
                        __this.fxOpenShift(onSucceed);
                        alert.shutdown();
                    });
                }
            });
        });
    }

    this.fxOpenShift = function (succeed) {
        KPOS.checkPrivilege("P012", function (posInfo) {
            startShiftForm("Cash In", "/POS/ProcessOpenShift", succeed);
        });
    }

    this.fxCloseShift = function (succeed) {
        KPOS.checkPrivilege("P013", function (posInfo) {
            startShiftForm("Cash Out", "/POS/ProcessCloseShift", succeed);
        });
    }

    function startShiftForm(caption, submitUrl, onSucceed) {
        let $shiftDialog = new DialogBox(
            {
                caption: caption,
                content: {
                    selector: "#shift-form-content"
                },
                position: "top-center",
                type: "ok-cancel",
                icon: "far fa-money-bill-alt",
                button: {
                    ok: {
                        text: "Done",
                        callback: function (e) {
                            this.meta.shutdown();
                        }
                    },
                    cancel: {
                        callback: function (e) {
                            this.meta.shutdown();
                        }
                    }
                }
            }
        );

        $shiftDialog.invoke(function () {
            $.get("/POS/GetShiftTemplate", function (resp) {
                if (resp) {
                    $shiftDialog.content.find(".c-symbol").text(resp.SystemCurrency);
                    ViewTable({
                        selector: "table.cash-box",
                        keyField: "ID",
                        visibleFields: ["Decription", "InputCash", "Currency"],
                        paging: {
                            enabled: false
                        },
                        columns: [
                            {
                                name: "InputCash",
                                template: "<input autofocus class='input-cash'>",
                                on: {
                                    "keyup": function (e) {
                                        let curr = resp.ShiftForms.find(w => w.ID == e.key);
                                        if (this.value === "") {
                                            this.value = "0";
                                        }
                                        curr.InputCash = parseFloat(this.value);
                                        curr.Amount = parseFloat(this.value) * parseFloat(curr.RateIn);
                                        $shiftDialog.content.find(".total-cash").text(sumTotal(resp.ShiftForms).toFixed(3));
                                    }
                                }
                            }
                        ]

                    }).bindRows(resp.ShiftForms);

                    $shiftDialog.confirm(function () {
                        $.post(submitUrl, { total: sumTotal(resp.ShiftForms).toFixed(3) },
                            function (model) {
                                if (model.Count > 0) {
                                    let d_setting = {
                                        caption: "Access Denied",
                                        icon: "warning",
                                        type: "ok"
                                    }

                                    let dialog = new DialogBox(d_setting);
                                    $(dialog.content)[0].textContent = "";
                                    ViewMessage({
                                        summary: {
                                            bordered: false
                                        }
                                    }).bind(model).appendTo(dialog.content);

                                } else {
                                    if (typeof onSucceed === "function") {
                                        onSucceed(sumTotal(resp.ShiftForms));
                                    }
                                    $shiftDialog.shutdown();
                                }
                            });
                    });
                    $shiftDialog.content.find("table .input-cash").asPositiveNumber();
                }

            });

        });

        function sumTotal(array) {
            for (
                var
                index = 0,              // The iterator
                length = array.length,  // Cache the array length
                sum = 0;                // The total amount
                index < length;         // The "for"-loop condition
                sum += array[index++].Amount  // Add number on each iteration
            );
            return sum;
        };
        return $shiftDialog;
    }

    //Start form change rate for payment
    this.fxChangeRate = function () {
        let info = KPOS.fallbackInfo();
        let orderId = isValidJSON(info.Order) ? info.Order.OrderID : 0;
        $.get("/POS/GetDisplayRateTemplate", { orderId: orderId }, function (resp) {
            let dialog = new DialogBox({
                caption: "Change Rate",
                icon: "fas fa-exchange-alt",
                content: {
                    selector: "#change-rate-content"
                },
                type: "ok-cancel",
                button: {
                    ok: {
                        text: "Save"
                    }
                }
            });

            dialog.invoke(function () {
                var viewTable = ViewTable({
                    keyField: "ID",
                    visibleFields: ["Name", "Currencies", "Rate"],
                    paging: {
                        enabled: false,
                    },
                    columns: [
                        {
                            name: "Currencies",
                            template: "<select>",
                            on: {
                                change: function (e) {
                                    e.data.DisplayCurrency.AltCurrencyID = parseInt(this.value);
                                }
                            }
                        },
                        {
                            name: "Rate",
                            template: "<input class='number'>",
                            on: {
                                keyup: function (e) {
                                    e.data.DisplayCurrency.DisplayRate = !isNaN(parseFloat(this.value)) ? parseFloat(this.value) : 0.000;
                                }
                            }
                        }
                    ],
                    selector: "#list-change-rate",
                }).bindRows(resp).appendTo(this.content.find("#change-rate-content"));
                dialog.confirm(function () {
                    let displayCurrencies = new Array();
                    $.each(viewTable.yield(), function (i, item) {
                        displayCurrencies.push(item.DisplayCurrency);
                    });
                    $.post("/KVMS/SaveDisplayCurrencies",
                        { displayCurrencies: displayCurrencies },
                        function (model) {
                            if (model.IsRejected) {
                                let msgDialog = new DialogBox();
                                msgDialog.content[0].textContent = "";
                                ViewMessage({
                                    summary: {
                                        bordered: false
                                    }
                                }).bind(model).appendTo(msgDialog.content);
                            } else {
                                location.reload();
                            }
                        });
                    dialog.shutdown();
                });

                dialog.reject(function () {
                    dialog.shutdown();
                });

                dialog.content.find("table input.number").asPositiveNumber();
            })
        });
    }

    //Edit item price
    this.fxEditPrice = function () {
        KPOS.checkPrivilege("P004", function (posInfo) {
            KPOS.checkCart(function () {
                let dialog = new DialogBox({
                    caption: "Edit Item Price",
                    type: "yes-no",
                    icon: "fas fa-edit",
                    content: {
                        selector: "#edit-item-price"
                    }
                });

                dialog.invoke(function () {
                    var $listView = ViewTable({
                        selector: "#edit-item-price-listview",
                        keyField: "Line_ID",
                        visibleFields: ["Code", "KhmerName", "Qty", "UnitPrice", "Uom", "DiscountRate", "Total"],
                        paging: {
                            enabled: false
                        },
                        columns: [
                            {
                                name: "KhmerName",
                                template: "<input>",
                                on: {
                                    "keyup": function (e) {
                                        e.data.KhmerName = this.value;
                                    }
                                }
                            },
                            {
                                name: "UnitPrice",
                                template: "<input class='number'>",
                                on: {
                                    "keyup": function (e) {
                                        updateUnitPrice(e.data, this.value);
                                        $listView.updateColumn(e.key, "Total", e.data.Total.toFixed(3));
                                    }

                                }
                            }
                        ]
                    }).bindRows(posInfo.Order.OrderDetailQAutoMs);
                    dialog.content.find("table input.number").asNumber();
                    dialog.confirm(function () {
                        posInfo.Order.OrderDetailQAutoMs = $listView.yield();
                        KPOS.updateOrder(posInfo.Order);
                        dialog.shutdown();
                    });
                    dialog.reject(function () { dialog.shutdown(); });
                });
            });

        });
    }

    function updateUnitPrice(item, inputValue) {
        if (inputValue == "") {
            inputValue = "0";
        }

        if (!Number.isNaN(inputValue)) {
            item.UnitPrice = parseFloat(inputValue);
            item.Total = parseFloat(item.Qty) * parseFloat(item.UnitPrice) * (1 - (parseFloat(item.DiscountRate) / 100));
            item.Total = parseFloat(item.Total.toFixed(3));

        }
    }

    //Start form pos setting
    //Start form pos setting
    this.fxSetting = function () {
        KPOS.checkPrivilege("P015", function (info) {
            KPOS.loadScreen();
            let dialog = new DialogBox({
                caption: "Setting",
                icon: "fas fa-cogs",
                type: "ok-cancel",
                content: {
                    selector: "#setting-content"
                },
                button: {
                    ok: {
                        text: "<i class='fas fa-save'></i> Save",
                        callback: function () {
                            saveSetting(this.meta.content, info.Setting, function (resp) {
                                KPOS.resetOrder(info.Order.TableID);
                                $(".customer-id").val(info.Setting.CustomerID);
                                $("#chosen-customer").text($(".customer-id option:selected")[0].textContent);
                                if (info.Setting.CustomerTips) {
                                    $("#wrapper-customer-tips").show();
                                }
                                else {
                                    $("#wrapper-customer-tips").hide();
                                }
                                dialog.shutdown();
                            });

                        }
                    }
                }
            });

            dialog.invoke(function () {
                initDataSetting(this.content, info.Setting);
                KPOS.loadScreen(false);
            });
            dialog.reject(function () {
                dialog.shutdown();
            });

            function saveSetting(content, setting, afterSave) {
                setting.PrintReceiptOrder = content.find(".print-receipt-order").prop("checked");
                setting.PrintLabel = content.find(".print-receipt-label").prop("checked");
                setting.PrintReceiptTender = content.find(".print-receipt-pay").prop("checked");
                setting.PrintCountReceipt = parseInt(content.find('.print-receipt-count').val());
                setting.QueueCount = parseInt(content.find('.max-queue-count').val());
                setting.Receiptsize = content.find(".receipt-size option:selected").text();
                setting.ReceiptTemplate = content.find('.receipt-template option:selected').text();
                setting.DaulScreen = content.find('.dual-screen').prop("checked");
                setting.VatAble = content.find('.vat-able').prop("checked");
                setting.VatNum = content.find('.vat-invoice-no').val();
                setting.Wifi = content.find('.password-wifi').val();
                setting.CustomerID = parseInt(content.find('.customer-id').val());
                setting.PriceListID = parseInt(content.find('.price-list-id').val());
                setting.WarehouseID = parseInt(content.find('.warehouse-id').val());
                setting.PaymentMeansID = parseInt(content.find('.payment-means-id').val());
                setting.CloseShift = parseInt(content.find('.close-shift').val());
                setting.AutoQueue = content.find('.autoqueue').prop("checked");
                setting.UserID = parseInt(content.find(".user-id").val());
                setting.ItemViewType = parseInt(content.find(".item-view-type option:selected").val());
                setting.ItemPageSize = parseInt(content.find(".item-page-size").val());
                setting.Printer = content.find(".printer option:selected").val();
                $.ajax({
                    url: '/POS/UpdateSetting',
                    type: 'POST',
                    data: { setting: setting },
                    success: afterSave
                });
            }

            function initDataSetting(content, setting) {
                content.find('.print-receipt-order').prop('checked', setting.PrintReceiptOrder);
                content.find('.print-receipt-label').prop('checked', setting.PrintLabel);
                content.find('.print-receipt-pay').prop('checked', setting.PrintReceiptTender);
                content.find('.print-receipt-count').val(setting.PrintCountReceipt);
                content.find('.autoqueue').val(setting.QueueCount);
                content.find('.max-queue-count').val(setting.QueueCount);
                content.find(".receipt-size").val(setting.Receiptsize);
                content.find(".receipt-template").val(setting.ReceiptTemplate);
                content.find('.dual-screen').prop('checked', setting.DaulScreen);
                content.find('.vat-able').prop('checked', setting.VatAble);
                content.find('.vat-invoice-no').val(setting.VatNum);
                content.find('.password-wifi').val(setting.Wifi);
                content.find('.customer-id').val(setting.CustomerID);
                content.find('.price-list-id').val(setting.PriceListID);
                content.find('.warehouse-id').val(setting.WarehouseID);
                content.find('.payment-means-id').val(setting.PaymentMeansID);
                content.find('.close-shift').val(setting.CloseShift);
                content.find(".autoqueue").prop("checked", setting.AutoQueue);
                content.find(".item-view-type").val(setting.ItemViewType);
                content.find(".item-page-size").val(setting.ItemPageSize);
                content.find(".printer").val(setting.Printer);
            }
        });
    }
    //this.fxSetting = function () {
    //    KPOS.checkPrivilege("P015", function () {
    //        KPOS.loadScreen();
    //        $.get("/POS/GeneralSetting", { json: true }, function (setting) {
    //            let dialog = new DialogBox({
    //                caption: "Setting",
    //                icon: "fas fa-cogs",
    //                content: {
    //                    selector: "#setting-content"
    //                },
    //                button: {
    //                    ok: {
    //                        text: "<i class='fas fa-save'></i> Save",
    //                        callback: function () {
    //                            saveSetting(this.meta.content, setting, function (resp) {
    //                                dialog.shutdown("before", function () {
    //                                    window.location.reload();
    //                                });
    //                            });

    //                        }
    //                    }
    //                }
    //            });

    //            dialog.invoke(function () {
    //                initDataSetting(this.content, setting);
    //                KPOS.loadScreen(false);
    //            });
    //        });

    //        function saveSetting(content, setting, afterSave) {
    //            setting.PrintReceiptOrder = content.find(".print-receipt-order").prop("checked");
    //            setting.PrintLabel = content.find(".print-receipt-label").prop("checked");
    //            setting.PrintReceiptTender = content.find(".print-receipt-pay").prop("checked");
    //            setting.PrintCountReceipt = parseInt(content.find('.print-receipt-count').val());
    //            setting.QueueCount = parseInt(content.find('.max-queue-count').val());
    //            setting.Receiptsize = content.find(".receipt-size option:selected").text();
    //            setting.ReceiptTemplate = content.find('.receipt-template option:selected').text();
    //            setting.DaulScreen = content.find('.dual-screen').prop("checked");
    //            setting.VatAble = content.find('.vat-able').prop("checked");
    //            setting.VatNum = content.find('.vat-invoice-no').val();
    //            setting.Wifi = content.find('.password-wifi').val();
    //            setting.CustomerID = parseInt(content.find('.customer-id').val());
    //            setting.PriceListID = parseInt(content.find('.price-list-id').val());
    //            setting.WarehouseID = parseInt(content.find('.warehouse-id').val());
    //            setting.PaymentMeansID = parseInt(content.find('.payment-means-id').val());
    //            setting.CloseShift = parseInt(content.find('.close-shift').val());
    //            setting.AutoQueue = content.find('.autoqueue').prop("checked");
    //            setting.UserID = parseInt(content.find(".user-id").val());
    //            setting.ItemViewType = parseInt(content.find(".item-view-type option:selected").val());
    //            setting.ItemPageSize = parseInt(content.find(".item-page-size").val());
    //            setting.Printer = content.find(".printer option:selected").val();
    //            $.ajax({
    //                url: '/POS/UpdateSetting',
    //                type: 'POST',
    //                data: { setting: setting },
    //                success: afterSave
    //            });
    //        }

    //        function initDataSetting(content, setting) {
    //            content.find('.print-receipt-order').prop('checked', setting.PrintReceiptOrder);
    //            content.find('.print-receipt-label').prop('checked', setting.PrintLabel);
    //            content.find('.print-receipt-pay').prop('checked', setting.PrintReceiptTender);
    //            content.find('.print-receipt-count').val(setting.PrintCountReceipt);
    //            content.find('.autoqueue').val(setting.QueueCount);
    //            content.find('.max-queue-count').val(setting.QueueCount);
    //            content.find(".receipt-size").val(setting.Receiptsize);
    //            content.find(".receipt-template").val(setting.ReceiptTemplate);
    //            content.find('.dual-screen').prop('checked', setting.DaulScreen);
    //            content.find('.vat-able').prop('checked', setting.VatAble);
    //            content.find('.vat-invoice-no').val(setting.VatNum);
    //            content.find('.password-wifi').val(setting.Wifi);
    //            content.find('.customer-id').val(setting.CustomerID);
    //            content.find('.price-list-id').val(setting.PriceListID);
    //            content.find('.warehouse-id').val(setting.WarehouseID);
    //            content.find('.payment-means-id').val(setting.PaymentMeansID);
    //            content.find('.close-shift').val(setting.CloseShift);
    //            content.find(".autoqueue").prop("checked", setting.AutoQueue);
    //            content.find(".item-view-type").val(setting.ItemViewType);
    //            content.find(".item-page-size").val(setting.ItemPageSize);
    //            content.find(".printer").val(setting.Printer);
    //        }
    //    });    
    //}
}
