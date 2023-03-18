$(document).ready(function () {
    const data = JSON.parse($(".data-dis-cur").text());
    const dataDisCur = data.Display;
    const paramId = data.ParamID;
    const num = NumberFormat({
        decimalSep: dataDisCur.DecimalSeparator,
        thousandSep: dataDisCur.ThousandsSep
    });
    const setupCanRingTable = ViewTable({
        keyField: "LineID",
        selector: "#table-can-ring-setup",
        visibleFields: [
            "Name", "Qty", "ItemName", "ChangeQty", "UomLists",
            "ItemChangeName", "UomChangeLists", "ChargePrice", "PriceLists"
        ],
        indexed: true,
        dataSynced: true,
        model: {
            data: []
        },
        columns: [
            {
                name: "Name",
                template: "<input autocomplete='off'/>",
            },
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
                name: "ItemName",
                template: "<input readonly/>",
                on: {
                    click: function (e) {
                        chooseItem(setupCanRingTable, e.data);
                    }
                }
            },
            {
                name: "ItemChangeName",
                template: "<input readonly/>",
                on: {
                    click: function (e) {
                        chooseItem(setupCanRingTable, e.data, true);
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
                        setupCanRingTable.updateColumn(e.key, "UomChangeID", this.value);
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
                name: "PriceLists",
                template: "<select></select>",
                on: {
                    change: function (e) {
                        setupCanRingTable.updateColumn(e.key, "PriceListID", this.value);
                        const selectList = [{
                            Value: "0",
                            Text: "",
                        }]
                        const priceList = e.data.ListPriceLists.filter(i => i.ID == this.value)[0];
                        if (priceList) {
                            const exRate = e.data.ExchangeRates.filter(i => i.CurrencyID == priceList.CurrencyID)[0];
                            e.data.ExchangRate = exRate.Rate;
                        }
                        e.data.ItemID = 0;
                        e.data.ItemChangeID = 0;
                        e.data.UomID = 0;
                        e.data.UomChangeID = 0;

                        setupCanRingTable.updateColumn(e.key, "ItemName", "");
                        setupCanRingTable.updateColumn(e.key, "ItemChangeName", "");
                        setupCanRingTable.updateColumn(e.key, "UomChangeLists", selectList);
                        setupCanRingTable.updateColumn(e.key, "UomChangeLists", selectList);
                        setupCanRingTable.updateColumn(e.key, "UomLists", selectList);
                        setupCanRingTable.updateColumn(e.key, "UomLists", selectList);
                    }
                }
            },
            {
                name: "UomLists",
                template: "<select></select>",
                on: {
                    change: function (e) {
                        setupCanRingTable.updateColumn(e.key, "UomID", this.value);
                    }
                }
            }
        ],
        actions: [
            {
                template: `<i class="fas fa-trash fa-lg csr-pointer"></i>`,
                on: {
                    "click": function (e) {
                        setupCanRingTable.removeRow(e.key);
                        //location.href = "/Currency/Edit?id=" + e.key;
                    }
                }
            }
        ]
    });
    if (paramId <= 0) {
        $("#new-can-ring-setup").click(function () {
            $.get("/CanRing/GetCanRingSetupDefault", function (res) {
                setupCanRingTable.addRow(res);
            });
        })
        $.get("/CanRing/GetCanRingsSetupDefault", function (res) {
            setupCanRingTable.bindRows(res);
        });
    } else {
        $("#submit-data").text("Save");
        $.get("/CanRing/GetCanRingSetup", { id: paramId }, function (res) {
            if (res.length > 0) {
                setupCanRingTable.bindRows(res);
                setupCanRingTable.disableColumns(undefined, ["PriceLists"]);
                total();

            }
        });
    }
    
    $("#submit-data").click(function () {
        const data = $("#form-submit").serialize();
        $.post("/CanRing/CreateUpdate", data, function (res) {
            new ViewMessage({
                summary: {
                    selector: ".error-message"
                },
            }, res);
            if (res.IsApproved) {
                location.href = "/CanRing/Index";
            }
        })
    })
    const chooseItem = (table, data, isItemChange = false) => {
        $("#loadingitem").prop("hidden", false);
        const itemMasterDataDialog = new DialogBox({
            content: {
                selector: "#item-master-content"
            },
            caption: "Item master data",
        });
        itemMasterDataDialog.invoke(function () {
            //// Bind Item Master ////
            const itemMaster = ViewTable({
                keyField: "ID",
                selector: $("#list-item"),
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                dynamicCol: {
                    afterAction: true,
                    headerContainer: "#col-to-append-after",
                },
                visibleFields: ["Code", "KhmerName", "Currency", "UnitPrice", "BarCode", "UoM"],
                columns: [
                    {
                        name: "UnitPrice",
                        dataType: "number",
                    },
                    {
                        name: "AddictionProps",
                        valueDynamicProp: "ValueName",
                        dynamicCol: true,
                    }
                ],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down csr-pointer"></i>`,
                        on: {
                            "click": function (e) {
                                if (isItemChange) {
                                    table.updateColumn(data.LineID, "ItemChangeID", e.data.ItemID);
                                    table.updateColumn(data.LineID, "ItemChangeName", e.data.KhmerName);
                                    table.updateColumn(data.LineID, "UomChangeLists", e.data.UomChangeLists);
                                    table.updateColumn(data.LineID, "UomChangeLists", e.data.UomChangeLists);
                                    table.updateColumn(data.LineID, "UomChangeID", e.data.UomID);
                                }
                                else {
                                    table.updateColumn(data.LineID, "ItemID", e.data.ItemID);
                                    table.updateColumn(data.LineID, "ItemName", e.data.KhmerName);
                                    table.updateColumn(data.LineID, "UomLists", e.data.UomLists);
                                    table.updateColumn(data.LineID, "UomLists", e.data.UomLists);
                                    table.updateColumn(data.LineID, "UomID", e.data.UomID);
                                }
                                itemMasterDataDialog.shutdown();
                            }
                        }
                    }
                ]
            });
            $.get("/CanRing/GetItemMasterData", { plid: data.PriceListID }, function (res) {
                $("#no-data").prop("hidden", true);
                if (res.length > 0) {
                    itemMaster.clearHeaderDynamic(res[0].AddictionProps);
                    itemMaster.createHeaderDynamic(res[0].AddictionProps);
                    itemMaster.bindRows(res);

                    $("#find-item").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(res, function (item) {
                            return item.Barcode === __value || item.KhmerName.toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.UnitPrice == __value || item.Code.toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.UoM.toLowerCase().replace(/\s+/, "").includes(__value);
                        });
                        itemMaster.bindRows(items);
                    });
                } else {
                    $("#no-data").prop("hidden", false);
                }
                $("#loadingitem").prop("hidden", true);
                setTimeout(() => {
                    $("#find-item").focus();
                }, 300);
            })
        });
        itemMasterDataDialog.confirm(function () {
            itemMasterDataDialog.shutdown();
        })
    };
    const total = () => {
        const data = setupCanRingTable.yield();
        let total = 0;
        if (data.length > 0) {
            data.forEach(i => {
                total += i.ChargePrice * i.ExchangRate;
            })
            $("#total").val(`${data[0].Currency} ${num.formatSpecial(total, dataDisCur.Amounts)}`);
        }
    }
})