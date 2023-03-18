"use strict";
$(function () {
    let masterAll = JSON.parse($("#objectLoanPartner").text());

    let dataContactPerson = [];

    // Dialog create Group 1
    $("#creategroup1").click(function () {
        let dialog = new DialogBox({
            type: 'yes-no-cancel',
            button: {
                yes: {
                    text: "Add",
                },
                no: {
                    text: "New",
                    callback: function () {
                        $("#g1name").val("");
                         $("#g1id").val(0);
                    }
                },
                cancel: {
                    text: "Close",
                    callback: function () {
                        $("#g1name").val("");
                         $("#g1id").val(0);
                        this.meta.shutdown();
                    }
                }
            },
            caption: 'Create New Group 1',
            content: {
                selector: "#model-group1"
            }
        });
        dialog.invoke(function () {
            CreateGroup1();
        });
        dialog.confirm(function () {
            const obj={};
                obj.ID=$("#g1id").val();
                obj.Name=$("#g1name").val();
                obj.Grouploan=1;
               
            $.post("/LoanPartner/CreateGroup1",{data:obj},function(res){
            
                new ViewMessage({
                    summary: {
                        selector: "#error-summary",
                    },
                }, res);
                $("#g1name").val("");
                $("#g1id").val(0);
                CreateGroup1();
            }); 
        });
    });
    /////////////////////////// Dialog Create Group 2
    $("#creategroup2").click(function () {
        let dialog = new DialogBox({
            type: 'yes-no-cancel',
            button: {
                yes: {
                    text: "Add",
                },
                no: {
                    text: "New",
                    callback: function () {
                        $("#txt_group2name").val("");
                        $("#slectg1").val(0);
                        $("#g2id").val(0);
                    }
                },
                cancel: {
                    text: "Close",
                    callback: function () {
                        $("#txt_group2name").val("");
                        $("#slectg1").val(0);
                        $("#g2id").val(0);
                        this.meta.shutdown();
                    }
                }
            },
            caption: 'Create New Group 1',
            content: {
                selector: "#modal-group2"
            }
        });
        dialog.invoke(function () {
            CreateGroup2();
        });
        dialog.confirm(function () {
            const obj={};
                obj.ID=$("#g2id").val();
                obj.Group1ID=$("#slectg1").val();
                obj.Name=$("#txt_group2name").val();
                obj.Grouploan=2;
               
            $.post("/LoanPartner/CreateGroup1",{data:obj},function(res){
            
                new ViewMessage({
                    summary: {
                        selector: "#error-summary",
                    },
                }, res);
                $("#txt_group2name").val("");
                $("#slectg1").val(0);
                $("#g2id").val(0);
                CreateGroup2();
            }); 
        });
    });

    // Dilog Table Employee
    $("#txt_saleemp").click(function () 
    {

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
                selector: "#cont-em"
            }
        });
        dialog.invoke(function () {
            $.get("/LoanPartner/GetSaleEmployee", function (resp) {
                let $listControlGL = ViewTable({
                    keyField: "ID",
                    indexed: true,
                    selector: "#tbl_emp",
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    visibleFields: ["Code", "Name", "GenderDisplay", "Position", "Address", "Phone", "Email", "EMType"],
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down'></i>",
                            on: {
                                "click": function (e) {
                                    $("#txt_empid").val(e.data.ID);
                                    $("#txt_saleemp").val(e.data.Name);
                                  
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                $listControlGL.clearRows();
                $listControlGL.bindRows(resp);
                
                search(resp, "#txtSearchemp", $listControlGL);
            });
        });
    });

    function search(data, inputSearch, table) 
    {
        if (data.length > 0) {
            $(inputSearch).on("keyup", function (e) {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let rex = new RegExp(__value, "gi");
                let items = $.grep(data, function (item) {
                    return item.Code.toLowerCase().replace(/\s+/, "").match(rex)||
                            item.Name.toLowerCase().replace(/\s+/, "").match(rex);
                });
                table.bindRows(items);
            });
        }
    }


    
    // CreateGroup1();
    // CreateGroup2();
    function CreateGroup1() {
        $.get("/LoanPartner/GetLoanGroup",{group1:1}, function (resp) {
            let $listGroup1 = ViewTable({
                keyField: "ID",
                indexed: true,
                selector: "#tbl_group1",
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
                               
                                dialog.shutdown();
                            }
                        }
                    },
                    {
                        template: "<i class='fa fa-edit fa-lg'></i>",
                        on: {
                            "click": function (e) {
                               
                                $("#g1id").val(e.data.ID);
                                $("#g1name").val(e.data.Name);
                            }
                        }
                    }
                ]
            });
           
            $listGroup1.clearRows();
            $listGroup1.bindRows(resp);
            if(resp.length>0)
            {
                let selectg1=`<option value="0">--- select item ---</option>`;
                resp.forEach(i=> {
                    selectg1+=`<option value="${i.ID}">${i.Name}</option>`;
                });
                $("#slg1").html(selectg1);
                $("#slectg1").html(selectg1);
            }
        });

    }

    function CreateGroup2() {
        $.get("/LoanPartner/GetLoanGroup",{group1:2}, function (resp) {
            let $listGroup2 = ViewTable({
                keyField: "ID",
                indexed: true,
                selector: "#tbl_group2",
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
                               
                                dialog.shutdown();
                            }
                        }
                    },
                    {
                        template: "<i class='fa fa-edit fa-lg'></i>",
                        on: {
                            "click": function (e) {
                                $("#txt_group2name").val(e.data.Name);
                                $("#slectg1").val(e.data.Group1ID);
                                $("#g2id").val(e.data.ID);
                               
                            }
                        }
                    }
                ]
            });
           
            $listGroup2.clearRows();
            $listGroup2.bindRows(resp);
            if(resp.length>0)
            {
                let selectg1=`<option value="0">--- select item ---</option>`;
                resp.forEach(i=> {
                    selectg1+=`<option value="${i.ID}">${i.Name}</option>`;
                });
                $("#group2").html(selectg1);
            }
        });
       
    }


