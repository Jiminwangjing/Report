
let masterAll = JSON.parse($("#vmc-all").text());
let masterBP = masterAll.BusinessPartner;
let vmcDetails = masterAll.AutoMobiles;
let vmclist;
$(document).ready(function () {
    $.each($("[data-year]"), function (i, item) {
        $(item).yearpicker();
    });
    $.ajax({
        url: "/Currency/GetCurrency",
        type: "GET",
        dataType: "JSON",
        success: function (respones) {
            let data = "";
            $.each(respones, function (i, item) {
                data +=
                    '<option value="' + item.ID + '">' + item.Symbol + '</option>';
            });
            $("#txtcurrency").append(data);
        }
    });
    $("#accounts-receivable input").val(masterAll.GLAccCode);
    $("#name").text(masterAll.GLAccName);
    if (masterBP.Type == "Vendor") {
        $("#vehicle").hide();
    }
    const __tablesalehistory = ViewTable({
        keyField: "LineID",
        selector: "#list-sale",
        paging: {
            enabled: true,
            pageSize: 20
        },
        indexed: true,
        visibleFields: [
            "ReceiptNmber",
            "DouType",
            "DateOut",
            "ItemName",
            "Uom",
            "Price",
            "Qty",
            "Total",
        ],

    });
   
    $.get("/BusinessPartner/GetHistoryPurchase", { id: parseInt($("#busid").val()) }, function (res) {
        if (res.length > 0)
        {
            const i = res[0];
            $("#lcc").val(i.GrandTotalLCC);
            $("#scc").val(i.GrandTotalSys);
            $("#vat").val(i.VatCal);
            $("#distotal").val(i.DiscountTotal);
            $("#disitem").val(i.DiscountItem);
            __tablesalehistory.clearRows();
            __tablesalehistory.bindRows(res);
            $("#search-sale").on("keyup", function () {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let items = $.grep(res, function (item) {
                    return item.ReceiptNmber.toLowerCase().replace(/\s+/, "").includes(__value) || item.DouType.toLowerCase().replace(/\s+/, "").includes(__value)
                        || item.DateOut.toLowerCase().replace(/\s+/, "").includes(__value);
                });
                __tablesalehistory.bindRows(items)
            });
        }
    });
    $("#filter-reportsale").click(function () {
       
        let datef = $("#datefrom").val();
        let datet = $("#dateto").val();
        if (datef != null || datef != "" && datet != null || datet != "") {
            $.get("/BusinessPartner/GetHistoryPurchase", { id: $("#busid").val(), datefrom: datef, dateto: datet }, function (data) {
                
                __tablesalehistory.clearRows();
                __tablesalehistory.bindRows(data);
                $("#search-sale").on("keyup", function () {
                    let __value = this.value.toLowerCase().replace(/\s+/, "");
                    let items = $.grep(data, function (item) {
                        return item.ReceiptNmber.toLowerCase().replace(/\s+/, "").includes(__value) || item.DouType.toLowerCase().replace(/\s+/, "").includes(__value)
                            || item.DateOut.toLowerCase().replace(/\s+/, "").includes(__value);
                    });
                    __tablesalehistory.bindRows(items)
                });
                
            });
        }

    });
    $("#accounts-receivable").on("dblclick", function () {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    }
                }
            },
            content: {
                selector: "#controlAc-gl-content"
            }
        });

        dialog.invoke(function () {
            $.get("/bussinesspartner/glcontrolaccount", function (resp) {
                let $listControlGL = ViewTable({
                    keyField: "ID",
                    indexed: true,
                    selector: dialog.content.find("#list-controlAc-gl"),
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    visibleFields: ["Code", "Name"],
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down'></i>",
                            on: {
                                "click": function (e) {
                                    $("#accounts-receivable input").val(e.data.Code);
                                    $("#name").text(e.data.Name);
                                    $("#glAccId").val(e.data.ID);
                                    //masterBP.GLAccID = e.data.ID;
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                $listControlGL.clearRows();
                $listControlGL.bindRows(resp);
            });
        });
    })
    // Create New Price List
    $(".add-price-list").click(function () {
        let dialog = new DialogBox({
            type: 'yes-no',
            button: {
                yes: {
                    text: "Add",
                },
                no: {
                    text: "Close"
                }
            },
            caption: 'Create New Price List',
            content: {
                selector: "#modalPricelist"
            }
        });
        dialog.invoke(function () {
            dialog.confirm(function () {
                clickinsertpricelist();
            })
            dialog.reject(function () {
                dialog.shutdown()
            })
        })
    })
    function clickinsertpricelist() {

        let name = $("#txtpricelist").val();
        let cur = $("#txtcurrency").val();

        let count = 0;
        if (name == 0) {
            count++;
            $("#txtpricelist").css("border-color", "red");

        } else {
            $("#txtpricelist").css("border-color", "lightgrey");
            count = 0;
        }
        if (cur == 0) {
            count++;
            $("#txtcurrency").css("border-color", "red");
        }
        else {
            $("#txtcurrency").css("border-color", "lightgrey");
        }
        if (count > 0) {
            count = 0;
            return;
        }
        let data = {
            Name: name,
            CurrencyID: cur
        }
        $.ajax({
            url: "/PriceList/InsertPricelist",
            type: "POST",
            dataType: "JSON",
            data: { priceList: data },
            complete: function () {
                $("#txtpricelist").val("");
                $("#txtcurrency").val("");
                $.ajax({
                    url: "/PriceList/GetselectPricelist",
                    type: "Get",
                    dataType: "JSON",
                    success: function (respones) {
                        let sata = "";
                        $.each(respones, function (i, item) {
                            sata +=
                                '<option value="' + item.ID + '">' + item.Name + '</option>';
                        });
                        $("#txtselectprice").html(sata);
                    }
                })
            }

        });
    }
    function error_bus(message) {
        $("#error_val_bus").text(message);
        $("#sh_bus").show();
        setTimeout(function () {
            $("#sh_bus").hide();
        }, 3000);

    }


    let bp = JSON.parse($("#vmc-all").text());

    var contactPersonTable = ViewTable({
        selector: ".item-detail",
        keyField: "LineID",
        indexed: true,
        visibleFields: [
            "ContactID",
            "FirstName",
            "MidelName",
            "LastName",
            "Title",
            "Position",
            "Address",
            "Tel1",
            "Tel2",
            "MobilePhone",
            "Fax",
            "Email",
            "Pager",
            "Remark1",
            "Remark2",
            "Parssword",
            "CountriesOfBirth",
            "DateOfBirth",
            "Genders",
            "Profession",
            "SetAsDefualt"

        ],
        dataSynced: true,
        columns: [
            {
                name: "ContactID",
                template: "<input id='bolde'>"
            },
            {
                name: "FirstName",
                template: "<input>"
            },
            {
                name: "MidelName",
                template: "<input>"
            },
            {
                name: "LastName",
                template: "<input>"
            },
            {
                name: "Title",
                template: "<input>"
            },
            {
                name: "Position",
                template: "<input>"
            },
            {
                name: "Address",
                template: "<input>"
            },
            {
                name: "Tel1",
                template: "<input>"
            },
            {
                name: "Tel2",
                template: "<input>"
            },
            {
                name: "MobilePhone",
                template: "<input>"
            },
            {
                name: "Fax",
                template: "<input>"
            },
            {
                name: "Email",
                template: "<input>"
            },
            {
                name: "Pager",
                template: "<input>"
            },
            {
                name: "Remark1",
                template: "<input>"
            },
            {
                name: "Remark2",
                template: "<input>"
            },
            {
                name: "Parssword",
                template: "<input>"
            },
            {
                name: "CountriesOfBirth",
                nameField: "CountryOfBirth",
                template: "<select></select>"
            },
            {
                name: "DateOfBirth",
                template: "<input type='date'>"
            },
            {
                name: "Genders",
                nameField: "Gender",
                template: "<select></select>"
            },
            {
                name: "Profession",
                template: "<input>"
            },
            {
                name: "SetAsDefualt",
                nameField: "SetAsDefualt",
                template: "<input type='checkbox'  class='input-box-kernel'>",
                on: {
                    "click": function (e) {
                        const active = $(this).prop("checked") ? true : false;
                        //updatedata(__data, "LineID", e.key, "SetAsDefualt", active);
                        updateSetDefault(contactPersonTable, contactPersonTable.yield(), e.key, active);


                    }
                }
            },
        ],
        model: {
            BusinessPartner: {
                ContactPeople: []
            }
        }
    });

    contactPersonTable.bindRows(bp.BusinessPartner.ContactPeople);
    function updateSetDefault(table, data, id, value) {
        if (data.length > 0) {
            if (value) {
                for (var i of data) {
                    if (i.LineID == id) {
                        table.updateColumn(i.LineID, "SetAsDefualt", true);
                        table.updateColumn(i.LineID, "SetDefualt", true);
                    } else {
                        table.updateColumn(i.LineID, "SetAsDefualt", false);
                        table.updateColumn(i.LineID, "SetDefualt", false);
                    }
                }
            } else {
                table.updateColumn(id, "SetAsDefualt", false);
                table.updateColumn(id, "SetDefualt", false);
            }
        }
    }
});

