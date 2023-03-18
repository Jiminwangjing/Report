; (function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
        typeof define === 'function' && define.amd ? define(factory) : global.Carousel = factory;
})(window, function Carousel(config) {
    if (!(this instanceof Carousel)) {
        return new Carousel(config);
    }
    const __this = this;
    var __setting = {
        container: "",
        attribute: {
            class: {
                meta: "kedi-carousel full-screen",
                prevButton: "carousel-prev fas fa-chevron-left fa-lg",
                nextButton: "carousel-next fas fa-chevron-right fa-lg",
                item: "carousel-item"
            }
        },
        autoSlide: {
            timer: undefined,
            enabled: true,
            forward: false,
            interval: 7000
        },
        display: {
            timer: undefined,
            offsetInterval: 5000,
            currentTimeout: 30000,
            defaultTimeout: 30000
        }
    },
        __activeIndex = 1;
    __setting = Object.assign(__setting, config);
    this.meta = {
        template: createSelfTemplate(),
        proto: __this,
        itemNodes: []
    }
    function createSelfTemplate() {
        let template = document.createElement("div");
        template.setAttribute("class", __setting.attribute.class.meta);
        return template;
    }

    function setAutoSlide(__itemNodes = []) {
        if (__setting.autoSlide.enabled) {
            if (__setting.autoSlide.timer != undefined) { clearInterval(__setting.autoSlide.timer); }
            __setting.autoSlide.timer = setInterval(function () {
                if (__setting.autoSlide.forward) {
                    __this.meta.template.classList.remove("backward");
                    skipActiveItem(1, __itemNodes);
                } else {
                    __this.meta.template.classList.add("backward");
                    skipActiveItem(-1, __itemNodes);
                }
            }, __setting.autoSlide.interval);
        }
    }

    function skipActiveItem(offsetIndex, itemNodes) {
        displayActiveItem(__activeIndex += offsetIndex, itemNodes);
        setAutoSlide(itemNodes);
    }

    function displayActiveItem(n, itemNodes) {
        if (n < 1) { __activeIndex = itemNodes.length; }
        if (n > itemNodes.length) { __activeIndex = 1; }
        for (let i = 0; i < itemNodes.length; i++) {
            itemNodes[i].classList.remove("active");
        }
        itemNodes[__activeIndex - 1].classList.add("active");
    }

    this.build = function (imageNames = []) {
        let __container = document.querySelector(__setting.container),
            __template = __this.meta.template;

        __template.classList.remove("collapsed");
        __template.textContent = "";
        let prevButton = document.createElement("button"),
            nextButton = document.createElement("button");
        prevButton.setAttribute("class", __setting.attribute.class.prevButton);
        nextButton.setAttribute("class", __setting.attribute.class.nextButton);
        let __itemNodes = __this.meta.itemNodes = [];
        for (let i = 0; i < imageNames.length; i++) {
            let item = document.createElement("div"),
                img = document.createElement("img");
            item.setAttribute("class", __setting.attribute.class.item);
            if (i <= 0) {
                item.classList.add("active");
            }
            img.setAttribute("src", imageNames[i]);
            item.appendChild(img);
            __template.appendChild(item);
            __template.appendChild(prevButton);
            __template.appendChild(nextButton);
            __itemNodes.push(item);
        }

        if (__container != undefined) {
            __container.appendChild(__template);
        }

        setAutoSlide(__itemNodes);
        prevButton.addEventListener("click", function (e) {
            __template.classList.add("backward");
            skipActiveItem(-1, __itemNodes);
        });

        nextButton.addEventListener("click", function (e) {
            __template.classList.remove("backward");
            skipActiveItem(1, __itemNodes);
        });
    }

    //Default 30 seconds
    this.setDisplayTimeout = function (timeout = 30000) {
        if (typeof timeout === "number") {
            __setting.display.currentTimeout = timeout;
        }

        if (__setting.display.currentTimeout <= 0) {
            __setting.display.currentTimeout = __setting.display.defaultTimeout;
        }

        if (__setting.display.offsetInterval > __setting.display.currentTimeout) {
            __setting.display.offsetInterval = __setting.display.currentTimeout;
        }

        clearInterval(__setting.display.timer);
        __setting.display.timer = setInterval(function () {
            __setting.display.currentTimeout -= __setting.display.offsetInterval;
            if (__setting.display.currentTimeout <= 0) {
                __setting.display.currentTimeout = 0;
                clearInterval(__setting.display.timer);
                __this.show();
                setAutoSlide(__this.meta.itemNodes);
            }
        }, __setting.display.offsetInterval);
    }

    this.show = function () {
        let __template = __this.meta.template;
        if (__template == undefined) { return; }
        setTimeout(function () {
            __template.classList.remove("collapsed");
        }, 0);
    }

    this.hide = function () {
        let __template = __this.meta.template;
        if (__template == undefined) { return; }
        setTimeout(function () {
            __template.classList.add("collapsed");
        }, 0);
    }
});