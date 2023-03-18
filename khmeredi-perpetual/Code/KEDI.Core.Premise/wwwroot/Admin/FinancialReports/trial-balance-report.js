$(document).ready(function () {
    let __AllData = [];
    let __CUSDATA = [];
    let __VENDATA = [];
    var formatNumber = 2;
    $("#checkBP").click(function () {
        if ($("#checkBP").prop("checked") === true) {
            $(".cv-hide-show").prop("hidden", false);
        } else {
            $(".cv-hide-show").prop("hidden", true);
        }
    })
    //// **** Check Select Display Type Report **** ////
    $("#d-report-select").change(function () {
        if (this.value == 1 || this.value == 2) {
            $(".to-sh-posting-period").css("pointer-events", "all");
            $(".from-sh-posting-period").css("pointer-events", "none");
            $("#from-date").prop("placeholder", "No Date Needed");

        }
        if (this.value == 0) {
            $(".to-sh-posting-period").css("pointer-events", "none");
            $(".from-sh-posting-period").css("pointer-events", "all");
            $("#from-date").prop("placeholder", "");
        }
    });
    search("#find-bp", ".bp tr:not(:first-child)");
    // ********* GET: Business Parnters ********** //
    $(".from-bp-choose").click(function () {
        getBP("from");
    })
    $(".to-bp-choose").click(function () {
        getBP("to");
    })
    // ********* GET: Posting Period ********** //
    $(".from-sh-posting-period").click(function () {
        postingPeriod("from");
    });
    $(".to-sh-posting-period").click(function () {
        postingPeriod("to");
    });

    // Click Ok
    $("#btn-ok").click(function () {
        const dateFrom = $("#from-date").val();
        const dateTo = $("#to-date").val();
        const fromId = $("#from-bp-id").val();
        const toId = $("#to-bp-id").val();
        const cus_gp = $("#cus-gp").val();
        const ven_gp = $("#ven-gp").val();
        const typeDisplay = parseInt($("#d-report-select").val());
        const showZeroAcc = $("#checkAccZero").is(":checked") ? "true" : "false";
        const showGla = $("#checkAccGl").is(":checked") ? "true" : "false";
        $("#show-hide-loader").prop("hidden", false);
        $("#content").prop("hidden", true);
        GetBusPartners(ven_gp, cus_gp, fromId, toId, dateFrom, dateTo, typeDisplay, showZeroAcc, showGla, function (res) {
            formatNumber = res.FormtNumber;
            if (res.AnnualReport) {
                $("#annual_report").prop("hidden", false);
                $("#quarterly_report").prop("hidden", true);
                $("#monthly_report").prop("hidden", true);
                //$("#PeriodicReport").prop("hidden", true);
            }
            if (res.QuarterlyReport) {
                $("#annual_report").prop("hidden", true);
                $("#quarterly_report").prop("hidden", false);
                $("#monthly_report").prop("hidden", true);
                //$("#PeriodicReport").prop("hidden", true);
            }
            if (res.MonthlyReport) {
                $("#annual_report").prop("hidden", true);
                $("#quarterly_report").prop("hidden", true);
                $("#monthly_report").prop("hidden", false);
                //$("#PeriodicReport").prop("hidden", true);
            }
            //if (res.PeriodicReport) {
            //    $("#AnnualReport").prop("hidden", true);
            //    $("#QuarterlyReport").prop("hidden", true);
            //    $("#MonthlyReport").prop("hidden", true);
            //    $("#PeriodicReport").prop("hidden", false);
            //}
            if (isValidArray(__AllData)) {
                __AllData = [];
                __AllData.push(res);
            } else __AllData.push(res);

            res.TrialBalanceViewModels.forEach(i => {
                if (i.Type == "Customer")
                    __CUSDATA.push(i);
                else
                    __VENDATA.push(i);
            })
            bindCusVen(".cus-gp", "Customer", "General Customer", __CUSDATA);
            bindCusVen(".ven-gp", "Vendor", "General Vendor", __VENDATA);
            setTimeout(function () {
                GetGategories("100000000000000", function (res) {
                    bindGategoties("#data", res, "Assets");
                });
            }, 0);

            setTimeout(function () {
                GetGategories("200000000000000", function (res) {
                    bindGategoties("#data1", res, "Liabilities");
                });
            }, 50);

            setTimeout(function () {
                GetGategories("300000000000000", function (res) {
                    bindGategoties("#data2", res, "CapitalandReserves");
                });
            }, 100);

            setTimeout(function () {
                GetGategories("400000000000000", function (res) {
                    bindGategoties("#turnover", res, "Turnover");
                });
            }, 150);

            setTimeout(function () {
                GetGategories("500000000000000", function (res) {
                    bindGategoties("#cost-of-sales", res, "CostOfSales");
                });
            }, 200);
            setTimeout(function () {
                GetGategories("600000000000000", function (res) {
                    bindGategoties("#operating-costs", res, "OperatingCosts");
                });
            }, 250);
            setTimeout(function () {
                GetGategories("700000000000000", function (res) {
                    bindGategoties("#non-operating-income-expenditure", res, "NonOperatingIncomeExpenditure");
                });
            }, 250);
            setTimeout(function () {
                GetGategories("800000000000000", function (res) {
                    bindGategoties("#taxation-extraordinary-items", res, "TaxationExtraordinaryItems");
                });
            }, 250);
            __CUSDATA = [];
            __VENDATA = [];
            $("#show-hide-loader").prop("hidden", true);
            $("#content").prop("hidden", false);
        });
    })
    $("#print").click(function () {
        clickedPrint();
    })
    function getBP(type) {
        let dialog = new DialogBox({
            content: {
                selector: "#bp-content"
            }
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
        dialog.invoke(function () {
            let $bp = ViewTable({
                keyField: "ID",
                selector: ".bp",
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
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                if (type === "from") {
                                    $("#from-bp").val(e.data.Code);
                                    $("#from-bp-id").val(e.data.ID);
                                }
                                if (type === "to") {
                                    $("#to-bp").val(e.data.Code);
                                    $("#to-bp-id").val(e.data.ID);
                                }
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            onGetbpTemplates(function (postingPeriod) {
                $bp.clearRows();
                $bp.bindRows(postingPeriod);
            });
        })
    }

    function postingPeriod(type) {
        let dialog = new DialogBox({
            content: {
                selector: "#postingPeriod-content"
            }
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
        dialog.invoke(function () {
            let $postingperiod = ViewTable({
                keyField: "ID",
                selector: ".posting-period",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 20
                },
                visibleFields: [
                    "PeriodCode",
                    "PeriodName",
                ],
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                if (type.toLowerCase() == "to") {
                                    $("#to-date").val(formatDate(e.data.PostingDateTo, "YYYY-MM-DD"));
                                }
                                if (type.toLowerCase() == "from") {
                                    $("#from-date").val(formatDate(e.data.PostingDateFrom, "YYYY-MM-DD"));
                                    $(".to-sh-posting-period").css("pointer-events", "all");
                                }
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            onGetPostingPeriodTemplates(function (postingPeriod) {
                $postingperiod.clearRows();
                $postingperiod.bindRows(postingPeriod);
            });
        })
    }
    function onGetbpTemplates(success) {
        // filter BP
        $("#select-bp").change(function () {
            const type = this.value;
            $.get("/FinancialReports/GetBusPartnersAll", { type }, success);
        })

        $.get("/FinancialReports/GetBusPartnersAll", { type: "All" }, success);

    }
    function onGetPostingPeriodTemplates(success) {
        $.get("/FinancialReports/GetPostingPeriods", success);
    }
    function GetBusPartners(VType, CType, fromId, toId, fromDate, toDate, displayType, showZeroAcc, showGla, success) {
        $.get("/FinancialReports/GetBusPartners", { VType, CType, fromId, toId, fromDate, toDate, displayType, showZeroAcc, showGla }, success);
    }

    //search
    function search(searchInput, filter) {
        $(searchInput).on("keyup", function () {
            var value = $(this).val().toLowerCase();
            $(filter).filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
            });
        });
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
    //format currency
    function curFormat(value) {
        return value.toFixed(formatNumber).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
    //Util functions
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }
    function bindCusVen(container, title, group, data) {
        let curname = "";
        if (__AllData[0].Count) {
            if (__AllData[0].AnnualReport) {
                let sumDebit = 0;
                let sumCredit = 0;
                let sumBalance = 0;
                $(`${container}-AnnualReport ul`).remove();
                let _ul = $(`<ul></ul>`);
                let _li = $(`<ul><li class='group font-size'>${group}</li><ul>`);
                let _childUl = $(`<ul></ul>`);
                if (isValidArray(data)) {
                    data.forEach(i => {
                        curname = i.CurrencyName;
                        let _div = $(`<div class='row cv-b-row-width'></div>`);
                        let _liChild = $(`<li></li>`)
                        _div.append(`
                            <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${i.Code}-${i.Name}</span></div>
                            <div class='col-md-3'><span class='font-size'>${i.CurrencyName} ${curFormat(i.Debit)}</span></div>
                            <div class='col-md-3 cv-ml1'><span class='font-size'>${i.CurrencyName} ${curFormat(i.Credit)}</span></div>
                            <div class='col-md-3 cv-ml'><span class='font-size'>${i.CurrencyName} ${curFormat(i.Debit - i.Credit)}</span></div>
                        `);
                        sumCredit += i.Credit;
                        sumBalance += (i.Debit - i.Credit);
                        sumDebit += i.Debit;
                        _liChild.append(_div);
                        _childUl.append(_liChild);
                    });
                }
                let divTotal = $(`<div class='row gcv-b-row-width'> </div>`);
                let divTotalTitle = $(`<div class='row tcv-b-row-width'> </div>`);
                let liTotal = $(`<li> </li>`);
                let liTotalTitle = $(`<li> </li>`);
                divTotal.append(`
                <div class='col-md-4'><span class='group font-size'>Total ${group}</span></div>
                <div class='col-md-3 gcv-ml2'><span class='sumDebit-${title} groupTotal font-size' data-sum='${sumDebit}'>${curname} ${curFormat(sumDebit)}</span></div>
                <div class='col-md-3 gcv-ml3'><span class='sumCredit-${title} groupTotal font-size' data-sum='${sumCredit}'>${curname} ${curFormat(sumCredit)}</span></div>
                <div class='col-md-3 gcv-ml4'><span class='sumBalance-${title} groupTotal font-size' data-sum='${sumBalance}'>${curname} ${curFormat(sumBalance)}</span></div>
            `);
                liTotal.append(divTotal)
                _li.append(_childUl).append(liTotal);
                _ul.append(`<li class='title font-size'>${title}</li>`).append(_li);
                $(`${container}-AnnualReport`).append(_ul);
                // Total Title
                let sumTitleDebit = parseFloat($(`.sumDebit-${title}`).data("sum"));
                let sumTitleCredit = parseFloat($(`.sumCredit-${title}`).data("sum"));
                let sumTitleBalance = parseFloat($(`.sumBalance-${title}`).data("sum"));
                /*const sumTitleDebitEl = $()*/
                divTotalTitle.append(`
                <div class='col-md-4'><span class='title font-size'>Total ${title}</span></div>
                <div class='col-md-3 tcv-ml1'><span class='titleTotal font-size'>${curname} ${curFormat(sumTitleDebit)}</span></div>
                <div class='col-md-3 tcv-ml2'><span class='titleTotal font-size'>${curname} ${curFormat(sumTitleCredit)}</span></div>
                <div class='col-md-3 tcv-ml3'><span class='titleTotal font-size'>${curname} ${curFormat(sumTitleBalance)}</span></div>
            `);
                liTotalTitle.append(divTotalTitle)
                _ul.append(liTotalTitle)
            }
            if (__AllData[0].QuarterlyReport) {
                let _sumDQ = 0, _sumDQ1 = 0, _sumDQ2 = 0, _sumDQ3 = 0, _sumDQ4 = 0;
                let _sumCQ = 0, _sumCQ1 = 0, _sumCQ2 = 0, _sumCQ3 = 0, _sumCQ4 = 0;
                let _sumBQ = 0, _sumBQ1 = 0, _sumBQ2 = 0, _sumBQ3 = 0, _sumBQ4 = 0;
                $(`${container}-QuarterlyReport ul`).remove();
                let _ul = $(`<ul class='ml__5'></ul>`);
                let _li = $(`<ul class='ml__5'><li class='group font-size'>${group}</li><ul>`);
                let _childUl = $(`<ul></ul>`);
                if (isValidArray(data)) {
                    data.forEach(i => {
                        curname = i.CurrencyName;
                        let __row = $(`<div class='row'></div>`);
                        let __colmd3 = $(`<div class="col-md-3"></div>`);
                        let __colmd4Q = $(`<div class="col-md-4 cl-md-width"></div>`);
                        let __colmd4Q1 = $(`<div class="col-md-4 cl-md-width1"></div>`);
                        let __colmd4Q2 = $(`<div class="col-md-4 cl-md-width2"></div>`);
                        let __colmd4Q3 = $(`<div class="col-md-4 cl-md-width3"></div>`);
                        let __colmd4Q4 = $(`<div class="col-md-4 cl-md-width4"></div>`);
                        let __rowInCol4Q = $(`<div class="row"></div>`);
                        let __rowInCol4Q1 = $(`<div class="row"></div>`);
                        let __rowInCol4Q2 = $(`<div class="row"></div>`);
                        let __rowInCol4Q3 = $(`<div class="row"></div>`);
                        let __rowInCol4Q4 = $(`<div class="row"></div>`);
                        //let _div = $(`<div class='divFlex'></div>`);
                        let _liChild = $(`<li></li>`);
                        __rowInCol4Q.append(`
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.Debit)}</span></div>
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.Credit)}</span></div>
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.Debit - i.Credit)}</span></div>
                    `)
                        __rowInCol4Q1.append(`
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DQ1)}</span></div>
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CQ1)}</span></div>
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BQ1)}</span></div>
                    `)

                        __rowInCol4Q2.append(`
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DQ2)}</span></div>
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CQ2)}</span></div>
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BQ2)}</span></div>
                    `)

                        __rowInCol4Q3.append(`
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DQ3)}</span></div>
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CQ3)}</span></div>
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BQ3)}</span></div>
                    `)

                        __rowInCol4Q4.append(`
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DQ4)}</span></div>
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CQ4)}</span></div>
                        <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BQ4)}</span></div>
                    `)

                        __colmd3.append(`<span class='font-size'>${i.CurrencyName} ${i.Code}-${i.Name}</span> `);
                        __colmd4Q.append(__rowInCol4Q);

                        __colmd4Q1.append(__rowInCol4Q1);

                        __colmd4Q2.append(__rowInCol4Q2);

                        __colmd4Q3.append(__rowInCol4Q3);

                        __colmd4Q4.append(__rowInCol4Q4);

                        __row.append(__colmd3).append(__colmd4Q).append(__colmd4Q1).append(__colmd4Q2).append(__colmd4Q3).append(__colmd4Q4);
                        _liChild.append(__row);
                        _childUl.append(_liChild);

                        /// Sum :::Debit, Credit, Balance::: ///
                        _sumDQ += i.Debit;
                        _sumDQ1 += i.DQ1;
                        _sumDQ2 += i.DQ2;
                        _sumDQ3 += i.DQ3;
                        _sumDQ4 += i.DQ4;

                        _sumCQ += i.Credit;
                        _sumCQ1 += i.CQ1;
                        _sumCQ2 += i.CQ2;
                        _sumCQ3 += i.CQ3;
                        _sumCQ4 += i.CQ4;

                        _sumBQ += (i.Debit - i.Credit);
                        _sumBQ1 += (i.DQ1 - i.CQ1);
                        _sumBQ2 += (i.DQ2 - i.CQ2);
                        _sumBQ3 += (i.DQ3 - i.CQ3);
                        _sumBQ4 += (i.DQ4 - i.CQ4);
                    });
                }
                //Sum Group elements
                let liTotal = $(`<li> </li>`);
                let __rowTotal = $(`<div class='row' style='width: 1310px;'> </div>`);
                let __colmd3 = $(`<div class="col-md-3"></div>`);
                let __colmd4Q = $(`<div class="col-md-4 ${__AllData[0].ShowGLAcc ? "cvg-ll" : "cv-ll"}"></div>`);
                let __colmd4Q1 = $(`<div class="col-md-4 ${__AllData[0].ShowGLAcc ? "cvg-ll1" : "cv-ll1"}"></div>`);
                let __colmd4Q2 = $(`<div class="col-md-4 ${__AllData[0].ShowGLAcc ? "cvg-ll2" : "cv-ll2"}"></div>`);
                let __colmd4Q3 = $(`<div class="col-md-4 ${__AllData[0].ShowGLAcc ? "cvg-ll3" : "cv-ll3"}"></div>`);
                let __colmd4Q4 = $(`<div class="col-md-4 ${__AllData[0].ShowGLAcc ? "cvg-ll4" : "cv-ll4"}"></div>`);
                let __rowInCol4Q = $(`<div class="row"></div>`);
                let __rowInCol4Q1 = $(`<div class="row"></div>`);
                let __rowInCol4Q2 = $(`<div class="row"></div>`);
                let __rowInCol4Q3 = $(`<div class="row"></div>`);
                let __rowInCol4Q4 = $(`<div class="row"></div>`);

                __rowInCol4Q.append(`
                <div class='col-md-4'><span class='font-size groupTotal sumD-${title}' data-sum='${_sumDQ}'>${curname} ${curFormat(_sumDQ)}</span></div>
                <div class='col-md-4'><span class='font-size groupTotal sumC-${title}' data-sum='${_sumCQ}'>${curname} ${curFormat(_sumCQ)}</span></div>
                <div class='col-md-4'><span class='font-size groupTotal sumB-${title}' data-sum='${_sumBQ}'>${curname} ${curFormat(_sumBQ)}</span></div>
            `)

                __rowInCol4Q1.append(`
                <div class='col-md-4'><span class='font-size groupTotal sumDQ1-${title}' data-sum='${_sumDQ1}'>${curname} ${curFormat(_sumDQ1)}</span></div>
                <div class='col-md-4'><span class='font-size groupTotal sumCQ1-${title}' data-sum='${_sumCQ1}'>${curname} ${curFormat(_sumCQ1)}</span></div>
                <div class='col-md-4'><span class='font-size groupTotal sumBQ1-${title}' data-sum='${_sumBQ1}'>${curname} ${curFormat(_sumBQ1)}</span></div>
            `)

                __rowInCol4Q2.append(`
                <div class='col-md-4'><span class='font-size groupTotal sumDQ2-${title}' data-sum='${_sumDQ2}'>${curname} ${curFormat(_sumDQ2)}</span></div>
                <div class='col-md-4'><span class='font-size groupTotal sumCQ2-${title}' data-sum='${_sumCQ2}'>${curname} ${curFormat(_sumCQ2)}</span></div>
                <div class='col-md-4'><span class='font-size groupTotal sumBQ2-${title}' data-sum='${_sumBQ2}'>${curname} ${curFormat(_sumBQ2)}</span></div>
            `)

                __rowInCol4Q3.append(`
                <div class='col-md-4'><span class='font-size groupTotal sumDQ3-${title}' data-sum='${_sumDQ3}'>${curname} ${curFormat(_sumDQ3)}</span></div>
                <div class='col-md-4'><span class='font-size groupTotal sumCQ3-${title}' data-sum='${_sumCQ3}'>${curname} ${curFormat(_sumCQ3)}</span></div>
                <div class='col-md-4'><span class='font-size groupTotal sumBQ3-${title}' data-sum='${_sumBQ3}'>${curname} ${curFormat(_sumBQ3)}</span></div>
            `)

                __rowInCol4Q4.append(`
                <div class='col-md-4'><span class='font-size groupTotal sumDQ4-${title}' data-sum='${_sumDQ4}'>${curname} ${curFormat(_sumDQ4)}</span></div>
                <div class='col-md-4'><span class='font-size groupTotal sumCQ4-${title}' data-sum='${_sumCQ4}'>${curname} ${curFormat(_sumCQ4)}</span></div>
                <div class='col-md-4'><span class='font-size groupTotal sumBQ4-${title}' data-sum='${_sumBQ4}'>${curname} ${curFormat(_sumBQ4)}</span></div>
            `)

                __colmd3.append(`<span class='font-size group'>Total ${group}</span> `);
                __colmd4Q.append(__rowInCol4Q);

                __colmd4Q1.append(__rowInCol4Q1);
                __colmd4Q2.append(__rowInCol4Q2);
                __colmd4Q3.append(__rowInCol4Q3);
                __colmd4Q4.append(__rowInCol4Q4);
                __rowTotal.append(__colmd3).append(__colmd4Q)
                    .append(__colmd4Q1).append(__colmd4Q2)
                    .append(__colmd4Q3).append(__colmd4Q4);

                liTotal.append(__rowTotal);
                _li.append(_childUl).append(liTotal);
                _ul.append(_li);
                $(`${container}-QuarterlyReport`).append(_ul);

                //Sum Title Elements//
                let __rowTotalTitle = $(`<div class='row' style='width: 1310px;'> </div>`);
                let __colmd3T = $(`<div class="col-md-3"></div>`);
                let __colmd4QT = $(`<div class="col-md-4 ${__AllData[0].ShowGLAcc ? "cltg-ml" : "clt-ml"}"></div>`);
                let __colmd4Q1T = $(`<div class="col-md-4 ${__AllData[0].ShowGLAcc ? "cltg-ml1" : "clt-ml1"}"></div>`);
                let __colmd4Q2T = $(`<div class="col-md-4 ${__AllData[0].ShowGLAcc ? "cltg-ml2" : "clt-ml2"}"></div>`);
                let __colmd4Q3T = $(`<div class="col-md-4 ${__AllData[0].ShowGLAcc ? "cltg-ml3" : "clt-ml3"}"></div>`);
                let __colmd4Q4T = $(`<div class="col-md-4 ${__AllData[0].ShowGLAcc ? "cltg-ml4" : "clt-ml4"}"></div>`);
                let __rowInCol4QT = $(`<div class="row"></div>`);
                let __rowInCol4Q1T = $(`<div class="row"></div>`);
                let __rowInCol4Q2T = $(`<div class="row"></div>`);
                let __rowInCol4Q3T = $(`<div class="row"></div>`);
                let __rowInCol4Q4T = $(`<div class="row"></div>`);
                let liTotalTitle = $(`<li> </li>`);
                // Total Title
                let sumD = parseFloat($(`.sumD-${title}`).data("sum"));
                let sumC = parseFloat($(`.sumC-${title}`).data("sum"));
                let sumB = parseFloat($(`.sumB-${title}`).data("sum"));
                let sumDQ1 = parseFloat($(`.sumDQ1-${title}`).data("sum"));
                let sumCQ1 = parseFloat($(`.sumCQ1-${title}`).data("sum"));
                let sumBQ1 = parseFloat($(`.sumBQ1-${title}`).data("sum"));
                let sumDQ2 = parseFloat($(`.sumDQ2-${title}`).data("sum"));
                let sumCQ2 = parseFloat($(`.sumCQ2-${title}`).data("sum"));
                let sumBQ2 = parseFloat($(`.sumBQ2-${title}`).data("sum"));
                let sumDQ3 = parseFloat($(`.sumDQ3-${title}`).data("sum"));
                let sumCQ3 = parseFloat($(`.sumCQ3-${title}`).data("sum"));
                let sumBQ3 = parseFloat($(`.sumBQ3-${title}`).data("sum"));
                let sumDQ4 = parseFloat($(`.sumDQ4-${title}`).data("sum"));
                let sumCQ4 = parseFloat($(`.sumCQ4-${title}`).data("sum"));
                let sumBQ4 = parseFloat($(`.sumBQ4-${title}`).data("sum"));

                __rowInCol4QT.append(`
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumD)}</span></div>
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumC)}</span></div>
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumB)}</span></div>
            `)

                __rowInCol4Q1T.append(`
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumDQ1)}</span></div>
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumCQ1)}</span></div>
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumBQ1)}</span></div>
            `)
                __rowInCol4Q2T.append(`
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumDQ2)}</span></div>
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumCQ2)}</span></div>
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumBQ2)}</span></div>
            `)

                __rowInCol4Q3T.append(`
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumDQ3)}</span></div>
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumCQ3)}</span></div>
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumBQ3)}</span></div>
            `)

                __rowInCol4Q4T.append(`
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumDQ4)}</span></div>
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumCQ4)}</span></div>
                <div class='col-md-4'><span class='font-size titleTotal'>${curname} ${curFormat(sumBQ4)}</span></div>
            `)

                __colmd3T.append(`<li class='qtitle font-size'>Total ${title}</li>`);
                __colmd4QT.append(__rowInCol4QT);
                __colmd4Q1T.append(__rowInCol4Q1T);
                __colmd4Q2T.append(__rowInCol4Q2T);
                __colmd4Q3T.append(__rowInCol4Q3T);
                __colmd4Q4T.append(__rowInCol4Q4T);
                __rowTotalTitle.append(__colmd3T).append(__colmd4QT)
                    .append(__colmd4Q1T).append(__colmd4Q2T)
                    .append(__colmd4Q3T).append(__colmd4Q4T);
                liTotalTitle.append(__rowTotalTitle)
                _ul.append(liTotalTitle)
            }
            if (__AllData[0].MonthlyReport) {

                let _sumDM = 0, _sumDM1 = 0, _sumDM2 = 0, _sumDM3 = 0, _sumDM4 = 0;
                let _sumCM = 0, _sumCM1 = 0, _sumCM2 = 0, _sumCM3 = 0, _sumCM4 = 0;
                let _sumBM = 0, _sumBM1 = 0, _sumBM2 = 0, _sumBM3 = 0, _sumBM4 = 0;

                let _sumDM5 = 0, _sumDM6 = 0, _sumDM7 = 0, _sumDM8 = 0, _sumDM9 = 0;
                let _sumCM5 = 0, _sumCM6 = 0, _sumCM7 = 0, _sumCM8 = 0, _sumCM9 = 0;
                let _sumBM5 = 0, _sumBM6 = 0, _sumBM7 = 0, _sumBM8 = 0, _sumBM9 = 0;

                let _sumDM10 = 0, _sumDM11 = 0, _sumDM12 = 0;
                let _sumCM10 = 0, _sumCM11 = 0, _sumCM12 = 0;
                let _sumBM10 = 0, _sumBM11 = 0, _sumBM12 = 0;

                $(`${container}-MonthlyReport ul`).remove();
                let _ul = $(`<ul class='ml__5'></ul>`);
                let _li = $(`<ul class='ml__5'><li class='group font-size'>${group}</li><ul>`);
                let _childUl = $(`<ul></ul>`);
                if (isValidArray(data)) {
                    data.forEach(i => {
                        curname = i.CurrencyName;
                        let __row = $(`<div class='row' style='width: 1400px;'></div>`);
                        let __colmd3 = $(`<div class="col-md-3"></div>`);
                        let __colmd4M = $(`<div class="col-md-4 cvmcl-ml"></div>`);
                        let __colmd4M1 = $(`<div class="col-md-4 cvmcl-ml1"></div>`);
                        let __colmd4M2 = $(`<div class="col-md-4 cvmcl-ml2"></div>`);
                        let __colmd4M3 = $(`<div class="col-md-4 cvmcl-ml3"></div>`);
                        let __colmd4M4 = $(`<div class="col-md-4 cvmcl-ml4"></div>`);
                        let __colmd4M5 = $(`<div class="col-md-4 cvmcl-ml5"></div>`);
                        let __colmd4M6 = $(`<div class="col-md-4 cvmcl-ml6"></div>`);
                        let __colmd4M7 = $(`<div class="col-md-4 cvmcl-ml7"></div>`);
                        let __colmd4M8 = $(`<div class="col-md-4 cvmcl-ml8"></div>`);
                        let __colmd4M9 = $(`<div class="col-md-4 cvmcl-ml9"></div>`);
                        let __colmd4M10 = $(`<div class="col-md-4 cvmcl-ml10"></div>`);
                        let __colmd4M11 = $(`<div class="col-md-4 cvmcl-ml11"></div>`);
                        let __colmd4M12 = $(`<div class="col-md-4 cvmcl-ml12"></div>`);

                        let __rowInCol4M = $(`<div class="row"></div>`);
                        let __rowInCol4M1 = $(`<div class="row"></div>`);
                        let __rowInCol4M2 = $(`<div class="row"></div>`);
                        let __rowInCol4M3 = $(`<div class="row"></div>`);
                        let __rowInCol4M4 = $(`<div class="row"></div>`);
                        let __rowInCol4M5 = $(`<div class="row"></div>`);
                        let __rowInCol4M6 = $(`<div class="row"></div>`);
                        let __rowInCol4M7 = $(`<div class="row"></div>`);
                        let __rowInCol4M8 = $(`<div class="row"></div>`);
                        let __rowInCol4M9 = $(`<div class="row"></div>`);
                        let __rowInCol4M10 = $(`<div class="row"></div>`);
                        let __rowInCol4M11 = $(`<div class="row"></div>`);
                        let __rowInCol4M12 = $(`<div class="row"></div>`);

                        let _liChild = $(`<li></li>`);
                        __rowInCol4M.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.Debit)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.Credit)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.Debit - i.Credit)}</span></div>
                    `)

                        __rowInCol4M1.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM1)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM1)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM1)}</span></div>
                    `)

                        __rowInCol4M2.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM2)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM2)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM2)}</span></div>
                    `)

                        __rowInCol4M3.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM3)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM3)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM3)}</span></div>
                    `)

                        __rowInCol4M4.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM4)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM4)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM4)}</span></div>
                    `)

                        __rowInCol4M5.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM5)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM5)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM5)}</span></div>
                    `)

                        __rowInCol4M6.append(`
                        <div class='col-md-5'><span class='font-size al-center'>${i.CurrencyName} ${curFormat(i.DM6)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM6)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM6)}</span></div>
                    `)

                        __rowInCol4M7.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM7)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM7)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM7)}</span></div>
                    `)

                        __rowInCol4M8.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM8)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM8)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM8)}</span></div>
                    `)

                        __rowInCol4M9.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM9)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM9)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM9)}</span></div>
                    `)

                        __rowInCol4M10.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM10)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM10)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM10)}</span></div>
                    `)

                        __rowInCol4M11.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM11)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM11)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM11)}</span></div>
                    `)

                        __rowInCol4M12.append(`
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM12)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM12)}</span></div>
                        <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM12)}</span></div>                    
                    `)

                        __colmd3.append(`<span class='font-size'>${i.CurrencyName} ${i.Code}-${i.Name}</span> `);

                        __colmd4M.append(__rowInCol4M);
                        __colmd4M1.append(__rowInCol4M1);
                        __colmd4M2.append(__rowInCol4M2);
                        __colmd4M3.append(__rowInCol4M3);
                        __colmd4M4.append(__rowInCol4M4);
                        __colmd4M5.append(__rowInCol4M5);
                        __colmd4M6.append(__rowInCol4M6);
                        __colmd4M7.append(__rowInCol4M7);
                        __colmd4M8.append(__rowInCol4M8);
                        __colmd4M9.append(__rowInCol4M9);
                        __colmd4M10.append(__rowInCol4M10);
                        __colmd4M11.append(__rowInCol4M11);
                        __colmd4M12.append(__rowInCol4M12);

                        __row.append(__colmd3).append(__colmd4M).append(__colmd4M1).append(__colmd4M2)
                            .append(__colmd4M3).append(__colmd4M4).append(__colmd4M5).append(__colmd4M6)
                            .append(__colmd4M7).append(__colmd4M8).append(__colmd4M9).append(__colmd4M10)
                            .append(__colmd4M11).append(__colmd4M12);

                        _liChild.append(__row);
                        _childUl.append(_liChild);

                        /// Sum :::Debit, Credit, Balance::: ///
                        _sumDM += i.Debit;
                        _sumDM1 += i.DM1;
                        _sumDM2 += i.DM2;
                        _sumDM3 += i.DM3;
                        _sumDM4 += i.DM4;
                        _sumDM5 += i.DM5;
                        _sumDM6 += i.DM6;
                        _sumDM7 += i.DM7;
                        _sumDM8 += i.DM8;
                        _sumDM9 += i.DM9;
                        _sumDM10 += i.DM10;
                        _sumDM11 += i.DM11;
                        _sumDM12 += i.DM12;

                        _sumCM += i.Credit;
                        _sumCM1 += i.CM1;
                        _sumCM2 += i.CM2;
                        _sumCM3 += i.CM3;
                        _sumCM4 += i.CM4;
                        _sumCM5 += i.CM5;
                        _sumCM6 += i.CM6;
                        _sumCM7 += i.CM7;
                        _sumCM8 += i.CM8;
                        _sumCM9 += i.CM9;
                        _sumCM10 += i.CM10;
                        _sumCM11 += i.CM11;
                        _sumCM12 += i.CM12;

                        _sumBM += (i.Debit - i.Credit);
                        _sumBM1 += (i.DM1 - i.CM1);
                        _sumBM2 += (i.DM2 - i.CM2);
                        _sumBM3 += (i.DM3 - i.CM3);
                        _sumBM4 += (i.DM4 - i.CM4);
                        _sumBM5 += (i.DM5 - i.CM5);
                        _sumBM6 += (i.DM6 - i.CM6);
                        _sumBM7 += (i.DM7 - i.CM7);
                        _sumBM8 += (i.DM8 - i.CM8);
                        _sumBM9 += (i.DM9 - i.CM9);
                        _sumBM10 += (i.DM10 - i.CM10);
                        _sumBM11 += (i.DM11 - i.CM11);
                        _sumBM12 += (i.DM12 - i.CM12);

                    });
                }
                //Sum Group elements
                let liTotal = $(`<li> </li>`);
                let __rowTotal = $(`<div class='row' style='width: 1400px;'> </div>`);
                let __colmd3 = $(`<div class="col-md-3"></div>`);
                let __colmd4M = $(`<div class="col-md-4 Mcl-width"></div>`);
                let __colmd4M1 = $(`<div class="col-md-4 Mcl-width2"></div>`);
                let __colmd4M2 = $(`<div class="col-md-4 Mcl-width3"></div>`);
                let __colmd4M3 = $(`<div class="col-md-4 Mcl-width4"></div>`);
                let __colmd4M4 = $(`<div class="col-md-4 Mcl-width5"></div>`);
                let __colmd4M5 = $(`<div class="col-md-4 Mcl-width6"></div>`);
                let __colmd4M6 = $(`<div class="col-md-4 Mcl-width7"></div>`);
                let __colmd4M7 = $(`<div class="col-md-4 Mcl-width8"></div>`);
                let __colmd4M8 = $(`<div class="col-md-4 Mcl-width9"></div>`);
                let __colmd4M9 = $(`<div class="col-md-4 Mcl-width10"></div>`);
                let __colmd4M10 = $(`<div class="col-md-4 Mcl-width11"></div>`);
                let __colmd4M11 = $(`<div class="col-md-4 Mcl-width12"></div>`);
                let __colmd4M12 = $(`<div class="col-md-4 Mcl-width13"></div>`);
                let __rowInCol4M = $(`<div class="row"></div>`);
                let __rowInCol4M1 = $(`<div class="row"></div>`);
                let __rowInCol4M2 = $(`<div class="row"></div>`);
                let __rowInCol4M3 = $(`<div class="row"></div>`);
                let __rowInCol4M4 = $(`<div class="row"></div>`);
                let __rowInCol4M5 = $(`<div class="row"></div>`);
                let __rowInCol4M6 = $(`<div class="row"></div>`);
                let __rowInCol4M7 = $(`<div class="row"></div>`);
                let __rowInCol4M8 = $(`<div class="row"></div>`);
                let __rowInCol4M9 = $(`<div class="row"></div>`);
                let __rowInCol4M10 = $(`<div class="row"></div>`);
                let __rowInCol4M11 = $(`<div class="row"></div>`);
                let __rowInCol4M12 = $(`<div class="row"></div>`);

                __rowInCol4M.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumD-${title}' data-sum='${_sumDM}'>${curname} ${curFormat(_sumDM)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumC-${title}' data-sum='${_sumCM}'>${curname} ${curFormat(_sumCM)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumB-${title}' data-sum='${_sumBM}'>${curname} ${curFormat(_sumBM)}</span></div>
            `)

                __rowInCol4M1.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM1-${title}' data-sum='${_sumDM1}'>${curname} ${curFormat(_sumDM1)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM1-${title}' data-sum='${_sumCM1}'>${curname} ${curFormat(_sumCM1)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM1-${title}' data-sum='${_sumBM1}'>${curname} ${curFormat(_sumBM1)}
            </span></div>
            `)

                __rowInCol4M2.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM2-${title}' data-sum='${_sumDM2}'>${curname} ${curFormat(_sumDM2)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM2-${title}' data-sum='${_sumCM2}'>${curname} ${curFormat(_sumCM2)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM2-${title}' data-sum='${_sumBM2}'>${curname} ${curFormat(_sumBM2)}</span></div>
            `)

                __rowInCol4M3.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM3-${title}' data-sum='${_sumDM3}'>${curname} ${curFormat(_sumDM3)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM3-${title}' data-sum='${_sumCM3}'>${curname} ${curFormat(_sumCM3)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM3-${title}' data-sum='${_sumBM3}'>${curname} ${curFormat(_sumBM3)}</span></div>
            `)

                __rowInCol4M4.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM4-${title}' data-sum='${_sumDM4}'>${curname} ${curFormat(_sumDM4)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM4-${title}' data-sum='${_sumCM4}'>${curname} ${curFormat(_sumCM4)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM4-${title}' data-sum='${_sumBM4}'>${curname} ${curFormat(_sumBM4)}</span></div>
            `)

                __rowInCol4M5.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM5-${title}' data-sum='${_sumDM5}'>${curname} ${curFormat(_sumDM5)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM5-${title}' data-sum='${_sumCM5}'>${curname} ${curFormat(_sumCM5)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM5-${title}' data-sum='${_sumBM5}'>${curname} ${curFormat(_sumBM5)}</span></div>
            `)

                __rowInCol4M6.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM6-${title}' data-sum='${_sumDM6}'>${curname} ${curFormat(_sumDM6)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM6-${title}' data-sum='${_sumCM6}'>${curname} ${curFormat(_sumCM6)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM6-${title}' data-sum='${_sumBM6}'>${curname} ${curFormat(_sumBM6)}</span></div>
            `)

                __rowInCol4M7.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM7-${title}' data-sum='${_sumDM7}'>${curname} ${curFormat(_sumDM7)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM7-${title}' data-sum='${_sumCM7}'>${curname} ${curFormat(_sumCM7)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM7-${title}' data-sum='${_sumBM7}'>${curname} ${curFormat(_sumBM7)}</span></div>
            `)

                __rowInCol4M8.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM8-${title}' data-sum='${_sumDM8}'>${curname} ${curFormat(_sumDM8)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM8-${title}' data-sum='${_sumCM8}'>${curname} ${curFormat(_sumCM8)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM8-${title}' data-sum='${_sumBM8}'>${curname} ${curFormat(_sumBM8)}</span></div>
            `)

                __rowInCol4M9.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM9-${title}' data-sum='${_sumDM9}'>${curname} ${curFormat(_sumDM9)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM9-${title}' data-sum='${_sumCM9}'>${curname} ${curFormat(_sumCM9)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM9-${title}' data-sum='${_sumBM9}'>${curname} ${curFormat(_sumBM9)}</span></div>
            `)

                __rowInCol4M10.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM10-${title}' data-sum='${_sumDM10}'>${curname} ${curFormat(_sumDM10)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM10-${title}' data-sum='${_sumCM10}'>${curname} ${curFormat(_sumCM10)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM10-${title}' data-sum='${_sumBM10}'>${curname} ${curFormat(_sumBM10)}</span></div>
            `)

                __rowInCol4M11.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM11-${title}' data-sum='${_sumDM11}'>${curname} ${curFormat(_sumDM11)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM11-${title}' data-sum='${_sumCM11}'>${curname} ${curFormat(_sumCM11)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM11-${title}' data-sum='${_sumBM11}'>${curname} ${curFormat(_sumBM11)}</span></div>

            `)

                __rowInCol4M12.append(`
                <div class='col-md-5'><span class='font-size groupTotal sumDM12-${title}' data-sum='${_sumDM12}'>${curname} ${curFormat(_sumDM12)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumCM12-${title}' data-sum='${_sumCM12}'>${curname} ${curFormat(_sumCM12)}</span></div>
                <div class='col-md-5'><span class='font-size groupTotal sumBM12-${title}' data-sum='${_sumBM12}'>${curname} ${curFormat(_sumBM12)}</span></div>
            `)

                __colmd3.append(`<span class='font-size group'>Total ${group}</span> `);
                __colmd4M.append(__rowInCol4M);

                __colmd4M1.append(__rowInCol4M1);
                __colmd4M2.append(__rowInCol4M2);
                __colmd4M3.append(__rowInCol4M3);
                __colmd4M4.append(__rowInCol4M4);
                __colmd4M5.append(__rowInCol4M5);
                __colmd4M6.append(__rowInCol4M6);
                __colmd4M7.append(__rowInCol4M7);
                __colmd4M8.append(__rowInCol4M8);
                __colmd4M9.append(__rowInCol4M9);
                __colmd4M10.append(__rowInCol4M10);
                __colmd4M11.append(__rowInCol4M11);
                __colmd4M12.append(__rowInCol4M12);
                __rowTotal.append(__colmd3).append(__colmd4M)
                    .append(__colmd4M1).append(__colmd4M2)
                    .append(__colmd4M3).append(__colmd4M4)
                    .append(__colmd4M5).append(__colmd4M6)
                    .append(__colmd4M7).append(__colmd4M8)
                    .append(__colmd4M9).append(__colmd4M10)
                    .append(__colmd4M11).append(__colmd4M12);

                liTotal.append(__rowTotal);
                _li.append(_childUl).append(liTotal);
                _ul.append(_li);
                $(`${container}-MonthlyReport`).append(_ul);


                /////// Need TO WORK ON ///////

                //Sum Title Elements//
                let __rowTotalTitle = $(`<div class='row' style='width: 1400px;'> </div>`);
                let __colmd3T = $(`<div class="col-md-3"></div>`);
                let __colmd4MT = $(`<div class="col-md-4 mclt-ml"></div>`);
                let __colmd4M1T = $(`<div class="col-md-4 mclt-ml1"></div>`);
                let __colmd4M2T = $(`<div class="col-md-4 mclt-ml2"></div>`);
                let __colmd4M3T = $(`<div class="col-md-4 mclt-ml3"></div>`);
                let __colmd4M4T = $(`<div class="col-md-4 mclt-ml4"></div>`);
                let __colmd4M5T = $(`<div class="col-md-4 mclt-ml5"></div>`);
                let __colmd4M6T = $(`<div class="col-md-4 mclt-ml6"></div>`);
                let __colmd4M7T = $(`<div class="col-md-4 mclt-ml7"></div>`);
                let __colmd4M8T = $(`<div class="col-md-4 mclt-ml8"></div>`);
                let __colmd4M9T = $(`<div class="col-md-4 mclt-ml9"></div>`);
                let __colmd4M10T = $(`<div class="col-md-4 mclt-ml10"></div>`);
                let __colmd4M11T = $(`<div class="col-md-4 mclt-ml11"></div>`);
                let __colmd4M12T = $(`<div class="col-md-4 mclt-ml12"></div>`);

                let __rowInCol4MT = $(`<div class="row"></div>`);
                let __rowInCol4M1T = $(`<div class="row"></div>`);
                let __rowInCol4M2T = $(`<div class="row"></div>`);
                let __rowInCol4M3T = $(`<div class="row"></div>`);
                let __rowInCol4M4T = $(`<div class="row"></div>`);
                let __rowInCol4M5T = $(`<div class="row"></div>`);
                let __rowInCol4M6T = $(`<div class="row"></div>`);
                let __rowInCol4M7T = $(`<div class="row"></div>`);
                let __rowInCol4M8T = $(`<div class="row"></div>`);
                let __rowInCol4M9T = $(`<div class="row"></div>`);
                let __rowInCol4M10T = $(`<div class="row"></div>`);
                let __rowInCol4M11T = $(`<div class="row"></div>`);
                let __rowInCol4M12T = $(`<div class="row"></div>`);
                let liTotalTitle = $(`<li> </li>`);
                // Total Title
                let sumD = parseFloat($(`.sumD-${title}`).data("sum"));
                let sumC = parseFloat($(`.sumC-${title}`).data("sum"));
                let sumB = parseFloat($(`.sumB-${title}`).data("sum"));
                let sumDM1 = parseFloat($(`.sumDM1-${title}`).data("sum"));
                let sumCM1 = parseFloat($(`.sumCM1-${title}`).data("sum"));
                let sumBM1 = parseFloat($(`.sumBM1-${title}`).data("sum"));
                let sumDM2 = parseFloat($(`.sumDM2-${title}`).data("sum"));
                let sumCM2 = parseFloat($(`.sumCM2-${title}`).data("sum"));
                let sumBM2 = parseFloat($(`.sumBM2-${title}`).data("sum"));
                let sumDM3 = parseFloat($(`.sumDM3-${title}`).data("sum"));
                let sumCM3 = parseFloat($(`.sumCM3-${title}`).data("sum"));
                let sumBM3 = parseFloat($(`.sumBM3-${title}`).data("sum"));
                let sumDM4 = parseFloat($(`.sumDM4-${title}`).data("sum"));
                let sumCM4 = parseFloat($(`.sumCM4-${title}`).data("sum"));
                let sumBM4 = parseFloat($(`.sumBM4-${title}`).data("sum"));
                let sumDM5 = parseFloat($(`.sumDM5-${title}`).data("sum"));
                let sumCM5 = parseFloat($(`.sumCM5-${title}`).data("sum"));
                let sumBM5 = parseFloat($(`.sumBM5-${title}`).data("sum"));
                let sumDM6 = parseFloat($(`.sumDM6-${title}`).data("sum"));
                let sumCM6 = parseFloat($(`.sumCM6-${title}`).data("sum"));
                let sumBM6 = parseFloat($(`.sumBM6-${title}`).data("sum"));
                let sumDM7 = parseFloat($(`.sumDM7-${title}`).data("sum"));
                let sumCM7 = parseFloat($(`.sumCM7-${title}`).data("sum"));
                let sumBM7 = parseFloat($(`.sumBM7-${title}`).data("sum"));
                let sumDM8 = parseFloat($(`.sumDM8-${title}`).data("sum"));
                let sumCM8 = parseFloat($(`.sumCM8-${title}`).data("sum"));
                let sumBM8 = parseFloat($(`.sumBM8-${title}`).data("sum"));
                let sumDM9 = parseFloat($(`.sumDM9-${title}`).data("sum"));
                let sumCM9 = parseFloat($(`.sumCM9-${title}`).data("sum"));
                let sumBM9 = parseFloat($(`.sumBM9-${title}`).data("sum"));
                let sumDM10 = parseFloat($(`.sumDM10-${title}`).data("sum"));
                let sumCM10 = parseFloat($(`.sumCM10-${title}`).data("sum"));
                let sumBM10 = parseFloat($(`.sumBM10-${title}`).data("sum"));
                let sumDM11 = parseFloat($(`.sumDM11-${title}`).data("sum"));
                let sumCM11 = parseFloat($(`.sumCM11-${title}`).data("sum"));
                let sumBM11 = parseFloat($(`.sumBM11-${title}`).data("sum"));
                let sumDM12 = parseFloat($(`.sumDM12-${title}`).data("sum"));
                let sumCM12 = parseFloat($(`.sumCM12-${title}`).data("sum"));
                let sumBM12 = parseFloat($(`.sumBM12-${title}`).data("sum"));

                __rowInCol4MT.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumD)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumC)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumB)}</span></div>
            `)

                __rowInCol4M1T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM1)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM1)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM1)}</span></div>
            `)

                __rowInCol4M2T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM2)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM2)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM2)}</span></div>
            `)

                __rowInCol4M3T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM3)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center' >${curname} ${curFormat(sumCM3)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM3)}</span></div>
            `)

                __rowInCol4M4T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM4)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM4)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM4)}</span></div>
            `)

                __rowInCol4M5T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM5)} </span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM5)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM5)}</span></div>
            `)

                __rowInCol4M6T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM6)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM6)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM6)}</span></div>
            `)

                __rowInCol4M7T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM7)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM7)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM7)}</span></div>
            `)

                __rowInCol4M8T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM8)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM8)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM8)}</span></div>
            `)

                __rowInCol4M9T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM9)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM9)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM9)}</span></div>
            `)

                __rowInCol4M10T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM10)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM10)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM10)}</span></div>
            `)

                __rowInCol4M11T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM11)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM11)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM11)}</span></div>
            `)

                __rowInCol4M12T.append(`
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumDM12)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumCM12)}</span></div>
                <div class='col-md-5'><span class='font-size titleTotal text-center'>${curname} ${curFormat(sumBM12)}</span></div>
            `)
                __colmd3T.append(`<li class='title font-size gmml'>Total ${title}</li>`);
                __colmd4MT.append(__rowInCol4MT);
                __colmd4M1T.append(__rowInCol4M1T);
                __colmd4M2T.append(__rowInCol4M2T);
                __colmd4M3T.append(__rowInCol4M3T);
                __colmd4M4T.append(__rowInCol4M4T);

                __colmd4M5T.append(__rowInCol4M5T);
                __colmd4M6T.append(__rowInCol4M6T);
                __colmd4M7T.append(__rowInCol4M7T);
                __colmd4M8T.append(__rowInCol4M8T);
                __colmd4M9T.append(__rowInCol4M9T);

                __colmd4M10T.append(__rowInCol4M10T);
                __colmd4M11T.append(__rowInCol4M11T);
                __colmd4M12T.append(__rowInCol4M12T);

                __rowTotalTitle.append(__colmd3T).append(__colmd4MT)
                    .append(__colmd4M1T).append(__colmd4M2T)
                    .append(__colmd4M3T).append(__colmd4M4T)
                    .append(__colmd4M5T).append(__colmd4M6T)
                    .append(__colmd4M7T).append(__colmd4M8T)
                    .append(__colmd4M9T).append(__colmd4M10T)
                    .append(__colmd4M11T).append(__colmd4M12T);
                liTotalTitle.append(__rowTotalTitle)
                _ul.append(liTotalTitle)
            }
        }
    }
    function clickedPrint() {
        //string VType, string CType, int fromId, int toId, string fromDate, string toDate
        var fromDate = $("#date-from").val().split("T")[0];
        var toDate = $("#date-to").val().split("T")[0];
        var frombpid = $("#from-bp-id").val();
        var tobpid = $("#to-bp-id").val();
        var vtype = 'all';
        var ctype = 'all';
        window.open("/pdf/printtrialbalance?VType=" + vtype + "&CType=" + ctype + "&fromId=" + frombpid + "&toId=" + tobpid + "&fromDate=" + fromDate + "&toDate=" + toDate, "_blank");
    }
    function bindGla(container, group, prop) {
        if (isValidArray(group)) {
            let __ul = $(`<ul></ul>`);
            if (__AllData[0].AnnualReport) {
                group.forEach(i => {
                    let row = $(`<div class='row b-row-width' ></div>`);
                    let _li = $("<li></li>");
                    let __divTotal = $(`<div class='totalAccount row total-row-width' style='width: 1000px;'></div>`);
                    let $treeview = $(`<div class='col-md-5'><span class='font-size acGlName'>${i.Code} - ${i.Name}</span></div>`);
                    let spanAmount = $(`<span class='font-size'></span>`);

                    if (i.IsActive) {
                        let sumDebit = 0, sumCredit = 0;
                        i.AccountBalances.forEach(s => {
                            if (s.GLAID === i.ID) {
                                sumDebit += s.Debit;
                                sumCredit += s.Credit;
                            }
                        })
                        $treeview.children("span").addClass("text-dark");
                        spanAmount = $(`
                            <div class='col-md-3 col-width1'><span class='font-size text-dark total ' data-sum='${sumDebit}'>${i.CurrencyName} ${curFormat(sumDebit)}</span></div>
                            <div class='col-md-3 col-width2'><span class='font-size text-dark total ' data-sum='${sumCredit}'>${i.CurrencyName} ${curFormat(sumCredit)}</span></div>
                            <div class='col-md-3 col-width3'><span class='font-size text-dark total ' data-sum='${sumDebit - sumCredit}'>${i.CurrencyName} ${curFormat(sumDebit - sumCredit)}</span></div>
                        `);
                    }

                    row.append($treeview).append(spanAmount);
                    _li.append(row).append(__divTotal);
                    __ul.append(_li);

                    bindGla(_li, getItemByGroup(__AllData[0].ProfitAndLossReportViewModel[prop], i.ID), prop);
                    $(container).append(__ul);

                    if (i.IsActive === false && i.IsTitle === true) {
                        $(".acGlName").addClass("text-primary");
                    }
                    if (!i.IsActive) {
                        if (i.Level == 4) {
                            _li.append(__divTotal.append(`
                                <div class='col-md-5'><span class="pm font-size">Total ${i.Code}-${i.Name} : </span></div>
                                <div class='col-md-3 colT-width1'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Debit)}</span></div>
                                <div class='col-md-3 colT-width2'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Credit)}</span></div>
                                <div class='col-md-3 colT-width3'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Balance)}</span></div>
                            `));
                        }
                        if (i.Level == 3) {
                            _li.append(__divTotal.append(`
                                <div class='col-md-5'><span class="pm font-size">Total ${i.Code}-${i.Name} : </span></div>
                                <div class='col-md-3 ml_sl1'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Debit)}</span></div>
                                <div class='col-md-3 ml_sl2'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Credit)}</span></div>
                                <div class='col-md-3 ml_sl3'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Balance)}</span></div>
                            `));
                        }
                        if (i.Level == 2) {
                            _li.append(__divTotal.append(`
                                <div class='col-md-5'><span class="pm font-size">Total ${i.Code}-${i.Name} : </span></div>
                                <div class='col-md-3 ml_ml1'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Debit)}</span></div>
                                <div class='col-md-3 ml_ml2'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Credit)}</span></div>
                                <div class='col-md-3 ml_ml3'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Balance)}</span></div>
                            `));
                        }
                    }
                })
            }
            if (__AllData[0].QuarterlyReport) {
                let __ul = $(`<ul></ul>`);
                group.forEach(i => {
                    let li = $(`<li></li>`);
                    let row = $(`<di class='row row-width-q'></div>`);
                    let __divTotal = $(`<di class='row row-total-width-q'></div>`);
                    let colmd4 = $(`<div class='col-md-5'></div>`);
                    let colmd8q = $(`<div class='col-md-4 row-qml'></div>`);
                    let colmd8q1 = $(`<div class='col-md-4 row-qml1'></div>`);
                    let colmd8q2 = $(`<div class='col-md-4 row-qml2'></div>`);
                    let colmd8q3 = $(`<div class='col-md-4 row-qml3'></div>`);
                    let colmd8q4 = $(`<div class='col-md-4 row-qml4'></div>`);
                    let row_in_colmd8q = $(`<div class='row'></div>`);
                    let row_in_colmd8q1 = $(`<div class='row'></div>`);
                    let row_in_colmd8q2 = $(`<div class='row'></div>`);
                    let row_in_colmd8q3 = $(`<div class='row'></div>`);
                    let row_in_colmd8q4 = $(`<div class='row'></div>`);
                    let name = $(`<span class='font-size'>${i.Code}-${i.Name}</span>`);
                    if (i.IsActive) {
                        let sumCredit = 0, sumDebit = 0;
                        i.AccountBalances.forEach(s => {
                            if (s.GLAID === i.ID) {
                                sumCredit += s.Credit;
                                sumDebit += s.Debit;
                            }                            
                        })
                        name.addClass("text-dark");
                        row_in_colmd8q.append(
                            `
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(sumDebit)}</span></div>
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(sumCredit)}</span></div>
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(sumDebit - sumCredit)}</span></div>                        
                        `
                        )
                        row_in_colmd8q1.append(
                            `
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DQ1)}</span></div>
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CQ1)}</span></div>
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BQ1)}</span></div>                        
                        `
                        )
                        row_in_colmd8q2.append(
                            `
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DQ2)}</span></div>
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CQ2)}</span></div>
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BQ2)}</span></div>                        
                        `
                        )
                        row_in_colmd8q3.append(
                            `
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DQ3)}</span></div>
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CQ3)}</span></div>
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BQ3)}</span></div>                        
                        `
                        )
                        row_in_colmd8q4.append(
                            `
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DQ4)}</span></div>
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CQ4)}</span></div>
                         <div class='col-md-4'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BQ4)}</span></div>                        
                        `
                        )
                    }
                    if (i.IsTitle)
                        name.addClass("text-primary");
                    colmd4.append(name);

                    colmd8q.append(row_in_colmd8q);
                    colmd8q1.append(row_in_colmd8q1);
                    colmd8q2.append(row_in_colmd8q2);
                    colmd8q3.append(row_in_colmd8q3);
                    colmd8q4.append(row_in_colmd8q4);

                    row.append(colmd4).append(colmd8q).append(colmd8q1).append(colmd8q2)
                        .append(colmd8q3).append(colmd8q4);
                    li.append(row).append(__divTotal);
                    __ul.append(li);
                    bindGla(li, getItemByGroup(__AllData[0].ProfitAndLossReportViewModel[prop], i.ID), prop);
                    $(container).append(__ul);
                    if (!i.IsActive) {
                        li.append(__divTotal.append(`
                            <div class='col-md-5'><span class="pm font-size">Total ${i.Code}-${i.Name} : </span></div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width-q' : i.Level === 3 ? 'ml_sl-q' : i.Level === 2 ? 'ml_ml-q' : ''}'>                                    
                                <div class='row'>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Debit)}</span></div>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Credit)}</span></div>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Balance)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width1-q' : i.Level === 3 ? 'ml_sl1-q' : i.Level === 2 ? 'ml_ml1-q' : ''}'>                                    
                                <div class='row'>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DQ1)}</span></div>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CQ1)}</span></div>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BQ1)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width2-q' : i.Level === 3 ? 'ml_sl2-q' : i.Level === 2 ? 'ml_ml2-q' : ''}'>
                                <div class='row'>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DQ2)}</span></div>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CQ2)}</span></div>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BQ2)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width3-q' : i.Level === 3 ? 'ml_sl3-q' : i.Level === 2 ? 'ml_ml3-q' : ''}'>
                                <div class='row'>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DQ3)}</span></div>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CQ3)}</span></div>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BQ3)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width4-q' : i.Level === 3 ? 'ml_sl4-q' : i.Level === 2 ? 'ml_ml4-q' : ''}'>
                                <div class='row'>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DQ4)}</span></div>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CQ4)}</span></div>
                                    <div class='col-md-4'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BQ4)}</span></div>
                                </div>
                            </div>
                        `));
                    }
                })
            }

            if (__AllData[0].MonthlyReport) {
                let __ul = $(`<ul></ul>`);
                group.forEach(i => {
                    let li = $(`<li></li>`);
                    let row = $(`<di class='row row-width-m'></div>`);
                    let __divTotal = $(`<di class='row row-total-width-m'></div>`);
                    let colmd4 = $(`<div class='col-md-5'></div>`);
                    let colmd8m = $(`<div class='col-md-4 row-mml'></div>`);
                    let colmd8m1 = $(`<div class='col-md-4 row-mml1'></div>`);
                    let colmd8m2 = $(`<div class='col-md-4 row-mml2'></div>`);
                    let colmd8m3 = $(`<div class='col-md-4 row-mml3'></div>`);
                    let colmd8m4 = $(`<div class='col-md-4 row-mml4'></div>`);
                    let colmd8m5 = $(`<div class='col-md-4 row-mml5'></div>`);
                    let colmd8m6 = $(`<div class='col-md-4 row-mml6'></div>`);
                    let colmd8m7 = $(`<div class='col-md-4 row-mml7'></div>`);
                    let colmd8m8 = $(`<div class='col-md-4 row-mml8'></div>`);
                    let colmd8m9 = $(`<div class='col-md-4 row-mml9'></div>`);
                    let colmd8m10 = $(`<div class='col-md-4 row-mml10'></div>`);
                    let colmd8m11= $(`<div class='col-md-4 row-mml11'></div>`);
                    let colmd8m12 = $(`<div class='col-md-4 row-mml12'></div>`);
                    let row_in_colmd8m = $(`<div class='row'></div>`);
                    let row_in_colmd8m1 = $(`<div class='row'></div>`);
                    let row_in_colmd8m2 = $(`<div class='row'></div>`);
                    let row_in_colmd8m3 = $(`<div class='row'></div>`);
                    let row_in_colmd8m4 = $(`<div class='row'></div>`);
                    let row_in_colmd8m5 = $(`<div class='row'></div>`);
                    let row_in_colmd8m6 = $(`<div class='row'></div>`);
                    let row_in_colmd8m7 = $(`<div class='row'></div>`);
                    let row_in_colmd8m8 = $(`<div class='row'></div>`);
                    let row_in_colmd8m9 = $(`<div class='row'></div>`);
                    let row_in_colmd8m10 = $(`<div class='row'></div>`);
                    let row_in_colmd8m11 = $(`<div class='row'></div>`);
                    let row_in_colmd8m12 = $(`<div class='row'></div>`);
                    let name = $(`<span class='font-size'>${i.Code}-${i.Name}</span>`);
                    if (i.IsActive) {
                        let sumCredit = 0, sumDebit = 0;
                        i.AccountBalances.forEach(s => {
                            if (s.GLAID === i.ID) {
                                sumCredit += s.Credit;
                                sumDebit += s.Debit;
                            }
                        })
                        name.addClass("text-dark");
                        row_in_colmd8m.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(sumDebit)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(sumCredit)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(sumDebit - sumCredit)}</span></div>                        
                        `
                        )
                        row_in_colmd8m1.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM1)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM1)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM1)}</span></div>                        
                        `
                        )
                        row_in_colmd8m2.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM2)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM2)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM2)}</span></div>                        
                        `
                        )
                        row_in_colmd8m3.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM3)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM3)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM3)}</span></div>                        
                        `
                        )
                        row_in_colmd8m4.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM4)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM4)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM4)}</span></div>                        
                        `
                        )

                        row_in_colmd8m5.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM5)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM5)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM5)}</span></div>                        
                        `
                        )
                        row_in_colmd8m6.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM6)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM6)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM6)}</span></div>                        
                        `
                        )
                        row_in_colmd8m7.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM7)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM7)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM7)}</span></div>                        
                        `
                        )
                        row_in_colmd8m8.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM8)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM8)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM8)}</span></div>                        
                        `
                        )
                        row_in_colmd8m9.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM9)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM9)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM9)}</span></div>                        
                        `
                        )
                        row_in_colmd8m10.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM10)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM10)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM10)}</span></div>                        
                        `
                        )
                        row_in_colmd8m11.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM11)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM11)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM11)}</span></div>                        
                        `
                        )
                        row_in_colmd8m12.append(
                            `
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.DM12)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.CM12)}</span></div>
                         <div class='col-md-5'><span class='font-size'>${i.CurrencyName} ${curFormat(i.BM12)}</span></div>                        
                        `
                        )
                    }
                    if (i.IsTitle)
                        name.addClass("text-primary");
                    colmd4.append(name);

                    colmd8m.append(row_in_colmd8m);
                    colmd8m1.append(row_in_colmd8m1);
                    colmd8m2.append(row_in_colmd8m2);
                    colmd8m3.append(row_in_colmd8m3);
                    colmd8m4.append(row_in_colmd8m4);
                    colmd8m5.append(row_in_colmd8m5);
                    colmd8m6.append(row_in_colmd8m6);
                    colmd8m7.append(row_in_colmd8m7);
                    colmd8m8.append(row_in_colmd8m8);
                    colmd8m9.append(row_in_colmd8m9);
                    colmd8m10.append(row_in_colmd8m10);
                    colmd8m11.append(row_in_colmd8m11);
                    colmd8m12.append(row_in_colmd8m12);

                    row.append(colmd4).append(colmd8m).append(colmd8m1).append(colmd8m2)
                        .append(colmd8m3).append(colmd8m4)
                        .append(colmd8m5).append(colmd8m6).append(colmd8m7)
                        .append(colmd8m8).append(colmd8m9)
                        .append(colmd8m10).append(colmd8m11).append(colmd8m12);
                    li.append(row).append(__divTotal);
                    __ul.append(li);
                    bindGla(li, getItemByGroup(__AllData[0].ProfitAndLossReportViewModel[prop], i.ID), prop);
                    $(container).append(__ul);
                    if (!i.IsActive) {
                        li.append(__divTotal.append(`
                            <div class='col-md-5'><span class="pm font-size">Total ${i.Code}-${i.Name} : </span></div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width-m' : i.Level === 3 ? 'ml_sl-m' : i.Level === 2 ? 'ml_ml-m' : ''}'>                                    
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Debit)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Credit)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.Balance)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width1-m' : i.Level === 3 ? 'ml_sl1-m' : i.Level === 2 ? 'ml_ml1-m' : ''}'>                                    
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM1)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM1)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM1)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width2-m' : i.Level === 3 ? 'ml_sl2-m' : i.Level === 2 ? 'ml_ml2-m' : ''}'>
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM2)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM2)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM2)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width3-m' : i.Level === 3 ? 'ml_sl3-m' : i.Level === 2 ? 'ml_ml3-m' : ''}'>
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM3)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM3)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM3)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width4-m' : i.Level === 3 ? 'ml_sl4-m' : i.Level === 2 ? 'ml_ml4-m' : ''}'>
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM4)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM4)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM4)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width5-m' : i.Level === 3 ? 'ml_sl5-m' : i.Level === 2 ? 'ml_ml5-m' : ''}'>                                    
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM5)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM5)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM5)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width6-m' : i.Level === 3 ? 'ml_sl6-m' : i.Level === 2 ? 'ml_ml6-m' : ''}'>                                    
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM6)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM6)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM6)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width7-m' : i.Level === 3 ? 'ml_sl7-m' : i.Level === 2 ? 'ml_ml7-m' : ''}'>
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM7)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM7)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM7)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width8-m' : i.Level === 3 ? 'ml_sl8-m' : i.Level === 2 ? 'ml_ml8-m' : ''}'>
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM8)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM8)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM8)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width9-m' : i.Level === 3 ? 'ml_sl9-m' : i.Level === 2 ? 'ml_ml9-m' : ''}'>
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM9)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM9)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM9)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width10-m' : i.Level === 3 ? 'ml_sl10-m' : i.Level === 2 ? 'ml_ml10-m' : ''}'>
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM10)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM10)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM10)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width11-m' : i.Level === 3 ? 'ml_sl11-m' : i.Level === 2 ? 'ml_ml11-m' : ''}'>
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM11)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM11)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM11)}</span></div>
                                </div>
                            </div>
                            <div class='col-md-4 ${i.Level === 4 ? 'colT-width12-m' : i.Level === 3 ? 'ml_sl12-m' : i.Level === 2 ? 'ml_ml12-m' : ''}'>
                                <div class='row'>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.DM12)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.CM12)}</span></div>
                                    <div class='col-md-5'><span class='dotBorder font-size'>${i.CurrencyName} ${curFormat(i.BM12)}</span></div>
                                </div>
                            </div>
                        `));
                    }
                })
            }
        }
    }
    function getItemByGroup(items, parentId) {
        return groupBy(items, ["ParentId"])[parentId];
    }
    function groupBy(values, keys, process) {
        let grouped = {};
        $.each(values, function (k, a) {
            keys.reduce(function (o, k, i) {
                o[a[k]] = o[a[k]] || (i + 1 === keys.length ? [] : {});
                return o[a[k]];
            }, grouped).push(a);
        });
        if (!!process && typeof process === "function") {
            process(grouped);
        }
        return grouped;
    }

    function bindGategoties(container, res, prop) {
        if (isValidJSON(res)) {
            let __ul = $(`<ul class='menu-item-list mainacc'></ul>`);
            let _li = $("<li></li>");
            let __div = $(`<div class='menu-item flex-container'></div>`);
            let __divTotal = $(`<div class='totalAccount'></div>`);
            let $treeview = $(`<span class='font-size text-danger'>${res.Name}</span>`);
            let spanAmount = $(`<span class='font-size'></span>`);
            __div.append($treeview).append(spanAmount)
            _li.append(__div).append(__divTotal);
            __ul.append(_li);
            if (__AllData[0].ShowGLAcc && __AllData[0].Count) {
                const asset = __AllData[0].Asset;
                const liabi = __AllData[0].Liability;
                const capital = __AllData[0].CapitalandReserve;
                const turnO = __AllData[0].Turnover;
                const costOfsale = __AllData[0].CostOfSale;
                const opeC = __AllData[0].OperatingCost;
                const nOpeInExp = __AllData[0].NonOperatingIncomeExpenditure;
                const tax = __AllData[0].TaxationExtraordinaryItem;
                $("#totalDCB").prop("hidden", false)
                if (__AllData[0].AnnualReport) {
                    $(`${container}-AnnualReport ul`).remove();
                    $(`${container}-AnnualReport`).append(__ul);
                    bindGla(__ul, getItemByGroup(__AllData[0].ProfitAndLossReportViewModel[prop], res.ID), prop);
                    const sumDebit = asset.Debit + liabi.Debit + capital.Debit + turnO.Debit + costOfsale.Debit + opeC.Debit + nOpeInExp.Debit + tax.Debit;
                    const sumCredit = asset.Credit + liabi.Credit + capital.Credit + turnO.Credit + costOfsale.Credit + opeC.Credit + nOpeInExp.Credit + tax.Credit;
                    const sumBalance = sumDebit - sumCredit === 0 ? "" : `${asset.CurrencyName} ${curFormat(sumDebit - sumCredit)}`;
                    $("#totalDebit").text(`${asset.CurrencyName} ${curFormat(sumDebit)}`);
                    $("#totalCredit").text(`${asset.CurrencyName} ${curFormat(sumCredit)}`);
                    $("#TotalBalance").text(`${sumBalance}`)
                }
                if (__AllData[0].QuarterlyReport) {
                    $("#totalDCBQ").prop("hidden", false)
                    $(`${container}-QuarterlyReport ul`).remove();
                    $(`${container}-QuarterlyReport`).append(__ul);
                    bindGla(__ul, getItemByGroup(__AllData[0].ProfitAndLossReportViewModel[prop], res.ID), prop);
                    const sumDebit = asset.Debit + liabi.Debit + capital.Debit + turnO.Debit + costOfsale.Debit + opeC.Debit + nOpeInExp.Debit + tax.Debit;
                    const sumCredit = asset.Credit + liabi.Credit + capital.Credit + turnO.Credit + costOfsale.Credit + opeC.Credit + nOpeInExp.Credit + tax.Credit;
                    const sumBalance = sumDebit - sumCredit === 0 ? "" : `${asset.CurrencyName} ${curFormat(sumDebit - sumCredit)}`;

                    const dq1 = asset.DQ1 + liabi.DQ1 + capital.DQ1 + turnO.DQ1 + costOfsale.DQ1 + opeC.DQ1 + nOpeInExp.DQ1 + tax.DQ1;
                    const cq1 = asset.CQ1 + liabi.CQ1 + capital.CQ1 + turnO.CQ1 + costOfsale.CQ1 + opeC.CQ1 + nOpeInExp.CQ1 + tax.CQ1;
                    const bq1 = dq1 - cq1 === 0 ? "" : `${asset.CurrencyName} ${curFormat(dq1 - cq1)}`;

                    const dq2 = asset.DQ2 + liabi.DQ2 + capital.DQ2 + turnO.DQ2 + costOfsale.DQ2 + opeC.DQ2 + nOpeInExp.DQ2 + tax.DQ2;
                    const cq2 = asset.CQ2 + liabi.CQ2 + capital.CQ2 + turnO.CQ2 + costOfsale.CQ2 + opeC.CQ2 + nOpeInExp.CQ2 + tax.CQ2;
                    const bq2 = dq2 - cq2 === 0 ? "" : `${asset.CurrencyName} ${curFormat(dq2 - cq2)}`;

                    const dq3 = asset.DQ3 + liabi.DQ3 + capital.DQ3 + turnO.DQ3 + costOfsale.DQ3 + opeC.DQ3 + nOpeInExp.DQ3 + tax.DQ3;
                    const cq3 = asset.CQ3 + liabi.CQ3 + capital.CQ3 + turnO.CQ3 + costOfsale.CQ3 + opeC.CQ3 + nOpeInExp.CQ3 + tax.CQ3;
                    const bq3 = dq3 - cq3 === 0 ? "" : `${asset.CurrencyName} ${curFormat(dq3 - cq3)}`;

                    const dq4 = asset.DQ4 + liabi.DQ4 + capital.DQ4 + turnO.DQ4 + costOfsale.DQ4 + opeC.DQ4 + nOpeInExp.DQ4 + tax.DQ4;
                    const cq4 = asset.CQ4 + liabi.CQ4 + capital.CQ4 + turnO.CQ4 + costOfsale.CQ4 + opeC.CQ4 + nOpeInExp.CQ4 + tax.CQ4;
                    const bq4 = dq4 - cq4 === 0 ? "" : `${asset.CurrencyName} ${curFormat(dq4 - cq4)}`;
                    $(".dq").text(`${asset.CurrencyName} ${curFormat(sumDebit)}`);
                    $(".cq").text(`${asset.CurrencyName} ${curFormat(sumCredit)}`);
                    $(".bq").text(`${sumBalance}`)

                    $(".dq1").text(`${asset.CurrencyName} ${curFormat(dq1)}`);
                    $(".cq1").text(`${asset.CurrencyName} ${curFormat(cq1)}`);
                    $(".bq1").text(`${bq1}`)

                    $(".dq2").text(`${asset.CurrencyName} ${curFormat(dq2)}`);
                    $(".cq2").text(`${asset.CurrencyName} ${curFormat(cq2)}`);
                    $(".bq2").text(`${bq2}`)

                    $(".dq3").text(`${asset.CurrencyName} ${curFormat(dq3)}`);
                    $(".cq3").text(`${asset.CurrencyName} ${curFormat(cq3)}`);
                    $(".bq3").text(`${bq3}`)

                    $(".dq4").text(`${asset.CurrencyName} ${curFormat(dq4)}`);
                    $(".cq4").text(`${asset.CurrencyName} ${curFormat(cq4)}`);
                    $(".bq4").text(`${bq4}`)

                }
                if (__AllData[0].MonthlyReport) {
                    const assetM = __AllData[0].Asset2;
                    const liabiM = __AllData[0].Liability2;
                    const capitalM = __AllData[0].CapitalandReserve2;
                    const turnOM = __AllData[0].Turnover2;
                    const costOfsaleM = __AllData[0].CostOfSale2;
                    const opeCM = __AllData[0].OperatingCost2;
                    const nOpeInExpM = __AllData[0].NonOperatingIncomeExpenditure2;
                    const taxM = __AllData[0].TaxationExtraordinaryItem2;
                    $(`${container}-MonthlyReport ul`).remove();
                    $(`${container}-MonthlyReport`).append(__ul);
                    bindGla(__ul, getItemByGroup(__AllData[0].ProfitAndLossReportViewModel[prop], res.ID), prop);
                    $("#totalDCBM").prop("hidden", false)
                    const sumDebit = assetM.Debit + liabiM.Debit + capitalM.Debit + turnOM.Debit + costOfsaleM.Debit + opeCM.Debit + nOpeInExpM.Debit + taxM.Debit;
                    const sumCredit = assetM.Credit + liabiM.Credit + capitalM.Credit + turnOM.Credit + costOfsaleM.Credit + opeCM.Credit + nOpeInExpM.Credit + taxM.Credit;
                    const sumBalance = sumDebit - sumCredit === 0 ? "" : `${assetM.CurrencyName} ${curFormat(sumDebit - sumCredit)}`;

                    const dM1 = assetM.DM1 + liabiM.DM1 + capitalM.DM1 + turnOM.DM1 + costOfsaleM.DM1 + opeCM.DM1 + nOpeInExpM.DM1 + taxM.DM1;
                    const cM1 = assetM.CM1 + liabiM.CM1 + capitalM.CM1 + turnOM.CM1 + costOfsaleM.CM1 + opeCM.CM1 + nOpeInExpM.CM1 + taxM.CM1;
                    const bM1 = dM1 - cM1 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dM1 - cM1)}`;

                    const dm2 = assetM.DM2 + liabiM.DM2 + capitalM.DM2 + turnOM.DM2 + costOfsaleM.DM2 + opeCM.DM2 + nOpeInExpM.DM2 + taxM.DM2;
                    const cm2 = assetM.CM2 + liabiM.CM2 + capitalM.CM2 + turnOM.CM2 + costOfsaleM.CM2 + opeCM.CM2 + nOpeInExpM.CM2 + taxM.CM2;
                    const bm2 = dm2 - cm2 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm2 - cm2)}`;

                    const dm3 = assetM.DM3 + liabiM.DM3 + capitalM.DM3 + turnOM.DM3 + costOfsaleM.DM3 + opeCM.DM3 + nOpeInExpM.DM3 + taxM.DM3;
                    const cm3 = assetM.CM3 + liabiM.CM3 + capitalM.CM3 + turnOM.CM3 + costOfsaleM.CM3 + opeCM.CM3 + nOpeInExpM.CM3 + taxM.CM3;
                    const bm3 = dm3 - cm3 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm3 - cm3)}`;

                    const dm4 = assetM.DM4 + liabiM.DM4 + capitalM.DM4 + turnOM.DM4 + costOfsaleM.DM4 + opeCM.DM4 + nOpeInExpM.DM4 + taxM.DM4;
                    const cm4 = assetM.CM4 + liabiM.CM4 + capitalM.CM4 + turnOM.CM4 + costOfsaleM.CM4 + opeCM.CM4 + nOpeInExpM.CM4 + taxM.CM4;
                    const bm4 = dm4 - cm4 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm4 - cm4)}`;

                    const dm5 = assetM.DM5 + liabiM.DM5 + capitalM.DM5 + turnOM.DM5 + costOfsaleM.DM5 + opeCM.DM5 + nOpeInExpM.DM5 + taxM.DM5;
                    const cm5 = assetM.CM5 + liabiM.CM5 + capitalM.CM5 + turnOM.CM5 + costOfsaleM.CM5 + opeCM.CM5 + nOpeInExpM.CM5 + taxM.CM5;
                    const bm5 = dm5 - cm5 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm5 - cm5)}`;

                    const dm6 = assetM.DM6 + liabiM.DM6 + capitalM.DM6 + turnOM.DM6 + costOfsaleM.DM6 + opeCM.DM6 + nOpeInExpM.DM6 + taxM.DM6;
                    const cm6 = assetM.CM6 + liabiM.CM6 + capitalM.CM6 + turnOM.CM6 + costOfsaleM.CM6 + opeCM.CM6 + nOpeInExpM.CM6 + taxM.CM6;
                    const bm6 = dm3 - cm6 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm6 - cm6)}`;

                    const dm7 = assetM.DM7 + liabiM.DM7 + capitalM.DM7 + turnOM.DM7 + costOfsaleM.DM7 + opeCM.DM7 + nOpeInExpM.DM7 + taxM.DM7;
                    const cm7 = assetM.CM7 + liabiM.CM7 + capitalM.CM7 + turnOM.CM7 + costOfsaleM.CM7 + opeCM.CM7 + nOpeInExpM.CM7 + taxM.CM7;
                    const bm7 = dm4 - cm7 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm7 - cm7)}`;

                    const dm8 = assetM.DM8 + liabiM.DM8 + capitalM.DM8 + turnOM.DM8 + costOfsaleM.DM8 + opeCM.DM8 + nOpeInExpM.DM8 + taxM.DM8;
                    const cm8 = assetM.CM8 + liabiM.CM8 + capitalM.CM8 + turnOM.CM8 + costOfsaleM.CM8 + opeCM.CM8 + nOpeInExpM.CM8 + taxM.CM8;
                    const bm8 = dm2 - cm8 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm8 - cm8)}`;

                    const dm9 = assetM.DM9 + liabiM.DM9 + capitalM.DM9 + turnOM.DM9 + costOfsaleM.DM9 + opeCM.DM9 + nOpeInExpM.DM9 + taxM.DM9;
                    const cm9 = assetM.CM9 + liabiM.CM9 + capitalM.CM9 + turnOM.CM9 + costOfsaleM.CM9 + opeCM.CM9 + nOpeInExpM.CM9 + taxM.CM9;
                    const bm9 = dm3 - cm9 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm9 - cm9)}`;

                    const dm10 = assetM.DM10 + liabiM.DM10 + capitalM.DM10 + turnOM.DM10 + costOfsaleM.DM10 + opeCM.DM10 + nOpeInExpM.DM10 + taxM.DM10;
                    const cm10 = assetM.CM10 + liabiM.CM10 + capitalM.CM10 + turnOM.CM10 + costOfsaleM.CM10 + opeCM.CM10 + nOpeInExpM.CM10 + taxM.CM10;
                    const bm10 = dm4 - cm10 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm10 - cm10)}`;

                    const dm11 = assetM.DM11 + liabiM.DM11 + capitalM.DM11 + turnOM.DM11 + costOfsaleM.DM11 + opeCM.DM11 + nOpeInExpM.DM11 + taxM.DM11;
                    const cm11 = assetM.CM11 + liabiM.CM11 + capitalM.CM11 + turnOM.CM11 + costOfsaleM.CM11 + opeCM.CM11 + nOpeInExpM.CM11 + taxM.CM11;
                    const bm11 = dm11 - cm11 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm11 - cm11)}`;

                    const dm12 = assetM.DM12 + liabiM.DM12 + capitalM.DM12 + turnOM.DM12 + costOfsaleM.DM12 + opeCM.DM12 + nOpeInExpM.DM12 + taxM.DM12;
                    const cm12 = assetM.CM12 + liabiM.CM12 + capitalM.CM12 + turnOM.CM12 + costOfsaleM.CM12 + opeCM.CM12 + nOpeInExpM.CM12 + taxM.CM12;
                    const bm12 = dm12 - cm12 === 0 ? "" : `${assetM.CurrencyName} ${curFormat(dm12 - cm12)}`;

                    $(".dm").text(`${assetM.CurrencyName} ${curFormat(sumDebit)}`);
                    $(".cm").text(`${assetM.CurrencyName} ${curFormat(sumCredit)}`);
                    $(".bm").text(`${sumBalance}`)

                    $(".dm1").text(`${assetM.CurrencyName} ${curFormat(dM1)}`);
                    $(".cm1").text(`${assetM.CurrencyName} ${curFormat(cM1)}`);
                    $(".bm1").text(`${bM1}`)

                    $(".dm2").text(`${assetM.CurrencyName} ${curFormat(dm2)}`);
                    $(".cm2").text(`${assetM.CurrencyName} ${curFormat(cm2)}`);
                    $(".bm2").text(`${bm2}`)

                    $(".dm3").text(`${assetM.CurrencyName} ${curFormat(dm3)}`);
                    $(".cm3").text(`${assetM.CurrencyName} ${curFormat(cm3)}`);
                    $(".bm3").text(`${bm3}`)

                    $(".dm4").text(`${assetM.CurrencyName} ${curFormat(dm4)}`);
                    $(".cm4").text(`${assetM.CurrencyName} ${curFormat(cm4)}`);
                    $(".bm4").text(`${bm4}`)

                    $(".dm5").text(`${assetM.CurrencyName} ${curFormat(dm5)}`);
                    $(".cm5").text(`${assetM.CurrencyName} ${curFormat(cm5)}`);
                    $(".bm5").text(`${bm5}`)

                    $(".dm6").text(`${assetM.CurrencyName} ${curFormat(dm6)}`);
                    $(".cm6").text(`${assetM.CurrencyName} ${curFormat(cm6)}`);
                    $(".bm6").text(`${bm6}`)

                    $(".dm7").text(`${assetM.CurrencyName} ${curFormat(dm7)}`);
                    $(".cm7").text(`${assetM.CurrencyName} ${curFormat(cm7)}`);
                    $(".bm7").text(`${bm7}`)

                    $(".dm8").text(`${assetM.CurrencyName} ${curFormat(dm8)}`);
                    $(".cm8").text(`${assetM.CurrencyName} ${curFormat(cm8)}`);
                    $(".bm8").text(`${bm8}`)

                    $(".dm9").text(`${assetM.CurrencyName} ${curFormat(dm9)}`);
                    $(".cm9").text(`${assetM.CurrencyName} ${curFormat(cm9)}`);
                    $(".bm9").text(`${bm9}`)

                    $(".dm10").text(`${assetM.CurrencyName} ${curFormat(dm10)}`);
                    $(".cm10").text(`${assetM.CurrencyName} ${curFormat(cm10)}`);
                    $(".bm10").text(`${bm10}`)

                    $(".dm11").text(`${assetM.CurrencyName} ${curFormat(dm11)}`);
                    $(".cm11").text(`${assetM.CurrencyName} ${curFormat(cm11)}`);
                    $(".bm11").text(`${bm11}`)

                    $(".dm12").text(`${assetM.CurrencyName} ${curFormat(dm12)}`);
                    $(".cm12").text(`${assetM.CurrencyName} ${curFormat(cm12)}`);
                    $(".bm12").text(`${bm12}`)
                }
                if (__AllData[0].PeriodicReport) {
                    $(`${container}-PeriodicReport ul`).remove();
                    $(`${container}-PeriodicReport`).append(__ul);
                    bindGla(__ul, getItemByGroup(__AllData[0].ProfitAndLossReportViewModel[prop], res.ID), prop);
                }


            } else {
                $(`${container}-AnnualReport ul`).remove();
                $(`${container}-QuarterlyReport ul`).remove();
                $(`${container}-MonthlyReport ul`).remove();
                $(`${container}-PeriodicReport ul`).remove();
                $("#totalDCB").prop("hidden", true)
                $("#totalDCBQ").prop("hidden", true)
                $("#totalDCBM").prop("hidden", true)
            }
        }
    }
    function GetGategories(code, success) {
        $.get("/FinancialReports/GetLvl1", { code }, success);
    }
    //Util functions
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }
});