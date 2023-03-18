using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.ReportSale.dev;
using CKBS.Models.Services.ReportSaleAdmin;
using CKBS.Models.Services.Account;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class KVMSReportController : Controller
    {
        private readonly DataContext _context;
        public KVMSReportController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Privilege("SR001")]
        public IActionResult SummarySaleKvmsView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "KVMS";
            ViewBag.Subpage = "Summary Sale KVMS Report";
            ViewBag.Report = "show";
            ViewBag.KVMS = "show";
            ViewBag.SummarySaleKvms = "highlight";
            return View();
        }

        [Privilege("SR005")]
        public IActionResult DetailSaleKvmsView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "KVMS";
            ViewBag.Subpage = "Detail Sale KVMS Report";
            ViewBag.Report = "show";
            ViewBag.KVMS = "show";
            ViewBag.DetailSaleKvms = "highlight";
            return View();
        }

        [Privilege("SR007")]
        public IActionResult SaleByCusKvmsView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "KVMS";
            ViewBag.Subpage = "Sale By Customer KVMS Report";
            ViewBag.Report = "show";
            ViewBag.KVMS = "show";
            ViewBag.SaleByCusKVMS = "highlight";
            return View();
        }

        [Privilege("SA003")]
        public IActionResult PaymentTransactionKvmsView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "KVMS";
            ViewBag.Subpage = "Payment Transaction Report";
            ViewBag.Report = "show";
            ViewBag.KVMS = "show";
            ViewBag.PaymentTransactionKVMS = "highlight";
            return View();
        }

        [Privilege("SA004")]
        public IActionResult CustomerStatementKvmsView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "KVMS";
            ViewBag.Subpage = "Customer Statement Report";
            ViewBag.Report = "show";
            ViewBag.KVMS = "show";
            ViewBag.CustomerStatementKVMS = "highlight";
            return View();
        }

        [Privilege("SA002")]
        public IActionResult ReceiptQuotationKvmsView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "KVMS";
            ViewBag.Subpage = "Receipt Quotation";
            ViewBag.Report = "show";
            ViewBag.KVMS = "show";
            ViewBag.ReceiptQuotation = "highlight";
            return View();
        }

        [Privilege("SA002")]
        public IActionResult ReceiptMemoKvmsView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "KVMS";
            ViewBag.Subpage = "Receipt Memo Kvms";
            ViewBag.Report = "show";
            ViewBag.KVMS = "show";
            ViewBag.ReceiptMemoKvms = "highlight";
            return View();
        }

        //Get Datas
        public IActionResult SummarySaleKVMSReport(string DateFrom, string DateTo, string TimeFrom, string TimeTo, int BranchID, int UserID)
        {
            List<ReceiptKvms> receiptsFilter = new List<ReceiptKvms>();
            if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID == 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID != 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else if (DateFrom == null && DateTo == null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.BranchID == BranchID).ToList();
            }
            else if (DateFrom == null && DateTo == null && TimeFrom == null && TimeTo == null && BranchID == 0 && UserID != 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.UserOrderID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID == 0 && UserID == 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));

                receiptsFilter = _context.ReceiptKvms.Where(w => Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID != 0 && UserID == 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));

                receiptsFilter = _context.ReceiptKvms.Where(w => Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID != 0 && UserID != 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));

                receiptsFilter = _context.ReceiptKvms.Where(w => Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else
            {
                return Ok(new List<ReceiptKvms>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            if (TimeFrom == null)
            {
                TimeFrom = "00:00";
            }
            if (TimeTo == null)
            {
                TimeTo = "00:00";
            }

            var Summary = GetSummaryTotalsKVMS(DateFrom, DateTo, BranchID, UserID, TimeFrom, TimeTo);
            if (Summary != null)
            {
                var Receipts = receiptsFilter;
                var Sale = from r in Receipts
                           join kvmsinfo in _context.KvmsInfo on r.KvmsInfoID equals kvmsinfo.KvmsInfoID
                           join user in _context.UserAccounts on r.UserOrderID equals user.ID
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                           join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                           join cur_pl in _context.Currency on r.PLCurrencyID equals cur_pl.ID
                           group new { r, emp, curr, curr_sys, kvmsinfo, cur_pl } by new { kvmsinfo.KvmsInfoID, r.ReceiptKvmsID } into datas
                           let data = datas.FirstOrDefault()
                           select new
                           {
                               //Header
                               data.kvmsinfo.QNo,
                               CusName = data.kvmsinfo.Name,
                               data.kvmsinfo.Plate,
                               data.kvmsinfo.Frame,
                               data.kvmsinfo.Engine,
                               data.kvmsinfo.TypeName,
                               data.kvmsinfo.BrandName,
                               data.kvmsinfo.ModelName,
                               data.kvmsinfo.ColorName,
                               data.kvmsinfo.Year,
                               //detail
                               EmpCode = data.emp.Code,
                               EmpName = data.emp.Name,
                               ReceiptNo = data.r.ReceiptNo,
                               DateIn = data.r.DateIn.ToString("dd-MM-yyyy"),
                               TimeIn = data.r.TimeIn,
                               DateOut = data.r.DateOut.ToString("dd-MM-yyyy"),
                               TimeOut = data.r.TimeOut,
                               DiscountItem = string.Format("{0:#,0.000}", data.r.DiscountValue),
                               Currency = data.cur_pl.Description,
                               GrandTotal = data.r.GrandTotal.ToString("0.000"),
                               //Summary
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               SCount = string.Format("{0:n0}", Summary.FirstOrDefault().CountReceipt),
                               //SSoldAmount = string.Format("{0:#,0.000}", Summary.FirstOrDefault().SoldAmount),
                               SDiscountItem = string.Format("{0:#,0.000}", Summary.FirstOrDefault().DiscountItem),
                               SDiscountTotal = string.Format("{0:#,0.000}", Summary.FirstOrDefault().DiscountTotal),
                               SVat = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", Summary.FirstOrDefault().TaxValue),
                               SGrandTotal_Sys = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", Summary.FirstOrDefault().GrandTotalSys),
                               SGrandTotal = data.curr.Description + " " + Summary.FirstOrDefault().GrandTotal.ToString("0.000")


                           };

                return Ok(Sale.OrderBy(s => s.ReceiptNo));
            }
            return Ok(new List<ReceiptKvms>());
        }

        public IActionResult DetailSaleKVMSReport(string DateFrom, string DateTo, int BranchID, int UserID)
        {

            List<ReceiptKvms> receiptsFilter = new List<ReceiptKvms>();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else
            {
                return Ok(new List<ReceiptKvms>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var Summary = GetSummaryTotalsKVMS(DateFrom, DateTo, BranchID, UserID, "", "");
            if (Summary != null)
            {
                var Receipts = receiptsFilter;
                var Sale = from rd in _context.ReceiptDetailKvms
                           join r in Receipts on rd.ReceiptKvmsID equals r.ReceiptKvmsID
                           join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                           join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                           join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                           join user in _context.UserAccounts on r.UserOrderID equals user.ID
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           join Uom in _context.UnitofMeasures on rd.UomID equals Uom.ID
                           group new { r, emp, rd, Uom, curr, curr_sys, curr_pl } by new { r.ReceiptKvmsID, rd.ID } into datas

                           let data = datas.FirstOrDefault()
                           let receipt = data.r
                           let emp = data.emp
                           let receiptDetail = data.rd
                           let Uom = data.Uom
                           select new
                           {
                               //Master
                               MReceiptID = receipt.ReceiptKvmsID,
                               MReceiptNo = receipt.ReceiptNo,
                               MUserName = emp.Name,
                               MDateOut = receipt.DateOut.ToString("dd-MM-yyyy") + " " + receipt.TimeOut,
                               MVat = string.Format("{0:n3}", receipt.TaxValue),
                               MDisTotal = string.Format("{0:#,0.000}", receipt.DiscountValue),
                               MSubTotal = receiptDetail.Currency + " " + string.Format("{0:n3}", receipt.Sub_Total),
                               MTotal = receiptDetail.Currency + " " + string.Format("{0:n3}", receipt.GrandTotal),
                               //Detail
                               ID = receiptDetail.ID,
                               ItemCode = receiptDetail.Code,
                               KhmerName = receiptDetail.KhmerName,
                               Qty = receiptDetail.Qty,
                               Uom = Uom.Name,
                               UnitPrice = receiptDetail.UnitPrice.ToString("0.000"),
                               DisItem = receiptDetail.DiscountValue.ToString("0.000"),
                               Total = receiptDetail.Total.ToString("0.000"),
                               //Summary
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               SCount = string.Format("{0:n0}", Summary.FirstOrDefault().CountReceipt),
                               SDiscountItem = string.Format("{0:#,0.000}", Summary.FirstOrDefault().DiscountItem),
                               SDiscountTotal = string.Format("{0:#,0.000}", Summary.FirstOrDefault().DiscountTotal),
                               SVat = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", Summary.FirstOrDefault().TaxValue),
                               SGrandTotalSys = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", Summary.FirstOrDefault().GrandTotalSys),
                               SGrandTotal = data.curr.Description + " " + string.Format("{0:#,0.000}", Summary.FirstOrDefault().GrandTotal),
                           };
                return Ok(Sale);
            }
            else
            {
                return Ok(new List<ReceiptKvms>());
            }
        }

        public IActionResult SaleByCusKVMSReport(string DateFrom, string DateTo, int BranchID, int CusID)
        {
            List<ReceiptKvms> receiptsFilter = new List<ReceiptKvms>();
            if (DateFrom != null && DateTo != null && BranchID == 0 && CusID == 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && CusID == 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && CusID != 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.CustomerID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && CusID != 0)
            {
                receiptsFilter = _context.ReceiptKvms.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.CustomerID == CusID).ToList();
            }
            else
            {
                return Ok(new List<ReceiptKvms>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var Summary = GetSummaryTotalsKVMS(DateFrom, DateTo, BranchID, 0, "", "");
            if (Summary != null)
            {
                var Receipts = receiptsFilter;
                var Sale = from rd in _context.ReceiptDetailKvms
                           join r in Receipts on rd.ReceiptKvmsID equals r.ReceiptKvmsID
                           join kvmsinfo in _context.KvmsInfo on r.KvmsInfoID equals kvmsinfo.KvmsInfoID
                           join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                           join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                           join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                           join user in _context.UserAccounts on r.UserOrderID equals user.ID
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           join cus in _context.BusinessPartners on r.CustomerID equals cus.ID
                           join uom in _context.UnitofMeasures on rd.UomID equals uom.ID
                           group new { r, emp, rd, cus, uom, curr, curr_sys, curr_pl, kvmsinfo } by new { r.CustomerID, r.ReceiptKvmsID, rd.ID } into datas

                           let data = datas.FirstOrDefault()
                           let receipt = data.r
                           let emp = data.emp
                           let receiptDetail = data.rd
                           let uom = data.uom
                           select new
                           {
                               //Master
                               MCustomer = datas.FirstOrDefault().cus.Name,
                               MCusTotal = datas.FirstOrDefault().r.Sub_Total,
                               MReceiptNo = receipt.ReceiptNo,
                               MSubTotal = receiptDetail.Total,
                               MCurreny = data.curr_pl.Description,
                               //Vehicle
                               data.kvmsinfo.Plate,
                               data.kvmsinfo.BrandName,
                               data.kvmsinfo.ModelName,
                               //Detail
                               ID = receiptDetail.ID,
                               ItemCode = receiptDetail.Code,
                               KhmerName = receiptDetail.KhmerName,
                               Qty = receiptDetail.Qty,
                               Uom = uom.Name,
                               UnitPrice = receiptDetail.UnitPrice.ToString("0.000"),
                               DisItem = receiptDetail.DiscountValue.ToString("0.000"),
                               Total = receiptDetail.Total.ToString("0.000"),
                               //Summary
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               SCount = string.Format("{0:n0}", Summary.FirstOrDefault().CountReceipt),
                               SSoldAmount = string.Format("{0:#,0.000}", Summary.FirstOrDefault().SoldAmount),
                               SDiscountItem = string.Format("{0:#,0.000}", Summary.FirstOrDefault().DiscountItem),
                               SDiscountTotal = string.Format("{0:#,0.000}", Summary.FirstOrDefault().DiscountTotal),
                               SVat = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", Summary.FirstOrDefault().TaxValue),
                               SGrandTotalSys = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", Summary.FirstOrDefault().GrandTotalSys),
                               SGrandTotal = data.curr.Description + " " + string.Format("{0:#,0.000}", Summary.FirstOrDefault().GrandTotal)
                           };
                return Ok(Sale);
            }
            else
            {
                return Ok(new List<ReceiptKvms>());
            }
        }

        public IActionResult GetSalePaymentKVMSTransaction(string DateFrom, string DateTo, int CusID)
        {
            List<AgingPaymentCustomer> AgingPayment = new List<AgingPaymentCustomer>();
            if (DateFrom != null && DateTo != null && CusID == 0)
            {
                AgingPayment = _context.AgingPaymentCustomer.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && CusID != 0)
            {
                AgingPayment = _context.AgingPaymentCustomer.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CustomerID == CusID).ToList();
            }
            else
            {
                return Ok(new List<AgingPaymentCustomer>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var agingpaycuskvms = AgingPayment;
            var SumTotal = agingpaycuskvms.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.TotalPayment * x.ExchangeRate);
            var SumBalanceDue = agingpaycuskvms.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.BalanceDue * x.ExchangeRate);
            var SumApplied = agingpaycuskvms.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.Applied_Amount * x.ExchangeRate);

            var list = from IPC in agingpaycuskvms
                       join IPD in _context.AgingPaymentDetails.Where(x => x.Delete == false) on IPC.DocumentNo equals IPD.DocumentNo into k
                       from IPD in k.DefaultIfEmpty()
                       join CUS in _context.BusinessPartners on IPC.CustomerID equals CUS.ID
                       join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                       join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                       where IPC.Applied_Amount != 0 || IPC.Total == 0
                       let cus = CUS
                       let ipc = IPC
                       let ipd = IPD

                       select new
                       {
                           //Master
                           CusName = cus.Name,
                           MasDocumentNo = ipc.DocumentNo,
                           MasDate = ipc.PostingDate.ToString("MM-dd-yyy"),
                           MasTotal = CUN.Description + " " + ipc.TotalPayment.ToString("0.000"),
                           MasApplied = CUN.Description + " " + ipc.Applied_Amount.ToString("0.000"),
                           MasBalanceDue = CUN.Description + " " + ipc.BalanceDue.ToString("0.000"),
                           MasStatus = ipc.Status == StatusReceipt.Aging ? "Aging" : "Payed",
                           //Detail
                           DetailDate = ipd == null ? "No Data" : CUN.Description + " " + ipd.Date.ToString("MM-dd-yyy"),
                           DetailTotal = ipd == null ? "No Data" : CUN.Description + " " + ipd.Total.ToString("0.000"),
                           DetailCash = ipd == null ? "No Data" : CUN.Description + " " + ipd.Cash.ToString("0.000"),
                           DetailBalanceDue = ipd == null ? "No Data" : CUN.Description + " " + (ipd.BalanceDue - ipd.Cash).ToString("0.000"),
                           //Summary
                           SumTotal = CUR.Description + " " + SumTotal.ToString("0.000"),
                           SumBalanceDue = CUR.Description + " " + SumBalanceDue.ToString("0.000"),
                           SumApplied = CUR.Description + " " + SumApplied.ToString("0.000")
                       };
            return Ok(list);
        }

        public IActionResult GetCustomerKVMSStatement(string DateFrom, string DateTo, int CusID)
        {
            List<AgingPaymentCustomer> AgingPayment = new List<AgingPaymentCustomer>();
            if (DateFrom != null && DateTo != null && CusID == 0)
            {
                AgingPayment = _context.AgingPaymentCustomer.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && CusID != 0)
            {
                AgingPayment = _context.AgingPaymentCustomer.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.CustomerID == CusID).ToList();
            }
            else
            {
                return Ok(new List<AgingPaymentCustomer>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var agingpaycuskvms = AgingPayment;
            var SumBalanceDue = agingpaycuskvms.Where(x => x.Status == StatusReceipt.Aging).Sum(x => x.BalanceDue * x.ExchangeRate);

            var list = from IPC in agingpaycuskvms
                       join CUS in _context.BusinessPartners on IPC.CustomerID equals CUS.ID
                       join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                       join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                       where IPC.Status == StatusReceipt.Aging

                       select new
                       {
                           //Master
                           CusName = CUS.Name,
                           MasDocumentNo = IPC.DocumentNo,
                           MasDate = IPC.PostingDate.ToString("MM-dd-yyy"),
                           OverdueDays = (IPC.Date.Date - DateTime.Now.Date).Days,
                           MasTotal = CUN.Description + " " + IPC.TotalPayment.ToString("0.000"),
                           MasBalanceDue = CUN.Description + " " + IPC.BalanceDue.ToString("0.000"),
                           //Summary
                           SumBalanceDue = CUR.Description + " " + SumBalanceDue.ToString("0.000")
                       };
            return Ok(list);
        }

        public IActionResult GetReceiptQuotationKVMSReport(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<OrderQAutoM> orderqkvms = new List<OrderQAutoM>();

            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
            {
                orderqkvms = _context.OrderQAutoMs.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
            {
                orderqkvms = _context.OrderQAutoMs.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID != 0)
            {
                orderqkvms = _context.OrderQAutoMs.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.UserOrderID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
            {
                orderqkvms = _context.OrderQAutoMs.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else
            {
                return Ok(new List<OrderQAutoM>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var OrderQKvms = orderqkvms;
            var list = from orderq in OrderQKvms
                       join orderDq in _context.OrderDetailQAutoMs on orderq.OrderID equals orderDq.OrderID
                       join BP in _context.BusinessPartners on orderq.CustomerID equals BP.ID
                       join I in _context.ItemMasterDatas on orderDq.ItemID equals I.ID
                       join CUR in _context.Currency on orderq.PLCurrencyID equals CUR.ID
                       group new { orderq, orderDq, BP, I, CUR } by new { orderq.OrderID } into g
                       let data = g.FirstOrDefault()
                       let master = data.orderq
                       let cu = data.CUR

                       //Summary Footer
                       let SCount = orderqkvms.Count()
                       let SSoldAmount = orderqkvms.Sum(c => c.Sub_Total)
                       //let SDisItem = g.Sum(c => c.orderDq.DiscountValue)
                       let SDisTotal = orderqkvms.Sum(c => c.DiscountValue)
                       let SVat = orderqkvms.Sum(c => c.TaxValue)
                       let SGrandTotal = orderqkvms.Sum(c => c.GrandTotal)

                       select new
                       {
                           //For Print
                           ReceiptMID = master.OrderID,
                           //Master
                           InvoiceNo = _context.QuoteAutoMs.FirstOrDefault(c => c.ID == master.OrderID).QNo,
                           CusName = g.First().BP.Name,
                           PostingDate = Convert.ToDateTime(master.DateOut).ToString("MM-dd-yyy"),
                           Discount = string.Format("{0:#,0.000}", master.DiscountValue),
                           Sub_Total = string.Format("{0:#,0.000}", master.Sub_Total),
                           VatValue = string.Format("{0:#,0.000}", master.TaxValue),
                           GrandTotal = string.Format("{0:#,0.000}", master.GrandTotal),
                           //Summary
                           SumCount = SCount,
                           SumSoldAmount = string.Format("{0:#,0.000}", SSoldAmount),
                           //SumDisItem = string.Format("{0:#,0.000}", SDisItem),
                           SumDisTotal = string.Format("{0:#,0.000}", SDisTotal),
                           SumVat = cu.Description + " " + string.Format("{0:#,0.000}", SVat),
                           SumGrandTotal = cu.Description + " " + string.Format("{0:#,0.000}", SGrandTotal)

                       };
            return Ok(list);
        }

        public IActionResult GetReceiptMemoKVMSReport(string DateFrom, string DateTo, int BranchID, int UserID, int CusID)
        {
            List<ReceiptMemo> receiptMemoKv = new List<ReceiptMemo>();

            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID == 0)
            {
                receiptMemoKv = _context.ReceiptMemo.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID == 0)
            {
                receiptMemoKv = _context.ReceiptMemo.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID == 0)
            {
                receiptMemoKv = _context.ReceiptMemo.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && CusID != 0)
            {
                receiptMemoKv = _context.ReceiptMemo.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.CustomerID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                receiptMemoKv = _context.ReceiptMemo.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID && w.CustomerID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && CusID != 0)
            {
                receiptMemoKv = _context.ReceiptMemo.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID && w.CustomerID == CusID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && CusID != 0)
            {
                receiptMemoKv = _context.ReceiptMemo.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.CustomerID == CusID).ToList();
            }
            else
            {
                return Ok(new List<ReceiptMemo>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var summary = GetSummarySaleTotalKVMS(DateFrom, DateTo, BranchID, UserID, CusID, "RM");
            if (summary != null)
            {
                var FilterReceiptMemo = receiptMemoKv;
                var list = from RM in FilterReceiptMemo
                           join RMD in _context.ReceiptDetailMemoKvms on RM.ID equals RMD.ReceiptMemoID
                           join BP in _context.BusinessPartners on RM.CustomerID equals BP.ID
                           join I in _context.ItemMasterDatas on RMD.ItemID equals I.ID
                           join CUR in _context.Currency on RM.PLCurrencyID equals CUR.ID
                           group new { RM, RMD, BP, I, CUR } by new { RM.ID } into g
                           let data = g.FirstOrDefault()
                           let master = data.RM
                           let detail = data.RMD
                           let cu = data.CUR
                           select new
                           {
                               //For Print
                               ReceiptMID = master.ID,
                               //Master
                               InvoiceNo = master.ReceiptMemoNo,
                               CusName = g.First().BP.Name,
                               PostingDate = Convert.ToDateTime(master.DateOut).ToString("MM-dd-yyy"),
                               Discount = string.Format("{0:#,0.000}", master.DisValue),
                               Sub_Total = string.Format("{0:#,0.000}", master.SubTotal),
                               VatValue = string.Format("{0:#,0.000}", master.TaxValue),
                               GrandTotal = string.Format("{0:#,0.000}", master.GrandTotal),
                               BasedOn = master.ReceiptNo,
                               //Detail
                               //ItemCode = detail.Code,
                               //ItemName = detail.KhmerName,
                               //Qty = detail.Qty,
                               //Uom = _context.UnitofMeasures.FirstOrDefault(c=>c.ID == detail.UomID).Name,
                               //UnitPrice = detail.UnitPrice,
                               //DisItem = string.Format("{0:n3}", detail.DiscountValue),
                               //TotalItem = string.Format("{0:n3}", detail.Total),
                               //Summary
                               SumCount = summary.FirstOrDefault().CountInvoice,
                               SumSoldAmount = string.Format("{0:#,0.000}", summary.FirstOrDefault().SoldAmount),
                               SumDisItem = string.Format("{0:#,0.000}", summary.FirstOrDefault().DisCountItem),
                               SumDisTotal = string.Format("{0:#,0.000}", summary.FirstOrDefault().DisCountTotal),
                               SumVat = cu.Description + " " + string.Format("{0:#,0.000}", summary.FirstOrDefault().TotalVatRate),
                               SumGrandTotal = cu.Description + " " + string.Format("{0:#,0.000}", summary.FirstOrDefault().Total)

                           };
                return Ok(list);
            }
            else
            {
                return Ok(new List<ReceiptMemo>());
            }
        }

        //Store
        public IEnumerable<SummaryTotalSale> GetSummaryTotalsKVMS(string DateFrom, string DateTo, int BranchID, int UserID, string TimeFrom, string TimeTo)
        {
            try
            {
                var data = _context.SummaryTotalSale.FromSql("rp_GetSummarrySaleTotalKVMS @DateFrom={0},@DateTo={1}, @BranchID={2},@UserID={3},@TimeFrom={4},@TimeTo={5}",
                parameters: new[] {
                    DateFrom.ToString(),
                    DateTo.ToString(),
                    BranchID.ToString(),
                    UserID.ToString(),
                    TimeFrom.ToString(),
                    TimeTo.ToString()
                }).ToList();
                return data;
            }
            catch (Exception exx)
            {
                var e = exx.Message;
                return null;
            }
        }

        public IEnumerable<SummarySaleAdmin> GetSummarySaleTotalKVMS(string DateFrom, string DateTo, int BranchID, int UserID, int CusID, string Type)
        {
            try
            {
                var data = _context.SummarySaleAdmin.FromSql("rp_GetSummarySaleAdminTotalKVMS @DateFrom={0},@DateTo={1}, @BranchID={2},@UserID={3},@CusID={4},@Type={5}",
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

        //Paramater
        [HttpGet]
        public IActionResult GetVendor()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Vendor").ToList();
            return Ok(list);
        }

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

        public IActionResult GetWarehouse(int BranchID)
        {
            var list = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetBranchFrom()
        {
            var list = _context.Branches.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetBranchTo()
        {
            var list = _context.Branches.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehouseFrom(int BranchID)
        {
            var list = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehouseTo(int BranchID)
        {
            var list = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == BranchID).ToList();
            return Ok(list);
        }
    }
}
