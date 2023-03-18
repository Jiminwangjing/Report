let _groupData = JSON.parse($("#groupData").text());
let config = {
    master: {
        keyField: "WarehouseID",
        value: _groupData.ItemGroup1
    },
    singleDetail: {
        value: _groupData.ItemAccounting,
    },
    glAccName: {
        values: _groupData.GlAccName
    }
};
var countClick = 0;
// var clickAcc = false;
var clickAcc;
let coreMaster = new CoreItemMaster(config);
$(document).ready(function () {
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

    $("#btn-add").on("click", function () {
        let count = 0;
        var checkName = $("#name").val();
        if (checkName == "") {
            count += 1;
            $("#errors").text("Please Input Name!");
        }
        // if (clickAcc != true) {
        //     count += 1;
        //     $("#error").text("Please Choose Any Accounts That Have * More Less Than 5!");
        // }
        if (parseInt($("#smserr").val()) == 0) {
            count += 1;
            $("#error").text("Please Choose Any Accounts That Have * More Less Than 5!");
        }
        if (count > 0) {
            setTimeout(function () {
                $("#error").text("");
                $("#errors").text("");
            }, 3000);
            return;
        }

        var formData = new FormData($("#formId")[0]);
        formData.append('files', $('input[type=file]')[0].files[0])
        // Attach file
        // coreMaster.updateImage($('input[type=file]')[0].files[0]);
        coreMaster.updateMaster("Name", $("#name").val());
        coreMaster.updateMaster("Images", $("#img").val());
        coreMaster.submitSingleData("/ItemGroup1/Create", ["itemGroup1", "itemAccounting", "glAccName"], function (res) {
            formData.append("itemGroup1", res.Name);
            $.ajax({
                url: "/ItemGroup1/UploadImageCreate",
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

    function onGetWhAccTemplates(success) {
        $.get("/ItemGroup1/GetGlProp", success);
    }
    let $listJE = ViewTable({
        keyField: "LineID",
        selector: "#allw-gl",
        paging: {
            enabled: false
        },
        visibleFields: [
            "NameProp",
            "Code",
            "Name"
        ],
        columns: [
            {
                name: "Code",
                template: "<input type='text' readonly>",
                on: {
                    "dblclick": function (e) {
                        var split = e.data.NameProp.split("*", 1);
                        var splitName = split[0];
                        const propName = splitName.replace(/ /g, "");
                        chooseCode(e.key, propName);

                        if (e.data.LineID <= 5 && e.data.Name == null) {
                            countClick += 1;
                            // if (countClick >= 5) {
                            //     clickAcc = true;
                            // }
                            if (countClick >= 5) {
                                $("#smserr").val(1)
                            }
                        }
                    }
                }
            },

        ]
    });

    function chooseCode(id, propName) {
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
                    selector: dialog.content.find("#list-active-gl")[0],
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
                                    $listJE.updateColumn(id, "Code", e.data.Code);
                                    $listJE.updateColumn(id, "Name", e.data.Name);
                                    coreMaster.updateSingleDetail(propName, e.data.Code);
                                    coreMaster.updateGlAccName(id, "Name", e.data.Name);
                                    dialog.shutdown();
                                }
                            }
                        }
                    ]
                });
                $listActiveGL.clearRows();
                $listActiveGL.bindRows(resp);
                $(".input").attr("readonly", true);

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

    onGetWhAccTemplates(function (whAcc) {
        $listJE.clearRows();
        $listJE.bindRows(whAcc);
        $(".input").attr("readonly", true);
        $(".input").parent().hide();
        $(".hideTH").hide();
    });
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
            $('input[type=file]')[0].files[0];
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