$(document).ready(function () {
    let allData = {};
    getMenuData();

    $("#itemsearch").change(function () {
        if (this.value == 1) {
         
            $(".container-search-data").removeClass('show-search');
            getMenuData();

        } else if (this.value != '') {
            $("#menu-container").removeClass('show-search');
            $("#menu-container1").style = "color:red"
            getSerachData();


        }

    });
    function bindMenu(data) {
        if (data.length > 0) {
            let stirngBuilder = ''
            data.forEach(i => {
                stirngBuilder += `<a href="${i.Url}">${i.Title}</a>`
            })
            $("#menu-container").find('a').remove()
            $("#menu-container").append(stirngBuilder)
        }
    }
    function getMenuData() {
        $("#search-menu-input").off('keyup')
        $.get("/SearchGloble/Getsearchgbloble", function (data) {

            bindMenu(data);
            $("#search-menu-input").on("keyup", function (e) {
                if (this.value != '') {
                    $("#menu-container").addClass('show-search');
                } else {
                    $("#menu-container").removeClass('show-search');
                }
                let __value = this.value.toLowerCase().replace(/\s+/, "");
                let rex = new RegExp(__value, "gi");
                let menu = $.grep(data, function (person) {
                    return person.Title.toLowerCase().replace(/\s+/, "").match(rex)
                });
                bindMenu(menu)
            });
        });
    }
    //Search Data
    const getSerachData = () => {
        $("#search-menu-input").off('keyup')
        $("#search-menu-input").on("keyup", function (e) {
            //if (e.which == 13) {
            $.get("/SearchGloble/SearchAllItem", { keyword: this.value }, function (res) {
                var itemsearch = $("#search-menu-input").val();
                if (itemsearch != '') {
                    allData = res;
                    if (allData.TotalItems > 0) {
                        bindItemMasterData(allData.ItemMaster)
                        bindReceipt(allData.Receipts)
                        bindItemAcccbalnce(allData.AccountBalances)
                        bindItemAcc(allData.ItemAccounts)
                        bindcompany(allData.Companys)
                        bindbrand(allData.BrandsViews)
                        bindUserAcc(allData.UserAccounts)
                        bindPrinterName(allData.PrinterNames)
                        bindItemG1(allData.ItemGroup1s)
                        bindItemG2(allData.ItemGroup2s)
                        bindItemG3(allData.ItemGroup3s)
                        bindProperty(allData.Property)
                        bindTable(allData.Table)
                        bindGTable(allData.Groups)
                        bindFreight(allData.Freights)
                        bindRemark(allData.Remarkdis)
                        bindper(allData.Per)
                        bindPostingPeriod(allData.Posting)
                        bindChartAcc(allData.Chartsacc)
                        bindjournal(allData.Journal)
                        bindPricelist(allData.Pricelist)
                        bindBusinesPartner(allData.Busines)
                        bindCardmember(allData.Cardsmember)
                        bindCurrency(allData.Currency)
                        bindpaymeant(allData.Payment)
                        bindEmployee(allData.Emplyee)
                        bindSetupservice(allData.Setup)
                        bindReceiptinformation(allData.ReceiptIfo)
                        bindFunctions(allData.Function)
                        bindUnitmeasure(allData.Unitmeasure)
                        bindgroupuom(allData.Guom)
                        bindWahouse(allData.Warhous)

                        $(".container-search-data").addClass('show-search');
                    } else $(".container-search-data").removeClass('show-search');
                } else {
                    $(".container-search-data").removeClass('show-search');
                }


            });
            //}
        });
    }

    //itemmaster data
    const bindItemMasterData = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
               /* enabled: false*/
            },
            visibleFields: ["Code", "EnglishName", "KhmerName"],
            columns: [
                {
                    name: "KhmerName",
                    on: {
                        click: function (e) {
                            location.href = `/ItemMasterData/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#SearchlistItem",
        });
        bindTables(data, $listView, $("#item-master-data1"));
/*        bindAllData(data, $listView, "#all");*/
    }
    //Receipt
    const bindReceipt = (data) => {
        let $listView = ViewTable({
            keyField: "ReceiptID",
            paging: {
               /* enabled: false*/
            },
            visibleFields: ["ReceiptNo", "OrderNO"],
            columns: [
                {
                    name: "ReceiptNo",
                    on: {
                        click: function (e) {
                            location.href = `/ReceiptInformation/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbreceipt",
        });

        bindTables(data, $listView, $("#receipts"));
   /*     bindAllData(data, $listView, "#rec");*/
    }
    //AccountBalnce
    const bindItemAcccbalnce = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
              /*  enabled: false*/
            },
            visibleFields: ["OfsetAccount", "Detail"],
            columns: [
                {
                    name: "OfsetAccount",
                    on: {
                        click: function (e) {
                            location.href = `/ItemMasterData/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbAccBalance",
        });

        bindTables(data, $listView, $("#AccBalnce"));
   /*     bindAllData(data, $listView, "#ba");*/
    }
    //ItemAccounting
    const bindItemAcc = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["ExpenesAcc", "RevenueAcc", "InventoryAcc"],
            columns: [
                {
                    name: "OfsetAccount",
                    on: {
                        click: function (e) {
                            location.href = `/ItemMasterData/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbItemAcc",
        });

        bindTables(data, $listView, $("#ItemAcc"));
   
    }
    // company
    const bindcompany = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
               /* enabled: false*/
            },
            visibleFields: ["Name", "Location"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/Company/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbCompanys",
        });

        bindTables(data, $listView, $("#Company"));
     /*   bindAllData(data, $listView, "#Com");*/

    }
    //brand
    const bindbrand = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["BrandName", "Location"],
            columns: [
                {
                    name: "BrandName",
                    on: {
                        click: function (e) {
                            location.href = `/Branch/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbBrand",
        });

        bindTables(data, $listView, $("#Brand"));

    }
    //UserAccount
    const bindUserAcc = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["UserName"],
            columns: [
                {
                    name: "UserName",
                    on: {
                        click: function (e) {
                            location.href = `/Account/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbUseracc",
        });

        bindTables(data, $listView, $("#Useracc"));
    }
    //Printer Name
    const bindPrinterName = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["PrinterName", "OrderCount"],
            columns: [
                {
                    name: "PrinterName",
                    on: {
                        click: function (e) {
                            location.href = `/PrinterName/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbPrinterName",
        });

        bindTables(data, $listView, $("#PrinterName"));
    }
    //item group 1
    const bindItemG1 = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["NameGroup"],
            columns: [
                {
                    name: "NameGroup",
                    on: {
                        click: function (e) {
                            location.href = `/ItemGroup1/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbiemtg1",
        });

        bindTables(data, $listView, $("#iemtg1"));
    }
    //item group2
    const bindItemG2 = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["NameGroup"],
            columns: [
                {
                    name: "NameGroup",
                    on: {
                        click: function (e) {
                            location.href = `/ItemGroup2/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbiemtg2",
        });

        bindTables(data, $listView, $("#iemtg2"));
    }
    //itemg3
    const bindItemG3 = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["NameGroup"],
            columns: [
                {
                    name: "NameGroup",
                    on: {
                        click: function (e) {
                            location.href = `/ItemGroup3/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbiemtg3",
        });

        bindTables(data, $listView, $("#iemtg3"));
    }
    //property
    const bindProperty = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "Active"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/Property/Update?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbPro",
        });

        bindTables(data, $listView, $("#Pro"));
    }

    //Table
    const bindTable = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "Status"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/Table/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbTable",
        });

        bindTables(data, $listView, $("#Table"));
    }
    //Group Table
    const bindGTable = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "Type"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/GroupTable/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbGTable",
        });

        bindTables(data, $listView, $("#GTable"));
    }
    //Freights
    const bindFreight = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/Freight/index`
                        }
                    }
                }
            ],
            selector: "#tbFreight",
        });

        bindTables(data, $listView, $("#Freight"));
    }
    //RemarkDiscount
    const bindRemark = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/RemarkDiscount/Index`
                        }
                    }
                }
            ],
            selector: "#tbRemark",
        });

        bindTables(data, $listView, $("#Remark"));
    }

    //periodIdecator
    const bindper = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/PeriodIndicator/edit/?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbper",
        });

        bindTables(data, $listView, $("#per"));
    }
    //Posting Period
    const bindPostingPeriod = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "PeriodCode"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/postingperiod/edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbPosting",
        });

        bindTables(data, $listView, $("#Posting"));
    }
    //chart account
    const bindChartAcc = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "Code"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/ChartOfAccounts/Detail`
                        }
                    }
                }
            ],
            selector: "#tbchartacc",
        });

        bindTables(data, $listView, $("#chartacc"));
    }
    //JournalEntry
    const bindjournal = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Number", "Tranno", "Remarks"],
            columns: [
                {
                    name: "Number",
                    on: {
                        click: function (e) {
                            location.href = `/Financials/JournalEntry`
                        }
                    }
                }
            ],
            selector: "#tbjournal",
        });

        bindTables(data, $listView, $("#journal"));
    }
    //PriceList
    const bindPricelist = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/PriceList/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbPricelist",
        });

        bindTables(data, $listView, $("#Pricelist"));
    }
    //BusinesPartner
    const bindBusinesPartner = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "Code", "Type", "Phone", "Email", "Address"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/BusinessPartner/UpdateCustomer?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbBusinespartner",
        });

        bindTables(data, $listView, $("#Businespartner"));
    }
    //Card member
    const bindCardmember = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "Code", "Discription"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/BusinessPartner/UpdateCustomer?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbcard",
        });

        bindTables(data, $listView, $("#cardmember"));
    }
    //corrency
    const bindCurrency = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Symbol", "Description",],
            columns: [
                {
                    name: "Symbol",
                    on: {
                        click: function (e) {
                            location.href = `/Currency/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbScurrency",
        });

        bindTables(data, $listView, $("#Scurrency"));
    }
    // Paymeant 
    const bindpaymeant = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Type", "AccountID",],
            columns: [
                {
                    name: "Type",
                    on: {
                        click: function (e) {
                            location.href = `/PaymentMeans/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbSpaymeant",
        });

        bindTables(data, $listView, $("#Spaymeant"));
    }
    //Employee
    const bindEmployee = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "Code", "Gender", "Address", "Phone", "Email", "Positon"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/Employee/Update?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbSEmployee",
        });

        bindTables(data, $listView, $("#SEmployee"));
    }
    // Setup service
    const bindSetupservice = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Setupdcode", "Price", "Remark"],
            columns: [
                {
                    name: "Setupdcode",
                    on: {
                        click: function (e) {
                            location.href = `/ksmsservicesetup/history`
                        }
                    }
                }
            ],
            selector: "#tbSSetupserivce",
        });

        bindTables(data, $listView, $("#SSetupserivce"));

    }
    //Receipt Infomations
    const bindReceiptinformation = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Title", "Address", "Tel1", "Tel2", "Khmername", "Englishname"],
            columns: [
                {
                    name: "Title",
                    on: {
                        click: function (e) {
                            location.href = `/ReceiptInformation/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbSReceiptinfo",
        });

        bindTables(data, $listView, $("#SReceiptinfo"));
    }
    //Funtions
    const bindFunctions = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "Type", "Code"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/Account/UserPrivileges`
                        }
                    }
                }
            ],
            selector: "#tbSFuntion",
        });

        bindTables(data, $listView, $("#SFuntion"));
    }
    //Unit of Measure
    const bindUnitmeasure = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name","Code"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/UnitofMeasure/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbunitofmeasure",
        });
      

        bindTables(data, $listView, $("#unitofmeasure"));
    }

    //Group Uom
    const bindgroupuom = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "Code"],
            columns: [
                {
                    name: "Name",
                    on: {
                        click: function (e) {
                            location.href = `/GroupUOM/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbguom",
        });


        bindTables(data, $listView, $("#guom"));
    }
    //Warhouse
    const bindWahouse = (data) => {
        let $listView = ViewTable({
            keyField: "ID",
            paging: {
            },
            visibleFields: ["Name", "Code", "Location","Address"],
            columns: [
                {
                    name: "Code",
                    on: {
                        click: function (e) {
                            location.href = `/Warehouse/Edit?id=${e.key}`
                        }
                    }
                }
            ],
            selector: "#tbwarhouse",
        });


        bindTables(data, $listView, $("#warhouse"));
    }

    var n=100

    function bindTables(data, table, container) {
        if (data.length <= 0) {
            table.clearRows();
            container.removeClass("show-search");
        } else {
            container.addClass("show-search");
            table.clearRows();
            table.bindRows(data.slice(0, n));
        }
    }
    function bindAllData(data, table, container) {
        $(container).click(function () {
            const __this = this;
            $("#search-menu-input").keyup(function () {
                $(__this).data("all", true);
                $(__this).text("Expand");
                $(__this).off("click");
            });
            const isExpand = $(this).data("all");
            if (isExpand) {
                $(this).text("Collapse");
                if (data.length <= 0) {
                    table.clearRows();
                } else {
                    table.clearRows();
                    table.bindRows(data);
                }
                $(this).data("all", false)
            } else if (!isExpand){
                $(this).text("Expand");
                if (data.length <= 0) {
                    table.clearRows();
                } else {
                    table.clearRows();
                    table.bindRows(data.slice(0, 10));
                }
                $(this).data("all", true);
            }

        })

    }

    $("#cl").click(function () {
        $(".container-search-data").removeClass('show-search');
    });

})