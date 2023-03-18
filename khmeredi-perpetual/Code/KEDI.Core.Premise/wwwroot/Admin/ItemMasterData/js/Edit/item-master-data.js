let _data = JSON.parse($("#model-data").text());
let _tgPur = JSON.parse($(".tgPur").text());
let _tgSale = JSON.parse($(".tgSale").text());
let config = {
    master: {
        keyField: "WarehouseID",
        value: _data.ItemMasterData
    },
    detail: {
        keyField: "WarehouseID",
        name: "ItemAccountings",
        values: _data.ItemAccountings
    },
    properties: {
        keyField: "UnitID",
        name: "",
        values: _data.PropertyDetails
    }
};
const converageObj = {
    ID: 0,
    Monthday: "",
    Thuesday: "",
    Wednesday: "",
    Thursday: "",
    Friday: "",
    Saturday: "",
    Sunday: "",
    StarttimeMon: "",
    StarttimeThu: "",
    StarttimeWed: "",
    StarttimeThur: "",
    StarttimeFri: "",
    StarttimeSat: "",
    StarttimeSun: "",
    EndtimeMon: "",
    EndtimeThu: "",
    EndtimeWed: "",
    EndtimeThur: "",
    EndtimeFri: "",
    EndtimeSat: "",
    EndtimeSun: "",
    Part: "",
    Labor: "",
    Travel: "",
    Holiday: "",
};
var contracttemplate_master = {
    ID: 0,
    Name: "",
    ContracType: 0,
    ResponseTime: "",
    ResponseTimeDH: "",
    ResultionTime: "",
    ResultionTimeDH: "",
    Remarks: "",
    Expired: "",
    Description: "",
    Converage: {},
    Remark: {},
    AttachmentFiles: {}
};
const remarkObj = {
    ID: 0,
    Remarks: ""
};
let master = [];
master.push(contracttemplate_master);

let coreMaster = new CoreItemMaster(config);
var ID1 = 0;
var ID2 = 0;

