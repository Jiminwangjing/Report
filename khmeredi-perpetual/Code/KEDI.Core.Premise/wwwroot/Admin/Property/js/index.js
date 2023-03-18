$(document).ready(function () {
    let dataCPD = [];
    let $properties = ViewTable({
        keyField: "ID",
        selector: "#property",
        indexed: true,
        paging: {
            enabled: false,
            pageSize: 20
        },
        visibleFields: [
            "Name",
            "Active"
        ],
        columns: [
            {
                name: "Name",
                on: {
                    "dblclick": function (e) {
                        const id = e.data.ID;
                        location.href = `/Property/Update?id=${id}`
                    }
                }
            },
            {
                name: "Active",
                template: "<input type='checkbox'>",
                on: {
                    "click": function (e) {
                        const checked = $(this).prop("checked") ? true : false;
                        e.data.Active = checked;
                        $.post("/Property/UpdateProp", { data: e.data });
                    }
                }
            }
        ],
        actions: [
            {
                template: `<i class="fas fa-plus-circle hover" title="Create"></i>`,
                on: {
                    "click": function (e) {
                        $("#CPDID").val(e.data.ID);
                        let dialog = new DialogBox({
                            content: {
                                selector: "#c-property-d-content"
                            },
                            caption: "Create Child Property Detail",
                            type: "ok/cancel",
                            button: {
                                ok: {
                                    text: "Save",
                                    callback: function () {
                                        createCPD(this.meta);
                                    }
                                },
                                cancel: {
                                    text: "Close"
                                }
                            }
                        });
                        dialog.reject(function () {
                            dialog.shutdown();
                        })
                    }
                }
            },
            {
                template: `<i class="fas fa-info-circle hover" title="View"></i>`,
                on: {
                    "click": function (e) {
                        viewChildProp(e.data.ID);
                    }
                }
            }
        ]
    });
    getProperties(function (res) {
        $properties.clearRows();
        $properties.bindRows(res);
        $("#property tr").children("td:last-child").addClass("text-center");
        $("#property tr").children("td:nth-child(5)").addClass("text-center");
    })
    function viewChildProp(propID) {
        let dialog = new DialogBox({
            content: {
                selector: "#c-property-content"
            },
            caption: "List Children Property Detail",
            type: "ok/cancel",
            button: {
                ok: {
                    text: "Save",
                    callback: function () {
                        $.post("/Property/UpdateCPD", { cprop: JSON.stringify(dataCPD[0]) }, function (res) {
                            const msg = new ViewMessage({
                                summary: {
                                    selector: ".err-success-summery"
                                }
                            });
                            if (res.IsApproved) {
                                msg.bind(res);
                                setTimeout(function () { msg.remove("success") }, 700);
                            }
                            if (res.IsRejected) {
                                msg.bind(res);
                            }
                        })
                    }
                }
            }
        })
        dialog.invoke(function () {
            let $Childproperties = ViewTable({
                keyField: "ID",
                selector: "#list-child-properties",
                indexed: true,
                paging: {
                    enabled: false,
                    pageSize: 20
                },
                visibleFields: [
                    "Name",
                ],
                columns: [
                    {
                        name: "Name",
                        template: "<input >",
                        on: {
                            "keyup": function (e) {
                                updateCPD(dataCPD, e.data.ID, "Name", this.value);
                            }
                        }
                    }
                ],
                actions: [
                    {
                        template: "<i class='fas fa-trash hover'></i>",
                        on: {
                            "click": function (e) {
                                $.post("/Property/DeletedCPD", { id: e.data.ID }, function (res) {
                                    const msg = new ViewMessage({
                                        summary: {
                                            selector: ".err-success-summery"
                                        }
                                    });
                                    if (res.IsApproved) {
                                        $(".err-success-summery").prop("hidden", false);
                                        msg.bind(res);
                                        setTimeout(function () {
                                            dialog.shutdown();
                                            //msg.remove("success");
                                            $(".err-success-summery").prop("hidden", true);
                                        }, 700);
                                    }
                                    if (res.IsRejected) {
                                        $(".err-success-summery").prop("hidden", false);
                                        setTimeout(function () {
                                            $(".err-success-summery").prop("hidden", true);
                                        }, 1500);
                                        msg.bind(res);
                                    }
                                });
                                //dialog.shutdown();
                            }
                        }
                    }
                ]
            })

            $.get("/Property/GetChildrenOfProperty", { propID }, function (res) {
                $Childproperties.clearRows();
                $Childproperties.bindRows(res);
                if (isValidArray(dataCPD)) {
                    dataCPD = [];
                    dataCPD.push(res);
                } else {
                    dataCPD.push(res);
                }
            })
        })
        dialog.reject(function () {
            dialog.shutdown();
        })


    }
    function updateCPD(data, id, prop, value) {
        const res = data[0];
        if (isValidArray(res)) {
            $.grep(res, function (item) {
                if (item.ID === id) {
                    item[prop] = value;
                }
            });
        }
    }
    function createCPD(dialog) {
        const propId = $("#CPDID").val();
        const name = $("#nameCPD").val();
        const data = {
            ProID: propId,
            Name: name
        }
        $.post("/Property/CreateCPD", { childPreoperty: data }, function (res) {
            if (res.IsApproved) {
                new ViewMessage({
                    summary: {
                        selector: ".successCPD"
                    }
                }, res);
                setTimeout(function () { dialog.shutdown(); }, 700);
            }
            if (res.IsRejected) {
                new ViewMessage({
                    summary: {
                        selector: "#errorCPD"
                    }
                }, res);
            }
        })
    }
    function getProperties(success) {
        $.get("/Property/GetProperties", success);
    }
    function isValidJson(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
});