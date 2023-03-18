using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CKBS.AppContext;
using CKBS.Models.Services.POS.KVMS;
using KEDI.Core.Models.Validation;
using CKBS.Models.Services.Administrator.SystemInitialization;
using KEDI.Core.Premise.Authorization;
using KEDI.Core.Premise.Models.Sale;

namespace POS_WEB.Controllers
{
    [Privilege]
    public class AgingPaymentController : Controller
    {
        private readonly DataContext _context;
        
        public AgingPaymentController(DataContext context) { 
          
            _context = context;
        }
        public IActionResult AgingPayment()
        {
            ViewBag.style = "fa fa-random";
            ViewBag.Main = "Banking";
            ViewBag.Page = "Aging Payment";
            ViewBag.Subpage = "";
            ViewBag.Aging = "show";
            ViewBag.AgingPaymentMenu = "highlight";
            _ = int.TryParse(User.FindFirst("UserID").Value, out int userid);
            if (User.FindFirst("Password").Value == "YdQusX4G7SJ+txRJ2IZYDmx/L+s6SnnI4hQ+PqwCoDl09gtTubaDQiCfqhfDNYVn" && User.FindFirst("Username").Value == "Kernel")
            {
                var SysCur = GetSystemCurrencies().FirstOrDefault();
                ViewBag.SysCur = SysCur.Description;
                return View();
            }

            var permision = _context.UserPrivilleges.FirstOrDefault(x => x.UserID == userid && x.Code == "AG006");
            if (permision != null)
            {
                if (permision.Used == true)
                {
                    var SysCur = GetSystemCurrencies().FirstOrDefault();
                    ViewBag.SysCur = SysCur.Description;
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
        private IEnumerable<SystemCurrency> GetSystemCurrencies()
        {
            IEnumerable<SystemCurrency> currencies = (from com in _context.Company.Where(x => x.Delete == false)
                                                      join p in _context.PriceLists.Where(x => x.Delete == false) on com.SystemCurrencyID equals p.ID
                                                      join c in _context.Currency.Where(x => x.Delete == false) on p.CurrencyID equals c.ID
                                                      select new SystemCurrency
                                                      {
                                                          Description = c.Description
                                                      });
            return currencies;
        }
        private void ValidateSummary(AgingPayment master, List<AgingPaymentDetail> detail)
        {
            if (master.CustomerID == 0)
            {
                ModelState.AddModelError("CustomerID", "Please choose any customer.");
            }

            if (detail.Count == 0)
            {
                ModelState.AddModelError("Details", "Please select at least one invoice.");
            }
        }

        //VMC Edition
        //Save Data
        [HttpPost]
        public IActionResult SaveAgingPayment(string aging)
        {
            AgingPayment agingpay = JsonConvert.DeserializeObject<AgingPayment>(aging);
            ModelMessage msg = new ();
            ValidateSummary(agingpay, agingpay.AgingPaymentDetails);

            if (ModelState.IsValid)
            {
                //VMC
                SeriesDetail seriesDetail = new();
                var _docType = _context.DocumentTypes.FirstOrDefault(c => c.Code == "AG");
                var _seriesQ = _context.Series.FirstOrDefault(c => c.DocuTypeID == _docType.ID && c.Default);

                //insert seriesDetail
                seriesDetail.SeriesID = _seriesQ.ID;
                seriesDetail.Number = _seriesQ.NextNo;
                _context.Update(seriesDetail);
                //update series
                string Sno = _seriesQ.NextNo;
                long No = long.Parse(Sno);
                _seriesQ.NextNo = Convert.ToString(No + 1);
                _context.Update(_seriesQ);
                _context.SaveChanges();
                //update quote
                agingpay.SeriesID = _seriesQ.ID;
                agingpay.SeriesDID​ = seriesDetail.ID;
                agingpay.DocTypeID = _docType.ID;
                //END VMC

                _context.AgingPayment.Add(agingpay);
                _context.SaveChanges();
                foreach (var item in agingpay.AgingPaymentDetails.ToList())
                {
                    var balance = item.BalanceDue - item.Cash - item.DiscountValue;
                    if (balance == 0)
                    {
                        var agingcus = _context.AgingPaymentCustomer.FirstOrDefault(x => x.DocumentNo == item.DocumentNo);
                        agingcus.Status = StatusReceipt.Paid;
                        agingcus.Applied_Amount += agingcus.BalanceDue;
                        agingcus.BalanceDue = balance;
                        _context.AgingPaymentCustomer.Update(agingcus);

                        var receiptkvms = _context.ReceiptKvms.FirstOrDefault(x => x.ReceiptNo == item.DocumentNo);
                        receiptkvms.Status = StatusReceipt.Paid;
                        receiptkvms.AppliedAmount += receiptkvms.BalanceDue;
                        receiptkvms.BalanceDue = balance;
                        _context.ReceiptKvms.Update(receiptkvms);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var agingcus = _context.AgingPaymentCustomer.FirstOrDefault(x => x.DocumentNo == item.DocumentNo);
                        agingcus.Applied_Amount = agingcus.Applied_Amount + item.Cash + item.DiscountValue;
                        agingcus.BalanceDue -= (item.Cash + item.DiscountValue);
                        _context.AgingPaymentCustomer.Update(agingcus);

                        var receiptkvms = _context.ReceiptKvms.FirstOrDefault(x => x.ReceiptNo == item.DocumentNo);
                        receiptkvms.AppliedAmount = receiptkvms.AppliedAmount + item.Cash + item.DiscountValue;
                        receiptkvms.BalanceDue -= (item.Cash + item.DiscountValue);
                        _context.ReceiptKvms.Update(receiptkvms);
                        _context.SaveChanges();
                    }

                    msg.Action = ModelAction.Approve;
                }
            }
            return Ok(new { Model = msg.Bind(ModelState) });
        }

        //Get Paramater
        [HttpPost]
        public IActionResult GetAgingPaymentCus(int CusID)
        {
            var list = from apc in _context.AgingPaymentCustomer
                       join bus in _context.BusinessPartners on apc.CustomerID equals bus.ID
                       join b in _context.Branches on apc.BranchID equals b.ID
                       join wh in _context.Warehouses on apc.WarehouseID equals wh.ID
                       join cur in _context.Currency on apc.CurrencyID equals cur.ID
                       join cur_s in _context.Currency on apc.SysCurrency equals cur_s.ID
                       where apc.CustomerID == CusID && apc.Status == StatusReceipt.Aging
                       select new
                       {
                           IPCID = apc.AgingPaymentCustomerID,
                           CusID = bus.ID,
                           BranchID = b.ID,
                           CurrencyID = cur.ID,
                           apc.DocumentNo,
                           apc.DocumentType,
                           apc.Date,
                           OverdueDays = (apc.Date.Date - DateTime.Now.Date).Days,
                           apc.Total,
                           apc.BalanceDue,
                           apc.TotalPayment,
                           apc.Applied_Amount,
                           CurrencyName = cur.Description,
                           SysName = cur_s.Description,
                           apc.Status,
                           apc.DiscountRate,
                           apc.DiscountValue,
                           apc.Cash,
                           apc.ExchangeRate,
                           apc.SysCurrency
                       };
            return Ok(list);
        }
        public IActionResult GetNumberNo()
        {
            var _docType = _context.DocumentTypes.FirstOrDefault(c => c.Code == "AG");
            var _seriesQ = _context.Series.FirstOrDefault(c => c.DocuTypeID == _docType.ID && c.Default);
            var numberAging = _seriesQ.PreFix + "-" + _seriesQ.NextNo;

            //var count = _context.AgingPayment.Count() + 1;
            //var list = "APM-" + count.ToString().PadLeft(7, '0');
            return Ok(numberAging);
        }
        public IActionResult GetCustomer()
        {
            var list = _context.BusinessPartners.Where(x => x.Delete == false && x.Type == "Customer").ToList();

            return Ok(list);
        }
    }
}
