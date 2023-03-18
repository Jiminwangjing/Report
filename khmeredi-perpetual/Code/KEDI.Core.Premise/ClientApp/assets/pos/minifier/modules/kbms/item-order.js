
$("#add-new-order").click(function (e) {
    //clearNewBarcode();
    clearOrder();
    newdefaultOrder();
    $(".Table").text(table_info.name);
    $("#send").text("Send");
});

function gridClicked(e) {
    
    let group_id = $(this).children().data("id");
    let level = parseInt($(this).children().data("group"));
    let group1_id = parseInt($(this).children().data("group1"));
    let group2_id = parseInt($(this).children().data("group2"));
    let group3_id = parseInt($(this).children().data("group3"));
    let step_name = $(this).children().data("step");
    //Show step of group
    $(".group-steps").append($('<div class="wrap-step step" data-id="' + group_id + '" data-group1="' + group1_id + '" data-group2="' + group2_id + '" data-group3="' + group3_id + '" data-group="' + level + '" >' + step_name + '</div>').on('click', clickOnstep));
    //Find group from controler
    let group_items;
    if (level <= 3) {
        group_items = $.ajax({
            url: "/POS/GetGroupItem",
            async: false,
            type: "GET",
            data: {
                group1_id: group1_id,
                group2_id: group2_id,
                level: level

            },
            dataType: "JSON"

        }).responseJSON;
    }
    level = level + 1;
    if (level == 5) {
        if (db.table("tb_item_master").size > 0) {
            let item_filtered = db.from("tb_item_master").where(w => {
                return w.ItemID === group_id;
            });
            singleItem($("#group-item-gridview .wrap-grid"), item_filtered);
        }
      
    }
    //Check group or item
    else if (Array.isArray(group_items) && group_items.length >= 0) {

        groupItemFiltered($(this).parent(), group_items, level, group1_id, group2_id, group3_id);
    }
    else {
        $(this).parent().children().remove();
    }
};

let $grid = $("#group-item-gridview .wrap-grid .grid");
//Add event listener to grid on first DOM loading.
$grid.click(gridClicked);

function groupItemFiltered($wrap_grid, group_items, level, group1_id, group2_id, group3_id) {
   
    let group_filter = level - 1;
    $wrap_grid.children().remove();
    if (group_items.length ===0) {

        switch (group_filter) {
            case 1:
                item_filtered = db.from("tb_item_master").where(w => {
                    return w.Group1 === group1_id;
                });
                break;

            case 2:
                item_filtered = db.from("tb_item_master").where(w => {
                    return w.Group1 === group1_id && w.Group2 === group2_id;
                });
                break;
            case 3:
                item_filtered = db.from("tb_item_master").where(w => {
                    return w.Group1 === group1_id && w.Group2 === group2_id && w.Group3 === group3_id;
                });
                break;

        }
        apply_pagination_order($wrap_grid,item_filtered);
    }
    else {
        $.each(group_items, function (i, item) {
            var data = "";
            if (level === 1) {
                group1_id = item.ItemG1ID;
                data = '<div data-group="' + level + '" data-group1="' + group1_id + '" data-group2="' + group2_id + '" data-group3="' + group3_id + '" data-step="' + item.Name + '" data-id="' + item.ItemG1ID + '" hidden></div>'
            }
            else if (level === 2) {
                group2_id = item.ItemG2ID;
                data = '<div data-group="' + level + '" data-group1="' + group1_id + '" data-group2="' + group2_id + '" data-group3="' + group3_id + '" data-step="' + item.Name + '" data-id="' + item.ItemG2ID + '" hidden></div>'
               
            }
            else {
                group3_id = item.ID;
                data = '<div data-group="' + level + '" data-group1="' + group1_id + '" data-group2="' + group2_id + '" data-group3="' + group3_id + '" data-step="' + item.Name + '" data-id="' + item.ID + '" hidden></div>'

            }

            let $grid = $("<div class='grid'></div>")
                .append(data)
                .append('<div class="grid-caption">' + item.Name + '</div>');
            let $grid_image = $("<div class='grid-image'></div>")
                .append('<img src="/Images/itemgroup/' + item.Images + '"/>');
            $grid.append($grid_image);
            $grid.on("click", gridClicked);
            $wrap_grid.append($grid);
        });
    }

};

function MultiItemFiltered($wrap_grid) {
    if (db.from("tb_multi_item") === 0) { return; }
    $.each(db.from("tb_multi_item"), function (i, item) {
        if (item.Image === null || item.Image === '') {
            item.Image = 'no-image.jpg';
        }
        var data = "";
        data = '<div data-group="' + 4 + '" data-group1="' + 0 + '" data-group2="' + 0 + '" data-group3="' + 0 + '" data-step="' + item.KhmerName + '" data-id="' + item.ItemID + '" hidden></div>'
        let $grid = $("<div class='grid'></div>")
            .append(data)
            .append('<div class="grid-caption">' + item.KhmerName + '</div>');
        let $grid_image = $("<div class='grid-image'></div>")
            .append('<img src="/Images/' + item.Image + '"/>');
        $grid.append($grid_image);
        $grid.on("click", gridClicked);
        $wrap_grid.append($grid);
    });

};

