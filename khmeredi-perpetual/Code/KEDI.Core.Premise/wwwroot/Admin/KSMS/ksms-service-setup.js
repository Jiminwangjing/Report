$(document).ready(function () {
    const date = new Date(),
        MASTER = "master",
        DETIAL = "detial";
    let detials = [],
        masterData = {
            ID: 0,
            UserID: 0,
            ItemID: 0,
            UomID: 0,
            PriceListID: 0,
            CreatedAt: "",
            SetupCode: "",
            Remark: "",
            Price: 0,
            Active: false,
            ServiceSetupDetials: []
        };

    //find
    $("#find").click(function () {
        $("#setupcode").prop("readonly", false).val("").focus();
    })
    $("#setupcode").on("keypress", function (e) {
        if (e.which === 13) {
            $("#add").text("Update");
            $.get(
                '/KSMSServiceSetUp/GetService',
                {
                    setupCode: this.value.trim(),
                },
                function (res) {
                    if (res.Error) {
                        new DialogBox({
                            content: res.Message
                        })
                    } else {
                        masterData = res.ServiceSetUpView
                        detialTable.bindRows(res.ServiceSetUpDetailModels)
                        detials = res.ServiceSetUpDetailModels
                        bindMaster()
                        detialTable.disableColumns(undefined, ["Qty", "UomList"])
                    }
                }
            )
        }
    });

    // add data
    $("#add").click(function () {

        masterData.ServiceSetupDetials = detials
        masterData.Price = $("#price").val()
        masterData.SetupCode = $("#setupcode").val()
        masterData.Remark = $("#remark").val()
        masterData.CreatedAt = $("#date").val()
        masterData.PriceListID = $("#pricelistid").val()
        masterData.Active = $("#active").prop("checked") ? true : false
        $.post("/KSMSServiceSetUp/UpdateData", { serviceSetup: masterData }, function (res) {
            const message = new ViewMessage({
                summary: {
                    selector: ".error-summery"
                }
            }, res)
            if (res.IsApproved) {
                message.refresh(1500);
            }
        })
    })
    $("#cancel").click(function () {
        location.reload()
    })
    //choose items
    $("#chooseItem").click(function () {
        chooseItemMasterData(DETIAL)
    })

    $("#itemname").click(function () {
        chooseItemMasterData(MASTER)
    })

    $("#date")[0].valueAsDate = date;
    
    const detialTable = ViewTable({
        keyField: "ItemID",
        selector: "#list-detail",
        indexed: true,
        paging: {
            pageSize: 20,
            enabled: true
        },
        visibleFields: ["ItemCode", "ItemName", "Qty", "UomList", "UnitPrice"],
        columns: [
            {
                name: "Qty",
                template: "<input />",
                on: {
                    "keyup": function (e) {
                        updateData(detials, "ItemID", e.key, "Qty", this.value);
                    }
                }
            },
            {
                name: "UnitPrice",
                dataType: "number"
            },
            {
                name: "UomList",
                template: "<select></select>",
                on: {
                    "change": function (e) {
                        updateData(detials, "ItemID", e.key, "UomID", parseInt(this.value));
                        const uomList = findArray("UoMID", this.value, e.data.UomPriceLists);
                        const uom = findArray("ID", this.value, e.data.UoMsList);
                        if (!!uom && !!uomList) {
                            updateData(detials, "ItemID", e.key, "UnitPrice", uomList.UnitPrice);
                            updateData(detials, "ItemID", e.key, "Factor", uom.Factor);
                            detialTable.updateColumn(e.key, "UnitPrice", uomList.UnitPrice);
                        }
                    }
                }

            }
        ],
        actions: [
            {
                template: `<i class="fas fa-trash text-danger hover"></i>`,
                on: {
                    "click": function (e) {
                        detialTable.removeRow(e.key);
                        detials = detials.filter(i => i.ItemID !== e.key);
                    }
                }
            }
        ]
    })

    function bindMaster() {
        $("#price").val(masterData.Price)
        $("#remark").val(masterData.Remark)
        $("#date").val(masterData.CreatedAt.split("T")[0])
        $("#itemname").val(masterData.ItemName).prop("disabled", true)
        $("#itemcode").val(masterData.Code)
        $("#uom").val(masterData.UomName)
        $("#userId").val(masterData.UserName)
        $("#pricelistid").val(masterData.PriceListID).prop("disabled", true)
        $("#active").prop("checked", masterData.Active)
    }

    function chooseItemMasterData(type) {
        const dialog = new DialogBox({
            content: {
                selector: "#container-list-item"
            },
            button: {
                ok: {
                    text: "Close"
                }
            },
            caption: "List of items",
        });
        dialog.invoke(function () {
            const itemTable = ViewTable({
                keyField: "ID",
                selector: ".list-items",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "ItemName1", "Uom", "Barcode"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
                        on: {
                            "click": function (e) {
                                if (type === MASTER) {
                                    $("#itemname").val(e.data.ItemName1)
                                    $("#itemcode").val(e.data.Code)
                                    $("#uom").val(e.data.Uom)
                                    masterData.ItemID = e.data.ID
                                    masterData.UomID = e.data.UomID
                                    $("#chooseItem").prop("disabled", false);
                                    dialog.shutdown();
                                }
                                if (type === DETIAL) {
                                    const plId = $("#pricelistid").val()
                                    $.get("/KSMSServiceSetUp/GetitemDetials", { itemId: e.data.ID, plId }, function (res) {
                                        if (isValidArray(detials)) {
                                            var isExisted = findArray("ItemID", res.ItemID, detials);
                                            if (isExisted) {
                                                var qty = isExisted.Qty + 1;
                                                updateData(detials, "ItemID", res.ItemID, "Qty", qty);
                                                detialTable.updateColumn(isExisted.ItemID, "Qty", qty);
                                            } else {
                                                detialTable.addRow(res);
                                                detials.push(res);
                                            }
                                        } else {
                                            detialTable.addRow(res);
                                            detials.push(res);
                                        }
                                    })
                                }
                            }
                        }
                    }
                ]
            })
            $.get("/KSMSServiceSetUp/GetItems", { plId: masterData.PriceListID }, function (res) {
                if (res.length > 0) {
                    itemTable.bindRows(res)
                    $("#txtSearch-item").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(res, function (item) {
                            return item.Code.toLowerCase().replace(/\s+/, "").includes(__value) || item.ItemName1.toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.Barcode.toLowerCase().replace(/\s+/, "").includes(__value)
                        });
                        itemTable.bindRows(items)
                    });
                }
            })
        })
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }

    function updateData(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] == keyValue) {
                    i[prop] = propValue;
                }
            });
        }
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
})