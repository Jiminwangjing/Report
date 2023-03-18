const ___userID = parseInt($("#UserID").text());
const ___branchID = parseInt($("#BranchID").text());
const ___RCJE = JSON.parse($("#data-invioce").text());
var __sysCur = ___RCJE.sysCur;
let __singleElementJE;
//tbMaster
let master = ___RCJE.incomingPayment;

let details = [];
let data_cancel = [];
let __multipaydetail = [];
$(document).ready(function () {

    let _data = JSON.parse($("#model-data").text());
    const disSetting = _data.GeneralSetting;
    let config = {
        master: {
            keyField: "ID",
            value: _data
        }
    };
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });


    var d = new Date();
    document.getElementById("txtPostingdate").valueAsDate = d;
    document.getElementById("txtDocumentDate").valueAsDate = d;
    ___RCJE.seriesRC.forEach(i => {
        if (i.Default == true) {
            $("#DocumentID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    ___RCJE.seriesJE.forEach(i => {
        if (i.Default == true) {
            $("#JEID").val(i.ID);
            $("#JENumber").val(i.NextNo);
            __singleElementJE = findArray("ID", i.ID, ___RCJE.seriesJE);
        }
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
        $.each(___RCJE.seriesRC, function (i, item) {
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
        var seriesRC = find("ID", id, ___PCJE.seriesRC);
        ___PCJE.seriesRC.Number = seriesRC.NextNo;
        ___PCJE.seriesRC.ID = id;
        $("#DocumentTypeID").val(seriesRC.DocumentTypeID);
        $("#number").val(seriesRC.NextNo);
        $("#next_number").val(seriesRC.NextNo);
    });
    if (___RCJE.seriesRC.length == 0) {
        $('#txtInvoice').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
    }


    $.get("/incomingpayment/GetPaymentMeansDefault", function (e) {
        $("#paymentMeans").val(`${e[0].PMName} ${e[0].GLAccCode} - ${e[0].GLAccName}`);
        $("#paymentMeansID").val(e[0].ID);
    })
    $("#show-list-vendor").click(function () {
        chooseVendor();
    });
    // Save All
    $(".btn_add").click(function () {
        saveInComing();
    });
    $(".btn_cancel").click(function () {
        ClickCancel();
    });

    const $multipayment = ViewTable({
        keyField: "LineID",
        selector: "#list-payment",
        indexed: true,
        paging: {
            pageSize: 5,
            enabled: true
        },
        visibleFields: ["Amount", "AmmountSys", "PMName", "Currency"],
        columns: [
            {
                name: "Amount",
                template: "<input type='text' >",
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        let value = parseFloat(this.value);
                        if (value == "" || isNaN(value)) {
                            value = 0;
                        }

                        updatemutipay(__multipaydetail, "LineID", e.key, "Amount", value);
                        updatemutipay(__multipaydetail, "LineID", e.key, "OpenAmount", value);
                        $multipayment.updateColumn(e.key, "AmmountSys", this.value * e.data.ExchangeRate);
                        updatemutipay(__multipaydetail, "LineID", e.key, "CurrID", e.data.CurrID);
                        updatemutipay(__multipaydetail, "LineID", e.key, "PaymentMeanID", e.data.PaymentMeanID);
                        updatemutipay(__multipaydetail, "LineID", e.key, "SCRate", e.data.ExchangeRate);
                        updatemutipay(__multipaydetail, "LineID", e.key, "AmmountSys", this.value * e.data.ExchangeRate);

                        let totalpay = 0;
                        __multipaydetail.forEach(i => {

                            if (i.AmmountSys > 0) {
                                totalpay += i.AmmountSys;
                            }
                        })
                        if (num.toNumberSpecial($("#totalamount_due_asing").val()) == 0) {
                            this.value = 0;
                            $multipayment.updateColumn(e.key, "AmmountSys", this.value * e.data.ExchangeRate);
                        }
                        if (totalpay > num.toNumberSpecial($("#totalamount_due_asing").val()) && num.toNumberSpecial($("#totalamount_due_asing").val()) > 0) {
                            this.value = 0;
                            //var amount = totalpay - parseFloat(e.data.AmmountSys);
                            //this.value = num.toNumberSpecial($("#totalamount_due_asing").val()) - amount;
                            $multipayment.updateColumn(e.key, "AmmountSys", this.value * e.data.ExchangeRate);

                        }

                        $("#total_amount").val(totalpay);


                    }
                }
            },

            {
                name: "AmmountSys",
                template: "<input disabled>",
                on: {
                    "change": function (e) {

                    }
                }
            },
            {
                name: "Currency",
                template: "<select></select>",
                on: {
                    "change": function (e) {
                        $.ajax({
                            url: "/Incomingpayment/GetExchangeRate",
                            type: "Get",
                            dataType: "Json",
                            data: { id: parseInt(this.value) },
                            success: function (res) {
                                $multipayment.updateColumn(e.key, "AmmountSys", res[0].Rate * num.toNumberSpecial(e.data.Amount));
                                updatemutipay(__multipaydetail, "LineID", e.key, "ExchangeRate", res[0].Rate);

                                updatemutipay(__multipaydetail, "LineID", e.key, "CurrID", this.value);
                                updatemutipay(__multipaydetail, "LineID", e.key, "SCRate", e.data.ExchangeRate);
                                updatemutipay(__multipaydetail, "LineID", e.key, "AmmountSys", e.data.Amount * res[0].Rate);
                                updatemutipay(__multipaydetail, "LineID", e.key, "SetRate", res[0].SetRate);

                             
                            }

                        });

                    }
                }
            },
        ],

    });


    $.get("/Incomingpayment/GetPaymentMeans", function (resp) {

        $multipayment.bindRows(resp);
        __multipaydetail = $multipayment.yield();

        $("#txtSearch").keyup(function () {
            let input = this.value.replace(/\s/g, '');
            let rex = new RegExp(input, "i");
            var filtereds = $.grep(resp, function (item, i) {
                return item.PMName.toLowerCase().replace(/\s/g, "").match(rex);
            });

            $multipayment.bindRows(filtereds);
        })
        loadScreen(false)
    });



    let $list_incoming_payment_details = ViewTable({
        keyField: "IncomingPaymentCustomerID",
        selector: "#list-items",
        paging: {
            pageSize: 20,
            enabled: false
        },
        attribute:{
            key: "data-id"
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
                $(`.totalPay:nth(${i})`).html(`<input class='form-control' style='font-size: 0.8rem' value='${txtPayment[i].txtTotalPayment}'/>`);
                if (selectAllTotalSummaryCount <= 1) {
                    let totalpay = $(`.totalPay:nth(${i})`).children("input").val().split(" ")[1];
                    selectAllTotalSummary.push({
                        ID: id,
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
                    updateAmount(details, "IncomingPaymentCustomerID", id, "TotalPayment", this.value);
                });
                $("#totalamount_due_asing").val(curFormat(parseFloat(total(details, "TotalPayment"))));
                //updateAmount(details, "IncomingPaymentCustomerID", id, "TotalPayment", parseFloat(total(selectAllTotalSummary, "txtTotalPayment")).toFixed(3));
                updateAmount(details, "IncomingPaymentCustomerID", id, "TotalDiscount", 0);
                updateAmount(details, "IncomingPaymentCustomerID", id, "CheckPay", true);
                updateAmount(details, "IncomingPaymentCustomerID", id, "TotalPayment", selectAllTotalSummary[i].txtTotalPayment);
            }
            totalPay.children("input").asNumber();
        }
        if ($(this).prop("checked") == false) {
            for (let i = 0; i < txtPayment.length; i++) {
                const id = $(`.cashDis:nth(${i})`).parent().parent().data("id");
                $(`.totalPay:nth(${i})`).html(txtPayment[i].txtTotalPayment);
                $(`.totalDiscounts:nth(${i})`).text(txtPayment[i].totalDiscounts);
                //updateAmount(details, "IncomingPaymentCustomerID", id, "TotalPayment", 0);
                updateAmount(details, "IncomingPaymentCustomerID", id, "TotalDiscount", 0);
                updateAmount(details, "IncomingPaymentCustomerID", id, "CashDiscount", 0);
                updateAmount(details, "IncomingPaymentCustomerID", id, "CheckPay", false);
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
            $("#type").prop("hidden", true);
            $("#labelType").prop("hidden", true);
            $(this).text("New");
        }
    });
    $("#btn_cancel").on("click", function () {
        const done = new DialogBox({
            content: "Are you sure to cancel this Invoice?",
            type: "yes/no",
        });
        done.confirm(function (e) {
            const invoice = $("#next_number").val();
            cancelIncomingInvoice(invoice, function (res) 
            {
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
            findIncomingPaymentToCancel(invoiceNumber, function (res) {
                getIncomingPaymentbyInvoice(res);
            });
        }
    });


    // Table for Cancel
    let $list_incoming_payment_cancel = ViewTable({
        keyField: "IncomingPaymentID",
        selector: "#list-items-cancel",
        paging: {
            pageSize: 10,
            enabled: false
        },
        attribute:{
            key: "data-id"
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

    /// change Invoice Type ///
    $("#type").change(function () {
        const vendorID = $("#vendorID").val();
        dataFromCustomerIncomingPayment(vendorID);
    });
    function dataFromCustomerIncomingPayment(vendorId) {
        const type = $("#type").val();
        $.get("/Incomingpayment/GetSaleAR", { customerID: vendorId, type }, function (resp) {

            $list_incoming_payment_details.clearRows();
            $list_incoming_payment_details.bindRows(resp);
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
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            loadScreen()
            $.get("/Incomingpayment/GetCustomers", function (resp) {
                $vendor_list.clearRows();
                $vendor_list.bindRows(resp);
                $("#VendorSearch").keyup(function () {
                    let __value = this.value.toLowerCase().replace(/\s/g, "");
                    let rex = new RegExp(__value, "i");
                    let __customers = $.grep(resp, function (person) {
                        let phone = isEmpty(person.Phone) ? "" : person.Phone;
                        return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s/g, "").match(rex)
                            || phone.replace(/\s/g, "").match(rex)
                            || person.Type.match(rex)
                    });

                    $vendor_list.bindRows(__customers);
                })
                loadScreen(false)
            });
        });
    }

    function isEmpty(value) {
        return value == null || value == undefined || value == "";
    }

    //save outgoingpayment
    function saveInComing() {
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
        const branchID = $("#branch").val();
        updateMaster(master, "Ref_No", ref_no);
        updateMaster(master, "InvoiceNumber", numberSeries);
        updateMaster(master, "SeriesID", seriesID);
        updateMaster(master, "SeriesDID", seriesDetailId);
        updateMaster(master, "DocTypeID", docTypeId);
        updateMaster(master, "PaymentMeanID", paymentMeanID);
        updateMaster(master, "Remark", remark);
        updateMaster(master, "DocumentDate", documentdate);
        updateMaster(master, "PostingDate", postingdate);
        updateMaster(master, "CustomerID", parseInt(vendorID));
        updateMaster(master, "BranchID", parseInt(branchID));
        updateMaster(master, "UserID", parseInt(___userID));
        const __detail = $.grep(details, function (item, index) {
            return item.CheckPay === true
        });
        __multipaydetail = $multipayment.yield().length == 0 ? new Array() : $multipayment.yield();
        updateMaster(master, "IncomingPaymentDetails", __detail);
        updateMaster(master, "MultiIncommings", __multipaydetail);
        updateMaster(master, "TotalAmountDue", parseFloat($("#totalamount_due_asing").val()));
        console.log("Data=",master);
       master.IncomingPaymentOrderDetails= master.IncomingPaymentDetails;
       master.MultiIncomingPaymentOrders = master.MultiIncommings;
       master.Type=$("#type").val();
        console.log("master=",master)
  
        $.ajax({
            url: "/Incomingpayment/SaveIncomingPaymentOrder",
            type: "Post",
            dataType: "Json",
            data: $.antiForgeryToken({ incoming: JSON.stringify(master), je: JSON.stringify(__singleElementJE) }),
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
    //search
    function search(searchInput, filter) {
        $(searchInput).on("keyup", function () {
            var value = $(this).val().toLowerCase();
            $(filter).filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
            });
        });
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
            if (totalSummary.some(i => i.IncomingPaymentCustomerID != e.data.IncomingPaymentCustomerID))
                totalSummary.push(e.data);
            $("#totalamount_due_asing").val(curFormat(parseFloat(total(totalSummary, "TotalPayment"))));


            totalPayment.html(`<input class='form-control' value='${parseFloat(e.data.TotalPayment)}'/>`);
            totalPayment.children("input").asNumber();
            cashDic.removeAttr("readonly");
            totalPayment.children("input").keyup(function () {
                if (this.value > e.data.BalanceDue) {
                    this.value = e.data.BalanceDue;
                }
                if (this.value == '' || this.value == '-') this.value = 0;
                cashDic.val(0);
                totalDic.text(`${totalDic.text().split(' ')[0]} 0.000`)
                updateAmount(totalSummary, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "TotalPayment", this.value);
                $("#totalamount_due_asing").val(curFormat(parseFloat(total(totalSummary, "TotalPayment"))));

                updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "CashDiscount", 0);
                updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "TotalDiscount", 0);
            });
            updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "CheckPay", true);
        } else {
            if (totalSummary.length > 0) {
                const dTotalSummary = $.grep(totalSummary, function (item, index) {
                    return item.IncomingPaymentCustomerID != e.data.IncomingPaymentCustomerID
                });
                totalSummary = dTotalSummary;
            }
            countFalse--;
            $("#totalamount_due_asing").val(curFormat(parseFloat(total(totalSummary, "TotalPayment"))));

            cashDic.prop("readonly", true);
            totalPayment.html(`<span>${e.data.TotalPayments}</span>`);
            cashDic.val(0);
            updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "CashDiscount", 0);
            updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "TotalDiscount", 0);
            updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "CheckPay", false);
        }
        updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "TotalPayment", e.data.TotalPayment);
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
            updateAmount(selectAllTotalSummary, "ID", e.data.IncomingPaymentCustomerID, "cashDic", _this.value);

            selectAllTotalSummary.forEach(i => {
                if (e.data.IncomingPaymentCustomerID == i.ID) {
                    const tArray = [];
                    //const tpValue = $(e.row).children("td:nth(20)").children("div").children("input").val();
                    const Tdis = $(e.row).children("td:nth(18)").children("span").text().split(" ")[0];
                    const totalDis = parseFloat(e.data.BalanceDue) * parseFloat(i.cashDic) / 100;
                    const totalPayment = parseFloat(e.data.BalanceDue) - parseFloat(totalDis);
                    tArray[0] = totalPayment;
                    updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "casTotalPaymentDic", totalPayment);
                    updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "TotalPayment", totalPayment);
                    updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "CashDiscount", _this.value);
                    updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "TotalDiscount", totalDis);
                    updateAmount(selectAllTotalSummary, "ID", e.data.IncomingPaymentCustomerID, "txtTotalPayment", tArray[0]);
                    $(e.row).children("td:nth(18)").children("span").html(`${Tdis} ${parseFloat(totalDis)}`);
                    $(e.row).children("td:nth(20)").children("div").children("input").val(`${parseFloat(tArray[0])}`);
                    $("#totalamount_due_asing").val(curFormat(parseFloat(total(selectAllTotalSummary, "txtTotalPayment"))));
                    sum = $("#totalamount_due_asing").val();
                }
            });
            $(".currency_text").text($(e.row).children("td:nth(22)").text())
        }
        if (totalSummary.length > 0) {
            updateAmount(totalSummary, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "cashDic", _this.value);
            totalSummary.forEach(i => {
                if (e.data.IncomingPaymentCustomerID == i.IncomingPaymentCustomerID) {
                    const tArray = [];
                    const Tdis = $(e.row).children("td:nth(18)").children("span").text().split(" ")[0];
                    const totalDis = parseFloat(e.data.BalanceDue) * parseFloat(i.cashDic) / 100;
                    const totalPayment = parseFloat(e.data.BalanceDue) - parseFloat(totalDis);
                    tArray[0] = totalPayment;
                    updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "casTotalPaymentDic", totalPayment);
                    updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "TotalPayment", totalPayment);
                    updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "CashDiscount", _this.value);
                    updateAmount(details, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "TotalDiscount", totalDis);
                    updateAmount(totalSummary, "IncomingPaymentCustomerID", e.data.IncomingPaymentCustomerID, "TotalPayment", tArray[0]);
                    $(e.row).children("td:nth(18)").children("span").html(`${Tdis} ${parseFloat(totalDis)}`);
                    $(e.row).children("td:nth(20)").children("div").children("input").val(`${parseFloat(tArray[0])}`);
                    $("#totalamount_due_asing").val(curFormat(parseFloat(total(totalSummary, "TotalPayment"))));
                    sum = $("#totalamount_due_asing").val();


                }
            });
        }
        updateMaster(master, "TotalAmountDue", sum);
    }
    function total(data, prop) {
        let total = 0;
        $.grep(data, function (item, index) {
            if (item[prop].toString().includes(",")) {
                const amountSys = parseFloat(item[prop].toString().split(".")[0].split(',').join('')) * parseFloat(item.ExchangeRate);
                total += amountSys;
            }
            const amountSys = parseFloat(item[prop]) * parseFloat(item.ExchangeRate);
            total += amountSys;
        });
        return total;
    }
    // find Outgoing Payment To Cancel
    function findIncomingPaymentToCancel(invoiceNumber, success) {
        const seriesID = parseInt($("#txtInvoice").val());
        if (invoiceNumber !== null) {
            $.get("/IncomingPayment/FindPaymentOrder", { invoiceNumber, seriesID }, success);
        }
    }

    let _seriesDID = 0;
    function getIncomingPaymentbyInvoice(_res) {
       
        if (_res.Error != null) {
            $list_incoming_payment_cancel.clearRows();
            new DialogBox({
                content: _res.Error,
            });
        }
        data_cancel.push(_res);
        let sum = 0;
        $.each(_res.IncomingPaymentOrderDetails, function (index, item) {
            item.IncomingPaymentID=item.ID;
            item.Date = item.Date.split("T")[0]
            sum += item.Totalpayment;
        });
        _res.IncomingPaymentDetails =_res.IncomingPaymentOrderDetails
        $list_incoming_payment_cancel.bindRows(_res.IncomingPaymentOrderDetails);

        $multipayment.bindRows(_res.MultPayIncommings);
        _res.MultPayIncommings.forEach(i => {
            __multipaydetail.push(i);
            $multipayment.disableColumns(i.LineID, ["Amount"]);
            $multipayment.disableColumns(i.LineID, ["Currency"]);
        });

        _seriesDID = _res.SeriesDID;
      
        $("#show-list-vendor").val(_res.BusinessPartner.Name).prop("disabled", true);
        $("#txtref_no").val(_res.Ref_No).prop("disabled", true);
        $("#txtUser").val(_res.UserAccount.Username).prop("disabled", true);
        $("#paymentMeans").val(`${_res.PaymentMeans.Type} ${_res.GLAccount.Code} - ${_res.GLAccount.Name}`).prop("disabled", true);
        $("#txtPostingdate").val(_res.PostingDate.split("T")[0]).prop("disabled", true);
        $("#txtDocumentDate").val(_res.DocumentDate.split("T")[0]).prop("disabled", true);
        $("#totalamount_due_asing").val(sum);
        $("#total_amount").val(sum);
        $("#total_amount").val(sum);
        $("#id").val(_res.IncomingPaymentID);
        $("#branch").val(_res.BranchID);
    }
    function cancelIncomingInvoice(invoice, success) {
        let id =  parseInt($("#id").val());
        let remark=$("#txtremark").val();
        $.post("/IncomingPayment/CancelInvoicePaymentOrder", {id:id,remark:remark}, success);
    }
    //format currency
    function curFormat(value) {
        return value.toFixed(__sysCur.DecimalPlaces).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }

    function updatemutipay(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue)
                    i[prop] = propValue
            })
        }
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



