using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.InventoryAuditReport;
using CKBS.Models.Services.ReportDashboard;
using CKBS.Models.Services.ReportInventory;
using CKBS.Models.Services.ReportPurchase;
using CKBS.Models.Services.ReportSale;
using CKBS.Models.Services.ReportSale.dev;
using System;
using System.Collections.Generic;
using System.Linq;
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Models.Services.POS.Templates;
using CKBS.Models.Services.POS.Template;
using CKBS.Models.Services.Inventory.PriceList;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Sale;
using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.ServicesClass.Report;
using System.Threading.Tasks;
using CKBS.Models.Services.POS.KVMS;

namespace CKBS.Models.Services.Responsitory
{
    public interface IReport
    {
        IEnumerable<SummarySale> GetSummarySales(string DateFrom, string DateTo, int BranchID, int UserID);
        IEnumerable<DetailSale> GetDetailSales(int OrderID);
        IEnumerable<SummaryPurchaseAP> GetSummaryPurchaseAPs(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID, int VendorID);
        IEnumerable<DetailPurchaseAp> GetDetailPurchaseAps(int PurchaseID);
        IEnumerable<ReportCloseShft> GetReportCloseShfts(string DateFrom, string DateTo, int BranchID, int UserID);
        IEnumerable<SummarySale> GetDetailCloseShif(int UserID, int Tran_From, int Tran_To);
        IEnumerable<TopSaleQuantity> GetTopSaleQuantities(string DateFrom, string DateTo, int BranchID);
        IEnumerable<DetailTopSaleQty> DetailTopSaleQties(int ItemID, int UomID, string DateFrom, string DateTo, int BranchID);
        IEnumerable<DetailPurchaseAp> GetDetailPurchaseMemo(int PurchaseID);
        IEnumerable<SummaryPurchaseAP> GetPruchaseMemoSummary(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID, int VendorID);
        IEnumerable<DashboardSaleSummary> GetDashboardSaleSummary(int BranchID);
        IEnumerable<DashboardSaleSummary> GetDashboardPurchaseSummary(int BranchID);
        IEnumerable<SummaryPurchaseAP> GetPruchaseOrderSummary(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID, int VendorID);
        IEnumerable<DetailPurchaseAp> GetDetailPurchaseOrder(int PurchaseID);
        IEnumerable<StockInWarehouse> GetStockInWarehouses(int WarehouseID, string Process, int ItemID);
        IEnumerable<StockInWarehouse> GetStockAudit();
        IEnumerable<StockInWarehouse_Detail> GetStockInWarehouse_Details(int ItemID, string Process, int WarehouseID);
        IEnumerable<ServiceInventoryAudit> GetServiceInventoryAudits(int ItemID, int WarehouseID);
        IEnumerable<SummaryOutgoingPayment> GetSummaryOutgoings(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID);
        IEnumerable<SummaryDetailOutgoingPayment> GetDetailOutgoingPayments(int OutID);
        IEnumerable<SummaryDetailOutgoingPayment> GetDetailInvoice(string Invoice);
        IEnumerable<SummaryTransferStock> GetSummaryTransferStocks(string DateFrom, string DateTo, int FromBranchID, int ToBranchID, int FromWarehouseID, int ToWarehouseID, int UserID);
        IEnumerable<SummaryDetaitTransferStock> GetSummaryDetaitTransferStocks(int TranID);
        IEnumerable<SummaryTransferStock> GetSummaryGoodsReceiptStock(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID);
        IEnumerable<SummaryDetaitTransferStock> GetSummaryDetailGoodsReceipt(int ReceiptID);
        IEnumerable<SummaryTransferStock> GetSummaryGoodsIssuseStock(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID);
        IEnumerable<SummaryDetaitTransferStock> GetSummaryDetailGoodsIssuse(int IssuseID);
        IEnumerable<SummaryRevenuesItem> GetSummaryRevenuesItems(String DateFrom, string DateTo, int BranchID, int ItemID, string Process);
        IEnumerable<SummaryRevenuesItem> GetSummaryRevenuesItemsDetail(String DateFrom, string DateTo, int BranchID, int ItemID, string Process);
        IEnumerable<DashboardTopSale> GetDashboardTopSale { get; }
        IEnumerable<DashboardTopSale> GetDashboardStockInwaerhose { get; }

