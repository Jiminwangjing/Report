const ___userID = parseInt($("#UserID").text());
const ___branchID = parseInt($("#BranchID").text());
const ___PSJE = JSON.parse($("#data-invioce").text());
let __singleElementJE;
//tbMaster
let master = ___PSJE.outgoingPayment;
let details = [];
let data_cancel = [];
$(document).ready(function () {
    var d = new Date();
    document.getElementById("txtPostingdate").valueAsDate = d;
    document.getElementById("txtDocumentDate").valueAsDate = d;
    ___PSJE.seriesPS.forEach(i => {
        if (i.Default == true) {
            $("#DocumentID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    ___PSJE.seriesJE.forEach(i => {
        if (i.Default == true) {
            $("#JEID").val(i.ID);
            $("#JENumber").val(i.NextNo);
            __singleElementJE = findArray("ID", i.ID, ___PSJE.seriesJE);
        }
    });
    let _data = JSON.parse($("#model-data").text());
    const disSetting = _data.GeneralSetting;
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });
 // Get Mutibranch
 $.get("/Branch/GetMultiBranch",function(res){
    let data='<option selected value="0">--- No Branch ---</option>';
    if(res.length>0)
    {
        data="";
       $.each(res,function(i,item){
               if(item.Active)
                    data+=`<option selected value="${item.ID}">${item.Name}</option>`;
               else
                    data+=`<option value="${item.ID}">${item.Name}</option>`;
       });
       
    }
    $("#branch").append(data);       
});
    //invioce
    let selected = $("#txtInvoice");
    selectSeries(selected);
    function selectSeries(selected) {
        $.each(___PSJE.seriesPS, function (i, item) {
            if (item.Default == true) {
                $("<option selected value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
                $("#next_number").val(item.NextNo);
            }
            else {
                $("<option value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
            }
        });
        return selected.on('change')
    }
    $('#txtInvoice').change(function () {
        var id = parseInt($(this).val());
        var seriesPS = find("ID", id, ___PCJE.seriesPS);
        ___PCJE.seriesPS.Number = seriesPS.NextNo;
        ___PCJE.seriesPS.ID = id;
        $("#DocumentTypeID").val(seriesPS.DocumentTypeID);
        $("#number").val(seriesPS.NextNo);
        $("#next_number").val(seriesPS.NextNo);
    });
    if (___PSJE.seriesPS.length == 0) {
        $('#txtInvoice').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
    }
    $("#paymentMeans").click(function () {
        chooseCode();
    });
    $.get("/Outgoingpayment/GetPaymentMeansDefault", function (e) {
        $("#paymentMeans").val(`${e[0].PMName} ${e[0].GLAccCode} - ${e[0].GLAccName}`);
        $("#paymentMeansID").val(e[0].ID);
    })
    $("#show-list-vendor").click(function () {
        chooseVendor();
    });
    // Save All
    $(".btn_add").click(function () {
        saveOutgoing();
    });
    $(".btn_cancel").click(function () {
        ClickCancel();
    });
    let $list_outgoing_payment_details = ViewTable({
        keyField: "OutgoingPaymentVendorID",
        selector: "#list-items",
        paging: {
            pageSize: 10,
            enabled: false
        },
        attribute:{
            key:"data-id",
        },
        visibleFields: [
            "Invoice",
            "DocumentTypeIDValue",
            "Date",
            "OverdueDays",
            "Totals",
            "Applied_Amounts",
            "BalanceDues",
            "CashDiscount",
            "TotalDiscounts",
            "TotalPayments"
        ],
        columns: [
            {
                name: "TotalPayments",
                template: "<div class='totalPay'></div>",
            },
            {
                name: "CashDiscount",
                template: "<input class='cashDis' readonly/>",
                on: {
                    "keyup": function (e) {
                        cashDis(e, this);
                        $list_outgoing_payment_details.updateColumn(e.key, "Total", num.formatSpecial(0.00, disSetting.Prices));
                    }
                }
            },
            {
                name: "TotalDiscounts",
                template: "<span class='totalDiscounts'></span>",
            },
        ],
        actions: [
            {
                template: "<input type='checkbox' class='checkedbox'>",
                on: {
                    "click": function (e) {
                        checkbox(e, this);
                    }
                }
            }
        ],
    });
    let totalSummary = [];
    let txtPayment = [];
    let selectAllTotalSummary = [];
    let countFalse = 0;
    let countClick = 0;
    let selectAllTotalSummaryCount = 0;
    $("#selectall").click(function () {
        const totalPay = $(".totalPay");
        if ($(this).prop("checked") == true) {
            countFalse = $(".checkedbox").length;
            $(".checkedbox").prop("checked", true);
            selectAllTotalSummaryCount++;
            totalSummary = details;
            for (let i = 0; i < txtPayment.length; i++) {
                const id = $(`.cashDis:nth(${i})`).parent().parent().data("id");
                let _row = $(`.totalDiscounts:nth(${i})`).parent().parent();
                $(".cashDis").removeAttr("readonly");
                // $(`.totalPay:nth(${i})`).html(`<input class='form-control' value='${curFormat(parseFloat(txtPayment[i].txtTotalPayment.split(" ")[1]))}'/>`);
                $(`.totalPay:nth(${i})`).html(`<input class='form-control' style='font-size: 0.8rem' value='${txtPayment[i].txtTotalPayment}'/>`);
                if (selectAllTotalSummaryCount <= 1) {
                    let totalpay=$(`.totalPay:nth(${i})`).children("input").val().split(" ")[1];
                    selectAllTotalSummary.push({
                        ID: id,
                        // txtTotalPayment: $(`.totalPay:nth(${i})`).children("input").val(),
                        txtTotalPayment: num.toNumberSpecial(totalpay),
                        cashDic: $(`.cashDis:nth(${i})`).val(),
                        totalDiscounts: $(`.totalDiscounts:nth(${i})`).text(),
                        ExchangeRate: _row.children("td:nth(24)").text()
                    });
                    $("#totalamount_due_asing").val(curFormat(parseFloat(total(selectAllTotalSummary, "txtTotalPayment"))));
                    $(".currency_text").text(_row.children("td:nth(22)").text());
                }
                $(`.totalPay:nth(${i})`).children("input").on("keyup", function () {
                    if (parseFloat(this.value) > parseFloat(txtPayment[i].txtTotalPayment.split(' ')[1])) {
                        this.value = txtPayment[i].txtTotalPayment.split(' ')[1];
                    }
                    if (this.value == '' || this.value == '-') this.value = 0;
                    $(`.cashDis:nth(${i})`).val(0);
                    $(`.totalDiscounts:nth(${i})`).text(`${$(`.totalDiscounts:nth(${i})`).text().split(' ')[0]} 0.000`)
                    updateAmount(selectAllTotalSummary, "ID", id, "txtTotalPayment", this.value);
                    $("#totalamount_due_asing").val(curFormat(parseFloat(total(selectAllTotalSummary, "txtTotalPayment"))));
                    updateAmount(details, "OutgoingPaymentVendorID", id, "TotalPayment", this.value);
                });
                $("#totalamount_due_asing").val(curFormat(parseFloat(total(details, "TotalPayment"))));
                updateAmount(details, "OutgoingPaymentVendorID", id, "TotalDiscount", 0);
                updateAmount(details, "OutgoingPaymentVendorID", id, "CheckPay", true);
                updateAmount(details, "OutgoingPaymentVendorID", id, "TotalPayment", selectAllTotalSummary[i].txtTotalPayment);
            }
            totalPay.children("input").asNumber();
        }
        if ($(this).prop("checked") == false) {
            for (let i = 0; i < txtPayment.length; i++) {
                const id = $(`.cashDis:nth(${i})`).parent().parent().data("id");
                $(`.totalPay:nth(${i})`).html(txtPayment[i].txtTotalPayment);
                $(`.totalDiscounts:nth(${i})`).text(txtPayment[i].totalDiscounts);
                //updateAmount(details, "OutgoingPaymentVendorID", id, "TotalPayment", 0);
                updateAmount(details, "OutgoingPaymentVendorID", id, "TotalDiscount", 0);
                updateAmount(details, "OutgoingPaymentVendorID", id, "CashDiscount", 0);
                updateAmount(details, "OutgoingPaymentVendorID", id, "CheckPay", false);
            }
            totalSummary = [];
            countFalse = 0;
            selectAllTotalSummaryCount = 0;
            selectAllTotalSummary = [];
            $(".checkedbox").prop("checked", false);
            $(".cashDis").prop("readonly", true);
            $(".cashDis").val(0);
            $("#totalamount_due_asing").val(0.000);
        }

    })
   

    // Find To Cancel
    $(".findbtn").click(function () {
        if ($(this).text() === "New") {
            location.reload();
        } else {
            $("#div-add-cancel").prop("hidden", true);
            $("#div-cancel").prop("hidden", false);
            $("#next_number").val("").prop("readonly", false).focus();
            $("#list-items-cancel").prop("hidden", false);
            $("#list-items").prop("hidden", true);
            $(this).text("New");
        }
    });
    $("#btn_cancel").on("click", function () {
        const done = new DialogBox({
            content: "Are you sure to cancel this Invoice?",
            type: "yes/no",
        });
        done.confirm(function (e) {
            let invoice = parseInt($("#id").val());
            cancelOutgoingInvoice(invoice, function (res) {
                if (res.Action == 1) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        },
                    }, res).refresh(1000);
                    done.shutdown();
                } else {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        },
                        for: {
                            attribute: "vm-for",
                            color: "red"
                        }
                    }, res);
                    done.shutdown();
                }
                $(window).scrollTop(0);
            });
        });
        done.reject(function () {
            location.reload();
        });
    });
    $("#next_number").keypress(function (event) {
        const keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == "13") {
            const invoiceNumber = this.value;
            findOutgoingPaymentToCancel(invoiceNumber, function (res) {
                console.log("Data=",res)
                res.OutgoingPaymentDetails=res.OutgoingPaymentOrderDetail;
            $("#id").val(res.ID);
                getOutgoingPaymentbyInvoice(res);
            });
        }
    });
    // Table for Cancel
    let $list_outgoing_payment_cancel = ViewTable({
        keyField: "ID",
        selector: "#list-items-cancel",
        paging: {
            pageSize: 10,
            enabled: false
        },
        attribute:{
            key:"data-id",
        },
        visibleFields: [
            "ItemInvoice",
            "DocNo",
            "Date",
            "OverdueDays",
            "CurrencyName",
            "Total",
            "Applied_Amount",
            "BalanceDue",
            "CashDiscount",
            "TotalDiscount",
            "TotalPayment"
        ],
    });
    function dataFromVendorOutgoingPayment(vendorId) {
        $.get("/Outgoingpayment/GetPurchaseAP", { VendorID: vendorId }, function (resp) {
            $list_outgoing_payment_details.bindRows(resp);
            countClick++;
            if (countClick <= 1) {
                for (let i = 0; i < $(".totalPay").length; i++) {
                    txtPayment.push({
                        ID: $(`.cashDis:nth(${i})`).parent().parent().data("id"),
                        txtTotalPayment: $(`.totalPay:nth(${i})`).text(),
                        cashDic: $(`.cashDis:nth(${i})`).val(),
                        totalDiscounts: $(`.totalDiscounts:nth(${i})`).text()
                    });
                }
            }
            resp.forEach(i => {
                details.push(i);
            })
        });
    }
    function chooseVendor() {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    }
                }
            },
            content: {
                selector: "#vendor-content"
            }
        });

        dialog.invoke(function () {
            let $vendor_list = ViewTable({
                keyField: "ID",
                selector: dialog.content.find("#list-vendor"),
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Name", "Phone"],
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                $("#show-list-vendor").val(e.data.Name);
                                $("#vendorID").val(e.data.ID);
                                dataFromVendorOutgoingPayment(e.data.ID);
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            loadScreen()
            $.get("/Outgoingpayment/GetVendor", function (resp) {
                $vendor_list.clearRows();
                $vendor_list.bindRows(resp);
                $("#VendorSearch").keyup(function () {
                    let input = this.value.replace(/\s+/g, '');
                    let rex = new RegExp(input, "gi");
                    var filtereds = $.grep(resp, function (item, i) {
                        return item.Name.toLowerCase().replace(/\s+/, "").match(rex) ||
                            item.Phone.toLowerCase().replace(/\s+/, "").match(rex);
                    });
                    $vendor_list.bindRows(filtereds);
                })
                loadScreen(false);
            });
        });
    }
    function chooseCode() {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    }
                }
            },
            content: {
                selector: "#active-gl-content"
            }
        });

        dialog.invoke(function () {
            let $listPaymentMeans = ViewTable({
                keyField: "ID",
                selector: dialog.content.find("#list-payment-means"),
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["PMName"],
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                $("#paymentMeans").val(`${e.data.PMName} ${e.data.GLAccCode} - ${e.data.GLAccName}`);
                                $("#paymentMeansID").val(e.data.ID);
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            loadScreen()
            $.get("/Outgoingpayment/GetPaymentMeans", function (resp) {
                $listPaymentMeans.clearRows();
                $listPaymentMeans.bindRows(resp);
                $("#txtSearch").keyup(function () {
                    let input = this.value.replace(/\s+/g, '');
                    let rex = new RegExp(input, "gi");
                    var filtereds = $.grep(resp, function (item, i) {
                        return item.PMName.toLowerCase().replace(/\s+/, "").match(rex);
                    });
                    $listPaymentMeans.bindRows(filtereds);
                })
                loadScreen(false)
            });
        });
    }
    //save outgoingpayment
    function saveOutgoing() {
        loadScreen();
        const ref_no = $("#txtref_no").val();
        //const number = $("#txtnumber").val();
        const postingdate = $("#txtPostingdate").val();
        const documentdate = $("#txtDocumentDate").val();
        const remark = $("#txtremark").val();
        const paymentMeanID = $("#paymentMeansID").val();
        const docTypeId = $("#DocumentID").val();
        const seriesDetailId = $("#SeriesDetailID").val();
        const seriesID = $("#txtInvoice").val();
        const numberSeries = $("#number").val();
        const vendorID = $("#vendorID").val();
        const branchID = parseInt($("#branch").val());
        updateMaster(master, "Ref_No", ref_no);
        updateMaster(master, "NumberInvioce", numberSeries);
        updateMaster(master, "SeriesID", seriesID);
        updateMaster(master, "SeriesDetailID", seriesDetailId);
        updateMaster(master, "DocumentID", docTypeId);
        updateMaster(master, "PaymentMeanID", paymentMeanID);
        updateMaster(master, "Remark", remark);
        updateMaster(master, "DocumentDate", documentdate);
        updateMaster(master, "PostingDate", postingdate);
        updateMaster(master, "VendorID", parseInt(vendorID));
        updateMaster(master, "BranchID", parseInt(branchID));
        updateMaster(master, "UserID", parseInt(___userID));
        const __detail = $.grep(details, function (item, index) {
            return item.CheckPay === true
        });
        updateMaster(master, "OutgoingPaymentDetails", __detail);
        master.OutgoingPaymentOrderDetail = master.OutgoingPaymentDetails;
        console.log("master ",master);
      
        $.ajax({
            url: "/Outgoingpayment/SaveOutgoingPaymentOrder",
            type: "Post",
            dataType: "Json",
            data: $.antiForgeryToken({ outgoing: JSON.stringify(master), je: JSON.stringify(__singleElementJE) }),
            success: function (e) {
                if (e.Model.Action == 1) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        },
                    }, e.Model).refresh(1000);
                } else {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        },
                        for: {
                            attribute: "vm-for",
                            color: "red"
                        }
                    }, e.Model);
                }
                $(window).scrollTop(0);
                loadScreen(false);
            }
        });
    }
    //cancel
    function ClickCancel() {
        location.reload();
    }
    function updateAmount(data, propKey, key, prop, value) {
        $.grep(data, function (item, index) {
            if (item[propKey] == key) {
                item[prop] = value;
            }
        });
        return this;
    }
    function updateMaster(data, prop, value) {
        data[prop] = value;
        return this;
    }
    function checkbox(e, _this) {
        const totalPayment = $(e.row).children("td:nth(20)").children("div");
        const cashDic = $(e.row).children("td:nth(15)").children("input");
        const totalDic = $(e.row).children("td:nth(18)").children("span");
        if ($(_this).prop("checked") == true) {
            countFalse++;
            e.data.TotalPayment = e.data.BalanceDue;
            if (totalSummary.length === 0)
                totalSummary.push(e.data);
            if (totalSummary.some(i => i.OutgoingPaymentVendorID != e.data.OutgoingPaymentVendorID))
                totalSummary.push(e.data);
            $("#totalamount_due_asing").val(curFormat(parseFloat(total(totalSummary, "TotalPayment"))));
            totalPayment.html(`<input class='form-control' value='${curFormat(parseFloat(e.data.TotalPayment))}'/>`);
            totalPayment.children("input").asNumber();
            cashDic.removeAttr("readonly");
            totalPayment.children("input").keyup(function () {
                if (parseFloat(this.value) > parseFloat(e.data.TotalPayments.split(' ')[1])) {
                    this.value = parseFloat(e.data.TotalPayments.split(' ')[1]);
                }
                if (this.value == '' || this.value == '-') this.value = 0;
                cashDic.val(0);
                totalDic.text(`${totalDic.text().split(' ')[0]} 0.000`)
                updateAmount(totalSummary, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "TotalPayment", this.value);
                $("#totalamount_due_asing").val(curFormat(parseFloat(total(totalSummary, "TotalPayment"))));
                updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "CashDiscount", 0);
                updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "TotalDiscount", 0);
            });
            updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "CheckPay", true);
            //updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "TotalPayment", e.data.TotalPayment);
        } else {
            if (totalSummary.length > 0) {
                const dTotalSummary = $.grep(totalSummary, function (item, index) {
                    return item.OutgoingPaymentVendorID != e.data.OutgoingPaymentVendorID
                });
                totalSummary = dTotalSummary;
            }
            countFalse--;
            $("#totalamount_due_asing").val(curFormat(parseFloat(total(totalSummary, "Total"))));

            cashDic.prop("readonly", true);
            totalDic.text(`${e.data.CurrencyName} 0.00`)
            totalPayment.html(`<span>${e.data.TotalPayments}</span>`);
            cashDic.val(0);
            updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "CashDiscount", 0);
            updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "TotalDiscount", 0);
            updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "CheckPay", false);
        }
        updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "TotalPayment", e.data.TotalPayment);

        $(".currency_text").text($(e.row).children("td:nth(22)").text())
        if (countFalse == $(".checkedbox").length) {
            $("#selectall").prop("checked", true);
        } else {
            $("#selectall").prop("checked", false);
        }
    }
    function cashDis(e, _this) {
        $(_this).asNumber();

        if ($(_this).val() == "" || $(_this).val() == "-") $(_this).val(0)
        //checkAccount(this.value);
        if (_this.value > 100) {
            new DialogBox({
                type: "ok",
                content: "Value less than or equal to 100 !"
            });
            _this.value = 100;
        }
        let sum = 0;
        if (selectAllTotalSummary.length > 0) {
            updateAmount(selectAllTotalSummary, "ID", e.data.OutgoingPaymentVendorID, "cashDic", _this.value);

            selectAllTotalSummary.forEach(i => {
                if (e.data.OutgoingPaymentVendorID == i.ID) {
                    const tArray = [];
                    //const tpValue = $(e.row).children("td:nth(20)").children("div").children("input").val();
                    const Tdis = $(e.row).children("td:nth(18)").children("span").text().split(" ")[0];
                    const totalDis = parseFloat(e.data.BalanceDue) * parseFloat(i.cashDic) / 100;
                    const totalPayment = parseFloat(e.data.BalanceDue) - parseFloat(totalDis);
                    tArray[0] = totalPayment;
                    updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "casTotalPaymentDic", totalPayment);
                    updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "TotalPayment", totalPayment);
                    updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "CashDiscount", _this.value);
                    updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "TotalDiscount", totalDis);
                    updateAmount(selectAllTotalSummary, "ID", e.data.OutgoingPaymentVendorID, "txtTotalPayment", tArray[0]);
                    $(e.row).children("td:nth(18)").children("span").html(`${Tdis} ${curFormat(parseFloat(totalDis))}`);
                    $(e.row).children("td:nth(20)").children("div").children("input").val(`${curFormat(parseFloat(tArray[0]))}`);
                    $("#totalamount_due_asing").val(curFormat(parseFloat(total(selectAllTotalSummary, "txtTotalPayment"))));
                    sum = $("#totalamount_due_asing").val();

                }
            });
            $(".currency_text").text($(e.row).children("td:nth(22)").text())
        }
        if (totalSummary.length > 0) {
            updateAmount(totalSummary, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "cashDic", _this.value);
            totalSummary.forEach(i => {
                if (e.data.OutgoingPaymentVendorID == i.OutgoingPaymentVendorID) {
                    const tArray = [];
                    const Tdis = $(e.row).children("td:nth(18)").children("span").text().split(" ")[0];
                    const totalDis = parseFloat(e.data.BalanceDue) * parseFloat(i.cashDic) / 100;
                    const totalPayment = parseFloat(e.data.BalanceDue) - parseFloat(totalDis);
                    tArray[0] = totalPayment;
                    updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "casTotalPaymentDic", totalPayment);
                    updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "TotalPayment", totalPayment);
                    updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "CashDiscount", _this.value);
                    updateAmount(details, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "TotalDiscount", totalDis);
                    updateAmount(totalSummary, "OutgoingPaymentVendorID", e.data.OutgoingPaymentVendorID, "Total", tArray[0]);
                    $(e.row).children("td:nth(18)").children("span").html(`${Tdis} ${curFormat(parseFloat(totalDis))}`);
                    $(e.row).children("td:nth(20)").children("div").children("input").val(`${curFormat(parseFloat(tArray[0]))}`);

                    $("#totalamount_due_asing").val(curFormat(parseFloat(total(totalSummary, "Total"))));
                    sum = $("#totalamount_due_asing").val();
                }
            });
        }
        updateMaster(master, "TotalAmountDue", sum);

    }
    function total(data, prop) {
        let total = 0;
        $.grep(data, function (item, index) {
            const amountSys = parseFloat(item[prop]) * parseFloat(item.ExchangeRate);
            total += amountSys;
        });
        return total;
    }
    // find Outgoing Payment To Cancel
    function findOutgoingPaymentToCancel(invoiceNumber, success) {
        const seriesID = parseInt($("#txtInvoice").val());
        if (invoiceNumber !== null) {
            $.get("/outgoingpayment/FindOutgoingPayOrder", { invoiceNumber, seriesID }, success);
        }
    }
    let _seriesDID = 0;
    function getOutgoingPaymentbyInvoice(_res) {
        if (_res.Error != null) {
            $list_outgoing_payment_cancel.clearRows();
            new DialogBox({
                content: _res.Error,
            });
        }
        //const _res = res[0];
        data_cancel.push(_res);
        let sum = 0;
        $.each(_res.OutgoingPaymentDetails, function (index, item) {
            item.Date = item.Date.split("T")[0]
            sum += item.Totalpayment;
        });
        $list_outgoing_payment_cancel.bindRows(_res.OutgoingPaymentDetails)
        _seriesDID = _res.SeriesDetailID;
        $("#show-list-vendor").val(_res.BusinessPartner.Name).prop("disabled", true);
        $("#txtref_no").val(_res.Ref_No).prop("disabled", true);
        $("#txtUser").val(_res.UserAccount.Username).prop("disabled", true);
        $("#paymentMeans").val(`${_res.PaymentMeans.Type} ${_res.GLAccount.Code} - ${_res.GLAccount.Name}`).prop("disabled", true);
        $("#txtPostingdate").val(_res.PostingDate.split("T")[0]).prop("disabled", true);
        $("#txtDocumentDate").val(_res.DocumentDate.split("T")[0]).prop("disabled", true);
        $("#totalamount_due_asing").val(curFormat(sum));
        $("#branch").val(_res.BranchID); 
    }
    function cancelOutgoingInvoice(invoice, success) {
               let remark=$("#txtremark").val();
        $.post("/Outgoingpayment/CancelOutgoingPayOrder", { id:invoice,remark:remark }, success);
    }
    //format currency
    function curFormat(value) {
        return value.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function findArray(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
    function loadScreen(enabled = true) {
        if (enabled) {
            $("#load-screen").show();
        } else {
            $("#load-screen").hide();
        }
    }
});

