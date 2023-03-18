$(document).ready(function () {
    const serviceContractTable = ViewTable({
        keyField: "LineID",
        selector: ".list-Service-contracts",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: true
        },
        visibleFields: ["ContractName", "StartDate", "EndDate", "TerminationDate", "ServiceType"],
    })
    const transactions = ViewTable({
        keyField: "LineID",
        selector: ".list-Service-transaction",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: true
        },
        visibleFields: ["Source", "DocumentNo", "Date", "WhsName", "GLAccBPCode", "GLAccBPName", "Direction"],
    })
    const serviceCalls = ViewTable({
        keyField: "LineID",
        selector: ".list-Service-calls",
        indexed: true,
        paging: {
            pageSize: 10,
            enabled: true
        },
        visibleFields: ["Creation", "CustomerName", "Subject", "ItemCode", "ItemName"],
    })
    $("#mfr-serial-no").on("keypress", function (e) {
        SearchData(e, this.value, "MfrSerial");
    });
    $("#serial-number").on("keypress", function (e) {
        SearchData(e, this.value, "serialnumber");
    });
    $("#plate-number").on("keypress", function (e) {
        SearchData(e, this.value, "platenumber");
    });


    function SearchData(e,value,type) {
        if (e.which === 13) {
            $("#loading").prop("hidden", false)
            $.get(
                '/Service/GetEquipmentMaster',
                {
                    serialNumber: value.trim(), type: type
                },
                function (res) {
                    serviceContractTable.clearRows()
                    transactions.clearRows()
                    serviceCalls.clearRows()
                    if (res.IsError) {
                        clearData()
                        new DialogBox({
                            content: res.Error.Message,
                        });
                    } else {
                        bindData(res)
                        serviceContractTable.addRow(res.ServiceContract)
                        transactions.bindRows(res.Transactions)
                        serviceCalls.bindRows(res.ServiceCalls)
                    }
                    $("#loading").prop("hidden", true)
                }
            )
        }
    }
    function bindData(data) {
        $("#mfr-serial-no").val(data.MfrSerialNo);
        $("#serial-number").val(data.SerialNumber);
        $("#item-code").val(data.ItemCode);
        $("#bp-code").val(data.BPCode);
        $("#bp-name").val(data.BPName);
        $("#technician").val(data.Technician);
        $("#territory").val(data.Territory);
        $("#territory").val(data.Territory);
        $("#telephone-no").val(data.TelephoneNo);
        $("#plate-number").val(data.PlateNumber);
    }
    function clearData() {
        $("#mfr-serial-no").val("");
        $("#item-code").val("");
        $("#bp-code").val("");
        $("#bp-name").val("");
        $("#technician").val("");
        $("#territory").val("");
        $("#territory").val("");
        $("#telephone-no").val("");
        $("#plate-number").val("");
    }
});