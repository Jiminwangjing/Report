$(document).ready(function () {
    let data = new Array(),
        itemCheck = new Array(),
        tableView = ViewTable({});

    $.ajax({
        url: '/PriceList/GetPriceList',
        type: 'GET',
        dataType: 'JSON',
        data: {
            price_list_baseid: parseInt($('#price-list-baseid').val())
        },
        success: function (price_lists) {
            $.each(price_lists, function (i, price_list) {
                $("#price-list").append(" <option value=" + price_list.ID + ">" + price_list.Name + "</option>");
            })
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
        tableView.yield().forEach(i=>{
           if(i.Active==true)
           {
            itemCheck.push(i);
           }
        });
        let ItemCopyToPriceList = {
            ID: 0,
            ToPriceListID: parseInt($('#price-list-baseid').val()),
            FromPriceListID: parseInt($('#price-list').val()),
            ItemCopyToPriceListDetail: itemCheck
        };

        $.ajax({
            url: '/PriceList/ItemCopyToPriceList',
            type: 'POST',
            dataType: 'JSON',
            data: { ItemCopyToPriceList: JSON.stringify(ItemCopyToPriceList) },
            success: function () {
                window.location.href = "/PriceList/Index";
            }
        })
      
    })

    $("#select-all").change(function () {
        const isChecked = $(this).prop("checked") ? true : false
        if (isChecked) {
            tableView.yield().forEach(i=>{
                tableView.updateColumn(i.ItemID,"Active",isChecked);
            });
            // itemCheck = data
            // $(".check-one").prop("checked", true)
        } else {
            tableView.yield().forEach(i=>{
                tableView.updateColumn(i.ItemID,"Active",isChecked);
            });
            // itemCheck = new Array()
            // $(".check-one").prop("checked", false)
        }
    })
    $("#filter-item").click(function () {
        let pricelist_id = parseInt($('#price-list').val());
        let group1 = parseInt($('#item-group1').val());
        let group2 = parseInt($('#item-group2').val());
        let group3 = parseInt($('#item-group3').val());
        $.ajax({
            url: '/PriceList/GetItemMasterToCopy',
            type: 'GET',
            dataType: 'JSON',
            data: {
                pricelistbase: $('#price-list-baseid').val(),
                pricelistid: pricelist_id,
                group1: group1,
                group2: group2,
                group3: group3
            },
            success: function (items)
             {
                tableView.clearRows();
                if(items.length>0)
                {
                    $("#txtseaerch").keyup(function () {
                        let input = this.value.replace(/\s+/g, '');
                        let rex = new RegExp(input, "gi");
                        var filtereds = $.grep(items, function (item, i) {
                            return item.Code.toLowerCase().replace(/\s+/, "").match(rex)
                                    ||item.KhmerName.toLowerCase().replace(/\s+/, "").match(rex)
                                    ||item.EnglishName.toLowerCase().replace(/\s+/, "").match(rex)
                                    ||item.Barcode.toLowerCase().replace(/\s+/, "").match(rex);
                                    
                        });
                        tableView.bindRows(filtereds);
                    });
                    tableView.bindRows(items);
                }   
                              
            }

        });
    })
    
        tableView = ViewTable({
            keyField: "ItemID",
            selector: "#item-list",
            indexed: true,
            paging: {
                pageSize: 20,
                enabled: true
            },
            visibleFields: ["Process", "Barcode", "Uom", "EnglishName", "KhmerName", "Code","Active"],
            columns:[
                {
                    name:"Active",
                    template:"<input type='checkbox'>",
                    on:{
                        click:function(e)
                        {
                            const isChecked = $(this).prop("checked") ? true : false
                            tableView.updateColumn(e.key,"Active",isChecked);
                        }
                    }
                }
            ],
           
        })
        if (isValidArray(data)) {
            tableView.bindRows(data)
        } else {
            tableView.clearRows()
        }
    
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
})