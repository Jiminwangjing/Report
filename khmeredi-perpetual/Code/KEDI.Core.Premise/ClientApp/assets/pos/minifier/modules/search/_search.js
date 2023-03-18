$(function(){
    let $searchBox = $(".group-search-boxes .search-box");
    $searchBox.find(".btn-search").click(function(e){
        $(this).parent(".search-box").addClass("show");
        $(this).parents(".group-search-boxes").find(".btn-search").hide();       
        setTimeout(() => {
            $(this).siblings(".input-box").find("input").focus().val("");
        }, 250);     
    });

    $searchBox.find("#i-close").click(function(e){
        e.preventDefault();
        $(this).parents(".search-box").removeClass("show");
        $(this).parents(".group-search-boxes").find(".btn-search").show();    
    });
});