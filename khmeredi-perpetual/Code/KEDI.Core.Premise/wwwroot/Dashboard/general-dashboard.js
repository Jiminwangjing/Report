$(document).ready(function () {
    $(".loading").prop("hidden", false);
    $.get("/Home/GetSaleReports", function (reportModel) {
        bindSaleGroups(reportModel.SaleByGroups);
        bindTopSales(reportModel.TopSales);
        bindMonthlySales(reportModel.MonthlySales);
        bindStock(reportModel.Stocks);
        bindAccountRevenue(reportModel.BalanceTotal);
        bindAccountReceivable(reportModel.Balance);
        bindAverageRecipt(reportModel.AverageReceipts);
        bindAverageQty(reportModel.AverageQty);
        $(".loading").prop("hidden", true);
    })
    function bindSaleGroups(dataSource) {
        $("#chart_by_Groups").dxPieChart({
            type: "Pie",
            palette: "harmony light",
            dataSource: dataSource,
            size: {
                height: 200
            },
            tooltip: {
                enabled: true,
                customizeTooltip: function (arg) {
                    return {
                        text: arg.argumentText + " : " + arg.valueText
                    };
                }
            },
            legend: {
                horizontalAlignment: "right",
                verticalAlignment: "top",
                margin: 0,


            },
            "export": {
                enabled: false
            },
            series: [{
                argumentField: "Group1",

                valueField: "Amount",

                label: {
                    visible: true,
                    connector: {
                        visible: true
                    }
                }
            }]
        });
    }

    function bindMonthlySales(dataSource) {
        $("#chart_Sale_Month").dxChart({

            dataSource: dataSource,
            size: {
                height: 200
            },
            legend: {
                visible: false
            },
            tooltip: {
                enabled: true,
                customizeTooltip: function (arg) {
                    return {
                        text: arg.valueText
                    };
                }
            },
            argumentAxis: {
                label: {
                    customizeText: function () {
                        return "" + this.valueText;
                    }
                }
            },
            series: {
                color: "#33D5FF",
                argumentField: "Month",
                valueField: "GrandTotal",
                type: "bar"
            },

        });
    }
    function bindTopSales(dataSource) {
        $("#chart_top_sale").dxChart({
            rotated: true,
            dataSource: dataSource,
            size: {
                height: 200
            },
            series: {
                label: {
                    visible: true,
                    backgroundColor: "#c9c9c9",

                },
                color: "#33D5FF",
                type: "bar",
                argumentField: "ItemName",
                valueField: "Amount"
            },
            argumentAxis: {
                label: {
                    customizeText: function () {
                        return "" + this.valueText;
                    }
                }
            },
            valueAxis: {

                tick: {
                    visible: false
                },
                label: {
                    visible: false
                }
            },
            "export": {
                enabled: false
            },
            legend: {
                visible: false
            }
        });
    }
    function bindStock(dataSource) {
        var formatNumber = new Intl.NumberFormat("en-US", { maximumFractionDigits: 0 }).format;
        var commonSettings = {
            innerRadius: 0.65,
            resolveLabelOverlapping: "shift",
            sizeGroup: "piesGroup",
            tooltip: {
                enabled: true,
                customizeTooltip: function (arg) {
                    return {
                        text: arg.argumentText + " : " + arg.valueText
                    };
                }
            },
            legend: {
                visible: true
            },

            type: "doughnut",
            series: [{
                argumentField: "ItemNameStock",
                valueField: "CumlativeValue",
                label: {
                    visible: true,
                    connector: {
                        visible: true
                    },
                    format: {
                        type: 'fixedPoint',
                        precision: 2
                    }
                    //format: "fixedPoint",
                    //backgroundColor: "none",

                }
            }],
            centerTemplate: function (pieChart, container) {
                var total = pieChart.getAllSeries()[0].getVisiblePoints().reduce(function (s, p) { return s + p.originalValue; }, 0),
                    TOTAL = pieChart.getAllSeries()[0].getVisiblePoints()[0]?.data.country,
                    content = $('<svg><circle cx="100" cy="100" fill="#eee" r="' + (pieChart.getInnerRadius() - 6) + '"></circle>' +
                        '<text text-anchor="middle" style="font-size: 18px" x="100" y="95" fill="#494949">' + 'TOTAL' +
                        '<tspan x="100" dy="20px" style="font-weight: 500">' +
                        formatNumber(total) +
                        '</tspan></text></svg>');
                container.appendChild(content.get(0));
            }
        };

        $("#chart_stock")
            .dxPieChart($.extend({}, commonSettings, {
                dataSource: dataSource,
                size: {
                    height: 200
                },
            }));

    }
    function bindAccountRevenue(dataSource) {
        $("#chart_account_revenue").text(`${curFormat(dataSource.BalanceTotal)} ${dataSource.Currency}`);
    };
    function bindAccountReceivable(dataSource) {

        $("#chart_account_receivable").text(`${curFormat(dataSource.BalanceTotal)} ${dataSource.Currency}`);
    };
    function bindAverageRecipt(dataSource) {
        $("#chart_average_receipt").text(`${curFormat(dataSource.BalanceTotal)} ${dataSource.Currency}`);
    };
    function bindAverageQty(dataSource) {
        $("#chart_average_qty").text(`${curFormat(dataSource.BalanceTotal)}`);
    };
    //format currency
    function curFormat(value) {
        return value.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
});