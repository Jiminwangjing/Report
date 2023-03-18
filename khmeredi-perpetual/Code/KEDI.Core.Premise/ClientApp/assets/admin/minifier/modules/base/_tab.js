; (function ($, container) {
    $(".tab-sheet > .tab-title", container).on("click", function () {
        $(this).parent(".tab-sheet").addClass("active").siblings().removeClass("active");
    });
}(jQuery, ".widget-tab"));