//Search Text
var $pagination = $('#pagination'),
    totalRecords = 0,
    records = [],
    displayRecords = [],
    recPerPage = 28,
    page = 1,
    totalPages = 0,
    currentPage = 1;
$("#item-search").on('keyup', function () {
    let item_filtered = [];
    setTimeout(function () {
        let value = $("#item-search").val().toLowerCase();
        item_filtered = db.from("tb_item_master").where(w => {
            return w.EnglishName.toLowerCase().includes(value) ||
                w.KhmerName.includes(value) ||
                w.Code.toLowerCase().includes(value);
        });
        apply_pagination(item_filtered);
    }, 180);

});

//appy pagination
function apply_pagination(items) {
    records = items;
    totalRecords = records.length;
    totalPages = Math.ceil(totalRecords / recPerPage);
    displayRecordsIndex = Math.max(page - 1, 0) * recPerPage;
    endRec = (displayRecordsIndex) + recPerPage;
    displayRecords = records.slice(displayRecordsIndex, endRec);
    singleItemSearch(displayRecords);
    $pagination.twbsPagination('destroy');
    $pagination.twbsPagination({
        totalPages: totalPages,
        visiblePages: 3,
        onPageClick: function (e, page) {
            displayRecordsIndex = Math.max(page - 1, 0) * recPerPage;
            endRec = (displayRecordsIndex) + recPerPage;
            displayRecords = records.slice(displayRecordsIndex, endRec);
            singleItemSearch(items);
        }
    });
}
function singleItemSearch() {
    const $wrap_grid = $("#group-item-gridview .wrap-grid");
    db.map("tb_show_item_barcode").clear();
    $wrap_grid.children().remove();
    $.each(displayRecords, function (i, item) {
        if (item.Image === null || item.Image === '') {
            item.Image = 'no-image.jpg';
        }
        let dis = "%";
        if (item.TypeDis != "Percent")
            dis = "";
        let KhmerName = item.KhmerName;
        if (item.UoM !== 'manual')
            KhmerName = item.KhmerName + ' (' + item.UoM + ')';

        let $grid = $("<div class='grid'​​​ " + displayRecords[i] + "></div>");
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
            '<div class="price">' + item.UnitPrice + '</div>'
        ).appendTo($grid);
        if (item.KhmerName !== item.EnglishName) {
            $('<div class="grid-caption" title=' + item.EnglishName + ' >' + item.Code + ' ' +item.EnglishName + '</div>').appendTo($grid);
        }
        else {
            $('<div class="grid-caption" title=' + item.EnglishName + ' >' + item.Code+ '</div>').appendTo($grid);
        }
        $wrap_grid.append($grid.on("click", bindToTable));
    });
}
$(".close-barcode").on('click', function () {
    //clearNewBarcode();
});
$("#open-barcode").on('click', function () {
    //clearNewBarcode();
});
//Search barcode
let count = 0;
let counts = 0;//for alter uom
$("#item-search-barcode").keypress(function (event) {
   
    let item = {};
    let barcode;
    let $wrap_grid = $("#group-item-gridview .wrap-grid");
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        if (db.from("tb_show_item_barcode") === 0) {
            //clearOrder();
            $wrap_grid.children().remove();
            $pagination.twbsPagination('destroy');
        }
        barcode = $("#item-search-barcode").val().toLowerCase();

        var scaleprice = barcode.slice(7, 12);
        var scale_unitprice = Number(scaleprice / 100).toFixed(3);

        let item_filtered = $.ajax({
            url: "/POS/GetItemMasterByBarcode",
            async: false,
            type: "GET",
            dataType: "JSON",
            data: {
                pricelist: setting.PriceListID,
                barcode: barcode,
                scaleprice: scale_unitprice
            },
            error: function () {
                $("#item-search-barcode").val("");
            }
        }).responseJSON;

        if (item_filtered == null || item_filtered == undefined) {return;}

        $.each(item_filtered, function (i, item_master) {
            let uom_definded = db.from('tb_group_uom_defined').where(w => { return w.GroupUoMID == item_master.GroupUomID; });
            if (item_master.UoM.toLowerCase() == "kg") {
                let item_ordered = db.table("tb_show_item_barcode").get(item_master.ID);
                if (item_ordered == undefined) {
                    count = item_master.Qty;
                }
                else {
                    let item_update = db.get('tb_show_item_barcode').get(item_master.ID);
                    count = item_update.PrintQty + item_master.Qty;
                }
            } else {
                let item_ordered = db.table("tb_show_item_barcode").get(item_master.ID);
                if (item_ordered == undefined) {
                    count = 1;
                }
                else {
                    let item_update = db.get('tb_show_item_barcode').get(item_master.ID);
                    count = item_update.PrintQty + 1;
                }
            }

            //let item_ordered = db.table("tb_show_item_barcode").get(item_master.ID);
            //if (item_ordered == undefined) {
            //    count = 1;
            //}
            //else {
            //    let item_update = db.get('tb_show_item_barcode').get(item_master.ID);
            //    count = item_update.PrintQty + 1;
            //}

            if (item_master.UomID == uom_definded[0].AltUOM) {
                item.Line_ID = item_master.ID;
                item.OrderDetailID = 0;
                item.Code = item_master.Code;
                item.ItemID = item_master.ItemID;
                item.KhmerName = item_master.KhmerName;
                item.EnglishName = item_master.EnglishName; 
                item.UnitPrice = parseFloat(item_master.UnitPrice);
                item.Cost = item_master.Cost;
                item.Qty = count;
                item.UoM = item_master.UoM;
                item.PrintQty = count;
                item.DiscountRate = parseFloat(item_master.DiscountRate);
                item.TypeDis = item_master.TypeDis;
                item.ItemStatus = 'new';
                item.Currency = item_master.Currency;

                if (item.TypeDis === 'Percent') {
                    item.Total = (item.Qty * item.UnitPrice) * (1 - (item.DiscountRate / 100));
                    item.DiscountValue = ((item.Qty * item.UnitPrice) * item.DiscountRate) / 100;
                }
                else {
                    item.Total = ((item.Qty * item.UnitPrice) - item.DiscountRate);
                    item.DiscountValue = item.DiscountRate;
                }
                item.Total_Sys = (item.Total * local_currency.ratein);
                item.UomID = item_master.UomID;
                item_master.PrintQty = count;
                item.ItemPrintTo = item_master.PrintTo;
                item.Comment = '';
                item.ItemType = item_master.ItemType;
                item.Description = item_master.Description;
                db.insert("tb_order_detail", item, 'Line_ID');
                db.insert('tb_item_master', item_master, 'ID');
                singleItemFilteredBarcode($wrap_grid, item_master);
                $("#item-search-barcode").val("");
                summaryTotal(item.Line_ID, 'Y');
               
            }
            else {
                let item_ordered = db.table("tb_show_item_barcode").get(item_master.ID);
                if (item_ordered == undefined) {
                    counts = 0;
                }
                else {
                    let item_update = db.get('tb_show_item_barcode').get(item_master.ID);
                    counts = item_update.PrintQty;
                }
                item_master.Qty = counts;
                item_master.PrintQty = counts;
                singleItemFilteredBarcode($wrap_grid, item_master);
            }

        });
        $("#item-search-barcode").val("");
    }
});

