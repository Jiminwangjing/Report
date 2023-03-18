using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.ReportSaleAdmin;
using KEDI.Core.Premise.Models.ReportSaleAdmin.PaymentTransaction;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory.PriceList;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.ServicesClass.Report;
using CKBS.Models.Services.POS;

namespace CKBS.Controllers
{
    [Privilege]
    public class SaleReportController : Controller
    {
        private readonly DataContext _context;
        private readonly UtilityModule _fncModule;
        public SaleReportController(DataContext context, UtilityModule format)
        {
            _context = context;
            _fncModule = format;
        }
        public IActionResult Index()
        {
            return View();
        }
        private List<PriceLists> Getpriclist()
        {
            var pricelist = _context.PriceLists.Where(x => x.Delete == false).ToList();
            return pricelist;
        }

        //Sale Quotation
        [Privilege("SA005")]
        public IActionResult SaleQuotationView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Admin";
            ViewBag.Subpage = "Sale Quotation Report";
            ViewBag.Report = "show";
            ViewBag.SaleAdmin = "show";
            ViewBag.SaleQuotationAdmin = "highlight";
            return View();

        }
        public IActionResult SaleARSummaryView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Admin";
            ViewBag.Subpage = "Sale A/R Report";
            ViewBag.Report = "show";
            ViewBag.SaleAdmin = "show";
            ViewBag.SaleARSummary = "highlight";
            ViewBag.PriceLists = new SelectList(Getpriclist(), "ID", "Name");
            return View();

        }

        //Sale Order
        [Privilege("SA006")]
        public IActionResult SaleOrderView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Admin";
            ViewBag.Subpage = "Sale Order Report";
            ViewBag.Report = "show";
            ViewBag.SaleAdmin = "show";
            ViewBag.SaleOrderAdmin = "highlight";
            return View();

        }

        //Sale Delivery
        [Privilege("SA007")]
        public IActionResult SaleDeliveryView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Admin";
            ViewBag.Subpage = "Sale Delivery Report";
            ViewBag.Report = "show";
            ViewBag.SaleAdmin = "show";
            ViewBag.SaleDeliveryAdmin = "highlight";
            return View();

        }
        //Sale AR
        [Privilege("SA001")]
        public IActionResult SaleARView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Admin";
            ViewBag.Subpage = "Sale A/R Report";
            ViewBag.Report = "show";
            ViewBag.SaleAdmin = "show";
            ViewBag.SaleARAdmin = "highlight";
            return View();

        }
        public IActionResult SaleAREditView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Admin";
            ViewBag.Subpage = "Sale A/R Report";
            ViewBag.Report = "show";
            ViewBag.SaleAdmin = "show";
            ViewBag.SaleAEdit = "highlight";
            return View();

        }
        [Privilege("SAR0002")]
        public IActionResult ARReserveEditable()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Admin";
            ViewBag.Subpage = "Sale A/R Report";
            ViewBag.Report = "show";
            ViewBag.SaleAdmin = "show";
            ViewBag.ARReserveEditable = "highlight";
            return View();

        }

        //Sale Credit Memo
        [Privilege("SA002")]
        public IActionResult SaleCreditMemoView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Admin";
            ViewBag.Subpage = "Sale Credit Memo Report";
            ViewBag.Report = "show";
            ViewBag.SaleAdmin = "show";
            ViewBag.SaleCreditMemoAdmin = "highlight";
            return View();
        }

        //Sale Payment Transaction
        [Privilege("SA003")]
        public IActionResult PaymentTransactionView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Admin";
            ViewBag.Subpage = "Payment Transaction Report";
            ViewBag.Report = "show";
            ViewBag.SaleAdmin = "show";
            ViewBag.SalePaymentTransaction = "highlight";
            return View();
        }

        //Customer Statement
        [Privilege("SA004")]
        public IActionResult CustomerStatementView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Admin";
            ViewBag.Subpage = "Customer Statement Report";
            ViewBag.Report = "show";
            ViewBag.SaleAdmin = "show";
            ViewBag.CustomerStatement = "highlight";
            return View();
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

        [HttpGet]
        public IActionResult SummarySaleARReport(string DateFrom, string DateTo, int BranchID, int UserID, int plid)
        {
            List<SaleAR> saleARs = new();
            List<SaleAREdite> saleAREdites = new();
            if (DateFrom == null || DateTo == null)
            {
                return Ok(saleARs);
            }
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                saleAREdites = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
                saleAREdites = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
                saleAREdites = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom == null && DateTo == null && BranchID != 0 && UserID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
                saleAREdites = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom == null && DateTo == null && BranchID == 0 && UserID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.UserID == UserID).ToList();
                saleAREdites = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.UserID == UserID).ToList();
            }

            var saleARSummary = saleARs;
            var saleAREditSummary = saleAREdites;
            if (plid > 0) saleARSummary = saleARSummary.Where(i => i.PriceListID == plid).ToList();
            if (plid > 0) saleAREditSummary = saleAREditSummary.Where(i => i.PriceListID == plid).ToList();

            var saleARDetail = from s in saleARSummary
                               join sd in _context.SaleARDetails on s.SARID equals sd.SARID
                               select new
                               {
                                   TotalDisItem = sd.DisValue * s.ExchangeRate
                               };
            var TotalDisItem = saleARDetail.Sum(s => s.TotalDisItem);
            var TotalDisTotal = saleARSummary.Sum(s => s.DisValue);
            var TotalVat = saleARSummary.Sum(s => s.VatValue * s.ExchangeRate);
            var GrandTotalSys = saleARSummary.Sum(s => s.TotalAmountSys);
            var GrandTotal = saleARSummary.Sum(s => s.TotalAmountSys * s.LocalSetRate);

            var saleAR = (from sale in saleARSummary
                          join user in _context.UserAccounts on sale.UserID equals user.ID
                          join com in _context.Company on user.CompanyID equals com.ID
                          join emp in _context.Employees on user.EmployeeID equals emp.ID
                          join curr_pl in _context.Currency on sale.SaleCurrencyID equals curr_pl.ID
                          join curr in _context.Currency on sale.LocalCurID equals curr.ID
                          join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                          join douType in _context.DocumentTypes on sale.DocTypeID equals douType.ID
                          join b in _context.Branches on sale.BranchID equals b.ID
                          group new { sale, emp, curr_pl, curr_sys, curr, douType, b } by new { sale.BranchID, sale.SARID } into datas
                          let data = datas.FirstOrDefault()
                          let sumByBranch = saleARSummary.Where(_r => _r.BranchID == data.sale.BranchID).Sum(_as => _as.TotalAmountSys)
                          let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                          let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                          select new DevSummarySale
                          {
                              ReceiptID = data.sale.SeriesDID,
                              AmountFreight = _fncModule.ToCurrency(data.sale.FreightAmount, plCur.Amounts),
                              DouType = data.douType.Code,
                              EmpCode = data.emp.Code,
                              EmpName = data.emp.Name,
                              BranchID = data.sale.BranchID,
                              BranchName = data.b.Name,
                              ReceiptNo = data.sale.InvoiceNo,
                              DateOut = data.sale.PostingDate.ToString("dd-MM-yyyy"),
                              DiscountItem = _fncModule.ToCurrency(data.sale.DisValue, plCur.Amounts),
                              Currency = data.curr_pl.Description,
                              GrandTotal = _fncModule.ToCurrency(data.sale.TotalAmount, plCur.Amounts),
                              //Summary
                              DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                              DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                              GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                              SDiscountItem = _fncModule.ToCurrency(TotalDisItem, sysCur.Amounts),
                              SDiscountTotal = _fncModule.ToCurrency(TotalDisTotal, sysCur.Amounts),
                              SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(TotalVat, sysCur.Amounts),
                              SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(GrandTotalSys, sysCur.Amounts),
                              SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(GrandTotal, plCur.Amounts),
                              //
                              TotalDiscountItem = (decimal)_context.SaleARDetails.Where(w => w.SARDID == data.sale.SARID).Sum(s => s.DisValue),
                              DiscountTotal = data.sale.DisValue,
                              Vat = data.sale.VatValue * data.sale.ExchangeRate,
                              GrandTotalSys = data.sale.TotalAmountSys,
                              MGrandTotal = data.sale.TotalAmountSys * data.sale.LocalSetRate,
                          }).ToList();

            var saleAREditDetail = from s in saleAREditSummary
                                   join sd in _context.SaleAREditeDetails on s.SARID equals sd.SARID
                                   select new
                                   {
                                       TotalDisItem = sd.DisValue * s.ExchangeRate
                                   };
            var TotalDisItems = saleAREditDetail.Sum(s => s.TotalDisItem);
            var TotalDisTotals = saleAREditSummary.Sum(s => s.DisValue);
            var TotalVats = saleAREditSummary.Sum(s => s.VatValue * s.ExchangeRate);
            var GrandTotalSyss = saleAREditSummary.Sum(s => s.TotalAmountSys);
            var GrandTotals = saleAREditSummary.Sum(s => s.TotalAmountSys * s.LocalSetRate);

            var saleAREdit = (from sale in saleAREditSummary
                              join user in _context.UserAccounts on sale.UserID equals user.ID
                              join com in _context.Company on user.CompanyID equals com.ID
                              join emp in _context.Employees on user.EmployeeID equals emp.ID
                              join curr_pl in _context.Currency on sale.SaleCurrencyID equals curr_pl.ID
                              join curr in _context.Currency on sale.LocalCurID equals curr.ID
                              join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                              join douType in _context.DocumentTypes on sale.DocTypeID equals douType.ID
                              join b in _context.Branches on sale.BranchID equals b.ID
                              group new { sale, emp, curr_pl, curr_sys, curr, douType, b } by new { sale.BranchID, sale.SARID } into datas
                              let data = datas.FirstOrDefault()
                              let sumByBranch = saleAREditSummary.Where(_r => _r.BranchID == data.sale.BranchID).Sum(_as => _as.TotalAmountSys)
                              let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                              let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                              select new DevSummarySale
                              {
                                  ReceiptID = data.sale.SeriesDID,
                                  AmountFreight = _fncModule.ToCurrency(data.sale.FreightAmount, plCur.Amounts),
                                  DouType = data.douType.Code,
                                  EmpCode = data.emp.Code,
                                  EmpName = data.emp.Name,
                                  BranchID = data.sale.BranchID,
                                  BranchName = data.b.Name,
                                  ReceiptNo = data.sale.InvoiceNo,
                                  DateOut = data.sale.PostingDate.ToString("dd-MM-yyyy"),
                                  DiscountItem = _fncModule.ToCurrency(data.sale.DisValue, plCur.Amounts),
                                  Currency = data.curr_pl.Description,
                                  GrandTotal = _fncModule.ToCurrency(data.sale.TotalAmount, plCur.Amounts),
                                  //Summary
                                  DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                  DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                  GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                                  SDiscountItem = _fncModule.ToCurrency(TotalDisItems, sysCur.Amounts),
                                  SDiscountTotal = _fncModule.ToCurrency(TotalDisTotals, sysCur.Amounts),
                                  SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(TotalVats, sysCur.Amounts),
                                  SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(GrandTotalSyss, sysCur.Amounts),
                                  SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(GrandTotals, plCur.Amounts),
                                  //
                                  TotalDiscountItem = (decimal)_context.SaleAREditeDetails.Where(w => w.SARDID == data.sale.SARID).Sum(s => s.DisValue),
                                  DiscountTotal = data.sale.DisValue,
                                  Vat = data.sale.VatValue * data.sale.ExchangeRate,
                                  GrandTotalSys = data.sale.TotalAmountSys,
                                  MGrandTotal = data.sale.TotalAmountSys * data.sale.LocalSetRate,
                              }).ToList();

            var allSummarySale = new List<DevSummarySale>
                (saleAR.Count + saleAREdit.Count);
            allSummarySale.AddRange(saleAR);
            allSummarySale.AddRange(saleAREdit);

            var allSale = (from all in allSummarySale
                           join b in _context.Branches on all.BranchID equals b.ID
                           join c in _context.Company on b.CompanyID equals c.ID
                           join curr_sys in _context.Currency on c.SystemCurrencyID equals curr_sys.ID
                           join curr in _context.Currency on c.LocalCurrencyID equals curr.ID
                           group new { all, b, curr_sys, curr } by new { all.BranchID, all.ReceiptID } into r
                           let data = r.FirstOrDefault()
                           let sumByBranch = allSummarySale.Where(_r => _r.BranchID == data.all.BranchID).Sum(_as => _as.GrandTotalSys)
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr.ID) ?? new Display()
                           select new
                           {
                               data.all.DouType,
                               data.all.EmpCode,
                               data.all.EmpName,
                               data.all.BranchID,
                               BranchName = data.b.Name,
                               data.all.ReceiptNo,
                               data.all.DateOut,
                               data.all.DiscountItem,
                               data.all.Currency,
                               data.all.GrandTotal,
                               data.all.AmountFreight,
                               //Summary
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               //SCount = ChCount.ToString(),
                               GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                               SDiscountItem = _fncModule.ToCurrency(allSummarySale.Sum(s => s.TotalDiscountItem), sysCur.Amounts),
                               SDiscountTotal = _fncModule.ToCurrency(r.Sum(s => s.all.DiscountTotal), sysCur.Amounts),
                               SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(allSummarySale.Sum(v => v.Vat), sysCur.Amounts),
                               SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(allSummarySale.Sum(_r => _r.GrandTotalSys), sysCur.Amounts),
                               SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(allSummarySale.Sum(_r => _r.MGrandTotal), plCur.Amounts),
                           }).ToList();
            return Ok(allSale.OrderBy(o => o.DateOut));
        }
        //Filter Data
        [HttpGet]
        public IActionResult GetSaleQuotation(string DateFrom, string DateTo, int BranchID, int UserID, int CusID)
        {
            List<SaleQuote> saleQuotes = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID == 0)
            {
                saleQuotes = _context.SaleQuotes.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID == 0)
            {
                saleQuotes = _context.SaleQuotes.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID == 0)
            {
                saleQuotes = _context.SaleQuotes.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleQuotes = _context.SaleQuotes.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID != 0)
            {
                saleQuotes = _context.SaleQuotes.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID != 0)
            {
                saleQuotes = _context.SaleQuotes.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CusID == CusID).ToList();
            }
            else
            {
                return Ok(new List<SaleQuote>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var summary = GetSummarySaleTotal(DateFrom, DateTo, BranchID, UserID, CusID, "SQ");
            if (summary != null)
            {
                SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
                var FiltersaleQuotes = saleQuotes;
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var list = from SQ in FiltersaleQuotes
                           join SQD in _context.SaleQuoteDetails on SQ.SQID equals SQD.SQID
                           join BP in _context.BusinessPartners on SQ.CusID equals BP.ID
                           join I in _context.ItemMasterDatas on SQD.ItemID equals I.ID
                           join lc in _context.Currency on SQ.LocalCurID equals lc.ID
                           join plc in _context.Currency on SQ.SaleCurrencyID equals plc.ID
                           group new { SQ, SQD, BP, I, syCurrency, lc, plc } by new { SQ.SQID, SQD.SQDID } into g
                           let data = g.FirstOrDefault()
                           let master = data.SQ
                           let detail = data.SQD
                           let lc = data.lc
                           let plc = data.plc
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == syCurrency.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.plc.ID) ?? new Display()
                           select new
                           {
                               //Master
                               InvoiceNo = master.InvoiceNumber,
                               CusName = g.First().BP.Name,
                               PostingDate = Convert.ToDateTime(master.PostingDate).ToString("MM-dd-yyy"),
                               Discount = _fncModule.ToCurrency(master.DisValue, plCur.Amounts),
                               Sub_Total = _fncModule.ToCurrency(master.SubTotal, plCur.Amounts),
                               VatValue = _fncModule.ToCurrency(master.VatValue, plCur.Amounts),
                               TotalAmount = _fncModule.ToCurrency(master.TotalAmount, plCur.Amounts),
                               PLC = plc.Description,
                               //Detail
                               detail.ItemCode,
                               ItemName = detail.ItemNameKH,
                               detail.Qty,
                               Uom = detail.UomName,
                               detail.UnitPrice,
                               DisItem = _fncModule.ToCurrency(detail.DisValue, plCur.Amounts),
                               TotalItem = _fncModule.ToCurrency(detail.Total, plCur.Amounts),
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               //Summary
                               SumCount = summary.FirstOrDefault().CountInvoice,
                               SumSoldAmount = _fncModule.ToCurrency(summary.FirstOrDefault().SoldAmount, sysCur.Amounts),
                               SumDisItem = _fncModule.ToCurrency(summary.FirstOrDefault().DisCountItem, sysCur.Amounts),
                               SumDisTotal = _fncModule.ToCurrency(summary.FirstOrDefault().DisCountTotal, sysCur.Amounts),
                               SumVat = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalVatRate, sysCur.Amounts),
                               SumGrandTotal = lc.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().Total, lcCur.Amounts),
                               SumGrandTotalSys = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalSys, sysCur.Amounts),
                           };
                return Ok(list);
            }
            else
            {
                return Ok(new List<SaleQuote>());
            }
        }

        [HttpGet]
        public IActionResult GetSaleOrder(string DateFrom, string DateTo, int BranchID, int UserID, int CusID)
        {
            List<SaleOrder> saleOrders = new();

            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID == 0)
            {
                saleOrders = _context.SaleOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID == 0)
            {
                saleOrders = _context.SaleOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID == 0)
            {
                saleOrders = _context.SaleOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID != 0)
            {
                saleOrders = _context.SaleOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleOrders = _context.SaleOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleOrders = _context.SaleOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID != 0)
            {
                saleOrders = _context.SaleOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CusID == CusID).ToList();
            }
            else
            {
                return Ok(new List<SaleOrder>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var summary = GetSummarySaleTotal(DateFrom, DateTo, BranchID, UserID, CusID, "SO");
            if (summary != null)
            {
                SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var FilterSaleOrder = saleOrders;
                var list = from SO in FilterSaleOrder
                           join SOD in _context.SaleOrderDetails on SO.SOID equals SOD.SOID
                           join BP in _context.BusinessPartners on SO.CusID equals BP.ID
                           join I in _context.ItemMasterDatas on SOD.ItemID equals I.ID
                           join CUR in _context.Currency on SO.SaleCurrencyID equals CUR.ID
                           join lc in _context.Currency on SO.LocalCurID equals lc.ID
                           group new { SO, SOD, BP, I, CUR, lc } by new { SO.SOID, SOD.SODID } into g
                           let data = g.FirstOrDefault()
                           let master = data.SO
                           let detail = data.SOD
                           let cu = data.CUR
                           let lc = data.lc
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == syCurrency.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                           select new
                           {
                               //Master
                               InvoiceNo = master.InvoiceNumber,
                               CusName = g.First().BP.Name,
                               PostingDate = Convert.ToDateTime(master.PostingDate).ToString("MM-dd-yyy"),
                               Discount = _fncModule.ToCurrency(master.DisValue, plCur.Amounts),
                               Sub_Total = _fncModule.ToCurrency(master.SubTotal, plCur.Amounts),
                               VatValue = _fncModule.ToCurrency(master.VatValue, plCur.Amounts),
                               TotalAmount = _fncModule.ToCurrency(master.TotalAmount, plCur.Amounts),
                               PLC = cu.Description,
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               //Detail
                               detail.ItemCode,
                               ItemName = detail.ItemNameKH,
                               detail.Qty,
                               Uom = detail.UomName,
                               detail.UnitPrice,
                               DisItem = _fncModule.ToCurrency(detail.DisValue, plCur.Amounts),
                               TotalItem = _fncModule.ToCurrency(detail.Total, plCur.Amounts),
                               //Summary
                               SumCount = summary.FirstOrDefault().CountInvoice,
                               SumSoldAmount = _fncModule.ToCurrency(summary.FirstOrDefault().SoldAmount, sysCur.Amounts),
                               SumDisItem = _fncModule.ToCurrency(summary.FirstOrDefault().DisCountItem, sysCur.Amounts),
                               SumDisTotal = _fncModule.ToCurrency(summary.FirstOrDefault().DisCountTotal, sysCur.Amounts),
                               SumVat = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalVatRate, sysCur.Amounts),
                               SumGrandTotal = lc.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().Total, lcCur.Amounts),
                               SumGrandTotalSys = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalSys, sysCur.Amounts),

                           };
                return Ok(list);
            }
            else
            {
                return Ok(new List<SaleOrder>());
            }
        }

        [HttpGet]
        public IActionResult GetSaleDelivery(string DateFrom, string DateTo, int BranchID, int UserID, int CusID)
        {
            List<SaleDelivery> saleDeliveries = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID == 0)
            {
                saleDeliveries = _context.SaleDeliveries.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID == 0)
            {
                saleDeliveries = _context.SaleDeliveries.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID == 0)
            {
                saleDeliveries = _context.SaleDeliveries.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID != 0)
            {
                saleDeliveries = _context.SaleDeliveries.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleDeliveries = _context.SaleDeliveries.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleDeliveries = _context.SaleDeliveries.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID != 0)
            {
                saleDeliveries = _context.SaleDeliveries.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CusID == CusID).ToList();
            }
            else
            {
                return Ok(new List<SaleDelivery>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var summary = GetSummarySaleTotal(DateFrom, DateTo, BranchID, UserID, CusID, "SD");
            if (summary != null)
            {
                SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var FilterSaleDelivery = saleDeliveries;
                var list = from SD in FilterSaleDelivery
                           join SDD in _context.SaleDeliveryDetails on SD.SDID equals SDD.SDID
                           join BP in _context.BusinessPartners on SD.CusID equals BP.ID
                           join I in _context.ItemMasterDatas on SDD.ItemID equals I.ID
                           join CUR in _context.Currency on SD.SaleCurrencyID equals CUR.ID
                           join lc in _context.Currency on SD.LocalCurID equals lc.ID
                           group new { SD, SDD, BP, I, CUR, lc } by new { SD.SDID, SDD.SDDID } into g
                           let data = g.FirstOrDefault()
                           let master = data.SD
                           let detail = data.SDD
                           let cu = data.CUR
                           let lc = data.lc
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == syCurrency.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                           select new
                           {
                               //Master
                               InvoiceNo = master.InvoiceNumber,
                               CusName = g.First().BP.Name,
                               PostingDate = Convert.ToDateTime(master.PostingDate).ToString("MM-dd-yyy"),
                               Discount = _fncModule.ToCurrency(master.DisValue, plCur.Amounts),
                               Sub_Total = _fncModule.ToCurrency(master.SubTotal, plCur.Amounts),
                               VatValue = _fncModule.ToCurrency(master.VatValue, plCur.Amounts),
                               TotalAmount = _fncModule.ToCurrency(master.TotalAmount, plCur.Amounts),
                               PLC = cu.Description,

                               //Detail
                               detail.ItemCode,
                               ItemName = detail.ItemNameKH,
                               detail.Qty,
                               Uom = detail.UomName,
                               detail.UnitPrice,
                               DisItem = _fncModule.ToCurrency(detail.DisValue, plCur.Amounts),
                               TotalItem = _fncModule.ToCurrency(detail.Total, plCur.Amounts),
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               //Summary
                               SumCount = summary.FirstOrDefault().CountInvoice,
                               SumSoldAmount = _fncModule.ToCurrency(summary.FirstOrDefault().SoldAmount, sysCur.Amounts),
                               SumDisItem = _fncModule.ToCurrency(summary.FirstOrDefault().DisCountItem, sysCur.Amounts),
                               SumDisTotal = _fncModule.ToCurrency(summary.FirstOrDefault().DisCountTotal, sysCur.Amounts),
                               SumVat = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalVatRate, sysCur.Amounts),
                               SumGrandTotal = lc.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().Total, lcCur.Amounts),
                               SumGrandTotalSys = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalSys, sysCur.Amounts),

                           };
                return Ok(list);
            }
            else
            {
                return Ok(new List<SaleDelivery>());
            }
        }

        [HttpGet]
        public IActionResult GetSaleAREdit(string DateFrom, string DateTo, int BranchID, int UserID, int CusID)
        {
            List<SaleAREditeHistory> saleARs = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID == 0)
            {
                saleARs = _context.SaleAREditeHistory.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID == 0)
            {
                saleARs = _context.SaleAREditeHistory.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID == 0)
            {
                saleARs = _context.SaleAREditeHistory.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID != 0)
            {
                saleARs = _context.SaleAREditeHistory.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleARs = _context.SaleAREditeHistory.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleARs = _context.SaleAREditeHistory.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID != 0)
            {
                saleARs = _context.SaleAREditeHistory.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CusID == CusID).ToList();
            }
            else
            {
                return Ok(new List<SaleAR>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var saleARDetail = from s in saleARs
                               join sd in _context.SaleAREditeDetailHistory.Where(x => x.Delete == false) on s.ID equals sd.SAREID
                               group s by new { s.ID } into g
                               let _g = g.Where(s => s.SARID == g.Max(s => s.SARID)).LastOrDefault()
                               select new
                               {
                                   ID = _g.SARID,
                                   SAREID = _g.ID,
                                   SARID = _g.SARID,
                                   TotalDisItem = _g.DisValue * _g.ExchangeRate,
                                   TotalDisTotal = _g.DisValue,
                                   TotalVat = _g.DisValue,
                                   GrandTotalSys = _g.TotalAmountSys,
                                   GrandTotal = _g.TotalAmountSys * _g.LocalSetRate,
                                   TotalAmount = _g.TotalAmount,
                                   DisValue = _g.DisValue,
                                   //Sub_Total = _g.SubTotal,
                                   Sub_Total = _context.SaleAREditeDetailHistory.Where(x => x.SAREID == _g.ID && x.Delete == false).ToList().Sum(X => X.TotalSys),
                                   VatValue = _g.VatValue,
                                   Applied_Amount = _g.AppliedAmount,
                                   UserID = _g.UserID,
                                   CusID = _g.CusID,
                                   SaleCurrencyID = _g.SaleCurrencyID,
                                   LocalCurID = _g.LocalCurID,
                                   InvoiceNumber = _g.InvoiceNumber,
                                   PostingDate = _g.PostingDate
                               };


            var TotalDisItem = saleARDetail.Sum(s => s.TotalDisItem);
            var TotalDisTotal = saleARDetail.Sum(s => s.TotalDisTotal);
            var TotalVat = saleARDetail.Sum(s => s.TotalVat);
            var GrandTotalSys = saleARDetail.Sum(s => s.GrandTotalSys);
            var GrandTotal = saleARDetail.Sum(s => s.GrandTotal);

            //var FilterSaleAR = saleARDetail;
            SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
            var list = from SAR in saleARs
                       join SARD in _context.SaleAREditeDetailHistory.Where(x => !x.Delete) on SAR.ID equals SARD.SAREID
                       join user in _context.UserAccounts on SAR.UserID equals user.ID
                       join com in _context.Company on user.CompanyID equals com.ID
                       join BP in _context.BusinessPartners on SAR.CusID equals BP.ID
                       join I in _context.ItemMasterDatas on SARD.ItemID equals I.ID
                       join curr_pl in _context.Currency on SAR.SaleCurrencyID equals curr_pl.ID
                       join curr in _context.Currency on SAR.LocalCurID equals curr.ID
                       join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                       group new { SAR, SARD, BP, I, com, user, curr_pl, curr, curr_sys } by new { SARD.ID } into g
                       let data = g.FirstOrDefault()
                       let master = data.SAR
                       let detail = data.SARD
                       let cu = data.curr_pl
                       let lc = data.curr
                       let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                       let curr = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr.ID) ?? new Display()
                       let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()

                       select new
                       {
                           //Master
                           ID = detail.SAREID,
                           InvoiceNo = master.InvoiceNumber,
                           CusName = g.First().BP.Name,
                           PostingDate = Convert.ToDateTime(master.PostingDate).ToString("MM-dd-yyy"),
                           Discount = _fncModule.ToCurrency(master.DisValue, plCur.Amounts),
                           Sub_Total = _fncModule.ToCurrency(master.SubTotal, plCur.Amounts),
                           VatValue = _fncModule.ToCurrency(master.VatValue, plCur.Amounts),
                           Applied_Amount = _fncModule.ToCurrency(master.AppliedAmount, plCur.Amounts),
                           TotalAmount = _fncModule.ToCurrency(master.TotalAmount, plCur.Amounts),
                           PLC = cu.Description,
                           //Detail
                           detail.ItemID,
                           ItemCode = detail.ItemCode,
                           ItemName = detail.ItemNameKH,
                           detail.Qty,
                           Uom = detail.UomName,
                           detail.UnitPrice,
                           DisItem = _fncModule.ToCurrency(detail.DisValue, plCur.Amounts),
                           TotalItem = _fncModule.ToCurrency(detail.Total, plCur.Amounts),
                           DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                           DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                           //Summary
                           SumCount = saleARDetail.Count(),
                           //SumSoldAmount = _fncModule.ToCurrency(summary.FirstOrDefault().SoldAmount, sysCur.Amounts),
                           SumDisItem = _fncModule.ToCurrency(TotalDisItem, sysCur.Amounts),
                           SumDisTotal = _fncModule.ToCurrency(TotalDisTotal, sysCur.Amounts),
                           SumVat = cu.Description + " " + _fncModule.ToCurrency(TotalVat, sysCur.Amounts),
                           SumGrandTotal = lc.Description + " " + _fncModule.ToCurrency(GrandTotal, curr.Amounts),
                           SumGrandTotalSys = cu.Description + " " + _fncModule.ToCurrency(GrandTotalSys, sysCur.Amounts),
                       };
            return Ok(list);
        }

        [HttpGet]
        public IActionResult ARReserveEditableView(string DateFrom, String DateTo, int BranchID, int UserID, int CusID)
        {
            List<ARReserveEditableHistory> SaleARR = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID == 0)
            {
                SaleARR = _context.ARReserveEditableHistories.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID == 0)
            {
                SaleARR = _context.ARReserveEditableHistories.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID == 0)
            {
                SaleARR = _context.ARReserveEditableHistories.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                SaleARR = _context.ARReserveEditableHistories.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID != 0)
            {
                SaleARR = _context.ARReserveEditableHistories.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID != 0)
            {
                SaleARR = _context.ARReserveEditableHistories.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CusID == CusID).ToList();
            }
            else
            {
                return Ok(new List<SaleQuote>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var saleARDetail = from s in SaleARR
                               join sd in _context.ARReserveEditableDetailHistories.Where(x => x.Delete == false) on s.ID equals sd.ARREDTHID
                               group s by new { s.ID } into g
                               let _g = g.FirstOrDefault()
                               // let _g = g.Where(s => s.ARReserveEditableID == g.Max(s => s.ARReserveEditableID)).LastOrDefault()
                               select new
                               {
                                   ID = _g.ARReserveEditableID,
                                   ARREDTHID = _g.ID,
                                   //ARReserveEditableID = _g.ID,
                                   TotalDisItem = _g.DisValue * _g.ExchangeRate,
                                   TotalDisTotal = _g.DisValue,
                                   TotalVat = _g.DisValue,
                                   GrandTotalSys = _g.TotalAmountSys,
                                   GrandTotal = _g.TotalAmountSys * _g.LocalSetRate,
                                   TotalAmount = _g.TotalAmount,
                                   DisValue = _g.DisValue,
                                   //Sub_Total = _g.SubTotal,
                                   Sub_Total = _context.ARReserveEditableDetailHistories.Where(x => x.ARREDTHID == _g.ID && x.Delete == false).ToList().Sum(X => X.TotalSys),
                                   VatValue = _g.VatValue,
                                   Applied_Amount = _g.AppliedAmount,
                                   UserID = _g.UserID,
                                   CusID = _g.CusID,
                                   SaleCurrencyID = _g.SaleCurrencyID,
                                   LocalCurID = _g.LocalCurID,
                                   InvoiceNumber = _g.InvoiceNumber,
                                   PostingDate = _g.PostingDate
                               };
            var TotalDisItem = saleARDetail.Sum(s => s.TotalDisItem);
            var TotalDisTotal = saleARDetail.Sum(s => s.TotalDisTotal);
            var TotalVat = saleARDetail.Sum(s => s.TotalVat);
            var GrandTotalSys = saleARDetail.Sum(s => s.GrandTotalSys);
            var GrandTotal = saleARDetail.Sum(s => s.GrandTotal);
            SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
            var list = from ARR in SaleARR
                       join ARRD in _context.ARReserveEditableDetailHistories.Where(x => !x.Delete) on ARR.ID equals ARRD.ARREDTHID
                       join user in _context.UserAccounts on ARR.UserID equals user.ID
                       join com in _context.Company on user.CompanyID equals com.ID
                       join BP in _context.BusinessPartners on ARR.CusID equals BP.ID
                       join I in _context.ItemMasterDatas on ARRD.ItemID equals I.ID
                       join curr_pl in _context.Currency on ARR.SaleCurrencyID equals curr_pl.ID
                       join curr in _context.Currency on ARR.LocalCurID equals curr.ID
                       join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                       group new { ARR, ARRD, BP, I, com, user, curr_pl, curr, curr_sys } by new { ARRD.ID } into g
                       let data = g.FirstOrDefault()
                       let master = data.ARR
                       let detail = data.ARRD
                       let cu = data.curr_pl
                       let lc = data.curr
                       let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                       let curr = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr.ID) ?? new Display()
                       let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()

                       select new
                       {
                           //Master
                           ID = detail.ARREDTHID,
                           InvoiceNo = master.InvoiceNumber,
                           CusName = g.First().BP.Name,
                           PostingDate = Convert.ToDateTime(master.PostingDate).ToString("MM-dd-yyy"),
                           Discount = _fncModule.ToCurrency(master.DisValue, plCur.Amounts),
                           Sub_Total = _fncModule.ToCurrency(master.SubTotal, plCur.Amounts),
                           VatValue = _fncModule.ToCurrency(master.VatValue, plCur.Amounts),
                           Applied_Amount = _fncModule.ToCurrency(master.AppliedAmount, plCur.Amounts),
                           TotalAmount = _fncModule.ToCurrency(master.TotalAmount, plCur.Amounts),
                           PLC = cu.Description,
                           //Detail
                           detail.ItemID,
                           ItemCode = detail.ItemCode,
                           ItemName = detail.ItemNameKH,
                           detail.Qty,
                           Uom = detail.UomName,
                           detail.UnitPrice,
                           DisItem = _fncModule.ToCurrency(detail.DisValue, plCur.Amounts),
                           TotalItem = _fncModule.ToCurrency(detail.Total, plCur.Amounts),
                           DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                           DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                           //Summary
                           SumCount = saleARDetail.Count(),
                           //SumSoldAmount = _fncModule.ToCurrency(summary.FirstOrDefault().SoldAmount, sysCur.Amounts),
                           SumDisItem = _fncModule.ToCurrency(TotalDisItem, sysCur.Amounts),
                           SumDisTotal = _fncModule.ToCurrency(TotalDisTotal, sysCur.Amounts),
                           SumVat = cu.Description + " " + _fncModule.ToCurrency(TotalVat, sysCur.Amounts),
                           SumGrandTotal = lc.Description + " " + _fncModule.ToCurrency(GrandTotal, curr.Amounts),
                           SumGrandTotalSys = cu.Description + " " + _fncModule.ToCurrency(GrandTotalSys, sysCur.Amounts),
                       };
            return Ok(list);
        }
        [HttpGet]
        public IActionResult GetSaleAR(string DateFrom, string DateTo, int BranchID, int UserID, int CusID)
        {
            List<SaleAR> saleARs = new();

            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CusID == CusID).ToList();
            }
            else
            {
                return Ok(new List<SaleAR>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var summary = GetSummarySaleTotal(DateFrom, DateTo, BranchID, UserID, CusID, "SAR");
            if (summary != null)
            {
                SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var FilterSaleAR = saleARs;
                var list = from SAR in FilterSaleAR
                           join SARD in _context.SaleARDetails on SAR.SARID equals SARD.SARID
                           join BP in _context.BusinessPartners on SAR.CusID equals BP.ID
                           join I in _context.ItemMasterDatas on SARD.ItemID equals I.ID
                           join CUR in _context.Currency on SAR.SaleCurrencyID equals CUR.ID
                           join lc in _context.Currency on SAR.LocalCurID equals lc.ID
                           group new { SAR, SARD, BP, I, CUR, lc } by new { SARD.SARID, SARD.UnitPrice, SARD.ItemID } into g
                           let data = g.FirstOrDefault()
                           let master = data.SAR
                           let detail = data.SARD
                           let cu = data.CUR
                           let lc = data.lc
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == syCurrency.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                           let totalQtyByCost = _context.SaleARDetails.Where(x => x.SARID == master.SARID && x.ItemID == detail.ItemID)
                         .Sum(x => x.Qty)
                           select new
                           {
                               //Master
                               master.InvoiceNo,
                               CusName = g.FirstOrDefault().BP.Name,
                               PostingDate = Convert.ToDateTime(master.PostingDate).ToString("MM-dd-yyy"),
                               Discount = _fncModule.ToCurrency(master.DisValue, plCur.Amounts),
                               Sub_Total = _fncModule.ToCurrency(master.SubTotal, plCur.Amounts),
                               VatValue = _fncModule.ToCurrency(master.VatValue, plCur.Amounts),
                               Applied_Amount = _fncModule.ToCurrency(master.AppliedAmount, plCur.Amounts),
                               TotalAmount = _fncModule.ToCurrency(master.TotalAmount, plCur.Amounts),
                               PLC = cu.Description,
                               //Detail
                               detail.ItemCode,
                               ItemName = detail.ItemNameKH,
                               //detail.Qty,
                               Qty = totalQtyByCost,
                               Uom = detail.UomName,
                               detail.UnitPrice,

                               //DisItem = _fncModule.ToCurrency(detail.DisValue, plCur.Amounts),
                               DisItem = _fncModule.ToCurrency(_context.SaleARDetails.Where(x => x.SARID == g.FirstOrDefault().SAR.SARID).Sum(x => x.DisValue), plCur.Amounts),
                               TotalItem = _fncModule.ToCurrency(detail.Total, plCur.Amounts),
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               //Summary
                               SumCount = summary.FirstOrDefault().CountInvoice,
                               SumSoldAmount = _fncModule.ToCurrency(summary.FirstOrDefault().SoldAmount, sysCur.Amounts),
                               SumDisItem = _fncModule.ToCurrency(summary.FirstOrDefault().DisCountItem, sysCur.Amounts),
                               SumDisTotal = _fncModule.ToCurrency(summary.FirstOrDefault().DisCountTotal, sysCur.Amounts),
                               SumVat = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalVatRate, sysCur.Amounts),
                               SumGrandTotal = lc.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().Total, lcCur.Amounts),
                               SumGrandTotalSys = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalSys, sysCur.Amounts),
                           };
                return Ok(list);

            }
            else
            {
                return Ok(new List<SaleAR>());
            }
        }
        //=================GetSaleServiceContract==========================
        public IActionResult ServiceContractView()
        {
            ViewBag.ServiceContractViewAdmin = "highlight";
            return View();
        }
        public IActionResult GetExpriedDate()
        {
            var data = (from ser in _context.ServiceContracts
                        select new ContractBiling
                        {
                            ID = ser.ID,
                            NumExpiresOfDay = DateTime.Now.Subtract(ser.ContractENDate).Days.ToString(),
                        }).ToList();
            return Ok(data);

        }
        [HttpGet]
        public IActionResult GetApliedAmount()
        {
            var data = _context.ServiceContracts.ToList();
            return Ok(data);
        }

        [HttpGet]

        public IActionResult GetServicContract(string DateFrom, string DateTo, string ContractType, string PaymentStatus, string Expried)
        {
            List<ServiceContract> serviceContracts = new();

            if (DateFrom != null && DateTo != null && ContractType == "" && PaymentStatus == "" && Expried == "")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "All" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }

            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "UnPaid" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.AppliedAmount == 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "FullyPaid" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.AppliedAmount == w.TotalAmount).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "NotYetFullyApplied" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.AppliedAmount > 0 && w.AppliedAmount < w.TotalAmount).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "Outstanding" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.AppliedAmount == 0 || w.AppliedAmount < w.TotalAmount).ToList();
            }

            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "UnPaid" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.AppliedAmount == 0 && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "FullyPaid" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.AppliedAmount == w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "NotYetFullyApplied" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.AppliedAmount > 0 && w.AppliedAmount < w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "Outstanding" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && (w.AppliedAmount == 0 || w.AppliedAmount < w.TotalAmount) && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "All" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }



            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "UnPaid" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.AppliedAmount == 0 && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "FullyPaid" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.AppliedAmount == w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "NotYetFullyApplied" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.AppliedAmount > 0 && w.AppliedAmount < w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "Outstanding" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && (w.AppliedAmount == 0 || w.AppliedAmount < w.TotalAmount) && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "All" && PaymentStatus == "All" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }

            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "UnPaid" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "FullyPaid" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == w.TotalAmount).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "NotYetFullyApplied" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount > 0 && w.AppliedAmount < w.TotalAmount).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "Outstanding" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && (w.AppliedAmount == 0 || w.AppliedAmount < w.TotalAmount)).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "All" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType).ToList();
            }

            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "UnPaid" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == 0 && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "FullyPaid" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "NotYetFullyApplied" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount > 0 && w.AppliedAmount < w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "Outstanding" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && (w.AppliedAmount == 0 || w.AppliedAmount < w.TotalAmount) && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "All" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }

            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "UnPaid" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == 0 && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "FullyPaid" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "NotYetFullyApplied" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount > 0 && w.AppliedAmount < w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "Outstanding" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && (w.AppliedAmount == 0 || w.AppliedAmount < w.TotalAmount) && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "New" && PaymentStatus == "All" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }


            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "UnPaid" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "FullyPaid" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == w.TotalAmount).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "NotYetFullyApplied" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount > 0 && w.AppliedAmount < w.TotalAmount).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "Outstanding" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && (w.AppliedAmount == 0 || w.AppliedAmount < w.TotalAmount)).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "All" && Expried == "All")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType).ToList();
            }

            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "UnPaid" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == 0 && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "FullyPaid" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "NotYetFullyApplied" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount > 0 && w.AppliedAmount < w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "Outstanding" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && (w.AppliedAmount == 0 || w.AppliedAmount < w.TotalAmount) && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "All" && Expried == "Yes")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) < 0).ToList();
            }

            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "UnPaid" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == 0 && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "FullyPaid" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount == w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "NotYetFullyApplied" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && w.AppliedAmount > 0 && w.AppliedAmount < w.TotalAmount && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "Outstanding" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && (w.AppliedAmount == 0 || w.AppliedAmount < w.TotalAmount) && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }
            else if (DateFrom != null && DateTo != null && ContractType == "Renewal" && PaymentStatus == "All" && Expried == "No")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.ContractType == ContractType && Convert.ToInt32(w.ContractENDate.Subtract(DateTime.Now).Days) >= 0).ToList();
            }


            else if (DateFrom != null && DateTo != null && ContractType == "")
            {
                serviceContracts = _context.ServiceContracts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else
            {
                return Ok(new List<ServiceContract>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            //var summary = GetSummaryTotalContractType(DateFrom, DateTo, ContractType);
            //var summary = GetSummarySaleTotal(DateFrom, DateTo, BranchID, UserID, CusID, "SAR");
            if (serviceContracts != null)
            {
                SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var FilterSaleAR = serviceContracts;
                var list = from SAR in FilterSaleAR
                           join SARD in _context.ServiceContractDetails on SAR.ID equals SARD.ServiceContractID
                           join BP in _context.BusinessPartners on SAR.CusID equals BP.ID
                           join I in _context.ItemMasterDatas on SARD.ItemID equals I.ID
                           join CUR in _context.Currency on SAR.SaleCurrencyID equals CUR.ID
                           join lc in _context.Currency on SAR.LocalCurID equals lc.ID
                           let contr = _context.Contracts.FirstOrDefault(x => x.ID == SAR.ContractTemplateID) ?? new KEDI.Core.Premise.Models.Services.ServiceContractTemplate.ContractTemplate()
                           let contype = _context.SetupContractTypes.FirstOrDefault(x => x.ID == SAR.ContractTemplateID) ?? new KEDI.Core.Premise.Models.Services.ServiceContractTemplate.SetupContractType()
                           group new { SAR, SARD, BP, I, CUR, lc, contr, contype } by new { SAR.ID } into g
                           let data = g.FirstOrDefault()
                           let master = data.SAR
                           let detail = data.SARD
                           let cu = data.CUR
                           let lc = data.lc
                           let bp = data.BP
                           let contr = data.contr
                           let contype = data.contype
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == syCurrency.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                           select new ContractBiling
                           {
                               CusCode = bp.Code,
                               CusName = bp.Name,
                               DocumentNo = master.InvoiceNumber,
                               ContractStartDate = Convert.ToDateTime(master.ContractStartDate).ToString("MM-dd-yyy"),
                               ContractEndDate = Convert.ToDateTime(master.ContractENDate).ToString("MM-dd-yyy"),
                               NumExpiresOfDay = master.ContractENDate.Subtract(DateTime.Now).Days.ToString() + " Days",
                               ContractRenewalDate = Convert.ToDateTime(master.ContractENDate).ToString("MM-dd-yyy"),
                               ContractType = master.ContractType,
                               ContractNameTemplate = contr.Name,
                               SubContractTypeTemplate = contype.ContractType,
                               Remark = master.Remark,
                               Amount = $"{cu.Description} {_fncModule.ToCurrency(master.TotalAmount, plCur.Amounts)}",


                               //TotalBalance = all.Currencysys + " " + allsale.Where(i => i.CustomerCode == all.CustomerCode).Sum(i => i.SBalanceDue).ToString("0,0.000")
                               SumGrandTotal = $"{cu.Description} {_context.ServiceContracts.Where(sc => sc.CusID == bp.ID).Sum(sc => sc.TotalAmount)}",
                               //SumGrandTotalSys = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalSys, sysCur.Amounts),

                           };
                return Ok(list);
            }
            else
            {
                return Ok(new List<ServiceContract>());
            }

        }


        //=================endSaleServiceContract=========================
        [HttpGet]
        public IActionResult GetSaleCreditMemo(string DateFrom, string DateTo, int BranchID, int UserID, int CusID)
        {
            List<SaleCreditMemo> saleCreditMemos = new();

            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID == 0)
            {
                saleCreditMemos = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID == 0)
            {
                saleCreditMemos = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID == 0)
            {
                saleCreditMemos = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID != 0)
            {
                saleCreditMemos = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleCreditMemos = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                saleCreditMemos = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID != 0)
            {
                saleCreditMemos = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CusID == CusID).ToList();
            }
            else
            {
                return Ok(new List<SaleCreditMemo>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var summary = GetSummarySaleTotal(DateFrom, DateTo, BranchID, UserID, CusID, "SC");
            if (summary != null)
            {
                SystemCurrency syCurrency = GetSystemCurrencies().FirstOrDefault();
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var FilterSaleCreditMemo = saleCreditMemos;
                var list = from SC in FilterSaleCreditMemo
                           join SCD in _context.SaleCreditMemoDetails on SC.SCMOID equals SCD.SCMOID
                           join BP in _context.BusinessPartners on SC.CusID equals BP.ID
                           join I in _context.ItemMasterDatas on SCD.ItemID equals I.ID
                           join CUR in _context.Currency on SC.SaleCurrencyID equals CUR.ID
                           join lc in _context.Currency on SC.LocalCurID equals lc.ID
                           group new { SC, SCD, BP, I, CUR, lc } by new { SC.SCMOID, SCD.SCMODID } into g
                           let data = g.FirstOrDefault()
                           let master = data.SC
                           let detail = data.SCD
                           let cu = data.CUR
                           let lc = data.lc
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == syCurrency.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                           select new
                           {
                               //Master
                               InvoiceNo = master.InvoiceNumber,
                               CusName = g.First().BP.Name,
                               PostingDate = Convert.ToDateTime(master.PostingDate).ToString("MM-dd-yyy"),
                               Discount = _fncModule.ToCurrency(master.DisValue, plCur.Amounts),
                               Sub_Total = _fncModule.ToCurrency(master.SubTotal, plCur.Amounts),
                               VatValue = _fncModule.ToCurrency(master.VatValue, plCur.Amounts),
                               TotalAmount = _fncModule.ToCurrency(master.TotalAmount, plCur.Amounts),
                               BasedOn = master.BasedCopyKeys,
                               PLC = cu.Description,
                               //Detail
                               detail.ItemCode,
                               ItemName = detail.ItemNameKH,
                               detail.Qty,
                               Uom = detail.UomName,
                               detail.UnitPrice,
                               DisItem = _fncModule.ToCurrency(detail.DisValue, plCur.Amounts),
                               TotalItem = _fncModule.ToCurrency(detail.Total, plCur.Amounts),
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               //Summary
                               SumCount = summary.FirstOrDefault().CountInvoice,
                               SumSoldAmount = _fncModule.ToCurrency(summary.FirstOrDefault().SoldAmount, sysCur.Amounts),
                               SumDisItem = _fncModule.ToCurrency(summary.FirstOrDefault().DisCountItem, sysCur.Amounts),
                               SumDisTotal = _fncModule.ToCurrency(summary.FirstOrDefault().DisCountTotal, sysCur.Amounts),
                               SumVat = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalVatRate, sysCur.Amounts),
                               SumGrandTotal = lc.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().Total, lcCur.Amounts),
                               SumGrandTotalSys = syCurrency.Description + " " + _fncModule.ToCurrency(summary.FirstOrDefault().TotalSys, sysCur.Amounts),
                           };
                return Ok(list);
            }
            else
            {
                return Ok(new List<SaleCreditMemo>());
            }
        }

        [HttpGet]
        public IActionResult GetSalePaymentTransaction(string DateFrom, string DateTo, int CusID, int CustoID)
        {
            List<IncomingPaymentCustomer> IncomingFilter = new();
            List<SaleCreditMemo> SaleCreditMemoFilter = new();
            if (DateFrom != null && DateTo != null && CusID == 0 && CustoID == 0)
            {
                IncomingFilter = _context.IncomingPaymentCustomers.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && CusID != 0 && CustoID == 0)
            {
                IncomingFilter = _context.IncomingPaymentCustomers.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CustomerID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && CusID == 0 && CustoID != 0)
            {
                IncomingFilter = _context.IncomingPaymentCustomers.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CustomerID == CustoID).ToList();
            }
            else if (DateFrom != null && DateTo != null && CusID != 0 && CustoID != 0)
            {
                IncomingFilter = _context.IncomingPaymentCustomers.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CustomerID >= CusID && w.CustomerID <= CustoID).ToList();
            }
            else
            {
                return Ok(new List<IncomingPaymentCustomer>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var incomingpaymentcustomer = IncomingFilter;
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
            var SumTotal = incomingpaymentcustomer.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.TotalPayment * x.ExchangeRate);
            var SumBalanceDue = incomingpaymentcustomer.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.BalanceDue * x.ExchangeRate);
            var SumAppliedSSC = incomingpaymentcustomer.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.Applied_Amount * x.ExchangeRate);
            var SumAppliedLC = incomingpaymentcustomer.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.Applied_Amount * x.ExchangeRate * x.LocalSetRate);
            var incomingpay = from IPC in incomingpaymentcustomer
                              join IPD in _context.IncomingPaymentDetails.Where(x => x.Delete == false) on IPC.IncomingPaymentCustomerID equals IPD.IcoPayCusID
                              join CUS in _context.BusinessPartners on IPC.CustomerID equals CUS.ID
                              join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                              join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                              join lC in _context.Currency on IPC.LocalCurID equals lC.ID
                              join IP in _context.IncomingPayments on IPD.IncomingPaymentID equals IP.IncomingPaymentID
                              join DOC in _context.DocumentTypes on IP.DocTypeID equals DOC.ID
                              where IPC.Applied_Amount != 0 || IPC.Total == 0
                              let cus = CUS
                              let ipc = IPC
                              let ipd = IPD
                              let lcu = lC
                              let doc = DOC
                              let ip = IP
                              let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == CUR.ID) ?? new Display()
                              let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == CUN.ID) ?? new Display()
                              select new IncomingPaymentSummary
                              {
                                  //Master
                                  CusName = cus.Name,
                                  MasDocumentNo = ipc.ItemInvoice,
                                  DocType = $"{doc.Code}-{ip.InvoiceNumber}",
                                  MasDate = ipc.PostingDate.ToString("MM-dd-yyy"),
                                  MasTotal = CUN.Description + " " + _fncModule.ToCurrency(ipc.Total, plCur.Amounts),
                                  MasApplied = CUN.Description + " " + _fncModule.ToCurrency(ipc.Applied_Amount, plCur.Amounts),
                                  MasBalanceDue = CUN.Description + " " + _fncModule.ToCurrency(ipc.BalanceDue, plCur.Amounts),
                                  MasStatus = ipc.Status,
                                  //Detail
                                  DetailDate = ip.PostingDate.ToString("MM-dd-yyy"),
                                  DetailTotal = ipd == null ? "No Detail" : CUN.Description + " " + _fncModule.ToCurrency(ipd.Total, plCur.Amounts),
                                  DetailApplied = ipd == null ? "No Detail" : CUN.Description + " " + _fncModule.ToCurrency(ipd.Totalpayment, plCur.Amounts),
                                  DetailBalanceDue = ipd == null ? "No Detail" : CUN.Description + " " + _fncModule.ToCurrency(ipd.BalanceDue, plCur.Amounts),
                                  DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                  DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                  //Summary
                                  SumTotal = CUR.Description + " " + _fncModule.ToCurrency(SumTotal, sysCur.Amounts),
                                  SumBalanceDue = CUR.Description + " " + _fncModule.ToCurrency(SumBalanceDue, sysCur.Amounts),
                                  SumAppliedSSC = CUR.Description + " " + _fncModule.ToCurrency(SumAppliedSSC, sysCur.Amounts),
                                  SumAppliedLC = lcu.Description + " " + _fncModule.ToCurrency(SumAppliedLC, lcCur.Amounts),
                              };
            if (DateFrom != null && DateTo != null && CusID == 0 && CustoID == 0)
            {
                SaleCreditMemoFilter = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && CusID != 0 && CustoID == 0)
            {
                SaleCreditMemoFilter = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CusID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && CusID == 0 && CustoID != 0)
            {
                SaleCreditMemoFilter = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CusID == CustoID).ToList();
            }
            else if (DateFrom != null && DateTo != null && CusID != 0 && CustoID != 0)
            {
                SaleCreditMemoFilter = _context.SaleCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CusID >= CusID && w.CusID <= CustoID).ToList();
            }
            else
            {
                return Ok(new List<SaleCreditMemo>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";
            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var incomingpaymentcustomerCredit = SaleCreditMemoFilter;
            var credit = (from CN in incomingpaymentcustomerCredit
                          join AR in _context.SaleARs on CN.BasedOn equals AR.SARID
                          join IPC in _context.IncomingPaymentCustomers on AR.SeriesDID equals IPC.SeriesDID
                          join IPD in _context.IncomingPaymentDetails.Where(x => x.Delete == false) on IPC.IncomingPaymentCustomerID equals IPD.IcoPayCusID
                          join CUS in _context.BusinessPartners on IPC.CustomerID equals CUS.ID
                          join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                          join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                          join LC in _context.Currency on IPC.LocalCurID equals LC.ID
                          join IP in _context.IncomingPayments on IPD.IncomingPaymentID equals IP.IncomingPaymentID
                          join DOC in _context.DocumentTypes on CN.DocTypeID equals DOC.ID
                          group new { CN, AR, IPC, IPD, CUS, CUR, CUN, LC, IP, DOC } by new { IPC.SeriesDID } into datas
                          let data = datas.FirstOrDefault()
                          let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                          let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUN.ID) ?? new Display()
                          select new IncomingPaymentSummary
                          {
                              //Master
                              CusName = data.CUS.Name,
                              SeriesDID = data.IPC.SeriesDID,
                              MasDocumentNo = data.IPC.ItemInvoice,
                              DocType = $"{data.DOC.Code}-{data.CN.InvoiceNumber}",
                              MasDate = data.IPC.PostingDate.ToString("MM-dd-yyy"),
                              MasTotal = data.CUN.Description + " " + _fncModule.ToCurrency(data.IPC.Total, plCur.Amounts),
                              MasApplied = data.CUN.Description + " " + _fncModule.ToCurrency(data.IPC.Applied_Amount, plCur.Amounts),
                              MasBalanceDue = data.CUN.Description + " " + _fncModule.ToCurrency(data.IPC.BalanceDue, plCur.Amounts),
                              MasStatus = data.IPC.Status,
                              DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                              DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                              //Detail
                              DetailDate = data.CN.PostingDate.ToString("MM-dd-yyy"),
                              DetailTotal = data.IPD == null ? "No Detail" : data.CUN.Description + " " + _fncModule.ToCurrency(data.IPD.Total, plCur.Amounts),
                              DetailApplied = data.CN == null ? "No Detail" : data.CUN.Description + " " + _fncModule.ToCurrency(data.IPD.Applied_Amount, plCur.Amounts),
                              DetailBalanceDue = data.IPC == null ? "No Detail" : data.CUN.Description + " " + _fncModule.ToCurrency(data.IPD.BalanceDue, plCur.Amounts),
                              //Summary
                              SumTotal = data.CUR.Description + " " + _fncModule.ToCurrency(SumTotal, sysCur.Amounts),
                              SumBalanceDue = data.CUR.Description + " " + _fncModule.ToCurrency(SumBalanceDue, sysCur.Amounts),
                              SumAppliedSSC = data.CUR.Description + " " + _fncModule.ToCurrency(SumAppliedSSC, sysCur.Amounts),
                              SumAppliedLC = data.LC.Description + " " + _fncModule.ToCurrency(SumAppliedLC, lcCur.Amounts),
                          }).ToList();
            var allSummaryImcomingPay = new List<IncomingPaymentSummary>
  (IncomingFilter.Count + SaleCreditMemoFilter.Count);
            allSummaryImcomingPay.AddRange(incomingpay);
            allSummaryImcomingPay.AddRange(credit);
            var allInComingPayment = from IPC in allSummaryImcomingPay

                                     select new
                                     {
                                         IPC.CusName,
                                         IPC.MasDocumentNo,
                                         IPC.DocType,
                                         IPC.MasDate,
                                         IPC.MasTotal,
                                         IPC.MasApplied,
                                         IPC.MasBalanceDue,
                                         IPC.MasStatus,
                                         IPC.DateFrom,
                                         IPC.DateTo,
                                         //Detail
                                         IPC.DetailDate,
                                         IPC.DetailTotal,
                                         IPC.DetailApplied,
                                         IPC.DetailBalanceDue,
                                         //Summary
                                         IPC.SumTotal,
                                         IPC.SumBalanceDue,
                                         IPC.SumAppliedSSC,
                                         IPC.SumAppliedLC
                                     };

            return Ok(allInComingPayment);
        }


        [HttpGet]
        public IActionResult GetCustomerStatement(string DateFrom, string DateTo, int CusID, int emID)
        {
            List<IncomingPaymentCustomer> IncomingFilter = new();
            IncomingFilter = _context.IncomingPaymentCustomers.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();

            if (CusID != 0)
            {
                IncomingFilter = IncomingFilter.Where(w => w.CustomerID == CusID).ToList();
            }
            if (emID != 0)
            {
                IncomingFilter = IncomingFilter.Where(w => w.EmID == emID).ToList();
            }
            if (IncomingFilter.Count == 0)
                return Ok(new List<IncomingPaymentCustomer>());

            var incomingpaymentcustomer = IncomingFilter;
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
            var SumBalanceDueSSC = incomingpaymentcustomer.Sum(x => x.BalanceDue * x.ExchangeRate);
            var SumBalanceDueLC = incomingpaymentcustomer.Sum(x => x.BalanceDue * x.ExchangeRate * x.LocalSetRate);
            var list = from IPC in incomingpaymentcustomer
                       join CUS in _context.BusinessPartners on IPC.CustomerID equals CUS.ID
                       join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                       join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                       join lc in _context.Currency on IPC.LocalCurID equals lc.ID
                       let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == CUR.ID) ?? new Display()
                       let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == CUN.ID) ?? new Display()
                       let re = _context.Receipt.FirstOrDefault(r => r.ReceiptNo == IPC.InvoiceNumber) ?? new Receipt()
                       let users = _context.UserAccounts.FirstOrDefault(u => u.ID == re.UserOrderID) ?? new UserAccount()
                       let employ = _context.Employees.FirstOrDefault(e => e.ID == users.EmployeeID) ?? new Employee()
                       //where IPC.Status == "open"
                       select new
                       {
                           //Master
                           CusName = CUS.Name,
                           MasDocumentNo = IPC.ItemInvoice,
                           MasDate = IPC.PostingDate.ToString("MM-dd-yyy"),
                           RequiredDate = IPC.Date.ToString("MM-dd-yyy"),
                           Employee = IPC.EmName,
                           OperatedBy = IPC.CreatorName == null ? employ.Name : IPC.CreatorName,
                           OverdueDays = (IPC.Date.Date - DateTime.Now.Date).Days,
                           MasTotal = CUN.Description + " " + _fncModule.ToCurrency(IPC.Total, plCur.Amounts),
                           ApplyAmount = CUN.Description + " " + _fncModule.ToCurrency(IPC.Applied_Amount, plCur.Amounts),
                           MasBalanceDue = CUN.Description + " " + _fncModule.ToCurrency(IPC.BalanceDue, plCur.Amounts),
                           //Summary
                           SumBalanceDueSSC = CUR.Description + " " + _fncModule.ToCurrency(SumBalanceDueSSC, sysCur.Amounts),
                           SumBalanceDueLC = lc.Description + " " + _fncModule.ToCurrency(SumBalanceDueLC, lcCur.Amounts),
                           DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                           DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                       };
            return Ok(list);
        }

        //Paramater
        [HttpGet]
        public IActionResult GetCustomer()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Customer").ToList();
            return Ok(list);
        }

        public IActionResult GetBranch()
        {
            var list = _context.Branches.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        public IActionResult GetEmployee(int BranchID)
        {
            var list = from user in _context.UserAccounts.Where(x => x.Delete == false)
                       join emp in _context.Employees
                       on user.EmployeeID equals emp.ID
                       where user.BranchID == BranchID
                       select new UserAccount
                       {
                           ID = user.ID,
                           Employee = new Employee
                           {
                               Name = emp.Name
                           }
                       };
            return Ok(list);
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

        public IEnumerable<SummarySaleAdmin> GetSummarySaleTotal(string DateFrom, string DateTo, int BranchID, int UserID, int CusID, string Type)
        {
            try
            {
                var data = _context.SummarySaleAdmin.FromSql("rp_GetSummarySaleAdminTotal @DateFrom={0},@DateTo={1}, @BranchID={2},@UserID={3},@CusID={4},@Type={5}",
                parameters: new[] {
                    DateFrom.ToString(),
                    DateTo.ToString(),
                    BranchID.ToString(),
                    UserID.ToString(),
                    CusID.ToString(),
                    Type.ToString()
                }).ToList();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
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
    }
}
