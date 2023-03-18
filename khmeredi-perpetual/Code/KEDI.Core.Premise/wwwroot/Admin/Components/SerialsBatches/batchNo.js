function BatchTemplatePur(config) {
    if (!(this instanceof BatchTemplatePur)) {
        return new BatchTemplatePur(config);
    }
    // Type //
    const BATCH = "batch";
    const BAATRR = "baatrr";
    const BAATRR2 = "baatrr2";
    var __config = {
        data: {
            batches: new Array(),
        },
    }
    const __infos = {
        batches: new Array(),
        batchStringCreation: new Array(),
        batchAttr1StringCreation: new Array(),
        batchAttr2StringCreation: new Array(),
    }
    if (isValidJSON(config)) {
        __config = $.extend(true, __config, config);
    }
    const autoBatchCreationDialog = new DialogBox({
        content: {
            selector: "#container-batch-option-pur"
        },
        caption: "Automatic Batch Creation",
        type: "ok/cancel",
    });
    this.batchTemplate = function () {
        const batchDialog = new DialogBox({
            content: {
                selector: "#container-list-batch-pur"
            },
            caption: "Batches - SetUp",
            type: "ok-cancel"
        });
        batchDialog.invoke(function () {
            batchDialogBox(batchDialog, __config.data.batches)
        });
        batchDialog.confirm(function () {
            $.post("/PurchasePO/CheckBatchNumber", { batches: JSON.stringify(__config.data.batches) }, function (res) {
                if (res.IsApproved) {
                    $(".batchErrorMessage-pur").text("");
                    autoBatchCreationDialog.shutdown();
                    batchDialog.shutdown();
                } else {
                    new ViewMessage({
                        summary: {
                            selector: ".batchErrorMessage-pur"
                        }
                    }, res);
                }
            });
        });
        batchDialog.reject(function () {
            $(".batchErrorMessage-pur").text("");
            batchDialog.shutdown();
            autoBatchCreationDialog.shutdown();
        });
    }
    this.callbackInfo = function () {
        return __infos;
    }
    // Batches //
    let batchDetialView = ViewTable({});
    function batchDialogBox(batchDialog, batches) {
        const batchView = ViewTable({
            keyField: "LineID",
            selector: batchDialog.content.find(".batch-lists-pur"),
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: [
                "DocNo", "ItemCode", "ItemDescription", "TotalCreated", "TotalNeeded",
                "WhseCode", "WhseName"
            ],
            columns: [
                {
                    name: "DocNo",
                    on: {
                        "dblclick": function (e) {
                            bindBatchDetails(batchDialog, e.data.BatchDetialViewModelPurchases, e)
                            batchOption(batchDialog, e);
                            __infos.batchStringCreation = e.data.BatchStringCreations;
                        }
                    }
                }
            ],
        });
        batchView.bindRows(batches);
        __infos.batches = batchView.yield();
        //
    }
    function bindBatchDetails(batchDialog, batches, _e) {
        batchDetialView = ViewTable({
            keyField: "LineID",
            selector: batchDialog.content.find(".batch-detail-lists-pur"),
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: [
                "Batch", "Qty", "BatchAttribute1", "BatchAttribute2", "ExpirationDate", "MfrDate",
                "AdmissionDate", "Location", "Detials", "UnitPrice"
            ],
            columns: [
                {
                    name: "Batch",
                    template: `<input type="text">`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.batches,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "BatchDetialViewModelPurchases",
                                "Batch",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "Qty",
                    template: `<input type="text">`,
                    on: {
                        "keyup": function (e) {
                            $(this).asNumber();                           
                            //const batch = find("LineID", _e.key, __infos.batches);
                            //if (!!batch) {
                            //    sumQtyCheck(this, _e.data.TotalNeeded, batch.BatchDetialViewModelPurchases);
                            //}
                            updateDetails(
                                __infos.batches,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "BatchDetialViewModelPurchases",
                                "Qty",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "BatchAttribute1",
                    template: `<input type="text">`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.batches,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "BatchDetialViewModelPurchases",
                                "BatchAttribute1",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "BatchAttribute2",
                    template: `<input type="text">`,
                    on: {
                        "keyup": function (e) {
                            updateDetails(
                                __infos.batches,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "BatchDetialViewModelPurchases",
                                "BatchAttribute2",
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
                                __infos.batches,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "BatchDetialViewModelPurchases",
                                "ExpirationDate",
                                this.value
                            );
                        }
                    }
                },
                {
                    name: "AdmissionDate",
                    template: `<input type="date">`,
                    on: {
                        "change": function (e) {
                            updateDetails(
                                __infos.batches,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "BatchDetialViewModelPurchases",
                                "AdmissionDate",
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
                                __infos.batches,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "BatchDetialViewModelPurchases",
                                "MfrDate",
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
                                __infos.batches,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "BatchDetialViewModelPurchases",
                                "Details",
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
                                __infos.batches,
                                "LineID",
                                _e.key,
                                "LineID",
                                e.key,
                                "BatchDetialViewModelPurchases",
                                "Location",
                                this.value
                            );
                        }
                    }
                }
            ],
        });
        batchDetialView.bindRows(batches);
    }
    // Option Automatic Creation //
    function batchOption(batchDialog, data) {
        batchDialog.content.find("#optionCreateBatch-pur")
            .prop("disabled", false)
            .click(function () {
                batchOptionsCreation(data);
            });
    }
    function batchOptionTable(batchOptionDialog, batchOptions, type) {
        const batchOptionView = ViewTable({
            keyField: "LineID",
            selector: batchOptionDialog.content.find(".batch-option-pur"),
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
                            if (type === BATCH) {
                                updateMAsters(
                                    __infos.batchStringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    this.value
                                );
                                bindFinalString(__infos.batchStringCreation);
                            }
                            if (type === BAATRR) {
                                updateMAsters(
                                    __infos.batchAttr1StringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    this.value
                                );
                                bindFinalString(__infos.batchAttr1StringCreation);
                            }
                            if (type === BAATRR2) {
                                updateMAsters(
                                    __infos.batchAttr2StringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    this.value
                                );
                                bindFinalString(__infos.batchAttr2StringCreation);
                            }
                        }
                    }
                },
                {
                    name: "Type",
                    template: `<select></select>`,
                    on: {
                        "change": function (e) {
                            const colTd = batchOptionView.getColumn(e.key, "Name");
                            var input = $(colTd).children("input");
                            if (this.value > 1) {
                                batchOptionView.disableColumns(e.key, ["Operation"], false);
                                input.focus().data("number", true);
                            }
                            if (this.value == 1) {
                                batchOptionView.disableColumns(e.key, ["Operation"]);
                                input.focus().data("number", false);
                            }
                            if (type === BATCH) {
                                updateMAsters(
                                    __infos.batchStringCreation,
                                    "LineID", e.key,
                                    "TypeInt",
                                    this.value
                                );
                                $(colTd).children("input").val("");
                                updateMAsters(
                                    __infos.batchStringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    ""
                                );
                                bindFinalString(__infos.batchStringCreation);
                            }
                            if (type === BAATRR) {
                                updateMAsters(
                                    __infos.batchAttr1StringCreation,
                                    "LineID", e.key,
                                    "TypeInt",
                                    this.value
                                );
                                $(colTd).children("input").val("");
                                updateMAsters(
                                    __infos.batchAttr1StringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    ""
                                );
                                bindFinalString(__infos.batchAttr1StringCreation);
                            }
                            if (type === BAATRR2) {
                                updateMAsters(
                                    __infos.batchAttr2StringCreation,
                                    "LineID", e.key,
                                    "TypeInt",
                                    this.value
                                );
                                $(colTd).children("input").val("");
                                updateMAsters(
                                    __infos.batchAttr2StringCreation,
                                    "LineID",
                                    e.key,
                                    "Name",
                                    ""
                                );
                                bindFinalString(__infos.batchAttr2StringCreation);
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
                            if (type === BATCH) {
                                updateMAsters(
                                    __infos.batchStringCreation,
                                    "LineID",
                                    e.key,
                                    "OperationInt",
                                    this.value
                                );
                            }
                            if (type === BAATRR) {
                                updateMAsters(
                                    __infos.batchAttr1StringCreation,
                                    "LineID",
                                    e.key,
                                    "OperationInt",
                                    this.value
                                );
                            }
                            if (type === BAATRR2) {
                                updateMAsters(
                                    __infos.batchAttr2StringCreation,
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
        batchOptionView.bindRows(batchOptions);
        if (type === BATCH) __infos.batchStringCreation = batchOptionView.yield();
        if (type === BAATRR) __infos.batchAttr1StringCreation = batchOptionView.yield();
        if (type === BAATRR2) __infos.batchAttr2StringCreation = batchOptionView.yield();
        batchOptionView.disableColumns(undefined, ["Operation"]);
    }
    function dialogMfrSerialno(e, type) {
        const {
            BatchDetialViewModelPurchases: sdac,
            BatchStringCreations: asc,
            Batchtrr1StringCreations: sasc,
            Batchtrr2StringCreations: lasc
        } = e.data;
        const batchNoDialog = new DialogBox({
            content: {
                selector: "#container-batch-string-option-pur"
            },
            caption: "Automatic String Creation",
        });
        batchNoDialog.invoke(function () {

            if (type === BATCH) {
                serialOptionTable(batchNoDialog, asc, type);
                $("#finalString-pur").val(sdac.MfrSerialNo);
            };
            if (type === BAATRR) {
                serialOptionTable(batchNoDialog, sasc, type);
                $("#finalString-pur").val(sdac.SerailNumber);
            }
            if (type === BAATRR2) {
                serialOptionTable(batchNoDialog, lasc, type);
                $("#finalString-pur").val(sdac.LotNumber);
            }
        });
        batchNoDialog.confirm(function () {
            if (type === BATCH) {
                sdac.MfrSerialNo = $("#finalString-pur").val();
                $("#batch-pur").val(sdac.MfrSerialNo);
                checkAutoStringCreation(batchNoDialog, __infos.batchStringCreation);
                sdac.BatchStringCreations = __infos.batchStringCreation;
            }
            if (type === BAATRR) {
                sdac.SerailNumber = $("#finalString-pur").val();
                $("#batchAttr2-pur").val(sdac.SerailNumber);
                checkAutoStringCreation(batchNoDialog, __infos.batchAttr1StringCreation);
                sdac.Batchtrr1StringCreations = __infos.batchAttr1StringCreation;
            }
            if (type === BAATRR2) {
                sdac.LotNumber = $("#finalString-pur").val();
                $("#batchAttr1-pur").val(sdac.LotNumber);
                checkAutoStringCreation(batchNoDialog, __infos.batchAttr2StringCreation);
                sdac.Batchtrr2StringCreations = __infos.batchAttr2StringCreation;
            }
        });
    }
    function batchOptionsCreation(e) {
        autoBatchCreationDialog.invoke(function () {
            const { BatchDetailAutoCreation: bdac } = e.data;
            
            autoBatchCreationDialog.content.find("#itemNumber-pur").val(bdac.ItemCode);
            autoBatchCreationDialog.content.find("#itemName-pur").val(bdac.ItemName);
            autoBatchCreationDialog.content.find("#whsCode-pur").val(bdac.WhsCode);
            autoBatchCreationDialog.content.find("#qty-pur").val(bdac.Qty);
            autoBatchCreationDialog.content.find("#noOfqty-pur").val(bdac.NoOfBatch);
            autoBatchCreationDialog.content.find("#batch-pur").val(bdac.Batch);
            autoBatchCreationDialog.content.find("#batchAttr2-pur").val(bdac.BatchAtrr2);
            autoBatchCreationDialog.content.find("#batchAttr1-pur").val(bdac.BatchAtrr1);
            autoBatchCreationDialog.content.find("#location-pur").val(bdac.Location);
            autoBatchCreationDialog.content.find("#details-pur").val(bdac.Detials);
            autoBatchCreationDialog.content.find("#cost-pur").val(bdac.Cost);
            setDate("#admiddiondate-pur", bdac.AdmissionDate);
            setDate("#mfrDate", bdac.MfrDate);
            setDate("#expDate", bdac.ExpDate);
            $("#noOfqty-pur").keyup(function () {
                if (this.value > e.data.TotalNeeded) this.value = e.data.TotalNeeded
                bdac.NoOfBatch = this.value;
            });
            $("#dialog-batch-pur").click(function () {
                type = BATCH;
                dialogMfrSerialno(e, BATCH);
            });
            $("#dialog-batchAttr2-pur").click(function () {
                type = BAATRR;
                dialogMfrSerialno(e, BAATRR);
            });
            $("#dialog-batchAttr1-pur").click(function () {
                type = BAATRR2;
                dialogMfrSerialno(e, BAATRR2);
            });
        });
        autoBatchCreationDialog.confirm(function () {
            const { BatchDetailAutoCreation: bdac } = e.data;
            mfrFillInput(bdac);
            $.post("/PurchasePO/CreateBatchAutomatically", { data: JSON.stringify(bdac) }, function (res) {
                updateMAsters(__config.data.batches, "LineID", e.key, "BatchDetialViewModelPurchases", res);
                batchDetialView.bindRows(res);
            });
            autoBatchCreationDialog.shutdown();
        });
        autoBatchCreationDialog.reject(function () {
            autoBatchCreationDialog.shutdown();
        })
    }
    function dialogMfrSerialno(e, type) {
        const {
            BatchDetailAutoCreation: bdac,
            BatchStringCreations: bsc,
            Batchtrr1StringCreations: basc1,
            Batchtrr2StringCreations: basc2
        } = e.data;
        const batchNoDialog = new DialogBox({
            content: {
                selector: "#container-batch-string-option-pur"
            },
            caption: "Automatic String Creation",
        });
        batchNoDialog.invoke(function () {

            if (type === BATCH) {
                batchOptionTable(batchNoDialog, bsc, type);
                $("#finalString-pur").val(bdac.Batch);
            };
            if (type === BAATRR) {
                batchOptionTable(batchNoDialog, basc1, type);
                $("#finalString-pur").val(bdac.BatchAttribute1);
            }
            if (type === BAATRR2) {
                batchOptionTable(batchNoDialog, basc2, type);
                $("#finalString-pur").val(bdac.BatchAttribute2);
            }
        });
        batchNoDialog.confirm(function () {
            if (type === BATCH) {
                bdac.Batch = $("#finalString-pur").val();
                $("#batch-pur").val(bdac.Batch);
                checkAutoStringCreation(batchNoDialog, __infos.batchStringCreation);
                bdac.BatchStringCreations = __infos.batchStringCreation;
            }
            if (type === BAATRR) {
                bdac.BatchAttribute1 = $("#finalString-pur").val();
                $("#batchAttr2-pur").val(bdac.BatchAttribute1);
                checkAutoStringCreation(batchNoDialog, __infos.batchAttr1StringCreation);
                bdac.Batchtrr1StringCreations = __infos.batchAttr1StringCreation;
            }
            if (type === BAATRR2) {
                bdac.BatchAttribute2 = $("#finalString-pur").val();
                $("#batchAttr1-pur").val(bdac.BatchAttribute2);
                checkAutoStringCreation(batchNoDialog, __infos.batchAttr2StringCreation);
                bdac.Batchtrr2StringCreations = __infos.batchAttr2StringCreation;
            }
        });
    }
    function mfrFillInput(sdac) {
        sdac.AdmissionDate = $("#admiddiondate-pur").val();
        sdac.MfrDate = $("#mfrDate-pur").val();
        sdac.ExpDate = $("#expDate-pur").val();
        sdac.MfrWanStartDate = $("#mfrwanStart-pur").val();
        sdac.MfrWanEndDate = $("#mfrwanEnd-pur").val();
        sdac.MfrWanEndDate = $("#mfrwanEnd-pur").val();
        sdac.Location = $("#location-pur-pur").val();
        sdac.Detials = $("#details-pur").val();

    }
    function bindFinalString(data) {
        if (isValidArray(data)) {
            const finalString = `${data[0].Name.trim()}${data[1].Name.trim()}`;
            $("#finalString-pur").val(finalString);
        }
    }
    function checkAutoStringCreation(batchNoDialog, data) {
        $.post(
            "/PurchasePO/CheckAutoStringCreation",
            { autoStringCreation: JSON.stringify(data) },
            function (res) {
                if (res.IsApproved) {
                    $(".autoStringCreationMessage-pur").text("");
                    batchNoDialog.shutdown();
                } else {
                    new ViewMessage({
                        summary: {
                            selector: ".autoStringCreationMessage-pur"
                        }
                    }, res);
                }
            })
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
            })
        }
    }
    function find(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
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
    //function sumQtyCheck(_this, qty, data) {
    //    let sumqty = 0;
    //    if (isValidArray(data)) {
    //        data.forEach(i => {
    //            qty -= parseFloat(i.Qty);
    //        });
    //    }
    //    //if (qty < 0) qty * -1;
    //    if (_this.value > qty) _this.value = qty
    //    //return { sumqty, qty };
    //}
}