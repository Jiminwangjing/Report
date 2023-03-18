
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints;
using System;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;
using KEDI.Core.Premise.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.Services.POS.KSMS;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode;
using Microsoft.AspNetCore.Authorization;
using CKBS.Models.ServicesClass;
using KEDI.Core.Premise.Models.Services.POS.Templates;
using KEDI.Core.Premise.Models.Services.CardMembers;
using KEDI.Core.Helpers.Enumerations;
using CKBS.Models.Services.Inventory;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.POS.Template;
using CKBS.Models.Services.Account;
using KEDI.Core.Services.Authorization;

namespace KEDI.Core.Premise.ApiControllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    [Privilege(AuthenticationSchemes = "Bearer")]
    public class POSController : ControllerBase
    {
        private readonly UserManager _userModule;
        readonly PosRetailModule _posRetail;
        readonly LoyaltyProgramPosModule _loyaltyProgramPos;
        readonly IPOS _pos;
        readonly IBusinessPartner _partner;
        private readonly IPOSSerialBatchRepository _POSSerialBatch;
        private readonly DataContext _context;
        private readonly IKSMSRopository _kSMS;

        public POSController(UserManager userModule, IPOS pos, IBusinessPartner partner,
            PosRetailModule posRetail, LoyaltyProgramPosModule loyaltyProgramPos,
            IPOSSerialBatchRepository POSSerialBatch, IKSMSRopository kSMS,
            DataContext context)
        {
            _userModule = userModule;
            _posRetail = posRetail;
            _loyaltyProgramPos = loyaltyProgramPos;
            _pos = pos;
            _partner = partner;
            _POSSerialBatch = POSSerialBatch;
            _context = context;
            _kSMS = kSMS;
        }

        public Dictionary<int, string> TypeCardDiscountTypes => EnumHelper.ToDictionary(typeof(TypeCardDiscountType));

        [HttpPost("getUserSettingModel")]
        public async Task<ActionResult> GetUserSettingModel()
        {
            var settingModel = await _posRetail.GetSettingViewModelAsync(GetUserId());
            return Ok(settingModel);
        }

        [HttpPost("fetchOrderInfo/{tableId?}/{orderId?}/{customerId?}/{setDefaultOrder?}")]
        public async Task<IActionResult> FetchOrderInfo(int tableId, int orderId = 0, int customerId = 0, bool setDefaultOrder = false)
        {
            var orderInfo = await _posRetail.GetOrderInfoAsync(tableId, orderId, customerId, setDefaultOrder);
            return Ok(orderInfo);
        }

        [HttpPost("newLineItem/{saleItemId}/{orderId}")]
        public async Task<IActionResult> NewLineItem(int saleItemId, int orderId)
        {
            var lineItem = await _posRetail.NewOrderDetailAsync(saleItemId, orderId);
            return Ok(lineItem);
        }

        [HttpPost("getSeriesCM")]
        public List<SeriesInPurchasePoViewModel> GetSeriesCM()
        {
            var seriesCM = (from dt in _context.DocumentTypes.Where(i => i.Code == "CM")
                            join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                                DocumentTypeID = sr.DocuTypeID,
                                SeriesDetailID = _context.SeriesDetails.Where(i => i.SeriesID == sr.ID).FirstOrDefault() == null ? 0 :
                                _context.SeriesDetails.Where(i => i.SeriesID == sr.ID).FirstOrDefault().ID
                            }).ToList();
            return seriesCM;
        }

        [HttpPost]
        public IActionResult InitialStatusTable()
        {
            _pos.InitialStatusTable();
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult UpdateTimeOnTable()
        {
            _pos.UpdateTimeOnTable();
            return Ok();
        }

        private int GetUserId()
        {
            int userId = _userModule.GetUserId();
            return userId;
        }

        [HttpPost("getUserSetting")]
        public async Task<IActionResult> GetUserSetting(int userId = 0)
        {
            if (userId <= 0)
            {
                userId = GetUserId();
            }
            var setting = await _posRetail.GetUserSettingAsync(userId);
            return Ok(setting);
        }

        [HttpPost("checkPrivilege/{code}")]
        public IActionResult CheckPrivilege(string code)
        {
            int userId = GetUserId();
            bool valid = _posRetail.CheckPrivilege(userId, code);
            return Ok(valid);
        }

        [HttpPost("getUserByUserNamePassword")]
        public IActionResult GetUserAccess([FromBody] UserCredentials credentials)
        {
            bool verified = _posRetail.VerifyUserAcess(credentials);
            if (!verified)
            {
                ModelState.AddModelError("Username", "The user has no access permission.");
            }

            ModelMessage message = new(ModelState);
            if (ModelState.IsValid)
            {
                message.Add("Username", "Access granted.");
                message.Approve();
            }

            return Ok(message);
        }

        [HttpPost("searchTables/{keyword?}/{onlyFree?}")]
        public async Task<IActionResult> SearchTables(string keyword = "", bool onlyFree = false)
        {
            var tables = await _posRetail.SearchTables(GetUserId(), keyword, onlyFree);
            return Ok(tables);
        }

        [HttpPost("getServiceTables")]
        public async Task<IActionResult> GetServiceTables()
        {
            var tables = await _posRetail.GetServiceTableAsync(GetUserId());
            return Ok(tables);
        }

        [HttpPost("getOtherTables/{currentTableId}")]
        public async Task<IActionResult> GetOtherTables(int currentTableId)
        {
            var serviceTable = await _posRetail.GetServiceTableAsync(GetUserId());
            var otherTables = serviceTable.Tables.Where(t => t.ID != currentTableId);
            return Ok(otherTables);
        }

        [HttpPost("getFreeTables/{tableId}")]
        public async Task<IActionResult> GetFreeTables(int tableId)
        {
            var serviceTable = await _posRetail.GetServiceTableAsync(GetUserId());
            var freeTables = serviceTable.Tables.Where(t => t.ID != tableId && t.Status == 'A');
            return Ok(freeTables);
        }

        [HttpPost("getTablesByGroup/{groupId?}")]
        public IActionResult GetTablesByGroup(int groupId = 0)
        {
            return Ok(_posRetail.GetTablesByGroupId(groupId));
        }

        /// Serial Batch ///
        [HttpPost("checkSerailNumber/{serials}")]
        public IActionResult CheckSerailNumber(string serails)
        {
            List<SerialNumber> _serails = JsonConvert.DeserializeObject<List<SerialNumber>>(serails, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            for (var i = 0; i < _serails.Count; i++)
            {
                if (_serails[i].OpenQty > 0)
                {
                    ModelState.AddModelError(
                        $"OpenQty{i}",
                        $"Item name {_serails[i].ItemName} at line {i + 1} \"Total Selected\" is not enough. \"Total QTY\" is {_serails[i].Qty}, and \"Total Selected\" is {_serails[i].TotalSelected}!");
                    return Ok(msg.Bind(ModelState));
                }
            }
            msg.Approve();
            return Ok(msg.Bind(ModelState));
        }

        [HttpGet("getSerialDetials/{itemId}/{saleId}/{wareId}/{isReturnItem}")]
        public async Task<IActionResult> GetSerialDetials(int itemId, int saleId, int wareId, bool isReturnItem)
        {
            if (isReturnItem)
            {
                var data = await _POSSerialBatch.GetSerialDetialsReturnItemAsync(itemId, saleId, wareId);
                return Ok(data);
            }
            else
            {
                var data = await _POSSerialBatch.GetSerialDetialsAsync(itemId, wareId);
                return Ok(data);
            }
        }

        [HttpGet("GetSerialDetialsReturn/{itemId}/{saleId}/{wareId}/{isKsms}")]
        public async Task<IActionResult> GetSerialDetialsReturn(int itemId, int saleId, int wareId, bool isKsms)
        {
            if (isKsms)
            {
                var data = await _POSSerialBatch
                             .GetSerialDetialsReturnAsync(itemId, saleId, wareId, TransTypeWD.POSService);
                return Ok(data);
            }
            else
            {
                var data = await _POSSerialBatch
                             .GetSerialDetialsReturnAsync(itemId, saleId, wareId, TransTypeWD.POS);
                return Ok(data);
            }
        }

        [HttpGet("getBatchDetialsReturn/{itemId}/{uomId}/{saleId}/{wareId}/{isKsms}")]
        public async Task<IActionResult> GetBatchDetialsReturn(int itemId, int uomId, int saleId, int wareId, bool isKsms)
        {
            if (isKsms)
            {
                var data = await _POSSerialBatch
                             .GetBatchNoDetialsReturnAsync(itemId, uomId, saleId, wareId, TransTypeWD.POSService);
                return Ok(data);
            }
            else
            {
                var data = await _POSSerialBatch
                             .GetBatchNoDetialsReturnAsync(itemId, uomId, saleId, wareId, TransTypeWD.POS);
                return Ok(data);
            }
        }

        [HttpPost("checkBatchNo/{batches}")]
        public IActionResult CheckBatchNo(string batches)
        {
            List<BatchNo> _batches = JsonConvert.DeserializeObject<List<BatchNo>>(batches, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            for (var i = 0; i < _batches.Count; i++)
            {
                if (_batches[i].TotalNeeded > 0)
                {
                    ModelState.AddModelError(
                        $"OpenQty{i}",
                        $"Item name {_batches[i].ItemName} at line {i + 1} \"Total Selected\" is not enough. \"Total QTY\" is {_batches[i].Qty}, and \"Total Selected\" is {_batches[i].TotalSelected}!");
                    return Ok(msg.Bind(ModelState));
                }
            }
            msg.Approve();
            return Ok(msg.Bind(ModelState));
        }

        [HttpGet("getBatchNoDetials/{itemId}/{uomId}/{saleId}/{wareId}/{isReturnItem}")]
        public async Task<IActionResult> GetBatchNoDetials(int itemId, int uomID, int saleId, int wareId, bool isReturnItem)
        {
            if (isReturnItem)
            {
                var data = await _POSSerialBatch.GetBatchNoDetialsReturnItemAsync(itemId, uomID, saleId, wareId);
                return Ok(data);
            }
            else
            {
                var data = await _POSSerialBatch.GetBatchNoDetialsAsync(itemId, uomID, wareId);
                return Ok(data);
            }
        }

        /// End Serial Batch ///
        [HttpPost("submitOrder/{printType?}/{batch?}/{serial?}/{promocode?}")]
        public async Task<IActionResult> SubmitOrder([FromBody] Order order, string printType = "Send", string batch = "[]", string serial = "[]", string promocode = "")
        {
            var _order = order;
            // Checking Serial Batch //
            List<SerialNumber> _serialNumber = new();
            List<SerialNumber> serialNumber = new();
            List<BatchNo> _batchNoes = new();
            List<BatchNo> batchNoes = new();
            List<ItemsReturn> list = new();
            List<ItemsReturn> list_group = new();
            var orderDetails = _order.OrderDetail.Where(i => !i.IsKsms && !i.IsKsmsMaster).ToList();
            var sbReturn = (from od in orderDetails
                            join item in _context.ItemMasterDatas on od.ItemID equals item.ID
                            join ws in _context.WarehouseSummary on item.ID equals ws.ItemID
                            join uom in _context.UnitofMeasures on item.InventoryUoMID equals uom.ID
                            group new { od, item, ws, uom } by new { item.ID } into g
                            let data = g.FirstOrDefault()
                            let uom_defined = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == data.item.GroupUomID && w.AltUOM == data.od.UomID) ?? new GroupDUoM()
                            let instock = (decimal)g.Where(i => i.ws.WarehouseID == _order.WarehouseID).Sum(i => i.ws.InStock)
                            select new ItemsReturn
                            {
                                Code = data.item.Code,
                                Committed = (decimal)data.ws.Committed,
                                ItemID = data.item.ID,
                                InStock = instock,
                                TotalStock = instock - (decimal)(data.od.PrintQty * uom_defined.Factor),
                                KhmerName = $"{data.item.KhmerName} ({data.uom.Name})",
                                LineID = data.od.LineID,
                                Uom = data.uom.Name,
                                OrderQty = (decimal)(data.od.PrintQty * uom_defined.Factor),//(decimal)data.od.Qty,
                                IsSerailBatch = data.item.ManItemBy == ManageItemBy.Batches || data.item.ManItemBy == ManageItemBy.SerialNumbers
                            }).ToList();
            var wh = _context.Warehouses.Find(_order.WarehouseID) ?? new Warehouse();
            foreach (var item in orderDetails)
            {
                var check = list_group.Find(w => w.ItemID == item.ItemID);
                var item_group_uom = _context.ItemMasterDatas.Include(gu => gu.GroupUOM).Include(uom => uom.UnitofMeasureInv).FirstOrDefault(w => w.ID == item.ItemID);
                var uom_defined = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_group_uom.GroupUomID && w.AltUOM == item.UomID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true);
                var item_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.NegativeStock == false && w.Detele == false)
                                     join i in _context.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                     join gd in _context.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                     join uom in _context.UnitofMeasures on i.InventoryUoMID equals uom.ID
                                     select new
                                     {
                                         bomd.ItemID,
                                         i.Code,
                                         i.KhmerName,
                                         Uom = uom.Name,
                                         gd.Factor,
                                         gd.GroupUoMID,
                                         GUoMID = i.GroupUomID,
                                         bomd.Qty,
                                         i.Process,
                                     }).Where(w => w.GroupUoMID == w.GUoMID);
                if (check == null)
                {
                    if (bom != null)
                    {
                        foreach (var items in item_material.ToList())
                        {
                            var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == _order.WarehouseID && w.ItemID == items.ItemID);
                            if (item_warehouse_material != null)
                            {
                                ItemsReturn item_group = new()
                                {
                                    LineID = item.LineID,
                                    Code = items.Code,
                                    ItemID = items.ItemID,
                                    KhmerName = items.KhmerName + " (" + items.Uom + ")",
                                    InStock = (decimal)item_warehouse_material.InStock,
                                    TotalStock = (decimal)item_warehouse_material.InStock - (decimal)(item.PrintQty * uom_defined.Factor * items.Qty * items.Factor),
                                    OrderQty = (decimal)(item.PrintQty * uom_defined.Factor * items.Qty * items.Factor),
                                    Committed = (decimal)item_warehouse_material.Committed,
                                    IsBOM = true,
                                    Uom = ""
                                };
                                list_group.Add(item_group);
                            }
                            else if (items.Process != "Standard" && item_warehouse_material == null)
                            {
                                ItemsReturn item_group = new()
                                {
                                    LineID = item.LineID,
                                    Code = item.Code,
                                    ItemID = item.ItemID,
                                    KhmerName = item_group_uom.KhmerName + " (" + item_group_uom.UnitofMeasureInv.Name + ")",
                                    InStock = 0,
                                    OrderQty = (decimal)(item.PrintQty * uom_defined.Factor),
                                    Committed = 0,
                                    IsBOM = true,
                                    Uom = "",
                                };
                                list_group.Add(item_group);
                            }
                        }
                    }
                }
                else
                {
                    check.OrderQty += (decimal)(item.PrintQty * uom_defined.Factor);
                }
            }

            foreach (var item in list_group)
            {
                if (wh.IsAllowNegativeStock)
                {
                    if (item.IsSerailBatch)
                    {
                        ItemsReturn item_return = new()
                        {
                            LineID = item.LineID,
                            Code = item.Code,
                            ItemID = item.ItemID,
                            KhmerName = item.KhmerName,
                            InStock = item.InStock,
                            OrderQty = item.OrderQty,
                            Committed = item.Committed,
                            IsBOM = item.IsBOM,
                            Uom = item.Uom
                        };
                        list.Add(item_return);
                    }
                }
                else
                {
                    if (item.OrderQty > item.InStock)
                    {
                        ItemsReturn item_return = new()
                        {
                            LineID = item.LineID,
                            Code = item.Code,
                            ItemID = item.ItemID,
                            KhmerName = item.KhmerName,
                            InStock = item.InStock,
                            OrderQty = item.OrderQty,
                            Committed = item.Committed,
                            IsBOM = item.IsBOM,
                            Uom = item.Uom
                        };
                        list.Add(item_return);
                    }
                }
            }
            List<ItemsReturn> itemsReturn = new(list.Count + sbReturn.Count);
            itemsReturn.AddRange(list);
            itemsReturn.AddRange(sbReturn);
            List<ItemsReturn> itemSBOnly = itemsReturn.Where(i => i.IsSerailBatch && i.TotalStock > 0 && !i.IsBOM).ToList();
            if (wh.IsAllowNegativeStock)
            {
                itemsReturn = itemsReturn.Where(i => i.IsSerailBatch && i.TotalStock < 0 && !i.IsBOM).ToList();
            }
            else
            {
                itemsReturn = itemsReturn.Where(i => i.TotalStock < 0 && !i.IsBOM).ToList();
            }

            if (itemsReturn.Count > 0)
            {
                ItemsReturnObj __dataItemTurnOutOfStokes = new()
                {
                    ItemsReturns = itemsReturn
                };
                return Ok(__dataItemTurnOutOfStokes);
            }

            if (orderDetails.Count > 0 && itemSBOnly.Count > 0)
            {
                if (printType == "Pay")
                {
                    serialNumber = serial != null ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }) : serialNumber;

                    _POSSerialBatch.CheckItemSerail(_order, orderDetails, serialNumber, "OrderID");
                    serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in serialNumber.ToList())
                    {
                        foreach (var i in orderDetails)
                        {
                            if (j.ItemID == i.ItemID)
                            {
                                _serialNumber.Add(j);
                            }
                        }
                    }

                    bool isHasSerialItem = _serialNumber.Any(i => i.OpenQty != 0);
                    if (isHasSerialItem)
                    {
                        return Ok(new { IsSerail = true, Data = _serialNumber });
                    }

                    batchNoes = batch != null ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }) : batchNoes;
                    _POSSerialBatch.CheckItemBatch(_order, orderDetails, batchNoes, "OrderID");
                    batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in batchNoes.ToList())
                    {
                        foreach (var i in orderDetails)
                        {
                            if (j.ItemID == i.ItemID)
                            {
                                _batchNoes.Add(j);
                            }
                        }
                    }
                    bool isHasBatchItem = _batchNoes.Any(i => i.TotalNeeded != 0);
                    if (isHasBatchItem)
                    {
                        return Ok(new { IsBatch = true, Data = _batchNoes });
                    }
                }
            }

            _order.UserOrderID = GetUserId();
            var itemReturns = await _posRetail.SubmitOrderAsync(_order, printType, _serialNumber, _batchNoes, promocode);
            itemReturns.ItemsReturns = itemsReturn;
            itemReturns.PrintInvoice = _posRetail.GetPrintInvoice(_order, printType);
            return Ok(itemReturns);
        }

        [HttpPost("findLineItemByBarcode/{orderId}/{pricelistId}/{barcode}")]
        public async Task<IActionResult> FindLineItemByBarcode(int orderId, int pricelistId, string barcode)
        {
            var lineItem = await _posRetail.FindItemByBarcodeAsync(orderId, pricelistId, GetCompany().ID, barcode);
            return Ok(lineItem);
        }

        [HttpPost("searchSaleItems/{orderId}/{keyword?}")]
        public async Task<IActionResult> SearchSaleItems(int orderId, string keyword = "")
        {
            var saleItems = await _posRetail.GetSaleItemsAsync(GetUserId(), orderId, keyword);
            return Ok(saleItems);
        }

        [HttpPost("getGroupItems/{group1}/{group2}/{group3}/{priceListId}/{level}/{onlyAddon}")]
        public async Task<IActionResult> GetGroupItems(int group1, int group2, int group3, int priceListId, int level = 0, bool onlyAddon = false)
        {
            var saleItems = await _posRetail.GetGroupItemsAsync(group1, group2, group3, priceListId, GetCompany().ID, level, onlyAddon);
            return Ok(saleItems);
        }

        [HttpPost("saveItemComment")]
        public async Task<IActionResult> SaveItemComment([FromBody] ItemComment comment)
        {
            var itemComment = await _posRetail.SaveItemCommentAsync(comment, ModelState);
            ModelMessage modelMessage = new(ModelState);
            if (ModelState.IsValid)
            {
                modelMessage.Approve();
            }
            return Ok(new { Comment = itemComment, Message = modelMessage });
        }

        [HttpPost("deleteItemComment/{commentId}")]
        public async Task<IActionResult> DeleteItemComment(int commentId)
        {
            var itemComment = await _posRetail.DeleteItemCommentAsync(commentId, ModelState);
            ModelMessage modelMessage = new(ModelState);
            if (ModelState.IsValid)
            {
                modelMessage.Approve();
            }
            return Ok(new { Comment = itemComment, Message = modelMessage });
        }

        [HttpPost("getItemComments")]
        public async Task<IActionResult> GetItemComments()
        {
            var comments = await _posRetail.GetItemCommentsAsync();
            return Ok(comments);
        }

        [HttpPost("updateSetting")]
        public async Task<IActionResult> UpdateSetting([FromBody] GeneralSetting setting)
        {
            await _posRetail.UpdateSettingAsync(setting);
            return Ok(setting);
        }

        [HttpPost("checkOpenShift/{userId?}")]
        public IActionResult CheckOpenShift(int userId = 0)
        {
            if (userId <= 0)
            {
                userId = GetUserId();
            }

            var hasOpenShift = _posRetail.CheckOpenShift(userId);
            return Ok(hasOpenShift);
        }

        [HttpPost("getShiftTemplate")]
        public async Task<IActionResult> GetShiftTemplate()
        {
            var shiftTemplate = await _posRetail.CreateShiftTemplateAsync(GetUserId());
            return Ok(shiftTemplate);
        }

        [HttpPost("voidOrder/{orderId}/{reason}")]
        public async Task<IActionResult> VoidOrder(int orderId, string reason)
        {
            var isVoidSuccess = await _posRetail.VoidOrderAsync(orderId, reason);
            return Ok(isVoidSuccess);
        }

        [HttpPost("moveOrder/{fromTableId}/{toTableId}/{orderId}")]
        public async Task<IActionResult> MoveReceipt(int fromTableId, int toTableId, int orderId)
        {
            var _orderId = _pos.MoveReceipt(fromTableId, toTableId, orderId);
            return Ok(await Task.FromResult(_orderId));
        }

        [HttpPost("changeTable/{fromTableId}/{toTableId}")]
        public async Task<IActionResult> ChangeTable(int fromTableId, int toTableId)
        {
            var currentTable = await _posRetail.ChangeTableAsync(fromTableId, toTableId);
            return Ok(currentTable);
        }

        [HttpPost("saveOrder/{order}")]
        public IActionResult SaveOrder([FromBody] Order order)
        {
            var _order = order;
            //var _order = JsonConvert.DeserializeObject<Order>(order,
            //    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _posRetail.SaveOrder(_order);
            return Ok(_order);
        }

        [HttpPost("deleteSavedOrder/{orderId}")]
        public IActionResult DeleteSavedOrder(int orderId)
        {
            ModelMessage message = new();
            _posRetail.DeleteSavedOrder(orderId, ModelState);
            if (ModelState.IsValid)
            {
                message.Approve();
            }
            return Ok(message);
        }

        [HttpPost("getChangeRateTemplate/{orderId}")]
        public async Task<IActionResult> GetChangeRateTemplate(int orderId = 0)
        {
            var changeRate = await _posRetail.CreateChangeRateTemplate(GetUserId(), orderId);
            return Ok(changeRate);
        }

        [HttpPost("saveDisplayCurrencies")]
        public IActionResult SaveDisplayCurrencies([FromBody] List<DisplayCurrency> displayCurrencies)
        {
            ModelMessage message = new();
            if (ModelState.IsValid)
            {
                _posRetail.SaveDisplayCurrencies(displayCurrencies, ModelState);
                message.Approve();
                message.Add("DisplayCurrency", "Data has been saved successfully.");
            }
            return Ok(message.Bind(ModelState));
        }

        [HttpPost("getCustomers/{keyword?}")]
        public async Task<IActionResult> GetCustomers(string keyword = "")
        {
            var customers = await _posRetail.GetCustomersAsyc(keyword);
            return Ok(customers);
        }

        [HttpPost("getMemberCards/{keyword?}")]
        public async Task<IActionResult> GetMemberCards(string keyword = "")
        {
            var membercards = await _loyaltyProgramPos.GetMemberCardsAsync(keyword);
            return Ok(membercards);
        }

        [HttpPost("getOrderDetailUnknown/{orderId}")]
        public async Task<IActionResult> GetOrderDetailUnknown(int orderId)
        {
            var unknownItem = await _posRetail.GetOrderDetailUnknownAsync(GetUserId(), orderId, GetCompany().ID);
            return Ok(unknownItem);
        }

        [HttpPost("getOrdersToCombine/{orderId}")]
        public async Task<IActionResult> GetOrdersToCombine(int orderId)
        {
            var orders = await _posRetail.GetOrdersToCombineAsync(orderId);
            return Ok(orders);
        }

        [HttpPost("combineOrders")]
        public async Task<IActionResult> CombineOrders([FromBody] CombineOrder combineOrder)
        {
            using var t = _context.Database.BeginTransaction();
            await _posRetail.CombineOrdersAsync(combineOrder);
            t.Commit();
            return Ok(combineOrder);
        }

        [HttpPost("splitOrder")]
        public async Task<IActionResult> SplitOrder([FromBody] Order splitOrder)
        {
            Order newOrder = await _posRetail.SplitOrderAsync(splitOrder);
            return Ok(newOrder);
        }

        [HttpPost("getReturnReceipts/{dateFrom?}/{dateTo?}/{keyword?}")]
        public async Task<IActionResult> GetReturnReceipts(string dateFrom = "", string dateTo = "", string keyword = "")
        {
            var receipts = await _posRetail.GetReturnReceiptsAsync(GetUserId(), dateFrom, dateTo, keyword);
            return Ok(receipts);
        }

        [HttpPost("getReturnItems/{receiptId}")]
        public async Task<IActionResult> GetReturnItems(int receiptId)
        {
            var returnedItems = await _posRetail.GetReturnItemsAsync(GetUserId(), receiptId);
            return Ok(returnedItems);
        }

        [HttpPost("sendReturnItems/{serial?}/{batch?}")]
        public async Task<IActionResult> SendReturnItems([FromBody] List<ReturnItem> returnItems, string serial, string batch)
        {
            #region
            var _order = _context.Receipt.Find(returnItems.FirstOrDefault()?.ReceiptID);
            var orderDetail = _context.ReceiptDetail.Where(i => i.ReceiptID == _order.ReceiptID).ToList();
            foreach (var i in orderDetail.ToList())
            {
                foreach (var j in returnItems.ToList())
                {
                    if (i.ItemID != j.ItemID)
                        orderDetail.Remove(i);
                }
            }
            // Checking Serial Batch //
            List<SerialNumber> _serialNumber = new();
            List<SerialNumber> serialNumber = new();
            List<BatchNo> _batchNoes = new();
            List<BatchNo> batchNoes = new();
            serialNumber = serial != null ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }) : serialNumber;

            _POSSerialBatch.CheckItemSerail(_order, orderDetail, serialNumber, "ReceiptID");
            serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
            foreach (var j in serialNumber.ToList())
            {
                foreach (var i in orderDetail)
                {
                    if (j.ItemID == i.ItemID)
                    {
                        _serialNumber.Add(j);
                    }
                }
            }
            bool isHasSerialItem = _serialNumber.Any(i => i.OpenQty != 0);
            if (isHasSerialItem)
            {
                return Ok(new { IsSerail = true, Data = _serialNumber });
            }
            batchNoes = batch != null ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }) : batchNoes;
            _POSSerialBatch.CheckItemBatch(_order, orderDetail, batchNoes, "ReceiptID");
            batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
            foreach (var j in batchNoes.ToList())
            {
                foreach (var i in orderDetail)
                {
                    if (j.ItemID == i.ItemID)
                    {
                        _batchNoes.Add(j);
                    }
                }
            }
            bool isHasBatchItem = _batchNoes.Any(i => i.TotalNeeded != 0);
            if (isHasBatchItem)
            {
                return Ok(new { IsBatch = true, Data = _batchNoes });
            }
            #endregion
            await _posRetail.ReturnReceiptsAsync(returnItems, ModelState, _serialNumber, _batchNoes);
            var message = new ModelMessage(ModelState);
            if (ModelState.IsValid)
            {
                message.Add("ItemReturns", "Items returned successfully...");
                message.Approve();
            }
            return Ok(message);
        }

        [HttpPost("getReceiptsToCancel/{dateFrom?}/{dateTo?}/{keyword?}")]
        public async Task<IActionResult> GetReceiptsToCancel(string dateFrom, string dateTo, string keyword = "")
        {
            var receipts = await _posRetail.GetReceiptsToCancelAsync(GetUserId(), dateFrom, dateTo, keyword);
            return Ok(receipts);
        }

        [HttpPost("cancelReceipt/{receiptId}/{serial?}/{batch?}/{checkingSerialString?}/{checkingBatchString?}")]
        public async Task<IActionResult> CancelReceipt(int receiptId, string serial = "[]", string batch = "[]",
            string checkingSerialString = "unchecked", string checkingBatchString = "unchecked")
        {
            #region
            ModelMessage message = new();
            var receipt = _context.Receipt.FirstOrDefault(w => w.ReceiptID == receiptId);
            var receiptDetial = _context.ReceiptDetail.Where(i => i.ReceiptID == receipt.ReceiptID).ToList();
            // Checking Serial Batch
            List<SerialNumber> _serialNumber = new();
            List<SerialNumber> serialNumber = new();
            List<BatchNo> _batchNoes = new();
            List<BatchNo> batchNoes = new();
            serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }) : serialNumber;
            _POSSerialBatch.CheckItemSerailReturn(receipt, receiptDetial.ToList(), serialNumber, "In");
            serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
            foreach (var j in serialNumber.ToList())
            {
                foreach (var i in receiptDetial.ToList())
                {
                    if (j.ItemID == i.ItemID)
                    {
                        _serialNumber.Add(j);
                    }
                }
            }
            if (checkingSerialString != "checked" && checkingSerialString == "unchecked" && _serialNumber.Count > 0)
            {
                return Ok(new { IsSerial = true, Data = _serialNumber });
            }
            //SerialNumberSelected = null
            batchNoes = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }) : batchNoes;
            _POSSerialBatch.CheckItemBatchReturn(receipt, receiptDetial.ToList(), batchNoes, "In", TransTypeWD.POS);
            batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
            foreach (var j in batchNoes.ToList())
            {
                foreach (var i in receiptDetial.ToList())
                {
                    if (j.ItemID == i.ItemID)
                    {
                        _batchNoes.Add(j);
                    }
                }
            }
            if (checkingBatchString != "checked" && checkingBatchString == "unchecked" && _batchNoes.Count > 0)
            {
                return Ok(new { IsBatch = true, Data = _batchNoes });
            }
            #endregion
            await _posRetail.CancelReceiptAsync(receipt, ModelState, _serialNumber, _batchNoes);
            if (ModelState.IsValid)
            {
                message.Add("Receipt", $"The specified receipt is cancelled successfully.");
                message.Approve();
            }
            return Ok(message);
        }

        [HttpPost("getReceiptsToReprint/{dateFrom}/{dateTo}/{keyword?}")]
        public async Task<IActionResult> GetReceiptsToReprint(string dateFrom, string dateTo, string keyword = "")
        {
            var receipts = await _posRetail.GetReceiptsToReprintAsync(GetUserId(), dateFrom, dateTo, keyword);
            return Ok(receipts);
        }

        [HttpPost("getPendingVoidItem/{dateFrom}/{dateTo}/{keyword?}")]
        public async Task<IActionResult> GetPendingVoidItem(string dateFrom, string dateTo, string keyword = "")
        {
            _ = DateTime.TryParse(dateTo, out DateTime _toDate);
            _ = DateTime.TryParse(dateFrom, out DateTime _fromDate);
            var receipts = await _posRetail.GetPendingVoidItemAsync(GetUserId(), _fromDate, _toDate, keyword);
            return Ok(receipts);
        }

        [HttpPost("submitPendingVoidItem/{reason}")]
        public IActionResult SubmitPendingVoidItem([FromBody] List<PendingVoidItemModel> voidItems, string reason)
        {
            var status = _posRetail.InsertPendingVoidItemToVoidItem(voidItems, reason);
            return Ok(status);
        }

        [HttpPost("reprintReceipt/{receiptId}/{printType?}")]
        public async Task<IActionResult> ReprintReceipt(int receiptId, string printType = "Pay")
        {
            var printBills = await _posRetail.ReprintReceiptAsync(GetUserId(), receiptId, printType, true);
            return Ok(printBills);
        }

        [HttpPost("reprintReceiptCloseShifts/{userid}/{closeShiftId}")]
        public async Task<IActionResult> ReprintReceiptcloseshift(int userid, int closeShiftId)
        {
            var cashoutReports = await _posRetail.ReprintReceiptCloseShiftAsync(userid, closeShiftId, false);
            return Ok(cashoutReports);
        }

        [HttpPost("getReprintCloseShifts/{dateFrom}/{dateTo}/{keyword?}")]
        public IActionResult ReprintCloseShift(string DateFrom, string DateTo, string keyword = "")
        {
            var data = _posRetail.GetReprintCloseShifts(DateFrom, DateTo, keyword);
            return Ok(data);
        }

        [HttpPost("processOpenShift/{total}")]
        public async Task<IActionResult> ProcessOpenShift(double total)
        {
            ModelMessage message = new();
            if (ModelState.IsValid)
            {
                await _posRetail.OpenShiftAsync(GetUserId(), total);
                message.Approve();
                return Ok(message.Bind(ModelState));
            }
            return Ok(message.Bind(ModelState));
        }

        [HttpPost("processCloseShift/{total}")]
        public async Task<IActionResult> ProcessCloseShift(double total)
        {
            ModelMessage message = new();
            if (ModelState.IsValid)
            {
                var closeShift = await _posRetail.CloseShiftAsync(GetUserId(), total, false);
                await _posRetail.VoidItemAsync(GetUserId());
                message.Approve();
                message.AddItem(closeShift, "CloseShift");
            }

            return Ok(message.Bind(ModelState));
        }

        [HttpPost("getBuyXGetXDetails/{priceListId}")]
        public async Task<IActionResult> GetBuyXGetXDetails(int priceListId)
        {
            var buyxGetxs = await _loyaltyProgramPos.GetBuyXGetXDetailsAsync(priceListId);
            return Ok(buyxGetxs);
        }

        [HttpPost("saveCustomer")]
        public async Task<IActionResult> SaveCustomer([FromBody] BusinessPartner customer)
        {
            var _customer = await _partner.SaveCustomerAsync(customer, ModelState);
            ModelMessage message = new(ModelState);
            if (ModelState.IsValid)
            {
                message.Add("__success", "Save completed...");
                message.AddItem(_customer, "Customer");
                message.Approve();
            }
            return Ok(message);
        }

        [HttpPost("voidItem")]
        public async Task<IActionResult> VoidItem([FromBody] Order order)
        {
            var voiditem = await _posRetail.VoidItemsAsync(order);
            return Ok(voiditem);
        }

        [HttpPost("pendingVoidItem")]
        public async Task<IActionResult> PendingVoidItem([FromBody] Order order)
        {
            var voiditem = await _posRetail.PendingVoidItemsAsync(order);
            return Ok(voiditem);
        }

        [HttpPost("deletePendingVoidItem/{id}")]
        public IActionResult DeletePendingVoidItem(int id)
        {
            var msg = _posRetail.DeletePendingVoidItem(id);
            return Ok(msg);
        }

        //Point Redemption
        [HttpPost("fetchLoyaltyProgram/{priceListId}")]
        public async Task<IActionResult> FetchLoyaltyProgram(int priceListId)
        {
            var loyProg = new LoyaltyProgModel
            {
                BuyXGetXDetails = await _loyaltyProgramPos.GetBuyXGetXDetailsAsync(priceListId),
                PointMembers = await _loyaltyProgramPos.GetAvailablePointMembersAsync()
            };
            return Ok(loyProg);
        }

        [HttpPost("getPointRedemptionWarehouse/{customerId}/{warehouseId}")]
        public async Task<IActionResult> GetPointRedemptionWarehouse(int customerId, int warehouseId)
        {
            var pointRedempts = await _loyaltyProgramPos.GetPointRedemptionWarehouseAsync(customerId, warehouseId);
            return Ok(pointRedempts);
        }

        [HttpPost("postPointRedemptions/{customerId}/{point}/{batch}/{serial}")]
        public async Task<IActionResult> PostPointRedemptions(int customerId, string point, string batch, string serial)
        {
            PointRedemptionMaster points = JsonConvert.DeserializeObject<PointRedemptionMaster>(point, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            SeriesDetail _seriesDetail = new();
            ModelMessage msg = new();
            var douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "RD");
            if (douType == null)
            {
                ModelState.AddModelError("DocumentType", "Document Type is required!");
                return Ok(msg.Bind(ModelState));
            }
            var Series = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douType.ID && w.NextNo != w.LastNo);
            if (Series == null)
            {
                ModelState.AddModelError("Series", "Series Number is required!");
                return Ok(msg.Bind(ModelState));
            }
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;

            // Checking Serial Batch //
            List<SerialNumber> serialNumber = new();
            List<BatchNo> batchNoes = new();
            serialNumber = serial != null ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }) : serialNumber;
            foreach (var i in points.PointRedemptions)
            {
                _POSSerialBatch.CheckItemSerail(i, i.PointItems, serialNumber, "ID");
            }
            int latestSaleSNID = serialNumber.Count == 0 ? 0 : serialNumber.Max(i => i.SaleID);
            serialNumber = serialNumber.Where(i => i.SaleID == latestSaleSNID).ToList();
            serialNumber = (from s in serialNumber
                            group s by s.ItemID into i
                            let sn = i.FirstOrDefault()
                            select new SerialNumber
                            {
                                BpId = sn.BpId,
                                OpenQty = i.Sum(sum => sum.OpenQty) < 0 ? 0 : i.Sum(sum => sum.OpenQty),
                                Cost = sn.Cost,
                                ItemName = sn.ItemName,
                                ItemID = sn.ItemID,
                                Direction = sn.Direction,
                                ItemCode = sn.ItemCode,
                                LineID = sn.LineID,
                                Qty = i.Sum(sum => sum.Qty),
                                SaleID = sn.SaleID,
                                SerialNumberSelected = sn.SerialNumberSelected,
                                SerialNumberUnselected = sn.SerialNumberUnselected,
                                TotalSelected = i.Sum(sum => sum.TotalSelected),
                                UomID = sn.UomID,
                                WhsCode = sn.WhsCode,
                                BaseQty = sn.BaseQty,
                            }).ToList();
            bool isHasSerialItem = serialNumber.Any(i => i.OpenQty != 0);
            if (isHasSerialItem)
            {
                return Ok(new { IsSerail = true, Data = serialNumber });
            }

            batchNoes = batch != null ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }) : batchNoes;

            foreach (var i in points.PointRedemptions)
            {
                _POSSerialBatch.CheckItemBatch(i, i.PointItems, batchNoes, "ID");
            }
            int latestSaleBNID = batchNoes.Count == 0 ? 0 : batchNoes.Max(i => i.SaleID);
            batchNoes = batchNoes.Where(i => i.SaleID == latestSaleBNID).ToList();
            batchNoes = (from b in batchNoes
                         group b by b.ItemID into i
                         let bn = i.FirstOrDefault()
                         let uom = _context.GroupDUoMs.FirstOrDefault(i => i.UoMID == bn.UomID) ?? new GroupDUoM()
                         select new BatchNo
                         {
                             UomID = uom.BaseUOM,
                             BaseQty = bn.BaseQty,
                             BatchNoSelected = bn.BatchNoSelected,
                             BatchNoUnselect = bn.BatchNoUnselect,
                             BpId = bn.BpId,
                             Cost = bn.Cost,
                             Direction = bn.Direction,
                             ItemCode = bn.ItemCode,
                             ItemID = bn.ItemID,
                             ItemName = bn.ItemName,
                             LineID = bn.LineID,
                             Qty = i.Sum(sum => sum.Qty),
                             SaleID = bn.SaleID,
                             TotalBatches = bn.TotalBatches,
                             TotalNeeded = i.Sum(sum => sum.TotalNeeded) < 0 ? 0 : i.Sum(sum => sum.TotalNeeded),
                             TotalSelected = i.Sum(sum => sum.TotalSelected),
                             WhsCode = bn.WhsCode
                         }).ToList();
            bool isHasBatchItem = batchNoes.Any(i => i.TotalNeeded != 0);
            if (isHasBatchItem)
            {
                return Ok(new { IsBatch = true, Data = batchNoes });
            }

            using var t = _context.Database.BeginTransaction();
            // insert Series Detail
            _seriesDetail.Number = Series.NextNo;
            _seriesDetail.SeriesID = Series.ID;
            _context.SeriesDetails.Update(_seriesDetail);
            _context.SaveChanges();
            var seriesDetailID = _seriesDetail.ID;
            string _Sno = Series.NextNo;
            long _No = long.Parse(_Sno);
            Series.NextNo = Convert.ToString(_No + 1);

            points.BranchID = GetBranchID();
            points.CompanyID = GetCompany().ID;
            points.LocalCurrencyID = GetCompany().LocalCurrencyID;
            points.LocalSetRate = localSetRate;
            points.Number = _seriesDetail.Number;
            points.UserID = GetUserId();
            points.SysCurrencyID = GetCompany().SystemCurrencyID;
            points.SeriesDID = _seriesDetail.ID;
            points.SeriesID = Series.ID;
            points.DocTypeID = douType.ID;
            points.CustomerID = customerId;
            if (ModelState.IsValid)
            {
                _context.PointRedemptionMasters.Update(points);
                _context.SaveChanges();
                await _loyaltyProgramPos.PostPointRedemptionsAsync(customerId, points, douType, serialNumber, batchNoes);
                ModelState.AddModelError("success", "Redemption point is succeeded!");
                msg.Approve();
                t.Commit();
            }
            return Ok(msg.Bind(ModelState));
        }

        [HttpPost("getInvoice")]
        public IActionResult GetInvoice()
        {
            var invoice = (from dt in _context.DocumentTypes.Where(i => i.Code == "RD")
                           join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                           select new
                           {
                               sr.ID,
                               sr.Name,
                               sr.Default,
                               sr.NextNo,
                           }).ToList();
            return Ok(invoice);
        }

        /// <summary>
        /// KSMS Block
        /// </summary>
        [HttpPost("getKSServices/{plId}")]
        public async Task<IActionResult> GetKSServices(int plId)
        {
            var data = await _kSMS.GetServiceAsync(plId);
            return Ok(data);
        }

        [HttpPost("getVehicles/{cusId}")]
        public async Task<IActionResult> GetVehicles(int cusId)
        {
            var data = await _kSMS.GetVehiclesAsync(cusId);
            return Ok(data);
        }

        [HttpPost("getServiceDetail/{id}")]
        public async Task<IActionResult> GetServiceDetail(int id)
        {
            var data = await _kSMS.GetServiceDetailAsync(id);
            return Ok(data);
        }

        [HttpPost("getSoldService/{cusId}/{plId}/{keyword?}")]
        public async Task<IActionResult> GetSoldService(int cusid, int plId, string keyword = "")
        {
            var data = await _kSMS.GetSoldServiceAsync(cusid, plId, keyword);
            return Ok(data);
        }

        [HttpPost("updateUseServices/{data}/{serial}/{batch}")]
        public async Task<IActionResult> UpdateUseServices(string data, string serial, string batch)
        {
            KSServiceMaster kSServices = JsonConvert.DeserializeObject<KSServiceMaster>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            using var t = _context.Database.BeginTransaction();
            SeriesDetail seriesDetail = new();
            var seriesUS = _context.Series.FirstOrDefault(w => w.ID == kSServices.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == kSServices.DocTypeID).FirstOrDefault();
            seriesDetail.Number = seriesUS.NextNo;
            seriesDetail.SeriesID = kSServices.SeriesID;
            _context.SeriesDetails.Update(seriesDetail);
            _context.SaveChanges();
            var seriesDetailID = seriesDetail.ID;
            string Sno = seriesUS.NextNo;
            long No = long.Parse(Sno);
            kSServices.Number = seriesUS.NextNo;
            seriesUS.NextNo = Convert.ToString(No + 1);
            if (long.Parse(seriesUS.NextNo) > long.Parse(seriesUS.LastNo))
            {
                ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
            }
            if (ModelState.IsValid)
            {
                var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID);
                var cur = _context.PriceLists.Find(kSServices.PriceListID);
                var rate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == cur.CurrencyID);
                kSServices.CompanyID = GetCompany().ID;
                kSServices.LocalCurrencyID = GetCompany().LocalCurrencyID;
                kSServices.SysCurrencyID = GetCompany().SystemCurrencyID;
                kSServices.LocalSetRate = localSetRate.SetRate;
                kSServices.BranchID = int.Parse(User.FindFirst("BranchID").Value);
                kSServices.UserID = GetUserId();
                kSServices.ExchangeRate = (decimal)rate.Rate;
                kSServices.CreatedAt = DateTime.Now;
                kSServices.SeriesDID = seriesDetail.ID;
                _context.KSServiceMaster.Update(kSServices);
                _context.SaveChanges();
                // TODO :: check stock
                var returnItems = await _kSMS.CheckItemOutOfStockAsync(kSServices.KsServiceDetials);
                if (returnItems.Any()) return Ok(returnItems);
                // Checking Serial Batch //
                List<SerialNumber> serialNumber = new();
                List<BatchNo> batchNoes = new();

                serialNumber = serial != null ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : serialNumber;
                foreach (var i in kSServices.KsServiceDetials)
                {
                    var _dataksItems = _context.ServiceSetupDetials.Where(item => item.ServiceSetupID == i.KSServiceSetupId).ToList();
                    _POSSerialBatch.CheckItemSerailKSMS(i, _dataksItems, serialNumber, kSServices.ID, kSServices.WarehouseID);
                }
                int latestSaleSNID = serialNumber.Count == 0 ? 0 : serialNumber.Max(i => i.SaleID);
                serialNumber = serialNumber.Where(i => i.SaleID == latestSaleSNID).ToList();
                serialNumber = (from s in serialNumber
                                group s by s.ItemID into i
                                let sn = i.FirstOrDefault()
                                select new SerialNumber
                                {
                                    BpId = sn.BpId,
                                    OpenQty = i.Sum(sum => sum.OpenQty) < 0 ? 0 : i.Sum(sum => sum.OpenQty),
                                    Cost = sn.Cost,
                                    ItemName = sn.ItemName,
                                    ItemID = sn.ItemID,
                                    Direction = sn.Direction,
                                    ItemCode = sn.ItemCode,
                                    LineID = sn.LineID,
                                    Qty = i.Sum(sum => sum.Qty),
                                    SaleID = sn.SaleID,
                                    SerialNumberSelected = sn.SerialNumberSelected,
                                    SerialNumberUnselected = sn.SerialNumberUnselected,
                                    TotalSelected = i.Sum(sum => sum.TotalSelected),
                                    UomID = sn.UomID,
                                    WhsCode = sn.WhsCode,
                                    BaseQty = sn.BaseQty,
                                }).ToList();
                bool isHasSerialItem = serialNumber.Any(i => i.OpenQty != 0);
                if (isHasSerialItem)
                {
                    return Ok(new { IsSerail = true, Data = serialNumber });
                }

                batchNoes = batch != null ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : batchNoes;

                foreach (var i in kSServices.KsServiceDetials)
                {
                    var _dataksItems = _context.ServiceSetupDetials.Where(item => item.ServiceSetupID == i.KSServiceSetupId).ToList();
                    _POSSerialBatch.CheckItemBatchKSMS(i, _dataksItems, batchNoes, kSServices.ID, kSServices.WarehouseID);
                }
                int latestSaleBNID = batchNoes.Count == 0 ? 0 : batchNoes.Max(i => i.SaleID);
                batchNoes = batchNoes.Where(i => i.SaleID == latestSaleBNID).ToList();
                batchNoes = (from b in batchNoes
                             group b by b.ItemID into i
                             let bn = i.FirstOrDefault()
                             let uom = _context.GroupDUoMs.FirstOrDefault(i => i.UoMID == bn.UomID) ?? new GroupDUoM()
                             select new BatchNo
                             {
                                 UomID = uom.BaseUOM,
                                 BaseQty = bn.BaseQty,
                                 BatchNoSelected = bn.BatchNoSelected,
                                 BatchNoUnselect = bn.BatchNoUnselect,
                                 BpId = bn.BpId,
                                 Cost = bn.Cost,
                                 Direction = bn.Direction,
                                 ItemCode = bn.ItemCode,
                                 ItemID = bn.ItemID,
                                 ItemName = bn.ItemName,
                                 LineID = bn.LineID,
                                 Qty = i.Sum(sum => sum.Qty),
                                 SaleID = bn.SaleID,
                                 TotalBatches = bn.TotalBatches,
                                 TotalNeeded = i.Sum(sum => sum.TotalNeeded) < 0 ? 0 : i.Sum(sum => sum.TotalNeeded),
                                 TotalSelected = i.Sum(sum => sum.TotalSelected),
                                 WhsCode = bn.WhsCode
                             }).ToList();
                bool isHasBatchItem = batchNoes.Any(i => i.TotalNeeded != 0);
                if (isHasBatchItem)
                {
                    return Ok(new { IsBatch = true, Data = batchNoes });
                }
                await _kSMS.UpdateKSServiceAsync(kSServices, serialNumber, batchNoes);
                t.Commit();
                msg.Approve();
                msg.AddItem(seriesUS, "seriesUS");
            }
            return Ok(msg.Bind(ModelState));
        }

        [HttpPost("getSaleReport/{fromDate}/{toDate}")]
        public async Task<IActionResult> GetSaleReport(string fromDate, string toDate)
        {
            var data = await _kSMS.GetSaleReportAsync(fromDate, toDate, GetCompany());
            return Ok(data);
        }

        [HttpPost("postPromoCode/{priceListId}/{code}")]
        public async Task<IActionResult> PostPromoCode(int priceListID, string Code)
        {
            ModelMessage message = new();
            PromoCodeDetail pcd = new();
            PromoCodeDiscount pcds = new();
            if (ModelState.IsValid)
            {
                pcds = await _posRetail.GetPromoCodeAsync(priceListID, Code);
                pcd = _context.PromoCodeDetails.FirstOrDefault(s => s.PromoCode == Code && s.MaxUse > s.UseCount) ?? new PromoCodeDetail();
                if (pcds == null || pcd.ID == 0)
                {
                    ModelState.AddModelError("PromoCode", "Promo code is invalid!");
                }
                else
                {
                    ModelState.AddModelError("PromoCode", "Promo code is verified.");
                    message.Approve();
                    message.AddItem(pcds, "PromoCodeDiscount");
                    message.AddItem(pcd, "PromoCodeDetail");
                }
            }
            return Ok(message.Bind(ModelState));
        }

        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserId())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }

        /// <summary>
        /// End KSMS Block
        /// </summary>
        /// 

        // Card Member //
        [HttpPost("getCardMemberDetial/{cardNumber}/{grandTotal}/{pricelistId}")]
        public IActionResult GetCardMemberDetial(string cardNumber, decimal grandTotal, int pricelistId)
        {
            var data = _posRetail.GetCardMemberDetial(cardNumber, grandTotal, pricelistId, ModelState);
            return Ok(data.Bind(ModelState));
        }

        [HttpPost("getMemberCardDiscount/{cardNumber}/{pricelistId}")]
        public IActionResult GetMemberCardDiscount(string cardNumber, int pricelistId)
        {
            var data = _posRetail.GetMemberCardDiscount(cardNumber, pricelistId, ModelState);
            return Ok(data.Bind(ModelState));
        }

        private int GetBranchID()
        {
            _ = int.TryParse(User.FindFirst("BranchID").Value, out int _id);
            return _id;
        }
    }
}
