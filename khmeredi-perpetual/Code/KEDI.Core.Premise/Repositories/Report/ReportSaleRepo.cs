using CKBS.AppContext;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.ReportSale.dev;
using CKBS.Models.ServicesClass.Report;
using KEDI.Core.Premise.Models.Sale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository.Report
{
    public class ReportSaleRepo
    {
        readonly DataContext _dataContext;
        public ReportSaleRepo(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<DevSummarySale> GetSummarySalesPos(DateTime dateFrom, DateTime dateTo, TimeSpan timeFrom, TimeSpan timeTo, 
            int branchId, int userId, int priceListId)
        {
            List<Receipt> receipts = new ();
            List<SaleAR> saleARs = new ();
            receipts = _dataContext.Receipt.Where(w => w.DateOut >= dateFrom && w.DateOut <= dateTo).ToList();
            if (branchId > 0)
            {
                receipts = receipts.Where(r => r.BranchID == branchId).ToList();
            }

            if (userId > 0)
            {
                receipts = receipts.Where(r => r.UserOrderID == userId).ToList();
            }

            if(priceListId > 0)
            {
                receipts = receipts.Where(r => r.PriceListID == priceListId).ToList();
            }

            var totalSales = GetSummaryTotals(dateFrom, dateTo, timeFrom, timeTo, branchId, userId);
            var saleReceipts = (from r in receipts
                        join user in _dataContext.UserAccounts on r.UserOrderID equals user.ID
                        join emp in _dataContext.Employees on user.EmployeeID equals emp.ID
                        join curr_pl in _dataContext.Currency on r.PLCurrencyID equals curr_pl.ID
                        join curr in _dataContext.Currency on r.LocalCurrencyID equals curr.ID
                        join curr_sys in _dataContext.Currency on r.SysCurrencyID equals curr_sys.ID
                        join b in _dataContext.Branches on r.BranchID equals b.ID
                        group new { r, emp, curr_pl, curr_sys, curr, b } by new { r.BranchID, r.ReceiptID } into datas
                        let data = datas.FirstOrDefault()
                        let douType = _dataContext.DocumentTypes.FirstOrDefault(w => w.Code == "SP")
                        let sumByBranch = receipts.Where(_r => _r.BranchID == data.r.BranchID).Sum(_as => _as.GrandTotal_Sys)
                        select new DevSummarySale
                        {
                            //detail
                            ReceiptID = data.r.SeriesDID,
                            DouType = douType.Code,
                            EmpCode = data.emp.Code,
                            EmpName = data.emp.Name,
                            BranchID = data.r.BranchID,
                            BranchName = data.b.Name,
                            ReceiptNo = data.r.ReceiptNo,
                            DateOut = data.r.DateOut.ToString("dd-MM-yyyy"),
                            TimeOut = data.r.TimeOut,
                            DiscountItem = string.Format("{0:#,0.000}", data.r.DiscountValue),
                            Currency = data.curr_pl.Description,
                            GrandTotal = string.Format("{0:#,0.000}", data.r.GrandTotal),
                            //Summary
                            DateFrom = dateFrom.ToString("dd-MM-yyyy"),
                            DateTo = dateTo.ToString("dd-MM-yyyy"),
                            //SCount = ChCount.ToString(),
                            GrandTotalBrand = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", sumByBranch),
                            //SCount = string.Format("{0:n0}", Summary.FirstOrDefault().CountReceipt),
                            SDiscountItem = string.Format("{0:#,0.000}", totalSales.FirstOrDefault().DiscountItem),
                            SDiscountTotal = string.Format("{0:#,0.000}", totalSales.FirstOrDefault().DiscountTotal),
                            SVat = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", totalSales.FirstOrDefault().TaxValue),
                            SGrandTotalSys = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", totalSales.FirstOrDefault().GrandTotalSys),
                            SGrandTotal = data.curr.Description + " " + string.Format("{0:#,0.000}", receipts.Sum(s => s.GrandTotal_Sys) * receipts.FirstOrDefault().LocalSetRate),
                            //
                            TotalDiscountItem = (decimal)_dataContext.ReceiptDetail.Where(w => w.ReceiptID == data.r.ReceiptID).Sum(s => s.DiscountValue),
                            DiscountTotal = data.r.DiscountValue,
                            Vat = data.r.TaxValue * data.r.ExchangeRate,
                            GrandTotalSys = data.r.GrandTotal_Sys,
                            MGrandTotal = data.r.GrandTotal_Sys * data.r.LocalSetRate,
                        }).ToList();
            return saleReceipts;
        }

        public List<DevSummarySale> GetSummarySalesARs(DateTime dateFrom, DateTime dateTo, TimeSpan timeFrom, TimeSpan timeTo,
          int branchId, int userId, int priceListId)
        {
            double totalDisTotal = 0;
            double totalVat = 0;
            double grandTotalSys = 0;
            double grandTotal = 0;
            var saleARs = _dataContext.SaleARs.Where(w => w.PostingDate >= dateFrom && w.PostingDate <= dateTo).ToList();
          
            var saleARDetails = _dataContext.SaleARDetails.ToList();
            totalDisTotal = saleARs.Sum(s => s.DisValue);
            totalVat = saleARs.Sum(s => s.DisValue);
            grandTotalSys = saleARs.Sum(s => s.TotalAmountSys);
            grandTotal = saleARs.Sum(s => s.TotalAmountSys * s.LocalSetRate);
            var _saleARs = (from sar in saleARs
                            join user in _dataContext.UserAccounts on sar.UserID equals user.ID
                            join com in _dataContext.Company on user.CompanyID equals com.ID
                            join emp in _dataContext.Employees on user.EmployeeID equals emp.ID
                            join curr_pl in _dataContext.Currency on sar.SaleCurrencyID equals curr_pl.ID
                            join curr in _dataContext.Currency on sar.LocalCurID equals curr.ID
                            join curr_sys in _dataContext.Currency on com.SystemCurrencyID equals curr_sys.ID
                            join dt in _dataContext.DocumentTypes on sar.DocTypeID equals dt.ID
                            join b in _dataContext.Branches on sar.BranchID equals b.ID
                            group new { sar, emp, curr_pl, curr_sys, curr, dt, b } by new { sar.BranchID, sar.SARID } into datas
                            let data = datas.FirstOrDefault()
                            let sumByBranch = saleARs.Where(_r => _r.BranchID == data.sar.BranchID).Sum(_as => _as.TotalAmountSys)
                            let sards = saleARDetails.Where(ard => ard.SARID == data.sar.SARID)
                            select new DevSummarySale
                            {
                                //detail
                                ReceiptID = data.sar.SeriesDID,
                                DouType = data.dt.Code,
                                EmpCode = data.emp.Code,
                                EmpName = data.emp.Name,
                                BranchID = data.sar.BranchID,
                                BranchName = data.b.Name,
                                ReceiptNo = data.sar.InvoiceNo,
                                DateOut = data.sar.PostingDate.ToString("dd-MM-yyyy"),
                                TimeOut = "",
                                DiscountItem = string.Format("{0:#,0.000}", data.sar.DisValue),
                                Currency = data.curr_pl.Description,
                                GrandTotal = string.Format("{0:#,0.000}", data.sar.TotalAmount),
                                //Summary
                                DateFrom = dateFrom.ToString("dd-MM-yyyy"),
                                DateTo = dateTo.ToString("dd-MM-yyyy"),
                                //SCount = ChCount.ToString(),
                                GrandTotalBrand = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", sumByBranch),
                                SDiscountItem = string.Format("{0:#,0.000}", sards.Sum(ard => ard.DisValue * data.sar.ExchangeRate)),
                                SDiscountTotal = string.Format("{0:#,0.000}", sards.Sum(ard => ard.DisValue * data.sar.ExchangeRate)),
                                SVat = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", totalVat),
                                SGrandTotalSys = data.curr_sys.Description + " " + string.Format("{0:#,0.000}", grandTotalSys),
                                SGrandTotal = data.curr.Description + " " + string.Format("{0:#,0.000}", grandTotal),
                                //
                                TotalDiscountItem = (decimal)sards.Sum(ard => ard.DisValue),
                                DiscountTotal = data.sar.DisValue,
                                Vat = data.sar.VatValue * data.sar.ExchangeRate,
                                GrandTotalSys = data.sar.TotalAmountSys,
                                MGrandTotal = data.sar.TotalAmountSys * data.sar.LocalSetRate,
                            }).ToList();
            return _saleARs;
        }

        public List<SummaryTotalSale> GetSummaryTotals(DateTime dateFrom, DateTime dateTo, TimeSpan timeFrom, TimeSpan timeTo, int branchID, int userID)
        {
            try
            {
                var _tf = timeFrom.ToString("hh\\:mm");
                var _tt = timeTo.ToString("hh\\:mm");
                var data = _dataContext.SummaryTotalSale.FromSql("rp_GetSummarrySaleTotal @DateFrom={0},@DateTo={1}, @BranchID={2},@UserID={3},@TimeFrom={4},@TimeTo={5}",
                parameters: new[] {
                    dateFrom.ToString("yyyy/MM/dd"),
                    dateTo.ToString("yyyy/MM/dd"),
                    branchID.ToString(),
                    userID.ToString(),
                    timeFrom.ToString("hh\\:mm"),
                    timeTo.ToString("hh\\:mm")
                }).ToList();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
