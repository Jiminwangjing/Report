$(document).ready(function () {
    let _data = JSON.parse($("#model-data").text());
    let config = {
        master: {
            value: _data
        }
    };
    let coreMaster = new CoreItemMaster(config);
    $("#subperiod").children(":nth(1)").hide();
    $("#subperiod").children(":nth(1)").removeAttr("selected");
    $("#periodIndID").change(function () {
        coreMaster.updateMaster("PeroidIndID", parseInt(this.value));
    });
    $("#periodStatus").change(function () {
        coreMaster.updateMaster("PeroidStatus", parseInt(this.value));
    })
    $("#subperiod").prop("disabled", true);
    $("#fy").val(_data.FiscalYear);
    $("#updatePostingPeriod").click(function () {
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
        const id = $("#id").val();
        const noOfPeriod = $("#noOfPeriod").val();
        coreMaster.updateMaster("ID", id);
        coreMaster.updateMaster("NoOfPeroid", noOfPeriod);
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
        coreMaster.updateMaster("SubPeriod", parseInt($("#subperiod").val()));
        coreMaster.updateMaster("PeroidIndID", parseInt($("#periodIndID").val()));
        coreMaster.submitData("/postingperiod/addpostingperiod", function (res) {
            if (res.Action == 1) {
                new ViewMessage({
                    summary: {
                        selector: "#error-summary"
                    }
                }, res);
                location.href = `/postingperiod/index`;
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