$(document).ready(function () {
    let $listproperties = ViewTable({
        keyField: "UnitID",
        selector: ".prop-tb",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: false
        },
        visibleFields: ["NameProp", "Values"],
        columns: [
            {
                name: "Values",
                template: "<select class='input-box-kernel' style='width:90%;'></select>",
                //mapped: true
                on: {
                    "change": function (e) {
                        coreMaster.updateProperty(e.data.UnitID, "Value", parseInt(this.value));
                    }
                }
            },
        ],
        actions: [
            {
                template: `<i class="fas fa-plus-circle ml-2"></i>`,
                on: {
                    "click": function (e) {
                        const childProps = {
                            Disabled: false,
                            Group: null,
                            Selected: false,
                            Text: "",
                            Value: "",
                        }
                        let dialog = new DialogBox({
                            content: {
                                selector: "#cc-property-content"
                            },
                            caption: "Create Children Property Detail",
                            type: "ok/cancel",
                            button: {
                                ok: {
                                    text: "Save",
                                    callback: function () {
                                        const _this = this;
                                        const Name = $("#nameCCP").val();
                                        const data = { ProID: e.data.ProID, Name }
                                        $.post("/Property/CreateCPD", { childPreoperty: data }, function (res) {
                                            const msg = new ViewMessage({
                                                summary: {
                                                    selector: ".err-success-summery"
                                                }
                                            });
                                            if (res.IsApproved) {
                                                msg.bind(res);
                                                setTimeout(function () { _this.meta.shutdown() }, 700);
                                                $.get("/Property/GetLastestCCP", { PropID: e.data.ProID }, function (ccp) {
                                                    for (var i of e.data.Values) i.Selected = false;
                                                    childProps.Selected = true;
                                                    childProps.Text = ccp.Name;
                                                    childProps.Value = ccp.ID;
                                                    e.data.Values.push(childProps);
                                                    e.data.Value = ccp.ID;
                                                    $listproperties.updateColumn(e.key, "Values", e.data.Values);
                                                    coreMaster.updateProperty(e.data.UnitID, "Value", ccp.ID);
                                                })
                                            }
                                            if (res.IsRejected) {
                                                msg.bind(res);
                                            }
                                        })
                                    }
                                }
                            }
                        })
                        //dialog.invoke(function () { })
                        dialog.reject(function () { dialog.shutdown() })
                    }
                }
            }]
    });
    $listproperties.clearRows();
    $listproperties.bindRows(_data.PropertyDetails);
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
    /// Bind Sales Data and Purchasing Data ///
    let opPur = '';
    let opSale = '';
    _tgPur.forEach(i => {
        if (_data.ItemMasterData.TaxGroupPurID == i.ID) {
            opPur += `<option value='${i.ID}' selected>${i.Code} - ${i.Name}</option>`;
            $("#purTgRate").val(i.Rate)
        } else
            opPur += `<option value='${i.ID}'>${i.Code} - ${i.Name}</option>`;
    })
    _tgSale.forEach(i => {
        if (_data.ItemMasterData.TaxGroupSaleID == i.ID) {
            opSale += `<option value='${i.ID}' selected >${i.Code} - ${i.Name}</option>`;
            $("#saleTgRate").val(i.Rate);
        } else
            opSale += `<option value='${i.ID}'>${i.Code} - ${i.Name}</option>`;

    })
    $("#txtTaxGroupPurId").append(opPur).change(function () {
        const tagGroup = findArray("ID", this.value, _tgPur);
        if (typeof tagGroup !== 'undefined')
            $("#purTgRate").val(tagGroup.Rate)
        else
            $("#purTgRate").val('')
    });
    $("#txtTaxGroupSaleId").append(opSale).change(function () {
        const tagGroup = findArray("ID", this.value, _tgSale);
        if (typeof tagGroup !== 'undefined')
            $("#saleTgRate").val(tagGroup.Rate)
        else
            $("#saleTgRate").val('')
    })
    let thName;
    //Get name Accounting Item
    $.ajax({
        url: "/ItemGroup1/Getname",
        type: "get",
        async: false,
        dataType: "JSON",
        success: function (res) {
            thName = res;
            let data = '';
            $.each(res, function (i, item) {
                data += `
                        <td class="p-1 hideTH" style="font-size:12px;">${item}</td>    
                    `;
            })
            $("#thAcc").append(data);
        }
    })
    $.get("/ItemMasterData/GetContractType", function (res) {
        var data = "<option>--- select ---</option>";
        res.forEach(i => {
            data += `<option value='${i.ID}'>${i.ContractType}</option>`;
        });
        $("#contract-type").append(data);

    })


    $("#btn-update").on("click", function () {
        $(this).prop("disabled", true);
        coreMaster.updateMaster("InventoryUoMID", $("#txtInventoryUOM").val());
        coreMaster.updateMaster("SaleUomID", $("#txtSaleUOM").val());
        coreMaster.updateMaster("PurchaseUomID", $("#txtPurchar").val());
        coreMaster.updateMaster("PrintToID", $("#txtprinter").val());
        coreMaster.updateMaster("WarehouseID", $("#txtselectwarehouse").val());
        coreMaster.updateMaster("PriceListID", $("#txtselectprice").val());
        coreMaster.updateMaster("Type", $("#type").val());
        coreMaster.updateMaster("Process", $("#txtProccess").val());
        coreMaster.updateMaster("Barcode", $("#txtbarcode").val());
        coreMaster.updateMaster("KhmerName", $("#khmerName").val());
        coreMaster.updateMaster("EnglishName", $("#englishName").val());
        coreMaster.updateMaster("Code", $("#itemCode").val());
        // coreMaster.updateMaster("Barcode", $("#barcode").val());
        coreMaster.updateMaster("KhmerName", $("#khmerName").val());
        coreMaster.updateMaster("EnglishName", $("#englishName").val());
        coreMaster.updateMaster("Cost", $("#txtCost").val());
        coreMaster.updateMaster("UnitPrice", $("#txtUnitPrice").val());
        coreMaster.updateMaster("TaxGroupSaleID", $("#txtTaxGroupSaleId").val());
        coreMaster.updateMaster("TaxGroupPurID", $("#txtTaxGroupPurId").val());
        coreMaster.updateMaster("ItemGroup1ID", $("#selectItemGroup1").val());
        coreMaster.updateMaster("ManItemBy", $("#manItemBy").val());
        coreMaster.updateMaster("ManMethod", $("#manMethod").val());
        coreMaster.updateMaster("MinOrderQty", $("#minOrderQty").val());
        coreMaster.updateMaster("MaxOrderQty", $("#maxOrderQty").val());
        coreMaster.updateMaster("IsLimitOrder", $("#isLimitOrder").prop("checked") ? true : false);
        coreMaster.updateMaster("Inventory", $("#invcheckbox").prop("checked") ? true : false);
        coreMaster.updateMaster("Sale", $("#salecheckbox").prop("checked") ? true : false);
        coreMaster.updateMaster("Scale", $("#scale").prop("checked") ? true : false);
        // coreMaster.updateMaster("AIAJ", $("#adv_inventory_ajustment").prop("checked") ? true : false);
        coreMaster.updateMaster("Purchase", $("#purcheckbox").prop("checked") ? true : false);
        coreMaster.updateMaster("Description", $("#itemDes").val());
        for (let i = 0; i < _data.ItemAccountings.length; i++) {
            coreMaster.updateDetail(_data.ItemAccountings[i].WarehouseID, $(`input[name='min${i}']`).data("prop"), $(`input[name='min${i}']`).val());
            coreMaster.updateDetail(_data.ItemAccountings[i].WarehouseID, $(`input[name='max${i}']`).data("prop"), $(`input[name='max${i}']`).val());
        }
        var formData = new FormData($("#formId")[0]);
        formData.append('files', $('input[type=file]')[0].files[0])
        coreMaster.updateData("/ItemMasterData/Edit", ["itemMasterData", "itemAccountings", "properties"], function (res) {
            $("#btn-update").prop("disabled", false);
            if (res.IsRejected) {
                ViewMessage({}, res);
                return;
            }

            if (isValidJson(res.Items["ItemMasterData"])) {
                formData.append("ItemMasterId", res.Items["ItemMasterData"].ID);
                $.ajax({
                    url: "/ItemMasterData/UploadImg",
                    type: "POST",
                    processData: false,
                    contentType: false,
                    data: formData,
                    success: function () {
                        location.href = "/ItemMasterData/Index";
                    }
                });
            }
        });
    })
    $("#delete").on("click", function () {

        if ($(this).prop("checked") === true) {
            coreMaster.updateMaster("Delete", true);
        }
        if ($(this).prop("checked") === false) {
            coreMaster.updateMaster("Delete", false);
        }
    })

    $("#selSetGlAcc").val(_data.ItemMasterData.SetGlAccount);
    $("#selSetGlAcc").on("change", function () {
        const __input = $("div.hideInput");
        const hideTh = $(".hideTH");
        if (this.value == "2") {
            __input.show();
            $(".input").parent().show();
            hideTh.show();
            coreMaster.updateMaster("SetGlAccount", parseInt(this.value));
            for (let i = 0; i < _data.ItemAccountings.length; i++) {
                coreMaster.updateDetail(_data.ItemAccountings[i].WarehouseID, $(this).data("glacc"), parseInt(this.value));
            }
        }
        else if (this.value == "1") {
            __input.hide();
            hideTh.hide();
            $(".input").parent().hide();
            $(".input").val("");
            coreMaster.updateMaster("SetGlAccount", parseInt(this.value));
            for (let i = 0; i < _data.ItemAccountings.length; i++) {
                coreMaster.updateDetail(_data.ItemAccountings[i].WarehouseID, $(this).data("glacc"), parseInt(this.value));
            }
        }
        else {
            __input.hide();
            hideTh.hide();
            $(".input").parent().hide();
            $(".input").val("");
            coreMaster.updateMaster("SetGlAccount", parseInt(this.value));
            for (let i = 0; i < _data.ItemAccountings.length; i++) {
                coreMaster.updateDetail(_data.ItemAccountings[i].WarehouseID, $(this).data("glacc"), parseInt(this.value));
            }
        }
    })

    // Contract Warranty //
    $("#warrantyName").val(_data.Contract.Name);
    const idOptions = [
        "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday",
        "name", "contract-type", "response-time-value", "res-type", "duration", "renewal",
        "resolution-time-value", "reso-type", "reminder-value", "reminder-type", "description", "parts",
        "labor", "travel", "include-holiday", "remarks"
    ]
    const dataWarranty = {
        ID: 0, Description: "", Name: "", ResponseTimeValue: 0, ResponseTimeType: 0,
        ResolutionTimeValue: 0, ResolutionTimeType: 0, Expired: false, Duration: 0,
        Renewal: false, ReminderType: 0, ContractType: 0, ReminderValue: 0, Monday: false, Tuesday: false,
        Wednesday: false, Thursday: false, Friday: false, Saturday: false, Sunday: false, MondayStartTime: "",
        TuesdayStartTime: "", WednesdayStartTime: "", ThursdayStartTime: "", FridayStartTime: "", SaturdayStartTime: "",
        SundayStartTime: "", MondayEndTime: "", TuesdayEndTime: "", WednesdayEndTime: "", ThursdayEndTime: "",
        FridayEndTime: "", SaturdayEndTime: "", SundayEndTime: "", Remarks: "", Parts: false,
        Labor: false, Travel: false, Includeholiday: false,
    };
    let contractTable = ViewTable({});
    $("#choose-warranty").click(function () {
        const warDialog = new DialogBox({
            content: {
                selector: "#warranty-dailog-container"
            },
            button: {
                ok: {
                    text: "New",
                }
            },
            caption: "List Of Contract Templates",
            type: "ok-cancel"
        });
        warDialog.invoke(function () {
            contractTable = ViewTable({
                selector: ".list-warranty",
                keyField: "ID",
                indexed: true,
                paging: {
                    enabled: true
                },
                visibleFields: ["Name", "ContractType"],
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function ({ data, key }) {
                                $("#warrantyName").val(data.Name)
                                coreMaster.updateMaster("ContractID", key)
                                warDialog.shutdown();
                            }
                        }
                    }
                ]
            });
            $.get("/ItemMasterData/GetAllContractTemplates", function (res) {
                contractTable.bindRows(res);
            });
        })
        warDialog.reject(function () {
            warDialog.shutdown()
        })
        warDialog.confirm(function () {
            createWarrantyTemplateDailog()
        })
    })

    function createWarrantyTemplateDailog() {
        const warDialog = new DialogBox({
            content: {
                selector: "#warranty-creating-dailog-container"
            },
            button: {
                ok: {
                    text: "Save",
                }
            },
            caption: "Contract Templates - Setup",
            type: "ok-cancel"
        });
        warDialog.invoke(function () {
            //$("#monday").click(function () {
            //    if ($(this).prop("checked")) {
            //        $("#mondayStartTime").prop("disabled", false);
            //        $("#mondayEndTime").prop("disabled", false);
            //        dataWarranty.Monday = true;
            //    } else {
            //        $("#mondayStartTime").prop("disabled", true);
            //        $("#mondayEndTime").prop("disabled", true);
            //        dataWarranty.Monday = false;
            //    }
            //})
            //$("#tuesday").click(function () {
            //    if ($(this).prop("checked")) {
            //        $("#tuesdayStartTime").prop("disabled", false);
            //        $("#tuesdayEndTime").prop("disabled", false);
            //        dataWarranty.Tuesday = true;
            //    } else {
            //        $("#tuesdayStartTime").prop("disabled", true);
            //        $("#tuesdayEndTime").prop("disabled", true);
            //        dataWarranty.Tuesday = false;
            //    }
            //})
            //$("#wednesday").click(function () {
            //    if ($(this).prop("checked")) {
            //        $("#wednesdayStartTime").prop("disabled", false);
            //        $("#wednesdayEndTime").prop("disabled", false);
            //        dataWarranty.Wednesday = true;
            //    } else {
            //        $("#wednesdayStartTime").prop("disabled", true);
            //        $("#wednesdayEndTime").prop("disabled", true);
            //        dataWarranty.Wednesday = false;
            //    }
            //})
            //$("#thursday").click(function () {
            //    if ($(this).prop("checked")) {
            //        $("#thursdayStartTime").prop("disabled", false);
            //        $("#thursdayEndTime").prop("disabled", false);
            //        dataWarranty.Thursday = true;
            //    } else {
            //        $("#thursdayStartTime").prop("disabled", true);
            //        $("#thursdayEndTime").prop("disabled", true);
            //        dataWarranty.Thursday = false;
            //    }
            //})
            //$("#friday").click(function () {
            //    if ($(this).prop("checked")) {
            //        $("#fridayStartTime").prop("disabled", false);
            //        $("#fridayEndTime").prop("disabled", false);
            //        dataWarranty.Friday = true;
            //    } else {
            //        $("#fridayStartTime").prop("disabled", true);
            //        $("#fridayEndTime").prop("disabled", true);
            //        dataWarranty.Friday = false;
            //    }
            //})
            //$("#saturday").click(function () {
            //    if ($(this).prop("checked")) {
            //        $("#saturdayStartTime").prop("disabled", false);
            //        $("#saturdayEndTime").prop("disabled", false);
            //        dataWarranty.Saturday = true;
            //    } else {
            //        $("#saturdayStartTime").prop("disabled", true);
            //        $("#saturdayEndTime").prop("disabled", true);
            //        dataWarranty.Saturday = false;
            //    }
            //})
            //$("#sunday").click(function () {
            //    if ($(this).prop("checked")) {
            //        $("#sundayStartTime").prop("disabled", false);
            //        $("#sundayEndTime").prop("disabled", false);
            //        dataWarranty.Sunday = true;
            //    } else {
            //        $("#sundayStartTime").prop("disabled", true);
            //        $("#sundayEndTime").prop("disabled", true);
            //        dataWarranty.Sunday = false;
            //    }
            //})
            //$("#expired").click(function () {
            //    if ($(this).prop("checked")) {
            //        idOptions.forEach(i => {
            //            $(`#${i}`).prop("disabled", true);
            //        });
            //        dataWarranty.Expired = true;
            //    } else {
            //        idOptions.forEach(i => {
            //            $(`#${i}`).prop("disabled", false);
            //        })
            //        dataWarranty.Expired = false;
            //    }
            //})
            //$("#renewal").click(function () {
            //    if ($(this).prop("checked")) {
            //        $("#reminder-type").prop("disabled", false)
            //        $("#reminder-type").val(1);
            //        dataWarranty.Renewal = true;
            //    } else {
            //        $("#reminder-type").val(0);
            //        $("#reminder-type").prop("disabled", true)
            //        dataWarranty.Renewal = false;
            //    }
            //});
        });
        warDialog.confirm(function () {
            //dataWarranty.ContractType = $("#contract-type").val();
            //dataWarranty.Description = $("#description").val();
            //dataWarranty.Duration = $("#duration").val();
            //dataWarranty.FridayEndTime = $("#fridayEndTime").val();
            //dataWarranty.FridayStartTime = $("#fridayStartTime").val();
            //dataWarranty.Includeholiday = $("#include-holiday").prop("checked") ? true : false;
            //dataWarranty.Labor = $("#labor").prop("checked") ? true : false;
            //dataWarranty.MondayEndTime = $("#mondayEndTime").val();
            //dataWarranty.MondayStartTime = $("#mondayStartTime").val();
            //dataWarranty.Name = $("#name").val();
            //dataWarranty.Parts = $("#parts").prop("checked") ? true : false;
            //dataWarranty.Remarks = $("#remarks").val();
            //dataWarranty.ReminderType = $("#reminder-type").val();
            //dataWarranty.ReminderValue = $("#reminder-value").val() === "" ? 0 : $("#reminder-value").val();
            //dataWarranty.ResolutionTimeType = $("#reso-type").val();
            //dataWarranty.ResolutionTimeValue = $("#resolution-time-value").val() === "" ? 0 : $("#resolution-time-value").val();
            //dataWarranty.ResponseTimeType = $("#res-type").val();
            //dataWarranty.ResponseTimeValue = $("#response-time-value").val() === "" ? 0 : $("#response-time-value").val();
            //dataWarranty.SaturdayEndTime = $("#saturdayEndTime").val();
            //dataWarranty.SaturdayStartTime = $("#saturdayStartTime").val();
            //dataWarranty.SundayEndTime = $("#sundayEndTime").val();
            //dataWarranty.SundayStartTime = $("#sundayStartTime").val();
            //dataWarranty.ThursdayEndTime = $("#thursdayEndTime").val();
            //dataWarranty.ThursdayStartTime = $("#thursdayStartTime").val();
            //dataWarranty.Travel = $("#travel").prop("checked") ? true : false;
            //dataWarranty.TuesdayEndTime = $("#tuesdayEndTime").val();
            //dataWarranty.TuesdayStartTime = $("#tuesdayStartTime").val();
            //dataWarranty.WednesdayEndTime = $("#wednesdayEndTime").val();
            //dataWarranty.WednesdayStartTime = $("#wednesdayStartTime").val();
            contracttemplate_master.ID = $("#id").val();
            contracttemplate_master.Name = $("#name").val();
            contracttemplate_master.ResponseTime = $("#response-time-value").val();
            contracttemplate_master.ResultionTime = $("#resolution-time-value").val();
            contracttemplate_master.ResponseTimeDH = $("#res-type").val();
            contracttemplate_master.ResultionTimeDH = $("#reso-type").val();
            contracttemplate_master.Description = $("#description").val();
            contracttemplate_master.ContracType = $("#contract-type").val();
            contracttemplate_master.Expired = $("#expired").prop("checked") ? true : false;

            converageObj.ID = $("#converid").val();
            converageObj.Part = $("#parts").prop("checked") ? true : false;
            converageObj.Labor = $("#labor").prop("checked") ? true : false;
            converageObj.Travel = $("#travel").prop("checked") ? true : false;
            converageObj.Holiday = $("#include-holiday").prop("checked") ? true : false;

            converageObj.Monthday = $("#monday").prop("checked") ? true : false;
            converageObj.Thuesday = $("#tuesday").prop("checked") ? true : false;
            converageObj.Wednesday = $("#wednesday").prop("checked") ? true : false;
            converageObj.Thursday = $("#thursday").prop("checked") ? true : false;
            converageObj.Friday = $("#friday").prop("checked") ? true : false;
            converageObj.Saturday = $("#saturday").prop("checked") ? true : false;
            converageObj.Sunday = $("#sunday").prop("checked") ? true : false;

            //converageObj.Sunday = $("#sun").prop("checked") ? true : false;
            //converageObj.Sunday = $("#sun").prop("checked") ? true : false;
            //converageObj.Sunday = $("#sun").prop("checked") ? true : false;
            //converageObj.Sunday = $("#sun").prop("checked") ? true : false;

            converageObj.StarttimeMon = $("#mondayStartTime").val();
            converageObj.StarttimeThu = $("#tuesdayStartTime").val();
            converageObj.StarttimeWed = $("#wednesdayStartTime").val();
            converageObj.StarttimeThur = $("#thursdayStartTime").val();
            converageObj.StarttimeFri = $("#fridayStartTime").val();
            converageObj.StarttimeSat = $("#saturdayStartTime").val();
            converageObj.StarttimeSun = $("#sundayStartTime").val();

            converageObj.EndtimeMon = $("#mondayEndTime").val();
            converageObj.EndtimeThu = $("#tuesdayEndTime").val();
            converageObj.EndtimeWed = $("#wednesdayEndTime").val();
            converageObj.EndtimeThur = $("#thursdayEndTime").val();
            converageObj.EndtimeFri = $("#fridayEndTime").val();
            converageObj.EndtimeSat = $("#saturdayEndTime").val();
            converageObj.EndtimeSun = $("#sundayEndTime").val();
            contracttemplate_master.Converage = converageObj;
            //=========Remark============
            remarkObj.ID = $("#remarkid").val();
            remarkObj.Remarks = $("#remarks").val();
            contracttemplate_master.Remark = remarkObj;


            $.post("/ItemMasterData/CreateContractTemplate", { contract: contracttemplate_master }, function (res) {
                new ViewMessage({
                    summary: {
                        selector: ".err-success-contract"
                    }
                });
                if (res.IsApproved) {
                    contractTable.addRow(res.Items["Contract"]);
                    warDialog.shutdown();
                }
            })
        })
        warDialog.reject(function () {
            warDialog.shutdown()
        })
    }

    // End Contract Warranty //

    if (_data.ItemMasterData.Process != "Standard") {
        document.getElementById("txtCost").disabled = true;
        document.getElementById("txtUnitPrice").disabled = true;
        $("#txtCost").val(0);
        $("#txtUnitPrice").val(0);
        //$("#salecheckbox").prop("checked", true);
        //$("#invcheckbox").prop("checked", true);
        //$("#purcheckbox").prop("checked", true);
        $("#errmanItemBy").text("");
        //coreMaster.updateMaster("Inventory", true);
        //coreMaster.updateMaster("Purchase", true);
        //coreMaster.updateMaster("Sale", true);
        coreMaster.updateMaster("Cost", 0);
        coreMaster.updateMaster("UnitPrice", 0);
    }
    else {
        document.getElementById("txtCost").disabled = false;
        document.getElementById("txtUnitPrice").disabled = false;
        $("#txtCost").val("");
        $("#txtUnitPrice").val("");
        $("#salecheckbox").prop("checked", true);
        $("#invcheckbox").prop("checked", false);
        $("#purcheckbox").prop("checked", false);
        coreMaster.updateMaster("Sale", true);
        coreMaster.updateMaster("Inventory", false);
        coreMaster.updateMaster("Purchase", false);
        coreMaster.updateMaster("Cost", "");
        coreMaster.updateMaster("UnitPrice", "");
    }
    if (_data.ItemMasterData.ManItemBy !== 0) {
        $(".seba").prop("hidden", false);
    }
    if (_data.ItemMasterData.ManItemBy === 1) {
        $("#warrantyContainer").prop("hidden", false);
    } else {
        $("#warrantyContainer").prop("hidden", true);
    }
    $("#manMethod").val(_data.ItemMasterData.ManMethod);
    function onGetWhAccTemplates(success) {
        const id = $("#txtitemID").val();
        $.get("/ItemMasterData/WarehouseAccounting", { id: parseInt(id) }, success);
    }
    let $listJE = ViewTable({
        container: "#wrap-table",
        keyField: "LineID",
        selector: "#allw-gl",
        paging: {
            enabled: false
        },
        visibleFields: [
            "CodeWarehouse",
            "Name",
            "InStock",
            "Committed",
            "Ordered",
            "Available",
            "MinimunInventory",
            "MaximunInventory",
            "ExpenseAccount",
            "RevenueAccount",
            "InventoryAccount",
            "CostofGoodsSoldAccount",
            "AllocationAccount",
            "VarianceAccount",
            "PriceDifferenceAccount",
            "NegativeInventoryAdjustmentAcct",
            "InventoryOffsetDecreaseAccount",
            "InventoryOffsetIncreaseAccount",
            "SalesReturnsAccount",
            "RevenueAccountEU",
            "ExpenseAccountEU",
            "RevenueAccountForeign",
            "ExpenseAccountForeign",
            "ExchangeRateDifferencesAccount",
            "GoodsClearingAccount",
            "GLDecreaseAccount",
            "GLIncreaseAccount",
            "WIPInventoryAccount",
            "WIPInventoryVarianceAccount",
            "WIPOffsetPLAccount",
            "InventoryOffsetPLAccount",
            "ExpenseClearingAccount",
            "StockInTransitAccount",
            "ShippedGoodsAccount",
            "SalesCreditAccount",
            "PurchaseCreditAccount",
            "SalesCreditAccountForeign",
            "PurchaseCreditAccountForeign",
            "SalesCreditAccountEU",
            "PurchaseCreditAccountEU"
        ],
        columns: [
            {
                name: "CodeWarehouse",
                on: {
                    "dblclick": function (e) {
                        chooseCode(e.key);
                    }
                }
            },
            {
                name: "Name",
                on: {
                    "dblclick": function (e) {
                        chooseCode(e.key);
                    }
                }
            },
            {
                name: "MinimunInventory",
                template: "<input type='number' class='number'>",
                on: {
                    "keyup": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        coreMaster.updateDetail(key, "MinimunInventory", $(this).val());
                    }
                }
            },
            {
                name: "MaximunInventory",
                template: "<input type='number' class='number'>",
                on: {
                    "keyup": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        coreMaster.updateDetail(key, "MaximunInventory", $(this).val());
                    }
                }
            },
            {
                name: "ExpenseAccount",
                template: "<input class='input' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "RevenueAccount",
                template: "<input class='input number' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "InventoryAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "CostofGoodsSoldAccount",
                template: "<input class='input number' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "AllocationAccount",
                template: "<input class='input number' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "VarianceAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "PriceDifferenceAccount",
                template: "<input class='input number' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "NegativeInventoryAdjustmentAcct",
                template: "<input class='input number' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "InventoryOffsetDecreaseAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "InventoryOffsetIncreaseAccount",
                template: "<input class='input number' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "SalesReturnsAccount",
                template: "<input class='input number' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "RevenueAccountEU",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "ExpenseAccountEU",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "RevenueAccountForeign",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "ExpenseAccountForeign",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "ExchangeRateDifferencesAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "GoodsClearingAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "GLDecreaseAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "GLIncreaseAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "WIPInventoryAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "WIPInventoryVarianceAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "WIPOffsetPLAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "InventoryOffsetPLAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "ExpenseClearingAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "StockInTransitAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "ShippedGoodsAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "SalesCreditAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "PurchaseCreditAccount",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "SalesCreditAccountForeign",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "PurchaseCreditAccountForeign",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "SalesCreditAccountEU",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            },
            {
                name: "PurchaseCreditAccountEU",
                template: "<input class='input ramarks' readonly>",
                on: {
                    "dblclick": function (e) {
                        let key = $(this).parent().parent().find("td:eq(4)").text();
                        chooseCode(e.key, this, key);
                    }
                }
            }
        ]
    });

    function chooseCode(id, template, key) {
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
                selector: "#active-gl-content"
            }
        });

        dialog.invoke(function () {
            $.get("/ItemGroup1/GetGlAccountLastLevel", function (resp) {
                let $listActiveGL = ViewTable({
                    keyField: "ID",
                    selector: dialog.content.find("#list-active-gl"),
                    paging: {
                        pageSize: 10,
                        enabled: true
                    },
                    visibleFields: ["Code", "Name"],
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down'></i>",
                            on: {
                                "click": function (e) {
                                    $(template).prop("readonly", true);
                                    $.each(thName, function (index, item) {
                                        if ($(template).val() === '') {
                                            if ($(template).parent().data("field") === item.replace(/ /g, "")) {
                                                for (let i = 0; i < _data.ItemAccountings.length; i++) {
                                                    $listJE.updateColumn(i, item.replace(/ /g, ""), e.data.Code);
                                                    coreMaster.updateDetail(_data.ItemAccountings[i].WarehouseID, item.replace(/ /g, ""), e.data.Code);
                                                }
                                            }
                                        }
                                        if ($(template).val() !== '') {
                                            if ($(template).parent().data("field") === item.replace(/ /g, "")) {
                                                $listJE.updateColumn(id, item.replace(/ /g, ""), e.data.Code);
                                                coreMaster.updateDetail(key, item.replace(/ /g, ""), e.data.Code);
                                            }
                                        }
                                    });
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                if (resp.length > 0) {
                    $listActiveGL.bindRows(resp);
                    $("#txtSearch").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let rex = new RegExp(__value, "gi");
                        let items = $.grep(resp, function (item) {
                            return item.Code === __value || item.Name.toLowerCase().replace(/\s+/, "").match(rex);
                        });
                        $listActiveGL.bindRows(items);
                    });
                }
            });
        });
    }
    let totalstock = 0;
    onGetWhAccTemplates(function (whAcc) {
        whAcc.forEach(i => {
            totalstock += i.InStock;
        });
        if (totalstock > 0) {
            $("#listGroupUom").attr("disabled", true);
        }
        $listJE.bindRows(whAcc);
        $(".input").attr("readonly", true);
        if (_data.ItemMasterData.SetGlAccount == 1) {
            const __input = $("div.hideInput");
            const hideTh = $(".hideTH");
            __input.hide();
            hideTh.hide();
            $(".input").parent().hide();
        }
    });
    $("#txtSearch").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#list-active-gl tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $.ajax({
        url: "/Currency/GetCurrency",
        type: "GET",
        dataType: "JSON",
        success: function (respones) {
            var data = "";

            $.each(respones, function (i, item) {
                data +=
                    '<option value="' + item.ID + '">' + item.Symbol + '</option>';
            });
            $("#txtcurrency").append(data);
        }
    });
    $.ajax({
        url: "/ItemGroup1/SelecrColor",
        type: "Get",
        dataType: "Json",
        success: function (respones) {
            var data = "";
            $.each(respones, function (i, item) {
                data +=
                    '<option value="' + item.ColorID + '">' + item.Name + '</option>';
            });
            $("#txtcolor").append(data);
            $("#txtcolor2").append(data);
            $("#txtcolor3").append(data);
        }
    });

    $.ajax({
        url: "/ItemGroup1/SelectBackground",
        type: "Get",
        dataType: "Json",
        success: function (respones) {
            var data = "";

            $.each(respones, function (i, item) {
                data +=
                    '<option value="' + item.BackID + '">' + item.Name + '</option>';
            });
            $("#txtbackground").append(data);
            $("#txtbackground2").append(data);
            $("#txtbackground3").append(data);
        }
    });
    $.ajax({
        url: "/ItemGroup1/GetItemGroup",
        type: "Get",
        dataType: "Json",
        success: function (respones) {
            var data = "";
            $.each(respones, function (i, item) {
                data +=
                    '<option value="' + item.ItemG1ID + '">' + item.Name + '</option>';
            });
            $("#txtItemgroup1").append(data);
            $("#txtname1").append(data);

        }
    });
    // check inventory transactions
    var itemid = $("#txtitemID").val();
    $.ajax({
        url: "/ItemMasterData/CheckTransactionItem",
        type: "Get",
        dataType: "text",
        data: { ItemID: itemid },
        success: function (respones) {
            if (respones == "Y") {
                $(".control_disabled").prop("disabled", true);
            }
            else {
                $(".control_disabled").prop("disabled", false);
            }
        }

    })

    $("#txtselectwarehouse").on("change", function () {
        coreMaster.updateMaster("WarehouseID", parseInt(this.value));
    });
    $("#txtselectprice").on("change", function () {
        coreMaster.updateMaster("PriceListID", parseInt(this.value));
    });
    $("#txtprinter").on("change", function () {
        coreMaster.updateMaster("PrintToID", parseInt(this.value));
    });
    $("#txtmanage").on("change", function () {
        coreMaster.updateMaster("ManageExpire", this.value);
    });
    $("#txtInventoryUOM").on("change", function () {
        coreMaster.updateMaster("InventoryUoMID", parseInt(this.value));
    });
    $("#txtSaleUOM").on("change", function () {
        coreMaster.updateMaster("SaleUomID", parseInt(this.value));
    });
    $("#txtPurchar").on("change", function () {
        coreMaster.updateMaster("PurchaseUomID", parseInt(this.value));
    });
    $("#itemgroup3Name").on("change", function () {
        coreMaster.updateMaster("ItemGroup3ID", parseInt(this.value));
    });
    $("#type").on("change", function () {
        coreMaster.updateMaster("Type", this.value);
    })
    $("#manItemBy").change(function () {
        if (this.value == 1) {
            $("#warrantyContainer").prop("hidden", false);
        } else {
            $("#warrantyContainer").prop("hidden", true);
        }
        if (this.value == 0) {
            $(".seba").prop("hidden", true)
            $("#manMethod").prop('disabled', true)
        } else {
            $(".seba").prop("hidden", false)
            $("#manMethod").prop('disabled', false)
            if (!$("#invcheckbox").prop("checked")) {
                $("#errmanItemBy").text("Non-inventory item cannot be managed by batch or serial numbers!")
                $(this).val(0);
            } else {
                $("#errmanItemBy").text("");
            }
        }
    });
});


