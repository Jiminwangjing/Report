using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.POS.service;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Services.Administrator.CanRingSetup;
using KEDI.Core.Premise.Models.Services.CanRingExchangeAdmin;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    [Privilege]
    public class CanRingController : Controller
    {
        private readonly ICanRingRepository _canRing;
        private readonly UtilityModule _utility;
        private readonly DataContext _context;

        public CanRingController(ICanRingRepository canRing, UtilityModule utility, DataContext context)
        {
            _canRing = canRing;
            _utility = utility;
            _context = context;
        }
        [Privilege("CR001")]
        public IActionResult Index()
        {
            ViewBag.CanRingIndex = "highlight";
            ViewBag.PriceList = new SelectList(_canRing.PriceLists, "ID", "Name");
            return View();
        }
        
        [Privilege("CR002")]
        [HttpGet]
        public IActionResult CanRingSetup(int id)
        {
            ViewBag.CanRingIndex = "highlight";
            ViewBag.Page = "Setup";
            var disCurr = _canRing.Display;
            return View(new { Display = disCurr, ParamID = id });
        }
        [HttpGet]
        public async Task<IActionResult> GetCanRingReport(string dateFrom, string dateTo, int customerId, int paymentMeansId)
        {
            var data = await _canRing.GetCanRingReportAsync(dateFrom, dateTo, customerId, paymentMeansId, GetUserId());
            return Ok(data);
        }

        [HttpGet]
        [Privilege("CR003")]
        public IActionResult ExchangeCanRing()
        {
            ViewBag.ExchangeCanRing = "highlight";
            ViewBag.Warehouse = new SelectList(_canRing.Warehouses, "ID", "Name");
            ViewBag.PriceList = new SelectList(_canRing.PriceLists, "ID", "Name");
            ViewBag.PaymentMeans = new SelectList(_canRing.PaymentMeans, "ID", "Type");
            return View(new ExchangeCanRingParam { SeriesEC = _utility.GetSeries("EC"), GenSetting = _utility.GetGeneralSettingAdmin() });
        }
        [HttpGet]
        [Privilege("CR003")]
        public IActionResult ExchangeCanRingHistory()
        {
            ViewBag.ExchangeCanRing = "highlight";
            ViewBag.PriceList = new SelectList(_canRing.PriceLists, "ID", "Name");
            ViewBag.PaymentMeans = new SelectList(_canRing.PaymentMeans, "ID", "Type");
            ViewBag.Branches = new SelectList(_canRing.Branches, "ID", "Name");
            return View();
        }
        [HttpGet]
        [Privilege("CR003")]
        public async Task<IActionResult> PrintExchangeCanRingHistory(int id)
        {
            var data = await _canRing.PrintExchangeCanRingHistoryAsync(id);
            return new ViewAsPdf(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetCanRingsSetupDefault()
        {
            List<CanRing> canRings = await _canRing.GetCanRingsSetupDefaultAsync();
            return Ok(canRings);
        }
        [HttpGet]
        public IActionResult GetUserAndWarehouse(int branchId)
        {
            var warehouse = _canRing.Warehouses.Where(i=> i.BranchID == branchId).ToList();
            var users = _canRing.Users.Where(i => i.BranchID == branchId).ToList();
            return Ok(new { warehouse, users});
        }
        [HttpGet]
        public async Task<IActionResult> GetCanRingsSetup(int plId = 0, ActiveCanRingType active = ActiveCanRingType.Active)
        {
            List<CanRing> canRings = await _canRing.GetCanRingsSetupAsync(plId, active);
            return Ok(canRings);
        }
        [HttpGet]
        public async Task<IActionResult> GetExchangeCanRingHistory(string param)
        {
            HistoryExchangeCanRingParamFilter canRingParamFilter = JsonConvert.DeserializeObject<HistoryExchangeCanRingParamFilter>(param, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var data = await _canRing.GetExchangeCanRingHistoryAsync(canRingParamFilter);
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetCanRingSetup(int id, string name)
        {
            List<CanRing> canRings = await _canRing.GetCanRingSetupAsync(id, name);
            return Ok(canRings);
        }
        [HttpGet]
        public async Task<IActionResult> GetCanRingSetupDefault()
        {
            CanRing canRings = await _canRing.GetCanRingSetupDefaultAsync();
            return Ok(canRings);
        }
        [HttpGet]
        public async Task<IActionResult> GetItemMasterData(int plid)
        {
            var data = await _canRing.GetItemMasterDataAsync(plid);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUpdate(List<CanRing> data)
        {
            ModelMessage msg = new();
            bool isSuccess = await _canRing.CreateUpdateAsync(data, ModelState);
            if (isSuccess)
            {
                ModelState.AddModelError("Success", "Operation is completed!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpPost]
        public async Task<IActionResult> UpdateActive(int id, bool active)
        {
            var data = await _context.CanRings.FindAsync(id);
            if (data == null) return Ok();
            data.IsActive = active;
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet]
        public IActionResult GetVendors()
        {
            var data = _canRing.BusinessPartners;
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetCustomers()
        {
            var data = _canRing.Customers;
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetCurrency(int plid)
        {
            return Ok(await _canRing.Currency(plid));
        }
        [HttpGet]
        public async Task<IActionResult> FindExchangeCanRing(int seriesId, string invoiceNumber)
        {
            var data = await _canRing.GetExchangeCanRingMasterAsync(seriesId, invoiceNumber);
            return Ok(data);
        }
        [HttpPost]
        public IActionResult SubmitExchangeCanRing(ExchangeCanRingParam data)
        {
            ModelMessage msg = new();
            var crm = data.ExchangeCanRingMaster;
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
            ExchangeCanringValidation(data.ExchangeCanRingMaster);
            if (ModelState.IsValid)
            {
                var wh = _context.Warehouses.Find(crm.WarehouseID) ?? new Warehouse();
                var itemsReturn = _canRing.CheckStockExchangeCanring(crm, wh);
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
                if (crm.ExchangeCanRingDetails.Count > 0 && itemSBOnly.Count > 0)
                {
                    serialNumber = data.SerialNumbers ?? new List<SerialNumber>();

                    _canRing.CheckCanRingItemSerailOut(crm, crm.ExchangeCanRingDetails, serialNumber, "ID");
                    serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in serialNumber.ToList())
                    {
                        foreach (var i in crm.ExchangeCanRingDetails)
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
                    batchNoes = data.BatchNos ?? new List<BatchNo>();
                    _canRing.CheckCanRingItemBatchOut(crm, crm.ExchangeCanRingDetails, batchNoes, "ID");
                    batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in batchNoes.ToList())
                    {
                        foreach (var i in crm.ExchangeCanRingDetails)
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
                serialViewModelPurchases = data.SerialPurViews ?? new List<SerialViewModelPurchase>();

                _canRing.CheckCanRingItemSerailIn(crm, crm.ExchangeCanRingDetails, serialViewModelPurchases);
                serialViewModelPurchases = serialViewModelPurchases
                    .GroupBy(i => i.ItemID)
                    .Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in serialViewModelPurchases.ToList())
                {
                    foreach (var i in crm.ExchangeCanRingDetails)
                    {
                        if (j.ItemID == i.ItemChangeID)
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
                batchViewModelPurchases = data.BatchPurViews ?? new List<BatchViewModelPurchase>();
                _canRing.CheckCanRingItemBatchIn(crm, crm.ExchangeCanRingDetails, batchViewModelPurchases);
                batchViewModelPurchases = batchViewModelPurchases
                    .GroupBy(i => i.ItemID)
                    .Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in batchViewModelPurchases.ToList())
                {
                    foreach (var i in crm.ExchangeCanRingDetails)
                    {
                        if (j.ItemID == i.ItemChangeID)
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
                _context.ExchangeCanRingMasters.Update(crm);
                _context.SaveChanges();
                _canRing.IssuseStockExchangeCanRing(data.ExchangeCanRingMaster, _serialNumber, _batchNoes, serialViewModelPurchases, batchViewModelPurchases);
                t.Commit();
                msg.Approve();
                //msg.AddItem(seriesCR, "seriesCR");
            }
            return Ok(msg.Bind(ModelState));
        }
        
        void ExchangeCanringValidation(ExchangeCanRingMaster data)
        {
            data.ExchangeCanRingDetails ??= new List<ExchangeCanRingDetail>();
            if (data.CusId == 0) ModelState.AddModelError("Vendor", "Vendor is required!");
            if(data.WarehouseID == 0) ModelState.AddModelError("Warehouse", "Warehouse is required!");
            if(data.PriceListID == 0) ModelState.AddModelError("PriceList", "Price List is required!");
            if(data.PaymentMeanID == 0) ModelState.AddModelError("PaymentMean", "Payment Means is required!");
            if(data.ExchangeCanRingDetails.Count == 0) ModelState.AddModelError("ExchangeCanRingDetails", "Exchange Can Ring Details is required!");
        }

        private int GetUserId()
        {
            _ = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _userId);
            return _userId;
        }
        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserId())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }
    }
}
