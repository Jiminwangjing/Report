using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CKBS.AppContext;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using CKBS.Models.Services.Banking;
using KEDI.Core.Models.Validation;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Localization.Resources;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases;
using KEDI.Core.Premise.Models.Services.Purchase;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Repository;
using CKBS.Models.ReportClass;
using Microsoft.AspNetCore.Mvc.Rendering;
using KEDI.Core.Helpers.Enumerations;
using static KEDI.Core.Premise.Controllers.PurchaseRequestController;
using KEDI.Core.System.Models;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CKBS.Controllers
{
    //[Authorize(Policy = "Permission")]
    [Privilege]
    public class PurchaseCreditMemoController : Controller
    {
        private readonly DataContext _context;
        private readonly IPurchaseCreditMemo _PurchasMemo;
        private readonly IPurchaseRepository _ipur;
        private readonly CultureLocalizer _culLocal;
        private readonly UtilityModule _formatNumber;
        public PurchaseCreditMemoController(
            DataContext context,
            IPurchaseCreditMemo purchaseCredit,
            IPurchaseRepository ipur,
            CultureLocalizer culLocal,
            UtilityModule formatNumber)
        {
            _context = context;
            _PurchasMemo = purchaseCredit;
            _ipur = ipur;
            _culLocal = culLocal;
            _formatNumber = formatNumber;
        }
        [HttpGet]
        [Privilege("A025")]
        public IActionResult PurchaseCreditMemo()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "A/P Credit Memo";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseCreditMemo = "highlight";

            var seriesPC = (from dt in _context.DocumentTypes.Where(i => i.Code == "PC")
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
            var seriesJE = (from dt in _context.DocumentTypes.Where(i => i.Code == "JE")
                            join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                            }).ToList();
            return View(new { seriesPC, seriesJE, genSetting = _formatNumber.GetGeneralSettingAdmin() });
        }

        [HttpPost]
        [Privilege("A025")]
        public IActionResult PurchaseCreditMemo(string data)
        {
            ViewBag.Invoice = data;
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "A/P Credit Memo";
            ViewBag.Menu = "show";

            return View();
        }
        // Basic Data
        [HttpGet]
        public async Task<IActionResult> GetItemMasterData(int ID)
        {
            var list = await _ipur.GetItemMasterDataCMAsync(ID, GetCompany().ID);
            return Ok(list.OrderBy(o => o.Code));
        }
        [HttpGet]
        public IActionResult GetItemDetails(string process, int itemid = 0, int curId = 0, string barcode = "")
        {
            var data = _ipur.GetItemDetailsCM(GetCompany(), process, itemid, curId, barcode);
            return (data == null || !data.Any()) && barcode != ""
                ? Ok(new { IsError = true, Error = $"No item with this Barcode \"{barcode}\"" })
                : Ok(data);
        }

        [HttpGet]
        public IActionResult Getbp(int id)
        {
            var data = _ipur.Getbp(id);
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetFilterLocaCurrency(int CurrencyID)
        {
            var list = await _ipur.GetExchangeRates(CurrencyID);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetBusinessPartners()
        {
            var list = await _ipur.GetBusinessPartnersAsync();
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> Getcurrency()
        {
            var list = await _ipur.GetcurrencyAsync(GetCompany().SystemCurrencyID);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetWarehouses(int ID)
        {
            var list = await _ipur.GetWarehousesAsync(ID);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetCurrencyDefualt()
        {
            var list = await _ipur.GetCurrencyDefualtAsync();
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

        private void ValidateSummary(PurchaseCreditMemo master)
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
            if (master.PurchaseCreditMemoDetails.Count <= 0)
            {
                ModelState.AddModelError("Details", "Master has no details");
            }
            else
            {
                for (var dt = 0; dt < master.PurchaseCreditMemoDetails.Count; dt++)
                {
                    master.PurchaseCreditMemoDetails[dt].TotalSys = master.PurchaseCreditMemoDetails[dt].Total * master.PurRate;

                    if (master.PurchaseCreditMemoDetails[dt].Qty <= 0)
                    {
                        ModelState.AddModelError($"Qty-{dt}", $"Detial at line {dt + 1}, Qty has to be greater than 0!");
                    }
                }
            }
            // if (!string.IsNullOrEmpty(master.ReffNo))
            // {
            //     bool isRefExisted = _context.Purchase_APs.AsNoTracking().Any(i => i.ReffNo == master.ReffNo);
            //     if (isRefExisted)
            //     {
            //         ModelState.AddModelError("RefNo", $"Transaction with Vendor Ref No. \"{master.ReffNo}\" already done.");
            //     }
            // }
            if(master.BranchID==0){
                ModelState.AddModelError("BranchID", "Please Select Branch");

            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePurchaseCreditMemo(string purchase, string type, string je, string serials, string batches)
        {
            ModelMessage msg = new();
            if (je == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(msg.Bind(ModelState));
            }
            PurchaseCreditMemo purchaseCreditMemo = JsonConvert.DeserializeObject<PurchaseCreditMemo>(purchase, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Series series_JE = JsonConvert.DeserializeObject<Series>(je, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var ex = _context.ExchangeRates.FirstOrDefault(i => i.CurrencyID == purchaseCreditMemo.PurCurrencyID) ?? new ExchangeRate();
            purchaseCreditMemo.SysCurrencyID = GetCompany().SystemCurrencyID;
            List<ItemsReturnPC> list = new();
            List<ItemsReturnPC> list_group = new();
            SeriesDetail seriesDetail = new();
            var seriesPC = _context.Series.FirstOrDefault(w => w.ID == purchaseCreditMemo.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == purchaseCreditMemo.DocumentTypeID).FirstOrDefault();
            var seriesJE = _context.Series.FirstOrDefault(w => w.ID == series_JE.ID);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            purchaseCreditMemo.SysCurrencyID = GetCompany().SystemCurrencyID;
            
            purchaseCreditMemo.UserID = GetUserID();
            ValidateSummary(purchaseCreditMemo);
            foreach (var item in purchaseCreditMemo.PurchaseCreditMemoDetails.ToList())
            {
                if (item.PurCopyType == PurCopyType.PurReserve)
                {
                    item.PurCopyType = PurCopyType.PurReserve;
                }
                if (item.PurCopyType == PurCopyType.PurAP)
                {
                    item.PurCopyType = PurCopyType.PurAP;
                }
                else
                {
                    item.PurCopyType = PurCopyType.None;
                }
                var itemMaster = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID);
                var gd = _context.GroupDUoMs.FirstOrDefault(W => W.GroupUoMID == itemMaster.GroupUomID && W.AltUOM == item.UomID);
                var Cost = item.PurchasPrice / gd.Factor * purchaseCreditMemo.PurRate;
                var TotalQty = item.Qty / gd.Factor;
                if (item.Qty <= 0)
                {
                    var ID = item.LineID;
                    purchaseCreditMemo.PurchaseCreditMemoDetails.Remove(item);
                    break;
                }
                if (itemMaster.Type == "FIFO")
                {
                    var itemWarehouseDetail = _context.WarehouseDetails.Where(w => w.WarehouseID == purchaseCreditMemo.WarehouseID && w.ItemID == item.ItemID && w.Cost == Cost).ToList();
                    if (TotalQty > itemWarehouseDetail.Sum(s => s.InStock))
                    {
                        ModelState.AddModelError("ItemID", "Item name " + itemMaster.KhmerName + ", Price =" + item.PurchasPrice + ", In stock = " + TotalQty + ". ");
                    }
                }
                else
                {
                    var itemWarehouseDetail = _context.WarehouseDetails.Where(w => w.WarehouseID == purchaseCreditMemo.WarehouseID && w.ItemID == item.ItemID).ToList();
                    if (TotalQty > itemWarehouseDetail.Sum(s => s.InStock))
                    {
                        ModelState.AddModelError("ItemID", "Item name " + itemMaster.KhmerName + ", Price =" + item.PurchasPrice + ", In stock = " + TotalQty + ". ");
                    }
                }
            }

            using (var t = _context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    // Checking Serial Batch //
                    List<APCSerialNumber> aPCSerialNumber = new();
                    List<APCSerialNumber> _aPCSerialNumber = new();
                    List<APCSerialNumber> SerialNumberold = new();
                    List<APCBatchNo> aPCBatchNos = new();
                    List<APCBatchNo> _aPCBatchNos = new();

                    aPCSerialNumber = serials != "[]" ? JsonConvert.DeserializeObject<List<APCSerialNumber>>(serials, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }) : aPCSerialNumber;

                    _ipur.CheckItemSerail(purchaseCreditMemo, purchaseCreditMemo.PurchaseCreditMemoDetails, aPCSerialNumber, ex);
                    // aPCSerialNumber = aPCSerialNumber.GroupBy(i => new { i.ItemID, i.Cost }).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in aPCSerialNumber.ToList())
                    {
                        foreach (var i in purchaseCreditMemo.PurchaseCreditMemoDetails.ToList())
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
                        foreach (var ob in _aPCSerialNumber)
                        {
                            if (ob.PurCopyType == (int)PurCopyType.PurAP)
                            {
                                var papd = _context.PurchaseAPDetail.FirstOrDefault(s => s.PurchaseDetailAPID == ob.BaseOnID) ?? new Purchase_APDetail();
                                if (ob.OpenQty == (decimal)papd.OpenQty)
                                {
                                    SerialNumberold.Add(ob);
                                    ob.Newrecord = false;
                                }
                            }
                            //  GetSerialDetials(decimal cost, int bpId, int itemId, int baseOnID = 0, int copyTuype = 0, string apsds = "[]", bool isAll = false)
                        }
                        foreach (var i in SerialNumberold)
                        {
                            i.APCSerialNumberDetial = await _ipur.GetSerialDetialsAsync(i.Cost, i.BpId, i.ItemID, i.BaseOnID, i.PurCopyType, "[]", false);
                            i.APCSerialNumberDetial.TotalAvailableQty = i.APCSerialNumberDetial.APCSNDDetials.Sum(x => x.Qty);
                        };

                        _aPCSerialNumber = _aPCSerialNumber.Where(s => s.Newrecord == true).ToList();
                        return Ok(new { IsSerail = true, Data = _aPCSerialNumber, Serialstatic = SerialNumberold });
                    }
                    // checking batch items
                    aPCBatchNos = batches != "[]" ? JsonConvert.DeserializeObject<List<APCBatchNo>>(batches, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }) : aPCBatchNos;
                    _ipur.CheckItemBatch(purchaseCreditMemo, purchaseCreditMemo.PurchaseCreditMemoDetails, aPCBatchNos, ex);
                    aPCBatchNos = aPCBatchNos.GroupBy(i => new { i.ItemID, i.Cost }).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in aPCBatchNos.ToList())
                    {
                        foreach (var i in purchaseCreditMemo.PurchaseCreditMemoDetails.ToList())
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
                    seriesDetail.Number = purchaseCreditMemo.Number;
                    seriesDetail.SeriesID = purchaseCreditMemo.SeriesID;
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.SaveChanges();
                    var seriesDetailID = seriesDetail.ID;
                    string Sno = purchaseCreditMemo.Number;
                    long No = long.Parse(Sno);
                    seriesPC.NextNo = Convert.ToString(No + 1);
                    if (long.Parse(seriesPC.NextNo) > long.Parse(seriesPC.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                        return Ok(msg.Bind(ModelState));
                    }
                    if (type == "Add")
                    {
                        purchaseCreditMemo.Status = "open";
                    }
                    purchaseCreditMemo.LocalCurID = GetCompany().LocalCurrencyID;
                    purchaseCreditMemo.CompanyID = GetCompany().ID;
                    purchaseCreditMemo.SeriesDetailID = seriesDetailID;
                    purchaseCreditMemo.LocalSetRate = localSetRate;
                    purchaseCreditMemo.UserID = GetUserID();
                    purchaseCreditMemo.InvoiceNo = seriesDetail.Number;
                    //purchaseCreditMemo.BaseOnID = (int)PurchaseARD.PurchaseAPID;
                    _context.Series.Update(seriesJE);
                    _context.Series.Update(seriesPC);
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.PurchaseCreditMemos.Update(purchaseCreditMemo);
                    _context.SaveChanges();
                    if (type == "Add")
                    {
                        var freight = UpdateFreight(purchaseCreditMemo);
                        if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                        {
                            _PurchasMemo.GoodIssuesStockBasic(purchaseCreditMemo.PurchaseMemoID, "Add", _aPCSerialNumber, _aPCBatchNos, freight);
                        }
                        else
                        {
                            _PurchasMemo.GoodIssuesStock(purchaseCreditMemo.PurchaseMemoID, "Add", _aPCSerialNumber, _aPCBatchNos, freight);
                        }

                        t.Commit();
                        ModelState.AddModelError("success", "Item save successfully.");
                        msg.Approve();
                    }
                    else if (type == "APR")
                    {
                        var PurchaseAPR = _context.PurchaseAPReserves.FirstOrDefault(w => w.ID == purchaseCreditMemo.BaseOnID) ?? new PurchaseAPReserve();
                        var purMemoD = purchaseCreditMemo.PurchaseCreditMemoDetails.Where(w => w.PurchaseCreditMemoID == purchaseCreditMemo.PurchaseMemoID).ToList();
                        var OutgoingPay = _context.OutgoingPaymentVendors.FirstOrDefault(w => w.SeriesDetailID == PurchaseAPR.SeriesDetailID && w.TypePurchase == TypePurchase.APReserve) ?? new OutgoingPaymentVendor();
                        decimal grantotalpay = (decimal)PurchaseAPR.FrieghtAmount + (decimal)PurchaseAPR.TaxValue + PurchaseAPR.SubTotalAfterDis;
                        decimal appliedAmount = grantotalpay - (decimal)PurchaseAPR.AppliedAmount;
                        decimal grandtotalMemo = (decimal)purchaseCreditMemo.FrieghtAmount + (decimal)purchaseCreditMemo.TaxValue + purchaseCreditMemo.SubTotalAfterDis;
                        if (grandtotalMemo > appliedAmount)
                        {
                            ModelState.AddModelError("grandtotalMemo", "Choose:<br/>1: Total Amount = " + grandtotalMemo + " bigger than AppliedAmount in A/P Reserve Invoice so You can't do CreditMemo.<br/>2: You can do CreditMemo Total Amount =" + appliedAmount + " or smaller than  " + appliedAmount + " <br>3: You must to cancel OutgoingPayment.");
                            return Ok(msg.Bind(ModelState));
                        }
                        if (purchaseCreditMemo.SubTotalSys > PurchaseAPR.BalanceDueSys)
                        {
                            ModelState.AddModelError("SumInvoice", "Your PurchaseAP BalanceDue = " + PurchaseAPR.BalanceDue + ", You can't do CreditMemo !");
                            return Ok(msg.Bind(ModelState));
                        }

                        else
                        {
                            foreach (var obj in purMemoD)
                            {
                                var PurchseAPRD = _context.PurchaseAPReserveDetails.FirstOrDefault(w => w.ID == obj.LineID && w.PurchaseAPReserveID == PurchaseAPR.ID);
                                if (PurchseAPRD != null)
                                {
                                    PurchseAPRD.OpenQty -= obj.Qty;
                                    _context.PurchaseAPReserveDetails.Update(PurchseAPRD);
                                    _context.SaveChanges();
                                }
                            }

                            PurchaseAPR.BalanceDue -= purchaseCreditMemo.BalanceDue;//purchaseCreditMemo.SubTotal ;
                            PurchaseAPR.BalanceDueSys -= purchaseCreditMemo.BalanceDueSys;////purchaseCreditMemo.SubTotalSys ;
                            PurchaseAPR.AppliedAmount += purchaseCreditMemo.BalanceDue;////purchaseCreditMemo.SubTotal ;
                            if (PurchaseAPR.BalanceDue == 0)
                            {
                                PurchaseAPR.Status = "close";
                            }
                            var PurMemo = _context.PurchaseCreditMemos.FirstOrDefault(w => w.PurchaseMemoID == purchaseCreditMemo.PurchaseMemoID) ?? new PurchaseCreditMemo();
                            PurMemo.BalanceDue = 0;
                            PurMemo.BalanceDueSys = 0;
                            PurMemo.Status = "close";
                            _context.PurchaseCreditMemos.Update(PurMemo);
                            _context.PurchaseAPReserves.Update(PurchaseAPR);
                            _context.SaveChanges();
                            var freight = UpdateFreight(purchaseCreditMemo);
                            _PurchasMemo.GoodIssuesStockAPReserve((int)PurchaseAPR.CopyToNote, purchaseCreditMemo.PurchaseMemoID, "PU", _aPCSerialNumber, _aPCBatchNos, freight);
                            t.Commit();
                            ModelState.AddModelError("success", "Item save successfully.");
                            msg.Approve();
                        }
                    }
                    else
                    {
                        var PurchaseAR = _context.Purchase_APs.FirstOrDefault(w => w.PurchaseAPID == purchaseCreditMemo.BaseOnID) ?? new Purchase_AP();
                        var purMemoD = purchaseCreditMemo.PurchaseCreditMemoDetails.Where(w => w.PurchaseCreditMemoID == purchaseCreditMemo.PurchaseMemoID).ToList();
                        var OutgoingPay = _context.OutgoingPaymentVendors.FirstOrDefault(w => w.SeriesDetailID == PurchaseAR.SeriesDetailID && w.TypePurchase == TypePurchase.AP) ?? new OutgoingPaymentVendor(); ;
                        decimal grantotalpay = (decimal)PurchaseAR.FrieghtAmount + (decimal)PurchaseAR.TaxValue + PurchaseAR.SubTotalAfterDis;
                        decimal appliedAmount = grantotalpay - (decimal)PurchaseAR.AppliedAmount;
                        decimal grandtotalMemo = (decimal)purchaseCreditMemo.FrieghtAmount + (decimal)purchaseCreditMemo.TaxValue + purchaseCreditMemo.SubTotalAfterDis;
                        if (grandtotalMemo > appliedAmount)
                        {
                            ModelState.AddModelError("grandtotalMemo", "Choose:<br/>1: Total Amount = " + grandtotalMemo + " bigger than AppliedAmount in A/P Invoice so You can't do CreditMemo.<br/>2: You can do CreditMemo Total Amount =" + appliedAmount + " or smaller than  " + appliedAmount + " <br>3: You must to cancel OutgoingPayment.");
                            return Ok(msg.Bind(ModelState));
                        }
                        if (purchaseCreditMemo.SubTotalSys > PurchaseAR.BalanceDueSys)
                        {
                            ModelState.AddModelError("SumInvoice", "Your PurchaseAP BalanceDue = " + PurchaseAR.BalanceDue + ", You can't do CreditMemo !");
                            return Ok(msg.Bind(ModelState));
                        }
                        else
                        {
                            foreach (var objap in purMemoD)
                            {
                                var PurchseARD = _context.PurchaseAPDetail.FirstOrDefault(w => w.PurchaseDetailAPID == objap.LineID && w.PurchaseAPID == PurchaseAR.PurchaseAPID);
                                if (PurchseARD != null)
                                {
                                    PurchseARD.OpenQty -= objap.Qty;
                                    _context.PurchaseAPDetail.Update(PurchseARD);
                                    _context.SaveChanges();
                                }
                            }
                            OutgoingPay.BalanceDue -= purchaseCreditMemo.BalanceDue;      //purchaseCreditMemo.SubTotal;
                            OutgoingPay.TotalPayment -= purchaseCreditMemo.BalanceDue;//   purchaseCreditMemo.SubTotal;
                            OutgoingPay.Applied_Amount += purchaseCreditMemo.BalanceDue;//  purchaseCreditMemo.SubTotal;
                            if (OutgoingPay.BalanceDue == 0)
                            {
                                OutgoingPay.Status = "close";
                            }
                            _context.OutgoingPaymentVendors.Update(OutgoingPay);
                            //
                            PurchaseAR.BalanceDue -= purchaseCreditMemo.BalanceDue;//purchaseCreditMemo.SubTotal;
                            PurchaseAR.BalanceDueSys -= purchaseCreditMemo.BalanceDueSys;//purchaseCreditMemo.SubTotalSys;
                            PurchaseAR.AppliedAmount += purchaseCreditMemo.BalanceDue;//purchaseCreditMemo.SubTotal;
                            if (PurchaseAR.BalanceDue == 0)
                            {
                                PurchaseAR.Status = "close";
                            }
                            var PurMemo = _context.PurchaseCreditMemos.FirstOrDefault(w => w.PurchaseMemoID == purchaseCreditMemo.PurchaseMemoID) ?? new PurchaseCreditMemo();
                            PurMemo.BalanceDue = 0;
                            PurMemo.BalanceDueSys = 0;
                            PurMemo.Status = "close";
                            _context.PurchaseCreditMemos.Update(PurMemo);
                            _context.Purchase_APs.Update(PurchaseAR);
                            _context.SaveChanges();
                            var freight = UpdateFreight(purchaseCreditMemo);
                            if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                            {
                                _PurchasMemo.GoodIssuesStockBasic(purchaseCreditMemo.PurchaseMemoID, "PU", _aPCSerialNumber, _aPCBatchNos, freight);
                            }
                            else
                            {
                                _PurchasMemo.GoodIssuesStock(purchaseCreditMemo.PurchaseMemoID, "PU", _aPCSerialNumber, _aPCBatchNos, freight);
                            }

                            t.Commit();
                            ModelState.AddModelError("success", "Item save successfully.");
                            msg.Approve();
                        }
                    }
                }
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpPost]
        public IActionResult CheckSerailNumber(string serails)
        {
            List<APCSerialNumber> _serails = JsonConvert.DeserializeObject<List<APCSerialNumber>>(serails, new JsonSerializerSettings
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

        [HttpPost]
        public IActionResult CheckBatchNo(string batches)
        {
            List<APCBatchNo> _batches = JsonConvert.DeserializeObject<List<APCBatchNo>>(batches, new JsonSerializerSettings
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

        [HttpGet]
        public async Task<IActionResult> GetSerialDetials(decimal cost, int bpId, int itemId, int baseOnID = 0, int copyTuype = 0, string apsds = "[]", bool isAll = false)
        {
            var data = await _ipur.GetSerialDetialsAsync(cost, bpId, itemId, baseOnID, copyTuype, apsds, isAll);
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetBatchNoDetials(decimal cost, int bpId, int itemId, int uomID)
        {
            var data = await _ipur.GetBatchNoDetialsAsync(cost, bpId, itemId, uomID);
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetPurchaseAP(int vendorId)
        {
            var data = await _context.Purchase_APs.Where(i => i.Status == "open" && i.VendorID == vendorId && i.BranchID == GetBranchID() && i.CompanyID == GetCompany().ID).ToListAsync();
            var list = await _ipur.GetAllPurPOAsync(data);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetAPreserve(int vendorId)
        {
            var data = await _context.PurchaseAPReserves.Where(i => i.Status == "open" && i.VendorID == vendorId && i.BranchID == GetBranchID() && i.CompanyID == GetCompany().ID).ToListAsync();
            var list = await _ipur.GetAllPurPOAsync(data);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetPurAPDetailCopy(int seriesId, string number)
        {
            var list = await _ipur.CopyPurchaseAPAsync(seriesId, number, GetCompany().ID);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetPurCaseAPD(int id, int copytype)
        {
            Purchase_APDetail obj = new Purchase_APDetail();
            var purd = copytype == (int)PurCopyType.PurAP ? await _context.PurchaseAPDetail.FirstOrDefaultAsync(s => s.PurchaseDetailAPID == id) ?? new Purchase_APDetail() : obj;
            return Ok(purd);
        }
        [HttpGet]
        public async Task<IActionResult> GetPurAPReserveDetailCopy(int seriesId, string number)
        {
            var list = await _ipur.CopyPurchaseAPReserveAsync(seriesId, number, GetCompany().ID);
            return Ok(list);
        }
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> FindPurchaseCreditMemo(string number, int seriesID)
        {
            var data = await _ipur.FindPurchaseCreditMemoAsync(seriesID, number, GetCompany().ID);
            if (data.PurchaseCreditMemo == null)
            {
                return Ok(new { Error = true, Message = $"Cannot find invoice \"{number}\"" });
            }
            return Ok(data);
        }
        private FreightPurchase UpdateFreight(PurchaseCreditMemo memo)
        {
            var freight = memo.FreightPurchaseView;
            if (freight != null)
            {
                //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                freight.PurID = memo.PurchaseMemoID;
                freight.PurType = PurCopyType.PurCreditMemo;
                freight.OpenExpenceAmount = freight.ExpenceAmount;
                _context.FreightPurchases.Update(freight);
                _context.SaveChanges();
            }
            return freight;
        }
        [Privilege("A025")]
        public IActionResult PurchaseCreditMemoHistory()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "A/P Credit Memo";
            ViewBag.Subpage = "A/P Credit Memo History";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseCreditMemo = "highlight";
            return View(new { Url = _formatNumber.PrintTemplateUrl() });

        }


        public Dictionary<int, string> Typevatt => EnumHelper.ToDictionary(typeof(PVatType));

        [HttpGet]
        public IActionResult GetPurchaseMemoReport(int BranchID, int WarehouseID, int VendorID, string DocumentDate, string PostingDate, bool check)
        {
            List<PurchaseCreditMemo> ServiceCalls = new();
            //filter WareHouse
            if (WarehouseID != 0 && VendorID == 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseCreditMemos.Where(w => w.WarehouseID == WarehouseID).ToList();
            }
            //filter Vendor
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseCreditMemos.Where(w => w.VendorID == VendorID).ToList();
            }
            //filter WareHouse and VendorName
            else if (WarehouseID != 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseCreditMemos.Where(w => w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            //filter all item
            else if (BranchID != 0 && WarehouseID == 0 && VendorID == 0 && PostingDate == null && DocumentDate == null)
            {
                ServiceCalls = _context.PurchaseCreditMemos.Where(w => w.UserID == BranchID).ToList();
            }
            //filter warehouse, vendor, datefrom ,dateto
            else if (WarehouseID != 0 & VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseCreditMemos.Where(w => w.VendorID == VendorID && w.WarehouseID == WarehouseID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter vendor and Datefrom and Dateto
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseCreditMemos.Where(w => w.VendorID == VendorID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter warehouse and Datefrom and DateTo
            else if (WarehouseID != 0 && VendorID == 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter Datefrom and DateTo
            else if (WarehouseID == 0 && VendorID == 0 && PostingDate != null && DocumentDate != null)
            {
                ServiceCalls = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.PostingDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            else
            {
                return Ok(new List<PurchaseCreditMemo>());
            }
            var list = (from s in ServiceCalls
                        join cus in _context.BusinessPartners on s.VendorID equals cus.ID
                        join item in _context.UserAccounts on s.UserID equals item.ID
                        select new PurchaseReport
                        {
                            ID = s.PurchaseMemoID,
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
        public IActionResult GetPurchaseCreditMemoByWarehouse(int BranchID, int WarehouseID, string PostingDate, string DocumenteDate, string DeliveryDate, string Check)
        {
            var list = (from P in _PurchasMemo.ReportPurchaseCreditMemo(BranchID, WarehouseID, PostingDate, DocumenteDate, DeliveryDate, Check)
                        select new ReportPurchasCreditMemo
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            UserName = P.UserName,
                            ExchangeRate = P.ExchangeRate,
                            Balance_due = P.Balance_due,
                            Balance_due_sys = P.Balance_due_sys,
                            BusinessName = P.BusinessName,
                            Status = P.Status,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList()
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseCreditMemoByPostingDate(int BranchID, int WarehouseID, string PostingDate, string DocumenteDate, string DeliveryDate, string Check)
        {
            var list = (from P in _PurchasMemo.ReportPurchaseCreditMemo(BranchID, WarehouseID, PostingDate, DocumenteDate, DeliveryDate, Check)
                        select new ReportPurchasCreditMemo
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            UserName = P.UserName,
                            ExchangeRate = P.ExchangeRate,
                            Balance_due = P.Balance_due,
                            Balance_due_sys = P.Balance_due_sys,
                            BusinessName = P.BusinessName,
                            Status = P.Status,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList()
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseCreditMemoByDocumentDate(int BranchID, int WarehouseID, string PostingDate, string DocumenteDate, string DeliveryDate, string Check)
        {
            var list = (from P in _PurchasMemo.ReportPurchaseCreditMemo(BranchID, WarehouseID, PostingDate, DocumenteDate, DeliveryDate, Check)
                        select new ReportPurchasCreditMemo
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            UserName = P.UserName,
                            ExchangeRate = P.ExchangeRate,
                            Balance_due = P.Balance_due,
                            Balance_due_sys = P.Balance_due_sys,
                            BusinessName = P.BusinessName,
                            Status = P.Status,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList()
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseCreditMemoByDeliveryDatedDate(int BranchID, int WarehouseID, string PostingDate, string DocumenteDate, string DeliveryDate, string Check)
        {
            var list = (from P in _PurchasMemo.ReportPurchaseCreditMemo(BranchID, WarehouseID, PostingDate, DocumenteDate, DeliveryDate, Check)
                        select new ReportPurchasCreditMemo
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            UserName = P.UserName,
                            ExchangeRate = P.ExchangeRate,
                            Balance_due = P.Balance_due,
                            Balance_due_sys = P.Balance_due_sys,
                            BusinessName = P.BusinessName,
                            Status = P.Status,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList()
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseCreditMemoAllItem(int BranchID, int WarehouseID, string PostingDate, string DocumenteDate, string DeliveryDate, string Check)
        {
            var list = (from P in _PurchasMemo.ReportPurchaseCreditMemo(BranchID, WarehouseID, PostingDate, DocumenteDate, DeliveryDate, Check)
                        select new ReportPurchasCreditMemo
                        {
                            ID = P.ID,
                            InvoiceNo = P.InvoiceNo,
                            UserName = P.UserName,
                            ExchangeRate = P.ExchangeRate,
                            Balance_due = P.Balance_due,
                            Balance_due_sys = P.Balance_due_sys,
                            BusinessName = P.BusinessName,
                            Status = P.Status,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList()
                        }).ToList();
            return Ok(list);
        }

    }
}