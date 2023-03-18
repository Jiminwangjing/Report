 let attchmentfiles = [];
let __setupcontractype = [];
let __listattchment = [];
var SerchName;

$(document).ready(function () {
   
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
                                location.href = "/SetupContractTemplate/DowloadFile?AttachID=" + e.data.ID;

                            }
                        }
                    }
                },
                {
                    template: `<i class="far fa-trash-alt" id='dislike'></i>`,
                    on: {
                        "click": function (e) {
                            if (e.data.ID == 0) {
                                $.post("/SetupContractTemplate/RemoveFileFromFolderMastre", { file: e.data.FileName }, function (res) {
                                    $list_Attachfile.updateColumn(e.key, "TargetPath", "");
                                    $list_Attachfile.updateColumn(e.key, "FileName", "");
                                    $list_Attachfile.updateColumn(e.key, "AttachmentDate", "");

                                });
                            }
                            else {
                                $.post("/SetupContractTemplate/DeleteFileFromDatabase", { id: e.data.ID, key: e.data.LineIdM }, function (res) {
                                    $list_Attachfile.bindRows(res);
                                });
                                //$('.dislike').prop('disabled', true);
                            }
                        }
                    }
                },
            ],
        });
   

    let _data = JSON.parse($("#model-data").text());
    if (_data == null) {
        $.get("/SetupContractTemplate/CreateDefaultRowAttachmetDetailOfContractTemplate", { num: 10, number: 0 }, function (res) {
            if (isValidArray(res)) {
                res.forEach(i => attchmentfiles.push(i));
                $list_Attachfile.bindRows(attchmentfiles);
            }
        });
    }
    if (_data != null) {
        if (_data.Name != null) {
            $("#submit-data").prop("hidden", true);
            $("#update-data").prop("hidden", false);
            $("#btn-createnew").prop("hidden", false);
            search(_data);
          
        }
    }
    $("#expired").click(function () {
        if ($(this).is(':checked')) {
            $("#name").attr("disabled","disabled");
            $(".fas ").attr("hidden","hidden");
            $(".far ").attr("hidden","hidden");
            $("#setup-list-contractype").attr("hidden","hidden");
            $("#responsetime").attr("disabled", "disabled");
            $("#resultiontime").attr("disabled", "disabled");
            $("#listhd1").attr("disabled", "disabled");
            $("#listhd2").attr("disabled", "disabled");
            $("#description").attr("disabled", "disabled");
            $("#listoftype").attr("disabled", "disabled");
            $("#part").attr("disabled", "disabled");
            $("#labor").attr("disabled", "disabled");
            $("#travel").attr("disabled", "disabled");
            $("#holiday").attr("disabled", "disabled");

            $("#mon").attr("disabled", "disabled");
            $("#thue").attr("disabled", "disabled");
            $("#wed").attr("disabled", "disabled");
            $("#thur").attr("disabled", "disabled");
            $("#fri").attr("disabled", "disabled");
            $("#sat").attr("disabled", "disabled");
            $("#sun").attr("disabled", "disabled");
          
            $("#stimemon").attr("disabled", "disabled");
            $("#stimethue").attr("disabled", "disabled");
            $("#stimewed").attr("disabled", "disabled");
            $("#stimethur").attr("disabled", "disabled");
            $("#stimefri").attr("disabled", "disabled");
            $("#stimesat").attr("disabled", "disabled");
            $("#stimesun").attr("disabled", "disabled");

            $("#etimemon").attr("disabled", "disabled");
            $("#etimethue").attr("disabled", "disabled");
            $("#etimewed").attr("disabled", "disabled");
            $("#etimethur").attr("disabled", "disabled");
            $("#etimefri").attr("disabled", "disabled");
            $("#etimesat").attr("disabled", "disabled");
            $("#etimesun").attr("disabled", "disabled");
            //=========Remark============
            $("#remark").attr("disabled", "disabled");

        } else {
            $("#name").removeAttr("disabled", "disabled");
            $(".fas ").removeAttr("hidden", "hidden");
            $(".far ").removeAttr("hidden", "hidden");
            $("#setup-list-contractype").removeAttr("hidden", "hidden");
            $("#responsetime").removeAttr("disabled", "disabled");
            $("#resultiontime").removeAttr("disabled", "disabled");
            $("#listhd1").removeAttr("disabled", "disabled");
            $("#listhd2").removeAttr("disabled", "disabled");
            $("#description").removeAttr("disabled", "disabled");
            $("#listoftype").removeAttr("disabled", "disabled");
            $("#part").removeAttr("disabled", "disabled");
            $("#labor").removeAttr("disabled", "disabled");
            $("#travel").removeAttr("disabled", "disabled");
            $("#holiday").removeAttr("disabled", "disabled");

            $("#mon").removeAttr("disabled", "disabled");
            $("#thue").removeAttr("disabled", "disabled");
            $("#wed").removeAttr("disabled", "disabled");
            $("#thur").removeAttr("disabled", "disabled");
            $("#fri").removeAttr("disabled", "disabled");
            $("#sat").removeAttr("disabled", "disabled");
            $("#sun").removeAttr("disabled", "disabled");

            $("#stimemon").removeAttr("disabled", "disabled");
            $("#stimethue").removeAttr("disabled", "disabled");
            $("#stimewed").removeAttr("disabled", "disabled");
            $("#stimethur").removeAttr("disabled", "disabled");
            $("#stimefri").removeAttr("disabled", "disabled");
            $("#stimesat").removeAttr("disabled", "disabled");
            $("#stimesun").removeAttr("disabled", "disabled");

            $("#etimemon").removeAttr("disabled", "disabled");
            $("#etimethue").removeAttr("disabled", "disabled");
            $("#etimewed").removeAttr("disabled", "disabled");
            $("#etimethur").removeAttr("disabled", "disabled");
            $("#etimefri").removeAttr("disabled", "disabled");
            $("#etimesat").removeAttr("disabled", "disabled");
            $("#etimesun").removeAttr("disabled", "disabled");
            //=========Remark============
            $("#remark").removeAttr("disabled", "disabled");
        }
    })
    $("#mon").click(function () {
        if ($(this).is(':checked')) {
            $("#stimemon").removeAttr("disabled", "disabled")
            $("#etimemon").removeAttr("disabled", "disabled")
         
        } else {
            $("#stimemon").attr("disabled", "disabled")
            $("#etimemon").attr("disabled", "disabled")
        }
    })
    $("#thue").click(function () {
        if ($(this).is(':checked')) {
            $("#stimethue").removeAttr("disabled", "disabled")
            $("#etimethue").removeAttr("disabled", "disabled")

        } else {
            $("#stimethue").attr("disabled", "disabled")
            $("#etimethue").attr("disabled", "disabled")
        }
    })
    $("#wed").click(function () {
        if ($(this).is(':checked')) {
            $("#stimewed").removeAttr("disabled", "disabled")
            $("#etimewed").removeAttr("disabled", "disabled")

        } else {
            $("#stimewed").attr("disabled", "disabled")
            $("#etimewed").attr("disabled", "disabled")
        }
    })
    $("#thur").click(function () {
        if ($(this).is(':checked')) {
            $("#stimethur").removeAttr("disabled", "disabled")
            $("#etimethur").removeAttr("disabled", "disabled")

        } else {
            $("#stimethur").attr("disabled", "disabled")
            $("#etimethur").attr("disabled", "disabled")
        }
    })
    $("#fri").click(function () {
        if ($(this).is(':checked')) {
            $("#stimefri").removeAttr("disabled", "disabled")
            $("#etimefri").removeAttr("disabled", "disabled")

        } else {
            $("#stimefri").attr("disabled", "disabled")
            $("#etimefri").attr("disabled", "disabled")
        }
    })
    $("#sat").click(function () {
        if ($(this).is(':checked')) {
            $("#stimesat").removeAttr("disabled", "disabled")
            $("#etimesat").removeAttr("disabled", "disabled")

        } else {
            $("#stimesat").attr("disabled", "disabled")
            $("#etimesat").attr("disabled", "disabled")
        }
    })
    $("#sun").click(function () {
        if ($(this).is(':checked')) {
            $("#stimesun").removeAttr("disabled", "disabled")
            $("#etimesun").removeAttr("disabled", "disabled")

        } else {
            $("#stimesun").attr("disabled", "disabled")
            $("#etimesun").attr("disabled", "disabled")
        }
    })
    $("#btn-find").on("click", function () {
        $("#btn-createnew").removeAttr("hidden", "hidden");
        $("#name").val("").prop("readonly", false).focus();
        $("#submit-data").hide();
        $("#update-data").removeAttr("hidden");
    });

    $("#choose").on("click", function () {
        $("#btn-createnew").removeAttr("hidden", "hidden");

        $("#submit-data").hide();
        $("#update-data").removeAttr("hidden");
    });
    $("#name").on("keypress", function (e) {
        if (e.which === 13 && $("#name").val()) {
            $("#btn-find").hide();
            //$.ajax({
            //    url: "/SetupContractTemplate/FindContractTemplate",
            //    data: { name: $("#name").val().trim() },
            //    success: function (result) {
            //    }

            //});
        }
    });
    function search(result) {
        if (!result) {
            let dialog = new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Service Contract Template Not found!",
                close_button: "none"
            });

            dialog.confirm(function () {
                $("#next_number").prop("readonly", false).focus();
                dialog.shutdown();
            });

            $("#next_number").val(" ")
            $("#next_number").blur();

        }
        else {
            contracttemplate_master = result;
            $("#id").val(result.ID);
            $("#remarkid").val(result.RemarksID);
            $("#converid").val(result.ConverID);
            $("#name").val(result.Name);
            $("#responsetime").val(result.ResponseTime);
            $("#resultiontime").val(result.ResultionTime);
            $("#listhd1").val(result.ResponseTimeDH);
            $("#listhd2").val(result.ResultionTimeDH);
            $("#description").val(result.Description);
            $("#listoftype").append(`<option value=${result.ContracType} selected> ${result.ContractName} </option>`);
            $("#expired").prop("checked", result.Expired).prop("enabled", true);
            $("#part").prop("checked", result.Part).prop("enabled", true);
            $("#labor").prop("checked", result.Labor).prop("enabled", true);
            $("#travel").prop("checked", result.Travel).prop("enabled", true);
            $("#holiday").prop("checked", result.Holiday).prop("enabled", true);

            $("#mon").prop("checked", result.Monthday).prop("enabled", true);
            $("#thue").prop("checked", result.Thuesday).prop("enabled", true);
            $("#wed").prop("checked", result.Wednesday).prop("enabled", true);
            $("#thur").prop("checked", result.Thursday).prop("enabled", true);
            $("#fri").prop("checked", result.Friday).prop("enabled", true);
            $("#sat").prop("checked", result.Saturday).prop("enabled", true);
            $("#sun").prop("checked", result.Sunday).prop("enabled", true);
            if ($("#mon").is(':checked')) {
                $("#stimemon").removeAttr("disabled", "disabled");
                $("#etimemon").removeAttr("disabled", "disabled");
            }
            if ($("#thue").is(':checked')) {
                $("#stimethue").removeAttr("disabled", "disabled");
                $("#etimethue").removeAttr("disabled", "disabled");
            }
            if ($("#wed").is(':checked')) {
                $("#stimewed").removeAttr("disabled", "disabled");
                $("#etimewed").removeAttr("disabled", "disabled");
            }
            if ($("#thur").is(':checked')) {
                $("#stimethur").removeAttr("disabled", "disabled");
                $("#etimethur").removeAttr("disabled", "disabled");
            }
            if ($("#fri").is(':checked')) {
                $("#stimefri").removeAttr("disabled", "disabled");
                $("#etimefri").removeAttr("disabled", "disabled");
            }
            if ($("#sat").is(':checked')) {
                $("#stimesat").removeAttr("disabled", "disabled");
                $("#etimesat").removeAttr("disabled", "disabled");
            }
            if ($("#sun").is(':checked')) {
                $("#stimesun").removeAttr("disabled", "disabled");
                $("#etimesun").removeAttr("disabled", "disabled");
            }
            $("#stimemon").val(result.StarttimeMon);
            $("#stimethue").val(result.StarttimeThu);
            $("#stimewed").val(result.StarttimeWed);
            $("#stimethur").val(result.StarttimeThur);
            $("#stimefri").val(result.StarttimeFri);
            $("#stimesat").val(result.StarttimeSat);
            $("#stimesun").val(result.StarttimeSun);

            $("#etimemon").val(result.EndtimeMon);
            $("#etimethue").val(result.EndtimeThu);
            $("#etimewed").val(result.EndtimeWed);
            $("#etimethur").val(result.EndtimeThur);
            $("#etimefri").val(result.EndtimeFri);
            $("#etimesat").val(result.EndtimeSat);
            $("#etimesun").val(result.EndtimeSun);
            //=========Remark============
            $("#remark").val(result.Remarks);
            __listattchment = result.AttachmentFiles;
            $list_Attachfile.bindRows(__listattchment);

        }
    }
    $("#history").click(function () {
        $("#btn-createnew").removeAttr("hidden", "hidden");
        $("#name").val("").prop("readonly", false).focus();
        $("#submit-data").hide();
        $("#update-data").removeAttr("hidden");
        $.ajax({
            url: "/SetupContractTemplate/GetListContractTemplate",
            type: "Get",
            dataType: "Json",
            data: { data: "True" },
            success: function (response) {
                bindServiceContract(response);
            }
        });
    });
    var name;
    function bindServiceContract(res) {
        let dialog = new DialogBox({
            content: {
                selector: ".listservicecontract_containers"
            },
            caption: "List Contract Template"
        });
        dialog.invoke(function () {
            const __listBussinesPartner = ViewTable({
                keyField: "ID",
                selector: "#list-contract",
                paging: {
                    pageSize: 15,
                    enabled: true
                },
                visibleFields: ["Name", "ContracType", "Description"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down" id="close"  ></i>`,
                        on: {
                            "click": function (e) {
                                $("#btn-createnew").removeAttr("hidden", "hidden");
                                name = e.data.Name;
                                $.ajax({
                                    url: "/SetupContractTemplate/FindContractTemplate",
                                    data: { name: name},
                                    success: function (result) {
                                        search(result)
                                    }

                                });

                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listBussinesPartner.bindRows(res)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
  
  

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
            url: '/SetupContractTemplate/SaveAttachment', //URL to upload files
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

    $("#setup-list-contractype").click(function () {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-type-content"

            },
            caption: "SetUp-Contract Type"
        });
        dialog.invoke(function () {
            $setupcontracttype = ViewTable({
                keyField: "LineID",
                selector: "#setup-contractype",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 10
                },
                visibleFields: ["ContractType"],
                columns: [
                    {
                        name: "ContractType",
                        template: "<input type='text' >",
                        on: {
                            "keyup": function (e) {

                                updatedata(__setupcontractype, "LineID", e.key, "ContractType", this.value);
                            }
                        }
                    },
                ],

            });
            $.get(
                "/SetupContractTemplate/GetEmptyTableSetupContractTypeDefalut",
                function (res) {
                    $setupcontracttype.bindRows(res);
                    __setupcontractype = $setupcontracttype.yield();
                }
            )
        })
        dialog.reject(function () {

            dialog.shutdown();
        });
        dialog.confirm(function () {
            var setupcontractype = [];
            __setupcontractype.forEach(i => {
                if (i.ContractType != "")
                    setupcontractype.push(i);
            })

            $.ajax({
                url: "/SetupContractTemplate/InsertContractType",
                type: "POST",
                dataType: "JSON",
                data: { contractype: setupcontractype },
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
                            '<option value="' + item.ID + '">' + item.ContractType + '</option>';
                    });
                    $("#listoftype").html(data);


                }
            })
            dialog.shutdown();

        });
    })
    const remarkObj = {
        ID: 0,
        Remarks:""
    }
    const converageObj = {
        ID: 0,
        Monthday:"",
        Thuesday:"",
        Wednesday:"",
        Thursday:"",
        Friday:"",
        Saturday:"",
        Sunday:"",
        StarttimeMon:"",
        StarttimeThu:"",
        StarttimeWed:"",
        StarttimeThur:"",
        StarttimeFri:"",
        StarttimeSat:"",
        StarttimeSun:"",
        EndtimeMon:"",
        EndtimeThu:"",
        EndtimeWed:"",
        EndtimeThur:"",
        EndtimeFri:"",
        EndtimeSat:"",
        EndtimeSun: "",
        Part: "",
        Labor: "",
        Travel: "",
        Holiday: "",
    }
    var contracttemplate_master = {
         ID:0,
          Name:"",
        ContracType:0,
          ResponseTime:"",
          ResponseTimeDH:"",
          ResultionTime:"",
        ResultionTimeDH: "",
        Remarks: "",
        Expired:"",
        Description:"",
        Converage: {},
        Remark: {},
        AttachmentFiles: {}
    }
    let master = [];
    master.push(contracttemplate_master);
    function datas() {
        const contracttemplate_master = master[0];
        contracttemplate_master.ID = $("#id").val();
        contracttemplate_master.Name = $("#name").val();
        contracttemplate_master.ResponseTime = $("#responsetime").val();
        contracttemplate_master.ResultionTime = $("#resultiontime").val();
        contracttemplate_master.ResponseTimeDH = $("#listhd1").val();
        contracttemplate_master.ResultionTimeDH = $("#listhd2").val();
        contracttemplate_master.Description = $("#description").val();
        contracttemplate_master.ContracType = $("#listoftype").val();
        contracttemplate_master.Expired = $("#expired").prop("checked") ? true : false;

        converageObj.ID = $("#converid").val();
        converageObj.Part = $("#part").prop("checked") ? true : false;
        converageObj.Labor = $("#labor").prop("checked") ? true : false;
        converageObj.Travel = $("#travel").prop("checked") ? true : false;
        converageObj.Holiday = $("#holiday").prop("checked") ? true : false;

        converageObj.Monthday = $("#mon").prop("checked") ? true : false;
        converageObj.Thuesday = $("#thue").prop("checked") ? true : false;
        converageObj.Wednesday = $("#wed").prop("checked") ? true : false;
        converageObj.Thursday = $("#thur").prop("checked") ? true : false;
        converageObj.Friday = $("#fri").prop("checked") ? true : false;
        converageObj.Saturday = $("#sat").prop("checked") ? true : false;
        converageObj.Sunday = $("#sun").prop("checked") ? true : false;

        converageObj.Sunday = $("#sun").prop("checked") ? true : false;
        converageObj.Sunday = $("#sun").prop("checked") ? true : false;
        converageObj.Sunday = $("#sun").prop("checked") ? true : false;
        converageObj.Sunday = $("#sun").prop("checked") ? true : false;

        converageObj.StarttimeMon = $("#stimemon").val();
        converageObj.StarttimeThu = $("#stimethue").val();
        converageObj.StarttimeWed = $("#stimewed").val();
        converageObj.StarttimeThur = $("#stimethur").val();
        converageObj.StarttimeFri = $("#stimefri").val();
        converageObj.StarttimeSat = $("#stimesat").val();
        converageObj.StarttimeSun = $("#stimesun").val();

        converageObj.EndtimeMon = $("#etimemon").val();
        converageObj.EndtimeThu = $("#etimethue").val();
        converageObj.EndtimeWed = $("#etimewed").val();
        converageObj.EndtimeThur = $("#etimethur").val();
        converageObj.EndtimeFri = $("#etimefri").val();
        converageObj.EndtimeSat = $("#etimesat").val();
        converageObj.EndtimeSun = $("#etimesun").val();
        contracttemplate_master.Converage = converageObj;
        //=========Remark============
        remarkObj.ID = $("#remarkid").val();
        remarkObj.Remarks = $("#remark").val();
        contracttemplate_master.Remark = remarkObj;
        //==============AttchmentFile==============
        contracttemplate_master.AttachmentFiles = $list_Attachfile.yield().length == 0 ? new Array() : $list_Attachfile.yield();
        //return;
        $.ajax({
            url: "/SetupContractTemplate/SubmmitData",
            type: "POST",
            dataType: "JSON",
            data: $.antiForgeryToken({ _data: JSON.stringify(contracttemplate_master) }),
            success: function (respones) {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    },
                }, respones)
                if (respones.IsApproved) {
                    //location.reload();
                    location.href = "/SetupContractTemplate/Index" ;
                    
                }
            }
        });
    }
    $("#submit-data").click(function () {
        datas();

    })
    $("#update-data").click(function () {
        datas();

    })
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
});
