"use strict";
$(document).ready(function () {
    const posCore = new KPOS({
        orderOnly: true,
        detail: {
            fieldName: "OrderDetailQAutoMs"
        }
    });
    POSPayment(posCore);
    var utils = POSUtils(posCore);
    var utilsKVMS = UtilsKVMS(posCore);
    posCore.fetchOrders(1);

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

    //VMC Edition
    $("#create-customer").on("click", function () {
        utilsKVMS.fxCreateCustomer();
    });

    $("#list-customer").on("click", function () {
        utilsKVMS.fxListCustomer();
    });

    $("#create_new_vehicle").on("click", function () {
        utilsKVMS.fxCreateNewVehicle();
    });

    $('.change-customer').click(function () {
        utilsKVMS.fxChangeCustomerPOS();
    });

    //Save Quotation
    $("#fx-save-quotation").click(function () {
        utilsKVMS.fxSaveQuotation();
    });

    //List Quotation
    $("#fx-list-quotation").click(function () {
        utilsKVMS.fxListQuotation();
    });
    //List Invoice
    $("#fx-list-invoice").click(function () {
        utilsKVMS.fxListInvoice();
    });

    //Clear order
    $("#fx-clear-order").click(function (e) {
        posCore.resetOrder();
    });

    //Discount order detail
    $("#fx-discount-item").click(function (e) {
        utils.fxDiscountItem();
    });

    //Return Invoice
    $("#fx-return-receipts").click(function () {
        utils.fxReturnReceipts(this);
    });

    //Discount order
    $('#fx-discount-total').click(function () {
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