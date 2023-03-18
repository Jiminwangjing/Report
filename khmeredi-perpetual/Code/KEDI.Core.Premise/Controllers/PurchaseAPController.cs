using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CKBS.AppContext;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.Services.Responsitory;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Models.Services.Purchase;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Repository;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Administrator.Inventory;
using KEDI.Core.Localization.Resources;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.Purchase;
using CKBS.Models.ReportClass;
using Microsoft.AspNetCore.Mvc.Rendering;
using static KEDI.Core.Premise.Controllers.PurchaseRequestController;
using KEDI.Core.Helpers.Enumerations;
using KEDI.Core.System.Models;
using KEDI.Core.Premise.DependencyInjection;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CKBS.Controllers
{
    //[Authorize(Policy = "Permission")]
    [Privilege]
    public class PurchaseAPController : Controller
    {
        private readonly DataContext _context;
        private readonly IPurchaseAP _purchaseAP;
        private readonly IGUOM _gUOM;
        private readonly IPurchaseRepository _ipur;
        private readonly UtilityModule _formatNumber;
        private readonly CultureLocalizer _culLocal;
        public PurchaseAPController(DataContext context, IPurchaseAP purchaseAP, IGUOM gUOM, IPurchaseRepository ipur, CultureLocalizer cultureLocalizer, UtilityModule formatNumber)
        {
            _context = context;
            _purchaseAP = purchaseAP;
            _gUOM = gUOM;
            _culLocal = cultureLocalizer;
            _ipur = ipur;
            _formatNumber = formatNumber;
        }
        [Privilege("A024")]
        public IActionResult PurchaseAP()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase A/P";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseAP = "highlight";
            return View(new { seriesPU = _formatNumber.GetSeries("PU"), seriesJE = _formatNumber.GetSeries("JE"), genSetting = _formatNumber.GetGeneralSettingAdmin() });
        }
        [HttpGet]
        public IActionResult GetPurchaseAPByInvoiceNo(string invoiceNo, int seriesID)
        {
            var number = invoiceNo.Contains("-") ? invoiceNo.Split("-")[1] : invoiceNo;
            var purchaseAp = _context.Purchase_APs.Include(o => o.PurchaseAPDetails)
                         .FirstOrDefault(m => string.Compare(m.Number, number) == 0 && m.SeriesID == seriesID);
            return Ok(purchaseAp);
        }
        [HttpGet]
        public async Task<IActionResult> GetBusinessPartners()
        {
            var list = await _ipur.GetBusinessPartnersAsync();
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
        public async Task<IActionResult> GetCurrencyDefualt()
        {
            var list = await _ipur.GetCurrencyDefualtAsync();
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
        [HttpPost]
        public IActionResult SaveDraft(string draft)
        {
            ModelMessage msg = new();
            DraftAP draft1 = JsonConvert.DeserializeObject<DraftAP>(draft, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var Dcheck = _context.DraftAPs.AsNoTracking();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            draft1.SysCurrencyID = GetCompany().SystemCurrencyID;
           
            draft1.UserID = GetUserID();
            draft1.Status = "open";
            if (draft1.DraftName == "")
            {
                ModelState.AddModelError("DraftName", "Input Draft Name!");
                return Ok(msg.Bind(ModelState));
            }
            if (draft1.BranchID==0)
            {
                ModelState.AddModelError("BranchID", "Input Draft Branch!");
                return Ok(msg.Bind(ModelState));
            }
            if (draft1.DraftID == 0)
            {
                if (Dcheck.Any(i => i.DraftName == draft1.DraftName && i.DocumentTypeID == draft1.DocumentTypeID && !i.Remove))
                {
                    ModelState.AddModelError("DraftName", "Draft Name Already Have!");
                    return Ok(msg.Bind(ModelState));
                }
            }
            else
            {
                var updateDraft = Dcheck.FirstOrDefault(i => i.DraftName == draft1.DraftName);
                if (updateDraft == null)
                {
                    if (Dcheck.Any(i => i.DraftName == draft1.DraftName))
                    {
                        ModelState.AddModelError("DraftName", "Draft Name Already Have!");
                        return Ok(msg.Bind(ModelState));
                    }
                }
                if (Dcheck.Any(i => i.DraftID != draft1.DraftID && i.DraftName == draft1.DraftName && i.DocumentTypeID == draft1.DocumentTypeID && !i.Remove))
                {
                    ModelState.AddModelError("DraftName", "Draft Name Already Have!");
                    return Ok(msg.Bind(ModelState));
                }
            }
            if (ModelState.IsValid)
            {
                draft1.LocalCurID = GetCompany().LocalCurrencyID;
                draft1.LocalSetRate = localSetRate;
                draft1.CompanyID = GetCompany().ID;
                _context.DraftAPs.Update(draft1);
                _context.SaveChanges();
                var freight = draft1.FreightPurchaseView;
                if (freight != null)
                {
                    //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                    freight.PurID = draft1.DraftID;
                    freight.PurType = PurCopyType.PurchaseDraft;
                    freight.OpenExpenceAmount = freight.ExpenceAmount;
                    _context.FreightPurchases.Update(freight);
                    _context.SaveChanges();
                }
                ModelState.AddModelError("success", "SaveDraft succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        public IActionResult DisplayDraftAP()
        {
            var DOC = _context.DocumentTypes.FirstOrDefault(s => s.Code == "PU");
            var DF = _context.DraftAPs.Where(s => s.DocumentTypeID == DOC.ID && !s.Remove).ToList();
            var data = (from df in DF
                        join cur in _context.Currency on df.PurCurrencyID equals cur.ID
                        select new DraftDataViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            BalanceDue = df.BalanceDue.ToString(),
                            CurrencyName = cur.Description,
                            DocType = DOC.Code,
                            DraftName = df.DraftName,
                            DraftID = df.DraftID,
                            PostingDate = df.PostingDate.ToString("dd-MM-yyyy"),
                            Remarks = df.Remark,
                            SubTotal = df.SubTotal.ToString(),
                            SeriesDetailID = df.SeriesDetailID,
                        }).ToList();
            return Ok(data);
        }
        public IActionResult DisplayDraftPO()
        {
            var DOC = _context.DocumentTypes.FirstOrDefault(s => s.Code == "PD");
            var DF = _context.DraftAPs.Where(s => s.DocumentTypeID == DOC.ID && !s.Remove).ToList();
            var data = (from df in DF
                        join cur in _context.Currency on df.PurCurrencyID equals cur.ID
                        select new DraftDataViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            BalanceDue = df.BalanceDue.ToString(),
                            CurrencyName = cur.Description,
                            DocType = DOC.Code,
                            DraftName = df.DraftName,
                            DraftID = df.DraftID,
                            PostingDate = df.PostingDate.ToString("dd-MM-yyyy"),
                            Remarks = df.Remark,
                            SubTotal = df.SubTotal.ToString(),
                            SeriesDetailID = df.SeriesDetailID,
                        }).ToList();
            return Ok(data);
        }
        public IActionResult DisplayDraftMemo()
        {
            var DOC = _context.DocumentTypes.FirstOrDefault(s => s.Code == "PC");
            var DF = _context.DraftAPs.Where(s => s.DocumentTypeID == DOC.ID && !s.Remove).ToList();
            var data = (from df in DF
                        join cur in _context.Currency on df.PurCurrencyID equals cur.ID
                        select new DraftDataViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            BalanceDue = df.BalanceDue.ToString(),
                            CurrencyName = cur.Description,
                            DocType = DOC.Code,
                            DraftName = df.DraftName,
                            DraftID = df.DraftID,
                            PostingDate = df.PostingDate.ToString("dd-MM-yyyy"),
                            Remarks = df.Remark,
                            SubTotal = df.SubTotal.ToString(),
                            SeriesDetailID = df.SeriesDetailID,
                        }).ToList();
            return Ok(data);
        }
        public async Task<IActionResult> FindDraft(string draftname, int draftId)
        {
            var data = await _ipur.FindDraftAsync(draftname, draftId, GetCompany().ID);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveDraft(int id)
        {
            await _ipur.RemoveDraft(id);
            return Ok();

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
                            if (d.LineID == cd.LineID)
                            {
                                if (cd.OpenQty > 0)
                                {
                                    cd.OpenQty -= d.Qty;
                                }
                            }
                            break;
                        case PurCopyType.PurQuote:
                            if (d.LineID == cd.LineID)
                            {
                                if (cd.OpenQty > 0)
                                {
                                    cd.OpenQty -= d.Qty;
                                }
                            }
                            break;
                        case PurCopyType.GRPO:
                            if (d.LineID == cd.LineID)
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
        //=================old function =====================
        // private void UpdateSourceCopy(IEnumerable<dynamic> details, dynamic copyMaster, IEnumerable<dynamic> copyDedails, PurCopyType copyType)
        // {
        //     foreach (var cd in copyDedails)
        //     {
        //         foreach (var d in details)
        //         {
        //             switch (copyType)
        //             {
        //                 case PurCopyType.PurOrder:
        //                     if (d.LineID == cd.PurchaseOrderDetailID)
        //                     {
        //                         if (cd.OpenQty > 0)
        //                         {
        //                             cd.OpenQty -= d.Qty;
        //                         }
        //                     }
        //                     break;
        //                 case PurCopyType.PurQuote:
        //                     if (d.LineID == cd.ID)
        //                     {
        //                         if (cd.OpenQty > 0)
        //                         {
        //                             cd.OpenQty -= d.Qty;
        //                         }
        //                     }
        //                     break;
        //                 case PurCopyType.GRPO:
        //                     if (d.LineID == cd.ID)
        //                     {
        //                         if (cd.OpenQty > 0)
        //                         {
        //                             cd.OpenQty -= d.Qty;
        //                         }
        //                     }
        //                     break;

        //             }

        //         }
        //     }
        //     double sum = copyDedails.ToList().Sum(x => (double)x.OpenQty);
        //     if (sum <= 0)
        //     {
        //         copyMaster.Status = "close";
        //     }
        //     _context.SaveChanges();
        // }
        private void ValidateSummary(Purchase_AP master)
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
            if (master.PurchaseAPDetails.Count <= 0)
            {
                ModelState.AddModelError("Details", "Master has no details");
            }
            else
            {
                for (var dt = 0; dt < master.PurchaseAPDetails.Count; dt++)
                {
                    master.PurchaseAPDetails[dt].TotalSys = master.PurchaseAPDetails[dt].Total * master.PurRate;

                    if (master.PurchaseAPDetails[dt].Qty <= 0)
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
            if(master.BranchID==0){
                ModelState.AddModelError("BranchID","Please Select Branch");
            }
        }
        [HttpPost]
        public IActionResult SavePurchaseAP(string purchase, string Type, string je, string serials = null, string batches = null)
        {
            ModelMessage msg = new();
            if (je == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(msg.Bind(ModelState));
            }
            Purchase_AP purchase_AP = JsonConvert.DeserializeObject<Purchase_AP>(purchase, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Series series_JE = JsonConvert.DeserializeObject<Series>(je, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            SeriesDetail seriesDetail = new();
            var seriesPU = _context.Series.FirstOrDefault(w => w.ID == purchase_AP.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == purchase_AP.DocumentTypeID).FirstOrDefault();
            //var seriesJE = _context.Series.FirstOrDefault(w => w.ID == series_JE.ID);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;

            purchase_AP.SysCurrencyID = GetCompany().SystemCurrencyID;
           
            purchase_AP.UserID = GetUserID();
            // Checking Serial Batch //
            List<SerialViewModelPurchase> serialViewModelPurchases = new();
            List<SerialViewModelPurchase> _serialViewModelPurchases = new();
            List<BatchViewModelPurchase> batchViewModelPurchases = new();
            List<BatchViewModelPurchase> _batchViewModelPurchases = new();
            ValidateSummary(purchase_AP);

            if (ModelState.IsValid)
            {

                foreach (var i in purchase_AP.PurchaseAPDetails.ToList())
                {

                    if (i.PurCopyType == PurCopyType.PurOrder)
                    {
                        i.PurCopyType = PurCopyType.PurOrder;
                    }
                    else if (i.PurCopyType == PurCopyType.GRPO)
                    {

                        i.PurCopyType = PurCopyType.GRPO;
                    }
                    else
                    {

                        i.PurCopyType = PurCopyType.None;
                    }
                }



                if (Type != "GRPO")
                {
                    serialViewModelPurchases = serials != "[]" ? JsonConvert.DeserializeObject<List<SerialViewModelPurchase>>(serials, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }) : serialViewModelPurchases;

                    _ipur.CheckItemSerail(purchase_AP, purchase_AP.PurchaseAPDetails, serialViewModelPurchases);
                    serialViewModelPurchases = serialViewModelPurchases.ToList();
                    // .GroupBy(i => i.ItemID)
                    // .Select(i => i.DefaultIfEmpty().Last()).ToList();

                    foreach (var j in serialViewModelPurchases.ToList())
                    {

                        _serialViewModelPurchases.Add(j);
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
                    _ipur.CheckItemBatch(purchase_AP, purchase_AP.PurchaseAPDetails, batchViewModelPurchases);
                    batchViewModelPurchases = batchViewModelPurchases.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in batchViewModelPurchases.ToList())
                    {
                        foreach (var i in purchase_AP.PurchaseAPDetails.ToList())
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
                }
                if (Type == "Add")
                {
                    purchase_AP.Status = "open";
                    using var t = _context.Database.BeginTransaction();
                    if (ModelState.IsValid)
                    {
                        // insert Series Detail
                        seriesDetail.Number = seriesPU.NextNo;
                        seriesDetail.SeriesID = purchase_AP.SeriesID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();

                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesPU.NextNo;
                        long No = long.Parse(Sno);
                        seriesPU.NextNo = Convert.ToString(No + 1);
                        Random rd = new Random();
                        int rand_num = 0;
                        foreach (var i in purchase_AP.PurchaseAPDetails)
                        {
                            i.LineID = i.LineID == 0 ? rand_num = rd.Next(100000000, 999999999) : i.LineID;

                        }
                        if (No > long.Parse(seriesPU.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }

                        if (purchase_AP.SubTotal <= (purchase_AP.AppliedAmount + purchase_AP.DiscountValue))
                        {
                            purchase_AP.Status = "close";
                        }
                        if (purchase_AP.SubTotal == 0)
                        {
                            purchase_AP.Status = "open";
                        }
                        purchase_AP.Number = seriesDetail.Number;
                        purchase_AP.InvoiceNo = seriesDetail.Number;
                        purchase_AP.LocalCurID = GetCompany().LocalCurrencyID;
                        purchase_AP.LocalSetRate = localSetRate;
                        purchase_AP.SeriesDetailID = seriesDetailID;
                        purchase_AP.CompanyID = GetCompany().ID;


                        _context.Series.Update(seriesPU);
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.Purchase_APs.Update(purchase_AP);
                        _context.SaveChanges();
                        foreach (var check in purchase_AP.PurchaseAPDetails.ToList())
                        {
                            if (check.Qty <= 0)
                            {
                                _context.Remove(check);
                                _context.SaveChanges();
                            }
                        }
                        var freight = UpdateFreight(purchase_AP);

                        if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                        {
                            _purchaseAP.GoodReceiptStockBasic(
                            purchase_AP.PurchaseAPID,
                            "AP",
                            _serialViewModelPurchases,
                            _batchViewModelPurchases,
                            freight
                            );
                        }

                        else
                        {
                            _purchaseAP.GoodReceiptStock(
                       purchase_AP.PurchaseAPID,
                       "AP",
                       _serialViewModelPurchases,
                       _batchViewModelPurchases,
                       freight
                       );
                        }


                        // maximun invoice JE
                        var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                        var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                        if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }

                        t.Commit();
                        ModelState.AddModelError("success", "Item save successfully.");
                        msg.Approve();
                    }
                }
                else if (Type == "PO")
                {
                    List<Purchase_APDetail> Comfirn = new();
                    List<Purchase_APDetail> List = new();
                    foreach (var items in purchase_AP.PurchaseAPDetails.ToList())
                    {
                        if (items.Qty > 0)
                        {
                            Comfirn.Add(items);
                        }
                    }
                    if (List.Count > 0)
                    {
                        return Ok(List);
                    }
                    else
                    {
                        if (purchase_AP.PurchaseAPID == 0)
                        {
                            using var t = _context.Database.BeginTransaction();
                            seriesDetail.Number = seriesPU.NextNo;
                            seriesDetail.SeriesID = purchase_AP.SeriesID;
                            _context.SeriesDetails.Update(seriesDetail);
                            _context.SaveChanges();
                            var seriesDetailID = seriesDetail.ID;
                            string Sno = seriesPU.NextNo;
                            long No = long.Parse(Sno);
                            seriesPU.NextNo = Convert.ToString(No + 1);
                            Random rd = new Random();
                            int rand_num = 0;
                            foreach (var i in purchase_AP.PurchaseAPDetails)
                            {
                                i.LineID = i.LineID == 0 ? rand_num = rd.Next(100000000, 999999999) : i.LineID;

                            }
                            if (No > long.Parse(seriesPU.LastNo))
                            {
                                ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                                return Ok(msg.Bind(ModelState));
                            }
                            purchase_AP.LocalCurID = GetCompany().LocalCurrencyID;
                            purchase_AP.InvoiceNo = seriesDetail.Number;
                            purchase_AP.Number = seriesDetail.Number;
                            purchase_AP.LocalSetRate = localSetRate;
                            purchase_AP.SeriesDetailID = seriesDetailID;
                            purchase_AP.CompanyID = GetCompany().ID;

                            _context.Series.Update(seriesPU);
                            _context.Purchase_APs.Update(purchase_AP);
                            _context.SaveChanges();
                            var freight = UpdateFreight(purchase_AP);
                            _purchaseAP.GoodReceiptStock(purchase_AP.PurchaseAPID, "PO", _serialViewModelPurchases, _batchViewModelPurchases, freight
                                );
                            var purchaseOrder = _context.PurchaseOrders.FirstOrDefault(x => x.PurchaseOrderID == purchase_AP.BaseOnID) ?? new PurchaseOrder();
                            // var purchaseOrder = _context.PurchaseOrders.FirstOrDefault(x => x.PurchaseOrderID == purchase_AP.BaseOnID);
                            var subAppliedAmount = _context.PurchaseOrders.Where(x => x.PurchaseOrderID == purchase_AP.BaseOnID);
                            int OrderID = purchaseOrder.PurchaseOrderID;
                            var detail = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == OrderID && x.Delete == false);
                            if (purchaseOrder.PurchaseOrderID != 0)
                            {
                                UpdateSourceCopy(purchase_AP.PurchaseAPDetails, purchaseOrder, detail, PurCopyType.PurOrder);
                            }


                            // maximun invoice JE
                            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE") ?? new DocumentType();
                            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID) ?? new Series();

                            if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                            {
                                ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                                return Ok(msg.Bind(ModelState));
                            }
                            t.Commit();
                            ModelState.AddModelError("success", "Item save successfully.");
                            msg.Approve();
                        }
                    }

                }
                else if (Type == "GRPO")
                {
                    List<Purchase_APDetail> List = new();
                    if (purchase_AP.PurchaseAPID == 0)
                    {
                        using var t = _context.Database.BeginTransaction();
                        if (ModelState.IsValid)
                        {
                            foreach (var item in purchase_AP.PurchaseAPDetails.ToList())
                            {
                                if (item.OpenQty <= 0)
                                {
                                    var ID = item.LineID;
                                    purchase_AP.PurchaseAPDetails.Remove(item);
                                }
                            }
                            seriesDetail.Number = seriesPU.NextNo;
                            seriesDetail.SeriesID = purchase_AP.SeriesID;
                            _context.SeriesDetails.Update(seriesDetail);
                            _context.SaveChanges();
                            var seriesDetailID = seriesDetail.ID;
                            string Sno = seriesPU.NextNo;
                            long No = long.Parse(Sno);
                            seriesPU.NextNo = Convert.ToString(No + 1);
                            Random rd = new Random();
                            int rand_num = 0;
                            foreach (var i in purchase_AP.PurchaseAPDetails)
                            {
                                i.LineID = i.LineID == 0 ? rand_num = rd.Next(100000000, 999999999) : i.LineID;

                            }
                            if (No > long.Parse(seriesPU.LastNo))
                            {
                                ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                                return Ok(msg.Bind(ModelState));
                            }
                            purchase_AP.LocalCurID = GetCompany().LocalCurrencyID;
                            purchase_AP.InvoiceNo = seriesDetail.Number;
                            purchase_AP.Number = seriesDetail.Number;
                            purchase_AP.LocalSetRate = localSetRate;
                            purchase_AP.SeriesDetailID = seriesDetailID;
                            purchase_AP.CompanyID = GetCompany().ID;
                            _context.Series.Update(seriesPU);
                            _context.Purchase_APs.Update(purchase_AP);
                            _context.SaveChanges();

                            var goodsPo = _context.GoodsReciptPOs.AsNoTracking().FirstOrDefault(i => i.ID == purchase_AP.BaseOnID);
                            if (goodsPo != null)
                            {
                                var goodPod = _context.GoodReciptPODetails.Where(s => s.GoodsReciptPOID == goodsPo.ID).ToList();
                                UpdateSourceCopy(purchase_AP.PurchaseAPDetails, goodsPo, goodPod, PurCopyType.GRPO);

                                double appliedAmount = (goodsPo.AppliedAmount + purchase_AP.BalanceDue + purchase_AP.DiscountValue);
                                if (goodsPo.BalanceDue <= appliedAmount)
                                {
                                    goodsPo.Status = "close";
                                    goodsPo.AppliedAmount = goodsPo.BalanceDue;
                                    goodsPo.AppliedAmountSys = goodsPo.BalanceDue * goodsPo.PurRate;
                                }
                                else
                                {
                                    goodsPo.Status = "open";
                                    goodsPo.AppliedAmount = appliedAmount;
                                    goodsPo.AppliedAmountSys = appliedAmount * goodsPo.PurRate;
                                }
                                _context.GoodsReciptPOs.Update(goodsPo);
                                _context.SaveChanges();
                            }
                            // maximun invoice JE
                            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE") ?? new DocumentType();
                            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID) ?? new Series();

                            if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                            {
                                ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                                return Ok(msg.Bind(ModelState));
                            }
                            var freight = UpdateFreight(purchase_AP);
                            _purchaseAP.CopyGRPOtoAP(purchase_AP.PurchaseAPID, freight);
                            t.Commit();
                            ModelState.AddModelError("success", "Item save successfully.");
                            msg.Approve();
                        }
                    }
                }
            }
            return Ok(msg.Bind(ModelState));
        }
        //  start block save draft Purchase reserve
        [HttpPost]
        public IActionResult SaveDraftPurchaseReserve(string draft)
        {
            ModelMessage msg = new();
            DraftReserve draft1 = JsonConvert.DeserializeObject<DraftReserve>(draft, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var Dcheck = _context.DraftReserves.AsNoTracking();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            draft1.SysCurrencyID = GetCompany().SystemCurrencyID;
           
            draft1.UserID = GetUserID();
            draft1.Status = "open";
            if (string.IsNullOrWhiteSpace(draft1.Name))
            {
                ModelState.AddModelError("DraftName", "Input Draft Name!");
                return Ok(msg.Bind(ModelState));
            }
             if (draft1.BranchID==0)
            {
                ModelState.AddModelError("BranchID", "Input Draft Branch!");
                return Ok(msg.Bind(ModelState));
            }
            if (draft1.ID == 0)
            {
                if (Dcheck.Any(i => i.Name == draft1.Name && i.DocumentTypeID == draft1.DocumentTypeID && !i.Delete))
                {
                    ModelState.AddModelError("DraftName", "Draft Name Already Have!");
                    return Ok(msg.Bind(ModelState));
                }
            }
            else
            {
                var updateDraft = Dcheck.FirstOrDefault(i => i.Name == draft1.Name);
                if (updateDraft == null)
                {
                    if (Dcheck.Any(i => i.Name == draft1.Name))
                    {
                        ModelState.AddModelError("DraftName", "Draft Name Already Have!");
                        return Ok(msg.Bind(ModelState));
                    }
                }
                if (Dcheck.Any(i => i.ID != draft1.ID && i.Name == draft1.Name && i.DocumentTypeID == draft1.DocumentTypeID && !i.Delete))
                {
                    ModelState.AddModelError("DraftName", "Draft Name Already Have!");
                    return Ok(msg.Bind(ModelState));
                }
            }
            if (ModelState.IsValid)
            {
                draft1.LocalCurID = GetCompany().LocalCurrencyID;
                draft1.LocalSetRate = localSetRate;
                draft1.CompanyID = GetCompany().ID;
                _context.DraftReserves.Update(draft1);
                _context.SaveChanges();
                var freight = draft1.FreightPurchaseView;
                if (freight != null)
                {
                    //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                    freight.PurID = draft1.ID;
                    freight.PurType = PurCopyType.PurchaseDraft;
                    freight.OpenExpenceAmount = freight.ExpenceAmount;
                    _context.FreightPurchases.Update(freight);
                    _context.SaveChanges();
                }
                ModelState.AddModelError("success", "SaveDraft succussfully!");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        public IActionResult DisplayDraftReserve()
        {
            var DOC = _context.DocumentTypes.FirstOrDefault(s => s.Code == "PU");
            var DF = _context.DraftAPs.Where(s => s.DocumentTypeID == DOC.ID && !s.Remove).ToList();
            var data = (from df in DF
                        join cur in _context.Currency on df.PurCurrencyID equals cur.ID
                        select new DraftDataViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            BalanceDue = df.BalanceDue.ToString(),
                            CurrencyName = cur.Description,
                            DocType = DOC.Code,
                            DraftName = df.DraftName,
                            DraftID = df.DraftID,
                            PostingDate = df.PostingDate.ToString("dd-MM-yyyy"),
                            Remarks = df.Remark,
                            SubTotal = df.SubTotal.ToString(),
                            SeriesDetailID = df.SeriesDetailID,
                        }).ToList();
            return Ok(data);
        }

        [HttpPost]
        public IActionResult SavePurchaseAPReserve(string purchase, string type, string je)
        {
            ModelMessage msg = new();
            if (je == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(msg.Bind(ModelState));
            }
            PurchaseAPReserve purchase_AP = JsonConvert.DeserializeObject<PurchaseAPReserve>(purchase, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Series series_JE = JsonConvert.DeserializeObject<Series>(je, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            SeriesDetail seriesDetail = new();
            var seriesPU = _context.Series.FirstOrDefault(w => w.ID == purchase_AP.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == purchase_AP.DocumentTypeID).FirstOrDefault();
            //var seriesJE = _context.Series.FirstOrDefault(w => w.ID == series_JE.ID);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;

            purchase_AP.SysCurrencyID = GetCompany().SystemCurrencyID;
        
            purchase_AP.UserID = GetUserID();
            ValidateSummaryAPReserve(purchase_AP);
            if (type == "Add")
            {
                purchase_AP.Status = "open";
                using var t = _context.Database.BeginTransaction();
                if (ModelState.IsValid)
                {
                    // insert Series Detail
                    seriesDetail.Number = seriesPU.NextNo;
                    seriesDetail.SeriesID = purchase_AP.SeriesID;
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.SaveChanges();

                    var seriesDetailID = seriesDetail.ID;
                    string Sno = seriesPU.NextNo;
                    long No = long.Parse(Sno);
                    seriesPU.NextNo = Convert.ToString(No + 1);
                    if (No > long.Parse(seriesPU.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                        return Ok(msg.Bind(ModelState));
                    }

                    if (purchase_AP.SubTotal <= (purchase_AP.AppliedAmount + purchase_AP.DiscountValue))
                    {
                        purchase_AP.Status = "close";
                    }
                    purchase_AP.Number = seriesDetail.Number;
                    purchase_AP.InvoiceNo = seriesDetail.Number;
                    purchase_AP.LocalCurID = GetCompany().LocalCurrencyID;
                    purchase_AP.LocalSetRate = localSetRate;
                    purchase_AP.SeriesDetailID = seriesDetailID;
                    purchase_AP.CompanyID = GetCompany().ID;


                    _context.Series.Update(seriesPU);
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.PurchaseAPReserves.Update(purchase_AP);
                    _context.SaveChanges();
                    foreach (var check in purchase_AP.PurchaseAPReserveDetails.ToList())
                    {
                        if (check.Qty <= 0)
                        {
                            _context.Remove(check);
                            _context.SaveChanges();
                        }
                    }
                    var freight = UpdateFreightAPReserve(purchase_AP);
                    _purchaseAP.GoodReceiptStockAPReserve(purchase_AP.ID, "AP", freight);

                    // maximun invoice JE
                    var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                    var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                    if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                        return Ok(msg.Bind(ModelState));
                    }

                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }
            else if (type == "PO")
            {
                List<PurchaseAPReserveDetail> Comfirn = new();
                List<PurchaseAPReserveDetail> List = new();
                foreach (var items in purchase_AP.PurchaseAPReserveDetails.ToList())
                {
                    if (items.Qty > 0)
                    {
                        Comfirn.Add(items);
                    }
                }
                if (List.Count > 0)
                {
                    return Ok(List);
                }
                else
                {
                    if (purchase_AP.ID == 0)
                    {
                        using var t = _context.Database.BeginTransaction();
                        if (ModelState.IsValid)
                        {
                            seriesDetail.Number = seriesPU.NextNo;
                            seriesDetail.SeriesID = purchase_AP.SeriesID;
                            _context.SeriesDetails.Update(seriesDetail);
                            _context.SaveChanges();
                            var seriesDetailID = seriesDetail.ID;
                            string Sno = seriesPU.NextNo;
                            long No = long.Parse(Sno);
                            seriesPU.NextNo = Convert.ToString(No + 1);
                            if (No > long.Parse(seriesPU.LastNo))
                            {
                                ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                                return Ok(msg.Bind(ModelState));
                            }
                            purchase_AP.LocalCurID = GetCompany().LocalCurrencyID;
                            purchase_AP.InvoiceNo = seriesDetail.Number;
                            purchase_AP.Number = seriesDetail.Number;
                            purchase_AP.LocalSetRate = localSetRate;
                            purchase_AP.SeriesDetailID = seriesDetailID;
                            purchase_AP.CopyToNote = CopyToNote.PO;
                            purchase_AP.CompanyID = GetCompany().ID;

                            _context.Series.Update(seriesPU);
                            _context.PurchaseAPReserves.Update(purchase_AP);
                            _context.SaveChanges();
                            var freight = UpdateFreightAPReserve(purchase_AP);
                            _purchaseAP.GoodReceiptStockAPReserve(purchase_AP.ID, "PO", freight);
                            var purchaseOrder = _context.PurchaseOrders.FirstOrDefault(x => x.PurchaseOrderID == purchase_AP.BaseOnID) ?? new PurchaseOrder();

                            var subAppliedAmount = _context.PurchaseOrders.Where(x => x.PurchaseOrderID == purchase_AP.BaseOnID);
                            int OrderID = purchaseOrder.PurchaseOrderID;
                            var detail = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == OrderID && x.Delete == false);
                            if (purchaseOrder.PurchaseOrderID != 0)
                            {

                                UpdateSourceCopy(purchase_AP.PurchaseAPReserveDetails, purchaseOrder, detail, PurCopyType.PurOrder);
                            }


                            // maximun invoice JE
                            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                            if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                            {
                                ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                                return Ok(msg.Bind(ModelState));
                            }
                            t.Commit();
                            ModelState.AddModelError("success", "Item save successfully.");
                            msg.Approve();
                        }

                    }
                }

            }
            else if (type == "PQ")
            {
                List<PurchaseAPReserveDetail> List = new();
                if (purchase_AP.ID == 0)
                {
                    using var t = _context.Database.BeginTransaction();
                    if (ModelState.IsValid)
                    {
                        foreach (var item in purchase_AP.PurchaseAPReserveDetails.ToList())
                        {
                            if (item.OpenQty <= 0)
                            {
                                var ID = item.LineID;
                                purchase_AP.PurchaseAPReserveDetails.Remove(item);
                            }
                        }
                        seriesDetail.Number = seriesPU.NextNo;
                        seriesDetail.SeriesID = purchase_AP.SeriesID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesPU.NextNo;
                        long No = long.Parse(Sno);
                        seriesPU.NextNo = Convert.ToString(No + 1);
                        if (No > long.Parse(seriesPU.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }
                        purchase_AP.LocalCurID = GetCompany().LocalCurrencyID;
                        purchase_AP.InvoiceNo = seriesDetail.Number;
                        purchase_AP.Number = seriesDetail.Number;
                        purchase_AP.LocalSetRate = localSetRate;
                        purchase_AP.SeriesDetailID = seriesDetailID;
                        purchase_AP.CopyToNote = CopyToNote.PQ;
                        purchase_AP.CompanyID = GetCompany().ID;
                        _context.Series.Update(seriesPU);
                        _context.PurchaseAPReserves.Update(purchase_AP);
                        _context.SaveChanges();

                        var quote = _context.PurchaseQuotations.FirstOrDefault(x => x.ID == purchase_AP.BaseOnID);
                        var quoted = _context.PurchaseQuotationDetails.Where(x => x.PurchaseQuotationID == quote.ID);
                        if (quote.ID != 0)
                        {

                            UpdateSourceCopy(purchase_AP.PurchaseAPReserveDetails, quote, quoted, PurCopyType.PurQuote);
                        }

                        // maximun invoice JE
                        var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                        var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                        if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }
                        var freight = UpdateFreightAPReserve(purchase_AP);
                        _purchaseAP.CopyGRPOtoAPReserves(purchase_AP.ID, freight);
                        t.Commit();
                        ModelState.AddModelError("success", "Item save successfully.");
                        msg.Approve();
                    }
                }
            }
            return Ok(msg.Bind(ModelState));

        }

        [HttpPost]
        public IActionResult CancelPurchaseAP(string purchaseAp, int purchaseApId, string serials, string batches)
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            var purchaseApData = _context.Purchase_APs.Find(purchaseApId);
            if (purchaseApData == null)
            {
                ModelState.AddModelError("ardata", _culLocal["A/P invoice could not found"]);
                return Ok(msg.Bind(ModelState));
            }
            Purchase_AP purAp = JsonConvert.DeserializeObject<Purchase_AP>(purchaseAp, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var ex = _context.ExchangeRates.FirstOrDefault(i => i.CurrencyID == purAp.PurCurrencyID) ?? new ExchangeRate();
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == purAp.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == purAp.DocumentTypeID).FirstOrDefault();
            if (ModelState.IsValid)
            {
                // Checking Serial Batch //
                List<APCSerialNumber> aPCSerialNumber = new();
                List<APCSerialNumber> _aPCSerialNumber = new();
                List<APCBatchNo> aPCBatchNos = new();
                List<APCBatchNo> _aPCBatchNos = new();

                aPCSerialNumber = serials != "[]" ? JsonConvert.DeserializeObject<List<APCSerialNumber>>(serials, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : aPCSerialNumber;

                _ipur.CheckItemSerail(purAp, purAp.PurchaseAPDetails, aPCSerialNumber, ex);
                aPCSerialNumber = aPCSerialNumber.GroupBy(i => new { i.ItemID, i.Cost }).Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in aPCSerialNumber.ToList())
                {
                    foreach (var i in purAp.PurchaseAPDetails.ToList())
                    {
                        if (j.ItemID == i.ItemID && j.Cost == (decimal)(i.PurchasPrice * ex.Rate))
                        {
                            _aPCSerialNumber.Add(j);
                        }
                    }
                }
                bool isHasSerialItem = _aPCSerialNumber.Any(i => i.OpenQty != 0);
                if (isHasSerialItem)
                {
                    return Ok(new { IsSerail = true, Data = _aPCSerialNumber });
                }
                // checking batch items
                aPCBatchNos = batches != "[]" ? JsonConvert.DeserializeObject<List<APCBatchNo>>(batches, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : aPCBatchNos;
                _ipur.CheckItemBatch(purAp, purAp.PurchaseAPDetails, aPCBatchNos, ex);
                aPCBatchNos = aPCBatchNos.GroupBy(i => new { i.ItemID, i.Cost }).Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in aPCBatchNos.ToList())
                {
                    foreach (var i in purAp.PurchaseAPDetails.ToList())
                    {
                        if (j.ItemID == i.ItemID && j.Cost == (decimal)(i.PurchasPrice * ex.Rate))
                        {
                            _aPCBatchNos.Add(j);
                        }
                    }
                }
                bool isHasBatchItems = _aPCBatchNos.Any(i => i.TotalNeeded != 0);
                if (isHasBatchItems)
                {
                    return Ok(new { IsBatch = true, Data = _aPCBatchNos });
                }
                using var t = _context.Database.BeginTransaction();
                seriesDetail.Number = seriesDN.NextNo;
                seriesDetail.SeriesID = seriesDN.ID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDN.NextNo;
                long No = long.Parse(Sno);
                purAp.InvoiceNo = seriesDN.NextNo;
                purAp.Number = seriesDN.NextNo;
                seriesDN.NextNo = Convert.ToString(No + 1);
                if (No > long.Parse(seriesDN.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your A/P Invoice has reached the limitation!!"]);
                    return Ok(msg.Bind(ModelState));
                }
                var outgoingVendor = _context.OutgoingPaymentVendors.FirstOrDefault(w => w.Number == purchaseApData.Number && w.SeriesDetailID == purchaseApData.SeriesDetailID);
                var _freight = _context.FreightPurchases.FirstOrDefault(i => i.PurID == purchaseApData.PurchaseAPID && i.PurType == PurCopyType.PurAP);
                if (_freight != null)
                {
                    _freight.OpenExpenceAmount = 0;
                    purchaseApData.FrieghtAmount -= purAp.FrieghtAmount;
                    purchaseApData.FrieghtAmountSys = purchaseApData.FrieghtAmount * (decimal)purAp.PurRate;
                    _context.FreightPurchases.Update(_freight);
                }
                purchaseApData.AppliedAmount = purAp.AppliedAmount;
                if (outgoingVendor != null)
                {
                    outgoingVendor.Applied_Amount = outgoingVendor.Total;
                    outgoingVendor.BalanceDue = 0;
                    outgoingVendor.TotalPayment = 0;
                    outgoingVendor.Status = "close";
                    _context.OutgoingPaymentVendors.Update(outgoingVendor);
                }

                _context.SaveChanges();
                var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "PS");
                var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
                var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
                if (seriesIn == null)
                {
                    ModelState.AddModelError("SeriesIn", "Series Outgoing Payment has No Data or Default One!!");
                    return Ok(msg.Bind(ModelState));
                }
                if (paymentMean == null)
                {
                    ModelState.AddModelError("SeriesIn", "Series Payment Means has No Data or Default One!!");
                    return Ok(msg.Bind(ModelState));
                }


                purAp.Status = "close";
                purAp.SeriesID = seriesDN.ID;
                purAp.SeriesDetailID = seriesDetailID;
                purchaseApData.Status = "cancel";
                _context.Purchase_APs.Update(purAp);
                _context.Purchase_APs.Update(purchaseApData);
                _context.SaveChanges();
                _purchaseAP.IssuseCancelPurchaseAP(purAp, _aPCSerialNumber, _aPCBatchNos, purAp.FreightPurchaseView);

                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Invoice Journal Entry has reached the limitation!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                t.Commit();
                ModelState.AddModelError("success", _culLocal["A/P Invoice has been cancelled successfully!"]);
                msg.Approve();
                return Ok(msg.Bind(ModelState));
            }
            return Ok(msg.Bind(ModelState));
        }
        private FreightPurchase UpdateFreight(Purchase_AP purchase_AP)
        {
            var freight = purchase_AP.FreightPurchaseView;
            if (freight != null)
            {
                //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                freight.PurID = purchase_AP.PurchaseAPID;
                freight.PurType = PurCopyType.PurAP;
                freight.OpenExpenceAmount = freight.ExpenceAmount;
                _context.FreightPurchases.Update(freight);
                _context.SaveChanges();
            }
            return freight;
        }
        public bool CheckStatus(IEnumerable<PurchaseOrderDetail> invoices)
        {
            bool result = true;
            foreach (var inv in invoices)
            {
                if (inv.Delete == false)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        [HttpGet]
        public async Task<IActionResult> GetPurchaseorder(int vendorId)
        {
            var data = await _context.PurchaseOrders.Where(i => i.Status == "open" && i.VendorID == vendorId && i.BranchID == GetBranchID() && i.CompanyID == GetCompany().ID).ToListAsync();
            var list = await _ipur.GetAllPurPOAsync(data);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetPurOrderDetailCopy(int seriesId, string number)
        {
            var list = await _ipur.CopyPurchaseOrderAsync(seriesId, number, GetCompany().ID);

            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetGoodsRecieptPO(int vendorId)
        {
            var data = await _context.GoodsReciptPOs.Where(i => i.Status == "open" && i.VendorID == vendorId && i.BranchID == GetBranchID() && i.CompanyID == GetCompany().ID).ToListAsync();
            var list = await _ipur.GetAllPurPOAsync(data);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetGoodsRecieptPODetailCopy(int seriesId, string number)
        {
            var list = await _ipur.CopyPurchasePOAsync(seriesId, number, GetCompany().ID);
            return Ok(list);
        }
        [HttpGet]
        public IActionResult Getbp(int id)
        {
            var data = _ipur.Getbp(id);
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> FindPurchaseAP(string number, int seriesID)
        {
            var data = await _ipur.FindPurchaseAPAsync(seriesID, number, GetCompany().ID);
            if (data.PurchaseAP == null)
            {
                return Ok(new { Error = true, Message = $"Cannot find invoice \"{number}\"" });
            }
            return Ok(data);
        }
        [Privilege("A024")]
        public IActionResult PurchaseAPStory()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase A/P";
            ViewBag.Subpage = "Purchase A/P Story";
            ViewBag.Menu = "show";
            ViewBag.PurchaseAP = "highlight";
            return View(new { Url = _formatNumber.PrintTemplateUrl() });
        }


        public Dictionary<int, string> Typevatt => EnumHelper.ToDictionary(typeof(PVatType));

        [HttpGet]
        public IActionResult GetPurchaseAPReport(int BranchID, int WarehouseID, int VendorID, String PostingDate, string DocumentDate, bool check)
        {
            List<Purchase_AP> ServiceCalls = new();
            //filter WareHouse
            if (WarehouseID != 0 && VendorID == 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.Purchase_APs.Where(w => w.WarehouseID == WarehouseID).ToList();
            }
            //filter Vendor
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.Purchase_APs.Where(w => w.VendorID == VendorID).ToList();
            }
            //filter WareHouse and VendorName
            else if (WarehouseID != 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.Purchase_APs.Where(w => w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            //filter all item
            else if (BranchID != 0 && WarehouseID == 0 && VendorID == 0 && PostingDate == null && DocumentDate == null)
            {
                ServiceCalls = _context.Purchase_APs.Where(w => w.UserID == BranchID).ToList();
            }
            //filter warehouse, vendor, datefrom ,dateto
            else if (WarehouseID != 0 & VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.Purchase_APs.Where(w => w.VendorID == VendorID && w.WarehouseID == WarehouseID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter vendor and Datefrom and Dateto
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.Purchase_APs.Where(w => w.VendorID == VendorID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter warehouse and Datefrom and DateTo
            else if (WarehouseID != 0 && VendorID == 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter Datefrom and DateTo
            else if (WarehouseID == 0 && VendorID == 0 && PostingDate != null && DocumentDate != null)
            {
                ServiceCalls = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.PostingDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            else
            {
                return Ok(new List<Purchase_AP>());
            }
            var list = (from s in ServiceCalls
                        join cus in _context.BusinessPartners on s.VendorID equals cus.ID
                        join item in _context.UserAccounts on s.UserID equals item.ID
                        select new PurchaseReport
                        {
                            ID = s.PurchaseAPID,
                            Invoice = s.InvoiceNo,
                            VendorName = cus.Name,
                            UserName = item.Username,
                            Balance = s.BalanceDue,

                            ExchangeRate = s.PurRate,
                            Cancele = "<i class= 'fa fa-ban'style='color:red;' ></i>",
                            Status = s.Status,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),

                        }).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPurchaseAPByWarehouse(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = (from P in _purchaseAP.GetReportPurchaseAPs(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check)
                        select new ReportPurchaseAP
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            Balance_due = P.Balance_due,
                            BusinessName = P.BusinessName,
                            ExchangeRate = P.ExchangeRate,
                            Status = P.Status,
                            UserName = P.UserName,
                            Warehouse = P.Warehouse,
                            Balance_due_sys = P.Balance_due_sys,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList()
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseAPByPostingDate(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = (from P in _purchaseAP.GetReportPurchaseAPs(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check)
                        select new ReportPurchaseAP
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            Balance_due = P.Balance_due,
                            BusinessName = P.BusinessName,
                            ExchangeRate = P.ExchangeRate,
                            Status = P.Status,
                            UserName = P.UserName,
                            Warehouse = P.Warehouse,
                            Balance_due_sys = P.Balance_due_sys,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList()
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseAPByDocumentDate(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = (from P in _purchaseAP.GetReportPurchaseAPs(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check)
                        select new ReportPurchaseAP
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            Balance_due = P.Balance_due,
                            BusinessName = P.BusinessName,
                            ExchangeRate = P.ExchangeRate,
                            Status = P.Status,
                            UserName = P.UserName,
                            Warehouse = P.Warehouse,
                            Balance_due_sys = P.Balance_due_sys,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList()
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseAPByDeliveryDatedDate(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = (from P in _purchaseAP.GetReportPurchaseAPs(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check)
                        select new ReportPurchaseAP
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            Balance_due = P.Balance_due,
                            BusinessName = P.BusinessName,
                            ExchangeRate = P.ExchangeRate,
                            Status = P.Status,
                            UserName = P.UserName,
                            Warehouse = P.Warehouse,
                            Balance_due_sys = P.Balance_due_sys,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList()
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseAPAllItem(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = (from P in _purchaseAP.GetReportPurchaseAPs(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check)
                        select new ReportPurchaseAP
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            Balance_due = P.Balance_due,
                            BusinessName = P.BusinessName,
                            ExchangeRate = P.ExchangeRate,
                            Status = P.Status,
                            UserName = P.UserName,
                            Warehouse = P.Warehouse,
                            Balance_due_sys = P.Balance_due_sys,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList()
                        }).ToList();
            return Ok(list);
        }


        // start Purchase AP Reserve
        private FreightPurchase UpdateFreightAPReserve(PurchaseAPReserve purchase_AP)
        {
            var freight = purchase_AP.FreightPurchaseView;
            if (freight != null)
            {
                //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                freight.PurID = purchase_AP.ID;
                freight.PurType = PurCopyType.PurReserve;
                freight.OpenExpenceAmount = freight.ExpenceAmount;
                _context.FreightPurchases.Update(freight);
                _context.SaveChanges();
            }
            return freight;
        }
        private void ValidateSummaryAPReserve(PurchaseAPReserve master)
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
            if (master.PurchaseAPReserveDetails.Count <= 0)
            {
                ModelState.AddModelError("Details", "Master has no details");
            }
            else
            {
                for (var dt = 0; dt < master.PurchaseAPReserveDetails.Count; dt++)
                {
                    master.PurchaseAPReserveDetails[dt].TotalSys = master.PurchaseAPReserveDetails[dt].Total * master.PurRate;

                    if (master.PurchaseAPReserveDetails[dt].Qty <= 0)
                    {
                        ModelState.AddModelError($"Qty-{dt}", $"Detial at line {dt + 1}, Qty has to be greater than 0!");
                    }
                }
            }
            if(master.BranchID==0){
                ModelState.AddModelError("BranchID", "Please Select Branch");
            }

        }
        public IActionResult Index()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase A/P";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseAPReserve = "highlight";
            return View(new { seriesPU = _formatNumber.GetSeries("PU"), seriesJE = _formatNumber.GetSeries("JE"), genSetting = _formatNumber.GetGeneralSettingAdmin() });
        }


        [HttpGet]
        public async Task<IActionResult> FindPurchaseAPReseerve(string number, int seriesID)
        {
            var data = await _ipur.FindPurchaseAPReserveAsync(seriesID, number, GetCompany().ID);
            if (data.PurchaseAP == null)
            {
                return Ok(new { Error = true, Message = $"Cannot find invoice \"{number}\"" });
            }
            return Ok(data);
        }
        // story Purchase AP Reserve
        public IActionResult PurchaseAPReserveStory()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase A/P";
            ViewBag.Subpage = "Purchase A/P Story";
            ViewBag.Menu = "show";
            ViewBag.PurchaseAPReserve = "highlight";
            return View(new { Url = _formatNumber.PrintTemplateUrl() });
        }


        [HttpGet]
        public IActionResult GetPurchaseAPReserveReport(int branchID, int warehouseID, int VendorID, string postingDate, string documentDate, bool check)
        {
            var list = _purchaseAP.GetPurchaseAPReserves(branchID, warehouseID, VendorID, postingDate, documentDate, check);
            return Ok(list);
        }

        [HttpPost]
        public IActionResult APReserveCancel(int PurchaseID)
        {
            APReserveCancelOne(PurchaseID);
            return Ok();
        }
        public void APReserveCancelOne(int purchaseID)
        {
            if (purchaseID != 0)
            {
                var purchase = _context.PurchaseAPReserves.Include(w => w.PurchaseAPReserveDetails).FirstOrDefault(w => w.ID == purchaseID) ?? new PurchaseAPReserve();
                if (purchase != null || purchase.PurchaseAPReserveDetails != null)
                {
                    purchase.Status = "close";
                    _context.PurchaseAPReserves.Update(purchase);
                    _context.SaveChanges();
                    foreach (var item in purchase.PurchaseAPReserveDetails.ToList())
                    {
                        var item_master = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID) ?? new ItemMasterData();
                        var gduom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_master.GroupUomID && w.AltUOM == item.UomID) ?? new GroupDUoM();

                        var warehouse_sum = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == purchase.WarehouseID && w.ItemID == item.ItemID) ?? new WarehouseSummary();
                        var itemAcc = _context.ItemAccountings.FirstOrDefault(w => w.WarehouseID == purchase.WarehouseID && w.ItemID == item.ItemID) ?? new Models.Services.Financials.ItemAccounting();
                        if (warehouse_sum != null)
                        {
                            warehouse_sum.Ordered -= (item.OpenQty * gduom.Factor);
                            itemAcc.Ordered = warehouse_sum.Ordered;
                            _context.ItemAccountings.Update(itemAcc);
                            _context.WarehouseSummary.Update(warehouse_sum);
                            _context.SaveChanges();
                        }
                        //Update in item maser
                        item_master.StockOnHand -= (item.OpenQty * gduom.Factor);
                        _context.ItemMasterDatas.Update(item_master);
                        _context.SaveChanges();
                    }
                }
            }
        }

        // copy qoute 
        public IActionResult GetPurchaseQuotationCopy(int id)
        {
            var list = (from pr in _context.PurchaseQuotations.Where(s => s.VendorID == id && s.Status == "open")
                        join dct in _context.DocumentTypes on pr.DocumentTypeID equals dct.ID
                        join currency in _context.Currency on pr.PurCurrencyID equals currency.ID
                        select new
                        {
                            Type = 1,
                            pr.ID,
                            pr.SeriesID,
                            DocType = dct.Code,
                            Invoice = pr.Number,
                            PostingDate = pr.PostingDate.ToString("MM-dd-yyyy"),
                            CurrencyName = currency.Description,
                            pr.SubTotal,
                            pr.BalanceDue,
                            Remarks = pr.Remark

                        }).ToList();
            return Ok(list);
        }
        // copy order
        public IActionResult GetPurchaseOrdercopy(int id)
        {
            var list = (from pr in _context.PurchaseOrders.Where(s => s.VendorID == id && s.Status == "open")
                        join dct in _context.DocumentTypes on pr.DocumentTypeID equals dct.ID
                        join currency in _context.Currency on pr.PurCurrencyID equals currency.ID
                        select new
                        {
                            Type = 2,
                            ID = pr.PurchaseOrderID,
                            pr.PurchaseOrderID,
                            pr.SeriesID,
                            DocType = dct.Code,
                            Invoice = pr.Number,
                            PostingDate = pr.PostingDate.ToString("MM-dd-yyyy"),
                            CurrencyName = currency.Description,
                            pr.SubTotal,
                            pr.BalanceDue,
                            Remarks = pr.Remark

                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> FindPurchaseQuotation(int seriesID, string number)
        {
            var data = await _ipur.CopyPurchaseQuote(seriesID, number, GetCompany().ID);
            if (data.PurchaseAP == null)
            {
                return Ok(new { Error = true, Message = $"Cannot find invoice \"{number}\"" });
            }
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> FindPurchaseOrder(int seriesID, string number)
        {
            var data = await _ipur.CopyPurchaseOrder(seriesID, number, GetCompany().ID);
            if (data.PurchaseAP == null)
            {
                return Ok(new { Error = true, Message = $"Cannot find invoice \"{number}\"" });
            }
            return Ok(data);
        }
    }
}
