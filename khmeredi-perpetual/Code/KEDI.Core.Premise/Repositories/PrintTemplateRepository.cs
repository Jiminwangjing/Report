using CKBS.AppContext;
using CKBS.Controllers;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Inventory.Property;
using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.POS.service;
using CKBS.Models.Services.Purchase.Print;
using CKBS.Models.Services.ReportSale.dev;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass.Report;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.Sale.Print;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Repository
{
    public interface IPrintTemplateRepository
    {
        List<PrintPurchaseAP> GetPrintPurchaseAP(int purchaseId);
        List<PrintPurchaseAP> GetPrintPurchaseCreditmemo(int purchaseid);
        List<PrintPurchaseAP> GetPrintPurchaseAPResers(int purcahseid);
        List<PrintPurchaseAP> GetPrintPurchaseRequest(int purcahseid);
        List<PrintPurchaseAP> GetPurchaseOrder(int purchaseid);
        List<PrintPurchaseAP> GetPurchaseQuotation(int purchaseid);
        List<PrintPurchaseAP> GetPurchasePO(int purchaseid);
        List<PrintSaleHistory> GetSaleARData(int id);
        List<PrintBill> GetPOSData(int id);
        Task<List<PrintBill>> GetRePrintPOSData(int id, int userid);
        List<PrintSaleHistory> GetSaleARDataEdit(int id);
        List<PrintSaleHistory> GetSaleARReserve(int id);
        List<PrintSaleHistory> GetSaleARReserveEdit(int id);
        List<PrintSaleHistory> GetSaleQoute(int id);
        List<PrintSaleHistory> GetSaleOrderHistory(int id);
        List<PrintSaleHistory> GetSaleDelivery(int id);
        List<PrintSaleHistory> GetReturnDeilivery(int id);
        List<PrintSaleHistory> GetSaleARDownPaymentHistory(int id);
        List<PrintSaleHistory> GetSaleCreditmemo(int id);
        List<GroupSummarySale> GetSaleSummary(string dateFrom, string dateTo, int branchID, int userID, string timeFrom, string timeTo, int plid, string doctype);
        List<SummaryTotalSale> GetSummaryTotals(string dateFrom, string dateTo, int branchID, int userID, string timeFrom, string timeTo);
        List<SummaryTotalSale> GetSummaryTotals(string dateFrom, string dateTo, int branchID, int userID, string timeFrom, string timeTo, int plid);
        List<DevSummarySale> GetSaleCrditMemoReport(string DateFrom, string DateTo, string TimeFrom, string TimeTo, int BranchID, int UserID, string DouType);

        //IActionResult GetAP(int PurchaseID);
    }
    public class PrintTemplateRepository : IPrintTemplateRepository
    {
        private readonly DataContext _context;
        private readonly UserAccount _user;
        private readonly UtilityModule _fncModule;
        private readonly UtilityModule _utility;
        readonly IPOS _pos;
        public PrintTemplateRepository(DataContext context, UserManager userModule, UtilityModule utilityModule, UtilityModule utility, IPOS pos)
        {
            _context = context;
            _user = userModule.CurrentUser;
            _fncModule = utilityModule;
            _utility = utility;
            _pos = pos;
        }

        public List<GroupSummarySale> GetSaleSummary(string dateFrom, string dateTo, int branchID, int userID, string timeFrom, string timeTo, int plid, string doctype)
        {
            List<Receipt> receiptsFilter = new();
            List<SaleAR> saleArFilter = new();
            DateTime _dateFrom = Convert.ToDateTime(dateFrom);
            DateTime _dateTo = Convert.ToDateTime(dateTo);
            bool isDateFrom = !string.IsNullOrEmpty(dateFrom);
            bool isDateTo = !string.IsNullOrEmpty(dateTo);
            bool isTimeFrom = timeFrom != "0";
            bool isTimeTo = timeTo != "0";
            if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && branchID == 0 && userID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo).ToList();
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= _dateFrom && w.PostingDate <= _dateTo).ToList();
            }
            else if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && branchID != 0 && userID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchID).ToList();
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= _dateFrom && w.PostingDate <= _dateTo && w.BranchID == branchID).ToList();
            }
            else if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && branchID != 0 && userID != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.BranchID == branchID && w.UserOrderID == userID).ToList();
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= _dateFrom && w.PostingDate <= _dateTo && w.BranchID == branchID && w.UserID == userID).ToList();
            }
            else if (isDateFrom && isDateTo && isTimeFrom && isTimeTo && branchID == 0 && userID == 0)
            {
                DateTime __dateFrom = DateTime.Parse(string.Format("{0} {1}", dateFrom, timeFrom));
                DateTime __dateTo = DateTime.Parse(string.Format("{0} {1}", dateTo, timeTo));
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= _dateFrom && w.PostingDate <= _dateTo).ToList();
                receiptsFilter = _context.Receipt.Where(w => Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= __dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= __dateTo).ToList();
            }
            else if (isDateFrom && isDateTo && isTimeFrom && isTimeTo && branchID != 0 && userID == 0)
            {
                DateTime __dateFrom = DateTime.Parse(string.Format("{0} {1}", dateFrom, timeFrom));
                DateTime __dateTo = DateTime.Parse(string.Format("{0} {1}", dateTo, timeTo));
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= _dateFrom && w.PostingDate <= _dateTo && w.BranchID == branchID).ToList();
                receiptsFilter = _context.Receipt.Where(w => Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= __dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= __dateTo && w.BranchID == branchID).ToList();
            }
            else if (isDateFrom && isDateTo && isTimeFrom && isTimeTo && branchID != 0 && userID != 0)
            {
                DateTime __dateFrom = DateTime.Parse(string.Format("{0} {1}", dateFrom, timeFrom));
                DateTime __dateTo = DateTime.Parse(string.Format("{0} {1}", dateTo, timeTo));
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= _dateFrom && w.PostingDate <= _dateTo && w.BranchID == branchID && w.UserID == userID).ToList();
                receiptsFilter = _context.Receipt.Where(w => Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= __dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= __dateTo && w.BranchID == branchID && w.UserOrderID == userID).ToList();
            }
            List<SummaryTotalSale> Summary = new();
            var Branch = "All";
            var EmpName = "All";
            var Logo = "";
            var TimeF = "";
            var TimeT = "";
            if (isTimeFrom && isTimeTo)
            {
                TimeF = Convert.ToDateTime(timeFrom).ToString("hh:mm tt");
                TimeT = Convert.ToDateTime(timeTo).ToString("hh:mm tt");
            }
            if (branchID != 0)
            {
                Branch = _context.Branches.FirstOrDefault(w => w.ID == branchID).Name;
                Logo = _context.Branches.Include(c => c.Company).FirstOrDefault(w => w.ID == branchID).Company.Logo;
            }
            if (userID != 0)
            {
                EmpName = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == userID).Employee.Name;
            }
            List<GroupSummarySale> Sale = new();
            List<GroupSummarySale> saleAr = new();
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == _user.Company.LocalCurrencyID) ?? new Display();
            var sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == _user.Company.SystemCurrencyID) ?? new Display();

            if (doctype == "SP" || doctype == "All")
            {
                var Receipts = receiptsFilter;
                string tf = !isTimeFrom ? "00:00" : timeFrom;
                string tt = !isTimeTo ? "00:00" : timeTo;
                string df = !isDateFrom ? "1900-01-01" : dateFrom;
                string dt = !isDateTo ? "1900-01-01" : dateTo;
                if (plid > 0)
                {
                    Receipts = Receipts.Where(i => i.PriceListID == plid).ToList();
                    Summary = GetSummaryTotals(dateFrom, dateTo, branchID, userID, tf, tt, plid) ?? new List<SummaryTotalSale>();
                }
                else
                {
                    Summary = GetSummaryTotals(dateFrom, dateTo, branchID, userID, tf, tt) ?? new List<SummaryTotalSale>();
                }
                Sale = (from r in Receipts
                        join user in _context.UserAccounts on r.UserOrderID equals user.ID
                        join emp in _context.Employees on user.EmployeeID equals emp.ID
                        join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                        join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                        join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                        group new { r, emp, curr_pl, curr_sys, curr } by new { emp.ID } into datas
                        let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == datas.FirstOrDefault().curr_pl.ID) ?? new Display()
                        //let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == datas.FirstOrDefault().curr_sys) ?? new Display()
                        select new GroupSummarySale
                        {
                            EmpCode = datas.First().emp.Code,
                            EmpName = datas.First().emp.Name,
                            SubTotal = datas.Sum(s => s.r.GrandTotal),
                            Receipts = datas.Select(receipt => new Receipts
                            {
                                User = receipt.emp.Name,
                                Receipt = receipt.r.ReceiptNo,
                                DateIn = receipt.r.DateIn.ToString("dd-MM-yyyy"),
                                TimeIn = receipt.r.TimeIn,
                                DateOut = receipt.r.DateOut.ToString("dd-MM-yyyy"),
                                TimeOut = receipt.r.TimeOut,
                                DisInv = receipt.r.DiscountValue,
                                Currency = receipt.curr_pl.Description,
                                GrandTotal = receipt.r.GrandTotal
                            }).ToList(),
                            Header = new Header
                            {
                                Logo = Logo,
                                DateFrom = _dateFrom.ToString("dd-MM-yyyy"),
                                DateTo = _dateTo.ToString("dd-MM-yyyy"),
                                TimeFrom = TimeF,
                                TimeTo = TimeT,
                                Branch = Branch,
                                EmpName = EmpName
                            },
                            Footer = new Footer
                            {
                                CountReceipt = string.Format("{0:#,0}", Summary.FirstOrDefault().CountReceipt),
                                CountReceiptCal = Summary.FirstOrDefault().CountReceipt,
                                //SoldAmount = string.Format("{0:n3}", Summary.FirstOrDefault().SoldAmount),
                                DiscountItem = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                                DiscountItemCal = Summary.FirstOrDefault().DiscountItem,
                                DiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                                DiscountTotalCal = Summary.FirstOrDefault().DiscountTotal,
                                TaxValue = datas.First().curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().TaxValue, sysCur.Amounts),
                                TaxValueCal = Summary.FirstOrDefault().TaxValue,
                                GrandTotal = datas.First().curr.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotal, sysCur.Amounts),
                                GrandTotalCal = Summary.FirstOrDefault().GrandTotal,
                                GrandTotalSys = datas.First().curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotalSys, sysCur.Amounts),
                                GrandTotalSysCal = Summary.FirstOrDefault().GrandTotalSys,
                            }
                        }).ToList();
                if (doctype == "SP") return Sale;
            }
            if (doctype == "IN" || doctype == "All")
            {
                if (plid > 0) saleArFilter = saleArFilter.Where(i => i.PriceListID == plid).ToList();
                var saleARDetail = (from s in saleArFilter
                                    join sd in _context.SaleARDetails on s.SARID equals sd.SARID
                                    select new
                                    {
                                        TotalDisItem = sd.DisValue * s.ExchangeRate
                                    }).ToList();
                double TotalDisItem = saleARDetail.Sum(s => s.TotalDisItem);
                double TotalDisTotal = saleArFilter.Sum(s => s.DisValue);
                double TotalVat = saleArFilter.Sum(s => s.VatValue * s.ExchangeRate);
                double GrandTotalSys = saleArFilter.Sum(s => s.TotalAmountSys);
                double GrandTotal = saleArFilter.Sum(s => s.TotalAmountSys * s.LocalSetRate);
                var currsys = _context.Currency.Find(_user.Company.SystemCurrencyID) ?? new Currency();
                saleAr = (from r in saleArFilter
                          join user in _context.UserAccounts on r.UserID equals user.ID
                          join emp in _context.Employees on user.EmployeeID equals emp.ID
                          join curr in _context.Currency on r.LocalCurID equals curr.ID
                          join curr_pl in _context.Currency on r.SaleCurrencyID equals curr_pl.ID
                          group new { r, emp, curr_pl, curr } by new { emp.ID } into datas
                          let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == datas.FirstOrDefault().curr_pl.ID) ?? new Display()
                          select new GroupSummarySale
                          {
                              EmpCode = datas.First().emp.Code,
                              EmpName = datas.First().emp.Name,
                              SubTotal = datas.Sum(s => s.r.TotalAmount),
                              LCC = datas.First().curr.Description,
                              SC = currsys.Description,
                              Receipts = datas.Select(receipt => new Receipts
                              {
                                  User = receipt.emp.Name,
                                  Receipt = receipt.r.InvoiceNumber,
                                  DateIn = receipt.r.PostingDate.ToString("dd-MM-yyyy"),
                                  TimeIn = "",
                                  DateOut = receipt.r.PostingDate.ToString("dd-MM-yyyy"),
                                  TimeOut = "",
                                  DisInv = receipt.r.DisValue,
                                  Currency = receipt.curr_pl.Description,
                                  GrandTotal = receipt.r.TotalAmount
                              }).ToList(),
                              Header = new Header
                              {
                                  Logo = Logo,
                                  DateFrom = _dateFrom.ToString("dd-MM-yyyy"),
                                  DateTo = _dateTo.ToString("dd-MM-yyyy"),
                                  TimeFrom = TimeF,
                                  TimeTo = TimeT,
                                  Branch = Branch,
                                  EmpName = EmpName
                              },
                              Footer = new Footer
                              {
                                  CountReceipt = string.Format("{0:#,0}", saleArFilter.Count),
                                  CountReceiptCal = saleArFilter.Count,
                                  DiscountItem = _fncModule.ToCurrency(TotalDisItem, sysCur.Amounts),
                                  DiscountItemCal = TotalDisItem,
                                  DiscountTotal = _fncModule.ToCurrency(TotalDisTotal, sysCur.Amounts),
                                  DiscountTotalCal = TotalDisTotal,
                                  TaxValue = currsys.Description + " " + _fncModule.ToCurrency(TotalVat, sysCur.Amounts),
                                  TaxValueCal = TotalVat,
                                  GrandTotal = datas.First().curr.Description + " " + _fncModule.ToCurrency(GrandTotal, sysCur.Amounts),
                                  GrandTotalCal = GrandTotal,
                                  GrandTotalSys = currsys.Description + " " + _fncModule.ToCurrency(GrandTotalSys, sysCur.Amounts),
                                  GrandTotalSysCal = GrandTotalSys
                              }
                          }).ToList();
                if (doctype == "IN") return saleAr;
            }
            List<GroupSummarySale> allSales = new(Sale.Count + saleAr.Count);
            allSales.AddRange(Sale);
            allSales.AddRange(saleAr);
            var sum = (from r in allSales
                       group r by r.EmpCode into g
                       select new GroupSummarySale
                       {
                           Footer = new Footer
                           {
                               CountReceipt = string.Format("{0:#,0}", g.Sum(i => i.Footer.CountReceiptCal)),
                               DiscountItem = _fncModule.ToCurrency(g.Sum(i => i.Footer.DiscountItemCal), sysCur.Amounts),
                               DiscountTotal = _fncModule.ToCurrency(g.Sum(i => i.Footer.DiscountTotalCal), sysCur.Amounts),
                               TaxValue = g.FirstOrDefault().SC + " " + _fncModule.ToCurrency(g.Sum(i => i.Footer.TaxValueCal), sysCur.Amounts),
                               GrandTotal = g.FirstOrDefault().LCC + " " + _fncModule.ToCurrency(g.Sum(i => i.Footer.GrandTotalCal), lcCur.Amounts),
                               GrandTotalSys = g.FirstOrDefault().SC + " " + _fncModule.ToCurrency(g.Sum(i => i.Footer.GrandTotalSysCal), sysCur.Amounts),
                           }
                       }).FirstOrDefault();
            var data = (from r in allSales
                        select new GroupSummarySale
                        {
                            EmpCode = r.EmpCode,
                            EmpName = r.EmpName,
                            SubTotal = r.SubTotal,
                            LCC = r.LCC,
                            SC = r.SC,
                            Receipts = r.Receipts.Select(rp => new Receipts
                            {
                                User = rp.User,
                                Receipt = rp.Receipt,
                                DateIn = rp.DateIn,
                                TimeIn = rp.TimeIn,
                                DateOut = rp.DateOut,
                                TimeOut = rp.TimeOut,
                                DisInv = rp.DisInv,
                                Currency = rp.Currency,
                                GrandTotal = rp.GrandTotal
                            }).ToList(),
                            Header = r.Header,
                            Footer = new Footer
                            {
                                CountReceipt = string.Format("{0:#,0}", sum.Footer.CountReceipt),
                                DiscountItem = _fncModule.ToCurrency(Convert.ToDecimal(sum.Footer.DiscountItem), sysCur.Amounts),
                                DiscountTotal = _fncModule.ToCurrency(Convert.ToDecimal(sum.Footer.DiscountTotal), sysCur.Amounts),
                                TaxValue = r.SC + " " + _fncModule.ToCurrency(Convert.ToDecimal(sum.Footer.TaxValue), sysCur.Amounts),
                                GrandTotal = r.LCC + " " + _fncModule.ToCurrency(Convert.ToDecimal(sum.Footer.GrandTotal), lcCur.Amounts),
                                GrandTotalSys = r.SC + " " + _fncModule.ToCurrency(Convert.ToDecimal(sum.Footer.GrandTotalSys), sysCur.Amounts),
                            }
                        }).ToList();
            return data;
        }
        public List<SummaryTotalSale> GetSummaryTotals(string dateFrom, string dateTo, int branchID, int userID, string timeFrom, string timeTo)
        {
            try
            {
                var data = _context.SummaryTotalSale.FromSql("rp_GetSummarrySaleTotal @DateFrom={0},@DateTo={1}, @BranchID={2},@UserID={3},@TimeFrom={4},@TimeTo={5}",
                parameters: new[] {
                    dateFrom.ToString(),
                    dateTo.ToString(),
                    branchID.ToString(),
                    userID.ToString(),
                    timeFrom.ToString(),
                    timeTo.ToString(),
                }).ToList();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<SummaryTotalSale> GetSummaryTotals(string dateFrom, string dateTo, int branchID, int userID, string timeFrom, string timeTo, int plid)
        {
            try
            {
                var data = _context.SummaryTotalSale.FromSql("rp_GetSummarrySaleTotalpric @DateFrom={0},@DateTo={1}, @BranchID={2},@UserID={3},@TimeFrom={4},@TimeTo={5},@plid={6}",
                parameters: new[] {
                    dateFrom.ToString(),
                    dateTo.ToString(),
                    branchID.ToString(),
                    userID.ToString(),
                    timeFrom.ToString(),
                    timeTo.ToString(),
                    plid.ToString()
                }).ToList();
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public List<PrintSaleHistory> GetSaleARData(int id)
        {
            var list = (from SAR in _context.SaleARs.Where(m => m.SARID == id)
                        join SARD in _context.SaleARDetails on SAR.SARID equals SARD.SARID
                        join BP in _context.BusinessPartners on SAR.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SARD.ItemID equals I.ID
                        join CUR in _context.Currency on SAR.SaleCurrencyID equals CUR.ID
                        join Lcur in _context.Currency on SAR.LocalCurID equals Lcur.ID
                        join B in _context.Branches on SAR.BranchID equals B.ID
                        join U in _context.UserAccounts on SAR.BranchID equals U.BranchID
                        join RI in _context.ReceiptInformation on B.ID equals RI.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SAR.SeriesID equals S.ID
                        join Ex in _context.ExchangeRates on SAR.ExchangeRate equals Ex.ID
                        group new { SAR, SARD, BP, I, CUR, B, U, RI, C, S, Lcur, Ex } by SARD.SARDID into g
                        let data = g.FirstOrDefault()
                        let master = data.SAR
                        let detail = data.SARD
                        let cu = data.CUR
                        let I = data.I
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let RI = data.RI
                        let C = data.C
                        let S = data.S
                        let Ex = data.Ex
                        let Lcur = data.Lcur
                        let SAR = data.SAR
                        let SARD = data.SARD
                        let dlar = _context.SaleDeliveries.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Delivery && i.SDID == data.SAR.BaseOnID) ?? new SaleDelivery()
                        let orderar = _context.SaleOrders.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Order && i.SOID == data.SAR.BaseOnID) ?? new SaleOrder()
                        let quotear = _context.SaleQuotes.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Quotation && i.SQID == data.SAR.BaseOnID) ?? new SaleQuote()
                        let dlOrder = data.SAR.CopyType == SaleCopyType.Delivery ? _context.SaleOrders.FirstOrDefault(i => dlar.CopyType == SaleCopyType.Order && i.SOID == dlar.BaseOnID) ?? new SaleOrder() : new SaleOrder()
                        let dlQuote = data.SAR.CopyType == SaleCopyType.Delivery ? _context.SaleQuotes.FirstOrDefault(i => dlOrder.CopyType == SaleCopyType.Quotation && i.SQID == dlOrder.BaseOnID) ?? new SaleQuote() : new SaleQuote()
                        let orderQuote = data.SAR.CopyType == SaleCopyType.Order ? _context.SaleQuotes.FirstOrDefault(i => orderar.CopyType == SaleCopyType.Quotation && i.SQID == orderar.BaseOnID) ?? new SaleQuote() : new SaleQuote()
                        let totalQtyByCost = _context.SaleARDetails.Where(x => x.SARID == master.SARID && x.ItemID == detail.ItemID)
                           .Sum(x => x.Qty)
                        let paymterm = _context.PaymentTerms.Where(s => s.ID == BP.PaymentTermsID).FirstOrDefault() ?? new PaymentTerms()
                        let requestedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.RequestedBy) ?? new Employee()
                        let receivedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.ReceivedBy) ?? new Employee()
                        let shippedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.ShippedBy) ?? new Employee()
                        let empName = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.SaleEmID) ?? new Employee()
                        let ex = _context.ExchangeRates.FirstOrDefault(s => s.Rate == data.SAR.ExchangeRate) ?? new ExchangeRate()
                        let incomtotal = _context.IncomingPaymentCustomers.Where(s => s.Status == "open" && s.CustomerID == SAR.CusID).Sum(x => x.BalanceDue)
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SAR.SARID,
                            RequestedBy = requestedBy.Name,
                            ReceivedBy = receivedBy.Name,
                            ShippedBy = shippedBy.Name,
                            EmpName = empName.Name,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            Title = RI.Title,
                            Name2 = BP.Name2,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            KhmerDesc = RI.KhmerDescription,
                            EnglishDesc = RI.EnglishDescription,
                            Addresskh = RI.Address,
                            AddressEng = RI.EnglishDescription,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            DSNumber = dlar.InvoiceNumber,
                            OrderNumber = data.SAR.CopyType == SaleCopyType.Delivery ? dlOrder.InvoiceNumber : orderar.InvoiceNumber,
                            QSNumber = data.SAR.CopyType == SaleCopyType.Delivery ? dlQuote.InvoiceNumber : data.SAR.CopyType == SaleCopyType.Order ? orderQuote.InvoiceNumber : quotear.InvoiceNumber,

                            //Detail
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            DiscountRate_Detail = detail.DisRate,
                            UomName = detail.UomName,
                            Amount = detail.TotalWTax,
                            LocalCurrency = Lcur.Description,
                            SysCurrency = cu.Description,
                            CompanyName = B.Name,
                            Logo = C.Logo,
                            Tel1 = RI.Tel1,
                            Tel2 = RI.Tel2,
                            Exange = Ex.SetRate,
                            //Brand = cp == null ? "" : cp.Name,
                            //Summary
                            Sub_Total = (double)master.SubTotalBefDis,
                            DiscountValue = master.DisValue,
                            DiscountRate = master.DisRate,
                            TaxValue = (decimal)master.VatValue,
                            TaxRate = (decimal)master.VatRate,
                            Applied_Amount = master.AppliedAmount,
                            TotalAmount = master.TotalAmount,
                            TotalAmountSys = cu.Description == "KHR" ? master.TotalAmountSys : master.TotalAmountSys * master.LocalSetRate,
                            Barcode = I.Barcode,
                            ExchangeRate = master.ExchangeRate,
                            LocalSetRate = SAR.LocalSetRate,
                            PriceList = cu.Description,
                            LabelUSA = cu.Description == "KHR" ? "KHR" : cu.Description == "USD" ? "$" : cu.Description,
                            LabelReal = Lcur.Description == "KHR" ? " KHR" : Lcur.Description == "USD" ? "$" : Lcur.Description,
                            Sub_totalAfterdis = master.SubTotalAfterDis,
                            Balance_Due_Local = 0,
                            Balance_Due_Sys = 0,
                            BaseOn = master.BasedCopyKeys,
                            BPBrandName = "",
                            Debit = master.TotalAmount - master.AppliedAmount,
                            DebitSys = cu.Description == "KHR" ? master.TotalAmount - master.AppliedAmount : (master.TotalAmount - master.AppliedAmount) * master.LocalSetRate,
                            //Paymenterm = paymterm.Days,
                            VatNumber = BP.VatNumber,
                            Email = BP.Email,
                            ShipTo = SAR.ShipTo,
                            DownPayment = SAR.DownPayment,
                            TotalBancedue = incomtotal
                        }).ToList();
            return list;
        }
        public List<PrintSaleHistory> GetSaleARReserveEdit(int id)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from SAR in _context.ARReserveInvoiceEditables.Where(m => m.ID == id)
                        join SARD in _context.ARReserveInvoiceEditableDetails on SAR.ID equals SARD.ARReserveInvoiceEditableID
                        join BP in _context.BusinessPartners on SAR.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SARD.ItemID equals I.ID
                        join CUR in _context.Currency on SAR.SaleCurrencyID equals CUR.ID
                        join Lcur in _context.Currency on SAR.LocalCurID equals Lcur.ID
                        join B in _context.Branches on SAR.BranchID equals B.ID
                        join U in _context.UserAccounts on SAR.BranchID equals U.BranchID
                        join RI in _context.ReceiptInformation on B.ID equals RI.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SAR.SeriesID equals S.ID
                        group new { SAR, SARD, BP, I, CUR, B, U, RI, C, S, Lcur } by SARD.ID into g
                        let data = g.FirstOrDefault()
                        let master = data.SAR
                        let detail = data.SARD
                        let cu = data.CUR
                        let I = data.I
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let RI = data.RI
                        let C = data.C
                        let S = data.S
                        let Lcur = data.Lcur
                        let SAR = data.SAR
                        let SARD = data.SARD
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        let dlar = _context.SaleDeliveries.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Delivery && i.SDID == data.SAR.BaseOnID) ?? new SaleDelivery()
                        let orderar = _context.SaleOrders.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Order && i.SOID == data.SAR.BaseOnID) ?? new SaleOrder()
                        let quotear = _context.SaleQuotes.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Quotation && i.SQID == data.SAR.BaseOnID) ?? new SaleQuote()
                        let dlOrder = data.SAR.CopyType == SaleCopyType.Delivery ? _context.SaleOrders.FirstOrDefault(i => dlar.CopyType == SaleCopyType.Order && i.SOID == dlar.BaseOnID) ?? new SaleOrder() : new SaleOrder()
                        let dlQuote = data.SAR.CopyType == SaleCopyType.Delivery ? _context.SaleQuotes.FirstOrDefault(i => dlOrder.CopyType == SaleCopyType.Quotation && i.SQID == dlOrder.BaseOnID) ?? new SaleQuote() : new SaleQuote()
                        let orderQuote = data.SAR.CopyType == SaleCopyType.Order ? _context.SaleQuotes.FirstOrDefault(i => orderar.CopyType == SaleCopyType.Quotation && i.SQID == orderar.BaseOnID) ?? new SaleQuote() : new SaleQuote()
                        let paymterm = _context.PaymentTerms.Where(s => s.ID == BP.PaymentTermsID).FirstOrDefault() ?? new PaymentTerms()
                        let requestedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.RequestedBy) ?? new Employee()
                        let receivedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.ReceivedBy) ?? new Employee()
                        let shippedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.ShippedBy) ?? new Employee()
                        let empName = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.SaleEmID) ?? new Employee()
                        let ex = _context.ExchangeRates.FirstOrDefault(s => s.Rate == data.SAR.ExchangeRate) ?? new ExchangeRate()
                        let incomtotal = _context.IncomingPaymentCustomers.Where(s => s.Status == "open" && s.CustomerID == SAR.CusID).Sum(x => x.BalanceDue)
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SAR.ID,
                            RequestedBy = requestedBy.Name,
                            ReceivedBy = receivedBy.Name,
                            ShippedBy = shippedBy.Name,
                            EmpName = empName.Name,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            Title = RI.Title,
                            Name2 = BP.Name2,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            KhmerDesc = RI.KhmerDescription,
                            EnglishDesc = RI.EnglishDescription,
                            Addresskh = RI.Address,
                            AddressEng = RI.EnglishDescription,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            DSNumber = dlar.InvoiceNumber,
                            OrderNumber = data.SAR.CopyType == SaleCopyType.Delivery ? dlOrder.InvoiceNumber : orderar.InvoiceNumber,
                            QSNumber = data.SAR.CopyType == SaleCopyType.Delivery ? dlQuote.InvoiceNumber : data.SAR.CopyType == SaleCopyType.Order ? orderQuote.InvoiceNumber : quotear.InvoiceNumber,

                            //Detail
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            DiscountRate_Detail = detail.DisRate,
                            UomName = detail.UomName,
                            Amount = detail.TotalWTax,
                            LocalCurrency = Lcur.Description,
                            SysCurrency = cu.Description,
                            CompanyName = B.Name,
                            Logo = data.C.Logo,
                            Tel1 = RI.Tel1,
                            Tel2 = RI.Tel2,
                            Brand = cp == null ? "" : cp.Name,
                            //Summary
                            Sub_Total = (double)master.SubTotalBefDis,
                            DiscountValue = master.DisValue,
                            DiscountRate = master.DisRate,
                            TaxValue = (decimal)master.VatValue,
                            TaxRate = (decimal)master.VatRate,
                            Applied_Amount = master.AppliedAmount,
                            TotalAmount = master.TotalAmount,
                            TotalAmountSys = cu.Description == "KHR" ? master.TotalAmountSys : master.TotalAmountSys * master.LocalSetRate,
                            Barcode = I.Barcode,
                            ExchangeRate = master.ExchangeRate,
                            LocalSetRate = SAR.LocalSetRate,
                            PriceList = cu.Description,
                            LabelUSA = cu.Description == "KHR" ? "?" : cu.Description == "USD" ? "$" : cu.Description,
                            LabelReal = Lcur.Description == "KHR" ? " ?" : Lcur.Description == "USD" ? "$" : Lcur.Description,
                            Sub_totalAfterdis = master.SubTotalAfterDis,
                            Balance_Due_Local = 0,
                            Balance_Due_Sys = 0,
                            BaseOn = master.BasedCopyKeys,
                            BPBrandName = "",
                            Debit = master.TotalAmount - master.AppliedAmount,
                            DebitSys = cu.Description == "KHR" ? master.TotalAmount - master.AppliedAmount : (master.TotalAmount - master.AppliedAmount) * master.LocalSetRate,
                            //Paymenterm = paymterm.Days,
                            VatNumber = BP.VatNumber,
                            Email = BP.Email,
                            DownPayment = SAR.DownPayment,
                            TotalBancedue = incomtotal
                        }).ToList();
            return list;
        }
        //==========================print preview pos=============
        public List<PrintBill> GetPOSData(int id)
        {
            var receipts = (from res in _context.Receipt.Where(s => s.ReceiptID == id)
                            join CUR in _context.Currency on res.SysCurrencyID equals CUR.ID
                            join Lcur in _context.Currency on res.PLCurrencyID equals Lcur.ID

                            join company in _context.Company on res.CompanyID equals company.ID
                            join bp in _context.BusinessPartners on res.CustomerID equals bp.ID
                            join brand in _context.Branches on res.BranchID equals brand.ID
                            join user in _context.UserAccounts on brand.ID equals user.BranchID
                            join ri in _context.ReceiptInformation on brand.ID equals ri.ID
                            group new { res, CUR, Lcur, company, bp, brand, user, ri } by res.ReceiptID into g
                            let data = g.FirstOrDefault()
                            let res = data.res
                            let mulipay = _context.MultiPaymentMeans.Where(s => s.ReceiptID == data.res.ReceiptID).ToList()

                            let company = data.company
                            let bp = data.bp
                            let brand = data.brand
                            let user = data.user
                            let ri = data.ri
                            let cu = data.CUR
                            let Lcur = data.Lcur
                            select new PrintBill
                            {
                                // master
                                OrderID = res.ReceiptID,
                                ReceiptTitle1 = ri.Title == null ? " " : ri.Title,
                                ReceiptTitle2 = ri.Title2 == null ? " " : ri.Title2,
                                Logo = company.Logo == null ? "" : company.Logo,
                                Logo2 = company.Logo2 == null ? "" : company.Logo2,
                                BranchName = brand.Name == null ? "" : brand.Name,
                                BrandKh = brand.Name2 == null ? "" : brand.Name2,
                                Address = ri.Address == null ? " " : ri.Address,
                                ReceiptAddress2 = ri.Address2 == null ? " " : ri.Address2,
                                CustomerInfo = bp.Name == "" ? bp.Name2 : bp.Name,
                                CustomerPhone = bp.Phone,
                                Tel1 = ri.Tel1 == null ? " " : ri.Tel1,
                                Tel2 = ri.Tel2 == null ? " " : ri.Tel2,
                                ReceiptEmail = ri.Email == null ? " " : ri.Email,
                                RecVATTIN = ri.Email == null ? " " : ri.Email,
                                RecWebsite = ri.Website == null ? " " : ri.Website,
                                PowerBy = ri.PowerBy == null ? " " : ri.PowerBy,
                                ReceiptNo = res.ReceiptNo,
                                Cashier = user.Username == null ? " " : user.Username,
                                DateTimeIn = res.DateIn.ToString("dd-MM-yyyy") + " " + res.TimeIn,
                                DateTimeOut = res.DateOut.ToString("dd-MM-yyyy") + " " + res.TimeOut,
                                Point = res.CumulativePoint.ToString(),
                                Team = ri.TeamCondition == null ? " " : ri.TeamCondition,
                                Team2 = ri.TeamCondition2 == null ? " " : ri.TeamCondition2,
                                ExchangeRate = res.ExchangeRate.ToString(),
                                PaymentMean = (from pay in _context.PaymentMeans.Where(s => mulipay.Any(x => x.PaymentMeanID == s.ID))

                                               select new PaymentMeans
                                               {
                                                   ID = pay.ID,
                                                   Type = pay.Type,

                                               }).ToList(),
                                LocalCurRate = res.LocalSetRate,
                                LabelUSA = cu.Description == "USD" ? "$" : cu.Description == "KHR" ? "?" : cu.Description,
                                LabelReal = Lcur.Description,
                                // detail
                                PrintviewDetail = (from rd in _context.ReceiptDetail.Where(s => s.ReceiptID == res.ReceiptID)
                                                   join item in _context.ItemMasterDatas on rd.ItemID equals item.ID
                                                   join uom in _context.GroupUOMs on rd.UomID equals uom.ID
                                                   select new PrintviewDetail
                                                   {
                                                       ID = rd.ID,
                                                       Description = item.KhmerName == "" ? item.EnglishName : item.KhmerName,
                                                       Photo = item.Image,
                                                       Currency = rd.Currency == "USD" ? "$" : rd.Currency == "KHR" ? "?" : rd.Currency,
                                                       Qty = rd.Qty,
                                                       UnitPice = rd.UnitPrice,
                                                       Amount = rd.Total,

                                                   }).ToList(),


                                // Summary
                                SubTotal = res.Sub_Total.ToString(),
                                DisRate = res.DiscountRate.ToString(),
                                DisValue = res.DiscountValue.ToString(),
                                TypeDis = res.TypeDis,
                                GrandTotal = res.GrandTotal.ToString(),
                                GrandTotalSys = res.GrandTotal_Sys.ToString(),
                                VatRate = res.TaxRate.ToString(),
                                VatValue = res.TaxValue.ToString(),
                                TaxTotal = res.TaxValue.ToString(),
                                AmountFrieght = (double)res.AmountFreight,
                                // receive
                                Received = res.Received.ToString(),
                                Change = res.Change.ToString(),
                                ChangeSys = res.Change_Display.ToString(),
                                // footer
                                DescKh = ri.KhmerDescription,
                                DescEn = ri.EnglishDescription,
                            }).ToList() ?? new List<PrintBill>();

            return receipts;
        }
        public async Task<List<PrintBill>> GetRePrintPOSData(int id, int userid)
        {
            var list = await _pos.PrintReceiptReprintAsync(id, "Pay", userid, true);
            return list;
        }
        public List<PrintSaleHistory> GetSaleARDataEdit(int id)
        {
            var list = (from SAR in _context.SaleAREdites.Where(m => m.SARID == id)
                        join SARD in _context.SaleAREditeDetails on SAR.SARID equals SARD.SARID
                        join BP in _context.BusinessPartners on SAR.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SARD.ItemID equals I.ID
                        join CUR in _context.Currency on SAR.SaleCurrencyID equals CUR.ID
                        join Lcur in _context.Currency on SAR.LocalCurID equals Lcur.ID
                        join B in _context.Branches on SAR.BranchID equals B.ID
                        join U in _context.UserAccounts on SAR.BranchID equals U.BranchID
                        join RI in _context.ReceiptInformation on B.ID equals RI.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SAR.SeriesID equals S.ID

                        group new { SAR, SARD, BP, I, CUR, B, U, RI, C, S, Lcur } by SARD.SARDID into g
                        let data = g.FirstOrDefault()
                        let master = data.SAR
                        let detail = data.SARD
                        let cu = data.CUR
                        let I = data.I
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let RI = data.RI
                        let C = data.C
                        let S = data.S
                        let Lcur = data.Lcur
                        let SAR = data.SAR
                        let SARD = data.SARD
                        let dlar = _context.SaleDeliveries.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Delivery && i.SDID == data.SAR.BaseOnID) ?? new SaleDelivery()
                        let orderar = _context.SaleOrders.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Order && i.SOID == data.SAR.BaseOnID) ?? new SaleOrder()
                        let quotear = _context.SaleQuotes.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Quotation && i.SQID == data.SAR.BaseOnID) ?? new SaleQuote()
                        let dlOrder = data.SAR.CopyType == SaleCopyType.Delivery ? _context.SaleOrders.FirstOrDefault(i => dlar.CopyType == SaleCopyType.Order && i.SOID == dlar.BaseOnID) ?? new SaleOrder() : new SaleOrder()
                        let dlQuote = data.SAR.CopyType == SaleCopyType.Delivery ? _context.SaleQuotes.FirstOrDefault(i => dlOrder.CopyType == SaleCopyType.Quotation && i.SQID == dlOrder.BaseOnID) ?? new SaleQuote() : new SaleQuote()
                        let orderQuote = data.SAR.CopyType == SaleCopyType.Order ? _context.SaleQuotes.FirstOrDefault(i => orderar.CopyType == SaleCopyType.Quotation && i.SQID == orderar.BaseOnID) ?? new SaleQuote() : new SaleQuote()
                        let paymterm = _context.PaymentTerms.Where(s => s.ID == BP.PaymentTermsID).FirstOrDefault() ?? new PaymentTerms()
                        let requestedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.RequestedBy) ?? new Employee()
                        let receivedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.ReceivedBy) ?? new Employee()
                        let shippedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.ShippedBy) ?? new Employee()
                        let empName = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.SaleEmID) ?? new Employee()
                        let ex = _context.ExchangeRates.FirstOrDefault(s => s.Rate == data.SAR.ExchangeRate) ?? new ExchangeRate()
                        let incomtotal = _context.IncomingPaymentCustomers.Where(s => s.Status == "open" && s.CustomerID == SAR.CusID).Sum(x => x.BalanceDue)
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SAR.SARID,
                            RequestedBy = requestedBy.Name,
                            ReceivedBy = receivedBy.Name,
                            ShippedBy = shippedBy.Name,
                            EmpName = empName.Name,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            Title = RI.Title,
                            Name2 = BP.Name2,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            KhmerDesc = RI.KhmerDescription,
                            EnglishDesc = RI.EnglishDescription,
                            Addresskh = RI.Address,
                            AddressEng = RI.EnglishDescription,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            DSNumber = dlar.InvoiceNumber,
                            OrderNumber = data.SAR.CopyType == SaleCopyType.Delivery ? dlOrder.InvoiceNumber : orderar.InvoiceNumber,
                            QSNumber = data.SAR.CopyType == SaleCopyType.Delivery ? dlQuote.InvoiceNumber : data.SAR.CopyType == SaleCopyType.Order ? orderQuote.InvoiceNumber : quotear.InvoiceNumber,

                            //Detail
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            DiscountRate_Detail = detail.DisRate,
                            UomName = detail.UomName,
                            Amount = detail.TotalWTax,
                            LocalCurrency = Lcur.Description,
                            SysCurrency = cu.Description,
                            CompanyName = B.Name,
                            Logo = data.C.Logo,
                            Tel1 = RI.Tel1,
                            Tel2 = RI.Tel2,
                            //Brand = cp == null ? "" : cp.Name,
                            //Summary
                            Sub_Total = (double)master.SubTotalBefDis,
                            DiscountValue = master.DisValue,
                            DiscountRate = master.DisRate,
                            TaxValue = (decimal)master.VatValue,
                            TaxRate = (decimal)master.VatRate,
                            Applied_Amount = master.AppliedAmount,
                            TotalAmount = master.TotalAmount,
                            TotalAmountSys = cu.Description == "KHR" ? master.TotalAmountSys : master.TotalAmountSys * master.LocalSetRate,
                            Barcode = I.Barcode,
                            ExchangeRate = master.ExchangeRate,
                            LocalSetRate = SAR.LocalSetRate,
                            PriceList = cu.Description,
                            LabelUSA = cu.Description == "KHR" ? "KHR" : cu.Description == "USD" ? "$" : cu.Description,
                            LabelReal = Lcur.Description == "KHR" ? " KHR" : Lcur.Description == "USD" ? "$" : Lcur.Description,
                            Sub_totalAfterdis = master.SubTotalAfterDis,
                            Balance_Due_Local = 0,
                            Balance_Due_Sys = 0,
                            BaseOn = master.BasedCopyKeys,
                            BPBrandName = "",
                            Debit = master.TotalAmount - master.AppliedAmount,
                            DebitSys = cu.Description == "KHR" ? master.TotalAmount - master.AppliedAmount : (master.TotalAmount - master.AppliedAmount) * master.LocalSetRate,
                            //Paymenterm = paymterm.Days,
                            VatNumber = BP.VatNumber,
                            Email = BP.Email,
                            ShipTo = SAR.ShipTo,
                            DownPayment = SAR.DownPayment,
                            TotalBancedue = incomtotal
                        }).ToList();
            return list;
        }

        public List<PrintPurchaseAP> GetPrintPurchaseAP(int purcahseId)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from PA in _context.Purchase_APs.Where(m => m.PurchaseAPID == purcahseId)
                        join PAD in _context.PurchaseAPDetail on PA.PurchaseAPID equals PAD.PurchaseAPID
                        join BP in _context.BusinessPartners on PA.VendorID equals BP.ID
                        join U in _context.UserAccounts on PA.UserID equals U.ID
                        join B in _context.Branches on PA.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PA.PurCurrencyID equals C.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on PAD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PAD.UomID equals N.ID
                        join S in _context.Series on PA.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            CusCode = BP.Code,
                            Invoice = PA.InvoiceNo,
                            ExchangeRate = PA.PurRate,
                            Balance_Due_Sys = PA.BalanceDueSys,
                            Applied_Amount = PA.AppliedAmount,
                            PostingDate = PA.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PA.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PA..ToString("dd-MM-yyyy"),
                            Sub_Total = PA.BalanceDue,
                            DiscountValue = PA.DiscountValue,
                            DiscountRate = PA.DiscountRate,
                            TaxValue = PA.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PA.TaxRate,
                            TypeDis = PA.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PA.ReffNo,
                            VendorNo = PA.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PAD.Total,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = PAD.PurchasPrice,
                            UomName = N.Name,
                            Qty = PAD.Qty,
                            Remark = PA.Remark
                        }).ToList();
            return list;
            //if (!list.Any()) return NotFound();
            //return new ViewAsPdf(list)
            //{
            //    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-font-size \"9\""
            //};
        }

        public List<PrintPurchaseAP> GetPrintPurchaseQuotation(int purchaseId)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from PA in _context.PurchaseQuotations.Where(m => m.ID == purchaseId)
                        join PAD in _context.PurchaseQuotationDetails on PA.ID equals PAD.PurchaseQuotationID
                        join BP in _context.BusinessPartners on PA.VendorID equals BP.ID
                        join U in _context.UserAccounts on PA.UserID equals U.ID
                        join B in _context.Branches on PA.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PA.PurCurrencyID equals C.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on PAD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PAD.UomID equals N.ID
                        join S in _context.Series on PA.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            CusCode = BP.Code,
                            Invoice = PA.InvoiceNo,
                            ExchangeRate = PA.PurRate,
                            Balance_Due_Sys = PA.BalanceDueSys,
                            Applied_Amount = PA.AppliedAmount,
                            PostingDate = PA.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PA.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PA..ToString("dd-MM-yyyy"),
                            Sub_Total = PA.BalanceDue,
                            DiscountValue = PA.DiscountValue,
                            DiscountRate = PA.DiscountRate,
                            TaxValue = PA.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PA.TaxRate,
                            TypeDis = PA.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PA.ReffNo,
                            VendorNo = PA.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PAD.Total,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = PAD.PurchasPrice,
                            UomName = N.Name,
                            Qty = PAD.Qty,
                            Remark = PA.Remark
                        }).ToList();
            return list;
        }
        public List<PrintPurchaseAP> GetPrintPurchaseAPResers(int purcahseId)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from PA in _context.PurchaseAPReserves.Where(m => m.ID == purcahseId)
                        join PAD in _context.PurchaseAPReserveDetails on PA.ID equals PAD.PurchaseAPReserveID
                        join BP in _context.BusinessPartners on PA.VendorID equals BP.ID
                        join U in _context.UserAccounts on PA.UserID equals U.ID
                        join B in _context.Branches on PA.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PA.PurCurrencyID equals C.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on PAD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PAD.UomID equals N.ID
                        join S in _context.Series on PA.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            CusCode = BP.Code,
                            Invoice = PA.InvoiceNo,
                            ExchangeRate = PA.PurRate,
                            Balance_Due_Sys = PA.BalanceDueSys,
                            Applied_Amount = PA.AppliedAmount,
                            PostingDate = PA.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PA.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PA..ToString("dd-MM-yyyy"),
                            Sub_Total = PA.BalanceDue,
                            DiscountValue = PA.DiscountValue,
                            DiscountRate = PA.DiscountRate,
                            TaxValue = PA.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PA.TaxRate,
                            TypeDis = PA.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PA.ReffNo,
                            VendorNo = PA.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PAD.Total,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = PAD.PurchasPrice,
                            UomName = N.Name,
                            Qty = PAD.Qty,
                            Remark = PA.Remark
                        }).ToList();
            return list;
        }

        public List<PrintPurchaseAP> GetPurchaseQuotation(int purchaseId)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from PA in _context.PurchaseQuotations.Where(m => m.ID == purchaseId)
                        join PAD in _context.PurchaseQuotationDetails on PA.ID equals PAD.PurchaseQuotationID
                        join BP in _context.BusinessPartners on PA.VendorID equals BP.ID
                        join U in _context.UserAccounts on PA.UserID equals U.ID
                        join B in _context.Branches on PA.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PA.PurCurrencyID equals C.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on PAD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PAD.UomID equals N.ID
                        join S in _context.Series on PA.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            CusCode = BP.Code,
                            Invoice = PA.InvoiceNo,
                            ExchangeRate = PA.PurRate,
                            Balance_Due_Sys = PA.BalanceDueSys,
                            Applied_Amount = PA.AppliedAmount,
                            PostingDate = PA.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PA.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PA..ToString("dd-MM-yyyy"),
                            Sub_Total = PA.BalanceDue,
                            DiscountValue = PA.DiscountValue,
                            DiscountRate = PA.DiscountRate,
                            TaxValue = PA.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PA.TaxRate,
                            TypeDis = PA.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PA.ReffNo,
                            VendorNo = PA.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PAD.Total,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = PAD.PurchasPrice,
                            UomName = N.Name,
                            Qty = PAD.Qty,
                            Remark = PA.Remark
                        }).ToList();
            return list;
        }

        public List<PrintPurchaseAP> GetPrintPurchaseRequest(int purchaseId)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from PA in _context.PurchaseRequests.Where(m => m.ID == purchaseId)
                        join PAD in _context.PurchaseRequestDetails on PA.ID equals PAD.PurchaseRequestID
                        join BP in _context.BusinessPartners on PA.RequesterID equals BP.ID
                        join U in _context.UserAccounts on PA.UserID equals U.ID
                        join B in _context.Branches on PA.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PA.PurCurrencyID equals C.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on PAD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PAD.UomID equals N.ID
                        join S in _context.Series on PA.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            CusCode = BP.Code,
                            Invoice = PA.Number,
                            ExchangeRate = PA.PurRate,
                            Balance_Due_Sys = PA.BalanceDueSys,
                            Applied_Amount = PA.AppliedAmount,
                            PostingDate = PA.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PA.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PA..ToString("dd-MM-yyyy"),
                            Sub_Total = PA.BalanceDue,
                            DiscountValue = PA.DiscountValue,
                            DiscountRate = PA.DiscountRate,
                            TaxValue = PA.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PA.TaxRate,
                            //TypeDis = PA.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PA.ReffNo,
                            VendorNo = PA.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PAD.Total,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = PAD.PurchasPrice,
                            UomName = N.Name,
                            Qty = PAD.Qty,
                            Remark = PA.Remark
                        }).ToList();
            return list;
        }
        public List<PrintSaleHistory> GetSaleARReserve(int id)
        {
            var list = (from SAR in _context.ARReserveInvoices.Where(m => m.ID == id)
                        join SARD in _context.ARReserveInvoiceDetails on SAR.ID equals SARD.ARReserveInvoiceID
                        join BP in _context.BusinessPartners on SAR.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SARD.ItemID equals I.ID
                        join CUR in _context.Currency on SAR.SaleCurrencyID equals CUR.ID
                        join Lcur in _context.Currency on SAR.LocalCurID equals Lcur.ID
                        join B in _context.Branches on SAR.BranchID equals B.ID
                        join U in _context.UserAccounts on SAR.BranchID equals U.BranchID
                        join RI in _context.ReceiptInformation on B.ID equals RI.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SAR.SeriesID equals S.ID
                        group new { SAR, SARD, BP, I, CUR, B, U, RI, C, S, Lcur } by SARD.ARReserveInvoiceID into g
                        let data = g.FirstOrDefault()
                        let master = data.SAR
                        let detail = data.SARD
                        let cu = data.CUR
                        let I = data.I
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let RI = data.RI
                        let C = data.C
                        let S = data.S
                        let Lcur = data.Lcur
                        let SAR = data.SAR
                        let SARD = data.SARD
                        let dlar = _context.SaleDeliveries.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Delivery && i.SDID == data.SAR.BaseOnID) ?? new SaleDelivery()
                        let orderar = _context.SaleOrders.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Order && i.SOID == data.SAR.BaseOnID) ?? new SaleOrder()
                        let quotear = _context.SaleQuotes.FirstOrDefault(i => data.SAR.CopyType == SaleCopyType.Quotation && i.SQID == data.SAR.BaseOnID) ?? new SaleQuote()
                        let dlOrder = data.SAR.CopyType == SaleCopyType.Delivery ? _context.SaleOrders.FirstOrDefault(i => dlar.CopyType == SaleCopyType.Order && i.SOID == dlar.BaseOnID) ?? new SaleOrder() : new SaleOrder()
                        let dlQuote = data.SAR.CopyType == SaleCopyType.Delivery ? _context.SaleQuotes.FirstOrDefault(i => dlOrder.CopyType == SaleCopyType.Quotation && i.SQID == dlOrder.BaseOnID) ?? new SaleQuote() : new SaleQuote()
                        let orderQuote = data.SAR.CopyType == SaleCopyType.Order ? _context.SaleQuotes.FirstOrDefault(i => orderar.CopyType == SaleCopyType.Quotation && i.SQID == orderar.BaseOnID) ?? new SaleQuote() : new SaleQuote()
                        let paymterm = _context.PaymentTerms.Where(s => s.ID == BP.PaymentTermsID).FirstOrDefault() ?? new PaymentTerms()
                        let requestedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.RequestedBy) ?? new Employee()
                        let receivedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.ReceivedBy) ?? new Employee()
                        let shippedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.ShippedBy) ?? new Employee()
                        let empName = _context.Employees.FirstOrDefault(i => i.ID == data.SAR.SaleEmID) ?? new Employee()
                        let ex = _context.ExchangeRates.FirstOrDefault(s => s.Rate == data.SAR.ExchangeRate) ?? new ExchangeRate()
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SAR.ID,
                            RequestedBy = requestedBy.Name,
                            ReceivedBy = receivedBy.Name,
                            ShippedBy = shippedBy.Name,
                            EmpName = empName.Name,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            Title = RI.Title,
                            Name2 = BP.Name2,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            KhmerDesc = RI.KhmerDescription,
                            EnglishDesc = RI.EnglishDescription,
                            Addresskh = RI.Address,
                            AddressEng = RI.EnglishDescription,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            DSNumber = dlar.InvoiceNumber,
                            OrderNumber = data.SAR.CopyType == SaleCopyType.Delivery ? dlOrder.InvoiceNumber : orderar.InvoiceNumber,
                            QSNumber = data.SAR.CopyType == SaleCopyType.Delivery ? dlQuote.InvoiceNumber : data.SAR.CopyType == SaleCopyType.Order ? orderQuote.InvoiceNumber : quotear.InvoiceNumber,

                            //Detail
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            DiscountRate_Detail = detail.DisRate,
                            UomName = detail.UomName,
                            Amount = detail.TotalWTax,
                            LocalCurrency = Lcur.Description,
                            SysCurrency = cu.Description,
                            CompanyName = B.Name,
                            Logo = data.C.Logo,
                            Tel1 = RI.Tel1,
                            Tel2 = RI.Tel2,
                            //Brand = cp == null ? "" : cp.Name,
                            //Summary
                            Sub_Total = (double)master.SubTotalAfterDis,
                            DiscountValue = master.DisValue,
                            DiscountRate = master.DisRate,
                            TaxValue = (decimal)master.VatValue,
                            TaxRate = (decimal)master.VatRate,
                            Applied_Amount = master.AppliedAmount,
                            TotalAmount = master.TotalAmount,
                            TotalAmountSys = cu.Description == "KHR" ? master.TotalAmountSys : master.TotalAmountSys * master.LocalSetRate,
                            Barcode = I.Barcode,
                            ExchangeRate = master.ExchangeRate,
                            LocalSetRate = SAR.LocalSetRate,
                            PriceList = cu.Description,
                            LabelUSA = cu.Description == "KHR" ? "?" : cu.Description == "USD" ? "$" : cu.Description,
                            LabelReal = Lcur.Description == "KHR" ? " ?" : Lcur.Description == "USD" ? "$" : Lcur.Description,
                            Sub_totalAfterdis = master.SubTotalAfterDis,
                            Balance_Due_Local = 0,
                            Balance_Due_Sys = 0,
                            BaseOn = master.BasedCopyKeys,
                            BPBrandName = "",
                            Debit = master.TotalAmount - master.AppliedAmount,
                            DebitSys = cu.Description == "KHR" ? master.TotalAmount - master.AppliedAmount : (master.TotalAmount - master.AppliedAmount) * master.LocalSetRate,
                            //Paymenterm = paymterm.Days,
                            VatNumber = BP.VatNumber,
                            Email = BP.Email
                        }).ToList();
            return list;
        }


        public List<PrintSaleHistory> GetSaleQoute(int id)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from SQ in _context.SaleQuotes.Where(m => m.SQID == id)
                        join SQD in _context.SaleQuoteDetails on SQ.SQID equals SQD.SQID
                        join BP in _context.BusinessPartners on SQ.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SQD.ItemID equals I.ID
                        join CUR in _context.Currency on SQ.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SQ.BranchID equals B.ID
                        join U in _context.UserAccounts on SQ.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SQ.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        let RequestedBy = _context.Employees.FirstOrDefault(i => i.ID == SQ.RequestedBy) ?? new Employee()
                        let ReceivedBy = _context.Employees.FirstOrDefault(i => i.ID == SQ.ReceivedBy) ?? new Employee()
                        let ShippedBy = _context.Employees.FirstOrDefault(i => i.ID == SQ.ReceivedBy) ?? new Employee()

                        group new { SQ, SQD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S, ReceivedBy, RequestedBy, ShippedBy } by SQD.SQDID into g
                        let data = g.FirstOrDefault()
                        let master = data.SQ
                        let detail = data.SQD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let cp = data.cp
                        let S = data.S
                        let RequestedBy = data.RequestedBy
                        let ReceivedBy = data.ReceivedBy
                        let ShippedBy = data.ShippedBy
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SQ.SQID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MMa-yyyy"),
                            DueDate = master.ValidUntilDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = data.BP.Phone,
                            Address = data.BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            Email = data.BP.Email,
                            Name2 = data.BP.Name2,
                            Title = R.Title,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            RequestedBy = RequestedBy.Name,
                            ReceivedBy = ReceivedBy.Name,
                            ShippedBy = ShippedBy.Name,
                            //Detail
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountRate = detail.DisRate,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            TotalWTax = detail.TotalWTax,
                            //Amount = detail.Total,
                            Amount = detail.Qty * detail.UnitPrice,
                            Sub_totalAfterdis = (decimal)detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            Addre = R.KhmerDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = master.DisValue,
                            VatValue = master.VatValue,
                            VatNumber = BP.VatNumber.ToString() == "0" ? "" : BP.VatNumber.ToString(),
                            TotalAmount = master.TotalAmount,
                            LabelUSA = cu.Description == "KHR" ? "KHR" : cu.Description == "USD" ? "$" : cu.Description,

                        }).ToList();
            return list;
        }

        public List<PrintSaleHistory> GetSaleOrderHistory(int id)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from SO in _context.SaleOrders.Where(m => m.SOID == id)
                        join SOD in _context.SaleOrderDetails on SO.SOID equals SOD.SOID
                        join BP in _context.BusinessPartners on SO.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SOD.ItemID equals I.ID
                        join CUR in _context.Currency on SO.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SO.BranchID equals B.ID
                        join U in _context.UserAccounts on SO.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SO.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        let RequestedBy = _context.Employees.FirstOrDefault(i => i.ID == SO.RequestedBy) ?? new Employee()
                        let ReceivedBy = _context.Employees.FirstOrDefault(i => i.ID == SO.ReceivedBy) ?? new Employee()
                        let ShippedBy = _context.Employees.FirstOrDefault(i => i.ID == SO.ShippedBy) ?? new Employee()
                        let EmpName = _context.Employees.FirstOrDefault(i => i.ID == SO.SaleEmID) ?? new Employee()
                        group new { SO, SOD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S, RequestedBy, ReceivedBy, ShippedBy, EmpName } by SOD.SODID into g
                        let data = g.FirstOrDefault()
                        let master = data.SO
                        let detail = data.SOD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let cp = data.cp
                        let S = data.S
                        let RequestedBy = data.RequestedBy
                        let ReceivedBy = data.ReceivedBy
                        let ShippedBy = data.ShippedBy
                        let EmpName = data.EmpName
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SO.SOID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DeliveryDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            Title = R.Title,
                            Name2 = BP.Name2,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            Remarks = master.Remarks,
                            RequestedBy = RequestedBy.Name,
                            ReceivedBy = ReceivedBy.Name,
                            ShippedBy = ShippedBy.Name,
                            EmpName = EmpName.Name,
                            Email = BP.Email,
                            //Detail
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            DiscountRate_Detail = detail.DisRate,
                            UomName = detail.UomName,
                            Amount = detail.Qty * detail.UnitPrice,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            Addre = R.KhmerDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            TotalVat = detail.TaxValue,
                            DiscountValue = detail.DisValue,
                            TotalWTaxSys = detail.TotalWTaxSys,
                            VatNumber = BP.VatNumber,
                            //Summary
                            TotalQty = _context.SaleOrderDetails.Where(x => x.SOID == master.SOID).Sum(x => x.Qty),
                            TotalUniprice = _context.SaleOrderDetails.Where(sc => sc.SOID == master.SOID).Sum(sc => sc.UnitPrice),
                            DiscountValueTotal = _context.SaleOrderDetails.Where(x => x.SOID == detail.SOID).Sum(x => x.DisValue),
                            AmountafterDis = detail.Total,
                            AmountafterDisTotal = _context.SaleOrderDetails.Where(x => x.SOID == master.SOID).Sum(x => x.Total),
                            TotalAmount = _context.SaleOrderDetails.Where(x => x.SOID == master.SOID).Sum(x => x.TotalWTaxSys),
                            VatValues = _context.SaleOrderDetails.Where(x => x.SOID == master.SOID).Sum(x => x.TaxValue),
                            totalsys = _context.SaleOrderDetails.Where(x => x.SOID == master.SOID).Sum(x => x.TotalWTaxSys),

                        }).ToList();
            return list;
        }

        public List<PrintSaleHistory> GetSaleDelivery(int id)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from SD in _context.SaleDeliveries.Where(m => m.SDID == id)
                        join SDD in _context.SaleDeliveryDetails on SD.SDID equals SDD.SDID
                        join BP in _context.BusinessPartners on SD.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SDD.ItemID equals I.ID
                        join CUR in _context.Currency on SD.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SD.BranchID equals B.ID
                        join U in _context.UserAccounts on SD.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SD.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SD, SDD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S } by SDD.SDDID into g
                        let data = g.FirstOrDefault()
                        let master = data.SD
                        let detail = data.SDD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let S = data.S
                        let cp = data.cp
                        let requestedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SD.RequestedBy) ?? new Employee()
                        let receivedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SD.ReceivedBy) ?? new Employee()
                        let shippedBy = _context.Employees.FirstOrDefault(i => i.ID == data.SD.ShippedBy) ?? new Employee()
                        let empName = _context.Employees.FirstOrDefault(i => i.ID == data.SD.SaleEmID) ?? new Employee()
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SD.SDID,
                            ShippedBy = shippedBy.Name,
                            RequestedBy = receivedBy.Name,
                            ReceivedBy = receivedBy.Name,
                            EmpName = empName.Name,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            Title = R.Title,
                            Name2 = BP.Name2,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = B.Name,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            //Detail
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Sub_Total = detail.Total,
                            Sub_Total_Sys = detail.TotalSys,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            SysCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            Addre = R.KhmerDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            BaseOn = master.BasedCopyKeys,
                            //Summary
                            DiscountValue = master.DisValue,
                            VatValue = master.VatValue,
                            TotalAmount = master.TotalAmount,
                            VatNumber = BP.VatNumber
                        }).ToList();
            return list;

        }
        public List<PrintSaleHistory> GetReturnDeilivery(int id)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from SD in _context.ReturnDeliverys.Where(m => m.ID == id)
                        join SDD in _context.ReturnDeliveryDetails on SD.ID equals SDD.ReturnDeliveryID
                        join BP in _context.BusinessPartners on SD.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SDD.ItemID equals I.ID
                        join CUR in _context.Currency on SD.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SD.BranchID equals B.ID
                        join U in _context.UserAccounts on SD.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SD.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SD, SDD, BP, I, CUR, B, U, R, C, pd, pdId, cp, S } by SDD.ID into g
                        let data = g.FirstOrDefault()
                        let master = data.SD
                        let detail = data.SDD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let S = data.S
                        let cp = data.cp
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SD.ID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = B.Name,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            //Detail
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = (double)master.DisValue,
                            VatValue = master.VatValue,
                            TotalAmount = master.TotalAmount
                        }).ToList();
            return list;
        }

        public List<PrintSaleHistory> GetSaleARDownPaymentHistory(int id)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from SAR in _context.ARDownPayments.Where(m => m.ARDID == id)
                        join SARD in _context.ARDownPaymentDetails on SAR.ARDID equals SARD.ARDID
                        join BP in _context.BusinessPartners on SAR.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SARD.ItemID equals I.ID
                        join CUR in _context.Currency on SAR.SaleCurrencyID equals CUR.ID
                        join Lcur in _context.Currency on SAR.LocalCurID equals Lcur.ID
                        join B in _context.Branches on SAR.BranchID equals B.ID
                        join U in _context.UserAccounts on SAR.BranchID equals U.BranchID
                        join RI in _context.ReceiptInformation on B.ID equals RI.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SAR.SeriesID equals S.ID
                        join Ex in _context.ExchangeRates on SAR.ExchangeRate equals Ex.ID
                        group new { SAR, SARD, BP, I, CUR, B, U, RI, C, S, Lcur, Ex } by SARD.ID into g
                        let data = g.FirstOrDefault()
                        let master = data.SAR
                        let detail = data.SARD
                        let cu = data.CUR
                        let I = data.I
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let RI = data.RI
                        let C = data.C
                        let S = data.S
                        let Ex = data.Ex
                        let Lcur = data.Lcur
                        let SAR = data.SAR
                        let SARD = data.SARD
                        let totalQtyByCost = _context.ARDownPaymentDetails.Where(x => x.ARDID == master.ARDID && x.ItemID == detail.ItemID)
                            .Sum(x => x.Qty)
                        let paymterm = _context.PaymentTerms.Where(s => s.ID == BP.PaymentTermsID).FirstOrDefault() ?? new PaymentTerms()
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SAR.ARDID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            Title = RI.Title,
                            Name2 = BP.Name2,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            KhmerDesc = RI.KhmerDescription,
                            EnglishDesc = RI.EnglishDescription,
                            Addresskh = RI.Address,
                            AddressEng = RI.EnglishDescription,
                            PreFix = S.PreFix,
                            Remarks = master.Remarks,
                            DPMRate = master.DPMRate,
                            DPMValue = master.DPMValue,
                            QSNumber = GetSaleARDNumber(master.CopyKey, "SQ"),
                            OrderNumber = GetSaleARDNumber(master.CopyKey, "SO"),
                            DSNumber = GetSaleARDNumber(master.CopyKey, "DN"),

                            //Detail
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = (double)totalQtyByCost,
                            Price = (double)detail.UnitPrice,
                            DiscountValue_Detail = (double)detail.DisValue,
                            DiscountRate_Detail = (double)detail.DisRate,
                            UomName = detail.UomName,
                            Amount = (double)detail.TotalWTax,
                            LocalCurrency = Lcur.Description,
                            SysCurrency = cu.Description,
                            CompanyName = B.Name,
                            Logo = C.Logo,
                            Tel1 = RI.Tel1,
                            Tel2 = RI.Tel2,
                            Exange = Ex.SetRate,
                            //Brand = cp == null ? "" : cp.Name,
                            //Summary
                            Sub_Total = master.SubTotal,
                            DiscountValue = (double)master.DisValue,
                            DiscountRate = (double)master.DisRate,
                            Total = detail.Total,
                            Total_Sys = detail.TotalSys,
                            TaxValue = detail.TaxValue,
                            TaxRate = detail.TaxRate,

                            Applied_Amount = (double)master.AppliedAmount,
                            TotalAmount = master.TotalAmount,
                            TotalAmountSys = cu.Description == "KHR" ? master.TotalAmountSys : master.TotalAmountSys * (double)master.LocalSetRate,
                            Barcode = I.Barcode,
                            ExchangeRate = (double)master.ExchangeRate,
                            LocalSetRate = (double)SAR.LocalSetRate,
                            PriceList = cu.Description,
                            LabelUSA = cu.Description == "KHR" ? "KHR" : cu.Description == "USD" ? "$" : cu.Description,
                            LabelReal = Lcur.Description == "KHR" ? " KHR" : Lcur.Description == "USD" ? "$" : Lcur.Description,
                            Sub_totalAfterdis = master.SubTotalAfterDis,
                            TotalWTaxSys = (double)detail.TotalWTax,
                            Balance_Due_Local = 0,
                            Balance_Due_Sys = 0,
                            BaseOn = master.BasedCopyKeys,
                            BPBrandName = "",
                            Debit = (double)master.TotalAmount - (double)master.AppliedAmount,
                            //DebitSys = cu.Description == "KHR" ? master.TotalAmount - master.AppliedAmount :(master.TotalAmount - master.AppliedAmount) * master.LocalSetRate,
                            //Paymenterm = paymterm.Days,
                            VatNumber = BP.VatNumber,
                            Email = BP.Email,
                        }).ToList();
            return list;
        }

        public List<PrintSaleHistory> GetSaleCreditmemo(int id)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from SC in _context.SaleCreditMemos.Where(m => m.SCMOID == id)
                        join SCD in _context.SaleCreditMemoDetails on SC.SCMOID equals SCD.SCMOID
                        join BP in _context.BusinessPartners on SC.CusID equals BP.ID
                        join I in _context.ItemMasterDatas on SCD.ItemID equals I.ID
                        join CUR in _context.Currency on SC.SaleCurrencyID equals CUR.ID
                        join B in _context.Branches on SC.BranchID equals B.ID
                        join U in _context.UserAccounts on SC.BranchID equals U.BranchID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join C in _context.Company on B.CompanyID equals C.ID
                        join S in _context.Series on SC.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                        group new { SC, SCD, BP, I, CUR, B, U, R, C, cp, S } by SCD.SCMODID into g
                        let data = g.FirstOrDefault()
                        let master = data.SC
                        let detail = data.SCD
                        let cu = data.CUR
                        let BP = data.BP
                        let B = data.B
                        let U = data.U
                        let R = data.R
                        let C = data.C
                        let cp = data.cp
                        let S = data.S
                        select new PrintSaleHistory
                        {
                            //Master
                            ID = g.FirstOrDefault().SC.SCMOID,
                            Invoice = master.InvoiceNumber,
                            PostingDate = master.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = master.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = master.DueDate.ToString("dd-MM-yyyy"),
                            CusName = BP.Name,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = U.Username,
                            CusNo = BP.Code,
                            RefNo = master.RefNo,
                            Branch = g.First().B.Name,
                            BaseOn = master.CopyKey,
                            Remarks = master.Remarks,
                            //Detail
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            ItemCode = detail.ItemCode,
                            ItemNameKh = detail.ItemNameKH,
                            Qty = detail.Qty,
                            Price = detail.UnitPrice,
                            DiscountValue_Detail = detail.DisValue,
                            UomName = detail.UomName,
                            Amount = detail.Total,
                            LocalCurrency = cu.Description,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Addresskh = R.Address,
                            AddressEng = R.EnglishDescription,
                            CompanyName = C.Name,
                            Logo = C.Logo,
                            //Summary
                            DiscountValue = master.DisValue,
                            VatValue = master.VatValue,
                            Applied_Amount = master.AppliedAmount,
                            TotalAmount = master.TotalAmount
                        }).ToList();
            //list = null;
            return list;
        }
        public List<PrintPurchaseAP> GetPurchaseOrder(int purchaseid)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from PO in _context.PurchaseOrders.Where(m => m.PurchaseOrderID == purchaseid)
                        join POD in _context.PurchaseOrderDetails on PO.PurchaseOrderID equals POD.PurchaseOrderID
                        join BP in _context.BusinessPartners on PO.VendorID equals BP.ID
                        join U in _context.UserAccounts on PO.UserID equals U.ID
                        join B in _context.Branches on PO.BranchID equals B.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PO.PurCurrencyID equals C.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join I in _context.ItemMasterDatas on POD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on POD.UomID equals N.ID
                        join S in _context.Series on PO.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            Brand = cp == null ? "" : cp.Name,
                            PreFix = S.PreFix,
                            CusCode = BP.Code,
                            Invoice = PO.Number,
                            ExchangeRate = PO.PurRate,
                            Balance_Due_Sys = PO.BalanceDueSys,
                            Applied_Amount = PO.AppliedAmount,
                            PostingDate = PO.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PO.DocumentDate.ToString("dd-MM-yyyy"),
                            DueDate = PO.DeliveryDate.ToString("dd-MM-yyyy"),
                            DiscountValue = PO.DiscountValue,
                            DiscountValue_Detail = POD.DiscountValue,
                            TaxValue = PO.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PO.TaxRate,
                            TypeDis = PO.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PO.ReffNo,
                            VendorNo = PO.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            Total = POD.Total,
                            Sub_Total = PO.BalanceDue,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = POD.PurchasPrice,
                            UomName = N.Name,
                            Qty = POD.Qty,
                            Remark = PO.Remark
                        }).ToList();

            return list;
        }
        public List<PrintPurchaseAP> GetPurchasePO(int purchaseid)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from PR in _context.GoodsReciptPOs.Where(m => m.ID == purchaseid)
                        join PRD in _context.GoodReciptPODetails on PR.ID equals PRD.GoodsReciptPOID
                        join BP in _context.BusinessPartners on PR.VendorID equals BP.ID
                        join U in _context.UserAccounts on PR.UserID equals U.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PR.PurCurrencyID equals C.ID
                        join I in _context.ItemMasterDatas on PRD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PRD.UomID equals N.ID
                        join B in _context.Branches on PR.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join S in _context.Series on PR.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            Invoice = PR.InvoiceNo,
                            CusCode = BP.Code,
                            ExchangeRate = PR.PurRate,
                            Balance_Due_Sys = PR.BalanceDueSys,
                            Applied_Amount = PR.AppliedAmount,
                            DiscountValue_Detail = PR.DiscountValue,
                            DiscountRate_Detail = PR.DiscountRate,
                            PostingDate = PR.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PR.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PR.di.ToString("dd-MM-yyyy"),
                            Sub_Total = PR.BalanceDue,
                            DiscountValue = PR.DiscountValue,
                            DiscountRate = PR.DiscountRate,
                            VendorName = BP.Name,
                            CompanyName = D.Name,
                            Phone = BP.Phone,
                            Addresskh = R.Address,
                            Address = BP.Address,
                            UserName = E.Name,
                            LocalCurrency = C.Description,
                            //Detail
                            SQN = PR.ReffNo,
                            Logo = D.Logo,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = I.UnitPrice,
                            UomName = N.Name,
                            Qty = PRD.Qty,
                            Remark = PR.Remark,
                        }).ToList();
            return list;
        }
        public List<PrintPurchaseAP> GetPrintPurchaseCreditmemo(int purchaseid)

        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active) ?? new Property();
            var list = (from PC in _context.PurchaseCreditMemos.Where(m => m.PurchaseMemoID == purchaseid)
                        join PCD in _context.PurchaseCreditMemoDetails on PC.PurchaseMemoID equals PCD.PurchaseCreditMemoID
                        join BP in _context.BusinessPartners on PC.VendorID equals BP.ID
                        join U in _context.UserAccounts on PC.UserID equals U.ID
                        join E in _context.Employees on U.EmployeeID equals E.ID
                        join C in _context.Currency on PC.PurCurrencyID equals C.ID
                        join I in _context.ItemMasterDatas on PCD.ItemID equals I.ID
                        join N in _context.UnitofMeasures on PCD.UomID equals N.ID
                        join B in _context.Branches on PC.BranchID equals B.ID
                        join R in _context.ReceiptInformation on B.ID equals R.ID
                        join D in _context.Company on B.CompanyID equals D.ID
                        join S in _context.Series on PC.SeriesID equals S.ID
                        let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == I.ID)
                        let pdId = pd == null ? 0 : pd.Value
                        let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)

                        select new PrintPurchaseAP
                        {
                            AddressEng = R.EnglishDescription,
                            PreFix = S.PreFix,
                            Brand = cp == null ? "" : cp.Name,
                            CusCode = BP.Code,
                            Invoice = PC.Number,
                            Balance_Due_Sys = PC.BalanceDueSys,
                            Applied_Amount = PC.AppliedAmount,
                            PostingDate = PC.PostingDate.ToString("dd-MM-yyyy"),
                            DocumentDate = PC.DocumentDate.ToString("dd-MM-yyyy"),
                            //DueDate = PC.DeliveryDate.ToString("dd-MM-yyyy"),
                            Sub_Total = PC.BalanceDue,
                            DiscountValue = PC.DiscountValue,
                            DiscountRate = PC.DiscountRate,
                            TaxValue = PC.TaxValue,
                            CompanyName = D.Name,
                            Addresskh = R.Address,
                            TaxRate = PC.TaxRate,
                            TypeDis = PC.TypeDis,
                            VendorName = BP.Name,
                            Tel1 = R.Tel1,
                            Tel2 = R.Tel2,
                            Phone = BP.Phone,
                            Address = BP.Address,
                            UserName = E.Name,
                            SQN = PC.ReffNo,
                            VendorNo = PC.Number,
                            Logo = D.Logo,
                            LocalCurrency = C.Description,
                            //Deatai
                            Total = PCD.Total,
                            KhmerName = I.KhmerName,
                            Code = I.Code,
                            Price = PCD.PurchasPrice,
                            UomName = N.Name,
                            Qty = PCD.Qty,
                            Remark = PC.Remark
                        }).ToList();
            return list;
        }
        //credtmemo
        public List<DevSummarySale> GetSaleCrditMemoReport(string DateFrom, string DateTo, string TimeFrom, string TimeTo, int BranchID, int UserID, string DouType)
        {
            List<ReceiptMemo> receiptsFilter = new();
            List<SaleCreditMemo> saleARs = new();
            bool isDateFrom = !string.IsNullOrEmpty(DateFrom);
            bool isDateTo = !string.IsNullOrEmpty(DateTo);
            bool isTimeFrom = TimeFrom != "0";
            bool isTimeTo = TimeTo != "0";
            //if (DateFrom == null || DateTo == null)
            //{
            //    return saleARs;
            //}
            if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && BranchID == 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == _user.Company.ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == _user.Company.ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && BranchID != 0 && UserID != 0)
            {
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == _user.Company.ID && w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
            }
            else if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == _user.Company.ID && w.BranchID == BranchID).ToList();
            }
            else if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && BranchID == 0 && UserID != 0)
            {
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == _user.Company.ID && w.UserOrderID == UserID).ToList();
            }


            if (isDateFrom && isDateTo && BranchID == 0 && UserID == 0)
            {
                saleARs = _context.SaleCreditMemos.Where(w => w.CompanyID == _user.Company.ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (isDateFrom && isDateTo && BranchID != 0 && UserID == 0)
            {
                saleARs = _context.SaleCreditMemos.Where(w => w.CompanyID == _user.Company.ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (isDateFrom && isDateTo && BranchID != 0 && UserID != 0)
            {
                saleARs = _context.SaleCreditMemos.Where(w => w.CompanyID == _user.Company.ID && w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (isDateFrom && isDateTo && BranchID != 0 && UserID == 0)
            {
                saleARs = _context.SaleCreditMemos.Where(w => w.CompanyID == _user.Company.ID && w.BranchID == BranchID).ToList();
            }
            else if (isDateFrom && isDateTo && BranchID == 0 && UserID != 0)
            {
                saleARs = _context.SaleCreditMemos.Where(w => w.CompanyID == _user.Company.ID && w.UserID == UserID).ToList();
            }





            else if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && BranchID == 0 && UserID == 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", isDateFrom, isTimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", isDateTo, isTimeTo));

                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == _user.Company.ID && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo).ToList();
            }
            else if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && BranchID != 0 && UserID == 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", isDateFrom, isTimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", isDateTo, isTimeTo));

                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == _user.Company.ID && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo && w.BranchID == BranchID).ToList();
            }
            else if (isDateFrom && isDateTo && !isTimeFrom && !isTimeTo && BranchID != 0 && UserID != 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", isDateFrom, isTimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", isDateTo, isTimeTo));
                receiptsFilter = _context.ReceiptMemo.Where(w => w.CompanyID == _user.Company.ID && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
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
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == _user.Company.LocalCurrencyID) ?? new Display();
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
                return Sale;
            }
            else if (DouType == "CN")
            {
                return saleAR;
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
                               select new DevSummarySale
                               {
                                   RefNo = data.all.RefNo,
                                   DouType = data.all.DouType,
                                   EmpCode = data.all.EmpCode,
                                   EmpName = data.all.EmpName,
                                   BranchID = data.all.BranchID,
                                   BranchName = data.b.Name,
                                   ReceiptNo = data.all.ReceiptNo,
                                   DateOut = data.all.DateOut,
                                   TimeOut = data.all.TimeOut,
                                   DiscountItem = data.all.DiscountItem,
                                   Currency = data.all.Currency,
                                   GrandTotal = data.all.GrandTotal,
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
                               }).ToList();
                return allSale;
            }
        }

        public string GetSaleARDNumber(string BaseOn, string type)
        {
            BaseOn = BaseOn ?? "";
            string[] arr = Array.Empty<string>();

            arr = BaseOn.Split("/");
            string returnString = "";
            foreach (string item in arr)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    string[] sale = item.Split(':');
                    string docNumber = sale[1];
                    string[] docNumbers = docNumber.Split('-');
                    string docType = docNumbers[0];
                    string innumber = docNumbers[1];
                    if (docType == "SQ" && type == "SQ")
                    {
                        returnString = innumber;
                    }
                    else if (docType == "SO" && type == "SO")
                    {
                        returnString = innumber;
                    }
                    else if (docType == "DN" && type == "DN")
                    {
                        returnString = innumber;
                    }
                }

            }

            return returnString;
        }

    }

}

