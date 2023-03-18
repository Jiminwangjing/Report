//Table transaction

let group_table = $.ajax({
    url: "/POS/Table",
    async: false,
    type: "GET",
    dataType: "JSON"

}).responseJSON;
db.addTable("tb_table_group", group_table.GroupTables, "ID");
db.addTable("tb_table", group_table.Tables, "ID");

db.select("tb_table_group").each(function (i, group) {
    let group1 = $('<div data-id= "' + group.ID + '" class="step">' + group.Name + '</div>');
    let group2 = $('<div data-id= "' + group.ID + '" class="step">' + group.Name + '</div>');
    $("#move-table-breadcrumb").append(group1.on("click", clickGroupTableOnMove));
    $(".table-group").append(group2.on("click", clickGroupTable));
    
});
getTables(0);

function getTables(group_id) {
    $("#table-item-gridview").children().empty();
    let table_filtered = [];
    if (group_id === 0) {
        table_filtered = db.from("tb_table");
    }
    else if (group_id === -1) {
        var query = ($("#table_search").val()).toLowerCase();
        table_filtered = db.from("tb_table").where(w => {
            return w.Name.toLowerCase().includes(query);
        });
    }
    else {
        table_filtered = db.from("tb_table").where(w => {
            return w.GroupTableID === group_id;
        });
    }
    $.each(table_filtered, function (i, table) {
        if (table.Image === null || table.Image === '') {
            table.Image = 'no-image.jpg';
        }
        let userid = "user" + table.ID;
        let grid = $("<div class='grid'></div>")
            .append('<div data-id="' + table.ID + '" data-name="' + table.Name + '" hidden></div>')
            .append('<div class="grid-caption">' + table.Name + '</div>');
        let grid_image = $("<div class='grid-image'></div>");

        grid_image.append('<img src="/Images/table/' + table.Image + '"/>');
        grid.append(grid_image);
        grid.append("<div id='" + userid + "' class='grid-subtitle'></div>");
        
        $("#table-item-gridview").children().append(grid.on("click", tableClicked));
    });
}
function clickGroupTableOnMove() {
    let group_id = $(this).data("id");
    getTableAvailable(group_id);
    $(this).siblings().removeClass("active");
    $(this).addClass("active");
}
function clickGroupTable() {
    let group_id = $(this).data("id");
    $("#table-item-gridview").children().empty();
    getTables(group_id);
    $(this).siblings().removeClass("active");
    $(this).addClass("active");
    
}

function tableClicked(e) {

    $("#panel-group-items").addClass("show");
    $("#panel-group-items").removeClass("hide");
    $(".nav-header .min-max").addClass("show");

    table_info.id = $(this).children().data("id");
    table_info.name = $(this).children().data("name");
    clickOnOrderNo(table_info.id, 0);

}
$("#all-table-move").on('click', function () {
    getTableAvailable(0);
    $(this).siblings().removeClass("active");
    $(this).addClass("active");
});
$("#all-table").on('click', function () {
    getTables(0);
    $(this).siblings().removeClass("active");
    $(this).addClass("active");
});
//Search table in grid
$('#table_search').on('keyup', function () {
    getTables(-1);
   
});
//Search table in grid
$('input[name=search-move-table]').on('keyup', function () {
   
    var query = ($(this).val()).toLowerCase();
    $('.move-table-list .rc-container').each(function () {
        var $this = $(this);
        if ($this.text().toLowerCase().indexOf(query) === -1)
            $this.closest('.rc-container').hide();
        else $this.closest('.rc-container').show();
    });

});


$("#dbl-move-table").click(function (e) {
    confirmMoveTable();
});
function confirmMoveTable() {
    let user_privillege = db.table("tb_user_privillege").get('P001');
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
                        let access = accessSecurity(this.meta.content, 'P001');
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
                                    initFormMove();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormMove();
    }
}
function initFormMove() {
    if (order_master.OrderID !== 0) {

        let msg = new DialogBox(
            {
                caption: "Move Table ( " + table_info.name + " )",
                content: {
                    selector: "#move-table"
                },
                position: "top-center",
                type: "ok-cancel"
            }
        );
        msg.setting.button.ok.text = "Move";
        msg.setting.animation.shutdown.animation_type = "slide-up";

        getTableAvailable(0);
        msg.confirm(moveTable);
        msg.reject(function (e) {
            this.meta.shutdown();
        });
    }
    else {
        let msg = new DialogBox(
            {
                caption: "Information",
                content: "Can not move this table...!",
                position: "top-center",
                type: "ok",
                icon: "info"
            }
        );
        msg.setting.button.ok.callback = function (e) {
            this.meta.shutdown();
        };
    }
}

function getTableAvailable(group_id) {
    let out = "";
    let tables = $.ajax({
        url: '/POS/GetTableAvailable',
        type: 'GET',
        async: false,
        data: { group_id: group_id, tableid: table_info.id }
    }).responseJSON;
    
    $.each(tables, function (i, table) {
        out += "<label class='rc-container mx1'>"
            + "<input data-id=" + table.ID + " name='table' type='radio'>"
            + "<span class='radiomark'></span>"
            + table.Name
            + "</label>"
            + "<hr/>";
    });
    $('.move-table-list').html(out);
    
}
function moveTable() {
   
    let old_tableid = table_info.id;
    let new_tableid = 0;
    let tables = $("input[name='table']");
   
    $.each(tables, function (i, table) {
        if ($(table).prop("checked")) {
            new_tableid = $(table).data("id");
        }
    });
    if (new_tableid !== 0) {
        let loader = null;
        loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
        $.ajax({
            url: '/POS/MoveTable',
            type: 'POST',
            data: { old_id: old_tableid, new_id: new_tableid },
            success: function () {
                loader.close();
            }
        });
        this.meta.shutdown();
        $("#panel-group-items").removeClass("show");
        $("#panel-group-tables").addClass("show");
       

    } else {
        let info = new DialogBox({
            caption: "Cannot Move Table",
            position: "top-center",
            icon: "danger",
            content: "Please select any table to move!",
            animation: {
                startup: {
                    delay: 0,
                    duration: 0
                }
            }
        });
        info.confirm(function (e) {
            this.meta.shutdown();
        });
    }
}
$(".goto-panel-group-tables").on('click', function () {
    clearOrder();
    newdefaultOrder();
    $.ajax({
        url: '/POS/ClearUserOrder',
        type: 'POST',
        data: { tableid: table_info.id }
    });

});