"user strict";

$(document).ready(function () {
    let masterAlert = JSON.parse($("#_masterAlert").text());

    ViewTable({
        selector: "#list-alert",
        keyField: "ID",
        indexed: true,
        paging: {
            enabled: true,
            pageSize: 10
        },
        visibleFields: [
            "Code", "Name", "StatusAlert"
        ],
        columns: [
            {
                name: "StatusAlert",
                template: "<input type='checkbox' style='margin:0 34%'>",
                on: {
                    "click": function (e) {

                        $.post("/AlertManagement/CheckActive", { ID: e.key }, function (_res) {
                            if (_res.Error == true) {
                                $("#modal-warning-alertM").modal("show");
                            } else {
                                location.reload();
                            }
                        });
                    }
                }
            }
        ],
        actions: [
            {
                template: "<button class='btn btn-sm btn-info fn-sky text-light' style='margin:0 36%'> Setting </button>",
                on: {
                    "click": function (e) {

                        $.post("/AlertManagement/GetSetting", { ID: e.key }, function (_res) {

                            $("#bf-app-input").val(_res.BeforeAppDate);
                            $("#bf-app-select").val(_res.TypeBeforeAppDate);
                            $("#fq-app-input").val(_res.Frequently);
                            $("#fq-app-select").val(_res.TypeFrequently);

                            ConfigSetting(_res);

                            $("#modal-setting-alert").modal("show");

                            //Table detail setting
                            ViewTable({
                                selector: "#list-user-alert",
                                keyField: "UserAccountID",
                                indexed: true,
                                paging: {
                                    enabled: true,
                                    pageSize: 10
                                },
                                visibleFields: [
                                    "UserName", "StatusAlertUser"
                                ],
                                columns: [
                                    {
                                        name: "StatusAlertUser",
                                        template: "<input  type='checkbox' style='margin:0 34%'>",
                                        on: {
                                            "click": function (e) {
                                                $.each(_res.SetttingAlertUser, function (i, item) {
                                                    if (e.key == item.UserAccountID) {
                                                        if (item.StatusAlertUser == 1) {
                                                            item.StatusAlertUser = 0;
                                                        } else {
                                                            item.StatusAlertUser = 1;
                                                        }
                                                    }
                                                });
                                            }
                                        }
                                    }
                                ]
                            }).bindRows(_res.SetttingAlertUser);

                        }); //End of Ajax
                    }
                }
            }
        ]

    }).bindRows(masterAlert);

    function ConfigSetting(_res) {
        //Before App date
        $("#bf-app-input").keyup(function () {
            _res.BeforeAppDate = this.value;
        });

        $("#bf-app-select").change(function () {
            _res.TypeBeforeAppDate = this.value;
        });

        //Frequently
        $("#fq-app-input").keyup(function () {
            _res.Frequently = this.value;
        });

        $("#fq-app-select").change(function () {
            _res.TypeFrequently = this.value;
        });

        //Save
        $("#save-alert-setting").click(function () {

            $.post("/AlertManagement/SaveAlertManagement", { Data: _res }, function (resp) {
                if (resp.Action == -1) {

                    ViewMessage({
                        summary: {
                            selector: "#validation-summary"
                        }
                    }, resp);

                    setTimeout(function () {
                        $("#validation-summary").text("");
                    }, 5000);
                }

                if (resp.Action == 1) {
                    ViewMessage({
                        summary: {
                            selector: "#validation-summary"
                        }
                    }, resp).refresh(700);
                }
            });

        });
    }

    $(".closeRefresh").click(function () {
        location.reload();
    });

    //Search Box
    $("#search-alert-list").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-alert tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });
    $("#search-user").keyup(function () {
        let _input = this;
        let search = $(_input).val().toString().toLowerCase();
        $("#list-user-alert tr:not(:first-child").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
        });
    });

    function defaultValApp(_input, selector) {
        $(selector).change(function () {
            var _bfValue = $(_input).val();
            if (_bfValue == "" || _bfValue == 0) {
                $(_input).val(1);
            }
        });
    }

    defaultValApp("#bf-app-input", "#bf-app-select");

    defaultValApp("#fq-app-input", "#fq-app-select");

});//End of document.ready
