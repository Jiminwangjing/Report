"use strict"
var db = new Warehouse();
var quote_details = [];
var _curencyID = 0;
let _itemID = 0

$.each($("[data-date]"), function (i, t) {
    setDate(t, moment(Date.now()).format("YYYY-MM-DD"));
});

function setDate(selector, date_value) {
    var _date = $(selector);
    _date[0].valueAsDate = new Date(date_value);
    _date[0].setAttribute(
        "data-date",
        moment(_date[0].value)
            .format(_date[0].getAttribute("data-date-format"))
    );
}

// tb master
var master = {
    BID: 0,
    ItemID: 0,
    UserID: 0,
    UomID: 0,
    PostingDate: "",
    SysCID: 0,
    TotalCost: 0,
    Active: 0,
    BOMDetail: new Array()
}

db.insert("tb_master", master, "ID");
// get Sys currency
$.ajax({
    url: "/Production/GetSysCurrency",
    type: "Get",
    dataType: "Json",
    async: false,
    success: function (response) {
        $.each(response, function (i, item) {
            $('.cur-class').text(item.Description);
            if (db.table("tb_master").size > 0) {
                db.from("tb_master").where(function (json) {
                    json.SysCID = item.ID;
                });
            }
        });
    }
});

// Get Group Defind Uom
$.ajax({
    url: "/Sale/GetGDUom",
    type: "Get",
    dataType: "Json",
    async: false,
    success: function (res) {
        db.insert("tb_uom", res, "ID");


    }

});

// Get Group Defind Uom
$(".modal-header").on("mousedown", function (mousedownEvt) {
    var $draggable = $(this);
    var x = mousedownEvt.pageX - $draggable.offset().left,
        y = mousedownEvt.pageY - $draggable.offset().top;
    $("body").on("mousemove.draggable", function (mousemoveEvt) {
        $draggable.closest(".modal-dialog").offset({
            "left": mousemoveEvt.pageX - x,
            "top": mousemoveEvt.pageY - y
        });
    });
    $("body").one("mouseup", function () {
        $("body").off("mousemove.draggable");
    });
    $draggable.closest(".modal").one("bs.modal.hide", function () {
        $("body").off("mousemove.draggable");
    });
});

// get item master
$("#show-list-itemster").click(function () {
    $("#ModalItem").modal("show");
    $.ajax({
        url: "/Production/GetItemMasters",
        type: "Get",
        dataType: "Json",
        success: function (response) {
            bindItemMaster(response);
            $(".modal-dialog #find-itemmaster").on("keyup", function (e) {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let rex = new RegExp(__value, "gi");
                let __itemmaster = $.grep(response, function (person) {
                    let name2 = person.EnglishName ?? "";
                    let barcode = person.Barcode ?? "";
                    return person.Code.match(rex) || barcode.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.KhmerName.toLowerCase().replace(/\s+/, "").match(rex)
                        || name2.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.Uom.match(rex);
                });
                bindItemMaster(__itemmaster);
            });
            //==================search===========================

        }
    });
});

// bind ItemMaster
function bindItemMaster(response) {
    const paging = new Kernel.DataPaging({
        pageSize: 10
    }).render(response, function (summary) {
        $(".modal-dialog #list-itemmater tr:not(:first-child)").remove();
        var _index = 1;
        $.each(summary.data, function (i, item) {
            var tr = $("<tr></tr>").on("dblclick", function () {

                dblItem(this);
            }).on("click", function () {
                clickCus(this);
            });
            let name2 = item.EnglishName ?? "";
            let barcode = item.Barcode ?? "";
            tr.append("<td style='min-width:3px;'>" + _index + "</td>")
                .append("<td hidden>" + item.ID + "</td>")
                .append("<td>" + item.Code + "</td>")
                .append("<td>" + barcode + "</td>")
                .append("<td>" + item.Uom + "</td>")
                .append("<td hidden>" + item.UomID + "</td>")
                .append("<td>" + item.KhmerName + "</td>")
                .append("<td>" + name2 + "</td>")
            $(".modal-dialog #list-itemmater").append(tr);
            _index++;
        });
        $(".modal-dialog .ck-data-loading").hide();
    });

    $(".modal-dialog #data-paging-customer").html(paging.selfElement);
    setTimeout(() => {
        $(".modal-dialog #find-cus").focus();
    }, 300);
}

function getItemDetails(itemID) {
    $.ajax({
        url: "/Production/GetItemDetails",
        data: { itemID: itemID },
        success: function (res) {

        }
    });
}

