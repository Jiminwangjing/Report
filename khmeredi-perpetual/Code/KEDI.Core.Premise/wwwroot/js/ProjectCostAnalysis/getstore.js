$(function () {
    $("#txt_cusname").click(() => {
        const dialogprojmana = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: "List Customer",
            content: {
                selector: "#customer-content"
            }
        });
        dialogprojmana.confirm(function () {
            dialogprojmana.shutdown();
        });
        dialogprojmana.invoke(function () {
            /// Bind Customers /// 
            const $listCustomers = ViewTable({
                keyField: "ID",
                selector: "#list-customers",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: [
                    "Code",
                    "Name",
                    "Type",
                    "Phone",
                ],

                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                $("#txt_cusname").val(e.data.Name);
                                $("#txt_cuscode").val(e.data.Code);
                                $("#txt_idcus").val(e.data.ID);

                                $("#txtcopy-quote").prop('disabled', false);
                              //  $("#item-id").prop("disabled", false);
                                /* master[0].CusID = e.data.ID;*/
                                dialogprojmana.shutdown();
                            }
                        }
                    }
                ]
            });
            GetCustomer($listCustomers);
        });

    })

    $("#txt_contactperson").click(() => {
        const dialogprojperson = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: "List Customer",
            content: {
                selector: "#customer-content"
            }
        });
        dialogprojperson.confirm(function () {
            dialogprojperson.shutdown();
        });
        dialogprojperson.invoke(function () {
            /// Bind Customers /// 
            const $listCustomers = ViewTable({
                keyField: "ID",
                selector: "#list-customers",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: [
                    "Code",
                    "Name",
                    "Type",
                    "Phone",
                ],

                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                $("#txt_contactperson").val(e.data.Name);
                                $("#txt_phone").val(e.data.Phone);
                                $("#txt_idcontect").val(e.data.ID);
                                master[0].ConTactID = e.data.ID;

                                dialogprojperson.shutdown();
                            }
                        }
                    }
                ]
            });
            GetCustomer($listCustomers);
        });

    })




    $("#saleem").click(function () {
        const dialogem = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: "List Employee",
            content: {
                selector: "#customer-content"
            }
        });
        dialogem.confirm(function () {
            dialogem.shutdown();
        });
        dialogem.invoke(function () {
            /// Bind Customers /// 
            const $listem = ViewTable({
                keyField: "ID",
                selector: "#list-customers",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: [
                    "Code",
                    "Name",
                    "Type",
                    "Phone",
                ],

                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                $("#txt_idem").val(e.data.ID);
                                $("#saleem").val(e.data.Name);
                                master[0].SaleEMID = e.data.ID;
                                dialogem.shutdown();
                            }
                        }
                    }
                ]
            });
            GetEmployee($listem);
        });
    })



    $("#owner").click(function () {
        const dialogown = new DialogBox({

            button: {
                ok: {
                    text: "Close",
                }
            },
            caption: "List Owner",
            content: {
                selector: "#customer-content"
            }
        });
        dialogown.confirm(function () {
            dialogown.shutdown();
        });
        dialogown.invoke(function () {
            /// Bind Customers /// 
            const $listem = ViewTable({
                keyField: "ID",
                selector: "#list-customers",
                indexed: true,
                paging: {
                    pageSize: 10,
                    enabled: true
                },
                visibleFields: [
                    "Code",
                    "Name",
                    "Type",
                    "Phone",
                ],

                actions: [
                    {
                        template: "<i class='fas fa-arrow-circle-down'></i>",
                        on: {
                            "click": function (e) {
                                $("#txt_idowner").val(e.data.ID);
                                $("#owner").val(e.data.Name);
                                master[0].OwnerID = e.data.ID;
                                dialogown.shutdown();
                            }
                        }
                    }
                ]
            });
            GetEmployee($listem);
        });
    })
    // Modal Customer
    function GetCustomer(nametable) {
        $.get("/ProjectCostAnalysis/GetCustomer", function (res) {

            nametable.bindRows(res)
        })
    }
    function GetEmployee(nametable) {
        $.get("/ProjectCostAnalysis/GetSaleEmployee", function (res) {

            nametable.bindRows(res);
        })
    }



    $.get("/ProjectCostAnalysis/GetPriceList", function (response) {
        $("#cur-id option:not(:first-child)").remove();
        $.each(response, function (i, item) {
            $("#cur-id").append("<option  value=" + item.CurrencyID + ">" + item.Name + "</option>");
        });
    })
    $("#txtSearchCustomer").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#list-customers tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });
    $("#txtSearchitemmaster").on("keyup", function (e) {
        var value = $(this).val().toLowerCase();
        $("#list-item tr:not(:first-child)").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

   
    




});