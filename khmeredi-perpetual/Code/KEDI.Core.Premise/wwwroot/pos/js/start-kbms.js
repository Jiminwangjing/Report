"use strict";
$(document).ready(function () {  
    const posCore = new PosCore({
        services: ["PosUtils", "PosPromotion", "PosPayment", "CardMemberUtility", "CanRingExchange","ReportSale"],
        orderOnly: true,
        detail: {
            fieldName: "OrderDetail"
        }
    }).start();
    var utils = posCore.instance["PosUtils"];
    var canring = posCore.instance["CanRingExchange"];
    posCore.startSignalR();
    
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

    $("#fx-choose-customer").click(function (e) {
        utils.fxChooseCustomer();
    });

    $("#fx-choose-items").click(function (e) {
        utils.fxChooseItems();
    });
    $("#fx-remark").on("click", function () {
        utils.fxRemark(this);
    });
    //can ring 

    $(".can-ring").on("click", function () {
        canring.fxShowHideCanRing(".container-buy-can-ring", "#panel");
    });
    $("#fx-choose-can-ring").click(function () {
        canring.fxCanringListDialog();
    });
    $("#fx-can-ring-report").click(function () {
        canring.fxCanringReportListDialog();
    });
    $("#fx-buy-can-ring").click(function () {
        canring.gotoPanelPaymentCanRing();
    });
    $("#fx-choose-customer-can-ring").click(function (e) {
        utils.fxChooseCustomer();
    });
});
