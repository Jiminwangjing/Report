//Create Customer & List

$("#goto-create-customer").click(function () {
    confirmCreateCustomer();
});
$('.change-customer').click(function () {
    initFormListCustomer();
});
$("#goto-list-customer").click(function () {
    initFormListCustomer();
});
function initFormListCustomer() {
     let list_customer = new DialogBox(
        {
            icon: "far fa-list-alt",
            close_button: true,
            content: {
                selector: "#list-customer-content",
            },
            caption: "List Customer",
            position: "top-center",
            type: "ok",
            button: {
                ok: {
                    text: "Cancel",
                    callback: function (e) {
                        this.meta.shutdown();
                    }
                }
            }
        }
    );
    initCustomer(list_customer);
};
function initCustomer(list_customer) {
    $.bindRows(".list-customer-listview", db.from('tb_customer'), "ID", {
        text_align: [{ "Name": "left" }],
        html: [
            {
                column: -1,
                insertion: "replace",
                element: '<div class=btn><i class="fas fa-arrow-circle-right"></i></div>',
                listener: ["click", function (e) {
                    rowChooseCustomer(this);
                    list_customer.shutdown();
                }]
            }
        ],
        hidden_columns: ["ID","Type","PriceListID","PriceList","Delete"]
    });
};
$("#search-customer").keyup(function () {
    let _input = this;
    let search = $(_input).val().toString().toLowerCase();
    $(".list-customer-listview tr:not(:first-child").filter(function () {
        $(this).toggle($(this).text().toLowerCase().indexOf(search) > -1);
    });
});
function rowChooseCustomer(row) {
    let customer_id = parseInt($(row).parent().parent().data('id'));
    let customer = db.get('tb_customer').get(customer_id);
    order_master.CustomerID = customer_id;
    $('.change-customer').text(customer.Name);
};
function confirmCreateCustomer() {
    let user_privillege = db.table("tb_user_privillege").get('P018');
    if (user_privillege.Used === false) {
        let dlg = new DialogBox({
            // close_button: false,
            position: "top-center",
            content: {
                selector: "#admin-authorization",
                class: "login"
            },
            icon: "fas fa-lock",
            button: {
                ok: {
                    text: "Login",
                    callback: function (e) {
                        let access = accessSecurity(this.meta.content, 'P018');
                        if (access === false) {
                            this.meta.content.find('.error-security-login').text('You can not access ...!');
                            return;
                        }
                        else {
                            this.meta.content.find('.security-username').focus();
                            this.meta.setting.icon = "fas fa-lock fa-spin";
                            this.text = "Logging...";
                            this.meta.content.find('.error-security-login').text('');
                            this.meta.build(this.setting);
                            setTimeout(() => {
                                this.meta.build(this.setting);
                                this.meta.setting.icon = "fas fa-unlock-alt";
                                setTimeout(() => {
                                    this.meta.shutdown();
                                    initFromCustomer();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });

    }
    else {
        initFromCustomer();
    }
};

function initFromCustomer() {
     let create_customer = new DialogBox(
        {
            icon: "far fa-list-alt",
            close_button: true,
            content: {
                selector: "#create-customer-content",
            },
            caption: "Create Customer",
            position: "top-center",
            type: "ok-cancel",
            button: {
                ok: {
                    text: "Add",
                    callback: function (e) {
                        let content = this.meta.content;
                        if (content.find('.customer-code').val() == null || content.find('.customer-code').val() == '') {
                            $('.customer-code-validate').text('Error: Code required !');
                            content.find('.customer-code').focus();
                        }
                        else if (content.find('.customer-name').val() == null || content.find('.customer-name').val() == '') {
                            $('.customer-code-validate').text('Error: Name required !');
                            content.find('.customer-name').focus();
                        }
                        else if (content.find('.customer-address').val() == null || content.find('.customer-address').val() == '') {
                            $('.customer-code-validate').text('Error: Address required !');
                            content.find('.customer-address').focus();
                        }
                        else if (content.find('.customer-phone').val() == null || content.find('.customer-phone').val() == '') {
                            $('.customer-code-validate').text('Error: Phone required !');
                            content.find('.customer-phone').focus();
                        }
                        else {
                            createCustomer(content);
                        }
                        
                    }
                },
                cancel: {
                    callback: function (e) {
                        this.meta.shutdown();
                    }
                }
            }
        }
    );
    create_customer.startup("after", function (dialog) {
        let content = dialog.content;
        content.find(".customer-code").focus();
    });
};
$(".customer-code").on('keyup', function () {
    let _input = this;
    let code = $(_input).val().toString().toLowerCase();

    let customer = db.from("tb_customer").where(w => {
        return w.Code.toLowerCase() == code;
    });
    if (customer.length != 0) {
        $('.customer-code-validate').text('Error: This code have exist...!');
    }
    else {
        $('.customer-code-validate').text('');
    }
   
});
$(".customer-code").on('focusout', function () {
    $('.customer-code-validate').text('');
});
function createCustomer(content) {

    let loader = null;
    loader = new AjaxLoader("/pos/icons/ajax-loader/loading.gif");
    BusinessPartner = {
        ID: 0,
        Code: content.find('.customer-code').val(),
        Name: content.find('.customer-name').val(),
        Type: "Customer",
        PriceListID: setting.PriceListID,
        Phone: content.find('.customer-phone').val(),
        Email: content.find('.customer-email').val(),
        Address: content.find('.customer-address').val(),
        Delete: false
    };
    $.ajax({
        url: '/POS/CreateCustomer',
        type: 'POST',
        data: { business: BusinessPartner },
        dataType: 'JSON',
        success: function (response) {
            
            if (response.ID < 0) {
                $('.customer-code-validate').text('Error: This code have exist...!');
                loader.hide();
            }
            else {
                $('.customer-code-validate').text('Successfully...').css("color","green");
                db.insert('tb_customer', response, "ID");
                order_master.CustomerID = parseInt(response.ID);
                $('.change-customer').text(response.Name);
                loader.hide();
                clearCreateCustomer(content);
            }
        },
        error: function () {
            console.log('customer not complete');
        }
    });
    
};
function clearCreateCustomer(content) {
    content.find('.customer-code').focus();
    content.find('.customer-code').val('');
    content.find('.customer-name').val('');
    Phone: content.find('.customer-phone').val('');
    Address: content.find('.customer-address').val('');
    setTimeout(function () {
        $('.customer-code-validate').text('').css("color", "red");
    },1000);
};