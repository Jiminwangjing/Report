const dataStr = $("#model-data").text();
let __arropp = [],
    __opp = [],
    __oppstage = [],
    __arroppstage = [],
    __data = [],
    __arr = [];
if (dataStr) {
    let disSetting = JSON.parse(dataStr).GeneralSetting;
    //const disSetting = _data.GeneralSetting;
    const num = NumberFormat({
        decimalSep: disSetting.DecimalSeparator,
        thousandSep: disSetting.ThousandsSep
    });
    //====================get systemcurr===============

    //========Get data of Top DEALS=======
    $.get("/CRMDashboard/GetTopDeals", function (res) {
        $.get("/CRMDashboard/GetSyscurr", function (curr) {


            var data;
            var x = 1;
            res.forEach(i => {
                data = `
                    <div id="content" >
                              <div  style='display:flex;height:26px !important'>
                                <div class="circle blue">${x}</div>
                            <p style="color:#FFFFFF;font-family: Arial;font-size:16px;font-weight:bold;margin-left:10px !important;">${i.BPName}</p>
                               </div>
                            <p style="color:#FFFFFF;font-family:Arial;font-size:14px;font-weight:bold;width:100%;text-align:left">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp  ${num.formatSpecial(i.ExcetedAmount, disSetting.Amounts)} ${curr.Description}</span></p>

                    <div>
                        <p style="margin-top:-10px;color:#FFFFFF;font-family:Arial;!important;font-size:12px;margin-bottom:20px !important;text-align:left" >&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Sale Rep :  ${i.Employee}</p>
                        <p style="margin-top:-20px;color:#FFFFFF;font-family:Arial; !important;font-size:12px;margin-bottom:20px !important;text-align:left">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Sales Cycle Length: ${i.PredictedClosingInNum} ${i.PredictedClosingInTime}</p>
                        <p style="margin-top:-20px;color:#FFFFFF;font-family:Arial; !important;font-size:12px;margin-bottom:20px !important;text-align:left">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Stage:  ${i.StageName} </p>
                    </div>
                        </div>
                `
                x++;
                $(".div").append(data);
            })

        });
    })

    let $listView = ViewTable({
        keyField: "ID",
        selector: "#list-data",
        paging: {
            enabled: false,
            pageSize: 10
        },
        visibleFields: [
            "EmpName",
            "data",
        ],
    });
    $.get("/CRMDashboard/GetDataUser", function (data) {
        $listView.clearRows();
        $listView.bindRows(data);
    });
    //=======SALE OPPORTUNITY TOP 5 SALE PEOPLE==
    $.get("/CRMDashboard/GetOpportunityTopFive", function (res) {
        datasopp(res)
    });
    //========Oppen Opportunity By Stage=====
    $.get("/CRMDashboard/GetOpportunityByStage", function (res) {
        datasoppbystage(res)
    });
    $.get("/CRMDashboard/GetActivityByType", function (res) {
        datas(res)
    });
    //===========get deal by customer source=====
    $.get("/CRMDashboard/GetdataDealByCustomerSource", function (res) {
        dataofcussorce(res)
    });
    //===========get win by stage=====
    $.get("/CRMDashboard/GetdataWinDealByStage", function (res) {
        dataofwindealbystage(res)
    });
    //===========oppoturnity win ratio=====
    //$.get("/CRMDashboard/GetWinOpportunityRatio", function (res) {
    //    dataofoppratio(res)
    //});
    var toalofsummary = 0;
    var totalper = 0;
    var totalopp = 0;
    $.get("/CRMDashboard/GetTotalOpporturnity", function (res) {
        $("#dataTotalOfOpp").append(res.length);
        if (res.length != 0) {
            $("#dataTotalOfSummary").append(res[0].CountSummary);
            if (res[0].CountSummary != 0) {
                totalper = res[0].CountSummary / res.length * 100;
                totalopp = 100 / totalper;
                $("#numofOpp").append(num.formatSpecial(totalopp, 0));
                $("#numofWin").append(1);
            }
            else {


                $("#numofOpp").append(0);
                $("#numofWin").append(0);
            }

        }
        else {
            $("#dataTotalOfSummary").append(res.length);
            $("#numofOpp").append(0);
            $("#numofWin").append(0);
        }

        var width = 80,
            height = 100;
        var outerRadius = width / 2;
        var innerRadius = 30;
        var data = [num.formatSpecial(totalper, 0)];
        var pie = d3.layout.pie().value(function (d) {
            return d;
        });
        var endAng = function (d) {
            return (d / 100) * Math.PI * 2;
        };
        var bgArc = d3.svg
            .arc()
            .innerRadius(innerRadius)
            .outerRadius(outerRadius)
            .startAngle(0)
            .endAngle(Math.PI * 2);
        var dataArc = d3.svg
            .arc()
            .innerRadius(innerRadius)
            .outerRadius(outerRadius)
            .cornerRadius(15)
            .startAngle(0);
        var svg = d3
            .select('.chart-area')
            .append("svg")
            .attr("preserveAspectRatio", "xMinYMin meet")
            .attr("viewBox", "0 0 100 100")
            .attr("class", "shadow")
            .classed("svg-content", true);
        var path = svg
            .selectAll("g")
            .data(pie(data))
            .enter()
            .append("g")
            .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");
        path
            .append("path")
            .attr("d", bgArc)
            .style("stroke-width", 5)
            .attr("fill", "rgb(0, 143, 208)");
        path
            .append("path")
            .attr("fill", "#DDDDDD")
            .transition()
            .ease("ease-in-out")
            .duration(750)
            .attrTween("d", arcTween);
        path
            .append("text")
            .attr("fill", "#fff")
            .attr("font-size", "1.3em")
            .attr("tex-anchor", "middle")
            .attr("x", -13)
            .attr("y", 8)
            .transition()
            .ease("ease-in-out")
            .duration(750)
            .attr("fill", "#000")
            .text(data);
        path.append("text")
            .attr("fill", "#fff")
            .attr("class", "ratingtext")
            .attr("font-size", "0.6em")
            .attr("tex-anchor", "middle")
            .attr("x", 10)
            .attr("y", 8)
            .text('%')
            .transition()
            .ease("ease-in-out")
            .duration(750)
            .attr("fill", "#000");
        function arcTween(d) {
            var interpolate = d3.interpolate(d.startAngle, endAng(d.data));
            return function (t) {
                d.endAngle = interpolate(t);
                return dataArc(d);
            };
        }

    })
    $(() => {
        let _width = 450, _height = 300;
        if (window.outerWidth < 1561) {
            _width = 450;
            _height = 300
            WinDealByStage();
            DealByCustomerSource();
            charts();
            chart();
            pie();
        }
        window.onresize = function () {
            _width = 510, _height = 300;
            if (window.innerWidth < 1561) {
                _width = 450;
                _height = 300
                WinDealByStage();
                DealByCustomerSource();
                charts();
                chart();
                pie();
            }
        }
        //const legendSettings = {
        //    // verticalAlignment: 'right',
        //    //horizontalAlignment: 'right',
        //    //itemTextPosition: 'right',
        //    //rowCount: 1,
        //    position: "inside", // or "outside" 
        //    horizontalAlignment: "right", // or "left" | "right" 
        //    verticalAlignment: "center" // or "bottom" 
        //};
        setTimeout(chart, 1000);
        function chart() {
            $('#chart-crm').dxChart({
                rotated: true,
                palette: 'soft',
                size: {
                    width: _width,
                    height: _height
                },
                dataSource: __opp,
                series: {
                    color: '#088FD7',
                    argumentField: 'country',
                    valueField: 'medals',
                    type: 'bar',
                    label: {
                        visible: true,
                    },
                },
                legend: {
                    visible: false,
                },

                argumentAxis: {
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
        setTimeout(charts, 1000);
        function charts() {
            $('#charts').dxChart({
                rotated: false,
                dataSource: __arroppstage,
                size: {
                    width: _width,
                    height: _height
                },
                series: {
                    color: '#99cc00',
                    argumentField: 'country',
                    valueField: 'medals',
                    type: 'bar',
                    label: {
                        visible: true,
                    },
                },

                legend: {
                    visible: false,
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
        setTimeout(pie, 1000);
        function pie() {
            $('#pie-crm').dxPieChart({
                size: {
                    width: _width,
                    height: _height
                },
                palette: 'bright',
                dataSource: __data,
                series: [
                    {
                        argumentField: 'country',
                        valueField: 'area',
                        label: {
                            visible: true,
                            format: {
                                type: 'fixedPoint',
                                precision: 2,
                            },
                            customizeText(arg) {
                                return `${arg.valueText} %`;
                            },
                            connector: {
                                visible: true,
                                width: 1,
                            },
                        },
                    },
                ],
                //legend: legendSettings,
            });
        }
        setTimeout(DealByCustomerSource, 1000);
        function DealByCustomerSource() {
            $('#piehcartofcus').dxPieChart({
                size: {
                    width: _width,
                    height: _height
                },
                innerRadius: 0.65,
                resolveLabelOverlapping: 'shift',
                palette: 'bright',
                dataSource: __datacussource,
                type: 'donut',
                title: '',
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
                        //format: 'fixedPoint',
                        customizeText(arg) {
                            return `${arg.valueText} K`;
                        },
                    },
                    //format: 'fixedPoint',

                }],
                centerTemplate(pieChart, container) {
                    const content = $(`<svg><circle cx="100" cy="100" fill="#eee" r="${pieChart.getInnerRadius() - 6}"></circle>`
                        + '<text text-anchor="middle" style="font-size: 18px" x="100" y="105" fill="#0DA2DD">'
                        /*  + `<tspan x="100" >${country}</tspan></text></svg>`);*/
                        + `<tspan x="100" >${total / 1000}K</tspan></text></svg>`);
                    container.appendChild(content.get(0));
                },
            }).dxPieChart('instance');
        }
        setTimeout(WinDealByStage, 1000);
        function WinDealByStage() {

            $('#piechartwindealbystate').dxPieChart({
                size: {
                    width: _width,
                    height: _height
                },
                innerRadius: 0.65,
                resolveLabelOverlapping: 'shift',
                palette: 'bright',
                dataSource: __dataofwindeal,
                type: 'donut',
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
                        customizeText(arg) {
                            return `${arg.valueText} K`;
                        },

                    },
                }],
                centerTemplate(pieChart, container) {


                    const content = $(`<svg><circle cx="100" cy="100" fill="#eee" r="${pieChart.getInnerRadius() - 6}"></circle>`

                        + '<text text-anchor="middle" style="font-size: 18px;x="100" y="105" fill="#0DA2DD">'
                        + `<tspan  x="100">${toalofex / 1000}K</tspan></text></svg>`);

                    container.appendChild(content.get(0));
                },

            }).dxPieChart('instance');
        }

    });

}
function datasopp(res) {
    res.forEach(i => {
        __arropp = [
            {
                country: i.SaleEmp,
                medals: i.ExcetedAmount,
            },
        ];
        __arropp.forEach(s => {
            __opp.push(s);
        })
    })

}
function datasoppbystage(res) {
    res.forEach(i => {
        __oppstage = [
            {
                country: i.Name,
                medals: i.ExcetedAmount,

            },
        ];
        __oppstage.forEach(s => {
            __arroppstage.push(s);
        })
    })
}
function datas(res) {

    res.forEach(i => {

        __arr = [{
            country: i.TypeName,
            area: i.PercentTage,
        }];
        __arr.forEach(s => {
            __data.push(s);
        })


    })

}
let __datasource = [];
let __datacussource = [];
var total = 0;
function dataofcussorce(res) {
    res.forEach(i => {
        if (i.CustomerSourceID > 0) {
            total += i.ExcetedAmount;
            __datasource = [{
                country: i.CustomerSourceName,
                medals: i.ExcetedAmount / 1000,

            }];
            __datasource.forEach(s => {
                __datacussource.push(s);
            })
        }


    })

}
var __datawidnealbystage = [];
var __dataofwindeal = [];
var toalofex = 0;
function dataofwindealbystage(res) {
    res.forEach(i => {
        toalofex += i.ExcetedAmount;
        __datawidnealbystage = [{
            country: i.StageName,
            medals: i.ExcetedAmount / 1000,
        }];
        __datawidnealbystage.forEach(s => {
            __dataofwindeal.push(s);
        })
    })

}


var __windratio = [];
var __windoppatio = [];


