using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.BOM;
using CKBS.Models.Services.Inventory.Transaction;
using CKBS.Models.Services.POS;

using CKBS.Models.Services.ReportSale.dev;
using Rotativa.AspNetCore;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.ServicesClass;
using CKBS.Models.ServicesClass.FinancailReportPrint;
using CKBS.Models.Services.ReportInventory;
using KEDI.Core.Premise.Authorization;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Sale;
using CKBS.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.FinancialReports;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.POS.KVMS;
using KEDI.Core.Premise.Models.ServicesClass.KSMS;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Repository;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Premise.Models.ServicesClass.FinancailReportPrint;
using KEDI.Core.Premise.Models.Services.ReportSale.dev;
using KEDI.Core.Premise.Models.ServicesClass.Report;
using KEDI.Core.Premise.Models.Services.POS;
using CKBS.Models.Services.Inventory.Property;
using KEDI.Core.Premise.Models.Services.HumanResources;

namespace CKBS.Controllers
{
    [Privilege]
    public class DevPrintController : Controller
    {
        private readonly DataContext _context;
        private readonly UtilityModule _fncModule;
        private readonly IReport _report;
        private readonly IFinancialReports _financialReports;
        private readonly UserAccount _user;
        public DevPrintController(DataContext context, UtilityModule format, IReport report, IFinancialReports financialReports, UserManager userModule)
        {
            _context = context;
            _fncModule = format;
            _report = report;
            _financialReports = financialReports;
            _user = userModule.CurrentUser;
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

        //Print Summary Sale
        public IActionResult PrintSummarySale(string DateFrom, string DateTo, int BranchID, int UserID, string TimeFrom, string TimeTo, int plid, string doctype)
        {
            List<Receipt> receiptsFilter = new();
            List<SaleAR> saleArFilter = new();
            if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID == 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo)).ToList();
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom == null && TimeTo == null && BranchID != 0 && UserID != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= Convert.ToDateTime(DateFrom) && w.DateOut <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID == 0 && UserID == 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo)).ToList();
                receiptsFilter = _context.Receipt.Where(w => Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID != 0 && UserID == 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID).ToList();
                receiptsFilter = _context.Receipt.Where(w => Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo && w.BranchID == BranchID).ToList();
            }
            else if (DateFrom != null && DateTo != null && TimeFrom != null && TimeTo != null && BranchID != 0 && UserID != 0)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", DateFrom, TimeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", DateTo, TimeTo));
                saleArFilter = _context.SaleARs.Where(w => w.PostingDate >= Convert.ToDateTime(DateFrom) && w.PostingDate <= Convert.ToDateTime(DateTo) && w.BranchID == BranchID && w.UserID == UserID).ToList();
                receiptsFilter = _context.Receipt.Where(w => Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) >= dateFrom && Convert.ToDateTime(w.DateOut.ToString("MM-dd-yyyy") + " " + w.TimeOut.ToString()) <= dateTo && w.BranchID == BranchID && w.UserOrderID == UserID).ToList();
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
            if (TimeFrom == null)
            {
                TimeFrom = "00:00";
            }
            if (TimeTo == null)
            {
                TimeTo = "00:00";
            }
            List<SummaryTotalSale> Summary = new();
            var Branch = "All";
            var EmpName = "All";
            var Logo = "";
            var TimeF = "";
            var TimeT = "";
            if (TimeFrom != null && TimeTo != null)
            {
                TimeF = Convert.ToDateTime(TimeFrom).ToString("hh:mm tt");
                TimeT = Convert.ToDateTime(TimeTo).ToString("hh:mm tt");
            }
            if (BranchID != 0)
            {
                Branch = _context.Branches.FirstOrDefault(w => w.ID == BranchID).Name;
                Logo = _context.Branches.Include(c => c.Company).FirstOrDefault(w => w.ID == BranchID).Company.Logo;
            }
            if (UserID != 0)
            {
                EmpName = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == UserID).Employee.Name;
            }
            List<GroupSummarySale> Sale = new();
            List<GroupSummarySale> saleAr = new();


            if (doctype == "SP" || doctype == "All")
            {
                var Receipts = receiptsFilter;
                if (plid > 0)
                {
                    Receipts = Receipts.Where(i => i.PriceListID == plid).ToList();
                    Summary = GetSummaryTotals(DateFrom, DateTo, BranchID, UserID, TimeFrom, TimeTo, plid) ?? new List<SummaryTotalSale>();
                }
                else
                {
                    Summary = GetSummaryTotals(DateFrom, DateTo, BranchID, UserID, TimeFrom, TimeTo) ?? new List<SummaryTotalSale>();
                }
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
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
                                DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
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
            }
            if (doctype == "IN" || doctype == "All")
            {
                if (plid > 0) saleArFilter = saleArFilter.Where(i => i.PriceListID == plid).ToList();
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
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
                var currsys = _context.Currency.Find(GetCompany().SystemCurrencyID) ?? new Currency();
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
                                  DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                  DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
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
            }
            if (doctype == "SP")
            {
                if (Sale.Any())
                {
                    return new ViewAsPdf(Sale);
                }
            }
            else if (doctype == "IN")
            {
                if (saleAr.Any())
                {
                    return new ViewAsPdf(saleAr);
                }
            }
            else
            {
                List<GroupSummarySale> allSales = new(Sale.Count + saleAr.Count);
                allSales.AddRange(Sale);
                allSales.AddRange(saleAr);
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
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
                return new ViewAsPdf(data)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            return Ok();
        }

        //Print Detail Sale
        public IActionResult PrintDetailSale(string DateFrom, string DateTo, int BranchID, int UserID)
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
                var Branch = "All";
                var EmpName = "All";
                var Logo = "";
                if (BranchID != 0)
                {
                    Branch = _context.Branches.FirstOrDefault(w => w.ID == BranchID).Name;
                    Logo = _context.Branches.Include(c => c.Company).FirstOrDefault(w => w.ID == BranchID).Company.Logo;
                }
                if (UserID != 0)
                {
                    EmpName = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == UserID).Employee.Name;
                }
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
                var List = from r in receiptsFilter
                           join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                           join u in _context.UnitofMeasures on rd.UomID equals u.ID
                           join user in _context.UserAccounts on r.UserOrderID equals user.ID
                           join bra in _context.Branches on user.BranchID equals bra.ID
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           join curr_pl in _context.Currency on r.PLCurrencyID equals curr_pl.ID
                           join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                           join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                           group new { r, rd, emp, bra, u, curr_pl, curr, curr_sys } by r.ReceiptID into g
                           select new GroupDetailSale
                           {
                               ReceiptID = g.Key,
                               TimeOut = g.First().r.TimeOut,
                               EmpName = g.First().emp.Name,
                               DateIn = g.First().r.DateIn.ToString("dd-MM-yyy"),
                               DateOut = g.First().r.DateOut.ToString("dd-MM-yyy"),
                               ReceiptNo = g.First().r.ReceiptNo,
                               DisInvoice = g.First().r.DiscountValue,
                               TotalTax = g.First().r.TaxValue,
                               Currency = g.First().curr_pl.Description,
                               GrandTotal = g.First().r.GrandTotal,
                               DetailItems = g.Select(x => new DetailItem
                               {
                                   Code = x.rd.Code,
                                   ItemName = x.rd.KhmerName,
                                   EnglisName = x.rd.EnglishName,
                                   Qty = x.rd.Qty,
                                   UoM = x.u.Name,
                                   SalePrice = x.rd.UnitPrice,
                                   DisItem = x.rd.DiscountValue,
                                   Total = x.rd.Total,
                               }).ToList(),
                               Header = new Header
                               {
                                   Logo = Logo,
                                   DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                   DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                   Branch = Branch,
                                   EmpName = EmpName
                               },
                               Footer = new Footer
                               {
                                   CountReceipt = string.Format("{0:#,0}", Summary.FirstOrDefault().CountReceipt),
                                   SoldAmount = _fncModule.ToCurrency(Summary.FirstOrDefault().SoldAmount, sysCur.Amounts),
                                   DiscountItem = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                                   DiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                                   TaxValue = g.First().curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().TaxValue, sysCur.Amounts),
                                   GrandTotal = g.First().curr.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotal, lcCur.Amounts),
                                   GrandTotalSys = g.First().curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotalSys, sysCur.Amounts)
                               }
                           };
                if (List.Any())
                {
                    return new ViewAsPdf(List)
                    {
                        CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                    };
                }
            }
            return Ok();
        }
        //Print Close Shift Detail
        public IActionResult PrintCloseShiftDetailall(double TranF, double TranT, int UserID/*,string Type="POS"*/)
        {
            ViewBag.CloseShift = "highlight";
            var Branch = _context.UserAccounts.Include(b => b.Branch).Include(c => c.Company).FirstOrDefault(w => w.ID == UserID);
            var datas = _report.GetCashoutReport(TranF, TranT, UserID, GetCompany().LocalCurrencyID).ToList();
            var sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
            var list = from item in datas
                       select new CloseshiftdetailView
                       {
                           Trans = $"{TranF}/{TranT}/{UserID}",
                           GroupName = item.ItemGroup1 + "/" + item.ItemGroup2 + "/" + item.ItemGroup3,
                           Barcode = item.Barcode,
                           Logo = Branch.Company.Logo,
                           Branch = Branch.Branch.Name,
                           DateIn = item.DateIn,
                           DateOut = item.DateOut,
                           EmpName = item.EmpName,
                           ExchangeRate = string.Format("{0:n3}", item.ExchangeRate),
                           Code = item.Code,
                           ItemName = item.KhmerName,
                           Qty = item.Qty,
                           SalePrice = item.Price,
                           DisItem = item.DisItemValue,
                           Total = item.Total,
                           UoM = item.Uom,
                           CountReceipt = string.Format("{0:n0}", datas.Count),
                           SoldAmount = string.Format("{0:n3}", item.TotalSoldAmount),
                           DiscountItem = string.Format("{0:n3}", item.TotalDiscountItem),
                           DiscountTotal = string.Format("{0:n3}", item.TotalDiscountTotal),
                           TaxValue = item.Currency_Sys + " " + string.Format("{0:n3}", item.TotalVat),
                           GrandTotalSys = item.Currency_Sys + " " + string.Format("{0:n3}", item.GrandTotal_Sys),
                           GrandTotal = item.Currency + " " + string.Format("{0:n3}", item.GrandTotal),
                       };
            return View(list);

        }
        //Print Close Shift Detail
        public IActionResult PrintCloseShiftDetail(double TranF, double TranT, int UserID)
        {
            var Branch = _context.UserAccounts.Include(b => b.Branch).Include(c => c.Company).FirstOrDefault(w => w.ID == UserID);
            var datas = _report.GetCashoutReport(TranF, TranT, UserID, GetCompany().LocalCurrencyID).ToList();
            var sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
            var list = from item in datas
                       group item by new { item.ItemGroup1, item.ItemGroup2, item.ItemGroup3 } into data
                       let d = data.FirstOrDefault()
                       select new GroupCloseShiftDetail
                       {
                           GroupName = d.ItemGroup1 + "/" + d.ItemGroup2 + "/" + d.ItemGroup3,
                           SubTotal = _fncModule.ToCurrency(data.Sum(i => i.TotalCal), sysCur.Amounts),
                           HeaderCloseShift = new HeaderCloseShift
                           {
                               Logo = Branch.Company.Logo,
                               Branch = Branch.Branch.Name,
                               DateIn = d.DateIn,
                               DateOut = d.DateOut,
                               EmpName = d.EmpName,
                               ExchangeRate = string.Format("{0:#,0}", d.ExchangeRate)
                           },
                           DetailItems = data.
                           Select(detail => new DetailItem
                           {
                               Code = detail.Code,
                               Barcode = detail.Barcode,
                               ItemName = detail.KhmerName,
                               QtyD = detail.Qty,
                               SalePriceD = detail.Price,
                               DisItemD = detail.DisItemValue,
                               TotalD = detail.Total,
                               UoM = detail.Uom,
                           }).ToList(),
                           Footer = new Footer
                           {
                               CountReceipt = string.Format("{0:#,0}", datas.Count),
                               SoldAmount = d.TotalSoldAmount,
                               DiscountItem = d.TotalDiscountItem,
                               DiscountTotal = d.TotalDiscountTotal,
                               TaxValue = d.Currency_Sys + " " + d.TotalVat,
                               GrandTotalSys = d.Currency_Sys + " " + d.GrandTotal_Sys,
                               GrandTotal = d.Currency + " " + d.GrandTotal,
                           }
                       };
            int count = list.Count();
            if (count == 0)
            {
                return Ok(count);
            }
            else
            {
                return new ViewAsPdf(list)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }

        }

        //Print Top Sale
        public IActionResult PrintTopSaleQuantity(string DateFrom, string DateTo, int BranchID)
        {
            List<Receipt> receiptsFilter = new();
            List<ReceiptMemo> receiptsMemoFilter = new();
            DateTime dateFrom = Convert.ToDateTime(DateFrom);
            DateTime dateto = Convert.ToDateTime(DateTo);
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
                        select new Topsaleviewmodel
                        {
                            GroupName = g.First().g1.Name + "/" + g.First().g2.Name + "/" + g.First().g3.Name,
                            C = data.r.ReceiptID,
                            ItemID = g.Key.ItemID,
                            Group1 = data.g1.Name,
                            Group2 = data.g2.Name,
                            Group3 = data.g3.Name,
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
                            SVatCal = data.r.TaxValue * data.r.ExchangeRate,
                            SDiscountItemCallocal = g.Sum(c => c.rd.DiscountValue * data.r.LocalSetRate),
                            LocalSetRate = data.r.LocalSetRate,
                            SGrandTotalSys = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate),
                            SGrandTotal = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate) * data.r.LocalSetRate,
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
                            select new Topsaleviewmodel
                            {
                                GroupName = g.First().g1.Name + "/" + g.First().g2.Name + "/" + g.First().g3.Name,
                                C = data.r.ReceiptKvmsID,
                                ItemID = g.Key.ItemID,
                                Barcode = data.i.Barcode,
                                Code = data.i.Code,
                                KhmerName = data.i.KhmerName,
                                Qty = g.Sum(c => c.rd.Qty) * -1,
                                ReturnQty = g.Sum(c => c.rd.Qty),
                                Uom = data.u.Name,
                                //CurrencyId = data.curr.ID,
                                BrandID = data.r.BranchID,
                                SysCurrency = data.curr_sys.Description,
                                Currency = data.curr.Description,
                                LocalCurrency = data.LCcurr.Description,
                                PriceCal = g.Sum(i => i.rd.UnitPrice) * data.r.PLRate,
                                TotalCal = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate) * -1,
                                SubTotalCal = (data.rd.Qty * data.rd.UnitPrice * data.r.PLRate) * -1,
                                ////Summary
                                SDiscountItemCal = g.Sum(c => c.rd.DisValue * data.r.PLRate) * -1,
                                SDiscountTotalCal = g.Sum(c => c.r.DisValue * data.r.PLRate) * -1,
                                SVatCal = (data.r.TaxValue * data.r.ExchangeRate) * -1,
                                SDiscountItemCallocal = g.Sum(c => c.rd.DisValue * data.r.LocalSetRate) * -1,
                                LocalSetRate = data.r.LocalSetRate,
                                SGrandTotalSys = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate) * -1,
                                SGrandTotal = g.Sum(c => c.rd.Qty * data.rd.UnitPrice * data.r.PLRate) * data.r.LocalSetRate * -1,
                            }).ToList();
            #endregion
            #region bind receipt and receipt memo
            List<Topsaleviewmodel> topSaleQty = new(list.Count + listMemo.Count);
            topSaleQty.AddRange(list);
            topSaleQty.AddRange(listMemo);
            var detialData = (from all in topSaleQty
                              group all by new { all.ItemID, all.GroupName, all.PriceCal } into g
                              let d = g.FirstOrDefault()
                              let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == d.CurrencyId) ?? new Display()
                              select new Topsaleviewmodel
                              {
                                  C = d.C,
                                  ItemID = g.Key.ItemID,
                                  GroupName = d.GroupName,
                                  Barcode = d.Barcode,
                                  Code = d.Code,
                                  Currency = d.Currency,
                                  DateFrom = d.DateFrom,
                                  DateTo = d.DateTo,
                                  KhmerName = d.KhmerName,
                                  TotalCal = d.TotalCal,
                                  PriceCal = d.PriceCal,
                                  Price = _fncModule.ToCurrency(d.PriceCal, plCur.Amounts),
                                  SubTotalCal = d.PriceCal * g.Sum(i => i.Qty),
                                  Uom = d.Uom,
                                  Qty = g.Where(i => i.Qty > 0).Sum(i => i.Qty),
                                  ReturnQty = g.Sum(i => i.ReturnQty),
                                  TotalQty = g.Sum(i => i.Qty),
                                  Total = d.PriceCal * g.Sum(i => i.Qty),
                              }).ToList();

            var totalData = topSaleQty.GroupBy(i => i.C).Select(i => i.FirstOrDefault()).ToList();
            var allData = (from all in topSaleQty
                           group new { all } by new { all.ItemID } into g
                           let data = g.FirstOrDefault()
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.all.CurrencyId) ?? new Display()
                           //let sumByBranch = topSaleQty.Where(r => r.BrandID == data.all.BrandID).Sum(i => i.SGrandTotalSysCal)
                           select new GroupTopSaleQty
                           {
                               GroupName = data.all.Group1 + "/" + data.all.Group2 + "/" + data.all.Group3,
                               SubTotal = detialData.Where(i => i.ItemID == g.Key.ItemID).Sum(i => i.Total),
                               Topsaleviewmodels = detialData.Where(i => i.ItemID == g.Key.ItemID).ToList(),
                               //
                               Header = new Header
                               {
                                   DateFrom = DateFrom,
                                   DateTo = DateTo,
                               },
                               Footer = new Footer
                               {
                                   SubTotalCal = detialData.Where(i => i.ItemID == g.Key.ItemID).Sum(i => i.Total),
                                   // SDiscountItem = $"{data.all.Currency} {_fncModule.ToCurrency(topSaleQty.Sum(i => i.SDiscountItemCal), plCur.Amounts)}",
                                   // SDiscountTotal = $"{data.all.Currency} {_fncModule.ToCurrency(topSaleQty.Sum(i => i.SDiscountTotalCal), plCur.Amounts)}",
                                   // SVat = $"{data.all.Currency} {_fncModule.ToCurrency(topSaleQty.Sum(x => x.SVatCal), plCur.Amounts)}",
                                   // SGrandTotalSys = $"{data.all.Currency} {_fncModule.ToCurrency(topSaleQty.Sum(x => x.SGrandTotalSys), plCur.Amounts)}",
                                   // SGrandTotal = _fncModule.ToCurrency(topSaleQty.Sum(x => x.SGrandTotal))

                                   SDiscountItem = $"{data.all.Currency} {_fncModule.ToCurrency(topSaleQty.Sum(i => i.SDiscountItemCal), plCur.Amounts)}",
                                   SDiscountTotal = $"{data.all.Currency} {_fncModule.ToCurrency(TotalDis, plCur.Amounts)}",
                                   SGrandTotalSys = $"{data.all.Currency} {_fncModule.ToCurrency((topSaleQty.Sum(x => x.SGrandTotalSys)) - (topSaleQty.Sum(i => i.SDiscountItemCal)) - (TotalDis), plCur.Amounts)}",
                                   SGrandTotal = $"{data.all.LocalCurrency} {_fncModule.ToCurrency((topSaleQty.Sum(x => x.SGrandTotal)) - (topSaleQty.Sum(i => i.SDiscountItemCallocal)) - (TotalDisCal), plCur.Amounts)}",
                               }
                           }).ToList();
            if (allData.Count > 0)
            {
                return new ViewAsPdf(allData)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            return Ok(allData.OrderByDescending(o => o.Qty));
            #endregion
            //return Ok(allData.OrderByDescending(o => o.Qty));
        }

        //Print Payment Means
        public IActionResult PrintPaymentMeans(string dateFrom, string dateTo, int branchId, int userId, int paymentId)
        {
            List<Receipt> receiptsFilter = new();
            List<MultiPaymentMeans> multiPaymentMeans = new();
            DateTime _dateFrom = Convert.ToDateTime(dateFrom);
            DateTime _dateTo = Convert.ToDateTime(dateTo);
            //===========================================sdfsdfsdf======================
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
            else if (dateFrom != null && dateTo != null && branchId == 0 && userId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.DateOut >= _dateFrom && w.DateOut <= _dateTo && w.UserOrderID == userId).ToList();
            }
            else if (dateFrom == null && dateTo == null && branchId != 0 && userId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.UserOrderID == userId && w.BranchID == branchId).ToList();
            }
            else if (dateFrom == null && dateTo == null && branchId == 0 && userId != 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.UserOrderID == userId).ToList();
            }
            else if (dateFrom == null && dateTo == null && branchId != 0 && userId == 0)
            {
                receiptsFilter = _context.Receipt.Where(w => w.BranchID == branchId).ToList();
            }
            else
            {
                return Ok(new List<Receipt>());
            }
            var Summary = GetSummaryTotals(dateFrom, dateTo, branchId, userId, "", "");
            if (Summary != null)
            {
                var Branch = "All";
                var EmpName = "All";
                var Logo = "";
                if (branchId != 0)
                {
                    Branch = _context.Branches.FirstOrDefault(w => w.ID == branchId).Name;
                    Logo = _context.Branches.Include(c => c.Company).FirstOrDefault(w => w.ID == branchId).Company.Logo;
                }
                if (userId != 0)
                {
                    EmpName = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == userId).Employee.Name;
                }
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                //var sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();


                var paymentMeans = paymentId == 0 ? _context.MultiPaymentMeans.ToList()
                                     : paymentId == -1 ? _context.MultiPaymentMeans.Where(s => s.Type == PaymentMeanType.CardMember).ToList()
                                     : _context.MultiPaymentMeans.Where(s => s.PaymentMeanID == paymentId).ToList();
                var List = (from p in paymentMeans
                            join r in receiptsFilter on p.ReceiptID equals r.ReceiptID
                            join u in _context.UserAccounts on r.UserOrderID equals u.ID
                            join e in _context.Employees on u.EmployeeID equals e.ID
                            join curr_pl in _context.Currency on p.AltCurrencyID equals curr_pl.ID
                            join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                            join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                            join cus in _context.BusinessPartners on r.CustomerID equals cus.ID
                            group new { p, r, e, curr_pl, curr, curr_sys, cus } by new { p.PaymentMeanID, } into c
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == c.First().curr_pl.ID) ?? new Display()
                            let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == c.First().curr_sys.ID) ?? new Display()
                            let multipay = c.FirstOrDefault().p
                            select new GroupPaymentMean
                            {
                                PaymentType = multipay.Type == PaymentMeanType.Normal ? _context.PaymentMeans.FirstOrDefault(s => s.ID == multipay.PaymentMeanID).Type : "Card Member",
                                SubTotal = c.Sum(s => s.r.GrandTotal_Sys),
                                Receipts = c.Select(receipt => new Receipts
                                {
                                    Receipt = receipt.r.ReceiptNo,
                                    User = receipt.e.Name,
                                    DateIn = receipt.r.DateIn.ToString("dd-MM-yyyy"),
                                    TimeIn = receipt.r.TimeIn,

                                    DateOut = receipt.r.DateOut.ToString("dd-MM-yyyy"),
                                    TimeOut = receipt.r.TimeOut,
                                    Currency = receipt.curr_pl.Description,
                                    TotalPayment = receipt.p.Amount,
                                    TotalGrouptPayment = receipt.p.Amount * receipt.p.SCRate,
                                    GrandTotal = receipt.r.GrandTotal_Sys,
                                    Customer = c.First().cus.Name,
                                    Remark = receipt.r.RemarkDiscount,
                                }).ToList(),
                                Header = new Header
                                {
                                    Logo = Logo,
                                    DateFrom = dateFrom,
                                    DateTo = dateTo,
                                    Branch = Branch,
                                    EmpName = EmpName
                                },
                                Footer = new Footer
                                {
                                    CountReceipt = string.Format("{0:#,0}", Summary.FirstOrDefault().CountReceipt),
                                    SoldAmount = _fncModule.ToCurrency(Summary.FirstOrDefault().SoldAmount, sysCur.Amounts),
                                    DiscountItem = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                                    DiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                                    TaxValue = c.First().curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().TaxValue, sysCur.Amounts),
                                    GrandTotalSys = c.First().curr_sys.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotalSys, sysCur.Amounts),
                                    GrandTotal = c.First().curr.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotal, lcCur.Amounts)
                                }
                            }).ToList();




                return new ViewAsPdf(List.ToList())
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            return Ok();
        }

        //Print Sale By Customer
        public async Task<IActionResult> PrintSaleByCustomer(string dateFrom, string dateTo, int branchId, int cusId)
        {
            var lists = await _report.GetSaleByCustomers(dateFrom, dateTo, branchId, cusId, GetCompany().LocalCurrencyID);
            var _data = (from d in lists
                         group d by d.CusId into g
                         let data = g.FirstOrDefault()
                         select new GroupSaleByCustomer
                         {
                             CusName = data.MCustomer,
                             SubCusTotal = data.MCusTotal,
                             MasterDetails = g.GroupBy(i => i.MReceiptNo).Select(c => new MasterDetails
                             {
                                 EmpName = c.First().MCustomer,
                                 //TimeOut = c.First().r.TimeOut,
                                 DateIn = c.First().DateIn,
                                 DateOut = c.First().DateOut,
                                 ReceiptNo = c.First().MReceiptNo,
                                 DisInvoice = c.First().DiscountTotal,
                                 TotalTax = c.First().TaxValue,
                                 Currency = c.First().PLCurrency,
                                 GrandTotal = c.First().SGrandTotalSys,
                                 DetailItems = c.Select(x => new DetailItem
                                 {
                                     Code = x.ItemCode,
                                     ItemName = x.KhmerName,
                                     Qty = x.Qty,
                                     UoM = g.First().PLCurrency,
                                     SalePrice = x.UnitPriceCal,
                                     DisItem = x.DiscountItem,
                                     Total = x.TotalCal,
                                     //PLC = c.First().curr_lc.Description,
                                 }).ToList(),
                                 SBCHeader = new SBCHeader
                                 {
                                     Logo = GetCompany().Logo,
                                     DateFrom = c.First().DateFrom,
                                     DateTo = c.First().DateTo,
                                     Branch = "",
                                     CusName = ""
                                 },
                                 Footer = new Footer
                                 {
                                     CountReceipt = data.SCount,
                                     DiscountItem = data.SDiscountItem,
                                     DiscountTotal = data.SDiscountTotal,
                                     TaxValue = data.SVat,
                                     GrandTotalSys = data.SGrandTotalSys,
                                     GrandTotal = data.SGrandTotal,
                                 }
                             }).ToList()
                         }).ToList();
            if (_data.Any())
            {
                return new ViewAsPdf(_data)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            return Ok();
        }

        //Print Tax Declaration
        public IActionResult PrintTaxDeclaration(string DateFrom, string DateTo, int BranchID, int UserID)
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
                var Branch = "All";
                var EmpName = "All";
                var Logo = "";
                if (BranchID != 0)
                {
                    Branch = _context.Branches.FirstOrDefault(w => w.ID == BranchID).Name;
                    Logo = _context.Branches.Include(c => c.Company).FirstOrDefault(w => w.ID == BranchID).Company.Logo;
                }
                if (UserID != 0)
                {
                    EmpName = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == UserID).Employee.Name;
                }
                var Receipts = receiptsFilter;
                var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().LocalCurrencyID) ?? new Display();
                var Sale = from r in Receipts
                           join user in _context.UserAccounts on r.UserOrderID equals user.ID
                           join emp in _context.Employees on user.EmployeeID equals emp.ID
                           join curr in _context.Currency on r.LocalCurrencyID equals curr.ID
                           join ssc in _context.Currency on r.SysCurrencyID equals ssc.ID
                           join lc in _context.Currency on r.LocalCurrencyID equals lc.ID
                           let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == ssc.ID) ?? new Display()
                           select new GroupTaxDeclaration
                           {
                               EmpCode = emp.Code,
                               EmpName = emp.Name,
                               User = emp.Name,
                               Receipt = r.ReceiptNo,
                               DateIn = r.DateIn.ToString("dd-MM-yyyy"),
                               TimeIn = r.TimeIn,
                               DateOut = r.DateOut.ToString("dd-MM-yyyy"),
                               TimeOut = r.TimeOut,
                               GrandTotal = r.GrandTotal_Sys * r.LocalSetRate,
                               Tax = r.TaxValue * r.PLRate * r.LocalSetRate,
                               LocalCurrency = curr.Description,
                               Header = new Header
                               {
                                   Logo = Logo,
                                   DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                   DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                   Branch = Branch,
                                   EmpName = EmpName
                               },
                               Footer = new Footer
                               {
                                   CountReceipt = string.Format("{0:#,0}", Summary.FirstOrDefault().CountReceipt),
                                   DiscountItem = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountItem, sysCur.Amounts),
                                   DiscountTotal = _fncModule.ToCurrency(Summary.FirstOrDefault().DiscountTotal, sysCur.Amounts),
                                   TaxValue = ssc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().TaxValue, sysCur.Amounts),
                                   GrandTotalSys = ssc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotalSys, sysCur.Amounts),
                                   GrandTotal = lc.Description + " " + _fncModule.ToCurrency(Summary.FirstOrDefault().GrandTotal, lcCur.Amounts),
                               }
                           };
                if (Sale.Any())
                {
                    return new ViewAsPdf(Sale.ToList())
                    {
                        CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                    };
                }
            }
            return Ok();
        }

        //PrintStockWarehouse
        #region
        //public IActionResult PrintStockinWareHouseinPdf(int BranchID, int WarehouseID, bool Inactive)
        //{
        //    if (BranchID != 0 && WarehouseID != 0)
        //    {
        //        var list = (from whs in _context.WarehouseSummary
        //                    join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
        //                    join uom in _context.UnitofMeasures on whs.UomID equals uom.ID
        //                    join wh in _context.Warehouses on whs.WarehouseID equals wh.ID
        //                    join B in _context.Branches on wh.BranchID equals B.ID
        //                    join R in _context.ReceiptInformation on B.ID equals R.BranchID
        //                    join C in _context.Company on B.CompanyID equals C.ID
        //                    where whs.WarehouseID == WarehouseID && item.Delete == Inactive && item.Inventory == true && item.Purchase == true
        //                    orderby item.Code

        //                    select new PrintStockinWarehouse
        //                    {
        //                        AddressEng = R.EnglishDescription,
        //                        //Brand = cp == null ? "" : cp.Name,
        //                        Logo = C.Logo,
        //                        Addresskh = R.Address,
        //                        ID = item.ID,
        //                        Image = item.Image,
        //                        ItemCode = item.Code,
        //                        ItemBarcode = item.Barcode,
        //                        itemName = item.KhmerName,
        //                        EngName = item.EnglishName,
        //                        Price = item.UnitPrice,
        //                        Uom = uom.Name,
        //                        WhCode = wh.Code,
        //                        StockIn = whs.InStock,
        //                        Orderedin = whs.Ordered,
        //                        CommittedName = whs.Committed,
        //                    }).ToList() ?? new List<PrintStockinWarehouse>();
        //        return new ViewAsPdf(list)
        //        {
        //            CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
        //        };
        //    }
        //    else if (BranchID != 0 && WarehouseID == 0)
        //    {
        //        var list = (from whs in _context.WarehouseSummary
        //                    join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
        //                    join uom in _context.UnitofMeasures on whs.UomID equals uom.ID
        //                    join wh in _context.Warehouses on whs.WarehouseID equals wh.ID
        //                    join B in _context.Branches on wh.BranchID equals B.ID
        //                    join R in _context.ReceiptInformation on B.ID equals R.BranchID
        //                    join C in _context.Company on B.CompanyID equals C.ID
        //                    where item.Delete == Inactive && item.Inventory == true && item.Purchase == true && item.Type != "Standard"
        //                    orderby item.Code

        //                    select new PrintStockinWarehouse
        //                    {
        //                        AddressEng = R.EnglishDescription,
        //                        Logo = C.Logo,
        //                        Addresskh = R.Address,
        //                        ID = item.ID,
        //                        Image = item.Image,
        //                        ItemCode = item.Code,
        //                        ItemBarcode = item.Barcode,
        //                        itemName = item.KhmerName,
        //                        EngName = item.EnglishName,
        //                        Price = item.UnitPrice,
        //                        Uom = uom.Name,
        //                        WhCode = wh.Code,
        //                        StockIn = whs.InStock,
        //                        Orderedin = whs.Ordered,
        //                        CommittedName = whs.Committed,
        //                    }).ToList() ?? new List<PrintStockinWarehouse>();
        //        return new ViewAsPdf(list)
        //        {
        //            CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
        //        };
        //    }
        //    return View();
        //}
        #endregion

        public IActionResult PrintStockinWareHouseinPdf(int BranchID, int WarehouseID, bool Inactive)
        {
            var brand = _context.Property.FirstOrDefault(i => i.Name == "Brand" && i.Active && i.CompanyID == GetCompany().ID) ?? new Property();
            if (BranchID != 0 && WarehouseID != 0)
            {
                var list = (from whs in _context.WarehouseSummary
                            join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                            join uom in _context.UnitofMeasures on whs.UomID equals uom.ID
                            join wh in _context.Warehouses on whs.WarehouseID equals wh.ID
                            join B in _context.Branches on wh.BranchID equals B.ID
                            join R in _context.ReceiptInformation on B.ID equals R.BranchID
                            join C in _context.Company on B.CompanyID equals C.ID
                            where whs.WarehouseID == WarehouseID && item.Delete == Inactive && item.Inventory == true && item.Purchase == true
                            orderby item.Code

                            let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == item.ID)
                            let pdId = pd == null ? 0 : pd.Value
                            let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                            select new PrintStockinWarehouse
                            {
                                AddressEng = R.EnglishDescription,
                                Brand = cp == null ? "" : cp.Name,
                                Logo = C.Logo,
                                Addresskh = R.Address,
                                ID = item.ID,
                                Image = item.Image,
                                ItemCode = item.Code,
                                ItemBarcode = item.Barcode,
                                itemName = item.KhmerName,
                                EngName = item.EnglishName,
                                Price = item.UnitPrice,
                                Uom = uom.Name,
                                WhCode = wh.Code,
                                StockIn = whs.InStock,
                                Orderedin = whs.Ordered,
                                CommittedName = whs.Committed,
                            }).ToList() ?? new List<PrintStockinWarehouse>();
                if (list.Count() == 0) return NotFound();
                return new ViewAsPdf(list)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
                //return View(list);
            }
            else if (BranchID != 0 && WarehouseID == 0)
            {
                var list = (from whs in _context.WarehouseSummary
                            join item in _context.ItemMasterDatas on whs.ItemID equals item.ID
                            join uom in _context.UnitofMeasures on whs.UomID equals uom.ID
                            join wh in _context.Warehouses on whs.WarehouseID equals wh.ID
                            join B in _context.Branches on wh.BranchID equals B.ID
                            join R in _context.ReceiptInformation on B.ID equals R.BranchID
                            join C in _context.Company on B.CompanyID equals C.ID
                            where item.Delete == Inactive && item.Inventory == true && item.Purchase == true && item.Type != "Standard"
                            orderby item.Code

                            let pd = _context.PropertyDetails.FirstOrDefault(i => i.ProID == brand.ID && i.ItemID == item.ID)
                            let pdId = pd == null ? 0 : pd.Value
                            let cp = _context.ChildPreoperties.FirstOrDefault(i => i.ID == pdId)
                            select new PrintStockinWarehouse
                            {
                                AddressEng = R.EnglishDescription,
                                Brand = cp == null ? "" : cp.Name,
                                Logo = C.Logo,
                                Addresskh = R.Address,
                                ID = item.ID,
                                Image = item.Image,
                                ItemCode = item.Code,
                                ItemBarcode = item.Barcode,
                                itemName = item.KhmerName,
                                EngName = item.EnglishName,
                                Price = item.UnitPrice,
                                Uom = uom.Name,
                                WhCode = wh.Code,
                                StockIn = whs.InStock,
                                Orderedin = whs.Ordered,
                                CommittedName = whs.Committed,
                            }).ToList() ?? new List<PrintStockinWarehouse>();
                return new ViewAsPdf(list)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
                //return View(list);
            }
            return View();
        }

        //Inventory Print
        public IActionResult PrintTransferStock(string DateFrom, string DateTo, int FromBranchID, int ToBranchID, int FromWHID, int ToWHID, int UserID)
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

            var Logo = "";
            var FromBranch = "All";
            var ToBranch = "All";
            var FromWH = "All";
            var ToWH = "All";
            var EmpName = "All";

            if (FromBranchID != 0 && ToBranchID != 0)
            {
                FromBranch = _context.Branches.FirstOrDefault(w => w.ID == FromBranchID).Name;
                ToBranch = _context.Branches.FirstOrDefault(w => w.ID == ToBranchID).Name;
                Logo = _context.Branches.Include(c => c.Company).FirstOrDefault(w => w.ID == FromBranchID).Company.Logo;
            }
            if (FromWHID != 0 && ToWHID != 0)
            {
                FromWH = _context.Warehouses.FirstOrDefault(x => x.ID == FromWHID).Name;
                ToWH = _context.Warehouses.FirstOrDefault(x => x.ID == ToWHID).Name;
            }
            if (UserID != 0)
            {
                EmpName = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == UserID).Employee.Name;
            }
            var Transfers = goodsFilter;
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
            var list = from t in Transfers
                       join td in _context.TransferDetails on t.TarmsferID equals td.TransferID
                       join Uom in _context.UnitofMeasures on td.UomID equals Uom.ID
                       group new { t, td, Uom } by t.Number into g
                       select new GroupTransferStock
                       {
                           Number = g.FirstOrDefault().t.Number,
                           PosDate = Convert.ToDateTime(g.FirstOrDefault().t.PostingDate).ToString("dd-MM-yyyy"),
                           DocDate = Convert.ToDateTime(g.FirstOrDefault().t.DocumentDate).ToString("dd-MM-yyyy"),
                           Time = Convert.ToDateTime(g.FirstOrDefault().t.Time).ToString("hh:mm tt"),
                           Subtotal = _fncModule.ToCurrency(g.Sum(x => x.td.Cost * x.td.Qty), lcCur.Amounts),
                           Goods = g.Select(good => new Goods
                           {
                               NumberNo = good.t.Number,
                               Barcode = good.td.Barcode,
                               Code = good.td.Code,
                               KhName = good.td.KhmerName,
                               EngName = good.td.EnglishName,
                               Qty = string.Format("{0:#,0}", good.td.Qty),
                               Cost = string.Format("{0:#,0}", good.td.Cost),
                               Uom = good.Uom.Name,
                               ExpireDate = Convert.ToDateTime(good.td.ExpireDate).ToString("dd-MM-yyyy"),
                           }).ToList(),
                           TFHeader = new TFHeader
                           {
                               Logo = Logo,
                               BranchFrom = FromBranch,
                               BranchTo = ToBranch,
                               WHFrom = FromWH,
                               WHTo = ToWH,
                               EmpName = EmpName,
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy")
                           },
                           GRFooter = new GRFooter
                           {
                               Currency = g.First().td.Currency,
                               SumGrandTotal = g.Sum(x => x.td.Cost * x.td.Qty)
                           }
                       };

            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }

        public IActionResult PrintGoodsReceiptStock(string DateFrom, string DateTo, int BranchID, int WHID, int UserID)
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

            var Logo = "";
            var Branch = "All";
            var EmpName = "All";
            var WHName = "All";
            if (BranchID != 0)
            {
                Branch = _context.Branches.FirstOrDefault(w => w.ID == BranchID).Name;
                Logo = _context.Branches.Include(c => c.Company).FirstOrDefault(w => w.ID == BranchID).Company.Logo;
            }
            if (WHID != 0)
            {
                WHName = _context.Warehouses.FirstOrDefault(x => x.ID == WHID).Name;
            }
            if (UserID != 0)
            {
                EmpName = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == UserID).Employee.Name;
            }

            var GoodReceipts = goodsFilter;
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
            var list = from gr in GoodReceipts
                       join grd in _context.GoodReceiptDetails on gr.GoodsReceiptID equals grd.GoodsReceiptID
                       join Uom in _context.UnitofMeasures on grd.UomID equals Uom.ID
                       group new { gr, grd, Uom } by gr.Number_No into g
                       select new GroupGoodsReceiptStock
                       {
                           NumberNo = g.FirstOrDefault().gr.Number_No,
                           PosDate = Convert.ToDateTime(g.FirstOrDefault().gr.PostingDate).ToString("dd-MM-yyyy"),
                           DocDate = Convert.ToDateTime(g.FirstOrDefault().gr.DocumentDate).ToString("dd-MM-yyyy"),
                           Subtotal = _fncModule.ToCurrency(g.Sum(x => x.grd.Cost * x.grd.Quantity), lcCur.Amounts),
                           Goods = g.Select(good => new Goods
                           {
                               Barcode = good.grd.BarCode,
                               Code = good.grd.Code,
                               KhName = good.grd.KhmerName,
                               EngName = good.grd.EnglishName,
                               Qty = string.Format("{0:#,0}", good.grd.Quantity),
                               Cost = string.Format("{0:#,0}", good.grd.Cost),
                               Uom = good.Uom.Name,
                               ExpireDate = Convert.ToDateTime(good.grd.ExpireDate).ToString("dd-MM-yyyy"),
                           }).ToList(),
                           GRHeader = new GRHeader
                           {
                               Logo = Logo,
                               Branch = Branch,
                               EmpName = EmpName,
                               WareHouse = WHName,
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy")
                           },
                           GRFooter = new GRFooter
                           {
                               Currency = g.First().grd.Currency,
                               SumGrandTotal = g.Sum(x => x.grd.Cost * x.grd.Quantity)
                           }
                       };

            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }

        public IActionResult PrintGoodsIssueStock(string DateFrom, string DateTo, int BranchID, int WHID, int UserID)
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

            var Logo = "";
            var Branch = "All";
            var EmpName = "All";
            var WHName = "All";
            if (BranchID != 0)
            {
                Branch = _context.Branches.FirstOrDefault(w => w.ID == BranchID).Name;
                Logo = _context.Branches.Include(c => c.Company).FirstOrDefault(w => w.ID == BranchID).Company.Logo;
            }
            if (WHID != 0)
            {
                WHName = _context.Warehouses.FirstOrDefault(x => x.ID == WHID).Name;
            }
            if (UserID != 0)
            {
                EmpName = _context.UserAccounts.Include(emp => emp.Employee).FirstOrDefault(w => w.ID == UserID).Employee.Name;
            }

            var GoodIssues = goodsFilter;
            var lcCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == GetCompany().SystemCurrencyID) ?? new Display();
            var list = from gi in GoodIssues
                       join gid in _context.GoodIssuesDetails on gi.GoodIssuesID equals gid.GoodIssuesID
                       join Uom in _context.UnitofMeasures on gid.UomID equals Uom.ID
                       //join cur in _context.Currency on grd.CurrencyID equals cur.ID
                       group new { gi, gid, Uom } by gi.Number_No into g
                       select new GroupGoodsIssueStock
                       {
                           NumberNo = g.FirstOrDefault().gi.Number_No,
                           PosDate = Convert.ToDateTime(g.FirstOrDefault().gi.PostingDate).ToString("dd-MM-yyyy"),
                           DocDate = Convert.ToDateTime(g.FirstOrDefault().gi.DocumentDate).ToString("dd-MM-yyyy"),
                           Subtotal = _fncModule.ToCurrency(g.Sum(x => x.gid.Cost * x.gid.Quantity), lcCur.Amounts),
                           Goods = g.Select(good => new Goods
                           {
                               Barcode = good.gid.BarCode,
                               Code = good.gid.Code,
                               KhName = good.gid.KhmerName,
                               EngName = good.gid.EnglishName,
                               Qty = string.Format("{0:#,0}", good.gid.Quantity),
                               Cost = string.Format("{0:#,0}", good.gid.Cost),
                               Uom = good.Uom.Name,
                               ExpireDate = Convert.ToDateTime(good.gid.ExpireDate).ToString("dd-MM-yyyy"),
                           }).ToList(),
                           GRHeader = new GRHeader
                           {
                               Logo = Logo,
                               Branch = Branch,
                               EmpName = EmpName,
                               WareHouse = WHName,
                               DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                               DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy")
                           },
                           GRFooter = new GRFooter
                           {
                               Currency = g.First().gid.Currency,
                               SumGrandTotal = g.Sum(x => x.gid.Cost * x.gid.Quantity)
                           }
                       };

            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
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

        public IEnumerable<CashoutReport> GetCashoutReport(double Tran_F, double Tran_T, int UserID) => _context.CashoutReport.FromSql("rp_GetCashoutReport @Tran_F={0},@Tran_T={1},@UserID={2}",
          parameters: new[] {
                Tran_F.ToString(),
                Tran_T.ToString(),
                UserID.ToString()
          }).ToList();

        //Print Bill of Material
        public IActionResult PrintBillofMaterial()
        {
            var list = from bom in _context.BOMaterial
                       join bomd in _context.BOMDetail.Where(w => w.Detele == false) on bom.BID equals bomd.BID
                       join id in _context.ItemMasterDatas on bomd.ItemID equals id.ID
                       join i in _context.ItemMasterDatas on bom.ItemID equals i.ID
                       join u in _context.UnitofMeasures on bomd.UomID equals u.ID
                       join guom in _context.GroupUOMs on bom.UomID equals guom.ID
                       join syscy in _context.Currency on bom.SysCID equals syscy.ID
                       group new { bom, bomd, id, i, u, guom, syscy } by bom.BID into g
                       select new BomReport

                       {
                           //master
                           BID = g.Key,
                           ID = g.First().bomd.BDID,
                           KhmerName = g.First().i.KhmerName,
                           Uom = g.First().guom.Name,
                           PostingDate = g.First().bom.PostingDate.ToString("dd-MM-yyyy"),
                           TotalCost = g.First().bom.TotalCost,
                           SysCy = g.First().syscy.Description,
                           BomDetails = g
                            .Select(x => new BomDetail
                            {
                                //Detail
                                ID = x.bomd.BID,
                                Qty = x.bomd.Qty,
                                UomD = x.u.Name,
                                Cost = x.bomd.Cost,
                                Amount = x.bomd.Amount,
                                ItemName = x.id.KhmerName,
                            }).ToList(),
                       };

            if (list == null) return NotFound();
            return new ViewAsPdf(list)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }

        /// Cash Flow For Treasury ///
        public IActionResult CashFlowTreasury(string fromDate, string toDate)
        {
            decimal balance = 0;
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            var currName = _context.Currency.Find(GetCompany().SystemCurrencyID);
            var data = (from doc in _context.DocumentTypes.Where(i => i.Code == "RC" || i.Code == "PS" || i.Code == "SP" || i.Code == "RP")
                        join ab in _context.AccountBalances
                        .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate) on doc.ID equals ab.Origin
                        join gla in _context.GLAccounts
                        .Where(i => i.CashFlowRelavant && i.IsCashAccount && i.CompanyID == GetCompany().ID) on ab.GLAID equals gla.ID
                        join series in _context.Series on doc.ID equals series.DocuTypeID
                        //join seriesd in _context.SeriesDetails on series.ID equals seriesd.SeriesID
                        select new CashFlowForTreasuryViewModel
                        {
                            ID = doc.ID,
                            Credit = (ab.Credit == 0) ? "" : string.Format("{0:#,0}", ab.Credit),
                            Debit = (ab.Debit == 0) ? "" : string.Format("{0:#,0}", ab.Debit),
                            ControlAccount = gla.Code,
                            Origin = doc.Code,
                            Remarks = ab.Details,
                            Referrence = ab.OriginNo,
                            GLAccBPCode = gla.Code,
                            Total = currName.Description + " " + string.Format("{0:#,0}", ab.Debit - ab.Credit),
                            DueDate = ab.PostingDate.ToString("MM/dd/yyyy"),
                        }).ToList();
            data.ForEach(i =>
            {
                balance += Convert.ToDecimal(i.Total.Split(" ")[1]);
                i.Balance = currName.Description + " " + string.Format("{0:#,0}", balance);
                i.CreditTotal = currName.Description + " " + string.Format("{0:#,0}", data.Sum(s => Convert.ToDecimal(s.Credit == "" ? "0" : s.Credit)));
                i.DebitTotal = currName.Description + " " + string.Format("{0:#,0}", data.Sum(s => Convert.ToDecimal(s.Debit == "" ? "0" : s.Debit)));
                i.TotalSummary = currName.Description + " " + string.Format("{0:#,0}", Convert.ToDecimal(i.DebitTotal.Split(" ")[1]) - Convert.ToDecimal(i.CreditTotal.Split(" ")[1]));
                i.BalanceTotal = currName.Description + " " + string.Format("{0:#,0}", Convert.ToDecimal(i.DebitTotal.Split(" ")[1]) - Convert.ToDecimal(i.CreditTotal.Split(" ")[1]));
            });
            var result = data.GroupBy(i => i.DueDate).ToList();
            var cashflow = new PirntCashFlowForTreasuryViewModel
            {
                CashFlowForTreasuryViewModels = result,
                HeaderCashflow = new HeaderCashFlow
                {
                    FromDate = fromDate,
                    ToDate = toDate
                },
                Summary = new SummaryCashFlow
                {
                    CreditTotal = data.FirstOrDefault().CreditTotal,
                    DebitTotal = data.FirstOrDefault().DebitTotal,
                    TotalSummary = data.FirstOrDefault().TotalSummary,
                    BalanceTotal = data.FirstOrDefault().BalanceTotal,
                }
            };
            return new ViewAsPdf(cashflow)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }

        /// Sale Gross Profit ///
        public async Task<IActionResult> SaleGrossProfit(string fromDate, string toDate, string timeFrom, string timeTo, int userId)
        {
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            List<Receipt> receipts = new();
            List<SaleAR> sales = new();
            List<ReceiptMemo> receiptsCredit = new();
            List<SaleCreditMemo> salesCredit = new();
            var cur = _context.Currency.Find(GetCompany().SystemCurrencyID) ?? new Currency();
            if ((toDate == null || fromDate == null) && userId == 0)
            {
                receipts = await _context.Receipt.ToListAsync();
                sales = await _context.SaleARs.ToListAsync();
                receiptsCredit = await _context.ReceiptMemo.ToListAsync();
                salesCredit = await _context.SaleCreditMemos.ToListAsync();
            }
            else if ((toDate == null || fromDate == null) && userId != 0)
            {
                receipts = await _context.Receipt.Where(i => i.UserOrderID == userId).ToListAsync();
                sales = await _context.SaleARs.Where(i => i.UserID == userId).ToListAsync();
                receiptsCredit = await _context.ReceiptMemo.Where(i => i.UserOrderID == userId).ToListAsync();
                salesCredit = await _context.SaleCreditMemos.Where(i => i.UserID == userId).ToListAsync();
            }
            else if (toDate != null && fromDate != null && userId == 0 && timeFrom == null && timeTo == null)
            {
                receipts = await _context.Receipt
                    .Where(i => i.DateIn >= _fromDate && i.DateIn <= _toDate)
                    .ToListAsync();
                sales = await _context.SaleARs
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
                receiptsCredit = await _context.ReceiptMemo
                    .Where(i => i.DateIn >= _fromDate && i.DateIn <= _toDate)
                    .ToListAsync();
                salesCredit = await _context.SaleCreditMemos
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
            }
            else if (toDate != null && fromDate != null && userId != 0 && timeFrom == null && timeTo == null)
            {
                receipts = await _context.Receipt
                    .Where(i => i.DateIn >= _fromDate && i.DateIn <= _toDate && i.UserOrderID == userId)
                    .ToListAsync();
                sales = await _context.SaleARs
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate && i.UserID == userId)
                    .ToListAsync();
                receiptsCredit = await _context.ReceiptMemo
                    .Where(i => i.DateIn >= _fromDate && i.DateIn <= _toDate && i.UserOrderID == userId)
                    .ToListAsync();
                salesCredit = await _context.SaleCreditMemos
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate && i.UserID == userId)
                    .ToListAsync();
            }
            else if (toDate != null && fromDate != null && userId == 0 && timeFrom != null && timeTo != null)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", fromDate, timeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", toDate, timeTo));
                receipts = await _context.Receipt
                    .Where(i => Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) >= dateFrom && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) <= dateTo)
                    .ToListAsync();
                sales = await _context.SaleARs
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
                receiptsCredit = await _context.ReceiptMemo
                    .Where(i => Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) >= dateFrom && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) <= dateTo)
                    .ToListAsync();
                salesCredit = await _context.SaleCreditMemos
                    .Where(i => i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
            }
            else if (toDate != null && fromDate != null && userId > 0 && timeFrom != null && timeTo != null)
            {
                DateTime dateFrom = DateTime.Parse(string.Format("{0} {1}", fromDate, timeFrom));
                DateTime dateTo = DateTime.Parse(string.Format("{0} {1}", toDate, timeTo));
                receipts = await _context.Receipt
                    .Where(i => i.UserOrderID == userId && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) >= dateFrom && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) <= dateTo)
                    .ToListAsync();
                sales = await _context.SaleARs
                    .Where(i => i.UserID == userId && i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
                receiptsCredit = await _context.ReceiptMemo
                    .Where(i => i.UserOrderID == userId && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) >= dateFrom && Convert.ToDateTime(i.DateIn.ToString("MM-dd-yyyy") + " " + i.TimeIn) <= dateTo)
                    .ToListAsync();
                salesCredit = await _context.SaleCreditMemos
                    .Where(i => i.UserID == userId && i.PostingDate >= _fromDate && i.PostingDate <= _toDate)
                    .ToListAsync();
            }

            #region receipts data
            var receiptsData = (from rp in receipts
                                join rpd in _context.ReceiptDetail on rp.ReceiptID equals rpd.ReceiptID
                                join series in _context.Series on rp.SeriesID equals series.ID
                                join audi in _context.InventoryAudits.Where(i => i.Qty < 0) on rp.SeriesDID equals audi.SeriesDetailID
                                join item in _context.ItemMasterDatas on audi.ItemID equals item.ID
                                join uom in _context.UnitofMeasures on audi.UomID equals uom.ID
                                group new { rp, rpd, item, series, uom, audi } by new { rp.ReceiptID, rpd.ItemID, audi.Cost, rpd.UnitPrice } into g
                                let data = g.DefaultIfEmpty().Last()
                                let qty = data.audi.Qty * -1
                                let cost = data.audi.Cost * data.rp.ExchangeRate
                                let unitprice = data.rpd.UnitPrice * data.rp.ExchangeRate
                                let total = (unitprice - cost) * qty
                                let totalCost = cost * qty
                                let totalPrice = unitprice * qty
                                let disValue = (data.rpd.DiscountRate + data.rp.DiscountRate) * total / 100
                                let totalAfterDis = totalPrice - disValue
                                let grossprofit = total - disValue
                                let grossprofitPercentage = grossprofit * 100 / totalPrice
                                select new SaleGrossProfitReport
                                {
                                    PostingDate = data.rp.DateIn.ToShortDateString(),
                                    DateOut = data.rp.DateOut.ToShortDateString(),
                                    //Code = data.item.Code,
                                    Code = data.rpd.Code,
                                    Cost = (decimal)cost,
                                    InvoiceNo = $"{data.series.Name}-{data.rp.ReceiptNo}",
                                    ItemID = data.item.ID,
                                    //ItemName = data.item.KhmerName,
                                    ItemName = data.rpd.KhmerName,
                                    ManItemBy = data.item.ManItemBy,
                                    Price = (decimal)unitprice,
                                    Qty = (decimal)qty,
                                    Total = (decimal)total,
                                    TotalGrossProfit = 0,
                                    TotalItem = 0,
                                    UoMName = data.uom.Name,
                                    Discount = (decimal)disValue,
                                    GrossProfit = (decimal)grossprofit,
                                    TotalPrice = (decimal)totalPrice,
                                    TotalCost = (decimal)totalCost,
                                    DisRateM = data.rp.DiscountRate,
                                    DisRate = data.rpd.DiscountRate,
                                    TotalAfterDis = (decimal)totalAfterDis == 0 ? (decimal)totalPrice : (decimal)totalAfterDis,
                                    GrossProfitP = (decimal)grossprofitPercentage,
                                    TimeIn = data.rp.TimeIn,
                                    TimeOut = data.rp.TimeOut,
                                }).ToList();
            #endregion

            #region receipts data credit
            var receiptsDataCredit = (from rp in receiptsCredit
                                      join rpd in _context.ReceiptDetailMemoKvms on rp.ID equals rpd.ReceiptMemoID
                                      join series in _context.Series on rp.SeriesID equals series.ID
                                      join audi in _context.InventoryAudits.Where(i => i.Qty > 0) on rp.SeriesDID equals audi.SeriesDetailID
                                      join item in _context.ItemMasterDatas on audi.ItemID equals item.ID

                                      join uom in _context.UnitofMeasures on audi.UomID equals uom.ID
                                      group new { rp, rpd, item, series, uom, audi } by new { RPID = rp.ID, rpd.ItemID, audi.Cost, rpd.UnitPrice } into g
                                      let data = g.DefaultIfEmpty().Last()
                                      let qty = data.audi.Qty
                                      let cost = data.audi.Cost * data.rp.ExchangeRate
                                      let unitprice = data.rpd.UnitPrice * data.rp.ExchangeRate
                                      let total = (unitprice - cost) * qty
                                      let totalCost = cost * qty
                                      let totalPrice = unitprice * qty
                                      let disValue = (data.rpd.DisRate + data.rp.DisRate) * total / 100
                                      let totalAfterDis = totalPrice - disValue
                                      let grossprofit = total - disValue
                                      let grossprofitPercentage = grossprofit * 100 / totalPrice
                                      select new SaleGrossProfitReport
                                      {
                                          PostingDate = data.rp.DateIn.ToShortDateString(),
                                          DateOut = data.rp.DateOut.ToShortDateString(),
                                          //Code = data.item.Code,
                                          Code = data.rpd.Code,
                                          Cost = (decimal)cost * -1,
                                          InvoiceNo = $"{data.series.Name}-{data.rp.ReceiptNo}",
                                          ItemID = data.item.ID,
                                          //ItemName = data.item.KhmerName,
                                          ItemName = data.rpd.KhmerName,
                                          ManItemBy = data.item.ManItemBy,
                                          Price = (decimal)unitprice * -1,
                                          Qty = (decimal)qty,
                                          Total = (decimal)total * -1,
                                          TotalGrossProfit = 0,
                                          TotalItem = 0,
                                          UoMName = data.uom.Name,
                                          Discount = (decimal)disValue * -1,
                                          GrossProfit = (decimal)grossprofit * -1,
                                          TotalPrice = (decimal)totalPrice * -1,
                                          TotalCost = (decimal)totalCost * -1,
                                          DisRateM = data.rp.DisRate * -1,
                                          DisRate = data.rpd.DisRate * -1,
                                          TotalAfterDis = (decimal)totalAfterDis == 0 ? (decimal)totalPrice * -1 : (decimal)totalAfterDis * -1,
                                          GrossProfitP = (decimal)grossprofitPercentage * -1,
                                          TimeIn = data.rp.TimeIn,
                                          TimeOut = data.rp.TimeOut,
                                      }).ToList();
            #endregion

            #region sale data
            var saleData = (from ar in sales
                            join ard in _context.SaleARDetails on ar.SARID equals ard.SARID
                            join series in _context.Series on ar.SeriesID equals series.ID
                            join audi in _context.InventoryAudits.Where(i => i.Qty < 0) on ar.SeriesDID equals audi.SeriesDetailID
                            join item in _context.ItemMasterDatas on audi.ItemID equals item.ID
                            join uom in _context.UnitofMeasures on audi.UomID equals uom.ID
                            group new { ar, ard, item, series, uom, audi } by new { ar.SARID, ard.ItemID, audi.Cost, ard.UnitPrice } into g
                            let data = g.DefaultIfEmpty().Last()
                            let qty = data.audi.Qty * -1
                            let cost = data.audi.Cost * data.ar.ExchangeRate
                            let unitprice = data.ard.UnitPrice * data.ar.ExchangeRate
                            let total = (unitprice - cost) * qty
                            let totalCost = cost * qty
                            let totalPrice = unitprice * qty
                            let disValue = (data.ard.DisRate + data.ar.DisRate) * total / 100
                            let grossprofit = total - disValue
                            let totalAfterDis = totalPrice - disValue
                            let grossprofitPercentage = grossprofit * 100 / totalPrice
                            select new SaleGrossProfitReport
                            {
                                // Code = data.item.Code,
                                Code = data.ard.ItemCode,
                                Cost = (decimal)cost,
                                InvoiceNo = $"{data.series.Name}-{data.ar.InvoiceNumber}",
                                ItemID = data.item.ID,
                                // ItemName = data.item.KhmerName,
                                ItemName = data.ard.ItemNameKH,
                                ManItemBy = data.item.ManItemBy,
                                PostingDate = data.ar.PostingDate.ToShortDateString(),
                                Price = (decimal)unitprice,
                                Qty = (decimal)qty,
                                Total = (decimal)total,
                                TotalGrossProfit = 0,
                                TotalItem = 0,
                                UoMName = data.uom.Name,
                                Discount = (decimal)disValue,
                                GrossProfit = (decimal)grossprofit,
                                TotalAfterDis = (decimal)totalAfterDis == 0 ? (decimal)totalPrice : (decimal)totalAfterDis,
                                TotalCost = (decimal)totalCost,
                                TotalPrice = (decimal)totalPrice,
                                DisRateM = data.ar.DisRate,
                                DisRate = data.ard.DisRate,
                                GrossProfitP = (decimal)grossprofitPercentage,
                            }).ToList();
            #endregion

            #region sale data credit
            var saleDataCredit = (from ar in salesCredit
                                  join ard in _context.SaleCreditMemoDetails on ar.SCMOID equals ard.SCMOID
                                  join series in _context.Series on ar.SeriesID equals series.ID
                                  join audi in _context.InventoryAudits.Where(i => i.Qty > 0) on ar.SeriesDID equals audi.SeriesDetailID
                                  join item in _context.ItemMasterDatas on audi.ItemID equals item.ID
                                  join uom in _context.UnitofMeasures on audi.UomID equals uom.ID
                                  group new { ar, ard, item, series, uom, audi } by new { ar.SCMOID, ard.ItemID, audi.Cost, ard.UnitPrice } into g
                                  let data = g.DefaultIfEmpty().Last()
                                  let qty = data.audi.Qty
                                  let cost = data.audi.Cost * data.ar.ExchangeRate
                                  let unitprice = data.ard.UnitPrice * data.ar.ExchangeRate
                                  let total = (unitprice - cost) * qty
                                  let totalCost = cost * qty
                                  let totalPrice = unitprice * qty
                                  let disValue = (data.ard.DisRate + data.ar.DisRate) * total / 100
                                  let grossprofit = total - disValue
                                  let totalAfterDis = totalPrice - disValue
                                  let grossprofitPercentage = grossprofit * 100 / totalPrice
                                  select new SaleGrossProfitReport
                                  {
                                      // Code = data.item.Code,
                                      Code = data.ard.ItemCode,
                                      Cost = (decimal)cost * -1,
                                      InvoiceNo = $"{data.series.Name}-{data.ar.InvoiceNumber}",
                                      ItemID = data.item.ID,
                                      // ItemName = data.item.KhmerName,
                                      ItemName = data.ard.ItemNameKH,
                                      ManItemBy = data.item.ManItemBy,
                                      PostingDate = data.ar.PostingDate.ToShortDateString(),
                                      Price = (decimal)unitprice * -1,
                                      Qty = (decimal)qty,
                                      Total = (decimal)total,
                                      TotalGrossProfit = 0,
                                      TotalItem = 0,
                                      UoMName = data.uom.Name,
                                      Discount = (decimal)disValue * -1,
                                      GrossProfit = (decimal)grossprofit * -1,
                                      TotalAfterDis = (decimal)totalAfterDis == 0 ? (decimal)totalPrice * -1 : (decimal)totalAfterDis * -1,
                                      TotalCost = (decimal)totalCost * -1,
                                      TotalPrice = (decimal)totalPrice * -1,
                                      DisRateM = data.ar.DisRate * -1,
                                      DisRate = data.ard.DisRate * -1,
                                      GrossProfitP = (decimal)grossprofitPercentage * -1,
                                  }).ToList();
            #endregion

            List<SaleGrossProfitReport> all = new(receiptsData.Count + saleData.Count + receiptsDataCredit.Count + saleDataCredit.Count);
            all.AddRange(receiptsData);
            all.AddRange(saleData);
            all.AddRange(receiptsDataCredit);
            all.AddRange(saleDataCredit);
            var sumGroupData = (from _all in all
                                group _all by _all.ItemID into g
                                select new SaleGrossProfitReport
                                {
                                    ItemID = g.Key,
                                    TotalItem = g.Sum(i => i.Total),
                                    TotalItemCost = g.Sum(i => i.TotalCost),
                                    TotalItemPrice = g.Sum(i => i.TotalPrice),
                                    GrossProfitItem = g.Sum(i => i.GrossProfit),
                                    TotalAfterDisItem = g.Sum(i => i.TotalAfterDis),
                                    TotalDiscountItem = g.Sum(i => i.Discount),
                                    GrossProfitItemP = g.Sum(i => i.TotalPrice) == 0 ? 0 : g.Sum(i => i.GrossProfit) * 100 / g.Sum(i => i.TotalPrice),
                                }).ToList();
            foreach (var i in all)
            {
                foreach (var j in sumGroupData)
                {
                    if (i.ItemID == j.ItemID)
                    {
                        i.TotalItem = j.TotalItem;
                        i.GrossProfitItem = j.GrossProfitItem;
                        i.GrossProfitItemP = j.GrossProfitItemP;
                        i.TotalItemCost = j.TotalItemCost;
                        i.TotalItemPrice = j.TotalItemPrice;
                        i.TotalAfterDisItem = j.TotalAfterDisItem;
                        i.TotalDiscountItem = j.TotalDiscountItem;
                    }
                }
            }
            decimal totalAfterDisAllF = all.Sum(i => i.TotalAfterDis);
            decimal totalGrossProfit = all.Sum(i => i.GrossProfit);
            decimal totalDiscount = all.Sum(i => i.Discount);
            decimal totalAllCost = all.Sum(i => i.Cost * i.Qty);
            decimal totalAllPrice = all.Sum(i => i.Price * i.Qty);
            decimal totalGrossProfitP = all.Sum(i => i.TotalPrice) == 0 ? 0 : all.Sum(i => i.GrossProfit) * 100 / all.Sum(i => i.TotalPrice);
            var _data = (from _all in all
                         group _all by new { _all.InvoiceNo, _all.ItemID, _all.Cost, _all.Price } into g
                         let _g = g.FirstOrDefault()
                         select new SaleGrossProfitReport
                         {
                             ItemID = _g.ItemID,
                             InvoiceNo = _g.InvoiceNo,
                             Code = _g.Code,
                             CostF = $"{cur.Description} {string.Format("{0:#,0}", _g.Cost)}",
                             QtyF = $"{string.Format("{0:#,0}", g.Sum(u => u.Qty))}",
                             TotalF = $"{cur.Description} {string.Format("{0:#,0}", _g.Total)}",
                             TotalItemF = $"{cur.Description} {string.Format("{0:#,0}", g.Sum(i => i.TotalItem))}",
                             TotalGrossProfitF = $"{cur.Description} {string.Format("{0:#,0}", totalGrossProfit)}",
                             PriceF = $"{cur.Description} {string.Format("{0:#,0}", _g.Price)}",
                             TotalItemCostF = $"{cur.Description} {string.Format("{0:#,0}", _g.TotalItemCost)}",
                             TotalItemPriceF = $"{cur.Description} {string.Format("{0:#,0}", _g.TotalItemPrice)}",
                             TotalCostF = $"{cur.Description} {string.Format("{0:#,0}", _g.TotalCost)}",
                             TotalPriceF = $"{cur.Description} {string.Format("{0:#,0}", _g.TotalPrice)}",
                             DiscountF = $"{cur.Description} {string.Format("{0:#,0}", _g.Discount)}",
                             GrossProfitF = $"{cur.Description} {string.Format("{0:#,0}", _g.GrossProfit)}",
                             TotalAfterDisF = $"{cur.Description} {string.Format("{0:#,0}", _g.TotalAfterDis)}",
                             TotalDiscountItemF = $"{cur.Description} {string.Format("{0:#,0}", _g.TotalDiscountItem)}",
                             TotalAfterDisItemF = $"{cur.Description} {string.Format("{0:#,0}", _g.TotalAfterDisItem)}",
                             TotalAfterDisAllF = $"{cur.Description} {string.Format("{0:#,0}", totalAfterDisAllF)}",
                             GrossProfitItemF = $"{cur.Description} {string.Format("{0:#,0}", _g.GrossProfitItem)}",
                             TotalDiscountF = $"{cur.Description} {string.Format("{0:#,0}", totalDiscount)}",
                             TotalAllCostF = $"{cur.Description} {string.Format("{0:#,0}", totalAllCost)}",
                             TotalAllPriceF = $"{cur.Description} {string.Format("{0:#,0}", totalAllPrice)}",
                             TotalGrossProfitPF = $"% {string.Format("{0:#,0}", totalGrossProfitP)}",
                             GrossProfitPF = $"% {string.Format("{0:#,0}", _g.GrossProfitP)}",
                             GrossProfitItemPF = $"% {string.Format("{0:#,0}", _g.GrossProfitItemP)}",
                             UoMName = _g.UoMName,
                             ItemName = _g.ItemName,
                             PostingDate = _g.PostingDate,
                             DateOut = _g.DateOut,
                             TimeIn = _g.TimeIn,
                             TimeOut = _g.TimeOut,
                         }).ToList();
            var dataPrint = _data.GroupBy(i => i.ItemID).ToList();
            var user = _context.UserAccounts.Include(i => i.Employee).FirstOrDefault(i => i.ID == userId) ?? new UserAccount();
            string empName = user.Employee == null ? "" : user.Employee.Name ?? "";
            PirntSaleGrossProfitViewModel saleGrossProfit = new()
            {
                HeaderSaleGrossProfit = new HeaderSaleGrossProfit
                {
                    FromDate = fromDate == null ? "" : _fromDate.ToShortDateString(),
                    ToDate = toDate == null ? "" : _toDate.ToShortDateString(),
                    ToTime = timeTo ?? "",
                    FromTime = timeFrom ?? "",
                    UserName = user != null ? $"{user.Username ?? ""} => {empName}" : ""
                },
                SaleGrossProfitReports = dataPrint,
                SummarySaleGrossProfit = new SummarySaleGrossProfit
                {
                    TotalItem = _data.FirstOrDefault()?.TotalItemF,
                    TotalSaleGrossProfit = _data.FirstOrDefault()?.TotalGrossProfitF,
                    TotalSaleAfterDiscount = _data.FirstOrDefault()?.TotalAfterDisAllF,
                    TotalDiscount = _data.FirstOrDefault()?.TotalDiscountF,
                    TotalCost = _data.FirstOrDefault()?.TotalAllCostF,
                    TotalPrice = _data.FirstOrDefault()?.TotalAllPriceF,
                    TotalSaleGrossProfitP = _data.FirstOrDefault()?.TotalGrossProfitPF,
                }
            };
            return new ViewAsPdf(saleGrossProfit)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }

        /// Sale KSMS Report //
        public IActionResult SaleKSMSReport(string fromDate, string toDate)
        {
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            var curDes = _context.Currency.Find(GetCompany().SystemCurrencyID)?.Description;
            var data = (from rp in _context.Receipt.Where(i => i.DateIn >= _fromDate & i.DateIn <= _toDate)
                        join branch in _context.Branches on rp.BranchID equals branch.ID
                        join cus in _context.BusinessPartners on rp.CustomerID equals cus.ID
                        join rpd in _context.ReceiptDetail.Where(i => !i.IsKsms) on rp.ReceiptID equals rpd.ReceiptID
                        join uom in _context.UnitofMeasures on rpd.UomID equals uom.ID
                        join item in _context.ItemMasterDatas on rpd.ItemID equals item.ID
                        join user in _context.UserAccounts on rp.UserOrderID equals user.ID
                        join series in _context.Series on rp.SeriesID equals series.ID
                        group new { rp, rpd, branch, cus, item, uom, user, series } by new { rp.ReceiptID, rpd.ID } into g
                        let d = g.FirstOrDefault()
                        let mobile = _context.AutoMobiles.FirstOrDefault(i => i.AutoMID == d.rp.VehicleID) ?? new AutoMobile()
                        let model = _context.AutoModels.FirstOrDefault(i => i.ModelID == mobile.ModelID) ?? new AutoModel()
                        select new SaleReportKSMS
                        {
                            Invoice = $"{d.series.Name}-{d.rp.ReceiptNo}",
                            RID = d.rp.ReceiptID,
                            BranchName = d.branch.Name,
                            CusName = d.cus.Name,
                            Discount = $"{curDes} {string.Format("{0:#,0}", d.rpd.DiscountValue * d.rp.ExchangeRate)}",
                            GrandTotal = $"{curDes} {string.Format("{0:#,0}", g.Sum(i => i.rp.GrandTotal) * d.rp.ExchangeRate)}",
                            ItemCode = d.item.Code,
                            ItemName = d.item.KhmerName,
                            LineID = DateTime.Now.Ticks.ToString(),
                            Plate = mobile.Plate,
                            ModelName = model.ModelName,
                            Qty = d.rpd.Qty,
                            SoldAmount = $"{curDes} {string.Format("{0:#,0}", g.Sum(i => i.rp.Sub_Total) * d.rp.ExchangeRate)}",
                            TotalF = $"{curDes} {string.Format("{0:#,0}", d.rpd.Total * d.rp.ExchangeRate)}",
                            UnitPrice = $"{curDes} {string.Format("{0:#,0}", d.rpd.UnitPrice * d.rp.ExchangeRate)}",
                            Total = d.rpd.Total * d.rp.ExchangeRate,
                            TotalItem = g.Count(),
                            UoM = d.uom.Name,
                            UserName = d.user.Username,
                            Cur = curDes,
                            CusID = d.cus.ID,
                            PostingDate = d.rp.DateIn.ToShortDateString(),
                        }).ToList();
            var _data = data.GroupBy(i => i.CusID).ToList();
            foreach (var i in data)
            {
                var total = _data.FirstOrDefault(d => d.Key == i.CusID).Sum(i => i.Total);
                i.TotalInvoice = $"{i.Cur} {string.Format("{0:#,0}", total)}";
                i.TotalAll = $"{i.Cur} {string.Format("{0:#,0}", data.Sum(sum => sum.Total))}";
            }

            SaleReportKSMSPrint salePrint = new()
            {
                Detials = data.GroupBy(i => i.CusID).ToList(),
                Footer = new SaleReportKSMSFooter
                {
                    TotalAll = data.FirstOrDefault()?.TotalAll,
                    TotalItem = data.Count,
                },
                Header = new SaleReportKSMSHeader
                {
                    FromDate = _fromDate.ToShortDateString(),
                    ToDate = _toDate.ToShortDateString(),
                }
            };
            return new ViewAsPdf(salePrint)
            {
                CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
            };
        }

        //=============outgoingpayment======
        public IActionResult PreviewOutgoingPaymentReport(string status, string fromDate, string toDate)
        {
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            var outgoingPayment = (from o in _context.OutgoingPayments.Where(i => i.CompanyID == GetCompany().ID)
                                   join od in _context.OutgoingPaymentDetails on o.OutgoingPaymentID equals od.OutgoingPaymentID
                                   join docType in _context.DocumentTypes on o.DocumentID equals docType.ID
                                   let _currencyName = _context.Currency.FirstOrDefault(i => i.ID == od.CurrencyID).Description
                                   select new OutgoingPaymentReport
                                   {
                                       LindID = o.OutgoingPaymentID,
                                       ItemInvoice = od.ItemInvoice,
                                       Date = o.PostingDate.ToString(),
                                       Vendor = _context.BusinessPartners.FirstOrDefault(i => i.ID == o.VendorID).Name,
                                       User = _context.UserAccounts.FirstOrDefault(i => i.ID == o.UserID).Username,
                                       TotalAmountDue = $"{_currencyName} {o.TotalAmountDue:0.000}",
                                       Totalpayment = $"{_currencyName} {od.Totalpayment:0.000}",
                                       Status = o.Status,
                                   }).ToList();
            var outgoingpay = outgoingPayment.ToList();
            if (status != "all" && fromDate != null && toDate != null)
            {
                outgoingpay = outgoingPayment.Where(i => i.Status == status && DateTime.Parse(fromDate) <= DateTime.Parse(i.Date) && DateTime.Parse(toDate) >= DateTime.Parse(i.Date)).ToList();
            }
            else if (status == "all" && fromDate != null && toDate != null)
            {
                outgoingpay = outgoingPayment.Where(i => i.Status != status && DateTime.Parse(fromDate) <= DateTime.Parse(i.Date) && DateTime.Parse(toDate) >= DateTime.Parse(i.Date)).ToList();
            }
            if (outgoingPayment.Any())
            {
                return new ViewAsPdf(outgoingpay)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            return Ok();

        }

        //=============  outgoingpayment Order  ============
        public IActionResult PreviewOutgoingPaymentOrderReport(string status, string fromDate, string toDate)
        {
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            var outgoingPaymentOrder = (from o in _context.OutgoingPaymentOrders.Where(i => i.CompanyID == GetCompany().ID)
                                        join od in _context.OutgoingPaymentOrderDetails on o.ID equals od.OutgoingPaymentOrderID
                                        join docType in _context.DocumentTypes on o.DocumentID equals docType.ID
                                        let _currencyName = _context.Currency.FirstOrDefault(i => i.ID == od.CurrencyID).Description
                                        select new OutgoingPaymentOrderReport
                                        {
                                            LindID = o.ID,
                                            ItemInvoice = od.ItemInvoice,
                                            Date = o.PostingDate.ToString(),
                                            Vendor = _context.BusinessPartners.FirstOrDefault(i => i.ID == o.VendorID).Name,
                                            User = _context.UserAccounts.FirstOrDefault(i => i.ID == o.UserID).Username,
                                            TotalAmountDue = $"{_currencyName} {o.TotalAmountDue:0.000}",
                                            Totalpayment = $"{_currencyName} {od.Totalpayment:0.000}",
                                            Status = o.Status,

                                        }).ToList();
            var outgoingpay = outgoingPaymentOrder.ToList();
            if (status != "all" && fromDate != null && toDate != null)
            {
                outgoingpay = outgoingPaymentOrder.Where(i => i.Status == status && DateTime.Parse(fromDate) <= DateTime.Parse(i.Date) && DateTime.Parse(toDate) >= DateTime.Parse(i.Date)).ToList();
            }
            else if (status == "all" && fromDate != null && toDate != null)
            {
                outgoingpay = outgoingPaymentOrder.Where(i => i.Status != status && DateTime.Parse(fromDate) <= DateTime.Parse(i.Date) && DateTime.Parse(toDate) >= DateTime.Parse(i.Date)).ToList();
            }
            if (outgoingPaymentOrder.Any())
            {
                return new ViewAsPdf(outgoingpay)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            return Ok();

        }

        //====================incomming payment=================
        public IActionResult PreviewIncommingPayment(int LineID)
        {
            var incompayment = (from i in _context.IncomingPayments.Where(i => i.IncomingPaymentID == LineID)
                                join icd in _context.IncomingPaymentDetails on i.IncomingPaymentID equals icd.IncomingPaymentID
                                join docType in _context.DocumentTypes on i.DocTypeID equals docType.ID
                                let _currencyName = _context.Currency.FirstOrDefault(i => i.ID == icd.CurrencyID).Description
                                select new IncomingPaymentReport
                                {
                                    LindID = i.IncomingPaymentID,
                                    ItemInvoice = icd.ItemInvoice,
                                    Date = i.PostingDate.ToString(),
                                    Vendor = _context.BusinessPartners.FirstOrDefault(x => x.ID == i.CustomerID).Name,
                                    User = _context.UserAccounts.FirstOrDefault(s => s.ID == i.UserID).Username,
                                    TotalAmountDue = $"{_currencyName} {i.TotalAmountDue:0.000}",
                                    Totalpayment = icd.Totalpayment,
                                    Status = i.Status,
                                    Currency = _currencyName
                                }).ToList();

            if (incompayment.Any())
            {
                return new ViewAsPdf(incompayment)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            return Ok();
        }
        public IActionResult PreviewIncommingPaymentReport(string status, string fromDate, string toDate)
        {
            _ = DateTime.TryParse(toDate, out DateTime _toDate);
            _ = DateTime.TryParse(fromDate, out DateTime _fromDate);
            var incompayment = (from i in _context.IncomingPayments.Where(i => i.CompanyID == GetCompany().ID)
                                join icd in _context.IncomingPaymentDetails on i.IncomingPaymentID equals icd.IncomingPaymentID
                                join docType in _context.DocumentTypes on i.DocTypeID equals docType.ID
                                let _currencyName = _context.Currency.FirstOrDefault(i => i.ID == icd.CurrencyID).Description
                                select new IncomingPaymentReport
                                {
                                    LindID = i.IncomingPaymentID,
                                    ItemInvoice = icd.ItemInvoice,
                                    Date = i.PostingDate.ToString(),
                                    Vendor = _context.BusinessPartners.FirstOrDefault(x => x.ID == i.CustomerID).Name,
                                    User = _context.UserAccounts.FirstOrDefault(s => s.ID == i.UserID).Username,
                                    TotalAmountDue = $"{_currencyName} {i.TotalAmountDue:0.000}",
                                    Totalpayment = icd.Totalpayment,
                                    Status = i.Status,
                                    Currency = _currencyName

                                }).ToList();
            var incomingpay = incompayment.ToList();
            if (status != "all" && fromDate != null && toDate != null)
            {
                incomingpay = incompayment.Where(i => i.Status == status && DateTime.Parse(fromDate) <= DateTime.Parse(i.Date) && DateTime.Parse(toDate) >= DateTime.Parse(i.Date)).ToList();
            }
            else if (status == "all" && fromDate != null && toDate != null)
            {
                incomingpay = incompayment.Where(i => i.Status != status && DateTime.Parse(fromDate) <= DateTime.Parse(i.Date) && DateTime.Parse(toDate) >= DateTime.Parse(i.Date)).ToList();
            }
            if (incompayment.Any())
            {
                return new ViewAsPdf(incomingpay)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            return Ok();

        }


        public IActionResult PrintGeneralLedger(string datefrom, string dateto)
        {
            var data = _financialReports.GetGeneralLedgerReports(datefrom, dateto, GetCompany());
            if (data.Any())
            {
                return new ViewAsPdf(data)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }

            return Ok();
        }
        //print GroupCustomer
        public IActionResult PrintGroupCustomer(string DateFrom, string DateTo, string TimeFrom, string TimeTo, int plid, string docType)
        {

            var data = _report.GetGroupCustomerReport(GetCompany(), DateFrom, DateTo, TimeFrom, TimeTo, plid, docType);
            var all = (from a in data
                       group a by new { a.Group1ID } into g
                       let d = g.FirstOrDefault()
                       select new GroupCustomer
                       {
                           Group1ID = d.Group1ID,
                           AppliedAmountCal = d.AppliedAmountCal,
                           ApplyAmountTotal = d.ApplyAmountTotal,
                           BalanceDue = d.BalanceDue,
                           BalanceDueTotal = d.BalanceDueTotal,
                           Currency = d.Currency,
                           DateOut = d.DateOut,
                           DiscountItem = d.DiscountItem,
                           EmpName = d.EmpName,
                           ExchangeRate = d.ExchangeRate,
                           Footer = new Footer
                           {
                               TotalSumGroup = d.TotalSumGroup,
                               TotalAppliedAmountGroup = d.TotalAppliedAmountGroup,
                               TotalBalanceDueGroup = d.TotalBalanceDueGroup
                           },
                           GrandTotalStr = d.GrandTotal,
                           GrandTotalCustomer = d.GrandTotalCustomer,
                           GrandTotalCal = d.GrandTotalCal,
                           GroupItems = g.Select(e => new GroupItem
                           {
                               CustName = e.CustName,
                               ReceiptNo = e.ReceiptNo,
                               Doutype = e.DouType,
                               DateOut = e.DateOut,
                               EmpName = e.EmpName,
                               Currency = e.Currency,
                               CustID = e.CustID,
                               TimeOut = e.TimeOut,
                               DiscountItem = e.DiscountItem,
                               TotalStr = e.GrandTotal,
                               ApplyAmount = e.ApplyAmount,
                               BalanceDue = e.BalanceDue,
                           }).ToList(),
                           GroupName = d.GroupName,
                           Header = new Header
                           {
                               DateFrom = d.DateFrom,
                               DateTo = d.DateTo
                           },
                           ReceiptID = d.ReceiptID,
                           ReceiptNo = d.ReceiptNo,
                           TimeOut = d.TimeOut,
                           TotalAppliedAmountGroup = d.TotalAppliedAmountGroup,
                           TotalBalanceDueGroup = d.TotalBalanceDueGroup,
                           TotalSumGroup = d.TotalSumGroup
                       }).ToList();
            if (all.Any())
            {
                return new ViewAsPdf(all)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            return Ok();
        }
        public IActionResult PrintCountMember(string DateFrom, string DateTo, int BranchID, int UserID, string TimeFrom, string TimeTo, int plid)
        {
            List<Receipt> receiptsFilter = new();
            var branchHeader = _context.Branches.Find(BranchID) ?? new Branch();
            var userHeader = _context.UserAccounts.Include(i => i.Employee).FirstOrDefault(i => i.ID == UserID) ?? new UserAccount();
            var userName = userHeader.Username ?? "";
            var employeeName = userHeader.Employee != null ? userHeader.Employee.Name : "";
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
            var com = GetCompany();
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
                             DateIn = data.r.DateIn.ToString("dd-MM-yyyy"),
                             TimeIn = data.r.TimeIn,
                             DiscountItem = _fncModule.ToCurrency(data.r.DiscountValue, plCur.Amounts),
                             Currency = data.curr_pl.Description,
                             GrandTotal = _fncModule.ToCurrency(data.r.GrandTotal, plCur.Amounts),
                             DisRemark = data.r.RemarkDiscount,
                             //Summary
                             DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                             DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                             Branch = branchHeader.Name,
                             UserName = userName + " , " + employeeName,
                             Logo = com.Logo,
                             //SCount = ChCount.ToString(),
                             GrandTotalBrand = data.curr_sys.Description + " " + _fncModule.ToCurrency(sumByBranch, sysCur.Amounts),
                             SumMale = _fncModule.ToCurrency(Convert.ToDouble(receiptsFilter.Sum(i => i.Male)), sysCur.Amounts),
                             SumFemale = _fncModule.ToCurrency(Convert.ToDouble(receiptsFilter.Sum(i => i.Female)), sysCur.Amounts),
                             SumChildren = _fncModule.ToCurrency(Convert.ToDouble(receiptsFilter.Sum(i => i.Children)), sysCur.Amounts)
                         }).ToList();
            if (sales.Any())
            {
                return new ViewAsPdf(sales)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }

            return Ok();
        }

        public IActionResult PrintCustomerStatement(string DateFrom, string DateTo, int CusID, int emID)
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
            var employee = _context.Employees.FirstOrDefault(s => s.ID == emID) ?? new Employee();
            var cusmer = _context.BusinessPartners.FirstOrDefault(s => s.ID == CusID) ?? new BusinessPartner();
            var list = from IPC in incomingpaymentcustomer
                       join CUS in _context.BusinessPartners on IPC.CustomerID equals CUS.ID
                       join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                       join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                       join lc in _context.Currency on IPC.LocalCurID equals lc.ID
                       let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == CUR.ID) ?? new Display()
                       let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == CUN.ID) ?? new Display()
                       //where IPC.Status == "open"
                       select new IncomingPaymentCustomer
                       {
                           //Master
                           CustomerName = cusmer.ID == 0 ? "All Customer" : cusmer.Name,
                           EmployeeName = employee.ID == 0 ? "All Saller" : employee.Name,
                           InvoiceNumber = IPC.ItemInvoice,
                           PostingDate = IPC.PostingDate,
                           Date = IPC.Date,
                           EmName = IPC.EmName,
                           CreatorName = IPC.CreatorName,
                           OverdueDays = (IPC.Date.Date - DateTime.Now.Date).Days,
                           TotalPayment = Convert.ToDouble(_fncModule.ToCurrency(IPC.Total, plCur.Amounts)),
                           Applied_Amount = Convert.ToDouble(_fncModule.ToCurrency(IPC.Applied_Amount, plCur.Amounts)),
                           BalanceDue = Convert.ToDouble(_fncModule.ToCurrency(IPC.BalanceDue, plCur.Amounts)),
                           SysName = CUR.Description,
                           CurrencyName = lc.Description,
                           //Summary
                           BalanceDueSSC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueSSC, sysCur.Amounts)),
                           BalanceDueLC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueLC, lcCur.Amounts)),
                           DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                           DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                       };
            if (list.Any())
            {
                return new ViewAsPdf(list)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            else
                return Ok();
        }
        public IActionResult PrintCustomerStatmentDetail(string DateFrom, string DateTo, int CusID, int emID)
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
            var employee = _context.Employees.FirstOrDefault(s => s.ID == emID) ?? new Employee();
            var cusmer = _context.BusinessPartners.FirstOrDefault(s => s.ID == CusID) ?? new BusinessPartner();

            var SaleAREdite = (from IPC in incomingpaymentcustomer
                               join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                               join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                               join lc in _context.Currency on IPC.LocalCurID equals lc.ID
                               join SAR in _context.SaleAREdites on IPC.InvoiceNumber equals SAR.InvoiceNo
                               join SAD in _context.SaleAREditeDetails on SAR.SARID equals SAD.SARID
                               group new { IPC, CUR, CUN, lc, SAR, SAD, } by new { IPC.IncomingPaymentCustomerID } into g
                               let data = g.FirstOrDefault()
                               let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                               let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUN.ID) ?? new Display()
                               let contact = _context.ContactPersons.FirstOrDefault(i => i.BusinessPartnerID == cusmer.ID) ?? new ContactPerson()
                               select new IncomingPaymentCustomer
                               {
                                   CustomerName = cusmer.ID == 0 ? "All Customer" : cusmer.Name,
                                   CustomerNametwo = cusmer.ID == 0 ? " All Customer" : cusmer.Name2,
                                   EmployeeName = employee.ID == 0 ? "All Saller" : employee.Name,
                                   ContactName = contact.FirstName,
                                   InvoiceNumber = data.IPC.ItemInvoice,
                                   PostingDate = data.IPC.PostingDate,
                                   Date = data.IPC.Date,
                                   EmName = data.IPC.EmName,
                                   CreatorName = data.IPC.CreatorName,
                                   OverdueDays = (data.IPC.Date.Date - DateTime.Now.Date).Days,
                                   TotalPayment = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.Total, plCur.Amounts)),
                                   Applied_Amount = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.Applied_Amount, plCur.Amounts)),
                                   BalanceDue = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.BalanceDue, plCur.Amounts)),
                                   SysName = data.CUR.Description,
                                   CurrencyName = data.lc.Description,
                                   //Summary
                                   BalanceDueSSC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueSSC, sysCur.Amounts)),
                                   BalanceDueLC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueLC, lcCur.Amounts)),
                                   DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                   DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                   DetailIteme = g.Select(x => new DetailItemd
                                   {
                                       Code = x.SAD.ItemCode,
                                       ItemName = x.SAD.ItemNameKH,
                                       Qty = x.SAD.Qty,
                                       Price = x.SAD.UnitPrice,
                                       Discount = x.SAD.DisValue,
                                       TotalAmount = x.SAD.Total
                                   }).ToList()
                               }).ToList();

            var SaleReInv = (from IPC in incomingpaymentcustomer
                             join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                             join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                             join lc in _context.Currency on IPC.LocalCurID equals lc.ID
                             join SAR in _context.ARReserveInvoices on IPC.InvoiceNumber equals SAR.InvoiceNo
                             join SAD in _context.ARReserveInvoiceDetails on SAR.ID equals SAD.ARReserveInvoiceID
                             group new { IPC, CUR, CUN, lc, SAR, SAD, } by new { IPC.IncomingPaymentCustomerID } into g
                             let data = g.FirstOrDefault()
                             let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                             let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUN.ID) ?? new Display()
                             let contact = _context.ContactPersons.FirstOrDefault(i => i.BusinessPartnerID == cusmer.ID) ?? new ContactPerson()
                             select new IncomingPaymentCustomer
                             {
                                 CustomerName = cusmer.ID == 0 ? "All Customer" : cusmer.Name,
                                 CustomerNametwo = cusmer.ID == 0 ? " All Customer" : cusmer.Name2,
                                 EmployeeName = employee.ID == 0 ? "All Saller" : employee.Name,
                                 ContactName = contact.FirstName,
                                 InvoiceNumber = data.IPC.ItemInvoice,
                                 PostingDate = data.IPC.PostingDate,
                                 Date = data.IPC.Date,
                                 EmName = data.IPC.EmName,
                                 CreatorName = data.IPC.CreatorName,
                                 OverdueDays = (data.IPC.Date.Date - DateTime.Now.Date).Days,
                                 TotalPayment = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.Total, plCur.Amounts)),
                                 Applied_Amount = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.Applied_Amount, plCur.Amounts)),
                                 BalanceDue = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.BalanceDue, plCur.Amounts)),
                                 SysName = data.CUR.Description,
                                 CurrencyName = data.lc.Description,
                                 //Summary
                                 BalanceDueSSC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueSSC, sysCur.Amounts)),
                                 BalanceDueLC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueLC, lcCur.Amounts)),
                                 DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                 DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                 DetailIteme = g.Select(x => new DetailItemd
                                 {
                                     Code = x.SAD.ItemCode,
                                     ItemName = x.SAD.ItemNameKH,
                                     Qty = x.SAD.Qty,
                                     Price = x.SAD.UnitPrice,
                                     Discount = x.SAD.DisValue,
                                     TotalAmount = x.SAD.Total
                                 }).ToList()
                             }).ToList();
            var SaleARReInvEd = (from IPC in incomingpaymentcustomer
                                 join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                                 join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                                 join lc in _context.Currency on IPC.LocalCurID equals lc.ID
                                 join SAR in _context.ARReserveInvoiceEditables on IPC.InvoiceNumber equals SAR.InvoiceNo
                                 join SAD in _context.ARReserveInvoiceEditableDetails on SAR.ID equals SAD.ARReserveInvoiceEditableID
                                 group new { IPC, CUR, CUN, lc, SAR, SAD, } by new { IPC.IncomingPaymentCustomerID } into g
                                 let data = g.FirstOrDefault()
                                 let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                                 let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUN.ID) ?? new Display()
                                 let contact = _context.ContactPersons.FirstOrDefault(i => i.BusinessPartnerID == cusmer.ID) ?? new ContactPerson()
                                 select new IncomingPaymentCustomer
                                 {
                                     CustomerName = cusmer.ID == 0 ? "All Customer" : cusmer.Name,
                                     CustomerNametwo = cusmer.ID == 0 ? " All Customer" : cusmer.Name2,
                                     EmployeeName = employee.ID == 0 ? "All Saller" : employee.Name,
                                     ContactName = contact.FirstName,
                                     InvoiceNumber = data.IPC.ItemInvoice,
                                     PostingDate = data.IPC.PostingDate,
                                     Date = data.IPC.Date,
                                     EmName = data.IPC.EmName,
                                     CreatorName = data.IPC.CreatorName,
                                     OverdueDays = (data.IPC.Date.Date - DateTime.Now.Date).Days,
                                     TotalPayment = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.Total, plCur.Amounts)),
                                     Applied_Amount = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.Applied_Amount, plCur.Amounts)),
                                     BalanceDue = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.BalanceDue, plCur.Amounts)),
                                     SysName = data.CUR.Description,
                                     CurrencyName = data.lc.Description,
                                     //Summary
                                     BalanceDueSSC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueSSC, sysCur.Amounts)),
                                     BalanceDueLC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueLC, lcCur.Amounts)),
                                     DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                     DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                     DetailIteme = g.Select(x => new DetailItemd
                                     {
                                         Code = x.SAD.ItemCode,
                                         ItemName = x.SAD.ItemNameKH,
                                         Qty = x.SAD.Qty,
                                         Price = x.SAD.UnitPrice,
                                         Discount = x.SAD.DisValue,
                                         TotalAmount = x.SAD.Total
                                     }).ToList()
                                 }).ToList();

            var SaleAr = (from IPC in incomingpaymentcustomer
                          join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                          join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                          join lc in _context.Currency on IPC.LocalCurID equals lc.ID
                          join SAR in _context.SaleARs on IPC.InvoiceNumber equals SAR.InvoiceNo
                          join SAD in _context.SaleARDetails on SAR.SARID equals SAD.SARID
                          group new { IPC, CUR, CUN, lc, SAR, SAD, } by new { IPC.IncomingPaymentCustomerID } into g
                          let data = g.FirstOrDefault()
                          let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                          let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUN.ID) ?? new Display()
                          let contact = _context.ContactPersons.FirstOrDefault(i => i.BusinessPartnerID == cusmer.ID) ?? new ContactPerson()
                          //where IPC.Status == "open"
                          select new IncomingPaymentCustomer
                          {
                              //Master
                              CustomerName = cusmer.ID == 0 ? "All Customer" : cusmer.Name,
                              CustomerNametwo = cusmer.ID == 0 ? " All Customer" : cusmer.Name2,
                              EmployeeName = employee.ID == 0 ? "All Saller" : employee.Name,
                              ContactName = contact.FirstName,
                              InvoiceNumber = data.IPC.ItemInvoice,
                              PostingDate = data.IPC.PostingDate,
                              Date = data.IPC.Date,
                              EmName = data.IPC.EmName,
                              CreatorName = data.IPC.CreatorName,
                              OverdueDays = (data.IPC.Date.Date - DateTime.Now.Date).Days,
                              TotalPayment = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.Total, plCur.Amounts)),
                              Applied_Amount = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.Applied_Amount, plCur.Amounts)),
                              BalanceDue = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.BalanceDue, plCur.Amounts)),
                              SysName = data.CUR.Description,
                              CurrencyName = data.lc.Description,
                              //Summary
                              BalanceDueSSC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueSSC, sysCur.Amounts)),
                              BalanceDueLC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueLC, lcCur.Amounts)),
                              DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                              DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                              DetailIteme = g.Select(x => new DetailItemd
                              {
                                  Code = x.SAD.ItemCode,
                                  ItemName = x.SAD.ItemNameKH,
                                  Qty = x.SAD.Qty,
                                  Price = x.SAD.UnitPrice,
                                  Discount = x.SAD.DisValue,
                                  TotalAmount = x.SAD.Total
                              }).ToList(),

                          }).ToList();

            var Receipts = (from IPC in incomingpaymentcustomer
                            join CUR in _context.Currency on IPC.SysCurrency equals CUR.ID
                            join CUN in _context.Currency on IPC.CurrencyID equals CUN.ID
                            join lc in _context.Currency on IPC.LocalCurID equals lc.ID
                            join re in _context.Receipt on IPC.InvoiceNumber equals re.ReceiptNo
                            join red in _context.ReceiptDetail on re.ReceiptID equals red.ReceiptID
                            group new { IPC, CUR, CUN, lc, re, red, } by new { IPC.IncomingPaymentCustomerID } into g
                            let data = g.FirstOrDefault()
                            let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUR.ID) ?? new Display()
                            let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == data.CUN.ID) ?? new Display()
                            let contact = _context.ContactPersons.FirstOrDefault(i => i.BusinessPartnerID == cusmer.ID) ?? new ContactPerson()
                            //where IPC.Status == "open"
                            select new IncomingPaymentCustomer
                            {
                                //Master
                                CustomerName = cusmer.ID == 0 ? "All Customer" : cusmer.Name,
                                CustomerNametwo = cusmer.ID == 0 ? " All Customer" : cusmer.Name2,
                                EmployeeName = employee.ID == 0 ? "All Saller" : employee.Name,
                                ContactName = contact.FirstName,
                                InvoiceNumber = data.IPC.ItemInvoice,
                                PostingDate = data.IPC.PostingDate,
                                Date = data.IPC.Date,
                                EmName = data.IPC.EmName,
                                CreatorName = data.IPC.CreatorName,
                                OverdueDays = (data.IPC.Date.Date - DateTime.Now.Date).Days,
                                TotalPayment = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.Total, plCur.Amounts)),
                                Applied_Amount = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.Applied_Amount, plCur.Amounts)),
                                BalanceDue = Convert.ToDouble(_fncModule.ToCurrency(data.IPC.BalanceDue, plCur.Amounts)),
                                SysName = data.CUR.Description,
                                CurrencyName = data.lc.Description,
                                //Summary
                                BalanceDueSSC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueSSC, sysCur.Amounts)),
                                BalanceDueLC = Convert.ToDouble(_fncModule.ToCurrency(SumBalanceDueLC, lcCur.Amounts)),
                                DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                                DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                                DetailIteme = g.Select(x => new DetailItemd
                                {
                                    Code = x.red.Code,
                                    ItemName = x.red.KhmerName,
                                    Qty = x.red.Qty,
                                    Price = x.red.UnitPrice,
                                    Discount = x.red.DiscountValue,
                                    TotalAmount = x.red.Total
                                }).ToList(),

                            }).ToList();

            var allpaymentcustomer = new List<IncomingPaymentCustomer>
            (SaleAREdite.Count + SaleReInv.Count + SaleARReInvEd.Count + SaleAr.Count + Receipts.Count);
            allpaymentcustomer.AddRange(Receipts);
            allpaymentcustomer.AddRange(SaleAr);
            allpaymentcustomer.AddRange(SaleAREdite);
            allpaymentcustomer.AddRange(SaleReInv);
            allpaymentcustomer.AddRange(SaleARReInvEd);
            var _data = (from rm in allpaymentcustomer
                         select new IncomingPaymentCustomer
                         {
                             CustomerName = cusmer.ID == 0 ? "All Customer" : cusmer.Name,
                             CustomerNametwo = cusmer.ID == 0 ? " All Customer" : cusmer.Name2,
                             EmployeeName = employee.ID == 0 ? "All Saller" : employee.Name,
                             ContactName = rm.ContactName,
                             InvoiceNumber = rm.InvoiceNumber,
                             PostingDate = rm.PostingDate,
                             Date = rm.Date,
                             EmName = rm.EmName,
                             CreatorName = rm.CreatorName,
                             OverdueDays = (rm.Date.Date - DateTime.Now.Date).Days,
                             TotalPayment = rm.TotalPayment,
                             Applied_Amount = rm.Applied_Amount,
                             BalanceDue = rm.BalanceDue,
                             SysName = rm.SysName,
                             CurrencyName = rm.CurrencyName,
                             //Summary
                             BalanceDueSSC = rm.BalanceDueSSC,
                             BalanceDueLC = rm.BalanceDueLC,
                             DateFrom = Convert.ToDateTime(DateFrom).ToString("dd-MM-yyyy"),
                             DateTo = Convert.ToDateTime(DateTo).ToString("dd-MM-yyyy"),
                             DetailIteme = rm.DetailIteme,
                         }).ToList();

            if (_data.Any())
            {
                return new ViewAsPdf(_data)
                {
                    CustomSwitches = "--footer-left \"Powerby: KHMER EDI\" " + "--footer-center \"Page: [page] of [toPage]\" --footer-line --footer-font-size \"9\" --footer-spacing 2 --footer-font-name \"Myriad Pro\""
                };
            }
            else
                return Ok();
        }

    }
}