$(document).ready(function () {
    let doctypeid = 0;
    $("#startdate").change(function () {
        const s = new Date(this.value);
        $("#startdate").attr(
            "data-date",
            moment(s, "YYYY-MM-DD")
                .format($("#startdate").attr("data-date-format"))
        )
        document.getElementById("startdate").valueAsDate = s;
        let sday = s.getDate();
        $("#sday").val(sday);
        let startingday = num.toNumberSpecial($("#cday").val()) - num.toNumberSpecial(sday);
        $("#predictedclosingnum").val(startingday);

        if ($("#predictedclosingnum").val() != 0) {
            const d = new Date();
            let preday = num.toNumberSpecial($("#predictedclosingnum").val());
            d.setDate(d.getDate() + preday);
            $("#predictedclosingdate").attr(
                "data-date",
                moment(d, "YYYY-MM-DD")
                    .format($("#predictedclosingdate").attr("data-date-format"))
            )
            document.getElementById("predictedclosingdate").valueAsDate = d;
        }
    })


    $("#closingdate").change(function () {

        const s = new Date(this.value);
        console.log("s", s)
        $("#closingdate").attr(
            "data-date",
            moment(s, "YYYY-MM-DD")
                .format($("#closingdate").attr("data-date-format"))
        )
        document.getElementById("closingdate").valueAsDate = s;
        let cday = s.getDate();
        $("#cday").val(cday);
        let closingday = num.toNumberSpecial(cday) - num.toNumberSpecial($("#sday").val());
        $("#predictedclosingnum").val(closingday);
        if ($("#predictedclosingnum").val() != 0) {
            const d = new Date();
            let preday = num.toNumberSpecial($("#predictedclosingnum").val());
            d.setDate(d.getDate() + preday);
            $("#predictedclosingdate").attr(
                "data-date",
                moment(d, "YYYY-MM-DD")
                    .format($("#predictedclosingdate").attr("data-date-format"))
            )
            document.getElementById("predictedclosingdate").valueAsDate = d;
        }

    })

    $.each($(".dd"), function (i, t) {
        setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
    });
    $("#predictedclosingnum").keyup(function () {
        const d = new Date();
        d.setDate(d.getDate() + parseInt(this.value));
        $("#predictedclosingdate").attr(
            "data-date",
            moment(d, "YYYY-MM-DD")
                .format($("#predictedclosingdate").attr("data-date-format"))
        )
        document.getElementById("predictedclosingdate").valueAsDate = d;
    })

    $("#predictedclosingdate").change(function () {
        s = new Date($('.dd').val());
        let month1 = s.getMonth();
        let year1 = s.getFullYear();

        const d = new Date(this.value);
        let month2 = d.getMonth();
        let year2 = d.getFullYear();
        $("#predictedclosingdate").attr(
            "data-date",
            moment(d, "YYYY-MM-DD")
                .format($("#predictedclosingdate").attr("data-date-format"))
        )

        let day = d.getDate();
        const n = new Date();
        let now = n.getDate();
        var dd = day - now;
        document.getElementById("predictedclosingnum").value = dd;
        if (day < now && month2 <= month1 && year2 <= year1) {
            $("#predictedclosingnum").val(0)
            $("#cpredate").show();
        }
        else if (day >= now && month2 >= month1 && year2 < year1) {
            $("#predictedclosingnum").val(0)
            $("#cpredate").show();
        }
        else if (day <= now && month2 >= month1 && year2 < year1) {
            $("#predictedclosingnum").val(0)
            $("#cpredate").show();
        }
        else {
            $("#cpredate").hide();
        }
    })
    $("#startdate").change(function () {
        s = new Date($('#predictedclosingdate').val());
        let day1 = s.getDate();
        let month1 = s.getMonth();
        let year1 = s.getFullYear();

        const d = new Date(this.value);
        let day2 = d.getDate();
        let month2 = d.getMonth();
        let year2 = d.getFullYear();


        if (day2 > day1 && month2 >= month1 && year2 >= year1) {

            $("#predictedclosingnum").val(0)
            $("#cpredate").show();
        }
        else if (day2 >= day1 && month2 >= month1 && year2 > year1) {
            $("#predictedclosingnum").val(0)
            $("#cpredate").show();
        }
        else if (day2 <= day1 && month2 >= month1 && year2 >= year1) {
            $("#predictedclosingnum").val(0)
            $("#cpredate").show();
        }
        else if (day2 == day1 && month2 == month1 && year2 == year1) {
        }
        else {
            $("#cpredate").hide();
        }
    })
    $("#cpredate").hide();
    $("#search-dataemp").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#list-dataemp tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#search-saleorder").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#list-saleorder tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#search-emp").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#list-em tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#search-doctype").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#list-doctypeno tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#search-opp").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#list-opp tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    $("#search-bus").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#list-bp tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $.get('/Opportunity/GetNum', function (num) {

        $("#next_number").val(num);

    })

    $("#empid").change(function () {
        $("#saleempidd").val(this.value);
    })
    $("#doctypeId").change(function () {
        if (this.value == 0) {
            $("#docNoid").val(this.value);
        }
        $("#doctypid").val(this.value)
    })
    $("#potentailamount").keyup(function () {
        $(this).asNumber();
    })
    $("#predictedclosingnum").keyup(function () {
        $(this).asNumber();
    })

    //$("#weightamount").keyup(function () {
    //    $(this).asNumber();
    //})

    $("#profitpercent").keyup(function () {
        $(this).asNumber();
    })
    $("#profittotal").keyup(function () {
        $(this).asNumber();
    })

    function setDate(selector, date_value) {
        var _date = $(selector);
        _date[0].valueAsDate = new Date(date_value);
        _date[0].setAttribute(
            "data-date",
            moment(_date[0].value)
                .format(_date[0].getAttribute("data-date-format"))
        );
    }

    $("#btn-find").on("click", function () {
        $("#btn-createnew").removeAttr("hidden", "hidden");
        $("#next_number").val("").prop("readonly", false).focus();
        $("#submit-data").hide();
        $("#update-data").removeAttr("hidden");
    });

    $("#choose").on("click", function () {
        $("#btn-createnew").removeAttr("hidden", "hidden");

        $("#submit-data").hide();
        $("#update-data").removeAttr("hidden");
    });
    const summaryObj = {
        ID: 0,
        OpportunityMasterDataID: 0,
        DoctypeID: 0,
        SeriesID: 0,
        SeriesDID: 0,
        DescriptionSummaryDetails: [],
        IsOpen: false,
        IsLost: false,
        IsWon: false,
    }

    const potentialObj = {
        ID: 0,
        OpportunityMasterDataID: 0,
        PredictedClosingInTime: 0,
        PredictedClosingInNum: 0,
        PredictedClosingDate: "",
        PotentailAmount: 0,
        WeightAmount: 0,
        GrossProfit: 0,
        GrossProfitTotal: 0,
        Level: 0,
        OpportunityMasterID: 0,
        InterestID: 0,
        DescriptionPotentialDetail: []
    }
    $("#won").click(function () {
        $('#show-list-doctypeNo').prop("disabled", !$("#won").prop("checked"));
        if ($(this).prop("checked")) {
            $("#open").prop("checked", false);
            $("#lost").prop("checked", false);
            $("#doctypeId").attr('enabled', 'enabled');
            $("#buttonlist").show();
            summaryObj.IsLost = false;
            summaryObj.IsWon = true;
            summaryObj.IsOpen = false;
            $("#doctypeId").removeAttr("disabled");
            $("#buttonlist").removeAttr("hidden");
        } else {
            $("#doctypeId").attr("disabled", "disabled");
            $("#buttonlist").attr("hidden", "hidden");
            summaryObj.IsWon = false;
        }
    });

    //master-data
    var opportunity_master = {
        ID: 0,
        SaleEmpID: 0,
        Owner: "",
        OpportunityName: "",
        OpportunityNo: 0,
        BPID: 0,
        Status: "",
        StartDate: "",
        ClosingDate: "",
        CloingPercentage: 0,
        OpportunityTpe: 0,
        PotentialDetails: {},
        SummaryDetails: {},
        StageDetail: {},
        CompetitorDetail: {},
        PartnerDetail: {}
    }
    $("#potentailamount").keyup(function () {
        __datastage.forEach(i => {
            i.PotentailAmount = this.value;
        })
        $("#stagepotentailamount").val(this.value);
        var weightamount = $("#weightamount").val();
        $("#weightamountt").val(weightamount);
        let amount = num.formatSpecial(this.value) * num.formatSpecial($("#closingid").val()) / 100;
        $("#weightamount").val(amount);
        if ($("#closingid").val() == 0) {
            $("#weightamount").val(0);
        }
        if (this.value == 0) {
            $("#weightamount").val(0);
        }
    })

    $("#weightamount").keyup(function () {
        var weightamount = $("#weightamount").val();
        var weightamountt = $("#weightamountt").val(weightamount);
    })
    let __data = [];
    let __datacompetitor = [];
    let __datastage = [];
    let __datapotentaildes = [];
    let __datasmmarydes = [];
    let __datapartner = []
    let __datapotentailselectdes = [];
    let __desselect = [];
    let __setupstages = [];
    let __setuppartners = [];
    let __setupnewcompetitor = [];
    let ____setupsaleemp = [];
    let __updateowner = [];
    let __setuplevel = [];
    let __relatedbp = [];
    let __dataemp = [];
    //model box of bussines partner
    $("#show-list-itemster").click(function () {
        $.ajax({
            url: "/Opportunity/GetBP",
            type: "Get",
            dataType: "Json",
            data: { data: "True" },
            success: function (response) {
                bindBP(response);
            }
        });
    });

    //==================
    $("#choose").click(function () {
        $.ajax({
            url: "/Opportunity/Getopp",
            type: "Get",
            dataType: "Json",
            success: function (response) {
                bindOpp(response);
            }
        });
    });

    function bindOpp(res) {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Close"
                }
            },
            content: {
                selector: ".opporutnity"
            },
            caption: "List Opporunity"
        });
        dialog.invoke(function () {
            const __listBussinesPartner = ViewTable({
                keyField: "ID",
                selector: "#list-opp",
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["OpportunityNo", "BPCode", "BpName", "Choose"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down" id="close"  ></i>`,
                        on: {
                            "click": function (e) {
                                var data = e.data.OpportunityNo
                                $.ajax({
                                    url: "/Opportunity/FindOpportunity",
                                    dataType: "Json",
                                    data: { number: data },
                                    success: function (result) {
                                        search(result);
                                    }
                                });
                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listBussinesPartner.bindRows(res)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    function search(result) {
        if (!result) {

            let dialog = new DialogBox({
                caption: "Searching",
                icon: "danger",
                content: "Opporturnity Not found!",
                close_button: "none"
            });

            dialog.confirm(function () {
                $("#next_number").prop("readonly", false).focus();
                dialog.shutdown();
            });

            $("#next_number").val(" ")
            $("#next_number").blur();

        }
        else {
            opportunity_master = result;
            $("#contact").find('option').remove();
            $("#tel").find('option').remove();
            result.Contact.forEach(i => {
                if (i.SetAsDefualt) {
                    $("#contact").append(`<option value='${i.ContactID}'  selected = "selected">${i.ContactID}</option>`)
                    $("#tel").append(`<option  value='${i.Tel1}>' selected = "selected">${i.Tel1}</option>`)
                }
                else {
                    $("#contact").append(`<option value='${i.ContactID}'>${i.ContactID}</option>`)
                    $("#tel").append(`<option value=${i.Tel1}>${i.Tel1}</option>`)

                }
            })

            $("#cussource").val(result.CusSources);
            $("#territory").val(result.Territery);
            $("#acid").val(result.ActID);
            $("#bpid").val(result.BPID);
            $("#doctypid").val(result.DoctypeID)
            $("#interestid").val(result.interestID)
            $("#potentialid").val(result.PotentialDetailsID)
            $("#summarydetailid").val(result.SummaryDetailsID)
            $("#masterid").val(result.ID)
            $("#ownerid").val(result.OwnerName);
            $("#saleempidd").val(result.SaleEmpName);
            $("#stageid").val(result.StagesID)

            if (result.DocNoID != null) {
                $("#doctypeid").val(result.DocNoID)
            }
            $("#code").val(result.BPCode);
            $("#name").val(result.BPName);
            //$("#empid").val(result.EmpID);
            $("#empowner").val(result.OwnerName);
            $("#next_number").val(result.OpportunityNo);
            $("#opportunityname").val(result.OpportunityName);
            $("#oppname").val(result.OpportunityName);
            $("#status").val(result.Status);

            $("#closingid").val(result.CloingPercentage);
            setDate("#startdate", result.StartDate.toString().split("T")[0]);
            setDate("#closingdate", result.ClosingDate.toString().split("T")[0]);
            setDate("#predictedclosingdate", result.PredictedClosingDate.toString().split("T")[0]);
            //stage detail

            __datastage = result.StageDetail;

            __datastage.forEach(i => {
                i.PotentailAmount = num.formatSpecial(i.PotentailAmount, disSetting.Amounts)
                i.WeightAmount = num.formatSpecial(i.WeightAmount, disSetting.Amounts)
            })

            $tablestage.bindRows(result.StageDetail);
            //partner detail
            $tablepartner.bindRows(result.PartnerDetail);
            __datapartner = result.PartnerDetail;
            //compeititor detail
            $tablecompetitor.bindRows(result.CompetitorDetail)
            __datacompetitor = result.CompetitorDetail;
            //potential detail
            $tablerange.bindRows(result.Descriptionpotential)
            __datapotentailselectdes = result.Descriptionpotential;
            $("#predictedclosingnum").val(result.PredictedClosingInNum)
            $("#predictedclosingtime").val(result.PredictedClosingInTime)
            $("#potentailamount").val(num.formatSpecial(result.PotentailAmount, disSetting.Amounts))
            $("#weightamount").val(num.formatSpecial(result.WeightAmount, disSetting.Amounts));
            $("#profitpercent").val(result.GrossProfit);
            $("#profittotal").val(num.formatSpecial(result.GrossProfitTotal, disSetting.Amounts));
            $("#levelID").val(result.Level);
            $tabledescription.bindRows(result.Descriptionsummary);
            __desselect = result.Descriptionsummary;
            if (result.IsWon) {
                $("#docNoid").val(result.SeriesDID)

            }
            //summary detail
            $("#summarydetailid").val(result.SummaryDetailsID);
            $("#open").prop("checked", result.IsOpen).prop("enabled", true);
            $("#lost").prop("checked", result.IsLost).prop("enabled", true);
            $("#won").prop("checked", result.IsWon).prop("enabled", true);
            //$("#levelID").append(` <option selected value=${result.LevelID}>${result.Level}</option>`)

            if (result.DocType != null) {
                $("#doctypeId").append(`<option value=${result.DoctypeID} selected> ${result.DocType} </option>`);
            }

            //$("#levelID").append(` <option selected value=${result.LevelID}>${result.Level}</option>`)
            $("#empid").append(` <option  selected value=${result.EmpID}>${result.SaleEmpName}</option>`);
            //$("#empid").val(result.SaleEmpName)
            if (result.DocType != null) {
                $("#doctypeId").val(result.DocType)
            }
            if ($("#won").is(':checked')) {
                if (result.DocType != null) {
                    $("#doctypeId").append(`<option value=${result.DoctypeID} selected> ${result.DocType} </option>`);
                }

                $("#doctypeId").removeAttr("disabled")
                $("#buttonlist").show();
            }
            $("#seriID").val(result.SeriesID);
            $("#docID").val(result.DoctypeID);

        }
    }

    //=================
    function bindBP(res) {

        let dialog = new DialogBox({
            content: {
                selector: ".bp_containers"
            },
            caption: "Business Partner"
        });
        dialog.invoke(function () {
            const __listBussinesPartner = ViewTable({
                keyField: "ID",
                selector: "#list-bp",
                paging: {
                    pageSize: 25,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "Choose"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down" id="close"  ></i>`,
                        on: {
                            "click": function (e) {
                                $("#bpid").val(e.data.ID)
                                $("#code").val(e.data.Code);
                                $("#territory").val(e.data.Territory);
                                $("#name").val(e.data.Name);
                                $("#contact").find('option').remove();
                                $("#tel").find('option').remove();
                                $("#acid").val(e.data.ActID);
                                $("#empid").val(e.data.SaleEMID);
                                $("#selectempid").val(e.data.SaleEMID);
                                $("#countac").val(e.data.Activities)
                                $("#cussource").val(e.data.CustomerSourceName);
                                $.grep($tablestage.yield(), function (item, i) {
                                    item.SaleEmpselectID = e.data.SaleEMID;
                                    item.Activety = e.data.Activities

                                });
                                $tablestage.updateColumn(e.data.SaleEMID, "SaleEmpselectID", e.data.SaleEMID);
                                $tablestage.updateColumn(e.data.Activities, "Activety", e.data.Activities);
                                e.data.ContactPeople.forEach(i => {

                                    if (i.SetAsDefualt) {
                                        $("#contact").append(`<option value='${i.ContactID}'  selected = "selected">${i.ContactID}</option>`)
                                        if (i.Tel1 != null) {
                                            $("#tel").append(`<option  value='${i.Tel1}>' selected = "selected">${i.Tel1}</option>`)
                                        }

                                    }
                                    else {
                                        $("#contact").append(`<option value='${i.ContactID}'>${i.ContactID}</option>`)
                                        if (i.Tel1) {
                                            $("#tel").append(`<option value=${i.Tel1}>${i.Tel1}</option>`)
                                        }


                                    }

                                })
                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listBussinesPartner.bindRows(res)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    //model box of edit busines partner
    $("#arow").click(function () {
        let id = $("#bpid").val();
        let code = $("#code").val();
        let name = $("#name").val();
        $("#editbpid").val(id);
        $("#editcode").val(code);
        $("#editname").val(name);

        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-edit-content"

            },
            caption: "SetUp-SaleEmployee"
        });
        dialog.reject(function () {

            dialog.shutdown();
        });

        dialog.confirm(function () {
            dialog.shutdown();
        });
        dialog.invoke(function () { });
    })

    ////set up employee
    $("#setupEm").click(function () {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-setupem-content"

            },
            caption: "SetUp-Employee"
        });
        dialog.invoke(function () {
            $gettablesetupemp = ViewTable({
                keyField: "LineID",
                selector: "#setup-emp",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 5
                },
                visibleFields: ["Name"],
                columns: [
                    {
                        name: "Name",
                        template: "<input type='text' class='empname' >",
                        on: {
                            "keyup": function (e) {

                                updatedata(__setupsaleemp, "LineID", e.key, "Name", this.value);
                            }
                        }
                    },
                ],

            });

            $("#create-saleem-description").click(function () {

                $.get("/Opportunity/GetEmptyTableSetupEmloyee", function (res) {

                    $gettablesetupemp.addRow(res);
                    __setupsaleemp = $gettablesetupemp.yield();
                })
            });
            $.get(
                "/Opportunity/GetEmptyTableSaleEmpDesDefault",
                function (res) {
                    $("#search-setupemp").on("keyup", function () {
                        let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                        var rgx = new RegExp(keyword);
                        var searcheds = $.grep(__setupsaleemp, function (item) {
                            if (item !== undefined) {

                                return item.Name.toLowerCase().match(keyword);

                            }
                        });
                        $gettablesetupemp.bindRows(searcheds);

                    });
                    $gettablesetupemp.bindRows(res);
                    __setupsaleemp = $gettablesetupemp.yield();
                }
            )

        })
        dialog.reject(function () {
            dialog.shutdown();
        });
        dialog.confirm(function () {
            var setemp = [];

            __setupsaleemp.forEach(i => {
                if (i.Name != "")
                    setemp.push(i);
            })
            $.ajax({
                url: "/Opportunity/InsertSaleEmp",
                type: "POST",
                dataType: "JSON",
                data: { saleEmployees: setemp },
                success: function (respones) {
                    $.get(
                        "/Opportunity/GetEmptyTableStageDefault",
                        function (res) {
                            $tablestage.bindRows(res);
                            __datastage = $tablestage.yield();
                        }
                    )
                    if (respones.Model.Action == 1) {
                        dialog.shutdown();
                    }
                    if (respones.IsApproved) {
                        new ViewMessage({
                            summary: {
                                selector: "#sms-emp"
                            },
                        }, respones.Model).refresh(1000);
                    } else {
                        new ViewMessage({
                            summary: {
                                selector: "#sms-emp"
                            }
                        }, respones.Model);
                    }

                    var data = "";
                    $.each(respones.Data, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.Name + '</option>';
                    });
                    $("#saleemployee").html(data);
                    $("#empid").html(data);

                }
            })
        });
    })
    $("#saleemployee").change(function () {
        coreData.updateItemData("empID", parseInt(this.value));
    });
    //model box of list employee

    $("#show-list-employee").click(function () {

        $.ajax({
            url: "/Opportunity/GetEmployee",
            type: "Get",
            dataType: "Json",
            success: function (response) {
                $("#ownerid").val();
                bindEm(response);
            }
        });
    });

    function bindEm(res) {
        let dialog = new DialogBox({
            content: {
                selector: ".em_containers"
            },
            caption: "List Employee"
        });
        dialog.invoke(function () {
            const __listEmployee = ViewTable({
                keyField: "ID",
                selector: "#list-em",
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["ID", "Name", "Position", "Choose"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down" id="close"  ></i>`,
                        on: {
                            "click": function (e) {


                                $("#empowner").val(e.data.Name);
                                $("#ownerid").val(e.data.Name);
                                updatedata(__updateowner, "LineID", e.key, "Name", e.data.Name);
                                updatedata(__updateowner, "LineID", e.key, "ID", e.data.ID);
                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listEmployee.bindRows(res)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }

    let _data = JSON.parse($("#model-data").text());
    const disSetting = _data.GeneralSetting;
    let config = {
        master: {
            keyField: "ID",
            value: _data
        }
    };
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });
    let coreData = new CoreItemData(config);
    $("#levelID").change(function () {
        coreData.updateItemData("LevelID", parseInt(this.value));
    });

    $("#modelinterest").click(function () {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-level-content"

            },
            caption: "SetUp-Level"
        });
        dialog.invoke(function () {
            $gettablesetupLevel = ViewTable({
                keyField: "LineID",
                selector: "#level-interest",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 5
                },
                visibleFields: ["Description"],
                columns: [
                    {
                        name: "Description",
                        template: "<input type='text' id='empname'>",
                        on: {
                            "keyup": function (e) {

                                updatedata(__setuplevel, "LineID", e.key, "Description", this.value);
                            }
                        }
                    },
                ],

            });

            $("#create-level-description").click(function () {

                $.get("/Opportunity/GetEmptyTableSetupLevel", function (res) {

                    $gettablesetupLevel.addRow(res);
                    __setuplevel.push(res);
                })
            });
            $.get(
                "/Opportunity/GetEmptyTableSetupInterestlevelDefalut",
                function (res) {
                    $("#search-levelinterest").on("keyup", function () {
                        let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                        var rgx = new RegExp(keyword);
                        var searcheds = $.grep(__setuplevel, function (item) {
                            if (item !== undefined) {
                                return item.Description.toLowerCase().match(keyword);

                            }
                        });
                        $gettablesetupLevel.bindRows(searcheds);

                    });
                    $gettablesetupLevel.bindRows(res);
                    __setuplevel = $gettablesetupLevel.yield();
                }
            )
        })
        dialog.reject(function () {

            dialog.shutdown();
        });
        dialog.confirm(function () {
            var setinterest = [];
            __setuplevel.forEach(i => {
                if (i.DescriptionLevel != "")
                    setinterest.push(i);
            })


            $.ajax({
                url: "/Opportunity/InsertInterestLevel",
                type: "POST",
                dataType: "JSON",
                data: { interestLevel: setinterest },
                success: function (__res) {
                    if (__res.IsApproved) {
                        new ViewMessage({
                            summary: {
                                selector: "#sms_level"
                            },
                        }, __res.Model).refresh(1000);
                    } else {
                        new ViewMessage({
                            summary: {
                                selector: "#sms_level"
                            }
                        }, __res.Model);
                    }


                    var data = "";
                    $.each(__res.Data, function (i, item) {
                        data +=
                            '<option value="' + item.Description + '">' + item.Description + '</option>';
                    });
                    $("#levelID").html(data);


                }
            })
            dialog.shutdown();

        });
    })

    function CoreItemData(config) {
        let __config = {
            master: {
                keyField: "",
                value: {}
            }
        };
        this.updateItemData = function (prop, value) {
            __config.master.value[prop] = value;
            return this;
        }
    }
    let $tablerange = ViewTable({
        keyField: "LineID",
        selector: "#description",
        indexed: true,
        paging: {
            enabled: true,
            pageSize: 5
        },
        visibleFields: [
            "selectDescription",
            "Action"

        ],
        columns: [
            {
                name: "selectDescription",
                template: "<select class='rangeid' value=0></select>",
                on: {
                    "change": function (e) {

                        updatedata(__datapotentailselectdes, "LineID", e.key, "interestID", this.value);
                        updatedata(__datapotentailselectdes, "LineID", e.key, "selectDescription", this.value);

                    }
                }
            },
        ],
        actions: [
            {
                template: `<i  class="fas fa-plus-square text-center" style="color:sandybrown;font-size:15px;margin-left:10px" id="setup"></i>`,
                on: {
                    "click": function (e) {
                        __setuprange();
                    }
                }
            }
        ]
    });
    $("#add-new-description").click(function () {
        $.get("/Opportunity/GetEmptyTable", function (res) {
            $tablerange.addRow(res);
            __datapotentailselectdes = $tablerange.yield();
        })
    });
    //get empty table  desfaul
    $.get(
        "/Opportunity/GetEmptyTablePotentailDesDefault",
        function (res) {
            $tablerange.bindRows(res);
            __datapotentailselectdes = $tablerange.yield();
        }
    )
    //summary table
    $("#add-new-reasons-description").click(function () {
        $.get("/Opportunity/GetEmptyTableReason", function (res) {

            $tablereasons.addRow(res);
            __data = $tablereasons.yield();

        })
    });
    let $tabledescription = ViewTable({
        keyField: "LineID",
        selector: "#description-summary",
        indexed: true,
        paging: {
            enabled: true,
            pageSize: 5
        },
        visibleFields: [
            "Descriptionselect",
            "Action"

        ],
        columns: [
            {
                name: "Descriptionselect",
                template: "<select class='setreasons' ></select>",
                on: {
                    "change": function (e) {

                        updatedata(__desselect, "LineID", e.key, "ReasonsID", this.value);
                        updatedata(__desselect, "LineID", e.key, "Descriptionselect", this.value);
                    }
                }
            },
        ],
        actions: [
            {
                template: `<i  class="fas fa-plus-square text-center" style="color:sandybrown;font-size:15px;margin-left:14px" id="setup"></i>`,
                on: {
                    "click": function (e) {
                        __setupreasons();
                    }
                }
            }
        ]
    });
    $("#add-new-description-summary").click(function () {
        $.get("/Opportunity/GetEmptyTableDescription", function (res) {

            $tabledescription.addRow(res);
            __desselect = $tabledescription.yield();
        })
    });
    //get empty table  desfaul
    $.get(
        "/Opportunity/GetEmptyTabledessummaryDefault",
        function (res) {
            $tabledescription.bindRows(res);

            __desselect = $tabledescription.yield();
        }
    )
    //setupreasons
    function __setupreasons() {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-setupreason-content"

            },
            caption: "SetUp-Reasons"
        });
        dialog.invoke(function () {
            $gettablesetupreasons = ViewTable({
                keyField: "LineID",
                selector: "#setup-reasons",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 5
                },
                visibleFields: ["Description"],
                columns: [
                    {
                        name: "Description",
                        template: "<input type='text' class='description'>",
                        on: {
                            "keyup": function (e) {

                                updatedata(__datasmmarydes, "LineID", e.key, "Description", this.value);
                            }
                        }
                    },
                ],


            });
            $("#add-new-reason").click(function () {
                $.get("/Opportunity/GetEmptyTableSetupReasons", function (res) {
                    $gettablesetupreasons.addRow(res)
                    __datasmmarydes = $gettablesetupreasons.yield();

                })
            });
            $.get(
                "/Opportunity/GetEmptyTableSetupreasonsDesDefault",
                function (res) {
                    $("#search-setupreason").on("keyup", function () {
                        let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                        var rgx = new RegExp(keyword);
                        var searcheds = $.grep(__datasmmarydes, function (item) {
                            if (item !== undefined) {
                                return item.Description.toLowerCase().match(keyword);

                            }
                        });
                        $gettablesetupreasons.bindRows(searcheds);

                    });
                    $gettablesetupreasons.bindRows(res);
                    __datasmmarydes = $gettablesetupreasons.yield();
                }
            )

        })
        dialog.reject(function () {
            dialog.shutdown();
        });
        dialog.confirm(function () {
            var setureason = [];
            __datasmmarydes.forEach(i => {
                if (i.Description != "")
                    setureason.push(i);
            })
            $.ajax({
                url: "/Opportunity/InsertSetupReasons",
                type: "POST",
                dataType: "JSON",
                data: { setupreasons: setureason },
                success: function (respones) {
                    if (respones.Model.Action == 1) {
                        dialog.shutdown();

                    }
                    if (respones.IsApproved) {
                        new ViewMessage({
                            summary: {
                                selector: "#sms_reason"
                            },
                        }, respones.Model).refresh(1000);
                    } else {
                        new ViewMessage({
                            summary: {
                                selector: "#sms_reason"
                            }
                        }, respones.Model);
                    }
                    var data = "<option>---select---</option>";
                    $.each(respones.Data, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.Description + '</option>';
                    });
                    $(".setreasons").html(data);

                }
            })

        });
    }

    //function setuprange
    function __setuprange() {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-setuprange-content"

            },
            caption: "SetUp-Interest-Range"
        });
        dialog.invoke(function () {
            $gettablesetup = ViewTable({
                keyField: "LineID",
                selector: "#setup-range",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 5
                },
                visibleFields: ["DescriptionLevel"],
                columns: [
                    {
                        name: "DescriptionLevel",
                        template: "<input type='text' class='rangedescription'>",
                        on: {
                            "keyup": function (e) {

                                updatedata(__datapotentaildes, "LineID", e.key, "DescriptionLevel", this.value);
                            }
                        }
                    },
                ],

            });
            $("#create-range-description").click(function () {
                $.get("/Opportunity/GetEmptyTableSetup", function (res) {
                    $gettablesetup.addRow(res)
                    __datapotentaildes = $gettablesetup.yield();
                })
            });
            $.get(
                "/Opportunity/GetEmptyTableRangeDesDefault",
                function (res) {
                    $("#search-setuprange").on("keyup", function () {
                        let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                        var rgx = new RegExp(keyword);
                        var searcheds = $.grep(__datapotentaildes, function (item) {
                            if (item !== undefined) {
                                return item.DescriptionLevel.toLowerCase().match(keyword);

                            }
                        });
                        $gettablesetup.bindRows(searcheds);

                    });
                    $gettablesetup.bindRows(res);
                    __datapotentaildes = $gettablesetup.yield();
                }
            )

        })
        dialog.reject(function () {
            dialog.shutdown();
        });
        dialog.confirm(function () {
            var setuprange = [];
            __datapotentaildes.forEach(i => {
                if (i.DescriptionLevel != "")
                    setuprange.push(i);

            })

            $.ajax({
                url: "/Opportunity/InsertInterestRange",
                type: "POST",
                dataType: "JSON",
                data: { interestRange: setuprange },
                success: function (respones) {
                    if (respones.Model.Action == 1) {
                        dialog.shutdown();
                    }
                    if (respones.IsApproved) {
                        new ViewMessage({
                            summary: {
                                selector: "#sms_range"
                            },
                        }, respones.Model).refresh(1000);
                    } else {
                        new ViewMessage({
                            summary: {
                                selector: "#sms_range"
                            }
                        }, respones.Model);
                    }
                    var data = "<option>---selecte---</option>";
                    $.each(respones.Data, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.DescriptionLevel + '</option>';
                    });
                    $(".rangeid").html(data);

                }
            })


        });
    }

    //setup stage
    function __setupstage() {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-setupstage-content"

            },
            caption: "SetUp-Stage"
        });
        dialog.invoke(function () {
            $gettablesetupstage = ViewTable({
                keyField: "LineID",
                selector: "#setup-stage",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 5
                },
                visibleFields: ["Name", "StageNo", "ClosingPercentTage", "Choose"],
                columns: [
                    {
                        name: "Name",
                        template: "<input type='text' class='name'>",
                        on: {
                            "keyup": function (e) {

                                updatedata(__setupstages, "LineID", e.key, "Name", this.value);
                            }
                        }
                    },
                    {
                        name: "StageNo",
                        template: "<input type='text' class='stageno'>",
                        on: {
                            "keyup": function (e) {

                                updatedata(__setupstages, "LineID", e.key, "StageNo", this.value);
                            }
                        }
                    },
                    {
                        name: "ClosingPercentTage",
                        template: "<input type='text' class='closingtage'>",
                        on: {
                            "keyup": function (e) {

                                updatedata(__setupstages, "LineID", e.key, "ClosingPercentTage", this.value);
                            }
                        }
                    },

                ],

            });
            $("#add-new-stage").click(function () {
                $.get("/Opportunity/GetEmptyTableSetupStage", function (res) {
                    $gettablesetupstage.addRow(res)
                    __setupstages = $gettablesetupstage.yield();

                })
            });
            $.get(
                "/Opportunity/GetEmptyTableSetupStageDesDefault",
                function (res) {
                    $("#search-setupstage").on("keyup", function () {
                        let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                        var rgx = new RegExp(keyword);
                        var searcheds = $.grep(__setupstages, function (item) {
                            if (item !== undefined) {

                                return item.Name.toLowerCase().match(keyword);

                            }
                        });
                        $gettablesetupstage.bindRows(searcheds);

                    });
                    $gettablesetupstage.bindRows(res);
                    __setupstages = $gettablesetupstage.yield();
                }
            )

        })
        dialog.reject(function () {
            dialog.shutdown();
        });
        dialog.confirm(function () {
            var setstage = [];
            __setupstages.forEach(i => {
                if (i.Name != "" || i.StageNo != 0)
                    setstage.push(i);
            })

            $.ajax({
                url: "/Opportunity/InsertSetupStage",
                type: "POST",
                dataType: "JSON",
                data: { setupstage: setstage },
                success: function (respones) {

                    if (respones.Model.Action == 1) {

                        dialog.shutdown();
                    }
                    if (respones.IsApproved) {

                        new ViewMessage({
                            summary: {
                                selector: "#sms_stage"
                            },
                        }, respones.Model).refresh(1000);
                    } else {
                        new ViewMessage({
                            summary: {
                                selector: "#sms_stage"
                            }
                        }, respones.Model);
                    }
                    var data = "<option>---select---</option>";
                    $.each(respones.Data, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.Name + '</option>';
                    });
                    $("#perstage").html(data);

                }
            })


        });
    }
    //model stage
    var d = $("#empid").val();
    $("#selectempid").val(d);
    let $tablestage = ViewTable({
        keyField: "LineID",
        selector: ".data-stage",
        indexed: false,
        paging: {
            enabled: false,
            pageSize: 5
        },
        visibleFields: [
            "StartDate",
            "CloseDate",
            "SaleEmpselect",
            "Nameselect",
            "SetupStage",
            "Percent",
            "PotentailAmount",
            "WeightAmount",
            "ShowBpsDoc",
            "Doctypeselect",
            "DocNo",
            "Activety",
            "Owner",
        ],
        columns: [
            {
                name: "StartDate",
                template: "<input type='date' class='input-box-kernel' id='startdates'>",
                on: {
                    "change": function (e) {

                        updatedata(__datastage, "LineID", e.key, "StartDate", this.value);
                    }
                }
            },
            {
                name: "CloseDate",
                template: "<input type='date' class='input-box-kernel' id='closedates'>",
                on: {
                    "change": function (e) {

                        updatedata(__datastage, "LineID", e.key, "CloseDate", this.value);
                    }
                }
            },
            {
                name: "SaleEmpselect",
                template: "<select id='selectempid'></select>",
                on: {
                    "change": function (e) {

                        updatedata(__datastage, "LineID", e.key, "SaleEmpselectID", this.value);
                    }
                }
            },
            {
                name: "Nameselect",
                template: "<select id='perstage' ></select>",
                on: {
                    "change": function (e) {
                        var poten = num.toNumberSpecial(e.data.PotentailAmount)
                        nameselect = this.value;
                        if (nameselect != 0) {
                            $.ajax({
                                url: "/Opportunity/Getsupstage",
                                type: "Get",
                                data: { nameselect: nameselect },
                                dataType: "Json",
                                success: function (response) {
                                    var percent = response.ClosingPercentTage;
                                    var potentialamount = poten;
                                    var calweightamount = (num.toNumberSpecial(percent) / 100) * num.toNumberSpecial(potentialamount);
                                    $("#closingid").val(response.ClosingPercentTage);
                                    $("#weightamountt").val(calweightamount);
                                    $("#weightamount").val(calweightamount);
                                    $tablestage.updateColumn(e.key, "StagesID", response.ID)
                                    updatedata(__datastage, "LineID", response.LineID, "StagesID", response.ID);

                                    $tablestage.updateColumn(e.key, "Percent", response.ClosingPercentTage)
                                    updatedata(__datastage, "LineID", response.LineID, "Percent", response.ClosingPercentTage);
                                    $tablestage.updateColumn(e.key, "WeightAmount", response.ClosingPercentTage)
                                    $tablestage.updateColumn(e.key, "WeightAmount", num.formatSpecial(calweightamount, disSetting.Amounts))

                                    updatedata(__datastage, "LineID", response.LineID, "WeightAmount", num.formatSpecial(calweightamount, disSetting.Amounts));

                                    let closing = response.ClosingPercentTage;
                                    if (closing > 100)
                                        closing = 100;
                                    $(".closingpercent").text(closing);
                                    $(".closingpercent").css("background-color", "blue");
                                    $(".closingpercent").css("width", `${closing}%`);

                                }
                            });
                        }
                        else {
                            $(".closingpercent").text(0);
                            $(".closingpercent").css("background-color", "transparent");
                            $(".closingpercent").css("width", `0%`);
                        }
                        updatedata(__datastage, "LineID", e.key, "StageID", this.value);
                        updatedata(__datastage, "LineID", e.key, "Nameselect", this.value);
                    }
                }
            },
            {
                name: "SetupStage",
                template: '<i  class="fas fa-plus-square text-center" style="color:sandybrown;font-size:15px;margin-left:40%"  ></i>',
                on: {
                    "click": function (e) {
                        __setupstage();
                    }
                }
            },
            {
                name: "Percent",
                template: "<input class='input-box-kernel' readonly id='percent'>",
                on: {
                    "keyup": function (e) {
                        var percent = this.value;
                        $("#percent").val(percent);
                        updatedata(__datastage, "LineID", e.key, "Percent", this.value);
                    }
                }
            },
            {
                name: "PotentailAmount",
                template: "<input type='text' readonly class='input-box-kernel'  id='stagepotentailamount'>",
                on: {
                    "keyup": function (e) {
                        var potentialamount = $("#potentailamount").val();
                        updatedata(__datastage, "LineID", e.key, "PotentailAmount", potentialamount);
                    }
                }
            },
            {
                name: "WeightAmount",
                template: "<input type='text' readonly  class='input-box-kernel' id='weightamountt'>",
                on: {
                    "keyup": function (e) {
                        $(this).asNumber();
                        var weight1 = $("#weightamount").val();
                        if (this.value > weight1) {
                            var po = parseFloat(this.value) * parseFloat($("#potentailamount").val())
                            $("#stagepotentailamount").val(po);
                            $("#potentailamount").val(po);
                        }
                        else if (this.value < weight1) {
                            var po = parseFloat($("#potentailamount").val())
                            var thiss = this.vvalue;
                        }
                        $("#weightamount").val(this.value);
                        updatedata(__datastage, "LineID", e.key, "WeightAmount", this.value);
                    }
                }
            },
            {
                name: "ShowBpsDoc",
                template: "<input type='checkbox' class='input-box-kernel' id='showbp'>",
                on: {
                    "click": function (e) {
                        const active = $(this).prop("checked") ? true : false;
                        updatedata(__datastage, "LineID", e.key, "ShowBpsDoc", active);
                    }
                }
            },
            {
                name: "Doctypeselect",
                template: "<select id='docidd'></select>",
                on: {
                    "change": function (e) {
                        $("#doctypeidd").val(this.value);

                        if ($("#doctypeidd").val() == 6) {

                            $("#add-newmemo").hide();
                            $("#add-neworder").hide();
                            $("#add-newdelivery").hide();
                            $("#add-newar").hide();
                            $("#add-newqutoe").show();
                        }
                        if ($("#doctypeidd").val() == 7) {

                            $("#add-newqutoe").hide();
                            $("#add-newmemo").hide();
                            $("#add-newdelivery").hide();
                            $("#add-newar").hide();
                            $("#add-neworder").show();
                        }
                        if ($("#doctypeidd").val() == 8) {

                            $("#add-newqutoe").hide();
                            $("#add-newmemo").hide();
                            $("#add-newar").hide();
                            $("#add-neworder").hide();
                            $("#add-newdelivery").show()

                        }
                        if ($("#doctypeidd").val() == 9) {

                            $("#add-newqutoe").hide();
                            $("#add-newmemo").hide();
                            $("#add-neworder").hide();
                            $("#add-newdelivery").hide()
                            $("#add-newar").show();
                        }
                        if ($("#doctypeidd").val() == 10) {

                            $("#add-newqutoe").hide();
                            $("#add-neworder").hide();
                            $("#add-newdelivery").hide()
                            $("#add-newar").hide();
                            $("#add-newmemo").show()
                        }
                        updatedata(__datastage, "LineID", e.key, "DoctypeID", this.value);

                    }
                }
            },
            {
                name: "DocNo",
                template: "<input type='text' readonly id='dcno'>",
                on: {
                    "click": function (e) {

                        $.ajax({
                            url: "/Opportunity/GetDocumenttype",
                            type: "Get",
                            data: { doctype: $("#doctypeidd").val() },
                            dataType: "Json",
                            success: function (response) {
                                binddatasaleorder(response, e);
                            }
                        });
                    }
                }
            },
            {
                name: "Activety",
                //template: '<i class="fas fa-arrow-right " style="color:#e95d46;font-size:16px" id="actid" ></i>',
                template: "<input type='text' readonly id='countac'>",
                on: {
                    "click": function (e) {
                        $.ajax({
                            url: "/Opportunity/GetdataActivity",
                            type: "Get",
                            dataType: "Json",
                            data: { id: $("#bpid").val() },
                            success: function (data) {
                                Activity(data, e);
                            }
                        });
                        //var bpid = $("#bpid").val();
                        //var acid = $("#acid").val();
                        ////if (bpid != 0 && acid != 0) {
                        ////    location.href = "/Opportunity/Detail?bpid=" + bpid;
                        ////}
                        //updatedata(__datastage, "LineID", e.key, "ActivetyID", acid);
                    }
                }
            },
            {
                name: "Owner",
                template: "<input type='text' readonly id='owner'>",
                on: {
                    "click": function (e) {
                        $.ajax({
                            url: "/Opportunity/GetEmployee",
                            type: "Get",
                            dataType: "Json",
                            success: function (response) {

                                binddataemp(response, e);
                            }
                        });
                    }
                }
            },


        ],

    });
    //=========Model box of activity===========
    function Activity(data, _e) {
        let dialog = new DialogBox({
            content: {
                selector: ".activity_containers"
            },
            caption: "Bussines Partner"
        });
        dialog.invoke(function () {
            const __listBussinesPartner = ViewTable({
                keyField: "ID",
                selector: "#list-activity",
                paging: {
                    pageSize: 15,
                    enabled: true
                },
                visibleFields: ["BPCode", "BPName", "TypeName", "StartTime", "EndTime"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down" id="close"  ></i>`,
                        on: {
                            "click": function (e) {
                                //location.href = "/BusinessPartner/Activity?number=" + e.data.Number;
                                if (e.data.Number != 0) {
                                    location.href = "/BusinessPartner/Activity?number=" + e.data.Number;
                                }

                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listBussinesPartner.bindRows(data)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    $("#create-stage").click(function () {
        $.get("/Opportunity/GetEmptyTableStage", function (res) {

            $tablestage.addRow(res);
            __datastage.push(res)
        })
    });

    // GetEmptyTableStageDefault
    $.get(
        "/Opportunity/GetEmptyTableStageDefault",
        function (res) {
            $tablestage.bindRows(res);
            __datastage = $tablestage.yield();

        }
    )


    //get table competitor
    $("#add-new-tbcompetitor").click(function () {
        $.get("/Opportunity/GetEmptyTableCompetitor", function (res) {
            $tablecompetitor.addRow(res);
            __datacompetitor = $tablecompetitor.yield();

        })
    });
    //get empty table defaul competitor
    $.get(
        "/Opportunity/GetEmptyTableCompeitorDefault",
        function (res) {
            $tablecompetitor.bindRows(res);
            __datacompetitor = $tablecompetitor.yield();
        }
    )
    //tabel competitor
    let $tablecompetitor = ViewTable({
        keyField: "LineID",
        selector: ".data-competitor",
        indexed: true,
        paging: {
            enabled: true,
            pageSize: 5
        },
        visibleFields: [
            "Nameselect",
            "SetupCompetitor",
            "ThreaLevel",
            "Remark",
        ],
        columns: [
            {
                name: "Nameselect",
                template: "<select class='nameselectid'></select>",
                on: {
                    "change": function (e) {
                        $("#doctypid").val(e.data.ID);
                        updatedata(__datacompetitor, "LineID", e.key, "NameCompetitorID", this.value);

                    }
                }
            },
            {
                name: "SetupCompetitor",
                template: `<i  class="fas fa-plus-square text-center" style="color:sandybrown;font-size:15px;margin-left:94px" id="setup"></i>`,
                on: {
                    "click": function (e) {
                        __setupcompetitor();
                    }
                }
            },
            {
                name: "ThreaLevel",
                template: "<select><option>--Select--</option></select>",
                on: {
                    "change": function (e) {

                        updatedata(__datacompetitor, "LineID", e.key, "ThrealevelID", this.value)


                    }
                }
            },
            {
                name: "Remark",
                template: "<input type='text'>",
                on: {
                    "change": function (e) {
                        updatedata(__datacompetitor, "LineID", e.key, "Remark", this.value);
                    }
                }
            },
        ],

    });
    //table partner
    let $tablepartner = ViewTable({
        keyField: "LineID",
        selector: ".data-partner",
        indexed: true,
        paging: {
            enabled: true,
            pageSize: 5
        },
        visibleFields: [
            "Nameselect",
            "SetupPartner",
            "Relationshipselect",
            "SetupRelationship",
            "RelatedBp",
            "Remark"
        ],
        columns: [
            {
                name: "Nameselect",
                template: "<select class='setname'></select>",
                on: {
                    "change": function (e) {

                        updatedata(__datapartner, "LineID", e.key, "NamePartnerID", this.value);


                    }
                }
            },
            {
                name: "SetupPartner",
                template: `<i  class="fas fa-plus-square text-center" style="color:sandybrown;font-size:15px;margin-left:40px" ></i>`,
                on: {
                    "click": function (e) {
                        __setuppartner();
                    }
                }
            },
            {
                name: "Relationshipselect",
                template: "<select class='setrelationsip'></select>",
                on: {
                    "change": function (e) {
                        updatedata(__datapartner, "LineID", e.key, "RelationshipID", this.value);

                    }
                }
            },
            {
                name: "SetupRelationship",
                template: `<i  class="fas fa-plus-square text-center" style="color:sandybrown;font-size:15px;margin-left:55px" ></i>`,
                on: {
                    "click": function (e) {
                        __setuprelationship();
                    }
                }
            },
            {
                name: "RelatedBp",
                template: "<input type='text' readonly>",
                on: {
                    "click": function (e) {
                        $.ajax({
                            url: "/Opportunity/GetBP",
                            type: "Get",
                            dataType: "Json",
                            success: function (response) {
                                getBussinesspartner(response, e);
                            }
                        });
                    }
                }
            },

            {
                name: "Remark",
                template: "<input type='text' >",
                on: {
                    "keyup": function (e) {
                        updatedata(__datapartner, "LineID", e.key, "Remark", this.value);
                    }
                }
            },

        ],


    });
    //get table partner
    $("#add-new-tbpartner").click(function () {
        $.get("/Opportunity/GetEmptyTablePartner", function (res) {

            $tablepartner.addRow(res);
            __datapartner = $tablepartner.yield();

        })
    });

    //get empty table
    $.get(
        "/Opportunity/GetEmptyTablePartnerDDefault",
        function (res) {
            $tablepartner.bindRows(res);
            __datapartner = $tablepartner.yield();
        }
    )
    //search by code and name of bp
    $("#next_number").on("keypress", function (e) {
        if (e.which === 13 && $("#next_number").val()) {
            $("#btn-find").hide();
            $.ajax({
                url: "/Opportunity/FindOpportunity",
                data: { number: $("#next_number").val().trim() },
                success: function (result) {
                    search(result)

                }

            });
        }
    });

    //model of list owner
    function binddataemp(data, _e) {
        let dialog = new DialogBox({
            content: {
                selector: ".dataemp_container"
            },
            caption: "List Of Employee"
        });
        dialog.invoke(function () {
            const __listEmployees = ViewTable({
                selector: "#list-dataemp",
                keyField: "LineID",
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["ID", "Name", "Position", "Choose",],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down"  ></i>`,
                        on: {
                            "click": function (e) {
                                $tablestage.updateColumn(_e.key, "OwnerID", e.data.ID)
                                updatedata(__dataemp, "LineID", data.LineID, "OwnerID", e.data.ID);

                                $tablestage.updateColumn(_e.key, "Owner", e.data.Name)
                                updatedata(__dataemp, "LineID", data.LineID, "Owner", e.data.Name);
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            __listEmployees.bindRows(data)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
    }
    //model of list sale order
    function binddatasaleorder(data, _e) {
        let dialog = new DialogBox({
            content: {
                selector: ".saleorder_container"
            },
            caption: "List Of DocType"
        });
        dialog.invoke(function () {

            const __listSaleorder = ViewTable({
                selector: "#list-saleorder",
                keyField: "LineID",
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: ["SOID", "InvoiceNumber", "PostingDate", "CusName", "Choose"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down" id="choose"  ></i>`,
                        on: {
                            "click": function (e) {
                                $tablestage.updateColumn(_e.key, "DocNo", e.data.InvoiceNumber);
                                updatedata(__data, "LineID", data.LineID, "DocNo", e.data.InvoiceNumber);
                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listSaleorder.bindRows(data)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
    }
    //model of set up competitor
    function __setupcompetitor() {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-setupcompetitor-content"

            },
            caption: "Competotor-Setup"
        });
        dialog.invoke(function () {
            $gettablesetupcompetitor = ViewTable({
                keyField: "LineID",
                selector: "#setup-competitor",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 5
                },
                visibleFields: ["Name", "ThreaLevel", "Detail"],
                columns: [
                    {
                        name: "Name",
                        template: "<input type='text' class='name'>",
                        on: {
                            "keyup": function (e) {
                                updatedata(__setupnewcompetitor, "LineID", e.key, "Name", this.value);
                            }
                        }
                    },
                    {
                        name: "ThreaLevel",
                        template: "<select class='threalevel'><option>---select---</option></select>",
                        on: {
                            "change": function (e) {
                                updatedata(__setupnewcompetitor, "LineID", e.key, "ThreaLevelID", this.value);
                            }
                        }
                    },
                    {
                        name: "Detail",
                        template: "<input type='text' class='detail'>",
                        on: {
                            "keyup": function (e) {
                                updatedata(__setupnewcompetitor, "LineID", e.key, "Detail", this.value);

                            }
                        }
                    },

                ],

            });
            $("#add-new-setupcompetitor").click(function () {
                $.get("/Opportunity/GetEmptyTableSetupCompetitor", function (res) {
                    $gettablesetupcompetitor.addRow(res)
                    __setupnewcompetitor = $gettablesetupcompetitor.yield();

                })
            });
            $.get(
                "/Opportunity/GetEmptyTableSetupcompetitorDesDefault",
                function (res) {
                    $("#search-setupcompeitior").on("keyup", function () {
                        let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                        var rgx = new RegExp(keyword);
                        var searcheds = $.grep(__setupnewcompetitor, function (item) {
                            if (item !== undefined) {
                                return item.Name.toLowerCase().match(keyword);

                            }
                        });
                        $gettablesetupcompetitor.bindRows(searcheds);

                    });
                    $gettablesetupcompetitor.bindRows(res);
                    __setupnewcompetitor = $gettablesetupcompetitor.yield();
                }
            )

        })
        dialog.reject(function () {
            dialog.shutdown();
        });
        dialog.confirm(function () {
            var setcompetitor = [];
            __setupnewcompetitor.forEach(i => {
                if (i.Name != "")
                    setcompetitor.push(i);
            })
            $.ajax({
                url: "/Opportunity/InsertsetupCompetitor",
                type: "POST",
                dataType: "JSON",
                data: { setupcompetitor: setcompetitor },
                success: function (respones) {
                    if (respones.Model.Action == 1) {
                        dialog.shutdown();
                    }
                    if (respones.IsApproved) {
                        new ViewMessage({
                            summary: {
                                selector: "#sms-competitor"
                            },
                        }, respones.Model).refresh(1000);
                    } else {
                        new ViewMessage({
                            summary: {
                                selector: "#sms-competitor"
                            }
                        }, respones.Model);
                    }
                    var data = "<option>---select---</option>";
                    $.each(respones.Data, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.Name + '</option>';
                    });
                    $(".nameselectid").html(data);

                }
            })


        });
    }
    //model of set up partner
    function __setuppartner() {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-setuppartner-content"

            },
            caption: "SetUp-Partner"
        });
        dialog.invoke(function () {
            $gettablesetuppartner = ViewTable({
                keyField: "LineID",
                selector: "#setup-partner",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 5
                },
                visibleFields: ["Name", "DFRelationshipselect", "RelatedBp", "Detail"],
                columns: [
                    {
                        name: "Name",
                        template: "<input type='text' class='setname'>",
                        on: {
                            "keyup": function (e) {
                                updatedata(__setuppartners, "LineID", e.key, "Name", this.value);
                            }
                        }
                    },
                    {
                        name: "DFRelationshipselect",
                        template: "<select class='relationid' class='dfrelationship'></select>",
                        on: {
                            "change": function (e) {
                                updatedata(__setuppartners, "LineID", e.key, "DFRelationship", this.value);
                            }
                        }
                    },
                    {
                        name: "Detail",
                        template: "<input type='text' class='detail' >",
                        on: {
                            "keyup": function (e) {
                                updatedata(__setuppartners, "LineID", e.key, "Detail", this.value);

                            }
                        }
                    },
                    {
                        name: "RelatedBp",
                        template: "<input type='text'  readonly class='relatedbp'>",
                        on: {
                            "click": function (e) {
                                $.ajax({
                                    url: "/Opportunity/GetBP",
                                    type: "Get",
                                    dataType: "Json",
                                    success: function (response) {
                                        bindBussinesspartner(response, e);
                                    }
                                });
                            }
                        }
                    },

                ],

            });
            $("#add-new-setuppartner").click(function () {
                $.get("/Opportunity/GetEmptyTableSetupPartner", function (res) {
                    $gettablesetuppartner.addRow(res)
                    __setuppartners = $gettablesetuppartner.yield();

                })
            });
            $.get(
                "/Opportunity/GetEmptyTableSetuppartnerDesDefault",
                function (res) {
                    $("#search-setuppartner").on("keyup", function () {
                        let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                        var rgx = new RegExp(keyword);
                        var searcheds = $.grep(__setuppartners, function (item) {
                            if (item !== undefined) {

                                return item.Name.toLowerCase().match(keyword);

                            }
                        });
                        $gettablesetuppartner.bindRows(searcheds);

                    });
                    $gettablesetuppartner.bindRows(res);
                    __setuppartners = $gettablesetuppartner.yield();
                }
            )


        })
        dialog.reject(function () {
            dialog.shutdown();
        });
        dialog.confirm(function (e) {

            var setpartner = [];

            __setuppartners.forEach(i => {
                if (i.Name != "" || i.RelatedBpID != 0 || i.Detail != "")
                    setpartner.push(i);

            })
            $.ajax({
                url: "/Opportunity/InsertSetuppartner",
                type: "POST",
                dataType: "JSON",
                data: { setuppartner: setpartner },
                success: function (respones) {
                    $.get(
                        "/Opportunity/GetprePartner",
                        function (res) {
                            if (isValidArray(res)) {

                                $gettablesetuppartner.bindRows(res);
                                res.forEach(i => {
                                    __setuppartners.push(i);
                                })
                            }
                        }
                    )
                    if (respones.Model.Action == 1) {
                        dialog.shutdown();
                    }
                    if (respones.IsApproved) {
                        new ViewMessage({
                            summary: {
                                selector: "#sms-partner"
                            },
                        }, respones.Model).refresh(1000);
                    } else {
                        new ViewMessage({
                            summary: {
                                selector: "#sms-partner"
                            }
                        }, respones.Model);
                    }
                    var data = "<option>---select---</option>";
                    $.each(respones.Data, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.Name + '</option>';
                    });
                    $(".setname").html(data);

                }
            })

        });
    }
    //model of set up relationship
    function __setuprelationship() {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",
            content: {
                selector: "#active-setuprelationship-content"

            },
            caption: "Relationship-Setup"
        });
        dialog.invoke(function () {
            $gettablesetuprelationship = ViewTable({
                keyField: "LineID",
                selector: "#setup-relationship",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 5
                },
                visibleFields: ["RelationshipDscription"],
                columns: [
                    {
                        name: "RelationshipDscription",
                        template: "<input type='text' class='relationshipdes'>",
                        on: {
                            "keyup": function (e) {
                                updatedata(__setupnewrelationship, "LineID", e.key, "RelationshipDscription", this.value);
                            }
                        }
                    },

                ],
            });
            $("#add-new-setuprelationship").click(function () {
                $.get("/Opportunity/GetEmptyTableSetupRelationship", function (res) {
                    $gettablesetuprelationship.addRow(res)
                    __setupnewrelationship = $gettablesetuprelationship.yield();


                })
            });
            $.get(
                "/Opportunity/GetEmptyTableSetuprelationshipDesDefault",
                function (res) {
                    $("#search-setuprelationship").on("keyup", function () {
                        let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                        var rgx = new RegExp(keyword);
                        var searcheds = $.grep(__setupnewrelationship, function (item) {
                            if (item !== undefined) {

                                return item.RelationshipDscription.toLowerCase().match(keyword);

                            }
                        });
                        $gettablesetuprelationship.bindRows(searcheds);

                    });
                    $gettablesetuprelationship.bindRows(res);
                    __setupnewrelationship = $gettablesetuprelationship.yield();
                }
            )

        })
        dialog.reject(function () {
            dialog.shutdown();
        });
        dialog.confirm(function () {
            var setrelationship = [];
            __setupnewrelationship.forEach(i => {
                if (i.RelationshipDscription != "")
                    setrelationship.push(i);
            })
            $.ajax({
                url: "/Opportunity/Insertsetuprelationship",
                type: "POST",
                dataType: "JSON",
                data: { setuprelationship: setrelationship },
                success: function (respones) {
                    if (respones.Model.Action == 1) {
                        dialog.shutdown();
                    }
                    if (respones.IsApproved) {
                        new ViewMessage({
                            summary: {
                                selector: "#sms-relationship"
                            },
                        }, respones.Model).refresh(1000);
                    } else {
                        new ViewMessage({
                            summary: {
                                selector: "#sms-relationship"
                            }
                        }, respones.Model);
                    }
                    var data = "<option>--selecte---</option>";
                    $.each(respones.Data, function (i, item) {
                        data +=
                            '<option value="' + item.ID + '">' + item.RelationshipDscription + '</option>';
                    });
                    $(".setrelationsip").html(data);


                }
            })


        });
    }

    //Bind tble bp of seuppartner
    function bindBussinesspartner(data, _e) {
        let dialog = new DialogBox({
            content: {
                selector: ".bp_containers"
            },
            caption: "Bussines Partner"
        });
        dialog.invoke(function () {
            const __listBussinesPartner = ViewTable({
                keyField: "LineID",
                selector: "#list-bp",
                paging: {
                    pageSize: 15,
                    enabled: false
                },
                visibleFields: ["Code", "Name", "ContactPerson", "Tel", "Choose"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down" id="close"  ></i>`,
                        on: {
                            "click": function (e) {

                                $("#code").val(e.data.Code);
                                $("#name").val(e.data.Name);
                                $("#cussource").val(e.data.CustomerSourceName);

                                $gettablesetuppartner.updateColumn(_e.key, "RelatedBp", e.data.Code);
                                updatedata(__relatedbp, "LineID", data.LineID, "RelatedBp", e.data.Code);
                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listBussinesPartner.bindRows(data)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    //get bp of partner
    function getBussinesspartner(data, _e) {
        let dialog = new DialogBox({
            content: {
                selector: ".bp_containers"
            },
            caption: "Bussines Partner"
        });
        dialog.invoke(function () {
            const __listBussinesPartner = ViewTable({
                keyField: "ID",
                selector: "#list-bp",
                paging: {
                    pageSize: 15,
                    enabled: false
                },
                visibleFields: ["Code", "Name", "Choose"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-circle-down" id="close"  ></i>`,
                        on: {
                            "click": function (e) {
                                $tablepartner.updateColumn(_e.key, "RelatedBp", e.data.Name)
                                updatedata(__data, "LineID", data.LineID, "RelatedBp", e.data.Name);
                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listBussinesPartner.bindRows(data)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    $("#showing-list-doctypeNo").click(function () {
        $.ajax({
            url: "/Opportunity/GetDoctypeNo",
            type: "Get",
            data: { ID: $("#doctypeId").val() },
            dataType: "Json",
            success: function (response) {
                if (response.Error) {
                    new DialogBox({
                        content: response.Message,
                        icon: "warning"
                    });
                } else {
                    doctypeno(response);
                }

            }
        });
    });
    //model of data doctypeno
    function doctypeno(res) {
        let dialog = new DialogBox({
            content: {
                selector: ".doctypeno_containers"
            },
            caption: "List Doctype"
        });
        dialog.invoke(function () {
            const __listDoctypeNo = ViewTable({
                keyField: "ID",
                selector: "#list-doctypeno",
                paging: {
                    pageSize: 10,
                    enabled: false
                },
                visibleFields: ["InvoiceNo", "PostingDate", "DueDate", "CustomerName", "Remark", "Choose"],
                actions: [
                    {
                        name: "Choose",
                        template: `<i class="fas fa-arrow-circle-down" id="close"  ></i>`,
                        on: {
                            "click": function (e) {

                                $("#seriID").val(e.data.SeriesID)
                                $("#docNoid").val(e.data.SeriesDID);
                                $("#docID").val(e.data.DoctypeID);
                                summaryObj.DoctypeID = e.data.DoctypeID
                                summaryObj.SeriesID = e.data.SeriesID
                                summaryObj.SeriesDID = e.data.SeriesDID
                                dialog.shutdown();
                            }
                        },
                    }
                ]
            });
            __listDoctypeNo.bindRows(res)
        });
        dialog.confirm(function () {
            dialog.shutdown();
        })
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function updatedata(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue)
                    i[prop] = propValue
            })
        }
    }
    //array of data
    let master = [];
    master.push(opportunity_master);
    $("#submit-data").click(function () {
        const opportunity_master = master[0];
        if ($("#lost").is(':checked') || $("#open").is(':checked')) {
            summaryObj.DoctypeID = 0;
            summaryObj.SeriesID = 0;
        }
        else {
            summaryObj.DoctypeID = $("#doctypid").val();
            summaryObj.SeriesID = $("#seriID").val();
        }
        opportunity_master.BPCode = $("#code").val();
        //opportunity_master.SaleEmpName = $("#saleempname").val();
        //opportunity_master.EmpName = $("#empname").val();
        //opportunity_master.BPName = $("#name").val();
        opportunity_master.SaleEmpID = $("#empid").val();
        opportunity_master.BPID = $("#bpid").val();
        //opportunity_master.Employee = $("#empid").val();
        opportunity_master.OpportunityNo = $("#next_number").val();
        opportunity_master.Owner = $("#ownerid").val();
        opportunity_master.ClosingDate = $("#closingdate").val();

        opportunity_master.StartDate = $("#startdate").val();
        opportunity_master.OpportunityName = $("#opportunityname").val();
        opportunity_master.OpportunityNo = $("#next_number").val();
        opportunity_master.Status = $("#status").val();
        opportunity_master.CloingPercentage = $("#closingid").val();
        //potenail-detail

        potentialObj.InterestID = $("#interestid").val();
        potentialObj.PotentailAmount = $("#potentailamount").val();
        potentialObj.GrossProfit = $("#profitpercent").val();
        potentialObj.GrossProfitTotal = $("#profittotal").val();
        potentialObj.Level = $("#levelID").val();
        potentialObj.PredictedClosingInNum = $("#predictedclosingnum").val();
        potentialObj.PredictedClosingDate = $("#predictedclosingdate").val();
        potentialObj.WeightAmount = $("#weightamount").val();
        potentialObj.PredictedClosingInTime = $("#predictedclosingtime").val();
        potentialObj.DescriptionPotentialDetail = $tablerange.yield().length == 0 ? new Array() : $tablerange.yield();
        opportunity_master.PotentialDetails = potentialObj;
        //summary-detail
        summaryObj.DescriptionSummaryDetails = $tabledescription.yield().length == 0 ? new Array() : $tabledescription.yield();
        opportunity_master.SummaryDetails = summaryObj;
        opportunity_master.StageDetail = $tablestage.yield().length == 0 ? new Array() : $tablestage.yield();
        opportunity_master.PartnerDetail = $tablepartner.yield().length == 0 ? new Array() : $tablepartner.yield();
        //Competitor
        opportunity_master.CompetitorDetail = $tablecompetitor.yield().length == 0 ? new Array() : $tablecompetitor.yield();
        $.ajax({
            url: "/Opportunity/SubmmitData",
            type: "POST",
            dataType: "JSON",
            data: $.antiForgeryToken({ _data: JSON.stringify(opportunity_master) }),
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

    //update data
    $("#update-data").click(function () {
        const opportunity_master = master[0];

        //master data
        opportunity_master.ID = $("#masterid").val();
        summaryObj.DoctypeID = $("#doctypid").val();
        //opportunity_master.BPCode = $("#code").val();
        //opportunity_master.BPName = $("#name").val();
        opportunity_master.BPID = $("#bpid").val();
        opportunity_master.SaleEmpID = $("#empid").val();
        opportunity_master.OpportunityNo = $("#next_number").val();
        opportunity_master.Owner = $("#ownerid").val();
        opportunity_master.OpportunityTpe = $("#sale").prop("checked") ? true : false;
        opportunity_master.ClosingDate = $("#closingdate").val();
        opportunity_master.OpportunityName = $("#opportunityname").val();
        opportunity_master.OpportunityNo = $("#next_number").val();
        opportunity_master.Status = $("#status").val();
        opportunity_master.StartDate = $("#startdate").val();
        opportunity_master.CloingPercentage = $("#closingid").val();

        potentialObj.ID = $("#potentialid").val();
        potentialObj.PotentailAmount = $("#potentailamount").val();
        potentialObj.GrossProfit = $("#profitpercent").val();
        potentialObj.GrossProfitTotal = $("#profittotal").val();
        potentialObj.Level = $("#levelID").val();
        potentialObj.PredictedClosingInNum = $("#predictedclosingnum").val();
        potentialObj.PredictedClosingDate = $("#predictedclosingdate").val();
        potentialObj.WeightAmount = $("#weightamount").val();
        potentialObj.PredictedClosingInTime = $("#predictedclosingtime").val();
        potentialObj.DescriptionPotentialDetail = $tablerange.yield().length == 0 ? new Array() : $tablerange.yield();
        opportunity_master.PotentialDetails = potentialObj;
        //summary detail
        summaryObj.ID = $("#summarydetailid").val();
        summaryObj.SeriesDID = $("#docNoid").val();
        summaryObj.SeriesID = $("#seriID").val();
        summaryObj.IsWon = $("#won").prop("checked") ? true : false;
        summaryObj.IsLost = $("#lost").prop("checked") ? true : false;
        summaryObj.IsOpen = $("#open").prop("checked") ? true : false;

        summaryObj.DescriptionSummaryDetails = $tabledescription.yield().length == 0 ? new Array() : $tabledescription.yield();
        opportunity_master.SummaryDetails = summaryObj;

        opportunity_master.StageDetail = $tablestage.yield().length == 0 ? new Array() : $tablestage.yield();
        opportunity_master.PartnerDetail = $tablepartner.yield().length == 0 ? new Array() : $tablepartner.yield();
        opportunity_master.CompetitorDetail = $tablecompetitor.yield().length == 0 ? new Array() : $tablecompetitor.yield();
        $.ajax({
            url: "/Opportunity/UpdateData",
            type: "POST",
            dataType: "JSON",
            data: $.antiForgeryToken({ _data: JSON.stringify(opportunity_master) }),
            success: function (respones) {
                if (respones.IsApproved) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        }
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
    $("#open").click(function () {
        if ($(this).prop("checked")) {
            $("#doctypeId").attr('disabled', 'disabled');
            $("#buttonlist").hide();
            $("#lost").prop("checked", false);
            $("#won").prop("checked", false);
            summaryObj.IsLost = false;
            summaryObj.IsWon = false;
            summaryObj.IsOpen = true;
        } else {
            summaryObj.IsOpen = false;
        }
    })
    $("#lost").click(function () {
        if ($(this).prop("checked")) {

            $("#doctypeId").attr('disabled', 'disabled');
            $("#buttonlist").hide();

            $("#open").prop("checked", false);
            $("#won").prop("checked", false);
            summaryObj.IsLost = true;
            summaryObj.IsWon = false;
            summaryObj.IsOpen = false;
        } else {
            summaryObj.IsLost = false;
        }
    })

    $("#doctypeId").attr('disabled', 'disabled');
    $("#buttonlist").hide();
})