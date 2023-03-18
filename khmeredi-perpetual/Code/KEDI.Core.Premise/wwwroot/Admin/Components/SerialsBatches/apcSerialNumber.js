function APCSerialTemplate(config) {
    if (!(this instanceof APCSerialTemplate)) {
        return new APCSerialTemplate(config);
    }

    var __config = {
        data: {
            purData: {},
            serials: new Array(),
        },
    }
    const __infos = {
        serials: new Array(),
        param: {
            itemId: 0,
            bpId: 0,
        }
    }

    if (isValidJSON(config)) {
        __config = $.extend(true, __config, config);
    }

    this.serialTemplate = function () {
        const serialDialog = new DialogBox({
            content: {
                selector: "#container-list-serial"
            },
            caption: "Serial Number Selection",
            type: "ok/cancel",
        });

        serialDialog.invoke(function () {



            serialDialogBox(serialDialog, __config.data.serials);


        });
        serialDialog.confirm(function () {

            $.post("/PurchaseCreditMemo/CheckSerailNumber", { serails: JSON.stringify(__infos.serials) }, function (res) {
                if (res.IsApproved) {
                    $(".serailErrorMessage").text("");
                    serialDialog.shutdown();
                } else {
                    new ViewMessage({
                        summary: {
                            selector: ".serailErrorMessage"
                        }
                    }, res);
                }
            });
        });
        serialDialog.reject(function () {
            serialDialog.shutdown()
            $(".serailErrorMessage").text("")
        });
    }
    this.callbackInfo = function () {
        return __infos;
    }

    // Serial Number //
    function serialDialogBox(serialDialog, serials) {
        const serialView = ViewTable({
            keyField: "LineID",
            selector: serialDialog.content.find(".serial-lists"),
            indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: [
                "Direction", "ItemCode", "ItemName", "OpenQty", "Qty", "TotalSelected",
                "WhsCode", "WhsName"
            ],
            columns: [
                {
                    name: "ItemCode",
                    on: {
                        "dblclick": function (e) {
                            __infos.param = {
                                itemId: e.data.ItemID,
                                bpId: e.data.BpId,
                                uomID: e.data.UomID,
                            }
                            if (!e.data.APCSNSelected.APCSNDSelectedDetails) {
                                fetchSerialDetials(e.data, function (res) {
                                    console.log("Data=> ", res)
                                    serialDialog.content.find("#avbQty").val(res.TotalAvailableQty);
                                    updateMaster(__infos.serials, "LineID", e.key, "APCSerialNumberDetial", res);
                                    const data = find("LineID", e.key, __infos.serials);
                                    detialTable(data.APCSerialNumberDetial.APCSNDDetials, serialDialog, serialView, e.data);
                                });
                            } else {
                                const serialDetial = e.data.APCSerialNumberDetial;
                                serialDialog.content.find("#avbQty").val(serialDetial.TotalAvailableQty);
                                updateMaster(__infos.serials, "LineID", e.key, "APCSerialNumberDetial", serialDetial);
                                const data = find("LineID", e.key, __infos.serials);
                                detialTable(data.APCSerialNumberDetial.APCSNDDetials, serialDialog, serialView, e.data);
                            }

                        }
                    }
                }
            ],
        });
        serialView.bindRows(serials);
        __infos.serials = serialView.yield();
    }
    function fetchSerialDetials(data, success) {
        $.get("/PurchaseCreditMemo/GetSerialDetials", { cost: data.Cost, bpId: data.BpId, itemId: data.ItemID, baseOnID: parseInt(data.BaseOnID), copyTuype: parseInt(data.PurCopyType) }, success);
    }

    function detialTable(data, serialDialog, masterTable, masterData) {

        const _data = find("LineID", masterData.LineID, __infos.serials);
        let serialDetialUnselectView = ViewTable({});
        const serialDetialSelectView = ViewTable({
            keyField: "LineID",
            selector: serialDialog.content.find(".serial-select-lists"),
            //indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: [
                "AdmissionDate", "Detials", "ExpirationDate", "Location", "LotNumber", "MfrDate", "ExpireDate",
                "MfrSerialNo", "MfrWarrantyEnd", "MfrWarrantyStart", "SerialNumber", "PlateNumber", "Color", "Brand", "Condition", "Type", "Power", "Year",
            ],
            // visibleFields: ["SerialNumber", "UnitCost"],
            columns: [
                {
                    name: "SerialNumber",
                    on: {
                        "dblclick": function (e) {
                            serialDetialSelectView.removeRow(e.key);
                            let totalSelected = parseFloat(_data.APCSNSelected.TotalSelected)
                            let avqty = parseFloat(_data.APCSerialNumberDetial.TotalAvailableQty)
                            totalSelected -= 1;
                            avqty += 1;
                            const serialObj = {
                                TotalSelected: totalSelected,
                                APCSNDSelectedDetails: serialDetialSelectView.yield(),
                            }
                            const serialObjUn = {
                                TotalAvailableQty: avqty,
                                APCSNDDetials: serialDetialUnselectView.yield(),
                            }
                            updateMaster(__infos.serials, "LineID", masterData.LineID, "APCSerialNumberDetial", serialObjUn);
                            updateMaster(__infos.serials, "LineID", masterData.LineID, "APCSNSelected", serialObj);
                            //Update Master Serial
                            const openQty = masterData.OpenQty + 1;
                            const selectedQty = masterData.TotalSelected - 1;
                            updateMaster(__infos.serials, "LineID", masterData.LineID, "TotalSelected", selectedQty);
                            updateMaster(__infos.serials, "LineID", masterData.LineID, "OpenQty", openQty);
                            masterTable.updateColumn(masterData.LineID, "OpenQty", openQty);
                            masterTable.updateColumn(masterData.LineID, "TotalSelected", selectedQty);

                            data.push(e.data)
                            data.sort((a, b) => a.SerialNumber < b.SerialNumber && 1 || -1);
                            console.log("sdfgdsfg=> E=", data, e, data)
                            serialDetialUnselectView.bindRows(data);
                            serialDialog.content.find("#avbQty").val(avqty);
                            serialDialog.content.find("#ttSlt").val(totalSelected);
                        }
                    }
                }
            ],
        });
        serialDetialSelectView.clearRows();
        serialDetialSelectView.bindRows(_data.APCSNSelected.APCSNDSelectedDetails);
        serialDetialUnselectView = ViewTable({
            keyField: "LineID",
            selector: serialDialog.content.find(".serial-unselect-lists"),
            //indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: [
                "AdmissionDate", "Detials", "ExpirationDate", "Location", "LotNumber", "MfrDate", "ExpireDate",
                "MfrSerialNo", "MfrWarrantyEnd", "MfrWarrantyStart", "SerialNumber", "PlateNumber", "Color", "Brand", "Condition", "Type", "Power", "Year",
            ],
            // visibleFields: [
            //     "LotNumber", "MfrSerialNo", "SerialNumber", "UnitCost"
            // ],
            columns: [
                {
                    name: "SerialNumber",
                    on: {
                        "dblclick": function (e) {
                            if (masterData.OpenQty > 0) {
                                serialDetialUnselectView.removeRow(e.key);
                                data = serialDetialUnselectView.yield();
                                let totalSelected = parseFloat(_data.APCSNSelected.TotalSelected)
                                let avqty = parseFloat(_data.APCSerialNumberDetial.TotalAvailableQty)
                                totalSelected += 1;
                                avqty -= 1;
                                //Update Master Serial
                                const openQty = masterData.OpenQty - 1;
                                const selectedQty = masterData.TotalSelected + 1;
                                updateMaster(__infos.serials, "LineID", masterData.LineID, "TotalSelected", selectedQty);
                                updateMaster(__infos.serials, "LineID", masterData.LineID, "OpenQty", openQty);
                                masterTable.updateColumn(masterData.LineID, "OpenQty", openQty);
                                masterTable.updateColumn(masterData.LineID, "TotalSelected", selectedQty);

                                //__infos.selectedDetial.APCSNDSelectedDetails.push(e.data);
                                serialDetialSelectView.addRow(e.data);
                                const serialObj = {
                                    TotalSelected: totalSelected,
                                    APCSNDSelectedDetails: serialDetialSelectView.yield(),
                                }
                                const serialObjUn = {
                                    TotalAvailableQty: avqty,
                                    APCSNDDetials: serialDetialUnselectView.yield(),
                                }
                                updateMaster(__infos.serials, "LineID", masterData.LineID, "APCSNSelected", serialObj);
                                updateMaster(__infos.serials, "LineID", masterData.LineID, "APCSerialNumberDetial", serialObjUn);
                                serialDialog.content.find("#avbQty").val(avqty);
                                serialDialog.content.find("#ttSlt").val(totalSelected);
                            } else if (masterData.OpenQty === 0) {
                                new DialogBox({
                                    content: "Qty is equal to Total Selected Qty",
                                    type: "ok",
                                    icon: "warning"
                                });
                                //dialogSubmit.confirm(function () { dialogSubmit.shutdown() });
                            }
                        }
                    }
                }
            ],
        });
        serialDetialUnselectView.clearRows();
        serialDetialUnselectView.bindRows(data);
        $("#searchSNs").keyup(function () {
            let __value = this.value.toLowerCase().replace(/\s+/, "");
            let rex = new RegExp(__value, "gi");
            let items = $.grep(data, function (item) {
                const mfrSerialNo = item.MfrSerialNo ?? "";
                const lotNumber = item.LotNumber ?? "";
                return item.UnitCost === __value || item.SerialNumber.toLowerCase().replace(/\s+/, "").match(rex) ||
                    lotNumber.toLowerCase().replace(/\s+/, "").match(rex) ||
                    mfrSerialNo.toLowerCase().replace(/\s+/, "").match(rex) ||
                    item.Qty === __value
            });
            serialDetialUnselectView.bindRows(items);
        });
        $("#filterBP").change(function () {
            const { bpId, itemId } = __infos.param;
            let { APCSNDSelectedDetails: apsds } = __infos.selectedDetial;
            if (this.value == 1) {
                $.get("/PurchaseCreditMemo/GetSerialDetials", { bpId, itemId, apsds: JSON.stringify(apsds), isAll: true }, function (res) {
                    serialDetialUnselectView.bindRows(res.APCSNDDetials);
                    __infos.serialDetial = res;
                    serialDialog.content.find("#avbQty").val(__infos.serialDetial.TotalAvailableQty);
                });
            } else if (this.value == 2) {
                $.get("/PurchaseCreditMemo/GetSerialDetials", { bpId, itemId, apsds: JSON.stringify(apsds), isAll: false }, function (res) {
                    serialDetialUnselectView.bindRows(res.APCSNDDetials);
                    __infos.serialDetial = res;
                    serialDialog.content.find("#avbQty").val(__infos.serialDetial.TotalAvailableQty);
                });
            }
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
