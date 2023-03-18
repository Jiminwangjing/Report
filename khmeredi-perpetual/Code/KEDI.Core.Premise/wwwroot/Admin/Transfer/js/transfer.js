const __UserID = $("#UserID").text();
// const __BranchID = $("#BrandID").text();
const ___STJE = JSON.parse($("#data-invoice").text());
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
    var time = d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
    $("#txttime").val(time);
    ___STJE.seriesST.forEach(i => {
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
   var __BranchID = parseInt($("#branch").val());
    //invioce
    let selected = $("#txtInvoice");
    selectSeries(selected);
    function selectSeries(selected) {
        $.each(___STJE.seriesST, function (i, item) {
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
        var seriesST = findArray("ID", id, ___STJE.seriesST);
        ___STJE.seriesST.Number = seriesST.NextNo;
        ___STJE.seriesST.ID = id;
        $("#DocumentTypeID").val(seriesST.DocumentTypeID);
        $("#number").val(seriesST.NextNo);
        $("#next_number").val(seriesST.NextNo);
    });
    if (___STJE.seriesST.length == 0) {
        $('#txtInvoice').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
    }
    //$("#glacc").click(function () {
    //    chooseCode();
    //});
    //GetWarehouse from
    $.ajax({
        url: "/Transfer/GetWarehousesFrom",
        type: "Get",
        dataType: "Json",
        data: {
            BranchID: __BranchID
        },
        success: function (response) {
            var data = "";
            $.each(response, function (i, item) {
                data +=
                    '<option value="' + item.ID + '">' + item.Name + '</option>';
            });
            $("#txtwarehouse_from").append(data).on("change", changeWarehouseFrom);
        }
    });
    //GetWarehouse To
    $.ajax({
        url: "/Transfer/GetWarehousesTo",
        type: "Get",
        dataType: "Json",
        data: { BranchID: __BranchID },
        success: function (response) {
            var sata = "";
            $.each(response, function (i, item) {
                sata +=
                    '<option value="' + item.ID + '">' + item.Name + '</option>';
            });
            $("#txtwarehouse_to").append(sata).on("change", changeWarehouseTo);
        }
    });
    // Get branch
    $.ajax({
        url: "/Transfer/GetBranch",
        type: "Get",
        dataType: "Json",
        success: function (e) {
            var data = "";
            $.each(e, function (i, item) {
                if (item.ID == __BranchID) {
                    data +=
                        '<option selected value="' + item.ID + '">' + item.Name + '</option>';
                }
                else {
                    data +=
                        '<option value="' + item.ID + '">' + item.Name + '</option>';
                }
            });
            $("#txtbranch").append(data).on("change", filterBranch);
        }
    })
    $("#ch-item_master").click(function ()
     {
        const itemMasterDataDialog = new DialogBox({
            content: {
                selector: "#item-master-content"
            },
            caption: "Item master data",
        });
        itemMasterDataDialog.invoke(function ()
         {
            const $listItemM = ViewTable({
                keyField: "LineID",
                selector: $("#item-master"),
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                dynamicCol: {
                    afterAction: true,
                    headerContainer: "#col-to-append-after",
                },
                visibleFields: ["Code", "Barcode", "KhmerName", "EnglishName", "Qty", "Cost", "Currency", "UomName"],
                columns: [
                    {
                        name: "Cost",
                        dataType: "number",
                    },
                    {
                        name: "Qty",
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
            })

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
                        let barcode = item.Barcode ?? "";
                        return item.Code.toLowerCase().replace(/\s+/, "").match(rex) || item.KhmerName.toLowerCase().replace(/\s+/, "").match(rex) ||
                            name2.toLowerCase().replace(/\s+/, "").match(rex) || item.Qty.toString().toLowerCase().replace(/\s+/, "").match(rex) ||
                            item.Currency.toLowerCase().replace(/\s+/, "").match(rex) || item.UomName.toLowerCase().replace(/\s+/, "").match(rex) ||
                            barcode.toLowerCase().replace(/\s+/, "").match(rex);
                    });
                    $listItemM.bindRows(items);
                });
            }
        });
        itemMasterDataDialog.confirm(function () {
            itemMasterDataDialog.shutdown()
        })
    })
