

    const BranchID = parseInt($("#BranchID").text());
    //get warehouse by branch
    $.ajax({
        url: "/PurchaseRequest/GetWarehouses",
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
        url: "/PurchaseRequest/GetRequesters",
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
            $(".select_vender").append(data);
        }
    });

    const _PrintUrl = JSON.parse($("#print-template-url").text()).Url;
    console.log("_PrintUrl", _PrintUrl);

    $("#btn-fillter").click(() => {
        filter()
    })
    function filter() {
        let Username = $("#txtusername").val();
        let warehouse = $("#txtwarehouse").val();
        let validUntil = $("#valid-until").val();
        let documentDate = $("#document-date").val();
        let requiredDate = $("#required-date").val();
        let search = $("#txtSearch").val();
        let check = $("#checkAllItem").prop('checked') ? true : false;
        $.get("/PurchaseRequest/GetPurchaseRequestReport", { UserID: Username, BranchID: BranchID, WarehouseID: warehouse, validUntil, documentDate, requiredDate, search, Check: check }, function (res) {
            res.forEach(i => {
                if (i.Status == "close") {
                    i.Cancele = "<i style=color:#f2e8c5;' class= 'fa fa-ban' ></i>";
                    i.Status = "<span style='color:red;text-align:center;'>close</span>";
                }
            })
            listReport(res);
        })
    }


    function listReport(data) {
        const $listItemChoosed = ViewTable({
            keyField: "ID",
            selector: $("#item-list"),
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: false
            },
            visibleFields: [
                "Invoice", "Requester", "UserName", "Balance", "Status", "Cancele", "VatType"
            ],
            columns: [
                {
                    name: "Cancele",
                    //template: `<input class='form-control font-size disvalue' type='text' />`,
                    on: {
                        "click": function (e) {
                            if (e.data.Status == "open") {
                                cancel(e.data.ID);
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
                            console.log(e.data.VatType)
                            let purchaseId = e.data.ID;
                            let vatnumber = parseInt(e.data.TypeVatNumber)
                            console.log("vatnumber", vatnumber)
                            if (vatnumber >= 1) {
                                window.open(_PrintUrl + "Home/PrintPurchaseRequest/?purchaseId=" + purchaseId + "", "_blank");
                            }
                            else {
                                window.open("/Print/PrintPurchasRequest?PurchaseID=" + purchaseId + "", "_blank");
                            }

                        }
                    }
                }
            ]
        });
        $listItemChoosed.bindRows(data);
    }
    //Cancel PO
    function cancel(id) {
        let msg = new DialogBox({
            type: "ok-cancel",
            content: "Do you want to cancel?"
        });
        msg.confirm(function (e) {

            $.ajax({
                url: '/PurchaseRequest/PRCancel',
                type: 'POST',
                dataType: 'JSON',
                data: { PurchaseID: id },
                success: function () {

                }
            });
            location.reload();
        });

    }
