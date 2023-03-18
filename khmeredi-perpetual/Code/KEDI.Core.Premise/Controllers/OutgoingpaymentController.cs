using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Responsitory;
using KEDI.Core.Models.Validation;
using CKBS.Models.ServicesClass;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Administrator.General;
using Type = CKBS.Models.Services.Financials.Type;
using Microsoft.EntityFrameworkCore;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;
using CKBS.Models.Services.Purchase;
using KEDI.Core.Premise.Repository;
using KEDI.Core.System.Models;
using KEDI.Core.Premise.Models.ServicesClass.GeneralSettingAdminView;
using KEDI.Core.Premise.Models.GeneralSettingAdmin;
using KEDI.Core.Premise.Models.Services.Banking;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CKBS.Controllers
{
    [Privilege]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OutgoingpaymentController : Controller
    {
        private readonly DataContext _context;
        private readonly IOutgoingPayment _outgoing;
        private readonly UtilityModule _utility;

        public OutgoingpaymentController(DataContext context, IOutgoingPayment outgoing, UtilityModule utility)
        {
            _context = context;
            _outgoing = outgoing;
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
        [Privilege("A042")]
        public IActionResult Outgoingpayment()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Outgoing Payment";
            ViewBag.Subpage = "";
            ViewBag.Banking = "show";
            ViewBag.OutgoingPaymentMenu = "highlight";
            var outgoingPayment = new OutgoingPayment();
            return View(new { seriesPS = _utility.GetSeries("PS"), seriesJE = _utility.GetSeries("JE"), outgoingPayment, GeneralSetting = GetGeneralSettingAdmin().Display });
        }

        public IActionResult OutgoingpaymentHistory()
        {
            ViewBag.style = "fa-cogs";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Outgoing Payment";
            ViewBag.Subpage = "";
            ViewBag.Banking = "show";
            ViewBag.OutgoingPaymentMenu = "highlight";
            return View();
        }

        [HttpGet]
        public IActionResult GetWarehousesOutgoingPayment(int ID)
        {
            var list = _context.Warehouses.Where(x => x.BranchID == ID && x.Delete == false).ToList();
            return Ok(list);
        }

        public IActionResult OutgoingpaymentHistoryFilter(string status, string DateFrom, string DateTo)
        {
            var outgoingPayment = (from OGP in _context.OutgoingPayments.Where(i => i.CompanyID == GetCompany().ID)
                                   join OGPD in _context.OutgoingPaymentDetails on OGP.OutgoingPaymentID equals OGPD.OutgoingPaymentID
                                   join s in _context.Series on OGP.SeriesID equals s.ID
                                   join docType in _context.DocumentTypes on OGP.DocumentID equals docType.ID
                                   let _currencyName = _context.Currency.FirstOrDefault(i => i.ID == OGPD.CurrencyID).Description
                                   select new
                                   {
                                       LineID = OGP.OutgoingPaymentID,
                                       Invioce = $"{s.PreFix}-{OGP.NumberInvioce}",
                                       OGPD.ItemInvoice,
                                       Date = OGP.PostingDate.ToShortDateString(),
                                       Vendor = _context.BusinessPartners.FirstOrDefault(i => i.ID == OGP.VendorID).Name,
                                       User = _context.UserAccounts.FirstOrDefault(i => i.ID == OGP.UserID).Username,
                                       OGPD.OverdueDays,
                                       TotalAmountDue = $"{_currencyName} {OGP.TotalAmountDue:0.000}",
                                       Totalpayment = $"{_currencyName} {OGPD.Totalpayment:0.000}",
                                       OGPD.ExchangeRate,
                                       OGP.Status,
                                       Currencyname = _currencyName,
                                       OGP.CompanyID,
                                       OGP.BranchID,
                                       OGP.DocumentID,
                                       OGP.LocalCurID,
                                       OGP.LocalSetRate,
                                       OGP.PaymentMeanID,
                                       OGP.SeriesDetailID,
                                       OGP.SeriesID,
                                       OGP.UserID,
                                       OGP.VendorID,
                                   }
                                    );
            var outgoingpay = outgoingPayment.ToList();
            if (status != "all" && DateFrom != null && DateTo != null)
            {
                outgoingpay = outgoingPayment.Where(i => i.Status == status && DateTime.Parse(DateFrom) <= DateTime.Parse(i.Date) && DateTime.Parse(DateTo) >= DateTime.Parse(i.Date)).ToList();
            }
            else if (status == "all" && DateFrom != null && DateTo != null)
            {
                outgoingpay = outgoingPayment.Where(i => i.Status != status && DateTime.Parse(DateFrom) <= DateTime.Parse(i.Date) && DateTime.Parse(DateTo) >= DateTime.Parse(i.Date)).ToList();
            }
            return Ok(outgoingpay);
        }

        public IActionResult OutgoingpaymentOrderHistoryFilter(string status, string DateFrom, string DateTo)
        {
            var outgoingPaymentOrder    =   (from OGP in _context.OutgoingPaymentOrders.Where(i => i.CompanyID == GetCompany().ID)
                                        join OGPD in _context.OutgoingPaymentOrderDetails on OGP.ID equals OGPD.OutgoingPaymentOrderID
                                        join s in _context.Series on OGP.SeriesID equals s.ID
                                        join docType in _context.DocumentTypes on OGP.DocumentID equals docType.ID
                                        let _currencyName = _context.Currency.FirstOrDefault(i => i.ID == OGPD.CurrencyID).Description
                                        select new
                                        {
                                            LineID              =   OGP.ID,
                                            Invioce             =   $"{s.PreFix}-{OGP.NumberInvioce}",
                                            OGPD.ItemInvoice,
                                            Date                =   OGP.PostingDate.ToShortDateString(),
                                            Vendor              =   _context.BusinessPartners.FirstOrDefault(i => i.ID == OGP.VendorID).Name,
                                            User                =   _context.UserAccounts.FirstOrDefault(i => i.ID == OGP.UserID).Username,
                                            OGPD.OverdueDays,
                                            TotalAmountDue      =   $"{_currencyName} {OGP.TotalAmountDue:0.000}",
                                            Totalpayment        =   $"{_currencyName} {OGPD.Totalpayment:0.000}",
                                            OGPD.ExchangeRate,
                                            OGP.Status,
                                            Currencyname        =   _currencyName,
                                            OGP.CompanyID,
                                            OGP.BranchID,
                                            OGP.DocumentID,
                                            OGP.LocalCurID,
                                            OGP.LocalSetRate,
                                            OGP.PaymentMeanID,
                                            OGP.SeriesDetailID,
                                            OGP.SeriesID,
                                            OGP.UserID,
                                            OGP.VendorID,
                                        });
            var outgoingpay = outgoingPaymentOrder.ToList();
            if (status != "all" && DateFrom != null && DateTo != null)
            {
                outgoingpay = outgoingPaymentOrder.Where(i => i.Status == status && DateTime.Parse(DateFrom) <= DateTime.Parse(i.Date) && DateTime.Parse(DateTo) >= DateTime.Parse(i.Date)).ToList();
            }
            else if (status == "all" && DateFrom != null && DateTo != null)
            {
                outgoingpay = outgoingPaymentOrder.Where(i => i.Status != status && DateTime.Parse(DateFrom) <= DateTime.Parse(i.Date) && DateTime.Parse(DateTo) >= DateTime.Parse(i.Date)).ToList();
            }
            return Ok(outgoingpay);
        }   

        #region  ValidateSummaryBasic
        private void ValidateSummaryBasic(OutgoingPayment master, List<OutgoingPaymentDetail> detail)
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
            if (master.VendorID == 0)
            {
                ModelState.AddModelError("VendorID", "Please choose any vendor.");
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
            if (countCheck == 0)
            {
                ModelState.AddModelError("Check", "Please Check At Least One Invoice.");
            }
        }
        #endregion ValidateSummaryBasic



        private void ValidateSummary(OutgoingPayment master, List<OutgoingPaymentDetail> detail)
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
            if (master.VendorID == 0)
            {
                ModelState.AddModelError("VendorID", "Please choose any vendor.");
            }
            if (master.PaymentMeanID == 0)
            {
                ModelState.AddModelError("PaymentMeanID", "Please choose any Account.");
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
            if (countCheck == 0)
            {
                ModelState.AddModelError("Check", "Please Check At Least One Invoice.");
            }
        }
         #region  ValidateSummaryBasic OutgoingPaymentOrder

        private void ValidateSummaryBasicOutgoingPaymentOrder(OutgoingPaymentOrder master, List<OutgoingPaymentOrderDetail> detail)
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
            if (master.VendorID == 0)
            {
                ModelState.AddModelError("VendorID", "Please choose any vendor.");
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
            if (countCheck == 0)
            {
                ModelState.AddModelError("Check", "Please Check At Least One Invoice.");
            }
        }

        #endregion ValidateSummaryBasic OutgoingPaymentOrder
        private void ValidateSummaryOutgoingPaymentOrder(OutgoingPaymentOrder master, List<OutgoingPaymentOrderDetail> detail)
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
            if (master.VendorID == 0)
            {
                ModelState.AddModelError("VendorID", "Please choose any vendor.");
            }
            if (master.PaymentMeanID == 0)
            {
                ModelState.AddModelError("PaymentMeanID", "Please choose any Account.");
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
        private int GetBranchID()
        {
            _ = int.TryParse(User.FindFirst("BranchID").Value, out int _id);
            return _id;
        }

        [HttpGet]
        public IActionResult GetVendor()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Vendor").ToList();
            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetPaymentMeans()
        {
            var paymentMeans = (from pm in _context.PaymentMeans.Where(i => !i.Delete && i.CompanyID == GetCompany().ID)
                                join glAcc in _context.GLAccounts on pm.AccountID equals glAcc.ID
                                select new
                                {
                                    pm.ID,
                                    PMName = pm.Type,
                                    GLAccName = glAcc.Name,
                                    GLAccCode = glAcc.Code,
                                }
                               ).ToList();
            return Ok(paymentMeans);
        }

        [HttpGet]
        public IActionResult GetPaymentMeansDefault()
        {
            var paymentMeans = (from pm in _context.PaymentMeans.Where(i => i.Default == true && !i.Delete && i.CompanyID == GetCompany().ID)
                                join glAcc in _context.GLAccounts on pm.AccountID equals glAcc.ID
                                select new
                                {
                                    pm.ID,
                                    PMName = pm.Type,
                                    GLAccName = glAcc.Name,
                                    GLAccCode = glAcc.Code,
                                }
                                ).ToList();
            return Ok(paymentMeans);
        }

        [HttpGet]
        public IActionResult GetPurchaseAP(int VendorID)
        {
            var outcomepay = _context.OutgoingPaymentVendors.Where(i => i.CompanyID == GetCompany().ID && i.VendorID == VendorID && i.Status == "open").ToList();
            foreach (var obj in outcomepay)
            {
                if (obj.BalanceDue <= 0)
                {
                    obj.Status = "close";
                }
            }
            _context.UpdateRange(outcomepay);
            _context.SaveChanges();
            var list = from p in outcomepay.Where(i => i.CompanyID == GetCompany().ID && i.VendorID == VendorID && i.Status == "open" && i.BalanceDue > 0)
                       join b in _context.BusinessPartners on p.VendorID equals b.ID
                       join br in _context.Branches on p.BranchID equals br.ID
                       join w in _context.Warehouses on p.WarehouseID equals w.ID
                       join c in _context.Currency on p.CurrencyID equals c.ID
                       join c_s in _context.Currency on p.SysCurrency equals c_s.ID
                       join doc in _context.DocumentTypes on p.DocumentID equals doc.ID
                       // note whenever you add new prop, you have to put it or them at the end
                       select new
                       {
                           p.OutgoingPaymentVendorID,
                           VendorID = b.ID,
                           BranchID = br.ID,
                           WarehouseID = w.ID,
                           CurrencyID = c.ID,
                           Invoice = $"{doc.Code}-{p.Number}",
                           DocumentTypeIDValue = doc.Code,
                           Date = p.Date.ToShortDateString(),
                           OverdueDays = (p.Date.Date - DateTime.Now.Date).Days,
                           p.Total,
                           Totals = $"{c.Description} {p.Total:0.000}",
                           p.Applied_Amount,
                           Applied_Amounts = $"{c.Description} {p.Applied_Amount:0.000}",
                           p.BalanceDue,
                           BalanceDues = $"{c.Description} {p.BalanceDue:0.000}",
                           p.CashDiscount,
                           CashDiscounts = $"{c.Description} {p.CashDiscount:0.000}",
                           p.TotalDiscount,
                           TotalDiscounts = $"{c.Description} {p.TotalDiscount:0.000}",
                           p.TotalPayment,
                           TotalPayments = $"{c.Description} {p.TotalPayment:0.000}",
                           CurrencyName = c.Description,
                           SysName = c_s.Description,
                           p.Status,
                           p.ExchangeRate,
                           p.SysCurrency,
                           NumberInvioce = p.Number,
                           Prefix = doc.Code,
                           DocTypeID = p.DocumentID,
                           p.ItemInvoice,
                           DocNo = doc.Code,
                           CheckPay = false,
                           BasedOnID = p.OutgoingPaymentVendorID,
                           PurchaseType = p.TypePurchase,
                       };
            return Ok(list);
        }

        [HttpPost]
        public IActionResult SaveOutgoingPayment(string outgoing, string je)
        {
            OutgoingPayment outgoingpay = JsonConvert.DeserializeObject<OutgoingPayment>(outgoing, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            if (je == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(msg.Bind(ModelState));
            }
            Series series_JE = JsonConvert.DeserializeObject<Series>(je, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            SeriesDetail seriesDetail = new();
            var seriesPS = _context.Series.FirstOrDefault(w => w.ID == outgoingpay.SeriesID);
            var douType = _context.DocumentTypes.Where(i => i.ID == outgoingpay.DocumentID).FirstOrDefault();
            var seriesJE = _context.Series.FirstOrDefault(w => w.ID == series_JE.ID);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
            {
                ValidateSummaryBasic(outgoingpay, outgoingpay.OutgoingPaymentDetails);
            }
            else
            {
                ValidateSummary(outgoingpay, outgoingpay.OutgoingPaymentDetails);
            }

            if (ModelState.IsValid)
            {
                using var t = _context.Database.BeginTransaction();
                seriesDetail.Number = seriesPS.NextNo;
                seriesDetail.SeriesID = outgoingpay.SeriesID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();


                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDetail.Number;
                long No = long.Parse(Sno);
                seriesPS.NextNo = Convert.ToString(No + 1);
                if (No > long.Parse(seriesPS.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Outgoing Payment Invoice has reached the limitation!!");
                    return Ok(msg.Bind(ModelState));
                }

                outgoingpay.LocalCurID = GetCompany().LocalCurrencyID;
                outgoingpay.LocalSetRate = localSetRate;
                outgoingpay.SeriesDetailID = seriesDetailID;
                outgoingpay.CompanyID = GetCompany().ID;
                outgoingpay.Status = "open";
                outgoingpay.NumberInvioce = seriesDetail.Number;
                outgoingpay.Number = seriesDetail.Number;
                _context.OutgoingPayments.Add(outgoingpay);
                _context.SaveChanges();
                if (!_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                {
                    _outgoing.OutgoingPaymentSeriesAccounting(outgoingpay.OutgoingPaymentID);
                }
                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                foreach (var item in outgoingpay.OutgoingPaymentDetails.ToList())
                {
                    var doctypeCode = _context.DocumentTypes.Find(item.DocTypeID);
                    if (doctypeCode.Code == "PU")
                    {
                        //OutgoingPaymentVendors
                        var outvender = _context.OutgoingPaymentVendors.FirstOrDefault(x => x.OutgoingPaymentVendorID == item.BasedOnID);
                        outvender.Applied_Amount += item.Totalpayment + item.TotalDiscount;
                        outvender.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                        outvender.TotalPayment = outvender.BalanceDue;
                        if (outvender.Applied_Amount == outvender.Total)
                        {
                            outvender.Status = "close";
                            outgoingpay.Status = "close";
                        }
                        item.Applied_Amount = outvender.Applied_Amount;
                        item.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                        item.Delete = item.BalanceDue <= 0 ? true : false;
                        outgoingpay.TotalAmountDue = outvender.BalanceDue;
                        _context.OutgoingPayments.Update(outgoingpay);
                        _context.OutgoingPaymentVendors.Update(outvender);

                        if (outvender.TypePurchase == TypePurchase.AP)
                        {
                            var purchaseAP = _context.Purchase_APs.FirstOrDefault(x => x.SeriesDetailID == outvender.SeriesDetailID);
                            purchaseAP.AppliedAmount += item.Totalpayment + item.TotalDiscount;
                            purchaseAP.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                            if (purchaseAP.AppliedAmount == purchaseAP.SubTotal)
                            {
                                purchaseAP.Status = "close";
                                outgoingpay.Status = "close";
                            }
                            _context.Purchase_APs.Update(purchaseAP);
                            _context.SaveChanges();
                        }
                        else if (outvender.TypePurchase == TypePurchase.APReserve)
                        {
                            var purchaseAP = _context.PurchaseAPReserves.FirstOrDefault(x => x.SeriesDetailID == outvender.SeriesDetailID);
                            purchaseAP.AppliedAmount += item.Totalpayment + item.TotalDiscount;
                            purchaseAP.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                            if (purchaseAP.AppliedAmount == purchaseAP.SubTotal)
                            {
                                //purchaseAP.Status = "close";
                                outgoingpay.Status = "close";
                            }
                            _context.PurchaseAPReserves.Update(purchaseAP);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        var outvender = _context.OutgoingPaymentVendors.FirstOrDefault(x => x.OutgoingPaymentVendorID == item.BasedOnID);
                        outvender.Applied_Amount += item.Totalpayment + item.TotalDiscount;
                        outvender.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                        item.Delete = item.BalanceDue <= 0 ? true : false;
                        outvender.TotalPayment -= outvender.TotalPayment;
                        if (outvender.Applied_Amount == item.Total)
                        {
                            outvender.Status = "close";
                            outgoingpay.Status = "close";
                        }
                        item.Applied_Amount = outvender.Applied_Amount;
                        item.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                        outgoingpay.TotalAmountDue = outvender.BalanceDue;
                        _context.OutgoingPayments.Update(outgoingpay);
                        _context.OutgoingPaymentVendors.Update(outvender);

                        var purchaseMemo = _context.PurchaseCreditMemos.FirstOrDefault(x => x.SeriesDetailID == outvender.SeriesDetailID);
                        purchaseMemo.AppliedAmount += item.Totalpayment + item.TotalDiscount;
                        purchaseMemo.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                        if (purchaseMemo.AppliedAmount == purchaseMemo.SubTotal)
                        {
                            purchaseMemo.Status = "close";
                            outgoingpay.Status = "close";
                        }
                        _context.PurchaseCreditMemos.Update(purchaseMemo);
                        _context.SaveChanges();
                    }
                    var curName = _context.Currency.Find(item.CurrencyID).Description;
                    item.CurrencyName = curName;
                    _context.OutgoingPayments.Update(outgoingpay);
                    _context.SaveChanges();
                }
                _context.Series.Update(seriesPS);
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                msg.Action = ModelAction.Approve;
                ModelState.AddModelError("Success", "You have successfully paid!!");
                t.Commit();
            }
            return Ok(new { Model = msg.Bind(ModelState) });
        }

        [HttpGet]
        [Route("/outgoingpayment/getbyinvoice")]
        public IActionResult GetByInvoice(string invoiceNumber, int seriesID)
        {
            var OGP = _context.OutgoingPayments.FirstOrDefault(i => i.NumberInvioce == invoiceNumber && i.CompanyID == GetCompany().ID && i.SeriesID == seriesID && i.Status == "open");
            if (OGP != null)
            {
                var details = _context.OutgoingPaymentDetails.Include(i => i.Currency).Where(i => i.OutgoingPaymentID == OGP.OutgoingPaymentID).ToList();
                var bus = _context.BusinessPartners.FirstOrDefault(i => i.ID == OGP.VendorID);
                var user = _context.UserAccounts.FirstOrDefault(i => i.ID == OGP.UserID);
                var payMentmeans = _context.PaymentMeans.FirstOrDefault(i => i.ID == OGP.PaymentMeanID);
                var GLAccount = _context.GLAccounts.FirstOrDefault(i => i.ID == payMentmeans.AccountID);
                var outgoingPayment = new OutgoingPamentCancelViewModel
                {
                    OutgoingPaymentID = OGP.OutgoingPaymentID,
                    UserID = OGP.UserID,
                    SeriesID = OGP.SeriesID,
                    SeriesDetailID = OGP.SeriesDetailID,
                    DocumentID = OGP.DocumentID,
                    CompanyID = OGP.CompanyID,
                    NumberInvioce = OGP.NumberInvioce,
                    Status = OGP.Status,
                    PaymentMeanID = OGP.PaymentMeanID,
                    VendorID = OGP.VendorID,
                    BranchID = OGP.BranchID,
                    Ref_No = OGP.Ref_No,
                    PostingDate = OGP.PostingDate,
                    DocumentDate = OGP.DocumentDate,
                    TotalAmountDue = OGP.TotalAmountDue,
                    Number = OGP.Number,
                    Remark = OGP.Remark,
                    OutgoingPaymentDetails = details,
                    LocalCurID = OGP.LocalCurID,
                    LocalSetRate = OGP.LocalSetRate,
                    BusinessPartner = bus,
                    UserAccount = user,
                    PaymentMeans = payMentmeans,
                    GLAccount = GLAccount,
                };

                return Ok(outgoingPayment);
            }
            else
            {
                return Ok(new { Error = "Invoice Not Found!!! " });
            }
        }

        [HttpPost]
        [Route("/outgoingpaymenthistory/cancel")]
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
            var outgoing = _context.OutgoingPayments.FirstOrDefault(i => i.SeriesDetailID == seriesDID);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            var checkpay = _context.OutgoingPaymentDetails.Where(x => x.OutgoingPaymentID == outgoing.OutgoingPaymentID).ToList();
            using (var t = _context.Database.BeginTransaction())
            {
                foreach (var item in checkpay)
                {
                    var doctypeCode = _context.DocumentTypes.Find(item.DocTypeID);
                    var outgoingVendor = _context.OutgoingPaymentVendors.FirstOrDefault(i => i.OutgoingPaymentVendorID == item.BasedOnID) ?? new OutgoingPaymentVendor();
                    // Update AP
                    if (doctypeCode.Code == "PU")
                    {

                        if (outgoingVendor.TypePurchase == TypePurchase.AP)
                        {
                            var _Ap = _context.Purchase_APs.FirstOrDefault(x => x.SeriesDetailID == outgoingVendor.SeriesDetailID);
                            _Ap.AppliedAmount -= item.Totalpayment;
                            _Ap.BalanceDue += item.Totalpayment;
                            _Ap.Status = "open";
                            _context.Purchase_APs.Update(_Ap);
                        }
                        else if (outgoingVendor.TypePurchase == TypePurchase.APReserve)
                        {
                            var _Apr = _context.PurchaseAPReserves.FirstOrDefault(x => x.SeriesDetailID == outgoingVendor.SeriesDetailID);
                            _Apr.AppliedAmount -= item.Totalpayment;
                            _Apr.BalanceDue += item.Totalpayment;
                            _Apr.Status = "open";
                            _context.PurchaseAPReserves.Update(_Apr);
                        }
                    }
                    else if (doctypeCode.Code == "PC")
                    {
                        var purmemo = _context.PurchaseCreditMemos.FirstOrDefault(w => w.SeriesDetailID == outgoingVendor.SeriesDetailID) ?? new PurchaseCreditMemo();
                        purmemo.AppliedAmount += item.Totalpayment;
                        purmemo.BalanceDue -= item.Totalpayment;
                        purmemo.Status = "open";
                        _context.PurchaseCreditMemos.Update(purmemo);
                    }
                    // Update Outgoing Vendor
                    outgoingVendor.Applied_Amount -= item.Totalpayment;
                    outgoingVendor.BalanceDue += item.Totalpayment;
                    outgoingVendor.TotalPayment += item.Totalpayment;
                    outgoingVendor.Status = "open";
                    item.Delete = false;
                    _context.OutgoingPaymentDetails.Update(item);
                    _context.OutgoingPaymentVendors.Update(outgoingVendor);
                    _context.SaveChanges();
                }
                outgoing.Status = "cancel";
                _context.OutgoingPayments.Update(outgoing);
                _context.SaveChanges();
                if (!_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                {
                    _outgoing.OutgoingPaymentSeriesAccountingCancel(invoice, seriesDID);
                }
                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                msg.Action = ModelAction.Approve;
                ModelState.AddModelError("Success", $"You have successfully canceled this PS-{invoice} invoice!!");
                t.Commit();
            }
            return Ok(msg.Bind(ModelState));
        }

        //................Outgoing Payment Order.......................//
        public IActionResult OutgoingpaymentOrder(){

             ViewBag.style = "fa-cogs";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Outgoing Payment";
            ViewBag.Subpage = "";
            ViewBag.Banking = "show";
            ViewBag.OutgoingPaymentOrderMenu = "highlight";
            var outgoingPayment = new OutgoingPaymentOrder();
            return View(new { seriesPS = _utility.GetSeries("OO"), seriesJE = _utility.GetSeries("JE"), outgoingPayment, GeneralSetting = GetGeneralSettingAdmin().Display });
       

        }

        public IActionResult OutgoingpaymentOrderHistory(){
           
           ViewBag.style = "fa-cogs";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Outgoing Payment Order";
            ViewBag.Subpage = "";
            ViewBag.Banking = "show";
            ViewBag.OutgoingPaymentOrderMenu = "highlight";
            return View();
        }

        [HttpPost]
        public IActionResult SaveOutgoingPaymentOrder(string outgoing, string je)
        {
            OutgoingPaymentOrder outgoingpay = JsonConvert.DeserializeObject<OutgoingPaymentOrder>(outgoing, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            ModelMessage msg = new();
            if (je == null)
            {
                ModelState.AddModelError("SeriesJENone", "Series Journal Entry has No Data or Default One!!");
                return Ok(msg.Bind(ModelState));
            }
            Series series_JE = JsonConvert.DeserializeObject<Series>(je, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            SeriesDetail seriesDetail = new();
            var seriesPS    = _context.Series.FirstOrDefault(w => w.ID == outgoingpay.SeriesID);
            var douType     = _context.DocumentTypes.Where(i => i.ID == outgoingpay.DocumentID).FirstOrDefault();
            var seriesJE    = _context.Series.FirstOrDefault(w => w.ID == series_JE.ID);
            var localSetRate = _context.ExchangeRates.FirstOrDefault(w => w.CurrencyID == GetCompany().LocalCurrencyID).SetRate;
            if (_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
            {
                ValidateSummaryBasicOutgoingPaymentOrder(outgoingpay, outgoingpay.OutgoingPaymentOrderDetail);
            }
            else
            {
                ValidateSummaryOutgoingPaymentOrder(outgoingpay, outgoingpay.OutgoingPaymentOrderDetail);
            }

            if (ModelState.IsValid)
            {
                using var t             = _context.Database.BeginTransaction();
                seriesDetail.Number     = seriesPS.NextNo;
                seriesDetail.SeriesID   = outgoingpay.SeriesID;
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();


                var seriesDetailID = seriesDetail.ID;
                string Sno = seriesDetail.Number;
                long No = long.Parse(Sno);
                seriesPS.NextNo = Convert.ToString(No + 1);
                if (No > long.Parse(seriesPS.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Outgoing Payment Invoice has reached the limitation!!");
                    return Ok(msg.Bind(ModelState));
                }

                outgoingpay.LocalCurID = GetCompany().LocalCurrencyID;
                outgoingpay.LocalSetRate = localSetRate;
                outgoingpay.SeriesDetailID = seriesDetailID;
                outgoingpay.CompanyID = GetCompany().ID;
                outgoingpay.Status = "open";
                outgoingpay.NumberInvioce = seriesDetail.Number;
                outgoingpay.Number = seriesDetail.Number;
                _context.OutgoingPaymentOrders.Add(outgoingpay);
                _context.SaveChanges();
                // if (!_context.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                // {
                //     _outgoing.OutgoingPaymentSeriesAccounting(outgoingpay.ID);
                // }
                // checking maximun Invoice
                var douTypeID = _context.DocumentTypes.FirstOrDefault(w => w.Code == "JE");
                var defaultJE = _context.Series.FirstOrDefault(w => w.Default == true && w.DocuTypeID == douTypeID.ID);
                if (long.Parse(defaultJE.NextNo) - 1 > long.Parse(defaultJE.LastNo))
                {
                    ModelState.AddModelError("SumInvoice", "Your Invoice Journal Entry has reached the limitation!!");
                    return Ok(new { Model = msg.Bind(ModelState) });
                }
                foreach (var item in outgoingpay.OutgoingPaymentOrderDetail.ToList())
                {
                    var doctypeCode = _context.DocumentTypes.Find(item.DocTypeID);
                    if (doctypeCode.Code == "PU")
                    {
                        //OutgoingPaymentVendors
                        var outvender = _context.OutgoingPaymentVendors.AsNoTracking().FirstOrDefault(x => x.OutgoingPaymentVendorID == item.BasedOnID);
                        outvender.Applied_Amount += item.Totalpayment + item.TotalDiscount;
                        outvender.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                        outvender.TotalPayment = outvender.BalanceDue;
                       
                        item.Applied_Amount = outvender.Applied_Amount;
                        item.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                        item.Delete = item.BalanceDue <= 0 ? true : false;
                        outgoingpay.TotalAmountDue = outvender.BalanceDue;
                        _context.OutgoingPaymentOrders.Update(outgoingpay);
                        
                 
                    }
                    else
                    {
                        var outvender = _context.OutgoingPaymentVendors.AsNoTracking().FirstOrDefault(x => x.OutgoingPaymentVendorID == item.BasedOnID);
                        outvender.Applied_Amount += item.Totalpayment + item.TotalDiscount;
                        outvender.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                        item.Delete = item.BalanceDue <= 0 ? true : false;
                        outvender.TotalPayment -= outvender.TotalPayment;
                        
                        item.Applied_Amount = outvender.Applied_Amount;
                        item.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                        outgoingpay.TotalAmountDue = outvender.BalanceDue;
                        _context.OutgoingPaymentOrders.Update(outgoingpay);
                        // _context.OutgoingPaymentVendors.Update(outvender);

                        var purchaseMemo = _context.PurchaseCreditMemos.AsNoTracking().FirstOrDefault(x => x.SeriesDetailID == outvender.SeriesDetailID);
                        purchaseMemo.AppliedAmount += item.Totalpayment + item.TotalDiscount;
                        purchaseMemo.BalanceDue -= item.Totalpayment + item.TotalDiscount;
                        if (purchaseMemo.AppliedAmount == purchaseMemo.SubTotal)
                        {
                            
                            outgoingpay.Status = "close";
                        }
                        // _context.PurchaseCreditMemos.Update(purchaseMemo);
                        // _context.SaveChanges();
                    }
                    var curName = _context.Currency.Find(item.CurrencyID).Description;
                    item.CurrencyName = curName;
                    _context.OutgoingPaymentOrders.Update(outgoingpay);
                    _context.SaveChanges();
                }
                _context.Series.Update(seriesPS);
                _context.SeriesDetails.Update(seriesDetail);
                _context.SaveChanges();
                msg.Action = ModelAction.Approve;
                ModelState.AddModelError("Success", "You have successfully paid!!");
                t.Commit();
            }
            return Ok(new { Model = msg.Bind(ModelState) });
        }
        
        public IActionResult FindOutgoingPayOrder(string invoiceNumber, int seriesID )
        {
            var obj= _outgoing.FindOutgoing(0,invoiceNumber, seriesID);
            if(obj !=null)
                return Ok(obj);
            else
                return Ok(new { Error = "Invoice Not Found!!! " });
        }
        [HttpPost]
        public IActionResult CancelOutgoingPayOrder(int id,string remark)
        {
            ModelMessage msg = new();
            var obj= _context.OutgoingPaymentOrders.FirstOrDefault(s=> s.ID==id)?? new OutgoingPaymentOrder();
            if(obj.ID==0)
                    ModelState.AddModelError("", " Invoice cancel invalid ...! ");
            if(ModelState.IsValid)
            {
                obj.Status="Cancel";
                obj.Remark= string.IsNullOrWhiteSpace(remark)?obj.Remark:remark;
                _context.Update(obj);
                _context.SaveChanges();
                 msg.Action = ModelAction.Approve;
                ModelState.AddModelError("Success", $"You have successfully canceled this PS-{obj.NumberInvioce} invoice!!");
            
            }
           return Ok(msg.Bind(ModelState));
        }
        [HttpGet]
        public async Task<IActionResult> CopyOutgoingOrder(int id)
        {
            var obj =  _outgoing.FindOutgoing(id,"",0);
            var outgoingpayorder    =   new
                    {
                        OutgoingPayOrder        =   obj,
                        OutgoingPaymentDetails  =   (from li in obj.OutgoingPaymentOrderDetail
                                                    join ogpv in _context.OutgoingPaymentVendors on li.BasedOnID equals ogpv.OutgoingPaymentVendorID
                                                    join b in _context.BusinessPartners on ogpv.VendorID equals b.ID
                                                    join br in _context.Branches on ogpv.BranchID equals br.ID
                                                    join w in _context.Warehouses on ogpv.WarehouseID equals w.ID
                                                    join c in _context.Currency on ogpv.CurrencyID equals c.ID
                                                    join c_s in _context.Currency on ogpv.SysCurrency equals c_s.ID
                                                    join doc in _context.DocumentTypes on ogpv.DocumentID equals doc.ID
                                                    select new 
                                                    {
                                                        OutgoingPaymentVendorID =   li.ID,
                                                        VendorID                =   b.ID,
                                                        BranchID                =   br.ID,
                                                        WarehouseID             =   w.ID,
                                                        CurrencyID              =   c.ID,
                                                        Invoice                 =   $"{doc.Code}-{ogpv.Number}",
                                                        DocumentTypeIDValue     =   doc.Code,
                                                        Date                    =   obj.PostingDate.ToShortDateString(),
                                                        OverdueDays             =   li.OverdueDays,
                                                        ogpv.Total,
                                                        Totals                  =   ogpv.Total,
                                                        ogpv.Applied_Amount,
                                                        Applied_Amounts         =   ogpv.Applied_Amount,
                                                        ogpv.BalanceDue,
                                                        BalanceDues             =   ogpv.BalanceDue,
                                                        li.CashDiscount,
                                                        CashDiscounts           =   li.CashDiscount,
                                                        li.TotalDiscount,
                                                        TotalDiscounts          =   li.TotalDiscount,
                                                        li.Totalpayment,
                                                        TotalPayments           =   li.Totalpayment,
                                                        CurrencyName            =   c.Description,
                                                        SysName                 =   c_s.Description,
                                                    
                                                        obj.Status,
                                                        li.ExchangeRate,
                                                        ogpv.SysCurrency,
                                                        NumberInvioce           =   ogpv.Number,
                                                        Prefix                  =   doc.Code,
                                                        DocTypeID               =   ogpv.DocumentID,
                                                        ogpv.ItemInvoice,
                                                        DocNo                   =   doc.Code,
                                                        CheckPay                =   li.CheckPay,
                                                        BasedOnID               =   ogpv.OutgoingPaymentVendorID,
                                                        PurchaseType            =   ogpv.TypePurchase,
                                                    }).ToList(),
                    };
            return Ok(await Task.FromResult(outgoingpayorder));
        }

        [HttpGet]
        public async Task<IActionResult> GetOutgoingOrder(int vendorId)
        {
            var alldata = await _context.OutgoingPaymentOrders.Where(i => i.Status == "open" && i.VendorID == vendorId && i.BranchID == GetBranchID() && i.CompanyID == GetCompany().ID).ToListAsync();
                    var list=from og in alldata
                        join doc in _context.DocumentTypes on og.DocumentID equals doc.ID
                        join vd  in _context.BusinessPartners on og.VendorID equals vd.ID
                        join ogd in _context.OutgoingPaymentOrderDetails on og.ID equals ogd.OutgoingPaymentOrderID
                        group new { og, doc,vd,ogd} by new { ogd.OutgoingPaymentOrderID } into datas
                         let data   = datas.FirstOrDefault()
                         
                         let Cur    = _context.Currency.FirstOrDefault(i => i.ID == data.ogd.CurrencyID) ?? new Currency()
                         let sysCur = _context.Displays.FirstOrDefault(i => i.DisplayCurrencyID == Cur.ID) ?? new Display()
                       
                        select new  OutgoingPaymentOrder
                        {
                            //master
                            ID=data.og.ID,
                            NumberInvioce            =  data.og.Number,
                            DocumentCode             =  data.doc.Code,
                            PostingDate              =  data.og.PostingDate,
                            VendorName               =  data.vd.Name,
                            TotalAmountDue           =  data.og.TotalAmountDue,
                            TotalPayment             =  datas.Sum(s=> s.ogd.Totalpayment),
                           // DocumentDate             =  data.og.DocumentDate,
                       
                          
                        };

            return Ok(list);
        }
        
    }
}
