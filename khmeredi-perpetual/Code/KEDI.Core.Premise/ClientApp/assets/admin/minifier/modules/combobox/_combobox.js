
class combobox 
{
    static get target() {
        return "html, .combobox-trigger, .i-close";
    }

    constructor(container, config = 0){
        this._target = "html, #demo-order-list .combobox-trigger, #demo-order-list .i-close";
        this._self = $();
        this._setting = {
            container: container,
            direction: "down",
            insertion: "replace",
            onTarget: "mousedown",
            type: "list",
            withCrumb: true,
            trigger: {
                icon: "fas fa-list-alt"
            }, 
            content: {
                add_option: {
                    class: "fas fa-plus",
                    listener: []
                },
                scroll: "vertical",
                data: [],
                option: {
                    has_title: false,
                    icon: "fas fa-receipt",
                    listener: []
                }
            }
        }
        
        this.setting = config;
        // this._target = "html," + this.setting.container + " combobox-trigger," + this.setting.container + " i-close";
        this.settingUpdated();
        //Filter for objects by specified condition.
        Array.prototype.where = function(condition){
            return $.grep(this, function(json, i){
                return condition(json, i);
            });
        };

        //Find single object from Array by specified condition.
        Array.prototype.first = function(condition){
            return $.grep(this, function(json, i){
                return condition(json, i);
            })[0];
        };

        //Remove object from array by specified key.
        Array.prototype.remove = function(key){
            if(_$_.validArray(key)){
                for(let k of key){
                    this.remove(k);
                }
            } else {
                let item = {};
                item = this.first(function(item){
                    return item.key === key;
                });
                if(this.indexOf(item) !== -1){
                    this.splice(this.indexOf(item), 1);
                }
            }
        }
    }//End constructor

    get target(){ return this._target; }

    get self(){ return this._self; }

    get setting(){ return this._setting; }
    set setting(config){
        if(_$_.isValidJSON(config)){
            this._setting = $.extend(true, {}, this._setting, config);
        } 
    }

    async settingUpdated(){
        await Promise.resolve(
           setTimeout(() => {
                this.refresh(); 
            }, 0)
        );  
         
    }

    refresh() {
        this.init(this.setting);
    }

    moveScrollY(selector, position){
        switch(position){
            case "top":
                $(selector).scrollTop(0);
            break;

            case "bottom":
                $(selector).scrollTop($(selector)[0].scrollHeight);
            break;
            default: 
                $(selector).scrollTop($(selector)[0].scrollHeight);
        }  
    }

    onTarget(event_type){
        let target = this.target;
        let $self = this.self;
        $(target).on(event_type, function(e){
            e.stopPropagation();
            if($(this).is($self.find(".combobox-trigger"))){
                $(this).toggleClass("show");
                $(this).siblings(".combobox-content").toggleClass("show"); 
            }

            if($(this).is($self.find(".i-close"))){
                $self.find(".combobox-trigger").removeClass("show");
                $self.find(".combobox-content").removeClass("show");
            }

            if($(this).is($self.parents()) && !$(e.target).is($self.find("*"))){
                $self.find(".combobox-trigger").removeClass("show");
                $self.find(".combobox-content").removeClass("show");
            }  
        });
    }

    static async onTarget(event_type){
        let selector = combobox.target;
        $(selector).on(event_type, function(e){
            e.stopPropagation();
            if($(this).is($(".combobox-trigger"))){
                $(this).toggleClass("show");
                $(this).siblings(".combobox-content").toggleClass("show"); 
            }

            if($(this).is($(".i-close"))){
                $(this).parents(".combobox-content").removeClass("show").siblings(".combobox-trigger").removeClass("show");
            }

            if($(this).is($("html")) && !$(e.target).is($(".combobox *"))){
                $(".combobox .combobox-content").removeClass("show").siblings(".combobox-trigger").removeClass("show");
            }  
        });
    }
    
    async buildTrigger($combobox){
        let $combobox_btn = $("<div class='combobox-trigger'></div>");
        let $badge = $("<div class='badge'></div>");
        let $icon = $("<i class='icon'></i>");
        let $img_circle = $("<img class='img-circle' src='/images/user.png'/>");

        if($combobox.hasClass("list")){
            if(_$_.isValidJSON(this.setting)){
                if(_$_.isHTML(this.setting.trigger.icon)){
                    $icon.replaceWith(this.setting.trigger.icon);
                } else {
                    if(typeof this.setting.trigger.icon === "string"){
                        $icon.addClass(this.setting.trigger.icon);
                    }  
                }

                if(this.setting.trigger.badge !== undefined){
                    $badge.append(this.setting.trigger.badge);
                }
            }

            $combobox_btn.append($icon).append($badge);
        }

        if($combobox.hasClass("profile")){ 
            if(_$_.isValidJSON(this.setting)){
                if(_$_.isHTML(this.setting.trigger.icon)){
                    $img_circle.replaceWith(this.setting.trigger.icon);
                } else {
                    $img_circle.prop("src", this.setting.trigger.icon);
                }
            }
            
            $combobox_btn.append($img_circle);
        }
        $combobox.append($combobox_btn);
    }

