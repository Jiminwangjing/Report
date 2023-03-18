'use strict'
$(document).ready(function () {
    let __glAccountData;
    const glAccObjectMove = {
        id: 0,
    }
    var arr_subtypeaccount = [];
    $.ajax({
        url: "/ChartOfAccounts/AllGlAcc",
        async: false,
        type: "get",
        dataType: "JSON",
        complete: function ({ responseJSON }) {
            __glAccountData = responseJSON
        }

    });





    GetSubtype();
    function GetSubtype() {
        $.get("/ChartOfAccounts/GetSubTypeAcount", function (res) {
            if (res.length > 0) {
                var data = `<option value="0"></option>`;
                $.each(res, function (i, item) {
                    data +=
                        '<option value="' + item.ID + '">' + item.Name + '</option>';
                });
                $("#subtypeacount").html(data);
            }

        });
    }
    $("#dialog-subtype").click(() => {

        const dialog = new DialogBox({
            content: {
                selector: "#container-lis-Subtype"
            },
            type: "ok-cancel",
            button: {
                ok: {
                    text: "Save"
                }
            },
            caption: "List Subtype Acount",
        });
        dialog.invoke(function () {

            const $list_subtypeaccount = ViewTable({
                keyField: "LineID",
                selector: ".subtype",
                indexed: true,
                // dataSynced: true,
                paging: {
                    enabled: true,
                    pageSize: 20
                },
                visibleFields: ["Name", "Default"],
                columns: [
                    {
                        name: "Name",
                        template: "<input type='text'/>",
                        on: {
                            "keyup": function (e) {
                                updatedata(arr_subtypeaccount, "LineID", e.key, "Name", this.value.trim());
                                $list_subtypeaccount.disableColumns(e.key, ["Default"], false);
                            }
                        }
                    },
                    {
                        name: "Default",
                        template: "<input name='default' type='radio'/> ",
                        on: {
                            "change": function (e) {

                                const active = $(this).prop("checked") ? true : false;
                                updatedata(arr_subtypeaccount, "LineID", e.key, "Default", active);
                                if (arr_subtypeaccount.length > 0) {
                                    arr_subtypeaccount.forEach(i => {
                                        if (i.LineID == e.key)
                                            i.Default = true
                                        else
                                            i.Default = false;
                                    });
                                }
                            }
                        }
                    },
                ],
                // actions: [
                //     {
                //         template: `<input type="radio" name="default"/>`,
                //         on: {
                //             "click": function (e) {

                //                 const active = $(this).prop("checked") ? true : false;
                //                 $list_subtypeaccount.disableColumns(e.key,"Default",true);


                //                 updatedata(arr_subtypeaccount, "LineID", e.key, "Default",active);
                //                
                //             }
                //         }
                //     }
                // ]
            });
            $.get("/ChartOfAccounts/BindRowSubType", function (res) {
                $("#find-list").on("keyup", function () {

                    let keyword = this.value.replace(/\s+/g, '').trim().toLowerCase();
                    var rgx = new RegExp(keyword);
                    var searcheds = $.grep(arr_subtypeaccount, function (item) {
                        if (item !== undefined) {
                            if (item.Name != null || item.Name != "")
                                return item.Name.toLowerCase().match(keyword)

                        }
                    });
                    $list_subtypeaccount.bindRows(searcheds);

                });

                $list_subtypeaccount.bindRows(res);
                let subAccts = res.filter(sa => !sa.Name);

                for (let i = 0; i < subAccts.length; i++) {
                    $list_subtypeaccount.disableColumns(subAccts[i].LineID, ["Default"]);
                }
                $list_subtypeaccount.disableColumns(undefined,)
                arr_subtypeaccount = $list_subtypeaccount.yield();
                arr_subtypeaccount.forEach(i => {
                    if (i.Name == null)
                        i.Name = "";
                })

            });
        })
        dialog.confirm(function () {
            var arrsubtype = [];

            arr_subtypeaccount.forEach(i => {
                if (i.ID > 0) {
                    arrsubtype.push(i);
                }
                else {
                    if (i.Name != "")
                        arrsubtype.push(i);
                }

            })

            $.ajax({
                url: "/ChartOfAccounts/SaveSubTypeAccount",
                type: "POST",
                dataType: "JSON",
                data: { subTypes: arrsubtype },
                success: function (__res) {

                    var data = `<option  value="' + item.ID + '">' + item.Name + '</option>`;
                    $.each(__res.Data, function (i, item) {

                        data += '<option  value="' + item.ID + '">' + item.Name + '</option>';


                    });
                    $("#subtypeacount").html(data);
                }
            })
            dialog.shutdown();
        });
        dialog.reject(function () {
            dialog.shutdown();
        })

    })

    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function updatedata(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue)
                    i[prop] = propValue
            })
        }
    }


    $("#glcode").on("change", function () {
        const glAccMoveLocation = findObjectOfArray(__glAccountData, "ID", glAccObjectMove.id);
        if (glAccMoveLocation && this.value > 0) {
            updateGlAcc(__glAccountData, "ID", glAccMoveLocation.ID, "ParentId", this.value, true);
            getGroup(glAccMoveLocation.MainParentId);
            $("#parentId").val(this.value);
        }

    });


    $(".fa-long-arrow-alt-right").prop("hidden", true);
    let $container = $(".treeview-container .treeview");
    $("[name='GLAccount.IsTitle']").on("change", function () {
        setTitle($(this).prop("checked"));
    });
    disabledRadioBox("is-title", true);
    changeValue();
    let $GLAccount = $(".v-menu-box .menu-group");
    $(".menu-item", $GLAccount).on("click", function () {
        $("#subtypehidden").prop("hidden", true);
        resetGLAccount(this);
        $(".fa-long-arrow-alt-right").prop("hidden", true);
        let __id = $(this).data("id");
        $("#update").prop("disabled", true);
        $("#add-glaccount").prop("disabled", true);
        $("#mainParentIDNoASP").val(__id);

        $.get("/ChartOfAccounts/Getglcode", { mainId: __id }, function (res) {
            var data = '';
            $("#glcode").find('option').not(':first').remove();
            $.each(res, function (i, item) {
                data +=
                    '<option value="' + item.ID + '">' + item.Code + '</option>';
            });
            $("#glcode").append(data);
        })
        getGroup(__id);

    });
    function getGroup(parentId) {
        disabledRadioBox("is-title", true);

        $.post("/ChartOfAccounts/GetGroup", { id: parentId }, function (item) {
            bindGroup($container, item, "ID", true);
            $(".item", $container).on("click", function () {
                $(".menu-title-group").removeClass("white");
                $(".menu-item").removeClass("active").removeClass("white");
                $("#idParent").val(item.ID);
                $("#update").prop("disabled", false);
                disabledRadioBox("is-title", true);
                $(".side-detail *").css("pointer-events", "auto");
                $("#add-glaccount").prop("disabled", false);
                $(this).addClass("active");
                setGLAccount(item);
            });
            bindLevel(".treeview", getItemByGroup(__glAccountData, parentId));
        });
    }
    $("#cancel").click(function () {
        location.reload();
    })
    $("#add-glaccount").on("click", function () {
        let __parentId = $("#idParent").val()
        checkValid(".valHidden");
        //hide the button Add
        $(this).hide();
        //show the button save
        $("#save").prop("hidden", false);
        $("#update").prop("hidden", true);
        $("#cancel").prop("hidden", false);
        resetGLAccount();
        const mainParentID = $("#mainParentIDNoASP").val();
        $("#mainParentID").val(mainParentID);
        $("#code").prop('readonly', false);
        disabledRadioBox("IsTitle", false);
        $.get("/ChartOfAccounts/CreateDetailByCategory",
            { parentId: __parentId }, function (item) {
                $("#parentId").val(item.GLAccount.ParentId);
                $("#level").val(item.GLAccount.Level);
                $("#balance").val(item.GLAccount.TotalBalance == null ? 0.00 : item.GLAccount.TotalBalance);
            }
        );
    });
    checkValid(".valHidden");
    function getItemByGroup(items, parentId) {
        return groupBy(items, ["ParentId"])[parentId];
    }
    function setGLAccount(item) {
        let keys = Object.keys(item);
        for (let i = 0; i < keys.length; i++) {
            if ($("[name='GLAccount." + keys[i] + "']").attr("type") === "checkbox") {
                if (keys[i] == "IsTitle") {
                    setTitle(item[keys[i]]);
                } else {
                    $("[name='GLAccount." + keys[i] + "']").prop("checked", item[keys[i]]).val(item[keys[i]]);
                }

            } else {
                $("[name='GLAccount." + keys[i] + "']").val(item[keys[i]]);
            }
        }
    }
    function setTitle(value) {
        $("[name='GLAccount.IsTitle']").prop("checked", value).val(value);
        if (value) {
            $(".section-not-title").css("display", "none");
            $("[name='GLAccount.CurrencyID']").val(0).prop("disabled", true).children().remove();
        } else {
            $(".section-not-title").css("display", "block");
        }
    }

    function resetGLAccount($GLAccount = undefined) {

        $("#parent-id").val(0);
        $("#code").val("");
        $("#name").val("");
        $("#externalCode").val("");
        $("#parentId").val("");
        $("#currId").val("");
        $("#level").val("");
        $("#balance").val(1);
        $("#isConfidential").val(false);
        $("#isControlAccount").val(false);
        $("#isIndexed").val(false);
        $("#isCashAccount").val(false);
        $("#blockManualPosting").val(false);
        $("#cashFlowRelavant").val(false);
        $("#accountType").val(0);
        if ($GLAccount) {
            $($GLAccount).children(".item-title").addClass("active")
                .parents().siblings().children(".item-title").removeClass("active");
        }
    }
    function bindGroup(container, group, key, isRoot) {
        $(container).children().remove();
        let $treeview = $("<li data-id='" + group[key] + "' class='item'><div class='title'>" + group.Code + "-" + group.Name + "</div></li>");
        if (isRoot) {
            $treeview = $("<li data-id='" + group[key] + "' class='item'><div class='title'>" + group.Name + "</div></li>");
        }
        $(container).append($treeview);
    }

    function bindLevel(container, group) {
        if (isValidArray(group)) {
            let __ul = $(`<ul class='menu-item-list'></ul>`);
            group.forEach(i => {

                let _li = $("<li class='ml-5'$></li>");
                let __div = $(`<div data-code='${i.ID}' data-parentid='${i.MainParentId}' class='menu-item'></div>`);
                let $treeview = $(`<span  class='menu-title-group menu-title menu-dropdown-arrow-up'> ${i.Code} - ${i.Name}</span>`);
                let _i = $(`<i data-id='${i.ID}' class="fas fa-caret-down fm-1"></i>`);
                if (i.IsActive === true && i.IsTitle === false) {
                    _i = $(`<i data-id='${i.ID}' class="fm-1"></i>`);
                }
                if (i.IsActive === false && i.IsTitle === true) {
                    $treeview.addClass("text-primary");
                    _i.addClass("text-primary");
                }
                __div.append(_i);
                __div.append($treeview);
                _li.append(__div);
                __ul.append(_li)
                const __id = $(__div).data("code");
                if (i.SetActiveClass) __div.addClass("active").addClass("white");

                if (i.IsActive) {
                    __div.dblclick(function () {
                        let $dialog = new DialogBox({
                            content: {
                                selector: "#delete-glAcc"
                            },
                            type: "ok-cancel",
                            caption: "Delete Account!", // header
                            //content: `Are you sure you want to delete ${i.Code}-${i.Name}?`    // body
                        });
                        $dialog.invoke(function () {
                            $("#text-delete-glAcc-content").text(`Are you sure you want to delete ${i.Code}-${i.Name}?`)
                        })
                        $dialog.reject(function () {
                            $dialog.shutdown();
                        })
                        $dialog.confirm(function () {
                            $.post("/ChartOfAccounts/DeleteglAccount", { id: i.ID, code: i.Code },
                                function (res) {
                                    ViewMessage({
                                        summary: {
                                            selector: "#text-delete-glAcc"
                                        }
                                    }, res)
                                    if (res.IsApproved) {
                                        setTimeout(function () {
                                            $dialog.shutdown();
                                            $("#text-delete-glAcc").text("");
                                        }, 2000)
                                        location.reload();
                                    }
                                });
                        });
                    })
                }

                __div.on("click", function () {
                    $("#glcode").val(0);
                    $(this).parents().find(".menu-item").removeClass("active").removeClass("white");
                    $(this).addClass("active").addClass("white");
                    $(this).parents().children().find(".menu-title-group").removeClass("white");
                    $(this).parents().children().find(".fm-1").removeClass("white");
                    $(this).children("span").addClass("white");
                    $(this).children("i").addClass("white");
                    $(".item", ".treeview").removeClass("active");
                    $("#update").prop("disabled", false);
                    //$("#code").prop("readonly", true);
                    checkValid(".valHidden", true);
                    //disabledRadioBox("IsTitle", true);
                    $(".side-detail *").css("pointer-events", "auto");
                    $("#add-glaccount").prop("disabled", false);
                    $("#idParent").val(i.ID);
                    $("#GLAccID").val(i.ID);
                    if (i.IsTitle) {
                        $("#istitle").prop("checked", true);
                        $("#isactive").prop("checked", false);
                        hide();
                    }
                    else if (i.IsActive) {
                        $("#isactive").prop("checked", true);
                        $("#istitle").prop("checked", false);
                        glAccObjectMove.id = i.ID;
                        show();
                        let parentid = parseInt($(this).attr('data-parentid'));
                        if (parentid == 6) {
                            $("#subtypehidden").prop("hidden", false);
                        }
                        else {
                            $("#subtypehidden").prop("hidden", true);
                        }
                        //let mainid=data
                    }
                    if (i.Edit) {
                        $("#code").prop("readonly", false);
                        disabledRadioBox("IsTitle", false);
                    }
                    else {
                        if (i.IsTitle) {
                            $("#code").prop("readonly", false);
                            disabledRadioBox("IsTitle", true);
                        }
                        else {
                            $("#code").prop("readonly", true);
                            disabledRadioBox("IsTitle", true);
                        }
                    }
                    $.ajax({
                        url: "/ChartOfAccounts/GlAccBalance",
                        type: "get",
                        dataType: "JSON",
                        data: { id: i.ID },
                        success: function (res) {
                            if (res.show === 1) {
                                $(".fa-long-arrow-alt-right").prop("hidden", false);
                            } else {
                                $(".fa-long-arrow-alt-right").prop("hidden", true);
                            }
                        }
                    })
                    setGLAccount(i);
                })
                _i.on("click", function () {
                    if ($(this).hasClass("fa-caret-right")) {
                        $(this).removeClass("fa-caret-right");
                        $(this).addClass("fa-caret-down");
                    }
                    else if ($(this).hasClass("fa-caret-down")) {
                        $(this).removeClass("fa-caret-down");
                        $(this).addClass("fa-caret-right");
                    }
                    $(this).parent().siblings("ul").toggle(200, function () {
                        this.scrollIntoView({ behavior: "smooth", block: "center" });
                    });
                    checkValid(".valHidden", true);
                });
                bindLevel(_li, getItemByGroup(__glAccountData, __id));
                $(container).append(__ul);
            });
            $("#isactive").change(function () {

                GetSubtype();
            });
        }
    }

    $(".fa-long-arrow-alt-right").click(function () {
        let dialog = new DialogBox({
            content: {
                selector: "#active-gl-content"
            }
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
        dialog.invoke(function () {
            let $glAccBalance = ViewTable({
                keyField: "ID",
                selector: ".allw-gl",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 10
                },
                visibleFields: [
                    "PostingDate",
                    "Code",
                    "OriginNo",
                    "OffsetAccount",
                    "Details",
                    "CumulativeBalance",
                    "Debit",
                    "Credit",
                ],
            });
            onGetGLAccBalanceTemplates(function (glAccBalance) {
                $glAccBalance.clearRows();
                $glAccBalance.bindRows(glAccBalance);
            });
            $("#dateFrom").change(function () {
                const id = $("#GLAccID").val();
                const dateFrom = $(this).val();
                const dateTo = $("#dateTo").val();
                $.get("/ChartOfAccounts/GetGlAccBalanceGLID", { id: parseInt(id), dateFrom, dateTo }, function (res) {
                    $glAccBalance.clearRows();
                    $glAccBalance.bindRows(res);
                });
            });
            $("#dateTo").change(function () {
                const id = $("#GLAccID").val();
                const dateFrom = $("#dateFrom").val();
                const dateTo = $(this).val();
                $.get("/ChartOfAccounts/GetGlAccBalanceGLID", { id: parseInt(id), dateFrom, dateTo }, function (res) {
                    $glAccBalance.clearRows();
                    $glAccBalance.bindRows(res);
                });
            });
        })
    })
    let startDate = moment().startOf('month');
    let nextDate = moment().endOf('month');
    $("#dateFrom").val(formatDate(startDate, "YYYY-MM-DD"));
    $("#dateTo").val(formatDate(nextDate, "YYYY-MM-DD"));
    const dateFrom = $("#dateFrom").val();
    const dateTo = $("#dateTo").val();

    function onGetGLAccBalanceTemplates(success) {
        const id = $("#GLAccID").val();
        $.get("/ChartOfAccounts/GetGlAccBalanceGLID", { id: parseInt(id), dateFrom, dateTo }, success);
    }
    function disabledRadioBox(selectorName, disable = false) {
        let title = document.getElementsByName(selectorName)
        for (let i = 0; i < title.length; i++) {
            $(title[i]).prop("disabled", disable);
        }
    }
    function changeValue() {
        $("[name='IsTitle']").on("change", function () {
            switch (this.value.toLowerCase()) {
                case "true":
                    hide();
                    break;
                case "false":
                    show();
                    break;
            }
        });
    }

    function hide() {

        $("#hide-detail").hide();
        $("#hide-balance").hide();
        $("#Account_Location").hide();
    }
    function show() {

        $("#hide-detail").show();
        $("#hide-balance").show();
        $("#Account_Location").show();
    }
    //onchnage input[type:checkbox]
    $("[type='checkbox']").change(function () {
        let a = $(this).val(this.checked);
        if (a.val() === "false") {
            let name = $(this).attr("name");
            $("input[name='" + name + "']").val(false);
        }
    });


    //Util functions
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function checkValid(selector, option = false) {
        $(selector).prop("hidden", option);
    }
    function isValidJSON(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }

    function groupBy(values, keys, process) {
        let grouped = {};
        $.each(values, function (k, a) {
            keys.reduce(function (o, k, i) {
                o[a[k]] = o[a[k]] || (i + 1 === keys.length ? [] : {});
                return o[a[k]];
            }, grouped).push(a);
        });
        if (!!process && typeof process === "function") {
            process(grouped);
        }
        return grouped;
    }

    function formatDate(value, format) {
        let dt = new Date(value),
            objFormat = {
                MM: ("0" + (dt.getMonth() + 1)).slice(-2),
                DD: ("0" + dt.getDate()).slice(-2),
                YYYY: dt.getFullYear()
            },
            dateString = "";

        let dateFormats = format.split("-");
        for (let i = 0; i < dateFormats.length; i++) {
            dateString += objFormat[dateFormats[i]];
            if (i < dateFormats.length - 1) {
                dateString += "-";
            }
        }

        return dateString;
    }

    function findObjectOfArray(values, keyName, keyValue) {
        if (isValidArray(values)) {
            return $.grep(values, function (item) {
                return item[keyName] == keyValue; // item.ID == 2
            })[0];
        }
    }
    const updateGlAcc = (data, keyProp, keyValue, updateProp, updateValue, setActiveClass = false) => {
        if (isValidArray(data)) {
            for (var i of data) {
                if (i[keyProp] == keyValue) {
                    if (setActiveClass) {
                        i["SetActiveClass"] = true
                        i[updateProp] = updateValue
                    } else {
                        i[updateProp] = updateValue
                    }
                }
                else {
                    i["SetActiveClass"] = false
                }
            }
        }
    }
});