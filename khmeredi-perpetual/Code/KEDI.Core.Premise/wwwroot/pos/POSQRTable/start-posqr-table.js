$(document).ready(function () {
    var poscore = PosCore({
        detail: {
            keyField: "",
            fieldName: "OrderDetail"
        }
    });
    
    const __this = this;
    const pathname = window.location.pathname.split("/");
    const name = pathname[pathname.length - 1];
    $("#loadscreen").hide();
    poscore.gotoPanelItemOrder();
    poscore.fetchOrderInfoQR(name, 0, true);
    $("#send").click(function () {
        poscore.sendOrderQR();
    });
    $("#bill").click(function () {
        poscore.billOrderQR();
    });
    const wWidth = window.innerWidth;
    if (wWidth > 770) {
        $("#lang-right-panel").prop("hidden", true);
        $("#hide-all").prop("hidden", true);
    }
    $("#fx-remark").on("click", function () {
        __this.fxRemark(this);
    });
    // add Remark
    this.fxRemark = function (target = undefined) {
        __this.highlight(target);
        poscore.checkPrivilegeQR("P019", function (info) {
            poscore.checkCart(function () {
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
    this.highlight = function (target, enabled = true) {
        if (target) {
            if (enabled) {
                $(target).addClass("active").siblings().removeClass("active");
            } else {
                $(target).removeClass("active");
            }
        }
    }
});