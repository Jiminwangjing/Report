$(document).ready(function () {

    let __arremark = [];
    let __dataremark = [];
    let __arrdata = [];
    let __data = [];
    let __listarr = [];
    let __list = [];


    $.get("/BusinessPartner/GetListDataActivity", function (res) {
        data(res)
        lists(res);
    });
    $.get("/BusinessPartner/GetListDataEmp", function (res) {

        dataemp(res);
    });
    function dataemp(res) {
        res.forEach(i => {
            __arremark = [
                {
                    text: i.EmpName,
                    id: i.UserID,
                    color: `${i.Color}`,
                },
            ];
            __arremark.forEach(s => {
                __dataremark.push(s);
            })
        })
    }
    function data(res) {
        res.forEach(i => {
            __arrdata = [
                {
                    ActvityID: i.ID,
                    text: i.Remark,
                    Number: i.Number,
                    movieId: i.ID,
                    theatreId: i.UserID,
                    startDate: i.StartTime,
                    endDate: i.EndTime,

                },
            ];
            __arrdata.forEach(s => {
                __data.push(s);
            })
        })
    }
    function lists(res) {
        res.forEach(i => {
            __listarr = [
                {
                    id: i.ID,
                    movieId: i.UserID,
                    text: i.Remark,
                    color: `${i.Color}`,

                },
            ];
            __listarr.forEach(s => {
                __list.push(s);
            })
        })
    }

    setTimeout(calandar, 500);
    function calandar() {

        const dayOfWeekNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Staturday'];

        const dateCellTemplate = function (cellData, index, container) {
            container.append(
                $('<div />')
                    .addClass('name')
                    .text(dayOfWeekNames[cellData.date.getDay()]),
                $('<div />')
                    .addClass('number')
                    .text(cellData.date.getDate()),
            );
        };


        //text: "Hide the Popup",
        //onClick: function () {
        //    $(".dx-popup-content").dxPopup("hide");
        //    // === or ===
        //    $(".dx-popup-content").dxPopup("toggle", false);
        //}
        console.log("__list", __list);
        console.log("__data", __data)
        $('#scheduler').dxScheduler({
            dataSource: __data,
            views: [
                //{
                //    type: 'timelineWorkWeek',
                //    name: 'Day',

                //    groupOrientation: 'vertical',
                //},
                {
                    type: 'timelineWeek',
                    name: 'Day',
                    groupOrientation: 'vertical',
                    dateCellTemplate
                },
                {
                    type: 'timelineWorkWeek',
                    type: 'week',
                    name: 'Week',
                    groupOrientation: 'vertical',
                    dateCellTemplate

                },

                {
                    type: 'month',
                    name: 'Month',
                    groupOrientation: 'horizontal',
                },

            ],
            currentView: 'Day',
            startDayHour: 8,
            endDayHour: 20,
            showAllDayPanel: false,
            editing: {
                allowAdding: false,
                allowDeleting: false,
                allowUpdating: false,
                allowResizing: false,
                allowDragging: false,
                allowTimeZoneEditing: false,
            },
            groups: ['theatreId'],
            crossScrollingEnabled: true,
            cellDuration: 120,
            resources: [{
                fieldExpr: 'movieId',
                dataSource: __list,
                useColorAsDefault: true,
                allowMultiple: false,
            }, {
                fieldExpr: 'theatreId',
                dataSource: __dataremark,
                allowMultiple: false,
            }],

            onContentReady: function (e) {
                if (__data.length == 0) {
                    $(".dx-scheduler-work-space").css("margin-top", "-66px");
                    $(".dx-scheduler-work-space-vertical-grouped.dx-scheduler-work-space-both-scrollbar.dx-scheduler-work-space.dx-scheduler-work-space-week.dx-widget.dx-visibility-change-handler").css("margin-top", "-118px");
                    $(".dx-scheduler-header-scrollable.dx-scrollable.dx-visibility-change-handler.dx-scrollable-horizontal.dx-scrollable-simulated.dx-scrollable-scrollbars-hidden").css("height", "0px !important")
                }
                let tabsday = $(".dx-tabs-wrapper").find(".dx-tab-text")[0];

                let month = $(".dx-tabs-wrapper").find(".dx-tab");
                let dxTable = $(".dx-scheduler-header-panel");
                let dayCells = dxTable.find(".dx-scheduler-header-row:nth-child(2) .dx-scheduler-cell-sizes-horizontal");
                let colors = ["rgb(60 129 89)", "rgb(203 153 84)", "#0066ff", "rgb(235 80 147)", " rgb(71 193 209)", "#8a8a5c", "#cccc00"];
                let colordefual = ["rgb(154 159 221)"];
                for (var i = 0; i < month.length; i++) {
                    month[i].onclick = function () {
                        let date = $(this).find(".dx-tab-text");
                        if (date.text() == "Month") {
                            $("#resoure").hide();
                            for (let i = 0; i < dayCells.length; i++) {
                                if (i < 6) {
                                    $(dayCells[i]).css("background-color", colordefual[0]);
                                } else if (i < 6 * 2) {
                                    $(dayCells[i]).css("background-color", colordefual[0]);
                                } else if (i < 6 * 3) {
                                    $(dayCells[i]).css("background-color", colordefual[0]);
                                }
                                else if (i < 6 * 4) {
                                    $(dayCells[i]).css("background-color", colordefual[0]);
                                }
                                else if (i < 6 * 5) {
                                    $(dayCells[i]).css("background-color", colordefual[0]);
                                }
                                else if (i < 6 * 6) {
                                    $(dayCells[i]).css("background-color", colordefual[0]);
                                }
                                else if (i < 6 * 7) {
                                    $(dayCells[i]).css("background-color", colordefual[0]);
                                }

                            }
                        } else {
                            $("#resoure").show();
                        }


                    }
                }
                if ($(tabsday).text().toLowerCase() == "day") {

                    for (let i = 0; i < dayCells.length; i++) {
                        if (i < 6) {
                            $(dayCells[i]).css("background-color", colors[0]);
                        } else if (i < 6 * 2) {
                            $(dayCells[i]).css("background-color", colors[1]);
                        } else if (i < 6 * 3) {
                            $(dayCells[i]).css("background-color", colors[2]);
                        }
                        else if (i < 6 * 4) {
                            $(dayCells[i]).css("background-color", colors[3]);
                        }
                        else if (i < 6 * 5) {
                            $(dayCells[i]).css("background-color", colors[4]);
                        }
                        else if (i < 6 * 6) {
                            $(dayCells[i]).css("background-color", colors[5]);
                        }
                        else if (i < 6 * 7) {
                            $(dayCells[i]).css("background-color", colors[6]);
                        }

                    }
                }

            },
            onAppointmentClick: function (e) {
                const { Number } = e.appointmentData // use it to extra data from appointmentData;
                location.href = "/BusinessPartner/Activity?number=" + Number;
            },

        });
        if (__data.length > 0) {
            let weiget = $(".dx-scheduler-navigator.dx-widget");
            let i = 0;
            for (weiget.length; i <= 1; i++) {
                if (i == 1) {
                    var html = "<div id='resoure'>Resoure</div>";
                    let str = weiget.innerHTML = html;

                    $(".dx-scheduler-navigator.dx-widget").append(str);
                }
            }
        }
        if (__data.length == 0) {

            $(".dx-scheduler-work-space").css("margin-top", "-66px");
        }
        //let colordate = $(".dx-tab-text");
        //for (let i = 1; i <= colordate.length; i++) {
        //    if (i != 1) {
        //        console.log(i)
        //        colordate.css("color", "red")[i];
        //    }
        //}



    }


});








