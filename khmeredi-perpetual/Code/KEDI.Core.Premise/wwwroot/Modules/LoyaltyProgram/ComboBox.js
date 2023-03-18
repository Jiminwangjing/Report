function ComboBox(setting) {
    if (!(this instanceof ComboBox)) {
        return new ComboBox(setting);
    }
    const __this = this;
    var __setting = {
        container: undefined,
        selector: undefined,
        trigger: {
            icon: "<i class='fas fa-gift fa-lg'></i>"
        },
        classList: "widget-combobox slide-down with-crumb pop-right"
    };

    const __meta = {
        dataset: new Array(),
        template: document.createElement("div"),
        prototype: this,
        id: undefined
    }, __template = {
        div: document.createElement("div"),
        i: document.createElement("i")
    }

    if (isValidJSON(setting)) {
        $.extend(true, __setting, setting);
        if ($(__setting.selector).length > 0) {
            __meta.template = $(__setting.selector)[0];
        } else {
            __meta.template = __template.div.cloneNode();
        }
    }

    setTimeout(function () {
        if (__meta.id === undefined) {
            __this.render();
        }   
    });

    this.render = function () {
        __meta.id = Date.now();
        __meta.template.setAttribute("data-id", __meta.id);
        __meta.template.setAttribute("class", __setting.classList);
        if (__setting.container) {          
            __meta.template.appendChild(createTrigger());
            __meta.template.appendChild(createContent());
            $(__setting.container).html(__meta.template);
        }
    }

    function createTrigger() {
        var trigger = document.createElement("div");
        trigger.setAttribute("class", "combobox-trigger");
        $(trigger).html(__setting.trigger.icon);
        $(trigger).on("click", function () {
            var $content = $(this).siblings(".combobox-content");
            $content.toggleClass("show");
            if ($content.hasClass("show")) {
                $(this).addClass("show");
            } else {
                $(this).removeClass("show");
            }
        });
        return trigger;
    }

    function createHeader() {
        var header = document.createElement("div");
        var iclose = document.createElement("i");
        var title = document.createElement("h4");
        header.setAttribute("class", "combobox-header");
        title.setAttribute("class", "title");
        title.appendChild(document.createTextNode(__setting.title));
        iclose.setAttribute("class", "i-close");
        header.appendChild(title);
        header.appendChild(iclose);
        return header;
    }

    function createContent() {
        var content = document.createElement("div");
        var body = document.createElement("div");
        content.setAttribute("class", "combobox-content");
        body.setAttribute("class", "combobox-body");
        content.appendChild(createHeader());
        content.appendChild(body);
        return content;
    }

    //Check if json object is valid.
    function isValidJSON(json) {
        return json !== undefined && json.constructor === Object && Object.keys(json).length > 0;
    };

    //Check if value is valid array.
    function isValidArray(array) {
        return array !== undefined && Array.isArray(array) && array.length > 0;
    };
}

$(document).ready(function () {
    var combobox = ComboBox({
        container: "#buyx-getx-combobox",
        title: "Loyalty Program"
    });
    
});
