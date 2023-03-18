"use strict";
$(document).ready(function () {  
    const posCore = new KPOS();
    POSPayment(posCore);
    var utils = POSUtils(posCore);
    posCore.fetchOrders(0);
    //Main Menu
    $("#open-shift").click(function (e) {
        utils.fxOpenShift();
    });

    $("#close-shift").click(function (e) {
        utils.fxCloseShift();
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

    //Discount order detail
    $("#fx-discount-item").click(function (e) {
        utils.fxDiscountItem();
    });

    //Discount order
    $('#btn-dis-total').click(function () {
        utils.fxDiscountTotal();
    });

    //Discount order
    $('#fx-move-table').click(function () {
        utils.fxMoveTable();
    });

    //Void order
    $("#fx-void-order").click(function (e) {
        utils.fxVoidOrder();
    });

    //Combine receipts
    $("#fx-combine-receipt").click(function (e) {
        utils.fxCombineReceipts();
    });

    $("#fx-edit-price").click(function (e) {
        utils.fxEditPrice();
    });
});
