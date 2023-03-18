using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.ReportSale;
using CKBS.Models.Services.ReportSale.dev;
using KEDI.Core.Premise.Authorization;

namespace CKBS.Controllers
{
    [Privilege]
    public class ChartsController : Controller
    {
        private readonly DataContext _context;
        public ChartsController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        //Sale
        [Privilege("SR001")]
        public IActionResult SummarySaleCharts()
        {
            ViewBag.Charts = "show";
            ViewBag.SaleCharts = "show";
            ViewBag.SummarySaleCharts = "highlight";
            return View();
        }

        [Privilege("SR001")]
        public IActionResult CloseShiftCharts()
        {
            ViewBag.Charts = "show";
            ViewBag.SaleCharts = "show";
            ViewBag.CloseShiftCharts = "highlight";
            return View();
        }

        [Privilege("SR001")]
        public IActionResult RevenueItemCharts()
        {
            ViewBag.Charts = "show";
            ViewBag.SaleCharts = "show";
            ViewBag.RevenueItemCharts = "highlight";
            return View();
        }

        [Privilege("SR001")]
        public IActionResult TopSaleQuantityCharts()
        {
            ViewBag.Charts = "show";
            ViewBag.SaleCharts = "show";
            ViewBag.TopSaleQuantityCharts = "highlight";
            return View();
        }

        [Privilege("SR001")]
        public IActionResult PaymentMeansCharts()
        {
            ViewBag.Charts = "show";
            ViewBag.SaleCharts = "show";
            ViewBag.PaymentMeansCharts = "highlight";
            return View();
        }

        //Purchase
        [Privilege("SR001")]
        public IActionResult PurchaseAPCharts()
        {
            ViewBag.Charts = "show";
            ViewBag.PurchaseCharts = "show";
            ViewBag.APSummaryCharts = "highlight";
            return View();
        }

        [Privilege("SR001")]
        public IActionResult OutgoingPaymentCharts()
        {
            ViewBag.Charts = "show";
            ViewBag.PurchaseCharts = "show";
            ViewBag.OutgoingPaymentCharts = "highlight";
            return View();
        }

