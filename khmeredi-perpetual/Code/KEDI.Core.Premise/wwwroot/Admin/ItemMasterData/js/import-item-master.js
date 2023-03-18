$("#FormFile").on("change", function (e) {
    $("#preview-item-master-data-list").find("tr:not(:first-child)").remove();
    $("#filename-list").prop("disabled", true);
    $("#submit-item-master-data").prop("disabled", true);
    $("#preview-item-master-data").prop("disabled", true);
    $("#filename-list").children().remove();
    $("#preview-motocycle-list").find("tr:not(:first-child)").remove();
    loadScreen();
    $.ajax({
        url: "/ItemMasterData/GetFileNames",
        type: "POST",
        enctype: "multipart/form-data",
        data: new FormData($("#item-master-formfile")[0]),
        processData: false,
        contentType: false,
        cache: false,
        success: function (result) {
            ViewMessage({}, result.Message);
            $("#filename-list").children().remove();
            if (Object.keys(result.SheetNames).length > 0) {
                $("#filename-list").prop("disabled", false);
                $("#submit-item-master-data").prop("disabled", false);
                $("#preview-item-master-data").prop("disabled", false);

                for (let i in result.SheetNames) {
                    $("#filename-list").append("<option value='" + i + "'>" + result.SheetNames[i] + "</option>");
                }

                $("#preview-item-master-data").on("click", function () {
                    loadScreen();
                    $.ajax({
                        url: "/ItemMasterData/PreviewItemMasterData",
                        type: "POST",
                        enctype: "multipart/form-data",
                        data: new FormData($("#item-master-formfile")[0]),
                        processData: false,
                        contentType: false,
                        cache: false,
                        success: function (result) {
                            if (result.length > 0) {
                                bindPreviewData("#preview-item-master-data-list", result);
                            }
                            loadScreen(false);
                        }
                    });
                });

                $("#filename-list").on("change", function () {
                    $("#preview-item-master-data-list").find("tr:not(:first-child)").remove();
                });

                $("#submit-item-master-data").on("click", function () {
                    var $submit = this;
                    $($submit).prop("disabled", true);
                    loadScreen();
                    $.ajax({
                        url: "/ItemMasterData/Upload",
                        type: "POST",
                        enctype: "multipart/form-data",
                        data: new FormData($("#item-master-formfile")[0]),
                        processData: false,
                        contentType: false,
                        cache: false,
                        success: function (result) {
                            ViewMessage({}, result);
                            $($submit).prop("disabled", false);
                            loadScreen(false);
                        },
                        error: function (response) {
                            if (response.status === 500) {
                                ViewMessage({
                                    model: {
                                        action: "reject"
                                    }
                                }, { data: { "itemMasterData.Barcode": "Insert Item Master Data failed." } });
                            }
                            $($submit).prop("disabled", false);
                            loadScreen(false);
                        }
                    });
                });
            }
            loadScreen(false);
        }
    });
});


//Tab content customer
$("ul#nav-import-list li").click(function () {
    $(this).addClass("active").siblings().removeClass("active");
    let content = $("#tab-content-customer");
    if ($(this).is($("#tab-new-upload"))) {
        $("#content-new-upload", content).prop("hidden", false).siblings().prop("hidden", true);
    }

    if ($(this).is($("#tab-preview"))) {
        $("#content-item-master-data", content).prop("hidden", false).siblings().prop("hidden", true);
        previewComponent("/ItemMasterData/PreviewComponent");
    }
});

function loadScreen(enabled = true) {
    if (enabled) {
        $("#load-screen").show();
    } else {
        $("#load-screen").hide();
    }
}

function previewComponent(url) {
    $("#uploaded-loader").addClass("show");
    let option = {
        url: url,
        dataType: "JSON",
        success: function (data) {
            Promise.all([
                $("#im-table-pricelist").bindRows(data.PriceLists, "ID", {
                    hidden_columns: ["Currency", "CurrencyID", "Delete", "SyncDate", "CurrencyName", "Currency", "Spk", "Synced", "Cpk"],
                }),
                $("#im-table-printer").bindRows(data.PrinterNames, "ID", {
                    hidden_columns: ["MachineName", "Delete", "Cpk", "Delete", "SyncDate", "Spk", "Cpk", "Split", "Synced", "OrderCount"],
                }),

                $("#im-table-group-uom").bindRows(data.GroupUOMs, "ID", {
                    hidden_columns: ["Delete", "Spk", "SyncDate", "Synced", "Cpk"],
                }),
                $("#im-table-warehouse").bindRows(data.Warehouses, "ID", {
                    hidden_columns: ["Delete", "Address", "Branch", "Location", "StockIn", "IsAllowNegativeStock", "Cpk", "Spk", "Synced", "SyncDate", "Delete"],
                }),
                $("#im-table-item-group1").bindRows(data.ItemGroup1, "ID", {
                    hidden_columns: ["BackID", "Background", "ColorID", "Colors", "Delete", "Visible", "Images", "Delete", "File", "Cpk", "Spk", "Visible", "Synced", "IsAddon", "SyncDate"],
                }),
                $("#im-table-item-group2").bindRows(data.ItemGroup2, "ID", {
                    hidden_columns: ["BackID", "Background", "ColorID", "Colors", "Delete", "ItemGroup1", "Images", "SyncDate", "Synced", "Cpk", "Spk"],
                }),
                $("#im-table-item-group3").bindRows(data.ItemGroup3, "ID", {
                    hidden_columns: ["BackID", "Background", "ColorID", "Colors", "Delete", "ItemGroup1", "ItemGroup2", "Images", "SyncDate", "Spk", "Split", "Synced", "Cpk"],
                }),
                $("#im-table-group-defined-uom").bindRows(data.GroupDefinedUoM, "ID", {
                    hidden_columns: ["ID"]
                })
            ]);
        }
    };

    $.ajax(option);
}

$("#export-motocycle-timely-appointments").click(function () {
    let file_dialog = new DialogBox({
        caption: "Export Excel",
        content: {
            selector: "#dialog-export-timely-motocycle-appointments",
        },
        type: "ok-cancel"
    });

    file_dialog.confirm(function () {
        exportExcel("/Customer/ExportMotocycleTimelyAppointments", file_dialog);
    });

    file_dialog.reject(function () {
        this.meta.shutdown();
    });
});


function exportExcel(url, dialog) {
    let content = dialog.content;
    let file_type = content.find("select[name=FileType] option:selected").val();
    let file_name = content.find("input[name=FileName]").val();
    let _dir = {
        FileType: file_type,
        FileName: file_name
    };

    $.ajax({
        type: "POST",
        url: url,
        data: $.antiForgeryToken({ directory: _dir }),
        success: function (message) {
            new ViewMessage(message);
            if (message.Action == 1) {
                new ViewMessage({
                    summary: {
                        attribute: "model-validation-success"
                    }
                }, message);
            }
        }
    });
}

function bindPreviewData(selector, data) {
    $(selector).find("tr:not(:first-child)").remove();
    const paging = new Kernel.DataPaging({
        pageSize: 15
    });

    paging.start(data, function (brief) {
        let _index = 1;
        $(selector).bindRows(brief.data, "Code", {
            postbuild: function (item) {
                $(this).find("td:first-child").before("<td>" + _index++ + "</td>");
            }
        });
        $("#load-screen").hide();
    })
    $("#item-master-data-paging").html(paging.selfElement);
}