$("#copyfrom").change(function()
{
    if(parseInt(this.value)==1)
    {
        CopytransferRequest();
    }
    
});
function CopytransferRequest()
{
    const DataDialog = new DialogBox({
        content: {
            selector: "#content-transfer",
        },
        caption: "Item Transfer Request",
    });
    DataDialog.invoke(function () {
        $.get("/Transfer/GetTransferRequest",function(res){
            $tableTransferrequest.bindRows(res);
        })
        const $tableTransferrequest = ViewTable({
            keyField: "ID",
            selector: $("#transferrequest"),
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: false,
            },
        
            visibleFields: ["Doctype", "Number", "Whname", "PostingDate", "Time"],
            columns: [
            
            ],
            actions: [
                {
                    template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                    on: {
                        "click": function (e) 
                        {
                            $.get("/Transfer/CopyTransferRequest",{number: e.data.Number},function(data){
                            data_itemM_show = [];
                            $("#baseonid").val(data.ID);
                            $("#txtwarehouse_from").val(data.WarehouseFromID);
                           $("#txtwarehouse_to").val(data.WarehouseToID);
                           
                            $("#txtbranch").val(data.BranchID);
                         
                            setDate("#txtPostingdate",data.PostingDate.toString().split("T")[0]);
                            setDate("#txtDocumentDate", data.DocumentDate.toString().split("T")[0]);
                            $("#txtuserrequest").val(data.UserRequestID);
                            $("#txttime").val(data.Time);
                            $("#txtRemark").val(data.Remark);
                            
                            $listItemMShow.bindRows(data.TransferViewModels);
                            data.TransferViewModels.forEach(i=>{
                                data_itemM_show.push(i);
                            });
                            $.get("/Transfer/GetItemByWarehouseFrom",{warehouesId:data.WarehouseFromID},function(res){
                                res.forEach(i=> {
                                    item_masters.push(i);
                                });
                                
                            });
                           });
                           $("#copyfrom").val(0);
                           DataDialog.shutdown();
                        }
                    }
                }
            ]
        });


    });
    DataDialog.confirm(function () {
        DataDialog.shutdown()
    })
}


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
        visibleFields: ["FWarehouse", "TWarehouse", "Code", "KhmerName", "EnglishName", "Qty", "Cost", "UoMs", "Type", "InStock"],
        columns: [
            {
                name: "FWarehouse",
                template: `<select></select>`,
                on: {
                    "change": function (e) {
                        updatedetail(data_itemM_show, e.key, "FWarehouseID", parseFloat(this.value));
                    }
                }
            },
            {
                name: "TWarehouse",
                template: `<select></select>`,
                on: {
                    "change": function (e) {
                        updatedetail(data_itemM_show, e.key, "TWarehouseID", parseFloat(this.value));
                    }
                }
            },
            {
                name: "Cost",
                dataType: "number",
            },
            {
                name: "InStock",
                dataType: "number",
            },
            {
                name: "Qty",
                template: `<input>`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        if (this.value === "" || this.value === '-' || this.value < 0) this.value = 0;
                        if (this.value > e.data.InStock) this.value = e.data.InStock;
                        updatedetail(data_itemM_show, e.key, "Qty", parseFloat(this.value));
                    }
                }
            },
            {
                name: "UoMs",
                template: `<select></select>`,
                on: {
                    "change": function (e) {
                        const uoM = findArray("ID", this.value, e.data.UoMsList);
                        if (this.value != uoM.BaseUoMID) {
                            $listItemMShow.updateColumn(e.key, "InStock", e.data.InStock / uoM.Factor);
                            $listItemMShow.updateColumn(e.key, "Cost", e.data.Cost * uoM.Factor);
                        }
                        else {
                            $listItemMShow.updateColumn(e.key, "InStock", e.data.QuantitySum);
                            $listItemMShow.updateColumn(e.key, "Cost", e.data.CostStore);
                        }
                        updatedetail(data_itemM_show, e.key, "UomID", this.value);
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
                template: `<i class="fa fa-trash hover"></i>`,
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
                    url: "/Transfer/FindBarcode",
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
                                $listItemMShow.clearHeaderDynamic(data_itemM_show[0].AddictionProps);
                                $listItemMShow.createHeaderDynamic(data_itemM_show[0].AddictionProps);
                                $listItemMShow.bindRows(data_itemM_show);
                                $("#txtbarcode").val('');
                            }
                        }
                    },
                })
            }
        }
    });
    //// Send data to server ////
    $("#saveData").click(function () {
        saveData(this);
    });
    function updatedetail(data, key, prop, value) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i.LineID === key) {
                    i[prop] = value;
                }
            });
        }
    }
    function getAllPurItemsByItemID(wareid, ID, success) {
        $.get("/Transfer/GetAllPurItemsByItemID", { wareid, ID }, success);
    }
});
//Filter warehouse from
function changeWarehouseFrom() {
    let id = $(this).find("option:selected").val();
    if (isValidArray(item_masters)) {
        item_masters = [];
        item_masters = $.ajax({
            url: "/Transfer/GetItemByWarehouseFrom",
            type: "GET",
            async: false,
            dataType: "JSON",
            data: { warehouesId: id }
        }).responseJSON;
    } else {
        item_masters = $.ajax({
            url: "/Transfer/GetItemByWarehouseFrom",
            type: "GET",
            async: false,
            dataType: "JSON",
            data: { warehouesId: id }
        }).responseJSON;
    }
}
// filter branch
function filterBranch() {
    var branchId = $("#txtbranch").val();
    $.ajax({
        url: "/Transfer/GetWarehouse_By_filterBranch",
        type: "GET",
        dataType: "JSON",
        data: { BranchID: branchId },
        success: function (e) {
            var data = "";
            $("#txtwarehouse_to option").remove();
            $.each(e, function (i, item) {
                data +=
                    '<option value="' + item.ID + '">' + item.Name + '</option>';
            });
            $("#txtwarehouse_to").append(data);
        }
    })
}
// filter warehouse to
function changeWarehouseTo() {

}
//save data in purchase quotuion
function saveData(_this) {
    
    const warehouse_from = $("#txtwarehouse_from").val();
    const warehouse_to = $("#txtwarehouse_to").val();
    const number = $("#txtnumber").val();
    const branch = $("#txtbranch").val();
    const posting = $("#txtPostingdate").val();
    const document = $("#txtDocumentDate").val();
    const user_request = $("#txtuserrequest").val();
    const time = $("#txttime").val();
    const remark = $("#txtRemark").val();
    const seriesId = $("#txtInvoice").val();
    var  branchID = parseInt($("#branch").val());
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
        $(_this).prop("disabled", true);
        const data = {
            BaseOnID: parseInt($("#baseonid").val()),
            BranchID: branchID,
            UserID: __UserID,
            UserRequestID: user_request,
            BranchToID: branch,
            WarehouseFromID: warehouse_from,
            WarehouseToID: warehouse_to,
            PostingDate: posting,
            DocumentDate: document,
            Number: number,
            Remark: remark,
            Time: time,
            SeriseID: seriesId,
            TransferDetails: data_itemM_show
        }
        $.ajax({
            url: "/Transfer/SaveTransfer",
            type: "POST",
            dataType: "JSON",
            data: {
                transfer: data,
                serial: JSON.stringify(serials),
                batch: JSON.stringify(batches),
            },
            success: function (res) {
                if (res.IsSerail) {
                    $(_this).prop("disabled", false);
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
                    $(_this).prop("disabled", false);
                    const batch = BatchNoTemplate({
                        data: {
                            isGoodsIssue: true,
                            batches: res.Data,
                        }
                    });
                    batch.batchTemplate();
                    const seba = batch.callbackInfo();
                    batches = seba.batches;
                }else if (res.IsApproved) {
                    new ViewMessage({
                        summary: {
                            selector: ".err-succ-message"
                        },
                    }, res).refresh(1000);
                } else {
                    $(_this).prop("disabled", false);
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
function setDate(selector, date_value) {
    var _date = $(selector);
    _date[0].valueAsDate = new Date(date_value);
    _date[0].setAttribute("data-date",moment(_date[0].value).format(_date[0].getAttribute("data-date-format"))
    );
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