$(document).ready(function () {
    const ___CM = JSON.parse($("#data-invoice").text());
    let data = {
        ID: 0,
        CusID: 0,
        CardMemberID: 0,
        UserID: 0,
        SeriesID: 0,
        SeriesDID: 0,
        DocTypeID: 0,
        Number: "",
        PostingDate: "",
        TotalDeposit: 0,
    };
    const date = new Date();
    $("#postingdate").prop("disabled", true);
    $("#amount").asNumber();
    document.getElementById("postingdate").valueAsDate = date;
    //invioce
    let selected = $("#no");
    selectSeries(selected);

    $('#no').change(function () {
        var id = ($(this).val());
        var seriesCM = findArray("ID", id, ___CM);
        data.SeriesID = seriesCM.ID;
        data.Number = seriesCM.NextNo;
        data.DocTypeID = seriesCM.DocumentTypeID;
        $("#number").val(seriesCM.NextNo);
    });
    if (___CM.length == 0) {
        $('#no').append(`
        <option selected> No Invoice Numbers Created!!</option>
        `).prop("disabled", true);
        $("#save").prop("disabled", true);
    }

    $(".choose-customer").click(function () {
        const cusDialog = new DialogBox({
            content: {
                selector: ".customer-dlg"
            },
            caption: "Customer",
            type: "ok",
        });
        cusDialog.invoke(function () {
            const _customer = ViewTable({
                keyField: "ID",
                selector: "#customer",
                indexed: true,
                paging: {
                    pageSize: 20,
                    enabled: false
                },
                visibleFields: ["Code", "Name", "Phone", "Address"],
                actions: [
                    {
                        template: `<i class="fas fa-arrow-alt-circle-down hover"></i>`,
                        on: {
                            "click": function (e) {
                                data.CusID = e.key;
                                data.CardMemberID = e.data.CardMemberID;
                                $("#customer-input").val(e.data.Name)
                                $("#pricelist").val(e.data.PriceListName)
                                $("#card").val(e.data.CardName)
                                cusDialog.shutdown()
                            }
                        },
                    }
                ]
            });
            getCustomer(function (res) {
                _customer.bindRows(res);
                $("#search-customer").on("keyup", function (e) {
                    let __value = this.value.toLowerCase().replace(/\s+/, "");
                    let rex = new RegExp(__value, "gi");
                    let items = $.grep(res, function (item) {
                        return item.Code.toLowerCase().replace(/\s+/, "").match(rex) || item.Name.toLowerCase().replace(/\s+/, "").match(rex)
                            || item.Phone.toLowerCase().replace(/\s+/, "").match(rex) || item.Address.toLowerCase().replace(/\s+/, "").match(rex)
                    });
                    _customer.bindRows(items);
                });
            })
        })
        cusDialog.confirm(function () {
            cusDialog.shutdown()
        })
    })
    $("#save").click(function () {
        data.PostingDate = $("#postingdate").val()
        data.TotalDeposit = $("#amount").val()
        $.post("/CardMember/DepositCardMember", { data }, function (res) {
            //error
            const error = new ViewMessage({
                summary: {
                    selector: ".error"
                }
            }, res);
            if (res.IsApproved) {
                error.refresh();
            }
        })
    })
    function getCustomer(success) {
        $.get("/CardMember/GetCustomerDiposit", success);
    }
    function selectSeries(selected) {
        $.each(___CM, function (i, item) {
            if (item.Default == true) {
                $("<option selected value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
                $("#number").val(item.NextNo);
                data.SeriesID = item.ID;
                data.Number = item.NextNo;
                data.DocTypeID = item.DocumentTypeID;
            }
            else {
                $("<option value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
            }
        });
        return selected.on('change')
    }
    function findArray(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
})