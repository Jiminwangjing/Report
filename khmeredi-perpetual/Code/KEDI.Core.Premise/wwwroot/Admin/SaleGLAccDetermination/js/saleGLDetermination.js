$(document).ready(function () {
    let __data = [],
        __dataRS = [],
        __dataCA = [],
        memberCard = {
            ID: 0,
            CashAccID: 0,
            UnearnedRevenueID: 0
        };
    let __dataMaster = {};
    const CASH = "cash";
    const UNEARNED_REVENUE = "unearn-revenue";
    ///Resourcse
    let $glAccRS = ViewTable({
        keyField: "ID",
        selector: ".allw-gl-r",
        indexed: true,
        paging: {
            enabled: true,
            pageSize: 20
        },
        visibleFields: [
            "TypeOfAccount",
            "GLACode",
            "GLAName",
        ],
        columns: [
            {
                name: "GLACode",
                template: "<input readonly>",
                on: {
                    "click": function (e) {
                        ChooseRscource(e.data, $glAccRS);
                    }
                }
            }
        ],
    });
    onGetSaleGLAccDeterminationResourcesTemplates(function (glA) {
        $glAccRS.bindRows(glA);
        glA.forEach(i => {
            __dataRS.push(i);
        });
    });
    function onGetSaleGLAccDeterminationResourcesTemplates(succuss) {
        $.get("/GLAccountDetermination/GetResources", succuss);
    }

    //dialog Resource
    function ChooseRscource(data, table) {
        let dialog = new DialogBox({
            content: {
                selector: "#Account-Resources"
            },
        });
        dialog.invoke(function () {
            let $glARS = ViewTable({
                keyField: "ID",
                selector: ".input-Resources",
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
                                table.updateColumn(data.ID, "GLACode", e.data.Code);
                                table.updateColumn(data.ID, "GLAName", e.data.Name);
                                updateDetails(__dataRS, "ID", data.ID, "GLAID", e.data.ID);
                                dialog.shutdown();
                            }

                        }
                    }
                ],
            });
            onGetGLARS(function (glAccRS) {
                $glARS.bindRows(glAccRS);
            });
        });
        function onGetGLARS(succuss) {
            $.get("/GLAccountDetermination/GetGLARS", succuss);
        }
        dialog.confirm(function () {
            dialog.shutdown();
        });
    }


    ///Sale Determination
    let $glAccSaleD = ViewTable({
        keyField: "ID",
        selector: ".allw-gl-d",
        indexed: true,
        paging: {
            enabled: true,  //show row GLAccount
            pageSize: 20
        },
        visibleFields: [
            "TypeOfAccount",
            "GlAccCode",
            "GlAccName",
        ],
        columns: [
            {
                name: "GlAccCode",
                template: "<input readonly>",
                on: {
                    "click": function (e) {
                        chooseGlAcc(e.data, $glAccSaleD);
                    }
                }
            }
        ]
    });
    onGetSaleGLAccDeterminationTemplates(function (glAcc) {
        $glAccSaleD.bindRows(glAcc.gld);
        glAcc.gld.forEach(i => {
            __data.push(i);
        });
        __dataMaster = glAcc.GLdm;
        $("#codeCus").val(glAcc.BP.Code);
        $("#name").text(glAcc.BP.Name);
        if (glAcc.MemberCard) {
            memberCard = glAcc.MemberCard;
            $("#cash-account").val(glAcc.MemberCard.CashAccCode);
            $("#cash-account-name").text(glAcc.MemberCard.CashAccName);
            $("#unearned-revenue").val(glAcc.MemberCard.UnearnedRevenueCode);
            $("#unearned-revenue-name").text(glAcc.MemberCard.UnearnedRevenueName);
        }
    });
    function onGetSaleGLAccDeterminationTemplates(succuss) {
        $.get("/GLAccountDetermination/GetSaleGLDetermination", succuss);
    }

    function onGetGlAcc(isAllAcc, success) {
        $.get("/GLAccountDetermination/GetGlAcc", { isAllAcc }, success);
    }
    ///dailog Sale Determination
    function chooseGlAcc(data, table, type) {
        let dialog = new DialogBox({
            content: {
                selector: "#active-gl-content"
            },
            caption: "List Of G/L Account"
        });
        dialog.invoke(function () {
            let $glAcc = ViewTable({
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
                                if (type === "CA") {
                                    table.updateColumn(data.ID, "GLCode", e.data.Code);
                                    table.updateColumn(data.ID, "GLName", e.data.Name);
                                    updateDetails(__dataCA, "ID", data.ID, "GLAID", e.data.ID);
                                } else if (type === CASH) {
                                    $("#cash-account").val(e.data.Code);
                                    $("#cash-account-name").text(e.data.Name);
                                    memberCard.CashAccID = e.data.ID;
                                } else if (type === UNEARNED_REVENUE) {
                                    $("#unearned-revenue").val(e.data.Code);
                                    $("#unearned-revenue-name").text(e.data.Name);
                                    memberCard.UnearnedRevenueID = e.data.ID;
                                } else {
                                    table.updateColumn(data.ID, "GlAccCode", e.data.Code);
                                    table.updateColumn(data.ID, "GLAccName", e.data.Name);
                                    updateDetails(__data, "ID", data.ID, "GLID", e.data.ID);
                                }
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            if (type === CASH || type === UNEARNED_REVENUE) {
                onGetGlAcc(true, function (glAcc) {
                    $glAcc.bindRows(glAcc);
                    searchGlAccount(glAcc, $glAcc, "#txtSearchLA")
                });
            } else {
                onGetGlAcc(false, function (glAcc) {
                    $glAcc.bindRows(glAcc);
                    searchGlAccount(glAcc, $glAcc, "#txtSearchLA")
                });
            }

        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
    }

    // search gl account
    function searchGlAccount(data, table, searchContainer) {
        $(searchContainer).on("keyup", function (e) {
            let __value = this.value.toLowerCase().replace(/\s+/, "");
            let rex = new RegExp(__value, "gi");
            let items = $.grep(data, function (item) {
                return item.Code.toLowerCase().replace(/\s+/, "").match(rex) || item.Name.toLowerCase().replace(/\s+/, "").match(rex)
            });
            table.bindRows(items);
        });
    }
    ///dailog  Control Accounts Receivable
    function onGetControlAcc(success) {
        $.get("/GLAccountDetermination/GetControlAccount", success);
    }
    $("#Control-Account-click").click(function () {
        chooseControl();
    })
    function chooseControl(table, data) {
        let dialog = new DialogBox({
            content: {
                selector: "#Control-Account-Receivable"
            },
            button: {
                yes: {
                    text: "Save"
                },
                no: {
                    text: "Cancel"
                }
            },
            type: "yes-no"
        });
        dialog.confirm(function (e) {
            //convert string to array (JSON.stringify(_data))
            $.post("/GLAccountDetermination/CASave", { controlAccountsReceivable: JSON.stringify(__dataCA) }, function () {
                location.reload();
            })
            dialog.shutdown();
        });
        dialog.reject(function () {
            dialog.shutdown();
        })
        dialog.invoke(function () {
            let $chooseControl = ViewTable({
                keyField: "ID",
                selector: ".input-Control",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 20
                },
                visibleFields: [
                    "TypeOfAccount",
                    "GLCode",
                    "GLName",
                ],
                columns: [
                    {
                        name: "GLCode",
                        template: "<input readonly>",
                        on: {
                            "click": function (e) {
                                chooseGlAcc(e.data, $chooseControl, "CA");
                            }
                        }
                    }
                ]
            });
            onGetControlAcc(function (chooseControl) {
                $chooseControl.bindRows(chooseControl);
                chooseControl.forEach(i => {
                    __dataCA.push(i);
                });

            });
        });
    }


    /// dailog Choose Customers
    $("#accounts-receivable").click(function () {
        chooseCustomer();
    })
    function chooseCustomer() {
        let dialog = new DialogBox({
            content: {
                selector: "#Cus-accounts-receivable"
            },
        });
        dialog.invoke(function () {
            let $chooseCutomer = ViewTable({
                keyField: "ID",
                selector: ".input-box",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 20
                },
                visibleFields: [
                    "Code",
                    "Name",
                    "Phone",
                ],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down"></i>`,
                        on: {
                            "click": function (e) {
                                $("#codeCus").val(e.data.Code);
                                $("#name").text(e.data.Name);
                                __dataMaster.CusID = e.data.ID;
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            onGetChooseCustomer(function (chooseCustomer) {
                $chooseCutomer.bindRows(chooseCustomer);
            });
            dialog.confirm(function () {
                dialog.shutdown();
            });


        });
    }
    function onGetChooseCustomer(success) {
        $.get("/GLAccountDetermination/GetChooseCustomer", success);
    }


    //// button update and Cancel Account Determination
    $("#Update").on("click", function (e) {
        __dataMaster.SaleGLAccountDeterminations = __data;
        __dataMaster.SaleGLAccountDeterminationResources = __dataRS;
        __dataMaster.AccountMemberCard = memberCard;
        $.post("/GLAccountDetermination/Update", { saleGLDetermination: __dataMaster }, function (res) {
            const error = new ViewMessage({
                summary: {
                    selector: ".error"
                }
            }, res);
            if (res.IsApproved) {
                error.refresh();
            }
        })
    });
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

    $("#txtSearchBP").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $(".input-box tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#txtSearchCR").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $(".input-Control tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#txtSearchRS").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $(".input-Resources tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });


    // member card

    $("#cash-account").click(function () {
        chooseGlAcc(undefined, undefined, CASH)
    })
    $("#unearned-revenue").click(function () {
        chooseGlAcc(undefined, undefined, UNEARNED_REVENUE)
    })
    // end member card
})

