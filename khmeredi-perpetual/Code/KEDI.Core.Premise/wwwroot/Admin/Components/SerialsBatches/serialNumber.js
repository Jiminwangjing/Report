function SerialTemplatePur(config) {
    if (!(this instanceof SerialTemplatePur)) {
        return new SerialTemplatePur(config);
    }
    // Type //
    const MFR = "mfr";
    const SENUM = "senum";
    const LOTNUM = "lotnum";

    var __config = {
        data: {
            purData: {},
            serials: new Array(),
        },
    }
    const __infos = {
        serials: new Array(),
        serialStringCreation: new Array(),
        sSerialStringCreation: new Array(),
        lotSerialStringCreation: new Array(),
    }
    if (isValidJSON(config)) {
        __config = $.extend(true, __config, config);
    }
    const serialDialog = new DialogBox({
        content: {
            selector: "#container-list-serial-s-pur"
        },
        caption: "Serial Number - SetUp",
        type: "ok-cancel"
    });
    this.serialTemplate = function () {
        serialDialog.invoke(function () {

            if (__config.data.serials.length > 0) {
                // for (let i = 0; i < __config.data.serials.length; i++) {
                //     __config.data.serials[i].LineID = __config.data.serials[i].LineID + i;
                // }

                serialDialogBox(serialDialog, __config.data.serials);
            }

        });
        serialDialog.confirm(function () {
            // AutoSerialCreationDialog.shutdown();


            $.post("/PurchasePO/CheckSerailNumber", { serails: JSON.stringify(__config.data.serials) }, function (res) {
                if (res.IsApproved) {
                    $(".serailErrorMessage-s-pur").text("");
                    serialDialog.shutdown();
                } else {
                    new ViewMessage({
                        summary: {
                            selector: ".serailErrorMessage-s-pur"
                        }
                    }, res);
                }
            });
        });
        serialDialog.reject(function () {
            $(".serailErrorMessage-s-pur").text("");
            serialDialog.shutdown();
            //  AutoSerialCreationDialog.shutdown();
        });
    }

    this.callbackInfo = function () {
        return __infos;
    }

    let serialDetialView = ViewTable({});
    // Serial Number //
    function serialDialogBox(serialDialog, serials) {
        const serialView = ViewTable({
            keyField: "LineID",
            selector: serialDialog.content.find(".serial-lists-s-pur"),
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: [
                "DocNo", "ItemCode", "ItemName", "OpenQty", "TotalCreated", "TotalNeeded",
                "WhseCode", "WhseName"
            ],
            columns: [
                {
                    name: "DocNo",
                    on: {
                        "dblclick": function (e) {
                            e.data.SerialDetailAutoCreation.SerialAutomaticStringCreations = e.data.SerialAutomaticStringCreations;
                            e.data.SerialDetailAutoCreation.LotAutomaticStringCreations = e.data.LotAutomaticStringCreations;
                            e.data.SerialDetailAutoCreation.AutomaticStringCreations = e.data.AutomaticStringCreations;
                            bindSerialDetails(serialDialog, e.data.SerialDetialViewModelPurchase, e)
                            serialOption(serialDialog, e);
                            __infos.serialStringCreation = e.data.AutomaticStringCreations;
                        }
                    }
                }
            ],
        });
        serialView.bindRows(serials);
        __infos.serials = serialView.yield();
    }
    function bindSerialDetails(serialDialog, serials, _e) {
        serialDetialView = ViewTable({
            keyField: "LineID",
            selector: serialDialog.content.find(".serial-detail-lists-s-pur"),
            indexed: true,
            paging: {
                pageSize: 12,
                enabled: false
            },
            visibleFields: [
                "AdmissionDate", "Detials", "ExpirationDate", "Location", "LotNumber", "MfrDate",
                "MfrSerialNo", "MfrWarrantyEnd", "MfrWarrantyStart", "SerialNumber", "PlateNumber", "Color", "Brand", "Condition", "Type", "Power", "Year",
            ],
            columns: [
                {
                    name: "AdmissionDate",
                    template: `<input type="date">`,
                    on: {
                        "change": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "AdmissionDate",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "LotNumber",
                    template: `<input type="text">`,
                    on: {
                        "change": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "LotNumber",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "ExpirationDate",
                    template: `<input type="date">`,
                    on: {
                        "change": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "ExpirationDate",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "MfrDate",
                    template: `<input type="date">`,
                    on: {
                        "change": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "MfrDate",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "ExpirationDate",
                    template: `<input type="date">`,
                    on: {
                        "change": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "ExpirationDate",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "MfrWarrantyEnd",
                    template: `<input type="date">`,
                    on: {
                        "change": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "MfrWarrantyEnd",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "MfrWarrantyStart",
                    template: `<input type="date">`,
                    on: {
                        "change": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "MfrWarrantyStart",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "Detials",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "Detials",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "Location",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "Location",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "MfrSerialNo",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "MfrSerialNo",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "SerialNumber",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "SerialNumber",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "PlateNumber",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "PlateNumber",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "Color",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "Color",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "Brand",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "Brand",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "Condition",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "Condition",
                                this.value
                            );
                        }
                    }
                },

                {
                    name: "Type",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "Type",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "Power",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "Power",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "Year",
                    template: `<input type="text" >`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.serials,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "SerialDetialViewModelPurchase",
                                "Year",
                                this.value
                            );
                        }
                    }
                },
            ],
        });
        serialDetialView.bindRows(serials);
    }
    // Option Automatic Creation //
    function serialOption(serialDialog, data) {
        serialDialog.content.find("#optionCreateSerial-s-pur").prop("disabled", false).click(function () {
            serialOptionsCreation(data);
        });
    }

    function serialOptionsCreation(e) {
        const AutoSerialCreationDialog = new DialogBox({
            content: {
                selector: "#container-serial-option-s-pur"
            },
            caption: "Automatic Serial Creation",
            type: "ok/cancel",
        });
        AutoSerialCreationDialog.invoke(function () {
            const { SerialDetailAutoCreation: sdac } = e.data;
            AutoSerialCreationDialog.content.find("#itemNumber-s-pur").val(sdac.ItemCode);
            AutoSerialCreationDialog.content.find("#itemName-s-pur").val(sdac.ItemName);
            AutoSerialCreationDialog.content.find("#whsCode-s-pur").val(sdac.WhsCode);
            AutoSerialCreationDialog.content.find("#qty-s-pur").val(sdac.Qty);
            AutoSerialCreationDialog.content.find("#mrf-serial-no-s-pur").val(sdac.MfrSerialNo);
            AutoSerialCreationDialog.content.find("#serial-number-s-pur").val(sdac.SerailNumber);
            AutoSerialCreationDialog.content.find("#lot-number-s-pur").val(sdac.LotNumber);
            AutoSerialCreationDialog.content.find("#location-s-pur").val(sdac.Location);
            AutoSerialCreationDialog.content.find("#details-s-pur").val(sdac.Detials);
            AutoSerialCreationDialog.content.find("#cost-s-pur").val(sdac.Cost);
            AutoSerialCreationDialog.content.find("#platenumber").val(sdac.PlateNumber);
            AutoSerialCreationDialog.content.find("#color").val(sdac.Color);
            AutoSerialCreationDialog.content.find("#brand").val(sdac.Brand);
            AutoSerialCreationDialog.content.find("#condition").val(sdac.Condition);
            AutoSerialCreationDialog.content.find("#type").val(sdac.Type);
            AutoSerialCreationDialog.content.find("#power").val(sdac.Power);
            AutoSerialCreationDialog.content.find("#year").val(sdac.Year);

            setDate("#admiddiondate-s-pur", sdac.AdmissionDate);
            setDate("#mfrDate", sdac.MfrDate);
            setDate("#expDate-s-pur", sdac.ExpDate);
            setDate("#mfrwanStart-s-pur", sdac.MfrWanStartDate);
            setDate("#mfrwanEnd-s-pur", sdac.MfrWanEndDate);
            $("#dialog-mfr-serial-no-s-pur").click(function () {
                type = MFR;
                dialogMfrSerialno(e, MFR);
            });
            $("#dialog-serial-number-s-pur").click(function () {
                type = SENUM;
                dialogMfrSerialno(e, SENUM);
            });
            $("#dialog-lot-number-s-pur").click(function () {
                type = LOTNUM;
                dialogMfrSerialno(e, LOTNUM);
            });
        });
        AutoSerialCreationDialog.confirm(function () {
            const { SerialDetailAutoCreation: sdac } = e.data;
            mfrFillInput(sdac);
            $.post("/PurchasePO/CreateSerialAutomatically", { data: JSON.stringify(sdac) }, function (res) {
                updateMAsters(__config.data.serials, "LineID", e.key, "SerialDetialViewModelPurchase", res);
                serialDetialView.bindRows(res);
            });
            serialDialog.shutdown();
            AutoSerialCreationDialog.shutdown();
        });
        AutoSerialCreationDialog.reject(function () {
            AutoSerialCreationDialog.shutdown();
        })
    }
    function serialOptionTable(serialOptionDialog, serailOptions, type) {
        const serialOptionView = ViewTable({
            keyField: "LineID",
            selector: serialOptionDialog.content.find(".serial-option-s-pur"),
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: ["Name", "Type", "Operation"],
            columns: [
                {
                    name: "Name",
                    template: `<input data-number="false"/>`,
                    on: {
                        "keyup": function (e) {
                            if ($(this).data('number')) {
                                numberValidation(this);
                            }
                            if (type === MFR) {
                                updateMAsters(
                                    __infos.serialStringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    this.value
                                );
                                bindFinalString(__infos.serialStringCreation);
                            }
                            if (type === SENUM) {
                                updateMAsters(
                                    __infos.sSerialStringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    this.value
                                );
                                bindFinalString(__infos.sSerialStringCreation);
                            }
                            if (type === LOTNUM) {
                                updateMAsters(
                                    __infos.lotSerialStringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    this.value
                                );
                                bindFinalString(__infos.lotSerialStringCreation);
                            }
                        }
                    }
                },
                {
                    name: "Type",
                    template: `<select></select>`,
                    on: {
                        "change": function (e) {
                            const colTd = serialOptionView.getColumn(e.key, "Name");
                            var input = $(colTd).children("input");
                            if (this.value > 1) {
                                serialOptionView.disableColumns(e.key, ["Operation"], false);
                                input.focus().data("number", true);
                            }
                            if (this.value == 1) {
                                serialOptionView.disableColumns(e.key, ["Operation"]);
                                input.focus().data("number", false);
                            }
                            if (type === MFR) {
                                updateMAsters(
                                    __infos.serialStringCreation,
                                    "LineID", e.key,
                                    "TypeInt",
                                    this.value
                                );
                                $(colTd).children("input").val("");
                                updateMAsters(
                                    __infos.serialStringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    ""
                                );
                                bindFinalString(__infos.serialStringCreation);
                            }
                            if (type === SENUM) {
                                updateMAsters(
                                    __infos.sSerialStringCreation,
                                    "LineID", e.key,
                                    "TypeInt",
                                    this.value
                                );
                                $(colTd).children("input").val("");
                                updateMAsters(
                                    __infos.sSerialStringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    ""
                                );
                                bindFinalString(__infos.sSerialStringCreation);
                            }
                            if (type === LOTNUM) {
                                updateMAsters(
                                    __infos.lotSerialStringCreation,
                                    "LineID", e.key,
                                    "TypeInt",
                                    this.value
                                );
                                $(colTd).children("input").val("");
                                updateMAsters(
                                    __infos.lotSerialStringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    ""
                                );
                                bindFinalString(__infos.lotSerialStringCreation);
                            }
                            updateSelect(e.data.Type, this.value, "Selected");
                        }
                    }
                },
                {
                    name: "Operation",
                    template: `<select></select>`,
                    on: {
                        "change": function (e) {
                            if (type === MFR) {
                                updateMAsters(
                                    __infos.serialStringCreation,
                                    "LineID",
                                    e.key,
                                    "OperationInt",
                                    this.value
                                );
                            }
                            if (type === SENUM) {
                                updateMAsters(
                                    __infos.sSerialStringCreation,
                                    "LineID",
                                    e.key,
                                    "OperationInt",
                                    this.value
                                );
                            }
                            if (type === LOTNUM) {
                                updateMAsters(
                                    __infos.lotSerialStringCreation,
                                    "LineID",
                                    e.key,
                                    "OperationInt",
                                    this.value
                                );
                            }
                            updateSelect(e.data.Operation, this.value, "Selected");
                        }
                    }
                }
            ],
        });
        serialOptionView.bindRows(serailOptions);
        if (type === MFR) __infos.serialStringCreation = serialOptionView.yield();
        if (type === SENUM) __infos.sSerialStringCreation = serialOptionView.yield();
        if (type === LOTNUM) __infos.lotSerialStringCreation = serialOptionView.yield();
        serialOptionView.disableColumns(undefined, ["Operation"]);
    }
    function dialogMfrSerialno(e, type) {
        const {
            SerialDetailAutoCreation: sdac,
            AutomaticStringCreations: asc,
            SerialAutomaticStringCreations: sasc,
            LotAutomaticStringCreations: lasc
        } = e.data;
        const mfrSerialnoDialog = new DialogBox({
            content: {
                selector: "#container-string-option-s-pur"
            },
            caption: "Automatic String Creation",
        });
        mfrSerialnoDialog.invoke(function () {

            if (type === MFR) {
                serialOptionTable(mfrSerialnoDialog, asc, type);
                $("#finalString-s-pur").val(sdac.MfrSerialNo);
            };
            if (type === SENUM) {
                serialOptionTable(mfrSerialnoDialog, sasc, type);
                $("#finalString-s-pur").val(sdac.SerailNumber);
            }
            if (type === LOTNUM) {
                serialOptionTable(mfrSerialnoDialog, lasc, type);
                $("#finalString-s-pur").val(sdac.LotNumber);
            }
        });
        mfrSerialnoDialog.confirm(function () {
            if (type === MFR) {
                sdac.MfrSerialNo = $("#finalString-s-pur").val();
                $("#mrf-serial-no-s-pur").val(sdac.MfrSerialNo);
                checkAutoStringCreation(mfrSerialnoDialog, __infos.serialStringCreation);
                sdac.AutomaticStringCreations = __infos.serialStringCreation;
            }
            if (type === SENUM) {
                sdac.SerailNumber = $("#finalString-s-pur").val();
                $("#serial-number-s-pur").val(sdac.SerailNumber);
                checkAutoStringCreation(mfrSerialnoDialog, __infos.sSerialStringCreation);
                sdac.SerialAutomaticStringCreations = __infos.sSerialStringCreation;
            }
            if (type === LOTNUM) {
                sdac.LotNumber = $("#finalString-s-pur").val();
                $("#lot-number-s-pur").val(sdac.LotNumber);
                checkAutoStringCreation(mfrSerialnoDialog, __infos.lotSerialStringCreation);
                sdac.LotAutomaticStringCreations = __infos.lotSerialStringCreation;
            }
        });
    }
    function mfrFillInput(sdac) {
        sdac.AdmissionDate = $("#admiddiondate-s-pur").val();
        sdac.MfrDate = $("#mfrDate").val();
        sdac.ExpDate = $("#expDate-s-pur").val();
        sdac.MfrWanStartDate = $("#mfrwanStart-s-pur").val();
        sdac.MfrWanEndDate = $("#mfrwanEnd-s-pur").val();
        sdac.MfrWanEndDate = $("#mfrwanEnd-s-pur").val();
        sdac.Location = $("#location-s-pur").val();
        sdac.Detials = $("#details-s-pur").val();
        sdac.PlateNumber = $("#platenumber").val();

        sdac.Color = $("#color").val();
        sdac.Brand = $("#brand").val();
        sdac.Condition = $("#condition").val();
        sdac.Type = $("#type").val();
        sdac.Power = $("#power").val();
        sdac.Year = $("#year").val();


    }
    function bindFinalString(data) {
        if (isValidArray(data)) {
            const finalString = `${data[0].Name.trim()}${data[1].Name.trim()}`;
            $("#finalString-s-pur").val(finalString);
        }
    }
    function checkAutoStringCreation(mfrSerialnoDialog, data) {
        $.post(
            "/PurchasePO/CheckAutoStringCreation",
            { autoStringCreation: JSON.stringify(data) },
            function (res) {
                if (res.IsApproved) {
                    $(".autoStringCreationMessage-s-pur").text("");
                    mfrSerialnoDialog.shutdown();
                } else {
                    new ViewMessage({
                        summary: {
                            selector: ".autoStringCreationMessage-s-pur"
                        }
                    }, res);
                }
            })
    }
    // utils //
    function numberValidation(input) {
        if (input.value == "" || isNaN(parseFloat(input.value))) {
            input.value = "";
        }
        if (input.value.endsWith("-")) {
            input.value = input.value.toString().split("-")[0];
        }

        if (input.value.startsWith("--")) {
            input.value = input.value.toString().substring(2, input.value.length - 1);
        }
        if (input.value.includes(".")) {
            input.value = input.value.toString().split(".")[0];
        }
        input.value = $.validNumber(input.value).toString();
    }
    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.keys(value).length > 0;
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function setDate(selector, date_value) {
        if (date_value) {
            var _date = $(selector);
            _date[0].valueAsDate = new Date(date_value.split("T")[0]);
            _date[0].setAttribute(
                "data-date",
                moment(_date[0].value)
                    .format(_date[0].getAttribute("data-date-format"))
            );
        }
    }
    function updateDetails(data, masterKey, masterValue, dKey, dValue, dProp, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[masterKey] === masterValue) {
                    if (isValidArray(i[dProp])) {
                        i[dProp].forEach(j => {
                            if (j[dKey] === dValue) {
                                j[prop] = propValue;
                            }
                        });
                    }
                }
            });
        }
    }
    function updateMAsters(data, masterKey, masterValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[masterKey] === masterValue) {
                    i[prop] = propValue;
                }
            });
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
}
