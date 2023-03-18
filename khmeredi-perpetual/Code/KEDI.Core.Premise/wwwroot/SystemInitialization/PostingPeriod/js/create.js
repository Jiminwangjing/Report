let _data = JSON.parse($("#model-data").text());
let config = {
    master: {
        keyField: "WarehouseID",
        value: _data
    }
};
let coreMaster = new CoreItemMaster(config);
$(document).ready(function () {

    let pdfrom = moment().startOf('month');
    let pdto = moment().endOf('month');

    $("#pdfrom").val(formatDate(pdfrom, "YYYY-MM-DD"));
    $("#ddfrom").val(formatDate(pdfrom, "YYYY-MM-DD"));
    $("#docdfrom").val(formatDate(pdfrom, "YYYY-MM-DD"));
    $("#pdto").val(formatDate(pdto, "YYYY-MM-DD"));
    $("#ddto").val(formatDate(pdto, "YYYY-MM-DD"));
    $("#docdto").val(formatDate(pdto, "YYYY-MM-DD"));
    $("#sfy").val(formatDate(pdfrom, "YYYY-MM-DD"));

    function formatDate(value, format) {
        let dt = new Date(value),
            objFormat = {
                MM: ("0" + (dt.getMonth() + 1)).slice(-2),
                DD: ("0" + dt.getDate()).slice(-2),
                YYYY: dt.getFullYear()
            },
            dateString = "";

        let dateFormats = format.split("-");
        for (let i = 0; i < dateFormats.length; i++) {
            dateString += objFormat[dateFormats[i]];
            if (i < dateFormats.length - 1) {
                dateString += "-";
            }
        }
        return dateString;
    }

    //const __dateformat = $(".date");
    //$.each(__dateformat, function (_, value) {
    //    value.valueAsDate = new Date();
    //})
    $("#subperiod").children(":nth(1)").hide();
    $("#subperiod").children(":nth(1)").removeAttr("selected");
    $("#subperiod").children(":nth(0)").prop("selected", "selected");
    $("#subperiod").change(function () {
        const noOfPeriod = $("#noOfPeriod");
        if (this.value == 1) {
            noOfPeriod.val(1);
        }
        if (this.value == 2) {
            noOfPeriod.val(12);
        }
        coreMaster.updateMaster("NoOfPeroid", noOfPeriod.val());
        coreMaster.updateMaster("SubPeriod", parseInt(this.value));
    });
    $("#periodIndID").change(function () {
        coreMaster.updateMaster("PeroidIndID", parseInt(this.value));
    });
    $("#addPostingPeriod").click(function () {

        const pCode = $("#periodCode").val();
        const pName = $("#periodName").val();
        const ddfrom = $("#ddfrom").val();
        const ddto = $("#ddto").val();
        const docdfrom = $("#docdfrom").val();
        const docdto = $("#docdto").val();
        const pdfrom = $("#pdfrom").val();
        const pdto = $("#pdto").val();
        const sfy = $("#sfy").val();
        const fy = $("#fy").val();
        coreMaster.updateMaster("PeriodCode", pCode);
        coreMaster.updateMaster("PeriodName", pName);
        coreMaster.updateMaster("PostingDateFrom", pdfrom);
        coreMaster.updateMaster("PostingDateTo", pdto);
        coreMaster.updateMaster("DueDateFrom", ddfrom);
        coreMaster.updateMaster("DueDateTo", ddto);
        coreMaster.updateMaster("DocuDateFrom", docdfrom);
        coreMaster.updateMaster("DocuDateTo", docdto);
        coreMaster.updateMaster("StartOfFiscalYear", sfy);
        coreMaster.updateMaster("FiscalYear", fy);
        coreMaster.updateMaster("PeroidStatus", 1);
        coreMaster.submitData("/postingperiod/addpostingperiod", function (res) {
            if (res.Action == 1) {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, res);
                location.href = "/postingperiod/index"
            }
            new ViewMessage({}, res);
        })
    })
    $("#addPeriodIndId").click(function () {
        let dialog = new DialogBox({
            button: {
                ok: {
                    text: "SAVE"
                }
            },
            type: "ok-cancel",//[ok, ok-cancel, yes-no, yes-no-cancel]
            content: {
                selector: "#active-gl-content"
            }
        });
        dialog.reject(function () {
            dialog.shutdown();
        });
        dialog.confirm(function () {
            const name = dialog._content.find("#indecatorName").val();
            let count = 0;
            if (name == "") {
                count++;
                $("#indecatorName").css("border-color", "red");
            }
            else {

                $("#indecatorName").css("border-color", "lightgrey");
            }

            if (count > 0) {
                count = 0;
                return;
            }
            const indecator = {
                name
            }
            $.ajax({
                url: "/postingperiod/addperiodindecator",
                type: "POST",
                dataType: "JSON",
                data: { indecator },
                complete: function (respones) {
                    $.ajax({
                        url: "/postingperiod/getperiodindecator",
                        type: "GET",
                        dataType: "JSON",
                        success: function (respones) {
                            var data = "";
                            $.each(respones, function (i, item) {
                                data +=
                                    '<option value="' + item.ID + '">' + item.Name + '</option>';
                            });
                            $("#periodIndID").html(data);
                            coreMaster.updateMaster("PeroidIndID", respones[0].ID);
                        }
                    });

                }
            })
            dialog.shutdown();
        });
        dialog.invoke(function () { });
    })
})