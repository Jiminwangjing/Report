function Numberal() {
    if (!(this instanceof Numberal)) {
        return new Numberal();
    }
    /**
     * 
     * @param {any} num: string or number to format
     * @param {any} digits: decimal places
     * EX1: numberShorten(1000)=> 1K
     * EX2: numberShorten(1300)=> 1K
     * EX3: numberShorten(1300,1)=> 1.3K
     */
    this.numberShorten = function (num, digits) {
        const lookup = [
            { value: 1, symbol: "" },
            { value: 1e3, symbol: "K" },
            { value: 1e6, symbol: "M" },
            { value: 1e9, symbol: "B" },
            { value: 1e12, symbol: "T" },
            { value: 1e15, symbol: "P" },
            { value: 1e18, symbol: "E" }
        ]

        const rx = /\.0+$|(\.[0-9]*[1-9])0+$/
        const item = lookup.slice().reverse().find((item) => {
            return num >= item.value
        })
        return item ? (num / item.value).toFixed(digits).replace(rx, "$1") + item.symbol : "0"
    }
}

function NumberFormat(config) {
    if (!(this instanceof NumberFormat)) {
        return new NumberFormat(config);
    }
    var __config = {
        decimalSep: ".",
        thousandSep: ","
    }

    if (isValidJSON(config)) {
        __config = $.extend(true, __config, config);
    }
    /**
     * @param {any} n: number
     * @param {any} decimal: number(how many decimal places you want to have)
     * ex1: formatSpecial(134322364, 6) => 134,322,364.000000
     * ex2: formatSpecial(134322364, 6) => 134.322.364,000000 if __config.thousandSep === "."
     */
    this.formatSpecial = function (n, decimal) {
        let num = parseFloat(n).toFixed(decimal).replace(/\d(?=(\d{3})+\.)/g, '$&,').split(",");
        let a = num[num.length - 1].replace(__config.thousandSep, ",");
        let b = num.slice(0, num.length - 1).join(__config.thousandSep);
        let c = [];
        c.push(b);
        c.push(a);
        if (b !== "") c = c.join(__config.thousandSep);
        if (b === "") c = c.join("");
        return c;
    }
    /**
     * 
     * @param {any} n: number
     * @param {any} decimal: number(how many decimal places you want to have)
     * ex: formatSimple(42424, 6) => 42,424.000000
     */
    this.formatSimple = function (n, decimal) {
        return parseFloat(n).toFixed(decimal).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
    /**
     * @param {any} n:  Number of string || or whatever but can be convert to number
     * ex: toNumberSimple("324,452,345.34") => 324452345.34 (typeof Number)
     */
    this.toNumberSimple = function (n) {
        if (value.toString().includes(",")) {
            return parseFloat(value.split(",").join(""));
        }
        else
            return parseFloat(n);
    }
    /**
     * @param {any} n: Number of string || or whatever but can be convert to number
     * ex1: toNumberSpecial("4,387,342,243.843") => 4387342243.843 (typeof Number)
     * ex2: toNumberSpecial("4.387.342.243,843") => 4387342243.843 (typeof Number)
     */
    this.toNumberSpecial = function (n) {
        if (n == "") n = "0";
        n = n.toString();
        if (__config.thousandSep === ",") {
            n = n.split(__config.thousandSep);
            let a = n[n.length - 1];
            let b = n.slice(0, n.length - 1).join(__config.thousandSep).split(__config.thousandSep).join("");
            let c = [];
            c.push(b);
            c.push(a);
            c = c.join("")
            return parseFloat(c);
        }
        if (__config.thousandSep === ".") {
            n = n.split(__config.thousandSep);
            let a = n[n.length - 1].replace(",", __config.thousandSep);
            let b = n.slice(0, n.length - 1).join(__config.thousandSep).split(__config.thousandSep).join("");
            let c = [];
            c.push(b);
            c.push(a);
            c = c.join(".")
            return parseFloat(c);
        }
        return n;
    }
    /**
     * 
     * @param {any} num: string or number to format
     * @param {any} digits: decimal places
     * EX1: numberShorten(1000)=> 1K
     * EX2: numberShorten(1300)=> 1K
     * EX3: numberShorten(1300,1)=> 1.3K
     */
    this.numberShorten = function (num, digits) {
        const lookup = [
            { value: 1, symbol: "" },
            { value: 1e3, symbol: "K" },
            { value: 1e6, symbol: "M" },
            { value: 1e9, symbol: "B" },
            { value: 1e12, symbol: "T" },
            { value: 1e15, symbol: "P" },
            { value: 1e18, symbol: "E" }
        ]

        const rx = /\.0+$|(\.[0-9]*[1-9])0+$/
        const item = lookup.slice().reverse().find((item) => {
            return num >= item.value
        })
        return item ? (num / item.value).toFixed(digits).replace(rx, "$1") + item.symbol : "0"
    }
    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.keys(value).length > 0;
    }
}
