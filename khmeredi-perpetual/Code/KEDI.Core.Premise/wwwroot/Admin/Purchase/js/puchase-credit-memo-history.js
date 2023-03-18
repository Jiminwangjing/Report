

    const BranchID = parseInt($("#BranchID").text());
    let _postingdate = $("#txtpostingdate");
    let _documentDate = $("#txtDocumentDate");
    _postingdate[0].valueAsDate = new Date();
    _documentDate[0].valueAsDate = new Date();
    GetPurchaseMemoReport();
    //get warehouse by branch
    $.ajax({
        url: "/PurchaseCreditMemo/GetWarehouses",
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
        url: "/PurchaseCreditMemo/GetBusinessPartners",
        type: "GET",
        dataType: "JSON",
        dta: {
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
    })
const _printUrl = JSON.parse($("#print-template-url").text()).Url;
    console.log("ff", _printUrl);

    $("#btn-fillter").click(() => {
        GetPurchaseMemoReport();
    })
    function GetPurchaseMemoReport() {
        let vendor = $("#txtvendor").val();
        let warehouse = $("#txtwarehouse").val();
        let postingdate = $("#txtpostingdate").val();
        let documentDate = $("#txtDocumentDate").val();
        let delivery = $("#txtDelivery").val();
        let Search = $("#txtSearch").val();
        let check = $("#checkAllItem").prop('checked') ? true : false;
        $.get("/PurchaseCreditMemo/GetPurchaseMemoReport", { BranchID: BranchID, WarehouseID: warehouse, vendorID: vendor, PostingDate: postingdate, DocumentDate: documentDate, DeliveryDate: delivery, Search, Check: check }, function (res) {

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
                    name: "Cancele",
                    //template: `<input class='form-control font-size disvalue' type='text' />`,
                    on: {
                        "click": function (e) {
                            if (e.data.Status == "open") {
                                Cancel(e.data.ID);
                            }
                        }
                    }
                },

            ],
            columns: [
                {
                    name: "VatType",
                    template: `<select>`,
                    on: {
                        "change": function (e) {
                            console.log("ff", this.value);
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
                            console.log(e.data)
                            let purchaseid = e.data.ID;
                            let vatnumber = parseInt(e.data.TypeVatNumber)
                            if (vatnumber >= 1) {
                                window.open(_printUrl + "Home/PrintPurchaseMemo/?purchaseid=" + purchaseid + "", "_blank");
                            }
                            else {

                                window.open("/Print/PrintPurchaseCreditMemo?PurchaseID=" + purchaseid + "", "_blank");
                            }
                        }
                    }
                }
            ]

        });
        $listItemChoosed.bindRows(data);
    }


    //Cancel PO
    //function Cancel(id) {

    //    let msg = new DialogBox({
    //        type: "ok-cancel",
    //        content: "Do you want to cancel?"
    //    });
    //    msg.confirm(function (e) {

    //        $.ajax({
    //            url: '/PurchaseQuotation/PQCancel',
    //            type: 'POST',
    //            dataType: 'JSON',
    //            data: { PurchaseID: id },
    //            success: function () {

    //            }
    //        });
    //        location.reload();
    //    });

    //}
