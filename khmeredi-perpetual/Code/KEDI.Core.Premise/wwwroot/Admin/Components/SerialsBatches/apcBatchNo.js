function APCBatchNoTemplate(config) {
    if (!(this instanceof APCBatchNoTemplate)) {
        return new APCBatchNoTemplate(config);
    }

    var __config = {
        data: {
            batches: new Array(),
        },
    }
    const __infos = {
        batches: new Array(),
        param: {
            itemId: 0,
            bpId: 0
        }
    }

    if (isValidJSON(config)) {
        __config = $.extend(true, __config, config);
    }

    this.batchTemplate = function () {
        const batchDialog = new DialogBox({
            content: {
                selector: "#container-list-batch"
            },
            caption: "Batches Selection",
            type: "ok/cancel",
        });

        batchDialog.invoke(function () {
            batchDialogBox(batchDialog, __config.data.batches);
        });
        batchDialog.confirm(function () {
            $.post("/PurchaseCreditMemo/CheckBatchNo", { batches: JSON.stringify(__infos.batches) }, function (res) {
                if (res.IsApproved) {
                    $(".serailErrorMessage").text("");
                    batchDialog.shutdown();
                } else {
                    new ViewMessage({
                        summary: {
                            selector: ".serailErrorMessage"
                        }
                    }, res);
                }
            });
        });
        batchDialog.reject(function () {
            batchDialog.shutdown()
            $(".serailErrorMessage").text("")
        });
    }
    this.callbackInfo = function () {
        return __infos;
    }

    // Serial Number //
    function batchDialogBox(batchDialog, batches) {
        const batchView = ViewTable({
            keyField: "LineID",
            selector: batchDialog.content.find(".batch-lists"),
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: [
                "Direction", "ItemCode", "ItemName", "TotalBatches", "Qty", "TotalSelected",
                "WhsCode", "WhsName"
            ],
            columns: [
                {
                    name: "ItemCode",
                    on: {
                        "dblclick": function (e) {
                            __infos.param = {
                                itemId: e.data.ItemID,
                                bpId: e.data.BpId
                            }
                            if (!e.data.APCBatchNoSelected.APCBatchNoSelectedDetails) {
                                fetchBatchNoDetials(e.data, function (res) {
                                    batchDialog.content.find("#avbQty").val(res.TotalAvailableQty);
                                    updateMaster(__infos.batches, "LineID", e.key, "APCBatchNoUnselect", res);
                                    detialTable(batchDialog, batchView, e.data);
                                });
                            } else {
                                const batchDetial = e.data.APCBatchNoUnselect;
                                batchDialog.content.find("#avbQty").val(batchDetial.TotalAvailableQty);
                                updateMaster(__infos.batches, "LineID", e.key, "APCBatchNoUnselect", batchDetial);
                                detialTable(batchDialog, batchView, e.data);
                            }

                        }
                    }
                }
            ],
        });
        batchView.bindRows(batches);
        __infos.batches = batchView.yield();
    }
    function fetchBatchNoDetials(data, success) {
        $.get("/PurchaseCreditMemo/GetBatchNoDetials", { cost: data.Cost, bpId: data.BpId, itemId: data.ItemID, uomID: data.UomID }, success);
    }

    function detialTable(batchDialog, masterTable, masterData) {
        const _data = find("LineID", masterData.LineID, __infos.batches);
        let batchDetialUnselectView = ViewTable({});
        const batchDetialSelectView = ViewTable({
            keyField: "BatchNo",
            selector: batchDialog.content.find(".batch-select-lists"),
            //indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: ["BatchNo", "SelectedQty", "UnitCost"],
            columns: [
                {
                    name: "BatchNo",
                    on: {
                        "dblclick": function (e) {
                            batchDetialSelectView.removeRow(e.key);
                            let totalSelected = parseFloat(_data.APCBatchNoSelected.TotalSelected)
                            let avqty = parseFloat(_data.APCBatchNoUnselect.TotalAvailableQty)
                            totalSelected -= 1;
                            avqty += 1;
                            //Update Master Serial
                            const openQty = masterData.TotalNeeded + e.data.SelectedQty;
                            const selectedQty = masterData.TotalSelected - e.data.SelectedQty;
                            updateMaster(__infos.batches, "LineID", masterData.LineID, "TotalSelected", selectedQty);
                            updateMaster(__infos.batches, "LineID", masterData.LineID, "TotalNeeded", openQty);
                            masterTable.updateColumn(masterData.LineID, "TotalNeeded", openQty);
                            masterTable.updateColumn(masterData.LineID, "TotalSelected", selectedQty);
                            const unselected = find("BatchNo", e.key, _data.APCBatchNoUnselect.APCBatchNoUnselectDetials);
                            if (unselected) {
                                const availableQty = unselected.AvailableQty + e.data.SelectedQty;
                                updateMaster(_data.APCBatchNoUnselect.APCBatchNoUnselectDetials, "BatchNo", e.key, "SelectedQty", 0);
                                batchDetialUnselectView.updateColumn(unselected.BatchNo, "AvailableQty", availableQty);
                                batchDetialUnselectView.updateColumn(unselected.BatchNo, "SelectedQty", 0);
                            }
                            else {
                                data.push(e.data)
                                data.sort((a, b) => a.BatchNo < b.BatchNo && 1 || -1);
                                batchDetialUnselectView.bindRows(data);
                            }
                            const totalBatches = batchDetialSelectView.yield().length;
                            updateMaster(__infos.batches, "LineID", masterData.LineID, "TotalBatches", totalBatches);
                            masterTable.updateColumn(masterData.LineID, "TotalBatches", totalBatches);
                            batchDialog.content.find("#avbQty").val(avqty);
                            batchDialog.content.find("#ttSlt").val(totalSelected);
                            const batchObj = {
                                TotalSelected: totalSelected,
                                APCBatchNoSelectedDetails: batchDetialSelectView.yield(),
                            }
                            const batchObjUn = {
                                TotalAvailableQty: avqty,
                                APCBatchNoUnselectDetials: batchDetialUnselectView.yield(),
                            }
                            updateMaster(__infos.batches, "LineID", masterData.LineID, "APCBatchNoUnselect", batchObjUn);
                            updateMaster(__infos.batches, "LineID", masterData.LineID, "APCBatchNoSelected", batchObj);
                        }
                    }
                }
            ],
        });
        batchDetialSelectView.clearRows();
        batchDetialSelectView.bindRows(_data.APCBatchNoSelected.APCBatchNoSelectedDetails);
        batchDetialUnselectView = ViewTable({
            keyField: "BatchNo",
            selector: batchDialog.content.find(".batch-unselect-lists"),
            //indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: [
                "BatchNo", "AvailableQty", "SelectedQty", "UnitCost"
            ],
            columns: [
                {
                    name: "SelectedQty",
                    template: `<input />`,
                    on: {
                        "keyup": function (e) {
                            $(this).asNumber();
                            if (this.value == "" || parseFloat(this.value) < 0) this.value = 0;
                            if (parseFloat(this.value) > e.data.AvailableQty) this.value = e.data.AvailableQty;
                            updateMaster(_data.APCBatchNoUnselect.APCBatchNoUnselectDetials, "BatchNo", e.key, "SelectedQty", parseFloat(this.value));
                        }
                    }
                },
                {
                    name: "BatchNo",
                    on: {
                        "dblclick": function (e) {
                            if (e.data.SelectedQty > 0) {
                                if (masterData.TotalSelected < masterData.Qty && masterData.TotalNeeded >= e.data.SelectedQty) {
                                    if (e.data.AvailableQty == e.data.SelectedQty) {
                                        batchDetialUnselectView.removeRow(e.key);
                                    }
                                    const availableQty = e.data.AvailableQty - e.data.SelectedQty;
                                    data = batchDetialUnselectView.yield();
                                    batchDetialUnselectView.updateColumn(e.key, "AvailableQty", availableQty);
                                    let totalSelected = parseFloat(_data.APCBatchNoSelected.TotalSelected)
                                    let avqty = parseFloat(_data.APCBatchNoUnselect.TotalAvailableQty)
                                    totalSelected += 1;
                                    avqty -= 1;
                                    //Update Master Serial
                                    const openQty = masterData.TotalNeeded - parseFloat(e.data.SelectedQty);
                                    const selectedQty = masterData.TotalSelected + parseFloat(e.data.SelectedQty);
                                    updateMaster(__infos.batches, "LineID", masterData.LineID, "TotalSelected", selectedQty);
                                    updateMaster(__infos.batches, "LineID", masterData.LineID, "TotalNeeded", openQty);
                                    masterTable.updateColumn(masterData.LineID, "TotalNeeded", openQty);
                                    masterTable.updateColumn(masterData.LineID, "TotalSelected", selectedQty);
                                    const batchSelected = find("BatchNo", e.data.BatchNo, _data.APCBatchNoSelected.APCBatchNoSelectedDetails);

                                    if (batchSelected) {
                                        const selectedQty = batchSelected.SelectedQty + e.data.SelectedQty;
                                        batchDetialSelectView.updateColumn(batchSelected.BatchNo, "SelectedQty", selectedQty);
                                    } else {
                                        //__infos.selectedDetial.APCBatchNoSelectedDetails.push(e.data);
                                        batchDetialSelectView.addRow(e.data);
                                        _data.APCBatchNoSelected.APCBatchNoSelectedDetails = batchDetialSelectView.yield();
                                    }
                                    
                                    const batchObj = {
                                        TotalSelected: totalSelected,
                                        APCBatchNoSelectedDetails: batchDetialSelectView.yield(),
                                    }
                                    const batchObjUn = {
                                        TotalAvailableQty: avqty,
                                        APCBatchNoUnselectDetials: batchDetialUnselectView.yield(),
                                    }
                                    updateMaster(__infos.batches, "LineID", masterData.LineID, "APCBatchNoUnselect", batchObjUn);
                                    updateMaster(__infos.batches, "LineID", masterData.LineID, "APCBatchNoSelected", batchObj);
                                    const totalBatches = batchDetialSelectView.yield().length;
                                    updateMaster(__infos.batches, "LineID", masterData.LineID, "TotalBatches", totalBatches);
                                    masterTable.updateColumn(masterData.LineID, "TotalBatches", totalBatches);
                                    //updateMaster(__infos.batches, "LineID", masterData.LineID, "APCBatchNoSelected", __infos.selectedDetial);
                                    batchDialog.content.find("#avbQty").val(avqty);
                                    batchDialog.content.find("#ttSlt").val(totalSelected);
                                }
                                else if (masterData.TotalNeeded < e.data.SelectedQty) {
                                    new DialogBox({
                                        content: "Total Created is over Total Needed!",
                                        type: "ok",
                                        icon: "warning"
                                    });
                                }
                                else if (masterData.TotalNeeded === 0) {
                                    new DialogBox({
                                        content: "Qty is equal to Total Selected Qty",
                                        type: "ok",
                                        icon: "warning"
                                    });
                                    //dialogSubmit.confirm(function () { dialogSubmit.shutdown() });
                                }
                            }
                            else if (e.data.SelectedQty === 0) {
                                new DialogBox({
                                    content: "Selected Qty has to be greater than 0!",
                                    type: "ok",
                                    icon: "warning"
                                });
                            }
                        }
                    }
                }
            ],
        });
        batchDetialUnselectView.bindRows(_data.APCBatchNoUnselect.APCBatchNoUnselectDetials);
        $("#searchBatchNo").keyup(function () {
            let __value = this.value.toLowerCase().replace(/\s+/, "");
            let rex = new RegExp(__value, "gi");
            let items = $.grep(data, function (item) {
                return item.UnitCost === __value || item.BatchNo.toLowerCase().replace(/\s+/, "").match(rex) ||
                    item.AvailableQty === __value
            });
            batchDetialUnselectView.bindRows(items);
        });
    }
    // utils //
    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.keys(value).length > 0;
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function updateMaster(data, masterKey, masterValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[masterKey] === masterValue) {
                    i[prop] = propValue;
                }
            });
        }
    }
    function find(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
}
