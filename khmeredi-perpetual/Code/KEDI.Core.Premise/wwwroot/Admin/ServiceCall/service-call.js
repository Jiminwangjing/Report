$(document).ready(function () {
    const __dataNo = JSON.parse($(".data").text());
    const date = new Date();
    const min = date.getMinutes().toString().length == 1 ? `0${date.getMinutes()}` : date.getMinutes();
    const time = `${date.getHours()}:${min}`;
    var offon = true;
    $(".even-list").click(function () {
        if (offon) {
            $(this).css("color", "green");
            offon = false;
        }
        else if (!status) {
            $(this).css("color", "black");
            offon = true;
        }

        //$(this).siblings("div").toggle(100).closest("li").siblings().children("div").hide();
        //$(this).children().toggleClass("fa-caret-right").addClass("fa-caret-down")
        //    .closest("li").siblings().children("span").children("i").removeClass("fa-caret-down").addClass("fa-caret-right");

    });


    const serviceCall = {
        ID: 0,
        BPID: 0,
        Name: "",
        MfrSerialNo: "",
        SerialNumber: "",
        ItemID: 0,
        ItemGroupID: 0,
        SeriesID: 0,
        SeriesDID: 0,
        DocTypeID: 0,
        Number: "",
        CallStatus: 0,
        Priority: 0,
        CreatedOnDate: "",
        CreatedOnTime: "",
        ClosedOnDate: "",
        ClosedOnTime: "",
        ContractNo: "",
        Subject: "",
    }

    $("#bpcode").click(function () {
        const dialog = new DialogBox({
            content: {
                selector: "#container-list-customers"
            },
            button: {
                ok: {
                    text: "Close"
                }
            },
            caption: "List of cusotmers",
        });
        dialog.invoke(function () {
            const customerTable = ViewTable({
                keyField: "ID",
                selector: ".list-customers",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "Phone", "Address"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
                        on: {
                            "click": function (e) {
                                serviceCall.BPID = e.data.ID;
                                $("#bpcode").val(e.data.Code);
                                $("#bpname").val(e.data.Name);
                                $("#telephoneno").val(e.data.Phone);
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            })
            $.get("/Service/GetCustomers", function (res) {
                if (res.length > 0) {
                    customerTable.bindRows(res)
                    $("#txtSearch-customer").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(res, function (item) {
                            return item.Code.toLowerCase().replace(/\s+/, "").includes(__value) || item.Name.toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.Address.toLowerCase().replace(/\s+/, "").includes(__value) || item.Phone.toLowerCase().replace(/\s+/, "").includes(__value)
                        });
                        customerTable.bindRows(items)
                    });
                }
            })
        })
        dialog.confirm(function () {
            dialog.shutdown();
        })
    })



    //$("#serialNumber").click(function () {
    //    const dialog = new DialogBox({
    //        content: {
    //            selector: "#container-list-serial"
    //        },
    //        button: {
    //            ok: {
    //                text: "Close"
    //            }
    //        },
    //        caption: "List of Serials",
    //    });
    //    dialog.invoke(function () {
    //        const customerTable = ViewTable({
    //            keyField: "LineID",
    //            selector: ".list-serial",
    //            indexed: true,
    //            paging: {
    //                pageSize: 10,
    //                enabled: true
    //            },
    //            visibleFields: ["CustomerName", "ItemCode", "ItemName", "MfrSerialNo", "Serial"],
    //            actions: [
    //                {
    //                    template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
    //                    on: {
    //                        "click": function (e) {
    //                            serviceCall.MfrSerialNo = e.data.MfrSerialNo;
    //                            serviceCall.SerialNumber = e.data.Serial;
    //                            serviceCall.ItemID = e.data.ItemID;
    //                            serviceCall.ItemGroupID = e.data.GroupItemID
    //                            $("#mfrSerialNo").val(e.data.MfrSerialNo);
    //                            $("#serialNumber").val(e.data.Serial);
    //                            $("#itemCode").val(e.data.ItemCode);
    //                            $("#itemName").val(e.data.ItemName);
    //                            $("#itemGroupName").val(e.data.ItemGroupName);
    //                            dialog.shutdown();
    //                        }
    //                    }
    //                }
    //            ]
    //        })
    //        $.get("/Service/GetSerials", { bpid: serviceCall.BPID }, function (res) {
    //            if (res.length > 0) {
    //                customerTable.bindRows(res)
    //                $("#txtSearch-customer").on("keyup", function () {
    //                    let __value = this.value.toLowerCase().replace(/\s+/, "");
    //                    let items = $.grep(res, function (item) {
    //                        return item.Code.toLowerCase().replace(/\s+/, "").includes(__value) || item.Name.toLowerCase().replace(/\s+/, "").includes(__value)
    //                            || item.Address.toLowerCase().replace(/\s+/, "").includes(__value) || item.Phone.toLowerCase().replace(/\s+/, "").includes(__value)
    //                    });
    //                    customerTable.bindRows(items)
    //                });
    //            }
    //        })
    //    })
    //    dialog.confirm(function () {
    //        dialog.shutdown();
    //    })
    //})



    //$("#createdOnDate")[0].valueAsDate = date;
    $("#createdOnTime").val(time);
    serviceCall.CreatedOnDate = $("#createdOnDate").val();
    serviceCall.CreatedOnTime = $("#createdOnTime").val();
    //invioce
    let selected = $("#no");
    selectSeries(selected);

    $('#no').change(function () {
        var id = ($(this).val());
        var seriesSC = find("ID", id, __dataNo);
        serviceCall.Number = seriesSC.NextNo;
        serviceCall.DocTypeID = seriesSC.DocumentTypeID;
        serviceCall.SeriesID = seriesSC.ID;
        $("#number").val(seriesSC.NextNo);
    });
    if (__dataNo.length == 0) {
        $('#no').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
        $("#add").prop("disabled", true);
    }

    $("#add").click(function () {
        serviceCall.CreatedOnDate = $("#createdOnDate").val();
        serviceCall.CreatedOnTime = $("#createdOnTime").val();
        serviceCall.CallStatus = $("#callStatus").val();
        serviceCall.Name = $("#name").val();
        serviceCall.BPRefNo = $("#bpRefNo").val();
        serviceCall.ClosedOnDate = $("#closedOnDate").val();
        serviceCall.ClosedOnTime = $("#closedOnTime").val();
        serviceCall.ContractNo = $("#contractNo").val();
        serviceCall.Priority = $("#priority").val();
        serviceCall.Subject = $("#subject").val();
        serviceCall.CallID = $("#txt-callid").val();
        serviceCall.Resolution = $("#txt_resolution").val();
        serviceCall.ChannelID = $("#slchn").val();
        serviceCall.HandledByID = $("#handleid").val();
        serviceCall.TechnicianID = $("#tectnicianid").val();
        serviceCall.ServiceDatas = servicedata;

        $.post("/Service/UpdateServiceCall", { serviceCalls: JSON.stringify(serviceCall) }, function (res) {
            new ViewMessage({
                summary: {
                    selector: ".message"
                }
            }, res)
            if (res.IsApproved) {
                location.reload()
            }
        })
    })
    $("#cancel").click(function () {
        location.reload();
    })
    function selectSeries(selected) {
        $.each(__dataNo, function (i, item) {

            if (item.Default == true) {
                $("<option selected value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
                $("#number").val(item.NextNo);
                serviceCall.Number = item.NextNo;
                serviceCall.DocTypeID = item.DocumentTypeID;
                serviceCall.SeriesID = item.ID;
            }
            else {
                $("<option value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
            }
        });
        return selected.on('change')
    }

    $.get("/Service/GetCallID", function (number) {
        $("#txt-callid").val(number);

    });
    //Create Channel
    var id = 0;
    $("#create-channel").click(function () {

        const dialogchannel = new DialogBox({
            button: {
                ok: {
                    text: "SAVE",
                },
            },
            type: "ok-cancel",
            caption: "List Project Cost Analysis",
            content: {
                selector: "#channel-content",
            }
        });
        dialogchannel.confirm(function () {

            let obj = {};
            obj.ID = id;
            obj.Name = $("#txt_name").val();
            $.post("/Service/CreateUpdate", { data: obj }, function (data) {
                id = 0;
                $("#txt_name").val("");
                Channel(data.Data, dialogchannel);
                let addOption = "<option  value='0'>--- Select ---</option>";
                $.each(data.Data, function (i, item) {
                    addOption +=
                        '<option value="' + item.ID + '">' + item.Name + '</option>';
                });
                $("#slchn").html(addOption);


                if (data.IsApproved) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summarys"
                        }
                    }, data.Model);

                }
                new ViewMessage({
                    summary: {
                        selector: "#error-summarys"
                    }
                }, data.Model);
                $(window).scrollTop(0);


            });
        });
        dialogchannel.invoke(function () {

            $("#txt_name").val("");
            $.get("/Service/GetChannel", function (res) {
                Channel(res, dialogchannel);
            });
        });
        dialogchannel.reject(function () {
            $("#txt_name").val("");
            dialogchannel.shutdown();
        });

    });
    $.get("/Service/GetChannel", function (res) {
        if (res.length > 0) {
            let addOption = "<option value='0'>--- Select ---</option>";
            $.each(res, function (i, item) {
                addOption +=
                    '<option value="' + item.ID + '">' + item.Name + '</option>';
            });
            $("#slchn").html(addOption);
        }
    });
    function Channel(data, dialog) {
        const $list_channel = ViewTable({
            keyField: "ID",
            selector: "#list_channel",
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: true
            },
            visibleFields: ["Name"],

            actions: [
                {
                    template: `<i class="fas fa-edit"></i>`,
                    on: {
                        "click": function (e) {
                            $("#txt_name").val(e.data.Name);
                            id = e.data.ID;

                        }
                    }
                },
                {
                    name: "",
                    template: `<i class="fas fa-arrow-circle-down"></i>`,
                    on: {
                        "click": function (e) {

                            $("#txt_name").val("");
                            dialog.shutdown();
                        }
                    }
                },
            ],


        })
        if (data.length > 0) {
            $list_channel.clearRows();
            $list_channel.bindRows(data);
        }

        $("#txtsearchativity").on("keyup", function (e) {
            var value = $(this).val().toLowerCase();
            $("#list_channel tr:not(:first-child)").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
            });
        });
    }
    //// Find Service Call ////
    $("#History").click(() => {
        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: "List Service Call",
            content: {
                selector: "#service-content"
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown();
        });
        dialogprojmana.invoke(function () {
            /// Bind Customers /// 
            $("#txtSearch-serial").on("keyup", function (e) {
                var value = $(this).val().toLowerCase();
                $("#list-servicecall tr:not(:first-child)").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
            const $listservice = ViewTable({
                keyField: "ID",
                selector: "#list-servicecall",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: [
                    "CallID",
                    "Number",
                    "BPCode",
                    "BName",
                    "Subject",
                    "Priority",
                    "CallStatus", "Handleby", "Technician", "CreatedOnDate", "CreatedOnTime",
                ],

                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                $.get("/Service/FindServiceCall", { number: e.data.Number, seriesid: e.data.SeriesID }, function (res) {
                                    SearchData(res)
                                })

                                dialogprojmana.shutdown();
                            }
                        }
                    }
                ]
            });
            GetServiceCall($listservice);
        });

    })

    function GetServiceCall(table) {
        $.get("/Service/GetServiceCall", function (data) {
            if (data.length > 0)
                table.bindRows(data);
        })
    }


    $("#btn-find").on("click", function () {
        $("#btn-addnew").prop("hidden", false);
        $("#btn-find").prop("hidden", true);
        $("#number").val("").prop("readonly", false).focus();
    });
    $("#number").on("keypress", function (e) {

        if (e.which === 13) {
            $.ajax({
                url: "/Service/FindServiceCall",
                data: { number: $(this).val().trim(), seriesid: $("#no").val() },
                success: function (result) {
                    SearchData(result);
                }
            });
        }
    });
    function SearchData(result) {

        servicedata = [];
        if (result.ID == 0) {
            new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Service Call Not found!",
                close_button: "none"
            });
        }
        else {
            serviceCall.ID = result.ID;
            serviceCall.BPID = result.BPID;
            serviceCall.Name = result.Name;
            serviceCall.MfrSerialNo = result.MfrSerialNo;
            serviceCall.SerialNumber = result.SerialNumber;
            serviceCall.ItemID = result.ItemID;
            serviceCall.ItemGroupID = result.ItemGroupID;
            serviceCall.SeriesID = result.SeriesID;
            serviceCall.SeriesDID = result.SeriesDID;
            serviceCall.DocTypeID = result.DocTypeID;
            serviceCall.Number = result.Number;
            serviceCall.CallStatus = result.CallStatus;
            serviceCall.Priority = result.Priority;
            serviceCall.CreatedOnDate = result.CreatedOnDate;
            serviceCall.CreatedOnTime = result.CreatedOnTime;
            serviceCall.ClosedOnDate = result.ClosedOnDate;
            serviceCall.ClosedOnTime = result.ClosedOnTime;
            serviceCall.ContractNo = result.ContractNo;
            if (result.ServiceDatas.length > 0) {
                result.ServiceDatas.forEach(i => {
                    servicedata.push(i);
                    $list_servicedata.bindRows(servicedata);
                })
            }
            if (result.ChannelID != 0)
                $("#slchn").val(result.ChannelID);
            $("#tectnicianid").val(result.TechnicianID);
            $("#handleid").val(result.HandledByID);
            $("#txt_handleby").val(result.HandleName);
            $("#txt_tectnician").val(result.TectnicalName);
            if (result.CallStatus == 2) {
                $("#createdOnDate").prop("disabled", true);
                $("#createdOnTime").prop("disabled", true);
                $("#closedOnDate").prop("disabled", false);
                $("#closedOnTime").prop("disabled", false);
                $("#slchn").prop("disabled", true);
            }
            $("#name").val(result.Name);
            $("#bpcode").val(result.BCode);
            $("#bpname").val(result.BName);
            $("#telephoneno").val(result.BPhone);
            $("#bpRefNo").val(result.BPRefNo);
            $("#mfrSerialNo").val(result.MfrSerialNo);
            $("#serialNumber").val(result.SerialNumber);
            $("#itemCode").val(result.ItemCode);
            $("#itemName").val(result.ItemName);
            $("#itemGroupName").val(result.GName);
            $("#no").val(result.SeriesID);
            $("#number").val(result.Number);
            $("#callStatus").val(result.CallStatus);
            $("#txt-callid").val(result.CallID);
            $("#priority").val(result.Priority);

            $("#createdOnTime").val(result.CreatedOnTime);
            if (result.ClosedOnDate != null)
                setDate("#closedOnDate", result.ClosedOnDate.toString().split("T")[0]);
            setDate("#createdOnDate", result.CreatedOnDate.toString().split("T")[0]);
            if (result.EndDate != null)
                setDate("#endDate", result.EndDate.toString().split("T")[0]);

            $("#closedOnTime").val(result.ClosedOnTime);
            $("#contractNo").val(result.ContractNo);
            $("#subject").val(result.Subject);
            $("#txt_resolution").val(result.Resolution);
            $("#add").html("Update");
        }


    }
    $("#callStatus").change(function () {

        if (this.value == 2) {
            $("#createdOnDate").prop("disabled", true);
            $("#createdOnTime").prop("disabled", true);
            $("#closedOnDate").prop("disabled", false);
            $("#closedOnTime").prop("disabled", false);
            $("#slchn").prop("disabled", true);
        }
        else {
            $("#createdOnDate").prop("disabled", false);
            $("#createdOnTime").prop("disabled", false);
            $("#closedOnDate").prop("disabled", true);
            $("#closedOnTime").prop("disabled", true);
            $("#slchn").prop("disabled", false);
        }

    })
    // employee
    $("#txt_handleby").click(function () {
        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                },
            },
            // type: "ok-cancel",
            caption: "List Employee",
            content: {
                selector: "#em-content",
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown()
        });
        dialogprojmana.invoke(function () {
            $("#txtsearchsaleem").on("keyup", function (e) {
                var value = $(this).val().toLowerCase();
                $("#list_em tr:not(:first-child)").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
            const $list_em = ViewTable({
                keyField: "ID",
                selector: "#list_em",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "GenderDisplay", "Position", "Address", "Phone", "Email", "EMType"],

                actions: [
                    {
                        name: "",
                        template: `<i class="fas fa-arrow-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                $("#handleid").val(e.data.ID);
                                $("#txt_handleby").val(e.data.Name);
                                dialogprojmana.shutdown();

                            }
                        }
                    },
                ],
            })
            SaleEM($list_em);
        });
        dialogprojmana.reject(function () {

            dialogprojmana.shutdown();
        });
    })
    $("#txt_tectnician").click(function () {

        // DialogEmployee("#tectnicianid", "");

        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                },
            },
            // type: "ok-cancel",
            caption: "List Employee",
            content: {
                selector: "#em-content",
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown()
        });
        dialogprojmana.invoke(function () {
            $("#txtsearchsaleem").on("keyup", function (e) {
                var value = $(this).val().toLowerCase();
                $("#list_em tr:not(:first-child)").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
            const $list_em = ViewTable({
                keyField: "ID",
                selector: "#list_em",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "GenderDisplay", "Position", "Address", "Phone", "Email", "EMType"],

                actions: [
                    {
                        name: "",
                        template: `<i class="fas fa-arrow-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                $("#tectnicianid").val(e.data.ID);
                                $("#txt_tectnician").val(e.data.Name);
                                dialogprojmana.shutdown();

                            }
                        }
                    },
                ],
            })
            SaleEM($list_em);
        });
        dialogprojmana.reject(function () {

            dialogprojmana.shutdown();
        });
    });


    function SaleEM(table) {
        $.get("/Service/GetSaleEmployee", function (res) {
            table.bindRows(res);
        });
    }


    // date format
    $("input[type='date']").on("change", function () {
        this.setAttribute("data-date", moment(this.value, "YYYY-MM-DD")
            .format(this.getAttribute("data-date-format"))
        )
    });
    function updateData(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue) {
                    i[prop] = propValue;
                }
            })
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
    function setDate(selector, date_value) {
        var _date = $(selector);
        _date[0].valueAsDate = new Date(date_value);
        _date[0].setAttribute("data-date", moment(_date[0].value).format(_date[0].getAttribute("data-date-format"))
        );
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function find(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
    let servicedata = [];

    let statusop = true;
    function FunServiceItem(arr) {

        const $list_serviceitem = ViewTable({
            keyField: "LineID",
            selector: "#list-serviceitem",
            indexed: true,
            paging: {
                pageSize: 15,
                enabled: true
            },
            visibleFields: ["Activitys", "HandledByName", "TechnicianName", "StartDate", "StartTime", "EndDate", "EndTime", "Completed", "Finnish", "Acitivity", "ActivityName", "Remark"],
            columns:
                [
                    {
                        name: "Activitys",
                        template: `<select></select>`,
                        on: {
                            "change": function (e) {

                                updateData(arr, "LineID", e.key, "ActivityID", this.value);
                                updateSelect(e.data.Activitys, this.value, "Selected");
                            }
                        }
                    },
                    {
                        name: "HandledByName",
                        template: ` <input/>`,
                        on: {
                            "click": function (e) {
                                DialogEmployee($list_serviceitem, e, "HandledByName", arr, "HandledByID", 1);
                                //  updateData(arr, "LineID", e.key, "HandledByName", this.value);
                            }
                        }
                    },
                    {
                        name: "TechnicianName",
                        template: ` <input/>`,
                        on: {
                            "click": function (e) {
                                DialogEmployee($list_serviceitem, e, "TechnicianName", arr, "TechnicianID", 2);
                                // updateData(objStage, "LineID", e.key, "StartDate", this.value);
                            }
                        }
                    },
                    {
                        name: "StartDate",
                        template: ` <input type="date" class="datetime" />`,
                        on: {
                            "change": function (e) {

                                updateData(arr, "LineID", e.key, "StartDate", this.value);
                            }
                        }
                    },
                    {
                        name: "StartTime",
                        template: ` <input type="time"/>`,
                        on: {
                            "change": function (e) {

                                updateData(arr, "LineID", e.key, "StartTime", this.value);
                            }
                        }
                    },
                    {
                        name: "EndDate",
                        template: ` <input type="date" />`,
                        on: {
                            "change": function (e) {

                                updateData(arr, "LineID", e.key, "EndDate", this.value);
                            }
                        }
                    },
                    {
                        name: "EndTime",
                        template: ` <input type="time"/>`,
                        on: {
                            "change": function (e) {

                                updateData(arr, "LineID", e.key, "EndTime", this.value);
                            }
                        }
                    },
                    {
                        name: "Completed",
                        template: ` <input/>`,
                        on: {
                            "keyup": function (e) {
                                $(this).asNumber();
                                let value = parseFloat(this.value);
                                if (value >= 100) {
                                    value = 100;
                                    $list_serviceitem.updateColumn(e.key, "Finnish", true);
                                }
                                else
                                    $list_serviceitem.updateColumn(e.key, "Finnish", false);
                                Fun_Complete(e, this);
                                updateData(arr, "LineID", e.key, "Completed", value);
                            }
                        }
                    },
                    {
                        name: "Finnish",
                        template: ` <input type="checkbox"/>`,
                        on: {
                            "change": function (e) {

                                const isChecked = $(this).prop("checked") ? true : false;
                                if (isChecked) {
                                    $list_serviceitem.updateColumn(e.key, "Completed", 100);
                                    updateData(arr, "LineID", e.key, "Completed", 100);
                                }
                                else {
                                    $list_serviceitem.updateColumn(e.key, "Completed", 0);
                                    updateData(arr, "LineID", e.key, "Completed", 0);
                                }
                                updateData(arr, "LineID", e.key, "Finnish", isChecked);
                                $list_serviceitem.updateColumn(e.key, "Finnish", isChecked);
                            }
                        }
                    },
                    {
                        name: "Acitivity",
                        template: ` <input type="checkbox"/>`,
                        on: {
                            "change": function (e) {
                                const isChecked = $(this).prop("checked") ? true : false;
                                updateData(arr, "LineID", e.key, "Acitivity", isChecked);
                                $list_serviceitem.updateColumn(e.key, "Acitivity", isChecked);

                            }
                        }
                    },
                    {
                        name: "ActivityName",
                        template: ` <input />`,
                        on: {
                            "click": function (e) {

                                DialogActivity($list_serviceitem, e, "ActivityName", arr, "LinkActivytyID");
                            }
                        }
                    },
                    {
                        name: "Remark",
                        template: ` <input/>`,
                        on: {
                            "keyup": function (e) {

                                updateData(arr, "LineID", e.key, "Remark", this.value);
                            }
                        }
                    },
                ],
            //actions: [
            //    {
            //        template: `<i class="fas fa-arrow-circle-down"></i>`,
            //        on: {
            //            "click": function (e) {

            //                // $list_serviceitem.bindRows();

            //            }
            //        }
            //    },
            //],
        });

        $list_serviceitem.bindRows(arr);
    }
    function Fun_Complete(e, _this) {
        if (parseFloat(_this.value) > 100) _this.value = 100;
        if (parseFloat(_this.value) < 0) _this.value = 0;
        if (parseFloat(_this.value) > 0) {
            if (_this.value.toString().includes(".")) {
                _this.value = _this.value;
            }
        }

        updatedetail(servicedata.ServiceItems, e.data.LineID, e.key, "Completed", _this.value);

        // $listItemChoosed.updateColumn(e.key, "DisValue", num.formatSpecial(num.toNumberSpecial(disvalue), disSetting.Prices));

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
    const $list_servicedata = ViewTable({
        keyField: "LineMTID",
        selector: "#list-servicedata",
        indexed: true,
        paging: {
            pageSize: 20,
            enabled: true
        },
        visibleFields: ["ItemCode", "ItemName", "MfrSerialNo", "SerialNo", "PlateNumber", "Qty"],
        columns:
            [
                {
                    name: "ItemCode",
                    template: `<input readonly/>`,
                    on: {
                        "click": function (e) {
                            ModalEmployee($list_servicedata, e, "ItemCode", servicedata, "ItemID");
                            // updateData(objStage, "LineID", e.key, "StartDate", this.value);
                        }
                    }
                },
                {
                    name: "MfrSerialNo",
                    template: ` <input/>`,
                    on: {
                        "click": function (e) {

                            DialogSerial($list_servicedata, e);
                            // DialogActivity($list_serviceitem, e, "ActivityName", arr, "LinkActivytyID");
                            updateData(servicedata, "LineMTID", e.key, "MfrSerialNo", this.value);
                        }
                    }
                },
                {
                    name: "SerialNo",
                    template: ` <input/>`,
                    on: {
                        "click": function (e) {

                            DialogSerial($list_servicedata, e);
                            // DialogActivity($list_serviceitem, e, "ActivityName", arr, "LinkActivytyID");
                            updateData(servicedata, "LineMTID", e.key, "SerialNo", this.value);
                        }
                    }
                },
                {
                    name: "PlateNumber",
                    template: ` <input/>`,
                    on: {
                        "click": function (e) {

                            DialogSerial($list_servicedata, e);
                            // DialogActivity($list_serviceitem, e, "ActivityName", arr, "LinkActivytyID");
                            updateData(servicedata, "LineMTID", e.key, "PlateNumber", this.value);
                        }
                    }
                },
                {
                    name: "Qty",
                    template: ` <input/>`,
                    on: {
                        "keyup": function (e) {
                            $(this).asNumber();
                            updateData(servicedata, "LineMTID", e.key, "Qty", this.value);
                        }
                    }
                },
            ],
        actions: [
            {
                name: "",
                template: `<i class="fas fa-arrow-circle-down" style="padding-left:50% !important"></i>`,
                on: {
                    "click": function (e) {

                        if (statusop) {

                            statusop = false;
                            FunServiceItem(e.data.ServiceItems);
                        }
                        else if (!statusop) {

                            statusop = true;
                            FunServiceItem(e.data.ServiceItems);
                        }

                    }
                }
            },
            {
                name: "",
                template: `<i class="fas fa-trash" style="padding-left:50% !important colr:red;"></i>`,

                on: {
                    "click": function (e) {
                        let props = Object.getOwnPropertyNames(e.data);
                        let rowObj = e.data;
                        for (let i = 0; i < props.length; i++) {
                            if (props[i] == "LineMTID") { continue; }
                            switch (typeof rowObj[props[i]]) {
                                case "string":
                                    $list_servicedata.updateColumn(e.key, props[i], "");
                                    break;
                                case "number":
                                    $list_servicedata.updateColumn(e.key, props[i], 0);
                                    break;
                            }
                            updateData(servicedata, "LineMTID", e.key, "ItemCode", "");
                            updateData(servicedata, "LineMTID", e.key, "ItemName", "");
                            updateData(servicedata, "LineMTID", e.key, "ServiceCallID", 0);
                            updateData(servicedata, "LineMTID", e.key, "ItemID", 0);
                            updateData(servicedata, "LineMTID", e.key, "MfrSerialNo", "");
                            updateData(servicedata, "LineMTID", e.key, "SerialNo", "");
                            updateData(servicedata, "LineMTID", e.key, "PlateNumber", " ");
                            updateData(servicedata, "LineMTID", e.key, "Qty", 0);

                            let arr_ServiceItem = [];

                            FunServiceItem(arr_ServiceItem);

                            e.data.ServiceItems.forEach(i => {
                                i.Acitivity = false;
                                i.ActivityID = 0;
                                i.ActivityName = "";
                                i.Completed = 0;
                                i.EndDate = "";
                                i.EndTime = "";
                                i.Finnish = false;
                                i.HandledByID = 0;
                                i.HandledByName = "";
                                i.LinkActivytyID = 0;
                                i.Remark = "";
                                i.StartDate = "";
                                i.StartTime = "";
                                i.TechnicianID = 0;
                                i.TechnicianName = "";
                                i.Activitys.forEach(x => {
                                    x.selected = false;
                                })


                            })

                        }
                    }
                }
            },
        ],
    })
    $.get("/Service/CreateDefualtRows", function (res) {

        if (res.length > 0) {
            res.forEach(i => {
                servicedata.push(i);
            })
        }

        $list_servicedata.bindRows(servicedata);
    })

    // dialog Serail Number=
    function DialogSerial(tableName, _e,) {

        const dialog = new DialogBox({
            content: {
                selector: "#container-list-serial"
            },
            button: {
                ok: {
                    text: "Close"
                }
            },
            caption: "List of Serials",
        });
        dialog.invoke(function () {
            const customerTable = ViewTable({
                keyField: "LineID",
                selector: ".list-serial",
                indexed: true,
                paging: {
                    pageSize: 20,

                    enabled: true
                },
                visibleFields: ["ItemCode", "ItemName", "MfrSerialNo", "Serial", "PlateNumber", "CustomerName"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down hover"></i>`,
                        on: {
                            "click": function (e) {

                                tableName.updateColumn(_e.key, "MfrSerialNo", e.data.MfrSerialNo);
                                tableName.updateColumn(_e.key, "SerialNo", e.data.Serial);
                                tableName.updateColumn(_e.key, "PlateNumber", e.data.PlateNumber);
                                tableName.updateColumn(_e.key, "Qty", 1);
                                updateData(servicedata, "LineMTID", e.key, "MfrSerialNo", e.data.MfrSerialNo);
                                updateData(servicedata, "LineMTID", e.key, "SerialNo", e.data.Serial);
                                updateData(servicedata, "LineMTID", e.key, "PlateNumber", e.data.PlateNumber);
                                updateData(servicedata, "LineMTID", e.key, "Qty", 1);
                                // tableName.updateColumn("") e.data.MfrSerialNo;
                                //tableName.SerialNumber = e.data.Serial;
                                //serviceCall.ItemID = e.data.ItemID;
                                //serviceCall.ItemGroupID = e.data.GroupItemID
                                //$("#mfrSerialNo").val(e.data.MfrSerialNo);
                                //$("#serialNumber").val(e.data.Serial);
                                //$("#itemCode").val(e.data.ItemCode);
                                //$("#itemName").val(e.data.ItemName);
                                //$("#itemGroupName").val(e.data.ItemGroupName);
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            })
            $.get("/Service/GetSerials", { bpid: serviceCall.BPID }, function (res) {
                if (res.length > 0) {
                    customerTable.bindRows(res)
                    $("#txtSearch-customer").on("keyup", function () {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let items = $.grep(res, function (item) {
                            return item.Code.toLowerCase().replace(/\s+/, "").includes(__value) || item.Name.toLowerCase().replace(/\s+/, "").includes(__value)
                                || item.Address.toLowerCase().replace(/\s+/, "").includes(__value) || item.Phone.toLowerCase().replace(/\s+/, "").includes(__value)
                        });
                        customerTable.bindRows(items)
                    });
                }
            })
        })
        dialog.confirm(function () {
            dialog.shutdown();
        })

    }




    // dialog Activyty
    function DialogActivity(tableName, _e, name, arr, feild, condition) {
        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                },
            },
            // type: "ok-cancel",
            caption: "List Activity",
            content: {
                selector: "#container-list-actyvity",
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown()
        });
        dialogprojmana.invoke(function () {
            $("#txtSearch-customer").on("keyup", function (e) {
                var value = $(this).val().toLowerCase();
                $("#list-activity tr:not(:first-child)").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
            const $list_activity = ViewTable({
                keyField: "ID",
                selector: "#list-activity",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["ActivityType", "Activity", "Subject", "BPName"],
                actions: [
                    {
                        name: "",
                        template: `<i class="fas fa-arrow-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                tableName.updateColumn(_e.key, "LinkActivytyID", e.data.ID);
                                tableName.updateColumn(_e.key, name, e.data.Activity);

                                updateData(arr, "LineID", _e.key, feild, e.data.ID);

                                dialogprojmana.shutdown();

                            }
                        }
                    },
                ],
            })
            $.get("/Service/GetActivity", function (res) {
                $list_activity.bindRows(res);
            });

        });
        dialogprojmana.reject(function () {

            dialogprojmana.shutdown();
        });
    }
    // function ModalEmployee()
    function DialogEmployee(tableName, _e, name, arr, feild, condition) {
        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                },
            },
            // type: "ok-cancel",
            caption: "List Employee",
            content: {
                selector: "#em-content",
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown()
        });
        dialogprojmana.invoke(function () {
            $("#txtsearchsaleem").on("keyup", function (e) {
                var value = $(this).val().toLowerCase();
                $("#list_em tr:not(:first-child)").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
            const $list_em = ViewTable({
                keyField: "ID",
                selector: "#list_em",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "GenderDisplay", "Position", "Address", "Phone", "Email", "EMType"],

                actions: [
                    {
                        name: "",
                        template: `<i class="fas fa-arrow-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                if (condition == 1) {
                                    tableName.updateColumn(_e.key, "HandledByID", e.data.ID);
                                    tableName.updateColumn(_e.key, name, e.data.Name);

                                    updateData(arr, "LineID", _e.key, feild, e.data.ID);
                                }
                                else if (condition == 2) {
                                    tableName.updateColumn(_e.key, "TechnicianID", e.data.ID);
                                    tableName.updateColumn(_e.key, name, e.data.Name);

                                    updateData(arr, "LineID", _e.key, feild, e.data.ID);
                                }


                                dialogprojmana.shutdown();

                            }
                        }
                    },
                ],
            })
            SaleEM($list_em);
        });
        dialogprojmana.reject(function () {

            dialogprojmana.shutdown();
        });
    }


    function ModalEmployee(tableName, _e, name, arr, feild) {

        const dialogCus = new DialogBox(
            {

                button: {
                    ok: {
                        text: "Close",
                    }
                },
                caption: "List Item Master Data",
                content:
                {
                    selector: "#itemmaster-content"
                }
            });
        dialogCus.confirm(function () {
            dialogCus.shutdown();
        });
        dialogCus.invoke(function () {
            $("#txtsearchsaleem").on("keyup", function (e) {

                var value = $(this).val().toLowerCase();

                $("#list_item tr:not(:first-child)").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
            const $listEmployee = ViewTable({
                keyField: "ID",
                selector: "#list_item",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["ItemCode", "ItemName", "Unitprice", "Stock"],
                columns:
                    [
                        {
                            name: "Birthdate",
                            template: "",
                            dataType: "date",
                            dataFormat: "MM-DD-YYYY",
                            on: {
                                "change": function (e) {

                                }
                            }
                        },
                    ],

                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {

                                tableName.updateColumn(_e.key, feild, e.data.ID);
                                tableName.updateColumn(_e.key, "ItemCode", e.data.ItemCode);
                                tableName.updateColumn(_e.key, "ItemName", e.data.ItemName);

                                updateData(arr, "LineMTID", _e.key, feild, e.data.ID);
                                updateData(arr, "LineMTID", _e.key, "ItemCode", e.data.ItemCode);
                                updateData(arr, "LineMTID", _e.key, "ItemName", e.data.ItemName);

                                dialogCus.shutdown();
                            }
                        }
                    }
                ]
            });
            $.get("/Service/GetItemmasterData", function (employees) {
                //employees.forEach(i => {
                //     i.Birthdate = formatDate(i.Birthdate, "YYYY-MM-DD");
                //     i.Hiredate = formatDate(i.Hiredate, "YYYY-MM-DD");

                //})
                $listEmployee.bindRows(employees);
            });

        });
    }


});