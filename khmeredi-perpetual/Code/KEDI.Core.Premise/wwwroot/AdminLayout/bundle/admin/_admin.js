"use strict";

;(function (window, $, container) {
    const $sidemenu = $(".widget-body .widget-sidemenu", container);
    const $stackcrumb = $(".widget-stackcrumb", container); 
    if (window.innerWidth < 415) {
        $sidemenu.addClass("sidemenu-collapse");
    }
    $(".widget-sidemenu-trigger").click(function () {
        $sidemenu.toggleClass("sidemenu-collapse");
        if ($sidemenu.hasClass("sidemenu-collapse")) {
            $(".sidemenu-group:not(.group-root)", $sidemenu).hide(200)
            .siblings(".item-title").removeClass("title-icon-open");
        } else {
            $(".highlight", $sidemenu).parents(".sidemenu-group:not(.group-root)").show(200, function () {
                this.scrollIntoView({ behavior: "smooth", block: "nearest" });
            }).siblings(".item-title").addClass("title-icon-open");            
        }      
    });

    $(".sidemenu-item .item-title", $sidemenu).on("click", function (e) {
        if ($(this).siblings(".sidemenu-group").length > 0) {                   
            $(this).toggleClass("title-icon-open").siblings(".sidemenu-group").toggle(200, function () {
                this.scrollIntoView({ behavior: "smooth", block: "nearest" });
            })
            .closest(".sidemenu-item").siblings().children(".sidemenu-group").hide(200)
            .siblings(".item-title").removeClass("title-icon-open");               
        } 

        if ($sidemenu.hasClass("sidemenu-collapse")) {
            $sidemenu.removeClass("sidemenu-collapse");          
            $(".highlight", $sidemenu).parents(".sidemenu-group:not(.group-root)").show(200, function () {
                this.scrollIntoView({ behavior: "smooth", block: "nearest" });
            })
            .siblings(".item-title").addClass("title-icon-open");             
        }    
            
    });

    $(".sidemenu-item .item-label", $sidemenu).on("click", function () {
        $(".highlight", $sidemenu).removeClass("highlight");
        $(this).addClass("highlight");
        //Show module tree names on toolbar.
        setHighlight(this);
    });

    setHighlight($(".highlight", $sidemenu));
    setInterval(function () {
        const now = new Date().toLocaleString().split(",");
        $(".sidebar-datetime", $sidemenu).text(now[0] + " " + now[1]);
    }, 1000);
    $(".widget-loadscreen", container).hide();

    function setHighlight(target) {    
        let groups = $(target, $sidemenu).parents(".sidemenu-group:not(.group-root)"), n = groups.length;
        $stackcrumb.text("");
        if ($(target, $sidemenu)) {
            while (n > 0) {
                if (!$sidemenu.hasClass("sidemenu-collapse")) {
                    $(groups[n - 1]).show().siblings(".item-title").addClass("title-icon-open");
                } 
                
                $stackcrumb.append("<li>" + $(groups[n - 1]).siblings(".item-title").text() + "</li>");
                n--;
            }
            $stackcrumb.append("<li class='active'>" + $(target).text() + "</li>");
        }
       
        if ($(target, $sidemenu).parents(".sidemenu-group").length > 0) {
            $(target, $sidemenu).parents(".sidemenu-group")[0].scrollIntoView({ behavior: "auto", block: "nearest" });
        }                  
    }
}(window, jQuery, ".ck-widget-container"));
