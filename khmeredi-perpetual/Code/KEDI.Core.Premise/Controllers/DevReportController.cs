using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Inventory.Transaction;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.Purchase;
using CKBS.Models.Services.ReportPurchase.dev;
using CKBS.Models.Services.ReportSale;
using CKBS.Models.Services.ReportSale.dev;
using CKBS.Models.ServicesClass.Report;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.POS.KVMS;
using KEDI.Core.Premise.Models.Services.ReportPurchase.SummaryOutgoinPaymentDetail;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.Services.POS;
using KEDI.Core.Premise.Models.Services.RemarkDiscount;
using Microsoft.AspNetCore.Mvc.Rendering;
using CKBS.Models.Services.Inventory.PriceList;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.ServicesClass.Report;
using CKBS.Models.Services.Responsitory;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram;
using KEDI.Core.Premise.Models.Services.ReportSale;
using KEDI.Core.Premise.Repositories.Sync;
using KEDI.Core.Premise.Models.InventoryAuditReport;
using Models.Services.ReportSale.dev;
using KEDI.Core.Premise.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.IO;
using KEDI.Core.Premise.Models.Files;
using NPOI.SS.UserModel;
using KEDI.Core.Premise;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Utilities;
using CKBS.Models.Services.Pagination;
using KEDI.Core.Premise.Models.Services.CustomerConsignments;

