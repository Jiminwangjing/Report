
$("#btn-discount").click(function (e) {
    confirmDiscount();
    
});
function confirmDiscount() {
    let user_privillege = db.table("tb_user_privillege").get('P006');
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
                        let access = accessSecurity(this.meta.content, 'P006');
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
                                    initFormDiscountMember();
                                }, 100);
                            }, 500);
                        }
                    }
                }
            }

        });
    }
    else {

        initFormDiscountMember();
    }

}

let discountRate = 0;
let typeDis = 'Percent';
function rowClicked() {
    let ref_no = $(this).data("ref_no");
   
    let detail_info = db.table("tb_members_card").get(ref_no.toString());
    discountRate = detail_info.discount;
    typeDis = detail_info.typedis;
    $(".member-id").text(detail_info.ref_no);
    $(".member-name").text(detail_info.name);
    $(".member-card-type").text(detail_info.card_type);
    if (detail_info.typedis==='Percent')
        $(".member-discount").text(detail_info.discount + "%");
    else
        $(".member-discount").text(detail_info.discount + " " + local_currency.name);
    $(".member-expire").text(detail_info.expire);
    $(".member-descr").text(detail_info.desc);
}
function initFormDiscountMember() {
    if ($("#item-listview").find('tr:not(:first-child)').length !== 0) {
        let members = $.ajax({
            url: '/POS/GetMemberCard',
            type: 'GET',
            async: false,
            success: function () {

            },
            error: function (e) {
            
            }
        }).responseJSON;

        $.each(members, function (i, mem) {
            let member = {};
            member.ref_no = mem.Ref_No;
            member.name = mem.Name;
            member.card_type = mem.CardType.Name;
            member.desc = mem.Description;
            member.discount = mem.CardType.Discount;
            member.typedis = mem.CardType.TypeDis;
            member.expire = mem.ExpireDate.slice(0, 10);
            db.insert("tb_members_card", member, "ref_no");
        });

        $.bindRows(".membercard-discount-listview", db.from("tb_members_card"), "ref_no", {
            click: rowClicked,
            text_align: [{ "name": "left" }],
            hidden_columns: ["discount", "desc", "expire", "typedis"]

        });
        let msg = new DialogBox(
            {
                caption: "Discount Member Card",
                content: {
                    selector: "#membercard-discount"
                },
                position: "top-center",
                type: "ok-cancel",
                button: {
                    ok: {
                        text: "Apply"
                    }
                },
                animation: {
                    shutdown: {
                        animation_type: "slide-up"
                    }
                }
            }
        );
        msg.confirm(discountApply);
        msg.reject(function (e) {
            this.meta.shutdown();
        });
    }
    else {
        let msg = new DialogBox(
            {
                caption: "Empty",
                content: "Can not discount...!",
                position: "top-center",
                type: "ok",
                icon: "info"
            }
        );
        msg.setting.button.ok.callback = function (e) {
            this.meta.shutdown();
        };
    }
}

function discountApply() {
    order_master.DiscountRate = discountRate;
    order_master.TypeDis = typeDis;
    summaryTotal(0, 'Y');
    this.meta.shutdown();
}
//Search member in table not yet
$('input[name=search-member]').on('keyup', function () {
    var query = ($(this).val()).toLowerCase();
    if (db.from("tb_members_card") !== 0) {
        let member_filtered = db.from("tb_members_card").where(w => {
            return w.name.toLowerCase().includes(query) || w.ref_no === query;
        });
        $.bindRows(".membercard-discount-listview", member_filtered, "ref_no", {
            click: rowClicked,
            text_align: [{ "name": "left" }],
            hidden_columns: ["discount", "desc", "expire", "typedis"]
        });
    }
   
});