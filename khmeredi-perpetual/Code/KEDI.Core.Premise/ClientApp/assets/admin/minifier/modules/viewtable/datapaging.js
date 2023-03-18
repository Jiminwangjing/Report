
; (function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
        typeof define === 'function' && define.amd ? define(factory) : global.DataPaging = factory;
})(window, (function DataPaging(config) {
    if (!(this instanceof DataPaging)) {
        return new DataPaging(config);
    }
    const __this = this;
    var __template = {
        ul: document.createElement("ul"),
        li: document.createElement("li")
    };
    var __setting = {
        container: undefined,
        keyField: "",
        pageSize: 15,
        startIndexSize: 4,
        align: "right",
        style: {
            class: "ck-pagination",
            mode: "circle",
            border: 1, // true: positive, false: 0 or negative
            font: {
                size: "1em",
                weight: "bold",
                family: "monospace"
            }
        },
        onQueryPage: function (fallbackEvent) { }
    };

    var __meta = {
        proto: this,
        template: __template.ul,
        dataset: new Array()
    }, __pageStore = {
        activeIndex: 1,
        nodes: new Array(),
        numbers: new Array()
    };
    configure(config);

    //******** Initialization section *//
    this.template = undefined;
    this.configure = configure;
    function configure(setting) {
        if (isValidJson(setting)) {
            __setting = $.extend(true, __setting, setting);
        }
    }

    this.yield = function () {
        return __meta.dataset;
    }

    this.clearData = function () {
        __meta.dataset = new Array();
    }

    this.appendTo = function (container, clearContent = true) {
        if (container) {
            if (clearContent) {
                $(container).children().remove();
            }
            $(container).append(__meta.template);
        }
    };

    this.bindTo = function (container) {
        if (container) {
            $(container).html(__meta.template);
        }
    };

    function countMaxPage() {
        var opt = __setting;
        var maxPageCount = Math.floor(__meta.dataset.length / parseFloat(opt.pageSize));
        if (__meta.dataset.length % parseFloat(opt.pageSize) !== 0) {
            maxPageCount = maxPageCount + 1;
        }
        return maxPageCount;
    }

    this.find = function (value, dataset) {
        var _dataset = __meta.dataset;
        if (isValidArray(dataset)) {
            _dataset = dataset;
        }
        return $.grep(_dataset, function (_item) {
            if (isValidJson(value)) {
                return _item[__setting.keyField] == value[__setting.keyField];
            }
            else {
                return _item[__setting.keyField] == value;
            }
        })[0];
    };

    this.findPage = function (value) {
        var page = {
            index: 1,
            dataset: this.filterByIndex(1)
        };

        for (let i = 1; i <= countMaxPage(); i++) {
            var _dataset = __meta.proto.filterByIndex(i);
            var item = __this.find(value, _dataset);
            if (isValidJson(item)) {
                page.index = i;
                page.dataset = _dataset;
                break;
            }
        }

        return page;
    };

    this.filterByIndex = function (index) {
        return paginate(__meta.dataset, __setting.pageSize, index);
    };

    this.isOnActivePage = function (value) {
        var page = this.findPage(value);
        return page["item"] !== undefined && getActiveIndex() == page["index"];
    };

    this.addJson = function (item, callback) {
        __meta.dataset.push(item);
        var page = {
            index: countMaxPage(),
            data: this.filterByIndex(countMaxPage())
        };

        if (!this.isOnActivePage(item)) {
            this.navigate(page.index);
        }

        if (typeof callback === "function") {
            callback.call(this, page);
        }
        return this;
    };

    this.updateJson = function (item, callback) {
        $.grep(__meta.dataset, function (json) {
            if (item[__setting.keyField] === json[__setting.keyField]) {
                $.each(item, function (k, v) {
                    json[k] = item[k];
                });
            }
        });

        var page = this.findPage(item);
        if (!this.isOnActivePage(item)) {
            this.navigate(page.index);
        }

        if (typeof callback === "function") {
            callback.call(this, page);
        }
        return this;
    };

    this.removeJson = function (item) {
        //item can be key or valid object
        var key = item
        if (isValidJson(item)) {
            key = item[__setting.keyField];
        }

        __meta.dataset = $.grep(__meta.dataset, function (json) {
            return json[__setting.keyField] != key;
        });
    }

    this.load = function (items, item, callback) {
        if (Array.isArray(items)) {
            __meta.dataset = items;
        }

        var page = this.findPage(item);
        return this.navigate(page.index, items, callback);
    }

    this.navigate = function (index, items, callback) {
        if (isValidArray(items)) {
            __meta.dataset = items;
        }

        if (typeof parseInt(index) === "number") {
            var pageIndex = index;
            if (index > countMaxPage()) {
                pageIndex = countMaxPage();
            }
            setActiveIndex(pageIndex);
        }
        return this.render(items, callback);
    };

    function setActiveIndex(index) {
        __pageStore["activeIndex"] = index;
    }

    function getActiveIndex() {
        return parseInt(__pageStore["activeIndex"]);
    }

    function getStartIndex() {
        return parseInt(__setting.startIndexSize);
    }

    function getDefaultIndexes() {
        var c = 2, indexes = new Array(), defaultSize = getStartIndex();
        if (getStartIndex() > countMaxPage()) {
            defaultSize = countMaxPage();
        }
        while (c <= defaultSize) {
            indexes.push(c++);
        }
        return indexes;
    }

    function defineStyle() {
        var style = __setting.style;
        $(__meta.template).addClass("ck-pagination").addClass(style["class"]);
        switch (style.mode) {
            case "square":
                $(__meta.template).addClass("square");
                break;
            case "circle":
                $(__meta.template).addClass("circle");
                break;
        }
        __meta.template.style.fontFamily = style.font.family;
        __meta.template.style.fontFamily = style.font.family;
        __meta.template.style.fontSize = style.font.size;
        __meta.template.style.fontWeight = style.font.weight;

        if (style.border === true || style.border > 0) {
            $(__meta.template).addClass("border");
        }
    }

    function buildFirstPage() {
        let firstWalk = __template.li.cloneNode(true);
        firstWalk.textContent = 1;
        firstWalk.setAttribute("name", "page-first");
        $(firstWalk).on("click", function (e) {
            setActiveIndex(1);
            if (getStartIndex() < countMaxPage()) {
                offsetPrevious(countMaxPage());
            }

            $(firstWalk).addClass("active");
            $.each(firstWalk.parentNode.childNodes, function (i, node) {
                if (node !== firstWalk) {
                    $(node).removeClass("active");
                }
            });

            if (parseInt(firstWalk.textContent) === getActiveIndex()) {
                __setting.onQueryPage.call(__meta.proto, fallbackEvent());
            }

        });
        if (getActiveIndex() === parseInt(firstWalk.textContent)) {
            $(firstWalk).addClass("active");
        }
        __meta.template.appendChild(firstWalk);
    }

    function buildPreviousPage() {
        var previousWalk = __template.li.cloneNode(true);
        previousWalk.setAttribute("name", "page-previous");
        previousWalk.innerHTML = "&lt;&lt;";
        $(previousWalk).on("click", onPrevious);
        function onPrevious(e) {
            if (getActiveIndex() > 1) {
                __pageStore["activeIndex"]--;
                if (getActiveIndex() <= countMaxPage() - getStartIndex()) {
                    offsetPrevious(1);
                }

                $.each(__meta.template.childNodes, function (i, node) {
                    if (parseInt(node.textContent) === getActiveIndex()) {
                        $(node).addClass("active");
                    }
                    else {
                        $(node).removeClass("active");
                    }
                });
                __setting.onQueryPage.call(__meta.proto, fallbackEvent());
            }
        }
        __meta.template.appendChild(previousWalk);
    }

    function buildNextPage() {
        var nextWalk = __template.li.cloneNode(true);
        nextWalk.setAttribute("name", "page-next");
        nextWalk.innerHTML = "&gt;&gt;";
        $(nextWalk).on("click", onNext);
        function onNext(e) {
            if (getActiveIndex() < countMaxPage()) {
                __pageStore["activeIndex"]++;
                if (getActiveIndex() > getStartIndex()) {
                    offsetNext(1);
                }
                $.each(__meta.template.childNodes, function (i, node) {
                    if (parseInt(node.textContent) === getActiveIndex()) {
                        $(node).addClass("active");
                    }
                    else {
                        $(node).removeClass("active");
                    }
                });
                __setting.onQueryPage.call(__meta.proto, fallbackEvent());
            }

        }
        __meta.template.appendChild(nextWalk);
    }

    function buildLastPage() {
        var lastWalk = __template.li.cloneNode(true);
        lastWalk.setAttribute("name", "page-last");
        lastWalk.textContent = countMaxPage().toString();
        $(lastWalk).on("click", function (e) {
            setActiveIndex(countMaxPage());
            offsetNext(countMaxPage());
            $(lastWalk).addClass("active");
            $.each(lastWalk.parentNode.childNodes, function (i, node) {
                if (node !== lastWalk) {
                    $(node).removeClass("active");
                }
            });
            __setting.onQueryPage.call(__meta.proto, fallbackEvent());
        });


        if (getActiveIndex() >= countMaxPage() - getStartIndex()) {
            offsetNext(countMaxPage());
            if (getActiveIndex() === parseInt(lastWalk.textContent)) {
                $(lastWalk).addClass("active");
            }
        }
        __meta.template.appendChild(lastWalk);
    }

    function buildPreviousSkip() {
        var skipPrevNode = __template.li.cloneNode(true);
        skipPrevNode.setAttribute("name", "page-skip-previous");
        skipPrevNode.className = "hidden";
        skipPrevNode.textContent = "...";
        $(skipPrevNode).on("click", onSkipPrev);
        function onSkipPrev(e) {
            offsetPrevious(getStartIndex() - 1);
            var activeNode = skipPrevNode.nextElementSibling;
            $(activeNode).addClass("active");
            $.each(activeNode.parentNode.childNodes, function (i, node) {
                if (node !== activeNode) {
                    $(node).removeClass("active");
                }
            });
            setActiveIndex(parseInt(activeNode.textContent));
            __setting.onQueryPage.call(__meta.proto, fallbackEvent());
        }
        __meta.template.appendChild(skipPrevNode);
    }

    function buildNextSkip() {
        var skipNextNode = __template.li.cloneNode(true);
        skipNextNode.setAttribute("name", "page-skip-next");
        skipNextNode.textContent = "...";
        $(skipNextNode).on("click", onSkipNext);
        function onSkipNext(e) {
            offsetNext(getStartIndex() - 1);
            var activeNode = skipNextNode.previousElementSibling;
            $(activeNode).addClass("active");
            $.each(activeNode.parentNode.childNodes, function (i, node) {
                if (node !== activeNode) {
                    $(node).removeClass("active");
                }
            });
            setActiveIndex(parseInt(activeNode.textContent));
            __setting.onQueryPage.call(__meta.proto, fallbackEvent());
        }

        __meta.template.appendChild(skipNextNode);
    }

    function buildIndexPage() {
        __pageStore["numbers"] = new Array();
        __pageStore["nodes"] = new Array();

        $.each(getDefaultIndexes(), function (i, m) {
            var indexNode = __template.li.cloneNode(true);
            indexNode.setAttribute("name", "page-index");
            indexNode.textContent = m.toString();
            __pageStore["numbers"].push(m);
            __pageStore["nodes"].push(indexNode);
            $(indexNode).on("click", onIndex);
            if (parseInt(indexNode.textContent) === getActiveIndex()) {
                $(indexNode).addClass("active");
            }
            __meta.template.appendChild(indexNode);
        });
        if (getActiveIndex() > getStartIndex()) {
            offsetNext(getActiveIndex() - getStartIndex());
        } else {
            offsetPrevious(getActiveIndex() + getStartIndex());
        }
    }

    function onIndex(e) {
        setActiveIndex(parseInt(e.target.textContent));
        $(e.target).addClass("active");
        $.each(e.target.parentNode.childNodes, function (i, node) {
            if (node !== e.target) {
                $(node).removeClass("active");
            }
        });
        if (getActiveIndex() === parseInt(e.target.textContent)) {
            __setting.onQueryPage.call(__meta.proto, fallbackEvent());
        }
    }

    this.render = function (values, callback) {
        __meta.template = __template.ul.cloneNode(true);
        if (isValidArray(values)) {
            __meta.dataset = values;
        }

        if (typeof callback === "function") {
            __setting.onQueryPage = callback;
        }

        buildPreviousPage();
        buildFirstPage();
        buildPreviousSkip();
        buildIndexPage();
        if (countMaxPage() > getStartIndex() + 1) {
            buildNextSkip();
        }

        if (getStartIndex() < countMaxPage()) {
            buildLastPage();
        }

        buildNextPage();
        defineStyle();
        this.template = __meta.template;
        if (__setting.container) {
            if (typeof __setting.container === "string") {
                $(__setting.container).html(__meta.template);
            }
            else {
                __setting.container.textContent = __meta.template;
            }
        }

        __setting.onQueryPage.call(this, fallbackEvent());
        return this;
    };

    function offsetPrevious(offsetSize) {
        offsetNext(-offsetSize);
    }

    function offsetNext(offsetSize) {
        var maxPageCount = countMaxPage(), indexNumbers = __pageStore["numbers"], indexNodes = __pageStore["nodes"], n = indexNodes.length;
        if (getStartIndex() < countMaxPage()) {
            $(__meta.template).children("li[name='page-skip-previous']").removeClass("hidden");
            $(__meta.template).children("li[name='page-skip-next']").removeClass("hidden");

            $.each(indexNodes, function (i, elem) {
                indexNumbers[i] += offsetSize;
                if (indexNumbers[0] <= 2) {
                    indexNumbers[i] = i + 2;
                    $(__meta.template).children("li[name='page-skip-previous']").addClass("hidden");
                }
                if (indexNumbers[i] >= maxPageCount - n + i) {
                    indexNumbers[i] = maxPageCount - n + i;
                    $(__meta.template).children("li[name='page-skip-next']").addClass("hidden");
                }

                elem.textContent = indexNumbers[i];
                if (parseInt(elem.textContent) === getActiveIndex()) {
                    $(elem).addClass("active").siblings().removeClass("active");
                }
            });
        }
    }

    function fallbackEvent() {
        return {
            meta: __meta,
            setting: __setting,
            dataset: paginate(__meta.dataset, __setting.pageSize, getActiveIndex()),
            page: {
                firstIndex: 1,
                lastIndex: countMaxPage(),
                activeIndex: getActiveIndex()
            }
        };
    }

    this.onQueryPage = function (callback) {
        __setting.onQueryPage = callback;
        return this;
    };

    function paginate(values, maxPageCount, pageIndex) {
        return values.slice((pageIndex - 1) * maxPageCount, pageIndex * maxPageCount);
    }

    function isEmpty(value) {
        return value === undefined || value == null || value === "";
    }

    //Check if json object is valid.
    function isValidJson(json) {
        return !isEmpty(json) && json.constructor === Object && Object.keys(json).length > 0;
    };

    //Check if value is valid array.
    function isValidArray(array) {
        return Array.isArray(array) && array.length > 0;
    }
}));


