$(document).ready(function () {
    let data = new Array(),
        tableView = ViewTable({});
    $.ajax({
        url: '/PriceList/GetPriceList',
        type: 'GET',
        dataType: 'JSON',
        success: function (price_lists) {
            $.each(price_lists, function (i, price_list) {
                $("#price-list").append(" <option value=" + price_list.ID + ">" + price_list.Name + "</option>");
            })
            let pricelist_id = $('#price-list-baseid').val();
            $("#price-list").val(pricelist_id);
            filterItems();
        }
    });
    $.ajax({
        url: '/PriceList/GetGroup1',
        type: 'GET',
        dataType: 'JSON',
        success: function (group1s) {
            $.each(group1s, function (i, group1) {
                $("#item-group1").append(" <option value=" + group1.ItemG1ID + ">" + group1.Name + "</option>");
            })
        }

    });
    $('#item-group1').change(function (e) {
        let group1 = $(this).val();
        $.ajax({
            url: '/PriceList/GetGroup2',
            type: 'GET',
            dataType: 'JSON',
            data: {
                group1: parseInt(group1)
            },
            success: function (group2s) {
                $('#item-group2 option:not(:first)').remove();
                $('#item-group3 option:not(:first)').remove();
                $.each(group2s, function (i, group2) {
                    $("#item-group2").append(" <option value=" + group2.ItemG2ID + ">" + group2.Name + "</option>");
                })
            }

        });
    });
    $('#item-group2').change(function (e) {
        let group2 = $(this).val();
        let group1 = $('#item-group1').val();
        $.ajax({
            url: '/PriceList/GetGroup3',
            type: 'GET',
            dataType: 'JSON',
            data: {
                group1: parseInt(group1),
                group2: parseInt(group2)
            },
            success: function (group3s) {
                $('#item-group3 option:not(:first)').remove();
                $.each(group3s, function (i, group3) {
                    $("#item-group3").append(" <option value=" + group3.ID + ">" + group3.Name + "</option>");
                })
            }

        });
    });
    $('#add-item').click(function () {
        $(this).text('Saving...');
        let PricelistSetUpdatePrice = {
            PricelistSetPrice: data
        };
        $.ajax({
            url: '/PriceList/SetAndUpdateSalePrice',
            type: "POST",
            data: { data: JSON.stringify(PricelistSetUpdatePrice) },
            success: function () {
                window.location.href = "/PriceList/Index"
            }
        });
    })

    $("#filter-item").click(function (e) {
        filterItems();
    })
    function filterItems() {
        let pricelist_id = parseInt($('#price-list').val());
        let group1 = parseInt($('#item-group1').val());
        let group2 = parseInt($('#item-group2').val());
        let group3 = parseInt($('#item-group3').val());
        $.ajax({
            url: '/PriceList/GetItemsUpdatePrice',
            type: 'GET',
            dataType: 'JSON',
            data: {
                PriceListID: pricelist_id,
                Group1: group1,
                Group2: group2,
                Group3: group3
            },
            success: function (items) {
                if (items.length > 0) {
                    items.checked = false;
                    data = items
                    displayData(data)
                }
                else {
                    displayData([])
                }
            }
        });
    }

    $('#txtsearch').keyup(function () {
        const items = searchItem(data, this.value)
        displayData(items)
    });
    function searchItem(items, keyword) {
        let input = noSpace(keyword);
        let regex = new RegExp(input, "i");
        return $.grep(items, function (item) {
            return regex.test(noSpace(item.Currency))
                || regex.test(noSpace(item.Process))
                || regex.test(noSpace(item.Barcode))
                || regex.test(noSpace(item.Cost))
                || regex.test(noSpace(item.Uom))
                || regex.test(noSpace(item.EnglishName))
                || regex.test(noSpace(item.KhmerName))
                || regex.test(noSpace(item.Code));
        });
    }

    function noSpace(phrase) {
        if (typeof phrase === "string") {
            return phrase.replace(/\s/g, "");
        }
    }
    function isEmpty(value) {
        return value == undefined || value == null || value == "";
    }


    function displayData(data) {
        tableView = ViewTable({
            keyField: "ID",
            selector: "#item-list",
            indexed: false,
            paging: {
                pageSize: 10,
                enabled: true
            },
            visibleFields: ["Currency", "Process", "Barcode", "Cost", "Uom", "EnglishName", "KhmerName", "Code", "Price"],
            columns: [
                {
                    name: "Cost",
                    template: "<input />",
                    on: {
                        "keyup": function (e) {
                            $(this).asNumber();
                            updateData(data, "ID", e.key, "Cost", this.value);
                        }
                    }
                },
                {
                    name: "Price",
                    template: "<input />",
                    on: {
                        "keyup": function (e) {
                            $(this).asNumber();
                            updateData(data, "ID", e.key, "Price", this.value);
                        }
                    }
                },
                {
                    name: "Barcode",
                    template: "<input />",
                    on: {
                        "keyup": function (e) {
                            updateData(data, "ID", e.key, "Barcode", this.value);
                        }
                    }
                }
            ],
            actions: [
                {
                    template: `<i class="fas fa-save fa-lg csr-pointer text-center"></i>`,
                    on: {
                        "click": function (e) {
                            $.post("/pricelist/updatepricelistdetial", { data: e.data }, function (res) {
                                new ViewMessage({
                                    summary: {
                                        selector: ".error-message"
                                    }
                                }, res)
                            })
                        }
                    }
                }
            ]
        })
        if (isValidArray(data)) {
            tableView.bindRows(data)
            if (data.length > 0) {
                data.forEach(i => {
                    if (i.Process !== "Standard") {
                        tableView.disableColumns(i.ID, ["Cost"])
                    }
                })
            }
        } else {
            tableView.clearRows()
        }

    }
    function updateData(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] == keyValue) {
                    i[prop] = propValue;
                }
            });
        }
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
})