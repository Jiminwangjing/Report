"use strict";
//paymentterms
function PaymentTerm() {
    var __this = this;
    var __paymentTerm = {};
    this.fetch = function (paymentTermId, onSuccess) {
        $.get("/BusinessPartner/FetchPaymentTerm",
            {
                paymentTermId: paymentTermId
            },
            function (paymentTerm) {
                __paymentTerm = $.extend(true, __paymentTerm, paymentTerm);
                if (typeof onSuccess === "function") {
                    onSuccess.call(__this, __paymentTerm);
                }
            }
        )
    }

    this.submit = function (paymentTerm) {
        __paymentTerm = $.extend(true, __paymentTerm, paymentTerm);
        if (isValidJson(__paymentTerm)) {
            $.post("/BusinessPartner/InsertEditPayment",
                {
                    paymentTerms: __paymentTerm
                },
                function () {
                   
                    //$("#paymentid option:not(:first-child)").remove();
                    //    $.each(res, function (i, item) {
                    //        $("#paymentid").append("<option selected  value=" + item.ID + ">" + item.Code + "</option>");
                    //    });
                    $.get("/BusinessPartner/getPaymentEditCode", function (res) {
                        $("#paymentid option:not(:first-child)").remove();
                        $.each(res, function (i, item) {
                            $("#paymentid").append("<option selected  value=" + item.ID + ">" + item.Code + "</option>");
                        });

                    });
                });
        }
    }

    this.update = function (key, value) {
        __paymentTerm[key] = value;
    }

    this.access = function (callback) {
        if (typeof callback === "function") {
            callback.call(__this, __paymentTerm);
        }
    }

    function isValidJson(json) {
        return json !== undefined && json.constructor === Object && Object.getOwnPropertyNames(json).length > 0;
    }
}

//instaillment
function Instaillment() {
    var __this = this;
    var __instaillment = {};
    this.fetch = function (instaillmentId, onSuccess) {
        $.get("/BusinessPartner/FetchInstaillment",
            {
                instaillmentId: instaillmentId
            },
            function (instaillment) {
                __instaillment = $.extend(true, __instaillment, instaillment);
                if (typeof onSuccess === "function") {
                    onSuccess.call(__this, __instaillment);
                }
            }
        )
    }
    this.submitInstaillment = function () {
        //__instaillment = $.extend(true, __instaillment, instaillment);
        if (isValidJson(__instaillment)) {
            $.post("/BusinessPartner/InsertEditinstallment",
                {
                    obj: __instaillment
                },
                function () { });
        }
    }

    this.update = function (key, value) {
        __instaillment[key] = value;
   
    }
    function isValidJson(json) {
        return json !== undefined && json.constructor === Object && Object.getOwnPropertyNames(json).length > 0;
    }
    //end instaillment

}


//Discount
function Discount() {
    var __this = this;
    var __discount = {};
    this.fetch = function (discountId, onSuccess) {
        $.get("/BusinessPartner/FetchDiscount",
            {
                discountId: discountId
               
            },
            function (discount) {
                __discount = $.extend(true, __discount, discount);
                if (typeof onSuccess === "function") {
                    onSuccess.call(__this, __discount);
                }
            }
        )
    }
    this.submitDiscount = function (discount) {
        __discount = $.extend(true, __discount, discount);
        if (isValidJson(__discount)) {
            $.post("/BusinessPartner/InsertEditdiscount",
                {
                    discounts: __discount
                },
                function () { });
        }
    }

    this.update = function (key, value) {
        __discount[key] = value;
    }
    function isValidJson(json) {
        return json !== undefined && json.constructor === Object && Object.getOwnPropertyNames(json).length > 0;
    }
    //end discount

}