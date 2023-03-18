
let masterbp = JSON.parse($("#master_bus").text());
let detailbps = JSON.parse($("#detail_auto").text());
let vmclist;

$(document).ready(function () {
    $.each($("[data-year]"), function (i, y) {
        $(y).yearpicker();
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

    //==================contact person================

    let contactPerson = ViewTable({
        keyField: "LineID",
        selector: ".item-detail",
        indexed: true,
        paging: {
            enabled: true,
            pageSize: 20
        },
        visibleFields: [
            "ContactID", "FirstName", "MidelName", "LastName", "Title", "Position", "Address", "Tel1",
            "Tel2", "MobilePhone", "Fax", "Email", "Pager", "Remark1", "Remark2", "Parssword", "CountriesOfBirth",
            "DateOfBirth", "Genders", "Profession", "SetAsDefualt"
        ],
        columns: [

            {
                name: "ContactID",
                template: "<input >",
                on: {
                    "keyup": function (e) {

                        updatedata(dataContactPerson, "LineID", e.key, "ContactID", this.value);
                    }
                }
            },
            {
                name: "FirstName",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "FirstName", this.value);
                    }
                }
            },
            {
                name: "MidelName",
                template: "<input >",

                on: {
                    "keyup": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "MidelName", this.value);
                    }
                }
            },
            {
                name: "LastName",
                template: "<input >",

                on: {
                    "keyup": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "LastName", this.value);
                    }
                }
            },
            {
                name: "Title",
                template: "<input >",

                on: {
                    "keyup": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "Title", this.value);
                    }
                }
            },
            {
                name: "Position",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        //$(this).asNumber();
                        updatedata(dataContactPerson, "LineID", e.key, "Position", this.value);
                    }
                }
            },
            {
                name: "Address",
                template: "<input   >",
                on: {
                    "keyup": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "Address", this.value);

                    }
                }
            },
            {
                name: "Tel1",
                template: "<input >",
                on: {
                    "keyup": function (e) {

                        updatedata(dataContactPerson, "LineID", e.key, "Tel1", this.value);
                    }
                }
            },
            {
                name: "Tel2",
                template: "<input >",

                on: {
                    "keyup": function (e) {

                        updatedata(dataContactPerson, "LineID", e.key, "Tel2", this.value);

                    }
                }
            },
            {
                name: "MobilePhone",
                template: "<input >",

                on: {
                    "keyup": function (e) {

                        updatedata(dataContactPerson, "LineID", e.key, "MobilePhone", this.value);

                    }
                }
            },
            {
                name: "Fax",
                template: "<input >",

                on: {
                    "keyup": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "Fax", this.value);

                    }
                }
            },
            {
                name: "Email",
                template: "<input >",

                on: {
                    "keyup": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "Email", this.value);

                    }
                }
            },
            {
                name: "Pager",
                template: "<input >",

                on: {
                    "keyup": function (e) {

                        updatedata(dataContactPerson, "LineID", e.key, "Pager", this.value);

                    }
                }
            },
            {
                name: "Remark1",
                template: "<input >",

                on: {
                    "keyup": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "Remark1", this.value);

                    }
                }
            },
            {
                name: "Remark2",
                template: "<input >",

                on: {
                    "keyup": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "Remark2", this.value);

                    }
                }
            },
            {
                name: "Parssword",
                template: "<input >",

                on: {
                    "keyup": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "Parssword", this.value);

                    }
                }
            },
            {
                name: "CountriesOfBirth",
                template: "<select><option>---select---</option></select>",
                on: {
                    "change": function (e) {
                        updatedata(dataContactPerson, "LineID", e.key, "ContryOfBirth", this.value);

                    }
                }
            },
            {
                name: "DateOfBirth",
                template: "<input type='date' >",
                on: {
                    "keyup": function (e) {

                        updatedata(dataContactPerson, "LineID", e.key, "DateOfBirth", this.value);

                    }
                }
            },
            {
                name: "Genders",
                template: "<select><option>---select---</option></select>",
                on: {
                    "change": function (e) {

                        updatedata(dataContactPerson, "LineID", e.key, "Gender", this.value);

                    }
                }
            },
            {
                name: "Profession",
                template: "<input >",

                on: {
                    "keyup": function (e) {

                        updatedata(dataContactPerson, "LineID", e.key, "Profession", this.value);

                    }
                }
            },
            {
                name: "SetAsDefualt",
                template: "<input type='checkbox' name='radio' class='input-box-kernel'>",
                on: {
                    "click": function (e) {
                        const active = $(this).prop("checked") ? true : false;
                        updatedata(dataContactPerson, "LineID", e.key, "SetAsDefualt", active);
                        updateSetDefault(contactPerson, contactPerson.yield(), e.key, active);
                        //contactPerson.updateColumn(dataContactPerson, "LineID", e.key, "SetAsDefualt", false);
                    }
                }
            },
        ]
    });
    $.get("/BusinessPartner/GetDefultContactPerson", function (res) {
        contactPerson.bindRows(res);
        dataContactPerson = contactPerson.yield();
    }
    )
    $("#SaveAll").click(function () {
        let g2 = parseInt($("#group2").val());
        let g1 = parseInt($("#slg1").val());
        let bcode = $("#codeid").val();
        let bname = $("#nameid").val();
        let bname2 = $("#nameid2").val();
        let txt_empid = $("#txt_empid").val();
        let txt_vatnumber = $("#txt_vatnumber").val();
        let gpslink = $("#gpslink").val();
        let paymentid = $("#paymentid").val();
        let crlimit = $("#crlimit").val();
        let bpricelistid = $("#pricelistid option:selected").val();
        let bphone = $("#phoneid").val();
        let bemail = $("#emailid").val();
        let baddress = $("#addressid").val();
        let account = $("#accounts-receivable input").val();

        masterbp.Code = bcode;
        masterbp.Name = bname;
        masterbp.Name2 = bname2;
        masterbp.PaymentTermsID = paymentid;
        masterbp.SaleEMID = txt_empid;
        masterbp.VatNumber = txt_vatnumber;
        masterbp.GPSink = gpslink;
        masterbp.CreditLimit = crlimit;
        masterbp.PriceListID = bpricelistid;
        masterbp.Type = "Vendor";
        masterbp.Phone = bphone;
        masterbp.Email = bemail;
        masterbp.Address = baddress;
        masterbp.Group1ID = isNaN(g1) ? 0 : g1;
        masterbp.Group2ID = isNaN(g2) ? 0 : g2;
        masterbp.ContactPeople = contactPerson.yield().length == 0 ? new Array() : contactPerson.yield();
        $.post("/BusinessPartner/CreateVendor", { data: JSON.stringify(masterbp) }, function (respones) {
            new ViewMessage({
                summary: {
                    selector: "#error-summary",
                },
            }, respones);
            if (respones.IsApproved) {
                setTimeout(function () {
                    location.reload()
                }, 1000)
            }
            //if (respones.url) location.href = respones.url;
        })
    });
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function updatedata(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue)
                    i[prop] = propValue
            })
        }
    }
    raido();
    function raido() {
        $.grep(contactPerson.yield(), function (item, i) {
            contactPerson.updateColumn(item.LineID, "SetAsDefualt", false);
            contactPerson.bindRows(item);
        });

    }

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
                                    masterbp.GLAccID = e.data.ID;
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
    });
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
                        let addOption = "";
                        $.each(respones, function (i, item) {
                            addOption +=
                                '<option value="' + item.ID + '">' + item.Name + '</option>';
                        });
                        $("#pricelistid").html(addOption);
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
});