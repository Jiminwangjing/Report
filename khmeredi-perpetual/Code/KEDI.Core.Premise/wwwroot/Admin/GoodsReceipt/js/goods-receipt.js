const __UserID = $("#UserID").text(),
    // __BranchID = $("#BranchID").text(),
    ___GRJE = JSON.parse($("#data-invoice").text());
//tbMaster
let details = [],
    serials = [],
    batches = [],
    item_masters = [],
    data_itemM_show = [],
    tb_item_manage = [];
$(document).ready(function () {
  
    var d = new Date();
    document.getElementById("txtPostingdate").valueAsDate = d;
    document.getElementById("txtDocumentDate").valueAsDate = d;
    ___GRJE.seriesGR.forEach(i => {
        if (i.Default == true) {
            $("#DocumentID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#next_number").val(i.NextNo);
        }
    });

    $.get("/Branch/GetMultiBranch",function(res){
        let data =`<option selected value="0">-- no Branch --</option>`;
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
        $("#branch").html(data);     
   });
   

    //invioce
    let selected = $("#txtInvoice");
    selectSeries(selected);
    function selectSeries(selected) {
        $.each(___GRJE.seriesGR, function (i, item) {
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
        var seriesGR = findArray("ID", id, ___GRJE.seriesGR);
        ___GRJE.seriesGR.Number = seriesGR.NextNo;
        ___GRJE.seriesGR.ID = id;
        $("#DocumentTypeID").val(seriesGR.DocumentTypeID);
        $("#number").val(seriesGR.NextNo);
        $("#next_number").val(seriesGR.NextNo);
    });
    if (___GRJE.seriesGR.length == 0) {
        $('#txtInvoice').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
    }
    $("#glacc").click(function () {
        chooseCode(true);
    });
    //GetWarehouse from
    $.ajax({
        url: "/GoodsReceipt/GetWarehousesFrom",
        type: "Get",
        dataType: "Json",
        data: { ID: __UserID },
        success: function (response) {
            var data = "";
            $.each(response, function (i, item) {
                data +=
                    '<option value="' + item.ID + '">' + item.Name + '</option>';
            });
            $("#txtwarehouse_from").append(data).on("change", changeWarehouseFrom);
        }
    });
    function chooseCode(isMaster, key) {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    }
                },
                cancel: {
                    text: "Clear"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-gl-content"
            },
            caption: "G/L Accounts"
        });
        dialog.invoke(function () {
            $.get("/ItemGroup1/GetGlAccountLastLevel", function (resp) {
                let $GLAcc = ViewTable({
                    keyField: "ID",
                    selector: $("#list-glacc"),
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    indexed: true,
                    visibleFields: ["Code", "Name"],
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down' style='cursor:pointer;'></i>",
                            on: {
                                "click": function (e) {
                                    if (isMaster) {
                                        $("#glacc").val(`${e.data.Code} - ${e.data.Name}`);
                                        $("#glaccID").val(e.data.ID);
                                        if (data_itemM_show.length > 0) {
                                            data_itemM_show.forEach(i => {
                                                $listItemMShow.updateColumn(i.LineID, "PaymentMeans", '')
                                                updatedetail(data_itemM_show, i.LineID, "GLID", 0);
                                            })
                                        }
                                    } else {
                                        $("#glacc").val('');
                                        $("#glaccID").val(0);
                                        $listItemMShow.updateColumn(key, "PaymentMeans", e.data.Code)
                                        updatedetail(data_itemM_show, key, "GLID", e.data.ID);
                                    }
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                if (resp.length > 0) {
                    $GLAcc.bindRows(resp);
                    $("#txtSearchPM").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let rex = new RegExp(__value, "gi");
                        let items = $.grep(resp, function (item) {
                            return item.Name.toLowerCase().replace(/\s+/, "").match(rex) || item.Code.toLowerCase().replace(/\s+/, "").match(rex);
                        });
                        $GLAcc.bindRows(items);
                    });
                }
            });
        });
        dialog.reject(function () {
            if (!isMaster) {
                $listItemMShow.updateColumn(key, "PaymentMeans", "")
                updatedetail(data_itemM_show, key, "GLID", 0);
                dialog.shutdown();
            }
        })
    }
    $("#ch-item_master").click(function () {
        const itemMasterDataDialog = new DialogBox({
            content: {
                selector: "#item-master-content"
            },
            caption: "Item master data",
        });
        itemMasterDataDialog.invoke(function () {
            const $listItemM = ViewTable({
                keyField: "LineID",
                selector: $("#item-master"),
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                indexed: true,
                dynamicCol: {
                    afterAction: true,
                    headerContainer: "#col-to-append-after",
                },
                visibleFields: ["Code", "BarCode", "KhmerName", "EnglishName", "Quantity", "Currency", "UomName"],
                columns: [
                    {
                        name: "Quantity",
                        dataType: "number"
                    },
                    {
                        name: "AddictionProps",
                        valueDynamicProp: "ValueName",
                        dynamicCol: true,
                    }
                ],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                const wareid = $("#txtwarehouse_from").val();
                                getAllPurItemsByItemID(wareid, e.data.ItemID, function (res) {
                                    res.forEach(i => {
                                        const found = data_itemM_show.some(el => el.LineID === i.LineID);
                                        if (isValidArray(data_itemM_show)) {
                                            if (!found) {
                                                data_itemM_show.push(i);
                                                $listItemMShow.addRow(i);
                                            }
                                        } else {
                                            data_itemM_show.push(i);
                                            $listItemMShow.addRow(i);
                                        }
                                    });
                                });
                            }
                        }
                    }
                ]
            });
            if (item_masters.length > 0) {
                $listItemMShow.clearHeaderDynamic(item_masters[0].AddictionProps);
                $listItemMShow.createHeaderDynamic(item_masters[0].AddictionProps);
                $listItemM.clearHeaderDynamic(item_masters[0].AddictionProps);
                $listItemM.createHeaderDynamic(item_masters[0].AddictionProps);
                $listItemM.bindRows(item_masters);
                $("#txtseaerch").on("keyup", function (e) {
                    let __value = this.value.toLowerCase().replace(/\s+/, "");
                    let rex = new RegExp(__value, "gi");
                    let items = $.grep(item_masters, function (item) {
                        let name2 = item.EnglishName ?? "";
                        let barCode = item.BarCode ?? "";
                        return item.Code.toLowerCase().replace(/\s+/, "").match(rex) || item.KhmerName.toLowerCase().replace(/\s+/, "").match(rex) ||
                            name2.toLowerCase().replace(/\s+/, "").match(rex) || item.Quantity.toString().toLowerCase().replace(/\s+/, "").match(rex) ||
                            item.Currency.toLowerCase().replace(/\s+/, "").match(rex) || item.UomName.toLowerCase().replace(/\s+/, "").match(rex) ||
                            barCode.toLowerCase().replace(/\s+/, "").match(rex);
                    });
                    $listItemM.bindRows(items);
                });
            }
        })
        itemMasterDataDialog.confirm(function () { itemMasterDataDialog.shutdown() })
    })
    const $listItemMShow = ViewTable({
        keyField: "LineID",
        selector: $("#list-items"),
        paging: {
            pageSize: 20,
            enabled: false
        },
        dynamicCol: {
            afterAction: true,
            headerContainer: "#col-to-append-after-detail",
        },
        indexed: true,
        visibleFields: ["Code", "KhmerName", "EnglishName", "Warehouse", "PaymentMeans", "Quantity", "Cost", "UoMs", "Type"],
        columns: [
            {
                name: "Warehouse",
                template: `<select class='form-control font-size warehouse'></select>`,
                on: {
                    "change": function (e) {
                        updatedetail(data_itemM_show, e.key, "WarehouseID", this.value);
                    }
                }
            },
            {
                name: "PaymentMeans",
                template: `<input readonly/>`,
                on: {
                    "click": function (e) {
                        chooseCode(false, e.key);
                    }
                }
            },
            {
                name: "Cost",
                template: `<input />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updatedetail(data_itemM_show, e.key, "Cost", parseFloat(this.value));
                    }
                }
            },
            {
                name: "Quantity",
                template: `<input>`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        if (e.data.IsLimitOrder) {
                            if (this.value < e.data.MinOrderQty) {
                                //new DialogBox({
                                //    content: `Qty is less than minimun planning data!`,
                                //    caption: "Qty checking"
                                //});
                                this.value = e.data.MinOrderQty;
                            }
                            if (this.value > e.data.MaxOrderQty) {
                                //new DialogBox({
                                //    content: `Qty is more than maximun planning data!`,
                                //    caption: "Qty checking"
                                //});
                                this.value = e.data.MaxOrderQty;
                            }
                        }
                        updatedetail(data_itemM_show, e.key, "Quantity", parseFloat(this.value));
                    }
                }
            },
            {
                name: "UoMs",
                template: `<select></select>`,
                on: {
                    "change": function (e) {
                        updatedetail(data_itemM_show, e.key, "UomID", this.value);
                        const uoM = findArray("ID", e.data.UomID, e.data.UOMView);
                        if (this.value != uoM.BaseUoMID) {
                            //$listItemMShow.updateColumn(e.key, "InStock", e.data.InStock / uoM.Factor);
                            $listItemMShow.updateColumn(e.key, "Cost", e.data.Cost * uoM.Factor);
                        }
                        else {
                            //$listItemMShow.updateColumn(e.key, "Cost", e.data.QuantitySum);
                            $listItemMShow.updateColumn(e.key, "Cost", e.data.CostStore);
                        }

                    }
                }
            },
            {
                name: "AddictionProps",
                valueDynamicProp: "ValueName",
                dynamicCol: true,
            }
        ],
        actions: [
            {
                template: `<i class="fa fa-trash" style="cursor: pointer;"></i>`,
                on: {
                    "click": function (e) {
                        $listItemMShow.removeRow(e.key, true)
                        data_itemM_show = data_itemM_show.filter(i => i.LineID !== e.data.LineID);
                    }
                }
            }
        ]
    });
    /// find items by barcode
    $("#txtbarcode").keypress(function (event) {

        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            var warehouseID = $("#txtwarehouse_from").val();
            var barcode = $("#txtbarcode").val();
            if (barcode != null) {
                $.ajax({
                    url: "/GoodsReceipt/FindBarcode",
                    type: 'GET',
                    dataType: "JSON",
                    data: {
                        WarehouseID: parseInt(warehouseID),
                        Barcode: barcode
                    },
                    success: function (response) {
                        if (response.ActiveError) {
                            new DialogBox({ content: response.Error });
                        }
                        if (response.length <= 0) {
                            $("#txtbarcode").val('');
                        }
                        else {
                            if (isValidArray(response)) {
                                data_itemM_show = [];
                                $.each(response, function (i, item) {
                                    const found = data_itemM_show.some(el => el.LineID === item.LineID);
                                    if (isValidArray(data_itemM_show)) {
                                        if (!found) data_itemM_show.push(item);
                                    } else {
                                        data_itemM_show.push(item);
                                    }
                                });
                                if (isValidArray(data_itemM_show)) {
                                    $listItemMShow.clearHeaderDynamic(data_itemM_show[0].AddictionProps);
                                    $listItemMShow.createHeaderDynamic(data_itemM_show[0].AddictionProps);
                                    $listItemMShow.bindRows(data_itemM_show);
                                }
                                $("#txtbarcode").val('');
                            }
                        }
                    },
                    error: function () {

                    }
                })

            }
        }
    });
    //// Send data to server ////
    $("#saveData").click(function () {
        saveData();
    })
    function updatedetail(data, key, prop, value) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i.LineID === key) {
                    i[prop] = value;
                }
            });
        }
    }
});
//Filter warehouse from
function changeWarehouseFrom() {
    let id = $(this).find("option:selected").val();
    if (isValidArray(item_masters)) {
        item_masters = [];
        item_masters = $.ajax({
            url: "/GoodsReceipt/GetItemByWarehouseFrom",
            type: "GET",
            async: false,
            dataType: "JSON",
            data: { ID: id }
        }).responseJSON;
    } else {
        item_masters = $.ajax({
            url: "/GoodsReceipt/GetItemByWarehouseFrom",
            type: "GET",
            async: false,
            dataType: "JSON",
            data: { ID: id }
        }).responseJSON;
    }
}
function getAllPurItemsByItemID(wareid, itemID, success) {
    $.get("/GoodsReceipt/GetAllPurItemsByItemID", { wareid, itemID }, success);
}
//save data in purchase quotuion
function saveData() {
    var warehouse_from = $("#txtwarehouse_from").val();
    var number = $("#next_number").val();
    var posting = $("#txtPostingdate").val();
    var document = $("#txtDocumentDate").val();
    var ref_no = $("#txtref_no").val();
    var remark = $("#txtRemark").val();
    var seriesId = $("#txtInvoice").val();
    var glAccId = $("#glaccID").val();
    var branchid =parseInt( $("#branch").val());
    data_itemM_show.forEach(i => {
        if (i.ManageExpire == "Manage" && i.ExpireDate == null) {
            var item = {};
            item.LineID = i.LineID;
            item.KhmerName = i.KhmerName;
            item.EnglishName = i.EnglishName;
            item.UomName = i.UomName;
            item.ExpireDate = i.ExpireDate;
            tb_item_manage.push(item)
        }
        else if (i.ManageExpire == "Manage" && i.ExpireDate.slice(0, 10) == '2019-09-09') {
            var item = {};
            item.LineID = i.LineID;
            item.KhmerName = i.KhmerName;
            item.EnglishName = i.EnglishName;
            item.UomName = i.UomName;
            item.ExpireDate = i.ExpireDate;
            tb_item_manage.push(item)
        }
    })
    if (isValidArray(tb_item_manage)) {
        const $item_manage = ViewTable({
            keyField: "LineID",
            selector: $("#item-manage"),
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: ["KhmerName", "EnglishName", "UomName", "ExpireDate"],
            columns: [
                {
                    name: "ExpireDate",
                    template: `<input type="date">`,
                    on: {
                        "change": function (e) {
                            changeExpireDate(e);
                        }
                    }
                }
            ]
        });
        $item_manage.bindRows(tb_item_manage);
    }
    else {
        $("#loading").prop("hidden", false);
        var data = {
            BranchID: branchid,
            UserID: __UserID,
            WarehouseID: warehouse_from,
            PostingDate: posting,
            DocumentDate: document,
            Ref_No: ref_no,
            Number_No: number,
            Remark: remark,
            SeriseID: seriesId,
            GLID: glAccId,
            GoodReceiptDetails: data_itemM_show
        }
        $.ajax({
            url: "/GoodsReceipt/SaveGoodsReceipt",
            type: "POST",
            dataType: "JSON",
            data: {
                data: JSON.stringify(data),
                serial: JSON.stringify(serials),
                batch: JSON.stringify(batches),
            },
            success: function (res) {
                $("#loading").prop("hidden", true);
                if (res.IsApproved) {
                    new ViewMessage({
                        summary: {
                            selector: ".err-succ-message"
                        },
                    }, res).refresh(1000);
                }
                else if (res.IsSerail) {
                    //$("#loading").prop("hidden", true);
                    const serial = SerialTemplate({
                        data: {
                            serials: res.Data,
                        }
                    });
                    serial.serialTemplate();
                    const seba = serial.callbackInfo();
                    serials = seba.serials;
                } else if (res.IsBatch) {
                    //$("#loading").prop("hidden", true);
                    const batch = BatchTemplate({
                        data: {
                            batches: res.Data,
                        }
                    });
                    batch.batchTemplate();
                    const seba = batch.callbackInfo();
                    batches = seba.batches;
                } else {
                    new ViewMessage({
                        summary: {
                            selector: ".err-succ-message"
                        }
                    }, res);
                }
            }
        })
    }
}
//cancel
function cancelpurchase() {
    location.reload();
}
//change Expire date
function changeExpireDate(e) {
    const ID = e.key;
    const expire = $(this).val();
    data_itemM_show.forEach(i => {
        if (i.LineID === ID) {
            json.ExpireDate = expire;
            tb_item_manage = tb_item_manage.filter(i => i.ItemID !== ID)
        }
    });
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