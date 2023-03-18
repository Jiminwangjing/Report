

    const BranchID = parseInt($("#BranchID").text());
    let _postingdate = $("#txtpostingdate");
    let _documentDate = $("#txtDocumentDate");
    _postingdate[0].valueAsDate = new Date();
    _documentDate[0].valueAsDate = new Date();
    GetPurchaseAPReserveReport();
    //GetWarehouse
    $.ajax({
        url: "/PurchaseAP/GetWarehouses",
        type: "GET",
        dataType: "JSON",
        data: {
            ID: BranchID
        },
        success: function (e) {
            var data = "";
            $.each(e, function (i, item) {
                data +=
                    '<option value="' + item.ID + '">' + item.Name + '</option>';
            })
            $(".select_warehouse").append(data);
        }
    });
    $.ajax({
        url: "/PurchaseAP/GetBusinessPartners",
        type: "Get",
        dataType: "JSON",
        data: {
            ID: BranchID
        },
        success: function (e) {
            var data = "";
            $.each(e, function (i, item) {
                data +=
                    '<option value="' + item.ID + '">' + item.Name + '</option>';
            })
            $(".select_vender").append(data);
        }
    });

    const _printUrl = JSON.parse($("#print-template-url").text()).Url;
    console.log("_printUrl", _printUrl);

    $("#btn-fillter").click(() => {
        GetPurchaseAPReserveReport();
    })
    function GetPurchaseAPReserveReport() {
        let Vendor = $("#txtvendor").val();
        let warehouse = $("#txtwarehouse").val();
        let postingdate = $("#txtpostingdate").val();
        let documentDate = $("#txtDocumentDate").val();
        let delivery = $("#txtDelivery").val();
        let Search = $("#txtSearch").val();
        let check = $("#checkAllItem").prop('checked') ? true : false;

        console.log("cc", Vendor);
        console.log("dd", warehouse);

        $.get("/PurchaseAP/GetPurchaseAPReserveReport", { BranchID: BranchID, warehouseID: warehouse, VendorID: Vendor, PostingDate: postingdate, DocumentDate: documentDate, DeliveryDate: delivery, Search, Check: check }, function (res) {

            res.forEach(i => {
                if (i.Status == "close") {
                    i.Cancele = "<i style=color:#f2e8c5;' class= 'fa fa-ban' ></i>";
                    i.Status = "<span style='color:red;text-align:center;'>close</span>";
                }
            })

            ListReport(res);
        })
    }
    function ListReport(data) {

        const $listItemChoosed = ViewTable({
            keyField: "ID",
            selector: $("#item-list"),
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: false
            },
            dynamicCol: {
                afterAction: true,
                headerContainer: "#col-to-append-after-detail",
            },
            visibleFields: [
                "Invoice", "VendorName", "UserName", "Balance", "ExchangeRate", "Status", "VatType"

            ],

            columns: [
                {
                    name: "VatType",
                    template: `<select>`,
                    on: {
                        "change": function (e) {
                            console.log("ff", this.value);
                            console.log(e)
                            $listItemChoosed.updateColumn(e.data.ID, "TypeVatNumber", this.value);
                        }
                    }
                },
            ],
            actions: [
                {
                    template: `<i class="fa fa-print font-size hover"></i>`,
                    on: {
                        "click": function (e) {
                            let vatnumber = parseInt(e.data.TypeVatNumber)
                            let purchaseId = e.data.ID;
                            if (vatnumber >= 1) {

                                window.open(_printUrl + "Home/PrintPurchaseAPReserve/?purchaseId=" + purchaseId + "", "_blank");
                            }
                            else {
                                window.open("/Print/PrintPurchaseAPReserve?PurchaseID=" + purchaseId + "", "_blank");
                            }
                        }
                    }
                }
            ]
        });
        $listItemChoosed.bindRows(data);
    }
    //Cancel PO
    function Cancel(id) {

        let msg = new DialogBox({
            type: "ok-cancel",
            content: "Do you want to cancel?"
        });
        msg.confirm(function (e) {

            $.ajax({
                url: '/PurchaseQuotation/APReserveCancel',
                type: 'POST',
                dataType: 'JSON',
                data: { PurchaseID: id },
                success: function () {

                }
            });
            location.reload();
        });

    }
