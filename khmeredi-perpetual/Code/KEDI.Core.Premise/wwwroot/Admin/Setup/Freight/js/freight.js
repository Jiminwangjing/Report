$(document).ready(function () {
    let __data = [];
    let $glRevenCode = ViewTable({
        keyField: "LineID",
        selector: ".allw-gl-frieht",
        indexed: true,
        paging: {
            enabled: false,
            pageSize: 10
        },
        visibleFields: [
            "Name",
            "RevenAcctCode",
            "ExpenAcctCode",
            "OutTaxList",
            "InTaxList",
            "AmountReven",
            "AmountExpen",
            "FreightReceiptTypes",
            "IsActive"
        ],
        // when select update data(update detail)
        columns: [
            {
                name: "RevenAcctCode",
                template: "<input readonly>",
                on: {
                    "click": function (e) {
                        ChooseRevenAcctCode(e.data, $glRevenCode);
                    }
                }
            },
            {
                name: "Name",
                template: "<input class='input-box-kernel'>",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data,"LineID", e.key, "Name", this.value);
                    }
                }
                
            },
            {
                name: "ExpenAcctCode",
                template: "<input readonly>",
                on: {
                    "click": function (e) {
                        chooseExpenAcctCode(e.data, $glRevenCode);
                    }
                }
            },
            {
                name: "OutTaxList",
                template: "<select></select>",
                on: {
                    "change": function (e) {
                        updateDetails(__data, "LineID", e.key, "OutTaxID", this.value);
                    }
                }
            },
            {
                name: "InTaxList",
                template: "<select></select>",
                on: {
                    "change": function (e) {
                        updateDetails(__data, "LineID", e.key, "InTaxID", this.value);
                    }
                }
            },
            {
                name: "AmountReven",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data, "LineID", e.key, "AmountReven", this.value);
                    }
                }
            },
            {
                name: "AmountExpen",
                template: "<input >",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data, "LineID", e.key, "AmountExpen", this.value);
                    }
                }
            },
            {
                name: "FreightReceiptTypes",
                template: "<select></select>",
                on: {
                    "change": function (e) {
                        updateDetails(__data, "LineID", e.key, "FreightReceiptType", this.value);
                    }
                }
            },
            {
                name: "IsActive",
                template: "<input type='checkbox'>",
                on: {
                    "click": function (e) {
                        const isChecked = $(this).prop("checked");
                        updateDetails(__data, "LineID", e.key, "IsActive", isChecked);
                    }
                }
            },
        ]
    });
    onGetFriehtTemplates(function (glex) {
        console.log("glex",glex)
        $glRevenCode.bindRows(glex);
        glex.forEach(i => {
            __data.push(i);
        });
      
    });
    $("#add-new-friegh").click(function () {
        $.get("/Freight/GetEmptyData", function (res) {
            
            $glRevenCode.addRow(res);
            __data.push(res);
        })
    });
    function onGetFriehtTemplates(succuss) {
        $.get("/Freight/Getfrieht", succuss);
    }

    function onGetGlAcc(success) {
        $.get("/Freight/Getfrieht", success);
    }
    function getGlAcc(success) {
        $.get("/Freight/GetGlAcc", success);
    }
    /// dailog RevenAcctCode
    function ChooseRevenAcctCode(data, table) {
        let dialog = new DialogBox({
            content: {
                selector: "#active-gl-content"
            },
            caption: "GL Account"
        });
        dialog.invoke(function () {
            let $glRes = ViewTable({
                keyField: "ID",
                selector: ".allw-gl",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 20
                },
                visibleFields: [
                    "Code",
                    "Name",
                ],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                table.updateColumn(data.LineID, "RevenAcctCode", e.data.Code);
                                updateDetails(__data, "LineID", data.LineID, "RevenAcctID", e.data.ID);
                                dialog.shutdown();
                            }

                        }
                    }
                ],
            });
            getGlAcc(function (res) {
                $glRes.bindRows(res);
            });
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
    }

    // dailog ExpenAcctCode
    function chooseExpenAcctCode(data,table) {
        let dialog = new DialogBox({
            content: {
                selector: "#active-gl-content"
            },
            caption: "GL Account"
        });
        dialog.invoke(function () {
            let $glEx = ViewTable({
                keyField: "ID",
                selector: ".allw-gl",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize:20
                },
                visibleFields: [
                    "Code",
                    "Name",
                ],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                table.updateColumn(data.LineID, "ExpenAcctCode", e.data.Code);
                                updateDetails(__data, "LineID", data.LineID, "ExpenAcctID", e.data.ID);
                                dialog.shutdown();
                            }
                        }
                    }
                ],
            });
            getGlAcc(function (res) {
                $glEx.bindRows(res);
            });
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
    }

    ////update detail
    // when select update data
    function updateDetails(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue)
                    i[prop] = propValue
            })
        }
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }

    /////Save data Frieht
    $("#Update").click(function () {
        $.ajax({
            url: "/Freight/UpdateFrieht",
            type: "POST",
            dataType: "JSON",
            data: { data: JSON.stringify(__data) },
            success: function (respones) {
                if (respones.IsApproved) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        },
                    }, respones).refresh(1000);
                } else {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        }
                    }, respones);
                }
            }
        });
    });
    /// search data in dailog GL Account
    $("#txtSearchLA").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $(".allw-gl tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
})

