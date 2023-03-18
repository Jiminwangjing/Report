using CKBS.AppContext;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.Services.Inventory;
using KEDI.Core.Premise.Models.Services.Purchase;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.ServicesClass.Purchase;
using KEDI.Core.Premise.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Controllers
{
    public class PurchaseRequestController : Controller
    {
        private readonly DataContext _context;
        private readonly IPurchaseOrder _order;
        private readonly IPurchaseRepository _ipur;
        private readonly UtilityModule _formatNumber;

        public PurchaseRequestController(DataContext context, IPurchaseOrder order, IPurchaseRepository ipur, UtilityModule formatNumber)
        {
            _context = context;
            _order = order;
            _ipur = ipur;
            _formatNumber = formatNumber;
        }

        public IActionResult Index()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase Order";
            ViewBag.Menu = "show";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseRequest = "highlight";
            ViewBag.Currency = new SelectList(_context.Currency.Where(i => !i.Delete), "ID", "Description");
            return View(new { seriesPO = _formatNumber.GetSeries("PR"), genSetting = _formatNumber.GetGeneralSettingAdmin() });
        }
        public enum PVatType
        {
            Defualt,
            Template01,
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
        public async Task<IActionResult> GetRequesters()
        {
            var list = await _context.UserAccounts.Include(i => i.Employee).Where(i => !i.Delete).Select(i => new Requester
            {
                Username = i.Username,
                Code = i.Employee.Code,
                Name = i.Employee.Name,
                ID = i.ID
            }).ToListAsync();
            return Ok(list);
        }
        [HttpGet]
        public async Task<IActionResult> GetVendors()
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
        public async Task<IActionResult> GetItemMasterData()
        {
            var list = await _ipur.GetItemMasterDataPRAsync(GetCompany().ID);
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetItemDetails(int itemid = 0, int curId = 0, string barcode = "")
        {
            var data = _ipur.GetItemDetails(GetCompany(), itemid, curId, barcode, PurCopyType.PurRequest);
            return data == null && barcode != ""
                ? Ok(new { IsError = true, Error = $"No item with this Barcode {barcode} ..." })
                : Ok(data);
        }
        public IActionResult Getbp(int id)
        {
            var data = _ipur.Getbp(id);
            return Ok(data);
        }

        private void ValidateSummary(PurchaseRequest master)
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
            if (master.PurchaseRequestDetails.Count <= 0)
            {
                ModelState.AddModelError("Details", "Master has no details");
            }
            else
            {
                for (var dt = 0; dt < master.PurchaseRequestDetails.Count; dt++)
                {
                    master.PurchaseRequestDetails[dt].TotalSys = master.PurchaseRequestDetails[dt].Total * master.PurRate;

                    if (master.PurchaseRequestDetails[dt].Qty <= 0)
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
            if (master.RequesterID == 0)
            {
                ModelState.AddModelError("Requester", "Please choose a requester.");
            }
            if(master.BranchID==0){
                ModelState.AddModelError("BranchID","Please Select Branch");
            }

        }

        [HttpPost]
        public async Task<IActionResult> SavePurchaseRequest(string purchase)
        {
            PurchaseRequest PurchaseRequest = JsonConvert.DeserializeObject<PurchaseRequest>(purchase, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            if(PurchaseRequest.SeriesID==0){
                ModelState.AddModelError("SeriesID","Please Create Invoice Number");
            }
            var series = await _context.Series.FirstOrDefaultAsync(w => w.ID == PurchaseRequest.SeriesID) ?? new Series();
            ModelMessage msg = new();
            PurchaseRequest.SysCurrencyID = GetCompany().SystemCurrencyID;
            var _localSetRate = await _context.ExchangeRates.FirstOrDefaultAsync(w => w.CurrencyID == GetCompany().LocalCurrencyID);
            var localSetRate = _localSetRate.SetRate;
          
            PurchaseRequest.LocalCurID = _localSetRate.CurrencyID;
            PurchaseRequest.UserID = GetUserID();
            ValidateSummary(PurchaseRequest);
            if (ModelState.IsValid)
            {
                SeriesDetail seriesDetail = new();
                using var t = _context.Database.BeginTransaction();

                seriesDetail.ID = 0;
                seriesDetail.Number = series.NextNo;
                seriesDetail.SeriesID = PurchaseRequest.SeriesID;
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
                PurchaseRequest.Number = Sno;
                PurchaseRequest.Status = "open";
                PurchaseRequest.SeriesDetailID = seriesDetailID;
                PurchaseRequest.LocalCurID = GetCompany().LocalCurrencyID;
                PurchaseRequest.LocalSetRate = localSetRate;
                PurchaseRequest.CompanyID = GetCompany().ID;
                _context.Series.Update(series);
                _context.SeriesDetails.Update(seriesDetail);
                _context.PurchaseRequests.Update(PurchaseRequest);
                await _context.SaveChangesAsync();
                var freight = PurchaseRequest.FreightPurchaseView;
                UpdateFreight(freight, PurchaseRequest.ID);
            t.Commit();
                ModelState.AddModelError("success", "Item save successfully.");
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public async Task<IActionResult> FindPurchaseRequest(int seriesID, string number)
        {
            var data = await _ipur.FindPurchaseRequestAsync(seriesID, number, GetCompany().ID);
            if (data.PurchaseOrder.ID == 0)
            {
                return Ok(new { Error = true, Message = $"Cannot find invoice \"{number}\"" });
            }
            return Ok(data);
        }
        [HttpGet]
        public async Task<IActionResult> CopyPurchaseRequest(int seriesID, string number)
        {
            var data = await _ipur.CopyPurchaseRequestAsync(seriesID, number, GetCompany().ID);
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
                freight.PurType = PurCopyType.PurRequest;
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
        public IActionResult PurchaseRequestHistory()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase Request";
            ViewBag.Menu = "show";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseRequest = "highlight";
            string url = _formatNumber.PrintTemplateUrl();
            return View(new { Url = url });

        }
        [HttpGet]
        public IActionResult GetPurchaseRequestReport(int UserID, int BranchID, int WarehouseID, string PostingDate, string DocumentDate, bool check)
        {
            var list = _order.GetAllPurchaseRequests(UserID, BranchID, WarehouseID, PostingDate, DocumentDate, check);
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
        //02.02.2020
        [HttpPost]
        public IActionResult PRCancel(int purchaseID)
        {
            var pr = _context.PurchaseRequests.Find(purchaseID);
            if (pr != null)
            {
                pr.Status = "close";
                _context.SaveChanges();
            }
            return Ok();
        }
    }
    public class Requester
    {
        public string Username { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }
    }
}
