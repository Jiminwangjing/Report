
; (function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
        typeof define === 'function' && define.amd ? define(factory) : global.ViewMessage = factory;
}(this, function ViewMessage(config, model) {
    if (!(this instanceof ViewMessage)) {
        return new ViewMessage(config, model);
    }
    const __setting = {
        summary: {
            title: {
                text: "Validation Summary",
                text_align: "left",
                font_size: "1rem",
                font_weight: "bold",
                color: "",
                backgroundColor: "#E0E5EB"
            },
            content: {
                symbol: "",
                color: "",
                backgroundColor: "#E6EAEF"
            },
            selector: "[data-valmsg-summary]",
            enabled: false,
            attribute: "",
            bordered: false
        },
        for: {
            aspAttribute: "data-valmsg-for",
            attribute: "vm-for",
            color: ""
        },       
        autoclose: {
            delay: 1000,
            duration: 5000,
            action: 1, //{REJECT = -1, ALERT = 0, CONFIRM = 1}
            callback: function () { }
        }
    },
    __meta = {
        proto: this,
        template: document.createElement("div"),
        model: {
            action: -1, //{ REJECT = -1, ALERT = 0, CONFIRM = 1 }
            data: {},
            redirect: "" //{ CURRENT = 0 }
        },
    }

    //Initialize setting
    if (isValidJSON(config)) {
        $.extend(true, __setting, config);   
    }

    if (isValidJSON(model)) {
        setTimeout(function () {
            __meta.proto.bind(model);
        });      
    }
   
    this.render = function (value) {
        let model_action = __meta.model.action.toString().toLowerCase();
        let autoclose_action = __setting.autoclose.action.toString().toLowerCase();
        __meta.model.data = Object.assign({}, value);
        let summary_message = $(__setting.summary.selector);
        if (!!__setting.summary.attribute) {
            summary_message = $("[" + __setting.summary.attribute + "]");
        }
        
        let ul = $("<ul></ul>").css("padding", "0 10px");            
        if (__setting.summary.bordered) {
            $(__meta.template).css({
                "border-radius": "3px",
                "-webkit-box-shadow": "0px 0px 0.5px 0px rgba(0, 0, 0, 0.15)",
                "-moz-box-shadow": "0px 0px 0.5px 0px rgba(0, 0, 0, 0.15)",
                "box-shadow": "0px 0px 0.5px 0px rgba(0, 0, 0, 0.15)"
            });

            let title = $("<div>" + __setting.summary.title.text + "</div>").css({
                "border-top-left-radius": "3px",
                "border-top-right-radius": "3px",
                "height": "35px",
                "line-height": "35px",
                "border-bottom": ".5px solid rgba(0, 0, 0, .1)",
                "padding": "0 10px",
                "background-color": __setting.summary.title.backgroundColor,
                "font-size": __setting.summary.title.font_size,
                "font-weight": __setting.summary.title.font_weight,
                "text-align": __setting.summary.title.text_align
            });

            ul.css({
                "list-style-type": "'" + __setting.summary.content.symbol + "'",
                "background-color": __setting.summary.content.backgroundColor,
                "margin": "0",
                "padding": "15px 30px",
                "border-bottom-left-radius": "3px",
                "border-bottom-right-radius": "3px"
            });

            $(__meta.template).append(title);
        }

        ul.appendTo(__meta.template);                
        if (__setting.summary.content.symbol === "") {
            switch (model_action) {
                case "reject": case "-1":
                    ul.css({ "list-style-type": "'✘'" });
                    break;
                case "alert": case "0":
                    ul.css({ "list-style-type": "'⚠'" });
                    break;
                case "confirm": case "1":
                    ul.css({ "list-style-type": "'✔'" });
                    break;
            }
        }       

        $.each(Object.keys(__meta.model.data), function (i, key) {
            if (__meta.model.data[key]) {
                let li = $("<li>" + __meta.model.data[key] + "</li>");
                li.css({
                    "font-family": "monospace",
                    "font-size": "0.8em",
                    "padding": "5px",
                    "color": "#EC5B5B",
                    "width": "100%",
                    "white-space": "normal",
                    "word-break": "break-word",
                    "background-color": "transparent"
                });

                li.css({ "color": __setting.summary.content.color });
                if (__setting.summary.content.color === "") {
                    switch (model_action) {
                        case "reject": case "-1":
                            li.css({ "color": "#EC5B5B" });
                            break;
                        case "alert": case "0":
                            li.css({ "color": "#F09F09" });
                            break;
                        case "confirm": case "1":
                            li.css({ "color": "#50C05E" });
                            break;
                    }
                }
                
                ul.append(li);
            }
        });

        let forNodes = $("[" + __setting.for.attribute + "]"),
            aspForNodes = $("[" + __setting.for.aspAttribute + "]");
        if (forNodes.length > 0) {
            console.log("model data -> ", __meta.model.data)
            for (let i = 0; i < forNodes.length; i++) {
                defineForMessage(forNodes[i], forNodes[i].getAttribute(__setting.for.attribute), __meta.model.data);
            }
        }

        if (aspForNodes.length > 0) {
            for (let i = 0; i < aspForNodes.length; i++) {
                defineForMessage(aspForNodes[i], aspForNodes[i].getAttribute(__setting.for.aspAttribute), __meta.model.data);
            }
        }

        if (Object.keys(__meta.model.data).length > 0) {
            if (!!summary_message) {                 
                summary_message.html(__meta.template);
            }
        } else {
            __meta.proto.close();
        }

        if (autoclose_action === model_action) {
            __meta.proto.autoclose(__setting.autoclose.callback);
        }

        if (!!__meta.model.redirect.toLowerCase()) {
            __meta.proto.redirect(__meta.model.redirect);
        }

        return __meta.proto;
    }

    this.bind = function (model) {
        if (isValidJSON(model)) {
            for (let p of Object.keys(model)) {
                if (model[p] != undefined) {
                    __meta.model[p.toLowerCase()] = model[p];
                }
            }
        }
        return this.render(__meta.model.data);
    }

    this.appendTo = function (container) {
        if (container !== undefined) {
            $(container).append(__meta.template);
        }
    }

    this.close = function(callback) {
        if (this instanceof ViewMessage) {
            let _summary = $(__setting.summary.selector);
            if (!!__setting.summary.attribute) {
                _summary = $("[" + __setting.summary.attribute + "]");
            }
            let _for = $("[" + __setting.for.attribute + "]");
            setTimeout(() => {
                _summary.children().fadeOut(0, function () {
                    if (!!callback && typeof callback === "function") {
                        Promise.resolve(callback(__self, this)).then($(this).remove());
                    }
                });

                _for.children().fadeOut(0, function () {
                    if (!!callback && typeof callback === "function") {
                        Promise.resolve(callback(__self, this)).then($(this).remove());
                    }
                });
            }, 0);
            __meta.model.data = {};
            delete this;
        }
    }

    this.autoclose = function(callback) {
        if (this instanceof ViewMessage) {
            let __self = this;
            let _autoclose = __setting.autoclose;
            let _summary = $(__setting.summary.selector);
            if (!!__setting.summary.attribute) {
                _summary = $("[" + __setting.summary.attribute + "]");
            }
            let _for = $("[" + __setting.for.attribute + "]");
            setTimeout(() => {
                _summary.children().fadeOut(_autoclose.duration, function () {
                    if (!!callback && typeof callback === "function") {
                        Promise.resolve(callback(__self, this)).then($(this).remove());
                    }
                });

                _for.children().fadeOut(_autoclose.duration, function () {
                    if (!!callback && typeof callback === "function") {
                        Promise.resolve(callback(__self, this)).then($(this).remove());
                    }
                });
            }, _autoclose.delay);
            delete this;
        }

    }

    this.dialog = function () {
        $(__meta.template).css({
            "position": "fixed",
            "top": "0",
            "z-index": "9999"
        });
        return this;
    }

    this.refresh = function(delay, force) {
        setTimeout(() => {
            if (force) {
                location.reload(force);
            }
            location.reload();
        }, delay);
    }

    this.redirect = function(url, delay, force) {
        url = url.toLowerCase();
        setTimeout(() => {
            if (url === "." || url === "current" || url == "0") {
                if (force) {
                    location.reload(force);
                }
                location.reload();
            } else {
                location.href = url;
            }
        }, delay);
    }

    this.remove = function (key) {
        if (!!key) {
            if (typeof key === "string") {
                delete __meta.model.data[key];
            }

            if (isValidArray(key)) {
                for (let k of key) {
                    __meta.proto.remove(k);
                }
            }
            render();
        }
    }

    this.clear = function () {
        __meta.model.data = {};
        render();
    }

    /*................../
    / Private methods   /
    /..................*/

    //Validation Message For Each Controls
    function defineForMessage(node, key, value) {
        const modelaction = __meta.model.action.toString().toLowerCase();  
        if (node) {
            node.textContent = "";
            node.style.removeProperty("color");
            let p = key;          
            if ((p.indexOf(".") > 0)) {
                let props = p.split(".");
                p = props[props.length - 1];
            }   
            
            if (value[p]) {
                $(node).html("<span>" + value[p] + "</span>");
                if (__setting.for.color === "") {
                    switch (modelaction) {
                        case "reject": case "-1":
                            $(node).css("color", "#EC5B5B");
                            break;
                        case "alert": case "0":
                            $(node).children().css("color", "#F09F09");
                            break;
                        case "confirm": case "1":
                            $(node).children().css("color", "#50C05E");
                            break;
                    }
                }
            }
        }
    }

    //Check if json object is valid.
    function isValidJSON(json) {
        return json !== undefined && json.constructor === Object && Object.keys(json).length > 0;
    };

    //Check if value is valid array.
    function isValidArray(array) {
        return array !== undefined && Array.isArray(array) && array.length > 0;
    };
}));





