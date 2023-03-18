class AjaxLoader 
{
    constructor(image, config){
        this._self = $("<div class='ajax-loader'></div>");
        this._content = $("<div class='content'></div>");
        this._setting = {
            container: "body",
            image: image,
        }

        if(config !== undefined){
            this.setting = config;
        }
        this.refresh();
    }


    set setting(config){
        if(_$_.isValidJSON(config)){
            this._setting = $.extend(true, {}, this._setting, config);
        } 
    }

    get setting(){ return this._setting; }
    get self(){ return this._self; }
    get content(){ return this._content; }

    refresh(){
        setTimeout(() => {
            this.build(this.setting);
        }, 0);
    }

    async build(config){
        this.setting = config;
        let $image = $("<img src='"+ this.setting.image +"'/>");
        this.content.html($image);
        this.self.append(this.content);
        $(this.setting.container).append(this.self);
    }

    async show(){
        this.self.removeClass("hide");
    }

    async hide(){
        this.self.addClass("hide");
    }

    async close(){
        if(this instanceof AjaxLoader){
            setTimeout(() => {
                delete this;
                this.self.remove();
            }, 0);
        }
    }
}