
$(document).ready(function ()
{
    let __glAccountData = [];
    let __dataExcel = {
        assets: [],
        liabilities: [],
        capitalReserves: []
    };
    var bitformatNumer = 2;
    //**** Click OK *******//
    $("#btn-ok").click(function () {
        $("#print").prop("disabled", false);
        $("#to-excel").prop("disabled", false);
        const date = $("#to-date").val();
        const typeDisplay = parseInt($("#d-report-select").val());
        const showZeroAcc = $("#checkAccZero").is(":checked") ? "true" : "false";
        GetGLAccountsActiveOnly(date, function (gla)
        {
            console.log("Data",gla);
            bitformatNumer = gla.FormatNumber;
            bindGlaActiveOnly("#active-only", gla);
        });
        $("#show-hide-loader").prop("hidden", false);
        $("#content").prop("hidden", true);
        GetGLAccounts(date, typeDisplay, showZeroAcc, function (gla)
        {
            bitformatNumer = gla.FormatNumber;
            
            if (gla.AnnualReport) {
                $("#AnnualReport").prop("hidden", false);
                $("#QuarterlyReport").prop("hidden", true);
                $("#MonthlyReport").prop("hidden", true);
                $("#PeriodicReport").prop("hidden", true);
            }
            if (gla.QuarterlyReport) {
                $("#AnnualReport").prop("hidden", true);
                $("#QuarterlyReport").prop("hidden", false);
                $("#MonthlyReport").prop("hidden", true);
                $("#PeriodicReport").prop("hidden", true);
            }
            if (gla.MonthlyReport) {
                $("#AnnualReport").prop("hidden", true);
                $("#QuarterlyReport").prop("hidden", true);
                $("#MonthlyReport").prop("hidden", false);
                $("#PeriodicReport").prop("hidden", true);
            }
            if (gla.PeriodicReport) {
                $("#AnnualReport").prop("hidden", true);
                $("#QuarterlyReport").prop("hidden", true);
                $("#MonthlyReport").prop("hidden", true);
                $("#PeriodicReport").prop("hidden", false);
            }
            if (isValidArray(__glAccountData)) {
                __glAccountData = [];
                __glAccountData.push(gla);
            } else {
                __glAccountData.push(gla);
            }
            GetGategories("100000000000000", function (res) {
                bitformatNumer = res.FormatNumber;
                bindGategoties("#data", res, "Assets");
            });
            GetGategories("200000000000000", function (res) {
                bitformatNumer = res.FormatNumber;
                bindGategoties("#data1", res, "Liabilities");
            });
            GetGategories("300000000000000", function (res) {
                bitformatNumer = res.FormatNumber;
                bindGategoties("#data2", res, "CapitalandReserves");
            });
            $("#show-hide-loader").prop("hidden", true);
            $("#content").prop("hidden", false);
        });
    });

    $(".sh-posting-period").click(function () {
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
                                $("#to-date").val(formatDate(e.data.PostingDateTo, "YYYY-MM-DD"));
                                $("#hide-title").prop("disabled", false);
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
    });
    search("#find-cus", ".posting-period tr:not(:first-child)");
    $("#hide-title").click(function () {
        if ($(this).is(":checked")) {
            $("#data").hide();
            $("#active-only").prop("hidden", false);
        }
        else {
            $("#data").show();
            $("#active-only").prop("hidden", true);
        }
    })
    $("#print").click(function () {
        clickedPrint();
    })
    $("#to-excel").click(function () {
        toExcel();
    })
    function bindLevel(container, group, prop) {
        if (isValidArray(group)) {
            let __ul = $(`<ul class='menu-item-list'></ul>`);
            if (__glAccountData[0].AnnualReport) {
                group.forEach(i => {
                    let _li = $("<li></li>");
                    let __div = $(`<div class='flexAn'></div>`);
                    let __divTotal = $(`<div class='totalAccount flexAn'></div>`);
                    let $treeview = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up flexGl'>${i.Code} - ${i.Name}</span>`);
                    let spanAmount = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up'></span>`);

                    if (i.IsActive) {
                        spanAmount = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up text-right total' data-sum='${i.Balance}'>${i.CurrencyName} ${curFormat(i.Balance)}</span>`)
                    }

                    if (i.IsActive === false && i.IsTitle === true) {
                        $treeview.addClass("text-primary");
                    }
                    __div.append($treeview).append(spanAmount)
                    _li.append(__div).append(__divTotal);
                    __ul.append(_li);

                    bindLevel(_li, getItemByGroup(__glAccountData[0][prop], i.ID), prop);
                    $(container).append(__ul);

                    if (!i.IsActive) {
                        _li.append(__divTotal.append(`<span class="pm">Total ${i.Code}-${i.Name} : </span> <span class='dotBorder'>${i.CurrencyName} ${curFormat(i.Balance)}</span>`));
                    }
                })
            }
            if (__glAccountData[0].QuarterlyReport) {
                group.forEach(i => {
                    let _li = $("<li></li>");
                    let __div = $(`<div class='menu-item flex-container'></div>`);
                    let __divTotal = $(`<div class='totalAccount'></div>`);
                    let $treeview = $(`<span class='font-size flexQauater1'>${i.Code} - ${i.Name}</span>`);
                    let q1 = "", q2 = "", q3 = "", q4 = "";

                    if (i.IsActive) {
                        q1 = $(`<span class='font-size flexQauater2'>${i.CurrencyName} ${curFormat(i.Q1)}</span>`);
                        q2 = $(`<span class='font-size flexQauater2'>${i.CurrencyName} ${curFormat(i.Q2)}</span>`);
                        q3 = $(`<span class='font-size flexQauater2'>${i.CurrencyName} ${curFormat(i.Q3)}</span>`);
                        q4 = $(`<span class='font-size flexQauater2'>${i.CurrencyName} ${curFormat(i.Q4)}</span>`);
                    }

                    if (i.IsActive === false && i.IsTitle === true) {
                        $treeview.addClass("text-primary");
                    }
                    __div.append($treeview).append(q1).append(q2).append(q3).append(q4)
                    _li.append(__div).append(__divTotal);
                    __ul.append(_li);

                    bindLevel(_li, getItemByGroup(__glAccountData[0][prop], i.ID), prop);
                    $(container).append(__ul);

                    if (!i.IsActive) {
                        _li.append(__divTotal.append(`
                            <span class="pm flexQauaterTotal1">Total ${i.Code}-${i.Name} : </span> 
                            <span class='dotBorder flexQauaterTotal2'>${i.CurrencyName} ${curFormat(i.Q1)}</span>
                            <span class='dotBorder flexQauaterTotal2'>${i.CurrencyName} ${curFormat(i.Q2)}</span>
                            <span class='dotBorder flexQauaterTotal2'>${i.CurrencyName} ${curFormat(i.Q3)}</span>
                            <span class='dotBorder flexQauaterTotal2'>${i.CurrencyName} ${curFormat(i.Q4)}</span>
                        `));
                    }
                })
            }

            if (__glAccountData[0].MonthlyReport) {
                group.forEach(i => {
                    let row = $(`<div class='row'></div>`);
                    let col3 = $(`<div class='col-md-4' ></div>`);
                    let col9 = $(`<div class='col-md-10' style="margin-left: -40px;"></div>`);
                    let row_col9 = $(`<div class='row dpCol1'></div>`);
                    let _li = $("<li></li>");
                    let __divTotal = $(`<div class='row'></div>`);
                    let $treeview = $(`<span class='text-nowrap font-size'>${i.Code} - ${i.Name}</span>`);
                    let m1 = "", m2 = "", m3 = "", m4 = "", m5 = "", m6 = "",
                        m7 = "", m8 = "", m9 = "", m10 = "", m11 = "", m12 = "";

                    if (i.IsActive) {
                        m1 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M1)}</span></div>`);
                        m2 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M2)}</span></div>`);
                        m3 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M3)}</span></div>`);
                        m4 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M4)}</span></div>`);
                        m5 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M5)}</span></div>`);
                        m6 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M6)}</span></div>`);
                        m7 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M7)}</span></div>`);
                        m8 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M8)}</span></div>`);
                        m9 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M9)}</span></div>`);
                        m10 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M10)}</span></div>`);
                        m11 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M11)}</span></div>`);
                        m12 = $(`<div class='col-md-2'><span class='font-size'>${i.CurrencyName} ${curFormat(i.M12)}</span></div>`);
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

                    bindLevel(_li, getItemByGroup(__glAccountData[0][prop], i.ID), prop);
                    $(container).append(__ul);

                    if (!i.IsActive) {
                        let _col3 = $(`<div class='col-md-4'></div>`);
                        let _col9 = $(`<div class='col-md-10' style="margin-left: -40px;"></div>`);
                        let _row_col9 = $(`<div class='row dpCol1'></div>`);
                        _col3.append(`<span class="pm text-nowrap">Total ${i.Code}-${i.Name} : </span> `);
                        _row_col9.append(`                            
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M1)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M2)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M3)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M4)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M5)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M6)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M7)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M8)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M9)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M10)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M11)}</span></div>
                            <div class='col-md-2' ><span class='font-size dotBorder pd_2'>${i.CurrencyName} ${curFormat(i.M12)}</span></div>
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
    function bindGlaActiveOnly(container, data) {
        if (isValidArray(data)) {
            data.forEach(i => {
                let __li = $("<li class='ml-5 glaActive' ></li>");
                let __div = $(`<div class='menu-item flex-container'></div>`);
                let $treeview = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up'>${i.Code} - ${i.Name}</span>`);
                let spanAmount = $(`<span class='menu-title-group menu-title menu-dropdown-arrow-up text-right'>${i.CurrencyName} ${curFormat(i.AccountBalance.CumulativeBalance)}</span>`)
                __div.append($treeview).append(spanAmount);
                __li.append(__div);
                $(container).append(__li);
            })
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
            let $treeview = $(`<span class='text-danger font-size'>${res.Name}</span>`);
            let spanAmount = $(`<span class='font-size'></span>`);
            __div.append($treeview).append(spanAmount)
            _li.append(__div).append(__divTotal);
            __ul.append(_li);
            if (__glAccountData[0].AnnualReport) {
                $(`${container}-AnnualReport ul`).remove();
                $(`${container}-AnnualReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(__glAccountData[0][prop], res.ID), prop);
            }
            if (__glAccountData[0].QuarterlyReport) {
                $(`${container}-QuarterlyReport ul`).remove();
                $(`${container}-QuarterlyReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(__glAccountData[0][prop], res.ID), prop);
            }
            if (__glAccountData[0].MonthlyReport) {
                $(`${container}-MonthlyReport ul`).remove();
                $(`${container}-MonthlyReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(__glAccountData[0][prop], res.ID), prop);
            }
            if (__glAccountData[0].PeriodicReport) {
                $(`${container}-PeriodicReport ul`).remove();
                $(`${container}-PeriodicReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(__glAccountData[0][prop], res.ID), prop);
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
    function onGetPostingPeriodTemplates(success) {
        $.get("/FinancialReports/GetPostingPeriods", success);
    }
    function GetGLAccounts(date, TypeDisplayReport, showZeroAcc, success) {
        $.get("/FinancialReports/GetBalanceSheet", { date, TypeDisplayReport, showZeroAcc }, success);
    }
    function GetGLAccountsActiveOnly(date, success) {
        $.get("/FinancialReports/GetGLAccountsActiveOnly", { date }, success);
    }
    function GetGategories(code, success) {
        $.get("/FinancialReports/GetGategories", { code }, success);
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
        return value.toFixed(bitformatNumer).replace(/\d(?=(\d{3})+\.)/g, '$&,');
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
    function clickedPrint() {
        var date = $("#to-date").val();
        const typeDisplay = parseInt($("#d-report-select").val());
        const showZeroAcc = $("#checkAccZero").is(":checked") ? "true" : "false";
        window.open("/pdf/printbalancesheet?date=" + date + "&typeDisplay=" + typeDisplay + "&showZeroAcc=" + showZeroAcc, "_blank");
    }
    function toExcel() {
        var workbook = generateFile();
        workbook.xlsx.writeBuffer()
            .then(buffer => saveAs(new Blob([buffer]), `BalanceSheet_${Date.now()}.xlsx`))
            .then(function () {
                console.info('Done Writing file');
            })
            .catch(err => console.error('Error writing excel export', err));

        __dataExcel = {
            assets: [],
            liabilities: [],
            capitalReserves: []
        };
    }

    function generateFile() {
        var workbook = new ExcelJS.Workbook();

        workbook.creator = "Khmer EDI";
        workbook.lastModifiedBy = "Khmer EDI";
        workbook.created = new Date();
        workbook.modified = new Date();
        const wsTitle = "Balance Sheet";
        var worksheet = workbook.addWorksheet('Balance Sheet', { properties: { tabColor: { argb: 'FFC0000' } } });

        if (__glAccountData[0].AnnualReport) {
            worksheet.mergeCells('A1', 'B3');
            worksheet.getCell('A1').value = wsTitle;
            worksheet.getCell('A1').font = {
                name: 'Arial Black',
                family: 4,
                color: { argb: '26de81' },
                size: 25,
                bold: true
            };
            worksheet.getCell('A1').alignment = {
                vertical: "middle", horizontal: "center"
            }
        } else if (__glAccountData[0].QuarterlyReport) {
            worksheet.mergeCells('B1', 'C3');
            worksheet.getCell('B1').value = wsTitle;
            worksheet.getCell('B1').font = {
                name: 'Arial Black',
                family: 4,
                color: { argb: '26de81' },
                size: 25,
                bold: true
            };
            worksheet.getCell('B1').alignment = {
                vertical: "middle", horizontal: "center"
            }
        } else if (__glAccountData[0].MonthlyReport) {
            worksheet.mergeCells('C1', 'F3');
            worksheet.getCell('C1').value = wsTitle;
            worksheet.getCell('C1').font = {
                name: 'Arial Black',
                family: 4,
                color: { argb: '26de81' },
                size: 25,
                bold: true
            };
            worksheet.getCell('C1').alignment = {
                vertical: "middle", horizontal: "center"
            }
        }


        //Set Column Headers
        worksheet.getRow(6).values = setHeaderExcel();
        worksheet.columns = setCell();


        worksheet.getRow(6).eachCell({ includeEmpty: true }, function (cell, colNumber) {
            worksheet.getCell(cell._address).font = { bold: true };
        });

        bindDataForExcel(__glAccountData[0]["EAssets"][0][0].ID, "EAssets", "assets")
        __dataExcel.assets.forEach(asset => {
            addRowToExcel(asset, worksheet);
        });

        bindBlankData(worksheet);
        bindBlankData(worksheet);
        bindDataForExcel(__glAccountData[0]["ELiabilities"][0][0].ID, "ELiabilities", "liabilities")
        __dataExcel.liabilities.forEach(asset => {
            addRowToExcel(asset, worksheet);
        });

        bindBlankData(worksheet);
        bindBlankData(worksheet);
        bindDataForExcel(__glAccountData[0]["ECapitalandReserves"][0][0].ID, "ECapitalandReserves", "capitalReserves")
        worksheet.addRow({
            Balance: "",
            Level: 0,
            Name: "Capital & Reserves",
            ParentId: 0
        });
        __dataExcel.capitalReserves.forEach(asset => {
            addRowToExcel(asset, worksheet);
        })
        return workbook;
    }

    function setHeaderExcel() {
        if (__glAccountData[0].AnnualReport) {
            return ['Name', 'Balance']
        } else if (__glAccountData[0].QuarterlyReport) {
            return ['Name', '1st Quarter', "2st Quarter", "3st Quarter", "4st Quarter"]
        } else if (__glAccountData[0].MonthlyReport) {
            return ['Name', 'January', "February", "March", "April", 'May', "June", "July", "August", 'September', "October", "November", "December"]
        }
    }
    function setCell() {
        if (__glAccountData[0].AnnualReport) {
            return [
                { key: 'Name', width: 50},
                { key: 'Balance', width: 20 },
            ]
        } else if (__glAccountData[0].QuarterlyReport) {
            return [
                { key: 'Name', width: 50 },
                { key: 'Q1', width: 20 },
                { key: 'Q2', width: 20 },
                { key: 'Q3', width: 20 },
                { key: 'Q4', width: 20 },
            ]
        } else if (__glAccountData[0].MonthlyReport) {
            return [
                { key: 'Name', width: 50 },
                { key: 'M1', width: 20 },
                { key: 'M2', width: 20 },
                { key: 'M3', width: 20 },
                { key: 'M4', width: 20 },
                { key: 'M5', width: 20 },
                { key: 'M6', width: 20 },
                { key: 'M7', width: 20 },
                { key: 'M8', width: 20 },
                { key: 'M9', width: 20 },
                { key: 'M10', width: 20 },
                { key: 'M11', width: 20 },
                { key: 'M12', width: 20 },
            ]
        }
    }

    function bindBlankData(worksheet) {
        worksheet.addRow({
            Balance: "",
            Level: 0,
            Name: "",
            ParentId: 1
        });
    }

    function addRowToExcel(asset, worksheet) {
        const space = "    ";
        asset.Balance = asset.Balance === 0 ? "" : `${asset.CurrencyName} ${currencyFormat(asset.Balance)}`;
        for (let i = 1; i <= 4; i++) {
            asset[`Q${i}`] = asset[`Q${i}`] === 0 ? "" : `${asset.CurrencyName} ${currencyFormat(asset[`Q${i}`])}`;
        }
        for (let i = 1; i <= 12; i++) {
            asset[`M${i}`] = asset[`M${i}`] === 0 ? "" : `${asset.CurrencyName} ${currencyFormat(asset[`M${i}`])}`;
        }
        
        if (asset.Level === 1) {
            worksheet.lastRow.outlineLevel = 0;
            asset.Name = `${space}${asset.Name}`;
        }
        else if (asset.Level === 2) {
            worksheet.lastRow.outlineLevel = 1;
            asset.Name = `${space}${space}${asset.Name}`;
        }
        else if (asset.Level === 3) {
            worksheet.lastRow.outlineLevel = 2;
            asset.Name = `${space}${space}${space}${asset.Name}`;
        }
        else if (asset.Level === 4) {
            worksheet.lastRow.outlineLevel = 3;
            asset.Name = `${space}${space}${space}${space}${asset.Name}`;
        }
        else if (asset.Level === 5) {
            worksheet.lastRow.outlineLevel = 4;
            asset.Name = `${space}${space}${space}${space}${space}${asset.Name}`;
            //worksheet.lastRow.hidden = true;
        }
        worksheet.addRow(asset);
    }

    function bindDataForExcel(id, prop, dataprop) {
        __glAccountData[0][prop].forEach((value, index) => {
            if (index > 0) {
                value.forEach(j => {
                    if (j.ParentId === id) {
                        __dataExcel[dataprop].push(j);
                        bindDataForExcel(j.ID, prop, dataprop)
                    }
                })
            }
        })
    }

    function currencyFormat(value) {
        return parseFloat(value).toFixed(bitformatNumer).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
});