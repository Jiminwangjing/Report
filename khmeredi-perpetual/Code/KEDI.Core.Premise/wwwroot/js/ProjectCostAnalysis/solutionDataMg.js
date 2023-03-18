"use strict"
let quote_details = [];
let ExChange = [];
let master = [];
let _currency = "";
let _curencyID = 0;
let _priceList = 0;

const ___PA = JSON.parse($("#data-invoice").text());
const disSetting = ___PA.genSetting.Display;

$(document).ready(function () {
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    })
    $.each($("[data-date]"), function (i, t) {
        setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
    });
    // tb master
    var itemmaster = {
        ID: 0,
        Name:"",
        CusID: 0,
        ConTactID:0,
        BranchID: 0,
        WarehouseID: 0,
        UserID: 0,
        SaleCurrencyID: 0,
        RefNo: "",
        InvoiceNo: "",
        PostingDate: "",
        ValidUntilDate: "",
        Status: "open",
        Remarks: "",
        SaleEMID: 0,
        OwnerID:0,
       
        SolutionDataManagementDetails: new Array()
    }
    master.push(itemmaster);
    // select inoice with defualt == true
    ___PA.seriesPA.forEach(i => {
        if (i.Default == true) {
            $("#DocumentTypeID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    let selected = $("#invoice-no");
    selectSeries(selected);
    $('#invoice-no').change(function () {
        var id = ($(this).val());
        var seriesPA = findArray("ID", id, ___PA.seriesPA);
        ___PA.seriesPA.Number = seriesPA.NextNo;
        ___PA.seriesPA.ID = id;
        $("#DocumentID").val(seriesPA.DocumentTypeID);
        $("#number").val(seriesPA.NextNo);
        $("#next_number").val(seriesPA.NextNo);
    });
    if (___PA.seriesPA.length == 0) {
        $('#invoice-no').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
        $("#submit-item").prop("disabled", true);
    }
    // get warehouse
    //setWarehouse(0);
    // get Default currency
    $.ajax({
        url: "/ProjectCostAnalysis/GetDefaultCurrency",
        type: "Get",
        dataType: "Json",
        async: false,
        success: function (response) {
            _currency = response.Description;
            _curencyID = response.ID;
            $(".cur-class").text(_currency);
        }
    });
    // date format
    $("input[type='date']").on("change", function () {
        this.setAttribute(
            "data-date",
            moment(this.value, "YYYY-MM-DD")
                .format(this.getAttribute("data-date-format"))
        )
    });
    // get  currency option
    $.ajax({
        url: "/ProjectCostAnalysis/GetPriceList",
        type: "Get",
        dataType: "Json",
        success: function (response) {
            $("#cur-id option:not(:first-child)").remove();
            $.each(response, function (i, item) {
                $("#cur-id").append("<option  value=" + item.CurrencyID + ">" + item.Name + "</option>");
            });
        }
    });
 
    $("#cus-choosed").click(function () {
        $(".cus-id").val(_nameCus);
        master.forEach(i => {
            i.CusID = _idCus;
        })
        $("#barcode-reading").prop("disabled", false).focus();
        $("#item-id").prop("disabled", false);
        
    });
    
    //// Bind Item Choosed ////
    const $listItemChoosed = ViewTable({
        keyField: "LineID",
        selector: "#list-detail",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: true
        },
       
        visibleFields: [
            "ItemCode", "BarCode", "Description", "Qty", "UoMs", "Currency",  "InStock",  "Remarks"],
        columns: [
           
            {
                name: "Currency",
                template: `<span class='font-size cur'></span`,
            },
           
           
            {
                name: "Qty",
                template: `<input class='font-size' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                       
                        updatedetail(quote_details, e.data.UomID, e.key, "Qty", this.value);
                     
                    }
                }
            },
           
            {
                name: "UoMs",
                template: `<select class='font-size'></select>`,
                on: {
                    "change": function (e) {
                        updatedetail(quote_details, e.data.UomID, e.key, "UomID", parseInt(this.value));
                        const uomList = findArray("UoMID", this.value, e.data.UomPriceLists);
                        const uom = findArray("ID", this.value, e.data.UoMsList);
                        if (!!uom && !!uomList) {
                            updatedetail(quote_details, e.data.UomID, e.key, "UnitPrice", uomList.UnitPrice);
                            updatedetail(quote_details, e.data.UomID, e.key, "UomName", uom.Name);
                            updatedetail(quote_details, e.data.UomID, e.key, "Factor", uom.Factor);
                            $listItemChoosed.updateColumn(e.key, "UnitPrice", num.formatSpecial(uomList.UnitPrice, disSetting.Prices));
                            
                            
                           // disInputUpdate(master[0]);
                        }
                    }
                }
            },
            {
                name: "InStock",
                template: `<input class='font-size'>`,
                on: {
                    "keyup": function (e) {
                        updatedetail(quote_details, e.data.UomID, e.key, "InStock", this.value);
                    }
                }
            },
            {
                name: "Remarks",
                template: `<input>`,
                on: {
                    "keyup": function (e) {
                        updatedetail(quote_details, e.data.UomID, e.key, "Remarks", this.value);
                    }
                }
            }
        ],
        actions: [
            {
                template: `<i class="fas fa-trash"></i>`,
                on: {
                    "click": function (e) {
                        if (e.data.ID == 0) {
                            $listItemChoosed.removeRow(e.key);
                            quote_details = quote_details.filter(i => i.LineID !== e.key);
                        }
                       
                       // disInputUpdate(master[0]);
                    }
                }
            }
        ]
    });
    //// change currency 
    $("#cur-id").change(function () {
        var $this = this;
        _priceList = $this.value;
        $.ajax({
            url: "/ProjectCostAnalysis/GetSaleCur",
            type: "Get",
            dataType: "Json",
            data: { PriLi: _priceList },
            success: function (res) {
                if (master.length > 0)
                {
                    master.forEach(i => {
                      
                        i.SaleCurrencyID = res.CurrencyID;
                        i.PriceListID = _priceList;
                    });
                   
                }
                quote_details = [];
                $listItemChoosed.clearRows();
                $listItemChoosed.bindRows(quote_details);
            }
        });
    })

  

    //// Choose Item by BarCode ////
    $(window).on("keypress", function (e) {
        if (e.which === 13) {
            if (document.activeElement === this.document.getElementById("barcode-reading")) {
                let activeElem = this.document.activeElement;
                if (master[0].CurID != 0) {
                    $.get("/ProjectCostAnalysis/GetItemDetails", { PriLi: _priceList, itemId: 0, barCode: activeElem.value.trim(), uomId: 0 }, function (res) {
                        if (res.IsError) {
                            new DialogBox({
                                content: res.Error,
                            });
                        } else {
                            if (isValidArray(quote_details)) {
                                res.forEach(i => {
                                    const isExisted = quote_details.some(qd => qd.ItemID === i.ItemID);
                                    if (isExisted) {
                                        const item = quote_details.filter(_i => _i.BarCode === i.BarCode)[0];
                                        if (!!item) {
                                            const qty = parseFloat(item.Qty) + 1;
                                            const openqty = parseFloat(item.Qty) + 1;
                                            updatedetail(quote_details, i.UomID, item.LineID, "Qty", qty);
                                            updatedetail(quote_details, i.UomID, item.LineID, "OpenQty", openqty);
                                            $listItemChoosed.updateColumn(item.LineID, "Qty", qty);
                                           
                                        }
                                    } else {
                                        quote_details.push(i);
                                        $listItemChoosed.addRow(i);
                                    }
                                })
                            } else {
                                res.forEach(i => {
                                    quote_details.push(i);
                                    $listItemChoosed.addRow(i);
                                })
                            }
                         
                            activeElem.value = "";
                        }
                    });
                }

            }
        }
    });
    //// Find Sale Qoute ////
    $("#btn-find").on("click", function () {
        $("#btn-addnew").prop("hidden", false);
        $("#btn-find").prop("hidden", true);
        $("#next_number").val("").prop("readonly", false).focus();
    });
    $("#next_number").on("keypress", function (e) {
        if (e.which === 13 && !!$("#invoice-no").val().trim()) {
            $.ajax({
                url: "/ProjectCostAnalysis/FindSolutionData",
                data: { number: $("#next_number").val(), seriesID: parseInt($("#invoice-no").val()) },
                success: function (result) {
                   
                    getSaleItemMasters(result);
                }
            });
        }
    });
    //// Bind Item Master ////
    function BindItemMastor(result) {
        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: "List Item Data",
            content: {
                selector: "#ModalItem"
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown();
        })
        dialogprojmana.invoke(function () {
            const itemMaster = ViewTable({
                keyField: "ID",
                selector: $("#list-item"),
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Code", "KhmerName", "Currency", "UnitPrice", "BarCode", "UoM", "InStock"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                $.get("/ProjectCostAnalysis/GetItemDetails", { PriLi: e.data.PricListID, itemId: e.data.ItemID, barCode: "", uomId: e.data.UomID }, function (res) {
                                   
                                    res[0].TaxRate = num.formatSpecial(res[0].TaxRate, disSetting.Prices);
                                    res[0].TaxValue = num.formatSpecial(res[0].TaxValue, disSetting.Prices);
                                    res[0].TaxOfFinDisValue = num.formatSpecial(res[0].TaxOfFinDisValue, disSetting.Prices);
                                    res[0].FinDisRate = num.formatSpecial(res[0].FinDisRate, disSetting.Prices);
                                    res[0].FinDisValue = num.formatSpecial(res[0].FinDisValue, disSetting.Prices);
                                    res[0].Total = num.formatSpecial(res[0].Total, disSetting.Prices);
                                    res[0].Cost = num.formatSpecial(res[0].Cost, disSetting.Prices);
                                    res[0].LineTotalCost = num.formatSpecial(res[0].LineTotalCost, disSetting.Prices);
                                    res[0].UnitMargin = num.formatSpecial(res[0].UnitMargin, disSetting.Prices);
                                    res[0].TotalWTax = num.formatSpecial(res[0].TotalWTax, disSetting.Prices);
                                    res[0].LineTotalMargin = num.formatSpecial(res[0].LineTotalMargin, disSetting.Prices);
                                    res[0].FinTotalValue = num.formatSpecial(res[0].FinTotalValue, disSetting.Prices);
                                    res[0].UnitPriceAfterDis = num.formatSpecial(res[0].UnitPriceAfterDis, disSetting.Prices);
                                    res[0].LineTotalBeforeDis = num.formatSpecial(res[0].LineTotalBeforeDis, disSetting.Prices);
                                    quote_details.push(res[0]);
                                    // $listItemChoosed.addRow(quote_details);
                                    $listItemChoosed.bindRows(quote_details);
                                   
                                   
                                });
                            }
                        }
                    }
                ]
            });
            itemMaster.bindRows(result);
        });

    }
   
    //// choose item
    $("#item-id").click(function () {
        //$("#ModalItem").modal("show");
        $("#loadingitem").prop("hidden", false);
        if (master.CurID != 0) {
            $.ajax({
                url: "/ProjectCostAnalysis/GetItem",
                type: "Get",
                dataType: "Json",
                data: { PriLi: _priceList },
                success: function (res) {
 
                    if (res.status == "T") {
                        if (res.ls_item.length > 0) {
                            BindItemMastor(res.ls_item)
                          //  itemMaster.bindRows(res.ls_item);
                            $("#loadingitem").prop("hidden", true);
                            $(".modal-dialog #find-item").on("keyup", function (e) {
                                let __value = this.value.toLowerCase().replace(/\s+/, "");
                                let items = $.grep(res.ls_item, function (item) {
                                    return item.Barcode === __value || item.KhmerName.toLowerCase().replace(/\s+/, "").includes(__value)
                                        || item.UnitPrice == __value || item.Code.toLowerCase().replace(/\s+/, "").includes(__value)
                                        || item.UoM.toLowerCase().replace(/\s+/, "").includes(__value);
                                });
                                BindItemMastor(items)
                                //itemMaster.bindRows(items);
                            });
                        }
                        else {
                            $("#loadingitem").prop("hidden", true);
                        }
                        setTimeout(() => {
                            $(".modal-dialog #find-item").focus();
                        }, 300);
                    }
                }
            });
        }
    });

    
    function getSaleItemMasters(_master) {
       
        if (!!_master)
        {
            quote_details = [];
            master[0] = _master.ProjectCostAnalysis;
           
            _master.DetailItemMasterDatas.forEach(i => {
               
                quote_details.push(i);
            });
            
            
            _priceList = _master.Currency.ID;
            $("#txt_proid").val(_master.ProjectCostAnalysis.ID);
            $("#txt_proname").val(_master.ProjectCostAnalysis.Name);
            $("#txt_cusname").val(_master.ProjectCostAnalysis.CusName);
            $("#txt_idcus").val(_master.ProjectCostAnalysis.CusID);
            $("#txt_cuscode").val(_master.ProjectCostAnalysis.CusCode); 
            $("#txt_idcontect").val(_master.ProjectCostAnalysis.ConTactID);
            $("#txt_contactperson").val(_master.ProjectCostAnalysis.ContName);
            $("#txt_phone").val(_master.ProjectCostAnalysis.Phone);
            $("#invoice-no").val(_master.ProjectCostAnalysis.SeriesID);
            $("#invoice-no").val(_master.ProjectCostAnalysis.SeriesID);
            $("#next_number").val(_master.ProjectCostAnalysis.InvoiceNumber);

            $("#ware-id").val(_master.ProjectCostAnalysis.WarehouseID);
            $("#ref-id").val(_master.ProjectCostAnalysis.RefNo);
            $("#cur-id").val(_master.ProjectCostAnalysis.SaleCurrencyID);
            $("#sta-id").val(_master.ProjectCostAnalysis.Status);
           
            setDate("#post-date", _master.ProjectCostAnalysis.PostingDate.toString().split("T")[0]);
            setDate("#valid-date", _master.ProjectCostAnalysis.ValidUntilDate.toString().split("T")[0]);
           
            $("#txt_idem").val(_master.ProjectCostAnalysis.SaleEMID);
            $("#saleem").val(_master.ProjectCostAnalysis.EmName);
            $("#txt_idowner").val(_master.ProjectCostAnalysis.OwnerID);
            $("#owner").val(_master.ProjectCostAnalysis.OwnerName);
            $("#remark-id").val(_master.ProjectCostAnalysis.Remarks);
            
          
            $("#item-id").prop("disabled", false);
            var ex = findArray("CurID", _master.ProjectCostAnalysis.SaleCurrencyID, ExChange[0]);
            if (!!ex) {
               
                $(".cur-class").text(ex.CurName);
            }
      
            $listItemChoosed.bindRows(quote_details);
           
        } else {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Project CostAnalysis Not found!",
                close_button: "none"
            });
        }
    }

    /// submit data ///
    $("#submit-item").on("click", function (e) {
        const item_master = master[0];
       
        item_master.Name = $("#txt_proname").val();
        item_master.CusID = parseInt($("#txt_idcus").val());
        item_master.ConTactID = parseInt($("#txt_idcontect").val());
        item_master.WarehouseID = parseInt($("#ware-id").val()); 
        item_master.RefNo = $("#ref-id").val();
        item_master.PostingDate = $("#post-date").val();
        item_master.ValidUntilDate = $("#valid-date").val();
        
        item_master.Remarks = $("#remark-id").val();
        item_master.SolutionDataManagementDetails = quote_details.length === 0 ? new Array() : quote_details;
       
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.SeriesDID = parseInt($("#SeriesDetailID").val());
        item_master.SeriesID = parseInt($("#invoice-no").val());
        item_master.DocTypeID = parseInt($("#DocumentTypeID").val());
        item_master.InvoiceNumber = parseInt($("#next_number").val());
       
        item_master.SaleEMID = $("#txt_idem").val();
        item_master.OwnerID = $("#txt_idowner").val();

       
      //  $("#loading").prop("hidden", false);
       //console.log(item_master);
     
        $.ajax({
            url: "/ProjectCostAnalysis/UpdateSolutionDataMG",
            type: "post",
            data: $.antiForgeryToken({ data: JSON.stringify(item_master) }),
            success: function (model) {
                $("#submit-item").html("Save");
                if (model.Action == 1) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        }
                    }, model).refresh(800);
                   // $("#loading").prop("hidden", true);
                }
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, model);
                $(window).scrollTop(0);
              //  $("#loading").prop("hidden", true);
            }
        });
    });
    setWarehouse(0);
    function setWarehouse(id) {
        $.ajax({
            url: "/ProjectCostAnalysis/GetWarehouse",
            type: "Get",
            dataType: "Json",
            success: function (response) {
                $("#ware-id option:not(:first-child)").remove();
                $.each(response, function (i, item) {
                    $("#ware-id").append("<option value=" + item.ID + ">" + item.Name + "</option>");
                    if (!!id && item.ID === id) {
                        $("#ware-id").val(id);
                    }
                });
            }
        });
    }
   
    // Modal Story ProjectCostAnalysis
    $("#History").click(() => {
        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: "List Solution Data Management",
            content: {
                selector: "#projectcost-content"
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown();
        });
        dialogprojmana.invoke(function () {
            /// Bind Customers /// 
            const $listprojectcost = ViewTable({
                keyField: "ID",
                selector: "#list-projcost",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["Name", "InvoiceNumber", "PostingDate", "ValidUntilDate"],

                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {

                                $.ajax({
                                    url: "/ProjectCostAnalysis/FindSolutionData",
                                    // data: { number: $("#next_number").val(), seriesID: ) },
                                    data: { number: e.data.InvoiceNumber, seriesID: e.data.SeriesID },
                                    success: function (result) {
                                     
                                        getSaleItemMasters(result);
                                        $("#btn-addnew").prop("hidden", false);
                                        $("#btn-find").prop("hidden", true);
                                        $("#submit-item").html("Update");

                                    }
                                });
                                dialogprojmana.shutdown();
                            }
                        }
                    }
                ]
            });
            $.get("/ProjectCostAnalysis/GetHiststorySolution", function (res) {
                $listprojectcost.bindRows(res);
            })
        });
    })
   
    function selectSeries(selected) {
        $.each(___PA.seriesPA, function (i, item) {
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
    function setDate(selector, date_value) {
        var _date = $(selector);
        _date[0].valueAsDate = new Date(date_value);
        _date[0].setAttribute(
            "data-date",
            moment(_date[0].value)
                .format(_date[0].getAttribute("data-date-format"))
        );
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
    function updatedetail(data, uomid, key, prop, value) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i.LineID === key && i.UomID === uomid) {
                    i[prop] = value;
                }
            });
        }
    }

    
    function updateSelect(data, key, prop)
    {
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
    // Copy Project Cost Analysis to SaleQuote
   

  
})
