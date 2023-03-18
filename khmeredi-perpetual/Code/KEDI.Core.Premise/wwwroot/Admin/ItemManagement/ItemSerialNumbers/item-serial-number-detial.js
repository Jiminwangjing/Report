$(document).ready(function () {
    let __data = {};
    $("#cancel-data").click(function () {
        location.reload();
    })
    $("#item-code").on("keypress", function (e) {
        if (e.which === 13) {
            $.get(
                '/ItemSerialNumbers/GetSerialItemDetails',
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
            "/ItemSerialNumbers/UpdateSerialNumberDetial",
            {
                serialDetail: __data
            },
            function (res) {
                if (res.IsRejected) {
                    new ViewMessage({
                        summary: {
                            selector: ".serial-message"
                        }
                    }, res);
                } else {
                    //$(".serial-message").text("");
                    new ViewMessage({
                        summary: {
                            selector: ".serial-message"
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
                                    '/ItemSerialNumbers/GetSerialItemDetails',
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
            $.get("/ItemSerialNumbers/GetItemMasterData", function (res) {
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
            const serialNumberDetial = new DialogBox({
                content: {
                    selector: "#dialog-serial-number-detial"
                },
                caption: "Choose Serial Number",
            });
            serialNumberDetial.invoke(function () {
                const serial = ViewTable({
                    keyField: "LineID",
                    selector: ".serial-number-detial",
                    indexed: true,
                    paging: {
                        pageSize: 10,
                        enabled: false
                    },
                    visibleFields: ["ItemCode", "ItemName", "SerialNumber", "Location", "Details"],
                    actions: [
                        {
                            template: `<i class="fa fa-arrow-alt-circle-down cursor"></i>`,
                            on: {
                                "click": function (e) {
                                    bindDetials(e.data);
                                    serialNumberDetial.shutdown();
                                }
                            }
                        }
                    ]
                });
                serial.bindRows(res)
            });
            serialNumberDetial.confirm(function () {
                serialNumberDetial.shutdown();
            })
        }
    }
    function bindDetials(data) {
        if (data.ExpirationDate.toString() === "0001-01-01T00:00:00") data.ExpirationDate = null;
        $("#item-name").val(data.ItemName);
        $("#warehouse-name").val(data.WhsName);
        $("#mfr-serial-no").val(data.MfrSerialNo);
        $("#serial-number").val(data.SerialNumber);
        $("#lot-number").val(data.LotNumber);
        $("#system-no").val(data.SystemNo);
        $("#serial-number-og").val(data.SerialNumberOG);
        setDate("#admission-date", data.AdmissionDate?.toString().split("T")[0]);
        setDate("#manufacturing-date", data.ManufacturingDate?.toString().split("T")[0]);
        setDate("#expiration-date", data.ExpirationDate?.toString().split("T")[0]);
        setDate("#mfr-warranty-start", data.MfrWarrantyStart?.toString().split("T")[0]);
        setDate("#mfr-warranty-end", data.MfrWarrantyEnd?.toString().split("T")[0]);
        $("#location").val(data.Location);
        $("#detials").val(data.Details);

    }
    function setData() {
        __data.ItemName = $("#item-name").val();
        __data.WhsName = $("#warehouse-name").val();
        __data.MfrSerialNo = $("#mfr-serial-no").val();
        __data.SerialNumber = $("#serial-number").val();
        __data.LotNumber = $("#lot-number").val();
        __data.SystemNo = $("#system-no").val();
        __data.AdmissionDate = $("#admission-date").val();
        __data.ManufacturingDate = $("#manufacturing-date").val();
        __data.ExpirationDate = $("#expiration-date").val() === "" ? "0001-01-01T00:00:00" : $("#expiration-date").val();
        __data.MfrWarrantyStart = $("#mfr-warranty-start").val();
        __data.MfrWarrantyEnd = $("#mfr-warranty-end").val();
        __data.Location = $("#location").val();
        __data.Details = $("#detials").val();
        __data.SerialNumberOG = $("#serial-number-og").val();
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