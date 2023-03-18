using CKBS.AppContext;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using KEDI.Core.Models.Validation;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.Services.Banking;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using KEDI.Core.Premise.Repository;
using KEDI.Core.System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CKBS.Controllers
{
    [Privilege]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IncomingPaymentController : Controller
    {
        private readonly DataContext _context;
        private readonly IIncomingPayment _incoming;
        private readonly UtilityModule _utility;

        public IncomingPaymentController(DataContext context, IIncomingPayment incoming, UtilityModule utility)
        {
            _context = context;
            _incoming = incoming;
            _utility = utility;
        }
        private GeneralSettingAdminViewModel GetGeneralSettingAdmin()
        {
            Display display = _context.Displays.FirstOrDefault() ?? new Display();
            GeneralSettingAdminViewModel data = new()
            {
                Display = display
            };
            return data;

        }

        //View
        [Privilege("IP")]
        public IActionResult IncomingPayment()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Incoming Payment";
            ViewBag.Subpage = "";
            ViewBag.Banking = "show";
            ViewBag.IncomingPaymentMenu = "highlight";
            var SysCur = GetSystemCurrencies().FirstOrDefault();
            ViewBag.SysCur = SysCur.Description;
            var incomingPayment = new IncomingPayment();
            return View(new { seriesRC = _utility.GetSeries("RC"), seriesJE = _utility.GetSeries("JE"), incomingPayment, sysCur = SysCur, GeneralSetting = GetGeneralSettingAdmin().Display });
        }

        public IActionResult IncomingpaymentHistory()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Incoming Payment";
            ViewBag.Subpage = "";
            ViewBag.Banking = "show";
            ViewBag.IncomingPaymentMenu = "highlight";
            return View();
        }
        public IActionResult IncomingpaymentOrder()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Incoming PaymentOrder";
            ViewBag.Subpage = "";
            ViewBag.Banking = "show";
            ViewBag.IncomingPaymentOrderMenu = "highlight";
            var SysCur = GetSystemCurrencies().FirstOrDefault();
            ViewBag.SysCur = SysCur.Description;
            var incomingPayment = new IncomingPayment();
            return View(new { seriesRC = _utility.GetSeries("IO"), seriesJE = _utility.GetSeries("JE"), incomingPayment, sysCur = SysCur, GeneralSetting = GetGeneralSettingAdmin().Display });
        }
        public IActionResult IncomingpaymentOrderHistory()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Incoming PaymentOrderHistory";
            ViewBag.Subpage = "";
            ViewBag.Banking = "show";
            ViewBag.IncomingPaymentMenu = "highlight";
            return View();
        }
        [HttpGet]
        public IActionResult GetWarehousesIncomingPayment(int ID)
        {
            var list = _context.Warehouses.Where(x => x.BranchID == ID && x.Delete == false).ToList();
            return Ok(list);
        }

        private IEnumerable<SystemCurrency> GetSystemCurrencies()
        {
            IEnumerable<SystemCurrency> currencies = (from com in _context.Company.Where(x => x.Delete == false)
                                                      join c in _context.Currency.Where(x => x.Delete == false) on com.SystemCurrencyID equals c.ID
                                                      join dp in _context.Displays on c.ID equals dp.DisplayCurrencyID
                                                      select new SystemCurrency
                                                      {
                                                          Description = c.Description,
                                                          DecimalPlaces = dp.Amounts
                                                      });
            return currencies;
        }

        public IActionResult IncomingpaymentHistoryFilter(string status, string DateFrom, string DateTo)
        {
            var sTotal = _context.IncomingPayments.Where(i => i.CompanyID == GetCompany().ID).ToList();
            var sdetail = _context.IncomingPaymentDetails.Where(i => sTotal.Any(k => k.IncomingPaymentID == i.IncomingPaymentID)).ToList();
            double sumTotal = sdetail.Sum(s => s.Totalpayment);
            var incomingPayment = (from ICP in _context.IncomingPayments.Where(i => i.CompanyID == GetCompany().ID)
                                   join ICPD in _context.IncomingPaymentDetails on ICP.IncomingPaymentID equals ICPD.IncomingPaymentID
                                   join s in _context.Series on ICP.SeriesID equals s.ID
                                   join docType in _context.DocumentTypes on ICP.DocTypeID equals docType.ID
                                   let _currencyName = _context.Currency.FirstOrDefault(i => i.ID == ICPD.CurrencyID).Description
                                   select new
                                   {
                                       LineID = ICP.IncomingPaymentID,
                                       Invioce = $"{s.PreFix}-{ICP.InvoiceNumber}",
                                       ICPD.ItemInvoice,
                                       Date = ICP.PostingDate.ToShortDateString(),
                                       Customer = _context.BusinessPartners.FirstOrDefault(i => i.ID == ICP.CustomerID).Name,
                                       User = _context.UserAccounts.FirstOrDefault(i => i.ID == ICP.UserID).Username,
                                       ICPD.OverdueDays,
                                       TotalAmountDue = $"{_currencyName} {ICP.TotalAmountDue:0.000}",
                                       Totalpayment = $"{_currencyName} {ICPD.Totalpayment:0.000}",
                                       GrandTotal = $"{_currencyName} {sumTotal:0.000}",
                                       ICPD.ExchangeRate,
                                       ICP.Status,
                                       Currencyname = _currencyName,
                                       ICP.CompanyID,
                                       ICP.BranchID,
                                       ICP.DocTypeID,
                                       ICP.LocalCurID,
                                       ICP.LocalSetRate,
                                       ICP.PaymentMeanID,
                                       ICP.SeriesDID,
                                       ICP.SeriesID,
                                       ICP.UserID,
                                       ICP.CustomerID,
                                   }
                                    );
            var incomingpay = incomingPayment.ToList();
            if (status != "all" && DateFrom != null && DateTo != null)
            {
                incomingpay = incomingPayment.Where(i => i.Status == status && DateTime.Parse(DateFrom) <= DateTime.Parse(i.Date) && DateTime.Parse(DateTo) >= DateTime.Parse(i.Date)).ToList();
            }
            else if (status == "all" && DateFrom != null && DateTo != null)
            {
                incomingpay = incomingPayment.Where(i => i.Status != status && DateTime.Parse(DateFrom) <= DateTime.Parse(i.Date) && DateTime.Parse(DateTo) >= DateTime.Parse(i.Date)).ToList();
            }
            return Ok(incomingpay);
        }


        public IActionResult IncomingpaymentOrderHistoryFilter(string status, string DateFrom, string DateTo)
        {
            var incomingPayment = (from ICP in _context.IncomingPaymentOrders.Where(i => i.CompanyID == GetCompany().ID)
                                   join ICPD in _context.IncomingPaymentOrderDetails on ICP.ID equals ICPD.IncomingPaymentOrderID
                                   join s in _context.Series on ICP.SeriesID equals s.ID
                                   join docType in _context.DocumentTypes on ICP.DocTypeID equals docType.ID
                                   let _currencyName = _context.Currency.FirstOrDefault(i => i.ID == ICPD.CurrencyID).Description
                                   select new
                                   {
                                       LineID = ICP.ID,
                                       Invioce = $"{s.PreFix}-{ICP.InvoiceNumber}",
                                       ICPD.ItemInvoice,
                                       Date = ICP.PostingDate.ToShortDateString(),
                                       Customer = _context.BusinessPartners.FirstOrDefault(i => i.ID == ICP.CustomerID).Name,
                                       User = _context.UserAccounts.FirstOrDefault(i => i.ID == ICP.UserID).Username,
                                       ICPD.OverdueDays,
                                       TotalAmountDue = $"{_currencyName} {ICP.TotalAmountDue:0.000}",
                                       Totalpayment = $"{_currencyName} {ICPD.Totalpayment:0.000}",
                                       ICPD.ExchangeRate,
                                       ICP.Status,
                                       Currencyname = _currencyName,
                                       ICP.CompanyID,
                                       ICP.BranchID,
                                       ICP.DocTypeID,
                                       ICP.LocalCurID,
                                       ICP.LocalSetRate,
                                       ICP.PaymentMeanID,
                                       ICP.SeriesDID,
                                       ICP.SeriesID,
                                       ICP.UserID,
                                       ICP.CustomerID,
                                   }
                                    );
            var incomingpay = incomingPayment.ToList();
            if (status != "all" && DateFrom != null && DateTo != null)
            {
                incomingpay = incomingPayment.Where(i => i.Status == status && DateTime.Parse(DateFrom) <= DateTime.Parse(i.Date) && DateTime.Parse(DateTo) >= DateTime.Parse(i.Date)).ToList();
            }
            else if (status == "all" && DateFrom != null && DateTo != null)
            {
                incomingpay = incomingPayment.Where(i => i.Status != status && DateTime.Parse(DateFrom) <= DateTime.Parse(i.Date) && DateTime.Parse(DateTo) >= DateTime.Parse(i.Date)).ToList();
            }
            return Ok(incomingpay);
        }
        private void ValidateSummary(IncomingPayment master, List<IncomingPaymentDetail> detail, List<MultiIncomming> multiIncommings)
        {
            var countCheck = 0;
            var postingPeriod = _context.PostingPeriods.Where(w => w.PeroidStatus == PeroidStatus.Unlocked).ToList();
            if (postingPeriod.Count <= 0)
            {
                ModelState.AddModelError("PostingDate", "PeroiStatus is closed or locked");
            }

            else
            {
                bool isValidPostingDate = false,
                    isValidDocumentDate = false;
                foreach (var item in postingPeriod)
                {
                    if (DateTime.Compare(master.PostingDate, item.PostingDateFrom) >= 0 && DateTime.Compare(master.PostingDate, item.PostingDateTo) <= 0)
                    {
                        isValidPostingDate = true;
                    }

                    if (DateTime.Compare(master.DocumentDate, item.DocuDateFrom) >= 0 && DateTime.Compare(master.DocumentDate, item.DocuDateTo) <= 0)
                    {
                        isValidDocumentDate = true;
                    }
                }
                if (!isValidPostingDate)
                {
                    ModelState.AddModelError("PostingDate", "PostingDate is closed or locked");
                }

                if (!isValidDocumentDate)
                {
                    ModelState.AddModelError("DocumentDate", "DocumentDate is closed or locked");
                }

            }
            //if (master.PaymentMeanID == 0)
            //{
            //    ModelState.AddModelError("PaymentMeanID", "Please choose any Account.");
            //}
            if (master.TotalAmountDue < 0)
            {
                ModelState.AddModelError("TotalAmountDue", "Applied Amount <0 Please Input Enough​ Amount");

            }
            if (master.CustomerID == 0)
            {
                ModelState.AddModelError("CustomerID", "Please choose any customer.");
            }

            if (detail.Count == 0)
            {
                ModelState.AddModelError("Details", "Please select at least one invoice.");
            }
            foreach (var value in detail)
            {
                if (value.CheckPay == true)
                {
                    countCheck++;
                }
            }
            if (multiIncommings.Count == 0)
            {
                ModelState.AddModelError("multiIncommings", "Please input amount in payment Means");

            }
            decimal totalpay = 0;
            foreach (var mul in multiIncommings)
            {
                totalpay += mul.AmmountSys;

            }
            if (totalpay < (decimal)master.TotalAmountDue)
            {
                ModelState.AddModelError("AmmountSys", "Please Input Enough Amount.");

            }
            if (countCheck == 0)
            {
                ModelState.AddModelError("Check", "Please Check At Least One Invoice.");
            }
        }
        private void ValidateSummaryIMOrder(IncomingPaymentOrder master, List<IncomingPaymentOrderDetail> detail, List<MultiIncomingPaymentOrder> multiIncommings)
        {
            var countCheck = 0;
            var postingPeriod = _context.PostingPeriods.Where(w => w.PeroidStatus == PeroidStatus.Unlocked).ToList();
            if (postingPeriod.Count <= 0)
            {
                ModelState.AddModelError("PostingDate", "PeroiStatus is closed or locked");
            }

            else
            {
                bool isValidPostingDate = false,
                    isValidDocumentDate = false;
                foreach (var item in postingPeriod)
                {
                    if (DateTime.Compare(master.PostingDate, item.PostingDateFrom) >= 0 && DateTime.Compare(master.PostingDate, item.PostingDateTo) <= 0)
                    {
                        isValidPostingDate = true;
                    }

                    if (DateTime.Compare(master.DocumentDate, item.DocuDateFrom) >= 0 && DateTime.Compare(master.DocumentDate, item.DocuDateTo) <= 0)
                    {
                        isValidDocumentDate = true;
                    }
                }
                if (!isValidPostingDate)
                {
                    ModelState.AddModelError("PostingDate", "PostingDate is closed or locked");
                }

                if (!isValidDocumentDate)
                {
                    ModelState.AddModelError("DocumentDate", "DocumentDate is closed or locked");
                }

            }

            if (master.TotalAmountDue < 0)
            {
                ModelState.AddModelError("TotalAmountDue", "Applied Amount <0 Please Input Enough​ Amount");

            }
            if (master.CustomerID == 0)
            {
                ModelState.AddModelError("CustomerID", "Please choose any customer.");
            }

            if (detail.Count == 0)
            {
                ModelState.AddModelError("Details", "Please select at least one invoice.");
            }
            foreach (var value in detail)
            {
                if (value.CheckPay == true)
                {
                    countCheck++;
                }
            }
            if (multiIncommings.Count == 0)
            {
                ModelState.AddModelError("multiIncommings", "Please input amount in payment Means");

            }
            decimal totalpay = 0;
            foreach (var mul in multiIncommings)
            {
                totalpay += mul.AmmountSys;

            }
            if (totalpay < (decimal)master.TotalAmountDue)
            {
                ModelState.AddModelError("AmmountSys", "Please Input Enough Amount.");

            }
            if (countCheck == 0)
            {
                ModelState.AddModelError("Check", "Please Check At Least One Invoice.");
            }
        }



        private int GetUserID()
        {
            _ = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _id);
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
        public IActionResult GetCustomers()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type != "Vendor").ToList();
            return Ok(list);
        }
        private List<Currency> Currency()
        {
            var data = _context.Currency.ToList();
            data.Insert(0, new Currency
            {
                Description = data.FirstOrDefault().Description,

                ID = data.FirstOrDefault().ID,
            });
            return data;
        }

        [HttpGet]
        public IActionResult GetPaymentMeans()
        {
            var paymentMeans = (from pm in _context.PaymentMeans.Where(i => !i.Delete && i.CompanyID == GetCompany().ID)
                                join glAcc in _context.GLAccounts on pm.AccountID equals glAcc.ID
                                let ex = _context.ExchangeRates.FirstOrDefault(x => x.ID == Currency().FirstOrDefault().ID) ?? new ExchangeRate()
                                select new MultiIncomming
                                {
                                    ID = 0,
                                    LineID = pm.ID,
                                    PaymentMeanID = pm.ID,
                                    SCRate = (decimal)ex.SetRate,
                                    Amount = 0,
                                    AmmountSys = 0,
                                    PMName = pm.Type,
                                    Currency = Currency().GroupBy(i => i.ID).Select(i => i.FirstOrDefault()).Select(i => new SelectListItem
                                    {
                                        Text = i.Description, //+ i.Name,
                                        Value = i.ID.ToString(),
                                    }).ToList(),
                                    ExchangeRate = (decimal)ex.Rate,

                                    GLAccID = glAcc.ID,
                                    CurrID = Currency().FirstOrDefault().ID

                                }
                               ).ToList();
            return Ok(paymentMeans);
        }

        [HttpGet]
        public IActionResult GetExchangeRateFirstDefault(int id)
        {
            var ex = _context.ExchangeRates.FirstOrDefault(x => x.CurrencyID == id) ?? new ExchangeRate();
            return Ok(ex);
        }
        [HttpGet]
        public IActionResult GetExchangeRate(int id)
        {
            var data = (from curr in _context.Currency.Where(x => x.ID == id)
                        join ex in _context.ExchangeRates on curr.ID equals ex.CurrencyID
                        select new ExchangeRate
                        {
                            ID = ex.ID,
                            Rate = ex.Rate,
                            SetRate = ex.SetRate

                        }).ToList();
            return Ok(data);
        }

        [HttpGet]
        public IActionResult GetPaymentMeansDefault()
        {
            var paymentMeans = (from pm in _context.PaymentMeans.Where(i => i.Default == true && !i.Delete && i.CompanyID == GetCompany().ID)
                                join glAcc in _context.GLAccounts on pm.AccountID equals glAcc.ID
                                select new
                                {
                                    PaymentMeanID = pm.ID,
                                    PMName = pm.Type,
                                    GLAccName = glAcc.Name,
                                    GLAccCode = glAcc.Code,
                                }
                                ).ToList();
            return Ok(paymentMeans);
        }

        [HttpGet]
        public IActionResult GetSaleAR(int customerID, string type)
        {
            var incomepay = _context.IncomingPaymentCustomers.Where(i => i.CompanyID == GetCompany().ID && i.CustomerID == customerID && i.Status == "open").ToList();
            foreach (var obj in incomepay)
            {
                if (obj.BalanceDue <= 0)
                {
                    obj.Status = "Close";
                }
            }
            _context.UpdateRange(incomepay);
            _context.SaveChanges();
            var list = from p in incomepay.Where(i => i.CompanyID == GetCompany().ID && i.CustomerID == customerID && i.Status == "open" && i.BalanceDue > 0)
                       join b in _context.BusinessPartners on p.CustomerID equals b.ID
                       join br in _context.Branches on p.BranchID equals br.ID
                       join w in _context.Warehouses on p.WarehouseID equals w.ID
                       join c in _context.Currency on p.CurrencyID equals c.ID
                       join c_s in _context.Currency on p.SysCurrency equals c_s.ID
                       join doc in _context.DocumentTypes on p.DocTypeID equals doc.ID
                       let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == c.ID) ?? new Display()
                       // note whenever you add new prop, you have to put it or them at the end
                       select new
                       {
                           p.IncomingPaymentCustomerID,
                           CustomerID = b.ID,
                           BranchID = br.ID,
                           WarehouseID = w.ID,
                           CurrencyID = c.ID,
                           Invoice = $"{doc.Code}-{p.InvoiceNumber}",
                           DocumentTypeIDValue = doc.Code,
                           Date = p.Date.ToShortDateString(),
                           OverdueDays = (p.Date.Date - DateTime.Now.Date).Days,
                           Total = Convert.ToDouble(_utility.ToCurrency(p.Total, plCur.Amounts)),
                           Totals = $"{c.Description} {_utility.ToCurrency(p.Total, plCur.Amounts)}",
                           Applied_Amount = Convert.ToDouble(_utility.ToCurrency(p.Applied_Amount, plCur.Amounts)),
                           Applied_Amounts = $"{c.Description} {_utility.ToCurrency(p.Applied_Amount, plCur.Amounts)}",
                           BalanceDue = Convert.ToDouble(_utility.ToCurrency(p.BalanceDue, plCur.Amounts)),
                           BalanceDues = $"{c.Description} {_utility.ToCurrency(p.BalanceDue, plCur.Amounts)}",
                           CashDiscount = Convert.ToDouble(_utility.ToCurrency(p.CashDiscount, plCur.Amounts)),
                           CashDiscounts = $"{c.Description} {_utility.ToCurrency(p.CashDiscount, plCur.Amounts)}",
                           TotalDiscount = Convert.ToDouble(_utility.ToCurrency(p.TotalDiscount, plCur.Amounts)),
                           TotalDiscounts = $"{c.Description} {_utility.ToCurrency(p.TotalDiscount, plCur.Amounts)}",
                           TotalPayment = Convert.ToDouble(_utility.ToCurrency(p.TotalPayment, plCur.Amounts)),
                           TotalPayments = $"{c.Description} {_utility.ToCurrency(p.BalanceDue, plCur.Amounts)}",
                           CurrencyName = c.Description,
                           SysName = c_s.Description,
                           p.Status,
                           p.ExchangeRate,
                           p.SysCurrency,
                           p.InvoiceNumber,
                           Prefix = doc.Code,
                           p.DocTypeID,
                           p.ItemInvoice,
                           DocNo = doc.Code,
                           CheckPay = false,
                           IcoPayCusID = p.IncomingPaymentCustomerID,
                           p.LocalCurID,
                           p.LocalSetRate,
                           p.Types,
                       };
            if (type == "all")
            {
                return Ok(list);
            }
            if (type != "all")
            {
                var doctype = _context.DocumentTypes.FirstOrDefault(i => i.Code == type) ?? new DocumentType();
                return Ok(list.Where(i => i.DocTypeID == doctype.ID));
            }
            return Ok();
        }

        //Save Data
        [HttpPost]
        public IActionResult SaveIncomingPayment(string incoming, string je)
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            if (je == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(msg.Bind(ModelState));
            }
            IncomingPayment incomingpay = JsonConvert.DeserializeObject<IncomingPayment>(incoming, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Series seriesje = JsonConvert.DeserializeObject<Series>(je, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var seriesRC = _context.Series.FirstOrDefault(w => w.ID == incomingpay.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == incomingpay.DocTypeID).FirstOrDefault();
            //var seriesJE = _context.Series.FirstOrDefault(w => w.ID == seriesje.ID);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            incomingpay.MultiIncommings = incomingpay.MultiIncommings
               .Where(i => i.Amount > 0).ToList();

            ValidateSummary(incomingpay, incomingpay.IncomingPaymentDetails, incomingpay.MultiIncommings);

            if (ModelState.IsValid)
            {
                using var t = _context.Database.BeginTransaction();
                seriesDetail.Number = seriesRC.NextNo;
                seriesDetail.SeriesID = incomingpay.SeriesID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDetail.Number;
                long No = long.Parse(Sno);
                seriesRC.NextNo = Convert.ToString(No + 1);
                if (No > long.Parse(seriesRC.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Incoming Payment Invoice has reached the limitation!!");
                    return Ok(msg.Bind(ModelState));
                }
                incomingpay.LocalCurID = GetCompany().LocalCurrencyID;
                incomingpay.LocalSetRate = localSetRate;
                incomingpay.SeriesDID = seriesDetailID;
                incomingpay.CompanyID = GetCompany().ID;
                incomingpay.Status = "open";
                incomingpay.Number = seriesDetail.Number;
                incomingpay.InvoiceNumber = seriesDetail.Number;
                _context.IncomingPayments.Add(incomingpay);
                _context.SaveChanges();
                if (!_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                {
                    _incoming.IncomingPaymentSeriesAccounting(incomingpay.IncomingPaymentID);
                }

                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                foreach (var item in incomingpay.IncomingPaymentDetails.ToList())
                {
                    var plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == item.CurrencyID) ?? new Display();
                    var doctypeCode = _context.DocumentTypes.Find(item.DocTypeID);
                    var incomingcus = _context.IncomingPaymentCustomers.FirstOrDefault(x => x.IncomingPaymentCustomerID == item.IcoPayCusID) ?? new IncomingPaymentCustomer();
                    if (incomingcus.Types == "SC")
                    {
                        var servicear = _context.ServiceContracts.FirstOrDefault(x => x.SeriesDID == incomingcus.SeriesDID) ?? new ServiceContract();
                        servicear.AppliedAmount += item.Totalpayment + item.TotalDiscount;
                        if (servicear.AppliedAmount == servicear.TotalAmount)
                        {
                            servicear.Status = "Close";
                        }
                        _context.ServiceContracts.Update(servicear);
                        _context.SaveChanges();
                    }
                    if (incomingcus.Types == SaleCopyType.ARReserveInvoice.ToString())
                    {
                        var aRReserveInvoice = _context.ARReserveInvoices.FirstOrDefault(x => x.SeriesDID == incomingcus.SeriesDID) ?? new ARReserveInvoice();
                        aRReserveInvoice.AppliedAmount += item.Totalpayment + item.TotalDiscount;
                        // if (aRReserveInvoice.AppliedAmount == aRReserveInvoice.TotalAmount)
                        // {
                        //     aRReserveInvoice.Status = "Close";
                        // }
                        _context.ARReserveInvoices.Update(aRReserveInvoice);
                        _context.SaveChanges();
                    }
                    if (incomingcus.Types == SaleCopyType.ARReserveInvoiceEDT.ToString())
                    {
                        var aRReserveInvoice = _context.ARReserveInvoiceEditables.FirstOrDefault(x => x.SeriesDID == incomingcus.SeriesDID) ?? new ARReserveInvoiceEditable();
                        aRReserveInvoice.AppliedAmount += item.Totalpayment + item.TotalDiscount;
                        // if (aRReserveInvoice.AppliedAmount == aRReserveInvoice.TotalAmount)
                        // {
                        //     aRReserveInvoice.Status = "Close";
                        // }
                        _context.ARReserveInvoiceEditables.Update(aRReserveInvoice);
                        _context.SaveChanges();
                    }
                    if (doctypeCode.Code == "IN")
                    {
                        if (incomingcus.Types == SaleCopyType.SaleAREdite.ToString())
                        {
                            var salearedit = _context.SaleAREdites.FirstOrDefault(s => s.SeriesDID == incomingcus.SeriesDID) ?? new SaleAREdite();
                            salearedit.AppliedAmount += item.Totalpayment + item.TotalDiscount;
                            if (salearedit.AppliedAmount == salearedit.TotalAmount)
                            {
                                salearedit.Status = "Close";
                                _context.SaleAREdites.Update(salearedit);
                                _context.SaveChanges();
                            }
                        }
                        if (incomingcus.Types == SaleCopyType.AR.ToString())
                        {
                            var salear = _context.SaleARs.FirstOrDefault(x => x.SeriesDID == incomingcus.SeriesDID) ?? new SaleAR();
                            salear.AppliedAmount += item.Totalpayment + item.TotalDiscount;
                            if (salear.AppliedAmount == salear.TotalAmount)
                            {
                                salear.Status = "Close";
                                _context.SaleARs.Update(salear);
                                _context.SaveChanges();
                            }
                        }



                    }
                    else if (doctypeCode.Code == "SP")
                    {
                        //Receipt
                        var receipt = _context.Receipt.FirstOrDefault(x => x.SeriesDID == incomingcus.SeriesDID);
                        receipt.AppliedAmount += (decimal)(item.Totalpayment + item.TotalDiscount);
                        receipt.Received = (double)receipt.AppliedAmount;
                        receipt.BalanceReturn = receipt.AppliedAmount;
                        _context.Receipt.Update(receipt);
                        _context.SaveChanges();
                    }
                    else if (doctypeCode.Code == "CN")
                    {
                        var saleMemo = _context.SaleCreditMemos.FirstOrDefault(x => x.SeriesDID == incomingcus.SeriesDID);
                        saleMemo.AppliedAmount += item.Totalpayment + item.TotalDiscount;
                        if (saleMemo.AppliedAmount == saleMemo.TotalAmount)
                        {
                            saleMemo.Status = "Close";
                        }
                        _context.SaleCreditMemos.Update(saleMemo);
                        _context.SaveChanges();
                    }
                    else if (doctypeCode.Code == "CD")
                    {
                        var ard = _context.ARDownPayments.FirstOrDefault(x => x.SeriesDID == incomingcus.SeriesDID);
                        ard.AppliedAmount += (decimal)(item.Totalpayment + item.TotalDiscount);
                        if (ard.AppliedAmount >= (decimal)ard.TotalAmount)
                        {
                            ard.Status = "Close";
                        }
                        _context.ARDownPayments.Update(ard);
                        _context.SaveChanges();
                    }
                    //IncomingPaymentCustomers 
                    incomingcus.Applied_Amount += Convert.ToDouble(_utility.ToCurrency(item.Totalpayment + item.TotalDiscount, plCur.Amounts));
                    incomingcus.BalanceDue -= Convert.ToDouble(_utility.ToCurrency(item.Totalpayment + item.TotalDiscount, plCur.Amounts));
                    incomingcus.TotalPayment -= Convert.ToDouble(_utility.ToCurrency(item.Totalpayment + item.TotalDiscount, plCur.Amounts));
                    if (Convert.ToDouble(_utility.ToCurrency(incomingcus.Applied_Amount, plCur.Amounts)) >= Convert.ToDouble(_utility.ToCurrency(incomingcus.Total, plCur.Amounts)))
                    {
                        incomingcus.Status = "Close";
                        incomingpay.Status = "close";
                    }
                    //IncomingPaymentDetails
                    item.Applied_Amount = incomingcus.Applied_Amount;
                    item.BalanceDue -= Convert.ToDouble(_utility.ToCurrency(item.Totalpayment + item.TotalDiscount, plCur.Amounts));
                    incomingpay.TotalAmountDue = incomingcus.BalanceDue;
                    _context.IncomingPayments.Update(incomingpay);
                    _context.IncomingPaymentCustomers.Update(incomingcus);
                    //                                                                  
                    var curName = _context.Currency.Find(item.CurrencyID).Description;
                    item.CurrencyName = curName;
                    _context.IncomingPayments.Update(incomingpay);
                    _context.SaveChanges();
                    msg.Action = ModelAction.Approve;
                }
                incomingpay.TotalApplied = (decimal)incomingpay.IncomingPaymentDetails.Sum(s => (s.Totalpayment + s.TotalDiscount) * s.ExchangeRate);
                _context.IncomingPayments.Update(incomingpay);
                _context.Series.Update(seriesRC);
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                msg.Action = ModelAction.Approve;
                ModelState.AddModelError("Success", "You have successfully paid!!");
                t.Commit();
            }
            return Ok(new { Model = msg.Bind(ModelState) });
        }

        [HttpGet]
        [Route("/incomingpayment/getbyinvoice")]
        public IActionResult GetByInvoice(string invoiceNumber, int seriesID)
        {
            var ICP = _context.IncomingPayments.FirstOrDefault(i => i.InvoiceNumber == invoiceNumber && i.CompanyID == GetCompany().ID && i.SeriesID == seriesID && i.Status == "open");
            if (ICP != null)
            {
                var details = _context.IncomingPaymentDetails.Include(i => i.Currency).Where(i => i.IncomingPaymentID == ICP.IncomingPaymentID).ToList();
                var bus = _context.BusinessPartners.FirstOrDefault(i => i.ID == ICP.CustomerID) ?? new Models.Services.HumanResources.BusinessPartner();
                var user = _context.UserAccounts.FirstOrDefault(i => i.ID == ICP.UserID) ?? new UserAccount();
                var payMentmeans = _context.PaymentMeans.FirstOrDefault(i => i.ID == ICP.PaymentMeanID) ?? new PaymentMeans();
                var GLAccount = _context.GLAccounts.FirstOrDefault(i => i.ID == payMentmeans.AccountID) ?? new Models.Services.ChartOfAccounts.GLAccount();

                var icomingPayment = new IncomingPamentCancelViewModel
                {
                    IncomingPaymentID = ICP.IncomingPaymentID,
                    UserID = ICP.UserID,
                    SeriesID = ICP.SeriesID,
                    SeriesDID = ICP.SeriesDID,
                    DocTypeID = ICP.DocTypeID,
                    CompanyID = ICP.CompanyID,
                    InvoiceNumber = ICP.InvoiceNumber,
                    Status = ICP.Status,
                    PaymentMeanID = ICP.PaymentMeanID,
                    CustomerID = ICP.CustomerID,
                    BranchID = ICP.BranchID,
                    Ref_No = ICP.Ref_No,
                    PostingDate = ICP.PostingDate,
                    DocumentDate = ICP.DocumentDate,
                    TotalAmountDue = ICP.TotalAmountDue,
                    Number = ICP.Number,
                    Remark = ICP.Remark,
                    IncomingPaymentDetails = details,
                    LocalCurID = ICP.LocalCurID,
                    LocalSetRate = ICP.LocalSetRate,
                    BusinessPartner = bus,
                    UserAccount = user,
                    PaymentMeans = payMentmeans,
                    GLAccount = GLAccount,
                    MultPayIncommings = (from incom in _context.MultiIncommings.Where(i => i.IncomingPaymentID == ICP.IncomingPaymentID)
                                         join paymean in _context.PaymentMeans on incom.PaymentMeanID equals paymean.ID
                                         join glAcc in _context.GLAccounts on paymean.AccountID equals glAcc.ID
                                         let ex = _context.ExchangeRates.FirstOrDefault(x => x.ID == Currency().FirstOrDefault().ID) ?? new ExchangeRate()
                                         select new MultiIncomming
                                         {
                                             ID = incom.ID,
                                             LineID = incom.ID,
                                             IncomingPaymentID = incom.IncomingPaymentID,
                                             PaymentMeanID = incom.PaymentMeanID,
                                             SCRate = (decimal)ex.SetRate,
                                             Amount = incom.Amount,
                                             AmmountSys = incom.AmmountSys,
                                             PMName = paymean.Type,
                                             Currency = Currency().GroupBy(i => i.ID).Select(i => i.FirstOrDefault()).Select(i => new SelectListItem
                                             {
                                                 Text = i.Description, //+ i.Name,
                                                 Value = i.ID.ToString(),
                                                 Selected = incom.CurrID == i.ID,
                                             }).ToList(),
                                             ExchangeRate = (decimal)ex.Rate,

                                             GLAccID = glAcc.ID,
                                             CurrID = Currency().FirstOrDefault().ID

                                         }).ToList(),
                };
                return Ok(icomingPayment);
            }
            else
            {
                return Ok(new { Error = "Invoice Not Found!!! " });
            }
        }

        public async Task<IActionResult> FindPaymentOrder(string invoiceNumber, int seriesID)
        {
            int id = 0;
            var obj = await _incoming.GetPaymentOrder(id, invoiceNumber, seriesID);
            if (obj != null)
            {
                return Ok(obj);
            }
            else
            {
                return Ok(new { Error = "Invoice Not Found!!! " });
            }
        }
        [HttpPost]
        [Route("/incomingpaymenthistory/cancel")]
        public IActionResult Cancel(string invoice, string seriesJE, int seriesDID)
        {
            ModelMessage msg = new();
            if (seriesJE == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(msg.Bind(ModelState));
            }
            Series series_JE = JsonConvert.DeserializeObject<Series>(seriesJE, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var incoming = _context.IncomingPayments.FirstOrDefault(i => i.SeriesDID == seriesDID) ?? new IncomingPayment();
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            //var _seriesPS = _context.Series.FirstOrDefault(w => w.ID == series_PS.ID);
            var douType = _context.DocumentTypes.Where(i => i.ID == incoming.DocTypeID).FirstOrDefault();
            var _seriesJE = _context.Series.FirstOrDefault(w => w.ID == series_JE.ID);
            var checkpay = _context.IncomingPaymentDetails.Where(x => x.IncomingPaymentID == incoming.IncomingPaymentID && x.Delete == false).ToList();
            if (!_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
            {
                _incoming.IncomingPaymentSeriesAccountingCancel(invoice, incoming.SeriesDID);
            }

            // checking maximun Invoice
            var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
            var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
            if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
            {
                ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                return Ok(new { Model = msg.Bind(ModelState) });
            }
            using (var t = _context.Database.BeginTransaction())
            {
                foreach (var item in checkpay)
                {
                    var customer = _context.IncomingPaymentCustomers.FirstOrDefault(x => x.IncomingPaymentCustomerID == item.IcoPayCusID);
                    if (item.DocNo == "IN")
                    {
                        if (customer.Types == SaleCopyType.AR.ToString())
                        {
                            var _AR = _context.SaleARs.FirstOrDefault(x => x.SeriesDID == customer.SeriesDID) ?? new SaleAR();
                            _AR.AppliedAmount -= item.Totalpayment;
                            _AR.Status = "open";
                            _context.SaleARs.Update(_AR);
                        }
                        else if (customer.Types == SaleCopyType.SaleAREdite.ToString())
                        {
                            var _AR = _context.SaleAREdites.FirstOrDefault(x => x.SeriesDID == customer.SeriesDID) ?? new SaleAREdite();
                            _AR.AppliedAmount -= item.Totalpayment;
                            _AR.Status = "open";
                            _context.SaleAREdites.Update(_AR);
                        }
                        else if (customer.Types == SaleCopyType.ARReserveInvoice.ToString())
                        {
                            var _AR = _context.ARReserveInvoices.FirstOrDefault(x => x.SeriesDID == customer.SeriesDID) ?? new ARReserveInvoice();
                            _AR.AppliedAmount -= item.Totalpayment;
                            _AR.Status = "open";
                            _context.ARReserveInvoices.Update(_AR);
                        }
                        else if (customer.Types == SaleCopyType.ARReserveInvoiceEDT.ToString())
                        {
                            var _AR = _context.ARReserveInvoiceEditables.FirstOrDefault(x => x.SeriesDID == customer.SeriesDID) ?? new ARReserveInvoiceEditable();
                            _AR.AppliedAmount -= item.Totalpayment;
                            _AR.Status = "open";
                            _context.ARReserveInvoiceEditables.Update(_AR);
                        }
                    }
                    if (item.DocNo == "SP")
                    {
                        var sp = _context.Receipt.FirstOrDefault(x => x.SeriesDID == customer.SeriesDID) ?? new Models.Services.POS.Receipt();
                        sp.Received -= item.Totalpayment;
                        sp.AppliedAmount -= (decimal)item.Totalpayment;
                        _context.Receipt.Update(sp);
                    }
                    if (item.DocNo == "CD")
                    {
                        var sp = _context.ARDownPayments.FirstOrDefault(x => x.SeriesDID == customer.SeriesDID) ?? new ARDownPayment();
                        sp.AppliedAmount -= (decimal)item.Totalpayment;
                        _context.ARDownPayments.Update(sp);
                    }
                    if (item.DocNo == "CN")
                    {
                        var memo = _context.SaleCreditMemos.FirstOrDefault(w => w.SeriesDID == customer.SeriesDID) ?? new SaleCreditMemo();
                        if (memo.BasedOn == 0)
                        {
                            memo.AppliedAmount -= item.Totalpayment * -1;
                        }
                        else
                        {
                            memo.AppliedAmount -= item.Totalpayment;
                        }
                        memo.Status = "open";
                        _context.SaleCreditMemos.Update(memo);
                    }
                    customer.Applied_Amount -= item.Totalpayment;
                    customer.BalanceDue += item.Totalpayment;
                    customer.TotalPayment += item.Totalpayment;
                    customer.Status = "open";
                    item.Delete = false;
                    _context.IncomingPaymentDetails.Update(item);
                    _context.IncomingPaymentCustomers.Update(customer);
                    _context.SaveChanges();
                }
                incoming.Status = "cancel";
                _context.IncomingPayments.Update(incoming);
                _context.SaveChanges();
                msg.Action = ModelAction.Approve;
                ModelState.AddModelError("Success", $"You have successfully canceled this PS-{invoice} invoice!!");
                t.Commit();
            }
            return Ok(msg.Bind(ModelState));
        }

        [HttpPost]
        public IActionResult CancelInvoicePaymentOrder(int id, string remark)
        {
            ModelMessage msg = new();
            var incoming = _context.IncomingPaymentOrders.FirstOrDefault(s => s.ID == id);
            if (incoming != null)
            {
                incoming.Remark = string.IsNullOrWhiteSpace(remark) ? incoming.Remark : remark;
            }

            using (var t = _context.Database.BeginTransaction())
            {

                incoming.Status = "cancel";
                _context.IncomingPaymentOrders.Update(incoming);
                _context.SaveChanges();
                msg.Action = ModelAction.Approve;
                ModelState.AddModelError("Success", $"You have successfully canceled this PS-{incoming.Number} invoice!!");
                t.Commit();
            }
            return Ok(msg.Bind(ModelState));
        }
        // Save incoming Paymentorder
        [HttpPost]
        public IActionResult SaveIncomingPaymentOrder(string incoming, string je)
        {
            ModelMessage msg = new();
            SeriesDetail seriesDetail = new();
            if (je == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(msg.Bind(ModelState));
            }
            IncomingPaymentOrder incomingpay = JsonConvert.DeserializeObject<IncomingPaymentOrder>(incoming, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            Series seriesje = JsonConvert.DeserializeObject<Series>(je, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            var seriesRC = _context.Series.FirstOrDefault(w => w.ID == incomingpay.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == incomingpay.DocTypeID).FirstOrDefault();
            //var seriesJE = _context.Series.FirstOrDefault(w => w.ID == seriesje.ID);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            incomingpay.MultiIncomingPaymentOrders = incomingpay.MultiIncomingPaymentOrders.Where(i => i.Amount > 0).ToList();

            ValidateSummaryIMOrder(incomingpay, incomingpay.IncomingPaymentOrderDetails, incomingpay.MultiIncomingPaymentOrders);

            if (ModelState.IsValid)
            {
                using var t = _context.Database.BeginTransaction();
                seriesDetail.Number = seriesRC.NextNo;
                seriesDetail.SeriesID = incomingpay.SeriesID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDetail.Number;
                long No = long.Parse(Sno);
                seriesRC.NextNo = Convert.ToString(No + 1);
                if (No > long.Parse(seriesRC.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Incoming Payment Invoice has reached the limitation!!");
                    return Ok(msg.Bind(ModelState));
                }
                incomingpay.LocalCurID = GetCompany().LocalCurrencyID;
                incomingpay.LocalSetRate = localSetRate;
                incomingpay.SeriesDID = seriesDetailID;
                incomingpay.CompanyID = GetCompany().ID;
                incomingpay.Status = "open";
                incomingpay.Number = seriesDetail.Number;
                incomingpay.InvoiceNumber = seriesDetail.Number;
                _context.IncomingPaymentOrders.Add(incomingpay);
                _context.SaveChanges();
                //if (!_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                //{
                //    _incoming.IncomingPaymentSeriesAccounting(incomingpay.IncomingPaymentID);
                //}

                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                foreach (var item in incomingpay.IncomingPaymentOrderDetails.ToList())
                {
                    var plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == item.CurrencyID) ?? new Display();
                    var doctypeCode = _context.DocumentTypes.Find(item.DocTypeID);
                    var incomingcus = _context.IncomingPaymentCustomers.AsNoTracking().FirstOrDefault(x => x.IncomingPaymentCustomerID == item.IcoPayCusID) ?? new IncomingPaymentCustomer();

                    //IncomingPaymentCustomers 
                    incomingcus.Applied_Amount += Convert.ToDouble(_utility.ToCurrency(item.Totalpayment + item.TotalDiscount, plCur.Amounts));
                    incomingcus.BalanceDue -= Convert.ToDouble(_utility.ToCurrency(item.Totalpayment + item.TotalDiscount, plCur.Amounts));
                    incomingcus.TotalPayment -= Convert.ToDouble(_utility.ToCurrency(item.Totalpayment + item.TotalDiscount, plCur.Amounts));

                    // if (Convert.ToDouble(_utility.ToCurrency(incomingcus.Applied_Amount, plCur.Amounts)) >= Convert.ToDouble(_utility.ToCurrency(incomingcus.Total, plCur.Amounts)))
                    // {
                    //     incomingcus.Status = "Close";
                    //     incomingpay.Status = "close";
                    // }
                    //IncomingPaymentDetails
                    item.Applied_Amount = incomingcus.Applied_Amount;
                    item.BalanceDue -= Convert.ToDouble(_utility.ToCurrency(item.Totalpayment + item.TotalDiscount, plCur.Amounts));
                    incomingpay.TotalAmountDue = incomingcus.BalanceDue;
                    item.OpenTotalpayment = item.Totalpayment;
                    _context.IncomingPaymentOrders.Update(incomingpay);
                    //_context.IncomingPaymentCustomers.Update(incomingcus);
                    //                                                                  
                    var curName = _context.Currency.Find(item.CurrencyID).Description;
                    item.CurrencyName = curName;
                    _context.IncomingPaymentOrders.Update(incomingpay);
                    _context.SaveChanges();
                    msg.Action = ModelAction.Approve;
                }
                incomingpay.TotalApplied = (decimal)incomingpay.IncomingPaymentOrderDetails.Sum(s => (s.Totalpayment + s.TotalDiscount) * s.ExchangeRate);
                _context.IncomingPaymentOrders.Update(incomingpay);
                _context.Series.Update(seriesRC);
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                msg.Action = ModelAction.Approve;
                ModelState.AddModelError("Success", "You have successfully paid!!");
                t.Commit();
            }
            return Ok(new { Model = msg.Bind(ModelState) });

        }

        public async Task<IActionResult> GetIncomingPayOrder(int cusid)
        {
            var list = await (from ipo in _context.IncomingPaymentOrders.Where(s => s.CompanyID == GetCompany().ID && s.CustomerID == cusid && s.Status == "open")
                              join doc in _context.DocumentTypes on ipo.DocTypeID equals doc.ID
                              join businesspatner in _context.BusinessPartners on ipo.CustomerID equals businesspatner.ID
                              select new
                              {
                                  ID = ipo.ID,
                                  InvoiceNumber = ipo.Number,
                                  DocumentType = doc.Code,
                                  Customer = businesspatner.Name,
                                  PostingDate = ipo.PostingDate.ToString("dd/MM/yyyy"),
                                  TotalAmountDue = ipo.TotalAmountDue,
                                  ApplyAmount = ipo.TotalApplied,
                              }

            ).ToListAsync();
            return Ok(list);
        }
        public async Task<IActionResult> CopyIncomingPayOrder(int id)
        {


            var obj = await _incoming.GetPaymentOrder(id, "", 0);
            obj.MultPayIncommings.ForEach(s =>
            {
                s.ID = 0;
            });
            //var list=from ob in obj.
            var objs = new
            {
                incompay = obj,
                ConpyType = ConpyType.IncomingPaymentOrder,
                IncompayOrderDetail = from li in obj.IncomingPaymentOrderDetails
                                      join c in _context.Currency on li.CurrencyID equals c.ID
                                      join incustmer in _context.IncomingPaymentCustomers on li.IcoPayCusID equals incustmer.IncomingPaymentCustomerID
                                      join doc in _context.DocumentTypes on incustmer.DocTypeID equals doc.ID
                                      join c_s in _context.Currency on incustmer.SysCurrency equals c_s.ID
                                      let plCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == c.ID) ?? new Display()
                                      select new
                                      {
                                          IncomingPaymentCustomerID = li.ID,
                                          BranchID = incustmer.BranchID,
                                          WarehouseID = incustmer.WarehouseID,
                                          CurrencyID = li.CurrencyID,
                                          Invoice = li.ItemInvoice,
                                          DocumentTypeIDValue = li.DocNo,
                                          Date = li.Date.ToShortDateString(),
                                          OverdueDays = li.OverdueDays,
                                          Total = Convert.ToDouble(_utility.ToCurrency(li.Total, plCur.Amounts)),
                                          Totals = $"{c.Description} {_utility.ToCurrency(li.Total, plCur.Amounts)}",
                                          Applied_Amount = Convert.ToDouble(_utility.ToCurrency(incustmer.Applied_Amount, plCur.Amounts)),
                                          Applied_Amounts = $"{c.Description} {_utility.ToCurrency(incustmer.Applied_Amount, plCur.Amounts)}",
                                          BalanceDue = Convert.ToDouble(_utility.ToCurrency(incustmer.BalanceDue, plCur.Amounts)),
                                          BalanceDues = $"{c.Description} {_utility.ToCurrency(incustmer.BalanceDue, plCur.Amounts)}",
                                          CashDiscount = Convert.ToDouble(_utility.ToCurrency(li.CashDiscount, plCur.Amounts)),
                                          CashDiscounts = $"{c.Description} {_utility.ToCurrency(li.CashDiscount, plCur.Amounts)}",
                                          TotalDiscount = Convert.ToDouble(_utility.ToCurrency(li.TotalDiscount, plCur.Amounts)),
                                          TotalDiscounts = $"{c.Description} {_utility.ToCurrency(li.TotalDiscount, plCur.Amounts)}",
                                          TotalPayment = Convert.ToDouble(_utility.ToCurrency(li.Totalpayment, plCur.Amounts)),
                                          TotalPayments = $"{c.Description} {_utility.ToCurrency(li.Totalpayment, plCur.Amounts)}",
                                          totalPay = $"{c.Description} {_utility.ToCurrency(li.Totalpayment, plCur.Amounts)}",
                                          CurrencyName = c.Description,

                                          SysName = c_s.Description,
                                          incustmer.Status,
                                          li.ExchangeRate,
                                          incustmer.SysCurrency,
                                          li.InvoiceNumber,
                                          Prefix = doc.Code,
                                          li.DocTypeID,
                                          li.ItemInvoice,
                                          DocNo = li.DocNo,
                                          CheckPay = li.CheckPay,
                                          IcoPayCusID = li.IcoPayCusID,
                                          li.LocalCurID,
                                          li.LocalSetRate,
                                          incustmer.Types,
                                      }
            };
            return Ok(objs);
        }


    }
}
