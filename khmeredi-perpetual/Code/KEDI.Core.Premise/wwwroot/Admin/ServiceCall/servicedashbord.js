
$(function () {
    let prioritypp = [];
    let priority = [];
    let originpp = [];
    let origin = [];
    let acountpp = [];
    let acount = [];
    let resolutionpp = [];
    let resolution = [];
    let closepp = [];
    let closevalue = [];
    var total = 0;
    var totalorigin = 0;
    $.get("/Service/CountChartPie", function (res) {
        datasopp(res)
    })
    $.get("/Service/CountChartPieOrigin", function (res) {

        if (res.length > 0) {
            res.forEach(i => {
                totalorigin += i.CountData;
                originpp = [{
                    country: i.ChannelName,
                    medals: i.CountData,
                },];
                originpp.forEach(s => {
                    origin.push(s);
                })

            })
        }
    })
    $.get("/Service/CountChartAcount", function (res) {

        res.forEach(i => {

            acountpp = [{
                country: i.ChannelName,
                medals: i.CountData,
            },];
            acountpp.forEach(s => {
                acount.push(s);
            })

        })

    })

    $.get("/Service/ClosebyAcount", function (res) {

        if (res.length > 0) {
            res.forEach(i => {
                closepp = [{
                    country: i.CallStatus,
                    medals: i.CountData,
                },];
                closepp.forEach(s => {
                    closevalue.push(s);
                })
            })
        }
    })
    $.get("/Service/AverageResolutiontAcount", function (res) {

        if (res.length > 0) {
            res.forEach(i => {
                i.Resolvedontime = parseFloat(i.Resolvedontime);
                resolutionpp = [{
                    country: i.ChannelName,
                    medals: i.Resolvedontime,
                },];
                resolutionpp.forEach(s => {
                    resolution.push(s);
                })

            })
        }

    })

    $.get("/Service/AVGCloseTime", function (res) {

        if (res.length > 0) {

            // $(".gauge-result").html(res[0].Resolvedondate);
            // let arr = res[0].Resolvedondate;
            // avg= arr.split(':'); 
            const gaugeElement = document.querySelector("#gauge");
            AVGCloseTime(gaugeElement, res[0].Resolvedondate, res[0].AvgResolutionTime / 100);
        }
        else {
            const gaugeElement = document.querySelector("#gauge");
            AVGCloseTime(gaugeElement, 0, 0);
        }
    })


    // start chart pie 1
    function datasopp(res) {
        res.forEach(i => {
            total += i.CountData;
            prioritypp = [{
                country: i.Priority,
                medals: i.CountData,
            },];
            prioritypp.forEach(s => {
                priority.push(s);
            })
        })
    }

    setTimeout(PiePriority, 2000);
    function PiePriority() {
        $('#pie').dxPieChart({
            palette: 'bright',
            dataSource: priority,
            type: 'doughnut',
            title: '',
            //margin: {
            //    top: 10,
            //},
            legend: {
                visible: true,
            },
            animation: {
                enabled: false,
            },

            export: {
                enabled: true,
            },
            series: [{
                argumentField: 'country',
                valueField: 'medals',
                label: {

                    visible: true,
                    position: 'inside',
                    radialOffset: 0,
                    backgroundColor: 'transparent',
                    //customizeText(arg) {
                    //    return `${arg.argumentText} (${arg.percentText})`;
                    //},
                },
            }],
            centerTemplate(pieChart, container) {


                const content = $(`<svg><circle cx="100" cy="100" fill="#eee" r="${pieChart.getInnerRadius() - 6}"></circle>`

                    + '<text text-anchor="middle" style="font-size: 18px" x="100" y="105" fill="#494949">'
                    /*  + `<tspan x="100" >${country}</tspan></text></svg>`);*/
                    + `<tspan x="100" >${total}</tspan></text></svg>`);

                container.appendChild(content.get(0));
            },
            //size: {
            //    height: 200,
            //    width: 350
            //},
        }).dxPieChart('instance');
    }
    setTimeout(PieOrigin, 2000);
    function PieOrigin() {
        $('#pie2').dxPieChart({
            palette: 'bright',
            dataSource: origin,
            type: 'doughnut',
            title: '',
            //margin: {
            //    top: 10,
            //},
            legend: {
                visible: true,
            },
            animation: {
                enabled: false,
            },

            export: {
                enabled: true,
            },
            series: [{
                argumentField: 'country',
                valueField: 'medals',
                label: {

                    visible: true,
                    position: 'inside',
                    radialOffset: 0,
                    backgroundColor: 'transparent',
                    //customizeText(arg) {
                    //    return `${arg.argumentText} (${arg.percentText})`;
                    //},
                },
            }],
            centerTemplate(pieChart, container) {


                const content = $(`<svg><circle cx="100" cy="100" fill="#eee" r="${pieChart.getInnerRadius()}"></circle>`

                    + '<text text-anchor="middle" style="font-size: 18px" x="100" y="105" fill="#494949">'
                    /*  + `<tspan x="100" >${country}</tspan></text></svg>`);*/
                    + `<tspan x="100" >${totalorigin}</tspan></text></svg>`);

                container.appendChild(content.get(0));
            },
            //size: {
            //    height: 200,
            //    width: 350
            //},
        }).dxPieChart('instance');
    }
    setTimeout(Acount, 2000);
    function Acount() {
        $('#chart').dxChart({
            rotated: false,
            palette: 'soft',
            title: ' <p class="font-titile">Closed Case This Year by Account</p>',
            margin: {
                button: 10,
            },
            dataSource: closevalue,
            series: {
                color: '#4db8ff',
                argumentField: 'country',
                valueField: 'medals',
                type: 'bar',
                label: {
                    visible: true,
                },
            },
            legend: {
                visible: false,
                position: "inside", // or "outside"
                horizontalAlignment: "center", // or "left" | "right"
                verticalAlignment: "top" // or "bottom"
            },

            argumentAxis:
            {
                grid: {
                    visible: false,
                },
                label: {
                    rotationAngle: -45,
                    overlappingBehavior: 'rotate',
                },

            },

        }).dxChart('instance');
    }

    setTimeout(CallStatus, 3000);
    function CallStatus() {
        $('#chart3').dxChart({
            rotated: false,
            palette: 'soft',
            title: ' <p class="font-titile">Open Cases by Account</p><br><p class="font-count">Record Count</p>',
            dataSource: acount,
            series: {
                color: '#e580ff',
                argumentField: 'country',
                valueField: 'medals',
                type: 'bar',
                label: {
                    visible: true,
                },
            },
            legend: {
                visible: false,
                position: "inside", // or "outside"
                horizontalAlignment: "center", // or "left" | "right"
                verticalAlignment: "top" // or "bottom"
            },

            argumentAxis: {
                grid: {
                    visible: true,
                },
                label: {
                    rotationAngle: -45,
                    overlappingBehavior: 'rotate',
                },
            },

        }).dxChart('instance');
    }


    setTimeout(AverageSolution, 3300);
    function AverageSolution() {
        $('#chart2').dxChart({
            rotated: true,
            palette: 'soft',

            title: '<p class="font-titile">Average Resolution Time this Year(Hour) by Account</p><br><p class="font-count">Record Count</p>',
            dataSource: resolution,
            series: {
                color: '#8c8cd9',
                argumentField: 'country',
                valueField: 'medals',
                type: 'bar',
                label: {
                    visible: true,
                },
            },

            legend: {
                visible: false,
                position: "inside", // or "outside"
                horizontalAlignment: "center", // or "left" | "right"
                verticalAlignment: "top" // or "bottom"
            },

            argumentAxis: {
                //argumentType: 'datetime',
                //tickInterval: 'hour',
                grid: {
                    visible: true,
                },
                label: {
                    rotationAngle: 150,
                    overlappingBehavior: 'rotate',
                },
            },

        }).dxChart('instance');
    }
    // setTimeout(AVGCloseTime, 4000);


    function AVGCloseTime(gauge, value, rotated) {

        if (rotated < 0 || rotated > 1) {
            return;
        }
        rotated = rotated == 0 ? 0 : rotated / 2;
        gauge.querySelector(".gauge__fill_only").style.transform = `rotate(${rotated}turn)`;
        gauge.querySelector(".gauge__cover_only").textContent = `${value}`;


        // $(() => {
        //     $('#gauge').dxCircularGauge({
        //         scale: {
        //             startValue: 0,
        //             endValue: 100,
        //             tickInterval: 10,
        //             label: {
        //                 customizeText(arg) {
        //                     return `${arg.valueText} `;
        //                 },
        //             },
        //         },
        //         rangeContainer: {
        //             ranges: [
        //                 { startValue: 0, endValue: 40, color: '#228B22' },
        //                 { startValue: 40, endValue: 80, color: '#FFD700' },
        //                 { startValue: 80, endValue: 100, color: '#CE2029' },
        //             ],
        //         },
        //         export: {
        //             enabled: true,
        //         },
        //         title: {
        //             text: '<p class="x">a</p>',
        //             font: { size: 28 },
        //         },
        //         value: avg[0] ,
        //     });
        // });
    }
    // block code added news
    const color = ['#0099e6', '#4dc3ff', '#80dfff'];

    $.get("/Service/ChartOfAccount", function (res) {

        let currency = res[0].Currency == null ? "" : res[0].Currency;
        $("#revenue").html(res[0].Revenue.toFixed(res[0].Format) + " " + currency);
        $("#cogs").html(res[0].COGS.toFixed(res[0].Format) + " " + currency);
        $("#grossProfit").html(res[0].GrossProfit.toFixed(res[0].Format) + " " + currency);
        $("#opex").html(res[0].Opex.toFixed(res[0].Format) + " " + currency);
        $("#operating").html(res[0].OperatingProfit.toFixed(res[0].Format) + " " + currency);
        $("#tax").html(res[0].InterestTax.toFixed(res[0].Format) + " " + currency);
        $("#netprofit").html(res[0].NetProfit.toFixed(res[0].Format) + " " + currency);
        let groupopex = "";
        res[0].OpexItem.forEach(i => {
            groupopex += `<div class="bold b-income"><p >${i.Name} </p> <span>${i.Amount.toFixed(res[0].Format) + " " + currency}</span></div>`;

        });
        var dashboadopex = "";
        const arr = [];
        var list = "";
        var index = 0;
        var j = 0;

        for (let i = 0; i < res[0].OpexItem.length; i++) {
            var obj = {
                Name: res[0].OpexItem[i].Name,
                Width: res[0].OpexItem[i].Amount == 0 ? 10 : res[0].OpexItem[i].Amount,
                Person: (res[0].OpexItem[i].Amount / res[0].Opex) * 100,
            };
            arr.push(obj);

            if (i == 2)
                break;
        }

        arr.sort((a, b) => b.Width - a.Width);
        arr.forEach(i => {

            dashboadopex += `<div style="background-color:${color[index]};width:${i.Width}%;display: inline-block;height:50px;text-align:center; line-height: 41px;color:white;padding:5px;">${isNaN(i.Person) ? 0 : i.Person.toFixed(2)}%</div>`;
            index++;
        });
        arr.forEach(i => {
            list += ` <p style="width:10px;height:10px;background-color:${color[j]}; "></p><span style=" margin-top: -6px;padding: 0px 10px;">${i.Name}</span>`;
            j++;
        });


        dashboadopex += `<div style="padding-top:100px;position: absolute;width:100% ;display:flex;">${list}</div>`;
        $("#groupopex").html(groupopex);
        // $("#chart-demo-list").html(dashboadopex_list);
        // block pie chart
        let _grossProfit = res[0].GrossProfit == 0 ? 0 : res[0].GrossProfit / res[0].Revenue;
        let _opex = res[0].Opex == 0 ? 0 : res[0].Opex / res[0].Revenue;
        let _operatingProfit = res[0].OperatingProfit == 0 ? 0 : res[0].OperatingProfit / res[0].Revenue;
        let _netprofitmargin = res[0].NetProfit == 0 ? 0 : res[0].NetProfit / res[0].Revenue;
        const gaugeElement = document.querySelector("#grossprofit");
        setGaugeValue(gaugeElement, _grossProfit);
        const opexmargin = document.querySelector("#opexmargin");
        setGaugeValue(opexmargin, _opex);
        const operatingprofitmargin = document.querySelector("#operatingprofitmargin");
        setGaugeValue(operatingprofitmargin, _operatingProfit);
        const netprofitmargin = document.querySelector("#netprofitmargin");
        setGaugeValue(netprofitmargin, _netprofitmargin);
        //...........................................
        $("#chart-demo-opex").append(dashboadopex);


    })
    function setGaugeValue(gauge, value) {
        // if (value < 0 || value > 1) {
        //     return;
        // }
        let rotate = value == 0 ? 0 : value / 2;

        gauge.querySelector(".gauge__fill").style.transform = `rotate(${rotate}turn)`;
        gauge.querySelector(".gauge__cover").textContent = `${Math.round(value * 100)}%`;
    }

    $.get("/Service/BarchartRevenue_COGS", function (res) {
        BarChartRevenue_COGS(res);

    });
    $.get("/Service/ChartbarEBIT", function (res) {
        Earrningbefor(res);
    });
    $.get("/Service/BarchartOpexs", function (res) {

        BarcharOpex_OnMonth(res);
    });
    function BarChartRevenue_COGS(arr) {
        $('#chart-service').dxChart({
            dataSource: arr,
            // rotated:true,
            commonSeriesSettings: {
                argumentField: 'Date',
                type: 'bar',
                hoverMode: 'allArgumentPoints',
                selectionMode: 'allArgumentPoints',
                label: {
                    visible: false,
                    format: {
                        type: 'fixedPoint',
                        precision: 0,
                    },
                },
            },
            series: [
                { valueField: 'Revenue', name: 'Revenue', color: "#33D5FF", },
                { valueField: 'COGS', name: 'Gross Profit' },

            ],
            //title: '',
            legend: {
                verticalAlignment: 'bottom',
                horizontalAlignment: 'center',
            },
            argumentAxis: {
                grid: {
                    visible: true,
                },
                label: {
                    rotationAngle: -45,
                    overlappingBehavior: 'rotate',
                },
            },
            export: {
                enabled: true,
            },
            // onPointClick(e) {
            //   e.target.select();
            // },
        });
    }
    // ..............................
    function Earrningbefor(arr) {
        $(() => {
            $('#chart-demo-ebti').dxChart({
                dataSource: arr,
                commonSeriesSettings: {
                    argumentField: 'Date',
                    type: 'spline',
                    hoverMode: 'includePoints',
                    point: {
                        hoverMode: 'allArgumentPoints',
                    },
                },
                series: [
                    { valueField: 'OperatingProfit', name: 'EBIT Actual -- EBIT Target' },

                ],
                stickyHovering: false,
                title: {
                    text: '',
                },
                export: {
                    enabled: true,
                },
                legend: {
                    verticalAlignment: 'bottom',
                    horizontalAlignment: 'center',
                    hoverMode: 'excludePoints',
                },
                argumentAxis: {
                    grid: {
                        visible: true,
                    },
                    label: {
                        rotationAngle: -45,
                        overlappingBehavior: 'rotate',
                    },
                },

            });
        });
    }
    //...........................OpexChart...........
    function BarcharOpex_OnMonth(arr) {

        const dataSource = [];
        if (arr.length > 0)
            arr.forEach(i => {
                dataSource.push({
                    year: i.Date,
                    africa: i.Revenue1,
                    asia: i.Revenue2,
                    europe: i.Revenue3,
                    total: i.Total,
                });
            });

        $(() => {
            $('#chart-demo-opexbar').dxChart({
                palette: 'vintage',
                dataSource: dataSource,
                commonSeriesSettings: {
                    argumentField: 'year',
                    type: 'fullstackedbar',
                },
                series: [{
                    valueField: 'africa',
                    name: arr.length > 0 ? arr[0].Name : " ",
                    color: arr.length > 0 ? "#0099e6" : "white",

                }, {
                    valueField: 'asia',
                    name: arr.length > 0 ? arr[0].Name2 : " ",
                    color: arr.length > 0 ? "#4dc3ff" : "white",
                }, {
                    valueField: 'europe',
                    name: arr.length > 0 ? arr[0].Name3 : " ",
                    color: arr.length > 0 ? "#80dfff" : "white",
                }, {
                    axis: 'total',
                    type: 'spline',
                    valueField: 'total',
                    name: 'Total',
                    color: '#ffa366',
                },
                ],
                valueAxis: [{
                    grid: {
                        visible: true,
                    },

                }, {
                    name: 'total',
                    position: 'right',
                    grid: {
                        visible: true,
                    },

                }],
                tooltip: {
                    enabled: true,
                    shared: true,
                    format: {
                        type: 'largeNumber',
                        precision: 1,
                    },
                    customizeTooltip(arg) {
                        const items = arg.valueText.split('\n');
                        const color = arg.point.getColor();
                        $.each(items, (index, item) => {
                            if (item.indexOf(arg.seriesName) === 0) {
                                items[index] = $('<span>')
                                    .text(item)
                                    .addClass('active')
                                    .css('color', color)
                                    .prop('outerHTML');
                            }
                        });
                        return { text: items.join('\n') };
                    },
                },
                legend: {
                    verticalAlignment: 'bottom',
                    horizontalAlignment: 'center',
                },
                argumentAxis: {
                    grid: {
                        visible: true,
                    },
                    label: {
                        rotationAngle: -45,
                        overlappingBehavior: 'rotate',
                    },
                },
                export: {
                    enabled: true,
                },
                //   title: {
                //     text: 'Evolution of Population by Continent',
                //   },
            });
        });

    }


});