function singleItem($wrap_grid, item_filtered) {
    $pagination.twbsPagination('destroy');
    $wrap_grid.children().remove();
    $.each(item_filtered, function (i, item) {
        if (item.Image === null || item.Image === '') {
            item.Image = 'no-image.jpg';
        }
        let dis = "%";
        if (item.TypeDis != "Percent")
            dis = "";
        let KhmerName = item.KhmerName + ' (' + item.UoM + ')';
            

        let $grid = $("<div class='grid'></div>");
        $grid.append('<div data-bonus="' + item.DiscountRate + '" data-id="' + item.ID + '" hidden></div>')
        $grid.append('<div class="grid-caption" title=' + KhmerName + ' >' + KhmerName + ' </div > ');
        let $grid_image = $("<div class='grid-image'></div>");
        $grid_image.append($("<i class='fas fa-calculator fa-lg fn-sky order-calc'></i>").on("click", openCalculator));

        let $wrap_scale = $('<div class="wrap-scale">'
            + '<i class="scale-down">-</i>'
            + '<label data-scale="' + item.PrintQty + '" class="scale">' + item.PrintQty + '</label>'
            + '<i class="scale-up">+</i>'
            + '</div>');
        if (item.PrintQty !== 0) {
            $wrap_scale.addClass("show");
        } else {
            $wrap_scale.removeClass("show");        
        }
        $wrap_scale.appendTo($grid_image);
        if (item.DiscountRate > 0) {
            $('<div class="discount">' + item.DiscountRate + ' ' + dis + ' </div>').appendTo($grid_image);
        }
   
        $grid_image.append('<img src="/Images/' + item.Image + '"/>',
            '<div class="price">' + item.UnitPrice.toFixed(3) + '</div>'
        ).appendTo($grid);
        if (item.KhmerName !== item.EnglishName) {
            $('<div class="grid-caption" title=' + item.EnglishName + ' >' + item.Code + ' ' +item.EnglishName + '</div>').appendTo($grid);
        }
        else {
            $('<div class="grid-caption" title=' + item.EnglishName + ' >' + item.Code+ '</div>').appendTo($grid);
        }
        $wrap_grid.append($grid.on("click", bindToTable));
    });
   
};

function singleItemFiltered($wrap_grid, item_filtered) {
    db.table("tb_multi_item").clear();
    $wrap_grid.children().remove();
    $.each(item_filtered, function (i, item) {
     
        let multiy_item = db.from("tb_item_master").where(w => {
            return w.ItemID == item.ItemID;
        });
        if (multiy_item.length > 1) {
            db.insert("tb_multi_item", item, "ItemID");
        }
        else {
            if (item.Image === null || item.Image=== '')
            {
                item.Image = 'no-image.jpg';
            }

            let KhmerName = item.KhmerName + ' (' + item.UoM + ')';
           
            let dis = "%";
            if (item.TypeDis !== "Percent")
                dis = "";

            let $grid = $("<div class='grid' " + displayRecords[i]+"></div>");
            $grid.append('<div data-bonus="' + item.DiscountRate + '" data-id="' + item.ID + '" hidden></div>');
            $grid.append('<div class="grid-caption" title=' + KhmerName + ' >' + KhmerName +' </div > ');
            
            let $grid_image = $("<div class='grid-image'></div>");
            $grid_image.append($("<i class='fas fa-calculator fa-lg fn-sky order-calc'></i>").on("click", openCalculator));

            let $wrap_scale = $('<div class="wrap-scale">'
                + '<i class="scale-down">-</i>'
                + '<label data-scale="' + item.PrintQty + '" class="scale">' + item.PrintQty + '</label>'
                + '<i class="scale-up">+</i>'
                + '</div>');
            
            $wrap_scale.appendTo($grid_image);
            if (item.PrintQty != 0) {
                $wrap_scale.addClass("show");
            } else {
                $wrap_scale.removeClass("show");
            }
            $wrap_scale.appendTo($grid_image);
            if (item.DiscountRate > 0)
                $('<div class="discount">' + item.DiscountRate + ' ' + dis + ' </div>').appendTo($grid_image);
            $grid_image.append('<img src="/Images/' + item.Image + '"/>',
                '<div class="price">' + item.UnitPrice + '</div>'
            ).appendTo($grid);
            if (item.KhmerName !== item.EnglishName) {
                $('<div class="grid-caption" title=' + item.EnglishName + ' >' + item.Code + ' ' + item.EnglishName + '</div>').appendTo($grid);
            }
            else {
                $('<div class="grid-caption" title=' + item.EnglishName + ' >' + item.Code  + '</div>').appendTo($grid);
            }
            
            $wrap_grid.append($grid.on("click", bindToTable));
        }

    });
    MultiItemFiltered($wrap_grid);
};

