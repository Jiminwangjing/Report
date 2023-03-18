$(document).ready(function () {
    let customerObj = {};
    const _customer = ViewTable({
        keyField: "ID",
        selector: "#customer",
        indexed: true,
        paging: {
            pageSize: 20,
            enabled: false
        },
        visibleFields: ["Code", "Name", "Phone", "Address"],
    });

    if ($("#card-expiry-period").val() == 0) {
        const dateFrom = $("#exp-date-from").val();
        const dateNow = new Date();
        $("#exp-date-to").prop("disabled", false);
        if (!dateFrom || dateFrom == "" || dateFrom == "01-01-0001") {
            $("#exp-date-from")[0].valueAsDate = dateNow;
            $("#exp-date-to")[0].valueAsDate = dateNow;
        }
    } else {
        $("#exp-date-to").prop("disabled", true);
    }

    $("#card-expiry-period").change(function () {
        const dateFrom = $("#exp-date-from").val();
        const dateNow = new Date();
        if (!dateFrom || dateFrom == "" || dateFrom == "01-01-0001") {
            $("#exp-date-from")[0].valueAsDate = dateNow;
        }
        if (this.value == 0) {// None
            $("#exp-date-to").prop("disabled", false);
            $("#exp-date-to")[0].valueAsDate = dateNow;
        } else if (this.value == 1) {// ThreeMonths
            setDateTo(3, dateNow, dateFrom)
        }
        else if (this.value == 2) {// SixMonths
            setDateTo(6, dateNow, dateFrom)
        }
        else if (this.value == 3) {// OneYear
            setDateTo(1, dateNow, dateFrom, true)
        }
    })


    $("#save").click(function () {
        const data = {
            ID: $("#id").val(),
            TypeCardID: $("#card-type").val(),
            Name: $("#name").val(),
            Code: $("#code").val(),
            Description: $("#description").val(),
            Active: $("#active").prop("checked") ? true : false,
            Customer: customerObj,
            LengthExpireCard: $("#card-expiry-period").val(),
            ExpireDateFrom: $("#exp-date-from").val(),
            ExpireDateTo: $("#exp-date-to").val(),
        }
        $.post("/CardMember/RegisterMember", { data }, function (res) {
            new ViewMessage({
                summary: {
                    selector: ".error"
                },
            }, res);
            if (res.IsApproved) {
                location.href = "/CardMember/Index"
            }
        })
    })

    $("#choose-cus").click(function () {
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
                    pageSize: 20,
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
                    $("#txtSearch-cuss").on("keyup", function () {
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
    })

    const id = parseInt($("#id").val())
    if (id > 0) {
        $("#loading").prop("hidden", false);
        $.get("/CardMember/GetRegisterMemberDetial", { id }, function (res) {
            $("#id").val(res.ID);
            $("#card-type").val(res.TypeCardID);
            $("#name").val(res.Name);
            $("#code").val(res.Code).prop("disabled", true);
            $("#description").val(res.Description);
            $("#card-expiry-period").val(res.LengthExpireCard);
            $("#active").prop("checked", res.Active);
            res.ExpireDateFrom == "0001-01-01T00:00:00" ? $("#exp-date-from")[0].valueAsDate = new Date() : formatDate("#exp-date-from", res.ExpireDateFrom);
            res.ExpireDateTo == "0001-01-01T00:00:00" ? $("#exp-date-from")[0].valueAsDate = new Date() : formatDate("#exp-date-to", res.ExpireDateTo);
            if (res.LengthExpireCard == 0) {
                $("#exp-date-from").prop("disabled", false);
                $("#exp-date-to").prop("disabled", false);
            } else {
                $("#exp-date-from").prop("disabled", true);
                $("#exp-date-to").prop("disabled", true);
            }
            _customer.clearRows();
            _customer.addRow(res.Customer);
            customerObj = res.Customer;
            if (res.Customer?.Balance > 0) {
                $("#card-type").prop("disabled", true);
            }
            $("#loading").prop("hidden", true);
            $("#choose-cus").prop("hidden", true);
        })
    } else {
        $("#filter-block").prop("hidden", false)
        $("#choose-cus").prop("hidden", false)
    }
    $("#optionCreateCode").change(function () {
        if (this.value == 1) {
            $("#code").val("").focus().prop("readonly", false);
        }
        if (this.value == 2) {
            $.get("/CardMember/GetCodeCardRamdom", function (res) {
                $("#code").val(res).prop("readonly", true)
            })
        }
    })
    function setDateTo(numMonths, dateNow, dateFrom, isyear = false) {
        $("#exp-date-to").prop("disabled", true);
        if (dateFrom != "") {
            let ymd = dateFrom.split("-");
            let m = parseInt(ymd[1]);
            let y = parseInt(ymd[0]);
            if (isyear) {
                ymd[0] = y + numMonths;
                $("#exp-date-to")[0].valueAsDate = new Date(`${ymd.join("-")}T12:00:00`);
            }
            else {
                // ymd[1] = (m + numMonths).toString();
                // ymd[1] = ymd[1].length === 1 ? `0${ymd[1]}` : ymd[1]
                const dateto = new Date().setMonth(dateNow.getMonth() + numMonths);
                $("#exp-date-to")[0].valueAsDate = new Date(dateto);
            }
            // $("#exp-date-to")[0].valueAsDate = new Date(`${ymd.join("-")}T12:00:00`);
        } else {
            const dateto = new Date().setMonth(dateNow.getMonth() + numMonths);
            $("#exp-date-to")[0].valueAsDate = new Date(dateto);
        }
    }
    function formatDate(elm, value) {
        value = value.split("T")[0];
        let ymd = value.split("-");
        ymd[1] = ymd[1].length === 1 ? `0${ymd[1]}` : ymd[1]
        $(elm)[0].valueAsDate = new Date(`${ymd.join("-")}T12:00:00`);
    }
})