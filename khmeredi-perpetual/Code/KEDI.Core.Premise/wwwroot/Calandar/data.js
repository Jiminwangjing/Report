$(document).ready(function () {
    $.get("/BusinessPartner/GetListDataActivity", function (res) {
      
        datas(res);
        datacolor(res);
    });
    let __arr = [];
    let __data = [];
    function datas(res) {  
        res.forEach(i => {
            __arr = [
                {
                    text: i.Remark,
                    roomId: [i.ActivityID],
                    Number: i.Number,
                    startDate: i.StartTime,
                    endDate: i.EndTime,

                },
            ];
            __arr.forEach(s => {
                __data.push(s);
            })
            
        })

        //calandar(__data)
    }

    let __arrc = [];
    let __datac = []
    function datacolor(res) {

        res.forEach(i => {
            console.log("i", i.Color)
            __arrc = [
                {
                    text: i.Remark,
                    id: i.ActivityID,
                    color: `${i.Color}`,
                },
            ];
            __arrc.forEach(s => {
                __datac.push(s);
            })
        })
    }
        $(() => {
            $('#scheduler').dxScheduler({

                height: 600,
                dataSource: __data,
                showAllDayPanel: false,
                views: ['day',, 'week', 'month'],
                //views: [
                //    {
                //        type: 'timelineWorkWeek',
                //        name: 'day',
                //    },
                //    {
                //        type: 'week',
                //        name: 'Week',
                //        //groupOrientation: 'vertical',
                //    },
                //    {
                //        type: 'month',
                //        groupOrientation: 'horizontal',
                //    },
                //],
                currentView: 'day',
                resources: [{
                    fieldExpr: 'roomId',
                    dataSource: __datac,
                    label: 'Room',
                }],
              
                //editing: {
                //    allowAdding: false,
                //    allowDeleting: false,
                //    allowUpdating: false,
                //    allowResizing: false,
                //    allowDragging: false,
                //},
                onAppointmentClick: function (e) {

                    const { Number } = e.appointmentData // use it to extra data from appointmentData;
                    location.href = "/BusinessPartner/Activity?number=" + Number;
                },
                onAppointmentAdded: function (e) {
                    console.log(e)
                }
            });

        });


    //==============================================get list employee=========================================

    //const __listemp = ViewTable({
    //    keyField: "ID",
    //    selector: "#list-emp", 
    //    paging: {
    //        pageSize: 10,
    //        enabled: false
    //    },
    //    visibleFields: ["EmpName"],
    //    columns: [
    //        {
    //            name: "EmpName",
    //            template: "<input type='text' readonly style='cursor:pointer;' class='chcolor'>",
    //            on: {
    //                "click": function (e) {
                      
    //                    $.ajax({
    //                        url: "/BusinessPartner/GetListDataActivity",
    //                            type: "Get",
    //                            data: { id: e.data.UserID },
    //                            dataType: "Json",
    //                             success: function (response) {
    //                            __data = [];
    //                            datas(response);
    //                    }
    //                });
    //                }
    //            }
    //        },
    //    ]
    //});

    //$.get("/BusinessPartner/GetListDataEmp", function (res) {
    //    console.log("res", res);
    //    __listemp.bindRows(res)
       
    //});

    $("#view-all").click(function () {
        $.get("/BusinessPartner/GetListDataActivity", function (res) {
            __data = [];
            datas(res);
            datacolor(res);
        });
    })

});

   