function summaryTotal(selected_id, option) {
    //Formate data
    if (db.from('tb_order_detail') != 0) {
        $('#count-item').text('(' + db.from('tb_order_detail').where(w => { return w.Qty>0 }).length + ')');
        db.from("tb_order_detail").where(function (item) {
            if (item.Currency != 'KHR' || item.Currency != '៛') {
                item.UnitPrice = parseFloat(item.UnitPrice).toFixed(3);
                item.Total = parseFloat(item.Total).toFixed(3);
                item.Total_Sys = parseFloat(item.Total_Sys).toFixed(3);
            }
            if (parseFloat(item.Qty).toString().includes('.') || parseFloat(item.PrintQty).toString().includes('.')) {
                if (parseFloat(item.Qty) < 1 || parseFloat(item.PrintQty) < 1) {

                    item.Qty = parseFloat(item.Qty.toFixed(2));
                    item.PrintQty = parseFloat(item.PrintQty.toFixed(3));

                }

            }
        });
    }
    else {
        $('#count-item').text('')
    }
    //Bind data to list table
    if (option === 'Y') {
        if (db.from("tb_order_detail") != 0) {
            $.bindRows("#item-listview", db.from("tb_order_detail").where(w => { return w.Qty > 0}), 'Line_ID', {
                highlight_row: parseFloat(selected_id),
                scalable: {
                    column: "Qty",
                    event: "click",
                    callback: rowScaleClicked
                },
                text_align: [{ "KhmerName": "left" }, { "Code": "left" }],
                scrollview: {
                    block: "center"
                },
                html: [
                    {
                        column: -1,
                        insertion: "replace",
                        element: '<i class="fa fa-trash trash csr-pointer" title="Delete"></i>',
                        listener: ["click", function (e) {
                            rowDelete(this);
                        }]
                    }
                ],
                prebuild: function (data) {

                   //do something               
                },
                postbuild: function (data) {
                    if (data.ItemType == 'Item') {
                        $("td:nth-child(3)", this).prop("title", "Click to add comment").on("click", rowCommentClicked).addClass('csr-pointer');
                        $("td:nth-child(7)", this).prop("title", "Double click to copy item").on("dblclick", rowCopyItem).addClass('csr-pointer');
                        //$("td:nth-child(5)", this).prop("title", "Double click edit item name").on("dblclick", rowEditName).addClass('csr-pointer');
                    }
                    if (data.ItemType == 'Item' && data.ItemStatus == 'new') {
                      
                        $("td:nth-child(5)", this).prop("title", "Double click add on item").on("dblclick", function () {
                            parent_row = $(this).parent();
                            addOnItem(parent_row);
                        }).addClass('csr-pointer');
                    }
                    else if (data.ItemType == 'Addon') {
                        $(this).find("td:nth-child(3)").html("<i style='left:50%; position:absolute; bottom:10%' class='fas fa-plus-circle fn-green fa-xs'></i>");
                    }
                   
                    //option-addon
                    //$(this).find("td:nth-child(3)").prepend($("<i class='icon-option fas fa-ellipsis-v'></i>")
                    //.on("click", function () {
                    //    $(".option-addon").prop("hidden", false).find(".wrap-dropbox-content").addClass("show")
                    //        .css("top", "2.5px")
                    //        .css("right", "35%");
                    //}));
                },
                show_key: false,
                hidden_columns: ["Line_ID","Description","ParentLevel", "Comment", "PrintQty", "ItemStatus", "TypeDis", "DiscountValue", "UomID", "ItemID", "OrderDetailID", "Total_Sys", "EnglishName", "Cost", "Currency", "ItemPrintTo","ItemType"]
            });
        }
    }

    function rowCopyItem(e) {
        let item = {};
        let id = parseInt($(this).parent().data('line_id'));
        let item_master = db.table('tb_order_detail').get(id);
        let new_line_id = parseInt(db.from('tb_order_detail').length) + 1;
        item.Line_ID = parseInt(id +'3'+new_line_id);
        item.OrderDetailID = 0;
        item.Code = item_master.Code;
        item.ItemID = item_master.ItemID;
        item.KhmerName = item_master.KhmerName;
        item.EnglishName = item_master.EnglishName;
        item.UnitPrice = parseFloat(item_master.UnitPrice);
        item.Cost = item_master.Cost;
        item.Qty = 1;
        item.UoM = item_master.UoM;
        item.PrintQty = 1;
        item.DiscountRate = parseFloat(item_master.DiscountRate);
        item.TypeDis = item_master.TypeDis;
        item.ItemStatus = 'new';
        item.Currency = item_master.Currency;

        if (item_master.ItemType != 'Service') {
            if (item.TypeDis === 'Percent') {
                item.Total = ((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)));
                item.DiscountValue = ((item.Qty * item.UnitPrice) * item.DiscountRate) / 100;

            }
            else {
                item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate);
                item.DiscountValue = item.DiscountRate;
            }
        }
        else {
            let time = getTimeOnTable();
            var arr_time = time.split(':');
            let h = parseInt(arr_time[0]);
            let m = parseInt(arr_time[1]);
            item.Qty = h + (m * 0.01);

            let price = (item.UnitPrice * h) + (item.UnitPrice / 60) * m;
            if (item.TypeDis === 'Percent') {
                item.Total = (price * (1 - (item.DiscountRate / 100)));
                item.DiscountValue = price * item.DiscountRate / 100;

            }
            else {

                item.Total = (price - item.DiscountRate);
                item.DiscountValue = item.DiscountRate;
            }
        }

        item.Total_Sys = item.Total * local_currency.ratein;
        item.UomID = item_master.UomID;
        item_master.PrintQty = item.PrintQty;
        item.ItemPrintTo = item_master.ItemPrintTo;
        item.Comment = '';
        item.ItemType = item_master.ItemType;
        item.Description = item_master.Description;
        item.ParentLevel = item.Line_ID.toString();
        db.insert('tb_order_detail', item, 'Line_ID');
        summaryTotal(item.Line_ID, 'Y');
       
    };

    function rowCommentClicked(e){
        let id = parseInt($(this).parent().data('line_id'));
        rowInitFromComment(id);
    };

    function rowEditName(e) {
        let line_id = parseInt($(this).parent().data('line_id'));
        let item = db.get('tb_order_detail').get(line_id);
        let dlg = new DialogBox({
            position: "center-left",
            content: {
                selector: "#order-editname",
                class: 'login'
            },
            icon: "fas fa-edit",
            button: {
                ok: {
                    text: "OK",
                    callback: function (e) {
                        item.KhmerName = this.meta.content.find('.order-khname').val();
                        summaryTotal(line_id, 'Y');
                        this.meta.shutdown();
                    }
                }
            }
        });
        dlg.startup("after", function (dialog) {
            dialog.content.find('.order-khname').val(item.KhmerName);
            dialog.content.find('.order-khname').focus();

        });
    };

    //Summary
    let sub_total = 0;
    let discount_symbol = '%';
   
    let item_ordered = db.from("tb_order_detail");
    $.each(item_ordered, function (i, item) {
        sub_total += parseFloat(item.Total);
    });
    if (isNaN(sub_total)) {
        sub_total = 0;
    }
    order_master.Sub_Total = sub_total;
    if (order_master.TypeDis === 'Percent') {
        order_master.DiscountValue = order_master.Sub_Total * order_master.DiscountRate / 100;
        discount_symbol = '%';

    }
    else {

        order_master.DiscountValue = order_master.DiscountRate;
        discount_symbol = local_currency.symbol;

    }
    let vat = (order_master.TaxRate + 100) / 100;
    let rate = order_master.TaxRate / 100;

    order_master.TaxValue = (order_master.Sub_Total / vat) * rate;
    order_master.GrandTotal = order_master.Sub_Total - order_master.DiscountValue;
    order_master.GrandTotal_Sys = order_master.GrandTotal * local_currency.ratein;
    if (local_currency.symbol === 'KHR' || local_currency.symbol === '៛') {
        $("#summary-sub-total").text(local_currency.symbol + " " + numeral(order_master.Sub_Total).format('0.000'));
        $("#summary-bonus").text(order_master.DiscountRate + " " + discount_symbol);
        $("#summary-vat").text(local_currency.symbol + " " + numeral(order_master.TaxValue).format('0.000'));
        $("#summary-total").text(local_currency.symbol + " " + numeral(order_master.GrandTotal).format('0.000'));
    }
    else {
        $("#summary-sub-total").text(local_currency.symbol + " " + order_master.Sub_Total.toFixed(3));
        $("#summary-bonus").text(order_master.DiscountRate + " " + discount_symbol);
        $("#summary-vat").text(local_currency.symbol + " " + order_master.TaxValue.toFixed(3));
        $("#summary-total").text(local_currency.symbol + " " + order_master.GrandTotal.toFixed(3));
    }
   
    //Send data to second screen
    //let data = {
    //    OrderNo: table_info.name + " #" + order_master.OrderNo,
    //    DiscountRate: order_master.DiscountRate,
    //    TypeDis: order_master.TypeDis,
    //    TaxRate: order_master.TaxRate,
    //    TableID: selected_id,
    //    OrderDetail: db.from("tb_order_detail")
    //};

    //$.ajax({
    //    url: "/POS/SendDataToSecondScreen",
    //    type: "POST",
    //    data: { data: data },
    //    success: function () {

    //    }
    //});
   
}

