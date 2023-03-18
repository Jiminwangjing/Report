$(document).ready(function () {
    const data = JSON.parse($("#empty-data").text());
    let dataUpdate = {};
    let costcenters = [];
    const edfrom = moment().format();
    $("#edfrom").val(formatDate(edfrom, "YYYY-MM-DD"));

    $("#ch-ower").click(function () {
        let dialog = new DialogBox({
            content: {
                selector: "#container-list-emps"
            }
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
        dialog.invoke(function () {
            let $emps = ViewTable({
                keyField: "ID",
                selector: ".emps",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 10
                },
                visibleFields: [
                    "Name",
                    "Code",
                    "Position",
                ],
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down hover'></i>",
                        on: {
                            "click": function (e) {
                                $("#ownerName").val(e.data.Name);
                                $("#ownerNameId").val(e.data.ID);
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            onGetEmpsTemplates(function (emps) {
                if (emps.length > 0) {
                    $emps.bindRows(emps);
                    $("#txtSearch").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let rex = new RegExp(__value, "gi");
                        let items = $.grep(emps, function (item) {
                            return item.Code.toLowerCase().replace(/\s+/, "").match(rex) || item.Name.toLowerCase().replace(/\s+/, "").match(rex) || item.Position.toLowerCase().replace(/\s+/, "").match(rex);
                        });
                        $emps.bindRows(items);
                    });
                }
            });
        })
    });
    $(".ch-cct").click(function () {
        let dialog = new DialogBox({
            content: {
                selector: "#container-list-cct"
            }
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
        dialog.invoke(function () {
            let $cct = ViewTable({
                keyField: "ID",
                selector: ".cct",
                indexed: true,
                paging: {
                    enabled: true,
                    pageSize: 10
                },
                visibleFields: [
                    "CACodeType",
                    "CACodeName",
                ],
                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down hover'></i>",
                        on: {
                            "click": function (e) {
                                $("#costCenterType").val(e.data.CACodeType);
                                $("#costCenterTypeid").val(e.data.ID);
                                dialog.shutdown();
                            }
                        }
                    }
                ]
            });
            onGetCCTTemplates(function (cct) {
                if (cct.length > 0) {
                    $cct.bindRows(cct);
                    $("#txtSearchcct").on("keyup", function (e) {
                        let __value = this.value.toLowerCase().replace(/\s+/, "");
                        let rex = new RegExp(__value, "gi");
                        let items = $.grep(cct, function (item) {
                            return item.CACodeType.toLowerCase().replace(/\s+/, "").match(rex) || item.CACodeName.toLowerCase().replace(/\s+/, "").match(rex) || item.Position.toLowerCase().replace(/\s+/, "").match(rex);
                        });
                        $emps.bindRows(items);
                    });
                }
            });
        })
    });
    // save //
    $("#save").click(function () {
        const costcenter = $("#cc").val();
        const name = $("#name").val();
        const ownerNameId = $("#ownerNameId").val();
        const shortCode = $("#shortCode").val();
        const dimensionId = $("#dimensionId").val();
        const costCenterTypeId = $("#costCenterTypeId").val();
        const edfrom = $("#edfrom").val();
        const edto = $("#edto").val();
        const parentId = $("#parentId").val();
        const level = $("#level").val();
        const active = $("#active").prop("checked") ? true : false;
        data.CostCenter = costcenter;
        data.CostOfAccountingTypeID = costCenterTypeId;
        data.EffectiveFrom = edfrom;
        data.EffectiveTo = edto;
        data.OwnerEmpID = ownerNameId;
        data.Name = name;
        data.ShortCode = shortCode;
        data.Active = active;
        data.DimensionID = dimensionId;
        data.ParentID = parentId;
        data.Level = level;
        $.post("/CostOfAccounting/CreateCostOfAccounting", { costOfCenter: data }, function (res) {
            if (res.IsApproved) {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, res).refresh(1500);
            }

            ViewMessage({}, res);
        })
    });
    // update //
    $("#update").click(function () {
        const costcenter = $("#cc").val();
        const name = $("#name").val();
        const ownerNameId = $("#ownerNameId").val();
        const shortCode = $("#shortCode").val();
        const costCenterTypeId = $("#costCenterTypeId").val();
        const edfrom = $("#edfrom").val();
        const edto = $("#edto").val();
        const active = $("#active").prop("checked") ? true : false;
        dataUpdate.CostCenter = costcenter;
        dataUpdate.CostOfAccountingTypeID = costCenterTypeId;
        dataUpdate.EffectiveFrom = edfrom;
        dataUpdate.EffectiveTo = edto;
        dataUpdate.OwnerEmpID = ownerNameId;
        dataUpdate.Name = name;
        dataUpdate.ShortCode = shortCode;
        dataUpdate.Active = active;
        $.post("/CostOfAccounting/CreateCostOfAccounting", { costOfCenter: dataUpdate }, function (res) {
            if (res.IsApproved) {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, res).refresh(1500);
            }

            ViewMessage({}, res);
        })
    });
    // click create new cct //
    $("#create-cct").click(function () {
        let dialog = new DialogBox({
            content: {
                selector: "#container-list-cct-create"
            },
            caption: "Cost Center Type - Setup",
            type: "ok-cancel-",
        });
        dialog.confirm(function () {
            dialog.shutdown();
        });
        dialog.invoke(function () {
            
        })
        $("#save-cct").click(function () {
            const codecct = $("#cct-code").val();
            const codename = $("#cct-name").val();
            const data = {
                CACodeType: codecct,
                CACodeName: codename
            }
            $.post("/CostOfAccounting/CreateCCT", { costOfAccountingType: data }, function () {
                $.get("/CostOfAccounting/GetLatestCCT", function (res) {
                    $("#costCenterType").val(res.CACodeType);
                    $("#costCenterTypeId").val(res.ID);
                })
            });
            dialog.shutdown();
        });
        $("#cancel-cct").click(function () {
            //location.reload();
            dialog.shutdown();
        });
    });
    // Cancel //
    $("#cancel").click(function () {
        location.reload();
    })
    $.get("/CostOfAccounting/GetDimensions", function (res) {
        bindDimensions("#dimension", res)
    })
    $.get("/CostOfAccounting/GetCostOfCenter", function (res) {
        if (isValidArray(costcenters)) {
            costcenters = [];
            costcenters.push(res);
        } else {
            costcenters.push(res);
        }
    })
    $("#add").on("click", function () {
        let __parentId = $("#parentId").val();
        //hide the button Add
        $(this).hide();
        //show the button save
        $("#save").prop("hidden", false);
        $("#update").prop("hidden", true);
        $("#cancel").prop("hidden", false);
        setCostCenter();
        $.get("/CostOfAccounting/CreateDetailByCategory",
            { id: __parentId }, function (item) {
                $("#parentId").val(item.ParentID);
                $("#level").val(item.Level);
            }
        );
    });
    // get none //
    $.get("/CostOfAccounting/GetNone", function (res) {
        $("#noneName")
            .append(`<span data-id='${res.ID}' class="item-title">${res.Name}</span></li>`);
    })
    let $Dimension = $(".v-menu-box .menu-group");
    let $container = $(".treeview-container .treeview");
    $(".menu-item", $Dimension).on("click", function () {
        $(".fa-long-arrow-alt-right").prop("hidden", true);
        let __id = $(this).children("span").data("id");
        $("#update").prop("disabled", true);
        $("#add").prop("disabled", true);
        $("#parentId").val(__id);
        getGroup(__id);

    });
    function getGroup(parentId) {
        $.get("/CostOfAccounting/GetGroup", { id: parentId }, function (item) {
            bindGroup($container, item, "ID", true);
            $(".item", $container).on("click", function () {
                $(".menu-title-group").removeClass("white");
                $(".menu-item").removeClass("active").removeClass("white");
                $("#parentId").val(item.ID);
                $("#update").prop("disabled", false);
                //disabledRadioBox("is-title", true);
                $(".side-detail *").css("pointer-events", "auto");
                $("#add").prop("disabled", false);
                $(this).addClass("active");
                setCostCenter(item);
            });
            bindCostOfAccounting(".treeview", getItemByGroup(costcenters[0], parentId));
        });
    }
    function bindGroup(container, group, key, isRoot) {
        $(container).children().remove();
        let $treeview = $("<li data-id='" + group[key] + "' class='item'><div class='title'>" + group.Code + "-" + group.Name + "</div></li>");
        if (isRoot) {
            $treeview = $("<li data-id='" + group[key] + "' class='item'><div class='title'>" + group.Name + "</div></li>");
        }
        $(container).append($treeview);
    }
    function setCostCenter(item) {
        if (isValidJson(item)) {
            $("#dimension").val(item.Dimension);
            $("#cc").val(item.CostCenter);
            $("#name").val(item.Name);
            $("#ownerName").val(item.OwnerName);
            $("#ownerNameId").val(item.OwnerEmpID);
            $("#shortCode").val(item.ShortCode);
            $("#costCenterType").val(item.CostCenterType);
            $("#costCenterTypeId").val(item.CostCenterTypeID);
            $("#costCenterType").val(item.CostCenterType);
            $("#edfrom").val(item.EffectiveFrom != null ? formatDate(item.EffectiveFrom, "YYYY-MM-DD") : "");
            $("#edto").val(item.EffectiveTo != null ? formatDate(item.EffectiveTo, "YYYY-MM-DD") : "");
            $("#active").val(item.Active);
            $("#active").prop("checked",item.Active);
        } else {
            $("#dimension").val(false);
            $("#cc").val('');
            $("#name").val('');
            $("#ownerName").val('');
            $("#ownerNameId").val(0);
            $("#shortCode").val('');
            $("#costCenterType").val('');
            $("#costCenterTypeId").val(0);
            $("#edfrom").val(formatDate(edfrom, "YYYY-MM-DD"));
            $("#edto").val('');
            $("#active").val(false);
            $("#active").prop("checked",false);
        }
    }
    function bindDimensions(container, data) {
        if (isValidArray(data)) {
            let __ul = $(`<ul class=''></ul>`);
            data.forEach(i => {
                let __li = $(`<li class='menu-item hover'><span class='font-size'>${i.CostCenter}</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class='font-size'>${i.Name}</span ></li >`);
                let _i = $(`<i data-id='${i.ID}' class="fas fa-caret-down fs-1 font-size"></i>`);
                __li.prepend(_i);
                __ul.append(__li);

                __li.click(function () {
                    $(this).parents().find(".menu-item").removeClass("active").removeClass("white");
                    $(this).addClass("active").addClass("white");
                    $("#mainParentId").val(i.MainParentID);
                    $("#parentId").val(i.ParentID);
                    $("#add").prop("disabled", false);
                    $("#update").prop("disabled", false);
                })
                _i.click(function () {
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
                })
            })
            $(container).append(__ul);
        }
    }
    function bindCostOfAccounting(container, data) {
        if (isValidArray(data)) {
            let __ul = $(`<ul></ul>`);
            data.forEach(i => {
                let __li = $(`<li class='hover'></li>`);
                let div = $(`<div class='menu-item'></div>`)
                let spanCode = $(`<span class='font-size fm-1 txtcolor-${i.Level}' data-id='${i.ID}'>${i.CostCenter}</span>`);
                div.append(spanCode);
                __li.append(div);
                __ul.append(__li);
                bindCostOfAccounting(__li, getItemByGroup(costcenters[0], i.ID));
                div.click(function () {
                    $(this).parents().find(".menu-item").removeClass("active").removeClass("white");
                    $(this).addClass("active").addClass("white");
                    $("#mainParentId").val(i.MainParentID);
                    $("#parentId").val(i.ID);
                    $("#add").prop("disabled", false);
                    $("#update").prop("disabled", false);
                    setCostCenter(i);
                    dataUpdate = i;
                    $(this).siblings("ul").toggle(200, function () {
                        this.scrollIntoView({ behavior: "smooth", block: "center" });
                    });
                })
                //_i.on("click", function () {
                //    if ($(this).hasClass("fa-caret-right")) {
                //        $(this).removeClass("fa-caret-right");
                //        $(this).addClass("fa-caret-down");
                //    }
                //    else if ($(this).hasClass("fa-caret-down")) {
                //        $(this).removeClass("fa-caret-down");
                //        $(this).addClass("fa-caret-right");
                //    }
                    
                //});
            });
            $(container).append(__ul);
        }
    }
    function getItemByGroup(items, parentId = 1) {
        return groupBy(items, ["ParentID"])[parentId];
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
    function onGetEmpsTemplates(success) {
        $.get("/CostOfAccounting/GetEmps", success)
    }
    function onGetCCTTemplates(success) {
        $.get("/CostOfAccounting/GetCCT", success)
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
    //Util functions
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    function isValidJson(value) {
        return value !== undefined && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }
})