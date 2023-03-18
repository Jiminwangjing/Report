

$.get("/CRMDashboard/GetActivityByType", function (res) {
    datas(res)
});
let __data = [];
let __arr = [];
function datas(res) {
    res.forEach(i => {

        __arr = [{
            country: i.Type,
            area: i.PercentTage,
        }];
        __arr.forEach(s => {
            __data.push(s);
        })


    })

}
$(() => {

    const legendSettings = {
        verticalAlignment: 'bottom',
        horizontalAlignment: 'center',
        itemTextPosition: 'right',
        rowCount: 2,
    };
    setTimeout(piechart,3000);
    function piechart() {
        $('#pie').dxPieChart({
            size: {
                width: 300,
                height: 300
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

            legend: legendSettings,
            customizeText(arg) {
                return `${arg.valueText} %`;
            },

        });
    }
     



});