let parentLevel = "";
function bindToTable(e) {
    let scale = $(this).find(".grid-image .wrap-scale .scale");
    let scale_down = $(this).find(".grid-image .wrap-scale .scale-down");
    let scale_up = $(this).find(".grid-image .wrap-scale .scale-up");

    let id = $(this).children().data("id");

    //Get item master data from tb_item_master in warehouse
    let item_master = db.from("tb_item_master").where(function (json) {
        return json.ID === id;
    })[0];
    if (item_master.ItemType == 'Addon') {
        addNewItemAddon(scale, item_master);
        return;
    }
    //Declare order item
    let item = {};
    let check_item = db.table("tb_order_detail").find(id);
    if (item_master.PrintQty == 0) {
        if (check_item === undefined) {
            item.Line_ID = item_master.ID;
            item.OrderDetailID = 0;
            item.Code = item_master.Code;
            item.ItemID = item_master.ItemID;
            item.KhmerName = item_master.KhmerName;
            item.EnglishName = item_master.EnglishName;
            item.UnitPrice = parseFloat(item_master.UnitPrice);
            item.Cost = item_master.Cost;
            item.Qty = 1;
            item.UoM = item_master.UoM;
            item.PrintQty = 1;
            item.DiscountRate = parseFloat(item_master.DiscountRate);
            item.TypeDis = item_master.TypeDis;
            item.ItemStatus = 'new';
            item.Currency = item_master.Currency;
            
            if (item_master.ItemType != 'Service') {
                if (item.TypeDis === 'Percent') {
                    item.Total = ((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)));
                    item.DiscountValue = ((item.Qty * item.UnitPrice) * item.DiscountRate) / 100;

                }
                else {
                    item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate);
                    item.DiscountValue = item.DiscountRate;
                }
            }
            else {
                let time = getTimeOnTable();
                var arr_time = time.split(':');
                let h = parseInt(arr_time[0]);
                let m = parseInt(arr_time[1]);
                item.Qty = h + (m * 0.01);
            
                let price = (item.UnitPrice * h) + (item.UnitPrice / 60) * m;
                if (item.TypeDis === 'Percent') {
                    item.Total = (price * (1 - (item.DiscountRate / 100)));
                    item.DiscountValue = price * item.DiscountRate / 100;

                }
                else {

                    item.Total = (price - item.DiscountRate);
                    item.DiscountValue = item.DiscountRate;
                }
            }
           
            item.Total_Sys = item.Total * local_currency.ratein;
            item.UomID = item_master.UomID;
            item_master.PrintQty = item.PrintQty;
            item.ItemPrintTo = item_master.PrintTo;
            item.Comment = '';
            item.ItemType = item_master.ItemType;
            item.Description = item_master.Description;
            item.ParentLevel = item_master.ID.toString();
        }
        else {

            item = db.get("tb_order_detail").get(id);
            if (item.ItemType != 'Service') {
                if (item.PrintQty == 0) {
                    item.Qty = ((item.Qty) + 1);
                    item.UoM = item_master.UoM;
                    item.PrintQty = (item.PrintQty + 1);
                    
                    if (item_master.ItemType !== 'Service') {
                        if (item.TypeDis === 'Percent') {
                            item.Total = ((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)));
                            item.DiscountValue = ((item.Qty * item.UnitPrice) * item.DiscountRate) / 100;

                        }
                        else {
                            item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate);
                            item.DiscountValue = item.DiscountRate;
                        }
                    }
                    else {
                        let time = getTimeOnTable();
                        var arr_time = time.split(':');
                        let h = parseInt(arr_time[0]);
                        let m = parseInt(arr_time[1]);
                        item.Qty = h + m * 0.01;
                        let price = (item.UnitPrice * h) + (item.UnitPrice / 60) * m;
                        if (item.TypeDis === 'Percent') {
                            item.Total = (price * (1 - (item.DiscountRate / 100)));
                            item.DiscountValue = price * item.DiscountRate / 100;

                        }
                        else {

                            item.Total = (price - item.DiscountRate);
                            item.DiscountValue = item.DiscountRate;
                        }
                    }
                    item.Total_Sys = item.Total * local_currency.ratein;
                }
            }
        }

    }

    db.insert("tb_order_detail", item, 'Line_ID');
    db.insert('tb_show_item', item_master, 'ID');
   
    //Show scale qty
    scale.data("scale", item.PrintQty);
    scale.text(item.PrintQty);
    $(this).find(".wrap-scale").addClass("show").siblings(".order-calc").addClass("show");
    //Scale up qty
    if ($(e.target).is(scale_up)) {
        item = db.get("tb_order_detail").get(id);
        if (item.ItemType !== 'Service') {
            item.Qty = (parseFloat(item.Qty) + 1);
            item.PrintQty = (parseFloat(item.PrintQty) + 1);
            item_master.PrintQty = parseFloat(item.PrintQty);
      
            if (item.TypeDis == "Percent") {
                item.Total = ((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)));
                item.DiscountValue = (item.Qty * item.UnitPrice) * item.DiscountRate / 100;

            }
            else {
                item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate);
                item.DiscountValue = item.DiscountRate;
            }
            item.Total_Sys = item.Total * local_currency.ratein;
            item.UomID = item_master.UomID;
            if (item.PrintQty === 0) {
                $(this).find(".wrap-scale").removeClass("show").siblings(".order-calc").removeClass("show");
            }
        }
    }
    //Scale down qty
    if ($(e.target).is(scale_down)) {
   
        item = db.get("tb_order_detail").get(id);
        if (item.ItemType != 'Service') {
            if (item.Qty > 0) {
                if (item.PrintQty > 0) {
                    item.Qty = (item.Qty - 1);
                    item.PrintQty = (item.PrintQty - 1);
                    item_master.PrintQty = item.PrintQty;
                }

                if (item.TypeDis == "Percent") {
                    item.Total = ((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)));
                    item.DiscountValue = (item.Qty * item.UnitPrice) * item.DiscountRate / 100;

                }
                else {
                    item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate);
                    item.DiscountValue = item.DiscountRate;
                }
                item.Total_Sys = item.Total * local_currency.ratein;
                item.UomID = item_master.UomID;

                if (item.ItemStatus == 'old') {
                    if (item.PrintQty == 0) {
                        $(this).find(".wrap-scale").removeClass("show").siblings(".order-calc").removeClass("show");
                    }
                    if (item.PrintQty < 0) {
                        let minus = 1;
                        if (item.PrintQty<1) {
                            minus = parseFloat(item.PrintQty).toPrecision(3);
                            $(this).find(".wrap-scale").removeClass("show").siblings(".order-calc").removeClass("show");
                        }
                        item.Qty = (item.Qty - minus);
                        item.PrintQty = (item.PrintQty - minus);
                        item_master.PrintQty = item.PrintQty;
                        if (item.TypeDis == "Percent") {
                            item.Total = ((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)));
                            item.DiscountValue = (item.Qty * item.UnitPrice) * item.DiscountRate / 100;
                        }
                        else {
                            item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate);
                            item.DiscountValue = item.DiscountRate;
                        }
                        item.Total_Sys = item.Total * local_currency.ratein;
                        if (item.Qty <= 0) {
                            $(this).find(".wrap-scale").removeClass("show").siblings(".order-calc").removeClass("show");
                        }
                    }
                }
                else {
                    if (item.PrintQty <= 0) {
                        let item_master = db.from("tb_item_master").where(function (json) {
                            return json.ID === id;
                        })[0];
                        item_master.Qty = 0;
                        item_master.PrintQty = 0;
                        db.insert('tb_show_item', item_master, 'ID');
                        
                        db.table("tb_order_detail").delete(id);
                        $(this).find(".wrap-scale").removeClass("show").siblings(".order-calc").removeClass("show");
                        let rows = $("#item-listview").children().children();
                        $.each(rows, function (i, row) {
                            let row_id = parseInt($(row).data("line_id"));
                            if (row_id === id) {
                                $(row).remove();
                            }
                        });
                        summaryTotal(id, 'Y');
                        return;
                    }
                }
            }
            
        }
    }
    db.update("tb_order_detail", item, 'Line_ID');
    db.update("tb_show_item", item_master, 'ID');
    scale.data("scale", item.PrintQty);
    scale.text(item.PrintQty);
    summaryTotal(item.Line_ID, 'Y');
};

