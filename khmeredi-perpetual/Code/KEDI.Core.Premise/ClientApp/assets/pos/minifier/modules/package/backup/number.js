(function( $ ){
    $.extend({
        /******************************************/
        /* Return string of valid decimal number. */
        /******************************************/
        validNumber: function(value){
            if(value == "00"){
                value = "0";
            }

            if(value === "."){
                value = "0.";
            } 

            if(!value.includes(".")){
                value = parseFloat(value).toString();
            }

            if (!(/^[-]?\d+[.]?\d*$/).test(value)) {
                value = value.substring(0, value.length - 1);
            } 

            return value;
        }//End validNumber()

    });//End $.extend
})(jQuery);