using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CKBS.AppContext;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Banking;
using KEDI.Core.Models.Validation;
using CKBS.Models.ServicesClass;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Administrator.General;
using System.Diagnostics;
using KEDI.Core.Premise.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.ServicesClass.Sale;
using KEDI.Core.Localization.Resources;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using System.Threading.Tasks;
using KEDI.Core.Premise.Authorization;
using CKBS.Models.Services.Inventory;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Helpers.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using IoFile = System.IO.File;
using CKBS.Models.ServicesClass.GoodsIssue;
using KEDI.Core.Premise.Models.ServicesClass.TaxGroup;
using KEDI.Core.Premise.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Services.ServiceContractTemplate;
using KEDI.Core.Premise.Models.ServicesClass.Purchase;
using KEDI.Core.System.Models;

namespace KEDI.Core.Premise.Controllers
{
    //[Authorize(Policy = "Permission")]
    [Privilege]
    public class SaleController : Controller
    {
        private readonly DataContext _context;
        private readonly ISale _isale;
        private readonly ICopyFromDiliveryToAR _isalecopy;
        private readonly IReturnOrCancelStockMaterial _returnOrCancelStockMaterial;


        private readonly CultureLocalizer _culLocal;
        private readonly ISaleSerialBatchRepository _saleSerialBatch;
        private readonly UtilityModule _utility;
        private readonly IDataPropertyRepository _dataProp;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager _userModule;

        public SaleController(
            DataContext context,
            UserManager userModule,
            ISale sale,
            IReturnOrCancelStockMaterial returnOrCancelStockMaterial,
            CultureLocalizer cultureLocalizer,
            ISaleSerialBatchRepository saleSerialBatch, UtilityModule utility,
            IWebHostEnvironment env, IDataPropertyRepository dataProperty, ICopyFromDiliveryToAR isalecopy)
        {
            _context = context;
            _isale = sale;
            _returnOrCancelStockMaterial = returnOrCancelStockMaterial;
            _culLocal = cultureLocalizer;
            _saleSerialBatch = saleSerialBatch;
            _utility = utility;
            _dataProp = dataProperty;
            _env = env;
            _isalecopy = isalecopy;
            _userModule = userModule;
        }
        public IActionResult GetFreights()
        {
            var freights = _isale.GetFreights();
            return Ok(freights);
        }
        private void ValidateSummary(dynamic master, IEnumerable<dynamic> details)
        {
            if (master.WarehouseID == 0)
            {
                ModelState.AddModelError("WarehouseID", _culLocal["Warehouse need to be selected."]);
            }
            if (master.BranchID == 0)
            {
                ModelState.AddModelError("BranchID", "Branch not matched with warehouse.");
            }
            if (master.CusID == 0)
            {
                ModelState.AddModelError("CusID", "Please choose any customer.");
            }

            if (!details.Any())
            {
                ModelState.AddModelError("Details", "Please choose at least one detail item.");
            }
            //if (master.SaleEmID == 0)
            //{
            //    ModelState.AddModelError("SaleEMID", "please input Sale Employee");
            //}
            double subtotal = 0;
            foreach (var dt in details.Where(x => !x.Delete))
            {
                subtotal += dt.Total;
                dt.OpenQty = dt.Qty;
                if (dt.Qty <= 0)
                {
                    ModelState.AddModelError("Details", "Required item detail quantity greater than 0.");
                }
                dt.TotalSys = dt.Total * (double)master.ExchangeRate;
                dt.TotalWTaxSys = dt.TotalWTax * master.ExchangeRate;
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
            }
            var doctype = _context.DocumentTypes.Find(master.DocTypeID) ?? new DocumentType();
            master.SubTotal = subtotal;
            master.SubTotalSys = subtotal * (double)master.ExchangeRate;
            master.TotalAmountSys = master.TotalAmount * (double)master.ExchangeRate;
            if (doctype.Code != "CD" && doctype.Code != null) master.FreightAmountSys = master.FreightAmount * (decimal)master.ExchangeRate;
            master.SubTotalBefDisSys = master.SubTotalBefDis * (decimal)master.ExchangeRate;
            master.SubTotalAfterDisSys = master.SubTotalAfterDis * (decimal)master.ExchangeRate;
        }

        private void CheckTaxAcc(IEnumerable<dynamic> details)
        {
            foreach (var dt in details)
            {
                if (dt.TaxGroupID > 0)
                {
                    var taxg = _context.TaxGroups.Find(dt.TaxGroupID) ?? new TaxGroup();
                    if (taxg.GLID <= 0)
                    {
                        ModelState.AddModelError("GLAcc", $"Item's code {dt.ItemCode} connected with tax {taxg.Code}-{taxg.Name} does not have account for tax!");
                    }
                }
            }
        }
        [HttpPost]
        public IActionResult GetData()
        {
            return Ok(_context.SaleQuotes);
        }

        private void UpdateSourceCopy(IEnumerable<dynamic> details, dynamic copyMaster, IEnumerable<dynamic> copyDedails, SaleCopyType copyType, bool memo = false)
        {
            bool canClose = true;
            foreach (var cd in copyDedails)
            {
                foreach (var d in details)
                {
                    switch (copyType)
                    {
                        case SaleCopyType.Quotation:
                            if (d.SQDID == cd.SQDID)
                            {
                                if (cd.OpenQty > 0)
                                {
                                    cd.OpenQty -= d.Qty;
                                }

                                if (cd.OpenQty <= 0)
                                {
                                    cd.Delete = true;
                                }

                            }
                            break;
                        case SaleCopyType.Order:
                            if (d.SODID == cd.SODID)
                            {
                                if (cd.OpenQty > 0)
                                {
                                    cd.OpenQty -= d.OpenQty;
                                }

                                if (cd.OpenQty <= 0)
                                {
                                    cd.Delete = true;

                                }
                            }
                            break;
                        case SaleCopyType.Delivery:
                            if (d.SDDID == cd.SDDID)
                            {
                                if (cd.OpenQty > 0)
                                {
                                    cd.OpenQty -= d.OpenQty;
                                }

                                if (cd.OpenQty <= 0)
                                {
                                    cd.Delete = true;
                                }

                                int aredit = cd.SAREDTDID;
                                if (cd.SAREDTDID != 0)
                                {
                                    var arReserEdit = _context.ARReserveInvoiceEditableDetails.FirstOrDefault(s => s.ID == aredit);
                                    arReserEdit.OpenQty += d.OpenQty;
                                    arReserEdit.DeliveryQty -= d.OpenQty;
                                }
                            }
                            break;
                        case SaleCopyType.AR:
                            if (d.SARDID == cd.SARDID)
                            {
                                if (cd.OpenQty > 0)
                                {
                                    cd.OpenQty -= d.Qty;

                                }

                                //if (cd.OpenQty <= 0)
                                //{
                                //    cd.Delete = true;
                                //}
                            }
                            break;
                        case SaleCopyType.ARReserveInvoice:
                            if (d.LineID == cd.LineID)
                            {
                                if (cd.OpenQty > 0)
                                {
                                    cd.OpenQty -= d.OpenQty;
                                }
                            }
                            break;
                        case SaleCopyType.ARReserveInvoiceEDT:
                            if (d.ARReDetEDTID == cd.ID)
                            {
                                if (memo)
                                {
                                    if (cd.PrintQty > 0)
                                    {
                                        cd.PrintQty -= d.Qty;
                                    }
                                }
                                else
                                {
                                    if (cd.OpenQty > 0)
                                    {
                                        cd.OpenQty -= d.Qty;
                                    }
                                }
                            }
                            break;
                        case SaleCopyType.SaleAREdite:
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

            foreach (var cd in copyDedails)
            {
                canClose = memo == true ? canClose && cd.PrintQty <= 0 : canClose && cd.OpenQty <= 0;
            }

            if (canClose)
            {
                copyMaster.Status = "close";
            }
            _context.SaveChanges();
        }

        private void UpdateSourceCopyDelivery(IEnumerable<dynamic> details, dynamic copyMaster, IEnumerable<dynamic> copyDedails, SaleCopyType copyType)
        {
            bool canClose = true;
            foreach (var cd in copyDedails)
            {
                foreach (var d in details)
                {
                    switch (copyType)
                    {
                        case SaleCopyType.Delivery:
                            if (d.SDDID == cd.SDDID)
                            {
                                if (cd.OpenQty > 0)
                                {
                                    cd.OpenQty -= d.Qty;
                                }

                                if (cd.OpenQty <= 0)
                                {
                                    cd.Delete = true;
                                }
                            }
                            break;
                    }

                }
            }

            foreach (var cd in copyDedails)
            {
                canClose = canClose && cd.OpenQty <= 0;
            }

            if (canClose)
            {
                copyMaster.Status = "close";
            }
            _context.SaveChanges();
        }

        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst("UserID").Value, out int _id);
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

        public IActionResult ARDownPaymentINCN(int curId, string status, int arid = 0)
        {
            var data = _isale.ARDownPaymentINCN(curId, status, arid);
            return Ok(data);
        }
        public IActionResult GetDisplayFormatCurrency(int curId)
        {
            var data = _utility.GetGeneralSettingAdmin(curId);
            return Ok(data);
        }
        //--------------------------------------------// start Sale SQ //------------------------------------//
        #region Sale SQ

        [Privilege("SQ")]
        public IActionResult SaleQuote()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Quotation";
            ViewBag.Sale = "show";
            ViewBag.SaleQuotation = "highlight";
            return View(new { seriesSQ = _utility.GetSeries("SQ"), genSetting = _utility.GetGeneralSettingAdmin() });
        }

        [HttpGet]
        public IActionResult GetCustomer(int? id)
        {
            if (id != null)
            {

                return Ok(_context.BusinessPartners.FirstOrDefault(c => c.ID == id));
            }
            else
            {
                //var cus = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Customer").ToList();
                var cus = (from bp in _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Customer")
                           let pay = _context.PaymentTerms.Where(x => x.ID == bp.PaymentTermsID).FirstOrDefault() ?? new PaymentTerms()
                           //let ins =_context.Instaillments.Where(x=>x.ID==pay.InstaillmentID).FirstOrDefault() ?? new Instaillment()
                           select new BusinessPartner
                           {
                               ID = bp.ID,
                               PaymentTermsID = pay.ID,
                               Code = bp.Code,
                               Name = bp.Name,
                               Type = bp.Type,
                               Phone = bp.Phone,
                               Months = pay.Months,
                               Days = pay.Days,
                               ContactPeople = (from con in _context.ContactPersons.Where(x => x.BusinessPartnerID == bp.ID)
                                                select new ContactPerson
                                                {
                                                    ID = con.ID,
                                                    ContactID = con.ContactID,
                                                    FirstName = con.FirstName,
                                                    LastName = con.LastName,
                                                    Tel1 = con.Tel1,
                                                    SetAsDefualt = con.SetAsDefualt
                                                }).ToList() ?? new List<ContactPerson>()

                           }).ToList();

                return Ok(cus);
            }
        }

        [HttpGet]
        public IActionResult GetGLAccount()
        {
            var data = _context.GLAccounts.Where(x => x.IsCashAccount == true).ToList();
            return Ok(data);
        }
        public IActionResult GetEmp()
        {
            var data = _context.Employees.Where(x => !x.Delete).ToList();
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetWarehouse()
        {
            var ware = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == int.Parse(User.FindFirst("BranchID").Value)).ToList();
            return Ok(ware);
        }

        [HttpGet]
        public IActionResult GetDefaultCurrency()
        {
            var currency = GetSystemCurrencies().First();
            return Ok(currency);
        }

        private IEnumerable<SystemCurrency> GetSystemCurrencies()
        {
            IEnumerable<SystemCurrency> currencies =
                                        (from com in _context.Company.Where(x => x.Delete == false)
                                         join c in _context.Currency.Where(x => x.Delete == false) on com.SystemCurrencyID equals c.ID
                                         select new SystemCurrency
                                         {
                                             ID = c.ID,
                                             Description = c.Description
                                         });
            return currencies;
        }

        [HttpGet]
        public IActionResult GetPriceList()
        {
            var pri = _context.PriceLists.Include(w => w.Currency).Where(W => !W.Delete);
            return Ok(pri);
        }

        [HttpGet]
        public IActionResult FindCustomer(string name = "")
        {
            var cus = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Customer").ToList();
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Replace(" ", string.Empty).ToLower();
                cus = cus.Where(ap =>
                    ap.Code.Replace(" ", string.Empty).ToLower().Contains(name)
                   | ap.Name.Replace(" ", string.Empty).ToLower().Contains(name)
                   | ap.Phone.Replace(" ", string.Empty).ToLower().Contains(name)
                ).ToList();
            }
            return Ok(cus);
        }
        [HttpGet]
        public IActionResult GetExchange()
        {
            var ex = from e in _context.ExchangeRates.Where(x => x.Delete == false)
                     join c in _context.Currency.Where(x => x.Delete == false) on e.CurrencyID equals c.ID
                     select new TpExchange
                     {
                         ID = e.ID,
                         CurID = e.CurrencyID,
                         CurName = c.Description,
                         Rate = e.Rate,
                         SetRate = e.SetRate
                     };
            return Ok(ex);
        }
        [HttpGet]
        public IActionResult GetItem(int PriLi, int wareId)
        {
            var ls_item = _isale.GetItemMaster(PriLi, wareId, GetCompany().ID).ToList();
            return Ok(ls_item);
        }
        public IActionResult GetItemDetails(int PriLi, int itemId = 0, string barCode = "", int uomId = 0)
        {
            var ls_item = _isale.GetItemDetails(PriLi, GetCompany().ID, itemId, barCode, uomId).ToList();
            return ls_item.Count == 0 && barCode != ""
                ? Ok(new { IsError = true, Error = $"No item with this Barcode \"{barCode}\"" })
                : Ok(ls_item);
        }
        public IActionResult GetItemDetailsForSaleCM(
            int PriLi, int itemId = 0, string barCode = "", int uomId = 0, int wareId = 0, string process = ""
            )
        {
            var ls_item = _isale.GetItemDetailsForSaleCM(PriLi, GetCompany().ID, itemId, barCode, uomId, wareId, process);
            if (ls_item == null && !string.IsNullOrEmpty(barCode))
            {
                return Ok(new { IsError = true, Error = $"No item with this Barcode \"{barCode}\"" });
            }
            //if (ls_item != null && ls_item.Process != "Standard" && ls_item.Stock <= 0)
            //{
            //    return Ok(new { IsError = true, Error = $"item has no stock!" });
            //}
            if (ls_item == null)
            {
                return Ok(new { IsError = true, Error = $"Could not get item detail" });
            }
            return Ok(ls_item);
        }
        [HttpGet]
        public IActionResult GetSaleCur(int PriLi)
        {
            var prili = _context.PriceLists.FirstOrDefault(w => w.ID == PriLi);
            return Ok(prili);
        }
        [HttpGet]
        public IActionResult GetGDUom()
        {
            var g = _isale.GetAllGroupDefind().ToList();
            return Ok(g);
        }
        [HttpPost]
        public IActionResult UpdateSaleQuote(string data)
        {
            SaleQuote saleQuote = JsonConvert.DeserializeObject<SaleQuote>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            saleQuote.ChangeLog = DateTime.Now;
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            var seriesSQ = _context.Series.FirstOrDefault(w => w.ID == saleQuote.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == saleQuote.DocTypeID).FirstOrDefault();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;

            saleQuote.UserID = GetUserID();
          

            var g = _isale.GetAllGroupDefind().ToList();

            if (saleQuote.ValidUntilDate < saleQuote.PostingDate)
            {
                ModelState.AddModelError("UntilDate", "Item has invalid until date.");
            }
            if (!string.IsNullOrEmpty(saleQuote.RefNo))
            {
                bool isRefExisted = _context.SaleQuotes.AsNoTracking().Any(i => i.RefNo == saleQuote.RefNo);
                if (isRefExisted)
                {
                    ModelState.AddModelError("RefNo", $"Transaction with Customer Ref No. \"{saleQuote.RefNo}\" already done.");
                }
            }
            ValidateSummary(saleQuote, saleQuote.SaleQuoteDetails);
            CheckTaxAcc(saleQuote.SaleQuoteDetails);
            foreach (var d in saleQuote.SaleQuoteDetails)
            {
                var factor = g.FirstOrDefault(w => w.GroupUoMID == d.GUomID && w.AltUOM == d.UomID).Factor;
                d.Factor = factor;
                if (saleQuote.SQID == 0)
                {
                    d.OpenQty = d.Qty;
                }
            }

            using (var t = _context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    if (saleQuote.SQID > 0)
                    {
                        _context.SaleQuotes.Update(saleQuote);
                    }
                    else
                    {
                        seriesDetail.Number = seriesSQ.NextNo;
                        seriesDetail.SeriesID = saleQuote.SeriesID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesSQ.NextNo;
                        long No = long.Parse(Sno);
                        saleQuote.InvoiceNumber = seriesSQ.NextNo;
                        seriesSQ.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                        if (long.Parse(seriesSQ.NextNo) > long.Parse(seriesSQ.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }
                        saleQuote.LocalSetRate = localSetRate;
                        saleQuote.LocalCurID = GetCompany().LocalCurrencyID;
                        saleQuote.LocalSetRate = localSetRate;
                        saleQuote.SeriesDID = seriesDetailID;
                        saleQuote.CompanyID = GetCompany().ID;

                        _context.SaleQuotes.Update(saleQuote);
                    }
                    _context.SaveChanges();
                    var freight = saleQuote.FreightSalesView;
                    if (freight != null)
                    {
                        //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                        freight.SaleID = saleQuote.SQID;
                        freight.SaleType = SaleCopyType.Quotation;
                        freight.OpenAmountReven = freight.AmountReven;
                        _context.FreightSales.Update(freight);
                        _context.SaveChanges();
                    }
                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }
            return Ok(msg.Bind(ModelState));
        }

        [HttpGet]
        public IActionResult FindSaleQuote(string number, int seriesID)
        {
            var list = _isale.FindSaleQuote(number, seriesID, GetCompany().ID);
            if (list.SaleQuote != null)
            {
                return Ok(list);
            }
            return Ok();
        }

        #endregion
        //--------------------------------------------// End Sale SQ // -------------------------

        //--------------------------------------------// start Sale Order //------------------------------------//
        #region Sale Order
        [Privilege("SO")]
        public IActionResult SaleOrder()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Order";
            ViewBag.Sale = "show";
            ViewBag.SaleOrder = "highlight";
            return View(new { seriesSO = _utility.GetSeries("SO"), genSetting = _utility.GetGeneralSettingAdmin() });
        }
        public IActionResult ItemMasterDatas()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Order";
            ViewBag.Sale = "show";
            ViewBag.SaleOrder = "highlight";
            return View(new { seriesSO = _utility.GetSeries("SO"), genSetting = _utility.GetGeneralSettingAdmin() });
        }
        public IActionResult GetSaleQuotes(int cusId)
        {
            var allItems = _isale.GetSaleQuotesCopy(cusId);
            return Ok(allItems);
        }
        public IActionResult GetSaleQuoteDetailCopy(string number, int seriesId)
        {
            var data = _isale.GetSaleQuoteDetailCopy(number, seriesId, GetCompany().ID);
            return Ok(data);
        }
        public IActionResult GetItemMasterDataDetailCopy(string number, int seriesId)
        {
            var data = _isale.GetItemMasterDataDetailCopy(number, seriesId, GetCompany().ID);
            return Ok(data);
        }
        public IActionResult GetSaleOrders(int cusId)
        {
            var allItems = _isale.GetSaleOrdersCopy(cusId);
            return Ok(allItems);
        }
        public IActionResult GetSaleOrderDetailCopy(string number, int seriesId)
        {
            var data = _isale.GetSaleOrderDetailCopy(number, seriesId, GetCompany().ID);
            return Ok(data);
        }
        public IActionResult GetSaleDeliveries(int cusId)
        {
            var allItems = _isale.GetSaleDeliverysCopy(cusId);
            return Ok(allItems);
        }
        public IActionResult GetSaleDeliveryDetailCopy(string number, int seriesId)
        {
            var data = _isale.GetSaleDeliveryDetailCopy(number, seriesId, GetCompany().ID);
            return Ok(data);
        }
        public IActionResult GetSaleARs(int cusId)
        {
            var allItems = _isale.GetSaleARsCopy(cusId);

            return Ok(allItems);
        }
        public IActionResult GetSaleARsDetailCopy(string number, int seriesId)
        {
            var data = _isale.GetSaleARsDetailCopy(number, seriesId, GetCompany().ID);
            return Ok(data);
        }
        //=========AR RESERVE INVOICE============
        public IActionResult GetSaleAREdit(int cusId)
        {
            var allItems = _isale.GetSaleAREditCopy(cusId);
            return Ok(allItems);
        }
        public IActionResult GetSaleAREditDetailCopy(string number, int seriesId)
        {
            var data = _isale.GetSaleAREditDetailCopy(number, seriesId, GetCompany().ID);
            return Ok(data);
        }
        [HttpGet]
        public IActionResult GetSaleAPEditReport(int branchID, int warehouseID, string postingDate, string documentDate, string deliveryDate, string search, bool check)
        {
            var list = _isale.GetSaleAPEdits(branchID, warehouseID, postingDate, documentDate, deliveryDate, search, check);
            return Ok(list);
        }
        //=========AREdit RESERVE INVOICE============
        public IActionResult GetARReserveInvoice(int cusId)
        {
            var allItems = _isale.GetARReserveInvoiceCopy(cusId);
            return Ok(allItems);
        }

        public IActionResult GetARReserveInvoiceDetailDetailCopy(string number, int seriesId)
        {
            var data = _isale.GetARReserveInvoiceDetailDetailCopy(number, seriesId, GetCompany().ID);
            return Ok(data);
        }
        public IActionResult GetARReserveInvoiceEDT(int cusId)
        {
            var allItems = _isale.GetARReserveInvoicEDTeCopy(cusId);
            return Ok(allItems);
        }
        public IActionResult GetARReserveInvoiceEDTDetailCopy(string number, int seriesId)
        {
            var data = _isale.GetARReserveInvoiceEDTDetailCopy(number, seriesId, GetCompany().ID);
            return Ok(data);
        }
        public IActionResult GetARReserveInvoiceEDTDetailCopyToMemo(string number, int seriesId)
        {
            var data = _isale.GetARReserveInvoiceEDTDetailCopy(number, seriesId, GetCompany().ID, true);
            return Ok(data);
        }
        //=========END AR RESERVE INVOICE============
        public IActionResult RemoveQuoteDetail(int detailID)
        {
            ModelMessage msg = new();
            var details = _context.SaleQuoteDetails.Where(d => d.SQDID == detailID);
            if (ModelState.IsValid)
            {
                _context.SaleQuoteDetails.Find(detailID).Delete = true;
                _context.SaveChanges();
                msg.Approve();
            }

            return Ok(msg.Bind(ModelState));
        }

        [HttpPost]
        public IActionResult UpdateSaleOrder(string data)
        {
            SaleOrder saleOrder = JsonConvert.DeserializeObject<SaleOrder>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            saleOrder.ChangeLog = DateTime.Now;
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            var seriesSO = _context.Series.FirstOrDefault(w => w.ID == saleOrder.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == saleOrder.DocTypeID).FirstOrDefault();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;

            saleOrder.UserID = GetUserID();
          
            if (saleOrder.DeliveryDate < saleOrder.PostingDate)
            {
                ModelState.AddModelError("DeliveryDate", "Item has invalid delivery date.");
            }
            if (!string.IsNullOrEmpty(saleOrder.RefNo))
            {
                bool isRefExisted = _context.SaleOrders.AsNoTracking().Any(i => i.RefNo == saleOrder.RefNo);
                if (isRefExisted)
                {
                    ModelState.AddModelError("RefNo", $"Transaction with Customer Ref No. \"{saleOrder.RefNo}\" already done.");
                }
            }
            ValidateSummary(saleOrder, saleOrder.SaleOrderDetails);
            CheckTaxAcc(saleOrder.SaleOrderDetails);
            using (var t = _context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    if (saleOrder.SOID > 0)
                    {
                        _context.SaleOrders.Update(saleOrder);
                    }
                    else
                    {
                        if (saleOrder.CopyType == SaleCopyType.Quotation)
                        {
                            var saleQuoteInvoice = saleOrder.CopyKey.Split("-")[1];
                            var quoteMaster = _context.SaleQuotes.Include(q => q.SaleQuoteDetails).
                                FirstOrDefault(m => string.Compare(m.InvoiceNumber, saleQuoteInvoice) == 0);
                            if (quoteMaster != null)
                            {
                                UpdateSourceCopy(saleOrder.SaleOrderDetails, quoteMaster, quoteMaster.SaleQuoteDetails, SaleCopyType.Quotation);
                            }
                        }
                        if (saleOrder.SOID == 0 && saleOrder.CopyType == 0)
                        {
                            saleOrder.LocalSetRate = localSetRate;
                        }
                        foreach (var item in saleOrder.SaleOrderDetails)
                        {
                            var warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == saleOrder.WarehouseID && w.ItemID == item.ItemID);
                            if (warehouse != null)
                            {
                                warehouse.Committed += item.Qty * item.Factor;
                                _context.WarehouseSummary.Update(warehouse);
                            }
                        }

                        seriesDetail.Number = seriesSO.NextNo;
                        seriesDetail.SeriesID = saleOrder.SeriesID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();

                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesSO.NextNo;
                        long No = long.Parse(Sno);
                        saleOrder.InvoiceNumber = seriesSO.NextNo;
                        seriesSO.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                        if (long.Parse(seriesSO.NextNo) > long.Parse(seriesSO.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }

                        saleOrder.LocalCurID = GetCompany().LocalCurrencyID;
                        saleOrder.LocalSetRate = localSetRate;
                        saleOrder.SeriesDID = seriesDetailID;
                        saleOrder.CompanyID = GetCompany().ID;
                        _context.SaleOrders.Update(saleOrder);
                        _context.SaveChanges();
                    }


                    var freight = saleOrder.FreightSalesView;
                    if (freight != null)
                    {
                        //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                        freight.SaleID = saleOrder.SOID;
                        freight.SaleType = SaleCopyType.Order;
                        freight.OpenAmountReven = freight.AmountReven;
                        _context.FreightSales.Update(freight);
                        _context.SaveChanges();
                    }
                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }

            return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
        }

