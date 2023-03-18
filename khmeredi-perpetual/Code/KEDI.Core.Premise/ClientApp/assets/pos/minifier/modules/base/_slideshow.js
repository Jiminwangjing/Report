class Slideshow
{
    constructor(config){
        this._self = $("<div class='slideshow'></div>");
        this._content = $("<div class='content'></div>");
        this._setting = {
            container: "",
            image_list: []
        }
    }

    set setting(config) {
        this._setting = $.extend(true, {}, this._setting, config);
    }
    get setting() {return this._setting; }

    get self(){ return this._self; }
    get content(){ return this._content; }

    async build(){
        

        this.setting.container.html(this.self);
    }

}