        IEnumerable<CashoutReportMaster> GetCashoutReport(double Tran_F, double Tran_To, int UserID, int localCurId);
        IEnumerable<CashoutReportMaster> GetCashoutPaymentMean(double Tran_F, double Tran_To, int UserID, int localCurId);
        IEnumerable<VoidItemReport> GetCashoutVoidItems(double Tran_F, double Tran_T, int UserID);
        List<DevGroupCustomer> GetGroupCustomerReport(Company company, string DateFrom, string DateTo, string TimeFrom, string TimeTo, int plid, string docType);
        Task<List<SaleByCustomer>> GetSaleByCustomers(string dateFrom, string dateTo, int branchId, int cusId, int curlcId);
    }
    public class ReportResponsitory : IReport
    {
        private readonly DataContext _context;
        public IEnumerable<DashboardTopSale> GetDashboardTopSale => _context.DashboardTopsale.FromSql("db_TopSale");
        public IEnumerable<DashboardTopSale> GetDashboardStockInwaerhose => _context.DashboardTopsale.FromSql("db_GetStockInwarehouse");
        private readonly UtilityModule _fncModule;
        public ReportResponsitory(DataContext context, UtilityModule fncModule)
        {
            _context = context;
            _fncModule = fncModule;
        }
        public IEnumerable<DetailSale> GetDetailSales(int OrderID) => _context.DetailSales.FromSql("rp_GetDetailSale @OrderID={0}",
          parameters: new[] {
                OrderID.ToString()
          });
        public IEnumerable<SummarySale> GetSummarySales(string DateFrom, string DateTo, int BranchID, int UserID) => _context.SummarySales.FromSql("rp_GetSummarrySale @BranchId={0},@DateT={1},@DateF={2},@UserId={3}",
            parameters: new[] {

                BranchID.ToString(),
                DateTo.ToString(),
                DateFrom.ToString(),
                UserID.ToString()
            });
        public IEnumerable<DetailPurchaseAp> GetDetailPurchaseAps(int PurchaseID) => _context.DetailPurchaseAps.FromSql("rp_DatailPruchaseAP @PurchaseID={0}",
          parameters: new[] {
                PurchaseID.ToString()
          });
        public IEnumerable<SummaryPurchaseAP> GetSummaryPurchaseAPs(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID, int VendorID) => _context.SummaryPurchaseAPs.FromSql("rp_SummaryPruchaseAP @DateF={0},@DateT={1},@BranchID={2},@WarehousID={3},@UserID={4},@VendorID={5}",
           parameters: new[] {
               DateFrom.ToString(),
               DateTo.ToString(),
               BranchID.ToString(),
               WarehouseID.ToString(),
               UserID.ToString(),
               VendorID.ToString()
           });
        public IEnumerable<ReportCloseShft> GetReportCloseShfts(string DateFrom, string DateTo, int BranchID, int UserID) => _context.ReportCloseShfts.FromSql("rp_GetCloseShift @BranchId={0},@DateF={1},@DateT={2},@UserId={3}",
          parameters: new[] {
                BranchID.ToString(),
                DateFrom.ToString(),
                DateTo.ToString(),
                UserID.ToString()
          });
        public IEnumerable<SummarySale> GetDetailCloseShif(int UserID, int Tran_From, int Tran_To) => _context.SummarySales.FromSql("rp_GetDetailCloseShift @UserID={0},@Tran_From={1},@Tran_To={2}",
                parameters: new[] {
                    UserID.ToString(),
                    Tran_From.ToString(),
                    Tran_To.ToString()
                });
        public IEnumerable<TopSaleQuantity> GetTopSaleQuantities(string DateFrom, string DateTo, int BranchID) => _context.TopSaleQuantities.FromSql("rp_GetTopSaleQauntity @BranchId={0},@DateF={1},@DateT={2}",
            parameters: new[] {
                BranchID.ToString(),
                DateFrom.ToString(),
                DateTo.ToString()
            });
        public IEnumerable<DetailTopSaleQty> DetailTopSaleQties(int ItemID, int UomID, string DateFrom, string DateTo, int BranchID) => _context.DetailTopSaleQties.FromSql("rp_GetTopSaleQauntityDetail @BranchId={0},@DateF={1},@DateT={2},@ItemID={3},@UomID={4}",
            parameters: new[] {
                BranchID.ToString(),
                DateFrom.ToString(),
                DateTo.ToString(),
                ItemID.ToString(),
                UomID.ToString()
            });
        public IEnumerable<SummaryPurchaseAP> GetPruchaseMemoSummary(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID, int VendorID) => _context.SummaryPurchaseAPs.FromSql("rp_SummaryPruchaseMemo @DateF={0},@DateT={1},@BranchID={2},@WarehousID={3},@UserID={4},@VendorID={5}",
            parameters: new[] {
                DateFrom.ToString(),
                DateTo.ToString(),
                BranchID.ToString(),
                WarehouseID.ToString(),
                UserID.ToString(),
                VendorID.ToString()
                });
        public IEnumerable<DetailPurchaseAp> GetDetailPurchaseMemo(int PurchaseID) => _context.DetailPurchaseAps.FromSql("rp_DatailPruchasMemo @PurchaseID={0}",
            parameters: new[] {
                PurchaseID.ToString()
            });
        public IEnumerable<DashboardSaleSummary> GetDashboardSaleSummary(int BranchID) => _context.DashboardSaleSummary.FromSql("db_GetSaleSummary @BranchID={0}",
            parameters: new[] {
                BranchID.ToString()
            });
        public IEnumerable<DashboardSaleSummary> GetDashboardPurchaseSummary(int BranchID) => _context.DashboardSaleSummary.FromSql("db_GetPurchaseSummary @BranchID={0}",
            parameters: new[] {
                BranchID.ToString()
            });
        public IEnumerable<SummaryPurchaseAP> GetPruchaseOrderSummary(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID, int VendorID) => _context.SummaryPurchaseAPs.FromSql("rp_SummaryPruchaseOrder @DateF={0},@DateT={1},@BranchID={2},@WarehousID={3},@UserID={4},@VendorID={5}",
            parameters: new[] {

                DateFrom.ToString(),
                DateTo.ToString(),
                BranchID.ToString(),
                WarehouseID.ToString(),
                UserID.ToString(),
                VendorID.ToString()
            });
        public IEnumerable<DetailPurchaseAp> GetDetailPurchaseOrder(int PurchaseID) => _context.DetailPurchaseAps.FromSql("rp_DatailPruchaseOrder @PurchaseID={0}",
            parameters: new[] {
                PurchaseID.ToString()
            });
        public IEnumerable<StockInWarehouse> GetStockAudit() => _context.StockInWarehouses.FromSql("rp_GetItemStockAudit");
        public IEnumerable<StockInWarehouse> GetStockInWarehouses(int WarehouseID, string Process, int ItemID) => _context.StockInWarehouses.FromSql("rp_GetItemInWarehouse @Process={0},@WarehouseID={1},@ItemID={2}",
           parameters: new[] {
                Process.ToString(),
                WarehouseID.ToString(),
                ItemID.ToString()
           });
        public IEnumerable<StockInWarehouse_Detail> GetStockInWarehouse_Details(int ItemID, string Process, int WarehouseID) => _context.StockInWarehouse_Details.FromSql("rp_GetItemInWarehouse @Process={0},@WarehouseID={1},@ItemID={2}",
            parameters: new[] {
                Process.ToString(),
                WarehouseID.ToString(),
                ItemID.ToString()
            });
        public IEnumerable<ServiceInventoryAudit> GetServiceInventoryAudits(int ItemID, int WarehouseID) => _context.ServiceInventoryAudits.FromSql("sp_GetInventoryAudit @ItemID={0},@WarehouseID={1}",
            parameters: new[] {
                ItemID.ToString(),
                WarehouseID.ToString()
            });
        public IEnumerable<SummaryOutgoingPayment> GetSummaryOutgoings(string DateFrom, string DateTo, int BranchID, int UserID, int VendorID) => _context.SummaryOutgoingPayments.FromSql("rp_GetOutgoingPayment @DateFrom={0},@DateTo={1},@BranchID={2},@UserID={3},@VendorID={4}",
            parameters: new[] {
                DateFrom.ToString(),
                DateTo.ToString(),
                BranchID.ToString(),
                UserID.ToString(),
                VendorID.ToString()
            });
        public IEnumerable<SummaryDetailOutgoingPayment> GetDetailOutgoingPayments(int OutID) => _context.SummaryDetailOutgoingPayments.FromSql("rp_GetDetailOutgoingPayment @OutID={0}",
            parameters: new[] {
                OutID.ToString()
            });
        public IEnumerable<SummaryDetailOutgoingPayment> GetDetailInvoice(string Invoice) => _context.SummaryDetailOutgoingPayments.FromSql("rp_GetDetail_Invoice_outgoingpayment @Invoice={0}",
            parameters: new[] {
                Invoice.ToString()
            });
        public IEnumerable<SummaryTransferStock> GetSummaryTransferStocks(string DateFrom, string DateTo, int FromBranchID, int ToBranchID, int FromWarehouseID, int ToWarehouseID, int UserID) => _context.SummaryTransferStocks.FromSql("rp_GetSummaryTransferStock @DateFrom={0},@DateTo={1},@FromBranchID={2},@ToBranchID={3},@FromWarehouseID={4},@ToWarehouseID={5},@UserID={6}",
            parameters: new[] {
                DateFrom.ToString(),
                DateTo.ToString(),
                FromBranchID.ToString(),
                ToBranchID.ToString(),
                FromWarehouseID.ToString(),
                ToWarehouseID.ToString(),
                UserID.ToString()
            });
        public IEnumerable<SummaryDetaitTransferStock> GetSummaryDetaitTransferStocks(int TranID) => _context.SummaryDetaitTransferStocks.FromSql("rp_GetSummaryDetailTransferStock @TranID={0}",
            parameters: new[] {
                TranID.ToString()
            });
        public IEnumerable<SummaryTransferStock> GetSummaryGoodsReceiptStock(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID) => _context.SummaryTransferStocks.FromSql("rp_GetSummaryGoodsRecepitStock @DateFrom={0},@DateTo={1},@BranchID={2},@WarehouseID={3},@UserID={4}",
            parameters: new[] {
                DateFrom.ToString(),
                DateTo.ToString(),
                BranchID.ToString(),
                WarehouseID.ToString(),
                UserID.ToString()
            });
        public IEnumerable<SummaryDetaitTransferStock> GetSummaryDetailGoodsReceipt(int ReceiptID) => _context.SummaryDetaitTransferStocks.FromSql("rp_GetDetailGoodsRecipt @ReceiptID={0}",
          parameters: new[] {
              ReceiptID.ToString()
          });
        public IEnumerable<SummaryTransferStock> GetSummaryGoodsIssuseStock(string DateFrom, string DateTo, int BranchID, int WarehouseID, int UserID) => _context.SummaryTransferStocks.FromSql("rp_GetSummaryGoodsIssuse @DateFrom={0},@DateTo={1},@BranchID={2},@WarehouseID={3},@UserID={4}",
            parameters: new[] {
                DateFrom.ToString(),
                DateTo.ToString(),
                BranchID.ToString(),
                WarehouseID.ToString(),
                UserID.ToString()
       });
        public IEnumerable<SummaryDetaitTransferStock> GetSummaryDetailGoodsIssuse(int IssuseID) => _context.SummaryDetaitTransferStocks.FromSql("rp_GetDetailGoodsIssuse @IssuseID={0}",
            parameters: new[] {
                  IssuseID.ToString()
            });
        public IEnumerable<SummaryRevenuesItem> GetSummaryRevenuesItems(string DateFrom, string DateTo, int BranchID, int ItemID, string Process) => _context.SummaryRevenuesItems.FromSql("rp_GetRevenuesItems @Process={0},@Branch={1},@DateF={2},@DateT={3},@ItemID={4}",
            parameters: new[] {
                Process.ToString(),
                BranchID.ToString(),
                DateFrom.ToString(),
                DateTo.ToString(),
                ItemID.ToString()
            });
        public IEnumerable<SummaryRevenuesItem> GetSummaryRevenuesItemsDetail(string DateFrom, string DateTo, int BranchID, int ItemID, string Process) => _context.SummaryRevenuesItems.FromSql("rp_GetRevenuesItems @Process={0},@Branch={1},@DateF={2},@DateT={3},@ItemID={4}",
            parameters: new[] {
                Process.ToString(),
                BranchID.ToString(),
                DateFrom.ToString(),
                DateTo.ToString(),
                ItemID.ToString()
            });
        public List<DevGroupCustomer> GetGroupCustomerReport(Company company, string DateFrom, string DateTo, string TimeFrom, string TimeTo, int plid, string docType)
        {
            List<Receipt> receiptsFilter = new();
            List<SaleAR> saleARsFilter = new();
            var dateFrom = Convert.ToDateTime(DateFrom);
            var dateTo = Convert.ToDateTime(DateTo);
            DateTime _dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
            DateTime _dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));
            if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && plid <= 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == company.ID && w.DateOut >= dateFrom && w.DateOut <= dateTo).ToList();
                saleARsFilter = _context.SaleARs.Where(w => w.CompanyID == company.ID && w.PostingDate >= dateFrom && w.PostingDate <= dateTo).ToList();
            }
            else if (DateFrom == null && DateTo == null && TimeFrom == null && TimeTo == null && plid <= 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == company.ID).ToList();
                saleARsFilter = _context.SaleARs.Where(w => w.CompanyID == company.ID).ToList();
            }
            else if (DateFrom == null && DateTo == null && TimeFrom == null && TimeTo == null && plid > 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.CompanyID == company.ID && w.PriceListID == plid).ToList();
                saleARsFilter = _context.SaleARs.Where(w => w.CompanyID == company.ID && w.PriceListID == plid).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && plid > 0)
            {
                receiptsFilter = _context.Receipt.Where(w =>
                    w.CompanyID == company.ID &&
                    w.PriceListID == plid &&
                    Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= _dateFrom &&
                    Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= _dateTo)
                    .ToList();
                saleARsFilter = _context.SaleARs.Where(w => w.CompanyID == company.ID && w.PostingDate >= dateFrom && w.PostingDate <= dateTo && w.PriceListID == plid).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && plid <= 0)
            {
                receiptsFilter = _context.Receipt.Where(w =>
                    w.CompanyID == company.ID &&
                    Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= _dateFrom &&
                    Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= _dateTo)
                    .ToList();
                saleARsFilter = _context.SaleARs.Where(w => w.CompanyID == company.ID && w.PostingDate >= dateFrom && w.PostingDate <= dateTo).ToList();
            }
            var sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == company.SystemCurrencyID) ?? new Display();
            List<DevGroupCustomer> receipts = new();
            List<DevGroupCustomer> saleArs = new();
            if (docType is "SP" or "All")
            {
                receipts = (from r in receiptsFilter
                            join user in _context.UserAccounts on r.UserOrderID equals user.ID
                            join emp in _context.Employees on user.EmployeeID equals emp.ID
                            join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                            join c in _context.BusinessPartners on r.CustomerID equals c.ID
                            join g1 in _context.GroupCustomer1s on c.Group1ID equals g1.ID
                            group new { r, emp, curr_pl, c, g1 } by new { g1.ID, r.ReceiptID } into datas
                            let data = datas.FirstOrDefault()
                            let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "SP")
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                            //let dataSum = receiptsFilter.Where(i=> i.ReceiptID)
                            select new DevGroupCustomer
                            {
                                Group1ID = data.g1.ID,
                                GroupName = data.g1.Name,
                                //detail
                                ReceiptID = data.r.SeriesDID,
                                DouType = douType.Code,
                                EmpCode = data.emp.Code,
                                ApplyAmount = data.r.AppliedAmount,
                                BalanceDue = data.r.GrandTotal - (double)data.r.AppliedAmount,
                                CustID = data.r.CustomerID,
                                CustName = data.c.Name,
                                EmpName = data.emp.Name,
                                ReceiptNo = data.r.ReceiptNo,
                                DateOut = data.r.DateOut.ToString("dd-MM-yyyy"),
                                TimeOut = data.r.TimeOut,
                                DiscountItem = _fncModule.ToCurrency(data.r.DiscountValue, plCur.Amounts),
                                Currency = data.curr_pl.Description,
                                GrandTotal = _fncModule.ToCurrency(data.r.GrandTotal, plCur.Amounts),
                                DisRemark = data.r.RemarkDiscount,
                                //Summary
                                DateFrom = DateFrom == null ? "" : Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                DateTo = DateTo == null ? "" : Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                GrandTotalSys = data.r.GrandTotal_Sys,
                                MGrandTotal = data.r.GrandTotal_Sys * data.r.LocalSetRate,
                                ExchangeRate = (decimal)data.r.ExchangeRate,
                                GrandTotalCal = (decimal)data.r.GrandTotal,
                                AppliedAmountCal = data.r.AppliedAmount,
                                TotalSumGroup = _fncModule.ToCurrency(receiptsFilter.Sum(i => i.GrandTotal_Sys), sysCur.Amounts),
                                TotalAppliedAmountGroup = _fncModule.ToCurrency(receiptsFilter.Sum(i => i.AppliedAmount) * (decimal)data.r.ExchangeRate, sysCur.Amounts),
                                TotalBalanceDueGroup = _fncModule.ToCurrency(receiptsFilter.Sum(i => (decimal)i.GrandTotal - i.AppliedAmount) * (decimal)data.r.ExchangeRate, sysCur.Amounts),
                            }).ToList();
                if (docType == "SP")
                {
                    var _sumGroups = (from i in receipts
                                      group i by i.Group1ID into g
                                      select new DevGroupCustomer
                                      {
                                          GrandTotalCustomer = _fncModule.ToCurrency(g.Sum(i => i.GrandTotalSys), sysCur.Amounts),
                                          ApplyAmountTotal = _fncModule.ToCurrency(g.Sum(i => i.AppliedAmountCal) * g.First().ExchangeRate, sysCur.Amounts),
                                          BalanceDueTotal = _fncModule.ToCurrency(g.Sum(i => i.BalanceDue), sysCur.Amounts),
                                          Group1ID = g.First().Group1ID
                                      }).ToList();
                    foreach (var i in receipts)
                    {
                        var sumgroup = _sumGroups.FirstOrDefault(s => i.Group1ID == s.Group1ID);
                        if (sumgroup != null)
                        {
                            i.GrandTotalCustomer = sumgroup.GrandTotalCustomer;
                            i.ApplyAmountTotal = sumgroup.ApplyAmountTotal;
                            i.BalanceDueTotal = sumgroup.BalanceDueTotal;

                        }
                    }
                    return receipts;
                }
            }
            if (docType is "IN" or "All")
            {
                saleArs = (from r in saleARsFilter
                           join user in _context.UserAccounts on r.UserID equals user.ID
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           join curr_pl in _context.Currency on r.SaleCurrencyID equals curr_pl.ID
                           join c in _context.BusinessPartners on r.CusID equals c.ID
                           join g1 in _context.GroupCustomer1s on c.Group1ID equals g1.ID
                           group new { r, emp, curr_pl, c, g1 } by new { g1.ID, r.SARID } into datas
                           let data = datas.FirstOrDefault()
                           let douType = _context.DocumentTypes.FirstOrDefault(w => w.Code == "IN")
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.curr_pl.ID) ?? new Display()
                           let appliedAmount = (decimal)data.r.AppliedAmount + data.r.DownPayment
                           select new DevGroupCustomer
                           {
                               Group1ID = data.g1.ID,
                               GroupName = data.g1.Name,
                               //detail
                               ReceiptID = data.r.SeriesDID,
                               DouType = douType.Code,
                               EmpCode = data.emp.Code,
                               ApplyAmount = appliedAmount,
                               BalanceDue = data.r.TotalAmount - (double)appliedAmount,
                               CustID = data.r.CusID,
                               CustName = data.c.Name,
                               EmpName = data.emp.Name,
                               ReceiptNo = data.r.InvoiceNumber,
                               DateOut = data.r.PostingDate.ToString("dd-MM-yyyy"),
                               TimeOut = "",
                               DiscountItem = _fncModule.ToCurrency(data.r.DisValue, plCur.Amounts),
                               Currency = data.curr_pl.Description,
                               GrandTotal = _fncModule.ToCurrency(data.r.TotalAmount, plCur.Amounts),
                               DisRemark = data.r.Remarks,
                               //Summary
                               DateFrom = DateFrom == null ? "" : Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = DateTo == null ? "" : Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                               GrandTotalSys = data.r.TotalAmountSys,
                               MGrandTotal = data.r.TotalAmountSys * data.r.LocalSetRate,
                               ExchangeRate = (decimal)data.r.ExchangeRate,
                               GrandTotalCal = (decimal)data.r.TotalAmount,
                               AppliedAmountCal = appliedAmount,
                               TotalSumGroup = _fncModule.ToCurrency(saleARsFilter.Sum(i => i.TotalAmountSys), sysCur.Amounts),
                               TotalAppliedAmountGroup = _fncModule.ToCurrency(saleARsFilter.Sum(i => (decimal)i.AppliedAmount + i.DownPayment) * (decimal)data.r.ExchangeRate, sysCur.Amounts),
                               TotalBalanceDueGroup = _fncModule.ToCurrency(saleARsFilter.Sum(i => (decimal)i.TotalAmount - ((decimal)i.AppliedAmount + i.DownPayment)) * (decimal)data.r.ExchangeRate, sysCur.Amounts),
                           }).ToList();
                if (docType == "IN")
                {
                    var _sumGroups = (from i in saleArs
                                      group i by i.Group1ID into g
                                      select new DevGroupCustomer
                                      {
                                          GrandTotalCustomer = _fncModule.ToCurrency(g.Sum(i => i.GrandTotalSys), sysCur.Amounts),
                                          ApplyAmountTotal = _fncModule.ToCurrency(g.Sum(i => i.AppliedAmountCal) * g.First().ExchangeRate, sysCur.Amounts),
                                          BalanceDueTotal = _fncModule.ToCurrency(g.Sum(i => i.BalanceDue), sysCur.Amounts),
                                          Group1ID = g.First().Group1ID
                                      }).ToList();
                    foreach (var i in saleArs)
                    {
                        var sumgroup = _sumGroups.FirstOrDefault(s => i.Group1ID == s.Group1ID);
                        if (sumgroup != null)
                        {
                            i.GrandTotalCustomer = sumgroup.GrandTotalCustomer;
                            i.ApplyAmountTotal = sumgroup.ApplyAmountTotal;
                            i.BalanceDueTotal = sumgroup.BalanceDueTotal;

                        }
                    }
                    return saleArs;
                }
            }
            List<DevGroupCustomer> all = new(receipts.Count + saleArs.Count);
            all.AddRange(receipts);
            all.AddRange(saleArs);
            var sumGroups = (from i in all
                             group i by i.Group1ID into g
                             select new DevGroupCustomer
                             {
                                 GrandTotalCustomer = _fncModule.ToCurrency(g.Sum(i => i.GrandTotalSys), sysCur.Amounts),
                                 ApplyAmountTotal = _fncModule.ToCurrency(g.Sum(i => i.AppliedAmountCal) * g.First().ExchangeRate, sysCur.Amounts),
                                 BalanceDueTotal = _fncModule.ToCurrency(g.Sum(i => i.BalanceDue), sysCur.Amounts),
                                 Group1ID = g.First().Group1ID
                             }).ToList();
            foreach (var i in all)
            {
                i.TotalSumGroup = _fncModule.ToCurrency(all.Sum(s => s.GrandTotalSys), sysCur.Amounts);
                i.TotalAppliedAmountGroup = _fncModule.ToCurrency(all.Sum(s => s.AppliedAmountCal) * i.ExchangeRate, sysCur.Amounts);
                i.TotalBalanceDueGroup = _fncModule.ToCurrency(all.Sum(s => s.GrandTotalCal - s.AppliedAmountCal) * i.ExchangeRate, sysCur.Amounts);
                var sumgroup = sumGroups.FirstOrDefault(s => i.Group1ID == s.Group1ID);
                if (sumgroup != null)
                {
                    i.GrandTotalCustomer = sumgroup.GrandTotalCustomer;
                    i.ApplyAmountTotal = sumgroup.ApplyAmountTotal;
                    i.BalanceDueTotal = sumgroup.BalanceDueTotal;

                }
            }
            return all;
        }
        public IEnumerable<CashoutReportMaster> GetCashoutReport(double Tran_F, double Tran_T, int UserID, int localCurId)
        {
            var receipt = _context.Receipt.Where(w => w.ReceiptID > Tran_F && w.ReceiptID <= Tran_T && w.UserOrderID == UserID).ToList();
            var userSetting = _context.GeneralSettings.FirstOrDefault(w => w.UserID == UserID) ?? new GeneralSetting();
            var currencies = GetDisplayPayCurrency(userSetting.PriceListID);
            var baseCurrency = currencies.FirstOrDefault(i => i.AltCurrencyID == userSetting.SysCurrencyID) ?? new DisplayPayCurrencyModel();
            var localCurrency = currencies.FirstOrDefault(i => i.AltCurrencyID == localCurId) ?? new DisplayPayCurrencyModel();
            var close = _context.CloseShift.Where(c => c.Trans_From == Tran_F && c.Trans_To == Tran_T && c.UserID == UserID);
            var soldAount = (from r in receipt
                             join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                             select new { Total = rd.Qty * rd.UnitPrice * r.PLRate, DisItem = rd.DiscountValue }).ToList();
            var receiptMemo = (from r in receipt
                               join rm in _context.ReceiptMemo on r.ReceiptID equals rm.BasedOn
                               group new { rm, r } by new { rm } into g
                               let data = g.FirstOrDefault()
                               select new
                               {
                                   data.rm.ID,
                                   GrandToralSys = data.rm.GrandTotalSys,
                                   GrandTotal = data.rm.GrandTotalSys * data.r.LocalSetRate,
                                   DisValue = data.rm.DisValue * data.r.PLRate,
                                   TaxValue = data.rm.TaxValue * data.r.PLRate,
                                   data.rm.LocalCurrencyID,
                                   data.rm.SysCurrencyID,
                                   data.rm.PLRate,
                                   UserID = data.r.UserOrderID,
                               }).ToList();
            var soldAountMemo = (from r in receiptMemo
                                 join rd in _context.ReceiptDetailMemoKvms on r.ID equals rd.ReceiptMemoID
                                 select new { Total = rd.Qty * rd.UnitPrice * r.PLRate, Disitem = rd.DisValue }).ToList();

            var incomingpayment = (from r in receipt
                                   join ipm in _context.IncomingPaymentCustomers on r.ReceiptNo equals ipm.InvoiceNumber
                                   group new { ipm, r } by new { ipm } into g
                                   let data = g.FirstOrDefault()
                                   select new
                                   {
                                       BalanceDue = data.ipm.BalanceDue
                                   }).ToList();

            double incmpy = incomingpayment.Sum(s => s.BalanceDue);
            double TotalBalanceDue = incomingpayment.Sum(s => s.BalanceDue) * -1;
            double TotalVAT = receipt.Sum(s => s.TaxValue * s.ExchangeRate) - receiptMemo.Sum(s => s.TaxValue);
            double GrandTotal = receipt.Sum(s => s.GrandTotal_Sys * s.LocalSetRate) - receiptMemo.Sum(s => s.GrandTotal);
            double GrandTotal_Sys = receipt.Sum(s => s.GrandTotal_Sys) - receiptMemo.Sum(s => s.GrandToralSys);
            double TotalDiscountTotal = receipt.Sum(s => s.DiscountValue * s.ExchangeRate) - receiptMemo.Sum(s => s.DisValue);
            double TotalSoldAmount = soldAount.Sum(s => s.Total) - soldAountMemo.Sum(s => s.Total);
            double TotalDisItem = soldAount.Sum(s => s.DisItem) - soldAountMemo.Sum(s => s.Disitem);
            var receiptDeial = (from r in receipt
                                join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                                join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                                join currSys in _context.Currency on r.SysCurrencyID equals currSys.ID
                                join item in _context.ItemMasterDatas on rd.ItemID equals item.ID
                                join g1 in _context.ItemGroup1 on item.ItemGroup1ID equals g1.ItemG1ID
                                join g2 in _context.ItemGroup2 on item.ItemGroup2ID equals g2.ItemG2ID
                                join g3 in _context.ItemGroup3 on item.ItemGroup3ID equals g3.ID
                                join uom in _context.UnitofMeasures on rd.UomID equals uom.ID
                                join u in _context.UserAccounts on r.UserOrderID equals u.ID
                                join e in _context.Employees on u.EmployeeID equals e.ID
                                select new CashoutReport
                                {
                                    ID = rd.ID,
                                    Code = item.Code,
                                    Barcode = item.Barcode,
                                    KhmerName = item.KhmerName,
                                    EnglishName = item.EnglishName,
                                    Qty = rd.Qty,
                                    Price = rd.UnitPrice * r.PLRate,
                                    DisItemValue = rd.DiscountValue * r.PLRate,
                                    Total = (rd.UnitPrice * rd.Qty) - rd.DiscountValue * r.PLRate,
                                    Currency = curr.Description,
                                    Currency_Sys = currSys.Description,
                                    TotalSoldAmount = TotalSoldAmount,
                                    Uom = uom.Name,
                                    EmpName = e.Name,
                                    ItemGroup1 = g1.Name,
                                    ItemGroup2 = g2.Name,
                                    ItemGroup3 = g3.Name,
                                    DateIn = Convert.ToDateTime(close.FirstOrDefault().DateIn).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeIn,
                                    DateOut = Convert.ToDateTime(close.FirstOrDefault().DateOut).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeOut,
                                    TimeIn = close.FirstOrDefault().TimeIn,
                                    TimeOut = close.FirstOrDefault().TimeOut,
                                    TotalVat = TotalVAT,
                                    GrandTotal = GrandTotal,
                                    GrandTotal_Sys = GrandTotal_Sys,
                                    TotalDiscountTotal = TotalDiscountTotal,
                                    TotalCashIn_Sys = close.FirstOrDefault().CashInAmount_Sys,
                                    TotalCashOut_Sys = close.FirstOrDefault().CashOutAmount_Sys,
                                    TotalDiscountItem = TotalDisItem,
                                    ExchangeRate = r.ExchangeRate,
                                    ItemId = item.ID,
                                }).ToList();

            var receiptDeialMemo = (from r in receiptMemo
                                    join rd in _context.ReceiptDetailMemoKvms on r.ID equals rd.ReceiptMemoID
                                    join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                                    join currSys in _context.Currency on r.SysCurrencyID equals currSys.ID
                                    join item in _context.ItemMasterDatas on rd.ItemID equals item.ID
                                    join g1 in _context.ItemGroup1 on item.ItemGroup1ID equals g1.ItemG1ID
                                    join g2 in _context.ItemGroup2 on item.ItemGroup2ID equals g2.ItemG2ID
                                    join g3 in _context.ItemGroup3 on item.ItemGroup3ID equals g3.ID
                                    join uom in _context.UnitofMeasures on rd.UomID equals uom.ID
                                    join u in _context.UserAccounts on r.UserID equals u.ID
                                    join e in _context.Employees on u.EmployeeID equals e.ID
                                    select new CashoutReport
                                    {
                                        ID = rd.ID,
                                        Code = item.Code,
                                        Barcode = item.Barcode,
                                        KhmerName = item.KhmerName,
                                        EnglishName = item.EnglishName,
                                        Qty = rd.Qty * -1,
                                        Price = rd.TotalSys * r.PLRate * -1,
                                        DisItemValue = rd.DisValue * r.PLRate * -1,
                                        Total = rd.Qty * rd.UnitPrice * r.PLRate * -1,
                                        Currency = curr.Description,
                                        Currency_Sys = currSys.Description,
                                        TotalSoldAmount = TotalSoldAmount,
                                        EmpName = e.Name,
                                        Uom = uom.Name,
                                        ItemGroup1 = g1.Name,
                                        ItemGroup2 = g2.Name,
                                        ItemGroup3 = g3.Name,
                                        DateIn = Convert.ToDateTime(close.FirstOrDefault().DateIn).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeIn,
                                        DateOut = Convert.ToDateTime(close.FirstOrDefault().DateOut).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeOut,
                                        TimeIn = close.FirstOrDefault().TimeIn,
                                        TimeOut = close.FirstOrDefault().TimeOut,
                                        TotalVat = TotalVAT,
                                        GrandTotal = GrandTotal,
                                        GrandTotal_Sys = GrandTotal_Sys,
                                        TotalDiscountTotal = TotalDiscountTotal,
                                        TotalDiscountItem = TotalDisItem,
                                        TotalCashIn_Sys = close.FirstOrDefault().CashInAmount_Sys,
                                        TotalCashOut_Sys = close.FirstOrDefault().CashOutAmount_Sys,
                                        ExchangeRate = r.PLRate,
                                        ItemId = item.ID
                                    }).ToList();
            var itemRedeem = (from sh in _context.OpenShift.Where(t => t.Trans_From == Tran_F)
                              join rd in _context.Redeems on sh.ID equals (int)rd.OpenShiftID
                              join rdd in _context.RedeemDetails on rd.ID equals rdd.RID
                              join item in _context.ItemMasterDatas on rdd.ItemID equals item.ID
                              join uom in _context.UnitofMeasures on rdd.UomID equals uom.ID
                              join u in _context.UserAccounts on rd.UserID equals u.ID
                              join e in _context.Employees on u.EmployeeID equals e.ID
                              group new { sh, rd, rdd, item, uom, u, e } by new { item.ID } into g
                              let s = g.FirstOrDefault()
                              select new CashoutReport
                              {
                                  ID = s.rd.ID,
                                  Code = s.item.Code,
                                  Barcode = s.item.Barcode,
                                  KhmerName = s.item.KhmerName,
                                  EnglishName = s.item.EnglishName,
                                  Qty = (double)g.Sum(q => q.rdd.Qty),
                                  Price = 0,
                                  DisItemValue = 0,
                                  Total = 0,
                                  Currency = "",
                                  Currency_Sys = "",
                                  TotalSoldAmount = TotalSoldAmount,
                                  EmpName = s.e.Name,
                                  Uom = s.uom.Name,
                                  ItemGroup1 = "Redeemed item ",
                                  ItemGroup2 = "",
                                  ItemGroup3 = "",
                                  DateIn = Convert.ToDateTime(close.FirstOrDefault().DateIn).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeIn,
                                  DateOut = Convert.ToDateTime(close.FirstOrDefault().DateOut).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeOut,
                                  TimeIn = close.FirstOrDefault().TimeIn,
                                  TimeOut = close.FirstOrDefault().TimeOut,
                                  TotalVat = TotalVAT,
                                  GrandTotal = GrandTotal,
                                  GrandTotal_Sys = GrandTotal_Sys,
                                  TotalDiscountTotal = TotalDiscountTotal,
                                  TotalDiscountItem = TotalDisItem,
                                  TotalCashIn_Sys = close.FirstOrDefault().CashInAmount_Sys,
                                  TotalCashOut_Sys = close.FirstOrDefault().CashOutAmount_Sys,
                                  ExchangeRate = 0,
                                  ItemId = s.item.ID
                              }).ToList();
            var allCashout = new List<CashoutReport>
            (receiptDeial.Count + receiptDeialMemo.Count + itemRedeem.Count);
            allCashout.AddRange(receiptDeial);
            allCashout.AddRange(receiptDeialMemo);
            allCashout.AddRange(itemRedeem);
            var _data = (from rm in allCashout
                         group new { rm } by new { rm.ItemGroup1, rm.Price, rm.ItemId } into g
                         let d = g.FirstOrDefault()
                         select new CashoutReportMaster
                         {
                             ID = d.rm.ID,
                             Code = d.rm.Code,
                             Barcode = d.rm.Barcode,
                             KhmerName = d.rm.KhmerName,
                             EnglishName = d.rm.EnglishName,
                             Qty = g.Sum(i => i.rm.Qty).ToString(),
                             Price = _fncModule.ToCurrency(d.rm.Price, baseCurrency.DecimalPlaces),
                             DisItemValue = _fncModule.ToCurrency(g.Sum(i => i.rm.DisItemValue), baseCurrency.DecimalPlaces),
                             Total = _fncModule.ToCurrency(g.Sum(i => i.rm.Total), baseCurrency.DecimalPlaces),
                             TotalCal = g.Sum(i => i.rm.Total),
                             Currency = d.rm.Currency,
                             Currency_Sys = d.rm.Currency_Sys,
                             TotalSoldAmount = _fncModule.ToCurrency(d.rm.TotalSoldAmount, baseCurrency.DecimalPlaces),
                             EmpName = d.rm.EmpName,
                             Uom = d.rm.Uom,
                             ItemGroup1 = d.rm.ItemGroup1,
                             ItemGroup2 = d.rm.ItemGroup2,
                             ItemGroup3 = d.rm.ItemGroup3,
                             DateIn = d.rm.DateIn,
                             DateOut = d.rm.DateOut,
                             TimeIn = d.rm.TimeIn,
                             TimeOut = d.rm.TimeOut,
                             TotalVat = _fncModule.ToCurrency(d.rm.TotalVat, baseCurrency.DecimalPlaces),
                             GrandTotal = _fncModule.ToCurrency(d.rm.GrandTotal, localCurrency.DecimalPlaces),
                             GrandTotal_Sys = _fncModule.ToCurrency(d.rm.GrandTotal_Sys, baseCurrency.DecimalPlaces),
                             TotalDiscountTotal = _fncModule.ToCurrency(d.rm.TotalDiscountTotal, baseCurrency.DecimalPlaces),
                             TotalDiscountItem = _fncModule.ToCurrency(d.rm.TotalDiscountItem, baseCurrency.DecimalPlaces),
                             TotalCashIn_Sys = _fncModule.ToCurrency(d.rm.TotalCashIn_Sys, baseCurrency.DecimalPlaces),
                             TotalCashOut_Sys = _fncModule.ToCurrency(d.rm.TotalCashOut_Sys, baseCurrency.DecimalPlaces),
                             ExchangeRate = _fncModule.ToCurrency(d.rm.ExchangeRate, baseCurrency.DecimalPlaces),
                             TotalCredites = _fncModule.ToCurrency(TotalBalanceDue, baseCurrency.DecimalPlaces)
                         }).ToList();
            if (_data.Count <= 0)
            {
                var emp = _context.UserAccounts.Include(i => i.Employee).FirstOrDefault(i => i.ID == UserID) ?? new Account.UserAccount();
                var localCur = _context.ExchangeRates.FirstOrDefault(i => i.CurrencyID == userSetting.LocalCurrencyID) ?? new ExchangeRate();
                _data.Add(new CashoutReportMaster
                {
                    TotalCashIn_Sys = _fncModule.ToCurrency(close.LastOrDefault().CashInAmount_Sys, baseCurrency.DecimalPlaces),
                    TotalCashOut_Sys = _fncModule.ToCurrency(close.LastOrDefault().CashOutAmount_Sys, baseCurrency.DecimalPlaces),
                    DateIn = Convert.ToDateTime(close.FirstOrDefault().DateIn).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeIn,
                    DateOut = Convert.ToDateTime(close.FirstOrDefault().DateOut).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeOut,
                    TimeIn = close.FirstOrDefault().TimeIn,
                    TimeOut = close.FirstOrDefault().TimeOut,
                    TotalVat = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    GrandTotal = _fncModule.ToCurrency(0M, localCurrency.DecimalPlaces),
                    GrandTotal_Sys = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    TotalDiscountTotal = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    TotalDiscountItem = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    Qty = "0",
                    Price = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    DisItemValue = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    Total = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    TotalSoldAmount = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    Currency = localCurrency.AltCurrency,
                    Currency_Sys = baseCurrency.BaseCurrency,
                    EmpName = emp.Employee.Name,
                    ExchangeRate_Display = _fncModule.ToCurrency(localCur.SetRate, baseCurrency.DecimalPlaces),
                    ItemGroup1 = "No Item",
                    IsNone = true,
                    TotalCredites = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces)
                });
            }
            return _data.OrderBy(i => i.ItemGroup1);
        }
        #region old getcashoutpayment
        #endregion

        public IEnumerable<CashoutReportMaster> GetCashoutPaymentMean(double Tran_F, double Tran_T, int UserID, int localCurId)
        {
            var receipt = _context.Receipt.Where(w => w.ReceiptID > Tran_F && w.ReceiptID <= Tran_T && w.UserOrderID == UserID);
            var userSetting = _context.GeneralSettings.FirstOrDefault(w => w.UserID == UserID) ?? new GeneralSetting();
            var currencies = GetDisplayPayCurrency(userSetting.PriceListID);
            var baseCurrency = currencies.FirstOrDefault(i => i.AltCurrencyID == userSetting.SysCurrencyID) ?? new DisplayPayCurrencyModel();
            var localCurrency = currencies.FirstOrDefault(i => i.AltCurrencyID == localCurId) ?? new DisplayPayCurrencyModel();
            var close = _context.CloseShift.Where(c => c.Trans_From == Tran_F && c.Trans_To == Tran_T && c.UserID == UserID);

            var SumDetail = from r in receipt
                            join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                            select new
                            {
                                TotalDisItem = rd.DiscountValue * r.ExchangeRate,
                                TotalSoldAmount = rd.Qty * rd.UnitPrice * r.ExchangeRate,
                                TotalDiscountItem = rd.DiscountValue * r.ExchangeRate,
                            };
            var receiptMemo = from cs in close
                              join rm in _context.ReceiptMemo on cs.OpenShiftID equals rm.OpenShiftID
                              group new { rm, cs } by new { rm } into g
                              let data = g.FirstOrDefault()
                              select new
                              {
                                  data.rm.ID,
                                  ReceiptNo = "RP " + data.rm.ReceiptNo,
                                  GrandToralSys = data.rm.GrandTotalSys,
                                  GrandTotal = data.rm.GrandTotalSys * data.rm.LocalSetRate,
                                  DisValue = data.rm.DisValue * data.rm.PLRate,
                                  TaxValue = data.rm.TaxValue * data.rm.PLRate,
                                  data.rm.LocalCurrencyID,
                                  data.rm.SysCurrencyID,
                                  data.rm.PLRate,
                                  UserID = data.rm.UserOrderID,
                                  PaymentID = data.rm.PaymentMeansID,
                                  GrandTotal_Sys = data.rm.GrandTotalSys,
                                  data.rm.ExchangeRate,
                                  data.rm.TimeIn,
                                  data.rm.TimeOut,
                                  data.rm.DateOut,
                              };
            var soldAountMemo = (from r in receiptMemo
                                 join rd in _context.ReceiptDetailMemoKvms on r.ID equals rd.ReceiptMemoID
                                 select new
                                 {
                                     Total = rd.Qty * rd.UnitPrice * r.PLRate,
                                     Disitem = rd.DisValue
                                 }).ToList();
            var incomingpayment = (from r in receipt
                                   join ipm in _context.IncomingPaymentCustomers on r.ReceiptNo equals ipm.InvoiceNumber
                                   group new { ipm, r } by new { ipm } into g
                                   let data = g.FirstOrDefault()
                                   select new
                                   {
                                       BalanceDue = data.ipm.BalanceDue
                                   }).ToList();
            double incmpy = incomingpayment.Sum(s => s.BalanceDue);
            double TotalBalanceDue = incomingpayment.Sum(s => s.BalanceDue) * -1;

            double TotalVAT = receipt.Sum(s => s.TaxValue * s.ExchangeRate) - receiptMemo.Sum(s => s.TaxValue);
            double GrandTotal = receipt.Sum(s => s.GrandTotal_Sys * s.LocalSetRate) - receiptMemo.Sum(s => s.GrandTotal);
            double GrandTotal_Sys = receipt.Sum(s => s.GrandTotal_Sys) - receiptMemo.Sum(s => s.GrandToralSys);
            double TotalDiscountTotal = receipt.Sum(s => s.DiscountValue * s.ExchangeRate) - receiptMemo.Sum(s => s.DisValue);
            double TotalSoldAmount = SumDetail.Sum(s => s.TotalSoldAmount) - soldAountMemo.Sum(s => s.Total);
            double TotalDisItem = SumDetail.Sum(s => s.TotalDiscountItem) - soldAountMemo.Sum(s => s.Disitem);
            


            var list = (from r in receipt
                        join multiPay in _context.MultiPaymentMeans on r.ReceiptID equals multiPay.ReceiptID
                        join pay in _context.PaymentMeans on multiPay.PaymentMeanID equals pay.ID
                        join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                        join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                        join u in _context.UserAccounts on r.UserOrderID equals u.ID
                        join e in _context.Employees on u.EmployeeID equals e.ID
                        select new CashoutReportMaster
                        {
                            KhmerName = r.ReceiptNo,
                            EnglishName = r.ReceiptNo,
                            Price = Convert.ToDateTime(r.DateOut).ToString("dd-MM-yyyy"),
                            DisItemValue = r.TimeOut,
                            Total =_fncModule.ToCurrency(((double)multiPay.Amount * multiPay.PLRate), baseCurrency.DecimalPlaces),
                            TotalCal = r.GrandTotal_Sys,
                            ItemGroup1 = pay.Type,
                            Currency = curr.Description,
                            Currency_Sys = curr_sys.Description,
                            TotalSoldAmount = _fncModule.ToCurrency(TotalSoldAmount, baseCurrency.DecimalPlaces),
                            TotalDiscountItem = _fncModule.ToCurrency(TotalDisItem, baseCurrency.DecimalPlaces),
                            TotalDiscountTotal = _fncModule.ToCurrency(TotalDiscountTotal, baseCurrency.DecimalPlaces),
                            TotalVat = _fncModule.ToCurrency(TotalVAT, baseCurrency.DecimalPlaces),
                            GrandTotal = _fncModule.ToCurrency(GrandTotal, localCurrency.DecimalPlaces),
                            GrandTotal_Sys = _fncModule.ToCurrency(GrandTotal_Sys, baseCurrency.DecimalPlaces),
                            TotalCashIn_Sys = _fncModule.ToCurrency(close.FirstOrDefault().CashInAmount_Sys, baseCurrency.DecimalPlaces),
                            TotalCashOut_Sys = _fncModule.ToCurrency(close.FirstOrDefault().CashOutAmount_Sys, baseCurrency.DecimalPlaces),
                            ExchangeRate = r.ExchangeRate.ToString(),
                            ExchangeRate_Display = r.DisplayRate.ToString(),
                            CurrencyDisplay = r.CurrencyDisplay,
                            DateIn = Convert.ToDateTime(close.FirstOrDefault().DateIn).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeIn,
                            DateOut = Convert.ToDateTime(close.FirstOrDefault().DateOut).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeOut,
                            TimeIn = close.FirstOrDefault().TimeIn,
                            TimeOut = close.FirstOrDefault().TimeOut,
                            EmpName = e.Name,
                            TotalCredites = _fncModule.ToCurrency(TotalBalanceDue, baseCurrency.DecimalPlaces)
                        }).ToList();
            //
            var memo = (from r in receiptMemo
                        join pay in _context.PaymentMeans on r.PaymentID equals pay.ID
                        join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                        join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                        join u in _context.UserAccounts on r.UserID equals u.ID
                        join e in _context.Employees on u.EmployeeID equals e.ID
                        select new CashoutReportMaster
                        {
                            KhmerName = r.ReceiptNo,
                            EnglishName = r.ReceiptNo,
                            Price = Convert.ToDateTime(r.DateOut).ToString("dd-MM-yyyy"),
                            DisItemValue = r.TimeOut,
                            //ReceiptNo = r.ReceiptNo,
                            Total = (r.GrandTotal_Sys * (-1)).ToString(),
                            TotalCal = r.GrandTotal_Sys * -1,
                            ItemGroup1 = pay.Type,
                            Currency = curr.Description,
                            Currency_Sys = curr_sys.Description,
                            TotalSoldAmount = _fncModule.ToCurrency(TotalSoldAmount, baseCurrency.DecimalPlaces),
                            TotalDiscountItem = _fncModule.ToCurrency(TotalDisItem, baseCurrency.DecimalPlaces),
                            TotalDiscountTotal = _fncModule.ToCurrency(TotalDiscountTotal, baseCurrency.DecimalPlaces),
                            TotalVat = _fncModule.ToCurrency(TotalVAT, baseCurrency.DecimalPlaces),
                            GrandTotal = _fncModule.ToCurrency(GrandTotal, localCurrency.DecimalPlaces),
                            GrandTotal_Sys = _fncModule.ToCurrency(GrandTotal_Sys, baseCurrency.DecimalPlaces),
                            TotalCashIn_Sys = _fncModule.ToCurrency(close.FirstOrDefault().CashInAmount_Sys, baseCurrency.DecimalPlaces),
                            TotalCashOut_Sys = _fncModule.ToCurrency(close.FirstOrDefault().CashOutAmount_Sys, baseCurrency.DecimalPlaces),
                            ExchangeRate = r.ExchangeRate.ToString(),
                            DateIn = Convert.ToDateTime(close.FirstOrDefault().DateIn).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeIn,
                            DateOut = Convert.ToDateTime(close.FirstOrDefault().DateOut).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeOut,
                            TimeIn = close.FirstOrDefault().TimeIn,
                            TimeOut = close.FirstOrDefault().TimeOut,
                            EmpName = e.Name,
                            TotalCredites = _fncModule.ToCurrency(TotalBalanceDue, baseCurrency.DecimalPlaces)
                            //ReceiptTime = r.TimeIn,
                            //ReceiptDate = Convert.ToDateTime(r.DateOut).ToString("dd-MM-yyyy"),
                        }).ToList();

            List<CashoutReportMaster> allCashOut = new(list.Count + memo.Count);
            allCashOut.AddRange(list);
            allCashOut.AddRange(memo);
            if (allCashOut.Count <= 0)
            {
                var emp = _context.UserAccounts.Include(i => i.Employee).FirstOrDefault(i => i.ID == UserID) ?? new Account.UserAccount();
                var localCur = _context.ExchangeRates.FirstOrDefault(i => i.CurrencyID == userSetting.LocalCurrencyID) ?? new ExchangeRate();
                allCashOut.Add(new CashoutReportMaster
                {
                    TotalCashIn_Sys = _fncModule.ToCurrency(close.LastOrDefault().CashInAmount_Sys, baseCurrency.DecimalPlaces),
                    TotalCashOut_Sys = _fncModule.ToCurrency(close.LastOrDefault().CashOutAmount_Sys, baseCurrency.DecimalPlaces),
                    DateIn = Convert.ToDateTime(close.FirstOrDefault().DateIn).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeIn,
                    DateOut = Convert.ToDateTime(close.FirstOrDefault().DateOut).ToString("dd-MM-yyyy") + " " + close.FirstOrDefault().TimeOut,
                    TimeIn = close.FirstOrDefault().TimeIn,
                    TimeOut = close.FirstOrDefault().TimeOut,
                    TotalVat = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    GrandTotal = _fncModule.ToCurrency(0M, localCurrency.DecimalPlaces),
                    GrandTotal_Sys = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    TotalDiscountTotal = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    TotalDiscountItem = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    DisItemValue = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    TotalSoldAmount = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    Total = _fncModule.ToCurrency(0M, baseCurrency.DecimalPlaces),
                    Currency = localCurrency.AltCurrency,
                    Currency_Sys = baseCurrency.BaseCurrency,
                    EmpName = emp.Employee.Name,
                    ExchangeRate_Display = _fncModule.ToCurrency(localCur.SetRate, baseCurrency.DecimalPlaces),
                    ReceiptNo = "No Item",
                    //ReceiptDate = "0",
                    //ReceiptTime = "0",
                    IsNone = true,
                    KhmerName = "",
                    EnglishName = "",
                    TotalCal = 0,
                    TotalCredites = "0",
                });
            }
            return allCashOut;
        }
        public IEnumerable<VoidItemReport> GetCashoutVoidItems(double Tran_F, double Tran_T, int userID)
        {
            double SGrandTotal = 0;
            var closshift = _context.CloseShift.FirstOrDefault(s => s.Trans_To == Tran_T && s.Trans_From == Tran_F);

            var _VoidItem = _context.VoidItems.Where(w => w.DateIn >= closshift.DateIn && w.DateIn <= closshift.DateOut && w.UserOrderID == userID && w.OpenShiftID == closshift.OpenShiftID).ToList();
            var STotal = (from vi in _VoidItem
                          join vd in _context.VoidItemDetails on vi.ID equals vd.VoidItemID
                          group new { vi, vd } by new { vi.ID, VIDD = vd.ID } into datas
                          let data = datas.FirstOrDefault()
                          select new
                          {
                              SSoldAmount = data.vd.Total * data.vi.PLRate,
                              SDiscountItem = data.vd.DiscountValue * data.vi.PLRate,
                              SDiscountTotal = data.vi.DiscountValue * data.vi.PLRate,
                          }).ToList();
            var IGrandTotal = _VoidItem.Sum(s => s.GrandTotal);

            var _VoidOrder = _context.VoidOrders.Where(w => w.DateIn >= closshift.DateIn && w.DateIn <= closshift.DateOut && w.UserOrderID == userID && w.OpenShiftID == closshift.OpenShiftID).ToList();
            var OTotal = (from vi in _VoidOrder
                          join vd in _context.VoidOrderDetails on vi.OrderID equals vd.OrderID
                          group new { vi, vd } by new { vi.OrderID, VIDD = vd.OrderDetailID } into datas
                          let data = datas.FirstOrDefault()
                          select new
                          {
                              SSoldAmount = data.vd.Total * data.vi.PLRate,
                              SDiscountItem = data.vd.DiscountValue * data.vi.PLRate,
                              SDiscountTotal = data.vi.DiscountValue * data.vi.PLRate,
                              GrandTotal = data.vd.Total,
                          }).ToList();
            var OGrandTotal = OTotal.Sum(s => s.GrandTotal);
            SGrandTotal = IGrandTotal + OGrandTotal;

            var vItem = _VoidItem;
            var _VdItems = (from vo in vItem
                            join VoidDetails in _context.VoidItemDetails on vo.ID equals VoidDetails.VoidItemID
                            join user in _context.UserAccounts on vo.UserOrderID equals user.ID
                            join emp in _context.BusinessPartners on vo.CustomerID equals emp.ID
                            join plc in _context.Currency on vo.PLCurrencyID equals plc.ID
                            join ssc in _context.Currency on vo.SysCurrencyID equals ssc.ID
                            join Uom in _context.UnitofMeasures on VoidDetails.UomID equals Uom.ID
                            group new { vo, user, emp, plc, ssc, VoidDetails, Uom } by new { vo.ID, VID = VoidDetails.ID } into datas
                            let data = datas.FirstOrDefault()
                            select new VoidItemReport
                            {
                                //Master
                                ID = data.vo.ID.ToString(),
                                IDD = data.VoidDetails.ID.ToString(),
                                OrderID = data.vo.OrderID,
                                OrderNo = data.vo.OrderNo,
                                UserName = data.user.Username,
                                DateIn = data.vo.DateIn.ToString("dd-MM-yyyy") + " " + data.vo.TimeIn,
                                DateOut = data.vo.DateOut.ToString("dd-MM-yyyy") + " " + data.vo.TimeOut,
                                DisTotal = string.Format("{0:#,0.000}", data.vo.DiscountValue),
                                SubTotal = data.plc.Description + " " + string.Format("{0:#,0.000}", data.vo.Sub_Total),
                                GTotal = data.plc.Description + " " + string.Format("{0:#,0.000}", data.vo.GrandTotal),
                                Reason = data.vo.Reason,
                                //Detail
                                ItemCode = data.VoidDetails.Code,
                                KhmerName = data.VoidDetails.KhmerName,
                                EnglishName = data.VoidDetails.EnglishName,
                                Qty = data.VoidDetails.Qty,
                                Uom = data.Uom.Name,
                                UnitPrice = string.Format("{0:#,0.000}", data.VoidDetails.UnitPrice),
                                DisItem = string.Format("{0:#,0.000}", data.VoidDetails.DiscountValue),
                                Total = string.Format("{0:#,0.000}", data.VoidDetails.Total),
                                //Summary

                                SSoldAmount = string.Format("{0:#,0.000}", STotal.Sum(s => s.SSoldAmount)),
                                SDiscountItem = string.Format("{0:#,0.000}", STotal.Sum(s => s.SDiscountItem)),
                                SDiscountTotal = string.Format("{0:#,0.000}", STotal.Sum(s => s.SDiscountTotal)),
                                SGrandTotal = data.ssc.Description + " " + string.Format("{0:#,0.000}", SGrandTotal),
                            }).ToList();

            var _VdOrders = (from vo in _VoidOrder
                             join VoidDetails in _context.VoidOrderDetails on vo.OrderID equals VoidDetails.OrderID
                             join user in _context.UserAccounts on vo.UserOrderID equals user.ID
                             join emp in _context.BusinessPartners on vo.CustomerID equals emp.ID
                             join plc in _context.Currency on vo.PLCurrencyID equals plc.ID
                             join ssc in _context.Currency on vo.SysCurrencyID equals ssc.ID
                             join Uom in _context.UnitofMeasures on VoidDetails.UomID equals Uom.ID
                             group new { vo, user, emp, plc, ssc, VoidDetails, Uom } by new { vo.OrderID, VID = VoidDetails.OrderDetailID } into datas
                             let data = datas.FirstOrDefault()
                             select new VoidItemReport
                             {
                                 //Master
                                 ID = data.vo.OrderID.ToString(),
                                 IDD = data.VoidDetails.OrderDetailID.ToString(),
                                 OrderID = data.vo.OrderID,
                                 OrderNo = data.vo.OrderNo,
                                 UserName = data.user.Username,
                                 DateIn = data.vo.DateIn.ToString("dd-MM-yyyy") + " " + data.vo.TimeIn,
                                 DateOut = data.vo.DateOut.ToString("dd-MM-yyyy") + " " + data.vo.TimeOut,
                                 DisTotal = string.Format("{0:#,0.000}", data.vo.DiscountValue),
                                 SubTotal = data.plc.Description + " " + string.Format("{0:#,0.000}", data.vo.Sub_Total),
                                 GTotal = data.plc.Description + " " + string.Format("{0:#,0.000}", data.vo.GrandTotal),
                                 Reason = data.vo.Reason,
                                 //Detail
                                 ItemCode = data.VoidDetails.Code,
                                 KhmerName = data.VoidDetails.KhmerName,
                                 EnglishName = data.VoidDetails.EnglishName,
                                 Qty = data.VoidDetails.Qty,
                                 Uom = data.Uom.Name,
                                 UnitPrice = string.Format("{0:#,0.000}", data.VoidDetails.UnitPrice),
                                 DisItem = string.Format("{0:#,0.000}", data.VoidDetails.DiscountValue),
                                 Total = string.Format("{0:#,0.000}", data.VoidDetails.Total),
                                 //Summary

                                 SSoldAmount = string.Format("{0:#,0.000}", STotal.Sum(s => s.SSoldAmount)),
                                 SDiscountItem = string.Format("{0:#,0.000}", STotal.Sum(s => s.SDiscountItem)),
                                 SDiscountTotal = string.Format("{0:#,0.000}", STotal.Sum(s => s.SDiscountTotal)),
                                 SGrandTotal = data.ssc.Description + " " + string.Format("{0:#,0.000}", SGrandTotal),
                             }).ToList();

            var allSummarySale = new List<VoidItemReport>(_VdItems.Count + _VdOrders.Count);
            allSummarySale.AddRange(_VdItems);
            allSummarySale.AddRange(_VdOrders);

            var allVoids = (from all in allSummarySale
                            group new { all } by new { all.ID, all.IDD } into r
                            let data = r.FirstOrDefault()
                            select new VoidItemReport
                            {
                                OrderNo = data.all.OrderNo,
                                UserName = data.all.UserName,
                                DateIn = data.all.DateIn,
                                DateOut = data.all.DateOut,
                                DisTotal = data.all.DisTotal,
                                SubTotal = data.all.SubTotal,
                                GTotal = data.all.GTotal,
                                Reason = data.all.Reason,
                                ItemCode = data.all.ItemCode,
                                KhmerName = data.all.KhmerName,
                                EnglishName = data.all.EnglishName,
                                Qty = data.all.Qty,
                                Uom = data.all.Uom,
                                UnitPrice = data.all.UnitPrice,
                                DisItem = data.all.DisItem,
                                Total = data.all.Total,
                                SSoldAmount = data.all.SSoldAmount,
                                SDiscountItem = data.all.SDiscountItem,
                                SDiscountTotal = data.all.SDiscountTotal,
                                SGrandTotal = data.all.SGrandTotal,
                            }).ToList();
            return allVoids;

        }
        public async Task<List<SaleByCustomer>> GetSaleByCustomers(string dateFrom, string dateTo, int branchId, int cusId, int curlcId)
        {
            List<Receipt> receiptsFilter = new();
            List<ReceiptMemo> receiptsMemoFilter = new();
            DateTime _dateFrom = Convert.ToDateTime(dateFrom);
            DateTime _dateTo = Convert.ToDateTime(dateTo);
            if (dateFrom != null && dateTo != null && branchId == 0 && cusId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo).ToList();
                receiptsMemoFilter = _context.ReceiptMemo.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId != 0 && cusId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId).ToList();
                receiptsMemoFilter = _context.ReceiptMemo.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId == 0 && cusId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.CustomerID == cusId).ToList();
                receiptsMemoFilter = _context.ReceiptMemo.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.CustomerID == cusId).ToList();
            }
            else if (dateFrom != null && dateTo != null && branchId != 0 && cusId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId && w.CustomerID == cusId).ToList();
                receiptsMemoFilter = _context.ReceiptMemo.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchId && w.CustomerID == cusId).ToList();
            }
            else
            {
                receiptsFilter = _context.Receipt.ToList();
                receiptsMemoFilter = _context.ReceiptMemo.ToList();
            }

            var receipts = (from rd in _context.ReceiptDetail
                            join r in receiptsFilter on rd.ReceiptID equals r.ReceiptID
                            join series in _context.Series on r.SeriesID equals series.ID
                            group new { r, rd, series } by new { r.CustomerID, r.ReceiptID, rd.ID } into g
                            let data = g.FirstOrDefault()
                            select new SaleByCustomer
                            {
                                CurLCId = data.r.LocalCurrencyID,
                                CurPLId = data.r.PLCurrencyID,
                                CurSysId = data.r.SysCurrencyID,
                                CusId = data.r.CustomerID,
                                ID = data.r.ReceiptID,
                                DetailID = data.rd.ID,
                                UomId = data.rd.UomID,
                                UserId = data.r.UserOrderID,
                                MReceiptNo = $"{data.series.Name}-{data.r.ReceiptNo}",
                                MReceiptGroup = $"{data.series.Name}-{data.r.ReceiptNo}",
                                Qty = data.rd.Qty,
                                TotalCal = data.rd.Total,
                                UnitPriceCal = data.rd.UnitPrice,
                                DiscountItem = data.rd.DiscountValue,
                                TaxValue = (double)data.rd.TaxValue,
                                MCusTotal = data.r.Sub_Total,
                                ItemId = data.rd.ItemID,
                                GrandTotal = data.r.GrandTotal,
                                Rate = data.r.PLRate,
                                RateLocal = data.r.LocalSetRate,
                                DiscountTotal = data.r.DiscountValue,
                                BranchId = data.r.BranchID,
                                DateIn = data.r.DateIn.ToString("dd-MM-yyyy"),
                                DateOut = data.r.DateOut.ToString("dd-MM-yyyy"),
                            }).ToList();
            var receiptsMemo = (from rd in _context.ReceiptDetailMemoKvms
                                join r in receiptsMemoFilter on rd.ReceiptMemoID equals r.ID
                                join reciept in receiptsFilter on r.BasedOn equals reciept.ReceiptID
                                join series in _context.Series on reciept.SeriesID equals series.ID
                                join series1 in _context.Series on r.SeriesID equals series1.ID
                                group new { r, rd, series, reciept, series1 } by new { r.CustomerID, ReceiptKvmsID = r.ID, rd.ID } into g
                                let data = g.FirstOrDefault()
                                select new SaleByCustomer
                                {
                                    CurLCId = data.r.LocalCurrencyID,
                                    CurPLId = data.r.PLCurrencyID,
                                    CurSysId = data.r.SysCurrencyID,
                                    CusId = data.r.CustomerID,
                                    ID = data.r.ID,
                                    DetailID = data.rd.ID,
                                    UomId = data.rd.UomID,
                                    UserId = data.r.UserOrderID,
                                    MReceiptNo = $"{data.series.Name}-{data.reciept.ReceiptNo}",
                                    MReceiptGroup = $"{data.series1.Name}-{data.r.ReceiptNo}",
                                    Qty = data.rd.Qty * -1,
                                    TotalCal = data.rd.Total * -1,
                                    UnitPriceCal = data.rd.UnitPrice,
                                    DiscountItem = data.rd.DisValue * -1,
                                    TaxValue = (double)data.rd.TaxValue * -1,
                                    MCusTotal = data.r.SubTotal * -1,
                                    ItemId = data.rd.ItemID,
                                    GrandTotal = data.r.GrandTotal * -1,
                                    Rate = data.r.PLRate,
                                    RateLocal = data.r.LocalSetRate,
                                    DiscountTotal = data.r.DisValue,
                                    BranchId = data.r.BranchID,
                                    DateIn = data.r.DateIn.ToString("dd-MM-yyyy"),
                                    DateOut = data.r.DateOut.ToString("dd-MM-yyyy"),
                                }).ToList();

            List<SaleByCustomer> allData = new(receipts.Count + receiptsMemo.Count);
            allData.AddRange(receipts);
            allData.AddRange(receiptsMemo);
            var allDataGrouping = allData.GroupBy(i => i.MReceiptGroup).Select(i => i.FirstOrDefault()).ToList();
            int countReciept = allData.GroupBy(i => i.MReceiptNo).Select(i => i.FirstOrDefault()).ToList().Count;
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == curlcId) ?? new Display();
            var _data = (from d in allData
                         join user in _context.UserAccounts.Include(i => i.Employee) on d.UserId equals user.ID
                         join cus in _context.BusinessPartners on d.CusId equals cus.ID
                         join uom in _context.UnitofMeasures on d.UomId equals uom.ID
                         join curr_sys in _context.Currency on d.CurSysId equals curr_sys.ID
                         join curr in _context.Currency on d.CurLCId equals curr.ID
                         join curr_pl in _context.Currency on d.CurPLId equals curr_pl.ID
                         join item in _context.ItemMasterDatas on d.ItemId equals item.ID
                         join branch in _context.Branches on d.BranchId equals branch.ID
                         group new { d, cus, uom, curr_sys, curr, curr_pl, item, branch } by new { d.CusId, d.ID, d.DetailID } into g
                         let s = g.FirstOrDefault()
                         let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == s.curr_pl.ID) ?? new Display()
                         let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == s.curr_sys.ID) ?? new Display()
                         let sub = allDataGrouping.Where(i => i.MReceiptNo == s.d.MReceiptNo).Sum(i => i.MCusTotal)
                         select new SaleByCustomer
                         {
                             //Master
                             MCustomer = s.cus.Name,
                             MBranch = s.branch.Name,
                             MCusTotal = s.d.MCusTotal,
                             MReceiptNo = s.d.MReceiptNo,
                             MSubTotal = s.curr_pl.Description + " " + _fncModule.ToCurrency(sub, plCur.Amounts),
                             DateIn = s.d.DateIn,
                             DateOut = s.d.DateOut,
                             PLCurrency = s.curr_pl.Description,
                             //MSubTotalAmount = data.curr_pl.Description + " " + _fncModule.ToCurrency(SubTotalAmount, plCur.Amounts),
                             //Detail
                             ID = s.d.ID,
                             DetailID = s.d.DetailID,
                             ItemCode = s.item.Code,
                             KhmerName = s.item.KhmerName,
                             Qty = s.d.Qty,
                             Uom = s.uom.Name,
                             UnitPrice = _fncModule.ToCurrency(s.d.UnitPriceCal, plCur.Amounts),
                             UnitPriceCal = s.d.UnitPriceCal,
                             DisItem = _fncModule.ToCurrency(s.d.DiscountItem, plCur.Amounts),
                             Total = _fncModule.ToCurrency(s.d.TotalCal, plCur.Amounts),
                             TotalCal = s.d.TotalCal,
                             //PLC = data.curr_pl.Description,
                             //Summary
                             DateFrom = _dateFrom.ToString("dd-MM-yyyy"),
                             DateTo = _dateTo.ToString("dd-MM-yyyy"),
                             SCount = string.Format("{0:n0}", countReciept),
                             SDiscountItem = _fncModule.ToCurrency(allData.Sum(i => i.DiscountItem) * s.d.Rate, sysCur.Amounts),
                             SDiscountTotal = _fncModule.ToCurrency(allDataGrouping.Sum(i => i.DiscountTotal) * s.d.Rate, sysCur.Amounts),
                             SVat = s.curr_sys.Description + " " + _fncModule.ToCurrency(allData.Sum(i => i.TaxValue) * s.d.Rate, sysCur.Amounts),
                             SGrandTotalSys = s.curr_sys.Description + " " + _fncModule.ToCurrency(allDataGrouping.Sum(i => i.GrandTotal) * s.d.Rate, sysCur.Amounts),
                             SGrandTotal = s.curr.Description + " " + _fncModule.ToCurrency(allDataGrouping.Sum(i => i.GrandTotal) * s.d.RateLocal, lcCur.Amounts),
                         }).ToList();
            return await Task.FromResult(_data);
        }
        List<DisplayPayCurrencyModel> GetDisplayPayCurrency(int priceListId)
        {
            PriceLists priceList = GetPriceList(priceListId);
            var cur = _context.Currency.FirstOrDefault(i => i.ID == priceList.CurrencyID) ?? new Currency();
            var dcs = _context.DisplayCurrencies.Where(i => i.PriceListID == priceListId).ToList();
            var dc = dcs.FirstOrDefault(i => i.IsActive) ?? new DisplayCurrency();
            var altCurrencies = (from c in _context.Currency.Where(i => !i.Delete)
                                 let plBasedDc = dcs.FirstOrDefault(dc => dc.PriceListID == priceList.ID && c.ID == dc.AltCurrencyID) ?? new DisplayCurrency()
                                 select new DisplayPayCurrencyModel
                                 {
                                     LineID = $"{DateTime.Now.Ticks}{c.ID}",
                                     AltCurrency = c.Description,
                                     BaseCurrency = cur.Description,
                                     Amount = 0,
                                     AltAmount = 0,
                                     Rate = (decimal)plBasedDc.PLDisplayRate,
                                     AltRate = (decimal)plBasedDc.DisplayRate,
                                     BaseCurrencyID = cur.ID,
                                     AltCurrencyID = c.ID,
                                     AltSymbol = c.Symbol,
                                     BaseSymbol = cur.Symbol,
                                     IsLocalCurrency = dc.AltCurrencyID == 0 ? c.ID == cur.ID : c.ID == dc.AltCurrencyID,
                                     IsShowCurrency = plBasedDc.IsShowCurrency,
                                     IsActive = plBasedDc.IsActive,
                                     IsShowOtherCurrency = plBasedDc.IsShowOtherCurrency,
                                     DecimalPlaces = plBasedDc.DecimalPlaces,
                                 }).ToList();
            return altCurrencies;
        }
        PriceLists GetPriceList(int priceListId)
        {
            return _context.PriceLists.Include(p => p.Currency)
                     .FirstOrDefault(p => p.ID == priceListId)
                     ?? new PriceLists
                     {
                         Currency = new Currency()
                     };
        }
    }
}
