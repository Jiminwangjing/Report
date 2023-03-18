$(document).ready(function () {
    var __paymentTerm = new PaymentTerm();
    var __instaillment = new Instaillment();
    var __discount = new Discount();
    let installmentDetails = [];
    let discountDetails = [];
    ////insert payment terms
    $("#add").click(function () {
        let id = 0;
        let name = $("#txtpayment").val();
        $("#paymentid").val(name);
        let month = parseInt($("#input1").val());
        let day = parseInt($("#input2").val());
        let due = parseInt($("#duedate").val());
        let start = parseInt($("#sartfrom").val());
        let toleranceday = parseInt($("#input4").val());
        let incompayment = parseInt($("#incoming").val());
        let totaldiscount = parseFloat($("#totaldiscount").val());
        let interestonrecivables = parseFloat($("#interestonrecivables").val());
        let maxcredit = parseFloat($("#maxcredit").val());
        let commitlimit = parseFloat($("#commitlimit").val());
        let pricelistid = parseInt($("#pricelist").val());
        let instaillmentID = parseInt($("#instaillmentid").val());
        let cashdiscountid = parseInt($("#cashdiscountid").val());
        let data = {
            ID: id,
            Code: name,
            Months: isNaN(month) ? 0 : month,
            Days: isNaN(day) ? 0 : day,
            DueDate: isNaN(due) ? 0 : due,
            StartFrom: isNaN(start) ? 0 : start,
            TolerenceDay: isNaN(toleranceday) ? 0 : toleranceday,
            OpenIncomingPayment: isNaN(month) ? 0 : incompayment,
            TotalDiscount: isNaN(incompayment) ? 0 : totaldiscount,
            InterestOnReceiVables: isNaN(interestonrecivables) ? 0 : interestonrecivables,
            CommitLimit: isNaN(commitlimit) ? 0 : commitlimit,
            MaxCredit: isNaN(maxcredit) ? 0 : maxcredit,
            PriceListID: isNaN(pricelistid) ? 0 : pricelistid,
            InstaillmentID: isNaN(instaillmentID) ? 0 : instaillmentID,
            CashDiscountID: isNaN(cashdiscountid) ? 0 : cashdiscountid
        }
        $.ajax({
            url: "/BusinessPartner/InsertPayment",
            type: "POST",
            dataType: "JSON",
            data: { payment: data },
            success: function (respones) {
                if (respones.Error) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary-payment"
                        },
                    }, respones.Model)
                } else {
                    $("#paymentid option:not(:first-child)").remove();
                    $.each(respones.Data, function (i, item) {
                        $("#paymentid").append("<option selected  value=" + item.ID + ">" + item.Code + "</option>");
                    });
                }

            }
        });
        $("#duedate").val("");
        $("#input1").val("");
        $("#input2").val("");
        $("#sartfrom").val("");
        $("#input4").val("");
        $("#incoming").val("");
        $("#totaldiscount").val("");
        $("#interestonrecivables").val("");
        $("#pricelist").val("");
        $("#maxcredit").val("");
        $("#commitlimit").val("");
        $("#discountid").val("");
        $("#instaillmentID").val(0);
        $("#instaillment").val('');
    })
    ////end insert payment term
    //start edit payment
    $("#showInfoPT").click(function () {
        var id = $("#paymentid").val();
        __paymentTerm.fetch(id, function (res) {
            $("#discountid").val(res.CashDiscountID);
            $("#idEdit").val(res.ID);
            $("#iddispay").val(res.CashDiscountID);
            $("#Edittxtpayment").val(res.Code);
            $("#Editduedate").val(res.DueDate);
            $("#Editsartfrom").val(res.StartFrom);
            $("#Editinput1").val(res.Months);
            $("#Editinput2").val(res.Days);
            $("#Editinput4").val(res.TolerenceDay);
            $("#instaillmentID").val(res.CodeInstaill);
            $("#EditinstaillmentID").val(res.InstaillmentID);
            $("#_editinstaillment").val(res.InstaillmentName);
            $("#Editincoming").val(res.OpenIncomingPayment);
            $("#Editdiscountid").val(res.CashDiscount);
            $("#Edittotaldiscount").val(res.TotalDiscount);
            $("#Editinterestonrecivables").val(res.InterestOnReceiVables);
            $("#Editpricelist").val(res.PriceListID);
            $("#Editmaxcredit").val(res.MaxCredit);
            $("#Editcommitlimit").val(res.CommitLimit);
            $("#Editdiscountid").val(res.CashDiscountID);
            __instaillment.fetch(res.InstaillmentID, function (Inres) {
                $("#Editinstaillment").val(Inres.NoOfInstaillment);
            });
            //__discount.fetch(res.CashDiscountID, function (Inres) {
            //    consoloe.log(Inres)
            //    $("#Editinstaillment").val(Inres.NoOfInstaillment);
            //});
        });

    });

    $("#ShowEditcashdis").click(function () {
        var id = $("#discountid").val();

        __discount.fetch(id, function (res) {
            $("#cashid").val(res.ID);
            $("#Editcode").val(res.CodeName);
            $("#Editnamed").val(res.Name);
            $("#Editbydate").prop("checked", res.ByDate).prop("enabled", true);
            $("#Editfreight").prop("checked", res.Freight).prop("enabled", true);
            if (res.ByDate)
                byDate(".bydate", ".freight", '#output2', "editdayy", "editmonthh", "editdiscountt", res);
            if (res.Freight)
                byFreight(".freight", ".bydate", '#output2', "editcashdiscountday", "editdiscountpercent", res);

            $(".bydate").click(function () {
                byDate(".bydate", ".freight", '#output2', "editdayy", "editmonthh", "editdiscountt", res)
            });
            $(".freight").click(function () {
                byFreight(".freight", ".bydate", '#output2', "editcashdiscountday", "editdiscountpercent", res)
            });
            $("#showButton").click(function () {
                $("#discountid").append('<option selected value="' + $("#cashid").val() + '">' + $("#Editcode").val() + '</option>');
                __discount.update("ID", $("#cashid").val());
                __discount.update("CodeName", $("#Editcode").val());
                __discount.update("Name", $("#Editnamed").val());
                __discount.update("Name", $("#Editnamed").val());
                __discount.update("ByDate", $("#Editbydate").prop("checked") ? true : false);
                __discount.update("Freight", $("#Editfreight").prop("checked") ? true : false);
                __discount.update("Day", $("#editdayy").val());
                __discount.update("Month", $("#editmonthh").val());
                __discount.update("Discount", $("#editdiscountt").val());
                __discount.update("CashDiscountDay", $("#editcashdiscountday").val());
                __discount.update("DiscountPercent", $("#editdiscountpercent").val());
                __discount.submitDiscount();
            });
        });
    });

    $("#editdispay1").click(function () {
        var id1 = $("#discountid").val();
        var id2 = $("#Editdiscountid").val();
        if (id1 != id2) {
            __discount.fetch(id2, function (res) {
                $("#cashid").val(res.ID);
                $("#Editcodepay").val(res.CodeName);
                $("#Editnamedpay").val(res.Name);
                $("#Editbydatepay").prop("checked", res.ByDate).prop("enabled", true);
                $("#Editfreightpay").prop("checked", res.Freight).prop("enabled", true);
                if (res.ByDate)
                    byDate("#Editbydatepay", "#Editfreightpay", '#tabledis', "editdayy", "editmonthh", "editdiscountt", res);
                if (res.Freight)
                    byFreight("#Editfreightpay", "#Editbydatepay", '#tabledis', "editcashdiscountday", "editdiscountpercent", res);

                $("#Editbydatepay").click(function () {
                    byDate("#Editbydatepay", "#Editfreightpay", '#tabledis', "editdayy", "editmonthh", "editdiscountt", res)
                });
                $("#Editfreightpay").click(function () {
                    byFreight("#Editfreightpay", "#Editbydatepay", '#tabledis', "editcashdiscountday", "editdiscountpercent", res)
                });
                $("#addeditdis").click(function () {
                    $("#Editdiscountid").append('<option selected value="' + $("#Editdiscountid").val() + '">' + $("#Editcodepay").val() + '</option>');
                    __discount.update("ID", $("#cashid").val());
                    __discount.update("CodeName", $("#Editcodepay").val());
                    __discount.update("Name", $("#Editnamedpay").val());
                    __discount.update("ByDate", $("#Editbydatepay").prop("checked") ? true : false);
                    __discount.update("Freight", $("#Editfreightpay").prop("checked") ? true : false);
                    __discount.update("Day", $("#editdayy").val());
                    __discount.update("Month", $("#editmonthh").val());
                    __discount.update("Discount", $("#editdiscountt").val());
                    __discount.update("CashDiscountDay", $("#editcashdiscountday").val());
                    __discount.update("DiscountPercent", $("#editdiscountpercent").val());
                    __discount.submitDiscount();
                });
            });
        }
        else {
            __discount.fetch(id1, function (res) {

                $("#cashid").val(res.ID);
                $("#Editcodepay").val(res.CodeName);
                $("#Editnamedpay").val(res.Name);
                $("#Editbydatepay").prop("checked", res.ByDate).prop("enabled", true);
                $("#Editfreightpay").prop("checked", res.Freight).prop("enabled", true);
                if (res.ByDate)
                    byDate("#Editbydatepay", "#Editfreightpay", '#tabledis', "editdayy", "editmonthh", "editdiscountt", res);
                if (res.Freight)
                    byFreight("#Editfreightpay", "#Editbydatepay", '#tabledis', "editcashdiscountday", "editdiscountpercent", res);

                $("#Editbydatepay").click(function () {
                    byDate("#Editbydatepay", "#Editfreightpay", '#tabledis', "editdayy", "editmonthh", "editdiscountt", res)
                });
                $("#Editfreightpay").click(function () {
                    byFreight("#Editfreightpay", "#Editbydatepay", '#tabledis', "editcashdiscountday", "editdiscountpercent", res)
                });
                $("#addeditdis").click(function () {
                    $("#Editdiscountid").append('<option selected value="' + $("#discountid").val() + '">' + $("#Editcodepay").val() + '</option>');
                    __discount.update("ID", $("#cashid").val());
                    __discount.update("CodeName", $("#Editcodepay").val());
                    __discount.update("Name", $("#Editnamedpay").val());
                    __discount.update("ByDate", $("#Editbydatepay").prop("checked") ? true : false);
                    __discount.update("Freight", $("#Editfreightpay").prop("checked") ? true : false);
                    __discount.update("Day", $("#editdayy").val());
                    __discount.update("Month", $("#editmonthh").val());
                    __discount.update("Discount", $("#editdiscountt").val());
                    __discount.update("CashDiscountDay", $("#editcashdiscountday").val());
                    __discount.update("DiscountPercent", $("#editdiscountpercent").val());
                    __discount.submitDiscount();
                });
            });
        }
    });
    ////end edit show discount
    $("#Editdiscountid").change(function () {
        $("#editdispay2").removeAttr("hidden", "hidden");
        $("#editdispay1").hide();
    })
    $("#editdispay2").click(function () {
        var id = $("#Editdiscountid").val();

        __discount.fetch(id, function (res) {

            $("#cashid").val(res.ID);
            $("#Editcodepay2").val(res.CodeName);
            $("#Editnamedpay2").val(res.Name);
            $("#Editbydatepay2").prop("checked", res.ByDate).prop("enabled", true);
            $("#Editfreightpay2").prop("checked", res.Freight).prop("enabled", true);
            if (res.ByDate)
                byDate("#Editbydatepay2", "#Editfreightpay2", '#tbeditdispay2', "editdayy", "editmonthh", "editdiscountt", res);
            if (res.Freight)
                byFreight("#Editfreightpay2", "#Editbydatepay2", '#tbeditdispay2', "editcashdiscountday", "editdiscountpercent", res);

            $("#Editbydatepay2").click(function () {
                byDate("#Editbydatepay2", "#Editfreightpay2", '#tbeditdispay2', "editdayy", "editmonthh", "editdiscountt", res)
            });
            $("#Editfreightpay2").click(function () {
                byFreight("#Editfreightpay2", "#Editbydatepay2", '#tbeditdispay2', "editcashdiscountday", "editdiscountpercent", res)
            });
            $("#addeditdis2").click(function () {
                __discount.update("ID", $("#cashid").val());
                __discount.update("CodeName", $("#Editcodepay2").val());
                __discount.update("Name", $("#Editnamedpay2").val());
                __discount.update("ByDate", $("#Editbydatepay2").prop("checked") ? true : false);
                __discount.update("Freight", $("#Editfreight").prop("checked") ? true : false);
                __discount.update("Day", $("#editdayy").val());
                __discount.update("Month", $("#editmonthh").val());
                __discount.update("Discount", $("#editdiscountt").val());
                __discount.update("CashDiscountDay", $("#editcashdiscountday").val());
                __discount.update("DiscountPercent", $("#editdiscountpercent").val());
                __discount.submitDiscount();
            });
        });
    });
    //$("#editdispay1").click(function () {
    //    __paymentTerm.access(function (paymentTerm) {
    //        __discount.fetch(paymentTerm.CashDiscountID, function (res) {
    //            if (res.Freight == true) {

    //                let $editdistable = ViewTable({
    //                    keyField: "ID",
    //                    selector: "#tabledis",
    //                    indexed: true,
    //                    paging: {
    //                        enabled: false,
    //                        pageSize: 10
    //                    },
    //                    visibleFields: [
    //                        "CashDiscountDay",
    //                        "Discount",
    //                    ],
    //                    columns: [
    //                        {
    //                            name: "CashDiscountDay",
    //                            template: "<input>",
    //                            on: {
    //                                "keyup": function (e) {
    //                                    updateInDetails(e.data.ID, "Months", this.value);
    //                                }
    //                            }
    //                        },
    //                        {
    //                            name: "Discount",
    //                            template: "<input>",
    //                            on: {
    //                                "keyup": function (e) {
    //                                    updateInDetails(e.data.ID, "Months", this.value);
    //                                }
    //                            }
    //                        }
    //                    ]
    //                });
    //                //if (isValidArray(discountDetails)) {
    //                //    discountDetails = [];
    //                //    discountDetails.push(res["0"].InstaillmentDetails);
    //                //} else {
    //                //    discountDetails.push(res["0"].InstaillmentDetails);
    //                //}
    //                $editdistable.clearRows();
    //                $editdistable.addRow(res);
    //                $("#Editcodepay").val(res.ID);
    //                $("#Editnamedpay").val(res.CodeName);
    //                $("#Name").val(res.Name);
    //                $("#Editbydatepay").prop("checked", res.ByDate).prop("enabled", true);
    //                $("#Editfreightpay").prop("checked", res.Freight).prop("enabled", true);
    //                //let data = {
    //                //    ID: res["0"].ID,
    //                //    //NoOfInstaillment: instaillment,
    //                //    NoOfInstaillment: $("#Editamountofinstall").val(),
    //                //    CreditMethod: $("#Editcreditmethod").val(),
    //                //    ApplyTax: $("#Editappytax").val(),
    //                //    UpdateTax: $("#Editupdatetax").val(),
    //                //    InstaillmentDetails: installmentDetails[0],
    //                //}
    //                ////$("#editbodyInstallment tr").remove();
    //                //$("#editinstaillment").click(function () {
    //                //    $.post("/BusinessPartner/InsertInstaillment", { instaillment: data }, function () { });
    //                //});
    //            }


    //        });
    //    });
    //});
    $("#editpaymentt").click(function () {
        __paymentTerm.update("ID", $("#idEdit").val());
        __paymentTerm.update("Code", $("#Edittxtpayment").val());
        __paymentTerm.update("DueDate", $("#Editduedate").val());
        __paymentTerm.update("Editsartfrom", $("#StartFrom").val());
        __paymentTerm.update("Months", $("#Editinput1").val());
        __paymentTerm.update("Days", $("#Editinput2").val());
        __paymentTerm.update("TolerenceDay", $("#Editinput4").val());
        __paymentTerm.update("CodeInstaill", $("#instaillmentID").val());
        __paymentTerm.update("InstailMent", $("#Editinstaillment").val());
        __paymentTerm.update("OpenIncomingPayment", $("#Editincoming").val());
        __paymentTerm.update("CashDiscountID", $("#Editdiscountid").val());
        __paymentTerm.update("TotalDiscount", $("#Edittotaldiscount").val());
        __paymentTerm.update("InterestOnReceiVables", $("#Editinterestonrecivables").val());
        __paymentTerm.update("PriceListID", $("#Editpricelist").val());
        __paymentTerm.update("CommitLimit", $("#Editcommitlimit").val());
        __paymentTerm.update("CashDiscount", $("#cashdiscountid").val());
        __paymentTerm.update("MaxCredit", $("#Editmaxcredit").val());
        __paymentTerm.update("StartFrom", $("#Editsartfrom").val());
        __paymentTerm.submit();
    })
    //end edit payment

    //show edit disocunt
    $("#Editdiscount").click(function () {
        var id = $("#Editdiscountid").val();
        __paymentTerm.access(function (paymentTerm) {
            __discount.fetch(paymentTerm.CashDiscountID, function (res) {

                $("#cashEditdiscountid").val(res.ID);
                $("#Editcode").val(res.CodeName);
                $("#Editnamed").val(res.Name);
                $("#Editbydate").prop("checked", res.ByDate);
                $("#Editfreight").prop("checked", res.Freight);
                $("#Editbydate").val(res.ByDate);
                $("#Editfreight").val(res.Freight);
                $("#editdayy").val(res.Day);
                $("#editmonthh").val(res.Month);
                $("#editdiscountt").val(res.Discount);
                $("#editcashdiscountday").val(res.CashDiscountDay);
                $("#editdiscountpercent").val(res.DiscountPercent);
                if (res.ByDate)
                    byDate(".bydate", ".freight", '#output2', "editdayy", "editmonthh", "editdiscountt", res);
                if (res.Freight)
                    byFreight(".freight", ".bydate", '#output2', "editcashdiscountday", "editdiscountpercent", res);

                $(".bydate").click(function () {
                    byDate(".bydate", ".freight", '#output2', "editdayy", "editmonthh", "editdiscountt", res)
                });
                $(".freight").click(function () {
                    byFreight(".freight", ".bydate", '#output2', "editcashdiscountday", "editdiscountpercent", res)
                });
                $("#showButton").click(function () {
                    __discount.update("ID", $("#cashEditdiscountid").val());
                    __discount.update("CodeName", $("#Editcode").val());
                    __discount.update("Name", $("#Editnamed").val());
                    __discount.update("Name", $("#Editnamed").val());
                    __discount.update("ByDate", $("#Editbydate").prop("checked") ? true : false);
                    __discount.update("Freight", $("#Editfreight").prop("checked") ? true : false);
                    __discount.update("Day", $("#editdayy").val());
                    __discount.update("Month", $("#editmonthh").val());
                    __discount.update("Discount", $("#editdiscountt").val());
                    __discount.update("CashDiscountDay", $("#editcashdiscountday").val());
                    __discount.update("DiscountPercent", $("#editdiscountpercent").val());
                    __discount.submitDiscount();
                });
            });
        });
    });
    //end edit discount
    //$("#editdispay").click(function () {
    //    var id = $("#Editdiscountid").val();
    //    __paymentTerm.access(function (paymentTerm) {
    //        __discount.fetch(paymentTerm.CashDiscountID, function (res) {
    //            $("#cashEditdiscountid").val(res.ID);
    //            $("#Editcode").val(res.CodeName);
    //            $("#Editnamed").val(res.Name);
    //            $("#Editbydate").prop("checked", res.ByDate);
    //            $("#Editfreight").prop("checked", res.Freight);
    //            $("#Editbydate").val(res.ByDate);
    //            $("#Editfreight").val(res.Freight);
    //            $("#editdayy").val(res.Day);
    //            $("#editmonthh").val(res.Month);
    //            $("#editdiscountt").val(res.Discount);
    //            $("#editcashdiscountday").val(res.CashDiscountDay);
    //            $("#editdiscountpercent").val(res.DiscountPercent);
    //            if (res.ByDate)
    //                byDate(".bydate", ".freight", '#output2', "editdayy", "editmonthh", "editdiscountt", res);
    //            if (res.Freight)
    //                byFreight(".freight", ".bydate", '#output2', "editcashdiscountday", "editdiscountpercent", res);

    //            $(".bydate").click(function () {
    //                byDate(".bydate", ".freight", '#output2', "editdayy", "editmonthh", "editdiscountt", res)
    //            });
    //            $(".freight").click(function () {
    //                byFreight(".freight", ".bydate", '#output2', "editcashdiscountday", "editdiscountpercent", res)
    //            });
    //            $("#showButton").click(function () {
    //                __discount.update("ID", $("#cashEditdiscountid").val());
    //                __discount.update("CodeName", $("#Editcode").val());
    //                __discount.update("Name", $("#Editnamed").val());
    //                __discount.update("Name", $("#Editnamed").val());
    //                __discount.update("ByDate", $("#Editbydate").prop("checked") ? true : false);
    //                __discount.update("Freight", $("#Editfreight").prop("checked") ? true : false);
    //                __discount.update("Day", $("#editdayy").val());
    //                __discount.update("Month", $("#editmonthh").val());
    //                __discount.update("Discount", $("#editdiscountt").val());
    //                __discount.update("CashDiscountDay", $("#editcashdiscountday").val());
    //                __discount.update("DiscountPercent", $("#editdiscountpercent").val());
    //                __discount.submitDiscount();
    //            });
    //        });
    //    });
    //});
    //shwo edit instaillment
    $("#EditshowInstallmentt").click(function () {
        __paymentTerm.access(function (paymentTerm) {
            __instaillment.fetch(paymentTerm.InstaillmentID, function (res) {
                let $installmentDetallsEdit = ViewTable({
                    keyField: "ID",
                    selector: ".installmentDetailEdit",
                    indexed: true,
                    paging: {
                        enabled: false,
                        pageSize: 10
                    },
                    visibleFields: [
                        "Months",
                        "Day",
                        "Percent",
                    ],
                    columns: [
                        {
                            name: "Months",
                            template: "<input >",
                            on: {
                                "keyup": function (e) {
                                    $(this).asNumber();
                                    updateInstall(installmentDetails, e.data.ID, "Months", this.value);

                                }
                            }
                        },
                        {
                            name: "Day",
                            template: "<input >",
                            on: {
                                "keyup": function (e) {
                                    $(this).asNumber();
                                    updateInstall(installmentDetails, e.data.ID, "Day", this.value);
                                }
                            }
                        }
                    ]
                });

                //if (isValidArray(installmentDetails)) {
                //    installmentDetails = [];
                //    installmentDetails.push(res["0"].InstaillmentDetails);
                //} else {
                //    installmentDetails.push(res["0"].InstaillmentDetails);
                //}
                installmentDetails = [];
                res[0].InstaillmentDetails.forEach(s => {
                    installmentDetails.push(s);
                })

                $installmentDetallsEdit.clearRows();
                $installmentDetallsEdit.bindRows(res[0].InstaillmentDetails);
                $("#installmentId").val(res["0"].ID);
                $("#Editamountofinstall").val(res["0"].NoOfInstaillment);
                $("#Editcreditmethod").val(res["0"].CreditMethod);
                $("#Editappytax").prop("checked", res["0"].ApplyTax);
                $("#Editupdatetax").prop("checked", res["0"].UpdateTax);
                $("#Editbydate").prop("checked", res.ByDate);

                //$("#editbodyInstallment tr").remove();
                $("#editinstaillment").click(function () {
                    var d = $("#Editamountofinstall").val();
                    __instaillment.update("ID", $("#installmentId").val());
                    __instaillment.update("NoOfInstaillment", $("#Editamountofinstall").val());
                    __instaillment.update("CreditMethod", $("#Editcreditmethod").val());
                    __instaillment.update("ApplyTax", $("#Editappytax").prop("checked") ? true : false);
                    __instaillment.update("UpdateTax", $("#Editupdat etax").prop("checked") ? true : false);
                    __instaillment.update("InstaillmentDetails", installmentDetails);
                    $("#_editinstaillment").val(d);

                    __instaillment.submitInstaillment();

                });

            });
        });
    });


    //end edit instaillment
    ////start edit discount

    //chedkbox table
    $(".checkbox1").click(function () {
        byDate(".checkbox1", ".checkbox2", '#output', "dayy", "monthh", "discountt")
    });
    $(".checkbox2").click(function () {
        byFreight(".checkbox2", ".checkbox1", '#output', "cashdiscountday", "discountpercent")
    });
    $(".chbox1").click(function () {
        byDate(".chbox1", ".chbox2", '#add-output', "dayy", "monthh", "discountt")
    });
    $(".chbox2").click(function () {
        byFreight(".chbox2", ".chbox1", '#add-output', "cashdiscountday", "discountpercent")
    });
    function byDate(container, hide, tbcontainer, id1, id2, id3, res) {
        if ($(container).prop("checked")) {
            $(hide).prop("checked", false);
            $(tbcontainer).html(`
                    <table>
                        <thead>
                            <tr>
                                <th>No.</th>
                                <th style="text-align:center!important;">Day</th>
                                <th>+ Month</th><th>Discount</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td><input type="text" readonly></td>
                                <td><input type="text" value='${res !== undefined ? res.Day : ""}' id="${id1}"></td>
                                <td><input type="text" value='${res !== undefined ? res.Month : ""}' id="${id2}"></td>
                                <td><input type="text" value='${res !== undefined ? res.Discount : ""}' id="${id3}"></td>
                            </tr>
                        </tbody>
                    </table>`
            );
        }
    }

    function byFreight(container, hide, tbcontainer, id1, id2, res) {
        if ($(container).prop("checked")) {
            $(hide).prop("checked", false);
            $(tbcontainer).html(`
                <table>
                    <thead>
                        <tr>
                            <th>No.</th>
                            <th style="text-align:center!important;">Cash Discount Day</th>
                            <th>Discount %</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><input type="text" readonly></td>
                            <td><input type="text" value="${res !== undefined ? res.CashDiscountDay : ""}" id="${id1}"></td>
                            <td><input type="text" value="${res !== undefined ? res.DiscountPercent : ""}" id="${id2}"></td>
                        </tr>
                    </tbody>
                </table>`);
        }
    }
    //edit colum

    $("#update").click(function () {
        createDefaultInstallment("#amountofinstall", ".installmentDetailCreate");
    });

    $("#updateEdit").click(function () {
        createDefaultInstallment("#Editamountofinstall", ".installmentDetailEdit");
    });
    function createDefaultInstallment(amount, selector) {
        let amountofinstall = $(amount).val();
        let $installmentDetallsCreate = ViewTable({
            keyField: "UnitID",
            selector: selector,
            indexed: true,
            paging: {
                enabled: false,
                pageSize: 10
            },
            visibleFields: [
                "Months",
                "Day",
                "Percent",
            ],
            columns: [
                {
                    name: "Months",
                    template: "<input >",
                    on: {
                        "keyup": function (e) {
                            updateInstall(installmentDetails, e.data.UnitID, "Months", this.value);
                        }
                    }
                },
                {
                    name: "Day",
                    template: "<input >",
                    on: {
                        "keyup": function (e) {
                            updateInstall(installmentDetails, e.data.UnitID, "Day", this.value);
                        }
                    }
                }
            ]
        });
        $.get("/bp/createdefaultinstalldetail", { amount: amountofinstall }, function (res) {
            if (isValidArray(installmentDetails)) {
                installmentDetails = [];
                res.forEach(i => {
                    installmentDetails.push(i);

                })
            } else {
                res.forEach(i => {
                    installmentDetails.push(i);


                })
            }

            $installmentDetallsCreate.clearRows();
            $installmentDetallsCreate.bindRows(res);
        })
    }

    //end edit instaillment
    ////start create insertinstaillment
    $("#added").click(function () {
        //let InstaillmenttID = $("#instaillmentid").val();
        let amountofinstall = $("#amountofinstall").val();
        let creditmethod = $("#creditmethod").val();
        let appytax = $("#appytax").prop("checked") ? true : false;
        let updatetax = $("#updatetax").prop("checked") ? true : false;
        let data = {
            //ID: InstaillmenttID,
            //NoOfInstaillment: instaillment,
            NoOfInstaillment: amountofinstall,
            CreditMethod: creditmethod,
            ApplyTax: appytax,
            UpdateTax: updatetax,
            InstaillmentDetails: installmentDetails,
        }
        $.ajax({
            url: "/BusinessPartner/InsertInstaillment",
            type: "POST",
            dataType: "JSON",
            data: { instaillment: data },
            complete: function (respones) {
                $("#Editamountofinstall").val("")
                $.ajax({
                    url: "/BusinessPartner/GetInstaillmentLastest",
                    type: "Get",
                    dataType: "JSON",
                    success: function (res) {

                        $("#instaillment").val(res.NoOfInstaillment);
                        $("#instaillmentid").val(res.ID);

                    }
                })
            }
        });
        $("#bodyInstallment tr").remove();
        $("#Editamountofinstall").val("");
        $("#creditmethod").val(0);
        $("#appytax").prop("checked", false);
        $("#updatetax").prop("checked", false);
    });



    function updateInDetails(data, id, prop, value) {

        const res = data[0];
        if (isValidArray(res)) {
            $.grep(res, function (item, index) {
                if (item.UnitID === undefined) {
                    if (item.ID === id) {
                        item[prop] = value;
                    }
                } else {
                    if (item.UnitID === id) {
                        item[prop] = value;
                    }
                }

            });
        }
    }
    function updateInstall(res, id, prop, value) {

        if (isValidArray(res)) {
            $.grep(res, function (item, index) {
                if (item.UnitID === undefined) {
                    if (item.ID === id) {
                        item[prop] = value;
                    }
                } else {
                    if (item.UnitID === id) {
                        item[prop] = value;
                    }
                }

            });
        }
    }

    //end function apend table

    //start discount 
    $("#insertcasdis").click(function () {
        let cashdiscountid = $("#cashdiscountid").val();
        let code = $("#code").val();
        let named = $("#named").val();
        let bydate = $("#bydate").prop("checked") ? true : false;
        let freight = $("#freight").prop("checked") ? true : false;
        let dayy = $("#dayy").val();
        let monthh = $("#monthh").val();
        let discountt = $("#discountt").val();
        let cashdiscountday = $("#cashdiscountday").val();
        let discountpercent = $("#discountpercent").val();
        let data = {
            ID: cashdiscountid,
            CodeName: code,
            Name: named,
            ByDate: bydate,
            Freight: freight,
            Day: dayy,
            Month: monthh,
            Discount: discountt,
            CashDiscountDay: cashdiscountday,
            DiscountPercent: discountpercent,

        }
        $.ajax({
            url: "/BusinessPartner/InsertCashDiscount",
            type: "POST",
            dataType: "JSON",
            data: { cashDiscount: data },
            complete: function (respones) {
                $("#code").val("");
                $("#amountofinstall").val("")
                $.ajax({
                    url: "/BusinessPartner/GetCashDiscountLastest",
                    type: "Get",
                    dataType: "JSON",
                    success: function (respones) {
                        $("#cashdiscountid").val(respones.ID);

                        $("#discountid").append('<option selected value="' + respones.ID + '">' + respones.CodeName + '</option>');
                    }

                })
            }
        });
        $("#code").val("");
        $("#named").val("");
        $("#bydate").prop("checked", false);
        $("#freight").prop("checked", false);
        $("#dayy").val("");
        $("#monthh").val("");
        $("#discountt").val("");
        $("#cashdiscountday").val("");
        $("#discountpercent").val("");
        $("#cashdiscountid").val("");
    })
       
    
    function addnewdiscount() {
        /* let cashdiscountid = $("#cashdiscountid").val();*/
        let code = $("#addnewnamed").val();
        let named = $("#addnewnamed").val();
        let bydate = $("#chbydate").prop("checked") ? true : false;
        let freight = $("#chfreight").prop("checked") ? true : false;
        let dayy = $("#dayy").val();
        let monthh = $("#monthh").val();
        let discountt = $("#discountt").val();
        let cashdiscountday = $("#cashdiscountday").val();
        let discountpercent = $("#discountpercent").val();
        let data = {
            CodeName: code,
            Name: named,
            ByDate: bydate,
            Freight: freight,
            Day: dayy,
            Month: monthh,
            Discount: discountt,
            CashDiscountDay: cashdiscountday,
            DiscountPercent: discountpercent,

        }
        $.ajax({
            url: "/BusinessPartner/InsertCashDiscount",
            type: "POST",
            dataType: "JSON",
            data: { cashDiscount: data },
            complete: function (respones) {
                $("#code").val("");
                $("#amountofinstall").val("")
                $.ajax({
                    url: "/BusinessPartner/GetCashDiscountLastest",
                    type: "Get",
                    dataType: "JSON",
                    success: function (respones) {
                        $("#Editdiscountid").val(respones.ID);

                        $("#Editdiscountid").append('<option selected value="' + respones.ID + '">' + respones.CodeName + '</option>');
                    }

                })
            }
        });
        $("#code").val("");
        $("#named").val("");
        $("#bydate").prop("checked", false);
        $("#freight").prop("checked", false);
        $("#dayy").val("");
        $("#monthh").val("");
        $("#discountt").val("");
        $("#cashdiscountday").val("");
        $("#discountpercent").val("");
        $("#cashdiscountid").val("");
    }

    //end discount

    //Util functions
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }
})