function singleItemFilteredBarcode($wrap_grid, item) {
    let grids = $("#group-item-gridview .wrap-grid .grid");
    if (db.from('tb_show_item_barcode') === 0) {
        db.insert("tb_show_item_barcode", item, "ID");
        //db.insert('tb_show_item', item_master, 'ID');
        $wrap_grid.children().remove();
        if (item.Image === null || item.Image === '') {
            item.Image = 'no-image.jpg';
        }
        let KhmerName = item.KhmerName;
        if (item.UoM !== 'manual')
            KhmerName = item.KhmerName + ' (' + item.UoM + ')';

        let dis = "%";
        if (item.TypeDis !== "Percent")
            dis = "";

        let $grid = $("<div class='grid'></div>");
        $grid.append('<div data-bonus="' + item.DiscountRate + '" data-id="' + item.ID + '" hidden></div>');
        $grid.append('<div class="grid-caption" title=' + KhmerName + ' >' + KhmerName + ' </div > ');

        let $grid_image = $("<div class='grid-image'></div>");
        $grid_image.append($("<i class='fas fa-calculator fa-lg fn-sky order-calc'></i>").on("click", openCalculator));

        let $wrap_scale = $('<div class="wrap-scale">'
            + '<i class="scale-down">-</i>'
            + '<label data-scale="' + item.PrintQty + '" class="scale">' + item.PrintQty + '</label>'
            + '<i class="scale-up">+</i>'
            + '</div>');
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
            $('<div class="grid-caption" title=' + item.EnglishName + ' >' + item.Code + ' ' +item.EnglishName + '</div>').appendTo($grid);
        }
        else {
            $('<div class="grid-caption" title=' + item.EnglishName + ' >' + item.Code + '</div>').appendTo($grid);
        }
        $wrap_grid.append($grid.on("click", bindToTable));
    }
    else {
        if (item.Image === null || item.Image === '') {
            item.Image = 'no-image.jpg';
        }
        let KhmerName = item.KhmerName;
        if (item.UoM !== 'manual')
            KhmerName = item.KhmerName + ' (' + item.UoM + ')';

        let dis = "%";
        if (item.TypeDis !== "Percent")
            dis = "";

        let $grid = $("<div class='grid'></div>");
        $grid.append('<div data-bonus="' + item.DiscountRate + '" data-id="' + item.ID + '" hidden></div>');
        $grid.append('<div class="grid-caption" title=' + KhmerName + ' >' + KhmerName + ' </div > ');

        let $grid_image = $("<div class='grid-image'></div>");
        $grid_image.append($("<i class='fas fa-calculator fa-lg fn-sky order-calc'></i>").on("click", openCalculator));

        let $wrap_scale = $('<div class="wrap-scale">'
            + '<i class="scale-down">-</i>'
            + '<label data-scale="' + item.PrintQty + '" class="scale">' + item.PrintQty + '</label>'
            + '<i class="scale-up">+</i>'
            + '</div>');
        if (item.PrintQty != 0) {
            $wrap_scale.addClass("show");
        } else {
            $wrap_scale.removeClass("show");
        }
        $wrap_scale.appendTo($grid_image)
        if (item.DiscountRate > 0)
            $('<div class="discount">' + item.DiscountRate + ' ' + dis + ' </div>').appendTo($grid_image);
        $grid_image.append('<img src="/Images/' + item.Image + '"/>',
            '<div class="price">' + item.UnitPrice + '</div>'
        ).appendTo($grid);
        if (item.KhmerName !== item.EnglishName) {
            $('<div class="grid-caption" title=' + item.EnglishName + ' >' + item.EnglishName + '</div>').appendTo($grid);
        }
        else {
            $('<div class="grid-caption" title=' + item.EnglishName + ' >' + + '</div>').appendTo($grid);
        }
       
        let item_ordered = db.table('tb_show_item_barcode').get(item.ID);
        if (item_ordered == undefined) {
            $wrap_grid.append($grid.on("click", bindToTable));
        }
        else {
            $.each(grids, function (i, grid) {
                let grid_id = parseInt($(grid).children().data("id"));
                if (grid_id === parseInt(item.ID)) {
                    $(grid).find(".scale").data("scale", item.PrintQty);
                    $(grid).find(".scale").text(item.PrintQty);

                }
            });
        }
        db.insert("tb_show_item_barcode", item, "ID");
    }
}