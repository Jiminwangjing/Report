function CoreItemMaster(config) {
    let __config = {
        master: {
            keyField: "",
            value: {}
        },
        detail: {
            keyField: "",
            name: "",
            values: new Array()
        },
        singleDetail: {
            keyField: "",
            name: "",
            value: {}
        },
        properties: {
            keyField: "",
            name: "",
            values: new Array()
        }
    };
    if (isValidJson(config)) {
        __config = $.extend(true, __config, config);
    }

    this.submitData = function (url, params, success) {
        if (isValidArray(params)) {
            let __data = {};
            __data[params[0]] = __config.master.value;
            __data[params[1]] = __config.detail.values;
            __data[params[2]] = JSON.stringify(__config.properties.values);
            $.ajax({
                url: url,
                type: "POST",
                data: __data,
                success: success
            });
        }
        return this;
    }

    this.submitSingleData = function (url, params, success) {
        if (isValidArray(params)) {
            let __data = {};
            __data[params[0]] = JSON.stringify(__config.master.value);
            __data[params[1]] = JSON.stringify(__config.singleDetail.value);
            $.ajax({
                url,
                type: "POST",
                data: __data,
                success
            });
        }
        return this;
    }
    this.updateData = function (url, params, success) {
        if (isValidArray(params)) {
            let __data = {};
            __data[params[0]] = JSON.stringify(__config.master.value);
            __data[params[1]] = JSON.stringify(__config.detail.values);
            __data[params[2]] = JSON.stringify(__config.properties.values);
            $.ajax({
                url: url,
                type: "POST",
                data: __data,
                success: success
            });
        }
        return this;
    }
    this.updateGlAccName = function (key, prop, value) {
        $.each(__config.glAccName.values, function (index, item) {
            if (key == index) {
                item[prop] = value
            }
        })
        return this;
    }
    this.updateMaster = function (prop, value) {
        __config.master.value[prop] = value;
        return this;
    }

    this.updateDetail = function (key, prop, value) {
        $.grep(__config.detail.values, function (item) {
            if (item[__config.detail.keyField] == key) {
                item[prop] = value;
            }
        });
        return this;
    }
    this.updateSingleDetail = function (prop, value) {
        __config.singleDetail.value[prop] = value;
        return this;
    }

    this.fallbackInfo = function (callback) {
        if (typeof callback === "function") {
            callback.call(this, __config);
        }
        return __config;
    }
    this.updateProperty = function(key, prop, value){
        $.grep(__config.properties.values, function (item) {
            if (item[__config.properties.keyField] == key) {
                item[prop] = value;
            }
        });
        return this;
    }
    function isValidJson(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
}