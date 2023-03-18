//Access admin authorization
function accessSecurity(content, code) {
    let check = false;
    let username = content.find('.security-username').val();
    let pass = content.find('.security-password').val();
   
    let access = $.ajax({
        url: '/POS/GetUserAccessAdmin',
        async: false,
        type: 'GET',
        dataType: 'JSON',
        data: { username: username, pass: pass, code: code }
    }).responseJSON;
    if (access !== undefined) {
        return access.Used;
    }
    else {
        if (username == "") {
            content.find('.security-username').focus();
        }
        else if (pass == "") {
            content.find('.security-password').focus();
        }
        return check;
    }
   
}