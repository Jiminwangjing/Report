$(function () {
    // block Mr Bunthorn 
    let id = 0;
    $("#EMType").click(function () {
        const dialogprojmana = new DialogBox({
            button: {
                ok: {
                    text: "SAVE",
                },
            },
            type: "ok-cancel",
            caption: "List ProjectCostAnalysis",
            content: {
                selector: "#em-content",
            }
        });
        dialogprojmana.confirm(function () {

            let obj = {};
            obj.ID = id;
            obj.Type = $("#txt_type").val();
            $.post("/Employee/SaveEMType", { data: obj }, function (data) {
                $("#txt_type").val("");
                id = 0;
                SaleEmployee(data.Data, dialogprojmana);
                if (data.IsApproved) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summarys"
                        }
                    }, data.Model);
                }
                new ViewMessage({
                    summary: {
                        selector: "#error-summarys"
                    }
                }, data.Model);
                $(window).scrollTop(0);
            });
        });
        dialogprojmana.invoke(function () {
            $.get("/Employee/CreateDefualtType", function (res) {
                SaleEmployee(res, dialogprojmana);
            });
        });
        dialogprojmana.reject(function () {
            dialogprojmana.shutdown();
            id = 0;
        });
    });


    function SaleEmployee(data, dialog) {
        const $list_activity = ViewTable({
            keyField: "LineID",//id unit not in dababase
            selector: "#list_activity",// id of table
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: true
            },
            visibleFields: ["Type"],

            actions: [
                {

                    template: `<i class="fas fa-edit"></i>`,
                    on: {
                        "click": function (e) {
                            $("#txt_type").val(e.data.Type);
                            id = e.data.ID;
                        }
                    }
                },
                {
                    name: "",
                    template: `<i class="fas fa-arrow-circle-down"></i>`,
                    on: {
                        "click": function (e) {
                            $("#EMType").val(e.data.Type);
                            $("#empTypeId").val(e.data.ID);
                            dialog.shutdown();
                        }
                    }
                },
            ],
        })
        $list_activity.bindRows(data);
        $("#txtsearchativity").on("keyup", function (e) {
            var value = $(this).val().toLowerCase();
            $("#list_activity tr:not(:first-child)").filter(function () {
                $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
            });
        });
    }
})