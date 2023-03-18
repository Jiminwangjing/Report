$(function () {

    var id = 0;
    var g2id = 0;

    // Create New Group 1
    $(".add-group-1").click(function () {
        let dialog = new DialogBox({
            type: 'yes-no-cancel',
            button: {
                yes: {
                    text: "Add",
                },
                no: {
                    text: "New",
                    callback: function () {
                        id = 0;
                        g2id = 0;
                        this.meta._footer.find(".button.bg-success").text("Add");
                        $("#g1name").val("").focus();
                    }
                },
                cancel: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    }
                }
            },
            caption: 'Create New Group 1',
            content: {
                selector: "#model-group1"
            }
        });
        dialog.invoke(function () {
            const button = dialog._footer.find(".button.bg-success");
            $.get("/BusinessPartner/GetGroup1", { type: "Vendor" }, function (respones) {
                Tableg1(respones, button)
            })
            dialog.confirm(function () {
                if (button.text().toLowerCase() == 'update') {
                    button.text("Add");
                }
                Insertgroup1(button);
            })
        })
    })

    // Create New Group 2
    $(".add-group-2").click(function () {
        let dialog = new DialogBox({
            type: 'yes-no-cancel',
            button: {
                yes: {
                    text: "Add",
                },
                no: {
                    text: "New",
                    callback: function () {
                        id = 0;
                        g2id = 0;
                        $.get("/BusinessPartner/GetGroup1", { type: "Vendor" }, function (respones) {

                            let addOption = "<option value='0' selected>--- Select ---</option>";
                            $.each(respones, function (i, item) {
                                addOption +=
                                    '<option value="' + item.ID + '">' + item.Name + '</option>';
                            });
                            $("#mgoup2").html(addOption);
                            $("#txt_group2name").val("").focus();
                            $("#g1name").val("");
                        })
                        this.meta._footer.find(".button.bg-success").text("Add");
                    }
                },
                cancel: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    }
                }
            },
            caption: 'Create New Group 2',
            content: {
                selector: "#modal-group2"
            }
        });
        dialog.invoke(function () {
            const button = dialog._footer.find(".button.bg-success");
            const datag2 = ViewTable({
                keyField: "ID",
                selector: $("#tbl_group2"),
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Name", "Type"],
                actions: [
                    {
                        template: `<i class="fa fa-edit cursor"></i>`,
                        on: {
                            "click": function (e) {
                                g2id = e.data.ID;
                                $("#txt_group2name").val(e.data.Name);
                                $("#mgoup2").val(e.data.Group1ID);
                                button.text("Update")
                            }
                        }
                    }
                ]
            });
            $.get("/BusinessPartner/GetGroup2", { type: "Vendor" }, function (respones) {
                datag2.bindRows(respones);
                search(respones, "#txtSearchg2", datag2);
            })
            dialog.confirm(function () {
                if (button.text().toLowerCase() == 'update') {
                    button.text("Add");
                }
                Insertgroup2(datag2);
            })
        })
    })
    //Group 
    $("#slg1").change(function () {
        let value = parseInt(this.value);

        if (value != 0) {
            $.get("/BusinessPartner/GetGroup2", { value: value, type: "Vendor" }, function (res) {

                let addOption = "<option value='0' selected>--- Select ---</option>";
                $.each(res, function (i, item) {
                    addOption +=
                        '<option value="' + item.ID + '">' + item.Name + '</option>';
                });
                $("#txtg2").html(addOption);

            });
        }
    });
    function Tableg1(data, button) {
        const datag1 = ViewTable({
            keyField: "ID",
            selector: $("#tbl_group1"),
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: true
            },
            visibleFields: ["Name", "Type"],
            actions: [
                {
                    template: `<i class="fa fa-edit cursor"></i>`,
                    on: {
                        "click": function (e) {
                            button.text("Update")
                            id = e.data.ID;
                            $("#g1name").val(e.data.Name);
                        }
                    }
                }
            ]
        });
        datag1.bindRows(data);
        search(data, "#txtSearchg1", datag1);
    }
    function Insertgroup1(button) {
        let name = $("#g1name").val();
        let count = 0;
        if (name == "") {
            count++;
            $("#g1name").css("border-color", "red");

        } else {
            $("#g1name").css("border-color", "lightgrey");
            count = 0;
        }

        if (count > 0) {
            count = 0;
            return;
        }
        Data = {
            ID: id,
            Name: name,
            Type: "Vendor",
        }

        $.ajax({
            url: "/BusinessPartner/CreateUpdateG1",
            type: "POST",
            dataType: "JSON",
            data: { group1: Data },
            complete: function () {
                $("#g1name").val("");
                id = 0;
                $.get("/BusinessPartner/GetGroup1", { type: "Vendor" }, function (respones) {

                    let addOption = "<option value='0' selected>--- Select ---</option>";
                    $.each(respones, function (i, item) {
                        addOption +=
                            '<option value="' + item.ID + '">' + item.Name + '</option>';
                    });
                    $("#slg1").html(addOption);
                    $("#mgoup2").html(addOption);
                    Tableg1(respones, button)
                })

            }
        });
    }
    function Insertgroup2(datag2) {
        let group1 = parseInt($("#mgoup2").val());
        let group2name = $("#txt_group2name").val();
        let count = 0;
        if (group1 == 0 || isNaN(group1)) {
            count++;
            $("#mgoup2").css("border-color", "red");

        }
        else {
            $("#mgoup2").css("border-color", "lightgrey");
        }
        if (group2name == "") {
            count++;
            $("#txt_group2name").css("border-color", "red");
        }
        else {
            $("#txt_group2name").css("border-color", "lightgrey");
        }
        let data = {
            ID: g2id,
            Group1ID: group1,
            Name: group2name,
            Type: "Vendor",
        }
        if (count > 0) {
            count = 0;
            return;
        }
        else {
            $.ajax({
                url: "/BusinessPartner/CreateUpdateG2",
                type: "POST",
                dataType: "JSON",
                data: { group2: data },
                complete: function () {
                    g2id = 0;
                    $("#mgoup2").val("");
                    $("#txt_group2name").val("");
                    $.get("/BusinessPartner/GetGroup1", { value: 0, type: "Vendor" }, function (rest) {

                        let addOption = "<option value='0' selected>--- Select ---</option>";
                        $.each(rest, function (i, item) {
                            addOption +=
                                '<option value="' + item.ID + '">' + item.Name + '</option>';
                        });
                        $("#mgoup2").html(addOption);

                    })
                    $.get("/BusinessPartner/GetGroup2", { value: 0, type: "Vendor" }, function (res) {
                        let addOption = "<option value='0' selected>--- Select ---</option>";
                        $.each(res, function (i, item) {
                            addOption +=
                                '<option value="' + item.ID + '">' + item.Name + '</option>';
                        });
                        $("#txtg2").html(addOption);

                        datag2.bindRows(res);
                    })
                }
            });
        }

    }
    $("#txt_saleemp").click(function () {

        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    }
                }
            },
            content: {
                selector: "#cont-em"
            }
        });

        dialog.invoke(function () {
            $.get("/BusinessPartner/GetSaleEMP", function (resp) {
                let $listControlGL = ViewTable({
                    keyField: "ID",
                    indexed: true,
                    selector: "#tbl_emp",
                    paging: {
                        pageSize: 20,
                        enabled: true
                    },
                    visibleFields: ["Name"],
                    actions: [
                        {
                            template: "<i class='fas fa-arrow-circle-down cursor'></i>",
                            on: {
                                "click": function (e) {
                                    $("#txt_empid").val(e.data.ID);
                                    $("#txt_saleemp").val(e.data.Name);
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                $listControlGL.clearRows();
                $listControlGL.bindRows(resp);
            });
        });
    });
    function search(data, inputSearch, table) {
        if (data.length > 0) {
            $(inputSearch).on("keyup", function (e) {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let rex = new RegExp(__value, "gi");
                let items = $.grep(data, function (item) {
                    return item.Name.toLowerCase().replace(/\s+/, "").match(rex);
                });
                table.bindRows(items);
            });
        }
    }
});