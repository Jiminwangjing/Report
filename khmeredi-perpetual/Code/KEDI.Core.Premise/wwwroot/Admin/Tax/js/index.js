$(document).ready(function () {
    let __addObject = [];
    let __updateObjects = [];
    /// Tax group list table ///
    let $listTaxGroup = ViewTable({
        keyField: "LineID",
        selector: $("#list-taxgroups"),
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: false
        },
        visibleFields: [
            "Code", "Active", "Name", "Types", "Effectivefrom", "Rate", "GlAcc"
        ],
        columns: [
            {
                name: "Code",
                template: "<input readonly class='_code' class='input-box-kernel'> ",
                on: {
                    "keyup": function (e) {
                        if (isValidArray(__addObject)) updatedata(e.key, "Code", this.value);
                        if (isValidArray(__updateObjects)) updatedatas(__updateObjects, e.key, "Code", this.value);
                    },
                }
            },
            {
                name: "Effectivefrom",
                on: {
                    "dblclick": function (e) {
                        taxDefinition(e);
                    }
                }
            },
            {
                name: "Active",
                template: "<input type='checkbox' class='input-box-kernel'>",
                on: {
                    "click": function (e) {
                        if (isValidArray(__addObject)) {
                            const active = $(this).prop("checked") ? true : false;
                            updatedata(e.key, "Active", active);
                        }
                        if (isValidArray(__updateObjects)) {
                            const active = $(this).prop("checked") ? true : false;
                            updatedatas(__updateObjects[0], e.key, "Active", active);
                        }
                    }
                }
            },
            {
                name: "Name",
                template: "<input class='input-box-kernel'>",
                on: {
                    "keyup": function (e) {
                        if (isValidArray(__addObject)) updatedata(e.key, "Name", this.value);
                        if (isValidArray(__updateObjects)) updatedatas(__updateObjects[0], e.key, "Name", this.value);
                    }
                }
            },
            {
                name: "Types",
                template: "<select disabled class='input-box-kernel _selectType'>",
                on: {
                    "change": function (e) {
                        if (isValidArray(__addObject)) updatedata(e.key, "Type", this.value);
                        if (isValidArray(__updateObjects)) updatedatas(__updateObjects[0], e.key, "Type", this.value);
                    }
                }
            },
            {
                name: "GlAcc",
                template: "<input readonly class='input-box-kernel'>",
                on: {
                    "dblclick": function (e) {
                        if (isValidArray(__addObject)) chooseGlaccCode(e);
                        if (isValidArray(__updateObjects)) chooseGlaccCode(e);
                    }
                }
            }
        ]
    });
    getTaxGraoups(function (res) {
        res.forEach(i => {
            i.Effectivefrom = formatDate(i.Effectivefrom, "YYYY-MM-DD");
        })
        $listTaxGroup.bindRows(res);
        if (isValidArray(res))
            __updateObjects.push(res);
    });

    $("#add").click(function () {
        __addObject[0].TaxGroupDefinitions = __addObject.TaxGroupDefinitions;
        $.post("/TaxGroup/CreateTaxGroup", { tax: __addObject[0] }, function (res) {
            if (res.IsApproved) {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, res).refresh(1500);
            } else {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, res);
            }
        })
    })

    $("#add-new-taxgroup").click(function () {
        $(this).addClass("no-event");
        $.get("/TaxGroup/GetNewOneToCreate", function (res) {
            $("#save").prop("hidden", true);
            $("#add").prop("hidden", false);
            res.Effectivefrom = res.Effectivefrom.split("T")[0];
            $listTaxGroup.addRow(res);
            __addObject.push(res);
            const _code = $("._code").length;
            const _selectType = $("._selectType").length;
            $(`._code:nth(${_code - 1})`).prop("readonly", false);
            $(`._selectType:nth(${_selectType - 1})`).prop("disabled", false);
        });
    });
    /// Save Tax Group ///
    $("#save").click(function () {
        $.post("/TaxGroup/UpdateTaxgroups", { taxes: JSON.stringify(__updateObjects[0]) }, function (res) {
            if (res.IsApproved) {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, res).refresh(1500);
            } else {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, res);
            }
        })
    });
    /// Cancel ///
    $("#cancel").click(function () {
        location.reload();
    });
    /// Tax Definition List Table ///
    function taxDefinition(e) {
        let dialog = new DialogBox({
            content: {
                selector: "#list-td-content",
            },
            type: "ok/cancel"
        });
        const datatd = findArray("ID", e.data.ID, __updateObjects[0]);
        dialog.invoke(function () {
            let $listTaxD = ViewTable({
                keyField: "ID",
                selector: $("#list-tds"),
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: false
                },
                visibleFields: [
                    "EffectiveFrom", "Rate"
                ],
                columns: [
                    {
                        name: "EffectiveFrom",
                        template: "<input class='input-box-kernel' type='date'> ",
                        on: {
                            "change": function (e) {
                                if (datatd)
                                    if (isValidArray(datatd.TaxGroupDefinitions)) updatedataTD(datatd.TaxGroupDefinitions, "ID", e.key, "EffectiveFrom", this.value);
                                if (isValidArray(__addObject.TaxGroupDefinitions)) updatedataTD(__addObject.TaxGroupDefinitions, "LineID", e.data.LineID, "EffectiveFrom", this.value);

                            },
                        }
                    },
                    {
                        name: "Rate",
                        template: "<input class='input-box-kernel'>",
                        on: {
                            "keyup": function (e) {
                                $(this).asNumber();
                                if (isValidArray(__addObject.TaxGroupDefinitions)) updatedataTD(__addObject.TaxGroupDefinitions, "LineID", e.data.LineID, "Rate", this.value);
                                if (datatd)
                                    if (isValidArray(datatd.TaxGroupDefinitions)) updatedataTD(datatd.TaxGroupDefinitions, "ID", e.key, "Rate", this.value);
                            }
                        }
                    },
                ]
            });
            if (datatd) {
                if (isValidArray(datatd.TaxGroupDefinitions)) {
                    datatd.TaxGroupDefinitions.forEach(i => {
                        i.EffectiveFrom = i.EffectiveFrom.split("T")[0];
                    })
                    $listTaxD.bindRows(datatd.TaxGroupDefinitions);
                }
                else
                    $listTaxD.bindRows(__addObject.TaxGroupDefinitions);
            }
            else
                $listTaxD.bindRows(__addObject.TaxGroupDefinitions);

            $("#add-new-td").click(function () {
                $.get("/TaxGroup/GetNewOneToCreateTd", function (res) {
                    res.EffectiveFrom = res.EffectiveFrom.split("T")[0];
                    res.TaxGroupID = e.data.ID;
                    $listTaxD.addRow(res);
                    __addObject.TaxGroupDefinitions = $listTaxD.yield();
                });
            });
        });

        dialog.confirm(function () {
            if (datatd) {
                if (isValidArray(datatd.TaxGroupDefinitions)) {
                    const tdslatest = datatd.TaxGroupDefinitions.
                        sort((a, b) => new Date(a.EffectiveFrom).getTime() - new Date(b.EffectiveFrom).
                            getTime())[datatd.TaxGroupDefinitions.length - 1];
                    $listTaxGroup.updateColumn(e.key, "Effectivefrom", tdslatest.EffectiveFrom);
                    $listTaxGroup.updateColumn(e.key, "Rate", tdslatest.Rate);
                }
                else {
                    const tdslatest = __addObject.TaxGroupDefinitions.
                        sort((a, b) => new Date(a.EffectiveFrom).getTime() - new Date(b.EffectiveFrom).
                            getTime())[__addObject.TaxGroupDefinitions.length - 1];

                    $listTaxGroup.updateColumn(e.key, "Effectivefrom", tdslatest.EffectiveFrom);
                    $listTaxGroup.updateColumn(e.key, "Rate", tdslatest.Rate);
                }
            }
            else {
                const tdslatest = __addObject.TaxGroupDefinitions.
                    sort((a, b) => new Date(a.EffectiveFrom).getTime() - new Date(b.EffectiveFrom).
                        getTime())[__addObject.TaxGroupDefinitions.length - 1];

                $listTaxGroup.updateColumn(e.key, "Effectivefrom", tdslatest.EffectiveFrom);
                $listTaxGroup.updateColumn(e.key, "Rate", tdslatest.Rate);
            }

            dialog.shutdown();
        })
        dialog.reject(function () {
            dialog.shutdown();
        })

    }

    function chooseGlaccCode(e) {
        let dialog = new DialogBox({
            content: {
                selector: "#list-glacc-content",
            }
        });
        dialog.invoke(function () {
            let $listActiveGL = ViewTable({
                keyField: "ID",
                selector: dialog.content.find("#list-glaccs"),
                indexed: true,
                paging: {
                    pageSize: 12,
                    enabled: true
                },
                visibleFields: [
                    "Name",
                    "Code"
                ],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-alt-circle-down hover"></i>`,
                        on: {
                            "click": function (data) {
                                $(e.column).children("input").val(data.data.Code);
                                if (isValidArray(__addObject)) updatedata(e.key, "GLID", data.data.ID);
                                if (isValidArray(__updateObjects)) updatedatas(__updateObjects[0], e.key, "GLID", data.data.ID);
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            getActiveGlacc(function (resp) {
                if (resp.length > 0) {
                    $listActiveGL.bindRows(resp);
                    $("#txtSearglacc").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let rex = new RegExp(__value, "gi");
                        let items = $.grep(resp, function (item) {
                            return item.Code.toLowerCase().replace(/\s+/, "").match(rex) || item.Name.toLowerCase().replace(/\s+/, "").match(rex);
                        });
                        $listActiveGL.bindRows(items);
                    });
                }
            })

        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    function getActiveGlacc(success) {
        $.get("/TaxGroup/GetActiveGlacc", success);
    }
    function getTaxGraoups(success) {
        $.get("/TaxGroup/GetTaxGraoups", success);
    }
    function updatedata(key, prop, value) {
        if (key === __addObject[0]["LineID"]) {
            __addObject[0][prop] = value;
        }
    }
    function updatedatas(data, key, prop, value) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (key === i.LineID) {
                    i[prop] = value;
                }
            });
        }
    }
    function updatedataTD(data, keyCheck, key, prop, value) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (key === i[keyCheck]) {
                    i[prop] = value;
                }
            });
        }

    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function findArray(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
    function formatDate(value, format) {
        let dt = new Date(value),
            objFormat = {
                MM: ("0" + (dt.getMonth() + 1)).slice(-2),
                DD: ("0" + dt.getDate()).slice(-2),
                YYYY: dt.getFullYear()
            },
            dateString = "";

        let dateFormats = format.split("-");
        for (let i = 0; i < dateFormats.length; i++) {
            dateString += objFormat[dateFormats[i]];
            if (i < dateFormats.length - 1) {
                dateString += "-";
            }
        }

        return dateString;
    }
});