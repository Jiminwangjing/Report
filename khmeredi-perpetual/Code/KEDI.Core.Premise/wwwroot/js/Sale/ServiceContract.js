"use strict";
let attchmentfiles = [];
let ar_details = [],
    _currency = "",
    _curencyID = 0,
    master = [],
    ExChange = [],
    ARDownPayments = [],
    freights = [],
    freightMaster = {},
    __singleElementJE,
    _nameCus = "",
    _idCus = 0,
    serials = [],
    batches = [],
    type = "AR Service",
    isCopied = false,
    __ardpSum = {
        sum: 0,
        taxSum: 0,
    },
    _priceList = 0,
    ___IN = JSON.parse($("#data-invoice").text()),
    /// class of colunms items ////
    itemArr = [".remark", ".uom", ".unitprice", ".disvalue", ".disrate", ".taxgroup"],
    disSetting = ___IN.genSetting.Display;
freightMaster.IsEditabled = true;
$(document).ready(function () {
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });
    $.each($("[data-date]"), function (i, t) {
        setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
    });
    // tb master
    var itemmaster = {
        ID: 0,
        CusID: 0,
        BranchID: 0,
        WarehouseID: 0,
        ContractTemplateID: 0,
        UserID: 0,
        SaleCurrencyID: 0,
        RefNo: "",
        Types: "",
        InvoiceNo: "",
        InvoiceNumber: "",
        ExchangeRate: 0,
        PostingDate: "",
        DueDate: "",
        DocumentDate: "",
        IncludeVat: false,
        Status: "open",
        Remarks: "",
        ContractType: "",
        SubTotal: 0,
        SubTotalSys: 0,
        DisRate: 0,
        DisValue: 0,
        TypeDis: "Percent",
        VatRate: 0,
        VatValue: 0,
        AppliedAmount: 0,
        FeeNote: 0,
        FeeAmount: 0,
        TotalAmount: 0,
        TotalAmountSys: 0,
        CopyType: 0,
        CopyKey: "",
        BasedCopyKeys: "",
        LocalSetRate: 0,
        PriceListID: 0,
        DownPayment: 0,
        BaseOnID: 0,
        AdditionalContractNo: "",
        ContractStartDate: "",
        ContractENDate: "",
        ContractRenewalDate: "",
        Remark: "",
        ContractTemplateID: 0,
        ServiceContractDetails: new Array(),
        AttchmentFiles: new Array(),

    }
    master.push(itemmaster);
    // Invoice
    ___IN.seriesIN.forEach(i => {
        if (i.Default == true) {
            $("#DocumentTypeID").val(i.DocumentTypeID);
            $("#SeriesDetailID").val(i.SeriesDetailID);
            $("#number").val(i.NextNo);
        }
    });
    ___IN.seriesJE.forEach(i => {
        if (i.Default == true) {
            $("#JEID").val(i.ID);
            $("#JENumber").val(i.NextNo);
            __singleElementJE = findArray("ID", i.ID, ___IN.seriesJE);
        }
    });
    let selected = $("#invoice-no");
    selectSeries(selected);
    $('#invoice-no').change(function () {
        var id = $(this).val();
        var seriesIN = findArray("ID", id, ___IN.seriesIN);
        ___IN.seriesIN.Number = seriesIN.NextNo;
        ___IN.seriesIN.ID = id;
        $("#DocumentID").val(seriesIN.DocumentTypeID);
        $("#number").val(seriesIN.NextNo);
        $("#next_number").val(seriesIN.NextNo);
    });
    if (___IN.seriesIN.length == 0) {
        $('#invoice-no').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
        $("#submit-item").prop("disabled", true);
    }
    // get warehouse
    setWarehouse(0);
    // get Default currency
    $.ajax({
        url: "/Sale/GetDefaultCurrency",
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
        url: "/Sale/GetPriceList",
        type: "Get",
        dataType: "Json",
        success: function (response) {
            $("#cur-id option:not(:first-child)").remove();
            $.each(response, function (i, item) {
                $("#cur-id").append("<option  value=" + item.ID + ">" + item.Name + "</option>");
            });
        }
    });
    $(".modal-header").on("mousedown", function (mousedownEvt) {
        var $draggable = $(this);
        var x = mousedownEvt.pageX - $draggable.offset().left,
            y = mousedownEvt.pageY - $draggable.offset().top;
        $("body").on("mousemove.draggable", function (mousemoveEvt) {
            $draggable.closest(".modal-dialog").offset({
                "left": mousemoveEvt.pageX - x,
                "top": mousemoveEvt.pageY - y
            });
        });
        $("body").one("mouseup", function () {
            $("body").off("mousemove.draggable");
        });
        $draggable.closest(".modal").one("bs.modal.hide", function () {
            $("body").off("mousemove.draggable");
        });
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
   
    // get customer
    var cus_id;
    $("#show-list-cus").click(function () {
        const cusDialog = new DialogBox({
            content: {
                selector: "#cus-content"
            },
            caption: "Customers",
        });
        cusDialog.invoke(function () {
            const customers = ViewTable({
                keyField: "ID",
                selector: "#list-cus",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "Type", "Phone"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                        on: {
                            "click": function (e) {

                                const id = e.key;
                                cus_id = e.data.ID;
                                const name = e.data.Name;
                                const code = e.data.Code;
                                $("#cus-code").val(code);
                                $("#cus-name").val(name);
                                $("#cus-id").val(e.data.ID);
                                $("#contactperson").find('option').remove();
                                e.data.ContactPeople.forEach(i => {
                                    if (i.SetAsDefualt) {
                                        $("#contactperson").append(`<option value='${i.ContactID}'  selected = "selected">${i.ContactID}</option>`)
                                    }
                                    else {
                                        $("#contactperson").append(`<option value='${i.ContactID}'>${i.ContactID}</option>`)
                                    }

                                })
                                master[0].CusID = id;
                                $("#item-id").prop("disabled", false);
                                $("#barcode-reading").prop("disabled", false).focus();
                                $("#copy-from").prop("disabled", false);
                                $("#tdp-dailog").removeClass("disabled");


                                //if (e.data.Days != 0) {
                                //    var posdate = new Date($("#post-date").val());
                                //    var days = e.data.Days;
                                //    posdate.setDate(posdate.getDate() + parseInt(days) - 1);
                                //    document.getElementById("due-date").valueAsDate = posdate;
                                //    $("#days").val(e.data.Days);
                                //}
                                //else if (e.data.Days == 0) {
                                //    var posdate = new Date($("#post-date").val());
                                //    var days = e.data.Days;
                                //    //for (var i = 0; i < e.data.Instiallment.length; i++) {
                                //    //    var days = e.data.Instiallment[i].Day;
                                //    //    break;

                                //    //}
                                //    //$("#days").val(days);
                                //    ////item.Instiallment.forEach(i => {
                                //    ////    break;
                                //    ////})
                                //    posdate.setDate(posdate.getDate() + parseInt(days) - 1);
                                //    document.getElementById("due-date").valueAsDate = posdate;
                                //    $("#days").val(e.data.Days);
                                //}


                                cusDialog.shutdown();
                                getARDownPayments(id, "close", 0, function (res) {
                                    if (isValidArray(res)) {
                                        if (isValidArray(ARDownPayments)) {
                                            ARDownPayments = [];
                                            res.forEach(i => {
                                                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                                                ARDownPayments.push(i);
                                            });
                                        }
                                        else {
                                            res.forEach(i => {
                                                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                                                ARDownPayments.push(i);
                                            });
                                        }
                                    }
                                });
                            }
                        }
                    }
                ]
            });
            $.ajax({
                url: "/Sale/GetCustomer",
                type: "Get",
                dataType: "Json",
                success: function (response) {

                    customers.bindRows(response);
                    $("#find-cus").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let rex = new RegExp(__value, "gi");
                        let __customers = $.grep(response, function (person) {
                            let phone = isEmpty(person.Phone) ? "" : person.Phone;
                            return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s+/, "").match(rex)
                                || phone.replace(/\s+/, "").match(rex)
                                || person.Type.match(rex)
                        });
                        customers.bindRows(__customers);
                    });
                }
            });

        })
        cusDialog.confirm(function () {
            cusDialog.shutdown()
        })
    });
    function isEmpty(value) {
        return value == null || value == undefined || value == "";
    }

    var $list_Attachfile = ViewTable(
        {
            keyField: "LineID",
            selector: "#tblsprojManagementAttach",
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["TargetPath", "FileName", "AttachmentDate"],

            actions: [
                {
                    template: `<i class="fas fa-paperclip"> <form method="post" id="form_upload" enctype="multipart/form-data">  
                                      <input type="file"accept=".xls, .xlsx, .pdf, .docx, .pptx,.txt" />                  
                                    </form></i>`,
                    on: {
                        "change": function (e) {
                            var fileUpload = $(this).children('form').children('input').get(0);
                            SaveFile($list_Attachfile, fileUpload, e.data.LineID);
                            $list_Attachfile.updateColumn(e.key, "FileName", e.FileName);
                            $list_Attachfile.updateColumn(e.key, "TargetPath", e.TargetPath);


                        }
                    }
                },
                {
                    template: `<i class= "fas fa-download" ></i >`,
                    on: {
                        "click": function (e) {
                            if (e.data.ID != 0) {
                                location.href = "/Sale/DowloadFile?AttachID=" + e.data.ID;

                            }
                        }
                    }
                },
                {
                    template: `<i class="far fa-trash-alt" id='dislike'></i>`,
                    on: {
                        "click": function (e) {
                            if (e.data.ID == 0) {
                                $.post("/Sale/RemoveFileFromFolderMastre", { file: e.data.FileName }, function (res) {
                                    $list_Attachfile.updateColumn(e.key, "TargetPath", "");
                                    $list_Attachfile.updateColumn(e.key, "FileName", "");
                                    $list_Attachfile.updateColumn(e.key, "AttachmentDate", "");

                                });
                            }
                            else {
                                $.post("/Sale/DeleteFileFromDatabase", { id: e.data.ID, key: e.data.LineIdM }, function (res) {
                                    $list_Attachfile.bindRows(res);
                                });
                                //$('.dislike').prop('disabled', true);
                            }
                        }
                    }
                },
            ],
        });
    $.get("/Sale/CreatedefaultRowAttchment", { num: 10, number: 0 }, function (res) {
        if (isValidArray(res)) {
            res.forEach(i => attchmentfiles.push(i));
            $list_Attachfile.bindRows(attchmentfiles);
        }
    });

    function SaveFile(tablename, inputfile, key) {
        var files = inputfile.files;
        // Create  a FormData object
        var fileData = new FormData();
        //// if there are multiple files , loop through each files
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }
        //Adding more keys/values here if need
        //fileData.append('Test', "Test Object values");
        $.ajax({
            url: '/Sale/SaveAttachment', //URL to upload files
            type: "POST", //as we will be posting files and other method POST is used
            //enctype: "multipart/form-data",
            processData: false, //remember to set processData and ContentType to false, otherwise you may get an error
            contentType: false,
            cache: false,
            data: fileData,
            success: function (result) {

                tablename.updateColumn(key, "TargetPath", result.TargetPath);
                tablename.updateColumn(key, "FileName", result.FileName);
                tablename.updateColumn(key, "AttachmentDate", result.AttachmentDate);

            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    }
    var name;
    var id;
    $("#setup-list-contractype").click(function () {
        $.ajax({
            url: "/Sale/GetContractTemplate",
            type: "Get",
            dataType: "Json",
            success: function (response) {
                name = response.Name;
                bindContractTemplate(response);
            }
        });
    });
    function bindContractTemplate(res) {

        let dialog = new DialogBox({
            content: {
                selector: ".type_containers"
            },
            caption: "Contract Template"
        });
        dialog.invoke(function () {
            const __listcontracttemplate = ViewTable({
                keyField: "ID",
                selector: "#list-contracttype",
                paging: {
                    pageSize: 15,
                    enabled: true
                },
                visibleFields: ["Name", "ContractName", "Description"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down" id="close"  ></i>`,
                        on: {
                            "click": function (e) {

                                id = e.data.ID;
                                name = e.data.Name;
                                $("#contracttemplate").val(e.data.ContractName)
                                $("#contracttemplateid").val(e.data.ID)
                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listcontracttemplate.bindRows(res)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    function updateData(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue) {
                    i[prop] = propValue;
                }
            })
        }
    }

    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    $("#post-date").change(function () {
        var days = $("#days").val();
        var sdays = new Date(this.value);
        sdays.setDate(sdays.getDate() + parseInt(days) - 1);
        document.getElementById("due-date").valueAsDate = sdays;
    })
    var semid = 0;
    var semnale = "";
    $("#cus-cancel").click(function () {
        $("#sale-emid").val(0);
        $("#sale-em").val("");
    });
    $("#cus-choosed").click(function () {
        $("#sale-emid").val(semid);
        $("#sale-em").val(semnale);
        $("#cus-id").val(_nameCus);
        master.forEach(i => {
            i.CusID = _idCus;
        })
        $("#barcode-reading").prop("disabled", false).focus();
        $("#item-id").prop("disabled", false);
        $("#ModalCus").modal("hide");
        $("#copy-from").prop("disabled", false);
        $("#tdp-dailog").removeClass("disabled");
        getARDownPayments(_idCus, "close", 0, function (res) {
            if (isValidArray(res)) {
                if (isValidArray(ARDownPayments)) {
                    ARDownPayments = [];
                    res.forEach(i => {
                        i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                        ARDownPayments.push(i);
                    });
                }
                else {
                    res.forEach(i => {
                        i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                        ARDownPayments.push(i);
                    });
                }
            }
        })
    });
    $.ajax({
        url: "/Sale/GetExchange",
        type: "Get",
        dataType: "Json",
        success: function (res) {
            $.each(res, function (i, item) {
                if (item.CurID == _curencyID) {
                    $("#ex-id").val(1 + " " + _currency + " = " + item.SetRate + " " + item.CurName);
                    master.forEach(j => {
                        j.ExchangeRate = item.Rate;
                    })
                }
            });
            ExChange.push(res);
        }
    });
    //// Bind Item Choosed ////
    const $listItemChoosed = ViewTable({
        keyField: "LineID",
        selector: $("#list-detail"),
        paging: {
            pageSize: 10,
            enabled: false
        },
        dynamicCol: {
            afterAction: true,
            headerContainer: "#col-to-append-after-detail",
        },
        visibleFields: [
            "ItemCode", "UnitPrice", "BarCode", "Qty", "TaxGroupList", "TaxRate", "TaxValue", "TotalWTax", "Total", "Currency",
            "UoMs", "ItemNameKH", "DisValue", "DisRate", "Remarks", "FinDisRate", "FinDisValue", "TaxOfFinDisValue", "FinTotalValue"
        ],
        columns: [
            {
                name: "UnitPrice",
                template: `<input class='form-control font-size unitprice' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        updatedetail(ar_details, e.data.UomID, e.key, "UnitPrice", this.value);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this);
                        setSummary(ar_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "Currency",
                template: `<span class='font-size cur'></span`,
            },
            {
                name: "DisValue",
                template: `<input class='form-control font-size disvalue' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        calDisValue(e, this);
                        setSummary(ar_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "DisRate",
                template: `<input class='form-control font-size disrate' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        calDisRate(e, this);
                        setSummary(ar_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "Qty",
                template: `<input class='form-control font-size qty' type='text' />`,
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        if (isCopied && this.value > e.data.OpenQty) this.value = e.data.OpenQty;
                        updatedetail(ar_details, e.data.UomID, e.key, "Qty", this.value);
                        //calDisrate(e);
                        calDisvalue(e);
                        totalRow($listItemChoosed, e, this);
                        setSummary(ar_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "TaxGroupList",
                template: `<select class='form-control font-size taxgroup'></select>`,
                on: {
                    "change": function (e) {
                        const taxg = findArray("ID", this.value, e.data.TaxGroups);
                        if (!!taxg) {
                            updatedetail(ar_details, e.data.UomID, e.key, "TaxRate", taxg.Rate);

                        } else {
                            updatedetail(ar_details, e.data.UomID, e.key, "TaxRate", 0);
                        }
                        updatedetail(ar_details, e.data.UomID, e.key, "TaxGroupID", parseInt(this.value))
                        totalRow($listItemChoosed, e, this);
                        setSummary(ar_details);
                        disInputUpdate(master[0]);
                    }
                }
            },
            {
                name: "UoMs",
                template: `<select class='form-control font-size uom'></select>`,
                on: {
                    "change": function (e) {
                        updatedetail(ar_details, e.data.UomID, e.key, "UomID", parseInt(this.value));
                        const uomList = findArray("UoMID", this.value, e.data.UomPriceLists);
                        const uom = findArray("ID", this.value, e.data.UoMsList);
                        if (!!uom && !!uomList) {
                            updatedetail(ar_details, e.data.UomID, e.key, "UnitPrice", uomList.UnitPrice);
                            updatedetail(ar_details, e.data.UomID, e.key, "UomName", uom.Name);
                            updatedetail(ar_details, e.data.UomID, e.key, "Factor", uom.Factor);
                            $listItemChoosed.updateColumn(e.key, "UnitPrice", num.formatSpecial(uomList.UnitPrice, disSetting.Prices));
                            calDisvalue(e);
                            totalRow($listItemChoosed, e, this);
                            setSummary(ar_details);
                            disInputUpdate(master[0]);
                        }
                    }
                }
            },
            {
                name: "Remarks",
                template: `<input class="form-control font-size remark">`,
                on: {
                    "keyup": function (e) {
                        updatedetail(ar_details, e.data.UomID, e.key, "Remarks", this.value);
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
                template: `<i class="fas fa-trash font-size"></i>`,
                on: {
                    "click": function (e) {
                        $listItemChoosed.removeRow(e.key);
                        ar_details = ar_details.filter(i => i.LineID !== e.key);
                        setSummary(ar_details);
                        disInputUpdate(master[0]);
                        //$("#source-copy").val("");
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
            url: "/Sale/GetSaleCur",
            type: "Get",
            dataType: "Json",
            data: { PriLi: _priceList },
            success: function (res) {
                $.get("/Sale/GetDisplayFormatCurrency", { curId: res.CurrencyID }, function (resp) {
                    disSetting = resp.Display;
                    const ex = ExChange[0].filter(i => {
                        return res.CurrencyID == i.CurID
                    })
                    if (master.length > 0) {
                        master.forEach(i => {
                            i.ExchangeRate = ex[0].Rate;
                            i.SaleCurrencyID = res.CurrencyID;
                            i.PriceListID = _priceList;

                        });
                    }
                    $("#ex-id").val(1 + " " + _currency + " = " + ex[0].SetRate + " " + ex[0].CurName);
                    $(".cur-class").text(ex[0].CurName);
                    // change currency name in the rows of items after choosed ///
                    $(".cur").text(ex[0].CurName);
                    ar_details = [];
                    $listItemChoosed.clearRows();
                    $listItemChoosed.bindRows(ar_details);
                    setSummary(ar_details);
                    disInputUpdate(master[0]);
                })
            }
        });
    })

    /// applied-amount ///
    $("#applied-amount").keyup(function () {
        $(this).asNumber();
        if (this.value < 0) this.value = 0;
        if (this.value > num.toNumberSpecial(master[0].TotalAmount)) {
            this.value = num.toNumberSpecial(master[0].TotalAmount);
        }
        //master[0].AppliedAmount = num.toNumberSpecial(this.value) + num.toNumberSpecial(master[0].DownPayment)
    });
    /// dis rate on invoice //
    $("#dis-rate-id").keyup(function () {
        $(this).asNumber();
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > 100) this.value = 100;
        const subtotal = num.toNumberSpecial($("#sub-id").val());
        const disvalue = (num.toNumberSpecial(this.value) * subtotal) === 0 ? 0 : (num.toNumberSpecial(this.value) * subtotal) / 100;
        $("#dis-value-id").val(num.toNumberSpecial(disvalue, disSetting.Prices));
        invoiceDisAllRowRate(this);
        if (parseFloat(disvalue) > 0) {
            const totalAfDis = subtotal - parseFloat(disvalue) + master[0].VatValue;
            $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
            master[0].TotalAmount = totalAfDis;
            master[0].DisValue = disvalue;
            master[0].DisRate = this.value;
            master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
        } else {
            setSummary(ar_details);
        }
    });
    /// dis value on invoice //
    $("#dis-value-id").keyup(function () {
        $(this).asNumber();
        const subtotal = num.toNumberSpecial($("#sub-id").val());
        const vat = num.toNumberSpecial($("#vat-value").val());
        if (this.value == '' || this.value < 0) this.value = 0;
        if (this.value > subtotal) this.value = subtotal;

        const disrate = (num.toNumberSpecial(this.value) * 100 === 0) || (subtotal === 0) ? 0 :
            (num.toNumberSpecial(this.value) * 100) / subtotal;
        $("#dis-rate-id").val(disrate);
        invoiceDisAllRowValue(disrate);
        if (parseFloat(disrate) > 0) {
            const totalAfDis = subtotal - num.toNumberSpecial(this.value) + master[0].VatValue;
            $("#sub-after-dis").val(num.formatSpecial(totalAfDis - master[0].VatValue, disSetting.Prices));
            $("#total-id").val(num.formatSpecial(totalAfDis, disSetting.Prices));
            master[0].TotalAmount = totalAfDis;
            master[0].DisValue = this.value;
            master[0].DisRate = disrate;
            master[0].TotalAmountSys = totalAfDis * master[0].ExchangeRate;
        } else {
            setSummary(ar_details);
        }
    });
    //// Choose Item by BarCode ////
    $(window).on("keypress", function (e) {
        if (e.which === 13) {
            if (document.activeElement === this.document.getElementById("barcode-reading")) {
                let activeElem = this.document.activeElement;
                if (master[0].CurID != 0) {
                    $.get("/Sale/GetItemDetails", { PriLi: _priceList, itemId: 0, barCode: activeElem.value.trim(), uomId: 0 }, function (res) {
                        if (res.IsError) {
                            new DialogBox({
                                content: res.Error,
                            });
                        } else {
                            if (isValidArray(ar_details)) {
                                res.forEach(i => {
                                    const isExisted = ar_details.some(qd => qd.ItemID === i.ItemID);
                                    if (isExisted) {
                                        const item = ar_details.filter(_i => _i.BarCode === i.BarCode)[0];
                                        if (!!item) {
                                            const qty = parseFloat(item.Qty) + 1;
                                            const openqty = parseFloat(item.Qty) + 1;
                                            updatedetail(ar_details, i.UomID, item.LineID, "Qty", qty);
                                            updatedetail(ar_details, i.UomID, item.LineID, "OpenQty", openqty);
                                            $listItemChoosed.updateColumn(item.LineID, "Qty", qty);
                                            const _e = { key: item.LineID, data: item };
                                            calDisvalue(_e);
                                            totalRow($listItemChoosed, _e, qty);
                                            //setSummary(ar_details);
                                        }
                                    } else {
                                        ar_details.push(i);
                                        $listItemChoosed.addRow(i);
                                    }
                                })
                            } else {
                                $listItemChoosed.clearHeaderDynamic(res[0].AddictionProps)
                                $listItemChoosed.createHeaderDynamic(res[0].AddictionProps)
                                res.forEach(i => {
                                    ar_details.push(i);
                                    $listItemChoosed.addRow(i);
                                })

                            }
                            setItemToAbled([...itemArr, ".qty"]);
                            setSummary(ar_details);
                            disInputUpdate(master[0]);
                            activeElem.value = "";
                        }
                    });
                }

            }
        }
    });

    // Freight Dialog //
    $("#freight-dailog").click(function () {
        const freightDialog = new DialogBox({
            content: {
                selector: "#container-list-freight"
            },
            caption: "Freight",
        });
        freightDialog.invoke(function () {
            if (freightMaster.IsEditabled) {
                freightsEditable(freightDialog);
            } else {
                freightsNoneEditable(freightDialog);
            }
        });
        freightDialog.confirm(function () {
            $("#freight-value").val(num.formatSpecial(freightMaster.AmountReven, disSetting.Prices));
            setSummary(ar_details);
            freightDialog.shutdown();
        });
    });
    getFreight(function (res) {
        freightMaster = res;
        freightMaster.IsEditabled = true;
        if (isValidArray(res.FreightSaleDetailViewModels)) {
            if (isValidArray(freights)) {
                freights = [];
                res.FreightSaleDetailViewModels.forEach(i => {
                    freights.push(i);
                });
            }
            else {
                res.FreightSaleDetailViewModels.forEach(i => {
                    freights.push(i);
                });
            }
        }
    });
    function getFreight(success) {
        $.get("/Sale/GetFreights", success);
    }
    // TDP Dialog //
    $("#tdp-dailog").click(function () {
        const ARDownPaymentsDialog = new DialogBox({
            content: {
                selector: "#container-list-tdp"
            },
            caption: "A/R Down Payments",
        });
        ARDownPaymentsDialog.invoke(function () {
            const ARDownPaymentsView = ViewTable({
                keyField: "ARDID",
                selector: ARDownPaymentsDialog.content.find(".tdp-lists"),
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: false
                },
                visibleFields: ["Docnumber", "DocType", "Remarks", "Amount", "DocDate", "Selected", "Currency"],
                columns: [
                    {
                        name: "Selected",
                        template: "<input type='checkbox'>",
                        on: {
                            "click": function (e) {
                                const _this = this;
                                if (e.data.CurrencyID !== master[0].SaleCurrencyID) {
                                    const done = new DialogBox({
                                        content: `Currency name "${_currency}" does not match with currency name "${e.data.Currency}"`,
                                        animation: {
                                            startup: {
                                                name: "slide-left",
                                                duration: 500,
                                            }
                                        }
                                    });
                                    done.confirm(function () {
                                        $(_this).prop("checked", false);
                                    })
                                } else {
                                    const isSelected = $(this).prop("checked") ? true : false;
                                    updateARDP(ARDownPayments, "ARDID", e.key, "Selected", isSelected);
                                }
                            }
                        }
                    },
                    {
                        name: "Docnumber",
                        on: {
                            "dblclick": function (e) {
                                ARDPDdialog(e.data.SaleARDPINCNDetails);
                            }
                        }
                    }
                ]
            });
            ARDownPaymentsView.bindRows(ARDownPayments);
        });
        ARDownPaymentsDialog.confirm(function () {
            //setSummary(ar_details);
            sumARDP(ARDownPayments);
            $("#total-dp-value").val(num.formatSpecial(__ardpSum.sum, disSetting.Prices));
            master[0].DownPayment = num.formatSpecial(__ardpSum.sum, disSetting.Prices);
            ARDownPaymentsDialog.shutdown();
        });
    });
    //// Find Sale AR ////
    $("#btn-find").on("click", function () {
        $("#btn-addnew").prop("hidden", false);
        $("#btn-find").prop("hidden", true);
        $("#next_number").val("").prop("readonly", false).focus();
        $("#copy-from").prop("disabled", true);
    });
    $("#next_number").on("keypress", function (e) {
        if (e.which === 13 && !!$("#invoice-no").val().trim()) {
            setDisabled();
            $("#submit-item").prop("disabled", true);
            $("#copy-from").prop("disabled", true);
            $("#brand").removeAttr("hidden", "hidden")
            $("#CountryOfOrigin").removeAttr("hidden", "hidden")
            $("#color").removeAttr("hidden", "hidden")
            $("#width").removeAttr("hidden", "hidden")
            $("#height").removeAttr("hidden", "hidden")
            $("#SellByDate").removeAttr("hidden", "hidden")

            $("#ware-idhide").prop("hidden", true)
            $("#ware-id").prop("hidden", false);
            $("#cur_pri").prop("hidden", true);
            $("#cur-id").prop("hidden", false);
            if ($("#next_number").val() == "*") {
                findInvoice("/Sale/GetServiceDisplay", "/Sale/FindServiceContract", "Fine ServiceContract", "ID", "ServiceContract", "ServiceContractDetail", 369);
            }
            else {
                $.ajax({
                    url: "/Sale/FindServiceContract",
                    data: { number: $("#next_number").val(), seriesID: parseInt($("#invoice-no").val()) },
                    success: function (result) {
                        getSaleItemMasters(result);
                    }
                });
            }
        }
    });
    function findInvoice(urlMaster, urlDetail, caption, key, keyMaster, keyDetail, type) {
        $.get(urlMaster, { cusId: master[0].CusID }, function (res) {
            res.forEach(i => {
                i.PostingDate = i.PostingDate.toString().split("T")[0];
                i.SubTotal = num.formatSpecial(i.SubTotal, disSetting.Prices);
                i.TotalAmount = num.formatSpecial(i.TotalAmount, disSetting.Prices);
            })
            let dialog = new DialogBox({
                content: {
                    selector: "#container-list-item"
                },
                button: {
                    ok: {
                        text: "Close"
                    }
                },
                caption: caption,
            });
            dialog.invoke(function () {
                const itemMaster = ViewTable({
                    keyField: key,
                    selector: $(".list-item"),
                    indexed: true,
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    visibleFields: ["InvoiceNumber", "PostingDate", "Currency", "SubTotal", "TotalAmount", "Remarks"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                            on: {
                                "click": function (e) {
                                    $("#sale-emid").val(res[0].SaleEmID);
                                    $("#sale-em").val(res[0].SaleEmName);
                                    $.get(urlDetail, { number: e.data.InvoiceNo, seriesId: e.data.SeriesID }, function (res) {
                                        getSaleItemMasters(res);
                                        dialog.shutdown();
                                    });
                                }
                            }
                        }
                    ]
                });
                if (res.length > 0) {
                    itemMaster.bindRows(res);
                    $("#txtSearch-item-copy").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s/g, "");
                        let items = $.grep(res, function (item) {
                            return item.InvoiceNumber.toLowerCase().replace(/\s/g, "").includes(__value) || item.SubTotal.toString().toLowerCase().replace(/\s/g, "").includes(__value)
                                || item.PostingDate.toLowerCase().replace(/\s/g, "").includes(__value) || item.TotalAmount.toString().toLowerCase().replace(/\s/g, "").includes(__value)
                                || item.Currency.toLowerCase().replace(/\s/g, "").includes(__value);
                        });
                        itemMaster.bindRows(items);
                    });
                }
            });
            dialog.confirm(function () {
                dialog.shutdown();
            });
        });
    }

    if (___IN.data.ServiceContract != null) {
        $("#submit-item").prop("disabled", true)
        $("#ware-idhide").prop("hidden", false)
        $("#ware-id").prop("hidden", true);
        $("#cur_pri").prop("hidden", false);
        $("#cur-id").prop("hidden", true);
        getSaleItemMasters(___IN.data);
    }
    $("#viewdetial").click(function () {
        if (id != 0) {
            //location.href = "/SetupContractTemplate/Index";
            //location.href = "/SetupContractTemplate/Index?name=" +name;
            location.href = "/SetupContractTemplate/Index?id=" + id;
        }

    })

    $("#btn-find").click(function () {
        $("#submit-item").attr("disabled", "disabled")
        $("#post-date").change(function () {
            setDisabled();
            $("#submit-item").prop("disabled", true);
            $("#copy-from").prop("disabled", true);
            $("#post-date").prop("disabled", false);
            $("#brand").removeAttr("hidden", "hidden")
            $("#CountryOfOrigin").removeAttr("hidden", "hidden")
            $("#color").removeAttr("hidden", "hidden")
            $("#width").removeAttr("hidden", "hidden")
            $("#height").removeAttr("hidden", "hidden")
            $("#SellByDate").removeAttr("hidden", "hidden")
            $.ajax({
                url: "/Sale/FindServiceContract",
                data: { post_date: $("#post-date").val(), seriesID: parseInt($("#invoice-no").val()) },
                success: function (result) {
                    getSaleItemMasters(result);
                }
            });
        });
        $("#contractrenewal").change(function () {
            setDisabled();
            $("#submit-item").prop("disabled", true);
            $("#copy-from").prop("disabled", true);
            $("#post-date").prop("disabled", false);
            $("#contractrenewal").prop("disabled", false);
            $("#brand").removeAttr("hidden", "hidden")
            $("#CountryOfOrigin").removeAttr("hidden", "hidden")
            $("#color").removeAttr("hidden", "hidden")
            $("#width").removeAttr("hidden", "hidden")
            $("#height").removeAttr("hidden", "hidden")
            $("#SellByDate").removeAttr("hidden", "hidden")
            $.ajax({
                url: "/Sale/FindServiceContract",
                data: { renewal_date: $("#contractrenewal").val(), seriesID: parseInt($("#invoice-no").val()) },
                success: function (result) {
                    getSaleItemMasters(result);
                }
            });
        });
        $("#contractno").on("keypress", function (e) {
            $("#submit-item").prop("disabled", true);
            $("#post-date").prop("disabled", false);
            $("#contractrenewal").prop("disabled", false);
            $("#brand").removeAttr("hidden", "hidden")
            $("#CountryOfOrigin").removeAttr("hidden", "hidden")
            $("#color").removeAttr("hidden", "hidden")
            $("#width").removeAttr("hidden", "hidden")
            $("#height").removeAttr("hidden", "hidden")
            $("#SellByDate").removeAttr("hidden", "hidden")
            if (e.which === 13 && $("#contractno").val()) {
                $("#btn-find").hide();
                $.ajax({
                    url: "/Sale/FindServiceContract",
                    data: { contract_no: $("#contractno").val(), seriesID: parseInt($("#invoice-no").val()) },
                    success: function (result) {
                        getSaleItemMasters(result);
                    }
                });
            }
        });
        $("#show-list-cus_search").removeAttr("hidden", "hidden");
        $("#show-list-cus").attr("hidden", "hidden");
        $("#show-list-cus_search").click(function () {
            const cusDialog = new DialogBox({
                content: {
                    selector: "#cus-content_search"
                },
                caption: "Customers",
            });
            cusDialog.invoke(function () {
                const customers_search = ViewTable({
                    keyField: "ID",
                    selector: "#list-cus_search",
                    indexed: true,
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    visibleFields: ["Code", "Name", "Type", "Phone"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                            on: {
                                "click": function (e) {
                                    $.ajax({
                                        url: "/Sale/FindServiceContract",
                                        data: { cus_id: e.data.ID, seriesID: parseInt($("#invoice-no").val()) },
                                        success: function (result) {

                                            getSaleItemMasters(result);
                                        }
                                    });
                                    cusDialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                $.ajax({
                    url: "/Sale/GetCustomer",
                    type: "Get",
                    dataType: "Json",
                    success: function (response) {
                        customers_search.bindRows(response);
                        $("#find-cus").on("keyup", function (e) {
                            let __value = this.value.toLowerCase().replace(/\s+/, "");
                            let rex = new RegExp(__value, "gi");
                            let __customers = $.grep(response, function (person) {
                                return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s+/, "").match(rex)
                                    || person.Phone.toLowerCase().replace(/\s+/, "").match(rex)
                                    || person.Type.match(rex)
                            });
                            customers_search.bindRows(__customers);
                        });
                    }
                });

            })
            cusDialog.confirm(function () {
                cusDialog.shutdown()
            })
        });
        $("#contractstartdate").change(function () {
            $.ajax({
                url: "/Sale/FindServiceContract",
                data: { contract_sdate: this.value, seriesID: parseInt($("#invoice-no").val()) },
                success: function (result) {
                    getSaleItemMasters(result);
                }
            });
        })
        $("#contractendtdate").change(function () {
            $.ajax({
                url: "/Sale/FindServiceContract",
                data: { contract_edate: this.value, seriesID: parseInt($("#invoice-no").val()) },
                success: function (result) {
                    getSaleItemMasters(result);
                }
            });
        })
    })




    //// choose item
    $("#item-id").click(function () {
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
                visibleFields: ["Code", "KhmerName", "Currency", "UnitPrice", "BarCode", "UoM", "InStock"],
                columns: [
                    {
                        name: "UnitPrice",
                        dataType: "number",
                    },
                    {
                        name: "InStock",
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
                                $.get("/Sale/GetItemDetails", { PriLi: e.data.PricListID, itemId: e.data.ItemID, barCode: "", uomId: e.data.UomID }, function (res) {
                                    ar_details.push(res[0]);
                                    $listItemChoosed.addRow(res[0])
                                    setItemToAbled(itemArr);
                                    setSummary(ar_details);
                                    disInputUpdate(master[0]);
                                });
                            }
                        }
                    }
                ]
            });
            if (master.CurID != 0) {
                $.ajax({
                    url: "/Sale/GetItem",
                    type: "Get",
                    dataType: "Json",
                    data: { PriLi: _priceList, wareId: $("#ware-id").val() },
                    success: function (res) {
                        if (res.length > 0) {
                            itemMaster.clearHeaderDynamic(res[0].AddictionProps);
                            itemMaster.createHeaderDynamic(res[0].AddictionProps);
                            $listItemChoosed.clearHeaderDynamic(res[0].AddictionProps);
                            $listItemChoosed.createHeaderDynamic(res[0].AddictionProps);
                            itemMaster.bindRows(res);
                            $("#loadingitem").prop("hidden", true);
                            $("#find-item").on("keyup", function (e) {
                                let __value = this.value.toLowerCase().replace(/\s+/, "");
                                let items = $.grep(res, function (item) {
                                    return item.Barcode === __value || item.KhmerName.toLowerCase().replace(/\s+/, "").includes(__value)
                                        || item.UnitPrice == __value || item.Code.toLowerCase().replace(/\s+/, "").includes(__value)
                                        || item.UoM.toLowerCase().replace(/\s+/, "").includes(__value);
                                });
                                itemMaster.bindRows(items);
                            });
                        }
                        setTimeout(() => {
                            $("#find-item").focus();
                        }, 300);
                    }
                });
            }
        });
        itemMasterDataDialog.confirm(function () {
            itemMasterDataDialog.shutdown();
        })
    });
    //Copy from quotation
    $("#copy-from").change(function () {
        switch (this.value) {
            case "1":
                initModalDialog("/Sale/GetSaleQuotes", "/Sale/GetSaleQuoteDetailCopy", "Sale Quotes (Copy)", "SQID", "SaleQuote", "SaleQuoteDetails", 1);
                type = "SQ";
                break;
            case "2":
                initModalDialog("/Sale/GetSaleOrders", "/Sale/GetSaleOrderDetailCopy", "Sale Orders (Copy)", "SOID", "SaleOrder", "SaleOrderDetails", 2);
                type = "SO";
                break;
            case "3":
                initModalDialog("/Sale/GetSaleDeliveries", "/Sale/GetSaleDeliveryDetailCopy", "Delivery (Copy)", "SDID", "SaleDelivery", "SaleDeliveryDetails", 3);
                type = "DN";
                break;
            //case "4":
            //    initModalDialogitem("/Sale/GetItemMasterData", "/Sale/GetItemDetailsForFranchise", "Franchise (Copy)", "ID");
            //    break;
        }
        $(this).val(0);
    });
    $("#btn-finddraft").click(function () {
        DraftDataDialog();
    });
    var draftID = 0;
    /// submit data ///
    $("#submit-item").on("click", function (e) {
        SubmitData(1);
    });
    $("#submit-draft").on("click", function (e) {
        SubmitData(2);
    });
    function SubmitData(savetype) {
        const item_master = master[0];
        freightMaster.FreightSaleDetails = freights.length === 0 ? new Array() : freights;
        item_master.WarehouseID = parseInt($("#ware-id").val());
        item_master.RefNo = $("#ref-id").val();
        item_master.Types = "SC";
        item_master.ContractTemplateID = $("#contracttemplateid").val();
        item_master.AdditionalContractNo = $("#contractno").val();
        item_master.ContractStartDate = $("#contractstartdate").val();
        item_master.ContractType = $("#contracttype").val();
        item_master.ContractENDate = $("#contractendtdate").val();
        item_master.ContractRenewalDate = $("#contractrenewal").val();
        item_master.Remark = $("#remark").val();
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.PostingDate = $("#post-date").val();
        item_master.SaleEmID = $("#sale-emid").val();
        item_master.DueDate = $("#due-date").val();
        item_master.DeliveryDate = $("#due-date").val();
        item_master.DocumentDate = $("#document-date").val();
        item_master.Remarks = $("#remark-id").val();
        item_master.ServiceContractDetails = ar_details.length === 0 ? new Array() : ar_details;
        item_master.AttchmentFiles = $list_Attachfile.yield().length == 0 ? new Array() : $list_Attachfile.yield();

        item_master.FreightSalesView = freightMaster;
        item_master.PriceListID = parseInt($("#cur-id").val());
        item_master.SeriesDID = parseInt($("#SeriesDetailID").val());
        item_master.SeriesID = parseInt($("#invoice-no").val());
        item_master.DocTypeID = parseInt($("#DocumentTypeID").val());
        item_master.InvoiceNumber = parseInt($("#next_number").val());
        item_master.AppliedAmount = parseInt($("#applied-amount").val());
        item_master.FreightAmount = $("#freight-value").val();
        item_master.DownPayment = num.toNumberSpecial(__ardpSum.sum);
        item_master.DownPaymentSys = num.toNumberSpecial(__ardpSum.sum) * num.toNumberSpecial(item_master.ExchangeRate);
        item_master.BranchID = parseInt($("#branch").val());
        const _this = this;
        $(_this).prop("disabled", true);
        //----------------------Noted----------------
        //savetype == 1 => data must save to ARReserveInvoice
        //savetype != 1 => data must save to DraftReserveInvoice
        //------------------------------------------------
        if (savetype == 1) {
            if (draftID != 0) {
                item_master.ID = 0;
                item_master.AttchmentFiles.forEach(i => { i.ID = 0; });
                item_master.ServiceContractDetails.forEach(i => { i.ID = 0; });
            }

            var dialogSubmit = new DialogBox({
                content: "Are you sure you want to save the item?",
                type: "yes-no",
                icon: "warning"
            });
            dialogSubmit.confirm(function () {
                $("#loading").prop("hidden", false);

                $.ajax({
                    url: "/Sale/CreateServiceContract",
                    type: "post",
                    data: $.antiForgeryToken({
                        data: JSON.stringify(item_master),
                        seriesJE: JSON.stringify(__singleElementJE),
                        ardownpayment: JSON.stringify(ARDownPayments),
                        _type: type,
                        serial: JSON.stringify(serials),
                        batch: JSON.stringify(batches),
                    }),
                    success: function (model) {
                        if (model.IsSerail) {
                            $("#loading").prop("hidden", true);
                            $(_this).prop("disabled", false);
                            const serial = SerialTemplate({
                                data: {
                                    serials: model.Data,
                                }
                            });
                            serial.serialTemplate();
                            const seba = serial.callbackInfo();
                            serials = seba.serials;
                        } if (model.IsBatch) {
                            $("#loading").prop("hidden", true);
                            $(_this).prop("disabled", false);
                            const batch = BatchNoTemplate({
                                data: {
                                    batches: model.Data,
                                }
                            });
                            batch.batchTemplate();
                            const seba = batch.callbackInfo();
                            batches = seba.batches;
                        } if (model.Model.IsApproved) {
                            $.get("/Sale/DeleteDraftServiceContract", { id: draftID }, function () { })
                            new ViewMessage({
                                summary: {
                                    selector: "#error-summary"
                                }
                            }, model.Model).refresh(1500);
                            $(_this).prop("disabled", false);
                            $("#loading").prop("hidden", true);
                        }
                        new ViewMessage({
                            summary: {
                                selector: "#error-summary"
                            }
                        }, model.Model);
                        $(window).scrollTop(0);
                        $(_this).prop("disabled", false);
                        $("#loading").prop("hidden", true);
                    }
                });
                this.meta.shutdown();
            });
            dialogSubmit.reject(function () {
                $(_this).prop("disabled", false);
            });
        }
        else {
            var dialogsave = new DialogBox({
                caption: "Save Draft",
                content: {
                    selector: "#form-Draft-name",
                },
                button: {
                    ok: {
                        text: "Save"
                    }
                },
                type: "ok-cancel"

            });
            dialogsave.reject(function () {
                dialogsave.shutdown();
            })
            dialogsave.confirm(function () {
                item_master.Name = $("#draftname").val();
                item_master.FreightSalesView.ID = item_master.FreightSalesView.StorID;
                item_master.FreightSalesView.FreightSaleDetails.forEach(i => {
                    i.ID = i.StorDID;
                })
                item_master.DraftServiceContractDetails = ar_details.length === 0 ? new Array() : ar_details;
                item_master.DraftAttchmentFiles = $list_Attachfile.yield().length == 0 ? new Array() : $list_Attachfile.yield();
                $("#loading").prop("hidden", false);


                $.ajax({
                    url: "/Sale/SaveDraftServiceContract",
                    type: "POST",
                    dataType: "JSON",
                    data: {
                        draftAR: JSON.stringify(item_master),
                    },
                    success: function (model) {
                        if (model.Action != -1) {

                            new ViewMessage({
                                summary: {
                                    selector: "#error-summary"
                                }
                            }, model).refresh(1500);
                            $("#loading").prop("hidden", true);
                        }
                        new ViewMessage({
                            summary: {
                                selector: "#error-summary"
                            }
                        }, model);
                        $(window).scrollTop(0);
                        $("#loading").prop("hidden", true);
                    }
                });
                this.meta.shutdown();
            });
        }


    }
    $("#cancel-item").click(function () {
        const item_master = master[0];
        const saleArId = item_master.ID;
        const _this = this;
        $(_this).prop("disabled", true);
        $.get("/Sale/GetServiceContractARByInvoiceNo", { invoiceNo: item_master.InvoiceNumber, seriesID: item_master.SeriesID }, function (data) {
            $(_this).prop("disabled", false);
            if (data.AppliedAmount > 0) {
                var dialogSubmit = new DialogBox({
                    content: "The remaining applied amount is " + data.AppliedAmount
                        + ". Do you want clear and cancel this A/R Invoice?",
                    type: "ok-cancel",
                    icon: "warning"
                });
                dialogSubmit.confirm(function () {
                    cancelAr(item_master, saleArId)
                    dialogSubmit.shutdown();
                });
            } else {
                var dialogSubmit = new DialogBox({
                    content: "Are you sure you want to cancel this A/R Invoice Service Contract?",
                    type: "ok-cancel",
                    icon: "warning"
                });
                dialogSubmit.confirm(function () {
                    cancelAr(item_master, saleArId)
                    dialogSubmit.shutdown();
                });
            }
        });

    })
    $("#sale-em").click(function () {
        const dialogprojmana = new DialogBox({
            button: {
                ok: {
                    text: "Cancel",
                },
            },
            // type: "ok-cancel",
            caption: "List Sale Employee",
            content: {
                selector: "#em-content",
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown()
        });
        //  dialogprojmana.shutdown();
        dialogprojmana.invoke(function () {
            const $list_sem = ViewTable({
                keyField: "ID",//id unit not in dababase
                selector: "#list_saleem",// id of table
                indexed: true,
                paging: {
                    pageSize: 15,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "GenderDisplay", "Position", "Address", "Phone", "Email", "EMType"],

                actions: [
                    {
                        name: "",
                        template: `<i class="fas fa-arrow-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                $("#sale-emid").val(e.data.ID);
                                $("#sale-em").val(e.data.Name);
                                dialogprojmana.shutdown();
                            }
                        }
                    },
                ],
            })
            $.get("/Sale/GetSaleEmployee", function (res) {
                $list_sem.bindRows(res);
            });
        });
        dialogprojmana.reject(function () {
            dialogprojmana.shutdown();
        });
    })
    // dialog diplay draft
    function DraftDataDialog() {
        $.get("/Sale/DisplayDraftServiceContract", { cusId: master[0].CusID }, function (res) {
            res.forEach(i => {
                i.PostingDate = i.PostingDate.toString().split("T")[0];
                i.SubTotal = num.formatSpecial(i.SubTotal, disSetting.Prices);
                i.TotalAmount = num.formatSpecial(i.TotalAmount, disSetting.Prices);
            })
            let dialog = new DialogBox({
                content: {
                    selector: "#Draft-list"
                },
                button: {
                    ok: {
                        text: "Close"
                    }
                },
                caption: "Draft Detail",
            });
            dialog.invoke(function () {
                const itemMaster = ViewTable({
                    keyField: "LineID",
                    selector: $("#Draft-Data"),
                    indexed: true,
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    visibleFields: ["DocType", "DraftName", "PostingDate", "CurrencyName", "SubTotal", "BalanceDue", "Remarks"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
                            on: {
                                "click": function (e) {

                                    $.get("/Sale/FindDraftServiceContract", { draftId: e.data.DraftID, cusid: e.data.CustomerID }, function (result) {

                                        draftID = result.ServiceContract.ID;
                                        getSaleItemMasters(result, true);
                                        $("#draftname").val(result.ServiceContract.Name);


                                    });
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                if (res.length > 0) {
                    itemMaster.bindRows(res);
                    $("#txtSearchdraft").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s/g, "");
                        let items = $.grep(res, function (item) {
                            return item.DraftName.toLowerCase().replace(/\s/g, "").includes(__value) || item.SubTotal.toString().toLowerCase().replace(/\s/g, "").includes(__value)
                                || item.PostingDate.toLowerCase().replace(/\s/g, "").includes(__value) || item.BalanceDue.toString().toLowerCase().replace(/\s/g, "").includes(__value)
                                || item.CurrencyName.toLowerCase().replace(/\s/g, "").includes(__value);
                        });
                        itemMaster.bindRows(items);
                    });
                }
            });
            dialog.confirm(function () {
                dialog.shutdown();
            });
        });
    }




    function cancelAr(item_master, saleArId) {
        item_master.SeriesID = $("#invoice-no").val();
        item_master.ID = 0;
        item_master.ServiceContractDetails = ar_details;
        item_master.FreightSalesView.FreightSaleDetails = item_master.FreightSalesView.FreightSaleDetailViewModels;
        getARDownPayments(item_master.CusID, "used", item_master.SARID, function (res) {
            if (isValidArray(res)) {
                item_master.SaleARDPINCNs = res ?? new Array();
            } else {
                item_master.SaleARDPINCNs = new Array();
            }
            $.post('/Sale/CancelServiceContract', { saleAr: JSON.stringify(item_master), saleArId }, function (res) {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, res)
                if (res.IsApproved) {
                    setTimeout(function () { location.reload() }, 1500);
                }
            });
        });
    }
    function getARDownPayments(curId, status, arid, success) {
        $.get("/Sale/ARDownPaymentINCN", { curId, status, arid }, success);
    }
    function freightsEditable(freightDialog) {
        const freightView = ViewTable({
            keyField: "FreightID",
            selector: freightDialog.content.find(".freight-lists"),
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: true
            },
            visibleFields: ["Amount", "Name", "TaxGroupSelect", "TaxRate", "TotalTaxAmount", "AmountWithTax"],
            columns: [
                {
                    name: "Amount",
                    template: `<input>`,
                    on: {
                        "keyup": function (e) {
                            $(this).asNumber();
                            if (this.value === "") this.value = 0;
                            updatedetailFreight(freights, e.key, "Amount", this.value);
                            totalRowFreight(freightView, e);
                            sumSummaryFreight(freights);
                        }
                    }
                },
                {
                    name: "TaxGroupSelect",
                    template: `<select></select>`,
                    on: {
                        "change": function (e) {
                            const taxg = findArray("ID", this.value, e.data.TaxGroups);
                            //const taxgselect = findArray("Value", this.value, e.data.TaxGroupSelect);
                            //if (!!taxgselect) {
                            //    updateSelect(e.data.TaxGroupSelect, ta)
                            //}
                            updateSelect(e.data.TaxGroupSelect, this.value, "Selected");
                            if (!!taxg) {
                                updatedetailFreight(freights, e.key, "TaxRate", taxg.Rate);
                            } else {
                                updatedetailFreight(freights, e.key, "TaxRate", 0);
                            }
                            updatedetailFreight(freights, e.key, "TaxGroupID", parseInt(this.value));
                            totalRowFreight(freightView, e);
                            sumSummaryFreight(freights);
                        }
                    }
                }
            ],
        });
        freightDialog.content.find(".freightSumAmount").val(num.formatSpecial(freightMaster.AmountReven, disSetting.Prices));
        freightView.bindRows(freights);
    }
    function freightsNoneEditable(freightDialog) {
        const freightView = ViewTable({
            keyField: "FreightID",
            selector: freightDialog.content.find(".freight-lists"),
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: true
            },
            visibleFields: ["Amount", "Name", "TaxGroupSelect", "TaxRate", "TotalTaxAmount", "AmountWithTax"],
            columns: [
                {
                    name: "TaxGroupSelect",
                    template: "<select disabled></select>"
                }
            ]
        });
        freightDialog.content.find(".freightSumAmount").val(num.formatSpecial(freightMaster.AmountReven, disSetting.Prices));
        freightView.bindRows(freights);
    }
    function initModalDialog(urlMaster, urlDetail, caption, key, keyMaster, keyDetail, type) {
        $.get(urlMaster, { cusId: master[0].CusID }, function (res) {
            res.forEach(i => {
                i.PostingDate = i.PostingDate.toString().split("T")[0];
                i.SubTotal = num.formatSpecial(i.SubTotal, disSetting.Prices);
                i.TotalAmount = num.formatSpecial(i.TotalAmount, disSetting.Prices);
            })
            let dialog = new DialogBox({
                content: {
                    selector: "#container-list-item-copy"
                },
                button: {
                    ok: {
                        text: "Close"
                    }
                },
                caption: caption,
            });
            dialog.invoke(function () {
                const itemMasterCopy = ViewTable({
                    keyField: key,
                    selector: $(".item-copy"),
                    indexed: true,
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    visibleFields: ["Code", "InvoiceNumber", "PostingDate", "Currency", "SubTotal", "TypeDis", "TotalAmount", "Remarks"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                            on: {
                                "click": function (e) {
                                    $("#sale-emid").val(res[0].SaleEmID);
                                    $("#sale-em").val(res[0].SaleEmName);
                                    $.get(urlDetail, { number: e.data.InvoiceNo, seriesId: e.data.SeriesID }, function (res) {
                                        bindItemCopy(res, keyMaster, keyDetail, key, type);
                                        dialog.shutdown();
                                    });
                                }
                            }
                        }
                    ]
                });
                if (res.length > 0) {
                    itemMasterCopy.bindRows(res);
                    $("#txtSearch-itemFR-copy").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(res, function (item) {
                            return item.InvoiceNumber.toLowerCase().replace(/\s+/, "").includes(__value) || item.SubTotal.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.PostingDate.toLowerCase().replace(/\s+/, "").includes(__value) || item.TotalAmount.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.Currency.toLowerCase().replace(/\s+/, "").includes(__value) || item.TypeDis.toLowerCase().replace(/\s+/, "").includes(__value);
                        });
                        itemMasterCopy.bindRows(items);
                    });
                }
            });
            dialog.confirm(function () {
                dialog.shutdown();
            });
            //itemMasterCopy.bindRows(res)
        });
    }
    function initModalDialogitem(urlMaster, urlDetail, caption, key) {
        $.get(urlMaster, { cusId: master[0].CusID }, function (res) {
            if (res.length > 0) {
                res.forEach(i => {
                    //$("#customer").html(i.CustomerName);
                    $("#customer").append(`<option>${i.CustomerName}</option>`)
                })
            }
            res.forEach(i => {
                i.PostingDate = i.PostingDate.toString().split("T")[0];
                i.SubTotal = num.formatSpecial(i.SubTotal, disSetting.Prices);
                i.TotalAmount = num.formatSpecial(i.TotalAmount, disSetting.Prices);
            })
            let dialog = new DialogBox({
                content: {
                    selector: "#container-list-item-copy-itemmasterdata"
                },
                button: {
                    ok: {
                        text: "Close"
                    }
                },
                caption: caption,
            });
            dialog.invoke(function () {
                const itemMasterCopy = ViewTable({
                    keyField: key,
                    selector: ".item-copy-masterdata",
                    indexed: true,
                    paging: {
                        pageSize: 10,
                        enabled: true
                    },
                    visibleFields: ["Code", "KhmerName", "EnglishName", "Dateitem", "MonthlyFee"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down"></i>`,
                            on: {
                                "click": function (e) {
                                    //$("#ware-id").append(`<option selected>${e.data.Wherehouse}</option>`)
                                    ////$("#ware-id").val(e.data.PriceListID);
                                    //$("#ware-id").css("pointer-events", "none").prop("disabled", true);
                                    $("#source-copy").val("/Franchise :");
                                    $("#cur-id").append(`<option selected>${e.data.Wherehouse}</option>`)
                                    $("#cur-id").val(e.data.PriceListID);
                                    $("#cur-id").css("pointer-events", "none").prop("disabled", true);

                                    $.get(urlDetail, { Dateitem: e.data.Dateitem, plid: e.data.PriceListID, itemId: e.data.ID, frid: e.data.FranchiseID }, function (res) {
                                        ar_details.push(res[0]);
                                        $listItemChoosed.addRow(res[0]);
                                        setItemToAbled(itemArr);
                                        setSummary(ar_details);
                                        disInputUpdate(master[0]);
                                    });
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                if (res.length > 0) {
                    itemMasterCopy.bindRows(res);
                    $("#txtSearch-item-copy").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(res, function (item) {
                            return item.InvoiceNumber.toLowerCase().replace(/\s+/, "").includes(__value) || item.SubTotal.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.PostingDate.toLowerCase().replace(/\s+/, "").includes(__value) || item.TotalAmount.toString().toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.Currency.toLowerCase().replace(/\s+/, "").includes(__value) || item.TypeDis.toLowerCase().replace(/\s+/, "").includes(__value);
                        });
                        itemMasterCopy.bindRows(items);
                    });
                }
            });
            dialog.confirm(function () {
                dialog.shutdown();
            });
        });
    }
    //========================
    function bindItemCopy(_master, keyMaster, keyDetail, key, copyType = 0) {
        $.get("/Sale/GetCustomer", { id: _master[keyMaster].CusID }, function (cus) {
            $("#cus-code").val(cus.Code);
            $("#cus-name").val(cus.Name);
        });
        master[0] = _master[keyMaster];
        master[0].BaseOnID = _master[keyMaster][key];
        freights = _master[keyMaster].FreightSalesView.FreightSaleDetailViewModels;
        freightMaster = _master[keyMaster].FreightSalesView;
        freightMaster.ID = 0;
        freightMaster.IsEditabled = true;
        freights.forEach(i => {
            i.ID = 0;
            i.FreightSaleID = 0;
            i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
            i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
            i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
            i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
        });
        ar_details = _master[keyDetail];
        $("#ware-id").val(_master[keyMaster].WarehouseID);
        $("#ref-id").val(_master[keyMaster].RefNo);
        $("#cur-id").val(_master[keyMaster].SaleCurrencyID);
        $("#sta-id").val(_master[keyMaster].Status);
        $("#sub-id").val(num.formatSpecial(_master[keyMaster].SubTotal, disSetting.Prices));
        $("#sub-dis-id").val(_master[keyMaster].TypeDis);
        $("#sub-after-dis").val(num.formatSpecial(_master[keyMaster].SubTotalAfterDis, disSetting.Prices));
        $("#freight-value").val(num.formatSpecial(_master[keyMaster].FreightAmount, disSetting.Prices));
        $("#seriesID").val(_master[keyMaster].SeriesID);
        $("#branch").val(_master[keyMaster].BranchID); 
        //$("#sub-dis-id").val(_master.SaleQuote.TypeDis);
        $("#dis-rate-id").val(num.formatSpecial(_master[keyMaster].DisRate, disSetting.Rates));
        $("#dis-value-id").val(num.formatSpecial(_master[keyMaster].DisValue, disSetting.Prices));
        $("#remark-id").val(_master[keyMaster].Remarks);
        $("#total-id").val(num.formatSpecial(_master[keyMaster].TotalAmount, disSetting.Prices));
        $("#vat-value").val(num.formatSpecial(_master[keyMaster].VatValue, disSetting.Prices));
        setDate("#post-date", _master[keyMaster].PostingDate.toString().split("T")[0]);
        setDate("#due-date", _master[keyMaster].DeliveryDate.toString().split("T")[0]);
        setDate("#document-date", _master[keyMaster].DocumentDate.toString().split("T")[0]);
        $("#show-list-cus").addClass("noneEvent");
        var ex = findArray("CurID", _master[keyMaster].SaleCurrencyID, ExChange[0]);
        if (!!ex) {
            $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
            $(".cur-class").text(ex.CurName);
        }
        master[0].CopyType = copyType; // 1: Quotation, 2: Order, 3: Delivery, 4: AR  
        master[0].CopyKey = _master[keyMaster].InvoiceNo;
        master[0].BasedCopyKeys = "/" + _master[keyMaster].InvoiceNo;
        setSourceCopy(master[0].BasedCopyKeys)
        if (isValidArray(_master[keyDetail])) {
            $listItemChoosed.clearHeaderDynamic(_master[keyDetail][0].AddictionProps);
            $listItemChoosed.createHeaderDynamic(_master[keyDetail][0].AddictionProps);
            $listItemChoosed.bindRows(_master[keyDetail]);
        }
        if (copyType == 3) {
            setDisabled();
            setItemToDisabled(itemArr);
            isCopied = !isCopied;
        } else {
            isCopied = false;
            setItemToAbled(itemArr);
        }
        $("#tdp-dailog").removeClass("disabled");
        getARDownPayments(_master[keyMaster].CusID, "close", 0, function (res) {
            if (isValidArray(res)) {
                if (isValidArray(ARDownPayments)) {
                    ARDownPayments = [];
                    res.forEach(i => {
                        i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                        ARDownPayments.push(i);
                    });
                }
                else {
                    res.forEach(i => {
                        i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                        ARDownPayments.push(i);
                    });
                }
            }
        });
    }
    function setSourceCopy(basedCopyKeys) {
        let copyKeys = new Array();
        if (basedCopyKeys) {
            copyKeys = basedCopyKeys.split("/");
        }
        var copyInfo = "";
        $.each(copyKeys, function (i, key) {
            if (key.startsWith("SQ")) {
                copyInfo += "Sale Quotation: " + key;
            }

            if (key.startsWith("SO")) {
                copyInfo += "/Sale Order: " + key;
            }

            if (key.startsWith("DN")) {
                copyInfo += "/Sale Delivery: " + key;
            }
        });
        $("#source-copy").val(copyInfo);
    }
    function getSaleItemMasters(_master, disable = false) {
        if (!!_master) {
            $.get("/Sale/GetingCustomer", { id: _master.ServiceContract.CusID }, function (cus) {

                $("#cus-name").val(cus.Name);
                $("#cus-code").val(cus.Code);
                $("#cus-code_bp").val(cus.Code);
                $("#contactperson").find('option').remove();
                cus.ContactPeople.forEach(i => {
                    if (i.SetAsDefualt) {
                        $("#contactperson").append(`<option value='${i.ContactID}'  selected = "selected">${i.ContactID}</option>`)
                    }
                    else {
                        $("#contactperson").append(`<option value='${i.ContactID}'>${i.ContactID}</option>`)
                    }
                })
            });
            master[0] = _master.ServiceContract;
            freights = _master.ServiceContract.FreightSalesView.FreightSaleDetailViewModels;
            freightMaster = _master.ServiceContract.FreightSalesView;
            if (disable == false)
                freightMaster.IsEditabled = false;
            else
                freightMaster.IsEditabled = true;
            freightMaster.ID = 0;
            freights.forEach(i => {
                i.Amount = num.formatSpecial(i.Amount, disSetting.Prices);
                i.AmountWithTax = num.formatSpecial(i.AmountWithTax, disSetting.Prices);
                i.TotalTaxAmount = num.formatSpecial(i.TotalTaxAmount, disSetting.Prices);
                i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
                updatedetailFreight(freights, i.FreightID, "ID", 0);
            });

            $listItemChoosed.bindRows(_master.ServiceContractDetail);
            ar_details = _master.ServiceContractDetail;

            $list_Attachfile.bindRows(_master.ServiceContract.AttchmentFiles);
            attchmentfiles = _master.ServiceContract.AttchmentFiles;

            $("#next_number").val(_master.ServiceContract.InvoiceNumber);
            $("#sale-emid").val(_master.ServiceContract.SaleEmID);
            setDate("#contractstartdate", _master.ServiceContract.ContractStartDate.toString().split("T")[0]);
            setDate("#contractendtdate", _master.ServiceContract.ContractENDate.toString().split("T")[0]);
            setDate("#contractrenewal", _master.ServiceContract.ContractRenewalDate.toString().split("T")[0]);
            $("#contracttemplateid").val(_master.ServiceContract.ContractTemplateID);
            $("#contracttype").val(_master.ServiceContract.ContractType);
            $("#contracttemplate").val(_master.ServiceContract.ContractName);
            $("#remark").val(_master.ServiceContract.Remark);
            $("#contractno").val(_master.ServiceContract.AdditionalContractNo);
            $("#remark").val(_master.ServiceContract.Remark);
            $("#sale-em").val(_master.ServiceContract.SaleEmName);
            $("#ware-id").val(_master.ServiceContract.WarehouseID);
            $("#branch").val(_master.ServiceContract.BranchID); 
            $("#ware-idhide").val(_master.ServiceContract.WareHouseName)
            $("#ref-id").val(_master.ServiceContract.RefNo);
            $("#cur-id").val(_master.ServiceContract.SaleCurrencyID);
            $("#cur_pri").val(_master.ServiceContract.PriListName);
            $("#sta-id").val(_master.ServiceContract.Status);
            $("#sub-id").val(num.formatSpecial(_master.ServiceContract.SubTotal, disSetting.Prices));
            $("#sub-after-dis").val(num.formatSpecial(_master.ServiceContract.SubTotalAfterDis, disSetting.Prices));
            $("#freight-value").val(num.formatSpecial(_master.ServiceContract.FreightAmount, disSetting.Prices));
            $("#total-dp-value").val(num.formatSpecial(_master.ServiceContract.DownPayment, disSetting.Prices));
            //$("#sub-dis-id").val(_master.SaleQuote.TypeDis);
            $("#dis-rate-id").val(num.formatSpecial(_master.ServiceContract.DisRate, disSetting.Rates));
            $("#dis-value-id").val(num.formatSpecial(_master.ServiceContract.DisValue, disSetting.Prices));
            setDate("#post-date", _master.ServiceContract.PostingDate.toString().split("T")[0]);
            setDate("#due-date", _master.ServiceContract.DeliveryDate.toString().split("T")[0]);
            setDate("#document-date", _master.ServiceContract.DocumentDate.toString().split("T")[0]);
            $("#remark-id").val(_master.ServiceContract.Remarks);
            $("#total-id").val(num.formatSpecial(_master.ServiceContract.TotalAmount, disSetting.Prices));
            $("#vat-value").val(num.formatSpecial(_master.ServiceContract.VatValue, disSetting.Prices));
            $("#item-id").prop("disabled", false);
            $("#applied-amount").val(num.formatSpecial(_master.ServiceContract.AppliedAmount, disSetting.Prices))
            var ex = findArray("CurID", _master.ServiceContract.SaleCurrencyID, ExChange[0]);
            if (!!ex) {
                $("#ex-id").val(1 + " " + _currency + " = " + ex.SetRate + " " + ex.CurName);
                $(".cur-class").text(ex.CurName);
            }
            if (isValidArray(_master.ServiceContractDetails)) {
                $listItemChoosed.clearHeaderDynamic(_master.ServiceContractDetail[0].AddictionProps);
                $listItemChoosed.createHeaderDynamic(_master.ServiceContractDetail[0].AddictionProps);
                $listItemChoosed.bindRows(_master.ServiceContractDetail);
            }
            if (disable == false)
                setItemToDisabled([...itemArr, ".qty"]);
            if (master[0].Status != 'cancel') $("#cancel-item").prop("hidden", false);
            //setRequested(_master, key, detailKey, copyType);
        } else {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Service Contract  Not found!",
                close_button: "none"
            });
        }
    }
    function setWarehouse(id) {
        $.ajax({
            url: "/Sale/GetWarehouse",
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
    function setDisabled() {
        $("#item-id").prop("disabled", true);
        $("#ipost-date").prop("disabled", false);
        $("#contractendtdate").prop("disabled", false);
        $("#sub-dis-id").prop("disabled", true);
        $("#dis-rate-id").prop("disabled", true);
        $("#dis-value-id").prop("disabled", true);
        $("#sub-dis-id").prop("disabled", true);
        $("#sub-id").prop("disabled", true);
        $("#sub-after-dis").prop("disabled", true);
        $("#remark-id").prop("disabled", true);
        $("#include-vat-id").prop("disabled", true);
        $("#applied-amount").prop("disabled", true);
        $("#cur-id").prop("disabled", true);
        $("#ref-id").prop("disabled", true);
        $("#ware-id").prop("disabled", true);
        $("#branch").prop("disabled", true);
        $("#show-list-cus").addClass("csr-default").off();
        $("#fee-note").prop("disabled", true);
        $("#fee-amount").prop("disabled", true);
        $.each($("[data-date]"), function (i, t) {
            $(t).prop("disabled", true);
        });
    }
    function selectSeries(selected) {
        $.each(___IN.seriesIN, function (i, item) {
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
    function totalRow(table, e, _this) {
        if (_this === '' || _this === '-') _this = 0;
        const totalWDis = (num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice)) - num.toNumberSpecial(e.data.DisValue);
        let vatValue = num.toNumberSpecial(e.data.TaxRate) * totalWDis === 0 ? 0 : num.toNumberSpecial(e.data.TaxRate) * totalWDis / 100;

        let totalrow = num.toNumberSpecial(e.data.Qty) * num.toNumberSpecial(e.data.UnitPrice) - num.toNumberSpecial(e.data.DisValue) + vatValue;

        // Update Object
        updatedetail(ar_details, e.data.UomID, e.key, "TotalWTax", isNaN(totalrow) ? 0 : totalrow);
        updatedetail(ar_details, e.data.UomID, e.key, "Total", isNaN(totalrow - vatValue) ? 0 : totalrow - vatValue);
        updatedetail(ar_details, e.data.UomID, e.key, "TaxValue", vatValue);

        let subtotal = 0;
        let disRate = num.toNumberSpecial($("#dis-rate-id").val());
        ar_details.forEach(i => {
            subtotal += num.toNumberSpecial(i.Total);// - num.toNumberSpecial(i.DisValue);
        });
        const disValue = (disRate * subtotal) === 0 ? 0 : (disRate / 100) * subtotal;
        // const taxFinDisValue = disRate === 0 ? vatValue : disRate * (totalWDis - disValue) === 0 ? 0 :  disRate * (totalWDis - disValue) / 100;
        const taxFinDisValue = num.toNumberSpecial(e.data.TaxRate) == 0 ? 0 : disRate === 0 ? vatValue : (num.toNumberSpecial(e.data.TaxRate) / 100) * (totalWDis - disValue);
        const totalwtax = totalWDis + taxFinDisValue

        const finalTotal = num.toNumberSpecial(totalWDis) - disValue;
        updatedetail(ar_details, e.data.UomID, e.key, "TotalWTax", totalwtax);
        updatedetail(ar_details, e.data.UomID, e.key, "FinTotalValue", finalTotal);
        updatedetail(ar_details, e.data.UomID, e.key, "TaxOfFinDisValue", taxFinDisValue);

        //Update View
        table.updateColumn(e.key, "TaxOfFinDisValue", num.formatSpecial(isNaN(taxFinDisValue) ? 0 : taxFinDisValue, disSetting.Prices));
        table.updateColumn(e.key, "Total", num.formatSpecial(isNaN(totalrow - vatValue) ? 0 : totalrow - vatValue, disSetting.Prices));
        table.updateColumn(e.key, "TotalWTax", num.formatSpecial(isNaN(totalrow) ? 0 : totalwtax, disSetting.Prices));
        table.updateColumn(e.key, "TaxValue", num.formatSpecial(isNaN(vatValue) ? 0 : vatValue, disSetting.Prices));
        table.updateColumn(e.key, "TaxRate", num.formatSpecial(isNaN(e.data.TaxRate) ? 0 : e.data.TaxRate, disSetting.Rate));
        table.updateColumn(e.key, "FinTotalValue", num.formatSpecial(finalTotal, disSetting.Prices));
    }
    function calDisRate(e, _this) {
        if (_this.value > 100) _this.value = 100;
        if (_this.value < 0) _this.value = 0;
        updatedetail(ar_details, e.data.UomID, e.key, "DisRate", _this.value);
        const disvalue = parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0 ? 0 :
            parseFloat(isNaN(_this.value) ? 0 : _this.value) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) / 100;

        updatedetail(ar_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisValue(e, _this) {
        if (_this.value > e.data.Qty * e.data.UnitPrice) _this.value = e.data.Qty * e.data.UnitPrice;
        if (_this.value == '' || _this.value < 0) _this.value = 0;
        const value = parseFloat(_this.value);
        updatedetail(ar_details, e.data.UomID, e.key, "DisValue", value);
        const ratedis = (value * 100 === 0) || (parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0) ? 0 :
            (value * 100) / (parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice));
        updatedetail(ar_details, e.data.UomID, e.key, "DisRate", ratedis);
        $listItemChoosed.updateColumn(e.key, "DisRate", num.formatSpecial(isNaN(ratedis) ? 0 : ratedis, disSetting.Rates));
        totalRow($listItemChoosed, e, _this);
    }
    function calDisvalue(e) {
        let disvalue = num.toNumberSpecial(e.data.DisRate) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice) === 0 ? 0 :
            (num.toNumberSpecial(e.data.DisRate) * parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice)) / 100;
        if (disvalue > parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice)) disvalue = parseFloat(e.data.Qty) * parseFloat(e.data.UnitPrice);
        updatedetail(ar_details, e.data.UomID, e.key, "DisValue", disvalue);
        $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(isNaN(disvalue) ? 0 : disvalue, disSetting.Prices));
    }
    function setSummary(data) {
        let subtotal = 0;
        let vat = 0;
        let disRate = $("#dis-rate-id").val();
        data.forEach(i => {
            subtotal += num.toNumberSpecial(i.Total);
            vat += num.toNumberSpecial(i.TaxOfFinDisValue);
        });
        const disValue = (num.toNumberSpecial(disRate) * subtotal) === 0 ? 0 : (num.toNumberSpecial(disRate) * subtotal) / 100;
        const _master = master[0];
        _master.SubTotalSys = subtotal * _master.ExchangeRate;
        _master.SubTotal = subtotal;
        _master.VatRate = (vat * 100) === 0 ? 0 : vat * 100 / subtotal;
        _master.VatValue = vat + num.toNumberSpecial(freightMaster.TaxSumValue) - num.toNumberSpecial(__ardpSum.taxSum);
        _master.DisValue = disValue;
        _master.SubTotalAfterDis = subtotal - disValue;
        _master.SubTotalBefDis = subtotal;
        _master.DisRate = disRate;
        _master.TotalAmount = num.toNumberSpecial(_master.SubTotalAfterDis) + _master.VatValue - num.toNumberSpecial(__ardpSum.sum) + num.toNumberSpecial(freightMaster.AmountReven);
        _master.TotalAmountSys = _master.TotalAmount * _master.ExchangeRate;
        // ar_details.forEach(i => {
        //     $listItemChoosed.updateColumn(i.LineID, "FinDisValue", _master.DisValue);
        //     updatedetail(ar_details, i.UomID, i.LineID, "FinDisValue", _master.DisValue);
        //     const lastDisval = num.toNumberSpecial(i.Total) - (num.toNumberSpecial(_master.DisRate) * num.toNumberSpecial(i.Total) / 100);
        //     const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
        //         num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
        //     updatedetail(ar_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
        //     updatedetail(ar_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
        //     $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
        //     $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
        // });
        $("#vat-value").val(num.formatSpecial(_master.VatValue, disSetting.Prices));
        $("#sub-id").val(num.formatSpecial(subtotal, disSetting.Prices));
        $("#total-id").val(num.formatSpecial(_master.TotalAmount, disSetting.Prices));
        $("#sub-after-dis").val(num.formatSpecial(_master.SubTotalAfterDis, disSetting.Prices));
    }
    function disInputUpdate(_master) {
        $("#dis-rate-id").val(_master.DisRate);
        $("#dis-value-id").val(_master.DisValue);
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
    function setItemToAbled(item) {
        if (isValidArray(item)) {
            item.forEach(i => {
                $(i).prop("disabled", false);
            });
        }
    }
    function setItemToDisabled(item) {
        if (isValidArray(item)) {
            item.forEach(i => {
                $(i).prop("disabled", true);
            });
        }
    }
    function updatedetailFreight(data, key, prop, value) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i.FreightID === key) {
                    i[prop] = value;
                }
            });
        }
    }
    function invoiceDisAllRowRate(_this) {
        if (isValidArray(ar_details)) {
            ar_details.forEach(i => {
                const value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(_this.value) / 100;
                if (_this.value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updatedetail(ar_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(ar_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(value);
                    const taxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;

                    updatedetail(ar_details, i.UomID, i.LineID, "TaxOfFinDisValue", taxOfFinDisValue);
                    updatedetail(ar_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(taxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                } else if (_this.value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(_this.value, disSetting.Rates));
                    updatedetail(ar_details, i.UomID, i.LineID, "FinDisValue", value);
                    updatedetail(ar_details, i.UomID, i.LineID, "FinDisRate", _this.value);

                    updatedetail(ar_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(ar_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(ar_details);
        }
    }
    function invoiceDisAllRowValue(value) {
        if (isValidArray(ar_details)) {
            ar_details.forEach(i => {
                const _value = num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) === 0 ? 0 :
                    num.toNumberSpecial(i.Total) * num.toNumberSpecial(value) / 100;
                if (value < 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(_value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updatedetail(ar_details, i.UomID, i.LineID, "FinDisValue", _value);
                    updatedetail(ar_details, i.UomID, i.LineID, "FinDisRate", value);

                    const lastDisval = num.toNumberSpecial(i.Total) - num.toNumberSpecial(_value);
                    const TaxOfFinDisValue = num.toNumberSpecial(i.TaxRate) * lastDisval === 0 ? 0 :
                        num.toNumberSpecial(i.TaxRate) * lastDisval / 100;
                    updatedetail(ar_details, i.UomID, i.LineID, "TaxOfFinDisValue", TaxOfFinDisValue);
                    updatedetail(ar_details, i.UomID, i.LineID, "FinTotalValue", lastDisval);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(TaxOfFinDisValue, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(lastDisval, disSetting.Rates));
                }
                else if (value >= 100) {
                    $listItemChoosed.updateColumn(i.LineID, "FinDisValue", num.formatSpecial(value, disSetting.Prices));
                    $listItemChoosed.updateColumn(i.LineID, "FinDisRate", num.formatSpecial(value, disSetting.Rates));
                    updatedetail(ar_details, i.UomID, i.LineID, "FinDisValue", _value);
                    updatedetail(ar_details, i.UomID, i.LineID, "FinDisRate", value);

                    updatedetail(ar_details, i.UomID, i.LineID, "TaxOfFinDisValue", 0);
                    updatedetail(ar_details, i.UomID, i.LineID, "FinTotalValue", 0);
                    $listItemChoosed.updateColumn(i.LineID, "TaxOfFinDisValue", num.formatSpecial(0, disSetting.Rates));
                    $listItemChoosed.updateColumn(i.LineID, "FinTotalValue", num.formatSpecial(0, disSetting.Rates));
                }
            });
            setSummary(ar_details);
        }
    }
    function totalRowFreight(table, e,) {
        table.updateColumn(e.key, "TaxRate", num.formatSpecial(e.data.TaxRate, disSetting.Rates));
        const taxValue = num.toNumberSpecial(e.data.TaxRate) * num.toNumberSpecial(e.data.Amount) === 0 ? 0 :
            num.toNumberSpecial(e.data.TaxRate) * num.toNumberSpecial(e.data.Amount) / 100;
        const amountWTax = num.toNumberSpecial(e.data.Amount) + taxValue;
        updatedetailFreight(freights, e.key, "TotalTaxAmount", taxValue);
        updatedetailFreight(freights, e.key, "AmountWithTax", amountWTax);

        table.updateColumn(e.key, "TotalTaxAmount", num.formatSpecial(e.data.TotalTaxAmount, disSetting.Prices));
        table.updateColumn(e.key, "AmountWithTax", num.formatSpecial(e.data.AmountWithTax, disSetting.Prices));
    }
    function sumSummaryFreight(data) {
        if (isValidArray(data)) {
            let sumFreight = 0;
            let sumTax = 0;
            data.forEach(i => {
                sumFreight += num.toNumberSpecial(i.Amount);
                sumTax += num.toNumberSpecial(i.TotalTaxAmount);
            });
            freightMaster.AmountReven = sumFreight;
            freightMaster.TaxSumValue = sumTax;
            $(".freightSumAmount").val(num.formatSpecial(sumFreight, disSetting.Prices));
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
    function updateARDP(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue) {
                    i[prop] = propValue;
                }
            });
        }
    }
    function sumARDP(data) {
        let sum = 0,
            taxSum = 0;
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i.Selected) {
                    sum += num.toNumberSpecial(i.Amount);
                    if (isValidArray(i.SaleARDPINCNDetails)) {
                        i.SaleARDPINCNDetails.forEach(j => {
                            taxSum += num.toNumberSpecial(j.TaxDownPaymentValue);
                        });
                    }
                }
            });
        }
        __ardpSum.sum = sum;
        __ardpSum.taxSum = taxSum;
        setSummary(ar_details);
    }
    function ARDPDdialog(data) {
        const ARDownPaymentsdDialog = new DialogBox({
            content: {
                selector: "#container-list-tdpd"
            },
            caption: "A/R Down Payment Details",
        });
        ARDownPaymentsdDialog.invoke(function () {
            const ARDownPaymentsdView = ViewTable({
                keyField: "ID",
                selector: ARDownPaymentsdDialog.content.find(".tdpd-lists"),
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: [
                    "Barcode", "ItemCode", "Remarks", "ItemName", "Quantity", "UnitPrice", "UoMName", "TaxCode",
                    "TaxRate", "TaxValue", "TaxOfFinDisValue", "TaxDownPaymentValue", "DisRate", "DisValue",
                    "FinDisRate", "FinDisValue", "Total", "TotalWTax", "FinTotalValue", "Currency"
                ],
            });
            if (isValidArray(data)) {
                data.forEach(i => {
                    i.Quantity = num.formatSpecial(i.Quantity, disSetting.Amounts);
                    i.UnitPrice = num.formatSpecial(i.UnitPrice, disSetting.Prices);
                    i.TaxRate = num.formatSpecial(i.TaxRate, disSetting.Rates);
                    i.TaxValue = num.formatSpecial(i.TaxValue, disSetting.Prices);
                    i.TaxOfFinDisValue = num.formatSpecial(i.TaxOfFinDisValue, disSetting.Prices);
                    i.TaxDownPaymentValue = num.formatSpecial(i.TaxDownPaymentValue, disSetting.Prices);
                    i.DisRate = num.formatSpecial(i.DisRate, disSetting.Rates);
                    i.DisValue = num.formatSpecial(i.DisValue, disSetting.Prices);
                    i.FinDisRate = num.formatSpecial(i.FinDisRate, disSetting.Rates);
                    i.FinDisValue = num.formatSpecial(i.FinDisValue, disSetting.Prices);
                    i.Total = num.formatSpecial(i.Total, disSetting.Prices);
                    i.TotalWTax = num.formatSpecial(i.TotalWTax, disSetting.Prices);
                    i.FinTotalValue = num.formatSpecial(i.FinTotalValue, disSetting.Prices);
                });
                ARDownPaymentsdView.bindRows(data);
            }
        });
        ARDownPaymentsdDialog.confirm(function () {
            ARDownPaymentsdDialog.shutdown();
        })
    }
});