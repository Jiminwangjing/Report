$(document).ready(function () {
        const model = JSON.parse($("#datamodel").text());
        const asset = JSON.parse($("#asset").text());
        const capReseve = JSON.parse($("#capReseve").text());
        const liability = JSON.parse($("#liability").text());
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
        setTimeout(function () {
            bindGategoties("#data", asset, "Assets");
        }, 0);
        setTimeout(function () {
            bindGategoties("#data1", capReseve, "Liabilities");
        }, 300);
        setTimeout(function () {
            bindGategoties("#data2", liability, "CapitalandReserves");
        }, 600);
        $("#pdf").click(function () {
            window.print();
        })
        function bindLevel(container, group, prop) {
            if (isValidArray(group)) {
                let __ul = $(`<ul class='menu-item-list'></ul>`);
                if (model.AnnualReport) {
                    group.forEach(i => {
                        let _li = $("<li'></li>");
                        let __div = $(`<div class='menu-item flex-container'></div>`);
                        let __divTotal = $(`<div class='totalAccount'></div>`);
                        let $treeview = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up'>${i.Code} - ${i.Name}</span>`);
                        let spanAmount = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up'></span>`);

                        if (i.AccountBalance != null) {
                            spanAmount = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up text-right total' data-sum='${i.AccountBalance.CumulativeBalance}'>${i.CurrencyName} ${curFormat(i.AccountBalance.CumulativeBalance)}</span>`)
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
                        let __div = $(`<div class='menu-item flex-container'></div>`);
                        let __divTotal = $(`<div class='totalAccount'></div>`);
                        let $treeview = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexQauater1'>${i.Code} - ${i.Name}</span>`);
                        let q1 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexQauater2'></span>`);
                        let q2 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexQauater2'></span>`);
                        let q3 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexQauater2'></span>`);
                        let q4 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexQauater2'></span>`);

                        if (i.IsActive) {
                            q1 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexQauater2'>${i.CurrencyName} ${curFormat(i.Q1 == null ? "" : i.Q1)}</span>`);
                            q2 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexQauater2'>${i.CurrencyName} ${curFormat(i.Q2 == null ? "" : i.Q2)}</span>`);
                            q3 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexQauater2'>${i.CurrencyName} ${curFormat(i.Q3 == null ? "" : i.Q3)}</span>`);
                            q4 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexQauater2'>${i.CurrencyName} ${curFormat(i.Q4 == null ? "" : i.Q4)}</span>`);
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
                        let _li = $("<li></li>");
                        let __div = $(`<div class='menu-item flex-container'></div>`);
                        let __divTotal = $(`<div class='totalAccount'></div>`);
                        let $treeview = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth1'>${i.Code} - ${i.Name}</span>`);
                        let m1 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m2 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m3 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m4 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m5 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m6 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m7 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m8 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m9 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m10 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m11 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);
                        let m12 = "" //$(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'></span>`);

                        if (i.IsActive) {
                            m1 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M1 == null ? "" : i.M1)}</span>`);
                            m2 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M2 == null ? "" : i.M2)}</span>`);
                            m3 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M3 == null ? "" : i.M3)}</span>`);
                            m4 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M4 == null ? "" : i.M4)}</span>`);
                            m5 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M5 == null ? "" : i.M5)}</span>`);
                            m6 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M6 == null ? "" : i.M6)}</span>`);
                            m7 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M7 == null ? "" : i.M7)}</span>`);
                            m8 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M8 == null ? "" : i.M8)}</span>`);
                            m9 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M9 == null ? "" : i.M9)}</span>`);
                            m10 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M10 == null ? "" : i.M10)}</span>`);
                            m11 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M11 == null ? "" : i.M11)}</span>`);
                            m12 = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexmonth2'>${i.CurrencyName} ${curFormat(i.M12 == null ? "" : i.M12)}</span>`);
                        }

                        if (i.IsActive === false && i.IsTitle === true) {
                            $treeview.addClass("text-primary");
                        }
                        __div.append($treeview).append(m1).append(m2).append(m3).append(m4).append(m5).append(m6)
                            .append(m7).append(m8).append(m9).append(m10).append(m11).append(m12)
                        _li.append(__div).append(__divTotal);
                        __ul.append(_li);

                        bindLevel(_li, getItemByGroup(model[prop], i.ID), prop);
                        $(container).append(__ul);

                        if (!i.IsActive) {
                            _li.append(__divTotal.append(`
                            <span class="pm flexMonthTotal1">Total ${i.Code}-${i.Name} : </span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M1 == null ? "" : i.M1)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M2 == null ? "" : i.M2)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M3 == null ? "" : i.M3)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M4 == null ? "" : i.M4)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M5 == null ? "" : i.M5)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M6 == null ? "" : i.M6)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M7 == null ? "" : i.M7)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M8 == null ? "" : i.M8)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M9 == null ? "" : i.M9)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M10 == null ? "" : i.M10)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M11 == null ? "" : i.M11)}</span>
                            <span class='dotBorder flexMonthTotal2'>${i.CurrencyName} ${curFormat(i.M12 == null ? "" : i.M12)}</span>
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
                let $treeview = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up text-danger'>${res.Name}</span>`);
                let spanAmount = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up'></span>`);
                __div.append($treeview).append(spanAmount)
                _li.append(__div).append(__divTotal);
                __ul.append(_li);
                if (model.AnnualReport) {
                    $(`${container}-AnnualReport ul`).remove();
                    $(`${container}-AnnualReport`).append(__ul);
                    bindLevel(__ul, getItemByGroup(model[prop], res.ID), prop);
                }
                if (model.QuarterlyReport) {
                    $(`${container}-QuarterlyReport ul`).remove();
                    $(`${container}-QuarterlyReport`).append(__ul);
                    bindLevel(__ul, getItemByGroup(model[prop], res.ID), prop);
                }
                if (model.MonthlyReport) {
                    $(`${container}-MonthlyReport ul`).remove();
                    $(`${container}-MonthlyReport`).append(__ul);
                    bindLevel(__ul, getItemByGroup(model[prop], res.ID), prop);
                }
                if (model.PeriodicReport) {
                    $(`${container}-PeriodicReport ul`).remove();
                    $(`${container}-PeriodicReport`).append(__ul);
                    bindLevel(__ul, getItemByGroup(model[prop], res.ID), prop);
                }
            }
        }
        //Util functions
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