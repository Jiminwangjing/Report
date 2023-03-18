"use strict";

$(document).ready(function () {
    const _ICJE = JSON.parse($("#data-invioce").text());
    let __singleElementJE;
    var invCounting=[];
    var d = new Date();
    document.getElementById("txtdate").valueAsDate = d;
   
    _ICJE.seriesIC.forEach(i => {
        if (i.Default == true) {
            $("#DocumentID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });

    _ICJE.seriesJE.forEach(i => {
        if (i.Default == true) {
            $("#JEID").val(i.ID);
            $("#JENumber").val(i.NextNo);
            __singleElementJE = findArray("ID", i.ID, _ICJE.seriesJE);
        }
    });
    var disSetting = _ICJE.GeneralSetting;
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });
    //invioce
    let selected = $("#seriesID");
    selectSeries(selected);
    function selectSeries(selected) {
        $.each(_ICJE.seriesIC, function (i, item) {
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
    $('#seriesID').change(function () {
        var id = parseInt($(this).val());
        var seriesIC = find("ID", id, ___PCJE.seriesIC);
        ___PCJE.seriesIC.Number = seriesIC.NextNo;
        ___PCJE.seriesIC.ID = id;
        $("#DocumentTypeID").val(seriesIC.DocumentTypeID);
        $("#number").val(seriesIC.NextNo);
        $("#next_number").val(seriesIC.NextNo);
    });
    if (_ICJE.seriesIC.length == 0) {
        $('#seriesID').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
    }
    // Get Branch 
    $.get("/InventoryCounting/GetBranch",function(res){
        let data=`<option value="0"> --- select branch ---</option>`;
        if(res.length>0)
        {
            res.forEach(i=>{
                data+=`<option value="${i.ID}">${i.Name}</opion>`;
            })
               
            $("#txtBranch").html(data);
        }
    });
   
    let $inventoryCounting = ViewTable({
        keyField: "LineID",
        selector: "#list-items",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: false
        },

        visibleFields: [
            "Barcode",
            "ItemNo",
            "ItemName",
            "Warehouse",
            "InstockQty",
            "Counted",
            "UomCountQty",
            "CountedQty",
            "Varaince",
            "UomName",
            "EmName"
        ],
        columns: [
            {
                name: "Barcode",
                template: "<input />",
                on: {
                    "change": function (e) {
                        let barcode=this.value;
                        $.get("/InventoryCounting/GetItemMaster",{barcode:barcode},function(res){
                           if(res.length>0)
                           {
                             UpdateRow(e.data,res[0]);
                           }
                         });
                        
                    }
                }
            },
            {
                name: "ItemNo",
                template: "<input  readonly/>",
                on: {
                    "click": function (e) {
                        DialogItemMaster(e.data);
                       
                    }
                }
            },
            {
                name: "ItemName",
                template: "<input  readonly/>",
                on: {
                    "click": function (e) {
                        DialogItemMaster(e.data);
                        
                    }
                }
            },
            {
                name: "Warehouse",
                template: "<select></select>",
                on: {
                    "change": function (e) {
                        updateDetail(invCounting, "LineID", e.data.LineID, "WarehouseID", parseInt(this.value));
                        $.get("/InventoryCounting/GetStockFromWarehouse",{itemID:e.data.ItemID,wID:parseInt(this.value),uomID:e.data.UomID},function(res){
                            if(res.InStock<=0)
                            {
                                $inventoryCounting.updateColumn(e.key, "Counted",false);
                                $inventoryCounting.updateColumn(e.key, "UomCountQty",num.formatSpecial(0,disSetting.Amounts));
                                $inventoryCounting.updateColumn(e.key, "CountedQty",num.formatSpecial(0,disSetting.Amounts));
                                $inventoryCounting.updateColumn(e.key, "Varaince",num.formatSpecial(0,disSetting.Amounts));
                            }
                            $inventoryCounting.updateColumn(e.key, "InstockQty",num.formatSpecial(res.InStock,disSetting.Amounts));
                           
                        });
                        
                    }
                }
            },
            {
                name: "Counted",
                template: `<input  type="checkbox" class="check"/>`,
                on: {
                    "click": function (e) {
                        let active=$(this).prop("checked")?true:false;
                        if(active)
                        {
                            let varial=num.toNumberSpecial(e.data.UomCountQty)-num.toNumberSpecial(e.data.InstockQty);
                            $inventoryCounting.updateColumn(e.key, "Varaince",num.formatSpecial(varial,disSetting.Amounts));
                            updateDetail(invCounting, "LineID", e.data.LineID, "Varaince", num.formatSpecial(varial,disSetting.Amounts));
                        }
                        else
                        {
                            $inventoryCounting.updateColumn(e.key, "Varaince",num.formatSpecial(0,disSetting.Amounts));
                            updateDetail(invCounting, "LineID", e.data.LineID, "Varaince", num.formatSpecial(0,disSetting.Amounts));
                        }
                        updateDetail(invCounting, "LineID", e.data.LineID, "Counted", active);
                        $inventoryCounting.updateColumn(e.key, "Counted",active);
                       
                    }
                }
            },
            {
                name: "UomCountQty",
                template: `<input />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        // $(this).css({"color":"red"});
                        // $(this).parents("tr").css({"color":"red"});
                   
                        // console.log($(this).parents("td").siblings().children().css({"color":"red"}));
                         
                        let value=parseFloat(this.value);
                         if(isNaN(value))
                        {
                            value=0;
                        }
                        value=parseFloat(value)
                        if(value>0)
                        {
                            
                            $inventoryCounting.updateColumn(e.key, "Counted",true);
                        }
                        else if(value<=0)
                        {
                            $inventoryCounting.updateColumn(e.key, "Counted",false);
                        }
                        let varial=value==0?0:value-num.toNumberSpecial(e.data.InstockQty);
                        $inventoryCounting.updateColumn(e.key, "CountedQty",num.formatSpecial(value,disSetting.Amounts));
                        $inventoryCounting.updateColumn(e.key, "Varaince",num.formatSpecial(varial,disSetting.Amounts));
                        updateDetail(invCounting, "LineID", e.data.LineID, "UomCountQty", value);
                        updateDetail(invCounting, "LineID", e.data.LineID, "CountedQty", num.formatSpecial(value,disSetting.Amounts));
                        updateDetail(invCounting, "LineID", e.data.LineID, "Varaince", num.formatSpecial(varial,disSetting.Amounts));    
                             
                    }
                }
            },
            {
                name: "EmName",
                template: `<input />`,
                on: {
                    "click": function (e) {

                        DialogEM(e.data);
                    }
                }
            },
        ],

    });
    $.get("/InventoryCounting/BindRows", function (res) {
       res.forEach(i=>{
            i.InstockQty=num.formatSpecial(i.InstockQty,disSetting.Amounts);
            i.CountedQty=num.formatSpecial(i.CountedQty,disSetting.Amounts);
            i.Varaince=num.formatSpecial(i.Varaince,disSetting.Amounts);
           
            invCounting.push(i);
       });
        $inventoryCounting.bindRows(invCounting);
        res.forEach(i=>{
            $inventoryCounting.disableColumns(i.LineID,["Warehouse"]);
        });

    });
    // Add row 
    $("#addrow").click(function() { 
        $.get("/InventoryCounting/AddRowINVCounting", function (res) { 
            res.InstockQty  =num.formatSpecial(res.InstockQty,disSetting.Amounts);
            res.CountedQty  =num.formatSpecial(res.CountedQty,disSetting.Amounts);
            res.Varaince    =num.formatSpecial(res.Varaince,disSetting.Amounts);
            invCounting.push(res); 
            $inventoryCounting.addRow(res); 
            $inventoryCounting.disableColumns(res.LineID,["Warehouse"]);
            
        }); 
    });

     // ================================= Submit Data=============================
     $("#btn_save").click(function(res){
        var obj={};
            obj.ID              =   parseInt($("#id").val());
            obj.DocTypeID       =   parseInt($("#DocumentID").val());
            obj.SeriesID        =   parseInt($("#seriesID").val());
            obj.SeriesDetailID  =   parseInt($("#SeriesDetailID").val());
            obj.BranchID        =   parseInt($("#txtBranch").val());
            obj.InvioceNumber   =   $("#next_number").val();
            obj.Status          =   $("#txtstaus").val();
            obj.Ref_No          =   $("#txtref_no").val();
           
            obj.Date            =   $("#txtdate").val();
            obj.Time            =   $("#txttime").val();
            obj.Remark          =   $("#txtremark").val();
            obj.InventoryCountingDetails=invCounting;

//return;
        $.ajax({
            url: "/InventoryCounting/SaveINVCounting",
            type: "Post",
            dataType: "Json",
            data: $.antiForgeryToken({ invcounting: obj, je: JSON.stringify(__singleElementJE) }),
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
                        
                    }, e.Model);
                }
                $(window).scrollTop(0);
               
            }
        });
     });


     //// find Inventory Counting
     $("#find").click(function(){
        if($(this).text()=="Find")
        {
            $(this).text("Create");
            $("#next_number").val("").prop("readonly",false).focus();
        }
        else{
            location.reload();
        }      
     });

     $("#next_number").keypress(function (event) {
        const keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == "13") {
            const invoiceNumber = this.value;
             $.get("/InventoryCounting/FindInventoryCounting",{number:invoiceNumber},function(res){
                FindInventoryCounting(res);
             });
           
        }
    });

     function FindInventoryCounting(data)
     {
            $("#id").val(data.ID);
            $("#DocumentID").val(data.DocTypeID);
            $("#seriesID").val(data.SeriesID);
            $("#SeriesDetailID").val(data.SeriesDetailID);
            $("#txtBranch").val(data.BranchID);
            $("#next_number").val(data.InvioceNumber);
            $("#txtstaus").val(data.Status);
            $("#txtref_no").val(data.Ref_No);
            $("#txtdate").val(data.Date.split("T")[0]);
            $("#txttime").val(data.Time);
            $("#txtremark").val(data.Remark);
            $("#btn_save").html("Update");
            if(data.InventoryCountingDetails.length>0)
            {
                invCounting=[];
                data.InventoryCountingDetails.forEach(i=>{
                    i.InstockQty  = num.formatSpecial(i.InstockQty,disSetting.Amounts);
                    i.UomCountQty  = num.formatSpecial(i.UomCountQty,disSetting.Amounts);
                    i.CountedQty  = num.formatSpecial(i.CountedQty,disSetting.Amounts);
                    i.Varaince  = num.formatSpecial(i.Varaince,disSetting.Amounts);
                    invCounting.push(i);
                });
                $inventoryCounting.bindRows(invCounting);
                invCounting.forEach(i=>{
                    if(i.ID==0)
                        $inventoryCounting.disableColumns(i.LineID,["Warehouse"]);
                });
            }
     }

function DialogItemMaster(obj)
{
    let dialogItem = new DialogBox({
        button: {
            ok: {
                text: "Close",
                callback: function () {
                    this.meta.shutdown();
                }
            }
        },
        content: {
            selector: "#content-item"
        }
    });
    dialogItem.invoke(function () {
        let $listItem = ViewTable({
            keyField: "ID",
            selector: "#tb_itemMasterdata",
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: true
            },
            visibleFields: ["ItemNo","ItemName","BarCode","InStock","Batch","Serial"],
            actions: [
                {
                    template: "<i class='fas fa-arrow-circle-down'></i>",
                    on: {
                        "click": function (e) {
                            UpdateRow(obj,e.data);
                            dialogItem.shutdown();
                        }
                    }
                }
            ]
        });
        $.get("/InventoryCounting/GetItemMaster",function(res){
            if(res.length>0)
            {
                res.forEach(i=>{
                    i.InstockQty=num.formatSpecial(i.InstockQty,disSetting.Amounts)
                });
              
                $listItem.clearRows();
               // $listItem.bindRows(res);
                $("#searchitem").keyup(function () {
                    let input = this.value.replace(/\s+/g, '');
                    let rex = new RegExp(input, "gi");
                    var filtereds = $.grep(res, function (item, i) {
                        return item.ItemNo.toLowerCase().replace(/\s+/, "").match(rex)
                                || item.ItemName.toLowerCase().replace(/\s+/, "").match(rex);
                    });
                    $listItem.bindRows(filtereds);
                });
                $listItem.bindRows(res);
            }
        });
    });
}
// Dialog Employee
function DialogEM(obj)
{
    let dialogEM = new DialogBox({
        button: {
            ok: {
                text: "Close",
                callback: function () {
                    this.meta.shutdown();
                }
            }
        },
        content: {
            selector: "#content-em"
        }
    });
    dialogEM.invoke(function () {
        let $listEm = ViewTable({
            keyField: "ID",
            selector: "#tb_employee",
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: true
            },
            visibleFields: ["Name","GenderDisplay","Address","Phone","Email","Position"],
            actions: [
                {
                    template: "<i class='fas fa-arrow-circle-down'></i>",
                    on: {
                        "click": function (e) {
                           
                            $inventoryCounting.updateColumn(obj.LineID, "EmName",e.data.Name);
                            $inventoryCounting.updateColumn(obj.LineID, "EmployeeID",e.data.ID);

                            updateDetail(invCounting, "LineID", obj.LineID, "EmName",e.data.Name);
                            updateDetail(invCounting, "LineID", obj.LineID, "EmployeeID",e.data.ID);
                           dialogEM.shutdown();
                        }
                    }
                }
            ]
        });
        $.get("/InventoryCounting/GetEmployee",function(res){
            if(res.length>0)
            {
                $listEm.clearRows();
                $("#searchem").keyup(function () {
                    let input = this.value.replace(/\s+/g, '');
                    let rex = new RegExp(input, "gi");
                    var filtereds = $.grep(res, function (item, i) {
                        return item.Name.toLowerCase().replace(/\s+/, "").match(rex)||item.Position.toLowerCase().replace(/\s+/, "").match(rex);
                    });
                    $listEm.bindRows(filtereds);
                });
                $listEm.bindRows(res);
            }
        });
    });
}
function UpdateRow(obj,data)
{
    updateDetail(invCounting, "LineID", obj.LineID, "Barcode", data.BarCode);
    updateDetail(invCounting, "LineID", obj.LineID, "ItemID", data.ID);
    updateDetail(invCounting, "LineID", obj.LineID, "ItemNo", data.ItemNo);
    updateDetail(invCounting, "LineID", obj.LineID, "ItemName", data.ItemName);
    updateDetail(invCounting, "LineID", obj.LineID, "InstockQty", num.formatSpecial(data.InStock,disSetting.Amounts));
    updateDetail(invCounting, "LineID", obj.LineID, "UomName", data.GuomName);
    updateDetail(invCounting, "LineID", obj.LineID, "UomID", data.GuomID);
   // updateDetail(invCounting, "LineID", obj.LineID, "Warehouse", data.WarehouseID);
    updateDetail(invCounting, "LineID", obj.LineID, "WarehouseID", data.WarehouseID);

    $inventoryCounting.updateColumn(obj.LineID, "Barcode",data.BarCode);
    $inventoryCounting.updateColumn(obj.LineID, "ItemID",data.ID);
    $inventoryCounting.updateColumn(obj.LineID, "ItemNo",data.ItemNo );
    $inventoryCounting.updateColumn(obj.LineID, "ItemName", data.ItemName);
    $inventoryCounting.updateColumn(obj.LineID, "InstockQty",num.formatSpecial(data.InStock,disSetting.Amounts));
    $inventoryCounting.updateColumn(obj.LineID, "UomName", data.GuomName);
    $inventoryCounting.updateColumn(obj.LineID, "UomID", data.GuomID);
    // $inventoryCounting.updateColumn(obj.LineID, "Warehouse", data.WarehouseID);
    $inventoryCounting.updateColumn(obj.LineID, "WarehouseID", data.WarehouseID);
    $inventoryCounting.disableColumns(obj.LineID,["Warehouse"],false);
    updateSelect(obj.Warehouse, data.WarehouseID, "Selected"); 
    $inventoryCounting.updateColumn(obj.LineID, "Warehouse",obj.Warehouse);
    let uomCountQty=num.toNumberSpecial(obj.UomCountQty);
    let inStock=num.toNumberSpecial(data.InStock);
    if(uomCountQty>0)
    {
        updateDetail(invCounting, "LineID", obj.LineID, "Counted",true);
        updateDetail(invCounting, "LineID", obj.LineID, "CountedQty", num.formatSpecial(uomCountQty,disSetting.Amounts));
        updateDetail(invCounting, "LineID", obj.LineID, "Varaince", num.formatSpecial(uomCountQty-inStock,disSetting.Amounts));
        $inventoryCounting.updateColumn(obj.LineID, "Counted",true);
        $inventoryCounting.updateColumn(obj.LineID, "CountedQty",num.formatSpecial(uomCountQty,disSetting.Amounts));
        $inventoryCounting.updateColumn(obj.LineID, "Varaince",num.formatSpecial(uomCountQty-inStock,disSetting.Amounts));
    }
}
function updateSelect(data, key, prop) { 
    if (isValidArray(data)) { 
        data.forEach(i => { 
            if (i.Value == key) { 
                i[prop] = true; 
            } else { 
                i[prop] = false; 
            } 
        }); 
    } 
}
function updateDetail(data, propKey, key, prop, value) {
    $.grep(data, function (item, index) {
        if (item[propKey] == key) {
            item[prop] = value;
        }
    });
    return this;
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
});