//  dbl item
function dblItem(c) {
    //let id = parseInt($(this).data("id"));
    getItemDetails(id);
    var id = parseInt($(c).find("td:eq(1)").text());
    var code = $(c).find("td:eq(2)").text();
    var khmername = $(c).find("td:eq(6)").text();
    var uom = $(c).find("td:eq(4)").text();
    var uomid = $(c).find("td:eq(5)").text();
    $("#item-id").val(khmername);
    $('#uom-id').val(uom);
    $('#code').val(code);
    $.ajax({
        url: "/Production/GetItemMaterial",
        data: { ItemID: parseInt(id) },
        contentType: "application/text",
        success: function (res) {
            if (res.length != 0) {
                db.table("tb_detail").clear();
                $("#list-detail tr:not(:first-child)").remove();
                $.each(res, function (i, item) {
                    const _detail = {
                        LineID: Date.now(),
                        BDID: item.BDID,
                        BID: item.BID,
                        ItemID: item.ItemID,
                        ItemCode: item.Code,
                        ItemName: item.KhmerName,
                        Qty: item.Qty,
                        UomID: item.UomID,
                        GUomID: item.GuomID,
                        Cost: item.Cost,
                        Amount: item.Amount,
                        Factor: item.Factor,
                    };
                    db.insert("tb_detail", _detail, "LineID");
                    bindDetail("#list-detail", _detail, false, false);
                    setSummary();
                    //
                    if (db.table("tb_master").size > 0) {
                        db.from("tb_master").where(function (json) {
                            json.BID = item.BID;
                            json.ItemID = id;
                            json.UomID = uomid;
                            json.Action = item.Active;
                        });
                    }
                    $("#active").prop("checked", item.Active);
                    setDate("#post-date", item.PostingDate.split("T")[0]);
                    //$("#total-amount").val(item.TotalCost);
                })
            }
            else {
                db.table("tb_detail").clear();
                $("#list-detail tr:not(:first-child)").remove();
                $("#total-amount").val('');
                if (db.table("tb_master").size > 0) {
                    db.from("tb_master").where(function (json) {
                        json.BID = 0;
                        json.ItemID = id;
                        json.UomID = uomid;
                    });
                }
                $("#active").prop("checked", false);

            }

        }
    });
    $("#item-id").prop("disabled", false);
    $("#ModalItem").modal("hide");
}

// click Cus
var _nameCus = "";
var _idCus = 0;
function clickCus(c) {
    _idCus = parseInt($(c).find("td:eq(1)").text());
    _nameCus = $(c).find("td:eq(3)").text();
    $(c).addClass("active").siblings().removeClass("active");
}

$("#cus-choosed").click(function () {
    $("#cus-id").val(_nameCus);
    db.from("tb_master").where(function (json) {
        json.CusID = _idCus;
    });
    $("#barcode-reading").prop("disabled", false).focus();
    $("#item-id").prop("disabled", false);
    $("#ModalCus").modal("hide");
});

function chooseItem(itemID, e) {
    let _item = db.table("tb_item").get(itemID);
    switch (e.type) {
        case "dblclick":
            $("#ModalItemDetail").modal("hide");
            createDetail(_item);
            break;
        default:
            _itemID = itemID;
            $(e.currentTarget).addClass("active").siblings().removeClass("active");
            break;
    }

}


// choose detail
$("#item-detail").click(function () {
    $("#ModalItemDetail").modal("show");
    var master = db.from("tb_master");
    $.ajax({
        url: "/Production/GetItemDetails",
        type: "Get",
        dataType: "Json",
        success: function (res) {
            bindItem(res);
            $(".modal-dialog #find-itemMasterdeatil").on("keyup", function (e) {
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let rex = new RegExp(__value, "gi");
                let __itemmaster = $.grep(res, function (person) {
                    let name2 = person.EnglishName ?? "";
                    let barcode = person.Barcode ?? "";
                    return person.Code.match(rex) || barcode.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.KhmerName.toLowerCase().replace(/\s+/, "").match(rex)
                        || name2.toLowerCase().replace(/\s+/, "").match(rex)
                        || person.Uom.match(rex);
                });
                bindItem(__itemmaster);
            });

            db.table("tb_item").clear();
            db.insert("tb_item", res, "ID");
        }
    });
});