        //Filter To Charts//
        //GetSale
        public IActionResult GetSummarySale(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<Receipt> receiptsFilter = new ();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else
            {
                return Ok(new List<Receipt>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
                var Receipts = receiptsFilter;
                var Sale = from r in Receipts
                           join user in _context.UserAccounts on r.UserOrderID equals user.ID
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           join curr in _context.Currency on r.SysCurrencyID equals curr.ID
                           group new { r, emp, curr } by emp.Name into datas
                           let data = datas.FirstOrDefault()
                           select new
                           {
                               EmpName = data.emp.Name +" ("+ data.curr.Description +") ",
                               GrandTotal = datas.Sum(c=>c.r.GrandTotal_Sys)
                           };
                return Ok(Sale);
        }
        public IActionResult GetCloseShift(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<CloseShift> closeShiftFilter = new ();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
            {
                closeShiftFilter = _context.CloseShift.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
            {
                closeShiftFilter = _context.CloseShift.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
            {
                closeShiftFilter = _context.CloseShift.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else
            {
                return Ok(new List<CloseShift>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";
            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var CloseShift = from c in closeShiftFilter
                             join b in _context.Branches on c.BranchID equals b.ID
                             join u in _context.UserAccounts on c.UserID equals u.ID
                             join e in _context.Employees on u.EmployeeID equals e.ID
                             join cp in _context.Company on b.CompanyID equals cp.ID
                             join pr in _context.PriceLists on cp.PriceListID equals pr.CurrencyID
                             join cr in _context.Currency on pr.CurrencyID equals cr.ID
                             group new { c, e } by e.Name into g
                             select new
                             {
                                 //EmpName = g.First().e.Name +" ("+ g.First().c.LocalCurrency +") ",
                                 //SaleAmount = g.Sum(c => c.c.SaleAmount)
                             };

            return Ok(CloseShift);
        }
        public IActionResult GetRevenueItem(string DateFrom, string DateTo, int BranchID)
        {
            List<RevenueItem> revenueItemsFilter = new ();
            if (DateFrom != null && DateTo != null && BranchID == 0)
            {
                revenueItemsFilter = _context.RevenueItems.Where(w => w.SystemDate >= Convert.ToDateTime(DateFrom) && w.SystemDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0)
            {
                revenueItemsFilter = _context.RevenueItems.Where(w => w.SystemDate >= Convert.ToDateTime(DateFrom) && w.SystemDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";
            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var ChartProfit = GetSummaryTotals(DateFrom, DateTo, BranchID, 0);
            if (ChartProfit != null)
            {
                return Ok(ChartProfit);
            }
            else
            {
                return Ok(revenueItemsFilter);
            }
        }

        public IActionResult GetTopSaleQuantity(string DateFrom, string DateTo, int BranchID)
        {
            List<Receipt> receiptsFilter = new();
            if (DateFrom != null && DateTo != null && BranchID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else
            {
                return Ok(new List<Receipt>());
            }
            var list = from r in receiptsFilter
                       join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                       join i in _context.ItemMasterDatas on rd.ItemID equals i.ID
                       join u in _context.UnitofMeasures on rd.UomID equals u.ID
                       group new { rd, i, u, r } by new { rd.ItemID, } into g
                       select new
                       {
                           ItemName = g.First().i.KhmerName + " " + g.First().u.Name,
                           Qty = g.Sum(c => c.rd.Qty)
                       };
            return Ok(list.OrderByDescending(o => o.Qty));
        }
        public IActionResult GetPaymentMeans(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<Receipt> receiptsFilter = new ();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else
            {
                return Ok(new List<Receipt>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var receipt = receiptsFilter;
            var list = from p in _context.PaymentMeans
                        join r in receipt on p.ID equals r.PaymentMeansID
                        join u in _context.UserAccounts on r.UserOrderID equals u.ID
                        join e in _context.Employees on u.EmployeeID equals e.ID
                        join cur in _context.Currency on r.SysCurrencyID equals cur.ID
                        group new { p, r, e, cur } by r.PaymentMeansID into g

                        let data = g.FirstOrDefault()
                        let payment = data.p

                        select new
                        {
                            PaymentMean = payment.Type +" ("+ g.First().cur.Description +") ",
                            GrandTotal = g.Sum(c=>c.r.GrandTotal_Sys)
                        };
            return Ok(list);
        }

        //GetPurchase
        public IActionResult GetPurchaseAP(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID, int WarehouseID)
        {
            List<Purchase_AP> Purchase_APs = new ();
            if (DateFrom != null && DateTo != null && BranchID == 0 && VendorID == 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && VendorID == 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && VendorID != 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && VendorID != 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.VendorID == VendorID).ToList();
            }

            else
            {
                return Ok(new List<Purchase_AP>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var Purchase_AP = Purchase_APs;
            var list = from AP in Purchase_APs
                       join BP in _context.BusinessPartners on AP.VendorID equals BP.ID
                       join CU in _context.Currency on AP.PurCurrencyID equals CU.ID
                       group new { AP, BP, CU } by BP.Code into datas
                       let data = datas.FirstOrDefault()
                       select new
                       {
                           Name = data.BP.Name + " (" + data.CU.Description + ") ",
                           GrandTotal = datas.Sum(s => s.AP.BalanceDue)
                       };
            return Ok(list);
        }

        public IActionResult GetOutgoingPayment(string DateFrom, string DateTo, int BranchID, int VendorID)
        {
            List<OutgoingPayment> OutgoingPayment = new ();

            if (DateFrom != null && DateTo != null && BranchID == 0 && VendorID == 0)
            {
                OutgoingPayment = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && VendorID == 0)
            {
                OutgoingPayment = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && VendorID != 0)
            {
                OutgoingPayment = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && VendorID != 0)
            {
                OutgoingPayment = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.VendorID == VendorID).ToList();
            }
            else
            {
                return Ok(new List<OutgoingPayment>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = from OP in OutgoingPayment
                       join BP in _context.BusinessPartners on OP.VendorID equals BP.ID
                       join opd in _context.OutgoingPaymentDetails on OP.OutgoingPaymentID equals opd.OutgoingPaymentID
                       join CU in _context.Currency on opd.CurrencyID equals CU.ID
                       group new { OP, BP, CU } by BP.ID into datas
                       let data = datas.FirstOrDefault()
                       select new
                       {
                           Name = data.BP.Name + " (" + data.CU.Description +") ",
                           GrandTotal = datas.Sum(s => s.OP.TotalAmountDue)
                       };
            return Ok(list);

        }

        public IEnumerable<SummaryTotalSale> GetSummaryTotals(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            try
            {
                var data = _context.SummaryTotalSale.FromSql("rp_GetSummarrySaleTotal @DateFrom={0},@DateTo={1}, @BranchID={2},@UserID={3}",
                parameters: new[] {
                    DateFrom.ToString(),
                    DateTo.ToString(),
                    BranchID.ToString(),
                    UserID.ToString()
                }).ToList();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }


        //Default Data Filter
        [HttpGet]
        public IActionResult GetBranch()
        {
            var list = _context.Branches.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
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

        [HttpGet]
        public IActionResult GetWarehouse(int BranchID)
        {
            var list = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetVendor()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Vendor").ToList();
            return Ok(list);
        }
    }
}