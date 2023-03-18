/****************/
/* @version 1.0 */
/****************/
;(function ($) {
    let event_types = ["prebuild", "postbuild", "click", "dblclick", "mouseenter", "mouseleave", "mouseout",
        "mousemove", "mouseover", "mousedown", "mouseup", "wheel"];
    let h = {};
    $.extend({    
        bindRows: function(table_selector, jsons, key, option = 0){
            /**************************************************************************/
            /*  Create a header of table if array of column names specified than use, */
            /*  otherwise use default properties of json as column names.             */
            /**************************************************************************/
            $(table_selector).find("tr:not(:first-child)").remove();
            
            let header = $("<tr></tr>");
            if(validArray(option.columns)){
                for(let m of option.columns){
                    header.append(
                        "<th>" + m + "</th>"
                    );
                }
            } else {
                h = Object.assign({}, jsons[0]);
                for(let m of Object.getOwnPropertyNames(h)){
                    header.append(
                        "<th>" + m + "</th>"
                    );   
                }     
            }

            if ($(table_selector).find("tr:first-child").length === 0) {
                if (validArray(option.columns)) {
                    $(table_selector).append(header);
                }
                
            }
            
            /*************************************************************************/
            /* Create rows of table depending on number of json object in the array. */
            /*************************************************************************/
            if (validArray(jsons)) {

                let _counter = -1;
                if (!!option.start_index && typeof parseInt(option.start_index) === "number") {
                    _counter = parseInt(option.start_index);
                }
                for (let json of jsons) {               
                    let row = builtRow(json, key, option, table_selector);
                    if (_counter > 0) {                      
                        row.prepend("<td>" + _counter + "</td>");
                        _counter++;
                    }
                    row.on("mouseover", resetHighlight);
                    $(table_selector).append(row);  
                    fitTable(table_selector); 
                    if(option.highlight_row !== undefined){
                        if(row.data(key.toLowerCase()) === option.highlight_row){
                            row.siblings().removeClass("highlight");
                            row.addClass("highlight");
                            row[0].scrollIntoView({
                                behavior: !option.scrollview.behavior ? 'auto' : option.scrollview.behavior,
                                block: !option.scrollview.block ? 'center' : option.scrollview.block 
                            });
                        }  
                    }                 
                }  
                
                //if (!!option.scrollview) {
                //    document.querySelector(table_selector)
                //        .parentNode.scrollTop = document.querySelector(table_selector).scrollHeight;
                //}
                         
            }       
        },//End bindRows()  

        updateRow: function (table_selector, json, key, option = 0) {           
            let $row = $();
            $row = builtRow(json, key, option);
            $(table_selector).append($row);
            fitTable(table_selector);  
            let _key = option.highlight_row = json[key];
            $.each($(table_selector).find("tr:not(:first-child)"), function(i, tr){ 
                if($(tr).data(key.toLowerCase()) == _key){
                    tr.scrollIntoView({
                        behavior: 'auto',
                        block: 'center'
                    });
                   
                    $(tr).replaceWith($row);
                    if (!!option.start_index && typeof parseInt(option.start_index) === "number") {
                        let _counter = $(tr).prevUntil("tbody").length + parseInt(option.start_index) - 1;
                        $(tr).prepend("<td cell-index>" + _counter + "</td>");
                    }
                }      
            });         
        }//End updateRow()
    
    });

    $.fn.bindRows = function (jsons, key, option = 0) {
        $.bindRows(this, jsons, key, option);
    }

    $.fn.updateRow = function (jsons, key, option = 0) {
        $.updateRow(this, jsons, key, option);
    }

    function highlightRow() {

    }

    function sortBy(array, key) {
        return array.sort(function (a, b) {
            return a[key] - b[key];
        });
    }

    function groupBy(xs, key) {
        return xs.reduce(function (rv, x) {
            (rv[x[key]] = rv[x[key]] || []).push(x);
            return rv;
        }, {});
    };

    function resetHighlight(e){
        $(this).parent().children().removeClass("highlight");
    };

    function builtRow(json, key, option) {
        let row = $("<tr></tr>");            
        bindEvent(row, option);
        if (!!option["prebuild"]) {
            onBuildRow(row, json, option, option["prebuild"]);
        }
        $.each(json, function (c, value) {           
            var col = $("<td></td>");
            let col_name = c;
            let row_data = $("<tr></tr>");
            let hide_key = option.hide_key !== undefined && option.hide_key;
            let scale_col = "";
            if (validJSON(option.scalable)) {
                if (typeof option.scalable.column == 'string') {
                    scale_col = option.scalable.column;
                }
            }

            if (key !== undefined && key == c) {
                row_data = $("<tr data-" + key + "=" + value + "></tr>");
                row = row_data;
            }

            if (validJSON(option.scalable) && col_name == scale_col) {
                col = scalableColumn(value, option.scalable);
            } else {
                col = $("<td>" + value + "</td>");
            }           

            if(validArray(option.text_align)){
               for(let ta of option.text_align){
                   $.each(ta, function(cn, p){
                       if(col_name === cn){
                           col = $("<td>"+ value +"</td>");
                           col.css("text-align", p);
                       }
                   });
               }
            }

            //As default show primary key
            if(col_name === key){
                col = $("<td>" + value + "</td>");
                if(hide_key){
                    col = $("<td hidden>" + value + "</td>"); 
                }
            } 

            //Hide specified columns
            if(validArray(option.hidden_columns)){              
                for(let hc of option.hidden_columns){             
                    if(hc === col_name){
                        col = $("<td cell-" + col_name +" hidden>" + value + "</td>");  
                    }
                }
            }

            if(validArray(option.html)){
                for(let value of option.html){
                    if(value.column !== undefined && value.column === col_name){
                        injectHtml(col, value, col_name);
                    }
                }
            }    
            col.attr("cell-" + col_name, "");
            row.append(col);
        });

        //Inject html into column outside of json properties range.
        if (option.html !== undefined) {
            for(let hc of option.html){
                if(hc.column === -1){   
                    let col = $("<td></td>");       
                    injectHtml(col, hc);      
                    row.append(col);
                }
            }           
        }
                    
        /* Note */
        /******************************************************************************************/
        /* Event listener callback could be 'function' or 'arrow function'.                       */
        /* Function is either 'constructible or callable' but arrow function is 'only callable'.  */
        /* 'Constructible' function can possess 'this' so lexical 'this' in the function refers   */
        /*      to current event binding element (here each row of the table).                    */
        /* 'Only callable' function cannot possess 'this' so 'this'                               */
        /*      in the arrow function always refers to 'DOM'.                                        */ 
        /******************************************************************************************/
        bindEvent(row, option);
        if (!!option["postbuild"]) {
            onBuildRow(row, json, option, option["postbuild"]);
        }
        return row;
    };

    function bindEvent(row, option) {
        if (validJSON(option)) {
            $.each(option, function (key, value) {
                for (let et of event_types) {
                    if (typeof value == 'function') {
                        if (key == et) {
                            row.on(key, value);
                        }
                    }
                }
            });
        } 
    };

    function onBuildRow(row, data, setting, callback) { 
        return callback.call(row, data, setting);
    };

    function scalableColumn(value, scalable){
        let $col = $("<td></td>");
        $wrap_scale = $("<div class='wrap-scale'>"
            + "<i class='scale-down'>-</i>"
            + "<label data-scale='" + value + "' class='scale'>" + value + "</label>" 
            + "<i class='scale-up'>+</i>"
        +"</div>");
        if(scalable.callback !== undefined){ 
            $wrap_scale.on(scalable.event, scalable.callback);
        }    
        $wrap_scale.appendTo($col);
        return $col;  
    };

    function injectHtml($col, value) {
        if(value.element !== undefined){                    
            let $_json = (value.listener === undefined)?
                        $(value.element) : $(value.element).on(value.listener.event, value.listener.callback);
            let $_arr = (value.listener === undefined)? 
                        $(value.element) : $(value.element).on(value.listener[0], value.listener[1]);    
            switch(value.insertion){
                case "prepend":
                    $col.prepend(Array.isArray(value.listener)? $_arr : $_json);                                                      
                break;

                case "append":
                    $col.append(Array.isArray(value.listener)? $_arr : $_json); 
                break;

                case "replace":
                    $col.html(Array.isArray(value.listener)? $_arr : $_json); 
                break;

                default: 
                    $col.append(Array.isArray(value.listener)? $_arr : $_json);  
            }

        }  
    };

    function fitTable(table_selector){
        if($(table_selector).height() > $(table_selector).parent().height()){
            $(table_selector).parent().addClass("fit-scroll-y");
        } else {
            $(table_selector).parent().removeClass("fit-scroll-y");
        }
    };

    //Check if json object is valid.
    function validJSON(json){
        return json !== undefined && json.constructor === Object && Object.keys(json).length > 0;
    };
    //Check if value is valid array.
    function validArray(array){
        return array !== undefined && Array.isArray(array) && array.length > 0;
    };
}(jQuery));
