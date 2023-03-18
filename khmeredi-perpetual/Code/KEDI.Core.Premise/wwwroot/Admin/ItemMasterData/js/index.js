
var db = new Warehouse();
let itemMaster = {
    ID: "",
    Code: "",
    KhmerName: "",
    EnglishName: "",
    Barcode: ""
};
///// Genterate Dynamic Header For Property /////
$.get("/Property/GetActivePropertiesOrdering", function (res) {
    res.forEach(i => {
        $("#item-list tr").append(`
            <th>${i.NameProp}</th>
        `);
    })
})

let __pageNumber = $("#selectpage option:selected").val(),
    __inActive = $("#ch-show-inactive").prop("checked");

function submitForm() {
    document.getElementById("form-id").submit();
}

function loadScreen(enabled = true) {
    if (enabled) {
        $("#table-content-loading").show();
    } else {
        $("#table-content-loading").hide();
    }
}

function applyPaging(response, pageNumber) {
    $("#item-list").find("tr:not(:first-child)").remove();
    var __setting = {
        container: "#data-paging-item-master",
        keyField: "ID",
        pageSize: pageNumber,
        startIndexSize: 4
    };

    const __paging = DataPaging(__setting);
    setPageIndex(1);
    __paging.navigate(getPageIndex(), response, function (info) {
        setPageIndex(info.page.activeIndex);
        var __index = __setting.pageSize * (info.page.activeIndex - 1);
        displayData(info.dataset, __index);
    });
}

function getPageIndex() {
    var pageIndex = parseInt(localStorage.getItem("__ITEM_MASTER_INDEX"));
    if (isNaN(pageIndex)) { return 1; }
    return pageIndex;
}

function setPageIndex(index) {
    if (isNaN(index)) {
        localStorage.setItem("__ITEM_MASTER_INDEX", 1);
    }
    localStorage.setItem("__ITEM_MASTER_INDEX", index);
}

if ($('#page-remember').val().toString().includes('/')) {
    pageRemember = parseInt($('#page-remember').val().split('/')[0]);
    recPerPage = parseInt($('#page-remember').val().split('/')[1]);
    $('#selectpage').val(recPerPage);
}

queryItemMasterData(__inActive, __pageNumber);
function queryItemMasterData(inActive, pageNumber, keyword = "") {
    loadScreen();
    $.get("/ItemMasterData/GetItems", {
        inActive: inActive,
        keyword: keyword
    }, function (items) {
        let pageNumber = $("#selectpage option:selected").val();
        applyPaging(items, pageNumber);
        db.insert("tb_items", items, "ID");
        $("#total-items").text(items.length);

        $("#search-item-masters").on("keyup", function () {
            let keyword = this.value;
            searchItemMasters(items, pageNumber, keyword);
        });

        $("#selectpage").on('change', function () {
            let pageNumber = parseInt(this.value);
            searchItemMasters(items, pageNumber, keyword);
        });

        setTimeout(function () {
            loadScreen(false);
        }, 15000);
        loadScreen(false);
    });
}

function searchItemMasters(allItems, pageNumber, keyword) {
    keyword = noSpace(keyword);
    if (Array.isArray(allItems) && allItems.length > 0) {
        const regex = new RegExp(keyword, "i");
        var searcheds = $.grep(allItems, function (item) {
            if (item !== undefined) {
                return item.ItemMasterData.Code.toLowerCase().match(keyword)
                    || regex.test(noSpace(item.ItemMasterData.Barcode))
                    || regex.test(noSpace(item.ItemMasterData.KhmerName))
                    || regex.test(noSpace(item.ItemMasterData.EnglishName))
                    || regex.test(noSpace(item.ItemMasterData.Process))
                    || regex.test(noSpace(item.ItemMasterData.ItemGroup1.Name))
                    || regex.test(noSpace(item.ItemMasterData.UnitofMeasureInv.Name));
            }
        });
        applyPaging(searcheds, pageNumber);
    }
}

function noSpace(phrase) {
    if (typeof phrase === "string") {
        return phrase.replace(/\s/g, "");
    }    
}
$('#ch-show-inactive').change(function () {
    var inActive = $(this).prop("checked");
    queryItemMasterData(inActive, __pageNumber);
});

$("#item-list").on("click", "#DeleteItemMaster", function () {
    var cut = $(this).closest('tr');
    var id = cut.find('td:eq(0)').text();
    document.getElementById("txtid").value = id;
});

function ClickYes() {
    var id = $("#txtid").val();
    $.ajax({
        url: "/ItemMasterData/DeleteItemMaster",
        type: "Delete",
        dataType: "Json",
        data: { ID: id },
        complete: function (respones) {
            location.reload();
        }
    });
}

