using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CKBS.AppContext;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.ServicesClass;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.Services.Purchase;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Helpers.Enumerations;
using static KEDI.Core.Premise.Controllers.PurchaseRequestController;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.ReportClass;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace CKBS.Controllers
{
    //[Authorize(Policy = "Permission")]
    [Privilege]
    public class PurchaseOrderController : Controller
    {
        private readonly DataContext _context;
        private readonly IPurchaseOrder _order;
        private readonly IPurchaseRepository _ipur;
        private readonly UtilityModule _formatNumber;
        public PurchaseOrderController(DataContext context, IPurchaseOrder order, IPurchaseRepository ipur, UtilityModule formatNumber)
        {
            _context = context;
            _order = order;
            _ipur = ipur;
            _formatNumber = formatNumber;
        }
        public Dictionary<int, string> Typevatt => EnumHelper.ToDictionary(typeof(PVatType));
        [Privilege("A023")]
        public IActionResult Purchaseorder()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase Order";
            ViewBag.Menu = "show";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseOrder = "highlight";
            return View(new { seriesPO = _formatNumber.GetSeries("PO"), genSetting = _formatNumber.GetGeneralSettingAdmin() });

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
        [Privilege("A023")]
        public IActionResult PurchaseOrderHistory()
        {
            ViewBag.style = "fa fa-shopping-cart";
            ViewBag.Main = "Purchase";
            ViewBag.Page = "Purchase Order";
            ViewBag.Menu = "show";
            ViewBag.PurchaseMenu = "show";
            ViewBag.PurchaseOrder = "highlight";
            return View(new { Url = _formatNumber.PrintTemplateUrl() });
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
                        case PurCopyType.PurQuote:
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
        private void ValidateSummary(PurchaseOrder master)
        {
            var postingPeriod = _context.PostingPeriods.Where(w => w.PeroidStatus == PeroidStatus.Unlocked).ToList();
            if (postingPeriod.Count <= 0)
            {
                ModelState.AddModelError("PostingDate", "PeroiStatus is closed or locked");
            }
            else
            {
                bool isValidPostingDate = false,
                    isValidDocumentDate = false;
                foreach (var item in postingPeriod)
                {
                    if (DateTime.Compare(master.PostingDate, item.PostingDateFrom) >= 0 && DateTime.Compare(master.PostingDate, item.PostingDateTo) <= 0)
                    {
                        isValidPostingDate = true;
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
            }
            if (master.PurchaseOrderDetails.Count <= 0)
            {
                ModelState.AddModelError("Details", "Master has no details");
            }
            else
            {
                for (var dt = 0; dt < master.PurchaseOrderDetails.Count; dt++)
                {
                    master.PurchaseOrderDetails[dt].TotalSys = master.PurchaseOrderDetails[dt].Total * master.PurRate;

                    if (master.PurchaseOrderDetails[dt].Qty <= 0)
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
            if (master.BranchID == 0) ModelState.AddModelError("BranchID", "Please Select BranchID.");

        }

        [HttpPost]
        public async Task<IActionResult> SavePurchaseOrder(string purchase, string Type, string copytype)
        {
            PurchaseOrder purchaseOrder = JsonConvert.DeserializeObject<PurchaseOrder>(purchase, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var series = await _context.Series.FirstOrDefaultAsync(w => w.ID == purchaseOrder.SeriesID);
            ModelMessage msg = new();
            purchaseOrder.SysCurrencyID = GetCompany().SystemCurrencyID;
            var _localSetRate = await _context.ExchangeRates.FirstOrDefaultAsync(w => w.CurrencyID == GetCompany().LocalCurrencyID);
            var localSetRate = _localSetRate.SetRate;
            
            purchaseOrder.LocalCurID = _localSetRate.CurrencyID;
            purchaseOrder.UserID = GetUserID();
            ValidateSummary(purchaseOrder);
            if (ModelState.IsValid)
            {
                if (Type == "Add" || Type == "Update")
                {
                    SeriesDetail seriesDetail = new();
                    if (purchaseOrder.PurchaseOrderID == 0)
                    {
                        using var t = _context.Database.BeginTransaction();
                        seriesDetail.ID = 0;
                        seriesDetail.Number = series.NextNo;
                        seriesDetail.SeriesID = purchaseOrder.SeriesID;
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
                        if (purchaseOrder.BaseOnID != 0 && copytype == "PQ")
                            purchaseOrder.CopyType = PurCopyType.PurQuote;
                        if (purchaseOrder.BaseOnID != 0 && copytype == "PR")
                            purchaseOrder.CopyType = PurCopyType.PurRequest;
                        purchaseOrder.Number = Sno;
                        purchaseOrder.InvoiceNo = Sno;
                        purchaseOrder.Status = "open";
                        purchaseOrder.SeriesDetailID = seriesDetailID;
                        purchaseOrder.LocalCurID = GetCompany().LocalCurrencyID;
                        purchaseOrder.LocalSetRate = localSetRate;
                        purchaseOrder.CompanyID = GetCompany().ID;
                        _context.Series.Update(series);
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.PurchaseOrders.Update(purchaseOrder);
                        await _context.SaveChangesAsync();
                        if (purchaseOrder.BaseOnID != 0 && copytype == "PQ")
                        {
                            PurchaseQuotation Prquest = new();
                            Prquest = _context.PurchaseQuotations.FirstOrDefault(s => s.ID == purchaseOrder.BaseOnID) ?? new PurchaseQuotation();
                            var prqd = _context.PurchaseQuotationDetails.Where(s => s.PurchaseQuotationID == Prquest.ID).ToList();
                            UpdateSourceCopy(purchaseOrder.PurchaseOrderDetails, Prquest, prqd, PurCopyType.PurQuote);

                        }
                        if (purchaseOrder.BaseOnID != 0 && copytype == "PR")
                        {
                            PurchaseRequest Pe = new();
                            Pe = _context.PurchaseRequests.FirstOrDefault(s => s.ID == purchaseOrder.BaseOnID) ?? new PurchaseRequest();
                            var prrd = _context.PurchaseRequestDetails.Where(s => s.PurchaseRequestID == Pe.ID).ToList();
                            UpdateSourceCopy(purchaseOrder.PurchaseOrderDetails, Pe, prrd, PurCopyType.PurRequest);
                        }
                        foreach (var check in purchaseOrder.PurchaseOrderDetails.ToList())
                        {
                            if (check.Qty <= 0)
                            {
                                _context.Remove(check);
                                await _context.SaveChangesAsync();
                            }
                        }
                        await _order.GoodReceiptStockOrderAsync(purchaseOrder.PurchaseOrderID, purchaseOrder.WarehouseID);
                        var freight = purchaseOrder.FreightPurchaseView;
                        UpdateFreight(freight, purchaseOrder.PurchaseOrderID);
                        t.Commit();
                        ModelState.AddModelError("success", "Item save successfully.");
                        msg.Approve();
                    }
                    else
                    {
                        seriesDetail.ID = purchaseOrder.SeriesDetailID;
                        seriesDetail.Number = purchaseOrder.Number;
                        seriesDetail.SeriesID = purchaseOrder.SeriesID;
                        _context.Series.Update(series);
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.Update(purchaseOrder);
                        await _context.SaveChangesAsync();
                        foreach (var item in purchaseOrder.PurchaseOrderDetails.ToList())
                        {
                            using var t = _context.Database.BeginTransaction();
                            if (ModelState.IsValid)
                            {
                                var item_master = await _context.ItemMasterDatas.FirstOrDefaultAsync(w => w.ID == item.ItemID);
                                var gduom = await _context.GroupDUoMs.FirstOrDefaultAsync(w => w.GroupUoMID == item_master.GroupUomID && w.AltUOM == item.UomID);
                                //Remove item order
                                if (item.Qty <= 0)
                                {
                                    var warehouse_sum = await _context.WarehouseSummary.FirstOrDefaultAsync(w => w.WarehouseID == purchaseOrder.WarehouseID && w.ItemID == item.ItemID);
                                    if (warehouse_sum != null)
                                    {
                                        warehouse_sum.Ordered -= item.OldQty * gduom.Factor;
                                        _context.WarehouseSummary.Update(warehouse_sum);
                                        await _context.SaveChangesAsync();
                                    }
                                    //Update in item maser
                                    item_master.StockOnHand -= (item.OldQty * gduom.Factor);
                                    _context.ItemMasterDatas.Update(item_master);

                                    _context.PurchaseOrderDetails.Remove(item);
                                    await _context.SaveChangesAsync();
                                }
                                else
                                {
                                    var add_qty = item.Qty - item.OldQty;
                                    var warehouse_sum = await _context.WarehouseSummary.FirstOrDefaultAsync(w => w.WarehouseID == purchaseOrder.WarehouseID && w.ItemID == item.ItemID);
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
                                var freight = purchaseOrder.FreightPurchaseView;
                                UpdateFreight(freight, purchaseOrder.PurchaseOrderID);
                                t.Commit();
                                ModelState.AddModelError("success", "Item save successfully.");
                                msg.Approve();
                            }
                        }
                    }

                }
                else if (Type == "PQ")
                {
                    var remark = purchaseOrder.Remark;
                    string InvoiceOrder = "";
                    double openqty = 0;
                    InvoiceOrder = (remark.Split("/"))[1];
                    List<PurchaseOrderDetail> Comfirn = new();
                    foreach (var items in purchaseOrder.PurchaseOrderDetails.ToList())
                    {
                        if (items.Qty > 0)
                        {
                            Comfirn.Add(items);
                        }
                    }
                    if (purchaseOrder.PurchaseOrderID == 0)
                    {
                        using var t = _context.Database.BeginTransaction();
                        _context.PurchaseOrders.Add(purchaseOrder);
                        _context.SaveChanges();
                        foreach (var check in purchaseOrder.PurchaseOrderDetails.ToList())
                        {
                            if (check.Qty <= 0)
                            {
                                _context.Remove(check);
                                await _context.SaveChangesAsync();
                            }
                        }
                        await _order.GoodReceiptStockOrderAsync(purchaseOrder.PurchaseOrderID, purchaseOrder.WarehouseID);

                        var purchaseQuotation = await _context.PurchaseQuotations.FirstOrDefaultAsync(x => x.InvoiceNo == InvoiceOrder);

                        int QuoID = purchaseQuotation.ID;
                        var detail = await _context.PurchaseQuotationDetails.Where(x => x.ID == QuoID && x.Delete == false).ToListAsync();
                        if (Comfirn.Count > 0)
                        {
                            foreach (var item in detail.ToList())
                            {
                                var items = Comfirn.FirstOrDefault(i => i.QuotationID == item.ID);
                                if (items != null)
                                {
                                    if (items.Qty > item.OpenQty)
                                    {
                                        openqty = item.OpenQty - item.OpenQty;
                                        var purchaseDetail_1 = await _context.PurchaseQuotationDetails.FirstOrDefaultAsync(x => x.ID == item.ID);
                                        purchaseDetail_1.OpenQty = openqty;
                                        _context.Update(purchaseDetail_1);
                                        await _context.SaveChangesAsync();
                                        if (openqty == 0)
                                        {
                                            purchaseDetail_1.Delete = true;
                                            _context.Update(purchaseDetail_1);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                    else
                                    {
                                        openqty = item.OpenQty - items.Qty;
                                        var purchaseDetail = await _context.PurchaseQuotationDetails.FirstOrDefaultAsync(x => x.ID == item.ID);
                                        purchaseDetail.OpenQty = openqty;
                                        _context.Update(purchaseDetail);
                                        await _context.SaveChangesAsync();
                                        if (openqty == 0)
                                        {
                                            purchaseDetail.Delete = true;
                                            _context.Update(purchaseDetail);
                                            _context.SaveChanges();
                                        }
                                    }

                                }
                            }
                        }
                        if (CheckStatus(detail))
                        {
                            var purchaseAP = await _context.PurchaseQuotations.FirstOrDefaultAsync(x => x.InvoiceNo == InvoiceOrder);
                            purchaseAP.Status = "close";
                            _context.Update(purchaseAP);
                            _context.SaveChanges();
                        }
                        var freight = purchaseOrder.FreightPurchaseView;
                        UpdateFreight(freight, purchaseOrder.PurchaseOrderID);
                        t.Commit();
                        ModelState.AddModelError("success", "Item save successfully.");
                        msg.Approve();
                    }
                }
                else if (Type == "PR")
                {
                    var remark = purchaseOrder.Remark;
                    string InvoiceOrder = "";
                    double openqty = 0;
                    InvoiceOrder = (remark.Split("/"))[1];
                    List<PurchaseOrderDetail> Comfirn = new();
                    foreach (var items in purchaseOrder.PurchaseOrderDetails.ToList())
                    {
                        if (items.Qty > 0)
                        {
                            Comfirn.Add(items);
                        }
                    }
                    if (purchaseOrder.PurchaseOrderID == 0)
                    {
                        using var t = _context.Database.BeginTransaction();
                        await _context.PurchaseOrders.AddAsync(purchaseOrder);
                        await _context.SaveChangesAsync();
                        foreach (var check in purchaseOrder.PurchaseOrderDetails.ToList())
                        {
                            if (check.Qty <= 0)
                            {
                                _context.Remove(check);
                                await _context.SaveChangesAsync();
                            }
                        }
                        await _order.GoodReceiptStockOrderAsync(purchaseOrder.PurchaseOrderID, purchaseOrder.WarehouseID);

                        var purchaseRequest = await _context.PurchaseRequests.FirstOrDefaultAsync(x => x.Number == InvoiceOrder);

                        int ReqestID = purchaseRequest.ID;
                        var detail = await _context.PurchaseRequestDetails.Where(x => x.PurchaseRequestID == ReqestID && x.Delete == false).ToListAsync();
                        if (Comfirn.Count > 0)
                        {
                            foreach (var item in detail.ToList())
                            {
                                var items = Comfirn.FirstOrDefault(i => item.ID == i.QuotationID);

                                if (items != null)
                                {
                                    if (items.Qty > item.OpenQty)
                                    {
                                        openqty = item.OpenQty - item.OpenQty;
                                        var purchaseDetail_1 = await _context.PurchaseRequestDetails.FirstOrDefaultAsync(x => x.ID == item.ID);
                                        purchaseDetail_1.OpenQty = openqty;
                                        _context.Update(purchaseDetail_1);
                                        await _context.SaveChangesAsync();
                                        if (openqty == 0)
                                        {
                                            purchaseDetail_1.Delete = true;
                                            _context.Update(purchaseDetail_1);
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                    else
                                    {
                                        openqty = item.OpenQty - items.Qty;
                                        var purchaseDetail = await _context.PurchaseRequestDetails.FirstOrDefaultAsync(x => x.ID == item.ID);
                                        purchaseDetail.OpenQty = openqty;
                                        _context.Update(purchaseDetail);
                                        await _context.SaveChangesAsync();
                                        if (openqty == 0)
                                        {
                                            purchaseDetail.Delete = true;
                                            _context.Update(purchaseDetail);
                                            await _context.SaveChangesAsync();
                                        }
                                    }

                                }
                            }
                        }
                        if (CheckStatus(detail))
                        {
                            var purchaseAP = await _context.PurchaseRequests.FirstOrDefaultAsync(x => x.Number == InvoiceOrder);
                            purchaseAP.Status = "close";
                            _context.Update(purchaseAP);
                            await _context.SaveChangesAsync();
                        }
                        var freight = purchaseOrder.FreightPurchaseView;
                        UpdateFreight(freight, purchaseOrder.PurchaseOrderID);
                        t.Commit();
                        ModelState.AddModelError("success", "Item save successfully.");
                        msg.Approve();
                    }
                }
            }
            return Ok(msg.Bind(ModelState));
        }
        private void UpdateFreight(FreightPurchase freight, int purID)
        {
            if (freight != null)
            {
                //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                freight.PurID = purID;
                freight.PurType = PurCopyType.PurOrder;
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

        [HttpGet]
        public async Task<IActionResult> FindPurchaseOrder(int seriesID, string number)
        {
            var data = await _ipur.FindPurchaseOrderAsync(seriesID, number, GetCompany().ID);
            if (data.PurchaseOrder == null)
            {
                return Ok(new { Error = true, Message = $"Cannot find invoice \"{number}\"" });
            }
            return Ok(data);
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

        [HttpGet]
        public IActionResult GetPurchaseOrderReport(int VendorID, int BranchID, int WarehouseID, string PostingDate, string DocumentDate, bool check)
        {

            List<PurchaseOrder> ServiceCalls = new();
            //filter WareHouse
            if (WarehouseID != 0 && VendorID == 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseOrders.Where(w => w.WarehouseID == WarehouseID).ToList();
            }
            // filter Vendor 
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseOrders.Where(w => w.VendorID == VendorID).ToList();
            }
            //filter WareHouse and VendorName
            else if (WarehouseID != 0 && VendorID != 0 && PostingDate == null && DocumentDate == null && check == false)
            {
                ServiceCalls = _context.PurchaseOrders.Where(w => w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            //filter all item
            else if (BranchID != 0 && WarehouseID == 0 && VendorID == 0 && PostingDate == null && DocumentDate == null)
            {
                ServiceCalls = _context.PurchaseOrders.Where(w => w.UserID == BranchID).ToList();
            }
            //filter warehouse, vendor, datefrom ,dateto
            else if (WarehouseID != 0 & VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseOrders.Where(w => w.VendorID == VendorID && w.WarehouseID == WarehouseID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter vendor and Datefrom and Dateto
            else if (WarehouseID == 0 && VendorID != 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseOrders.Where(w => w.VendorID == VendorID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter warehouse and Datefrom and DateTo
            else if (WarehouseID != 0 && VendorID == 0 && PostingDate != null && DocumentDate != null && check == false)
            {
                ServiceCalls = _context.PurchaseOrders.Where(w => w.WarehouseID == WarehouseID && w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            //filter Datefrom and DateTo
            else if (WarehouseID == 0 && VendorID == 0 && PostingDate != null && DocumentDate != null)
            {
                ServiceCalls = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(PostingDate) && w.DocumentDate <= Convert.ToDateTime(DocumentDate)).ToList();
            }
            else
            {
                return Ok(new List<PurchaseOrder>());
            }
            var list = (from s in ServiceCalls
                        join cus in _context.BusinessPartners on s.VendorID equals cus.ID
                        join item in _context.UserAccounts on s.UserID equals item.ID
                        select new ReportPurchaseOrder
                        {
                            ID = s.PurchaseOrderID,
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
        public IActionResult GetPurchaseOrderByWarehouse(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = (from s in _order.ReportPurchaseOrders(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check)
                        select new ReportPurchaseOrder
                        {
                            ID = s.ID,
                            InvoiceNo = s.InvoiceNo,
                            BusinessName = s.BusinessName,
                            Warehouse = s.Warehouse,
                            UserName = s.UserName,
                            //LocalCurrency =s.LocalCurrency,
                            //SystemCurrency = s.SystemCurrency,
                            Balance_due = s.Balance_due,
                            Balance_due_sys = s.Balance_due_sys,
                            ExchangeRate = s.ExchangeRate,
                            Status = s.Status,
                            Cencel = s.Cencel,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),
                            //Cancele = "<i class= 'fa fa-ban'style='color:red;' ></i>",
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseOrderByPostingDate(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = (from s in _order.ReportPurchaseOrders(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check)
                        select new ReportPurchaseOrder
                        {
                            ID = s.ID,
                            InvoiceNo = s.InvoiceNo,
                            BusinessName = s.BusinessName,
                            Warehouse = s.Warehouse,
                            UserName = s.UserName,
                            //LocalCurrency =s.LocalCurrency,
                            //SystemCurrency = s.SystemCurrency,
                            Balance_due = s.Balance_due,
                            Balance_due_sys = s.Balance_due_sys,
                            ExchangeRate = s.ExchangeRate,
                            Cencel = s.Cencel,
                            Status = s.Status,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),
                            //Cancele = "<i class= 'fa fa-ban'style='color:red;' ></i>",
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseOrderDocumentDate(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = (from s in _order.ReportPurchaseOrders(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check)
                        select new ReportPurchaseOrder
                        {
                            ID = s.ID,
                            InvoiceNo = s.InvoiceNo,
                            BusinessName = s.BusinessName,
                            Warehouse = s.Warehouse,
                            UserName = s.UserName,
                            //LocalCurrency =s.LocalCurrency,
                            //SystemCurrency = s.SystemCurrency,
                            Balance_due = s.Balance_due,
                            Balance_due_sys = s.Balance_due_sys,
                            ExchangeRate = s.ExchangeRate,
                            Status = s.Status,
                            Cencel = s.Cencel,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseOrderDeliveryDatedDate(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = (from s in _order.ReportPurchaseOrders(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check)
                        select new ReportPurchaseOrder
                        {
                            ID = s.ID,
                            InvoiceNo = s.InvoiceNo,
                            BusinessName = s.BusinessName,
                            Warehouse = s.Warehouse,
                            UserName = s.UserName,
                            //LocalCurrency =s.LocalCurrency,
                            //SystemCurrency = s.SystemCurrency,
                            Balance_due = s.Balance_due,
                            Balance_due_sys = s.Balance_due_sys,
                            ExchangeRate = s.ExchangeRate,
                            Status = s.Status,
                            Cencel = s.Cencel,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetPurchaseOrderAllItem(int BranchID, int WarehouseID, string PostingDate, string DocumentDate, string DeliveryDate, string Check)
        {
            var list = (from s in _order.ReportPurchaseOrders(BranchID, WarehouseID, PostingDate, DocumentDate, DeliveryDate, Check)
                        select new ReportPurchaseOrder
                        {
                            ID = s.ID,
                            InvoiceNo = s.InvoiceNo,
                            BusinessName = s.BusinessName,
                            Warehouse = s.Warehouse,
                            UserName = s.UserName,
                            //LocalCurrency =s.LocalCurrency,
                            //SystemCurrency = s.SystemCurrency,
                            Balance_due = s.Balance_due,
                            Balance_due_sys = s.Balance_due_sys,
                            ExchangeRate = s.ExchangeRate,
                            Status = s.Status,
                            Cencel = s.Cencel,
                            VatType = Typevatt.Select(s => new SelectListItem
                            {
                                Value = s.Key.ToString(),
                                Text = s.Value
                            }).ToList(),
                        }).ToList();
            return Ok(list);
        }
        [HttpGet]
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
        [HttpGet]
        public IActionResult GetPurchaseRequest()
        {
            var list = _order.GetAllPurchaseRequest().ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult FindPurchaseRequest(int ID, string Invoice)
        {
            var list = _order.GetPurchaseOrder_From_PurchaseRequest(ID, Invoice).ToList();
            return Ok(list);
        }
        //02.02.2020
        public IActionResult POCancel(int PurchaseID)
        {
            _order.POCancel(PurchaseID);
            return Ok();
        }

        public IActionResult GetPurchaseQuotationCopy(int id, int type)
        {
            if (type == 1)
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
                                pr.Remark,
                                Type = 1,
                            }).ToList();
                return Ok(list);
            }
            if (type == 2)
            {
                var list = (from pr in _context.PurchaseQuotations.Where(s => s.VendorID == id && s.Status == "open")
                            join dct in _context.DocumentTypes on pr.DocumentTypeID equals dct.ID
                            join currency in _context.Currency on pr.PurCurrencyID equals currency.ID
                            select new
                            {
                                pr.ID,
                                pr.SeriesID,
                                DocType = dct.Code,
                                Invoice = pr.Number,
                                PostingDate = pr.PostingDate.ToString("MM-dd-yyyy"),
                                CurrencyName = currency.Description,
                                pr.SubTotal,
                                pr.BalanceDue,
                                pr.Remark,
                                Type = 2,
                            }).ToList();
                return Ok(list);
            }
            return Ok();
        }

    }
}
