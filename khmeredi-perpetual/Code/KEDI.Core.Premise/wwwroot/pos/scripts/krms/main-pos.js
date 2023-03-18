
$('#log-out-pos').click(function () {
    let msg = new DialogBox(
        {
            caption: "Log out",
            content: "Do you want to log out ?",
            position: "top-center",
            type: "ok-cancel",//ok, ok-cancel, yes-no, yes-no-cancel
            icon: "info" //info, warning, danger
        }
    );
    msg.setting.button.ok.callback = function (e) {
        window.location.href = "/Account/Login";
    };
    msg.reject(function (e) {
        this.meta.shutdown();
    });
});
$('#change-password-user').click(function () {
    let msg = new DialogBox(
        {
            caption: "Change Password",
            content: {
                selector:"#user-change-pass"
            },
            position: "top-center",
            type: "ok-cancel",//ok, ok-cancel, yes-no, yes-no-cancel
            icon: "info" //info, warning, danger
        }
    );
    msg.setting.button.ok.callback = function (e) {
        
    };
    msg.reject(function (e) {
        this.meta.shutdown();
    });
});