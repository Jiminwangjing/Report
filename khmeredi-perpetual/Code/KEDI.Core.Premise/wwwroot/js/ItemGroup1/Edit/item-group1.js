let _data = JSON.parse($("#item-group-data").text());
let config = {
    master: {
        keyField: "WarehouseID",
        value: _data.ItemGroup1
    },
    singleDetail: {
        keyField: "WarehouseID",
        name: "ItemAccounting",
        value: _data.ItemAccounting
    }
};
let coreMaster = new CoreItemMaster(config);
let GLAcc = [];

$.ajax({
    url: "/ItemGroup1/GetGlAccountLastLevel",
    type: "get",
    async: false,
    dataType: "JSON",
    success: function (response) {
        $.each(response, function (i, item) {
            GLAcc.push(item)
        })
    }
});

$(document).ready(function () {
    var name = $("#name").text();
    $("#nameg1").hide();
    $("#revenue").hide();
    $("#inventory").hide();
    $("#cogs").hide();
    $("#allocation").hide();
    $("#expenseAccount").hide();
    $(".modal-header").on("mousedown", function (mousedownEvt) {
        var $draggable = $(this);
        var x = mousedownEvt.pageX - $draggable.offset().left,
            y = mousedownEvt.pageY - $draggable.offset().top;
        $("body").on("mousemove.draggable", function (mousemoveEvt) {
            $draggable.closest(".modal-dialog").offset({
                "left": mousemoveEvt.pageX - x,
                "top": mousemoveEvt.pageY - y
            });
        });
        $("body").one("mouseup", function () {
            $("body").off("mousemove.draggable");
        });
        $draggable.closest(".modal").one("bs.modal.hide", function () {
            $("body").off("mousemove.draggable");
        });
    });

    $("#btn-update").on("click", function () {
        let count = 0;
        var checkName = $("#name").val();

        if (_data.ItemGroup1.Name == null || _data.ItemGroup1.Name == "" || checkName == "") {
            count += 1;
            $("#nameg1").show();
	    $("#nameg1").text("Please Input Name!").css('color', '#CB4335');
        }
        if (_data.ItemAccounting.RevenueAccount == null) {
            $("#revenue").show();
        }
        if (_data.ItemAccounting.InventoryAccount == null) {
            $("#inventory").show();
        }
        if (_data.ItemAccounting.CostofGoodsSoldAccount == null) {
            $("#cogs").show()
        }
        if (_data.ItemAccounting.AllocationAccount == null) {
            $("#allocation").show()
        }

        if (count > 0) {
            setTimeout(function () {
                $("#nameg1").text("");
            }, 3000);
            return;
        }

        var formData = new FormData($("#formId")[0]);
        formData.append('files', $('input[type=file]')[0].files[0]);
        coreMaster.updateMaster("Name", $("#name").val());
        coreMaster.submitSingleData("/ItemGroup1/Edit", ["itemGroup1", "itemAccounting"], function (res) {

            formData.append("ItemGroup1Id", res.ItemG1ID);
            $.ajax({
                url: "/ItemGroup1/UploadImageEdit",
                type: "POSt",
                processData: false,
                contentType: false,
                data: formData,
                success: function (resp) {
                    location.href = "/ItemGroup1/Index";
                }
            });
        });
    })

    $("#exampleCheck1").on("click", function () {
        if ($(this).prop("checked") === true) {
            coreMaster.updateMaster("Visible", true);
        }
        if ($(this).prop("checked") === false) {
            coreMaster.updateMaster("Visible", false);
        }
    })

    $.ajax({
        url: "/ItemGroup1/Getname",
        type: "get",
        dataType: "JSON",
        success: function (res) {
            let data = '';
            let __td = '';
            $.each(res, function (i, item) {
                var split = item.split("*", 1);
                var splitName = split[0];

                let __data = _data.ItemAccounting[splitName.replace(/ /g, "")];
                GLAcc.forEach(name => {
                    if (__data === name.Code) {
                        __td = `
                            <span class='p-0' data-name='${splitName.replace(/ /g, "")}'>${name.Name}</span>
                        `
                    }
                })
                data += `
                    <tr>
                        <td class='p-0'>${item}</td>
                        <td class='p-0'>
                            <div class="input-group" id="input${i}" data-prop='${splitName.replace(/ /g, "")}'>
                                <input class="form-control input-focus"
                                    autocomplete='off' readonly
                                    name='validation' data-prop='${splitName.replace(/ /g, "")}' 
                                    value="${__data === null ? "" : __data}" type="text">
                            </div>
                        </td>
                        <td class='p-0' hidden>${i + 1}</td>
                        <td class='p-0 itemName' data-name='${splitName.replace(/ /g, "")}'>${__td}</td>
                    </tr>
                `;
            })

            $("#tbody-item").append(data);
            $("[name='validation']").prop("event-pointer", "auto");
            for (let i = 0; i < 32; i++) {
                if ($(`input[name='validation']:nth(${i})`).val() === '') {
                    $(`.itemName:nth(${i})`).text("");
                }
                $(`input[name='validation']:nth(${i})`).on("keyup", function () {
                    if ($(this).val() === '') {
                        if ($(this).data("prop") === $(`.itemName:nth(${i})`).data("name")) {
                            $(`.itemName:nth(${i})`).text("");
                            coreMaster.updateSingleDetail($(this).data("prop"), "");
                        }
                    }
                })
            }

            for (let i = 0; i < 32; i++) {
                $(`#input${i}`).on("dblclick", function () {
                    chooseCode(i, this);
                })
            }
            function chooseCode(id, tag) {
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
                    $.get("/ItemGroup1/GetGlAccountLastLevel", function (resp) {
                        let $listActiveGL = ViewTable({
                            keyField: "ID",
                            selector: dialog.content.find("#list-active-gl"),
                            paging: {
                                pageSize: 10,
                                enabled: true
                            },
                            visibleFields: ["Code", "Name"],
                            actions: [
                                {
                                    template: "<i class='fas fa-arrow-circle-down'></i>",
                                    on: {
                                        "click": function (e) {
                                            const propName = $(tag).data("prop");
                                            $(tag).children().val(e.data.Code);
                                            $(tag).parent().parent().find(".itemName").text(e.data.Name);
                                            coreMaster.updateSingleDetail(propName, e.data.Code);
                                            dialog.shutdown();
                                        }
                                    }
                                }
                            ]
                        });
                        $listActiveGL.clearRows();
                        $listActiveGL.bindRows(resp);

                        if (resp.length > 0) {
                            $("#txtSearch").on("keyup", function (e) {
                                let input = noSpace(this.value);
                                let searcheds = searchItems(resp, input);
                                $listActiveGL.clearRows();
                                $listActiveGL.bindRows(searcheds);
                            });
                        }
                    });
                });
            }
        }
    })
    const imgtag = $("#logo");
    if (_data.ItemGroup1.Images !== null) {
        imgtag.attr("src", `/Images/items/${_data.ItemGroup1.Images}`);
    } else {
        imgtag.attr("src", "/Images/default/no-image.jpg")
    }

})

