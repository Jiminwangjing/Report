$.each($("[data-year]"), function (i, y) {
    $(y).yearpicker();
});

//Type
$("select#typeid_add").on("change", function (e) {
    dialogUpdateData("/KVMS/GetAutoType", "/KVMS/UpdateAutoType",
        "TypeID", "TypeName", this, "List of Types", true);
});
//Brand
$("select#brandid_add").on("change", function (e) {
    dialogUpdateData("/KVMS/GetAutoBrand", "/KVMS/UpdateAutoBrand",
        "BrandID", "BrandName", this, "List of brands", true);
});
//Model
$("select#modelid_add").on("change", function (e) {
    dialogUpdateData("/KVMS/GetAutoModel", "/KVMS/UpdateAutoModel",
        "ModelID", "ModelName", this, "List of models", true);
});
//Color
$("select#colorid_add").on("change", function (e) {
    dialogUpdateData("/KVMS/GetAutoColor", "/KVMS/UpdateAutoColor",
        "ColorID", "ColorName", this, "List of colors", true);
});

var selected_value = 0;
function dialogUpdateData(from_url, to_url, key, name, select, dialog_title, has_active, related) {
    if ($(select).val() == "-1") {
        $(select).val(selected_value);
        let define_new = new DialogBox({
            position: "top-center", 
            content: {
                selector: "#dialog-define-new"
            },
            caption: dialog_title,
            type: "ok-cancel",
            button: {
                cancel: {
                    callback: function (e) {
                        this.meta.shutdown();
                    }
                }
            }
           
        });

        define_new.startup("during", function (dialog) {
            let new_index = 1;
            let table = dialog.content.find("table");
            let rel_key = "";
            if (!!related) {
                rel_key = Object.keys(related)[0];
            }
            if (!!has_active) {
                $("tr:first-child th:last-child", table).prop("hidden", false);
            }

            if (!!related) {
                $("tr:first-child th:last-child", table).after("<th style='width: 25px;'>Related</th>");
            }

            let add_new = $("<tr><td class='add-new fas fa-plus-circle fa-lg csr-pointer fn-sky' style='padding-top:6px;'></td></tr>").on("click", function (e) {
                if ($(this).prev().find("input[name='Name']").val() !== "") {
                    new_index++;
                    let row = $("<tr data-" + key + "=0><td>" + new_index + "</td><td><input name='Name' class='form-control create-new'/></td></tr>");
                    if (!!has_active) {
                        row.append("<td><input name='Active' type='checkbox' checked></td>");
                    }

                    if (!!related) {
                        row.append("<td><input name='Related' class='form-control' readonly disabled value='" + related[Object.keys(related)][0] + "'></td>");
                    }
                    $(this).before(row);
                }

                table.parent().scrollTop(table[0].scrollHeight);
            });

            $.ajax({
                url: from_url,
                dataType: "JSON",
                data: { related_id: !!related ? related[rel_key] : 0 },
                success: function (results) {
                    if (results.length > 0) {
                        let items = [];
                        $.each(results, function (index, data) {
                            index++;
                            let item = {};
                            item[key] = data[key];
                            item["index"] = index;
                            item[name] = "<input name='Name' value='" + data[name] + "' class='form-control' readonly>";

                            if (!!related) {
                                item[rel_key] = "<div name='Related'>" + related[rel_key] + "</div>";
                            }

                            if (!!has_active) {
                                item["active"] = "<input name='Active' type='checkbox'>";

                                if (!!data.Active) {
                                    item.active = "<input name='Active' type=checkbox checked>";
                                }
                            }

                            items.push(item);
                            new_index = index + 1;
                        });

                        $.bindrows(table, items, key, {
                            hide_key: true,
                            dblclick: function (e) {
                                let current_input = $("td input[name=name]:not(.create-new)", this).prop("readonly", false);
                                let sibling_inputs = current_input.parent().parent().siblings().find("td input[name=name]:not(.create-new)");
                                sibling_inputs.prop("readonly", true);
                            }
                        });

                        $(document).on("click", function (e) {
                            if (!$(e.target).is(table.find("tr *"))) {
                                table.find("tr td input:not(.create-new)").prop("readonly", true);
                            }

                        });
                    }

                    let create_new = $("<tr data-" + key + "=0><td>" + new_index + "</td><td><input name='Name' autocomplete='off' class='create-new form-control' /></tr>");

                    if (!!has_active) {
                        create_new.append("<td><input name='Active' type='checkbox' checked></td>");
                    }

                    if (!!related) {
                        create_new.append("<td><div name='Related'>" + related[Object.keys(related)][0] + "</div></td>");
                    }

                    table.append(create_new).append(add_new);

                }
            });

        });

        define_new.confirm(function (e) {
            let table = this.meta.content.find("table");
            let rows = table.find("tr:not(:first-child):not(:last-child)");
            let items = [];
            rows.each(function () {
                let td = $("td", this);
                if ($("input[name=Name]", td).val() !== "") {
                    let item = {};
                    item[key] = $(this).data(key.toLowerCase());
                    if (!!related) {
                        let _key = Object.keys(related);
                        item[_key] = $("div[name=Related]", td).text();
                    }
                    item[name] = $.stripHTML($("input[name=Name]", td).val());

                    //item[name] = $.jQuery($("input[name=Name]", td).val()).text();

                    if (!!has_active) {
                        item["Active"] = $("input[name=Active]", td).is(":checked");
                    }
                    items.push(item);
                }

            });

            $.ajax({
                url: to_url,
                type: "post",
                dataType: "json",
                data: $.antiForgeryToken({ data: items }),
                success: function (res) {
                    updateSelect(select, res.Data, key, name);
                    location.reload();
                }

            });
            this.meta.shutdown();
        });
    }
}

function updateSelect(selector, jsons, key, value) {
    if ($(selector)[0].tagName === "SELECT") {
        $(selector).children().remove();

        let add_new = $("<option value='-1' class='font-weight-bold'>Define New</option>");
        $.each(jsons, function (i, json) {
            let option = $("<option value='" + json[key] + "'>" + json[value] + "</option>");
            $(selector).append(option);
        });
        $(selector).append(add_new);
        if ($(selector).children().length <= 1) {
            $(selector).prepend("<option value='0' selected disabled></option>");
        }
    }
}