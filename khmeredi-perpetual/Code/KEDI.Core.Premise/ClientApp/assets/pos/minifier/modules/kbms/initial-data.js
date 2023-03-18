//Get all setting
let setting = $.ajax({
    url: "/POS/GetSetting",
    async: false,
    type: "GET",
    dataType: "JSON",
    data: { branchid: order_info.branchid }
}).responseJSON[0];

//Get VAT
let vat_include = {};
vat_include.name;
vat_include.rate = 0;
vat_include.value = 0;
$.ajax({
    url: "/POS/GetVat",
    async: false,
    type: "GET",
    dataType: "JSON",
    success: function (response) {
        if (response.length > 0) {
            vat_include.name = response[0].Name;
            vat_include.rate = response[0].Rate;
        }
    }
});

newdefaultOrder();
//Get order information
function newdefaultOrder() {
    return order_master = {
        OrderID: 0,
        OrderNo: 'N/A',
        TableID: 1,
        ReceiptNo: 'N/A',
        QueueNo: "1",
        DateIn: date.getDate(),
        DateOut: date.getDate(),
        TimeIn: date.toLocaleTimeString(),
        TimeOut: date.toLocaleTimeString(),
        WaiterID: 1,
        UserOrderID: order_info.user,
        UserDiscountID: order_info.user,
        CustomerID: setting.CustomerID,
        CustomerCount: 1,
        PriceListID: setting.PriceListID,
        LocalCurrencyID: setting.LocalCurrencyID,
        SysCurrencyID: setting.SysCurrencyID,
        ExchangeRate: setting.RateIn,
        WarehouseID: setting.WarehouseID,
        BranchID: order_info.branchid,
        CompanyID: setting.CompanyID,
        Sub_Total: 0,
        DiscountRate: 0,
        DiscountValue: 0,
        TypeDis: 'Percent',
        TaxRate: vat_include.rate,
        TaxValue: vat_include.value,
        GrandTotal: 0,
        GrandTotal_Sys: 0,
        Tip: 0,
        Received: 0,
        Change: 0,
        PaymentMeansID:setting.PaymentMeansID,
        CheckBill: 'N'
      
    };
};

//Get Local Currency
let local_currency = {};
local_currency.id;
local_currency.symbol;
local_currency.ratein;
local_currency.rateout;

$.ajax({
    url: "/POS/GetLocalCurrecny",
    async: false,
    type: "GET",
    dataType: "JSON",
    data: { currencyid: setting.LocalCurrencyID },
    success: function (response) {
        $(".local-symbol").text(response.Currency.Description);
        local_currency.id = response.Currency.ID;
        local_currency.symbol = response.Currency.Description;
        local_currency.ratein = response.Rate;
        local_currency.rateout = response.RateOut;
    }
});
//Get display rate 
let displayCurrency = $.ajax({
    url: "/POS/GetDisplayCurrency",
    async: false,
    type: "GET",
    dataType: "JSON"
}).responseJSON;

fx.base = displayCurrency[0].BaseCurr;
fx.rates = {};
$.each(displayCurrency, function (i, curr) {
    if (curr.AltCurr !== fx.base) {
     
        db.insert('tb_display_curr', curr, 'ID');
        fx.rates[curr.AltCurr] = curr.Rate;
        
    }
    let dis_curr = "<option value=" + curr.AltCurr + ">" + curr.AltCurr + "</option>";
    $('.receipt-other-curr').append(dis_curr);
    
});
let dis_curr = db.from('tb_display_curr');

//Get user information
let user_infor = $.ajax({
    url: '/POS/GetUserInfo',
    async: false,
    type: 'GET',
    data: { userid: order_info.user },
    success: function () { }
}).responseJSON;
db.insert("tb_user_info", user_infor, 'ID');

//Get user open shift
OpenShift();
function OpenShift() {
    let user_open_shift = $.ajax({
        url: '/POS/CheckOpenShift',
        async: false,
        type: 'GET',
        data: { userid: order_info.user },
        success: function () { }
    }).responseJSON;
    db.insert("tb_check_open_shift", user_open_shift, 'UserID');
};

//Get exchange rate 
let exchange_rate=$.ajax({
    url: '/POS/GetExchangeRate',
    async: false,
    type: 'GET',
    success: function () { }
}).responseJSON;
db.insert("tb_exchange_rate", exchange_rate, 'CurrencyID');