function rowDelete(row) {
    let item_ordered = db.table("tb_order_detail");
    let $row = $(row).parent().parent();
    let line_id = $row.data("line_id");
    let check_item = db.table("tb_order_detail").get(line_id);
    let item_master = db.table("tb_show_item").get(line_id);
    //Find in db tb_show_item_barcode
    if (item_master == undefined) {
        item_master = db.table("tb_show_item_barcode").get(line_id);
    }
    let grids = $("#group-item-gridview .wrap-grid .grid");
    if (check_item.ItemStatus == 'new') {
        if (item_master != undefined) {
            item_master.PrintQty = 0;
            db.table("tb_order_detail").delete(line_id);
            db.update("tb_order_detail", item_ordered, "Line_ID");
            db.update("tb_show_item", item_master, "ID");
            $row.remove();
            $.each(grids, function (i, grid) {
                let grid_id = $(grid).children().data("id");

                if (grid_id === line_id) {
                    if (db.from('tb_show_item_barcode') != 0) {
                        db.table('tb_show_item_barcode').delete(line_id);
                        $(grid).remove();
                    }
                    else {
                        $(grid).find(".wrap-scale").removeClass("show").siblings(".order-calc").removeClass("show");
                    }
                }
            });
        }
        else {
            db.table("tb_order_detail").delete(line_id);
            db.update("tb_order_detail", item_ordered, "Line_ID");
            //db.update("tb_show_item", item_master, "ID");
            $row.remove();
        }
        summaryTotal(line_id, 'Y');
    }
    else {
        confirmDeleteItem(row);
    }
};