    async buildContent($combobox){
        let $wrap_combobox_content = $("<div class='combobox-content'></div>");
        $("<div class='combobox-navigator'>"
            +"<div class='i-close far fa-window-close'></div>" 
        + "</div>").appendTo($wrap_combobox_content);
        let $combobox_content = $("<div class='combobox-body'></div>");
        let add_opt = this.setting.content.add_option;
        if($combobox.hasClass("list")){
            if(_$_.isValidJSON(add_opt)){
                let $add_option = $("<i class='add-option'></i>");
                if(add_opt.class !== undefined){
                    $add_option.addClass(add_opt.class);
                }

                //Add listener to option of list as array.
                if(_$_.validArray(add_opt.listener)){
                    $add_option.on(add_opt.listener[0], add_opt.listener[1]);
                }

                //Add listener to option of list as json.
                if(_$_.isValidJSON(add_opt.listener)){
                    $add_option.on(add_opt.listener.event, add_opt.listener.callback);
                }
                $combobox_content.append($add_option);
            }
        }

        let $wrap_option = $("<div class='wrap-option'></div>");
        switch(this.setting.content.scroll){
            case "vertical": $wrap_option.addClass("scroll-y"); break;
            case "none": $wrap_option.removeClass("scroll-y"); break;
            default: $wrap_option.addClass("scroll-y");
        }

        let data = this.setting.content.data;
        let opt = this.setting.content.option;
        if(_$_.validArray(data)){
            data.forEach(function(d){
                let $option = $("<a class='option' href='#' data-key='"+ d.key +"'>" + " " + d.value +"</a>");
                let $icon = $("<i class='"+ opt.icon +"'></i>");
                if(d.icon !== undefined){
                    if(_$_.isHTML(d.icon)){
                        $icon = d.icon;
                    } else {
                        $icon.addClass(d.icon);
                    }
                }
                $option.prepend($icon);

                if(d.url !== undefined){
                    $option.prop("href", d.url);
                } 

                if(opt.has_title){
                    $option.prop("title", d.value);
                }

                if(d.title !== undefined){
                    $option.prop("title", d.title);
                }

                if(_$_.validArray(opt.listener)){
                    $option.on(opt.listener[0], opt.listener[1]);
                }

                if(_$_.isValidJSON(opt.listener)){
                    $option.on(opt.listener.event, opt.listener.callback);
                }

                $wrap_option.append($option);
            });
        }
 
        $combobox_content.append($wrap_option);
        $wrap_combobox_content.append($combobox_content);
        $combobox.append($wrap_combobox_content);
    }

    async render(){
        let $combobox = $("<div class='combobox crumb'></div>");
        if(_$_.isValidJSON(this.setting)){
            if(this.setting.type !== undefined){
                $combobox.addClass(this.setting.type);
            }
            
            if(this.setting.direction !== undefined){
                $combobox.addClass(this.setting.direction);
            }
            
            if(this.setting.withCrumb !== undefined && !this.setting.withCrumb){
                $combobox.removeClass("crumb");
            } 
        }

        await this.buildTrigger($combobox);
        await this.buildContent($combobox);
        this._self = $combobox;   
    }

    async init(config){
        if(!_$_.isValidJSON(config)){ return; };
        this.setting = config;
        if(this.setting.container !== undefined){ 
            await this.render(); 
            switch(this.setting.insertion){
                case "prepend":
                    $(this.setting.container).prepend(this.self);
                break;

                case "append":
                    $(this.setting.container).append(this.self);
                break;

                case "replace":
                    $(this.setting.container).html(this.self);
                break;
                
                default:
                    $(this.setting.container).prepend(this.self);
            }  
            this.onTarget(this.setting.onTarget);
        }   
         
    }
};

//Bind default listener for predefined html structure of combobox.
$(function ($, container) {
    $("html, .widget-combobox .combobox-trigger, .widget-combobox .i-close").on("mousedown", function (e) {
        if($(this).is($(".combobox-trigger"))){
            $(this).toggleClass("show").siblings(".combobox-content").toggleClass("show"); 
        }
       
        if($(e.target).is($(".i-close"))) {
            $(e.target).parents(".combobox-content").removeClass("show").siblings(".combobox-trigger").removeClass("show");
        }

        if (!$(e.target).is($(".widget-combobox *")) && !$(".widget-combobox").is($(".menubox"))) {
            $(".widget-combobox .combobox-content", container).removeClass("show").siblings(".combobox-trigger").removeClass("show");
        }  
    });
}(jQuery, ".ck-widget-container"));









