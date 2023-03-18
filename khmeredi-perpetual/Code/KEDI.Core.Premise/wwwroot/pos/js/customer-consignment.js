$("#customer-consignment").click(function () {
  const __this = this;
  const dateNow = new Date();
  let detailObj = {},
    cusDetail = {},
    serials = [],
    batches = [],
    __bool = false;

  //Start Create
  $("#dateto")[0].valueAsDate = dateNow;
  this.fxCardCardExpiryPeriod = function (_this) {
    if (_this.value == 0) {
      $("#dateto")[0].valueAsDate = dateNow;
      $("#dateto").prop("readonly", false);
    } else if (_this.value == 1) {
      setDateTo(7, dateNow);
    } else if (_this.value == 2) {
      setDateTo(14, dateNow);
    }
  };

  function setDateTo(numDates, dateNow) {
    const dateto = new Date().setDate(dateNow.getDate() + numDates);
    $("#dateto")[0].valueAsDate = new Date(dateto);
    $("#dateto").prop("readonly", true);
  }

  _customer = ViewTable({
    keyField: "LineID",
    selector: "#receipt-detail",
    indexed: true,
    paging: {
      pageSize: 10,
      enabled: false,
    },
    visibleFields: ["Code", "Name", "Uom", "Qty"],
    columns: [
      {
        name: "Uom",
        template: "<select></select>",
        on: {
          change: function (e) {
            e.data.UomID = this.value;
          },
        },
      },
      {
        name: "Qty",
        template: `<input class='form-control font-size qty' type='number' />`,
        on: {
          "keyup change": function (e) {
            if (this.value > e.data.DisplayQty || this.value == 0) {
              this.value = e.data.DisplayQty;
            }
            e.data.Qty = this.value;
          },
        },
      },
    ],
    actions: [
      {
        template:
          "<i class='fas fa-trash fn-yellow fa-lg csr-pointer' title='Clear'></i>",
        on: {
          click: function (e) {
            _customer.removeRow(e.data.LineID);
            removeItemDetail(e);
          },
        },
      },
    ],
  });

  $("#find_number")
    .off("keyup")
    .keyup(function (e) {
      if (e.which == 13) {
        $.get("/CusConsignment/FindReceipt", {
          number: $("#find_number").val(),
          seriesid: $("#series-no").val(),
        }).done(function (result) {
          if (result == 0) {
            _customer.clearRows();
            valiDate();
          } else {
            var wh = result[0].Warehouse;
            var cus = result[0].Customer;
            $(".warehouse").val(wh);
            $(".customer-name").val(cus);

            _customer.clearRows();
            detailObj = result;
            _customer.bindRows(detailObj);
          }
        });
      }
    });

  this.fxSaveCus = function () {
    const data = {
      ReceiptNo: $("#find_number").val(),
      ValidDate: $("#dateto").val(),
      LengthExpire: $(".date-expiry-period").val(),
      SeriesID: $("#series-no").val(),
      ItemDetail: detailObj,
    };

    if (!isValidArray(detailObj)) {
      valiDate();
    } else {
      $.post("/CusConsignment/CreateCus", { data: data }, function () {
        __this.clearData();
        __this.success(1);
      });
    }
  };

  this.clearData = function () {
    $(".customer-name").val("");
    $(".date-expiry-period").val(0);
    $(".warehouse").val("");
    $("#series-no").val(7);
    $(".receipt-no").val("");
    $("#dateto")[0].valueAsDate = new Date();
    _customer.clearRows();
    detailObj = {};
  };
  //End of Create

  //Customer Item Details
  _itemdetail = ViewTable({
    keyField: "DetailID",
    selector: "#item-detail",
    indexed: true,
    paging: {
      pageSize: 10,
      enabled: false,
    },
    visibleFields: [
      "Code",
      "KhmerName",
      "ExpireDate",
      "UomName",
      "Qty",
      "Action",
    ],
    columns: [
      {
        name: "Qty",
        template: `<input class='form-control font-size qty' type='number' />`,
        on: {
          "keyup change": function (e) {
            if (this.value > e.data.DisplayQty || this.value == 0) {
              this.value = e.data.DisplayQty;
            }
            e.data.Qty = this.value;
            e.data.Quantity = this.value;
          },
        },
      },
    ],
    actions: [
      {
        template: "<input type='checkbox' class='input-box-kernel'>",
        on: {
          click: function (e) {
            e.data.Isselect = $(this).prop("checked");
            _itemdetail.updateColumn(e.key, "Isselect", e.data.Isselect);
          },
        },
      },
    ],
  });

  this.filter = function () {
    $.ajax({
      url: "/CusConsignment/GetCustomerItemDetails",
      data: { customer: $("#find-cus").val(), warehouse: $("#find-wh").val() },
      success: function (item) {
        if (item == 0) {
          cusDetail = {};
          _itemdetail.clearRows();

          if (!isValidArray(cusDetail)) {
            $("#withdraw-item").hide();
            $("#return-item").hide();
            $("#errorDetail").text("Item Not found!");

            setTimeout(function () {
              $("#errorDetail").text("");
            }, 1500);
            return;
          }
        } else {
          $("#withdraw-item").show();
          $("#return-item").show();
          checkDataDetail(item);
        }
      },
    });
  };

  this.fxWithdrawItem = function () {
    var cd = $.grep(_itemdetail.yield(), function (cd) {
      return cd.Isselect;
    });
    cusDetail = cd;

    const data = {
      WarehouseID: $("#find-wh").val(),
      CustomerID: $("#find-cus").val(),
      ItemDetail: cusDetail,
    };

    if (!isValidArray(cusDetail)) {
      $("#errorDetail").text("Please Select Item!");

      setTimeout(function () {
        $("#errorDetail").text("");
      }, 1500);
      return;
    } else {
      checkItemExpire("withdraw");

      if (__bool == false) {
        $.post(
          "/CusConsignment/SaveWithdrawOrReturn",
          { data: data, status: "withdraw" },
          function () {
            _itemdetail.clearRows();
            __this.success(2);
          }
        );
      }
      __bool = false;
    }
  };

  this.fxReturnItem = function () {
    var cd = $.grep(_itemdetail.yield(), function (cd) {
      return cd.Isselect;
    });
    cusDetail = cd;
    var master = cusDetail[0];

    if (!isValidArray(cusDetail)) {
      $("#errorDetail").text("Please Select Item!");

      setTimeout(function () {
        $("#errorDetail").text("");
      }, 1500);
      return;

    } else {
      const data = {
        BranchID: master.BranchID,
        UserID: master.UserID,
        WarehouseID: master.WarehouseID,
        PostingDate: master.PostingDate,
        DocumentDate: master.PostingDate,
        Ref_No: "",
        Number_No: $("#next_number").val(),
        Remark: "",
        SeriseID: master.SeriseID,
        GLID: master.GLID,
        GoodReceiptDetails: cusDetail,
      };
      checkItemExpire("return");

      if (__bool == false) {
        $.ajax({
          url: "/GoodsReceipt/SaveGoodsReceipt",
          type: "POST",
          dataType: "JSON",
          data: {
            data: JSON.stringify(data),
            serial: JSON.stringify(serials),
            batch: JSON.stringify(batches),
          },
          success: function () {
            checkItemReturns();
            filterSeries();

            _itemdetail.clearRows();
            __this.success(2);
          },
        });
      }
      __bool = false;
    }
  };

  function checkItemReturns() {
    const data = {
      ItemDetail: cusDetail,
    };
    $.post(
      "/CusConsignment/SaveWithdrawOrReturn",
      { data: data, status: "return" },
      function () { }
    );
  }

  function checkItemExpire(_check) {
    let _count = 0;
    cusDetail.forEach((i) => {
      if (_check == "withdraw") {
        if (i.ExpireDate < i.Date) {
          __bool = true;
          _count += 1;
          $("#errorDetail").text(
            "This Item " + " (" + i.KhmerName + ") " + "Can't Not Withdraw!"
          );
        }
      }
      if (_check == "return") {
        if (i.ExpireDate >= i.Date) {
          __bool = true;
          _count += 1;
          $("#errorDetail").text(
            "This Item" + " (" + i.KhmerName + ") " + "Can't Not Return!"
          );
        }
      }

      if (_count > 0) {
        setTimeout(function () {
          $("#errorDetail").text("");
        }, 1500);
        return;
      }
    });
  }

  function checkDataDetail(item) {
    _itemdetail.clearRows();
    _itemdetail.bindRows(item);

    item.forEach((i) => {
      if (i.ExpireDate < i.Date) {
        let row = _itemdetail.getRow(i.DetailID);
        $(row).addClass("bg-pink fn-white");
      }

      _itemdetail.updateColumn(i.DetailID, "ExpireDate", i.ExpireDate);
    });
  }

  this.fxItemDetail = function () {
    filterSeries();
    var cd = _itemdetail.yield();
    if (cd == 0) {
      $("#withdraw-item").hide();
      $("#return-item").hide();
      _itemdetail.clearRows();
    }
  };
  //End of Customer Item Details

  function removeItemDetail(e) {
    var id = e.data.LineID;

    if (isValidArray(detailObj)) {
      detailObj = detailObj.filter(function (obj) {
        return obj.LineID !== id;
      });
    }
  }

  function valiDate() {
    var reciptNo = $("#find_number").val();

    if (reciptNo == "") {
      $("#errorNo").text("Please Input ReceiptNo To Find Item!");
    }
    if (reciptNo != "") {
      $("#errorNo").text("ReceiptNo Not found!");
    }

    setTimeout(function () {
      $("#error").text("");
      $("#errorNo").text("");
    }, 1500);
    return;
  }

  function filterSeries() {
    $.ajax({
      url: "/CusConsignment/GetNumberSeries",
      success: function (item) {
        $("#next_number").val(item);
      },
    });
  }

  $("#find-cus")
    .off()
    .change(function (e) {
      __this.filter();
    });

  this.success = function (data) {
    if (data == 1) {
      $("#successC").text("Item Save successfully!");
    } else {
      $("#successD").text("Item Save successfully!");
    }

    setTimeout(function () {
      $("#successC").text("");
      $("#successD").text("");
    }, 1500);
  };

  $(".date-expiry-period").change(function () {
    __this.fxCardCardExpiryPeriod(this);
  });
  $("#save-cus-csm")
    .off("click")
    .click(function () {
      __this.fxSaveCus();
    });
  $("#clear-data")
    .off("click")
    .click(function () {
      __this.clearData();
    });
  $("#filter-item")
    .off("click")
    .click(function () {
      __this.filter();
    });
  $("#withdraw-item")
    .off("click")
    .click(function () {
      __this.fxWithdrawItem();
    });
  $("#return-item")
    .off("click")
    .click(function () {
      __this.fxReturnItem();
    });
  $("#tab-itemdetail").click(function () {
    __this.fxItemDetail();
  });

  function isValidArray(values) {
    return Array.isArray(values) && values.length > 0;
  }
});

$(document).ready(function () {
  $.ajax({
    url: "/CusConsignment/GetSeries",
    type: "Get",
    dataType: "Json",
    success: function (respones) {
      var data = "";
      $.each(respones, function (i, item) {
        data += '<option value="' + item.ID + '">' + item.Name + "</option>";
      });
      $("#series-no").append(data);
    },
  });

  $.ajax({
    url: "/DevReport/GetCustomer",
    type: "Get",
    dataType: "Json",
    success: function (respones) {
      var data = "";
      $.each(respones, function (i, item) {
        data += '<option value="' + item.ID + '">' + item.Name + "</option>";
      });
      $("#find-cus").append(data);
    },
  });

  $.ajax({
    url: "/DevReport/GetWarehouseStock",
    type: "Get",
    dataType: "Json",
    success: function (respones) {
      var data = "";
      $.each(respones, function (i, item) {
        data += '<option value="' + item.ID + '">' + item.Name + "</option>";
      });
      $("#find-wh").append(data);
    },
  });
});
