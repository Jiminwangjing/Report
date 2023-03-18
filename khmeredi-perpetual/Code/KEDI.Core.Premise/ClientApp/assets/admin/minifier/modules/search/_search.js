$(function(){
    let wrap_input =  $(".group-search-boxes .wrap-search-box .wrap-input");
    wrap_input.siblings(".btn-search").click(function(e){
        $(this).siblings(".wrap-input").addClass("show");
        $(this).parent().parent().find(".btn-search").hide();
        setTimeout(() => {
            wrap_input.find("input").focus().val("");
        }, 250);
        if($(this).parent().parent().parent().hasClass("nav-toolbar")){
            $(this).parent().parent().siblings().addClass("hide");
        }       
    });

    wrap_input.find("#i-close").click(function(e){
        e.preventDefault();
        $(this).parent().removeClass("show");
        $(this).parent().parent().parent().find(".btn-search").show();    
        $(this).parent().parent().parent().siblings().removeClass("hide");      
    });
});