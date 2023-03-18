let comment_line_id = 0;
let com_id = 0;
$('.btn-add-item-comment').click(function () {
    
    let name = $('#txt-item-comment-search').val();
    if (name == "" || name == null) {
        return;
    }
    let loader = null;
    loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
    $.ajax({
        url: '/POS/CreateItemComment',
        type: 'POST',
        dataType: 'JSON',
        data: {
            Description:name
        },
        success: function (response) {
            let comment = {};
            comment.ID = response.ID;
            comment.Description = response.Description;
            db.insert('tb_item_comment', comment, 'ID');

            let item_choosed = {};
            let lengt = db.table('tb_item_comment_choosed').size;
            item_choosed.no = lengt + 1;
            item_choosed.desc = response.Description;
            db.insert('tb_item_comment_choosed', item_choosed, 'no');
            bindRowItemCommentChoosed(response.ID);

            $.bindRows(".item-comment-listview", db.from('tb_item_comment'), 'ID', {

                text_align: [{ "Description": "left" }],
                highlight_row: 0,
                html: [
                    {
                        column: -1,
                        insertion: "replace",
                        element: '<i style="color:green;" class="fa fa-arrow-circle-right"></i>',
                        listener: ["click", function (e) {
                            $(this).parent().parent().remove();
                            rowItemCommentChoose(this);
                        }]
                    }
                ],
                show_key: false,
                mouseover: row_hovered,
                click: clickRowItemComment,
                hidden_columns: ["ID"]

            });
            loader.close();
        }

    })
});

$('.btn-delete-item-comment').click(function () {
    let loader = null;
    loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
    db.table('tb_item_comment').delete(com_id);
    $.bindRows(".item-comment-listview", db.from('tb_item_comment'), 'ID', {

        text_align: [{ "Description": "left" }],
        highlight_row: 0,
        html: [
            {
                column: -1,
                insertion: "replace",
                element: '<i style="color:green;" class="fa fa-arrow-circle-right"></i>',
                listener: ["click", function (e) {
                    $(this).parent().parent().remove();
                    rowItemCommentChoose(this);
                }]
            }
        ],
        show_key: false,
        mouseover: row_hovered,
        click: clickRowItemComment,
        hidden_columns: ["ID"]
    });
    $.ajax({
        url: '/POS/DeleteItemComment',
        type: 'POST',
        data: {
            ID: com_id
        },
        success: function (response) {
            loader.close();
        }
    })
});

$('#txt-item-comment-search').keyup(function () {
    let value = $(this).val().toString().toLowerCase();
    let item_comment = db.from('tb_item_comment').where(w => { return w.Description.toLowerCase().includes(value) })
    $.bindRows(".item-comment-listview", item_comment, 'ID', {

        text_align: [{ "Description": "left" }],
        highlight_row: 0,
        html: [
            {
                column: -1,
                insertion: "replace",
                element: '<i style="color:green;" class="fa fa-arrow-circle-right"></i>',
                listener: ["click", function (e) {
                    $(this).parent().parent().remove();
                    rowItemCommentChoose(this);
                }]
            }
        ],
        show_key: false,
        mouseover: row_hovered,
        click: clickRowItemComment,
        hidden_columns: ["ID"]

    });
});

function rowInitFromComment(id) {
    clearItemComment();
    //let $row = $(row);
    comment_line_id = id; //parseInt($row.data("line_id"));
    let item = db.get("tb_order_detail").get(comment_line_id);
    if (item.Comment != null) {
        var splitCom = [];
        splitCom = item.Comment.split(",");
        for (var i = 0; i < splitCom.length; i++) {
            let item_choosed = {};
            item_choosed.no = i + 1;
            item_choosed.desc = splitCom[i];
            if (splitCom[i] != '') {
                db.insert('tb_item_comment_choosed', item_choosed, 'no');
            }
        }
        bindRowItemCommentChoosed(0);
    }
    $("#detail-item-comment").text("Detail :   " + item.KhmerName);
    let dlg = new DialogBox(
        {
            caption: "Comment Order",
            content: {
                selector: "#item-comment-order"
            },
            position: "top-center",
            type: "ok-cancel",
            button: {
                ok: {
                    text: "Apply"
                }
            },
            animation: {
                shutdown: {
                    animation_type: "slide-up"
                }
            }
        }
    );
    dlg.startup("after", function (dialog) {
        let loader = null;
        loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
        $.ajax({
            url: '/POS/GetItemComment',
            type: 'GET',
            dataType: 'JSON',
            success: function (response) {
                db.insert('tb_item_comment', response, 'ID');
                $.bindRows(".item-comment-listview", db.from('tb_item_comment'), 'ID', {

                    text_align: [{ "Description": "left" }],
                    highlight_row: 0,
                    html: [
                        {
                            column: -1,
                            insertion: "replace",
                            element: '<i style="color:green;" class="fa fa-arrow-circle-right"></i>',
                            listener: ["click", function (e) {
                                $(this).parent().parent().remove();
                                rowItemCommentChoose(this);
                            }]
                        }
                    ],
                    show_key: false,
                    mouseover: row_hovered,
                    click:clickRowItemComment,
                    hidden_columns: ["ID"]

                });
                loader.close();
            }
        });
    });
    dlg.confirm(function () {
        itemCommentApply();
        dlg.shutdown();
    });
    dlg.reject(function () {
        dlg.shutdown();
    });
};

function clickRowItemComment() {
    com_id = parseInt($(this).data('id'));
};

function rowItemCommentChoose(row) {
    let item_choosed = {};
    let id = parseInt($(row).parent().parent().data('id'));
    let item_comment = db.table('tb_item_comment').get(id);
    let lengt = db.table('tb_item_comment_choosed').size;
    item_choosed.no = lengt + 1;
    item_choosed.desc = item_comment.Description;
    db.insert('tb_item_comment_choosed', item_choosed, 'no');
    bindRowItemCommentChoosed(id);
};

function bindRowItemCommentChoosed(selected_id) {
   
    $.bindRows(".item-comment-choosed-listview", db.from('tb_item_comment_choosed'), 'no', {
        text_align: [{ "desc": "left" }],
        highlight_row: selected_id,
        html: [
            {
                column: -1,
                insertion: "replace",
                element: '<i class="fa fa-trash trash"></i>',
                listener: ["click", function (e) {
                    $(this).parent().parent().remove();
                    let no = parseInt($(this).parent().parent().data('no'));
                    db.table('tb_item_comment_choosed').delete(no);
                }]
            }
        ],
        show_key: false,
        mouseover: row_hovered,
        hidden_columns: ["delete"]
    });
};

function itemCommentApply() {
    let item = db.get("tb_order_detail").get(comment_line_id);
    let size = db.table('tb_item_comment_choosed').size;
    if (size != 0) {
        item.Comment = '';
        $.each(db.from('tb_item_comment_choosed'), function (i, item_choosed) {
            if ((i + 1) < size) {
                item.Comment += item_choosed.desc + ',';
            }
            else {
                item.Comment += item_choosed.desc;
            }
        });
    }
    else {
        item.Comment = '';
    }
    db.insert('tb_order_detail', item, 'Line_ID');
};

function clearItemComment() {
    db.map('tb_item_comment').clear();
    db.map('tb_item_comment_choosed').clear();
    bindRowItemCommentChoosed(0);
};