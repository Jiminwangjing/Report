
; (function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
        typeof define === 'function' && define.amd ? define(factory) : global.ViewMessage = factory;
}(this,
    (class ViewMessage {
        constructor(config, model) {      
            this._setting = {
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
                    selector: "",
                    attribute: "",
                },
                for: {
                    attribute: "vm-for",
                    color: ""
                },
                model: {
                    action: -1, //{REJECT = -1, ALERT = 0, CONFIRM = 1 }
                    data: {},
                    redirect: "" //{ CURRENT = 0 }
                },
                autoclose: {
                    delay: 1000,
                    duration: 5000,
                    action: 1, //{REJECT = -1, ALERT = 0, CONFIRM = 1}
                    callback: function (self_object, self_dom) { }
                }
            }

           
            if (!model) {
                if (!!config && typeof config === "string") {
                    this.setting.summary.selector = config;
                }
                this.bind(config);                
            } else {
                this.bind(model);
            }

            this.setting = config;                       
            setTimeout(() => {
                this.build();
            }, 0);
        }

        set setting(value) {
            if (!!value && value.constructor === Object && Object.keys(value).length > 0) {
                this._setting = $.extend(true, {}, this._setting, value);
            }
        }

        get setting() { return this._setting; }

        set data(value) {
            if (!!value) {
                if (Array.isArray(value) && value.length > 0) {
                    for (let item of value) {
                        this.data = item;
                    }
                }

                if (value.constructor === Object && Object.getOwnPropertyNames(value).length > 0) {
                    for (let p of Object.getOwnPropertyNames(value)) {
                        if (value.hasOwnProperty(p)) {
                            this.data[p] = value[p];
                        }
                    }
                }
            }

            this.build();
        }

        get data() { return this.setting.model.data; }

        add(key, value) {
            let item = {};
            item[key] = value;
            this.data = item;
            this.build();
        }

        bind(model) {
            if (!!model && model.constructor === Object && Object.keys(model).length > 0) {
                for (let p of Object.keys(model)) {
                    if (model[p] != undefined) {
                        this.setting.model[p.toLowerCase()] = model[p];
                    }                  
                }            
            }
            this.build();
            return this;
        }

        remove(key) {
            if (!!key) {
                if (typeof key === "string") {
                    delete this.setting.model.data[key];
                }

                if (Array.isArray(key) && key.length > 0) {
                    for (let k of key) {
                        this.remove(k);
                    }
                }
                this.build();
            }
        }

        clear() {
            this.setting.model.data = {};
            this.build();
        }

        build(values) {
            let __self = this;
            let model_action = this.setting.model.action.toString().toLowerCase();
            let autoclose_action = this.setting.autoclose.action.toString().toLowerCase();
            if (!!values) {
                this.data = values;
            }

            let summary_message = $(this.setting.summary.selector);
            if (!!this.setting.summary.attribute) {
                summary_message = $("[" + this.setting.summary.attribute + "]");
            }

            let forNodes = $("[" + this.setting.for.attribute + "]");
            let _box = $("<div></div>");
            _box.css({
                "border-radius": "3px",
                "-webkit-box-shadow": "0px 0px 0.5px 0px rgba(0, 0, 0, 0.15)",
                "-moz-box-shadow": "0px 0px 0.5px 0px rgba(0, 0, 0, 0.15)",
                "box-shadow": "0px 0px 0.5px 0px rgba(0, 0, 0, 0.15)"
            });

            let _title = $("<div>" + this.setting.summary.title.text + "</div>").css({
                "border-top-left-radius": "3px",
                "border-top-right-radius": "3px",
                "height": "35px",
                "line-height": "35px",
                "border-bottom": ".5px solid rgba(0, 0, 0, .1)",
                "padding": "0 10px",
                "background-color": this.setting.summary.title.backgroundColor,
                "font-size": this.setting.summary.title.font_size,
                "font-weight": this.setting.summary.title.font_weight,
                "text-align": this.setting.summary.title.text_align
            });

            let ul = $("<ul></ul>").css({
                "list-style-type": "'" + this.setting.summary.content.symbol + "'",
                "background-color": this.setting.summary.content.backgroundColor,
                "margin": "0",
                "padding": "15px 30px",
                "border-bottom-left-radius": "3px",
                "border-bottom-right-radius": "3px"
            }).appendTo(_box);
            
            if (this.setting.summary.content.symbol === "") {
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
           
            $.each(Object.keys(__self.data), function (i, key) {
                if (__self.data[key]) {
                    let li = $("<li>" + __self.data[key] + "</li>");
                    li.css({
                        "padding": "5px",
                        "color": "#EC5B5B",
                        "width": "100%",
                        "white-space": "normal",
                        "word-break": "break-all"
                    });

                    li.css({ "color": __self.setting.summary.content.color });
                    if (__self.setting.summary.content.color === "") {
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
                    __self.defineForMessage(forNodes[i], __self.data);                  
                    ul.append(li);
                }
            });


            if (Object.keys(this.data).length > 0) {
                if (!!summary_message) {
                    if (!!this.setting.summary.title.text) {
                        ul.before(_title);
                    }
                    summary_message.html(_box);
                }

            } else {
                this.close();
            }

            if (autoclose_action === model_action) {
                this.autoclose(this.setting.autoclose.callback);
            }

            if (!!this.setting.model.redirect.toLowerCase()) {
                this.redirect(this.setting.model.redirect);
            }
        }

        //action Message For Each Controls
        defineForMessage(node, value) {          
            const modelaction = this.setting.model.action.toString().toLowerCase();       
            if (node) {
                node.textContent = "";
                node.style.removeProperty("color");
                let p = node.getAttribute(this.setting.for.attribute);
                if (value[p]) {
                    $(node).html("<span>" + value[p] + "</span>");
                    if (this.setting.for.color === "") {
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

        close(callback) {
            if (this instanceof ViewMessage) {
                let _summary = $(this.setting.summary.selector);
                if (!!this.setting.summary.attribute) {
                    _summary = $("[" + this.setting.summary.attribute + "]");
                }
                let _for = $("[" + this.setting.for.attribute + "]");
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
                this.setting.model.data = {};
                delete this;
            }
        }

        autoclose(callback) {
            if (this instanceof ViewMessage) {
                let __self = this;
                let _autoclose = this.setting.autoclose;
                let _summary = $(this.setting.summary.selector);
                if (!!this.setting.summary.attribute) {
                    _summary = $("[" + this.setting.summary.attribute + "]");
                }
                let _for = $("[" + this.setting.for.attribute + "]");
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

        refresh(delay, force) {
            setTimeout(() => {
                if (force) {
                    location.reload(force);
                }
                location.reload();
            }, delay);
        }

        redirect(url, delay, force) {
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
    }

)));





