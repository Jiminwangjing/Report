$(document).ready(function () {
    let __data = {};
    $("#cancel-data").click(function () {
        location.reload();
    })
    $("#item-code").on("keypress", function (e) {
        if (e.which === 13) {
            $.get(
                '/ItemBatchNo/GetBatchNoDetails',
                {
                    itemcode: this.value.trim()
                },
                function (res) {
                    responce(res);
                }
            )
        }
    });
    $("#save-data").click(function () {
        setData();
        $.post(
            "/ItemBatchNo/UpdateBatchNoDetial",
            {
                batchDetail: __data
            },
            function (res) {
                if (res.IsRejected) {
                    new ViewMessage({
                        summary: {
                            selector: ".batch-message"
                        }
                    }, res);
                } else {
                    //$(".serial-message").text("");
                    new ViewMessage({
                        summary: {
                            selector: ".batch-message"
                        }
                    }, res);
                    location.reload();
                }
            }
        )
    })
    $(".choose-item").click(function () {
        const itemDialog = new DialogBox({
            content: {
                selector: ".item-lists"
            },
            caption: "Item Master Data",
            type: "ok",
            button: {
                ok: {
                    text: "Close"
                }
            }
        });
        itemDialog.invoke(function () {
            const itemTable = ViewTable({
                keyField: "ID",
                selector: ".item-table",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: false
                },
                visibleFields: ["Code", "Barcode", "KhmerName"],
                actions: [
                    {
                        template: `<i class='fas fa-arrow-circle-down' style="cursor:pointer"></i>`,
                        on: {
                            "click": function (e) {
                                $("#item-code").val(e.data.Code);
                                $.get(
                                    '/ItemBatchNo/GetBatchNoDetails',
                                    {
                                        itemcode: e.data.Code
                                    },
                                    function (res) {
                                        responce(res);
                                    }
                                )
                                itemDialog.shutdown();
                            }
                        }
                    }
                ]
            });
            $.get("/ItemBatchNo/GetItemMasterData", function (res) {
                itemTable.bindRows(res);
            });
        })
        itemDialog.confirm(function () {
            itemDialog.shutdown();
        })
    })
    function responce(res) {
        if (res.IsRejected) {
            new DialogBox({
                content: res.Data.item,
                icon: "warning"
            });
        } else {
            const batchNoDetial = new DialogBox({
                content: {
                    selector: "#dialog-batch-no-detial"
                },
                caption: "Choose Batch No",
            });
            batchNoDetial.invoke(function () {
                const serial = ViewTable({
                    keyField: "LineID",
                    selector: ".batch-no-detial",
                    indexed: true,
                    paging: {
                        pageSize: 10,
                        enabled: false
                    },
                    visibleFields: ["ItemCode", "ItemName", "Batch", "Location", "Details"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down cursor"></i>`,
                            on: {
                                "click": function (e) {
                                    bindDetials(e.data);
                                    batchNoDetial.shutdown();
                                }
                            }
                        }
                    ]
                });
                serial.bindRows(res)
            });
            batchNoDetial.confirm(function () {
                batchNoDetial.shutdown();
            })
        }
    }
    function bindDetials(data) {
        if (data.ExpirationDate.toString() === "0001-01-01T00:00:00") data.ExpirationDate = null;
        $("#item-name").val(data.ItemName);
        $("#warehouse-name").val(data.WhsName);
        $("#batch-attribute-2").val(data.BatchAttribute2);
        $("#batch").val(data.Batch);
        $("#batch-attribute-1").val(data.BatchAttribute1);
        $("#batch-no-og").val(data.BatchOG);
        setDate("#admission-date", data.AdmissionDate?.toString().split("T")[0]);
        setDate("#manufacturing-date", data.ManufacturingDate?.toString().split("T")[0]);
        setDate("#expiration-date", data.ExpirationDate?.toString().split("T")[0]);
        $("#location").val(data.Location);
        $("#detials").val(data.Details);

    }
    function setData() {
        __data.ItemName = $("#item-name").val();
        __data.WhsName = $("#warehouse-name").val();
        __data.BatchAttribute2 = $("#batch-attribute-2").val();
        __data.Batch = $("#batch").val();
        __data.BatchAttribute1 = $("#batch-attribute-1").val();
        __data.AdmissionDate = $("#admission-date").val();
        __data.ManufacturingDate = $("#manufacturing-date").val();
        __data.ExpirationDate = $("#expiration-date").val() === "" ? "0001-01-01T00:00:00" : $("#expiration-date").val();
        __data.Location = $("#location").val();
        __data.Details = $("#detials").val();
        __data.BatchOG = $("#batch-no-og").val();
    }
    function setDate(selector, date_value) {
        var __date = $(selector);
        __date[0].valueAsDate = new Date(date_value);
        __date[0].setAttribute(
            "data-date",
            moment(__date[0].value)
                .format(__date[0].getAttribute("data-date-format"))
        );
    }
});