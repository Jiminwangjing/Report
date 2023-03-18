//Click All Step
const $all_step = $(".group-steps .all-step");
$all_step.on('click', function (e) {
    //arr_steps = [];
    const $wrap_grid = $("#group-item-gridview .wrap-grid");
    $(this).parent().find(":not(:first-child)").remove();
    let group_items = db.from("tb_group1");
    groupItemFiltered($wrap_grid, group_items, 1, 0, 0, 0);
    $(this).siblings().removeClass("active");
    $(this).addClass("active");

});
//Click on group step
function clickOnstep(e) {
    const $wrap_grid = $("#group-item-gridview .wrap-grid");
    let level = parseInt($(this).data("group"));
    let group_id = $(this).data("id");
    let group1_id = parseInt($(this).data("group1"));
    let group2_id = parseInt($(this).data("group2"));
    let group3_id = parseInt($(this).data("group3"));
    //Remove last step
    let length = $(this).parent().children().length
   
    let nth_child = $(this).index();

    for (let i = length; i > 0; i--) {
        if (i > nth_child + 1) {

            $(".group-steps").find(":nth-child(" + i + ")").remove();
        }
    }
    //Active step
    $(this).siblings().removeClass("active");
    $(this).addClass("active");
    //Find group from controller
    let group_items;
    if (level !== 3) {
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
 
    //search item multi
    if (level === 4) {

        let multi_items = db.from("tb_item_master").where(w => {
            return w.ItemID === group_id;
        });
        singleItem($("#group-item-gridview .wrap-grid"), multi_items);
    }
    //Check group or item
    else if (Array.isArray(group_items) && group_items.length >=0) {

        groupItemFiltered($wrap_grid, group_items, level, group1_id, group2_id, group3_id);
    }
    else {
        $wrap_grid.children().remove();
    }
}
//Default group 1
function defaultGroup1() {
    const $wrap_grid = $("#group-item-gridview .wrap-grid");
    $(".all-step").parent().find(":not(:first-child)").remove();
    let group_items = db.from("tb_group1");
    groupItemFiltered($wrap_grid, group_items, 1, 0, 0, 0);

}