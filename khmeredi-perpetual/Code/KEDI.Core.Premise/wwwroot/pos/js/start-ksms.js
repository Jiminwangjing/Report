"use strict";
$(document).ready(function () {
    const posCore = PosCore({
        services: ["PosKSMS", "PosUtils", "PosPromotion", "PosPayment", "CardMemberUtility", "ReportSale"],
        orderOnly: false,
        detail: {
            fieldName: "OrderDetail"
        }
    }).start();

    var utils = posCore.instance["PosUtils"];
    var ksms = posCore.instance["PosKSMS"];
    posCore.startSignalR(function () {
        this.on("tick", function (timeTables) {
            posCore.changeStatusTimeTableSignalR(timeTables);
        });

        this.on("changeStatusTimeTables", function (timeTables) {
            posCore.changeStatusTimeTableSignalR(timeTables);
        });
    });
    //Main Menu
    $("#open-shift").click(function (e) {
        utils.fxOpenShift(function () {
            posCore.loadScreen(false);
        });
    });

    $("#close-shift").click(function (e) {
        utils.fxCloseShift(function (model) {
            if (model.IsApproved) {
                $.get("/Account/Logout", function () {
                    posCore.loadScreen(false);
                    location.reload();
                });
            }
        });
    });

    $("#goto-reprint").click(function (e) {
        utils.fxReprint();
    });

    $("#goto-setting").on("click", function () {
        utils.fxSetting();
    });

    $("#change-rate").on("click", function () {
        utils.fxChangeRate();
    });

    //Util functions
    $("#send").on("click", function () {
        utils.fxSendOrder();
    });

    $("#bill").on("click", function () {
        utils.fxBillOrder();
    });

    $(".fx-cancel-receipts").on("click", function () {
        utils.fxCancelReceipts();
    });

    $(".fx-return-receipts").on("click", function () {
        utils.fxReturnReceipts();
    });

    $(".fx-save-order-lists").on("click", function () {
        utils.fxSaveOrdersList();
    });

    //Discount order detail
    $("#fx-discount-item").click(function (e) {
        utils.fxDiscountItem();
    });

    //Discount order
    $("#fx-discount-total").click(function () {
        utils.fxDiscountTotal();
    });

    //Discount order
    $('#fx-move-table').click(function () {
        utils.fxMoveTable();
    });

    $("#fx-clear-order").on("click", function () {
        utils.fxClearOrder();
    });

    //Void order
    $("#fx-void-order").click(function (e) {
        utils.fxVoidOrder();
    });

    //Combine receipts
    $("#fx-combine-receipt").click(function (e) {
        utils.fxCombineReceipts();
    });

    $("#fx-split-receipt").click(function (e) {
        utils.fxSplitReceipts();
    });

    $("#fx-edit-price").click(function (e) {
        utils.fxEditPrice();
    });
    $("#fx-save-orders").click(function (e) {
        utils.fxSaveOrder();
    });
    $("#fx-discount-membercard").click(function (e) {
        utils.fxDiscountMember();
    });

    $("#fx-choose-customer").click(function () {
        utils.fxChooseCustomer(true);
    });
    $("#fx-choose-customer-ks").click(function () {
        ksms.fxChooseCustomer();
    });

    $("#fx-choose-vehicle").click(function () {
        utils.fxVehicleDialog();
    });

    $("#fx-choose-items").click(function (e) {
        utils.fxChooseItems();
    });
    $("#ks-service").click(function (e) {
        ksms.fxGetKSService();
    });
    $(".use-service").click(function (e) {
        ksms.fxShowHideUseService(".container-use-service", "#panel");
    });
    $("#fx-choose-ks").click(function () {
        ksms.fxServiceDialog();
    });
    $("#fx-use-service").click(function () {
        ksms.fxUseService();
    });
    $("#fx-remark").on("click", function () {
        utils.fxRemark(this);
    });

    //KSMS Report
    const date = moment();
    $("#from-date").val(formatDate(date, "YYYY-MM-DD"));
    $("#to-date").val(formatDate(date, "YYYY-MM-DD"));
    $("#used-from-date").val(formatDate(date, "YYYY-MM-DD"));
    $("#used-to-date").val(formatDate(date, "YYYY-MM-DD"));

    const fromDate = $("#from-date").val();
    const toDate = $("#to-date").val();
    const usedfromDate = $("#used-from-date").val();
    const usedtoDate = $("#used-to-date").val();
    $("#ks-service-report").click(function () {
        ksms.getSaleReport(fromDate, toDate);
        //ksms.getUsedServiceReport(usedfromDate, usedtoDate);
    });

    $("#from-date").change(function () {
        ksms.getSaleReport(this.value, toDate);
    });

    $("#to-date").change(function () {
        ksms.getSaleReport(fromDate, this.value);
    });

    function formatDate(value, format) {
        let dt = new Date(value),
            objFormat = {
                MM: ("0" + (dt.getMonth() + 1)).slice(-2),
                DD: ("0" + dt.getDate()).slice(-2),
                YYYY: dt.getFullYear()
            },
            dateString = "";

        let dateFormats = format.split("-");
        for (let i = 0; i < dateFormats.length; i++) {
            dateString += objFormat[dateFormats[i]];
            if (i < dateFormats.length - 1) {
                dateString += "-";
            }
        }

        return dateString;
    }
});
