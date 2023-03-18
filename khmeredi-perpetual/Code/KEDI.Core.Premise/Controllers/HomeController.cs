using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using CKBS.AppContext;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.Services.Administrator.Report;
using System.Globalization;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SettingDashboard;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;
using KEDI.Core.Premise.Repository;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;

namespace CKBS.Controllers
{
    [Privilege]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        private readonly IReport _report;
        private readonly UtilityModule _utility;
        private readonly UserManager _userModule;

        public HomeController(DataContext context, IReport report, UtilityModule utility, UserManager userModule)
        {
            _context = context;
            _report = report;
            _utility = utility;
            _userModule = userModule;
        }

        private static readonly string R1 = "R1";
        private static readonly string R2 = "R2";
        private static readonly string R3 = "R3";
        //private static string R4 = "R4";

        public IActionResult Dashboard()
        {
            ViewBag.style = "fa fa-home";
            ViewBag.Main = "Dashborad";
            ViewBag.Page = "Dashborad";
            ViewBag.Dashboard = "highlight";

            var company = _userModule.CurrentUser.Company;
            var dashboard = View(new DashboardModel
            {
                DashboardSetting = new List<DashboardSetting>(),
                GeneralSetting = new Display()
            });

            if (company == null)
            {
                return dashboard;
            }

            var cur = _context.Currency.Find(company.SystemCurrencyID);
            if (cur == null) { return dashboard; }

            ViewBag.Cur = cur.Description;
            var dashset = _context.DashboardSettings.ToList();
            var generalSetting = _utility.GetGeneralSettingAdmin(company.SystemCurrencyID);
            bool isGerneralDashboard = _userModule.Check("DB001");
            bool isServiceDashboard = _userModule.Check("DB003");
            bool isCRMDashboard = _userModule.Check("DB002");
            if (!isCRMDashboard && !isServiceDashboard && isGerneralDashboard) ViewBag.GD = "active";
            else if (isCRMDashboard && !isServiceDashboard && !isGerneralDashboard) ViewBag.CRM = "active";
            else if (!isCRMDashboard && isServiceDashboard && !isGerneralDashboard) ViewBag.SD = "active";
            else if ((isCRMDashboard || isServiceDashboard) && isGerneralDashboard) ViewBag.GD = "active";
            else if ((isCRMDashboard || isGerneralDashboard) && isServiceDashboard) ViewBag.SD = "active";
            else if ((isServiceDashboard || isGerneralDashboard) && isCRMDashboard) ViewBag.CRM = "active";
            DashboardModel data = new()
            {
                DashboardSetting = dashset,
                GeneralSetting = generalSetting.Display
            };
            var multi=_context.MultiBrands.FirstOrDefault(x=>x.UserID ==GetUserID()) ?? new Models.Services.Account.MultiBrand();
            if(multi.ID==0){
                multi.BranchID=_userModule.CurrentUser.BranchID;
                multi.UserID=GetUserID();
                multi.Active=true;
                _context.Add(multi);
                _context.SaveChanges();
            }

            return View(data);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SelectLanguage(string culture, string returnUrl = "")
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new Microsoft.AspNetCore.Http.CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(1) }
                );
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return LocalRedirect("/");
            }
            return LocalRedirect(returnUrl);
        }

        private int GetUserID()
        {
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _userId))
            {
                return _userId;
            }
            return 0;
        }

        private Company GetCompany()
        {
            var com = (from us in _context.UserAccounts.Where(w => w.ID == GetUserID())
                       join co in _context.Company on us.CompanyID equals co.ID
                       select co
                      ).FirstOrDefault();
            return com;
        }
        private Double Fomart(double amount)
        {
            var cur = _context.Currency.Find(GetCompany().SystemCurrencyID);
            var displaycurr = _context.DisplayCurrencies.FirstOrDefault(x => x.AltCurrencyID == cur.ID) ?? new Models.Services.POS.DisplayCurrency();
            var disformat = _context.Displays.FirstOrDefault(x => x.DisplayCurrencyID == displaycurr.AltCurrencyID) ?? new Display();
            string num = _utility.ToCurrency(amount, disformat.Amounts);
            double total = Convert.ToDouble(num);
            return total;
        }
        public IActionResult GetSaleReports()
        {
            var r1 = _context.DashboardSettings.FirstOrDefault(i => i.Code == R1) ?? new DashboardSetting();
            var r2 = _context.DashboardSettings.FirstOrDefault(i => i.Code == R2) ?? new DashboardSetting();
            var r3 = _context.DashboardSettings.FirstOrDefault(i => i.Code == R3) ?? new DashboardSetting();
            var cur = _context.Currency.Find(GetCompany().SystemCurrencyID);
            var balTotal = new GetCurrency();
            var bal = new GetCurrency();
            var AvgOBJ = new GetCurrency();
            var AvgQty = new GetCurrency();
            var top10Sales = new List<TopSale>();
            var saleGroups = new List<SaleByGroup>();
            var stocks = new List<Stock>();
            var monthlySales = new List<MonthlySale>();
            var rp = GetReciept().Receipts;
            var sar = GetSaleARs().SaleARs;
            var rm = GetRecieptMemo().ReceiptMemos;
            var scm = GetSaleCreadiMemos().SaleCreditMemo;
            var ms = GetMonthlySales();
            var sare = GetSaleReserve().SaleARReserve;
            var sarsercont = GetSaleARSercontract().SaleARServiceContract;
            var saredit = GetSaleAREdit().SaleAREdit;
            if (r1.Show)
            {
                var receipt = rp.Where(w => w.DateOut == DateTime.Today);
                var receiptMemo = rm.Where(w => w.DateOut == DateTime.Today);
                var saleAr = sar.Where(w => w.DateOut == DateTime.Today);
                var saleArMemo = scm.Where(w => w.DateOut == DateTime.Today);
                var salearreserve = sare.Where(w => w.DateOut == DateTime.Today);
                var salearsercon = sarsercont.Where(w => w.DateOut == DateTime.Today);
                var salearedits = saredit.Where(w => w.DateOut == DateTime.Today);
                balTotal = new GetCurrency
                {
                    Currency = cur.Description,
                    BalanceTotal = (decimal)(receipt.Sum(s => s.TotalItem) + saleAr.Sum(i => i.TotalItem) + salearreserve.Sum(i => i.TotalItem) + salearsercon.Sum(i => i.TotalItem) + salearedits.Sum(i => i.TotalItem) - (receiptMemo.Sum(i => i.TotalItem) + saleArMemo.Sum(i => i.TotalItem))),
                };
                var purch = _context.Purchase_APs.Where(w => w.PostingDate == DateTime.Today);
                var purchaseMemos = _context.PurchaseCreditMemos.Where(w => w.PostingDate == DateTime.Today);
                bal = new GetCurrency
                {
                    Currency = cur.Description,
                    BalanceTotal = (decimal)(purch.Sum(s => s.SubTotalAfterDisSys) - purchaseMemos.Sum(w => w.SubTotalAfterDisSys)),
                };
                var avg = _context.Receipt.Where(r => IsCurrentYear(r.DateOut));
                var receiptmemo = _context.ReceiptMemo.Where(w => IsCurrentYear(w.DateOut));
                var saleaR = _context.SaleARs.Where(w => IsCurrentYear(w.PostingDate));
                var salearRes = _context.ARReserveInvoices.Where(w => IsCurrentYear(w.PostingDate));
                var salearSer = _context.ServiceContracts.Where(w => IsCurrentYear(w.PostingDate));
                var salearedit = _context.SaleAREdites.Where(w => IsCurrentYear(w.PostingDate));

                var saleARMemo = _context.SaleCreditMemos.Where(w => IsCurrentYear(w.PostingDate));
                decimal Count = avg.Count() + saleaR.Count() + salearRes.Count() + salearSer.Count() + salearedit.Count();
                decimal SumAvg = (decimal)(avg.Sum(c => c.GrandTotal_Sys) + saleaR.Sum(i => i.SubTotalSys) + salearRes.Sum(i => i.SubTotalSys) + salearSer.Sum(i => i.SubTotalSys) + salearedit.Sum(i => i.SubTotalSys));
                //decimal avgreceipt = Count != 0 ? SumAvg / Count : 0;
                decimal countMemo = receiptmemo.Count() + saleARMemo.Count();
                decimal SumAvgMemo = (decimal)(receiptmemo.Sum(c => c.GrandTotalSys) + saleARMemo.Sum(i => i.SubTotalSys));
                //decimal avgreceiptMemo = countMemo != 0 ? SumAvgMemo / countMemo : 0;
                decimal totalCount = Count - countMemo;
                decimal totalAmount = SumAvg - SumAvgMemo;
                decimal allavg = totalCount == 0 ? 0 : totalAmount / totalCount;
                AvgOBJ = new GetCurrency
                {
                    Currency = cur.Description,
                    BalanceTotal = allavg,
                };
                var reDetail = from r in avg join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID select rd;
                var memoDetail = from rmemo in receiptmemo join rdmemo in _context.ReceiptDetailMemoKvms on rmemo.ID equals rdmemo.ReceiptMemoID select rdmemo;
                var saleARDetail = from s in saleaR join sd in _context.SaleARDetails on s.SARID equals sd.SARID select sd;
                var SaleARredetail = from re in salearRes join srd in _context.ARReserveInvoiceDetails on re.ID equals srd.ARReserveInvoiceID select srd;
                var saleARSercondetail = from sr in salearSer join serd in _context.ServiceContractDetails on sr.ID equals serd.ServiceContractID select serd;
                var salememoDetail = from scMemo in saleARMemo join scMemoD in _context.SaleCreditMemoDetails on scMemo.SCMOID equals scMemoD.SCMOID select scMemoD;
                var saleEditDetail = from se in _context.SaleAREdites join sed in _context.SaleAREditeDetails on se.SARID equals sed.SARID select sed;
                ///Recipt///
                decimal ReMemoDetailCount = memoDetail.Count();
                decimal ReMemoDetailQty = memoDetail.Sum(c => (decimal)c.Qty);
                decimal ReDetailCount = reDetail.Count();
                decimal ReDetailQty = reDetail.Sum(c => (decimal)c.Qty);
                /// Sale ///
                decimal saleMemoDetailCount = salememoDetail.Count();
                decimal saleMemoDetailQty = salememoDetail.Sum(c => (decimal)c.Qty);
                decimal saleDetailCount = saleARDetail.Count();
                decimal saleDetailQty = saleARDetail.Sum(c => (decimal)c.Qty);

                decimal saleresDetailCount = SaleARredetail.Count();
                decimal saleARRDetailQty = SaleARredetail.Sum(c => (decimal)c.Qty);

                decimal salearserDetailConunt = saleARSercondetail.Count();
                decimal saleARSerdetailQty = saleARSercondetail.Sum(c => (decimal)c.Qty);

                decimal saleArEditDetailCount = saleEditDetail.Count();
                decimal saleArEditDetailQty = saleEditDetail.Sum(c => (decimal)c.Qty);

                /// Calculate //
                decimal totalCountAvgQty = (ReDetailCount + saleDetailCount + saleresDetailCount + salearserDetailConunt + saleArEditDetailCount) - (ReMemoDetailCount + saleMemoDetailCount);
                decimal totalQty = (ReDetailQty + saleDetailQty + saleARRDetailQty + saleARSerdetailQty + saleArEditDetailQty) - (ReMemoDetailQty + saleMemoDetailQty);
                decimal avgQty = totalCountAvgQty == 0 ? 0 : totalQty / totalCountAvgQty;

                AvgQty = new GetCurrency
                {
                    //Currency = cur.Description,
                    BalanceTotal = avgQty
                };
            }
            if (r2.Show)
            {
                top10Sales = (from im in ms
                              select new TopSale
                              {
                                  Amount = rp.Where(i => i.ItemID == im.ItemID).Sum(i => i.TotalItem) + sar.Where(i => i.ItemID == im.ItemID).Sum(i => i.TotalItem) + sare.Where(i => i.ItemID == im.ItemID).Sum(i => i.TotalItem) + sarsercont.Where(i => i.ItemID == im.ItemID).Sum(i => i.TotalItem) + saredit.Where(i => i.ItemID == im.ItemID).Sum(i => i.TotalItem) -
                                  (rm.Where(i => i.ItemID == im.ItemID).Sum(i => i.TotalItem) + scm.Where(i => i.ItemID == im.ItemID).Sum(i => i.TotalItem)),
                                  ItemName = im.ItemName,
                              }).OrderByDescending(r => r.Amount).Take(10).Reverse().ToList();
                top10Sales.ForEach(i =>
                {
                    i.Amount = Fomart(i.Amount);
                });
                var firstMonth = new DateTime(DateTime.Today.Year, 1, 1);
                saleGroups = (from ig1 in _context.ItemGroup1
                              join item in ms on ig1.ItemG1ID equals item.Group1ID
                              group new { ig1, item } by ig1.ItemG1ID into g
                              let data = g.FirstOrDefault()
                              select new SaleByGroup
                              {
                                  Group1 = data.ig1.Name,
                                  Amount = rp.Where(i => i.Group1ID == data.ig1.ItemG1ID).Sum(i => i.TotalItem) +
                                    sar.Where(i => i.Group1ID == data.ig1.ItemG1ID).Sum(i => i.TotalItem) + sare.Where(x => x.Group1ID == data.ig1.ItemG1ID).Sum(x => x.TotalItem) + sarsercont.Where(x => x.Group1ID == data.ig1.ItemG1ID).Sum(x => x.TotalItem) + saredit.Where(x => x.Group1ID == data.ig1.ItemG1ID).Sum(x => x.TotalItem) -
                                    (rm.Where(i => i.Group1ID == data.ig1.ItemG1ID).Sum(i => i.TotalItem) +
                                    scm.Where(i => i.Group1ID == data.ig1.ItemG1ID).Sum(i => i.TotalItem))
                              }).ToList();
                saleGroups.ForEach(i =>
                {
                    i.Amount = Fomart(i.Amount);
                });
            }
            if (r3.Show)
            {
                stocks = (from ws in _context.WarehouseSummary
                          join im in _context.ItemMasterDatas on ws.ItemID equals im.ID
                          join g1 in _context.ItemGroup1 on im.ItemGroup1ID equals g1.ItemG1ID
                          group new { ws, g1 } by new { ws, g1 } into g
                          group g by g.Key.g1.ItemG1ID into gg
                          select new Stock
                          {
                              CumlativeValue = (double)gg.Sum(_g => _g.Key.ws.CumulativeValue),
                              ItemID = gg.FirstOrDefault().Key.g1.ItemG1ID,
                              ItemNameStock = gg.FirstOrDefault().Key.g1.Name
                          }).ToList();
                List<MonthlySale> _monthlySalesRevenue = new(rp.Count + sar.Count + sare.Count + sarsercont.Count() + saredit.Count());
                List<MonthlySale> _monthlySalesCredit = new(rm.Count + scm.Count);
                _monthlySalesCredit.AddRange(rm);
                _monthlySalesCredit.AddRange(scm);
                _monthlySalesRevenue.AddRange(rp);
                _monthlySalesRevenue.AddRange(sar);
                _monthlySalesRevenue.AddRange(sare);
                _monthlySalesRevenue.AddRange(sarsercont);
                _monthlySalesRevenue.AddRange(saredit);

                _monthlySalesCredit = (from msc in _monthlySalesCredit
                                       select new MonthlySale
                                       {
                                           SubTotal = msc.TotalItem * -1,
                                           DateOut = msc.DateOut
                                       }).ToList();

                List<MonthlySale> wholeSale = new(_monthlySalesCredit.Count + _monthlySalesRevenue.Count);
                wholeSale.AddRange(_monthlySalesCredit);
                wholeSale.AddRange(_monthlySalesRevenue);
                monthlySales = (from s in wholeSale
                                group s by s.DateOut.Month into g
                                let data = g.FirstOrDefault()
                                select new MonthlySale
                                {
                                    GrandTotal = Fomart(g.Sum(i => i.TotalItem)),
                                    Month = ToShortMonthName(g.FirstOrDefault().DateOut),
                                    DateOut = g.FirstOrDefault().DateOut
                                }).OrderBy(i => i.DateOut).ToList();
            }

            var reportModel = new SaleReportModel
            {
                MonthlySales = monthlySales.ToList(),
                TopSales = top10Sales.ToList(),
                SaleByGroups = saleGroups.ToList(),
                Stocks = stocks.ToList(),
                BalanceTotal = balTotal,
                Balance = bal,
                AverageReceipts = AvgOBJ,
                AverageQty = AvgQty,
            };
            return Ok(reportModel);
        }

        private static DateTime CombineWithTimeSpan(DateTime date, string time)
        {
            _ = DateTime.TryParse($"{date:yyyy/MM/dd} {time}", out DateTime _datetime);
            return _datetime;
        }

        private IEnumerable<MonthlySale> GetMonthlySales()
        {
            var list = (from r in _context.Receipt
                        join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                        join i in _context.ItemMasterDatas on rd.ItemID equals i.ID
                        join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                        group new { r, rd, i, curr_sys } by rd.ItemID into g
                        where IsCurrentYear(g.FirstOrDefault().r.DateOut)
                        let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == g.FirstOrDefault().curr_sys.ID) ?? new Display()
                        select new MonthlySale
                        {
                            ReceiptID = g.Key,
                            ItemID = g.FirstOrDefault().i.ID,
                            ItemName = g.FirstOrDefault().i.KhmerName,
                            // SubTotal = g.Sum(_rd => _rd.rd.Total_Sys),
                            SubTotal = Convert.ToDouble(_utility.ToCurrency(g.Sum(_rd => _rd.rd.Total_Sys), plCur.Amounts)),
                            GrandTotal = Convert.ToDouble(_utility.ToCurrency(g.Sum(_r => _r.r.GrandTotal_Sys), plCur.Amounts)),
                            Group1ID = g.FirstOrDefault().i.ItemGroup1ID,
                            Month = ToShortMonthName(g.FirstOrDefault().r.DateOut),
                            DateOut = g.FirstOrDefault().r.DateOut
                        }).ToList();
            var lists = (from s in _context.SaleARs
                         join sd in _context.SaleARDetails on s.SARID equals sd.SARID
                         join i in _context.ItemMasterDatas on sd.ItemID equals i.ID
                         join curr_sys in _context.Currency on sd.CurrencyID equals curr_sys.ID
                         group new { s, sd, i, curr_sys } by sd.ItemID into g
                         let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == g.FirstOrDefault().curr_sys.ID) ?? new Display()
                         // where IsCurrentYear(g.FirstOrDefault().s.DateOut)
                         select new MonthlySale
                         {
                             Sarid = g.Key,
                             ItemID = g.FirstOrDefault().i.ID,
                             ItemName = g.FirstOrDefault().i.KhmerName,
                             // SubTotal = g.Sum(_sd => _sd.s.SubTotal),
                             SubTotal = Convert.ToDouble(_utility.ToCurrency(g.Sum(_sd => _sd.s.SubTotal), plCur.Amounts)),
                             GrandTotal = Convert.ToDouble(_utility.ToCurrency(g.Sum(_sd => _sd.s.TotalAmount), plCur.Amounts)),
                             Group1ID = g.FirstOrDefault().i.ItemGroup1ID,
                             Month = ToShortMonthName(g.FirstOrDefault().s.PostingDate)
                             // DateOut = g.FirstOrDefault().s.DateOut
                         }).ToList();
            var listres = (from s in _context.ARReserveInvoices
                           join sd in _context.ARReserveInvoiceDetails on s.ID equals sd.ARReserveInvoiceID
                           join i in _context.ItemMasterDatas on sd.ItemID equals i.ID
                           join curr_sys in _context.Currency on sd.CurrencyID equals curr_sys.ID
                           group new { s, sd, i, curr_sys } by sd.ItemID into g
                           let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == g.FirstOrDefault().curr_sys.ID) ?? new Display()
                           // where IsCurrentYear(g.FirstOrDefault().s.DateOut)
                           select new MonthlySale
                           {
                               Sarid = g.Key,
                               ItemID = g.FirstOrDefault().i.ID,
                               ItemName = g.FirstOrDefault().i.KhmerName,
                               // SubTotal = g.Sum(_sd => _sd.s.SubTotal),
                               SubTotal = Convert.ToDouble(_utility.ToCurrency(g.Sum(_sd => _sd.s.SubTotal), plCur.Amounts)),
                               GrandTotal = Convert.ToDouble(_utility.ToCurrency(g.Sum(_sd => _sd.s.TotalAmount), plCur.Amounts)),
                               Group1ID = g.FirstOrDefault().i.ItemGroup1ID,
                               Month = ToShortMonthName(g.FirstOrDefault().s.PostingDate)
                               // DateOut = g.FirstOrDefault().s.DateOut
                           }).ToList();

            var allDetail = new List<MonthlySale>
                    (list.Count + lists.Count + listres.Count);
            allDetail.AddRange(list);
            allDetail.AddRange(lists);
            allDetail.AddRange(listres);
            return allDetail.ToList();
        }

        private TopTenSale GetReciept()
        {
            var receipts = (from r in _context.Receipt
                            join rd in _context.ReceiptDetail on r.ReceiptID equals rd.ReceiptID
                            join i in _context.ItemMasterDatas on rd.ItemID equals i.ID
                            join curr_sys in _context.Currency on r.SysCurrencyID equals curr_sys.ID
                            group new { r, rd, i } by rd.ID into g
                            where IsCurrentYear(g.FirstOrDefault().r.DateOut)
                            let receipt = g.FirstOrDefault()
                            let subTotal = g.Where(i => i.i.ID == i.rd.ItemID).Sum(_rd => _rd.rd.Total_Sys)
                            let disInvoiceValue = subTotal * receipt.r.DiscountRate / 100
                            select new MonthlySale
                            {
                                ReceiptID = receipt.r.ReceiptID,
                                ItemID = receipt.rd.ItemID,
                                ItemName = receipt.i.KhmerName,
                                SubTotal = g.Sum(_rd => _rd.rd.Total_Sys),
                                GrandTotal = g.Sum(_r => _r.r.GrandTotal_Sys),
                                Group1ID = receipt.i.ItemGroup1ID,
                                Month = ToShortMonthName(receipt.r.DateOut),
                                TotalItem = receipt.rd.Total_Sys - disInvoiceValue,
                                DateOut = receipt.r.DateOut
                            }).ToList();
            var data = new TopTenSale
            {
                Receipts = receipts
            };
            return data;
        }

        private TopTenSale GetRecieptMemo()
        {

            var rememo = (from _rememo in _context.ReceiptMemo
                          join rmemod in _context.ReceiptDetailMemoKvms on _rememo.ID equals rmemod.ReceiptMemoID
                          join i in _context.ItemMasterDatas on rmemod.ItemID equals i.ID
                          join curr_sys in _context.Currency on _rememo.SysCurrencyID equals curr_sys.ID
                          group new { _rememo, rmemod, i } by rmemod.ID into g
                          where IsCurrentYear(g.FirstOrDefault()._rememo.DateOut)
                          let receipt = g.FirstOrDefault()
                          let subTotal = g.Where(i => i.i.ID == i.rmemod.ItemID).Sum(_rd => _rd.rmemod.TotalSys)
                          let disInvoiceValue = subTotal * receipt._rememo.DisRate / 100
                          select new MonthlySale
                          {
                              ReceiptID = receipt._rememo.ReceiptKvmsID,
                              ItemID = receipt.i.ID,
                              ItemName = receipt.i.KhmerName,
                              SubTotal = g.Sum(_rd => _rd.rmemod.TotalSys),
                              GrandTotal = g.Sum(_r => _r._rememo.GrandTotalSys),
                              Group1ID = receipt.i.ItemGroup1ID,
                              Month = ToShortMonthName(receipt._rememo.DateOut),
                              TotalItem = receipt.rmemod.TotalSys - disInvoiceValue,
                              DateOut = receipt._rememo.DateOut
                          }).ToList();
            var data = new TopTenSale
            {
                ReceiptMemos = rememo
            };
            return data;
        }

        private TopTenSale GetSaleARs()
        {
            var SaleARDetail = (from r in _context.SaleARs
                                join sra in _context.SaleARDetails on r.SARID equals sra.SARID
                                join i in _context.ItemMasterDatas on sra.ItemID equals i.ID
                                group new { r, sra, i } by sra.SARDID into g
                                where IsCurrentYear(g.FirstOrDefault().r.PostingDate)
                                let receipt = g.FirstOrDefault()
                                let subTotal = g.Where(i => i.i.ID == i.sra.ItemID).Sum(_rd => _rd.sra.TotalSys)
                                let disInvoiceValue = subTotal * receipt.r.DisRate / 100
                                let totalItem = receipt.r.Status == "cancel" ? (receipt.sra.TotalSys - disInvoiceValue) * -1 : receipt.sra.TotalSys - disInvoiceValue
                                let grandTotal = receipt.r.Status == "cancel" ? g.Sum(_r => _r.r.SubTotalSys) * -1 : g.Sum(_r => _r.r.SubTotalSys)
                                let _subTotal = receipt.r.Status == "cancel" ? g.Sum(_rd => _rd.sra.TotalSys) * -1 : g.Sum(_rd => _rd.sra.TotalSys)
                                select new MonthlySale
                                {
                                    ReceiptID = receipt.r.SARID,
                                    ItemID = receipt.i.ID,
                                    ItemName = receipt.i.KhmerName,
                                    SubTotal = _subTotal,
                                    GrandTotal = grandTotal,
                                    Group1ID = receipt.i.ItemGroup1ID,
                                    Month = ToShortMonthName(receipt.r.PostingDate),
                                    TotalItem = totalItem,
                                    DateOut = receipt.r.PostingDate
                                }).ToList();
            var data = new TopTenSale
            {
                SaleARs = SaleARDetail
            };
            return data;
        }

        private TopTenSale GetSaleCreadiMemos()
        {
            var SalememoDetail = (from _smemo in _context.SaleCreditMemos
                                  join smemo in _context.SaleCreditMemoDetails on _smemo.SCMOID equals smemo.SCMOID
                                  join i in _context.ItemMasterDatas on smemo.ItemID equals i.ID
                                  group new { _smemo, smemo, i } by smemo.SCMODID into g
                                  where IsCurrentYear(g.FirstOrDefault()._smemo.PostingDate)
                                  let receipt = g.FirstOrDefault()
                                  let subTotal = g.Where(i => i.i.ID == i.smemo.ItemID).Sum(_rd => _rd.smemo.TotalSys)
                                  let disInvoiceValue = subTotal * receipt._smemo.DisRate / 100
                                  select new MonthlySale
                                  {
                                      ReceiptID = receipt._smemo.SCMOID,
                                      ItemID = receipt.i.ID,
                                      ItemName = receipt.i.KhmerName,
                                      SubTotal = g.Sum(_rd => _rd.smemo.TotalSys),
                                      GrandTotal = g.Sum(_r => _r._smemo.SubTotalSys),
                                      Group1ID = receipt.i.ItemGroup1ID,
                                      Month = ToShortMonthName(receipt._smemo.PostingDate),
                                      TotalItem = receipt.smemo.TotalSys - disInvoiceValue,
                                      DateOut = receipt._smemo.PostingDate
                                  }).ToList();
            var data = new TopTenSale
            {
                SaleCreditMemo = SalememoDetail
            };
            return data;
        }
        private TopTenSale GetSaleReserve()
        {
            var SaleARReserve = (from r in _context.ARReserveInvoices
                                 join sra in _context.ARReserveInvoiceDetails on r.ID equals sra.ARReserveInvoiceID
                                 join i in _context.ItemMasterDatas on sra.ItemID equals i.ID
                                 group new { r, sra, i } by sra.ID into g
                                 where IsCurrentYear(g.FirstOrDefault().r.PostingDate)
                                 let receipt = g.FirstOrDefault()
                                 let subTotal = g.Where(i => i.i.ID == i.sra.ItemID).Sum(_rd => _rd.sra.TotalSys)
                                 let disInvoiceValue = subTotal * receipt.r.DisRate / 100
                                 let totalItem = receipt.r.Status == "cancel" ? (receipt.sra.TotalSys - disInvoiceValue) * -1 : receipt.sra.TotalSys - disInvoiceValue
                                 let grandTotal = receipt.r.Status == "cancel" ? g.Sum(_r => _r.r.SubTotalSys) * -1 : g.Sum(_r => _r.r.SubTotalSys)
                                 let _subTotal = receipt.r.Status == "cancel" ? g.Sum(_rd => _rd.sra.TotalSys) * -1 : g.Sum(_rd => _rd.sra.TotalSys)
                                 select new MonthlySale
                                 {
                                     ReceiptID = receipt.r.ID,
                                     ItemID = receipt.i.ID,
                                     ItemName = receipt.i.KhmerName,
                                     SubTotal = _subTotal,
                                     GrandTotal = grandTotal,
                                     Group1ID = receipt.i.ItemGroup1ID,
                                     Month = ToShortMonthName(receipt.r.PostingDate),
                                     TotalItem = totalItem,
                                     DateOut = receipt.r.PostingDate
                                 }).ToList();
            var data = new TopTenSale
            {
                SaleARReserve = SaleARReserve
            };
            return data;
        }
        private TopTenSale GetSaleAREdit()
        {
            var SaleAREdit = (from r in _context.SaleAREdites
                              join sra in _context.SaleAREditeDetails on r.SARID equals sra.SARID
                              join i in _context.ItemMasterDatas on sra.ItemID equals i.ID
                              group new { r, sra, i } by sra.SARDID into g
                              where IsCurrentYear(g.FirstOrDefault().r.PostingDate)
                              let receipt = g.FirstOrDefault()
                              let subTotal = g.Where(i => i.i.ID == i.sra.ItemID).Sum(_rd => _rd.sra.TotalSys)
                              let disInvoiceValue = subTotal * receipt.r.DisRate / 100
                              let totalItem = receipt.r.Status == "cancel" ? (receipt.sra.TotalSys - disInvoiceValue) * -1 : receipt.sra.TotalSys - disInvoiceValue
                              let grandTotal = receipt.r.Status == "cancel" ? g.Sum(_r => _r.r.SubTotalSys) * -1 : g.Sum(_r => _r.r.SubTotalSys)
                              let _subTotal = receipt.r.Status == "cancel" ? g.Sum(_rd => _rd.sra.TotalSys) * -1 : g.Sum(_rd => _rd.sra.TotalSys)
                              select new MonthlySale
                              {
                                  ReceiptID = receipt.r.SARID,
                                  ItemID = receipt.i.ID,
                                  ItemName = receipt.i.KhmerName,
                                  SubTotal = _subTotal,
                                  GrandTotal = grandTotal,
                                  Group1ID = receipt.i.ItemGroup1ID,
                                  Month = ToShortMonthName(receipt.r.PostingDate),
                                  TotalItem = totalItem,
                                  DateOut = receipt.r.PostingDate
                              }).ToList();
            var data = new TopTenSale
            {
                SaleAREdit = SaleAREdit
            };
            return data;
        }

        private TopTenSale GetSaleARSercontract()
        {
            var SaleARServiceContract = (from r in _context.ServiceContracts
                                         join sra in _context.ServiceContractDetails on r.ID equals sra.ServiceContractID
                                         join i in _context.ItemMasterDatas on sra.ItemID equals i.ID
                                         group new { r, sra, i } by sra.ID into g
                                         where IsCurrentYear(g.FirstOrDefault().r.PostingDate)
                                         let receipt = g.FirstOrDefault()
                                         let subTotal = g.Where(i => i.i.ID == i.sra.ItemID).Sum(_rd => _rd.sra.TotalSys)
                                         let disInvoiceValue = subTotal * receipt.r.DisRate / 100
                                         let totalItem = receipt.r.Status == "cancel" ? (receipt.sra.TotalSys - disInvoiceValue) * -1 : receipt.sra.TotalSys - disInvoiceValue
                                         let grandTotal = receipt.r.Status == "cancel" ? g.Sum(_r => _r.r.SubTotalSys) * -1 : g.Sum(_r => _r.r.SubTotalSys)
                                         let _subTotal = receipt.r.Status == "cancel" ? g.Sum(_rd => _rd.sra.TotalSys) * -1 : g.Sum(_rd => _rd.sra.TotalSys)
                                         select new MonthlySale
                                         {
                                             ReceiptID = receipt.r.ID,
                                             ItemID = receipt.i.ID,
                                             ItemName = receipt.i.KhmerName,
                                             SubTotal = _subTotal,
                                             GrandTotal = grandTotal,
                                             Group1ID = receipt.i.ItemGroup1ID,
                                             Month = ToShortMonthName(receipt.r.PostingDate),
                                             TotalItem = totalItem,
                                             DateOut = receipt.r.PostingDate
                                         }).ToList();
            var data = new TopTenSale
            {
                SaleARServiceContract = SaleARServiceContract
            };
            return data;
        }


        private static bool IsCurrentYear(DateTime dateOut)
        {
            var firstMonth = new DateTime(DateTime.Today.Year, 1, 1);
            DateTime specifiedDate = dateOut;
            var a = specifiedDate.CompareTo(firstMonth) >= 0
                && specifiedDate.CompareTo(DateTime.Now) <= 0;
            return a;
        }

        private static bool IsCurrentYearPeriod(DateTime dateOut, string timeOut)
        {
            var firstMonth = new DateTime(DateTime.Today.Year, 1, 1);
            DateTime specifiedDate = GetFullDateTime(dateOut, timeOut);
            return specifiedDate.CompareTo(firstMonth) >= 0
                && specifiedDate.CompareTo(DateTime.Now) <= 0;
        }

        private static DateTime GetFullDateTime(DateTime dateOut, string timeout)
        {
            _ = TimeSpan.TryParse(timeout, out TimeSpan _timeOut);
            return dateOut.Add(_timeOut);
        }

        public static string ToShortMonthName(DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month);
        }
    }

    public class TopTenSale
    {
        public List<MonthlySale> Receipts { get; set; }
        public List<MonthlySale> ReceiptMemos { get; set; }
        public List<MonthlySale> SaleARs { get; set; }
        public List<MonthlySale> SaleCreditMemo { get; set; }
        public List<MonthlySale> SaleARReserve { get; set; }
        public List<MonthlySale> SaleARServiceContract { get; set; }
        public List<MonthlySale> SaleAREdit { get; set; }

    }

}