namespace CKBS.Controllers
{
    [Privilege]
    public class DevReportController : Controller
    {
        private readonly DataContext _context;
        private readonly UtilityModule _fncModule;
        private readonly IReport _report;
        private readonly UserAccount _user;
        private readonly IPosExcelRepo _posExcelRepo;
        private readonly WorkbookContext _workbook;
        private readonly IWorkbookAdapter _wbExport;
        private readonly IPosSyncRepo _posSyncRepo;
        public DevReportController(DataContext context, UtilityModule format, IReport report,
            UserManager userModule, IPosExcelRepo posExcelRepo, IPosSyncRepo posSyncRepo, IWorkbookAdapter wbAdapter
        )
        {
            _context = context;
            _fncModule = format;
            _report = report;
            _user = userModule.CurrentUser;
            _posSyncRepo = posSyncRepo;
            _posExcelRepo = posExcelRepo;
            _wbExport = wbAdapter;
            _workbook = new WorkbookContext();
        }
        private List<PriceLists> Getpriclist()
        {
            var pricelist = _context.PriceLists.Where(x => x.Delete == false).ToList();
            return pricelist;
        }
        private List<UserAccount> Users()
        {
            var users = _context.UserAccounts.Where(x => x.Delete == false).ToList();
            return users;
        }
        //Sale View
        [HttpGet]
        [Privilege("SR001")]
        public IActionResult SummarySaleView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Summary Sale Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.SummarySale = "highlight";
            ViewBag.PriceLists = new SelectList(Getpriclist(), "ID", "Name");
            return View();
        }
        [HttpGet]
        [Privilege("SR010")]
        public IActionResult CountMemberView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Count Member";
            ViewBag.Subpage = "Count Member Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.CountMember = "highlight";
            ViewBag.PriceLists = new SelectList(Getpriclist(), "ID", "Name");
            ViewBag.UserLists = new SelectList(Users(), "ID", "Username");
            return View();
        }
        [Privilege("SR009")]
        public IActionResult GroupCustomerView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Group Customer";
            ViewBag.Subpage = "Group Cusromer Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.GroupCustomer = "highlight";
            ViewBag.PriceLists = new SelectList(Getpriclist(), "ID", "Name");
            return View();
        }
        [Privilege("SR001")]
        public IActionResult SeleCreditMemoReport()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Summary Sale Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.SaleCreditMemoReport = "highlight";
            return View();
        }

        [Privilege("SR005")]
        public IActionResult DetailSaleView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Detail Sale Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.DetailSale = "highlight";
            return View();
        }

        [Privilege("SR002")]
        public IActionResult CloseShiftView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Close Shift Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.CloseShift = "highlight";
            return View();
        }

        [Privilege("SR004")]
        public IActionResult RevenueItemView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Profit & Loss Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.RevenueItem = "highlight";
            return View();
        }

        [Privilege("SR003")]
        public IActionResult TopSaleQuantityView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Top Sale Quantity";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.TopSaleQuantity = "highlight";
            return View();
        }

        [Privilege("SR006")]
        public IActionResult PaymentMeansView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Payment Means";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.PaymentMeans = "highlight";
            ViewBag.PaymentMeanList = new SelectList(_context.PaymentMeans.Where(i => !i.Delete), "ID", "Type");
            return View();
        }

        [Privilege("SR007")]
        public IActionResult SaleByCustomerView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Sale By Customer Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.SaleByCustomer = "highlight";
            return View();
        }

        [Privilege("SR008")]
        public IActionResult TaxDeclarationView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Tax Declaration Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.TaxDeclaration = "highlight";
            return View();
        }

        [Privilege("A048")]
        public IActionResult SaleByTableView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Tax Declaration Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.SaleByTable = "highlight";
            return View();
        }

        //Purchase View
        [Privilege("PR001")]
        public IActionResult SummaryPurchaseOrder()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Purchase";
            ViewBag.Subpage = "Purchaes Order Summary";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.POSummary = "highlight";
            return View();
        }

        [Privilege("PR005")]
        public IActionResult SummaryGoodsReceiptPO()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Purchase";
            ViewBag.Subpage = "Goods Receipt PO Summary";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.GRPOSummary = "highlight";
            return View();
        }

        [Privilege("PR002")]
        public IActionResult SummaryPurchaseAP()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Purchase";
            ViewBag.Subpage = "Purchase AP Summary";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.APSummary = "highlight";
            return View();
        }

        [Privilege("PR003")]
        public IActionResult SummaryPurchaseMemo()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Purchase";
            ViewBag.Subpage = "Purchase CreditMemo Summary";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.PCSummary = "highlight";
            return View();
        }

        [Privilege("PR004")]
        public IActionResult SummaryOutgoingPayment()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Outgoing Payment Summary";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.OutgoingPayment = "highlight";
            return View();
        }

        //SummaryPurchasePaymentTransaction
        [Privilege("PR005")]
        public IActionResult SummaryPurchasePaymentTransaction()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Purchase";
            ViewBag.Subpage = "Payment Transaction";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.PurchasePaymentTransaction = "highlight";
            return View();
        }

        [Privilege("PR006")]
        public IActionResult SummaryPurchaseVendorStatement()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Purchase";
            ViewBag.Subpage = "Vendor Statement";
            ViewBag.Report = "show";
            ViewBag.Purchase = "show";
            ViewBag.VendorStatement = "highlight";
            return View();
        }

        [Privilege("RP003")]
        public IActionResult DataSelectionExport()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Data Selection Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.DataSelectionExport = "highlight";
            return View(new FormDataSelection());
        }

        //Inventory View
        [Privilege("IR001")]
        public IActionResult StockInWarehouseView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Stock In Warehouse";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.StockInWarehouse = "highlight";
            return View();
        }

        [Privilege("IR005")]
        public IActionResult InventoryAuditView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Inventory Audit";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.InventoryAudit = "highlight";
            return View();
        }

        [Privilege("IR006")]
        public IActionResult StockMovingView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Stock Moving";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.StockMoving = "highlight";
            return View();
        }

        [Privilege("IR007")]
        public IActionResult StockExpiredView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Stock Expired";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.StockExpired = "highlight";
            return View();
        }

        [Privilege("SR002")]
        public IActionResult TransferStockView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Transfer Stock";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.TransferStock = "highlight";
            return View();
        }

        [Privilege("SR003")]
        public IActionResult GoodsReceiptStockView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Goods Receipt Stock";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.GoodsReceiptStock = "highlight";
            return View();
        }

        [Privilege("SR004")]
        public IActionResult GoodsIssueStockView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Inventory";
            ViewBag.Subpage = "Goods Issue Stock";
            ViewBag.Report = "show";
            ViewBag.InventoryReport = "show";
            ViewBag.GoodsIssueStock = "highlight";
            return View();
        }
        [Privilege("A047")]
        public IActionResult VoidOrderView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Void Order Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.VoidOrder = "highlight";
            return View();
        }
        [Privilege("RP002")]
        public IActionResult VoidItemView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Void Item Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.VoidItem = "highlight";
            return View();
        }
        public IActionResult PurchaseSummaryReport()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "PurchaseSummary Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.PurchaseSummaryReport = "highlight";
            ViewBag.PriceLists = new SelectList(Getpriclist(), "ID", "Name");
            return View();
        }
        [Privilege("SR011")]
        public IActionResult PaymentMeansSummaryView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Payment Means Summary";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.PaymentMeansSummary = "highlight";
            ViewBag.PaymentMeanList = new SelectList(_context.PaymentMeans.Where(i => !i.Delete), "ID", "Type");
            return View();
        }
        public IActionResult CustomerConsignmentView()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Pos";
            ViewBag.Subpage = "Customer Consignment Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.CusConsignment = "highlight";
            return View();
        }
        //End View
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





        public IActionResult CloseShiftReport(ShiftDateType ShiftDate, string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<CloseShift> closeShiftFilter = new();
            if (ShiftDate == ShiftDateType.OpenShift)
            {
                if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
                {
                    closeShiftFilter = _context.CloseShift.Where(w => w.DateIn >= Convert.ToDateTime(DateFrom) && w.DateIn <= Convert.ToDateTime(DateTo)).ToList();
                }
                else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
                {
                    closeShiftFilter = _context.CloseShift.Where(w => w.DateIn >= Convert.ToDateTime(DateFrom) && w.DateIn <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
                }
                else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
                {
                    closeShiftFilter = _context.CloseShift.Where(w => w.DateIn >= Convert.ToDateTime(DateFrom) && w.DateIn <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
                }
                else
                {
                    return Ok(new List<CloseShift>());
                }

            }
            else
            {
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
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";
            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var GrandTotal = from c in closeShiftFilter
                             select new
                             {
                                 c.SaleAmount_Sys,
                                 CashOut = c.CashInAmount_Sys + c.SaleAmount_Sys,
                             };
            var TotalSale = GrandTotal.Sum(s => s.SaleAmount_Sys);
            var TotalCashOut = GrandTotal.Sum(s => s.CashOut);
            var CloseShift = from c in closeShiftFilter
                             join b in _context.Branches on c.BranchID equals b.ID
                             join u in _context.UserAccounts on c.UserID equals u.ID
                             join e in _context.Employees on u.EmployeeID equals e.ID
                             join cp in _context.Company on b.CompanyID equals cp.ID
                             join cr in _context.Currency on c.SysCurrencyID equals cr.ID
                             let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == cr.ID) ?? new Display()
                             let receipt = _context.Receipt.FirstOrDefault(s => s.ReceiptID == c.Trans_To)
                             let receiptMemo = _context.ReceiptMemo.FirstOrDefault(s => s.ReceiptMemoNo == receipt.ReceiptNo) ?? new ReceiptMemo()
                             select new
                             {
                                 Trans = c.Trans_From + "/" + c.Trans_To + "/" + c.UserID,
                                 Branch = b.Name,
                                 EmpCode = e.Code,
                                 EmpName = e.Name,
                                 DateIn = c.DateIn.ToString("dd-MM-yyyy"),
                                 c.TimeIn,
                                 DateOut = c.DateOut.ToString("dd-MM-yyyy"),
                                 c.TimeOut,
                                 CashInAmountSys = _fncModule.ToCurrency(c.CashInAmount_Sys, sysCur.Amounts),
                                 CashOutAmountSys = _fncModule.ToCurrency(c.CashOutAmount_Sys, sysCur.Amounts),
                                 SaleAmountSys = _fncModule.ToCurrency(c.SaleAmount_Sys - receiptMemo.SubTotal, sysCur.Amounts),
                                 TotalSaleAmount = _fncModule.ToCurrency(TotalSale, sysCur.Amounts),
                                 TotalCashOutSys = _fncModule.ToCurrency(c.CashInAmount_Sys + c.SaleAmount_Sys - receiptMemo.SubTotal, sysCur.Amounts),
                                 TotalCashOut = _fncModule.ToCurrency(TotalCashOut, sysCur.Amounts),
                                 SSC = cr.Description,
                                 DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                 DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                 //columns for sum data                                 
                             };
            return Ok(CloseShift);
        }

        public IActionResult RevenueItemReport(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<RevenueItem> revenueItemsFilter = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
            {
                revenueItemsFilter = _context.RevenueItems.Where(w => w.SystemDate >= Convert.ToDateTime(DateFrom) && w.SystemDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
            {
                revenueItemsFilter = _context.RevenueItems.Where(w => w.SystemDate >= Convert.ToDateTime(DateFrom) && w.SystemDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
            {
                revenueItemsFilter = _context.RevenueItems.Where(w => w.SystemDate >= Convert.ToDateTime(DateFrom) && w.SystemDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";
            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var Summary = GetSummaryTotals(DateFrom, DateTo, BranchID, UserID, "", "");
            if (Summary != null)
            {
                var revenue = from rv in revenueItemsFilter
                              join uom in _context.UnitofMeasures on rv.UomID equals uom.ID
                              join cur in _context.Currency on rv.CurrencyID equals cur.ID
                              join wh in _context.Warehouses on rv.WarehouseID equals wh.ID
                              join item in _context.ItemMasterDatas on rv.ItemID equals item.ID
                              orderby rv.InvoiceNo, item.Code
                              group new { rv, uom, cur, wh, item } by new { rv.WarehouseID, rv.ItemID, rv.Cost } into datas
                              let data = datas.FirstOrDefault()
                              let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.cur.ID) ?? new Display()
                              select new
                              {
                                  data.rv.InvoiceNo,
                                  data.item.Barcode,
                                  data.rv.ItemID,
                                  ItemCode = data.item.Code,
                                  ItemName = data.item.KhmerName,
                                  Qty = datas.Sum(s => s.rv.Qty),
                                  Uom = data.uom.Name,
                                  Cost = _fncModule.ToCurrency(datas.Max(m => m.rv.Cost), plCur.Amounts),
                                  TotalCost = _fncModule.ToCurrency(datas.Sum(s => s.rv.Trans_Valuse), plCur.Amounts),
                                  Currency = data.cur.Description,
                                  WarehouseCode = data.wh.Code,
                                  DateOut = data.rv.SystemDate.ToString("dd-MM-yyyy") + " " + data.rv.TimeIn,
                                  //Summary
                                  DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                  DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                  SCount = string.Format("{0:n0}", Summary.FirstOrDefault().CountReceipt),
                                  SSoldAmount = _fncModule.ToCurrency(Summary.FirstOrDefault().SoldAmount, plCur.Amounts),
                                  SDiscountItem = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, plCur.Amounts),
                                  SDiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, plCur.Amounts),
                                  SVat = data.cur.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().TaxValue, plCur.Amounts),
                                  SGrandTotal = data.cur.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotal, plCur.Amounts),
                                  //STotalCost = data.cur.Description + " " + Summary.FirstOrDefault().TotalCost.ToString("0.000"),
                                  //STotalProfit = data.cur.Description + " " + Summary.FirstOrDefault().TotalProfit.ToString("0.000")
                              };
                return Ok(revenue);
            }
            return Ok(revenueItemsFilter);
        }

        public IActionResult TopSaleQuantityReport(string DateFrom, string DateTo, int BranchID, string TimeFrom, string TimeTo)
        {
            List<SaleAR> saleARFilter = new();
            List<SaleAREdite> saleAREditesFilter = new();
            List<SaleCreditMemo> SaleCreditMemoFilter = new();
            TimeSpan _timeFrom = Convert.ToDateTime(TimeFrom).TimeOfDay;
            TimeSpan _timeTo = Convert.ToDateTime(TimeTo).TimeOfDay;
            DateTime dateFrom = Convert.ToDateTime(DateFrom);
            DateTime dateto = Convert.ToDateTime(DateTo);
            List<Receipt> receiptsFilter = _context.Receipt.Where(r => r.CompanyID == GetCompany().ID && _fncModule.IsBetweenDate(dateFrom, dateto, r.DateOut)).ToList();
            List<ReceiptMemo> receiptsMemoFilter = new();
            if (_timeFrom.Minutes > 0 || _timeTo.Minutes > 0)
            {
                receiptsFilter = _context.Receipt.Where(r => _fncModule.IsBetweenDate(dateFrom, dateto, _fncModule.ConcatDateTime(r.DateOut.ToString(), r.TimeOut))).ToList();

                receiptsMemoFilter = _context.ReceiptMemo.Where(r => _fncModule.IsBetweenDate(dateFrom, dateto, _fncModule.ConcatDateTime(r.DateOut.ToString(), r.TimeOut))).ToList();

            }

            double TotalDis = 0;
            double TotalDisCal = 0;

            if (DateFrom != null && DateTo != null && BranchID == 0)
            {
                receiptsFilter = _context.Receipt.Include(i => i.RececiptDetail).Where(w => w.DateOut >= dateFrom && w.DateOut <= dateto).ToList();
                receiptsMemoFilter = _context.ReceiptMemo.Where(w => w.DateOut >= dateFrom && w.DateOut <= dateto).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0)
            {
                receiptsFilter = _context.Receipt.Include(i => i.RececiptDetail).Where(w => w.DateOut >= dateFrom && w.DateOut <= dateto && w.BranchID == BranchID).ToList();
                receiptsMemoFilter = _context.ReceiptMemo.Where(w => w.DateOut >= dateFrom && w.DateOut <= dateto && w.BranchID == BranchID).ToList();
            }
            else
            {
                return Ok(new List<Receipt>());
            }
            TotalDis += receiptsFilter.Sum(s => s.DiscountValue * s.PLRate);
            TotalDis -= receiptsMemoFilter.Sum(s => s.DisValue * s.PLRate);
            TotalDisCal += receiptsFilter.Sum(s => s.DiscountValue * s.LocalSetRate);
            TotalDisCal -= receiptsMemoFilter.Sum(s => s.DisValue * s.LocalSetRate);
            var sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == _user.Company.SystemCurrencyID) ?? new Display();
            var localCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == _user.Company.LocalCurrencyID) ?? new Display();
            //var plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr.ID) ?? new Display();
            #region Receipt
            var list = (from r in receiptsFilter
                        join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                        join i in _context.ItemMasterDatas on rd.ItemID equals i.ID
                        join g1 in _context.ItemGroup1 on i.ItemGroup1ID equals g1.ItemG1ID
                        join g2 in _context.ItemGroup2 on i.ItemGroup2ID equals g2.ItemG2ID
                        join g3 in _context.ItemGroup3 on i.ItemGroup3ID equals g3.ID
                        join u in _context.UnitofMeasures on rd.UomID equals u.ID
                        join b in _context.Branches on r.BranchID equals b.ID
                        join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                        join curr in _context.Currency on r.PLCurrencyID equals curr.ID
                        join LCcurr in _context.Currency on r.LocalCurrencyID equals LCcurr.ID
                        group new { r, rd, i, g1, g2, g3, u, curr_sys, curr, LCcurr, } by new { rd.ItemID, rd.UnitPrice } into g
                        let data = g.FirstOrDefault()
                        let sumByBranch = receiptsFilter.Where(_r => _r.BranchID == data.r.BranchID).Sum(_as => _as.GrandTotal_Sys)
                        select new TopSaleQtyReport
                        {
                            GroupName = g.First().g1.Name + "/" + g.First().g2.Name + "/" + g.First().g3.Name,
                            ItemID = g.Key.ItemID,
                            Group1 = data.g1.Name,
                            SumGrandTotalSys = data.r.GrandTotal_Sys,
                            Barcode = data.i.Barcode,
                            Code = data.i.Code,
                            KhmerName = data.i.KhmerName,
                            Qty = g.Sum(c => c.rd.Qty),
                            Uom = data.u.Name,
                            BrandID = data.r.BranchID,
                            SysCurrency = data.curr_sys.Description,
                            Currency = data.curr.Description,
                            LocalCurrency = data.LCcurr.Description,
                            PriceCal = data.rd.UnitPrice * data.r.PLRate,
                            TotalCal = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate),
                            SubTotalCal = (data.rd.Qty * data.rd.UnitPrice * data.r.PLRate),
                            DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                            DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                            //Summary
                            SDiscountItemCal = g.Sum(c => c.rd.DiscountValue * data.r.PLRate),
                            SDiscountTotalCal = g.Sum(c => c.r.DiscountValue * data.r.PLRate),
                            SDiscountItemCallocal = g.Sum(c => c.rd.DiscountValue * data.r.LocalSetRate),
                            SVatCal = data.r.TaxValue * data.r.ExchangeRate,
                            GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                            SGrandTotalSys = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate),
                            SGrandTotal = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate) * data.r.LocalSetRate,
                            LocalSetRate = data.r.LocalSetRate,
                        }).ToList();

            #endregion
            #region Receipt Credit Memo
            var listMemo = (from r in receiptsMemoFilter
                            join rd in _context.ReceiptDetailMemoKvms on r.ID equals rd.ReceiptMemoID
                            join i in _context.ItemMasterDatas on rd.ItemID equals i.ID
                            join g1 in _context.ItemGroup1 on i.ItemGroup1ID equals g1.ItemG1ID
                            join g2 in _context.ItemGroup2 on i.ItemGroup2ID equals g2.ItemG2ID
                            join g3 in _context.ItemGroup3 on i.ItemGroup3ID equals g3.ID
                            join u in _context.UnitofMeasures on rd.UomID equals u.ID
                            join b in _context.Branches on r.BranchID equals b.ID
                            join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                            join curr in _context.Currency on r.PLCurrencyID equals curr.ID
                            join LCcurr in _context.Currency on r.LocalCurrencyID equals LCcurr.ID
                            group new { r, rd, i, u, curr_sys, curr, LCcurr, g1, g2, g3 } by new { rd.ItemID, rd.UnitPrice } into g
                            let data = g.FirstOrDefault()
                            let sumByBranch = receiptsMemoFilter.Where(_r => _r.BranchID == data.r.BranchID).Sum(_as => _as.GrandTotalSys)
                            select new TopSaleQtyReport
                            {
                                GroupName = g.First().g1.Name + "/" + g.First().g2.Name + "/" + g.First().g3.Name,
                                ItemID = g.Key.ItemID,
                                Qty = g.Sum(c => c.rd.Qty) * -1,
                                ReturnQty = g.Sum(c => c.rd.Qty),
                                Uom = data.u.Name,
                                Group1 = data.g1.Name,
                                //CurrencyId = data.curr.ID,
                                BrandID = data.r.BranchID,
                                SysCurrency = data.curr_sys.Description,
                                Currency = data.curr.Description,
                                LocalCurrency = data.LCcurr.Description,
                                PriceCal = g.Sum(i => i.rd.UnitPrice) * data.r.PLRate,
                                TotalCal = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate) * -1,
                                SubTotalCal = (data.rd.Qty * data.rd.UnitPrice * data.r.PLRate) * -1,
                                ////Summary
                                SumGrandTotalSys = data.r.GrandTotalSys * -1,
                                SDiscountItemCal = g.Sum(c => c.rd.DisValue * data.r.PLRate) * -1,
                                SDiscountTotalCal = g.Sum(c => c.r.DisValue * data.r.PLRate) * -1,
                                SDiscountItemCallocal = g.Sum(c => c.rd.DisValue * data.r.LocalSetRate) * -1,
                                SVatCal = (data.r.TaxValue * data.r.ExchangeRate) * -1,
                                LocalSetRate = data.r.LocalSetRate,
                                SGrandTotalSys = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate) * -1,
                                SGrandTotal = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate * data.r.LocalSetRate) * -1,
                            }).ToList();
            #endregion
            if (DateFrom != null && DateTo != null && BranchID == 0)
            {
                saleARFilter = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                saleAREditesFilter = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                SaleCreditMemoFilter = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0)
            {
                saleARFilter = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
                saleAREditesFilter = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
                SaleCreditMemoFilter = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0)
            {
                saleARFilter = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
                saleAREditesFilter = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
                SaleCreditMemoFilter = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom == null && DateTo == null && BranchID != 0)
            {
                saleARFilter = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
                saleAREditesFilter = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
                SaleCreditMemoFilter = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom == null && DateTo == null && BranchID == 0)
            {
                saleARFilter = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID).ToList();
                saleAREditesFilter = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID).ToList();
                SaleCreditMemoFilter = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID).ToList();
            }
            TotalDis += saleARFilter.Sum(s => s.DisValue);
            TotalDis += saleAREditesFilter.Sum(s => s.DisValue);
            TotalDis -= SaleCreditMemoFilter.Sum(s => s.DisValue);
            TotalDisCal += saleARFilter.Sum(s => s.DisValue * s.LocalSetRate);
            TotalDisCal += saleAREditesFilter.Sum(s => s.DisValue * s.LocalSetRate);
            TotalDisCal -= SaleCreditMemoFilter.Sum(s => s.DisValue * s.LocalSetRate);
            var listAR = (from ar in saleARFilter
                          join ard in _context.SaleARDetails on ar.SARID equals ard.SARID
                          join i in _context.ItemMasterDatas on ard.ItemID equals i.ID
                          join g1 in _context.ItemGroup1 on i.ItemGroup1ID equals g1.ItemG1ID
                          join g2 in _context.ItemGroup2 on i.ItemGroup2ID equals g2.ItemG2ID
                          join g3 in _context.ItemGroup3 on i.ItemGroup3ID equals g3.ID
                          join uom in _context.UnitofMeasures on ard.UomID equals uom.ID
                          join br in _context.Branches on ar.BranchID equals br.ID
                          join curr_sys in _context.Currency on ar.SaleCurrencyID equals curr_sys.ID
                          join curr in _context.Currency on ar.PriceListID equals curr.ID
                          join LCcurr in _context.Currency on ar.LocalCurID equals LCcurr.ID
                          group new { ar, ard, i, g1, g2, g3, uom, br, curr_sys, curr, LCcurr } by new { ard.UnitPrice, ard.ItemID } into g
                          let data = g.FirstOrDefault()
                          select new TopSaleQtyReport
                          {
                              GroupName = g.First().g1.Name + "/" + g.First().g2.Name + "/" + g.First().g3,
                              ItemID = g.Key.ItemID,
                              Barcode = data.i.Barcode,
                              Code = data.i.Code,
                              KhmerName = data.i.KhmerName,
                              Uom = data.uom.Name,
                              Group1 = data.g1.Name,

                              SysCurrency = data.curr_sys.Description,
                              Currency = data.curr.Description,
                              LocalSetRate = data.ar.LocalSetRate,
                              LocalCurrency = data.LCcurr.Description,
                              BrandID = data.ar.BranchID,
                              Qty = g.Sum(c => c.ard.Qty * data.ar.ExchangeRate),
                              PriceCal = data.ard.UnitPrice * data.ar.ExchangeRate,
                              TotalCal = g.Sum(s => s.ard.Qty * data.ard.UnitPrice * data.ar.ExchangeRate),
                              SubTotalCal = data.ard.Qty * data.ard.UnitPrice * data.ar.ExchangeRate,
                              DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                              DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                              // summary
                              SumGrandTotalSys = data.ar.TotalAmountSys,
                              SDiscountItemCal = g.Sum(s => s.ard.DisValue * data.ar.ExchangeRate),
                              SDiscountTotalCal = g.Sum(s => s.ar.DisValue * data.ar.ExchangeRate),
                              SDiscountItemCallocal = g.Sum(s => s.ard.DisValue * data.ar.LocalSetRate),
                              SVatCal = data.ar.VatValue,
                              SGrandTotalSys = g.Sum(s => s.ard.Qty * data.ard.UnitPrice * data.ar.ExchangeRate),
                              SGrandTotal = g.Sum(s => s.ard.Qty * data.ard.UnitPrice * data.ar.ExchangeRate) * data.ar.LocalSetRate,
                          }).ToList();
            var listARE = (from are in saleAREditesFilter
                           join ared in _context.SaleAREditeDetails on are.SARID equals ared.SARID
                           join item in _context.ItemMasterDatas on ared.ItemID equals item.ID
                           join g1 in _context.ItemGroup1 on item.ItemGroup1ID equals g1.ItemG1ID
                           join g2 in _context.ItemGroup2 on item.ItemGroup2ID equals g2.ItemG2ID
                           join g3 in _context.ItemGroup3 on item.ItemGroup3ID equals g3.ID
                           join uom in _context.UnitofMeasures on ared.UomID equals uom.ID
                           join br in _context.Branches on are.BranchID equals br.ID
                           join curr_sys in _context.Currency on are.SaleCurrencyID equals curr_sys.ID
                           join curr in _context.Currency on are.PriceListID equals curr.ID
                           join LCcurr in _context.Currency on are.LocalCurID equals LCcurr.ID
                           group new { are, ared, g1, g2, g3, uom, br, curr_sys, curr, LCcurr } by new { ared.UnitPrice, ared.ItemID } into g
                           let data = g.FirstOrDefault()
                           select new TopSaleQtyReport
                           {
                               GroupName = g.First().g1.Name + "/" + g.First().g2.Name + "/" + g.First().g3,
                               ItemID = g.Key.ItemID,
                               Qty = g.Sum(x => x.ared.Qty),
                               KhmerName = data.ared.ItemNameKH,
                               Code = data.ared.ItemCode,
                               Uom = data.uom.Name,
                               Group1 = data.g1.Name,

                               SysCurrency = data.curr_sys.Description,
                               LocalCurrency = data.LCcurr.Description,
                               Currency = data.curr.Description,
                               BrandID = data.are.BranchID,
                               PriceCal = g.Sum(s => s.ared.UnitPrice * data.are.ExchangeRate),
                               TotalCal = g.Sum(s => s.ared.Qty * data.ared.UnitPrice * data.are.ExchangeRate),
                               SubTotalCal = data.ared.Qty * data.ared.UnitPrice * data.are.ExchangeRate,
                               LocalSetRate = data.are.LocalSetRate,
                               // summary
                               SumGrandTotalSys = data.are.TotalAmountSys,
                               SDiscountItemCal = g.Sum(s => s.ared.DisValue * data.are.ExchangeRate),
                               SDiscountTotalCal = g.Sum(s => s.are.DisValue * data.are.ExchangeRate),
                               SDiscountItemCallocal = g.Sum(s => s.ared.DisValue * data.are.LocalSetRate),
                               SVatCal = data.are.VatValue,
                               SGrandTotalSys = g.Sum(s => s.ared.Qty * data.ared.UnitPrice * data.are.ExchangeRate),
                               SGrandTotal = g.Sum(s => s.ared.Qty * data.ared.UnitPrice * data.are.ExchangeRate) * data.are.LocalSetRate,
                           }).ToList();
            var listARMemo = (from armemo in SaleCreditMemoFilter
                              join armed in _context.SaleCreditMemoDetails on armemo.SCMOID equals armed.SCMOID
                              join item in _context.ItemMasterDatas on armed.ItemID equals item.ID
                              join g1 in _context.ItemGroup1 on item.ItemGroup1ID equals g1.ItemG1ID
                              join g2 in _context.ItemGroup2 on item.ItemGroup2ID equals g2.ItemG2ID
                              join g3 in _context.ItemGroup3 on item.ItemGroup3ID equals g3.ID
                              join uom in _context.UnitofMeasures on armed.UomID equals uom.ID
                              join br in _context.Branches on armemo.BranchID equals br.ID
                              join curr_sys in _context.Currency on armemo.SaleCurrencyID equals curr_sys.ID
                              join curr in _context.Currency on armemo.PriceListID equals curr.ID
                              join LCcurr in _context.Currency on armemo.LocalCurID equals LCcurr.ID
                              group new { armed, armemo, item, g1, g2, g3, uom, br, curr, curr_sys, LCcurr } by new { armed.ItemID, armed.UnitPrice } into g
                              let data = g.FirstOrDefault()
                              select new TopSaleQtyReport
                              {
                                  GroupName = g.First().g1.Name + "/" + g.First().g2.Name + "/" + g.First().g3.Name,
                                  ItemID = g.Key.ItemID,
                                  Qty = g.Sum(c => c.armed.Qty) * -1,
                                  ReturnQty = g.Sum(c => c.armed.Qty),
                                  Uom = data.uom.Name,
                                  Code = data.item.Code,
                                  KhmerName = data.item.KhmerName,
                                  Barcode = data.item.Barcode,
                                  Group1 = data.g1.Name,
                                  //CurrencyId = data.curr.ID,
                                  BrandID = data.armemo.BranchID,
                                  SysCurrency = data.curr_sys.Description,
                                  Currency = data.curr.Description,
                                  LocalCurrency = data.LCcurr.Description,
                                  PriceCal = g.Sum(i => i.armed.UnitPrice),
                                  TotalCal = g.Sum(c => c.armed.Qty * data.armed.UnitPrice * data.armemo.ExchangeRate) * -1,
                                  SubTotalCal = (data.armed.Qty * data.armed.UnitPrice * data.armemo.ExchangeRate) * -1,
                                  ////Summary
                                  SumGrandTotalSys = data.armemo.TotalAmountSys,
                                  SDiscountItemCal = g.Sum(c => c.armed.DisValue * data.armemo.ExchangeRate) * -1,
                                  SDiscountTotalCal = g.Sum(c => c.armemo.DisValue * data.armemo.ExchangeRate) * -1,
                                  SDiscountItemCallocal = g.Sum(c => c.armed.DisValue * data.armemo.LocalSetRate) * -1,
                                  SVatCal = (data.armemo.VatValue * data.armemo.ExchangeRate) * -1,
                                  LocalSetRate = data.armemo.LocalSetRate,
                                  SGrandTotalSys = g.Sum(c => c.armed.Qty * data.armed.UnitPrice * data.armemo.ExchangeRate) * -1,
                                  SGrandTotal = g.Sum(c => c.armed.Qty * data.armed.UnitPrice * data.armemo.ExchangeRate * data.armemo.LocalSetRate) * -1,
                              }).ToList();
            #region bind receipt and receipt memo
            List<TopSaleQtyReport> topSaleQty = new(list.Count + listMemo.Count + listAR.Count + listARE.Count + listARMemo.Count);
            topSaleQty.AddRange(list);
            topSaleQty.AddRange(listMemo);
            topSaleQty.AddRange(listAR);
            topSaleQty.AddRange(listARE);
            topSaleQty.AddRange(listARMemo);
            var allDatas = from all in topSaleQty
                           select new
                           {
                               Total = all.PriceCal * all.Qty,
                           };
            var sumGrandTotalSys = allDatas.Sum(s => s.Total);
            var allData = (from all in topSaleQty
                           group new { all } by new { all.ItemID, all.PriceCal } into g
                           let data = g.FirstOrDefault()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.all.CurrencyId) ?? new Display()
                           select new
                           {
                               GroupName = data.all.GroupName,
                               Barcode = data.all.Barcode,
                               ItemID = g.Key.ItemID,
                               Code = data.all.Code,
                               Currency = data.all.Currency,
                               SysCurrency = data.all.SysCurrency,
                               LocalCurrency = data.all.LocalCurrency,
                               CurrencyId = data.all.CurrencyId,
                               DateFrom = data.all.DateFrom,
                               DateTo = data.all.DateTo,
                               Group1 = data.all.Group1,
                               LocalSetRate = data.all.LocalSetRate,
                               ExchangeRate = data.all.ExchangeRate,
                               KhmerName = data.all.KhmerName,
                               Price = _fncModule.ToCurrency(data.all.PriceCal, plCur.Amounts),
                               PriceCal = _fncModule.ToCurrency(data.all.PriceCal, plCur.Amounts),
                               SubTotalCal = _fncModule.ToCurrency(data.all.PriceCal * g.Sum(s => s.all.Qty), plCur.Amounts),
                               Uom = data.all.Uom,
                               Qty = g.Where(i => i.all.Qty > 0).Sum(i => i.all.Qty),
                               ReturnQty = g.Sum(s => s.all.ReturnQty),
                               TotalQty = g.Sum(s => s.all.Qty),
                               SumGrandTotalSys = _fncModule.ToCurrency(data.all.SumGrandTotalSys, plCur.Amounts),
                               Total = _fncModule.ToCurrency((data.all.PriceCal * g.Sum(s => s.all.Qty)), plCur.Amounts),
                               SubTotal = $"{data.all.Currency:C3} {_fncModule.ToCurrency(topSaleQty.Sum(i => i.SubTotalCal), plCur.Amounts)}",
                               //GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                               //summary
                               SDiscountItem = $"{data.all.Currency} {_fncModule.ToCurrency(topSaleQty.Sum(i => i.SDiscountItemCal), plCur.Amounts)}",
                               SDiscountTotal = $"{data.all.Currency} {_fncModule.ToCurrency(TotalDis, plCur.Amounts)}",
                               //    SVat = $"{data.all.Currency} {_fncModule.ToCurrency(TotalVat, plCur.Amounts)}",
                               SGrandTotalSysCal = $"{data.all.Currency} {_fncModule.ToCurrency((topSaleQty.Sum(x => x.SGrandTotalSys)) - (topSaleQty.Sum(i => i.SDiscountItemCal)) - (TotalDis), plCur.Amounts)}",
                               //    SGrandTotalCal = _fncModule.ToCurrency(topSaleQty.Sum(x => x.SGrandTotal), plCur.Amounts),
                               SGrandTotalCal = $"{data.all.LocalCurrency} {_fncModule.ToCurrency((topSaleQty.Sum(x => x.SGrandTotal)) - (topSaleQty.Sum(i => i.SDiscountItemCallocal)) - (TotalDisCal), plCur.Amounts)}",
                           }).ToList();
            #endregion
            return Ok(allData.OrderByDescending(o => o.Qty));
        }
        public IActionResult GetPaymentMean()
        {
            var data = _context.PaymentMeans.ToList();
            return Ok(data);
        }
        public IActionResult PaymentMeansReport(string dateFrom, string dateTo, int branchId, int userId, int paymentId)
        {
            List<Receipt> receiptsFilter = new();
            DateTime _dateFrom = Convert.ToDateTime(dateFrom);
            DateTime _dateTo = Convert.ToDateTime(dateTo);

            if (dateFrom != null && dateTo != null && branchId == 0 && userId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId != 0 && userId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId != 0 && userId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId && w.UserOrderID == userId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId != 0 && userId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId && w.UserOrderID == userId && w.PaymentMeansID == paymentId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId != 0 && userId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId && w.PaymentMeansID == paymentId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId == 0 && userId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.PaymentMeansID == paymentId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId == 0 && userId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.PaymentMeansID == paymentId && w.UserOrderID == userId).ToList();
            }
            else
            {
                return Ok(new List<Receipt>());
            }
            var Summary = GetSummaryTotals(dateFrom, dateTo, branchId, userId, "", "");
            if (Summary != null)
            {

                var lcCur = _context.Displays.FirstOrDefault(s => s.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var paymentMeans = paymentId == 0 ? _context.MultiPaymentMeans.ToList()
                                : paymentId == -1 ? _context.MultiPaymentMeans.Where(s => s.Type == PaymentMeanType.CardMember).ToList()
                                : _context.MultiPaymentMeans.Where(s => s.PaymentMeanID == paymentId).ToList();
                var list = (from multipay in paymentMeans
                            join receipts in receiptsFilter on multipay.ReceiptID equals receipts.ReceiptID
                            join curr_pl in _context.Currency on receipts.PLCurrencyID equals curr_pl.ID
                            join curr in _context.Currency on receipts.LocalCurrencyID equals curr.ID
                            join curr_sys in _context.Currency on receipts.SysCurrencyID equals curr_sys.ID
                            join cus in _context.BusinessPartners on receipts.CustomerID equals cus.ID
                            join useracount in _context.UserAccounts on receipts.UserOrderID equals useracount.ID
                            join em in _context.Employees on useracount.EmployeeID equals em.ID
                            join cusm in _context.BusinessPartners on receipts.CustomerID equals cusm.ID
                            join e in _context.Employees on useracount.EmployeeID equals e.ID

                            let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curr_sys.ID) ?? new Display()
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curr_pl.ID) ?? new Display()
                            select new
                            {
                                PaymentMean = multipay.Type == PaymentMeanType.Normal ? _context.PaymentMeans.FirstOrDefault(s => s.ID == multipay.PaymentMeanID).Type : "Card Member",
                                ReceiptNo = receipts.ReceiptNo,
                                Customer = cusm.Name,
                                UserName = e.Name,
                                DateIn = Convert.ToDateTime(receipts.DateIn).ToString("dd-MM-yyyy"),
                                TimeIn = receipts.TimeIn,
                                DateOut = Convert.ToDateTime(receipts.DateOut).ToString("dd-MM-yyyy"),
                                TimeOut = receipts.TimeOut,
                                Multipayment = multipay.Amount < 0 ? multipay.PLCurrency + " " + _fncModule.ToCurrency(multipay.Amount, plCur.Amounts) : multipay.AltCurrency + " " + _fncModule.ToCurrency(multipay.Amount, plCur.Amounts),
                                MultipaymentSys = multipay.Amount < 0 ? multipay.PLCurrency + " " + _fncModule.ToCurrency(multipay.Amount, plCur.Amounts)
                                : multipay.AltCurrencyID == 2 ? multipay.PLCurrency + " " + _fncModule.ToCurrency(multipay.Amount / multipay.AltRate, plCur.Amounts)
                                : multipay.AltCurrencyID == 3 ? multipay.PLCurrency + " " + _fncModule.ToCurrency(multipay.Amount / multipay.AltRate, plCur.Amounts)
                                : multipay.AltCurrencyID == 4 ? multipay.PLCurrency + " " + _fncModule.ToCurrency(multipay.Amount / multipay.AltRate, plCur.Amounts)
                                : multipay.AltCurrencyID == 5 ? multipay.PLCurrency + " " + _fncModule.ToCurrency(multipay.Amount / multipay.AltRate, plCur.Amounts)
                                : multipay.AltCurrency + " " + _fncModule.ToCurrency(multipay.Amount, plCur.Amounts),

                                //Last Summary
                                SDiscountItem = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                                SDiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                                SVat = curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().TaxValue, sysCur.Amounts),
                                SGrandTotalSys = curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotalSys, sysCur.Amounts),
                                SGrandTotal = curr.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotal, lcCur.Amounts),
                            }).ToList();
                return Ok(list);
            }
            return Ok(new List<Receipt>());

        }

        public async Task<IActionResult> SaleByCustomerReport(string dateFrom, string dateTo, int branchId, int cusId)
        {
            var _data = await _report.GetSaleByCustomers(dateFrom, dateTo, branchId, cusId, GetCompany().LocalCurrencyID);
            return Ok(_data);
        }

        public IActionResult TaxDeclarationReport(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<Receipt> receiptsFilter = new();
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
            var Summary = GetSummaryTotals(DateFrom, DateTo, BranchID, UserID, "", "");
            if (Summary != null)
            {
                var Receipts = receiptsFilter;
                var Sale = from r in Receipts
                           join user in _context.UserAccounts on r.UserOrderID equals user.ID
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                           join ssc in _context.Currency on r.SysCurrencyID equals ssc.ID
                           join lc in _context.Currency on r.LocalCurrencyID equals lc.ID
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curr.ID) ?? new Display()
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == ssc.ID) ?? new Display()
                           select new
                           {
                               //Detail
                               EmpCode = emp.Code,
                               EmpName = emp.Name,
                               r.ReceiptNo,
                               DateIn = r.DateIn.ToString("dd-MM-yyyy"),
                               r.TimeIn,
                               DateOut = r.DateOut.ToString("dd-MM-yyyy"),
                               r.TimeOut,
                               GrandTotal = lc.Description + " " + _fncModule.ToCurrency(r.GrandTotal * r.LocalSetRate, plCur.Amounts),
                               TotalSys = ssc.Description + " " + _fncModule.ToCurrency(r.GrandTotal_Sys, sysCur.Amounts),
                               Tax = lc.Description + " " + _fncModule.ToCurrency(r.TaxValue * r.PLRate * r.LocalSetRate, plCur.Amounts),
                               Currency = curr.Description,
                               SysCurrency = ssc.Description,
                               //Summary
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               SCount = string.Format("{0:n0}", Summary.FirstOrDefault().CountReceipt),
                               SDiscountItem = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                               SDiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                               SVat = ssc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().TaxValue, sysCur.Amounts),
                               SGrandTotalSys = ssc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotalSys, sysCur.Amounts),
                               SGrandTotal = lc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotal, plCur.Amounts),
                           };
                return Ok(Sale);
            }
            return Ok(new List<Receipt>());
        }
        public IActionResult VoidOrderReport(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<VoidOrder> voidOrders = new();
            if (DateFrom != null && BranchID == 0 && UserID == 0)
            {
                voidOrders = _context.VoidOrders.Include(x => x.VoidOrderDetail).Where(w => w.DateIn >= Convert.ToDateTime(DateFrom) && w.DateIn <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && BranchID != 0 && UserID == 0)
            {
                voidOrders = _context.VoidOrders.Include(x => x.VoidOrderDetail).Where(w => w.DateIn >= Convert.ToDateTime(DateFrom) && w.DateIn <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && BranchID != 0 && UserID != 0)
            {
                voidOrders = _context.VoidOrders.Include(x => x.VoidOrderDetail).Where(w => w.DateIn >= Convert.ToDateTime(DateFrom) && w.DateIn <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else
            {
                return Ok(new List<VoidOrder>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }

            var SCount = voidOrders.Count;
            //var SSoldAmount = voidOrders.Sum(x => x.VoidOrderDetail.FirstOrDefault().UnitPrice * x.VoidOrderDetail.FirstOrDefault().Qty * x.PLRate);
            var SSoldAmount = (from vo in voidOrders
                               join vd in _context.VoidOrderDetails on vo.OrderID equals vd.OrderID
                               select new
                               {
                                   SSoldAmount = vd.UnitPrice * vd.Qty * vo.PLRate
                               }).ToList();
            var SoldAmount = SSoldAmount.Sum(x => x.SSoldAmount);
            var SDiscountItem = voidOrders.Sum(x => x.VoidOrderDetail.First().DiscountValue * x.PLRate);
            var SDiscountTotal = voidOrders.Sum(x => x.DiscountValue * x.PLRate);
            var SVat = voidOrders.Sum(x => x.TaxValue * x.PLRate);
            var SGrandTotal = voidOrders.Sum(x => x.GrandTotal_Sys);
            var vOrder = voidOrders;
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
            var Sale = from vo in vOrder
                       join vod in _context.VoidOrderDetails on vo.OrderID equals vod.OrderID
                       join user in _context.UserAccounts on vo.UserOrderID equals user.ID
                       join emp in _context.BusinessPartners on vo.CustomerID equals emp.ID
                       join Uom in _context.UnitofMeasures on vod.UomID equals Uom.ID
                       join plc in _context.Currency on vo.PLCurrencyID equals plc.ID
                       join ssc in _context.Currency on vo.SysCurrencyID equals ssc.ID
                       group new { vo, vod, user, emp, Uom, plc, ssc } by new { vo.OrderID, vod.OrderDetailID } into g
                       let order = g.FirstOrDefault().vo
                       let orderdetail = g.FirstOrDefault().vod
                       let emp = g.FirstOrDefault().emp
                       let Uom = g.FirstOrDefault().Uom
                       let plc = g.FirstOrDefault().plc
                       let ssc = g.FirstOrDefault().ssc
                       let user = g.FirstOrDefault().user
                       let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == plc.ID) ?? new Display()
                       let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == ssc.ID) ?? new Display()
                       select new
                       {
                           //Master
                           MReceiptID = order.OrderID,
                           MReceiptNo = order.OrderNo,
                           MUserName = emp.Name,
                           Username = user.Username,
                           MDateIn = order.DateIn.ToString("dd-MM-yyyy") + " " + order.TimeIn,
                           MVat = string.Format("{0:n3}", order.TaxValue),
                           MDisTotal = _fncModule.ToCurrency(order.DiscountValue, plCur.Amounts),
                           MSubTotal = plc.Description + " " + _fncModule.ToCurrency(order.Sub_Total, plCur.Amounts),
                           MTotal = plc.Description + " " + _fncModule.ToCurrency(order.GrandTotal, plCur.Amounts),
                           order.Reason,
                           //Detail
                           ID = orderdetail.OrderDetailID,
                           ItemCode = orderdetail.Code,
                           orderdetail.KhmerName,
                           orderdetail.Qty,
                           Uom = Uom.Name,
                           UnitPrice = _fncModule.ToCurrency(orderdetail.UnitPrice, plCur.Amounts),
                           DisItem = _fncModule.ToCurrency(orderdetail.DiscountValue, plCur.Amounts),
                           Total = _fncModule.ToCurrency(orderdetail.Total, plCur.Amounts),
                           //Summary
                           DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                           DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                           SCount = string.Format("{0:n0}", SCount),
                           SSoldAmount = _fncModule.ToCurrency(SoldAmount, sysCur.Amounts),
                           SDiscountItem = _fncModule.ToCurrency(SDiscountItem, sysCur.Amounts),
                           SDiscountTotal = _fncModule.ToCurrency(SDiscountTotal, sysCur.Amounts),
                           SGrandTotal = ssc.Description + " " + _fncModule.ToCurrency(SGrandTotal, lcCur.Amounts),
                       };
            return Ok(Sale);
        }
        public IActionResult VoidItemReport(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<VoidItem> voidItems = new();
            if (DateFrom != null && BranchID == 0 && UserID == 0)
            {
                voidItems = _context.VoidItems.Include(x => x.VoidItemDetails).Where(w => w.DateIn >= Convert.ToDateTime(DateFrom) && w.DateIn <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && BranchID != 0 && UserID == 0)
            {
                voidItems = _context.VoidItems.Include(x => x.VoidItemDetails).Where(w => w.DateIn >= Convert.ToDateTime(DateFrom) && w.DateIn <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && BranchID != 0 && UserID != 0)
            {
                voidItems = _context.VoidItems.Include(x => x.VoidItemDetails).Where(w => w.DateIn >= Convert.ToDateTime(DateFrom) && w.DateIn <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else
            {
                return Ok(new List<VoidItem>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            var SCount = voidItems.Count;
            var STotal = (from vi in voidItems
                          join vd in _context.VoidItemDetails on vi.ID equals vd.VoidItemID
                          group new { vi, vd } by new { vi.ID, VIDD = vd.ID } into datas
                          let data = datas.FirstOrDefault()
                          select new
                          {
                              SSoldAmount = data.vd.Total * data.vi.PLRate,
                              SDiscountItem = data.vd.DiscountValue * data.vi.PLRate,
                              SDiscountTotal = data.vi.DiscountValue * data.vi.PLRate,
                          }).ToList();
            var SGrandTotal = voidItems.Sum(s => s.GrandTotal);
            var vItem = voidItems;
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
            var VdItem = from vo in vItem
                         join VoidDetails in _context.VoidItemDetails on vo.ID equals VoidDetails.VoidItemID
                         join user in _context.UserAccounts on vo.UserOrderID equals user.ID
                         join emp in _context.BusinessPartners on vo.CustomerID equals emp.ID
                         join plc in _context.Currency on vo.PLCurrencyID equals plc.ID
                         join ssc in _context.Currency on vo.SysCurrencyID equals ssc.ID
                         join Uom in _context.UnitofMeasures on VoidDetails.UomID equals Uom.ID
                         group new { vo, user, emp, plc, ssc, VoidDetails, Uom } by new { vo.ID, IDD = VoidDetails.ID } into datas
                         let data = datas.FirstOrDefault()
                         let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.plc.ID) ?? new Display()
                         let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.ssc.ID) ?? new Display()
                         select new
                         {
                             //Master
                             VID = data.vo.ID,
                             MReceiptID = data.vo.OrderID,
                             MReceiptNo = data.vo.OrderNo,
                             MUserName = data.emp.Name,
                             Username = data.user.Username,
                             MDateIn = data.vo.DateIn.ToString("dd-MM-yyyy") + " " + data.vo.TimeIn,
                             MVat = _fncModule.ToCurrency(data.vo.TaxValue, plCur.Amounts),
                             MDisTotal = _fncModule.ToCurrency(data.vo.DiscountValue, plCur.Amounts),
                             MSubTotal = data.plc.Description + " " + _fncModule.ToCurrency(data.vo.Sub_Total, plCur.Amounts),
                             MTotal = data.plc.Description + " " + _fncModule.ToCurrency(data.vo.GrandTotal, plCur.Amounts),
                             data.vo.Reason,
                             //Detail
                             ID = data.VoidDetails.OrderDetailID,
                             ItemCode = data.VoidDetails.Code,
                             data.VoidDetails.KhmerName,
                             data.VoidDetails.Qty,
                             Uom = data.Uom.Name,
                             UnitPrice = _fncModule.ToCurrency(data.VoidDetails.UnitPrice, plCur.Amounts),
                             DisItem = _fncModule.ToCurrency(data.VoidDetails.DiscountValue, plCur.Amounts),
                             Total = _fncModule.ToCurrency(data.VoidDetails.Total, plCur.Amounts),
                             //Summary
                             DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                             DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                             SCount = string.Format("{0:n0}", SCount),
                             SSoldAmount = _fncModule.ToCurrency(STotal.Sum(s => s.SSoldAmount), sysCur.Amounts),
                             SDiscountItem = _fncModule.ToCurrency(STotal.Sum(s => s.SDiscountItem), sysCur.Amounts),
                             SDiscountTotal = _fncModule.ToCurrency(STotal.Sum(s => s.SDiscountTotal), sysCur.Amounts),
                             SGrandTotal = data.ssc.Description + " " + _fncModule.ToCurrency(SGrandTotal, lcCur.Amounts),
                         };
            return Ok(VdItem);
        }
        public List<SummaryTotalSale> GetSummaryTotals(string DateFrom, string DateTo, int BranchID, int UserID, string TimeFrom, string TimeTo)
        {
            try
            {
                var data = _context.SummaryTotalSale.FromSql("rp_GetSummarrySaleTotal @DateFrom={0},@DateTo={1}, @BranchID={2},@UserID={3},@TimeFrom={4},@TimeTo={5}",
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
            catch (Exception)
            {
                return null;
            }
        }
        public List<SummaryTotalSale> GetSummaryTotals(string DateFrom, string DateTo, int BranchID, int UserID, string TimeFrom, string TimeTo, int plid)
        {
            try
            {
                var data = _context.SummaryTotalSale.FromSql("rp_GetSummarrySaleTotalpric @DateFrom={0},@DateTo={1}, @BranchID={2},@UserID={3},@TimeFrom={4},@TimeTo={5},@plid={6}",
                parameters: new[] {
                    DateFrom.ToString(),
                    DateTo.ToString(),
                    BranchID.ToString(),
                    UserID.ToString(),
                    TimeFrom.ToString(),
                    TimeTo.ToString(),
                    plid.ToString()
                }).ToList();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //SaleByTableReport
        public IActionResult SaleByTableReport(string DateFrom, string DateTo, int BranchID, int UserID)
        {
            List<Receipt> receiptsFilter = new();
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
            var Summary = GetSummaryTotals(DateFrom, DateTo, BranchID, UserID, "", "");
            if (Summary != null)
            {
                var Receipts = receiptsFilter;
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var Sales = (from rd in _context.ReceiptDetail
                             join r in Receipts on rd.ReceiptID equals r.ReceiptID
                             group new { rd, r } by new { r.ReceiptID, rd.ID } into datas
                             let data = datas.FirstOrDefault()
                             select new
                             {
                                 Totals = data.rd.Total * data.r.PLRate,
                             }).ToList();
                var SubTotals = Sales.Sum(s => s.Totals);
                var Sale = from rd in _context.ReceiptDetail
                           join r in Receipts on rd.ReceiptID equals r.ReceiptID
                           join user in _context.UserAccounts on r.UserOrderID equals user.ID
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           join Uom in _context.UnitofMeasures on rd.UomID equals Uom.ID
                           join t in _context.Tables on r.TableID equals t.ID
                           join lc in _context.Currency on r.LocalCurrencyID equals lc.ID
                           join cur in _context.Currency on r.PLCurrencyID equals cur.ID
                           join ssc in _context.Currency on r.SysCurrencyID equals ssc.ID
                           group new { r, emp, rd, Uom, t, lc, ssc, cur } by new { r.ReceiptID, rd.ID } into datas
                           let data = datas.FirstOrDefault()
                           let receipt = data.r
                           let emp = data.emp
                           let receiptDetail = data.rd
                           let Uom = data.Uom
                           let cur = data.cur
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.ssc.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.cur.ID) ?? new Display()
                           select new
                           {
                               //Master
                               MReceiptID = receipt.ReceiptID,
                               MTableName = datas.FirstOrDefault().t.Name,
                               MUserName = emp.Name,
                               MDateOut = receipt.DateOut.ToString("dd-MM-yyyy") + " " + receipt.TimeOut,
                               MVat = _fncModule.ToCurrency(receipt.TaxValue * receipt.PLRate, plCur.Amounts),
                               MDisTotal = _fncModule.ToCurrency(receipt.DiscountValue * receipt.PLRate, plCur.Amounts),
                               MSubTotal = receipt.Sub_Total * receipt.PLRate,
                               SSC = datas.First().ssc.Description,
                               RefNo = receipt.RefNo,
                               //MTotal = datas.First().ssc.Description + " " + string.Format("{0:#,0.000}", receipt.GrandTotal_Sys),
                               //Detail
                               receiptDetail.ID,
                               ItemCode = receiptDetail.Code,
                               receiptDetail.KhmerName,
                               receiptDetail.Qty,
                               Uom = Uom.Name,
                               UnitPrice = _fncModule.ToCurrency(receiptDetail.UnitPrice * receipt.PLRate, sysCur.Amounts),
                               DisItem = _fncModule.ToCurrency(receiptDetail.DiscountValue * receipt.PLRate, sysCur.Amounts),
                               Total = _fncModule.ToCurrency(receiptDetail.Total * receipt.PLRate, sysCur.Amounts),
                               SubTotal = _fncModule.ToCurrency(SubTotals, sysCur.Amounts),
                               //Summary
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               SCount = string.Format("{0:#,0}", Summary.FirstOrDefault().CountReceipt),
                               SSoldAmount = _fncModule.ToCurrency(Summary.FirstOrDefault().SoldAmount, sysCur.Amounts),
                               SDiscountItem = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                               SDiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                               SVat = datas.First().ssc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().TaxValue, sysCur.Amounts),
                               SGrandTotal = datas.First().lc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotal, lcCur.Amounts),
                               SGrandTotalSys = datas.First().ssc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotalSys, sysCur.Amounts),
                           };
                return Ok(Sale);
            }
            else
            {
                return Ok(new List<Receipt>());
            }
        }

        //Purchase
        public IActionResult GetPurchaseOrder(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID, int WarehouseID)
        {
            List<PurchaseOrder> PurchaseOrders = new();

            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && WarehouseID == 0 && VendorID == 0)
            {
                PurchaseOrders = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID == 0 && VendorID == 0)
            {
                PurchaseOrders = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID == 0 && VendorID == 0)
            {
                PurchaseOrders = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID != 0 && VendorID == 0)
            {
                PurchaseOrders = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WarehouseID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID != 0 && VendorID == 0)
            {
                PurchaseOrders = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.WarehouseID == WarehouseID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID != 0 && VendorID != 0)
            {
                PurchaseOrders = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && WarehouseID == 0 && VendorID != 0)
            {
                PurchaseOrders = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID == 0 && VendorID != 0)
            {
                PurchaseOrders = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID == 0 && VendorID != 0)
            {
                PurchaseOrders = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID != 0 && VendorID != 0)
            {
                PurchaseOrders = _context.PurchaseOrders.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            else
            {
                return Ok(new List<PurchaseOrder>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var Summary = GetSummaryPurchaseTotals(DateFrom, DateTo, BranchID, UserID, VendorID, WarehouseID, "PO");
            if (Summary != null)
            {
                SystemCurrency SSC = GetSystemCurrencies().FirstOrDefault();
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var PurchaseOrder = PurchaseOrders;
                var list = (from PD in _context.PurchaseOrderDetails
                            join PO in PurchaseOrder on PD.PurchaseOrderID equals PO.PurchaseOrderID
                            join BP in _context.BusinessPartners on PO.VendorID equals BP.ID
                            join I in _context.ItemMasterDatas on PD.ItemID equals I.ID
                            join CU in _context.Currency on PO.PurCurrencyID equals CU.ID
                            join Uom in _context.UnitofMeasures on PD.UomID equals Uom.ID
                            join lc in _context.Currency on PO.LocalCurID equals lc.ID
                            group new { PD, PO, BP, I, CU, Uom, lc } by new { PO.PurchaseOrderID, PD.PurchaseOrderDetailID, PD.PurchasPrice } into datas
                            let data = datas.FirstOrDefault()
                            let PO = data.PO
                            let BP = data.BP
                            let I = data.I
                            let PurchaseOrderDetail = data.PD
                            let CU = data.CU
                            let Uom = data.Uom
                            let lc = data.lc
                            let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == SSC.ID) ?? new Display()
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CU.ID) ?? new Display()
                            select new
                            {
                                //Master
                                PO.InvoiceNo,
                                BP.Name,
                                PostingDate = PO.PostingDate.ToString("dd-MM-yyyy"),
                                Discount = _fncModule.ToCurrency(PO.DiscountValue, plCur.Amounts),
                                Applied_Amount = _fncModule.ToCurrency(PO.AppliedAmount, plCur.Amounts),
                                Balance_Due = _fncModule.ToCurrency(PO.BalanceDue, plCur.Amounts),
                                Sub_Total = _fncModule.ToCurrency(PO.SubTotal, plCur.Amounts),
                                PLC = CU.Description,
                                //Detail
                                I.Code,
                                I.KhmerName,
                                Uom = Uom.Name,
                                PurchaseOrderDetail.Qty,
                                UnitPrice = _fncModule.ToCurrency(PurchaseOrderDetail.PurchasPrice, plCur.Amounts),
                                DisItem = _fncModule.ToCurrency(PurchaseOrderDetail.DiscountValue, plCur.Amounts),
                                Total = _fncModule.ToCurrency(PurchaseOrderDetail.Total, plCur.Amounts),
                                DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                //Summary
                                CountInvoiceNo = Summary.FirstOrDefault().CountReceipt,
                                DisItems = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                                DiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                                GrandTotalSSC = SSC.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().BalanceDueSSC, sysCur.Amounts),
                                GrandTotal = lc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().BalanceDue, lcCur.Amounts),
                            }).ToList();
                return Ok(list);
            }
            else
            {
                return Ok(new List<PurchaseOrder>());
            }
        }

        //GetGoodsReceiptPO
        [HttpGet]
        public IActionResult GetGoodsReceiptPO(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID, int WarehouseID)
        {
            List<GoodsReciptPO> GoodsReciptPOs = new();

            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && WarehouseID == 0 && VendorID == 0)
            {
                GoodsReciptPOs = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID == 0 && VendorID == 0)
            {
                GoodsReciptPOs = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID == 0 && VendorID == 0)
            {
                GoodsReciptPOs = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID != 0 && VendorID == 0)
            {
                GoodsReciptPOs = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WarehouseID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID != 0 && VendorID == 0)
            {
                GoodsReciptPOs = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.WarehouseID == WarehouseID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID != 0 && VendorID != 0)
            {
                GoodsReciptPOs = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && WarehouseID == 0 && VendorID != 0)
            {
                GoodsReciptPOs = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID == 0 && VendorID != 0)
            {
                GoodsReciptPOs = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID == 0 && VendorID != 0)
            {
                GoodsReciptPOs = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID != 0 && VendorID != 0)
            {
                GoodsReciptPOs = _context.GoodsReciptPOs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            else
            {
                return Ok(new List<GoodsReciptPO>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var Summary = GetSummaryPurchaseTotals(DateFrom, DateTo, BranchID, UserID, VendorID, WarehouseID, "GRPO");
            if (Summary != null)
            {
                SystemCurrency SSC = GetSystemCurrencies().FirstOrDefault();
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var GoodsReciptPO = GoodsReciptPOs;
                var list = from GRPOD in _context.GoodReciptPODetails
                           join GRPO in GoodsReciptPO on GRPOD.GoodsReciptPOID equals GRPO.ID
                           join BP in _context.BusinessPartners on GRPO.VendorID equals BP.ID
                           join I in _context.ItemMasterDatas on GRPOD.ItemID equals I.ID
                           join CU in _context.Currency on GRPO.PurCurrencyID equals CU.ID
                           join Uom in _context.UnitofMeasures on GRPOD.UomID equals Uom.ID
                           join lc in _context.Currency on GRPO.LocalCurID equals lc.ID
                           group new { GRPOD, GRPO, BP, I, CU, Uom, lc } by new { GRPO.ID, GRPOD.ItemID, GRPOD.PurchasPrice } into datas
                           let data = datas.FirstOrDefault()
                           let GRPOD = data.GRPOD
                           let GRPO = data.GRPO
                           let BP = data.BP
                           let I = data.I
                           let CU = data.CU
                           let Uom = data.Uom
                           let lc = data.lc
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == SSC.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CU.ID) ?? new Display()
                           select new
                           {
                               //Master
                               GRPO.InvoiceNo,
                               BP.Name,
                               PostingDate = GRPO.PostingDate.ToString("dd-MM-yyyy"),
                               Discount = _fncModule.ToCurrency(GRPO.DiscountValue, plCur.Amounts),
                               Applied_Amount = _fncModule.ToCurrency(GRPO.AppliedAmount, plCur.Amounts),
                               Balance_Due = _fncModule.ToCurrency(GRPO.BalanceDue, plCur.Amounts),
                               Sub_Total = _fncModule.ToCurrency(GRPO.SubTotal, plCur.Amounts),
                               PLC = CU.Description,
                               //Detail
                               I.Code,
                               I.KhmerName,
                               Uom = Uom.Name,
                               GRPOD.Qty,
                               UnitPrice = _fncModule.ToCurrency(GRPOD.PurchasPrice, plCur.Amounts),
                               DisItem = _fncModule.ToCurrency(GRPOD.DiscountValue, plCur.Amounts),
                               Total = _fncModule.ToCurrency(GRPOD.Total, plCur.Amounts),
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               //Summary
                               CountInvoiceNo = Summary.FirstOrDefault().CountReceipt,
                               DisItems = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                               DiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                               GrandTotalSSC = SSC.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().BalanceDueSSC, sysCur.Amounts),
                               GrandTotal = lc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().BalanceDue, lcCur.Amounts),
                           };
                return Ok(list);
            }
            else
            {
                return Ok(new List<GoodsReciptPO>());
            }
        }

        // GetPurchaseAP
        [HttpGet]
        public IActionResult GetPurchaseAP(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID, int WarehouseID)
        {
            List<Purchase_AP> Purchase_APs = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && WarehouseID == 0 && VendorID == 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID == 0 && VendorID == 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID == 0 && VendorID == 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID != 0 && VendorID == 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WarehouseID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID != 0 && VendorID == 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.WarehouseID == WarehouseID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID != 0 && VendorID != 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && WarehouseID == 0 && VendorID != 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID == 0 && VendorID != 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID == 0 && VendorID != 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID != 0 && VendorID != 0)
            {
                Purchase_APs = _context.Purchase_APs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
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

            var Summary = GetSummaryPurchaseTotals(DateFrom, DateTo, BranchID, UserID, VendorID, WarehouseID, "PU");
            if (Summary != null)
            {
                SystemCurrency SSC = GetSystemCurrencies().FirstOrDefault();
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var Purchase_AP = Purchase_APs;
                var list = from APD in _context.PurchaseAPDetail
                           join AP in Purchase_AP on APD.PurchaseAPID equals AP.PurchaseAPID
                           join BP in _context.BusinessPartners on AP.VendorID equals BP.ID
                           join I in _context.ItemMasterDatas on APD.ItemID equals I.ID
                           join CU in _context.Currency on AP.PurCurrencyID equals CU.ID
                           join Uom in _context.UnitofMeasures on APD.UomID equals Uom.ID
                           join lc in _context.Currency on AP.LocalCurID equals lc.ID
                           group new { APD, AP, BP, I, CU, Uom, lc } by new { AP.PurchaseAPID, APD.PurchaseDetailAPID, APD.PurchasPrice } into datas
                           let data = datas.FirstOrDefault()
                           let PurchaseAP = data.AP
                           let BP = data.BP
                           let I = data.I
                           let DetailPurchaseAP = data.APD
                           let CU = data.CU
                           let Uom = data.Uom
                           let lc = data.lc
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == SSC.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CU.ID) ?? new Display()
                           select new
                           {
                               ///Master
                               PurchaseAP.InvoiceNo,
                               BP.Name,
                               PostingDate = PurchaseAP.PostingDate.ToString("dd-MM-yyyy"),
                               Discount = _fncModule.ToCurrency(PurchaseAP.DiscountValue, plCur.Amounts),
                               Applied_Amount = _fncModule.ToCurrency(PurchaseAP.AppliedAmount, plCur.Amounts),
                               Balance_Due = _fncModule.ToCurrency(PurchaseAP.BalanceDue, plCur.Amounts),
                               Sub_Total = _fncModule.ToCurrency(PurchaseAP.SubTotal, plCur.Amounts),
                               PLC = CU.Description,
                               //Detail
                               I.Code,
                               I.KhmerName,
                               Uom = Uom.Name,
                               DetailPurchaseAP.Qty,
                               UnitPrice = _fncModule.ToCurrency(DetailPurchaseAP.PurchasPrice, plCur.Amounts),
                               DisItem = _fncModule.ToCurrency(DetailPurchaseAP.DiscountValue, plCur.Amounts),
                               Total = _fncModule.ToCurrency(DetailPurchaseAP.Total, plCur.Amounts),
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               // Summary
                               CountInvoiceNo = Summary.FirstOrDefault().CountReceipt,
                               DisItems = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                               DiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                               GrandTotalSSC = SSC.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().BalanceDueSSC, sysCur.Amounts),
                               GrandTotal = lc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().BalanceDue, lcCur.Amounts),

                           };

                return Ok(list);
            }
            else
            {
                return Ok(new List<Purchase_AP>());
            }
        }

        public IActionResult DetailSaleReport(string DateFrom, string DateTo, int BranchID, int UserID)
        {

            List<Receipt> receiptsFilter = new();
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
            var Summary = GetSummaryTotals(DateFrom, DateTo, BranchID, UserID, "", "");
            if (Summary != null)
            {
                var Receipts = receiptsFilter;
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var Sale = from rd in _context.ReceiptDetail
                           join r in Receipts on rd.ReceiptID equals r.ReceiptID

                           join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                           join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                           join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                           join user in _context.UserAccounts on r.UserOrderID equals user.ID
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           join Uom in _context.UnitofMeasures on rd.UomID equals Uom.ID
                           group new { r, emp, rd, Uom, curr_pl, curr, curr_sys } by new { r.ReceiptID, rd.ID } into datas

                           let data = datas.FirstOrDefault()
                           let receipt = data.r
                           let emp = data.emp
                           let receiptDetail = data.rd
                           let Uom = data.Uom
                           let curr_pl = data.curr_pl
                           let curr = data.curr
                           let curr_sys = data.curr_sys
                           //let remarkDisM = _context.RemarkDiscounts.FirstOrDefault(i=> i.ID == data.r.RemarkDiscountID) ?? new RemarkDiscountItem()
                           let remarkDisD = _context.RemarkDiscounts.FirstOrDefault(i => i.ID == data.rd.RemarkDiscountID) ?? new RemarkDiscountItem()
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                           select new
                           {
                               //Master
                               MReceiptID = receipt.ReceiptID,
                               MReceiptNo = receipt.ReceiptNo,
                               MUserName = emp.Name,
                               MDateOut = receipt.DateOut.ToString("dd-MM-yyyy") + " " + receipt.TimeOut,
                               MVat = _fncModule.ToCurrency(receipt.TaxValue, plCur.Amounts),
                               MDisTotal = _fncModule.ToCurrency(receipt.DiscountValue, plCur.Amounts),
                               MSubTotal = data.curr_pl.Description + " " + _fncModule.ToCurrency(receipt.Sub_Total, plCur.Amounts),
                               MTotal = data.curr_pl.Description + " " + _fncModule.ToCurrency(receipt.GrandTotal, plCur.Amounts),
                               MRemarkDiscount = data.r.RemarkDiscount,
                               //Detail
                               receiptDetail.ID,
                               AmountFreight = _fncModule.ToCurrency(receipt.AmountFreight, plCur.Amounts),
                               ItemCode = receiptDetail.Code,
                               receiptDetail.KhmerName,
                               receiptDetail.EnglishName,
                               receiptDetail.Qty,
                               Uom = Uom.Name,
                               DRemarkDiscount = remarkDisD.Remark,
                               UnitPrice = _fncModule.ToCurrency(receiptDetail.UnitPrice, plCur.Amounts),
                               DisItem = _fncModule.ToCurrency(receiptDetail.DiscountValue, plCur.Amounts),
                               Total = _fncModule.ToCurrency(receiptDetail.Total, plCur.Amounts),
                               //Summary
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               SCount = string.Format("{0:n0}", Summary.FirstOrDefault().CountReceipt),
                               SDiscountItem = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                               SDiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                               SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().TaxValue, sysCur.Amounts),
                               SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotalSys, sysCur.Amounts),
                               SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotal, lcCur.Amounts),
                           };
                return Ok(Sale);
            }
            else
            {
                return Ok(new List<Receipt>());
            }
        }

        public async Task<IActionResult> SummarySaleReport(string DateFrom, string DateTo, string TimeFrom, string TimeTo, int BranchID, int UserID, string DouType, int plid)
        {
            TimeSpan _timeFrom = Convert.ToDateTime(TimeFrom).TimeOfDay;
            TimeSpan _timeTo = Convert.ToDateTime(TimeTo).TimeOfDay;
            DateTime _dateFrom = _fncModule.ConcatDateTime(DateFrom, TimeFrom);
            DateTime _dateTo = _fncModule.ConcatDateTime(DateTo, TimeTo);

            IQueryable<Receipt> _receipts = _context.Receipt.Where(r =>
                _fncModule.IsBetweenDate(_dateFrom, _dateTo, r.DateOut));
            if (_timeFrom.Minutes > 0 || _timeTo.Minutes > 0)
            {
                _receipts = _context.Receipt.Where(r =>
                    _fncModule.IsBetweenDate(_dateFrom, _dateTo,
                        _fncModule.ConcatDateTime(r.DateOut.ToString(), r.TimeOut)
                ));
            }

            if (BranchID > 0) { _receipts = _receipts.Where(r => r.BranchID == BranchID); }
            if (UserID > 0) { _receipts = _receipts.Where(r => r.UserOrderID == UserID); }
            if (plid > 0) { _receipts = _receipts.Where(r => r.PriceListID == plid); }

            List<DevSummarySale> Sale = new();
            List<Receipt> receipts = new();
            List<SaleAR> saleARs = new();
            receipts = await _receipts.ToListAsync();

            List<SummaryTotalSale> Summary = new();
            if (plid > 0) Summary = GetSummaryTotals(DateFrom, DateTo, BranchID, UserID, TimeFrom, TimeTo, plid) ?? new List<SummaryTotalSale>();
            if (plid <= 0) Summary = GetSummaryTotals(DateFrom, DateTo, BranchID, UserID, TimeFrom, TimeTo) ?? new List<SummaryTotalSale>();

            Sale = (from r in receipts
                    join user in _context.UserAccounts on r.UserOrderID equals user.ID
                    join emp in _context.Employees on user.EmployeeID equals emp.ID
                    join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                    join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                    join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                    join b in _context.Branches on r.BranchID equals b.ID
                    group new { r, emp, curr_pl, curr_sys, curr, b } by new { r.BranchID, r.ReceiptID } into datas
                    let data = datas.FirstOrDefault()
                    let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "SP")
                    let sumByBranch = receipts.Where(_r => _r.BranchID == data.r.BranchID).Sum(_as => _as.GrandTotal_Sys)
                    let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                    let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                    let _summary = Summary.FirstOrDefault() ?? new SummaryTotalSale()
                    select new DevSummarySale
                    {
                        //detail
                        ReceiptID = data.r.SeriesDID,
                        AmountFreight = _fncModule.ToCurrency(data.r.AmountFreight, plCur.Amounts),
                        DouType = douType.Code,
                        EmpCode = data.emp.Code,
                        EmpName = data.emp.Name,
                        BranchID = data.r.BranchID,
                        BranchName = data.b.Name,
                        ReceiptNo = data.r.ReceiptNo,
                        DateOut = data.r.DateOut.ToString("dd-MM-yyyy"),
                        TimeOut = data.r.TimeOut,
                        DiscountItem = _fncModule.ToCurrency(data.r.DiscountValue, plCur.Amounts),
                        Currency = data.curr_pl.Description,
                        GrandTotal = _fncModule.ToCurrency(data.r.GrandTotal, plCur.Amounts),
                        DisRemark = data.r.RemarkDiscount,
                        //Summary
                        DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                        DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                        //SCount = ChCount.ToString(),
                        GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                        //SCount = string.Format("{0:n0}", Summary.FirstOrDefault().CountReceipt),
                        //SDiscountItem = string.Format("{0:#,0.000}", Summary.FirstOrDefault().DiscountItem),
                        SDiscountItem = _fncModule.ToCurrency(_summary.DiscountItem, sysCur.Amounts),
                        SDiscountTotal = _fncModule.ToCurrency(_summary.DiscountTotal, sysCur.Amounts),
                        SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(_summary.TaxValue, sysCur.Amounts),
                        SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(_summary.GrandTotalSys, sysCur.Amounts),
                        SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(receipts.Sum(s => s.GrandTotal_Sys) * receipts.FirstOrDefault().LocalSetRate, plCur.Amounts),
                        //
                        Remark = data.r.Remark,
                        TotalDiscountItem = (decimal)_context.ReceiptDetail.Where(w => w.ReceiptID == data.r.ReceiptID).Sum(s => s.DiscountValue),
                        DiscountTotal = data.r.DiscountValue,
                        Vat = data.r.TaxValue * data.r.ExchangeRate,
                        GrandTotalSys = data.r.GrandTotal_Sys,
                        MGrandTotal = data.r.GrandTotal_Sys * data.r.LocalSetRate,
                    }).ToList();
            double TotalDisItem = 0;
            double TotalDisTotal = 0;
            double TotalVat = 0;
            double GrandTotalSys = 0;
            double GrandTotal = 0;
            var _saleARs = _context.SaleARs.Where(s => _fncModule.IsBetweenDate(_dateFrom, _dateTo, s.PostingDate));
            if (BranchID > 0) { _saleARs = _saleARs.Where(s => s.BranchID == BranchID); }
            if (UserID > 0) { _saleARs = _saleARs.Where(s => s.UserID == UserID); }
            if (plid > 0) { _saleARs = _saleARs.Where(s => s.PriceListID == plid); }
            saleARs = await _saleARs.ToListAsync();

            var saleARSummary = saleARs;
            if (plid > 0) saleARSummary = saleARSummary.Where(i => i.PriceListID == plid).ToList();
            var saleARDetail = from s in saleARSummary
                               join sd in _context.SaleARDetails on s.SARID equals sd.SARID
                               select new
                               {
                                   TotalDisItem = sd.DisValue * s.ExchangeRate
                               };
            TotalDisItem = saleARDetail.Sum(s => s.TotalDisItem);
            TotalDisTotal = saleARSummary.Sum(s => s.DisValue);
            TotalVat = saleARSummary.Sum(s => s.VatValue * s.ExchangeRate);
            GrandTotalSys = saleARSummary.Sum(s => s.TotalAmountSys);
            GrandTotal = saleARSummary.Sum(s => s.TotalAmountSys * s.LocalSetRate);

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
                              //detail
                              ReceiptID = data.sale.SeriesDID,
                              AmountFreight = _fncModule.ToCurrency(data.sale.FreightAmount, plCur.Amounts),
                              DouType = data.douType.Code,
                              EmpCode = data.emp.Code,
                              EmpName = data.emp.Name,
                              BranchID = data.sale.BranchID,
                              BranchName = data.b.Name,
                              ReceiptNo = data.sale.InvoiceNo,
                              DateOut = data.sale.PostingDate.ToString("dd-MM-yyyy"),
                              TimeOut = "",
                              DiscountItem = _fncModule.ToCurrency(data.sale.DisValue, plCur.Amounts),
                              Currency = data.curr_pl.Description,
                              GrandTotal = _fncModule.ToCurrency(data.sale.TotalAmount, plCur.Amounts),
                              //Summary
                              DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                              DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                              //SCount = ChCount.ToString(),
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

            if (DouType == "SP")
            {
                return Ok(Sale.OrderBy(o => o.DateOut));
            }
            else if (DouType == "IN")
            {
                return Ok(saleAR.OrderBy(o => o.DateOut));
            }
            else
            {
                var saleSummary = Summary;
                var allSummarySale = new List<DevSummarySale>
                (Sale.Count + saleAR.Count + saleSummary.Count);
                allSummarySale.AddRange(Sale);
                allSummarySale.AddRange(saleAR);
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
                                   data.all.TimeOut,
                                   data.all.DiscountItem,
                                   data.all.Currency,
                                   data.all.GrandTotal,
                                   data.all.DisRemark,
                                   data.all.AmountFreight,
                                   //Summary
                                   DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                   DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                   //SCount = ChCount.ToString(),
                                   GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                                   SDiscountItem = _fncModule.ToCurrency(allSummarySale.Sum(s => s.TotalDiscountItem), sysCur.Amounts),
                                   SDiscountTotal = _fncModule.ToCurrency(allSummarySale.Sum(s => s.DiscountTotal), sysCur.Amounts),
                                   data.all.Remark,
                                   //    SDiscountTotal = _fncModule.ToCurrency(r.Sum(s => s.all.DiscountTotal), sysCur.Amounts),
                                   SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(allSummarySale.Sum(v => v.Vat), sysCur.Amounts),
                                   SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(allSummarySale.Sum(_r => _r.GrandTotalSys), sysCur.Amounts),
                                   SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(allSummarySale.Sum(_r => _r.MGrandTotal), plCur.Amounts),
                               }).ToList();
                return Ok(allSale.OrderBy(o => o.DateOut));
            }
        }

        //GetPurchaseMemo edit
        [HttpGet]
        public IActionResult GetPurchaseMemo(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID, int WarehouseID)
        {
            List<PurchaseCreditMemo> PurchaseCreditMemos = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && WarehouseID == 0 && VendorID == 0)
            {
                PurchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID == 0 && VendorID == 0)
            {
                PurchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID == 0 && VendorID == 0)
            {
                PurchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID != 0 && VendorID == 0)
            {
                PurchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WarehouseID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID != 0 && VendorID == 0)
            {
                PurchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.WarehouseID == WarehouseID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID != 0 && VendorID != 0)
            {
                PurchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && WarehouseID == 0 && VendorID != 0)
            {
                PurchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID == 0 && VendorID != 0)
            {
                PurchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && WarehouseID == 0 && VendorID != 0)
            {
                PurchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && WarehouseID != 0 && VendorID != 0)
            {
                PurchaseCreditMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WarehouseID && w.VendorID == VendorID).ToList();
            }
            else
            {
                return Ok(new List<PurchaseCreditMemo>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var Summary = GetSummaryPurchaseTotals(DateFrom, DateTo, BranchID, UserID, VendorID, WarehouseID, "PC");
            if (Summary != null)
            {
                SystemCurrency SSC = GetSystemCurrencies().FirstOrDefault();
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var PurchaseCreditMemo = PurchaseCreditMemos;
                var list = from PCD in _context.PurchaseCreditMemoDetails
                           join PC in PurchaseCreditMemo on PCD.PurchaseCreditMemoID equals PC.PurchaseMemoID
                           join BP in _context.BusinessPartners on PC.VendorID equals BP.ID
                           join I in _context.ItemMasterDatas on PCD.ItemID equals I.ID
                           join CU in _context.Currency on PC.PurCurrencyID equals CU.ID
                           join UOM in _context.UnitofMeasures on PC.UserID equals UOM.ID
                           join lc in _context.Currency on PC.LocalCurID equals lc.ID
                           group new { PCD, PC, BP, I, CU, UOM, lc } by new { PC.PurchaseMemoID, PCD.PurchaseMemoDetailID, PCD.PurchasPrice } into datas
                           let data = datas.FirstOrDefault()
                           let PC = data.PC
                           let BP = data.BP
                           let I = data.I
                           let PurchaseCreditMemoDetail = data.PCD
                           let CU = data.CU
                           let Uom = data.UOM
                           let lc = data.lc
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == SSC.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CU.ID) ?? new Display()
                           select new
                           {
                               ///Master
                               PC.InvoiceNo,
                               BP.Name,
                               PostingDate = PC.PostingDate.ToString("dd-MM-yyyy"),
                               Discount = _fncModule.ToCurrency(PC.DiscountValue, plCur.Amounts),
                               Applied_Amount = _fncModule.ToCurrency(PC.AppliedAmount, plCur.Amounts),
                               Balance_Due = _fncModule.ToCurrency(PC.BalanceDue, plCur.Amounts),
                               Sub_Total = _fncModule.ToCurrency(PC.SubTotal, plCur.Amounts),
                               BaseOn = PC.BaseOnID,
                               PLC = CU.Description,
                               //Detail
                               I.Code,
                               I.KhmerName,
                               PurchaseCreditMemoDetail.Qty,
                               UnitPrice = _fncModule.ToCurrency(PurchaseCreditMemoDetail.PurchasPrice, plCur.Amounts),
                               DisItem = _fncModule.ToCurrency(PurchaseCreditMemoDetail.DiscountValue, plCur.Amounts),
                               Total = _fncModule.ToCurrency(PurchaseCreditMemoDetail.Total, plCur.Amounts),
                               Uom = Uom.Name,
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               // Summary
                               CountInvoiceNo = Summary.FirstOrDefault().CountReceipt,
                               DisItems = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                               DiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                               GrandTotalSSC = SSC.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().BalanceDueSSC, sysCur.Amounts),
                               GrandTotal = lc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().BalanceDue, lcCur.Amounts),
                           };
                return Ok(list);
            }
            else
            {
                return Ok(new List<PurchaseCreditMemo>());
            }
        }

        [HttpGet]
        public IActionResult GetOutgoingPayment(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID)
        {
            List<OutgoingPayment> OutgoingPayments = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && VendorID == 0)
            {
                OutgoingPayments = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && VendorID == 0)
            {
                OutgoingPayments = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && VendorID == 0)
            {
                OutgoingPayments = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && VendorID != 0)
            {
                OutgoingPayments = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && VendorID != 0)
            {
                OutgoingPayments = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && VendorID != 0)
            {
                OutgoingPayments = _context.OutgoingPayments.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID).ToList();
            }
            else
            {
                return Ok(new List<OutgoingPayment>());
            }
            var TotalAmountDues = OutgoingPayments.Sum(s => s.TotalAmountDue);
            var countInvoiceNo = OutgoingPayments.Count;
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
            SystemCurrency SSC = GetSystemCurrencies().FirstOrDefault();
            var summ = from SOPD in _context.OutgoingPaymentDetails
                       join OP in OutgoingPayments on SOPD.OutgoingPaymentID equals OP.OutgoingPaymentID
                       select new
                       {
                           SOPD = SOPD.Totalpayment * SOPD.ExchangeRate,
                       };
            var list = from OPD in _context.OutgoingPaymentDetails
                       join OP in OutgoingPayments on OPD.OutgoingPaymentID equals OP.OutgoingPaymentID
                       join CU in _context.Currency on OPD.CurrencyID equals CU.ID
                       join Bus in _context.BusinessPartners on OP.VendorID equals Bus.ID
                       join s in _context.Series on OP.SeriesID equals s.ID
                       join docType in _context.DocumentTypes on OP.DocumentID equals docType.ID
                       group new { OPD, OP, CU, Bus, docType } by new { OP.VendorID, OPD.NumberInvioce, OPD.OutgoingPaymentDetailID, s.PreFix } into datas
                       let data = datas.FirstOrDefault()
                       let OP = data.OP
                       let OPD = data.OPD
                       let CU = data.CU
                       let docType = data.docType
                       let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CU.ID) ?? new Display()
                       select new
                       {
                           //Master
                           OP.Number,
                           VendorID = datas.FirstOrDefault().Bus.Name,
                           PostingDate = OP.PostingDate.ToString("dd-MM-yyyy"),
                           DocumentDate = OP.DocumentDate.ToString("dd-MM-yyyy"),
                           TotalAmountDue = _fncModule.ToCurrency(OP.TotalAmountDue, plCur.Amounts),
                           //Detail
                           OPD.NumberInvioce,
                           docType.Name,
                           Date = OPD.Date.ToString("dd-MM-yyyy"),
                           DiscountValue = CU.Description + " " + _fncModule.ToCurrency(OPD.TotalDiscount, plCur.Amounts),
                           OPD.OverdueDays,
                           OPD.BalanceDue,
                           TotalPayment = CU.Description + " " + _fncModule.ToCurrency(OPD.Totalpayment, plCur.Amounts),
                           Cash = CU.Description + " " + _fncModule.ToCurrency(OPD.Totalpayment, plCur.Amounts),
                           DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                           DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                           //summary
                           TotalAmountDues = SSC.Description + " " + _fncModule.ToCurrency(summ.Sum(s => s.SOPD), lcCur.Amounts),
                           CountInvoiceNo = countInvoiceNo,
                       };
            return Ok(list);
        }

        //Inventory
        public async Task<IActionResult> StockInWarehouseReport(int BranchID, int WarehouseID, bool Inactive)
        {
            // var pendingstock = GetPendingStock().ToList();
            var items = await GetWarehouseStockItemsAsync(BranchID, WarehouseID, Inactive);
            return Ok(items);
        }

        private async Task<List<WarehouseStockItemView>> GetWarehouseStockItemsAsync(int branchId, int warehouseId, bool inActive)
        {
            var receipts = await _context.Receipt.Where(x =>
                !_context.InventoryAudits.Any(i => i.SeriesDetailID == x.SeriesDID)
            ).ToListAsync();

            var items = await (from whs in _context.WarehouseSummary
                               join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                               join curr in _context.Currency on whs.CurrencyID equals curr.ID
                               join uom in _context.UnitofMeasures on whs.UomID equals uom.ID
                               join wh in _context.Warehouses on whs.WarehouseID equals wh.ID
                               where item.Delete == inActive && item.Inventory == true && item.Purchase == true && item.Type != "Standard"
                               let dc = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curr.ID) ?? new Display()
                               // let receiptDetails = pendingstock.SelectMany(r => r.ReceiptDetailVeiws).Where(rd => rd.ItemID == item.ID)
                               orderby item.Code
                               select new { whs, item, uom, wh, dc, curr }).ToListAsync();
            if (branchId > 0)
            {
                receipts = receipts.Where(r => r.BranchID == branchId).ToList();
                items = items.Where(i => i.wh.BranchID == branchId).ToList();
            }

            if (warehouseId > 0)
            {
                receipts = receipts.Where(r => r.WarehouseID == warehouseId).ToList();
                items = items.Where(i => i.wh.ID == warehouseId).ToList();
            }

            var _items = items.Select(g => new WarehouseStockItemView
            {
                ID = g.item.ID,
                BranchID = g.wh.BranchID,
                WarehouseID = g.wh.ID,
                Barcode = g.item.Barcode,
                Code = g.item.Code,
                Image = g.item.Image,
                KhmerName = g.item.KhmerName,
                EnglishName = g.item.EnglishName,
                StockIn = g.whs.InStock,
                //StockPending = receiptDetails.Sum(rd => rd.Qty * rd.Factor),
                StockPending = GetPendingStockCount(receipts, g.item.GroupUomID, g.item.ID),
                StockCommit = g.whs.Committed,
                Ordered = g.whs.Ordered,
                UomName = g.uom.Name,
                CumulativeValue = _fncModule.ToCurrency(g.whs.CumulativeValue, g.dc.Amounts),
                WhCode = g.wh.Code,
                // TotalCost = g.whs.CumulativeValue.ToString(),
                TotalCost = g.whs.CumulativeValue,
                CumulativeCost = g.curr.Description + " " + g.whs.CumulativeValue
            }).ToList();
            return _items;
        }

        public double GetPendingStockCount(List<Receipt> receipts, int groupUomId, int itemId)
        {
            var data = (from r in receipts
                        join red in _context.ReceiptDetail.Where(rd => rd.ItemID == itemId) on r.ReceiptID equals red.ReceiptID
                        join guom in _context.GroupDUoMs.Where(gd => gd.GroupUoMID == groupUomId) on red.UomID equals guom.AltUOM
                        // let guom = _context.GroupDUoMs.FirstOrDefault(w => w.AltUOM == red.UomID && w.GroupUoMID == GroupUomID) ?? new GroupDUoM()
                        select new StockCount
                        {
                            ReceiptDetailID = red.ID,
                            ItemID = red.ItemID,
                            Qty = red.Qty,
                            UomID = red.UomID,
                            Factor = guom.Factor
                        }).ToList();
            return data.Sum(s => s.Qty * s.Factor);
        }

        public IActionResult InventoryAuditReport(string DateFrom, string DateTo, int BranchID, int WarehouseID)
        {
            List<InventoryAudit> inventoryAudits = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && WarehouseID == 0)
            {
                inventoryAudits = _context.InventoryAudits.Where(w => w.SystemDate >= Convert.ToDateTime(DateFrom) && w.SystemDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && WarehouseID == 0)
            {
                inventoryAudits = _context.InventoryAudits.Where(w => w.BranchID == BranchID && w.SystemDate >= Convert.ToDateTime(DateFrom) && w.SystemDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && WarehouseID != 0)
            {
                inventoryAudits = _context.InventoryAudits.Where(w => w.BranchID == BranchID && w.WarehouseID == WarehouseID && w.SystemDate >= Convert.ToDateTime(DateFrom) && w.SystemDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom == null && DateTo == null && BranchID != 0 && WarehouseID == 0)
            {
                inventoryAudits = _context.InventoryAudits.Where(w => w.BranchID == BranchID).ToList();
            }
            else if (DateFrom == null && DateTo == null && BranchID != 0 && WarehouseID != 0)
            {
                inventoryAudits = _context.InventoryAudits.Where(w => w.BranchID == BranchID && w.WarehouseID == WarehouseID).ToList();
            }
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
            var TranValue = _context.InventoryAudits.Sum(s => s.Trans_Valuse);
            var TotalQty = _context.InventoryAudits.Sum(s => s.Qty);
            var Inventory = from au in inventoryAudits
                            join curr in _context.Currency on au.CurrencyID equals curr.ID
                            join uom in _context.UnitofMeasures on au.UomID equals uom.ID
                            join wh in _context.Warehouses on au.WarehouseID equals wh.ID
                            join user in _context.UserAccounts on au.UserID equals user.ID
                            join emp in _context.Employees on user.EmployeeID equals emp.ID
                            join item in _context.ItemMasterDatas on au.ItemID equals item.ID
                            where au.Process != "Standard"
                            orderby item.Code
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curr.ID) ?? new Display()
                            select new
                            {
                                au.ID,
                                WhCode = wh.Code,
                                EmpName = emp.Name,
                                au.InvoiceNo,
                                TranType = au.Trans_Type,
                                SystemDate = au.SystemDate.ToString("dd-MM-yyyy"),
                                au.TimeIn,
                                ItemID = item.ID,
                                item.Code,
                                item.KhmerName,
                                item.EnglishName,
                                au.Qty,
                                TotalQty = string.Format("{0:n0}", TotalQty),
                                Uom = uom.Name,
                                Cost = _fncModule.ToCurrency(au.Cost, plCur.Amounts),
                                TranValue = _fncModule.ToCurrency(au.Trans_Valuse, plCur.Amounts),
                                TranValues = _fncModule.ToCurrency(TranValue, plCur.Amounts),
                                au.CumulativeQty,
                                CumulativeValue = _fncModule.ToCurrency(au.CumulativeValue, lcCur.Amounts),
                                Currency = curr.Description,
                                au.Process,
                                item.Barcode,
                                ExpireDate = (au.ExpireDate.ToString("dd-MM-yyyy") == "01-01-0001" || au.ExpireDate.ToString("dd-MM-yyyy") == "09-09-2019") ? "None" : au.ExpireDate.ToString("dd-MM-yyyy"),
                                DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                            };
            return Ok(Inventory);
        }
        public IActionResult StockMovingReport(string DateFrom, string DateTo, int BranchID, int WarehouseID)
        {
            if (string.IsNullOrWhiteSpace(DateFrom) || string.IsNullOrWhiteSpace(DateTo))
                return Ok();

            List<InventoryAudit> StockMovingFilter = new();
            List<InventoryAudit> invYesterday = new();
            _ = DateTime.TryParse(DateFrom, out DateTime _dateFrom);
            DateTime dateFromYest = _dateFrom.Subtract(TimeSpan.FromDays(1));// addDay()
                                                                             // _dateFrom = _dateFrom.Add(TimeSpan.FromDays(1));
            _ = DateTime.TryParse(DateTo, out DateTime _dateTo);


            invYesterday = _context.InventoryAudits.Where(i => _fncModule.IsBetweenDate(dateFromYest, _dateTo, i.PostingDate)).ToList();
            if (BranchID > 0)
            {
                invYesterday = invYesterday.Where(i => i.BranchID == BranchID).ToList();
            }
            if (WarehouseID > 0)
            {
                invYesterday = invYesterday.Where(i => i.WarehouseID == WarehouseID).ToList();
            }
            StockMovingFilter = invYesterday.Where(i => _fncModule.IsBetweenDate(_dateFrom, _dateTo, i.PostingDate)).ToList();
            invYesterday = invYesterday.Where(i => _fncModule.IsBetweenDate(dateFromYest, dateFromYest, i.PostingDate)).ToList();
            var stockmovingYest = (from st in invYesterday
                                   join item in _context.ItemMasterDatas.Where(s => s.Process != "Standard") on st.ItemID equals item.ID
                                   join uom in _context.UnitofMeasures on st.UomID equals uom.ID
                                   join cur in _context.Currency on st.CurrencyID equals cur.ID
                                   join g1 in _context.ItemGroup1 on item.ItemGroup1ID equals g1.ItemG1ID

                                   join wh in _context.Warehouses on st.WarehouseID equals wh.ID
                                   orderby item.Code
                                   group new { uom, cur, g1, wh, item, st } by new { st.WarehouseID, st.ItemID } into g
                                   let data = g.FirstOrDefault()
                                   let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == g.FirstOrDefault().cur.ID) ?? new Display()
                                   select new
                                   {
                                       WhID = data.st.WarehouseID,
                                       ItemID = data.st.ItemID,
                                       ItemCode = g.FirstOrDefault().item.Code,
                                       KhmerName = g.FirstOrDefault().item.KhmerName,
                                       EnglishName = g.FirstOrDefault().item.EnglishName,
                                       Barcode = g.FirstOrDefault().item.Barcode,
                                       Uom = data.uom.Name,
                                       Group1 = data.g1.Name,

                                       Whcode = data.wh.Code,
                                       OB = g.FirstOrDefault().st.CumulativeQty,


                                   }).ToList();

            double totalcostt = 0;
            foreach (var In in StockMovingFilter)
            {
                totalcostt += In.Trans_Valuse;
            }

            var arEdit = (from ard in _context.SaleAREdites
                          join inv in StockMovingFilter on ard.SeriesDID equals inv.SeriesDetailID
                          select inv
                            );

            var GetSysCurr = GetSystemCurrencies().FirstOrDefault();
            var stockmoving = (from st in StockMovingFilter
                               join item in _context.ItemMasterDatas.Where(s => s.Process != "Standard") on st.ItemID equals item.ID
                               join uom in _context.UnitofMeasures on st.UomID equals uom.ID
                               join cur in _context.Currency on st.CurrencyID equals cur.ID
                               join g1 in _context.ItemGroup1 on item.ItemGroup1ID equals g1.ItemG1ID
                               join wh in _context.Warehouses on st.WarehouseID equals wh.ID
                               orderby item.Code
                               group new { uom, cur, g1, wh, item, st } by new { st.WarehouseID, st.ItemID } into g
                               let data = g.FirstOrDefault()
                               let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == g.FirstOrDefault().cur.ID) ?? new Display()
                               let arEditIn = arEdit.Where(w => w.ItemID == data.st.ItemID && w.WarehouseID == data.st.WarehouseID && w.Qty > 0)
                               let arEditout = arEdit.Where(w => w.ItemID == data.st.ItemID && w.WarehouseID == data.st.WarehouseID && w.Qty < 0)
                               let g2 = _context.ItemGroup2.FirstOrDefault(s => s.ItemG2ID == data.item.ItemGroup2ID) ?? new Models.Services.Inventory.Category.ItemGroup2()
                               let g3 = _context.ItemGroup3.FirstOrDefault(s => s.ID == data.item.ItemGroup3ID) ?? new Models.Services.Inventory.Category.ItemGroup3()
                               select new StockMovingView
                               {
                                   WhID = data.st.WarehouseID,
                                   ItemID = data.st.ItemID,
                                   ItemCode = g.FirstOrDefault().item.Code,
                                   KhmerName = g.FirstOrDefault().item.KhmerName,
                                   EnglishName = g.FirstOrDefault().item.EnglishName,
                                   Barcode = g.FirstOrDefault().item.Barcode,
                                   Uom = data.uom.Name,
                                   Group1 = data.g1.Name,
                                   Group2 = g2.Name,
                                   Group3 = g3.Name,
                                   Whcode = data.wh.Code,

                                   OB = g.FirstOrDefault().st.CumulativeQty,
                                   PU = g.Where(w => w.st.Trans_Type.Trim() == "PU").Sum(s => s.st.Qty),
                                   GR = g.Where(w => w.st.Trans_Type.Trim() == "GR").Sum(s => s.st.Qty),
                                   PC = g.Where(w => w.st.Trans_Type.Trim() == "PC").Sum(s => s.st.Qty),
                                   IN = g.Where(w => w.st.Trans_Type.Trim() == "IN").Sum(s => s.st.Qty),
                                   CN = g.Where(w => w.st.Trans_Type.Trim() == "CN").Sum(s => s.st.Qty),
                                   GI = g.Where(w => w.st.Trans_Type.Trim() == "GI").Sum(s => s.st.Qty),
                                   SP = g.Where(w => w.st.Trans_Type.Trim() == "SP").Sum(s => s.st.Qty),
                                   RP = g.Where(w => w.st.Trans_Type.Trim() == "RP").Sum(s => s.st.Qty),
                                   RE = g.Where(w => w.st.Trans_Type.Trim() == "RE").Sum(s => s.st.Qty),
                                   PD = g.Where(w => w.st.Trans_Type.Trim() == "PD").Sum(s => s.st.Qty),
                                   ST = g.Where(w => w.st.Trans_Type.Trim() == "ST" && w.st.Qty > 0).Sum(s => s.st.Qty),
                                   STT = g.Where(w => w.st.Trans_Type.Trim() == "ST" && w.st.Qty < 0).Sum(s => s.st.Qty),
                                   DN = g.Where(w => w.st.Trans_Type.Trim() == "DN").Sum(s => s.st.Qty),
                                   INEIN = arEditIn.Sum(s => s.Qty),
                                   INEOUT = arEditout.Sum(s => s.Qty),
                                   EB = g.LastOrDefault().st.CumulativeQty,
                                   TotalCost = _fncModule.ToCurrency(g.Sum(s => s.st.Trans_Valuse), plCur.Amounts),
                                   totalcosttt = _fncModule.ToCurrency(totalcostt, plCur.Amounts),
                                   Currency = GetSysCurr.Description
                               }).ToList();
            stockmoving.ForEach(i =>
            {
                i.OB = stockmovingYest.Where(x => x.WhID == i.WhID && x.ItemID == i.ItemID).Sum(s => s.OB);
                i.EB = (i.PU + i.OB) + i.IN + i.GR + i.PC + i.CN + i.GI + i.SP + i.RP + i.ST + i.STT + i.DN + i.INEIN + i.INEOUT;

            });
            return Ok(stockmoving);
        }
        private IEnumerable<SystemCurrency> GetSystemCurrencies()
        {
            IEnumerable<SystemCurrency> currencies = (from com in _context.Company.Where(x => x.Delete == false)
                                                      join c in _context.Currency.Where(x => x.Delete == false) on com.SystemCurrencyID equals c.ID
                                                      select new SystemCurrency
                                                      {
                                                          ID = c.ID,
                                                          Description = c.Description
                                                      });
            return currencies;
        }
        public IActionResult StockExpiredReport(string DateFrom, string DateTo, int WarehouseID)
        {
            List<WarehouseDetail> warehouseDetailsFilter = new();
            if (DateFrom != null && DateTo != null && WarehouseID == 0)
            {
                warehouseDetailsFilter = _context.WarehouseDetails.Where(w => w.ExpireDate >= Convert.ToDateTime(DateFrom) && w.ExpireDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && WarehouseID != 0)
            {
                warehouseDetailsFilter = _context.WarehouseDetails.Where(w => w.ExpireDate >= Convert.ToDateTime(DateFrom) && w.ExpireDate <= Convert.ToDateTime(DateTo) && w.WarehouseID == WarehouseID).ToList();
            }

            else
            {
                return Ok(new List<WarehouseDetail>());
            }

            var WarehouseD = warehouseDetailsFilter;
            var WareDetail = from wd in WarehouseD
                             join wh in _context.Warehouses on wd.WarehouseID equals wh.ID
                             join curr in _context.Currency on wd.CurrencyID equals curr.ID
                             join item in _context.ItemMasterDatas on wd.ItemID equals item.ID
                             join uom in _context.UnitofMeasures on item.SaleUomID equals uom.ID
                             where wd.InStock > 0 && wd.BatchNo != null
                             group new { wd, wh, item, uom, curr } by new { wd.ID, wd.WarehouseID } into datas
                             let data = datas.FirstOrDefault()
                             let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr.ID) ?? new Display()
                             select new
                             {

                                 data.item.Barcode,
                                 data.wd.BatchNo,
                                 data.item.Code,
                                 ItemID = data.item.ID,
                                 data.item.KhmerName,
                                 data.item.EnglishName,
                                 Stock = datas.Sum(s => s.wd.InStock),
                                 Cost = _fncModule.ToCurrency(data.wd.Cost, plCur.Amounts),
                                 ExpiredDate = data.wd.ExpireDate.ToString("dd-MM-yyyy"),
                                 Uom = data.uom.Name,
                                 WhCode = data.wh.Code,
                                 Currency = data.curr.Description,
                                 Total = _fncModule.ToCurrency(datas.Sum(s => s.wd.InStock) * data.wd.Cost, plCur.Amounts),

                             };

            return Ok(WareDetail);
        }

        public IActionResult GoodsReceiptStockReport(string DateFrom, string DateTo, int BranchID, int WHID, int UserID)
        {
            List<GoodsReceipt> goodsFilter = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && WHID == 0 && UserID == 0)
            {
                goodsFilter = _context.GoodsReceipts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && WHID == 0 && UserID == 0)
            {
                goodsFilter = _context.GoodsReceipts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && WHID != 0 && UserID == 0)
            {
                goodsFilter = _context.GoodsReceipts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WHID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && WHID != 0 && UserID != 0)
            {
                goodsFilter = _context.GoodsReceipts.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WHID && w.UserID == UserID).ToList();
            }
            else
            {
                return Ok(new List<GoodsReceipt>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";
            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var GoodReceipts = goodsFilter;
            var list = from gr in GoodReceipts
                       join grd in _context.GoodReceiptDetails on gr.GoodsReceiptID equals grd.GoodsReceiptID
                       join Uom in _context.UnitofMeasures on grd.UomID equals Uom.ID
                       group new { gr, grd, Uom } by new { gr.Number_No, grd.ItemID } into g
                       select new
                       {
                           //Master
                           NumberNo = g.FirstOrDefault().gr.Number_No,
                           PostingDate = Convert.ToDateTime(g.FirstOrDefault().gr.PostingDate).ToString("dd-MM-yyyy"),
                           DocDate = Convert.ToDateTime(g.FirstOrDefault().gr.DocumentDate).ToString("dd-MM-yyyy"),
                           //Detail
                           g.FirstOrDefault().grd.Code,
                           g.FirstOrDefault().grd.KhmerName,
                           g.FirstOrDefault().grd.EnglishName,
                           Qty = g.FirstOrDefault().grd.Quantity,
                           g.FirstOrDefault().grd.Cost,
                           Uom = g.FirstOrDefault().Uom.Name,
                           Barcode = g.FirstOrDefault().grd.BarCode,
                           ExpireDate = Convert.ToDateTime(g.First().grd.ExpireDate).ToString("dd-MM-yyyy"),
                           //Summary
                           Subtotal = g.Sum(x => x.grd.Cost * x.grd.Quantity),
                           g.FirstOrDefault().grd.Currency,
                           GrandTotal = g.Sum(x => x.grd.Cost * x.grd.Quantity)
                       };

            return Ok(list);
        }
        public IActionResult GoodsIssueStockReport(string DateFrom, string DateTo, int BranchID, int WHID, int UserID)
        {
            List<GoodIssues> goodsFilter = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && WHID == 0 && UserID == 0)
            {
                goodsFilter = _context.GoodIssues.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && WHID == 0 && UserID == 0)
            {
                goodsFilter = _context.GoodIssues.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && WHID != 0 && UserID == 0)
            {
                goodsFilter = _context.GoodIssues.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WHID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && WHID != 0 && UserID != 0)
            {
                goodsFilter = _context.GoodIssues.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.WarehouseID == WHID && w.UserID == UserID).ToList();
            }
            else
            {
                return Ok(new List<GoodIssues>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";
            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var GoodIssues = goodsFilter;
            var list = from gi in GoodIssues
                       join gid in _context.GoodIssuesDetails on gi.GoodIssuesID equals gid.GoodIssuesID
                       join Uom in _context.UnitofMeasures on gid.UomID equals Uom.ID
                       group new { gi, gid, Uom } by new { gi.Number_No, gid.ItemID } into g
                       select new
                       {
                           //Master
                           NumberNo = g.FirstOrDefault().gi.Number_No,
                           PostingDate = Convert.ToDateTime(g.FirstOrDefault().gi.PostingDate).ToString("dd-MM-yyyy"),
                           DocDate = Convert.ToDateTime(g.FirstOrDefault().gi.DocumentDate).ToString("dd-MM-yyyy"),
                           //Detail
                           g.FirstOrDefault().gid.Code,
                           g.FirstOrDefault().gid.KhmerName,
                           g.FirstOrDefault().gid.EnglishName,
                           Qty = g.FirstOrDefault().gid.Quantity,
                           g.FirstOrDefault().gid.Cost,
                           Uom = g.FirstOrDefault().Uom.Name,
                           Barcode = g.FirstOrDefault().gid.BarCode,
                           ExpireDate = Convert.ToDateTime(g.First().gid.ExpireDate).ToString("dd-MM-yyyy"),
                           //Summary
                           Subtotal = g.Sum(x => x.gid.Cost * x.gid.Quantity),
                           g.FirstOrDefault().gid.Currency,
                           GrandTotal = g.Sum(x => x.gid.Cost * x.gid.Quantity)
                       };

            return Ok(list);
        }
        public IActionResult TransferStockReport(string DateFrom, string DateTo, int FromBranchID, int ToBranchID, int FromWHID, int ToWHID, int UserID)
        {
            List<Transfer> goodsFilter = new();
            if (DateFrom != null && DateTo != null && FromBranchID == 0 && ToBranchID == 0 && FromWHID == 0 && ToWHID == 0 && UserID == 0)
            {
                goodsFilter = _context.Transfers.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && FromBranchID != 0 && ToBranchID != 0 && FromWHID == 0 && ToWHID == 0 && UserID == 0)
            {
                goodsFilter = _context.Transfers.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == FromBranchID && w.BranchToID == ToBranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && FromBranchID != 0 && ToBranchID != 0 && FromWHID != 0 && ToWHID != 0 && UserID == 0)
            {
                goodsFilter = _context.Transfers.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == FromBranchID && w.BranchToID == ToBranchID && w.WarehouseFromID == FromWHID && w.WarehouseToID == ToWHID).ToList();
            }
            else if (DateFrom != null && DateTo != null && FromBranchID != 0 && ToBranchID != 0 && FromWHID == 0 && ToWHID == 0 && UserID != 0)
            {
                goodsFilter = _context.Transfers.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == FromBranchID && w.BranchToID == ToBranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && FromBranchID != 0 && ToBranchID != 0 && FromWHID != 0 && ToWHID != 0 && UserID != 0)
            {
                goodsFilter = _context.Transfers.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == FromBranchID && w.BranchToID == ToBranchID && w.WarehouseFromID == FromWHID && w.WarehouseToID == ToWHID && w.UserID == UserID).ToList();
            }
            else
            {
                return Ok(new List<Transfer>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";
            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }

            var Transfer = goodsFilter;
            var list = from t in Transfer
                       join td in _context.TransferDetails on t.TarmsferID equals td.TransferID
                       join Uom in _context.UnitofMeasures on td.UomID equals Uom.ID
                       join cur in _context.Currency on t.LocalCurID equals cur.ID
                       join cursys in _context.Currency on t.SysCurID equals cursys.ID
                       group new { t, td, Uom, cur, cursys } by new { t.Number, td.ItemID } into g
                       let data = g.FirstOrDefault()
                       let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.cur.ID) ?? new Display()
                       let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.cursys.ID) ?? new Display()
                       select new
                       {
                           //Master
                           data.t.Number,
                           PostingDate = Convert.ToDateTime(data.t.PostingDate).ToString("dd-MM-yyyy"),
                           DocDate = Convert.ToDateTime(data.t.DocumentDate).ToString("dd-MM-yyyy"),
                           Time = Convert.ToDateTime(data.t.Time).ToString("hh:mm tt"),
                           //Detail
                           data.td.Code,
                           data.td.KhmerName,
                           data.td.Qty,
                           Cost = _fncModule.ToCurrency(data.td.Cost, plCur.Amounts),
                           Uom = data.Uom.Name,
                           data.td.Barcode,
                           ExpireDate = Convert.ToDateTime(data.td.ExpireDate).ToString("dd-MM-yyyy"),
                           DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                           DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                           //Summary
                           Subtotal = _fncModule.ToCurrency(g.Sum(x => x.td.Cost * x.td.Qty), plCur.Amounts),
                           data.td.Currency,
                           GrandTotal = _fncModule.ToCurrency(g.Sum(x => x.td.Cost * x.td.Qty), plCur.Amounts)
                       };

            return Ok(list);
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
                       join emp in _context.Employees on user.EmployeeID equals emp.ID
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
        public IActionResult GetWarehouseStock()
        {
            var list = _context.Warehouses.Where(x => x.Delete == false).ToList();
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

        //GetSummaryTotals
        public IEnumerable<SummaryPurchaseTotal> GetSummaryPurchaseTotals(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID, int WarehouseID, string Type)
        {
            try
            {
                var data = _context.SummaryPurchaseTotal.FromSql("rp_GetSummarryPurchaseTotal @DateFrom={0},@DateTo={1}, @BranchID={2},@UserID={3},@VendorID={4},@WarehouseID={5},@Type={6}",
                parameters: new[] {
                    DateFrom.ToString(),
                    DateTo.ToString(),
                    BranchID.ToString(),
                    UserID.ToString(),
                    VendorID.ToString(),
                    WarehouseID.ToString(),
                    Type.ToString()
                }).ToList();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //GetPurchasePaymentTransaction
        [HttpGet]
        public IActionResult GetPurchasePaymentTransaction(string DateFrom, string DateTo, int VendorID, int VendortoID, string Status)
        {
            List<OutgoingPaymentVendor> OutFilter = new();
            List<PurchaseCreditMemo> PurchaseCreditMemoFilter = new();
            if (Status != null)
            {
                if (Status == "open")
                {
                    if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID == 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "open").ToList();
                    }
                    else if (DateFrom == null && DateTo == null && VendorID != 0 && VendortoID == 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.VendorID == VendorID && w.Status == "open").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID != 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendortoID && w.Status == "open").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID == 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendortoID && w.Status == "open").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID != 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID >= VendortoID && w.VendorID <= VendortoID && w.Status == "open").ToList();
                    }
                    else if (DateFrom == null && DateTo == null && VendorID == 0 && VendortoID == 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.Status == "open").ToList();
                    }
                    else
                    {
                        return Ok(new List<OutgoingPaymentVendor>());
                    }
                }
                else
                {
                    if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID == 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "close").ToList();
                    }
                    else if (DateFrom == null && DateTo == null && VendorID != 0 && VendortoID == 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.VendorID == VendorID && w.Status == "close").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID != 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendortoID && w.Status == "close").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID == 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID && w.Status == "close").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID != 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID >= VendorID && w.VendorID <= VendortoID && w.Status == "close").ToList();
                    }
                    else if (DateFrom == null && DateTo == null && VendorID == 0 && VendortoID == 0 && Status != null)
                    {
                        OutFilter = _context.OutgoingPaymentVendors.Where(w => w.Status == "close").ToList();
                    }
                    else
                    {
                        return Ok(new List<OutgoingPaymentVendor>());
                    }
                }
            }
            else
            {
                if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID == 0 && Status == null)
                {
                    OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                }
                else if (DateFrom == null && DateTo == null && VendorID != 0 && VendortoID == 0 && Status == null)
                {
                    OutFilter = _context.OutgoingPaymentVendors.Where(w => w.VendorID == VendorID).ToList();
                }
                else if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID != 0 && Status == null)
                {
                    OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendortoID).ToList();
                }
                else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID == 0 && Status == null)
                {
                    OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID).ToList();
                }
                else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID != 0 && Status == null)
                {
                    OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID >= VendorID && w.VendorID <= VendortoID).ToList();
                }
                else
                {
                    return Ok(new List<OutgoingPaymentVendor>());
                }
            }

            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var outgoingpaymentvendors = OutFilter;
            var SumTotal = outgoingpaymentvendors.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.TotalPayment * x.ExchangeRate);
            var SumBalanceDue = outgoingpaymentvendors.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.BalanceDue * x.ExchangeRate);
            var SumAppliedSSC = outgoingpaymentvendors.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.Applied_Amount * x.ExchangeRate);
            var SumApplied = outgoingpaymentvendors.Where(x => x.Applied_Amount != 0 || x.Total == 0).Sum(x => x.Applied_Amount * x.ExchangeRate * x.LocalSetRate);
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
            var OutgoingPayment = from opv in outgoingpaymentvendors
                                  join opd in _context.OutgoingPaymentDetails on opv.OutgoingPaymentVendorID equals opd.BasedOnID
                                  join ven in _context.BusinessPartners on opv.VendorID equals ven.ID
                                  join ssc in _context.Currency on opv.SysCurrency equals ssc.ID
                                  join pcc in _context.Currency on opv.CurrencyID equals pcc.ID
                                  join lc in _context.Currency on opv.LocalCurID equals lc.ID
                                  join op in _context.OutgoingPayments on opd.OutgoingPaymentID equals op.OutgoingPaymentID
                                  join doc in _context.DocumentTypes on op.DocumentID equals doc.ID
                                  join docs in _context.DocumentTypes on opv.DocumentID equals docs.ID
                                  where opv.Applied_Amount != 0 || opv.Total == 0
                                  let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == pcc.ID) ?? new Display()
                                  let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == ssc.ID) ?? new Display()
                                  select new SummaryOutgoingPaymentsInfo
                                  {
                                      //Master
                                      VendorName = ven.Name,
                                      MasDocumentNo = $"{docs.Code}-{opv.Number}",
                                      DocType = $"{doc.Code}-{op.NumberInvioce}",
                                      MasDate = opv.PostingDate.ToString("MM-dd-yyy"),
                                      MasTotal = pcc.Description + " " + _fncModule.ToCurrency(opv.Total, plCur.Amounts),
                                      MasApplied = pcc.Description + " " + _fncModule.ToCurrency(opv.Applied_Amount, plCur.Amounts),
                                      MasBalanceDue = pcc.Description + " " + _fncModule.ToCurrency(opv.BalanceDue, plCur.Amounts),
                                      MasStatus = opv.Status,
                                      DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                      DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                      //Detail
                                      DetailDate = op == null ? "No Detail" : op.PostingDate.ToString("MM-dd-yyy"),
                                      DetailTotal = opd == null ? "No Detail" : pcc.Description + " " + _fncModule.ToCurrency(opd.Total, plCur.Amounts),
                                      DetailCash = opd == null ? "No Detail" : pcc.Description + " " + _fncModule.ToCurrency(opd.Totalpayment, plCur.Amounts),
                                      DetailBalanceDue = opd == null ? "" : pcc.Description + " " + _fncModule.ToCurrency(opd.BalanceDue, plCur.Amounts),
                                      //Summary
                                      SumTotal = ssc.Description + " " + _fncModule.ToCurrency(SumTotal, sysCur.Amounts),
                                      SumBalanceDue = ssc.Description + " " + _fncModule.ToCurrency(SumBalanceDue, sysCur.Amounts),
                                      SumAppliedSSC = ssc.Description + " " + _fncModule.ToCurrency(SumAppliedSSC, sysCur.Amounts),
                                      SumApplied = lc.Description + " " + _fncModule.ToCurrency(SumApplied, lcCur.Amounts),
                                  };
            if (Status != null)
            {
                if (Status == "open")
                {
                    if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID == 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "open").ToList();
                    }
                    else if (DateFrom == null && DateTo == null && VendorID != 0 && VendortoID == 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.VendorID == VendorID && w.Status == "open").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID != 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendortoID && w.Status == "open").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID != 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID >= VendortoID && w.VendorID <= VendortoID && w.Status == "open").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID == 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID && w.Status == "open").ToList();
                    }
                    else if (DateFrom == null && DateTo == null && VendorID == 0 && VendortoID == 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.Status == "open").ToList();
                    }
                    else
                    {
                        return Ok(new List<PurchaseCreditMemo>());
                    }
                }
                else
                {
                    if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID == 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.Status == "close").ToList();
                    }
                    else if (DateFrom == null && DateTo == null && VendorID != 0 && VendortoID == 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.VendorID == VendorID && w.Status == "close").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID != 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendortoID && w.Status == "close").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID == 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID && w.Status == "close").ToList();
                    }
                    else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID != 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID >= VendorID && w.VendorID <= VendortoID && w.Status == "close").ToList();
                    }
                    else if (DateFrom == null && DateTo == null && VendorID == 0 && VendortoID == 0 && Status != null)
                    {
                        PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.Status == "close").ToList();
                    }
                    else
                    {
                        return Ok(new List<PurchaseCreditMemo>());
                    }
                }
            }
            else
            {
                if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID == 0 && Status == null)
                {
                    PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                }
                else if (DateFrom == null && DateTo == null && VendorID != 0 && VendortoID == 0 && Status == null)
                {
                    PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.VendorID == VendorID).ToList();
                }
                else if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID != 0 && Status == null)
                {
                    PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendortoID).ToList();
                }
                else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID == 0 && Status == null)
                {
                    PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID >= VendorID).ToList();
                }
                else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID != 0 && Status == null)
                {
                    PurchaseCreditMemoFilter = _context.PurchaseCreditMemos.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID >= VendorID && w.VendorID <= VendortoID).ToList();
                }
                else
                {
                    return Ok(new List<PurchaseCreditMemo>());
                }
            }

            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var purchaseCreditMemo = PurchaseCreditMemoFilter;
            var CreditMeomo = from cm in purchaseCreditMemo
                              join ap in _context.Purchase_APs on cm.BaseOnID equals ap.PurchaseAPID
                              join ou in _context.OutgoingPaymentVendors on ap.SeriesDetailID equals ou.SeriesDetailID
                              join oud in _context.OutgoingPaymentDetails on ou.OutgoingPaymentVendorID equals oud.BasedOnID
                              join ven in _context.BusinessPartners on ou.VendorID equals ven.ID
                              join ssc in _context.Currency on ou.SysCurrency equals ssc.ID
                              join pcc in _context.Currency on ou.CurrencyID equals pcc.ID
                              join lc in _context.Currency on ou.LocalCurID equals lc.ID
                              join op in _context.OutgoingPayments on oud.OutgoingPaymentID equals op.OutgoingPaymentID
                              join doc in _context.DocumentTypes on ou.DocumentID equals doc.ID
                              group new { cm, ap, ou, oud, ven, ssc, pcc, lc, op, doc } by new { ou.SeriesDetailID } into datas
                              let data = datas.FirstOrDefault()
                              let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.pcc.ID) ?? new Display()
                              let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.ssc.ID) ?? new Display()
                              select new SummaryOutgoingPaymentsInfo
                              {
                                  //Master
                                  VendorName = data.ven.Name,
                                  MasDocumentNo = $"{data.doc.Code}-{data.ou.Number}",
                                  DocType = $"{data.doc.Code}-{data.cm.Number}",
                                  MasDate = data.ou.PostingDate.ToString("MM-dd-yyy"),
                                  MasTotal = data.pcc.Description + " " + _fncModule.ToCurrency(data.ou.Total, plCur.Amounts),
                                  MasApplied = data.pcc.Description + " " + _fncModule.ToCurrency(data.ou.Applied_Amount, plCur.Amounts),
                                  MasBalanceDue = data.pcc.Description + " " + _fncModule.ToCurrency(data.ou.BalanceDue, plCur.Amounts),
                                  MasStatus = data.ou.Status,
                                  //Detail
                                  DetailDate = data.cm == null ? "No Detail" : data.cm.PostingDate.ToString("MM-dd-yyy"),
                                  DetailTotal = data.oud == null ? "No Detail" : data.pcc.Description + " " + _fncModule.ToCurrency(data.oud.Total, plCur.Amounts),
                                  DetailCash = data.cm == null ? "No Detail" : data.pcc.Description + " " + _fncModule.ToCurrency(data.cm.AppliedAmount, plCur.Amounts),
                                  DetailBalanceDue = data.ou == null ? "" : data.pcc.Description + " " + _fncModule.ToCurrency(data.ou.BalanceDue, plCur.Amounts),
                                  DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                  DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                  //Summary
                                  SumTotal = data.ssc.Description + " " + _fncModule.ToCurrency(SumTotal, sysCur.Amounts),
                                  SumBalanceDue = data.ssc.Description + " " + _fncModule.ToCurrency(SumBalanceDue, sysCur.Amounts),
                                  SumAppliedSSC = data.ssc.Description + " " + _fncModule.ToCurrency(SumAppliedSSC, sysCur.Amounts),
                                  SumApplied = data.lc.Description + " " + _fncModule.ToCurrency(SumApplied, lcCur.Amounts),
                              };
            var allSummaryOutgoingPayment = new List<SummaryOutgoingPaymentsInfo>
             (OutFilter.Count + PurchaseCreditMemoFilter.Count);
            allSummaryOutgoingPayment.AddRange(OutgoingPayment);
            allSummaryOutgoingPayment.AddRange(CreditMeomo);
            var allIOutgoingPayment = from ou in allSummaryOutgoingPayment
                                      select new
                                      {  //Master
                                          ou.VendorName,
                                          ou.MasDocumentNo,
                                          ou.DocType,
                                          ou.MasDate,
                                          ou.MasTotal,
                                          ou.MasApplied,
                                          ou.MasBalanceDue,
                                          ou.MasStatus,
                                          //Detail
                                          ou.DateFrom,
                                          ou.DateTo,
                                          ou.DetailDate,
                                          ou.DetailTotal,
                                          ou.DetailCash,
                                          ou.DetailBalanceDue,
                                          //Summary
                                          ou.SumTotal,
                                          ou.SumBalanceDue,
                                          ou.SumAppliedSSC,
                                          ou.SumApplied,
                                      };
            return Ok(allIOutgoingPayment);
        }
        //GetPurchaseVendorStatement
        [HttpGet]
        public IActionResult GetPurchaseVendorStatement(string DateFrom, string DateTo, int VendorID, int VendortoID)
        {
            List<OutgoingPaymentVendor> OutFilter = new();
            if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID == 0)
            {
                OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom == null && DateTo == null && VendorID != 0 && VendortoID == 0)
            {
                OutFilter = _context.OutgoingPaymentVendors.Where(w => w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && VendorID == 0 && VendortoID != 0)
            {
                OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendortoID).ToList();
            }
            else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID == 0)
            {
                OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID == VendorID).ToList();
            }
            else if (DateFrom != null && DateTo != null && VendorID != 0 && VendortoID != 0)
            {
                OutFilter = _context.OutgoingPaymentVendors.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.VendorID >= VendorID && w.VendorID <= VendorID).ToList();
            }
            else
            {
                return Ok(new List<OutgoingPaymentVendor>());
            }
            if (DateFrom == null)
            {
                DateFrom = "1900-01-01";

            }
            if (DateTo == null)
            {
                DateTo = "1900-01-01";
            }
            var outgoingpaymentvendors = OutFilter;
            var SumBalanceDueSSC = outgoingpaymentvendors.Where(x => x.Status == "open").Sum(x => x.BalanceDue * x.ExchangeRate);
            var SumBalanceDue = outgoingpaymentvendors.Where(x => x.Status == "open").Sum(x => x.BalanceDue * x.ExchangeRate * x.LocalSetRate);
            var Outgoingpay = from opv in outgoingpaymentvendors
                              join ven in _context.BusinessPartners on opv.VendorID equals ven.ID
                              join ssc in _context.Currency on opv.SysCurrency equals ssc.ID
                              join pcc in _context.Currency on opv.CurrencyID equals pcc.ID
                              join lc in _context.Currency on opv.LocalCurID equals lc.ID
                              join doc in _context.DocumentTypes on opv.DocumentID equals doc.ID
                              let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == pcc.ID) ?? new Display()
                              let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == ssc.ID) ?? new Display()
                              let lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == lc.ID) ?? new Display()
                              where opv.Status == "open"
                              select new
                              {
                                  //Master
                                  VendorName = ven.Name,
                                  MasDocumentNo = $"{doc.Code}-{opv.Number}",
                                  MasDate = opv.PostingDate.ToString("MM-dd-yyy"),
                                  OverdueDays = (opv.Date.Date - DateTime.Now.Date).Days,
                                  MasTotal = pcc.Description + " " + _fncModule.ToCurrency(opv.TotalPayment, plCur.Amounts),
                                  MasBalanceDue = pcc.Description + " " + _fncModule.ToCurrency(opv.BalanceDue, plCur.Amounts),
                                  //Summary
                                  SumBalanceDueSSC = ssc.Description + " " + _fncModule.ToCurrency(SumBalanceDueSSC, sysCur.Amounts),
                                  SumBalanceDue = lc.Description + " " + _fncModule.ToCurrency(SumBalanceDue, lcCur.Amounts),
                                  DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                  DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                              };
            return Ok(Outgoingpay);
        }
        public IActionResult GetSummarySale()
        {
            List<DevSummarySale> devSummarySales = new();
            var list = (from r in _context.Receipt
                        select new DevSummarySale
                        {
                            DouType = "SP",
                            ReceiptNo = r.ReceiptNo,
                            GrandTotal = r.GrandTotal.ToString(),
                        }).ToList();

            var list2 = (from s in _context.SaleARs
                         select new DevSummarySale
                         {
                             DouType = "AR",
                             ReceiptNo = s.InvoiceNo,
                             GrandTotal = s.SubTotal.ToString(),
                         }).ToList();
            var allProducts = new List<DevSummarySale>(list.Count +
                                    list2.Count);
            allProducts.AddRange(list);
            allProducts.AddRange(list2);

            return Ok(allProducts);
        }

        public IActionResult SaleCrditMemoReport(string DateFrom, string DateTo, string TimeFrom, string TimeTo, int BranchID, int UserID, string DouType)
        {
            List<ReceiptMemo> receiptsFilter = new();
            List<SaleCreditMemo> saleARs = new();
            if (DateFrom == null || DateTo == null)
            {
                return Ok(saleARs);
            }
            if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID == 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID != 0)
            {
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else if (DateFrom == null && DateTo == null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom == null && DateTo == null && TimeFrom == null && TimeTo == null && BranchID == 0 && UserID != 0)
            {
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && w.UserOrderID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID == 0 && UserID == 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));

                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID != 0 && UserID == 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));

                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID != 0 && UserID != 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
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
            var saleCreditMemoedetail = from s in receiptsFilter
                                        join sd in _context.ReceiptDetailMemoKvms on s.ReceiptKvmsID equals sd.ID
                                        select new
                                        {
                                            TotalDisItem = sd.DisValue * s.ExchangeRate
                                        };
            var TotalDisItemr = receiptsFilter.Sum(s => s.DisValue);
            var TotalDisTotalr = receiptsFilter.Sum(s => s.DisValue);
            var TotalVatr = receiptsFilter.Sum(s => s.TaxValue);
            var GrandTotalSysr = receiptsFilter.Sum(s => s.GrandTotalSys);
            var GrandTotalr = receiptsFilter.Sum(s => s.GrandTotalSys * s.LocalSetRate);
            var ReceiptsReturnr = receiptsFilter;
            var Receipts = receiptsFilter;
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
            var Sale = (from r in Receipts
                        join user in _context.UserAccounts on r.UserOrderID equals user.ID
                        join emp in _context.Employees on user.EmployeeID equals emp.ID
                        join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                        join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                        join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                        join b in _context.Branches on r.BranchID equals b.ID
                        join re in _context.Receipt on r.BasedOn equals re.ReceiptID
                        group new { r, emp, curr_pl, curr_sys, curr, b, re } by new { r.BranchID, r.ID } into datas
                        let data = datas.FirstOrDefault()
                        let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "RP")
                        let sumByBranch = Receipts.Where(_r => _r.BranchID == data.r.BranchID).Sum(_as => _as.GrandTotalSys)
                        let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                        let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                        select new DevSummarySale
                        {
                            //detail
                            RefNo = "SP" + " " + data.re.ReceiptNo,
                            Reasons = data.r.Reason,
                            ReceiptID = data.r.SeriesDID,
                            DouType = douType.Code,
                            EmpCode = data.emp.Code,
                            EmpName = data.emp.Name,
                            BranchID = data.r.BranchID,
                            BranchName = data.b.Name,
                            ReceiptNo = data.r.ReceiptNo,
                            DateOut = data.r.DateOut.ToString("dd-MM-yyyy"),
                            TimeOut = data.r.TimeOut,
                            DiscountItem = _fncModule.ToCurrency(data.r.DisValue, plCur.Amounts),
                            Currency = data.curr_pl.Description,
                            GrandTotal = _fncModule.ToCurrency(data.r.GrandTotal, plCur.Amounts),
                            //Summary
                            DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                            DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                            //SCount = ChCount.ToString(),
                            GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                            //SCount = string.Format("{0:n0}", Summary.FirstOrDefault().CountReceipt),
                            SDiscountItem = _fncModule.ToCurrency(TotalDisItemr, sysCur.Amounts),
                            SDiscountTotal = _fncModule.ToCurrency(TotalDisTotalr, sysCur.Amounts),
                            SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(TotalVatr, sysCur.Amounts),
                            SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(GrandTotalSysr, sysCur.Amounts),
                            SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(GrandTotalr, lcCur.Amounts),
                            //
                            DiscountTotal = data.r.DisValue,
                            Vat = data.r.DisValue * data.r.ExchangeRate,
                            GrandTotalSys = data.r.GrandTotalSys,
                            MGrandTotal = data.r.GrandTotalSys * data.r.LocalSetRate,
                        }).ToList();
            double TotalDisItem = 0;
            double TotalDisTotal = 0;
            double TotalVat = 0;
            double GrandTotalSys = 0;
            double GrandTotal = 0;
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0)
            {
                saleARs = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0)
            {
                saleARs = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0)
            {
                saleARs = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom == null && DateTo == null && BranchID != 0 && UserID == 0)
            {
                saleARs = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom == null && DateTo == null && BranchID == 0 && UserID != 0)
            {
                saleARs = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.UserID == UserID).ToList();
            }
            var saleARDetail = from s in saleARs
                               join sd in _context.SaleARDetails on s.SCMOID equals sd.SARID
                               select new
                               {
                                   TotalDisItem = sd.DisValue * s.ExchangeRate
                               };
            TotalDisItem = saleARDetail.Sum(s => s.TotalDisItem);
            TotalDisTotal = saleARs.Sum(s => s.DisValue);
            TotalVat = saleARs.Sum(s => s.DisValue);
            GrandTotalSys = saleARs.Sum(s => s.TotalAmountSys);
            GrandTotal = saleARs.Sum(s => s.TotalAmountSys * s.LocalSetRate);
            //
            var saleARSummary = saleARs;
            var saleAR = (from sale in saleARSummary
                          join user in _context.UserAccounts on sale.UserID equals user.ID
                          join com in _context.Company on user.CompanyID equals com.ID
                          join emp in _context.Employees on user.EmployeeID equals emp.ID
                          join curr_pl in _context.Currency on sale.SaleCurrencyID equals curr_pl.ID
                          join curr in _context.Currency on sale.LocalCurID equals curr.ID
                          join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                          join douType in _context.DocumentTypes on sale.DocTypeID equals douType.ID
                          join re in _context.SaleARs on sale.BasedOn equals re.SARID
                          join b in _context.Branches on sale.BranchID equals b.ID
                          group new { sale, emp, curr_pl, curr_sys, curr, douType, b, re } by new { sale.BranchID, sale.SCMOID } into datas
                          let data = datas.FirstOrDefault()
                          let sumByBranch = saleARSummary.Where(_r => _r.BranchID == data.sale.BranchID).Sum(_as => _as.TotalAmountSys)
                          let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                          let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                          select new DevSummarySale
                          {
                              //detail
                              RefNo = "CN" + " " + data.re.InvoiceNo,
                              ReceiptID = data.sale.SeriesDID,
                              DouType = data.douType.Code,
                              EmpCode = data.emp.Code,
                              EmpName = data.emp.Name,
                              BranchID = data.sale.BranchID,
                              BranchName = data.b.Name,
                              ReceiptNo = data.sale.InvoiceNo,
                              DateOut = data.sale.PostingDate.ToString("dd-MM-yyyy"),
                              TimeOut = "",
                              DiscountItem = _fncModule.ToCurrency(data.sale.DisValue, plCur.Amounts),
                              Currency = data.curr_pl.Description,
                              GrandTotal = _fncModule.ToCurrency(data.sale.TotalAmount, plCur.Amounts),
                              //Summary
                              DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                              DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                              //SCount = ChCount.ToString(),
                              GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                              SDiscountItem = _fncModule.ToCurrency(TotalDisItem, sysCur.Amounts),
                              SDiscountTotal = _fncModule.ToCurrency(TotalDisTotal, sysCur.Amounts),
                              SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(TotalVat, sysCur.Amounts),
                              SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(GrandTotalSys, sysCur.Amounts),
                              SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(GrandTotal, plCur.Amounts),
                              //
                              DiscountTotal = data.sale.DisValue,
                              Vat = data.sale.VatValue * data.sale.ExchangeRate,
                              GrandTotalSys = data.sale.TotalAmountSys,
                              MGrandTotal = data.sale.TotalAmountSys * data.sale.LocalSetRate,
                          }).ToList();
            if (DouType == "RP")
            {
                return Ok(Sale.OrderBy(o => o.DateOut));
            }
            else if (DouType == "CN")
            {
                return Ok(saleAR.OrderBy(o => o.DateOut));
            }
            else
            {
                var allSummarySale = new List<DevSummarySale>
                (Sale.Count + saleAR.Count);
                allSummarySale.AddRange(Sale);
                allSummarySale.AddRange(saleAR);
                var allSale = (from all in allSummarySale
                               join b in _context.Branches on all.BranchID equals b.ID
                               join c in _context.Company on b.CompanyID equals c.ID
                               join curr_sys in _context.Currency on c.SystemCurrencyID equals curr_sys.ID
                               join curr in _context.Currency on c.LocalCurrencyID equals curr.ID
                               group new { all, b, curr_sys, curr } by new { all.BranchID, all.ReceiptID } into r
                               let data = r.FirstOrDefault()
                               let sumByBranch = allSummarySale.Where(_r => _r.BranchID == data.all.BranchID).Sum(_as => _as.GrandTotalSys)
                               let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr.ID) ?? new Display()
                               let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                               select new
                               {
                                   data.all.RefNo,
                                   data.all.DouType,
                                   data.all.EmpCode,
                                   data.all.EmpName,
                                   data.all.BranchID,
                                   data.b.Name,
                                   data.all.ReceiptNo,
                                   data.all.DateOut,
                                   data.all.TimeOut,
                                   data.all.DiscountItem,
                                   data.all.Currency,
                                   data.all.GrandTotal,
                                   data.all.Reasons,
                                   //Summary
                                   DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                   DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                   //SCount = ChCount.ToString(),
                                   GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                                   SDiscountItem = _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                                   SDiscountTotal = _fncModule.ToCurrency(r.Sum(s => s.all.DiscountTotal), sysCur.Amounts),
                                   SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(allSummarySale.Sum(v => v.Vat), sysCur.Amounts),
                                   SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(allSummarySale.Sum(_r => _r.GrandTotalSys), sysCur.Amounts),
                                   SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(allSummarySale.Sum(_r => _r.MGrandTotal), plCur.Amounts),
                               });
                return Ok(allSale.OrderBy(o => o.DateOut));
            }
        }

        public IActionResult InventoryByUom()
        {
            int userid = int.Parse(User.FindFirst("UserID").Value);
            var permistion = _context.UserPrivilleges.FirstOrDefault(x => x.UserID == userid && x.Code == "IR001");
            if (permistion != null)
            {
                if (permistion.Used == true)
                {
                    ViewBag.InventoryByUom = "highlight";
                    return View();
                }
                else
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }
        public IActionResult GetInventoryByUom(int BranchID, int WarehouseID, int UOM)
        {
            if (BranchID == 0 && WarehouseID == 0)
            {
                if (UOM == 1)
                {
                    var items = from whs in _context.WarehouseSummary
                                join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                                //join gduom in _context.GroupDUoMs on item.GroupUomID equals gduom.GroupUoMID
                                //join uom in _context.UnitofMeasures on gduom.AltUOM equals uom.ID
                                join wh in _context.Warehouses on whs.WarehouseID equals wh.ID
                                let gduom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID)
                                let uom = _context.UnitofMeasures.FirstOrDefault(w => w.ID == gduom.AltUOM)
                                orderby item.Code
                                select new
                                {
                                    item.Barcode,
                                    item.Code,
                                    item.ID,
                                    item.KhmerName,
                                    item.EnglishName,
                                    InStock = ((whs.InStock == 0) ? 0 : whs.InStock / gduom.Factor).ToString() + "  " + uom.Name,
                                    Uom = uom.Name,
                                    WhCode = wh.Code
                                };
                    return Ok(items.ToList());
                }
                else if (UOM == 2)
                {
                    var items = from whs in _context.WarehouseSummary
                                join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                                //join gduom in _context.GroupDUoMs on item.GroupUomID equals gduom.GroupUoMID
                                //join uom in _context.UnitofMeasures on gduom.AltUOM equals uom.ID
                                join wh in _context.Warehouses on whs.WarehouseID equals wh.ID
                                let gduom = _context.GroupDUoMs.Where(w => w.GroupUoMID == item.GroupUomID).DefaultIfEmpty().Last()
                                let uom = _context.UnitofMeasures.FirstOrDefault(w => w.ID == gduom.AltUOM)
                                select new
                                {
                                    item.Barcode,
                                    item.Code,
                                    item.ID,
                                    item.KhmerName,
                                    item.EnglishName,
                                    InStock = ((whs.InStock == 0) ? 0 : whs.InStock / gduom.Factor).ToString() + "  " + uom.Name,
                                    Uom = uom.Name,
                                    WhCode = wh.Code
                                };
                    return Ok(items.ToList());
                }
                else
                {
                    var items = from whs in _context.WarehouseSummary
                                join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                                join gduom in _context.GroupDUoMs on item.GroupUomID equals gduom.GroupUoMID
                                join uom in _context.UnitofMeasures on gduom.AltUOM equals uom.ID
                                join wh in _context.Warehouses on whs.WarehouseID equals wh.ID
                                orderby item.Code
                                select new
                                {
                                    item.Barcode,
                                    item.Code,
                                    item.ID,
                                    item.KhmerName,
                                    item.EnglishName,
                                    InStock = ((whs.InStock == 0) ? 0 : whs.InStock / gduom.Factor).ToString() + "  " + uom.Name,
                                    Uom = uom.Name,
                                    WhCode = wh.Code
                                };
                    return Ok(items.ToList());
                }
            }
            else if (BranchID != 0 && WarehouseID == 0)
            {
                if (UOM == 1)
                {
                    var items = from wh in _context.Warehouses.Where(w => w.BranchID == BranchID)
                                join whs in _context.WarehouseSummary on wh.ID equals whs.WarehouseID
                                join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                                //join gduom in _context.GroupDUoMs on item.GroupUomID equals gduom.GroupUoMID
                                //join uom in _context.UnitofMeasures on gduom.AltUOM equals uom.ID
                                let gduom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID)
                                let uom = _context.UnitofMeasures.FirstOrDefault(w => w.ID == gduom.AltUOM)
                                orderby item.Code
                                select new
                                {
                                    item.Barcode,
                                    item.Code,
                                    item.ID,
                                    item.KhmerName,
                                    item.EnglishName,
                                    InStock = ((whs.InStock == 0) ? 0 : whs.InStock / gduom.Factor).ToString() + "  " + uom.Name,
                                    Uom = uom.Name,
                                    WhCode = wh.Code
                                };
                    return Ok(items.ToList());
                }
                else if (UOM == 2)
                {
                    var items = from wh in _context.Warehouses.Where(w => w.BranchID == BranchID)
                                join whs in _context.WarehouseSummary on wh.ID equals whs.WarehouseID
                                join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                                //join gduom in _context.GroupDUoMs on item.GroupUomID equals gduom.GroupUoMID
                                //join uom in _context.UnitofMeasures on gduom.AltUOM equals uom.ID
                                let gduom = _context.GroupDUoMs.AsEnumerable().LastOrDefault(w => w.GroupUoMID == item.GroupUomID)
                                let uom = _context.UnitofMeasures.AsEnumerable().LastOrDefault(w => w.ID == gduom.AltUOM)
                                orderby item.Code
                                select new
                                {
                                    item.Barcode,
                                    item.Code,
                                    item.ID,
                                    item.KhmerName,
                                    item.EnglishName,
                                    InStock = ((whs.InStock == 0) ? 0 : whs.InStock / gduom.Factor).ToString() + "  " + uom.Name,
                                    Uom = uom.Name,
                                    WhCode = wh.Code
                                };
                    return Ok(items.ToList());
                }
                else
                {
                    var items = from wh in _context.Warehouses.Where(w => w.BranchID == BranchID)
                                join whs in _context.WarehouseSummary on wh.ID equals whs.WarehouseID
                                join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                                join gduom in _context.GroupDUoMs on item.GroupUomID equals gduom.GroupUoMID
                                join uom in _context.UnitofMeasures on gduom.AltUOM equals uom.ID
                                orderby item.Code
                                select new
                                {
                                    item.Barcode,
                                    item.Code,
                                    item.ID,
                                    item.KhmerName,
                                    item.EnglishName,
                                    InStock = ((whs.InStock == 0) ? 0 : whs.InStock / gduom.Factor).ToString() + "  " + uom.Name,
                                    Uom = uom.Name,
                                    WhCode = wh.Code
                                };
                    return Ok(items.ToList());
                }
            }
            else if (BranchID != 0 && WarehouseID != 0)
            {
                if (UOM == 1)
                {
                    var items = from wh in _context.Warehouses.Where(w => w.BranchID == BranchID && w.ID == WarehouseID)
                                join whs in _context.WarehouseSummary on wh.ID equals whs.WarehouseID
                                join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                                //join gduom in _context.GroupDUoMs on item.GroupUomID equals gduom.GroupUoMID
                                //join uom in _context.UnitofMeasures on gduom.AltUOM equals uom.ID
                                let gduom = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID)
                                let uom = _context.UnitofMeasures.FirstOrDefault(w => w.ID == gduom.AltUOM)
                                orderby item.Code
                                select new
                                {
                                    item.Barcode,
                                    item.Code,
                                    item.ID,
                                    item.KhmerName,
                                    item.EnglishName,
                                    InStock = ((whs.InStock == 0) ? 0 : whs.InStock / gduom.Factor).ToString() + "  " + uom.Name,
                                    Uom = uom.Name,
                                    WhCode = wh.Code
                                };
                    return Ok(items.ToList());
                }
                else if (UOM == 2)
                {
                    var items = from wh in _context.Warehouses.Where(w => w.BranchID == BranchID && w.ID == WarehouseID)
                                join whs in _context.WarehouseSummary on wh.ID equals whs.WarehouseID
                                join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                                //join gduom in _context.GroupDUoMs on item.GroupUomID equals gduom.GroupUoMID
                                //join uom in _context.UnitofMeasures on gduom.AltUOM equals uom.ID
                                let gduom = _context.GroupDUoMs.LastOrDefault(w => w.GroupUoMID == item.GroupUomID)
                                let uom = _context.UnitofMeasures.LastOrDefault(w => w.ID == gduom.AltUOM)
                                orderby item.Code
                                select new
                                {
                                    item.Barcode,
                                    item.Code,
                                    item.ID,
                                    item.KhmerName,
                                    item.EnglishName,
                                    InStock = ((whs.InStock == 0) ? 0 : whs.InStock / gduom.Factor).ToString() + "  " + uom.Name,
                                    Uom = uom.Name,
                                    WhCode = wh.Code
                                };
                    return Ok(items.ToList());
                }
                else
                {
                    var items = from wh in _context.Warehouses.Where(w => w.BranchID == BranchID && w.ID == WarehouseID)
                                join whs in _context.WarehouseSummary on wh.ID equals whs.WarehouseID
                                join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                                join gduom in _context.GroupDUoMs on item.GroupUomID equals gduom.GroupUoMID
                                join uom in _context.UnitofMeasures on gduom.AltUOM equals uom.ID
                                orderby item.Code
                                select new
                                {
                                    item.Barcode,
                                    item.Code,
                                    item.ID,
                                    item.KhmerName,
                                    item.EnglishName,
                                    InStock = ((whs.InStock == 0) ? 0 : whs.InStock / gduom.Factor).ToString() + "  " + uom.Name,
                                    Uom = uom.Name,
                                    WhCode = wh.Code
                                };
                    return Ok(items.ToList());
                }
            }
            return Ok();
        }

        [HttpGet]
        [Privilege("SR001")]
        public IActionResult SaleRevenue()
        {
            ViewBag.SaleRevenue = "highlight";
            return View();
        }


        public IActionResult GetEmpDelivery()
        {
            var list = _context.Employees.ToList();
            return Ok(list);
        }
        public IActionResult GetSaleRevenue(int typedate, string DateFrom, string DateTo, int BranchID, int UserID, int DeliveryID, string TimeFrom, string TimeTo)
        {
            TimeSpan _timeFrom = Convert.ToDateTime(TimeFrom).TimeOfDay;
            TimeSpan _timeTo = Convert.ToDateTime(TimeTo).TimeOfDay;
            DateTime _dateFrom = _fncModule.ConcatDateTime(DateFrom, TimeFrom);
            DateTime _dateTo = _fncModule.ConcatDateTime(DateTo, TimeTo);


            List<SaleAR> saleARs = new();
            List<SaleAREdite> saleAREditable = new();
            List<ARReserveInvoice> aRRInvoice = new();
            List<ARReserveInvoiceEditable> aRREditable = new();
            List<SaleCreditMemo> Creditmemo = new();

            List<Receipt> receipts = typedate == 1 ? _context.Receipt.Where(r => r.CompanyID == GetCompany().ID && _fncModule.IsBetweenDate(_dateFrom, _dateTo, r.DateOut)).ToList()
                                                       : _context.Receipt.Where(r => r.CompanyID == GetCompany().ID && _fncModule.IsBetweenDate(_dateFrom, _dateTo, r.PostingDate)).ToList();

            List<ReceiptMemo> receiptMemo = _context.ReceiptMemo.Where(r => r.CompanyID == GetCompany().ID && _fncModule.IsBetweenDate(_dateFrom, _dateTo, r.DateOut)).ToList();

            if (_timeFrom.Minutes > 0 || _timeTo.Minutes > 0)
            {
                receipts = typedate == 1 ? _context.Receipt.Where(r => _fncModule.IsBetweenDate(_dateFrom, _dateTo, _fncModule.ConcatDateTime(r.DateOut.ToString(), r.TimeOut))).ToList()
                                         : _context.Receipt.Where(r => _fncModule.IsBetweenDate(_dateFrom, _dateTo, _fncModule.ConcatDateTime(r.PostingDate.ToString(), r.TimeOut))).ToList();

                receiptMemo = _context.ReceiptMemo.Where(r => _fncModule.IsBetweenDate(_dateFrom, _dateTo, _fncModule.ConcatDateTime(r.DateOut.ToString(), r.TimeOut))).ToList();

            }


            int saleEmpID = 0;
            double totalDiscountValue = 0;
            double grandTotalSSC = 0;
            double grandTotalLC = 0;
            if (DateFrom == null || DateTo == null)
            {
                return Ok(saleARs);
            }
            if (UserID != 0)
            {
                var list = from user in _context.UserAccounts.Where(a => a.ID == UserID)
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           select new
                           {
                               SaleEmpID = emp.ID,
                           };
                saleEmpID = list.FirstOrDefault().SaleEmpID;
            }

            if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && DeliveryID == 0)
            {
                receipts = receipts.Where(s => s.BranchID == BranchID).ToList();
                receiptMemo = receiptMemo.Where(s => s.BranchID == BranchID).ToList();

            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && DeliveryID == 0)
            {
                receipts = receipts.Where(s => s.UserOrderID == UserID && s.BranchID == BranchID).ToList();
                receiptMemo = receiptMemo = receiptMemo.Where(s => s.BranchID == BranchID && s.UserOrderID == UserID).ToList();

            }
            totalDiscountValue = receipts.Sum(x => x.DiscountValue);
            grandTotalSSC = receipts.Sum(s => s.GrandTotal_Sys);
            grandTotalLC = receipts.Sum(s => s.GrandTotal_Sys * s.LocalSetRate);
            totalDiscountValue -= receiptMemo.Sum(x => x.DisValue);
            grandTotalSSC -= receiptMemo.Sum(s => s.GrandTotalSys);
            grandTotalLC -= receiptMemo.Sum(s => s.GrandTotalSys * s.LocalSetRate);

            var receiptDetail = (from s in receipts
                                 join sd in _context.ReceiptDetail on s.ReceiptID equals sd.ReceiptID
                                 select new ReportRevenueValues
                                 {
                                     TotalDiscountItem = sd.DiscountValue * s.ExchangeRate,
                                     DiscountTotal = s.DiscountValue,
                                     TotalVat = s.TaxValue,
                                     TotalGrandTotal = sd.Total_Sys * s.LocalSetRate,
                                     TotalGrandTotalSys = sd.Total_Sys
                                 }).ToList();

            var Receipts = receipts;
            var SaleReceipt = (from r in Receipts
                               join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                               join user in _context.UserAccounts on r.UserOrderID equals user.ID
                               join emp in _context.Employees on user.EmployeeID equals emp.ID
                               join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                               join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                               join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                               join b in _context.Branches on r.BranchID equals b.ID

                               group new { r, rd, emp, curr_pl, curr_sys, curr, b } by new { r.ReceiptID, rd.ItemID, rd.UnitPrice } into datas
                               let data = datas.FirstOrDefault()
                               let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "SP")
                               let sumByBranch = Receipts.Where(_r => _r.BranchID == data.r.BranchID).Sum(_as => _as.GrandTotal_Sys)
                               let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                               let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                               let uom = _context.GroupUOMs.FirstOrDefault(s => s.ID == data.rd.UomID)
                               select new DevSummarySale
                               {
                                   //master
                                   ItemID = data.rd.ItemID,
                                   ReceiptID = data.r.SeriesDID,
                                   DouType = douType.Code + "-" + douType.Name,
                                   EmpCode = data.emp.Code,
                                   EmpName = data.emp.Name,
                                   BranchID = data.r.BranchID,
                                   BranchName = data.b.Name,
                                   ReceiptNo = data.r.ReceiptNo,
                                   DateOut = typedate == 1 ? data.r.DateOut.ToString("dd-MM-yyyy") + " " + data.r.TimeOut : data.r.PostingDate.ToString("dd-MM-yyyy"),
                                   TimeOut = data.r.TimeOut,
                                   Currency = data.curr_pl.Description,
                                   //detail
                                   UnitPrice = data.rd.UnitPrice,
                                   Total = data.rd.Total,
                                   ItemCode = data.rd.Code,
                                   ItemNameKhmer = data.rd.KhmerName,
                                   ItemNameEng = data.rd.EnglishName,
                                   Qty = data.rd.Qty,
                                   Uom = uom.Name,
                                   DisItem = data.rd.DiscountValue,
                                   DiscountTotal = totalDiscountValue,
                                   Distotalin = data.r.DiscountValue,
                                   AmmountFreightss = data.r.AmountFreight,
                                   Remark = string.IsNullOrWhiteSpace(data.r.Remark) ? "" : data.r.Remark,
                                   //Summary
                                   DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                   DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                   //SCount = ChCount.ToString(),
                                   GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                                   GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency(data.r.GrandTotal_Sys, sysCur.Amounts),
                                   //SCount = string.Format("{0:n0}", Summary.FirstOrDefault().CountReceipt),  
                               }).ToList();

            var receiptMemoedetail = (from s in receiptMemo
                                      join sd in _context.ReceiptDetailMemoKvms on s.ID equals sd.ReceiptMemoID
                                      select new ReportRevenueValues
                                      {
                                          TotalDiscountItem = (sd.DisValue * s.ExchangeRate) * -1,
                                          DiscountTotal = s.DisValue * -1,
                                          TotalVat = s.TaxValue * -1,
                                          TotalGrandTotal = (sd.TotalSys * s.LocalSetRate) * -1,
                                          TotalGrandTotalSys = sd.TotalSys * -1
                                      }).ToList();

            var SaleMemo = (from r in receiptMemo
                            join rd in _context.ReceiptDetailMemoKvms on r.ID equals rd.ReceiptMemoID
                            join user in _context.UserAccounts on r.UserOrderID equals user.ID
                            join emp in _context.Employees on user.EmployeeID equals emp.ID
                            join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                            join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                            join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                            join b in _context.Branches on r.BranchID equals b.ID
                            group new { r, rd, emp, curr_pl, curr_sys, curr, b } by new { r.ID, rd.ItemID, rd.UnitPrice } into datas
                            let data = datas.FirstOrDefault()
                            let re = _context.Receipt.FirstOrDefault(s => s.ReceiptID == data.r.BasedOn) ?? new Receipt()
                            let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "RP") ?? new Models.Services.Administrator.SystemInitialization.DocumentType()
                            let sumByBranch = receiptMemo.Where(_r => _r.BranchID == data.r.BranchID).Sum(_as => _as.GrandTotalSys)
                            let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                            let uom = _context.GroupUOMs.FirstOrDefault(s => s.ID == data.rd.UomID) ?? new GroupUOM()
                            select new DevSummarySale
                            {
                                //detail
                                ItemID = data.rd.ItemID,
                                RefNo = "SP" + " " + re.ReceiptNo == "0" ? "" : data.r.ReceiptNo,
                                ReceiptID = data.r.SeriesDID,
                                DouType = douType.Code + "-" + douType.Name,
                                EmpCode = data.emp.Code,
                                EmpName = data.emp.Name,
                                BranchID = data.r.BranchID,
                                BranchName = data.b.Name,
                                ReceiptNo = data.r.ReceiptMemoNo,
                                DateOut = data.r.DateOut.ToString("dd-MM-yyyy") + " " + data.r.TimeOut,
                                TimeOut = data.r.TimeOut,
                                Currency = data.curr_pl.Description,

                                //detail
                                UnitPrice = data.rd.UnitPrice,
                                Total = data.rd.Total * -1,
                                ItemCode = data.rd.Code,
                                ItemNameKhmer = data.rd.KhmerName,
                                ItemNameEng = data.rd.EnglishName,
                                Qty = (data.rd.Qty) * -1,
                                Uom = uom.Name,
                                DisItem = (data.rd.DisValue) * -1,
                                Distotalin = (data.r.DisValue) * -1,
                                //Summary
                                DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                //SCount = ChCount.ToString(),
                                GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency((data.r.GrandTotalSys * -1), sysCur.Amounts),
                            }).ToList();
            saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && _fncModule.IsBetweenDate(_dateFrom, _dateTo, w.PostingDate)).ToList();
            saleAREditable = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && _fncModule.IsBetweenDate(_dateFrom, _dateTo, w.PostingDate)).ToList();
            Creditmemo = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && _fncModule.IsBetweenDate(_dateFrom, _dateTo, w.PostingDate)).ToList();
            aRREditable = _context.ARReserveInvoiceEditables.Where(w => w.CompanyID == GetCompany().ID && _fncModule.IsBetweenDate(_dateFrom, _dateTo, w.PostingDate)).ToList();
            aRRInvoice = _context.ARReserveInvoices.Where(w => w.CompanyID == GetCompany().ID && _fncModule.IsBetweenDate(_dateFrom, _dateTo, w.PostingDate)).ToList();

            if (BranchID != 0 && saleEmpID != 0 && DeliveryID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.BranchID == BranchID && w.SaleEmID == saleEmpID && w.ShippedBy == DeliveryID).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.BranchID == BranchID && w.SaleEmID == saleEmpID && w.ShippedBy == DeliveryID).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.BranchID == BranchID && w.SaleEmID == saleEmpID && w.ShippedBy == DeliveryID).ToList();
                aRREditable = _context.ARReserveInvoiceEditables.Where(w => w.BranchID == BranchID && w.SaleEmID == saleEmpID && w.ShippedBy == DeliveryID).ToList();
                aRRInvoice = _context.ARReserveInvoices.Where(w => w.BranchID == BranchID && w.SaleEmID == saleEmpID && w.ShippedBy == DeliveryID).ToList();
            }
            else if (BranchID != 0 && saleEmpID == 0 && DeliveryID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.BranchID == BranchID).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.BranchID == BranchID).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.BranchID == BranchID).ToList();
                aRREditable = _context.ARReserveInvoiceEditables.Where(w => w.BranchID == BranchID).ToList();
                aRRInvoice = _context.ARReserveInvoices.Where(w => w.BranchID == BranchID).ToList();
            }
            else if (BranchID != 0 && saleEmpID == 0 && DeliveryID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.BranchID == BranchID && w.ShippedBy == DeliveryID).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.BranchID == BranchID && w.ShippedBy == DeliveryID).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.BranchID == BranchID && w.ShippedBy == DeliveryID).ToList();

                aRREditable = _context.ARReserveInvoiceEditables.Where(w => w.BranchID == BranchID && w.ShippedBy == DeliveryID).ToList();
                aRRInvoice = _context.ARReserveInvoices.Where(w => w.BranchID == BranchID && w.ShippedBy == DeliveryID).ToList();
            }
            else if (BranchID != 0 && saleEmpID != 0 && DeliveryID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.BranchID == BranchID && w.SaleEmID == saleEmpID).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.BranchID == BranchID && w.SaleEmID == saleEmpID).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.BranchID == BranchID && w.SaleEmID == saleEmpID).ToList();
                aRREditable = _context.ARReserveInvoiceEditables.Where(w => w.BranchID == BranchID && w.SaleEmID == saleEmpID).ToList();
                aRRInvoice = _context.ARReserveInvoices.Where(w => w.BranchID == BranchID && w.SaleEmID == saleEmpID).ToList();
            }
            else if (BranchID == 0 && saleEmpID == 0 && DeliveryID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.ShippedBy == DeliveryID).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.ShippedBy == DeliveryID).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.ShippedBy == DeliveryID).ToList();
                aRREditable = _context.ARReserveInvoiceEditables.Where(w => w.ShippedBy == DeliveryID).ToList();
                aRRInvoice = _context.ARReserveInvoices.Where(w => w.ShippedBy == DeliveryID).ToList();

            }
            totalDiscountValue += saleARs.Sum(x => x.DisValue);
            grandTotalSSC += saleARs.Sum(s => s.TotalAmountSys);
            grandTotalLC += saleARs.Sum(s => s.TotalAmountSys * s.LocalSetRate);

            totalDiscountValue += saleAREditable.Sum(x => x.DisValue);
            grandTotalSSC += saleAREditable.Sum(s => s.TotalAmountSys);
            grandTotalLC += saleAREditable.Sum(s => s.TotalAmountSys * s.LocalSetRate);

            totalDiscountValue -= Creditmemo.Sum(x => x.DisValue);
            grandTotalSSC -= Creditmemo.Sum(s => s.TotalAmountSys);
            grandTotalLC -= Creditmemo.Sum(s => s.TotalAmountSys * s.LocalSetRate);

            totalDiscountValue += aRREditable.Sum(x => x.DisValue);
            grandTotalSSC += aRREditable.Sum(s => s.TotalAmountSys);
            grandTotalLC += aRREditable.Sum(s => s.TotalAmountSys * s.LocalSetRate);

            totalDiscountValue += aRRInvoice.Sum(x => x.DisValue);
            grandTotalSSC += aRRInvoice.Sum(s => s.TotalAmountSys);
            grandTotalLC += aRRInvoice.Sum(s => s.TotalAmountSys * s.LocalSetRate);

            var saleARDetail = (from s in saleARs
                                join sd in _context.SaleARDetails on s.SARID equals sd.SARID
                                select new ReportRevenueValues
                                {
                                    TotalDiscountItem = sd.DisValue * s.ExchangeRate,
                                    DiscountTotal = s.DisValue,
                                    TotalVat = s.VatValue,
                                    TotalGrandTotal = sd.TotalSys * s.LocalSetRate,
                                    TotalGrandTotalSys = sd.TotalSys
                                }).ToList();

            var saleARSummary = saleARs;
            var saleAR = (from sale in saleARSummary
                          join saledetail in _context.SaleARDetails on sale.SARID equals saledetail.SARID
                          join user in _context.UserAccounts on sale.UserID equals user.ID
                          join com in _context.Company on user.CompanyID equals com.ID
                          join curr_pl in _context.Currency on sale.SaleCurrencyID equals curr_pl.ID
                          join curr in _context.Currency on sale.LocalCurID equals curr.ID
                          join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                          join douType in _context.DocumentTypes on sale.DocTypeID equals douType.ID
                          join b in _context.Branches on sale.BranchID equals b.ID
                          group new { sale, saledetail, curr_pl, curr_sys, curr, douType, b, user } by new { sale.SARID, saledetail.ItemID, saledetail.UnitPrice } into datas
                          let data = datas.FirstOrDefault()
                          let emp = _context.Employees.FirstOrDefault(s => s.ID == data.sale.SaleEmID) ?? new Employee()
                          let ship = _context.Employees.FirstOrDefault(s => s.ID == data.sale.ShippedBy) ?? new Employee()
                          let sumByBranch = saleARSummary.Where(_r => _r.BranchID == data.sale.BranchID).Sum(_as => _as.TotalAmountSys)
                          let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                          let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                          select new DevSummarySale
                          {
                              //detail
                              ItemID = data.saledetail.ItemID,
                              ReceiptID = data.sale.SeriesDID,
                              DouType = data.douType.Code + "-" + data.douType.Name,
                              EmpCode = emp.Code,
                              EmpName = emp.Name,
                              ShipBy = ship.Name,
                              BranchID = data.sale.BranchID,
                              BranchName = data.b.Name,
                              ReceiptNo = data.sale.InvoiceNo,
                              DateOut = data.sale.PostingDate.ToString("dd-MM-yyyy"),
                              TimeOut = "",
                              Currency = data.curr_pl.Description,
                              //detail
                              UnitPrice = data.saledetail.UnitPrice,
                              Total = data.saledetail.Total,
                              ItemCode = data.saledetail.ItemCode,
                              ItemNameKhmer = data.saledetail.ItemNameKH,
                              ItemNameEng = data.saledetail.ItemNameEN,
                              Qty = data.saledetail.Qty,
                              Uom = data.saledetail.UomName,
                              DisItem = data.saledetail.DisValue,
                              Distotalin = data.sale.DisValue,
                              AmmountFreightss = data.sale.FreightAmount,
                              //Summary
                              DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                              DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                              //SCount = ChCount.ToString(),
                              GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency(data.sale.TotalAmountSys, sysCur.Amounts),
                          }).ToList();

            var saleAREditableDetail = (from s in saleAREditable
                                        join sd in _context.SaleAREditeDetails on s.SARID equals sd.SARID
                                        select new ReportRevenueValues
                                        {
                                            TotalDiscountItem = sd.DisValue * s.ExchangeRate,
                                            DiscountTotal = s.DisValue,
                                            TotalVat = s.VatValue,
                                            TotalGrandTotal = sd.TotalSys * s.LocalSetRate,
                                            TotalGrandTotalSys = sd.TotalSys
                                        }).ToList();


            var saleAREditSummary = saleAREditable;
            var saleAREdit = (from sale in saleAREditSummary
                              join saledetail in _context.SaleAREditeDetails on sale.SARID equals saledetail.SARID
                              join user in _context.UserAccounts on sale.UserID equals user.ID
                              join com in _context.Company on user.CompanyID equals com.ID
                              join curr_pl in _context.Currency on sale.SaleCurrencyID equals curr_pl.ID
                              join curr in _context.Currency on sale.LocalCurID equals curr.ID
                              join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                              join douType in _context.DocumentTypes on sale.DocTypeID equals douType.ID
                              join b in _context.Branches on sale.BranchID equals b.ID
                              group new { sale, saledetail, curr_pl, curr_sys, curr, douType, b, user } by new { sale.SARID, saledetail.ItemID, saledetail.UnitPrice } into datas
                              let data = datas.FirstOrDefault()
                              let emp = _context.Employees.FirstOrDefault(s => s.ID == data.sale.SaleEmID) ?? new Employee()
                              let ship = _context.Employees.FirstOrDefault(s => s.ID == data.sale.ShippedBy) ?? new Employee()
                              let sumByBranch = saleARSummary.Where(_r => _r.BranchID == data.sale.BranchID).Sum(_as => _as.TotalAmountSys)
                              let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                              let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                              select new DevSummarySale
                              {
                                  //detail
                                  ItemID = data.saledetail.ItemID,
                                  ReceiptID = data.sale.SeriesDID,
                                  DouType = data.douType.Code + "-" + data.douType.Name + "" + "Editable",
                                  EmpCode = emp.Code,
                                  EmpName = emp.Name,
                                  ShipBy = ship.Name,
                                  BranchID = data.sale.BranchID,
                                  BranchName = data.b.Name,
                                  ReceiptNo = data.sale.InvoiceNo,
                                  DateOut = data.sale.PostingDate.ToString("dd-MM-yyyy"),
                                  TimeOut = "",
                                  Currency = data.curr_pl.Description,
                                  //detail
                                  UnitPrice = data.saledetail.UnitPrice,
                                  Total = data.saledetail.Total,
                                  ItemCode = data.saledetail.ItemCode,
                                  ItemNameKhmer = data.saledetail.ItemNameKH,
                                  ItemNameEng = data.saledetail.ItemNameEN,
                                  Qty = data.saledetail.Qty,
                                  Uom = data.saledetail.UomName,
                                  DisItem = data.saledetail.DisValue,
                                  Distotalin = data.sale.DisValue,
                                  AmmountFreightss = data.sale.FreightAmount,
                                  //Summary
                                  DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                  DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                  //SCount = ChCount.ToString(),
                                  GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency(data.sale.TotalAmountSys, sysCur.Amounts),
                              }).ToList();
            //Memo
            var CreditMemoedetail = (from s in Creditmemo
                                     join sd in _context.SaleCreditMemoDetails on s.SCMOID equals sd.SCMOID
                                     select new ReportRevenueValues
                                     {
                                         TotalDiscountItem = (sd.DisValue * s.ExchangeRate) * -1,
                                         DiscountTotal = s.DisValue * -1,
                                         TotalVat = s.VatValue * -1,
                                         TotalGrandTotal = (sd.TotalSys * s.LocalSetRate) * -1,
                                         TotalGrandTotalSys = sd.TotalSys * -1
                                     }).ToList();

            var SaleCreditMemo = (from sale in Creditmemo
                                  join saledetail in _context.SaleCreditMemoDetails on sale.SCMOID equals saledetail.SCMOID
                                  join user in _context.UserAccounts on sale.UserID equals user.ID
                                  join com in _context.Company on user.CompanyID equals com.ID
                                  join curr_pl in _context.Currency on sale.SaleCurrencyID equals curr_pl.ID
                                  join curr in _context.Currency on sale.LocalCurID equals curr.ID
                                  join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                                  join douType in _context.DocumentTypes on sale.DocTypeID equals douType.ID
                                  join b in _context.Branches on sale.BranchID equals b.ID
                                  group new { sale, saledetail, curr_pl, curr_sys, curr, douType, b, user } by new { sale.SCMOID, saledetail.ItemID, saledetail.UnitPrice } into datas
                                  let data = datas.FirstOrDefault()
                                  let re = _context.SaleARs.FirstOrDefault(s => s.SARID == data.sale.BasedOn) ?? new SaleAR()
                                  let emp = _context.Employees.FirstOrDefault(s => s.ID == data.sale.SaleEmID) ?? new Employee()
                                  let ship = _context.Employees.FirstOrDefault(s => s.ID == data.sale.ShippedBy) ?? new Employee()
                                  let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                                  let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                                  select new DevSummarySale
                                  {
                                      //detail
                                      ItemID = data.saledetail.ItemID,
                                      RefNo = "IN" + " " + re.InvoiceNo,
                                      ReceiptID = data.sale.SeriesDID,
                                      DouType = data.douType.Code + "-" + data.douType.Name,
                                      EmpCode = emp.Code,
                                      EmpName = emp.Name,
                                      ShipBy = ship.Name,
                                      BranchID = data.sale.BranchID,
                                      BranchName = data.b.Name,
                                      ReceiptNo = data.sale.InvoiceNo,
                                      DateOut = data.sale.PostingDate.ToString("dd-MM-yyyy"),
                                      TimeOut = "",
                                      Currency = data.curr_pl.Description,
                                      //detail 
                                      UnitPrice = data.saledetail.UnitPrice,
                                      Total = data.saledetail.Total * -1,
                                      ItemCode = data.saledetail.ItemCode,
                                      ItemNameKhmer = data.saledetail.ItemNameKH,
                                      ItemNameEng = data.saledetail.ItemNameEN,
                                      Qty = data.saledetail.Qty * -1,
                                      Uom = data.saledetail.UomName,
                                      DisItem = data.saledetail.DisValue * -1,
                                      Distotalin = data.sale.DisValue * -1,
                                      AmmountFreightss = data.sale.FreightAmount,
                                      //Summary
                                      DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                      DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                      //SCount = ChCount.ToString(),              
                                      GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency((data.sale.TotalAmountSys * -1), sysCur.Amounts),
                                  }).ToList();
            // ARReserve Editable
            var aARRDetail = (from s in aRREditable
                              join sd in _context.ARReserveInvoiceEditableDetails on s.ID equals sd.ARReserveInvoiceEditableID
                              select new ReportRevenueValues
                              {
                                  TotalDiscountItem = sd.DisValue * s.ExchangeRate,
                                  DiscountTotal = s.DisValue,
                                  TotalVat = s.VatValue,
                                  TotalGrandTotal = sd.TotalSys * s.LocalSetRate,
                                  TotalGrandTotalSys = sd.TotalSys
                              }).ToList();

            // ARReserve Editable
            var aRREDT = (from arrEDT in aRREditable
                          join arredtDetail in _context.ARReserveInvoiceEditableDetails on arrEDT.ID equals arredtDetail.ARReserveInvoiceEditableID
                          join user in _context.UserAccounts on arrEDT.UserID equals user.ID
                          join com in _context.Company on user.CompanyID equals com.ID
                          join curr_pl in _context.Currency on arrEDT.SaleCurrencyID equals curr_pl.ID
                          join curr in _context.Currency on arrEDT.LocalCurID equals curr.ID
                          join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                          join douType in _context.DocumentTypes on arrEDT.DocTypeID equals douType.ID
                          join b in _context.Branches on arrEDT.BranchID equals b.ID
                          group new { arrEDT, arredtDetail, curr_pl, curr_sys, curr, douType, b, user } by new { arrEDT.ID, arredtDetail.ItemID, arredtDetail.UnitPrice } into datas
                          let data = datas.FirstOrDefault()
                          let emp = _context.Employees.FirstOrDefault(s => s.ID == data.arrEDT.SaleEmID) ?? new Employee()
                          let ship = _context.Employees.FirstOrDefault(s => s.ID == data.arrEDT.ShippedBy) ?? new Employee()
                          let sumByBranch = saleARSummary.Where(_r => _r.BranchID == data.arrEDT.BranchID).Sum(_as => _as.TotalAmountSys)
                          let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                          let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                          select new DevSummarySale
                          {
                              //detail
                              ItemID = data.arredtDetail.ItemID,
                              ReceiptID = data.arrEDT.SeriesDID,
                              DouType = data.douType.Code + "-" + data.douType.Name,
                              EmpCode = emp.Code,
                              EmpName = emp.Name,
                              ShipBy = ship.Name,
                              BranchID = data.arrEDT.BranchID,
                              BranchName = data.b.Name,
                              ReceiptNo = data.arrEDT.InvoiceNo,
                              DateOut = data.arrEDT.PostingDate.ToString("dd-MM-yyyy"),
                              TimeOut = "",
                              Currency = data.curr_pl.Description,
                              //detail
                              UnitPrice = data.arredtDetail.UnitPrice,
                              Total = data.arredtDetail.Total,
                              ItemCode = data.arredtDetail.ItemCode,
                              ItemNameKhmer = data.arredtDetail.ItemNameKH,
                              ItemNameEng = data.arredtDetail.ItemNameEN,
                              Qty = data.arredtDetail.Qty,
                              Uom = data.arredtDetail.UomName,
                              DisItem = data.arredtDetail.DisValue,
                              Distotalin = data.arrEDT.DisValue,
                              AmmountFreightss = data.arrEDT.FreightAmount,
                              //Summary
                              DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                              DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                              //SCount = ChCount.ToString(),
                              GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency(data.arrEDT.TotalAmountSys, sysCur.Amounts),
                          }).ToList();
            /// aARRInvoiceDetail
            var aARRInvoiceDetail = (from s in aRRInvoice
                                     join sd in _context.ARReserveInvoiceDetails on s.ID equals sd.ARReserveInvoiceID
                                     select new ReportRevenueValues
                                     {
                                         TotalDiscountItem = sd.DisValue * s.ExchangeRate,
                                         DiscountTotal = s.DisValue,
                                         TotalVat = s.VatValue,
                                         TotalGrandTotal = sd.TotalSys * s.LocalSetRate,
                                         TotalGrandTotalSys = sd.TotalSys
                                     }).ToList();
            // aARRInvoiceDetail
            var aRRIn = (from arrinvoice in aRRInvoice
                         join arrinvDetail in _context.ARReserveInvoiceDetails on arrinvoice.ID equals arrinvDetail.ARReserveInvoiceID
                         join user in _context.UserAccounts on arrinvoice.UserID equals user.ID
                         join com in _context.Company on user.CompanyID equals com.ID
                         join curr_pl in _context.Currency on arrinvoice.SaleCurrencyID equals curr_pl.ID
                         join curr in _context.Currency on arrinvoice.LocalCurID equals curr.ID
                         join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                         join douType in _context.DocumentTypes on arrinvoice.DocTypeID equals douType.ID
                         join b in _context.Branches on arrinvoice.BranchID equals b.ID
                         group new { arrinvoice, arrinvDetail, curr_pl, curr_sys, curr, douType, b, user } by new { arrinvoice.ID, arrinvDetail.ItemID, arrinvDetail.UnitPrice } into datas
                         let data = datas.FirstOrDefault()
                         let emp = _context.Employees.FirstOrDefault(s => s.ID == data.arrinvoice.SaleEmID) ?? new Employee()
                         let ship = _context.Employees.FirstOrDefault(s => s.ID == data.arrinvoice.ShippedBy) ?? new Employee()
                         let sumByBranch = saleARSummary.Where(_r => _r.BranchID == data.arrinvoice.BranchID).Sum(_as => _as.TotalAmountSys)
                         let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                         let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                         select new DevSummarySale
                         {
                             //detail
                             ItemID = data.arrinvDetail.ItemID,
                             ReceiptID = data.arrinvoice.SeriesDID,
                             DouType = data.douType.Code + "-" + data.douType.Name,
                             EmpCode = emp.Code,
                             EmpName = emp.Name,
                             ShipBy = ship.Name,
                             BranchID = data.arrinvoice.BranchID,
                             BranchName = data.b.Name,
                             ReceiptNo = data.arrinvoice.InvoiceNo,
                             DateOut = data.arrinvoice.PostingDate.ToString("dd-MM-yyyy"),
                             TimeOut = "",
                             Currency = data.curr_pl.Description,
                             //detail
                             UnitPrice = data.arrinvDetail.UnitPrice,
                             Total = data.arrinvDetail.Total,
                             ItemCode = data.arrinvDetail.ItemCode,
                             ItemNameKhmer = data.arrinvDetail.ItemNameKH,
                             ItemNameEng = data.arrinvDetail.ItemNameEN,
                             Qty = data.arrinvDetail.Qty,
                             Uom = data.arrinvDetail.UomName,
                             DisItem = data.arrinvDetail.DisValue,
                             Distotalin = data.arrinvoice.DisValue,
                             AmmountFreightss = data.arrinvoice.FreightAmount,
                             //Summary
                             DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                             DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                             //SCount = ChCount.ToString(),
                             GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency(data.arrinvoice.TotalAmountSys, sysCur.Amounts),
                         }).ToList();



            var allSummarySale = new List<DevSummarySale>(SaleReceipt.Count + saleAR.Count + saleAREdit.Count + SaleMemo.Count + SaleCreditMemo.Count + aRREDT.Count + aRRIn.Count);
            allSummarySale.AddRange(SaleReceipt);
            allSummarySale.AddRange(saleAR);
            allSummarySale.AddRange(saleAREdit);
            allSummarySale.AddRange(SaleMemo);
            allSummarySale.AddRange(SaleCreditMemo);
            allSummarySale.AddRange(aRREDT);
            allSummarySale.AddRange(aRRIn);

            var allSummaryValue = new List<ReportRevenueValues>(receiptDetail.Count + saleARDetail.Count + saleAREditableDetail.Count + receiptMemoedetail.Count + CreditMemoedetail.Count + aARRDetail.Count + aARRInvoiceDetail.Count);
            allSummaryValue.AddRange(receiptDetail);
            allSummaryValue.AddRange(saleARDetail);
            allSummaryValue.AddRange(saleAREditableDetail);
            allSummaryValue.AddRange(receiptMemoedetail);
            allSummaryValue.AddRange(CreditMemoedetail);
            allSummaryValue.AddRange(aARRDetail);
            allSummaryValue.AddRange(aARRInvoiceDetail);

            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
            var allSale = (from all in allSummarySale
                           join b in _context.Branches on all.BranchID equals b.ID
                           join c in _context.Company on b.CompanyID equals c.ID
                           join curr_sys in _context.Currency on c.SystemCurrencyID equals curr_sys.ID
                           join curr in _context.Currency on c.LocalCurrencyID equals curr.ID
                           group new { all, b, curr_sys, curr } by new { all.ReceiptID, all.ItemID, all.UnitPrice } into r
                           let data = r.FirstOrDefault()
                           let sumByBranch = allSummarySale.Where(_r => _r.ReceiptID == data.all.ReceiptID).Sum(_as => _as.GrandTotalSys)
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr.ID) ?? new Display()
                           //summary
                           let sumDisItem = allSummaryValue.Sum(s => s.TotalDiscountItem)
                           let sumDisTotal = totalDiscountValue
                           let sumVat = allSummaryValue.Sum(s => s.TotalVat)
                           //let sumGrandTotalSys = allSummaryValue.Sum(s => s.TotalGrandTotalSys)
                           let sumGrandTotalSys = grandTotalSSC
                           //    let sumGrandTotal = allSummaryValue.Sum(s => s.TotalGrandTotal)
                           let sumGrandTotal = grandTotalLC
                           select new
                           {
                               data.all.RefNo,
                               //    ReceiptNo=data.all.ReceiptNo + " " + "(" + data.all.DouType + ")" +,
                               ReceiptNo = data.all.Remark == "" ? data.all.ReceiptNo + " " + "(" + data.all.DouType + ")" : data.all.ReceiptNo + " " + "(" + data.all.DouType + ") " + " Remark :" + data.all.Remark,
                               data.all.EmpCode,
                               data.all.EmpName,
                               data.all.ShipBy,
                               data.all.BranchID,
                               BranchName = data.b.Name,
                               data.all.ReceiptID,
                               Invoice = data.all.ReceiptNo,
                               data.all.DateOut,
                               data.all.TimeOut,
                               Hide = "",
                               //detail
                               UnitPrice = _fncModule.ToCurrency(data.all.UnitPrice, sysCur.Amounts),
                               Total = _fncModule.ToCurrency(data.all.Total, sysCur.Amounts),
                               data.all.ItemCode,
                               data.all.ItemNameKhmer,
                               data.all.ItemNameEng,
                               data.all.Qty,
                               data.all.Uom,
                               data.all.Currency,
                               DisItem = _fncModule.ToCurrency(data.all.DisItem, sysCur.Amounts),
                               Distotalin = _fncModule.ToCurrency(data.all.Distotalin, sysCur.Amounts),
                               AmountFreightss = _fncModule.ToCurrency(data.all.AmmountFreightss, sysCur.Amounts),
                               data.all.Remark,
                               //Summary
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               //SCount = ChCount.ToString(),                              
                               data.all.GrandTotal,
                               GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                               SDiscountItem = _fncModule.ToCurrency(sumDisItem, sysCur.Amounts),
                               SDiscountTotal = _fncModule.ToCurrency(sumDisTotal, sysCur.Amounts),
                               SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumVat, sysCur.Amounts),
                               SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumGrandTotalSys, sysCur.Amounts),
                               SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(sumGrandTotal, lcCur.Amounts),
                           }).ToList();
            return Ok(allSale.OrderBy(o => o.DateOut));
        }
        public IActionResult GroupCustomerReport(string DateFrom, string DateTo, string TimeFrom, string TimeTo, int plid, string docType)
        {
            var data = _report.GetGroupCustomerReport(GetCompany(), DateFrom, DateTo, TimeFrom, TimeTo, plid, docType);
            return Ok(data);
        }
        public IActionResult CountMemberReport(string DateFrom, string DateTo, string TimeFrom, string TimeTo, int BranchID, int UserID, int plid)
        {
            List<Receipt> receiptsFilter = new();
            if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID == 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else if (DateFrom == null && DateTo == null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom == null && DateTo == null && TimeFrom == null && TimeTo == null && BranchID == 0 && UserID != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.UserOrderID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID == 0 && UserID == 0 && plid != 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));

                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID != 0 && UserID == 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));

                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID != 0 && UserID != 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));
                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }

            var sales = (from r in receiptsFilter
                         join user in _context.UserAccounts on r.UserOrderID equals user.ID
                         join emp in _context.Employees on user.EmployeeID equals emp.ID
                         join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                         join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                         join b in _context.Branches on r.BranchID equals b.ID
                         group new { r, emp, curr_pl, curr_sys, b } by new { r.BranchID, r.ReceiptID } into datas
                         let data = datas.FirstOrDefault()
                         let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "SP")
                         let sumByBranch = receiptsFilter.Where(_r => _r.BranchID == data.r.BranchID).Sum(_as => _as.GrandTotal_Sys)
                         let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                         let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                         select new DevCountMember
                         {
                             //detail
                             ReceiptID = data.r.SeriesDID,
                             Male = data.r.Male,
                             Female = data.r.Female,
                             Children = data.r.Children,
                             DouType = douType.Code,
                             EmpCode = data.emp.Code,
                             EmpName = data.emp.Name,
                             BranchID = data.r.BranchID,
                             BranchName = data.b.Name,
                             ReceiptNo = data.r.ReceiptNo,
                             DateOut = data.r.DateOut.ToString("dd-MM-yyyy"),
                             TimeOut = data.r.TimeOut,
                             DiscountItem = _fncModule.ToCurrency(data.r.DiscountValue, plCur.Amounts),
                             Currency = data.curr_pl.Description,
                             GrandTotal = _fncModule.ToCurrency(data.r.GrandTotal, plCur.Amounts),
                             DisRemark = data.r.RemarkDiscount,
                             //Summary
                             DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                             DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                             //SCount = ChCount.ToString(),
                             GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                             SumMale = _fncModule.ToCurrency(Convert.ToDouble(receiptsFilter.Sum(i => i.Male)), sysCur.Amounts),
                             SumFemale = _fncModule.ToCurrency(Convert.ToDouble(receiptsFilter.Sum(i => i.Female)), sysCur.Amounts),
                             SumChildren = _fncModule.ToCurrency(Convert.ToDouble(receiptsFilter.Sum(i => i.Children)), sysCur.Amounts)
                         }).ToList();

            return Ok(sales.OrderBy(o => o.DateOut));

        }
        #region Redeemed items      
        [HttpGet]
        [Privilege("BP005")]
        public IActionResult RedemPoint()
        {
            ViewBag.style = "fa fa-chart-line";
            ViewBag.Main = "Report";
            ViewBag.Page = "Sale";
            ViewBag.Subpage = "Summary Sale Report";
            ViewBag.Report = "show";
            ViewBag.Sale = "show";
            ViewBag.RedemPoint = "highlight";
            ViewBag.PriceLists = new SelectList(Getpriclist(), "ID", "Name");
            return View();
        }

        //Get Data
        [HttpGet]
        public IActionResult RedemPointView(string DateFrom, string DateTo, int BranchID, int user_id)
        {
            List<Redeem> redempoint = new();
            if (DateFrom != null && DateTo != null && BranchID == 0 && user_id == 0)
            {
                redempoint = _context.Redeems.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && user_id == 0)
            {
                redempoint = _context.Redeems.Where(w => w.BranchID == BranchID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && user_id != 0)
            {
                redempoint = _context.Redeems.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == user_id).ToList();
            }
            var rdds = from r in redempoint
                       join rdd in _context.RedeemDetails on r.ID equals rdd.RID
                       select rdd;
            var list = (from r in redempoint
                        join cus in _context.BusinessPartners on r.CustomerID equals cus.ID
                        join rd in _context.RedeemDetails on r.ID equals rd.RID
                        join item in _context.ItemMasterDatas.Where(s => !s.Delete) on rd.ItemID equals item.ID
                        select new
                        {
                            //detail
                            RedeemCode = r.Number,
                            DateOut = r.DateOut.ToString("MM/dd/yyyy"),
                            CusName = cus.Name,
                            RedeemPoint = r.RedeemPoint,
                            ItemCode = item.Code,
                            ItemName = item.KhmerName,
                            Qty = rd.Qty,
                            Uom = rd.Uom,
                            TotalQty = rdds.Sum(x => x.Qty),
                            TotalRedeempoint = redempoint.Sum(_r => _r.RedeemPoint)
                        }).ToList();
            return Ok(list);
        }
        #endregion  Redeemed items

        [HttpPost]
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        public IActionResult GetAllSelectedDisplay(FormDataSelection dataSelection)
        {
            ViewBag.DataSelectionExport = "highlight";
            if (dataSelection.DataSelections != null)
            {
                var item_process = dataSelection.DataSelections.Where(s => s.Process == "FIFO" || s.Process == "Average").ToList();

                var list = (from ds in dataSelection.DataSelections
                            join item in _context.PriceListDetails on ds.ItemID equals item.ItemID
                            group new { ds, item } by new { ds.LineID } into datas
                            let data = datas.FirstOrDefault()
                            select new
                            {
                                InvoiceNo = data.ds.InvoiceNo,
                                Process = data.ds.Process,
                                Currency = data.ds.Currency,
                                UnitPrice = data.ds.UnitPrice,
                                Total = data.ds.Total,
                                Cost = data.item.Cost,
                                ItemCode = data.ds.ItemCode,
                                ItemNameKhmer = data.ds.ItemNameKhmer,
                                ItemNameEng = data.ds.ItemNameEng,
                                Qty = data.ds.Qty,
                                Uom = data.ds.Uom,
                            }).ToList();

                return View(list);
            }
            else
            {
                var none = dataSelection.DataSelections = new();
                return View(none);
            }
        }

        public IActionResult GetDataSelectionExport(string DateFrom, string DateTo, int BranchID, int UserID, int DeliveryID)
        {
            int saleEmpID = 0;
            List<Receipt> receipts = new();
            List<SaleAR> saleARs = new();
            List<SaleAREdite> saleAREditable = new();
            List<SaleCreditMemo> Creditmemo = new();
            List<ReceiptMemo> receiptMemo = new();
            if (DateFrom == null || DateTo == null)
            {
                return Ok(saleARs);
            }
            if (UserID != 0)
            {
                var list = from user in _context.UserAccounts.Where(a => a.ID == UserID)
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           select new
                           {
                               SaleEmpID = emp.ID,
                           };
                saleEmpID = list.FirstOrDefault().SaleEmpID;
            }

            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && DeliveryID == 0)
            {
                receipts = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
                receiptMemo = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && DeliveryID == 0)
            {
                receipts = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
                receiptMemo = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && DeliveryID == 0)
            {
                receipts = _context.Receipt.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
                receiptMemo = _context.ReceiptMemo.Where(w => w.CompanyID == GetCompany().ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            var Receipts = receipts;
            var SaleReceipt = (from r in Receipts
                               join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                               join user in _context.UserAccounts on r.UserOrderID equals user.ID
                               join emp in _context.Employees on user.EmployeeID equals emp.ID
                               join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                               join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                               join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                               join item in _context.ItemMasterDatas on rd.ItemID equals item.ID
                               join b in _context.Branches on r.BranchID equals b.ID

                               group new { r, rd, emp, curr_pl, curr_sys, curr, b, item } by new { r.ReceiptID, rd.ItemID } into datas
                               let data = datas.FirstOrDefault()
                               let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "SP")
                               let sumByBranch = Receipts.Where(_r => _r.BranchID == data.r.BranchID).Sum(_as => _as.GrandTotal_Sys)
                               let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                               let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                               let uom = _context.GroupUOMs.FirstOrDefault(s => s.ID == data.rd.UomID)
                               select new DevSummarySale
                               {
                                   //master
                                   LineID = DateTime.Now.Ticks.ToString(),
                                   ItemID = data.rd.ItemID,
                                   ReceiptID = data.r.ReceiptID,
                                   DouType = douType.Code + "-" + douType.Name,
                                   EmpCode = data.emp.Code,
                                   EmpName = data.emp.Name,
                                   BranchID = data.r.BranchID,
                                   BranchName = data.b.Name,
                                   ReceiptNo = data.r.ReceiptNo,
                                   DateOut = data.r.DateOut.ToString("dd-MM-yyyy") + " " + data.r.TimeOut,
                                   TimeOut = data.r.TimeOut,
                                   Currency = data.curr_pl.Description,
                                   Process = data.item.Process,
                                   InvoiceNo = data.r.ReceiptNo,
                                   //detail
                                   UnitPrice = data.rd.UnitPrice,
                                   Total = data.rd.Total,
                                   ItemCode = data.rd.Code,
                                   ItemNameKhmer = data.rd.KhmerName,
                                   ItemNameEng = data.rd.EnglishName,
                                   Qty = data.rd.Qty,
                                   Uom = uom.Name,
                                   DisItem = data.rd.DiscountValue,
                                   //Summary
                                   DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                   DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                   GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                                   GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency(data.r.GrandTotal_Sys, sysCur.Amounts),
                                   TotalDiscountItem = Convert.ToDecimal(data.rd.DiscountValue * data.r.ExchangeRate),
                                   DiscountTotal = data.r.DiscountValue,
                                   TotalVat = data.r.TaxValue,
                                   TotalGrandTotal = data.rd.Total_Sys * data.r.LocalSetRate,
                                   TotalGrandTotalSys = data.rd.Total_Sys
                               }).ToList();

            var SaleMemo = (from r in receiptMemo
                            join rd in _context.ReceiptDetailMemoKvms on r.ID equals rd.ReceiptMemoID
                            join user in _context.UserAccounts on r.UserOrderID equals user.ID
                            join emp in _context.Employees on user.EmployeeID equals emp.ID
                            join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                            join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                            join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                            join b in _context.Branches on r.BranchID equals b.ID
                            join item in _context.ItemMasterDatas on rd.ItemID equals item.ID
                            group new { r, rd, emp, curr_pl, curr_sys, curr, b, item } by new { r.ID, rd.ItemID } into datas
                            let data = datas.FirstOrDefault()
                            let re = _context.Receipt.FirstOrDefault(s => s.ReceiptID == data.r.BasedOn) ?? new Receipt()
                            let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "RP")
                            let sumByBranch = receiptMemo.Where(_r => _r.BranchID == data.r.BranchID).Sum(_as => _as.GrandTotalSys)
                            let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                            let uom = _context.GroupUOMs.FirstOrDefault(s => s.ID == data.rd.UomID)
                            select new DevSummarySale
                            {
                                //detail
                                LineID = DateTime.Now.Ticks.ToString(),
                                ItemID = data.rd.ItemID,
                                RefNo = "SP" + " " + re.ReceiptNo == "0" ? "" : data.r.ReceiptNo,
                                ReceiptID = data.r.ID,
                                DouType = douType.Code + "-" + douType.Name,
                                EmpCode = data.emp.Code,
                                EmpName = data.emp.Name,
                                BranchID = data.r.BranchID,
                                BranchName = data.b.Name,
                                ReceiptNo = data.r.ReceiptMemoNo,
                                DateOut = data.r.DateOut.ToString("dd-MM-yyyy") + " " + data.r.TimeOut,
                                TimeOut = data.r.TimeOut,
                                Currency = data.curr_pl.Description,
                                Process = data.item.Process,
                                InvoiceNo = data.r.ReceiptNo,
                                //detail
                                UnitPrice = data.rd.UnitPrice,
                                Total = data.rd.Total * -1,
                                ItemCode = data.rd.Code,
                                ItemNameKhmer = data.rd.KhmerName,
                                ItemNameEng = data.rd.EnglishName,
                                Qty = (data.rd.Qty) * -1,
                                Uom = uom.Name,
                                DisItem = (data.rd.DisValue) * -1,
                                //Summary
                                DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                //SCount = ChCount.ToString(),
                                GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency((data.r.GrandTotalSys * -1), sysCur.Amounts),
                                TotalDiscountItem = Convert.ToDecimal(data.rd.DisValue * data.r.ExchangeRate),
                                DiscountTotal = data.r.DisValue,
                                TotalVat = data.r.TaxValue,
                                TotalGrandTotal = data.rd.TotalSys * data.r.LocalSetRate,
                                TotalGrandTotalSys = data.rd.TotalSys
                            }).ToList();

            if (DateFrom != null && DateTo != null && BranchID == 0 && saleEmpID == 0 && DeliveryID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && saleEmpID != 0 && DeliveryID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.SaleEmID == saleEmpID && w.ShippedBy == DeliveryID).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.SaleEmID == saleEmpID && w.ShippedBy == DeliveryID).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.SaleEmID == saleEmpID && w.ShippedBy == DeliveryID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && saleEmpID == 0 && DeliveryID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && saleEmpID == 0 && DeliveryID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID && w.ShippedBy == DeliveryID).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID && w.ShippedBy == DeliveryID).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID && w.ShippedBy == DeliveryID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && saleEmpID != 0 && DeliveryID == 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID && w.SaleEmID == saleEmpID).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID && w.SaleEmID == saleEmpID).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.BranchID == BranchID && w.SaleEmID == saleEmpID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && saleEmpID == 0 && DeliveryID != 0)
            {
                saleARs = _context.SaleARs.Where(w => w.CompanyID == GetCompany().ID && w.ShippedBy == DeliveryID).ToList();
                saleAREditable = _context.SaleAREdites.Where(w => w.CompanyID == GetCompany().ID && w.ShippedBy == DeliveryID).ToList();
                Creditmemo = _context.SaleCreditMemos.Where(w => w.CompanyID == GetCompany().ID && w.ShippedBy == DeliveryID).ToList();
            }

            var saleARSummary = saleARs;
            var saleAR = (from sale in saleARSummary
                          join saledetail in _context.SaleARDetails on sale.SARID equals saledetail.SARID
                          join user in _context.UserAccounts on sale.UserID equals user.ID
                          join com in _context.Company on user.CompanyID equals com.ID
                          join curr_pl in _context.Currency on sale.SaleCurrencyID equals curr_pl.ID
                          join curr in _context.Currency on sale.LocalCurID equals curr.ID
                          join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                          join douType in _context.DocumentTypes on sale.DocTypeID equals douType.ID
                          join b in _context.Branches on sale.BranchID equals b.ID
                          join item in _context.ItemMasterDatas on saledetail.ItemID equals item.ID
                          group new { sale, saledetail, curr_pl, curr_sys, curr, douType, b, user, item } by new { sale.SARID, saledetail.ItemID } into datas
                          let data = datas.FirstOrDefault()
                          let emp = _context.Employees.FirstOrDefault(s => s.ID == data.sale.SaleEmID) ?? new Employee()
                          let ship = _context.Employees.FirstOrDefault(s => s.ID == data.sale.ShippedBy) ?? new Employee()
                          let sumByBranch = saleARSummary.Where(_r => _r.BranchID == data.sale.BranchID).Sum(_as => _as.TotalAmountSys)
                          let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                          let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                          select new DevSummarySale
                          {
                              //detail
                              LineID = DateTime.Now.Ticks.ToString(),
                              ItemID = data.saledetail.ItemID,
                              ReceiptID = data.sale.SARID,
                              DouType = data.douType.Code + "-" + data.douType.Name,
                              EmpCode = emp.Code,
                              EmpName = emp.Name,
                              ShipBy = ship.Name,
                              BranchID = data.sale.BranchID,
                              BranchName = data.b.Name,
                              ReceiptNo = data.sale.InvoiceNo,
                              DateOut = data.sale.PostingDate.ToString("dd-MM-yyyy"),
                              TimeOut = "",
                              Currency = data.curr_pl.Description,
                              Process = data.item.Process,
                              InvoiceNo = data.sale.InvoiceNo,
                              //detail
                              UnitPrice = data.saledetail.UnitPrice,
                              Total = data.saledetail.Total,
                              ItemCode = data.saledetail.ItemCode,
                              ItemNameKhmer = data.saledetail.ItemNameKH,
                              ItemNameEng = data.saledetail.ItemNameEN,
                              Qty = data.saledetail.Qty,
                              Uom = data.saledetail.UomName,
                              DisItem = data.saledetail.DisValue,
                              //Summary
                              DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                              DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                              //SCount = ChCount.ToString(),
                              GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency(data.sale.TotalAmountSys, sysCur.Amounts),
                              TotalDiscountItem = Convert.ToDecimal(data.saledetail.DisValue * data.sale.ExchangeRate),
                              DiscountTotal = data.sale.DisValue,
                              TotalVat = data.sale.VatValue,
                              TotalGrandTotal = data.saledetail.TotalSys * data.sale.LocalSetRate,
                              TotalGrandTotalSys = data.saledetail.TotalSys
                          }).ToList();

            var saleAREditSummary = saleAREditable;
            var saleAREdit = (from sale in saleAREditSummary
                              join saledetail in _context.SaleAREditeDetails on sale.SARID equals saledetail.SARID
                              join user in _context.UserAccounts on sale.UserID equals user.ID
                              join com in _context.Company on user.CompanyID equals com.ID
                              join curr_pl in _context.Currency on sale.SaleCurrencyID equals curr_pl.ID
                              join curr in _context.Currency on sale.LocalCurID equals curr.ID
                              join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                              join douType in _context.DocumentTypes on sale.DocTypeID equals douType.ID
                              join b in _context.Branches on sale.BranchID equals b.ID
                              join item in _context.ItemMasterDatas on saledetail.ItemID equals item.ID
                              group new { sale, saledetail, curr_pl, curr_sys, curr, douType, b, user, item } by new { sale.SARID, saledetail.ItemID } into datas
                              let data = datas.FirstOrDefault()
                              let emp = _context.Employees.FirstOrDefault(s => s.ID == data.sale.SaleEmID) ?? new Employee()
                              let ship = _context.Employees.FirstOrDefault(s => s.ID == data.sale.ShippedBy) ?? new Employee()
                              let sumByBranch = saleARSummary.Where(_r => _r.BranchID == data.sale.BranchID).Sum(_as => _as.TotalAmountSys)
                              let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                              let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                              select new DevSummarySale
                              {
                                  //detail
                                  LineID = DateTime.Now.Ticks.ToString(),
                                  ItemID = data.saledetail.ItemID,
                                  ReceiptID = data.sale.SARID,
                                  DouType = data.douType.Code + "-" + data.douType.Name + "" + "Editable",
                                  EmpCode = emp.Code,
                                  EmpName = emp.Name,
                                  ShipBy = ship.Name,
                                  BranchID = data.sale.BranchID,
                                  BranchName = data.b.Name,
                                  ReceiptNo = data.sale.InvoiceNo,
                                  DateOut = data.sale.PostingDate.ToString("dd-MM-yyyy"),
                                  TimeOut = "",
                                  Currency = data.curr_pl.Description,
                                  Process = data.item.Process,
                                  InvoiceNo = data.sale.InvoiceNo,
                                  //detail
                                  UnitPrice = data.saledetail.UnitPrice,
                                  Total = data.saledetail.Total,
                                  ItemCode = data.saledetail.ItemCode,
                                  ItemNameKhmer = data.saledetail.ItemNameKH,
                                  ItemNameEng = data.saledetail.ItemNameEN,
                                  Qty = data.saledetail.Qty,
                                  Uom = data.saledetail.UomName,
                                  DisItem = data.saledetail.DisValue,
                                  //Summary
                                  DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                  DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                  //SCount = ChCount.ToString(),
                                  GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency(data.sale.TotalAmountSys, sysCur.Amounts),
                                  TotalDiscountItem = Convert.ToDecimal(data.saledetail.DisValue * data.sale.ExchangeRate),
                                  DiscountTotal = data.sale.DisValue,
                                  TotalVat = data.sale.VatValue,
                                  TotalGrandTotal = data.saledetail.TotalSys * data.sale.LocalSetRate,
                                  TotalGrandTotalSys = data.saledetail.TotalSys
                              }).ToList();

            var SaleCreditMemo = (from sale in Creditmemo
                                  join saledetail in _context.SaleCreditMemoDetails on sale.SCMOID equals saledetail.SCMOID
                                  join user in _context.UserAccounts on sale.UserID equals user.ID
                                  join com in _context.Company on user.CompanyID equals com.ID
                                  join curr_pl in _context.Currency on sale.SaleCurrencyID equals curr_pl.ID
                                  join curr in _context.Currency on sale.LocalCurID equals curr.ID
                                  join curr_sys in _context.Currency on com.SystemCurrencyID equals curr_sys.ID
                                  join douType in _context.DocumentTypes on sale.DocTypeID equals douType.ID
                                  join b in _context.Branches on sale.BranchID equals b.ID
                                  join item in _context.ItemMasterDatas on saledetail.ItemID equals item.ID
                                  group new { sale, saledetail, curr_pl, curr_sys, curr, douType, b, user, item } by new { sale.SCMOID, saledetail.ItemID } into datas
                                  let data = datas.FirstOrDefault()
                                  let re = _context.SaleARs.FirstOrDefault(s => s.SARID == data.sale.BasedOn) ?? new SaleAR()
                                  let emp = _context.Employees.FirstOrDefault(s => s.ID == data.sale.SaleEmID) ?? new Employee()
                                  let ship = _context.Employees.FirstOrDefault(s => s.ID == data.sale.ShippedBy) ?? new Employee()
                                  let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                                  let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                                  select new DevSummarySale
                                  {
                                      //detail
                                      LineID = DateTime.Now.Ticks.ToString(),
                                      ItemID = data.saledetail.ItemID,
                                      RefNo = "IN" + " " + re.InvoiceNo,
                                      ReceiptID = data.sale.SCMOID,
                                      DouType = data.douType.Code + "-" + data.douType.Name,
                                      EmpCode = emp.Code,
                                      EmpName = emp.Name,
                                      ShipBy = ship.Name,
                                      BranchID = data.sale.BranchID,
                                      BranchName = data.b.Name,
                                      ReceiptNo = data.sale.InvoiceNo,
                                      DateOut = data.sale.PostingDate.ToString("dd-MM-yyyy"),
                                      TimeOut = "",
                                      Currency = data.curr_pl.Description,
                                      Process = data.item.Process,
                                      InvoiceNo = data.sale.InvoiceNo,
                                      //detail 
                                      UnitPrice = data.saledetail.UnitPrice,
                                      Total = data.saledetail.Total * -1,
                                      ItemCode = data.saledetail.ItemCode,
                                      ItemNameKhmer = data.saledetail.ItemNameKH,
                                      ItemNameEng = data.saledetail.ItemNameEN,
                                      Qty = data.saledetail.Qty * -1,
                                      Uom = data.saledetail.UomName,
                                      DisItem = data.saledetail.DisValue * -1,
                                      //Summary
                                      DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                      DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                      GrandTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency((data.sale.TotalAmountSys * -1), sysCur.Amounts),
                                      TotalDiscountItem = Convert.ToDecimal(data.saledetail.DisValue * data.sale.ExchangeRate),
                                      DiscountTotal = data.sale.DisValue,
                                      TotalVat = data.sale.VatValue,
                                      TotalGrandTotal = data.saledetail.TotalSys * data.sale.LocalSetRate,
                                      TotalGrandTotalSys = data.saledetail.TotalSys
                                  }).ToList();

            var allSummarySale = new List<DevSummarySale>(SaleReceipt.Count + saleAR.Count + saleAREdit.Count + SaleMemo.Count + SaleCreditMemo.Count);
            allSummarySale.AddRange(SaleReceipt);
            allSummarySale.AddRange(saleAR);
            allSummarySale.AddRange(saleAREdit);
            allSummarySale.AddRange(SaleMemo);
            allSummarySale.AddRange(SaleCreditMemo);

            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
            var allSale = (from all in allSummarySale
                           join b in _context.Branches on all.BranchID equals b.ID
                           join c in _context.Company on b.CompanyID equals c.ID
                           join curr_sys in _context.Currency on c.SystemCurrencyID equals curr_sys.ID
                           join curr in _context.Currency on c.LocalCurrencyID equals curr.ID
                           group new { all, b, curr_sys, curr } by new { all.ReceiptID, all.ItemID } into r
                           let data = r.FirstOrDefault()
                           let sumByBranch = allSummarySale.Where(_r => _r.ReceiptID == data.all.ReceiptID).Sum(_as => _as.GrandTotalSys)
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr.ID) ?? new Display()
                           //summary
                           let sumDisItem = allSummarySale.Sum(s => s.TotalDiscountItem)
                           let sumDisTotal = allSummarySale.Sum(s => s.DiscountTotal)
                           let sumVat = allSummarySale.Sum(s => s.TotalVat)
                           let sumGrandTotalSys = allSummarySale.Sum(s => s.TotalGrandTotalSys)
                           let sumGrandTotal = allSummarySale.Sum(s => s.TotalGrandTotal)
                           select new
                           {
                               data.all.LineID,
                               data.all.RefNo,
                               ReceiptNo = data.all.ReceiptNo + " " + "(" + data.all.DouType + ")",
                               data.all.EmpCode,
                               data.all.EmpName,
                               data.all.ShipBy,
                               data.all.BranchID,
                               BranchName = data.b.Name,
                               data.all.ReceiptID,
                               Invoice = data.all.ReceiptNo,
                               data.all.DateOut,
                               data.all.TimeOut,
                               Hide = "",
                               data.all.Process,
                               data.all.ItemID,
                               data.all.InvoiceNo,
                               //detail
                               UnitPrice = _fncModule.ToCurrency(data.all.UnitPrice, sysCur.Amounts),
                               Total = _fncModule.ToCurrency(data.all.Total, sysCur.Amounts),
                               data.all.ItemCode,
                               data.all.ItemNameKhmer,
                               data.all.ItemNameEng,
                               data.all.Qty,
                               data.all.Uom,
                               data.all.Currency,
                               DisItem = _fncModule.ToCurrency(data.all.DisItem, sysCur.Amounts),
                               //Summary
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               //SCount = ChCount.ToString(),                              
                               data.all.GrandTotal,
                               GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                               SDiscountItem = _fncModule.ToCurrency(sumDisItem, sysCur.Amounts),
                               SDiscountTotal = _fncModule.ToCurrency(sumDisTotal, sysCur.Amounts),
                               SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumVat, sysCur.Amounts),
                               SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumGrandTotalSys, sysCur.Amounts),
                               SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(sumGrandTotal, lcCur.Amounts),
                           }).ToList();
            return Ok(allSale.OrderBy(o => o.DateOut));
        }

        // Purchase Summary Report
        public ActionResult SummaryPurchaseReport(string DateFrom, string DateTo, int BranchID, int UserID, int plid)
        {
            List<DevSummarySale> pur = new();
            List<Purchase_AP> purchaseFilter = new();

            if (DateFrom == null || DateTo == null)
            {
                return Ok(pur);
            }
            if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && plid == 0)
            {
                purchaseFilter = _context.Purchase_APs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID == 0 && plid == 0)
            {
                purchaseFilter = _context.Purchase_APs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID != 0 && UserID != 0 && plid == 0)
            {
                purchaseFilter = _context.Purchase_APs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && BranchID == 0 && UserID == 0 && plid != 0)
            {
                purchaseFilter = _context.Purchase_APs.Where(w => w.CompanyID == GetCompany().ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.PurCurrencyID == plid).ToList();
            }

            double TotalDisItem = 0;
            double TotalDisTotal = 0;
            double TotalVat = 0;
            double GrandTotalSys = 0;
            double GrandTotal = 0;

            var purAPSummary = purchaseFilter;
            // if (plid > 0) saleARSummary = saleARSummary.Where(i => i.PriceListID == plid).ToList();
            var saleARDetail = (from s in purAPSummary
                                join sd in _context.PurchaseAPDetail on s.PurchaseAPID equals sd.PurchaseAPID
                                select new
                                {
                                    TotalDisItem = sd.DiscountValue * s.PurRate
                                }).ToList();
            TotalDisItem = saleARDetail.Sum(s => s.TotalDisItem);
            TotalDisTotal = purAPSummary.Sum(s => s.DiscountValue);
            TotalVat = purAPSummary.Sum(s => s.TaxValue * s.PurRate);
            GrandTotalSys = purAPSummary.Sum(s => (double)s.SubTotalAfterDisSys);
            GrandTotal = purAPSummary.Sum(s => (double)s.SubTotalAfterDisSys * s.LocalSetRate);


            var list = (from ap in purchaseFilter
                        join apd in _context.PurchaseAPDetail on ap.PurchaseAPID equals apd.PurchaseAPID
                        join emp in _context.BusinessPartners on ap.VendorID equals emp.ID
                        join curr_pl in _context.Currency on ap.PurCurrencyID equals curr_pl.ID
                        join curr in _context.Currency on ap.LocalCurID equals curr.ID
                        join curr_sys in _context.Currency on ap.SysCurrencyID equals curr_sys.ID
                        join douType in _context.DocumentTypes on ap.DocumentTypeID equals douType.ID
                        join b in _context.Branches on ap.BranchID equals b.ID

                        group new { ap, apd, emp, curr_pl, curr_sys, curr, douType, b } by new { ap.BranchID, ap.PurchaseAPID, apd.PurchasPrice } into datas
                        let data = datas.FirstOrDefault()
                        let sumByBranch = purAPSummary.Where(_r => _r.BranchID == data.ap.BranchID).Sum(_as => _as.SubTotalAfterDisSys)
                        let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                        let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                        select new DevSummarySale
                        {
                            ReceiptID = data.ap.SeriesDetailID,
                            AmountFreight = _fncModule.ToCurrency(data.ap.FrieghtAmount, plCur.Amounts),
                            DouType = data.douType.Code,
                            EmpCode = data.emp.Code,
                            EmpName = data.emp.Name,
                            BranchID = data.ap.BranchID,
                            BranchName = data.b.Name,
                            ReceiptNo = data.ap.Number,
                            DateOut = data.ap.PostingDate.ToString("dd-MM-yyyy"),

                            DiscountItem = _fncModule.ToCurrency(data.ap.DiscountValue, plCur.Amounts),
                            Currency = data.curr_pl.Description,
                            GrandTotal = _fncModule.ToCurrency(data.ap.SubTotalAfterDis, plCur.Amounts),
                            //Summary
                            DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                            DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                            //SCount = ChCount.ToString(),
                            GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                            SDiscountItem = data.curr_sys.Description + " " + _fncModule.ToCurrency(TotalDisItem, sysCur.Amounts),
                            SDiscountTotal = data.curr_sys.Description + " " + _fncModule.ToCurrency(TotalDisTotal, sysCur.Amounts),
                            SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(TotalVat, sysCur.Amounts),
                            SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(GrandTotalSys, sysCur.Amounts),
                            SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(GrandTotal, plCur.Amounts),
                            //
                            TotalDiscountItem = (decimal)_context.PurchaseAPDetail.Where(w => w.PurchaseAPID == data.ap.PurchaseAPID).Sum(s => s.DiscountValue),
                            //   DiscountTotal = data.sale.DisValue,
                            //   Vat = data.sale.VatValue * data.sale.ExchangeRate,
                            //   GrandTotalSys = data.sale.TotalAmountSys,
                            //   MGrandTotal = data.sale.TotalAmountSys * data.sale.LocalSetRate,

                        }).ToList();

            return Ok(list);
        }
        public IActionResult PaymentMeansSummary(string dateFrom, string dateTo, int branchId, int userId, int paymentId)
        {
            List<Receipt> receiptsFilter = new();
            DateTime _dateFrom = Convert.ToDateTime(dateFrom);
            DateTime _dateTo = Convert.ToDateTime(dateTo);

            if (dateFrom != null && dateTo != null && branchId == 0 && userId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId != 0 && userId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId != 0 && userId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId && w.UserOrderID == userId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId != 0 && userId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId && w.UserOrderID == userId && w.PaymentMeansID == paymentId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId != 0 && userId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId && w.PaymentMeansID == paymentId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId == 0 && userId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.PaymentMeansID == paymentId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId == 0 && userId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.PaymentMeansID == paymentId && w.UserOrderID == userId).ToList();
            }
            else
            {
                return Ok(new List<Receipt>());
            }

            var Summary = GetSummaryTotals(dateFrom, dateTo, branchId, userId, "", "");
            if (Summary != null)
            {
                var lcCur = _context.Displays.FirstOrDefault(s => s.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var paymentMeans = paymentId == 0 ? _context.MultiPaymentMeans.ToList()
                                : paymentId == -1 ? _context.MultiPaymentMeans.Where(s => s.Type == PaymentMeanType.CardMember).ToList()
                                : _context.MultiPaymentMeans.Where(s => s.PaymentMeanID == paymentId).ToList();

                var mpay = (from r in receiptsFilter
                            join m in paymentMeans on r.ReceiptID equals m.ReceiptID
                            join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                            join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                            join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID

                            let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curr_sys.ID) ?? new Display()
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curr_pl.ID) ?? new Display()
                            select new
                            {
                                ReceiptID = r.ReceiptID,
                                Amount = m.Amount < 0 ? Convert.ToDecimal(_fncModule.ToCurrency(m.Amount, plCur.Amounts))
                                : m.AltCurrencyID == 2 ? Convert.ToDecimal(_fncModule.ToCurrency(m.Amount / m.AltRate, plCur.Amounts))
                                : m.AltCurrencyID == 3 ? Convert.ToDecimal(_fncModule.ToCurrency(m.Amount / m.AltRate, plCur.Amounts))
                                : m.AltCurrencyID == 4 ? Convert.ToDecimal(_fncModule.ToCurrency(m.Amount / m.AltRate, plCur.Amounts))
                                : m.AltCurrencyID == 5 ? Convert.ToDecimal(_fncModule.ToCurrency(m.Amount / m.AltRate, plCur.Amounts))
                                : Convert.ToDecimal(_fncModule.ToCurrency(m.Amount, plCur.Amounts)),
                                PayID = m.PaymentMeanID
                            }).ToList();

                var mpays = mpay.GroupBy(x => x.PayID).Select(s => new
                {
                    ReceiptID = s.FirstOrDefault().ReceiptID,
                    PaymentMeanID = s.FirstOrDefault().PayID,
                    Amount = s.Sum(x => x.Amount)
                }).ToList();

                var list = (from multipay in paymentMeans
                            join receipts in receiptsFilter on multipay.ReceiptID equals receipts.ReceiptID
                            join curr_pl in _context.Currency on receipts.PLCurrencyID equals curr_pl.ID
                            join curr in _context.Currency on receipts.LocalCurrencyID equals curr.ID
                            join curr_sys in _context.Currency on receipts.SysCurrencyID equals curr_sys.ID
                            group new { multipay, receipts, curr_pl, curr_sys, curr } by new { multipay.PaymentMeanID } into datas

                            let data = datas.FirstOrDefault()
                            let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_sys.ID) ?? new Display()
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                            let totalAmount = mpays.FirstOrDefault(s => s.PaymentMeanID == data.multipay.PaymentMeanID)
                            select new
                            {
                                PaymentMean = data.multipay.Type == PaymentMeanType.Normal ? _context.PaymentMeans.FirstOrDefault(s => s.ID == data.multipay.PaymentMeanID).Type : "Card Member",
                                Multipayment = data.multipay.Amount < 0 ? data.multipay.PLCurrency + " " + _fncModule.ToCurrency(totalAmount.Amount, plCur.Amounts)
                                : data.multipay.AltCurrencyID == 2 ? data.multipay.PLCurrency + " " + _fncModule.ToCurrency(totalAmount.Amount, plCur.Amounts)
                                : data.multipay.AltCurrencyID == 3 ? data.multipay.PLCurrency + " " + _fncModule.ToCurrency(totalAmount.Amount, plCur.Amounts)
                                : data.multipay.AltCurrencyID == 4 ? data.multipay.PLCurrency + " " + _fncModule.ToCurrency(totalAmount.Amount, plCur.Amounts)
                                : data.multipay.AltCurrencyID == 5 ? data.multipay.PLCurrency + " " + _fncModule.ToCurrency(totalAmount.Amount, plCur.Amounts)
                                : data.multipay.AltCurrency + " " + _fncModule.ToCurrency(totalAmount.Amount, plCur.Amounts),

                                //Last Summary
                                SDiscountItem = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                                SDiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                                SVat = data.curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().TaxValue, sysCur.Amounts),
                                SGrandTotalSys = data.curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotalSys, sysCur.Amounts),
                                SGrandTotal = data.curr.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotal, lcCur.Amounts),
                            }).ToList();
                return Ok(list);
            }
            return Ok(new List<Receipt>());
        }

        //===============================================manual stock================================

        [HttpGet("devReport/invoicePending/{pageIndex?}/{dateFrom?}/{dateTo?}")]
        public async Task<IActionResult> InvoicePending(int pageIndex = 1, string dateFrom = "", string dateTo = "")
        {
            ViewBag.InvoicePending = "highlight";
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;

            var resultForm = new PushCancelForm();
            var pager = await FilterPendingInvoicesAsync(dateFrom, dateTo, pageIndex);
            resultForm.PageItems = pager;
            if (pageIndex <= 1) { pageIndex = 1; }
            if (pageIndex >= pager.TotalPages) { pageIndex = pager.TotalPages; }
            ViewBag.TotalPages = resultForm.PageItems.TotalPages;
            ViewBag.PageIndex = pageIndex;

            return View(resultForm);
        }

        [HttpPost]
        public async Task<IActionResult> InvoicePending(IDictionary<string, string> form)
        {
            PushCancelForm resultForm = new PushCancelForm();
            int pageIndex = 1;

            bool a = form.TryGetValue("DateFrom", out string _dateFrom);
            bool b = form.TryGetValue("DateTo", out string _dateTo);
            ViewBag.DateFrom = _dateFrom;
            ViewBag.DateTo = _dateTo;
            resultForm.PageItems = await FilterPendingInvoicesAsync(_dateFrom, _dateTo, pageIndex);
            if (resultForm.PageItems.TotalPages <= 0) { ViewBag.TotalPages = 1; }
            else { ViewBag.TotalPages = resultForm.PageItems.TotalPages; }
            ViewBag.TotalPages = resultForm.PageItems.TotalPages;
            ViewBag.PageIndex = pageIndex;
            return View(resultForm);

        }

        private async Task<Pagination<T>> PaginateAsync<T>(IQueryable<T> items, int pageIndex, int pageSize)
        {
            var pageItems = await Pagination<T>.CreateAsync(items, pageIndex, pageSize);
            return pageItems;
        }

        private async Task<Pagination<PushCancel>> FilterPendingInvoicesAsync(string dateFrom, string dateTo, int pageIndex = 1)
        {
            var pendingReceipts = GetPendingReceipts();
            if (string.IsNullOrWhiteSpace(dateTo))
            {
                dateTo = DateTime.Today.ToString("yyyy-MM-dd");
            }

            if (!string.IsNullOrWhiteSpace(dateFrom) || !string.IsNullOrWhiteSpace(dateTo))
            {
                DateTime.TryParse(dateFrom, out DateTime _dateFrom);
                DateTime.TryParse(dateTo, out DateTime _dateTo);
                var receipts = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo);
                pendingReceipts = GetPendingReceipts(receipts);
            }

            var results = await PaginateAsync(pendingReceipts, pageIndex, 10);
            return results;
        }

        public async Task<IActionResult> InsusseStock(PushCancelForm form)
        {
            if (form == null) { return RedirectToAction(nameof(InvoicePending)); }
            if (form.PageItems == null) { return RedirectToAction(nameof(InvoicePending)); }
            var receipts = await _context.Receipt.Include(r => r.RececiptDetail)
                .Where(r => form.PageItems.Any(_r => r.ReceiptID == _r.ReceiptID && _r.Selected))
                .ToListAsync();
            await _posSyncRepo.IssueStockInternalAsync(receipts);

            return RedirectToAction(nameof(InvoicePending));
        }
        public IQueryable<PushCancel> GetPendingReceipts(IQueryable<Receipt> receipts = null)
        {
            var invs = receipts;
            if (invs == null)
            {
                invs = _context.Receipt;
            }

            invs = invs.Where(w => !_context.InventoryAudits.Any(i => i.SeriesDetailID == w.SeriesDID));
            var data = (from r in invs
                        join user in _context.UserAccounts on r.UserOrderID equals user.ID
                        join em in _context.Employees on user.EmployeeID equals em.ID
                        join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                        join Lcurr in _context.Currency on r.LocalCurrencyID equals Lcurr.ID
                        join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                        select new PushCancel
                        {
                            ReceiptID = r.ReceiptID,
                            ReceiptNo = r.ReceiptNo,
                            DateOut = r.DateOut,
                            TimeOut = r.TimeOut,
                            EmpName = em.Name,
                            LCCurrency = Lcurr.Description,
                            PLCurrency = curr_pl.Description,
                            SSCCurrency = curr_sys.Description,
                            LSetRate = r.LocalSetRate,
                            ReceiptDetailVeiws = (from rd in _context.ReceiptDetail.Where(i => i.ReceiptID == r.ReceiptID)
                                                  join item in _context.ItemMasterDatas on rd.ItemID equals item.ID
                                                  //   join ge in _context.GroupDUoMs.Where(x => x.GroupUoMID == item.GroupUoMID) on rd.UomID equals ge.AltUOM
                                                  let orft = _context.GroupDUoMs.FirstOrDefault(w => w.GroupUoMID == item.GroupUomID && w.AltUOM == rd.UomID) ?? new GroupDUoM()
                                                  select new ReceiptDetailVeiw
                                                  {
                                                      ID = rd.ID,
                                                      ReceiptID = rd.ReceiptID,
                                                      WarehouseID = r.WarehouseID,
                                                      ItemID = rd.ItemID,
                                                      Code = rd.Code,
                                                      KhmerName = rd.KhmerName,
                                                      EnglisName = rd.EnglishName,
                                                      Qty = rd.Qty,
                                                      UnitPrice = rd.UnitPrice,
                                                      DisValue = rd.DiscountValue,
                                                      Total = rd.Total,
                                                      UomID = rd.UomID,
                                                      Factor = orft.Factor,
                                                  }).ToArray() ?? Array.Empty<ReceiptDetailVeiw>(),
                        });
            return data;
        }
        public async Task<IActionResult> ExportReceipt()
        {
            var getMultiMean = await _posExcelRepo.ExportPaymentMeanAsynce();
            var FreightRecp = await _posExcelRepo.ExportFreightAsync();
            var receiptDe = await _posExcelRepo.ExportReceiptsDetailAsync();
            var receiptGets = await _posExcelRepo.ExportReceiptsAsync();
            _wbExport.AddSheet(receiptGets);
            _wbExport.AddSheet(receiptDe);
            _wbExport.AddSheet(FreightRecp);
            _wbExport.AddSheet(getMultiMean);
            Stream ms = new MemoryStream();
            _wbExport.Write(ms);
            ms.Position = 0;
            return File(ms, "application/octet-stream", "SummarySaleReport.xls");
        }
        public async Task<IActionResult> ImportReceipt(FileCollection<ReceiptExport> fileExcel)
        {
            if (fileExcel.Files == null)
            {
                ModelState.AddModelError("fileExcel", "Please choose file to import");
            }
            if (ModelState.IsValid)
            {
                var files = Request.Form.Files[0];
                if (fileExcel.Files.Count <= 0) { return RedirectToAction(nameof(SummarySaleView)); }
                IFormFile file = fileExcel.Files[0];
                string path = _posExcelRepo.GetFilePath();
                using (Stream fs = new MemoryStream())
                {
                    await file.CopyToAsync(fs);
                    fs.Seek(0, SeekOrigin.Begin);
                    IWorkbook wb = _workbook.ReadWorkbook(fs);
                    await _posExcelRepo.ImportReceiptsAsync(wb, ModelState);
                }
            }
            return View(nameof(SummarySaleView));
        }

        public IActionResult CusConsignmentDetails(int customerid, int warehouseid)
        {
            List<CustomerConsignment> cusConsignments = new();
            if (customerid == 0 && warehouseid > 0)
            {
                cusConsignments = _context.CustomerConsignments.Where(s => s.Status == StatusSelect.Close && s.WarehouseID == warehouseid).ToList();
            }
            else if (customerid > 0)
            {
                cusConsignments = _context.CustomerConsignments.Where(s => s.Status == StatusSelect.Close && s.WarehouseID == warehouseid && s.CustomerID == customerid).ToList();
            }

            var data = (from cu in cusConsignments
                        join cud in _context.CustomerConsignmentDetails on cu.CustomerConsignmentID equals cud.CustomerConsignmentID
                        join item in _context.ItemMasterDatas on cud.ItemID equals item.ID
                        join uom in _context.UnitofMeasures on cud.UomID equals uom.ID
                        join cus in _context.BusinessPartners on cu.CustomerID equals cus.ID
                        select new
                        {
                            Customer = cus.Name,
                            Code = item.Code,
                            ItemName = item.KhmerName,
                            Expire = cu.ValidDate.Date.ToString("dd-MM-yyyy"),
                            Uom = uom.Name,
                            Qty = cud.Qty,
                            Status = cud.Status == StatusItem.Withdraw ? "Withdraw" : "Return",
                        }).ToList();
            if (data.Count > 0)
            {
                return Ok(data);
            }
            return Ok(new List<CustomerConsignment>());
        }

    }
}