
$('#dbl-add-new-item').click(function () {
    let dlg = new DialogBox(
        {
            icon: "far fa-list-alt",
            close_button: true,
            content: {
                selector: ".add-new-item-content",
                class: "login"
            },
            caption: "Create New Item",
            position: "top-center",
            type: "ok-cancel",
            button: {
                ok: {
                    text: "Add"
                }
            }
        }
    );
    dlg.startup("after", function (dialog) {
        dialog.content.find('.add-new-item-name').focus();
    });
    dlg.confirm(function (e) {
        createNewItem(this.meta.content);
    });

    dlg.reject(function () {
        dlg.shutdown();
    });
});

function createNewItem(content) {
    let print = $(".add-new-item-printto", content).val();
    let name = $(".add-new-item-name", content).val();
    let price = parseFloat($(".add-new-item-price", content).val());
    if (print == null) {
        $(".add-new-item-printto", content).addClass('error');
    }
    else if (name == "") {
        $(".add-new-item-name").focus();
        $(".add-new-item-name", content).addClass('error');
    } else {
        let item_add = {}
        let item_master = db.from("tb_item_master").first(function (json) {
            return json.KhmerName == 'unknown';
        });
    
        let order = db.table('tb_order_detail').size;
        if (item_master == 0 || item_master == undefined) {
               
            let msg = new DialogBox(
                {
                    caption: "Information",
                    content: "Please add default item follow by Khmer Name name is 'unknown' and process type Standard",
                    position: "top-center",
                    type: "ok"
                })
            return;
        }

        item_add.Line_ID = parseInt(item_master.ItemID + '1' + (order + 1));
        item_add.OrderDetailID = 0;
        item_add.Code = item_master.Code;
        item_add.ItemID = item_master.ItemID;
        item_add.KhmerName = name;
        item_add.EnglishName = name;
        item_add.UnitPrice = price;
        item_add.Cost = item_master.Cost;
        item_add.Qty = 1;
        item_add.UoM = item_master.UoM;
        item_add.PrintQty = 1;
        item_add.DiscountRate = parseFloat(item_master.DiscountRate);
        item_add.TypeDis = item_master.TypeDis;
        item_add.ItemStatus = 'new';
        item_add.Currency = item_master.Currency;
        if (item_add.TypeDis === 'Percent') {
            item_add.Total = ((item_add.Qty * item_add.UnitPrice) * (1 - (item_add.DiscountRate / 100)));
            item_add.DiscountValue = ((item_add.Qty * item_add.UnitPrice) * item_add.DiscountRate) / 100;

        }
        else {
            item_add.Total = ((item_add.Qty * item_add.UnitPrice) - item_add.DiscountRate);
            item_add.DiscountValue = item_add.DiscountRate;
        }
        item_add.Total_Sys = item_add.Total * local_currency.ratein;
        item_add.UomID = item_master.UomID;
        item_add.ItemPrintTo = print;
        item_add.Comment = '';
        item_add.ItemType = item_master.ItemType;
        item_add.Description = item_master.Description;
        item_master.PrintQty = item_add.PrintQty;
        item_master.ID = item_add.Line_ID;
            
        db.insert('tb_order_detail', item_add, "Line_ID");
        
        summaryTotal(item_add.Line_ID, 'Y');
        $(".add-new-item-printto").val(0);
        $(".add-new-item-name").val('');
        $(".add-new-item-price").val('0');
        $(".add-new-item-name").focus();
        $(".add-new-item-printto").removeClass("error");
        $(".add-new-item-name").removeClass('error');
      }
};

$('.add-new-item-printto').change(function () {
    $(".add-new-item-printto").removeClass("error");
});

$('.add-new-item-name').focus(function () {
    $(".add-new-item-name").removeClass('error');
});