function isValidJson(value) {
    return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
}
function isValidArray(values) {
    return Array.isArray(values) && values.length > 0;
}
//check number
function show(input) {
    if (input.files && input.files[0]) {
        var filerdr = new FileReader();

        filerdr.onload = function (e) {
            $('#logo').attr('src', e.target.result);
            $('#image_name').val(input.files[0].name);
        }
        filerdr.readAsDataURL(input.files[0]);
    }
}
//chang proccess
function SelectChangProccess(val) {
    coreMaster.updateMaster("Process", val.value);
    var type = $("#txtProccess").val();
    if (type != "Standard") {
        document.getElementById("txtCost").disabled = true;
        document.getElementById("txtUnitPrice").disabled = true;
        $("#txtCost").val(0);
        $("#txtUnitPrice").val(0);
        $("#salecheckbox").prop("checked", true);
        $("#invcheckbox").prop("checked", true);
        $("#purcheckbox").prop("checked", true);
        $("#errmanItemBy").text("");
        coreMaster.updateMaster("Inventory", true);
        coreMaster.updateMaster("Purchase", true);
        coreMaster.updateMaster("Sale", true);
        coreMaster.updateMaster("Cost", 0);
        coreMaster.updateMaster("UnitPrice", 0);
    }
    // else if (type == "FEFO") {
    //     document.getElementById("txtCost").disabled = true;
    //     document.getElementById("txtUnitPrice").disabled = true;
    //     $("#txtCost").val(0);
    //     $("#manItemBy").val(2).prop("disabled", true);
    //     $("#txtUnitPrice").val(0);
    //     $("#salecheckbox").prop("checked", true);
    //     $("#invcheckbox").prop("checked", true);
    //     $("#purcheckbox").prop("checked", true);
    //     $("#errmanItemBy").text("");
    //     coreMaster.updateMaster("Inventory", true);
    //     coreMaster.updateMaster("Purchase", true);
    //     coreMaster.updateMaster("Sale", true);
    //     coreMaster.updateMaster("Cost", 0);
    //     coreMaster.updateMaster("UnitPrice", 0);
    // }
    else {
        document.getElementById("txtCost").disabled = false;
        document.getElementById("txtUnitPrice").disabled = false;
        $("#txtCost").val("");
        $("#manItemBy").val(0);
        $("#manMethod").val(0);
        $("#txtUnitPrice").val("");
        $("#salecheckbox").prop("checked", true);
        $("#invcheckbox").prop("checked", false);
        $("#purcheckbox").prop("checked", false);
        coreMaster.updateMaster("Sale", true);
        coreMaster.updateMaster("Inventory", false);
        coreMaster.updateMaster("Purchase", false);
        coreMaster.updateMaster("Cost", "");
        coreMaster.updateMaster("UnitPrice", "");
    }
}
//insertpircelist
function clickinsertpricelist() {

    var name = $("#txtpricelist").val();
    var cur = $("#txtcurrency").val();

    var count = 0;
    if (name == 0) {
        count++;
        $("#txtpricelist").css("border-color", "red");

    } else {
        $("#txtpricelist").css("border-color", "lightgrey");

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
    var data = {
        Name: name,
        CurrencyID: cur
    }
    $.ajax({
        url: "/PriceList/InsertPricelist",
        type: "POST",
        dataType: "JSON",
        data: { priceList: data },
        complete: function (respones) {
            $("#txtpricelist").val("");
            $("#txtcurrency").val("");
            $.ajax({
                url: "/PriceList/GetselectPricelist",
                type: "Get",
                dataType: "JSON",
                success: function (respones) {
                    var sata = "";
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
//addwarehouse
function AddWarehouse() {
    var code = $("#Wtxtcode").val();
    var name = $("#Wtxtname").val();
    var loca = $("#Wtxtlocation").val();
    var add = $("#Wtxtaddress").val();

    var count = 0;
    if (code == 0) {
        count++;
        $("#Wtxtcode").css("border-color", "red");
    }
    else {
        $("#Wtxtcode").css("border-color", "lightgrey");

    }
    if (name == 0) {
        count++;
        $("#Wtxtname").css("border-color", "red");
    }
    else {

        $("#Wtxtname").css("border-color", "lightgrey");
    }
    if (loca == 0) {
        count++;
        $("#Wtxtlocation").css("border-color", "red");
    }
    else {
        $("#Wtxtlocation").css("border-color", "lightgrey");

    }
    if (add == 0) {
        count++;
        $("#Wtxtaddress").css("border-color", "red");
    }
    else {

        $("#Wtxtaddress").css("border-color", "lightgrey");
    }
    if (count > 0) {
        count = 0;
        return;
    }

    var data = {

        Code: code,
        Name: name,
        Location: loca,
        Address: add
    }

    $.ajax({
        url: "/Warehouse/AddWarehouse",
        type: "POST",
        dataType: "JSON",
        data: { warehouse: data },
        complete: function (respones) {

            $("#Wtxtcode").val("");
            $("#Wtxtname").val("");
            $("#Wtxtlocation").val("");
            $("#Wtxtaddress").val("");

            $.ajax({
                url: "/Warehouse/GetWarehouse",
                type: "Get",
                dataType: "JSON",
                success: function (respones) {
                    var sata = "";
                    $.each(respones, function (i, item) {
                        sata +=
                            '<option value="' + item.ID + '">' + item.Name + '</option>';
                    });
                    $("#txtselectwarehouse").html(sata);
                }

            });
        },

    })
}
//check code warehouse
function CheckCodeWarehouse() {
    var code = $("#Wtxtcode").val();
    $.ajax({
        url: "/Warehouse/CheckCodeWarehouse",
        type: "GET",
        dataType: "JSON",
        data: { Code: code },
        success: function (respones) {

            var isHase = false;
            $.each(respones, function (i, item) {
                isHase = true;
            })
            if (isHase == true) {
                $('#error_codewarehouse').text("This code already exist !");
            }
            else {
                $('#error_barcode').text("");
            }

        }
    });
}
//additemgroup(1)
function AddItemGroup1() {

    var name = $("#Itemgroup1Name").val();
    var col = $("#txtcolor").val();
    var back = $("#txtbackground").val();
    var count = 0;
    if (name == 0) {
        count++;
        $("#Itemgroup1Name").css("border-color", "red");
    }
    else {

        $("#Itemgroup1Name").css("border-color", "lightgrey");
    }

    if (count > 0) {
        count = 0;
        return;
    }
    var data = {
        Name: name,
        ColorID: col,
        BackID: back
    }
    $.ajax({
        url: "/ItemGroup1/Additmegroup1",
        type: "POST",
        dataType: "JSON",
        data: { itemGroup1: data },
        complete: function (respones) {
            $("#Itemgroup1Name").val("");
            $("#txtcolor").val("");
            $("#txtbackground").val("");
            $("#selectItmegroup2 option").remove();
            $("#itemgroup3Name option").remove();
            $("#txtItemgroup1 option").remove();
            $("#txtname1 option").remove();
            $("#itmeID1").val("");
            $("#itemID2").val("");
            $.ajax({
                url: "/ItemGroup1/GetItemGroup",
                type: "GET",
                dataType: "JSON",
                success: function (respones) {
                    var sata = "";
                    $.each(respones, function (i, item) {
                        sata +=
                            '<option value="' + item.ItemG1ID + '">' + item.Name + '</option>';
                    });
                    $("#selectItemGroup1").html(sata);
                    $("#txtItemgroup1").html(sata);
                    $("#txtname1").html(sata);
                }
            });

        }
    });
}
//additmegroup2
function AddItemGroup2() {
    var name2 = $("#itemgroup2Name").val();
    var name1 = $("#txtItemgroup1").val();
    var col = $("#txtcolor2").val();
    var back = $("#txtbackground2").val();

    var count = 0;
    if (name2 == 0) {
        count++;
        $("#itemgroup2Name").css("border-color", "red");

    } else {
        $("#itemgroup2Name").css("border-color", "lightgrey");

    }
    if (name1 == 0) {
        count++;
        $("#txtItemgroup1").css("border-color", "red");
    } else {
        $("#txtItemgroup1").css("border-color", "lightgrey");

    }
    if (count > 0) {
        count = 0;
        return;
    }

    var data = {
        Name: name2,
        ItemG1ID: name1,
        ColorID: col,
        BackID: back
    }
    $.ajax({
        url: "/ItemGroup2/Additmegroup2",
        type: "POST",
        dataType: "JSON",
        data: { itemGroup2: data },
        complete: function (respones) {
            $("#itemgroup2Name").val("");
            $("#txtItemgroup1").val("");
            $("#txtcolor2").val("");
            $("#txtbackground2").val("");
            $("#itemgroup3Name option").remove();
            var item1id = $("#selectItemGroup1").val();
            $.ajax({
                url: "/ItemGroup2/GetData",
                type: "Get",
                data: { ID: item1id },
                dataType: "Json",
                success: function (respones) {
                    var data = "";

                    $("#txtitem2 option").remove();
                    $.each(respones, function (i, item) {
                        data +=
                            '<option value="' + item.ItemG2ID + '">' + item.Name + '</option>';
                    });
                    $("#selectItmegroup2").html(data);

                }
            });

        }
    });
}
//selectchangitem2
function selectchangitemgroup1(val) {
    coreMaster.updateMaster("ItemGroup1ID", parseInt(val.value));
    var ID = $("#selectItemGroup1").val();
    $.ajax({
        type: "GET",
        datatype: "Json",
        url: "/ItemGroup2/GetItemGroup2",
        data: { Item1ID: ID },
        success: function (response) {
            var data = "";
            $.each(response, function (key, value) {
                data += '<option value="' + value.ItemG2ID + '">' + value.Name + '</option>';
            });
            $('#selectItmegroup2').html(data);
            $('#selectItmegroup2').val(0);
            var item_g2ID = $("#selectItmegroup2").val();
            $.ajax({
                url: "/ItemGroup3/GetitemGroup3",
                type: "GET",
                dataType: "JSON",
                data: { Item2ID: item_g2ID, Item1ID: ID },
                success: function (respones) {
                    var sata = "";

                    $.each(respones, function (i, item) {
                        sata +=
                            '<option value="' + item.ID + '">' + item.Name + '</option>';
                    });
                    $("#itemgroup3Name").html(sata);
                    $("#itemgroup3Name").val(0);
                }
            });

        }
    });
}

//select Chang ItemGroup3
function selectChangItemGroup3(val) {
    coreMaster.updateMaster("ItemGroup2ID", parseInt(val.value));
    var id2 = $("#selectItmegroup2").val();
    var id1 = $("#selectItemGroup1").val();
    $('#itemgroup3Name option').remove();
    $.ajax({
        url: "/ItemGroup3/GetitemGroup3",
        type: "GET",
        dataType: "JSON",
        data: { Item2ID: id2, Item1ID: id1 },
        success: function (respones) {
            var sata = "";

            $.each(respones, function (i, item) {
                sata +=
                    '<option value="' + item.ID + '">' + item.Name + '</option>';
            });
            $("#itemgroup3Name").append(sata);
            $("#itemgroup3Name").val(0);
        }
    })
}

function ChangeItemGroup2() {

    var ID = $("#txtname1").val();
    $('#txtname2 option').remove();
    $.ajax({
        type: "GET",
        datatype: "Json",
        url: "/ItemGroup2/GetItemGroup2",
        data: { Item1ID: ID },
        success: function (response) {
            var data = "";
            $.each(response, function (key, value) {
                data += '<option value="' + value.ItemG2ID + '">' + value.Name + '</option>';
            });
            $('#txtname2').append(data);
            $("#txtname2").val(ID2);

        }
    });
}
//additemgorup3
function AddItemGroup3() {
    var name1 = $("#txtname1").val();
    var name2 = $("#txtname2").val();
    var name3 = $("#txtname3").val();
    var col = $("#txtcolor3").val();
    var back = $("#txtbackground3").val();

    var count = 0;
    if (name1 == 0) {
        count++;
        $("#txtname1").css("border-color", "red");
    }
    else {

        $("#txtname1").css("border-color", "lightgrey");
    }
    if (name2 == 0) {
        count++;
        $("#txtname2").css("border-color", "red");
    }
    else {

        $("#txtname2").css("border-color", "lightgrey");
    }
    if (name3 == 0) {
        count++;
        $("#txtname3").css("border-color", "red");
    }
    else {

        $("#txtname3").css("border-color", "lightgrey");
    }
    if (count > 0) {
        count = 0;
        return;
    }
    var data = {
        Name: name3,
        ItemG1ID: name1,
        ItemG2ID: name2,
        ColorID: col,
        BackID: back
    }

    $.ajax({
        url: "/ItemGroup3/AddItemGroup3",
        type: "POST",
        dataType: "JSON",
        data: { itemGroup3: data },
        complete: function (respones) {
            $("#txtname1").val("");
            $("#txtname2").val("");
            $("#txtname3").val("");
            $("#txtcolor3").val("");
            $("#txtbackground3").val("");

            var id = $("#selectItmegroup2").val();
            $("#itemgroup3Name option").remove();
            $.ajax({
                url: "/ItemGroup3/GetitemGroup3",
                type: "GET",
                dataType: "JSON",
                data: { ID: id },
                success: function (respones) {
                    var sata = "";
                    $.each(respones, function (i, item) {
                        sata +=
                            '<option value="' + item.ID + '">' + item.Name + '</option>';
                    });
                    $("#itemgroup3Name").append(sata);
                }
            })
        }
    });

}

//insertPrinter Name
function InsertPrintName() {
    var name = $("#txtPrinterName").val();

    var count = 0;
    if (name == '') {
        count++;
        $("#txtPrinterName").css("border-color", "red");
    }
    else {

        $("#txtPrinterName").css("border-color", "lightgrey");
    }
    if (count > 0) {
        count = 0;
        return;
    }
    $.ajax({
        url: "/PrinterName/InsertPrinter",
        type: "POST",
        dataType: "JSON",
        data: { Name: name },
        complete: function (respones) {
            $("#txtPrinterName").val("");
            $("#txtprinter option").remove();
            $.ajax({
                url: "/PrinterName/GetPrinterName",
                type: "GET",
                dataType: "JSON",
                success: function (respones) {
                    var data = "";
                    $.each(respones, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.Name + '</option>';
                    });
                    $("#txtprinter").append(data);
                }
            })
        }
    });
}

//selectGroup UOM
function selectCahngeGroupUOM(val) {
    coreMaster.updateMaster("GroupUomID", parseInt(val.value));
    var id = $("#listGroupUom").val();
    $.ajax({
        url: "/GroupUOM/listGroupUom",
        type: "GET",
        dataType: "JSON",
        data: { id: id },
        success: function (respones) {
            var data = "";
            var baseuomid;
            //var res = JSON.stringify(respones); alert(res)
            $.each(respones, function (i, item) {
                data +=
                    '<option value="' + item.AltUOM + '">' + item.UnitofMeasure.AltUomName + '</option>';
                baseuomid = item.BaseUOM;

            });
            $("#txtInventoryUOM").html(data);
            $("#txtSaleUOM").html(data);
            $("#txtPurchar").html(data);

            $("#txtInventoryUOM").innerHTML = baseuomid;
            $("#txtSaleUOM").innerHTML = baseuomid;
            $("#txtPurchar").innerHTML = baseuomid;
            coreMaster.updateMaster("InventoryUoMID", baseuomid);
            coreMaster.updateMaster("SaleUomID", baseuomid);
            coreMaster.updateMaster("PurchaseUomID", baseuomid);
        }
    })
}

//Add Color
function AddColor() {
    var name = $("#colorName").val();
    var cout = "";
    if (name == 0) {
        cout++;
        $("#colorName").css("border-color", "red");
    } else {

        $("#colorName").css("border-color", "lightgrey");
    }
    if (cout > 0) {
        cout = 0;
        return;
    }
    $.ajax({
        url: "/ItemGroup1/AddColors",
        type: "POST",
        dataType: "Json",
        data: { Name: name },
        complete: function (respones) {
            $("#colorName").val("");
            $("#txtcolor option").remove();
            $("#txtcolor2 option").remove();
            $("#txtcolor3 option").remove();
            $.ajax({
                url: "/ItemGroup1/SelecrColor",
                type: "Get",
                dataType: "Json",
                success: function (respones) {
                    var data = "";
                    $.each(respones, function (i, item) {
                        data +=
                            '<option value="' + item.ColorID + '">' + item.Name + '</option>';
                    });
                    $("#txtcolor").append(data);
                    $("#txtcolor2").append(data);
                    $("#txtcolor3").append(data);
                }
            });
        }
    });
}
//Add Background
function AddBack() {
    var name = $("#backName").val();
    var cout = "";
    if (name == 0) {
        cout++;
        $("#backName").css("border-color", "red");
    } else {

        $("#backName").css("border-color", "lightgrey");
    }
    if (cout > 0) {
        cout = 0;
        return;
    }
    $.ajax({
        url: "/ItemGroup1/AddBackground",
        type: "POST",
        dataType: "Json",
        data: { Name: name },
        complete: function (respones) {
            $("#backName").val("");
            $("#txtbackground option").remove();
            $("#txtbackground2 option").remove();
            $("#txtbackground3 option").remove();
            $.ajax({
                url: "/ItemGroup1/SelectBackground",
                type: "Get",
                dataType: "Json",
                success: function (respones) {
                    var data = "";

                    $.each(respones, function (i, item) {
                        data +=
                            '<option value="' + item.BackID + '">' + item.Name + '</option>';
                    });
                    $("#txtbackground").append(data);
                    $("#txtbackground2").append(data);
                    $("#txtbackground3").append(data);
                }
            });
        }
    });
}
//Add curreny
function AddCurrency() {
    var name = $("#currrencyName").val();
    var des = $("#txtDescript").val();
    var cout = "";
    if (name == 0) {
        cout++;
        $("#currrencyName").css("border-color", "red");
    } else {

        $("#currrencyName").css("border-color", "lightgrey");
    }
    if (des == 0) {
        cout++;
        $("#txtDescript").css("border-color", "red");
    }
    else {

        $("#txtDescript").css("border-color", "lightgrey");
    }
    if (cout > 0) {
        cout = 0;
        return;
    }
    $.ajax({
        url: "/Currency/AddCurrency",
        type: "POST",
        dataType: "Json",
        data: { Symbol: name, Description: des },
        complete: function (respones) {
            $("#currrencyName").val("");
            $("#txtDescript").val("");
            $("#txtcurrency option").remove();
            $.ajax({
                url: "/Currency/GetCurrency",
                type: "GET",
                dataType: "JSON",
                success: function (respones) {
                    var data = "";

                    $.each(respones, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.Symbol + '</option>';
                    });
                    $("#txtcurrency").append(data);
                }
            });
        }
    });
}
//check barcode
function CheckBarCode() {

    var bar = $("#txtbarcode").val();
    var barcode = $("#txtbar").val();
    if (bar == barcode) {
        $('#error_barcode').text("");
    } else {
        setTimeout(function () {
            $.ajax({
                url: "/ItemMasterData/FindbarCode",
                type: "GET",
                dataType: "JSON",
                data: { barcode: bar },
                success: function (respones) {

                    var isHase = false;
                    $.each(respones, function (i, item) {
                        isHase = true;
                    })
                    if (isHase == true) {
                        $('#error_barcode').text("This barcode already exist !");
                    }
                    else {
                        $('#error_barcode').text("");
                    }

                }
            });
        }, 600);
    }
}
function findArray(keyName, keyValue, values) {
    if (isValidArray(values)) {
        return $.grep(values, function (item, i) {
            return item[keyName] == keyValue;
        })[0];
    }
}