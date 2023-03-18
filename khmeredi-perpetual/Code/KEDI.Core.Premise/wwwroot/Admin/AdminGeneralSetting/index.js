$(document).ready(function () {
    let __data = [];
    let $glRevenCode = ViewTable({
        keyField: "LineID",
        selector: ".allw-decimal",
        indexed: true,
        paging: {
            enabled: false,
            pageSize: 10
        },
        visibleFields: [
            "Currency",
            "Amounts",
            "Prices",
            "Rates",
            "Quantities",
            "Percent",
            "Units",
            "DecimalSeparator",
            "ThousandsSep",
        ],
        columns: [
            {
                name: "Currency",
                template: "<input type='text' readonly>",
                on: {
                    "change": function (e) {
                        updateDetails(__data, "LineID", e.key, "DisplayCurrencyID", this.value);
                    }
                }
            },
            {
                name: "Amounts",
                template: "<input class='input-box-kernel' type='number'>",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data, "LineID", e.key, "Amounts", this.value);
                    }
                }
            },
            {
                name: "Prices",
                template: "<input class='input-box-kernel' type='number'>",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data, "LineID", e.key, "Prices", this.value);
                    }
                }
            },
            {
                name: "Rates",
                template: "<input class='input-box-kernel' type='number'>",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data, "LineID", e.key, "Rates", this.value);
                    }
                }
            },
            {
                name: "Quantities",
                template: "<input class='input-box-kernel' type='number'>",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data, "LineID", e.key, "Quantities", this.value);
                    }
                }
            },
            {
                name: "Percent",
                template: "<input class='input-box-kernel' type='number'>",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data, "LineID", e.key, "Percent", this.value);
                    }
                }
            },
            {
                name: "Units",
                template: "<input class='input-box-kernel' type='number'>",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data, "LineID", e.key, "Units", this.value);
                    }
                }
            },
            {
                name: "DecimalSeparator",
                template: "<input class='input-box-kernel'>",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data, "LineID", e.key, "DecimalSeparator", this.value);
                    }
                }
            },
            {
                name: "ThousandsSep",
                template: "<input class='input-box-kernel'>",
                on: {
                    "keyup": function (e) {
                        updateDetails(__data, "LineID", e.key, "ThousandsSep", this.value);
                    }
                }
            }
        ]
    });
    onGetDisplayTemplates(function (glex) {
        $glRevenCode.bindRows(glex);
        glex.forEach(i => {
            __data.push(i);
        });
    });
    function onGetDisplayTemplates(succuss) {
        $.get("/GeneralSettingAdmin/GetDisplay", succuss);
    }
    function updateDetails(data, keyField, keyValue, prop, propValue) {
        if (isValidArray(data)) {
            data.forEach(i => {
                if (i[keyField] === keyValue)
                    i[prop] = propValue
            })
        }
    }
    function isValidArray(values) {
        return Array.isArray(values) && values.length > 0;
    }
    $("#update-display").click(function () {
        $.ajax({
            url: "/GeneralSettingAdmin/CreateDisplay",
            type: "POST",
            dataType: "JSON",
            data: { data: JSON.stringify(__data) },
            success: function (respones) {
                if (respones.IsApproved) {
                    ViewMessage({
                        summary: {
                            selector: ".display-message"
                        }
                    }, respones).refresh(1300);
                } else {
                    ViewMessage({
                        summary: {
                            selector: ".display-message"
                        }
                    }, respones);
                }
            }
        })
    });
    $("#cancel-display").click(function () {
        location.reload();
    })

    // //============Create Color Setting ===============
    const obj = {
        ID: 0,
        EmployeeID: 0,
        Skin: false,
    }
    var Skin = false;

    $("#checkbox1").click(function () {
        $('#checkbox1 :checkbox').change(function () {
            if ($(this).is(':checked')) {
                Skin = true;
            }
            else {
                Skin = false;
            }
            $.ajax({
                url: "/GeneralSettingAdmin/UpdateUserSkin",
                type: "POST",
                dataType: "JSON",
                data: { skin: Skin },
                success: function (respones) {
                    console.log('skin', Skin);
                    console.log(respones)
                    if (respones.IsApproved) {
                        ViewMessage({
                            summary: {
                                selector: ".display-message"
                            }
                        }, respones).refresh(1300);
                    } else {
                        ViewMessage({
                            summary: {
                                selector: ".display-message"
                            }
                        }, respones);
                    }
                }
            });

        })


    })


    //======================Create ColorSetting ===================

    ////////////GetColor via userID///////////////////////

    $.ajax({
        url: "/GeneralSettingAdmin/GetSS",
        type: "get",
        datatype: "JSON",
        success: function (respones) {
            $(".item-title").css({ color: respones.FontColor });
            $(".ck-widget-container .widget-header").css({ backgroundColor: respones.BackgroundColor })
            $(".ck-widget-container .widget-body .widget-content .content-toolbar").css({ backgroundColor: respones.BackgroundColor });
            $(".ck-widget-container .widget-body .widget-sidemenu").css({ backgroundColor: respones.BackgroundColor });
            $(".ck-widget-container .widget-body .widget-sidemenu .sidemenu-group.group-root").css({ backgroundColor: respones.BackgroundColor });
            $(".ck-widget-container .widget-body .widget-sidemenu .sidemenu-group .sidemenu-item .item-title, .ck-widget-container .widget-body .widget-sidemenu .sidemenu-group .sidemenu-item .item-title.title-icon-open").css({ backgroundColor: respones.BackgroundColor })
            $(".itemsearchGloble").css({ backgroundColor: respones.BackgroundColor, color: respones.FontColor });
            $(".ChangeColorLanguage").css({ backgroundColor: respones.BackgroundColor, color: respones.FontColor })
            //BackgroundMenu
            $(".item-title").css({ backgroundColor: respones.BackgroundMenu });
            //BackOfSubmenu
            $(" .sidemenu-group").css({ backgroundColor: respones.BackOfSubmenu });
            $(".sidemenu-group, .sidemenu-item:hover").css({ backgroundColor: respones.BackOfSubmenu });
            //HoverBackgroundSubmenu
            $(".item-title").hover(function () {
                $(this).css({ backgroundColor: respones.HoverBackgSubmenu });
            }, function () {
                $(this).css({ backgroundColor: respones.BackgroundMenu });
            });
            $(".item-label").hover(function () {
                $(this).css({ backgroundColor: respones.HoverBackgSubmenu });
            }, function () {
                $(this).css({ backgroundColor: respones.BackOfSubmenu });
            });
            //BacksubmenuOnItem 
            $(".ck-widget-container .widget-body .widget-sidemenu .sidemenu-group .sidemenu-item .item-label.highlight").css({ backgroundColor: respones.BacksubmenuOnItem });
            $(".ck - widget - container.widget - body.widget - sidemenu.sidemenu - group.sidemenu - item.item - label.highlight").css({ backgroundColor: respones.BacksubmenuOnItem })
            //FontColor
            $(".ck-widget-container .widget-body .widget-sidemenu .sidemenu-group .sidemenu-item .item-title>*, .ck-widget-container .widget-body .widget-sidemenu .sidemenu-group .sidemenu-item .item-label>*").css({ color: respones.FontColor });
            $(".combobox-trigger").css({ color: respones.FontColor });
            $(".ck-widget-container .widget-combobox .combobox-trigger").css({ color: respones.FontColor });
            $(".ck-widget-container .widget-sidemenu-trigger").css({ color: respones.FontColor });
            $(".ck-widget-container .widget-body .widget-sidemenu .sidebar-title").css({ color: respones.FontColor });
            // $("input, select, textarea, .form-control").css({ color: respones.FontColor });
            //$(".ck-widget-container .widget-body .widget-content .content-section").css({ color: respones.FontColor });
            // $(".btn.btn-kernel").css({ color: respones.FontColor });
            //Button
            $(".btn.btn-kernel").css({ backgroundColor: respones.BackgButton });
            //table th
            $("table, tr, th").css({ backgroundColor: respones.Backgtableth });
            //table td
            $("table, tr, td").css({ backgroundColor: respones.Backgtabletd });
            $("table tr td").css({ backgroundColor: respones.Backgtabletd })
            $(".form-group .form-label").css({ backgroundColor: respones.Backgtabletd })
            //input
            $("input").css({ backgroundColor: respones.backgroundInput })
            //backgroundCard
            $(".widget-tab>.tab-sheet>.tab-content").css({ backgroundColor: respones.BackgroundCard });
            $(".widget-stackcrumb>*").css('color', respones.FontColor);
            //backgroundBar
            $("#themes").click(function () {
                $("#themes").css({ backgroundColor: respones.BackgroundBar })
                $("#displaysthem").css({ backgroundColor: respones.BackgroundBarItem })
                $("#displaycolor").css({ backgroundColor: respones.BackgroundBarItem })
                $("#CardMember").css({ backgroundColor: respones.BackgroundBarItem })
                $("#templatess").css({ backgroundColor: respones.BackgroundBarItem })
                $("#templatess").css({ backgroundColor: respones.BackgroundBarItem })
                $("#fonttheme").css({ backgroundColor: respones.BackgroundBarItem })
            })
            $("#displaysthem").click(function () {
                $("#displaysthem").css({ backgroundColor: respones.BackgroundBar })
                $("#themes").css({ backgroundColor: respones.BackgroundBarItem })
                $("#displaycolor").css({ backgroundColor: respones.BackgroundBarItem })
                $("#CardMember").css({ backgroundColor: respones.BackgroundBarItem })
                $("#templatess").css({ backgroundColor: respones.BackgroundBarItem })
                $("#fonttheme").css({ backgroundColor: respones.BackgroundBarItem })

            })
            $("#displaycolor").click(function () {
                $("#displaycolor").css({ backgroundColor: respones.BackgroundBar })
                $("#displaysthem").css({ backgroundColor: respones.BackgroundBarItem })
                $("#themes").css({ backgroundColor: respones.BackgroundBarItem })
                $("#CardMember").css({ backgroundColor: respones.BackgroundBarItem })
                $("#templatess").css({ backgroundColor: respones.BackgroundBarItem })
                $("#fonttheme").css({ backgroundColor: respones.BackgroundBarItem })

            })
            $("#CardMember").click(function () {
                $("#CardMember").css({ backgroundColor: respones.BackgroundBar })
                $("#themes").css({ backgroundColor: respones.BackgroundBarItem })
                $("#displaycolor").css({ backgroundColor: respones.BackgroundBarItem })
                $("#displaysthem").css({ backgroundColor: respones.BackgroundBarItem })
                $("#templatess").css({ backgroundColor: respones.BackgroundBarItem })
                $("#fonttheme").css({ backgroundColor: respones.BackgroundBarItem })

            })
            $("#templatess").click(function () {
                $("#templatess").css({ backgroundColor: respones.BackgroundBar })
                $("#themes").css({ backgroundColor: respones.BackgroundBarItem })
                $("#displaycolor").css({ backgroundColor: respones.BackgroundBarItem })
                $("#displaysthem").css({ backgroundColor: respones.BackgroundBarItem })
                $("#CardMember").css({ backgroundColor: respones.BackgroundBarItem })
                $("#fonttheme").css({ backgroundColor: respones.BackgroundBarItem })

            })

            $("#GeneralDashboard").click(function () {
                $("#GeneralDashboard").css({ backgroundColor: respones.BackgroundBar })
                $("#CRMDashboard").css({ backgroundColor: respones.BackgroundBarItem })
                $("#ServiceDashboard").css({ backgroundColor: respones.BackgroundBarItem })
                $("#DashboardIncome").css({ backgroundColor: respones.BackgroundBarItem })
            })
            $("#CRMDashboard").click(function () {
                $("#CRMDashboard").css({ backgroundColor: respones.BackgroundBar })
                $("#GeneralDashboard").css({ backgroundColor: respones.BackgroundBarItem })
                $("#ServiceDashboard").css({ backgroundColor: respones.BackgroundBarItem })
                $("#DashboardIncome").css({ backgroundColor: respones.BackgroundBarItem })
            })
            $("#ServiceDashboard").click(function () {
                $("#ServiceDashboard").css({ backgroundColor: respones.BackgroundBar })
                $("#CRMDashboard").css({ backgroundColor: respones.BackgroundBarItem })
                $("#GeneralDashboard").css({ backgroundColor: respones.BackgroundBarItem })
                $("#DashboardIncome").css({ backgroundColor: respones.BackgroundBarItem })
            })
            $("#DashboardIncome").click(function () {
                $("#DashboardIncome").css({ backgroundColor: respones.BackgroundBar })
                $("#CRMDashboard").css({ backgroundColor: respones.BackgroundBarItem })
                $("#ServiceDashboard").css({ backgroundColor: respones.BackgroundBarItem })
                $("#GeneralDashboard").css({ backgroundColor: respones.BackgroundBarItem })
            })

            //slideBar - title

            // if ($(".active").length > 0) {
            //     $(".widget-tab>.tab-sheet.active>.tab-title").css({ backgroundColor: respones.BackgroundBar }

            $("input.form-control, select.form-control, select.form-control:not([size]):not([multiple])").css({ backgroundColor: respones.BackgSlideBarTitle });
            //icon
            $(".ck-notification.orange .n-icon path").css({ fill: respones.BackgroundIcon });
            $(".ck-notification .n-icon path").css({ fill: respones.BackgroundIcon });
            $(".tab-title").on('click', 'li', function () {
                $(".tab-title").css({ backgroundColor: respones.BackgroundBar });
                $(this).addClass('active');
            })
            //bodywet
            $(".ck-widget-container .widget-body .widget-content .content-section").css({ backgroundColor: respones.BackgBodyWet })
            //card
            $(".card").css({ backgroundColor: respones.BackgBodyWetCard });
            $(".ck-widget-container .widget-body .widget-content .content-section>*").css({ backgroundColor: respones.BackgBodyCard })
            $(".widget-stackcrumb>*").css('color', respones.FontColor);
        }
    })

    //------------------------------------Create FontSetting-------------------------

    const _Data = {
        ID: 0,
        FontName: "",
        FontSize: "",
    }
    var exists = false;
    FontSize = $("#inputfontsize").val();
    FontName = $("#inputfont").val();

    $("#inputfont").change(function () {
        // FontName = $(this).val();
        if (FontName == 31) {
            FontName = "Arial";
        }
        if (FontName == 32) {
            FontName = "Verdana";
        }
        if (FontName == 33) {
            FontName = "Calibri";
        }
        if (FontName == 35) {
            FontName = "system-ui";
        }
        if (FontName == 36) {
            FontName = "cursive";
        }
        if (FontName == 37) {
            FontName = "serif";
        }
        if (FontName == 38) {
            FontName = "Georgia, serif";
        }
        if (FontName == 39) {
            FontName = "Time New Roman";
        }
        if (FontName == 34) {
            FontName = "cursive";
        }
    });

    $("#inputfontsize").change(function () {
        FontSize = $(this).val();
        if (FontSize == 32) {
            FontSize = "14";
        }
        if (FontSize == 33) {
            FontSize = "15";
        }
        if (FontSize == 34) {
            FontSize = "16";
        }
        if (FontSize == 35) {
            FontSize = "17";
        }
        if (FontSize == 36) {
            FontSize = "18";
        }
        if (FontSize == 37) {
            FontSize = "19";
        }
        if (FontSize == 38) {
            FontSize = "20";
        }
        if (FontSize == 39) {
            FontSize = "21";
        }
        if (FontSize == 31) {
            FontSize = "13"
        }
    });
    $("#update-Font").click(function () {
        if (exists == true) {
            fontName = $("#inputfont").find(":selected").val();
            fontSize = $("#inputfontsize").find(":selected").val();
            _Data.Active = exists;
        }
        _Data.ID = 0;
        _Data.FontName = $("#inputfont").val();
        _Data.FontSize = $("#inputfontsize").val();
        _Data.Active = exists;

        $.ajax({
            url: "/GeneralSettingAdmin/AddFontSetting",
            type: "POST",
            dataType: "JSON",
            data: { data: _Data },
            success: function (respones) {
                if (respones.IsApproved) {
                    ViewMessage({
                        summary: {
                            selector: ".display-message"
                        }
                    }, respones).refresh(3000);
                } else {
                    ViewMessage({
                        summary: {
                            selector: ".display-message"
                        }
                    }, respones);
                }
            }
        });

    });
    $.ajax({
        url: "/GeneralSettingAdmin/getFont",
        type: "GET",
        dataType: "JSON",
        success: function (respones) {
            var active = respones.FontName;
            var actives = respones.FontSize;
            active = $("#inputfont").val(respones.FontName);
            actives = $("#inputfontsize").val(respones.FontSize);
            // $("#fontsettingid").val(respones.ID);
            // $("#inputfontsize").append(`<option value=${respones.ID} selected> ${respones.FontSize} </option>`);
            // $("#inputfont").append(`<option value=${respones.ID} selected> ${respones.FontName} </option>`);
            $(".ck-widget-container .widget-body .widget-sidemenu .sidemenu-group .sidemenu-item").css('font-family', respones.FontName);
            $(".font-size ").css('font-family', respones.FontName);
            $("table tr").css('font - family', respones.FontName);
            $('table tr th').css('font-family', respones.FontName);
            $('table tr td').css('font-family', respones.FontName);
            $(".widget-tab").css('font-family', respones.FontName);
            $(".font-size").css('font-family', respones.FontName);

            $("table").css('font-family', respones.FontName);
            $(".widget-stackcrumb>*").css('font-family', respones.FontName);
            $(".ck-widget-container .widget-combobox .combobox-trigger").css('font-family', respones.FontName);
            $("input,button,select,optgroup,textarea").css('font-family', respones.FontName);
            $(".ck-widget-container .widget-body .widget-content .content-section").css('font-family', respones.FontName);
            $('.ck-widget-container .widget-body .widget-sidemenu .sidebar-title').css('font-family', respones.FontName);
            $(".widget-tab>.tab-sheet>.tab-title").css('font-family', respones.FontName);
            //$("input.form-control,select.form - control,select.form - control: not([size]): not([multiple]) ").css('font-family', respones.FontTimeNewRoman);
            $(".widget-tab>.tab-sheet.active>.tab-title").css('font-family', respones.FontName);
            $(".widget-stackcrumb>*").css('font-family', respones.FontName);
            //Change Font Size
            $(".ck-widget-container .widget-body .widget-sidemenu .sidemenu-group .sidemenu-item").css('font-size', (parseInt(respones.FontSize) + "px"));
            $(".ck-widget-container .widget-body .widget-sidemenu .sidemenu-group .sidemenu-item .item-label>*").css('font-size', (parseInt(respones.FontSize) + "px"));
            $(".font-size ").css('font-size', (parseInt(respones.FontSize) + "px"));
            $("table tr").css('font - size', (parseInt(respones.FontSize) + "px"));
            $('table tr th').css('font-size', (parseInt(respones.FontSize) + "px"));
            $('table tr td').css('font-size', (parseInt(respones.FontSize) + "px"));
            $(".widget-tab").css('font-size', (parseInt(respones.FontSize) + "px"));
            $("td").css('font-size', (parseInt(respones.FontSize) + "px"));
            $("select").css('font-size', (parseInt(respones.FontSize) + "px"));
            $("input").css('font-size', (parseInt(respones.FontSize) + "px"));
            $("table tr th, table tr td").css('font-size', (parseInt(respones.FontSize) + "px"));
            $(".widget-stackcrumb>*").css('font-size', (parseInt(respones.FontSize) + "px"));
            $(".dx-widget").css('font-size', (parseInt(respones.FontSize) + "px"));
            //$("input.form-control,select.form - control,select.form - control: not([size]): not([multiple]) ").css('font-size', respones.FontSize + "em");
            $(".ck-widget-container .widget-combobox .combobox-trigger").css('font-size', (parseInt(respones.FontSize) + "px"));
            $("input,button,select,optgroup,textarea").css('font-size', (parseInt(respones.FontSize) + "px"));
            $(".ck-widget-container .widget-body .widget-content .content-section").css('font-size', (parseInt(respones.FontSize) + "px"));
            $('.cidget-tab>.tab-sheet>.tab-title').css('font-size', (parseInt(respones.FontSize) + "px"));
            $(".wk-widget-container .widget-body .widget-sidemenu .sidebar-title").css('font-size', (parseInt(respones.FontSize) + "px"));
            $(".widget-tab>.tab-sheet.active>.tab-title").css('font-size', (parseInt(respones.FontSize) + "px"));

        }
    })





});