function searchItems(items = [], keyword = "") {
    let input = noSpace(keyword);
    let regex = new RegExp(input, "i");
    return $.grep(items, function (item) {
        return regex.test(noSpace(item.Code))
            || regex.test(noSpace(item.Name))
    });
}

function noSpace(phrase) {
    if (typeof phrase === "string") {
        return phrase.replace(/\s/g, "");
    }
}

function show(input) {
    if (input.files && input.files[0]) {
        var filerdr = new FileReader();

        filerdr.onload = function (e) {
            $('#logo').attr('src', e.target.result);
            $('#img').val(input.files[0].name)
        }
        filerdr.readAsDataURL(input.files[0]);
    }
}

function SelectColor() {
    var iD = $("#txtcolor").val();
    $.ajax({
        url: "/ItemGroup1/GetColor",
        type: "Get",
        dataType: "Json",
        data: { id: iD },
        success: function (respones) {
            $.each(respones, function (i, item) {

                // document.getElementById("txtcolorChang").innerHTML = item.Name;
                //document.getElementById("txtcolorChang").style.color = "" + item.Name;
            });
        }
    });
}

function SelectBack() {
    var iD = $("#txtBack").val();
    $.ajax({
        url: "/ItemGroup1/GetBackground",
        type: "Get",
        dataType: "Json",
        data: { id: iD },
        success: function (respones) {
            $.each(respones, function (i, item) {
                $("#txtcolorChangback").val(item.Name);
                document.getElementById("txtcolorChangback").style.background = "" + item.Name;
            });
        }
    });
}

function AddColor() {
    var name = $("#colorName").val();
    var cout = "";
    if (name == 0) {
        cout++;
        $("#colorName").css("border-color", "red");
    } else {
        cout = 0;
        $("#colorName").css("border-color", "lightgrey");
    }
    if (cout > 0) {
        cout = 0;
        return;
    }
    $.ajax({
        url: "/ItemGroup1/AddColors",
        type: "POST",
        dataType: "Json",
        data: { Name: name },
        complete: function (respones) {
            $("#colorName").val("");
            $("#txtcolor option").remove();

            $.ajax({
                url: "/ItemGroup1/SelecrColor",
                type: "Get",
                dataType: "Json",
                success: function (respones) {
                    var data = "";
                    $.each(respones, function (i, item) {
                        data +=
                            '<option value="' + item.ColorID + '">' + item.Name + '</option>';
                    });
                    $("#txtcolor").append(data);

                }
            });
        }
    });
}

function AddBack() {
    var name = $("#backName").val();
    var cout = "";
    if (name == 0) {
        cout++;
        $("#backName").css("border-color", "red");
    } else {
        cout = 0;
        $("#backName").css("border-color", "lightgrey");
    }
    if (cout > 0) {
        cout = 0;
        return;
    }
    $.ajax({
        url: "/ItemGroup1/AddBackground",
        type: "POST",
        dataType: "Json",
        data: { Name: name },
        complete: function (respones) {
            $("#backName").val("");
            $("#txtBack option").remove();

            $.ajax({
                url: "/ItemGroup1/SelectBackground",
                type: "Get",
                dataType: "Json",
                success: function (respones) {
                    var data = "";

                    $.each(respones, function (i, item) {
                        data +=
                            '<option value="' + item.BackID + '">' + item.Name + '</option>';
                    });
                    $("#txtBack").append(data);

                }
            });
        }
    });
}