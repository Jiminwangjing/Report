"use strict";
$(document).ready(function () {
    const posCore = PosCore({
        services: ["PosUtils", "PosKSMS", "PosPromotion", "PosPayment", "ReportSale"],
        orderOnly: true,
        detail: {
            fieldName: "OrderDetail"
        }
    }).start();
    var utils = posCore.instance["PosUtils"];
    //Main Menu
    $("#open-shift").click(function (e) {
        utils.fxOpenShift();
    });

    $("#close-shift").click(function (e) {
        utils.fxCloseShift(function () {
            $.get("/Account/Logout", function () {
                location.reload();
            });
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

    $("#fx-discount-membercard").click(function (e) {
        utils.fxDiscountMember();
    });

    $("#fx-choose-customer").click(function (e) {
        utils.fxChooseCustomer();
    });

    $("#fx-choose-items").click(function (e) {
        utils.fxChooseItems();
    });
});
