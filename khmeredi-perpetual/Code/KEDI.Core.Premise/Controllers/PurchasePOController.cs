using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CKBS.AppContext;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using CKBS.Models.ServicesClass;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Administrator.General;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Services.Responsitory;
using CKBS.Models.Services.Administrator.Inventory;
using KEDI.Core.Localization.Resources;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Helpers.Enumerations;
using static KEDI.Core.Premise.Controllers.PurchaseRequestController;
using CKBS.Models.ReportClass;
using Microsoft.AspNetCore.Mvc.Rendering;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CKBS.Controllers
{
    //[Authorize(Policy = "Permission")]
    [Privilege]
    public class PurchasePOController : Controller
    {
        private readonly DataContext _context;
        private readonly IPurchaseRepository _ipur;
        private readonly IGoodRecepitPo _recepitPo;
        private readonly CultureLocalizer _culLocal;
        private readonly UtilityModule _formatNumber;
        public PurchasePOController(
            DataContext context,
            IGoodRecepitPo recepitPo,
            IPurchaseRepository ipur,
            CultureLocalizer culLocal, UtilityModule formatNumber)
        {
            _context = context;
            _recepitPo = recepitPo;
            _ipur = ipur;
            _culLocal = culLocal;
            _formatNumber = formatNumber;
        }
        // GET: /<controller>/
        [Privilege("A022")]
        public IActionResult PurchasePO()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase Order";
            ViewBag.Menu = "show";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchasePO = "highlight";
            return View(new { seriesPD = _formatNumber.GetSeries("PD"), seriesJE = _formatNumber.GetSeries("JE"), genSetting = _formatNumber.GetGeneralSettingAdmin() });
        }
        public Dictionary<int, string> Typevatt => EnumHelper.ToDictionary(typeof(PVatType));
        [HttpGet]
        public async Task<IActionResult> GetFreights()
        {
            var data = await _ipur.GetFreightsAsync();
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetBusinessPartners()
        {
            var list = await _ipur.GetBusinessPartnersAsync();
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetCurrencyDefualt()
        {
            var list = await _ipur.GetCurrencyDefualtAsync();
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetWarehouses(int ID)
        {
            var list = await _ipur.GetWarehousesAsync(ID);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetItemMasterData(int ID)
        {
            var list = await _ipur.GetItemMasterDataAsync(ID, GetCompany().ID);
            return Ok(list.OrderBy(o => o.Code));
        }
        [HttpGet]
        public IActionResult GetItemDetails(int itemid = 0, int curId = 0, string barcode = "")
        {
            var data = _ipur.GetItemDetails(GetCompany(), itemid, curId, barcode);
            return data == null && barcode != ""
                ? Ok(new { IsError = true, Error = $"No item with this Barcode \"{barcode}\"" })
                : Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> Getcurrency()
        {
            var list = await _ipur.GetcurrencyAsync(GetCompany().SystemCurrencyID);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetFilterLocaCurrency(int CurrencyID)
        {
            var list = await _ipur.GetExchangeRates(CurrencyID);
            return Ok(list);
        }
        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst("UserID").Value, out int _id);
            return _id;
        }
        private int GetBranchID()
        {
            _ = int.TryParse(User.FindFirst("BranchID").Value, out int _id);
            return _id;
        }

        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }
        private void UpdateSourceCopy(IEnumerable<dynamic> details, dynamic copyMaster, IEnumerable<dynamic> copyDedails, PurCopyType copyType)
        {
            foreach (var cd in copyDedails)
            {
                foreach (var d in details)
                {
                    switch (copyType)
                    {
                        case PurCopyType.PurOrder:
                            if (d.LineID == cd.PurchaseOrderDetailID)
                            {
                                if (cd.OpenQty > 0)
                                {
                                    cd.OpenQty -= d.Qty;
                                }
                            }
                            break;
                        case PurCopyType.PurReserve:
                            if (d.LineID == cd.ID)
                            {
                                if (cd.OpenQty > 0)
                                {
                                    cd.OpenQty -= d.Qty;
                                }
                            }
                            break;

                    }

                }
            }
            double sum = copyDedails.ToList().Sum(x => (double)x.OpenQty);
            if (sum <= 0)
            {
                copyMaster.Status = "close";
            }
            _context.SaveChanges();
        }
        private void ValidateSummary(GoodsReciptPO master)
        {
            var postingPeriod = _context.PostingPeriods.Where(w => w.PeroidStatus == PeroidStatus.Unlocked).ToList();
            if (postingPeriod.Count <= 0)
            {
                ModelState.AddModelError("PostingDate", "PeroiStatus is closed or locked");
            }
            else
            {
                bool isValidPostingDate = false,
                    isValidDueDate = false,
                    isValidDocumentDate = false;
                foreach (var item in postingPeriod)
                {
                    if (DateTime.Compare(master.PostingDate, item.PostingDateFrom) >= 0 && DateTime.Compare(master.PostingDate, item.PostingDateTo) <= 0)
                    {
                        isValidPostingDate = true;
                    }
                    if (DateTime.Compare(master.DueDate, item.DueDateFrom) >= 0 && DateTime.Compare(master.DueDate, item.DueDateTo) <= 0)
                    {
                        isValidDueDate = true;
                    }
                    if (DateTime.Compare(master.DocumentDate, item.DocuDateFrom) >= 0 && DateTime.Compare(master.DocumentDate, item.DocuDateTo) <= 0)
                    {
                        isValidDocumentDate = true;
                    }
                }
                if (!isValidPostingDate)
                {
                    ModelState.AddModelError("PostingDate", "PostingDate is closed or locked");
                }

                if (!isValidDocumentDate)
                {
                    ModelState.AddModelError("DocumentDate", "DocumentDate is closed or locked");
                }
                if (!isValidDueDate)
                {
                    ModelState.AddModelError("DocumentDate", "DueDate is closed or locked");
                }
            }
            if (master.GoodReciptPODetails.Count <= 0)
            {
                ModelState.AddModelError("Details", "Master has no details");
            }
            else
            {
                for (var dt = 0; dt < master.GoodReciptPODetails.Count; dt++)
                {
                    master.GoodReciptPODetails[dt].TotalSys = master.GoodReciptPODetails[dt].Total * master.PurRate;

                    if (master.GoodReciptPODetails[dt].Qty <= 0)
                    {
                        ModelState.AddModelError($"Qty-{dt}", $"Detial at line {dt + 1}, Qty has to be greater than 0!");
                    }
                }
            }
            if (!string.IsNullOrEmpty(master.ReffNo))
            {
                bool isRefExisted = _context.Purchase_APs.AsNoTracking().Any(i => i.ReffNo == master.ReffNo);
                if (isRefExisted)
                {
                    ModelState.AddModelError("RefNo", $"Transaction with Vendor Ref No. \"{master.ReffNo}\" already done.");
                }
            }
            if(master.BranchID==0) {
                ModelState.AddModelError("BranchID","Please Select Branch");
            }
        }

        [HttpPost]
        public IActionResult SavePurchaseGoodPO(string purchase, string Type, string je, int seriesid, string serials = null, string batches = null)
        {
            ModelMessage msg = new();
            if (je == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(msg.Bind(ModelState));
            }
            GoodsReciptPO goodsReciptPO = JsonConvert.DeserializeObject<GoodsReciptPO>(purchase, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Series series_JE = JsonConvert.DeserializeObject<Series>(je, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            SeriesDetail seriesDetail = new();
            goodsReciptPO.SysCurrencyID = GetCompany().SystemCurrencyID;
            //Insert Series
            var series = _context.Series.FirstOrDefault(w => w.ID == goodsReciptPO.SeriesID) ?? new Series();
            goodsReciptPO.GoodReciptPODetails = goodsReciptPO.GoodReciptPODetails.Where(i => i.Qty > 0).ToList();
            var docType = _context.DocumentTypes.FirstOrDefault(i => i.Code == "PD") ?? new DocumentType();
            var whs = _context.Warehouses.Find(goodsReciptPO.WarehouseID) ?? new Warehouse();

            ValidateSummary(goodsReciptPO);
            using (var t = _context.Database.BeginTransaction())
            {
                if (series.ID > 0)
                {
                    //insert seriesDetail
                    seriesDetail.SeriesID = series.ID;
                    seriesDetail.Number = series.NextNo;
                    _context.Update(seriesDetail);
                    //update series
                    string Sno = series.NextNo;
                    long No = long.Parse(Sno);
                    series.NextNo = Convert.ToString(No + 1);
                    _context.Update(series);
                    _context.SaveChanges();
                    //update order
                    var seriesDID = seriesDetail.ID;
                    goodsReciptPO.SeriesDetailID = seriesDID;
                    goodsReciptPO.InvoiceNo = seriesDetail.Number;
                }
                if (ModelState.IsValid)
                {
                    // Checking Serial Batch //
                    List<SerialViewModelPurchase> serialViewModelPurchases = new();
                    List<SerialViewModelPurchase> _serialViewModelPurchases = new();
                    List<BatchViewModelPurchase> batchViewModelPurchases = new();
                    List<BatchViewModelPurchase> _batchViewModelPurchases = new();
                    serialViewModelPurchases = serials != "[]" ? JsonConvert.DeserializeObject<List<SerialViewModelPurchase>>(serials, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }) : serialViewModelPurchases;

                    _ipur.CheckItemSerail(goodsReciptPO, goodsReciptPO.GoodReciptPODetails, serialViewModelPurchases);
                    serialViewModelPurchases = serialViewModelPurchases
                        .GroupBy(i => i.ItemID)
                        .Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in serialViewModelPurchases.ToList())
                    {
                        foreach (var i in goodsReciptPO.GoodReciptPODetails.ToList())
                        {
                            if (j.ItemID == i.ItemID)
                            {
                                _serialViewModelPurchases.Add(j);
                            }
                        }
                    }
                    bool isHasSerialItem = _serialViewModelPurchases.Any(i => i.TotalCreated != i.TotalNeeded);
                    if (isHasSerialItem)
                    {
                        return Ok(new { IsSerail = true, Data = _serialViewModelPurchases });
                    }
                    // checking batch items
                    batchViewModelPurchases = batches != "[]" ? JsonConvert.DeserializeObject<List<BatchViewModelPurchase>>(batches, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }) : batchViewModelPurchases;
                    _ipur.CheckItemBatch(goodsReciptPO, goodsReciptPO.GoodReciptPODetails, batchViewModelPurchases);
                    batchViewModelPurchases = batchViewModelPurchases
                        .GroupBy(i => i.ItemID)
                        .Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in batchViewModelPurchases.ToList())
                    {
                        foreach (var i in goodsReciptPO.GoodReciptPODetails.ToList())
                        {
                            if (j.ItemID == i.ItemID)
                            {
                                _batchViewModelPurchases.Add(j);
                            }
                        }
                    }
                    bool isHasBatchItems = _batchViewModelPurchases.Any(i => i.TotalNeeded != i.TotalCreated);
                    if (isHasBatchItems)
                    {
                        return Ok(new { IsBatch = true, Data = _batchViewModelPurchases });
                    }
                    if (Type == "Add")
                    {
                        if (ModelState.IsValid)
                        {
                            goodsReciptPO.LocalCurID = GetCompany().LocalCurrencyID;
                            goodsReciptPO.LocalSetRate = localSetRate;
                            goodsReciptPO.CompanyID = GetCompany().ID;
                            goodsReciptPO.Status = "open";
                           
                            goodsReciptPO.UserID = GetUserID();
                            _context.GoodsReciptPOs.Update(goodsReciptPO);
                            _context.SaveChanges();
                            UpdateFreight(goodsReciptPO);
                            _recepitPo.GoodReceiptStockPO(goodsReciptPO.ID, Type, _serialViewModelPurchases, _batchViewModelPurchases);
                            ModelState.AddModelError("success", "Item save successfully.");
                            msg.Approve();
                        }
                    }
                    else if (Type == "PO")
                    {
                        List<GoodReciptPODetail> Comfirn = new();
                        List<GoodReciptPODetail> List = new();
                        foreach (var items in goodsReciptPO.GoodReciptPODetails.ToList())
                        {
                            var checkOrdered = _context.WarehouseDetails.FirstOrDefault(w => w.ItemID == items.ItemID && w.UomID == items.UomID && w.WarehouseID == goodsReciptPO.WarehouseID);
                            if (checkOrdered == null)
                            {
                                List.Add(items);
                            }
                            else
                            {
                                if (items.Qty > 0)
                                {
                                    Comfirn.Add(items);
                                }
                            }
                        }
                        if (List.Count > 0)
                        {
                            return Ok(List);
                        }
                        else
                        {
                            if (ModelState.IsValid)
                            {
                                if (goodsReciptPO.ID == 0)
                                {
                                    var purOrder = _context.PurchaseOrders.FirstOrDefault(s => s.PurchaseOrderID == goodsReciptPO.BaseOnID) ?? new PurchaseOrder();
                                    var prOrd = _context.PurchaseOrderDetails.Where(s => s.PurchaseOrderID == purOrder.PurchaseOrderID).ToList();
                                    if (purOrder.PurchaseOrderID != 0)
                                    {

                                        UpdateSourceCopy(goodsReciptPO.GoodReciptPODetails, purOrder, prOrd, PurCopyType.PurOrder);
                                    }
                                    goodsReciptPO.LocalCurID = GetCompany().LocalCurrencyID;
                                    goodsReciptPO.LocalSetRate = localSetRate;
                                    goodsReciptPO.CompanyID = GetCompany().ID;
                                    goodsReciptPO.Status = "open";
                                    _context.GoodsReciptPOs.Update(goodsReciptPO);
                                    _context.SaveChanges();
                                    UpdateFreight(goodsReciptPO);
                                    _recepitPo.GoodReceiptStockPO(goodsReciptPO.ID, Type, _serialViewModelPurchases, _batchViewModelPurchases);
                                    // Checking maximun Invoice   
                                    _context.SaveChanges();
                                }
                                ModelState.AddModelError("success", "Item save successfully.");
                                msg.Approve();
                            }
                        }
                    }
                    else if (Type == "APR")
                    {
                        List<GoodReciptPODetail> Comfirn = new();
                        List<GoodReciptPODetail> List = new();
                        foreach (var items in goodsReciptPO.GoodReciptPODetails.ToList())
                        {
                            var checkOrdered = _context.WarehouseDetails.FirstOrDefault(w => w.ItemID == items.ItemID && w.UomID == items.UomID && w.WarehouseID == goodsReciptPO.WarehouseID);
                            if (checkOrdered == null)
                            {
                                List.Add(items);
                            }
                            else
                            {
                                if (items.Qty > 0)
                                {
                                    Comfirn.Add(items);
                                }
                            }
                        }
                        if (List.Count > 0)
                        {
                            return Ok(List);
                        }
                        else
                        {
                            if (ModelState.IsValid)
                            {
                                var number = goodsReciptPO.CopyKey.Split("-")[1];
                                var ARMaster = _context.PurchaseAPReserves.FirstOrDefault(s => s.ID == goodsReciptPO.BaseOnID);
                                goodsReciptPO.CopyType = PurCopyType.PurReserve;
                                goodsReciptPO.BaseOnID = ARMaster.ID;
                                if (ARMaster != null)
                                {
                                    var arrd = _context.PurchaseAPReserveDetails.Where(s => s.PurchaseAPReserveID == goodsReciptPO.BaseOnID).ToList();

                                    UpdateSourceCopy(goodsReciptPO.GoodReciptPODetails, ARMaster, arrd, PurCopyType.PurReserve);


                                }
                                if (goodsReciptPO.ID == 0)
                                {
                                    var purOrder = ARMaster;
                                    if (ARMaster != null)
                                    {
                                        purOrder.CopyToNote = CopyToNote.GRPO;
                                    }



                                    goodsReciptPO.LocalCurID = GetCompany().LocalCurrencyID;
                                    goodsReciptPO.LocalSetRate = localSetRate;
                                    goodsReciptPO.CompanyID = GetCompany().ID;
                                    goodsReciptPO.Status = "open";
                                    _context.GoodsReciptPOs.Update(goodsReciptPO);
                                    _context.SaveChanges();
                                    UpdateFreight(goodsReciptPO);
                                    _recepitPo.GoodReceiptStockPO(goodsReciptPO.ID, Type, _serialViewModelPurchases, _batchViewModelPurchases);
                                    // Checking maximun Invoice   
                                    _context.SaveChanges();
                                }
                                ModelState.AddModelError("success", "Item save successfully.");
                                msg.Approve();
                            }
                        }
                    }
                    t.Commit();
                }
            }
            return Ok(msg.Bind(ModelState));
        }


        [HttpGet]
        public async Task<IActionResult> GetAPReserve(int vendorId)
        {
            var data = await _context.PurchaseAPReserves.Where(i => i.Status == "open" && i.VendorID == vendorId && i.BranchID == GetBranchID() && i.CompanyID == GetCompany().ID).ToListAsync();
            var list = await _ipur.GetAllAPreserveOAsync(data);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> FindPurchaseGoodReceiptPO(int seriesID, string number)
        {
            var data = await _ipur.FindPurchasePOAsync(seriesID, number, GetCompany().ID);
            if (data.PurchasePO == null)
            {
                return Ok(new { Error = true, Message = $"Cannot find invoice \"{number}\"" });
            }
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetPurchaseorder(int vendorId)
        {
            var data = await _context.PurchaseOrders.Where(i => i.Status == "open" && i.VendorID == vendorId && i.BranchID == GetBranchID() && i.CompanyID == GetCompany().ID).ToListAsync();
            var list = await _ipur.GetAllPurPOAsync(data);
            return Ok(list);
        }
        private void UpdateFreight(GoodsReciptPO goodsReciptPO)
        {
            var freight = goodsReciptPO.FreightPurchaseView;
            if (freight != null)
            {
                //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                freight.PurID = goodsReciptPO.ID;
                freight.PurType = PurCopyType.GRPO;
                freight.OpenExpenceAmount = freight.ExpenceAmount;
                _context.FreightPurchases.Update(freight);
                _context.SaveChanges();
            }
        }
        [HttpPost]
        public IActionResult CreateSerialAutomatically(string data)
        {
            SerialDetailAutoCreation serialDetailAuto = JsonConvert
                .DeserializeObject<SerialDetailAutoCreation>(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            List<SerialDetialViewModelPurchase> serialDetialViewModelPurchase = new();
            _ipur.GetStringCreation(serialDetialViewModelPurchase, serialDetailAuto);
            return Ok(serialDetialViewModelPurchase);
        }

        [HttpPost]
        public IActionResult CreateBatchAutomatically(string data)
        {
            BatchDetailAutoCreation batchDetailAuto = JsonConvert
                .DeserializeObject<BatchDetailAutoCreation>(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            List<BatchDetialViewModelPurchase> batchDetialViewModelPurchase = new();
            _ipur.GetStringCreation(batchDetialViewModelPurchase, batchDetailAuto);
            return Ok(batchDetialViewModelPurchase);
        }
        [HttpPost]
        public IActionResult CheckAutoStringCreation(string autoStringCreation)
        {
            ModelMessage msg = new();
            List<AutomaticStringCreation> automaticStringCreations = JsonConvert
                .DeserializeObject<List<AutomaticStringCreation>>(autoStringCreation, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            if (automaticStringCreations.Where(i => i.TypeInt == TypeAutomaticStringCreation.Number).Count() >= 2)
            {
                ModelState.AddModelError("TypeInt", _culLocal["Type cannot be all \"Number\""]);
            }
            if (automaticStringCreations.Where(i => i.TypeInt == TypeAutomaticStringCreation.String).Count() >= 2)
            {
                ModelState.AddModelError("TypeInt", _culLocal["Type cannot be all \"String\""]);
            }

            for (var i = 0; i < automaticStringCreations.Count; i++)
            {
                if (
                    automaticStringCreations[i].TypeInt == TypeAutomaticStringCreation.Number &&
                    automaticStringCreations[i].OperationInt == OperationAutomaticStringCreation.NoOperation
                    )
                {
                    ModelState.AddModelError($"TpyeOperation{i}", _culLocal[$"At Line {i + 1} Type \"Number\", so \"Operation\" cannot be \"No Operation.\""]);
                }
                if (String.IsNullOrEmpty(automaticStringCreations[i].Name))
                {
                    ModelState.AddModelError($"Name{i}", _culLocal[$"At Line {i + 1} Name cannot be empty"]);
                }
            }

            if (ModelState.IsValid)
            {
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpPost]
        public IActionResult CheckSerailNumber(string serails)
        {
            ModelMessage msg = new();
            List<SerialViewModelPurchase> _serails = JsonConvert
                .DeserializeObject<List<SerialViewModelPurchase>>(serails, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            List<WarehouseDetail> whsd = _context.WarehouseDetails.ToList();
            foreach (var item in _serails)
            {
                for (var i = 0; i < item.SerialDetialViewModelPurchase.Count; i++)
                {
                    var data = item.SerialDetialViewModelPurchase[i];


                    foreach (var _item in item.SerialDetialViewModelPurchase)
                    {
                        if (string.IsNullOrWhiteSpace(_item.SerialNumber))
                        {
                            ModelState.AddModelError(
                                   $"SerailNumber-{i}",
                                   $"Item Name \"{item.ItemName}\", SerailNumber at line {i + 1} is empty existed");
                            return Ok(msg.Bind(ModelState));
                        }
                        if (data.LineID != _item.LineID)
                        {
                            if (data.SerialNumber == _item.SerialNumber)
                            {
                                ModelState.AddModelError(
                                    $"SerailNumber-{i}",
                                    $"Item Name \"{item.ItemName}\", SerailNumber at line {i + 1} is already existed");
                                return Ok(msg.Bind(ModelState));
                            }
                            if (data.LotNumber == _item.LotNumber && !String.IsNullOrEmpty(data.LotNumber))
                            {
                                ModelState.AddModelError(
                                    $"SerailNumber-{i}",
                                    $"Item Name \"{item.ItemName}\", Lot Number at line {i + 1} is already existed");
                                return Ok(msg.Bind(ModelState));
                            }
                            if (data.MfrSerialNo == _item.MfrSerialNo && !String.IsNullOrEmpty(data.MfrSerialNo))
                            {
                                ModelState.AddModelError(
                                    $"SerailNumber-{i}",
                                    $"Item Name \"{item.ItemName}\", Mfr Serial No. at line {i + 1} is already existed");
                                return Ok(msg.Bind(ModelState));
                            }
                        }
                    }
                    if (whsd.Any(i => i.SerialNumber == data.SerialNumber))
                    {
                        ModelState.AddModelError(
                                    $"SerailNumber-{i}",
                                    $"Item Name \"{item.ItemName}\", Serail Number at line {i + 1} is already existed");
                        return Ok(msg.Bind(ModelState));
                    }
                    if (whsd.Any(i => i.LotNumber == data.LotNumber) && !String.IsNullOrEmpty(data.LotNumber))
                    {
                        ModelState.AddModelError(
                                    $"SerailNumber-{i}",
                                    $"Item Name \"{item.ItemName}\", Lot Number at line {i + 1} is already existed");
                        return Ok(msg.Bind(ModelState));
                    }
                    if (whsd.Any(i => i.MfrSerialNumber == data.MfrSerialNo) && !String.IsNullOrEmpty(data.MfrSerialNo))
                    {
                        ModelState.AddModelError(
                                    $"SerailNumber-{i}",
                                    $"Item Name \"{item.ItemName}\", Mfr Serial No. at line {i + 1} is already existed");
                        return Ok(msg.Bind(ModelState));
                    }
                }

            }
            msg.Approve();
            return Ok(msg.Bind(ModelState));
        }
        [HttpPost]
        public IActionResult CheckBatchNumber(string batches)
        {
            ModelMessage msg = new();
            List<BatchViewModelPurchase> _serails = JsonConvert
                .DeserializeObject<List<BatchViewModelPurchase>>(batches, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            List<WarehouseDetail> whsd = _context.WarehouseDetails.ToList();
            foreach (var item in _serails)
            {
                item.BatchDetialViewModelPurchases = item.BatchDetialViewModelPurchases.
                    Where(i => i.Qty > 0 && !string.IsNullOrEmpty(i.Batch)).ToList();
                for (var i = 0; i < item.BatchDetialViewModelPurchases.Count; i++)
                {
                    var data = item.BatchDetialViewModelPurchases[i];
                    if (String.IsNullOrEmpty(data.Batch))
                    {
                        ModelState.AddModelError($"BatchNumber{i}", $"Item Name \"{item.ItemName}\" missing Batch at line {i + 1}");
                        return Ok(msg.Bind(ModelState));
                    }
                    foreach (var _item in item.BatchDetialViewModelPurchases)
                    {
                        if (data.LineID != _item.LineID)
                        {
                            if (data.Batch == _item.Batch)
                            {
                                ModelState.AddModelError(
                                    $"BatchNumber-{i}",
                                    $"Item Name \"{item.ItemName}\", Batch at line {i + 1} is already existed");
                                return Ok(msg.Bind(ModelState));
                            }
                            if (data.BatchAttribute1 == _item.BatchAttribute1 && !String.IsNullOrEmpty(data.BatchAttribute1))
                            {
                                ModelState.AddModelError(
                                    $"BatchAttribute1-{i}",
                                    $"Item Name \"{item.ItemName}\", Batch Attribute 1 at line {i + 1} is already existed");
                                return Ok(msg.Bind(ModelState));
                            }
                            if (data.BatchAttribute2 == _item.BatchAttribute2 && !String.IsNullOrEmpty(data.BatchAttribute2))
                            {
                                ModelState.AddModelError(
                                    $"BatchAttribute2-{i}",
                                    $"Item Name \"{item.ItemName}\", Batch Attribute 2 at line {i + 1} is already existed");
                                return Ok(msg.Bind(ModelState));
                            }
                        }
                    }
                    if (whsd.Any(i => i.BatchNo == data.Batch))
                    {
                        ModelState.AddModelError(
                                    $"BatchNumber-{i}",
                                    $"Item Name \"{item.ItemName}\", Batch at line {i + 1} is already existed");
                        return Ok(msg.Bind(ModelState));
                    }
                    if (whsd.Any(i => i.BatchAttr1 == data.BatchAttribute1) && !String.IsNullOrEmpty(data.BatchAttribute1))
                    {
                        ModelState.AddModelError(
                                    $"BatchAttr1-{i}",
                                    $"Item Name \"{item.ItemName}\", Batch Attribute 1 at line {i + 1} is already existed");
                        return Ok(msg.Bind(ModelState));
                    }
                    if (whsd.Any(i => i.BatchAttr2 == data.BatchAttribute2) && !String.IsNullOrEmpty(data.BatchAttribute2))
                    {
                        ModelState.AddModelError(
                                    $"BatchAttr2-{i}",
                                    $"Item Name \"{item.ItemName}\", Batch Attribute 2 at line {i + 1} is already existed");
                        return Ok(msg.Bind(ModelState));
                    }
                }
                if (item.TotalNeeded > item.BatchDetialViewModelPurchases.Sum(i => i.Qty))
                {
                    ModelState.AddModelError("qty", _culLocal["Total Qty created Batch is less than Total needed"]);
                    return Ok(msg.Bind(ModelState));
                }
                else if (item.TotalNeeded < item.BatchDetialViewModelPurchases.Sum(i => i.Qty))
                {
                    ModelState.AddModelError("qty", _culLocal["Total Qty created Batch is greater than Total needed"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            msg.Approve();
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public async Task<IActionResult> GetPurOrderDetailCopy(int seriesId, string number)
        {
            var list = await _ipur.CopyPurchaseOrderAsync(seriesId, number, GetCompany().ID);

            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetApReserveDetailCopy(int seriesId, string number)
        {
            var list = await _ipur.CopyPurchaseAPReserveAsync(seriesId, number, GetCompany().ID);

            return Ok(list);
        }
        [HttpGet]
        public IActionResult Getbp(int id)
        {
            var data = _ipur.Getbp(id);
            return Ok(data);
        }

        public IActionResult GoodReceptPOHistory()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Goods Receipt PO";
            ViewBag.Subpage = "History";
            ViewBag.Menu = "show";
            ViewBag.PurchasePO = "highlight";
            return View(new { Url = _formatNumber.PrintTemplateUrl() });
        }

        [HttpGet]
        public IActionResult GetPurchaseGoodPoReport(int VendorID, int BranchID, int WarehouseID, string PostingDate, string DocumentDate, bool check)
        {
            List<GoodsReciptPO> ServiceCalls = new();
            if (WarehouseID != 0 && VendorID == 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.GoodsReciptPOs.Where(w => w.WarehouseID == WarehouseID).ToList();
            }
            //filter Vendor
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.GoodsReciptPOs.Where(w => w.VendorID == VendorID).ToList();
            }
            //filter WareHouse and VendorName
            else if (WarehouseID != 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.GoodsReciptPOs.Where(w => w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            //filter all item
            else if (BranchID != 0 && WarehouseID == 0 && VendorID == 0 && PostingDate == null && DocumentDate == null)
            {
                ServiceCalls = _context.GoodsReciptPOs.Where(w => w.UserID == BranchID).ToList();
            }
            //filter warehouse, vendor, datefrom ,dateto
            else if (WarehouseID != 0 & VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.GoodsReciptPOs.Where(w => w.VendorID == VendorID && w.WarehouseID == WarehouseID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter vendor and Datefrom and Dateto
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.GoodsReciptPOs.Where(w => w.VendorID == VendorID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter warehouse and Datefrom and DateTo
            else if (WarehouseID != 0 && VendorID == 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter Datefrom and DateTo
            else if (WarehouseID == 0 && VendorID == 0 && PostingDate != null && DocumentDate != null)
            {
                ServiceCalls = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.PostingDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            else
            {
                return Ok(new List<GoodsReciptPO>());
            }
            var list = (from s in ServiceCalls
                        join cus in _context.BusinessPartners on s.VendorID equals cus.ID
                        join item in _context.UserAccounts on s.UserID equals item.ID
                        select new ReportPurchaseAP
                        {
                            ID = s.ID,
                            InvoiceNo = s.InvoiceNo,
                            BusinessName = cus.Name,
                            Warehouse = Convert.ToString(s.WarehouseID),
                            UserName = item.Username,
                            Balance_due = s.BalanceDue,
                            Balance_due_sys = s.BalanceDueSys,
                            Status = s.Status,
                            ExchangeRate = s.SysCurrencyID,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),
                        }).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPurchaseGoodPOByWarehouse(int BarbchID, int WarehouseID, string PostingDate, string DocumentDate, string DuDate, string Check)
        {
            var list = (from P in _recepitPo.GetReportGoodReceiptPO(BarbchID, WarehouseID, PostingDate, DocumentDate, DuDate, Check)
                        select new ReportPurchaseAP
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            UserName = P.UserName,
                            BusinessName = P.BusinessName,
                            Balance_due = P.Balance_due,
                            Balance_due_sys = P.Balance_due_sys,
                            Status = P.Status,
                            ExchangeRate = P.ExchangeRate,
                            Warehouse = P.Warehouse,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),

                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseGoodReceiptByPostingDate(int BarbchID, int WarehouseID, string PostingDate, string DocumentDate, string DuDate, string Check)
        {
            var list = (from P in _recepitPo.GetReportGoodReceiptPO(BarbchID, WarehouseID, PostingDate, DocumentDate, DuDate, Check)
                        select new ReportPurchaseAP
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            UserName = P.UserName,
                            BusinessName = P.BusinessName,
                            Balance_due = P.Balance_due,
                            Balance_due_sys = P.Balance_due_sys,
                            Status = P.Status,
                            ExchangeRate = P.ExchangeRate,
                            Warehouse = P.Warehouse,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),

                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseGoodP0ByDocumentDate(int BarbchID, int WarehouseID, string PostingDate, string DocumentDate, string DuDate, string Check)
        {
            var list = (from P in _recepitPo.GetReportGoodReceiptPO(BarbchID, WarehouseID, PostingDate, DocumentDate, DuDate, Check)
                        select new ReportPurchaseAP
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            UserName = P.UserName,
                            BusinessName = P.BusinessName,
                            Balance_due = P.Balance_due,
                            Balance_due_sys = P.Balance_due_sys,
                            Status = P.Status,
                            ExchangeRate = P.ExchangeRate,
                            Warehouse = P.Warehouse,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseGoodPOByDeliveryDatedDate(int BarbchID, int WarehouseID, string PostingDate, string DocumentDate, string DuDate, string Check)
        {
            var list = (from P in _recepitPo.GetReportGoodReceiptPO(BarbchID, WarehouseID, PostingDate, DocumentDate, DuDate, Check)
                        select new ReportPurchaseAP
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            UserName = P.UserName,
                            BusinessName = P.BusinessName,
                            Balance_due = P.Balance_due,
                            Balance_due_sys = P.Balance_due_sys,
                            Status = P.Status,
                            ExchangeRate = P.ExchangeRate,
                            Warehouse = P.Warehouse,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseGoodPOAllItem(int BarbchID, int WarehouseID, string PostingDate, string DocumentDate, string DuDate, string Check)
        {
            var list = (from P in _recepitPo.GetReportGoodReceiptPO(BarbchID, WarehouseID, PostingDate, DocumentDate, DuDate, Check)
                        select new ReportPurchaseAP
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            UserName = P.UserName,
                            BusinessName = P.BusinessName,
                            Balance_due = P.Balance_due,
                            Balance_due_sys = P.Balance_due_sys,
                            Status = P.Status,
                            ExchangeRate = P.ExchangeRate,
                            Warehouse = P.Warehouse,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),
                        }).ToList();
            return Ok(list);
        }
    }

}
