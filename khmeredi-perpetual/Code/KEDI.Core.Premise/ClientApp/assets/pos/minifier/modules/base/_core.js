class _$_ 
{
    //Check if value is json
    static isJSON(value){
        return value !== undefined && value.constructor === Object;
    }

    //Check if json object is valid.
    static isValidJSON(json){
        return json !== undefined && json.constructor === Object && Object.keys(json).length > 0;
    }

    //Check if value is array.
    static isArray(value){
        return value !== undefined && Array.isArray(value);
    }

    //Check if valid array.
    static isValidArray(value){
        return value !== undefined && Array.isArray(value) && value.length > 0;
    }

    //Check if string is html.
    static isHTML(value){
        return value !== undefined && (/<[a-z/][\s\S]*>/i).test(value);
    }

    //Check if value is valid decimal number.
    static validNumber(value){
        if(value == "00"){
            value = "0";
        }

        if(value === "."){
            value = "0.";
        } 

        if(!value.includes(".")){
            value = parseFloat(value);
        }

        if (!(/^[-]?\d+[.]?\d*$/).test(value)) {
            value = value.toString().substring(0, value.length - 1);
        } 

        return value;
    }

    //Check if two value has the same type.
    static isUniform(a, b, equal){
        let identical = false;
        if(_$_.isValidJSON(a) && _$_.isValidJSON(b)){ 
            $.each(a, function(ak, av){
                $.each(b, function(bk, bv){ 
                    identical = ak === bk;                
                });
            });      
        }
        if(a !== undefined && !_$_.isValidJSON(a)){
            if(b !== undefined && !_$_.isValidJSON(b)){
                return a === b;
            }
        }
        
        return identical; 
    }

    //Check if any event bound to target element
    static targetBound(target, event_type){
        let bound = false
        if(target !== undefined){
            $.each($._data(target, "events"), function(k, v){
                if(k === event_type){
                    bound = true;
                }
            });
        }
        return bound;
    }

    //Get highest z-index value of all specified elements.
    static highestZindex(highest, tag_name){
        let tag = "*";
        if(tag_name !== undefined){
            tag = tag_name;
        }

        $(tag).each(function(){
            let zindex = $(this).css("z-index");
            if((zindex > highest) && (zindex != 'auto')){
                highest = zindex;
            }  
        });
        return highest;
    }

}