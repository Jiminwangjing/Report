"use strict";
$(document).ready(function () {
    const posCore = new PosCore({
        services: ["PosUtils", "PosPromotion", "PosPayment", "CardMemberUtility", "ReportSale"],
        detail: {
            fieldName: "OrderDetail"
        }
    }).start();

    var utils = posCore.instance["PosUtils"];
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
        utils.fxCloseShift(function () {
            $.get("/Account/Logout", function () {
                location.reload();
                posCore.loadScreen(false);
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
        utils.fxCancelReceipts(this);
    });

    $(".fx-return-receipts").on("click", function () {
        utils.fxReturnReceipts(this);
    });

    //Discount order detail
    $("#fx-discount-item").click(function (e) {
        utils.fxDiscountItem(this);
    });

    //Discount order
    $("#fx-discount-total").click(function () {
        utils.fxDiscountTotal(this);
    });

    //Move receipt
    $('#fx-move-receipt').click(function () {
        utils.fxMoveReceipt(this);
    });

    //Move table
    $('#fx-move-table').click(function () {
        utils.fxMoveTable(this);
    });

    //Void order
    $("#fx-void-order").click(function (e) {

        utils.fxVoidOrder(this);
    });

    //Combine receipts
    $("#fx-combine-receipt").click(function (e) {
        utils.fxCombineReceipts(this);
    });

    $("#fx-split-receipt").click(function (e) {
        utils.fxSplitReceipts(this);
    });

    $("#fx-edit-price").click(function (e) {
        utils.fxEditPrice(this);
    });

    $("#fx-add-new-item").click(function (e) {
        utils.fxAddNewItem(this);
    });

    $("#fx-discount-membercard").click(function (e) {
        utils.fxDiscountMember(this);
    });

    $("#fx-remark").on("click", function () {
        utils.fxRemark(this);
    });

    $("#fx-choose-items").click(function (e) {
        utils.fxChooseItems();
    });

    $("#fx-choose-customer").click(function (e) {
        utils.fxChooseCustomer();
    });

    $("#fx-void-item").click(function (e) {

        utils.fxVoidItem(this);
    });
    $("#fx-pending-void-item").click(function (e) {
        utils.fxPendingVoidItem(this);
    });
    $("#fx-customer-tips").click(function (e) {
        utils.fxCustomerTips(this);
    });
});
