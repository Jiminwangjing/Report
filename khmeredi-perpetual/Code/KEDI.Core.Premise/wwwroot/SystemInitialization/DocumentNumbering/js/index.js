$(document).ready(function () {
    var _data = JSON.parse($("#data").text());
    let config = {
        master: {
            value: _data,
        }
    }
    const coreMaster = new CoreItemMaster(config);
    $("#txtSearch").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#allw-gl tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    
    $("#hidetr").dblclick(function () {
        $(this).hide();
    });
    $("#showtr").dblclick(function () {
        $(hidetr).show();
    });

    function onGetWhAccTemplates(success) {
        $.get("/DocNumbering/GetAllDocNumbering", success);
    }
    let $listJE = ViewTable({
        keyField: "LineID",
        selector: "#allw-gl",
        paging: {
            enabled: false,
            pageSize: 20
        },
        visibleFields: [
            "LineID",
            "Code",
            "Document",
            "DefaultSeries",
            "FirstNo",
            "NextNo",
            "LastNo"
        ],
        columns: [
            {
                name: "LineID",
                on: {
                    "dblclick": function (e) {
                        const id = e.data.ID;
                        chooseData(id);
                        $(".wrap-button :nth(1)").prop("disabled", true);
                        $(".wrap-button :nth(0)").prop("disabled", true);
                    }
                }
            }
        ],
    });

    function chooseData(id) {
        let dialog = new DialogBox({
            button: {
                cancel: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    },
                },
                no: {
                    text: "Set Default",
                    callback: function () {
                        const id = $("#idSeries").val();
                        const idDocNum = $("#idDocNum").val();
                        $.post("/DocNumbering/SetSeriesDefault", { id, idDocNum }, function (res) {                           
                            if (res.Action == 1) {
                                new ViewMessage({
                                    summary: {
                                        selector: "#error-summary",
                                    }
                                }, res);
                                setTimeout(function () {
                                    location.reload();
                                }, 500);
                                $("#idSeries").val(0);
                                $("#idDocNum").val(0);
                            } else {
                                new ViewMessage({
                                    summary: {
                                        selector: "#error-summary",
                                    }
                                }, res);
                                $("#idSeries").val(0);
                                $("#idDocNum").val(0);
                            }
                        });
                    }
                },
                yes: {
                    text: "Save",
                    callback: function (e) {
                        if (parseInt($("#idSeriesUpdate").val()) == 0) {
                            coreMaster.updateMaster("ID", 0);
                            coreMaster.updateMaster("DocuTypeID", id);
                            coreMaster.submitData("/DocNumbering/CreateSeries", function (res) {
                                if (res.Action == 1) {
                                    new ViewMessage({
                                        summary: {
                                            selector: "#error-summary"
                                        }
                                    }, res).refresh(1000);
                                } else {
                                    new ViewMessage({
                                        summary: {
                                            selector: "#error-summary"
                                        }
                                    }, res);
                                }
                            });
                        } else {
                            coreMaster.updateMaster("DocuTypeID", id);
                            const periodIndecator = dialog._content.find("#periodIndecator").val();
                            coreMaster.updateMaster("PeriodIndID", parseInt(periodIndecator));
                            coreMaster.submitData("/DocNumbering/CreateSeries", function (res) {
                                if (res.Action == 1) {
                                    new ViewMessage({
                                        summary: {
                                            selector: "#error-summary"
                                        }
                                    }, res).refresh(1000);
                                } else {
                                    new ViewMessage({
                                        summary: {
                                            selector: "#error-summary"
                                        }
                                    }, res);
                                }
                            });
                        }

                    },

                }
            },
            type: "yes-no-cancel",
            content: {
                selector: "#list-series-content",
            }
        });
        dialog.invoke(function () {
            let $listActiveGL = ViewTable({
                keyField: "LineID",
                selector: dialog.content.find("#list-series"),
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: [
                    "LineID",
                    "Name",
                    "FirstNo",
                    "NextNo",
                    "LastNo",
                    "Prefix",
                    "PeriodIndecator",
                    "Lock",
                ],

                columns: [
                    {
                        name: "LineID",
                        on: {
                            "dblclick": function (e) {
                                $("#idSeries").val(e.data.ID);
                                $("#idDocNum").val(id);
                                $(".wrap-button :nth(1)").prop("disabled", false);
                            }
                        }
                    },
                    {
                        name: "Name",
                        template: "<input type='text' class='disabled'>",
                        on: {
                            "keyup": function () {
                                const name = $(this).val();
                                coreMaster.updateMaster("Name", name);
                            }
                        }
                    },
                    {
                        name: "FirstNo",
                        template: "<input type='number' class='disabled firstNo'>",
                        on: {
                            "keyup": function (e) {
                                const firstNo = $(this).val();
                                if (parseInt($("#idSeriesUpdate").val()) > 0) {
                                    $(this).parent().parent().children().children("input:nth(2)").val(firstNo);
                                } else {
                                    const inputLength = dialog._content.find(".nextNo").length - 1;
                                    dialog._content.find(`.nextNo:nth(${inputLength})`).val(firstNo);
                                }                                
                                coreMaster.updateMaster("FirstNo", firstNo);
                                coreMaster.updateMaster("NextNo", firstNo);
                            }
                        }
                    },
                    {
                        name: "NextNo",
                        template: "<input type='number' class='disabled nextNo' readonly disabled>",
                    },
                    {
                        name: "LastNo",
                        template: "<input type='text'  class='disabled lastNo'>",
                        on: {
                            "keyup": function (e) {
                                const lastNo = $(this).val();
                                coreMaster.updateMaster("LastNo", lastNo);
                            }
                        }
                    },
                    {
                        name: "PreFix",
                        template: "<input type='text'  class='disabled'>",
                        on: {
                            "keyup": function (e) {
                                const prefix = $(this).val();
                                coreMaster.updateMaster("PreFix", prefix);
                            }
                        }
                    },
                    {
                        name: "Lock",
                        template: "<input type='checkbox' class='disabled'>",
                        on: {
                            "click": function (e) {
                                if ($(this).prop("checked") == true) {
                                    coreMaster.updateMaster("Lock", true);
                                }
                                if ($(this).prop("checked") == false) {
                                    coreMaster.updateMaster("Lock", false);
                                }
                            }
                        }
                    },
                    {
                        name: "PeriodIndecator",
                        template: "<select id='periodIndecator' class='disabled'>",
                        on: {
                            "change": function (e) {
                                coreMaster.updateMaster("PeriodIndID", parseInt(this.value));
                            }
                        }
                    }
                ],
                actions: [
                    {
                        template: `<i class="fas fa-edit" style="cursor:pointer;"></i>`,
                        on: {
                            "click": function (e) {
                                for (let i = 3; i < 6; i++) {
                                    if (e.data.FirstNo !== e.data.NextNo)
                                        $(e.row).children(`td:nth(${i})`).children().prop("readonly", true);
                                }

                                //if (e.data.FirstNo === e.data.NextNo) {
                                //    $(".firstNo").prop("disabled", false);
                                //    $(".lastNo").prop("disabled", false);
                                //}

                                $("#idSeriesUpdate").val(e.data.ID);
                                $(".wrap-button :nth(1)").prop("disabled", true);
                                $(".wrap-button :nth(0)").prop("disabled", false);
                                $(`#list-series tr:nth(${e.key}) td .disabled`).prop("disabled", false);
                                coreMaster.updateMaster("Default", e.data.Default);
                                coreMaster.updateMaster("ID", e.data.ID);
                                coreMaster.updateMaster("FirstNo", e.data.FirstNo);
                                coreMaster.updateMaster("DocuTypeID", e.data.DocuTypeID);
                                coreMaster.updateMaster("LastNo", e.data.LastNo);
                                coreMaster.updateMaster("Lock", e.data.Lock);
                                coreMaster.updateMaster("Name", e.data.Name);
                                coreMaster.updateMaster("NextNo", e.data.NextNo);
                                coreMaster.updateMaster("PeriodIndID", e.data.PeriodIndID);
                                coreMaster.updateMaster("PreFix", e.data.PreFix);
                            }
                        }
                    }
                ]
            });

            $.get("/DocNumbering/GetAllSeries", { id }, function (resp) {               
                $listActiveGL.clearRows();
                $listActiveGL.bindRows(resp);
                $(".disabled").prop("disabled", true);
            });

            $("#add-new-series").click(function () {
                $(".wrap-button :nth(0)").prop("disabled", false);
                $(".fa-edit").prop("disabled", true);
                $("#action").prop("disabled", true);
                $(this).hide()
                $("#idSeriesUpdate").val(0);
                $.get("/DocNumbering/GetOneCreate", { id }, function (resp) {
                    $listActiveGL.addRow(resp);
                    $(".fa-edit").remove();
                });
            });
        });
    }
    
    onGetWhAccTemplates(function (postingperiod) {
        $listJE.clearRows();
        $listJE.bindRows(postingperiod);
    });
})