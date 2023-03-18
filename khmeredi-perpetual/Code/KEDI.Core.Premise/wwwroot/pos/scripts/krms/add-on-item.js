function addNewItemAddon(scale, item_master) {
    //Declare order item
    let item = {};
    item.Line_ID = item_master.ID;
    item.OrderDetailID = 0;
    item.Code = item_master.Code;
    item.ItemID = item_master.ItemID;
    item.KhmerName = item_master.KhmerName 
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

    let lineId;
    if (db.from('tb_order_detail') == 0) {
        lineId = parentLevel + '2' + '1';
    } else {
        let addOn = db.from('tb_order_detail').where(w => { return w.ParentLevel == parentLevel });
        if (!!addOn) {
            lineId = parentLevel + '2' + (parseInt(addOn.length) + 1);
        } else {
            lineId = parentLevel + '2' + '1';
        }
     }
        item.Total_Sys = item.Total * local_currency.ratein;
        item.UomID = item_master.UomID;
        item_master.PrintQty = item.PrintQty;
        item.ItemPrintTo = item_master.PrintTo;
        item.Comment = '';
        item.ItemType = item_master.ItemType;
        item.Description = item_master.Description;
        item.ParentLevel = parentLevel;

        item.Line_ID = parseInt(lineId);
        item_master.PrintQty = 0;
        scale.parent().remove();
        insertAddonIndex(item)
        summaryTotal(item.Line_ID, 'Y');
}


function addOnItem(parent_row) {
    let line_id = parseInt($(parent_row).data('line_id'));
    parentLevel = line_id.toString();
    const $wrap_grid = $("#group-item-gridview .wrap-grid");
    $(".all-step").parent().find(":not(:first-child)").remove();
    let group_items = db.from("tb_group1").where(w => { return w.Name.includes('*')});
    groupItemFiltered($wrap_grid, group_items, 1, 0, 0, 0);
}

function insertAddonIndex(item) {
    let p_index, add_ons = new Array();
    let parents = db.from('tb_order_detail');
    if (parentLevel != "" && db.table("tb_order_detail").size > 0) {
        add_ons = db.from('tb_order_detail').where(w => { return w.ParentLevel == parentLevel; });
        let last_ch = add_ons[add_ons.length - 1];
        if (last_ch != undefined) {
            p_index = parents.findIndex(w => w.Line_ID == last_ch.Line_ID) + 1;
        }

        if (add_ons.length == 1) {
            p_index = parents.findIndex(w => w.ParentLevel == parentLevel) + 1;
        }
        else {
            let last_ch = add_ons[add_ons.length - 1];
            if (last_ch != undefined) {
                p_index = parents.findIndex(w => w.Line_ID == last_ch.Line_ID) + 1;
            }
        }
        parents.splice(p_index, 0, item);
        db.table('tb_order_detail').clear();
        db.insert('tb_order_detail', parents, 'Line_ID');
    } else {
        db.insert('tb_order_detail', item, 'Line_ID');
    }
}

