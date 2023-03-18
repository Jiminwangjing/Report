$(document).ready(function () {
    if ($("#card-extend-expiry-period").val() == 0) {
        const dateFrom = $("#exp-extend-date-from").val();
        const dateNow = new Date();
        $("#exp-extend-date-to").prop("disabled", false);
        if (!dateFrom || dateFrom == "") {
            $("#exp-extend-date-from")[0].valueAsDate = dateNow;
            $("#exp-extend-date-to")[0].valueAsDate = dateNow;
        }
    } else {
        $("#exp-extend-date-to").prop("disabled", true);
    }

    $("#card-extend-expiry-period").change(function () {
        const dateFrom = $("#exp-extend-date-from").val();
        const dateNow = new Date();
        if (!dateFrom || dateFrom == "") {
            $("#exp-extend-date-from")[0].valueAsDate = dateNow;
        }
        if (this.value == 0) {// None
            $("#exp-extend-date-to").prop("disabled", false);
            $("#exp-extend-date-to")[0].valueAsDate = dateNow;
        } else if (this.value == 1) {// ThreeMonths
            setDateTo(3, dateNow, dateFrom)
        }
        else if (this.value == 2) {// SixMonths
            setDateTo(6, dateNow, dateFrom)
        }
        else if (this.value == 3) {// OneYear
            setDateTo(1, dateNow, dateFrom, true)
        }
    })

    $("#save").click(function (){
        const data = {
            CardID: parseInt($("#cardId").val()),
            CusID: parseInt($("#cusId").val()),
            LengthExpireCard: $("#card-extend-expiry-period").val(),
            DateFrom: $("#exp-extend-date-from").val(),
            DateTo: $("#exp-extend-date-to").val(),
        }
        const dialogComfirm = new DialogBox({
            content: "Are you sure you want to save the item?",
            type: "yes-no",
            icon: "warning"
        })
        dialogComfirm.confirm(function () {
            $.post("/CardMember/RenewExpireDateCard", { data }, function (res) {
                new ViewMessage({
                    summary: {
                        selector: ".error"
                    },
                }, res);
                if (res.IsApproved) {
                    location.href = "/CardMember/Index"
                }
            })
        })
        dialogComfirm.reject(function () {
            dialogComfirm.shutdown();
        })
    })

    function setDateTo(numMonths, dateNow, dateFrom, isyear = false) {
        $("#exp-extend-date-to").prop("disabled", true);
        if (dateFrom != "") {
            let ymd = dateFrom.split("-");
            let m = parseInt(ymd[1]);
            let y = parseInt(ymd[0]);
            if (isyear) {
                ymd[0] = y + numMonths;
            }
            else {
                ymd[1] = (m + numMonths).toString();
                ymd[1] = ymd[1].length === 1 ? `0${ymd[1]}` : ymd[1]
            }
            $("#exp-extend-date-to")[0].valueAsDate = new Date(`${ymd.join("-")}T12:00:00`);
        } else {
            const dateto = new Date().setMonth(dateNow.getMonth() + numMonths);
            $("#exp-extend-date-to")[0].valueAsDate = new Date(dateto);
        }
    }
})