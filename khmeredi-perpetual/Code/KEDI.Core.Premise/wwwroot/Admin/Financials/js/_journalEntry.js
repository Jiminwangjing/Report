
$(document).ready(function () {
    var __JE = JSON.parse($("#journal-entry").text());
    let selected = $("#series-id");
    selectSeries(selected);
    function selectSeries(selected) {
        $.each(__JE.Series, function (i, item) {
            if (item.Default == true) {
                $("<option selected value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
            }
            else {
                $("<option  value=" + item.ID + ">" + item.Name + "</option>").appendTo(selected);
            }
        });
        return selected.on('change')
    }
    $.get("/Financials/GetBranch",function(res){
        if(res.length>0)
        {
           let data="";
           $.each(res,function(i,item){
                   if(item.Active)
                   data+=`<option selected value="${item.ID}">${item.Name}</option>`;
                   else
                   data+=`<option value="${item.ID}">${item.Name}</option>`;
           });
           $("#branch").append(data);
        }
          
   
   });

    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }

    function isValidObject(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }

    function setValue(obj, prop, value) {
        if (isValidObject(obj)) {
            obj[prop] = value;
        }
    }

    function find(keyName, keyValue, values) {
        if (isValidArray(values)) {
            return $.grep(values, function (item, i) {
                return item[keyName] == keyValue;
            })[0];
        }
    }
    $('#series-id').change(function () {
        var id = ($(this).val());
        var series = find("ID", id, __JE.Series);
        __JE.Number = series.NextNo;
        __JE.SeriesID = id;
        $("#next-number").val(series.NextNo);
    })


    let $listJE = ViewTable({
        keyField: "LineID",
        selector: "#journal-entries",
        paging: {
            enabled: false
        },

        visibleFields: ["CodeTM", "NameTM", "DebitTM", "CreditTM", "Remarks"],
        columns: [
            {
                name: "CodeTM",
                on: {
                    "dblclick": function (e) {
                        chooseCode(e.key);
                    }
                }
            },
            {
                name: "NameTM",
                on: {
                    "dblclick": function (e) {
                        chooseCode(e.key);
                    }
                }
            },
            {
                name: "DebitTM",
                template: "<input type='text' class='number'>",
                on: {
                    "Keyup": function (e) {
                        $(this).asNumber()
                        e.data.Debit = this.value;
                        const _sum = sum($listJE.yield(), "Debit");
                        $("#total-debit").val(toCurrency(_sum));
                        if (this.value != "") {
                            $listJE.disableColumns(e.key, ["CreditTM"]);
                        } else {
                            $listJE.disableColumns(e.key, ["CreditTM"], false);
                        }
                    }
                }
            },
            {
                name: "CreditTM",
                template: "<input type='text' class='number'>",
                on: {
                    "Keyup": function (e) {
                        $(this).asNumber()
                        e.data.Credit = this.value;
                        const _sum = sum($listJE.yield(), "Credit");
                        $("#total-credit").val(toCurrency(_sum));
                        if (this.value != "") {
                            $listJE.disableColumns(e.key, ["DebitTM"]);
                        } else {
                            $listJE.disableColumns(e.key, ["DebitTM"], false);
                        }
                    }
                }
            },
            {
                name: "Remarks",
                template: "<input class='ramarks'>",
                on: {
                    "Keyup": function (e) {
                        e.data.Remarks = this.value;
                    }
                }
            }
        ]

    });

    function sum(items, fieldName, callback) {
        let total = 0;
        for (let i = 0; i < items.length; i++) {
            if (items[i][fieldName] == "") items[i][fieldName] = 0;
            total += toNumber(items[i][fieldName]);
            if (typeof callback === "function") {
                callback.call(__this, i, items[i], total);
            }
        }
        return total;
    }
    function toNumber(value) {
        if (value !== undefined) {
            if (typeof value === "number") {
                value = value.toString();
            }
            return parseFloat(value.split(",").join(""));
        }
    }
    function toCurrency(value) {
        return parseFloat(value).toFixed(3).replace(/\d(?=(\d{3})+\.)/g, '$&,');
    }
    function chooseCode(id) {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "Close",
                    callback: function () {
                        this.meta.shutdown();
                    }
                }
            },
            content: {
                selector: "#active-gl-content"
            }
        });

        dialog.invoke(function () {
            $("#txtseaerch-acc").on("keyup", function (e) {
                var value = $(this).val().toLowerCase();
                $("#list-active-gl tr:not(:first-child)").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
                });
            });
            let $listActiveGL = ViewTable({
                keyField: "ID",
                selector: dialog.content.find("#list-active-gl"),
                paging: {
                    pageSize: 20,
                    enabled: true
                },
                visibleFields: ["Code", "Name"],
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                $listJE.updateColumn(id, "CodeTM", e.data.Code);
                                $listJE.updateColumn(id, "NameTM", e.data.Name);
                                $listJE.updateColumn(id, "ItemID", e.data.ID);
                                dialog.shutdown();
                                $listJE.disableColumns(e.key, ["CreditTM"]);
                                $listJE.disableColumns(e.key, ["DebitTM"]);
                                $listJE.disableColumns(e.key, ["Remarks"]);
                            }
                        }
                    }
                ]
            });
            $.get("/Financials/GetActiveGLAccounts", function (resp) {
                $listActiveGL.bindRows(resp, resp[0]);
            });
        });
    }
    $listJE.bindRows(__JE.JournalEntryDetails);
    $("#submit-item").on("click", function (e) {
        __JE.PostingDate = $("#post-date").val();
        __JE.DueDate = $("#due-date").val();
        __JE.DocumentDate = $("#docu-date").val();
        __JE.Remarks = $("#master-remarks").val();
        __JE.BranchID = $("#branch").val();
        //Details       
        __JE.JournalEntryDetails = $.grep($listJE.yield(), function (item) {
            return item.CodeTM !== "";
        });
        $.ajax({
            url: "/Financials/SubmitJournal",
            type: "POST",
            data: $.antiForgeryToken({ data: JSON.stringify(__JE) }),
            success: function (data) {
                if (data.Model.Action == 1) {
                    new ViewMessage({
                        summary: {
                            selector: "#error-summary"
                        }
                    }, data.Model).refresh(1500);
                }
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, data.Model);
                $(window).scrollTop(0);
            }
        });
    });
});
