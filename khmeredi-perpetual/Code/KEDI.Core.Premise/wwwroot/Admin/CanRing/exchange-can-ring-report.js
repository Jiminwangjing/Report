$(document).ready(function () {
    const date = new Date();
    $("#date-from")[0].valueAsDate = date;
    $("#date-to")[0].valueAsDate = date;
    $("#branch-id").change(function () {
        $.get("/canring/GetUserAndWarehouse", { branchId: this.value }, function (res) {
            $("#warehouse-id option").remove()
            $("#user option").remove()
            if (res) {
                let user = `<option value=0></options>`
                res.users.forEach(i => {
                    user += `<option value="${i.ID}">${i.Username}</options>`
                })
                $("#user").append(user);

                let warehouse = `<option value=0></options>`
                res.warehouse.forEach(i => {
                    warehouse += `<option value="${i.ID}">${i.Name}</options>`
                })
                $("#warehouse-id").append(warehouse);
            }
        })
    })
    $("#choose-vendor").click(function () {
        const vendorDialog = new DialogBox({
            content: {
                selector: ".vendor-container-list"
            },
            caption: "Vendor Lists",
            type: "ok/cancel",
            button: {
                ok: {
                    text: "Reset"
                },
                cancel: {
                    text: "Close"
                }
            }
        });
        vendorDialog.invoke(function () {
            const venTable = ViewTable({
                keyField: "ID",
                selector: $("#list-vendor"),
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name", "Type", "Phone"],
                actions: [
                    {
                        template: `<i class="fa fa-arrow-alt-circle-down fa-lg csr-pointer"></i>`,
                        on: {
                            "click": function (e) {
                                $("#vendor-name").val(e.data.Name);
                                $("#vendor-id").val(e.data.ID);
                                vendorDialog.shutdown();
                            }
                        }
                    }
                ]
            });
            //GetVendors
            $.get("/CanRing/GetVendors", function (res) {
                venTable.bindRows(res);
                $("#find-vendor").on("keyup", function (e) {
                    let __value = this.value.toLowerCase().replace(/\s+/, "");
                    let rex = new RegExp(__value, "gi");
                    let __vendors = $.grep(res, function (person) {
                        return person.Code.match(rex) || person.Name.toLowerCase().replace(/\s+/, "").match(rex)
                            || person.Phone.toLowerCase().replace(/\s+/, "").match(rex)
                            || person.Type.match(rex);
                    });
                    venTable.bindRows(__vendors);
                });
            })

        });
        vendorDialog.confirm(function () {
            $("#vendor-name").val("");
            $("#vendor-id").val(0);
            vendorDialog.shutdown();
        })
        vendorDialog.reject(function () {
            vendorDialog.shutdown();
        })

    })
    const historyTable = ViewTable({
        keyField: "ID",
        selector: "#history-list",
        indexed: true,
        paging: {
            pageSize: 20,
            enabled: true
        },
        visibleFields: ["DocCode", "Number", "CreatedAt", "VendorName", "UserName", "WarehouseName", "PriceList", "PaymentMeans", "TotalDis"],
        columns: [
            {
                name: "CreatedAt",
                dataType: "date",
                dataFormat: "DD-MM-YYYY"
            },
            {
                name: "TotalDis",
                dataType: "number",
                dataFormat: { fixed: 2 }
            }
        ],
        actions: [
            {
                template: `<i class="fas fa-print fa-lg csr-pointer"></i>`,
                on: {
                    "click": function (e) {
                        window.open(`/canring/printexchangecanringhistory?id=${e.key}`, "_blank");
                    }
                }
            }
        ]
    });
    getHistory();
    $("#filtering").click(function () {
        getHistory()
    })
    function getHistory() {
        const param = {
            BranchID: $("#branch-id").val(),
            UserID: $("#user").val(),
            VendorID: $("#vendor-id").val(),
            PriceListID: $("#plid").val(),
            WarehouseID: $("#warehouse-id").val(),
            PaymentMeansID: $("#pmid").val(),
            DateFrom: $("#date-from").val(),
            DateTo: $("#date-to").val(),
        }
        $.get("/canring/getexchangecanringhistory", { param: JSON.stringify(param) }, function (res) {
            historyTable.clearRows();
            historyTable.bindRows(res);
            $("#search-history").on("keyup", function (e) {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let rex = new RegExp(__value, "gi");
                let lists = $.grep(res, function (person) {
                    return person.DocCode.match(rex) || person.Number.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.CreatedAt.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.VendorName.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.UserName.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.PriceList.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.PaymentMeans.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.TotalDis.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.WarehouseName.toLowerCase().replace(/\s+/, "").match(rex);
                });
                historyTable.bindRows(lists);
            });
        })
    }
})