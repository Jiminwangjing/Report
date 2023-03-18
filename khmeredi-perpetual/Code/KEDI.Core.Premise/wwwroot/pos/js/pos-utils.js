"use strict";
function PosUtils(__poscore) {
    if (!(this instanceof PosUtils)) {
        return new PosUtils(__poscore);
    }
    const _printUrlPOS = JSON.parse($("#print-template-url").text()).Templateurl;
    var returnamount = 0;
    const __this = this;
    var __array = [];
    var __config = __poscore.config(),
        __orderInfo = {},
        __dataSB = {
            serials: [],
            batches: []
        },
        __urls = {
            getCustomers: "/POS/GetCustomers",
            getMembercards: "/POS/GetMemberCards",
            getLineItemUnknown: "/POS/GetOrderDetailUnknown",
            getReturnItems: "/POS/GetReturnItems",
            getReceiptsToCancel: "/POS/GetReceiptsToCancel",
            cancelReceipt: "/POS/CancelReceipt",
            sendReturnItems: "/POS/SendReturnItems",
            getReturnReceipts: "/POS/GetReturnReceipts",
            getOtherTables: "/POS/GetOtherTables",
            getFreeTables: "/POS/GetFreeTables",
            getOrdersToCombine: "/POS/GetOrdersToCombine",
            combineOrders: "/POS/CombineOrders",
            sendSplitOrder: "/POS/SplitOrder",
            getReceiptsToReprint: "/POS/GetReceiptsToReprint",
            getPendingVoidItem: "/POS/GetPendingVoidItem",
            reprintReceipt: "/POS/ReprintReceipt",
            saveOrder: "/POS/SaveOrder",
            deleteSavedOrder: "/POS/DeleteSavedOrder",
            getKSServices: "/POS/GetKSServices",
            getVehicles: "/POS/GetVehicles",
            GetReprintcloseshift: "/POS/ReprintCloseShift",
            reprintcloseshift: "/POS/ReprintReceiptcloseshift",
            submitPendingVoidItem: "/POS/SubmitPendingVoidItem",
            getCardMemberDetial: "/POS/GetMemberCardDiscount",
            deletePendingVoidItem: "/POS/DeletePendingVoidItem",
            processOpenShift: "/POS/ProcessOpenShift",
         
        }

    this.load = function (callback) {
        if (typeof callback === "function") {
            callback.call(this, __poscore);
        }
    }

    __poscore.load(function (orderInfo) {
        __orderInfo = orderInfo;
    });

    function isValidJSON(value) {
        return !isEmpty(value) && value.constructor === Object && Object.keys(value).length > 0;
    }

    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }

    function isEmpty(value) {
        return value == undefined || value == null || value == "";
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

    this.fxClearOrder = function (target = undefined) {
        __poscore.clearOrder2ndScreen();
        __poscore.loadCurrentOrderInfo(0, 0, 0, true);
    }

    $("#customer-count").click(function (e) {
        __this.fxCustomerCount();
    });
    this.fxCustomerCount = function () {
        __poscore.checkPrivilege("P027", function (posInfo) {
            const order = posInfo.Order;
            let dialog = new DialogBox({
                caption: "Number of customers",
                icon: "fas fa-user-friends",
                content: {
                    selector: "#choose-customer-count"
                },
                type: 'ok-cancel',
                button: {
                    ok: {
                        text: "Save"
                    }
                }
            });
            dialog.invoke(function () {
                dialog.content.find(".male-count").val(order.Male);
                dialog.content.find(".female-count").val(order.Female);
                dialog.content.find(".children-count").val(order.Children);
                dialog.confirm(function () {
                    order.Male = dialog.content.find(".male-count").val();
                    order.Female = dialog.content.find(".female-count").val();
                    order.Children = dialog.content.find(".children-count").val();
                    dialog.shutdown();
                });
                dialog.reject(function () {
                    dialog.shutdown();
                });
            });

        })
    }
    this.fxChooseCustomer = function (isKsms = false) {
        __poscore.checkPrivilege("P018", function (posInfo) {
            let dialog = new DialogBox({
                caption: "Customer Information",
                icon: "fas fa-user-friends",
                content: {
                    selector: "#choose-customer-content"
                },
                button: {
                    ok: {
                        class: "bg-red",
                        text: "Close"
                    }
                }
            });

            dialog.invoke(function () {
                var _form = dialog.content.find("#form-customer");
                let __vmclist = addVehicles();
                let __vehicles = ViewTable({
                    selector: "#form-vehicles",
                    keyField: "KeyID",
                    model: {
                        AutoMobiles: []
                    }
                });
                let $listView = ViewTable({
                    keyField: "ID",
                    indexed: true,
                    visibleFields: ["Code", "Name", "Type", "Phone", "Email"],
                    selector: "#listview-customers",
                    paging: {
                        pageSize: 20
                    },
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>",
                            on: {
                                "click": function (e) {
                                    dialog.content.find(".error-message").text("");

                                    if (posInfo.Order.CustomerID != e.data.ID) {
                                        __poscore.updateOrderCustomer(e.data, false);
                                        dialog.shutdown();
                                        if (isKsms) {
                                            __this.fxVehicleDialog(e.data.ID, posInfo);
                                        }

                                        // update fix customer
                                        if (posInfo.Setting.RememberCustomer) {
                                            posInfo.Setting.CustomerID = e.key;
                                            __poscore.saveUserSetting(posInfo.Setting);
                                        }
                                        // update price list
                                        const { TableID, CustomerID, OrderID } = posInfo.Order;
                                        if (posInfo.Setting.IsCusPriceList) __poscore.fetchOrderInfo(TableID, OrderID, CustomerID, false);
                                        dialog.shutdown();
                                    }
                                }
                            }
                        }
                    ]
                });

                $("#choose-glacc").on("click", function () {
                    let dialog = new DialogBox({
                        button: {
                            ok: {
                                text: "Close",
                                callback: function () {
                                    this.meta.shutdown();
                                }
                            }
                        },
                        caption: 'GL/Account',
                        content: {
                            selector: "#controlAc-gl-content"
                        }
                    });

                    dialog.invoke(function () {
                        let $listControlGL = ViewTable({
                            keyField: "ID",
                            indexed: true,
                            selector: dialog.content.find("#list-controlAc-gl"),
                            paging: {
                                pageSize: 10,
                                enabled: true
                            },
                            visibleFields: ["Code", "Name"],
                            actions: [
                                {
                                    template: "<i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>",
                                    on: {
                                        "click": function (e) {
                                            $(".glaccNumber").val(`${e.data.Code}   ${e.data.Name}`);
                                            $(".glaccId").val(e.data.ID);
                                            dialog.shutdown();
                                        }
                                    }
                                }
                            ]
                        });
                        $.get("/bussinesspartner/glcontrolaccount", function (resp) {
                            $listControlGL.bindRows(resp);
                            $("#search-customers-glacc").on("keyup", function (e) {
                                let __value = this.value.toLowerCase().replace(/\s/g, "");
                                let rex = new RegExp(__value, "i");
                                let __glaccs = $.grep(resp, function (person) {
                                    return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s/g, "").match(rex);
                                });
                                $listControlGL.bindRows(__glaccs);
                            });
                        });
                    });
                });

                resetFormCustomer(_form);
                searchCustomers($listView);
                $("#search-customers").on("keyup", function () {
                    searchCustomers($listView, this.value);
                });

                dialog.content.find("#save-customer").on("click", function () {
                    __vehicles.bindRows(__vmclist.yield());
                    __poscore.dialog("Create Customer", "Confirm create new customer?", "info", "ok-cancel")
                        .confirm(function () {
                            saveCustomer(_form, $listView);
                            this.meta.shutdown();
                        });

                });
            });

            dialog.confirm(function () {
                dialog.shutdown();
            });
        });

        function saveCustomer(form, $list) {
            $.ajax({
                url: "/POS/SaveCustomer",
                type: "POST",
                data: $(form).serialize(),
                success: function (message) {
                    var customer = message.Items["Customer"];
                    if (message.IsApproved && customer.ID > 0) {
                        $list.addRow(customer);
                        $list.reload(customer);
                        resetFormCustomer(form);
                        __poscore.dialog("Create Customer", message.Data["__success"]);
                    }
                    ViewMessage({}, message);
                }
            });
        }

        function resetFormCustomer(form) {
            $(form).find("[name='Customer.ID']").val(0);
            $(form).find("[name='Customer.Code']").val("");
            $(form).find("[name='Customer.Name']").val("");
            $(form).find("[name='Customer.Phone']").val("");
            $(form).find("[name='Customer.Email']").val("");
            $(form).find("[name='Customer.Address']").val("");
        }

        function searchCustomers($listView, keyword = "") {
            $.post(__urls.getCustomers, {
                keyword: keyword
            }, function (customers) {
                $listView.bindRows(customers);
            });
        }
    }

    this.fxVehicleDialog = function () {
        __poscore.getOrderInfo(function (orderInfo) {
            var id = orderInfo.Order.CustomerID;
            let dialog = new DialogBox({
                caption: "Information of Vehicles",
                icon: "fas fa-user-friends",
                content: {
                    selector: "#choose-vehicle-content"
                },
                button: {
                    ok: {
                        class: "bg-red",
                        text: "Close"
                    }
                }
            });

            dialog.invoke(function () {
                let $listVehicleView = ViewTable({
                    keyField: "ID",
                    indexed: true,
                    visibleFields: ["Plate", "Frame", "Type", "Engine", "Brand", "Model", "Year", "Color"],
                    selector: "#listview-vehicles",
                    paging: {
                        pageSize: 7
                    },
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>",
                            on: {
                                "click": function (e) {
                                    orderInfo.Order.VehicleID = e.data.ID;
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                $.get(__urls.getVehicles, { cusId: id }, function (res) {
                    $listVehicleView.clearRows();
                    $listVehicleView.bindRows(res);
                    $("#search-vehicle").on("keyup", function () {
                        let input = $(this).val().replace(/\s/g, '');
                        let regex = new RegExp(input, "i");
                        const items = $.grep(res, function (item) {
                            return regex.test(item.Plate) || regex.test(item.Frame) || regex.test(item.Engine) || regex.test(item.Type)
                                || regex.test(item.Brand) || regex.test(item.Model) || regex.test(item.Year) || regex.test(item.Color)
                        });
                        $listVehicleView.clearRows();
                        $listVehicleView.bindRows(items);
                    });
                })
            });

            dialog.confirm(function () {
                dialog.shutdown();
            });
        });
    }
    //reprint close shift
    this.fxReprintCloseShift = function () {
        __poscore.checkPrivilege("P014", function (posInfo) {
            let $reprint = new DialogBox({
                position: "top-center",
                caption: "Reprint Close Shift",
                content: {
                    selector: "#reprint-closeshift"
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
                    selector: ".reprint-closeshift-listview",
                    visibleFields: ["EmpName", "Trans", "DateIn", "TimeIn", "DateOut", "TimeOut", "CashInAmountSys", "SaleAmountSys", "TotalCashOutSys", "CashOutAmountSys"],
                    keyField: "ID",
                    paging: {
                        enabled: true,
                        pageSize: 6
                    },

                    actions: [
                        {
                            template: '<div class="btn"><i class="fas fa-print"></i></div>',
                            on: {
                                "click": function (e) {
                                    onReprint(e.data);
                                }
                            }
                        }
                    ]
                });

                let $dateFrom = $reprint.content.find(".closeshift-date-from"),
                    $dateTo = $reprint.content.find(".closeshift-date-to");
                $dateFrom[0].valueAsDate = new Date();
                $dateTo[0].valueAsDate = new Date();
                bindreprintcloseshift($listView, $dateFrom.val(), $dateTo.val());
                $dateFrom.on("change", function () {
                    bindreprintcloseshift($listView, this.value, $dateTo.val());
                });

                $dateTo.on("change", function () {
                    bindreprintcloseshift($listView, $dateFrom.val(), this.value);
                });

                $reprint.content.find("#search-reprint-closeshift").on("keyup", function () {
                    bindreprintcloseshift($listView, $dateFrom.val(), $dateTo.val(), this.value);
                });
            });

            function bindreprintcloseshift($listView, dateFrom, dateTo, keyword = "") {
                $.post(__urls.GetReprintcloseshift, {
                    dateFrom: dateFrom,
                    dateTo: dateTo,
                    keyword: keyword
                }, function (receipts) {
                    $listView.clearRows();
                    $listView.bindRows(receipts);
                });
            };

            function onReprint(data) {
                $.post(__urls.reprintcloseshift,
                    {
                        userid: data.UserID,
                        closeShiftId: data.ID,
                    });
            };
        });


    }

    $("#ReprintCloseShift").click(function (e) {
        __this.fxReprintCloseShift();
    });
    this.fxDiscountMember = function (target = undefined) {
        __this.highlight(target);
        __poscore.checkPrivilege("P006", function (posInfo) {
            let dialog = new DialogBox({
                caption: "Discount Member",
                icon: "fas fa-user-friends",
                content: {
                    selector: "#discount-membercard-content"
                },
                button: {
                    ok: {
                        text: "Close"
                    }
                },
            });

            dialog.invoke(function () {
                __poscore.readyPayment = false;
                const cardElement = dialog.content.find("#card-number-discount");
                if (posInfo.CardMemberNumberDisCount) {
                    cardElement.val(posInfo.CardMemberNumberDisCount);
                }
                if (posInfo.CardMemberOption == 1) { /// Scan Card Member                      
                    initScanCardMember(posInfo, dialog);
                }
                if (posInfo.CardMemberOption == 0) { /// Manual input card member number
                    initManualCardMember(cardElement, posInfo, dialog);
                }
                dialog.confirm(function () {

                });
            });

            dialog.confirm(function () {
                dialog.shutdown();
                __this.highlight(target, false);
            });

        });

        function initScanCardMember(orderInfo, dialog) {
            $("#form-scan-card-discount").prop("hidden", false);
            $("#form-card-manual-discount").prop("hidden", true);
            var $accessToken = dialog.content.find("#scan-card-number-discount");
            $(dialog.content.parent()).on("click", function () {
                $accessToken.focus();
                dialog.content.find("#form-scan-card-discount").addClass("frame-active");
            });

            setTimeout(function () {
                $accessToken.focus();
                dialog.content.find("#form-scan-card-discount").addClass("frame-active");
            }, 250);
            $accessToken.on("focusout", function () {
                dialog.content.find("#form-scan-card-discount").removeClass("frame-active");
            });

            $accessToken.on("keyup", function (e) {
                if (e.which === 13) {
                    $.post(
                        __urls.getCardMemberDetial,
                        {
                            cardNumber: this.value.trim(),
                            pricelistId: orderInfo.Setting.PriceListID
                        },
                        function (res) {
                            $accessToken.focus().val("");
                            responseCardMemberPay(res, orderInfo, dialog);
                        })
                }
            });
        }
        function initManualCardMember(cardElement, orderInfo, dialog) {
            $("#form-scan-card-discount").prop("hidden", true);
            $("#form-card-manual-discount").prop("hidden", false);
            cardElement.on("keyup", function (e) {
                if (e.which === 13) {
                    $.post(
                        __urls.getCardMemberDetial,
                        {
                            cardNumber: this.value.trim(),
                            pricelistId: orderInfo.Setting.PriceListID
                        },
                        function (res) {
                            responseCardMemberPay(res, orderInfo, dialog);
                        })
                }
            })
        }
        function responseCardMemberPay(res, orderInfo, dialog) {
            const TYPEDISCOUNT = {
                RATE: 0,
                VALUE: 1
            };
            ViewMessage({
                summary: {
                    selector: ".manual-card-discount-message",
                    bordered: false
                }
            }, res);
            if (res.IsApproved) {
                const { data } = res.Items;
                __poscore.updateOrderCustomer(data.Customer, false);

                if (data.TypeDiscount == TYPEDISCOUNT.RATE) {
                    orderInfo.Order.CardMemberDiscountRate = data.Discount;
                    orderInfo.Order.CardMemberDiscountValue = data.Discount * orderInfo.Order.Sub_Total / 100;
                }
                if (data.TypeDiscount == TYPEDISCOUNT.VALUE) {
                    let reqAmount = data.Discount - orderInfo.Order.Sub_Total;
                    if (orderInfo.Order.Sub_Total < data.Discount) {
                        __poscore.dialog("Over Discount", "Required amount at least "
                            + orderInfo.Order.Currency.Description + " " + reqAmount + " more to use the member card discount.", "warning");
                        return;
                    }

                    orderInfo.Order.CardMemberDiscountRate = orderInfo.Order.Sub_Total == 0 ? 0 : data.Discount * 100 / orderInfo.Order.Sub_Total;
                    orderInfo.Order.CardMemberDiscountValue = data.Discount;
                }
                if (isValidArray(orderInfo.Order.OrderDetail)) {
                    orderInfo.Order.OrderDetail.forEach(i => {
                        __poscore.sumLineItem(i);
                    })
                }
                __poscore.summarizeOrder(orderInfo.Order);
                setTimeout(function () {
                    $(".manual-card-discount-message").text("")
                    dialog.shutdown();
                }, 500);
            }
        }
    }


    this.fxAddNewItem = function (target = undefined) {
        __this.highlight(target);
        __poscore.fallbackInfo(function (posInfo) {
            $.post(__urls.getLineItemUnknown, { orderId: posInfo.Order.OrderID }, function (lineItemUnknown) {
                let unknownItem = $.extend(true, {}, lineItemUnknown);
                if (unknownItem.ItemID <= 0) {
                    new DialogBox(
                        {
                            caption: "Information",
                            content: "Please add default item follow by Item name 1 is 'unknown' and process type Standard",
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
                        let __order = $.extend(true, {}, posInfo.Order);
                        var $listView = ViewTable({
                            selector: "#add-new-item-listview",
                            keyField: "LineID",
                            visibleFields: ["Code", "KhmerName", "EnglishName", "Qty", "UnitPrice", "Total", "Printers"],
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
                                    name: "EnglishName",
                                    template: "<input>",
                                    on: {
                                        "keyup": function (e) {
                                            e.data.EnglishName = this.value;
                                        }
                                    }
                                },
                                {
                                    name: "Code",
                                    template: "<input>",
                                    on: {
                                        "keyup": function (e) {
                                            e.data.Code = this.value;
                                        }
                                    }
                                },
                                {
                                    name: "UnitPrice",
                                    template: "<input class='number'>",
                                    on: {
                                        "keyup": function (e) {
                                            let unitPrice = __poscore.toNumber(this.value);
                                            e.data.UnitPrice = unitPrice;
                                            __poscore.sumLineItem(e.data);
                                            $listView.updateColumn(e.key, "Total", __poscore.toCurrency(e.data.Total));
                                        }
                                    }
                                },
                                {
                                    name: "Printers",
                                    template: "<select>",
                                    on: {
                                        "change": function (e) {
                                            let _selectText = $(this).find("option:selected").text();
                                            $listView.updateColumn(e.key, "ItemPrintTo", _selectText);
                                        }

                                    }
                                }
                            ]
                        });

                        addItemTemplate(unknownItem, $listView);
                        dialog.content.find("#add-new-row").on("click", function () {
                            addItemTemplate(unknownItem, $listView);
                            dialog.content.find("table input.number").asPositiveNumber();
                        });
                        dialog.content.find("table input.number").asPositiveNumber();
                        dialog.confirm(function () {
                            let newItems = $listView.yield();
                            __poscore.addLineItems(newItems);
                            dialog.shutdown();
                            __this.highlight(target, false);
                        });
                    });

                    dialog.reject(function () {
                        dialog.shutdown();
                        __this.highlight(target, false);
                    });
                }
            });

            function addItemTemplate(unknownItem, $listView) {
                let lineId = Date.now().toString();
                let __newItem = {}, __unknownItem = {};
                __newItem = $.extend(__newItem, unknownItem);
                __unknownItem = $.extend(true, {}, unknownItem);
                __newItem.LineID = lineId;
                __unknownItem.LineID = __newItem.LineID;
                $listView.updateRow(__newItem);
            }
        });
    }
    $.get("/POS/GetPaymean", function (res) {

        let list = "";
        res.forEach(i => {
            if (i.Default == true) {
                list += `<option value="${i.ID}" selected>${i.Name}</option>`;
            }
            else {
                list += `<option value="${i.ID}">${i.Name}</option>`;
            }
        });

        $("#list-multipay").html(list);
        $("#list-multipay-cancele").html(list);
    });
   
    this.fxCancelReceipts = function (target = undefined) {
        __this.highlight(target);
        __poscore.checkPrivilege("P020", function () {
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
                                "click": function (e, e2) {
                                    __poscore.checkOpenShift(function(){           
                                        if (!e2.detail || e2.detail == 1) { //Apply first click only
                                            cancelReceipt(e.data, $listView);
                                        }
                                    }) 
                                   
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
            $.post(__urls.getReceiptsToCancel,
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
            let isConfirmed = false;
            if (isConfirmed) { return; }
            var dialog = __poscore.dialog("Cancel Receipt", "Are you sure you want to cancel receipt '" + receipt.ReceiptNo + "'?", "warning", "yes-no")
                .confirm(function () {
                    $(".cancel-receipt-reason").val("");
                    let $dialogReason = new DialogBox({
                        content: {
                            selector: "#reason-cancel-receipt-content",
                        },
                        caption: "Reason",
                        type: "ok-cancel"
                    });
                    $dialogReason.confirm(function () {
                        var reason;
                        $dialogReason.content.find(".cancel-receipt-reason").val();
                        reason = $(".cancel-receipt-reason").val();

                        if (reason == "" || reason == undefined || isEmpty(reason)) {
                            $dialogReason.content.find(".cancel-receipt-error").text("Reason is required.");
                        } else {
                            __poscore.loadScreen();
                            const checkingSerialString = $("#checkingSerialString").val();
                            const checkingBatchString = $("#checkingBatchString").val();
                            $.ajax({
                                data: {
                                    receiptId: receipt.ReceiptID,
                                    serial: JSON.stringify(__dataSB.serials),
                                    batch: JSON.stringify(__dataSB.batches),
                                    checkingSerialString,
                                    checkingBatchString,
                                    PaymentmeansID: $("#list-multipay-cancele").val(),
                                    reason: reason,
                                },
                                url: __urls.cancelReceipt,
                                method: "POST",
                                success: function (message) {
                                    __poscore.loadScreen(false);
                                    isConfirmed = false;
                                    if (message.IsSerial) {
                                        const serial = SerialTemplate({
                                            isReturn: true,
                                            data: {
                                                serials: message.Data,
                                            }
                                        });
                                        serial.serialTemplate();
                                        const seba = serial.callbackInfo();
                                        __dataSB.serials = seba.serials;
                                    } else if (message.IsBatch) {
                                        const batch = BatchNoTemplate({
                                            isReturn: true,
                                            data: {
                                                batches: message.Data,
                                            }
                                        });
                                        batch.batchTemplate();
                                        const seba = batch.callbackInfo();
                                        __dataSB.batches = seba.batches;
                                    } else if (!message.IsRejected) {
                                        __dataSB.serials = [];
                                        __dataSB.batches = [];
                                        $("#checkingSerialString").val("unchecked");
                                        $("#checkingBatchString").val("unchecked");
                                        $listView.removeRow(receipt.ReceiptID);
                                        dialog.shutdown();
                                    }
                                    ViewMessage({
                                        summary: {
                                            bordered: false
                                        }
                                    }).bind(message);

                                },
                                error: function (resp) {
                                    isConfirmed = false;
                                }
                            });
                           
                             $dialogReason.shutdown();
                        }
                        __this.highlight(target, false);
                        __array.length = 0;
                    });
                    $dialogReason.reject(function () {
                        $(".cancel-receipt-reason").val("");
                        __array.length = 0;
                        $dialogReason.shutdown();
                    });
                    //========================Void Reason========================

                    $("#btn_addvoidreson").click(function () {
                        $(".reason").focus();
                        dataobj.ID = 0;
                        dataobj.Reason = $(".reason").val();
                        $.ajax({
                            url: "/POS/AddVoidReason",
                            type: "POST",
                            dataType: "JSON",
                            data: { data: dataobj },
                            success: function (respones) {
                                $(".reason").val("");
                                $.ajax({
                                    url: "/POS/GetVoidReason",
                                    type: "get",
                                    dataType: "JSON",
                                    success: function (data) {
                                        CancelReceiptReason(data)
                                    }
                                });
                                if (respones.IsApproved) {
                                    new ViewMessage({
                                        summary: {
                                            selector: "#error-summary"
                                        },
                                    }, respones);
                                } else {
                                    new ViewMessage({
                                        summary: {
                                            selector: "#error-summary"
                                        }
                                    }, respones);
                                }
                            }
                        });
                        // dialog.preventConfirm(true);
                    });
                    $("#btn_addnewvoidreson").click(function () {
                        $("#btn_addvoidreson").show();
                        $("#btn_updatevoidreson").css("visibility", "hidden");
                        $("#btn_addnewvoidreson").css("visibility", "hidden");
                        $(".reason").css("width", "85%")
                        $(".reason").focus();
                    })
                    const dataobj = {
                        ID: 0,
                        Reason: "",
                        Delete: false
                    }
                    $("#btn_updatevoidreson").click(function () {
                        $(".reason").focus();
                        dataobj.ID = parseInt($(".idreason").val());
                        dataobj.Reason = $(".reason").val();
                        $.ajax({
                            url: "/POS/AddVoidReason",
                            type: "POST",
                            dataType: "JSON",
                            data: { data: dataobj },
                            success: function (respones) {
                                $(".reason").val("");
                                $.ajax({
                                    url: "/POS/GetVoidReason",
                                    type: "get",
                                    dataType: "JSON",
                                    success: function (data) {
                                        CancelReceiptReason(data)
                                    }
                                });
                                if (respones.IsApproved) {
                                    new ViewMessage({
                                        summary: {
                                            selector: "#error-summary"
                                        },
                                    }, respones);
                                } else {
                                    new ViewMessage({
                                        summary: {
                                            selector: "#error-summary"
                                        }
                                    }, respones);
                                }
                            }
                        });
                        // dialog.preventConfirm(true);
                    });
                    $("#void-item-listview").val("");
                    $.ajax({
                        url: "/POS/GetVoidReason",
                        type: "get",
                        dataType: "JSON",
                        success: function (data) {
                            CancelReceiptReason(data)
                        }
                    });
                    $(".cancel-receipt-reason").html("");
                    
                      
                });
        }

    }


    this.fxReturnReceipts = function (target = undefined) {

        __this.highlight(target);
        __poscore.checkPrivilege("P021", function (posInfo) {
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
                    }
                }
            });

            dialog.invoke(function () {
                let $dateFrom = dialog.content.find(".return-date-from"),
                    $dateTo = dialog.content.find(".return-date-to");
                $dateFrom[0].valueAsDate = new Date();
                $dateTo[0].valueAsDate = new Date();
                let $receipts = ViewTable({
                    selector: ".listview-return-receipts",
                    keyField: "ReceiptID",
                    visibleFields: ["ReceiptNo"],
                    paging: {
                        pageSize: 4
                    },
                    columns: [
                        {
                            name: "ReceiptNo",
                            on: {
                                "click": function (e) {

                                    onGetReceiptDetails(e.data.ReturnItems);
                                }
                            }
                        }
                    ]
                });

                let $receiptDetail = ViewTable({
                    selector: ".item-return-choosed-listview",
                    keyField: "ID",
                    visibleFields: ["Code", "KhName", "UoM", "OpenQty", "ReturnQty", "GrandAmount"],
                    paging: {
                        enabled: false
                    },
                    columns: [
                        {
                            name: "ReturnQty",
                            template: "<input class='number'>",
                            on: {
                                "keyup": function (e) {
                                    $(this).asNumber();
                                    let value = parseFloat(this.value);
                                    if (isNaN(value))
                                        value = 0;

                                    returnamount = e.data.Price * parseFloat(value);

                                    $receiptDetail.updateColumn(e.key, "Amount", returnamount);
                                    $receiptDetail.updateColumn(e.key, "GrandAmount", returnamount);
                                    e.data.ReturnQty = __poscore.toNumber(value);
                                    checkOverQty(dialog, $receiptDetail.yield());

                                }
                            }
                        }
                    ],

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
                    __poscore.checkOpenShift(function(){           
                        let _returnSerials = [],
                        _returnBatches = [],

                        __receiptDetails = $.grep($receiptDetail.yield(), function (item) {
                            return item.ReturnQty > 0;
                        });

                    if (__receiptDetails.length <= 0) {
                        dialog.content.find(".error-message").removeClass("fn-green").addClass("fn-red").text("No item to return.");
                    }
                    else {

                        var dlgReturnItem = __poscore.dialog("Return Items", "Confirm return the specified items?", "warning", "ok-cancel");
                        dlgReturnItem.confirm(function () {
                        $(".return-receipt-reason").val("");
                            let $dialogReason = new DialogBox({
                                content: {
                                    selector: "#reason-return-receipt-content",
                                },
                                caption: "Reasons",
                                type: "ok-cancel"
                            });
                            var reason = "";
                            $dialogReason.confirm(function () {
                             
                                reason = $(".return-receipt-reason").val();
                                if (reason == "" || reason == undefined || isEmpty(reason)) {
                                    $dialogReason.content.find(".cancel-receipt-error").text("Reason is required.");
                                }
                                else {
                                    $.post(__urls.sendReturnItems, {
                                        returnItems: __receiptDetails,
                                        serial: JSON.stringify(_returnSerials),
                                        batch: JSON.stringify(_returnBatches),
                                        PaymentmeansID: $("#list-multipay").val(),
                                        reason: reason,
                                    }, function (message) {
                                        if (message.IsSerail) {
                                            const serial = SerialTemplate({
                                                isReturnItem: true,
                                                data: {
                                                    serials: message.Data,
                                                }
                                            });
                                            serial.serialTemplate();
                                            const seba = serial.callbackInfo();
                                            _returnSerials = seba.serials;
                                        } else if (message.IsBatch) {
                                            const batch = BatchNoTemplate({
                                                isReturnItem: true,
                                                data: {
                                                    batches: message.Data,
                                                }
                                            });
                                            batch.batchTemplate();
                                            const seba = batch.callbackInfo();
                                            _returnBatches = seba.batches;
                                        } else if (!message.IsRejected) {
                                            _returnSerials = [];
                                            _returnBatches = [];
                                            $("#checkingSerialString").val("unchecked");
                                            $("#checkingBatchString").val("unchecked");
                                            let _receiptId = 0;
                                            $.grep(__receiptDetails, function (item) {
                                                _receiptId = item.ReceiptID;
                                                item.OpenQty -= item.ReturnQty;
                                                item.ReturnQty = 0;
                                                if (item.OpenQty <= 0) {
                                                    $receiptDetail.removeRow(item.ID);
                                                } else {
                                                    $receiptDetail.updateRow(item);
                                                }
                                            });

                                            if ($receiptDetail.yield().length <= 0) {
                                                $receipts.removeRow(_receiptId);
                                                $receipts.reload();
                                            }
                                            dlgReturnItem.shutdown();
                                        }
                                        ViewMessage({
                                            summary: {
                                                selector: ".error-message",
                                                bordered: false
                                            }
                                        }).bind(message);
                                    });
                                    $dialogReason.shutdown();
                                }
                                __this.highlight(target, false);
                                __array.length = 0;
                            });
                            $dialogReason.reject(function () {
                                $(".return-receipt-reason").val("");
                                __array.length = 0;
                                $dialogReason.shutdown();
                            });
                            //========================Void Reason========================
                            $("#btn_addvoidreson").click(function () {
                                $(".reason").focus();
                                dataobj.ID = 0;
                                dataobj.Reason = $(".reason").val();
                                $.ajax({
                                    url: "/POS/AddVoidReason",
                                    type: "POST",
                                    dataType: "JSON",
                                    data: { data: dataobj },
                                    success: function (respones) {
                                        $(".reason").val("");
                                        $.ajax({
                                            url: "/POS/GetVoidReason",
                                            type: "get",
                                            dataType: "JSON",
                                            success: function (data) {
                                                ReturnReceiptReason(data)
                                            }
                                        });
                                        if (respones.IsApproved) {
                                            new ViewMessage({
                                                summary: {
                                                    selector: "#error-summary"
                                                },
                                            }, respones);
                                        } else {
                                            new ViewMessage({
                                                summary: {
                                                    selector: "#error-summary"
                                                }
                                            }, respones);
                                        }
                                    }
                                });
                                // dialog.preventConfirm(true);
                            });
                            $("#btn_addnewvoidreson").click(function () {
                                $("#btn_addvoidreson").show();
                                $("#btn_updatevoidreson").css("visibility", "hidden");
                                $("#btn_addnewvoidreson").css("visibility", "hidden");
                                $(".reason").css("width", "85%")
                                $(".reason").focus();
                            })
                            const dataobj = {
                                ID: 0,
                                Reason: "",
                                Delete: false
                            }

                            $("#btn_updatevoidreson").click(function () {
                                $(".reason").focus();
                                dataobj.ID = parseInt($(".idreason").val());
                                dataobj.Reason = $(".reason").val();
                                $.ajax({
                                    url: "/POS/AddVoidReason",
                                    type: "POST",
                                    dataType: "JSON",
                                    data: { data: dataobj },
                                    success: function (respones) {
                                        $(".reason").val("");
                                        $.ajax({
                                            url: "/POS/GetVoidReason",
                                            type: "get",
                                            dataType: "JSON",
                                            success: function (data) {
                                                ReturnReceiptReason(data)
                                            }
                                        });
                                        if (respones.IsApproved) {
                                            new ViewMessage({
                                                summary: {
                                                    selector: "#error-summary"
                                                },
                                            }, respones);
                                        } else {
                                            new ViewMessage({
                                                summary: {
                                                    selector: "#error-summary"
                                                }
                                            }, respones);
                                        }
                                    }
                                });
                                // dialog.preventConfirm(true);
                            });
                            $("#void-item-listview").val("");
                            $.ajax({
                                url: "/POS/GetVoidReason",
                                type: "get",
                                dataType: "JSON",
                                success: function (data) {
                                    ReturnReceiptReason(data)
                                }
                            });
                            $(".return-receipt-reason").html("");
                        });
                    }

                    }) 
                    
                  
                });

                function onGetReceiptDetails(receiptDetails) {
                    $receiptDetail.clearRows();
                    $receiptDetail.bindRows(receiptDetails);
                    $("#search-receipt-items").on("keyup", function () {
                        var _receiptItems = searchReceiptItems(receiptDetails, this.value);
                        $receiptDetail.clearRows();
                        $receiptDetail.bindRows(_receiptItems);
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
            $.post(__urls.getReturnReceipts, {
                dateFrom: dateFrom,
                dateTo: dateTo,
                keyword: keyword
            }, function (receipts) {
                $listView.clearRows();
                $listView.bindRows(receipts);
            });
        }

        function searchReceiptItems(receiptDetails, keyword = "") {
            let input = keyword.replace(/\s/g, '');
            let regex = new RegExp(input, "i");
            if (isEmpty(keyword)) {
                return receiptDetails;
            }
            return $.grep(receiptDetails, function (item) {
                return regex.test(item.Code.replace(/\s/g, ''))
                    || regex.test(item.KhName.replace(/\s/g, ''))
                    || regex.test(item.UoM.replace(/\s/g, ''));
            });
        }

        function checkOverQty(dialog, items) {
            let isOverQty = false;
            $.grep(items, function (_item) {
                if (__poscore.toNumber(_item.ReturnQty) > _item.OpenQty) {
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
        __poscore.sendOrder();
    }

    this.fxBillOrder = function () {
        __poscore.billOrder();
    }

    this.fxPayOrder = function (onPaid) {
        __poscore.payOrder(onPaid);
    }

    //update on 19-06-2021
    this.fxVoidOrder = function (target = undefined) {
        $(".void-order-reason").val("");
        __array.length = 0;
        __this.highlight(target);
        __poscore.checkPrivilege("P005", function (posInfo) {

            var __order = $.extend(true, {}, posInfo.Order);
            __poscore.checkCart(function () {
                if (__order.OrderID <= 0) {
                    __poscore.dialog("Void Order", "Please send before voiding order!");
                    return;
                }
                let $dialogReason = new DialogBox({
                    content: {
                        selector: "#void-order-content"
                    },
                    caption: "Void Order",
                    type: "ok-cancel"
                });
                //========================Void Reason========================

                $("#btn_addvoidreson").click(function () {
                    $(".reason").focus();
                    dataobj.ID = 0;
                    dataobj.Reason = $(".reason").val();
                    $.ajax({
                        url: "/POS/AddVoidReason",
                        type: "POST",
                        dataType: "JSON",
                        data: { data: dataobj },
                        success: function (respones) {
                            $(".reason").val("");
                            $.ajax({
                                url: "/POS/GetVoidReason",
                                type: "get",
                                dataType: "JSON",
                                success: function (data) {
                                    VoidorderReason(data)
                                }
                            });
                            if (respones.IsApproved) {
                                new ViewMessage({
                                    summary: {
                                        selector: "#error-summary"
                                    },
                                }, respones);
                            } else {
                                new ViewMessage({
                                    summary: {
                                        selector: "#error-summary"
                                    }
                                }, respones);
                            }
                        }
                    });
                    // dialog.preventConfirm(true);
                });
                $("#btn_addnewvoidreson").click(function () {
                    $("#btn_addvoidreson").show();
                    $("#btn_updatevoidreson").css("visibility", "hidden");
                    $("#btn_addnewvoidreson").css("visibility", "hidden");
                    $(".reason").css("width", "85%")
                    $(".reason").focus();
                })
                const dataobj = {
                    ID: 0,
                    Reason: "",
                    Delete: false
                }

                $("#btn_updatevoidreson").click(function () {
                    $(".reason").focus();
                    dataobj.ID = parseInt($(".idreason").val());
                    dataobj.Reason = $(".reason").val();
                    $.ajax({
                        url: "/POS/AddVoidReason",
                        type: "POST",
                        dataType: "JSON",
                        data: { data: dataobj },
                        success: function (respones) {
                            $(".reason").val("");
                            $.ajax({
                                url: "/POS/GetVoidReason",
                                type: "get",
                                dataType: "JSON",
                                success: function (data) {
                                    VoidorderReason(data)
                                }
                            });
                            if (respones.IsApproved) {
                                new ViewMessage({
                                    summary: {
                                        selector: "#error-summary"
                                    },
                                }, respones);
                            } else {
                                new ViewMessage({
                                    summary: {
                                        selector: "#error-summary"
                                    }
                                }, respones);
                            }
                        }
                    });
                    // dialog.preventConfirm(true);
                });
                $.ajax({
                    url: "/POS/GetVoidReason",
                    type: "get",
                    dataType: "JSON",
                    success: function (data) {
                        VoidorderReason(data)
                    }
                });
                $dialogReason.confirm(function () {
                    __order.Reason = $dialogReason.content.find(".void-order-reason").val();
                    if (__order.Reason == "" || __order.Reason == undefined) {
                        $dialogReason.content.find(".void-order-error").text("Reason is required.");
                    } else {
                        __poscore.updateOrder(__order);
                        __poscore.voidOrder();
                        $dialogReason.shutdown();
                    }
                    $(".void-order-reason").html("");
                    __array.length = 0;
                });
                $dialogReason.reject(function () {
                    $(".void-order-reason").val("");
                    __array.length = 0;
                   
                    $dialogReason.shutdown();
                });
                __this.highlight(target, false);
            });
        });
    }

    this.fxDiscountTotal = function () {
        __poscore.checkCart(function () {
            __poscore.checkPrivilege("P022", onCheckPrivilege);
        });

        function onCheckPrivilege(posInfo) {
            let __order = $.extend({}, posInfo.Order);
            $('.discount-percent-erorr').text('');
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
                var discount = dialog.content.find(".discount-percent").val();
                if (isNaN(discount)) {
                    dialog.content.find('.discount-percent').val('');
                    dialog.content.find('.discount-percent').focus();
                    dialog.content.find('.discount-total-erorr').text("Please input discount rate ...!");
                } else {
                    if (__poscore.toNumber(__order.DiscountRate) <= 100) {
                        if (isValidArray(__order.OrderDetail)) {
                            __order.OrderDetail.forEach(i => {
                                __poscore.sumLineItem(i);
                            })
                        }
                        __poscore.summarizeOrder(__order);
                        dialog.shutdown();
                    }
                    else {
                        dialog.content.find('.discount-percent').focus();
                        dialog.content.find('.discount-total-erorr').text("Invalid total amount.");
                    }
                }
            });

            dialog.reject(function () {
                dialog.shutdown();
            });

            dialog.invoke(function () {
                var $granTotal = dialog.content.find(".grandtotal");
                var $disPercent = dialog.content.find('.discount-percent');
                var $disValue = dialog.content.find('.discount-value');
                var $currencyType = dialog.content.find('.currency-type');
                var $remarkDis = dialog.content.find('#remarkDiscount');
                $currencyType.text(posInfo.Order.Currency.Description);
                $disValue.val(__poscore.toCurrency(__order.DiscountValue));
                $disPercent.val(__poscore.toCurrency(__order.DiscountRate));
                dialog.content.find(".grandtotal").val(__poscore.toCurrency(__order.GrandTotal));
                $disPercent.focus();
                $disPercent.asNumber();
                $disValue.asNumber();
                $disValue.on("keyup", function () {
                    __order.DiscountValue = __poscore.toNumber(this.value);
                    if (__order.Sub_Total !== 0) {
                        __order.DiscountRate = __order.DiscountValue / __order.SubTotalNoTax * 100;
                    }

                    __poscore.calculateOrder(__order);
                    $disPercent.val(__poscore.toCurrency(__order.DiscountRate));
                    $granTotal.val(__poscore.toCurrency(__order.GrandTotal));
                });
                $disPercent.on("keyup", function () {
                    __order.DiscountRate = __poscore.toNumber(this.value);
                    __order.DiscountValue = __poscore.toNumber(__order.SubTotalNoTax) * __order.DiscountRate / 100;
                    __poscore.calculateOrder(__order);
                    $granTotal.val(__poscore.toCurrency(__order.GrandTotal));
                    $disValue.val(__poscore.toCurrency(__order.DiscountValue));
                    $granTotal.addClass("fn-black").removeClass("fn-red");
                    if (__order.GrandTotal < 0) {
                        $granTotal.removeClass("fn-black").addClass("fn-red");
                    }
                });
                if (__order.RemarkDiscountID > 0) {
                    updateRemarkSelect(posInfo.RemarkDiscountItem, __order.RemarkDiscountID)
                }
                posInfo.RemarkDiscountItem.forEach(i => {
                    if (i.Selected) {
                        $(`<option selected value="${i.Value}">${i.Text}</option>`).appendTo($remarkDis);
                        __order.RemarkDiscountID = i.Value;
                    }
                    else {
                        $(`<option value="${i.Value}">${i.Text}</option>`).appendTo($remarkDis);
                    }
                })
                $remarkDis.change(function () {
                    const id = parseInt(this.value);
                    updateRemarkSelect(posInfo.RemarkDiscountItem, this.value);
                    __order.RemarkDiscountID = id;
                })
            });
        }
    }


    //=====================add funtion tips==============

    function updateRemarkSelect(data, id) {
        if (data.length > 0) {
            data.forEach(i => {
                if (i.Value == id) {
                    i.Selected = true;
                } else {
                    i.Selected = false;
                }
            })
        }
    }

    function processMoveOrders(posInfo, config, onConfirm, onReject) {
        __poscore.loadScreen();
        let _orders = [].concat(__orderInfo.Orders);
        $.post(config.ajax.url, config.ajax.data,
            function (resp) {
                var __order = $.extend(true, {}, posInfo.Order),
                    __currentTableId = __order.TableID,
                    dialogSetting = {
                        content: {
                            selector: "#move-receipt"
                        },
                        position: "top-center",
                        type: "ok-cancel",
                        button: {
                            ok: {
                                text: "CONFIRM"
                            }
                        }
                    }

                let freeTables = resp;
                dialogSetting = $.extend(true, dialogSetting, config.dialog);
                let tableDialog = new DialogBox(dialogSetting);
                let $container = tableDialog.content.find("#movable-table-list");
                __poscore.displayOrderTitle(tableDialog.content.find("#title-order-no"), config.titleTableOnly);
                $container.html(createTableList(freeTables, __order.TableID));
                if (config.includeOrders) {
                    createTemplateListOrders(_orders);
                    $("#content-included-orders").show();
                }

                tableDialog.content.find("#search-movable-tables").on("keyup", function () {
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
                    $.each($tables, function (i, table) {
                        if ($(table).prop("checked")) {
                            __currentTableId = parseInt($(table).data("id"));
                        }
                    });

                    if (__currentTableId > 0) {
                        if (typeof onConfirm === "function") {
                            onConfirm.call(__this, __currentTableId, _orders);
                        }

                        tableDialog.shutdown();
                    } else {
                        __poscore.dialog("Move Table Orders",
                            "Please select any table to move.", "warning");
                    }
                });

                tableDialog.reject(function (e) {
                    tableDialog.shutdown();
                    if (typeof onReject === "function") {
                        onReject.call(__this, __currentTableId, _orders);
                    }
                });
                __poscore.loadScreen(false);
            });

        function createTemplateListOrders(orders) {
            let _table = ViewTable({
                selector: "#list-orders-in-table",
                keyField: "OrderID",
                visibleFields: ["OrderNo", "DateIn", "TimeIn", "DateOut", "TimeOut", "Selected"],
                columns: [
                    {
                        name: "DateIn",
                        dataFormat: "DD-MM-YYYY"
                    },
                    {
                        name: "DateOut",
                        dataFormat: "DD-MM-YYYY"
                    },
                    {
                        name: "Selected",
                        template: "<input type='checkbox' class='form-control'>",
                        on: {
                            "change": function (e) {
                                e.data.Selected = $(this).prop("checked");
                            }
                        }
                    }
                ]
            });
            orders.filter(o => {
                if (o.OrderID == __orderInfo.Order.OrderID) {
                    o.Selected = true;
                }
            });

            _table.bindRows(orders);
        }

        function createTableList(freeTables, currentTableId) {
            let template = "";
            let $table = $("<table></table>");
            for (let i = 0; i < freeTables.length; i++) {
                template = '<input name="table" data-id="' + freeTables[i].ID + '" type="radio">'
                    + freeTables[i].Name;
                if (i == 0) {
                    template = '<input name="table" data-id="' + freeTables[i].ID + '" checked="checked" type="radio"> '
                        + freeTables[i].Name;
                }
                let row = $("<tr><td>" + template + "</td></tr>");
                $table.append(row);
            }
            return $table;
        }
    }

    this.fxMoveReceipt = function (target = undefined, tableAliasName = "Table") {
        __this.highlight(target);
        __poscore.checkPrivilege("P001", function (info) {
            var __config = {
                tableAliasName: tableAliasName,
                titleTableOnly: false,
                ajax: {
                    url: __urls.getOtherTables,
                    data: { currentTableId: info.Order.TableID }
                },
                dialog: {
                    icon: "fas fa-cut",
                    caption: "Move Receipt"
                },
                includeOrders: true
            };
            if (info.Order.OrderID <= 0) {
                __poscore.dialog("Move Receipt", "Receipt[" + info.Order.OrderNo + "] is not available for moving.", "warning");
            } else {
                processMoveOrders(info, __config, function (currentTableId, orders) {
                    var __order = $.extend(true, {}, info.Order);
                    __poscore.moveOrders(__order.TableID, currentTableId, orders);
                    __this.highlight(target, false);
                }, function () {
                    __this.highlight(target, false);
                });
            }
        });
    }

    this.fxMoveTable = function (target = undefined, tableAliasName = "Table") {
        __this.highlight(target);
        __poscore.checkPrivilege("P001", function (info) {
            var __order = $.extend(true, {}, info.Order),
                __config = {
                    tableAliasName: tableAliasName,
                    titleTableOnly: true,
                    ajax: {
                        url: __urls.getFreeTables,
                        data: {
                            tableId: __order.TableID
                        }
                    },
                    dialog: {
                        icon: "fas fa-sync-alt",
                        caption: "Change " + tableAliasName
                    },
                    includeOrders: false
                };

            if (info.Order.OrderID <= 0) {
                __poscore.dialog("Change " + __config.tableAliasName, __config.tableAliasName
                    + "[" + info.OrderTable.Name + "] is not available for changing.", "warning");
            } else {
                processMoveOrders(info, __config, function (currentTableId) {
                    __poscore.changeTable(__order.TableID, currentTableId);
                    __this.highlight(target, false);
                }, function () {
                    __this.highlight(target, false);
                });
            }
        });
    }

    this.fxDiscountItem = function (target = undefined) {
        __this.highlight(target);
        __poscore.checkPrivilege("P019", function (posInfo) {
            __poscore.checkCart(function () {
                var __order = $.extend(true, {}, posInfo.Order);
                const baseCurrency = posInfo.DisplayPayOtherCurrency.filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0] ?? { DecimalPlaces: 0 };
                //__poscore.resetTax(__order[__config.detail.fieldName]);
                let dialog = new DialogBox({
                    caption: "Discount Item",
                    type: "yes-no",
                    icon: "fas fa-percent",
                    content: {
                        selector: "#discount-item"
                    }
                });

                dialog.invoke(function () {
                    checkOverDiscount(dialog, __order[__config.detail.fieldName]);
                    var $listView = ViewTable({
                        selector: ".discount-item-listview",
                        keyField: "LineID",
                        visibleFields: ["Code", "KhmerName", "Qty", "UnitPrice", "Uom", "DiscountRate", "DiscountValue", "TaxValue", "Total", "RemarkDiscounts"],
                        paging: {
                            enabled: false
                        },
                        dynamicCol: {
                            afterAction: true,
                            headerContainer: "#col-append-after",
                        },
                        columns: [
                            {
                                name: "Total",
                                dataType: "number",
                                dataFormat: { fixed: baseCurrency.DecimalPlaces },
                            },
                            {
                                name: "Qty",
                                dataType: "number",
                                dataFormat: { fixed: baseCurrency.DecimalPlaces },
                            },
                            {
                                name: "TaxValue",
                                dataType: "number",
                                dataFormat: { fixed: baseCurrency.DecimalPlaces },
                            },
                            {
                                name: "UnitPrice",
                                dataType: "number",
                                dataFormat: { fixed: baseCurrency.DecimalPlaces },
                            },
                            {
                                name: "DiscountRate",
                                template: "<input class='number' type='text'>",
                                on: {
                                    "keyup blur": function (e) {
                                        $(this).asNumber();
                                        e.data.DiscountRate = __poscore.toNumber(this.value);
                                        __poscore.sumLineItem(e.data);
                                        $listView.updateColumn(e.key, "DiscountValue", __poscore.toCurrency(e.data.DiscountValue));
                                        $listView.updateColumn(e.key, "Total", __poscore.toNumber(e.data.Total));
                                        checkOverDiscount(dialog, $listView.yield());
                                    }
                                }
                            },
                            {
                                name: "DiscountValue",
                                template: "<input class='number' type='text'>",
                                on: {
                                    "keyup blur": function (e) {
                                        $(this).asNumber();
                                        var discountRate = 0;
                                        var lineTotal = (parseFloat(e.data.UnitPrice) * parseFloat(e.data.Qty));
                                        if (lineTotal != 0) {
                                            discountRate = parseFloat(this.value) / lineTotal * 100;
                                        }
                                        e.data.DiscountRate = discountRate;
                                        __poscore.sumLineItem(e.data);
                                        $listView.updateColumn(e.key, "DiscountRate", __poscore.toCurrency(e.data.DiscountRate));
                                        $listView.updateColumn(e.key, "Total", __poscore.toNumber(e.data.Total));
                                        checkOverDiscount(dialog, $listView.yield());
                                    }
                                }
                            },
                            {
                                name: "RemarkDiscounts",
                                template: `<select></select>`,
                                on: {
                                    "change": function (e) {
                                        e.data.RemarkDiscountID = parseInt(this.value);
                                        updateRemarkSelect(e.data.RemarkDiscounts, this.value);
                                    }
                                }
                            },
                            {
                                name: "AddictionProps",
                                valueDynamicProp: "ValueName",
                                dynamicCol: true,
                            }
                        ]
                    });
                    __poscore.bindLineItems(__order[__config.detail.fieldName], $listView);
                    __this.displayTax(".discount-item-listview", posInfo);
                    var $itemSearch = dialog.content.find("#discount-item-search");
                    $itemSearch.on("keyup", function () {
                        var searcheds = searchItems(this.value, __order[__config.detail.fieldName]);
                        $listView.clearRows();
                        $listView.bindRows(searcheds);
                        __this.displayTax(".discount-item-listview", posInfo);
                    });
                    dialog.content.find("table input.number").asPositiveNumber();
                    dialog.confirm(function () {
                        __poscore.updateOrder(__order);
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
                if (__poscore.toNumber(_item.Total) < 0) {
                    isOverDiscount = true;
                }
            });

            if (isOverDiscount) {
                dialog.content.find(".error-message").text("Invalid total amount.");
                dialog.preventConfirm(true);
            } else {
                dialog.content.find(".error-message").text("");
                dialog.preventConfirm(false);
            }
        }

        function searchItems(inputValue, items) {
            let input = inputValue.replace(/\s/g, '');
            let regex = new RegExp(input, "i");
            var searcheds = $.grep(items, function (item) {
                return regex.test(item.KhmerName) || regex.test(item.Code) || regex.test(item.Uom);
            });
            return searcheds;
        }
    }
    this.displayTax = function (container, __orderInfo) {
        switch (__orderInfo.Setting.TaxOption) {
            case 0:
                $(container).find(".tax-display").hide();
                $(container).find("[data-field='TaxGroups']").hide();
                $(container).find("[data-field='TaxValue']").hide();
                break;
            case 3:
                $(container).find(".tax-display").hide();
                $(container).find("[data-field='TaxGroups']").hide();
                $(container).find("[data-field='TaxValue']").hide();
                break;
            default:
                $(container).find(".tax-display").show();
                $(container).find("[data-field='TaxGroups']").hide();
                $(container).find("[data-field='TaxValue']").show();
                break;
        }
    }
    this.fxCombineReceipts = function (target = undefined) {
        __this.highlight(target);
        __poscore.checkPrivilege("P002", function (posInfo) {
            $.post(__urls.getOrdersToCombine, {
                orderId: posInfo.Order.OrderID
            }, function (receipts) {
                if (receipts.length > 0) {
                    __poscore.loadScreen();
                    let dialog = new DialogBox({
                        caption: "Combine Orders ( " + posInfo.Order.OrderNo + " )",
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
                        __poscore.loadScreen();
                        var $receipts = dialog.content.find("input[name='receipt']");
                        let activeOrders = [];
                        $.each($receipts, function (i, rec) {
                            if ($(rec).prop("checked")) {
                                activeOrders.push({
                                    OrderID: $(rec).data("id")
                                });
                            }
                        });

                        let combinedOrder = {
                            TableId: posInfo.Order.TableID,
                            OrderID: posInfo.Order.OrderID,
                            Orders: activeOrders
                        };

                        if (activeOrders.length > 0) {
                            $.post(__urls.combineOrders,
                                { combineOrder: combinedOrder },
                                function () {
                                    __poscore.loadCurrentOrderInfo(posInfo.Order.TableID);
                                    // __poscore.resetOrder(posInfo.Order.TableID, 0, true);
                                    __poscore.loadScreen(false);
                                }
                            );
                            dialog.shutdown();
                        } else {
                            __poscore.dialog("Combine Orders", "Please select any order to combine!", "warning");
                        }
                        __this.highlight(target, false);
                    });
                    dialog.reject(function () {
                        dialog.shutdown();
                        __this.highlight(target, false);
                    });
                    __poscore.loadScreen(false);
                } else {
                    __poscore.dialog("Combine Orders", "No order to combine!", "warning").
                        confirm(function () {
                            __this.highlight(target, false);
                        });
                    __poscore.loadScreen(false);
                }

            });
        });

        function createList(receipts) {
            let template = "";
            let $table = $("<table></table>");
            for (let i = 0; i < receipts.length; i++) {
                template = '<input name="receipt" data-id="' + receipts[i].OrderID + '" type="radio"> '
                    + receipts[i].TitleNote;
                let row = $("<tr><td style='text-align: left;'>" + template + "</td></tr>");
                $table.append(row);
            }
            return $table;
        }
    }

    this.fxSplitReceipts = function (target = undefined) {
        __this.highlight(target);
        __poscore.checkPrivilege("P003", function (posInfo) {
            if (isValidJSON(posInfo.Order) && posInfo.Order.OrderID > 0) {
                onCheckedOrder(posInfo.Order);
            } else {
                __poscore.dialog("Split Receipt Denied", "No receipts to split.", "warning");
            }
        });

        function onCheckedOrder(order) {
            let dialog = new DialogBox(
                {
                    caption: "Split Receipt ( " + order.OrderNo + " )",
                    content: {
                        selector: "#split-receipt-content"
                    },
                    icon: "fas fa-clone",
                    position: "top-center",
                    type: "ok-cancel"
                }
            );
            dialog.invoke(function () {
                var __splitOrder = {};
                __splitOrder = $.extend(true, {}, order);
                var $listView = ViewTable({
                    selector: dialog.content.find("#split-receipt-listview"),
                    keyField: "LineID",
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
                                    checkOverQtySplit(dialog, $listView.yield());
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
                                }
                            }
                        }
                    ]

                });

                __poscore.bindLineItems(__splitOrder[__config.detail.fieldName], $listView);
                dialog.content.find("input.number").asNumber();
                dialog.reject(function () {
                    dialog.shutdown();
                    __this.highlight(target, false);
                });

                dialog.confirm(function () {
                    __splitOrder[__config.detail.fieldName] = $.grep($listView.yield(), function (item) {
                        return item.PrintQty > 0 && item.PrintQty <= item.Qty;
                    });

                    if (isValidArray(__splitOrder[__config.detail.fieldName])) {
                        $.ajax({
                            url: __urls.sendSplitOrder,
                            type: "POST",
                            data: { splitOrder: __splitOrder },
                            success: function (orderSplit) {
                                __poscore.resetOrder(orderSplit.TableID, 0, true);
                            }
                        });
                        dialog.shutdown();
                        __this.highlight(target, false);
                    } else {
                        dialog.content.find(".error-message").text("Please input quantity or no item to split.");
                    }

                });

            });
        }

        function checkOverQtySplit(dialog, items) {
            let isOverQty = false;
            let isNegative = false;
            $.grep(items, function (_item) {
                if (__poscore.toNumber(_item.PrintQty) < 0) {
                    isNegative = true;
                }

                if (__poscore.toNumber(_item.PrintQty) > _item.Qty || __poscore.toNumber(_item.PrintQty) < 0) {
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
    let priew = false;
    let userid = 0;
    this.fxReprint = function () {
        __poscore.checkPrivilege("P014", function (posInfo) {
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
                    visibleFields: ["ReceiptNo", "Cashier", "CusName", "Phone", "DateOut", "TimeOut", "TableName", "GrandTotal"],
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
                                    if (priew == true) {
                                        let method = "RePrintPreviewReceipt";
                                        let __printUrl = `${_printUrlPOS}POS/${method}/?id=${e.data.ReceiptID}&userid=${userid}`;

                                        window.open(__printUrl, "_blank");
                                    }
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
                $.post(__urls.getReceiptsToReprint, {
                    dateFrom: dateFrom,
                    dateTo: dateTo,
                    keyword: keyword
                }, function (receipts) {
                    userid = receipts.userid;
                    priew = receipts.priveiwsetting;
                    $listView.clearRows();
                    $listView.bindRows(receipts.receipts);
                });
            };

            function onReprint(receiptId) {
                $.post(__urls.reprintReceipt,
                    {
                        receiptId: receiptId,
                        printType: "Reprint"
                    });
            };
        });
    }
    this.fxRePendingVoidItem = function () {
        __poscore.checkPrivilege("P014", function (posInfo) {
            let $reprint = new DialogBox({
                position: "top-center",
                caption: "Pending Void Item",
                content: {
                    selector: "#pending-void-item-content"
                },
                type: "ok/cancel",
                button: {
                    cancel: {
                        callback: function (e) {
                            this.meta.shutdown();
                        }
                    },
                    ok: {
                        text: "Apply"
                    }
                },
                icon: "fas fa-broom fa-fw"
            });
            $reprint.invoke(function () {
                const $listView = ViewTable({
                    selector: ".table-pending-void-item",
                    visibleFields: ["Amount", "Cashier", "Date", "IsVoided", "OrderNo", "QueueNo", "Table", "Time"],
                    keyField: "ID",
                    paging: {
                        enabled: true,
                        pageSize: 6
                    },
                    columns: [
                        {
                            name: "IsVoided",
                            template: '<input type="checkbox" />',
                            on: {
                                "click": function (e) {
                                    e.data.IsVoided = $(this).prop("checked");
                                }
                            }
                        }
                    ],
                    // actions: [
                    //     {
                    //         template: `<i class="fas fa-trash-alt csr-pointer fn-red"></i>`,
                    //         on: {
                    //             "click": function (e) {
                    //                 __poscore.checkPrivilege("P026", function () {
                    //                     $.post(__urls.deletePendingVoidItem, { id: e.key }, function (res) {
                    //                         if (res.Error) {
                    //                             new DialogBox({
                    //                                 content: res.Message
                    //                             });
                    //                         } else {
                    //                             $listView.removeRow(e.key);
                    //                         }
                    //                     })
                    //                 })
                    //             }
                    //         }
                    //     }
                    // ]
                });
                let $dateFrom = $reprint.content.find(".pending-void-item-date-from"),
                    $dateTo = $reprint.content.find(".pending-void-item-date-to");
                $dateFrom[0].valueAsDate = new Date();
                $dateTo[0].valueAsDate = new Date();
                bindPendingVoidItem($listView, $dateFrom.val(), $dateTo.val());
                $dateFrom.on("change", function () {
                    bindPendingVoidItem($listView, this.value, $dateTo.val());
                });

                $dateTo.on("change", function () {
                    bindPendingVoidItem($listView, $dateFrom.val(), this.value);
                });

                $reprint.content.find("#search-pending-void-item").on("keyup", function () {
                    bindPendingVoidItem($listView, $dateFrom.val(), $dateTo.val(), this.value);
                });
                $reprint.confirm(function () {
                   $(".pending-void-item-reason").val("");
                    const data = $listView.yield().filter(i => i.IsVoided);
                    if (isValidArray(data)) {
                        $(".pending-void-items-error").text("");
                        let $dialogReason = new DialogBox({
                            content: {
                                selector: "#pending-void-item-content-reason"
                            },
                            caption: "Pending Void Item Reason",
                            type: "ok-cancel"
                        });
                        $dialogReason.confirm(function () {
                            $("#pending-void-item-reason").html("");
                            var reason = $dialogReason.content.find(".pending-void-item-reason").val();
                            if (reason == "" || reason == undefined) {
                                $dialogReason.content.find(".pending-void-item-error").text("Reason is required.");
                            } else {
                                $.post(__urls.submitPendingVoidItem, { data: JSON.stringify(data), reason }, function (res) {
                                    if (res.IsApproved) {
                                        $reprint.shutdown();
                                    }
                                    if (res.IsRejected) {
                                        $(".pending-void-items-error").text(res.Items["Message"].Message)
                                    }
                                })
                                $dialogReason.shutdown();
                            }

                        });

                        $dialogReason.reject(function () {
                           $(".pending-void-item-reason").val("");
                            __array.length = 0;
                            $dialogReason.shutdown();
                        });
                        //========================Void Reason========================
                        $("#btn_addvoidreson").click(function () {
                            $(".reason").focus();
                            dataobj.ID = 0;
                            dataobj.Reason = $(".reason").val();
                            $.ajax({
                                url: "/POS/AddVoidReason",
                                type: "POST",
                                dataType: "JSON",
                                data: { data: dataobj },
                                success: function (respones) {
                                    $(".reason").val("");
                                    $.ajax({
                                        url: "/POS/GetVoidReason",
                                        type: "get",
                                        dataType: "JSON",
                                        success: function (data) {
                                            PendingVoidItemReason(data)
                                        }
                                    });
                                    if (respones.IsApproved) {
                                        new ViewMessage({
                                            summary: {
                                                selector: "#error-summary"
                                            },
                                        }, respones);
                                    } else {
                                        new ViewMessage({
                                            summary: {
                                                selector: "#error-summary"
                                            }
                                        }, respones);
                                    }
                                }
                            });
                            // dialog.preventConfirm(true);
                        });
                        $("#btn_addnewvoidreson").click(function () {
                            $("#btn_addvoidreson").show();
                            $("#btn_updatevoidreson").css("visibility", "hidden");
                            $("#btn_addnewvoidreson").css("visibility", "hidden");
                            $(".reason").css("width", "85%")
                            $(".reason").focus();
                        })
                        const dataobj = {
                            ID: 0,
                            Reason: "",
                            Delete: false
                        }
                        $("#btn_updatevoidreson").click(function () {
                            $(".reason").focus();
                            dataobj.ID = parseInt($(".idreason").val());
                            dataobj.Reason = $(".reason").val();
                            $.ajax({
                                url: "/POS/AddVoidReason",
                                type: "POST",
                                dataType: "JSON",
                                data: { data: dataobj },
                                success: function (respones) {
                                    $(".reason").val("");
                                    $.ajax({
                                        url: "/POS/GetVoidReason",
                                        type: "get",
                                        dataType: "JSON",
                                        success: function (data) {
                                            PendingVoidItemReason(data)
                                        }
                                    });
                                    if (respones.IsApproved) {
                                        new ViewMessage({
                                            summary: {
                                                selector: "#error-summary"
                                            },
                                        }, respones);
                                    } else {
                                        new ViewMessage({
                                            summary: {
                                                selector: "#error-summary"
                                            }
                                        }, respones);
                                    }
                                }
                            });
                            // dialog.preventConfirm(true);
                        });
                        $("#void-item-listview").val("");
                        $.ajax({
                            url: "/POS/GetVoidReason",
                            type: "get",
                            dataType: "JSON",
                            success: function (data) {
                                PendingVoidItemReason(data)
                            }
                        });
                        $(".pending-void-item-reason").html("");

                    } else {
                        $(".pending-void-items-error").text("Please check any of these order(s) to do the pending void item!");
                    }
                })
            });

            function bindPendingVoidItem($listView, dateFrom, dateTo, keyword = "") {
                $.post(__urls.getPendingVoidItem, {
                    dateFrom: dateFrom,
                    dateTo: dateTo,
                    keyword: keyword
                }, function (receipts) {
                    $listView.clearRows();
                    $listView.bindRows(receipts);
                });
            };
        });
    }
    $("#goto-pending-void-item").click(function () {
        __this.fxRePendingVoidItem();
    })
    this.fxOpenShift = function (succeed) {
        __poscore.openShift(succeed);
    }

    this.fxCloseShift = function (succeed) {
        __poscore.closeShift(succeed);
    }
    //Start form change rate for payment
    this.fxChangeRate = function () {
        __poscore.checkPrivilege("P023", function () {
            let info = __poscore.fallbackInfo();
            let orderId = isValidJSON(info.Order) ? info.Order.OrderID : 0;
            let customerId = isValidJSON(info.Order) ? info.Order.CustomerID : 0;
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
                __poscore.loadScreen();
                var viewTable = ViewTable({
                    keyField: "LineID",
                    visibleFields: ["PLCurrencyName", "ALCurrencyName", "DisplayRate", "IsActive", "IsShowCurrency", "IsShowOtherCurrency", "DecimalPlaces"],
                    selector: "#list-change-rate",
                    paging: {
                        enabled: false,
                    },
                    columns: [
                        {
                            name: "DecimalPlaces",
                            template: "<input type='text'>",
                            on: {
                                keyup: function (e) {
                                    $(this).asNumber();
                                    e.data.DecimalPlaces = __poscore.toNumber(this.value).toFixed(0);
                                }
                            }
                        },
                        {
                            name: "IsActive",
                            template: "<input type='checkbox'>",
                            on: {
                                click: function (e) {
                                    const isChecked = $(this).prop("checked") ? true : false;
                                    if (isChecked) {
                                        updateCurrencyActive(e.data, viewTable.yield());
                                    }
                                }
                            }
                        },
                        {
                            name: "IsShowCurrency",
                            template: "<input type='checkbox'>",
                            on: {
                                click: function (e) {
                                    const isChecked = $(this).prop("checked") ? true : false;
                                    e.data.IsShowCurrency = isChecked;
                                }
                            }
                        },
                        {
                            name: "IsShowOtherCurrency",
                            template: "<input type='checkbox'>",
                            on: {
                                click: function (e) {
                                    const isChecked = $(this).prop("checked") ? true : false;
                                    e.data.IsShowOtherCurrency = isChecked;
                                }
                            }
                        },
                        {
                            name: "DisplayRate",
                            template: "<input class='number'>",
                            on: {
                                keyup: function (e) {
                                    $(this).asNumber();
                                    e.data.DisplayRate = !isNaN(__poscore.toNumber(this.value)) ? __poscore.toNumber(this.value) : 0.000;
                                }
                            }
                        }
                    ],
                });
                $.post("/POS/GetChangeRateTemplate", { orderId, customerId }, function (resp) {
                    viewTable.bindRows(resp);
                    __poscore.loadScreen(false);
                });
                dialog.confirm(function () {
                    var dcs = viewTable.yield();
                    $.post("/POS/SaveDisplayCurrencies",
                        { displayCurrencies: dcs },
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
                                __poscore.resetOrder(info.Order.TableID, 0, true, true);
                            }
                        });
                    dialog.shutdown();
                });

                dialog.reject(function () {
                    dialog.shutdown();
                });
                dialog.content.find("table input.number").asPositiveNumber();
                function updateCurrencyActive(self, data) {
                    if (isValidArray(data) && isValidJSON(self)) {
                        data.forEach(i => {
                            if (i.LineID === self.LineID) {
                                i.IsActive = true;
                                viewTable.updateColumn(i.LineID, "IsActive", i.IsActive);
                            } else {
                                i.IsActive = false;
                                viewTable.updateColumn(i.LineID, "IsActive", i.IsActive);
                            }
                        })
                    }
                }
            });

        });
    }
    //Edit item price
    this.fxEditPrice = function (target = undefined) {
        __this.highlight(target);
        __poscore.checkPrivilege("P004", function (posInfo) {
            var __order = $.extend(true, {}, posInfo.Order);
            const baseCurrency = posInfo.DisplayPayOtherCurrency.filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0] ?? { DecimalPlaces: 0 };
            __poscore.checkCart(function () {
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
                        keyField: "LineID",
                        visibleFields: ["Code", "KhmerName", "EnglishName", "Qty", "UnitPrice", "Uom", "DiscountRate", "TaxValue", "Total"],
                        paging: {
                            enabled: false
                        },
                        dynamicCol: {
                            afterAction: true,
                            headerContainer: "#edit-price-col-append",
                        },
                        columns: [
                            {
                                name: "KhmerName",
                                template: "<input>",
                                on: {
                                    "keyup": function (e) {
                                        e.data.KhmerName = this.value;
                                        __this.displayTax("#edit-item-price-listview", posInfo);
                                    }
                                }
                            },
                            {
                                name: "EnglishName",
                                template: "<input>",
                                on: {
                                    "keyup": function (e) {
                                        e.data.EnglishName = this.value;
                                        __this.displayTax("#edit-item-price-listview", posInfo);
                                    }
                                }
                            },
                            {
                                name: "Total",
                                dataType: "number",
                                dataFormat: { fixed: baseCurrency.DecimalPlaces },
                            },
                            {
                                name: "Qty",
                                dataType: "number",
                                dataFormat: { fixed: baseCurrency.DecimalPlaces },
                            },
                            {
                                name: "TaxValue",
                                dataType: "number",
                                dataFormat: { fixed: baseCurrency.DecimalPlaces },
                            },
                            {
                                name: "DiscountRate",
                                dataType: "number",
                                dataFormat: { fixed: baseCurrency.DecimalPlaces },
                            },
                            {
                                name: "UnitPrice",
                                template: "<input class='number'>",
                                //dataType: "number",
                                //dataFormat: { fixed: baseCurrency.DecimalPlaces },
                                on: {
                                    "keyup": function (e) {
                                        $(this).asNumber;
                                        updateUnitPrice(e.data, this.value);
                                        $listView.updateColumn(e.key, "Total", __poscore.toNumber(e.data.Total));
                                        $listView.updateColumn(e.key, "TaxValue", __poscore.toNumber(e.data.TaxValue));
                                        __this.displayTax("#edit-item-price-listview", posInfo);
                                    }
                                }
                            },
                            {
                                name: "AddictionProps",
                                valueDynamicProp: "ValueName",
                                dynamicCol: true,
                            }
                        ]
                    });
                    __poscore.bindLineItems(__order[__config.detail.fieldName], $listView);
                    __this.displayTax("#edit-item-price-listview", posInfo);
                    dialog.content.find("table input.number").asNumber();
                    dialog.confirm(function () {
                        __poscore.updateOrder(__order, $listView.yield());
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
    }

    function updateUnitPrice(item, inputValue) {
        if (inputValue == "") {
            inputValue = "0";
        }

        if (!Number.isNaN(inputValue)) {
            item.UnitPrice = __poscore.toNumber(inputValue);
            item.Total = __poscore.sumLineItem(item);
        }
    }
    // add Remark
    this.fxRemark = function (target = undefined) {
        __this.highlight(target);
        __poscore.checkPrivilege("P019", function (info) {
            __poscore.checkCart(function () {
                let dialog = new DialogBox({
                    caption: "Remark",
                    icon: "fa fa-comment",
                    content: {
                        selector: "#add-new-remark-content"
                    },
                    button: {
                        ok: {
                            text: "Apply"
                        }
                    },
                    type: "ok-cancel"
                });
                dialog.invoke(function () {

                    var orderRemark = dialog.content.find("#order-remark");
                    orderRemark.val(info.Order.Remark);
                    dialog.confirm(function () {
                        info.Order.Remark = orderRemark.val();

                        dialog.shutdown();
                        __this.highlight(target, false);
                    });
                });
                dialog.reject(function () {
                    __this.highlight(target, false);
                    dialog.shutdown();
                });

            });

        });
    }

    //Start form pos setting
    this.fxSetting = function (target) {
        __poscore.checkPrivilege("P015", function (info) {
            __poscore.loadScreen();
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
                                $.post("/api/pos/enableDualScreen", { enabled: info.Setting.DaulScreen });
                                $(".customer-id").val(info.Setting.CustomerID);
                                if (isValidJSON(info.Order)) {
                                    __poscore.loadCurrentOrderInfo(info.Order.TableID, info.Order.OrderID, 0, true);
                                }
                                //======================delay hours================
                                let hours;
                                if (info.Setting.DelayHours != 0) {
                                    $.ajax({
                                        url: "/POS/GetDelayhours",
                                        type: "get",
                                        success: function (res) {
                                                hours = res.DelayHours;
                                                // const result = addHours(hours, this.value);
                                                // let postdate = (result.getFullYear()) + "/" + (result.getMonth() + 1) + "/" + (result.getDate());
                                                // console.log("postdate",postdate);
                                                // $("#post-date-delay").val(postdate);
                                                var result  = new Date();
                                                var end     = result.setTime(result.getTime() - hours * 60 * 60 * 1000);
                                                $("#post-date").val(dateToYMD(end));
                                             function dateToYMD(end_date) {
                                                var ed = new Date(end_date);
                                                var d = ed.getDate();
                                                var m = ed.getMonth() + 1;
                                                var y = ed.getFullYear();
                                                return '' + y + '-' + (m<=9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d);
                                            }
                                        }
                                    })
                                }
                                else if(info.Setting.DelayHours == 0){
                                    $.each($("[data-date]"), function (i, t) {
                                        setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
                                    });
                                    function setDate(selector, date_value) {
                                        var _date = $(selector);
                                        _date[0].valueAsDate = new Date(date_value);
                                        _date[0].setAttribute(
                                            "data-date",
                                            moment(_date[0].value)
                                                .format(_date[0].getAttribute("data-date-format"))
                                        );
                                    }
                                }
                                if (info.Setting.CustomerTips) {
                                    $("#wrapper-customer-tips").show();
                                }
                                else if(info.Setting.CustomerTips==false){
                                    $("#wrapper-customer-tips").hide();
                                }
                                dialog.shutdown();
                            });
                            // function addHours(hours, date = new Date()) {
                            //     const dateCopy = date;
                            //     dateCopy.setTime(dateCopy.getTime() - hours * 60 * 60 * 1000);
                            //    dateCopy.getUTCFullYear() + "/" + (dateCopy.getMonth() + 1) + "/" + dateCopy.getDate() + " " + dateCopy.getHours() + ":" + dateCopy.getMinutes() + ":" + dateCopy.getSeconds();
                               
                            //    return dateCopy;
                            // }
                        }
                    }
                }
            });

            dialog.invoke(function () {
                const __this = dialog;
                __poscore.loadUserSetting(function (setting) {
                    initDataSetting(__this.content, setting);
                    $.extend(true, info.Setting, setting);
                });

                __poscore.loadScreen(false);
            });

            dialog.reject(function () {
                dialog.shutdown();
            });

            function saveSetting(content, setting, success, fail) {
                setting.PrintReceiptOrder = content.find(".print-receipt-order").prop("checked");
                setting.PrintLabel = content.find(".print-receipt-label").prop("checked");
                setting.PrintReceiptTender = content.find(".print-receipt-pay").prop("checked");
                setting.PrintCountReceipt = parseInt(content.find('.print-receipt-count').val());
                setting.PrintCountBill = parseInt(content.find('.print-bill-count').val());
                setting.QueueCount = parseInt(content.find('.max-queue-count').val());
                setting.Receiptsize = content.find(".receipt-size option:selected").text();
                setting.ReceiptTemplate = content.find('.receipt-template option:selected').text();
                setting.DaulScreen = content.find('.dual-screen').prop("checked");
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
                setting.SeriesID = content.find(".series-id option:selected").val();
                setting.TaxOption = content.find(".tax-option option:selected").val();
                setting.Tax = content.find(".tax option:selected").val();
                setting.EnablePromoCode = content.find(".enable-promocode").prop("checked");
                setting.PrinterOrder = content.find(".printerOrder option:selected").val();
                setting.PrintOrderCount = parseInt(content.find('.print-Order-count').val());
                setting.PrintLabelName = content.find(".printer-Label-Name option:selected").val();
                setting.PrintBillName = content.find(".printerBillname option:selected").val();
                setting.PrintReceiptOption = content.find(".receipt-template-option option:selected").val();
                setting.QueueOption = content.find(".queue-option option:selected").val();
                setting.IsCusPriceList = content.find(".enable-customer-price-list").prop("checked");
                setting.EnableCountMember = content.find(".enable-countmember").prop("checked");
                setting.RememberCustomer = content.find(".enable-remember-customer").prop("checked");
                setting.Cash = content.find(".enable-cash").prop("checked");
                setting.PreviewReceipt = content.find(".enable-pre-receipt").prop("checked");
                setting.Portraite = content.find(".enable-protrait").prop("checked");
                setting.CustomerTips = content.find(".enable-cus-tips").prop("checked");
                setting.DelayHours = content.find(".delay-hours").val();
                setting.SlideShow = content.find(".enable-slideshow").prop("checked");
                setting.TimeOut = content.find(".timeout-slide").val();
                setting.DefualtAmount = content.find(".enable-defualt-amount").prop("checked");

                __poscore.saveUserSetting(setting, success, fail);
            }

            function initDataSetting(content, setting) {
                content.find('.print-receipt-order').prop('checked', setting.PrintReceiptOrder);
                content.find('.print-receipt-label').prop('checked', setting.PrintLabel);
                content.find('.print-receipt-pay').prop('checked', setting.PrintReceiptTender);
                content.find('.print-receipt-count').val(setting.PrintCountReceipt);
                content.find('.print-bill-count').val(setting.PrintCountBill);
                content.find('.autoqueue').val(setting.QueueCount);
                content.find('.max-queue-count').val(setting.QueueCount);
                content.find(".receipt-size").val(setting.Receiptsize);
                //content.find(".receipt-template").val(setting.ReceiptTemplate);
                content.find('.dual-screen').prop('checked', setting.DaulScreen);
                content.find('.vat-invoice-no').val(setting.VatNum);
                content.find('.password-wifi').val(setting.Wifi);
                content.find('.customer-id').val(setting.CustomerID);
                content.find('.price-list-id').val(setting.PriceListID);
                content.find('.warehouse-id').val(setting.WarehouseID);
                content.find('.payment-means-id').val(setting.PaymentMeansID);
                content.find('.close-shift').val(setting.CloseShift);
                content.find(".autoqueue").prop("checked", setting.AutoQueue);
                content.find(".user-id").val(setting.UserID);
                content.find(".item-view-type").val(setting.ItemViewType);
                content.find(".item-page-size").val(setting.ItemPageSize);
                content.find(".printer").val(setting.Printer);
                content.find(".series-id").val(setting.SeriesID);
                content.find(".tax-option").val(setting.TaxOption);
                content.find(".tax").val(setting.Tax);
                content.find(".enable-promocode").prop("checked", setting.EnablePromoCode);

                content.find(".printerOrder").val(setting.PrinterOrder);
                content.find('.print-Order-count').val(setting.PrintOrderCount);
                content.find(".printer-Label-Name").val(setting.PrintLabelName);
                content.find(".printerBillname").val(setting.PrintBillName);
                content.find(".receipt-template-option").val(setting.PrintReceiptOption);
                content.find(".queue-option").val(setting.QueueOption);
                content.find(".enable-customer-price-list").prop("checked", setting.IsCusPriceList);
                content.find(".enable-countmember").prop("checked", setting.EnableCountMember);
                content.find(".enable-remember-customer").prop("checked", setting.RememberCustomer);
                content.find(".enable-cash").prop("checked", setting.Cash);
                content.find(".enable-pre-receipt").prop("checked", setting.PreviewReceipt);
                content.find(".enable-protrait").prop("checked", setting.Portraite);
                content.find(".enable-cus-tips").prop("checked", setting.CustomerTips);
                content.find(".delay-hours").val(setting.DelayHours);
                content.find(".enable-slideshow").prop("checked", setting.SlideShow);
                content.find(".timeout-slide").val(setting.TimeOut);
                content.find(".enable-defualt-amount").prop("checked", setting.DefualtAmount);
                
                //Queue Option
                if (setting.QueueOption == 0) {
                    content.find("#hide-show-queue").show();
                }
                else {
                    content.find("#hide-show-queue").hide();
                }
                content.find(".queue-option").change(function () {
                    if (this.value == 0) {
                        content.find("#hide-show-queue").show();
                        $(".max-queue-count").val(100);
                        $(".queue-option").attr('title', 'Queue count.');
                    } else if (this.value == 1) {
                        content.find("#hide-show-queue").hide();
                        $(".queue-option").attr('title', 'Queue count until closeshift.');
                    }
                    else if (this.value == 2) {
                        content.find("#hide-show-queue").hide();
                        $(".queue-option").attr('title', 'Queue count until a day.');
                    }
                })

                if (setting.TaxOption == 3) {
                    content.find("#hide-show-tax").show();
                    info.Order.TaxGroupID = setting.Tax;
                } else {
                    info.Order.TaxGroupID = setting.Tax;
                }

                content.find(".tax-option").change(function () {
                    if (this.value == 3) {
                        content.find("#hide-show-tax").show();
                        info.Order.TaxGroupID = setting.Tax;
                    } else {
                        content.find("#hide-show-tax").hide();
                        info.Order.TaxGroupID = 0;
                    }
                })
            }
        });
    }

    /// Save Reciept Lists ///
    this.fxSaveOrdersList = function (target = undefined) {
        __this.highlight(target);
        __poscore.checkPrivilege("P020", function (orderInfo) {
            let dialog = new DialogBox({
                position: "top-center",
                caption: "Save Receipt",
                icon: "fas fa-trash-restore",
                content: {
                    selector: "#save-receipt-content"
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
                    selector: ".save-receipt-listview",
                    visibleFields: ["OrderNo", "CustomerName", "CustomerCode", "DateOut", "TimeOut", "GrandTotal", "Currency"],
                    keyField: "OrderID",
                    paging: {
                        pageSize: 6
                    },
                    columns: [
                        {
                            name: "DateOut",
                            dataType: "date",
                            dataFormat: "MM/DD/YYYY"
                        },
                        {
                            name: "GrandTotal",
                            dataType: "number",
                        },
                        {
                            name: "Currency.Description",
                        }
                    ],
                    actions: [
                        {
                            template: `<i class="fas fa-arrow-circle-down" style="cursor: pointer;"></i>`,
                            on: {
                                "click": function (e) {
                                    chooseReceipt(e.data);
                                    dialog.shutdown();
                                }
                            }
                        },
                        {
                            template: `<i class="fas fa-trash-alt csr-pointer fn-red"></i>`,
                            on: {
                                "click": function (e) {
                                    deleteSavedOrder($listView, e.data);
                                }
                            }
                        }
                    ]
                });

                let $dateFrom = dialog.content.find(".save-date-from");
                let $dateTo = dialog.content.find(".save-date-to");
                //$dateFrom[0].valueAsDate = new Date();
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

                //Search receipts to choose
                dialog.content.find("#search-receipt-save").keyup(function () {
                    $dateFrom.val("");
                    searchReceipts($listView, $dateFrom.val(), __dateTo, this.value);
                });

                dialog.confirm(function () {
                    __this.highlight(target, false);
                    dialog.shutdown();
                });

            });
        });

        function searchReceipts($listView, dateFrom, dateTo, keyword = "") {
            var dtFrom = new Date(dateFrom), dtTo = new Date(dateTo);
            let input = keyword.replace(/\s/g, '');
            let regex = new RegExp(input, "i");
            var _orders = $.grep(__poscore.getOrders(), function (order) {
                if (isEmpty(dateFrom)) {
                    if (!isEmpty(keyword)) {
                        return regex.test(order.OrderNo.replace(/\s/g, ''))
                            || regex.test(order.Currency.Description.toString().replace(/\s/g, ''));
                    } else {
                        return true;
                    }
                }

                var dateOut = new Date(order.DateOut.split("T")[0]);
                return dtFrom <= dateOut && dateOut <= dtTo;
            });
            $listView.clearRows();
            $listView.bindRows(_orders);
        }

        function chooseReceipt(order) {
            __poscore.loadCurrentOrderInfo(order.TableID, order.OrderID, 0, false, true);
        }

        function deleteSavedOrder($listview, order) {
            if (isValidJSON(order)) {
                __poscore.dialog("Delete Order", "Confirm delete order[" + order.OrderNo + "]?", "warning", "ok-cancel")
                    .confirm(function () {
                        $.post(__urls.deleteSavedOrder, {
                            orderId: order.OrderID
                        }, function (message) {
                            if (message.IsApproved) {
                                $listview.removeRow(order.OrderID);
                                __poscore.removeOrder(order.OrderID);
                            }
                            ViewMessage({}, message);
                        });
                        this.meta.shutdown();
                    });
            }
        }
    }

    /// Save Order ///
    this.fxSaveOrder = function (target = undefined) {
        __this.highlight(target);
        __poscore.checkPrivilege("P004", function (posInfo) {
            var __order = $.extend(true, {}, posInfo.Order);
            __poscore.checkCart(function () {
                __poscore.loadScreen();
                saveReceipt(__order, function () {
                    __poscore.loadCurrentOrderInfo(posInfo.Order.TableID, 0, 0, true);
                    __poscore.loadScreen(false);
                });
            });

        });
        function saveReceipt(order, success) {
            $.post(__urls.saveOrder, {
                order: JSON.stringify(order)
            }, success);
        }
    }

    this.fxChooseItems = function (isFromCanring = false, canringTable = {}, canringData = {}) {
        __poscore.fallbackInfo(function (orderInfo) {
            var $dialog = new DialogBox({
                caption: "List of items for sale",
                icon: "fas fa-tag",
                content: {
                    selector: "#group-item-listview"
                },
                button: {
                    ok: {
                        text: "Close",
                        callback: function () {
                            $dialog.shutdown();
                        }
                    }
                }
            });

            $dialog.invoke(function () {
                $dialog.content.find(".group-search-boxes").removeClass("hidden");
                var $itemListView = ViewTable({
                    selector: "#list-sale-items",
                    keyField: "ID",
                    dynamicCol: {
                        afterAction: true,
                        headerContainer: "#item-col-append",
                    },
                    visibleFields: ["Code", "KhmerName", "UnitPrice", "UoM", "InStock",],
                    columns: [
                        {
                            name: "AddictionProps",
                            valueDynamicProp: "ValueName",
                            dynamicCol: true,
                        },
                        {
                            name: "InStock",
                            dataType: "number",
                            dataFormat: { fixed: 0 }
                        }
                    ],
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down fn-sky fa-lg csr-pointer'></i>",
                            on: {
                                "click": function (e) {
                                    if (isFromCanring) {
                                        if (canringTable && isValidJSON(canringData)) {
                                            const uoms = __poscore.filterUoMs(e.data.GroupUomID, e.data.UomID);
                                            const uom = uoms.filter(i => i.Selected)[0] ?? {}
                                            canringData.ItemChangeName = e.data.KhmerName
                                            canringData.ItemChangeID = e.data.ItemID
                                            canringData.UomChangeLists = uoms
                                            canringData.UomChangeID = uom.Value
                                            canringTable.updateRow(canringData)
                                        }
                                        $dialog.shutdown()
                                    }
                                    else __poscore.changeItemQty(e.key, e.data.Qty);
                                }
                            }
                        },
                    ],
                    paging: {
                        enabled: true
                    }
                });
                if (isValidArray(orderInfo.SaleItems)) {
                    $itemListView.clearHeaderDynamic(orderInfo.SaleItems[0]["AddictionProps"])
                    $itemListView.createHeaderDynamic(orderInfo.SaleItems[0]["AddictionProps"])
                }

                $itemListView.bindRows(orderInfo.SaleItems);

                let $inputSearch = $dialog.content.find("#search-sale-items");
                $inputSearch.on("keyup", function () {
                    $itemListView.clearRows();
                    var searcheds = searchSaleItems(orderInfo.SaleItems, this.value);
                    $itemListView.bindRows(searcheds);
                });
            });
        });

        function searchSaleItems(saleItems, keyword = "") {
            let input = keyword.replace(/\s/g, '');
            let regex = new RegExp(input, "i");
            var filtereds = $.grep(saleItems, function (item, i) {
                return regex.test(item.KhmerName.replace(/\s/g, ''))
                    || regex.test(item.Code.replace(/\s/g, ''))
                    || regex.test(item.UnitPrice.toString().replace(/\s/g, ''))
                    || regex.test(item.UoM.replace(/\s/g, ''));
            });
            return filtereds;
        }
    }

    this.fxVoidItem = function (target = undefined) {
        __array.length = 0;
        __this.highlight(target);
        __poscore.checkPrivilege("P030", function (posInfo) {
            var __order = $.extend(true, {}, posInfo.Order);
            __poscore.checkCart(function () {
                if (__order.OrderID <= 0) {
                    __poscore.dialog("Void Item", "Please send before voiding item!");
                    return;
                }
                let dialog = new DialogBox({
                    caption: "Void Item",
                    type: "ok-cancel",
                    icon: "fas fa-broom fa-fw",
                    content: {
                        selector: "#void-item"
                    },
                    button: {
                        ok: {
                            text: "Apply",

                        }
                    },

                });

                dialog.invoke(function () {
                    var $listView = ViewTable({
                        selector: "#void-item-listview",
                        keyField: "LineID",
                        visibleFields: ["Code", "KhmerName", "Qty", "UnitPrice", "Uom", "DiscountRate", "Total", "IsVoided"],
                        paging: {
                            enabled: false
                        },
                        dynamicCol: {
                            afterAction: true,
                            headerContainer: "#void-item-col-append",
                        },
                        columns: [
                            {
                                name: "IsVoided",
                                template: "<input type='checkbox'>",
                                on: {
                                    "change": function (e) {
                                        e.data.IsVoided = $(this).prop("checked");
                                        e.data.PrintQty = e.data.Qty * -1;
                                        $listView.updateColumn(e.key, "IsVoided", e.data.IsVoided);
                                        $listView.updateColumn(e.key, "PrintQty", e.data.PrintQty);

                                    }
                                }
                            },
                            {
                                name: "Qty",
                                template: "<input type='text'>",
                                on: {
                                    "keyup": function (e) {
                                        if (__poscore.toNumber(this.value) == 0) {
                                            this.value = e.data.Qty;
                                        }
                                        $listView.updateColumn(e.key, "Qty", __poscore.toNumber(this.value));

                                    }
                                }
                            },

                            {
                                name: "AddictionProps",
                                valueDynamicProp: "ValueName",
                                dynamicCol: true,
                            }
                        ]

                    });

                    __poscore.bindLineItems(__order[__config.detail.fieldName], $listView);
                    dialog.confirm(function () {
                        $(".void-item-reason").val("");
                        $("#void-item-listview").val("");
                        var prs = $.grep($listView.yield(), function (pr) {
                            return pr.IsVoided;
                        });
                        if (!isValidArray(prs)) {
                            __poscore.dialog("Selection", "Please select at least one Void Item.")
                                .confirm(function () {
                                    this.meta.shutdown();
                                });
                        }
                        else {
                            let $dialogReason = new DialogBox({
                                content: {
                                    selector: "#void-item-content",

                                },
                                caption: "Void Item",
                                type: "ok-cancel"
                            });
                            $dialogReason.confirm(function () {
                                __order.Reason = $dialogReason.content.find(".void-item-reason").val();
                                if (__order.Reason == "" || __order.Reason == undefined) {
                                    $dialogReason.content.find(".void-item-error").text("Reason is required.");
                                } else {
                                    __poscore.updateOrder(__order);
                                    __poscore.voidItem(function (status) {
                                        if (status === true) {
                                            __poscore.loadCurrentOrderInfo(__order.TableID, __order.OrderID);
                                            dialog.shutdown();
                                        }
                                        else {
                                            __poscore.dialog("Void Item ", "Please get authorization from administrator to cancel...!", "warning");
                                        }
                                    });

                                    $dialogReason.shutdown();
                                }
                                __this.highlight(target, false);
                                __array.length = 0;

                            });
                            $dialogReason.reject(function () {
                               $(".void-item-reason").val("");
                                __array.length = 0;
                                $dialogReason.shutdown();
                            });
                        }
                        //========================Void Reason========================
                        $("#btn_addvoidreson").click(function () {
                            $(".reason").focus();
                            dataobj.ID = 0;
                            dataobj.Reason = $(".reason").val();
                            $.ajax({
                                url: "/POS/AddVoidReason",
                                type: "POST",
                                dataType: "JSON",
                                data: { data: dataobj },
                                success: function (respones) {
                                    $(".reason").val("");
                                    $.ajax({
                                        url: "/POS/GetVoidReason",
                                        type: "get",
                                        dataType: "JSON",
                                        success: function (data) {
                                            VoiditemsReason(data)
                                        }
                                    });
                                    if (respones.IsApproved) {
                                        new ViewMessage({
                                            summary: {
                                                selector: "#error-summary"
                                            },
                                        }, respones);
                                    } else {
                                        new ViewMessage({
                                            summary: {
                                                selector: "#error-summary"
                                            }
                                        }, respones);
                                    }
                                }
                            });
                            // dialog.preventConfirm(true);
                        });
                        $("#btn_addnewvoidreson").click(function () {
                            $("#btn_addvoidreson").show();
                            $("#btn_updatevoidreson").css("visibility", "hidden");
                            $("#btn_addnewvoidreson").css("visibility", "hidden");
                            $(".reason").css("width", "85%")
                            $(".reason").focus();
                        })
                        const dataobj = {
                            ID: 0,
                            Reason: "",
                            Delete: false
                        }

                        $("#btn_updatevoidreson").click(function () {
                            $(".reason").focus();
                            dataobj.ID = parseInt($(".idreason").val());
                            dataobj.Reason = $(".reason").val();
                            $.ajax({
                                url: "/POS/AddVoidReason",
                                type: "POST",
                                dataType: "JSON",
                                data: { data: dataobj },
                                success: function (respones) {
                                    $(".reason").val("");
                                    $.ajax({
                                        url: "/POS/GetVoidReason",
                                        type: "get",
                                        dataType: "JSON",
                                        success: function (data) {
                                            VoiditemsReason(data)
                                        }
                                    });
                                    if (respones.IsApproved) {
                                        new ViewMessage({
                                            summary: {
                                                selector: "#error-summary"
                                            },
                                        }, respones);
                                    } else {
                                        new ViewMessage({
                                            summary: {
                                                selector: "#error-summary"
                                            }
                                        }, respones);
                                    }
                                }
                            });
                            // dialog.preventConfirm(true);
                        });
                        $("#void-item-listview").val("");
                        $.ajax({
                            url: "/POS/GetVoidReason",
                            type: "get",
                            dataType: "JSON",
                            success: function (data) {
                                VoiditemsReason(data)
                            }
                        });
                        $(".void-item-reason").html("");

                    });
                    dialog.reject(function () {
                        $(".void-item-reason").html("");
                        dialog.shutdown();
                        __array = [];
                        __this.highlight(target, false);
                    });
                });


            });
        });
    }
    //====================VoideItems Reasons   =====================
    function VoiditemsReason(data) {
        const __listreasons = ViewTable({
            keyField: "ID",
            selector: "#void-reasons",
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["Reason"],
            actions: [
                {
                    template: `<i class="fas fa-edit hover text-center"></i>`,
                    on: {
                        "click": function (e) {
                            $("#btn_addvoidreson").hide();
                            $("#btn_updatevoidreson").css("visibility", "visible");
                            $("#btn_addnewvoidreson").css("visibility", "visible");
                            $(".reason").focus();
                            $(".idreason").val(e.data.ID);
                            $(".reason").val(e.data.Reason);
                            $(".reason").css("width", "57%")
                        }
                    },
                },
                {
                    template: ` <i class="fas fa-trash-alt csr-pointer fn-red"></i>`,
                    on: {
                        "click": function (e) {
                            $.ajax({
                                url: "/POS/DeleteReason",
                                type: "get",
                                dataType: "JSON",
                                data: { id: e.data.ID },
                                success: function (data) {
                                    // __listreasons.bindRows(data);
                                    __listreasons.removeRow(e.key);
                                }
                            });
                        }
                    },
                },
                {
                    template: ` <i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>`,
                    on: {
                        "click": function (e) {
                            // __array.push(e.data)
                            // let newArray = [];
                            // let uniqueObject = {};
                            // for (let i in __array) {
                            //     uniqueObject[__array[i]['ID']] = __array[i];
                            // }
                            // for (let i in uniqueObject) {
                            //     newArray.push(uniqueObject[i]);
                            // }
                            // $(".void-item-reason").html("");
                            // newArray.forEach(s => {
                            //     $(".void-item-reason").append(" -" + s.Reason);
                            // })
                            let txtarea = $(".void-item-reason").val();
                            if (txtarea == "") {
                                __array = [];
                            }
                            $(".void-item-reason").val(txtarea + " " + e.data.Reason);

                        }
                    },
                }
            ]
        });
        __listreasons.bindRows(data);
    }
    //=======================void order items=========================
    function VoidorderReason(data) {
        const __listreasons = ViewTable({
            keyField: "ID",
            selector: "#void-reasons",
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["Reason"],
            actions: [
                {

                    template: `<i class="fas fa-edit hover text-center"></i>`,
                    on: {
                        "click": function (e) {
                            $("#btn_addvoidreson").hide();
                            $("#btn_updatevoidreson").css("visibility", "visible");
                            $("#btn_addnewvoidreson").css("visibility", "visible");
                            $(".reason").focus();
                            $(".idreason").val(e.data.ID);
                            $(".reason").val(e.data.Reason);
                            $(".reason").css("width", "57%")
                        }
                    },
                },
                {
                    template: ` <i class="fas fa-trash-alt csr-pointer fn-red"></i>`,
                    on: {
                        "click": function (e) {
                            $.ajax({
                                url: "/POS/DeleteReason",
                                type: "get",
                                dataType: "JSON",
                                data: { id: e.data.ID },
                                success: function (data) {
                                    __listreasons.removeRow(e.key);
                                }
                            });
                        }
                    },
                },
                {
                    template: ` <i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>`,
                    on: {
                        "click": function (e) {
                            // __array.push(e.data)
                            // let newArray = [];
                            // let uniqueObject = {};
                            // for (let i in __array) {
                            //     uniqueObject[__array[i]['ID']] = __array[i];
                            // }
                            // for (let i in uniqueObject) {
                            //     newArray.push(uniqueObject[i]);
                            // }
                            // $(".void-order-reason").html("");
                            // newArray.forEach(s => {
                            //     $(".void-order-reason").append(" -" + s.Reason);
                            // })
                            let txtarea = $(".void-order-reason").val();
                            if (txtarea == "") {
                                __array = [];
                            }
                            $(".void-order-reason").val(txtarea + " " + e.data.Reason);

                        }
                    },
                }

            ]
        });
        __listreasons.bindRows(data);
    }
    //====================================cancel receipt reason====================
    function CancelReceiptReason(data) {
        const __listreasons = ViewTable({
            keyField: "ID",
            selector: "#void-reasons",
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["Reason"],
            actions: [
                {

                    template: `<i class="fas fa-edit hover text-center"></i>`,
                    on: {
                        "click": function (e) {
                            $("#btn_addvoidreson").hide();
                            $("#btn_updatevoidreson").css("visibility", "visible");
                            $("#btn_addnewvoidreson").css("visibility", "visible");
                            $(".reason").focus();
                            $(".idreason").val(e.data.ID);
                            $(".reason").val(e.data.Reason);
                            $(".reason").css("width", "57%")
                        }
                    },
                },
                {
                    template: ` <i class="fas fa-trash-alt csr-pointer fn-red"></i>`,
                    on: {
                        "click": function (e) {
                            $.ajax({
                                url: "/POS/DeleteReason",
                                type: "get",
                                dataType: "JSON",
                                data: { id: e.data.ID },
                                success: function (data) {
                                    __listreasons.removeRow(e.key);
                                }
                            });
                        }
                    },
                },
                {
                    template: ` <i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>`,
                    on: {
                        "click": function (e) {
                            let txtarea = $(".cancel-receipt-reason").val();
                            if (txtarea == "") {
                                __array = [];
                            }
                            // __array.push(e.data);
                            // let newArray = [];
                            // let uniqueObject = {};
                            // // let textarea = "";
                            // for (let i in __array) {
                            //     uniqueObject[__array[i]['ID']] = __array[i];
                            // }
                            // for (let i in uniqueObject) {
                            //     newArray.push(uniqueObject[i]);
                            // }
                            // // $(".cancel-receipt-reason").val("");
                            // $(".cancel-receipt-reason").val("");


                            // newArray.forEach(s => {
                            //     txtarea += "-" + s.Reason;
                            // });
                            // console.log(" Data=", uniqueObject);
                            $(".cancel-receipt-reason").val(txtarea + " " + e.data.Reason);
                        }
                    },
                }

            ]
        });
        __listreasons.bindRows(data);

    }
    //=======================return receipt reason====================
    function ReturnReceiptReason(data) {
        const __listreasons = ViewTable({
            keyField: "ID",
            selector: "#void-reasons",
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["Reason"],
            actions: [
                {
                    template: `<i class="fas fa-edit hover text-center"></i>`,
                    on: {
                        "click": function (e) {
                            $("#btn_addvoidreson").hide();
                            $("#btn_updatevoidreson").css("visibility", "visible");
                            $("#btn_addnewvoidreson").css("visibility", "visible");
                            $(".reason").focus();
                            $(".idreason").val(e.data.ID);
                            $(".reason").val(e.data.Reason);
                            $(".reason").css("width", "57%")
                        }
                    },
                },
                {
                    template: ` <i class="fas fa-trash-alt csr-pointer fn-red"></i>`,
                    on: {
                        "click": function (e) {
                            $.ajax({
                                url: "/POS/DeleteReason",
                                type: "get",
                                dataType: "JSON",
                                data: { id: e.data.ID },
                                success: function (data) {
                                    __listreasons.removeRow(e.key);
                                }
                            });
                        }
                    },
                },
                {
                    template: ` <i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>`,
                    on: {
                        "click": function (e) {
                            let txtarea = $(".return-receipt-reason").val();
                            if (txtarea == "") {
                                __array = [];
                            }
                            // __array.push(e.data)
                            // let newArray = [];
                            // let uniqueObject = {};
                            // for (let i in __array) {
                            //     uniqueObject[__array[i]['ID']] = __array[i];
                            // }
                            // for (let i in uniqueObject) {
                            //     newArray.push(uniqueObject[i]);
                            // }
                            // $(".return-receipt-reason").html("");
                            // newArray.forEach(s => {
                            //     $(".return-receipt-reason").append(" -" + s.Reason);
                            // })
                            $(".return-receipt-reason").val(txtarea + " " + e.data.Reason);

                        }
                    },
                }

            ]
        });
        __listreasons.bindRows(data);

    }
    function PendingVoidItemReason(data) {
        const __listreasons = ViewTable({
            keyField: "ID",
            selector: "#void-reasons",
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["Reason"],
            actions: [
                {
                    template: `<i class="fas fa-edit hover text-center"></i>`,
                    on: {
                        "click": function (e) {
                            $("#btn_addvoidreson").hide();
                            $("#btn_updatevoidreson").css("visibility", "visible");
                            $("#btn_addnewvoidreson").css("visibility", "visible");
                            $(".reason").focus();
                            $(".idreason").val(e.data.ID);
                            $(".reason").val(e.data.Reason);
                            $(".reason").css("width", "57%")
                        }
                    },
                },
                {
                    template: ` <i class="fas fa-trash-alt csr-pointer fn-red"></i>`,
                    on: {
                        "click": function (e) {
                            $.ajax({
                                url: "/POS/DeleteReason",
                                type: "get",
                                dataType: "JSON",
                                data: { id: e.data.ID },
                                success: function (data) {
                                    __listreasons.removeRow(e.key);
                                }
                            });
                        }
                    },
                },
                {
                    template: ` <i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>`,
                    on: {
                        "click": function (e) {
                            let txtarea = $(".pending-void-item-reason").val();
                            if (txtarea == "") {
                                __array = [];
                            }
                            // __array.push(e.data)
                            // let newArray = [];
                            // let uniqueObject = {};
                            // for (let i in __array) {
                            //     uniqueObject[__array[i]['ID']] = __array[i];
                            // }
                            // for (let i in uniqueObject) {
                            //     newArray.push(uniqueObject[i]);
                            // }
                            // $(".return-receipt-reason").html("");
                            // newArray.forEach(s => {
                            //     $(".return-receipt-reason").append(" -" + s.Reason);
                            // })
                            $(".pending-void-item-reason").val(txtarea + " " + e.data.Reason);

                        }
                    },
                }

            ]
        });
        __listreasons.bindRows(data);

    }

    //=====================dialog choose items =================
    this.fxPendingVoidItem = function (target = undefined) {
        __this.highlight(target);
        __poscore.checkPrivilege("P004", function (posInfo) {
            var __order = $.extend(true, {}, posInfo.Order);
            __poscore.checkCart(function () {
                if (__order.OrderID <= 0) {
                    __poscore.dialog("Pending Void Item", "Please send before pending void item!");
                    return;
                }
                let dialog = new DialogBox({
                    caption: "Pending Void Item",
                    type: "ok-cancel",
                    icon: "fas fa-broom fa-fw",
                    content: {
                        selector: "#pending-void-item-send"
                    },
                    button: {
                        ok: {
                            text: "Apply",

                        }
                    },

                });

                dialog.invoke(function () {
                    var $listView = ViewTable({
                        selector: "#pending-void-item-listview",
                        keyField: "LineID",
                        visibleFields: ["Code", "KhmerName", "Qty", "UnitPrice", "Uom", "DiscountRate", "Total", "IsVoided"],
                        paging: {
                            enabled: false
                        },
                        dynamicCol: {
                            afterAction: true,
                            headerContainer: "#pending-void-item-col-append",
                        },
                        columns: [
                            {
                                name: "IsVoided",
                                template: "<input type='checkbox'>",
                                on: {
                                    "change": function (e) {
                                        e.data.IsVoided = $(this).prop("checked");
                                        e.data.PrintQty = e.data.Qty * -1;
                                        $listView.updateColumn(e.key, "IsVoided", e.data.IsVoided);
                                        $listView.updateColumn(e.key, "PrintQty", e.data.PrintQty);

                                    }
                                }
                            },
                            {
                                name: "AddictionProps",
                                valueDynamicProp: "ValueName",
                                dynamicCol: true,
                            }
                        ]
                    });
                    __poscore.bindLineItems(__order[__config.detail.fieldName], $listView);
                    dialog.confirm(function () {
                        var prs = $.grep($listView.yield(), function (pr) {
                            return pr.IsVoided;

                        });
                        if (!isValidArray(prs)) {
                            __poscore.dialog("Selection", "Please select at least one Pending Void Item.")
                                .confirm(function () {
                                    this.meta.shutdown();
                                });
                        }
                        else {
                            __poscore.updateOrder(__order);
                            $("[data-field='TaxGroups']").hide();
                            __poscore.pendingVoidItem(function (status) {
                                if (status === true) {
                                    __poscore.loadCurrentOrderInfo(__order.TableID, __order.OrderID);
                                    dialog.shutdown();
                                }
                                else {
                                    __poscore.dialog("Pending Void Item ", "Please get authorization from administrator to cancel...!", "warning");
                                }
                            });
                            __this.highlight(target, false);
                        }
                        $dialogReason.shutdown();

                    });
                    dialog.reject(function () {
                        dialog.shutdown();
                        __this.highlight(target, false);
                    });
                });
            });
        });
    }
    function bindSelectListItems(selector, jsons) {
        $(selector).children().remove();
        var data = '';
        $.each(jsons, function (i, item) {
            data +=
                '<option value="' + item["Value"] + '">' + item["Text"] + '</option>';
        });
        $(selector).append(data);
    }

    function addVehicles() {
        $.get("/businessPartner/getVehicleComponents", function (resp) {
            bindSelectListItems("#typeid", resp.VehiTypes,);
            bindSelectListItems("#brandid", resp.VehiBrands);
            bindSelectListItems("#modelid", resp.VehiModels);
            bindSelectListItems("#colorid", resp.VehiColors);
        });

        const __vmclist = ViewTable({
            selector: "#list-body-vehicle",
            keyField: "KeyID",
            dataSynced: true,
            visibleFields: [
                "Plate", "Frame", "Engine", "VehiTypes", "VehiBrands", "VehiModels", "Year", "VehiColors"
            ],
            columns: [
                {
                    name: "Plate",
                    template: "<input>"
                },
                {
                    name: "Frame",
                    template: "<input>"
                },
                {
                    name: "Year",
                    template: "<input type='number'>"
                },
                {
                    name: "Engine",
                    template: "<input>",
                },
                {
                    name: "VehiTypes",
                    nameField: "TypeID",
                    template: "<select>"
                },
                {
                    name: "VehiBrands",
                    nameField: "BrandID",
                    template: "<select>"
                },
                {
                    name: "VehiModels",
                    nameField: "ModelID",
                    template: "<select>"
                },

                {
                    name: "VehiColors",
                    nameField: "ColorID",
                    template: "<select>"
                }
            ]
        });

        $("#add-vehicle-row").on("click", function () {
            $.get("/businessPartner/getVehicleComponents", function (resp) {
                let keyID = Date.now();
                resp.KeyID = keyID;
                __vmclist.addRow(resp);
                let colYear = __vmclist.getColumn(resp.KeyID, "Year");
                convertDateToYearPicker($(colYear).children("input"));
                __vmclist.updateColumn(resp.KeyID, "Year", $(colYear).children("input").val());
            });
        });

        $("#create_vehicle").click(function () {
            let dialog = new DialogBox({
                type: 'yes-no',
                button: {
                    yes: {
                        text: "Drop to list",
                    },
                    no: {
                        text: "Close"
                    }
                },
                caption: 'Vehicle Detail',
                content: {
                    selector: "#model_create_v"
                }
            });

            dialog.invoke(function () {
                $("#yearid").yearpicker();
                dialog.confirm(function () {
                    createNewVehicle();
                })
                dialog.reject(function () {
                    clearVehicle();
                    dialog.shutdown()
                })
            })

        });

        return __vmclist;
        function clearVehicle() {
            $("#plateid").val("");
            $("#frameid").val("");
            $("#engineid").val("");
            $("#yearid").val("");
            $("#typeid").val(0);
            $("#brandid").val(0);
            $("#modelid").val(0);
            $("#colorid").val(0);
        }

        function createNewVehicle() {
            let keyID = Date.now();
            let plate = $("#plateid").val();
            let frame = $("#frameid").val();
            let engine = $("#engineid").val();
            let year = $("#yearid").val();
            let typeID = $("#typeid option:selected").val();
            let brandID = $("#brandid option:selected").val();
            let modelID = $("#modelid option:selected").val();
            let colorID = $("#colorid option:selected").val();

            if (plate.trim() == "") {
                error("Please input plate");
                return;
            }
            var item = {
                KeyID: keyID,
                AutoMID: 0,
                Plate: plate,
                Frame: frame,
                Engine: engine,
                TypeID: typeID,
                BrandID: brandID,
                ModelID: modelID,
                Year: year,
                ColorID: colorID
            }

            //masterbp.AutoMobile.push(item);
            $.ajax({
                url: "/BusinessPartner/GetBusDetail",
                type: "POST",
                dataType: "JSON",
                data: { data: item },
                success: function (_detail) {
                    __vmclist.addRow(_detail);
                }
            });
            clearVehicle();
        }

        function error(message) {
            $("#error_val").text(message);
            $("#show-hide").show();
            setTimeout(function () {
                $("#show-hide").hide();
            }, 3000);
        }

        function convertDateToYearPicker(selector, onInput) {
            if (isEmpty(selector)) {
                selector = "[data-year]";
            }
            $(selector).yearpicker();
        }

        function isValidArray(values) {
            return Array.isArray(values) && values.length > 0;
        }

        function updatedata(data, keyField, keyValue, prop, propValue) {
            if (isValidArray(data)) {
                data.forEach(i => {
                    if (i[keyField] === keyValue)
                        i[prop] = propValue
                })
            }
        }
    }

    //=====================add funtion tips==============

    this.fxCustomerTips = function (target = undefined) {
        var customer = {
            ID: 0,
            ReceiptID: 0,
            Amount: 0,
            AltCurrencyID: 0,
            AltCurrency: "",
            AltRate: 0,
            SCRate: 0,
            LCRate: 0,
            BaseCurrency: "",
            BaseCurrencyID: 0,
        }
        let master = [];
        master.push(customer);
        const __customertips = master[0];
        let dialog = new DialogBox({
            caption: "Customer Tip",
            icon: "fas fa-user-friends",
            content: {
                selector: "#empre-content"
            },
            button: {
                ok: {
                    text: "Save"
                }
            },
        });

        dialog.invoke(function () {
            const customers = ViewTable({
                keyField: "ID",
                selector: "#list-customertip",
                indexed: false,
                visibleFields: ["Amount", "Currency"],
                paging: {
                    pageSize: 10,
                    enabled: false
                },
                columns: [
                    {
                        name: "Amount",
                        template: `<input class='form-control font-size unitprice' type='text' />`,
                        on: {
                            "keyup": function (e) {

                                __customertips.Amount = this.value;

                            }
                        }
                    },
                    {
                        name: "Currency",
                        template: `<select></select>`,
                        on: {
                            "change": function (e) {
                                findCurrencyById(parseInt(this.value));


                            }
                        }
                    },
                ],

            });
            $.get("/POS/GetDisplayCurrency", function (resp) {
                __poscore.checkPrivilege("P006", function (posInfo) {
                    const _currency = posInfo.DisplayPayOtherCurrency.filter(c => c.AltCurrencyID == parseInt(resp[0].Currency[0].Value))[0]
                    __customertips.AltCurrencyID = _currency.AltCurrencyID;
                    __customertips.AltCurrency = _currency.AltCurrency;
                    __customertips.AltRate = _currency.AltRate;
                    __customertips.SCRate = _currency.Rate;
                    __customertips.LCRate = _currency.LCRate;
                    __customertips.BaseCurrency = _currency.BaseCurrency;
                    __customertips.BaseCurrencyID = _currency.BaseCurrencyID;
                    __customertips.ReceiptID = _currency.ReceiptID;
                })
                customers.bindRows(resp);

            });

            dialog.confirm(function () {

                $.ajax({
                    url: "/POS/SaveCustomerTips",
                    type: "post",
                    dataType: "JSON",
                    data: $.antiForgeryToken({ _data: JSON.stringify(__customertips) }),
                    success: function (response) {
                    }
                });
                dialog.shutdown();
            });

        });
        function findCurrencyById(currencyId) {
            __poscore.checkPrivilege("P006", function (posInfo) {
                const _currency = posInfo.DisplayPayOtherCurrency.filter(c => c.AltCurrencyID == parseInt(currencyId))[0]
                __customertips.AltCurrencyID = _currency.AltCurrencyID;
                __customertips.AltCurrency = _currency.AltCurrency;
                __customertips.AltRate = _currency.AltRate;
                __customertips.SCRate = _currency.Rate;
                __customertips.LCRate = _currency.LCRate;
                __customertips.BaseCurrency = _currency.BaseCurrency;
                __customertips.BaseCurrencyID = _currency.BaseCurrencyID;
                __customertips.ReceiptID = _currency.ReceiptID;


                return posInfo.DisplayPayOtherCurrency.filter(c => c.AltCurrencyID == parseInt(currencyId))[0];
            })

        }
    }
    //====================end function tips=============
}
