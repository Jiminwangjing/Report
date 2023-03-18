
(function( $ ){
    $.extend({
        buildCalculator: function(container){
            buildCalculator($(container));
        }
    });

    $.fn.buildCalculator = function(){
        buildCalculator(this.parent());
    }
    
    class Calculator {
        constructor(option = 0){
            this._left = 0;
            this._right = 0;
            this._result = 0;
        }
        get result(){
            return this._result;
        }

        set left(value){
            this._left = parseFloat(value);
        }

        get left(){
            return this._left;
        }

        set right(value){
            this._right = parseFloat(value);
        }

        get right(){
            return this._right;
        }

        multiply(){
            this._result = this._left * this._right;
            return this._result;
        }

        divide(){
            if(this._right !== 0){
                this._result = this._left / this._right;
            }  else {
                if(negative(this._left)){
                    this._result = Number.NEGATIVE_INFINITY;
                } else{
                    this._result = Number.POSITIVE_INFINITY;
                }
            }

            return this._result;
        }

        add(){
            this._result = this._left + this._right;
            return this._result;
        }

        substract(){
            this._result = this._left - this._right;
            return this._result;
        }

    }

    function buildCalculator(container){
        let $calc = $("<div class='calculator standard'>" 
            +"<div class='navigator reserve'></div>"
            +"<div class='navigator output'>0</div>"
            +"<div class='keypad'>"
                +"<div data-key='ce' class='key control'>CE</div>"
                +"<div data-key='c' class='key control'>C</div>"
                +"<div data-key='backspace' class='key control'><img src='./icons/backspace.png'></div>"
                +"<div data-key='÷' class='key operator'>÷</div>"

                +"<div data-key='7' class='key operant'>7</div>"
                +"<div data-key='8' class='key operant'>8</div>"
                +"<div data-key='9' class='key operant'>9</div>"
                +"<div data-key='×' class='key operator'>×</div>"

                +"<div data-key='4' class='key operant'>4</div>"
                +"<div data-key='5' class='key operant'>5</div>"
                +"<div data-key='6' class='key operant'>6</div>"
                +"<div data-key='-' class='key operator'>-</div>"

                +"<div data-key='1' class='key operant'>1</div>"
                +"<div data-key='2' class='key operant'>2</div>"
                +"<div data-key='3' class='key operant'>3</div>"
                +"<div data-key='+' class='key operator'>+</div>"

                +"<div data-key='±' class='key control'>±</div>"
                +"<div data-key='0' class='key operant'>0</div>"
                +"<div data-key='.' class='key operant'>.</div>"
                +"<div data-key='=' class='key r-operator'>=</div>"
            +"</div>"
        +"</div>").appendTo(container);
        $calc.append($("<div class='accept-result'>Accept</div>").on("click", resultClicked));
        $calc.siblings("input").addClass("final-result").on("click", resultClicked);
        
        calculate($calc);
    };

    function resultClicked(e){ 
        if($(this).is($(".final-result"))){//Clicked on input.final-result
            $(this).siblings(".calculator.standard").toggleClass("show");
        } else {//Clicked on div.accept-result          
            const output = $(this).siblings(".navigator.output").text();
            $(this).parent().siblings(".final-result").val(output);
            $(this).parent(".calculator.standard").removeClass("show");
        }  
    }

    function calculate($calc){
        let calc = new Calculator();
        const $operant = $calc.find('.keypad .key.operant');
        const $operator = $operant.siblings(".operator");
        const $r_operator = $operant.siblings(".r-operator");
        const $output = $operant.parent().siblings(".output");
        const $reserve = $output.siblings(".reserve");
        const $control = $operant.siblings(".control");
        let output = 0;
        let reserve = "";
        let sign = "";
        let ready = false;

        //Clicked on digit
        $operant.click(function(e){
            if(ready){
                output = $(this).data('key').toString();   
            } else {
                if($output.text().length < 13){
                    output += $(this).data('key').toString();
                }  
            }    

            output = validNumber(output);
            $output.text(output); 
            ready = false;  
        });

        //Clicked on binary operators (*, /, +, -)
        $operator.click(function(e){
            resultClicked(e);
            sign = $(this).data("key").toString(); 
            calc.left = parseFloat(output); 
            reserve =  calc.left.precise(9) + " " + sign;
            
            $(".reserve").text(reserve);
            $(".output").text(output);
            ready = true; 
        });

          $r_operator.click(resultClicked);
        
        function resultClicked(e){
            if(sign === ""){ return; }
            calc.right = parseFloat(output);
            reserve = calc.left.precise(6) + " " + sign + " " + calc.right.precise(6);
            if(negative(calc.right)){
                reserve = calc.left.precise(6) + " " + sign + " ( " + calc.right.precise(6) + " )"; 
            }
            output = "";
            switch(sign){
                case "×":         
                    calc.multiply();
                break;
                
                case "÷": 
                    calc.divide();
                break;

                case "+": 
                    calc.add();
                break;

                case "-": 
                    calc.substract();
                break;
            }
            output = calc.result.precise(9);
            
            $reserve.text(reserve);
            $output.text(output);
            ready = true;
        }


        // $r_operator.click(function(e){
        //     if(sign === ""){ return; }
        //     calc.right = parseFloat(output);
        //     reserve = calc.left.precise(6) + " " + sign + " " + calc.right.precise(6);
        //     if(negative(calc.right)){
        //         reserve = calc.left.precise(6) + " " + sign + " ( " + calc.right.precise(6) + " )"; 
        //     }
        //     output = "";
        //     switch(sign){
        //         case "×":         
        //             calc.multiply();
        //         break;
                
        //         case "÷": 
        //             calc.divide();
        //         break;

        //         case "+": 
        //             calc.add();
        //         break;

        //         case "-": 
        //             calc.substract();
        //         break;
        //     }
        //     output = calc.result.precise(9);
            
        //     $reserve.text(reserve);
        //     $output.text(output);
        //     ready = true;
        // });

        //Clicked on cancel control (CE, C, BACKSPACE)
        $control.click(function(e){
            output = output.toString();
            switch($(this).data("key")){
                case '±':
                    output = negate(output).toString();
                break;
                case 'backspace': 
                    if(output.includes("Infinity")){
                        output = 0;
                    } else {
                        output = output.substring(0, output.length - 1); 
                        if(output.length < 2 && output.includes("-")){
                            output = 0;
                        } 
                    }
                    
                break;
                case 'c': 
                    calc.left = 0;
                    calc.right = 0;
                    calc.result = 0;
                    output = 0;
                    sign = "";
                    reserve = "";
                break;

                case 'ce': 
                    output = 0;
                break;        
            }

            if(output.length === 0){
                output = 0;
            }

            $reserve.text(reserve);
            $output.text(output);
        });
    }

    Number.prototype.precise = function(max_precision){
        return (this.toString().length > max_precision)?
                this.toPrecision(max_precision) : this;
    }

    function negative(value){
        return parseFloat(value) < 0;
    }

    function negate(value){
        return parseFloat(value) * (-1);
    }

    /******************************************/
    /* Return string of valid decimal number. */
    /******************************************/
    function validNumber(value){
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

})(jQuery);