function bindItem(items) {
    const paging = new Kernel.DataPaging({
        pageSize: 10
    }).render(items, function (summary) {
        $(".modal-dialog #list-itemdetail tr:not(:first-child)").remove();
        let _index = 1;
        if (items.length > 0) {
            $.each(summary.data, function (i, json) {
                var tr = $("<tr data-id=" + json.ID + "></tr>").on("dblclick click", function (e) {
                    chooseItem($(this).data("id"), e);
                });
                tr.append("<td style='min-width:3px; width:3px;'>" + _index + "</td>")
                    .append("<td>" + json.Code + "</td>")
                    .append("<td>" + json.Barcode + "</td>")
                    .append("<td>" + json.Uom + "</td>")
                    .append("<td>" + json.Cost + "</td>")
                    .append("<td>" + json.KhmerName + "</td>")
                    .append("<td>" + json.EnglishName + "</td>")
                $(".modal-dialog #list-itemdetail").append(tr);
                _index++;
            });
        }
        $(".ck-data-loading").hide();
    });
    $(".modal-dialog #data-paging-item").html(paging.selfElement);
}

$("#item-detail-choosed").click(function () {
    let _item = db.table("tb_item").get(_itemID);
    createDetail(_item);
    $(".modal-dialog-container").modal("hide");
});

$("#applied-amount").on("keyup", function () {
    if (this.value == "") {
        this.value = 0;
    }
    let __value = parseFloat(this.value);
    db.from("tb_master").where(function (item) {
        item.AppliedAmount = __value;
    });
});

function createDetail(detail) {
    const _detail = {
        LineID: Date.now(),
        BDID: 0,
        BID: 0,
        ItemID: detail.ID,
        ItemCode: detail.Code,
        ItemName: detail.KhmerName,
        Qty: 0,
        UomID: detail.UomID,
        GUomID: detail.GuomID,
        Cost: detail.Cost,
        Amount: detail.Amount,
        Factor: detail.Factor,
    };
    db.insert("tb_detail", _detail, "LineID");
    bindDetail("#list-detail", _detail, false, false);
}

function bindDetail(table_selector, detail, as_copy, disabled = false) {
    $(table_selector).updateRow(detail, "LineID", {
        //start_index: detail.LineID,
        hidden_columns: ["LineID", "BDID", "ItemID", "BID", "GUomID", "Factor", "UomName", "OpenQty"],
        html: [
            {
                insertion: "replace",
                column: "UomID",
                element: defineUom(detail, disabled)
            },
            {
                insertion: "replace",
                column: "ItemName",
                element: $("<input disabled value='" + detail.ItemName + "'>")
            },
            {
                insertion: "replace",
                column: "Qty",
                element: $("<input " + (disabled ? 'disabled' : '') + " type='number' value='" + (as_copy ? detail.OpenQty : detail.Qty) + "'>").on("keyup", function () {
                    let row = $(this).parent().parent();
                    let __detail = db.table("tb_detail").get(row.data("lineid"));
                    __detail.Qty = parseFloat(this.value);
                    __detail.OpenQty = __detail.Qty;
                    if (__detail.BDID > 0) {
                        if (__detail.Qty > __detail.OpenQty) {
                            return;
                        }
                    }
                    else {

                        if (this.value == '') {
                            $(this).parent().siblings("[cell-amount]").text(0);
                            return;
                        }

                        __detail.Qty = parseFloat(this.value);
                        __detail.OpenQty = __detail.Qty;
                        __detail.Amount = __detail.Amount = (__detail.Qty * __detail.Cost) * __detail.Factor;
                        $(this).parent().siblings("[cell-amount]").text(__detail.Amount);
                        db.insert("tb_detail", __detail, "LineID");
                    }
                    setSummary();
                })

            },
            {
                insertion: "replace",
                column: "Cost",
                element: $("<input disabled" + (disabled ? 'disabled' : '') + " data-number type='number' value='" + (detail.Cost * detail.Factor).toFixed(3) + "'>").on("keyup", function () {
                    let row = $(this).parent().parent();
                    let detail = db.table("tb_detail").get(row.data("lineid"));

                    if (this.value == '') {
                        $(this).parent().siblings("[cell-amount]").text(0);
                        return;
                    }

                    detail.Cost = parseFloat(this.value);
                    detail.Amount = detail.Qty * detail.Cost;
                    $(this).parent().siblings("[cell-amount]").text(detail.Qty * detail.Cost);
                    db.insert("tb_detail", detail, "LineID");
                    setSummary();
                })
            },
            {
                insertion: "replace",
                column: "Amount",
                element: $("<input disabled " + (disabled ? 'disabled' : '') + " data-number type='number' value='" + parseFloat(detail.Amount).toFixed(3) + "'>").on("keyup", function () {
                    let row = $(this).parent().parent();
                    let detail = db.table("tb_detail").get(row.data("lineid"));
                    detail.Amount = this.value;
                })
            },
        ],
        postbuild: function (data) {
            if (!disabled) {
                $(this).find("td:last-child").after($("<td></td>")
                    .append($("<span><i title='Remove' class='fas fa-trash-alt fa-lg text-danger csr-pointer'></i></span>").on("click", removeDetail)));
            } else {
                $(this).find("td:last-child").after($("<td><i title='Remove' class='fas fa-trash-alt fa-lg text-danger'></i></td>"));
            }
        }
    });

    function removeDetail() {
        let _row = $(this).parent().parent();
        const _dialog = new DialogBox({
            caption: "Removal",
            icon: "warning",
            type: "yes-no",
            content: "Are you sure you want to remove this item? You can stil choose it again."
        });
        _dialog.confirm(function () {
            _dialog.shutdown("during", function () {
                let rowdata = db.table("tb_detail").get(_row.data("lineid"));
                if (rowdata.BDID > 0) {
                    $.get("/Production/RemoveMaterialDetail", { detailID: rowdata.BDID }, function (model) {
                        if (model.Action == 1) {
                            _row.remove();
                        }
                        new ViewMessage({
                            summary: {
                                selector: "#error-summary"
                            }
                        }, model);
                        $(window).scrollTop(0);
                    });
                } else {
                    _row.remove();
                }
                if ($(table_selector).children("tr:not(:first-child)").length === 0) {
                    $("#include-vat-id").prop("disabled", true);
                }

            });

            db.table("tb_detail").delete(_row.data("lineid"));
            setSummary($("#include-vat-id").prop("checked"), $("#sub-dis-id").val());
        });
    }
}