        private static string GetTransactType(string invoiceNo, bool hasVAT)
        {
            string[] transactTypes = invoiceNo.Split("-");
            string transactType = transactTypes[0];
            if (hasVAT)
            {
                transactType = string.Format("{0}-{1}", transactTypes[0], transactTypes[1]);
            }
            return transactType;
        }
        private IEnumerable<ItemsReturn> CheckStock<T, TD>(T dataMaster, List<TD> data, string idProp)
        {
            List<ItemsReturn> list = new();
            List<ItemsReturn> list_group = new();
            //data = data.Where(w => (int)GetValue(w, idProp) == 0).ToList();
            int warehouseID = (int)GetValue(dataMaster, "WarehouseID");
            foreach (var item in data)
            {
                int itemID = (int)GetValue(item, "ItemID");
                int uomID = (int)GetValue(item, "UomID");
                double printQty = (double)GetValue(item, "PrintQty");
                List<SaleAREditeDetail> saleareditdetail = new();
                double editeQty = (double)GetValue(item, "EditeQty");
                var check = list_group.Find(w => w.ItemID == itemID);
                var item_group_uom = _context.ItemMasterDatas.Include(gu => gu.GroupUOM).Include(uom => uom.UnitofMeasureInv).FirstOrDefault(w => w.ID == itemID);
                var uom_defined = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item_group_uom.GroupUomID && w.AltUOM == uomID);
                var bom = _context.BOMaterial.FirstOrDefault(w => w.ItemID == itemID && w.Active == true);
                var item_material = (from bomd in _context.BOMDetail.Where(w => w.BID == bom.BID && w.NegativeStock == false && w.Detele == false)
                                     join i in _context.ItemMasterDatas on bomd.ItemID equals i.ID
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
                                         i.ManItemBy
                                     }).Where(w => w.GroupUoMID == w.GUoMID);
                if (check == null)
                {
                    var item_warehouse = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == warehouseID && w.ItemID == itemID);
                    if (item_warehouse != null)
                    {
                        ItemsReturn item_group = new()
                        {
                            Code = GetValue(item, "ItemCode").ToString(),
                            ItemID = itemID,
                            KhmerName = item_group_uom.KhmerName + ' ' + item_group_uom.UnitofMeasureInv.Name,
                            InStock = (decimal)item_warehouse.InStock,
                            OrderQty = (decimal)(editeQty * uom_defined.Factor),
                            Committed = (decimal)item_warehouse.Committed,
                            IsSerailBatch = item_group_uom.ManItemBy == ManageItemBy.Batches || item_group_uom.ManItemBy == ManageItemBy.SerialNumbers
                        };
                        list_group.Add(item_group);
                    }
                    else if (item_group_uom.Process != "Standard" && item_warehouse == null)
                    {
                        ItemsReturn item_group = new()
                        {
                            Code = GetValue(item, "ItemCode").ToString(),
                            ItemID = itemID,
                            KhmerName = item_group_uom.KhmerName + ' ' + item_group_uom.UnitofMeasureInv.Name,
                            InStock = 0,
                            OrderQty = (decimal)(editeQty * uom_defined.Factor),
                            Committed = 0,
                            IsSerailBatch = item_group_uom.ManItemBy == ManageItemBy.Batches || item_group_uom.ManItemBy == ManageItemBy.SerialNumbers
                        };
                        list_group.Add(item_group);
                    }
                    if (bom != null)
                    {
                        foreach (var items in item_material.ToList())
                        {
                            var item_warehouse_material = _context.WarehouseSummary.FirstOrDefault(w => w.WarehouseID == warehouseID && w.ItemID == items.ItemID);
                            if (item_warehouse_material != null)
                            {
                                ItemsReturn item_group = new()
                                {
                                    Code = items.Code,
                                    ItemID = items.ItemID,
                                    KhmerName = items.KhmerName + ' ' + items.Uom,
                                    InStock = (decimal)(item_warehouse_material.InStock - item_warehouse_material.Committed),
                                    OrderQty = (decimal)(editeQty * items.Factor * items.Qty * items.Factor),
                                    Committed = (decimal)item_warehouse_material.Committed
                                };
                                list_group.Add(item_group);
                            }
                            else if (items.Process != "Standard" && item_warehouse_material == null)
                            {
                                ItemsReturn item_group = new()
                                {
                                    Code = items.Code,
                                    ItemID = items.ItemID,
                                    KhmerName = items.KhmerName + ' ' + items.Uom,
                                    InStock = 0,
                                    OrderQty = (decimal)(editeQty * items.Factor * items.Qty * items.Factor),
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
                    check.OrderQty += (decimal)(editeQty * uom_defined.Factor);
                }
            }

            foreach (var item in list_group)
            {
                if (item.OrderQty > item.InStock && !item.IsSerailBatch)
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
                        IsSerailBatch = item.IsSerailBatch
                    };
                    list.Add(item_return);
                }
                else if (item.IsSerailBatch)
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
                        IsSerailBatch = item.IsSerailBatch
                    };
                    list.Add(item_return);
                }
            }
            return list;
        }
        [HttpGet]
        public IActionResult FindSaleOrder(string number, int seriesID)
        {
            var list = _isale.FindSaleOrder(number, seriesID, GetCompany().ID);
            if (list.SaleOrder != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        [HttpGet]
        public IActionResult FindItemMasterData(string number, int seriesID)
        {
            var list = _isale.FindItemMasterData(number, seriesID, GetCompany().ID);
            if (list.SaleAR != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        #endregion
        //--------------------------------------------// End Sale Order //------------------------------------//

        //--------------------------------------------// start Sale Delivery //------------------------------------//
        #region Sale Delivery
        [Privilege("SD")]
        public IActionResult SaleDelivery()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale Delivery";
            ViewBag.Sale = "show";
            ViewBag.SaleDelivery = "highlight";
            var seriesJE = (from dt in _context.DocumentTypes.Where(i => i.Code == "JE")
                            join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                            }).ToList();
            return View(new { seriesDN = _utility.GetSeries("DN"), seriesJE, genSetting = _utility.GetGeneralSettingAdmin() });
        }
        public void RemoveOrderDetail(int id)
        {
            _context.SaleOrderDetails.Find(id).Delete = true;
            _context.SaveChanges();
        }
        [HttpGet]
        public async Task<IActionResult> GetSerialDetials(int itemId, int wareId, bool isReturnDelivery, bool isCreditMemo, bool isGoodsIssue, decimal cost = 0M)
        {
            if (isReturnDelivery)
            {
                var data = await _saleSerialBatch.GetSerialDetialsReturnDeliveryAsync(itemId, wareId, TransTypeWD.Delivery);
                return Ok(data);
            }
            else if (isGoodsIssue)
            {
                var data = await _saleSerialBatch.GetSerialDetialsGoodsIssueAsync(itemId, wareId, cost);
                return Ok(data);
            }
            else if (isCreditMemo)
            {
                var data = await _saleSerialBatch.GetSerialDetialsReturnDeliveryAsync(itemId, wareId, TransTypeWD.AR);
                return Ok(data);
            }
            else
            {
                var data = await _saleSerialBatch.GetSerialDetialsAsync(itemId, wareId);
                return Ok(data);
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetSerialDetialsDeliveryCopy(int itemId, int saleId, int wareId, bool isCreditMemo)
        {
            if (isCreditMemo)
            {
                var data = await _saleSerialBatch
                                 .GetSerialDetialsCopyAsync(itemId, saleId, wareId, TransTypeWD.AR);
                return Ok(data);
            }
            else
            {
                var data = await _saleSerialBatch
                 .GetSerialDetialsCopyAsync(itemId, saleId, wareId, TransTypeWD.Delivery);
                return Ok(data);
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetBatchDetialsDeliveryCopy(int itemId, int uomId, int saleId, int wareId, bool isCreditMemo)
        {
            if (isCreditMemo)
            {
                var data = await _saleSerialBatch
                 .GetBatchNoDetialsCopyAsync(itemId, uomId, saleId, wareId, TransTypeWD.AR);
                return Ok(data);
            }
            else
            {
                var data = await _saleSerialBatch
                 .GetBatchNoDetialsCopyAsync(itemId, uomId, saleId, wareId, TransTypeWD.Delivery);
                return Ok(data);
            }

        }
        [HttpGet]
        public async Task<IActionResult> GetBatchNoDetials(int itemId, int uomID, int wareId, bool isReturnDelivery, bool isCreditMemo)
        {
            if (isCreditMemo)
            {
                var data = await _saleSerialBatch.GetBatchNoDetialsReturnDeliveryAsync(itemId, uomID, wareId, TransTypeWD.AR);
                return Ok(data);
            }
            else if (isReturnDelivery)
            {
                var data = await _saleSerialBatch.GetBatchNoDetialsReturnDeliveryAsync(itemId, uomID, wareId, TransTypeWD.Delivery);
                return Ok(data);
            }
            else
            {
                var data = await _saleSerialBatch.GetBatchNoDetialsAsync(itemId, uomID, wareId);
                return Ok(data);
            }
        }
        [HttpPost]
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

        [HttpPost]
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
        // get draft diplay
        public IActionResult DisplayDraftDelivery()
        {
            var DOCAR = _context.DocumentTypes.FirstOrDefault(s => s.Code == "DN");
            var AR = _context.DraftDeliveries.Where(s => s.DocTypeID == DOCAR.ID && !s.Delete).ToList();
            var data = (from df in AR
                        join cur in _context.Currency on df.SaleCurrencyID equals cur.ID
                        select new DraftDataViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString() + df.ID,
                            BalanceDue = df.TotalAmount.ToString(),
                            CurrencyName = cur.Description,
                            DocType = DOCAR.Code,
                            DraftName = df.Name,
                            DraftID = df.ID,
                            PostingDate = df.PostingDate.ToString("dd-MM-yyyy"),
                            Remarks = df.Remarks,
                            SubTotal = df.SubTotal.ToString(),
                            SeriesDetailID = df.SeriesDID,
                        }).ToList();
            return Ok(data);
        }
        [HttpGet]
        public IActionResult DeleteDraftDelivert(int id)
        {
            var obj = _context.DraftDeliveries.FirstOrDefault(s => s.ID == id) ?? new DraftDelivery();
            if (obj.ID > 0)
            {
                obj.Delete = true;
                _context.Update(obj);
                _context.SaveChanges();
            }
            return Ok();
        }
        // Findraft
        [HttpGet]
        public IActionResult FindDraftDelivery(string draftname, int draftId)
        {
            var obj = _isale.FindDraftSaleDeliveryAsync(draftname, draftId, GetCompany().ID);
            return Ok(obj);
        }
        //save draft SlaleDelivery
        [HttpPost]
        public IActionResult SaveDraftDelivery(string draftAR)
        {
            ModelMessage msg = new();
            DraftDelivery draft_AR = JsonConvert.DeserializeObject<DraftDelivery>(draftAR, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var Dcheck = _context.DraftDeliveries.AsNoTracking();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            draft_AR.ChangeLog = DateTime.Now;
           if(draft_AR.BranchID==0)
           {
            ModelState.AddModelError("BranchID", _culLocal["Input Draft Branch!"]);
                return Ok(msg.Bind(ModelState));
           }
            draft_AR.UserID = GetUserID();
            draft_AR.Status = "open";
            if (string.IsNullOrWhiteSpace(draft_AR.Name))
            {
                ModelState.AddModelError("Name", _culLocal["Input Draft Name!"]);
                return Ok(msg.Bind(ModelState));
            }
            if (draft_AR.ID == 0)
            {
                if (Dcheck.Any(i => i.Name == draft_AR.Name && i.DocTypeID == draft_AR.DocTypeID))
                {
                    ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            else
            {
                var updateDraft = Dcheck.FirstOrDefault(i => i.Name == draft_AR.Name);
                if (updateDraft == null)
                {
                    if (Dcheck.Any(i => i.Name == draft_AR.Name))
                    {
                        ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                        return Ok(msg.Bind(ModelState));
                    }
                }
                if (Dcheck.Any(i => i.ID != draft_AR.ID && i.Name == draft_AR.Name && i.DocTypeID == draft_AR.DocTypeID))
                {
                    ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            if (ModelState.IsValid)
            {
                draft_AR.LocalCurID = GetCompany().LocalCurrencyID;
                draft_AR.LocalSetRate = localSetRate;
                draft_AR.CompanyID = GetCompany().ID;
                _context.DraftDeliveries.Update(draft_AR);
                _context.SaveChanges();
                var freight = draft_AR.FreightSalesView;
                if (freight != null)
                {
                    //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                    freight.SaleID = draft_AR.ID;
                    freight.SaleType = SaleCopyType.Draftdelivery;
                    freight.OpenAmountReven = freight.AmountReven;
                    _context.FreightSales.Update(freight);
                    _context.SaveChanges();
                }
                ModelState.AddModelError("success", _culLocal["Item save successfully."]);
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        // -------------------------------block draft ARReserve--------------------
        public IActionResult DisplayDraftReserveInvoice()
        {

            var ARRS = _context.DraftReserveInvoices.Where(s => !s.Delete).ToList();
            var data = (from df in ARRS
                        join cur in _context.Currency on df.SaleCurrencyID equals cur.ID
                        join documenttype in _context.DocumentTypes on df.DocTypeID equals documenttype.ID
                        select new DraftDataViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString() + df.ID,
                            BalanceDue = df.TotalAmount.ToString(),
                            CurrencyName = cur.Description,
                            DocType = documenttype.Code,
                            DraftName = df.Name,
                            DraftID = df.ID,
                            PostingDate = df.PostingDate.ToString("dd-MM-yyyy"),
                            Remarks = df.Remarks,
                            SubTotal = df.SubTotal.ToString(),
                            SeriesDetailID = df.SeriesDID,
                        }).ToList();
            return Ok(data);
        }
        [HttpGet]
        public IActionResult DeleteDraftReserveInvoice(int id)
        {
            var obj = _context.DraftReserveInvoices.FirstOrDefault(s => s.ID == id) ?? new DraftReserveInvoice();
            if (obj.ID > 0)
            {
                obj.Delete = true;
                _context.Update(obj);
                _context.SaveChanges();
            }
            return Ok();
        }
        public IActionResult DisplayDraftReserveInvoiceEDT()
        {

            var ARRS = _context.DraftReserveInvoiceEditables.Where(s => !s.Delete).ToList();
            var data = (from df in ARRS
                        join cur in _context.Currency on df.SaleCurrencyID equals cur.ID
                        join documenttype in _context.DocumentTypes on df.DocTypeID equals documenttype.ID
                        select new DraftDataViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString() + df.DraffID,
                            BalanceDue = df.TotalAmount.ToString(),
                            CurrencyName = cur.Description,
                            DocType = documenttype.Code,
                            DraftName = df.Name,
                            DraftID = df.DraffID,
                            PostingDate = df.PostingDate.ToString("dd-MM-yyyy"),
                            Remarks = df.Remarks,
                            SubTotal = df.SubTotal.ToString(),
                            SeriesDetailID = df.SeriesDID,
                        }).ToList();
            return Ok(data);
        }
        [HttpGet]
        public IActionResult DeleteDraftReserveInvoiceEDT(int id)
        {
            var obj = _context.DraftReserveInvoiceEditables.FirstOrDefault(s => s.DraffID == id) ?? new DraftReserveInvoiceEditable();
            if (obj.DraffID > 0)
            {
                obj.Delete = true;
                _context.Update(obj);
                _context.SaveChanges();
            }
            return Ok();
        }
        [HttpGet]
        public IActionResult FindDraftReserveInvoiceEDT(string draftname, int draftId)
        {
            var obj = _isale.FindDraftReserveEDTAsync(draftname, draftId, GetCompany().ID);
            return Ok(obj);
        }
        // Findraft
        [HttpGet]
        public IActionResult FindDraftReserveInvoice(string draftname, int draftId)
        {
            var obj = _isale.FindDraftReserveAsync(draftname, draftId, GetCompany().ID);
            return Ok(obj);
        }
        //save draft DraftReserveInvoice
        [HttpPost]
        public IActionResult SaveDraftReserveInvoiceEDT(string draftAR)
        {
            ModelMessage msg = new();
            DraftReserveInvoiceEditable draft_AR = JsonConvert.DeserializeObject<DraftReserveInvoiceEditable>(draftAR, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var Dcheck = _context.DraftReserveInvoices.AsNoTracking();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            draft_AR.ChangeLog = DateTime.Now;
           
            draft_AR.UserID = GetUserID();
            draft_AR.Status = "open";
            if (string.IsNullOrWhiteSpace(draft_AR.Name))
            {
                ModelState.AddModelError("Name", _culLocal["Input Draft Name!"]);
                return Ok(msg.Bind(ModelState));
            }
             if (draft_AR.BranchID==0)
            {
                ModelState.AddModelError("BranchID", _culLocal["Input Draft Branch!"]);
                
            }
            if (draft_AR.DraffID == 0)
            {
                if (Dcheck.Any(i => i.Name == draft_AR.Name && i.DocTypeID == draft_AR.DocTypeID))
                {
                    ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            else
            {
                var updateDraft = Dcheck.FirstOrDefault(i => i.Name == draft_AR.Name);
                if (updateDraft == null)
                {
                    if (Dcheck.Any(i => i.Name == draft_AR.Name))
                    {
                        ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                        return Ok(msg.Bind(ModelState));
                    }
                }
                if (Dcheck.Any(i => i.ID != draft_AR.DraffID && i.Name == draft_AR.Name && i.DocTypeID == draft_AR.DocTypeID))
                {
                    ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            if (ModelState.IsValid)
            {
                draft_AR.LocalCurID = GetCompany().LocalCurrencyID;
                draft_AR.LocalSetRate = localSetRate;
                draft_AR.CompanyID = GetCompany().ID;
                _context.DraftReserveInvoiceEditables.Update(draft_AR);
                _context.SaveChanges();
                var freight = draft_AR.FreightSalesView;
                if (freight != null)
                {
                    freight.SaleID = draft_AR.DraffID;
                    freight.SaleType = SaleCopyType.DraftSARReserveInvoiceEDT;
                    freight.OpenAmountReven = freight.AmountReven;
                    _context.FreightSales.Update(freight);
                    _context.SaveChanges();
                }
                ModelState.AddModelError("success", _culLocal["Item save successfully."]);
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        //save draft DraftReserveInvoice
        [HttpPost]
        public IActionResult SaveDraftReserveInvoice(string draftAR)
        {
            ModelMessage msg = new();
            DraftReserveInvoice draft_AR = JsonConvert.DeserializeObject<DraftReserveInvoice>(draftAR, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var Dcheck = _context.DraftReserveInvoices.AsNoTracking();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            draft_AR.ChangeLog = DateTime.Now;
          
            draft_AR.UserID = GetUserID();
            draft_AR.Status = "open";
             if (draft_AR.BranchID==0)
            {
                ModelState.AddModelError("BranchID", _culLocal["Input Draft Branch!"]);
                 return Ok(msg.Bind(ModelState));
            }
            if (string.IsNullOrWhiteSpace(draft_AR.Name))
            {
                ModelState.AddModelError("Name", _culLocal["Input Draft Name!"]);
                return Ok(msg.Bind(ModelState));
            }
            if (draft_AR.ID == 0)
            {
                if (Dcheck.Any(i => i.Name == draft_AR.Name && i.DocTypeID == draft_AR.DocTypeID))
                {
                    ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            else
            {
                var updateDraft = Dcheck.FirstOrDefault(i => i.Name == draft_AR.Name);
                if (updateDraft == null)
                {
                    if (Dcheck.Any(i => i.Name == draft_AR.Name))
                    {
                        ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                        return Ok(msg.Bind(ModelState));
                    }
                }
                if (Dcheck.Any(i => i.ID != draft_AR.ID && i.Name == draft_AR.Name && i.DocTypeID == draft_AR.DocTypeID))
                {
                    ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            if (ModelState.IsValid)
            {
                draft_AR.LocalCurID = GetCompany().LocalCurrencyID;
                draft_AR.LocalSetRate = localSetRate;
                draft_AR.CompanyID = GetCompany().ID;
                _context.DraftReserveInvoices.Update(draft_AR);
                _context.SaveChanges();
                var freight = draft_AR.FreightSalesView;
                if (freight != null)
                {
                    freight.SaleID = draft_AR.ID;
                    freight.SaleType = SaleCopyType.DraftReserve;
                    freight.OpenAmountReven = freight.AmountReven;
                    _context.FreightSales.Update(freight);
                    _context.SaveChanges();
                }
                ModelState.AddModelError("success", _culLocal["Item save successfully."]);
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }

        //-------------------------------------block draft Service Contract ----------------------------------------------
        public IActionResult DisplayDraftServiceContract()
        {

            var ARRS = _context.DraftServiceContracts.Where(s => !s.Delete).ToList();
            var data = (from df in ARRS
                        join cur in _context.Currency on df.SaleCurrencyID equals cur.ID
                        join documenttype in _context.DocumentTypes on df.DocTypeID equals documenttype.ID
                        select new DraftDataViewModel
                        {
                            CustomerID = df.CusID,
                            LineID = DateTime.Now.Ticks.ToString() + df.ID,
                            BalanceDue = df.TotalAmount.ToString(),
                            CurrencyName = cur.Description,
                            DocType = documenttype.Code,
                            DraftName = df.Name,
                            DraftID = df.ID,
                            PostingDate = df.PostingDate.ToString("dd-MM-yyyy"),
                            Remarks = df.Remarks,
                            SubTotal = df.SubTotal.ToString(),
                            SeriesDetailID = df.SeriesDID,
                        }).ToList();
            return Ok(data);
        }
        [HttpGet]
        public IActionResult DeleteDraftServiceContract(int id)
        {
            var obj = _context.DraftServiceContracts.FirstOrDefault(s => s.ID == id) ?? new DraftServiceContract();
            if (obj.ID > 0)
            {
                obj.Delete = true;
                _context.Update(obj);
                _context.SaveChanges();
            }
            return Ok();
        }
        // Findraft
        [HttpGet]
        public async Task<IActionResult> FindDraftServiceContract(int draftId, int cusid)
        {
            var obj = await _isale.FinddraftServiceContract(draftId, cusid);

            return Ok(Task.FromResult(obj));
        }
        //save draft DraftServiceContract
        [HttpPost]
        public IActionResult SaveDraftServiceContract(string draftAR)
        {
            ModelMessage msg = new();
            DraftServiceContract draft_AR = JsonConvert.DeserializeObject<DraftServiceContract>(draftAR, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            draft_AR.DraftAttchmentFiles = draft_AR.DraftAttchmentFiles.Where(s => !string.IsNullOrWhiteSpace(s.FileName)).ToList();
            var Dcheck = _context.DraftServiceContracts.AsNoTracking();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            draft_AR.ChangeLog = DateTime.Now;
           
            draft_AR.UserID = GetUserID();
            draft_AR.Status = "open";
             if (draft_AR.BranchID==0)
            {
                ModelState.AddModelError("BranchID", _culLocal["Input Draft Branch!"]);
                 return Ok(msg.Bind(ModelState));
            }
            if (string.IsNullOrWhiteSpace(draft_AR.Name))
            {
                ModelState.AddModelError("Name", _culLocal["Input Draft Name!"]);
                return Ok(msg.Bind(ModelState));
            }
            if (draft_AR.ID == 0)
            {
                if (Dcheck.Any(i => i.Name == draft_AR.Name && i.DocTypeID == draft_AR.DocTypeID))
                {
                    ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            else
            {
                var updateDraft = Dcheck.FirstOrDefault(i => i.Name == draft_AR.Name);
                if (updateDraft == null)
                {
                    if (Dcheck.Any(i => i.Name == draft_AR.Name))
                    {
                        ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                        return Ok(msg.Bind(ModelState));
                    }
                }
                if (Dcheck.Any(i => i.ID != draft_AR.ID && i.Name == draft_AR.Name && i.DocTypeID == draft_AR.DocTypeID))
                {
                    ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            if (ModelState.IsValid)
            {
                draft_AR.LocalCurID = GetCompany().LocalCurrencyID;
                draft_AR.LocalSetRate = localSetRate;
                draft_AR.CompanyID = GetCompany().ID;
                _context.DraftServiceContracts.Update(draft_AR);
                _context.SaveChanges();
                var freight = draft_AR.FreightSalesView;
                if (freight != null)
                {
                    //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                    freight.SaleID = draft_AR.ID;
                    freight.SaleType = SaleCopyType.DraftServiceContract;
                    freight.OpenAmountReven = freight.AmountReven;
                    _context.FreightSales.Update(freight);
                    _context.SaveChanges();
                }
                ModelState.AddModelError("success", _culLocal["Item save successfully."]);
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }


        [HttpPost]

        public IActionResult CreateSaleDelivery(string data, string seriesJE, string serial, string batch, string type)
        {
            ModelMessage msg = new();
            SaleDelivery saleDelivery = JsonConvert.DeserializeObject<SaleDelivery>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            if (seriesJE == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }

            Series seriesje = JsonConvert.DeserializeObject<Series>(seriesJE, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            SeriesDetail seriesDetail = new();
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == saleDelivery.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == saleDelivery.DocTypeID).FirstOrDefault();
            var series_JE = _context.Series.FirstOrDefault(w => w.ID == seriesje.ID);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var g = _isale.GetAllGroupDefind().ToList();
            var wh = _context.Warehouses.Find(saleDelivery.WarehouseID) ?? new Warehouse();
            saleDelivery.UserID = GetUserID();
           
            saleDelivery.ChangeLog = DateTime.Now;
            // Checking Serial Batch //
            List<SerialNumber> _serialNumber = new();
            List<SerialNumber> serialNumber = new();
            List<BatchNo> batchNoes = new();
            List<BatchNo> _batchNoes = new();
            if (!string.IsNullOrEmpty(saleDelivery.RefNo))
            {
                bool isRefExisted = _context.SaleDeliveries.AsNoTracking().Any(i => i.RefNo == saleDelivery.RefNo);
                if (isRefExisted)
                {
                    ModelState.AddModelError("RefNo", $"Transaction with Customer Ref No. \"{saleDelivery.RefNo}\" already done.");
                }
            }
            ValidateSummary(saleDelivery, saleDelivery.SaleDeliveryDetails);
            CheckTaxAcc(saleDelivery.SaleDeliveryDetails);
            decimal disRate = (decimal)saleDelivery.DisRate;
            if (saleDelivery.DueDate < saleDelivery.PostingDate)
            {
                ModelState.AddModelError("DueDate", "Item has invalid due date.");
            }
            foreach (var dt in saleDelivery.SaleDeliveryDetails)
            {
                var factor = g.FirstOrDefault(w => w.GroupUoMID == dt.GUomID && w.AltUOM == dt.UomID).Factor;
                dt.Factor = factor;
                if (dt.SDDID <= 0)
                {
                    dt.PrintQty = dt.Qty;
                    dt.OpenQty = dt.Qty;
                    if (saleDelivery.CopyType > 0)
                    {
                        dt.Qty = dt.OpenQty;
                    }
                    else
                    {
                        dt.OpenQty = dt.Qty;
                    }
                }
                else
                {
                    dt.PrintQty = dt.Qty - dt.PrintQty;
                }
                dt.EditeQty = dt.Qty;
            }
            var itemsReturn = CheckStock(saleDelivery, saleDelivery.SaleDeliveryDetails.ToList(), "SDDID").ToList();
            List<ItemsReturn> returns = new();
            List<ItemsReturn> itemSBOnly = itemsReturn.Where(i => i.IsSerailBatch && i.InStock > 0 && !i.IsBOM).ToList();
            if (wh.IsAllowNegativeStock)
            {
                returns = itemsReturn.Where(i => i.InStock <= 0 && i.IsSerailBatch).ToList();
            }
            else
            {
                // returns = itemsReturn.Where(i => i.InStock <= 0).ToList();
                returns = itemsReturn.ToList();
            }
            bool isError = false;
            foreach (var ir in returns)
            {
                ModelState.AddModelError($"itemReturn{DateTime.Now.Ticks}", "The item &lt;&lt;" + ir.KhmerName + "&gt;&gt; has stock left " + ir.InStock + ".");
                isError = true;
            }
            if (isError) return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });

            #region Serial Batch Block
            if (itemSBOnly.Count > 0)
            {
                serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : serialNumber;
                _saleSerialBatch.CheckItemSerail(saleDelivery, saleDelivery.SaleDeliveryDetails.ToList(), serialNumber);
                serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in serialNumber.ToList())
                {
                    foreach (var i in saleDelivery.SaleDeliveryDetails.ToList())
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
                batchNoes = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : batchNoes;
                _saleSerialBatch.CheckItemBatch(saleDelivery, saleDelivery.SaleDeliveryDetails.ToList(), batchNoes);
                batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in batchNoes.ToList())
                {
                    foreach (var i in saleDelivery.SaleDeliveryDetails.ToList())
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
                    return Ok(new { IsBatch = true, Data = batchNoes });
                }
            }
            #endregion

            using (var t = _context.Database.BeginTransaction())
            {
                switch (saleDelivery.CopyType)
                {
                    case SaleCopyType.Quotation:
                        var copyKeyQ = saleDelivery.CopyKey.Split("-")[1];
                        var quoteMaster = _context.SaleQuotes.Include(q => q.SaleQuoteDetails)
                            .FirstOrDefault(m => string.Compare(m.InvoiceNumber, copyKeyQ) == 0);
                        if (quoteMaster != null)
                        {
                            UpdateSourceCopy(saleDelivery.SaleDeliveryDetails, quoteMaster, quoteMaster.SaleQuoteDetails, SaleCopyType.Quotation);
                        }
                        break;
                    case SaleCopyType.Order:
                        string copyKeyO = "";

                        copyKeyO = saleDelivery.CopyKey.Split("-")[1];
                        var docType = saleDelivery.CopyKey.Split(new Char[] { '-', ':' })[1];

                        if (docType == "SQ")
                        {
                            copyKeyO = saleDelivery.CopyKey.Split(new Char[] { '-', '/' })[3];
                        }
                        var orderMaster = _context.SaleOrders.Include(o => o.SaleOrderDetails)
                            .FirstOrDefault(m => string.Compare(m.InvoiceNumber, copyKeyO) == 0);
                        if (orderMaster != null)
                        {
                            UpdateSourceCopy(saleDelivery.SaleDeliveryDetails, orderMaster, orderMaster.SaleOrderDetails, SaleCopyType.Order);
                        }
                        break;
                    case SaleCopyType.ARReserveInvoice:
                        string copyKeyA = "";

                        copyKeyA = saleDelivery.CopyKey.Split("-")[1];
                        var docTypeA = saleDelivery.CopyKey.Split(new Char[] { '-', ':' })[1];

                        if (docTypeA == "SQ" || docTypeA == "SO")
                        {
                            if (docTypeA == "SQ")
                            {
                                copyKeyA = saleDelivery.CopyKey.Split(new Char[] { '-', '/' })[5];
                            }
                            else
                            {
                                copyKeyA = saleDelivery.CopyKey.Split(new Char[] { '-', '/' })[4];
                            }
                        }
                        var arreserveMaster = _context.ARReserveInvoices.Include(o => o.ARReserveInvoiceDetails)
                            .FirstOrDefault(m => string.Compare(m.InvoiceNumber, copyKeyA) == 0);
                        if (arreserveMaster != null)
                        {
                            UpdateSourceCopy(saleDelivery.SaleDeliveryDetails, arreserveMaster, arreserveMaster.ARReserveInvoiceDetails, SaleCopyType.ARReserveInvoice);
                        }
                        break;
                    case SaleCopyType.ARReserveInvoiceEDT:


                        _isale.UpdateSourceARReserveInvoiceEDT(saleDelivery.BaseOnID, saleDelivery.SaleDeliveryDetails.ToList());

                        break;
                }

                if (type == "QO")
                {
                    if (ModelState.IsValid)
                    {
                        seriesDetail.Number = seriesDN.NextNo;
                        seriesDetail.SeriesID = saleDelivery.SeriesID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesDN.NextNo;
                        long No = long.Parse(Sno);
                        saleDelivery.InvoiceNo = seriesDN.NextNo;
                        saleDelivery.InvoiceNumber = seriesDN.NextNo;
                        seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                        if (No > long.Parse(seriesDN.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Sale Delivery Invoice has reached the limitation!");
                            return Ok(msg.Bind(ModelState));
                        }
                        if (saleDelivery.SDID == 0 && saleDelivery.CopyType == 0)
                        {
                            saleDelivery.LocalSetRate = localSetRate;
                        }

                        saleDelivery.SeriesDID = seriesDetailID;
                        saleDelivery.CompanyID = GetCompany().ID;
                        saleDelivery.LocalCurID = GetCompany().LocalCurrencyID;
                        saleDelivery.LocalSetRate = localSetRate;

                        _context.SaleDeliveries.Update(saleDelivery);
                        _context.Series.Update(seriesDN);
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        _isale.IssuseInStockMaterialDelivery(saleDelivery.SDID, _serialNumber, _batchNoes);
                        // checking maximun Invoice
                        var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                        var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                        if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        var freight = saleDelivery.FreightSalesView;
                        if (freight != null)
                        {
                            //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                            freight.SaleID = saleDelivery.SDID;
                            freight.SaleType = SaleCopyType.Delivery;
                            freight.OpenAmountReven = freight.AmountReven;
                            _context.FreightSales.Update(freight);
                            _context.SaveChanges();
                        }
                        t.Commit();
                        ModelState.AddModelError("success", "Item save successfully.");
                        msg.Approve();
                    }
                }
                else if (type == "SO")
                {
                    if (ModelState.IsValid)
                    {
                        seriesDetail.Number = seriesDN.NextNo;
                        seriesDetail.SeriesID = saleDelivery.SeriesID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesDN.NextNo;
                        long No = long.Parse(Sno);
                        saleDelivery.InvoiceNo = seriesDN.NextNo;
                        saleDelivery.InvoiceNumber = seriesDN.NextNo;
                        seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                        if (No > long.Parse(seriesDN.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Sale Delivery Invoice has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }
                        if (saleDelivery.SDID == 0 && saleDelivery.CopyType == 0)
                        {
                            saleDelivery.LocalSetRate = localSetRate;
                        }
                        saleDelivery.SeriesDID = seriesDetailID;
                        saleDelivery.CompanyID = GetCompany().ID;
                        saleDelivery.LocalCurID = GetCompany().LocalCurrencyID;
                        saleDelivery.LocalSetRate = localSetRate;
                        _context.SaleDeliveries.Update(saleDelivery);
                        _context.Series.Update(seriesDN);
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        _isale.IssuseInStockMaterialDelivery(saleDelivery.SDID, _serialNumber, _batchNoes);
                        // checking maximun Invoice
                        var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                        var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                        if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        var freight = saleDelivery.FreightSalesView;
                        if (freight != null)
                        {
                            //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                            freight.SaleID = saleDelivery.SDID;
                            freight.SaleType = SaleCopyType.Delivery;
                            freight.OpenAmountReven = freight.AmountReven;
                            _context.FreightSales.Update(freight);
                            _context.SaveChanges();
                        }
                        t.Commit();
                        ModelState.AddModelError("success", "Item save successfully.");
                        msg.Approve();
                    }
                }
                else if (type == "AR")
                {
                    if (ModelState.IsValid)
                    {
                        seriesDetail.Number = seriesDN.NextNo;
                        seriesDetail.SeriesID = saleDelivery.SeriesID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesDN.NextNo;
                        long No = long.Parse(Sno);
                        saleDelivery.InvoiceNo = seriesDN.NextNo;
                        saleDelivery.InvoiceNumber = seriesDN.NextNo;
                        seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                        if (No > long.Parse(seriesDN.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Sale Delivery Invoice has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }
                        if (saleDelivery.SDID == 0 && saleDelivery.CopyType == 0)
                        {
                            saleDelivery.LocalSetRate = localSetRate;
                        }
                        saleDelivery.SeriesDID = seriesDetailID;
                        saleDelivery.CompanyID = GetCompany().ID;
                        saleDelivery.LocalCurID = GetCompany().LocalCurrencyID;
                        saleDelivery.LocalSetRate = localSetRate;
                        _context.SaleDeliveries.Update(saleDelivery);
                        _context.Series.Update(seriesDN);
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        _isale.IssuseInStockARReserveInvocklDelivery(saleDelivery.SDID, _serialNumber, _batchNoes);
                        // checking maximun Invoice
                        var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                        var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                        if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        var freight = saleDelivery.FreightSalesView;
                        if (freight != null)
                        {
                            //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                            freight.SaleID = saleDelivery.SDID;
                            freight.SaleType = SaleCopyType.Delivery;
                            freight.OpenAmountReven = freight.AmountReven;
                            _context.FreightSales.Update(freight);
                            _context.SaveChanges();
                        }
                        t.Commit();
                        ModelState.AddModelError("success", "Item save successfully.");
                        msg.Approve();
                    }
                }
            }
            return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
        }

        [HttpGet]
        public IActionResult FindSaleDelivery(string number, int seriesID)
        {
            var list = _isale.FindSaleDelivery(number, seriesID, GetCompany().ID);
            if (list.SaleDelivery != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        #endregion
        //--------------------------------------------// End Sale Delivery //------------------------------------//

        //--------------------------------------------// Start Return Delivery //------------------------------------//
        #region Return Delivery
        [Privilege("RD")]

        public IActionResult ReturnDelivery()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Return Delivery";
            ViewBag.Sale = "show";
            ViewBag.ReturnDelivery = "highlight";
            var seriesJE = (from dt in _context.DocumentTypes.Where(i => i.Code == "JE")
                            join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                            }).ToList();
            return View(new { seriesRE = _utility.GetSeries("RE"), seriesJE, genSetting = _utility.GetGeneralSettingAdmin() });
        }
        public IActionResult FindReturnDelivery(string number, int seriesID)
        {
            var list = _isale.FindReturnDelivery(number, seriesID, GetCompany().ID);
            if (list.ReturnDelivery != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        [HttpPost]
        public IActionResult CreateReturnDelivery(
            string data,
            string type,
            string seriesJE,
            int seriesID,
            string serial,
            string batch,
            string checkingSerialString = "",
            string checkingBatchString = "")
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            if (seriesJE == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(new { Model = msg.Bind(ModelState) });
            }
            Series seriesje = JsonConvert.DeserializeObject<Series>(seriesJE, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ReturnDelivery rd = JsonConvert.DeserializeObject<ReturnDelivery>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesCD = _context.Series.FirstOrDefault(w => w.ID == rd.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == rd.DocTypeID).FirstOrDefault();
            var series_JE = _context.Series.FirstOrDefault(w => w.ID == seriesje.ID);
            var g = _isale.GetAllGroupDefind().ToList();

            rd.ChangeLog = DateTime.Now;
            rd.UserID = GetUserID();
           

            // Checking Serial Batch //
            List<SerialNumber> _serialNumber = new();
            List<SerialNumber> serialNumber = new();
            List<BatchNo> _batchNoes = new();
            List<BatchNo> batchNoes = new();

            if (!string.IsNullOrEmpty(rd.RefNo))
            {
                bool isRefExisted = _context.ReturnDeliverys.AsNoTracking().Any(i => i.RefNo == rd.RefNo);
                if (isRefExisted)
                {
                    ModelState.AddModelError("RefNo", $"Transaction with Customer Ref No. \"{rd.RefNo}\" already done.");
                }
            }
            ValidateSummary(rd, rd.ReturnDeliveryDetails);
            CheckTaxAcc(rd.ReturnDeliveryDetails);
            foreach (var dt in rd.ReturnDeliveryDetails)
            {
                var factor = g.FirstOrDefault(w => w.GroupUoMID == dt.GUomID && w.AltUOM == dt.UomID).Factor;
                dt.Factor = factor;
                if (dt.ID == 0)
                {
                    dt.PrintQty = dt.Qty;
                    if (rd.CopyType > 0)
                    {
                        if (dt.Qty > dt.OpenQty)
                        {
                            ModelState.AddModelError($"Qty{dt.ItemID}", $"Item name {dt.ItemNameKH} its qty cannot be greater than {dt.OpenQty}!");
                            //return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        //dt.Qty = dt.OpenQty;
                    }
                    else
                    {
                        dt.OpenQty = dt.Qty;
                    }
                }
                else
                {
                    dt.PrintQty = dt.Qty - dt.PrintQty;
                }

                if (rd.CopyType == 0)
                {
                    if (dt.Process != "Standard")
                    {
                        var _purchasedItems = _context.InventoryAudits.Where(ia => ia.ItemID == dt.ItemID && ia.Trans_Type == "PU").ToList();
                        if (_purchasedItems.Count <= 0)
                        {
                            ModelState.AddModelError("itemDetailCost", "Item detail&lt;" + dt.ItemNameKH + "&gt; has not purchased yet.");
                        }
                        else
                        {
                            //var a = dt.Cost * rd.ExchangeRate;
                            //var b = _purchasedItems.Max(p => p.Cost) * dt.Factor;
                            if (dt.Cost < 0 || dt.Cost * rd.ExchangeRate > _purchasedItems.Max(p => p.Cost) * dt.Factor)
                            {
                                ModelState.AddModelError("itemDetailCost", "Item detail&lt;" + dt.ItemNameKH + "&gt; cost cannot be negative " +
                                    "or exceeds " + _purchasedItems.Max(p => p.Cost) * dt.Factor + ".");
                            }
                        }
                    }
                }
            }

            using (var t = _context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    switch (rd.CopyType)
                    {
                        case SaleCopyType.Delivery:

                            var deliveryMaster = _context.SaleDeliveries.Include(o => o.SaleDeliveryDetails)
                                 .FirstOrDefault(m => m.SDID == rd.BasedOn) ?? new SaleDelivery();
                            UpdateSourceCopy(rd.ReturnDeliveryDetails, deliveryMaster, deliveryMaster.SaleDeliveryDetails, SaleCopyType.Delivery);
                            type = "Delivery"; //DN
                            break;
                    }

                    if (type == "add")
                    {
                        serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) : serialNumber;

                        _saleSerialBatch.CheckItemSerail(rd, rd.ReturnDeliveryDetails.ToList(), serialNumber);
                        serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                        foreach (var j in serialNumber.ToList())
                        {
                            foreach (var i in rd.ReturnDeliveryDetails.ToList())
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
                        batchNoes = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) : batchNoes;
                        _saleSerialBatch.CheckItemBatch(rd, rd.ReturnDeliveryDetails.ToList(), batchNoes);
                        batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                        foreach (var j in batchNoes.ToList())
                        {
                            foreach (var i in rd.ReturnDeliveryDetails.ToList())
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
                    else
                    {
                        serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) : serialNumber;
                        _saleSerialBatch.CheckItemSerailCopy(rd, rd.ReturnDeliveryDetails.ToList(), serialNumber, "In");
                        serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                        foreach (var j in serialNumber.ToList())
                        {
                            foreach (var i in rd.ReturnDeliveryDetails.ToList())
                            {
                                if (j.ItemID == i.ItemID)
                                {
                                    _serialNumber.Add(j);
                                }
                            }
                        }
                        if (checkingSerialString != "checked" && checkingSerialString == "unchecked" && _serialNumber.Count > 0)
                        {
                            return Ok(new { IsSerialCopy = true, Data = _serialNumber });
                        }
                        //SerialNumberSelected = null
                        batchNoes = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) : batchNoes;
                        _saleSerialBatch.CheckItemBatchCopy(rd, rd.ReturnDeliveryDetails.ToList(), batchNoes, "In", TransTypeWD.Delivery);
                        batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                        foreach (var j in batchNoes.ToList())
                        {
                            foreach (var i in rd.ReturnDeliveryDetails.ToList())
                            {
                                if (j.ItemID == i.ItemID)
                                {
                                    _batchNoes.Add(j);
                                }
                            }

                        }
                        if (checkingBatchString != "checked" && checkingBatchString == "unchecked" && _batchNoes.Count > 0)
                        {
                            return Ok(new { IsBatchCopy = true, Data = _batchNoes });
                        }
                    }
                    seriesDetail.Number = seriesCD.NextNo;
                    seriesDetail.SeriesID = rd.SeriesID;
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.SaveChanges();
                    var seriesDetailID = seriesDetail.ID;
                    string Sno = seriesCD.NextNo;
                    long No = long.Parse(Sno);
                    rd.InvoiceNumber = seriesCD.NextNo;
                    seriesCD.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                    if (No > long.Parse(seriesCD.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Return Delivery Invoice has reached the limitation!!");
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                    if (SaleCopyType.Delivery == rd.CopyType)
                    {
                        var number = rd.CopyKey.Split("-")[1];
                        var delivery = _context.SaleDeliveries.Include(o => o.SaleDeliveryDetails)
                               .FirstOrDefault(m => string.Compare(m.InvoiceNumber, number) == 0 && m.SeriesID == seriesID);
                        //ARMaster.AppliedAmount = 0;
                        var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w => w.InvoiceNumber == number && w.SeriesID == seriesID);
                        if (delivery != null)
                        {
                            rd.BasedOn = delivery.SDID;
                            if (ModelState.IsValid)
                            {
                                UpdateSourceCopyDelivery(rd.ReturnDeliveryDetails, delivery, delivery.SaleDeliveryDetails, SaleCopyType.Delivery);
                                var totalOpenQty = delivery.SaleDeliveryDetails.Sum(i => i.OpenQty);
                                if (totalOpenQty == 0)
                                {
                                    delivery.Status = "close";
                                }
                                _context.SaleDeliveries.Update(delivery);
                                _context.SaveChanges();
                            }
                        }

                        _context.SaveChanges();
                    }
                    else
                    {
                        rd.LocalSetRate = (decimal)localSetRate;
                    }
                    rd.LocalCurID = GetCompany().LocalCurrencyID;
                    rd.LocalSetRate = (decimal)localSetRate;
                    rd.SeriesDID = seriesDetailID;
                    rd.CompanyID = GetCompany().ID;
                    _context.ReturnDeliverys.Update(rd);
                    _context.SaveChanges();
                    _isale.ReturnDeliveryStock(rd.ID, _serialNumber, _batchNoes);
                    // checking maximun Invoice
                    var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                    var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                    if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                    var freight = rd.FreightSalesView;
                    if (freight != null)
                    {
                        //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                        freight.SaleID = rd.ID;
                        freight.SaleType = SaleCopyType.ReturnDelivery;
                        freight.OpenAmountReven = freight.AmountReven;
                        _context.FreightSales.Update(freight);
                        _context.SaveChanges();
                    }
                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }
            return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
        }
        #endregion
        //--------------------------------------------// End Return Delivery //------------------------------------//

        //--------------------------------------------// Start Sale ARDown Payment //------------------------------------//
        #region A/R Down Payment Invoice
        public IActionResult ARDownPaymentInvoice()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Order";
            ViewBag.Sale = "show";
            ViewBag.ARDownPaymentInvoice = "highlight";
            var seriesJE = (from dt in _context.DocumentTypes.Where(i => i.Code == "JE")
                            join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                            }).ToList();

            return View(new { seriesCD = _utility.GetSeries("CD"), seriesJE, genSetting = _utility.GetGeneralSettingAdmin() });
        }
        [HttpPost]
        public IActionResult UpdateARDonw(string data, string seriesJe)
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            if (seriesJe == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(new { Model = msg.Bind(ModelState) });
            }
            Series seriesje = JsonConvert.DeserializeObject<Series>(seriesJe, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ARDownPayment ard = JsonConvert.DeserializeObject<ARDownPayment>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ard.ChangeLog = DateTime.Now;
            var seriesCD = _context.Series.FirstOrDefault(w => w.ID == ard.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == ard.DocTypeID).FirstOrDefault();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;

            ard.UserID = GetUserID();
         

            var g = _isale.GetAllGroupDefind().ToList();

            if (ard.DueDate < ard.ValidUntilDate)
            {
                ModelState.AddModelError("UntilDate", "Item has invalid until date.");
            }
            if (!string.IsNullOrEmpty(ard.RefNo))
            {
                bool isRefExisted = _context.ARDownPayments.AsNoTracking().Any(i => i.RefNo == ard.RefNo);
                if (isRefExisted)
                {
                    ModelState.AddModelError("RefNo", $"Transaction with Customer Ref No. \"{ard.RefNo}\" already done.");
                }
            }
            ValidateSummary(ard, ard.ARDownPaymentDetails);
            CheckTaxAcc(ard.ARDownPaymentDetails);
            foreach (var d in ard.ARDownPaymentDetails)
            {
                var factor = g.FirstOrDefault(w => w.GroupUoMID == d.GUomID && w.AltUOM == d.UomID).Factor;
                d.Factor = (decimal)factor;

                if (ard.ARDID == 0)
                {
                    d.PrintQty = d.Qty;
                    d.OpenQty = d.Qty;
                }
                else
                {
                    d.PrintQty = d.Qty - d.PrintQty;
                }
            }

            using (var t = _context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    if (ard.ARDID > 0)
                    {
                        _context.ARDownPayments.Update(ard);
                    }
                    else
                    {
                        seriesDetail.Number = seriesCD.NextNo;
                        seriesDetail.SeriesID = ard.SeriesID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesCD.NextNo;
                        long No = long.Parse(Sno);
                        ard.InvoiceNumber = seriesCD.NextNo;
                        seriesCD.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                        if (long.Parse(seriesCD.NextNo) > long.Parse(seriesCD.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", "Your Invoice has reached the limitation!!");
                            return Ok(msg.Bind(ModelState));
                        }
                        ard.LocalSetRate = (decimal)localSetRate;
                        ard.LocalCurID = GetCompany().LocalCurrencyID;
                        ard.LocalSetRate = (decimal)localSetRate;
                        ard.SeriesDID = seriesDetailID;
                        ard.CompanyID = GetCompany().ID;
                        ard.Status = "open";
                        _context.ARDownPayments.Update(ard);
                    }
                    _context.SaveChanges();
                    var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                    if (gldeter != null)
                    {
                        if (gldeter.GLID == 0)
                        {
                            ModelState.AddModelError("SaleGLDeter", "Sale GLAcc Determination is missing!!");
                            return Ok(msg.Bind(ModelState));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("SaleGLDeter", "Sale GLAcc Determination is not set up yet!!");
                        return Ok(msg.Bind(ModelState));
                    }
                    _isale.IssueARDownPayment(ard, gldeter);
                    // checking maximun Invoice
                    var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                    var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                    if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                        return Ok(msg.Bind(ModelState));
                    }
                    _isale.CreateIncomingPaymentCustomerByARDownPayment(ard, GetSystemCurrencies().FirstOrDefault());

                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult FindSaleARDown(string number, int seriesID)
        {
            var list = _isale.FindSaleARDown(number, seriesID, GetCompany().ID);
            if (list.ARDownPayment != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        public IActionResult GetSaleARDownPM(int cusId)
        {
            var allItems = _isale.GetSaleARDownPMCopy(cusId);
            return Ok(allItems);
        }
        public IActionResult GetSaleARDownPMDetailCopy(string number, int seriesId, bool fromCN = false)
        {
            var data = _isale.GetSaleARDownPMDetailCopy(number, seriesId, GetCompany().ID, fromCN);
            return Ok(data);
        }
        #endregion
        //--------------------------------------------// End Sale AR Down Payment //------------------------------------//

        //--------------------------------------------// Start Sale A/R //------------------------------------//
        #region Sale A/R
        [Privilege("AR")]
        public IActionResult SaleAR()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale AR";
            ViewBag.Sale = "show";
            ViewBag.SaleAR = "highlight";
            var seriesJE = (from dt in _context.DocumentTypes.Where(i => i.Code == "JE")
                            join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                            }).ToList();
            return View(new { seriesIN = _utility.GetSeries("IN"), seriesJE, genSetting = _utility.GetGeneralSettingAdmin() });
        }
        private bool CreateIncomingPaymentCustomerBySaleAR(SaleAR saleAR)
        {
            string currencyName = _context.Currency.Find(saleAR.SaleCurrencyID).Description;
            SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
            var payment = _context.IncomingPaymentCustomers.FirstOrDefault(p => p.InvoiceNumber == saleAR.InvoiceNumber && p.SeriesID == saleAR.SeriesID);
            var docType = _context.DocumentTypes.Find(saleAR.DocTypeID);
            var em = _context.Employees.FirstOrDefault(i => i.ID == saleAR.SaleEmID) ?? new Employee();
            var user = _userModule.CurrentUser;
            IncomingPaymentCustomer ipcustomer = new()
            {
                IncomingPaymentCustomerID = 0,
                EmID = em.ID,
                EmName = em.Name,
                CreatorID = user.ID,
                CreatorName = user.Username,
                CustomerID = saleAR.CusID,
                BranchID = saleAR.BranchID,
                WarehouseID = saleAR.WarehouseID,
                DocTypeID = saleAR.DocTypeID,
                SeriesID = saleAR.SeriesID,
                SeriesDID = saleAR.SeriesDID,
                CompanyID = saleAR.CompanyID,
                InvoiceNumber = saleAR.InvoiceNumber,
                CurrencyID = saleAR.SaleCurrencyID,
                Types = SaleCopyType.AR.ToString(),
                //DocumentNo = saleAR.InvoiceNo,
                //DocumentType = GetTransactType(saleAR.InvoiceNo, saleAR.IncludeVat),
                Applied_Amount = saleAR.AppliedAmount,
                CurrencyName = currencyName,
                ExchangeRate = saleAR.ExchangeRate,
                TotalPayment = (saleAR.TotalAmount - saleAR.AppliedAmount),
                CashDiscount = 0,
                Total = saleAR.TotalAmount,//(saleAR.TotalAmount - saleAR.AppliedAmount),//saleAR.SubTotal,
                TotalDiscount = 0,
                BalanceDue = (saleAR.TotalAmount - saleAR.AppliedAmount),
                Status = saleAR.Status,
                Date = saleAR.DueDate,
                PostingDate = saleAR.PostingDate,
                SysCurrency = syCurrency.ID,
                SysName = syCurrency.Description,
                LocalCurID = saleAR.LocalCurID,
                LocalSetRate = saleAR.LocalSetRate,
                ItemInvoice = $"{docType.Code}-{saleAR.InvoiceNumber}"
            };

            if (saleAR.TotalAmount <= saleAR.AppliedAmount)
            {
                saleAR.Status = "close";
            }
            foreach (var amount in saleAR.SaleARDetails)
            {
                if (amount.UnitPrice == 0)
                {
                    saleAR.Status = "open";

                }
            }

            if (payment != null)
            {
                payment.Applied_Amount = saleAR.AppliedAmount;
                payment.BalanceDue = saleAR.TotalAmount - saleAR.AppliedAmount;
                //payment.TotalPayment -= saleAR.AppliedAmount;
                payment.TotalPayment = saleAR.TotalAmount - saleAR.AppliedAmount;
                payment.Status = saleAR.Status;
                var paymentDetails = _context.IncomingPaymentDetails.Where(ipd => ipd.DocumentNo == payment.DocumentNo);
                foreach (var pd in paymentDetails)
                {
                    pd.Delete = true;
                }
                _context.IncomingPaymentCustomers.Update(payment);
            }
            else
            {
                _context.IncomingPaymentCustomers.Add(ipcustomer);
            }

            _context.SaveChanges();
            return true;
        }

        [HttpPost]
        public IActionResult SaveDraft(string draftAR)
        {
            ModelMessage msg = new();
            DraftAR draft_AR = JsonConvert.DeserializeObject<DraftAR>(draftAR, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            List<ItemsReturn> returns = new();
            var Dcheck = _context.DraftARs.AsNoTracking();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            draft_AR.ChangeLog = DateTime.Now;
          
            draft_AR.UserID = GetUserID();
            draft_AR.Status = "open";
            if (draft_AR.DraftName == "")
            {
                ModelState.AddModelError("DraftName", _culLocal["Input Draft Name!"]);
                return Ok(msg.Bind(ModelState));
            }
            if (draft_AR.BranchID ==0)
            {
                ModelState.AddModelError("BranchID", _culLocal["Input Draft Branch!"]);
                return Ok(msg.Bind(ModelState));
            }
            if (draft_AR.DraftID == 0)
            {
                if (Dcheck.Any(i => i.DraftName == draft_AR.DraftName && i.DocTypeID == draft_AR.DocTypeID && !i.Remove))
                {
                    ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            else
            {
                var updateDraft = Dcheck.FirstOrDefault(i => i.DraftName == draft_AR.DraftName);
                if (updateDraft == null)
                {
                    if (Dcheck.Any(i => i.DraftName == draft_AR.DraftName))
                    {
                        ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                        return Ok(msg.Bind(ModelState));
                    }
                }
                if (Dcheck.Any(i => i.DraftID != draft_AR.DraftID && i.DraftName == draft_AR.DraftName && i.DocTypeID == draft_AR.DocTypeID && !i.Remove))
                {
                    ModelState.AddModelError("DraftName", _culLocal["Draft Name Already Have!"]);
                    return Ok(msg.Bind(ModelState));
                }
            }
            if (ModelState.IsValid)
            {
                draft_AR.LocalCurID = GetCompany().LocalCurrencyID;
                draft_AR.LocalSetRate = localSetRate;
                draft_AR.CompanyID = GetCompany().ID;
                _context.DraftARs.Update(draft_AR);
                _context.SaveChanges();
                var freight = draft_AR.FreightSalesView;
                if (freight != null)
                {
                    //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                    freight.SaleID = draft_AR.DraftID;
                    freight.SaleType = SaleCopyType.DraftSAleAR;
                    freight.OpenAmountReven = freight.AmountReven;
                    _context.FreightSales.Update(freight);
                    _context.SaveChanges();
                }
                ModelState.AddModelError("success", _culLocal["Item save successfully."]);
                msg.Approve();
            }
            return Ok(msg.Bind(ModelState));
        }
        public IActionResult DisplayDraft()
        {
            var DOCAR = _context.DocumentTypes.FirstOrDefault(s => s.Code == "IN");
            var AR = _context.DraftARs.Where(s => s.DocTypeID == DOCAR.ID && !s.Remove).ToList();
            var data = (from df in AR
                        join cur in _context.Currency on df.SaleCurrencyID equals cur.ID
                        select new DraftDataViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            BalanceDue = df.TotalAmount.ToString(),
                            CurrencyName = cur.Description,
                            DocType = DOCAR.Code,
                            DraftName = df.DraftName,
                            DraftID = df.DraftID,
                            PostingDate = df.PostingDate.ToString("dd-MM-yyyy"),
                            Remarks = df.Remarks,
                            SubTotal = df.SubTotal.ToString(),
                            SeriesDetailID = df.SeriesDID,
                        }).ToList();
            return Ok(data);
        }
        public IActionResult DisplayDraftCM()
        {
            var DOCCM = _context.DocumentTypes.FirstOrDefault(s => s.Code == "CN");
            var CM = _context.DraftARs.Where(s => s.DocTypeID == DOCCM.ID && !s.Remove).ToList();
            var data = (from df in CM
                        join cur in _context.Currency on df.SaleCurrencyID equals cur.ID
                        select new DraftDataViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            BalanceDue = df.TotalAmount.ToString(),
                            CurrencyName = cur.Description,
                            DocType = DOCCM.Code,
                            DraftName = df.DraftName,
                            DraftID = df.DraftID,
                            PostingDate = df.PostingDate.ToString("dd-MM-yyyy"),
                            Remarks = df.Remarks,
                            SubTotal = df.SubTotal.ToString(),
                            SeriesDetailID = df.SeriesDID,
                        }).ToList();
            return Ok(data);
        }
        [HttpGet]
        public IActionResult FindDraft(string draftname, int draftARId)
        {
            var data = _isale.FindDraftAsync(draftname, draftARId, GetCompany().ID);
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveDraft(int id)
        {
            await _isale.RemoveDraft(id);
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateSaleAR(
            string data,
            string seriesJE,
            string ardownpayment,
            string _type,
            string serial, string batch
            )
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            if (seriesJE == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }
            SaleAR saleAR = JsonConvert.DeserializeObject<SaleAR>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Series seriesje = JsonConvert.DeserializeObject<Series>(seriesJE, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            List<SaleARDPINCN> ards = JsonConvert.DeserializeObject<List<SaleARDPINCN>>(ardownpayment, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == saleAR.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == saleAR.DocTypeID).FirstOrDefault();
            var series_JE = _context.Series.FirstOrDefault(w => w.ID == seriesje.ID);
            var wh = _context.Warehouses.Find(saleAR.WarehouseID) ?? new Warehouse();
            saleAR.ChangeLog = DateTime.Now;
            saleAR.UserID = GetUserID();
           
            string type = "SaleAR";
            if (saleAR.TotalAmount < 0)
            {
                ModelState.AddModelError("TotalAmount", _culLocal["TotalAmount cannot be less than 0!"]);
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }
            // Checking Serial Batch //
            List<SerialNumber> _serialNumber = new();
            List<SerialNumber> serialNumber = new();
            List<BatchNo> _batchNoes = new();
            List<BatchNo> batchNoes = new();
            if (!string.IsNullOrEmpty(saleAR.RefNo))
            {
                bool isRefExisted = _context.SaleARs.AsNoTracking().Any(i => i.RefNo == saleAR.RefNo);
                if (isRefExisted)
                {
                    ModelState.AddModelError("RefNo", $"Transaction with Customer Ref No. \"{saleAR.RefNo}\" already done.");
                }
            }
            // if (saleAR.AppliedAmount != 0 && saleAR.GLAccID == 0)
            // {
            //     ModelState.AddModelError("master", "Please choose Account.");

            // }
            // if (saleAR.AppliedAmount == 0 && saleAR.GLAccID != 0)
            // {
            //     ModelState.AddModelError("master", "Please input Apply Amount.");

            // }

            ValidateSummary(saleAR, saleAR.SaleARDetails);
            CheckTaxAcc(saleAR.SaleARDetails);
            var g = _isale.GetAllGroupDefind().ToList();
            if (saleAR.SARID > 0)
            {
                ModelState.AddModelError("saleAR", _culLocal["Item cannot be chanage."]);
            }

            if (saleAR.AppliedAmount > saleAR.TotalAmount)
            {
                ModelState.AddModelError("saleAppliedAmount", _culLocal["Applied amount cannot exceeds total amount."]);
            }
            foreach (var dt in saleAR.SaleARDetails)
            {
                var factor = g.FirstOrDefault(w => w.GroupUoMID == dt.GUomID && w.AltUOM == dt.UomID).Factor;
                dt.Factor = factor;
                if (dt.SARDID <= 0)
                {
                    dt.PrintQty = dt.Qty;
                    if (saleAR.CopyType > 0)
                    {
                        dt.Qty = dt.OpenQty;
                    }
                    else
                    {
                        dt.OpenQty = dt.Qty;
                    }
                }
                else
                {
                    dt.PrintQty = dt.Qty - dt.PrintQty;
                }
                dt.EditeQty = dt.OpenQty;
            }

            var _itemReturneds = CheckStock(saleAR, saleAR.SaleARDetails.ToList(), "SARDID").ToList();
            List<ItemsReturn> returns = new();
            List<ItemsReturn> itemSBOnly = _itemReturneds.Where(i => i.IsSerailBatch && i.InStock > 0 && !i.IsBOM).ToList();
            if (wh.IsAllowNegativeStock)
            {
                returns = _itemReturneds.Where(i => i.InStock <= 0 && i.IsSerailBatch).ToList();
            }
            else
            {
                returns = _itemReturneds.Where(i => i.OrderQty > i.InStock).ToList();
            }
            if (saleAR.CopyType != SaleCopyType.Delivery)
            {
                if (returns.Any())
                {
                    int count = 0;
                    foreach (var ir in returns)
                    {
                        ModelState.AddModelError("itemReturn" + count.ToString(), _culLocal["The item &lt;&lt;" + ir.KhmerName + "&gt;&gt; has stock left " + ir.InStock + "."]);
                        count++;
                    }
                    if (count > 0) return Ok(new { Model = msg.Bind(ModelState), ItemReturn = returns });
                }
            }

            using var t = _context.Database.BeginTransaction();
            if (ModelState.IsValid)
            {
                if (_type != "DN" && itemSBOnly.Count > 0)
                {
                    serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }) : serialNumber;

                    _saleSerialBatch.CheckItemSerail(saleAR, saleAR.SaleARDetails.ToList(), serialNumber);
                    serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in serialNumber.ToList())
                    {
                        foreach (var i in saleAR.SaleARDetails.ToList())
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
                    batchNoes = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }) : batchNoes;
                    _saleSerialBatch.CheckItemBatch(saleAR, saleAR.SaleARDetails.ToList(), batchNoes);
                    batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                    foreach (var j in batchNoes.ToList())
                    {
                        foreach (var i in saleAR.SaleARDetails.ToList())
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
                seriesDetail.Number = seriesDN.NextNo;
                seriesDetail.SeriesID = seriesDN.ID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDN.NextNo;
                long No = long.Parse(Sno);
                saleAR.InvoiceNo = seriesDN.NextNo;
                saleAR.InvoiceNumber = seriesDN.NextNo;
                seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                if (No > long.Parse(seriesDN.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Sale A/R Invoice has reached the limitation!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                switch (saleAR.CopyType)
                {
                    case SaleCopyType.Quotation:
                        var copyKeyQ = saleAR.CopyKey.Split("-")[1];
                        var quoteMaster = _context.SaleQuotes.Include(q => q.SaleQuoteDetails)
                            .FirstOrDefault(m => string.Compare(m.InvoiceNumber, copyKeyQ) == 0);
                        UpdateSourceCopy(saleAR.SaleARDetails, quoteMaster, quoteMaster.SaleQuoteDetails, SaleCopyType.Quotation);
                        type = "Quotation"; // SQ
                        break;
                    case SaleCopyType.Order:
                        string copyKeyO = "";

                        copyKeyO = saleAR.CopyKey.Split("-")[1];
                        var docType = saleAR.CopyKey.Split(new Char[] { '-', ':' })[1];

                        if (docType == "SQ")
                        {
                            copyKeyO = saleAR.CopyKey.Split(new Char[] { '-', '/' })[3];
                        }
                        var orderMaster = _context.SaleOrders.Include(o => o.SaleOrderDetails)
                            .FirstOrDefault(m => string.Compare(m.InvoiceNumber, copyKeyO) == 0);
                        UpdateSourceCopy(saleAR.SaleARDetails, orderMaster, orderMaster.SaleOrderDetails, SaleCopyType.Order);
                        type = "Order"; //SO
                        break;
                    case SaleCopyType.Delivery:
                        string copyKeyD = "";

                        copyKeyD = saleAR.CopyKey.Split("-")[1];
                        var docTypeD = saleAR.CopyKey.Split(new Char[] { '-', ':' })[1];

                        if (docTypeD == "SQ" || docTypeD == "SO")
                        {
                            if (docTypeD == "SQ")
                            {
                                copyKeyD = saleAR.CopyKey.Split(new Char[] { '-', '/' })[5];
                            }
                            else
                            {
                                copyKeyD = saleAR.CopyKey.Split(new Char[] { '-', '/' })[4];
                            }
                        }
                        var deliveryMaster = _context.SaleDeliveries.Include(o => o.SaleDeliveryDetails)
                            .FirstOrDefault(m => string.Compare(m.InvoiceNumber, copyKeyD) == 0);
                        UpdateSourceCopy(saleAR.SaleARDetails, deliveryMaster, deliveryMaster.SaleDeliveryDetails, SaleCopyType.Delivery);
                        type = "Delivery"; //DN
                        break;
                }
                if (saleAR.TotalAmount <= saleAR.AppliedAmount)
                {
                    saleAR.Status = "close";
                }
                if (saleAR.SARID == 0 && saleAR.CopyType == 0)
                {
                    saleAR.LocalSetRate = localSetRate;
                }
                saleAR.SeriesID = seriesDN.ID;
                saleAR.LocalCurID = GetCompany().LocalCurrencyID;
                saleAR.LocalSetRate = localSetRate;
                saleAR.SeriesDID = seriesDetailID;
                saleAR.CompanyID = GetCompany().ID;
                _context.SaleARs.Update(saleAR);
                _context.SaveChanges();
                var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                if (gldeter != null)
                {
                    if (gldeter.GLID == 0)
                    {
                        ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is missing!!"]);
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                }
                else
                {
                    ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is not set up yet!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                var freight = saleAR.FreightSalesView;
                if (freight != null)
                {
                    //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                    freight.SaleID = saleAR.SARID;
                    freight.SaleType = SaleCopyType.AR;
                    freight.OpenAmountReven = freight.AmountReven;
                    _context.FreightSales.Update(freight);
                    _context.SaveChanges();
                }
                if (SaleCopyType.Delivery == saleAR.CopyType)
                {
                    if (!_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                    {
                        _isalecopy.IssuseInStockSaleAR(saleAR, ards, gldeter, freight);
                    }
                }
                else
                {
                    if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                    {

                        _isale.IssuseInStockSaleARBasic(saleAR.SARID, type, ards, gldeter, freight, _serialNumber, _batchNoes);

                    }
                    else
                    {
                        _isale.IssuseInStockSaleAR(saleAR.SARID, type, ards, gldeter, freight, _serialNumber, _batchNoes);

                    }
                }
                //=======================================sale ar with deferienct cost============================
                List<SaleARDetail> saleARDetails = new();
                var arD = _context.SaleARDetails.Where(w => w.SARID == saleAR.SARID).ToList();
                var inv = _context.InventoryAudits.Where(w => w.SeriesDetailID == saleAR.SeriesDID);
                if (saleAR.BaseOnID == 0)
                {
                    foreach (var item in inv)
                    {
                        //var itemMas = _context.ItemMasterDatas.FirstOrDefault(w => w.ID == item.ItemID) ?? new ItemMasterData();
                        var saleD = arD.FirstOrDefault(w => w.ItemID == item.ItemID && w.LineID == item.LineID) ?? new SaleARDetail();
                        if (saleD.SARDID != 0)
                            saleARDetails.Add(new SaleARDetail
                            {
                                SARID = saleAR.SARID,
                                Cost = (item.Cost * saleD.Factor),
                                ItemID = item.ItemID,
                                LineID=saleD.LineID,
                                LoanPartnerID =saleD.LoanPartnerID,
                                ItemCode = saleD.ItemCode,
                                ItemNameKH = saleD.ItemNameKH,
                                ItemNameEN = saleD.ItemNameEN,
                                Qty = (item.Qty / saleD.Factor) * -1,
                                OpenQty = (item.Qty / saleD.Factor) * -1,
                                PrintQty = (item.Qty / saleD.Factor) * -1,
                                UomID = saleD.UomID,
                                UnitPrice = saleD.UnitPrice,
                                GUomID = saleD.GUomID,
                                UomName = saleD.UomName,
                                Factor = saleD.Factor,
                                CurrencyID = saleD.CurrencyID,
                                DisRate = saleD.DisRate,
                                DisValue = saleD.DisValue,
                                TypeDis = saleD.TypeDis,
                                VatRate = saleD.VatRate,
                                VatValue = saleD.VatValue,
                                Total = saleD.Total,
                                TotalWTaxSys = saleD.TotalWTaxSys,
                                ExpireDate = saleD.ExpireDate,
                                ItemType = saleD.ItemType,
                                Remarks = saleD.Remarks,
                                FinDisRate = saleD.FinDisRate,
                                FinDisValue = saleD.FinDisValue,
                                FinTotalValue = saleD.FinTotalValue,
                                TaxGroupID = saleD.TaxGroupID,
                                TaxOfFinDisValue = saleD.TaxOfFinDisValue,
                                TaxRate = saleD.TaxRate,
                                TaxValue = saleD.TaxValue,
                                TotalSys = saleD.TotalSys,
                                TotalWTax = saleD.TotalWTax,
                            });
                    }
                    if (saleARDetails.Count > 0)
                    {
                        _context.SaleARDetails.UpdateRange(saleARDetails);
                        _context.SaveChanges();
                        _context.SaleARDetails.RemoveRange(arD);
                        _context.SaveChanges();
                    }


                }

                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Invoice Journal Entry has reached the limitation!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                CreateIncomingPaymentCustomerBySaleAR(saleAR);

                t.Commit();
                ModelState.AddModelError("success", _culLocal["Item save successfully."]);
                msg.Approve();
            }
            return Ok(new { Model = msg.Bind(ModelState), ItemReturn = returns });
        }

        [HttpPost]
        public async Task<IActionResult> CancelSaleAR(string saleAr, int saleArId)
        {
            SaleAR saleAR = JsonConvert.DeserializeObject<SaleAR>(saleAr, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            var ardata = _context.SaleARs.Find(saleArId);
            if (ardata == null) ModelState.AddModelError("ardata", _culLocal["A/R invoice could not found"]);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == saleAR.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == saleAR.DocTypeID).FirstOrDefault();
            List<SerialNumber> serails = new();
            List<BatchNo> batches = new();
            foreach (var i in saleAR.SaleARDetails.ToList())
            {
                var item = _context.ItemMasterDatas.Find(i.ItemID) ?? new ItemMasterData();
                if (item.ManItemBy == ManageItemBy.SerialNumbers)
                {
                    var serialNumberSelected = await _saleSerialBatch.GetSerialDetialsCopyAsync(i.ItemID, saleArId, saleAR.WarehouseID, TransTypeWD.AR);
                    serails.Add(new SerialNumber
                    {
                        SerialNumberSelected = serialNumberSelected
                    });
                }
                if (item.ManItemBy == ManageItemBy.Batches)
                {
                    var batchSelected = await _saleSerialBatch.GetBatchNoDetialsCopyAsync(i.ItemID, i.UomID, saleArId, saleAR.WarehouseID, TransTypeWD.AR);
                    batches.Add(new BatchNo
                    {
                        BatchNoSelected = batchSelected
                    });
                }
            }
            if (ModelState.IsValid)
            {
                using var t = _context.Database.BeginTransaction();
                seriesDetail.Number = seriesDN.NextNo;
                seriesDetail.SeriesID = seriesDN.ID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDN.NextNo;
                long No = long.Parse(Sno);
                saleAR.InvoiceNo = seriesDN.NextNo;
                saleAR.InvoiceNumber = seriesDN.NextNo;
                seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                if (No > long.Parse(seriesDN.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Sale A/R Invoice has reached the limitation!!"]);
                    return Ok(msg.Bind(ModelState));
                }
                var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w => w.InvoiceNumber == ardata.InvoiceNumber && w.SeriesID == ardata.SeriesID);
                var _freight = _context.FreightSales.FirstOrDefault(i => i.SaleID == ardata.SARID && i.SaleType == SaleCopyType.AR);
                if (_freight != null)
                {
                    _freight.OpenAmountReven = 0;
                    ardata.FreightAmount -= saleAR.FreightAmount;
                    ardata.FreightAmountSys = ardata.FreightAmount * (decimal)saleAR.ExchangeRate;
                    _context.FreightSales.Update(_freight);
                }
                ardata.AppliedAmount = saleAR.AppliedAmount;
                if (incomingCus != null)
                {
                    incomingCus.Applied_Amount = incomingCus.Total;
                    incomingCus.BalanceDue = 0;
                    incomingCus.TotalPayment = 0;
                    incomingCus.Status = "close";
                    _context.IncomingPaymentCustomers.Update(incomingCus);
                }

                _context.SaveChanges();
                var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
                var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
                if (seriesIn == null)
                {
                    ModelState.AddModelError("SeriesIn", "Series Incoming Payment has No Data or Default One!!");
                    return Ok(msg.Bind(ModelState));
                }
                if (paymentMean == null)
                {
                    ModelState.AddModelError("SeriesIn", "Series Payment Means has No Data or Default One!!");
                    return Ok(msg.Bind(ModelState));
                }


                saleAR.Status = "close";
                saleAR.SeriesID = seriesDN.ID;
                saleAR.SeriesDID = seriesDetailID;
                ardata.Status = "cancel";
                _context.SaleARs.Update(saleAR);
                _context.SaleARs.Update(ardata);
                _context.SaveChanges();
                var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                if (gldeter != null)
                {
                    if (gldeter.GLID == 0)
                    {
                        ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is missing!!"]);
                        return Ok(msg.Bind(ModelState));
                    }
                }
                else
                {
                    ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is not set up yet!!"]);
                    return Ok(msg.Bind(ModelState));
                }
                _isale.IssuseCancelSaleAr(saleAR, serails, batches, gldeter);

                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Invoice Journal Entry has reached the limitation!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                t.Commit();
                ModelState.AddModelError("success", _culLocal["A/R Invoice has been cancelled successfully!"]);
                msg.Approve();
                return Ok(msg.Bind(ModelState));
            }
            return Ok(msg.Bind(ModelState));
        }


        [HttpPost]
        public async Task<IActionResult> CancelSaleAREdit(string saleArEdited, int saleArId)
        {
            SaleAREdite saleAredit = JsonConvert.DeserializeObject<SaleAREdite>(saleArEdited, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            var ardata = _context.SaleAREdites.Find(saleArId);
            if (ardata == null) ModelState.AddModelError("ardata", _culLocal["A/R invoice could not found"]);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == saleAredit.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == saleAredit.DocTypeID).FirstOrDefault();
            List<SerialNumber> serails = new();
            List<BatchNo> batches = new();
            foreach (var i in saleAredit.SaleAREditeDetails.ToList())
            {
                var item = _context.ItemMasterDatas.Find(i.ItemID) ?? new ItemMasterData();
                if (item.ManItemBy == ManageItemBy.SerialNumbers)
                {
                    var serialNumberSelected = await _saleSerialBatch.GetSerialDetialsCopyAsync(i.ItemID, saleArId, saleAredit.WarehouseID, TransTypeWD.AR_Edit);
                    serails.Add(new SerialNumber
                    {
                        SerialNumberSelected = serialNumberSelected
                    });
                }
                if (item.ManItemBy == ManageItemBy.Batches)
                {
                    var batchSelected = await _saleSerialBatch.GetBatchNoDetialsCopyAsync(i.ItemID, i.UomID, saleArId, saleAredit.WarehouseID, TransTypeWD.AR_Edit);
                    batches.Add(new BatchNo
                    {
                        BatchNoSelected = batchSelected
                    });
                }
            }
            if (ModelState.IsValid)
            {
                using var t = _context.Database.BeginTransaction();
                seriesDetail.Number = seriesDN.NextNo;
                seriesDetail.SeriesID = seriesDN.ID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDN.NextNo;
                long No = long.Parse(Sno);
                saleAredit.InvoiceNo = seriesDN.NextNo;
                saleAredit.InvoiceNumber = seriesDN.NextNo;
                seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                if (No > long.Parse(seriesDN.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Sale A/R Invoice has reached the limitation!!"]);
                    return Ok(msg.Bind(ModelState));
                }
                var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w => w.InvoiceNumber == ardata.InvoiceNumber && w.SeriesID == ardata.SeriesID);
                var _freight = _context.FreightSales.FirstOrDefault(i => i.SaleID == ardata.SARID && i.SaleType == SaleCopyType.AR);
                if (_freight != null)
                {
                    _freight.OpenAmountReven = 0;
                    ardata.FreightAmount -= saleAredit.FreightAmount;
                    ardata.FreightAmountSys = ardata.FreightAmount * (decimal)saleAredit.ExchangeRate;
                    _context.FreightSales.Update(_freight);
                }
                ardata.AppliedAmount = saleAredit.AppliedAmount;
                if (incomingCus != null)
                {
                    incomingCus.Applied_Amount = incomingCus.Total;
                    incomingCus.BalanceDue = 0;
                    incomingCus.TotalPayment = 0;
                    incomingCus.Status = "close";
                    _context.IncomingPaymentCustomers.Update(incomingCus);
                }

                _context.SaveChanges();
                var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
                var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
                if (seriesIn == null)
                {
                    ModelState.AddModelError("SeriesIn", "Series Incoming Payment has No Data or Default One!!");
                    return Ok(msg.Bind(ModelState));
                }
                if (paymentMean == null)
                {
                    ModelState.AddModelError("SeriesIn", "Series Payment Means has No Data or Default One!!");
                    return Ok(msg.Bind(ModelState));
                }


                saleAredit.Status = "close";
                saleAredit.SeriesID = seriesDN.ID;
                saleAredit.SeriesDID = seriesDetailID;
                ardata.Status = "cancel";
                _context.SaleAREdites.Update(saleAredit);
                _context.SaleAREdites.Update(ardata);
                _context.SaveChanges();
                var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                if (gldeter != null)
                {
                    if (gldeter.GLID == 0)
                    {
                        ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is missing!!"]);
                        return Ok(msg.Bind(ModelState));
                    }
                }
                else
                {
                    ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is not set up yet!!"]);
                    return Ok(msg.Bind(ModelState));
                }
                _isale.IssuseCancelSaleArEdit(saleAredit, serails, batches, gldeter);

                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Invoice Journal Entry has reached the limitation!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                t.Commit();
                ModelState.AddModelError("success", _culLocal["A/R Invoice has been cancelled successfully!"]);
                msg.Approve();
                return Ok(msg.Bind(ModelState));
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult FindSaleAR(string number, int seriesID)
        {
            var list = _isale.FindSaleAR(number, seriesID, GetCompany().ID);
            if (list.SaleAR != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        [HttpGet]
        public IActionResult GetSaleARByInvoiceNo(string invoiceNo, int seriesID)
        {
            var number = invoiceNo.Contains("-") ? invoiceNo.Split("-")[1] : invoiceNo;
            var ARMaster = _context.SaleARs.Include(o => o.SaleARDetails).FirstOrDefault(m => string.Compare(m.InvoiceNumber, number) == 0 && m.SeriesID == seriesID) ?? new Models.Sale.SaleAR();
            return Ok(ARMaster);
        }
        #endregion
        //--------------------------------------------// End Sale AR //------------------------------------//
        #region Sale A/R Edit
        [Privilege("ST0010")]
        public IActionResult SaleAREdite()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale AR Edite";
            ViewBag.Sale = "show";
            ViewBag.SaleAREdit = "highlight";
            var seriesJE = (from dt in _context.DocumentTypes.Where(i => i.Code == "JE")
                            join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                            }).ToList();
            return View(new { seriesIN = _utility.GetSeries("IN"), seriesJE, genSetting = _utility.GetGeneralSettingAdmin() });
        }


        [Privilege("ST0010")]
        public IActionResult SaleAREditeHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale AREdite History";
            ViewBag.Sale = "show";
            ViewBag.SaleAREdit = "highlight";
            ViewBag.Devlivery = new SelectList(_context.Employees.Where(i => !i.Delete), "ID", "Name");
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();
            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.AREdite).Where(h => GetValidDate(h.PostingDate).CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>(),
                Templateurl = _utility.PrintTemplateUrl()
            };
            return View(history);
        }

        [HttpPost]
        public IActionResult CreateUpdateSaleAREdite(string data, string seriesJE, string ardownpayment, string _type, string serial, string batch)
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            if (seriesJE == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }
            SaleAREdite saleAR = JsonConvert.DeserializeObject<SaleAREdite>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Series seriesje = JsonConvert.DeserializeObject<Series>(seriesJE, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            List<SaleARDPINCN> ards = JsonConvert.DeserializeObject<List<SaleARDPINCN>>(ardownpayment, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });

            if (!saleAR.SaleAREditeDetails.Any())
            {
                ModelState.AddModelError("saleAREditDetail", "Required at least one item!");
            }
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == saleAR.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == saleAR.DocTypeID).FirstOrDefault();
            var series_JE = _context.Series.FirstOrDefault(w => w.ID == seriesje.ID);
            var wh = _context.Warehouses.Find(saleAR.WarehouseID) ?? new Warehouse();
            var saleEdit = _context.SaleAREdites.AsNoTracking().FirstOrDefault(s => s.SARID == saleAR.SARID) ?? new SaleAREdite();
            saleAR.ChangeLog = DateTime.Now;
            saleAR.UserID = GetUserID();
            
            string type = "SaleAREdite";
            bool newAREdit = true;
            if (saleAR.SARID > 0)
            {
                newAREdit = false;
                if (saleAR.TotalAmount < saleAR.AppliedAmount)
                {
                    // var aredt= _context.SaleAREdites.FirstOrDefault(s=> s.SARID ==saleAR.SARID)?? new SaleAREdite();
                    // double balancedue=(aredt.VatValue+(double)aredt.SubTotalAfterDis+(double)aredt.FreightAmount)-aredt.AppliedAmount;

                    ModelState.AddModelError("TotalAmount", "TotalAmount < AppliedAmount can't do A/R Invoice Editale");
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                else
                {
                    saleAR.Status = "open";
                }


            }
            if (saleAR.TotalAmount < 0)
            {
                ModelState.AddModelError("TotalAmount", _culLocal["TotalAmount cannot be less than 0!"]);
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }
            // Checking Serial Batch //
            List<SerialNumber> _serialNumber = new();
            List<SerialNumber> serialNumber = new();
            List<BatchNo> _batchNoes = new();
            List<BatchNo> batchNoes = new();

            ValidateSummary(saleAR, saleAR.SaleAREditeDetails);
            CheckTaxAcc(saleAR.SaleAREditeDetails);
            var g = _isale.GetAllGroupDefind().ToList();
            if (saleAR.AppliedAmount > saleAR.TotalAmount)
            {
                ModelState.AddModelError("saleAppliedAmount", _culLocal["Applied amount cannot exceeds total amount."]);
            }
            foreach (var dt in saleAR.SaleAREditeDetails)
            {
                if (dt.SARDID == 0)
                {
                    dt.Status = AREDetailStatus.New;
                }
                else
                {
                    dt.Status = AREDetailStatus.Old;
                }
                var factor = g.FirstOrDefault(w => w.GroupUoMID == dt.GUomID && w.AltUOM == dt.UomID).Factor;
                dt.Factor = factor;
                if (dt.SARDID <= 0)
                {
                    dt.PrintQty = dt.Qty;
                    if (saleAR.CopyType > 0)
                    {
                        dt.Qty = dt.OpenQty;
                    }
                    else
                    {
                        dt.OpenQty = dt.Qty;
                    }
                    dt.EditeQty = dt.Qty;
                }
                else
                {
                    var ardqty = _context.SaleAREditeDetails.AsNoTracking().Where(s => s.SARDID == dt.SARDID);
                    foreach (var editqty in ardqty)
                    {
                        dt.PrintQty = editqty.Qty;
                        //if (dt.Qty > editqty.Qty)
                        //{
                        //    dt.EditeQty = dt.Qty - editqty.Qty;
                        //}
                        //else if (dt.Qty < editqty.Qty)
                        //{
                        //    dt.EditeQty = editqty.Qty - dt.Qty;
                        //}
                        //else if (dt.Qty == editqty.Qty)
                        //{
                        //    dt.EditeQty = -1;
                        //}
                        dt.EditeQty = dt.Qty - editqty.Qty;
                    }
                }
            }
            var _itemReturneds = CheckStock(saleAR, saleAR.SaleAREditeDetails.ToList(), "SARDID").ToList();
            List<ItemsReturn> returns = new();
            List<ItemsReturn> itemSBOnly = _itemReturneds.Where(i => i.IsSerailBatch && i.InStock > 0 && !i.IsBOM).ToList();
            if (wh.IsAllowNegativeStock)
            {
                returns = _itemReturneds.Where(i => i.InStock <= 0 && i.IsSerailBatch).ToList();
            }
            else
            {
                returns = _itemReturneds.ToList();
            }
            if (returns.Any())
            {
                int count = 0;
                foreach (var ir in returns)
                {
                    ModelState.AddModelError("itemReturn" + count.ToString(), _culLocal["The item &lt;&lt;" + ir.KhmerName + "&gt;&gt; has stock left " + ir.InStock + "."]);
                    count++;
                }
                if (count > 0)
                    return Ok(new { Model = msg.Bind(ModelState), ItemReturn = returns });
            }
            try
            {
                using var t = _context.Database.BeginTransaction();
                if (ModelState.IsValid)
                {
                    if (_type != "DN" && itemSBOnly.Count > 0)
                    {
                        serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) : serialNumber;

                        _saleSerialBatch.CheckItemSerail(saleAR, saleAR.SaleAREditeDetails.ToList(), serialNumber);
                        serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                        foreach (var j in serialNumber.ToList())
                        {
                            foreach (var i in saleAR.SaleAREditeDetails.ToList())
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
                        batchNoes = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) : batchNoes;
                        _saleSerialBatch.CheckItemBatch(saleAR, saleAR.SaleAREditeDetails.ToList(), batchNoes);
                        batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                        foreach (var j in batchNoes.ToList())
                        {
                            foreach (var i in saleAR.SaleAREditeDetails.ToList())
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

                    if (saleAR.SARID == 0)
                    {
                        seriesDetail.Number = seriesDN.NextNo;
                        seriesDetail.SeriesID = seriesDN.ID;
                        _context.SeriesDetails.Update(seriesDetail);
                        _context.SaveChanges();
                        var seriesDetailID = seriesDetail.ID;
                        string Sno = seriesDN.NextNo;
                        long No = long.Parse(Sno);
                        saleAR.InvoiceNo = seriesDN.NextNo;
                        saleAR.InvoiceNumber = seriesDN.NextNo;
                        seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                        if (No > long.Parse(seriesDN.LastNo))
                        {
                            ModelState.AddModelError("SumInvoice", _culLocal["Your Sale A/R Invoice has reached the limitation!!"]);
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        saleAR.SeriesID = seriesDN.ID;
                        saleAR.LocalCurID = GetCompany().LocalCurrencyID;
                        saleAR.LocalSetRate = localSetRate;
                        saleAR.SeriesDID = seriesDetailID;
                    }
                    else if (saleAR.SARID > 0)
                    {
                        saleAR.InvoiceNo = saleEdit.InvoiceNo;
                        saleAR.InvoiceNumber = saleEdit.InvoiceNumber;
                        saleAR.SeriesDID = saleEdit.SeriesDID;
                        saleAR.SeriesID = saleEdit.SeriesID;
                    }

                    switch (saleAR.CopyType)
                    {
                        case SaleCopyType.Quotation:
                            //var copyKeyQ = saleAR.CopyKey.Split("-")[1];
                            var quoteMaster = _context.SaleQuotes.Include(q => q.SaleQuoteDetails).FirstOrDefault(s => s.SQID == saleAR.BaseOnID) ?? new SaleQuote();
                            UpdateSourceCopy(saleAR.SaleAREditeDetails, quoteMaster, quoteMaster.SaleQuoteDetails, SaleCopyType.Quotation);
                            type = "Quotation"; // SQ
                            break;
                        case SaleCopyType.Order:

                            var orderMaster = _context.SaleOrders.Include(o => o.SaleOrderDetails)
                                .FirstOrDefault(m => m.SOID == saleAR.BaseOnID) ?? new SaleOrder();
                            UpdateSourceCopy(saleAR.SaleAREditeDetails, orderMaster, orderMaster.SaleOrderDetails, SaleCopyType.Order);
                            type = "Order"; //SO
                            break;
                        case SaleCopyType.Delivery:
                            var deliveryMaster = _context.SaleDeliveries.Include(o => o.SaleDeliveryDetails).FirstOrDefault(s => s.SDID == saleAR.BaseOnID) ?? new SaleDelivery();

                            UpdateSourceCopy(saleAR.SaleAREditeDetails, deliveryMaster, deliveryMaster.SaleDeliveryDetails, SaleCopyType.Delivery);
                            type = "Delivery"; //DN
                            break;
                    }
                    saleAR.Status = "open";
                    if (saleAR.SARID == 0 && saleAR.CopyType == 0)
                    {
                        saleAR.LocalSetRate = localSetRate;
                    }
                    saleAR.CompanyID = GetCompany().ID;
                    var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                    if (gldeter != null)
                    {
                        if (gldeter.GLID == 0)
                        {
                            ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is missing!!"]);
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is not set up yet!!"]);
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                    var freight = saleAR.FreightSalesView;
                    if (freight != null)
                    {
                        freight.SaleID = saleAR.SARID;
                        freight.SaleType = SaleCopyType.SaleAREdite;
                        freight.OpenAmountReven = freight.AmountReven;
                        _context.FreightSales.Update(freight);
                        _context.SaveChanges();
                    }
                    if (saleAR.CopyType == SaleCopyType.Delivery)
                    {
                        _context.SaleAREdites.Update(saleAR);
                        _context.SaveChanges();
                        _isalecopy.IssuseInstockSaleAREDT(saleAR, ards, gldeter, freight);

                    }
                    else
                    {
                        if (newAREdit)
                        {
                            _context.SaleAREdites.Update(saleAR);
                            _context.SaveChanges();
                            _isale.IssuseInStockSaleAREdit(saleAR.SARID, type, ards, gldeter, freight, _serialNumber, _batchNoes);
                        }
                        else
                        {
                            _isale.IssuseInStockSaleAREditOld(saleAR, type, ards, gldeter, freight, _serialNumber, _batchNoes);
                            _context.SaveChanges();
                        }

                    }
                    // SaleAREdit History                    
                    SaleAREditeHistory saleAREditeHistory = new();
                    saleAREditeHistory.SARID = saleAR.SARID;
                    saleAREditeHistory.CusID = saleAR.CusID;
                    saleAREditeHistory.RequestedBy = saleAR.RequestedBy;
                    saleAREditeHistory.ShippedBy = saleAR.ShippedBy;
                    saleAREditeHistory.ShipTo = saleAR.ShipTo;
                    saleAREditeHistory.ReceivedBy = saleAR.ReceivedBy;
                    saleAREditeHistory.BaseOnID = saleAR.BaseOnID;
                    saleAREditeHistory.BranchID = saleAR.BranchID;
                    saleAREditeHistory.WarehouseID = saleAR.WarehouseID;
                    saleAREditeHistory.UserID = saleAR.UserID;
                    saleAREditeHistory.SaleCurrencyID = saleAR.SaleCurrencyID;
                    saleAREditeHistory.CompanyID = saleAR.CompanyID;
                    saleAREditeHistory.DocTypeID = saleAR.DocTypeID;
                    saleAREditeHistory.SeriesID = saleAR.SeriesID;
                    saleAREditeHistory.SeriesDID = saleAR.SeriesDID;
                    saleAREditeHistory.InvoiceNumber = saleAR.InvoiceNumber;
                    saleAREditeHistory.RefNo = saleAR.RefNo;
                    saleAREditeHistory.InvoiceNo = saleAR.InvoiceNo;
                    saleAREditeHistory.ExchangeRate = saleAR.ExchangeRate;
                    saleAREditeHistory.PostingDate = saleAR.PostingDate;
                    saleAREditeHistory.DueDate = saleAR.DueDate;
                    saleAREditeHistory.DeliveryDate = saleAR.DeliveryDate;
                    saleAREditeHistory.DocumentDate = saleAR.DocumentDate;
                    saleAREditeHistory.IncludeVat = saleAR.IncludeVat;
                    saleAREditeHistory.Status = saleAR.Status;
                    saleAREditeHistory.Remarks = saleAR.Remarks;
                    saleAREditeHistory.SubTotalBefDis = saleAR.SubTotalBefDis;
                    saleAREditeHistory.SubTotalBefDisSys = saleAR.SubTotalBefDisSys;
                    saleAREditeHistory.SubTotalAfterDis = saleAR.SubTotalAfterDis;
                    saleAREditeHistory.SubTotalAfterDisSys = saleAR.SubTotalAfterDisSys;
                    saleAREditeHistory.FreightAmount = saleAR.FreightAmount;
                    saleAREditeHistory.FreightAmountSys = saleAR.FreightAmountSys;
                    saleAREditeHistory.DownPayment = saleAR.DownPayment;
                    saleAREditeHistory.DownPaymentSys = saleAR.DownPaymentSys;
                    saleAREditeHistory.SubTotal = saleAR.SubTotal;
                    saleAREditeHistory.SubTotalSys = saleAR.SubTotalSys;
                    saleAREditeHistory.DisRate = saleAR.DisRate;
                    saleAREditeHistory.DisValue = saleAR.DisValue;
                    saleAREditeHistory.TypeDis = saleAR.TypeDis;
                    saleAREditeHistory.VatRate = saleAR.VatRate;
                    saleAREditeHistory.VatValue = saleAR.VatValue;
                    saleAREditeHistory.AppliedAmount = saleAR.AppliedAmount;
                    saleAREditeHistory.FeeNote = saleAR.FeeNote;
                    saleAREditeHistory.FeeAmount = saleAR.FeeAmount;
                    saleAREditeHistory.TotalAmount = saleAR.TotalAmount;
                    saleAREditeHistory.TotalAmountSys = saleAR.TotalAmountSys;
                    saleAREditeHistory.CopyType = saleAR.CopyType;
                    saleAREditeHistory.CopyKey = saleAR.CopyKey;
                    saleAREditeHistory.BasedCopyKeys = saleAR.BasedCopyKeys;
                    saleAREditeHistory.ChangeLog = saleAR.ChangeLog;
                    saleAREditeHistory.PriceListID = saleAR.PriceListID;
                    saleAREditeHistory.LocalCurID = saleAR.LocalCurID;
                    saleAREditeHistory.LocalSetRate = saleAR.LocalSetRate;
                    saleAREditeHistory.SaleEmID = saleAR.SaleEmID;
                    _context.Add(saleAREditeHistory);
                    _context.SaveChanges();
                    foreach (var ard in saleAR.SaleAREditeDetails)
                    {

                        SaleAREditeDetailHistory saleAREditeDetailHistory = new();
                        saleAREditeDetailHistory.SAREID = saleAREditeHistory.ID;
                        saleAREditeDetailHistory.LineID = ard.LineID;
                        saleAREditeDetailHistory.SaleCopyType = ard.SaleCopyType;
                        saleAREditeDetailHistory.SARDID = ard.SARDID;
                        saleAREditeDetailHistory.SARID = saleAREditeHistory.SARID;
                        saleAREditeDetailHistory.LineID = ard.LineID;
                        //saleAREditeDetailHistory.SARID = ard.SARID;
                        saleAREditeDetailHistory.SQDID = ard.SQDID;
                        saleAREditeDetailHistory.SODID = ard.SODID;
                        saleAREditeDetailHistory.SDDID = ard.SDDID;
                        saleAREditeDetailHistory.ItemID = ard.ItemID;
                        saleAREditeDetailHistory.TaxGroupID = ard.TaxGroupID;
                        saleAREditeDetailHistory.TaxRate = ard.TaxRate;
                        saleAREditeDetailHistory.TaxValue = ard.TaxValue;
                        saleAREditeDetailHistory.TaxOfFinDisValue = ard.TaxOfFinDisValue;
                        saleAREditeDetailHistory.FinTotalValue = ard.FinTotalValue;
                        saleAREditeDetailHistory.ItemCode = ard.ItemCode;
                        saleAREditeDetailHistory.ItemNameKH = ard.ItemNameKH;
                        saleAREditeDetailHistory.ItemNameEN = ard.ItemNameEN;
                        saleAREditeDetailHistory.Qty = ard.Qty;
                        saleAREditeDetailHistory.OpenQty = ard.OpenQty;
                        saleAREditeDetailHistory.EditeQty = ard.EditeQty;
                        saleAREditeDetailHistory.PrintQty = ard.PrintQty;
                        saleAREditeDetailHistory.GUomID = ard.GUomID;
                        saleAREditeDetailHistory.UomID = ard.UomID;
                        saleAREditeDetailHistory.UomName = ard.UomName;
                        saleAREditeDetailHistory.Factor = ard.Factor;
                        saleAREditeDetailHistory.Cost = ard.Cost;
                        saleAREditeDetailHistory.UnitPrice = ard.UnitPrice;
                        saleAREditeDetailHistory.DisRate = ard.DisRate;
                        saleAREditeDetailHistory.DisValue = ard.DisValue;
                        saleAREditeDetailHistory.FinDisRate = ard.FinDisRate;
                        saleAREditeDetailHistory.FinDisValue = ard.FinDisValue;
                        saleAREditeDetailHistory.TypeDis = ard.TypeDis;
                        saleAREditeDetailHistory.VatRate = ard.VatRate;
                        saleAREditeDetailHistory.VatValue = ard.VatValue;
                        saleAREditeDetailHistory.Total = ard.Total;
                        saleAREditeDetailHistory.TotalSys = ard.TotalSys;
                        saleAREditeDetailHistory.TotalWTax = ard.TotalWTax;
                        saleAREditeDetailHistory.TotalWTaxSys = ard.TotalWTaxSys;
                        saleAREditeDetailHistory.CurrencyID = ard.CurrencyID;
                        saleAREditeDetailHistory.ExpireDate = ard.ExpireDate;
                        saleAREditeDetailHistory.ItemType = ard.ItemType;
                        saleAREditeDetailHistory.Remarks = ard.Remarks;
                        saleAREditeDetailHistory.Delete = ard.Delete;
                        _context.Add(saleAREditeDetailHistory);
                        _context.SaveChanges();
                    }
                    // checking maximun Invoice

                    if (saleAR.CopyType != SaleCopyType.Delivery)
                    {
                        var arD = _context.SaleAREditeDetails.Where(w => w.SARID == saleAR.SARID).ToList();

                        var saleARDetails = (from i in _context.InventoryAudits.Where(w => w.SeriesDetailID == saleAR.SeriesDID)
                                             group new { i } by new { i.LineID, i.ItemID } into datas
                                             let datainv = datas.FirstOrDefault()
                                             let saleD = saleAR.SaleAREditeDetails.FirstOrDefault(w => w.ItemID == datainv.i.ItemID && w.LineID == datainv.i.LineID) ?? new SaleAREditeDetail()
                                             select new SaleAREditeDetail
                                             {
                                                 SARID = saleAR.SARID,
                                                 Cost = datainv.i.Cost * saleD.Factor,
                                                 ItemID = datainv.i.ItemID,
                                                 ItemCode = saleD.ItemCode,
                                                 ItemNameKH = saleD.ItemNameKH,
                                                 ItemNameEN = saleD.ItemNameEN,
                                                 Qty = datas.Sum(s => s.i.Qty) == 0 || saleD.Factor == 0 ? 0 : (datas.Sum(s => s.i.Qty) * -1) / saleD.Factor,
                                                 OpenQty = datas.Sum(s => s.i.Qty) == 0 || saleD.Factor == 0 ? 0 : (datas.Sum(s => s.i.Qty) * -1) / saleD.Factor,
                                                 PrintQty = datas.Sum(s => s.i.Qty) == 0 || saleD.Factor == 0 ? 0 : (datas.Sum(s => s.i.Qty) * -1) / saleD.Factor,
                                                 LineID = saleD.LineID,
                                                 SaleCopyType = saleD.SaleCopyType,
                                                 UomID = saleD.UomID,
                                                 UnitPrice = saleD.UnitPrice,
                                                 GUomID = saleD.GUomID,
                                                 UomName = saleD.UomName,
                                                 Factor = saleD.Factor,
                                                 CurrencyID = saleD.CurrencyID,
                                                 DisRate = saleD.DisRate,
                                                 DisValue = saleD.DisValue,
                                                 TypeDis = saleD.TypeDis,
                                                 VatRate = saleD.VatRate,
                                                 VatValue = saleD.VatValue,
                                                 Total = saleD.Total,
                                                 TotalWTaxSys = saleD.TotalWTaxSys,
                                                 ExpireDate = saleD.ExpireDate,
                                                 ItemType = saleD.ItemType,
                                                 Remarks = saleD.Remarks,
                                                 FinDisRate = saleD.FinDisRate,
                                                 FinDisValue = saleD.FinDisValue,
                                                 FinTotalValue = saleD.FinTotalValue,
                                                 TaxGroupID = saleD.TaxGroupID,
                                                 TaxOfFinDisValue = saleD.TaxOfFinDisValue,
                                                 TaxRate = saleD.TaxRate,
                                                 TaxValue = saleD.TaxValue,
                                                 TotalSys = saleD.TotalSys,
                                                 TotalWTax = saleD.TotalWTax,
                                             }).GroupBy(i => i.LineID).Select(i => i.FirstOrDefault()).ToList();
                        //var _saleARDetails = saleARDetails.Where(s => s.Qty < 0);
                        //saleARDetails.ToList().RemoveAll(s => s.Qty <= 0);
                        saleARDetails = saleARDetails.Where(s => s.Qty > 0).ToList();
                        if (saleARDetails.Count > 0)
                        {
                            _context.SaleAREditeDetails.RemoveRange(arD);
                            _context.SaveChanges();
                        }
                        _context.SaleAREditeDetails.UpdateRange(saleARDetails);
                        _context.SaveChanges();
                    }
                    var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                    var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                    if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", _culLocal["Your Invoice Journal Entry has reached the limitation!!"]);
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                    CreateIncomingPaymentCustomerBySaleAREdite(saleAR);
                    t.Commit();
                    ModelState.AddModelError("success", _culLocal["Item save successfully."]);
                    msg.Approve();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return Ok(new
            {
                Model = msg.Bind(ModelState),
                ItemReturn = returns
            });
        }

        [HttpGet]
        public IActionResult FindSaleAREdite(string number, int seriesID)
        {
            var list = _isale.FindSaleAREdit(number, seriesID, GetCompany().ID);
            if (list.SaleAR != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        private bool CreateIncomingPaymentCustomerBySaleAREdite(SaleAREdite saleAR)
        {
            string currencyName = _context.Currency.Find(saleAR.SaleCurrencyID).Description;
            SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
            var payment = _context.IncomingPaymentCustomers.FirstOrDefault(p => p.InvoiceNumber == saleAR.InvoiceNumber && p.SeriesID == saleAR.SeriesID);
            var docType = _context.DocumentTypes.Find(saleAR.DocTypeID);
            var em = _context.Employees.FirstOrDefault(i => i.ID == saleAR.SaleEmID) ?? new Employee();
            var user = _userModule.CurrentUser;
            if (payment != null)
            {
                if (saleAR.TotalAmount <= saleAR.AppliedAmount)
                {
                    payment.Status = "close";
                }
                else
                {
                    payment.Status = saleAR.Status;
                }
                payment.Applied_Amount = saleAR.AppliedAmount;
                payment.BalanceDue = saleAR.TotalAmount - saleAR.AppliedAmount;
                //payment.TotalPayment -= saleAR.AppliedAmount;
                payment.TotalPayment = saleAR.TotalAmount - saleAR.AppliedAmount;
                payment.Total = saleAR.TotalAmount - saleAR.AppliedAmount;
                var paymentDetails = _context.IncomingPaymentDetails.Where(ipd => ipd.DocumentNo == payment.DocumentNo);
                foreach (var pd in paymentDetails)
                {
                    pd.Delete = true;
                }
                _context.IncomingPaymentCustomers.Update(payment);
            }
            else if (saleAR.TotalAmount <= saleAR.AppliedAmount)
            {
                IncomingPaymentCustomer ipcustomer = new()
                {
                    IncomingPaymentCustomerID = 0,
                    EmID = em.ID,
                    EmName = em.Name,
                    CreatorID = user.ID,
                    CreatorName = user.Username,
                    CustomerID = saleAR.CusID,
                    BranchID = saleAR.BranchID,
                    WarehouseID = saleAR.WarehouseID,
                    DocTypeID = saleAR.DocTypeID,
                    SeriesID = saleAR.SeriesID,
                    SeriesDID = saleAR.SeriesDID,
                    CompanyID = saleAR.CompanyID,
                    InvoiceNumber = saleAR.InvoiceNumber,
                    CurrencyID = saleAR.SaleCurrencyID,
                    //DocumentNo = saleAR.InvoiceNo,
                    //DocumentType = GetTransactType(saleAR.InvoiceNo, saleAR.IncludeVat),
                    Applied_Amount = saleAR.AppliedAmount,
                    CurrencyName = currencyName,
                    ExchangeRate = saleAR.ExchangeRate,
                    TotalPayment = (saleAR.TotalAmount - saleAR.AppliedAmount),
                    CashDiscount = 0,
                    Total = (saleAR.TotalAmount - saleAR.AppliedAmount),//saleAR.SubTotal,
                    TotalDiscount = 0,
                    BalanceDue = (saleAR.TotalAmount - saleAR.AppliedAmount),
                    Status = "close",
                    Date = saleAR.DueDate,
                    PostingDate = saleAR.PostingDate,
                    SysCurrency = syCurrency.ID,
                    SysName = syCurrency.Description,
                    LocalCurID = saleAR.LocalCurID,
                    LocalSetRate = saleAR.LocalSetRate,
                    ItemInvoice = $"{docType.Code}-{saleAR.InvoiceNumber}",
                    Types = SaleCopyType.SaleAREdite.ToString(),// "ARED",
                };
                _context.IncomingPaymentCustomers.Add(ipcustomer);
            }
            else
            {
                IncomingPaymentCustomer ipcustomer = new()
                {
                    IncomingPaymentCustomerID = 0,
                    EmID = em.ID,
                    EmName = em.Name,
                    CreatorID = user.ID,
                    CreatorName = user.Username,
                    CustomerID = saleAR.CusID,
                    BranchID = saleAR.BranchID,
                    WarehouseID = saleAR.WarehouseID,
                    DocTypeID = saleAR.DocTypeID,
                    SeriesID = saleAR.SeriesID,
                    SeriesDID = saleAR.SeriesDID,
                    CompanyID = saleAR.CompanyID,
                    InvoiceNumber = saleAR.InvoiceNumber,
                    CurrencyID = saleAR.SaleCurrencyID,
                    //DocumentNo = saleAR.InvoiceNo,
                    //DocumentType = GetTransactType(saleAR.InvoiceNo, saleAR.IncludeVat),
                    Applied_Amount = saleAR.AppliedAmount,
                    CurrencyName = currencyName,
                    ExchangeRate = saleAR.ExchangeRate,
                    TotalPayment = (saleAR.TotalAmount - saleAR.AppliedAmount),
                    CashDiscount = 0,
                    Total = (saleAR.TotalAmount - saleAR.AppliedAmount),//saleAR.SubTotal,
                    TotalDiscount = 0,
                    BalanceDue = (saleAR.TotalAmount - saleAR.AppliedAmount),
                    Status = "open",
                    Date = saleAR.DueDate,
                    PostingDate = saleAR.PostingDate,
                    SysCurrency = syCurrency.ID,
                    SysName = syCurrency.Description,
                    LocalCurID = saleAR.LocalCurID,
                    LocalSetRate = saleAR.LocalSetRate,
                    ItemInvoice = $"{docType.Code}-{saleAR.InvoiceNumber}",
                    Types = SaleCopyType.SaleAREdite.ToString(),//"ARED",
                };
                _context.IncomingPaymentCustomers.Add(ipcustomer);
            }
            _context.SaveChanges();
            return true;
        }


        [HttpGet]
        public IActionResult GetSaleAREditByInvoiceNo(string invoiceNo, int seriesID)
        {
            var number = invoiceNo.Contains("-") ? invoiceNo.Split("-")[1] : invoiceNo;
            var ARMaster = _context.SaleAREdites.Include(s => s.SaleAREditeDetails).FirstOrDefault(s => string.Compare(s.InvoiceNumber, number) == 0 && s.SeriesID == seriesID);
            return Ok(ARMaster);
        }


        #endregion
        //--------------------------------------------// End Sale AR Edit //------------------------------------//
        //==========================Sart Service Contract====================
        [HttpGet]
        public IActionResult GetingCustomer(int? id)
        {
            if (id != null)
            {
                var cus = (from bp in _context.BusinessPartners.Where(x => x.ID == id)
                           select new BusinessPartner
                           {
                               ID = bp.ID,
                               Code = bp.Code,
                               Name = bp.Name,
                               ContactPeople = (from con in _context.ContactPersons.Where(x => x.BusinessPartnerID == bp.ID)
                                                select new ContactPerson
                                                {
                                                    ID = con.ID,
                                                    ContactID = con.ContactID,
                                                    FirstName = con.FirstName,
                                                    LastName = con.LastName,
                                                    Tel1 = con.Tel1,
                                                    SetAsDefualt = con.SetAsDefualt
                                                }).ToList() ?? new List<ContactPerson>()

                           }).FirstOrDefault();
                return Ok(cus);
                //return Ok(_context.BusinessPartners.FirstOrDefault(c => c.ID == id));
            }
            else
            {
                //var cus = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Customer").ToList();
                var cus = (from bp in _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Customer")
                           let pay = _context.PaymentTerms.Where(x => x.ID == bp.PaymentTermsID).FirstOrDefault() ?? new PaymentTerms()
                           //let ins =_context.Instaillments.Where(x=>x.ID==pay.InstaillmentID).FirstOrDefault() ?? new Instaillment()
                           select new BusinessPartner
                           {
                               ID = bp.ID,
                               PaymentTermsID = pay.ID,
                               Code = bp.Code,
                               Name = bp.Name,
                               Type = bp.Type,
                               Phone = bp.Phone,
                               Months = pay.Months,
                               Days = pay.Days,
                               ContactPeople = (from con in _context.ContactPersons.Where(x => x.BusinessPartnerID == bp.ID)
                                                select new ContactPerson
                                                {
                                                    ID = con.ID,
                                                    ContactID = con.ContactID,
                                                    FirstName = con.FirstName,
                                                    LastName = con.LastName,
                                                    Tel1 = con.Tel1,
                                                    SetAsDefualt = con.SetAsDefualt
                                                }).ToList() ?? new List<ContactPerson>()

                           }).ToList();

                return Ok(cus);
            }
        }
        public IActionResult GetContractTemplate()
        {
            var data = (from con in _context.Contracts
                        let setup = _context.SetupContractTypes.FirstOrDefault(x => x.ID == con.ContracType)
                        select new ContractTemplate
                        {
                            ID = con.ID,
                            Name = con.Name,
                            ContractName = setup.ContractType,
                            Description = con.Description
                        }
                        ).ToList();
            return Ok(data);
        }
        private List<TaxGroupViewModel> GetTaxGroups()
        {
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            return tgs;
        }
        public IActionResult ServiceContract(string number, int seriesId, int comId)
        {

            ViewBag.ServiceContract = "highlight";
            var seriesIN = (from dt in _context.DocumentTypes.Where(i => i.Code == "IN")
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

            var taxGroup = GetTaxGroups();
            taxGroup.Insert(0, new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            });
            var serviceContract = (from ar in _context.ServiceContracts.Where(x => x.InvoiceNumber == number)
                                   join war in _context.Warehouses on ar.WarehouseID equals war.ID
                                   join pr in _context.PriceLists on ar.PriceListID equals pr.ID
                                   join docType in _context.DocumentTypes on ar.DocTypeID equals docType.ID
                                   let sem = _context.Employees.FirstOrDefault(i => ar.SaleEmID == i.ID) ?? new Employee()
                                   let fs = _context.FreightSales.Where(i => i.SaleID == ar.ID && i.SaleType == SaleCopyType.AR).FirstOrDefault() ?? new FreightSale()
                                   let con = _context.Contracts.FirstOrDefault(x => x.ID == ar.ContractTemplateID) ?? new ContractTemplate()
                                   select new ServiceContractViewModel
                                   {
                                       BasedCopyKeys = ar.BasedCopyKeys,
                                       BasedOn = ar.ID,
                                       ContractName = con.Name,
                                       ContractStartDate = ar.ContractStartDate,
                                       ContractRenewalDate = ar.ContractRenewalDate,
                                       ContractENDate = ar.ContractENDate,
                                       ContractType = ar.ContractType,
                                       CopyKey = ar.CopyKey,
                                       CopyType = ar.CopyType,
                                       BranchID = ar.BranchID,
                                       ChangeLog = ar.ChangeLog,
                                       CompanyID = ar.CompanyID,
                                       CusID = ar.CusID,
                                       Remark = ar.Remark,
                                       PriListName = pr.Name,
                                       WareHouseName = war.Code,
                                       AdditionalContractNo = ar.AdditionalContractNo,
                                       AttchmentFiles = (from at in _context.AttchmentFiles.Where(x => x.ServiceContractID == ar.ID)
                                                         select new AttchmentFile
                                                         {

                                                             LineID = DateTime.Now.Ticks.ToString() + at.ID,
                                                             ID = at.ID,
                                                             TargetPath = at.TargetPath,
                                                             FileName = at.FileName,
                                                             AttachmentDate = at.AttachmentDate
                                                         }
                                                       ).ToList() ?? new List<AttchmentFile>(),
                                       DeliveryDate = ar.DueDate,
                                       DueDate = ar.DueDate,
                                       DisRate = (decimal)ar.DisRate,
                                       DisValue = (decimal)ar.DisValue,
                                       DocTypeID = ar.DocTypeID,
                                       DocumentDate = ar.DocumentDate,
                                       ExchangeRate = (decimal)ar.ExchangeRate,
                                       FreightAmount = ar.FreightAmount,
                                       FreightAmountSys = ar.FreightAmountSys,
                                       DownPayment = ar.DownPayment,
                                       DownPaymentSys = ar.DownPaymentSys,
                                       SaleEmID = ar.SaleEmID,
                                       SaleEmName = sem.Name ?? "",
                                       FreightSalesView = new FreightSaleView
                                       {
                                           AmountReven = fs.AmountReven == fs.OpenAmountReven ? fs.AmountReven : fs.OpenAmountReven,
                                           SaleID = ar.ID,
                                           ID = fs.ID,
                                           SaleType = fs.SaleType,
                                           TaxSumValue = fs.TaxSumValue,
                                           FreightSaleDetailViewModels = (from fsd in _context.FreightSaleDetails.Where(i => i.FreightSaleID == fs.ID)
                                                                          select new FreightSaleDetailViewModel
                                                                          {
                                                                              ID = fsd.ID,
                                                                              FreightSaleID = fsd.FreightSaleID,
                                                                              Amount = fsd.Amount,
                                                                              AmountWithTax = fsd.AmountWithTax,
                                                                              FreightID = fsd.FreightID,
                                                                              Name = fsd.Name,
                                                                              TaxGroup = fsd.TaxGroup,
                                                                              TaxGroupID = fsd.TaxGroupID,
                                                                              TaxGroups = GetTaxGroups(),
                                                                              TaxGroupSelect = taxGroup.Select(i => new SelectListItem
                                                                              {
                                                                                  Value = i.ID.ToString(),
                                                                                  Selected = fsd.TaxGroupID == i.ID,
                                                                                  Text = $"{i.Code}-{i.Name}"
                                                                              }).ToList(),
                                                                              TaxRate = fsd.TaxRate,
                                                                              TotalTaxAmount = fsd.TotalTaxAmount
                                                                          }).ToList(),
                                       },
                                       IncludeVat = ar.IncludeVat,
                                       InvoiceNo = $"{docType.Code}-{ar.InvoiceNumber}",
                                       InvoiceNumber = ar.InvoiceNumber,
                                       LocalCurID = ar.LocalCurID,
                                       LocalSetRate = (decimal)ar.LocalSetRate,
                                       PostingDate = ar.PostingDate,
                                       PriceListID = ar.PriceListID,
                                       RefNo = ar.RefNo,
                                       Remarks = ar.Remarks,
                                       SaleCurrencyID = ar.SaleCurrencyID,
                                       SeriesDID = ar.SeriesDID,
                                       SeriesID = ar.SeriesID,
                                       ID = ar.ID,
                                       Status = ar.Status,
                                       SubTotal = (decimal)ar.SubTotal,
                                       SubTotalAfterDis = ar.SubTotalAfterDis,
                                       SubTotalAfterDisSys = ar.SubTotalAfterDisSys,
                                       SubTotalBefDis = ar.SubTotalBefDis,
                                       SubTotalBefDisSys = ar.SubTotalBefDisSys,
                                       SubTotalSys = (decimal)ar.SubTotalSys,
                                       TotalAmount = (decimal)ar.TotalAmount,
                                       TotalAmountSys = (decimal)ar.TotalAmountSys,
                                       TypeDis = ar.TypeDis,
                                       UserID = ar.UserID,
                                       VatRate = (decimal)ar.VatRate,
                                       VatValue = (decimal)ar.VatValue,
                                       WarehouseID = ar.WarehouseID,
                                       AppliedAmount = (decimal)ar.AppliedAmount,
                                   }).ToList();

            var _tg = new TaxGroupViewModel
            {
                ID = 0,
                Name = "- Select --",
                Code = "",
                Rate = 0,
                Type = 0,
            };
            var tgs = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                       let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                       select new TaxGroupViewModel
                       {
                           ID = t.ID,
                           Name = t.Name,
                           Code = t.Code,
                           Effectivefrom = tgds.EffectiveFrom,
                           Rate = tgds.Rate,
                           Type = (int)t.Type,
                       }).ToList();
            tgs.Insert(0, _tg);
            var _sqd = (from sAr in serviceContract
                        join sArd in _context.ServiceContractDetails on sAr.ID equals sArd.ServiceContractID
                        join item in _context.ItemMasterDatas on sArd.ItemID equals item.ID
                        join cur in _context.Currency on sAr.SaleCurrencyID equals cur.ID
                        select new ServiceContractDetialViewModel
                        {
                            LineID = DateTime.Now.Ticks.ToString(),
                            FinDisRate = sArd.FinDisRate,
                            FinDisValue = sArd.FinDisValue,
                            FinTotalValue = sArd.FinTotalValue,
                            TaxOfFinDisValue = sArd.TaxOfFinDisValue,
                            ItemCode = sArd.ItemCode,
                            BarCode = item.Barcode,
                            Currency = cur.Description,
                            TotalSys = (decimal)sArd.TotalSys,
                            SQDID = sArd.SQDID,
                            SODID = sArd.SODID,
                            SDDID = sArd.SDDID,
                            ServiceContractIDD = sArd.ID,
                            ID = sAr.ID,
                            Cost = (decimal)sArd.Cost,
                            CurrencyID = cur.ID,
                            DisRate = (decimal)sArd.DisRate,
                            DisValue = (decimal)sArd.DisValue,
                            ItemNameEN = item.EnglishName,
                            ItemNameKH = item.KhmerName,
                            GUomID = item.GroupUomID,
                            ItemID = item.ID,
                            ItemType = item.Type,
                            Process = item.Process,
                            Qty = (decimal)sArd.Qty,
                            Factor = (decimal)sArd.Factor,
                            OpenQty = (decimal)sArd.OpenQty,
                            UnitPrice = (decimal)sArd.UnitPrice,
                            UomName = sArd.UomName,
                            UomID = sArd.UomID,
                            TaxGroupID = sArd.TaxGroupID,
                            TaxRate = sArd.TaxRate,
                            Remarks = sArd.Remarks,
                            TaxGroupList = tgs.Select(c => new SelectListItem
                            {
                                Value = c.ID.ToString(),
                                Text = $"{c.Code}-{c.Name}",
                                Selected = c.ID == sArd.TaxGroupID
                            }).ToList(),
                            TotalWTax = (decimal)sArd.TotalWTax,
                            TaxValue = sArd.TaxValue,
                            Total = (decimal)sArd.Total,
                            /// select List UoM ///
                            UoMs = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                    join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                    select new UOMSViewModel
                                    {
                                        BaseUoMID = GDU.BaseUOM,
                                        Factor = GDU.Factor,
                                        ID = UNM.ID,
                                        Name = UNM.Name
                                    }).Select(c => new SelectListItem
                                    {
                                        Value = c.ID.ToString(),
                                        Text = c.Name,
                                        Selected = c.ID == sArd.UomID
                                    }).ToList(),
                            /// List UoM ///
                            UoMsList = (from GDU in _context.GroupDUoMs.Where(i => i.GroupUoMID == item.GroupUomID)
                                        join UNM in _context.UnitofMeasures on GDU.AltUOM equals UNM.ID
                                        select new UOMSViewModel
                                        {
                                            BaseUoMID = GDU.BaseUOM,
                                            Factor = GDU.Factor,
                                            ID = UNM.ID,
                                            Name = UNM.Name
                                        }).ToList(),
                            TaxGroups = (from t in _context.TaxGroups.Where(i => !i.Delete && i.Active && i.Type == TypeTax.OutPutTax)
                                         let tgds = _context.TaxGroupDefinitions.Where(i => i.TaxGroupID == t.ID).OrderByDescending(i => i.EffectiveFrom).FirstOrDefault() ?? new TaxGroupDefinition()
                                         select new TaxGroupViewModel
                                         {
                                             ID = t.ID,
                                             //GLID = tg.GLID,
                                             Name = t.Name,
                                             Code = t.Code,
                                             Effectivefrom = tgds.EffectiveFrom,
                                             Rate = tgds.Rate,
                                             Type = (int)t.Type,
                                         }
                                         ).ToList(),
                            UomPriceLists = (from pld in _context.PriceListDetails.Where(i => i.ItemID == item.ID && i.PriceListID == sAr.PriceListID)
                                             select new UomPriceList
                                             {
                                                 UoMID = (int)pld.UomID,
                                                 UnitPrice = (decimal)pld.UnitPrice
                                             }
                                             ).ToList(),
                        }).ToList();
            _dataProp.DataProperty(_sqd, comId, "ItemID", "AddictionProps");
            var data = new ServiceContractUpdateViewModel
            {
                ServiceContract = serviceContract.FirstOrDefault(),
                //AttchmentFileDetail = _att,
                ServiceContractDetail = _sqd,
            };

            return View(new { seriesIN, seriesJE, genSetting = _utility.GetGeneralSettingAdmin(), data });
        }
        public IActionResult CreatedefaultRowAttchment(int num, int number)
        {
            var list = _isale.CreateDefaultRowAttachmet(num, number);

            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttachment()
        {
            AttchmentFile obj = new();
            var files = HttpContext.Request.Form.Files;

            foreach (var f in files)
            {
                var savePath = Path.Combine(_env.WebRootPath + "\\js\\Sale\\UploadFile\\" + f.FileName);
                obj.TargetPath = savePath;
                obj.FileName = f.FileName;
                obj.AttachmentDate = DateTime.Now.ToString("yyyy'-'MM'-'dd");
                using Stream fs = IoFile.Create(savePath);
                await f.CopyToAsync(fs);
            }
            return Ok(obj);
        }
        public async Task<IActionResult> DowloadFile(int AttachID)
        {

            AttchmentFile attachment = _context.AttchmentFiles.Find(AttachID) ?? new AttchmentFile();

            if (attachment.ID > 0)
            {
                string fullPath = Path.GetFullPath(string.Format("{0}{1}", _env.WebRootPath + "\\js\\Sale\\UploadFile\\", attachment.FileName));
                byte[] bytes = await IoFile.ReadAllBytesAsync(fullPath);
                //Send the File to Download.
                return File(bytes, "application/octet-stream", attachment.FileName);
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult RemoveFileFromFolderMastre(string file)
        {
            string fullPath = Path.GetFullPath(string.Format("{0}{1}", _env.WebRootPath + "\\js\\Sale\\UploadFile\\", file));
            if (IoFile.Exists(fullPath))
            {
                IoFile.Delete(fullPath);
            }
            return Ok();
        }
        [HttpPost]
        public IActionResult CreateServiceContract(string data, string seriesJE, string ardownpayment, string _type, string serial, string batch)
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            if (seriesJE == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }
            ServiceContract ServiceContract = JsonConvert.DeserializeObject<ServiceContract>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ServiceContract.AttchmentFiles = ServiceContract.AttchmentFiles.Where(i => i.TargetPath != "").ToList();
            Series seriesje = JsonConvert.DeserializeObject<Series>(seriesJE, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            List<SaleARDPINCN> ards = JsonConvert.DeserializeObject<List<SaleARDPINCN>>(ardownpayment, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == ServiceContract.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == ServiceContract.DocTypeID).FirstOrDefault();
            var series_JE = _context.Series.FirstOrDefault(w => w.ID == seriesje.ID);
            var wh = _context.Warehouses.Find(ServiceContract.WarehouseID) ?? new Warehouse();
            ServiceContract.ChangeLog = DateTime.Now;
            ServiceContract.UserID = GetUserID();
            
            string type = "ServiceContract";
            if (ServiceContract.TotalAmount < 0)
            {
                ModelState.AddModelError("TotalAmount", _culLocal["TotalAmount cannot be less than 0!"]);
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }
            // Checking Serial Batch //
            List<SerialNumber> _serialNumber = new();
            List<SerialNumber> serialNumber = new();
            List<BatchNo> _batchNoes = new();
            List<BatchNo> batchNoes = new();
            ValidateSummary(ServiceContract, ServiceContract.ServiceContractDetails);
            CheckTaxAcc(ServiceContract.ServiceContractDetails);
            var g = _isale.GetAllGroupDefind().ToList();
            if (ServiceContract.ID > 0)
            {
                ModelState.AddModelError("saleAR", _culLocal["Item cannot be chanage."]);
            }

            if (ServiceContract.AppliedAmount > ServiceContract.TotalAmount)
            {
                ModelState.AddModelError("saleAppliedAmount", _culLocal["Applied amount cannot exceeds total amount."]);
            }
            foreach (var dt in ServiceContract.ServiceContractDetails)
            {
                var factor = g.FirstOrDefault(w => w.GroupUoMID == dt.GUomID && w.AltUOM == dt.UomID).Factor;
                dt.Factor = factor;
                if (dt.ID <= 0)
                {
                    dt.PrintQty = dt.Qty;
                    if (ServiceContract.CopyType > 0)
                    {
                        dt.Qty = dt.OpenQty;
                    }
                    else
                    {
                        dt.OpenQty = dt.Qty;
                    }
                }
                else
                {
                    dt.PrintQty = dt.Qty - dt.PrintQty;
                }
            }
            var _itemReturneds = CheckStock(ServiceContract, ServiceContract.ServiceContractDetails.ToList(), "ID").ToList();
            List<ItemsReturn> returns = new();
            List<ItemsReturn> itemSBOnly = _itemReturneds.Where(i => i.IsSerailBatch && i.InStock > 0 && !i.IsBOM).ToList();
            if (wh.IsAllowNegativeStock)
            {
                returns = _itemReturneds.Where(i => i.InStock <= 0 && i.IsSerailBatch).ToList();
            }
            else
            {
                returns = _itemReturneds.Where(i => i.InStock <= 0).ToList();
            }

            if (returns.Any())
            {
                int count = 0;
                foreach (var ir in returns)
                {
                    ModelState.AddModelError("itemReturn" + count.ToString(), _culLocal["The item &lt;&lt;" + ir.KhmerName + "&gt;&gt; has stock left " + ir.InStock + "."]);
                    count++;
                }
                if (count > 0) return Ok(new { Model = msg.Bind(ModelState), ItemReturn = returns });
            }
            if (_type != "DN" && itemSBOnly.Count > 0)
            {
                serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : serialNumber;

                _saleSerialBatch.CheckItemSerail(ServiceContract, ServiceContract.ServiceContractDetails.ToList(), serialNumber);
                serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in serialNumber.ToList())
                {
                    foreach (var i in ServiceContract.ServiceContractDetails.ToList())
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
                batchNoes = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                }) : batchNoes;
                _saleSerialBatch.CheckItemBatch(ServiceContract, ServiceContract.ServiceContractDetails.ToList(), batchNoes);
                batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                foreach (var j in batchNoes.ToList())
                {
                    foreach (var i in ServiceContract.ServiceContractDetails.ToList())
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

            try
            {
                using var t = _context.Database.BeginTransaction();
                if (ModelState.IsValid)
                {
                    seriesDetail.Number = seriesDN.NextNo;
                    seriesDetail.SeriesID = seriesDN.ID;
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.SaveChanges();
                    var seriesDetailID = seriesDetail.ID;
                    string Sno = seriesDN.NextNo;
                    long No = long.Parse(Sno);
                    ServiceContract.InvoiceNo = seriesDN.NextNo;
                    ServiceContract.InvoiceNumber = seriesDN.NextNo;
                    seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                    if (No > long.Parse(seriesDN.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", _culLocal["Your Sale A/R Invoice has reached the limitation!!"]);
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                    switch (ServiceContract.CopyType)
                    {
                        case SaleCopyType.Quotation:
                            var copyKeyQ = ServiceContract.CopyKey.Split("-")[1];
                            var quoteMaster = _context.SaleQuotes.Include(q => q.SaleQuoteDetails)
                                .FirstOrDefault(m => string.Compare(m.InvoiceNumber, copyKeyQ) == 0);
                            UpdateSourceCopy(ServiceContract.ServiceContractDetails, quoteMaster, quoteMaster.SaleQuoteDetails, SaleCopyType.Quotation);
                            type = "Quotation"; // SQ
                            break;
                        case SaleCopyType.Order:
                            var copyKeyO = ServiceContract.CopyKey.Split("-")[1];
                            var orderMaster = _context.SaleOrders.Include(o => o.SaleOrderDetails)
                                .FirstOrDefault(m => string.Compare(m.InvoiceNumber, copyKeyO) == 0);
                            UpdateSourceCopy(ServiceContract.ServiceContractDetails, orderMaster, orderMaster.SaleOrderDetails, SaleCopyType.Order);
                            type = "Order"; //SO
                            break;
                        case SaleCopyType.Delivery:
                            var copyKeyD = ServiceContract.CopyKey.Split("-")[1];
                            var deliveryMaster = _context.SaleDeliveries.Include(o => o.SaleDeliveryDetails)
                                .FirstOrDefault(m => string.Compare(m.InvoiceNumber, copyKeyD) == 0);
                            UpdateSourceCopy(ServiceContract.ServiceContractDetails, deliveryMaster, deliveryMaster.SaleDeliveryDetails, SaleCopyType.Delivery);
                            type = "Delivery"; //DN
                            break;
                    }
                    if (ServiceContract.TotalAmount <= ServiceContract.AppliedAmount)
                    {
                        ServiceContract.Status = "close";
                    }
                    if (ServiceContract.ID == 0 && ServiceContract.CopyType == 0)
                    {
                        ServiceContract.LocalSetRate = localSetRate;
                    }
                    ServiceContract.SeriesID = seriesDN.ID;
                    ServiceContract.LocalCurID = GetCompany().LocalCurrencyID;
                    ServiceContract.LocalSetRate = localSetRate;
                    ServiceContract.SeriesDID = seriesDetailID;
                    ServiceContract.CompanyID = GetCompany().ID;
                    _context.ServiceContracts.Update(ServiceContract);
                    _context.SaveChanges();
                    var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                    if (gldeter != null)
                    {
                        if (gldeter.GLID == 0)
                        {
                            ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is missing!!"]);
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is not set up yet!!"]);
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                    var freight = ServiceContract.FreightSalesView;
                    if (freight != null)
                    {
                        //freight.FreightSaleDetails = freight.FreightSaleDetails.Where(i => i.Amount > 0).ToList();
                        freight.SaleID = ServiceContract.ID;
                        freight.SaleType = SaleCopyType.AR;
                        freight.OpenAmountReven = freight.AmountReven;
                        _context.FreightSales.Update(freight);
                        _context.SaveChanges();
                    }
                    _isale.IssuseInStockServiceContract(ServiceContract.ID, type, ards, gldeter, freight, _serialNumber, _batchNoes);
                    // checking maximun Invoice
                    var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                    var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                    if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", _culLocal["Your Invoice Journal Entry has reached the limitation!!"]);
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                    CreateIncomingPaymentCustomerByServiceContract(ServiceContract);

                    t.Commit();
                    ModelState.AddModelError("success", _culLocal["Item save successfully."]);
                    msg.Approve();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return Ok(new { Model = msg.Bind(ModelState), ItemReturn = returns });
        }


        [HttpPost]
        public async Task<IActionResult> CancelServiceContract(string saleAr, int saleArId)
        {
            SaleAR saleAR = JsonConvert.DeserializeObject<SaleAR>(saleAr, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            var ardata = _context.SaleARs.Find(saleArId);
            if (ardata == null) ModelState.AddModelError("ardata", _culLocal["A/R invoice could not found"]);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == saleAR.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == saleAR.DocTypeID).FirstOrDefault();
            List<SerialNumber> serails = new();
            List<BatchNo> batches = new();
            foreach (var i in saleAR.SaleARDetails.ToList())
            {
                var item = _context.ItemMasterDatas.Find(i.ItemID) ?? new ItemMasterData();
                if (item.ManItemBy == ManageItemBy.SerialNumbers)
                {
                    var serialNumberSelected = await _saleSerialBatch.GetSerialDetialsCopyAsync(i.ItemID, saleArId, saleAR.WarehouseID, TransTypeWD.AR);
                    serails.Add(new SerialNumber
                    {
                        SerialNumberSelected = serialNumberSelected
                    });
                }
                if (item.ManItemBy == ManageItemBy.Batches)
                {
                    var batchSelected = await _saleSerialBatch.GetBatchNoDetialsCopyAsync(i.ItemID, i.UomID, saleArId, saleAR.WarehouseID, TransTypeWD.AR);
                    batches.Add(new BatchNo
                    {
                        BatchNoSelected = batchSelected
                    });
                }
            }
            if (ModelState.IsValid)
            {
                using var t = _context.Database.BeginTransaction();
                seriesDetail.Number = seriesDN.NextNo;
                seriesDetail.SeriesID = seriesDN.ID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDN.NextNo;
                long No = long.Parse(Sno);
                saleAR.InvoiceNo = seriesDN.NextNo;
                saleAR.InvoiceNumber = seriesDN.NextNo;
                seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                if (No > long.Parse(seriesDN.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Sale A/R Invoice has reached the limitation!!"]);
                    return Ok(msg.Bind(ModelState));
                }
                var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w => w.InvoiceNumber == ardata.InvoiceNumber && w.SeriesID == ardata.SeriesID);
                var _freight = _context.FreightSales.FirstOrDefault(i => i.SaleID == ardata.SARID);
                if (_freight != null)
                {
                    _freight.OpenAmountReven = 0;
                    ardata.FreightAmount -= saleAR.FreightAmount;
                    ardata.FreightAmountSys = ardata.FreightAmount * (decimal)saleAR.ExchangeRate;
                    _context.FreightSales.Update(_freight);
                }
                ardata.AppliedAmount = saleAR.AppliedAmount;
                if (incomingCus != null)
                {
                    incomingCus.Applied_Amount = incomingCus.Total;
                    incomingCus.BalanceDue = 0;
                    incomingCus.TotalPayment = 0;
                    incomingCus.Status = "close";
                    _context.IncomingPaymentCustomers.Update(incomingCus);
                }

                _context.SaveChanges();
                var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
                var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
                if (seriesIn == null)
                {
                    ModelState.AddModelError("SeriesIn", "Series Incoming Payment has No Data or Default One!!");
                    return Ok(msg.Bind(ModelState));
                }
                if (paymentMean == null)
                {
                    ModelState.AddModelError("SeriesIn", "Series Payment Means has No Data or Default One!!");
                    return Ok(msg.Bind(ModelState));
                }


                saleAR.Status = "close";
                saleAR.SeriesID = seriesDN.ID;
                saleAR.SeriesDID = seriesDetailID;
                ardata.Status = "cancel";
                _context.SaleARs.Update(saleAR);
                _context.SaleARs.Update(ardata);
                _context.SaveChanges();
                var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                if (gldeter != null)
                {
                    if (gldeter.GLID == 0)
                    {
                        ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is missing!!"]);
                        return Ok(msg.Bind(ModelState));
                    }
                }
                else
                {
                    ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is not set up yet!!"]);
                    return Ok(msg.Bind(ModelState));
                }
                _isale.IssuseCancelSaleAr(saleAR, serails, batches, gldeter);

                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Invoice Journal Entry has reached the limitation!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                t.Commit();
                ModelState.AddModelError("success", _culLocal["A/R Invoice has been cancelled successfully!"]);
                msg.Approve();
                return Ok(msg.Bind(ModelState));
            }
            return Ok(msg.Bind(ModelState));
        }
        //================history============
        public IActionResult ARServiceContractHistory()
        {

            return View();
        }
        //=======end history=========
        [HttpGet]
        public IActionResult FindServiceContract(DateTime contract_edate, DateTime contract_sdate, int cus_id, string contract_no, DateTime renewal_date, DateTime post_date, string number, int seriesID)
        {
            var list = _isale.FindServiceContract(contract_edate, contract_sdate, cus_id, contract_no, renewal_date, post_date, number, seriesID, GetCompany().ID);
            if (list.ServiceContract != null)
            {
                return Ok(list);
            }

            return Ok();
        }
        public IActionResult GetServiceContractARByInvoiceNo(string invoiceNo, int seriesID)
        {
            var number = invoiceNo.Contains("-") ? invoiceNo.Split("-")[1] : invoiceNo;
            var ARMaster = _context.SaleARs.Include(o => o.SaleARDetails)
                         .FirstOrDefault(m => string.Compare(m.InvoiceNumber, number) == 0 && m.SeriesID == seriesID);
            return Ok(ARMaster);
        }
        public IActionResult ServiceContractHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale AR History";
            ViewBag.Sale = "show";
            ViewBag.SaleAR = "highlight";
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();

            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.AR).Where(h => GetValidDate(h.PostingDate).CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>(),
                Templateurl = _utility.PrintTemplateUrl()
            };
            return View(history);
        }

        private bool CreateIncomingPaymentCustomerByServiceContract(ServiceContract serviceContract)
        {
            string currencyName = _context.Currency.Find(serviceContract.SaleCurrencyID).Description;
            SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
            var payment = _context.IncomingPaymentCustomers.FirstOrDefault(p => p.InvoiceNumber == serviceContract.InvoiceNumber && p.SeriesID == serviceContract.SeriesID);
            var docType = _context.DocumentTypes.Find(serviceContract.DocTypeID);
            var em = _context.Employees.FirstOrDefault(i => i.ID == serviceContract.SaleEmID) ?? new Employee();
            var user = _userModule.CurrentUser;
            IncomingPaymentCustomer ipcustomer = new()
            {
                IncomingPaymentCustomerID = 0,
                EmID = em.ID,
                EmName = em.Name,
                CreatorID = user.ID,
                CreatorName = user.Username,
                CustomerID = serviceContract.CusID,
                BranchID = serviceContract.BranchID,
                WarehouseID = serviceContract.WarehouseID,
                DocTypeID = serviceContract.DocTypeID,
                SeriesID = serviceContract.SeriesID,
                SeriesDID = serviceContract.SeriesDID,
                CompanyID = serviceContract.CompanyID,
                InvoiceNumber = serviceContract.InvoiceNumber,
                CurrencyID = serviceContract.SaleCurrencyID,
                Types = serviceContract.Types,
                //DocumentNo = saleAR.InvoiceNo,
                //DocumentType = GetTransactType(saleAR.InvoiceNo, saleAR.IncludeVat),
                Applied_Amount = serviceContract.AppliedAmount,
                CurrencyName = currencyName,
                ExchangeRate = serviceContract.ExchangeRate,
                TotalPayment = (serviceContract.TotalAmount - serviceContract.AppliedAmount),
                CashDiscount = 0,
                Total = (serviceContract.TotalAmount - serviceContract.AppliedAmount),//serviceContract.SubTotal,
                TotalDiscount = 0,
                BalanceDue = (serviceContract.TotalAmount - serviceContract.AppliedAmount),
                Status = serviceContract.Status,
                Date = serviceContract.DueDate,
                PostingDate = serviceContract.PostingDate,
                SysCurrency = syCurrency.ID,
                SysName = syCurrency.Description,
                LocalCurID = serviceContract.LocalCurID,
                LocalSetRate = serviceContract.LocalSetRate,
                ItemInvoice = $"{docType.Code}-{serviceContract.InvoiceNumber}"
            };

            if (serviceContract.TotalAmount <= serviceContract.AppliedAmount)
            {
                serviceContract.Status = "close";
            }

            if (payment != null)
            {
                payment.Applied_Amount = serviceContract.AppliedAmount;
                payment.BalanceDue = serviceContract.TotalAmount - serviceContract.AppliedAmount;
                //payment.TotalPayment -= saleAR.AppliedAmount;
                payment.TotalPayment = serviceContract.TotalAmount - serviceContract.AppliedAmount;
                payment.Status = serviceContract.Status;
                var paymentDetails = _context.IncomingPaymentDetails.Where(ipd => ipd.DocumentNo == payment.DocumentNo);
                foreach (var pd in paymentDetails)
                {
                    pd.Delete = true;
                }
                _context.IncomingPaymentCustomers.Update(payment);
            }
            else
            {
                _context.IncomingPaymentCustomers.Add(ipcustomer);
            }

            _context.SaveChanges();
            return true;
        }

        //=============================End Service Contract================

        //=====================Reserve Invoice===================
        //[Privilege("AR")]
        [Privilege("ARR")]
        public IActionResult ARReserveInvoice()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "AR Reserve Invoice";
            ViewBag.Sale = "show";
            ViewBag.ARReserveInvoice = "highlight";
            return View(new { seriesIN = _utility.GetSeries("IN"), seriesJE = _utility.GetSeries("JE"), genSetting = _utility.GetGeneralSettingAdmin() });
        }
        [Privilege("ST0011")]

        public IActionResult ARReserveInvoiceEditable()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "AR Reserve Invoice Editable";
            ViewBag.Sale = "show";
            ViewBag.ARReserveInvoiceEDT = "highlight";
            return View(new { seriesIN = _utility.GetSeries("IN"), seriesJE = _utility.GetSeries("JE"), genSetting = _utility.GetGeneralSettingAdmin() });
        }
        [HttpPost]
        public IActionResult CreateARReserveInvoice(string data, string seriesJE, string ardownpayment, string _type)
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            if (seriesJE == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }
            ARReserveInvoice aRReserveInvoice = JsonConvert.DeserializeObject<ARReserveInvoice>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Series seriesje = JsonConvert.DeserializeObject<Series>(seriesJE, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            List<SaleARDPINCN> ards = JsonConvert.DeserializeObject<List<SaleARDPINCN>>(ardownpayment, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == aRReserveInvoice.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == aRReserveInvoice.DocTypeID).FirstOrDefault();
            var series_JE = _context.Series.FirstOrDefault(w => w.ID == seriesje.ID);
            var wh = _context.Warehouses.Find(aRReserveInvoice.WarehouseID) ?? new Warehouse();
            aRReserveInvoice.ChangeLog = DateTime.Now;
            aRReserveInvoice.UserID = GetUserID();
           
            string type = "ARReserveInvoice";
            if (aRReserveInvoice.TotalAmount < 0)
            {
                ModelState.AddModelError("TotalAmount", _culLocal["TotalAmount cannot be less than 0!"]);
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }

            ValidateSummary(aRReserveInvoice, aRReserveInvoice.ARReserveInvoiceDetails);
            CheckTaxAcc(aRReserveInvoice.ARReserveInvoiceDetails);
            var g = _isale.GetAllGroupDefind().ToList();
            if (aRReserveInvoice.ID > 0)
            {
                ModelState.AddModelError("aRReserveInvoice", _culLocal["Item cannot be chanage."]);
            }

            if (aRReserveInvoice.AppliedAmount > aRReserveInvoice.TotalAmount)
            {
                ModelState.AddModelError("saleAppliedAmount", _culLocal["Applied amount cannot exceeds total amount."]);
            }
            foreach (var dt in aRReserveInvoice.ARReserveInvoiceDetails)
            {
                var factor = g.FirstOrDefault(w => w.GroupUoMID == dt.GUomID && w.AltUOM == dt.UomID).Factor;
                dt.Factor = factor;
                if (dt.ID <= 0)
                {
                    dt.PrintQty = dt.Qty;
                    if (aRReserveInvoice.CopyType > 0)
                    {
                        dt.Qty = dt.OpenQty;
                    }
                    else
                    {
                        dt.OpenQty = dt.Qty;
                    }
                }
                else
                {
                    dt.PrintQty = dt.Qty - dt.PrintQty;
                }
            }
            using var t = _context.Database.BeginTransaction();
            if (ModelState.IsValid)
            {
                seriesDetail.Number = seriesDN.NextNo;
                seriesDetail.SeriesID = seriesDN.ID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDN.NextNo;
                long No = long.Parse(Sno);
                aRReserveInvoice.InvoiceNo = seriesDN.NextNo;
                aRReserveInvoice.InvoiceNumber = seriesDN.NextNo;
                seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                if (No > long.Parse(seriesDN.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Sale A/R Invoice has reached the limitation!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                switch (aRReserveInvoice.CopyType)
                {
                    case SaleCopyType.Quotation:

                        var quoteMaster = _context.SaleQuotes.Include(q => q.SaleQuoteDetails)
                            .FirstOrDefault(m => m.SQID == aRReserveInvoice.BaseOnID) ?? new SaleQuote();
                        UpdateSourceCopy(aRReserveInvoice.ARReserveInvoiceDetails, quoteMaster, quoteMaster.SaleQuoteDetails, SaleCopyType.Quotation);
                        type = "Quotation"; // SQ
                        break;
                    case SaleCopyType.Order:

                        var orderMaster = _context.SaleOrders.Include(o => o.SaleOrderDetails)
                            .FirstOrDefault(m => m.SOID == aRReserveInvoice.BaseOnID) ?? new SaleOrder();
                        UpdateSourceCopy(aRReserveInvoice.ARReserveInvoiceDetails, orderMaster, orderMaster.SaleOrderDetails, SaleCopyType.Order);
                        type = "Order"; //SO
                        break;
                }
                if (aRReserveInvoice.TotalAmount <= aRReserveInvoice.AppliedAmount)
                {
                    aRReserveInvoice.Status = "close";
                }
                if (aRReserveInvoice.ID == 0 && aRReserveInvoice.CopyType == 0)
                {
                    aRReserveInvoice.LocalSetRate = localSetRate;
                }
                aRReserveInvoice.SeriesID = seriesDN.ID;
                aRReserveInvoice.LocalCurID = GetCompany().LocalCurrencyID;
                aRReserveInvoice.LocalSetRate = localSetRate;
                aRReserveInvoice.SeriesDID = seriesDetailID;
                aRReserveInvoice.CompanyID = GetCompany().ID;
                _context.ARReserveInvoices.Update(aRReserveInvoice);
                _context.SaveChanges();
                var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                if (gldeter != null)
                {
                    if (gldeter.GLID == 0)
                    {
                        ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is missing!!"]);
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                }
                else
                {
                    ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is not set up yet!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                var freight = aRReserveInvoice.FreightSalesView;
                if (freight != null)
                {
                    freight.SaleID = aRReserveInvoice.ID;
                    freight.SaleType = SaleCopyType.AR;
                    freight.OpenAmountReven = freight.AmountReven;
                    _context.FreightSales.Update(freight);
                    _context.SaveChanges();
                }
                _isale.IssuseInStockARReserveInvoice(aRReserveInvoice.ID, type, ards, gldeter, freight);
                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Invoice Journal Entry has reached the limitation!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                CreateIncomingPaymentCustomerByARReserveInvoice(aRReserveInvoice);

                t.Commit();
                ModelState.AddModelError("success", _culLocal["Item save successfully."]);
                msg.Approve();
            }
            return Ok(new { Model = msg.Bind(ModelState) });
        }

        [HttpPost]
        public async Task<IActionResult> CancelSaleARReserveEDT(string saleArEdited, int saleArId)
        {
            ARReserveInvoiceEditable saleAredit = JsonConvert.DeserializeObject<ARReserveInvoiceEditable>(saleArEdited, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            if (saleAredit.AppliedAmount > 0)
            {
                ModelState.AddModelError("AppliedAmount", _culLocal["A/R Reserve invoice Editable is Paded alresdy so can't not Cancel"]);
                return Ok(msg.Bind(ModelState));
            }
            var ardata = _context.ARReserveInvoiceEditables.Find(saleArId);
            if (ardata == null) ModelState.AddModelError("ardata", _culLocal["A/R Reserve invoice Editable  could not found"]);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == saleAredit.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == saleAredit.DocTypeID).FirstOrDefault();
            List<SerialNumber> serails = new();
            List<BatchNo> batches = new();
            foreach (var i in saleAredit.ARReserveInvoiceEditableDetails.ToList())
            {
                i.ID = 0;
                var item = _context.ItemMasterDatas.Find(i.ItemID) ?? new ItemMasterData();
                if (item.ManItemBy == ManageItemBy.SerialNumbers)
                {
                    var serialNumberSelected = await _saleSerialBatch.GetSerialDetialsCopyAsync(i.ItemID, saleArId, saleAredit.WarehouseID, TransTypeWD.AR_REDT);
                    serails.Add(new SerialNumber
                    {
                        SerialNumberSelected = serialNumberSelected
                    });
                }
                if (item.ManItemBy == ManageItemBy.Batches)
                {
                    var batchSelected = await _saleSerialBatch.GetBatchNoDetialsCopyAsync(i.ItemID, i.UomID, saleArId, saleAredit.WarehouseID, TransTypeWD.AR_REDT);
                    batches.Add(new BatchNo
                    {
                        BatchNoSelected = batchSelected
                    });
                }
            }
            if (ModelState.IsValid)
            {
                using var t = _context.Database.BeginTransaction();
                seriesDetail.Number = seriesDN.NextNo;
                seriesDetail.SeriesID = seriesDN.ID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDN.NextNo;
                long No = long.Parse(Sno);
                saleAredit.InvoiceNo = seriesDN.NextNo;
                saleAredit.InvoiceNumber = seriesDN.NextNo;
                seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                if (No > long.Parse(seriesDN.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Sale A/R Invoice has reached the limitation!!"]);
                    return Ok(msg.Bind(ModelState));
                }
                var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w => w.SeriesDID == ardata.SeriesDID);
                var _freight = _context.FreightSales.FirstOrDefault(i => i.SaleID == ardata.ID && i.SaleType == SaleCopyType.ARReserveInvoiceEDT);
                if (_freight != null)
                {
                    _freight.OpenAmountReven = 0;
                    ardata.FreightAmount -= saleAredit.FreightAmount;
                    ardata.FreightAmountSys = ardata.FreightAmount * (decimal)saleAredit.ExchangeRate;
                    _context.FreightSales.Update(_freight);
                }
                // ardata.AppliedAmount = saleAredit.AppliedAmount;
                if (incomingCus != null)
                {
                    incomingCus.Applied_Amount = incomingCus.Total;
                    incomingCus.BalanceDue = 0;
                    incomingCus.TotalPayment = 0;
                    incomingCus.Status = "close";
                    _context.IncomingPaymentCustomers.Update(incomingCus);
                }

                _context.SaveChanges();
                var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
                var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
                if (seriesIn == null)
                {
                    ModelState.AddModelError("SeriesIn", "Series Incoming Payment has No Data or Default One!!");
                    return Ok(msg.Bind(ModelState));
                }
                if (paymentMean == null)
                {
                    ModelState.AddModelError("SeriesIn", "Series Payment Means has No Data or Default One!!");
                    return Ok(msg.Bind(ModelState));
                }


                saleAredit.Status = "close";
                saleAredit.SeriesID = seriesDN.ID;
                saleAredit.SeriesDID = seriesDetailID;
                ardata.AppliedAmount = saleAredit.AppliedAmount;
                ardata.Status = "cancel";
                _context.ARReserveInvoiceEditables.Update(saleAredit);
                _context.ARReserveInvoiceEditables.Update(ardata);
                _context.SaveChanges();
                var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                if (gldeter != null)
                {
                    if (gldeter.GLID == 0)
                    {
                        ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is missing!!"]);
                        return Ok(msg.Bind(ModelState));
                    }
                }
                else
                {
                    ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is not set up yet!!"]);
                    return Ok(msg.Bind(ModelState));
                }
                _isale.CancelARReserveInvoiceEDT(saleAredit);

                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Invoice Journal Entry has reached the limitation!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                t.Commit();
                ModelState.AddModelError("success", _culLocal["A/R Invoice has been cancelled successfully!"]);
                msg.Approve();
                return Ok(msg.Bind(ModelState));
            }
            return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public IActionResult GetSaleARReserveEDTByInvoiceNo(string invoiceNo, int seriesID)
        {
            var number = invoiceNo.Contains("-") ? invoiceNo.Split("-")[1] : invoiceNo;
            var ARMaster = _context.ARReserveInvoiceEditables.Include(o => o.ARReserveInvoiceEditableDetails)
                         .FirstOrDefault(m => string.Compare(m.InvoiceNumber, number) == 0 && m.SeriesID == seriesID);
            return Ok(ARMaster);
        }
        [HttpPost]
        public IActionResult CreateARReserveInvoiceEditable(string data, string seriesJE, string ardownpayment, string _type)
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            bool update = true;
            if (seriesJE == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }
            ARReserveInvoiceEditable aRReserveInvoice = JsonConvert.DeserializeObject<ARReserveInvoiceEditable>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Series seriesje = JsonConvert.DeserializeObject<Series>(seriesJE, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            List<SaleARDPINCN> ards = JsonConvert.DeserializeObject<List<SaleARDPINCN>>(ardownpayment, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });

            update = aRReserveInvoice.ID > 0 ? false : true;
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesDN = _context.Series.FirstOrDefault(w => w.ID == aRReserveInvoice.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == aRReserveInvoice.DocTypeID).FirstOrDefault();
            var series_JE = _context.Series.FirstOrDefault(w => w.ID == seriesje.ID);
            var wh = _context.Warehouses.Find(aRReserveInvoice.WarehouseID) ?? new Warehouse();
            aRReserveInvoice.ChangeLog = DateTime.Now;
            aRReserveInvoice.UserID = GetUserID();
            
            aRReserveInvoice.Types = SaleCopyType.ARReserveInvoiceEDT.ToString();
            string type = SaleCopyType.ARReserveInvoiceEDT.ToString();
            if (aRReserveInvoice.TotalAmount < 0)
            {
                ModelState.AddModelError("TotalAmount", _culLocal["TotalAmount cannot be less than 0!"]);
                return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
            }
            if (aRReserveInvoice.ID != 0)
            {
                if (aRReserveInvoice.TotalAmount < aRReserveInvoice.AppliedAmount)
                {
                    ModelState.AddModelError("TotalAmount", "Choose: <br/>1: TotalAmount= " + aRReserveInvoice.TotalAmount + " must smaller than Applies Amount=" + aRReserveInvoice.AppliedAmount + "<br/>2: You must to return Sale Delivery.");
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
            }

            ValidateSummary(aRReserveInvoice, aRReserveInvoice.ARReserveInvoiceEditableDetails);
            CheckTaxAcc(aRReserveInvoice.ARReserveInvoiceEditableDetails);
            var g = _isale.GetAllGroupDefind().ToList();


            if (aRReserveInvoice.AppliedAmount > aRReserveInvoice.TotalAmount)
            {
                ModelState.AddModelError("saleAppliedAmount", _culLocal["Applied amount cannot exceeds total amount."]);
            }
            foreach (var dt in aRReserveInvoice.ARReserveInvoiceEditableDetails)
            {
                if (dt.ID > 0)
                {
                    var history = _context.ARReserveEditableDetailHistories.AsNoTracking().LastOrDefault(s => s.ARREDTDID == dt.ID);
                    var checkQty = _context.ARReserveInvoiceEditableDetails.AsNoTracking().FirstOrDefault(s => s.ID == dt.ID);
                    var ItemName = checkQty.ItemNameKH;

                    if (checkQty.OpenQty != history.OpenQty || checkQty.DeliveryQty > 0)
                    {
                        if (dt.Delete == true)
                        {
                            // if (checkQty.OpenQty != checkQty.Qty || checkQty.DeliveryQty > 0)
                            if (checkQty.DeliveryQty != 0)
                            {
                                ModelState.AddModelError("Details", "This Item" + " " + ItemName + " " + "Already Delivered Can't Not Delete !");
                            }
                        }

                        if (dt.Qty < checkQty.DeliveryQty)
                        {
                            ModelState.AddModelError("Details", "Please Input" + " " + ItemName + " " + "Qty More Than" + " " + checkQty.DeliveryQty + " " + "!");
                        }

                        if (dt.Qty >= checkQty.DeliveryQty)
                        {
                            var CDQty = dt.Qty - checkQty.DeliveryQty;
                            dt.OpenQty = CDQty;
                            dt.DeliveryQty = checkQty.DeliveryQty;
                        }
                    }
                }

                if (dt.ID == 0)
                {
                    dt.Status = AREDetailStatus.New;
                    dt.OpenQty = dt.Qty;
                }
                else
                {
                    dt.Status = AREDetailStatus.Old;
                }
                var factor = g.FirstOrDefault(w => w.GroupUoMID == dt.GUomID && w.AltUOM == dt.UomID).Factor;
                dt.Factor = factor;
                dt.PrintQty = dt.Qty;
            }
            using var t = _context.Database.BeginTransaction();
            if (ModelState.IsValid)
            {

                if (aRReserveInvoice.ID == 0)
                {
                    seriesDetail.Number = seriesDN.NextNo;
                    seriesDetail.SeriesID = seriesDN.ID;
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.SaveChanges();
                    var seriesDetailID = seriesDetail.ID;
                    string Sno = seriesDN.NextNo;
                    long No = long.Parse(Sno);
                    aRReserveInvoice.InvoiceNo = seriesDN.NextNo;
                    aRReserveInvoice.InvoiceNumber = seriesDN.NextNo;
                    seriesDN.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                    aRReserveInvoice.SeriesDID = seriesDetailID;
                    // if (No > long.Parse(seriesDN.LastNo))
                    // {
                    //     ModelState.AddModelError("SumInvoice", _culLocal["Your Sale A/R Invoice has reached the limitation!!"]);
                    //     return Ok(new { Model = msg.Bind(ModelState) });
                    // }
                }
                //  List<SaleModel> saleModels = new List<SaleModel>();
                if (aRReserveInvoice.ID == 0)
                    switch (aRReserveInvoice.CopyType)
                    {
                        case SaleCopyType.Quotation:


                            var orderMasterQ = _context.SaleQuotes.Include(o => o.SaleQuoteDetails)
                                .FirstOrDefault(m => m.SQID == aRReserveInvoice.BaseOnID);
                            UpdateSourceCopy(aRReserveInvoice.ARReserveInvoiceEditableDetails, orderMasterQ, orderMasterQ.SaleQuoteDetails, SaleCopyType.Quotation);
                            type = SaleCopyType.Quotation.ToString(); //SO
                            aRReserveInvoice.Types = SaleCopyType.Quotation.ToString();
                            break;
                        case SaleCopyType.Order:

                            var orderMaster = _context.SaleOrders.Include(o => o.SaleOrderDetails)
                                .FirstOrDefault(m => m.SOID == aRReserveInvoice.BaseOnID);
                            UpdateSourceCopy(aRReserveInvoice.ARReserveInvoiceEditableDetails, orderMaster, orderMaster.SaleOrderDetails, SaleCopyType.Order);
                            type = SaleCopyType.Order.ToString(); //SO
                            aRReserveInvoice.Types = SaleCopyType.Order.ToString();
                            break;
                    }
                if (aRReserveInvoice.TotalAmount <= aRReserveInvoice.AppliedAmount)
                {
                    if (aRReserveInvoice.Delivery)
                    {
                        aRReserveInvoice.Status = "close";
                    }
                    aRReserveInvoice.Paded = true;
                }
                if (aRReserveInvoice.ID == 0 && aRReserveInvoice.CopyType == 0)
                {
                    aRReserveInvoice.LocalSetRate = localSetRate;
                }
                aRReserveInvoice.SeriesID = seriesDN.ID;
                aRReserveInvoice.LocalCurID = GetCompany().LocalCurrencyID;
                aRReserveInvoice.LocalSetRate = localSetRate;

                aRReserveInvoice.CompanyID = GetCompany().ID;

                var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                if (gldeter != null)
                {
                    if (gldeter.GLID == 0)
                    {
                        ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is missing!!"]);
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                }
                else
                {
                    ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is not set up yet!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                var freight = aRReserveInvoice.FreightSalesView;
                if (update)
                {
                    _context.ARReserveInvoiceEditables.Update(aRReserveInvoice);
                    _context.SaveChanges();
                    if (freight != null)
                    {
                        freight.SaleID = aRReserveInvoice.ID;
                        // freight.SaleType = SaleCopyType.AR;
                        freight.SaleType = SaleCopyType.ARReserveInvoiceEDT;
                        freight.OpenAmountReven = freight.AmountReven;
                        _context.FreightSales.Update(freight);
                        _context.SaveChanges();
                    }
                    _isale.IssuseInStockARReserveInvoiceEDT(aRReserveInvoice, type, ards, gldeter);
                }
                else
                {
                    _isale.IssuseInStockARReserveInvoiceEDTOld(aRReserveInvoice, type, ards, gldeter);
                    var arReserve = aRReserveInvoice.ARReserveInvoiceEditableDetails.Where(w => w.Delete).ToList();
                    _context.ARReserveInvoiceEditables.Update(aRReserveInvoice);
                    _context.SaveChanges();
                    _context.ARReserveInvoiceEditableDetails.RemoveRange(arReserve);
                    _context.SaveChanges();

                }
                ARReserveEditableHistory saleAREditeHistory = new();
                saleAREditeHistory.ARReserveEditableID = aRReserveInvoice.ID;
                saleAREditeHistory.CusID = aRReserveInvoice.CusID;
                saleAREditeHistory.RequestedBy = aRReserveInvoice.RequestedBy;
                saleAREditeHistory.ShippedBy = aRReserveInvoice.ShippedBy;
                saleAREditeHistory.ReceivedBy = aRReserveInvoice.ReceivedBy;
                saleAREditeHistory.BaseOnID = aRReserveInvoice.BaseOnID;
                saleAREditeHistory.BranchID = aRReserveInvoice.BranchID;
                saleAREditeHistory.WarehouseID = aRReserveInvoice.WarehouseID;
                saleAREditeHistory.UserID = aRReserveInvoice.UserID;
                saleAREditeHistory.SaleCurrencyID = aRReserveInvoice.SaleCurrencyID;
                saleAREditeHistory.CompanyID = aRReserveInvoice.CompanyID;
                saleAREditeHistory.DocTypeID = aRReserveInvoice.DocTypeID;
                saleAREditeHistory.SeriesID = aRReserveInvoice.SeriesID;
                saleAREditeHistory.SeriesDID = aRReserveInvoice.SeriesDID;
                saleAREditeHistory.InvoiceNumber = aRReserveInvoice.InvoiceNumber;
                saleAREditeHistory.RefNo = aRReserveInvoice.RefNo;
                saleAREditeHistory.InvoiceNo = aRReserveInvoice.InvoiceNo;
                saleAREditeHistory.ExchangeRate = aRReserveInvoice.ExchangeRate;
                saleAREditeHistory.PostingDate = aRReserveInvoice.PostingDate;
                saleAREditeHistory.DueDate = aRReserveInvoice.DueDate;
                saleAREditeHistory.DeliveryDate = aRReserveInvoice.DeliveryDate;
                saleAREditeHistory.DocumentDate = aRReserveInvoice.DocumentDate;
                saleAREditeHistory.IncludeVat = aRReserveInvoice.IncludeVat;
                saleAREditeHistory.Status = aRReserveInvoice.Status;
                saleAREditeHistory.Remarks = aRReserveInvoice.Remarks;
                saleAREditeHistory.SubTotalBefDis = aRReserveInvoice.SubTotalBefDis;
                saleAREditeHistory.SubTotalBefDisSys = aRReserveInvoice.SubTotalBefDisSys;
                saleAREditeHistory.SubTotalAfterDis = aRReserveInvoice.SubTotalAfterDis;
                saleAREditeHistory.SubTotalAfterDisSys = aRReserveInvoice.SubTotalAfterDisSys;
                saleAREditeHistory.FreightAmount = aRReserveInvoice.FreightAmount;
                saleAREditeHistory.FreightAmountSys = aRReserveInvoice.FreightAmountSys;
                saleAREditeHistory.DownPayment = aRReserveInvoice.DownPayment;
                saleAREditeHistory.DownPaymentSys = aRReserveInvoice.DownPaymentSys;
                saleAREditeHistory.SubTotal = aRReserveInvoice.SubTotal;
                saleAREditeHistory.SubTotalSys = aRReserveInvoice.SubTotalSys;
                saleAREditeHistory.DisRate = aRReserveInvoice.DisRate;
                saleAREditeHistory.DisValue = aRReserveInvoice.DisValue;
                saleAREditeHistory.TypeDis = aRReserveInvoice.TypeDis;
                saleAREditeHistory.VatRate = aRReserveInvoice.VatRate;
                saleAREditeHistory.VatValue = aRReserveInvoice.VatValue;
                saleAREditeHistory.AppliedAmount = aRReserveInvoice.AppliedAmount;
                saleAREditeHistory.FeeNote = aRReserveInvoice.FeeNote;
                saleAREditeHistory.FeeAmount = aRReserveInvoice.FeeAmount;
                saleAREditeHistory.TotalAmount = aRReserveInvoice.TotalAmount;
                saleAREditeHistory.TotalAmountSys = aRReserveInvoice.TotalAmountSys;
                saleAREditeHistory.CopyType = aRReserveInvoice.CopyType;
                saleAREditeHistory.CopyKey = aRReserveInvoice.CopyKey;
                saleAREditeHistory.BasedCopyKeys = aRReserveInvoice.BasedCopyKeys;
                saleAREditeHistory.ChangeLog = aRReserveInvoice.ChangeLog;
                saleAREditeHistory.PriceListID = aRReserveInvoice.PriceListID;
                saleAREditeHistory.LocalCurID = aRReserveInvoice.LocalCurID;
                saleAREditeHistory.LocalSetRate = aRReserveInvoice.LocalSetRate;
                saleAREditeHistory.SaleEmID = aRReserveInvoice.SaleEmID;
                _context.Add(saleAREditeHistory);
                _context.SaveChanges();
                foreach (var ard in aRReserveInvoice.ARReserveInvoiceEditableDetails.Where(w => !w.Delete))
                {
                    ARReserveEditableDetailHistory saleAREditeDetailHistory = new();
                    saleAREditeDetailHistory.ARREDTDID = ard.ID;
                    saleAREditeDetailHistory.ARREDTHID = saleAREditeHistory.ID;
                    saleAREditeDetailHistory.SaleCopyType = ard.SaleCopyType;
                    saleAREditeDetailHistory.LineID = ard.LineID;
                    saleAREditeDetailHistory.SQDID = ard.SQDID;
                    saleAREditeDetailHistory.SODID = ard.SODID;
                    saleAREditeDetailHistory.SDDID = ard.SDDID;
                    saleAREditeDetailHistory.ItemID = ard.ItemID;
                    saleAREditeDetailHistory.TaxGroupID = ard.TaxGroupID;
                    saleAREditeDetailHistory.TaxRate = ard.TaxRate;
                    saleAREditeDetailHistory.TaxValue = ard.TaxValue;
                    saleAREditeDetailHistory.TaxOfFinDisValue = ard.TaxOfFinDisValue;
                    saleAREditeDetailHistory.FinTotalValue = ard.FinTotalValue;
                    saleAREditeDetailHistory.ItemCode = ard.ItemCode;
                    saleAREditeDetailHistory.ItemNameKH = ard.ItemNameKH;
                    saleAREditeDetailHistory.ItemNameEN = ard.ItemNameEN;
                    saleAREditeDetailHistory.Qty = ard.Qty;
                    saleAREditeDetailHistory.OpenQty = ard.OpenQty;
                    saleAREditeDetailHistory.PrintQty = ard.PrintQty;
                    saleAREditeDetailHistory.GUomID = ard.GUomID;
                    saleAREditeDetailHistory.UomID = ard.UomID;
                    saleAREditeDetailHistory.UomName = ard.UomName;
                    saleAREditeDetailHistory.Factor = ard.Factor;
                    saleAREditeDetailHistory.Cost = ard.Cost;
                    saleAREditeDetailHistory.UnitPrice = ard.UnitPrice;
                    saleAREditeDetailHistory.DisRate = ard.DisRate;
                    saleAREditeDetailHistory.DisValue = ard.DisValue;
                    saleAREditeDetailHistory.FinDisRate = ard.FinDisRate;
                    saleAREditeDetailHistory.FinDisValue = ard.FinDisValue;
                    saleAREditeDetailHistory.TypeDis = ard.TypeDis;
                    saleAREditeDetailHistory.VatRate = ard.VatRate;
                    saleAREditeDetailHistory.VatValue = ard.VatValue;
                    saleAREditeDetailHistory.Total = ard.Total;
                    saleAREditeDetailHistory.TotalSys = ard.TotalSys;
                    saleAREditeDetailHistory.TotalWTax = ard.TotalWTax;
                    saleAREditeDetailHistory.TotalWTaxSys = ard.TotalWTaxSys;
                    saleAREditeDetailHistory.CurrencyID = ard.CurrencyID;
                    saleAREditeDetailHistory.ExpireDate = ard.ExpireDate;
                    saleAREditeDetailHistory.ItemType = ard.ItemType;
                    saleAREditeDetailHistory.Remarks = ard.Remarks;
                    saleAREditeDetailHistory.Delete = ard.Delete;
                    _context.Add(saleAREditeDetailHistory);
                    _context.SaveChanges();
                }
                // _isale.IssuseInStockARReserveInvoice(aRReserveInvoice.ID, type, ards, gldeter, freight);
                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", _culLocal["Your Invoice Journal Entry has reached the limitation!!"]);
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                _isale.CreateIncomingPaymentCustomerByARReserveInvoiceEDTAsync(aRReserveInvoice);

                t.Commit();
                ModelState.AddModelError("success", _culLocal["Item save successfully."]);
                msg.Approve();
            }
            return Ok(new { Model = msg.Bind(ModelState) });
        }

        public IActionResult FindARReserveInvoice(string number, int seriesID)
        {
            var list = _isale.FindARReserveInvoice(number, seriesID, GetCompany().ID);
            if (list.ARReserveInvoice != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        public IActionResult FindARReserveInvoiceEDT(string number, int seriesID)
        {
            var list = _isale.FindARReserveInvoiceEDT(number, seriesID, GetCompany().ID);

            return Ok(list);
        }
        private bool CreateIncomingPaymentCustomerByARReserveInvoice(ARReserveInvoice aRReserveInvoice)
        {
            string currencyName = _context.Currency.Find(aRReserveInvoice.SaleCurrencyID).Description;
            SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
            var payment = _context.IncomingPaymentCustomers.FirstOrDefault(p => p.InvoiceNumber == aRReserveInvoice.InvoiceNumber && p.SeriesID == aRReserveInvoice.SeriesID);
            var docType = _context.DocumentTypes.Find(aRReserveInvoice.DocTypeID);
            var em = _context.Employees.FirstOrDefault(i => i.ID == aRReserveInvoice.SaleEmID) ?? new Employee();
            var user = _userModule.CurrentUser;
            IncomingPaymentCustomer ipcustomer = new()
            {
                IncomingPaymentCustomerID = 0,
                EmID = em.ID,
                EmName = em.Name,
                CreatorID = user.ID,
                CreatorName = user.Username,
                CustomerID = aRReserveInvoice.CusID,
                BranchID = aRReserveInvoice.BranchID,
                WarehouseID = aRReserveInvoice.WarehouseID,
                DocTypeID = aRReserveInvoice.DocTypeID,
                SeriesID = aRReserveInvoice.SeriesID,
                SeriesDID = aRReserveInvoice.SeriesDID,
                CompanyID = aRReserveInvoice.CompanyID,
                InvoiceNumber = aRReserveInvoice.InvoiceNumber,
                CurrencyID = aRReserveInvoice.SaleCurrencyID,
                Types = aRReserveInvoice.Types = SaleCopyType.ARReserveInvoice.ToString(),
                //DocumentNo = saleAR.InvoiceNo,
                //DocumentType = GetTransactType(saleAR.InvoiceNo, saleAR.IncludeVat),
                Applied_Amount = aRReserveInvoice.AppliedAmount,
                CurrencyName = currencyName,
                ExchangeRate = aRReserveInvoice.ExchangeRate,
                TotalPayment = (aRReserveInvoice.TotalAmount - aRReserveInvoice.AppliedAmount),
                CashDiscount = 0,
                Total = aRReserveInvoice.SubTotal,
                TotalDiscount = 0,
                BalanceDue = (aRReserveInvoice.TotalAmount - aRReserveInvoice.AppliedAmount),
                Status = aRReserveInvoice.Status,
                Date = aRReserveInvoice.DueDate,
                PostingDate = aRReserveInvoice.PostingDate,
                SysCurrency = syCurrency.ID,
                SysName = syCurrency.Description,
                LocalCurID = aRReserveInvoice.LocalCurID,
                LocalSetRate = aRReserveInvoice.LocalSetRate,
                ItemInvoice = $"{docType.Code}-{aRReserveInvoice.InvoiceNumber}"
            };

            if (aRReserveInvoice.TotalAmount <= aRReserveInvoice.AppliedAmount)
            {
                aRReserveInvoice.Status = "close";
            }

            if (payment != null)
            {
                payment.Applied_Amount = aRReserveInvoice.AppliedAmount;
                payment.BalanceDue = aRReserveInvoice.TotalAmount - aRReserveInvoice.AppliedAmount;
                //payment.TotalPayment -= saleAR.AppliedAmount;
                payment.TotalPayment = aRReserveInvoice.TotalAmount - aRReserveInvoice.AppliedAmount;
                payment.Status = aRReserveInvoice.Status;
                var paymentDetails = _context.IncomingPaymentDetails.Where(ipd => ipd.DocumentNo == payment.DocumentNo);
                foreach (var pd in paymentDetails)
                {
                    pd.Delete = true;
                }
                _context.IncomingPaymentCustomers.Update(payment);
            }
            else
            {
                _context.IncomingPaymentCustomers.Add(ipcustomer);
            }

            _context.SaveChanges();
            return true;
        }
        //=================END OF AR RESERVE INVOICE=============
        //--------------------------------------------// Start Sale Credit Memo //------------------------------------//
        #region Sale Credit Memo
        [Privilege("SCMO")]
        public IActionResult SaleCreditMemo()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale Credit Memo";
            ViewBag.Sale = "show";
            ViewBag.SaleCreditMemo = "highlight";
            var seriesJE = (from dt in _context.DocumentTypes.Where(i => i.Code == "JE")
                            join sr in _context.Series.Where(w => !w.Lock) on dt.ID equals sr.DocuTypeID
                            select new SeriesInPurchasePoViewModel
                            {
                                ID = sr.ID,
                                Name = sr.Name,
                                Default = sr.Default,
                                NextNo = sr.NextNo,
                            }).ToList();
            return View(new { seriesCN = _utility.GetSeries("CN"), seriesJE, genSetting = _utility.GetGeneralSettingAdmin() });
        }
        private bool CreateIncomingPaymentCustomerBySaleCreditMemo(SaleCreditMemo saleCreditMemo, string type)
        {
            string currencyName = _context.Currency.Find(saleCreditMemo.SaleCurrencyID).Description;
            var docType = _context.DocumentTypes.Find(saleCreditMemo.DocTypeID);
            SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
            var em = _context.Employees.FirstOrDefault(i => i.ID == saleCreditMemo.SaleEmID) ?? new Employee();
            var user = _userModule.CurrentUser;
            if (type == "CN")
            {
                IncomingPaymentCustomer ipcustomerCN = new()
                {
                    CustomerID = saleCreditMemo.CusID,
                    EmID = em.ID,
                    EmName = em.Name,
                    CreatorID = user.ID,
                    CreatorName = user.Username,
                    BranchID = saleCreditMemo.BranchID,
                    WarehouseID = saleCreditMemo.WarehouseID,
                    DocTypeID = saleCreditMemo.DocTypeID,
                    SeriesID = saleCreditMemo.SeriesID,
                    SeriesDID = saleCreditMemo.SeriesDID,
                    CompanyID = saleCreditMemo.CompanyID,
                    InvoiceNumber = saleCreditMemo.InvoiceNumber,
                    CurrencyID = saleCreditMemo.SaleCurrencyID,
                    DocumentNo = saleCreditMemo.InvoiceNo,
                    DocumentType = GetTransactType(saleCreditMemo.InvoiceNo, saleCreditMemo.IncludeVat),
                    Applied_Amount = 0,
                    Types = SaleCopyType.CreditMemo.ToString(),
                    CurrencyName = currencyName,
                    ExchangeRate = saleCreditMemo.ExchangeRate,
                    TotalPayment = (saleCreditMemo.TotalAmount - saleCreditMemo.AppliedAmount) * -1,
                    CashDiscount = 0,
                    Total = (saleCreditMemo.TotalAmount - saleCreditMemo.AppliedAmount) * -1,//saleCreditMemo.SubTotal * -1,
                    TotalDiscount = 0,
                    BalanceDue = saleCreditMemo.SubTotal * -1,// saleCreditMemo.TotalAmount - saleCreditMemo.AppliedAmount,
                    Status = saleCreditMemo.Status,
                    Date = saleCreditMemo.DueDate,
                    PostingDate = saleCreditMemo.PostingDate,
                    SysCurrency = syCurrency.ID,
                    SysName = syCurrency.Description,
                    LocalCurID = saleCreditMemo.LocalCurID,
                    LocalSetRate = saleCreditMemo.LocalSetRate,
                    ItemInvoice = $"{docType.Code}-{saleCreditMemo.InvoiceNumber}"
                };
                _context.IncomingPaymentCustomers.Update(ipcustomerCN);
                _context.SaveChanges();

            }
            else
            {
                IncomingPaymentCustomer ipcustomer = new()
                {
                    CustomerID = saleCreditMemo.CusID,
                    EmID = em.ID,
                    EmName = em.Name,
                    CreatorID = user.ID,
                    CreatorName = user.Username,
                    BranchID = saleCreditMemo.BranchID,
                    WarehouseID = saleCreditMemo.WarehouseID,
                    DocTypeID = saleCreditMemo.DocTypeID,
                    SeriesID = saleCreditMemo.SeriesID,
                    SeriesDID = saleCreditMemo.SeriesDID,
                    CompanyID = saleCreditMemo.CompanyID,
                    InvoiceNumber = saleCreditMemo.InvoiceNumber,
                    CurrencyID = saleCreditMemo.SaleCurrencyID,
                    DocumentNo = saleCreditMemo.InvoiceNo,
                    DocumentType = GetTransactType(saleCreditMemo.InvoiceNo, saleCreditMemo.IncludeVat),
                    Applied_Amount = saleCreditMemo.AppliedAmount,
                    CurrencyName = currencyName,
                    ExchangeRate = saleCreditMemo.ExchangeRate,
                    TotalPayment = saleCreditMemo.TotalAmount - saleCreditMemo.AppliedAmount,
                    CashDiscount = 0,
                    Total = saleCreditMemo.SubTotal,
                    TotalDiscount = 0,
                    BalanceDue = saleCreditMemo.TotalAmount - saleCreditMemo.AppliedAmount,
                    Status = saleCreditMemo.Status,
                    Date = saleCreditMemo.DueDate,
                    PostingDate = saleCreditMemo.PostingDate,
                    SysCurrency = syCurrency.ID,
                    SysName = syCurrency.Description,
                    LocalCurID = saleCreditMemo.LocalCurID,
                    LocalSetRate = saleCreditMemo.LocalSetRate,
                    ItemInvoice = $"{docType.Code}-{saleCreditMemo.InvoiceNumber}"
                };
                _context.IncomingPaymentCustomers.Update(ipcustomer);
                _context.SaveChanges();
            }

            return true;
        }
        [HttpPost]
        public IActionResult CreateSaleCreditMemo(
   string data,
   string seriesJE,
   string ardownpayment,
   int seriesID,
   string type,
   string serial,
   string batch,
   string checkingSerialString = "",
   string checkingBatchString = "")
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            if (seriesJE == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(new { Model = msg.Bind(ModelState) });
            }
            Series seriesje = JsonConvert.DeserializeObject<Series>(seriesJE, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            SaleCreditMemo creditMemo = JsonConvert.DeserializeObject<SaleCreditMemo>(data, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            List<SaleARDPINCN> ards = JsonConvert.DeserializeObject<List<SaleARDPINCN>>(ardownpayment, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var seriesCD = _context.Series.FirstOrDefault(w => w.ID == creditMemo.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == creditMemo.DocTypeID).FirstOrDefault();
            var series_JE = _context.Series.FirstOrDefault(w => w.ID == seriesje.ID);
            var g = _isale.GetAllGroupDefind().ToList();
            string _type = type;
            creditMemo.ChangeLog = DateTime.Now;
            creditMemo.UserID = GetUserID();
          
            // Checking Serial Batch //
            List<SerialNumber> serialNumber = new();
            List<SerialNumber> _serialNumber = new();
            List<BatchNo> batchNoes = new();
            List<BatchNo> _batchNoes = new();
            // if (!string.IsNullOrEmpty(creditMemo.RefNo))
            // {
            //     bool isRefExisted = _context.SaleCreditMemos.AsNoTracking().Any(i => i.RefNo == creditMemo.RefNo);
            //     if (isRefExisted)
            //     {
            //         ModelState.AddModelError("RefNo", $"Transaction with Customer Ref No. \"{creditMemo.RefNo}\" already done.");
            //     }
            // }
            ValidateSummary(creditMemo, creditMemo.SaleCreditMemoDetails);
            CheckTaxAcc(creditMemo.SaleCreditMemoDetails);
            foreach (var dt in creditMemo.SaleCreditMemoDetails)
            {
                var factor = g.FirstOrDefault(w => w.GroupUoMID == dt.GUomID && w.AltUOM == dt.UomID).Factor;
                dt.Factor = factor;

                if (creditMemo.CopyType == 0)
                {
                    if (dt.Process != "Standard")
                    {
                        var _purchasedItems = _context.InventoryAudits.Where(ia => ia.ItemID == dt.ItemID && ia.Trans_Type == "PU").ToList();
                        if (_purchasedItems.Count > 0)
                        {
                            if (dt.Cost < 0 || dt.Cost * creditMemo.ExchangeRate > _purchasedItems.Max(p => p.Cost) * dt.Factor)
                            {
                                ModelState.AddModelError("itemDetailCost", "Item detail&lt;" + dt.ItemNameKH + "&gt; cost cannot be negative " +
                                    "or exceeds " + _purchasedItems.Max(p => p.Cost) * dt.Factor + ".");
                            }
                        }
                    }
                }
            }

            using (var t = _context.Database.BeginTransaction())
            {
                if (ModelState.IsValid)
                {
                    if (type == "CN")
                    {
                        serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) : serialNumber;

                        _saleSerialBatch.CheckItemSerail(creditMemo, creditMemo.SaleCreditMemoDetails.ToList(), serialNumber);
                        serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                        foreach (var j in serialNumber.ToList())
                        {
                            foreach (var i in creditMemo.SaleCreditMemoDetails.ToList())
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
                        batchNoes = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) : batchNoes;
                        _saleSerialBatch.CheckItemBatch(creditMemo, creditMemo.SaleCreditMemoDetails.ToList(), batchNoes);
                        batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                        foreach (var j in batchNoes.ToList())
                        {
                            foreach (var i in creditMemo.SaleCreditMemoDetails.ToList())
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
                    else if (type == "IN")
                    {
                        serialNumber = serial != "[]" ? JsonConvert.DeserializeObject<List<SerialNumber>>(serial, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) : serialNumber;
                        _saleSerialBatch.CheckItemSerailCopy(creditMemo, creditMemo.SaleCreditMemoDetails.ToList(), serialNumber, "In");
                        serialNumber = serialNumber.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                        foreach (var j in serialNumber.ToList())
                        {
                            foreach (var i in creditMemo.SaleCreditMemoDetails.ToList())
                            {
                                if (j.ItemID == i.ItemID)
                                {
                                    _serialNumber.Add(j);
                                }
                            }
                        }
                        if (checkingSerialString != "checked" && checkingSerialString == "unchecked" && _serialNumber.Count > 0)
                        {
                            return Ok(new { IsSerialCopy = true, Data = _serialNumber });
                        }
                        //SerialNumberSelected = null
                        batchNoes = batch != "[]" ? JsonConvert.DeserializeObject<List<BatchNo>>(batch, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }) : batchNoes;
                        _saleSerialBatch.CheckItemBatchCopy(creditMemo, creditMemo.SaleCreditMemoDetails.ToList(), batchNoes, "In", TransTypeWD.AR);
                        batchNoes = batchNoes.GroupBy(i => i.ItemID).Select(i => i.DefaultIfEmpty().Last()).ToList();
                        foreach (var j in batchNoes.ToList())
                        {
                            foreach (var i in creditMemo.SaleCreditMemoDetails.ToList())
                            {
                                if (j.ItemID == i.ItemID)
                                {
                                    _batchNoes.Add(j);
                                }
                            }
                        }
                        if (checkingBatchString != "checked" && checkingBatchString == "unchecked" && _batchNoes.Count > 0)
                        {
                            return Ok(new { IsBatchCopy = true, Data = _batchNoes });
                        }
                    }

                    seriesDetail.Number = seriesCD.NextNo;
                    seriesDetail.SeriesID = creditMemo.SeriesID;
                    _context.SeriesDetails.Update(seriesDetail);
                    _context.SaveChanges();
                    var seriesDetailID = seriesDetail.ID;
                    string Sno = seriesCD.NextNo;
                    long No = long.Parse(Sno);
                    creditMemo.InvoiceNumber = seriesCD.NextNo;
                    seriesCD.NextNo = Convert.ToString(No + 1).PadLeft(Sno.Length, '0');
                    if (No > long.Parse(seriesCD.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Your Sale Credit Memo Invoice has reached the limitation!!");
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                    if (SaleCopyType.AR == creditMemo.CopyType)
                    {
                        _type = "IN";
                        var number = creditMemo.CopyKey.Split("-")[1];
                        var ARMaster = _context.SaleARs.Include(o => o.SaleARDetails).FirstOrDefault(m => string.Compare(m.InvoiceNumber, number) == 0 && m.SeriesID == seriesID);
                        decimal balancedue = ((decimal)ARMaster.TotalAmount + (decimal)ARMaster.VatValue + (decimal)ARMaster.FreightAmount) - (decimal)ARMaster.AppliedAmount;
                        if ((decimal)creditMemo.TotalAmount > balancedue)
                        {
                            ModelState.AddModelError("TotalAmount", "Choose:<br/>1: TotalAmount = " + creditMemo.TotalAmount + " bigger than balancedue = " + balancedue + " in Sale AR. <br/> 2: please cancel Invoice Sale AR in IncomePaymant");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        creditMemo.SaleType = SaleCopyType.AR;
                        var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w => w.InvoiceNumber == number && w.SeriesID == seriesID);

                        if (ARMaster != null)
                        {
                            creditMemo.BasedOn = ARMaster.SARID;
                            if (ModelState.IsValid)
                            {
                                UpdateSourceCopy(creditMemo.SaleCreditMemoDetails, ARMaster, ARMaster.SaleARDetails, SaleCopyType.AR);
                                var _freight = _context.FreightSales.FirstOrDefault(i => i.SaleID == ARMaster.SARID);
                                if (_freight != null)
                                {
                                    _freight.OpenAmountReven -= creditMemo.FreightAmount;
                                    ARMaster.FreightAmount -= creditMemo.FreightAmount;
                                    ARMaster.FreightAmountSys = ARMaster.FreightAmount * (decimal)creditMemo.ExchangeRate;
                                    _context.FreightSales.Update(_freight);
                                }

                                ARMaster.AppliedAmount += creditMemo.AppliedAmount;
                                if (ARMaster.AppliedAmount >= ARMaster.TotalAmount)
                                {
                                    ARMaster.Status = "close";
                                }
                                _context.SaleARs.Update(ARMaster);
                                _context.SaveChanges();
                            }
                        }
                        if (incomingCus != null)
                        {
                            incomingCus.Applied_Amount += creditMemo.AppliedAmount;
                            incomingCus.BalanceDue -= creditMemo.AppliedAmount;
                            //incomingCus.TotalPayment -= creditMemo.AppliedAmount;
                            if (incomingCus.Applied_Amount >= incomingCus.TotalPayment)
                            {
                                incomingCus.Status = "close";
                            }
                            _context.IncomingPaymentCustomers.Update(incomingCus);
                        }

                        _context.SaveChanges();
                        var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                        var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
                        var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
                        if (seriesIn == null)
                        {
                            ModelState.AddModelError("SeriesIn", "Series Incoming Payment has No Data or Default One!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        if (paymentMean == null)
                        {
                            ModelState.AddModelError("SeriesIn", "Series Payment Means has No Data or Default One!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                    }
                    if (SaleCopyType.ARReserveInvoiceEDT == creditMemo.CopyType)
                    {
                        _type = "ARReserveInvoiceEDT";
                        var number = creditMemo.CopyKey.Split("-")[1];
                        var ARMaster = _context.ARReserveInvoiceEditables.Include(o => o.ARReserveInvoiceEditableDetails).FirstOrDefault(m => string.Compare(m.InvoiceNumber, number) == 0 && m.SeriesID == seriesID);
                        decimal balancedue = ((decimal)ARMaster.TotalAmount + (decimal)ARMaster.VatValue + (decimal)ARMaster.FreightAmount) - (decimal)ARMaster.AppliedAmount;
                        if ((decimal)creditMemo.TotalAmount > balancedue)
                        {
                            ModelState.AddModelError("TotalAmount", "Choose:<br/>1: TotalAmount= " + creditMemo.TotalAmount + " bigger than balancedue = " + balancedue + " in Sale ARReserveEDitable <br/> 2: please cancel Invoice ARReserveEDitable in IncomePaymant.");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }

                        creditMemo.SaleType = SaleCopyType.ARReserveInvoiceEDT;
                        var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w => w.InvoiceNumber == number && w.SeriesID == seriesID);
                        if (ARMaster != null)
                        {
                            creditMemo.BasedOn = ARMaster.ID;
                            if (ModelState.IsValid)
                            {
                                UpdateSourceCopy(creditMemo.SaleCreditMemoDetails, ARMaster, ARMaster.ARReserveInvoiceEditableDetails, SaleCopyType.ARReserveInvoiceEDT, true);


                                ARMaster.AppliedAmount += creditMemo.AppliedAmount;
                                if (ARMaster.AppliedAmount >= ARMaster.TotalAmount)
                                {
                                    ARMaster.Status = "close";
                                }
                                _context.ARReserveInvoiceEditables.Update(ARMaster);
                                _context.SaveChanges();
                            }
                        }
                        if (incomingCus != null)
                        {
                            incomingCus.Applied_Amount += creditMemo.AppliedAmount;
                            incomingCus.BalanceDue -= creditMemo.AppliedAmount;
                            //incomingCus.TotalPayment -= creditMemo.AppliedAmount;
                            if (incomingCus.Applied_Amount >= incomingCus.TotalPayment)
                            {
                                incomingCus.Status = "close";
                            }
                            _context.IncomingPaymentCustomers.Update(incomingCus);
                        }

                        _context.SaveChanges();
                        var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                        var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
                        var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
                        if (seriesIn == null)
                        {
                            ModelState.AddModelError("SeriesIn", "Series Incoming Payment has No Data or Default One!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        if (paymentMean == null)
                        {
                            ModelState.AddModelError("SeriesIn", "Series Payment Means has No Data or Default One!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                    }
                    if (SaleCopyType.ARReserveInvoice == creditMemo.CopyType)
                    {
                        _type = "ARReserveInvoice";
                        var number = creditMemo.CopyKey.Split("-")[1];
                        var ARMaster = _context.ARReserveInvoices.Include(o => o.ARReserveInvoiceDetails).FirstOrDefault(m => string.Compare(m.InvoiceNumber, number) == 0 && m.SeriesID == seriesID);
                        decimal balancedue = ((decimal)ARMaster.TotalAmount + (decimal)ARMaster.VatValue + (decimal)ARMaster.FreightAmount) - (decimal)ARMaster.AppliedAmount;
                        if ((decimal)creditMemo.TotalAmount > balancedue)
                        {
                            ModelState.AddModelError("TotalAmount", "Choose:<br/>1: TotalAmount = " + creditMemo.TotalAmount + " bigger than balancedue = " + balancedue + " in Sale ARReserveInvoice <br/> 2: please cancel Invoice ARReserveEDitable in IncomePaymant.");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        creditMemo.SaleType = SaleCopyType.ARReserveInvoice;
                        var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w => w.InvoiceNumber == number && w.SeriesID == seriesID);
                        if (ARMaster != null)
                        {
                            creditMemo.BasedOn = ARMaster.ID;
                            if (ModelState.IsValid)
                            {
                                UpdateSourceCopy(creditMemo.SaleCreditMemoDetails, ARMaster, ARMaster.ARReserveInvoiceDetails, SaleCopyType.ARReserveInvoice, true);


                                ARMaster.AppliedAmount += creditMemo.AppliedAmount;
                                if (ARMaster.AppliedAmount >= ARMaster.TotalAmount)
                                {
                                    ARMaster.Status = "close";
                                }
                                _context.ARReserveInvoices.Update(ARMaster);
                                _context.SaveChanges();
                            }
                        }
                        if (incomingCus != null)
                        {
                            incomingCus.Applied_Amount += creditMemo.AppliedAmount;
                            incomingCus.BalanceDue -= creditMemo.AppliedAmount;
                            //incomingCus.TotalPayment -= creditMemo.AppliedAmount;
                            if (incomingCus.Applied_Amount >= incomingCus.TotalPayment)
                            {
                                incomingCus.Status = "close";
                            }
                            _context.IncomingPaymentCustomers.Update(incomingCus);
                        }

                        _context.SaveChanges();
                        var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                        var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
                        var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
                        if (seriesIn == null)
                        {
                            ModelState.AddModelError("SeriesIn", "Series Incoming Payment has No Data or Default One!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        if (paymentMean == null)
                        {
                            ModelState.AddModelError("SeriesIn", "Series Payment Means has No Data or Default One!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                    }

                    else if (SaleCopyType.SaleAREdite == creditMemo.CopyType)
                    {
                        _type = "SaleAREdite";
                        var number = creditMemo.CopyKey.Split("-")[1];
                        var ARMaster = _context.SaleAREdites.Include(o => o.SaleAREditeDetails).FirstOrDefault(m => string.Compare(m.InvoiceNumber, number) == 0 && m.SeriesID == seriesID);
                        decimal balancedue = ((decimal)ARMaster.TotalAmount + (decimal)ARMaster.VatValue + (decimal)ARMaster.FreightAmount) - (decimal)ARMaster.AppliedAmount;
                        if ((decimal)creditMemo.TotalAmount > balancedue)
                        {
                            ModelState.AddModelError("TotalAmount", "Choose:<br/>1: TotalAmount = " + creditMemo.TotalAmount + " bigger than balancedue = " + balancedue + " in Sale A/R Editable <br/> 2: please cancel Invoice Sale A/R Editable in IncomePaymant.");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }

                        creditMemo.SaleType = SaleCopyType.SaleAREdite;
                        var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w => w.InvoiceNumber == number && w.SeriesID == seriesID);
                        if (ARMaster != null)
                        {
                            creditMemo.BasedOn = ARMaster.SARID;
                            if (ModelState.IsValid)
                            {
                                UpdateSourceCopy(creditMemo.SaleCreditMemoDetails, ARMaster, ARMaster.SaleAREditeDetails, SaleCopyType.SaleAREdite);

                                ARMaster.AppliedAmount += creditMemo.AppliedAmount;
                                if (ARMaster.AppliedAmount >= ARMaster.TotalAmount)
                                {
                                    ARMaster.Status = "close";
                                }
                                _context.SaleAREdites.Update(ARMaster);
                                _context.SaveChanges();
                            }
                        }
                        if (incomingCus != null)
                        {
                            incomingCus.Applied_Amount += creditMemo.AppliedAmount;
                            incomingCus.BalanceDue -= creditMemo.AppliedAmount;
                            //incomingCus.TotalPayment -= creditMemo.AppliedAmount;
                            if (incomingCus.Applied_Amount >= incomingCus.TotalPayment)
                            {
                                incomingCus.Status = "close";
                            }
                            _context.IncomingPaymentCustomers.Update(incomingCus);
                        }

                        _context.SaveChanges();
                        var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                        var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
                        var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
                        if (seriesIn == null)
                        {
                            ModelState.AddModelError("SeriesIn", "Series Incoming Payment has No Data or Default One!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        if (paymentMean == null)
                        {
                            ModelState.AddModelError("SeriesIn", "Series Payment Means has No Data or Default One!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                    }
                    else if (SaleCopyType.ARDownPayment == creditMemo.CopyType)
                    {
                        _type = "CD";
                        var number = creditMemo.CopyKey.Split("-")[1];
                        var ardownP = _context.ARDownPayments.Include(o => o.ARDownPaymentDetails)
                               .FirstOrDefault(m => string.Compare(m.InvoiceNumber, number) == 0 && m.SeriesID == seriesID);
                        creditMemo.SaleType = SaleCopyType.ARDownPayment;
                        var incomingCus = _context.IncomingPaymentCustomers.FirstOrDefault(w => w.InvoiceNumber == number && w.SeriesID == seriesID);
                        if (ardownP != null)
                        {
                            creditMemo.BasedOn = ardownP.ARDID;
                            if (ModelState.IsValid)
                            {
                                UpdateSourceCopy(creditMemo.SaleCreditMemoDetails, ardownP, ardownP.ARDownPaymentDetails, SaleCopyType.ARDownPayment);
                                ardownP.AppliedAmount += (decimal)creditMemo.AppliedAmount;
                                if (ardownP.AppliedAmount >= (decimal)ardownP.TotalAmount)
                                {
                                    ardownP.Status = "used";
                                }
                                _context.ARDownPayments.Update(ardownP);
                                _context.SaveChanges();
                            }
                        }
                        if (incomingCus != null)
                        {
                            incomingCus.Applied_Amount += creditMemo.AppliedAmount;
                            incomingCus.BalanceDue -= creditMemo.AppliedAmount;
                            incomingCus.TotalPayment -= creditMemo.AppliedAmount;
                            if (incomingCus.Applied_Amount >= incomingCus.TotalPayment)
                            {
                                incomingCus.Status = "close";
                            }
                            _context.IncomingPaymentCustomers.Update(incomingCus);
                        }
                        _context.SaveChanges();
                        var docTypeIn = _context.DocumentTypes.FirstOrDefault(i => i.Code == "RC");
                        var seriesIn = _context.Series.FirstOrDefault(i => i.DocuTypeID == docTypeIn.ID && i.Default);
                        var paymentMean = _context.PaymentMeans.FirstOrDefault(i => i.Default);
                        if (seriesIn == null)
                        {
                            ModelState.AddModelError("SeriesIn", "Series Incoming Payment has No Data or Default One!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                        if (paymentMean == null)
                        {
                            ModelState.AddModelError("SeriesIn", "Series Payment Means has No Data or Default One!!");
                            return Ok(new { Model = msg.Bind(ModelState) });
                        }
                    }
                    else
                    {
                        _type = "CN";
                        creditMemo.SaleType = SaleCopyType.CreditMemo;
                        creditMemo.LocalSetRate = localSetRate;
                    }
                    if (type != "CN")
                    {
                        if (creditMemo.TotalAmount <= creditMemo.AppliedAmount)
                        {
                            creditMemo.Status = "close";
                        }
                    }

                    creditMemo.LocalCurID = GetCompany().LocalCurrencyID;
                    creditMemo.LocalSetRate = localSetRate;
                    creditMemo.SeriesDID = seriesDetailID;
                    creditMemo.CompanyID = GetCompany().ID;
                    _context.SaleCreditMemos.Update(creditMemo);
                    _context.SaveChanges();
                    var gldeter = _context.SaleGLAccountDeterminations.FirstOrDefault(i => i.Code == "DPCA");
                    if (gldeter != null)
                    {
                        if (gldeter.GLID == 0)
                        {
                            ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is missing!!"]);
                            return Ok(msg.Bind(ModelState));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("SaleGLDeter", _culLocal["Sale GLAcc Determination is not set up yet!!"]);
                        return Ok(msg.Bind(ModelState));
                    }
                    var freight = creditMemo.FreightSalesView;
                    if (freight != null)
                    {
                        freight.SaleID = creditMemo.SCMOID;
                        freight.SaleType = SaleCopyType.CreditMemo;
                        freight.OpenAmountReven = freight.AmountReven;
                        freight.OpenAmountReven = freight.OpenAmountReven;
                        _context.FreightSales.Update(freight);
                        _context.SaveChanges();
                    }
                    if (SaleCopyType.ARReserveInvoiceEDT == creditMemo.CopyType)
                    {
                        var aredt = _context.ARReserveInvoiceEditables.FirstOrDefault(s => s.ID == creditMemo.BasedOn) ?? new ARReserveInvoiceEditable();
                        bool transaction_Delivery = true;
                        _returnOrCancelStockMaterial.IssuseInStockARReserveInvoiceEDT(creditMemo.SCMOID, _type, ards, gldeter, freight, _serialNumber, _batchNoes, TransTypeWD.AR_REDT, creditMemo.BasedOn, transaction_Delivery);
                    }
                    else
                        _returnOrCancelStockMaterial.CreditmemoReturnStock(creditMemo.SCMOID, _type, ards, gldeter, freight, _serialNumber, _batchNoes, TransTypeWD.AR, creditMemo.BasedOn);
                    // checking maximun Invoice
                    var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                    var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                    if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                    {
                        ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                        return Ok(new { Model = msg.Bind(ModelState) });
                    }
                    if (SaleCopyType.AR != creditMemo.CopyType)
                    {
                        CreateIncomingPaymentCustomerBySaleCreditMemo(creditMemo, type);
                    }

                    t.Commit();
                    ModelState.AddModelError("success", "Item save successfully.");
                    msg.Approve();
                }
            }
            return Ok(new { Model = msg.Bind(ModelState), ItemReturn = new List<ItemsReturn>() });
        }

        [HttpGet]
        public IActionResult FindSaleCreditMemo(string number, int seriesID)
        {
            var list = _isale.FindSaleCreditMemo(number, seriesID, GetCompany().ID);
            if (list.SaleCreditMemo != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        #endregion
        //--------------------------------------------// End Sale Credit Memo //------------------------------------//
        //----Sale Quote History-----//
        public IActionResult SaleQuoteHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale Quote History";
            ViewBag.Sale = "show";
            ViewBag.SaleQuotation = "highlight";
            ViewBag.Devlivery = new SelectList(_context.Employees.Where(i => !i.Delete), "ID", "Name");
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();

            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.Quotation).Where(h => GetValidDate(h.PostingDate).CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>(),
                Templateurl = _utility.PrintTemplateUrl()
            };

            return View(history);
        }

        public IActionResult ARReserveInvoiceEDTHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = " AR Reserve Invoice Editable History";
            ViewBag.Sale = "show";
            ViewBag.ARReserveInvoiceEDT = "highlight";
            ViewBag.Devlivery = new SelectList(_context.Employees.Where(i => !i.Delete), "ID", "Name");
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();

            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.ARReEDT).Where(h => GetValidDate(h.PostingDate).CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>(),
                Templateurl = _utility.PrintTemplateUrl()
            };
            return View(history);
        }
        //----Sale Order History-----//
        public IActionResult SaleOrderHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale Order History";
            ViewBag.Sale = "show";
            ViewBag.SaleOrder = "highlight";
            ViewBag.Devlivery = new SelectList(_context.Employees.Where(i => !i.Delete), "ID", "Name");
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();

            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.Order).Where(h => GetValidDate(h.PostingDate).CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>(),
                Templateurl = _utility.PrintTemplateUrl()
            };
            return View(history);
        }

        //----Sale SaleDeliveryHistory-----//
        public IActionResult SaleDeliveryHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale Delivery History";
            ViewBag.Sale = "show";
            ViewBag.SaleDelivery = "highlight";
            ViewBag.Devlivery = new SelectList(_context.Employees.Where(i => !i.Delete), "ID", "Name");
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();

            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.Delivery).Where(h => GetValidDate(h.PostingDate).CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>(),
                Templateurl = _utility.PrintTemplateUrl()
            };
            return View(history);
        }

        [Privilege("RD")]
        public IActionResult ReturnDeliveryHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Return Delivery History";
            ViewBag.Sale = "show";
            ViewBag.ReturnDelivery = "highlight";
            ViewBag.Devlivery = new SelectList(_context.Employees.Where(i => !i.Delete), "ID", "Name");
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();

            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.ReturnDelivery).Where(h => GetValidDate(h.PostingDate).CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>()
            };
            return View(history);
        }
        public IActionResult ARDownPaymentHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "AR Down Payment Delivery History";
            ViewBag.Sale = "show";
            ViewBag.ARDownPaymentInvoice = "highlight";
            ViewBag.Devlivery = new SelectList(_context.Employees.Where(i => !i.Delete), "ID", "Name");
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();

            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.ARDownPayment).Where(h => GetValidDate(h.PostingDate).CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>(),
                Templateurl = _utility.PrintTemplateUrl()
            };
            return View(history);
        }
        //----Sale AR History-----//
        public IActionResult SaleARHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale AR History";
            ViewBag.Sale = "show";
            ViewBag.SaleAR = "highlight";
            ViewBag.Devlivery = new SelectList(_context.Employees.Where(i => !i.Delete), "ID", "Name");
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();
            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.AR).Where(h => GetValidDate(h.PostingDate).CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>(),
                Templateurl = _utility.PrintTemplateUrl()
            };
            return View(history);
        }
        //==================AR Reserve Invoice History====================
        [Privilege("ARR")]
        public IActionResult ARReserveInvoiceHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = " AR Reserve Invoice History";
            ViewBag.Sale = "show";
            ViewBag.ARReserveInvoice = "highlight";
            ViewBag.Devlivery = new SelectList(_context.Employees.Where(i => !i.Delete), "ID", "Name");
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();

            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.ARReserve).Where(h => GetValidDate(h.PostingDate).CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>(),
                Templateurl = _utility.PrintTemplateUrl()
            };
            return View(history);
        }
        //=================end AR Reserve Invoice History============

        private static DateTime GetValidDate(string dateString)
        {
            string[] _date = dateString.Split("-");
            string _dateString = string.Format($"{_date[2]}/{_date[0]}/{_date[1]}");
            _ = DateTime.TryParse(_dateString, out DateTime _validDate);
            return _validDate;
        }

        //----Sale Credit Memo History-----//
        public IActionResult SaleCreditMemoHistory()
        {
            ViewBag.style = "fa-dollar-sign";
            ViewBag.Main = "Sale";
            ViewBag.Page = "Sale Credit Memo History";
            ViewBag.Sale = "show";
            ViewBag.SaleCreditMemo = "highlight";
            ViewBag.Devlivery = new SelectList(_context.Employees.Where(i => !i.Delete), "ID", "Name");
            List<BusinessPartner> customers = _context.BusinessPartners.Where(c => !c.Delete && c.Type.ToLower() == "customer").ToList();
            List<Warehouse> warehouses = _context.Warehouses.Where(c => !c.Delete).ToList();

            var saleHistories = GetSaleHistoriesByType(HistoryOfSaleType.CreditMemo).Where(h => GetValidDate(h.PostingDate)
                .CompareTo(DateTime.Today) >= 0 && GetValidDate(h.PostingDate).CompareTo(DateTime.Today) <= 0);
            HistoryOfSaleViewModel history = new()
            {
                Customers = customers ?? new List<BusinessPartner>(),
                Warhouses = warehouses ?? new List<Warehouse>(),
                SaleHistories = saleHistories.ToList() ?? new List<HistoryOfSale>(),
                Templateurl = _utility.PrintTemplateUrl()
            };
            return View(history);
        }

        public IActionResult SearchSaleHistory(HistoryOfSaleType saleType, string word = "")
        {
            if (word != null)
            {
                string _word = word.Replace(" ", "").ToLowerInvariant();
                return Ok(GetSaleHistoriesByType(saleType).Where(h =>
                                    string.Compare(h.InvoiceNo.Trim().ToLowerInvariant(), _word, true) == 0
                                    || h.CustomerName.Replace(" ", "").ToLowerInvariant().Contains(_word)
                       ));
            }
            return Ok();
        }

        public IActionResult FilterSaleHistory(HistoryOfSaleFilter filter, string DateFrom, string DateTo)
        {
            var saleHistories = GetSaleHistoriesByType(filter.SaleType);
            switch (filter.DateType)
            {
                case SaleDateType.PosingDate:
                    saleHistories = saleHistories.Where(h => DateTime.Parse(h.PostingDate)
                        .CompareTo(filter.DateFrom) >= 0 && DateTime.Parse(h.PostingDate).CompareTo(filter.DateTo) <= 0);
                    break;
                case SaleDateType.DocumentDate:
                    saleHistories = saleHistories.Where(h => DateTime.Parse(h.DocumentDate)
                        .CompareTo(filter.DateFrom) >= 0 && DateTime.Parse(h.DocumentDate).CompareTo(filter.DateTo) <= 0);
                    break;
                case SaleDateType.DueDate:
                    saleHistories = saleHistories.Where(h => DateTime.Parse(h.DueDate)
                        .CompareTo(filter.DateFrom) >= 0 && DateTime.Parse(h.DueDate).CompareTo(filter.DateTo) <= 0);
                    break;
            }
            // filterCustomer // date
            if (filter.Customer != 0 && filter.Warehouse == 0 && DateFrom != null && DateTo != null && filter.Check == false && filter.ShippedBy == 0)
            {
                saleHistories = saleHistories.Where(w => Convert.ToDateTime(w.PostingDate) >= filter.DateFrom && Convert.ToDateTime(w.PostingDate) <= filter.DateTo && w.CustomerID == filter.Customer).ToList();
            }
            //filterCustomer and Warehouse // date
            else if (filter.Customer != 0 && filter.Warehouse != 0 && DateFrom != null && DateTo != null && filter.Check == false && filter.ShippedBy == 0)
            {
                saleHistories = saleHistories.Where(w => Convert.ToDateTime(w.PostingDate) >= filter.DateFrom && Convert.ToDateTime(w.PostingDate) <= filter.DateTo && w.CustomerID == filter.Customer && w.WarehouseID == filter.Warehouse).ToList();
            }
            //filter Customer warehouse shipby // date
            else if (filter.Customer != 0 && filter.Warehouse != 0 && DateFrom != null && DateTo != null && filter.Check == false && filter.ShippedBy != 0)
            {
                saleHistories = saleHistories.Where(w => Convert.ToDateTime(w.PostingDate) >= filter.DateFrom && Convert.ToDateTime(w.PostingDate) <= filter.DateTo && w.CustomerID == filter.Customer && w.WarehouseID == filter.Warehouse && w.ShippedBy == filter.ShippedBy).ToList();
            }
            //filter check
            else if (filter.Customer == 0 && filter.Warehouse == 0 && DateFrom == null && DateTo == null && filter.Check == true && filter.ShippedBy == 0)
            {
                saleHistories = saleHistories.ToList();
            }
            //filter ShippedBy //date
            else if (filter.Customer == 0 && filter.Warehouse == 0 && DateFrom != null && DateTo != null && filter.Check == false && filter.ShippedBy != 0)
            {
                saleHistories = saleHistories.Where(w => Convert.ToDateTime(w.PostingDate) >= filter.DateFrom && Convert.ToDateTime(w.PostingDate) <= filter.DateTo && w.ShippedBy == filter.ShippedBy).ToList();
            }
            //filter ShippedBy
            else if (filter.Customer == 0 && filter.Warehouse == 0 && DateFrom == null && DateTo == null && filter.Check == false && filter.ShippedBy != 0)
            {
                saleHistories = saleHistories.Where(w => w.ShippedBy == filter.ShippedBy).ToList();
            }
            //filter customer and Warehourse
            else if (filter.Customer != 0 && filter.Warehouse != 0 && DateFrom == null && DateTo == null && filter.Check == false && filter.ShippedBy == 0)
            {
                saleHistories = saleHistories.Where(w => w.CustomerID == filter.Customer && w.WarehouseID == filter.Warehouse).ToList();
            }
            //filter Warehourse
            else if (filter.Customer == 0 && filter.Warehouse != 0 && DateFrom == null && DateTo == null && filter.Check == false && filter.ShippedBy == 0)
            {
                saleHistories = saleHistories.Where(w => w.WarehouseID == filter.Warehouse).ToList();
            }
            //filter customer and Warehourse and Shipped By
            else if (filter.Customer != 0 && filter.Warehouse != 0 && DateFrom == null && DateTo == null && filter.Check == false && filter.ShippedBy != 0)
            {
                saleHistories = saleHistories.Where(w => w.CustomerID == filter.Customer && w.WarehouseID == filter.Warehouse && w.ShippedBy == filter.ShippedBy).ToList();
            }
            //filter customer and shippedby
            else if (filter.Customer != 0 && filter.Warehouse == 0 && DateFrom == null && DateTo == null && filter.Check == false && filter.ShippedBy != 0)
            {
                saleHistories = saleHistories.Where(w => w.CustomerID == filter.Customer && w.ShippedBy == filter.ShippedBy).ToList();
            }
            //filter warehourse and shippedby
            else if (filter.Customer == 0 && filter.Warehouse != 0 && DateFrom == null && DateTo == null && filter.Check == false && filter.ShippedBy != 0)
            {
                saleHistories = saleHistories.Where(w => w.WarehouseID == filter.Warehouse && w.ShippedBy == filter.ShippedBy).ToList();
            }
            // filterDatefrom Dateto
            else if (filter.Customer == 0 && filter.Warehouse == 0 && DateFrom != null && DateTo != null && filter.Check == false && filter.ShippedBy == 0)
            {
                saleHistories = saleHistories.Where(w => Convert.ToDateTime(w.PostingDate) >= filter.DateFrom && Convert.ToDateTime(w.PostingDate) <= filter.DateTo).ToList();
            }
            //filter customer
            else if (filter.Customer != 0 && filter.Warehouse == 0 && DateFrom == null && DateTo == null && filter.Check == false && filter.ShippedBy == 0)
            {
                saleHistories = saleHistories.Where(w => w.CustomerID == filter.Customer).ToList();
            }
            //filter warehouse and date
            else if (filter.Customer == 0 && filter.Warehouse != 0 && DateFrom != null && DateTo != null && filter.Check == false && filter.ShippedBy == 0)
            {
                saleHistories = saleHistories.Where(w => w.WarehouseID == filter.Warehouse && Convert.ToDateTime(w.PostingDate) >= filter.DateFrom && Convert.ToDateTime(w.PostingDate) <= filter.DateTo).ToList();
            }
            //filter check and date
            else if (filter.Customer == 0 && filter.Warehouse == 0 && DateFrom != null && DateTo != null && filter.Check == true && filter.ShippedBy == 0)
            {
                saleHistories = saleHistories.Where(w => Convert.ToDateTime(w.PostingDate) >= filter.DateFrom && Convert.ToDateTime(w.PostingDate) <= filter.DateTo).ToList();
            }
            else
            {
                return Ok();
            }
            return Ok(saleHistories);
        }


        private IEnumerable<HistoryOfSale> GetSaleHistoriesByType(HistoryOfSaleType saleType)
        {
            switch (saleType)
            {
                case HistoryOfSaleType.Quotation:
                    return GetSaleHistories(_context.SaleQuotes, "SQID", HistoryOfSaleType.Quotation);
                case HistoryOfSaleType.Order:
                    return GetSaleHistories(_context.SaleOrders, "SOID", HistoryOfSaleType.Order);
                case HistoryOfSaleType.Delivery:
                    return GetSaleHistories(_context.SaleDeliveries, "SDID", HistoryOfSaleType.Delivery);
                case HistoryOfSaleType.ReturnDelivery:
                    return GetSaleHistories(_context.ReturnDeliverys, "ID", HistoryOfSaleType.ReturnDelivery);
                case HistoryOfSaleType.ARDownPayment:
                    return GetSaleHistories(_context.ARDownPayments, "ARDID", HistoryOfSaleType.ARDownPayment);
                case HistoryOfSaleType.AR:
                    return GetSaleHistories(_context.SaleARs, "SARID", HistoryOfSaleType.AR);
                case HistoryOfSaleType.AREdite:
                    return GetSaleHistories(_context.SaleAREdites, "SARID", HistoryOfSaleType.AREdite);
                case HistoryOfSaleType.ARReserve:
                    return GetSaleHistories(_context.ARReserveInvoices, "ID", HistoryOfSaleType.ARReserve);
                case HistoryOfSaleType.CreditMemo:
                    return GetSaleHistories(_context.SaleCreditMemos, "SCMOID", HistoryOfSaleType.CreditMemo);
                case HistoryOfSaleType.ARReEDT:
                    return GetSaleHistories(_context.ARReserveInvoiceEditables, "ID", HistoryOfSaleType.ARReEDT);
                default:
                    break;
            }
            return new List<HistoryOfSale>();
        }
        //========================AR Reserve Invice History===============
        public IActionResult SearchARReserveHistory(HistoryOfSaleType saleType, string word = "")
        {
            if (word != null)
            {
                string _word = word.Replace(" ", "").ToLowerInvariant();
                return Ok(GetSaleHistoriesByType(saleType).Where(h =>
                                    string.Compare(h.InvoiceNo.Trim().ToLowerInvariant(), _word, true) == 0
                                    || h.CustomerName.Replace(" ", "").ToLowerInvariant().Contains(_word)
                       ));
            }
            return Ok();
        }


        public IActionResult FilterARReserveHistory(HistoryOfSaleFilter filter)
        {
            var saleHistories = GetSaleHistoriesByType(filter.SaleType);
            switch (filter.DateType)
            {
                case SaleDateType.PosingDate:
                    saleHistories = saleHistories.Where(h => DateTime.Parse(h.PostingDate)
                        .CompareTo(filter.DateFrom) >= 0 && DateTime.Parse(h.PostingDate).CompareTo(filter.DateTo) <= 0);
                    break;
                case SaleDateType.DocumentDate:
                    saleHistories = saleHistories.Where(h => DateTime.Parse(h.DocumentDate)
                        .CompareTo(filter.DateFrom) >= 0 && DateTime.Parse(h.DocumentDate).CompareTo(filter.DateTo) <= 0);
                    break;
                case SaleDateType.DueDate:
                    saleHistories = saleHistories.Where(h => DateTime.Parse(h.DueDate)
                        .CompareTo(filter.DateFrom) >= 0 && DateTime.Parse(h.DueDate).CompareTo(filter.DateTo) <= 0);
                    break;
            }

            if (filter.Customer > 0)
            {
                saleHistories = saleHistories.Where(h => h.CustomerID == filter.Customer);
            }

            if (filter.Warehouse > 0)
            {
                saleHistories = saleHistories.Where(h => h.WarehouseID == filter.Warehouse);
            }

            return Ok(saleHistories);
        }

        //=========================end AR Reserve Invoice History===========
        public Dictionary<int, string> Typevat => EnumHelper.ToDictionary(typeof(VatType));
        public Dictionary<int, string> TypevatB => EnumHelper.ToDictionary(typeof(VatTypeB));
        private IEnumerable<HistoryOfSale> GetSaleHistories<T>(IEnumerable<T> saleTypes, string key, HistoryOfSaleType saleType)
        {
            string duedate = "DueDate";
            switch (saleType)
            {
                case HistoryOfSaleType.Quotation:
                    duedate = "ValidUntilDate";
                    break;
                case HistoryOfSaleType.Order:
                    duedate = "DeliveryDate";
                    break;
            }

            var saleHistories = from sa in saleTypes
                                join c in _context.BusinessPartners.Where(bp => bp.Type.ToLower() == "customer" && !bp.Delete)
                                on typeof(T).GetProperty("CusID").GetValue(sa) equals c.ID
                                join w in _context.Warehouses.Where(wh => !wh.Delete) on typeof(T).GetProperty("WarehouseID").GetValue(sa) equals w.ID
                                join cu in _context.Currency on typeof(T).GetProperty("SaleCurrencyID").GetValue(sa) equals cu.ID
                                join docType in _context.DocumentTypes on typeof(T).GetProperty("DocTypeID").GetValue(sa) equals docType.ID
                                join s in _context.Series on typeof(T).GetProperty("SeriesID").GetValue(sa) equals s.ID
                                let emp = _context.Employees.Where(x => x.ID == (int)(typeof(T).GetProperty("SaleEmID").GetValue(sa))).FirstOrDefault() ?? new Employee()
                                // join em in _context.Employees on typeof(T).GetProperty("ShippedBy").GetValue(sa) equals em.ID
                                let em = _context.Employees.Where(x => x.ID == (int)(typeof(T).GetProperty("ShippedBy").GetValue(sa))).FirstOrDefault() ?? new Employee()
                                select new HistoryOfSale
                                {
                                    ID = int.Parse(GetValue(sa, key).ToString()),
                                    InvoiceNo = $"{s.Name}-{GetValue(sa, "InvoiceNumber")}",
                                    CustomerName = c.Name,
                                    EmployeeName = emp.Name,
                                    // UserName = HttpContext.User.Identity.Name,
                                    BalanceDueLC = string.Format("{0} {1:N3}", cu.Description, GetValue(sa, "TotalAmount")),
                                    BalanceDueSC = string.Format("{0} {1:N3}", cu.Description, GetValue(sa, "TotalAmountSys")),
                                    //ExchangeRate = string.Format("{0:N3}", 1 / Convert.ToDecimal(GetValue(sa, "ExchangeRate"))),
                                    ExchangeRate = string.Format("{0:N2}", GetValue(sa, "ExchangeRate")),
                                    Status = typeof(T).GetProperty("Status").GetValue(sa).ToString(),
                                    PostingDate = ((DateTime)GetValue(sa, "PostingDate")).ToString("MM-dd-yyyy"),
                                    DocumentDate = ((DateTime)GetValue(sa, "DocumentDate")).ToString("MM-dd-yyyy"),
                                    DueDate = ((DateTime)GetValue(sa, duedate)).ToString("MM-dd-yyyy"),
                                    CustomerID = c.ID,
                                    WarehouseID = w.ID,
                                    TypeVatNumber = 0,
                                    ShippedBy = em.ID,

                                    VatType = Typevat.Select(s => new SelectListItem
                                    {
                                        Value = s.Key.ToString(),
                                        Text = s.Value
                                    }).ToList(),
                                    VatTypeB = TypevatB.Select(s => new SelectListItem
                                    {
                                        Value = s.Key.ToString(),
                                        Text = s.Value
                                    }).ToList(),
                                };
            return saleHistories;
        }
        //----End Sale History-----//

        private static object GetValue<T>(T obj, string prop)
        {
            var data = typeof(T).GetProperty(prop).GetValue(obj);
            return data;
        }
        // copy Project Cost
        [HttpGet]
        public async Task<IActionResult> GetstoryProjectCost()
        {
            var list = await _isale.GetStoryProjcostAsyce();
            if (list != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        [HttpGet]
        public IActionResult CopyProjectCostAnalysis(string number, int seriesID, int comId)
        {
            var list = _isale.FindProjectCostAnalysis(number, seriesID, comId);
            if (list.SaleQuote != null)
            {
                return Ok(list);
            }
            return Ok();
        }
        // sale employee
        [HttpGet]
        public IActionResult GetSaleEmployee()
        {
            var list = (from em in _context.Employees.Where(s => s.Delete == false)
                        select new Employee
                        {
                            ID = em.ID,
                            Code = em.Code,
                            Name = em.Name,
                            GenderDisplay = em.Gender == 0 ? "Male" : "Female",
                            Position = em.Position,
                            Phone = em.Phone,
                            Email = em.Email,
                            Address = em.Address,
                            EMType = em.EMType,
                        }).OrderBy(s => s.Name).ToList();
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetOnlySaleEmployee(int id)
        {
            var obj = _context.Employees.FirstOrDefault(s => s.ID == id) ?? new Employee();
            return Ok(obj);
        }
        public IActionResult GetSaleQuotesDisplay()
        {
            var allItems = (from SQ in _context.SaleQuotes.Where(i => i.Status == "open")
                            join docType in _context.DocumentTypes on SQ.DocTypeID equals docType.ID
                            let cur = _context.Currency.FirstOrDefault(i => i.ID == SQ.SaleCurrencyID)
                            select new
                            {
                                SQ.SQID,
                                SQ.SeriesID,
                                InvoiceNo = SQ.InvoiceNumber,
                                InvoiceNumber = $"{docType.Code}-{SQ.InvoiceNumber}",
                                SQ.PostingDate,
                                Currency = cur.Description,
                                SQ.SubTotal,
                                SQ.TotalAmount,
                                SQ.Remarks,
                            }).ToList();
            return Ok(allItems);
        }
        public IActionResult GetSaleOrderDisplay()
        {
            var allItems = (from SQ in _context.SaleOrders.Where(i => i.Status == "open")
                            join docType in _context.DocumentTypes on SQ.DocTypeID equals docType.ID
                            let cur = _context.Currency.FirstOrDefault(i => i.ID == SQ.SaleCurrencyID)
                            select new
                            {
                                SQ.SOID,
                                SQ.SeriesID,
                                InvoiceNo = SQ.InvoiceNumber,
                                InvoiceNumber = $"{docType.Code}-{SQ.InvoiceNumber}",
                                SQ.PostingDate,
                                Currency = cur.Description,
                                SQ.SubTotal,
                                SQ.TotalAmount,
                                SQ.TypeDis,
                                SQ.Remarks,
                            }).ToList();
            return Ok(allItems);
        }
        public IActionResult GetSaleDeliveriesDisplay()
        {
            var allItems = (from SQ in _context.SaleDeliveries.Where(i => i.Status == "open")
                            join docType in _context.DocumentTypes on SQ.DocTypeID equals docType.ID
                            let cur = _context.Currency.FirstOrDefault(i => i.ID == SQ.SaleCurrencyID)
                            select new
                            {
                                SQ.SDID,
                                SQ.SeriesID,
                                InvoiceNo = SQ.InvoiceNumber,
                                InvoiceNumber = $"{docType.Code}-{SQ.InvoiceNumber}",
                                SQ.PostingDate,
                                Currency = cur.Description,
                                SQ.SubTotal,
                                SQ.TotalAmount,
                                SQ.TypeDis,
                                SQ.Remarks,
                            }).ToList();
            return Ok(allItems);
        }
        public IActionResult GetSaleCrediMemoDisplay()
        {
            var allItems = (from SQ in _context.SaleCreditMemos.Where(i => i.Status == "open")
                            join docType in _context.DocumentTypes on SQ.DocTypeID equals docType.ID
                            let cur = _context.Currency.FirstOrDefault(i => i.ID == SQ.SaleCurrencyID)
                            select new
                            {
                                SQ.SCMOID,
                                SQ.SeriesID,
                                InvoiceNo = SQ.InvoiceNumber,
                                InvoiceNumber = $"{docType.Code}-{SQ.InvoiceNumber}",
                                SQ.PostingDate,
                                Currency = cur.Description,
                                SQ.SubTotal,
                                SQ.TotalAmount,
                                SQ.TypeDis,
                                SQ.Remarks,
                            }).ToList();
            return Ok(allItems);
        }
        public IActionResult GetSaleARDownDisplay()
        {
            var allItems = (from SQ in _context.ARDownPayments.Where(i => i.Status == "open")
                            join docType in _context.DocumentTypes on SQ.DocTypeID equals docType.ID
                            let cur = _context.Currency.FirstOrDefault(i => i.ID == SQ.SaleCurrencyID)
                            select new
                            {
                                SQ.ARDID,
                                SQ.SeriesID,
                                InvoiceNo = SQ.InvoiceNumber,
                                InvoiceNumber = $"{docType.Code}-{SQ.InvoiceNumber}",
                                SQ.PostingDate,
                                Currency = cur.Description,
                                SQ.SubTotal,
                                SQ.TotalAmount,
                                SQ.TypeDis,
                                SQ.Remarks,
                            }).ToList();
            return Ok(allItems);
        }
        public IActionResult GetSaleARDisplay()
        {
            var allItems = (from SQ in _context.SaleARs.Where(i => i.Status == "open")
                            join docType in _context.DocumentTypes on SQ.DocTypeID equals docType.ID
                            let cur = _context.Currency.FirstOrDefault(i => i.ID == SQ.SaleCurrencyID)
                            select new
                            {
                                SQ.SARID,
                                SQ.SeriesID,
                                InvoiceNo = SQ.InvoiceNumber,
                                InvoiceNumber = $"{docType.Code}-{SQ.InvoiceNumber}",
                                SQ.PostingDate,
                                Currency = cur.Description,
                                SQ.SubTotal,
                                SQ.TotalAmount,
                                SQ.TypeDis,
                                SQ.Remarks,
                            }).ToList();
            return Ok(allItems);
        }
        public IActionResult GetSaleAREditDisplay()
        {
            var allItems = (from SQ in _context.SaleAREdites.Where(i => i.Status == "open")
                            join docType in _context.DocumentTypes on SQ.DocTypeID equals docType.ID
                            let cur = _context.Currency.FirstOrDefault(i => i.ID == SQ.SaleCurrencyID)
                            select new
                            {
                                SQ.SARID,
                                SQ.SeriesID,
                                InvoiceNo = SQ.InvoiceNumber,
                                InvoiceNumber = $"{docType.Code}-{SQ.InvoiceNumber}",
                                SQ.PostingDate,
                                Currency = cur.Description,
                                SQ.SubTotal,
                                SQ.TotalAmount,
                                SQ.TypeDis,
                                SQ.Remarks,
                            }).ToList();
            return Ok(allItems);
        }
        public IActionResult GetARReserveInvoiceDisplay()
        {
            var allItems = (from SQ in _context.ARReserveInvoices.Where(i => i.Status == "open")
                            join docType in _context.DocumentTypes on SQ.DocTypeID equals docType.ID
                            let cur = _context.Currency.FirstOrDefault(i => i.ID == SQ.SaleCurrencyID)
                            select new
                            {
                                SQ.ID,
                                SQ.SeriesID,
                                InvoiceNo = SQ.InvoiceNumber,
                                InvoiceNumber = $"{docType.Code}-{SQ.InvoiceNumber}",
                                SQ.PostingDate,
                                Currency = cur.Description,
                                SQ.SubTotal,
                                SQ.TotalAmount,
                                SQ.TypeDis,
                                SQ.Remarks,
                            }).ToList();
            return Ok(allItems);
        }
        public IActionResult GetARReserveInvoiceEDTDisplay()
        {
            var allItems = (from SQ in _context.ARReserveInvoiceEditables.Where(i => i.Status == "open")
                            join docType in _context.DocumentTypes on SQ.DocTypeID equals docType.ID
                            let cur = _context.Currency.FirstOrDefault(i => i.ID == SQ.SaleCurrencyID)
                            select new
                            {
                                SQ.ID,
                                SQ.SeriesID,
                                InvoiceNo = SQ.InvoiceNumber,
                                InvoiceNumber = $"{docType.Code}-{SQ.InvoiceNumber}",
                                SQ.PostingDate,
                                Currency = cur.Description,
                                SQ.SubTotal,
                                SQ.TotalAmount,
                                SQ.TypeDis,
                                SQ.Remarks,
                            }).ToList();
            return Ok(allItems);
        }
        public IActionResult GetServiceDisplay()
        {
            var allItems = (from SQ in _context.ServiceContracts.Where(i => i.Status == "open")
                            join docType in _context.DocumentTypes on SQ.DocTypeID equals docType.ID
                            let cur = _context.Currency.FirstOrDefault(i => i.ID == SQ.SaleCurrencyID)
                            select new
                            {
                                SQ.ID,
                                SQ.SeriesID,
                                InvoiceNo = SQ.InvoiceNumber,
                                InvoiceNumber = $"{docType.Code}-{SQ.InvoiceNumber}",
                                SQ.PostingDate,
                                Currency = cur.Description,
                                SQ.SubTotal,
                                SQ.TotalAmount,
                                SQ.TypeDis,
                                SQ.Remarks,
                            }).ToList();
            return Ok(allItems);
        }
        public async Task<IActionResult>GetLoanPartner()
        {
            var list= await _context.LoanPartners.Where(s=> s.Delete==false).ToListAsync();
            return Ok(list);
        }
    }
}