function getTimeOnTable() {
    let time = $.ajax({
        url: '/POS/GetTimeByTable',
        async: false,
        type: 'GET',
        data: {
            TableID: table_info.id
        }
    }).responseText;
    return time;
};

function confirmDeleteItem(row) {
    let $row = $(row).parent().parent();
    let line_id = $row.data("line_id");
    let item_master = db.table("tb_item_master").get(line_id);
    let grids = $("#group-item-gridview .wrap-grid .grid");
    let user_privillege = db.table("tb_user_privillege").get('P011');
    if (user_privillege.Used === false) {
        let dlg = new DialogBox({
            // close_button: false,
            position: "top-center",
            content: {
                selector: "#admin-authorization",
                class: "login"
            },
            icon: "fas fa-lock",
            button: {
                ok: {
                    text: "Login",
                    callback: function (e) {
                        let access = accessSecurity(this.meta.content, 'P011');
                        if (access === false) {
                            this.meta.content.find('.error-security-login').text('You can not access ...!');
                            return;
                        }
                        else {
                            this.meta.content.find('.security-username').focus();
                            this.meta.setting.icon = "fas fa-lock fa-spin";
                            this.text = "Logging...";
                            this.meta.content.find('.error-security-login').text('');
                            this.meta.build(this.setting);
                            setTimeout(() => {
                                this.meta.build(this.setting);
                                this.meta.setting.icon = "fas fa-unlock-alt";
                                setTimeout(() => {
                                    this.meta.shutdown();
                                    //cofirm delete
                                    if (item_master != undefined) {
                                        item = db.get("tb_order_detail").get(line_id);
                                        item_master.PrintQty = 0;
                                        item.PrintQty = parseFloat(item.Qty) * (-1);
                                        item.Qty = 0;
                                        db.update("tb_order_detail", item, "Line_ID");
                                        db.update("tb_item_master", item_master, "ID");

                                        $row.remove();
                                        $.each(grids, function (i, grid) {
                                            let grid_id = $(grid).children().data("id");

                                            if (grid_id === line_id) {
                                                $(grid).find(".wrap-scale").removeClass("show");
                                            }
                                        });
                                    }
                                    else {
                                        item = db.get("tb_order_detail").get(line_id);
                                        item.PrintQty = parseFloat(item.Qty) * (-1);
                                        item.Qty = 0;
                                        db.update("tb_order_detail", item, "Line_ID");
                                        $row.remove();
                                    }
                                    summaryTotal(line_id, 'Y');
                                    //end
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {
        if (item_master != undefined) {
            item = db.get("tb_order_detail").get(line_id);
            item_master.PrintQty = 0;
            item.PrintQty = parseFloat(item.Qty) * (-1);
            item.Qty = 0;
            db.update("tb_order_detail", item, "Line_ID");
            db.update("tb_item_master", item_master, "ID");
        }
        else {
            item = db.get("tb_order_detail").get(line_id);
            item.PrintQty = parseFloat(item.Qty) * (-1);
            item.Qty = 0;
            db.update("tb_order_detail", item, "Line_ID");
            db.update("tb_item_master", item_master, "ID");
        }
        $row.remove();
        $.each(grids, function (i, grid) {
            let grid_id = $(grid).children().data("id");

            if (grid_id === line_id) {
                $(grid).find(".wrap-scale").removeClass("show");
            }
        });
        summaryTotal(line_id, 'Y');
        if (item.ItemType == 'Service') {
            getTimeOnTable();
        }
    }
};

function row_hovered(e) {
    $(this).siblings().removeClass("highlight");
};

function rowScaleClicked(e) {
    let item = {};
    let $row = $(this).parent().parent();
    let line_id = $row.data("line_id");
    item_master = db.get("tb_item_master").get(line_id);

    let scale = $(this).children(".scale");
    if ($(e.target).is($(".scale-up"))) {
        item = db.get("tb_order_detail").get(line_id);
        if (item.ItemType != 'Service') {
            if (item_master != undefined) {//isn't add on or copy item
                item.Qty = item.Qty + 1;
                item.PrintQty = item.PrintQty + 1;
                item_master.PrintQty = item.PrintQty;
            }
            else {
                item.Qty = item.Qty + 1;
                item.PrintQty = item.PrintQty + 1;
            }
            if (item.TypeDis == "Percent") {
                item.Total = (((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)))).toPrecision(3);
                item.DiscountValue = (item.Qty * item.UnitPrice) * item.DiscountRate / 100;

            }
            else {
                item.Total = (((item.Qty * item.UnitPrice) - item.DiscountRate)).toPrecision(3);
                item.DiscountValue = item.DiscountRate;
            }
            item.Total_Sys = (item.Total * local_currency.ratein).toPrecision(3);
            $(this).parent().parent()[0].cells[14].innerHTML = item.Total;

            db.update("tb_order_detail", item, "Line_ID");
            db.update("tb_item_master", item_master, "ID");
            scale.data("scale", item.Qty);
            scale.text(item.Qty);
            if (item_master != undefined) {
                updateSclaeGrids($row, item);
            }
            summaryTotal(line_id, 'N');
        }
        
    }
    if ($(e.target).is($(".scale-down"))) {
        confirmReduceQty(line_id, $(this), $row);
    }
};

function updateSclaeGrids($row, item) {
    let grids = $("#group-item-gridview .wrap-grid .grid");

    $.each(grids, function (i, grid) {
        let grid_id = $(grid).children().data("id");

        if (grid_id === item.Line_ID) {
            if (item.PrintQty > 0) {
                $(grid).find(".wrap-scale").addClass("show");
            }
            if (item.ItemStatus === 'new') {
             
                if (item.PrintQty == 0) {
                    db.table("tb_order_detail").delete(item.Line_ID);
                    $(grid).find(".wrap-scale").removeClass("show");
                    $row.remove();
                    let row_list = $('#item-listview').children().children();

                    $.each(row_list, function (i, row_remove) {
                        let data_id = parseInt($(row_remove).data("line_id"));
                        if (item.Line_ID === data_id) {
                           
                            $(row_remove).remove();
                        }

                    });
                }
            }
            else {
               
                if (item.Qty == 0) {
                    $(grid).find(".wrap-scale").removeClass("show");
                    $(grid).remove();
                    $row.remove();
                }
                if (item.PrintQty < 0) {
                    $(grid).find(".wrap-scale").addClass("show");
                }
                if (item.PrintQty == 0) {
                    $(grid).find(".wrap-scale").removeClass("show");
                }
            }
            $(grid).find(".scale").data("scale", item.PrintQty);
            $(grid).find(".scale").text(item.PrintQty);

        }

    });
  
};

function confirmReduceQty(line_id, parent, $row){
    let user_privillege = db.table("tb_user_privillege").get('P009');
    if (user_privillege.Used === false) {
        let dlg = new DialogBox({
            position: "top-center",
            content: {
                selector: "#admin-authorization",
                class: "login"
            },
            icon: "fas fa-lock",
            button: {
                ok: {
                    text: "Login",
                    callback: function (e) {
                        let access = accessSecurity(this.meta.content, 'P009');
                        if (access === false) {
                            this.meta.content.find('.error-security-login').text('You can not access ...!');
                            return;
                        }
                        else {
                            this.meta.content.find('.security-username').focus();
                            this.meta.setting.icon = "fas fa-lock fa-spin";
                            this.text = "Logging...";
                            this.meta.content.find('.error-security-login').text('');
                            this.meta.build(this.setting);
                            setTimeout(() => {
                                this.meta.build(this.setting);
                                this.meta.setting.icon = "fas fa-unlock-alt";
                                setTimeout(() => {
                                    this.meta.shutdown();
                                    initReduceQty(line_id, parent, $row);
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initReduceQty(line_id, parent, $row);
    }
};

function initReduceQty(line_id, parent, $row) {
    let item = {};
    item_master = db.get("tb_show_item").get(line_id);
    let scale = parent.children(".scale");
    item = db.get("tb_order_detail").get(line_id);
    if (item.ItemType != 'Service') {
        if (item_master != undefined) {
            let minus = 1;
            if (item.Qty < 1) {
                minus = parseFloat(item.Qty);
            }
            item.Qty = item.Qty - minus;
            item.PrintQty = item.PrintQty - minus;
            item_master.PrintQty = item.PrintQty;
        }
        else {
            let minus = 1;
            if (item.Qty < 1) {
                minus = parseFloat(item.Qty);
            }
            item.Qty = item.Qty - minus;
            item.PrintQty = item.PrintQty - minus;
            //item_master.PrintQty = item.PrintQty;
        }
        
        if (item.TypeDis == "Percent") {
            item.Total = (((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)))).toPrecision(3);
            item.DiscountValue = (item.Qty * item.UnitPrice) * item.DiscountRate / 100;

        }
        else {
            item.Total = (((item.Qty * item.UnitPrice) - item.DiscountRate)).toPrecision(3);
            item.DiscountValue = item.DiscountRate;
        }
        item.Total_Sys = (item.Total * local_currency.ratein).toPrecision(3);
        if (item.Qty <= 0) {
            if (item_master != undefined) {
                updateSclaeGrids($row, item);
            }
            summaryTotal(line_id, 'N');
            $row.remove();
            return;

        }
        parent.parent().parent()[0].cells[14].innerHTML = item.Total;

        db.update("tb_order_detail", item, "Line_ID");
        db.update("tb_item_master", item_master, "ID");
        scale.data("scale", item.Qty);
        scale.text(item.Qty);
        if (item_master != undefined) {
            updateSclaeGrids($row, item);
        }
        summaryTotal(line_id, 'N');
    }
    
};

//appy pagination
function apply_pagination_order($wrap_grid,items) {
    records = items;
    totalRecords = records.length;
    totalPages = Math.ceil(totalRecords / recPerPage);
    displayRecordsIndex = Math.max(page - 1, 0) * recPerPage;
    endRec = (displayRecordsIndex) + recPerPage;
    displayRecords = records.slice(displayRecordsIndex, endRec);
    singleItemFiltered($wrap_grid,displayRecords);
    $pagination.twbsPagination('destroy');
    $pagination.twbsPagination({
        totalPages: totalPages,
        visiblePages: 5,
        onPageClick: function (e, page) {
            displayRecordsIndex = Math.max(page - 1, 0) * recPerPage;
            endRec = (displayRecordsIndex) + recPerPage;
            displayRecords = records.slice(displayRecordsIndex, endRec);
            singleItemFiltered($wrap_grid, displayRecords);
        }
    });
};

//Calculator
let calc = null;
function openCalculator(e) {
    e.preventDefault();
    calc = new DialogCalculator();
    calc.accept(e => {
        let qty = parseFloat(calc.self.find(".navigator.output").text());       
        let grid = $(this).parent().parent();
        let line_id = parseInt($(grid).children().data('id'));
        calc.shutdown("before", function () {
            if (qty < 0) { return; }
            calulateQty(line_id,qty, grid);
        });
    });

    $(this).parent().addClass("active");
    $(this).parent().parent().parent().addClass("active")
        .siblings().removeClass("active")
        .find(".wrap-scale").removeClass("active");
};
//
function calulateQty(id, qty, grid) {
    let item_master = db.from("tb_item_master").where(function (json) {
        return json.ID === id;
    })[0];
    item = db.get("tb_order_detail").get(id);

    if (item.ItemStatus == 'new') {
        if (item.ItemType !== 'Service') {
            item.Qty = parseFloat(qty)
            item.PrintQty = parseFloat(qty)
            item_master.PrintQty = item.PrintQty;

            if (item.TypeDis == "Percent") {
                item.Total = ((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)));
                item.DiscountValue = (item.Qty * item.UnitPrice) * item.DiscountRate / 100;
            }
            else {
                item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate);
                item.DiscountValue = item.DiscountRate;
            }
            item.Total_Sys = item.Total * local_currency.ratein;
            item.UomID = item_master.UomID;
            if (item.PrintQty == 0) {
                item_master.PrintQty = 0;
                $(grid).find(".wrap-scale").removeClass("show").siblings(".order-calc").removeClass("show");
            }
            db.update("tb_order_detail", item, 'Line_ID');
            db.update("tb_show_item", item_master, 'ID');
            grid.find(".scale").data("scale", item_master.PrintQty);
            grid.find(".scale").text(item_master.PrintQty);
            summaryTotal(id, 'Y');
        }
    }
    else {
        if (item.ItemType !== 'Service') {
            if (item_master.PrintQty == 0) {
                item.Qty = (item.Qty + parseFloat(qty) - 1);
                item.PrintQty = ((item.PrintQty + parseFloat(qty)) - 1);
                item_master.PrintQty = item.PrintQty;
            }
            else {
                item.Qty = item.Qty + parseFloat(qty);
                item.PrintQty = (item.PrintQty + parseFloat(qty));
                item_master.PrintQty = item.PrintQty;
            }
            //if (item_master.PrintQty < 1) {
            //    item_master.PrintQty=item_master.PrintQty.toFixed(3);
            //}

            if (item.TypeDis == "Percent") {
                item.Total = ((item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100)));
                item.DiscountValue = (item.Qty * item.UnitPrice) * item.DiscountRate / 100;
            }
            else {
                item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate);
                item.DiscountValue = item.DiscountRate;
            }
            item.Total_Sys = item.Total * local_currency.ratein;
            item.UomID = item_master.UomID;
            if (item.PrintQty == 0) {
                item_master.PrintQty = 0;
                $(grid).find(".wrap-scale").removeClass("show").siblings(".order-calc").removeClass("show");
            }

            grid.find(".scale").data("scale", item_master.PrintQty);
            grid.find(".scale").text(item_master.PrintQty);
            db.update("tb_order_detail", item, 'Line_ID');
            db.update("tb_show_item", item_master, 'ID');
            
            summaryTotal(id, 'Y');
        }
    }
    
};
