function CoreItemMaster(config) {
    let __config = {
        master: {
            keyField: "",
            value: {}
        }
    };
    if (isValidJson(config)) {
        __config = $.extend(true, __config, config);
    }
    this.submitData = function (url, success) {
        $.post(url, __config.master.value, success);
        return this;
    }
    this.updateMaster = function (prop, value) {
        __config.master.value[prop] = value;
        return this;
    }

    this.fallbackInfo = function (callback) {
        if (typeof callback === "function") {
            callback.call(this, __config);
        }
        return __config;
    }

    function isValidJson(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
}