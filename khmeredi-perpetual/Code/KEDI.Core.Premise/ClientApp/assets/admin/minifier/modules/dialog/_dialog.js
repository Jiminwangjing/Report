class DialogBuilder {
    constructor(config){
        this._background = $("<div class='dialog-box-background'></div>");
        this._template = $("<div class='dialog-box'></div>");
        this._header = $("<div class='header'></div>");
        this._content = $("<div class='content'></div>");
        this._footer = $("<div class='footer'></div>");
        this._setting = {
            class: "",
            id: "#" + Date.now().toString(),
            container: {
                selector: "",
                attribute: "dialog-container"
            },
            insertion: "append",
            caption: "",
            content: "",
            type: "",
            icon: "",
            position: "",
            button: {
                meta: this
            },
            animation: {
                startup:{
                    complete: false,
                    timing: "after",
                    name: "slide-down",//["slide-up", "slide-down", "slide-left", "slide-right", "fade"]
                    delay: 0,
                    duration: 300,  
                    callback: function(e){}
                },
                shutdown: {
                    complete: false,
                    timing: "after",
                    name: "slide-up",
                    delay: 0,
                    duration: 300,  
                    callback: function(e){},
                    keycode: 27,//ESCAPE
                }  
            },
            on: {
                "close": function () { }
            }
        }
        this.setting = config;
    }

    set setting(config){
        if(!!config && config.constructor === Object && Object.keys(config).length > 0){
            this._setting = $.extend(true, {}, this._setting, config);
        } 
    }

    get setting(){ return this._setting; }
    get background() { return this._background; }
    get template(){ return this._template; }
    get header(){ return this._header; }
    get content(){ return this._content; }
    get footer(){ return this._footer; }

    addOrigin(process, name){
        this.setting.animation[process].name = name;
        switch(name){
            case "slide-up":  
                if(process === "startup"){
                    this.template.addClass("origin-bottom");
                } 
                if(process === "shutdown"){
                    this.template.addClass("origin-top");
                }  
                
            break;
            case "slide-down":
                if(process === "startup"){
                    this.template.addClass("origin-top");
                } 
                if(process === "shutdown"){
                    this.template.addClass("origin-bottom");
                } 
            break;
            case "slide-left":
                if(process === "startup"){
                    this.template.addClass("origin-right");
                } 
                if(process === "shutdown"){
                    this.template.addClass("origin-left");
                }          
            break;
            case "slide-right":
                if(process === "startup"){
                    this.template.addClass("origin-left");
                } 
                if(process === "shutdown"){
                    this.template.addClass("origin-right");
                }         
            break;
            case "fade":
                this.template.addClass("fade");
            break; 
        }
    }

    removeOrigin(process, name){
        this.setting.animation[process].name = name;
        switch(name){
            case "slide-up":  
                this.template.removeClass("origin-bottom");
            break;
            case "slide-down":
                this.template.removeClass("origin-top");  
            break;
            case "slide-left":
                this.template.removeClass("origin-right");      
            break;
            case "slide-right":
                this.template.removeClass("origin-left");        
            break;
            case "fade":
                this.template.removeClass("fade");
            break; 
        }
    }

    animate(process, delay, duration, name){
        this.setting.animation[process].delay = delay;
        this.setting.animation[process].duration = duration;
        this.setting.animation[process].name = name;
        let timespan = parseFloat(duration)/1000; 
        setTimeout(() => {
            switch(process){
                case "startup":  
                    this.template.addClass("startup").css("transition", timespan + "s");
                    this.background.addClass("blur").css("transition", timespan + "s");
                break;
                case "shutdown":
                    this.template.removeClass("startup").addClass("shutdown").css("transition", timespan + "s");
                        this.background.removeClass("blur").css("transition", timespan + "s");
                break;
            }
            
        }, parseFloat(this.setting.animation[process].delay));   
    }

    onEffect(process, timing, callback){
        this.setting.animation[process].timing = timing;
        this.setting.animation[process].callback = callback;
    }

    invoke(callback) {
        if (typeof callback === "function") {
            this.startup("before", callback);
        }      
        return this;
    }

    startup(timing, callback) {
        if(!!timing && typeof timing === "string"){
            this.setting.animation.startup.timing = timing;
            this.setting.animation.startup.callback = callback;
        }
      
        let $this = this, _startup = this.setting.animation.startup;
        switch(_startup.timing.toLowerCase()){
            case "before":                                    
                _startup.callback.call($this, $this, $this.template);
                $this.setPosition($this.setting.position);
                $this.addOrigin("startup", $this.setting.animation.startup.name);   
                $this.animate("startup", _startup.delay, _startup.duration, _startup.name);
            break;
            case "during":                
                _startup.callback.call($this, $this, $this.template);
                $this.setPosition($this.setting.position);
                $this.addOrigin("startup", $this.setting.animation.startup.name);
                $this.animate("startup", _startup.delay, _startup.duration, _startup.name);
            break;
            case "after":          
                $this.build($this.setting);
                _startup.callback.call($this, $this, $this.template);
                $this.setPosition($this.setting.position);
                $this.addOrigin("startup", $this.setting.animation.startup.name);
                $this.animate("startup", _startup.delay, _startup.duration, _startup.name);                     
            break;
        }

        this.onEffect("startup", _startup.timing, _startup.callback);
        this.setting.animation.startup.complete = true;
    }

    shutdown(timing, callback) {
        if(this.setting.animation.startup.complete){   
            if(!!timing && typeof timing === "string"){
                this.setting.animation.shutdown.timing = timing;
                this.setting.animation.shutdown.callback = callback;
            }

            let _shutdown = this.setting.animation.shutdown;
            switch(_shutdown.timing.toLowerCase()){
                case "before":
                    _shutdown.callback.call(this, this, this.template);
                    this.removeOrigin("startup", this.setting.animation.startup.name);
                    this.addOrigin("shutdown", this.setting.animation.shutdown.name);
                    this.animate("shutdown", _shutdown.delay, _shutdown.duration, _shutdown.name);
                break;
                case "during":
                    this.removeOrigin("startup", this.setting.animation.startup.name);
                    this.addOrigin("shutdown", this.setting.animation.shutdown.name);
                    this.animate("shutdown", _shutdown.delay, _shutdown.duration, _shutdown.name);
                    _shutdown.callback.call(this, this, this.template);
                break;
                case "after":
                    this.removeOrigin("startup", this.setting.animation.startup.name);
                    this.addOrigin("shutdown", this.setting.animation.shutdown.name);
                    this.animate("shutdown", _shutdown.delay, _shutdown.duration, _shutdown.name);
                    setTimeout(() => {
                        _shutdown.callback.call(this, this, this.template);
                    }, parseFloat(_shutdown.delay) + parseFloat(_shutdown.duration));
                break;   
            }
            setTimeout(() => {
                this.onEffect("shutdown", _shutdown.timing, _shutdown.callback);
                this.destroy();            
            }, parseFloat(this.setting.animation.shutdown.delay)
                + parseFloat(this.setting.animation.shutdown.duration)
            );                          
            this.setting.animation.shutdown.complete = true;       
        } 
    }

    setPosition(value){
        if(value !== undefined){
            this.setting.position = value;
            switch(this.setting.position){
                case "top-left": 
                    this.background.addClass("top-left");
                break;
                case "top-center": 
                    this.background.addClass("top-center");
                break;
                case "top-right": 
                    this.background.addClass("top-right");
                break;
                case "center-left": 
                    this.background.addClass("center-left");
                break;
                case "center": 
                    this.background.addClass("center");
                break;
                case "center-right": 
                    this.background.addClass("center-right");
                break;
                case "bottom-left": 
                    this.background.addClass("bottom-left");
                break;
                case "bottom-center": 
                    this.background.addClass("bottom-center");
                break;
                case "bottom-right": 
                    this.background.addClass("bottom-right");
                break;
            }
        }
    }

    //Override method build() in child class for creating dialog box interface.
    //build(config){}

    render() {
        let container = this.setting.container.selector;
        if(!!this.setting.container.attribute){
            container = $("["+ this.setting.container.attribute +"]");
        }

        if(!!container){         
            let $this = this, insertion = this.setting.insertion;   
            $(document).off("keyup").on("keyup", function (e) {
                let _keycode = $this.setting.animation.shutdown.keycode;
                if (typeof $this.setting.animation.shutdown.keycode === "string") {
                    _keycode = $this.setting.animation.shutdown.keycode.toLowerCase();
                }

                if (e.which === _keycode) {
                    $this.shutdown();
                }
            });

            switch(insertion.toLowerCase()){
                case "replace":
                    container.html(this.background);
                break;
                case "prepend":
                    container.prepend(this.background);
                break;
                case "append":
                    container.append(this.background);
                break;
            }     
            
            container.css({
                "position": "fixed",
                "top": "0",
                "z-index": "10"//"2147483647"
            });       
        }     
    }

    //Method for deleting the built dialog box.
    destroy(){
        if(this instanceof DialogBuilder){
            delete this;
            this.background.remove();
        }
    }
    
    //Utility Methods
    //Get highest z-index value of all specified elements.
    highestZIndex(tag_name){
        let tag = "*";
        let highest = 0;
        if(!!tag_name){
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

class DialogBox extends DialogBuilder
{
    constructor(config){
        super(config);
        this.setting = {
            type: "ok",
            icon: "info",
            closeButton: true,
            position: "top-center",
            button: {
                yes: {
                    meta: this,
                    template: "<button class='button bg-success'></button>",
                    text: "YES", 
                    callback: function (e) { },                    
                    keycode: undefined
                },
                no: {
                    meta: this,
                    template: "<button class='button bg-danger'></button>",
                    text: "NO",                   
                    callback: function (e) { },                  
                    keycode: undefined
                },
                ok: {
                    meta: this,
                    template: "<button class='button'></button>",
                    text: "OK", 
                    callback: function (e) { },                  
                    keycode: undefined
                },
                cancel: {
                    meta: this,
                    template: "<button class='button bg-danger'></button>",
                    text: "CANCEL", 
                    callback: function (e) { },                    
                    keycode: 27
                }
            }
        }//End setting
        const __this = this;
        this.setting = config;
        __this.startup();
       
    }//End constructor

    static Create(_config) {
        return new DialogBox(_config);
    }

    on(event, callback) {
        switch (event.toLowerCase()) {
            case "yes":
                this.setting.button.yes.callback = callback;
                break;
            case "ok":
                this.setting.button.ok.callback = callback;
                break;
            case "no":
                this.setting.button.no.callback = callback;
                return this;
            case "cancel":
                this.setting.button.cancel.callback = callback;
                break;
        }
        return this;
    }

    confirm(callback){
        this.setting.button.ok.callback = callback;
        this.setting.button.yes.callback = callback;
        return this;
    }

    reject(callback){
        this.setting.button.no.callback = callback;
        this.setting.button.cancel.callback = callback;
        return this;
    }
   

    get caption(){
        let $caption = $("<div class='caption'></div>");
        if(this.setting.caption !== undefined){
            $caption.append(this.setting.caption);
        }
        return $caption;
    }

    get closeButton(){
        let $this = this;
        let $wrap_icon = $("<div class='wrap-icon'></div>");
        let $closeButton = $();
        if(this.setting.closeButton){
            $closeButton = $("<i class='icon icon-close'></i>");
        } else {
            if(typeof this.setting.closeButton === "string"){
                $closeButton = $("<i class='"+ this.setting.closeButton +"'></i>");
            }
        }
        return $wrap_icon.append($closeButton.on("click", function (e) {
            let _shutdown = $this.setting.animation.shutdown;
            $this.shutdown(_shutdown.timing, $this.setting.on.close);
        }));
    }

    onClose(callback) {
        this.setting.on.close = callback;
        return this;
    }
    
    get icon(){ 
        let $wrap_icon = $("<div class='wrap-icon'></div>");
        let $icon = $("<div class='icon'></div>");
        if(this.setting.icon !== undefined){ 
            switch(this.setting.icon){
                case "info":
                    $icon.addClass("info");
                break;

                case "warning":
                    $icon.addClass("warning");
                break;

                case "danger":
                    $icon.addClass("danger");
                break;
                default: 
                    if(!!this.setting.icon && this.setting.icon.constructor === Object
                    && Object.keys(this.setting.icon).length > 0){
                        let icon = this.setting.icon;
                        let $i_icon = $("<i></i>");
                        if(icon.class !== undefined){
                            $i_icon.addClass(icon.class);
                        }
                        
                        if(icon.color !== undefined){
                            $i_icon.css("color", icon.color);
                        }
                        
                        if(icon.background_color !== undefined){
                            $icon.css("background", icon.background_color); 
                        }
                        $icon.append($i_icon);         
                    }

                    if(typeof this.setting.icon === "string"){
                        $icon.append("<i class='"+ this.setting.icon +"'></i>");
                        if(!!this.setting.icon && (/<[a-z/][\s\S]*>/i).test(this.setting.icon)){
                            $icon.append(this.setting.icon);
                        }
                    }

                    if(this.setting.icon === "none" || this.setting.icon == null){
                        $icon = $();
                    }    
            } 
        }
        return $wrap_icon.append($icon); 
    }

    get button() {
        let dialog = this;
        let $button_wrap = $("<div class='wrap-button'></div>");
        if(this.setting.button === "none"){
            $button_wrap.addClass("free-space");
        }

        if(!!this.setting.button && this.setting.button.constructor === Object
        && Object.keys(this.setting.button).length > 0){
            let type = this.setting.type.toLowerCase();
            let button = this.setting.button;
            let $ok = $(button.ok.template);
            let $yes = $(button.yes.template);
            let $no = $(button.no.template);
            let $cancel = $(button.cancel.template);
            switch(type){
                case "ok":
                    if (typeof button.ok.callback === "function") {
                        $ok.attr("id", "ok" + this.setting.id).html(button.ok.text);
                        if (button.ok.class) {
                            $ok.addClass(button.ok.class);
                        }
                        $ok.on("click", function(e){
                            button.ok.callback(dialog.fallback(button.ok, this), e);
                            if(typeof button.meta.setting.content === "string"){
                                button.meta.shutdown();
                            }
                        });            
                    }
                    $button_wrap.append($ok).addClass("right");
                break;

                case "yes-no": case "yes/no":  
                    if (typeof button.yes.callback === "function") {
                        $yes.attr("id", "yes" + this.setting.id).html(button.yes.text);
                        if (button.yes.class) {
                            $ok.addClass(button.yes.class);
                        }
                        $yes.on("click", function(e){
                            button.yes.callback(dialog.fallback(button.yes, this), e);
                        });
                    }

                    if (typeof button.no.callback === "function") {
                        $no.attr("id", "no" + this.setting.id).html(button.no.text);
                        if (button.no.class) {
                            $ok.addClass(button.no.class);
                        }
                        $no.on("click", function(e){
                            button.no.callback(dialog.fallback(button.no, this), e);
                            if(typeof button.meta.setting.content === "string"){
                                button.meta.shutdown();
                            }
                        });
                    }
                    $button_wrap.append($yes).append($no).addClass("right");
                break;

                case "ok-cancel": case "ok/cancel":
                    if (typeof button.ok.callback === "function") {
                        $ok.attr("id", "ok" + this.setting.id).html(button.ok.text);
                        if (button.ok.class) {
                            $ok.addClass(button.ok.class);
                        }
                        $ok.on("click", function(e){
                            button.ok.callback(dialog.fallback(button.ok, this), e);
                        });
                    }

                    if (typeof button.cancel.callback === "function") {
                        $cancel.attr("id", "cancel" + this.setting.id).html(button.cancel.text);
                        if (button.cancel.class) {
                            $ok.addClass(button.cancel.class);
                        }
                        $cancel.on("click", function(e){
                            button.cancel.callback(dialog.fallback(button.cancel, this), e);
                            if(typeof button.meta.setting.content === "string"){
                                button.meta.shutdown();
                            }
                        });
                    }
                    $button_wrap.append($ok).append($cancel).addClass("right");
                break;

                case "yes-no-cancel": case "yes/no/cancel":
                    if (typeof button.yes.callback === "function") {
                        $yes.attr("id", "yes" + this.setting.id).html(button.yes.text);
                        if (button.yes.class) {
                            $ok.addClass(button.yes.class);
                        }
                        $yes.on("click", function(e){
                            button.yes.callback(dialog.fallback(button.yes, this), e);
                        });
                    }

                    if (typeof button.no.callback === "function") {
                        $no.attr("id", "no" + this.setting.id).removeClass("bg-danger").html(button.no.text);
                        if (button.no.class) {
                            $ok.addClass(button.no.class);
                        }
                        $no.on("click", function(e){
                            button.no.callback(dialog.fallback(button.no, this), e);
                        });
                    }
                    
                    if (typeof button.cancel.callback === "function") {
                        $cancel.attr("id", "cancel" + this.setting.id).html(button.cancel.text);
                        if (button.cancel.class) {
                            $ok.addClass(button.cancel.class);
                        }
                        $cancel.on("click", function(e){
                            button.cancel.callback(dialog.fallback(button.cancel, this), e);
                            if(typeof button.meta.setting.content === "string"){
                                button.meta.shutdown();
                            }
                        });
                    }
                    $button_wrap.append($yes).append($no).append($cancel).addClass("right");
                break;
                default:
                    if(this.setting.type === "none"){
                        return $button_wrap.addClass("free-space");
                    }     
            }
            
        }
        
        return $button_wrap;
    }

    fallback(proto, target) {
        return {
            meta: this,
            proto: proto,
            target: target
        }
    }

    preventConfirm(okay = true) {
        this.disable("yes", okay);
        this.disable("ok", okay); 
    }

    disable(target, okay = true) {
        let _target = target;
        if (typeof _target === "string") {
            _target = $(this.template).find("button[id='" + target + this.setting.id + "']")[0];
        }

        $(_target).prop("disabled", okay);
    }

    build(config) {   
        this.setting = config;
        this.header.children().remove();
        this.header.append(this.icon).append(this.caption).append(this.closeButton);
        if(this.setting.content !== undefined){
            this.content.html("<span>"+ this.setting.content +"</span>");
            if(!!this.setting.content && this.setting.content.constructor === Object
            && Object.keys(this.setting.content).length > 0){
                if(this.setting.content.selector.includes("#")
                || this.setting.content.selector.includes(".")){
                    let $cloned = $(this.setting.content.selector).clone(true);
                    this.content.html($cloned).addClass("outer-content widget-scrollbar");
                    if(this.setting.content.class !== undefined){
                        this.content.removeClass("outer-content")
                            .addClass(this.setting.content.class);                      
                    }
                }
            }
        }

        this.footer.html(this.button);
        this.template
            .append(this.header)
            .append(this.content)
            .append(this.footer);           
        this.background.append(this.template);
        this.render();
    }
}

class DialogCalculator extends DialogBuilder
{
    constructor(config){
        super(config);
        this._left = 0;
        this._right = 0;
        this._result = 0;
        this.setting = {
            position: "top-center",
            output: this.result,
            button: {
                accept: {
                    meta: this,
                    text: "Accept",
                    callback: function(e){}
                }
            },
            animation: {
                shutdown: {
                    name: "slide-up"
                }
            }
        }

        Number.prototype.precise = function(max_precision){
            return (this.toString().length > max_precision)?
                    this.toPrecision(max_precision) : this;
        }

        Math.negative = function(value){
            return parseFloat(value) < 0;
        }

        Math.negate = function(value){
            return parseFloat(value) * (-1);
        }

        this.setting = config;
        this.startup();
    }

    set result(value){
        this._result = value;
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
            if(Math.negative(this._left)){
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

    accept(callback){
        this.setting.button.accept.callback = callback;
    }

    build(setting){
        let $this = this;
        this.setting = setting;
        this.template.addClass("calculator standard");
        let $content = $("<div class='navigator reserve'></div>"
            +"<div class='navigator output'>0</div>"
            +"<div class='keypad'>"
                +"<div data-key='ce' class='key control'>CE</div>"
                +"<div data-key='c' class='key control'>C</div>"
                +"<div data-key='backspace' class='key control'><i class='backspace'>×</i></div>"
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
            +"</div>");
        this.template.html($content)
            .append($("<div class='navigator accept-result'>Accept</div>")
                .on("click", function(e){
                    $this.setting.button.accept.callback(this, e);
                }));
        this.calculate(this.template);
        this.background.html(this.template).on("click", function(e){
            if($(e.target).is(this)){
                $this.shutdown();
            }
        });
        this.render();
    };

    calculate($calc){
        const self = this;
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
            
            output = self.validNumber(output);
            $output.text(output);           
            ready = false;               
        });

        //Clicked on binary operators (*, /, +, -)
        $operator.click(function(e){
            resultClicked(e);
            sign = $(this).data("key").toString(); 
            self.left = parseFloat(output); 
            reserve = self.left.precise(9) + " " + sign;
            
            $(".reserve").text(reserve);
            $(".output").text(output);
            ready = true; 
        });

        $r_operator.click(resultClicked);
        
        function resultClicked(e) {
            if(sign === ""){ return; }
            self.right = parseFloat(output);
            reserve = self.left.precise(6) + " " + sign + " " + self.right.precise(6);
            if(Math.negative(self.right)){
                reserve = self.left.precise(6) + " " + sign + " ( " + self.right.precise(6) + " )"; 
            }
            output = "";
            switch(sign){
                case "×":         
                    self.multiply();
                break;
                
                case "÷": 
                    self.divide();
                break;

                case "+": 
                    self.add();
                break;

                case "-": 
                    self.substract();
                break;
            }
            output = self.result.precise(9);
            
            $reserve.text(reserve);
            $output.text(output);
            ready = true;
        }
        //Clicked on cancel control (CE, C, BACKSPACE)
        $control.click(function(e){
            output = output.toString();
            switch($(this).data("key")){
                case '±':
                    output = Math.negate(output).toString();
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
                    self.left = 0;
                    self.right = 0;
                    self.result = 0;
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

    validNumber (value) {
        if (!!value && typeof value === "number") {
            value = value.toString();             
        }

        if (value == "-00") {
            value = "-0";
        }

        if (value == "" || value == "-" || value == "00") {
            value = "0";
        }

        if (value == ".") {
            value = "0.";
        } 
        
        if (!(/^[-]?\d+[.]?\d*$/).test(value)) {
            if (value == "-") {
                value = "-" + value;
            }

            value = value.toString().substring(0, value.length - 1);
        } else {
            if (!value.includes(".") && value != "-0") {
                value = parseFloat(value);
            }
        }
        return value;
    }
    
}
