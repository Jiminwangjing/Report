
//Bind default listener for predefined html structure of combobox.
$(function ($, container) {
    $("html, .widget-combobox .combobox-trigger, .widget-combobox .i-close").on("mousedown", function (e) {
        var $content = undefined;
        if ($(this).is($(".combobox-trigger"))) {
            $content = $(this).toggleClass("show").siblings(".combobox-content");
            if ($content.hasClass("show")) {
                closeContent($content);
            } else {
                showContent($content);
            }      
        }
       
        if ($(e.target).is($(".i-close"))) {
            $content = $(this).parent(".combobox-header").parent(".combobox-content");
            closeContent($content);
        }

        $content = $(e.target).find(".combobox-content");
        closeContent($content);
    });

    function showContent(content) {
        if ($(content).length > 0) {
            $(content).addClass("show").siblings(".combobox-trigger").addClass("show");
        }
    }

    function closeContent(content) {
        if ($(content).length > 0) {
            $(content).removeClass("show").siblings(".combobox-trigger").removeClass("show");
            $(content).find(".combobox-content").removeClass("show").siblings(".combobox-trigger").removeClass("show");
        }
    }

}(jQuery, ".ck-widget-container"));