function displayData(displayRecords, index) {
    $("#item-list").find("tr:not(:first-child)").remove();
    var tr;
    for (var i = 0; i < displayRecords.length; i++) {
        index++;
        tr = $('<tr/>');
        if (displayRecords.length == 0) {
            tr = $('<tr/>');
            tr.append("<td colspan='11' class='text-center'>NoData</td>");
            $('#item-list').append(tr);
        }
        else {
            var currPage = pageRemember + '/' + recPerPage + '/' + displayRecords[i].ItemMasterData.ID;
            if (displayRecords[i].ID == parseInt($('#page-remember').val().split('/')[2])) {
                tr = $('<tr class=seleted/>').on('dblclick', editRow);
                tr.append("<td>" + index + "</td>");
                tr.append("<td contenteditable='false' hidden data-id=" + displayRecords[i].ItemMasterData.ID + ">" + displayRecords[i].ItemMasterData.ID + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.Code + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.KhmerName + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.EnglishName + "</td>");
                tr.append("<td>" + displayRecords[i].ItemMasterData.UnitofMeasureInv.Name + "</td>");
                tr.append("<td>" + displayRecords[i].ItemMasterData.ItemGroup1.Name + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.Barcode + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.Process + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.UnitPrice + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.Stock + "</td>");
                tr.append(`<td><form enctype='multipart/form-data' class='input-file'>
                        <input name='ItemID' type=hidden value='${displayRecords[i].ItemMasterData.ID}'>
                        <img src='/Images/items/${displayRecords[i].ItemMasterData.Image}'/>
                        <input name='ImageFile' type='file' onchange='show(this)' value='' accept='.ico, .png, .jpg, .jpeg, .gif'>
                        </form></td>`);

                let td = $("<td>");
                td.append($("<a title='Save'><i class='fa fa-save fa-lg hide csr-pointer'></i></a>").on('click', saveEdit))
                td.append("&nbsp;|&nbsp;<a class='btn btn-xs btn-kernel' title='Edit' href='/ItemMasterData/Edit?id=" + displayRecords[i].ItemMasterData.ID + "&currPage=" + currPage + " '>Edit</a>");
                td.append("&nbsp;|&nbsp;<a title='Copy' href='/ItemMasterData/Copy?id=" + displayRecords[i].ItemMasterData.ID + "&currPage=" + currPage + " '><i class='fa fa-copy fa-lg' style='color:#f2e8c5'></i></a>");
                tr.append(td);
                displayRecords[i].PropWithName?.forEach(prop => {
                    tr.append(`
                                <td>
                                    ${prop.ValueName}
                                </td>`);
                });
                $('#item-list').append(tr);
            }
            else {
                tr = $('<tr/>').on('dblclick', editRow);
                tr.append("<td>" + index + "</td>");
                tr.append("<td contenteditable='false' hidden data-id=" + displayRecords[i].ItemMasterData.ID + ">" + displayRecords[i].ItemMasterData.ID + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.Code + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.KhmerName + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.EnglishName + "</td>");
                tr.append("<td>" + displayRecords[i].ItemMasterData.UnitofMeasureInv.Name + "</td>");
                tr.append("<td>" + displayRecords[i].ItemMasterData.ItemGroup1.Name + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.Barcode + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.Process + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.UnitPrice + "</td>");
                tr.append("<td contenteditable='false'>" + displayRecords[i].ItemMasterData.Stock + "</td>");
                tr.append(`
                            <td>
                                <form enctype='multipart/form-data' class='input-file'>
                                    <input name='ItemID' type=hidden value='${displayRecords[i].ItemMasterData.ID}'>
                                    <img src='/Images/items/${displayRecords[i].ItemMasterData.Image}'/>
                                    <input name='ImageFile' type='file' onchange='show(this)' value=''accept='.ico, .png, .jpg, .jpeg, .gif'>
                                </form>
                            </td>`);

                let td = $("<td>");
                td.append($("<a title='Save'><i class='fa fa-save fa-lg hide csr-pointer fn-dark-purple'></i></a>").on('click', saveEdit))
                td.append("&nbsp;✦&nbsp;<a title='Edit' href='/ItemMasterData/Edit?id=" + displayRecords[i].ItemMasterData.ID + "&currPage=" + currPage + " '><i class='fa fa-edit fa-lg'></i></a>");
                td.append("&nbsp;✦&nbsp;<a  title='Copy' href='/ItemMasterData/Copy?id=" + displayRecords[i].ItemMasterData.ID + "&currPage=" + currPage + " '><i class='fa fa-copy fa-lg'></i></a>");
                tr.append(td);
                displayRecords[i].PropWithName?.forEach(prop => {
                    tr.append(`
                                <td>
                                    ${prop.ValueName}
                                </td>`);
                });
                $('#item-list').append(tr);
            }
        }
    }
}


function editRow() {
    $(this).removeClass('seleted');
    $(this).find('.fa-save').removeClass('hide');
    $(this).find('input').removeAttr('disabled');
    let tds = $(this).find('[contenteditable=false]');
    $.each(tds, function (i, td) {
        $(td).prop('contenteditable', true);
    });
}

function saveEdit() {
    let editables = $(this).parent().siblings("[contenteditable=true]");
    $.each(editables, function (i, cell) {
        let header = Object.getOwnPropertyNames(itemMaster)[i];
        itemMaster[header] = cell.textContent;
    });
    let cell = $(this).parent().parent();
    if ($(cell).children().length > 0) {
        let files = $(cell).find("input[type=file]")[0].files;
        if (files[0] !== undefined) {
            itemMaster["Image"] = files[0].name
        }
    }
    $(this).addClass('hide');
    $.each(editables, function (i, td) {
        $(td).prop('contenteditable', false);
    });

    if (itemMaster["ID"] && parseInt(itemMaster["ID"]) > 0) {
        $.ajax({
            url: '/ItemMasterData/UpdateRow',
            type: 'POST',
            dataType: 'JSON',
            data: {
                itemMaster: itemMaster
            },
            success: function (data) {
                new ViewMessage({
                    summary: {
                        attribute: "message-summary"
                    }
                }, data);

            }
        });
    }

}

function show(input) {
    let img = $(input).siblings('img');
    if (input.files && input.files[0]) {
        $.ajax({
            url: "/ItemMasterData/UploadImage",
            data: new FormData($(input).parent(".input-file")[0]),
            type: "POST",
            contentType: false,
            processData: false,
            success: function () {
                var filerdr = new FileReader();
                filerdr.onload = function (e) {
                    img.attr('src', e.target.result);
                }

                filerdr.readAsDataURL(input.files[0]);
            }
        });

    }
}