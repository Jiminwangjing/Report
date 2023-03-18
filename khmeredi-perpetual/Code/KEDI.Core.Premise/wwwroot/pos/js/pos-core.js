"use strict";
function PosCore(config) {
  if (!(this instanceof PosCore)) {
    return new PosCore(config);
  }

  let jsonText = $("#print-template-url").text();
  const _printUrlPOS = isEmpty(jsonText)
    ? {}
    : JSON.parse(jsonText).Templateurl;
  const __this = this;
  var __config = {
    orderOnly: false,
    master: {
      keyField: "",
    },
    detail: {
      keyField: "",
      fieldName: "OrderDetail",
    },
    services: [],
    events: {
      load: [],
    },
  };

  if (isValidJson(config)) {
    __config = $.extend(true, __config, config);
  }

  var __signalR = undefined;
  var __orderStatus = { Sent: 1, Bill: 2, Paid: 3 };
  var __loadComplete = false;
  var __orderDetail = {};
  var __orderInfo = {
      OrderDetail: {},
      Setting: {
        ItemPageSize: 12,
      },
    },
    __batchSerial = {
      batches: null,
      serial: null,
    },
    __fallbackPanelPayment = function () {};
  let __loadscreen = "#loadscreen",
    __notFound = "#image-not-found",
    __btnPanelTable = ".goto-panel-group-tables",
    __btnPanelPayment = "#goto-panel-payment",
    __btnPanelOrder = "#goto-panel-group-items",
    __wrapGridTimeTable = "#table-item-gridview .wrap-grid",
    __itemListview = "#item-listview",
    __wrapGridItem = "#group-item-gridview .wrap-grid",
    __textSearch = "#item-search",
    __barcodeSearch = ".item-search-barcode",
    __listOrderReceipt = "#list-order-receipt",
    __groupItemStep = ".group-item-steps",
    __isScanQrcode = false,
    __encryptedTableID = "",
    __itemlistcomment = "#item-listcomment";

  const __template = {
      div: document.createElement("div"),
      i: document.createElement("i"),
    },
    $gridItemPaging = DataPaging({
      container: "#pagination-list",
      keyField: "ID",
      pageSize: __orderInfo.Setting.ItemPageSize,
      startIndexSize: 4,
    }),
    $listOrderedItems = ViewTable({
      selector: __itemListview,
      keyField: "LineID",
      visibleFields: [
        "Code",
        "KhmerName",
        "UnitPrice",
        "Qty",
        "ItemUoMs",
        "DiscountRate",
        "TaxGroups",
        "Total",
      ],
      paging: {
        //pageSize: 11,
        enabled: false,
      },
      dataSynced: false,
      dynamicCol: {
        afterAction: true,
        headerContainer: "#col-to-append-after",
      },
      columns: [
        {
          name: "KhmerName",
          title: {
            enabled: true,
          },
          template:
            "<div title='Double click to adddon' class='csr-pointer'></div>",
          on: {
            dblclick: function (e) {
              //PromoType = 0 //Not promo item
              if (
                e.data.PromoType == 0 &&
                e.data.ItemType.toLowerCase() !== "addon"
              ) {
                __orderInfo.OrderDetail = e.data;
                resetGroupItems(true);
                if (__orderInfo.Setting.PanelViewMode == 1) {
                  __this.listItemAddons();
                }
              } else {
                __this.dialog(
                  "Unavailable Addon",
                  "The item is not available for addon.",
                  "warning"
                );
              }
            },
          },
        },
        {
          name: "UnitPrice",
          dataType: "number",
          title: {
            enabled: true,
            text: "Double click to copy the item.",
          },
          template: "<div class='csr-pointer'></div>",
          on: {
            dblclick: function (e) {
              if (e.data.ItemType.toLowerCase() !== "addon") {
                __this.copyLineItem(e.data);
              }
            },
          },
        },
        {
          name: "ItemUoMs",
          template: "<select>",
          on: {
            change: function (e) {
              var uomVal = parseInt(this.value);
              changeItemByUoM(e.key, uomVal, e.data.Qty);
            },
          },
        },
        {
          name: "TaxGroups",
          template: "<select>",
          on: {
            change: function (e) {
              e.data.TaxGroupID = this.value;
              changeItemByTaxGroup(e.data);
            },
          },
        },
        {
          name: "Total",
          dataType: "number",
          //dataFormat: { fixed: baseCurrency?.DecimalPlaces }
        },
        {
          name: "Code",
          title: {
            enabled: true,
            text: "Click to comment on the item.",
          },
          template: "<span class='csr-pointer'></span>",
          on: {
            click: function (e) {
              __this.commentItem(e.data);
            },
          },
        },
        {
          name: "AddictionProps",
          valueDynamicProp: "ValueName",
          dynamicCol: true,
        },
      ],
      scaleColumns: [
        {
          name: "Qty",
          dataType: "number",
          onScaleDown: {
            click: function (e) {
              if (__isScanQrcode) {
                updateLineItemQR(e.key, -1, false);
              } else {
                updateLineItem(e.key, -1, false);
              }
            },
          },
          onScaleUp: {
            click: function (e) {
              if (__isScanQrcode) {
                updateLineItemQR(e.key, 1, false);
              } else {
                updateLineItem(e.key, 1, false);
              }
            },
          },
        },
      ],
      actions: [
        {
          template:
            "<i title='Comments' class='icon-calculator csr-pointer far fa-comment-dots fa-fw fn-blue'></i>",
          on: {
            click: function (e, event) {
              __this.commentItem(e.data);
            },
          },
        },
        {
          template:
            "<i title='Addons' class='icon-calculator csr-pointer fas fa-plus-circle fa-fw fn-orange'></i>",
          on: {
            click: function (e, event) {
              __orderInfo.OrderDetail = e.data;
              resetGroupItems(true);
              if (__orderInfo.Setting.PanelViewMode == 1) {
                __this.listItemAddons();
              }
            },
          },
        },

        {
          template:
            "<i title='Calculator' class='icon-calculator csr-pointer fas fa-calculator fa-fw fn-sky'></i>",
          on: {
            click: function (e, event) {
              openLineCalculator(event, e.data);
            },
          },
        },
        {
          template:
            "<i title='Delete item' class='fas fa-trash-alt csr-pointer fn-red'></i>",
          on: {
            click: function (e) {
              if (__isScanQrcode) __this.deleteLineItemQR(e.data);
              else __this.deleteLineItem(e.data, true);
            },
          },
        },
      ],
    }),
    $itemListView = ViewTable({
      container: "#group-item-listview",
      keyField: "ID",
      dynamicCol: {
        afterAction: true,
        headerContainer: "#item-col-append",
      },
      visibleFields: ["Code", "KhmerName", "UnitPrice", "InStock", "UoM"],
      columns: [
        {
          name: "AddictionProps",
          valueDynamicProp: "ValueName",
          dynamicCol: true,
        },
      ],
      actions: [
        {
          template:
            "<i class='fas fa-arrow-circle-left fa-lg fn-gainsboro csr-pointer'></i>",
          on: {
            click: function (e) {
              if (__isScanQrcode) updateLineItemQR(e.key, 1, true);
              else updateLineItem(e.key, 1, true);
            },
          },
        },
      ],
      paging: {
        enabled: false,
      },
    });

  var __urls = {
    fetchOrderInfo: "/POS/FetchOrderInfo",
    getTablesByGroup: "/POS/GetTablesByGroup",
    searchTables: "/POS/SearchTables",
    searchSaleItems: "/POS/SearchSaleItems",
    submitOrder: "/POS/SubmitOrder",
    findByBarcode: "/POS/FindLineItemByBarcode",
    submitUserSetting: "/POS/UpdateSetting",
    checkOpenShift: "/POS/CheckOpenShift",
    getServiceTables: "/POS/GetServiceTables",
    getUserSetting: "/POS/GetUserSetting",
    getItemComments: "/POS/GetItemComments",
    saveItemComment: "/POS/SaveItemComment",
    deleteItemComment: "/POS/DeleteItemComment",
    checkPrivilege: "/POS/CheckPrivilege",
    getUserAccess: "/POS/GetUserAccess",
    getItemGroups: "/POS/GetGroupItems",
    submitVoidOrder: "/POS/VoidOrder",
    moveOrders: "/POS/moveOrders",
    changeTable: "/POS/ChangeTable",
    processOpenShift: "/POS/ProcessOpenShift",
    processCloseShift: "/POS/ProcessCloseShift",
    getShiftTemplate: "/POS/GetShiftTemplate",
    submitVoidItem: "/POS/VoidItem",
    submitPendingVoidItem: "/POS/PendingVoidItem",
    // QR Url
    fetchOrderInfoQR: "/api/v1/QRCodeAPI/fetchOrderInfoQR",
    getNewOrderDetail: "/api/v1/QRCodeAPI/GetNewOrderDetail",
    getItemGroupQR: "/api/v1/QRCodeAPI/GetGroupItems",
    sendQR: "/api/v1/QRCodeAPI/Send",
    checkPrivilegeQR: "/api/v1/QRCodeAPI/CheckPrivilege/",
  };
  this.getListViewItemOrder = function () {
    return $listOrderedItems;
  };
  this.readyPayment = false;
  this.instance = {};
  this.start = function (callback) {
    if (isValidArray(config.services)) {
      for (let i = 0; i < config.services.length; i++) {
        if (typeof eval(config.services[i]) === "function") {
          __this.instance[config.services[i]] = eval(config.services[i])(
            __this
          );
        }
      }
    }
    __this.fetchOrderInfo(0, 0, 0, !__config.orderOnly);
    return this;
  };

  //SignalR Implementations
  this.startSignalR = function (onConnected) {
    var connection = new signalR.HubConnectionBuilder()
      .withUrl("/realbox")
      .configureLogging(signalR.LogLevel.Information)
      .build();
    connection.onclose((e) => {
      console.info("Connection closed!", e);
    });

    connection
      .start()
      .then(function () {
        if (typeof onConnected === "function") {
          onConnected.call(connection, __this.fallbackInfo());
        }
      })
      .catch(function (err) {
        connection = __this.startSignalR(onConnected);
        return console.error("Error connection->:" + err.toString());
      });

    __signalR = connection;
    return connection;
  };

  function allow2ndScreen() {
    return __orderInfo.Setting.DaulScreen && !isEmpty(__signalR);
  }

  this.clearOrder2ndScreen = function () {
    if (allow2ndScreen()) {
      __signalR.invoke("ClearOrder");
    }
  };

  this.start2ndScreen = function (tableId, orderId, customerId) {
    if (allow2ndScreen()) {
      __signalR.invoke(
        "LoadOrderInfo",
        tableId,
        orderId,
        customerId,
        __config.orderOnly
      );
    }
  };

  this.sendOrder2ndScreen = function () {
    if (allow2ndScreen()) {
      __signalR.invoke(
        "SendOrder",
        JSON.stringify(__orderInfo.Order),
        JSON.stringify(__orderInfo.OrderTable)
      );
    }
  };

  this.changeViewMode2ndScreen = function () {
    if (allow2ndScreen()) {
      __signalR.invoke("ChangeViewMode", JSON.stringify(__orderInfo.Setting));
    }
  };

  this.getOrderInfo = function (callback) {
    if (typeof callback === "function") {
      callback.call(__this, __orderInfo);
    }
    return __orderInfo;
  };
  $("#panel-view-mode").on("click", function () {
    var viewMode = $(this).data("view-mode");
    switchPanelViewMode(viewMode);
  });

  $("#panel-resizer").on("click", function () {
    $(this).toggleClass("fa-compress fa-expand");
    $("#panel-group-items .left-panel").toggleClass("width-zero");
  });

  //Search table grids
  $(__btnPanelPayment).on("click", function () {
    __this.checkCart(function () {
      __this.gotoPanelPayment(__fallbackPanelPayment);
    });
  });

  $(".back-to-listview").on("click", function () {
    $(this).hide().siblings().hide();
    $("#panel-group-items .right-panel")
      .addClass("width-zero")
      .find("#right-search-box")
      .hide();
  });

  $(".back-to-gridview").on("click", function () {
    $(".back-to-listview").show().siblings().show();
    $("#panel-group-items .right-panel")
      .removeClass("width-zero")
      .find("#right-search-box")
      .show();
  });

  $(__btnPanelTable).on("click", function () {
    __this.gotoPanelGroupTable();
  });

  $(__btnPanelOrder).on("click", function () {
    __this.gotoPanelItemOrder();
  });

  $("#search-tables").on("keyup", function () {
    __this.bindServiceTables(this.value);
  });

  $("#group-tables > *").on("click", function () {
    let groupId = $(this).data("group-id");
    __this.loadScreen();
    $(this).addClass("active").siblings().removeClass("active");
    $.post(__urls.getTablesByGroup, { groupId: groupId }, function (tables) {
      setTimeTableGrids(tables);
      __this.loadScreen(false);
    });
  });

  $(__wrapGridTimeTable).children(".grid").on("click", onClickTimeTableGrid);
  $(__textSearch).on("keyup", function (e) {
    let keyword = this.value;
    if (keyword === "") {
      showItemsByViewType(__orderInfo.SaleItems);
    } else {
      var _items = searchSaleItems(__orderInfo.SaleItems, keyword);
      if (isValidArray(_items)) {
        showItemsByViewType(_items);
      } else {
        delay(
          $.ajax({
            url: __urls.searchSaleItems,
            type: "POST",
            data: {
              orderId: __orderInfo.Order.OrderID,
              keyword: keyword,
            },
            timeout: 3000,
            success: function (items) {
              if (isValidArray(items)) {
                showItemsByViewType(items);
                __this.addSaleItems(items);
              }
            },
          })
        );
      }
    }
  });

  function delay(callback, ms = 500) {
    var timer = 0;
    return function () {
      var context = this,
        args = arguments;
      clearTimeout(timer);
      timer = setTimeout(function () {
        callback.apply(context, args);
      }, ms || 0);
    };
  }

  $(__barcodeSearch).on("change", function () {
    let barcode = this.value.replace(/\s/g, "");
    let items = $.grep(__orderInfo.SaleItems, function (item) {
      if (isEmpty(item.Barcode)) {
        return false;
      }
      let _barcode = item.Barcode.replace(/\s/g, "");
      return _barcode == barcode;
    });
    $.post(
      __urls.findByBarcode,
      {
        orderId: __orderInfo.Order.OrderID,
        pricelistId: __orderInfo.Order.PriceListID,
        barcode: barcode,
      },
      function (item) {
        //if (item.Error) {
        //    const errorDialog = __this.dialog("Item Out Of Stock", item.Message, "info");
        //    errorDialog.confirm(function () {
        //        $(".item-search-barcode").focus()
        //    })
        //}

        if (!isEmpty(item.LineID)) {
          let od = $listOrderedItems.find(item.LineID);
          if (item.SerialNumberSelectedDetial) {
            if (isValidArray(__batchSerial.serial)) {
              const itemSerial = findInArray(
                "ItemID",
                item.ItemID,
                __batchSerial.serial
              );
              if (itemSerial && itemSerial?.SerialNumberSelected) {
                const serialNumber = findInArray(
                  "SerialNumber",
                  item.SerialNumberSelectedDetial.SerialNumber,
                  itemSerial.SerialNumberSelected.SerialNumberSelectedDetails
                );
                if (!serialNumber) {
                  itemSerial.SerialNumberSelected.SerialNumberSelectedDetails.push(
                    item.SerialNumberSelectedDetial
                  );
                  itemSerial.SerialNumberSelected.TotalSelected =
                    itemSerial.SerialNumberSelected.SerialNumberSelectedDetails.length;
                  itemSerial.BaseQty = od ? od.Qty + 1 : item.Qty;
                  itemSerial.OpenQty = itemSerial.BaseQty;
                } else {
                  return;
                }
              }
            } else {
              if (!__batchSerial.serial) __batchSerial.serial = new Array();
              item.SerialNumber.BpId = __orderInfo.Setting.CustomerID;
              item.SerialNumber.UomID = item.UomID;
              item.SerialNumber.BaseQty = od ? od.Qty + 1 : item.Qty;
              item.SerialNumber.OpenQty = item.SerialNumber.BaseQty;
              __batchSerial.serial.push(item.SerialNumber);
            }
          }
          if (isValidJson(od)) {
            item.Qty += od.Qty;
            item.PrintQty += od.PrintQty;
            item.Total += od.Total;
          }
          __this.sumLineItem(item);
          $listOrderedItems.updateRow(item);
          items = $.grep(__orderInfo.SaleItems, function (_item) {
            return _item.ID == item.LineID;
          });
          $(__wrapGridItem).children().remove();
          bindSingleItemGrids(items);
          updateItemGridView(item);
        }
      }
    );
    this.value = "";
  });

  function getImage(imagePath) {
    return imagePath
      ? "/Images/items/" + imagePath
      : "/Images/default/no-image.jpg";
  }

  this.removeSrialFromItem = function (item) {
    const serialsItem = findInArray(
      "ItemID",
      item.ItemID,
      __batchSerial.serial
    );
    if (serialsItem) {
      if (
        serialsItem.SerialNumberSelected.TotalSelected == item.Qty + 1 &&
        item.Qty > 0
      ) {
        const serialItemDialog = new DialogBox({
          caption: `List Serials of Item "${item.KhmerName}"`,
          icon: "fas fa-tag",
          content: {
            selector: "#serials-item-dailog",
          },
          button: {
            ok: {
              text: "Close",
            },
          },
        });
        serialItemDialog.invoke(function () {
          var serialItemTable = ViewTable({
            selector: "#list-serials-item",
            indexed: true,
            keyField: "LineID",
            visibleFields: ["SerialNumber"],
            actions: [
              {
                template: "<i class='fas fa-trash-alt fn-red csr-pointer'></i>",
                on: {
                  click: function (e) {
                    serialsItem.SerialNumberSelected.SerialNumberSelectedDetails =
                      serialsItem.SerialNumberSelected.SerialNumberSelectedDetails.filter(
                        (i) => i.SerialNumber !== e.data.SerialNumber
                      );
                    serialsItem.SerialNumberSelected.TotalSelected =
                      serialsItem.SerialNumberSelected.SerialNumberSelectedDetails.length;
                    serialItemDialog.shutdown();
                  },
                },
              },
            ],
          });
          serialItemTable.bindRows(
            serialsItem.SerialNumberSelected.SerialNumberSelectedDetails
          );
        });
        serialItemDialog.confirm(function () {
          item.Qty = serialsItem.SerialNumberSelected.TotalSelected;
          item.PrintQty = item.Qty;
          __this.sumLineItem(item);
          updateItemGridView(item);
          $listOrderedItems.updateRow(item);
          __this.displayTax();
          serialItemDialog.shutdown();
        });
      } else {
        serialsItem.SerialNumberSelected.SerialNumberSelectedDetails = [];
        serialsItem.SerialNumberSelected.TotalSelected = 0;
      }
    }
  };
  //Add new Void  item
  this.voidItem = function (success) {
    if (__orderInfo.Order.OrderID <= 0) {
      __this.dialog(
        "Void Item",
        "Please send before making void item!",
        "info"
      );
    } else {
      var _order = $.extend(true, {}, __orderInfo.Order);
      var _orderDetails = $.grep(
        _order[__config.detail.fieldName],
        function (item) {
          return item.IsVoided === true;
        }
      );
      _order[__config.detail.fieldName] = _orderDetails;

      $.post(__urls.submitVoidItem, { order: _order }, success);
    }
  };

  this.pendingVoidItem = function (success) {
    if (__orderInfo.Order.OrderID <= 0) {
      __this.dialog(
        "Pending Void Item",
        "Please send before making pending void item!",
        "info"
      );
    } else {
      var _order = $.extend(true, {}, __orderInfo.Order);
      var _orderDetails = $.grep(
        _order[__config.detail.fieldName],
        function (item) {
          return item.IsVoided === true;
        }
      );
      _order[__config.detail.fieldName] = _orderDetails;

      $.post(__urls.submitPendingVoidItem, { order: _order }, success);
    }
  };

  this.listItemAddons = function () {
    var addons = $.grep(__orderInfo.SaleItems, function (item) {
      return item.ItemType.toLowerCase() === "addon";
    });

    var $dialog = new DialogBox({
      caption: "List of items for sale",
      icon: "fas fa-tag",
      content: {
        selector: "#group-item-listview",
      },
      button: {
        ok: {
          text: "Close",
          callback: function () {
            $dialog.shutdown();
          },
        },
      },
    });

    $dialog.invoke(function () {
      $dialog.content.find(".group-search-boxes").removeClass("hidden");
      var $itemListView = ViewTable({
        selector: "#list-sale-items",
        keyField: "ID",
        dynamicCol: {
          afterAction: true,
          headerContainer: "#item-col-append",
        },
        visibleFields: ["Code", "KhmerName", "UnitPrice", "UoM"],
        columns: [
          {
            name: "AddictionProps",
            valueDynamicProp: "ValueName",
            dynamicCol: true,
          },
        ],
        actions: [
          {
            template:
              "<i class='fas fa-arrow-circle-down fn-sky fa-lg csr-pointer'></i>",
            on: {
              click: function (e) {
                addonLineItem(e.key, __orderInfo.OrderDetail.LineID);
                $dialog.shutdown();
              },
            },
          },
        ],
      });
      if (isValidArray(addons)) {
        $itemListView.clearHeaderDynamic(addons[0]["AddictionProps"]);
        $itemListView.createHeaderDynamic(addons[0]["AddictionProps"]);
      }
      $itemListView.bindRows(addons);
      let $inputSearch = $dialog.content.find("#search-sale-items");
      $inputSearch.on("keyup", function () {
        $itemListView.clearRows();
        var searcheds = searchSaleItems(addons, this.value);
        $itemListView.bindRows(searcheds);
      });
    });

    return addons;
  };

  function searchSaleItems(saleItems, keyword = "") {
    let input = keyword.replace(/\s/g, "");
    let regex = new RegExp(input, "i");
    var filtereds = $.grep(saleItems, function (item, i) {
      var nameEn = isEmpty(item.EnglishName)
        ? ""
        : item.EnglishName.replace(/\s/g, "");
      var uom = isEmpty(item.UoM) ? "" : item.UoM.replace(/\s/g, "");
      return (
        regex.test(item.KhmerName.replace(/\s/g, "")) ||
        regex.test(nameEn) ||
        regex.test(item.Code.replace(/\s/g, "")) ||
        regex.test(item.UnitPrice.toString().replace(/\s/g, "")) ||
        regex.test(uom) ||
        regex.test(item.Barcode)
      );
    });
    return filtereds;
  }

  this.addSaleItems = function (saleItems = []) {
    let _saleItems = saleItems.filter(function (item) {
      return !__orderInfo.SaleItems.some(function (sItem) {
        return item.ID == sItem.ID;
      });
    });
    __orderInfo.SaleItems = __orderInfo.SaleItems.concat(_saleItems);
    return __orderInfo.SaleItems;
  };

  this.bindServiceTables = function (keyword = "") {
    let tables = __this.getTablesByGroup(0);
    if (isEmpty(keyword)) {
      setTimeTableGrids(tables);
    } else {
      keyword = keyword.replace(/\s/g, "");
      let pattern = "[^,]*" + keyword + "[,$]*";
      let regex = new RegExp(pattern, "i");
      let founds = $.grep(tables, function (t) {
        return t.Name.replace(/\s/g, "").match(regex);
      });
      setTimeTableGrids(founds);
    }
  };

  this.getTablesByGroup = function (groupId = 0) {
    let tables = [];
    let groupTables = __orderInfo.GroupTables;
    if (isValidArray(groupTables)) {
      if (groupId > 0) {
        groupTables = groupTables.filter((gt) => gt.ID == groupId);
      }

      for (let i = 0; i < groupTables.length; i++) {
        tables = tables.concat(groupTables[i].Tables);
      }
      return tables;
    }
  };

  this.findTableById = function (tableId) {
    var tables = __this.getTablesByGroup();
    return findInArray("ID", tableId, tables);
  };

  this.changeStatusTimeTableSignalR = function (timeTables) {
    for (let i = 0; i < timeTables.length; i++) {
      let tGrid = $(__wrapGridTimeTable).children(
        "[data-id='" + timeTables[i].ID + "']"
      );
      let timeLabel = $(tGrid).find(".grid-image .time");
      $(timeLabel).text(timeTables[i].DurationText);
      switch (timeTables[i].Status) {
        case "A":
          $(tGrid).css("background-color", "#CC9"); //white
          break;
        case "B":
          $(tGrid).css("background-color", "#EC3E3E"); //red
          break;
        case "P":
          $(tGrid).css("background-color", "#50A775"); // green
          break;
      }
    }
  };

  function setTimeTableGrids(tables) {
    $(__wrapGridTimeTable).children().remove();
    $.each(tables, function (i, table) {
      if (!table.Image) {
        table.Image = "no-image.jpg";
      }
      if (!table.Time) {
        table.Time = "";
      }
      let userid = "user" + table.ID;
      let $grid = $(
        "<div data-id='" +
          table.ID +
          "' data-name='" +
          table.Name +
          "' class='grid'></div>"
      ).append('<div class="grid-caption">' + table.Name + "</div>");
      let $grid_image = $("<div class='grid-image'></div>");
      switch (table.Status) {
        case "A":
          $grid.css("background-color", "#CC9");
          break;
        case "B":
          $grid.css("background-color", "#EC3E3E");
          break;
        case "P":
          $grid.css("background-color", "#50A775");
          break;
        default:
          $grid.css("background-color", "#CC9");
          break;
      }
      $grid_image.append(
        '<img src="/Images/table/' +
          table.Image +
          '"/><div class="time">' +
          table.Time +
          "</div>"
      );
      $grid.append($grid_image);
      $grid.append("<div id='" + userid + "' class='grid-subtitle'></div>");
      $(__wrapGridTimeTable).append($grid.on("click", onClickTimeTableGrid));
    });
  }
  var text_refno = "";
  function onClickTimeTableGrid() {
    if (__loadComplete) {
      // $.get("/POS/CheckPromotionDiscount");
      var tableId = $(this).data("id");
      var table = __this.findTableById(tableId);
      if (!isValidJson(table)) {
        return;
      }
      if (table.Type == 2 || table.Type == 3) {
        let dialog = new DialogBox({
          caption: "RefNo",
          icon: "fas fa-user-friends",
          content: {
            selector: "#dialog-delvery-takaway",
          },
          button: {
            ok: {
              text: "Save",
            },
          },
        });
        dialog.invoke(function () {
          dialog.content.find("#delvery-takaway");
        });
        dialog.confirm(function () {
          if (!$("#delvery-takaway").val()) {
            $("#text_refno").show();
            setTimeout(function () {
              $("#text_refno").text("");
            }, 3000);
            return;
          }

          text_refno = $("#delvery-takaway").val();
          if (__loadComplete) {
            if (tableId !== __orderInfo.OrderTable.ID) {
              __this.loadCurrentOrderInfo(tableId, 0);
            }
          } else {
            __this.loadCurrentOrderInfo(tableId, 0);
          }

          if (__orderInfo.OrderTable.ID == tableId) {
            __this.sendOrder2ndScreen();
          }

          undefinedrefno = "undefined";
          __this.displayOrderTitle("#order-number", __config.orderOnly);
          dialog.shutdown();
          __this.gotoPanelItemOrder();
        });
        return;
      }

      if (__orderInfo.OrderTable.ID == tableId) {
        __this.sendOrder2ndScreen();
      }

      if (tableId !== __orderInfo.OrderTable.ID) {
        __this.loadCurrentOrderInfo(tableId, 0);
      }
      __this.gotoPanelItemOrder();
    }
  }

  this.loadCurrentOrderInfo = function (
    tableId,
    orderId,
    customerId = 0,
    newOrder = false,
    displayLineItems = true
  ) {
    __this.loadScreen();
    return $.post(
      "/pos/getCurrentOrderInfo",
      {
        tableId: tableId,
        orderId: orderId,
        customerId: customerId,
        newOrder: newOrder,
      },
      function (orderInfo) {
        __orderInfo = $.extend(__orderInfo, orderInfo);
        __this.setOrder(__orderInfo.Order, true, displayLineItems);
        displaySettings();
        __this.loadScreen(false);
        //logSizeInBytes("orderInfo size in byte ", getSizeInBytes(orderInfo));
      }
    );
  };

  this.showNotFound = function (enabled = true) {
    if (enabled) {
      $(__notFound).removeClass("hide");
    } else {
      $(__notFound).addClass("hide");
    }
  };
  function disableTarget(target, enabled = false) {
    if (enabled) {
      $(target).css("pointer-events", "auto");
    } else {
      $(target).css("pointer-events", "none");
    }
  }
  //Prccess add comment to item order detail.
  this.commentItem = function (orderDetail) {
    const dialog = new DialogBox({
      caption: "Comment",
      icon: "fas fa-comments",
      content: {
        selector: "#comment-item-content",
      },
      type: "ok-cancel",
      button: {
        ok: {
          text: "Apply",
        },
      },
    });

    dialog.invoke(function () {
      let itemComments = new Array();
      let $item = dialog.content.find("#detail-item-comment");
      $item.append(orderDetail.KhmerName + " (" + orderDetail.Code + ")");
      var $chosenComments = ViewTable({
        selector: "#listview-choosed-item-comments",
        visibleFields: ["Index", "Description"],
        keyField: "ID",
        actions: [
          {
            template: "<i class='fas fa-trash-alt fn-red csr-pointer'></i>",
            on: {
              click: function (e) {
                removeComment(e.data);
              },
            },
          },
        ],
      });
      itemComments.forEach((item, index) => {
        $chosenComments.addRow({
          Index: index + 1,
          ID: new Date().getTime() + (index + 1),
          Description: item,
          Deleted: false,
        });
      });
      var $comments = ViewTable({
        selector: "#listview-item-comments",
        visibleFields: ["Description"],
        keyField: "ID",
        indexed: true,
        actions: [
          {
            template:
              "<i title='Edit existing comment' class='fas fa-edit fn-orange csr-pointer'></i>",
            on: {
              click: function (e) {
                var dlgcomment = __this.dialog(
                  "Edit Comment",
                  {
                    selector: "#item-comment-edit-content",
                  },
                  "info",
                  "ok-cancel"
                );

                dlgcomment.invoke(function () {
                  var $decription = dlgcomment.content.find(
                    ".comment-description"
                  );
                  $decription.val(e.data.Description);
                  dlgcomment.confirm(function () {
                    e.data.Description = $decription.val();
                    saveComment($comments, e.data);
                    dlgcomment.shutdown();
                  });
                  dlgcomment.reject(function () {
                    dlgcomment.shutdown();
                  });
                });
              },
            },
          },
          {
            template:
              "<i title='Delete existing comment' class='fas fa-trash-alt fn-red csr-pointer'></i>",
            on: {
              click: function (e) {
                var dlgcomment = __this.dialog(
                  "Delete Comment",
                  "Are sure you want to delete this comment?",
                  "warning",
                  "ok-cancel"
                );
                dlgcomment.confirm(function () {
                  $.post(
                    __urls.deleteItemComment,
                    { commentId: e.key },
                    function (resp) {
                      if (!resp.Message.IsRejected) {
                        $comments.removeRow(e.key);
                        $chosenComments.removeRow(e.key);
                      }
                    }
                  );
                  dlgcomment.shutdown();
                });
              },
            },
          },
          {
            template:
              "<i title='Choose comment for the item' class='fas fa-arrow-circle-right fn-green csr-pointer'></i>",
            on: {
              click: function (e) {
                chooseComment(e.data);
              },
            },
          },
        ],
        paging: {
          pageSize: 9,
        },
      });
      const $saveNewComment = dialog.content.find("#add-new-comment"),
        $searchComments = dialog.content.find("#search-item-comment"),
        $message = dialog.content.find(".error-message").addClass("fn-green");
      searchComments($comments, $searchComments);

      $(window).on("keypress", function (e) {
        if (e.which == 13 && $searchComments.is(":focus")) {
          saveComment();
        }
      });

      $saveNewComment.on("click", function () {
        let inputValue = $searchComments.val();
        inputValue = inputValue.replace(/\s/g, "").toLowerCase();

        let $searchbox = dialog.content.find("#search-item-comment");
        if ($searchbox.val() != "") {
          let $commentDialog = __this.dialog(
            "Create comment",
            "Do you want to create new comment from the text searched above?",
            "info",
            "ok-cancel"
          );
          $commentDialog.confirm(function () {
            var itemComment = {
              ID: 0,
              Description: $searchComments.val(),
            };
            saveComment($comments, $searchComments, itemComment);
            $commentDialog.shutdown();
          });
        }
      });

      function chooseComment(comment) {
        let _commented = findInComment(
          "Description",
          comment.Description,
          $chosenComments.yield()
        );
        if (!_commented) {
          let _comment = {};
          _comment["Index"] = $chosenComments.yield().length + 1;
          _comment = $.extend(_comment, comment);
          $chosenComments.updateRow(_comment);
        }
      }
      function findInComment(keyName, keyValue, values) {
        if (isValidArray(values)) {
          return $.grep(values, function (item, i) {
            return item[keyName].trim() == keyValue.trim();
          })[0];
        }
      }
      function removeComment(comment) {
        $chosenComments.removeRow(comment.ID);
      }

      function searchComments($listComments, $searchComments) {
        $.post(__urls.getItemComments, function (comments) {
          $.grep(comments, function (cmt) {
            for (let i = 0; i < itemComments.length; i++) {
              if (
                cmt.Description.toLowerCase() == itemComments[i].toLowerCase()
              ) {
                chooseComment(cmt);
              }
            }
          });

          $listComments.bindRows(comments);
          $searchComments.on("keyup", function () {
            let input = this.value.replace(/\s/g, "");
            let regex = new RegExp(input, "i");
            var filtereds = $.grep(comments, function (item, i) {
              return regex.test(item.Description.replace(/\s/g, ""));
            });

            $comments.clearRows();
            $comments.bindRows(filtereds);
          });
        });
      }

      function saveComment($commentListview, $searchComments, itemComment) {
        if (isValidJson(itemComment)) {
          $.post(
            __urls.saveItemComment,
            {
              comment: itemComment,
            },
            function (resp) {
              if (resp.Message.IsApproved) {
                orderDetail.Comment = "";
                $comments.updateRow(resp.Comment);
                chooseComment(resp.Comment);
                $searchComments.val("");
              }
              ViewMessage({
                summary: {
                  bordered: false,
                },
              })
                .bind(resp.Message)
                .appendTo($message);
              searchComments($commentListview, $searchComments);
            }
          );
        }
      }

      //Save comment description to item.
      dialog.confirm(function () {   
        $.each($chosenComments.yield(), function (i, comment) {
          if (i <= 0) {
            orderDetail.Comment = comment.Description.trim();
          } else {
            orderDetail.Comment += "+" + comment.Description.trim();
          }
          var commentCol = $listOrderedItems.getColumn(orderDetail.LineID, "KhmerName");
          let comments = orderDetail.Comment.split("+");
          for (let c = 0; c < comments.length; c++) {
            $(commentCol).append("<div>" + comments[c] + "</div>");
          }
        
        });       
        dialog.shutdown();
      });
      dialog.reject(function () {
        dialog.shutdown();
      });
    });
  };

  this.mergeItem = function (json) {
    var details = __orderInfo.Order[__config.detail.fieldName];
    $.grep(details, function (item) {
      if (item.LineID == json.LineID) {
        item = $.extend(true, item, json);
      }
    });
  };

  this.copyLineItem = function (rowData) {
    let newRow = {};
    newRow = $.extend(newRow, rowData);
    let lineId = Date.now().toString();
    if (isEmpty(newRow.CopyID)) {
      newRow.CopyID = rowData.LineID;
    }
    let saleItem = __this.findInArray(
      "ID",
      newRow.CopyID,
      __orderInfo.SaleItems
    );
    if (isValidJson(saleItem)) {
      newRow.CopyID = saleItem.ID;
    }
    newRow.LineID = lineId;
    newRow.OrderDetailID = 0;
    newRow.Qty = 1;
    newRow.PrintQty = 1;
    newRow.DiscountRate = 0;
    newRow.DiscountValue = 0;
    newRow.UnitPrice = __this.getUnitPrice(
      saleItem.UnitPrice,
      newRow.DiscountRate
    );
    newRow.Total = __this.sumLineItem(newRow);
    $listOrderedItems.addRow(newRow);
    __orderInfo.Order[__config.detail.fieldName] = $listOrderedItems.yield();
    __this.summarizeOrder(__orderInfo.Order);
    return newRow;
  };

  function checkOrderDetails(orderDetails) {
    let isValid = false;
    if (isValidArray(orderDetails)) {
      $.grep(orderDetails, function (item, i) {
        let newProps = Object.getOwnPropertyNames(item),
          prevProps = Object.getOwnPropertyNames(
            __orderInfo.Order[__config.detail.fieldName][0]
          );
        if (newProps.length == prevProps.length) {
          for (let j = 0; j < newProps.length; j++) {
            if (newProps[i] === prevProps[i]) {
              isValid = true;
            }
          }
        }
      });
    }
    return isValid;
  }

  function checkOrder(order) {
    let isValid = false;
    if (isValidJson(order)) {
      let newProps = Object.getOwnPropertyNames(order),
        prevProps = Object.getOwnPropertyNames(__orderInfo.Order);
      if (newProps.length == prevProps.length) {
        for (let i = 0; i < newProps.length; i++) {
          if (newProps[i] === prevProps[i]) {
            isValid = true;
          }
        }
      }
    }
    return isValid;
  }

  this.updateOrder = function (order) {
    if (isValidJson(order)) {
      __orderInfo.Order = $.extend(true, {}, order);
    }

    __this.bindLineItems(
      __orderInfo.Order[__config.detail.fieldName],
      $listOrderedItems
    );
    __this.summarizeOrder(__orderInfo.Order);
  };

  this.deleteLineItem = function (item, isFromDelete = false) {
    //Previlege for deleting item
    let dialogDelete = __this.dialog(
      "Delete item",
      "Are you sure you want to remove " +
        item.KhmerName +
        "(" +
        item.Code +
        ")?",
      "warning",
      "ok-cancel"
    );

    dialogDelete.confirm(function () {
      if (item.OrderDetailID <= 0) {
        deleteRelatedLineItems(item);
      } else {
        switch (__orderInfo.OrderTable.Status) {
          case "B":
            __this.checkPrivilege("P024", function () {
              deleteRelatedLineItems(item, isFromDelete);
            });

            break;
          case "P":
            __this.checkPrivilege("P025", function () {
              deleteRelatedLineItems(item, isFromDelete);
            });
            break;
        }
      }

      dialogDelete.shutdown();
    });

    dialogDelete.reject(function () {
      dialogDelete.shutdown();
    });
  };

  function deleteRelatedLineItems(item, isFromDelete = false) {
    //Check on bill status
    item.PrintQty = parseFloat(item.Qty) * -1;
    item.Qty = 0;
    updateLineItem(item.LineID, item.PrintQty);
    $.grep($listOrderedItems.yield(), function (_item) {
      if (item.LineID == _item.ParentLineID) {
        _item.PrintQty = item.PrintQty;
        _item.Qty = 0;
        updateLineItem(_item.LineID, _item.PrintQty);
      }
    });
  }

  this.on = function (eventType, callback) {
    if (typeof callback === "function") {
      this.off(eventType, callback);
      switch (eventType) {
        case "load":
          __config.events["load"].push(callback);
          break;
      }
    }
  };

  this.off = function (eventType, callback) {
    var events = __config.events[eventType];
    __config.events[eventType] = $.grep(events, function (cb, i) {
      return cb !== callback;
    });
  };

  this.load = function (callback) {
    this.on("load", callback);
  };

  this.fetchOrderInfo = async function (
    tableId,
    orderId,
    customerId,
    setDefaultOrder,
    onSucceed
  ) {
    __this.loadScreen();
    let orderInfo = {};
    __orderInfo.OrderDetail = {};
    __loadComplete = false;
    __isScanQrcode = false;
    $listOrderedItems.clearRows();
    orderInfo = await $.post(__urls.fetchOrderInfo, {
      tableId: tableId,
      orderId: orderId,
      customerId: customerId,
      setDefaultOrder: setDefaultOrder,
    });

    __orderInfo = $.extend(__orderInfo, orderInfo);

    if (typeof onSucceed === "function") {
      onSucceed.call(__this, __orderInfo);
    }

    for (let i = 0; i < __config.events["load"].length; i++) {
      if (typeof __config.events["load"][i] === "function") {
        __config.events["load"][i].call(__this, __orderInfo);
      }
    }

    __this.setOrder(__orderInfo.Order, false, !__config.orderOnly);
    setDynamicProperties();
    displaySettings();
    __loadComplete = true;
    __this.loadScreen(false);

    if (__orderInfo.Setting.DaulScreen) {
      __this.start2ndScreen(
        __orderInfo.Order.TableID,
        __orderInfo.Order.OrderID,
        0
      );
    }
    return __orderInfo;
  };

  function setDynamicProperties() {
    const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(
      (i) => i.AltCurrencyID == i.BaseCurrencyID
    )[0] ?? { DecimalPlaces: 0 };
    $listOrderedItems.updateFieldFormat(
      ["Total", "Qty", "UnitPrice"],
      "number",
      { fixed: baseCurrency.DecimalPlaces }
    );
    let item = __orderInfo["SaleItems"][0];
    if (item) {
      const props = item["AddictionProps"];
      $listOrderedItems.clearHeaderDynamic(props);
      $listOrderedItems.createHeaderDynamic(props);
      $itemListView.clearHeaderDynamic(props);
      $itemListView.createHeaderDynamic(props);
    }
  }

  function displaySettings() {
    $("#panel-view-mode").removeClass("hidden");
    if (__orderInfo.Setting.EnableCountMember) {
      $("#customer-count").removeClass("hidden");
    } else {
      $("#customer-count").addClass("hidden");
    }

    if (__orderInfo.Setting.PanelViewMode == 0) {
      $("#group-buttons").removeClass("fixed-width");
    } else {
      $("#left-search-box").show().find(".search-box input").focus();
      $("#right-toolbar").hide();
    }

    if (__orderInfo.Setting.EnablePromoCode) {
      $("#promocode").removeClass("hidden");
    } else {
      $("#promocode").addClass("hidden");
    }
  }

  function switchPanelViewMode(mode) {
    switch (mode) {
      case 0:
        $("#panel-view-mode")
          .removeClass("fa-cart-arrow-down")
          .addClass("fa-th-large")
          .attr("title", "Switch to split list");
        $("#panel-group-items .right-panel").addClass("width-zero");
        $("#left-search-box").show(200).find(".search-box input").focus();
        $("#right-toolbar").hide(200);

        $("#group-buttons").addClass("fixed-width");
        $("#panel-view-mode").data("view-mode", 1);
        break;
      case 1:
        $("#panel-view-mode")
          .addClass("fa-cart-arrow-down")
          .removeClass("fa-th-large")
          .attr("title", "Switch to full list");
        $("#panel-group-items .right-panel").removeClass("width-zero");
        $("#right-toolbar").show(200);
        $("#left-search-box").hide(200);
        $("#group-buttons").removeClass("fixed-width");
        $("#panel-view-mode").data("view-mode", 0);
        break;
    }
    __orderInfo.Setting.PanelViewMode = $("#panel-view-mode").data("view-mode");
    __this.saveUserSetting();
  }

  this.loadUserSetting = function (success, fail) {
    var postReq = undefined;
    if (typeof success === "function") {
      postReq = $.post(__urls.getUserSetting, success).fail;
    }

    if (typeof fail === "function") {
      if (postReq !== undefined) {
        postReq.fail(fail);
      }
    }
  };

  this.validateSettings = function () {
    if (
      isEmpty(__orderInfo.Setting.WarehouseID, true) ||
      __orderInfo.Setting.WarehouseID <= 0
    ) {
      __this.dialog("Warehouse Undefined", "Warehouse is required.", "warning");
      return false;
    }

    if (
      isEmpty(__orderInfo.Setting.SeriesID, true) ||
      __orderInfo.Setting.SeriesID <= 0
    ) {
      __this.dialog("Series Undefined", "Series is required.", "warning");
      return false;
    }
    return true;
  };

  this.saveUserSetting = function (userSetting, onsuccess, onfail) {
    __orderInfo.Setting = $.extend(true, __orderInfo.Setting, userSetting);
    if (!__this.validateSettings()) {
      return;
    }
    $.post(
      __urls.submitUserSetting,
      {
        setting: __orderInfo.Setting,
        redirectUrl: "",
        returnJson: true,
      },
      onsuccess
    ).fail(onfail);
  };

  this.resetOrder = function (
    tableId,
    customerId,
    setDefaultOrder = false,
    isFromChangeRate = false,
    onSuccess
  ) {
    if (isFromChangeRate) {
      __this.gotoPanelItemOrder();
    }
    return __this.fetchOrderInfo(
      tableId,
      0,
      customerId,
      setDefaultOrder,
      onSuccess
    );
  };

  this.setOrder = function (
    order,
    invoke2ndScreen = true,
    displayLineItems = true
  ) {
    __this.clearOrder2ndScreen();
    let pageSetting = {};
    pageSetting.pageSize = __orderInfo.Setting.ItemPageSize;
    $gridItemPaging.configure(pageSetting);

    if (displayLineItems) {
      Promise.all([
        bindOrderNumbers(__orderInfo.Orders, __orderInfo.Order.OrderID),
        __this.bindLineItems(
          order[__config.detail.fieldName],
          $listOrderedItems
        ),
        __this.summarizeOrder(order, invoke2ndScreen),
      ]);
    }

    Promise.all([
      __this.displaySaleItems(order),
      __this.updateOrderCustomer(__orderInfo.Order.Customer),
    ]);
  };

  this.displaySaleItems = function (order = {}) {
    switch (__orderInfo.Setting.ItemViewType) {
      case 0:
        $("#group-item-gridview").show();
        $("#group-item-listview").hide();
        showInGridview(order);
        break;
      case 1:
        $("#group-item-gridview").hide();
        $("#group-item-listview").show();
        showInListview(__orderInfo.SaleItems);
        break;
    }
  };

  this.updateOrderCustomer = function (customer, changePricelist = false) {
    if (isValidJson(customer)) {
      __orderInfo.Order.CustomerID = customer.ID;
      var _customer = $.extend(__orderInfo.Order.Customer, customer);
      $(".customer-name").text(_customer.Name);
      $(".customer-phone").text(
        isEmpty(_customer.Phone) ? "N/A" : _customer.Phone
      );

      if (changePricelist) {
        __this.fetchOrderInfo(__orderInfo.Order.TableID, 0, customer.ID, false);
      }
    }
  };

  this.findOrder = function (keyName, value) {
    return findInArray(keyName, value, __orderInfo.Orders);
  };

  this.getOrders = function () {
    return __orderInfo.Orders;
  };
  //Changed on 16-08-2021
  this.bindLineItems = function (items, table) {
    var orderDetails = [];
    if (table !== undefined) {
      table.clearRows();
    }

    for (let i = 0; i < items.length; i++) {
      items[i].ItemUoMs = __this.filterUoMs(
        items[i].GroupUomID,
        items[i].UomID,
        true
      );
      items[i].TaxGroups = selectTaxGroups(items[i].TaxGroupID, false);
      items[i].RemarkDiscounts = selectRemarkDiscount(
        items[i].RemarkDiscountID,
        false
      );
      items[i].TaxOption = __orderInfo.Setting.TaxOption;
      if (isEmpty(items[i].ParentLineID)) {
        orderDetails.push(items[i]);
      }

      var _addons = $.grep(items, function (item) {
        return item.ParentLineID == items[i].LineID;
      });

      for (let j = 0; j < _addons.length; j++) {
        if (items[i].LineID == _addons[j].ParentLineID) {
          orderDetails.push(_addons[j]);
        }
      }
      __this.sumLineItem(items[i]);
    }

    orderDetails = $.grep(orderDetails, function (_item) {
      if (_item.ItemType.toLowerCase() === "service") {
        return _item.PrintQty >= 0;
      } else {
        return !(_item.Qty === 0 && _item.PrintQty === 0);
      }
    });
    __orderInfo.Order[__config.detail.fieldName] = orderDetails;
    displayLineItems(orderDetails, table);
  };
  function displayLineItems(orderDetails, table) {
    if (table !== undefined) {
      for (let i = 0; i < orderDetails.length; i++) {
        table.addRow(orderDetails[i], function (info) {
            if(orderDetails[i].Comment !=undefined){
                var commentCol = $(info.row).find("[data-field='KhmerName']");
                let comments = orderDetails[i].Comment.split("+");
                if (isValidArray(comments)) {
                  for (let c = 0; c < comments.length; c++) {
                    $(commentCol).append("<div>" + comments[c] + "</div>");
                  }
                }
            }
          if (orderDetails[i].IsReadonly) {
            $(info.row).addClass("readonly");
          }
          if (!isEmpty(info.data.ParentLineID)) {
            $(info.row)
              .find("[data-field='Code']")
              .prepend("<span>" + info.data.Prefix + "</span>&nbsp;")
              .css("text-align", "left")
              .css("padding-left", "15px");
          }
        });
        if (__isScanQrcode) {
          $(table.getRow(orderDetails[i]))
            .find(".scale-box .scale-down")
            .addClass("readonly");
        } else {
          $(table.getRow(orderDetails[i]))
            .find(".scale-box .scale-down")
            .removeClass("readonly");
        }
      }
      if (isValidArray(orderDetails)) {
        const prop = orderDetails[0]["AddictionProps"];
        if (prop != null && prop != "") {
          table.clearHeaderDynamic(prop);
          table.createHeaderDynamic(prop);
          $listOrderedItems.clearHeaderDynamic(prop);
          $listOrderedItems.createHeaderDynamic(prop);
        }
      }
    }
  }

  $("[name='sort-field']").change(function () {
    __orderInfo.Setting.SortBy.Field = this.value;
    __orderInfo.Setting.SortBy.Desc = $("#sort-order-type").prop("checked");
    //sortBy(__orderInfo.SaleItems, __orderInfo.Setting.SortBy.Field, __orderInfo.Setting.SortBy.Desc);
    var items = $gridItemPaging.yield();
    if (items.some((i) => i.ItemID > 0)) {
      sortBy(
        items,
        __orderInfo.Setting.SortBy.Field,
        __orderInfo.Setting.SortBy.Desc
      );
      showItemsByViewType(items);
    }
  });

  $("#sort-order-type").change(function () {
    __orderInfo.Setting.SortBy.Desc = $("#sort-order-type").prop("checked");
    //sortBy(__orderInfo.SaleItems, __orderInfo.Setting.SortBy.Field, __orderInfo.Setting.SortBy.Desc);
    var items = $gridItemPaging.yield();
    if (items.some((i) => i.ItemID > 0)) {
      sortBy(
        items,
        __orderInfo.Setting.SortBy.Field,
        __orderInfo.Setting.SortBy.Desc
      );
      showItemsByViewType(items);
    }
  });

  function sortBy(items, prop, desc = false) {
    if (isValidArray(items)) {
      var retVal = desc ? -1 : 1;
      items.sort(function (a, b) {
        if (typeof a[prop] === "string" && typeof b[prop] === "string") {
          var nameA = a[prop].toUpperCase();
          var nameB = b[prop].toUpperCase();
          if (nameA < nameB) {
            return retVal * -1;
          }
          if (nameA > nameB) {
            return retVal;
          }
          return 0;
        }

        if (typeof a[prop] === "number" && typeof b[prop] === "number") {
          if (desc) {
            return b[prop] - a[prop];
          }
          return a[prop] - b[prop];
        }
      });
    }
  }

  function showItemsByViewType(items) {
    switch (__orderInfo.Setting.ItemViewType) {
      case 0:
        $("#group-item-gridview").show();
        $("#group-item-listview").hide();
        $gridItemPaging.load(items, items[0], function (info) {
          $(__wrapGridItem).children().remove();
          bindSingleItemGrids(info.dataset);
        });
        break;
      case 1:
        $("#group-item-gridview").hide();
        $("#group-item-listview").show();
        showInListview(items);
        break;
    }
  }

  function showInListview(items) {
    $gridItemPaging.load(items, items[0], function (info) {
      $itemListView.bindRows(info.dataset);
    });
  }

  function showInGridview(order) {
    if (order.OrderID > 0 && !__config.orderOnly) {
      let _items = $.grep(__orderInfo.SaleItems, function (item) {
        return isValidJson($listOrderedItems.find(item.ID));
      });
      $gridItemPaging.load(_items, _items[0], function (info) {
        $(__wrapGridItem).children().remove();
        bindSingleItemGrids(info.dataset);
      });
      $(".all-groups", __groupItemStep).removeClass("active");
    } else {
      resetGroupItems();
    }
  }

  var undefinedrefno;
  //Changed from bindItemOrder to bindOrderNumbers
  function bindOrderNumbers(orders, activeOrderId) {
    let $listBox = $(__listOrderReceipt);
    $listBox.find("#dropbox-order").children().remove();
    $listBox.find("#badge-order").text("");
    if (isValidArray(orders)) {
      for (let i = 0; i < orders.length; i++) {
        __this.addNewOrder(orders[i], activeOrderId);
      }
    }

    $listBox
      .find("#add-new-order")
      .off("click")
      .on("click", function (e) {
        $.get(
          "/pos/GetTableType",
          { tableId: __orderInfo.Order.TableID },
          function (i) {
            if (i.Type == 2 || i.Type == 3) {
              let dialog = new DialogBox({
                caption: "RefNo",
                icon: "fas fa-user-friends",
                content: {
                  selector: "#dialog-delvery-takaway",
                },
                button: {
                  ok: {
                    text: "Save",
                  },
                },
              });
              dialog.invoke(function () {
                dialog.content.find("#delvery-takaway");
              });
              dialog.confirm(function () {
                if (!$("#delvery-takaway").val()) {
                  $("#text_refno").show();
                  setTimeout(function () {
                    $("#text_refno").text("");
                  }, 3000);
                  return;
                }
                text_refno = $("#delvery-takaway").val();
                undefinedrefno = "undefined";

                __this.loadCurrentOrderInfo(
                  __orderInfo.Order.TableID,
                  0,
                  0,
                  true
                );
                dialog.shutdown();
              });
              return;
            }
            __this.loadCurrentOrderInfo(__orderInfo.Order.TableID, 0, 0, true);
          }
        );

        $listBox.find("#dropbox-order .option").removeClass("active");
      });
  }
  var notundefined;
  this.addNewOrder = function (order, activeOrderId) {
    if (isValidJson(order) && order.OrderID > 0) {
      let $listBox = $(__listOrderReceipt);
      let dateIn = moment(order.DateIn.split("T")[0]).format("DD.MM.YYYY");
      let $option = $(
        "<div data-id='" +
          order.OrderID +
          "' class='option csr-pointer'><i class='fas fa-receipt'></i> " +
          order.OrderNo +
          " (" +
          dateIn +
          " " +
          order.TimeIn +
          ")" +
          "</div > "
      );
      if (order.OrderID === activeOrderId) {
        $option.addClass("active");
      }

      if (order.CheckBill === "Y") {
        $option.addClass("bg-light-green");
      }

      if (order.CheckBill === "N") {
        $option.addClass("bg-red");
      }

      $option.on("click", function () {
        if (__isScanQrcode) {
          __this.fetchOrderInfoQR(__encryptedTableID, order.OrderID);
        } else {
          let orderId = $option.data("id");
          if (__orderInfo.Order.OrderID != orderId) {
            __this.loadCurrentOrderInfo(order.TableID, order.OrderID);
          }
        }
        $(this).addClass("active").siblings().removeClass("active");
      });

      $listBox.find("#dropbox-order").prepend($option);
      $listBox
        .find("#badge-order")
        .html("<span class='badge'>" + __orderInfo.Orders.length + "</span>");
    }
  };

  function onFirstClickItemGrid(itemKey, scaleValue, saleItem = {}) {
    
    if (
      isValidJson(__orderInfo.OrderDetail) &&
      !isEmpty(__orderInfo.OrderDetail.LineID) &&
      __orderInfo.OrderDetail.ItemType.toLowerCase() !== "addon"
    ) {
      if (__isScanQrcode) {
        addonLineItemQR(itemKey, __orderInfo.OrderDetail.LineID);
      } else {
        addonLineItem(itemKey, __orderInfo.OrderDetail.LineID);
      }

    } else {
      let orderDetail = $listOrderedItems.find(itemKey);
      if (isValidJson(orderDetail)) {
        
        let salePrice = __this.getUnitPrice(
          saleItem.UnitPrice,
          saleItem.DiscountRate
        );
        let itemPrice = __this.getUnitPrice(
          orderDetail.UnitPrice,
          orderDetail.DiscountRate
        );
        if (salePrice > itemPrice && saleItem.PromotionID <= 0) {
          __this.copyLineItem(orderDetail);
          return;
        }

        if (orderDetail.ItemType.toLowerCase() !== "service") {
          orderDetail.PrintQty += scaleValue;
          orderDetail.Qty += scaleValue;
        }
        if (__isScanQrcode) {
          updateLineItemQR(itemKey, orderDetail.PrintQty);
        } else {
          updateLineItem(itemKey, orderDetail.PrintQty);
        }
      } else {
        if (__isScanQrcode) {
          updateLineItemQR(itemKey, scaleValue);
        } else {
          updateLineItem(itemKey, scaleValue);
        }
      }
    }
  }

  function addonLineItem(itemKey, relatedKey) {
    var addon = __this.createLineItem(itemKey);
    let lineId = Date.now().toString();
    addon.LineID = lineId;
    addon.Qty = 1;
    addon.PrintQty = 1;
    addon.ParentLineID = relatedKey.toString();
    addon.Total = __this.sumLineItem(addon);
    __this.addSubLineItem(
      addon,
      "<i class='fas fa-plus-circle fn-green fa-xs'></i>"
    );
  }
  this.addLineItems = function (items) {
    if (isValidArray(items)) {
      for (let i = 0; i < items.length; i++) {
        if (isValidJson(items[i])) {
          $listOrderedItems.updateRow(items[i]);
        }
      }
    }
    __orderInfo.Order[__config.detail.fieldName] = $listOrderedItems.yield();
    __this.summarizeOrder(__orderInfo.Order);
  };
  this.addSubLineItem = function (
    item,
    prefixSymbol,
    isReadonly = false,
    disabledQty = false
  ) {
    var details = [];
    if (isValidJson(item) && Array.isArray(details)) {
      $listOrderedItems.addRowAfter(item, item.ParentLineID, function (info) {
        if (info.data.ParentLineID == item.ParentLineID) {
          info.data.Prefix = prefixSymbol;
          var $codeColumn = $(info.row)
            .find("[data-field='Code']")
            .css("text-align", "left")
            .css("padding-left", "15px");
          if (!isEmpty(prefixSymbol)) {
            $codeColumn.prepend("<span>" + prefixSymbol + "</span>&nbsp;");
          }
          if (isReadonly) {
            $(info.row).addClass("readonly");
          }
          if (disabledQty) {
            $(info.row).find("[data-field='Qty']").addClass("readonly");
          }
        }
      });
      __orderInfo.Order[__config.detail.fieldName] = $listOrderedItems.yield();
      __this.summarizeOrder(__orderInfo.Order);
    }
  };

  this.changeItemQty = function (itemId, scaleValue) {
    updateLineItem(itemId, scaleValue, false);
  };

  //Update item order details
  function updateLineItem(
    itemKey,
    scaleValue,
    isFromGrid = true,
    saleItem = {}
  ) {
    let lineItem = $listOrderedItems.find(itemKey);
    if (isValidJson(lineItem)) {
      if (scaleValue < 0) {
        __this.removeSrialFromItem(lineItem);
      }

      if (isFromGrid) {
        lineItem.Total = __this.sumLineItem(lineItem);
        $listOrderedItems.updateRow(lineItem);
      } else {
        
        lineItem.PrintQty += scaleValue;
        $listOrderedItems.updateColumn(
          itemKey,
          "Total",
          __this.sumLineItem(lineItem)
        );
        $gridItemPaging.load(
          findMultiUomItems(lineItem.ItemID),
          itemKey,
          function (info) {
            $(__wrapGridItem).children().remove();
            $(__groupItemStep).children().removeClass("active");
            bindSingleItemGrids(info.dataset);
          }
        );
      }

      if (lineItem.PrintQty >= 0) {
        __orderInfo.Order[__config.detail.fieldName] =
          $listOrderedItems.yield();
      }
    } else {
      lineItem = __this.createLineItem(itemKey);
      $listOrderedItems.updateRow(lineItem);
      __orderInfo.Order[__config.detail.fieldName] = $listOrderedItems.yield();
    }
    $listOrderedItems.updateColumn(
      lineItem.LineID,
      "PrintQty",
      lineItem.PrintQty
    );
    updateSubLineItem(lineItem, scaleValue);
    updateItemGridView(lineItem);
    const e = {
      key: itemKey,
      data: lineItem,
    };
    __this.checkBuyXQtyGetXDiscount(e);
  }

  function updateSubLineItem(lineItem, scaleValue) {
    if (isValidJson(lineItem)) {
      // Buy X Get X
      if (lineItem.PromoType == 1) {
        var sublineItems = $listOrderedItems.yield();
        $.grep(sublineItems, function (item, i) {
          if (lineItem.LineID == item.ParentLineID) {
            var baseQtyFactor = lineItem.Qty / lineItem.BaseQty;
            if (lineItem.Qty % lineItem.BaseQty === 0) {
              item.Qty = item.BaseQty * baseQtyFactor;
              item.PrintQty += item.BaseQty * scaleValue;
            }

            $listOrderedItems.updateColumn(item.LineID, "Qty", item.Qty);
            $listOrderedItems.updateColumn(
              item.LineID,
              "PrintQty",
              item.PrintQty
            );
          }
        });
        __orderInfo.Order[__config.detail.fieldName] =
          $listOrderedItems.yield();
      }
    }
  }

  this.findSaleItemByItemID = function (itemId) {
    return findInArray("ItemID", itemId, __orderInfo.SaleItems);
  };

  function checkTypeofService(orderDetail) {
    if (orderDetail.ItemType.toLowerCase() === "service") {
      if (
        isValidJson(__orderInfo.OrderTable) &&
        __orderInfo.OrderTable.ID > 0
      ) {
        let times = __orderInfo.OrderTable.Time.split(":");
        let h = parseInt(times[0]),
          m = parseFloat(times[1]);
        orderDetail.PrintQty = 1;
        orderDetail.Qty = parseFloat((h + m / 60 + 0.001).toFixed(3));
      }
    }
    return orderDetail;
  }

  this.createLineItem = function (saleItemId, uomId, taxGroupId, onCreated) {
    var item = $.grep(__orderInfo.SaleItems, function (item, i) {
      return item.ID == saleItemId;
    })[0];

    if (isValidJson(item) && uomId > 0) {
      item = $.grep(findMultiUomItems(item.ItemID), function (_item, i) {
        return _item.UomID == uomId;
      })[0];
    }
    if (isValidJson(item)) {
      var lineItem = {
        LineID: item.ID.toString(),
        CopyID: item.ID.toString(),
        OrderID: __orderInfo.Order.OrderID,
        OrderDetailID: 0,
        ItemID: item.ItemID,
        Code: item.Code,
        KhmerName: item.KhmerName,
        EnglishName: item.EnglishName,
        BaseQty: 1,
        Qty: 1,
        PrintQty: 1,
        ItemUoMs: __this.filterUoMs(item.GroupUomID, item.UomID),
        GroupUomID: item.GroupUomID,
        Uom: item.UoM,
        UomID: item.UomID,
        Cost: item.Cost,
        UnitPrice: item.UnitPrice,
        DiscountRate: item.DiscountRate,
        DiscountValue: item.DiscountValue,
        RemarkDiscounts: selectRemarkDiscount(0),
        TaxGroups: selectTaxGroups(
          taxGroupId > 0 ? taxGroupId : item.TaxGroupSaleID
        ),
        TaxGroupID: taxGroupId > 0 ? taxGroupId : item.TaxGroupSaleID,
        TaxRate: item.TaxRate ? item.TaxRate : 0,
        TaxValue: item.TaxValue ? item.TaxValue : 0,
        Total: item.UnitPrice,
        ItemStatus: "new",
        ItemPrintTo: item.PrintTo,
        Currency: item.Currency,
        ItemType: item.ItemType,
        TypeDis: item.TypeDis,
        Description: item.Description,
        PromoType: 0,
        PromoTypeDisplay: "None",
        ParentLineID: "",
        AddictionProps: item.AddictionProps,
      };
      lineItem.DiscountRate=item.DiscountRate;
      if(lineItem.TypeDis.toLowerCase()=="cash"){
        lineItem.DiscountRate=item.DiscountRate*100/__this.toNumber(lineItem.UnitPrice);
      }
      checkTypeofService(lineItem);
      if (typeof onCreated === "function") {
        onCreated.call(__this, lineItem);
      }
      __this.sumLineItem(lineItem);
      return lineItem;
    }
  };

  this.filterUoMs = function (groupUoMId, uomId, disabled = false) {
    var selectList = [];
    if (isValidArray(__orderInfo.ItemUoMs)) {
      $.grep(__orderInfo.ItemUoMs, function (uom, i) {
        if (uom.GroupUomID == groupUoMId) {
          selectList.push({
            Value: uom.UomID,
            Text: uom.Name,
            Selected: uom.UomID == uomId,
            Disabled: disabled,
          });
        }
      });
      return selectList;
    }
  };

  function selectTaxGroups(taxGroupId, disabled = false) {
    var taxes = __orderInfo.TaxGroups;
    var selectList = [];
    selectList.push({
      Value: 0,
      Text: "---",
    });
    for (let i = 0; i < taxes.length; i++) {
      selectList.push({
        Value: taxes[i].ID,
        Text: taxes[i].Name,
        Selected: taxes[i].ID == taxGroupId,
        Disabled: disabled,
      });
    }
    return selectList;
  }
  function selectRemarkDiscount(remarkDisId, disabled = false) {
    let remarks = __orderInfo.RemarkDiscountItem;
    let selectList = [];
    for (let i = 0; i < remarks.length; i++) {
      selectList.push({
        Value: remarks[i].Value,
        Text: remarks[i].Text,
        Selected: remarks[i].Value == remarkDisId,
        Disabled: disabled,
      });
    }
    return selectList;
  }

  function setDisabled(target, disabled = true) {
    if (disabled) {
      $(target).css("pointer-events", "none");
    } else {
      $(target).css("pointer-events", "auto");
    }
  }

  function updateItemGridView(orderDetail, disabled = false) {
    if (!isValidJson(orderDetail) || isEmpty(orderDetail.LineID)) {
      return;
    }
    if (__isScanQrcode) {
      $($listOrderedItems.getRow(orderDetail))
        .find(".scale-box .scale-down")
        .addClass("disabled");
    } else {
      $($listOrderedItems.getRow(orderDetail))
        .find(".scale-box .scale-down")
        .removeClass("disabled");
    }

    let $grid = $(__wrapGridItem).find(
      ".grid[data-id='" + orderDetail.LineID + "']"
    );
    let $scaleBox = $grid.find(".scale-box");
    orderDetail.PrintQty = parseFloat(orderDetail.PrintQty.toFixed(3));
    if (orderDetail.Qty <= 0) {
      $listOrderedItems.removeRow(orderDetail.LineID, true);
      if (orderDetail.OrderDetailID <= 0) {
        $listOrderedItems.removeRow(orderDetail.LineID);
        $.grep($listOrderedItems.yield(), function (item) {
          if (item.ParentLineID == orderDetail.LineID) {
            $listOrderedItems.removeRow(item.LineID);
          }
        });
        __orderInfo.Order[__config.detail.fieldName] =
          $listOrderedItems.yield();
      }
      $scaleBox.removeClass("active");
    }

    if (orderDetail.PrintQty <= 0) {
      $scaleBox.removeClass("active");
      $grid.removeClass("active");
    } else {
      let $scaleLabel = $scaleBox
        .find(".scale-label")
        .text(orderDetail.PrintQty);
      $scaleBox.find(".scale-label").removeClass("small-font2x small-font3x");
      if ($scaleLabel.text().length > 4) {
        $scaleLabel.addClass("small-font2x");
        if ($scaleLabel.text().length > 5) {
          $scaleLabel.removeClass("small-font2x").addClass("small-font3x");
        }
      }
      $scaleBox.addClass("active");
      $grid.addClass("active").siblings().removeClass("active");
    }

    if (orderDetail.ItemType.toLowerCase() === "service") {
      $listOrderedItems.disableColumns(orderDetail.LineID, ["Qty"], true);
    }
    __orderInfo.Order[__config.detail.fieldName] = $listOrderedItems.yield();
    __this.summarizeOrder(__orderInfo.Order);
    setDisabled($grid, disabled);
    __this.showNotFound(false);
  }

  this.getUnitPrice = function (unitPrice, discountRate = 0) {
    if (typeof unitPrice === "string") {
      unitPrice = __this.toNumber(unitPrice);
    }

    let price = __this.toCurrency(unitPrice * (1 - discountRate / 100));
    return __this.toNumber(price);
  };

  this.sumLineItem = function (lineItem = {}) {
    if (isValidJson(lineItem) && !isEmpty(lineItem.LineID)) {
      lineItem.DiscountRate = __this.toNumber(lineItem.DiscountRate);
      lineItem.DiscountValue = __this.toNumber(lineItem.DiscountValue);
      switch (lineItem.TypeDis.toLowerCase()) {
        case "percent":
          lineItem.TotalNoTax =
          lineItem.Qty * __this.toNumber(lineItem.UnitPrice) *(1 - lineItem.DiscountRate / 100);
          lineItem.Total = lineItem.TotalNoTax;
          lineItem.DiscountValue =(lineItem.Qty *__this.toNumber(lineItem.UnitPrice) *lineItem.DiscountRate) /100;
          break;
        case "cash":
          lineItem.TotalNoTax =
          lineItem.Qty * __this.toNumber(lineItem.UnitPrice) *(1 - lineItem.DiscountRate / 100);
          lineItem.Total = lineItem.TotalNoTax;
          lineItem.DiscountValue =(lineItem.Qty *__this.toNumber(lineItem.UnitPrice) *lineItem.DiscountRate) /100;
          break;
        default:
         alert()
          lineItem.DiscountRate =
            lineItem.Qty * __this.toNumber(lineItem.UnitPrice) == 0
              ? 0
              : (100 * lineItem.DiscountValue) / lineItem.Total;
          lineItem.TotalNoTax =
            lineItem.Qty * __this.toNumber(lineItem.UnitPrice) -
            lineItem.DiscountValue;
          lineItem.Total = lineItem.TotalNoTax;
          break;
      }

      applyTax(lineItem);
      lineItem.Total = __this.toNumber(__this.toCurrency(lineItem.Total));
      return lineItem.Total;
    }
  };

  function applyTax(lineItem) {
    if (isValidJson(lineItem)) {
      let taxRate = 0;
      if (lineItem.TaxGroupID > 0) {
        taxRate = getTaxRate(lineItem.TaxGroupID);
      }
      const disCountTotal =
        lineItem.TotalNoTax *
        (1 -
          (__this.toNumber(__orderInfo.Order.DiscountRate) +
            __this.toNumber(__orderInfo.Order.PromoCodeDiscRate) +
            __this.toNumber(__orderInfo.Order.BuyXAmGetXDisRate) +
            __this.toNumber(__orderInfo.Order.CardMemberDiscountRate)) /
            100);
      lineItem.TaxRate = taxRate;
      lineItem.TaxValue = (disCountTotal * taxRate) / (100 + taxRate);
      $listOrderedItems.updateColumn(
        lineItem.LineID,
        "TaxValue",
        lineItem.TaxValue
      );

      //TaxOption.None == 0 || TaxOption.InvoiceVAT == 3
      if (
        __orderInfo.Setting.TaxOption == 0 ||
        __orderInfo.Setting.TaxOption == 3
      ) {
        lineItem.TaxGroupID = 0;
        lineItem.TaxValue = 0;
      }
      //TaxOption.VAT
      if (__orderInfo.Setting.TaxOption == 1) {
        const taxDisplay = (__this.toNumber(lineItem.Total) * taxRate) / 100;
        lineItem.TaxValue = (disCountTotal * taxRate) / 100;
        lineItem.Total = __this.toNumber(lineItem.Total) + taxDisplay;
        lineItem.TotalNet = lineItem.TotalNoTax;
      }
      lineItem.Total = __this.toNumber(lineItem.Total);
      lineItem.Total_Sys =
        __this.toNumber(lineItem.Total) * parseFloat(__orderInfo.Order.PLRate);
    }
  }

  function getTaxRate(taxGroupId) {
    let taxRate = findInArray("ID", taxGroupId, __orderInfo.TaxGroups)["Rate"];
    return __this.toNumber(taxRate);
  }

  function changeItemByUoM(itemId, uomId, qty) {
    var lineItem = __this.createLineItem(itemId, uomId);
    lineItem.Qty = qty;
    lineItem.PrintQty = qty;
    lineItem.Total = __this.sumLineItem(lineItem);
    $listOrderedItems.updateColumn(itemId, "UnitPrice", lineItem.UnitPrice);
    $listOrderedItems.updateColumn(itemId, "Total", lineItem.Total);
    $listOrderedItems.updateColumn(itemId, "UomID", lineItem.UomID);
    $listOrderedItems.updateColumn(itemId, "Uom", lineItem.Uom);
    __orderInfo.Order[__config.detail.fieldName] = $listOrderedItems.yield();
    __this.summarizeOrder(__orderInfo.Order);
  }

  function changeItemByTaxGroup(lineItem) {
    if (isValidJson(lineItem)) {
      lineItem.Total = __this.sumLineItem(lineItem);
      $listOrderedItems.updateColumn(lineItem.LineID, "Total", lineItem.Total);
      __orderInfo.Order[__config.detail.fieldName] = $listOrderedItems.yield();
      __this.summarizeOrder(__orderInfo.Order);
    }
  }

  this.checkCart = function (succeeded) {
    if (isValidJson(__orderInfo.Order)) {
      let lineItems = __orderInfo.Order[__config.detail.fieldName];
      if (!isValidArray(lineItems)) {
        __this.dialog("Empty Cart", "Please order something first!", "warning");
      } else {
        if (typeof succeeded === "function") {
          succeeded.call(this, __this.fallbackInfo());
        }
      }
    }
  };

  this.displayTax = function () {
    switch (__orderInfo.Setting.TaxOption) {
      case 0:
        $(".tax-display").hide();
        $("[data-field='TaxGroups']").hide();
        $(".tax-rate-display-invoice").hide();
        break;
      case 3:
        $(".tax-display").hide();
        $(".tax-display-invoice").show();
        $("[data-field='TaxGroups']").hide();
        $(".tax-rate-display-invoice").show();
        break;
      default:
        $(".tax-display").show();
        $("[data-field='TaxGroups']").show();
        $(".tax-rate-display-invoice").hide();
        break;
    }
  };

  this.sum = function (items, fieldName, callback) {
    let total = 0;
    for (let i = 0; i < items.length; i++) {
      total += __this.toNumber(items[i][fieldName]);
      if (typeof callback === "function") {
        callback.call(__this, i, items[i], total);
      }
    }
    return total;
  };

  this.calculateOrder = function (order) {
    
    if (isValidJson(order)) {
      __orderInfo.Order = $.extend(__orderInfo.Order, order);
      if (__orderInfo.Setting.TaxOption == 3) {
        if (__orderInfo.Order.Sub_Total <= 0) {
          __orderInfo.Order.TaxRate = 0;
        }
      }

      let subtotal = 0,
        subNet = 0,
        subTotalNoTax = 0;
      if (isValidArray(order[__config.detail.fieldName])) {
        subNet = __this.sum(order[__config.detail.fieldName], "TotalNet");
        subtotal = __this.sum(order[__config.detail.fieldName], "Total");
        subTotalNoTax = __this.sum(
          order[__config.detail.fieldName],
          "TotalNoTax"
        );
      }

      order.Sub_Total = subtotal;
      //__orderInfo.Setting.TaxOption == 1 => Exclude
      const _subtotal = __orderInfo.Setting.TaxOption == 1 ? subNet : subtotal;
      order.CardMemberDiscountValue =
        (order.CardMemberDiscountRate * subtotal) / 100;
      const grandTotalAfterDiscount =
        _subtotal *
        (1 -
          (__this.toNumber(__orderInfo.Order.DiscountRate) +
            __this.toNumber(__orderInfo.Order.PromoCodeDiscRate) +
            __this.toNumber(__orderInfo.Order.BuyXAmGetXDisRate) +
            __this.toNumber(__orderInfo.Order.CardMemberDiscountRate)) /
            100);
      order.DiscountValue =
        (__this.toNumber(order.Sub_Total) *
          __this.toNumber(order.DiscountRate)) /
        100;
      order.PromoCodeDiscValue =
        (__this.toNumber(order.Sub_Total) *
          __this.toNumber(order.PromoCodeDiscRate)) /
        100;
      order.GrandTotal = grandTotalAfterDiscount;
      //TaxOption.VAT
      if (__orderInfo.Setting.TaxOption == 1) {
        order.GrandTotal =
          grandTotalAfterDiscount +
          __this.sum(order[__config.detail.fieldName], "TaxValue");
      }
      order.SubTotalNoTax = subTotalNoTax;
      order.GrandTotal += order.FreightAmount;
      order.GrandTotal_Sys = order.GrandTotal * order.PLRate;
      __this.calculateBuyXAmGetXDis();
      const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(
        (i) => i.AltCurrencyID == i.BaseCurrencyID
      )[0] ?? { DecimalPlaces: 0 };
      const altActiveCurrency = __orderInfo.DisplayPayOtherCurrency.filter(
        (i) => i.IsActive
      )[0] ?? { AltCurrency: "", Rate: 0 };
      order.CurrencyDisplay = altActiveCurrency.AltCurrency;
      order.TaxOption = __orderInfo.Setting.TaxOption;
      order.TaxValue = __this.sum(order[__config.detail.fieldName], "TaxValue");
      __this.calculateInvoiceVAT(order);
      __this.validateOrder(order);
      const gtotalStr = __this.toCurrency(order.GrandTotal);
      order.GrandTotal = __this.toNumber(gtotalStr);
      order.Change =
        __this.toNumber(order.Received) - __this.toNumber(order.GrandTotal);
      order.Change_Display =
        __this.toNumber(order.Change) * __this.toNumber(altActiveCurrency.Rate);
      order.GrandTotal_Display =
        __this.toNumber(order.GrandTotal) *
        __this.toNumber(altActiveCurrency.Rate);
      return __orderInfo.Order;
    }
  };

  this.sumFreights = function () {
    let amount = 0;
    let order = $.extend({}, __orderInfo.Order);
    let freights = order.Freights;
    if (isValidArray(freights)) {
      let totalFreightServiceCharge = 0;
      let totalFreightManual = 0;
      //i.FreightReceiptType::0 => Service Charge
      //i.FreightReceiptType::1 => Manual

      const freightServiceCharges = freights.filter(
        (i) => i.FreightReceiptType == 0
      );
      const freightManuals = freights.filter((i) => i.FreightReceiptType == 1);
      if (freightServiceCharges) {
        freightServiceCharges.forEach((i) => {
          totalFreightServiceCharge =
            (__this.toNumber(
              order.Sub_Total - order.DiscountValue + order.TaxValue
            ) *
              __this.toNumber(i.AmountReven)) /
            100;
        });
      }

      if (freightManuals) {
        freightManuals.forEach((i) => {
          totalFreightManual += __this.toNumber(i.AmountReven);
        });
      }
      amount =
        __this.toNumber(totalFreightManual) +
        __this.toNumber(totalFreightServiceCharge);
      __orderInfo.Order.FreightAmount = amount;
    }

    return __this.toNumber(amount);
  };

  this.calculateInvoiceVAT = function (order) {
    if (__orderInfo.Setting.TaxOption == 3) {
      const altActiveCurrency = __orderInfo.DisplayPayOtherCurrency.filter(
        (i) => i.IsActive
      )[0] ?? { AltCurrency: "", Rate: 0 };
      const taxRate = getTaxRate(__orderInfo.Setting.Tax);
      const disvalueTotal =
        order.Sub_Total *
        (1 -
          (__this.toNumber(order.DiscountRate) +
            __this.toNumber(order.PromoCodeDiscRate) +
            __this.toNumber(order.BuyXAmGetXDisRate) +
            __this.toNumber(order.CardMemberDiscountRate)) /
            100);
      order.TaxRate = taxRate;
      order.TaxGroupID = __orderInfo.Setting.Tax;
      order.TaxValue = (taxRate * disvalueTotal) / 100;
      order.GrandTotal = disvalueTotal + order.TaxValue;

      order.Change =
        __this.toNumber(order.Received) - __this.toNumber(order.GrandTotal);
      order.Change_Display =
        __this.toNumber(order.Change) *
        __this.toNumber(altActiveCurrency?.Rate);
      order.GrandTotal_Display =
        __this.toNumber(order.GrandTotal) *
        __this.toNumber(altActiveCurrency?.Rate);
    }
  };
  this.calculateBuyXAmGetXDis = function () {
    if (isValidArray(__orderInfo.LoyaltyProgram.BuyXAmGetXDis)) {
      const buyx = __orderInfo.LoyaltyProgram.BuyXAmGetXDis.filter(
        (i) => i.Amount <= __orderInfo.Order.GrandTotal
      );
      var buyxMax = 0;
      var _buyx = {};
      buyx.forEach((i) => {
        if (i.Amount > buyxMax) {
          buyxMax = i.Amount;
          _buyx = i;
        }
      });
      if (isValidJson(_buyx)) {
        /**
         * _buyx.DisType : Rate = 1, Value = 2
         * */
        __orderInfo.Order.BuyXAmountGetXDisID = _buyx.ID;
        if (_buyx.DisType == 1) {
          const grandxdisRateValue =
            _buyx.DisRateValue * __orderInfo.Order.GrandTotal;
          const value = grandxdisRateValue == 0 ? 0 : grandxdisRateValue / 100;
          __orderInfo.Order.GrandTotal -= value;
          __orderInfo.Order.BuyXAmGetXDisRate = _buyx.DisRateValue;
          __orderInfo.Order.BuyXAmGetXDisValue = value;
          __orderInfo.Order.BuyXAmGetXDisType = _buyx.DisType;
        }
        if (_buyx.DisType == 2) {
          const rate =
            _buyx.DisRateValue * 100 == 0 || __orderInfo.Order.GrandTotal == 0
              ? 0
              : (_buyx.DisRateValue * 100) / __orderInfo.Order.GrandTotal;
          __orderInfo.Order.GrandTotal -= _buyx.DisRateValue;
          __orderInfo.Order.BuyXAmGetXDisRate = rate;
          __orderInfo.Order.BuyXAmGetXDisValue = _buyx.DisRateValue;
          __orderInfo.Order.BuyXAmGetXDisType = _buyx.DisType;
        }
      } else {
        __orderInfo.Order.BuyXAmGetXDisRate = 0;
        __orderInfo.Order.BuyXAmGetXDisValue = 0;
      }
    }
    return __orderInfo.Order;
  };
  // what to add
  this.checkBuyXQtyGetXDiscount = function (e) {
    const buyx = __orderInfo.LoyaltyProgram.BuyXQtyGetXDis.filter(
      (i) => i.BuyItemID == e.data.ItemID && i.UomID == e.data.UomID
    )[0];
    if (isValidJson(buyx)) {
      const buyxChild = __orderInfo.SaleItems.filter(
        (i) => buyx.DisItemID == i.ItemID
      )[0];
      buyxChild.LineID = `${buyxChild.ID}${buyxChild.ItemID}${buyx.ID}`;
      const itemExisted = $listOrderedItems.find(buyxChild);
      if (e.data.Qty >= buyx.Qty) {
        var subLineItem = __this.createLineItem(buyxChild.ID, buyxChild.UomID);
        subLineItem.LineID = buyxChild.LineID;
        e.data.PromoType = 3; // Buy X Qty Get X Discount
        subLineItem.PromoType = 3;
        subLineItem.PromoTypeDisplay =
          __orderInfo.PromoTypes[subLineItem.PromoType];
        e.data.PromoTypeDisplay = __orderInfo.PromoTypes[e.data.PromoType];
        subLineItem.LinePosition = 1; // LinePosition -> Children
        subLineItem.DiscountRate = buyx.DisRate;
        subLineItem.Qty = subLineItem.PrintQty = subLineItem.Qty;
        __this.sumLineItem(subLineItem);
        subLineItem.IsReadonly = false;
        subLineItem.ParentLineID = e.data.LineID;
        if (!isValidJson(itemExisted))
          __this.addSubLineItem(
            subLineItem,
            `<i class='fas fa-gift' style="color: #f1c40f"></i>`,
            false,
            true
          );
      }
      if (e.data.Qty < buyx.Qty) {
        if (itemExisted) {
          itemExisted.Qty = 0;
          e.data.PromoType = 0; // Buy X Qty Get X Discount
          itemExisted.PromoType = 0;
          itemExisted.PromoTypeDisplay =
            __orderInfo.PromoTypes[itemExisted.PromoType];
          e.data.PromoTypeDisplay = __orderInfo.PromoTypes[e.data.PromoType];
          updateLineItem(itemExisted.LineID, -1, false);
        }
      }
    }
  };

  this.summarizeOrder = function (order = {}, invoke2ndScreen = true) {
    if (isValidJson(order)) {
      __this.calculateOrder(order);
      if (__orderInfo.Setting.DaulScreen && invoke2ndScreen) {
        __this.sendOrder2ndScreen();
      }
      const currencyBase = __orderInfo.DisplayPayOtherCurrency.filter(
        (i) => i.BaseCurrencyID === i.AltCurrencyID
      )[0] ?? { BaseCurrency: 0 };
      $("#summary-discount-rate").text(
        "(" +
          __this.toCurrency(
            __orderInfo.Order.DiscountRate +
              __orderInfo.Order.BuyXAmGetXDisRate +
              __orderInfo.Order.CardMemberDiscountRate
          ) +
          "%)"
      );
      $("#summary-discount").text(
        currencyBase.BaseCurrency +
          " " +
          __this.toCurrency(
            __orderInfo.Order.DiscountValue +
              __orderInfo.Order.BuyXAmGetXDisValue +
              __orderInfo.Order.CardMemberDiscountValue,
            currencyBase?.DecimalPlaces
          )
      );
      $("#summary-vat").text(
        currencyBase.BaseCurrency +
          " " +
          __this.toCurrency(
            __orderInfo.Order.TaxValue,
            currencyBase?.DecimalPlaces
          )
      );
      $("#summary-vat-rate").text(
        __this.toCurrency(__orderInfo.Order.TaxRate) + " %"
      );
      $("#summary-sub-total").text(
        currencyBase.BaseCurrency +
          " " +
          __this.toCurrency(
            __orderInfo.Order.Sub_Total,
            currencyBase?.DecimalPlaces
          )
      );
      $("#total-qty").text(
        currencyBase.BaseCurrency +
          " " +
          __this.toCurrency(
            __orderInfo.Order.Sub_Total,
            currencyBase?.DecimalPlaces
          )
      );
      $("#summary-total").text(
        currencyBase.BaseCurrency +
          " " +
          __this.toCurrency(
            __orderInfo.Order.GrandTotal,
            currencyBase?.DecimalPlaces
          )
      );
      $("#count-items").text(
        __orderInfo.Order[__config.detail.fieldName].length
      );
      __this.displayOrderTitle("#order-number", __config.orderOnly);
      __this.displayTax();
    }
  };
  $("#list-order-receipt").click(function (e) {
    undefinedrefno = "notundefined";
  });

  this.displayOrderTitle = function (
    selector,
    orderOnly = false,
    itemCount = 0
  ) {
    let count = __orderInfo.Order[__config.detail.fieldName].length;
    if (itemCount > 0) {
      count = itemCount;
    }
    if (undefinedrefno == "undefined") {
      let title = isValidJson(__orderInfo.OrderTable)
        ? __orderInfo.OrderTable.Name + " > #" + text_refno
        : "#" + __orderInfo.Order.OrderNo;
      if (orderOnly) {
        title = __orderInfo.Order.OrderNo;
      }
      $("#count-row-number-text").text(count);
      $(selector).text(title);
    } else {
      let title = isValidJson(__orderInfo.OrderTable)
        ? __orderInfo.OrderTable.Name + " > #" + __orderInfo.Order.OrderNo
        : "#" + __orderInfo.Order.OrderNo;
      if (orderOnly) {
        title = __orderInfo.Order.OrderNo;
      }
      $("#count-row-number-text").text(count);
      $(selector).text(title);
    }

    if (isEmpty(text_refno)) {
      let title = isValidJson(__orderInfo.OrderTable)
        ? __orderInfo.OrderTable.Name + " > #" + __orderInfo.Order.OrderNo
        : "#" + __orderInfo.Order.OrderNo;
      if (orderOnly) {
        title = __orderInfo.Order.OrderNo;
      }
      $("#count-row-number-text").text(count);
      $(selector).text(title);
    }
  };

  this.toNumber = function (value) {
    if (value !== undefined) {
      if (typeof value === "number") {
        value = value.toString();
      }
      return parseFloat(value.split(",").join(""));
    }
  };

  this.toCurrency = function (value, decimalPlaces = -1) {
    const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(
      (i) => i.AltCurrencyID == i.BaseCurrencyID
    )[0] ?? { DecimalPlaces: 0 };
    decimalPlaces =
      decimalPlaces == -1 ? baseCurrency.DecimalPlaces : decimalPlaces;
    return preciseRound(value, decimalPlaces);
  };

  function preciseRound(num, decimals) {
    if (decimals > 7) {
      decimals = 7;
      console.error("Too big size of fractional digits.");
    }
    var sign = num >= 0 ? 1 : -1;
    return (
      Math.round(num * Math.pow(10, decimals) + sign * 0.001) /
      Math.pow(10, decimals)
    ).toFixed(decimals);
  }

  function createZeroes(length) {
    let result = "";
    for (let i = 0; i < length; i++, result += "0");
    return result;
  }

  function formatNumber(value, decimalPlaces = 3) {
    if (!isNaN(value)) {
      let format = { decimal: ".", separator: ",", fixed: decimalPlaces };
      //format = $.extend(format, dataFormat);
      let _value = value.toString();
      if (_value.indexOf(format.decimal) > 0) {
        let values = value.toString().split(format.decimal);
        values[values.length - 1] += createZeroes(
          format.fixed - values[values.length - 1].length
        );
        values[values.length - 1] = values[values.length - 1].substring(
          0,
          format.fixed
        );
        _value = values.join(format.decimal);
      } else {
        _value = _value + format.decimal + createZeroes(format.fixed);
      }

      let pattern = new RegExp(
        "\\d(?=(\\d{decimalPlaces})+\\" + format.decimal + ")",
        "g"
      );
      let data = _value.replace(pattern, "$&" + format.separator);
      if (format.fixed == 0) {
        data = data.split(".")[0];
      }
      return data;
    }
  }

  this.loadScreen = function (enabled = true) {
    $(__loadscreen).show();
    if (!enabled) {
      $(__loadscreen).hide();
    }
  };

  this.checkOpenShift = function (succeed) {
    $.post(__urls.checkOpenShift, function (hasOpenShift) {
      if (hasOpenShift) {
        succeed(__this.fallbackInfo());
      } else {
        let alert = __this.dialog(
          "Check Open Shift",
          "You need to open shift before sale. Do you want to open shift now?",
          "warning",
          "yes-no"
        );
        alert.confirm(function () {
          alert.shutdown();
          __this.openShift(succeed);
        });
      }
    });
  };

  this.validateOrder = function (order, orderDetails) {
    if (checkOrder(order)) {
      __orderInfo.Order = order;
    }

    if (checkOrderDetails(orderDetails)) {
      __this.bindLineItems(orderDetails);
    }
    return __orderInfo.Order;
  };

  function isNegativeQty() {
    var lineItems = __orderInfo.Order[__config.detail.fieldName];
    if (!isValidArray(lineItems)) {
      return false;
    }
    var isNegativeQty = lineItems.some(function (item) {
      return item.OrderDetailID > 0 && item.PrintQty < 0;
    });
    return isNegativeQty;
  }

  function checkReducedQtyItems(onConfirm) {
    if (!isNegativeQty()) {
      onConfirm.call(__this, __this.fallbackInfo());
      return;
    }

    if (typeof onConfirm === "function") {
      switch (__orderInfo.Order.CheckBill) {
        case "N":
          __this.checkPrivilege("P009", function () {
            onConfirm.call(__this, __this.fallbackInfo());
          });

          break;
        case "Y":
          __this.checkPrivilege("P028", function () {
            onConfirm.call(__this, __this.fallbackInfo());
          });
          break;
        default:
          if (!isNegativeQty()) {
            onConfirm.call(__this, __this.fallbackInfo());
          }
          break;
      }
    }
  }

  var __isSubmitting = false;
  this.submitOrder = function (order, printType, succeeded) {
    if (!__this.validateSettings()) {
      return;
    }
    if (!__isSubmitting) {
      __orderInfo.Order = __this.validateOrder(
        order,
        order[__config.detail.fieldName]
      );
      __orderInfo.Order.GrandTotalCurrencies =
        __orderInfo.DisplayPayOtherCurrency.filter(
          (i) => i.AltCurrencyID != i.BaseCurrencyID && i.IsShowCurrency
        );
      __orderInfo.Order.ChangeCurrencies =
        __orderInfo.Order.GrandTotalCurrencies;
      __orderInfo.Order.DisplayPayOtherCurrency =
        __orderInfo.DisplayPayOtherCurrency;
      __orderInfo.Order.RefNo = text_refno;
      if (!isValidArray(__orderInfo.Order.GrandTotalOtherCurrencies)) {
        __orderInfo.Order.GrandTotalOtherCurrencies =
          __orderInfo.DisplayGrandTotalOtherCurrency;
      }
      __this.checkCart(function () {
        checkReducedQtyItems(function () {
          __this.checkOpenShift(function () {
            if (parseFloat(__orderInfo.Order.GrandTotal) < 0) {
              __this.dialog(
                "Invalid Amount",
                "Total amount must be positive.",
                "warning"
              );
              return;
            }
            if (__orderInfo.Setting.AutoQueue) {
              __this.loadScreen();
              postOrder(__orderInfo, printType, succeeded, __isSubmitting);
            } else {
              let dialog = new DialogBox({
                caption: "Set Order Number",
                content: {
                  selector: "#edit-orderno-content",
                },
                type: "ok-cancel",
              });
              dialog.invoke(function () {
                dialog.content
                  .find(".order-number")
                  .val(__orderInfo.Order.OrderNo);
              });
              dialog.confirm(function () {
                let $orderNo = dialog.content.find(".order-number");
                if ($orderNo.val() == "") {
                  $(".order-number-erorr").text("Please input order number!");
                  dialog.content.find(".order-number").focus();
                } else {
                  __orderInfo.Order.OrderNo = $orderNo.val();
                  __this.loadScreen();
                  postOrder(__orderInfo, printType, succeeded, __isSubmitting);
                }
                dialog.shutdown();
              });

              dialog.reject(function () {
                dialog.shutdown();
                __this.readyPayment = true;
              });
              __isSubmitting = false;
            }
          });
        });
      });
    }
  };

  function postOrder(__orderInfo, printType, succeeded, __isSubmitting) {
    //TypeOfSerialBatch { None = 0, Serial = 1, Batch = 2 }
    var promo = $.trim($("#promo-code").val());
    $.post(
      __urls.submitOrder,
      {
        order: __orderInfo.Order,
        printType: printType,
        serials: __batchSerial.serial,
        batches: __batchSerial.batches,
        promocode: promo,
        //order: JSON.stringify(__orderInfo.Order),
        //printType: printType,
        //batch: __batchSerial.batches ? JSON.stringify(__batchSerial.batches) : __batchSerial.batches,
        //serial: __batchSerial.serial ? JSON.stringify(__batchSerial.serial) : __batchSerial.serial,
        //promocode: promo
      },
      function (res) {
        if (isValidJson(res.ErrorMessages)) {
          var errKeys = Object.getOwnPropertyNames(res.ErrorMessages);
          if (errKeys.length > 0) {
            let msg = "";
            for (let i = 0; i < errKeys.length; i++) {
              msg += res.ErrorMessages[errKeys[i]];
            }
            __this.dialog(errKeys[0], msg, "warning");
            __this.loadScreen(false);
            __isSubmitting = false;
            __this.readyPayment = true;
            return;
          }
        }

        if (res.ReceiptID > 0 && res.PreviewReceipt == true) {
          let method = "PreviewReceipt";
          let __printUrl = `${_printUrlPOS}POS/${method}/?id=${res.ReceiptID}`;
          window.open(__printUrl, "_blank");
        }

        if (res.ReceiptID > 0) {
          if (
            __orderInfo.Setting.ReceiptTemplate == "KSMS" &&
            (__orderInfo.Setting.Receiptsize == "A4" ||
              __orderInfo.Setting.Receiptsize == "A5")
          ) {
            window.open(
              `/Print/PrintInvoiceKSMS?id=${res.ReceiptID}`,
              "_blank"
            );
          }
        }

        if (printType === "Pay") {
          if (res.TypeOfSerialBatch == 1) {
            __this.loadScreen(false);
            const serial = SerialTemplate({
              data: {
                serials: res.ItemSerialBatches,
              },
            });
            serial.serialTemplate();
            const seba = serial.callbackInfo();
            __batchSerial.serial = seba.serials;
            __this.readyPayment = true;
          } else if (res.TypeOfSerialBatch == 2) {
            __this.loadScreen(false);
            const batch = BatchNoTemplate({
              data: {
                batches: res.ItemSerialBatches,
              },
            });
            batch.batchTemplate();
            const seba = batch.callbackInfo();
            __batchSerial.batches = seba.batches;
            __this.readyPayment = true;
          } else if (res.ItemsReturns.length > 0) {
            alertOutStock(res.ItemsReturns);
          } else {
            if (typeof succeeded === "function") {
              succeeded(__this.fallbackInfo());
            }
            $(".item-search-barcode").focus();
            __batchSerial.batches = null;
            __batchSerial.serial = null;
          }
        } else {
          if (res.ItemsReturns.length > 0) {
            alertOutStock(res.ItemsReturns);
          } else {
            if (typeof succeeded === "function") {
              succeeded(__this.fallbackInfo());
            }
          }
        }

        __this.loadScreen(false);
        __isSubmitting = false;
      }
    ).fail(function (err) {
      __isSubmitting = false;
    });
  }

  this.openShift = function (succeed) {
    __this.checkPrivilege("P012", function (posInfo) {
      __this.startShiftForm(false, "Cash In", __urls.processOpenShift, succeed);
    });
  };

  this.closeShift = function (succeed) {
    __this.checkPrivilege("P013", function (posInfo) {
      __this.startShiftForm(
        true,
        "Cash Out",
        __urls.processCloseShift,
        succeed
      );
    });
  };

  this.startShiftForm = function (isCashout, caption, submitUrl, onSucceed) {
    let $shiftDialog = new DialogBox({
      caption: caption,
      content: {
        selector: "#shift-form-content",
      },
      position: "top-center",
      type: "ok-cancel",
      icon: "far fa-money-bill-alt",
      button: {
        ok: {
          text: "Done",
          callback: function (e) {
            this.meta.shutdown();
          },
        },
        cancel: {
          callback: function (e) {
            this.meta.shutdown();
          },
        },
      },
    });

    $shiftDialog.invoke(function () {
      if (!isCashout) {
        $shiftDialog.content.find(".wrap-grand-total").hide();
      }
      $.post(__urls.getShiftTemplate, function (resp) {
        if (isValidJson(resp)) {
          $shiftDialog.content
            .find(".total-cash")
            .text(
              resp.CurrencySys +
                " " +
                __this.toCurrency(sumTotal(resp.ShiftForms))
            );
          $shiftDialog.content
            .find(".grand-total")
            .text(
              resp.CurrencySys + " " + __this.toCurrency(resp.GrandTotalSys)
            );
          ViewTable({
            selector: "table.cash-box",
            keyField: "ID",
            visibleFields: ["Decription", "InputCash", "Currency"],
            paging: {
              enabled: false,
            },
            columns: [
              {
                name: "InputCash",
                template: "<input autofocus class='input-cash'>",
                on: {
                  keyup: function (e) {
                    let curr = resp.ShiftForms.find((w) => w.ID == e.key);
                    if (this.value === "") {
                      this.value = "0";
                    }
                    curr.InputCash = __this.toNumber(this.value);
                    curr.Amount =
                      __this.toNumber(this.value) *
                      __this.toNumber(curr.RateIn);
                    $shiftDialog.content
                      .find(".total-cash")
                      .text(
                        resp.CurrencySys +
                          " " +
                          __this.toCurrency(sumTotal(resp.ShiftForms))
                      );
                    $shiftDialog.content
                      .find(".grand-total")
                      .text(
                        resp.CurrencySys +
                          " " +
                          __this.toCurrency(resp.GrandTotalSys)
                      );
                  },
                },
              },
            ],
          }).bindRows(resp.ShiftForms);
          $shiftDialog.confirm(function () {
            __this.loadScreen();
            $.post(
              submitUrl,
              { total: __this.toNumber(sumTotal(resp.ShiftForms)) },
              function (model) {
                if (model.Count > 0) {
                  let d_setting = {
                    caption: "Access Denied",
                    icon: "warning",
                    type: "ok",
                  };

                  let dialog = new DialogBox(d_setting);
                  $(dialog.content)[0].textContent = "";
                  ViewMessage({
                    summary: {
                      bordered: false,
                    },
                  })
                    .bind(model)
                    .appendTo(dialog.content.find(".error-message"));
                } else {
                  if (typeof onSucceed === "function") {
                    onSucceed.call(__this, model);
                    __this.loadScreen(false);
                  }
                  $shiftDialog.shutdown();
                }
              }
            );
          });
          $shiftDialog.content.find("table .input-cash").asPositiveNumber();
        }
      });
    });

    function sumTotal(array) {
      for (
        var index = 0, // The iterator
          length = array.length, // Cache the array length
          sum = 0; // The total amount
        index < length; // The "for"-loop condition
        sum += array[index++].Amount // Add number on each iteration
      );
      return sum;
    }
    return $shiftDialog;
  };

  function resetGroupItems(showOnlyAddon = false) {
    __this.showNotFound(false);
    $(__groupItemStep).find(":not(:first-child)").remove();
    let groups = __orderInfo.BaseItemGroups;
    if (showOnlyAddon) {
      groups = $.grep(groups, function (group) {
        return group.IsAddon || group.KhmerName.includes("*");
      });
    }

    $gridItemPaging.load(groups, groups[0], function (info) {
      $(__wrapGridItem).children().remove();
      $.each(info.dataset, function (i, item) {
        if (item.ItemID <= 0) {
          createGroupItemGrid(item, 0, showOnlyAddon);
        }
      });
    });
  }

  this.dialog = function (caption, content, icon, type, closeButton = true) {
    return new DialogBox({
      caption: caption,
      content: content,
      icon: icon,
      type: type ? type : undefined,
      closeButton: closeButton,
    });
  };
  // remove save order
  this.removeOrder = function (orderId) {
    __orderInfo.Orders = $.grep(__orderInfo.Orders, function (order) {
      return order.OrderID != orderId;
    });
  };

  function initFormQR(code, onSuccess, onFail) {
    let dialog = new DialogBox({
      caption: "QR Authorization",
      icon: "fas fa-qrcode",
      content: {
        selector: "#form-qr",
      },
      button: {
        ok: { text: "CLOSE" },
      },
    });

    var $accessToken = dialog.content.find("#access-token");
    var $tokenRadar = dialog.content.find("#token-radar");
    var $message = $(".validation-summary-valid");
    $(dialog.content.parent()).on("click", function () {
      $accessToken.focus();
      dialog.content.find("#form-qr").addClass("frame-active");
    });

    setTimeout(function () {
      $accessToken.focus();
      dialog.content.find("#form-qr").addClass("frame-active");
    }, 250);
    $accessToken.on("focusout", function () {
      dialog.content.find("#form-qr").removeClass("frame-active");
    });
    var __reading = false;
    $accessToken.on("input", function (e) {
      e.preventDefault();
      if (!__reading) {
        $tokenRadar.attr("src", "/pos/images/griddot.gif");
        var __this = this;
        setTimeout(function () {
          var _token = $accessToken.val();
          let creds = {
            Code: code,
            Username: "",
            Password: "",
            AccessToken: _token,
          };
          $.ajax({
            type: "POST",
            url: __urls.getUserAccess,
            data: { creds: creds },
            success: function (message) {
              if (!message.IsRejected) {
                $message.show();
                setTimeout(function () {
                  if (typeof onSuccess === "function") {
                    onSuccess.call(__this, __orderInfo);
                  }
                  dialog.shutdown();
                }, 500);
              } else {
                $tokenRadar.attr("src", "/pos/images/qr-default.png");
                $message.show();
                setTimeout(function () {
                  if (typeof onFail === "function") {
                    onFail.call(__this, __orderInfo);
                  }
                  $message.hide();
                  dialog.shutdown();
                }, 1500);
              }

              ViewMessage({
                summary: {
                  bordered: false,
                },
              }).bind(message);
              $accessToken.focus().val("");
            },
          });
        }, 1000);
        $(__this).off("input");
        __reading = true;
      }
    });
    dialog.confirm(function () {
      dialog.shutdown();
    });
  }
  function initFormPassword(code, onSuccess) {
    let dialog = new DialogBox({
      caption: "Authorization",
      icon: "fas fa-user-shield",
      content: {
        selector: "#form-password",
      },
      type: "ok-cancel",
      button: {
        ok: { text: "Access" },
      },
    });

    dialog.confirm(function () {
      let username = dialog.content.find("#username").val();
      let password = dialog.content.find("#password").val();
      let creds = {
        Code: code,
        Username: username,
        Password: password,
        AcccesToken: "",
      };
      $.ajax({
        type: "POST",
        url: __urls.getUserAccess,
        data: { creds: creds },
        success: function (message) {
          ViewMessage({
            summary: {
              bordered: false,
            },
          }).bind(message);
          if (!message.IsRejected) {
            setTimeout(function () {
              onSuccess.call(__this, __orderInfo);
              dialog.shutdown();
            }, 500);
          }
        },
      });
    });
    dialog.reject(function () {
      dialog.shutdown();
      __this.readyPayment = true;
    });
  }
  this.checkPrivilege = function (code, onSuccess, onFail) {
    $.post(__urls.checkPrivilege, { code: code }, function (isAllowed) {
      if (isAllowed === true) {
        if (typeof onSuccess === "function") {
          onSuccess.call(__this, __orderInfo);
        }
      } else {
        if (typeof onSuccess === "function") {
          if (__orderInfo.AuthOption == 0) {
            initFormPassword(code, onSuccess);
          } else {
            initFormQR(code, onSuccess);
          }
        }
        if (typeof onFail === "function") {
          onFail.call(__this, __orderInfo);
        }
      }
    }).fail(function () {
      if (typeof onFail === "function") {
        onFail.call(__this, __orderInfo);
      }
    });
  };

  function alertOutStock(itemReturns) {
    if (itemReturns.length > 0) {
      ViewTable({
        keyField: "LineID",
        selector: "#item-stock-info-listview",
        visibleFields: [
          "Code",
          "KhmerName",
          "Instock",
          "OrderQty",
          "Committed",
        ],
        paging: {
          enabled: false,
        },
      }).bindRows(itemReturns);
      let dialog = new DialogBox({
        caption: "Not enough stock",
        icon: "warning",
        content: {
          selector: "#item-stock-info",
        },
      });
      dialog.confirm(function () {
        dialog.shutdown();
      });
    }
  }

  function filterGroupItems(
    group1,
    group2,
    group3,
    priceListId,
    level,
    onlyAddon = false
  ) {
    $(__notFound).removeClass("show");
    let key = group1 + "-" + group2 + "-" + group3;
    let items = __orderInfo.Cache.ItemsByGroup[key];
    if (isValidArray(items)) {
      bindItemGrids(items, items[0], level, onlyAddon);
    } else {
      __this.loadScreen();
      $.post(
        __urls.getItemGroups,
        { group1, group2, group3, priceListId, level, onlyAddon },
        function (resp) {
          __orderInfo.Cache.ItemsByGroup[key] = resp;
          bindItemGrids(resp, resp[0], level, onlyAddon);
          __this.loadScreen(false);
        }
      );
    }
  }

  function filterItemsByGroup(
    group1,
    group2,
    group3,
    level,
    onlyAddon = false
  ) {
    let itemsByGroup = [];
    if (onlyAddon) {
      itemsByGroup = __orderInfo.SaleItems.filter(
        (i) => i.ItemType.toLowerCase() == "addon"
      );
    }

    switch (level) {
      case 1:
        itemsByGroup = __orderInfo.ItemGroups.filter(
          (g) => g.Level == level + 1 && g.Group1 == group1
        );
        let itemsByGroup1 = __orderInfo.SaleItems.filter(
          (i) =>
            i.Group1 == group1 &&
            !__orderInfo.ItemGroups.some((g) => g.Group2 == i.Group2)
        );
        itemsByGroup = itemsByGroup.concat(itemsByGroup1);
        break;
      case 2:
        itemsByGroup = __orderInfo.ItemGroups.filter(
          (g) =>
            g.Level == level + 1 && g.Group1 == group1 && g.Group2 == group2
        );
        let itemsByGroup2 = __orderInfo.SaleItems.filter(
          (i) =>
            i.Group1 == group1 &&
            i.Group2 == group2 &&
            !__orderInfo.ItemGroups.some((g) => g.Group3 == i.Group3)
        );
        itemsByGroup = itemsByGroup.concat(itemsByGroup2);
        break;
      case 3:
        itemsByGroup = __orderInfo.ItemGroups.filter(
          (g) =>
            g.Level == level + 1 &&
            g.Group1 == group1 &&
            g.Group2 == group2 &&
            g.Group3 == group3
        );
        let itemsByGroup3 = __orderInfo.SaleItems.filter(
          (i) =>
            i.Group1 == group1 &&
            i.Group2 == group2 &&
            i.Group3 == group3 &&
            __orderInfo.ItemGroups.some((g) => g.Group3 == i.Group3)
        );
        itemsByGroup = itemsByGroup.concat(itemsByGroup3);
        break;
    }
    bindItemGrids(itemsByGroup, itemsByGroup[0], level, onlyAddon);
  }

  function createGroupItemGrid(item, level, onlyAddon = false) {
    let multiUomItems = findMultiUomItems(item.ItemID);
    var data = "",
      _imagePath = getImage(item.Image);
    let $grid = $(
      "<div class='grid' group1='" +
        item.Group1 +
        "' group2='" +
        item.Group2 +
        "' group3='" +
        item.Group3 +
        "'></div>"
    )
      .append(data)
      .append('<div class="grid-caption">' + item.KhmerName + "</div>");
    let $grid_image = $("<div class='grid-image'></div>").append(
      '<img src="' + _imagePath + '">'
    );
    let multiCount =
      multiUomItems.length > 0 ? "(" + multiUomItems.length + ")" : "";
    let $code = clone("div")
      .addClass("grid-subtitle")
      .text(item.Code + multiCount);
    $grid.append($grid_image);
    if (!isEmpty(item.Code)) {
      $grid.append($code);
    }

    if (__orderInfo.Setting.Portraite) {
      $grid.addClass("portrait");
    }

    $grid.on("click", function () {
      level++;
      if (multiUomItems.length > 1) {
        $(__wrapGridItem).children().remove();
        bindSingleItemGrids(multiUomItems);
      } else {
        if (__isScanQrcode)
          onClickGroupItemQR(
            item.Group1,
            item.Group2,
            item.Group3,
            item.KhmerName,
            level,
            onlyAddon
          );
        else
          onClickGroupItem(
            item.Group1,
            item.Group2,
            item.Group3,
            item.KhmerName,
            level,
            onlyAddon
          );
      }
    });
    $(__wrapGridItem).append($grid);
  }

  function findMultiUomItems(itemId) {
    let multiUomItems = $.grep(__orderInfo.SaleItems, function (item) {
      if (item.ItemID > 0) {
        return item.ItemID === itemId;
      }
    });
    return multiUomItems;
  }

  function groupBy(values, keys, process) {
    let grouped = {};
    $.each(values, function (c, a) {
      keys
        .reduce(function (o, k, i) {
          o[a[k]] = o[a[k]] || (i + 1 === keys.length ? [] : {});
          return o[a[k]];
        }, grouped)
        .push(a);
    });
    if (!!process && typeof process === "function") {
      process(grouped);
    }
    return grouped;
  }

  $(".all-groups", __groupItemStep).on("click", function (e) {
    __orderInfo.OrderDetail = {};
    switch (__orderInfo.Setting.ItemViewType) {
      case 0:
        resetGroupItems();
        break;
      case 1:
        showInListview(__orderInfo.SaleItems);
        break;
    }

    $(this).siblings().removeClass("active");
    $(this).addClass("active");
  });

  function onClickGroupItem(
    group1,
    group2,
    group3,
    name,
    level,
    onlyAddon = false
  ) {
    //Find group from controler
    filterItemsByGroup(group1, group2, group3, level, onlyAddon);
    createGroupItemStep(group1, group2, group3, name, level);
  }

  function createGroupItemStep(group1, group2, group3, name, level) {
    $(__groupItemStep).append(
      $(
        '<li class="active"' +
          '" data-group1="' +
          group1 +
          '" data-group2="' +
          group2 +
          '" data-group3="' +
          group3 +
          '" data-level="' +
          level +
          '" >' +
          name +
          "</li>"
      ).on("click", function () {
        __orderInfo.OrderDetail = {};
        filterItemsByGroup(group1, group2, group3, level);
        $(this).addClass("active").nextAll().remove();
      })
    );
    $(__groupItemStep).children(":not(:last-child)").removeClass("active");
  }

  function bindItemGrids(items, activeItem, level, onlyAddon = false) {
    __this.showNotFound(false);
    if (!isValidArray(items)) {
      __this.showNotFound();
    }

    $gridItemPaging.load(items, activeItem, function (info) {
      bindGroupItemGrids(info.dataset, level, onlyAddon);
    });
  }

  function bindSingleItemGrids(items) {
    const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(
      (i) => i.AltCurrencyID == i.BaseCurrencyID
    )[0] ?? { DecimalPlaces: 0 };
    $.each(items, function (i, item) {
      let discType = item.TypeDis.toLowerCase() === "percent" ? "%" : "",
        imagePath = getImage(item.Image);
      let khName = item.KhmerName + " (" + item.UoM + ")";
      let $calc = $(
        "<i class='icon-calculator csr-pointer fas fa-calculator fa-lg fn-gray'></i>"
      ).on("click", openCalculator);
      let $grid = clone("div").addClass("grid").attr("data-id", item.ID),
        $title = clone("div")
          .addClass("grid-caption")
          .text(khName)
          .prop("title", khName),
        $image = clone("div")
          .addClass("grid-image")
          .append("<img src=" + imagePath + ">"),
        $price = clone("div")
          .addClass("price")
          .append(baseCurrency.BaseCurrency + " "),
        $priceSlash = clone("div")
          .addClass("price-slash")
          .text(
            baseCurrency.BaseCurrency + " " + __this.toCurrency(item.UnitPrice)
          ),
        $discount = clone("div").addClass("discount hover-resize"),
        $barcode = clone("div").addClass("grid-subtitle").text(item.Code),
        $instock = clone("div")
          .addClass("instock")
          .text("Stock: " + __this.toCurrency(item.InStock, 0)),
        $qty = createScale(item, $calc);
      if (__orderInfo.Setting.Portraite) {
        $grid.addClass("portrait");
      }
      if (item.InStock > 0) {
        $instock.addClass("fn-green");
      } else {
        $instock.addClass("fn-red");
      }

      $image.append($qty).append($price);
      if (item.Process.toLowerCase() != "standard") {
        $image.append($instock);
      }
      if (item.TypeDis == "Cash" && item.DiscountRate > 0) {
       
        $discount.text(-item.DiscountRate + item.Symbol);
        $image.append($discount);
        $price.append(" " +__this.toCurrency( __this.toNumber(item.UnitPrice) - item.DiscountRate,baseCurrency.DecimalPlaces));
        $image.append($priceSlash);
      } else if (item.TypeDis == "Percent" && item.DiscountRate > 0) {
        $discount.text(-item.DiscountRate + "%");
        $image.append($discount);
        $price.append(" " + __this.toCurrency( __this.toNumber(item.UnitPrice) * (1 - item.DiscountRate / 100),baseCurrency.DecimalPlaces));
        $image.append($priceSlash);
      } else {
        $price.append(
          __this.toCurrency(
            __this.toNumber(item.UnitPrice) * (1 - item.DiscountRate / 100),
            baseCurrency.DecimalPlaces
          )
        );
      }
      // if (item.DiscountRate > 0) {
      //     $discount.text(-item.DiscountRate + "%");
      //     $image.append($discount);
      //     $price.append(" " + __this.toCurrency(__this.toNumber(item.UnitPrice) * (1 - item.DiscountRate / 100), baseCurrency.DecimalPlaces));
      //     $image.append($priceSlash);
      // } else {
      //     $price.append(__this.toCurrency(__this.toNumber(item.UnitPrice) * (1 - item.DiscountRate / 100), baseCurrency.DecimalPlaces));
      // }

      $image.append($price);
      $grid.append($title).append($image).append($barcode).append($calc);
      if (__isScanQrcode) {
        if (__orderInfo.Setting.IsOrderByQR) {
          $image.on("click", function (e) {
            if (!$grid.find(".scale-box").hasClass("active")) {
              item.PrintQty = 1;
              onFirstClickItemGrid(item.ID, item.PrintQty, item);
            }
          });
        }
      } else {
        $image.on("click", function (e) {
          if (!$grid.find(".scale-box").hasClass("active")) {
            item.PrintQty = 1;
            onFirstClickItemGrid(item.ID, item.PrintQty, item);
          }
        });
      }
      $(__wrapGridItem).append($grid);
    });
  }

  function bindGroupItemGrids(items, level, onlyAddon = false) {
    $(__wrapGridItem).children().remove();
    //sortBy(items, __orderInfo.Setting.SortBy.Field, __orderInfo.Setting.SortBy.Desc);

    if (items.length > 0) {
      let multiUomItem = groupBy(items, ["ItemID"]);
      let props = Object.getOwnPropertyNames(multiUomItem);
      for (let i = 0; i < props.length; i++) {
        if (props[i] > 0 && multiUomItem[props[i]].length > 1) {
          createGroupItemGrid(multiUomItem[props[i]][0], level, onlyAddon);
        }
      }

      $.each(items, function (i, item) {
        if (item.ItemID <= 0) {
          createGroupItemGrid(item, level, onlyAddon);
        } else {
          if (multiUomItem[item.ItemID].length === 1) {
            bindSingleItemGrids(multiUomItem[item.ItemID]);
          }
        }
      });
    }
  }

  //Mini calculator
  function openCalculator(e) {
    e.preventDefault();
    const calc = new DialogCalculator();
    calc.accept((e) => {
      let qty = parseFloat($(calc.template).find(".navigator.output").text());
      let LineID = parseInt($(this).parent(".grid").data("id"));
      calc.shutdown("before", function () {
        if (qty < 0) {
          return;
        }
        setQty(LineID, qty, false);
      });
    });

    $(this).parent().addClass("active");
    $(this)
      .parent()
      .parent()
      .parent()
      .addClass("active")
      .siblings()
      .removeClass("active")
      .find(".wrap-scale")
      .removeClass("active");
  }

  function openLineCalculator(e, data) {
    e.preventDefault();
    const calc = new DialogCalculator();
    calc.accept((e) => {
      let qty = parseFloat($(calc.template).find(".navigator.output").text());
      calc.shutdown("before", function () {
        if (qty < 0) {
          return;
        }
        setQty(data.LineID, qty);
        updateLineItem(data.LineID, 1, false);
      });
    });
  }

  function setQty(itemKey, value, isAdded = true) {
    let orderDetail = $listOrderedItems.find(itemKey);
    if (!isValidJson(orderDetail)) {
      orderDetail = __this.createLineItem(itemKey);
    }
    orderDetail.Qty -= orderDetail.PrintQty;
    orderDetail.PrintQty += value;
    orderDetail.Qty += orderDetail.PrintQty;
    if (!isAdded) {
      orderDetail.Qty = value;
      orderDetail.PrintQty = value;
    }

    __this.sumLineItem(orderDetail);
    $listOrderedItems.updateRow(orderDetail);
    updateItemGridView(orderDetail);
  }

  function createScale(item, $calc) {

    let itemOrder = $listOrderedItems.find(item.ID);
    if (itemOrder) {
      item.PrintQty = itemOrder.PrintQty;
    } else {
      item.PrintQty = 0;
    }

    let $scaleBox = clone("div").addClass("scale-box"),
      $scaleDown = clone("i").addClass("scale-down").text("-"),
      $scaleUp = clone("i").addClass("scale-up").text("+"),
      $value = clone("div").addClass("scale-label").text(item.PrintQty);

    if (item.PrintQty > 0) {
      $scaleBox.addClass("active");
      $calc.addClass("show");
    }

    if (
      isValidJson(__orderInfo.OrderDetail) &&
      __orderInfo.OrderDetail.LineID != undefined
    ) {
      $scaleBox.removeClass("active");
      $calc.removeClass("show");
    }

    $scaleDown.on("click", function (e) {
      const lineId = $(this).parent().parent().parent().data("id");
      const orderD = findInArray(
        "LineID",
        lineId,
        __orderInfo.Order[__config.detail.fieldName]
      );
      if (orderD.OrderDetailID > 0) {
        onGridScale(item.ID, -1);
      } else {
        e.stopPropagation();
        if (item.PrintQty >= 0) {
          onGridScale(item.ID, -1);
        }
      }
    });

    $scaleUp.on("click", function (e) {
      if (__isScanQrcode) {
        __this.checkPrivilegeQR(
          "P010",
          function () {
            e.stopPropagation();
            onGridScale(item.ID, 1);
          },
          function () {
            __this.dialog(
              "Add QTY",
              "You cannot add more QTY on this item",
              "warning"
            );
          }
        );
      } else {
        e.stopPropagation();
        onGridScale(item.ID, 1);
      }
    });

    return $scaleBox.append($scaleDown).append($value).append($scaleUp);
  }

  function onGridScale(itemKey, scaleValue) {
    let lineItem = $listOrderedItems.find(itemKey);
    if (isValidJson(lineItem)) {
      if (lineItem.ItemType.toLowerCase() !== "service") {
        lineItem.PrintQty += scaleValue;
        lineItem.Qty += scaleValue;
      }
      if (__isScanQrcode) updateLineItemQR(itemKey, scaleValue);
      else {
        updateLineItem(itemKey, scaleValue);
      }
    }
  }

  //Panel accessibility
  this.gotoPanelGroupTable = function (callback) {
    __this.clearOrder2ndScreen();

    __this.readyPayment = false;
    //__this.clearCurrentOrder();
    $("#panel-group-tables").addClass("show");
    $("#panel-payment").removeClass("show");
    $("#panel-group-items").removeClass("show");
    //$("#panel-view-mode").addClass("hidden");

    if (typeof callback === "function") {
      callback.call(__this, __this.fallbackInfo());
    }
  };

  this.clearCurrentOrder = function () {
    __orderInfo.Order[__config.detail.fieldName] = [];
    $listOrderedItems.clearRows();
    resetGroupItems();
    bindOrderNumbers([]);

    let currencyBase = __orderInfo.DisplayPayOtherCurrency.filter(
      (i) => i.BaseCurrencyID === i.AltCurrencyID
    )[0] ?? { BaseCurrency: 0 };
    $("#summary-discount-rate").text("(" + __this.toCurrency(0) + "%)");
    $("#summary-discount").text(
      currencyBase.BaseCurrency +
        " " +
        __this.toCurrency(0, currencyBase?.DecimalPlaces)
    );
    $("#summary-vat").text(
      currencyBase.BaseCurrency +
        " " +
        __this.toCurrency(0, currencyBase?.DecimalPlaces)
    );
    $("#summary-vat-rate").text(__this.toCurrency(0) + " %");
    $("#summary-sub-total").text(
      currencyBase.BaseCurrency +
        " " +
        __this.toCurrency(0, currencyBase?.DecimalPlaces)
    );
    $("#total-qty").text(
      currencyBase.BaseCurrency +
        " " +
        __this.toCurrency(0, currencyBase?.DecimalPlaces)
    );
    $("#summary-total").text(
      currencyBase.BaseCurrency +
        " " +
        __this.toCurrency(0, currencyBase?.DecimalPlaces)
    );
    $("#count-items").text(0);
    __this.displayOrderTitle("#order-number");
    __this.displayTax();
  };

  this.gotoPanelItemOrder = function (callback) {
    if (isValidJson(__orderInfo.Order)) {
      __orderInfo.Order.GrandTotal -= __orderInfo.Order.FreightAmount;
    }
    __this.readyPayment = false;
    $("#panel-group-items").addClass("show");
    $("#panel-group-tables").removeClass("show");
    $("#panel-payment").removeClass("show");
    //$("#barcode-search-focus").addClass("show")
    //$("#btn-search.btn-search").css("display", "none")
    //$(".item-search-barcode").focus();
    //$("#panel-view-mode").removeClass("hidden");
    if (typeof callback === "function") {
      callback.call(__this, __this.fallbackInfo());
    }
  };

  this.gotoPanelPayment = function (callback) {
    if (isValidJson(__orderInfo.Order)) {
      __orderInfo.Order.GrandTotal += __orderInfo.Order.FreightAmount;
    }
    __this.readyPayment = true;
    $("#panel-view-mode").addClass("hidden");
    __this.checkCart(function () {
      __this.bindAdditionalCurrencies();
      __this.bindAdditionalCurrenciesReceived();
      __this.displayTotalFreight(__orderInfo.Freights);
      $("#panel-payment").addClass("show");
      $("#panel-group-tables").removeClass("show");
      $("#panel-group-items").removeClass("show");
      if (typeof callback === "function") {
        callback.call(__this, __this.fallbackInfo());
      }
    });
  };

  this.displayTotalFreight = function (freights) {
    if (isValidArray(freights)) {
      __orderInfo.Order.Freights = freights;
      let total = __this.sumFreights();
      //const baseCurrency = __orderInfo.DisplayPayOtherCurrency.filter(i => i.BaseCurrencyID == i.AltCurrencyID)[0] ?? { DecimalPlaces: 0 }
      $("#amount-freight").val(__this.toCurrency(total));
      __this.calculateOrder(__orderInfo.Order);
    }
  };

  this.diablePromotion = function (disabled = true) {
    if (disabled) {
      $("#loyalty-program").addClass("disabled");
    } else {
      $("#loyalty-program").removeClass("disabled");
    }
  };

  this.onPanelPayment = function (callback) {
    if (typeof callback === "function") {
      __fallbackPanelPayment = callback;
    }
  };

  //================old function=================
  this.voidOrder = function () {
    if (__orderInfo.Order.OrderID <= 0) {
      __this.dialog(
        "Void Order",
        "Please send before making void order!",
        "info"
      );
    } else {
      $.post(
        __urls.submitVoidOrder,
        {
          orderId: __orderInfo.Order.OrderID,
          reason: __orderInfo.Order.Reason,
        },
        function (status) {
          if (status === true) {
            __this.loadCurrentOrderInfo(__orderInfo.Order.TableID);
          } else {
            __this.dialog(
              "Void Order",
              "Please get authorization from administrator to cancel...!",
              "warning"
            );
          }
        }
      );
    }
  };

  //Utility functions
  this.sendOrder = function () {
    if (isBilling() && isNegativeQty()) {
      __this.dialog(
        "Bill Order",
        "Not allowed to send any billed order where its items have qty reduced."
      );
      return;
    }

    __orderInfo.Order.Status = __orderStatus.Sent;
    __this.submitOrder(__orderInfo.Order, "Send", function (posInfo) {
      if (__config.orderOnly) {
        __this.gotoPanelItemOrder();
      } else {
        __this.gotoPanelGroupTable();
      }
      __this.loadCurrentOrderInfo(
        __orderInfo.OrderTable.ID,
        __orderInfo.Order.OrderID
      );
    });
  };

  function getActiveFreightServiceCharge() {
    return __orderInfo.Freights.filter(
      (f) => f.FreightReceiptType == 0 && f.IsActive
    );
  }

  this.billOrder = function () {
    this.checkPrivilege(
      "P007",
      function (posInfo) {
        __orderInfo.Order.Status = __orderStatus.Bill;
        __orderInfo.Order.Freights = getActiveFreightServiceCharge();
        __this.sumFreights();
        __this.submitOrder(__orderInfo.Order, "Bill", function () {
          if (__config.orderOnly) {
            __this.gotoPanelItemOrder();
          } else {
            __this.gotoPanelGroupTable();
          }
          __this.loadCurrentOrderInfo(
            __orderInfo.OrderTable.ID,
            __orderInfo.Order.OrderID
          );
        });
      },
      function () {
        __orderInfo.Order.Status = __orderStatus.Sent;
      }
    );
  };

  function isBilling(order = {}) {
    if (isValidJson(order)) {
      return order.CheckBill === "Y";
    }
    return __orderInfo.Order.CheckBill === "Y";
  }

  this.payOrder = function (complete, reject) {
    this.checkPrivilege("P008", function () {
      __orderInfo.Order.Status = __orderStatus.Paid;
      checkCredit(complete, reject);
    });
  };

  function checkCredit(complete, reject) {
    if (
      __this.toNumber(__orderInfo.Order.GrandTotal) >
      __this.toNumber(__orderInfo.Order.Received)
    ) {
      var $dialog = __this
        .dialog(
          "Credit",
          "Payment is not enough. Do you want to credit?",
          "warning",
          "yes-no"
        )
        .confirm(function () {
          __orderInfo.Order.AppliedAmount = __orderInfo.Order.Received;
          __this.submitOrder(__orderInfo.Order, "Pay", complete);
          $dialog.shutdown();
        })
        .reject(function () {
          $dialog.shutdown();
          if (typeof reject === "function") {
            reject.call(__this, __this.fallbackInfo());
          }
        });
    } else {
      __this.submitOrder(__orderInfo.Order, "Pay", complete);
    }
    return $dialog;
  }

  this.moveOrders = function (fromTableId, toTableId, orders = []) {
    if (orders.length <= 0) {
      return;
    }
    $.post(
      __urls.moveOrders,
      {
        fromTableId: fromTableId,
        toTableId: toTableId,
        orders: orders,
      },
      function (_orderId) {
        __this.loadCurrentOrderInfo(toTableId, _orderId, 0);
      }
    );
  };

  this.changeTable = function (previousId, currentId, onAfterMove) {
    if (currentId > 0) {
      $.ajax({
        url: __urls.changeTable,
        type: "POST",
        data: { previousId: previousId, currentId: currentId },
        success: function (currentTable) {
          __orderInfo.OrderTable = currentTable;
          __this.loadCurrentOrderInfo(currentTable.ID, 0);
          __this.gotoPanelGroupTable(onAfterMove);
        },
      });
    } else {
      __this.dialog("Move Failed", "Please select item to move!", "warning");
    }
  };

  this.fallbackInfo = function (callback) {
    let info = __orderInfo;
    if (typeof callback === "function") {
      callback.call(this, info);
    }
    return info;
  };

  this.config = function () {
    return __config;
  };

  /// POS QRCode Order ////
  this.fetchOrderInfoQR = function (
    encryptedTableID,
    orderId,
    setDefaultOrder,
    onSucceed
  ) {
    __orderInfo.OrderDetail = {};
    __isScanQrcode = true;
    __encryptedTableID = encryptedTableID;
    __this.loadScreen();
    $listOrderedItems.clearRows();
    $("#panel-resizer").removeClass("hidden");
    $.get(
      `${__urls.fetchOrderInfoQR}/${encryptedTableID}/${orderId}/${setDefaultOrder}`,
      function (resp) {
        __orderInfo = resp;
        if (resp.Error) {
          __this.loadScreen(false);
          __this.dialog("Invalid URL", resp.Message, "warning");
        } else {
          if (!resp.Setting.IsOrderByQR) {
            $("#isOrderByQr").css("display", "none");
          }
          __this.setOrder(__orderInfo.Order);
          __this.updateOrder(resp.Order);
          if (resp.Order.TableID > 0) {
            bindOrderNumbersQR(__orderInfo.Orders, __orderInfo.Order.OrderID);
          } else {
            bindOrderNumbersQR([]);
          }
          if (typeof onSucceed === "function") {
            onSucceed.call(__this, __this.fallbackInfo());
          }
          __this.loadScreen(false);
        }
      }
    );
  };

  //Update item order details
  function updateLineItemQR(itemKey, scaleValue, isFromGrid = true) {
    let lineItem = $listOrderedItems.find(itemKey);
    if (lineItem) {
      if (isFromGrid) {
        lineItem.Total = __this.sumLineItem(lineItem);
        $listOrderedItems.updateRow(lineItem);
      } else {
        lineItem.PrintQty += scaleValue;
        $listOrderedItems.updateColumn(
          itemKey,
          "Total",
          __this.sumLineItem(lineItem)
        );
        $gridItemPaging.load(
          findMultiUomItems(lineItem.ItemID),
          itemKey,
          function (info) {
            $(__wrapGridItem).children().remove();
            $(__groupItemStep).children().removeClass("active");
            bindSingleItemGrids(info.dataset);
          }
        );
      }

      if (lineItem.PrintQty >= 0) {
        __orderInfo.Order[__config.detail.fieldName] =
          $listOrderedItems.yield();
      }
      updateItemGridView(lineItem);
    } else {
      lineItem = __this.createLineItem(itemKey);
      $listOrderedItems.updateRow(lineItem);
      __orderInfo.Order[__config.detail.fieldName] = $listOrderedItems.yield();
    }
    $listOrderedItems.updateColumn(
      lineItem.LineID,
      "PrintQty",
      lineItem.PrintQty
    );
    updateSubLineItem(lineItem, scaleValue);
    updateItemGridView(lineItem);
  }

  function filterGroupItemsQR(
    group1,
    group2,
    group3,
    priceListId,
    level,
    onlyAddon = false
  ) {
    __this.loadScreen();
    $(__notFound).removeClass("show");
    $.get(
      `${__urls.getItemGroupQR}/${group1}/${group2}/${group3}/${priceListId}/${level}/${onlyAddon}`,
      function (resp) {
        bindItemGrids(resp, resp[0], level, onlyAddon);
        __this.loadScreen(false);
      }
    );
  }

  function onClickGroupItemQR(
    group1,
    group2,
    group3,
    name,
    level,
    onlyAddon = false
  ) {
    //Find group from controler
    const { PriceListID } = __orderInfo.Order;
    filterGroupItemsQR(group1, group2, group3, PriceListID, level, onlyAddon);
    createGroupItemStepQR(group1, group2, group3, name, level);
  }

  function createGroupItemStepQR(group1, group2, group3, name, level) {
    $(__groupItemStep).append(
      $(
        '<li class="active"' +
          '" data-group1="' +
          group1 +
          '" data-group2="' +
          group2 +
          '" data-group3="' +
          group3 +
          '" data-level="' +
          level +
          '" >' +
          name +
          "</li>"
      ).on("click", function () {
        __orderInfo.OrderDetail = {};
        filterGroupItemsQR(
          group1,
          group2,
          group3,
          __orderInfo.Order.PriceListID,
          level
        );
        $(this).addClass("active").nextAll().remove();
      })
    );
    $(__groupItemStep).children(":not(:last-child)").removeClass("active");
  }

  var __isSubmittingQR = false;
  this.submitOrderQR = function (order, printType, succeeded) {
    if (!__isSubmittingQR) {
      __isSubmittingQR = true;
      __orderInfo.Order = __this.validateOrder(
        order,
        order[__config.detail.fieldName]
      );
      __orderInfo.Order.GrandTotalCurrencies =
        __orderInfo.DisplayPayOtherCurrency.filter(
          (i) => i.AltCurrencyID != i.BaseCurrencyID && i.IsShowCurrency
        );
      __orderInfo.Order.ChangeCurrencies =
        __orderInfo.Order.GrandTotalCurrencies;
      __orderInfo.Order.DisplayPayOtherCurrency =
        __orderInfo.DisplayPayOtherCurrency;
      if (!isValidArray(__orderInfo.Order.GrandTotalOtherCurrencies)) {
        __orderInfo.Order.GrandTotalOtherCurrencies =
          __orderInfo.DisplayGrandTotalOtherCurrency;
      }
      __this.checkCart(function () {
        if (parseFloat(__orderInfo.Order.GrandTotal) < 0) {
          __this.dialog(
            "Invalid Amount",
            "Total amount must be positive.",
            "warning"
          );
          return;
        }
        if (__orderInfo.Setting.AutoQueue) {
          __this.loadScreen();
          $.post(
            __urls.sendQR,
            { data: JSON.stringify(__orderInfo.Order), printType: printType },
            function (itemReturns) {
              if (itemReturns.Error) {
                __this.loadScreen(false);
                const orderPaid = __this.dialog(
                  "Order Paid",
                  itemReturns.Message,
                  "warning"
                );
                orderPaid.confirm(function () {
                  location.reload();
                });
              } else {
                if (itemReturns.length > 0) {
                  alertOutStock(itemReturns);
                } else {
                  if (typeof succeeded === "function") {
                    succeeded(__this.fallbackInfo());
                  }
                  __this.fetchOrderInfoQR(__encryptedTableID, 0, true);
                }
                __this.loadScreen(false);
                __isSubmittingQR = false;
              }
            }
          ).fail(function (err) {
            __isSubmittingQR = false;
          });
        }
      });
    }
  };

  //Utility functions
  this.sendOrderQR = function () {
    //connectionAlert.invoke("SendAlert", JSON.stringify(__orderInfo.Order));
    __this.submitOrderQR(
      __orderInfo.Order,
      "Send",
      function (posInfo) {
        __this.gotoPanelItemOrder();
      },
      function () {
        __this.dialog(
          "Send Order",
          "You cannot send order right now, please contact to staff or cashier.",
          "Warning"
        );
      }
    );
  };

  this.billOrderQR = function () {
    this.checkPrivilegeQR(
      "P007",
      function (posInfo) {
        __this.submitOrderQR(__orderInfo.Order, "Bill", function () {
          __this.gotoPanelItemOrder();
        });
      },
      function () {
        __this.dialog(
          "Bill Order",
          "You cannot bill right now, please contact to staff or cashier.",
          "Warning"
        );
      }
    );
  };

  this.mergeJson = function (json1, json2) {
    if (isValidJson(json1)) {
      if (isValidJson(json2)) {
        return $.extend(true, json1, json2);
      }
      return $.extend(true, {}, json1);
    }
  };

  this.checkPrivilegeQR = function (code, success, fail) {
    $.get(__urls.checkPrivilegeQR + code, function (cp) {
      if (cp.Privilege.Used) {
        if (typeof success === "function") {
          success(__orderInfo, cp.Privilege);
        }
      } else {
        if (typeof fail === "function") {
          fail(__this.fallbackInfo(), cp.Privilege);
        }
      }
    });
  };

  //Changed from bindItemOrder to bindOrderNumbers
  function bindOrderNumbersQR(orders, activeOrderId) {
    let $listBox = $(__listOrderReceipt);
    $listBox.find("#dropbox-order").children().remove();
    $listBox.find("#badge-order").text("");
    if (isValidArray(orders)) {
      __orderInfo.Orders = new Array();
      for (let i = 0; i < orders.length; i++) {
        __this.addNewOrder(orders[i], activeOrderId);
      }
    }
    $listBox
      .find("#add-new-order")
      .off("click")
      .on("click", function (e) {
        __this.resetOrderQR(__encryptedTableID);
        $listBox.find("#dropbox-order .option").removeClass("active");
      });
  }

  this.resetOrderQR = function (tableId, setDefaultOrder = false, onSuccess) {
    return __this.fetchOrderInfoQR(tableId, 0, setDefaultOrder, onSuccess);
  };

  this.deleteLineItemQR = function (item) {
    //Previlege for deleting item
    if (item.OrderDetailID == 0) {
      this.checkPrivilegeQR(
        "P011",
        function () {
          let dialogDelete = __this.dialog(
            "Delete item",
            "Are you sure you want to remove " +
              item.KhmerName +
              "(" +
              item.Code +
              ")?",
            "warning",
            "ok-cancel"
          );

          dialogDelete.confirm(function () {
            item.PrintQty = parseFloat(item.Qty) * -1;
            item.Qty = 0;
            updateLineItemQR(item.LineID, item.PrintQty);
            dialogDelete.shutdown();
          });

          dialogDelete.reject(function () {
            dialogDelete.shutdown();
          });
        },
        function () {
          __this.dialog(
            "Delete Item",
            "Please contact to staff or cashier to delete item.",
            "warning"
          );
        }
      );
    } else {
      __this.dialog(
        "Delete Item",
        "Please contact to staff or cashier to delete item.",
        "warning"
      );
    }
  };

  function addonLineItemQR(itemKey, relatedKey) {
    $.ajax({
      url: `${__urls.getNewOrderDetail}/${itemKey}/${__orderInfo.Order.OrderID}/0`,
      success: function (orderDetail) {
        let lineId = Date.now().toString();
        orderDetail.LineID = lineId;
        orderDetail.Qty = 1;
        orderDetail.PrintQty = 1;
        orderDetail.ParentLineID = relatedKey.toString();
        orderDetail.Total = __this.sumLineItem(orderDetail);
        __this.addSubLineItem(orderDetail);
      },
    });
  }
  /// End QRCode Order ///

  //Display additional total currencies and total change currencies
  this.bindAdditionalCurrencies = function (totalcus) {
    const additionalCurrenciesEl = $(".addition-currencies");
    const additionalCurrenciesElChange = $(".addition-currencies-change");
    const subParent = $(`<div id="remove-sub"></div>`);
    const subParentChange = $(`<div id="remove-sub-change"></div>`);
    $("#remove-sub").remove();
    $("#remove-sub-change").remove();
    if (isValidArray(__orderInfo.DisplayTotalAndChangeCurrency)) {
      for (var i of __orderInfo.DisplayTotalAndChangeCurrency) {
        const total = __orderInfo.Order.GrandTotal * i.AltRate;
        const totalChange = __orderInfo.Order.Change * i.AltRate;
        const totalCusTips = totalcus * i.AltRate;

        const stackEl = $(`
                        <div class="stack-inline">
                            <div class="widget-stackcrumb">
                                <div class="step"><i class="far fa-money-bill-alt"> Total</i></div>
                            </div>
                            <input id="pay-total-alt" readonly value="${__this.toCurrency(
                              total,
                              i.DecimalPlaces
                            )}">
                            <div class="symbol bg-grey">${i.AltCurrency}</div>
                        </div>`);
        const stackElChange = $(`
                    <div class="stack-inline">
                        <div class="widget-stackcrumb">
                            <div class="step"><i class="far fa-money-bill-alt"> Changed</i></div>
                        </div>
                        <input readonly class="error-change" value="${(
                          totalChange - totalCusTips
                        ).toFixed(i.DecimalPlaces)}">
                        <div class="symbol bg-grey">${i.AltCurrency}</div>
                    </div>
                `);
        subParent.append(stackEl);
        subParentChange.append(stackElChange);
      }
      additionalCurrenciesEl.append(subParent);
      additionalCurrenciesElChange.append(subParentChange);
    }
  };
  // Display additional total Currencies and total received currencies
  //Display additional total currencies and total change currencies
  this.bindAdditionalCurrenciesReceived = function () {
    const additionalCurrenciesEl = $(".addition-currencies");
    const additionalCurrenciesElReceived = $(".addition-currencies-received");
    const subParent = $(`<div id="remove-received"></div>`);
    const subParentReceived = $(`<div id="remove-sub-received"></div>`);
    $("#remove-sub").remove();
    $("#remove-sub-received").remove();
    if (isValidArray(__orderInfo.DisplayTotalAndReceiveCurrency)) {
      for (var i of __orderInfo.DisplayTotalAndReceiveCurrency) {
        const total = __orderInfo.Order.GrandTotal * i.AltRate;
        const totalReceived = __orderInfo.Order.Received * i.AltRate;
        const stackEl = $(`
                        <div class="stack-inline">
                            <div class="widget-stackcrumb">
                                <div class="step"><i class="far fa-money-bill-alt"> Total</i></div>
                            </div>
                            <input id="pay-total-alt" readonly value="${__this.toCurrency(
                              total,
                              i.DecimalPlaces
                            )}">
                            <div class="symbol bg-grey">${i.AltCurrency}</div>
                        </div>`);
        const stackElReceived = $(`
                    <div class="stack-inline">
                        <div class="widget-stackcrumb">
                            <div class="step"><i class="far fa-money-bill-alt"> Received</i></div>
                        </div>
                        <input readonly class="pay-received" value="${__this.toCurrency(
                          totalReceived,
                          i.DecimalPlaces
                        )}">
                        <div class="symbol bg-grey">${i.AltCurrency}</div>
                    </div>
                `);
        subParent.append(stackEl);
        subParentReceived.append(stackElReceived);
      }
      additionalCurrenciesEl.append(subParent);
      additionalCurrenciesElReceived.append(subParentReceived);
    }
  };

  function isEmpty(value, includeZero = false) {
    var isEmpty = value == undefined || value == null || value == "";
    if (includeZero) {
      isEmpty = isEmpty || value == 0 || isNaN(value);
    }
    return isEmpty;
  }

  function isValidJson(value) {
    return (
      !isEmpty(value) &&
      value.constructor === Object &&
      Object.keys(value).length > 0
    );
  }

  function isValidArray(values) {
    return Array.isArray(values) && values.length > 0;
  }

  function clone(prop, deep) {
    if (deep) {
      return $(__template[prop]).clone(deep);
    }
    return $(__template[prop]).clone();
  }

  this.findInArray = findInArray;
  function findInArray(keyName, keyValue, values) {
    if (isValidArray(values)) {
      return $.grep(values, function (item, i) {
        return item[keyName] == keyValue;
      })[0];
    }
  }

  this.roundNumber = function (value) {
    if (typeof value === "number") {
      return Math.round(value);
    }

    if (typeof __this.toNumber(value) === "number") {
      return Math.round(__this.toNumber(value));
    }
  };

  function getSizeInBytes(obj) {
    let str = null;
    if (typeof obj === "string") {
      // If obj is a string, then use it
      str = obj;
    } else {
      // Else, make obj into a string
      str = JSON.stringify(obj);
    }
    // Get the length of the Uint8Array
    const bytes = new TextEncoder().encode(str).length;
    return bytes;
  }

  function logSizeInBytes(description, obj) {
    const bytes = getSizeInBytes(obj);
    console.log(`${description} is approximately ${bytes} B`);
  }

  function isExpired(expiration) {
    // in milliseconds
    return expiration <= new Date().getTime();
  }

  function deleteCache(key) {
    if (window.caches) {
      window.caches.delete(key);
    }
  }

  async function findCache(key) {
    let expr = new Date();
    expr.setMinutes(expr.getMinutes() - 1);
    let cacheInfo = { expiration: expr.getTime(), data: {} };
    if (window.caches) {
      let currentCache = await window.caches.match(key);
      if (currentCache !== undefined && currentCache.status === 200) {
        cacheInfo = await currentCache.json();
      }
    }
    return cacheInfo;
  }

  async function updateCache(key, data = {}) {
    if ("caches" in window) {
      let expr = new Date();
      expr.setMinutes(expr.getMinutes() + 3); // in minute expired.
      let newCache = await window.caches.open(key);
      let info = {
        expiration: expr.getTime(),
        data: data,
      };
      newCache.put(key, new Response(JSON.stringify(info)));
    }
  }
}
// (function () {
//     var shouldHandleKeyDown = true;
//     document.onkeydown = function () {
//         if (!shouldHandleKeyDown) return;
//         shouldHandleKeyDown = false;
//         // HANDLE KEY DOWN HERE
//     }
//     document.onkeyup = function () {
//         shouldHandleKeyDown = true;
//     }
// })();
$(document).ready(function () {
  $(".number").asNumber();
  setInterval(function () {
    const now = new Date().toLocaleString().split(",");
    $(".datetime").text(now[0] + " " + now[1]);
  }, 1000);
  $("#loadscreen").hide();
});