var defined_uoms = [];
function defineUom(detail, disabled = false) {
    defined_uoms = db.from("tb_uom").where(function (json) {
        return json.GroupUoMID == detail.GUomID;
    });
    let select = $("<select " + (disabled ? 'disabled' : '') + "></select>").on("change", function () {
        detail.UomID = parseInt(this.value);
        detail.UomName = $(this).find("option:selected").text();
        var far = db.from("tb_uom").first(function (item) {
            return item.UoMID == detail.UomID;
        });
        if (far != undefined) {
            db.from("tb_detail").where(function (item) {
                if (detail.LineID == item.LineID) {
                    item.Factor = far.Factor
                    item.Amount = item.Qty * item.Cost * item.Factor;
                    bindDetail("#list-detail", item, false, false);
                    setSummary();
                }
            });
        }
    });
    $.each(defined_uoms, function (i, item) {
        $("<option value='" + item.UnitofMeasure.ID + "'>" + item.UnitofMeasure.AltUomName + "</option>").appendTo(select);
    });

    select.val(detail.UomID);
    return select;
}

function updateLineItem(checked, dis_type, is_copy) {
    let rows = $("#list-detail").find("tr:not(:first-child)");
    let vat_value = $("#vat-id").val();
    let vat_text = $("#vat-id option:selected").text();
    if (rows.length > 0) {
        $.each(rows, function (i, row) {
            let detail = db.table("tb_detail").get($(row).data("lineid"));
            if (checked) {
                detail.VatRate = vat_value;
                $(row).find("td[cell-vatrate]").html(vat_text);
            } else {
                detail.VatRate = 0;
                $(row).find("td[cell-vatrate]").html("0 %");
            }
        });
    }
}


function openAppliedAmount(balanceDue = 0, appliedAmount = 0) {
    if (balanceDue > 0 && appliedAmount <= balanceDue) {
        $("#applied-amount").prop("readonly", false);
        return true;
    }
    return false;
}

function setSummary() {
    let subtotal = 0;
    if (db.table("tb_detail").size > 0) {
        db.from("tb_detail").where(function (item) {
            subtotal += parseFloat(item.Qty * item.Cost * item.Factor);
        });
    }
    if (db.table("tb_master").size > 0) {
        db.from("tb_master").where(function (item) {
            item.TotalCost = parseFloat(subtotal);
        });
    }
    $("#total-amount").val(subtotal.toFixed(3));

}

$.each($("input[data-number]"), function (i, input) {
    $(input).asNumber();
});

