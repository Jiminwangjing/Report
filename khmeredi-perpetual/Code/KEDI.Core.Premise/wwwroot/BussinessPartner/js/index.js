$(document).ready(function () {
    $("#selectpage").on('change', function () {
        submitForm();
    })
    $(".myTable").on("click", "#setDefialts", function () {
        var cut = $(this).closest('tr');
        var id = cut.find('td:eq(0)').text();
        $("#txtid").val(id);
    });
    $("#txtseaerch").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $(".myTable tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    function submitForm() {
        document.getElementById("form-id").submit();
    }
    let currentRow = undefined;
    function popupDialog(target) {
        let $row = $(target).parent().parent();
        let id = $row.data("id");
        let _dialog = new DialogBox({
            type: "yes-no",
            content: "Are you sure you want to delete?",
            caption: "Business Partner",
            icon: "danger"
        });
        _dialog.confirm(function () {
            $.ajax({
                url: "/BusinessPartner/Delete",
                type: "POST",
                data: { id: id },
                complete: function (respones) {
                    $row.remove();
                }
            });
            _dialog.shutdown();
        });
    }
})