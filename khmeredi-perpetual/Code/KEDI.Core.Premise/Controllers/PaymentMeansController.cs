using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CKBS.AppContext;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.Responsitory;
using CKBS.Models.ServicesClass;
using CKBS.Models.Services.Administrator.General;
using System.Text.RegularExpressions;
using KEDI.Core.Premise.Authorization;
using System.Security.Claims;
using KEDI.Core.System.Models;

namespace CKBS.Controllers
{
    [Privilege]
    public class PaymentMeansController : Controller
    {
        private readonly IPaymentMean _context;
        private readonly DataContext _contexts;
        public PaymentMeansController(IPaymentMean context,DataContext contexts)
        {
            _context = context;
            _contexts = contexts;
        }

        [HttpGet]
        [Privilege("A015")]
        public IActionResult Index()
        {
            ViewBag.PaymentMeansBanking = "highlight";
            return View();
        }

        string RawWord(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                return Regex.Replace(keyword, "\\+s", string.Empty);
            }
            return string.Empty;
        }

        public IActionResult GetPaymentMeans(string keyword = "")
        {
            var paymentMeans = _context.PaymentMeans.Where(u => u.Delete == false);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = RawWord(keyword);
                StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
                paymentMeans = paymentMeans.Where(c => RawWord(c.Type).Contains(keyword, ignoreCase));
            }
            return Ok(paymentMeans.ToList());
        }

        [Privilege("A015")]
        public IActionResult Create()
        {            
            ViewBag.PaymentMeansBanking = "highlight";
            ViewBag.button = "fa-plus-circle";
            return View();
        }

        private int GetUserID()
        {
            int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int _id);
            return _id;
        }

        private Company GetCompany()
        {
            var com = (from us in _contexts.UserAccounts.Where(w => w.ID == GetUserID())
                       join co in _contexts.Company on us.CompanyID equals co.ID
                       select co
                       ).FirstOrDefault();
            return com;
        }

        // POST: PaymentMeans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( PaymentMeans paymentMeans)
        {       
            if (ModelState.IsValid)
            {
                try
                {
                    await _context.AddOrEdit(paymentMeans, GetCompany().ID);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {

                    ViewBag.ErrorCode = "This code already exist !";
                    return View(paymentMeans);
                }
            }
          
            return View(paymentMeans);
        }

        // GET: PaymentMeans/Edit/5
        [Privilege("A015")]
        public IActionResult Edit(int id)
        {            
            ViewBag.PaymentMeansBanking = "highlight";
            if (ModelState.IsValid)
            {
                var payment = _context.GetId(id);
                if (payment.Delete == true)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(payment);
            }
            return Ok();
        }

        // POST: PaymentMeans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PaymentMeans paymentMeans)
        {            
            ViewBag.PaymentMeansBanking = "highlight";
            var _paymentMeans = _context.PaymentMeans.AsNoTracking().ToList();
            if (id != paymentMeans.ID)
            {
                return NotFound();
            }
            if (!_contexts.SystemLicenses.Any(w => w.Edition == SystemEdition.Basic))
                 {
                if(paymentMeans.AccountID == 0)
                 {
                        ViewBag.Account = "Please choose Account !";
                        return View(paymentMeans);
                     }
                 }
           
            if (ModelState.IsValid)
            {
                foreach (var value in _paymentMeans)
                {
                    if (value.ID == paymentMeans.ID)
                    {
                        value.Default = paymentMeans.Default;
                    }
                    else
                    {
                        value.Default = false;
                    }
                    _contexts.PaymentMeans.Update(paymentMeans);
                }                
                _contexts.SaveChanges();
                await _context.AddOrEdit(paymentMeans, GetCompany().ID);
                
                return RedirectToAction(nameof(Index));
            }
            return View(_paymentMeans);
        }

        // POST: Warehouse/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _context.Delete(id);
            return Ok();
        }       
        
    }
}
