 <reference path="../lib/jquery/dist/jquery.min.js" />
(function ($) {
    //"use strict";

    //Team chart

    //var ctx = document.getElementById( "team-chart" );
    //ctx.height = 150;
    //var myChart = new Chart( ctx, {
    //	type: 'line',
    //	data: {
    //		labels: [ "2010", "2011", "2012", "2013", "2014", "2015", "2016" ],
    //		type: 'line',
    //		defaultFontFamily: 'Montserrat',
    //		datasets: [ {
    //			data: [ 0, 7, 3, 5, 2, 10, 7 ],
    //			label: "Expense",
    //			backgroundColor: 'rgba(0,103,255,.15)',
    //			borderColor: 'rgba(0,103,255,0.5)',
    //			borderWidth: 3.5,
    //			pointStyle: 'circle',
    //			pointRadius: 5,
    //			pointBorderColor: 'transparent',
    //			pointBackgroundColor: 'rgba(0,103,255,0.5)'
    //                   }, ]
    //	},
    //	options: {
    //		responsive: true,
    //		tooltips: {
    //			mode: 'index',
    //			titleFontSize: 12,
    //			titleFontColor: '#000',
    //			bodyFontColor: '#000',
    //			backgroundColor: '#fff',
    //			titleFontFamily: 'Montserrat',
    //			bodyFontFamily: 'Montserrat',
    //			cornerRadius: 3,
    //			intersect: false
    //		},
    //		legend: {
    //			display: false,
    //			position: 'top',
    //			labels: {
    //				usePointStyle: true,
    //				fontFamily: 'Montserrat'
    //			},


    //		},
    //		scales: {
    //			xAxes: [ {
    //				display: true,
    //				gridLines: {
    //					display: false,
    //					drawBorder: false
    //				},
    //				scaleLabel: {
    //					display: false,
    //					labelString: 'Month'
    //				}
    //                       } ],
    //			yAxes: [ {
    //				display: true,
    //				gridLines: {
    //					display: false,
    //					drawBorder: false
    //				},
    //				scaleLabel: {
    //					display: true,
    //					labelString: 'Value'
    //				}
    //                       } ]
    //		},
    //		title: {
    //			display: false
    //		}
    //	}
    //} );


    //Sales chart
    var ctx = document.getElementById("sales-chart");
    ctx.height = 150;
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: ["2010", "2011", "2012", "2013", "2014", "2015", "2016"],
            type: 'line',
            defaultFontFamily: 'Montserrat',
            datasets: [{
                label: "Foods",
                data: [0, 30, 10, 120, 50, 63, 10],
                backgroundColor: 'transparent',
                borderColor: 'rgba(220,53,69,0.75)',
                borderWidth: 3,
                pointStyle: 'circle',
                pointRadius: 5,
                pointBorderColor: 'transparent',
                pointBackgroundColor: 'rgba(220,53,69,0.75)',
            }, {
                label: "Electronics",
                data: [0, 50, 40, 80, 40, 79, 120],
                backgroundColor: 'transparent',
                borderColor: 'rgba(40,167,69,0.75)',
                borderWidth: 3,
                pointStyle: 'circle',
                pointRadius: 5,
                pointBorderColor: 'transparent',
                pointBackgroundColor: 'rgba(40,167,69,0.75)',
            }]
        },
        options: {
            responsive: true,

            tooltips: {
                mode: 'index',
                titleFontSize: 12,
                titleFontColor: '#000',
                bodyFontColor: '#000',
                backgroundColor: '#fff',
                titleFontFamily: 'Montserrat',
                bodyFontFamily: 'Montserrat',
                cornerRadius: 3,
                intersect: false,
            },
            legend: {
                display: false,
                labels: {
                    usePointStyle: true,
                    fontFamily: 'Montserrat',
                },
            },
            scales: {
                xAxes: [{
                    display: true,
                    gridLines: {
                        display: false,
                        drawBorder: false
                    },
                    scaleLabel: {
                        display: false,
                        labelString: 'Month'
                    }
                }],
                yAxes: [{
                    display: true,
                    gridLines: {
                        display: false,
                        drawBorder: false
                    },
                    scaleLabel: {
                        display: true,
                        labelString: 'Value'
                    }
                }]
            },
            title: {
                display: false,
                text: 'Normal Legend'
            }
        }
    });


    //line chart
    var ctx = document.getElementById("lineChart");
    ctx.height = 150;
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: ["January", "February", "March", "April", "May", "June", "July"],
            datasets: [
                {
                    label: "My First dataset",
                    borderColor: "rgba(0,0,0,.09)",
                    borderWidth: "1",
                    backgroundColor: "rgba(0,0,0,.07)",
                    data: [22, 44, 67, 43, 76, 45, 12]
                },
                {
                    label: "My Second dataset",
                    borderColor: "rgba(0, 123, 255, 0.9)",
                    borderWidth: "1",
                    backgroundColor: "rgba(0, 123, 255, 0.5)",
                    pointHighlightStroke: "rgba(26,179,148,1)",
                    data: [16, 32, 18, 26, 42, 33, 44]
                }
            ]
        },
        options: {
            responsive: true,
            tooltips: {
                mode: 'index',
                intersect: false
            },
            hover: {
                mode: 'nearest',
                intersect: true
            }

        }
    });


    //bar chart
    var ctx = document.getElementById("barChart");
    //    ctx.height = 200;
    var myChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ["January", "February", "March", "April", "May", "June", "July"],
            datasets: [
                {
                    label: "My First dataset",
                    data: [65, 59, 80, 81, 56, 55, 40],
                    borderColor: "rgba(0, 123, 255, 0.9)",
                    borderWidth: "0",
                    backgroundColor: "rgba(0, 123, 255, 0.5)"
                },
                {
                    label: "My Second dataset",
                    data: [28, 48, 40, 19, 86, 27, 90],
                    borderColor: "rgba(0,0,0,0.09)",
                    borderWidth: "0",
                    backgroundColor: "rgba(0,0,0,0.07)"
                }
            ]
        },
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        }
    });

    //radar chart
    var ctx = document.getElementById("radarChart");
    ctx.height = 160;
    var myChart = new Chart(ctx, {
        type: 'radar',
        data: {
            labels: [["Eating", "Dinner"], ["Drinking", "Water"], "Sleeping", ["Designing", "Graphics"], "Coding", "Cycling", "Running"],
            datasets: [
                {
                    label: "My First dataset",
                    data: [65, 59, 66, 45, 56, 55, 40],
                    borderColor: "rgba(0, 123, 255, 0.6)",
                    borderWidth: "1",
                    backgroundColor: "rgba(0, 123, 255, 0.4)"
                },
                {
                    label: "My Second dataset",
                    data: [28, 12, 40, 19, 63, 27, 87],
                    borderColor: "rgba(0, 123, 255, 0.7",
                    borderWidth: "1",
                    backgroundColor: "rgba(0, 123, 255, 0.5)"
                }
            ]
        },
        options: {
            legend: {
                position: 'top'
            },
            scale: {
                ticks: {
                    beginAtZero: true
                }
            }
        }
    });


    var datapiechart = [];
    var lblpiechart = [];
    var color = [];
    var numcolor = [209, 160, 255, 111, 192, 93, 111, 222, 201, 33, 95, 87, 45, 92, 190, 89, 145, 120, 123, 98, 76, 34, 66, 78, 200, 44, 222, 98, 22];
    //var numcolor = ["YELLOW", "ORANGERED", "DEEPPINK", "DARKSLATEBLUE", "LAWNGREEN", "SPRINGGREEN", "CYAN", "BLUE", "MAROON",
    //    "DarkOrchid", "GhostWhite", "green", "green1", "LightSlateBlue", "maroon", "MediumSlateBlue", "rebeccapurple", "yellow"];
    $(document).ready(function () {
        //pie chart
        $.ajax({
            type: "GET",
            url: "/Chart/GetProductStock",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $.each(response, function (i, item) {
                    datapiechart.push(item.TotalStockIn);
                    lblpiechart.push(item.WhsName);
                    color.push("rgba(" + numcolor[i + 2] + ", " + numcolor[i + 5] + ", " + numcolor[i + 3] + "," + 0.8 + ")");

                });
                //pie chart
                var ctx = document.getElementById("pieChart");
                ctx.height = 300;
                var myChart = new Chart(ctx, {
                    type: 'pie',
                    data: {
                        datasets: [{
                            data: datapiechart,
                            backgroundColor: color,
                            hoverBackgroundColor: color
                            //    [
                            //    "rgba(0, 123, 255,0.9)",
                            //    "rgba(0, 123, 255,0.7)",
                            //    "rgba(0, 123, 255,0.5)",
                            //    "rgba(0,0,0,0.07)"
                            //]

                        }],
                        labels: lblpiechart
                    },
                    options: {
                        responsive: true
                    }
                });

                //polar chart
                var ctx = document.getElementById("polarChart");
                ctx.height = 150;
                var myChart = new Chart(ctx, {
                    type: 'polarArea',
                    data: {
                        datasets: [{
                            data: datapiechart,
                            backgroundColor: [
                                "rgba(0, 123, 255,0.9)",
                                "rgba(0, 123, 255,0.8)",
                                "rgba(0, 123, 255,0.7)",
                                "rgba(0,0,0,0.2)",
                                "rgba(0, 123, 255,0.5)"
                            ]

                        }],
                        labels: lblpiechart
                    },
                    options: {
                        responsive: true
                    }
                });
            }

        });
    })


    $(document).ready(function () {
        var datadoughutchart = [];
        var lbldoughutchart = [];
        $.ajax({
            type: "GET",
            url: "/Chart/GetUserSale",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $.each(response, function (i, item) {
                    datadoughutchart.push(item.Amount);
                    lbldoughutchart.push(item.UserName);
                    color.push("rgba(" + numcolor[i + 2] + ", " + numcolor[i + 5] + ", " + numcolor[i + 3] + "," + 0.8 + ")");

                });
                //doughut chart
                var ctx = document.getElementById("doughutChart");
                ctx.height = 150;
                var myChart = new Chart(ctx, {
                    type: 'doughnut',
                    data: {
                        datasets: [{
                            data: datadoughutchart,
                            backgroundColor: color,
                            //[
                            //"rgba(0, 123, 255,0.9)",
                            //"rgba(0, 123, 255,0.7)",
                            //"rgba(0, 123, 255,0.5)",
                            //    "rgba(0,0,0,0.07)"],

                            hoverBackgroundColor: color
                            //    ["rgba(0, 123, 255,0.9)",
                            //    "rgba(0, 123, 255,0.7)",
                            //    "rgba(0, 123, 255,0.5)",
                            //    "rgba(0,0,0,0.07)"
                            //]

                        }],
                        labels: lbldoughutchart
                    },
                    options: {
                        responsive: true
                    }
                });
            }
        });
    })




    var lblbarchart = [];
    var databarchart = [];



    $(document).ready(function () {
        //single bar chart
        $.ajax({
            type: "GET",
            url: "/Chart/GetTop10Sale",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                $.each(response, function (i, item) {
                    lblbarchart.push(item.ProName);
                    databarchart.push(item.Quantity);
                    color.push("rgba(" + numcolor[i + 2] + ", " + numcolor[i + 5] + ", " + numcolor[i + 3] + "," + 0.8 + ")");

                });
                var myChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: lblbarchart,
                        datasets: [
                            {
                                label: "Sale",
                                data: databarchart,
                                borderColor: color,
                                borderWidth: "0",
                                backgroundColor: color //"rgba(0, 123, 255, 0.5)"
                            }
                        ]
                    },
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        }
                    }
                });
            }
        });
    })
    // single bar chart
    var ctx = document.getElementById("singelBarChart");
    ctx.height = 150;





});
