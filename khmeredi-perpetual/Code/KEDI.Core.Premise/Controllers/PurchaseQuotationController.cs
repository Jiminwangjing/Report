using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using KEDI.Core.Helpers.Enumerations;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.Services.Purchase;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static KEDI.Core.Premise.Controllers.PurchaseRequestController;

namespace KEDI.Core.Premise.Controllers
{
    public class PurchaseQuotationController : Controller
    {
        private readonly DataContext _context;
        private readonly IPurchaseOrder _order;
        private readonly IPurchaseRepository _ipur;
        private readonly UtilityModule _formatNumber;
        public PurchaseQuotationController(DataContext context, IPurchaseOrder order, IPurchaseRepository ipur, UtilityModule formatNumber)
        {
            _context = context;
            _order = order;
            _ipur = ipur;
            _formatNumber = formatNumber;
        }
        public Dictionary<int, string> Typevat => EnumHelper.ToDictionary(typeof(PVatType));
        public IActionResult Index()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase Quotation";
            ViewBag.Menu = "show";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseQuotation = "highlight";
            return View(new { seriesPO = _formatNumber.GetSeries("PQ"), genSetting = _formatNumber.GetGeneralSettingAdmin() });
        }
        public IActionResult GetDisplayFormatCurrency(int curId)
        {
            var data = _formatNumber.GetGeneralSettingAdmin(curId);
            return Ok(data);
        }
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
        public async Task<IActionResult> GetFilterLocaCurrency(int CurrencyID)
        {
            var list = await _ipur.GetExchangeRates(CurrencyID);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetWarehouses(int ID)
        {
            var list = await _ipur.GetWarehousesAsync(ID);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> Getcurrency()
        {
            var list = await _ipur.GetcurrencyAsync(GetCompany().SystemCurrencyID);
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetExchangeRate()
        {
            var data = await _ipur.GetExchange();
            return Ok(data);
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
        public IActionResult Getbp(int id)
        {
            var data = _ipur.Getbp(id);
            return Ok(data);
        }
        private void UpdateSourceCopy(IEnumerable<dynamic> details, dynamic copyMaster, IEnumerable<dynamic> copyDedails, PurCopyType copyType)
        {
            foreach (var cd in copyDedails)
            {
                foreach (var d in details)
                {
                    switch (copyType)
                    {
                        case PurCopyType.PurRequest:
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

        private void ValidateSummary(PurchaseQuotation master)
        {
            var postingPeriod = _context.PostingPeriods.Where(w => w.PeroidStatus == PeroidStatus.Unlocked).ToList();
            if (postingPeriod.Count <= 0)
            {
                ModelState.AddModelError("PostingDate", "PeroiStatus is closed or locked");
            }
            else
            {
                bool isValidPostingDate = postingPeriod.Any(item => DateTime.Compare(master.PostingDate, item.PostingDateFrom) >= 0 && DateTime.Compare(master.PostingDate, item.PostingDateTo) <= 0);
                bool isValidDocumentDate = postingPeriod.Any(item => DateTime.Compare(master.DocumentDate, item.DocuDateFrom) >= 0 && DateTime.Compare(master.DocumentDate, item.DocuDateTo) <= 0);
                if (!isValidPostingDate)
                {
                    ModelState.AddModelError("PostingDate", "PostingDate is closed or locked");
                }

                if (!isValidDocumentDate)
                {
                    // ModelState.AddModelError("DocumentDate", "DocumentDate is closed or locked");
                    ModelState.AddModelError("DocumentDate", "DocumentDate is closed or locked");
                }
            }
            if (master.PurchaseQuotationDetails.Count <= 0)
            {
                ModelState.AddModelError("Details", "Master has no details");
            }
            else
            {
                for (var dt = 0; dt < master.PurchaseQuotationDetails.Count; dt++)
                {
                    master.PurchaseQuotationDetails[dt].TotalSys = master.PurchaseQuotationDetails[dt].Total * master.PurRate;

                    if (master.PurchaseQuotationDetails[dt].Qty <= 0)
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
            if (master.WarehouseID == 0) ModelState.AddModelError("Warehouse", "Please select warehouse.");
            if (master.VendorID == 0) ModelState.AddModelError("Vendor", "Please choose vendor.");
             if(master.BranchID==0) ModelState.AddModelError("BranchID","Please Select Branch");
        }

        [HttpPost]
        public async Task<IActionResult> SavePurchaseQuotation(string purchase)
        {
            PurchaseQuotation PurchaseQuotation = JsonConvert.DeserializeObject<PurchaseQuotation>(purchase, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var series = await _context.Series.FirstOrDefaultAsync(w => w.ID == PurchaseQuotation.SeriesID);
            ModelMessage msg = new();
            PurchaseQuotation.SysCurrencyID = GetCompany().SystemCurrencyID;
            var _localSetRate = await _context.ExchangeRates.FirstOrDefaultAsync(w => w.CurrencyID == GetCompany().LocalCurrencyID);
            var localSetRate = _localSetRate.SetRate;
           
            PurchaseQuotation.LocalCurID = _localSetRate.CurrencyID;
            PurchaseQuotation.UserID = GetUserID();
            if (PurchaseQuotation.BaseOnID != 0)
                PurchaseQuotation.CopyType = PurCopyType.PurRequest;
            ValidateSummary(PurchaseQuotation);
            if (ModelState.IsValid)
            {
                SeriesDetail seriesDetail = new();
                if (PurchaseQuotation.ID == 0)
                {
                    using var t = _context.Database.BeginTransaction();
                    seriesDetail.ID = 0;
                    seriesDetail.Number = series.NextNo;
                    seriesDetail.SeriesID = PurchaseQuotation.SeriesID;
                    _context.SeriesDetails.Update(seriesDetail);
                    await _context.SaveChangesAsync();
                    var seriesDetailID = seriesDetail.ID;
                    string Sno = series.NextNo;
                    long No = long.Parse(Sno);
                    series.NextNo = Convert.ToString(No + 1);
                    if (long.Parse(series.NextNo) > long.Parse(series.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                        return Ok(msg.Bind(ModelState));
                    }
                    PurchaseQuotation.Number = Sno;
                    PurchaseQuotation.InvoiceNo = Sno;
                    PurchaseQuotation.Status = "open";
                    PurchaseQuotation.SeriesDetailID = seriesDetailID;
                    PurchaseQuotation.LocalCurID = GetCompany().LocalCurrencyID;
                    PurchaseQuotation.LocalSetRate = localSetRate;
                    PurchaseQuotation.CompanyID = GetCompany().ID;
                    _context.Series.Update(series);
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.PurchaseQuotations.Update(PurchaseQuotation);
                    await _context.SaveChangesAsync();
                    if (PurchaseQuotation.BaseOnID != 0)
                    {
                        PurchaseRequest Prquest = new();
                        Prquest = _context.PurchaseRequests.FirstOrDefault(s => s.ID == PurchaseQuotation.BaseOnID) ?? new PurchaseRequest();
                        var prqd = _context.PurchaseRequestDetails.Where(s => s.PurchaseRequestID == Prquest.ID).ToList();
                        UpdateSourceCopy(PurchaseQuotation.PurchaseQuotationDetails, Prquest, prqd, PurCopyType.PurRequest);
                    }

                    foreach (var check in PurchaseQuotation.PurchaseQuotationDetails.ToList())
                    {
                        if (check.Qty <= 0)
                        {
                            _context.Remove(check);
                            await _context.SaveChangesAsync();
                        }
                    }
                    await _order.GoodReceiptStockOrderAsync(PurchaseQuotation.ID, PurchaseQuotation.WarehouseID);
                    var freight = PurchaseQuotation.FreightPurchaseView;
                    UpdateFreight(freight, PurchaseQuotation.ID);
                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
                else
                {
                    seriesDetail.ID = PurchaseQuotation.SeriesDetailID;
                    seriesDetail.Number = PurchaseQuotation.Number;
                    seriesDetail.SeriesID = PurchaseQuotation.SeriesID;
                    _context.Series.Update(series);
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.Update(PurchaseQuotation);
                    await _context.SaveChangesAsync();
                    foreach (var item in PurchaseQuotation.PurchaseQuotationDetails.ToList())
                    {
                        using var t = _context.Database.BeginTransaction();

                        var item_master = await _context.ItemMasterDatas.FirstOrDefaultAsync(w => w.ID == item.ItemID);
                        var gduom = await _context.GroupDUoMs.FirstOrDefaultAsync(w => w.GroupUoMID == item_master.GroupUomID && w.AltUOM == item.UomID);
                        //Remove item order
                        if (item.Qty <= 0)
                        {
                            var warehouse_sum = await _context.WarehouseSummary.FirstOrDefaultAsync(w => w.WarehouseID == PurchaseQuotation.WarehouseID && w.ItemID == item.ItemID);
                            if (warehouse_sum != null)
                            {
                                warehouse_sum.Ordered -= item.OldQty * gduom.Factor;
                                _context.WarehouseSummary.Update(warehouse_sum);
                                await _context.SaveChangesAsync();
                            }
                            //Update in item maser
                            item_master.StockOnHand -= (item.OldQty * gduom.Factor);
                            _context.ItemMasterDatas.Update(item_master);

                            _context.PurchaseQuotationDetails.Remove(item);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            var add_qty = item.Qty - item.OldQty;
                            var warehouse_sum = await _context.WarehouseSummary.FirstOrDefaultAsync(w => w.WarehouseID == PurchaseQuotation.WarehouseID && w.ItemID == item.ItemID);
                            if (warehouse_sum != null)
                            {
                                warehouse_sum.Ordered += (add_qty * gduom.Factor);
                                _context.WarehouseSummary.Update(warehouse_sum);
                                _context.SaveChanges();
                            }
                            //Update in item maser
                            item_master.StockOnHand += (add_qty * gduom.Factor);
                            _context.ItemMasterDatas.Update(item_master);
                            _context.SaveChanges();
                        }
                        var freight = PurchaseQuotation.FreightPurchaseView;
                        UpdateFreight(freight, PurchaseQuotation.ID);
                        t.Commit();
                        ModelState.AddModelError("success", "Item save successfully.");
                        msg.Approve();
                    }
                }
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public async Task<IActionResult> FindPurchaseQuotation(int seriesID, string number)
        {
            var data = await _ipur.FindPurchaseQuotationAsync(seriesID, number, GetCompany().ID);
            if (data.PurchaseOrder.ID == 0)
            {
                return Ok(new { Error = true, Message = $"Cannot find invoice \"{number}\"" });
            }
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> CopyPurchaseQuotation(int seriesID, string number)
        {
            var data = await _ipur.CopyPurchaseQuotationAsync(seriesID, number, GetCompany().ID);
            if (data.PurchaseOrder.ID == 0)
            {
                return Ok(new { Error = true, Message = $"Cannot find invoice \"{number}\"" });
            }
            return Ok(data);
        }
        private void UpdateFreight(FreightPurchase freight, int purID)
        {
            if (freight != null)
            {
                //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                freight.PurID = purID;
                freight.PurType = PurCopyType.PurQuote;
                freight.OpenExpenceAmount = freight.OpenExpenceAmount;
                _context.FreightPurchases.Update(freight);
                _context.SaveChanges();
            }
        }
        public bool CheckStatus(IEnumerable<PurchaseQuotationDetail> invoices)
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
        public bool CheckStatus(IEnumerable<PurchaseRequestDetail> invoices)
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
        public IActionResult PurchaseQuoteHistory()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase Quotation";
            ViewBag.Menu = "show";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseQuotation = "highlight";
            return View(new { Url = _formatNumber.PrintTemplateUrl() });

        }

        [HttpGet]
        public IActionResult GetPurchaseQuotationReport(int VendorID, int BranchID, int WarehouseID, string PostingDate, string DocumentDate, bool check)
        {

            List<PurchaseQuotation> ServiceCalls = new();
            //filter WareHouse
            if (WarehouseID != 0 && VendorID == 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseQuotations.Where(w => w.WarehouseID == WarehouseID).ToList();
            }
            //filter Vendor
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseQuotations.Where(w => w.VendorID == VendorID).ToList();
            }
            //filter WareHouse and VendorName
            else if (WarehouseID != 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseQuotations.Where(w => w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            //filter all item
            else if (BranchID != 0 && WarehouseID == 0 && VendorID == 0 && PostingDate == null && DocumentDate == null)
            {
                ServiceCalls = _context.PurchaseQuotations.Where(w => w.UserID == BranchID).ToList();
            }
            //filter warehouse, vendor, datefrom ,dateto
            else if (WarehouseID != 0 & VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseQuotations.Where(w => w.VendorID == VendorID && w.WarehouseID == WarehouseID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter vendor and Datefrom and Dateto
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseQuotations.Where(w => w.VendorID == VendorID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter warehouse and Datefrom and DateTo
            else if (WarehouseID != 0 && VendorID == 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseQuotations.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter Datefrom and DateTo
            else if (WarehouseID == 0 && VendorID == 0 && PostingDate != null && DocumentDate != null)
            {
                ServiceCalls = _context.PurchaseQuotations.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.PostingDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            else
            {
                return Ok(new List<PurchaseQuotation>());
            }
            var list = (from s in ServiceCalls
                        join cus in _context.BusinessPartners on s.VendorID equals cus.ID
                        join item in _context.UserAccounts on s.UserID equals item.ID
                        select new PurchaseReport
                        {
                            ID = s.ID,
                            Invoice = s.InvoiceNo,
                            Requester = cus.Name,
                            UserName = item.Username,
                            Balance = s.BalanceDue,
                            ExchangeRate = s.PurRate,
                            Cancele = "<i class= 'fa fa-ban'style='color:red;' ></i>",
                            Status = s.Status,
                            VatType = Typevat.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),

                        }).ToList();
            return Ok(list);
        }

        public IActionResult GetPurchaseQuotation(int BranchID)
        {
            var list = _order.GetAllPurchaeQuotation(BranchID).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult FindPurchasQuotation(int ID, string Invoice)
        {
            var list = _order.GetPurchaseOrder_From_PurchaseQuotation(ID, Invoice);
            return Json(list);

        }
        public IActionResult GetPurchaseRequestCopy()
        {
            var list = (from pr in _context.PurchaseRequests.Where(s => s.Status == "open")
                        join dct in _context.DocumentTypes on pr.DocumentTypeID equals dct.ID
                        join req in _context.UserAccounts.Include(i => i.Employee) on pr.RequesterID equals req.ID
                        join currency in _context.Currency on pr.PurCurrencyID equals currency.ID
                        select new
                        {
                            pr.ID,
                            pr.SeriesID,
                            DocType = dct.Code,
                            Invoice = pr.Number,
                            ValidUntil = pr.DeliveryDate.ToString("MM-dd-yyyy"),
                            DocumentDate = pr.DocumentDate.ToString("MM-dd-yyyy"),
                            ReqiuredDate = pr.PostingDate.ToString("MM-dd-yyyy"),
                            RequesterUsername = req.Username,
                            RequesterName = req.Employee.Name,
                            RequesterCode = req.Employee.Code,
                            CurrencyName = currency.Description,
                            pr.SubTotal,
                            pr.BalanceDue,
                            pr.Remark

                        }).ToList();
            return Ok(list);
        }

        //02.02.2020

        [HttpPost]
        public IActionResult PQCancel(int PurchaseID)
        {
            _order.PQCancel(PurchaseID);
            return Ok();
        }

    }
}
