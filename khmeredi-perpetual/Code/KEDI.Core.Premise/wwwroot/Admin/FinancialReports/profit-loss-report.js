let __glAccountData = [];

let MEMSUM = 0;
$(document).ready(function () {
    let __dataExcelPL = {
        turnovers: [],
        costofSales: [],
        operatingCosts: [],
        nonOperatingIncomeExpenditure: [],
        taxationExtraordinaryItems: []
    }
    var arr_branch=[];
    var _branch_submit=[];
    var formatNumber = 2;
    let cur = "";
    let sumq1 = 0;
    let sumq2 = 0;
    let sumq3 = 0;
    let sumq4 = 0;
    let sumLv2 = [];
    let sumM1 = 0, sumM2 = 0, sumM3 = 0, sumM4 = 0, sumM5 = 0, sumM6 = 0,
        sumM7 = 0, sumM8 = 0, sumM9 = 0, sumM10 = 0, sumM11 = 0, sumM12 = 0;
    $("#d-report-select").change(function () {

        if (this.value == 2) {
            $("#print").prop("hidden", true);
        } else {
            $("#print").prop("hidden", false);
        }
    });
    $("#branch").click(()=>{
        DialogBranch();
    });

    $(".from-sh-posting-period").click(function () {
        postingPeriodToFrom(true);
    });
    $(".to-sh-posting-period").click(function () {
        postingPeriodToFrom();
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

    // Click Ok
    $("#btn-ok").click(function () {
        const dateFrom = $("#from-date").val();
        const dateTo = $("#to-date").val();
        const typeDisplay = parseInt($("#d-report-select").val());
        const showZeroAcc = $("#checkAccZero").is(":checked") ? "true" : "false";
        _branch_submit=[];
        arr_branch.forEach(i=>{if(i.Active==true)_branch_submit.push(i)} );
        
        let branch = _branch_submit;

        GetGLAccountsActiveOnlyPL(dateFrom, dateTo, branch, function (gla) {
            formatNumber = gla.FormatNumber;
            bindGlaActiveOnlyPL("#active-only", gla);
            $("#to-excel").prop("disabled", false);
            $("#print").prop("disabled", false);
        })
        //MEMSUM = 0;
        $("#show-hide-loader").prop("hidden", false);
        $("#content").prop("hidden", true);
        GetGLAccountsPL(dateFrom, dateTo, typeDisplay, showZeroAcc, branch, function (gla) {
            formatNumber = gla.FormatNumber;
            cur = gla["ETurnovers"][0][0].CurrencyName
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
            setTimeout(function () {
                GetGategoriesPL("400000000000000", function (res) {
                    formatNumber = res.FormatNumber;
                    bindGategotiesPL("turnover", "Total Turnover", "#turnover", res, "Turnover");
                });
            }, 0);

            setTimeout(function () {
                GetGategoriesPL("500000000000000", function (res) {
                    formatNumber = res.FormatNumber;
                    bindGategotiesPL("gross", "Gross Profit", "#cost-of-sales", res, "CostOfSales");
                });
            }, 100);

            setTimeout(function () {
                GetGategoriesPL("600000000000000", function (res) {
                    formatNumber = res.FormatNumber;
                    bindGategotiesPL("operating", "Operating Profit", "#operating-costs", res, "OperatingCosts");
                });
            }, 200);

            setTimeout(function () {
                GetGategoriesPL("700000000000000", function (res) {
                    formatNumber = res.FormatNumber;
                    bindGategotiesPL("peofit", "Profit After Financing Expenses", "#non-operating-income-expenditure", res, "NonOperatingIncomeExpenditure");
                });
            }, 300);

            setTimeout(function () {
                GetGategoriesPL("800000000000000", function (res) {
                    formatNumber = res.FormatNumber;
                    bindGategotiesPL("total", "Total Taxation and Extraordinary Items", "#taxation-extraordinary-items", res, "TaxationExtraordinaryItems");
                });
            }, 400);
            $("#show-hide-loader").prop("hidden", true);
            $("#content").prop("hidden", false);
            MEMSUM = 0;
        });
    })
    function DialogBranch() 
    {
        let dialog = new DialogBox({
            caption: "List All Branch " ,
           
        type: "ok",
            button: {
                ok: {
                    text: "OK",
                    // callback: function () {
                    //     this.meta.shutdown();
                    //}
                },
              
            },
            content: {
                selector: "#content-branch"
            },
          
        });

        dialog.invoke(function () {
            $("#txtseaerch_branch").on("keyup", function (e) {
                var value = $(this).val().toLowerCase();
                $("#tb_branch tr:not(:first-child)").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
             $list_branch = ViewTable({
                keyField: "ID",
                selector: "#tb_branch",
                indexed:true,
               
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Name", "Location","Address","Active"],
                columns:[
                    {
                        name: "Active",
                        template: "<input type='checkbox'>",
                        on: {
                            "click": function (e) {
                                let active=$(this).prop("checked")?true:false;
                                $list_branch.updateColumn(e.key, "Active",active);
                                if(arr_branch.length>0)
                                {
                                    arr_branch.forEach(i=>{
                                        if(parseInt(i.ID)==parseInt(e.data.ID))
                                            i.Active==active;
                                    });
                                }
                              
                            }
                        }
                    },
                ],
                
            });
            $list_branch.bindRows(arr_branch);
           
            dialog.confirm(function () {
                let text="";
                let all=true;
                 if($list_branch.yield().length>0)
                 {
                     $list_branch.yield().forEach(i=>{
                         if(i.Active)
                         {
                             text+= i.Name+",  "
                         }
                         else
                         {
                            all=false;
                         }
                         
                     });
                     text=all==true?"All Branch":text;
                     $("#branch").val(text);
                 }
     
                 dialog.shutdown();
             });
        });
        
       
    }
    $.get("/FinancialReports/GetBranch", function (res) {
      
        if(res.length>0)
        {
            res.forEach(i=>{
                arr_branch.push(i);
            });
            CurrentUserLogin=res[0].UserName;
            $("#branch").val("All Branch");
        }
       
       
    });
    function postingPeriodToFrom(isFrom = false) {
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
                                if (isFrom) {
                                    $("#from-date").val(formatDate(e.data.PostingDateFrom, "YYYY-MM-DD"));
                                    $(".to-sh-posting-period").css("pointer-events", "all");
                                } else {
                                    $("#to-date").val(formatDate(e.data.PostingDateTo, "YYYY-MM-DD"));
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
    function bindLevel(container, group, prop) {

        if (isValidArray(group)) {
            let __ul = $(`<ul class='menu-item-list'></ul>`);
            if (__glAccountData[0].AnnualReport) {
                group.forEach(i => {
                    let _li = $("<li></li>");
                    let __div = $(`<div class='menu-item flex-container'></div>`);
                    let __divTotal = $(`<div class='totalAccount tagborder'></div>`);
                    let $treeview = $(`<span class='font-size ' style='padding:4px'>${i.Code} - ${i.Name}</span>`);
                    let spanAmount = '';

                    if (i.IsActive) {
                        spanAmount = $(`
                            <span class='font-size totalAccount2' style='padding:4px' data-sum='${i.Balance}'>
                            ${i.CurrencyName} ${curFormat(i.Balance)}
                            </span>`);
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

                    bindLevel(_li, getItemByGroup(__glAccountData[0][prop], i.ID), prop);
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
            if (__glAccountData[0].MonthlyReport) {
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

                    bindLevel(_li, getItemByGroup(__glAccountData[0][prop], i.ID), prop);
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

    function bindGlaActiveOnlyPL(container, data) {

        $(`${container} li`).remove();
        if (isValidArray(data)) {
            data.forEach(i => {
                let __li = $("<li class='ml-5 glaActive' ></li>");
                let __div = $(`<div class='flex-container'></div>`);
                let $treeview = $(`<span class='font-size'>${i.Code} - ${i.Name}</span>`);
                let spanAmount = $(`
                    <span class='font-size text-right' style='color:red;'>
                        ${i.CurrencyName} ${curFormat(i.AccountBalance.CumulativeBalance)}
                    </span>`)

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
    function bindGategotiesPL(textcom, text, container, res, prop, sumData = null) {
        $(`${container} ul`).remove();
        const dataSum = __glAccountData[0][prop][__glAccountData[0][prop].length - 1].AccountBalances;
        let sum_one_acc = 0;
        if (isValidJSON(res)) {
            let __ul = $(`<ul class='mainacc'></ul>`);
            let _li = $("<li></li>");
            let __div = $(`<div class='flex-container'></div>`);
            let __divTotal = $(`<div class='totalAccount'></div>`);
            let $treeview = $(`<span class='font-size text-danger'>${res.Name}</span>`);
            let spanAmount = $(`<span class='font-size font-size'></span>`);
            __div.append($treeview).append(spanAmount)
            _li.append(__div).append(__divTotal);
            __ul.append(_li);
            if (__glAccountData[0].AnnualReport) {
                $(`${container}-AnnualReport ul`).remove();
                $(`${container}-AnnualReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(__glAccountData[0][prop], res.ID), prop);
                //MEMSUM = 0;
                if (textcom == "turnover") {
                    if (dataSum.length > 0) {

                        dataSum.forEach(i => {
                            sum_one_acc += ((i.Debit - i.Credit) * (-1));
                            MEMSUM = sum_one_acc;
                        })
                    }
                }
                else {
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
            if (__glAccountData[0].QuarterlyReport) {

                $(`${container}-QuarterlyReport ul`).remove();
                $(`${container}-QuarterlyReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(__glAccountData[0][prop], res.ID), prop);
                if (textcom == "turnover") {
                    __glAccountData[0][prop].forEach(i => {
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
            if (__glAccountData[0].MonthlyReport) {
                sumLv2 = [];

                $(`${container}-MonthlyReport ul`).remove();
                $(`${container}-MonthlyReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(__glAccountData[0][prop], res.ID), prop);
                if (textcom == "turnover") {
                    __glAccountData[0][prop].forEach(i => {
                        if (i.Level === 2)
                            sumLv2.push(i);
                    });
                    if (isValidArray(sumLv2)) {
                        sumM1 = sumM2 = sumM3 = sumM4 = sumM5 = sumM6 = sumM7 = sumM8 = sumM9 = sumM10 = sumM11 = sumM12 = 0;
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
            if (__glAccountData[0].PeriodicReport) {
                $(`${container}-PeriodicReport ul`).remove();
                $(`${container}-PeriodicReport`).append(__ul);
                bindLevel(__ul, getItemByGroup(__glAccountData[0][prop], res.ID), prop);
            }

        }
    }

    function sumPL(prop, idDataAttr, textcom, curname) {
        sumLv2 = [];
        let sumq1 = 0;
        let sumq2 = 0;
        let sumq3 = 0;
        let sumq4 = 0;
        __glAccountData[0][prop].forEach(i => {
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
    function sumPLM(prop, idDataAttr, textcom, curname) {
        sumLv2 = [];
        let sumM1 = 0, sumM2 = 0, sumM3 = 0, sumM4 = 0, sumM5 = 0, sumM6 = 0,
            sumM7 = 0, sumM8 = 0, sumM9 = 0, sumM10 = 0, sumM11 = 0, sumM12 = 0;
        __glAccountData[0][prop].forEach(i => {
            if (i.Level === 2)
                sumLv2.push(i);
        });
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

        const SumM1 = sumM1 + $(`${idDataAttr}1`).data("sum");
        const SumM2 = sumM2 + $(`${idDataAttr}2`).data("sum");
        const SumM3 = sumM3 + $(`${idDataAttr}3`).data("sum");
        const SumM4 = sumM4 + $(`${idDataAttr}4`).data("sum");
        const SumM5 = sumM5 + $(`${idDataAttr}5`).data("sum");
        const SumM6 = sumM6 + $(`${idDataAttr}6`).data("sum");
        const SumM7 = sumM7 + $(`${idDataAttr}7`).data("sum");
        const SumM8 = sumM8 + $(`${idDataAttr}8`).data("sum");
        const SumM9 = sumM9 + $(`${idDataAttr}9`).data("sum");
        const SumM10 = sumM10 + $(`${idDataAttr}10`).data("sum");
        const SumM11 = sumM11 + $(`${idDataAttr}11`).data("sum");
        const SumM12 = sumM12 + $(`${idDataAttr}12`).data("sum");


        $(`#${textcom}1`).text(`${curname} ${curFormat(SumM1)}`).data("sum", SumM1);
        $(`#${textcom}2`).text(`${curname} ${curFormat(SumM2)}`).data("sum", SumM2);
        $(`#${textcom}3`).text(`${curname} ${curFormat(SumM3)}`).data("sum", SumM3);
        $(`#${textcom}4`).text(`${curname} ${curFormat(SumM4)}`).data("sum", SumM4);
        $(`#${textcom}5`).text(`${curname} ${curFormat(SumM5)}`).data("sum", SumM5);
        $(`#${textcom}6`).text(`${curname} ${curFormat(SumM6)}`).data("sum", SumM6);
        $(`#${textcom}7`).text(`${curname} ${curFormat(SumM7)}`).data("sum", SumM7);
        $(`#${textcom}8`).text(`${curname} ${curFormat(SumM8)}`).data("sum", SumM8);
        $(`#${textcom}9`).text(`${curname} ${curFormat(SumM9)}`).data("sum", SumM9);
        $(`#${textcom}10`).text(`${curname} ${curFormat(SumM10)}`).data("sum", SumM10);
        $(`#${textcom}11`).text(`${curname} ${curFormat(SumM11)}`).data("sum", SumM11);
        $(`#${textcom}12`).text(`${curname} ${curFormat(SumM12)}`).data("sum", SumM12);
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

    function GetGLAccountsPL(fromDate, toDate, typeDisplay, showZeroAcc, branch, success) {
        $.get("/FinancialReports/GetGLAccountsPL", { fromDate, toDate, typeDisplay, showZeroAcc, branchs:JSON.stringify(branch) }, success);
    }

    function GetGLAccountsActiveOnlyPL(fromDate, toDate, branch, success) {
        $.get("/FinancialReports/GetGLAccountsActiveOnlyPL", { fromDate, toDate, branchs:JSON.stringify(branch) }, success);
    }

    function GetGategoriesPL(code, success) {
        $.get("/FinancialReports/GetGategoriesPL", { code }, success);
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
        var fromDate = $("#from-date").val().split("T")[0];
        var toDate = $("#to-date").val().split("T")[0];
        var typeDisplay = parseInt($("#d-report-select").val());
        var checkAccZero = $("#checkAccZero").is(":checked") ? "true" : "false";
        _branch_submit=[];
        arr_branch.forEach(i=>{if(i.Active==true)_branch_submit.push(i)} );
        var branch =_branch_submit; 
        window.open("/pdf/printprofitandloss?fromDate=" + fromDate + "&toDate=" + toDate + "&typeDisplay=" + typeDisplay + "&showZeroAcc=" + checkAccZero +  "&branchs=" + JSON.stringify(branch)+"&user="+CurrentUserLogin, "_blank");
    }
    function toExcel() {
        var workbook = generateFile();
        workbook.xlsx.writeBuffer()
            .then(buffer => saveAs(new Blob([buffer]), `ProfitAndLoss_${Date.now()}.xlsx`))
            .then(function () {
                console.info('Done Writing file');
            })
            .catch(err => console.error('Error writing excel export', err));

        __dataExcelPL = {
            turnovers: [],
            costofSales: [],
            operatingCosts: [],
            nonOperatingIncomeExpenditure: [],
            taxationExtraordinaryItems: []
        };
    }
    let _MEMSUM = 0;
    const sumQ = {
        sumq1: 0,
        sumq2: 0,
        sumq3: 0,
        sumq4: 0,
    }
    const sumM = {
        sumM1: 0,
        sumM2: 0,
        sumM3: 0,
        sumM4: 0,
        sumM5: 0,
        sumM6: 0,
        sumM7: 0,
        sumM8: 0,
        sumM9: 0,
        sumM10: 0,
        sumM11: 0,
        sumM12: 0,
    }
    function sumGategory(prop, name, worksheet, textcom = "") {
        let _sumLv2 = [];

        let sum_one_acc = 0;
        const dataSum = __glAccountData[0][prop][__glAccountData[0][prop].length - 1].AccountBalances;

        if (__glAccountData[0].AnnualReport) {
            //MEMSUM = 0;
            if (textcom == "turnover") {
                if (dataSum.length > 0) {

                    dataSum.forEach(i => {
                        sum_one_acc += ((i.Debit - i.Credit) * (-1));
                        _MEMSUM = sum_one_acc;
                    })
                }
            } else {
                if (dataSum.length > 0) {
                    dataSum.forEach(i => {
                        _MEMSUM += ((i.Debit - i.Credit) * (-1));
                    })
                }
            }
        }
        if (__glAccountData[0].QuarterlyReport) {
            __glAccountData[0][prop].forEach(i => {
                if (i.Level === 2)
                    _sumLv2.push(i);
            });
            if (textcom == "turnover") {
                _sumLv2.forEach(j => {
                    for (let i = 1; i <= 4; i++) {
                        sumQ[`sumq${i}`] = j[`Q${i}`]
                    }
                });
            } else {
                _sumLv2.forEach(j => {
                    for (let i = 1; i <= 4; i++) {
                        sumQ[`sumq${i}`] += j[`Q${i}`];
                    }
                });
            }
        }
        if (__glAccountData[0].MonthlyReport) {
            _sumLv2 = [];
            __glAccountData[0][prop].forEach(i => {
                if (i.Level === 2)
                    _sumLv2.push(i);
            });
            if (textcom == "turnover") {
                if (isValidArray(_sumLv2)) {
                    _sumLv2.forEach(j => {
                        for (let i = 1; i <= 12; i++) {
                            sumM[`sumM${i}`] = j[`M${i}`]
                        }
                    });
                }
            } else {
                if (isValidArray(_sumLv2)) {
                    _sumLv2.forEach(j => {
                        for (let i = 1; i <= 12; i++) {
                            sumM[`sumM${i}`] += j[`M${i}`]
                        }
                    });
                }
            }
        }
        worksheet.addRow({
            Balance: `${cur} ${curFormat(_MEMSUM)}`,
            Level: 0,
            Name: name,
            ParentId: -1,
            Q1: `${cur} ${curFormat(sumQ.sumq1)}`,
            Q2: `${cur} ${curFormat(sumQ.sumq2)}`,
            Q3: `${cur} ${curFormat(sumQ.sumq3)}`,
            Q4: `${cur} ${curFormat(sumQ.sumq4)}`,
            M1: `${cur} ${curFormat(sumM.sumM1)}`,
            M2: `${cur} ${curFormat(sumM.sumM2)}`,
            M3: `${cur} ${curFormat(sumM.sumM3)}`,
            M4: `${cur} ${curFormat(sumM.sumM4)}`,
            M5: `${cur} ${curFormat(sumM.sumM5)}`,
            M6: `${cur} ${curFormat(sumM.sumM6)}`,
            M7: `${cur} ${curFormat(sumM.sumM7)}`,
            M8: `${cur} ${curFormat(sumM.sumM8)}`,
            M9: `${cur} ${curFormat(sumM.sumM9)}`,
            M10: `${cur} ${curFormat(sumM.sumM10)}`,
            M11: `${cur} ${curFormat(sumM.sumM11)}`,
            M12: `${cur} ${curFormat(sumM.sumM12)}`,
        });
    }


    function generateFile() {
        var workbook = new ExcelJS.Workbook();

        workbook.creator = "Khmer EDI";
        workbook.lastModifiedBy = "Khmer EDI";
        workbook.created = new Date();
        workbook.modified = new Date();
        const wsTitle = "Profit And Loss";
        var worksheet = workbook.addWorksheet(wsTitle, { properties: { tabColor: { argb: 'FFC0000' } } });

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
            worksheet.mergeCells('B1', 'F3');
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
        }


        //Set Column Headers
        worksheet.getRow(6).values = setHeaderExcel();
        worksheet.columns = setCell();


        worksheet.getRow(6).eachCell({ includeEmpty: true }, function (cell, colNumber) {
            worksheet.getCell(cell._address).font = { bold: true };
        });
        worksheet.addRow({
            Balance: "",
            Level: 0,
            Name: "Turnover",
            ParentId: 0
        });
        __dataExcelPL.turnovers.push(__glAccountData[0]["ETurnovers"][0][0]);
        bindDataForExcel(__glAccountData[0]["ETurnovers"][0][0].ID, "ETurnovers", "turnovers")
        __dataExcelPL.turnovers.forEach(asset => {
            addRowToExcel(asset, worksheet);
        });
        sumGategory("Turnover", "Total Turnover", worksheet, "turnover");
        bindBlankData(worksheet);
        bindBlankData(worksheet);
        worksheet.addRow({
            Balance: "",
            Level: 0,
            Name: "Cost of Sales",
            ParentId: 0
        });
        __dataExcelPL.costofSales.push(__glAccountData[0]["ECostofSales"][0][0]);
        bindDataForExcel(__glAccountData[0]["ECostofSales"][0][0].ID, "ECostofSales", "costofSales")
        __dataExcelPL.costofSales.forEach(asset => {
            addRowToExcel(asset, worksheet);
        });
        sumGategory("CostOfSales", "Gross Profit", worksheet);
        bindBlankData(worksheet);
        bindBlankData(worksheet);
        //Operating Costs
        worksheet.addRow({
            Balance: "",
            Level: 0,
            Name: "Operating Costs",
            ParentId: 0
        });
        __dataExcelPL.operatingCosts.push(__glAccountData[0]["EOperatingCosts"][0][0]);
        bindDataForExcel(__glAccountData[0]["EOperatingCosts"][0][0].ID, "EOperatingCosts", "operatingCosts")

        __dataExcelPL.operatingCosts.forEach(asset => {
            addRowToExcel(asset, worksheet);
        })
        sumGategory("OperatingCosts", "Operating Profit", worksheet);

        bindBlankData(worksheet);
        bindBlankData(worksheet);
        worksheet.addRow({
            Balance: "",
            CurrencyName: "USD",
            Level: 0,
            Name: "Non-Operating Income & Expenditure",
            ParentId: 0
        });
        __dataExcelPL.nonOperatingIncomeExpenditure.push(__glAccountData[0]["ENonOperatingIncomeExpenditure"][0][0]);
        bindDataForExcel(__glAccountData[0]["ENonOperatingIncomeExpenditure"][0][0].ID, "ENonOperatingIncomeExpenditure", "nonOperatingIncomeExpenditure")

        __dataExcelPL.nonOperatingIncomeExpenditure.forEach(asset => {
            addRowToExcel(asset, worksheet);
        })
        sumGategory("NonOperatingIncomeExpenditure", "Profit After Financing Expenses", worksheet);


        bindBlankData(worksheet);
        bindBlankData(worksheet);

        worksheet.addRow({
            Balance: "",
            Level: 0,
            Name: "Taxation & Extraordinary Items",
            ParentId: 0
        });
        __dataExcelPL.taxationExtraordinaryItems.push(__glAccountData[0]["ETaxationExtraordinaryItems"][0][0]);
        bindDataForExcel(__glAccountData[0]["ETaxationExtraordinaryItems"][0][0].ID, "ETaxationExtraordinaryItems", "taxationExtraordinaryItems")
        __dataExcelPL.taxationExtraordinaryItems.forEach(asset => {
            addRowToExcel(asset, worksheet);
        })
        sumGategory("TaxationExtraordinaryItems", "Total Taxation and Extraordinary Items", worksheet);
        worksheet.eachRow(function (row, rowNumber) {
            if (row._number > 6) {
                worksheet.getRow(row._number).eachCell({ includeEmpty: true }, function (cell, colNumber) {
                    if (__glAccountData[0].QuarterlyReport) {
                        if (row.values[6] == -1) {
                            worksheet.getCell(cell._address).font = {
                                bold: true,
                                //color: { argb: '26de81' },
                            }
                        }

                    } else if (__glAccountData[0].MonthlyReport) {
                        if (row.values[14] == -1) {
                            worksheet.getCell(cell._address).font = {
                                bold: true,
                                //color: { argb: '26de81' },
                            }
                        }
                    }

                });
            }

        });
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
                { key: 'Name', width: 50 },
                { key: 'Balance', width: 20 },
                { key: 'ParentId', width: 20, hidden: true },
            ]
        } else if (__glAccountData[0].QuarterlyReport) {
            return [
                { key: 'Name', width: 50 },
                { key: 'Q1', width: 20 },
                { key: 'Q2', width: 20 },
                { key: 'Q3', width: 20 },
                { key: 'Q4', width: 20 },
                { key: 'ParentId', width: 20, hidden: true },
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
                { key: 'ParentId', width: 20, hidden: true },
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
        asset.Balance = asset.Balance === 0 ? "" : `${asset.CurrencyName} ${curFormat(asset.Balance)}`;
        for (let i = 1; i <= 4; i++) {
            asset[`Q${i}`] = asset[`Q${i}`] === 0 ? "" : `${asset.CurrencyName} ${curFormat(asset[`Q${i}`])}`;
        }
        for (let i = 1; i <= 12; i++) {
            asset[`M${i}`] = asset[`M${i}`] === 0 ? "" : `${asset.CurrencyName} ${curFormat(asset[`M${i}`])}`;
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
                        __dataExcelPL[dataprop].push(j);
                        bindDataForExcel(j.ID, prop, dataprop)
                    }
                })
            }
        })
    }

});