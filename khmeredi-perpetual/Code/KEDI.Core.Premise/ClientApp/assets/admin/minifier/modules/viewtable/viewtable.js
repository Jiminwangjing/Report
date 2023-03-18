
; (function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
        typeof define === 'function' && define.amd ? define(factory) : global.ViewTable = factory;
})(window, (function ViewTable(config) {
    'use strict'
    if (!(this instanceof ViewTable)) {
        return new ViewTable(config);
    }
    const __this = this;
    const __template = {
        table: document.createElement("table"),
        tbody: document.createElement("tbody"),
        tfoot: document.createElement("tfoot"),
        tr: document.createElement("tr"),
        th: document.createElement("th"),
        td: document.createElement("td"),
        input: document.createElement("input"),
        select: document.createElement("select"),
        optgroup: document.createElement("optgroup"),
        option: document.createElement("option"),
        textarea: document.createElement("textarea"),
        label: document.createElement("label"),
        div: document.createElement("div"),
        span: document.createElement("span"),
        i: document.createElement("i")
    }, __setting = {
        container: "",
        selector: "",
        model: {},
        dynamicCol: {
            afterAction: false,
            beforeAction: false,
            headerContainer: "",
        },
        dataSynced: false,
        indexed: false,
        grouping: {
            dataFields: []
        },
        paging: {
            enabled: true
        },
        id: Date.now(),
        keyField: "",
        visibleFields: [],
        attribute: {
            key: "data-key",
            field: "data-field"
        },
        event: {
            onColumnChange: function () { },
            onRowChange: function () { }
        },
        row: {
            highlightOnUpdate: true,
            on: { "click": function (e) { } }
        },
        scaleColumns: [
            // {
            //     name: "", 
            //     offset: 1,    
            //     onScaleUp: {"": function(e){}},
            //     onScaleDown: {"": function(e){}}
            // }
        ],
        columns: [
            // {
            //     name: "",
            //     template: "",
            //     dataType: "number", //["number", "string", "boolean", "date", "time"]
            //     dataFormat: { decimal: ".", "separator": ",", fixed: 3 }, //dataFormat: "MM-DD-YYYY" for date
            //     title: {
            //          enabled: false,
            //          text: ""
            //     },
            //     on: {"": function(e, evt){}},
            //     insertion: ""//["prepend", "append", "replace"],
            //     dynamicCol: true/false,
            //     valueDynamicProp: "ValueName",
            // }
        ],
        actions: [
            // {
            //     template: "<i class='fas fa-edit'></i>",
            //     on: {"click": function(e){}}
            // }
        ]
    };

    const __meta = {
        dataset: /*Array.isArray(dataset)? dataset : */new Array(),
        container: undefined,
        template: undefined,
        proto: this
    }, __activeRow = {};

    //******** Initialization section *// 
    const __paging = createPagination();
    configure(config);
    //******** ...................... *// 

    function configure(setting, reset = false) {
        $.extend(true, __setting, setting);
        if (isEmpty(__meta.template)) {
            __meta.template = __template.table;
        }

        if (isValidJson(setting)) {
            if (reset) {
                __setting.model = undefined;
                __setting.columns = [];
                __setting.scaleColumns = [];
                ViewTable(config);
            }

            if (__setting.paging.enabled) {
                __paging.configure(__setting.paging);
            }

            if ($(__setting.selector).length > 0) {
                __meta.template = $(__setting.selector)[0];
            }

            if ($(__setting.container).length > 0) {
                __meta.container = $(__setting.container)[0];
                __meta.template = $(__meta.container).find("table")[0];
                if (__meta.template === undefined) {
                    __meta.template = __template.table.cloneNode();
                }
            }

            if ($(__setting.selector).length > 0) {
                __meta.container = __meta.template.parentNode;
            }
        }
    }
    this.configure = configure;

    this.getSetting = function () {
        return __setting;
    }

    this.appendTo = function (container) {
        if (container) {
            $(container).append(__meta.template);
        }
        return this;
    }

    this.bindRows = function (values, value, onBinding) {
        if (Array.isArray(values)) {
            if (values.length > 0) {
                let item = isEmpty(value) ? values[0] : value;
                if (__setting.paging.enabled) {
                    __setting.paging.keyField = __setting.keyField;
                    __paging.configure(__setting.paging);
                    __paging.load(values, item, function (event) {
                        __this.clearPageRows();
                        let n = __setting.paging.pageSize * (event.page.activeIndex - 1) + 1;
                        for (let i = 0; i < event.dataset.length; i++) {
                            __this.addRow(event.dataset[i], onBinding, n++);
                        }
                        __meta.dataset = values;
                    });
                    setContainer(undefined, __meta.template, __paging.template);
                } else {
                    __this.clearRows();
                    for (let i = 0; i < values.length; i++) {
                        __this.addRow(values[i], onBinding, i + 1);
                    }
                    setContainer(undefined, __meta.template, undefined);
                }
            } else {
                __this.clearRows();
            }
        }
        return __this;
    };

    function createPagination() {
        return DataPaging(__setting.paging);
    }

    this.hideColumns = function (key, fieldNames, hidden = true) {
        if (isValidArray(fieldNames)) {
            if (isEmpty(key)) {
                if (isValidArray(__meta.dataset)) {
                    for (let c = 0; c < __meta.dataset.length; c++) {
                        let _key = __meta.dataset[c][__setting.keyField];
                        __this.hideColumns(_key, fieldNames, hidden);
                    }
                }
            }

            for (let i = 0; i < fieldNames.length; i++) {
                let col = __this.getColumn(key, fieldNames[i]);
                if (hidden) {
                    $(col).css("visibility", "hidden");
                } else {
                    $(col).css("visibility", "visible");
                }
            }
        }
    }

    this.disableTarget = function (target, disabled = true) {
        if (disabled) {
            $(target).css("pointer-events", "none");
        } else {
            $(target).css("pointer-events", "auto");
        }
    }

    this.disableColumns = function (key, fieldNames, disabled = true) {
        if (isValidArray(fieldNames)) {
            if (isEmpty(key)) {
                if (isValidArray(__meta.dataset)) {
                    for (let c = 0; c < __meta.dataset.length; c++) {
                        let _key = __meta.dataset[c][__setting.keyField];
                        __this.disableColumns(_key, fieldNames, disabled);
                    }
                }
            }

            for (let i = 0; i < fieldNames.length; i++) {
                let col = __this.getColumn(key, fieldNames[i]);
                if (disabled) {
                    $(col).css("pointer-events", "none");
                } else {
                    $(col).css("pointer-events", "auto");
                }
            }
        }
    }

    this.disableRow = function (key, enabled = true) {
        let row = __this.getRow(key);
        if (enabled) {
            $(row).css("pointer-events", "none");
        } else {
            $(row).css("pointer-events", "auto");
        }
    }

    function setContainer(top, middle, bottom) {
        let $gridBox = $(__meta.template).parents(".grid-box");
        if ($gridBox.length > 0) {
            __meta.container = $gridBox[0];
        }

        $(__meta.container).addClass("grid-box");
        let $header = $(clone("div")).addClass("box-header"),
            $body = $(clone("div")).addClass("box-body"),
            $footer = $(clone("div")).addClass("box-footer");
        if (top) { $header.html(top); }
        if (middle) { $body.html(middle).css("min-height", $(middle).height()); }
        if (bottom) {
            $footer.html(bottom);
        }

        if ($(__meta.container).children().length > 0) {
            $(__meta.container).find(".box-header").replaceWith($header);
            $(__meta.container).find(".box-body").replaceWith($body);
            $(__meta.container).find(".box-footer").replaceWith($footer);
        }

        $(__meta.container).append($header).append($body).append($footer);
    }

    this.createColumn = function (json, fieldName, value, item = {}) {
        let $col = $(clone("td"));
        $col.append(value).attr(__setting.attribute.field, fieldName).prop("hidden", true);
        defineVisibleFields(json, fieldName, $col);
        if (isValidJson(__setting.model)) {
            var formControl = clone("input");
            formControl.setAttribute("type", "hidden");
            formControl.value = value;
            concatFieldName(__setting.model, formControl, fieldName, __meta.dataset.indexOf(json));
            $col.append(formControl);
        }
        if (__setting.scaleColumns.length > 0) {
            for (let c = 0; c < __setting.scaleColumns.length; c++) {
                if (fieldName.toLowerCase() === __setting.scaleColumns[c].name.toLowerCase()) {
                    defineScaleColumn(json, __setting.scaleColumns[c], $col);
                }
            }
        }
        if (__setting.columns.length > 0) {
            for (let k = 0; k < __setting.columns.length; k++) {
                let _name = __setting.columns[k].name.toLowerCase(),
                    _fieldName = fieldName.toLowerCase();
                if (_name === _fieldName || _name.split(".")[0] === _fieldName) {
                    defineTitle(__setting.columns[k], value, $col);

                    if (_name === _fieldName) {
                        insertTemplate(json, __setting.columns[k], value, $col);
                    }

                    if (_name.split(".")[0] === _fieldName) {
                        defineNestedField(json, __setting.columns[k], fieldName, $col);
                    }
                }
            }
        }

        let dynamicValue = __setting.columns.filter(i => i.dynamicCol == true)[0];
        if (isValidJson(dynamicValue)) {
            for (var i in item[dynamicValue.name]) {
                if (fieldName.toLowerCase() === i.toLowerCase()) {
                    $col.prop("hidden", false);
                }
            }
            if (fieldName.toLowerCase() === dynamicValue.name.toLowerCase()) {
                $col.prop("hidden", true);
            }
        }
        __setting.event.onColumnChange.call(this, fallbackColumn(json, fieldName, json[fieldName], $col[0], $col[0]));
        return $col[0];
    }

    function defineNestedField(json, field, fieldName, $col) {
        if (isValidJson(json[fieldName]) && field.name.indexOf(".") > 0) {
            $col.html(json[fieldName][field.name.split(".")[1]]);
            $col.attr(__setting.attribute.field, field.name);
            insertTemplate(json, field, json[fieldName][field.name.split(".")[1]], $col);
        }
    }

    function defineReadonly(json, propName, readonly = true) {
        Object.defineProperty(json, propName, {
            writable: !readonly
        });
    }

    function defineIndex(row, index) {
        if (__setting.indexed) {
            if ($(row).length > 0) {
                let td = clone("td");
                $(td).text(index);
                $(row).prepend(td);
            }
        }
    }

    function defineSelectList(items, select) {
        let _select = select ? select : clone("select", true);
        if (isValidArray(items)) {
            $.each(items, function (i, item) {
                let option = $(clone("option")).prop("selected", item["Selected"])
                    .prop("disabled", item["Disabled"])
                    .val(item["Value"]).text(item["Text"]);
                $(_select).append(option);
            });
        }
        return $(_select)[0];
    }

    function defineVisibleFields(json, fieldName, col) {
        if (__setting.visibleFields.length === 0) {
            __setting.visibleFields = Object.getOwnPropertyNames(json);
        }

        for (let c = 0; c < __setting.visibleFields.length; c++) {
            if (typeof __setting.visibleFields[c] !== "function") {
                if (fieldName.toLowerCase() === __setting.visibleFields[c].toLowerCase()) {
                    $(col).prop("hidden", false);
                }
            }
        }
    }

    this.createRow = function (item, index) {
        let $row = $(clone("tr"));
        if (isValidJson(item)) {
            // defineReadonly(item, __setting.keyField, false);
            $.each(item, function (field, value) {
                let $col = __this.createColumn(item, field, value);
                defineSummary(field);
                $row.append($col).attr(__setting.attribute.key, item[__setting.keyField]);
            });

            defineIndex($row, index);
            if (__setting.dynamicCol.beforeAction) {
                createRowDynamicCol(item, $row)
            }
            defineActions(item, $row);
            if (__setting.dynamicCol.afterAction) {
                createRowDynamicCol(item, $row)
            }
            __setting.event.onRowChange.call(this, fallbackRow(item, $row[0]));
        }
        return $row[0];
    };

    function createRowDynamicCol(item, row) {
        let dynamicValue = __setting.columns.filter(i => i.dynamicCol == true)[0];
        if (isValidJson(dynamicValue)) {
            const _item = item[dynamicValue.name]
            if (isValidJson(_item)) {
                for (var i in _item) {
                    let $col = __this.createColumn(_item[i], i, _item[i][dynamicValue.valueDynamicProp], item);
                    row.append($col)
                }
            }
        }
    }
    this.yield = function (callback) {
        if (typeof callback === "function") {
            callback.call(this, __meta.dataset);
        } else {
            return __meta.dataset;
        }
    }

    this.reload = function (values, pageItem) {
        if (Array.isArray(values)) {
            __meta.dataset = values;
        }

        __this.bindRows(__meta.dataset, pageItem);
    }

    this.forEachRow = function (callback) {
        if (typeof callback === "function") {
            let rows = $(__meta.template).find("tr[" + __setting.attribute.key + "]");
            for (let i = 0; i < __meta.dataset.length; i++) {
                callback.call(this, i, __meta.dataset[i], rows[i]);
            }
        }
        return this;
    }

    this.addRow = function (json, callback, index) {
        let isExisting = __meta.dataset.some(function (item) {
            return item[__setting.keyField] == json[__setting.keyField];
        });

        if (!isExisting) {
            __meta.dataset.push(json);
            let row = clone("tr"), _index = __meta.template.rows.length;
            if (!isNaN(index)) {
                _index = index;
            }

            row = this.createRow(json, _index);
            if (typeof callback === "function") {
                callback.call(__this, fallbackRow(json, row));
            }
            $(__meta.template).append(row);
        }
        return this;
    };

    this.addRowAfter = function (json, relatedKey, callback) {
        let existedItem = this.find(json);
        if (existedItem) {
            throw Error("The item[key=" + json[__setting.keyField] + "] is already existing.");
        } else {
            let row = $(__meta.template).find("tr[" + __setting.attribute.key + "='" + relatedKey + "']")[0],
                $newRow = __this.createRow(json);
            if (typeof callback === "function") {
                callback.call(__this, fallbackRow(json, $newRow));
            }
            $(row).after($newRow);
            let itemIndex = __this.find(relatedKey);
            __meta.dataset.splice(itemIndex + 1, 0, json);
        }
    }

    this.updateRow = function (json, callback) {
        if (typeof highlight === "boolean" && highlight === true) {
            __setting.row.highlightOnUpdate = highlight;
        }

        let found = $.grep(__meta.dataset, function (item, i) {
            if (item[__setting.keyField] == json[__setting.keyField]) {
                $.each(item, function (k, v) {
                    item[k] = json[k];
                });
                let $row = $(__meta.template).find("tr[" + __setting.attribute.key + "='" + json[__setting.keyField] + "']"),
                    $newRow = __this.createRow(item, i + 1);

                if ($row.length <= 0) {
                    $(__meta.template).append($newRow);
                } else {
                    $row.replaceWith($newRow);
                }

                if (typeof callback === "function") {
                    __setting.onRowChange = callback;
                }

                if (__setting.row.highlightOnUpdate) {
                    __this.highlightRow(json);
                }

                return true;
            }
            return false;
        })[0];

        if (!found) {
            __this.addRow(json, callback);
            if (__setting.row.highlightOnUpdate) {
                __this.highlightRow(json);
            }
        }
    }

    this.updateColumn = function (key, fieldName, value) {
        let _item = this.find(key);
        if (_item) {
            let col = this.getColumn(key, fieldName);
            $(col).replaceWith(this.createColumn(_item, fieldName, value));
            _item[fieldName] = value;
        }
    }

    this.clearPageRows = function (templateOnly = false) {
        if (!templateOnly) {
            __meta.dataset = new Array();
        }

        $(__meta.template).find("tr[" + __setting.attribute.key + "]").remove();
    }

    this.clearRows = function (templateOnly = false) {
        this.clearPageRows(templateOnly);
        if (__setting.paging.enabled) {
            __paging.load(__meta.dataset);
        }
        return this;
    }

    this.removeRow = function (key, templateOnly = false) {
        let $row = $(__meta.template).find("tr[" + __setting.attribute.key + "='" + key + "']");
        $row.remove();
        if (!templateOnly) {
            __meta.dataset = $.grep(__meta.dataset, function (item, i) {
                return item[__setting.keyField] !== key;
            });

            if (__setting.paging.enabled) {
                __paging.removeJson(key);
            }
        }
        return this;
    };

    this.find = function (value) {
        if (isValidJson(value)) {
            return $.grep(__meta.dataset, function (item, i) {
                return item[__setting.keyField] == value[__setting.keyField];
            })[0];
        }
        return $.grep(__meta.dataset, function (item, i) {
            return item[__setting.keyField] == value;
        })[0];
    };

    this.highlightRow = function (value) {
        let _row = this.getRow(value);
        if (_row) {
            __activeRow.template = $(_row)[0];
            __activeRow.data = value;
            $(_row).addClass("active").siblings().removeClass("active");
            $(_row)[0].scrollIntoView({ behavior: "smooth", block: "nearest" });
        }
    }

    this.getRow = function (value) {
        if (isValidJson(value)) {
            return $(__meta.template).find("tr[" + __setting.attribute.key + "='" + value[__setting.keyField] + "']")[0];
        }
        return $(__meta.template).find("tr[" + __setting.attribute.key + "='" + value + "']")[0];
    }

    this.getColumn = function (key, fieldName) {
        var col = $(this.getRow(key)).find("td[" + __setting.attribute.field + "=" + fieldName + "]")[0];
        return col;
    }

    this.getActiveRow = function () {
        if (isValidJson(__activeRow)) {
            return __activeRow;
        }
    }

    //Custom events
    this.onColumnChange = function (callback) {
        __setting.event.onColumnChange = callback;
        return this;
    }

    this.onRowChange = function (callback) {
        __setting.event.onRowChange = callback;
        return this;
    }

    this.spaceBetweenWords = function (text) {
        return text.replace(/([a-z])([A-Z])/g, '$1 $2');
    }
    // header dynamic //
    this.createHeaderDynamic = function (values) {
        if (isValidJson(values)) {
            for (var prop in values) {
                if (__setting.dynamicCol.afterAction) {
                    $(__setting.dynamicCol.headerContainer).parents("tr").append(`<th id="${prop}">${__this.spaceBetweenWords(prop)}</th>`);
                }
                if (__setting.dynamicCol.beforeAction) {
                    $(__setting.dynamicCol.headerContainer).before(`<th id="${prop}">${__this.spaceBetweenWords(prop)}</th>`);
                }
            }
        }
    }
    this.clearHeaderDynamic = function (values) {
        if (isValidJson(values)) {
            for (var prop in values) {
                if (!isEmpty(__setting.selector)) {
                    $(__setting.selector).find(`#${prop}`).remove();
                }
                if (!isEmpty(__setting.container)) {
                    $(__setting.container).find(`#${prop}`).remove();
                }
            }
        }
    }
    function defineScaleColumn(data, scaleColumn, col) {
        if (isValidJson(scaleColumn)) {
            let value = modifyDataType(data[scaleColumn.name], scaleColumn);
            let $scaleBox = $(clone("div")).addClass("scale-box active"),
                $scaleDown = $(clone("i")).text("-").addClass("scale-down"),
                $scaleValue = $(clone("div")).addClass("scale").text(value),
                $scaleUp = $(clone("i")).text("+").addClass("scale-up");
            let downEvent = isValidJson(scaleColumn.onScaleDown) ? Object.keys(scaleColumn.onScaleDown)[0] : "",
                upEvent = isValidJson(scaleColumn.onScaleUp) ? Object.keys(scaleColumn.onScaleUp)[0] : "";

            $scaleDown.on(downEvent.toLowerCase(), function (e) {
                if (typeof data[scaleColumn.name] === "number") {
                    data[scaleColumn.name] -= (scaleColumn.offset) ? scaleColumn.offset : 1;
                    $scaleValue.text(data[scaleColumn.name]);
                }
                scaleColumn.onScaleDown[downEvent].call($scaleDown[0],
                    fallbackColumn(data, scaleColumn.name, data[scaleColumn.name], $scaleDown[0], $(col)[0]), e);
            });

            $scaleUp.on(upEvent.toLowerCase(), function (e) {
                if (typeof data[scaleColumn.name] === "number") {
                    data[scaleColumn.name] += (scaleColumn.offset) ? scaleColumn.offset : 1;
                    $scaleValue.text(data[scaleColumn.name]);
                }
                scaleColumn.onScaleUp[upEvent].call($scaleUp[0],
                    fallbackColumn(data, scaleColumn.name, data[scaleColumn.name], $scaleUp[0], $(col)[0]), e);
            });
            $scaleBox.append($scaleDown).append($scaleValue).append($scaleUp);
            $(col).html($scaleBox);
        }
    };

    function defineSummary(column) {
        if (isValidJson(column)) {
            if (column.summary === true) {
                for (let i = 0; i < __meta.dataset; i++) {
                    result += __meta.dataset[i][column.name];
                }
            }
        }
    }

    function defineActions(data, row) {
        let $col = $(clone("td"));
        for (let i = 0; i < __setting.actions.length; i++) {
            if (isValidJson(__setting.actions[i])) {
                let target = $(__setting.actions[i].template)[0],
                    event = Object.keys(__setting.actions[i].on)[0];
                $(target).on(event.toLowerCase(), function (e) {
                    __setting.actions[i].on[event].call(target, fallbackColumn(data, undefined, undefined, target, $col[0]), e);
                });
                $col.append(target);
                if (i < __setting.actions.length - 1) {
                    $col.append("&nbsp;✦&nbsp;");
                }
                $(row).append($col);
            }
        }
    }

    const EPSILON = Math.pow(2, -52); //2.2204460492503130808472633361816e-16
    function formatNumber(value, dataFormat) {
        if (!isNaN(value)) {
            let format = { decimal: ".", separator: ",", fixed: 3 };
            format = $.extend(format, dataFormat);
            let _value = value.toString();
            if (_value.indexOf(format.decimal) > 0) {
                let values = value.toString().split(format.decimal);
                values[values.length - 1] += createZeroes(format.fixed - values[values.length - 1].length);
                values[values.length - 1] = values[values.length - 1].substring(0, format.fixed);
                _value = values.join(format.decimal);
            } else {
                _value = _value + format.decimal + createZeroes(format.fixed);
            }

            let pattern = new RegExp("\\d(?=(\\d{3})+\\" + format.decimal + ")", "g");
            let data = _value.replace(pattern, "$&" + format.separator);
            if (format.fixed == 0) {
                data = data.split(".")[0];
            }
            return data;
        }
    }


    this.updateFieldFormat = function (keys, dataType, dataFormat) {
        let columns = __setting.columns;
        let scaleColumns = __setting.scaleColumns;
        if (typeof dataType !== "string") {
            return;
        }

        for (let k = 0; k < keys.length; k++) {
            for (let i = 0; i < scaleColumns.length; i++) {
                let scaleField = scaleColumns[i];
                if (keys[k] == scaleField["name"] && dataType === "number") {
                    if (isValidJson(scaleField) && isValidJson(dataFormat)) {
                        scaleField["dataFormat"] = $.extend(true, scaleField["dataFormat"], dataFormat);
                    }
                }
            }

            for (let i = 0; i < columns.length; i++) {
                let field = columns[i];
                if (keys[k] == field["name"] && dataType === field["dataType"]) {
                    if (isValidJson(field) && isValidJson(dataFormat)) {
                        field["dataFormat"] = $.extend(true, field["dataFormat"], dataFormat);
                    }
                }
            }
        }
    }

    function createZeroes(length) {
        let result = "";
        for (let i = 0; i < length; i++, result += "0");
        return result;
    }

    function modifyDataType(value, field) {
        if (field.dataType) {
            switch (field.dataType.toLowerCase()) {
                case "number":
                    value = formatNumber(value, field.dataFormat);
                    break;
                case "date":
                    value = formatDate(value, field);
                    break;
            }
        }
        return value;
    }

    function formatDate(value, field) {
        var regex = new RegExp("[\-\/\.\s+]", "gi");
        var separator = regex.exec(field.dataFormat);
        let _field = {
            dataType: "date",
            dataFormat: "YYYY-MM-DD"
        },
            dt = new Date(value),
            objFormat = {
                MM: ("0" + (dt.getMonth() + 1)).slice(-2),
                DD: ("0" + dt.getDate()).slice(-2),
                YYYY: dt.getFullYear().toString()
            },
            dateString = "";

        if (isValidJson(field)) {
            $.extend(_field, field);
        }

        let dateFormats = _field.dataFormat.split(regex);
        for (let i = 0; i < dateFormats.length; i++) {
            let datePattern = objFormat[dateFormats[i].replace(/\s+/g, "")];
            dateString += datePattern;
            if (i < dateFormats.length - 1) {
                dateString += separator[0];
            }
        }
        return dateString;
    }

    function concatFieldName(model, target, fieldName, rowIndex) {
        if (isValidJson(model)) {
            var props = Object.getOwnPropertyNames(model);
            var nameAttr = "";

            for (let i = 0; i < props.length; i++) {
                if (isValidJson(model[props[i]])) {
                    nameAttr += props[i] + "." + concatFieldName(model[props[i]], target, fieldName, rowIndex);
                }

                if (Array.isArray(model[props[i]]) && model[props[i]].length === 0) {
                    nameAttr += props[i] + "[" + rowIndex + "]";
                    if (model[props[i]].length <= 0) {
                        nameAttr += "." + fieldName;
                    }
                }
            }
            target.setAttribute("name", nameAttr);
            return nameAttr;
        }
    }

    function defineTitle(field, value, $col) {
        if (isValidJson(field.title)) {
            if (field.title.enabled) {
                $col.attr("title", value);
                if (field.title.text !== "") {
                    $col.attr("title", field.title.text);
                }
            }
        }
    }

    function insertTemplate(data, field, value, col) {
        if (isValidJson(field)) {
            let target = $(field.template)[0];
            if (field.nameField === undefined) {
                field.nameField = field.name;
            }
            concatFieldName(__setting.model, target, field.nameField, __meta.dataset.indexOf(data));
            value = modifyDataType(value, field);
            if (!target) {
                target = col;
                if (field.name) {
                    $(target).html(value);
                }
            } else {
                defineTargetValue(target, data, value, field);
                switch (field.insertion) {
                    case "prepend":
                        $(col).prepend(target);
                        break;
                    case "append":
                        $(col).append(target);
                        break;
                    case "replace":
                        $(col).html(target);
                        break;
                    default:
                        $(col).html(target);
                        break;
                }
            }

            syncData(target, data, field.nameField);
            if (isValidJson(field.on)) {
                let event = Object.getOwnPropertyNames(field.on)[0];
                $(target).off(event.toLowerCase()).on(event.toLowerCase(), function (e) {
                    syncData(target, data, field.nameField);
                    field.on[event].call(target, fallbackColumn(data, field.name, data[field.name], target, $(col)[0]), e);
                });
            } else {
                $(target).on("input change", function (e) {
                    syncData(target, data, field.nameField);
                });
            }
        }
    }

    function defineTargetValue(target, data, value, field) {
        switch (target.tagName) {
            case "INPUT":
                switch (target.getAttribute("type")) {
                    case "date":
                        target.valueAsDate = new Date(value);
                        if (!value) {
                            target.value = "";
                        }
                        break;
                    case "checkbox": case "radio":
                        $(target).prop("checked", value);
                        target.value = value;
                        break;
                    default:
                        target.value = value;
                        break;
                }

                break;
            case "SELECT":
                if (isValidArray(data[field.name])) {
                    defineSelectList(data[field.name], target);
                } else {
                    target.value = value;
                }

                break;
            case "TEXTAREA":
                target.value = value;
                break;
            case "IMG":
                const path = $(target).data("path").split("/");
                const defaultImg = path[path.length - 1];
                path.pop();
                const url = path.join("/");
                $(target).attr("src", `${url}/${value ?? defaultImg}`);
                break;
            default:
                target.textContent = value;
                break;
        }
    }

    function isBoolean(stringValue) {
        if ((/true/i).test(stringValue) || (/false/i).test(stringValue)) {
            return typeof JSON.parse(stringValue) === "boolean";
        }
        return false;
    }

    function syncData(target, json, fieldName) {
        if (__setting.dataSynced) {
            switch (target.tagName) {
                case "INPUT":
                    switch (target.getAttribute("type")) {
                        case "checkbox": case "radio":
                            if (isBoolean(json[fieldName])) {
                                json[fieldName] = $(target).prop("checked");
                            } else {
                                json[fieldName] = target.value;
                            }
                            target.value = json[fieldName];
                            break;
                        default:
                            json[fieldName] = target.value;
                            break;
                    }
                    break;
                case "SELECT": case "TEXTAREA":
                    json[fieldName] = target.value;
                    break;
                default:
                    json[fieldName] = target.textContent;
                    break;
            }
        }
    }

    function fallbackRow(data, target) {
        return {
            key: data[__setting.keyField],
            data: data,
            row: target,
            columns: target.childNodes,
            meta: __meta
        }
    }

    function fallbackColumn(data, fieldName, fieldValue, target, col) {
        __this.highlightRow(data);
        return {
            key: data[__setting.keyField],
            fieldName: fieldName,
            fieldValue: fieldValue,
            data: data,
            target: target,
            column: col,
            row: col.parentNode,
            meta: __meta
        }
    }

    function groupBy(values, keys, process) {
        let grouped = {};
        for (let i = 0; i < values.length; i++) {
            let a = values[i];
            keys.reduce(function (o, j, k) {
                o[a[j]] = o[a[j]] || (k + 1 === keys.length ? [] : {});
                return o[a[j]];
            }, grouped).push(a);
        }

        if (typeof process === "function") {
            process(grouped);
        }
        return grouped;
    }

    function clone(templateProp, deep) {
        if (deep) { return __template[templateProp].cloneNode(deep); }
        return __template[templateProp].cloneNode(deep);
    }

    function isEmpty(value) {
        return value == undefined || value == null || value == "";
    }

    //Check if json object is valid.
    function isValidJson(json) {
        return !isEmpty(json) && json.constructor === Object && Object.keys(json).length > 0;
    };

    //Check if value is valid array.
    function isValidArray(array) {
        return Array.isArray(array) && array.length > 0;
    };
}));