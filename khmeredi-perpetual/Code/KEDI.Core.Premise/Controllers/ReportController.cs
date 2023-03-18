using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using CKBS.AppContext;
using CKBS.Controllers.Event;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.SignalR;
using KEDI.Core.Premise.Authorization;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CKBS.Controllers
{
    [Privilege]
    public class ReportController : Controller
    {
        private readonly DataContext _context;
        private readonly IReport _report;
        //private readonly TimeDelivery _timeDelivery;
        public ReportController(DataContext context,IReport report)
        {
            _context = context;
            _report = report;
        }

        [Privilege("SR001")]
        public IActionResult SummarySale()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale Summary";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.SaleSummary = "highlight";
            return View();
        }

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
                       join emp in _context.Employees.Where(x => x.Delete == false && x.IsUser == true)
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
        public IActionResult GetSummarySale(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetSummarySales(DateFrom, DateTo, BranchID, UserID).ToList() ?? 
                new List<Models.Services.ReportSale.SummarySale>();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetailSale(int OrderID)
        {
            var list = _report.GetDetailSales(OrderID).ToList();
            return Ok(list);
        }
        //end

        [Privilege("PR002")]
        public IActionResult SummaryPurchaseAP()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Purchaes AP Summary";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.APSummary = "highlight";
            return View();
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
            var list = _context.BusinessPartners.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetSummaryPurchaseAP(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID, int WarehouseID)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetSummaryPurchaseAPs(DateFrom, DateTo, BranchID, WarehouseID, UserID, VendorID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetaiPurchaseAP(int PurchaseID)
        {
            var list = _report.GetDetailPurchaseAps(PurchaseID).ToList();
            return Ok(list);
        }

        [Privilege("SR002")]
        public IActionResult CloseShift()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Close Shift";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.CloseShift = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetSummaryCloseShift(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetReportCloseShfts(DateFrom, DateTo, BranchID, UserID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetailCloseShift(int UserID,int Tran_From,int Tran_To)
        {
            var list = _report.GetDetailCloseShif(UserID, Tran_From, Tran_To).ToList();
            return Ok(list);
        }

        [Privilege("SR003")]
        public IActionResult TopSaleQuantity()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Top Sale Quantity";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.TopSaleQuantity = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetTopSaleQuantity(string DateFrom,string DateTo,int BranchID)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetTopSaleQuantities(DateFrom, DateTo, BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetailTopSaleQty(int ItemID,int UomID,string DateFrom,string DateTo,int BranchID)
        {
            var list = _report.DetailTopSaleQties(ItemID,UomID,DateFrom,DateTo,BranchID).ToList();
            return Ok(list);
        }

        [Privilege("PR003")]
        public IActionResult SummaryPurchaseMemo()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Purchaes Credit Memo Summary";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.PCSummary = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetSummaryPurchaseMemo(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID, int WarehouseID)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetPruchaseMemoSummary(DateFrom, DateTo, BranchID, WarehouseID, UserID, VendorID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetaiPurchaseMemo(int PurchaseID)
        {
            var list = _report.GetDetailPurchaseMemo(PurchaseID).ToList();
            return Ok(list);
        }

        [HttpGet]
        [Privilege("PR001")]
        public IActionResult SummaryPurchaseOrder()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Purchaes Order Summry";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.POSummary = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetSummaryPurchaseOrder(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID, int WarehouseID)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetPruchaseOrderSummary(DateFrom, DateTo, BranchID, WarehouseID, UserID, VendorID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetaiPurchaseOrder(int PurchaseID)
        {
            var list = _report.GetDetailPurchaseOrder(PurchaseID).ToList();
            return Ok(list);
        }

        [HttpGet]
        [Privilege("PR004")]
        public IActionResult SummaryOutgoingPayment()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Out Going Payment";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.OutgoingPayment = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetOutgoingpayment(string DateFrom,string DateTo,int BranchID,int UserID,int VendorID)
        {
            var list = _report.GetSummaryOutgoings(DateFrom, DateTo, BranchID, UserID, VendorID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetailOutgoingPayment(int OutID)
        {
            var list = _report.GetDetailOutgoingPayments(OutID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetailInvoice(string Invoice)
        {
            var list = _report.GetDetailInvoice(Invoice).ToList();
            return Ok(list);
        }

        [HttpGet]
        [Privilege("IR001")]
        public IActionResult StockInWarehouse()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Stock In Warehouse";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.StockInWarehouse = "highlight";
            return View();
        }

        [Privilege("IR001")]
        public IActionResult StockAudit()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Live Stock";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.StockAudit = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetStockInWarehouse(int WarehouseID, string Process,int ItemID)
        {
            var list = _report.GetStockInWarehouses(WarehouseID, Process,ItemID).ToList();
            return Ok(list);
        }

        public IActionResult GetStockAudit()
        {
            var list = _report.GetStockAudit().ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetailStockInWarehouse(int ItemID,string Process,int WarehouseID)
        {
            var list = _report.GetStockInWarehouse_Details(ItemID, Process, WarehouseID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetInventoryAuditByItem(int ItemID,int WarehouseID)
        {
            var list = _report.GetServiceInventoryAudits(ItemID,WarehouseID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehousefilter(int BranchID)
        {
            var list = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        [Privilege("IR002")]
        public IActionResult SummaryTransferStock()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Summary Transfer Stock";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.TransferStock = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetBranchTo()
        {
            var list = _context.Branches.Where(x => x.Delete == false).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetWarehouseTo(int BranchID)
        {
            var list = _context.Warehouses.Where(x => x.Delete == false && x.BranchID == BranchID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetSummaryTransferStock(string DateFrom,string DateTo,int FromBranchID,int ToBranchID,int FromWarehouse,int ToWarehouse,int UserID)
        {

            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetSummaryTransferStocks(DateFrom, DateTo, FromBranchID, ToBranchID, FromWarehouse, ToWarehouse, UserID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetailTarnsfer(int TranID)
        {
            var list = _report.GetSummaryDetaitTransferStocks(TranID).ToList();
            return Ok(list);
        }

        [HttpGet]
        [Privilege("IR003")]
        public IActionResult SummaryGoodsReceiptStock()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Goods Receipt Stock";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.GoodsReceiptStock = "highlight";
            return View();
        }

        [HttpGet]
        [Privilege("IR004")]
        public IActionResult SummaryGoodsIssuseStock()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Goods Issuse Stock";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.GoodsIssueStock = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetSummaryGoodsReceipt(string DateFrom,string DateTo,int BranchID,int WarehouseID,int UserID)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetSummaryGoodsReceiptStock(DateFrom, DateTo, BranchID, WarehouseID, UserID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetailGoodsReceipt(int ReceiptID)
        {
            var list = _report.GetSummaryDetailGoodsReceipt(ReceiptID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetSummaryGoodsIssuse(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetSummaryGoodsIssuseStock(DateFrom, DateTo, BranchID, WarehouseID, UserID).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetDetailGoodsIssuse(int IssuseID)
        {
            var list = _report.GetSummaryDetailGoodsIssuse(IssuseID).ToList();
            return Ok(list);
        }

        [HttpGet]
        [Privilege("SR004")]
        public IActionResult RevenuesItem()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Revenues Item";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.RevenuesItem = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetSummaryRevenesItem(string DateFrom ,string DateTo,int BranchID,int ItemID,string Process)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetSummaryRevenuesItems(DateFrom, DateTo, BranchID, ItemID, Process).ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetSummaryDetailRevenesitem(string DateFrom,string DateTo,int BranchID,int ItemID,string Process)
        {
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var list = _report.GetSummaryRevenuesItemsDetail(DateFrom, DateTo, BranchID, ItemID, Process).ToList();
            return Ok(list);
        }
        
    }
}
