$(document).ready(function () {
    const __data_invoice = JSON.parse($(".data").text());
    const __ec = __data_invoice.SeriesEC;
    let num = {},
        disSetting = __data_invoice.GenSetting.Display,
        batches = [],
        serials = [],
        batchPur = [],
        serialPur = [],
        masterData = {
            ID: 0,
            Number: 0,
            DocTypeID: 0,
            SeriesID: 0,
            SeriesDID: 0,
            CusId: 0,
            PriceListID: 0,
            UserID: 0,
            ExchangeCanRingDetails: [],
            WarehouseID: 0,
            BranchID: 0,
            PaymentMeanID: 0,
            LocalSetRate: 0,
            LocalCurrencyID: 0,
            CompanyID: 0,
            SysCurrencyID: 0,
            ExchangeRate: 0,
            TotalSystem: 0,
            Total: 0,
        };
    //set date
    const date = new Date();
    $("#post-date")[0].valueAsDate = date;
    const exCanRing = ViewTable({
        keyField: "LineID",
        selector: "#list-detail",
        visibleFields: [
            "Name", "Qty", "ItemName", "ChangeQty", "UomLists",
            "ItemChangeName", "UomChangeLists", "ChargePrice"
        ],
        indexed: true,
        dataSynced: true,
        model: {
            ExchangeCanRingMaster: {
                ExchangeCanRingDetails: []
            }
        },
        columns: [
            {
                name: "Qty",
                template: "<input autocomplete='off'/>",
                on: {
                    keyup: function () {
                        $(this).asNumber();
                    }
                }
            },
            {
                name: "ChangeQty",
                template: "<input autocomplete='off'/>",
                on: {
                    keyup: function () {
                        $(this).asNumber();
                    }
                }
            },
            {
                name: "UomChangeLists",
                template: "<select></select>",
                on: {
                    change: function (e) {
                        exCanRing.updateColumn(e.key, "UomChangeID", this.value);
                    }
                }
            },
            {
                name: "ChargePrice",
                template: "<input autocomplete='off'/>",
                on: {
                    keyup: function () {
                        $(this).asNumber()
                        total()
                    }
                }
            },
            {
                name: "UomLists",
                template: "<select></select>",
                on: {
                    change: function (e) {
                        exCanRing.updateColumn(e.key, "UomID", this.value);
                    }
                }
            }
        ],
        actions: [
            {
                template: `<i class="fas fa-trash fa-lg csr-pointer"></i>`,
                on: {
                    "click": function (e) {
                        exCanRing.removeRow(e.key);
                        //location.href = "/Currency/Edit?id=" + e.key;
                    }
                }
            }
        ]
    });

    //GetCanRingsSetup
    $("#item-id").click(function () {
        let dialog = new DialogBox({
            caption: "List of can rings",
            icon: "fas fa-user-friends",
            content: {
                selector: "#choose-content-can-ring"
            },
            button: {
                ok: {
                    class: "bg-red",
                    text: "Close"
                }
            }
        })

        dialog.invoke(function () {
            let $listView = ViewTable({
                keyField: "LineID",
                indexed: true,
                visibleFields: [
                    "Name", "Qty", "ItemName", "ChangeQty", "UomName",
                    "ItemChangeName", "UomChangeName", "ChargePrice"
                ],
                selector: "#can-ring-list",
                paging: {
                    pageSize: 20
                },
                columns: [
                    {
                        name: "ChargePrice",
                        dataType: "number",
                        //dataFormat: { fixed: baseCurrency.DecimalPlaces }
                    },
                ],
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down fa-lg fn-green csr-pointer'></i>",
                        on: {
                            "click": function (e) {
                                $.get("/CanRing/GetCanRingSetup", { id: e.data.ID }, function (res) {
                                    res[0].ID = 0;
                                    if (res.length > 0) {
                                        exCanRing.addRow(res[0]);
                                        total();
                                    }
                                })
                            }
                        }
                    }
                ]
            });
            $.get("/CanRing/GetCanRingsSetup", {
                plId: $("#cur-id").val()
            }, function (res) {
                if (isValidArray(res)) {
                    $listView.bindRows(res)
                    $("#search-can-ring").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let rex = new RegExp(__value, "gi");
                        let __canrings = $.grep(res, function (person) {
                            return person.ItemName.match(rex) || person.Name.toLowerCase().replace(/\s+/, "").match(rex)
                                || person.ItemChangeName.toLowerCase().replace(/\s+/, "").match(rex);
                        });
                        venTable.bindRows(__canrings);
                    });
                    dialog.content.find("#loading").css("display", "none")
                    dialog.content.find("#no-data").css("display", "none")
                } else {
                    dialog.content.find("#loading").css("display", "none")
                    $("#no-data").css("display", "flex")
                }
            })
        })
        dialog.confirm(function () {
            dialog.shutdown()
        })
    })
    $("#cur-id").change(function () {
        $("#name-reading").focus();
        exCanRing.clearRows();
        $.get("/canring/getcurrency", { plid: this.value }, function (res) {
            $(".cur").text(res.Currency.Description);
            disSetting = res.Display;
            num = NumberFormat({
                decimalSep: disSetting.DecimalSeparator,
                thousandSep: disSetting.ThousandsSep
            });
            total();

        })
    })
    $("#name-reading").keypress(function (e) {
        const __this = this;
        if (e.which == 13) {
            $.get("/CanRing/GetCanRingSetup", { id: 0, name: __this.value.trim() }, function (res) {
                res[0].ID = 0;
                if (res.length > 0) {
                    exCanRing.addRow(res[0]);
                    total();
                    __this.value = ""
                }
            })
        }
    })
    $("#show-list-vendor").click(function () {
        const vendorDialog = new DialogBox({
            content: {
                selector: ".vendor-container-list"
            },
            caption: "Vendor Lists",
        });
        vendorDialog.invoke(function () {
            const venTable = ViewTable({
                keyField: "ID",
                selector: $("#list-vendor"),
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "Type", "Phone"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down fa-lg csr-pointer"></i>`,
                        on: {
                            "click": function (e) {
                                $("#vendor-name").val(e.data.Name);
                                $("#vendor-id").val(e.data.ID);
                                vendorDialog.shutdown();
                            }
                        }
                    }
                ]
            });
            //GetVendors
            $.get("/CanRing/GetVendors", function (res) {
                venTable.bindRows(res);
                $("#find-vendor").on("keyup", function (e) {
                    let __value = this.value.toLowerCase().replace(/\s+/, "");
                    let rex = new RegExp(__value, "gi");
                    let __vendors = $.grep(res, function (person) {
                        return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s+/, "").match(rex)
                            || person.Phone.toLowerCase().replace(/\s+/, "").match(rex)
                            || person.Type.match(rex);
                    });
                    venTable.bindRows(__vendors);
                });
            })

        });
        vendorDialog.confirm(function () {
            vendorDialog.shutdown();
        })

    })
    /// find here
    $("#btn-find").on("click", function () {
        $("#btn-addnew").prop("hidden", false);
        $("#btn-find").prop("hidden", true);
        $("#next_number").val("").prop("readonly", false).focus();
        $("#submit-item").prop("disabled", true);
    });
    $("#next_number").keypress(function (e) {
        if (e.which == 13) {
            const seriesId = $("#invoice-no").val();
            const invoiceNumber = this.value.trim();
            $.get("/canring/FindExchangeCanRing", { seriesId, invoiceNumber }, function (res) {
                bindExchangeCanring(res);
            })
        }
    })

    const bindExchangeCanring = data => {
        if (data) {
            $("#next_number").val(data.Number);
            $("#docTypeId").val(data.DocTypeID);
            $("#invoice-no").val(data.SeriesID).prop("disabled", true);
            $("#vendor-id").val(data.CusId);
            $("#vendor-name").val(data.VendorName).prop("disabled", true);
            $("#cur-id").val(data.PriceListID).prop("disabled", true);
            formatDate("#post-date", data.CreatedAt);
            exCanRing.bindRows(data.ExchangeCanRingDetails);
            $("#ware-id").val(data.WarehouseID).prop("disabled", true);
            $("#payment-means-id").val(data.PaymentMeanID).prop("disabled", true);
            $("#total").val(data.Total);
            $("#totalDis").val(data.TotalDis);
            $(".cur").text(data.CurrencyName)
        }
    }

    $("#submit-item").click(function () {
        $("#loading").prop("hidden", false);
        masterData = {
            ID: 0,
            Number: $("#next_number").val(),
            DocTypeID: $("#docTypeId").val(),
            SeriesID: $("#invoice-no").val(),
            CusId: $("#vendor-id").val(),
            PriceListID: $("#cur-id").val(),
            ExchangeCanRingDetails: exCanRing.yield(),
            WarehouseID: $("#ware-id").val(),
            PaymentMeanID: $("#payment-means-id").val(),
            Total: $("#total").val(),
        };
        const data = {
            ExchangeCanRingMaster: masterData,
            SerialPurViews: serialPur,
            BatchPurViews: batchPur,
            SerialNumbers: serials,
            BatchNos: batches,
        }
        $.post("/CanRing/SubmitExchangeCanRing", { data }, function (res) {
            $("#loading").prop("hidden", true);
            new ViewMessage({
                summary: {
                    selector: ".error-summary"
                },
            }, res);
            if (res.IsSerail) {
                const serial = SerialTemplate({
                    data: {
                        serials: res.Data,
                    }
                });
                serial.serialTemplate();
                const seba = serial.callbackInfo();
                serials = seba.serials;
            }
            else if (res.IsBatch) {
                const batch = BatchNoTemplate({
                    data: {
                        batches: res.Data,
                    }
                });
                batch.batchTemplate();
                const seba = batch.callbackInfo();
                batches = seba.batches;
            }
            else if (res.IsSerailPur) {
                const serial = SerialTemplatePur({
                    data: {
                        serials: res.Data,
                    }
                });
                serial.serialTemplate();
                const seba = serial.callbackInfo();
                serialPur = seba.serials;
            }
            else if (res.IsBatchPur) {
                const batch = BatchTemplatePur({
                    data: {
                        batches: res.Data,
                    }
                });
                batch.batchTemplate();
                const seba = batch.callbackInfo();
                batchPur = seba.batches;
            }
            else if (res.IsApproved) {
                location.reload();
            } else if (res.IsRejected) { }
            else if (res.ItemsReturns.length > 0) {
                alertOutStock(res.ItemsReturns)
            }
        })
    })
    // Invoice
    let selected = $("#invoice-no");
    selectSeries(selected);
    $('#invoice-no').change(function () {
        var id = $(this).val();
        var seriesecEc = findArray("ID", id, __ec);
        $("#docTypeId").val(seriesecEc.DocumentTypeID);
        $("#next_number").val(seriesecEc.NextNo);
    });
    if (__ec.length == 0) {
        $('#invoice-no').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
        $("#submit-item").prop("disabled", true);
    }
    const total = () => {
        const data = exCanRing.yield();
        let total = 0;
        $("#totalDis").val(0);
        $("#total").val(0);
        if (data.length > 0) {
            data.forEach(i => {
                total += i.ChargePrice;
            })
            $("#totalDis").val(num.formatSpecial(total, disSetting.Prices));
            $("#total").val(total);
        }
    }
    function selectSeries(selected) {
        $.each(__ec, function (i, item) {
            if (item.Default == true) {
                $("<option selected value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
                $("#next_number").val(item.NextNo);
                $("#docTypeId").val(item.DocumentTypeID);
            }
            else {
                $("<option value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
            }
        });
        return selected.on('change')
    }
    function alertOutStock(itemReturns) {
        if (itemReturns.length > 0) {
            ViewTable({
                keyField: "LineID",
                selector: "#item-stock-info-listview",
                visibleFields: ["Code", "KhmerName", "Instock", "OrderQty", "Committed"],
                paging: {
                    enabled: false
                }
            }).bindRows(itemReturns);
            let dialog = new DialogBox({
                caption: "Not enough stock",
                icon: "warning",
                content: {
                    selector: "#item-stock-info"
                }
            });
            dialog.confirm(function () {
                dialog.shutdown();
            });
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
    function formatDate(elm, value) {
        value = value.split("T")[0];
        let ymd = value.split("-");
        ymd[1] = ymd[1].length === 1 ? `0${ymd[1]}` : ymd[1]
        $(elm)[0].valueAsDate = new Date(`${ymd.join("-")}T12:00:00`);
    }
})