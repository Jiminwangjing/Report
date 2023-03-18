$(document).ready(function () {
    const model = JSON.parse($("#datamodel").text());
    if (model.AnnualReport) {
        $("#AnnualReport").prop("hidden", false);
        $("#QuarterlyReport").prop("hidden", true);
        $("#MonthlyReport").prop("hidden", true);
        $("#PeriodicReport").prop("hidden", true);
    }
    if (model.QuarterlyReport) {
        $("#AnnualReport").prop("hidden", true);
        $("#QuarterlyReport").prop("hidden", false);
        $("#MonthlyReport").prop("hidden", true);
        $("#PeriodicReport").prop("hidden", true);
    }
    if (model.MonthlyReport) {
        $("#AnnualReport").prop("hidden", true);
        $("#QuarterlyReport").prop("hidden", true);
        $("#MonthlyReport").prop("hidden", false);
        $("#PeriodicReport").prop("hidden", true);
    }
    if (model.PeriodicReport) {
        $("#AnnualReport").prop("hidden", true);
        $("#QuarterlyReport").prop("hidden", true);
        $("#MonthlyReport").prop("hidden", true);
        $("#PeriodicReport").prop("hidden", false);
    }
    const turnover = JSON.parse($("#turnover").text());
    const costOfSales = JSON.parse($("#costOfSales").text());
    const operatingCosts = JSON.parse($("#operatingCosts").text());
    const nonOperatingIncomeExpenditure = JSON.parse($("#nonOperatingIncomeExpenditure").text());
    const taxationExtraordinaryItems = JSON.parse($("#taxationExtraordinaryItems").text());
    setTimeout(function () {
        bindGategotiesPL("turnover", "Total Turnover", "#turnover", turnover, "Turnover", true);
    }, 0);

    setTimeout(function () {
        bindGategotiesPL("gross", "Gross Profit", "#cost-of-sales", costOfSales, "CostOfSales");
    }, 100);

    setTimeout(function () {
        bindGategotiesPL("operating", "Operating Profit", "#operating-costs", operatingCosts, "OperatingCosts");
    }, 200);

    setTimeout(function () {
        bindGategotiesPL("peofit", "Profit After Financing Expenses", "#non-operating-income-expenditure", nonOperatingIncomeExpenditure, "NonOperatingIncomeExpenditure");
    }, 300);

    setTimeout(function () {
        bindGategotiesPL("total", "Total Taxation and Extraordinary Items", "#taxation-extraordinary-items", taxationExtraordinaryItems, "TaxationExtraordinaryItems");
    }, 400);
    $("#pdf").click(function () {
        window.print();
    })
    function bindLevel(container, group, prop) {
        if (isValidArray(group)) {
            let __ul = $(`<ul class='menu-item-list'></ul>`);
            if (model.AnnualReport) {
                group.forEach(i => {
                    let _li = $("<li></li>");
                    let __div = $(`<div class='menu-item flex-container'></div>`);
                    let __divTotal = $(`<div class='totalAccount'></div>`);
                    let $treeview = $(`<span class='font-size'>${i.Code} - ${i.Name}</span>`);
                    let spanAmount = '';

                    if (i.IsActive) {
                        spanAmount = $(`
                            <span class='font-size totalAccount2' data-sum='${i.Balance}'>
                            ${i.CurrencyName} ${curFormat(i.Balance)}
                            </span>`);
                    }

                    if (i.IsActive === false && i.IsTitle === true) {
                        $treeview.addClass("text-primary");
                    }
                    __div.append($treeview).append(spanAmount)
                    _li.append(__div).append(__divTotal);
                    __ul.append(_li);
                    bindLevel(_li, getItemByGroup(model[prop], i.ID), prop);
                    $(container).append(__ul);

                    if (!i.IsActive) {
                        _li.append(__divTotal.append(`<span class="pm">Total ${i.Code}-${i.Name} : </span> <span class='dotBorder'>${i.CurrencyName} ${curFormat(i.Balance)}</span>`));
                    }
                })
            }
            if (model.QuarterlyReport) {
                group.forEach(i => {
                    let _li = $("<li></li>");
                    let __div = $(`<div class='flex-container'></div>`);
                    let __divTotal = $(`<div class='totalAccount'></div>`);
                    let $treeview = $(`<span class='flexQauater1 font-size'>${i.Code} - ${i.Name}</span>`);
                    let q1 = "", q2 = "", q3 = "", q4 = "";

                    if (i.IsActive) {
                        q1 = $(`<span class='flexQauater2 font-size'>${i.CurrencyName} ${curFormat(i.Q1 == null ? "" : i.Q1)}</span>`);
                        q2 = $(`<span class='flexQauater2 font-size'>${i.CurrencyName} ${curFormat(i.Q2 == null ? "" : i.Q2)}</span>`);
                        q3 = $(`<span class='flexQauater2 font-size'>${i.CurrencyName} ${curFormat(i.Q3 == null ? "" : i.Q3)}</span>`);
                        q4 = $(`<span class='flexQauater2 font-size'>${i.CurrencyName} ${curFormat(i.Q4 == null ? "" : i.Q4)}</span>`);
                    }

                    if (i.IsActive === false && i.IsTitle === true) {
                        $treeview.addClass("text-primary");
                    }
                    __div.append($treeview).append(q1).append(q2).append(q3).append(q4)
                    _li.append(__div).append(__divTotal);
                    __ul.append(_li);

                    bindLevel(_li, getItemByGroup(model[prop], i.ID), prop);
                    $(container).append(__ul);

                    if (!i.IsActive) {
                        _li.append(__divTotal.append(`
                            <span class="pm flexQauaterTotal1">Total ${i.Code}-${i.Name} : </span> 
                            <span class='dotBorder flexQauaterTotal2'>${i.CurrencyName} ${curFormat(i.Q1 == null ? "" : i.Q1)}</span>
                            <span class='dotBorder flexQauaterTotal2'>${i.CurrencyName} ${curFormat(i.Q2 == null ? "" : i.Q2)}</span>
                            <span class='dotBorder flexQauaterTotal2'>${i.CurrencyName} ${curFormat(i.Q3 == null ? "" : i.Q3)}</span>
                            <span class='dotBorder flexQauaterTotal2'>${i.CurrencyName} ${curFormat(i.Q4 == null ? "" : i.Q4)}</span>
                        `));
                    }
                })
            }
            if (model.MonthlyReport) {
                group.forEach(i => {
                    let row = $(`<div class='row'></div>`);
                    let col3 = $(`<div class='col-md-4' style="width: 400px;" ></div>`);
                    let col9 = $(`<div class='col-md-10' style="margin-left: -40px;"></div>`);
                    let row_col9 = $(`<div class='row dpCol1'></div>`);
                    let _li = $("<li></li>");
                    let __divTotal = $(`<div class='row'></div>`);
                    let $treeview = $(`<span class='text-nowrap font-size'>${i.Code} - ${i.Name}</span>`);
                    let m1 = "", m2 = "", m3 = "", m4 = "", m5 = "", m6 = "",
                        m7 = "", m8 = "", m9 = "", m10 = "", m11 = "", m12 = "";

                    if (i.IsActive) {
                        m1 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M1 == null ? "" : i.M1)}</span></div>`);
                        m2 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M2 == null ? "" : i.M2)}</span></div>`);
                        m3 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M3 == null ? "" : i.M3)}</span></div>`);
                        m4 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M4 == null ? "" : i.M4)}</span></div>`);
                        m5 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M5 == null ? "" : i.M5)}</span></div>`);
                        m6 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M6 == null ? "" : i.M6)}</span></div>`);
                        m7 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M7 == null ? "" : i.M7)}</span></div>`);
                        m8 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M8 == null ? "" : i.M8)}</span></div>`);
                        m9 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M9 == null ? "" : i.M9)}</span></div>`);
                        m10 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M10 == null ? "" : i.M10)}</span></div>`);
                        m11 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M11 == null ? "" : i.M11)}</span></div>`);
                        m12 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M12 == null ? "" : i.M12)}</span></div>`);
                        row_col9.addClass("ml_0_0").children().addClass("text-center");
                    }

                    if (i.IsActive === false && i.IsTitle === true) {
                        $treeview.addClass("text-primary");
                    }
                    col3.append($treeview);
                    row_col9.append(m1).append(m2).append(m3).append(m4).append(m5).append(m6)
                        .append(m7).append(m8).append(m9).append(m10).append(m11).append(m12);
                    col9.append(row_col9);
                    row.append(col3).append(col9);
                    _li.append(row).append(__divTotal);
                    __ul.append(_li);

                    bindLevel(_li, getItemByGroup(model[prop], i.ID), prop);
                    $(container).append(__ul);

                    if (!i.IsActive) {
                        let _col3 = $(`<div class='col-md-4' style="width: 400px;"></div>`);
                        let _col9 = $(`<div class='col-md-10' style="margin-left: -40px;"></div>`);
                        let _row_col9 = $(`<div class='row dpCol1'></div>`);
                        _col3.append(`<span class="pm text-nowrap">Total ${i.Code}-${i.Name} : </span> `);
                        _row_col9.append(`                            
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M1 == null ? "" : i.M1)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M2 == null ? "" : i.M2)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M3 == null ? "" : i.M3)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M4 == null ? "" : i.M4)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M5 == null ? "" : i.M5)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M6 == null ? "" : i.M6)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M7 == null ? "" : i.M7)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M8 == null ? "" : i.M8)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M9 == null ? "" : i.M9)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M10 == null ? "" : i.M10)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M11 == null ? "" : i.M11)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M12 == null ? "" : i.M12)}</span></div>
                        `);

                        _col9.append(_row_col9);
                        if (i.Level == 2)
                            _col9.children().addClass("ml_3");
                        if (i.Level == 3)
                            _col9.children().addClass("ml_1");
                        if (i.Level == 4)
                            _col9.children().addClass("ml_0");

                        __divTotal.append(_col3).append(_col9);
                        _li.append(__divTotal);
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

    let MEMSUM = 0;
    function bindGategotiesPL(textcom, text, container, res, prop) {
        $(`${container} ul`).remove();
        const dataSum = model[prop][model[prop].length - 1].AccountBalances;
        let sum_one_acc = 0;
        if (isValidJSON(res)) {
            let __ul = $(`<ul class='menu-item-list mainacc'></ul>`);
            let _li = $("<li class='ml-5'></li>");
            let __div = $(`<div class='menu-item flex-container'></div>`);
            let __divTotal = $(`<div class='totalAccount'></div>`);
            let $treeview = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up text-danger'>${res.Name}</span>`);
            let spanAmount = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up'></span>`);
            __div.append($treeview).append(spanAmount)
            _li.append(__div).append(__divTotal);
            __ul.append(_li);
            if (model.AnnualReport) {
                $(`${container}-AnnualReport ul`).remove();
                $(`${container}-AnnualReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(model[prop], res.ID), prop);
                //MEMSUM = 0;
                if (textcom == "turnover") {
                    if (dataSum.length > 0) {

                        dataSum.forEach(i => {
                            sum_one_acc += ((i.Debit - i.Credit) * (-1));
                            MEMSUM = sum_one_acc;
                        })
                    }
                } else {
                    if (dataSum.length > 0) {
                        dataSum.forEach(i => {
                            MEMSUM += ((i.Debit - i.Credit) * (-1));
                        })
                    }
                }
                let _li_sum = $("<li></li>");
                let __div_sum = $(`<div class='flex-container'></div>`);
                let __divTotal_sum = $(`<div class='totalAccount'></div>`);
                let $treeview_sum = $(`<span class='font-size'>${text}</span>`);
                let spanAmount_sum = $(`<span class='font-size'>${res.CurrencyName} ${curFormat(MEMSUM)}</span>`);
                __div_sum.append($treeview_sum).append(spanAmount_sum)
                _li_sum.append(__div_sum).append(__divTotal_sum);
                __ul.append(_li_sum);
            }
            if (model.QuarterlyReport) {
                let sumq1 = 0;
                let sumq2 = 0;
                let sumq3 = 0;
                let sumq4 = 0;
                let sumLv2 = [];
                $(`${container}-QuarterlyReport ul`).remove();
                $(`${container}-QuarterlyReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(model[prop], res.ID), prop);
                if (textcom == "turnover") {
                    model[prop].forEach(i => {
                        if (i.Level === 2)
                            sumLv2.push(i);
                    });
                    sumLv2.forEach(i => {
                        sumq1 += i.Q1;
                        sumq2 += i.Q2;
                        sumq3 += i.Q3;
                        sumq4 += i.Q4;
                    });
                }
                let _li_sum = $("<li></li>");
                let __div_sum = $(`<div class='flex-container'></div>`);
                let __divTotal_sum = $(`<div class='totalAccount'></div>`);
                let $treeview_sum = $(`<span class='font-size flexSizeQ'>${text}</span>`);
                let q1 = $(`
                    <span class='font-size flexSizeQ1'
                    data-sum='${sumq1}' id='${textcom}1'>${res.CurrencyName} ${curFormat(sumq1)}</span>`
                );
                let q2 = $(`
                        <span class='font-size flexSizeQ1'
                        data-sum='${sumq2}' id='${textcom}2'>${res.CurrencyName} ${curFormat(sumq2)}</span>`
                );
                let q3 = $(`
                        <span class='font-size flexSizeQ1' 
                        data-sum='${sumq3}' id='${textcom}3'>${res.CurrencyName} ${curFormat(sumq3)}</span>`
                );
                let q4 = $(`
                        <span class='font-size flexSizeQ1' 
                        data-sum='${sumq4}' id='${textcom}4'>${res.CurrencyName} ${curFormat(sumq4)}</span>`
                );
                __div_sum.append($treeview_sum).append(q1).append(q2).append(q3).append(q4)
                _li_sum.append(__div_sum).append(__divTotal_sum);
                __ul.append(_li_sum);
                if (textcom == "gross") {
                    sumPL(prop, "#turnover", textcom, res.CurrencyName);
                }
                if (textcom == "operating") {
                    sumPL(prop, "#gross", textcom, res.CurrencyName);
                }

                if (textcom == "peofit") {
                    sumPL(prop, "#operating", textcom, res.CurrencyName);
                }

                if (textcom == "total") {
                    sumPL(prop, "#peofit", textcom, res.CurrencyName);
                }
            }
            if (model.MonthlyReport) {
                sumLv2 = [];
                let sumM1 = 0, sumM2 = 0, sumM3 = 0, sumM4 = 0, sumM5 = 0, sumM6 = 0,
                    sumM7 = 0, sumM8 = 0, sumM9 = 0, sumM10 = 0, sumM11 = 0, sumM12 = 0
                $(`${container}-MonthlyReport ul`).remove();
                $(`${container}-MonthlyReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(model[prop], res.ID), prop);
                if (textcom == "turnover") {
                    model[prop].forEach(i => {
                        if (i.Level === 2)
                            sumLv2.push(i);
                    });
                    if (isValidArray(sumLv2)) {
                        sumLv2.forEach(i => {
                            sumM1 += i.M1;
                            sumM2 += i.M2;
                            sumM3 += i.M3;
                            sumM4 += i.M4;
                            sumM5 += i.M5;
                            sumM6 += i.M6;
                            sumM7 += i.M7;
                            sumM8 += i.M8;
                            sumM9 += i.M9;
                            sumM10 += i.M10;
                            sumM11 += i.M11;
                            sumM12 += i.M12;
                        });
                    }
                }
                let row = $(`<div class='row'></div>`);
                let col3 = $(`<div class='col-md-4' style="width: 600px"></div>`);
                let col9 = $(`<div class='col-md-11'></div>`);
                let row_col9 = $(`<div class='row col-total'></div>`);
                let _li_sum = $("<li></li>");
                let $treeview_sum = $(`<span class='font-size'>${text}</span>`);
                let m1 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM1}' id='${textcom}1'>
                        ${res.CurrencyName} ${curFormat(sumM1)}
                    </span></div>`
                );
                let m2 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM2}' id='${textcom}2'>
                        ${res.CurrencyName} ${curFormat(sumM2)}
                    </span></div>`
                );
                let m3 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM3}' id='${textcom}3'>
                        ${res.CurrencyName} ${curFormat(sumM3)}
                    </span></div>`
                );
                let m4 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM4}' id='${textcom}4'>
                        ${res.CurrencyName} ${curFormat(sumM4)}
                    </span></div>`
                );
                let m5 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM5}' id='${textcom}5'>
                        ${res.CurrencyName} ${curFormat(sumM5)}
                    </span></div>`
                );
                let m6 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM6}' id='${textcom}6'>
                        ${res.CurrencyName} ${curFormat(sumM6)}
                    </span></div>`
                );
                let m7 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM7}' id='${textcom}7'>
                        ${res.CurrencyName} ${curFormat(sumM7)}
                    </span></div>`
                );
                let m8 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM8}' id='${textcom}8'>
                        ${res.CurrencyName} ${curFormat(sumM8)}
                    </span></div>`
                );
                let m9 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM9}' id='${textcom}9'>
                        ${res.CurrencyName} ${curFormat(sumM9)}
                    </span></div>`
                );
                let m10 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM10}' id='${textcom}10'>
                        ${res.CurrencyName} ${curFormat(sumM10)}
                    </span></div>`
                );
                let m11 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM11}' id='${textcom}11'>
                        ${res.CurrencyName} ${curFormat(sumM11)}
                    </span></div>`
                );
                let m12 = $(`
                    <div class='col-md-2'><span class='font-size' data-sum='${sumM12}' id='${textcom}12'>
                        ${res.CurrencyName} ${curFormat(sumM12)}
                    </span></div>`
                );

                col3.append($treeview_sum);
                row_col9.append(m1).append(m2).append(m3).append(m4).append(m5).append(m6)
                    .append(m7).append(m8).append(m9).append(m10).append(m11).append(m12);
                col9.append(row_col9);
                row.append(col3).append(col9);

                _li_sum.append(row);
                __ul.append(_li_sum);
                if (textcom == "gross") {
                    sumPLM(prop, "#turnover", textcom, res.CurrencyName);
                }
                if (textcom == "operating") {
                    sumPLM(prop, "#gross", textcom, res.CurrencyName);
                }

                if (textcom == "peofit") {
                    sumPLM(prop, "#operating", textcom, res.CurrencyName);
                }

                if (textcom == "total") {
                    sumPLM(prop, "#peofit", textcom, res.CurrencyName);
                }
            }
            if (model.PeriodicReport) {
                $(`${container}-PeriodicReport ul`).remove();
                $(`${container}-PeriodicReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(model[prop], res.ID), prop);
            }
        }
    }
    //Util functions
    function sumPL(prop, idDataAttr, textcom, curname) {
        sumLv2 = [];
        let sumq1 = 0;
        let sumq2 = 0;
        let sumq3 = 0;
        let sumq4 = 0;
        model[prop].forEach(i => {
            if (i.Level === 2)
                sumLv2.push(i);
        });
        sumLv2.forEach(i => {
            sumq1 += i.Q1;
            sumq2 += i.Q2;
            sumq3 += i.Q3;
            sumq4 += i.Q4;
        });
        const SumQ1 = sumq1 + $(`${idDataAttr}1`).data("sum");
        const SumQ2 = sumq2 + $(`${idDataAttr}2`).data("sum");
        const SumQ3 = sumq3 + $(`${idDataAttr}3`).data("sum");
        const SumQ4 = sumq4 + $(`${idDataAttr}4`).data("sum");

        $(`#${textcom}1`).text(`${curname} ${curFormat(SumQ1)}`).data("sum", SumQ1);
        $(`#${textcom}2`).text(`${curname} ${curFormat(SumQ2)}`).data("sum", SumQ2);
        $(`#${textcom}3`).text(`${curname} ${curFormat(SumQ3)}`).data("sum", SumQ3);
        $(`#${textcom}4`).text(`${curname} ${curFormat(SumQ4)}`).data("sum", SumQ4);
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }
    //format currency
    function curFormat(value) {
        return value.toFixed(3).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
});