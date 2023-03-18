using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.Setup;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Administrator.Tables;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.POS.Template;
using CKBS.Models.Services.Promotions;
using CKBS.Models.Services.ReportSale.dev;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using KEDI.Core.Utilities;
using KEDI.Core.Helpers.Enumerations;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.DependencyInjection;
using KEDI.Core.Premise.Models.Services.Account;
using KEDI.Core.Premise.Models.Services.CardMembers;
using KEDI.Core.Premise.Models.Services.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.PromoCode;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Models.Services.POS.CanRing;
using KEDI.Core.Premise.Models.Services.POS.KSMS;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;
using KEDI.Core.Premise.Models.Services.POS.SecondScreen;
using KEDI.Core.Premise.Models.Services.POS.Templates;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Repositories.Sync;
using KEDI.Core.Premise.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository
{
    public class PosRetailModule
    {
        readonly ILogger<PosRetailModule> _logger;
        readonly IWebHostEnvironment _hostEnv;
        readonly IPOS _pos;
        readonly IIncomingPayment _incoming;
        readonly DataContext _dataContext;
        readonly UserManager _userModule;
        readonly LoyaltyProgramPosModule _loyaltyProg;
        private readonly UtilityModule _fnModule;
        private readonly IDataPropertyRepository _dataProp;
        private readonly IPOSSerialBatchRepository _POSSerialBatch;
        private readonly ISyncSender _clientPos;
        private readonly IPosClientSignal _clientSignal;
        private readonly UtilityModule _utility;
        public PosRetailModule(ILogger<PosRetailModule> logger, IWebHostEnvironment hostEnv, DataContext dataContext, UserManager userModule,
            IPOS pos, IIncomingPayment incoming, LoyaltyProgramPosModule loyaltyProg, UtilityModule fnModule,
            IDataPropertyRepository dataProperty, IPOSSerialBatchRepository POSSerialBatch, ISyncSender clientPos, IPosClientSignal clientSignal, UtilityModule utility)
        {
            _logger = logger;
            _hostEnv = hostEnv;
            _userModule = userModule;
            _pos = pos;
            _dataContext = dataContext;
            _incoming = incoming;
            _loyaltyProg = loyaltyProg;
            _fnModule = fnModule;
            _dataProp = dataProperty;
            _POSSerialBatch = POSSerialBatch;
            _clientPos = clientPos;
            _clientSignal = clientSignal;
            _utility = utility;
        }

        public Dictionary<int, string> PromoTypes => EnumHelper.ToDictionary(typeof(PromotionType));
        public Dictionary<int, string> FreightReceiptType => EnumHelper.ToDictionary(typeof(FreightReceiptType));

        private bool IsNotEmptyEqual(string a, string b, bool ignoreCase = false)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(a) && !string.IsNullOrWhiteSpace(b))
                {
                    if (ignoreCase)
                    {
                        return string.Compare(a.Trim(), b.Trim(), ignoreCase) == 0;
                    }

                    return string.Compare(a.Trim(), b.Trim()) == 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return false;
            }
        }

        public bool CheckPrivilege(int userId, string code)
        {
            bool isAllowed = _dataContext.UserPrivilleges.ToList().Any(up => up.UserID == userId
                && string.Compare(up.Code, code, true) == 0 && up.Used);
            return isAllowed;
        }

        public bool CheckReceiptInfo(int branchId)
        {
            return _dataContext.ReceiptInformation.Where(r => r.BranchID == branchId).ToList().Count > 0;
        }

        // public async Task<ModelStateDictionary> UpdateSettingAsync(GeneralSetting setting, ModelStateDictionary modelState)
        // {
        //     try
        //     {
        //         var user = await FindUserAsync(setting.UserID);
        //         var pricelist = _dataContext.PriceLists.Find(setting.PriceListID);
        //         var company = _dataContext.Company.Find(user.CompanyID);
        //         setting.BranchID = user.BranchID;
        //         setting.CompanyID = company.ID;
        //         setting.SysCurrencyID = company.SystemCurrencyID;
        //         if (pricelist != null)
        //         {
        //             var exchange = _dataContext.ExchangeRates.Find(pricelist.CurrencyID);
        //             setting.RateIn = exchange.Rate;
        //             setting.RateOut = exchange.RateOut;
        //         }

        //         if (company != null)
        //         {
        //             setting.LocalCurrencyID = (int)company?.LocalCurrencyID;
        //             setting.SysCurrencyID = (int)company?.SystemCurrencyID;
        //         }

        //         if (modelState.IsValid)
        //         {
        //             using var t = _dataContext.Database.BeginTransaction();
        //             _dataContext.GeneralSettings.Update(setting);
        //             await _dataContext.SaveChangesAsync();
        //             _pos.ResetQueue(setting);
        //             t.Commit();
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex.Message);
        //         modelState.AddModelError("Exception", ex.Message);
        //     }
        //     return modelState;
        // }

        public async Task<GeneralSetting> UpdateSettingAsync(GeneralSetting setting)
        {
            var user = await FindUserAsync(setting.UserID);
            var pricelist = _dataContext.PriceLists.Find(setting.PriceListID);
            var company = _dataContext.Company.Find(user.CompanyID);
            setting.BranchID = user.BranchID;
            setting.CompanyID = company.ID;
            setting.SysCurrencyID = company.SystemCurrencyID;
            if (pricelist != null)
            {
                var exchange = _dataContext.ExchangeRates.Find(pricelist.CurrencyID);
                setting.RateIn = exchange.Rate;
                setting.RateOut = exchange.RateOut;
            }

            if (company != null)
            {
                setting.LocalCurrencyID = (int)company?.LocalCurrencyID;
                setting.SysCurrencyID = (int)company?.SystemCurrencyID;
            }
            using var t = _dataContext.Database.BeginTransaction();
            _dataContext.GeneralSettings.Update(setting);
            await _dataContext.SaveChangesAsync();
            _pos.ResetQueue(setting);
            t.Commit();
            return setting;
        }

        public async Task<List<Table>> SearchTables(int userId, string keyword = "", bool onlyFree = false)
        {
            var user = await FindUserAsync(userId);
            var tables = _dataContext.Tables.Where(t => !t.Delete && t.GroupTable.BranchID == user.BranchID).ToList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = Regex.Replace(keyword, "\\s+", string.Empty);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                tables = tables.Where(t => Regex.Replace(t.Name, "\\s+", string.Empty).Contains(keyword, ignoreCase)
                        || string.Compare(t.Status.ToString(), keyword, ignoreCase) == 0).ToList();
            }

            if (onlyFree)
            {
                tables = tables.Where(t => t.Status == 'A').ToList();
            }

            return tables;
        }

        public async Task<ServiceTable> GetServiceTableAsync(int userId)
        {
            var user = await FindUserAsync(userId);
            int _branchId = user.BranchID;

            List<GroupTable> groupTables = await _dataContext.GroupTables.Where(gt => !gt.Delete && gt.BranchID == _branchId).ToListAsync();
            List<Table> tables = await _dataContext.Tables.Where(t => !t.Delete && groupTables.Any(gt => gt.ID == t.GroupTableID)).ToListAsync();
            ServiceTable serviceTable = new()
            {
                GroupTables = groupTables,
                Tables = tables
            };
            return serviceTable;
        }

        public List<Table> GetTablesByGroupId(int groupId = 0)
        {
            var branchId = _userModule.CurrentUser.BranchID;
            // var tables = _dataContext.Tables.Where(t => !t.Delete && t.BranchID).ToList();
            var tables = (from gr in _dataContext.GroupTables.Where(w => w.BranchID == branchId)
                          join tb in _dataContext.Tables.Where(x => !x.Delete) on gr.ID equals tb.GroupTableID
                          select tb).ToList();
            if (groupId > 0)
            {
                tables = tables.Where(t => t.GroupTableID == groupId && !t.Delete).ToList();
            }
            return tables;
        }

        public async Task<List<GroupTable>> GetGroupTablesAsync()
        {
            var branchId = _userModule.CurrentUser.BranchID;
            var groupTables = _dataContext.GroupTables.Where(gt => gt.BranchID == branchId);
            foreach (var gt in groupTables)
            {
                gt.Tables = await _dataContext.Tables.Where(t => t.GroupTableID == gt.ID && !t.Delete).ToListAsync();
            }

            return await groupTables.ToListAsync();
        }

        public async Task<Order> FindOrderAsync(IEnumerable<Order> orders, UserAccount user, int tableId, int orderId, int customerId, bool newOrder = false)
        {
            var order = orders.FirstOrDefault(o => o.OrderID == orderId) ?? orders.LastOrDefault();
            if (newOrder) { order = null; }
            order = order ?? await CreateOrderAsync(user, tableId, customerId);
            order.Customer = await FindCustomerAsync(order.CustomerID);
            GetSerialOrBatchOrder(order);
            _dataProp.DataProperty(order.OrderDetail, (int)user.CompanyID, "ItemID", "AddictionProps");
            return order;
        }

        public async Task<Order> FindOrderAsync(UserAccount user, int tableId, int orderId, int customerId, bool newOrder = false)
        {
            var orders = await GetOrdersAsync(tableId, user.BranchID, (int)user.CompanyID);
            return await FindOrderAsync(orders, user, tableId, orderId, customerId, newOrder);
        }

        public async Task<List<ServiceItemSales>> GetLimitSaleItemsAsync(int priceListId, int warehouseId, int limit = 500)
        {
            var saleItems = _pos.GetSaleItems(priceListId, warehouseId).Take(limit);
            return await saleItems.ToListAsync();
        }

        public async Task<CurrentOrderInfo> GetCurrentOrderInfoAsync(int tableId, int orderId, int customerId = 0, bool newOrder = false)
        {
            var user = _userModule.CurrentUser;
            var setting = await GetUserSettingAsync(user.ID);
            var table = await _dataContext.Tables.FindAsync(tableId);
            var orders = await GetOrdersAsync(tableId, user.BranchID, (int)user.CompanyID);
            var order = await FindOrderAsync(orders, user, tableId, orderId, customerId, newOrder);
            List<DisplayPayCurrencyModel> displayPayOtherCurrency = new();
            displayPayOtherCurrency = await GetDisplayPayCurrenciesAsync(user.ID, order.PriceListID);
            var saleItems = await GetLimitSaleItemsAsync(order.PriceListID, setting.WarehouseID);
            // var serviceItemSales = await _pos.GetItemMasterDatasAsync(order.PriceListID, setting.WarehouseID, (int)user.CompanyID); //
            //If item type is a time service (KTMS).
            foreach (var od in order.OrderDetail)
            {
                od.PrintQty = 0;
                CheckTypeofService(od, table);
            }

            var orderInfo = new CurrentOrderInfo
            {
                OrderTable = table,
                Order = order,
                // CustomerCode = orders.customer.CustomerCode,
                // CustomerName = orders.customer.CustomerName,,
                Orders = orders,
                DisplayPayOtherCurrency = displayPayOtherCurrency,
                DisplayTotalAndChangeCurrency = displayPayOtherCurrency.Where(i => i.IsShowCurrency).ToList(),
                // .Where(i => i.IsShowCurrency && !i.IsActive && i.AltCurrencyID != i.BaseCurrencyID)
                // .ToList(),
                DisplayTotalAndReceiveCurrency = displayPayOtherCurrency.Where(i => i.IsShowOtherCurrency).ToList(),
                DisplayGrandTotalOtherCurrency = displayPayOtherCurrency
                                                .Where(i => i.IsShowOtherCurrency)
                                                .ToList(),
                SaleItems = saleItems
            };
            return orderInfo;
        }

        public async Task<OrderInfo> GetOrderInfoQrAsync(int tableId, int orderId = 0, int customerId = 0, bool setDefaultOrder = false)
        {
            var user = _userModule.CurrentUser;
            if (user.ID <= 0)
            {
                user = await _userModule.GetUserQrAsync();
            }
            var setting = await GetUserSettingAsync(user.ID);
            var table = await _dataContext.Tables.FindAsync(tableId)
                        ?? await _dataContext.Tables.FirstOrDefaultAsync();
            var order = await CreateOrderAsync(user, table.ID, customerId);
            var branchId = user.BranchID;
            var orders = await _pos.GetUserOrders(branchId).Where(o => o.TableID == tableId).ToListAsync();

            if (setDefaultOrder)
            {
                if (orders.Count > 0)
                {
                    order = orders.FirstOrDefault(o => o.OrderID == orderId);
                    if (order == null)
                    {
                        order = orders.LastOrDefault();
                    }
                    order.Customer = await FindCustomerAsync(order.CustomerID);
                }
            }

            foreach (var od in order.OrderDetail)
            {
                od.PrintQty = 0;
                CheckTypeofService(od, table);
            }

            var freights = await _dataContext.Freights.Select(f => new FreightReceipt
            {
                FreightID = f.ID,
                Name = f.Name,
                AmountReven = f.IsActive ? f.AmountReven : 0,
                IsActive = f.IsActive,
                FreightReceiptType = f.FreightReceiptType,
                FreightReceiptTypes = FreightReceiptType.Select(i => new SelectListItem
                {
                    Selected = f.FreightReceiptType == (FreightReceiptType)i.Key,
                    Text = i.Value,
                    Value = i.Key.ToString()
                }).ToList()
            }).ToListAsync();
            // IEnumerable<ServiceItemSales> serviceItemSales = await _pos.GetItemMasterDatasAsync(order.PriceListID, setting.WarehouseID, user.Company.ID);
            DisplayCurrencyModel displayCurrency = new();
            List<DisplayPayCurrencyModel> displayPayOtherCurrency = new();
            List<DisplayPayCurrencyModel> displayPayOtherCurrencydefualt = new();
            List<ServiceItemSales> itemGroup1s = new();
            //displayCurrency = GetDisplayCurrency(user.ID, order.PriceListID);
            displayPayOtherCurrency = await GetDisplayPayCurrenciesAsync(user.ID, order.PriceListID);
            var saleItems = await GetLimitSaleItemsAsync(order.PriceListID, setting.WarehouseID);
            itemGroup1s.AddRange(_pos.GetGroup1s.Select(g => new ServiceItemSales
            {
                ID = g.ItemG1ID,
                KhmerName = g.Name,
                Image = g.Images,
                IsAddon = g.IsAddon,
                Group1 = g.ItemG1ID,
                Group2 = 1,
                Group3 = 1,
                Level = 1
            }));

            var members = await _loyaltyProg.GetAvailablePointMembersAsync();
            var buyXgetXs = await _loyaltyProg.GetBuyXGetXDetailsAsync(order.PriceListID);
            var comboSale = await _loyaltyProg.GetComboSaleAsync(order.PriceListID);
            var buyXamountgetXdiscounts = await _loyaltyProg.GetbuyxamountgetxdiscountAsync(order.PriceListID);
            var buyXQtygetXdiscounts = await _loyaltyProg.GetbuyxqtygetxdiscountAsync();
            var authOption = await _dataContext.AuthorizationTemplates.FirstOrDefaultAsync() ?? new AuthorizationTemplate();
            var cardMemberOption = await _dataContext.CardMemberTemplates.FirstOrDefaultAsync() ?? new CardMemberTemplate();
            var m = _dataContext.MultipayMeansSetting.FirstOrDefault(x => x.AltCurrencyID != 0) ?? new MultipayMeansSetting();
            order.DisplayRate = (double)displayCurrency.Rate;
            OrderInfo orderInfo = new()
            {
                Company​​ = user.Company,
                Branch​ = user.Branch,
                GroupTables = await GetGroupTablesAsync(),
                AuthOption = authOption.Option,
                CardMemberOption = cardMemberOption.Option,
                BaseItemGroups = itemGroup1s,
                ItemGroups = await GetItemGroupsAsync(),
                Setting = await GetUserSettingAsync(user.ID),
                SeriesPS = _fnModule.GetSeries("US"),
                SeriesCR = _fnModule.GetSeries("CR"),
                Orders = orders,
                OrderTable = table,
                Order = order,
                //DisplayCurrency = displayCurrency,
                DisplayPayOtherCurrency = displayPayOtherCurrency,

                DisplayTotalAndChangeCurrency = displayPayOtherCurrency.Where(i => i.IsShowCurrency).ToList(),
                // .Where(i => i.IsShowCurrency && !i.IsActive && i.AltCurrencyID != i.BaseCurrencyID)
                // .ToList(),
                DisplayTotalAndReceiveCurrency = displayPayOtherCurrency.Where(i => i.IsShowOtherCurrency).ToList(),
                DisplayGrandTotalOtherCurrency = displayPayOtherCurrency
                                                .Where(i => i.IsShowOtherCurrency)
                                                .ToList(),

                SaleItems = saleItems,
                Freights = freights,
                ItemUoMs = await GetItemUoMsAsync(),
                PromoTypes = PromoTypes,
                TaxGroups = await GetTaxGroupsAsync(),
                RemarkDiscountItem = await GetRemarkDiscountItemsAsync(),
                LoyaltyProgram = new LoyaltyProgModel
                {
                    PointMembers = members,
                    BuyXGetXDetails = buyXgetXs,
                    ComboSales = comboSale,
                    BuyXAmGetXDis = buyXamountgetXdiscounts,
                    BuyXQtyGetXDis = buyXQtygetXdiscounts,
                    Name = "name",
                },
                PaymentMeans = await GetPaymentMeansAsync(),
                SlideImageNames = GetSlideImageNames()
            };
            return orderInfo;
        }

        public async Task<PromotionInfo> GetPromotionInfoAsync(int priceListId, int warehouseId = 0)
        {
            var members = await _loyaltyProg.GetAvailablePointMembersAsync();
            var buyXgetXs = await _loyaltyProg.GetBuyXGetXDetailsAsync(priceListId);
            var comboSale = await _loyaltyProg.GetComboSaleAsync(priceListId);
            var buyXamountgetXdiscounts = await _loyaltyProg.GetbuyxamountgetxdiscountAsync(priceListId);
            var buyXQtygetXdiscounts = await _loyaltyProg.GetbuyxqtygetxdiscountAsync();
            var saleItems = await _pos.GetSaleItems(priceListId, warehouseId).ToListAsync();
            var lp = new LoyaltyProgModel
                {
                    PointMembers = members,
                    BuyXGetXDetails = buyXgetXs,
                    ComboSales = comboSale,
                    BuyXAmGetXDis = buyXamountgetXdiscounts,
                    BuyXQtyGetXDis = buyXQtygetXdiscounts,
                    Name = "name",
                };
            return new PromotionInfo{
                SaleItems = saleItems,
                LoyaltyProgram = lp
            };
        }

        public async Task<OrderInfo> GetOrderInfoAsync(int tableId, int orderId = 0, int customerId = 0, bool setDefaultOrder = false)
        {
            var user = _userModule.CurrentUser;
            if (user.ID <= 0)
            {
                user = await _userModule.GetUserQrAsync();
            }
            var setting = await GetUserSettingAsync(user.ID);
            var table = await _dataContext.Tables.FindAsync(tableId)
                        ?? await _dataContext.Tables.FirstOrDefaultAsync();
            var order = await CreateOrderAsync(user, table.ID, customerId);
            var orders = await GetOrdersAsync(table.ID, user.BranchID, user.Company.ID);

            if (setDefaultOrder)
            {
                if (orders.Count > 0)
                {
                    order = orders.FirstOrDefault(o => o.OrderID == orderId);
                    if (order == null)
                    {
                        order = orders.LastOrDefault();
                    }
                    order.Customer = await FindCustomerAsync(order.CustomerID);
                }
            }

            //If item type is a time service (KTMS).
            foreach (var od in order.OrderDetail)
            {
                od.PrintQty = 0;
                CheckTypeofService(od, table);
            }

            var freights = await _dataContext.Freights.Select(f => new FreightReceipt
            {
                FreightID = f.ID,
                Name = f.Name,
                AmountReven = f.IsActive ? f.AmountReven : 0,
                IsActive = f.IsActive,
                FreightReceiptType = f.FreightReceiptType,
                FreightReceiptTypes = FreightReceiptType.Select(i => new SelectListItem
                {
                    Selected = f.FreightReceiptType == (FreightReceiptType)i.Key,
                    Text = i.Value,
                    Value = i.Key.ToString()
                }).ToList()
            }).ToListAsync();
            // IEnumerable<ServiceItemSales> serviceItemSales = await _pos.GetItemMasterDatasAsync(order.PriceListID, setting.WarehouseID, user.Company.ID);
            DisplayCurrencyModel displayCurrency = new();
            List<DisplayPayCurrencyModel> displayPayOtherCurrency = new();
            List<DisplayPayCurrencyModel> displayPayOtherCurrencydefualt = new();
            List<ServiceItemSales> itemGroup1s = new();
            //displayCurrency = GetDisplayCurrency(user.ID, order.PriceListID);
            displayPayOtherCurrency = await GetDisplayPayCurrenciesAsync(user.ID, order.PriceListID);
            var saleItems = await GetLimitSaleItemsAsync(order.PriceListID, setting.WarehouseID);
            itemGroup1s.AddRange(_pos.GetGroup1s.Select(g => new ServiceItemSales
            {
                ID = g.ItemG1ID,
                KhmerName = g.Name,
                Image = g.Images,
                IsAddon = g.IsAddon,
                Group1 = g.ItemG1ID,
                Group2 = 1,
                Group3 = 1,
                Level = 1
            }));

            var members = await _loyaltyProg.GetAvailablePointMembersAsync();
            var buyXgetXs = await _loyaltyProg.GetBuyXGetXDetailsAsync(order.PriceListID);
            var comboSale = await _loyaltyProg.GetComboSaleAsync(order.PriceListID);
            var buyXamountgetXdiscounts = await _loyaltyProg.GetbuyxamountgetxdiscountAsync(order.PriceListID);
            var buyXQtygetXdiscounts = await _loyaltyProg.GetbuyxqtygetxdiscountAsync();
            var authOption = await _dataContext.AuthorizationTemplates.FirstOrDefaultAsync() ?? new AuthorizationTemplate();
            var cardMemberOption = await _dataContext.CardMemberTemplates.FirstOrDefaultAsync() ?? new CardMemberTemplate();
            var m = _dataContext.MultipayMeansSetting.FirstOrDefault(x => x.AltCurrencyID != 0) ?? new MultipayMeansSetting();
            order.DisplayRate = (double)displayCurrency.Rate;
            OrderInfo orderInfo = new()
            {
                Company​​ = user.Company,
                Branch​ = user.Branch,
                GroupTables = await GetGroupTablesAsync(),
                AuthOption = authOption.Option,
                CardMemberOption = cardMemberOption.Option,
                BaseItemGroups = itemGroup1s,
                ItemGroups = await GetItemGroupsAsync(),
                Setting = await GetUserSettingAsync(user.ID),
                SeriesPS = _fnModule.GetSeries("US"),
                SeriesCR = _fnModule.GetSeries("CR"),
                Orders = orders,
                OrderTable = table,
                Order = order,
                //DisplayCurrency = displayCurrency,
                DisplayPayOtherCurrency = displayPayOtherCurrency,

                DisplayTotalAndChangeCurrency = displayPayOtherCurrency.Where(i => i.IsShowCurrency).ToList(),
                // .Where(i => i.IsShowCurrency && !i.IsActive && i.AltCurrencyID != i.BaseCurrencyID)
                // .ToList(),
                DisplayTotalAndReceiveCurrency = displayPayOtherCurrency.Where(i => i.IsShowOtherCurrency).ToList(),
                DisplayGrandTotalOtherCurrency = displayPayOtherCurrency
                                                .Where(i => i.IsShowOtherCurrency)
                                                .ToList(),

                SaleItems = saleItems,
                Freights = freights,
                ItemUoMs = await GetItemUoMsAsync(),
                PromoTypes = PromoTypes,
                TaxGroups = await GetTaxGroupsAsync(),
                RemarkDiscountItem = await GetRemarkDiscountItemsAsync(),
                LoyaltyProgram = new LoyaltyProgModel
                {
                    PointMembers = members,
                    BuyXGetXDetails = buyXgetXs,
                    ComboSales = comboSale,
                    BuyXAmGetXDis = buyXamountgetXdiscounts,
                    BuyXQtyGetXDis = buyXQtygetXdiscounts,
                    Name = "name",
                },
                PaymentMeans = await GetPaymentMeansAsync(),
                SlideImageNames = GetSlideImageNames()
            };
            return orderInfo;
        }
        public async Task<List<PaymentMeans>> GetPaymentMeansAsync()
        {
            var displayCurrency = (from dc in _dataContext.DisplayCurrencies
                                   join curr in _dataContext.Currency.Where(x => !x.Delete) on dc.AltCurrencyID equals curr.ID
                                   group new { dc, curr } by new { dc.AltCurrencyID } into g
                                   let data = g.FirstOrDefault()
                                   select new DisplayCurrencyModel
                                   {
                                       ID = data.dc.AltCurrencyID,
                                       AltCurrencyID = data.dc.AltCurrencyID,
                                       BaseCurrency = data.curr.Description,
                                       Rate = data.dc.PLDisplayRate,
                                   }).ToList();


            var payments = from p in _dataContext.PaymentMeans.Where(p => !p.Delete)
                           join sp in _dataContext.MultipayMeansSetting on p.ID equals sp.PaymentID
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
                               // AltCurrencyID=_sp==null ? 0 :_sp.AltCurrencyID,
                               IsChecked = _sp == null ? false : _sp.Check,
                               IsReceivedChange = _sp == null ? false : _sp.Changed
                           };
            return await payments.ToListAsync();
        }
        public async Task<List<SelectListItem>> SelectTaxGroupsAsync(int taxGroupId)
        {
            List<SelectListItem> taxGroups = new()
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "---"
                }
            };

            var _taxGroups = await GetTaxGroupsAsync();
            var _selectTaxGroups = _taxGroups.Select(tg => new SelectListItem
            {
                Value = tg.ID.ToString(),
                Text = tg.Name,
                Selected = tg.ID == taxGroupId
            }).ToList();
            taxGroups.AddRange(_selectTaxGroups);
            return taxGroups;
        }

        public async Task<List<TaxGroupModel>> GetTaxGroupsAsync()
        {
            var taxgroups = await (from tg in _dataContext.TaxGroups
                             .Where(tg => tg.Active && !tg.Delete && tg.Type == Models.Services.Banking.TypeTax.OutPutTax)
                                   join td in _dataContext.TaxGroupDefinitions
                                   on tg.ID equals td.TaxGroupID
                                   group new { tg, td } by td.TaxGroupID into g
                                   let gx = g.OrderByDescending(t => t.td.EffectiveFrom).FirstOrDefault()
                                   select new TaxGroupModel
                                   {
                                       ID = gx.tg.ID,
                                       Name = gx.tg.Name,
                                       Rate = gx.td.Rate
                                   }).ToListAsync();
            return taxgroups;
        }

        public async Task<List<ServiceItemSales>> GetItemGroupsAsync()
        {
            var saleItemGroups = new List<ServiceItemSales>();
            var itemGroup1s = await _dataContext.ItemGroup1
                .Where(g => !g.Delete && string.Compare(g.Name, "None", true) != 0)
                .Select(g => new ServiceItemSales
                {
                    ID = g.ItemG1ID,
                    KhmerName = g.Name,
                    Image = g.Images,
                    Group1 = g.ItemG1ID,
                    Level = 1
                }).ToListAsync();
            var itemGroup2s = await _dataContext.ItemGroup2
                .Where(g => !g.Delete && string.Compare(g.Name, "None", true) != 0)
                .Select(g => new ServiceItemSales
                {
                    ID = g.ItemG2ID,
                    KhmerName = g.Name,
                    Image = g.Images,
                    Group1 = g.ItemG1ID,
                    Group2 = g.ItemG2ID,
                    Level = 2
                }).ToListAsync();
            var itemGroup3s = await _dataContext.ItemGroup3
                .Where(g => !g.Delete && string.Compare(g.Name, "None", true) != 0)
                .Select(g => new ServiceItemSales
                {
                    ID = g.ID,
                    KhmerName = g.Name,
                    Image = g.Images,
                    Group1 = g.ItemG1ID,
                    Group2 = g.ItemG2ID,
                    Group3 = g.ID,
                    Level = 3
                }).ToListAsync();
            saleItemGroups.AddRange(itemGroup1s);
            saleItemGroups.AddRange(itemGroup2s);
            saleItemGroups.AddRange(itemGroup3s);
            return saleItemGroups;
        }

        public async Task<List<ServiceItemSales>> GetGroupItemsAsync(int group1, int group2, int group3, int priceListId, int comId, int level = 1, bool onlyAddon = false)
        {
            var itemInfos = new List<ServiceItemSales>();
            var user = _userModule.CurrentUser;
            if (user.ID <= 0)
            {
                user = await _userModule.GetUserQrAsync();
            }
            var setting = await GetUserSettingAsync(user.ID);
            var items = await _pos.GetItemMasterDatasAsync(priceListId, setting.WarehouseID, comId);
            if (onlyAddon)
            {
                items = items.Where(i => string.Compare(i.ItemType, "addon", true) == 0).ToList();
            }

            switch (level)
            {
                case 1:
                    itemInfos.AddRange(_pos.FilterGroup2(group1).Select(g => new ServiceItemSales
                    {
                        ID = g.ItemG2ID,
                        KhmerName = g.Name,
                        Image = g.Images,
                        Group1 = g.ItemG1ID,
                        Group2 = g.ItemG2ID,
                        Group3 = 1,
                        Level = level + 1
                    }));

                    itemInfos.AddRange(items.Where(i => i.Group1 == group1
                        && _dataContext.ItemGroup2.Any(g => g.ItemG2ID == i.Group2 && string.Compare(g.Name, "None", true) == 0)));
                    break;
                case 2:
                    itemInfos.AddRange(_pos.FilterGroup3(group1, group2).Select(g => new ServiceItemSales
                    {
                        ID = g.ID,
                        KhmerName = g.Name,
                        Image = g.Images,
                        Group1 = g.ItemG1ID,
                        Group2 = g.ItemG2ID,
                        Group3 = g.ID,
                        Level = level + 1
                    }));

                    itemInfos.AddRange(items.Where(i => i.Group1 == group1 && i.Group2 == group2
                        && _dataContext.ItemGroup3.Any(g => g.ID == i.Group3 && string.Compare(g.Name, "None", true) == 0)));
                    break;
                case 3:
                    itemInfos.AddRange(items.Where(i => i.Group1 == group1 && i.Group2 == group2 && i.Group3 == group3));
                    break;
            }
            return await Task.FromResult(itemInfos);
        }

        public bool VerifyUserAcess(UserCredentials creds)
        {
            return _userModule.VerifyUserAccess(creds);
        }

        public string Encrypt(string clearText)
        {
            if (clearText != null)
            {
                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using MemoryStream ms = new();
                    using (CryptoStream cs = new(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
                return clearText;
            }
            else
            {
                return clearText;
            }
        }

        public async Task<UserAccount> FindUserAsync(int userId)
        {
            var user = _userModule.FindById(userId);
            return await Task.FromResult(user);
        }

        private async Task<List<SelectListItem>> GetRemarkDiscountItemsAsync()
        {
            SelectListItem empItem = new()
            {
                Value = "0",
                Text = "--Select--"
            };
            var data = await _dataContext.RemarkDiscounts.Where(i => i.Active).Select(i => new SelectListItem
            {
                Text = i.Remark,
                Value = i.ID.ToString()
            }).ToListAsync();
            data.Insert(0, empItem);
            return data;
        }


        public async Task<Order> CreateOrderAsync(UserAccount user, int tableId, int customerId = 0)
        {
            var setting = await GetUserSettingAsync(user.ID);
            var setRate = await _dataContext.ExchangeRates.Where(c => c.CurrencyID == setting.LocalCurrencyID).Select(c => c.SetRate).FirstOrDefaultAsync();
            var tax = await _dataContext.Tax.FirstOrDefaultAsync(t => !t.Delete) ?? new Tax();
            int _pricelistId = setting.PriceListID;
            int _customerId = customerId > 0 ? customerId : setting.CustomerID;
            var customer = FindCustomerAsync(_customerId).Result;
            var table = _dataContext.Tables.FirstOrDefault(i => !i.Delete && i.ID == tableId) ?? new Table();

            if (table.IsTablePriceList) _pricelistId = table.PriceListID;
            //While using customer pricelist
            if (setting.IsCusPriceList && _customerId > 0)
            {
                _pricelistId = customer.PriceListID;
            }
            var taxRate = setting.VatAble ? tax.Rate : 0;
            var priceList = await GetPriceListAsync(user.ID, _pricelistId);
            var exRate = await _dataContext.ExchangeRates.Where(c => c.CurrencyID == priceList.CurrencyID).FirstOrDefaultAsync() ?? new ExchangeRate();
            string queueNo = _pos.CreateQueueNumber(setting);

            Order order = new()
            {
                OrderID = 0,
                OrderNo = "Order-" + queueNo,
                TableID = tableId,
                ReceiptNo = "N/A",
                QueueNo = queueNo,
                DateIn = DateTime.Parse(DateTime.Today.ToString("yyyy/MM/dd")),
                DateOut = DateTime.Parse(DateTime.Today.ToString("yyyy/MM/dd")),
                TimeIn = DateTime.Now.ToString("hh:mm tt"),
                TimeOut = DateTime.Now.ToString("hh:mm tt"),
                WaiterID = 1,
                UserOrderID = setting.UserID,
                UserDiscountID = setting.UserID,
                CustomerID = customer.ID,
                Customer = customer,
                CustomerCode = customer.Code,
                CustomerName = customer.Name,
                CustomerCount = 1,
                PriceListID = priceList.ID,
                LocalCurrencyID = setting.LocalCurrencyID,
                LocalSetRate = setRate,
                SysCurrencyID = setting.SysCurrencyID,
                ExchangeRate = exRate.Rate,
                WarehouseID = setting.WarehouseID,
                BranchID = setting.BranchID,
                CompanyID = setting.CompanyID,
                Sub_Total = 0,
                DiscountRate = 0,
                DiscountValue = 0,
                TypeDis = "Percent",
                TaxRate = taxRate,
                TaxValue = 0,
                GrandTotal = 0,
                GrandTotal_Sys = 0,
                Received = 0,
                Change = 0,
                DisplayRate = 0,
                PLCurrencyID = priceList.CurrencyID,
                Currency = priceList.Currency,
                PLRate = exRate.Rate,
                GrandTotal_Display = 0,
                Change_Display = 0,
                PaymentMeansID = setting.PaymentMeansID,
                CheckBill = 'N',
                OrderDetail = new List<OrderDetail>(),
                Freights = new List<FreightReceipt>(),
                RemarkDiscountID = 0,
                AppliedAmount = 0,
                BuyXAmGetXDisRate = 0,
                BuyXAmGetXDisType = TypeDiscountBuyXAmountGetXDiscount.Rate,
                BuyXAmGetXDisValue = 0,
                BuyXAmountGetXDisID = 0,
                Cancel = false,
                CardMemberDiscountRate = 0,
                CardMemberDiscountValue = 0,
                ChangeCurrencies = new List<DisplayPayCurrencyModel>(),
                ChangeCurrenciesDisplay = "",
                CurrencyDisplay = "",
                Delete = false,
                DisplayPayOtherCurrency = new List<DisplayPayCurrencyModel>(),
                FreightAmount = 0,
                GrandTotalCurrencies = new List<DisplayPayCurrencyModel>(),
                GrandTotalCurrenciesDisplay = "",
                GrandTotalOtherCurrencies = new List<DisplayPayCurrencyModel>(),
                GrandTotalOtherCurrenciesDisplay = "",
                OtherPaymentGrandTotal = 0,
                PaymentType = PaymentType.None,
                PromoCodeDiscRate = 0,
                PromoCodeDiscValue = 0,
                PromoCodeID = 0,
                Reason = "",
                Remark = "",
                SeriesDID = 0,
                SeriesID = 0,
                Status = OrderStatus.Sending,
                TaxGroupID = 0,
                TaxOption = setting.TaxOption,
                Tip = 0,
                TitleNote = "",
                VehicleID = 0,
                RefNo = ""
            };
            return order;
        }

        public async Task<List<Order>> GetOrdersAsync(int tableId, int branchId, int comId)
        {
            var orders = await _pos.GetUserOrders(branchId).Where(o => o.TableID == tableId).ToListAsync();
            if (orders.Count > 0)
            {
                foreach (var i in orders)
                {
                    var customer = _dataContext.BusinessPartners.Find(i.CustomerID) ?? new BusinessPartner();
                    // i.Customer = customer;
                    i.CustomerName = customer.Name;
                    i.CustomerCode = customer.Code;
                    // var ods = _dataContext.OrderDetail.Where(od => od.OrderID == i.OrderID).ToList();
                    //    var odsSerialOrBatch = ods.Where(sb => sb.IsBatchNo || sb.IsSerialNumber).ToList();
                    // GetSerialOrBatchOrder(i);
                    // _dataProp.DataProperty(ods, comId, "ItemID", "AddictionProps");
                }
            }
            return orders;
        }

        public void GetSerialOrBatchOrder(Order order)
        {
            if (order == null) return;
            if (!order.OrderDetail.Any()) return;
            List<SerialNumber> serialNumbers = new();
            foreach (var i in order.OrderDetail)
            {
                if (i.IsSerialNumber)
                {
                    var serialNumberUnselected = _POSSerialBatch.GetSerialDetialsAsync(i.ItemID, order.WarehouseID).Result;
                    var snsd = _dataContext.SerialNumberSelectedDetails.Where(snd => snd.ItemID == i.ItemID).ToList();
                    var serialNumber = (from sn in _dataContext.SerialNumbers.Where(sn => sn.ItemID == i.ItemID)
                                        join sns in _dataContext.SerialNumberSelecteds on sn.Id equals sns.SerialNumberID
                                        select new SerialNumber
                                        {
                                            Barcode = sn.Barcode,
                                            Cost = sn.Cost,
                                            BaseQty = sn.BaseQty,
                                            BpId = sn.BpId,
                                            ItemID = sn.ItemID,
                                            Direction = sn.Direction,
                                            Id = sn.Id,
                                            ItemCode = sn.ItemCode,
                                            ItemName = sn.ItemName,
                                            LineID = sn.LineID,
                                            OpenQty = sn.OpenQty,
                                            OrderDetailID = sn.OrderDetailID,
                                            Qty = sn.Qty,
                                            SaleID = sn.SaleID,
                                            SerialNumberSelected = new SerialNumberSelected
                                            {
                                                SerialNumberID = sn.Id,
                                                Id = sns.Id,
                                                SerialNumberSelectedDetails = snsd.Where(i => i.SerialNumberSelectedID == sns.Id).ToList(),
                                                TotalSelected = sns.TotalSelected
                                            },
                                            TotalSelected = sn.TotalSelected,
                                            SerialNumberUnselected = serialNumberUnselected,
                                            UomID = sn.UomID,
                                            WareId = sn.WareId,
                                            WhsCode = sn.WhsCode,
                                        }).FirstOrDefault();
                    //i.SerialNumbers = serialNumber;
                    if (serialNumber != null) serialNumbers.Add(serialNumber);
                }
            }
            order.SerialNumbers = serialNumbers;
        }

        public Order FindOrder(int orderId)
        {
            return _dataContext.Order.Include(od => od.OrderDetail).FirstOrDefault(o => o.OrderID == orderId);
        }

        public async Task<List<ServiceItemSales>> GetSaleItemsAsync(int userId, int orderId, string keyword = "")
        {
            var user = _userModule.FindById(userId);
            var setting = await GetUserSettingAsync(userId);
            var order = FindOrder(orderId);
            int pricelistId = setting.PriceListID;
            if (order != null)
            {
                pricelistId = order.PriceListID;
            }

            // var saleItems = await _pos.GetItemMasterDatasAsync(pricelistId, setting.WarehouseID, user.Company.ID);
            var saleItems = _pos.GetSaleItems(pricelistId, setting.WarehouseID);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                keyword = RawWord(keyword);
                saleItems = saleItems.Where(s => RawWord(s.KhmerName).Contains(keyword, ignoreCase)
                        || RawWord(s.EnglishName).Contains(keyword, ignoreCase)
                        || RawWord(s.Barcode).Contains(keyword, ignoreCase));
            }
            return await saleItems.ToListAsync();
        }

        public async Task<ServiceItemSales> FindSaleItemAsync(int saleItemId, int orderId, string keyword = "")
        {
            var saleItems = await GetSaleItemsAsync(_userModule.GetUserId(), orderId, keyword);
            return saleItems.FirstOrDefault(s => s.ID == saleItemId) ?? new ServiceItemSales();
        }

        public async Task<OrderDetail> NewOrderDetailAsync(int saleItemId, int orderId)
        {
            var saleItem = await FindSaleItemAsync(saleItemId, orderId);
            return await CreateOrderDetailAsync(saleItem, orderId);
        }

        private List<SelectListItem> SelectPrinters(int printerId)
        {
            var printers = _dataContext.PrinterNames.Where(p => !p.Delete)
            .Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = p.Name,
                Selected = p.ID == printerId
            }).ToList();
            return printers;
        }

        private async Task<List<SelectListItem>> SelectItemUoMsAsync(int groupUomId, int itemUomId)
        {
            var itemUoMs = await GetItemUoMsAsync(groupUomId);
            return itemUoMs.Select(uom => new SelectListItem
            {
                Value = uom.UomID.ToString(),
                Text = uom.Name,
                Selected = uom.UomID == itemUomId
            }).ToList();
        }

        public async Task<List<ItemUoM>> GetItemUoMsAsync(int groupUoMId = 0)
        {
            var itemUoMs = from gdu in _dataContext.GroupDUoMs.Where(gd => !gd.Delete)
                           join uom in _dataContext.UnitofMeasures.Where(u => !u.Delete)
                           on gdu.UoMID equals uom.ID
                           select new ItemUoM
                           {
                               UomID = uom.ID,
                               GroupUomID = gdu.GroupUoMID,
                               Name = uom.Name
                           };
            if (groupUoMId > 0)
            {
                itemUoMs = itemUoMs.Where(gd => gd.GroupUomID == groupUoMId);
            }
            return await itemUoMs.ToListAsync();
        }

        public async Task<IEnumerable<ItemComment>> GetItemCommentsAsync()
        {
            var comments = await _dataContext.ItemComment.Where(c => !c.Deleted).ToListAsync();
            return comments;
        }

        public async Task<ItemComment> SaveItemCommentAsync(ItemComment comment, ModelStateDictionary modelState)
        {
            try
            {
                if (comment == null || string.IsNullOrWhiteSpace(comment?.Description))
                {
                    modelState.AddModelError("Description", "Comment description not allowed empty.");
                }

                if (modelState.IsValid)
                {
                    var _comment = await _dataContext.ItemComment.FirstOrDefaultAsync(c => IsNotEmptyEqual(c.Description, comment.Description, true));
                    if (_comment == null)
                    {
                        _dataContext.ItemComment.Update(comment);
                        await _dataContext.SaveChangesAsync();
                        return comment;
                    }
                    else
                    {
                        _comment.Description = comment.Description;
                        _comment.Deleted = false;
                        await _dataContext.SaveChangesAsync();
                        return _comment;
                    }
                }
            }
            catch (Exception ex)
            {
                modelState.AddModelError("Exception", ex.Message);
                _logger.LogError(ex.Message);
            }
            return comment;
        }

        public async Task<ItemComment> DeleteItemCommentAsync(int commentId, ModelStateDictionary modelState)
        {
            var itemComment = await _dataContext.ItemComment.FindAsync(commentId);
            if (itemComment == null)
            {
                modelState.AddModelError("Description", "The item comment does not exist.");
            }

            if (modelState.IsValid)
            {
                itemComment.Deleted = true;
                await _dataContext.SaveChangesAsync();
            }
            return itemComment;
        }

        public async Task<DisplayCurrencyModel> GetDisplayCurrencyAsync(int userId, int priceListId)
        {
            PriceLists priceList = await GetPriceListAsync(userId, priceListId);
            var cur = await _dataContext.Currency.FirstOrDefaultAsync(i => i.ID == priceList.CurrencyID) ?? new Currency();
            var dcs = await _dataContext.DisplayCurrencies.Where(i => i.PriceListID == priceListId).ToListAsync();
            var altCurrencies = await (from c in _dataContext.Currency.Where(i => !i.Delete)
                                       let plBasedDc = dcs.FirstOrDefault(dc => dc.PriceListID == priceList.ID && c.ID == dc.AltCurrencyID) ?? new DisplayCurrency()
                                       select new DisplayCurrencyModel
                                       {
                                           ID = plBasedDc.ID,
                                           AltCurrencyID = plBasedDc.AltCurrencyID == 0 ? c.ID : plBasedDc.AltCurrencyID,
                                           AltCurrency = c.Description,
                                           BaseCurrency = cur.Description,
                                           BaseCurrencyID = cur.ID,
                                           Rate = plBasedDc.PLDisplayRate,
                                           AltRate = plBasedDc.DisplayRate,
                                       }).ToListAsync();
            var displayCurrency = await (from pl in _dataContext.PriceLists
                                         join dc in _dataContext.DisplayCurrencies on pl.ID equals dc.AltCurrencyID
                                         where dc.PriceListID == priceList.ID
                                         select new DisplayCurrencyModel
                                         {
                                             ID = pl.ID,
                                             BaseCurrencyID = priceList.CurrencyID,
                                             AltCurrencyID = dc.AltCurrencyID,
                                             BaseCurrency = priceList.Currency.Description,
                                             AltCurrency = pl.Currency.Description,
                                             Rate = dc.DisplayRate,
                                             DisplayAltCurrency = altCurrencies
                                         }).FirstOrDefaultAsync() ?? new DisplayCurrencyModel();
            return displayCurrency;
        }

        public async Task<List<DisplayPayCurrencyModel>> GetDisplayPayCurrenciesAsync(int userId, int priceListId)
        {
            PriceLists priceList = await GetPriceListAsync(userId, priceListId);
            var cur = await _dataContext.Currency.FirstOrDefaultAsync(i => i.ID == priceList.CurrencyID) ?? new Currency();
            var dcs = await _dataContext.DisplayCurrencies.Where(i => i.PriceListID == priceListId).ToListAsync();
            var dc = dcs.FirstOrDefault(i => i.IsActive) ?? new DisplayCurrency();
            var altCurrencies = await (from ex in _dataContext.ExchangeRates.Where(ex => !ex.Delete)
                                       join c in _dataContext.Currency.Where(c => !c.Delete) on ex.CurrencyID equals c.ID
                                       let plBasedDc = dcs.FirstOrDefault(dc => dc.PriceListID == priceList.ID && c.ID == dc.AltCurrencyID) ?? new DisplayCurrency()
                                       select new DisplayPayCurrencyModel
                                       {
                                           LineID = $"{DateTime.Now.Ticks}{c.ID}",
                                           AltCurrency = c.Description,
                                           BaseCurrency = cur.Description,
                                           Amount = 0,
                                           AltAmount = 0,
                                           Rate = plBasedDc.PLDisplayRate,
                                           AltRate = plBasedDc.DisplayRate,
                                           LCRate = (decimal)ex.SetRate,
                                           SCRate = (decimal)ex.Rate,
                                           BaseCurrencyID = cur.ID,
                                           AltCurrencyID = c.ID,
                                           AltSymbol = c.Symbol,
                                           BaseSymbol = cur.Symbol,
                                           IsLocalCurrency = dc.AltCurrencyID == 0 ? c.ID == cur.ID : c.ID == dc.AltCurrencyID,
                                           IsShowCurrency = plBasedDc.IsShowCurrency,
                                           IsActive = plBasedDc.IsActive,
                                           IsShowOtherCurrency = plBasedDc.IsShowOtherCurrency,
                                           DecimalPlaces = plBasedDc.DecimalPlaces,
                                       }).ToListAsync();

            return altCurrencies;
        }


        private async Task<PriceLists> GetPriceListAsync(int userId, int priceListId = 0)
        {
            var userSetting = await GetUserSettingAsync(userId);
            int pricelistId = priceListId;
            if (priceListId <= 0)
            {
                pricelistId = userSetting.PriceListID;
            }

            return await _dataContext.PriceLists.Include(p => p.Currency)
                     .FirstOrDefaultAsync(p => p.ID == pricelistId)
                     ?? new PriceLists
                     {
                         Currency = new Currency()
                     };
        }

        private static OrderDetail CheckTypeofService(OrderDetail od, Table table)
        {
            if (string.Compare(od.ItemType, "service", true) == 0)
            {
                string[] times = table.Time.Split(":");
                double h = int.Parse(times[0]);
                double m = int.Parse(times[1]);
                od.Qty = double.Parse((h + (m / 60) + 0.001).ToString("N3"));
                switch (od.TypeDis.ToLower())
                {
                    case "percent":
                        od.Total = od.Qty * od.UnitPrice * (1 - (od.DiscountRate / 100));
                        od.DiscountValue = od.Qty * od.UnitPrice * od.DiscountRate / 100;
                        break;
                    case "cash":
                        od.Total = (od.Qty * od.UnitPrice - od.DiscountRate);
                        od.DiscountValue = od.DiscountRate;
                        break;
                }
            }
            return od;
        }

        public List<SelectListItem> SelectTaxOptions(int taxGroupId = 0)
        {
            return EnumHelper.ToDictionary<TaxOptions>().Select(e => new SelectListItem
            {
                Value = e.Key.ToString(),
                Text = e.Value,
                Selected = e.Key == taxGroupId
            }).ToList();
        }
        public List<SelectListItem> SelectReceiptTemplate(int receiptGroupId = 0)
        {
            return EnumHelper.ToDictionary<PrintReceiptOption>().Select(s => new SelectListItem
            {
                Value = s.Key.ToString(),
                Text = s.Value,
                Selected = s.Key == receiptGroupId
            }).ToList();
        }
        public List<SelectListItem> SelectQueueOptions(int queueGroupId = 0)
        {
            return EnumHelper.ToDictionary<QueueOptions>().Select(s => new SelectListItem
            {
                Value = s.Key.ToString(),
                Text = s.Value,
                Selected = s.Key == queueGroupId
            }).ToList();
        }
        public async Task<SettingModel> GetSettingViewModelAsync(int userId = 0, string redirectUrl = "")
        {
            var setting = await GetUserSettingAsync(userId);
            var customers = await _dataContext.BusinessPartners.Where(c => !c.Delete && string.Compare(c.Type, "customer", true) == 0).ToListAsync();
            var priceLists = await _dataContext.PriceLists.Where(p => !p.Delete).ToListAsync();
            var warehouses = await _dataContext.Warehouses.Where(w => w.BranchID == setting.BranchID && !w.Delete).ToListAsync();
            var systemTypes = await _dataContext.SystemType.Where(s => s.Status).ToListAsync();
            var paymentMeans = await _dataContext.PaymentMeans.Where(p => !p.Delete).ToListAsync();
            var printerNames = await _dataContext.PrinterNames.Where(p => !p.Delete).ToListAsync();
            var docType = await _dataContext.DocumentTypes.FirstOrDefaultAsync(w => w.Code == "SP");
            var series = await _dataContext.Series.Where(w => w.DocuTypeID == docType.ID && !w.Lock).ToListAsync();
            var taxGroups = await GetTaxGroupsAsync();
            var taxGroup = taxGroups.FirstOrDefault() ?? new TaxGroupModel();
            return new SettingModel
            {
                Setting = setting,
                Customers = new SelectList(customers, "ID", "Name", setting.CustomerID),
                PriceLists = priceLists.Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),
                    Text = p.Name,
                    Selected = p.ID == setting.PriceListID
                    //Disabled = p.ID != Setting.PriceListID && _dataContext.Order.Where(o => !o.Delete).Count() > 0
                }).ToList(),
                PrinterNames = new SelectList(printerNames, "Name", "Name", setting.Printer),
                Warehouses = new SelectList(warehouses, "ID", "Name", setting.WarehouseID),
                PaymentMeans = new SelectList(paymentMeans, "ID", "Type", setting.PaymentMeansID),
                Series = new SelectList(series, "ID", "Name", setting.SeriesID),
                RedirectUrl = redirectUrl,
                SeriesPS = GetSeriesPS("US"),
                TaxGroups = SelectTaxOptions((int)setting.TaxOption),
                QueueGroups = SelectQueueOptions((int)setting.QueueOption),
                ReceiptTemplateGroups = SelectReceiptTemplate((int)setting.PrintReceiptOption),
                Taxes = taxGroups.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ID.ToString(),
                    Selected = setting.Tax == 0 ? i.ID == taxGroup.ID : i.ID == setting.Tax
                }).ToList(),
            };
        }
        private List<SeriesInPurchasePoViewModel> GetSeriesPS(string code)
        {
            var seriesPS = (from dt in _dataContext.DocumentTypes.Where(i => i.Code == code)
                            join sr in _dataContext.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                                DocumentTypeID = sr.DocuTypeID,
                            }).ToList();
            return seriesPS;
        }
        public async Task<GeneralSetting> GetUserSettingAsync(int userId)
        {
            var setting = await _dataContext.GeneralSettings.FirstOrDefaultAsync(g => g.UserID == userId) ?? CreateUserSetting(userId);
            var taxGroups = await GetTaxGroupsAsync();
            var taxGroup = taxGroups.FirstOrDefault() ?? new TaxGroupModel();
            setting.Tax = setting.Tax == 0 ? taxGroup.ID : setting.Tax;
            setting.SortBy = new Dictionary<string, object>
            {
                {"Field", "Code" },
                {"Desc", false }
            };
            return setting;
        }

        private GeneralSetting CreateUserSetting(int userId = 0)
        {
            var user = FindUserAsync(userId).Result;
            var setting = new GeneralSetting
            {
                UserID = userId,
                BranchID = user.BranchID,
                CompanyID = (int)user.CompanyID
            };

            var company = _dataContext.Company.Find(user.CompanyID);
            if (company != null)
            {
                setting.LocalCurrencyID = company.LocalCurrencyID;
                setting.SysCurrencyID = company.SystemCurrencyID;
            }
            return setting;
        }
        private static string SetCommentItem(string comment)
        {
            string _comment = "";
            if (!string.IsNullOrEmpty(comment))
            {
                string[] comSplit = comment.Split("\n");

                foreach (var (item, index) in comSplit.ToList().Select((value, i) => (value, i)))
                {
                    var slash = "";
                    int length = comSplit.Length - 1;

                    if (index == length) slash = "";
                    else slash = "\n";

                    _comment += $"{item} {slash}";
                }
            }
            return _comment;
        }

        private async Task<Order> CheckedOrderAsync(Order order, string printType = null, List<SerialNumber> serials = null, List<BatchNo> batches = null)
        {
            var dis = _dataContext.DisplayCurrencies.FirstOrDefault(x => x.PriceListID == order.PriceListID && x.AltCurrencyID == order.PLCurrencyID) ?? new DisplayCurrency();
            var setting = await GetUserSettingAsync(order.UserOrderID);
            double total = 0;
            double totalNoTax = 0;
            //order.OrderDetail = order.OrderDetail.Where(i => i.Qty > 0).ToList();
            foreach (var od in order.OrderDetail)
            {
                od.DiscountValue = od.Qty * od.UnitPrice * od.DiscountRate / 100;
                od.Total = od.Qty * od.UnitPrice * (1 - (od.DiscountRate / 100));
                totalNoTax += od.Total;
                if (setting.TaxOption == TaxOptions.Exclude)
                {
                    od.Total += (double)od.TaxValue;
                }

                od.Total_Sys = od.Total * order.PLRate;
                total += _utility.ToDouble(_utility.ToCurrency(od.Total, dis.DecimalPlaces));
                //if (string.IsNullOrEmpty(od.CommentUpdate)) od.CommentUpdate = od.Comment;
                //if (order.OrderID == 0) od.Comment = SetCommentItem(od.CommentUpdate);
                od.CommentUpdate = SetCommentItem(od.CommentUpdate);
                if (serials != null && printType != "Pay")
                {
                    if (serials.Any())
                    {
                        od.SerialNumbers = serials.Where(i => i.ItemID == od.ItemID).ToList();
                    }
                }
                if (serials != null && printType != "Pay")
                {
                    if (batches.Any())
                    {
                        od.BatchNos = batches.Where(i => i.ItemID == od.ItemID).ToList();
                    }
                }
            }

            order.Sub_Total = total;
            double alltotaldiscount = order.DiscountRate + (double)order.BuyXAmGetXDisRate + order.PromoCodeDiscRate + (double)order.CardMemberDiscountRate;
            order.DiscountValue = totalNoTax * alltotaldiscount / 100;
            order.GrandTotal = total * (1 - alltotaldiscount / 100);
            if (setting.TaxOption == TaxOptions.InvoiceVAT)
            {
                var vatValue = (total - order.DiscountValue) * order.TaxRate / 100;
                order.GrandTotal = vatValue + (total * (1 - alltotaldiscount / 100));
            }

            //   order.GrandTotal=  _fnModule.ToCurrency(item.AltAmount, item.DecimalPlaces);
            order.GrandTotal += Convert.ToDouble(order.FreightAmount);
            order.GrandTotal_Sys = order.GrandTotal * order.PLRate;
            order.Change = order.Received - order.GrandTotal;
            order.GrandTotalCurrenciesDisplay = SetGrandTotalCurrencies(order);
            order.ChangeCurrenciesDisplay = SetChangeTotalCurrencies(order);
            order.GrandTotalOtherCurrenciesDisplay = SetGrandTotalOtherCurrencies(order);
            order.ReceiptNo = order.ReceiptNo ?? string.Empty;
            return order;
        }

        private async Task<PendingVoidItem> CheckedPendingVoidItemAsync(PendingVoidItem order)
        {
            var setting = await GetUserSettingAsync(order.UserOrderID);
            double total = 0;
            double vatValue = 0;
            order.PendingVoidItemDetails.ForEach(od =>
            {
                od.DiscountValue = od.Qty * od.UnitPrice * od.DiscountRate / 100;
                od.Total = od.Qty * od.UnitPrice * (1 - (od.DiscountRate / 100));
                if (setting.TaxOption == TaxOptions.Exclude)
                {
                    od.Total += (double)od.TaxValue;
                }

                od.Total_Sys = od.Total * order.PLRate;
                total += od.Total;
            });

            order.Sub_Total = total;
            double alltotaldiscount = order.DiscountRate + (double)order.BuyXAmGetXDisRate + order.PromoCodeDiscRate + (double)order.CardMemberDiscountRate;
            order.DiscountValue = total * alltotaldiscount / 100;
            if (setting.TaxOption == TaxOptions.InvoiceVAT)
            {
                vatValue = (total - order.DiscountValue) * order.TaxRate / 100;
            }
            order.GrandTotal = vatValue + (total * (1 - alltotaldiscount / 100));
            order.GrandTotal_Sys = order.GrandTotal * order.PLRate;
            order.Change = order.Received - order.GrandTotal;
            return order;
        }

        private string SetGrandTotalOtherCurrencies(Order receipt)
        {
            string grandTotalCurrenciesDisplay = "";
            if (receipt.GrandTotalOtherCurrencies == null) { return ""; }
            var grandTotalOtherCurrencies = receipt?.GrandTotalOtherCurrencies?.ToList();
            if (receipt.GrandTotalOtherCurrencies != null)
            {
                foreach (var (item, index) in grandTotalOtherCurrencies.Select((value, i) => (value, i)))
                {
                    var slash = "";
                    int length = grandTotalOtherCurrencies.Count - 1;

                    if (index == length) slash = "";
                    else slash = "\n";
                    string altamount = _fnModule.ToCurrency(item.AltAmount, item.DecimalPlaces);
                    grandTotalCurrenciesDisplay += $"{item.AltCurrency} {altamount} {slash}";
                }
            }
            return grandTotalCurrenciesDisplay;
        }
        private string SetGrandTotalCurrencies(Order receipt)
        {
            string grandTotalCurrenciesDisplay = "";
            if (receipt.GrandTotalCurrencies == null) { return ""; }
            var grandTotalCurrencies = receipt.GrandTotalCurrencies;
            foreach (var (item, index) in grandTotalCurrencies.Select((value, i) => (value, i)))
            {
                var slash = "";
                int length = grandTotalCurrencies.Count - 1;

                if (index == length) slash = "";
                else slash = "\n";
                decimal amount = (decimal)receipt.GrandTotal * item.AltRate;
                string altamount = _fnModule.ToCurrency(amount, item.DecimalPlaces);
                grandTotalCurrenciesDisplay += $"{item.AltCurrency} {altamount} {slash}";
            }
            return grandTotalCurrenciesDisplay;
        }
        private string SetChangeTotalCurrencies(Order receipt)
        {
            string changeTotalCurrenciesDisplay = "";
            if (receipt.ChangeCurrencies == null) { return ""; }
            var changeCurrencies = receipt?.ChangeCurrencies?.ToList();
            foreach (var (item, index) in changeCurrencies.Select((value, i) => (value, i)))
            {
                var slash = "";
                int length = changeCurrencies.Count - 1;

                if (index == length) slash = "";
                else slash = "\n";
                decimal amount = (decimal)receipt.Change * item.AltRate;
                string altamount = _fnModule.ToCurrency(amount, item.DecimalPlaces);
                changeTotalCurrenciesDisplay += $"{item.AltCurrency} {altamount} {slash}";
            }
            return changeTotalCurrenciesDisplay;
        }

        public string SetGrandTotalCurrencies(CanRingMaster receipt)
        {
            string grandTotalCurrenciesDisplay = "";
            if (receipt.GrandTotalAndChangeCurrencies == null) { return ""; }
            var grandTotalCurrencies = receipt.GrandTotalAndChangeCurrencies;
            foreach (var (item, index) in grandTotalCurrencies.Select((value, i) => (value, i)))
            {
                var slash = "";
                int length = grandTotalCurrencies.Count - 1;

                if (index == length) slash = "";
                else slash = "\n";
                decimal amount = receipt.Total * item.AltRate;
                string altamount = _fnModule.ToCurrency(amount, item.DecimalPlaces);
                grandTotalCurrenciesDisplay += $"{item.AltCurrency} {altamount} {slash}";
            }
            return grandTotalCurrenciesDisplay;
        }
        public string SetChangeTotalCurrencies(CanRingMaster receipt)
        {
            string changeTotalCurrenciesDisplay = "";
            if (receipt.GrandTotalAndChangeCurrencies == null) { return ""; }
            var changeCurrencies = receipt?.GrandTotalAndChangeCurrencies?.ToList();
            foreach (var (item, index) in changeCurrencies.Select((value, i) => (value, i)))
            {
                var slash = "";
                int length = changeCurrencies.Count - 1;

                if (index == length) slash = "";
                else slash = "\n";
                decimal amount = receipt.Change * item.AltRate;
                string altamount = _fnModule.ToCurrency(amount, item.DecimalPlaces);
                changeTotalCurrenciesDisplay += $"{item.AltCurrency} {altamount} {slash}";
            }
            return changeTotalCurrenciesDisplay;
        }

        public PrintInvoice GetPrintInvoice(Order order, string printType)
        {
            var baseCurrency = _dataContext.DisplayCurrencies.FirstOrDefault(i => i.PriceListID == order.PriceListID && order.SysCurrencyID == i.AltCurrencyID) ?? new DisplayCurrency();
            var altCurrency = _dataContext.DisplayCurrencies.FirstOrDefault(i => i.PriceListID == order.PriceListID && i.IsActive) ?? new DisplayCurrency();
            var payment = _dataContext.PaymentMeans.FirstOrDefault(w => w.ID == order.PaymentMeansID);
            var customer = _dataContext.BusinessPartners
                .Where(bp => string.Compare(bp.Type, "Customer", true) == 0)
                .FirstOrDefault(bp => bp.ID == order.CustomerID);
            var receiptInfo = _dataContext.ReceiptInformation.FirstOrDefault(w => w.BranchID == order.BranchID) ?? new ReceiptInformation();
            var user = _userModule.FindById(order.UserOrderID);
            var table = _dataContext.Tables.Find(order.TableID);
            var receiptsByCustomer = _dataContext.Receipt.Where(r => r.CustomerID == order.CustomerID).ToList();
            var invoice = new PrintInvoice
            {
                CompanyName = user.Company.Name,
                BranchName = receiptInfo.Branch.Name,
                Logo = receiptInfo.Logo,
                Description = receiptInfo.KhmerDescription,
                Description2 = receiptInfo.EnglishDescription,
                Tel = receiptInfo.Tel1 ?? "",
                Tel2 = receiptInfo.Tel2 ?? "",
                Address = receiptInfo.Address,
                AppliedAmount = order.AppliedAmount.ToString(),
                Subtotal = order.Sub_Total.ToString(),
                DateIn = order.DateIn.ToString("dd-MM-yyyy") + " " + order.TimeIn,
                DateOut = order.DateOut.ToString("dd-MM-yyyy") + " " + order.TimeIn,
                DiscountRate = order.DiscountRate + "" + "%",
                DiscountValue = _fnModule.ToCurrency(order.DiscountValue, baseCurrency.DecimalPlaces),
                VatRate = _fnModule.ToCurrency(order.TaxRate, baseCurrency.DecimalPlaces) + "%",
                VatValue = _fnModule.ToCurrency(order.TaxValue, baseCurrency.DecimalPlaces),
                ReceivedAmount = _fnModule.ToCurrency(order.Received, baseCurrency.DecimalPlaces),
                GrandTotal = _fnModule.ToCurrency(order.GrandTotal, baseCurrency.DecimalPlaces),
                GrandTotalSys = _fnModule.ToCurrency(order.GrandTotal_Display, altCurrency.DecimalPlaces),
                ChangedAmount = _fnModule.ToCurrency(order.Change, baseCurrency.DecimalPlaces),
                ChangedAmountSys = order.CurrencyDisplay + " " + _fnModule.ToCurrency(order.Change_Display, altCurrency.DecimalPlaces),
                CustomerInfo = customer.Code + '/' + customer.Name + '/' + customer.Phone + '/' + customer.Address + '/' + customer.Email,
                PaymentMeans = payment?.Type,
                Remark = order.Remark,
                TotalItemQty = _fnModule.ToCurrency(order.OrderDetail.Sum(od => od.Qty), baseCurrency.DecimalPlaces),
                Freights = _fnModule.ToCurrency(order.FreightAmount, baseCurrency.DecimalPlaces),
                PrintType = printType,
                UserOrder = user.Username,
                TableName = table?.Name,
                OrderNo = order.OrderNo,
                QueueNo = order.QueueNo,
                ReceiptNo = order.ReceiptNo,
                ReceiptCount = receiptsByCustomer.Count
            };

            invoice.LineItems = order.OrderDetail.Where(od => od.Qty != 0)?
            .Select(od => new PrintLineItem
            {
                LineID = od.LineID,
                ItemID = od.ItemID,
                ItemName = od.KhmerName,
                ItemName2 = od.EnglishName,
                Comment = od.Comment,
                Qty = $"{od.Qty}",
                UoM = od.Uom,
                PrintQty = $"{od.PrintQty} {od.Uom}",
                UnitPrice = _fnModule.ToCurrency(od.UnitPrice, baseCurrency.DecimalPlaces),
                DiscountRate = od.DiscountRate + "" + "%",
                DiscountValue = _fnModule.ToCurrency(od.DiscountValue, baseCurrency.DecimalPlaces),
                TaxRate = _fnModule.ToCurrency(od.TaxRate, baseCurrency.DecimalPlaces) + "%",
                TaxValue = _fnModule.ToCurrency(od.TaxValue, baseCurrency.DecimalPlaces),
                Total = _fnModule.ToCurrency(od.Total, baseCurrency.DecimalPlaces),
                PrinterName = od.ItemPrintTo,
                ItemType = od.ItemType
            }).ToList();
            return invoice;
        }

        public ModelMessage GetCardMemberDetial(string cardNumber, decimal grandTotal, int pricelistId, ModelStateDictionary modelState)
        {
            ModelMessage msg = new();
            var data = (from card in _dataContext.CardMembers.Where(i => i.Code == cardNumber)
                        join bp in _dataContext.BusinessPartners on card.ID equals bp.CardMemberID
                        select new CardMember
                        {
                            ID = card.ID,
                            Active = card.Active,
                            Code = card.Code,
                            TypeCardID = card.TypeCardID,
                            Customer = bp,
                            Description = card.Description,
                            Name = card.Name,
                            ExpireDateTo = card.ExpireDateTo
                        }).FirstOrDefault();

            if (data == null)
            {
                modelState.AddModelError("Message", "Card Not Found");
                var cardMember = new CardMember
                {
                    Customer = new BusinessPartner()
                };
                msg.AddItem(cardMember, "data");
                msg.Reject();
            }
            else
            {
                if (data.ExpireDateTo < DateTime.Today)
                {
                    modelState.AddModelError("Message", "Card is expired!");
                    msg.Reject();
                }
                else
                {
                    if (data.Customer == null)
                    {
                        modelState.AddModelError("Message", $"Customer did not link with Card number \"{data.Code}\"");
                        msg.Reject();
                    }
                    else
                    {
                        if (data.Customer.PriceListID != pricelistId)
                        {
                            var cusPriceList = _dataContext.PriceLists.Find(data.Customer.PriceListID) ?? new PriceLists();
                            var curPriceList = _dataContext.PriceLists.Find(pricelistId) ?? new PriceLists();
                            modelState.AddModelError("Message", $"Customer's Price List \"{cusPriceList.Name}\" does not match with current Price List \"{curPriceList.Name}\"");
                            msg.Reject();
                        }
                        else
                        {
                            if (data.Customer.Balance < grandTotal)
                            {
                                msg.Alert();
                                modelState.AddModelError("Message", "Customer's balance is less than GrandTotal. Do you want to continue?");
                            }
                        }
                    }
                }
                msg.AddItem(data, "data");
            }
            return msg;
        }

        public ModelMessage GetMemberCardDiscount(string cardNumber, int pricelistId, ModelStateDictionary modelState)
        {
            ModelMessage msg = new();
            var data = (from card in _dataContext.CardMembers.Where(i => i.Code == cardNumber)
                        join cardType in _dataContext.TypeCards on card.TypeCardID equals cardType.ID
                        join bp in _dataContext.BusinessPartners on card.ID equals bp.CardMemberID
                        select new CardMember
                        {
                            ID = card.ID,
                            Active = card.Active,
                            Code = card.Code,
                            TypeCardID = card.TypeCardID,
                            Customer = bp,
                            Description = card.Description,
                            Name = card.Name,
                            TypeDiscount = cardType.TypeDiscount,
                            Discount = cardType.Discount
                        }).FirstOrDefault();
            bool checking = false;
            if (data == null)
            {
                modelState.AddModelError("data", "Card Not Found");
                checking = true;
                var cardMember = new CardMember
                {
                    Customer = new BusinessPartner()
                };
                msg.AddItem(cardMember, "data");
            }
            else
            {
                if (data.Customer == null)
                {
                    modelState.AddModelError("customer", $"Customer did not link with Card number \"{data.Code}\"");
                    checking = true;
                }
                else
                {
                    if (data.Customer.PriceListID != pricelistId)
                    {
                        var cusPriceList = _dataContext.PriceLists.Find(data.Customer.PriceListID) ?? new PriceLists();
                        var curPriceList = _dataContext.PriceLists.Find(pricelistId) ?? new PriceLists();
                        modelState.AddModelError("PriceListID", $"Customer's Price List \"{cusPriceList.Name}\" does not match with current Price List \"{curPriceList.Name}\"");
                        checking = true;
                    }
                }
                if (!checking)
                {
                    modelState.AddModelError("card", $"Card found");
                    msg.AddItem(data, "data");
                    msg.Approve();
                }
            }
            return msg.Bind(modelState);
        }

        public async Task<ItemsReturnObj> SubmitOrderAsync(Order order, string printType, List<SerialNumber> serials, List<BatchNo> batches, string promocode = "")
        {
            var validOrder = await CheckedOrderAsync(order);
            ItemsReturnObj itemsReturns = new ItemsReturnObj
            {
                ItemsReturns = new List<ItemsReturn>(),
                ErrorMessages = new Dictionary<string, string>()
            };

            // if (string.Compare(printType, "pay", true) == 0)
            // {
            //     if (order.SeriesID <= 0 || order.SeriesDID <= 0)
            //     {
            //         itemsReturns.ErrorMessages.TryAdd("Series", "Series of order not found.");
            //         return itemsReturns;
            //     }
            // }


            if (!_clientPos.EnableExternalSync() && !_clientPos.EnableInternalSync())
            {
                itemsReturns = await CheckStockAsync(order, serials, batches, printType);
            }

            if (itemsReturns.ItemsReturns.Count > 0
                || itemsReturns.TypeOfSerialBatch != TypeOfSerialBatch.None
                || itemsReturns.ErrorMessages.Count > 0
            )
            {
                return itemsReturns;
            }

            itemsReturns = await _pos.SubmitOrderAsync(validOrder, printType, serials, batches, promocode);

            itemsReturns.ItemsReturns ??= new List<ItemsReturn>();
            return itemsReturns;
        }

        private List<PromoCodeDiscount> FindPromoCode(int priceListID)
        {
            var promo = _dataContext.PromoCodeDiscounts.Where(bx => bx.PriceListID == priceListID && bx.Active
                    && IsBetweenDate(bx.DateF, bx.DateT) && !bx.Used).ToList();
            return promo;
        }

        public async Task<PromoCodeDiscount> GetPromoCodeAsync(int priceListID, string Code)
        {
            var promocode = FindPromoCode(priceListID);
            var code = await _dataContext.PromoCodeDetails.Where(s => s.PromoCode == Code).FirstOrDefaultAsync();
            PromoCodeDiscount promocodediscount = new();
            if (code != null)
            {
                promocodediscount = _dataContext.PromoCodeDiscounts.Where(bx => bx.PriceListID == priceListID && bx.Active
                && IsBetweenDate(bx.DateF, bx.DateT) && !bx.Used && bx.ID == code.ID).FirstOrDefault();
            }
            return promocodediscount;
        }

        public void SaveOrder(Order order)
        {
            _pos.SaveOrder(order);
        }
        public void DeleteSavedOrder(int orderId, ModelStateDictionary modelState)
        {
            try
            {
                using var t = _dataContext.Database.BeginTransaction();
                var _order = _dataContext.Order.Find(orderId);
                if (_order != null)
                {
                    var _orderDetails = _dataContext.OrderDetail.Where(od => od.OrderID == _order.OrderID).ToList();
                    if (_orderDetails.Count > 0)
                    {
                        _dataContext.OrderDetail.RemoveRange(_orderDetails);
                    }

                    _dataContext.Order.Remove(_order);
                    _dataContext.SaveChanges();
                }
                t.Commit();
            }
            catch (Exception ex)
            {
                modelState.AddModelError("Exception", ex.Message);
                _logger.LogError(ex.Message);
            }
        }
        public bool SendReturnItem(List<ReturnItem> returnItems, List<SerialNumber> serials, List<BatchNo> batches)
        {
            using var t = _dataContext.Database.BeginTransaction();
            var reID = returnItems.FirstOrDefault().ReceiptID;
            var docType = _dataContext.DocumentTypes.FirstOrDefault(i => i.Code == "SP");
            var receipt = _dataContext.Receipt.FirstOrDefault(r => r.ReceiptID == reID);
            var receiptdetail = _dataContext.ReceiptDetail.Where(d => d.ReceiptID == receipt.ReceiptID).ToList();
            var currency = _dataContext.Currency.FirstOrDefault(i => i.ID == receipt.PLCurrencyID);
            var incoming = _dataContext.IncomingPaymentCustomers.FirstOrDefault(w => w.SeriesDID == receipt.SeriesDID);
            double checkAmount = 0;
            receiptdetail.ForEach(i =>
            {
                returnItems.ToList().ForEach(k =>
                {
                    if (i.ItemID == k.ItemID)
                    {
                        checkAmount += i.UnitPrice * k.ReturnQty;
                    }
                });
            });
            if ((decimal)checkAmount > ((decimal)receipt.GrandTotal - receipt.AppliedAmount))
            {
                var incomDetail = _dataContext.IncomingPaymentDetails.FirstOrDefault(w => w.IcoPayCusID == incoming.IncomingPaymentCustomerID) ?? new IncomingPaymentDetail();
                var incomingPay = _dataContext.IncomingPayments.FirstOrDefault(w => w.IncomingPaymentID == incomDetail.IncomingPaymentID) ?? new IncomingPayment();
                var incomDe = _dataContext.IncomingPaymentDetails.Where(w => w.IncomingPaymentID == incomingPay.IncomingPaymentID);

                foreach (var item in incomDe)
                {
                    var paymentCus = _dataContext.IncomingPaymentCustomers.FirstOrDefault(w => w.IncomingPaymentCustomerID == item.IcoPayCusID) ?? new IncomingPaymentCustomer();
                    var receiptPay = _dataContext.Receipt.FirstOrDefault(w => w.SeriesDID == paymentCus.SeriesDID) ?? new Receipt();
                    receiptPay.AppliedAmount -= (decimal)item.Totalpayment;
                    paymentCus.Applied_Amount -= item.Totalpayment;
                    paymentCus.BalanceDue += item.Totalpayment;
                    paymentCus.TotalPayment += item.Totalpayment;
                    if (paymentCus.Applied_Amount == 0)
                    {
                        paymentCus.Status = "close";
                    }
                    else
                    {
                        paymentCus.Status = "open";
                    }
                    _dataContext.Receipt.Update(receiptPay);
                    _dataContext.IncomingPaymentCustomers.Update(paymentCus);
                    _dataContext.SaveChanges();
                }
                if (incomDetail.IncomingPaymentID != 0)
                {
                    _incoming.IncomingPaymentCancelPOS(incomDetail.IncomingPaymentID);
                    incomingPay.Status = "cancel";
                    _dataContext.IncomingPayments.Update(incomingPay);
                    _dataContext.SaveChanges();
                }
            }
            else
            {
                receipt.AppliedAmount -= (decimal)checkAmount;
                incoming.Applied_Amount -= checkAmount;
                incoming.BalanceDue += checkAmount;
                incoming.TotalPayment += checkAmount;
                if (incoming.Applied_Amount == 0)
                {
                    incoming.Status = "close";
                }
                else
                {
                    incoming.Status = "open";
                }
                _dataContext.Receipt.Update(receipt);
                _dataContext.IncomingPaymentCustomers.Update(incoming);
                _dataContext.SaveChanges();
            }
            bool isReturned = _pos.SendReturnItem(returnItems, serials, batches);
            t.Commit();
            return isReturned;
        }

        public SerialNumberBarcodeFind FindSerialNumber(string serialNumber)
        {
            List<WarehouseDetail> _ws = _dataContext.WarehouseDetails.Where(i => !string.IsNullOrEmpty(i.SerialNumber) && i.SerialNumber.ToLower() == serialNumber.ToLower()).ToList();
            WarehouseDetail ws = _ws.FirstOrDefault(i => i.InStock > 0);
            if (!_ws.Any()) return null;
            var itemMaster = _pos.GetItemMasterDataById(_ws.FirstOrDefault().ItemID);
            if (ws == null) return new SerialNumberBarcodeFind { IsOutOfStock = true, SerialNumber = new SerialNumber { ItemName = itemMaster.KhmerName } };
            Warehouse wh = _dataContext.Warehouses.Find(ws.WarehouseID) ?? new Warehouse();
            List<SerialNumberSelectedDetail> serialNumberSelectedDetails = new();
            SerialNumberSelectedDetail serialDetial = new()
            {
                LineID = $"{DateTime.Now.Ticks}{ws.ItemID}{ws.ID}",
                LotNumber = ws.LotNumber,
                ExpireDate = ws.ExpireDate == null || ws.ExpireDate == default ? string.Empty : DateTime.ParseExact(ws.MfrDate.ToString(), "dd-MM-yyyy", null).ToString(),
                MfrSerialNo = ws.MfrSerialNumber,
                Qty = 1,
                SerialNumber = ws.SerialNumber,
                UnitCost = (decimal)ws.Cost,
                ItemID = ws.ItemID,



                PlateNumber = ws.PlateNumber,
                Color = ws.Color,
                Brand = ws.Brand,
                Condition = ws.Condition,
                Type = ws.Type,
                Power = ws.Power,
                Year = ws.Year,

                MfrDate = ws.MfrDate == default || ws.MfrDate == null ? "" : DateTime.ParseExact(ws.MfrDate.ToString(), "dd-MM-yyyy", null).ToString(),
                AdmissionDate = ws.AdmissionDate == default || ws.AdmissionDate == null ? "" : DateTime.ParseExact(ws.AdmissionDate.ToString(), "dd-MM-yyyy", null).ToString(),
                MfrWarDateStart = ws.MfrWarDateStart == default || ws.MfrWarDateStart == null ? "" : DateTime.ParseExact(ws.MfrWarDateStart.ToString(), "dd-MM-yyyy", null).ToString(),
                MfrWarDateEnd = ws.MfrWarDateEnd == default || ws.MfrWarDateEnd == null ? "" : DateTime.ParseExact(ws.MfrWarDateEnd.ToString(), "dd-MM-yyyy", null).ToString(),
                Location = ws.Location,
                Details = ws.Details,

            };
            serialNumberSelectedDetails.Add(serialDetial);
            var serialNumberUnselected = _POSSerialBatch.GetSerialDetialsAsync(ws.ItemID, ws.WarehouseID).Result;
            return new SerialNumberBarcodeFind
            {
                SerialNumber = new SerialNumber
                {
                    Barcode = itemMaster.Barcode,
                    BaseQty = 1,
                    BpId = 0,
                    Cost = (decimal)ws.Cost,
                    Direction = "Out",
                    ItemCode = itemMaster.Code,
                    ItemID = itemMaster.ID,
                    ItemName = itemMaster.KhmerName,
                    LineID = $"{DateTime.Now.Ticks}{ws.ItemID}{ws.ID}",
                    OpenQty = 1,
                    Qty = 0,
                    SaleID = 0,
                    SerialNumberSelected = new SerialNumberSelected
                    {
                        SerialNumberSelectedDetails = serialNumberSelectedDetails,
                        TotalSelected = 1,
                    },
                    TotalSelected = 1,
                    SerialNumberUnselected = serialNumberUnselected,
                    UomID = 0,
                    WareId = ws.WarehouseID,
                    WhsCode = wh.Code
                },
                SerialNumberSelectedDetail = serialDetial
            };
        }

        public async Task<OrderDetail> FindItemByBarcodeAsync(int orderId, int pricelistId, int comId, string barcode)
        {
            try
            {
                var serial = FindSerialNumber(barcode);
                var user = _userModule.CurrentUser;
                var setting = await GetUserSettingAsync(user.ID);
                // var items = await _pos.GetItemMasterDatasAsync(pricelistId, setting.WarehouseID, comId);
                var saleItems = _pos.GetSaleItems(pricelistId, setting.WarehouseID);
                if (serial != null)
                {
                    if (serial.IsOutOfStock) return new OrderDetail { IsOutOfStock = true, KhmerName = serial.SerialNumber.ItemName, IsSerialNumber = true };
                    var itemSerail = saleItems.FirstOrDefault(it => string.Compare(it.Barcode, serial.SerialNumber.Barcode, true) == 0);
                    OrderDetail odSerial = await CreateOrderDetailAsync(itemSerail, orderId);
                    odSerial.SerialNumberSelectedDetial = serial.SerialNumberSelectedDetail;
                    odSerial.SerialNumber = serial.SerialNumber;
                    odSerial.IsSerialNumber = true;
                    return odSerial;
                }

                ServiceItemSales item = saleItems.FirstOrDefault(it => string.Compare(it.Barcode, barcode, true) == 0);
                OrderDetail od = await CreateOrderDetailAsync(item, orderId);
                if (item != null) return od; ;

                if (!string.IsNullOrWhiteSpace(barcode))
                {
                    const int priceLength = 6;
                    if (barcode.Length > priceLength)
                    {
                        string priceDigits = barcode[^priceLength..];
                        string codeDigits = barcode.Substring(barcode.Length - priceDigits.Length - 4, 4);
                        string significant = priceDigits.Substring(0, 3);
                        string fix = priceDigits.Substring(significant.Length, 2);
                        double total = double.Parse(significant + "." + fix);
                        if (codeDigits.Length <= 5)
                        {
                            ServiceItemSales scaleItem = saleItems.FirstOrDefault(i => string.Compare(i.Barcode, codeDigits, true) == 0);
                            od = await CreateOrderDetailAsync(scaleItem, orderId);
                            od.Total = total;
                            if (od.UnitPrice > 0)
                            {
                                od.Qty = double.Parse((total / od.UnitPrice).ToString("N3"));
                            }
                            od.PrintQty = od.Qty;
                            return od;
                        }
                    }
                }

                return od;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new OrderDetail();
            }
        }

        public bool IsBetweenDate(DateTime dateFrom, DateTime dateTo)
        {
            DateTime today = DateTime.Today;
            var _dateFrom = DateTime.Compare(today, dateFrom);
            var _dateTo = DateTime.Compare(today, dateTo);

            bool isValid = _dateFrom >= 0 && _dateTo <= 0;
            return isValid;
        }

        public async Task<Currency> FindCurrencyAsync(int currencyId)
        {
            return await _dataContext.Currency.FindAsync(currencyId);
        }

        public async Task<PromotionPrice> FindPromotionAsync(int promotionId)
        {
            return await _dataContext.PromotionPrice.FindAsync(promotionId);
        }

        private async Task<OrderDetail> CreateOrderDetailAsync(ServiceItemSales item, int orderId)
        {
            try
            {
                if (item == null || item?.ID <= 0) { return new OrderDetail(); }
                var itemUoMs = await SelectItemUoMsAsync(item.GroupUomID, item.UomID);
                var printers = await _dataContext.PrinterNames.Where(p => !p.Delete).Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),
                    Text = p.Name,
                    Selected = IsNotEmptyEqual(p.Name, item.PrintTo, false)
                }).ToListAsync();
                var taxGroups = await GetTaxGroupsAsync();
                var taxForSale = taxGroups.FirstOrDefault(i => i.ID == item.TaxGroupSaleID) ?? new TaxGroupModel();
                OrderDetail od = new()
                {
                    LineID = item.ID.ToString(),
                    OrderID = orderId,
                    OrderDetailID = 0,
                    ItemID = item.ItemID,
                    Code = item.Code,
                    KhmerName = item.KhmerName,
                    EnglishName = item.EnglishName,
                    Cost = item.Cost,
                    DiscountRate = item.DiscountRate,
                    DiscountValue = item.DiscountValue,
                    TaxGroups = await SelectTaxGroupsAsync(item.TaxGroupSaleID),
                    TaxRate = taxForSale.Rate,
                    TaxGroupID = item.TaxGroupSaleID,
                    Qty = 1,
                    PrintQty = 1,
                    UnitPrice = item.UnitPrice,
                    Total = item.UnitPrice * (1 - item.DiscountRate / 100),
                    GroupUomID = item.GroupUomID,
                    UomID = item.UomID,
                    Uom = item.UoM,
                    ItemStatus = "new",
                    ItemPrintTo = item.PrintTo,
                    Currency = item.Currency,
                    ItemType = item.ItemType,
                    TypeDis = item.TypeDis,
                    Description = item.Description,
                    ParentLineID = "",
                    ItemUoMs = itemUoMs,
                    Printers = printers,
                    IsScale = item.IsScale,
                    AddictionProps = item.AddictionProps
                };
                return od;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new OrderDetail();
            }
        }

        public async Task<OrderDetail> GetOrderDetailUnknownAsync(int userId, int orderId, int comId)
        {
            var setting = await GetUserSettingAsync(userId);
            List<PrinterName> printerNames = await _dataContext.PrinterNames.Where(p => !p.Delete).ToListAsync();
            Order order = await _dataContext.Order.FindAsync(orderId);
            var priceListId = setting.PriceListID;
            if (order != null)
            {
                priceListId = order.PriceListID;
            }

            var items = await _pos.GetItemMasterDatasAsync(priceListId, setting.WarehouseID, comId);
            ServiceItemSales item = items.FirstOrDefault(i => string.Compare(i.KhmerName, "unknown", true) == 0);
            var od = await CreateOrderDetailAsync(item, orderId);
            od.LineID = DateTime.Now.Ticks.ToString();
            return await Task.FromResult(od);
        }

        public async Task<ShiftTemplate> CreateShiftTemplateAsync(int userId)
        {
            var user = await FindUserAsync(userId);
            var company = await _dataContext.Company.FindAsync(user.CompanyID) ?? new Company();
            List<ShiftForm> shiftForms = new();
            string sysCurrency = "";
            foreach (var xr in GetExchangeRates())
            {
                if (xr.CurrencyID == company.SystemCurrencyID)
                {
                    sysCurrency = xr.Currency.Description;
                }
                shiftForms.Add(new ShiftForm
                {
                    ID = xr.CurrencyID,
                    Decription = "Cash",
                    InputCash = 0,
                    Currency = xr.Currency.Description,
                    RateIn = xr.Rate
                });
            }

            var cashIn = _dataContext.OpenShift.FirstOrDefault(o => o.UserID == userId && o.Open);
            var receipts = _dataContext.Receipt.Where(r => r.UserOrderID == userId).ToList();
            double grandTotalSys = 0;
            if (cashIn != null)
            {
                int tranTo = 0;
                if (receipts.Count > 0)
                {
                    tranTo = receipts.Max(w => w.ReceiptID);
                }
                var receipt = _dataContext.Receipt.Where(w => w.UserOrderID == userId && w.ReceiptID > cashIn.Trans_From && w.ReceiptID <= tranTo);
                var memo = from r in receipt
                           join rd in _dataContext.ReceiptMemo on r.ReceiptID equals rd.BasedOn
                           select new { rd };
                grandTotalSys = (receipt.Sum(s => s.GrandTotal_Sys)) - (memo.Sum(s => s.rd.GrandTotalSys));
            }
            var shiftTemplate = new ShiftTemplate
            {
                ShiftForms = shiftForms,
                CurrencySys = sysCurrency,
                GrandTotalSys = grandTotalSys
            };
            return await Task.FromResult(shiftTemplate);
        }

        public bool CheckOpenShift(int userId = 0)
        {
            var hasOpenedShift = _dataContext.OpenShift.Any(w => w.UserID == userId && w.Open);
            return hasOpenedShift;
        }

        private IEnumerable<ExchangeRate> GetExchangeRates()
        {
            return _dataContext.ExchangeRates.Include(cur => cur.Currency).Where(w => !w.Currency.Delete).ToList();
        }

        public async Task<bool> VoidOrderAsync(int orderId, string reason)
        {
            try
            {
                return await _pos.VoidOrderAsync(orderId, reason);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<int> MoveOrdersAsync(int fromTableId, int toTableId, List<Order> orders)
        {
            var _orderId = await _pos.MoveOrdersAsync(fromTableId, toTableId, orders);
            return _orderId;
        }

        public async Task<Table> ChangeTableAsync(int previousId, int currentId)
        {
            var currentTable = await _pos.MoveTableAsync(previousId, currentId);
            return currentTable;
        }

        public async Task<List<DisplayCurrency>> CreateChangeRateTemplate(int orderId, int customerId = 0)
        {
            var setting = await GetUserSettingAsync(_userModule.CurrentUser.ID);
            var order = await _dataContext.Order.FindAsync(orderId);
            int pricelistId = setting.PriceListID;
            if (order != null)
            {
                pricelistId = order.PriceListID;
            }
            else
            {
                if (setting.IsCusPriceList)
                {
                    var cus = _dataContext.BusinessPartners.FirstOrDefault(i => i.ID == customerId) ?? new BusinessPartner();
                    pricelistId = cus.PriceListID;
                }
            }
            var dcs = _dataContext.DisplayCurrencies.Where(i => i.PriceListID == pricelistId).ToList();
            var p = _dataContext.PriceLists.FirstOrDefault(i => i.ID == pricelistId) ?? new PriceLists();
            var cur = _dataContext.Currency.FirstOrDefault(i => i.ID == p.CurrencyID) ?? new Currency();
            var dcTemps = (from c in _dataContext.Currency.Where(i => !i.Delete)
                           join g1 in dcs on c.ID equals g1.AltCurrencyID
                           into g
                           from _plBasedDc in g.DefaultIfEmpty()
                           let plBasedDc = _plBasedDc ?? new DisplayCurrency()
                           select new DisplayCurrency
                           {
                               LineID = $"{DateTime.Now.Ticks}{c.ID}",
                               ID = plBasedDc.ID,
                               PriceListID = p.ID,
                               AltCurrencyID = plBasedDc.AltCurrencyID == 0 || dcs.Count <= 0 ? c.ID : plBasedDc.AltCurrencyID,
                               CurPLID = cur.ID,
                               PLDisplayRate = dcs.Count <= 0 ? 0 : plBasedDc.PLDisplayRate,
                               PLCurrencyName = cur.Description,
                               ALCurrencyName = c.Description,
                               DisplayRate = dcs.Count <= 0 ? 0 : plBasedDc.DisplayRate,
                               IsActive = plBasedDc.IsActive,
                               IsShowCurrency = plBasedDc.IsShowCurrency,
                               IsShowOtherCurrency = plBasedDc.IsShowOtherCurrency,
                               DecimalPlaces = plBasedDc.DecimalPlaces,
                           }).ToList();
            return await Task.FromResult(dcTemps);
        }
        public ModelStateDictionary SaveDisplayCurrencies(List<DisplayCurrency> displayCurrencies, ModelStateDictionary modelState)
        {
            try
            {
                foreach (var i in displayCurrencies)
                {
                    i.PLDisplayRate = 1 / i.DisplayRate;
                }
                if (modelState.IsValid)
                {
                    _dataContext.DisplayCurrencies.UpdateRange(displayCurrencies);
                    _dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                modelState.AddModelError("Exception", ex.Message);
            }

            return modelState;
        }

        public async Task<BusinessPartner> FindCustomerAsync(int customerId)
        {
            try
            {
                var customer = await _dataContext.BusinessPartners.FindAsync(customerId) ?? new BusinessPartner();
                if (IsNotEmptyEqual(customer.Type, "customer", true)) { return customer; }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return new BusinessPartner();
        }

        public async Task<List<BusinessPartner>> GetCustomersAsyc(string keyword = "")
        {
            var customers = _dataContext.BusinessPartners.Where(w => w.Type.ToLower() == "Customer".ToLower() && !w.Delete).ToList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                customers = customers.Where(c =>
                            RawWord(c.Code).Contains(keyword, ignoreCase)
                            || RawWord(c.Name).Contains(keyword, ignoreCase)
                            || RawWord(c.Phone).Contains(keyword, ignoreCase)
                            || RawWord(c.Email).Contains(keyword, ignoreCase)
                            || RawWord(c.Type).Contains(keyword, ignoreCase)
                            || RawWord(c.Address).Contains(keyword, ignoreCase)).ToList();
            }
            return await Task.FromResult(customers);
        }

        public async Task<List<ReturnItem>> GetReturnItemsAsync(int userId, int receiptId)
        {
            var details = _dataContext.ReceiptDetail.Where(w => w.ReceiptID == receiptId && w.OpenQty > 0)
               .Select(r => new ReturnItem
               {
                   ID = r.ID,
                   ReceiptID = r.ReceiptID,
                   ItemID = r.ItemID,
                   Code = r.Code,
                   KhName = r.KhmerName,
                   UoM = r.UnitofMeansure.Name,
                   UomID = r.UomID,
                   OpenQty = r.OpenQty,
                   ReturnQty = 0,
                   UserID = userId
               });
            return await details.ToListAsync();
        }

        public async Task<List<ReceiptSummary>> GetReturnReceiptsAsync(int userId, string dateFrom, string dateTo, string keyword = "")
        {
            var user = FindUserAsync(userId).Result;
            var receipts = _pos.GetReceiptReturn(user.ID, user.BranchID, dateFrom, dateTo).ToList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                receipts = receipts.Where(r => r.ReceiptNo.ToLowerInvariant()
                .Contains(keyword.Replace("\\+s", string.Empty), StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            return await Task.FromResult(receipts);
        }

        public async Task<ModelStateDictionary> ReturnReceiptsAsync(List<ReturnItem> returnItems, ModelStateDictionary modelState, List<SerialNumber> serials, List<BatchNo> batches, int? PaymentmeansID = 0,int? userid=0, string reason = "")
        {
            try
            {
                using var t = await _dataContext.Database.BeginTransactionAsync();
                var reID = returnItems.FirstOrDefault().ReceiptID;
                var docType = _dataContext.DocumentTypes.FirstOrDefault(i => i.Code == "SP");
                var receipt = _dataContext.Receipt.FirstOrDefault(r => r.ReceiptID == reID);
                var receiptdetails = _dataContext.ReceiptDetail.Where(d => d.ReceiptID == receipt.ReceiptID).ToList();
                var currency = _dataContext.Currency.FirstOrDefault(i => i.ID == receipt.PLCurrencyID);
                var incomingCus = _dataContext.IncomingPaymentCustomers.FirstOrDefault(w => w.SeriesDID == receipt.SeriesDID);
             
                double checkAmount = 0;
                checkAmount = returnItems.Sum(ri => ri.ReturnQty * receiptdetails
                    .Where(rd => rd.ItemID == ri.ItemID && rd.UomID == ri.UomID)
                    .Select(rd => rd.UnitPrice).FirstOrDefault());
                if ((decimal)checkAmount >= receipt.BalanceToPay)
                {
                    double balanceDueleft = receipt.GrandTotal - (double)receipt.BalanceReturn;
                    receipt.AppliedAmount += (decimal)balanceDueleft;
                    receipt.BalanceReturn += (decimal)balanceDueleft;
                    receipt.BalanceToPay = 0;

                    if (incomingCus != null)
                    {
                        var incomDe = _dataContext.IncomingPaymentDetails.Where(w => w.IcoPayCusID == incomingCus.IncomingPaymentCustomerID).ToList();
                        var incomDeUMaster = incomDe.GroupBy(i => i.IncomingPaymentID).Select(i => i.FirstOrDefault()).ToList();
                        incomingCus.Applied_Amount += balanceDueleft;
                        incomingCus.BalanceDue -= balanceDueleft;
                        incomingCus.TotalPayment -= balanceDueleft;
                        if (incomingCus.Applied_Amount == incomingCus.Total)
                        {
                            incomingCus.Status = "close";
                        }
                        else
                        {
                            incomingCus.Status = "open";
                        }
                        _dataContext.IncomingPaymentCustomers.Update(incomingCus);
                        foreach (var item in incomDeUMaster)
                        {
                            if (item.IncomingPaymentID > 0)
                            {
                                var incoming = _dataContext.IncomingPayments.FirstOrDefault(w => w.IncomingPaymentID == item.IncomingPaymentID) ?? new IncomingPayment();
                                _incoming.IncomingPaymentCancelPOS(item.IncomingPaymentID);
                                incoming.Status = "cancel";
                                _dataContext.IncomingPayments.Update(incoming);
                                _dataContext.SaveChanges();
                            }
                        }
                    }

                    _dataContext.Receipt.Update(receipt);
                    _dataContext.SaveChanges();

                }
                else
                {
                    if (incomingCus != null)
                    {
                        incomingCus.Applied_Amount += checkAmount;
                        incomingCus.BalanceDue -= checkAmount;
                        incomingCus.TotalPayment -= checkAmount;
                        if (incomingCus.Applied_Amount == incomingCus.Total)
                        {
                            incomingCus.Status = "close";
                        }
                        else
                        {
                            incomingCus.Status = "open";
                        }
                        _dataContext.IncomingPaymentCustomers.Update(incomingCus);

                    }
                    receipt.AppliedAmount += (decimal)checkAmount;
                    receipt.BalanceReturn += (decimal)checkAmount;
                    receipt.BalanceToPay -= (decimal)checkAmount;
                    _dataContext.Receipt.Update(receipt);
                    _dataContext.SaveChanges();
                }
                if ((decimal)checkAmount > receipt.BalanceReturn)
                {
                    if (receipt.BalanceReturn < 0) receipt.BalanceReturn = 0;
                    receipt.BalancePay = receipt.BalanceReturn;
                }
                else
                {
                    receipt.BalancePay = (decimal)checkAmount;
                }
                bool isReturned = _pos.SendReturnItem(returnItems, serials, batches, PaymentmeansID,userid,reason);
                t.Commit();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddModelError("Exception", ex.Message);
            }
            return modelState;
        }

        public async Task<List<ReceiptSummary>> GetReceiptsToCancelAsync(int userId, string dateFrom, string dateTo, string keyword = "")
        {
            var user = FindUserAsync(userId).Result;
            var receipts = _pos.GetReceiptCancel(user.BranchID, dateFrom, dateTo);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                receipts = receipts.Where(r => r.ReceiptNo.ToLowerInvariant()
                .Contains(keyword.Replace("\\+s", string.Empty), StringComparison.CurrentCultureIgnoreCase));
            }
            return await Task.FromResult(MapToReceiptSummaries(receipts));
        }

        private static List<ReceiptSummary> MapToReceiptSummaries(IEnumerable<Receipt> receipts)
        {
            receipts ??= new List<Receipt>();
            List<ReceiptSummary> reprintReceipts = new();
            if (receipts.ToList().Count > 0)
            {
                foreach (Receipt r in receipts)
                {
                    reprintReceipts.Add(new ReceiptSummary
                    {
                        ReceiptID = r.ReceiptID,
                        ReceiptNo = r.ReceiptNo,
                        Cashier = r.Cashier,
                        CusName = r.CusName,
                        Phone = r.Phone,
                        DateOut = r.DateOut.ToString("MM-dd-yyyy"),
                        TimeOut = DateTime.Parse(r.TimeOut).ToString("hh:mm tt"),
                        TableName = r.TableName,
                        // GrandTotal = string.Format($"{r.Currency.Description} {r.GrandTotal:N3}"),
                        GrandTotal = r.GrandTotalAmount

                    });
                }
            }
            return reprintReceipts;
        }

        public async Task<ModelStateDictionary> CancelReceiptAsync(Receipt receipt, ModelStateDictionary modelState, List<SerialNumber> serials, List<BatchNo> batches, string reason = "", int? PaymentmeansID = 0)
        {
            try
            {
                using var t = await _dataContext.Database.BeginTransactionAsync();
                var incomingCus = _dataContext.IncomingPaymentCustomers.FirstOrDefault(w => w.SeriesDID == receipt.SeriesDID) ?? new IncomingPaymentCustomer();
                if (receipt == null)
                {
                    modelState.AddModelError("Receipt", "The specified receipt not found.");
                }

                // if (incomingCus.IncomingPaymentCustomerID == 0 && receipt.AppliedAmount != (decimal)receipt.GrandTotal)
                // {
                //     modelState.AddModelError("IncomingPayment", "Incoming payment customer not found.");
                // }

                if (modelState.IsValid)
                {
                    double balanceDueleft = receipt.GrandTotal - (double)receipt.BalanceReturn;
                    receipt.AppliedAmount += (decimal)balanceDueleft;
                    receipt.BalanceReturn += (decimal)balanceDueleft;
                    if (incomingCus.IncomingPaymentCustomerID >0)
                    {
                        var incomDe = _dataContext.IncomingPaymentDetails.Where(w => w.IcoPayCusID == incomingCus.IncomingPaymentCustomerID).ToList();
                        var incomDeUMaster = incomDe.GroupBy(i => i.IncomingPaymentID).Select(i => i.FirstOrDefault()).ToList();
                        incomingCus.Applied_Amount += balanceDueleft;
                        incomingCus.BalanceDue -= balanceDueleft;
                        incomingCus.TotalPayment -= balanceDueleft;
                        if (incomingCus.Applied_Amount == incomingCus.Total)
                        {
                            incomingCus.Status = "close";
                        }
                        else
                        {
                            incomingCus.Status = "open";
                        }

                        _dataContext.IncomingPaymentCustomers.Update(incomingCus);
                        foreach (var item in incomDeUMaster)
                        {
                            if (item.IncomingPaymentID > 0)
                            {
                                var incoming = _dataContext.IncomingPayments.FirstOrDefault(w => w.IncomingPaymentID == item.IncomingPaymentID) ?? new IncomingPayment();
                                _incoming.IncomingPaymentCancelPOS(item.IncomingPaymentID, PaymentmeansID);
                                incoming.Status = "cancel";
                                _dataContext.IncomingPayments.Update(incoming);
                                _dataContext.SaveChanges();
                            }

                        }
                    }

                    receipt.BalancePay = receipt.BalanceReturn;
                    await _pos.CancelReceiptAsync(receipt, serials, batches, reason, PaymentmeansID,_userModule.GetUserId());
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                modelState.AddModelError("Exception", ex.Message);
            }
            return modelState;
        }

        public async Task<List<Order>> GetOrdersToCombineAsync(int orderId)
        {
            var order = await _dataContext.Order.FindAsync(orderId);
            if (order == null) { return new List<Order>(); }
            int branchId = _userModule.CurrentUser.BranchID;
            var _orders = _pos.GetUserOrders(branchId);
            var orders = await _orders.Where(o => o.OrderID != orderId).OrderBy(o => o.TableID).ToListAsync();
            foreach (var o in orders)
            {
                var table = await _dataContext.Tables.FindAsync(o.TableID);
                o.TitleNote = $"{table.Name} #{o.OrderNo}";
            }
            return orders;
        }

        public async Task CombineOrdersAsync(CombineOrder combineOrder)
        {
            await _pos.CombineOrdersAsync(combineOrder);
        }

        public async Task<Order> SplitOrderAsync(Order splitOrder)
        {
            var newOrder = await _pos.SplitOrderAsync(splitOrder);
            return newOrder;
        }

        public async Task<List<PendingVoidItemModel>> GetPendingVoidItemAsync(int userId, DateTime dateFrom, DateTime dateTo, string keyword = "")
        {
            var user = await FindUserAsync(userId);
            var pendingVoidItems = GetPendingVoidItem(user, dateFrom, dateTo);
            StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                pendingVoidItems = pendingVoidItems.Where(r =>
                RawWord(r.OrderNo).Contains(keyword, ignoreCase)
                || RawWord(r.Cashier).Contains(keyword, ignoreCase)
                || RawWord(r.Table).Contains(keyword, ignoreCase)
                || RawWord(r.QueueNo).Contains(keyword, ignoreCase)
                || RawWord(r.Time).Contains(keyword, ignoreCase)
                || RawWord(r.Date).Contains(keyword, ignoreCase)
                || RawWord(r.Amount).Contains(keyword, ignoreCase)).ToList();
            }
            return await Task.FromResult(pendingVoidItems);
        }
        private List<PendingVoidItemModel> GetPendingVoidItem(UserAccount user, DateTime _fromDate, DateTime _toDate)
        {
            List<PendingVoidItem> pendingVoidItems = _dataContext.PendingVoidItems
                    .Where(i => !i.IsVoided && i.DateIn >= _fromDate && i.DateIn <= _toDate && i.BranchID == user.BranchID)
                    .ToList();
            var data = (from pvi in pendingVoidItems
                        join cur in _dataContext.Currency on pvi.PLCurrencyID equals cur.ID
                        join table in _dataContext.Tables on pvi.TableID equals table.ID
                        select new PendingVoidItemModel
                        {
                            Amount = $"{cur.Symbol} {string.Format("{0:#,0.00}", pvi.GrandTotal)}",
                            ID = pvi.ID,
                            Cashier = user.Username,
                            Date = pvi.DateIn.ToShortDateString(),
                            IsVoided = pvi.IsVoided,
                            OrderID = pvi.OrderID,
                            OrderNo = pvi.OrderNo,
                            QueueNo = pvi.QueueNo,
                            Table = table.Name,
                            Time = pvi.TimeIn
                        }).ToList();
            return data;
        }
        public async Task<List<ReceiptSummary>> GetReceiptsToReprintAsync(int userId, string dateFrom, string dateTo, string keyword = "")
        {
            var user = await FindUserAsync(userId);
            var receipts = _pos.GetReceiptReprint(user.BranchID, dateFrom, dateTo).ToList();
            var receiptSummaries = MapToReceiptSummaries(receipts);
            StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                receiptSummaries = receiptSummaries.Where(r =>
                RawWord(r.ReceiptNo).Contains(keyword, ignoreCase)
                || RawWord(r.Cashier).Contains(keyword, ignoreCase)
                || RawWord(r.TableName).Contains(keyword, ignoreCase)
                || RawWord(r.GrandTotal).Contains(keyword, ignoreCase)).ToList();
            }
            return await Task.FromResult(receiptSummaries);
        }

        public async Task<List<PrintBill>> ReprintReceiptAsync(int userId, int receiptId, string printType = "Pay", bool isWebClient = true)
        {
            var printBills = await _pos.PrintReceiptReprintAsync(receiptId, printType, userId, isWebClient);
            return printBills;
        }

        public async Task<List<CashoutReportMaster>> ReprintReceiptCloseShiftAsync(int userId, int closeShiftId, bool isWebClient = true)
        {
            var cashoutReports = _pos.ReprintCloseShift(userId, closeShiftId, isWebClient);
            return await Task.FromResult(cashoutReports);
        }

        public async Task<IEnumerable<OpenShift>> OpenShiftAsync(int userId, double total)
        {
            return await Task.FromResult(_pos.OpenShiftData(userId, total));
        }

        public async Task<List<CashoutReportMaster>> CloseShiftAsync(int userId, double total, bool isWebClient = true)
        {
            return await _pos.CloseShiftData(userId, total, isWebClient);
        }

        public async Task<List<VoidItem>> VoidItemAsync(int userId)
        {
            var voidItem = await _dataContext.VoidItems.Where(w => w.UserOrderID == userId && w.IsVoided == false).ToListAsync();

            foreach (var item in voidItem)
            {
                item.IsVoided = true;
                _dataContext.VoidItems.Update(item);
            }
            _dataContext.SaveChanges();
            return voidItem;
        }

        public async Task<List<SelectListItem>> SelectWarehousesAsync(int selectedId = 0)
        {
            return await _dataContext.Warehouses.Where(w => !w.Delete).Select(w => new SelectListItem
            {
                Value = w.ID.ToString(),
                Text = w.Name,
                Selected = w.ID == selectedId
            }).ToListAsync();
        }
        private async Task IssuseCommittedOrderAsync(Order order)
        {
            foreach (var order_detail in order.OrderDetail.ToList())
            {
                var item = _dataContext.ItemMasterDatas.FirstOrDefault(x => x.ID == order_detail.ItemID) ?? new ItemMasterData();
                if (item.Process != "Standard")
                {
                    var orft = _dataContext.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == order_detail.GroupUomID && w.AltUOM == order_detail.UomID);
                    var item_warehouse = _dataContext.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == order.WarehouseID && w.ItemID == order_detail.ItemID);
                    item_warehouse.Committed += (double)order_detail.PrintQty * orft.Factor;
                    _dataContext.WarehouseSummary.Update(item_warehouse);
                    await _dataContext.SaveChangesAsync();
                }

            }
        }
        public async Task<bool> VoidItemsAsync(Order order)
        {
            var _order = await CheckedOrderAsync(order);
            return await _pos.VoidItemsAsync(_order);
        }

        public async Task<bool> PendingVoidItemsAsync(Order order, string reason = "")
        {
            var _order = await CheckedOrderAsync(order);
            return await _pos.PendingVoidItemsAsync(_order, reason);
        }

        public dynamic DeletePendingVoidItem(int id)
        {
            using var t = _dataContext.Database.BeginTransaction();
            var pendingVoidItem = _dataContext.PendingVoidItems.Find(id);
            if (pendingVoidItem == null) return new { Error = true, Message = "Could not find Pending Void Item" };
            var pendingVoidItemDetails = _dataContext.PendingVoidItemDetails.Where(i => i.PendingVoidItemID == pendingVoidItem.ID).ToList();
            try
            {
                _dataContext.PendingVoidItemDetails.RemoveRange(pendingVoidItemDetails);
                _dataContext.PendingVoidItems.Remove(pendingVoidItem);
                _dataContext.SaveChanges();
                t.Commit();
                return new { Error = false };
            }
            catch (Exception e)
            {
                return new { Error = true, e.Message };
            }
        }


        public ModelMessage InsertPendingVoidItemToVoidItem(List<PendingVoidItemModel> data, string reason)
        {
            ModelMessage msg = new();
            try
            {
                using var t = _dataContext.Database.BeginTransaction();
                foreach (var i in data)
                {
                    var pendingVoidItem = _dataContext.PendingVoidItems.FirstOrDefault(p => p.ID == i.ID);
                    var _pendingVoidItemDetails = _dataContext.PendingVoidItemDetails.Where(p => p.PendingVoidItemID == i.ID).ToList();
                    pendingVoidItem.PendingVoidItemDetails = _pendingVoidItemDetails;
                    VoidItem obj = EntityHelper.Assign<VoidItem>(CheckedPendingVoidItemAsync(pendingVoidItem));
                    List<VoidItemDetail> voidItemDetails = new();
                    foreach (var item in pendingVoidItem.PendingVoidItemDetails)
                    {
                        //item.ID = 0;
                        voidItemDetails.Add(EntityHelper.Assign<VoidItemDetail>(item));
                    }
                    voidItemDetails.ForEach(i => i.ID = 0);
                    obj.VoidItemDetails = voidItemDetails;
                    obj.Reason = reason;
                    obj.OrderID = pendingVoidItem.OrderID;
                    obj.OrderNo = pendingVoidItem.OrderNo;
                    obj.TableID = pendingVoidItem.TableID;
                    obj.ReceiptNo = pendingVoidItem.ReceiptNo;
                    obj.QueueNo = pendingVoidItem.QueueNo;
                    obj.DateIn = pendingVoidItem.DateIn;
                    obj.DateOut = pendingVoidItem.DateOut;
                    obj.TimeIn = pendingVoidItem.TimeIn;
                    obj.TimeOut = pendingVoidItem.TimeOut;
                    obj.WaiterID = pendingVoidItem.WaiterID;
                    obj.UserOrderID = pendingVoidItem.UserOrderID;
                    obj.CustomerID = pendingVoidItem.CustomerID;
                    obj.CustomerCount = pendingVoidItem.CustomerCount;
                    obj.PriceListID = pendingVoidItem.PriceListID;
                    obj.LocalCurrencyID = pendingVoidItem.LocalCurrencyID;
                    obj.SysCurrencyID = pendingVoidItem.SysCurrencyID;
                    obj.ExchangeRate = pendingVoidItem.ExchangeRate;
                    obj.WarehouseID = pendingVoidItem.WarehouseID;
                    obj.BranchID = pendingVoidItem.BranchID;
                    obj.CompanyID = pendingVoidItem.CompanyID;
                    obj.Sub_Total = pendingVoidItem.Sub_Total;
                    obj.DiscountRate = pendingVoidItem.DiscountRate;
                    obj.DiscountValue = pendingVoidItem.DiscountValue;
                    obj.TypeDis = pendingVoidItem.TypeDis;
                    obj.TaxRate = pendingVoidItem.TaxRate;
                    obj.TaxValue = pendingVoidItem.TaxValue;
                    obj.GrandTotal = pendingVoidItem.GrandTotal;
                    obj.GrandTotal_Sys = pendingVoidItem.GrandTotal_Sys;
                    obj.Tip = pendingVoidItem.Tip;
                    obj.Received = pendingVoidItem.Received;
                    obj.Change = pendingVoidItem.Change;
                    obj.CurrencyDisplay = pendingVoidItem.CurrencyDisplay;
                    obj.DisplayRate = pendingVoidItem.DisplayRate;
                    obj.GrandTotal_Display = pendingVoidItem.GrandTotal_Display;
                    obj.Change_Display = pendingVoidItem.Change_Display;
                    obj.PaymentMeansID = pendingVoidItem.PaymentMeansID;
                    obj.CheckBill = pendingVoidItem.CheckBill;
                    obj.Cancel = pendingVoidItem.Cancel;
                    obj.Delete = pendingVoidItem.Delete;
                    obj.PLCurrencyID = pendingVoidItem.PLCurrencyID;
                    obj.PLRate = pendingVoidItem.PLRate;
                    obj.LocalSetRate = pendingVoidItem.LocalSetRate;
                    obj.IsVoided = pendingVoidItem.IsVoided;
                    obj.TaxOption = pendingVoidItem.TaxOption;
                    obj.RemarkDiscountID = pendingVoidItem.RemarkDiscountID;
                    obj.BuyXAmGetXDisRate = pendingVoidItem.BuyXAmGetXDisRate;
                    obj.BuyXAmGetXDisType = pendingVoidItem.BuyXAmGetXDisType;
                    obj.BuyXAmGetXDisValue = pendingVoidItem.BuyXAmGetXDisValue;
                    obj.BuyXAmountGetXDisID = pendingVoidItem.BuyXAmountGetXDisID;
                    obj.TaxGroupID = pendingVoidItem.TaxGroupID;
                    obj.ChangeCurrenciesDisplay = pendingVoidItem.ChangeCurrenciesDisplay;
                    obj.GrandTotalCurrenciesDisplay = pendingVoidItem.GrandTotalCurrenciesDisplay;
                    obj.OtherPaymentGrandTotal = pendingVoidItem.OtherPaymentGrandTotal;
                    obj.PaymentType = pendingVoidItem.PaymentType;
                    obj.PromoCodeDiscRate = pendingVoidItem.PromoCodeDiscRate;
                    obj.PromoCodeDiscValue = pendingVoidItem.PromoCodeDiscValue;
                    obj.PromoCodeID = pendingVoidItem.PromoCodeID;
                    obj.CardMemberDiscountRate = pendingVoidItem.CardMemberDiscountRate;
                    obj.CardMemberDiscountValue = pendingVoidItem.CardMemberDiscountValue;
                    obj.ID = 0;
                    _dataContext.VoidItems.Add(obj);

                    _dataContext.PendingVoidItems.Remove(pendingVoidItem);
                    _dataContext.SaveChanges();
                }
                t.Commit();
                msg.Approve();
                return msg;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                msg.AddItem(new { ex.Message }, "Message");
                msg.Reject();
                return msg;
            }
        }

        public List<SelectListItem> SelectPriceLists(int selectedId = 0)
        {
            return _dataContext.PriceLists.Where(w => !w.Delete).Select(w => new SelectListItem
            {
                Value = w.ID.ToString(),
                Text = w.Name,
                Selected = w.ID == selectedId
            }).ToList();
        }

        public InvoiceDisplay ToInvoiceDisplay(Order order, List<DisplayPayCurrencyModel> currencies)
        {
            var lineItems = new List<LineItemDisplay>();
            var alt = currencies.FirstOrDefault(i => i.IsActive) ?? new DisplayPayCurrencyModel();
            if (order.OrderDetail.Count > 0)
            {
                lineItems = order.OrderDetail.Where(od => od.Qty != 0)
                .Select(od => new LineItemDisplay
                {
                    LineID = od.LineID,
                    ParentLineID = od.ParentLineID,
                    ItemID = od.ItemID,
                    Code = od.Code,
                    ItemName = od.KhmerName,
                    ItemName2 = od.EnglishName,
                    ItemType = od.ItemType,
                    Qty = od.Qty.ToString(),
                    UoM = od.Uom,
                    UnitPrice = od.UnitPrice.ToString(),
                    DiscountRate = od.DiscountRate.ToString(),
                    DiscountValue = od.DiscountValue.ToString(),
                    TaxValue = od.TaxValue.ToString("N3"),
                    Total = od.Total.ToString("N3")
                }).ToList();
            }

            return new InvoiceDisplay
            {
                OrderNo = order.OrderNo,
                BaseCurrency = alt.BaseCurrency,
                AltCurrency = alt.AltCurrency,
                ExchangeRate = (double)alt.AltRate,
                DiscountRate = (decimal)order.DiscountRate,
                DiscountValue = (decimal)order.DiscountValue,
                TaxValue = (decimal)order.TaxValue,
                SubTotal = (decimal)order.Sub_Total,
                GrandTotal = (decimal)order.GrandTotal,
                Recevied = (decimal)order.Received,
                Changed = (decimal)order.Change,
                LineItems = lineItems
            };
        }
        public dynamic GetReprintCloseShifts(string DateFrom, string DateTo, string keyword = "")
        {
            List<CloseShift> closeShiftFilter = new();
            if (DateFrom != null && DateTo != null)
            {
                closeShiftFilter = _dataContext.CloseShift.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            var data = (from c in closeShiftFilter
                        join b in _dataContext.Branches on c.BranchID equals b.ID
                        join u in _dataContext.UserAccounts on c.UserID equals u.ID
                        join e in _dataContext.Employees on u.EmployeeID equals e.ID
                        join cp in _dataContext.Company on b.CompanyID equals cp.ID
                        join cr in _dataContext.Currency on c.SysCurrencyID equals cr.ID
                        let receipt = _dataContext.Receipt.FirstOrDefault(s => s.ReceiptID == c.Trans_To)
                        let receiptMemo = _dataContext.ReceiptMemo.FirstOrDefault(s => s.ReceiptMemoNo == receipt.ReceiptNo) ?? new CKBS.Models.Services.POS.KVMS.ReceiptMemo()
                        select new
                        {
                            c.ID,
                            EmpName = e.Name,
                            Trans = c.Trans_From + "/" + c.Trans_To + "/" + c.UserID,
                            DateIn = c.DateIn.ToString("dd-MM-yyyy"),
                            c.TimeIn,
                            DateOut = c.DateOut.ToString("dd-MM-yyyy"),
                            c.TimeOut,
                            CashInAmountSys = string.Format("{0:#,0.000}", c.CashInAmount_Sys),
                            SaleAmountSys = string.Format("{0:#,0.000}", c.SaleAmount_Sys - receiptMemo.SubTotal),
                            TotalCashOutSys = string.Format("{0:#,0.000}", (c.CashInAmount_Sys + c.SaleAmount_Sys - receiptMemo.SubTotal)),
                            CashOutAmountSys = string.Format("{0:#,0.000}", c.CashOutAmount_Sys),
                            Branch = b.Name,
                            EmpCode = e.Code,
                            SSC = cr.Description,
                            c.UserID
                            //columns for sum data                                 
                        }).ToList();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                data = data.Where(c =>
                            RawWord(c.EmpName).Contains(keyword, ignoreCase)
                            || RawWord(c.Trans).Contains(keyword, ignoreCase)
                            || RawWord(c.DateIn).Contains(keyword, ignoreCase)
                            || RawWord(c.TimeIn).Contains(keyword, ignoreCase)
                            || RawWord(c.DateOut.ToString()).Contains(keyword, ignoreCase)
                            || RawWord(c.TimeOut.ToString()).Contains(keyword, ignoreCase)
                            || RawWord(c.CashInAmountSys).Contains(keyword, ignoreCase)
                            || RawWord(c.TotalCashOutSys).Contains(keyword, ignoreCase)
                            || RawWord(c.CashOutAmountSys).Contains(keyword, ignoreCase)
                            || RawWord(c.SaleAmountSys).Contains(keyword, ignoreCase)
                            || RawWord(c.Branch).Contains(keyword, ignoreCase)
                            || RawWord(c.EmpCode).Contains(keyword, ignoreCase)
                            || RawWord(c.SSC).Contains(keyword, ignoreCase)
                            ).ToList();
            }
            return data;
        }
        private static string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }

        public List<ItemsReturn> CheckStockCanRing(CanRingMaster crm, Warehouse wh)
        {
            List<ItemsReturn> list = new();
            List<ItemsReturn> list_group = new();
            var sbReturn = (from od in crm.CanRingDetials
                            join item in _dataContext.ItemMasterDatas on od.ItemChangeID equals item.ID
                            join ws in _dataContext.WarehouseSummary on item.ID equals ws.ItemID
                            join uom in _dataContext.UnitofMeasures on item.InventoryUoMID equals uom.ID
                            group new { od, item, ws, uom } by new { item.ID } into g
                            let data = g.FirstOrDefault()
                            let uom_defined = _dataContext.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == data.item.GroupUomID && w.AltUOM == data.od.UomChangeID) ?? new GroupDUoM()
                            let instock = (decimal)g.Where(i => i.ws.WarehouseID == crm.WarehouseID).Sum(i => i.ws.InStock)
                            select new ItemsReturn
                            {
                                Code = data.item.Code,
                                Committed = (decimal)data.ws.Committed,
                                ItemID = data.item.ID,
                                InStock = instock,
                                TotalStock = instock - data.od.ChangeQty * (decimal)uom_defined.Factor,
                                KhmerName = $"{data.item.KhmerName} ({data.uom.Name})",
                                LineID = data.od.LineID,
                                OrderQty = data.od.ChangeQty * (decimal)uom_defined.Factor,//(decimal)data.od.Qty,
                                IsSerailBatch = data.item.ManItemBy == ManageItemBy.Batches || data.item.ManItemBy == ManageItemBy.SerialNumbers
                            }).ToList();
            foreach (var item in crm.CanRingDetials)
            {
                var itemChange = _dataContext.ItemMasterDatas.Find(item.ItemChangeID) ?? new ItemMasterData();
                var check = list_group.Find(w => w.ItemID == item.ItemChangeID);
                var item_group_uom = _dataContext.ItemMasterDatas.Include(gu => gu.GroupUOM).Include(uom => uom.UnitofMeasureInv).FirstOrDefault(w => w.ID == item.ItemChangeID);
                var uom_defined = _dataContext.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_group_uom.GroupUomID && w.AltUOM == item.UomChangeID);
                var bom = _dataContext.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemChangeID && w.Active == true);
                var item_material = (from bomd in _dataContext.BOMDetail.Where(w => w.BID == bom.BID && w.NegativeStock == false && w.Detele == false)
                                     join i in _dataContext.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                     join gd in _dataContext.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                     join uom in _dataContext.UnitofMeasures on i.InventoryUoMID equals uom.ID
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
                            var item_warehouse_material = _dataContext.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == crm.WarehouseID && w.ItemID == items.ItemID);
                            if (item_warehouse_material != null)
                            {
                                ItemsReturn item_group = new()
                                {
                                    LineID = item.LineID,
                                    Code = items.Code,
                                    ItemID = items.ItemID,
                                    KhmerName = items.KhmerName + " (" + items.Uom + ")",
                                    InStock = (decimal)item_warehouse_material.InStock,
                                    TotalStock = (decimal)item_warehouse_material.InStock - item.Qty * (decimal)(uom_defined.Factor * items.Qty * items.Factor),
                                    OrderQty = item.Qty * (decimal)(uom_defined.Factor * items.Qty * items.Factor),
                                    Committed = (decimal)item_warehouse_material.Committed,
                                    IsBOM = true,
                                };
                                list_group.Add(item_group);
                            }
                            else if (items.Process != "Standard" && item_warehouse_material == null)
                            {
                                ItemsReturn item_group = new()
                                {
                                    LineID = item.LineID,
                                    Code = itemChange.Code,
                                    ItemID = item.ItemID,
                                    KhmerName = item_group_uom.KhmerName + " (" + item_group_uom.UnitofMeasureInv.Name + ")",
                                    InStock = 0,
                                    OrderQty = item.Qty * (decimal)uom_defined.Factor,
                                    Committed = 0,
                                    IsBOM = true,
                                };
                                list_group.Add(item_group);
                            }
                        }
                    }
                }
                else
                {
                    check.OrderQty += item.Qty * (decimal)uom_defined.Factor;
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
                        };
                        list.Add(item_return);
                    }
                }
            }
            List<ItemsReturn> itemsReturn = new(list.Count + sbReturn.Count);
            itemsReturn.AddRange(list);
            itemsReturn.AddRange(sbReturn);
            return itemsReturn;
        }

        public void IssuseStockCanRing(CanRingMaster crm, List<SerialNumber> serials, List<BatchNo> batches, List<SerialViewModelPurchase> serialViewModelPurchases, List<BatchViewModelPurchase> batchViewModelPurchases)
        {
            _pos.IssuseStockCanRing(crm, serials, batches, serialViewModelPurchases, batchViewModelPurchases);
        }

        public void UpdateFreight(string data, ModelStateDictionary modelState, ModelMessage msg)
        {
            if (data == null)
            {
                modelState.AddModelError("data", "Data is empty, please try agian");
                return;
            }

            List<FreightReceipt> freightReceipts = JsonConvert.DeserializeObject<List<FreightReceipt>>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            foreach (var i in freightReceipts)
            {
                var freight = _dataContext.Freights.Find(i.FreightID);
                if (freight == null) continue;
                freight.FreightReceiptType = i.FreightReceiptType;
                freight.IsActive = i.IsActive;
                _dataContext.SaveChanges();
                modelState.AddModelError("success", "Freight update successfully.");
                msg.Approve();
            }
        }

        public async Task<ItemsReturnObj> CheckStockAsync(Order order, List<SerialNumber> serials, List<BatchNo> batches, string printType)
        {
            // Checking Serial Batch //
            ItemsReturnObj __dataItemTurnOutOfStokes = new ItemsReturnObj
            {
                ErrorMessages = new Dictionary<string, string>()
            };
            List<SerialNumber> serialNumber = serials ?? new();
            List<SerialNumber> _serialNumber = new();
            List<BatchNo> batchNoes = batches ?? new();
            List<BatchNo> _batchNoes = new();

            List<ItemsReturn> list = new();
            List<ItemsReturn> list_group = new();
            var orderDetails = order.OrderDetail.Where(i => !i.IsKsms && !i.IsKsmsMaster).ToList();
            #region Checking Stock
            var sbReturn = (from od in orderDetails
                            join item in _dataContext.ItemMasterDatas on od.ItemID equals item.ID
                            join ws in _dataContext.WarehouseSummary on item.ID equals ws.ItemID
                            join uom in _dataContext.UnitofMeasures on item.InventoryUoMID equals uom.ID
                            group new { od, item, ws, uom } by new { item.ID } into g
                            let data = g.FirstOrDefault()
                            let uom_defined = _dataContext.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == data.item.GroupUomID && w.AltUOM == data.od.UomID) ?? new GroupDUoM()
                            let instock = (decimal)g.Where(i => i.ws.WarehouseID == order.WarehouseID).Sum(i => i.ws.InStock)
                            select new ItemsReturn
                            {
                                Code = data.item.Code,
                                Committed = (decimal)data.ws.Committed,
                                ItemID = data.item.ID,
                                InStock = instock,
                                TotalStock = (instock - (decimal)data.ws.Committed) - (decimal)(data.od.PrintQty * uom_defined.Factor),
                                KhmerName = $"{data.item.KhmerName} ({data.uom.Name})",
                                LineID = data.od.LineID,
                                OrderQty = (decimal)(data.od.PrintQty * uom_defined.Factor),//(decimal)data.od.Qty,
                                IsSerailBatch = data.item.ManItemBy == ManageItemBy.Batches || data.item.ManItemBy == ManageItemBy.SerialNumbers
                            }).ToList();
            if (_clientPos.EnableExternalSync())
            {
                return new ItemsReturnObj
                {
                    ItemsReturns = sbReturn.Where(i => i.IsSerailBatch).ToList(),
                };
            }

            var wh = await _dataContext.Warehouses.FindAsync(order.WarehouseID) ?? new Warehouse();
            if (wh.ID <= 0)
            {
                __dataItemTurnOutOfStokes.ErrorMessages = new Dictionary<string, string>{
                    {"WarehouseID", "Warehouse is invalid."}
                };
            }

            foreach (var item in orderDetails)
            {
                var check = list_group.Find(w => w.ItemID == item.ItemID);
                var item_group_uom = _dataContext.ItemMasterDatas.Include(gu => gu.GroupUOM).Include(uom => uom.UnitofMeasureInv).FirstOrDefault(w => w.ID == item.ItemID);
                var uom_defined = _dataContext.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_group_uom.GroupUomID && w.AltUOM == item.UomID);
                var bom = _dataContext.BOMaterial.FirstOrDefault(w => w.ItemID == item.ItemID && w.Active == true);
                var item_material = (from bomd in _dataContext.BOMDetail.Where(w => w.BID == bom.BID && w.NegativeStock == false && w.Detele == false)
                                     join i in _dataContext.ItemMasterDatas.Where(im => !im.Delete) on bomd.ItemID equals i.ID
                                     join gd in _dataContext.GroupDUoMs on bomd.UomID equals gd.AltUOM
                                     join uom in _dataContext.UnitofMeasures on i.InventoryUoMID equals uom.ID
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
                            var item_warehouse_material = _dataContext.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == order.WarehouseID && w.ItemID == items.ItemID);
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
                        };
                        list.Add(item_return);
                    }
                }
            }

            List<ItemsReturn> itemsReturn = new(list.Count + sbReturn.Count);
            itemsReturn.AddRange(list);
            itemsReturn.AddRange(sbReturn);
            List<ItemsReturn> itemSBOnly = itemsReturn.Where(i => i.IsSerailBatch && i.TotalStock >= 0 && !i.IsBOM).ToList();
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
                __dataItemTurnOutOfStokes.ItemsReturns = itemsReturn;
                return __dataItemTurnOutOfStokes;
            }

            if (orderDetails.Count > 0 && itemSBOnly.Count > 0)
            {
                if (printType == "Pay")
                {
                    _POSSerialBatch.CheckItemSerail(order, orderDetails, serialNumber, "OrderID");
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
                        __dataItemTurnOutOfStokes.TypeOfSerialBatch = TypeOfSerialBatch.Serial;
                        __dataItemTurnOutOfStokes.ItemSerialBatches = _serialNumber;
                    }

                    _POSSerialBatch.CheckItemBatch(order, orderDetails, batchNoes, "OrderID");
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
                        __dataItemTurnOutOfStokes.TypeOfSerialBatch = TypeOfSerialBatch.Batch;
                        __dataItemTurnOutOfStokes.ItemSerialBatches = _batchNoes;
                    }
                }
                else
                {
                    if (serialNumber.Any()) _serialNumber = serialNumber;
                    if (batchNoes.Any()) _batchNoes = batchNoes;
                }
            }
            __dataItemTurnOutOfStokes.ItemsReturns = __dataItemTurnOutOfStokes.ItemsReturns ?? new List<ItemsReturn>();
            return __dataItemTurnOutOfStokes;
        }
        #endregion
        #region GetSlideImageNames
        public string[] GetSlideImageNames()
        {
            var filePath = Path.Combine(_hostEnv.WebRootPath, "Images/slides");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string[] fileNames = Directory.GetFiles(filePath)
                .Select(f => "/Images/slides/" + Path.GetFileName(f)).ToArray();
            return fileNames;
        }
        #endregion
    }
}