//Other currency list
$.each(exchange_rate, function (i, curr) {
    let out = "<option value=" + curr.Currency.ID + ">" + curr.Currency.Description + "</option>";
    $('#other-currency-list').append(out);
    $('#other-currency-list').val(setting.SysCurrencyID);
});

//Get Group Uom Defined
$.ajax({
    url: "/POS/GetGroupUomDefined",
    async: false,
    type: "GET",
    dataType: "JSON",
    success: function (response) {

        if (response.length > 0) {
            db.insert("tb_group_uom_defined", response, "ID");
        }

    }
});

let table_info = {};
table_info.id = 1;
table_info.name = 'N/A';
table_status = 'A';
table_info.time = "00:00:00";
//Get default group 1
$.ajax({
    url: "/POS/GetGroupItem",
    type: "GET",
    dataType: "JSON",
    data: { group_id: 0, level: 0 },
    success: function (group1) {
        db.insert("tb_group1", group1, "ItemG1ID");
    }
});

//Get all item master data
let item_masters = $.ajax({
    url: "/POS/GetItemMasterData",
    async: false,
    type: "GET",
    dataType: "JSON",
    data: { PriceListID: setting.PriceListID },
}).responseJSON;
db.addTable("tb_item_master", item_masters, "ID");

$.ajax({
    url: '/POS/GetPriceList',
    async: false,
    type: 'GET',
    dataType: 'JSON',
    success: function (pricelist) {
        $.each(pricelist, function (i, pr) {
            $('.price-list-id').append("<option value=" + pr.ID + ">" + pr.Name + "</option>");
        });
    }
});
$.ajax({
    url: '/POS/GetCustomer',
    async: false,
    type: 'GET',
    dataType: 'JSON',
    success: function (customer) {
        db.insert("tb_customer", customer, "ID");
        $.each(customer, function (i, cus) {
            $('.customer-id').append("<option value=" + cus.ID + ">" + cus.Name + "</option>");
            $('.price-list-id').val(cus.PriceListID);
        });
    }
});
//Customer
let customer = db.get('tb_customer').get(order_master.CustomerID);
$('.customer-name').text(customer.Name);

$.ajax({
    url: '/POS/GetPaymentMeans',
    async: false,
    type: 'GET',
    dataType: 'JSON',
    success: function (paymentmeans) {
        $.each(paymentmeans, function (i, pay) {
            $('.payment-means-id').append("<option value=" + pay.ID + ">" + pay.Type + "</option>");
            $('.payment-means-id-choosed').append("<option value=" + pay.ID + ">" + pay.Type + "</option>");
        })
    }
});

$.ajax({
    url: '/POS/GetWarehouse',
    async: false,
    type: 'GET',
    dataType: 'JSON',
    data: { branchid: order_info.branchid },
    success: function (warehouse) {
        $.each(warehouse, function (i, wh) {
            $('.warehouse-id').append("<option value=" + wh.ID + ">" + wh.Name + "</option>");
        })
    }
});

$.ajax({
    url: '/POS/GetPrinterName',
    async: false,
    type: 'GET',
    dataType: 'JSON',
    success: function (printer) {
        $.each(printer, function (i, print) {
            $('.printer-id').append("<option value=" + print.Name + ">" + print.Name + "</option>");
            $('.add-new-item-printto').append("<option value=" + print.Name + ">" + print.Name + "</option>");
        });
    }
});

$.ajax({
    url: '/POS/GetUserPriviliges',
    async: false,
    type: 'GET',
    dataType: 'JSON',
    data: { userid: order_info.user },
    success: function (user) {
        db.insert("tb_user_privillege", user, 'Code');
    }
});

getCurrentDate();
//Show datetime
function getCurrentDate() {
    setInterval(function () {
        $(".datetime").html(padLeft(new Date().getDate(), 2) + "-" + padLeft((new Date().getMonth() + 1), 2) + "-" + new Date().getFullYear() + " " + new Date().toLocaleTimeString());
    }, 1000);
};

//Pad left number
function padLeft(data, size, paddingChar) {
    return (new Array(size + 1).join(paddingChar || '0') + String(data)).slice(-size);
};
