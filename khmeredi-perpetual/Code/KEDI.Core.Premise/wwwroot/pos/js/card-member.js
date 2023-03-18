
function CardMemberUtility(__poscore) {
    if (!(this instanceof CardMemberUtility)) {
        return new CardMemberUtility(__poscore);
    }

    let baseCurrency = {};
    __poscore.load(info => {
        baseCurrency = info.DisplayPayOtherCurrency.filter(i => i.AltCurrencyID == i.BaseCurrencyID)[0];
    })

    const __this = this,
        cmdata = JSON.parse($(".data-invoice").text()),
        _customer = ViewTable({
            keyField: "ID",
            selector: "#customer",
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: ["Code", "Name", "Phone", "Address"],
        }),
        cardMembers = ViewTable({
            keyField: "ID",
            selector: "#cardmember",
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["Code", "Name", "ExpireDateFrom", "ExpireDateTo", "Active"],
            columns: [
                {
                    name: "Active",
                    template: `<input type="checkbox" disabled/>`
                },
                {
                    name: "ExpireDateFrom",
                    dataType: "date",
                    dataFormat: "MM-DD-YYYY"
                },
                {
                    name: "ExpireDateTo",
                    dataType: "date",
                    dataFormat: "MM-DD-YYYY"
                }

            ],
            actions: [
                {
                    template: `<i class="fas fa-edit hover text-center"></i>`,
                    on: {
                        "click": function (e) {
                            __this.getCardMemberByCode(e.data.Code, true)
                        }
                    },
                }

            ]
        }),
        _type_card = ViewTable({
            keyField: "ID",
            selector: "#type-card",
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["Code", "Name", "TypeDiscountName", "Discount"],
            actions: [
                {
                    template: `<i class="fas fa-pen hover text-info"></i>`,
                    on: {
                        "click": function (e) {
                            $(".type-card-name").val(e.data.Name);
                            $(".type-card-code").val(e.data.Code);
                            $(".type-card-id").val(e.data.ID);
                            $(".type-card-discount").val(e.data.Discount);
                            $(".type-card-discount-type").val(e.data.TypeDiscount);
                        }
                    },
                },
                {
                    template: `<i class="fas fa-trash text-danger hover"></i>`,
                    on: {
                        "click": function (e) {
                            $.post("/CardMember/DeleteCardType", { id: e.data.ID }, function (res) {
                                if (res.Error) {
                                    new DialogBox({
                                        content: res.Message,
                                        icon: "danger"
                                    });
                                } else {
                                    _type_card.removeRow(e.key);
                                }
                            })
                        }
                    },
                }
            ]
        });
    let dataDeposit = {
        ID: 0,
        CusID: 0,
        CardMemberID: 0,
        UserID: 0,
        SeriesID: 0,
        SeriesDID: 0,
        DocTypeID: 0,
        Number: "",
        PostingDate: "",
        TotalDeposit: 0,
    },
        customerObj = {};
    $(".deposit-amount").asNumber();
    //renew
    if ($(".card-extend-expiry-period").val() == 0) {
        const dateFrom = $(".renew-extend-exp-date-from").val();
        const dateNow = new Date();
        $(".renew-extend-exp-date-to").prop("readonly", false);
        if (!dateFrom || dateFrom == "") {
            $(".renew-extend-exp-date-from")[0].valueAsDate = dateNow;
            $(".renew-extend-exp-date-to")[0].valueAsDate = dateNow;
        }
    } else {
        $(".renew-extend-exp-date-to").prop("readonly", true);
    }

    if ($(".card-card-expiry-period").val() == 0) {
        const dateFrom = $(".card-exp-date-from").val();
        const dateNow = new Date();
        $(".card-exp-date-to").prop("readonly", false);
        if (!dateFrom || dateFrom == "" || dateFrom == "01-01-0001") {
            $(".card-exp-date-from")[0].valueAsDate = dateNow;
            $(".card-exp-date-to")[0].valueAsDate = dateNow;
        }
    } else {
        $(".card-exp-date-to").prop("readonly", true);
    }
    $(".card-posting-date")[0].valueAsDate = new Date();
    $(".deposit-posting-date")[0].valueAsDate = new Date();
    //invioce
    let selected = $(".deposit-no");
    selectSeries(selected);
    if (cmdata.length == 0) {
        $('.deposit-no').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
        $("#save-deposit").prop("disabled", true);
    }


    this.getCardMembers = function () {
        $.get("/CardMember/GetRegisterMembers", function (res) {
            if (res.length > 0) {
                cardMembers.bindRows(res);
                $("#item-card-member").on("keyup", function () {
                    let __value = this.value.toLowerCase().replace(/\s+/, "");
                    let items = $.grep(res, function (item) {
                        return item.Code.toLowerCase().replace(/\s+/, "").includes(__value) || item.Name.toLowerCase().replace(/\s+/, "").includes(__value);
                    });
                    cardMembers.bindRows(items);
                });
            }
        })
    }
    this.getCardMemberByCode = function (code, isFromList) {
        __poscore.loadScreen();
        $.ajax({
            url: "/CardMember/GetRegisterMemberDetialByCode",
            data: { code },
            success: function (res) {
                __poscore.loadScreen(false);
                if (isValidJson(res)) {
                    $(".card-description").css("height", "33px");
                    $("#balance").css("display", "block");
                    $(".card-balance").val(__poscore.toCurrency(res.Customer.Balance, baseCurrency.DecimalPlaces));
                    $("#card-id").val(res.ID);
                    $(".card-type").val(res.TypeCardID);
                    $(".card-name").val(res.Name);
                    if (isFromList) {
                        $(".card-code").val(res.Code).prop("disabled", true);
                        $("#tab-ccm").addClass("active");
                        $("#list-cards").removeClass("active");
                    }
                    $(".card-description").val(res.Description);
                    $(".card-card-expiry-period").val(res.LengthExpireCard);
                    $(".card-active").prop("checked", res.Active);
                    $("#optionCreateCode").css("display", "none");
                    res.ExpireDateFrom == "0001-01-01T00:00:00" ? $(".card-exp-date-from")[0].valueAsDate = new Date() : formatDate(".card-exp-date-from", res.ExpireDateFrom);
                    res.ExpireDateTo == "0001-01-01T00:00:00" ? $(".card-exp-date-from")[0].valueAsDate = new Date() : formatDate(".card-exp-date-to", res.ExpireDateTo);
                    makeInputReadOnly([".card-exp-date-from", ".card-exp-date-to"])
                    makeInputDisabled([".card-card-expiry-period"])
                    _customer.clearRows();
                    _customer.addRow(res.Customer);
                    customerObj = res.Customer;
                    if (res.Customer.Balance > 0) {
                        $(".card-type").prop("disabled", true);
                    }
                } else {
                    new DialogBox({
                        content: "Card not found",
                        icon: "warning"
                    });
                }
            }
        });
    }
    this.clearCard = function () {
        $("#balance").css("display", "none");
        $(".card-description").css("height", "90px");
        $(".card-balance").val(0);
        $("#card-id").val(0);
        $(".card-type").val(0).prop("disabled", false);
        $(".card-name").val("");
        $(".card-code").val("").prop("disabled", false);
        $(".card-description").val("");
        $(".card-active").prop("checked", false);
        $(".card-card-expiry-period").val(0);
        $(".card-exp-date-from")[0].valueAsDate = new Date();
        $(".card-exp-date-to")[0].valueAsDate = new Date();
        $("#optionCreateCode").css("display", "block");
        makeInputReadOnly([".card-exp-date-from", ".card-exp-date-to"], false)
        makeInputDisabled([".card-card-expiry-period"], false)
        _customer.clearRows();
        customerObj = {};
    }

    function makeInputReadOnly(inputs, readOnly = true) {
        if (isValidArray(inputs)) {
            inputs.forEach(i => {
                $(i).prop("readonly", readOnly);
            })
        }
    }
    function makeInputDisabled(inputs, disabled = true) {
        if (isValidArray(inputs)) {
            inputs.forEach(i => {
                $(i).prop("disabled", disabled);
            })
        }
    }
    function selectSeries(selected) {
        $.each(cmdata, function (i, item) {
            if (item.Default == true) {
                $("<option selected value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
                $(".deposit-number").val(item.NextNo);
                dataDeposit.SeriesID = item.ID;
                dataDeposit.Number = item.NextNo;
                dataDeposit.DocTypeID = item.DocumentTypeID;
            }
            else {
                $("<option value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
            }
        });
        return selected.on('change')
    }
    this.getData = function (isDeleted) {
        $.get("/CardMember/GetCardTypes", { isDeleted }, function (res) {
            _type_card.clearRows();
            _type_card.bindRows(res)
        })
    }
    this.clearTypeCard = function () {
        $(".type-card-name").val("");
        $(".type-card-code").val("");
        $(".type-card-discount").val(0);
        $(".type-card-discount-type").val(0);
        $(".type-card-id").val(0);
    }
    this.clearDeposit = function () {
        dataDeposit = {
            ID: 0,
            CusID: 0,
            CardMemberID: 0,
            UserID: 0,
            SeriesID: 0,
            SeriesDID: 0,
            DocTypeID: 0,
            Number: "",
            PostingDate: "",
            TotalDeposit: 0,
        };
        $(".deposit-amount").val("");
        $(".deposit-card-name").val("");
        $(".deposit-cus-name").val("");
        $(".deposit-price-list").val("");
        $(".deposit-balance").val("");
        $(".deposit-card-code").val("");
    }
    this.clearRenew = function () {
        $(".renew-card-name").val("");
        $(".renew-cus-name").val("");
        $(".card-extend-expiry-period").val(0);
        $(".renew-card-code").val("");
        $(".renew-exp-date-from").val("");
        $(".renew-card-type").val(0);
        $(".renew-exp-date-to").val("");
        $(".renew-card-id").val(0);
        $(".renew-cus-id").val(0);
    }

    this.fxCardExtendExpiryPeriod = function (_this) {
        const dateFrom = $(".renew-extend-exp-date-from").val();
        const dateNow = new Date();
        if (!dateFrom || dateFrom == "") {
            $(".renew-extend-exp-date-from")[0].valueAsDate = dateNow;
        }
        if (_this.value == 0) {// None
            $(".renew-extend-exp-date-to").prop("readonly", false);
            $(".renew-extend-exp-date-to")[0].valueAsDate = dateNow;
        } else if (_this.value == 1) {// ThreeMonths
            setDateTo(3, ".renew-extend-exp-date-to", dateNow, dateFrom)
        }
        else if (_this.value == 2) {// SixMonths
            setDateTo(6, ".renew-extend-exp-date-to", dateNow, dateFrom)
        }
        else if (_this.value == 3) {// OneYear
            setDateTo(1, ".renew-extend-exp-date-to", dateNow, dateFrom, true)
        }
    }
    this.fxRenewCard = function (e) {
        if (e.which === 13) {
            $.get("/CardMember/GetRegisterMemberDetialByCode", { code: $(".renew-card-code").val().trim(), active: true }, function (res) {
                if (isValidJson(res)) {
                    $(".renew-card-name").val(res.Name);
                    $(".renew-cus-name").val(res.Customer.Name);
                    formatDate(".renew-exp-date-from", res.ExpireDateFrom)
                    formatDate(".renew-exp-date-to", res.ExpireDateTo)
                    $(".renew-card-type").val(res.TypeCardID);
                    $(".renew-card-id").val(res.ID);
                    $(".renew-cus-id").val(res.Customer.ID);
                } else {
                    new DialogBox({
                        content: res,
                        icon: "warning"
                    });
                }
            })
        }
    }
    this.fxSaveRenewCard = function () {
        const data = {
            CardID: parseInt($(".renew-card-id").val()),
            CusID: parseInt($(".renew-cus-id").val()),
            LengthExpireCard: $(".card-extend-expiry-period").val(),
            DateFrom: $(".renew-extend-exp-date-from").val(),
            DateTo: $(".renew-extend-exp-date-to").val(),
        }
        const dialogComfirm = new DialogBox({
            content: "Are you sure you want to save the item?",
            type: "yes-no",
            icon: "warning"
        })
        dialogComfirm.confirm(function () {
            __poscore.loadScreen();
            $.post("/CardMember/RenewExpireDateCard", { data }, function (res) {
                new ViewMessage({
                    summary: {
                        selector: ".error"
                    },
                }, res);
                __poscore.loadScreen(false);
                __this.clearRenew();
                dialogComfirm.shutdown();
            })
        })
        dialogComfirm.reject(function () {
            dialogComfirm.shutdown();
        })
    }
    this.fxDepositCard = function (e) {
        if (e.which === 13) {
            $.get("/CardMember/GetRegisterMemberDetialByCode", { code: $(".deposit-card-code").val().trim(), active: true }, function (res) {
                if (isValidJson(res)) {
                    dataDeposit.CusID = res.Customer.ID;
                    dataDeposit.CardMemberID = res.ID;
                    $(".deposit-card-name").val(res.Name);
                    $(".deposit-cus-name").val(res.Customer.Name);
                    $(".deposit-price-list").val(res.Customer.PriceList.Name);
                    $(".deposit-balance").val(__poscore.toCurrency(res.Customer.Balance, baseCurrency.DecimalPlaces));
                    formatDate(".valid-from", res.ExpireDateFrom)
                    formatDate(".valid-to", res.ExpireDateTo)
                } else {
                    new DialogBox({
                        content: res,
                        icon: "warning"
                    });
                }
            })
        }
    }

    this.fxDepositSeries = function (_this) {
        var id = ($(_this).val());
        var seriesCM = findArray("ID", id, cmdata);
        dataDeposit.SeriesID = seriesCM.ID;
        dataDeposit.Number = seriesCM.NextNo;
        dataDeposit.DocTypeID = seriesCM.DocumentTypeID;
        $(".deposit-number").val(seriesCM.NextNo);
    }
    this.fxSaveDeposit = function () {
        dataDeposit.PostingDate = $(".deposit-posting-date").val();
        dataDeposit.TotalDeposit = $(".deposit-amount").val();
        $.post("/CardMember/DepositCardMember", { data: dataDeposit }, function (res) {
            new ViewMessage({
                summary: {
                    selector: ".error-deposit"
                }
            }, res);
            if (res.IsApproved) {
                __this.clearDeposit();
                $(".deposit-number").val(res.Items.series.NextNo)
                dataDeposit.Number = res.Items.series.NextNo;
                dataDeposit.SeriesID = res.Items.series.ID;
                dataDeposit.DocTypeID = res.Items.series.DocuTypeID;
            }
        })
    }
    this.fxSaveCard = function () {
        const data = {
            ID: $("#card-id").val(),
            TypeCardID: $(".card-type").val(),
            Name: $(".card-name").val(),
            Code: $(".card-code").val(),
            Description: $(".card-description").val(),
            Active: $(".card-active").prop("checked") ? true : false,
            Customer: customerObj,
            ExpireDateFrom: $(".card-exp-date-from").val(),
            ExpireDateTo: $(".card-exp-date-to").val(),
            LengthExpireCard: $(".card-card-expiry-period").val(),
        }
        $.post("/CardMember/RegisterMember", { data }, function (res) {
            new ViewMessage({
                summary: {
                    selector: ".error"
                },
            }, res);
            if (res.IsApproved) {
                __this.clearCard();
                __this.getCardMembers()
            }
        })
    }
    this.fxSaveTypeCard = function () {
        const data = {
            ID: $(".type-card-id").val(),
            Code: $(".type-card-code").val(),
            Name: $(".type-card-name").val(),
            TypeDiscount: $(".type-card-discount-type").val(),
            Discount: $(".type-card-discount").val(),
        }
        $.post("/CardMember/TypeCardCreateOrUpdate", { typeCard: data }, function (res) {
            new ViewMessage({
                summary: {
                    selector: ".error-type-card"
                },
            }, res);
            if (res.IsApproved) {
                __this.clearTypeCard();
                __this.getData(false);
            }
        })
    }
    this.fxChooseCustomer = function () {
        let dialog = new DialogBox({
            content: {
                selector: ".customer-dlg"
            },
            button: {
                ok: {
                    text: "Close"
                }
            },
            caption: "Customers",
        });
        dialog.invoke(function () {
            const customer = ViewTable({
                keyField: "ID",
                selector: "#customer-dlg",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "Phone", "Address"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
                        on: {
                            "click": function (e) {
                                if (e.data.CardMemberID > 0) {
                                    $.post("/CardMember/CheckMemberInCard", { bp: e.data }, function (res) {
                                        new DialogBox({
                                            content: res,
                                            icon: "warning"
                                        });
                                    })
                                } else {
                                    _customer.clearRows();
                                    _customer.addRow(e.data);
                                    customerObj = e.data;
                                    dialog.shutdown();
                                }
                            }
                        }
                    }
                ]
            });
            $.get("/CardMember/GetCustomer", function (res) {
                if (res.length > 0) {
                    customer.bindRows(res);
                    $("#item-customer").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(res, function (item) {
                            return item.Code.toLowerCase().replace(/\s+/, "").includes(__value) || item.Name.toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.Address.toLowerCase().replace(/\s+/, "").includes(__value) || item.Phone.toLowerCase().replace(/\s+/, "").includes(__value);
                        });
                        customer.bindRows(items);
                    });
                }
            })
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
    }
    this.fxGenerateCardNumber = function (_this) {
        if (_this.value == 1) {
            $(".card-code").val("").focus().prop("readonly", false);
        }
        if (_this.value == 2) {
            $.get("/CardMember/GetCodeCardRamdom", function (res) {
                $(".card-code").val(res).prop("readonly", true)
            })
        }
    }
    this.fxCardCardExpiryPeriod = function (_this) {
        const dateFrom = $(".card-exp-date-from").val();
        const dateNow = new Date();
        if (!dateFrom || dateFrom == "" || dateFrom == "01-01-0001") {
            $(".card-exp-date-from")[0].valueAsDate = dateNow;
        }
        if (_this.value == 0) {// None
            $(".card-exp-date-to").prop("readonly", false);
            $(".card-exp-date-to")[0].valueAsDate = dateNow;
        } else if (_this.value == 1) {// ThreeMonths

            setDateTo(3, ".card-exp-date-to", dateNow, dateFrom)
        }
        else if (_this.value == 2) {// SixMonths
            setDateTo(6, ".card-exp-date-to", dateNow, dateFrom)
        }
        else if (_this.value == 3) {// OneYear
            setDateTo(1, ".card-exp-date-to", dateNow, dateFrom, true)
        }
    }

    function findArray(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function isValidJson(json) {
        return !isEmpty(json) && json.constructor === Object && Object.keys(json).length > 0;
    };
    function isEmpty(value) {
        return value == undefined || value == null || value == "";
    }
    function setDateTo(numMonths, dateElm, dateNow, dateFrom, isyear = false) {
        $(dateElm).prop("readonly", true);
        if (dateFrom != "") {
            let ymd = dateFrom.split("-");
            let m = parseInt(ymd[1]);
            let y = parseInt(ymd[0]);
            if (isyear) {
                ymd[0] = y + numMonths;
                $(dateElm)[0].valueAsDate = new Date(`${ymd.join("-")}T12:00:00`);
            }
            else {
                // ymd[1] = (m + numMonths).toString();
                // ymd[1] = ymd[1].length === 1 ? `0${ymd[1]}` : ymd[1]
                const dateto = new Date().setMonth(dateNow.getMonth() + numMonths);
                $(dateElm)[0].valueAsDate = new Date(dateto);
            }

        } else {
            const dateto = new Date().setMonth(dateNow.getMonth() + numMonths);
            $(dateElm)[0].valueAsDate = new Date(dateto);
        }
    }
    function formatDate(elm, value) {
        value = value.split("T")[0];
        let ymd = value.split("-");
        ymd[1] = ymd[1].length === 1 ? `0${ymd[1]}` : ymd[1]
        $(elm)[0].valueAsDate = new Date(`${ymd.join("-")}T12:00:00`);
    }




    $(".card-extend-expiry-period").change(function () {
        __this.fxCardExtendExpiryPeriod(this);
    })
    $(".renew-card-code").keypress(function (e) {
        __this.fxRenewCard(e);
    })
    $("#save-renew").click(function () {
        __this.fxSaveRenewCard();
    })
    $("#clear-renew").click(function () {
        __this.clearRenew();
    })

    $(".deposit-card-code").on("keypress", function (e) {
        __this.fxDepositCard(e);
    });
    $('.deposit-no').change(function () {
        __this.fxDepositSeries(this)
    });

    $("#save-deposit").click(function () {
        __this.fxSaveDeposit();
    })
    $("#clear-card").click(function () {
        __this.clearCard();
    })
    $("#clear-deposit").click(function () {
        __this.clearDeposit();
    })
    $("#save-Card").click(function () {
        __this.fxSaveCard();
    })
    $("#clear-type-card").click(function () {
        __this.clearTypeCard();
    })
    $("#save-type-card").click(function () {
        __this.fxSaveTypeCard();
    })
    $("#choose-customer").click(function () {
        __this.fxChooseCustomer();
    })
    $("#optionCreateCode").change(function () {
        __this.fxGenerateCardNumber(this);
    })
    $(".card-code").on("keypress", function (e) {
        if (e.which === 13) {
            __this.getCardMemberByCode($(".card-code").val().trim());
        }
    });


    $(".card-card-expiry-period").change(function () {
        __this.fxCardCardExpiryPeriod(this);
    })

    __this.getCardMembers();
    __this.getData(false);
}





//const __this = new CardMemberUtility();