$("#submit-item").on("click", function (e) {
    db.from("tb_master").where(function (item_master) {
        item_master.BID = item_master.BID;
        item_master.PostingDate = $("#post-date").val();
        item_master.Active = $("#active").prop("checked");
        item_master.BOMDetails = db.from("tb_detail") == 0 ? new Array() : db.from("tb_detail");
    });
    var dialogSubmit = new DialogBox({
        content: "Are you sure you want to save the item?",
        type: "yes-no",
        icon: "warning"
    });

    dialogSubmit.confirm(function () {
        $.ajax({
            url: "/Production/CreateBOMaterial",
            type: "POST",
            data: $.antiForgeryToken({ data: JSON.stringify(db.from("tb_master")[0]) }),
            success: function (data) {
                if (data.Model.Action == 1) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        }
                    }, data.Model).refresh(1500);

                }
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, data.Model);
                $(window).scrollTop(0);
            }
        });
        this.meta.shutdown();
    });
});

$("#btn-find").on("click", function () {
    $("#invoice-no").val("").prop("readonly", false).css({
        "border": "1.5px solid #87ceeb"
    }).focus();
    $("#btn-addnew").prop("hidden", false);
    $("#btn-find").prop("hidden", true);
});


function getSaleItemMasters(master, key, detailKey, copyType = 0) {
    if (!!master) {
        $("#item-id").prop("disabled", false);
        if (master[key] > 0) {
            $("#applied-amount").prop("readonly", true);
        }
        setRequested(master, key, detailKey, copyType);
    } else {
        const _dialog = new DialogBox({
            caption: "Searching",
            icon: "danger",
            content: "Sale AR Not found!",
            close_button: "none"
        });
        _dialog.confirm(function () {
            location.reload();
        });
    }

}

function setRequested(item, key, detailKey, copyType = 0) {
    $.get("/Sale/GetCustomer", { id: item.CusID }, function (cus) {
        $("#cus-id").val(cus.Name);
    });
    $("#include-vat-id").prop("disabled", true);
    $("#ware-id").val(item.WarehouseID);
    $("#ref-id").val(item.RefNo);
    $("#cur-id").val(item.LocalCurrencyID);
    $("#include-vat-id").prop("checked", item.IncludeVat).prop("disabled", true);
    $("#sta-id").val(item.Status);
    $("#sub-id").val(item.SubTotal);
    $("#sub-dis-id").val(item.TypeDis);
    $("#dis-id").val(item.DisRate);
    $("#fee-note").val(item.FeeNote);
    if (item.FeeAmount) {
        $("#fee-amount").val(item.FeeAmount);
    }
    $("#remark-id").val(item.Remarks);
    setDate("#post-date", new Date(item.PostingDate.split("T")[0]));
    setDate("#document-date", new Date(item.DocumentDate.split("T")[0]));
    setVAT(item);
    $("#total-id").val(item.TotalAmount);

    db.table("tb_detail").clear();
    $("#list-detail").find("tr:not(:first-child)").remove();
    $.each(item[detailKey], function (i, detail) {
        if (!detail.Delete) {
            let _detail = { LineID: Date.now() };
            _detail = $.extend(_detail, detail);
            db.insert("tb_detail", _detail, "LineID");
            if (copyType > 0) {
                bindDetail("#list-detail", _detail, true, false);
            } else {
                bindDetail("#list-detail", _detail, false, item["SARID"] > 0);
            }
        }
    });

    if (item[detailKey]) {
        if (item.VatValue > 0) {
            $("#vat-value").val(item.VatValue);
        }
        delete item[detailKey];
    }

    db.table("tb_master").clear();
    db.insert("tb_master", item, key);
    if (item.Status === "open") {
        db.from("tb_master").where(function (json) {
            // 1: Quotation, 2: Order, 3: Delivery, 4: AR   
            json.CopyKey = item.InvoiceNo;
            json.CopyType = copyType;
            json.BasedCopyKeys += "/" + item.InvoiceNo;
            setSourceCopy(json.BasedCopyKeys);
        });
    }
    if (item.SARID > 0) {
        setDisabled();
    }
}

function setDisabled() {
    $("#submit-item").prop("disabled", true);
    $("#item-id").prop("disabled", true);
    $("#sub-dis-id").prop("disabled", true);
    $("#dis-id").prop("disabled", true);
    $("#remark-id").prop("disabled", true);
    $("#include-vat-id").prop("disabled", true);
    $("#cur-id").prop("disabled", true);
    $("#ref-id").prop("disabled", true);
    $("#ware-id").prop("disabled", true);
    $("#copy-from").prop("disabled", true);
    $("#show-list-cus").addClass("csr-default").off();
    $("#fee-note").prop("disabled", true);
    $("#fee-amount").prop("disabled", true);
    $.each($("[data-date]"), function (i, t) {
        $(t).prop("disabled", true);
    });
}