$("#txtSearchg1").on("keyup", function (e) {
    var value = $(this).val().toLowerCase();
    $("#tbl_group1 tr:not(:first-child)").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
    });
});
$("#txtSearchg2").on("keyup", function (e) 
{
    var value = $(this).val().toLowerCase();
    $("#tbl_group2 tr:not(:first-child)").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
    });
});

    ////////////////////////////////////////////////////////
   
    var contactPerson = ViewTable({
        keyField: "LineID",
        selector: ".item-detail",
        indexed: true,
        paging: {
            enabled: false,
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
                        updatedata(dataContactPerson, "LineID", e.key, "ContryOfBirth", parseInt(this.value));
                        console.log("Data=",this.value);
                     

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
                        console.log("Datad=",this.value);

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
                       
                    }
                }
            },
        ]
    });
    
    $("#addrow").click(function(){
        
        $.get("/LoanPartner/AddRowsConteractPerson", function (res) {
            contactPerson.addRow(res);
            dataContactPerson = contactPerson.yield();
        });
    });
  $("#submit").click(function(){
    let LoanPartner={};
    LoanPartner.ID      = $("#id").val();
    LoanPartner.Code    = $("#codeid").val();
    LoanPartner.Name1   = $("#name1").val();
    LoanPartner.Name2   = $("#name2").val();
    LoanPartner.Group1ID= $("#slg1").val();
    LoanPartner.Group2ID= $("#group2").val();
    LoanPartner.EmpID   = $("#txt_empid").val();
    LoanPartner.EmloyeeName = $("#txt_saleemp").val();
    LoanPartner.Phone       = $("#phone").val();
    LoanPartner.Email       = $("#email").val();
    LoanPartner.Address     = $("#address").val();
    LoanPartner.VatNumber   = $("#txt_vatnumber").val();
    LoanPartner.GPSLink     = $("#gpslink").val();
    LoanPartner.LoanContactPeople = dataContactPerson;
   
   
    $.post("/LoanPartner/SaveLoanPartner",{obj:LoanPartner},function(model){
        console.log("Test=",model)
         if (model.IsApproved) 
        {
            new ViewMessage({
                summary: {
                    selector: "#error-summary"
                }
            }, model.Model); 
            location.href = "/LoanPartner/Index";        
         }
         new ViewMessage({
            summary: {
                selector: "#error-summary"
            }
        }, model.Model);
        $(window).scrollTop(0);
                    
                
    });
   

  });
  FindLoanpartner();
  function FindLoanpartner()
  {
    
        if(masterAll.LoanContactPeople.length>0)
        {
            contactPerson.bindRows(masterAll.LoanContactPeople);
            dataContactPerson = contactPerson.yield();
            console.log("dataContactPerson=",dataContactPerson)
        }else
        {
            $.get("/LoanPartner/GetDefultRowsConteractPerson", function (res) {
       
                contactPerson.bindRows(res);
                dataContactPerson = contactPerson.yield();
            });
        }
    
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
                        //table.updateColumn(i.LineID, "SetDefualt", true);
                    } else {
                        table.updateColumn(i.LineID, "SetAsDefualt", false);
                       // table.updateColumn(i.LineID, "SetDefualt", false);
                    }
                }
            } else {
                table.updateColumn(id, "SetAsDefualt", false);
               // table.updateColumn(id, "SetDefualt", false);
            }
        }
    }
});