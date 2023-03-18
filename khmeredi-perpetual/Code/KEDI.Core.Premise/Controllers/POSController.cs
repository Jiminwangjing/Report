using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory.Category;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.POS.Template;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.ServicesClass;
using KEDI.Core.Premise.Models.Services.POS.Templates;
using KEDI.Core.Premise.Models.Services.CardMembers;
using KEDI.Core.Helpers.Enumerations;
using CKBS.Models.Services.Inventory;
using Microsoft.EntityFrameworkCore;
using KEDI.Core.Premise.Models.Services.POS.CanRing;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.Services.POS;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Services;
using CKBS.Models.Services.Administrator.Tables;
using System.Threading;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege("A044")]
    public class POSController : Controller
    {
        readonly PosRetailModule _posRetail;
        readonly LoyaltyProgramPosModule _loyaltyProgramPos;
        readonly IPOS _pos;
        readonly IBusinessPartner _partner;
        private readonly IPOSSerialBatchRepository _POSSerialBatch;
        private readonly DataContext _context;
        private readonly IKSMSRopository _kSMS;
        private readonly IPurchaseRepository _ipur;
        private readonly UtilityModule _utility;
        private readonly IPosClientSignal _clientSignal;
        public POSController(IPOS pos, IBusinessPartner partner,
            PosRetailModule posRetail, LoyaltyProgramPosModule loyaltyProgramPos,
            IPOSSerialBatchRepository POSSerialBatch, IKSMSRopository kSMS,
            DataContext context, IPurchaseRepository ipur, UtilityModule utility, IPosClientSignal clientSignal)
        {
            _posRetail = posRetail;
            _loyaltyProgramPos = loyaltyProgramPos;
            _pos = pos;
            _partner = partner;
            _POSSerialBatch = POSSerialBatch;
            _context = context;
            _kSMS = kSMS;
            _ipur = ipur;
            _utility = utility;
            _clientSignal = clientSignal;
        }

        public Dictionary<int, string> TypeCardDiscountTypes => EnumHelper.ToDictionary(typeof(TypeCardDiscountType));

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

        [AllowAnonymous]

        public IActionResult SecondScreen()
        {
            return View();
        }

        public async Task<IActionResult> KRMS()
        {
            var user = await _posRetail.FindUserAsync(GetUserId());
            if (!_posRetail.CheckReceiptInfo(user.BranchID))
            {
                return RedirectToAction("Create", "ReceiptInformation");
            }
            var sysTypes = await _context.SystemType.Where(s => s.Status == true && s.Type == "KRMS").ToListAsync();
            ViewBag.SysType = sysTypes.Select(i => new SelectListItem
            {
                Text = i.Type,
                Value = i.Type,
                Selected = sysTypes.Any(i => i.Status),
            }).ToList();
            var settingModel = await _posRetail.GetSettingViewModelAsync(user.ID, "/POS/KRMS");
            var setting = settingModel.Setting;
            ViewBag.CardType = new SelectList(_context.TypeCards.Where(i => !i.IsDeleted), "ID", "Code");
            ViewBag.DataInvoice = GetSeriesCM();
            ViewBag.TypeCardDiscountTypes = TypeCardDiscountTypes.Select(i => new SelectListItem
            {
                Text = i.Value,
                Value = i.Key.ToString()
            });
            if (setting.ID > 0)
            {
                var pos = new POSModel
                {
                    ServiceTable = await _posRetail.GetServiceTableAsync(user.ID),
                    ItemGroup1s = new List<ItemGroup1>(),
                    Setting = settingModel,
                    Customer = await _posRetail.FindCustomerAsync(setting.CustomerID),
                    Warehouses = await _posRetail.SelectWarehousesAsync(),
                    Templateurl = _utility.PrintTemplateUrl()
                };
                return View(pos);
            }
            else
            {
                ViewBag.KRMS = "highlight";
                return View("GeneralSetting", settingModel);
            }
        }

        public async Task<IActionResult> KBMS()
        {
            var user = await _posRetail.FindUserAsync(GetUserId());
            if (!_posRetail.CheckReceiptInfo(user.BranchID))
            {
                return RedirectToAction("Create", "ReceiptInformation");
            }
            var settingModel = await _posRetail.GetSettingViewModelAsync(user.ID, "/POS/KBMS");
            var setting = settingModel.Setting;
            var sysTypes = _context.SystemType.Where(s => s.Status == true && s.Type == "KBMS").ToList();
            ViewBag.SysType = sysTypes.Select(i => new SelectListItem
            {
                Text = i.Type,
                Value = i.Type,
                Selected = sysTypes.Any(i => i.Status),
            }).ToList();
            ViewBag.CardType = new SelectList(_context.TypeCards.Where(i => !i.IsDeleted).ToList(), "ID", "Code");
            ViewBag.DataInvoice = GetSeriesCM();
            ViewBag.TypeCardDiscountTypes = TypeCardDiscountTypes.Select(i => new SelectListItem
            {
                Value = i.Key.ToString(),
                Text = i.Value
            });
            if (setting.ID > 0)
            {
                var pos = new POSModel
                {
                    ServiceTable = await _posRetail.GetServiceTableAsync(user.ID),
                    ItemGroup1s = new List<ItemGroup1>(),
                    Setting = settingModel,
                    Customer = await _posRetail.FindCustomerAsync(setting.CustomerID),
                    Warehouses = await _posRetail.SelectWarehousesAsync(),
                    Templateurl = _utility.PrintTemplateUrl()
                };
                return View(pos);
            }
            else
            {
                ViewBag.KRMS = "highlight";
                return View("GeneralSetting", settingModel);
            }
        }

        public async Task<IActionResult> KVMS()
        {
            var user = await _posRetail.FindUserAsync(GetUserId());
            if (!_posRetail.CheckReceiptInfo(user.BranchID))
            {
                return RedirectToAction("Create", "ReceiptInformation");
            }
            var settingModel = await _posRetail.GetSettingViewModelAsync(user.ID, "/POS/KVMS");
            var setting = settingModel.Setting;
            var sysTypes = _context.SystemType.Where(s => s.Status == true && s.Type == "KVMS").ToList();
            ViewBag.SysType = sysTypes.Select(i => new SelectListItem
            {
                Text = i.Type,
                Value = i.Type,
                Selected = sysTypes.Any(i => i.Status),
            }).ToList();
            ViewBag.CardType = new SelectList(_context.TypeCards.Where(i => !i.IsDeleted).ToList(), "ID", "Code");
            ViewBag.DataInvoice = GetSeriesCM();
            ViewBag.TypeCardDiscountTypes = TypeCardDiscountTypes.Select(i => new SelectListItem
            {
                Value = i.Key.ToString(),
                Text = i.Value
            });
            if (setting.ID > 0)
            {
                var pos = new POSModel
                {
                    ServiceTable = await _posRetail.GetServiceTableAsync(user.ID),
                    ItemGroup1s = new List<ItemGroup1>(),
                    Setting = settingModel,
                    Customer = await _posRetail.FindCustomerAsync(setting.CustomerID),
                    Warehouses = await _posRetail.SelectWarehousesAsync(),
                    Templateurl = _utility.PrintTemplateUrl()
                };
                return View(pos);
            }
            else
            {
                ViewBag.KRMS = "highlight";
                return View("GeneralSetting", settingModel);
            }
        }

        public async Task<IActionResult> KTMS()
        {
            var user = await _posRetail.FindUserAsync(GetUserId());
            if (!_posRetail.CheckReceiptInfo(user.BranchID))
            {
                return RedirectToAction("Create", "ReceiptInformation");
            }
            var settingModel = await _posRetail.GetSettingViewModelAsync(user.ID, "/POS/KTMS");
            var setting = settingModel.Setting;
            var sysTypes = _context.SystemType.Where(s => s.Status == true && s.Type == "KTMS").ToList();
            ViewBag.SysType = sysTypes.Select(i => new SelectListItem
            {
                Text = i.Type,
                Value = i.Type,
                Selected = sysTypes.Any(i => i.Status),
            }).ToList();
            ViewBag.CardType = new SelectList(_context.TypeCards.Where(i => !i.IsDeleted).ToList(), "ID", "Code");
            ViewBag.DataInvoice = GetSeriesCM();
            ViewBag.TypeCardDiscountTypes = TypeCardDiscountTypes.Select(i => new SelectListItem
            {
                Value = i.Key.ToString(),
                Text = i.Value
            });
            if (setting.ID > 0)
            {
                var pos = new POSModel
                {
                    ServiceTable = await _posRetail.GetServiceTableAsync(user.ID),
                    ItemGroup1s = new List<ItemGroup1>(),
                    Setting = settingModel,
                    Customer = await _posRetail.FindCustomerAsync(setting.CustomerID),
                    Warehouses = await _posRetail.SelectWarehousesAsync(),
                    Templateurl = _utility.PrintTemplateUrl()
                };
                return View(pos);
            }
            else
            {
                ViewBag.KRMS = "highlight";
                return View("GeneralSetting", settingModel);
            }
        }

        public async Task<IActionResult> KSMS()
        {
            var user = await _posRetail.FindUserAsync(GetUserId());
            if (!_posRetail.CheckReceiptInfo(user.BranchID))
            {
                return RedirectToAction("Create", "ReceiptInformation");
            }
            var settingModel = await _posRetail.GetSettingViewModelAsync(user.ID, "/POS/KSMS");
            var setting = settingModel.Setting;
            var sysTypes = _context.SystemType.Where(s => s.Status == true && s.Type == "KSMS").ToList();
            ViewBag.SysType = sysTypes.Select(i => new SelectListItem
            {
                Text = i.Type,
                Value = i.Type,
                Selected = sysTypes.Any(i => i.Status),
            }).ToList();
            ViewBag.CardType = new SelectList(_context.TypeCards.Where(i => !i.IsDeleted).ToList(), "ID", "Code");
            ViewBag.DataInvoice = GetSeriesCM();
            ViewBag.TypeCardDiscountTypes = TypeCardDiscountTypes.Select(i => new SelectListItem
            {
                Value = i.Key.ToString(),
                Text = i.Value
            });
            if (setting.ID > 0)
            {
                var pos = new POSModel
                {
                    ServiceTable = await _posRetail.GetServiceTableAsync(user.ID),
                    ItemGroup1s = new List<ItemGroup1>(),
                    Setting = settingModel,
                    Customer = await _posRetail.FindCustomerAsync(setting.CustomerID),
                    Warehouses = await _posRetail.SelectWarehousesAsync(),
                    Templateurl = _utility.PrintTemplateUrl()
                };
                return View(pos);
            }
            else
            {
                ViewBag.KRMS = "highlight";
                return View("GeneralSetting", settingModel);
            }
        }
        [HttpGet]
        public string GetCustomerCode()
        {
            var cus = _context.BusinessPartners.Where(i => i.Type == "Customer").ToList().Count;
            return $"C{220000 + cus}";
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
            _ = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _userId);
            return _userId;
        }

        public async Task<IActionResult> GeneralSetting(int userId = 0, string redirectUrl = "", bool json = false)
        {
            ViewBag.UserAccount = "highlight";
            var sysTypes = await _context.SystemType.Where(s => s.Status == true).ToListAsync();
            ViewBag.SysType = sysTypes.Select(i => new SelectListItem
            {
                Text = i.Type,
                Value = i.Type,
            }).ToList();

            var setting = await _posRetail.GetUserSettingAsync(userId);
            if (json)
            {
                setting.UserID = (userId <= 0) ? GetUserId() : userId;
                return Ok(setting);
            }
            var data = await _posRetail.GetSettingViewModelAsync(userId, redirectUrl);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> GetUserSetting(int userId = 0)
        {
            if (userId <= 0)
            {
                userId = GetUserId();
            }
            var setting = await _posRetail.GetUserSettingAsync(userId);
            return Ok(setting);
        }

        [HttpPost]
        public IActionResult CheckPrivilege(string code)
        {
            int userId = GetUserId();
            bool valid = _posRetail.CheckPrivilege(userId, code);
            return Ok(valid);
        }
        [HttpPost]
        public IActionResult GetUserAccess(UserCredentials creds)
        {
            bool verified = _posRetail.VerifyUserAcess(creds);
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

        [HttpPost]
        public async Task<IActionResult> SearchTables(string keyword = "", bool onlyFree = false)
        {
            var tables = await _posRetail.SearchTables(GetUserId(), keyword, onlyFree);
            return Ok(tables);
        }

        [HttpPost]
        public async Task<IActionResult> GetServiceTables()
        {
            var tables = await _posRetail.GetServiceTableAsync(GetUserId());
            return Ok(tables);
        }

        [HttpPost]
        public async Task<IActionResult> GetOtherTables(int currentTableId)
        {
            var serviceTable = await _posRetail.GetServiceTableAsync(GetUserId());
            var otherTables = serviceTable.Tables.Where(t => t.ID != currentTableId);
            return Ok(otherTables);
        }

        [HttpPost]
        public async Task<IActionResult> GetFreeTables(int tableId)
        {
            var serviceTable = await _posRetail.GetServiceTableAsync(GetUserId());
            var freeTables = serviceTable.Tables.Where(t => t.ID != tableId && t.Status == 'A');
            return Ok(freeTables);
        }

        [HttpPost]
        public IActionResult GetTablesByGroup(int groupId = 0)
        {
            return Ok(_posRetail.GetTablesByGroupId(groupId));
        }

        [HttpPost]
        public async Task<IActionResult> GetPromotionInfo(int priceListId, int warehouseId = 0)
        {
            var promoInfo = await _posRetail.GetPromotionInfoAsync(priceListId, warehouseId);
            return Ok(promoInfo);
        }

        [HttpPost]
        public async Task<IActionResult> FetchOrderInfo(int tableId, int orderId = 0, int customerId = 0, bool setDefaultOrder = false)
        {
            var orderInfo = await _posRetail.GetOrderInfoAsync(tableId, orderId, customerId, setDefaultOrder);
            return Ok(orderInfo);
        }

        [HttpPost]
        public async Task<IActionResult> GetCurrentOrderInfo(int tableId, int orderId, int customerId, bool newOrder = false)
        {
            var order = await _posRetail.GetCurrentOrderInfoAsync(tableId, orderId, customerId, newOrder);
            return Ok(order);
        }

        /// Serial Batch ///
        [HttpPost]
        [RequestFormLimits(ValueCountLimit = int.MaxValue, Order = 1)]
        public IActionResult CheckSerailNumber(List<SerialNumber> serails)
        {
            ModelMessage msg = new();
            for (var i = 0; i < serails.Count; i++)
            {
                if (serails[i].OpenQty > 0)
                {
                    ModelState.AddModelError(
                        $"OpenQty{i}",
                        $"Item name {serails[i].ItemName} at line {i + 1} \"Total Selected\" is not enough. \"Total QTY\" is {serails[i].Qty}, and \"Total Selected\" is {serails[i].TotalSelected}!");
                    return Ok(msg.Bind(ModelState));
                }
            }
            msg.Approve();
            return Ok(msg.Bind(ModelState));
        }

        [HttpGet]
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

        [HttpGet]
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

        [HttpGet]
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

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = int.MaxValue, Order = 1)]
        public IActionResult CheckBatchNo(List<BatchNo> batches)
        {
            ModelMessage msg = new();
            for (var i = 0; i < batches.Count; i++)
            {
                if (batches[i].TotalNeeded > 0)
                {
                    ModelState.AddModelError(
                        $"OpenQty{i}",
                        $"Item name {batches[i].ItemName} at line {i + 1} \"Total Selected\" is not enough. \"Total QTY\" is {batches[i].Qty}, and \"Total Selected\" is {batches[i].TotalSelected}!");
                    return Ok(msg.Bind(ModelState));
                }
            }
            msg.Approve();
            return Ok(msg.Bind(ModelState));
        }

        [HttpGet]
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
        [HttpPost]
        [RequestFormLimits(ValueCountLimit = int.MaxValue, Order = 1)]
        //[ValidateAntiForgeryToken(Order = 2)]
        public async Task<IActionResult> SubmitOrder(Order order, string printType, List<SerialNumber> serials, List<BatchNo> batches, string promoCode = "")
        {
            var itemReturns = await _posRetail.SubmitOrderAsync(order, printType, serials, batches, promoCode);
            var objgeneralsetting = await _posRetail.GetUserSettingAsync(GetUserId());
            itemReturns.PreviewReceipt = objgeneralsetting.PreviewReceipt;
            return Ok(itemReturns);
        }

        [HttpPost]
        public async Task<IActionResult> FindLineItemByBarcode(int orderId, int pricelistId, string barcode)
        {
            var lineItem = await _posRetail.FindItemByBarcodeAsync(orderId, pricelistId, GetCompany().ID, barcode);
            if (lineItem != null && lineItem.IsOutOfStock && lineItem.IsSerialNumber)
                return Ok(new { Error = true, Message = $"Serial Number \"{barcode}\" of item \"{lineItem.KhmerName}\" is out of stock" });
            return Ok(lineItem);
        }

        [HttpPost]
        public async Task<IActionResult> SearchSaleItems(int orderId, string keyword)
        {
            var saleItems = await _posRetail.GetSaleItemsAsync(GetUserId(), orderId, keyword);
            return Ok(saleItems);
        }

        [HttpPost]
        public async Task<IActionResult> GetGroupItems(int group1, int group2, int group3, int priceListId, int level = 0, bool onlyAddon = false)
        {
            var saleItems = await _posRetail.GetGroupItemsAsync(group1, group2, group3, priceListId, GetCompany().ID, level, onlyAddon);
            return Ok(saleItems);
        }

        [HttpPost]
        public async Task<IActionResult> SaveItemComment(ItemComment comment)
        {
            var itemComment = await _posRetail.SaveItemCommentAsync(comment, ModelState);
            ModelMessage modelMessage = new(ModelState);
            if (ModelState.IsValid)
            {
                modelMessage.Approve();
            }
            return Ok(new { Comment = itemComment, Message = modelMessage });
        }

        [HttpPost]
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

        [HttpPost]
        public async Task<IActionResult> GetItemComments()
        {
            var comments = await _posRetail.GetItemCommentsAsync();
            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSetting(GeneralSetting setting, string redirectUrl = "", bool returnJson = false)
        {
            await _posRetail.UpdateSettingAsync(setting);

            if (returnJson)
            {
                return Ok(setting);
            }

            if (string.IsNullOrEmpty(redirectUrl))
            {
                return RedirectToAction("GeneralSetting");
            }

            return Redirect(redirectUrl);
        }

        [HttpPost]
        public IActionResult CheckOpenShift(int userId = 0)
        {
            if (userId <= 0)
            {
                userId = GetUserId();
            }

            var hasOpenShift = _posRetail.CheckOpenShift(userId);
            return Ok(hasOpenShift);
        }

        [HttpPost]
        public async Task<IActionResult> GetShiftTemplate()
        {
            var shiftTemplate = await _posRetail.CreateShiftTemplateAsync(GetUserId());
            return Ok(shiftTemplate);
        }
        [HttpPost]
        public async Task<IActionResult> VoidOrder(int orderId, string reason)
        {
            var isVoidSuccess = await _posRetail.VoidOrderAsync(orderId, reason);
            return Ok(isVoidSuccess);
        }

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = int.MaxValue, Order = 1)]
        public async Task<IActionResult> MoveOrders(int fromTableId, int toTableId, List<Order> orders)
        {
            var _orderId = await _posRetail.MoveOrdersAsync(fromTableId, toTableId, orders);
            return Ok(_orderId);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeTable(int previousId, int currentId)
        {
            var currentTable = await _posRetail.ChangeTableAsync(previousId, currentId);
            return Ok(currentTable);
        }

        [HttpPost]
        public IActionResult SaveOrder(string order)
        {
            var _order = JsonConvert.DeserializeObject<Order>(order,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            _posRetail.SaveOrder(_order);
            return Ok(_order);
        }

        [HttpPost]
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
        [HttpPost]
        public async Task<IActionResult> GetChangeRateTemplate(int orderId = 0, int customerId = 0)
        {
            var changeRate = await _posRetail.CreateChangeRateTemplate(orderId, customerId);
            return Ok(changeRate);
        }

        [HttpPost]
        public IActionResult SaveDisplayCurrencies(List<DisplayCurrency> displayCurrencies)
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

        [HttpPost]
        public async Task<IActionResult> GetCustomers(string keyword = "")
        {
            var customers = await _posRetail.GetCustomersAsyc(keyword);
            return Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> GetMemberCards(string keyword = "")
        {
            var membercards = await _loyaltyProgramPos.GetMemberCardsAsync(keyword);
            return Ok(membercards);
        }

        [HttpPost]
        public async Task<IActionResult> GetOrderDetailUnknown(int orderId)
        {
            var unknownItem = await _posRetail.GetOrderDetailUnknownAsync(GetUserId(), GetCompany().ID, orderId);
            return Ok(unknownItem);
        }

        [HttpPost]
        public async Task<IActionResult> GetOrdersToCombine(int orderId)
        {
            var orders = await _posRetail.GetOrdersToCombineAsync(orderId);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CombineOrders(CombineOrder combineOrder)
        {
            using var t = _context.Database.BeginTransaction();
            await _posRetail.CombineOrdersAsync(combineOrder);
            t.Commit();
            return Ok(combineOrder);
        }

        [HttpPost]
        public async Task<IActionResult> SplitOrder(Order splitOrder)
        {
            Order newOrder = await _posRetail.SplitOrderAsync(splitOrder);
            return Ok(newOrder);
        }

        [HttpPost]
        public async Task<IActionResult> GetReturnReceipts(string dateFrom, string dateTo, string keyword = "")
        {
            var receipts = await _posRetail.GetReturnReceiptsAsync(GetUserId(), dateFrom, dateTo, keyword);
            return Ok(receipts);
        }

        [HttpPost]
        public async Task<IActionResult> GetReturnItems(int receiptId)
        {
            var returnedItems = await _posRetail.GetReturnItemsAsync(GetUserId(), receiptId);
            return Ok(returnedItems);
        }
        [HttpGet]
        public IActionResult GetMultipaymean(int receipid)
        {
            var list = (from mp in _context.MultiPaymentMeans.Where(s => s.ReceiptID == receipid)

                        let pm = _context.PaymentMeans.FirstOrDefault(s => !s.Delete && s.ID == mp.PaymentMeanID) ?? new CKBS.Models.Services.Banking.PaymentMeans()
                        select new
                        {
                            ID = mp.ID,
                            ReceiptID = mp.ReceiptID,
                            PaymentMeanID = mp.PaymentMeanID,
                            Name = pm.ID == 0 ? "Pay Card" : pm.Type,
                            OpenAmount = mp.OpenAmount,
                            Amount = mp.OpenAmount,
                            Currency = mp.AltCurrency,
                            Status = false,
                        }).ToList();

            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPaymean()
        {
            PaymentMeans obj = new PaymentMeans();
            var list = (from pm in _context.PaymentMeans.Where(s => !s.Delete)
                        select new
                        {
                            ID = pm.ID,
                            Name = pm.Type,
                            Default = pm.Default,

                        }).ToList();
            if (list != null)
                return Ok(list);
            else
            {
                obj = _context.PaymentMeans.FirstOrDefault() ?? new PaymentMeans();
                return Ok(obj);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendReturnItems(List<ReturnItem> returnItems, string serial, string batch, int PaymentmeansID, string reason = "")
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
            await _posRetail.ReturnReceiptsAsync(returnItems, ModelState, _serialNumber, _batchNoes, PaymentmeansID,GetUserId(), reason);
            var message = new ModelMessage(ModelState);
            if (ModelState.IsValid)
            {
                message.Add("ItemReturns", "Items returned successfully...");
                message.Approve();
            }
            return Ok(message);
        }

        [HttpPost]
        public async Task<IActionResult> GetReceiptsToCancel(string dateFrom, string dateTo, string keyword = "")
        {
            var receipts = await _posRetail.GetReceiptsToCancelAsync(GetUserId(), dateFrom, dateTo, keyword);
            return Ok(receipts);
        }

        [HttpPost]
        public async Task<IActionResult> CancelReceipt(int receiptId, string serial, string batch, string checkingSerialString, string checkingBatchString, string reason, int PaymentmeansID)
        {
            #region
            ModelMessage message = new();
            var receipt = _context.Receipt.FirstOrDefault(w => w.ReceiptID == receiptId);
            var receiptDetial = _context.ReceiptDetail.Where(i => i.ReceiptID == receipt.ReceiptID).ToList();
            //// Checking Serial Batch //
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
            await _posRetail.CancelReceiptAsync(receipt, ModelState, _serialNumber, _batchNoes, reason, PaymentmeansID);
            if (ModelState.IsValid)
            {
                message.Add("Receipt", $"The specified receipt is cancelled successfully.");
                message.Approve();
            }
            return Ok(message);
        }

        [HttpPost]
        public async Task<IActionResult> GetReceiptsToReprint(string dateFrom, string dateTo, string keyword = "")
        {
            var receipts = await _posRetail.GetReceiptsToReprintAsync(GetUserId(), dateFrom, dateTo, keyword);
            var priveiwsetting = await _context.GeneralSettings.FirstOrDefaultAsync() ?? new GeneralSetting();

            return Ok(new { receipts = receipts, priveiwsetting = priveiwsetting.PreviewReceipt, userid = GetUserId() });
        }
        [HttpPost]
        public async Task<IActionResult> GetPendingVoidItem(string dateFrom, string dateTo, string keyword = "")
        {
            _ = DateTime.TryParse(dateTo, out DateTime _toDate);
            _ = DateTime.TryParse(dateFrom, out DateTime _fromDate);
            var receipts = await _posRetail.GetPendingVoidItemAsync(GetUserId(), _fromDate, _toDate, keyword);
            return Ok(receipts);
        }
        [HttpPost]
        public IActionResult SubmitPendingVoidItem(string data, string reason)
        {
            List<PendingVoidItemModel> _data = JsonConvert.DeserializeObject<List<PendingVoidItemModel>>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var status = _posRetail.InsertPendingVoidItemToVoidItem(_data, reason);
            return Ok(status);
        }
        [HttpPost]
        public async Task<IActionResult> ReprintReceipt(int receiptId, string printType)
        {
            await _posRetail.ReprintReceiptAsync(GetUserId(), receiptId, printType);
            return Ok(receiptId);
        }

        [HttpPost]
        public async Task<IActionResult> ReprintReceiptcloseshift(int userid, int closeShiftId)
        {
            await _posRetail.ReprintReceiptCloseShiftAsync(userid, closeShiftId);
            return Ok();
        }

        public IActionResult ReprintCloseShift(string DateFrom, string DateTo, string keyword = "")
        {
            var data = _posRetail.GetReprintCloseShifts(DateFrom, DateTo, keyword);
            return Ok(data);
        }
        [HttpPost]
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

        [HttpPost]
        public async Task<IActionResult> ProcessCloseShift(double total)
        {
            ModelMessage message = new();
            if (ModelState.IsValid)
            {
                await _posRetail.CloseShiftAsync(GetUserId(), total);
                // await _posRetail.VoidItemAsync(GetUserId());
                message.Approve();
            }
            return Ok(message.Bind(ModelState));
        }

        [HttpPost]
        public async Task<IActionResult> GetBuyXGetXDetails(int priceListId)
        {
            var buyxGetxs = await _loyaltyProgramPos.GetBuyXGetXDetailsAsync(priceListId);
            return Ok(buyxGetxs);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCustomer(POSModel posModel)
        {
            using (var t = _context.Database.BeginTransaction())
            {

                //if (posModel.Customer.Name.Length < 5)
                //{
                //    ModelState.AddModelError("Name", "Name cannot be less than 5 charaters.");
                //}

                var _customer = await _partner.SaveCustomerAsync(posModel.Customer, ModelState);
                await _partner.SaveVehiclesAsync(ModelState, posModel.Customer.ID, posModel.AutoMobiles);
                ModelMessage message = new(ModelState);
                if (ModelState.IsValid)
                {
                    message.Add("__success", "Save completed...");
                    message.AddItem(_customer, "Customer");
                    message.Approve();
                }
                t.Commit();
                return Ok(message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> VoidItem(Order order)
        {
            var voiditem = await _posRetail.VoidItemsAsync(order);
            return Ok(voiditem);
        }

        [HttpPost]
        public async Task<IActionResult> PendingVoidItem(Order order, string reason = "")
        {
            var voiditem = await _posRetail.PendingVoidItemsAsync(order, reason);
            return Ok(voiditem);
        }
        [HttpPost]
        public IActionResult DeletePendingVoidItem(int id)
        {
            var msg = _posRetail.DeletePendingVoidItem(id);
            return Ok(msg);
        }

        //Point Redemption
        [HttpPost]
        public async Task<IActionResult> FetchLoyaltyProgram(int priceListId)
        {
            var loyProg = new LoyaltyProgModel
            {
                BuyXGetXDetails = await _loyaltyProgramPos.GetBuyXGetXDetailsAsync(priceListId),
                PointMembers = await _loyaltyProgramPos.GetAvailablePointMembersAsync()
            };
            return Ok(loyProg);
        }

        [HttpPost]
        public async Task<IActionResult> GetPointRedemptionWarehouse(int customerId, int warehouseId)
        {
            var pointRedempts = await _loyaltyProgramPos.GetPointRedemptionWarehouseAsync(customerId, warehouseId);
            return Ok(pointRedempts);
        }

        [HttpPost]
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

        [HttpPost]
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
        public async Task<IActionResult> GetKSServices(int plId)
        {
            var data = await _kSMS.GetServiceAsync(plId);
            return Ok(data);
        }
        public async Task<IActionResult> GetVehicles(int cusId)
        {
            var data = await _kSMS.GetVehiclesAsync(cusId);
            return Ok(data);
        }
        public async Task<IActionResult> GetServiceDetail(int id)
        {
            var data = await _kSMS.GetServiceDetailAsync(id);
            return Ok(data);
        }
        public async Task<IActionResult> GetSoldService(int cusid, int plId, string keyword = "")
        {
            var data = await _kSMS.GetSoldServiceAsync(cusid, plId, keyword);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult SubmitBuyCanRing(string canring, string serial, string batch, string serailPur, string batchPur)
        {
            ModelMessage msg = new();
            CanRingMaster crm = JsonConvert.DeserializeObject<CanRingMaster>(canring, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            using var t = _context.Database.BeginTransaction();
            SeriesDetail seriesDetail = new();
            List<SerialNumber> _serialNumber = new();
            List<SerialNumber> serialNumber = new();
            List<BatchNo> _batchNoes = new();
            List<BatchNo> batchNoes = new();
            List<SerialViewModelPurchase> serialViewModelPurchases = new();
            List<SerialViewModelPurchase> _serialViewModelPurchases = new();
            List<BatchViewModelPurchase> batchViewModelPurchases = new();
            List<BatchViewModelPurchase> _batchViewModelPurchases = new();
            var wh = _context.Warehouses.Find(crm.WarehouseID) ?? new Warehouse();
            var itemsReturn = _posRetail.CheckStockCanRing(crm, wh);
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
                    ItemsReturns = itemsReturn,
                };
                return Ok(__dataItemTurnOutOfStokes);
            }
            if (ModelState.IsValid)
            {
                if (crm.CanRingDetials.Count > 0 && itemSBOnly.Count > 0)
                {
                    serialNumber = serial != null ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }) : serialNumber;

                    _POSSerialBatch.CheckCanRingItemSerail(crm, crm.CanRingDetials, serialNumber, "ID");
                    serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in serialNumber.ToList())
                    {
                        foreach (var i in crm.CanRingDetials)
                        {
                            if (j.ItemID == i.ItemChangeID)
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
                    _POSSerialBatch.CheckCanRingItemBatch(crm, crm.CanRingDetials, batchNoes, "ID");
                    batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in batchNoes.ToList())
                    {
                        foreach (var i in crm.CanRingDetials)
                        {
                            if (j.ItemID == i.ItemChangeID)
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
                serialViewModelPurchases = serailPur != "[]" ? JsonConvert.DeserializeObject<List<SerialViewModelPurchase>>(serailPur, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : serialViewModelPurchases;

                _ipur.CheckCanRingItemSerail(crm, crm.CanRingDetials, serialViewModelPurchases);
                serialViewModelPurchases = serialViewModelPurchases
                    .GroupBy(i => i.ItemID)
                    .Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in serialViewModelPurchases.ToList())
                {
                    foreach (var i in crm.CanRingDetials)
                    {
                        if (j.ItemID == i.ItemID)
                        {
                            _serialViewModelPurchases.Add(j);
                        }
                    }
                }
                bool isHasSerialItemPur = _serialViewModelPurchases.Any(i => i.TotalCreated != i.TotalNeeded);
                if (isHasSerialItemPur)
                {
                    return Ok(new { IsSerailPur = true, Data = _serialViewModelPurchases });
                }
                // checking batch items
                batchViewModelPurchases = batchPur != "[]" ? JsonConvert.DeserializeObject<List<BatchViewModelPurchase>>(batchPur, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : batchViewModelPurchases;
                _ipur.CheckCanRingItemBatch(crm, crm.CanRingDetials, batchViewModelPurchases);
                batchViewModelPurchases = batchViewModelPurchases
                    .GroupBy(i => i.ItemID)
                    .Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in batchViewModelPurchases.ToList())
                {
                    foreach (var i in crm.CanRingDetials)
                    {
                        if (j.ItemID == i.ItemID)
                        {
                            _batchViewModelPurchases.Add(j);
                        }
                    }
                }
                bool isHasBatchItemsPur = _batchViewModelPurchases.Any(i => i.TotalNeeded != i.TotalCreated);
                if (isHasBatchItemsPur)
                {
                    return Ok(new { IsBatchPur = true, Data = _batchViewModelPurchases });
                }
                var seriesCR = _context.Series.FirstOrDefault(w => w.ID == crm.SeriesID);
                if (seriesCR == null)
                {
                    ModelState.AddModelError("seriesUS", "No Series was created!");
                    return Ok(msg.Bind(ModelState));
                }
                var douType = _context.DocumentTypes.Where(i => i.ID == crm.DocTypeID).FirstOrDefault();
                seriesDetail.Number = seriesCR.NextNo;
                seriesDetail.SeriesID = crm.SeriesID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesCR.NextNo;
                long No = long.Parse(Sno);
                crm.Number = seriesCR.NextNo;
                seriesCR.NextNo = Convert.ToString(No + 1);
                if (long.Parse(seriesCR.NextNo) > long.Parse(seriesCR.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                    return Ok(msg.Bind(ModelState));
                }
                var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID);
                var cur = _context.PriceLists.Find(crm.PriceListID);
                var rate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == cur.CurrencyID);
                crm.CompanyID = GetCompany().ID;
                crm.LocalCurrencyID = GetCompany().LocalCurrencyID;
                crm.SysCurrencyID = GetCompany().SystemCurrencyID;
                crm.LocalSetRate = localSetRate.SetRate;
                crm.BranchID = int.Parse(User.FindFirst("BranchID").Value);
                crm.UserID = GetUserId();
                crm.ExchangeRate = (decimal)rate.Rate;
                crm.CreatedAt = DateTime.Now;
                crm.SeriesDID = seriesDetail.ID;
                crm.TotalSystem = crm.Total * crm.ExchangeRate;
                crm.GrandTotalCurrenciesDisplay = _posRetail.SetGrandTotalCurrencies(crm);
                crm.ChangeCurrenciesDisplay = _posRetail.SetChangeTotalCurrencies(crm);
                _context.CanRingMasters.Update(crm);
                _context.SaveChanges();
                _posRetail.IssuseStockCanRing(crm, _serialNumber, _batchNoes, serialViewModelPurchases, batchViewModelPurchases);
                t.Commit();
                msg.Approve();
                msg.AddItem(seriesCR, "seriesCR");
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUseServices(string data, string serial, string batch)
        {
            KSServiceMaster kSServices = JsonConvert.DeserializeObject<KSServiceMaster>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            using var t = _context.Database.BeginTransaction();
            SeriesDetail seriesDetail = new();
            if (ModelState.IsValid)
            {
                var seriesUS = _context.Series.FirstOrDefault(w => w.ID == kSServices.SeriesID);
                if (seriesUS == null)
                {
                    ModelState.AddModelError("seriesUS", "No Series was created!");
                    return Ok(msg.Bind(ModelState));
                }
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
                    return Ok(msg.Bind(ModelState));
                }
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

        public async Task<IActionResult> GetSaleReport(string fromDate, string toDate)
        {
            var data = await _kSMS.GetSaleReportAsync(fromDate, toDate, GetCompany());
            return Ok(data);
        }

        [HttpPost]
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

        public IActionResult UpdateFreight(string data)
        {
            ModelMessage msg = new();
            _posRetail.UpdateFreight(data, ModelState, msg);
            return Ok(msg.Bind(ModelState));
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

        // Card Member ///
        [HttpPost]
        public IActionResult GetCardMemberDetial(string cardNumber, decimal grandTotal, int pricelistId)
        {
            var data = _posRetail.GetCardMemberDetial(cardNumber, grandTotal, pricelistId, ModelState);
            return Ok(data.Bind(ModelState));
        }
        [HttpPost]
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

        /// //=================get data multipayment setting======
        public IActionResult GetMultipaymentMeansSetting()
        {
            var datas = _context.MultipayMeansSetting.Where(x => x.Check).ToList();
            return Ok(datas);
        }
        public IActionResult GetPaymean(int? id)
        {
            var list = _context.MultipayMeansSetting.ToList();
            if (id != 0)
            {
                foreach (var data in list)
                {
                    if (data.ID == id)
                    {
                        data.Changed = true;
                    }
                    else
                        data.Changed = false;
                    _context.Update(data);
                    _context.SaveChanges();
                }
            }
            return Ok(list);

        }
        public IActionResult GetDataChange()
        {
            var data = _context.MultipayMeansSetting.FirstOrDefault(x => x.Changed == true);
            return Ok(data);
        }
        //////=================Insert Setting Multipayment Means===============
        public IActionResult InsertSettingMultipayMeans(MultipayMeansSetting data)
        {
            int status = 0;
            if (data.Changed == true)
            {
                var list = _context.MultipayMeansSetting.Where(s => s.Changed == true).ToList();
                list.ForEach(i =>
                {
                    i.Changed = false;
                });
                _context.MultipayMeansSetting.UpdateRange(list);
                _context.SaveChanges();
            }
            var datas = _context.MultipayMeansSetting.FirstOrDefault(x => x.PaymentID == data.PaymentID) ?? new MultipayMeansSetting();
            if (datas.ID != 0)
            {

                datas.PaymentID = data.PaymentID;
                datas.SettingID = data.SettingID;
                datas.Check = data.Check;
                datas.Changed = data.Changed;
                datas.AltCurrencyID = data.AltCurrencyID;
                _context.MultipayMeansSetting.Update(datas);
                _context.SaveChanges();
            }
            else
            {
                var obj = _context.MultipayMeansSetting.FirstOrDefault() ?? new MultipayMeansSetting();
                if (obj.ID == 0)
                {
                    data.Changed = true;
                    status = 1;
                }
                _context.MultipayMeansSetting.Update(data);
                _context.SaveChanges();
            }

            if (status == 1)
                return Ok(data);
            return Ok();
        }

        public IActionResult GetMultipaymentCardMemberSetting()
        {
            var data = _context.MultipayMeansSetting.Where(x => x.PaymentID == 0).ToList();
            return Ok(data);
        }
        //====================delay hours======================
        public IActionResult GetDelayhours()
        {
            var data = _context.GeneralSettings.FirstOrDefault(x => x.DelayHours > 0) ?? new CKBS.Models.Services.POS.GeneralSetting();
            return Ok(data);
        }
        public IActionResult GetPaymentMeans()
        {
            var data = _context.MultipayMeansSetting.ToList();
            return Ok(data);
        }
        public IActionResult GetPaymentMeansSelect()
        {
            var displayCurrency = (from dc in _context.DisplayCurrencies
                                   join curr in _context.Currency on dc.AltCurrencyID equals curr.ID
                                   group new { dc, curr } by new { dc.AltCurrencyID } into g
                                   let data = g.FirstOrDefault()
                                   select new DisplayCurrencyModel
                                   {
                                       ID = data.dc.AltCurrencyID,
                                       AltCurrencyID = data.dc.AltCurrencyID,
                                       BaseCurrency = data.curr.Description,
                                       Rate = data.dc.PLDisplayRate,
                                   }).ToList();

            var payments = from p in _context.PaymentMeans.Where(p => !p.Delete)
                           join sp in _context.MultipayMeansSetting.Where(x => x.Check == true) on p.ID equals sp.PaymentID
                           select new PaymentMeans
                           {
                               ID = p.ID,
                               Type = p.Type,
                               Currency = displayCurrency.Select(c => new SelectListItem
                               {
                                   Value = c.ID.ToString(),
                                   Text = c.BaseCurrency,
                                   Selected = sp.AltCurrencyID == c.ID
                               }).ToList(),
                               IsChecked = sp == null ? false : sp.Check,
                               IsReceivedChange = sp == null ? false : sp.Changed,
                               AltCurrencyID = sp.AltCurrencyID
                           };
            return Ok(payments.ToList());
        }

        public async Task<OrderInfo> GetOrderInfoCurrAsync()
        {
            OrderInfo orderInfo = new()
            {
                PaymentMeans = await GetPaymentMeansAsync()
            };
            return orderInfo;
        }

        public async Task<List<PaymentMeans>> GetPaymentMeansAsync()
        {
            var displayCurrency = (from dc in _context.DisplayCurrencies
                                   join curr in _context.Currency.Where(x => !x.Delete) on dc.AltCurrencyID equals curr.ID
                                   group new { dc, curr } by new { dc.AltCurrencyID } into g
                                   let data = g.FirstOrDefault()
                                   select new DisplayCurrencyModel
                                   {
                                       ID = data.dc.AltCurrencyID,
                                       AltCurrencyID = data.dc.AltCurrencyID,
                                       BaseCurrency = data.curr.Description,
                                       Rate = data.dc.PLDisplayRate,
                                   }).ToList();

            var payments = from p in _context.PaymentMeans.Where(p => !p.Delete)
                           join sp in _context.MultipayMeansSetting on p.ID equals sp.PaymentID
                              into g
                           from _sp in g.DefaultIfEmpty()
                           select new PaymentMeans
                           {
                               ID = p.ID,
                               Type = p.Type,

                               Currency = displayCurrency.Select(c => new SelectListItem
                               {
                                   Value = c.ID.ToString(),
                                   Text = c.BaseCurrency,
                                   Selected = _sp == null ? false : _sp.AltCurrencyID == c.ID
                               }).ToList(),
                               AltCurrencyID = _sp == null ? 0 : _sp.AltCurrencyID,
                               IsChecked = _sp == null ? false : _sp.Check,
                               IsReceivedChange = _sp == null ? false : _sp.Changed
                           };
            return await payments.ToListAsync();
        }
        private void ValidateSummaryReason(VoidReason datas)
        {
            if (datas.Reason == null)
            {
                ModelState.AddModelError("Reason", "Please Input Reason ");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddVoidReason(VoidReason data)
        {
            ModelMessage msg = new();
            ValidateSummaryReason(data);
            if (ModelState.IsValid)
            {
                _context.VoidReasons.Update(data);
                await _context.SaveChangesAsync();
                ModelState.AddModelError("success", "Reason Add succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        public IActionResult GetVoidReason()
        {
            var data = (from re in _context.VoidReasons.Where(x => !x.Delete)
                        select new VoidReason
                        {
                            ID = re.ID,
                            Reason = re.Reason,

                        }).ToList();
            return Ok(data);

        }

        public async Task<IActionResult> DeleteReason(int id)
        {
            var datas = await _context.VoidReasons.Where(x => !x.Delete).ToListAsync();
            var data = await _context.VoidReasons.FirstOrDefaultAsync(x => x.ID == id) ?? new VoidReason();
            if (data != null)
            {
                data.Delete = true;
            }
            if (ModelState.IsValid)
            {
                _context.VoidReasons.Update(data);
                _context.SaveChanges();
            }
            return Ok(datas);
        }
        public IActionResult GetDisplayCurrency()
        {
            var displayCurrency = (from dc in _context.DisplayCurrencies
                                   join curr in _context.Currency.Where(x => !x.Delete) on dc.AltCurrencyID equals curr.ID
                                   group new { dc, curr } by new { dc.AltCurrencyID } into g
                                   let datas = g.FirstOrDefault()
                                   select new DisplayCurrencyModel
                                   {
                                       ID = datas.dc.AltCurrencyID,
                                       AltCurrencyID = datas.dc.AltCurrencyID,
                                       BaseCurrency = datas.curr.Description,
                                       Rate = datas.dc.PLDisplayRate,
                                   }).ToList();


            var data = (from pm in _context.DisplayCurrencies.Where(x => !x.IsActive)
                        select new
                        {
                            ID = 0,
                            LineID = pm.ID,
                            Amount = 0,
                            Currency = displayCurrency.Select(c => new SelectListItem
                            {
                                Value = c.ID.ToString(),
                                Text = c.BaseCurrency,
                            }).ToList(),
                        }).ToList();
            return Ok(data);
        }
        [HttpPost]
        public IActionResult SaveCustomerTips(string _data)
        {
            CustomerTips data = JsonConvert.DeserializeObject<CustomerTips>(_data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            if (data.Amount > 0)
            {
                _context.CustomerTips.Update(data);
                _context.SaveChanges();
            }

            return Ok();
        }
        public IActionResult GetTableType(int tableId)
        {
            var data = _context.Tables.Find(tableId);
            return Ok(data);
        }
        public IActionResult GetMultipaymentSettings(int paymentId)
        {
            var data = _context.MultipayMeansSetting.Find(paymentId);
            return Ok(data);
        }
     
        // public async Task<IActionResult> CheckPromotionDiscount()
        // {
        //     var date = DateTime.Now;
        //     var promoItem = await _context.PromotionDetails.Where(s => s.StartDate <= date && s.StopDate >= date).ToListAsync();
        //     var promoItemExpire = await _context.PromotionDetails.Where(s => s.StopDate < date).ToListAsync();

        //     if (promoItemExpire.Count > 0)
        //     {
        //         foreach (var item in promoItemExpire)
        //         {
        //             var item_update = _context.PriceListDetails.Find(item.ItemID);
        //             item_update.Discount = 0;
        //             item_update.PromotionID = 0;

        //             _context.PriceListDetails.Update(item_update);
        //             await _context.SaveChangesAsync();
        //         }
        //         _context.PromotionDetails.RemoveRange(promoItemExpire);
        //         await _context.SaveChangesAsync();
        //     }

        //     foreach (var item in promoItem)
        //     {
        //         var item_update = _context.PriceListDetails.Find(item.ItemID);

        //         item_update.PromotionID = item.PromotionID;
        //         item_update.Discount = item.Discount;
        //         _context.PriceListDetails.Update(item_update);
        //         await _context.SaveChangesAsync();
        //     }
        //     return Ok();
        // }

    }
}
