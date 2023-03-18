function DeleteConfirmation(id, isDeleteClick) {
    var deleteuser = "deleteuser_" + id;
    var cfdelete = "cfdelete_" + id;

    if (isDeleteClick) {
        $('#' + deleteuser).hide();
        $('#' + cfdelete).show();
    } else {
        $('#' + deleteuser).show();
        $('#' + cfdelete).hide();
    }
}