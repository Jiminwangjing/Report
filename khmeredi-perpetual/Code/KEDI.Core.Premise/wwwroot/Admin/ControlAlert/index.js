$(document).ready(function () {
    let alertM = [];
    const alertMasterView = ViewTable({
        keyField: "ID",
        selector: "#alertMaster",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: true
        },
        visibleFields: [
            "Code",
            "TypeOfAlerts",
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
                        $.post("/ControlAlert/CheckActive", { alertMaster: e.data });
                        // Uddate
                    }
                }
            },
        ],
        actions: [
            {
                template: `<button class="btn btn-kernel">Setting</button>`,
                on: {
                    "click": function (e) {
                        // dialog setting here
                        updateTime(e);
                    }
                }
            },
            {
                template: `<i class="fas fa-plus-circle cursor"></i>`,
                on: {
                    "click": function (e) {
                        // dialog setting here

                        updateDetails(e);
                    }
                }
            },
            {
                template: `<button class="btn btn-kernel">View Details</button>`,
                on: {
                    "click": function (e) {
                        location.href = `/ControlAlert/AlertDetail?id=${e.data.ID}`;
                    }
                }
            },
        ]
    });
    getAlertMaster(function (res) {
        alertMasterView.bindRows(res);
    });

    function updateTime(e) {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Save",
                }
            },
            caption: "Setting Master",
            content: {
                selector: "#container-setting"
            },
            type: "ok/cancel"
        });

        dialog.invoke(function () {
            if (e.data.TypeOfAlert == 1/* || e.data.TypeOfAlert == 2*/) {
                $("#timesbeforeAppointment").hide();
            }            
            else {
                $("#timesbeforeAppointment").show();
            }
            if (e.data.TypeOfAlert == 5 || e.data.TypeOfAlert == 6) {
                dialog.content.find("#setting").hide();
                dialog.content.find("#noSetting").show();
            } else {
                dialog.content.find("#noSetting").hide();
            }
            const fqappinput = dialog.content.find("#fq-app-input");
            const bfappinput = dialog.content.find("#bf-app-input");
            const fqappseelct = dialog.content.find("#fq-app-select");
            const bfappseelct = dialog.content.find("#bf-app-select");
            bfappinput.val(e.data.BeforeAppDate);
            bfappseelct.val(e.data.TypeBeforeAppDate);
            fqappinput.val(e.data.Frequently);
            fqappseelct.val(e.data.TypeFrequently);
            fqappinput.keyup(function () {
                e.data.Frequently = this.value;
            });
            bfappinput.keyup(function () {
                e.data.BeforeAppDate = this.value;
            });
            fqappseelct.change(function () {
                e.data.Frequently = 1;
                fqappinput.val(1);
                e.data.TypeFrequently = this.value;
            });
            bfappseelct.change(function () {
                e.data.BeforeAppDate = 1;
                bfappinput.val(1);
                e.data.TypeBeforeAppDate = this.value;
            });
        });
        dialog.confirm(function () {
            $.post("/ControlAlert/CheckActive", { alertMaster: e.data });
            dialog.shutdown();
        });
        dialog.reject(function () {
            dialog.shutdown();
        });
    }
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
            bindWHUser(dialog, e);
        });
        checkAll();
        dialog.confirm(function () {
            alertM[0].Code = dialog.content.find("#d-code").val();
            alertM[0].Name = dialog.content.find("#d-name").val();
            $.post("/ControlAlert/UpdateAlertDetails", { alertDetail: alertM[0] }, function (res) {
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
    function getAlertMaster(success) {
        $.get("/ControlAlert/GetAlertMasters", success);
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
    function bindWHUser(dialog, e) {
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
                            updateData(alertM[0].AlertWarehouses, "WarehouseID", e.key, "IsAlert", isChecked);
                            const isCheckAll = alertM[0].AlertWarehouses.filter(i => i.IsAlert);
                            if (isCheckAll.length == alertM[0].AlertWarehouses.length) {
                                $("#checkAllWh").prop("checked", true);
                                alertM[0].IsAllWh = true;
                            }
                            else if (isCheckAll.length < alertM[0].AlertWarehouses.length) {
                                $("#checkAllWh").prop("checked", false);
                                alertM[0].IsAllWh = false;
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
                            updateData(alertM[0].UserAlerts, "UserAccountID", e.key, "IsAlert", isChecked);
                            const isCheckAll = alertM[0].UserAlerts.filter(i => i.IsAlert);
                            if (isCheckAll.length == alertM[0].UserAlerts.length) {
                                $("#checkAllUser").prop("checked", true);
                                alertM[0].IsAllUsers = true;
                            }
                            else if (isCheckAll.length < alertM[0].UserAlerts.length) {
                                $("#checkAllUser").prop("checked", false);
                                alertM[0].IsAllUsers = false;
                            }
                        }
                    }
                }
            ],
        });
        $.get("/ControlAlert/GetWHUser", { alertM: e.data.ID }, function (res) {
            whAlertView.bindRows(res.AlertWarehouses);
            userAlertView.bindRows(res.UserAlerts);
            if (isValidArray(alertM)) {
                alertM = [];
                alertM.push(res);
            } else {
                alertM.push(res);
            }
            alertM[0].IsAllWh = false;
            alertM[0].IsAllUsers = false;
        });
    }
    function checkAll() {
        $("#checkAllWh").click(function () {
            if ($(this).prop("checked")) {
                $(".whCheck").prop("checked", true);
                alertM[0].IsAllWh = true;
                if (isValidArray(alertM[0].AlertWarehouses)) {
                    alertM[0].AlertWarehouses.forEach((item, index) => {
                        updateData(alertM[0].AlertWarehouses, "WarehouseID", item.WarehouseID, "IsAlert", true);
                    });
                }
            } else {
                $(".whCheck").prop("checked", false);
                alertM[0].IsAllWh = false;
                alertM[0].AlertWarehouses.forEach((item, index) => {
                    updateData(alertM[0].AlertWarehouses, "WarehouseID", item.WarehouseID, "IsAlert", false);
                });
            }
        });
        $("#checkAllUser").click(function () {
            if ($(this).prop("checked")) {
                $(".Checkuser").prop("checked", true);
                alertM[0].IsAllUsers = true;
                if (isValidArray(alertM[0].UserAlerts)) {
                    alertM[0].UserAlerts.forEach((item, index) => {
                        updateData(alertM[0].UserAlerts, "UserAccountID", item.UserAccountID, "IsAlert", true);
                    });
                }
            } else {
                $(".Checkuser").prop("checked", false);
                alertM[0].IsAllUsers = false;
                alertM[0].UserAlerts.forEach((item, index) => {
                    updateData(alertM[0].UserAlerts, "UserAccountID", item.UserAccountID, "IsAlert", false);
                });
            }
        });
    }
});