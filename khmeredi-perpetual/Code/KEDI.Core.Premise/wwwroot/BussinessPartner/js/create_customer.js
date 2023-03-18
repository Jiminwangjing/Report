
let masterbp = JSON.parse($("#master_bus").text());
//let detailbps = JSON.parse($("#detail_auto").text());
let detailbps = $("#detail_auto").text() == null || $("#detail_auto").text() == " " ? "" : JSON.parse($("#detail_auto").text());
let vmclist;

$(document).ready(function () {
    let dataContactPerson = [];
    let dataBranch = [];
    convertDateToYearPicker();
    $("#appytax").click(function () {
        if ($(this).prop("checked")) {
            $("#updatetax").prop("checked", false);

        }
    })
    $("#updatetax").click(function () {
        if ($(this).prop("checked")) {
            $("#appytax").prop("checked", false);

        }
    })

    $("#Editupdatetax").click(function () {
        if ($(this).prop("checked")) {
            $("#Editappytax").prop("checked", false);

        }
    })
    $("#Editappytax").click(function () {
        if ($(this).prop("checked")) {
            $("#Editupdatetax").prop("checked", false);

        }
    })
    //======================customer source=======
    let __setupcustomersource = [];
    $("#add-new-list-cus").click(function () {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#add-cus"

            },
            caption: "SetUp List Type"
        });
        dialog.invoke(function () {
            $setuplisttype = ViewTable({
                keyField: "LineID",
                selector: "#list-cus",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 20
                },
                visibleFields: ["Name"],
                columns: [
                    {
                        name: "Name",
                        template: "<input type='text'>",
                        on: {
                            "keyup": function (e) {
                                updatedata(__setupcustomersource, "LineID", e.key, "Name", this.value);
                            }
                        }
                    },
                ],

            });


            $.get(
                "/BusinessPartner/GetEmptyTableListTerritory",
                function (res) {
                    $("#search-list-type").on("keyup", function () {
                        let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                        var rgx = new RegExp(keyword);
                        var searcheds = $.grep(__setupcustomersource, function (item) {
                            if (item !== undefined) {
                                return item.Name.toLowerCase().match(keyword);

                            }
                        });
                        $setuplisttype.bindRows(searcheds);

                    });
                    $setuplisttype.bindRows(res);
                    __setupcustomersource = $setuplisttype.yield();
                }
            )
        })
        dialog.reject(function () {

            dialog.shutdown();
        });
        dialog.confirm(function () {
            var setuptype = [];
            __setupcustomersource.forEach(i => {
                if (i.Name != "")
                    setuptype.push(i);
            })
            $.ajax({
                url: "/BusinessPartner/InsertTerrority",
                type: "POST",
                dataType: "JSON",
                data: { territory: setuptype },
                success: function (__res) {
                    var data = "";
                    $.each(__res.Data, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.Name + '</option>';
                    });
                    $("#listofterr").html(data);
                }
            })
            dialog.shutdown();

        });
    })
    //=======================list territory=========
    $("#list_terriotry").click(function () {
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
                selector: "#cont-terr"
            }
        });
        dialog.invoke(function () {
            $.get("/BusinessPartner/GetTerritoryEmployee", function (resp) {
                let $listControlGL = ViewTable({
                    keyField: "ID",
                    indexed: true,
                    selector: "#tbl_terr",
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    visibleFields: ["Name"],
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down'></i>",
                            on: {
                                "click": function (e) {
                                    $("#txt_terrid").val(e.data.ID);
                                    $("#list_terriotry").val(e.data.Name);
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
    vmclist = ViewTable({
        selector: "#list-body-vehicle",
        keyField: "KeyID",
        visibleFields: [
            "Plate", "Frame", "Engine", "VehiTypes", "VehiBrands", "VehiModels", "Year", "VehiColors"
        ],
        columns: [
            {
                name: "Plate",
                template: "<input>",
                on: {
                    "keyup": function (e) {
                        let _value = this.value;
                        $.grep(masterbp.AutoMobile, function (item) {
                            if (item.KeyID == e.key) {
                                item.Plate = _value;
                            }
                        });
                    }
                }
            },
            {
                name: "Frame",
                template: "<input>",
                on: {
                    "keyup": function (e) {
                        let _value = this.value;
                        $.grep(masterbp.AutoMobile, function (item) {
                            if (item.KeyID == e.key) {
                                item.Frame = _value;
                            }
                        });
                    }
                }
            },
            {
                name: "Engine",
                template: "<input>",
                on: {
                    "keyup": function (e) {
                        let _value = this.value;
                        $.grep(masterbp.AutoMobile, function (item) {
                            if (item.KeyID == e.key) {
                                item.Engine = _value;
                            }
                        });
                    }
                }
            },
            {
                name: "VehiTypes",
                template: "<select>",
                on: {
                    "change": function (e) {
                        let _value = this.value;
                        $.grep(masterbp.AutoMobile, function (item) {
                            if (item.KeyID == e.key) {
                                item.TypeID = parseInt(_value);
                            }
                        });
                    }
                }
            },
            {
                name: "VehiBrands",
                template: "<select>",
                on: {
                    "change": function (e) {
                        let _value = this.value;
                        $.grep(masterbp.AutoMobile, function (item) {
                            if (item.KeyID == e.key) {
                                item.BrandID = parseInt(_value);
                            }
                        });
                    }
                }
            },
            {
                name: "VehiModels",
                template: "<select>",
                on: {
                    "change": function (e) {
                        let _value = this.value;
                        $.grep(masterbp.AutoMobile, function (item) {
                            if (item.KeyID == e.key) {
                                item.ModelID = parseInt(_value);
                            }
                        });
                    }
                }
            },
            {
                name: "Year",
                template: "<input>",
                on: {
                    "keyup": function (e) {
                        let _value = this.value;
                        $.grep(masterbp.AutoMobile, function (item) {
                            if (item.KeyID == e.key) {
                                item.Year = _value;
                            }
                        });
                    }
                }
            },
            {
                name: "VehiColors",
                template: "<select>",
                on: {
                    "change": function (e) {
                        let _value = this.value;
                        $.grep(masterbp.AutoMobile, function (item) {
                            if (item.KeyID == e.key) {
                                item.ColorID = parseInt(_value);
                            }
                        });
                    }
                }
            }
        ]
    });


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
    let $tabranch = ViewTable({
        keyField: "LineID",
        selector: ".branch-tb",
        indexed: true,
        paging: {
            enabled: true,
            pageSize: 20
        },
        visibleFields: [
            "Name",
            "Tel",
            "Address",
            "Email",
            "BranchCotactPerson",
            "ContactTel",
            "ContactEmail",
            "GPSLink",
            "SetDefualt",

        ],
        columns: [

            {
                name: "Name",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updatedata(dataBranch, "LineID", e.key, "Name", this.value);
                    }
                }
            },

            {
                name: "Tel",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updatedata(dataBranch, "LineID", e.key, "Tel", this.value);
                    }
                }
            },
            {
                name: "Address",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updatedata(dataBranch, "LineID", e.key, "Address", this.value);
                    }
                }
            },
            {
                name: "Email",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updatedata(dataBranch, "LineID", e.key, "Email", this.value);
                    }
                }
            },
            {
                name: "BranchCotactPerson",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updatedata(dataBranch, "LineID", e.key, "BranchCotactPerson", this.value);
                    }
                }
            },
            {
                name: "ContactTel",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updatedata(dataBranch, "LineID", e.key, "ContactTel", this.value);
                    }
                }
            },
            {
                name: "ContactEmail",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updatedata(dataBranch, "LineID", e.key, "ContactEmail", this.value);
                    }
                }
            },
            {
                name: "GPSLink",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updatedata(dataBranch, "LineID", e.key, "GPSLink", this.value);
                    }
                }
            },
            {
                name: "SetDefualt",
                template: "<input type='checkbox' name='radio' class='input-box-kernel'>",
                on: {
                    "click": function (e) {
                        const active = $(this).prop("checked") ? true : false;
                        updatedata(dataContactPerson, "LineID", e.key, "SetDefualt", active);
                        updateSetDefault($tabranch, $tabranch.yield(), e.key, active);

                    }
                }
            },
        ]
    });
    $.get("/BusinessPartner/GetDefultBranch",function (res) {
            $tabranch.bindRows(res);
            dataBranch = $tabranch.yield();
        }
    )

    $("#accounts-receivable").on("click", function () {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    }
                }
            },
            caption: 'GL/Account',
            content: {
                selector: "#controlAc-gl-content"
            }
        });

        dialog.invoke(function () {
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
            $.get("/bussinesspartner/glcontrolaccount", function (resp) {
                $listControlGL.bindRows(resp);
                $("#txtSearch").on("keyup", function (e) {
                    let __value = this.value.toLowerCase().replace(/\s+/, "");
                    let rex = new RegExp(__value, "gi");
                    let __glaccs = $.grep(resp, function (person) {
                        return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s+/, "").match(rex);
                    });
                    $listControlGL.bindRows(__glaccs);
                });
            });
        });
    });

    // Create New Vehicle
    $("#create_vehicle").click(function () {
        let dialog = new DialogBox({
            type: 'yes-no',
            button: {
                yes: {
                    text: "Drop to list",
                },
                no: {
                    text: "Close"
                }
            },
            caption: 'Vehicle Detail',
            content: {
                selector: "#model_create_v"
            }
        });
        dialog.invoke(function () {
            $("#yearid").yearpicker();
            dialog.confirm(function () {
                createNewVehicle();
            })
            dialog.reject(function () {
                clearVehicle();
                dialog.shutdown()
            })
        })
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
    //============Payment Term=========
    $("#editB").hide();
    $("#showButton").click(function () {
        $("#editB").show();
    });
    $("#added").hide();
    $("#update").click(function () {
        $("#added").show();

    });

    $("#SaveAll").click(function () {
        let g2 = parseInt($("#group2").val());
        let g1 = parseInt($("#slg1").val());
        masterbp.CreditLimit = $("#crlimit").val();
        masterbp.GPSink = $("#gpslink").val();
        masterbp.CustomerSourceID = $("#listofterr").val();
        masterbp.Code = $("#codeid").val();
        masterbp.PaymentTermsID = $("#paymentid").val();
        masterbp.Name = $("#nameid1").val();
        masterbp.Name2 = $("#nameid2").val();
        masterbp.SaleEMID = $("#txt_empid").val();
        masterbp.VatNumber = $("#txt_vatnumber").val();
        masterbp.PriceListID = $("#pricelistid").val();
        masterbp.Group1ID = isNaN(g1) ? 0 : g1;
        masterbp.Group2ID = isNaN(g2) ? 0 : g2;
        masterbp.Type = $("#typecusid option:selected").val();
        masterbp.Phone = $("#phoneid").val();
        masterbp.Email = $("#emailid").val();
        masterbp.Address = $("#addressid").val();
        masterbp.Point = $("#chk_point").is(":checked")?true:false;
        masterbp.ContactPeople = contactPerson.yield().length == 0 ? new Array() : contactPerson.yield();
        masterbp.BPBranches = $tabranch.yield().length == 0 ? new Array() : $tabranch.yield();
      
        $.post("/BusinessPartner/SubmmitDataBP", { _data: JSON.stringify(masterbp) }, function (respones) {
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
        })
    })



    function convertDateToYearPicker() {
        $.each($("[data-year]"), function (i, y) {
            $(y).yearpicker();
        });
    }
    function clearVehicle() {
        $("#plateid").val("");
        $("#frameid").val("");
        $("#engineid").val("");
        $("#yearid").val("");
        $("#typeid").val(0);
        $("#brandid").val(0);
        $("#modelid").val(0);
        $("#colorid").val(0);
    }
    function createNewVehicle() {
        let keyID = Date.now();
        let plate = $("#plateid").val();
        let frame = $("#frameid").val();
        let engine = $("#engineid").val();
        let year = $("#yearid").val();
        let typeID = $("#typeid option:selected").val();
        let brandID = $("#brandid option:selected").val();
        let modelID = $("#modelid option:selected").val();
        let colorID = $("#colorid option:selected").val();

        if (plate.trim() == "") {
            error("Please input plate");
            return;
        }
        if (frame.trim() == "") {
            error("Please input frame");
            return;
        }
        if (engine.trim() == "") {
            error("Please input engine");
            return;
        }
        if (typeID == 0) {
            error("Please select type");
            return;
        }
        if (brandID == 0) {
            error("Please select brand");
            return;
        }
        if (modelID == 0) {
            error("Please select model");
            return;
        }
        if (year.trim() == "") {
            error("Please input year");
            return;
        }
        if (colorID == 0) {
            error("Please select color");
            return;
        }

        let item = {
            KeyID: keyID,
            AutoMID: 0,
            Plate: plate,
            Frame: frame,
            Engine: engine,
            TypeID: typeID,
            BrandID: brandID,
            ModelID: modelID,
            Year: year,
            ColorID: colorID
        }

        masterbp.AutoMobile.push(item);

        $.ajax({
            url: "/BusinessPartner/GetBusDetail",
            type: "POST",
            dataType: "JSON",
            data: { data: item },
            success: function (_detail) {
                vmclist.addRow(_detail);
            }
        });
        clearVehicle();
    }
    function error(message) {
        $("#error_val").text(message);
        $("#show-hide").show();
        setTimeout(function () {
            $("#show-hide").hide();
        }, 3000);
    }
    function error_bus(message) {
        $("#error_val_bus").text(message);
        $("#sh_bus").show();
        setTimeout(function () {
            $("#sh_bus").hide();
        }, 3000);
    }
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
});
