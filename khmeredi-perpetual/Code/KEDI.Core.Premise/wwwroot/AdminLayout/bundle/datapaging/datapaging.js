var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
(function (Kernel) {
    var DataEngine = /** @class */ (function () {
        function DataEngine(setting) {
            this.setting = {};
            this.data = new Array();
            this.active = false;
            this.setting = $.extend(true, this.setting, setting);
        }
        DataEngine.prototype.addEvent = function (element, event, callback) {
            if (element.attachEvent) {
                return element.attachEvent('on' + event, callback);
            }
            else {
                return element.addEventListener(event, callback, false);
            }
        };
        DataEngine.prototype.start = function (values, process) {
            var _this = this;
            this.data = values;
            setTimeout(function () {
                _this.render(_this.data, process);
            }, 0);
            return this;
        };
        return DataEngine;
    }());
    var DataPaging = /** @class */ (function (_super) {
        __extends(DataPaging, _super);
        function DataPaging(option) {
            var _this = _super.call(this, option) || this;
            _this.selfElement = document.createElement("ul");
            _this.indexWalk = document.createElement("li");
            _this.previousWalk = document.createElement("li");
            _this.firstwalk = document.createElement("li");
            _this.nextWalk = document.createElement("li");
            _this.lastWalk = document.createElement("li");
            _this.__index__ = {};
            _this.setting = {
                container: undefined,
                identity: 1,
                pageSize: 5,
                startIndexSize: 5,
                align: "right",
                style: {
                    class: "ck-pagination",
                    mode: "square",
                    border: 1,
                    font: {
                        size: "1em",
                        weight: "bold",
                        family: "monospace"
                    }
                },
                onPageQuery: function (brief) { }
            };
            _this.setting = $.extend(true, _this.setting, option);
            Object.defineProperty(_this, "__index__", {
                enumerable: false,
                configurable: false
            });
            return _this;
        }
        DataPaging.prototype.getSelfElement = function () { return this.selfElement; };
        DataPaging.prototype.countMaxPage = function () {
            var opt = this.setting;
            var maxPageCount = Math.floor(this.data.length / parseInt(opt.pageSize));
            if (this.data.length % parseInt(opt.pageSize) !== 0) {
                maxPageCount = maxPageCount + 1;
            }
            return maxPageCount;
        };
        DataPaging.prototype.getCurrentIndex = function () {
            return parseInt(this.__index__["current"]);
        };
        DataPaging.prototype.getStartIndex = function () {
            return parseInt(this.setting.startIndexSize);
        };
        DataPaging.prototype.getDefaultIndexes = function () {
            var c = 2, indexes = new Array(), defaultSize = this.getStartIndex();
            if (this.getStartIndex() > this.countMaxPage()) {
                defaultSize = this.countMaxPage();
            }
            while (c <= defaultSize) {
                indexes.push(c++);
            }
            return indexes;
        };
        DataPaging.prototype.defineStyle = function () {
            var style = this.setting.style;
            t$.query(this.selfElement).addClass("ck-pagination").addClass(style.class);
            switch (style.mode) {
                case "square":
                    t$.query(this.selfElement).addClass("square");
                    break;
                case "circle":
                    t$.query(this.selfElement).addClass("circle");
                    break;
            }
            this.selfElement.style.fontFamily = style.font.family;
            this.selfElement.style.fontFamily = style.font.family;
            this.selfElement.style.fontSize = style.font.size;
            this.selfElement.style.fontWeight = style.font.weight;
            if (style.border > 0) {
                t$.query(this.selfElement).addClass("border");
            }
        };
        DataPaging.prototype.buildFirstWalk = function () {
            var self = this, option = this.setting;
            self.firstwalk.textContent = "";
            self.firstwalk.appendChild(document.createTextNode("1"));
            self.firstwalk.setAttribute("aria-label", "first-index");
            self.firstwalk.setAttribute("aria-label", "first-index");
            t$.query(self.firstwalk).addClass("active");
            this.addEvent(self.firstwalk, "click", function (event) {
                var _a;
                self.__index__["current"] = 1;
                if (self.getStartIndex() < self.countMaxPage()) {
                    self.offsetPrevious(self.countMaxPage());
                }
                t$.query(self.firstwalk).addClass("active");
                t$.each((_a = self.firstwalk.parentNode) === null || _a === void 0 ? void 0 : _a.childNodes, function (node, i) {
                    if (node !== self.firstwalk) {
                        t$.query(node).removeClass("active");
                    }
                });
                option.onPageQuery(self.brief());
            });
            this.selfElement.appendChild(this.firstwalk);
        };
        DataPaging.prototype.buildPreviousWalk = function () {
            var paging = this, option = this.setting;
            this.previousWalk.setAttribute("aria-label", "previous-index");
            this.previousWalk.innerHTML = "&lt;&lt;";
            this.addEvent(this.previousWalk, "click", onPrevious);
            this.addEvent(window, "keydown", function (e) {
                if (e.which === 37) { //ArrowLeft
                    onPrevious(e);
                }
            });
            function onPrevious(e) {
                if (paging.getCurrentIndex() > 1) {
                    paging.__index__["current"]--;
                    if (paging.getCurrentIndex() <= paging.countMaxPage() - paging.getStartIndex()) {
                        paging.offsetPrevious(1);
                    }
                }
                t$.each(paging.selfElement.childNodes, function (node, i) {
                    if (parseInt(node.textContent) === paging.getCurrentIndex()) {
                        t$.query(node).addClass("active");
                    }
                    else {
                        t$.query(node).removeClass("active");
                    }
                });
                option.onPageQuery(paging.brief());
            }
            ;
            this.selfElement.appendChild(this.previousWalk);
        };
        DataPaging.prototype.buildNextWalk = function () {
            var paging = this, option = this.setting;
            this.nextWalk.setAttribute("aria-label", "next-index");
            this.nextWalk.innerHTML = "&gt;&gt;";
            this.addEvent(this.nextWalk, "click", onNext);
            this.addEvent(window, "keydown", function (e) {
                if (e.which === 39) { //ArrowRight
                    onNext(e);
                }
            });
            function onNext(e) {
                if (paging.getCurrentIndex() < paging.countMaxPage()) {
                    paging.__index__["current"]++;
                    if (paging.getCurrentIndex() > paging.getStartIndex()) {
                        paging.offsetNext(1);
                    }
                }
                t$.each(paging.selfElement.childNodes, function (node, i) {
                    if (parseInt(node.textContent) === paging.getCurrentIndex()) {
                        t$.query(node).addClass("active");
                    }
                    else {
                        t$.query(node).removeClass("active");
                    }
                });
                option.onPageQuery(paging.brief());
            }
            ;
            this.selfElement.appendChild(this.nextWalk);
        };
        DataPaging.prototype.buildLastWalk = function () {
            var paging = this, option = this.setting;
            this.lastWalk.setAttribute("aria-label", "last-index");
            this.lastWalk.appendChild(document.createTextNode(paging.countMaxPage().toString()));
            this.addEvent(this.lastWalk, "click", function (e) {
                var _a;
                paging.offsetNext(paging.countMaxPage());
                paging.__index__["current"] = paging.countMaxPage();
                t$.query(paging.lastWalk).addClass("active");
                t$.each((_a = paging.lastWalk.parentNode) === null || _a === void 0 ? void 0 : _a.childNodes, function (node, i) {
                    if (node !== paging.lastWalk) {
                        t$.query(node).removeClass("active");
                    }
                });
                option.onPageQuery(paging.brief());
            });
            this.selfElement.appendChild(this.lastWalk);
        };
        DataPaging.prototype.buildSkipPrevious = function () {
            var paging = this, option = this.setting, skipPrevNode = this.indexWalk.cloneNode(false);
            skipPrevNode.setAttribute("aria-label", "skip-previous");
            skipPrevNode.className = "hidden";
            skipPrevNode.appendChild(document.createTextNode("..."));
            this.addEvent(skipPrevNode, "click", onSkipPrev);
            this.addEvent(window, "keydown", function (e) {
                if (e.which === 40) { //ArrowDown
                    onSkipPrev(e);
                }
            });
            function onSkipPrev(e) {
                var _a;
                paging.offsetPrevious(paging.getStartIndex() - 1);
                var activeNode = skipPrevNode.nextElementSibling;
                t$.query(activeNode).addClass("active");
                t$.each((_a = activeNode.parentNode) === null || _a === void 0 ? void 0 : _a.childNodes, function (node, i) {
                    if (node !== activeNode) {
                        t$.query(node).removeClass("active");
                    }
                });
                paging.__index__["current"] = parseInt(activeNode.textContent);
                option.onPageQuery(paging.brief());
            }
            ;
            this.selfElement.appendChild(skipPrevNode);
        };
        DataPaging.prototype.buildSkipNext = function () {
            var paging = this, option = this.setting, skipNextNode = this.indexWalk.cloneNode(false);
            skipNextNode.setAttribute("aria-label", "skip-next");
            skipNextNode.appendChild(document.createTextNode("..."));
            this.addEvent(skipNextNode, "click", onSkipNext);
            this.addEvent(window, "keydown", function (e) {
                if (e.which === 38) { //ArrowUp
                    onSkipNext(e);
                }
            });
            function onSkipNext(e) {
                var _a;
                paging.offsetNext(paging.getStartIndex() - 1);
                var activeNode = skipNextNode.previousElementSibling;
                t$.query(activeNode).addClass("active");
                t$.each((_a = activeNode.parentNode) === null || _a === void 0 ? void 0 : _a.childNodes, function (node, i) {
                    if (node !== activeNode) {
                        t$.query(node).removeClass("active");
                    }
                });
                paging.__index__["current"] = parseInt(activeNode.textContent);
                option.onPageQuery(paging.brief());
            }
            this.selfElement.appendChild(skipNextNode);
        };
        DataPaging.prototype.buildIndexWalk = function () {
            var paging = this, option = this.setting;
            paging.__index__["numbers"] = new Array();
            paging.__index__["nodes"] = new Array();
            t$.each(paging.getDefaultIndexes(), function (m, i) {
                var indexNode = paging.indexWalk.cloneNode(false);
                indexNode.setAttribute("aria-label", "index");
                indexNode.appendChild(document.createTextNode(m.toString()));
                paging.__index__["numbers"].push(m);
                paging.__index__["nodes"].push(indexNode);
                paging.addEvent(indexNode, "click", onIndex);
                function onIndex(e) {
                    var _a;
                    paging.__index__["current"] = parseInt(indexNode.textContent);
                    t$.query(indexNode).addClass("active");
                    t$.each((_a = indexNode.parentNode) === null || _a === void 0 ? void 0 : _a.childNodes, function (node, i) {
                        if (node !== indexNode) {
                            t$.query(node).removeClass("active");
                        }
                    });
                    option.onPageQuery(paging.brief());
                }
                paging.selfElement.appendChild(indexNode);
            });
        };
        DataPaging.prototype.render = function (values, callback) {
            this.__index__["current"] = 1;
            if (t$.isValidArray(values)) {
                this.data = values;
            }
            if (callback) {
                this.setting.onPageQuery = callback;
            }
            this.setting.onPageQuery(this.brief());
            this.selfElement.textContent = "";
            this.buildPreviousWalk();
            this.buildFirstWalk();
            this.buildSkipPrevious();
            this.buildIndexWalk();
            if (this.countMaxPage() > this.getStartIndex() + 1) {
                this.buildSkipNext();
            }
            if (this.getStartIndex() < this.countMaxPage()) {
                this.buildLastWalk();
            }
            this.buildNextWalk();
            this.defineStyle();
            if (this.setting.container) {
                if (typeof this.setting.container === "string") {
                    t$.getElement(this.setting.container).appendChild(this.selfElement);
                }
                else {
                    this.setting.container.appendChild(this.selfElement);
                }
            }
            return this;
        };
        DataPaging.prototype.offsetPrevious = function (offsetSize) {
            this.offsetNext(-offsetSize);
        };
        DataPaging.prototype.offsetNext = function (offsetSize) {
            var paging = this, maxPageCount = this.countMaxPage(), indexNumbers = this.__index__["numbers"], indexNodes = this.__index__["nodes"], n = indexNodes.length;
            if (this.getStartIndex() < this.countMaxPage()) {
                t$.query(this.selfElement).$child("li[aria-label=skip-previous]").removeClass("hidden");
                t$.query(this.selfElement).$child("li[aria-label=skip-next]").removeClass("hidden");
                t$.each(indexNodes, function (elem, i) {
                    indexNumbers[i] += offsetSize;
                    if (indexNumbers[0] <= 2) {
                        indexNumbers[i] = i + 2;
                        t$.query(paging.selfElement).$child("li[aria-label=skip-previous]").addClass("hidden");
                    }
                    if (indexNumbers[i] >= maxPageCount - n + i) {
                        indexNumbers[i] = maxPageCount - n + i;
                        t$.query(paging.selfElement).$child("li[aria-label=skip-next]").addClass("hidden");
                    }
                    elem.textContent = indexNumbers[i];
                });
            }
        };
        DataPaging.prototype.brief = function () {
            return {
                self: this,
                first: 1,
                prevous: this.getCurrentIndex() - 1,
                current: this.getCurrentIndex(),
                next: this.getCurrentIndex() + 1,
                last: this.countMaxPage(),
                data: t$.paginate(this.data, this.setting.pageSize, this.getCurrentIndex())
            };
        };
        DataPaging.prototype.onPageQuery = function (callback) {
            this.setting.onPageQuery = callback;
            return this;
        };
        return DataPaging;
    }(DataEngine));
    Kernel.DataPaging = DataPaging;
    var t$ = /** @class */ (function () {
        function t$(selector) {
            if (typeof selector === "string") {
                this.__elements__ = document.querySelector(selector);
            }
            else {
                this.__elements__ = selector;
            }
        }
        t$.query = function (selector) {
            return new t$(selector);
        };
        t$.on = function (selector, event, callback) {
            var el = t$.getElement(selector);
            if (callback && typeof callback === "function") {
                if (el.attachEvent) {
                    return el.attachEvent('on' + event, callback);
                }
                else {
                    return el.addEventListener(event, callback, false);
                }
            }
            return el;
        };
        t$.prototype.hasClass = function (className) {
            return ((" " + this.__elements__.className + " ")
                .replace(/[\t\r\n\f]/g, " ").indexOf(className) > -1);
        };
        t$.hasClass = function (selector, className) {
            return t$.query(selector).hasClass(className);
        };
        t$.prototype.addClass = function (className) {
            var element = this.__elements__;
            if (!((" " + element.className + " ").replace(/[\t\r\n\f]/g, " ")
                .indexOf(className) > -1)) {
                element.className += " " + className;
            }
            return this;
        };
        t$.addClass = function (selector, className) {
            return t$.query(selector).addClass(className);
        };
        t$.prototype.removeClass = function (className) {
            var element = this.__elements__, oldNames = element.className.split(/[\s]+/), matches = new Array();
            t$.each(oldNames, function (name, i) {
                if (className.indexOf(name) < 0) {
                    matches.push(name);
                }
            });
            element.className = matches.join(" ");
            return this;
        };
        t$.removeClass = function (selector, className) {
            return t$.query(selector).removeClass(className);
        };
        t$.prototype.toggleClass = function (className) {
            var element = this.__elements__, classNames = className.split(/[\s]+/);
            t$.each(classNames, function (c) {
                if (t$.query(element).hasClass(c)) {
                    t$.query(element).removeClass(c);
                }
                else {
                    t$.query(element).addClass(c);
                }
            });
            return this;
        };
        t$.siblings = function (self, selector, filter) {
            if (selector === void 0) { selector = ""; }
            if (filter === void 0) { filter = null; }
            var result = new Array(), i = 0, node = self.parentNode.firstChild;
            while (node) {
                if (node != self && node.nodeType === Node.ELEMENT_NODE) {
                    if (selector == "") {
                        if (!!filter) {
                            filter(node, i);
                        }
                        result.push(node);
                    }
                    else {
                        t$.each(document.querySelectorAll(selector), function (n, i) {
                            if (node === n) {
                                if (!!filter) {
                                    filter(node, i);
                                }
                                result.push(node);
                            }
                        });
                    }
                }
                node = node.nextElementSibling || node.nextSibling;
            }
            return result;
        };
        t$.getElement = function (selector) {
            return document.querySelector(selector);
        };
        t$.prototype.$child = function (selector) {
            this.__elements__ = this.__elements__.querySelector(selector);
            return this;
        };
        t$.getElements = function (selector, callback) {
            if (callback === void 0) { callback = null; }
            if (!!callback && typeof callback === "function") {
                t$.each(document.querySelectorAll(selector), callback);
            }
            return document.querySelectorAll(selector);
        };
        t$.sortBy = function (values, key, desc) {
            return values.sort(function (x, y) {
                var condition = x[key] < y[key];
                if (desc) {
                    condition = x[key] > y[key];
                }
                return (condition) ? -1 : (x[key] === y[key]) ? 0 : 1;
            });
        };
        t$.sumBy = function (values, key) {
            var sum = 0;
            t$.each(values, function (item) {
                if (item[key]) {
                    sum += parseFloat(item[key]);
                }
            });
            return sum;
        };
        t$.groupOnceBy = function (xs, key, callback) {
            var values = xs.reduce(function (rv, x) {
                (rv[x[key]] = rv[x[key]] || []).push(x);
                return rv;
            }, {});
            callback(values);
            return values;
        };
        ;
        t$.groupBy = function (values, keys, process) {
            if (process === void 0) { process = null; }
            var grouped = {};
            this.each(values, function (a) {
                keys.reduce(function (o, k, i) {
                    o[a[k]] = o[a[k]] || (i + 1 === keys.length ? [] : {});
                    return o[a[k]];
                }, grouped).push(a);
            });
            if (!!process && typeof process === "function") {
                process(grouped);
            }
            return grouped;
        };
        t$.each = function (values, process) {
            var n = values.length, i = 0;
            while (n > 0) {
                process(values[i], i);
                i++;
                n--;
            }
        };
        t$.paginate = function (values, maxPageCount, page_index) {
            return values.slice((page_index - 1) * maxPageCount, page_index * maxPageCount);
        };
        t$.isValidObject = function (value) {
            return !!value && Object.getOwnPropertyNames(value).length > 0;
        };
        t$.isValidArray = function (values) {
            return !!values && values.length > 0;
        };
        t$.fetch = function (option) {
            var xhr = new XMLHttpRequest();
            xhr.open(!option.method ? "GET" : option.method, option.url, option.async === undefined ? true : option.async);
            xhr.onreadystatechange = function () {
                if (this.readyState === 4) {
                    if (this.status === 200) {
                        if (!!option.success && typeof option.success === "function") {
                            option.success.call(this, JSON.parse(this.responseText));
                        }
                    }
                    else {
                        if (!!option.error && typeof option.error === "function") {
                            option.error.call(this, this.status, this.statusText);
                        }
                    }
                }
            };
            xhr.setRequestHeader("Content-Type", option.content_type);
            xhr.send(t$.isValidObject(option.data) ? JSON.stringify(option.data) : null);
            return xhr;
        };
        return t$;
    }());
    Kernel.t$ = t$;
})(Kernel = {});
