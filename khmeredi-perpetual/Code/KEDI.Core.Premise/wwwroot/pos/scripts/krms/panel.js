
    const $goto_group_tables = $(".goto-panel-group-tables");
    $goto_group_tables.click(function(e){
        $("#panel-group-tables").addClass("show");
        $("#panel-payment").removeClass("show");
        $("#panel-group-items").removeClass("show");
        $(".nav-header .min-max").removeClass("show");
    });