
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
    vmclist = ViewTable({
        selector: "#list-body-vehicle",
        keyField: "KeyID",
        visibleFields: [
            "Plate", "Frame", "Engine", "VehiTypes", "VehiBrands", "VehiModels", "Year", "VehiColors"
        ],
        dataSynced: true,
        model: {
            BusinessPartner: {
                AutoMobile: []
            }
        },
        columns: [
            {
                name: "Plate",
                template: "<input>",
            },
            {
                name: "Frame",
                template: "<input>",
            },
            {
                name: "Engine",
                template: "<input>",
            },
            {
                name: "VehiTypes",
                nameField: "TypeID",
                template: "<select>",
            },
            {
                name: "VehiBrands",
                template: "<select>",
                nameField: "BrandID",
            },
            {
                name: "VehiModels",
                template: "<select>",
                nameField: "ModelID",
            },
            {
                name: "Year",
                template: "<input data-year>",
            },
            {
                name: "VehiColors",
                template: "<select>",
                nameField: "ColorID",
            }
        ]
    }).bindRows(vmcDetails);
    $("#model_create_v").on("click", function () {

    });


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


    let bp = JSON.parse($("#vmc-all").text());
    var contactPersonTable = ViewTable({
        selector: ".item-detail",
        keyField: "LineID",
        paging: {
            pageSize: 5,
            enabled: true
        },
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
    var branchTable = ViewTable({
        selector: ".branch-tb",
        keyField: "LineID",
        paging: {
            pageSize: 10,
            enabled: true
        },
        indexed: true,
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
        dataSynced: true,
        columns: [
            {
                name: "Name",
                template: "<input >"
            },
            {
                name: "Tel",
                template: "<input>"
            },
            {
                name: "Address",
                template: "<input>"
            },
            {
                name: "Email",
                template: "<input>"
            },
            {
                name: "BranchCotactPerson",
                template: "<input>"
            },
            {
                name: "ContactTel",
                template: "<input>"
            },
            {
                name: "ContactEmail",
                template: "<input>"
            },
            {
                name: "GPSLink",
                template: "<input>"
            },
            {
                name: "SetDefualt",
                nameField: "SetDefualt",
                template: "<input type='checkbox'  class='input-box-kernel'>",
                on: {
                    "click": function (e) {
                        const active = $(this).prop("checked") ? true : false;
                        updateSetDefault(branchTable, branchTable.yield(), e.key, active);


                    }
                }
            },
        ],
        model: {
            BusinessPartner: {
                BPBranches: []
            }
        }
    });
    branchTable.bindRows(bp.BusinessPartner.BPBranches);
    //====================Contract AND billing==================

   

    var ContractBilingsTable = ViewTable({
        selector: ".contract-tb",
        keyField: "LineID",
        paging: {
            pageSize: 5,
            enabled: true
        },
        indexed: true,
        model: {
            BusinessPartner: {
                ContractBilings: []
            }
        },
        dataSynced: true,
        visibleFields: [
            "DocumentNo",
            "Amount",
            "Expires",
            "Statuss",
            "ConfrimRenews",
            "Payments",
            "NewContractStartDate",
            "NewContractEndDate",
            "NextOpenRenewalDate",
            "Renewalstartdate",
            "Renewalenddate",
            "TerminateDate",
            "ContractTypes",
            "SelectContractname",
            "SetupContractName",
            "Activities",
            "EstimateSupportCost",
            "Remark",
            "FileName",
            "Path",
        ],
        columns: [
            {
                name: "DocumentNo",
                template: "<input readonly>"
            },
            {
                name: "Amount",
                template: "<input readonly>"
            },
            {
                name: "Expires",
                template: "<input readonly style='width:200px !important'>"
            },
            {
                name: "Statuss",
                nameField: "Status",
                template: "<select></select>"
            },
            {
                name: "ConfrimRenews",
                nameField: "ConfrimRenew",
                template: "<select></select>"
            },
            {
                name: "Payments",
                nameField: "Payment",
                template: "<select></select>"
            },
            {
                name: "NewContractStartDate",
                template: "<input type='date'>"
            },
            {
                name: "NewContractEndDate",
                template: "<input type='date'>",
                on: {
                    "change": function (e) {
                        time(new Date(), this.value);
                        time(this.value, new Date());

                        function time(startDate, endDate) {
                            const sd = moment(startDate);
                            const ed = moment(endDate);
                            var duration = moment.duration(ed.diff(sd));
                            var days = duration.asDays() + 1;
                            var yearss = Math.floor(days / 365);
                            var monthss = Math.floor(days % 365 / 30);
                            var dayss = Math.floor(days % 365 % 30);
                            var dayss = Math.floor(days % 365 % 30);
                            if (days >= 1 && monthss == 0 && yearss == 0) {
                                var dayss = Math.floor(days % 365 % 30);
                                ContractBilingsTable.updateColumn(e.key, "Expires", dayss + " Days");
                            }
                            else if (days >= 1 && monthss >= 1 && yearss == 0) {
                                var monthss = Math.floor(days % 365 / 30);
                                var dayss = Math.floor(days % 365 % 30);
                                ContractBilingsTable.updateColumn(e.key, "Expires", monthss + " Months " + " And " + dayss + " Days");
                            }
                            else if (days >= 1 && monthss >= 1 && yearss >= 1) {
                                var yearss = Math.floor(days / 365);
                                var monthss = Math.floor(days % 365 / 30);
                                var dayss = Math.floor(days % 365 % 30);
                                ContractBilingsTable.updateColumn(e.key, "Expires", yearss + " And " + " Year" + monthss + " And " + " Months" + dayss + "Days");
                            }
                        }

                    }
                }
            },
            {
                name: "NextOpenRenewalDate",
                template: "<input type='date'>"
            },
            {
                name: "Renewalstartdate",
                template: "<input type='date'>"
            },
            {
                name: "Renewalenddate",
                template: "<input type='date'>"
            },
            {
                name: "TerminateDate",
                template: "<input type='date'>"
            },
            {
                name: "ContractTypes",
                nameField: "ContractType",
                template: "<select ></select>"
            },
            {
                name: "SelectContractname",
                nameField: "ContractID",
                template: "<select class='contractname'>----select------</select>"
            },
            {
                name: "SetupContractName",
                template: '<i  class="fas fa-plus-square text-center" style="color:sandybrown;font-size:15px;margin-left:40%"  ></i>',
                on: {
                    "click": function (e) {
                        __setupcontractName();
                    }
                }
            },
            {
                name: "Activities",
                template: "<input readonly >",
                on: {
                    "click": function (e) {
                        $.ajax({
                            url: "/BusinessPartner/GetdataActivity",
                            type: "Get",
                            dataType: "Json",
                            data: { id: $("#busid").val() },
                            success: function (data) {
                                Activity(data, e);
                            }
                        });
                    }
                }
            },
            {
                name: "EstimateSupportCost",
                template: "<input >"
            },
            {
                name: "Remark",
                template: "<input >"
            },

        ],
        actions: [
            {
                template: `<i class="fas fa-paperclip"> <form method="post" id="form_upload" enctype="multipart/form-data">  
                                      <input type="file"accept=".xls, .xlsx, .pdf, .docx, .pptx,.txt" />                  
                                    </form></i>`,
                on: {
                    "change": function (e) {
                        var fileUpload = $(this).children('form').children('input').get(0);
                        SaveFile(ContractBilingsTable, fileUpload, e.data.LineID);
                        ContractBilingsTable.updateColumn(e.key, "FileName", e.FileName);
                        ContractBilingsTable.updateColumn(e.key, "Path", e.Path);

                    }
                }
            },
            {
                template: `<i class= "fas fa-download" ></i >`,
                on: {
                    "click": function (e) {
                        location.href = "/BusinessPartner/DowloadFile?AttachID=" + e.data.ID;
                    }
                }
            },
            {
                template: `<i class="far fa-trash-alt" id='dislike'></i>`,
                on: {
                    "click": function (e) {
                        if (e.data.ID == 0) {
                            $.post("/BusinessPartner/RemoveFileFromFolderMastre", { file: e.data.FileName }, function (res) {
                                ContractBilingsTable.updateColumn(e.key, "Path", "");
                                ContractBilingsTable.updateColumn(e.key, "FileName", "");

                            });
                        }
                        else {
                            //$.post("/BusinessPartner/DeleteFileFromDatabase", { id: e.data.ID,  key: e.data.LineIdM }, function (res) {
                            //    ContractBilingsTable.bindRows(res);
                            //});
                            $('.dislike').prop('disabled', true);
                        }
                    }
                }
            },
        ],
    });

    ContractBilingsTable.bindRows(bp.BusinessPartner.ContractBilings);

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
            url: '/BusinessPartner/SaveAttachment', //URL to upload files
            type: "POST", //as we will be posting files and other method POST is used
            //enctype: "multipart/form-data",
            processData: false, //remember to set processData and ContentType to false, otherwise you may get an error
            contentType: false,
            cache: false,
            data: fileData,
            success: function (result) {
                tablename.updateColumn(key, "Path", result.Path);
                tablename.updateColumn(key, "FileName", result.FileName);
                tablename.updateColumn(key, "AttachmentDate", result.AttachmentDate);
            },
            error: function (err) {
                alert(err.statusText);
            }
        });
    }
    //=========Model box of activity===========
    function Activity(data, _e) {
        let dialog = new DialogBox({
            content: {
                selector: ".activity_containers"
            },
            caption: "Bussines Partner"
        });
        dialog.invoke(function () {
            const __listBussinesPartner = ViewTable({
                keyField: "ID",
                selector: "#list-activity",
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["BPCode", "BPName", "TypeName", "StartTime", "EndTime"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down" id="close"  ></i>`,
                        on: {
                            "click": function (e) {
                                location.href = "/BusinessPartner/Activity?number=" + e.data.Number;
                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listBussinesPartner.bindRows(data)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    //==============model setup name==============

    let __contractname = []

    function __setupcontractName() {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-contractname-content"

            },
            caption: "SetUp-ContractName"
        });
        dialog.invoke(function () {
            $contractname = ViewTable({
                keyField: "LineID",
                selector: "#contractname",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 20
                },
                visibleFields: ["ContractName"],
                columns: [
                    {
                        name: "ContractName",
                        template: "<input>",
                        on: {
                            "keyup": function (e) {
                                updatedata(__contractname, "LineID", e.key, "ContractName", this.value);
                            }
                        }
                    },
                ],

            });
            $.get(
                "/BusinessPartner/GetSetupContractNameDefalutTable",
                function (res) {
                    $("#search-levelinterest").on("keyup", function () {
                        let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                        var rgx = new RegExp(keyword);
                        var searcheds = $.grep(__contractname, function (item) {
                            if (item !== undefined) {
                                return item.Description.toLowerCase().match(keyword);

                            }
                        });
                        $contractname.bindRows(searcheds);

                    });
                    $contractname.bindRows(res);
                    __contractname = $contractname.yield();
                }
            )
        });
        dialog.reject(function () {

            dialog.shutdown();
        });
        dialog.confirm(function () {
            var setupcontrname = [];
            __contractname.forEach(i => {
                if (i.ContractName != "")
                    setupcontrname.push(i);
            })

            $.ajax({
                url: "/BusinessPartner/InsertSetupContractName",
                type: "POST",
                dataType: "JSON",
                data: { setupcontractname: setupcontrname },
                success: function (__res) {
                    if (__res.IsApproved) {
                        new ViewMessage({
                            summary: {
                                selector: "#sms_level"
                            },
                        }, __res.Model).refresh(1000);
                    } else {
                        new ViewMessage({
                            summary: {
                                selector: "#sms_level"
                            }
                        }, __res.Model);
                    }

                    var data = "";
                    $.each(__res.Data, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.ContractName + '</option>';
                    });
                    $(".contractname").html(data);


                }
            });
            dialog.shutdown();

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
    //============Payment Term=========
    $("#editB").hide();
    $("#showButton").click(function () {
        $("#editB").show();
    });
    $("#added").hide();
    $("#update").click(function () {
        $("#added").show();

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
                                    masterBP.GLAccID = e.data.ID;
                                    console.log("glid", masterBP.GLAccID)
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
        masterBP.AutoMobile.push(item);
        $.ajax({
            url: "/BusinessPartner/GetBusDetail",
            dataType: "JSON",
            type: "POST",
            data: { data: item },
            success: function (_detail) {
                vmclist.addRow(_detail);
                vmcDetails.push(_detail);
            }
        });
        ClearVehicle();
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
                        let sata = "";
                        $.each(respones, function (i, item) {
                            sata +=
                                '<option value="' + item.ID + '">' + item.Name + '</option>';
                        });
                        $("#pricelistid").html(sata);
                    }
                })
            }

        });
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
    $("#search-sale").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#list-sale tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    const __tablesalehistory = ViewTable({
        keyField: "LineID",
        selector: "#list-sale",
        paging: {
            enabled: true,
            pageSize: 5
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
   

    $.ajax({
        url: "/BusinessPartner/GetingSaleHistory",
        type: 'GET',
        dataType: 'JSON',
        data: { id: $("#busid").val() },
        success: function (res) {
           
            if (res.length > 0)
            {
                const i = res[0];
                $("#lcc").val(i.SGrandTotalLCC);
                $("#scc").val(i.SGrandTotalSys);
                $("#vat").val(i.SVat);
                $("#distotal").val(i.SDiscountTotal);
                $("#disitem").val(i.SDiscountItem);
                __tablesalehistory.clearRows();
                __tablesalehistory.bindRows(res);
                $("#search-sale").on("keyup", function () {
                    let __value = this.value.toLowerCase().replace(/\s+/, "");
                    let items = $.grep(res, function (item) {
                        return item.ReceiptNmber.toLowerCase().replace(/\s+/, "").includes(__value) || item.DouType.toLowerCase().replace(/\s+/, "").includes(__value)
                            || item.DateOut.toLowerCase().replace(/\s+/, "").includes(__value) || item.GrandTotal.toLowerCase().replace(/\s+/, "").includes(__value)
                    });
                    __tablesalehistory.bindRows(items)
                });
            }
        }
    });
    $("#filter-reportsale").click(function () {

        let datef = $("#datefrom").val();
        let datet = $("#dateto").val();
        if (datef != null || datef != "" && datet != null || datet != "") {
            $.get("/BusinessPartner/GetingSaleHistory", { id: $("#busid").val(), datefrom: datef, dateto: datet }, function (data) {
                if (data.length > 0) {
                    __tablesalehistory.clearRows();
                    __tablesalehistory.bindRows(data);
                }
            });
        }

    });

});

