$(document).ready(function () {
    const data = JSON.parse($(".data").text());
    const alertMasterView = ViewTable({
        keyField: "ID",
        selector: "#alert-detail",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: true
        },
        visibleFields: [
            "Code",
            "Name",
            "Active",
        ],
        columns: [
            {
                name: "Code",
                on: {
                    "dblclick": function (e) {
                        location.href = `/ControlAlert/Edit?id=${e.data.ID}`;
                    }
                }
            },
            {
                name: "TypeOfAlerts",
                template: `<select disabled></select>`
            },
            {
                name: "Active",
                template: `<input type="checkbox">`,
                on: {
                    "click": function (e) {
                        const isActive = $(this).prop("checked") ? true : false;
                        e.data.Active = isActive;
                        $.post("/ControlAlert/CheckActiveDetail", { id: e.data.ID, active: e.data.Active });
                        // Uddate
                    }
                }
            },
        ],
        actions: [
            {
                template: `<i class="fas fa-edit cursor"></i>`,
                on: {
                    "click": function (e) {
                        updateDetails(e);
                    }
                }
            },
        ]
    });
    alertMasterView.bindRows(data);
    function updateDetails(e) {

        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Save",
                }
            },
            caption: "Detail Master",
            content: {
                selector: "#container-detail"
            },
            type: "ok/cancel"
        });

        dialog.invoke(function () {
            if (e.data.TypeOfAlert == 1 || e.data.TypeOfAlert == 2) {
                $("#warehouse").show();
            } else {
                $("#warehouse").hide();
            }
            dialog.content.find("#d-code").val(e.data.Code);
            dialog.content.find("#d-name").val(e.data.Name);
            bindWHUser(dialog, e);
        });
        checkAll(e);
        dialog.confirm(function () {
            e.data.Code = dialog.content.find("#d-code").val();
            e.data.Name = dialog.content.find("#d-name").val();
            $.post("/ControlAlert/UpdateAlertDetails", { alertDetail: e.data }, function (res) {
                if (res.IsApproved) {
                    ViewMessage({
                        summary: {
                            selector: dialog.content.find(".detail-message")
                        }
                    }, res).refresh(1000);
                } else {
                    ViewMessage({
                        summary: {
                            selector: dialog.content.find(".detail-message")
                        }
                    }, res);
                }
            });
        });
        dialog.reject(function () {
            dialog.shutdown();
        });
    }
    function bindWHUser(dialog, _e) {
        const whAlertView = ViewTable({
            keyField: "WarehouseID",
            selector: dialog.content.find("#d-wh"),
            indexed: true,
            paging: {
                enabled: true,
                pageSize: 10
            },
            visibleFields: [
                "Code",
                "Name",
                "Location",
                "IsAlert"
            ],
            columns: [
                {
                    name: "IsAlert",
                    template: '<input type="checkbox" class="whCheck">',
                    on: {
                        "click": function (e) {
                            const isChecked = $(this).prop("checked") ? true : false;
                            updateData(_e.data.AlertWarehouses, "WarehouseID", e.key, "IsAlert", isChecked);
                            const isCheckAll = _e.data.AlertWarehouses.filter(i => i.IsAlert);
                            if (isCheckAll.length == _e.data.AlertWarehouses.length) {
                                $("#checkAllWh").prop("checked", true);
                                _e.data.IsAllWh = true;
                            }
                            else if (isCheckAll.length < _e.data.AlertWarehouses.length) {
                                $("#checkAllWh").prop("checked", false);
                                _e.data.IsAllWh = false;
                            }
                        }
                    }
                }
            ],
        });
        const userAlertView = ViewTable({
            keyField: "UserAccountID",
            selector: dialog.content.find("#d-user"),
            indexed: true,
            paging: {
                enabled: true,
                pageSize: 10
            },
            visibleFields: [
                "UserName",
                "TelegramUserID",
                "IsAlert",
            ],
            columns: [
                {
                    name: "IsAlert",
                    template: '<input type="checkbox" class="Checkuser">',
                    on: {
                        "click": function (e) {
                            const isChecked = $(this).prop("checked") ? true : false;
                            updateData(_e.data.UserAlerts, "UserAccountID", e.key, "IsAlert", isChecked);
                            const isCheckAll = _e.data.UserAlerts.filter(i => i.IsAlert);
                            if (isCheckAll.length == _e.data.UserAlerts.length) {
                                $("#checkAllUser").prop("checked", true);
                                _e.data.IsAllUsers = true;
                            }
                            else if (isCheckAll.length < _e.data.UserAlerts.length) {
                                $("#checkAllUser").prop("checked", false);
                                _e.data.IsAllUsers = false;
                            }
                        }
                    }
                }
            ],
        });
        $("#checkAllUser").prop("checked", _e.data.IsAllUsers);
        $("#checkAllWh").prop("checked", _e.data.IsAllWh);
        whAlertView.bindRows(_e.data.AlertWarehouses);
        userAlertView.bindRows(_e.data.UserAlerts);
    }
    function checkAll(e) {
        $("#checkAllWh").click(function () {
            if ($(this).prop("checked")) {
                $(".whCheck").prop("checked", true);
                e.data.IsAllWh = true;
                if (isValidArray(e.data.AlertWarehouses)) {
                    e.data.AlertWarehouses.forEach((item, index) => {
                        updateData(e.data.AlertWarehouses, "WarehouseID", item.WarehouseID, "IsAlert", true);
                    });
                }
            } else {
                $(".whCheck").prop("checked", false);
                e.data.IsAllWh = false;
                e.data.AlertWarehouses.forEach((item, index) => {
                    updateData(e.data.AlertWarehouses, "WarehouseID", item.WarehouseID, "IsAlert", false);
                });
            }
        });
        $("#checkAllUser").click(function () {
            if ($(this).prop("checked")) {
                $(".Checkuser").prop("checked", true);
                e.data.IsAllUsers = true;
                if (isValidArray(e.data.UserAlerts)) {
                    e.data.UserAlerts.forEach((item, index) => {
                        updateData(e.data.UserAlerts, "UserAccountID", item.UserAccountID, "IsAlert", true);
                    });
                }
            } else {
                $(".Checkuser").prop("checked", false);
                e.data.IsAllUsers = false;
                e.data.UserAlerts.forEach((item, index) => {
                    updateData(e.data.UserAlerts, "UserAccountID", item.UserAccountID, "IsAlert", false);
                });
            }
        });
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function updateData(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue) {
                    i[prop] = propValue;
                }
            });
        }
    }
});