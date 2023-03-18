function SerialTemplate(config) {
    if (!(this instanceof SerialTemplate)) {
        return new SerialTemplate(config);
    }

    var __config = {
        data: {
            isGoodsIssue: false,
            isReturnDelivery: false,
            isCreditMemo: false,
            isCopy: false,
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
            if (__config.data.serials.length > 0) {
                for (let i = 0; i < __config.data.serials.length; i++) {
                    __config.data.serials[i].LineID = __config.data.serials[i].LineID + i;
                }


                serialDialogBox(serialDialog, __config.data.serials);
            }
            // serialDialogBox(serialDialog, __config.data.serials);
        });
        serialDialog.confirm(function () {
            $.post("/Sale/CheckSerailNumber", { serails: JSON.stringify(__infos.serials) }, function (res) {
                if (res.IsApproved) {
                    $(".serailErrorMessage").text("");
                    $("#checkingSerialString").val("checked");
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
            serialDialog.shutdown();
            $(".serailErrorMessage").text("");
            $("#checkingSerialString").val("unckeched");
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
                                uomID: e.data.UomID,
                            }
                            if (__config.data.isCopy) {
                                if (__config.data.isCreditMemo) {
                                    fetchGetSerialDetialsCopy(e.data, true, function (res) {
                                        serialDialog.content.find("#ttSlt").val(res.TotalSelected);
                                        updateMaster(__infos.serials, "LineID", e.key, "SerialNumberSelected", res);
                                        bindSelectedTableCopy(serialDialog, e.data);
                                    });
                                } else if (__config.data.isReturnDelivery) {
                                    fetchGetSerialDetialsCopy(e.data, false, function (res) {
                                        bindSelectedTableCopy(serialDialog, e.data);
                                        serialDialog.content.find("#ttSlt").val(res.TotalSelected);
                                        updateMaster(__infos.serials, "LineID", e.key, "SerialNumberSelected", res);
                                    });
                                }
                            } else {
                                if (!e.data.SerialNumberSelected.SerialNumberSelectedDetails) {
                                    fetchSerialDetials(e.data, function (res) {
                                        console.log('Data', res)

                                        serialDialog.content.find("#avbQty").val(res.TotalAvailableQty);
                                        updateMaster(__infos.serials, "LineID", e.key, "SerialNumberUnselected", res);
                                        const data = find("LineID", e.key, __infos.serials);
                                        detialTable(data.SerialNumberUnselected, serialDialog, serialView, e.data);
                                    });
                                } else {
                                    const serialDetial = e.data.SerialNumberUnselected;
                                    serialDialog.content.find("#avbQty").val(serialDetial.TotalAvailableQty);
                                    updateMaster(__infos.serials, "LineID", e.key, "SerialNumberUnselected", serialDetial);
                                    const data = find("LineID", e.key, __infos.serials);
                                    detialTable(data.SerialNumberUnselected, serialDialog, serialView, e.data);
                                }
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
        $.get(
            "/Sale/GetSerialDetials",
            {
                itemId: data.ItemID,
                wareId: data.WareId,
                isReturnDelivery: __config.data.isReturnDelivery,
                isCreditMemo: __config.data.isCreditMemo,
                isGoodsIssue: __config.data.isGoodsIssue,
                cost: data.Cost,
            },
            success
        );
    }
    function fetchGetSerialDetialsCopy(data, isCreditMemo = false, success) {
        $.get(
            "/Sale/GetSerialDetialsDeliveryCopy",
            {
                itemId: data.ItemID,
                saleId: data.SaleID,
                wareId: data.WareId,
                isCreditMemo
            },
            success
        );
    }
    function detialTable(data, serialDialog, masterTable, masterData) {
        const _data = find("LineID", masterData.LineID, __infos.serials);
        let serialDetialUnselectView = ViewTable({});
        const serialDetialSelectView = ViewTable({
            keyField: "LineID",
            selector: serialDialog.content.find(".serial-select-lists"),
            indexed: false,
            paging: {
                pageSize: 20,
                enabled: false
            },
            // visibleFields: ["SerialNumber", "UnitCost"],
            visibleFields: [
                "AdmissionDate", "Details", "ExpirationDate", "Location", "LotNumber", "MfrDate", "ExpireDate",
                "MfrSerialNo", "MfrWarDateEnd", "MfrWarDateStart", "SerialNumber", "PlateNumber", "Color", "Brand", "Condition", "Type", "Power", "Year",
            ],
            columns: [
                {
                    name: "SerialNumber",
                    on: {
                        "dblclick": function (e) {
                            serialDetialSelectView.removeRow(e.key);
                            let totalSelected = parseFloat(_data.SerialNumberSelected.TotalSelected)
                            let avqty = parseFloat(_data.SerialNumberUnselected.TotalAvailableQty)
                            totalSelected -= 1;
                            avqty += 1;
                            const serialObj = {
                                TotalSelected: totalSelected,
                                SerialNumberSelectedDetails: serialDetialSelectView.yield(),
                            }
                            const serialObjUn = {
                                TotalAvailableQty: avqty,
                                SerialNumberUnselectedDetials: serialDetialUnselectView.yield(),
                            }
                            updateMaster(__infos.serials, "LineID", masterData.LineID, "SerialNumberUnselected", serialObjUn);
                            updateMaster(__infos.serials, "LineID", masterData.LineID, "SerialNumberSelected", serialObj);
                            //Update Master Serial
                            const openQty = masterData.OpenQty + 1;
                            const selectedQty = masterData.TotalSelected - 1;
                            updateMaster(__infos.serials, "LineID", masterData.LineID, "TotalSelected", selectedQty);
                            updateMaster(__infos.serials, "LineID", masterData.LineID, "OpenQty", openQty);
                            masterTable.updateColumn(masterData.LineID, "OpenQty", openQty);
                            masterTable.updateColumn(masterData.LineID, "TotalSelected", selectedQty);

                            data.push(e.data)
                            data.sort((a, b) => a.SerialNumber < b.SerialNumber && 1 || -1);
                            serialDetialUnselectView.bindRows(data);
                            serialDialog.content.find("#avbQty").val(avqty);
                            serialDialog.content.find("#ttSlt").val(totalSelected);
                        }
                    }
                }
            ],
        });
        serialDetialSelectView.clearRows()
        serialDetialSelectView.bindRows(_data.SerialNumberSelected.SerialNumberSelectedDetails);
        serialDetialUnselectView = ViewTable({
            keyField: "LineID",
            selector: serialDialog.content.find(".serial-unselect-lists"),
            indexed: false,
            paging: {
                pageSize: 15,
                enabled: false
            },
            // visibleFields: [
            //     "LotNumber", "MfrSerialNo", "SerialNumber","PlateNumber", "UnitCost","Color","Brand","Condition","Type","Power","Year"
            // ],
            visibleFields: [
                "AdmissionDate", "Details", "ExpirationDate", "Location", "LotNumber", "MfrDate", "ExpireDate",
                "MfrSerialNo", "MfrWarDateEnd", "MfrWarDateStart", "SerialNumber", "PlateNumber", "Color", "Brand", "Condition", "Type", "Power", "Year",
            ],
            columns: [
                {
                    name: "SerialNumber",
                    on: {
                        "dblclick": function (e) {
                            if (masterData.OpenQty > 0) {
                                serialDetialUnselectView.removeRow(e.key);
                                data = serialDetialUnselectView.yield();
                                let totalSelected = parseFloat(_data.SerialNumberSelected.TotalSelected)
                                let avqty = parseFloat(_data.SerialNumberUnselected.TotalAvailableQty)
                                totalSelected += 1;
                                avqty -= 1;
                                //Update Master Serial
                                const openQty = masterData.OpenQty - 1;
                                const selectedQty = masterData.TotalSelected + 1;
                                updateMaster(__infos.serials, "LineID", masterData.LineID, "TotalSelected", selectedQty);
                                updateMaster(__infos.serials, "LineID", masterData.LineID, "OpenQty", openQty);
                                masterTable.updateColumn(masterData.LineID, "OpenQty", openQty);
                                masterTable.updateColumn(masterData.LineID, "TotalSelected", selectedQty);

                                serialDetialSelectView.addRow(e.data);
                                const serialObj = {
                                    TotalSelected: totalSelected,
                                    SerialNumberSelectedDetails: serialDetialSelectView.yield(),
                                }
                                const serialObjUn = {
                                    TotalAvailableQty: avqty,
                                    SerialNumberUnselectedDetials: serialDetialUnselectView.yield(),
                                }
                                updateMaster(__infos.serials, "LineID", masterData.LineID, "SerialNumberSelected", serialObj);
                                updateMaster(__infos.serials, "LineID", masterData.LineID, "SerialNumberUnselected", serialObjUn);
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
        serialDetialUnselectView.bindRows(data.SerialNumberUnselectedDetials);
        $("#searchSNs").keyup(function () {
            if (this.value != "") {

                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let rex = new RegExp(__value, "gi");
                let items = $.grep(data.SerialNumberUnselectedDetials, function (item) {
                    console.log("item =", item)
                    const mfrSerialNo = item.MfrSerialNo ?? "";
                    const lotNumber = item.LotNumber ?? "";
                    const PlateNumber = item.PlateNumber ?? "";
                    return item.UnitCost === __value || item.SerialNumber.toLowerCase().replace(/\s+/, "").match(rex) ||
                        lotNumber.toLowerCase().replace(/\s+/, "").match(rex) ||
                        mfrSerialNo.toLowerCase().replace(/\s+/, "").match(rex) ||
                        PlateNumber.toLowerCase().replace(/\s+/, "").match(rex) ||
                        item.Qty === __value
                });
                serialDetialUnselectView.bindRows(items);
            }
            else {
                serialDetialUnselectView.bindRows(data.SerialNumberUnselectedDetials);
            }
        });
    }
    function bindSelectedTableCopy(serialDialog, data) {
        const _data = find("LineID", data.LineID, __infos.serials);
        const serialDetialSelectView = ViewTable({
            keyField: "LineID",
            selector: serialDialog.content.find(".serial-select-lists"),
            //indexed: true,
            paging: {
                pageSize: 10,
                enabled: false
            },
            visibleFields: ["SerialNumber", "UnitCost"],
        });
        serialDetialSelectView.bindRows(_data.SerialNumberSelected.SerialNumberSelectedDetails);
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
